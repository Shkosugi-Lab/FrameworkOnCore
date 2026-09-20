# 変換前の WebForms アプリを IIS で動かし、その実描画をゴールデンマスターとして採る。
#
# なぜ要るか
# ----------
# このプロジェクトの最終ゲートは ParityTest — 変換前アプリの実際の描画との照合 — で、
# その正解は samples\ の 4 本にしかありませんでした。6 本のコーパスには 1 本も無く、
# つまり「BlogEngine はビルドエラー 0」は「コンパイルが通る」であって「元と同じ挙動」
# ではありません。残差レポートもコンパイルエラーも「書かれているもの」しか捕捉できず、
# 既定レンダリングの漏れは旧アプリの実描画と突き合わせる以外に検出手段がありません。
#
# なぜ IIS Express ではないか
# ---------------------------
# 単体インストーラは配布が Visual Studio 同梱に寄っていて、この環境にはありません。
# Windows 同梱の IIS は機能として入っており、同じ System.Web パイプラインを動かします。
# 手製ホスト(tools\legacy\WebFormsHost)で採る道は採りません —— 手製ホストで採った
# 正解は「WebForms の描画」ではなく「そのホストの癖」を記録するためで、既存の
# samples のゴールデンと出自が揃いません。
#
# 使い方
#   .\corpora\record-webforms-golden.ps1 -Only be
#   .\corpora\record-webforms-golden.ps1 -Only be -KeepSite   # 調査用に残す
param(
    [string[]]$Only,
    [switch]$KeepSite
)

$ErrorActionPreference = 'Continue'
$repo = Split-Path $PSScriptRoot -Parent
$parityDir = Join-Path $PSScriptRoot 'parity'

# 旧アプリを動かせると実測できたものだけ。足すときは実際に採ってから。
#
# wt は入っていません: Web.config が実在しない Azure SQL(ms.database.windows.net、
# 資格情報も伏せ字)を指す匿名化済みサンプルで、データベースごと用意しないと動きません。
# mojo / yaf / dnn も同様にデータベースが要ります。
$targets = @(
    @{ Name = 'be'
       Path = 'BlogEngine.NET-3.3.8.0\BlogEngine\BlogEngine.NET'
       Port = 8091 }
)

if ($Only) {
    $targets = $targets | Where-Object { $Only -contains $_.Name }
    if (-not $targets) { Write-Error "-Only に一致する対象がありません。指定可能: be"; exit 1 }
}

$appcmd = Join-Path $env:SystemRoot 'System32\inetsrv\appcmd.exe'
if (-not (Test-Path $appcmd)) {
    Write-Error @"
IIS がありません($appcmd)。
有効化:
  Enable-WindowsOptionalFeature -Online -All -NoRestart -FeatureName `
    IIS-WebServerRole,IIS-ASPNET45,IIS-NetFxExtensibility45,IIS-ISAPIExtensions,IIS-ISAPIFilter
"@
    exit 1
}

$verifier = Join-Path $repo 'tools\WebForm2Blazor.ParityTest\bin\alt\WebForm2Blazor.ParityTest.dll'
if (-not (Test-Path $verifier)) {
    Write-Host '=== ParityTest のビルド ==='
    dotnet build (Join-Path $repo 'tools\WebForm2Blazor.ParityTest') `
        -o (Join-Path $repo 'tools\WebForm2Blazor.ParityTest\bin\alt') --nologo -v q | Out-Null
}

New-Item -ItemType Directory -Force $parityDir | Out-Null
$failures = @()

foreach ($target in $targets) {
    $name = $target.Name
    $siteName = "webform2blazor-$name"
    $poolName = "webform2blazor-$name"
    $physical = Join-Path $PSScriptRoot ("work\" + $target.Path)
    $scenario = Join-Path $PSScriptRoot "regression\$name.scenario.json"
    $golden = Join-Path $parityDir "$name.golden-webforms.json"

    Write-Host ""
    Write-Host ("=== {0} ===" -f $name)

    if (-not (Test-Path $scenario)) {
        Write-Warning "  シナリオがありません: $scenario"
        $failures += "${name}: シナリオなし"
        continue
    }

    Write-Host '  変換前アプリをビルド'
    & (Join-Path $PSScriptRoot 'build-original.ps1') -Only $name | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Warning "  ビルドに失敗しました"
        $failures += "${name}: ビルド失敗"
        continue
    }

    # 前回の残骸があると、古いパスのサイトに対して採ってしまう
    & $appcmd delete site $siteName 2>&1 | Out-Null
    & $appcmd delete apppool $poolName 2>&1 | Out-Null

    Write-Host '  IIS サイトを作成'
    & $appcmd add apppool /name:$poolName /managedRuntimeVersion:v4.0 /managedPipelineMode:Integrated | Out-Null
    & $appcmd add site /name:$siteName /physicalPath:$physical `
        ("/bindings:http/*:{0}:localhost" -f $target.Port) | Out-Null
    & $appcmd set app "$siteName/" /applicationPool:$poolName | Out-Null

    # BlogEngine は App_Data に書きます(設定・キャッシュ)。読み取り専用だと
    # 初回要求で落ち、そのエラーページがゴールデンマスターとして記録されます。
    $identity = "IIS AppPool\$poolName"
    & icacls $physical /grant "${identity}:(OI)(CI)(M)" /T /Q 2>&1 | Out-Null

    try {
        $url = "http://localhost:$($target.Port)/"
        $ready = $false
        foreach ($attempt in 1..30) {
            try {
                $response = Invoke-WebRequest -Uri $url -UseBasicParsing -TimeoutSec 10
                if ($response.StatusCode -eq 200) { $ready = $true; break }
            } catch { Start-Sleep -Seconds 2 }
        }
        if (-not $ready) {
            Write-Warning "  変換前アプリが応答しません: $url"
            $failures += "${name}: 起動失敗"
            continue
        }

        Write-Host "  採取: $golden"
        & dotnet $verifier record --url $url --scenario $scenario --out $golden
        if ($LASTEXITCODE -ne 0) {
            $failures += "${name}: 採取に失敗"
        }
    }
    finally {
        if (-not $KeepSite) {
            & $appcmd delete site $siteName 2>&1 | Out-Null
            & $appcmd delete apppool $poolName 2>&1 | Out-Null
        }
    }
}

Write-Host ""
if ($failures.Count -gt 0) {
    Write-Host '要確認:' -ForegroundColor Red
    foreach ($failure in $failures) { Write-Host "  - $failure" -ForegroundColor Red }
    exit 1
}

Write-Host '採取しました。' -ForegroundColor Green
Write-Host '中身を必ず目で確認してください。エラーページを 200 で記録していても' -ForegroundColor Yellow
Write-Host 'ここは成功と言います。以後すべての照合がそれを正解として測ります。' -ForegroundColor Yellow
exit 0
