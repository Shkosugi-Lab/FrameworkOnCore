# .NET Framework にしかない API の洗い出し(2026-09-29)

材料:
- 8 コーパス(be, wt, mojo, yaf, dnn, n2, imis, nop)の CONVERSION-REPORT.md:「.NET に無い、または Windows 専用の API」397 件と「スタブ・削除」365 件
- ソースの静的検索
- .NET 10 / Linux での実際の挙動(`api-probe/`、`docker run --rm -v <api-probe>:/src:ro mcr.microsoft.com/dotnet/sdk:10.0 bash /src/run.sh`)
- 変換後の動作(`samples/RuntimeProbe` を変換し、`probe-requests.ps1 [-Linux]` で確認する)

## A. .NET に対応するアセンブリがない(今はスタブ)

| API | 使うコーパス | 今の扱い | 案 |
|---|---|---|---|
| System.Web.DataVisualization(グラフ) | nop | **移植済み**(2026-10-01) | referencesource(MIT)から移植した、FrameworkOnCore.Web.DataVisualization(パッチ 0039・0040。Chart コントロール、ChartImg.axd)。描画は B の System.Drawing の移植版(Linux は libgdiplus)。既定の選択は「移植版を使う」。全 API の新旧比較は `tests/DataVisualizationParity`、サイトでの確認は `samples/ChartProbe` |
| System.Data.Linq(LINQ to SQL) | be(DB ファイルシステム)、yaf、n2、System.Web.Mvc の DLL | **移植済み**(2026-09-30) | referencesource(MIT)から移植した、FrameworkOnCore.Data.Linq(パッチ 0032。SQL Server 用)。既定の選択は「移植版を使う」。DLL だけが参照する場合(MVC のモデルバインド)もパッケージが付く |
| System.Data.Services.Client(WCF Data Services) | be(ギャラリー)、mojo | スタブ | 優先度は低い(使う機能が限られる) |
| System.Web.Mobile / MobileControls | nop(11 件)、mojo、dnn、n2 | スタブ | ASP.NET 4.0 で廃止済みなので、スタブのままにする |
| System.ServiceModel のサーバー側(ServiceHost、.svc) | mojo | スタブ | CoreWCF |
| System.Design / System.Web.Extensions.Design / Windows.Forms | be、dnn、mojo、nop、imis | スタブ・除去 | デザイナー用なので、対応不要 |
| System.Data.Entity(EF4) | nop | EF6 へ移動 | 対応済み。referencesource にソースはあるが、EF6 のほうが保守されている |

## B. Linux では例外になる(Windows 専用のパッケージ)

| API | 使うコーパス | Linux での挙動 | 案 |
|---|---|---|---|
| **System.Drawing** | 8 つ中 7 つ | `DllNotFoundException: gdiplus.dll` | **移植済み**(2026-09-30、旧パッチ 0034〜0037):dotnet/runtime 6.0 の System.Drawing.Common(MIT)を FrameworkOnCore.Drawing.Common としてビルドし(Windows は GDI+、Linux は libgdiplus。スイッチ不要)、変換器がそれに付け替え、配置が libgdiplus とフォントを入れる。全 API の新旧比較(`tests/DrawingParity`)で見つけた 6.0 の Unix 実装の不具合(EXIF のバイト順、Restore のクラッシュ、アイコンのサイズ選択など)も直した。Linux に残る差(システムの色・アイコン、印刷、パスの幾何、メタファイル、フォント)は `tests/DrawingParity/known-differences.json`。以下は移植前の記録:System.Drawing.Common 10 は libgdiplus を見つけても Windows 以外を拒む(PlatformNotSupported)。6.0 の Unix 実装 + `System.Drawing.EnableUnixSupport` + libgdiplus なら、試した 12 項目がすべて動く。FrameworkOnCore など 10 に対してビルドしたコードからも、10 をアプリのアセンブリの一覧から外し、`AssemblyLoadContext.Resolving` で 6.0 を渡せば動く。サポート切れの部品(6.0、libgdiplus)を使うかどうかは要判断 |
| EventLog | dnn、imis、yaf | PNSE | 対応済み(Compat:標準エラーへ) |
| WindowsIdentity | dnn、yaf | PNSE | 対応済み(Compat) |
| OleDb(Excel 取り込み) | nop、dnn | PNSE | Linux にプロバイダがない。報告のみ |
| DirectoryServices(AD) | be、mojo | PNSE | System.DirectoryServices.Protocols(LDAP)への書き換え。報告のみ |
| Registry | 実使用なし | `Registry.LocalMachine` が null → NRE | 報告のルールを追加する |
| WMI / PerformanceCounter / ServiceController | 実使用なし | PNSE | 報告済み |
| P/Invoke(kernel32 など) | imis Loader.cs、dnn(log4net、対策済み) | DllNotFound | 個別に対応 |

## C. .NET ではどの OS でも例外になる(ランタイムから外れた機能)

| API | 使うコーパス | 挙動 | 案 |
|---|---|---|---|
| **Thread.ResetAbort** | dnn(AdvancedUrlRewriter、Exceptions、ModuleHost、ProfileModuleUserControlBase、Modulesettings) | PNSE | **対応済み**:`Platform.ResetAbort`(FrameworkOnCore の `HttpResponse.ResetThreadAbort` を呼ぶ)。未対応だとリダイレクトが 503 になり、catch の後が実行されなかった |
| **BinaryFormatter** | n2(ContentDetail:DB に保存するオブジェクト)、be(拡張の設定)、mojo、yaf、dnn | PNSE(.NET 9 で削除) | **対応済み**:互換パッケージ System.Runtime.Serialization.Formatters 10.0.12 と、Web プロジェクトの `EnableUnsafeBinaryFormatterSerialization`。.NET Framework 4.8 が書いたデータを読めることを確認した。**残り**:ソースのない DLL だけが使う場合は検出しない(imis の ReportViewer など) |
| **Encoding.GetEncoding(コードページ)** | nop(Alipay)、dnn。日本語アプリでは Shift_JIS | `NotSupportedException` | **対応済み**:FrameworkOnCore の `UseWebForms` が CodePagesEncodingProvider を登録する(web.config の `responseEncoding="shift_jis"` も動く。テンプレートに入れた登録は重複だったので外した) |
| **Encoding.Default** | be、mojo、n2、nop、imis(コメント)、dnn(log4net) | .NET Framework は ANSI コードページ(日本語 Windows では Shift_JIS)、.NET は UTF-8 | **対応済み**:`Platform.DefaultEncoding`(Windows は GetACP、Linux は LANG のカルチャの ANSI コードページ)。**残り**:`Encoding.GetEncoding(0)`、VB の `FileOpen` 系は未対応(コーパスでは未使用) |
| AppDomain.CreateDomain | dnn(Telerik の保守ツール) | PNSE | 報告のみ |
| Delegate.BeginInvoke | dnn 1 件 | PNSE | 対応済み(FOC1005:`AsyncDelegate`) |
| StrongNameKeyPair | n2 1 件 | PNSE | 個別に確認 |
| 引数なしの HashAlgorithm/HMAC/SymmetricAlgorithm.Create() | dnn | PNSE | 対応済み(既定のアルゴリズムに置き換え) |
| RijndaelManaged BlockSize=256 | なし | PNSE | 報告のルールだけ |
| CodeDom でのコンパイル(CompileAssemblyFromSource) | なし | PNSE | 報告のルールだけ |

## D. パッケージの版上げで変わった API(Framework ではないが、同じ仕組みで対応できる)

- dnn:System.IdentityModel.Tokens.Jwt 4 → 8 で `JwtSecurityToken` の名前空間が変わり、JWT 認証がスタブになっている → namespaceMoves に追加する
- nop:AjaxControlToolkit 4.1 の `ToolkitScriptManager` → マークアップの書き換え

## E. 警告だけで、実際には動くもの(対応不要)

- SYSLIB0014 WebRequest/WebClient(87 件)
- SYSLIB0021/22/23 の旧暗号クラス
- SYSLIB0013 EscapeUriString
- SYSLIB0050/0051 シリアル化のコンストラクター
- Assembly.CodeBase/GlobalAssemblyCache
- SYSLIB0003 CAS:`Demand`/`Assert` は何もしない。部分信頼は効かなくなるが、Web アプリは完全信頼で動くのが普通
