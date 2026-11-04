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

# Each corpus comes from its original GitHub repository, at an exact commit:
# Repo        : the repository (its page)
# Tag         : the release tag, when it has one (its archive URL); Commit is the commit the tag points to
# Branch      : no tag: the branch it was taken from; Commit pins it (the archive of that commit, not of the branch,
#               so a later push cannot change the corpus the baselines and goldens were made from)
# Url         : the archive (ZIP) downloaded: by tag, or by commit
# ExtractedAs : 展開後のディレクトリ名。GitHub は先頭の "v" を落とすので
#               タグ v3.3.8.0 は BlogEngine.NET-3.3.8.0 になる。コミットのアーカイブは <repo>-<commit> に展開されるので、
#               ブランチ名の時代の名前(wingtiptoys-master など)に改名する(既存のパスとベースラインのため)。
# Git / Path  : a folder of a repository, by a sparse git clone at Commit (mvcmovie, contoso)
$corpora = @(
    @{ Name = 'be';   Title = 'BlogEngine.NET 3.3.8.0'
       Repo = 'https://github.com/rxtur/BlogEngine.NET'; Tag = 'v3.3.8.0'; Commit = 'e81ba7425e860c2265f66911436bd241061274f9'
       Url = 'https://github.com/rxtur/BlogEngine.NET/archive/refs/tags/v3.3.8.0.zip'
       ExtractedAs = 'BlogEngine.NET-3.3.8.0' },

    @{ Name = 'mojo'; Title = 'mojoPortal 3.1.6'
       Repo = 'https://github.com/i7MEDIA/mojoportal'; Tag = 'v3.1.6'; Commit = 'c0ada13952e4078ff2e89fe954511dfe29c1677c'
       Url = 'https://github.com/i7MEDIA/mojoportal/archive/refs/tags/v3.1.6.zip'
       ExtractedAs = 'mojoportal-3.1.6' },

    @{ Name = 'yaf';  Title = 'YAF.NET 3.2.15'
       Repo = 'https://github.com/YAFNET/YAFNET'; Tag = 'v3.2.15'; Commit = '4b785802ce3bc688e2685fe624e554cf90824f89'
       Url = 'https://github.com/YAFNET/YAFNET/archive/refs/tags/v3.2.15.zip'
       ExtractedAs = 'YAFNET-3.2.15' },

    @{ Name = 'dnn';  Title = 'DNN Platform 9.13.10'
       Repo = 'https://github.com/dnnsoftware/Dnn.Platform'; Tag = 'v9.13.10'; Commit = '53cdf4750477293b4e6316b4d91e1d5e43610127'
       Url = 'https://github.com/dnnsoftware/Dnn.Platform/archive/refs/tags/v9.13.10.zip'
       ExtractedAs = 'Dnn.Platform-9.13.10' },

    # Added as a held-out test set: the first five corpora had shaped the converter, and a
    # sixth it had never seen immediately exposed a gap they all missed (generic page bases).
    # No release tag for this source: master's last commit (2018-11-20).
    @{ Name = 'n2';   Title = 'n2cms (master, db0ec9d1)'
       Repo = 'https://github.com/n2cms/n2cms'; Branch = 'master'; Commit = 'db0ec9d193040e64ccbec65604c4599da67a4271'
       Url = 'https://github.com/n2cms/n2cms/archive/db0ec9d193040e64ccbec65604c4599da67a4271.zip'
       ExtractedAs = 'n2cms-master' },

    # No tags: master's last commit (2021-02-12).
    @{ Name = 'wt';   Title = 'WingtipToys (master, 4478e0e5)'
       Repo = 'https://github.com/corn-mendoza/wingtiptoys'; Branch = 'master'; Commit = '4478e0e5794ed709da924569c54bb5f4c714fb85'
       Url = 'https://github.com/corn-mendoza/wingtiptoys/archive/4478e0e5794ed709da924569c54bb5f4c714fb85.zip'
       ExtractedAs = 'wingtiptoys-master'
       # Its Web.config names an Azure SQL database that does not exist (ms.database.windows.net, uid1 / P@123): the
       # database it would have used, on SQL Server Express (Set-ConnectionStrings below).
       WebConfig = 'WingtipToys\WingtipToys\Web.config'
       ConnectionStrings = @{
           DefaultConnection = 'Data Source=.\SQLEXPRESS;Initial Catalog=WingtipToys;Integrated Security=True;MultipleActiveResultSets=True;Connect Timeout=30'
           WingtipToys = 'Data Source=.\SQLEXPRESS;Initial Catalog=WingtipToys;Integrated Security=True;MultipleActiveResultSets=True;Connect Timeout=30' } },

    # Visual Basic: openIMIS, a health insurance management system in use by governments (AGPL-3.0). The web application
    # (archived: its main is the last, 24.10) and the database scripts of the same release (fullDemoDatabase.sql).
    # main's last commit (2025-01-08; the repository is archived, read-only).
    @{ Name = 'imis'; Title = 'openIMIS web application (main, 24.10, f21970b3)'
       Repo = 'https://github.com/openimis/web_app_vb'; Branch = 'main'; Commit = 'f21970b3136d8083f2da6bb774652614c1ffd247'
       Url = 'https://github.com/openimis/web_app_vb/archive/f21970b3136d8083f2da6bb774652614c1ffd247.zip'
       ExtractedAs = 'web_app_vb-main' },

    @{ Name = 'imisdb'; Title = 'openIMIS SQL Server database 24.10'
       Repo = 'https://github.com/openimis/database_ms_sqlserver'; Tag = '24.10'; Commit = '166bf13f70d55c6fc2a9c1d0fbf8d661671c6cad'
       Url = 'https://github.com/openimis/database_ms_sqlserver/archive/refs/tags/24.10.zip'
       ExtractedAs = 'database_ms_sqlserver-24.10' },

    # nopCommerce 1.90 (2011), the last Web Forms version of the e-commerce platform (C#, .NET Framework 4.0, Entity Framework 4
    # with an EDMX model: System.Data.Objects). Later versions are MVC (2.x-3.x) and ASP.NET Core (4.x).
    @{ Name = 'nop'; Title = 'nopCommerce 1.90 (Web Forms)'
       Repo = 'https://github.com/nopSolutions/nopCommerce'; Tag = 'release-1.90'; Commit = '88f2c98d158a85b281b6a757e578e2f1cf00a2de'
       Url = 'https://github.com/nopSolutions/nopCommerce/archive/refs/tags/release-1.90.zip'
       ExtractedAs = 'nopCommerce-release-1.90' },

    # ASP.NET MVC 5. MvcMovie: the official MVC 5 tutorial's application (EF6 Code First with migrations, ASP.NET
    # Identity over OWIN, LocalDB), a folder of dotnet/AspNetDocs (a large repository: only that folder, at a commit).
    @{ Name = 'mvcmovie'; Title = 'MvcMovie (ASP.NET MVC 5 tutorial, AspNetDocs)'
       Repo = 'https://github.com/dotnet/AspNetDocs'; Git = 'https://github.com/dotnet/AspNetDocs.git'; Commit = '552c58dd8a809ef8cea02e66c9463ce9c7dcdfe5'
       Path = 'aspnet/mvc/overview/getting-started/introduction/sample/MvcMovie'
       ExtractedAs = 'MvcMovie'
       WebConfig = 'MvcMovie\Web.config'
       ConnectionStrings = @{
           MovieDBContext = 'Data Source=.\SQLEXPRESS;Initial Catalog=MvcMovie;Integrated Security=True'
           DefaultConnection = 'Data Source=.\SQLEXPRESS;Initial Catalog=MvcMovieIdentity;Integrated Security=True' } },

    # nopCommerce 3.90 (2017), the last MVC 5 version (.NET Framework 4.5.1, Entity Framework 6, Autofac); 4.x is ASP.NET Core.
    @{ Name = 'nop390'; Title = 'nopCommerce 3.90 (ASP.NET MVC 5)'
       Repo = 'https://github.com/nopSolutions/nopCommerce'; Tag = 'release-3.90'; Commit = '12d1f0139149a9d6e84964d325d62bae6ac748f6'
       Url = 'https://github.com/nopSolutions/nopCommerce/archive/refs/tags/release-3.90.zip'
       ExtractedAs = 'nopCommerce-release-3.90' },

    # ASP.NET MVC 5. Contoso University: the EF6 tutorial's application (students' CRUD with sorting, search and paging,
    # Code First with an initializer's seed, LocalDB; .NET Framework 4.7.2), as one learner wrote it. Its packages
    # and a bin of Microsoft's DLLs are checked in. No license given (the corpora are fetched, not redistributed).
    # At a commit (main has no tags); its solution folder only, as ContosoUniversity: the archive's folder name (with the
    # commit) made its packages' paths longer than .NET Framework's MSBuild takes (260 characters: "missing" packages).
    @{ Name = 'contoso'; Title = 'Contoso University (ASP.NET MVC 5, EF6 tutorial)'
       Repo = 'https://github.com/tigerbluejay/ContosoUniversity'; Git = 'https://github.com/tigerbluejay/ContosoUniversity.git'; Commit = '2ee1ad2aa66fbef012fe280e4552176489dcf0f3'
       Path = 'ContosoUniversitySolution'
       ExtractedAs = 'ContosoUniversity'
       WebConfig = 'ContosoUniversity\Web.config'
       ConnectionStrings = @{ SchoolContext = 'Data Source=.\SQLEXPRESS;Initial Catalog=ContosoUniversity1;Integrated Security=True' } }
)

# The databases of the corpora with SQL Server (wt, mvcmovie, contoso) on SQL Server Express (.\SQLEXPRESS): their
# Web.config's connection strings, by name, as corpora\record-webforms-golden.ps1 points them when it records the golden
# data (the same values). wt's names an Azure SQL database that does not exist (an anonymized sample: its start waits for
# it); mvcmovie's and contoso's LocalDB, whose instances are each user's (IIS's application pool and the converted
# application would see different databases). Without it a corpus fetched on a new machine did not start after
# convert-corpora.ps1 (verify-windows.ps1, run-linux.ps1 -SqlServer, Studio) until record-webforms-golden.ps1 had run.
function Set-ConnectionStrings($corpus, [string]$target) {
    if (-not $corpus.ConnectionStrings) { return }
    $webConfig = Join-Path $target $corpus.WebConfig
    $text = Get-Content $webConfig -Raw
    foreach ($entry in $corpus.ConnectionStrings.GetEnumerator()) {
        $text = [regex]::Replace($text,
            '(<add\s+name="' + [regex]::Escape($entry.Key) + '"\s+connectionString=")[^"]*(")',
            { param($m) $m.Groups[1].Value + $entry.Value + $m.Groups[2].Value })
    }
    Set-Content $webConfig -Value $text -Encoding utf8
    Write-Host ("  接続文字列を SQL Server Express(.\SQLEXPRESS)に向けました: " + (($corpus.ConnectionStrings.Keys | Sort-Object) -join ', '))
}

# nopCommerce 1.90 について:
#   最後の Web Forms 版。GitHub のタグ release-1.90 のアーカイブから取得する(nop)。

if ($Only) {
    $corpora = $corpora | Where-Object { $Only -contains $_.Name }
    if (-not $corpora) {
        Write-Error "-Only に一致するコーパスがありません。指定可能: be, mojo, yaf, dnn, n2, wt, imis, imisdb, nop, mvcmovie, nop390, contoso"
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
    if ($c.Tag) { $at = "タグ $($c.Tag) (コミット $($c.Commit))" }
    elseif ($c.Branch) { $at = "コミット $($c.Commit) ($($c.Branch) ブランチ)" }
    else { $at = "コミット $($c.Commit)" }
    Write-Host "  取得元: $($c.Repo) $at$(if ($c.Path) { " のフォルダ $($c.Path)" })"

    if ((Test-Path $target) -and (-not $Force)) {
        Write-Host "  取得済み: $($c.ExtractedAs)(再取得は -Force)"
        continue
    }

    if ((Test-Path $target) -and $Force) {
        Write-Host "  既存を削除中..."
        Remove-Item $target -Recurse -Force
    }

    # A folder of a large repository: a partial clone (no file contents until checked out) with only that folder,
    # at the commit given, moved to ExtractedAs.
    if ($c.Git) {
        $clone = Join-Path $Root ("{0}.git-tmp" -f $c.Name)
        try {
            if (Test-Path $clone) { Remove-Item $clone -Recurse -Force }
            Write-Host "  取得中: $($c.Git) @ $($c.Commit.Substring(0, 8)) : $($c.Path)"
            $previousEap = $ErrorActionPreference
            $ErrorActionPreference = 'Continue'
            try {
                # core.autocrlf=true: line ends as the baselines' trees have them (CRLF for text files: Git for Windows'
                # default), whatever this machine's Git is configured with (git clone -c sets it for this clone).
                git clone --quiet -c core.autocrlf=true --filter=blob:none --no-checkout --sparse $c.Git $clone 2>&1 | Out-Null
                if ($LASTEXITCODE -ne 0) { throw "git clone が終了コード $LASTEXITCODE を返しました" }
                git -C $clone sparse-checkout set $c.Path 2>&1 | Out-Null
                git -C $clone checkout --quiet $c.Commit 2>&1 | Out-Null
                if ($LASTEXITCODE -ne 0) { throw "git checkout が終了コード $LASTEXITCODE を返しました" }
            }
            finally { $ErrorActionPreference = $previousEap }
            Move-Item (Join-Path $clone ($c.Path -replace '/', '\')) $target
            Set-ConnectionStrings $c $target
            Write-Host "  OK: $target"
        }
        catch {
            Write-Warning "  失敗: $($c.Name) - $($_.Exception.Message)"
            $failed += $c.Name
        }
        finally {
            if (Test-Path $clone) { Remove-Item $clone -Recurse -Force }
        }
        continue
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

        # A commit's archive extracts to <repo>-<commit>: renamed to ExtractedAs.
        $extracted = if ($c.Tag) { $target } else { Join-Path $Root ("{0}-{1}" -f ($c.Repo -split '/')[-1], $c.Commit) }
        if (($extracted -ne $target) -and (Test-Path $extracted)) { Remove-Item $extracted -Recurse -Force }

        Write-Host "  展開中..."
        Expand-Archive -Path $zip -DestinationPath $Root -Force
        if (($extracted -ne $target) -and (Test-Path $extracted)) { Rename-Item $extracted $c.ExtractedAs }

        if (-not (Test-Path $target)) {
            throw "展開後に $($c.ExtractedAs) が見つかりません(アーカイブの構造が変わった可能性があります)"
        }
        Set-ConnectionStrings $c $target
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

Write-Host "すべて取得できました。次: .\experiments\wf4c\convert-corpora.ps1 -Only be,wt" -ForegroundColor Green
exit 0
