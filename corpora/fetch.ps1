# コーパス取得スクリプト。
#
# 変換の適用範囲は「実在する WebForms アプリを変換したときの残差」で測ります。
# その測定対象(コーパス)をここで取得します。
#
# 各コーパスはサードパーティのソースであり、それぞれ独自のライセンスを持ちます。
# リポジトリには含めず、ここで既知のバージョンを取得して -Root 配下に展開します。
# 既定の取得先 corpora/work/ は .gitignore 済みです。
#
# 使い方:
#   .\corpora\fetch.ps1                 # 未取得のものだけ取得
#   .\corpora\fetch.ps1 -Only be,yaf    # 指定したものだけ
#   .\corpora\fetch.ps1 -Force          # 取得済みでも再取得
param(
    [string]$Root = (Join-Path $PSScriptRoot 'work'),
    [string[]]$Only,
    [switch]$Force
)

$ErrorActionPreference = 'Stop'

# Url         : GitHub のアーカイブ URL(タグ / ブランチ)
# ExtractedAs : 展開後にできるディレクトリ名。GitHub は先頭の "v" を落とすので
#               タグ v3.3.8.0 は BlogEngine.NET-3.3.8.0 になる。
$corpora = @(
    @{ Name = 'be';   Title = 'BlogEngine.NET 3.3.8.0'
       Url = 'https://github.com/rxtur/BlogEngine.NET/archive/refs/tags/v3.3.8.0.zip'
       ExtractedAs = 'BlogEngine.NET-3.3.8.0' },

    @{ Name = 'mojo'; Title = 'mojoPortal 3.1.6'
       Url = 'https://github.com/i7MEDIA/mojoportal/archive/refs/tags/v3.1.6.zip'
       ExtractedAs = 'mojoportal-3.1.6' },

    @{ Name = 'yaf';  Title = 'YAF.NET 3.2.15'
       Url = 'https://github.com/YAFNET/YAFNET/archive/refs/tags/v3.2.15.zip'
       ExtractedAs = 'YAFNET-3.2.15' },

    @{ Name = 'dnn';  Title = 'DNN Platform 9.13.10'
       Url = 'https://github.com/dnnsoftware/Dnn.Platform/archive/refs/tags/v9.13.10.zip'
       ExtractedAs = 'Dnn.Platform-9.13.10' },

    # Added as a held-out test set: the first five corpora had shaped the converter, and a
    # sixth it had never seen immediately exposed a gap they all missed (generic page bases).
    @{ Name = 'n2';   Title = 'n2cms (master)'
       Url = 'https://github.com/n2cms/n2cms/archive/refs/heads/master.zip'
       ExtractedAs = 'n2cms-master' },

    @{ Name = 'wt';   Title = 'WingtipToys (master)'
       Url = 'https://github.com/corn-mendoza/wingtiptoys/archive/refs/heads/master.zip'
       ExtractedAs = 'wingtiptoys-master' }
)

# nopCommerce 1.90 について:
#   6 つめのコーパスとして以前計測していましたが(残差 264 / 変換可能 32)、
#   nopCommerce 1.x は CodePlex 時代のリリースで GitHub の nopSolutions/nopCommerce に
#   1.x のタグが存在せず、ミラーも見つかりませんでした。よって自動取得できません。
#   手元にアーカイブがある場合は $Root\nopcommerce-1.90 に展開すれば
#   convert-all.ps1 の -Only nop で計測できます(既定では対象外)。
#
#   注意: リポジトリにあった samples/nopCommerce3.8 は 3.8 = ASP.NET MVC で、
#   このコーパス(1.90 = WebForms)とは別物です。変換対象にはなりません。

if ($Only) {
    $corpora = $corpora | Where-Object { $Only -contains $_.Name }
    if (-not $corpora) {
        Write-Error "-Only に一致するコーパスがありません。指定可能: be, mojo, yaf, dnn, n2, wt"
    }
}

if (-not (Test-Path $Root)) {
    New-Item -ItemType Directory -Path $Root -Force | Out-Null
}
$Root = (Resolve-Path $Root).Path
Write-Host "取得先: $Root"
Write-Host ""

$failed = @()

foreach ($c in $corpora) {
    $target = Join-Path $Root $c.ExtractedAs
    Write-Host ("=== {0} ({1}) ===" -f $c.Name, $c.Title)

    if ((Test-Path $target) -and (-not $Force)) {
        Write-Host "  取得済み: $($c.ExtractedAs)(再取得は -Force)"
        continue
    }

    if ((Test-Path $target) -and $Force) {
        Write-Host "  既存を削除中..."
        Remove-Item $target -Recurse -Force
    }

    $zip = Join-Path $Root ("{0}.zip" -f $c.Name)
    try {
        Write-Host "  取得中: $($c.Url)"
        # curl.exe は Windows 10 以降に同梱。Invoke-WebRequest より大幅に速い
        # (DNN と mojoPortal は 40MB あるため体感差が大きい)。
        $curl = Get-Command curl.exe -ErrorAction SilentlyContinue
        if ($curl) {
            # PowerShell 5.1 は native コマンドが stderr に書いた行を ErrorRecord に
            # 変換する。呼び出し側が 2>&1 で出力を捕捉していると、$ErrorActionPreference
            # が Stop のせいでそれが終了エラーになり、ダウンロード成功時でも失敗扱いに
            # なる(CI やログ取得で実際に踏む)。この区間だけ Continue にして、成否は
            # $LASTEXITCODE だけで判定する。進捗バーも stderr に書くので使わない。
            $previousEap = $ErrorActionPreference
            $ErrorActionPreference = 'Continue'
            try { & curl.exe -L --fail --silent --show-error -o $zip $c.Url }
            finally { $ErrorActionPreference = $previousEap }
            if ($LASTEXITCODE -ne 0) { throw "curl.exe が終了コード $LASTEXITCODE を返しました" }
        }
        else {
            $previous = $ProgressPreference
            $ProgressPreference = 'SilentlyContinue'   # 進捗表示があると極端に遅くなる
            try { Invoke-WebRequest -Uri $c.Url -OutFile $zip -UseBasicParsing }
            finally { $ProgressPreference = $previous }
        }

        Write-Host "  展開中..."
        Expand-Archive -Path $zip -DestinationPath $Root -Force

        if (-not (Test-Path $target)) {
            throw "展開後に $($c.ExtractedAs) が見つかりません(アーカイブの構造が変わった可能性があります)"
        }
        Write-Host "  OK: $target"
    }
    catch {
        Write-Warning "  失敗: $($c.Name) - $($_.Exception.Message)"
        $failed += $c.Name
    }
    finally {
        if (Test-Path $zip) { Remove-Item $zip -Force }
    }
}

Write-Host ""
if ($failed.Count -gt 0) {
    Write-Host "失敗したコーパス: $($failed -join ', ')" -ForegroundColor Red
    exit 1
}

Write-Host "すべて取得できました。次: .\corpora\convert-all.ps1" -ForegroundColor Green
exit 0
