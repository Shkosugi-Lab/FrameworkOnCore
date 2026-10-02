# 移植版の候補

.NET Framework と .NET の両方にあるのに、動きや API が違うものの一覧です。移植版を作る(または移植版に足す)候補として管理します。

対象にするもの:

- 同じ名前のアセンブリやパッケージがあり、動きが違うもの。
  - 例: System.Data.SqlClient の `|DataDirectory|`。.NET Framework は展開し、.NET は例外にする。
- 同じ名前のアセンブリはあるが、.NET Framework にあったメンバーが .NET に無いもの。
  - 例: `SqlConnectionStringBuilder.AsynchronousProcessing`。使う所で MissingMethodException になる。
- .NET に無いが、.NET Framework のソースが公開されていて移植できるもの(referencesource、dotnet/maintenance-packages など。MIT)。

移植版は、変換器の部品ごとの選択で「移植版」と「.NET 版」から選べるようにします。既定は移植版です。

- 仕組み: `rules/packages.json` の `portOptions` と、`FrameworkOnCore.Analysis/catalog/components.json` の部品と選択肢。
- 詳しくは `README.md` の「System.Data.SqlClient の移植」。

## 移植済み

| 対象 | 違い(.NET 版) | 移植版 | 選択 |
|---|---|---|---|
| System.Data.SqlClient | 接続文字列の `\|DataDirectory\|` を拒否する | FrameworkOnCore.Data.SqlClient(maintenance-packages 4.9.0)。.NET Framework と同じく展開する | `sql-client`(移植版 / .NET 版) |
| System.Drawing.Common | .NET 7 から Windows のみ(Linux は例外) | FrameworkOnCore.Drawing.Common(dotnet/runtime 6.0、libgdiplus) | `system-drawing`(移植版のみ) |
| System.Data.Linq | .NET に無い | FrameworkOnCore.Data.Linq(referencesource) | `linq-to-sql`(移植版 / 対応しない) |
| System.Web.DataVisualization | .NET に無い | FrameworkOnCore.Web.DataVisualization(referencesource) | `charts`(移植版 / 対応しない) |
| System.Web.Mobile | .NET に無い | FrameworkOnCore.Web.Mobile(referencesource) | `mobile-controls`(移植版 / 対応しない) |

## 候補

優先度の目安:
- 高: 見つかったアプリがあり、普通に使われる。
- 中: 見つかったアプリがある。
- 低: まだ見つかっていない、または使う所が限られる。

### System.Data.SqlClient(移植版に足す)

.NET Framework 4.8 の System.Data.dll と移植版の公開 API を比べた結果です(2026-10-02)。SqlTypes は .NET の System.Data.Common にあるので除いています。

| 違い | .NET Framework | 移植版・.NET 版 | 見つかった所 | 優先度 |
|---|---|---|---|---|
| 接続文字列のキーワード `Asynchronous Processing`(`async`)、`Connection Reset`、`Network Library`(`net`、`network`) | 受け付ける(`Asynchronous Processing` は 4.5 から無視、`Network Library=dbmssocn` は TCP) | 例外(Keyword not supported など)。古いアプリの接続文字列によくある | n2 の接続文字列の例(docs/example.web.config の SQL Server 2000 用: `Network Library=DBMSSOCN`)。コーパスの設定ファイルには無い | 高 |
| `SqlConnectionStringBuilder.AsynchronousProcessing` | ある(4.5 から無視) | 無い(MissingMethodException) | wt の Elmah.dll | 高 |
| `SqlConnectionStringBuilder` の `ConnectionReset`、`NetworkLibrary`、`TransparentNetworkIPResolution`、`ContextConnection` | ある | 無い | — | 中 |
| `SqlParameterCollection.Add(string, object)` | ある(廃止予定) | 無い | — | 中 |
| `SqlDataSourceEnumerator`、`SqlClientFactory.CreateDataSourceEnumerator` | ある(サーバーの一覧) | 無い | — | 低 |
| Always Encrypted(`SqlColumnEncryption*`、`ColumnEncryptionSetting` など)、Azure AD 認証(`SqlAuthenticationProvider`、`Authentication`) | ある | 無い(Microsoft.Data.SqlClient にはある) | — | 低 |
| `SqlConnection.EnlistDistributedTransaction`(COM+)、`SqlCommand.NotificationAutoEnlist` | ある | 無い | — | 低 |

### ほかのアセンブリ

| 対象 | 違い | 見つかった所 | ソース | 優先度 |
|---|---|---|---|---|
| System.Data.Services.Client(WCF Data Services のクライアント) | .NET に無い | nop390 の Microsoft.WindowsAzure.Storage.dll(`DataServiceContext.SaveChanges` など) | referencesource | 中 |
| System.ServiceModel のサーバー側(`BindingElement.BuildChannelListener` など) | .NET はクライアントのみ | mojo の Microsoft.ApplicationServer.Caching.Core.dll、Microsoft.WindowsFabric.Common.dll | 移植ではなく CoreWCF(部品 `wcf-server` の予定の選択肢) | 中 |

## 移植では対応できないもの(参考)

.NET のランタイムそのもの(System.Private.CoreLib)の違いは、アセンブリを差し替えられないので移植の対象になりません。変換器の書き換え(`platformReplacements`、`dllCallReplacements`)か、既知の差として扱います。

- AppDomain の作成、Remoting、Code Access Security(Evidence など)
  - 見つかった所: be・dnn・mojo・n2・nop390 の System.Web.WebPages.Deployment.dll、NuGet.Core.dll、imis の ReportViewer、n2 の log4net.dll。
- `LambdaExpression.CompileToMethod`(be の SimpleInjector.dll)、`Debug.Listeners`、`ServicePointManager.CertificatePolicy`(mojo の Mono.Security.dll)。
- 数値の書式の丸め。wt の ¥23 と ¥22 で、`double` の 22.5 の通貨書式が .NET Framework と .NET で違う。
- デザイナー(System.Design、`ControlDesigner` など)。Visual Studio のデザイン時だけのもの。

パッケージの版の違い(.NET Framework と .NET の違いではないもの)も、ここには載せません。
- 例: imis の System.Data.SQLite.Linq.dll が使う `SQLiteConnection.ParseConnectionString` は、置き換えた System.Data.SQLite 1.0.119 で変わっている。

## 候補の見つけ方と更新

次のどれかで見つけたら、この一覧に足します(見つかった所も書く)。移植したら「移植済み」に移します。

- 変換レポートの `[Unsupported] <DLL>: members that neither .NET 10 nor the compatibility assembly has`。そのメンバーのアセンブリが .NET にもあれば候補。
- API の解析で「動きが違う」の部品。
- 元のアプリとの比較(ParityTest、IIS との比較)で見つかった違いのうち、ライブラリの動きの違いによるもの。
- .NET Framework のアセンブリと、.NET 版・移植版の公開 API の比較(MetadataLoadContext で両方の公開メンバーを並べて差を取る)。
