# Starts a converted corpus (after convert-corpora.ps1) on Windows, as probe-corpora.ps1 does (under supervise.ps1, from
# the assembled site if there is one), and compares it with the golden data recorded on IIS (corpora\parity, the
# scenario corpora\regression) with ParityTest; run-linux.ps1 does the same on Linux. The corpora with golden data: be,
# wt, mvcmovie (SQL Server Express: .\SQLEXPRESS; mvcmovie's databases dropped first, as they were when the golden data
# was recorded: EF 6 Code First creates them with its seed).
#   .\experiments\wf4c\verify-windows.ps1 -Name be
# -Suite operations (wt): every operation of corpora\regression\wt.operations.json (the scenario
# wt.operations.scenario.json, the golden data wt.operations.golden.json recorded by corpora\record-wt-operations.ps1),
# from a dropped database (the application's start creates it, as on IIS); per-operation results in
# experiments\wf4c\_results\wt.operations.windows.{md,json}. SQL Server Express is started if it is not running, and
# stopped again afterwards; the database and the uploaded images are removed.
#   .\experiments\wf4c\verify-windows.ps1 -Name wt -Suite operations
param([Parameter(Mandatory = $true)][ValidateSet('be', 'wt', 'mvcmovie', 'contoso')][string]$Name, [int]$Port = 5096,
    [ValidateSet('default', 'operations')][string]$Suite = 'default',
    # With -Suite operations: also crawl the converted application as each persona (ParityTest discover) and check the
    # inventory covers what it finds: corpora\regression\wt.discovered.windows.json.
    [switch]$Discover)

$ErrorActionPreference = 'Continue'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$webProjects = @{
    be       = 'be\BlogEngine\BlogEngine.NET\BlogEngine.NET.csproj'
    wt       = 'wt\WingtipToys\WingtipToys\WingtipToys.csproj'
    mvcmovie = 'mvcmovie\MvcMovie\MvcMovie.csproj'
    contoso  = 'contoso\ContosoUniversity\ContosoUniversity.csproj'
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
$fresh = @{ mvcmovie = @('MvcMovie', 'MvcMovieIdentity'); contoso = @('ContosoUniversity1') }
$scenario = Join-Path $repo "corpora\regression\$Name.scenario.json"
$golden = Join-Path $repo "corpora\parity\$Name.golden-webforms.json"
$extra = @()
$startedSql = $false
if ($Suite -eq 'operations') {
    if ($Name -ne 'wt') { throw '-Suite operations: wt only' }
    $scenario = Join-Path $repo 'corpora\regression\wt.operations.scenario.json'
    $golden = Join-Path $repo 'corpora\parity\wt.operations.golden.json'
    $fresh[$Name] = @('WingtipToys')
    if ((Get-Service 'MSSQL$SQLEXPRESS').Status -ne 'Running') { Start-Service 'MSSQL$SQLEXPRESS'; $startedSql = $true }
    $results = Join-Path $PSScriptRoot '_results'
    New-Item -ItemType Directory $results -Force | Out-Null
    $extra = @('--sql', 'Server=.\SQLEXPRESS;Database=WingtipToys;Integrated Security=true;TrustServerCertificate=true', '--platform', 'windows',
        '--mask', $env:COMPUTERNAME, '--report', (Join-Path $results 'wt.operations.windows.md'), '--results', (Join-Path $results 'wt.operations.windows.json'))
}
foreach ($database in $fresh[$Name]) {
    sqlcmd -S .\SQLEXPRESS -E -C -b -Q "IF DB_ID(N'$database') IS NOT NULL BEGIN ALTER DATABASE [$database] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$database]; END" | Out-Null
    if ($LASTEXITCODE -ne 0) { throw "could not drop $database" }
}
$log = Join-Path $directory 'verify.log'
Remove-Item $log -ErrorAction SilentlyContinue
$process = Start-Process -FilePath powershell -ArgumentList '-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', "`"$PSScriptRoot\supervise.ps1`"", '-Dll', "`"$dll`"", '-Port', $Port, '-Log', "`"$log`"" -WorkingDirectory $directory -PassThru -WindowStyle Hidden
$verifyExit = 1
try {
    foreach ($i in 1..90) { if ((Test-Path $log) -and (Select-String -Path $log -Pattern 'Now listening' -Quiet)) { break }; if ($process.HasExited) { break }; Start-Sleep 1 }
    if ($process.HasExited) { Get-Content $log -ErrorAction SilentlyContinue | Select-Object -Last 8; throw "$Name exited" }
    # The first request compiles and may restart the application: it is waited for.
    curl.exe -s -o NUL -m 300 --retry 10 --retry-connrefused --retry-delay 3 "http://localhost:$Port/" | Out-Null
    $watch = [Diagnostics.Stopwatch]::StartNew()
    & dotnet $verifier verify --url "http://localhost:$Port" --scenario $scenario --golden $golden @extra
    $verifyExit = $LASTEXITCODE
    Write-Host ("time: {0:mm\:ss}" -f $watch.Elapsed)
    if ($Discover -and $Suite -eq 'operations') {
        & dotnet $verifier discover --url "http://localhost:$Port" --out (Join-Path $repo 'corpora\regression\wt.discovered.windows.json') `
            --inventory (Join-Path $repo 'corpora\regression\wt.operations.json') --persona anonymous --persona register --persona 'login:canEditUser@wingtiptoys.com:Pa$$word1'
        if ($LASTEXITCODE -ne 0) { $verifyExit = 1 }
    }
}
finally {
    # The tree: the application runs in a worker process.
    taskkill /PID $process.Id /T /F 2>&1 | Out-Null
    Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue | ForEach-Object { Stop-Process -Id $_.OwningProcess -Force -ErrorAction SilentlyContinue }
    if ($Suite -eq 'operations') {
        # What the run left: the images the administrator uploaded, the database.
        Get-ChildItem (Join-Path $directory 'Catalog\Images') -Recurse -Filter 'parity-*' -File -ErrorAction SilentlyContinue | Remove-Item -Force
        sqlcmd -S .\SQLEXPRESS -E -C -b -Q "IF DB_ID(N'WingtipToys') IS NOT NULL BEGIN ALTER DATABASE [WingtipToys] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [WingtipToys]; END" | Out-Null
        if ($startedSql) { Stop-Service 'MSSQL$SQLEXPRESS' }
    }
}
exit $verifyExit
