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
    [string[]]$Exclude = @()
)

$ErrorActionPreference = 'Stop'
$repo = Resolve-Path (Join-Path $PSScriptRoot '..\..')
$sample = Join-Path $repo "samples\$Name"
$work = Join-Path $PSScriptRoot $Name
$verifier = Join-Path $repo 'tools\WebForm2Blazor.ParityTest\bin\alt\WebForm2Blazor.ParityTest.dll'

if (Test-Path $work) { Remove-Item $work -Recurse -Force }
Copy-Item $sample $work -Recurse
Remove-Item (Join-Path $work 'golden*.json'), (Join-Path $work 'parity-scenario.json') -ErrorAction SilentlyContinue
Move-Item (Join-Path $work "$Name.csproj") (Join-Path $work "$Name.csproj.netfx")
(Get-Content (Join-Path $PSScriptRoot 'template.csproj.txt') -Raw -Encoding UTF8).Replace('__NAME__', $Name) |
    Set-Content (Join-Path $work "$Name.csproj") -Encoding UTF8
Copy-Item (Join-Path $PSScriptRoot 'Program.cs.txt') (Join-Path $work 'Program.cs')
foreach ($file in $Exclude) { Remove-Item (Join-Path $work $file) }

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
        Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
    }
}
finally {
    Pop-Location
}
