# Starts a converted corpus (after convert-corpora.ps1) on Windows, as probe-corpora.ps1 does (under supervise.ps1, from
# the assembled site if there is one), and compares it with the golden data recorded on IIS (corpora\parity, the
# scenario corpora\regression) with ParityTest; run-linux.ps1 does the same on Linux. The corpora with golden data: be,
# wt, mvcmovie (SQL Server Express: .\SQLEXPRESS; mvcmovie's databases dropped first, as they were when the golden data
# was recorded: EF 6 Code First creates them with its seed).
#   .\experiments\wf4c\verify-windows.ps1 -Name be
param([Parameter(Mandatory = $true)][ValidateSet('be', 'wt', 'mvcmovie')][string]$Name, [int]$Port = 5096)

$ErrorActionPreference = 'Continue'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$webProjects = @{
    be       = 'be\BlogEngine\BlogEngine.NET\BlogEngine.NET.csproj'
    wt       = 'wt\WingtipToys\WingtipToys\WingtipToys.csproj'
    mvcmovie = 'mvcmovie\MvcMovie\MvcMovie.csproj'
}
$verifier = Join-Path $repo 'tools\FrameworkOnCore.ParityTest\bin\alt\FrameworkOnCore.ParityTest.dll'
& {  # built every time (incremental: quick): the verifier run is the one of this checkout
    dotnet build (Join-Path $repo 'tools\FrameworkOnCore.ParityTest') -o (Split-Path $verifier) --nologo -v q | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'FrameworkOnCore.ParityTest build failed' }
}
$project = Join-Path $PSScriptRoot $webProjects[$Name]
$directory = Split-Path $project -Parent
$assembled = Join-Path $PSScriptRoot "$Name\site"
if (Test-Path (Join-Path $assembled 'bin')) { $directory = $assembled }
[xml]$x = Get-Content $project -Raw
$assembly = ($x.Project.PropertyGroup | ForEach-Object { $_.AssemblyName } | Where-Object { $_ } | Select-Object -First 1)
$dll = Join-Path $directory "bin\$assembly.dll"
if (-not (Test-Path $dll)) { throw "$dll not built (convert-corpora.ps1)" }
$fresh = @{ mvcmovie = @('MvcMovie', 'MvcMovieIdentity') }
foreach ($database in $fresh[$Name]) {
    sqlcmd -S .\SQLEXPRESS -E -C -b -Q "IF DB_ID(N'$database') IS NOT NULL BEGIN ALTER DATABASE [$database] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$database]; END" | Out-Null
    if ($LASTEXITCODE -ne 0) { throw "could not drop $database" }
}
$log = Join-Path $directory 'verify.log'
Remove-Item $log -ErrorAction SilentlyContinue
$process = Start-Process -FilePath powershell -ArgumentList '-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', "`"$PSScriptRoot\supervise.ps1`"", '-Dll', "`"$dll`"", '-Port', $Port, '-Log', "`"$log`"" -WorkingDirectory $directory -PassThru -WindowStyle Hidden
try {
    foreach ($i in 1..90) { if ((Test-Path $log) -and (Select-String -Path $log -Pattern 'Now listening' -Quiet)) { break }; if ($process.HasExited) { break }; Start-Sleep 1 }
    if ($process.HasExited) { Get-Content $log -ErrorAction SilentlyContinue | Select-Object -Last 8; throw "$Name exited" }
    # The first request compiles and may restart the application: it is waited for.
    curl.exe -s -o NUL -m 300 --retry 10 --retry-connrefused --retry-delay 3 "http://localhost:$Port/" | Out-Null
    & dotnet $verifier verify --url "http://localhost:$Port" --scenario (Join-Path $repo "corpora\regression\$Name.scenario.json") --golden (Join-Path $repo "corpora\parity\$Name.golden-webforms.json")
}
finally {
    # The tree: the application runs in a worker process.
    taskkill /PID $process.Id /T /F 2>&1 | Out-Null
    Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue | ForEach-Object { Stop-Process -Id $_.OwningProcess -Force -ErrorAction SilentlyContinue }
}
