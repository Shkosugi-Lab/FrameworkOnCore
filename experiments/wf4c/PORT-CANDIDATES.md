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
| System.Data.SqlClient | 接続文字列の `\|DataDirectory\|` を拒否する。`Asynchronous Processing`・`Network Library`・`Connection Reset` を拒否し、`SqlConnectionStringBuilder` にそのプロパティが無い | FrameworkOnCore.Data.SqlClient(maintenance-packages 4.9.0)。.NET Framework 4.8 と同じく受け付ける(1.6.5-w2l.12 から。下の「済」) | `sql-client`(移植版 / .NET 版) |
| System.Drawing.Common | .NET 7 から Windows のみ(Linux は例外) | FrameworkOnCore.Drawing.Common(dotnet/runtime 6.0、libgdiplus) | `system-drawing`(移植版のみ) |
| System.Data.Linq | .NET に無い | FrameworkOnCore.Data.Linq(referencesource) | `linq-to-sql`(移植版 / 対応しない) |
| System.Web.DataVisualization | .NET に無い | FrameworkOnCore.Web.DataVisualization(referencesource) | `charts`(移植版 / 対応しない) |
| System.Web.Mobile | .NET に無い | FrameworkOnCore.Web.Mobile(referencesource) | `mobile-controls`(移植版 / 対応しない) |
| WCF のサーバー側(.svc、system.serviceModel、System.ServiceModel.Activation) | .NET はクライアントのみ | 移植ではなく CoreWCF で動かす: FrameworkOnCore.ServiceModel(System.ServiceModel.Activation の名前。AspNetCompatibilityRequirementsAttribute を持つ)。.svc と serviceActivations、web.config のエンドポイント・バインド・動作、WCF 4 の既定のエンドポイント、WSDL。クライアントの型の FaultException は CoreWCF の障害として返す | `wcf-server`(CoreWCF / 対応しない) |

## 候補

優先度の目安:
- 高: 見つかったアプリがあり、普通に使われる。
- 中: 見つかったアプリがある。
- 低: まだ見つかっていない、または使う所が限られる。

### System.Data.SqlClient(移植版に足す)

.NET Framework 4.8 の System.Data.dll と移植版の公開 API を比べた結果です(2026-10-02)。SqlTypes は .NET の System.Data.Common にあるので除いています。

| 違い | .NET Framework | 移植版・.NET 版 | 見つかった所 | 優先度 |
|---|---|---|---|---|
| **済(w2l.12)** 接続文字列のキーワード `Asynchronous Processing`(`async`)、`Connection Reset`、`Network Library`(`net`、`network`) | 受け付ける(`Asynchronous Processing` と `Connection Reset` は 4.5 から無視、`Network Library=dbmssocn` は TCP) | .NET 版は例外(Keyword not supported など)。古いアプリの接続文字列によくある。移植版は .NET Framework と同じく受け付ける。`Context Connection=true`(SQL CLR の中の接続)は、無いので例外のまま | n2 の接続文字列の例(docs/example.web.config の SQL Server 2000 用: `Network Library=DBMSSOCN`)。コーパスの設定ファイルには無い | 高 |
| **済(w2l.12)** `SqlConnectionStringBuilder` の `AsynchronousProcessing`、`ConnectionReset`、`NetworkLibrary`、`ContextConnection` | ある | .NET 版には無い(MissingMethodException)。移植版にはある | wt の Elmah.dll(`AsynchronousProcessing`)。変換レポートから消えた | 高 |
| **一部済(w2l.13)** `SqlConnectionStringBuilder.TransparentNetworkIPResolution` と接続文字列のキーワード(.NET Framework 4.6.1 から) | ある。最初の IP を短い待ち時間(500 ミリ秒以上)で試し、だめなら並列に試す | 移植版は受け付ける。接続の仕方は .NET 版と同じ(順に試す。並列は MultiSubnetFailover のとき)。同じ試し方には SNI の変更が要る | — | 低 |
| **済(w2l.13)** `SqlParameterCollection.Add(string, object)` | ある(廃止予定) | .NET 版には無い。移植版にはある | — | 中 |
| **済(w2l.13)** `SqlDataSourceEnumerator`、`SqlClientFactory.CreateDataSourceEnumerator` | ある(サーバーの一覧。ネイティブの SNI に問い合わせる) | .NET 版には無い。移植版は SQL Server Browser にブロードキャストで問い合わせる(SSRP、UDP 1434。ネイティブの SNI と同じ問い合わせ)。表(列、行)は .NET Framework と同じ。この PC では、.NET Framework の方は 36 秒かけて 0 件、移植版は 2 秒で SQLEXPRESS を返した | — | 低 |
| Always Encrypted(`SqlColumnEncryption*`、`ColumnEncryptionSetting` など)、Azure AD 認証(`SqlAuthenticationProvider`、`Authentication`) | ある | 無い(Microsoft.Data.SqlClient にはある) | — | 低 |
| `SqlConnection.EnlistDistributedTransaction`(COM+)、`SqlCommand.NotificationAutoEnlist` | ある | 無い | — | 低 |

### ほかのアセンブリ

| 対象 | 違い | 見つかった所 | ソース | 優先度 |
|---|---|---|---|---|
| System.Data.Services.Client(WCF Data Services のクライアント) | .NET に無い | be(ギャラリー)、mojo。nop390 の Microsoft.WindowsAzure.Storage.dll(`DataServiceContext.SaveChanges` など。こちらはパッケージ Microsoft.Data.Services.Client 5.x の、.NET Framework 向けにしかない API) | referencesource には無い。OData/odata.net の maintenance-5.x(`WCFDataService/Client`、MIT。Microsoft.Data.Services.Client 5.x。OData の Microsoft.Data.OData・Edm・System.Spatial に依存) | 中 |
| WCF のサーバー側の残り: netTcpBinding、enableWebScript(ASP.NET AJAX の .svc/js と {"d":...})、独自の ServiceHostFactory、操作の中の HttpContext.Current(aspNetCompatibilityEnabled)、サービスの中の `OperationContext.Current`(.NET の WCF クライアントの型でコンパイルされ、CoreWCF の中では null) | .NET はクライアントのみ。.svc と web.config のサービスは CoreWCF で動く(w2l.14、上の「移植済み」) | mojo の .svc(独自の ServiceHostFactory)。mojo の Microsoft.ApplicationServer.Caching.Core.dll、Microsoft.WindowsFabric.Common.dll(`BindingElement.BuildChannelListener` など、チャネルの実装) | CoreWCF | 中 |

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
