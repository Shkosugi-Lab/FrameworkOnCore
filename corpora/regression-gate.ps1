# 変換出力の回帰検出。
#
# これは**パリティテストではありません。** 比較相手は変換後アプリ自身の過去のスナップショットで、
# 「元の WebForms アプリと同じか」は何も言いません。それを言えるのは tools\verify-all.ps1 が回す
# samples\*\golden-webforms.json だけで、あちらは IIS Express で旧アプリを動かして録ったものです。
#
# ここが捕まえるのは「変換器を触ったら、意図せず DOM が変わった」です。実例として、
# 未対応コントロールのスタブは可視の [asp:LoginView] を描画していて、それを HTML コメントに
# 変えたとき全ページの DOM が変わりました。意図した変更でしたが、意図しない同種の変更を
# 検出する仕組みは当時ありませんでした。
#
# 対象はビルドが通るコーパスだけです(通らないものは起動できないため)。
#
#   .\corpora\regression-gate.ps1            # 記録済みスナップショットと照合
#   .\corpora\regression-gate.ps1 -Record    # 現在の出力を新しい基準として記録
#
# -Record は**変化の理由を説明できるときだけ**使ってください。convert-all の
# -UpdateBaseline と同じ扱いです。
param(
    [switch]$Record,
    [string[]]$Only
)

$ErrorActionPreference = 'Continue'
$repo = Split-Path $PSScriptRoot -Parent
$regressionDir = Join-Path $PSScriptRoot 'regression'

# ビルドが通り、起動できるコーパスだけ。ここに足す前に convert-all でビルドエラー 0 を確認すること。
$targets = @(
    @{ Name = 'be'; Port = 5080 },
    @{ Name = 'wt'; Port = 5080 }
)

if ($Only) {
    $targets = $targets | Where-Object { $Only -contains $_.Name }
    if (-not $targets) { Write-Error "-Only に一致する対象がありません。指定可能: be, wt" }
}

$verifier = Join-Path $repo 'tools\WebForm2Blazor.ParityTest\bin\alt\WebForm2Blazor.ParityTest.dll'
if (-not (Test-Path $verifier)) {
    Write-Host '=== ParityTest のビルド ==='
    dotnet build (Join-Path $repo 'tools\WebForm2Blazor.ParityTest') `
        -o (Join-Path $repo 'tools\WebForm2Blazor.ParityTest\bin\alt') --nologo -v q | Out-Null
    if ($LASTEXITCODE -ne 0) { Write-Error 'ParityTest のビルドに失敗しました。' }
}

$failures = @()

foreach ($target in $targets) {
    $name = $target.Name
    Write-Host ""
    Write-Host ("=== {0} ===" -f $name)

    $project = Join-Path $PSScriptRoot "out\$name\$name.csproj"
    if (-not (Test-Path $project)) {
        Write-Warning "  変換出力がありません: $project(先に convert-all.ps1 を実行してください)"
        $failures += "${name}: 出力なし"
        continue
    }

    $scenario = Join-Path $regressionDir "$name.scenario.json"
    $snapshot = Join-Path $regressionDir "$name.snapshot.json"
    if (-not (Test-Path $scenario)) {
        Write-Warning "  シナリオがありません: $scenario"
        $failures += "${name}: シナリオなし"
        continue
    }

    # 実行中のプロセスが残っていると、古いバイナリに対して検証してしまう
    foreach ($other in $targets) {
        try { Stop-Process -Name $other.Name -Force -ErrorAction Stop } catch {}
    }

    # 全対象が同じポートを使うため、直前のアプリが解放するまで待つ。待たないと
    # dotnet run がバインドに失敗し、しかも死にかけの前アプリが 200 を返すので
    # 起動確認を通過してしまう。wt が ERR_CONNECTION_REFUSED で落ちたのはこれ。
    foreach ($attempt in 1..30) {
        $inUse = Get-NetTCPConnection -LocalPort $target.Port -State Listen -ErrorAction SilentlyContinue
        if (-not $inUse) { break }
        Start-Sleep -Seconds 1
    }

    dotnet build $project --nologo -v q | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Warning "  ビルドに失敗しました(ビルドが通らないコーパスは対象外です)"
        $failures += "${name}: ビルド失敗"
        continue
    }

    $process = Start-Process -FilePath dotnet `
        -ArgumentList 'run', '--project', $project, '--no-build' `
        -PassThru -WindowStyle Hidden
    try {
        $ready = $false
        foreach ($attempt in 1..45) {
            try {
                $response = Invoke-WebRequest -Uri "http://localhost:$($target.Port)/" -UseBasicParsing -TimeoutSec 5
                if ($response.StatusCode -eq 200) { $ready = $true; break }
            } catch { Start-Sleep -Seconds 2 }
        }
        if (-not $ready) {
            Write-Warning "  アプリが起動しません"
            $failures += "${name}: 起動失敗"
            continue
        }

        if ($Record) {
            & dotnet $verifier record --url "http://localhost:$($target.Port)" `
                --scenario $scenario --out $snapshot
            if ($LASTEXITCODE -ne 0) { $failures += "${name}: 記録に失敗" }
        }
        elseif (-not (Test-Path $snapshot)) {
            Write-Warning "  スナップショットがありません。-Record で記録してください: $snapshot"
            $failures += "${name}: スナップショットなし"
        }
        else {
            & dotnet $verifier verify --url "http://localhost:$($target.Port)" `
                --scenario $scenario --golden $snapshot
            if ($LASTEXITCODE -ne 0) { $failures += "${name}: 差分あり" }
        }
    }
    finally {
        Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
    }
}

Write-Host ""
if ($Record) {
    Write-Host 'スナップショットを記録しました。差分を必ず確認してから commit してください。' -ForegroundColor Yellow
    Write-Host 'これは「元アプリと同じ」ではなく「現在の変換結果」です。' -ForegroundColor Yellow
    exit 0
}

if ($failures.Count -gt 0) {
    Write-Host '要確認:' -ForegroundColor Red
    foreach ($failure in $failures) { Write-Host "  - $failure" -ForegroundColor Red }
    Write-Host ""
    Write-Host 'DOM が変わった理由を説明できる場合のみ -Record で記録し直してください。' -ForegroundColor Yellow
    exit 1
}

Write-Host '記録済みスナップショットと一致しました。' -ForegroundColor Green
exit 0
