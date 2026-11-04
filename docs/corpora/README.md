# コーパスで変換を試す

この手引きは、FrameworkOnCore の変換を **自分の手で試したい人** のためのものです。
ASP.NET(Web Forms・MVC)は分かるけれど、このリポジトリの中身は知らない、という人を想定しています。

「コーパス」は、変換の確かめに使っている実在のオープンソースの .NET Framework の Web アプリです(11 本)。
コーパスを取ってきて、変換し、Windows と Linux で動かすまでを、コーパスごとに 1 ページで書きます。

- **実際にこの手引きのとおりに動かして確かめたもの**(2026-11-04): be(BlogEngine.NET)と wt(WingtipToys)の取得 → 変換 → Windows で起動・比較 → Linux(Docker)で起動・比較、Studio の画面。各ページでは「**確認**」と書きます。
- **それ以外のコーパス**: コマンドはスクリプトと照らし合わせて確かめましたが、この手引きのために動かしてはいません。結果は記録(`experiments/wf4c/README.md`、`tests/README.md`)の、日付つきの最新のものです。各ページでは「**記録**」と書きます。

## 進め方

1. [共通の準備](setup.md) をします(一度だけ)。入れるもの、リポジトリとコーパスの取得、変換のしかた、変換レポートの読み方、Windows・Linux での動かし方、後片付け、うまくいかないとき。
2. **[be](be.md) から始めます**。SQL Server も元のビルドも要らず、いちばん準備が少ないコーパスです。
3. 次に **[wt](wt.md)**。SQL Server Express を使うアプリの例です。
4. そのあとは、試したい種類に合わせて選びます。
   - MVC 5 を試す: [mvcmovie](mvcmovie.md) → [contoso](contoso.md) → [nop390](nop390.md)
   - 元のビルドが要る Web Forms を試す: [nop](nop.md) → [mojo](mojo.md) → [n2](n2.md) → [imis](imis.md)(VB) → [dnn](dnn.md)
   - [yaf](yaf.md) は、いまは変換とレポートまでです(`/` が 500)。

## コーパスの一覧

「難しさ」は、準備の多さの目安です(易: SQL Server までで動く。中: 元のビルドやインストーラーが要る。難: 元のビルドが長く、ディスクも多く要る)。
「最新の結果」の日付は、「確認」がこの手引きのために動かした日、「記録」が記録にある最新の日です。

| 名前 | 何か | 取得元(GitHub) | タグ / コミット | 種類 | 言語 | DB | 難しさ | 最新の結果 | ページ |
|---|---|---|---|---|---|---|---|---|---|
| be | BlogEngine.NET 3.3.8(ブログ) | [rxtur/BlogEngine.NET](https://github.com/rxtur/BlogEngine.NET) | タグ `v3.3.8.0`(`e81ba742`) | Web Forms | C# | 無し(App_Data の XML) | 易 | Windows・Linux とも画面 5/5、リンク 17/17(**確認** 2026-11-04)。すべての操作 201 件 `RESULT: OK`(記録 2026-11-02) | [be.md](be.md) |
| wt | WingtipToys(おもちゃ屋のチュートリアル) | [corn-mendoza/wingtiptoys](https://github.com/corn-mendoza/wingtiptoys) | コミット `4478e0e5`(master。タグ無し) | Web Forms | C# | SQL Server Express `WingtipToys` | 易 | Windows 画面 8/8、リンク 64/64。Linux 8/8(うち error-page は理由のある「許容」)、64/64(**確認** 2026-11-04) | [wt.md](wt.md) |
| mojo | mojoPortal 3.1.6(CMS) | [i7MEDIA/mojoportal](https://github.com/i7MEDIA/mojoportal) | タグ `v3.1.6`(`c0ada139`) | Web Forms | C# | SQL Server Express `mojo_w2l` | 中 | 空の DB からセットアップ → トップ・ログイン・サイトマップ 200。Windows・Linux とも(記録 2026-10-11) | [mojo.md](mojo.md) |
| yaf | YAF.NET 3.2.15(掲示板) | [YAFNET/YAFNET](https://github.com/YAFNET/YAFNET) | タグ `v3.2.15`(`4b785802`) | Web Forms | C# | SQL Server | 易(ただし動かない) | 変換とビルドは通るが **`/` が 500**(記録 2026-10-27・10-29) | [yaf.md](yaf.md) |
| dnn | DNN Platform 9.13.10(大きな CMS) | [dnnsoftware/Dnn.Platform](https://github.com/dnnsoftware/Dnn.Platform) | タグ `v9.13.10`(`53cdf475`) | Web Forms | C#、VB | SQL Server Express `dnn_w2l` | 難 | 空の DB からインストール完了、トップ・`/Login`・`/Terms`・`/Privacy` 200。Windows・Linux とも(記録 2026-10-11) | [dnn.md](dnn.md) |
| n2 | N2 CMS | [n2cms/n2cms](https://github.com/n2cms/n2cms) | コミット `db0ec9d1`(master。タグ無し) | Web Forms | C# | SQLite(App_Data) | 中 | インストーラー完了、トップにコンテンツ、`/N2/` はログインへ。Windows・Linux とも(記録 2026-10-11) | [n2.md](n2.md) |
| imis | openIMIS 24.10(健康保険の管理) | [openimis/web_app_vb](https://github.com/openimis/web_app_vb) | コミット `f21970b3`(main。タグ無し)。DB: [openimis/database_ms_sqlserver](https://github.com/openimis/database_ms_sqlserver) タグ `24.10`(`166bf13f`) | Web Forms | VB | SQL Server Express `imis_w2l` | 中 | ログインし、主な画面 7 つが 200。Windows・Linux とも(記録 2026-10-11) | [imis.md](imis.md) |
| nop | nopCommerce 1.90(EC サイト) | [nopSolutions/nopCommerce](https://github.com/nopSolutions/nopCommerce) | タグ `release-1.90`(`88f2c98d`) | Web Forms | C# | SQL Server Express | 中 | すべての操作 727 件(1,000 スナップショット)Windows・Linux とも `RESULT: OK`(記録 2026-11-03) | [nop.md](nop.md) |
| mvcmovie | MvcMovie(映画の一覧のチュートリアル) | [dotnet/AspNetDocs](https://github.com/dotnet/AspNetDocs) | コミット `552c58dd` の `aspnet/mvc/overview/getting-started/introduction/sample/MvcMovie` | MVC 5 | C# | SQL Server Express `MvcMovie`・`MvcMovieIdentity` | 易 | 12 画面中 11(記録 Windows 2026-10-27、Linux 2026-10-22) | [mvcmovie.md](mvcmovie.md) |
| nop390 | nopCommerce 3.90(EC サイト) | [nopSolutions/nopCommerce](https://github.com/nopSolutions/nopCommerce) | タグ `release-3.90`(`12d1f013`) | MVC 5 | C# | SQL Server Express | 中 | インストール(サンプルデータ)、店頭 19 画面、カートへの追加、管理画面 7 画面 200。Windows・Linux とも(記録 2026-10-11) | [nop390.md](nop390.md) |
| contoso | Contoso University(EF6 のチュートリアル) | [tigerbluejay/ContosoUniversity](https://github.com/tigerbluejay/ContosoUniversity) | コミット `2ee1ad2a` の `ContosoUniversitySolution` | MVC 5 | C# | SQL Server Express `ContosoUniversity1` | 易 | 12 画面中 9(記録 Windows 2026-10-27、Linux 2026-10-22) | [contoso.md](contoso.md) |

コーパスはこのリポジトリには入っていません。どれも元の GitHub のリポジトリから、表のタグ・コミット(結果はこの版で出したもの)を取ります。完全なコミット、ZIP の URL、git のコマンド、ライセンスは各ページの「3.1 取得する」にあります([共通の準備の手順 12](setup.md#手順-12-コーパスを取る))。

be・wt・nop 以外は、記録のあとにパッケージの版が上がっています(今は `1.6.5-w2l.27`)。同じ結果になるはずですが、確かめてはいません。

## 変換の値の一覧

Studio の「リポジトリのフォルダー」と「プロジェクト」に入れる値です(`corpora\work\` から)。値は `experiments\wf4c\convert-corpora.ps1` の一覧と同じです。

| 名前 | 元のビルド | リポジトリのフォルダー | Web プロジェクト |
|---|---|---|---|
| be | 不要 | [rxtur/BlogEngine.NET](https://github.com/rxtur/BlogEngine.NET) | タグ `v3.3.8.0`(`e81ba742`) | `BlogEngine.NET-3.3.8.0` | `BlogEngine.NET-3.3.8.0\BlogEngine\BlogEngine.NET\BlogEngine.NET.csproj` |
| wt | 不要 | [corn-mendoza/wingtiptoys](https://github.com/corn-mendoza/wingtiptoys) | コミット `4478e0e5`(master。タグ無し) | `wingtiptoys-master` | `wingtiptoys-master\WingtipToys\WingtipToys\WingtipToys.csproj` |
| mojo | 要(ソリューション) | [i7MEDIA/mojoportal](https://github.com/i7MEDIA/mojoportal) | タグ `v3.1.6`(`c0ada139`) | `mojoportal-3.1.6` | `mojoportal-3.1.6\Web\mojoPortal.Web.csproj` |
| yaf | 不要 | [YAFNET/YAFNET](https://github.com/YAFNET/YAFNET) | タグ `v3.2.15`(`4b785802`) | `YAFNET-3.2.15` | `YAFNET-3.2.15\yafsrc\YetAnotherForum.NET\YAF-SqlServer.csproj` |
| dnn | 要(DNN の Cake のビルド) | [dnnsoftware/Dnn.Platform](https://github.com/dnnsoftware/Dnn.Platform) | タグ `v9.13.10`(`53cdf475`) | `Dnn.Platform-9.13.10` | `Dnn.Platform-9.13.10\DNN Platform\Website\DotNetNuke.Website.csproj` |
| n2 | 要(ソリューション+手順 1 つ) | [n2cms/n2cms](https://github.com/n2cms/n2cms) | コミット `db0ec9d1`(master。タグ無し) | `n2cms-master` | `n2cms-master\src\WebForms\WebFormsTemplates\N2.Templates.csproj` |
| imis | 要(構成 `DemoRelease`) | [openimis/web_app_vb](https://github.com/openimis/web_app_vb) | コミット `f21970b3`(main。タグ無し)。DB: [openimis/database_ms_sqlserver](https://github.com/openimis/database_ms_sqlserver) タグ `24.10`(`166bf13f`) | `web_app_vb-main` | `web_app_vb-main\IMIS\IMIS.vbproj` |
| nop | 要(ソリューション) | [nopSolutions/nopCommerce](https://github.com/nopSolutions/nopCommerce) | タグ `release-1.90`(`88f2c98d`) | `nopCommerce-release-1.90` | `nopCommerce-release-1.90\NopCommerceStore\NopCommerceStore.csproj` |
| mvcmovie | 不要 | [dotnet/AspNetDocs](https://github.com/dotnet/AspNetDocs) | コミット `552c58dd` の `aspnet/mvc/overview/getting-started/introduction/sample/MvcMovie` | `MvcMovie` | `MvcMovie\MvcMovie\MvcMovie.csproj` |
| nop390 | 要(ソリューション) | [nopSolutions/nopCommerce](https://github.com/nopSolutions/nopCommerce) | タグ `release-3.90`(`12d1f013`) | `nopCommerce-release-3.90` | `nopCommerce-release-3.90\src\Presentation\Nop.Web\Nop.Web.csproj` |
| contoso | 不要 | [tigerbluejay/ContosoUniversity](https://github.com/tigerbluejay/ContosoUniversity) | コミット `2ee1ad2a` の `ContosoUniversitySolution` | `ContosoUniversity` | `ContosoUniversity\ContosoUniversity\ContosoUniversity.csproj` |

## IIS の結果と比べられるもの

元のアプリを IIS で動かして記録した正解があり、変換したアプリと自動で比べられるのは次のコーパスです(各ページの「IIS の結果と比べる」)。

| 名前 | 画面の比較(`verify-windows.ps1`、`run-linux.ps1`) | すべての操作の比較(`-Suite operations`) |
|---|---|---|
| be | 5 画面、リンク 17 | [rxtur/BlogEngine.NET](https://github.com/rxtur/BlogEngine.NET) | タグ `v3.3.8.0`(`e81ba742`) | 201 件 |
| wt | 8 画面、リンク 64 | [corn-mendoza/wingtiptoys](https://github.com/corn-mendoza/wingtiptoys) | コミット `4478e0e5`(master。タグ無し) | 152 件(223 スナップショット) |
| nop | ― | [nopSolutions/nopCommerce](https://github.com/nopSolutions/nopCommerce) | タグ `release-1.90`(`88f2c98d`) | 727 件(1,000 スナップショット) |
| mvcmovie | 12 画面 | [dotnet/AspNetDocs](https://github.com/dotnet/AspNetDocs) | コミット `552c58dd` の `aspnet/mvc/overview/getting-started/introduction/sample/MvcMovie` | ― |
| contoso | 12 画面 | [tigerbluejay/ContosoUniversity](https://github.com/tigerbluejay/ContosoUniversity) | コミット `2ee1ad2a` の `ContosoUniversitySolution` | ― |

ほかのコーパスは、ページを開いて確かめます。

## 関連する資料

- [共通の準備と使い方](setup.md)
- [README.md](../../README.md): 全体の流れ、Studio の手順、コマンドライン
- [tests/README.md](../../tests/README.md): テストと比較の一覧
- [corpora/README.md](../../corpora/README.md): コーパスの取得元と正解の記録
- [experiments/wf4c/README.md](../../experiments/wf4c/README.md): 検証の記録(コーパスごとの経緯)
