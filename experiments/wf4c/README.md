# 実験: WebFormsForCore で元の Web Forms をそのまま動かす(2026-09-26)

方針転換(Blazor 化ではなく「.NET Framework アプリを Linux で動かす」)の最初の試作。
サンプルのソースは変更せず、SDK 形式のプロジェクト(`template.csproj.txt`)と `Program.cs`
(`Program.cs.txt`)だけを足して、WebFormsForCore 1.6.4(NuGet)で .NET 10 上に載せる。
比較相手は IIS / .NET Framework で採った正解データ(`samples/*/golden-webforms.json`)。

    .\experiments\wf4c\run-sample.ps1 -Name ProductAdmin
    .\experiments\wf4c\run-sample.ps1 -Name DefaultsProbe -Exclude ProbeEmit.cs

## 結果(Windows 上。Linux(Docker)は未実施)

| サンプル | 結果 |
|---|---|
| ProductAdmin | **13/13 一致** |
| OrderAdmin(UpdatePanel・全バリデータ) | **10/10 一致** |
| MasterProbe(入れ子マスター) | **3/3 一致** |
| DefaultsProbe | **4/4 一致**(フォーク 0001 適用後) |

## 見つかったこと

- **DynamicData が NuGet に無い。** `ItemType` を持つデータバインドコントロールは OnInit で
  `DataBoundControlHelper.EnableDynamicData` を呼び、System.Web.DynamicData を読み込む。上流の
  WebFormsForCore.Web.DynamicData は LINQ to SQL / EF6 に依存して未完成でビルドできない。
  必要なのは EnableDynamicData の一部なので、ランタイム側で直せる見込み(フォークの必要性の具体例)。
- **出力先。** 当初は上流の既定どおり `bin_dotnet` にしていたが、be で `bin` に変えた(下記)。
- **既定のドキュメント。** IIS の「/ → Default.aspx」が Kestrel に無い。`UseDefaultFiles` で補う
  (`Program.cs.txt`)。変換器が生成すべき項目。
- **README の手順どおりでは動かない箇所がある。** `System.Configuration.ConfigurationManager` を
  参照から外す手順は、WebFormsForCore.Configuration(同名の DLL)まで外してしまう。
  `\dotnet\` の正規表現は Windows の MSBuild で評価できない(`[/\x5C]dotnet[/\x5C]` に変更)。
- **.NET Framework → .NET の API 差は変換器の仕事のまま。** DefaultsProbe の `ProbeEmit.cs`
  (Reflection.Emit の削除 API)は元のままではコンパイルできない。旧変換器の `RemovedEmitApis`
  などが流用できる。
- **上流のソースからのビルド**は `src/WebFormsForCore.Build` を先にビルドする必要がある
  (`lib/WebFormsForCore.Build/.../FakeStrongName.targets` を出力する)。

`_upstream/` は上流の浅いクローン(Git 管理外)。

## フォーク(2026-09-26、`_upstream` のローカルブランチ `w2l/dynamicdata`)

上流 WebFormsForCore の main(1.6.4 相当)に対する修正。`patches/` に `git format-patch` の形で置く。
`pack-fork.ps1` で `1.6.5-w2l.1` として `_feed/` にパッケージ化し、テンプレートはそれを参照する。

| パッチ | 内容 | 必要になった場面 |
|---|---|---|
| 0001 | DynamicData を .NET でビルド(EF6 / LINQ to SQL のモデルプロバイダーを除外、ScaffoldTableAttribute) | `ItemType` 付きのデータバインドコントロール(DefaultsProbe、wt) |
| 0002 | `Response.Headers` を IIS7 以外でも使えるように(クラシックの header リストと同期) | OWIN(Microsoft.Owin.Host.SystemWeb)。ASP.NET Identity を使う 4.5 以降のテンプレート |
| 0003 | ページのコンパイルに mscorlib / netstandard のファサードを参照 | .NET Framework 向けライブラリを呼ぶページ(wt の Site.Master → Identity) |
| 0004 | ApplicationServices: System.Web の型を実行時の名前(System.Web)で探す。InternalsVisibleTo に実行時の公開鍵(FakeStrongName が付ける Microsoft の鍵) | be の MembershipUser("Client Profile" 例外)、TypeLoadException |
| 0005 | ページのコンパイルの既定参照に、.NET Framework の既定参照(mscorlib / System / System.Core …)の型が .NET で置かれているアセンブリを追加。`App_Data/machine.config` は内容が違えば書き直す | ページ内の LINQ(be のテーマ、CS1061 Where) |
| 0006 | `Assembly.Load("App_GlobalResources")` / `("App_Code")` を解決(.NET は AssemblyResolve に完全名を渡す) | be のリソースクラス(labels.designer.cs)、拡張機能の列挙 |
| 0007 | `System.Web.IHtmlString` を System.Web.HttpUtility へ型転送 | .NET Framework 版の System.Web.WebPages.Razor(be のウィジェット) |
| 0008 | Linux: 物理パスを大文字小文字を区別せずに解決(MapPath、設定ファイル) | be の設定ファイル `Web.Config`(Linux で全画面エラー) |
| 0009 | ASP.NET Core ホスト: SERVER_NAME を Host ヘッダーから(IIS と同じ) | be の error404 へのリダイレクト先がコンテナの IP アドレスになった |

未対応: VB のページコンパイラー(`VBCompiler.cs`)にも 0003 と同じ対応が要る(VB 対応のときに)。

照合(0005・0007 のときに実施):
- .NET Framework 4.8 の System.Web の公開型のうち、フォークで定義も型転送もされていないのは `IHtmlString`(0007 で対応)と `RegiisUtility`(IIS の登録用、対象外)だけ。
- System.Web.Extensions では LinqDataSource(LINQ to SQL が前提)と ApplicationServicesHostFactory(WCF のホスト)が無い。
- System.Web.RegularExpressions では事前コンパイルされた正規表現クラスが無い。
- System.Drawing では System.Drawing.Design(デザイナー用)が無い。

フォークのビルドは `pack-fork.ps1 -Build All`(`fork.slnx`)で行う。WebFormsForCore.Build は先に一度だけビルドしておく。
fork.slnx に含めると、読み込み済みのタスク DLL とコピーがぶつかって失敗する。

## 互換アセンブリ(`shims/`)

.NET Framework にあって .NET に無いアセンブリを、.NET Framework 向けパッケージが参照している場合に置く。
公開鍵トークンが違っても名前とバージョンで解決される(DynamicData で確認)。

- `System.Net.Http.WebRequest`(`WebRequestHandler`): Katana の Microsoft.Owin.Security.*

## wt(WingtipToys、実在の OSS)

`convert-project.ps1` で旧形式の csproj から SDK 形式を作る(ソースは一切変更しない)。

    .\experiments\wf4c\convert-project.ps1 -Project corpora\work\wingtiptoys-master\WingtipToys\WingtipToys\WingtipToys.csproj -Out experiments\wf4c\wt
    # 実行: experiments\wf4c\wt\WingtipToys で dotnet bin\WingtipToys.dll

結果: **8 画面中 6 一致**。残り 2 は既知の丸めの差(`double` 22.5 の通貨書式が .NET Framework は ¥23、
.NET は ¥22)で、アプリのコード側の差。EF6 6.5.1、ASP.NET Identity 2 + OWIN(.NET Framework 版のまま)、
FriendlyUrls、Elmah、バンドルがそのまま動いている。

変換スクリプトに入れた規則(新しい変換器に持ち込むもの):
- packages.config → PackageReference。.NET に同梱の System.* 4.x 等は外す。WebFormsForCore に同等品が
  あるもの(バンドル、WebGrease、Microsoft.Web.Infrastructure、AjaxControlToolkit)は置き換える。
  EF6 は 6.5.1、Newtonsoft.Json は依存先の要求(13.0.4)まで上げる(下げると NU1605)。
- 元の `<Compile Include>` をそのまま使う。`GenerateAssemblyInfo=false`(元の AssemblyInfo.cs と重複するため)。
- ルーティング(RouteTable / FriendlyUrls)を使うアプリは `HandleAllRequestsWithWebForms()`。
  使わないアプリは IIS の既定ドキュメント(`/` → Default.aspx)を `UseDefaultFiles` で補う。

## be(BlogEngine.NET 3.3.8、実在の OSS)

Web プロジェクトと、それが参照するライブラリ(BlogEngine.Core)を変換する。

    $ex = 'Services\Compilation\Design\CodeExpressionEditor.cs', 'Services\Compilation\Design\QueryStringExpressionEditor.cs',
          'Services\Compilation\Design\ServerVariableExpressionEditor.cs', 'Services\Compilation\Design\SessionExpressionEditor.cs',
          'Services\Compilation\LinqLengthExpressionBuilder.cs', 'Services\FileSystem\FileStoreDb.cs',
          'Providers\FileSystemProviders\DbFileSystemProvider.cs', 'Service References\GalleryServer\Reference.cs'
    .\experiments\wf4c\convert-project.ps1 -Project corpora\work\BlogEngine.NET-3.3.8.0\BlogEngine\BlogEngine.NET\BlogEngine.NET.csproj -Out experiments\wf4c\be -ExcludeFiles $ex
    # 実行: experiments\wf4c\be\BlogEngine.NET で dotnet bin\BlogEngine.NET.dll

結果: **5 画面すべて一致**。
- Web API 2(SimpleInjector)、Web Pages のウィジェット(.NET Framework 版の System.Web.WebPages.Razor)、App_GlobalResources、拡張機能(bin 内の DLL の列挙)が動いている。
- 除外した 8 ファイルは .NET に無い API を使うもの。
  - LINQ to SQL(FileStoreDb と、それを使う DbFileSystemProvider)
  - WCF Data Services のクライアント(ギャラリーのサービス参照)
  - System.Web.UI.Design(式エディター。デザイナー用)
  - LinqLengthExpressionBuilder
- 既定の構成(XML ファイルのストア)では、除外したファイルは使われない。変換器では、これらはスタブにするか、除外して影響箇所を報告する対象。

変換で見つかった規則(be で追加):
- 出力先は `bin`(.NET Framework と同じ)。アプリは自分のアセンブリを `~/bin` から探す(be の拡張機能)。上流の既定は `bin_dotnet`。
- ProjectReference を辿ってライブラリも変換する。HintPath の DLL(packages 外)は `_lib` に置いて参照する。
- web.config の `<compilation><assemblies>` も参照に写す(System.Management → パッケージ)。
- System.ServiceModel.Syndication などは .NET のパッケージに置き換える。BinaryFormatter の警告(SYSLIB0011)は抑止する(動作は .NET 側の設定次第)。

## Linux(Docker、2026-09-26)

`run-linux.ps1` で、変換済みのアプリを Linux コンテナ(mcr.microsoft.com/dotnet/sdk:10.0)の中でビルドして実行し、ホストの ParityTest で正解データと比べる。

    .\experiments\wf4c\run-linux.ps1 -App ProductAdmin
    .\experiments\wf4c\run-linux.ps1 -App be\BlogEngine.NET -Scenario corpora\regression\be.scenario.json -Golden corpora\parity\be.golden-webforms.json
    .\experiments\wf4c\run-linux.ps1 -App wt\WingtipToys -Scenario corpora\regression\wt.scenario.json -Golden corpora\parity\wt.golden-webforms.json -SqlServer

| 対象 | Linux | Windows |
|---|---|---|
| サンプル 4 つ | **30/30** | 30/30 |
| be | **5/5** | 5/5 |
| wt | 5/8 | 6/8 |

wt の残り:
- 丸めの差(¥23 / ¥22)は Windows と同じ。
- error-page は検証環境の差。Docker のポート転送では接続元がローカルにならず(`Request.IsLocal` が偽)、詳細が出ない。コンテナの中から開けば詳細が出ることを確認した。

Linux で見つかって直したこと:
- **ファイル名の大文字小文字**(フォーク 0008): IIS は区別しない。be の設定ファイルは `Web.Config`。
- **SERVER_NAME**(フォーク 0009): 上流はローカルの IP アドレスを返していた。
- **プロジェクトファイルの絶対パス**(convert-project.ps1): 別の場所(コンテナ)でもビルドできるよう相対パスにした。

デプロイ時の設定として与えたもの(変換器が生成・提案する対象):
- **接続文字列**: `.\SQLEXPRESS`・LocalDB・Windows 認証は Linux に無い。`-SqlServer` で SQL Server のコンテナを立て、web.config の接続文字列を書き換えたものを重ねる。アプリのファイルは変えない。
- **カルチャ**: IIS はサーバーの OS のカルチャをアプリに渡す。コンテナには無い(インバリアントで通貨が `¤`)ので、`LANG` で渡す(既定は正解データを採ったこのマシンのカルチャ)。
- **ポート**: ホストとコンテナで同じ番号にそろえる(IIS の SERVER_PORT はローカルのポート)。リバースプロキシの後ろに置く場合は、`aspnet:UseHostHeaderForRequestUrl` でポートも Host ヘッダーから取る。

## カルチャのデータ(2026-09-26)

.NET Framework は Windows のカルチャデータ(NLS)を使う。.NET は ICU のデータを使い、両者は異なる。
- **Linux**: すべてのカルチャが ICU のデータ。
- **Windows の .NET**: ユーザー設定を反映しないで作ったカルチャ(`CultureInfo.GetCultureInfo`)が ICU のデータ。

ja-JP と en-US で見つかった差:
- 通貨記号: `¥` と `￥`
- 既定の小数桁(`N`): 2 と 3
- 長い日付: 曜日の有無
- 月の省略名: `1` と `1月`
- en-US の負の通貨: `($n)` と `-$n`
- en-US の AM/PM の前の空白: U+202F

アプリが `new CultureInfo("ja-JP")` で作ったカルチャも含め、どの作り方でも元のサーバーと同じになるよう、データ源のほうを合わせる。

1. **取得(元のサーバーで)**: `capture-culture.ps1` を Windows PowerShell(.NET Framework)で実行する。既定のカルチャ、web.config の `<globalization>`、指定したカルチャについて、数値と日付の書式を JSON に書き出す。変換時に `-CultureProfile` で渡すと `App_Data/culture-profile.json` に置かれる。
2. **Windows**: `System.Globalization.UseNls=true`(テンプレートに記載)。.NET も Windows のデータを使う。
3. **Linux**: `icu/build-icu-data.sh` がコンテナの中で次を行い、アプリは `ICU_DATA` を付けて起動する(`run-linux.ps1` が実行)。
   - 取得したデータとランタイムの ICU のデータを比べる。
   - 差がある項目だけを、そのカルチャの ICU リソース(ja_JP など)として生成する。ICU のソースはランタイムと同じ版(74.2)から取り、genrb でコンパイルする。
   - 生成後に全項目を比べ直し、残った差を表示する。
   - ICU は `ICU_DATA` の個別ファイルを組み込みのデータより先に探し、項目ごとに親(ja、root)へ継承するので、変える項目だけを置けばよい。

Linux で ICU のデータに表せないもの:
- 日付の代替パターンの一覧(`GetAllDateTimePatterns` の 2 つ目以降、`DateTime.GetDateTimeFormats`)。.NET は ICU から 1〜2 個しか取らない。
- 文字列の並び順(照合)。Windows は `UseNls` で一致する。

結果: Linux の wt で通貨記号が `¥` になった。Windows と Linux とも、サンプル・be・wt の結果は変わらない。

## 全コーパスでの検証(2026-09-27)

`verify-corpora.ps1` で 6 本を変換してビルドした(Windows)。正解データがあるのは be と wt だけ。

| コーパス | 結果 |
|---|---|
| be | ビルド成功。Windows 5/5、Linux 5/5 |
| wt | ビルド成功。Windows 6/8、Linux 5/8(丸めの差 2 件と、Linux の error-page は検証環境の差) |
| mojo | ライブラリ 11 本はすべてコンパイルできた。Web プロジェクトで止まる |
| yaf | ライブラリ(ServiceStack.OrmLite)のコンパイルで止まる |
| dnn | ライブラリ(DotNetNuke.Log4Net)のコンパイルで止まる |
| n2 | ライブラリ(N2)のコンパイルで止まる |

### 変換規則の穴(convert-project.ps1 を直した)

1. **リポジトリ単位のコピー**
   - 対象: dnn、yaf、n2(`..\SolutionInfo.cs` のリンク、`..\..\DNN_Platform.build`)。
   - プロジェクトのフォルダーだけではなく、リポジトリ全体をコピーし、プロジェクトファイルをその場で書き換える。
2. **SDK 形式のプロジェクト**
   - 対象: yaf、dnn。
   - 対象フレームワークは単一の net10.0 にする(netstandard2.0 の側は net10.0 のプロジェクトを参照できない、NU1201)。
   - 条件式の中の `net472` などは net10.0 に付け替える。
   - Reference は、その場で PackageReference に置き換える(条件が保たれる)。
   - XCOPY の配置用 Target は外す。
   - アナライザーとソースジェネレーターは付け替えない(RS1041)。
3. **兄弟プロジェクトのビルド出力を指す HintPath**
   - 対象: dnn(`..\bin\DotNetNuke.dll`)。
   - 出力しているプロジェクトへの ProjectReference に置き換える。
4. **構成ごとの条件の評価(Debug|AnyCPU)**
   - 対象: mojo(構成ごとにデータプロバイダーを選ぶ)、dnn の log4net(DefineConstants)。
   - 参照、コンパイル対象、DefineConstants に反映する。
5. **パッケージのダウングレード(NU1605)**
   - 対象: mojo、dnn。
   - restore が報告する版まで上げる。
6. **.NET Framework では標準で、.NET では別パッケージの API**
   - 対象: mojo の `System.Data.SqlClient`。
   - ソースで使われていれば追加する。同種として OleDb、Odbc、EventLog、PerformanceCounter、ServiceController、Cryptography.Xml、MEF なども表にした。
7. **新しい脆弱性の警告(NU1902)**
   - 対象: dnn(警告をエラーとして扱う設定)。
   - 元がビルドされた後に公開されたものなので、エラーにはしない。
8. **`System.Web.Services.Description` との型の重複(CS0433)**
   - 対象: mojo の `WsiProfiles`。
   - CoreWCF が持ち込むパッケージで、コンパイル参照から外す。

### 残り: .NET に無い API(新しい変換器のソース書き換え・除外・スタブの担当)

止めているファイルを手で除外して先に進めた範囲で見つかったもの:

| 種類 | 例 |
|---|---|
| コードアクセスセキュリティ | yaf `DbProviderFactory.CreatePermission` のオーバーライド |
| Remoting | yaf `CallContext`、dnn `RemotingServices`、`Activator.GetObject` |
| Reflection.Emit の削除 API | n2・yaf `AppDomain.DefineDynamicAssembly`、`AssemblyBuilder.Save`、`RunAndSave` |
| AppDomain | dnn `AppDomainSetup.ConfigurationFile` |
| Windows の偽装 | dnn `WindowsImpersonationContext` |
| シリアル化 | n2 `IDataContractSurrogate` |
| 新しい BCL の型との名前の衝突 | n2 `Range`(`System.Range`)、`CollectionExtensions` |
| WCF のサーバー | mojo `ServiceHost`、`ServiceHostFactory`(CoreWCF の担当) |
| WCF Data Services のクライアント | mojo `DataServiceQuery<>` |
| デザイナー(System.Design) | mojo `DataFieldConverter`(属性で使うだけ) |
| 旧 API | mojo `ICertificatePolicy` |
| .NET 向けのコード分岐 | yaf の ServiceStack(`#if NET6_0_OR_GREATER` の分岐が、元の net481 構成では除かれていたファイルを要求する) |

旧変換器には、このうち Reflection.Emit(`RemovedEmitApis`)と名前の衝突(`CompatImportDisambiguator`)に対応する部品がある。

ParityTest の修正: 拡張子なしのパスを HTTP で事前確認する方式をやめた(GET で Web Forms のページが
実行されるため、AddToCart が 2 回実行されてカートが 2 件になった)。開いて失敗したら .aspx で開き直す。
