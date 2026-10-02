# Experiment: run an unchanged Web Forms sample on .NET 10 via WebFormsForCore and compare it
# with the golden data recorded from the original app on IIS / .NET Framework.
#
#   .\experiments\wf4c\run-sample.ps1 -Name OrderAdmin
#
# The sample's sources are copied as they are; only an SDK-style project (template.csproj.txt)
# and Program.cs (Program.cs.txt) are added.
param(
    [Parameter(Mandatory = $true)][string]$Name,
    [int]$Port = 5094,
    # Files left out of the build: sources that exercise the CONVERTER (.NET Framework APIs
    # .NET removed, e.g. DefaultsProbe's ProbeEmit.cs), which no page uses. Running them
    # unchanged is not what this experiment measures.
    [string[]]$Exclude = @(),
    # The original server's culture data (capture-culture.ps1), put in App_Data like convert-project.ps1 does.
    [string]$CultureProfile
)

$ErrorActionPreference = 'Stop'
$repo = Resolve-Path (Join-Path $PSScriptRoot '..\..')
$sample = Join-Path $repo "samples\$Name"
$work = Join-Path $PSScriptRoot $Name
$verifier = Join-Path $repo 'tools\FrameworkOnCore.ParityTest\bin\alt\FrameworkOnCore.ParityTest.dll'
# The parity verifier (Playwright), built once into bin\alt (a separate output: a running copy does not lock the build).
& {  # built every time (incremental: quick): the verifier run is the one of this checkout
    dotnet build (Join-Path $repo 'tools\FrameworkOnCore.ParityTest') -o (Split-Path $verifier) --nologo -v q | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'FrameworkOnCore.ParityTest build failed' }
}

if (Test-Path $work) { Remove-Item $work -Recurse -Force }
Copy-Item $sample $work -Recurse
Remove-Item (Join-Path $work 'golden*.json'), (Join-Path $work 'parity-scenario.json') -ErrorAction SilentlyContinue
Move-Item (Join-Path $work "$Name.csproj") (Join-Path $work "$Name.csproj.netfx")
(Get-Content (Join-Path $PSScriptRoot 'template.csproj.txt') -Raw -Encoding UTF8).Replace('__NAME__', $Name) |
    Set-Content (Join-Path $work "$Name.csproj") -Encoding UTF8
Copy-Item (Join-Path $PSScriptRoot 'Program.cs.txt') (Join-Path $work 'Program.cs')
foreach ($file in $Exclude) { Remove-Item (Join-Path $work $file) }
if ($CultureProfile) {
    New-Item -ItemType Directory (Join-Path $work 'App_Data') -Force | Out-Null
    Copy-Item $CultureProfile (Join-Path $work 'App_Data\culture-profile.json')
}

Push-Location $work
try {
    dotnet build -v q -nologo
    if ($LASTEXITCODE -ne 0) { throw "build failed" }

    $env:ASPNETCORE_URLS = "http://localhost:$Port"
    $process = Start-Process -FilePath dotnet -ArgumentList "bin\$Name.dll" -PassThru -WindowStyle Hidden `
        -RedirectStandardOutput (Join-Path $work 'server.log') -RedirectStandardError (Join-Path $work 'server.err.log')
    try {
        foreach ($attempt in 1..60) {
            try { Invoke-WebRequest "http://localhost:$Port/" -UseBasicParsing -TimeoutSec 60 | Out-Null; break }
            catch { if ($_.Exception.Response) { break }; Start-Sleep -Seconds 2 }
        }
        & dotnet $verifier verify --url "http://localhost:$Port" `
            --scenario (Join-Path $sample 'parity-scenario.json') --golden (Join-Path $sample 'golden-webforms.json')
    }
    finally {
        taskkill /PID $process.Id /T /F 2>&1 | Out-Null  # the tree: the application runs in a worker process
    }
}
finally {
    Pop-Location
}
