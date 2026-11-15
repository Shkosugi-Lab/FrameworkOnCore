# FrameworkOnCore

.NET Framework の Web アプリ(ASP.NET Web Forms・MVC・Web API・WCF、C# と VB)を、**ほぼ書き換えずに .NET 10 に移し、Linux で動かす**ための変換ツールと実行基盤です。

- 変換器が書き換えるのは、プロジェクトファイルと、.NET にない API の呼び出しだけです。画面と業務ロジックは元のまま使います。
- アプリが頼っている土台(System.Web など)は、FrameworkOnCore が .NET 10 の上に用意します。Windows と Linux の違い(ファイル名の大文字・小文字、パス、カルチャ、IIS の設定など)も土台の側で合わせます。
- 解析・変換・起動までを画面で進める Studio と、コマンドラインの変換器があります。

使い方と開発者向けの情報は [docs/development.md](docs/development.md) にあります。

## 検証シナリオ

実在のオープンソースの .NET Framework のアプリ(コーパス)を変換し、Windows と Linux のコンテナで動かす手順です。準備は [共通の準備](docs/corpora/setup.md)、一覧の詳しい説明は [docs/corpora/README.md](docs/corpora/README.md) にあります。最初は be をお勧めします。

| 名前 | アプリ | 言語 | 種類 | 結果 |
|---|---|---|---|---|
| [be](docs/corpora/be.md)([Studio 版](docs/corpora/studio/be.md)) | BlogEngine.NET 3.3.8 | C# | Web Forms | 動く |
| [wt](docs/corpora/wt.md) | WingtipToys | C# | Web Forms | 動く |
| [mvcmovie](docs/corpora/mvcmovie.md) | MvcMovie | C# | MVC 5 | 12 画面中 11 が元と同じ |
| [contoso](docs/corpora/contoso.md) | Contoso University | C# | MVC 5 | 12 画面中 9 が元と同じ |
| [eshop](docs/corpora/eshop.md) | eShopLegacyMVC | C# | MVC 5 + Web API | 動く |
| [wcf](docs/corpora/wcf.md) | WCF の MultipleEndpoints | C#・VB | WCF | basicHttp の口が動く |
| [vbmvc](docs/corpora/vbmvc.md) | Bootstrap2-VB-MVC | VB | MVC 4 | 動く |
| [imis](docs/corpora/imis.md) | openIMIS 24.10 | VB | Web Forms | 動く |
| [nop](docs/corpora/nop.md) | nopCommerce 1.90 | C# | Web Forms | 動く |
| [nop390](docs/corpora/nop390.md) | nopCommerce 3.90 | C# | MVC 5 | 動く |
| [mojo](docs/corpora/mojo.md) | mojoPortal 3.1.6 | C# | Web Forms | 動く |
| [n2](docs/corpora/n2.md) | N2 CMS | C# | Web Forms | 動く |
| [dnn](docs/corpora/dnn.md) | DNN Platform 9.13.10 | C# | Web Forms | 動く |
| [yaf](docs/corpora/yaf.md) | YAF.NET 3.2.15 | C# | Web Forms | 動かない(変換まで) |

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
