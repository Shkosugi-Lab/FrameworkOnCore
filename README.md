# FrameworkOnCore

.NET Framework の Web アプリ(ASP.NET Web Forms・MVC・Web API・WCF、C# と VB)を、**ほぼ書き換えずに .NET 10 に移し、Linux で動かす**ための変換ツールと実行基盤です。

- 変換器が書き換えるのは、プロジェクトファイルと、.NET にない API の呼び出しだけです。画面と業務ロジックは元のまま使います。
- アプリが頼っている土台(System.Web など)は、FrameworkOnCore が .NET 10 の上に用意します。Windows と Linux の違い(ファイル名の大文字・小文字、パス、カルチャ、IIS の設定など)も土台の側で合わせます。
- 解析・変換・起動までを画面で進める Studio と、コマンドラインの変換器があります。

使い方と開発者向けの情報は [docs/development.md](docs/development.md) にあります。

## 検証シナリオ

実在のオープンソースの .NET Framework のアプリ(コーパス)を変換し、Windows と Linux のコンテナで動かす手順です。準備は [共通の準備](docs/corpora/setup.md) にあります。やさしい順に並べています。最初は BlogEngine.NET をお勧めします。

| アプリ | 言語 | 種類 | 画面数 | 難易度 | 準備と手間 |
|---|---|---|---|---|---|
| [BlogEngine.NET 3.3.8](docs/corpora/blogengine-net.md)([Studio 版](docs/corpora/studio/blogengine-net.md)) | C# | Web Forms | 53 | ★ | なし(データは App_Data の XML) |
| [Bootstrap2-VB-MVC](docs/corpora/bootstrap2-vb-mvc.md) | VB | MVC 4 | 23 | ★ | Visual Studio か Build Tools |
| [eShopLegacyMVC](docs/corpora/eshop-legacy-mvc.md) | C# | MVC 5 + Web API | 7(Web API 4) | ★ | Visual Studio か Build Tools(DB なし。モックのデータ) |
| [MultipleEndpoints(C# と VB)](docs/corpora/wcf-multiple-endpoints.md) | C#・VB | WCF | —(サービスの操作 4) | ★★ | Visual Studio か Build Tools。確かめるのは SOAP の呼び出し(PowerShell) |
| [WingtipToys](docs/corpora/wingtiptoys.md) | C# | Web Forms | 28 | ★★ | SQL Server のコンテナ(DB はアプリが作る) |
| [MvcMovie](docs/corpora/mvcmovie.md) | C# | MVC 5 | 29 | ★★ | SQL Server のコンテナ(DB はアプリが作る) |
| [Contoso University](docs/corpora/contoso-university.md) | C# | MVC 5 | 9 | ★★ | SQL Server のコンテナ(DB はアプリが作る) |
| [nopCommerce 1.90](docs/corpora/nopcommerce-1.90.md) | C# | Web Forms | 293 | ★★★ | Visual Studio か Build Tools、SQL Server、インストール画面での設定、ディスク 2 GB |
| [nopCommerce 3.90](docs/corpora/nopcommerce-3.90.md) | C# | MVC 5 | 388 | ★★★ | Visual Studio か Build Tools、SQL Server、インストール画面での設定、ディスク 3 GB |
| [mojoPortal 3.1.6](docs/corpora/mojoportal.md) | C# | Web Forms | 145 | ★★★ | Visual Studio か Build Tools、SQL Server(空の DB を作る)、セットアップ画面、ディスク 5 GB |
| [N2 CMS](docs/corpora/n2cms.md) | C# | Web Forms | 80 | ★★★ | Visual Studio か Build Tools、追加の手順、インストール画面での設定、ディスク 6 GB |
| [openIMIS 24.10](docs/corpora/openimis.md) | VB | Web Forms | 62 | ★★★ | Visual Studio か Build Tools、SQL Server(DB を別のリポジトリのスクリプトで作る。数分)、ディスク 2 GB |
| [DNN Platform 9.13.10](docs/corpora/dnn-platform.md) | C# | Web Forms | 9(モジュール 105) | ★★★ | Visual Studio か Build Tools、SQL Server、インストール画面(数分)、長いビルド(Cake)、ディスク 20 GB。いちばん重い |
| [YAF.NET 3.2.15](docs/corpora/yafnet.md) | C# | Web Forms | 3(コントロール 170) | — | 変換までで、動きません |

- **画面数**: Web プロジェクトのページ(`.aspx`)と MVC のビュー(`.cshtml`・`.vbhtml`。名前が `_` で始まる部分ビューとレイアウトを除く)の数です。DNN Platform と YAF.NET は画面の多くをユーザー コントロール(`.ascx`)で作るので、その数を括弧に書きました。
- **難易度**: コマンドラインの手順で動かすまでの手間です。Studio では、どのアプリも Visual Studio か Build Tools が要ります(ステップ 1 で元のアプリをビルドするため)。
  - ★ やさしい: DB が要らず、追加で要るものもほとんどない
  - ★★ ふつう: SQL Server のコンテナを動かす(DB はアプリが作る)、または確かめるのにコマンドが要る
  - ★★★ むずかしい: DB をスクリプトで作る、インストール画面での設定、数 GB のディスク、長いビルドのどれかがある

## ライセンス

FrameworkOnCore 自身のコードは [MIT License](LICENSE) です。

取り込んで手を入れている、ほかのプロジェクトのソースは、それぞれのライセンスに従います。

| 部分 | 元のソース | ライセンス |
|---|---|---|
| System.Web(`FrameworkOnCore.Runtime/`) | [WebFormsForCore](https://github.com/webformsforcore/WebFormsForCore) | MIT(`FrameworkOnCore.Runtime/LICENSE`) |
| MVC 5・Web Pages・Razor 3・Web API 2 | AspNetWebStack v3.3.0 | Apache 2.0 |
| LINQ to SQL、グラフ(DataVisualization)、モバイル コントロールなど | Microsoft の referencesource | MIT |
| System.Drawing | dotnet/runtime 6.0 の System.Drawing.Common | MIT |
| System.Data.SqlClient | dotnet/maintenance-packages 4.9.0 | MIT |
| ASP.NET の文言の日本語訳(`translations/`) | 独自に訳したもの | MIT |

変換したアプリが参照するパッケージ(CoreWCF、SkiaSharp など)は、それぞれのパッケージのライセンスに従います。
