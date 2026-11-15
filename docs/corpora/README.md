# コーパスで変換を試す

実在のオープンソースの .NET Framework の Web アプリ(コーパス)を、GitHub から取って FrameworkOnCore で変換し(コマンドラインか Studio)、Windows と Linux のコンテナで動かすまでの手順です。

1. [共通の準備](setup.md) をします(一度だけ)。
2. 試すコーパスのページを開きます。やさしい順に並べています。最初は **[BlogEngine.NET](blogengine-net.md)** をお勧めします(SQL Server も元のアプリのビルドも要らず、準備がいちばん少ないため)。

| アプリ | 言語 | 種類 | 画面数 | 難易度 | 準備と手間 |
|---|---|---|---|---|---|
| [BlogEngine.NET 3.3.8](blogengine-net.md)([Studio 版](studio/blogengine-net.md)) | C# | Web Forms | 53 | ★ | なし(データは App_Data の XML) |
| [Bootstrap2-VB-MVC](bootstrap2-vb-mvc.md) | VB | MVC 4 | 23 | ★ | Visual Studio か Build Tools |
| [eShopLegacyMVC](eshop-legacy-mvc.md) | C# | MVC 5 + Web API | 7(Web API 4) | ★ | Visual Studio か Build Tools(DB なし。モックのデータ) |
| [MultipleEndpoints(C# と VB)](wcf-multiple-endpoints.md) | C#・VB | WCF | —(サービスの操作 4) | ★★ | Visual Studio か Build Tools。確かめるのは SOAP の呼び出し(PowerShell) |
| [WingtipToys](wingtiptoys.md) | C# | Web Forms | 28 | ★★ | SQL Server のコンテナ(DB はアプリが作る) |
| [MvcMovie](mvcmovie.md) | C# | MVC 5 | 29 | ★★ | SQL Server のコンテナ(DB はアプリが作る) |
| [Contoso University](contoso-university.md) | C# | MVC 5 | 9 | ★★ | SQL Server のコンテナ(DB はアプリが作る) |
| [nopCommerce 1.90](nopcommerce-1.90.md) | C# | Web Forms | 293 | ★★★ | Visual Studio か Build Tools、SQL Server、インストール画面での設定、ディスク 2 GB |
| [nopCommerce 3.90](nopcommerce-3.90.md) | C# | MVC 5 | 388 | ★★★ | Visual Studio か Build Tools、SQL Server、インストール画面での設定、ディスク 3 GB |
| [mojoPortal 3.1.6](mojoportal.md) | C# | Web Forms | 145 | ★★★ | Visual Studio か Build Tools、SQL Server(空の DB を作る)、セットアップ画面、ディスク 5 GB |
| [N2 CMS](n2cms.md) | C# | Web Forms | 80 | ★★★ | Visual Studio か Build Tools、追加の手順、インストール画面での設定、ディスク 6 GB |
| [openIMIS 24.10](openimis.md) | VB | Web Forms | 62 | ★★★ | Visual Studio か Build Tools、SQL Server(DB を別のリポジトリのスクリプトで作る。数分)、ディスク 2 GB |
| [DNN Platform 9.13.10](dnn-platform.md) | C# | Web Forms | 9(モジュール 105) | ★★★ | Visual Studio か Build Tools、SQL Server、インストール画面(数分)、長いビルド(Cake)、ディスク 20 GB。いちばん重い |
| [YAF.NET 3.2.15](yafnet.md) | C# | Web Forms | 3(コントロール 170) | — | 変換までで、動きません |

- **画面数**: Web プロジェクトのページ(`.aspx`)と MVC のビュー(`.cshtml`・`.vbhtml`。名前が `_` で始まる部分ビューとレイアウトを除く)の数です。DNN Platform と YAF.NET は画面の多くをユーザー コントロール(`.ascx`)で作るので、その数を括弧に書きました。
- **難易度**: コマンドラインの手順で動かすまでの手間です。Studio では、どのアプリも Visual Studio か Build Tools が要ります(ステップ 1 で元のアプリをビルドするため)。
  - ★ やさしい: DB が要らず、追加で要るものもほとんどない
  - ★★ ふつう: SQL Server のコンテナを動かす(DB はアプリが作る)、または確かめるのにコマンドが要る
  - ★★★ むずかしい: DB をスクリプトで作る、インストール画面での設定、数 GB のディスク、長いビルドのどれかがある
