# 変換後のコーパスを、変換前アプリの実描画と照合する。
#
# regression-gate.ps1 との違い
# ----------------------------
# あちらの比較相手は変換後アプリ自身の過去のスナップショットで、「変換器を触ったら
# 意図せず DOM が変わった」を捕まえます。「元と同じか」は何も言いません。
#
# こちらの比較相手は corpora\parity\<name>.golden-webforms.json ——
# IIS で動かした変換前アプリの実描画です。これが「元と同じか」を言う唯一のゲートで、
# 残差レポートもコンパイルエラーも構造的に見られない層を見ます。
#
# いまは落ちます
# --------------
# be は 5 スナップショット中 5 つが不一致です。原因は特定済みで、BlogEngine が
# マスターページを実行時に決めている(BlogBasePage.GetSiteMaster)のに対し、変換器は
# レイアウトを静的に束ねるため、テーマが一度も適用されません。詳細は corpora\README.md。
#
# 通るまでベースラインは置きません。「いま何件落ちているか」を固定すると、それは
# 「元と同じ」ではなく「いつもの壊れ方」を守るゲートになります。
#
#   .\corpora\parity-gate.ps1
#   .\corpora\parity-gate.ps1 -Only be
param(
    [string[]]$Only
)

$ErrorActionPreference = 'Continue'
$repo = Split-Path $PSScriptRoot -Parent
$parityDir = Join-Path $PSScriptRoot 'parity'

# 正解データがあるものだけ。採取は record-webforms-golden.ps1。
$targets = @(
    @{ Name = 'be'; Port = 5080 }
)

if ($Only) {
    $targets = $targets | Where-Object { $Only -contains $_.Name }
    if (-not $targets) { Write-Error "-Only に一致する対象がありません。指定可能: be"; exit 1 }
}

$verifier = Join-Path $repo 'tools\WebForm2Blazor.ParityTest\bin\alt\WebForm2Blazor.ParityTest.dll'
if (-not (Test-Path $verifier)) {
    Write-Host '=== ParityTest のビルド ==='
    dotnet build (Join-Path $repo 'tools\WebForm2Blazor.ParityTest') `
        -o (Join-Path $repo 'tools\WebForm2Blazor.ParityTest\bin\alt') --nologo -v q | Out-Null
}

$failures = @()

foreach ($target in $targets) {
    $name = $target.Name
    $project = Join-Path $PSScriptRoot "out\$name\$name.csproj"
    $scenario = Join-Path $PSScriptRoot "regression\$name.scenario.json"
    $golden = Join-Path $parityDir "$name.golden-webforms.json"

    Write-Host ""
    Write-Host ("=== {0} ===" -f $name)

    if (-not (Test-Path $golden)) {
        Write-Warning "  正解データがありません: $golden(record-webforms-golden.ps1 で採取してください)"
        $failures += "${name}: 正解データなし"
        continue
    }
    if (-not (Test-Path $project)) {
        Write-Warning "  変換出力がありません: $project(先に convert-all.ps1 を実行してください)"
        $failures += "${name}: 出力なし"
        continue
    }

    # 実行中のプロセスが残っていると、古いバイナリに対して照合してしまう
    try { Stop-Process -Name $name -Force -ErrorAction Stop } catch {}
    foreach ($attempt in 1..30) {
        $inUse = Get-NetTCPConnection -LocalPort $target.Port -State Listen -ErrorAction SilentlyContinue
        if (-not $inUse) { break }
        Start-Sleep -Seconds 1
    }

    dotnet build $project --nologo -v q | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Warning "  ビルドに失敗しました"
        $failures += "${name}: ビルド失敗"
        continue
    }

    $process = Start-Process -FilePath dotnet `
        -ArgumentList 'run', '--project', $project, '--no-build' -PassThru -WindowStyle Hidden
    try {
        $ready = $false
        foreach ($attempt in 1..45) {
            try {
                if ((Invoke-WebRequest -Uri "http://localhost:$($target.Port)/" `
                        -UseBasicParsing -TimeoutSec 5).StatusCode -eq 200) { $ready = $true; break }
            } catch { Start-Sleep -Seconds 2 }
        }
        if (-not $ready) {
            Write-Warning "  変換後アプリが起動しません"
            $failures += "${name}: 起動失敗"
            continue
        }

        & dotnet $verifier verify --url "http://localhost:$($target.Port)" `
            --scenario $scenario --golden $golden
        if ($LASTEXITCODE -ne 0) { $failures += "${name}: 変換前と差分あり" }
    }
    finally {
        Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
    }
}

Write-Host ""
if ($failures.Count -gt 0) {
    Write-Host '変換前アプリと一致していません:' -ForegroundColor Red
    foreach ($failure in $failures) { Write-Host "  - $failure" -ForegroundColor Red }
    Write-Host ""
    Write-Host 'これは既知の状態です。件数を固定して「合格」にしないでください。' -ForegroundColor Yellow
    Write-Host '差分の読み方と現在の原因は corpora\README.md を参照してください。' -ForegroundColor Yellow
    exit 1
}

Write-Host '変換前アプリと一致しました。' -ForegroundColor Green
exit 0
