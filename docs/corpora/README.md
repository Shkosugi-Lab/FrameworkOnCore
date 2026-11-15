# コーパスで変換を試す

実在のオープンソースの .NET Framework の Web アプリ(コーパス)を、GitHub から取って FrameworkOnCore で変換し(コマンドラインか Studio)、Windows と Linux のコンテナで動かすまでの手順です。

1. [共通の準備](setup.md) をします(一度だけ)。
2. 試すコーパスのページを開きます。どのコーパスにも、コマンドラインの手順(「コマンド」)と、Studio(画面)の手順(「Studio」。操作する所を赤枠で囲んだ画面のキャプチャ付き)の 2 つのページがあります。種類の順に並べています。最初は **[BlogEngine.NET](blogengine-net.md)** をお勧めします(SQL Server も元のアプリのビルドも要らず、準備がいちばん少ないため)。

| アプリ | 種類 | 言語 | 画面数 | DB | 準備と手間 | 手順 |
|---|---|---|---|---|---|---|
| Bootstrap2-VB-MVC | MVC 4 | VB | 23 | 不要 | Visual Studio か Build Tools | [コマンド](bootstrap2-vb-mvc.md) ・ [Studio](studio/bootstrap2-vb-mvc.md) |
| MvcMovie | MVC 5 | C# | 29 | 必要 | SQL Server のコンテナ(DB はアプリが作る) | [コマンド](mvcmovie.md) ・ [Studio](studio/mvcmovie.md) |
| Contoso University | MVC 5 | C# | 9 | 必要 | SQL Server のコンテナ(DB はアプリが作る) | [コマンド](contoso-university.md) ・ [Studio](studio/contoso-university.md) |
| eShopLegacyMVC | MVC 5 + Web API | C# | 7(Web API 4) | 不要 | Visual Studio か Build Tools(データはモック) | [コマンド](eshop-legacy-mvc.md) ・ [Studio](studio/eshop-legacy-mvc.md) |
| MultipleEndpoints(C# と VB) | WCF | C#・VB | —(サービスの操作 4) | 不要 | Visual Studio か Build Tools。確かめるのは SOAP の呼び出し(PowerShell) | [コマンド](wcf-multiple-endpoints.md) ・ [Studio](studio/wcf-multiple-endpoints.md) |
| BlogEngine.NET 3.3.8 | Web Forms | C# | 53 | 不要 | なし(データは App_Data の XML) | [コマンド](blogengine-net.md) ・ [Studio](studio/blogengine-net.md) |
| WingtipToys | Web Forms | C# | 28 | 必要 | SQL Server のコンテナ(DB はアプリが作る) | [コマンド](wingtiptoys.md) ・ [Studio](studio/wingtiptoys.md) |
| WingtipToys(VB) | Web Forms | VB | 18 | 必要 | SQL Server のコンテナ(DB はアプリが作る) | [コマンド](wingtiptoys-vb.md) ・ [Studio](studio/wingtiptoys-vb.md) |

## 大規模なアプリ

画面が多く、準備に手間と時間がかかるアプリです。

| アプリ | 種類 | 言語 | 画面数 | DB | 準備と手間 | 手順 |
|---|---|---|---|---|---|---|
| nopCommerce 3.90 | MVC 5 | C# | 388 | 必要 | Visual Studio か Build Tools、SQL Server、インストール画面での設定、ディスク 3 GB | [コマンド](nopcommerce-3.90.md) ・ [Studio](studio/nopcommerce-3.90.md) |
| nopCommerce 1.90 | Web Forms | C# | 293 | 必要 | Visual Studio か Build Tools、SQL Server、インストール画面での設定、ディスク 2 GB | [コマンド](nopcommerce-1.90.md) ・ [Studio](studio/nopcommerce-1.90.md) |
| openIMIS 24.10 | Web Forms | VB | 62 | 必要 | Visual Studio か Build Tools、SQL Server(DB を別のリポジトリのスクリプトで作る。数分)、ディスク 2 GB | [コマンド](openimis.md) ・ [Studio](studio/openimis.md) |

- **画面数**: Web プロジェクトのページ(`.aspx`)と MVC のビュー(`.cshtml`・`.vbhtml`。名前が `_` で始まる部分ビューとレイアウトを除く)の数です。
- **DB**: 動かすのに SQL Server が要るかどうかです。小さなアプリの DB は、SQL Server のコンテナを動かせばアプリが作ります。
- Studio では、どのアプリも Visual Studio か Build Tools が要ります(ステップ 1 で元のアプリをビルドするため)。
