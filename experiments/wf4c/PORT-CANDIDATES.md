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
| double・float の文字列化と読み取り(System.Private.CoreLib) | .NET Core 3.0 から、書式なしは往復できる最短の桁(0.1 + 0.2 は `0.30000000000000004`。.NET Framework は 15 桁で `0.3`、float は 7 桁)、負のゼロと 0 に丸まる負の数は符号付き(`-0`、`-0.00`)、桁を決めた書式は 2 進の値そのものを偶数に丸める(2.675 の N2 は 2.67、22.5 の C0 は ¥22。.NET Framework は 15 桁から四捨五入で 2.68、¥23)。大きすぎる数の Parse は無限大(.NET Framework は OverflowException)。TimeSpan.From*(double) はミリ秒に丸めない | 移植ではなく置き換え: FrameworkOnCore.Compat の FrameworkNumbers・FrameworkParsing(.NET Framework の number.cpp の FormatDouble と同じく、15 桁〈float は 7 桁、R・G16 以上・E15 以上は 17 桁〉を正確な値から四捨五入で取り、その桁の decimal〈範囲外は整数〉を書式化)。変換器が、アプリの文字列化(ToString、連結、補間、string.Format の引数、Convert.ToString、Append・Write、VB の & と CStr)を書き換える(FOC1008。式ツリーの中は除く)。FrameworkOnCore の System.Web も同じ実装(System.Web.Util.FrameworkNumbers)で、バインドした値(Eval、BoundField などの書式、リストの項目、`<%= %>`、JavaScriptSerializer)を書き、サーバーがコンパイルするコード(ページの式、App_Code、CodeFile)を同じ解析で書き換える(w2l.18。2026-10-19。platformReplacements もサイトの bin の frameworkoncore.platform.txt で同じく)。.NET Framework 4.8 との比較 2,647,080 件(値・書式 58・カルチャ 9、double と float)で一致 | `floating-point`(.NET Framework と同じ / .NET のまま) |
| Visual Basic のランタイム(Microsoft.VisualBasic) | StrConv は Windows 以外で PlatformNotSupportedException(LCMapString)。Asc は カルチャの ANSI コードページ(.NET Framework はシステムの: Shift_JIS の Asc("あ") は -32096、.NET の en-US では 63)。Str・Format・FormatNumber・FormatCurrency・FormatPercent の double は .NET の桁と丸め。CDbl の大きすぎる数は無限大 | 移植ではなく置き換え: FrameworkOnCore.Compat の VisualBasicText(platformReplacements、FOC1008 の vbparse)。StrConv は Windows ではランタイムのもの、ほかでは Windows と同じ変換(ロケールの ANSI コードページで往復。Windows の StrConv と比べるテスト)(2026-10-19) | `floating-point`(数値の分)、`encoding-default`(Asc) |
| System.Configuration(System.Configuration.ConfigurationManager の移植) | — | FrameworkOnCore.Configuration(referencesource)。.NET Framework では System.dll にあったクラス(AppSettingsReader、ApplicationSettingsBase、LocalFileSettingsProvider、System.Net の構成)のメッセージ 37 件がリソースに無く、例外が ArgumentNullException(format)になっていた。内部のファクトリが無いアセンブリ名(WebFormsForCore.Configuration)で型を探していて `Properties.Settings.Default` が動かなかった。接続文字列の `\|DataDirectory\|` を読むときに展開していた(.NET Framework は値のまま。machine.config の LocalSqlServer が `C:\...\App_Data\aspnetdb.mdf` に見えた): 自分で展開するプロバイダー(System.Data.SqlClient〈FrameworkOnCore の移植版〉・OleDb・名前の無いもの)には値のまま返す。ConfigProbe で見つけた(2026-10-19) | なし(常に) |
| .NET Framework の言語パック(System.Web の文言の日本語など) | .NET に言語パックは無い。言語パックのあるサーバー(日本語の Windows では普通 ja)で UI カルチャがその言語のとき、Calendar の「カレンダー」、Login・Wizard・GridView のコマンドの文字、エラーページなどがその言語になる。FrameworkOnCore は英語だけだった | 移植ではなく独自の翻訳: リポジトリの `translations/<言語>/<アセンブリ>/<リソース>.<言語>.resx`(英語の原文から訳したもの。MIT。GitHub で管理し、`translations/check.ps1` と単体テストで検査)を各パッケージのサテライトにする。日本語は System.Web の全 3,369 件と、ほかの 12 アセンブリの全 4,061 件(2026-10-20)(画面のレイアウトが崩れないよう、.NET Framework の日本語の文言より表示幅が広くならない。check.ps1 が幅だけを比べる)。使う言語は Studio の「アプリの設定」の「言語パック」(設定 `languages`。英語と 23 言語から選ぶ。既定は英語と元のサーバーに言語パックがあった言語〈capture-culture.ps1 の LanguagePacks〉)。文言は Microsoft の訳と同じとは限らない(Calendar の「前の月を表示」と「先月へ移動」)。Microsoft の言語パックは使用許諾(.NET Framework と言語パックの Supplemental License Terms 1.c)が Windows 以外で動かすための配布を禁じるので、Linux に配置しないとき(`--deploy none`)に選んだ場合だけ使う(2026-10-19) | `language-packs`(FrameworkOnCore の翻訳 / Microsoft の言語パック〈Windows のみ〉)、設定 `languages` |
| CultureInfo.DisplayName・Parent・EnglishName・NativeName | DisplayName は Windows の表示言語の名前(.NET Framework は mscorlib の名前を UI カルチャの言語で: 言語パックが無ければ英語の「Chinese (Simplified, PRC)」。最初に作った要求の UI カルチャのまま)。zh-CN・zh-TW の親が zh-Hans・zh-Hant(.NET Framework は zh-CHS・zh-CHT)。Linux の EnglishName・NativeName は ICU の名前(「Chinese (Taiwan)」「中文（台灣）」) | 置き換え: FrameworkOnCore.Compat の FrameworkCultures(platformReplacements。インスタンスのプロパティの読み取りも書き換える)。DisplayName は、訳のある言語(言語パックがあった言語)の UI カルチャなら .NET の名前、それ以外は元のサーバーの Windows の英語名(カルチャ プロファイルの EnglishName。Microsoft の言語パックを選んだ Windows 配置だけは mscorlib の名前の表)。mscorlib 独自の英語名(「Chinese (Simplified, PRC)」「Hong Kong S.A.R.」)は再現しない。EnglishName・NativeName は元のサーバーのカルチャ プロファイル。FrameworkOnCore の System.Web の Page.Culture・UICulture も同じ。App_GlobalResources・App_LocalResources は .NET Framework の親の並びで探す(Strings.zh-CHS.resx が zh-CN で見つかる)(2026-10-19) | `language-packs`(DisplayName)、ほかは常に |
| System.Web が作るカルチャのデータ(Linux) | ICU のデータに元のサーバーのカルチャ プロファイルを入れても表せないもの: th-TH の週の始まり(月曜)、ar-SA の 2 桁の年(dd/MM/yy)と日付の区切り、es-419 の通貨記号 XDR、fa-IR の通貨の桁、ネイティブの数字、GetAllDateTimePatterns の 2 番目以降、Calendar.TwoDigitYearMax(th-TH は Windows 2592、ICU 2572。2 桁の年の解釈が変わる) | FrameworkOnCore の System.Web が、web.config の globalization・ページの Culture・UICulture・auto・InitializeCulture で作るカルチャに App_Data\culture-profile.json の値を入れる(CultureOverlay。読み取り専用にする前)。capture-culture.ps1 は TwoDigitYearMax、名前、指定された中立カルチャ(zh-Hans)も記録する。アプリが `new CultureInfo(...)` で作るものは ICU のデータのまま(2026-10-19) | なし(プロファイルがあれば常に) |
| ファイルの一覧(Directory.GetFiles など) | Linux はフォルダーの持つ順(NTFS は名前順)、パターンは大文字小文字を区別する(`*.txt` が readme.TXT を返さない)。8.3 名で合っていたもの(`*.xls` の book.xlsx、`*.htm` の page.html、`*.` の .hidden)を返さない(Windows でも)。Path.GetInvalidPathChars は 33(.NET Framework 36。Linux 1)、GetInvalidFileNameChars は Linux で 2 | 置き換え: FrameworkOnCore.Compat の FrameworkFiles(Directory・DirectoryInfo の GetFiles・EnumerateFiles・GetDirectories・GetFileSystemEntries など、Path の 2 つ。AppContext の FrameworkOnCore.ShortNamePatterns=false で 8.3 名の分を止める)。FrameworkOnCore 自身の一覧(テーマの CSS・スキン、App_Browsers、App_Code、bin の DLL、バンドルの IncludeDirectory)も名前順で、`*.dll`・`*.browser` は大文字小文字を区別しない。DataFileProbe で見つけた(2026-10-19) | なし(常に) |
| MVC 5・Web Pages・Razor 3(Microsoft.AspNet.Mvc・.WebPages・.Razor) | .NET に無い。Microsoft の .NET Framework 向け DLL は .NET 10 でも動くが、その許諾(.NET Library EULA)は逆アセンブルを禁じ、変換器の DLL の書き換えに当たる。.NET の数値の動き(0.30000000000000004、N2 の偶数丸め、"1e400" を ∞ で受け付ける) | FrameworkOnCore.Web.Mvc・.Web.WebPages・.Web.WebPages.Razor・.Web.WebPages.Deployment・.Web.Helpers・.Web.Razor(AspNetWebStack v3.3.0、Apache 2.0。2026-10-22) | なし(常に移植版) |
| Web API 2(Microsoft.AspNet.WebApi・.WebApi.Core・.WebHost・.Client・.Owin・.Cors) | 同上(許諾、.NET の数値の動き、yaf の static readonly への書き込みが FieldAccessException) | FrameworkOnCore.Net.Http.Formatting・.Web.Http・.Web.Http.WebHost・.Web.Http.Owin・.Web.Cors・.Web.Http.Cors(AspNetWebStack v3.3.0。2026-10-22) | なし(常に移植版) |
| .NET の DataAnnotations の文言(System.ComponentModel.Annotations) | .NET Framework は言語パックで日本語(「EnrollmentDate フィールドが必要です。」)、.NET は英語(「The EnrollmentDate field is required.」)。MVC の検証の `data-val-required` に出る(mvcmovie、contoso)。.NET は自分のアセンブリのサテライトをそのアセンブリの隣(共有フレームワークのフォルダー)でしか探さない | 独自の翻訳(62 件。translations\ja\System.ComponentModel.Annotations)のサテライトを FrameworkOnCore.Web のパッケージに入れ(bin\ja)、System.Web の FrameworkSatellites が見つからなかったサテライトを bin\&lt;カルチャ&gt; で探す(2026-10-22)。文言は FrameworkOnCore の訳(「EnrollmentDate は必須です。」)で、元のサーバーと同じではない | 部品 language-packs の言語(他の翻訳と同じ) |
| UTF-7(`Encoding.GetEncoding("utf-7")`) | .NET 5 から NotSupportedException(SYSLIB0001。XSS の危険のため) | 変換器: ソースが UTF-7 を名指しするときだけ `System.Text.Encoding.EnableUnsafeUTF7Encoding` を入れる(2026-10-19) | なし |
| Encoding.Default と ANSI コードページ(System.Web の `<globalization fileEncoding>` の既定、コンパイラーのソースの読み方) | .NET の Encoding.Default は UTF-8(.NET Framework はシステムの ANSI コードページ: 日本語の Windows では Shift_JIS)。System.Web は fileEncoding の無いページ・App_Code・CodeFile を Encoding.Default で読むので、BOM の無い Shift_JIS のページは U+FFFD になり、App_Code はコンパイルできない(CS1001)。web.config の `encoding="shift_jis"` は CodePagesEncodingProvider を登録するまで読めない(XmlException。UseWebForms が登録していたが、それより前に読む所では読めない)。コンパイラー(Roslyn)は BOM の無い UTF-8 でないファイルを Windows では ANSI コードページで読むが、Linux には無い(CS1056) | FrameworkOnCore.Web の AnsiCodePage: 実行時オプション `FrameworkOnCore.AnsiCodePage`(変換器が書く)、無ければ Windows の GetACP、無ければ LANG のカルチャ。fileEncoding の無いファイルは BOM、UTF-8 として正しければ UTF-8、それ以外は ANSI コードページ(.NET Framework は BOM が無ければ ANSI だけ。UTF-8 の判定は .NET Framework では文字化けしていたページを正しく読む違い)。System.Web の読み込み時にコードページを登録する。変換器: BOM の無い UTF-8 でないソースのあるプロジェクトに `CodePage`(BOM の無い UTF-8 のファイルのほうが多いプロジェクトでは、代わりにそのファイルを UTF-8〈BOM 付き〉にする)、サーバーが読むページがそうならオプション。書き換えるソースは元の文字コードと BOM のまま、書き換えない行はバイトのまま(1.6.5-w2l.25。2026-10-30) | `encoding-default`(アプリの Encoding.Default の分)、ほかは常に |
| XmlConvert.ToString(double・float) | R が最短の桁(0.7000000000000001。.NET Framework は 15 桁で往復しなければ 17 桁の 0.70000000000000007) | 置き換え: FrameworkNumbers.XmlText(platformReplacements。アプリのコードの分。DataSet・XmlSerializer の中は下の「対応できないもの」)(2026-10-19) | `floating-point` |
| BinaryFormatter(WebFormsForCore.Serialization.Formatters: セッション状態、ViewState の [Serializable] のオブジェクト、プロファイル、ResX) | .NET では CultureInfo・Type(RuntimeType)・NameValueCollection・TextInfo・CaseInsensitiveComparer などが [Serializable] でない(SerializationException、NameValueCollection は PlatformNotSupportedException)。.NET Framework が書いたセッションの CultureInfo・NameValueCollection・Type を読めず、書けなかった。型の名前を .NET のアセンブリ名(System.Private.CoreLib、System.Web 10.0.29.0)で書いた | FrameworkSurrogates: .NET Framework と同じ型名・メンバーで書き、.NET Framework が書いたものを読む(CultureInfo は名前と UseUserOverride から作り直す)。型の名前は .NET Framework の名前(mscorlib・System などの facade の転送元、ポートの System.Web は 4.0.0.0)。往復テスト(tests/RuntimeUnitTests/RoundTrip)で見つけた(2026-10-30) | なし(常に) |
| System.Resources.ResourceWriter・ResourceReader(App_GlobalResources・App_LocalResources の .resources) | 文字列とプリミティブ以外(Color、Bitmap、アプリの [Serializable] の型)を書けない(PlatformNotSupportedException)・読めない(NotSupportedException)。そういう .resx のあるフォルダーのコンパイルが失敗した | System.Web の SerializedResources: .NET Framework と同じ形式(型の名前と BinaryFormatter のバイト)で書き(AddResourceData)、プロバイダーと型付きクラスの ResourceManager に SerializedResourceSet を渡してポートの BinaryFormatter で読む(2026-10-30) | なし(常に) |
| BinaryWriter の既定のエンコーディング | .NET は Encoding.UTF8(孤立サロゲートを U+FFFD に置き換える)、.NET Framework は例外を投げる UTF-8(EncoderFallbackException)。ViewState・セッション状態に孤立サロゲートを含む文字列があると .NET Framework は書けず、ポートは黙って値を変えていた | System.Web の ObjectStateFormatter と SessionStateUtility の BinaryWriter に例外を投げる UTF-8 を渡す(2026-10-30) | なし(常に) |
| DeflateStream(などの Stream)の Read | .NET 6 から要求より少なく返すことがある。SessionStateItemCollection.Deserialize が一度の Read で足りると仮定し、`<sessionState compressionEnabled="true">` の SQL Server・StateServer のセッションで大きい項目が「セッション状態の情報が無効」になった | ReadAtLeast で全部読む(2026-10-30) | なし(常に) |
| CultureInfoConverter・VersionConverter(TypeConverter: ViewState、ResX、プロファイルの serializeAs="String") | CultureInfoConverter は GetCultures のインスタンス(変換器が持ち回る。ユーザーの上書き無し)を返す、.NET Framework は `new CultureInfo(name)`。VersionConverter は .NET 7 から(.NET Framework に無く、Version は BinaryFormatter で書かれた。ポートの文字列を .NET Framework が読めない) | System.Web の FrameworkConverters と SettingsPropertyValue: CultureInfo は新しいインスタンス、Version には変換器を使わない(2026-10-30) | なし(常に) |

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
- `ServicePointManager.SecurityProtocol` の既定。.NET Framework は `<httpRuntime targetFramework>` が 4.6 より前(wt の 4.5.2)だと SSL 3.0 / TLS 1.0 で、
  TLS 1.2 だけのサーバー(PayPal)に繋がらない。.NET はシステムの既定(TLS 1.2/1.3)。古い既定に合わせることはしない(既知の差。2026-10-31、wt の PayPal)。
- 例外メッセージの言語。.NET Framework は言語パックで日本語(「Int16 型の値が大きすぎるか…」)、.NET のランタイムは英語のみ(既知の差。System.Web の文言は FrameworkOnCore の訳)。
- Response.End(Thread.Abort が無い)。ポートは中断の前に Flush していて、EndRequest で付くヘッダー(OWIN のサインインの Cookie)が送られなかった。
  .NET Framework と同じく送らずに中断し、中断後のコードの書き込みは捨てる(2026-10-31、wt のログイン)。
- Thread.Abort が無い。ASP.NET は executionTimeout を超えた要求のスレッドを中断する。ポートは時間切れの印を付けて次の段階で
  "Request timed out." にする(2026-10-12。それまでは要求が終わらなくなっていた)。ページのコードは最後まで動く。
- AppDomain のデータ(`.appDomain`、`.appPath`、`.appVPath`、`.appId`)。ポートはロード コンテキストに置いていて、`AppDomain.GetData` で見る
  コード(System.Web.Services の既定値、System.Configuration、アプリのライブラリ)が ASP.NET の外と判断した。AppDomain にも置く(2026-10-12)。
- IIS・http.sys の要求の制限(requestFiltering の本文 30,000,000 バイト・URL 4,096・クエリ 2,048、http.sys のパスの区切り 260)は Kestrel に無いか
  値・状態が違う(Kestrel の本文の上限は例外としてアプリに届く)。ホストが web.config の requestLimits で IIS と同じく拒否する(2026-10-12)。
- Kestrel のデュアルモードのソケットは IPv4 の接続元を `::ffff:a.b.c.d` で渡す。IIS の REMOTE_ADDR は `a.b.c.d`(2026-10-12)。
- ICU に無いカルチャ名(`new CultureInfo("xx-YY")`、Accept-Language の未知の言語で Culture="auto")。.NET Framework(Windows)は「Unknown Locale (xx-YY)」(表示名は OS・アプリケーション プールの言語:「不明なロケール (xx-YY)」)と不変カルチャに近いデータで作る。.NET(ICU)は「xx (YY)」と既定のロケールのデータ。LocaleProbe の auto-unknown-first(Linux)。
- 中立カルチャ(zh-Hans、de)の数値・日付の書式(Linux)。ICU の中立のロケールには通貨などが無い(`¤`)。カルチャ プロファイルに名前を挙げたもの(capture-culture.ps1 -Cultures)は FrameworkOnCore の System.Web が作る時に入る。
- カルチャ プロファイルの名前(EnglishName・NativeName)は capture-culture.ps1 を動かしたユーザーの表示言語のもの。IIS のアプリケーション プールのユーザーと違う言語だと、未知のカルチャの名前が違う。
- System.Data の式の中の double(`DataColumn.Expression` の `Convert(Amount, 'System.String')`、文字列との連結、RowFilter での文字列との比較)、DataSet.WriteXml・GetXml の double の R。.NET の System.Data・System.Xml が自分で書くので、アプリのコードを書き換えても届かない(`0.30000000000000004`。.NET Framework は `0.3`)。DataFileProbe(2026-10-19)。
- Windows のドライブ付きのパスのリテラル(`Path.GetFileName(@"C:\inetpub\...\default.aspx")`)。Linux では区切りとみなされず、ファイル名がパス全体になる。変換器はデータから来るパスと区切りを書いたリテラルを WindowsPath.Native にするが、ドライブ付きのものは Linux に無いのでそのまま。ChangeExtension・Combine の結果の区切りも Linux では `/`。
- Microsoft.Data.SqlClient の .NET 版は `\|DataDirectory\|` を展開しない(.NET Framework 版は展開した)。構成の接続文字列は、FrameworkOnCore の System.Data.SqlClient が展開するので値のまま返す。
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
- 往復テスト(tests/RuntimeUnitTests/RoundTrip、.NET Framework 4.8 の書いたものとの比較。2026-10-30)で見つけた、.NET のライブラリの違いで対応しないもの:
  DataTable の `RemotingFormat = SerializationFormat.Binary` を .NET は拒否する(設定も読み取りも InvalidEnumArgumentException。.NET 5 から。XML 形式は読める)。
  DataSet・DataTable の XML スキーマが列の型を .NET のアセンブリ名で書く(`msdata:DataType="System.Guid, System.Private.CoreLib"`)ので、ポートが書いた
  Guid などの列のある DataTable を .NET Framework は読めない(逆は読める)。XmlSerializer は DateTimeOffset・TimeSpan を書く(.NET Framework は空の要素)。
  .NET の System.Decimal は ISerializable で、object 型のフィールドの decimal を BinaryFormatter がオブジェクトとして書く(.NET Framework はプリミティブ。どちらも読める)。
  List<T> の容量(BinaryFormatter が書く配列の長さ)、DeflateStream の圧縮結果(zlib-ng)も違う。
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
- XmlSerializer の名前空間の宣言の順(`xmlns:xsi` と `xmlns:xsd`)、制御文字(.NET Framework は `&#x1;` で書き、.NET は例外)。XmlConvert・DataSet・XDocument の double の `R`(.NET Framework は 17 桁、.NET は最短。アプリのコードの XmlConvert.ToString は FrameworkNumbers.XmlText に置き換える)。
- `Math.Sin` などの結果の最後の桁(C ランタイムの違い。`Math.Sin(Math.PI)` が 1.22460635382238E-16 と 1.22464679914735E-16)。
- `System.Web.HttpUtility`。.NET の共有フレームワークにもあり、FrameworkOnCore の System.Web は .NET のものへ型転送している(.NET 10 でアプリの `HttpUtility` は System.Web.HttpUtility.dll のもの)。
  `ParseQueryString(...).ToString()` が空のキーの `=` を落とす(`b&=c` が `b&c`)。web.config の `httpRuntime encoderType`(AntiXssEncoder など)をアプリの `HttpUtility.HtmlEncode` が使わない(候補: FrameworkOnCore の HttpUtility を使う。CS0433 を避ける参照の仕方が要る)。
  .NET Framework 4.8 との差分ファジング(tests/RuntimeUnitTests/Fuzz の FuzzTests.The_application_s_HttpUtility_gives_NET_Framework_s_results。2026-10-30)で
  ほかに: `ParseQueryString(...).ToString()` が ASCII 以外を UTF-8 の `%e3%81%82` で書く(.NET Framework は `%u3042`)、先頭の空の組の `&` を書く。
  `UrlPathEncode` が DEL(U+007F)をそのまま残す(.NET Framework は `%7f`)、`//` か `\\` で始まるパスをホストとして扱い符号化しない。
  不正な UTF-8 の U+FFFD の数(`UrlDecode`)。`UrlEncode`・`UrlEncodeUnicode`・`UrlDecode`・`HtmlEncode`・`HtmlAttributeEncode`・`HtmlDecode`・`JavaScriptStringEncode` はほかは同じ。
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
