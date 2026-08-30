# Parity gate for the AI residual layer.
#
# A rebuilt assembly is not enough: the converted app has to be restarted, or the
# verification would run against the previously loaded binary and pass on stale code.
# Exits 0 only when every snapshot matches the golden master.
param(
    [Parameter(Mandatory = $true)][string]$Project,
    [Parameter(Mandatory = $true)][string]$ProcessName,
    [Parameter(Mandatory = $true)][int]$Port,
    [Parameter(Mandatory = $true)][string]$Scenario,
    [Parameter(Mandatory = $true)][string]$Golden
)

$ErrorActionPreference = 'Continue'

try { Stop-Process -Name $ProcessName -Force -ErrorAction Stop } catch {}
Start-Sleep -Seconds 2

dotnet build $Project --nologo -v q | Out-Null
if ($LASTEXITCODE -ne 0) {
    Write-Output 'RESULT: FAIL (ビルド失敗)'
    exit 1
}

Start-Process -FilePath dotnet -ArgumentList 'run', '--project', $Project, '--no-build' -WindowStyle Hidden
$ready = $false
foreach ($i in 1..60) {
    try {
        $response = Invoke-WebRequest -Uri "http://127.0.0.1:$Port/" -UseBasicParsing -TimeoutSec 5
        if ($response.StatusCode -eq 200) { $ready = $true; break }
    } catch { Start-Sleep -Seconds 2 }
}
if (-not $ready) {
    Write-Output 'RESULT: FAIL (アプリが起動しません)'
    exit 1
}

$verifier = Join-Path $PSScriptRoot 'WebForm2Blazor.ParityTest\bin\alt\WebForm2Blazor.ParityTest.dll'
dotnet $verifier verify --url "http://localhost:$Port" --scenario $Scenario --golden $Golden
exit $LASTEXITCODE
