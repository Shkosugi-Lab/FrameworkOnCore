# Full regression for WebForm2Blazor.
#
# Encodes the operating rules this project learned the hard way:
#   - running apps hold their exe and make later builds fail with MSB3027, so every
#     converted app is stopped first
#   - the parity harness must run against a freshly started app, or it verifies the
#     previously loaded binary
#   - samples carry state (a deleted row, an added order), so each app is restarted
#     before its scenario runs
#
# Exit code 0 only when conversion, build, bUnit and every parity scenario pass.
param(
    [switch]$SkipParity
)

$ErrorActionPreference = 'Continue'
$root = Split-Path $PSScriptRoot -Parent
Set-Location $root

$converter = 'src\WebForm2Blazor.Converter\bin\alt\WebForm2Blazor.Converter.dll'
$parityTest = 'tools\WebForm2Blazor.ParityTest\bin\alt\WebForm2Blazor.ParityTest.dll'
$componentsRef = '..\..\src\WebForm2Blazor.Components\WebForm2Blazor.Components.csproj'

$samples = @(
    @{ Name = 'DefaultsProbe'; App = 'DefaultsProbeBlazor'; Port = 5096; Parity = $true },
    @{ Name = 'MasterProbe'; App = 'MasterProbeBlazor'; Port = 5097; Parity = $true },
    @{ Name = 'HelloWebForms'; App = 'HelloBlazor'; Port = 5080; Parity = $false },
    @{ Name = 'ProductAdmin'; App = 'ProductAdminBlazor'; Port = 5090; Parity = $true },
    @{ Name = 'OrderAdmin'; App = 'OrderAdminBlazor'; Port = 5095; Parity = $true }
)

$failures = @()

function Stop-Apps {
    foreach ($sample in $samples) {
        try { Stop-Process -Name $sample.App -Force -ErrorAction Stop } catch {}
    }
    Start-Sleep -Seconds 2
}

Write-Host '=== 0. 実行中アプリの停止 ==='
Stop-Apps

Write-Host '=== 1. 互換コンポーネントと変換器のビルド ==='
dotnet build src\WebForm2Blazor.Components --nologo -v q | Out-Null
if ($LASTEXITCODE -ne 0) { $failures += 'components build'; }
dotnet build src\WebForm2Blazor.Converter -o src\WebForm2Blazor.Converter\bin\alt --nologo -v q | Out-Null
if ($LASTEXITCODE -ne 0) { $failures += 'converter build'; }
dotnet build tools\WebForm2Blazor.ParityTest -o tools\WebForm2Blazor.ParityTest\bin\alt --nologo -v q | Out-Null
if ($LASTEXITCODE -ne 0) { $failures += 'paritytest build'; }

Write-Host '=== 2. サンプルの変換 ==='
foreach ($sample in $samples) {
    dotnet $converter --input "samples\$($sample.Name)" --output "output\$($sample.App)" `
        --name $sample.App --components-ref $componentsRef --port $sample.Port | Out-Null
    if ($LASTEXITCODE -ne 0) { $failures += "convert $($sample.Name)" }
}

Write-Host '=== 3. bUnit ==='
$testOutput = dotnet test tests\WebForm2Blazor.ConvertedAppTests --nologo 2>&1 | Out-String
if ($LASTEXITCODE -ne 0) { $failures += 'bUnit' }
($testOutput -split "`r?`n" | Where-Object { $_ -match '合格|失敗' } | Select-Object -Last 1)

Write-Host '=== 4. 変換出力のビルド検証 ==='
foreach ($sample in $samples) {
    $result = dotnet $converter --verify-build "output\$($sample.App)" 2>&1 | Out-String
    $line = ($result -split "`r?`n" | Where-Object { $_ -match 'エラー' } | Select-Object -First 1)
    Write-Host "  $($sample.App): $line"
    if ($LASTEXITCODE -ne 0) { $failures += "build-verify $($sample.App)" }
}

if (-not $SkipParity) {
    Write-Host '=== 5. パリティ(旧アプリのゴールデンマスター照合) ==='
    foreach ($sample in $samples | Where-Object { $_.Parity }) {
        # A fresh process per scenario: these samples mutate state as the scenario runs
        try { Stop-Process -Name $sample.App -Force -ErrorAction Stop } catch {}
        Start-Sleep -Seconds 2
        Start-Process -FilePath dotnet -ArgumentList 'run', '--project', "output\$($sample.App)", '--no-build' -WindowStyle Hidden

        $ready = $false
        foreach ($attempt in 1..60) {
            try {
                $response = Invoke-WebRequest -Uri "http://127.0.0.1:$($sample.Port)/" -UseBasicParsing -TimeoutSec 5
                if ($response.StatusCode -eq 200) { $ready = $true; break }
            } catch { Start-Sleep -Seconds 2 }
        }
        if (-not $ready) {
            Write-Host "  $($sample.Name): アプリが起動しません"
            $failures += "parity $($sample.Name) (起動失敗)"
            continue
        }

        $verify = dotnet $parityTest verify --url "http://localhost:$($sample.Port)" `
            --scenario "samples\$($sample.Name)\parity-scenario.json" `
            --golden "samples\$($sample.Name)\golden-webforms.json" 2>&1 | Out-String
        $result = ($verify -split "`r?`n" | Where-Object { $_ -match '^RESULT' } | Select-Object -Last 1)
        Write-Host "  $($sample.Name): $result"
        if ($result -notmatch 'OK') { $failures += "parity $($sample.Name)" }
    }
    Stop-Apps
}

Write-Host ''
if ($failures.Count -eq 0) {
    Write-Host 'すべて成功しました。'
    exit 0
}

Write-Host "失敗: $($failures -join ', ')"
exit 1
