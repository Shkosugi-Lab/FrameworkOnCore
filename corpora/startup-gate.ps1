# 変換後アプリが「そのまま起動して、例外なく描画できるか」を見るゲート。
#
# 他のゲートとの違い
# ------------------
# convert-all の --verify-build は【コンパイルが通るか】しか見ません。
# regression-gate は【変換後アプリ同士の DOM 比較】、parity-gate は【元アプリとの照合】で、
# どちらも正解データを持つ be / wt にしか適用できません。
#
# その 3 つの間に穴がありました:【ビルドは通るが、起動すると壊れている】です。
#
# 実例(このゲートを作る動機になったもの):
#   wt はビルドエラー 0、回帰ゲート 8/8 OK。しかし実際には全ページでこれが出ていました。
#
#     Unhandled exception in circuit:
#       Could not resolve type 'System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection'
#       in assembly 'wt'
#
#   App.config の型書き換えが EntityFramework まで潰し、Blazor の回線が毎ページ死んで
#   いました。SSR(プリレンダリング)の HTML は正常に返るので、Invoke-WebRequest でも
#   気づけません。回帰ゲートが 8/8 OK と言っていたのは、死んだ回線の下で静的 HTML だけが
#   一致していたからです。
#
# だからこのゲートは 2 つを見ます:
#   1. 起動して HTTP 200 を返すか
#   2. サーバーのログに【未処理例外が出ていないか】 ← こちらが本命
#
# 2 のほうが重要です。1 だけなら既存の parity-gate でも間接的に分かりますが、
# 「200 を返しながら回線が死んでいる」は 1 では絶対に捕まりません。
#
# 対象
# ----
# ビルドエラー 0 のコーパスだけです。通らないものは起動できません
# (現状 be / wt。yaf 14・dnn 14・n2 6・mojo 320 は対象外)。
# ビルドエラーが 0 になったコーパスは、ここに足してください。
#
#   .\corpora\startup-gate.ps1
#   .\corpora\startup-gate.ps1 -Only be
param(
    [string[]]$Only,
    # 例外が出たときに、ログの該当部分をそのまま出します。
    [switch]$ShowDetail,
    [string]$Configuration = 'Debug'
)

$ErrorActionPreference = 'Continue'
$repo = Split-Path $PSScriptRoot -Parent

# ビルドが通るコーパスだけ。足す前に convert-all でビルドエラー 0 を確認すること。
$targets = @(
    @{ Name = 'be'; Port = 5080; Paths = @('/', '/archive', '/contact') },
    @{ Name = 'wt'; Port = 5080; Paths = @('/', '/About', '/ProductList', '/ShoppingCart') }
)

if ($Only) {
    $targets = $targets | Where-Object { $Only -contains $_.Name }
    if (-not $targets) { Write-Error "-Only に一致する対象がありません。指定可能: be, wt"; exit 1 }
}

# 回線が落ちたときにサーバーが書く行。Blazor Server はここに出してから接続を切ります。
#
# "Exception" だけで拾わないのは、アプリが意図して握りつぶした例外までログに出ることが
# あるためです(BlogEngine の Utils.Log がそれ)。ここで見たいのは
# 【フレームワークが処理しきれなかった】ものだけです。
$fatalPatterns = @(
    'Unhandled exception in circuit',
    'Unhandled exception rendering component',
    'An unhandled exception has occurred while executing the request',
    'Unhandled exception\.'
)

$failures = @()

foreach ($target in $targets) {
    $name = $target.Name
    $project = Join-Path $PSScriptRoot "out\$name\$name.csproj"

    Write-Host ""
    Write-Host ("=== {0} ===" -f $name)

    if (-not (Test-Path $project)) {
        Write-Warning "  変換出力がありません: $project(先に convert-all.ps1 を実行してください)"
        $failures += "${name}: 出力なし"
        continue
    }

    # 実行中のプロセスが残っていると、古いバイナリに対して測ってしまう
    try { Stop-Process -Name $name -Force -ErrorAction Stop } catch {}
    foreach ($attempt in 1..30) {
        $inUse = Get-NetTCPConnection -LocalPort $target.Port -State Listen -ErrorAction SilentlyContinue
        if (-not $inUse) { break }
        Start-Sleep -Seconds 1
    }

    # App.config は「変換出力を直したのに bin の .dll.config が古いまま」になりやすく、
    # そのときこのゲートは【嘘をつきます】。実際、EF の不具合を再現 → 修正して戻したとき、
    # ソースは直っているのに bin\...\wt.dll.config が古く、失敗し続けました。
    # MSBuild は中身が変わっていないと見なすと配置し直しません。先に落とします。
    $staleConfig = Join-Path (Split-Path $project -Parent) "bin\$Configuration\net10.0\$name.dll.config"
    if (Test-Path $staleConfig) { Remove-Item $staleConfig -Force -ErrorAction SilentlyContinue }

    Write-Host '  ビルド'
    & dotnet build $project --nologo -v q | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Warning "  ビルドに失敗しました(起動できません)"
        $failures += "${name}: ビルド失敗"
        continue
    }

    # 消したものが戻っていること、そして変換出力と同じ中身であることを確かめます。
    $sourceConfig = Join-Path (Split-Path $project -Parent) 'App.config'
    if ((Test-Path $sourceConfig) -and (Test-Path $staleConfig)) {
        $same = (Get-FileHash $sourceConfig).Hash -eq (Get-FileHash $staleConfig).Hash
        if (-not $same) {
            Write-Warning "  App.config と $name.dll.config が一致しません(古い成果物で測ることになります)"
            $failures += "${name}: 構成ファイルが不一致"
            continue
        }
    }

    $stdout = Join-Path $PSScriptRoot "out\$name-startup.log"
    $stderr = Join-Path $PSScriptRoot "out\$name-startup-err.log"
    Remove-Item $stdout, $stderr -ErrorAction SilentlyContinue

    Write-Host '  起動'
    $process = Start-Process -FilePath dotnet `
        -ArgumentList 'run', '--project', $project, '--no-build' `
        -PassThru -WindowStyle Hidden `
        -RedirectStandardOutput $stdout -RedirectStandardError $stderr

    try {
        $ready = $false
        foreach ($attempt in 1..45) {
            try {
                if ((Invoke-WebRequest -Uri "http://localhost:$($target.Port)/" `
                        -UseBasicParsing -TimeoutSec 5).StatusCode -eq 200) { $ready = $true; break }
            }
            catch { Start-Sleep -Seconds 2 }
        }
        if (-not $ready) {
            Write-Warning "  起動しません"
            $failures += "${name}: 起動失敗"
            continue
        }

        # SSR だけを見ても意味がありません。回線を張って初めて動くコード
        # (OnAfterRender から呼ばれる Page_Load / PreRender)がこの変換器の主戦場で、
        # 壊れるのもそこだからです。だから実ブラウザで踏みます。
        $verifier = Join-Path $repo 'tools\WebForm2Blazor.ParityTest\bin\alt\WebForm2Blazor.ParityTest.dll'
        $scenario = Join-Path $PSScriptRoot "regression\$name.scenario.json"
        if ((Test-Path $verifier) -and (Test-Path $scenario)) {
            Write-Host '  実ブラウザで巡回'
            $golden = Join-Path $PSScriptRoot "regression\$name.snapshot.json"
            & dotnet $verifier verify --url "http://localhost:$($target.Port)" `
                --scenario $scenario --golden $golden 2>&1 | Out-Null
        }
        else {
            # 検証器が無くても、起動確認だけは HTTP で済ませます。
            Write-Host '  HTTP で巡回'
            foreach ($path in $target.Paths) {
                try { Invoke-WebRequest -Uri "http://localhost:$($target.Port)$path" `
                        -UseBasicParsing -TimeoutSec 30 | Out-Null } catch {}
            }
        }

        # 落ちた回線はすぐには書き出されないことがあるので、少し待ってから読みます。
        Start-Sleep -Seconds 2
    }
    finally {
        Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
    }

    $log = @()
    foreach ($file in @($stdout, $stderr)) {
        if (Test-Path $file) { $log += Get-Content $file -Encoding UTF8 -ErrorAction SilentlyContinue }
    }

    # 例外の【型】は "Unhandled exception ..." の行ではなく、その直後の行に出ます。
    # 一致行だけを見て分類すると全部「種類不明」になりました(実測)。
    # 見出し行から数行ぶんを一緒に拾い、そこから型名を探します。
    $fatal = @()
    for ($i = 0; $i -lt $log.Count; $i++) {
        $line = $log[$i]
        $isFatal = @($fatalPatterns | Where-Object { $line -match $_ }).Count -gt 0
        if (-not $isFatal) { continue }

        $window = $log[$i..([Math]::Min($i + 3, $log.Count - 1))] -join ' '
        $fatal += [PSCustomObject]@{
            Line = $line
            Kind = if ($window -match '([A-Za-z0-9_.]+Exception)') { $Matches[1] } else { '(種類不明)' }
        }
    }

    if ($fatal.Count -gt 0) {
        Write-Warning ("  未処理例外 {0} 件" -f $fatal.Count)
        # 種類ごとに 1 行。同じ例外が全ページで出るので、件数より種類が読みたい。
        foreach ($kind in ($fatal | Group-Object Kind | Sort-Object Count -Descending)) {
            Write-Host ("    {0} x {1}" -f $kind.Count, $kind.Name) -ForegroundColor Red
        }

        if ($ShowDetail) {
            Write-Host ""
            $fatal | Select-Object -First 3 | ForEach-Object {
                Write-Host ("    | " + $_.Line.Trim()) -ForegroundColor DarkGray
            }
        }

        $failures += "${name}: 未処理例外 $($fatal.Count) 件"
        continue
    }

    Write-Host ("  OK: 起動して例外なく巡回しました(ログ {0} 行)" -f $log.Count) -ForegroundColor Green
}

Write-Host ""
if ($failures.Count -gt 0) {
    Write-Host '起動ゲートに失敗しました:' -ForegroundColor Red
    foreach ($failure in $failures) { Write-Host "  - $failure" -ForegroundColor Red }
    Write-Host ""
    Write-Host 'ビルドが通っても、起動して壊れていれば変換は完了していません。' -ForegroundColor Yellow
    Write-Host '詳細は -ShowDetail、全文は corpora\out\<name>-startup.log を見てください。' -ForegroundColor Yellow
    exit 1
}

Write-Host '全ての対象が起動し、未処理例外はありませんでした。' -ForegroundColor Green
exit 0
