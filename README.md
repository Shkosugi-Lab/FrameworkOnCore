# FrameworkOnCore

.NET Framework の Web アプリ(ASP.NET Web Forms・MVC・Web API・WCF、C# と VB)を、**ほぼ書き換えずに .NET 10 に移し、Linux で動かす**ための変換ツールと実行基盤です。

- 変換器が書き換えるのは、プロジェクトファイルと、.NET にない API の呼び出しだけです。画面と業務ロジックは元のまま使います。
- アプリが頼っている土台(System.Web など)は、FrameworkOnCore が .NET 10 の上に用意します。Windows と Linux の違い(ファイル名の大文字・小文字、パス、カルチャ、IIS の設定など)も土台の側で合わせます。
- 解析・変換・起動までを画面で進める Studio と、コマンドラインの変換器があります。

使い方と開発者向けの情報は [docs/development.md](docs/development.md) にあります。

## 検証シナリオ

実在のオープンソースの .NET Framework のアプリ(コーパス)を変換し、Windows と Linux のコンテナで動かす手順です。準備は [共通の準備](docs/corpora/setup.md) にあります。種類の順に並べています。最初は BlogEngine.NET をお勧めします。

| アプリ | 種類 | 言語 | 画面数 | DB | 準備と手間 |
|---|---|---|---|---|---|
| [Bootstrap2-VB-MVC](docs/corpora/bootstrap2-vb-mvc.md) | MVC 4 | VB | 23 | 不要 | Visual Studio か Build Tools |
| [MvcMovie](docs/corpora/mvcmovie.md) | MVC 5 | C# | 29 | 必要 | SQL Server のコンテナ(DB はアプリが作る) |
| [Contoso University](docs/corpora/contoso-university.md) | MVC 5 | C# | 9 | 必要 | SQL Server のコンテナ(DB はアプリが作る) |
| [eShopLegacyMVC](docs/corpora/eshop-legacy-mvc.md) | MVC 5 + Web API | C# | 7(Web API 4) | 不要 | Visual Studio か Build Tools(データはモック) |
| [MultipleEndpoints(C# と VB)](docs/corpora/wcf-multiple-endpoints.md) | WCF | C#・VB | —(サービスの操作 4) | 不要 | Visual Studio か Build Tools。確かめるのは SOAP の呼び出し(PowerShell) |
| [BlogEngine.NET 3.3.8](docs/corpora/blogengine-net.md)([Studio 版](docs/corpora/studio/blogengine-net.md)) | Web Forms | C# | 53 | 不要 | なし(データは App_Data の XML) |
| [WingtipToys](docs/corpora/wingtiptoys.md) | Web Forms | C# | 28 | 必要 | SQL Server のコンテナ(DB はアプリが作る) |
| [WingtipToys(VB)](docs/corpora/wingtiptoys-vb.md) | Web Forms | VB | 18 | 必要 | SQL Server のコンテナ(DB はアプリが作る) |

### 大規模なアプリ

画面が多く、準備に手間と時間がかかるアプリです。

| アプリ | 種類 | 言語 | 画面数 | DB | 準備と手間 |
|---|---|---|---|---|---|
| [nopCommerce 3.90](docs/corpora/nopcommerce-3.90.md) | MVC 5 | C# | 388 | 必要 | Visual Studio か Build Tools、SQL Server、インストール画面での設定、ディスク 3 GB |
| [nopCommerce 1.90](docs/corpora/nopcommerce-1.90.md) | Web Forms | C# | 293 | 必要 | Visual Studio か Build Tools、SQL Server、インストール画面での設定、ディスク 2 GB |
| [openIMIS 24.10](docs/corpora/openimis.md) | Web Forms | VB | 62 | 必要 | Visual Studio か Build Tools、SQL Server(DB を別のリポジトリのスクリプトで作る。数分)、ディスク 2 GB |

- **画面数**: Web プロジェクトのページ(`.aspx`)と MVC のビュー(`.cshtml`・`.vbhtml`。名前が `_` で始まる部分ビューとレイアウトを除く)の数です。
- **DB**: 動かすのに SQL Server が要るかどうかです。小さなアプリの DB は、SQL Server のコンテナを動かせばアプリが作ります。
- Studio では、どのアプリも Visual Studio か Build Tools が要ります(ステップ 1 で元のアプリをビルドするため)。

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
