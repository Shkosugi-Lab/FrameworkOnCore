# FrameworkOnCore

.NET Framework の Web アプリ(ASP.NET Web Forms・MVC・Web API・WCF、C# と VB)を、**ほぼ書き換えずに .NET 10 に移し、Linux で動かす**ための変換ツールと実行基盤です。

- 変換器が書き換えるのは、プロジェクトファイルと、.NET にない API の呼び出しだけです。画面と業務ロジックは元のまま使います。
- アプリが頼っている土台(System.Web など)は、FrameworkOnCore が .NET 10 の上に用意します。Windows と Linux の違い(ファイル名の大文字・小文字、パス、カルチャ、IIS の設定など)も土台の側で合わせます。
- 解析・変換・起動までを画面で進める Studio と、コマンドラインの変換器があります。

使い方と開発者向けの情報は [docs/development.md](docs/development.md) にあります。

## 検証シナリオ

実在のオープンソースの .NET Framework のアプリ(コーパス)を変換し、Windows と Linux のコンテナで動かす手順です。準備は [共通の準備](docs/corpora/setup.md) にあります。手順は、コマンドラインで進める「コマンド」と、Studio(画面)で進める「Studio」(操作する所を赤枠で囲んだ画面のキャプチャ付き)の 2 通りあります。種類の順に並べています。最初は BlogEngine.NET をお勧めします。

| アプリ | 種類 | 言語 | 画面数 | DB | 準備と手間 | 手順 |
|---|---|---|---|---|---|---|
| Bootstrap2-VB-MVC | MVC 4 | VB | 23 | 不要 | Visual Studio か Build Tools | [コマンド](docs/corpora/bootstrap2-vb-mvc.md) ・ [Studio](docs/corpora/studio/bootstrap2-vb-mvc.md) |
| MvcMovie | MVC 5 | C# | 29 | 必要 | SQL Server のコンテナ(DB はアプリが作る) | [コマンド](docs/corpora/mvcmovie.md) ・ [Studio](docs/corpora/studio/mvcmovie.md) |
| Contoso University | MVC 5 | C# | 9 | 必要 | SQL Server のコンテナ(DB はアプリが作る) | [コマンド](docs/corpora/contoso-university.md) ・ [Studio](docs/corpora/studio/contoso-university.md) |
| eShopLegacyMVC | MVC 5 + Web API | C# | 7(Web API 4) | 不要 | Visual Studio か Build Tools(データはモック) | [コマンド](docs/corpora/eshop-legacy-mvc.md) ・ [Studio](docs/corpora/studio/eshop-legacy-mvc.md) |
| MultipleEndpoints(C# と VB) | WCF | C#・VB | —(サービスの操作 4) | 不要 | Visual Studio か Build Tools。確かめるのは SOAP の呼び出し(PowerShell) | [コマンド](docs/corpora/wcf-multiple-endpoints.md) ・ [Studio](docs/corpora/studio/wcf-multiple-endpoints.md) |
| BlogEngine.NET 3.3.8 | Web Forms | C# | 53 | 不要 | なし(データは App_Data の XML) | [コマンド](docs/corpora/blogengine-net.md) ・ [Studio](docs/corpora/studio/blogengine-net.md) |
| WingtipToys | Web Forms | C# | 28 | 必要 | SQL Server のコンテナ(DB はアプリが作る) | [コマンド](docs/corpora/wingtiptoys.md) ・ [Studio](docs/corpora/studio/wingtiptoys.md) |
| WingtipToys(VB) | Web Forms | VB | 18 | 必要 | SQL Server のコンテナ(DB はアプリが作る) | [コマンド](docs/corpora/wingtiptoys-vb.md) ・ [Studio](docs/corpora/studio/wingtiptoys-vb.md) |

### 大規模なアプリ

画面が多く、準備に手間と時間がかかるアプリです。

| アプリ | 種類 | 言語 | 画面数 | DB | 準備と手間 | 手順 |
|---|---|---|---|---|---|---|
| nopCommerce 3.90 | MVC 5 | C# | 388 | 必要 | Visual Studio か Build Tools、SQL Server、インストール画面での設定、ディスク 3 GB | [コマンド](docs/corpora/nopcommerce-3.90.md) ・ [Studio](docs/corpora/studio/nopcommerce-3.90.md) |
| nopCommerce 1.90 | Web Forms | C# | 293 | 必要 | Visual Studio か Build Tools、SQL Server、インストール画面での設定、ディスク 2 GB | [コマンド](docs/corpora/nopcommerce-1.90.md) ・ [Studio](docs/corpora/studio/nopcommerce-1.90.md) |
| openIMIS 24.10 | Web Forms | VB | 62 | 必要 | Visual Studio か Build Tools、SQL Server(DB を別のリポジトリのスクリプトで作る。数分)、ディスク 2 GB | [コマンド](docs/corpora/openimis.md) ・ [Studio](docs/corpora/studio/openimis.md) |

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
