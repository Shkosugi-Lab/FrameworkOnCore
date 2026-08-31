# コーパス一括変換 + ベースライン比較。
#
# 変換器を触ったらこれを回して、残差が増えていないかを見ます。
#
# 重要: 残差の前後比較は変換オプション(--include / --web-config / --control-map)を
# 揃えて初めて成立します。オプションを 1 つ落とすと数字は桁で動きます。
# 例: yaf は --include 無しだと移植 .cs が 662 -> 17 になり、<YAF:LocalizedLabel> などが
# LegacyRenderHost に解決されず UnmappedControl が 2,000 件超に爆発します。
# そのためオプションはこのスクリプトに固定し、コマンドラインからは変えられません。
#
# 使い方:
#   .\corpora\convert-all.ps1                    # 全コーパスを変換してベースライン比較
#   .\corpora\convert-all.ps1 -Only be,yaf       # 指定したものだけ
#   .\corpora\convert-all.ps1 -UpdateBaseline    # 現在の実測値を expected.json に書き戻す
param(
    [string]$Root = (Join-Path $PSScriptRoot 'work'),
    [string]$Out = (Join-Path $PSScriptRoot 'out'),
    [string[]]$Only,
    [switch]$SkipBuild,
    [switch]$UpdateBaseline
)

$ErrorActionPreference = 'Continue'
$repo = Split-Path $PSScriptRoot -Parent
$baselinePath = Join-Path $PSScriptRoot 'expected.json'

# Library projects are no longer listed here: the converter derives them from the web
# project's ProjectReference graph. What remains is only what the graph cannot decide -
# which .csproj to start from when an app ships several, and which of a set of mutually
# exclusive data providers to take.
$corpora = @(
    @{ Name = 'be'
       Input = 'BlogEngine.NET-3.3.8.0\BlogEngine\BlogEngine.NET'
       Include = @() },

    @{ Name = 'mojo'
       Input = 'mojoportal-3.1.6\Web'
       # The converter detects that the four mojoPortal.Data.* projects declare the same
       # types and takes none of them; the deployed database is picked here.
       Include = @('mojoportal-3.1.6\mojoPortal.Data.MSSQL')
       # <mp:mojoGridView> は独自コントロール。マップが無いと未対応コントロール扱いになる。
       ControlMap = 'mojo-control-map.json' },

    @{ Name = 'yaf'
       Input = 'YAFNET-3.2.15\yafsrc\YetAnotherForum.NET'
       # YAF ships one .csproj per database, so the entry point has to be named; the graph
       # from it reaches the vendored ServiceStack.OrmLite and Lucene.Net sources too.
       Project = 'YAFNET-3.2.15\yafsrc\YetAnotherForum.NET\YAF-SqlServer.csproj'
       Include = @()
       # YAF はサイトルートに Web.config が無く、配布時にリネームする前提。
       # これを渡さないと tagPrefix が読めず YAF: が全部未対応コントロールになる。
       WebConfig = 'YAFNET-3.2.15\yafsrc\YetAnotherForum.NET\recommended.web.config' },

    @{ Name = 'dnn'
       Input = 'Dnn.Platform-9.13.10\DNN Platform\Website'
       Include = @() },

    # 入力ルートが 1 階層深い(リポジトリ名 / ソリューション名 / プロジェクト名)。
    @{ Name = 'wt'
       Input = 'wingtiptoys-master\WingtipToys\WingtipToys'
       Include = @() }
)

if ($Only) {
    $corpora = $corpora | Where-Object { $Only -contains $_.Name }
    if (-not $corpora) {
        Write-Error "-Only に一致するコーパスがありません。指定可能: be, mojo, yaf, dnn, wt"
    }
}

# 変換レポート冒頭のサマリー表から実測値を読む。
function Read-Summary {
    param([string]$ReportPath)

    $lines = Get-Content $ReportPath -Encoding UTF8 -TotalCount 20
    $result = @{ PortedCs = -1; Residuals = -1; Convertible = -1; Errors = -1 }

    foreach ($line in $lines) {
        if ($line -notmatch '^\|\s*(?<label>.+?)\s*\|\s*(?<n>\d+)\s*\|\s*$') { continue }
        $label = $Matches['label'] -replace '&nbsp;', ''
        $n = [int]$Matches['n']

        # 「変換可能」は「残差」の内訳行なので、先に判定する
        if ($label -match '^変換可能')       { $result.Convertible = $n; continue }
        if ($label -match '^残差')           { $result.Residuals = $n;   continue }
        if ($label -match '^そのまま移植した') { $result.PortedCs = $n;    continue }
        if ($label -match '^エラー')         { $result.Errors = $n;      continue }
    }
    return $result
}

if (-not (Test-Path $Root)) {
    Write-Error "コーパスが見つかりません: $Root`n先に .\corpora\fetch.ps1 を実行してください。"
}
$Root = (Resolve-Path $Root).Path

if (-not $SkipBuild) {
    Write-Host '=== 変換器のビルド ==='
    # bin\alt に出すのは verify-all.ps1 と同じ理由。実行中のプロセスが bin\Debug を
    # ロックしていてもビルドできるようにするため。
    dotnet build (Join-Path $repo 'src\WebForm2Blazor.Converter') `
        -o (Join-Path $repo 'src\WebForm2Blazor.Converter\bin\alt') --nologo -v q | Out-Null
    if ($LASTEXITCODE -ne 0) { Write-Error '変換器のビルドに失敗しました。' }
}

$converter = Join-Path $repo 'src\WebForm2Blazor.Converter\bin\alt\WebForm2Blazor.Converter.dll'
if (-not (Test-Path $converter)) {
    Write-Error "変換器が見つかりません: $converter`n-SkipBuild を外して実行してください。"
}
$componentsRef = Join-Path $repo 'src\WebForm2Blazor.Components\WebForm2Blazor.Components.csproj'

$baseline = @{}
if (Test-Path $baselinePath) {
    $json = Get-Content $baselinePath -Raw -Encoding UTF8 | ConvertFrom-Json
    foreach ($p in $json.PSObject.Properties) { $baseline[$p.Name] = $p.Value }
}

$measured = @{}
$rows = @()
$problems = @()

foreach ($c in $corpora) {
    Write-Host ""
    Write-Host ("=== {0} ===" -f $c.Name)

    $inputPath = Join-Path $Root $c.Input
    if (-not (Test-Path $inputPath)) {
        Write-Warning "  入力が見つかりません: $inputPath(fetch.ps1 未実行?)"
        $problems += "$($c.Name): 入力なし"
        continue
    }

    # 出力は毎回作り直す。前回の生成物が残っているとレポートが混ざり、
    # 「直したはずの残差がまだ出る」ように見える(実際に一度これで誤読した)。
    $outDir = Join-Path $Out $c.Name
    if (Test-Path $outDir) { Remove-Item $outDir -Recurse -Force -ErrorAction SilentlyContinue }

    $arguments = @('--input', $inputPath, '--output', $outDir, '--name', $c.Name,
                   '--components-ref', $componentsRef)
    foreach ($inc in $c.Include) { $arguments += @('--include', (Join-Path $Root $inc)) }
    if ($c.Project) { $arguments += @('--project', (Join-Path $Root $c.Project)) }
    if ($c.WebConfig)  { $arguments += @('--web-config',  (Join-Path $Root $c.WebConfig)) }
    if ($c.ControlMap) { $arguments += @('--control-map', (Join-Path $PSScriptRoot $c.ControlMap)) }

    & dotnet $converter @arguments | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Warning "  変換が終了コード $LASTEXITCODE を返しました"
    }

    $report = Join-Path $outDir 'CONVERSION-REPORT.md'
    if (-not (Test-Path $report)) {
        Write-Warning "  レポートが生成されませんでした"
        $problems += "$($c.Name): レポートなし"
        continue
    }

    $s = Read-Summary -ReportPath $report
    $measured[$c.Name] = @{
        portedCs    = $s.PortedCs
        residuals   = $s.Residuals
        convertible = $s.Convertible
    }

    $expected = $baseline[$c.Name]
    $verdict = 'ベースラインなし'
    if ($expected) {
        $diffs = @()
        # 移植 .cs 数が動いていたらオプションが違う。この時点で残差の比較は無意味。
        if ($s.PortedCs -ne $expected.portedCs) {
            $diffs += ("移植 .cs {0}->{1}" -f $expected.portedCs, $s.PortedCs)
        }
        if ($s.Convertible -ne $expected.convertible) {
            $diffs += ("変換可能 {0}->{1}" -f $expected.convertible, $s.Convertible)
        }
        if ($s.Residuals -ne $expected.residuals) {
            $diffs += ("総残差 {0}->{1}" -f $expected.residuals, $s.Residuals)
        }

        if ($diffs.Count -eq 0) {
            $verdict = '一致'
        }
        else {
            $verdict = ($diffs -join ' / ')
            # 変換可能残差が減るのは改善。それ以外の変化は必ず理由を確認すること。
            $improved = ($s.Convertible -lt $expected.convertible) -and
                        ($s.PortedCs -eq $expected.portedCs)
            if ($improved) { $verdict = "改善: $verdict" }
            else           { $problems += "$($c.Name): $verdict" }
        }
    }

    if ($s.Errors -gt 0) { $problems += "$($c.Name): 変換エラー $($s.Errors) 件" }

    $rows += [PSCustomObject]@{
        コーパス    = $c.Name
        '移植 .cs'  = $s.PortedCs
        総残差      = $s.Residuals
        変換可能    = $s.Convertible
        エラー      = $s.Errors
        判定        = $verdict
    }
    Write-Host ("  移植 .cs {0} / 総残差 {1} / 変換可能 {2} / エラー {3} — {4}" -f `
        $s.PortedCs, $s.Residuals, $s.Convertible, $s.Errors, $verdict)
}

Write-Host ""
Write-Host '=== 結果 ==='
$rows | Format-Table -AutoSize

if ($rows.Count -gt 0) {
    $totalResiduals = ($rows | Measure-Object -Property 総残差 -Sum).Sum
    $totalConvertible = ($rows | Measure-Object -Property 変換可能 -Sum).Sum
    Write-Host ("合計: 総残差 {0} / 変換可能 {1}" -f $totalResiduals, $totalConvertible)
}

if ($UpdateBaseline) {
    # 対象を絞って実行した場合、計測していないコーパスの値は保持する
    foreach ($k in $measured.Keys) { $baseline[$k] = $measured[$k] }
    $ordered = [ordered]@{}
    foreach ($k in @('be', 'mojo', 'yaf', 'dnn', 'wt')) {
        if ($baseline.ContainsKey($k)) { $ordered[$k] = $baseline[$k] }
    }
    $text = ($ordered | ConvertTo-Json -Depth 5)
    [IO.File]::WriteAllText($baselinePath, $text, (New-Object Text.UTF8Encoding $false))
    Write-Host ""
    Write-Host "ベースラインを更新しました: $baselinePath" -ForegroundColor Yellow
    Write-Host "変更を必ず差分で確認してから commit してください。"
    exit 0
}

Write-Host ""
if ($problems.Count -gt 0) {
    Write-Host '要確認:' -ForegroundColor Red
    foreach ($p in $problems) { Write-Host "  - $p" -ForegroundColor Red }
    Write-Host ""
    Write-Host '「移植 .cs」が動いている場合、変換器ではなくオプションかコーパスの取得内容が' -ForegroundColor Yellow
    Write-Host '違っています。残差の比較はその状態では成立しません。' -ForegroundColor Yellow
    exit 1
}

Write-Host 'ベースラインと一致しました。' -ForegroundColor Green
exit 0
