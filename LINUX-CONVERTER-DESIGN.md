# 新しい変換器の設計: .NET Framework の Web アプリを Linux で動かす(2026-09-26)

方針転換(Blazor 化 → 「.NET Framework アプリを Windows Server でなく Linux で動かす」)後の変換器の設計。
根拠は `experiments/wf4c/`(WebFormsForCore のフォークで、サンプル 4 つ・wt・be をソースを変えずに動かした実験)。

## 1. 目的と範囲

- 対象: 言語は C# と VB、フレームワークは Web Forms、MVC、Web API、WCF。
- 優先順位: Web Forms(C#)→ Web Forms(VB)→ MVC → Web API → WCF。
- 画面の操作感は元のまま(ポストバック)。スティッキーセッションや WebSocket は要件にしない。
- 合格の基準: IIS / .NET Framework で採った正解データ(ParityTest の golden)との一致。まず Windows 上の .NET 10、その後 Linux(Docker)で確認する。

## 2. 基本方針: 差をどこで吸収するか

差が見つかったら、次の順で吸収する場所を決める。上ほど 1 回の修正で多くのアプリに効き、利用者のソースに手を入れずに済む。

| 差の種類 | 吸収する場所 | 実験での例 |
|---|---|---|
| System.Web の振る舞いの差(ポートの不具合・未実装、IIS との差) | ランタイム(WebFormsForCore のフォーク) | Response.Headers(0002)、既定参照(0005)、App_GlobalResources(0006)、IHtmlString(0007) |
| .NET で型ごと無くなったもの(アセンブリ・型) | 互換アセンブリ(`shims/`)。同じ名前で実装し、変換器は参照を足すだけ。ソースの無い .NET Framework 向け DLL にも効く | System.Net.Http.WebRequest。候補: Remoting の `CallContext` など |
| プロジェクト・パッケージ・ホストの差 | 変換器(生成物) | SDK 形式、パッケージの置き換え、`bin` 出力、既定のドキュメント |
| .NET で既存の型のメンバーが無くなったもの | 互換アセンブリ内の C# 14 の拡張メンバー(メソッド、プロパティ、静的メンバー)+ 変換器が `global using` を足す(呼び出し箇所は書き換えない) | `AppDomain.DefineDynamicAssembly`(DefaultsProbe、n2) |
| 拡張で補えないもの(override、コンストラクター、定数、存在するが実行時に例外になるメンバーなど) | 変換器(Roslyn の意味解析によるルールベースの書き換え) | `DbProviderFactory.CreatePermission` の override(yaf)、`AssemblyBuilderAccess.RunAndSave` |
| .NET に代わりが無い(LINQ to SQL、WCF Data Services のクライアント、デザイナー) | 変換器が除外またはスタブ化し、報告する | be の 8 ファイル |
| アプリの値の差(浮動小数の書式など) | 報告のみ | wt の ¥23 / ¥22 |

変換器は利用者のコードを書き換える量をできるだけ少なくする。書き換える場合も、規則として説明できる機械的な変換に限る。

## 3. 全体の流れ

```
入力(.sln / .csproj / .vbproj)
  1. 発見      : プロジェクトの依存関係、言語、種類(Web Forms / MVC / Web API / WCF / ライブラリ)
  2. プロジェクト: SDK 形式の csproj / vbproj、パッケージ、参照、除外
  3. ホスト    : Program.cs、web.config の system.webServer を ASP.NET Core の設定へ
  4. ソース    : Roslyn による機械的な書き換え(C# / VB)
  5. 除外とスタブ: .NET に代わりが無いファイルを除外し、依存先がコンパイルできるようスタブを生成
  6. ビルド検証: dotnet build → 診断を分類 → 既知の対処を適用して再ビルド(回数に上限)
  7. Linux 検査: Windows 依存の検出(報告と一部の書き換え)
  8. 出力      : 変換済みソース、Dockerfile、CONVERSION-REPORT.md
検証(別ツール): ParityTest で正解データと比較(Windows → Docker)
```

### 3.1 発見

- 旧変換器の `ProjectReferenceGraph`(条件式・DefineConstants の評価、HintPath 参照)と `WebFormsProject.ReadCompiledFiles` を流用する。
- VB への対応: `.vbproj` を同じ仕組みで読む。旧変換器は VB を「移植不可」と報告するだけだったので、この部分は拡張する。
- 種類の判定: ProjectTypeGuids、パッケージ(Microsoft.AspNet.Mvc / WebApi / FriendlyUrls)、ファイル(.aspx / .svc / Global.asax の内容)から判定する。

### 3.2 プロジェクト(実験で確定した規則)

- SDK 形式(Web プロジェクトは Microsoft.NET.Sdk.Web)、net10.0。元の `<Compile Include>` をそのまま使う(`EnableDefaultCompileItems=false`)。EmbeddedResource も同様。
- `GenerateAssemblyInfo=false`(元の AssemblyInfo と重複するため)。
- 出力先は `bin`。アプリは `~/bin` から自分のアセンブリを探す(be の拡張機能)。
- packages.config → PackageReference。規則はコードでなくデータファイル(`src/FrameworkOnCore.Converter/rules/packages.json`)に置く。
  - 外すもの: .NET に同梱の System.* 4.x、NETStandard.Library など。
  - 置き換えるもの: バンドル / WebGrease / Microsoft.Web.Infrastructure / AjaxControlToolkit → WebFormsForCore.*。EF6 → 6.5.1。
  - 上げるもの: 依存先が要求する版まで(Newtonsoft.Json 13.0.4。下げると NU1605)。
  - 残すもの: .NET Framework 向けのまま動くもの(Identity 2、OWIN / Katana、Web API 2、SimpleInjector、Elmah、FriendlyUrls)。NU1701 は抑止する。
- Framework の参照(`<Reference Include="System.Web">` など)→ WebFormsForCore のパッケージ、または .NET のパッケージ(System.Drawing.Common、System.ServiceModel.*、System.Management、System.Runtime.Caching、System.DirectoryServices)。代わりが無いもの(System.Data.Linq、System.Data.Services.Client、System.Design、System.Web.Mobile)は報告する。
- web.config の `<compilation><assemblies>` も参照に写す。
- HintPath の DLL(packages の外)は `_lib` に写して参照する。
- ProjectReference を辿り、依存するライブラリも変換する。

### 3.3 ホスト

- Program.cs: `UseSession` + `UseWebForms(o => o.UseAspNetCoreSessionProvider())`。
  - ルーティング(RouteTable、FriendlyUrls、MVC、Web API)を使う場合は `HandleAllRequestsWithWebForms()`。
  - 使わない場合は、IIS の既定のドキュメントを `UseDefaultFiles` で補う。
- web.config の `system.webServer` は IIS のホスト機能なので、ASP.NET Core の設定に写す(未実装。今後の作業):
  - defaultDocument → UseDefaultFiles
  - rewrite → Rewrite ミドルウェア
  - httpErrors → StatusCodePages
  - staticContent の MIME
  - requestFiltering の上限 → Kestrel の制限
  - httpProtocol の customHeaders

  写せないもの(handlers / modules の IIS 固有のもの)は報告する。

### 3.4 ソースの書き換え

- 基盤: Roslyn(C# は Microsoft.CodeAnalysis.CSharp、VB は Microsoft.CodeAnalysis.VisualBasic)。構文だけで判断できない規則には、旧変換器の `SemanticBaseIndex` の方式(メタデータ参照でコンパイルを作る)を使う。
- 規則は「.NET で削除・変更された BCL の API → .NET の API」に限る。System.Web は触らない(ランタイムが同じ API を持つため)。
  - 流用するもの: `RemovedEmitApis`、`FrameworkPolyfills`(CS0433)、`FieldKeywordRewriter`(C# 14 の `field`)、Remoting の using の削除。
  - 追加が必要なもの: AppDomain の作成、Thread.Abort、CAS / SecurityPermission 属性、BinaryFormatter の設定(警告ではなく実行時の例外になる箇所)、`WebRequest` 系の古い API。
- 旧変換器で System.Web の型を互換名前空間に付け替えていた処理(`RewriteQualifiedFrameworkTypes` など)は使わない。

### 3.5 除外とスタブ

- 旧変換器の除外判定(`FindUnportableNamespace`、`PortabilityRules`)とスタブ生成(`GenerateExcludedTypeStubs`、`RenderStubType`)を流用する。これが旧変換器で最も価値のある部分。
- 除外の単位はファイル。除外したファイルの公開型は、依存先がコンパイルできるようにスタブを生成する。スタブのメンバーは NotSupportedException を投げる。
- be の実験では手で `-ExcludeFiles` を渡した。新しい変換器では次のように自動で判定する。
  - 代わりが無い名前空間を使うファイルを除外する(System.Data.Linq、System.Data.Services.Client、System.Web.UI.Design)。
  - 除外したファイルに依存するファイルも除外するか、スタブで解決する(be の DbFileSystemProvider)。
- 除外とスタブはすべて報告する。その機能が既定の構成で使われるかどうかも、可能なら報告する(be では XML ストアのため未使用)。

### 3.6 ビルド検証

- 旧変換器の `BuildVerifier`(dotnet build の診断の解析と分類、BUILD-REPORT.md)を流用する。
- 新しく足すもの: 既知の診断に対処を割り当てて再ビルドするループ。回数には上限を設ける。

  | 診断 | 対処 |
  |---|---|
  | NU1605 | 版を上げる |
  | CS0579(AssemblyInfo の重複) | GenerateAssemblyInfo=false |
  | 型が無い | 除外とスタブ |

  未知の診断は報告に残す。

### 3.7 Linux 検査

Windows でしか動かないものを検出する。書き換えられるものは書き換え、残りは報告する。

- パスの大文字小文字: 変換器ではなくランタイムが扱う(フォーク 0008)。
  - IIS と同じく、仮想パスから物理パスへの変換と設定ファイルの読み込みで、大文字小文字を区別せずに実在の名前を探す。
  - 外部から来る URL にも効く。
  - 変換器が扱うのは、アプリが物理パスを自分で組み立てる箇所(`Path.Combine(AppDomainAppPath, "app_data")` など)と、`\` の区切りの報告。
- カルチャのデータ: 元のサーバーで `capture-culture.ps1` を実行して取得し、変換時に `App_Data/culture-profile.json` に置く。
  - Windows: `UseNls` で .NET も Windows のデータを使う。
  - Linux: イメージのビルド時に、差がある項目の ICU データを生成し、`ICU_DATA` で指す(`experiments/wf4c/icu`)。
  - どちらも、アプリが `new CultureInfo(...)` で作るカルチャを含め、すべての作り方に効く。
  - ICU で表せないもの(日付の代替パターンの一覧、並び順)は報告する。
- System.Drawing: Linux では System.Drawing.Common が使えない。使っている箇所を報告する。代わりのライブラリは選択式にする(未決定)。
- レジストリ、EventLog、パフォーマンスカウンター、WMI(System.Management)、Windows 認証、COM: 報告する。
- 接続文字列: LocalDB(`(LocalDB)\...`)と `AttachDbFilename` は Linux に無い。SQL Server コンテナへの置き換えを提案し、設定で差し替えられるようにする。

### 3.8 出力と報告

- 変換済みのソースツリー、Dockerfile(mcr.microsoft.com/dotnet/aspnet:10.0)、CONVERSION-REPORT.md を出力する。
- 報告の形式は旧変換器の `ConversionReport`(Residual の種類と処置: Convertible / Backlog / NeedsInput / OutOfScope / Informational)を流用し、種類の名前を付け直す。

## 4. ランタイム

| 対象 | ランタイム | 状態 |
|---|---|---|
| Web Forms | WebFormsForCore のフォーク(`experiments/wf4c/patches` 0001–0007)と互換アセンブリ(`shims/`) | Windows で wt・be・サンプルが動作 |
| Web API 2 | .NET Framework 版の DLL のまま、ポートした System.Web の上で動かす | be でビルドと起動を確認。API の動作は未検証 |
| MVC 5 | まず .NET Framework 版の DLL のまま試す。動かなければ AspNetWebStack(Apache 2.0)をポートする | 未着手。be の Web Pages(Razor)が DLL のまま動いたので見込みはある |
| WCF | CoreWCF(MIT)に載せる。.svc は ServiceHost の登録に、system.serviceModel はコードに変換する。対応しないもの(WSDualHttp、メッセージセキュリティ、トランザクション)は報告する | 未着手 |

- フォークの配布: パッケージ(1.6.5-w2l.x)を GitHub Release(`fork-<版>`、`experiments/wf4c/publish-fork.ps1`)に置き、変換器・解析・Studio が `_feed` に無ければ取得する(`RuntimeSetup`)。上流への還元(PR)を並行して検討する。0005〜0007 は上流の不具合そのものなので還元しやすい。
- ライセンス: WebFormsForCore と referencesource は MIT、AspNetWebStack は Apache 2.0、CoreWCF は MIT。

## 5. VB への対応

- ページ: VB のページコンパイラー(`VBCompiler.cs`)に 0003 と同じファサード参照を追加する。
- プロジェクト: vbproj の変換。以下の設定を引き継ぐ。
  - `OptionStrict` / `OptionExplicit` / `OptionInfer`
  - プロジェクト全体の Imports(`<Import Include>`)
  - `MyType`
- My 名前空間: Microsoft.VisualBasic の .NET 版で使えないもの(My.Computer の一部など)は報告する。
- 書き換え: C# と同じ規則を VB の構文木で実装する。規則は言語に依存しない形(対象の API とその書き換え先)で定義し、言語ごとに適用部だけを分ける。

## 6. 実装の構成

- 変換器は `src/FrameworkOnCore.Converter`(当初の案の仮称は NetFx2Linux.Converter)。旧方針(Blazor 化)の変換器 `src/WebForm2Blazor.Converter` は 2026-09-30 にリポジトリから消した(git の履歴にある)。
- 規則はデータで持つ: `src/FrameworkOnCore.Converter/rules/packages.json`(パッケージ、参照、ソースの書き換え)と、解析のカタログ `src/FrameworkOnCore.Analysis/catalog/components.json`(部品と選択肢)。規則を足すことが主な保守作業になるため。
- CLI: `convert --project <csproj|vbproj|sln> --out <dir> [--exclude <file>...] [--report <md>]`、`verify-build <dir>`。
- `experiments/wf4c/convert-project.ps1` は規則の試作。新しい変換器はこれと同じ結果を出すことから始める(be・wt で同じ csproj になることを確認する)。

## 7. 検証

- コーパスは be と wt だけ(CLAUDE.md)。加えてリポジトリ内のサンプル 4 つ(ProductAdmin、OrderAdmin、MasterProbe、DefaultsProbe)。
- 変換器やフォークを直したら、類似の問題がほかの場所で起きないか調べる(CLAUDE.md)。実験では次のように機械的に照合した。
  - 0005: .NET Framework のファサードの型転送先を列挙した。
  - 0007: .NET Framework 4.8 の参照アセンブリとフォークの公開型を比較した。
- Linux: Docker(mcr.microsoft.com/dotnet/sdk:10.0)でビルドと実行を行い、ホストの ParityTest から比較する。

## 8. 既知の差と未解決の点

- 浮動小数の書式: `double` の 22.5 を通貨書式にすると .NET Framework は ¥23、.NET は ¥22(wt)。ランタイムでは直せないので、該当する書式呼び出しを報告する。
- AssemblyResolve に渡る名前(.NET は完全名): 0006 で BuildManager を直した。アプリ自身の AssemblyResolve ハンドラーにも同じ差がありうるので、検出して報告する規則を入れる。
- BinaryFormatter: .NET 9 以降は既定で例外になる。be はビルドの警告を抑止しただけで、実行時の使われ方は未確認。
- フォークのビルド: 変更後の最初のビルドで Web.Extensions が CS7069 になることがあり、2 回目で通る(pack-fork.ps1 で 1 回だけ再試行)。原因は未調査。
- **互換アセンブリの網羅(残課題、2026-09-27 決定)**: .NET で型ごと無くなったものは、基本的にすべて互換アセンブリで用意する。進め方:
  1. 一覧を機械的に作る。.NET Framework 4.8 の参照アセンブリの公開型から、次のものを除く。
     - .NET 10 の標準ライブラリにあるもの(型転送を含む)
     - 公式パッケージ(Windows Compatibility Pack、System.Data.SqlClient など)にあるもの
     - 移植済みのもの(WebFormsForCore)
  2. 残った型を分類する。
     - 再現できるか: 同じ動作 / 型だけ(呼ばれたら PlatformNotSupportedException)/ 用意しない
     - 使われているか: コーパス、よく使われる NuGet の DLL
  3. 使われていて再現できるものから実装する。変換器は、ソースがその名前空間・型を使っていれば参照を足す。

  一覧にあるが未実装のものは、変換時に報告する。
- **メンバー単位の一覧(同じ残課題)**: 両方にある型について、.NET Framework 4.8 にあって .NET 10 に無いメンバーと、.NET 10 にあるが常に `PlatformNotSupportedException` を投げるメンバーを列挙する。それぞれを次のどれで扱うかに分類する。
  - 互換アセンブリの拡張メンバー(呼び出し箇所は変えずに `global using` を足す)
  - ルールベースの書き換え(override、コンストラクター、定数が必要な場所、存在するが例外になるメンバー)
  - 報告のみ(リフレクションや `dynamic` 経由の呼び出し、ソースの無い DLL からの呼び出し)

  ソースの無い DLL が .NET に無いメンバーを呼ぶ場合(型はある: `MissingMethodException`)は、変換器がその呼び出しを互換アセンブリの拡張メンバーに置き換える(`AssemblyRetargeter.ReplaceMembers`、2026-09-30)。C# 14 の拡張メンバーは静的メソッド(インスタンスのメンバーは受け手が最初の引数)にコンパイルされるので、`callvirt T::M(args)` を `call Members::M(T, args)` に、静的なものは静的なものに変える(スタックの並びは同じ)。拡張される型は、拡張ブロックのマーカー型の `<Extension>$(receiver)` から読む。基底の型の拡張メンバーでもよい。置き換えられないもの(コンストラクター、ジェネリックなメンバー、`constrained.` 付きの呼び出し、デリゲートの作成)と、互換アセンブリにも無いものはレポートに出す。ソースと DLL の両方に効くので、.NET に無いメンバーは互換アセンブリに拡張メンバーとして足せばよい。
- **ソースの無い DLL の型参照の付け替え(`AssemblyRetargeter`、2026-09-30)**: DLL は型を「アセンブリ+型名」で参照する。.NET Framework で mscorlib・System.Security などにあった型が、.NET では別のアセンブリにあるか、フォーク・互換アセンブリだけが持つ場合(`[mscorlib]CallContext` → フォークの System.Web)、その参照は実行時に解決しない(`TypeLoadException`)。変換器はビルドの後、bin のソースの無い DLL(パッケージ・リポジトリ・配置済みサイトのもの)の型参照を実行時と同じ手順(アセンブリ名 → 定義 → 型の転送)で解決し、解決しないものをその型を public で持つアセンブリに付け替える(優先: フォーク・shim・互換アセンブリ → アプリ → .NET)。どのアセンブリへの参照でも同じ。アプリにある版より新しい版への参照は、その版に下げる(.NET Framework の bindingRedirect に当たり、.NET は web.config の bindingRedirect を読まない)。書き換えた DLL は出力の `foc-retargeted` に置き、各プロジェクトのターゲット `FocUseRetargetedAssemblies` がコピーの前に元の DLL と差し替える。どこにも無い型への参照はレポートに出す。

## 9. 次の作業

1. (済)Linux(Docker)で wt・be・サンプルを動かした。全コーパスで検証した(experiments/wf4c/README.md)。
2. (済、2026-09-27)新しい変換器 FrameworkOnCore(`src/FrameworkOnCore.Converter`)の骨格を作った。
   - プロジェクト変換とビルドエラーの自動処理を実装した。
   - 6 本のコーパスすべてでビルドが通る。
   - 実行時の課題は experiments/wf4c/README.md に記録した: mojo の web.config の assemblies、yaf の Web API 2(AspNetWebStack の移植が要る)、dnn のプロバイダーの配置と DB、n2 の管理画面の配置。
   - (済)アプリの構成は、元のビルドが配置したサイトから取る(`--site`)。元のビルドは、リポジトリのビルドスクリプトをそのまま動かす。Cake(Frosting、スクリプト)は変換器が汎用に実行する(`--build-original`)。dnn はこれでインストールウィザードまで表示できた。
   - (済)アプリの再起動(web.config の変更など)は、プロセスを終了コード 75 で終え、スーパーバイザーが起動し直す(フォーク 0016)。生成する Dockerfile には再起動の方針(`--restart`)を含める。
   - (済)アプリは作業プロセスの中で bin のコピーから動き、再起動では新しいコピーから起動し直す(フォーク 0018。ASP.NET のシャドウコピーと w3wp に当たる)。
   - (済)ビルドスクリプトの無いリポジトリは、ソリューションを Visual Studio と同じ方法でビルドする(変換器に移した)。dnn は Windows でインストールが完了し、トップページが表示される。
   - (済)配置: 変換器がコンテナ(Dockerfile)と Linux のマシン(systemd の install.sh)の両方を出力する(`--deploy`)。設定は環境変数(Azure App Service と同じ名前、フォーク 0022)。mojo はどちらの方法でも動いた。
   - 次: Windows のパスを前提にしたコード(バックスラッシュの区切り)の書き換え。dnn は Linux で起動するが、パスが `\app\web.config` になる。Roslyn の意味モデルで、ファイルシステムの API に流れる文字列と、区切りを置き換える式(`Replace("/", "\\")`)を特定して書き換える。正規表現やエスケープの文字列には触れない。
3. VB 対応(VBCompiler のファサード、vbproj)。
4. MVC 5(DLL のまま動くかの確認から)、Web API の動作確認、WCF(CoreWCF)。
