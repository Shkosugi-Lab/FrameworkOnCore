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
| DbProviderFactories(System.Data.Common) | 構成ファイル(machine.config・web.config の `system.data/DbProviderFactories`)を読まず、.NET Framework が組み込みで持つプロバイダー(System.Data.SqlClient・OleDb・Odbc)も登録されていない。`GetFactory("System.Data.SqlClient")` が ArgumentException(SqlDataSource の `ProviderName="System.Data.SqlClient"`、アプリのコード) | 移植ではなく FrameworkOnCore.Web が登録する: 最初の要求の初期化で、.NET Framework の組み込みの 3 つと web.config の `add`・`remove`・`clear` を `DbProviderFactories.RegisterFactory` で(型の名前で。解決は使う時。2026-10-11)。SqlDataSource は要求の外でも組み込みの 3 つを登録する。Web アプリ以外(コンソール)は対象外 | なし(常に) |
| WCF のサーバー側(.svc、system.serviceModel、System.ServiceModel.Activation) | .NET はクライアントのみ | 移植ではなく CoreWCF で動かす: FrameworkOnCore.ServiceModel(System.ServiceModel.Activation の名前。AspNetCompatibilityRequirementsAttribute を持つ)。.svc と serviceActivations、web.config のエンドポイント・バインド・動作、WCF 4 の既定のエンドポイント、WSDL、ASP.NET AJAX の enableWebScript(w2l.15)。クライアントの型の FaultException は CoreWCF の障害として返す | `wcf-server`(CoreWCF / 対応しない) |
| double・float の文字列化と読み取り(System.Private.CoreLib) | .NET Core 3.0 から、書式なしは往復できる最短の桁(0.1 + 0.2 は `0.30000000000000004`。.NET Framework は 15 桁で `0.3`、float は 7 桁)、負のゼロと 0 に丸まる負の数は符号付き(`-0`、`-0.00`)、桁を決めた書式は 2 進の値そのものを偶数に丸める(2.675 の N2 は 2.67、22.5 の C0 は ¥22。.NET Framework は 15 桁から四捨五入で 2.68、¥23)。大きすぎる数の Parse は無限大(.NET Framework は OverflowException)。TimeSpan.From*(double) はミリ秒に丸めない | 移植ではなく置き換え: FrameworkOnCore.Compat の FrameworkNumbers・FrameworkParsing(.NET Framework の number.cpp の FormatDouble と同じく、15 桁〈float は 7 桁、R・G16 以上・E15 以上は 17 桁〉を正確な値から四捨五入で取り、その桁の decimal〈範囲外は整数〉を書式化)。変換器が、アプリの文字列化(ToString、連結、補間、string.Format の引数、Convert.ToString、Append・Write、VB の & と CStr)を書き換える(FOC1008。式ツリーの中は除く)。FrameworkOnCore の System.Web も同じ実装(System.Web.Util.FrameworkNumbers)で、バインドした値(Eval、BoundField などの書式、リストの項目、`<%= %>`、JavaScriptSerializer)を書き、サーバーがコンパイルするコード(ページの式、App_Code、CodeFile)を同じ解析で書き換える(w2l.18。2026-10-19)。.NET Framework 4.8 との比較 2,647,080 件(値・書式 58・カルチャ 9、double と float)で一致 | `floating-point`(.NET Framework と同じ / .NET のまま) |

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
| WCF のサーバー側の残り: netTcpBinding、独自の ServiceHostFactory、操作の中の HttpContext.Current(aspNetCompatibilityEnabled)、サービスの中の `OperationContext.Current`(.NET の WCF クライアントの型でコンパイルされ、CoreWCF の中では null) | .NET はクライアントのみ。.svc と web.config のサービスは CoreWCF で動く(w2l.14、上の「移植済み」) | mojo の .svc(独自の ServiceHostFactory)。mojo の Microsoft.ApplicationServer.Caching.Core.dll、Microsoft.WindowsFabric.Common.dll(`BindingElement.BuildChannelListener` など、チャネルの実装) | CoreWCF | 中 |

## 移植では対応できないもの(参考)

.NET のランタイムそのもの(System.Private.CoreLib)の違いは、アセンブリを差し替えられないので移植の対象になりません。変換器の書き換え(`platformReplacements`、`dllCallReplacements`)か、既知の差として扱います。

- AppDomain の作成、Remoting、Code Access Security(Evidence など)
  - 見つかった所: be・dnn・mojo・n2・nop390 の System.Web.WebPages.Deployment.dll、NuGet.Core.dll、imis の ReportViewer、n2 の log4net.dll。
- `LambdaExpression.CompileToMethod`(be の SimpleInjector.dll)、`Debug.Listeners`、`ServicePointManager.CertificatePolicy`(mojo の Mono.Security.dll)。
- (対応済み: 上の「移植済み」の double・float の文字列化。wt の ¥23 と ¥22 は一致するようになった。2026-10-19)
- デザイナー(System.Design、`ControlDesigner` など)。Visual Studio のデザイン時だけのもの。
- 文字列のハッシュ値のランダム化。.NET はプロセスごとに `string.GetHashCode` を変える(.NET Framework は変えない)ため、Hashtable の列挙順が違う。
  StateBag(項目が多いと Hashtable)が保存する ViewState の項目の並び、WebPartManager.AvailableTransformers の並び。中身は同じ(コントロールの比較で見つけた。2026-10-11)。
- Thread.Abort が無い。ASP.NET は executionTimeout を超えた要求のスレッドを中断する。ポートは時間切れの印を付けて次の段階で
  "Request timed out." にする(2026-10-12。それまでは要求が終わらなくなっていた)。ページのコードは最後まで動く。
- AppDomain のデータ(`.appDomain`、`.appPath`、`.appVPath`、`.appId`)。ポートはロード コンテキストに置いていて、`AppDomain.GetData` で見る
  コード(System.Web.Services の既定値、System.Configuration、アプリのライブラリ)が ASP.NET の外と判断した。AppDomain にも置く(2026-10-12)。
- IIS・http.sys の要求の制限(requestFiltering の本文 30,000,000 バイト・URL 4,096・クエリ 2,048、http.sys のパスの区切り 260)は Kestrel に無いか
  値・状態が違う(Kestrel の本文の上限は例外としてアプリに届く)。ホストが web.config の requestLimits で IIS と同じく拒否する(2026-10-12)。
- Kestrel のデュアルモードのソケットは IPv4 の接続元を `::ffff:a.b.c.d` で渡す。IIS の REMOTE_ADDR は `a.b.c.d`(2026-10-12)。
- ICU に無いカルチャ名(`new CultureInfo("xx-YY")`)。.NET Framework は不変カルチャのデータで作る。.NET(ICU)は既定のロケールのデータ。
- `ExternalException`(HttpException の基底)のメッセージが null の時。.NET は "External component has thrown an exception." を入れ、
  .NET Framework は Exception の "Exception of type '...' was thrown." のまま。Page の HttpUnhandledException(メッセージなし)で見つけた。
  FrameworkOnCore.Web の HttpException が .NET Framework の文を返す(2026-10-12)。
- `CallContext` の IllogicalCallContext(`HostContext`、`SetData`)。.NET Framework はスレッドのもの(await・タスクに流れない)。.NET には無く、
  ポートの代わりは AsyncLocal(流れる)だった。`HttpContext.Current` が await の後で null になった。スレッドのものにした(2026-10-12)。
- FileSystemWatcher の通知には変更の時刻が無い(処理した時刻だけ)。ASP.NET のネイティブのファイル監視は完了の時刻を持ち、監視を始める前の
  変更を除く。ポートは監視を始めた時のファイルの属性と比べる(2026-10-12)。
- System.Web という名前のアセンブリが .NET の共有フレームワークにもある(4.0.0.0、`HttpUtility` などの型転送だけ)。それを先に読んだプロセス(.NET の MSBuild など)には
  FrameworkOnCore の System.Web(10.0.29.0)を同じコンテキストに読み込めない。アプリ・ツールは deps.json で FrameworkOnCore のものを選ぶ(ビルドタスクのデザイナー
  ファイルの生成で見つけた。2026-10-12)。
- `DictionaryEntry.ToString`。.NET 5 から「[キー, 値]」を返す(.NET Framework は型の名前)。
- 例外のメッセージ(CoreLib・ほかのアセンブリの文言)。.NET は値を入れる(`The input string 'abc' was not in a correct format.`、`The given key 'x' was not present in the dictionary.`)、
  引数名を `(Parameter 'x')` と書く(.NET Framework は「\r\nParameter name: x」)、Regex の構文エラーは RegexParseException、Base64・Substring・PadLeft なども文が違う。
  `lblError.Text = ex.Message` のアプリでは画面に出る。BclProbe(samples/BclProbe)の Errors.aspx で約 60 件(2026-10-19)。
- Unicode の版。.NET は新しい文字の分類を持つ(U+20BF ₿ は CurrencySymbol、.NET Framework は OtherNotAssigned)、StringInfo は拡張書記素クラスター(絵文字の ZWJ 列が 1 つ)、
  `\w` が新しい文字に合う。UTF-8 の不正なバイト列の置き換えの数(U+FFFD)も違う。
- Linux の照合(ICU)。Windows は UseNls で .NET Framework と同じ。Linux では `"abc".IndexOf("\0")` が 0、ゼロ幅の文字・ß と ss・カナの比較、並び順(`co-op` と `coop`、記号、全角)が違う。
- `Environment.NewLine`。Linux では `\n`(XmlSerializer のインデント、例外の文、WriteLine)。.NET Framework は常に `\r\n`。
- タイムゾーンの表示名。.NET は UI カルチャの名前(ICU)、.NET Framework は OS の言語の名前。Linux は IANA の ID(`TimeZoneInfo.Local.Id` が `Asia/Tokyo`)と歴史(東京は 1948〜51 年に夏時間があり SupportsDaylightSavingTime が真)。
  オフセットと変換は同じ: 変換器はコンテナ・systemd に元のサーバーのタイムゾーンを `TZ` で渡す(capture-culture.ps1 の TimeZone。無ければ変換したマシンの。2026-10-19。それまでコンテナは UTC だった)。
- XmlSerializer の名前空間の宣言の順(`xmlns:xsi` と `xmlns:xsd`)、制御文字(.NET Framework は `&#x1;` で書き、.NET は例外)。XmlConvert・DataSet・XDocument の double の `R`(.NET Framework は 17 桁、.NET は最短)。
- `Math.Sin` などの結果の最後の桁(C ランタイムの違い。`Math.Sin(Math.PI)` が 1.22460635382238E-16 と 1.22464679914735E-16)。
- `System.Web.HttpUtility`。.NET の共有フレームワークにもあり、FrameworkOnCore の System.Web は .NET のものへ型転送している(.NET 10 でアプリの `HttpUtility` は System.Web.HttpUtility.dll のもの)。
  `ParseQueryString(...).ToString()` が空のキーの `=` を落とす(`b&=c` が `b&c`)。web.config の `httpRuntime encoderType`(AntiXssEncoder など)をアプリの `HttpUtility.HtmlEncode` が使わない(候補: FrameworkOnCore の HttpUtility を使う。CS0433 を避ける参照の仕方が要る)。
- パスに使えない文字(`<`、`>`、`"`)。.NET Framework のファイルの API は先に検査して ArgumentException(Illegal characters in path)、.NET はシステムに任せる
  (Windows は IOException、Linux では使える文字なので見つからない)。Xml コントロールの DocumentSource などで見つけた。
- OLE DB(System.Data.OleDb)は Windows のみ。Linux ではパッケージに実装が無い(`OleDbFactory.Instance` が null)。AccessDataSource(Access のファイル)は
  Linux では使えない。FrameworkOnCore.Web は NullReferenceException ではなく PlatformNotSupportedException で知らせる(2026-10-11)。

パッケージの版の違い(.NET Framework と .NET の違いではないもの)も、ここには載せません。
- 例: imis の System.Data.SQLite.Linq.dll が使う `SQLiteConnection.ParseConnectionString` は、置き換えた System.Data.SQLite 1.0.119 で変わっている。

## 候補の見つけ方と更新

次のどれかで見つけたら、この一覧に足します(見つかった所も書く)。移植したら「移植済み」に移します。

- 変換レポートの `[Unsupported] <DLL>: members that neither .NET 10 nor the compatibility assembly has`。そのメンバーのアセンブリが .NET にもあれば候補。
- API の解析で「動きが違う」の部品。
- 元のアプリとの比較(ParityTest、IIS との比較)で見つかった違いのうち、ライブラリの動きの違いによるもの。
- .NET Framework のアセンブリと、.NET 版・移植版の公開 API の比較(MetadataLoadContext で両方の公開メンバーを並べて差を取る)。
