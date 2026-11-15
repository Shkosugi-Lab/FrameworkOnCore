# コーパスで変換を試す

実在のオープンソースの .NET Framework の Web アプリ(コーパス)を、GitHub から取って FrameworkOnCore で変換し(コマンドラインか Studio)、Windows と Linux のコンテナで動かすまでの手順です。

1. [共通の準備](setup.md) をします(一度だけ)。
2. 試すコーパスのページを開きます。種類の順に並べています。最初は **[BlogEngine.NET](blogengine-net.md)** をお勧めします(SQL Server も元のアプリのビルドも要らず、準備がいちばん少ないため)。

| アプリ | 種類 | 言語 | 画面数 | DB | 準備と手間 |
|---|---|---|---|---|---|
| [Bootstrap2-VB-MVC](bootstrap2-vb-mvc.md) | MVC 4 | VB | 23 | 不要 | Visual Studio か Build Tools |
| [MvcMovie](mvcmovie.md) | MVC 5 | C# | 29 | 必要 | SQL Server のコンテナ(DB はアプリが作る) |
| [Contoso University](contoso-university.md) | MVC 5 | C# | 9 | 必要 | SQL Server のコンテナ(DB はアプリが作る) |
| [eShopLegacyMVC](eshop-legacy-mvc.md) | MVC 5 + Web API | C# | 7(Web API 4) | 不要 | Visual Studio か Build Tools(データはモック) |
| [MultipleEndpoints(C# と VB)](wcf-multiple-endpoints.md) | WCF | C#・VB | —(サービスの操作 4) | 不要 | Visual Studio か Build Tools。確かめるのは SOAP の呼び出し(PowerShell) |
| [BlogEngine.NET 3.3.8](blogengine-net.md)([Studio 版](studio/blogengine-net.md)) | Web Forms | C# | 53 | 不要 | なし(データは App_Data の XML) |
| [WingtipToys](wingtiptoys.md) | Web Forms | C# | 28 | 必要 | SQL Server のコンテナ(DB はアプリが作る) |
| [WingtipToys(VB)](wingtiptoys-vb.md) | Web Forms | VB | 18 | 必要 | SQL Server のコンテナ(DB はアプリが作る) |

## 大規模なアプリ

画面が多く、準備に手間と時間がかかるアプリです。

| アプリ | 種類 | 言語 | 画面数 | DB | 準備と手間 |
|---|---|---|---|---|---|
| [nopCommerce 3.90](nopcommerce-3.90.md) | MVC 5 | C# | 388 | 必要 | Visual Studio か Build Tools、SQL Server、インストール画面での設定、ディスク 3 GB |
| [nopCommerce 1.90](nopcommerce-1.90.md) | Web Forms | C# | 293 | 必要 | Visual Studio か Build Tools、SQL Server、インストール画面での設定、ディスク 2 GB |
| [openIMIS 24.10](openimis.md) | Web Forms | VB | 62 | 必要 | Visual Studio か Build Tools、SQL Server(DB を別のリポジトリのスクリプトで作る。数分)、ディスク 2 GB |

- **画面数**: Web プロジェクトのページ(`.aspx`)と MVC のビュー(`.cshtml`・`.vbhtml`。名前が `_` で始まる部分ビューとレイアウトを除く)の数です。
- **DB**: 動かすのに SQL Server が要るかどうかです。小さなアプリの DB は、SQL Server のコンテナを動かせばアプリが作ります。
- Studio では、どのアプリも Visual Studio か Build Tools が要ります(ステップ 1 で元のアプリをビルドするため)。
