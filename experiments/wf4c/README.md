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

フォークは、リポジトリのトップレベルの `FrameworkOnCore.Runtime/` にある(2026-10-01 から。下の「フォークをリポジトリに取り込む」)。それまでは `_upstream/`(上流の浅いクローン、Git 管理外)に `patches/` を当てていた。以下の記録の「フォーク」は今の `FrameworkOnCore.Runtime/`(FrameworkOnCore で保守している WebFormsForCore)、「パッチ」の番号はその時の番号(今は `FrameworkOnCore.Runtime/` の同じ件名のコミット)。2026-10-01 に、取り込んだフォルダーを `WebFormsForCore/` から `FrameworkOnCore.Runtime/` に、パッケージ名を `WebFormsForCore.*` から `FrameworkOnCore.*` に改めた(記録の中のパッケージ名は当時のもの。中のプロジェクトとアセンブリの名前は変えていない)。スクリプトなどの名前は 2026-10-01 に「fork」から「frameworkoncore」に改めた(`pack-frameworkoncore.ps1`、`publish-frameworkoncore.ps1`、`frameworkoncore.slnx`、`frameworkOnCoreVersion`、Release のタグ `frameworkoncore-<版>`)。

## フォーク(2026-09-26、`_upstream` のローカルブランチ `w2l/dynamicdata`)

上流 WebFormsForCore の main(1.6.4 相当)に対する修正。当時は `patches/` に `git format-patch` の形で置いていた。
`pack-frameworkoncore.ps1` で `1.6.5-w2l.3` として `_feed/` にパッケージ化し、テンプレートはそれを参照する。

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
| 0010 | ASP.NET Core ホスト: .NET Framework にあったコードページを登録(CodePagesEncodingProvider) | mojo の web.config `fileEncoding="iso-8859-15"`(構成エラー) |
| 0011 | 構成: `<system.codedom>` の Roslyn プロバイダー(DotNetCompilerPlatform の C# / VB)を WebFormsForCore のプロバイダーに対応付ける | mojo の構成エラー(Unable to locate type)。Visual Studio 2015 以降の Web テンプレートは全部この指定を持つ |
| 0012 | 型の解決: 無いアセンブリは例外ではなく null(`Type.GetType` はアセンブリリゾルバーの例外を `throwOnError` に関係なく通す。47 箇所) | dnn の `BuildManager.GetType(type, false)` が FileNotFoundException(web.config にあり後でインストールされるプロバイダー) |
| 0013 | 接続文字列の `\|DataDirectory\|` を展開する(構成から読む接続文字列。プロセスの DataDirectory も設定) | dnn の `AttachDBFilename=\|DataDirectory\|Database.mdf`(.NET の System.Data.SqlClient は拒否する) |
| 0014 | `AppDomain.BaseDirectory` をアプリのルートにする(.NET Framework と同じ。bin のパスは先に読んで保持) | dnn のインストールウィザードが `Install\DotNetNuke.install.config` を bin の下に探した |
| 0015 | ファイル変更通知: 監視フォルダーからの相対名で渡す(フルパスでは監視対象と一致せず、変更が一度も届いていなかった) | dnn のウィザードが web.config を書き換えても反映されず、同じページへのリダイレクトが続いた |
| 0016 | アプリの再起動: プロセスを終了コード 75 で終え、スーパーバイザーが起動し直す(既定の AssemblyLoadContext は解放できない)。再起動が始まった後の要求には 503 と Retry-After | 同上。web.config の変更、bin の変更、`HttpRuntime.UnloadAppDomain` |
| 0017 | 応答バッファ: プールから借りたものだけをプールに返す | dnn の `Install.aspx`(ページの途中の `Response.Flush`)が「バッファがこのプールのものではない」で失敗 |
| 0018 | 作業プロセス: アプリを bin のコピーから子プロセスで動かし、再起動(終了コード 75)で新しいコピーから起動し直す(ASP.NET のシャドウコピーと w3wp に当たる)。`HttpRuntime.BinDirectory` はアプリの bin、読み込みはコピーから | dnn のインストーラーが bin にモジュールの DLL を置けない(使用中) |
| 0019 | SQL Server の接続: 接続文字列に `Encrypt` が無ければ .NET Framework と同じ false(ランタイムの SQL 部品は Microsoft.Data.SqlClient で、既定が true) | dnn のインストールで管理者を作れない(SQL Server Express の証明書を信頼できない) |
| 0020 | 統合パイプラインの構成: web.config の `system.webServer` のマネージドのモジュールとハンドラーを、クラシックのパイプラインでも IIS と同じ規則で読む | dnn の URL 書き換えモジュールが動かず、どのページも PortalSettings が null |
| 0021 | 構成: `configSource` と appSettings の `file` の `\` を、Linux でもディレクトリの区切りとして扱う(.NET Framework では `configSource` に `/` は書けない) | n2 の `configSource="App_Data\n2_host.config"`(Linux で構成エラー) |
| 0022 | 構成: 配置先の設定を環境変数から(Azure App Service が .NET Framework のアプリに渡すのと同じ名前)。`APPSETTING_<キー>` は appSettings、`SQLCONNSTR_<名前>` などは connectionStrings を置き換える | コンテナや systemd で、web.config を書き換えずに DB などを渡す |
| 0023 | ASP.NET Core ホスト: SERVER_PORT を Host ヘッダーのポート(無ければスキームの既定)から、HTTPS をリクエストのスキームから(`IsSecure` が常に false だった) | コンテナのポートを別の番号で公開すると、リダイレクト先がコンテナ内のポートになった。HTTPS を終端するプロキシの後ろで http の URL になる |
| 0024 | マシンキー: 自動生成の検証・暗号化キーを、再起動の後も同じものにする(.NET Framework はレジストリに保存する。ここでは `LocalApplicationData/FrameworkOnCore.Runtime/AutogenKeys`、Unix ではモード 600) | dnn のインストール後の再起動で、ViewState の MAC の検証に失敗した |
| 0025 | VB のページコンパイラー: C# と同じく、ランタイムのライブラリを使う(`/nostdlib` と `/sdkpath` にランタイムのフォルダー、VB のランタイムは `Microsoft.VisualBasic.Core`、フレームワークのファサードも参照)。0003 の VB 版 | n2 の VB のページが BC2017(`Microsoft.VisualBasic.dll` が見つからない)でコンパイルできなかった |
| 0026 | ファイル変更通知(Linux): ファイルの監視をファイル名で引けるようにする(Linux ではフルパスを名前にしていて、変更の通知がどの監視とも一致しなかった)。bin・App_Code などの特別なフォルダーは全 OS で監視する(ツリー全体の名前変更の監視は Windows だけ。inotify ではフォルダーごとに 1 つ要る) | Linux で web.config を変えてもアプリが再起動せず、DNN のインストーラーが自分へのリダイレクトを繰り返した |
| 0027 | 構成(Linux): フォルダーの web.config を、小文字の構成パス(`machine/webroot/1/n2`)からも見つける。`UserMapPath` が物理パスを大文字小文字の違う実在のフォルダー(`N2`)に解決する(0008 の `PhysicalPathCasing`) | Linux で大文字を含むフォルダー(n2 の `N2`、wt の `Admin`、DNN の `Portals` など)の web.config が読まれず、その承認の規則が効いていなかった。n2 の管理画面(`/N2/`)に、ログインせずに入れた |
| 0028 | パスの大文字小文字: `WEBFORMSFORCORE_PATH_CASING=0` で、フォーク自身の大文字小文字の照合(`PhysicalPathCasing`)をしない | プロセスのファイル操作が大文字小文字を区別しないとき(`casefs/libfoccase.so`、区別しないファイルシステム)、同じ照合を 2 回しない。配置の `start.sh` が、ライブラリを読み込んだときに設定する |
| 0029 | `Server.MachineName`(Linux): kernel32 の `GetComputerName` ではなく `Environment.MachineName` | nopCommerce 1.90 のインストーラーのページが Linux で DllNotFoundException |

照合(0005・0007 のときに実施):
- .NET Framework 4.8 の System.Web の公開型のうち、フォークで定義も型転送もされていないのは `IHtmlString`(0007 で対応)と `RegiisUtility`(IIS の登録用、対象外)だけ。
- System.Web.Extensions では LinqDataSource(LINQ to SQL が前提)と ApplicationServicesHostFactory(WCF のホスト)が無い。
- System.Web.RegularExpressions では事前コンパイルされた正規表現クラスが無い。
- System.Drawing では System.Drawing.Design(デザイナー用)が無い。

フォークのビルドは `pack-frameworkoncore.ps1 -Build All`(`frameworkoncore.slnx`)で行う。WebFormsForCore.Build は先に一度だけビルドしておく。
frameworkoncore.slnx に含めると、読み込み済みのタスク DLL とコピーがぶつかって失敗する。

## 互換アセンブリ(`shims/`)

.NET Framework にあって .NET に無いアセンブリを、.NET Framework 向けパッケージが参照している場合に置く。
公開鍵トークンが違っても名前とバージョンで解決される(DynamicData で確認)。

- `System.Net.Http.WebRequest`(`WebRequestHandler`): Katana の Microsoft.Owin.Security.*
- `System.Web.Routing`、`System.Web.Abstractions`: .NET Framework 4 では System.Web への型転送だけのファサード。web.config や .NET Framework 向けパッケージがこの名前を指す(n2 の構成エラー)。`Forwards.cs` は .NET Framework 4.8 の参照アセンブリの型転送から生成した。
- `FrameworkOnCore.Compat`: .NET で削除された型とメンバーを、元の名前空間に置く(メンバーは C# 14 の拡張メンバー)。
  - `RemotingServices.IsTransparentProxy` など(.NET にはプロキシが無いので false、`GetRealProxy` は null)、`RealProxy`、`IRemotingTypeInfo`。
  - `AppDomain.DefineDynamicAssembly` → `AssemblyBuilder.DefineDynamicAssembly`、`AppDomainSetup.ConfigurationFile`。
  - `AssemblyBuilder.DefineDynamicModule` のファイル名・シンボルの引数を持つオーバーロード(メモリ上のモジュールを作る)。
  - Remoting での受け渡し(`Marshal`、`Activator.GetObject`)や `AssemblyBuilder.Save` のように、動きを代われないものは置かない(変換器がスタブにして報告する)。
  - Linux で動かすための部品(変換器がソースをこれらの呼び出しに書き換える。Windows では元のソースと同じ動き):
    - `WindowsPath.Native`: パスの `\` をその OS の区切りにし、アプリのフォルダーの中ではファイル名の大文字小文字をディスク上のものに合わせる(Windows のファイル名は大文字小文字を区別しない。Mono の IOMAP と同じ)。`TrimStartRelative`: フォルダーに結合する前に区切りを取り除くとき、絶対パスはそのままにする。
    - `WindowsUri`: `/Portals/0/home.css` のようなルートからのパスを、Windows と同じく相対 URI として扱う(.NET は Unix では絶対の file URI とみなす)。
    - `Platform`: `PrincipalPolicy.WindowsPrincipal`(Windows 以外では認証されていないプリンシパル)、`WindowsIdentity.GetCurrent().Name`(ユーザー名)。
    - `AsyncDelegate`: デリゲートの `BeginInvoke`/`EndInvoke`(.NET には無い。スレッドプールで実行する)。
    - `EventLogs`: イベントログ(`EventLog.WriteEntry`・`SourceExists`・`CreateEventSource`)。Windows 以外では .NET は例外を投げる。エントリーを標準エラーに書く(systemd のジャーナルやコンテナのログに残る)。
  - VB の My(Web のプロジェクトのもの。.NET Framework の Microsoft.VisualBasic.dll にあり、.NET では Windows のデスクトップの Microsoft.VisualBasic.Forms にしかないか、どこにも無い): `ContextValue`(コンパイラーの My のテンプレートが使う。要求ごと、要求の外ではスレッドごと)、`ServerComputer`(My.Computer)と `FileSystemProxy`(My.Computer.FileSystem。.NET にある `Microsoft.VisualBasic.FileIO.FileSystem` に任せる。`FindInFiles` は自前)、`WebUser`(My.User。要求のユーザー)、`AspLog`(My.Log。System.Diagnostics のトレース)。
- `QuickIO.NET`(アセンブリ `SchwabenCode.QuickIO`): Win32 のファイル API を使う Windows 専用のパッケージ(DNN の FileSystemUtils)。同じ API を System.IO で提供し、`PathNotFoundException` などの例外も同じ型で投げる。変換器はパッケージを外す(`rules/packages.json` の `shimPackages`)。
- 変換器は、変換するすべてのプロジェクトに互換アセンブリを参照させる。
- ファサードは Microsoft の公開鍵で公開署名する(`keys/`、`extract-keys.ps1` で .NET Framework のアセンブリから公開鍵を取り出したもの)。署名が無いと、元の名前で参照するアセンブリとの同一性が合わず CS0012 になる(dnn の ModulePresenterBase)。

## wt(WingtipToys、実在の OSS)

`convert-project.ps1` で旧形式の csproj から SDK 形式を作る(ソースは一切変更しない)。

    .\experiments\wf4c\convert-project.ps1 -Project corpora\work\wingtiptoys-master\WingtipToys\WingtipToys\WingtipToys.csproj -Out experiments\wf4c\wt -Root corpora\work\wingtiptoys-master
    # 実行: experiments\wf4c\wt\WingtipToys\WingtipToys で dotnet bin\WingtipToys.dll

結果: **8 画面中 6 一致**。残り 2 は既知の丸めの差(`double` 22.5 の通貨書式が .NET Framework は ¥23、
.NET は ¥22)で、アプリのコード側の差。(2026-10-19 に解消: FrameworkNumbers。8/8 になった)EF6 6.5.1、ASP.NET Identity 2 + OWIN(.NET Framework 版のまま)、
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
    .\experiments\wf4c\convert-project.ps1 -Project corpora\work\BlogEngine.NET-3.3.8.0\BlogEngine\BlogEngine.NET\BlogEngine.NET.csproj -Out experiments\wf4c\be -Root corpora\work\BlogEngine.NET-3.3.8.0 -ExcludeFiles $ex
    # 実行: experiments\wf4c\be\BlogEngine\BlogEngine.NET で dotnet bin\BlogEngine.NET.dll

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
- ProjectReference を辿ってライブラリも変換する。HintPath の DLL(packages 外)はリポジトリ内の場所のまま参照する(リポジトリ全体をコピーするため)。
- web.config の `<compilation><assemblies>` も参照に写す(System.Management → パッケージ)。
- System.ServiceModel.Syndication などは .NET のパッケージに置き換える。BinaryFormatter の警告(SYSLIB0011)は抑止する(動作は .NET 側の設定次第)。

## Linux(Docker、2026-09-26)

`run-linux.ps1` で、変換済みのアプリを Linux コンテナ(mcr.microsoft.com/dotnet/sdk:10.0)の中でビルドして実行し、ホストの ParityTest で正解データと比べる。

    .\experiments\wf4c\run-linux.ps1 -App ProductAdmin
    .\experiments\wf4c\run-linux.ps1 -App be\BlogEngine\BlogEngine.NET -Scenario corpora\regression\be.scenario.json -Golden corpora\parity\be.golden-webforms.json
    .\experiments\wf4c\run-linux.ps1 -App wt\WingtipToys\WingtipToys -Scenario corpora\regression\wt.scenario.json -Golden corpora\parity\wt.golden-webforms.json -SqlServer

| 対象 | Linux | Windows |
|---|---|---|
| サンプル 4 つ | **30/30** | 30/30 |
| be | **5/5** | 5/5 |
| wt | 5/8 | 6/8 |

be の 5/5 の見直し(2026-09-30): それまでの 5/5 は、ParityTest が 404 のときに `.aspx` を開き直す仕組み(Blazor 化のときの名残り)に助けられていた。変換後の be は `/archive`・`/post/...`・`/category/...`・`/page/...` が 404 だった(BlogEngine は URL を HttpModule で書き換え、IIS は `runAllManagedModulesForAllRequests` でどの要求もモジュールに渡す)。変換器は、web.config がどの要求もマネージドのモジュールに渡すとき(`runAllManagedModulesForAllRequests="true"`、または `managedHandler` の前提条件の無いモジュール)、すべての要求を Web Forms に渡すようにした。ParityTest の開き直しはやめ、シナリオはその時に開いていたパス(`.aspx`)にした(正解データは同じ)。Linux のコンテナで 5/5(開き直し無し)。同じ判定で mojo・n2・yaf もすべての要求を Web Forms に渡すようになる(未検証)。

be の管理画面の Web API(`/api/dashboard` など)が 500 だった(Windows でも)。SimpleInjector の古い DLL(ExecutionContextScoping)が `[mscorlib]System.Runtime.Remoting.Messaging.CallContext` を使い、.NET の mscorlib には無い。フォークの `CallContext` は System.Web にあり、ソースはコンパイルし直すので名前で見つかるが、DLL はアセンブリと型の組で結び付くので見つからない。変換器が、ビルドの後に bin のソースの無い DLL の型参照を調べ、実行時に解決しないものを、その型を public で持つアセンブリ(フォーク・shim・互換アセンブリを優先)に付け替えるようにした(`AssemblyRetargeter`。mscorlib に限らずどのアセンブリの参照も同じ。アプリにある版より新しい版への参照は、その版に下げる。bindingRedirect に当たる)。書き換えた DLL は出力の `foc-retargeted` に置き、各プロジェクトのビルドが元の DLL の代わりにコピーする(コンテナの中でビルドし直しても同じ)。配置済みサイトから組み立てたサイトの bin はその場で書き換える。どこにも無い型への参照はレポートに出す。be: ExecutionContextScoping・Integration.WebApi の CallContext → System.Web。wt: Microsoft.Owin.Security の `[System.Security]DataProtector` → System.Web。be はログインして管理画面の API(dashboard・packages・posts)が Windows・Linux とも 200。Linux の比較は be 5/5、wt 5/8(以前と同じ)。解析も、DLL の参照が「アセンブリ+型」のまま .NET 10 で解決するかを調べ、解決しないが .NET 10 にある型を「付け替える(→ 付け替え先)」と示す(`ApiUsage.RetargetedTo`、API-ANALYSIS.md の「DLL の参照の付け替え」、Studio のカードと部品のバッジ)。be・wt とも、解析の結果は変換器が実際に付け替えたものと同じ。型は .NET にあるがメンバーが無いもの(DLL からの呼び出しは `MissingMethodException`)は、変換器が DLL の呼び出しを互換アセンブリの拡張メンバーに置き換える(C# 14 の拡張メンバーが静的メソッドになるのを使う。`callvirt T::M` → `call Members::M(T, ...)`)。wt: OWIN(Microsoft.Owin.Host.SystemWeb)の起動時の探索が使う `AppDomainSetup.PrivateBinPath`・`PrivateBinPathProbe`(互換アセンブリに足した。ASP.NET と同じ `bin` と `*`)。be: SimpleInjector・Dynamic.dll の `AppDomain.DefineDynamicAssembly`(ソース向けに互換アセンブリにあったもの)。解析もこれを「呼び出しを置き換え」と示す(C# 14 の拡張プロパティ・静的メンバーもメタデータから読む)。残り(レポートに出る): `AppDomain.CreateDomain`・`Evidence`(.NET に無い仕組み)、`LambdaExpression.CompileToMethod`、Elmah の `SqlConnectionStringBuilder.AsynchronousProcessing`、デザイナーの API。あわせて、解析が DLL のジェネリック型の入れ子の型(`List<T>.Enumerator`)を `List`1.Enumerator{`0}` と書いていたため、`List<T>.GetEnumerator` などを「.NET に無い」と誤っていたのを直した(`List{`0}.Enumerator`)。Linux の比較は be 5/5、wt 5/8(以前と同じ)。wt は OWIN が起動するようになった(`GetOwinContext()` が動く)。

### ASP.NET MVC 5(2026-09-30、フォーク 0031、`1.6.5-w2l.3`)

コーパス `mvcmovie`(MvcMovie: MVC 5 の公式チュートリアルのアプリ。EF 6 Code First、ASP.NET Identity + OWIN、LocalDB。dotnet/AspNetDocs のフォルダーを commit 固定で取得)。MVC の DLL(System.Web.Mvc 5.2.3、Razor 3、WebPages 3)は移植せず、.NET 10 の上でフォークの System.Web を使ってそのまま動かす。IIS で採った正解(12 画面: ホーム、ルーティングとクエリのモデルバインド、一覧、作成の検証エラー、EF 6 での保存と一覧・詳細・検索、編集、ログイン、登録)と比べて **Windows・Linux とも 11/12**。残りの 1 つは検証メッセージの言語(下)。手で確かめたこと(Windows): 作成・編集・削除、検索とジャンルの絞り込み、Identity の登録・ログイン・ログオフ・パスワード違い、偽の偽造防止トークンが 500(IIS と同じ)。

直したこと:
- MVC が参照する .NET に無い型: `System.Data.Linq.Binary`(既定のモデルバインダーが登録する。無いと引数のあるアクションがすべて System.Data.Linq の読み込みで失敗)、`System.Data.EntityState`(EF 1-4。表示・編集テンプレートが typeof で使う)を、DLL からだけ見える互換アセンブリ `FrameworkOnCore.Compat.ForDlls` に置いた。ソースからは見えない(EF 6 の `System.Data.Entity.EntityState` と曖昧になるため)。変換後のプロジェクトはコンパイルに使わず、出力にコピーだけする(`ReferenceOutputAssembly=false`)。DLL の参照は付け替えで向く。`SHA256Cng`(偽造防止トークン、子アクションの出力キャッシュ)は互換アセンブリに置いた(ソースからも使える)。
- Razor のビューのコンパイル: MVC の HtmlHelper の拡張の型(`Expression<>`)が System.Core にあり CS0012。ページ・ビューのコンパイルに、ランタイムの型の転送だけのアセンブリ(System、System.Core、System.Data、System.Xml など)をすべて参照に入れる(フォーク 0031。アプリが自分で持つもの、System.Web は除く)。
- プロジェクトが挙げているのにリポジトリに無いソース・埋め込みリソース(MvcMovie の Properties\AssemblyInfo.cs)は外して報告する(元のビルドも CS2001 で止まる)。
- 正解の採取: `build-original.ps1` は HintPath が指す版のパッケージを入れる(MvcMovie は packages.config だけが依存の更新で上がり、HintPath は古い版のまま)。`record-webforms-golden.ps1` に接続文字列の名前ごとの置き換えと、採取前のデータベースの削除を足した。ParityTest に ID の無いボタンを文言で押す `clicksubmit` を足した。
- be・wt の Windows での比較は be 5/5、wt 6/8(以前と同じ)。

残り(未対応): **.NET Framework の言語パックの訳**。日本語の Windows の .NET Framework は、DataAnnotations などのメッセージを日本語で出す(「フィールド Price は 1 から 100 までの範囲で指定してください。」)。.NET 10 には各国語の訳が無く英語になる。元のサーバーから訳を採って、.NET の同じメッセージの訳として置く方法が考えられる(カルチャのデータと同じ考え方)。

nopCommerce 3.90(`nop390`、MVC 5 の最後の版、56 プロジェクト、プラグイン、Autofac、AutoMapper、EF 6)は、元のビルドから組み立てたサイトで **Windows で動く**。インストーラーで SQL Server Express にサンプルのデータ付きでインストールでき、店頭(トップ、カテゴリー、検索、商品の詳細、カートへの追加(AJAX)とカート、ログイン、登録)と管理画面(ダッシュボード、商品の一覧(Kendo の JSON)と編集、注文、設定)が 200。IIS の正解との比較と Linux はまだ。直したこと:
- `AppDomain.CurrentDomain.DynamicDirectory`(ASP.NET の Temporary ASP.NET Files の生成物のフォルダー。.NET では常に null): プラグインの読み込み(PluginManager)がそこへ DLL を写して名前で読む。ソースの書き換え(`platformReplacements`)で `Platform.DynamicDirectory` にし、フォークの生成物のフォルダー(`HttpRuntime.CodegenDir`、アセンブリの解決が探す)を返す。
- `Microsoft.Bcl.Build`(Microsoft.Bcl・Bcl.Async が依存する .NET Framework のビルドの道具。そのターゲットが MSB4062): `inertPackages` にし、ExcludeAssets all で参照する(依存からも何も来ない)。
- `WindowsAzure.Storage` 8.1.1: .NET Standard の版には同期の API(`CloudBlockBlob.Exists` など)が無く、AzurePictureService のメンバーが次々スタブになり、最後に DI の登録(`DependencyRegistrar.Register`)全体がスタブになっていた。元が動かしていた .NET Framework の版を使う(`frameworkAssets`)。nop 3.90 のパッケージで .NET Framework と .NET Standard の両方の版を持つものは 15 あり、API が違って困ったのはこれだけ。
- AutoMapper 5.2(12 より前すべて): `Enumerable` の拡張メソッドを private のものまで集めて MakeGenericMethod する。.NET の `Enumerable` には private の拡張メソッド(`MaxInteger<T>`、ジェネリック制約つき)があり、MapperConfiguration が作れず起動に失敗していた(6.2〜8.1 も同じ)。DLL の中の特定の呼び出しを互換アセンブリの同じシグネチャのメソッドに置き換える規則 `dllCallReplacements` を足し、`Profile.IncludeSourceExtensionMethods` の中の `TypeExtensions.GetDeclaredMethods` を public のものだけ返す `DllFixes.PublicDeclaredMethods` に替える(.NET Framework の Enumerable と同じ集まり)。
- **C# 14 の first-class span**: 式ツリーの中の配列の `Contains`(`productIds.Contains(p.Id)`)が `MemoryExtensions.Contains(ReadOnlySpan<T>, T)` に結び付き、EF 6 が SQL にできない(「LINQ to Entities does not recognize the method Contains(ReadOnlySpan)」)。アナライザー FOC1007 が見つけ、変換器が `Enumerable.Contains(配列, x)` に書き換える(当時の C# の結び付き)。nop 3.90 で 30 か所。EF 6、LINQ to SQL、OData など式ツリーを使うどのアプリでも起きる。テストの Roslyn は SDK のコンパイラーと同じ 5.9 にした(4.x・5.0 では再現しない)。
- Dynamic LINQ(System.Linq.Dynamic)が式に使える型の一覧に EF 4 の `System.Data.Objects.EntityFunctions` を入れていて(typeof)、動的な式がすべて System.Data.Entity の読み込みで失敗した(検証)。名前だけの型を `FrameworkOnCore.Compat.ForDlls` に置いた(関数そのものは無い。EF 6 では DbFunctions)。
- 変換器: 呼び出しの置き換えだけがあった DLL でもビルドし直すようにした(付け替えが無いと差し替えがビルドに入っていなかった)。

認可(フォーク 0030、`1.6.5-w2l.2`): wt の `/Account/Manage`(FriendlyUrls のルート)が、ログインしていなくても動いていた(`Account/Web.config` の `<location path="Manage.aspx">` の `<deny users="?"/>`。`/Account/Manage.aspx` は守られていた)。ルートは物理ページへのアクセスを `UrlAuthorizationModule.CheckUrlAccessForPrincipal` で確かめるが、これが「UrlAuthorizationModule が登録されているか」を型名で調べる。machine.config はアセンブリ名の無い型名で登録していて、フォークの `Type.GetType` の型リゾルバーはアセンブリ名が無いと null を返していた(.NET Framework の `Type.GetType` は呼び出し元のアセンブリと mscorlib を探す)。そのため常に「許可」だった。ルートのページ(FriendlyUrls、`PageRouteHandler`)と、サイトマップのセキュリティトリミング(be が有効にしている)に効く。同じ書き方のリゾルバー 62 か所(34 ファイル)を、アセンブリ名が無ければ `Type.GetType` に任せるように直した。確認: wt(Windows)の `/Account/Manage`・`/Checkout/...`・`/Admin/...` がログイン画面へ(302)、公開のページは 200。be(Windows)の `setup/`・`admin/` もログイン画面へ、ログインすると管理画面と API が 200(Linux でも)。Linux の比較は be 5/5、wt 5/8(以前と同じ)。コーパスの認可の規則: wt(Account/Manage.aspx、Admin、Checkout)、be(setup、サイトマップのトリミング)、n2(管理画面 N2/ とプラグイン)、dnn(CKEditor のモジュールがインストール時に web.config に足すもの)、nop(allow のみ)。mojo・yaf・imis はコードで認可していて規則は無い。

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

### LINQ to SQL(System.Data.Linq)の移植(2026-09-30、フォーク 0032、`1.6.5-w2l.4`)

referencesource(MIT)の System.Data.Linq を、フォークの新プロジェクト `WebFormsForCore.Data.Linq`(アセンブリ名 System.Data.Linq、元と同じ ECMA の公開キー)として移植した(PORT-PROMPT.md の手順)。スタブをやめ、既定の選択は「移植版を使う」(`linq-to-sql:port`)。SQL Server 用(System.Data.SqlClient 4.9.0、変換後アプリと同じパッケージ)。

- **ソース**: 元のコードは変えないのが原則。変更は `Core\` の追加ファイルと `#if`(ObjectReaderCompiler のデバッグ用キャプチャ = Reflection.Emit の Save は .NET Framework のみ)だけ。
- **リソース**: referencesource にはリソーステキストの一部(System.Data.Linq.txt)しか無く、Strings/Error(3 名前空間 × 各 50〜160 メンバー)が生成できない。`generate-dlinq-resources.ps1` が .NET Framework 4.8 の実アセンブリから再生成する(書式文字列はリソースから、例外の型は各 Error メソッドを実行して採取)。メッセージと例外の型は .NET Framework と同一。
- **変換器**: `frameworkReferenceOptions`(選択で有効になる frameworkReferences。選ばなければ noAnswer に落ちる)を追加。bin の DLL だけが参照するアセンブリ(System.Web.Mvc.dll・System.Web.WebPages.dll → System.Data.Linq)は、ビルド後に検出してパッケージを足し、もう一度ビルドする(`AssemblyRetargeter.ReferencedMissing`)。ForDlls の `System.Data.Linq.Binary` スタブは削除(型の二重定義になるため)。
- **テスト**(`tests/FrameworkOnCore.Tests/DataLinq`、97 件全体が Windows / Linux とも緑):

| 領域 | 正常系 | 境界・異常系 | 場所 |
|---|---|---|---|
| 属性マッピング(dbml 生成コードの形) | 表・列・キー・関連の両側・IsDbGenerated/IsVersion | — | DataLinqMappingTests |
| Binary(MVC のモデルバインドも使う) | 変換・等価・ToString(`"Base64"`) | null は空(net48 実機で確認)、不変性 | DataLinqTypesTests |
| EntitySet / EntityRef | Add/Remove/付け替えの両側同期 | 外部キーの変更拒否(ForeignKeyReferenceAlreadyHasValueException) | DataLinqTypesTests |
| 変更追跡 | Insert/Attach/更新/削除が GetChangeSet に載る | 未 Attach の削除・追跡無効・Dispose 後(メッセージが .NET Framework と同一) | DataLinqContextTests |
| SQL 変換 | be の 9 形(Guid 等価、ToLower、null 許容、関連の JOIN、Take(1)、Contains、射影、OrderBy) | net48 のゴールデン(golden/record.ps1)と完全一致(改行は Environment.NewLine 依存で正規化) | DataLinqSqlGoldenTests |
| DB 実行(SQL Server 必須、無ければスキップ) | CreateDatabase/CRUD/関連グラフ/遅延読み込み | IDENTITY・rowversion の書き戻し、競合(ChangeConflictException→Resolve)、TransactionScope のロールバック | DataLinqDatabaseTests |
| 変換器の組み込み | 選択で参照⇄noAnswer が切り替わる、DLL 参照の検出 | — | ChoicesTests・AssemblyRetargeterTests |

  テストしない領域と理由: 継承マッピング・ストアドプロシージャ・XML マッピング(MappedMetaModel)・DataBindingList — コーパス(be/yaf/n2/MVC)が使わない。SqlCE — 対象外。
- **検証**: be Windows 5/5・Linux 5/5。be の DB ファイルシステム(DbFileSystemProvider)を Windows で e2e(be_w2l を Setup.sql で作成、管理者ログイン → `/api/upload` → 一覧 → `/file.axd` で読み出し → 削除、すべて DB の be_FileStore* を経由)。wt Windows 6/8(既知の丸めの差)。mvcmovie Windows 11/12(既知の言語パックの差。ForDlls の Binary 削除後もモデルバインドが動く = DLL 参照が移植版に結び付く)。Linux のテストは `run-tests-linux.ps1 -SqlServer`(SQL Server 2022 のコンテナ)で DB のテストまで緑。
- **見つけたこと**: (1) `DataContext.GetCommand` は SQL 生成モードをサーバーの版で決めるため接続を開く(.NET Framework も同じ)。SQL のテストに実サーバーが要るのはこのため。 (2) SQL 文の改行は Environment.NewLine(Linux では `\n`)。SQL としては等価。 (3) be の `DbFileSystemProvider.GetDirectory` は、正規化後のパスが `/` になる入力で無限再帰する(be 自身の潜在バグ。.NET Framework でも同じ。管理画面の実際の呼び方では起きない)。
- **全 API の新旧比較**(2026-09-30、`tests/DataLinqParity`): System.Data.Linq の公開・protected の全 API(486 のドキュメント ID)を対象に、53 ケース・約 1,900 の観測値(戻り値、例外の型とメッセージ、生成 SQL とパラメーター、DB の状態、Log)。同じソースを net48(旧 = .NET Framework の本物)と net10.0(新 = 移植版)でビルドし、旧で採ったゴールデン(`record.ps1`)と新を xUnit で 1 ケースずつ比べる(`DataLinqParityTests`)。あわせて (1) 移植版の API 一覧が .NET Framework と完全一致、(2) 全 API にケースがある(ケースのソースを Roslyn で束縛して機械的に照合。漏れがあれば失敗)、(3) 移植版に Debug.Assert が無い、を検査する。Windows(SQL Server Express)・Linux(SQL Server 2022 のコンテナ)とも全件一致。
  - 正規化は、LINQ to SQL ではなくランタイム・SQL クライアント・サーバーの差に限る: 改行、引数例外の定型文(パラメーター名は別に記録)、double の表記(正確な 10 進展開を 17 桁で切り捨て。G17 は両ランタイムで末尾の丸めが違う)、FormatException の文言、SQL Server のエラー(番号で記録)と情報メッセージ(サーバーの言語)、Log の Build 番号。日付を文字列にする CONVERT の結果はサーバーの言語で変わるので SQL だけを比べる。
- **見つけたこと(新旧比較)**: 移植版は `DEBUG` 付きでビルドされていて、referencesource の Debug.Assert 150 か所が生きていた。`DataContext.Translate(DbDataReader)` の結果の GetResult で SingleResult の Assert(実行結果がある前提)が失敗し、**.NET ではプロセスが終了する**(.NET Framework の出荷版では Assert はコンパイルされていない)。移植版は構成によらず `DEBUG` を外す(フォーク 0033)。
  - **類似の問題の調査**: フォーク全体が Debug 構成でパッケージされていて、出荷 DLL に Debug.Assert/Fail の呼び出しが約 630 か所あった(System.Web 79、System.Web.Extensions 111、Serialization.Formatters 141、WebGrease 70、DynamicData 51 など。Mono.Cecil で IL を数えた)。どれも失敗すれば本番でプロセスが落ちる。`pack-frameworkoncore.ps1` を Release 構成に変えた(元の .NET Framework・上流の配布と同じ)。`1.6.5-w2l.5` で全パッケージ 0 か所。be Windows 5/5・Linux 5/5、wt Windows 6/8・Linux 5/8(いずれも以前と同じ既知の差のみ)、テスト 154 件が Windows・Linux とも緑。
  - MVC(Release のフォーク、2026-09-30): mvcmovie Windows 11/12・Linux 11/12(以前と同じ。差は .NET Framework の日本語の言語パックの検証メッセージのみ)。nop390 Windows: インストーラー、店頭 12 画面、カート、管理画面(一覧の JSON、編集、注文、設定)がすべて 200。
  - **nop390 を初めて Linux で(変換器が書いた Dockerfile のイメージ)**: インストーラーが `The type initializer for 'Windows.Win32.PInvokeGdiPlus' threw an exception` で失敗する(サンプルデータなしでも。既定の画像を System.Drawing で処理する)。Windows でインストールした DB を移して(SQL Server Express 2025 のバックアップは 2022 に戻せないので 2025 のコンテナ)起動すると、店頭 12 画面・カート・管理画面は Windows と同じくすべて 200。Windows で作られたサムネイルは配信されるが、新しいサイズのサムネイルの生成は同じ GDI+ の例外で失敗する。**nop390 の Linux の残りは System.Drawing(libgdiplus の選択肢、カタログの `system-drawing:libgdiplus` が planned)だけ**。
  - 検証の手順の注意: Windows で起動したアプリを親プロセスの Stop-Process で止めても、再起動後のワーカー(フォーク 0016・0018)が残って同じポートで待ち受ける。後の検証の要求がそちらに届いたので、止めるときはポートの待ち受けプロセスを止める。
- **残り**: be の DB ファイルシステムの Linux e2e は未実施(ポートの SQL 経路自体は Linux の DB テストで検証済み)。nop390・yaf・n2 の再検証は未実施(yaf・n2 は System.Data.Linq を型参照するだけ)。System.Web.Extensions の LinqDataSource はフォークで外れたまま(コーパスに利用が無い。戻すならフォークの Web.Extensions に WebFormsForCore.Data.Linq への参照を足す)。

### System.Drawing の移植(2026-09-30、フォーク 0034〜0037、`1.6.5-w2l.5`)

System.Drawing.Common は .NET 7 から Windows 専用。Linux 実装を持つ最後の版、dotnet/runtime release/6.0(76b96800a40e、MIT)の `src/libraries/System.Drawing.Common` を、フォークの新プロジェクト `WebFormsForCore.Drawing.Common` として移植した(PORT-PROMPT.md の手順)。アセンブリは System.Drawing.Common 10.0.1.0(Open キー。Microsoft の 10.0.0.0 より上なので、どの参照もこれに結び付く)。Windows 版(GDI+)と Unix 版(libgdiplus、Mono 由来の実装。.NET 6 のスイッチ `EnableUnixSupport` は不要にした)を 1 つのパッケージ(`runtimes/win`・`runtimes/unix`)に入れる。フォークの System.Web、System.Drawing のファサード、Ajax Control Toolkit はこれを参照する。

- **コミットの分け方**: 0034 は元のソースそのまま、0035 はフォークでのビルド(元の csproj のソース一覧を `System.Drawing.Common.Sources.props` に、リソースのアクセサーは `generate-drawing-sr.ps1` で生成、警告なし、DEBUG なし)、0036 は新旧比較で見つけた修正、0037 は Ajax Control Toolkit のサブモジュール(`patches/AjaxControlToolkit/0001`、サブモジュールで `git am`)。元のコードの変更は `#if WebFormsForCore` か `Core\` の追加ファイルだけ。
- **変換器**: System.Drawing(参照・パッケージ・ソースの using)をフォークのパッケージに付け替える(`rules/packages.json`。旧スクリプト `convert-project.ps1` も同じにし、あわせて System.Data.Linq の対応を足した)。カタログの `system-drawing` は「移植版」だけ。配置は、描画を使う DLL(自前のもの。フォークと、どのサイトにも入る System.Windows.Extensions は除く)かページがあるときだけ、Dockerfile・install.sh で libgdiplus と fonts-liberation2(Arial・Times New Roman・Courier New と同じ文字幅)を入れる。
- **全 API の新旧比較**(`tests/DrawingParity`、共通部分は `tests/Parity.Core` に分けた): .NET Framework の System.Drawing の全 API(3,354 のドキュメント ID)から、型とメンバー名ごとに 1 ケースを作る(列挙型は値の一覧で 1 ケース)。各オーバーロードを型と引数名から決めた見本の引数で呼び、戻り値・参照渡しの引数・呼んだ後のインスタンス・描いたキャンバスを書き出す。呼ばないメンバーは理由つき(`ApiCases.Excluded`: .NET に無いデザイナー型・CAS・構成セクション、ファイナライザー、デリゲートの非同期呼び出し、プリンターへの送信、画面のコピー、呼び出し側のメモリを指すポインター)。実際の使い方のシナリオ 12(サムネイル、JPEG の品質、EXIF の回転、GIF のフレーム、TIFF のページ、CAPTCHA 風の文字、グラフ、PNG の透過、インデックスカラー、ストリームが開いたままか、EncoderParameter に実メモリ、CopyFromScreen の引数)。計 1,616 ケース・約 3,900 の観測値。あわせて (1) 移植版の API が .NET Framework のものから `Excluded` の「.NET に無い」169 を引き、.NET が足した 40(`api-additions.txt`)を足したものと一致、(2) `Excluded` の各項目が何かを除外している、(3) Debug.Assert が無い、を検査する。
  - **比較の仕方**: Windows は全行を完全一致で比べる(同じ gdiplus.dll)。Linux は libgdiplus なので、画素(`~px` の行: 8×8 のグリッドの平均の差が 12 以下。同じ絵なら 5 以下、違う絵は 29 以上だった)、フォントに依存する値(`~font` の行: ラベルだけ)、数値(相対 1e-5。libgdiplus は float で計算の順序も違う)に許容を持つ。
  - **既知の差**(`known-differences.json`): 差ってよい行を、ケースと行の正規表現と理由で持つ。どの差分にも使われない項目はテストが失敗する(古くなった項目を残さない)。.NET Framework の値が実行ごとに変わる項目(未初期化のメモリ)は `varies`。Windows は 13 項目(DarkSeaGreen の色値、ImageFormat.ToString、例外のメッセージや ParamName、.NET が直した NullReferenceException、SupportsColor の問い合わせ方、CAS、メタファイルのヘッダーの未初期化メモリ、GDI+ の bilinear warp が呼ぶたびに変わる、など)。Linux は加えて 10 系統(システムの色とアイコン、印刷、パスの幾何、メタファイル、libgdiplus に無い機能、ウィンドウ・デバイスコンテキスト、フォント、libgdiplus の値、読んだ画像の情報)。どれも .NET の System.Drawing.Common、ランタイム、libgdiplus、.NET Framework の未定義動作のもので、移植の誤りではない。
  - Probe の数値の書き方を、正確な 10 進展開の切り捨て(double 17 桁、float 9 桁)に変えた。G17 は .NET Framework と .NET で末尾の丸めが違い、同じ値が違う文字列になった(System.Data.Linq のゴールデンも 1 行変わった)。
- **見つけて直したこと**(フォーク 0036):
  - Windows: `SystemFonts.DialogFont` が日本語の Windows で Tahoma になる(.NET の LCID の比較の誤り。.NET Framework の IL で `& 0x3ff` を確認)。メタファイルのヘッダーで GDI+ が書かない領域を読む(.NET Framework では実行ごとに違う値。移植版は 0)。`GraphicsPathIterator` の配列の後ろを 0 にする(.NET Framework と同じ)。
  - Linux: 別の Graphics の状態を `Restore`/`EndContainer` に渡すと libgdiplus が SIGSEGV でプロセスごと落ちる → GDI+ と同じく無視。64 KiB 未満の JPEG をストリームから読むと EXIF が消える(libgdiplus 6.1 のバッファの扱い)、Motorola 順("MM"、多くのカメラと GDI+ 自身)の EXIF の値がそのままの順で返り、回転 6 が 1536 に読める(写真が回転されない)→ 機械の順にする。`new Bitmap(w, h)` の解像度が 0(GDI+ は 96)。scan0 なしで小さすぎる stride を渡すと確保した領域の外を読み書きする。Icon のサイズの選び方が Windows と違う(16×16 のはずが 32×32)。`new Icon(stream)` がアプリのストリームを閉じる。`Font.FromLogFont` がアプリの LOGFONT を受け付けない・関数名の誤りで常に失敗・できても "Microsoft Sans Serif 10pt" になる。`GetNearestColor` が常に黒。メタファイルを PNG などで保存できない(GDI+ は描いて保存する)。メッセージや `ToString` の違い。
  - 両方: `Stream.Read` が要求より少なく返す場合を考えていない読み込み(CA2022)。
- **類似の問題の調査**: Unix 版の P/Invoke 556 本を libgdiplus 6.1 のエクスポートと照合し、名前のずれ 2 本(上の LOGFONT・HFONT)を直した(残る 1 本は macOS 専用)。アプリのストリームを閉じる箇所、不正確な読み取り(ビルド警告)を全体で確認した。配置の「描画を使うか」の判定が、どのサイトにも入る System.Windows.Extensions のために常に真になっていたのを直した。
- **検証**: テスト Windows 1,775 件・Linux 1,756 件(SQL Server の要る 19 件はスキップ)がすべて緑。be Windows 5/5・Linux 5/5、wt Windows 6/8・Linux 5/8(以前と同じ既知の差のみ)。be/wt の変換結果の差はプロジェクトファイルのパッケージ参照だけ。
- **nop390 を Linux で(2026-09-30、ユーザーの指示で検証)**: 変換し直したサイト(フォークの System.Drawing.Common。Dockerfile が libgdiplus とフォントを入れる)を、変換器が書いた Dockerfile のイメージと SQL Server 2022 のコンテナで動かした。以前 GDI+ の例外で失敗していた**インストーラーがサンプルデータ付きで完了**(商品 45、画像 78。既定の画像の処理が System.Drawing)。店頭 18 画面・カートへの追加(AJAX)とカート・ログイン・管理画面(ダッシュボード、商品一覧の JSON、編集、注文、設定)がすべて 200。ページのサムネイル 31 枚はすべて画像で、Linux で生成された(68 枚、目視でも正しく縮小)。管理画面からの画像のアップロード(縮小して保存)も成功。nopCommerce のエラーログは空。
  - **見つけて直したこと(フォーク 0038)**: 新しい配置の**初回の起動**でコンパイルが失敗し(CS0006: プラグインの DLL が見つからない)、アプリが再起動していた(InitializationError。2 回目から動く)。ASP.NET は PreApplicationStart の後、アプリのハッシュが変われば生成物のフォルダーを掃除する(referencesource と同じ順序)。nopCommerce は PreApplicationStart でプラグインを動的ディレクトリへ写して読み込む。Windows では読み込み済みの DLL は使用中で消えない(ASP.NET はそれを前提に `.delete` を残す)が、Linux は消せてしまい、続くページのコンパイルが参照できなかった。読み込み済みのアセンブリのファイルを、Windows と同じく使用中として扱う(`Util.IsLoadedAssemblyFile`、削除の 2 経路)。直した後は初回から 200 で再起動なし。Windows は変わらない(判定はファイルシステムに任せる)。
  - **検証の手順の注意**: `run-linux.ps1` はビルドサーバー(MSBuild、コンパイラー。約 1 GB)を残したままアプリを起動していて、SQL Server のコンテナと同時だと Docker の VM(3.8 GB)のメモリが尽き、be が最初の要求で OOM で落ちた。アプリの起動前にビルドサーバーを止めるようにした。be Linux 5/5(SQL Server のコンテナありでも)、wt Linux 5/8(以前と同じ)。
- **残り**: libgdiplus に無い機能(GraphicsPath.Widen/Warp、SaveAdd による複数ページ・アニメーションの書き込み、EMF+ の描画)は Linux では使えない。

### System.Web.DataVisualization(グラフ)の移植(2026-10-01、フォーク 0039・0040、`1.6.5-w2l.6`)

Chart コントロール(referencesource の System.Web.DataVisualization、MIT)。フォークにはソース(115 ファイル、元と同一)があってビルドされていなかったので、PORT-PROMPT.md の手順でビルドできるようにした。アセンブリは System.Web.DataVisualization 10.0.29.0(フォークの版)、公開キーは .NET Framework と同じ System.Web.Extensions のもの(31bf3856ad364e35)、net10.0。パッケージ WebFormsForCore.Web.DataVisualization。描画は System.Drawing の移植版(上)。

- **コミットの分け方**: 0039 はフォークでのビルド(csproj、`Core\AssemblyRef.cs`、.NET Framework のアセンブリから `generate-dataviz-resources.ps1` で作った `Core\SR.resx` 1,290 文字列。デザイナーのビットマップは取らない)、0040 は新旧比較と Linux で見つけた修正。元のコードの変更は `#if WebFormsForCore` か `Core\` の追加ファイルだけ。
  - `SQLRS_CONTROL` を定義する(.NET Framework の出荷版と同じ)。無いと空の点のコードが存在しないフィールドを参照し、.NET Framework の空の点の色(Empty)とも合わない。
- **変換器**: System.Web.DataVisualization をフォークのパッケージに付け替える(`rules/packages.json` の `frameworkReferences`。部品 `charts` の選択肢 `port`(既定)のとき。`none` を選ぶと従来どおり `noAnswer` で、web.config の登録も外す)。web.config の `<assemblies>`・`<pages><controls>`・ChartImg.axd のハンドラーはそのまま残る。appSettings の `ChartImageHandler` の `dir=` が Windows のドライブ(nop の `c:\TempImageFiles\` など)なら、Linux では配置の設定(`APPSETTING_ChartImageHandler`)で替えるよう Platform としてレポートする(.NET Framework もフォルダーが無ければ例外なので、値は書き換えない)。解析は、フィードのパッケージを読むので移植版の API を自動的に「ある」とする。
- **全 API の新旧比較**(`tests/DataVisualizationParity`): .NET Framework の全 API(1,661 のドキュメント ID)から型とメンバー名ごとに 1 ケース。System.Drawing の仕組みを `tests/Parity.Core/ApiParity.cs` に共通化して使う(ジェネリック型の定義のメンバーは見本のインスタンスの閉じた型で呼ぶ)。見本は、系列 2 つ・グラフ領域・凡例・タイトル・注釈・ストリップライン・カスタムラベル・画像を持つグラフ(`Samples.SampleChart`)。インスタンスはその中から型で探し、無ければコンストラクターで作る。グラフの要素は DV が宣言するプロパティを 1 行に書く(`ChartDescribe`)。シナリオ 12(全グラフの種類の描画、nop のレポートの円グラフと画像マップ、3 つのカルチャのキーワード 17 種、FormatNumber イベント、データバインド、全財務式、統計、並べ替え・絞り込み・グループ化、XML とバイナリの保存、画像マップ、3D と注釈、画像形式)。計 1,039 ケース・2,689 行。呼ばないのは IChartStorageHandler・IDataPointFilter のメンバーだけ(インターフェイス。実装はシナリオと `samples/ChartProbe` で通す)。移植版の API は .NET Framework と完全に一致(欠け・余分 0)。
  - 実行は別プロセス: テストのプロセスには .NET の System.Web(4.0.0.0 のファサード)があるので、変換後のアプリと同じくフィードのフォークのパッケージを参照する net10.0 の exe を `DataVisualizationParityData/program` に置いて走らせる。比較の仕方(画素・フォント・数値の許容、既知の差)は System.Drawing と共通(`tests/FrameworkOnCore.Tests/Parity/ParityComparison.cs`)。
  - **既知の差**: Windows は 1 項目(`Chart.BuildNumber` がアセンブリの版を返す)。Linux は加えて 6 項目: カルチャのデータ(ICU: パーセントが小数 3 桁、ja-JP の通貨記号が全角。変換後のアプリは元のサーバーのカルチャのデータを使う)、円グラフの画像マップの座標(フォントの寸法と libgdiplus の弧の折れ線)、注釈の文字に合わせた大きさ、読んだ画像の情報とメタファイル(System.Drawing と同じ)、ハンドラーの既定の一時フォルダー、保存した XML の中の PNG のバイト。
- **見つけて直したこと**(フォーク 0040):
  - 数値の書き方: .NET Core 3.0 から double の書き方が変わり、グラフに出る。負のゼロが "-0"(-0 から始まる軸のラベル。幅が変わり描画もずれた)、書式なし・"G" が往復できる最短の表記(0.1 刻みの軸で 0.30000000000000004)、中間の値の丸めが 2 進の正確な値で偶数へ(0.125 の "C2" が $0.12、.NET Framework は $0.13)。.NET Framework と同じ「15 桁 → 書式の桁で四捨五入」を decimal で行う(`Core\FrameworkNumberFormat.cs`、軸のラベル・キーワード・ツールチップが通る `ValueConverter.FormatValue`)。全シナリオの画素と文字が Windows で一致。
  - ChartHttpHandler(ChartImg.axd)が Linux で動かない: キーとフォルダーを `\` でつないでいて、Linux では区切りにならない(storage=file の画像が "/app\charts_0\..." という名前のファイルになり、次の要求で見つからない)。プラットフォームの区切りにした。画像ファイルを 1 回の Read で読んでいた(CA2022)のを ReadAtLeast に。
- **サイトでの確認**(`samples/ChartProbe`、`chart-probe.ps1`): nop と同じ登録の Web Forms のサイト(appSettings の ChartImageHandler `storage=file`、ハンドラー、`asp:` の Chart)。ChartImg.axd 経由の画像と画像マップ、ImageLocation でアプリのフォルダーに書く画像、コードで作って応答に書く PNG、無い画像の 404 を、IIS(.NET Framework 4.8)で採った 27 行と比べる。変換は新しい変換器(既定の選択)、Linux は変換器が書いた Dockerfile のイメージ。**Windows: 一致**(画像はサイズと 8×8 のグリッド。GDI+ は凡例のマーカーの 1 画素を実行ごとに違えて描く。.NET Framework でも 2 つのダイジェストが交互に出る)。**Linux: 一致**(許容の範囲。画像マップの座標の差は最大 5 画素、グリッドの差は平均 1.3 以下)。ハンドラーの画像は 1 回目 200、2 回目 404(配信後に消える)まで同じ。
- **類似の問題の調査**:
  - 変換器: 配置の `.dockerignore` が、サイトが出力そのもの(`--site` なしで Web プロジェクトをその場でビルド)のとき `*` と `!.` になり、イメージにサイトが入らなかった(`!.` は何も戻さない)。サイトに App_Data が無いと `VOLUME /app/App_Data` が root の持ち物で作られ、ランタイムが machine.config を書けず起動しなかった。どちらも直した(`DeployWriterTests`)。install.sh は該当しない(App_Data は実行時にアプリが作る)。
  - 新旧比較のケース: `Chart.SaveXml("Abc 123")` が作業フォルダーにファイルを残し、`LoadTemplate("Abc 123")` が前回の実行のそれを読んでいた(ゴールデンも)。テンプレートは事前に作った一時ファイルにし、ゴールデンを採り直した。
- **検証**: テスト Windows 2,823 件・Linux 2,804 件(SQL Server の要る 19 件はスキップ)がすべて緑。be Windows 5/5・Linux 5/5、wt Windows 6/8・Linux 5/8(以前と同じ既知の差のみ)。
- **残り**: nop(管理画面のレポート 2 つ)での実地検証は未実施(CLAUDE.md によりコーパスの検証は be/wt のみ。代わりに `samples/ChartProbe` で同じ登録と使い方を確かめた)。System.Windows.Forms.DataVisualization(Windows フォーム版)は移植していない。デザイナー用のリソース(Design.resources)は持たない。Linux では EMF 形式で保存できない(libgdiplus)。

### フォークをリポジトリに取り込む(2026-10-01)

それまでのフォークは、リポジトリの外の `_upstream`(上流の浅いクローン)に `patches/` の 40 本を当てたものだった。パッケージはそれを持つ開発機でしか作れず、修正のたびにパッチの書き出しとコミットが二度手間だった。上流 WebFormsForCore(MIT)を git subtree でトップレベルの `FrameworkOnCore.Runtime/` に履歴ごと取り込み、このリポジトリで保守する。

- 取り込み: 上流 22c7d354(当時の main の先頭)を `git subtree add --prefix=FrameworkOnCore.Runtime`(上流の履歴 441 コミット)。上流のサブモジュールの参照(`src/WebFormsForCore.AjaxControlToolkit`、`www`)は外し、Ajax Control Toolkit は上流が指していた c9952ac6 を `FrameworkOnCore.Runtime/src/WebFormsForCore.AjaxControlToolkit` に同じく subtree で取り込んだ(2,396 コミット)。`www`(上流の Web サイト)は取り込まない。
- パッチ 0001〜0036・0038〜0040 を `git am --directory=WebFormsForCore` でコミットとして積み、0037(サブモジュールの参照の更新)の代わりに Ajax Control Toolkit のパッチを `--directory=FrameworkOnCore.Runtime/src/WebFormsForCore.AjaxControlToolkit` で当てた。作者・日付・件名は元のまま。結果は `_upstream` の木と、サブモジュールの 2 か所を除いて同一(Ajax Control Toolkit の木も同一)。
- `patches/` と、パッチから作る `setup-fork.ps1` は消した。`frameworkoncore.slnx`、`pack-frameworkoncore.ps1`、`publish-frameworkoncore.ps1`(リリースの説明に、上流のどのコミットから取り込んだかを subtree の記録から書く)、テストのプロジェクト参照、`run-tests-linux.ps1`、リソースの生成スクリプトは `FrameworkOnCore.Runtime/` を指す。`pack-frameworkoncore.ps1 -Build All` は、`lib/WebFormsForCore.Build`(ビルドの出力で Git 管理外)が無ければ先にビルドする。
- 上流の更新の取り込み: `git subtree pull --prefix=FrameworkOnCore.Runtime https://github.com/webformsforcore/WebFormsForCore.git main`(Ajax Control Toolkit は `--prefix=FrameworkOnCore.Runtime/src/WebFormsForCore.AjaxControlToolkit` とそのリポジトリ)。上流に返すときは、`FrameworkOnCore.Runtime/` の変更のコミットだけを `git subtree split` で取り出せる(フォークの変更と変換器の変更はコミットを分ける)。
- GitHub Actions(`.github/workflows/frameworkoncore-packages.yml`): `FrameworkOnCore.Runtime/` などを変える push で、Windows でパッケージを作り、.NET SDK のコンテナで Linux のテストを走らせ、版の Release がまだ無ければ置く。Windows のテストはゴールデンがこの開発機のもの(日本語の Windows、GDI+ とフォント、IIS、SQL Server Express)なので Actions では走らせない。
- 確認: `FrameworkOnCore.Runtime/` から作ったパッケージ 18 個は、それまでの `_feed` のものと DLL の一覧が同じで、大きさの差は 1 KiB 未満(ビルドのパスの長さ)。テスト Windows 2,823 件・Linux 2,804 件(SQL Server の要る 19 件はスキップ)がすべて緑。開発機の `_upstream/` はもう使わない(Git 管理外のまま残っている。消してよい)。

### System.Web.Mobile(モバイル コントロール)の移植(2026-10-01、`1.6.5-w2l.7`)

ASP.NET 1.1 のモバイル コントロール(referencesource の System.Web.Mobile、MIT)。上流にプロジェクト(`src/WebFormsForCore.Web.Mobile`)があり、ビルドはできたが動かなかった。それを直して FrameworkOnCore.Web.Mobile として配る。アセンブリは System.Web.Mobile 10.0.29.0、公開キーは .NET Framework と同じ(b03f5f7f11d50a3a)、net10.0(net8.0 は 2026-10-11 に外した)。

- **コーパスでの使われ方(必須カバー一覧)**: nop(`System.Web.Mobile` の参照と、管理画面など 11 ファイルの `using System.Web.UI.MobileControls;`)、mojo・dnn・n2(参照だけ)。コントロールや MobileCapabilities を呼ぶ箇所は無い。必要なのは「参照が解決し、名前空間がある」ことで、移植版はそれを満たす(以前は `noAnswer` で参照を外していた)。コーパスに使い手が無いので、機能は新旧比較とサンプルのサイトで確かめた。
- **見つけて直したこと(`FrameworkOnCore.Runtime/`)**:
  - すべてのモバイル ページが 500(`Control 'System.Web.UI.Control' is not registered with device`): 上流が `IndividualDeviceConfig` のアダプターの登録(`FactoryGenerator`)を `#if NETFRAMEWORK` で外していた。戻した。
  - ビュー ステートの MAC で `DllNotFoundException: webengine4.dll`: MobilePage は `LosFormatter(true, macKey)` で旧来の MAC を使い、それが `MachineKeySection` のネイティブ関数(webengine4.dll の内側・外側のキー、HMAC、ハッシュ)を呼ぶ。マネージドで書いた。アルゴリズムは .NET Framework のネイティブを P/Invoke して測った: 出力の長さで決まる(16=MD5、20=SHA1、32=SHA256、48=SHA384、64=SHA512、それ以外は E_INVALIDARG)、HMAC は H(外側 + H(内側 + データ + 修飾子))。最初に SHA1 だけで書いて、HMACSHA256(既定の validation)の MAC が合わなかったので、長さで選ぶようにした。
  - 同じ webengine4.dll の旧形式のフォーム認証チケット(`CookieAuthConstructTicket`/`ParseTicket`、appSettings `aspnet:UseLegacyFormsAuthenticationTicketCompatibility=true` のとき)もマネージドにした(`WebFormsForCore/Security/LegacyFormsAuthenticationTicket.cs`)。ネイティブと 20,000 件の乱数のチケットで一致(エラーの種類 E_INVALIDARG / E_UNEXPECTED も)。
  - machine.config の `deviceFilters` が IgnoreSection の宣言のままで、web.config の `<deviceFilters>` が使えなかった(`MobileCapabilities.HasCapability` が InvalidCast)。`mobileControls` と同じく、System.Web.Mobile.dll があるときは `DeviceFiltersSection`(`@Mobile`)。
  - デザイン モード(要求の外。.NET Framework は Visual Studio のデザイナーの部品を使う): `MobilePage.Device` が NotImplementedException だったのを `DesignerCapabilities` に、Form・Panel の `DesignerAdapter` 属性を戻した。デザイナーの公開 API のうちデザイナー無しで成り立つもの(`IMobileDesigner`、`IMobileWebFormServices`、`MobileResource`)も入れ、API は .NET Framework と完全に一致(1,819 のドキュメント ID、欠け・余分 0)。デザイン時のアダプター(`System.Web.UI.Design.MobileControls.Adapters`。MSHTML でページを表示するデザイナーの一部)は移植しない。
- **類似の問題の調査で直したこと**:
  - Web アプリの外(アプリのパスが無い: ライブラリのテスト、System.Web のキャッシュを使うコンソール)で System.Web の構成が `Path.Combine(null, ...)` で失敗していた(`HttpConfigurationSystem.MachineConfigurationDirectory`)。.NET Framework はそこでも machine.config を読むので、クライアント構成の machine.config(`ClientConfigurationHost.MachineConfigFilePath`)のフォルダーにした。ほかの `AppDomainAppPath` の使い方は .NET Framework でもアプリの中だけのもので、該当しない。
  - Linux でクライアント構成(`ConfigurationManager`)が初期化できなかった: 実行ファイルの URI `file:///app/App.dll` から先頭の `/` を落とし、`/` を `\` にしていた(`ClientConfigPaths`)。Web アプリは HttpConfigurationSystem を使うので影響しなかった。
  - 古いワーカーが残ってポートを握り、テストが前のビルドに当たっていた(スクリプトがスーパーバイザーだけを止めていた): ワーカーはスーパーバイザーが終わったら終わる(`WebFormsProcess`。Windows はプロセスを待ち、Linux は `getppid` を見る。Linux のコンテナでスーパーバイザーを kill -9 してワーカーが終わるのを確認)。スクリプト(run-sample、probe-requests、probe-corpora、chart-probe、sample-parity)はプロセスの木ごと止める(`taskkill /T`)。
  - 変換器: 配置が libgdiplus を入れる判定(`DeployWriter.DrawingUsers`)で System.Web.Mobile を描く側に数えていた(System.Drawing の参照は Color などの型。描くのはデザイナーのフォント一覧だけ)。FrameworkOnCore 自身のものに加えた(`DeployWriterTests`)。
- **変換器**: `rules/packages.json` で System.Web.Mobile → FrameworkOnCore.Web.Mobile(`noAnswer` から外した。部品 `mobile-controls` の選択肢 `port`(既定)/ `none`)。web.config の `<assemblies>`・`<pages><controls>` の登録はそのまま残る。ParityTest は `name` だけを持つ入力(モバイル コントロールは id を出さない)も対象にできるようにした。
- **全 API の新旧比較**(`tests/MobileParity`): 1,495 ケース・2,293 行。見本はすべてのコントロールを持つモバイル ページと、HTML 3.2 の固定の値の MobileCapabilities。.NET では Web アプリと同じ machine.config を書いて読む(.NET Framework は machine.config に mobileControls・deviceFilters を持つ)。乱数で決まる値(GUID、`__ufps`、WML の短い名前)は形で書く。呼ばないのはデリゲートの BeginInvoke/EndInvoke だけ。
  - **既知の差**(`known-differences.json`、Windows・Linux 共通の 4 項目): デザイン モードのコントロールのアダプター(.NET Framework はデザイナーのもの)、値型のアンボックスの例外のメッセージ(.NET は型名を書く)、`DesignerAdapterAttribute` の型名の中のアセンブリの版、XHTML のスタイル シートのキャッシュ キー(`String.GetHashCode` を .NET はプロセスごとに乱数化する。キーはプロセスの中だけで使う)。
- **テスト マトリクス**:

  | 領域 | 正常系 | 境界(null・空・不正な状態) | 異常系 | サイト(IIS と比較) |
  |---|---|---|---|---|
  | コントロール(Form、Panel、Label、TextBox、TextView、Command、Link、List、SelectionList、ObjectList、Image、PhoneCall、Calendar、AdRotator、検証、StyleSheet、DeviceSpecific) | 全メンバー | 見本の引数(null・空・"Abc 123")とビュー ステートの型違い | 例外の型(と既知の差以外はメッセージ) | 表示・必須の検証・選択・コマンド・リストの項目のコマンド・フォームの移動・ObjectList の詳細・Panel |
  | アダプター(HTML、cHTML、WML、UP.Browser、XHTML) | 全メンバー | 同上 | 同上 | HTML のみ(ブラウザーは html32。ほかのマークアップの端末は無いので API のケースだけ) |
  | MobileCapabilities・デバイス フィルター | 全メンバー | 無いフィルター名 | ArgumentOutOfRange | 値の比較のフィルターとメソッドのフィルター(`deviceFilters`)、DeviceSpecific の Choice |
  | 構成(mobileControls、deviceFilters) | machine.config の既定 | — | — | 上と同じ |
  | ビュー ステートの旧来の MAC(LosFormatter) | — | — | — | すべてのポストバック(Windows・Linux) |
  | MobileFormsAuthentication・CookielessData | 要求の外の動き(例外) | — | 同左 | しない(コーパスが使わない。旧形式のチケットはネイティブとの比較で確認) |
  | デザイナー | 公開 API の有無 | — | — | 移植しない |

- **サイトでの確認**(`samples/MobileProbe`、`sample-parity.ps1`): 2 つのフォームのモバイル ページ(上の表の機能とデバイス フィルター)。IIS(.NET Framework 4.8)で採った 7 スナップショットと比べる。変換は変換器(既定の選択)、Linux は変換器が書いた Dockerfile のイメージ(libgdiplus なし)。**Windows 7/7・Linux 7/7 一致**。
- **検証**: テスト Windows 4,324 件・Linux 4,305 件(SQL Server の要る 19 件はスキップ)がすべて緑。be/wt は Windows で変換・ビルドしてトップが 200、Linux で be 5/5・wt 5/8(以前と同じ既知の差のみ)。
- **残り**: コントロールを使うコーパスが無いので、コーパスでの実地検証はしていない(nop・mojo・dnn・n2 は参照だけ。CLAUDE.md によりコーパスの検証は be/wt のみ)。WML・cHTML・XHTML の端末向けの表示はサイトで確かめていない(API のケースだけ)。デザイン時のアダプターは無い。

### 全コーパスの試験(2026-10-01、System.Web.Mobile の移植の後、ユーザーの指示)

10 本(Web Forms 8、MVC 2)をすべて変換し直し、Windows(`.\SQLEXPRESS`)と Linux(Linux は配置の既定どおり大文字小文字のライブラリあり。nop・nop390 は変換器が書いた Dockerfile のイメージ、ほかは `run-linux-site.ps1`/`run-linux.ps1`。SQL Server はコンテナ)で動かした。DB は空から作り直し、アプリのインストーラーを通した。

| コーパス | Windows | Linux |
|---|---|---|
| be | 5/5(IIS の正解と比較。`verify-windows.ps1`) | 5/5 |
| wt | 6/8(既知の丸めの差 2) | 5/8(同じ 2 と error-page。以前と同じ) |
| mvcmovie | 11/12(既知の言語パックの差) | 11/12 |
| mojo | トップ 200 | 空の DB からセットアップ → トップ・ログイン(`/secure/LOGIN.aspx` も)・サイトマップ 200、管理画面はログインへ |
| yaf | `/` は Web API 2 の FieldAccessException、インストーラーは ServiceStack のスタブ(`PclExport`) | インストーラー・`/` とも ServiceStack のスタブ(以前と同じ) |
| dnn | 空の DB からインストール完了、トップ・`/Login`・`/Terms`・`/Privacy` 200 | 同じ。ページの資源 21 件すべて 200 |
| n2 | インストーラー完了(管理者 → テーブル → サンプル)、トップがコンテンツ付き、`/N2/` はログインへ 302 | 同じ |
| imis | ログインし、主な画面 7 つが 200 | 同じ(コンテナのログにログインの記録) |
| nop(1.90) | インストーラー(サンプルデータ)完了、店頭 14 画面 200、商品のサムネイルを生成して配信、ログインが通る。商品の詳細・アカウント・管理画面は 500(下) | 同じ。**サムネイルは libgdiplus で生成**(以前の記録ではトップが System.Drawing で 500) |
| nop390 | インストーラー(サンプルデータ)完了、店頭 19 画面・商品画像・カートへの追加(AJAX)・管理画面 7 画面と一覧の JSON が 200 | 同じ |

サンプル: MobileProbe 7/7、ChartProbe 27 行一致、RuntimeProbe(下の `Paths.aspx`)1/1。いずれも Windows・Linux。テスト Windows 4,327 件・Linux 4,308 件(SQL Server の要る 19 件はスキップ)がすべて緑。System.Web.Mobile を参照するコーパス(nop の `using System.Web.UI.MobileControls;` 11 ファイル、mojo・dnn・n2 の参照)は移植版のパッケージを参照してそのままビルドできる(以前はスタブで using を外していた)。

見つけて直したこと:
- **変換器が 3 本で異常終了していた**(AssemblyRetargeter。DLL の参照の付け替え。`AssemblyRetargeterTests` に再現テスト 3 件、直す前はどれも失敗):
  - mojo: スタックオーバーフロー。配置済みサイトの bin にある .NET Framework 用のパッケージのファサード(System.Security.AccessControl 6.0.0.1 など)は型を mscorlib へ転送し、.NET の mscorlib はそれを System.Security.AccessControl へ転送し返す。アプリの同名のアセンブリを版によらず .NET のものより優先していたため、Cecil の型の解決が往復し続けた。.NET のホストと同じく、.NET にもあるアセンブリは版の高いほうを使う。
  - dnn: 「別のプロセスが使用中」。解決のために読んだ DLL のファイルを開いたままにしていて、その DLL をその場で書き換えられなかった(読むのはメモリーに)。
  - imis: 書き出しで AssemblyResolutionException。定数の列挙型(ReportViewer の `[WindowsBase]System.IO.Packaging.*`。.NET の WindowsBase は .NET に無い System.IO.Packaging へ転送する)を解決できない。書き出しはメモリーに行ってから置き換える(その場の書き換えで失敗すると元の DLL が空になる作りだった)。書けない DLL はそのまま残し、レポートに「not rewritten」と出す。
  - 解析(TargetApis)の同様の判定は .NET の参照アセンブリとパッケージだけを見ていて、深さの上限もあり、該当しない。
- **アプリの物理パスの末尾に区切りが無かった**(`FrameworkOnCore.Runtime/`、AspNetCoreHost): .NET Framework の `Request.PhysicalApplicationPath`・`HttpRuntime.AppDomainAppPath`・`APPL_PHYSICAL_PATH` は区切りで終わる(`C:\inetpub\app\`)。nop は `PhysicalApplicationPath + "images\\thumbs"` でサムネイルの場所を作り、Windows ではサイトの外の `...\siteimages\thumbs` に書いて画像が 404、Linux では `/appimages` に書けず、商品の画像のある画面(トップ・カテゴリーなど 6 画面)が 500 だった。`samples/RuntimeProbe/Paths.aspx` を IIS で記録し(7 項目すべて区切りで終わる)、直す前は Windows で不一致、直した後は Windows・Linux とも一致。同じ書き方はコーパスに 11 か所(nop のファビコン・テーマの一覧・PDF のロゴ、dnn のプロバイダーのパス、be の web.config)。`HostingEnvironment.ApplicationPhysicalPath`(SimpleApplicationHost)は前から区切り付きだった。
- テスト: `CompatTests` の BeginInvoke のテストが全件の実行中に 1 回失敗した(単独では 5 回とも成功)。スレッドプールの完了を 5 秒待っていた。30 秒にした。

検証の手順で気をつけること:
- DNN のインストーラーは `Install\*.aspx` を消し、n2 は SQLite の DB、nop は `ConnectionStrings.config`、nop390 は `App_Data` を書き換える。Windows と Linux の両方で入れるときは、変換直後のサイトを写しておいて戻す。
- 設定を書き換えた直後(セットアップ・インストーラー)はアプリが再起動し、その間の要求は応答が無い(000・503)。続けて要求すれば答える。
- nopCommerce 3.90 は curl の User-Agent を検索エンジンとみなし、カートとログインを断る。ブラウザーの User-Agent で要求する。
- mvcmovie の Windows の比較は、EF の初期データのある状態から始める(`verify-windows.ps1` が DB を消してから起動する)。

### Ajax Control Toolkit の ToolkitScriptManager(2026-10-02、`1.6.5-w2l.8`)

nopCommerce 1.90 は Ajax Control Toolkit 4.1 の `<ajaxToolkit:ToolkitScriptManager>` を管理画面のマスター 2 つ、商品のテンプレート 2 つ、アカウントなど 10 か所で使う。Toolkit 15.1 がこれを削除した(コントロールは ScriptManager で動く)ので、FrameworkOnCore の Toolkit(15.1 以降)ではページが Parser Error になり、変換器は designer.cs の宣言 8 つをスタブとして外していた。

- **方針**: マークアップやソースを書き換えず、FrameworkOnCore の Toolkit に `ToolkitScriptManager`(`Compat/ToolkitScriptManager`、ScriptManager の派生)を戻した。アプリのマークアップ・型の宣言・DLL・tagMapping(mojo の MyPage は ScriptManager を ToolkitScriptManager に写す)がそのまま通る。公開 API は 4.1(nop が使う 4.1.40412 の DLL をリフレクションで確かめた)と同じ: `CombineScripts`(既定 true)、`CombineScriptsHandlerUrl`([UrlProperty])、`OutputCombinedScriptFile(HttpContext)`(静的)、protected の `QuoteString`・`AppendCharAsUnicode`・`WebResourceRegex`・`HiddenFieldName`(ClientID + "_HiddenField")、[Themeable(true)]。`QuoteString`・`AppendCharAsUnicode` は 4.1 の実物と同じ結果(8 種の文字列(null、制御文字、< > '、非 ASCII、サロゲートを含む)と 3 文字で比べた。引用符を付けない、null は空、< > ' と制御文字は \uXXXX)。`OutputCombinedScriptFile` は、結合を要求しない要求に 4.1 と同じく false(null は NullReferenceException)。
- **しないこと**: スクリプトの結合(`CombineScripts`)。4.1 は Toolkit のスクリプトを 1 つの要求にまとめた。ここでは ScriptManager どおり 1 つずつ参照する(スクリプトは同じで、要求の数だけが違う)。結合されたスクリプトを求める要求(`_TSM_CombinedScripts_`)にも答えない(このページはそれを出さない)。
- **サイトでの確認**(`samples/ToolkitProbe`、`sample-parity.ps1`): nop と同じ宣言の ToolkitScriptManager、その型のフィールド、UpdatePanel の非同期ポストバック(`IsInAsyncPostBack`)、CollapsiblePanelExtender。NuGet の AjaxControlToolkit 4.1.60919 で IIS(.NET Framework 4.8)の 4 スナップショットを採った(`sample-parity.ps1 -Record` が packages.config のパッケージを nuget.org から取るようにした)。直す前は変換がスタブを作り、ページが Parser Error。直した後は変換のビルドが 1 回で通り、**Windows・Linux とも 4/4 一致**。
- **nop**(Windows・Linux、空の DB からインストール): 変換のスタブ 20 → 12。商品の詳細・アカウント・管理画面(ダッシュボード、商品・注文・顧客の一覧、全体設定、売上・顧客のレポート、商品の編集)が 200。管理画面のタブ(TabContainer、9・13 枚)が描かれ、ページのスクリプト(ScriptResource・WebResource)はすべて 200、顧客のレポートのグラフ(System.Web.DataVisualization の移植)3 枚が PNG で返る。商品のページから「カートに入れる」ポストバックでカートに入り、管理画面で商品名を保存すると DB が更新される。
- **類似の問題の調査**: コーパスが使う Toolkit の型(nop・imis・mojo のマークアップとコード: CalendarExtender、CollapsiblePanelExtender、ConfirmButtonExtender、FilteredTextBoxExtender、MaskedEditExtender、ModalPopupExtender、Rating、TabContainer・TabPanel、ValidatorCalloutExtender、BarChart・LineChart)はすべて今の Toolkit にある。変換のレポートに残る Toolkit の型は mojo の `SanitizerProviders.dll` の `[AjaxControlToolkit]AjaxControlToolkit.Sanitizer.SanitizerProvider`(15.1 で HtmlEditor.Sanitizer に変わった)だけで、mojo の構成では使われていない(Toolkit のエディターの設定はコメントアウト、ソースからの参照も無い)。
- **検証**: テスト Windows 4,327 件・Linux 4,308 件(SQL Server の要る 19 件はスキップ)がすべて緑。ToolkitProbe は protected の 2 つを足した後に Windows でもう一度 4/4(Linux はその前の版で 4/4。足したのは振る舞いの無いメンバー)。be Windows 5/5・Linux 5/5、wt Windows 6/8・Linux 5/8、imis(Toolkit を使う)Windows・Linux ともログインと主な画面 7 つが 200、mojo Windows トップ 200(いずれも以前と同じ)。

残り(以前からのもの): nop の `ToolkitScriptManager`(新しい Ajax Control Toolkit に無い。管理画面のマスター、商品のテンプレート、アカウントなど。**2026-10-02 に対応、下**)。yaf の Web API 2(AspNetWebStack の移植)と ServiceStack のスタブ。wt・mvcmovie の既知の差。

### エスケープされた文字を含む URL(2026-10-02、`1.6.5-w2l.9`)

別の PC の Studio で WingtipToys の商品ページ `/Product/Fast%20Car`(ルーティング)が 400 になった。Windows でも Linux でも、空白などをエスケープした URL はすべて 400 になっていた。

- **原因**: ホストのワーカー要求(`AspNetCoreWorkerRequest`)が、パスを `$"{Request.PathBase}{Request.Path}"` で組み立てていた。ASP.NET Core の `PathString` は、文字列にするとエスケープされた形(`ToUriComponent`)になる。そのため `/Product/Fast%20Car` のまま悪いパスの検査(`%` を含む)に当たり、400 になった。`GetUriPath`(Request.Path)も同じ組み立てで、エスケープされたまま `/` が重なっていた。
- **修正**: パス、`GetUriPath`、`GetRawUrl` を IIS と同じにした。
  - パスと `GetUriPath` はデコード済み(`PathString.Value`)。
  - `GetRawUrl` は、パスがデコード済みで、クエリ文字列は送られたまま。IIS の RawUrl は `/Urls.aspx/a b?q=x%20y` だった。
- **サイトでの確認**: `samples/RuntimeProbe` に `Urls.aspx` を足し、`/Urls.aspx/a%20b?q=x%20y` で次を IIS と比べた。
  - 比べた値: Request.Path、FilePath、PathInfo、CurrentExecutionFilePath、AppRelativeCurrentExecutionFilePath、RawUrl、Url.AbsolutePath、Url.PathAndQuery、QueryString。
  - 直す前の版では 400 になる。**Windows・Linux とも 2/2 一致**。
- **WingtipToys**: `/Product/Fast%20Car`・`/Product/Paper%20Boat` が 200 になり、その商品を表示した。
- **類似の問題の調査**: ホストの中で `PathString` を文字列に埋め込んでいるのは、ほかにデバッグ出力だけだった。
- **検証**(以前と同じ結果):
  - be: Windows 5/5、Linux 5/5
  - wt: Windows 6/8、Linux 5/8(丸めの差 2 件と、Linux の error-page は検証環境の差)

### リンクの巡回、URL の形の試験、ホストの見直し(2026-10-02、`1.6.5-w2l.10`)

上の不具合がテストで見つからなかった理由は 2 つあった。

- ホスト(リクエストを System.Web に渡す部分)を通る試験は、サイトのシナリオだけだった。
- シナリオの URL は手で書いた数個だけで(wt 8、be 5)、`%` を含む URL は 1 つも無かった。

そこで次の 3 つを入れた。

**1. リンクの巡回**(`tools/FrameworkOnCore.ParityTest`、シナリオの `crawl`)

- シナリオの手順のあと、各ページの `<a href>` を同じサイトの中で 2 段までたどる。リダイレクトは追わず、その応答も 1 件として記録する(最大 150 件)。
- 元のアプリ(IIS)での各 URL の応答(状態コードとリダイレクト先)を正解データの `Links` に記録する。変換後のアプリでは、同じ URL を同じ形で要求して比べる。
- 記録はスナップショットを残したままリンクだけ採れる。
  - コマンド: `record-links`
  - スクリプト: `record-webforms-golden.ps1 -LinksOnly`、`sample-parity.ps1 -Record -LinksOnly`
- 対象: be(17 件)、wt(64 件)、サンプル 7 つ。ログオフのリンクは `exclude` で除いた。
- 巡回をすぐに確かめられた。
  - **旧版の wt(w2l.8)では、商品ページ 14 件が 400、`/` が 301 になり、差分として出た。**
  - 新しい差分も 1 つ見つかった。wt の `/` が IIS では 200、変換後は 301 だった(2. の RawUrl)。
- 検証ツールのビルドは、以前は無いときだけだった(古いツールで検証していた)。毎回(差分)ビルドするようにした。
- `sample-parity.ps1 -Record` は、Visual Studio の MSBuild があればそれでビルドする。古いサンプルは C# 6 以降で書かれていて、.NET Framework の MSBuild(C# 5)ではビルドできない。サンプル 4 つには `OutputPath` が無かったので足した。

**2. URL の形の試験**(`samples/RuntimeProbe`、IIS で記録して比べる)

- 値を比べるページ:
  - `Urls.aspx`: Request.Path、FilePath、PathInfo、RawUrl、Url、QueryString、RouteData
  - `ServerVars.aspx`: GET とポストバックのメソッド、フォームの値、サーバー変数 20 個
  - `Sub/`・`Docs/`: 既定のドキュメント(`Docs/` は web.config の defaultDocument)
  - `Global.asax`: ページのルート `Item/{name}`
- 比べた URL の形: 空白、日本語、大文字、繰り返しのクエリー、既定のドキュメント、ルート、POST。
- 応答だけを比べる URL(`crawl.urls`、25 個):
  - `App_Data`・`bin`(小文字も、下のフォルダーも)
  - `Web.config`・`.csproj`・`.cs`・`.asax`
  - `/` の無いフォルダー
  - 無いページ
  - `%2F`・`%25`・`+`・`:`・`;`・`<`、クエリーの `<script>`
- 結果: Windows・Linux とも **10/10 スナップショット、34/34 リンクが一致**。

**3. ホストの見直し**(`AspNetCoreWorkerRequest`・`AspNetCoreHost` を IIS の IIS7WorkerRequest と照らし、2. で確かめた)。直したもの:

- **App_Data などが配信されていた(セキュリティ)**
  - 原因: IIS の隠しセグメントの一覧が、アンダースコアの抜けた名前(`/appdata`)だった。拒否されていたのは bin だけ。
  - 影響: be の `App_Data/users.xml`(ユーザーとパスワードのハッシュ)と `settings.xml` が 200 で取れた。
  - 修正: IIS と同じ 7 つ(bin、App_Code、App_Data ほか)を、URL のどのセグメントでも、大文字小文字を区別せずに 404 にした。
- **IIS が拒否する拡張子のほとんどが配信されていた**
  - 拒否していたのは一部だけで、`.mdf`・`.ldf`・`.csproj`・`.resources` などは配信されていた。
  - IIS の既定の拒否一覧(applicationHost.config)を 404 にした。
- **全リクエストがプロセスの Windows ユーザーで認証済みになっていた**
  - サーバー変数の名前も、アンダースコアが抜けていた(`ALLRAW`・`SERVERPROTOCOL`・`LOGONUSER`・`AUTHTYPE`)。名前を直すと、隠れていた処理が動いた。その処理は `LOGON_USER`・`AUTH_TYPE` にプロセスのユーザーと NTLM を返していた。
  - そのため Windows 認証モジュールが、全リクエストをそのユーザーで認証済みにしていた(認証モードは既定で Windows)。
  - IIS の匿名認証と同じく空にした。
- **サーバー変数が IIS と違っていた**
  - 名前の誤りで、ほぼすべてが `""` だった。
  - `HTTPS` は IIS では `off` で、`HTTPS != "off"` と判定するアプリには、全リクエストが HTTPS に見えていた。
  - IIS と同じ値にした: `HTTPS`、`SERVER_PROTOCOL`、`REMOTE_PORT`、`GATEWAY_INTERFACE`、`ALL_RAW`、`INSTANCE_ID`・`APPL_MD_PATH`、証明書関係(空)。知らない名前は null にした。
- **RawUrl が、既定のドキュメントに書き換えたあとのパスになっていた**
  - IIS では、クライアントが要求した URL(`/`)。
  - FriendlyUrls は `.aspx` で終わる RawUrl を拡張子なしへリダイレクトするので、wt の `/` が `/Default` への 301 になっていた。
  - 要求の生のターゲット(`IHttpRequestFeature.RawTarget`)から作るようにした。パスはデコードし、クエリーは送られたまま。
- **PathInfo の切り方が違っていた**
  - 最後の `.` のあとの最後の `/` で切っていたので、`/Page.aspx/x/y` が `/Page.aspx/x` の 404 になっていた。
  - 拡張子でハンドラーが決まる最初のセグメント(またはファイル)で切るようにした。
- **エスケープされた `/` と、二重のエスケープの扱いが違っていた**
  - Kestrel は `%2F` を Request.Path に残すので、400 になっていた。IIS はデコードする。
  - IIS が 404 にする二重のエスケープ(`+`・`%2520`)を配信していた。
- **既定のドキュメントが IIS と違っていた**
  - 名前の一覧と順番を IIS と同じにした(`iisstart.htm` が `iistart.htm` だった)。
  - ファイルは大文字小文字を区別せずに探し、IIS と同じく一覧の名前で出す(Request.Path は `/Sub/default.aspx`)。
  - web.config の `<defaultDocument>`(enabled、clear・remove・add)を読む。以前は読んでいなかった。足した名前は IIS の一覧より前に来る(IIS で確かめた)。
  - `/` の無いフォルダーは、IIS と同じく 301 で `/` 付きへリダイレクトする。
- 変換器の Program.cs は ASP.NET Core の `UseDefaultFiles` を使わず(IIS に無い Index.aspx を含んでいた)、ホストに任せる。

**検証**

- テスト: Windows 4,327 件がすべて緑。
- サンプル 7 つ: Windows・Linux とも全スナップショットと全リンクが一致。
- be: Windows・Linux とも 5/5、リンク 17/17。
- wt: Windows 6/8、Linux 5/8(以前と同じ差)、リンク 64/64。

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

## 新しい変換器 FrameworkOnCore(2026-09-27)

`src/FrameworkOnCore.Converter`(C#)。convert-project.ps1 の変換規則を移植し(規則は `rules/packages.json`)、ビルドエラーを自動で処理する層を加えた。

    .\experiments\wf4c\convert-corpora.ps1          # 7 本を変換してビルド(レポートは <out>\CONVERSION-REPORT.md)
    .\experiments\wf4c\probe-corpora.ps1            # 起動して "/" を開く

ビルドエラーの自動処理は、Roslyn でエラーの位置を構文木上で特定し、手を入れる範囲をできるだけ小さくする。

| エラーの場所 | 処理 |
|---|---|
| 名前の衝突(`System.Range` など) | using の別名でアプリ側の型を選ぶ |
| using、属性 | 外す |
| 削除された仮想メンバーの override | `override` を外す |
| メンバーの本体 | 本体だけを `PlatformNotSupportedException` にする(シグネチャは残る) |
| フィールドやプロパティの初期化子 | 初期化子を外す |
| 宣言 | メンバーを外す |
| 上のどれでもない | ファイルを除外する |

- 行ったことはすべてレポートに記録する。
- 互換アセンブリで .NET に無い API を網羅するまでの受け皿である(設計書 §8)。
- SYSLIB の警告(存在するが実行時に例外になるメンバー)も、レポートに記録する。

| コーパス | ビルド | 実行 |
|---|---|---|
| be | 成功(手動の除外なし。自動処理は以前の手動除外と同じ 8 ファイル) | Windows 5/5 |
| wt | 成功 | Windows 6/8(既知の丸めの差) |
| mojo | 成功。元のビルドはソリューションのビルド(`--build-original`) | **トップページが表示される**(`Home - mojoPortal`。DB は `.\SQLEXPRESS` の `mojo_w2l`)。**Linux でも**、空の DB からセットアップ画面がスキーマ(105 テーブル)を作り、トップページ・ログイン・サイトマップが 200(`run-linux-site.ps1`、SQL Server のコンテナ) |
| yaf | 成功 | `FieldAccessException`。アプリが Web API 2 の `HttpControllerRouteHandler._instance`(static readonly)をリフレクションで書き換えていて、.NET は型の初期化後の書き換えを禁止している。Web API 2 を DLL のまま使う限り直せないので、AspNetWebStack の移植が要る |
| dnn | 成功。元のビルドは DNN 自身の Cake ビルド(`--build-original`)。VB の DotNetNuke.WebUtility は配置済みサイトの .NET Framework の DLL をそのまま参照する(2026-09-28 から VB のプロジェクトも変換する。その後 Windows・Linux とも再検証した) | Windows: **インストール(`Install.aspx?mode=install`)が完了し、トップページが表示される**(`Home`。DB は `.\SQLEXPRESS` の `dnn_w2l`。`dnn-cycle.ps1` で DB の作成から通す)。**Linux でも**、空の DB からインストールが完了し(サイトの作成、スキンなどのモジュールの導入)、トップページ・`/Login`・`/Terms` が 200、ページの CSS・JS・画像 21 件がすべて 200、host でのログインが通る(`run-linux-site.ps1`、SQL Server のコンテナ。下の「Linux で動かすための書き換え」) |
| imis | 成功(VB のプロジェクト 5 本)。元のビルドはソリューションのビルド(構成 `DemoRelease`) | Windows・Linux とも、**デモの DB でログインでき、主な画面(ホーム、世帯・被保険者・保険・請求・ユーザーの検索、レポート)が 200**(`imis-login.ps1`。下の「VB のプロジェクト」) |
| n2 | 成功。元のビルドはソリューションのビルドと、リポジトリのセットアップ手順(`--original-step build\n2.proj;Templates-PrepareDependencies`) | インストーラーが表示される(`Install N2`)。Linux でも同じ(フォーク 0021 の後)。Windows: **SQLite(`App_Data\n2.sqlite.db`)で、空の DB からインストールが完了し(管理者のパスワード → テーブルの作成 → サンプルのコンテンツの取り込み。`n2-install.ps1`)、トップページがコンテンツ付きで表示される**(VB のページ。フォーク 0025)。**Linux でも同じ**(`run-linux-site.ps1 -Keep` の後 `n2-install.ps1 -Port 5098 -Running`)。SQLite は 1.0.119 に上げる(下) |

### 元のビルドと配置済みサイト

アプリの構成(どの DLL がサイトに置かれるか)は、元のビルドが配置したサイトから取る(`--site <フォルダー>`)。実際の移行では、IIS のサーバーでサイトを置いているフォルダーがそれに当たる。

- `--build-original [ターゲット]`: リポジトリ自身のビルドスクリプトで、コピー(`<out>.original`)をビルドする。
  - Cake Frosting(Cake.Frosting を参照する C# プロジェクト)は `dotnet run --project`、Cake スクリプト(`build.cake`)は Cake ツールで実行する。ターゲットの指定が無ければ既定のターゲット。
  - 足りない道具は、キャッシュ(`%LOCALAPPDATA%\FrameworkOnCore\tools`)に用意する。global.json の .NET SDK(rollForward を見て、インストール済みのもので足りなければ dotnet-install)、package.json の `packageManager`(corepack。Node.js 25 以降は同梱されない)。Node.js が無ければ、package.json のあるリポジトリには Node.js(.nvmrc・.node-version・package.json の engines.node の版、無ければ最新の LTS。この OS とプロセッサーの nodejs.org のアーカイブ)。
  - git は長いパス(260 文字超)も扱う(`core.longpaths`。コピーのリポジトリにも、ビルドの中の git にも)。コピーのリポジトリは、割り当てたドライブの側で作る。
  - 無い道具(npm など)は、変換器が落ちずに、エラーとして報告する。
  - キャッシュに用意した道具(dotnet、node、npm など)は、キャッシュのものを起動する(ビルドの PATH の先頭にも置く)。Windows は起動するプログラムを変換器自身の PATH で探すため、マシン全体の dotnet(global.json の SDK を知らない)が使われていた。
  - 割り当てたドライブは、ビルドが止められて残っていれば(Studio の中止)、次のビルドがそれを使う。
  - git のクローンでないソース(アーカイブ)は、コピーを 1 コミットのリポジトリにし、フォルダー名の版(`Dnn.Platform-9.13.10` → `v9.13.10`)をタグにする。版を git から求めるビルド(GitVersion)のため。
  - 長いパスは親フォルダーにドライブ文字を割り当てて避ける(ドライブのルートにあるリポジトリは GitVersion 5 が見つけられない)。MSBuild の常駐ノードと VBCSCompiler は使わない(コピーのファイルをつかんだまま残る)。
  - 配置済みサイトは「web.config と `bin\<Web プロジェクトのアセンブリ>.dll` があるフォルダー」で探す。Web プロジェクト自身のフォルダーより、ビルドが配置した先を選ぶ。
  - ビルドが配置の後で失敗したとき(DNN はパッケージ作成で、自身の参照解決が落とした DLL を探して止まる)は、配置済みサイトをそのまま使い、失敗をレポートに記録する。
  - ビルドスクリプトが無いリポジトリは、Web プロジェクトを含むソリューション(複数あれば最もプロジェクトが多いもの)を Visual Studio と同じ方法でビルドする(Windows のみ)。
    - Visual Studio (Build Tools) 2022 の MSBuild(無ければインストール方法をレポートに書いて止まる。勝手には入れない)。
    - .NET Framework の参照アセンブリ(全版、nuget.org から)と nuget.exe は、キャッシュに用意する。
    - C# コンパイラーは .NET SDK の最新のもの。構成が選ぶプロジェクトを、参照とソリューションの依存関係の順に 1 本ずつ `BuildingInsideVisualStudio` でビルドし、失敗したものはもう一度ビルドする。
    - リポジトリのセットアップ手順は `--original-step <プロジェクト;ターゲット>` で渡す(n2)。
    - ビルドが割り当てたドライブを指すリンク(n2 のセットアップの `mklink /J`)は、ドライブを外す前にリンク先のコピーに置き換える。
    - 配置済みサイトは Web プロジェクト自身のフォルダーを選ぶ(ビルド後イベントがそこへ配置する)。
  - 以前の `build-original-site.ps1` はこれに置き換えた。
- 変換しないプロジェクト(C# と VB 以外)は、配置済みサイトにあるその DLL を参照する(出力の `.deployed` にコピー)。
- サイトの中のパッケージ(DNN の `Install\Module\*.zip` など。拡張子によらず中身が zip のもの)にある DLL のうち、.NET のビルドで bin を置き換えたものは、パッケージの中も置き換える。インストールされると .NET Framework の DLL が bin に戻るため。
- アプリは作業プロセスの中で、bin のコピーから動く(フォーク 0018。変換器の Program.cs が `WebFormsProcess.RunInWorker` を呼ぶ)。.NET Framework の ASP.NET のシャドウコピーに当たる。bin は書き込めるままで(DNN のインストーラーがモジュールの DLL を置く)、web.config や bin が変わるとアプリが再起動し(フォーク 0015・0016)、新しいコピーから起動し直す。`WEBFORMSFORCORE_SHADOWCOPY=0` なら bin から直接動き、再起動はプロセスの終了(終了コード 75)を監視役(IIS、systemd の `Restart=`、Docker の `--restart`、`supervise.ps1`)が拾う。
- `run-linux-site.ps1`: 変換したサイトを、本番と同じ形(ASP.NET のランタイムイメージにサイトのフォルダーをコピーして起動。ビルドはしない)で Linux のコンテナで動かす。接続文字列は SQL Server のコンテナに向ける。

### 配置(`--deploy`、2026-09-27)

変換器は、Linux での配置のしかたも出力する。`--deploy container|linux|both|none`(既定は both)。

- コンテナ: 出力フォルダーに `Dockerfile` と `.dockerignore`。出力フォルダーをコンテキストに `docker build`。
  - ランタイムは `mcr.microsoft.com/dotnet/aspnet:10.0`。ICU のデータは同じイメージのステージで作る(ICU の版を合わせる)。
  - ユーザー `app`(root ではない)、ポート 8080、`App_Data` はボリューム。
- Linux のマシン: `deploy/linux/install.sh`(root で実行)。
  - ASP.NET Core 10 のランタイムが無ければ入れる。専用ユーザーを作り、`/opt/<アプリ>` に置き、ICU のデータを作り、systemd のサービスとして起動する。
  - 更新は同じコマンド(`App_Data` と設定は残る)。
- 共通(`deploy/start.sh`、`deploy/README.md`):
  - 設定は環境変数で渡す(フォーク 0022。`SQLCONNSTR_<名前>`、`APPSETTING_<キー>`)。コンテナは `-e`、systemd は `/etc/<アプリ>/environment`。ほかの設定ファイルは設定フォルダー(コンテナは `/config`、systemd は `/etc/<アプリ>/config`)に置けばサイトに上書きされる。
  - カルチャは `LANG`(カルチャのプロファイルの既定のカルチャ)と、動かすマシンで作る ICU のデータ(`ICU_DATA`)。
  - 作業プロセス(フォーク 0018)がそのまま動く(再起動はアプリ自身が行う)。
  - リバースプロキシの後ろでは `ASPNETCORE_FORWARDEDHEADERS_ENABLED=true`(フォーク 0023 で URL がクライアントの見たホスト・ポート・スキームになる)。

確認した結果:

| 対象 | 結果 |
|---|---|
| mojo・コンテナ | イメージを作り(34 秒、ICU 74.2)、接続文字列を `APPSETTING_MSSQLConnectionString` だけで渡して、空の DB からセットアップ → トップページ・ログインが 200。ポートを 8089 で公開してもリダイレクト先が正しい。`X-Forwarded-Proto: https` と Host を付けると `https://www.example.com/...` にリダイレクトする |
| mojo・Linux のマシン | systemd の動く Ubuntu 24.04(コンテナで代用)で `install.sh` → ランタイムの導入、ICU のデータ、サービスの起動。`/etc/mojoportal-web/environment` に接続文字列を書いて再起動 → セットアップ・トップページ・ログインが 200。停止で子プロセスも止まる。更新しても `App_Data` と設定は残る |
| be・コンテナ | プロジェクトのフォルダーのまま(サイトの組み立て無し)のイメージで、正解データと比べて 5/5 |

ICU のデータで表せないもの: `AllDateTimePatterns` の 2 番目以降のパターン(5 件)。主な書式(通貨記号、日付・時刻の既定の書式)は元のサーバーと同じ。
- `|DataDirectory|` をコードで組み立てる接続文字列は、.NET の System.Data.SqlClient が拒否する。変換器はこれを報告する(`rules/packages.json` の `sourceNotes`)。ファイルを接続する DB(LocalDB、ユーザーインスタンス)は Windows 専用なので、DB サーバーを使う。
  - 2026-10-02 から、System.Data.SqlClient の移植版が .NET Framework と同じく展開する(「System.Data.SqlClient の移植」)。

DNN を動かす過程で変換器に加えた規則:
- SYSLIB0007(`HashAlgorithm.Create()` などの引数なしの Create): .NET Framework の既定のアルゴリズム(SHA1、HMACSHA1、AES、RSA)に書き換える。型のメンバーは拡張メンバーより先に見つかるので、拡張では補えない。位置はコンパイラーの警告から取る。
- CS0121(アプリの拡張メソッドと、.NET が後から加えた拡張メソッドのあいまいさ。`CollectionExtensions.GetValueOrDefault`): アプリの方を静的メソッドとして明示的に呼ぶ。以前はメンバーをスタブにしていた。
- .NET Framework を含む複数ターゲットの SDK 形式プロジェクト: .NET Framework の方のシンボル(`NETFRAMEWORK`、`NET472`、`…_OR_GREATER`)を定義する。サイトが動かしていたのはそのビルドだから(DNN の ModulePipeline は `#if NET472` でサービスを登録する)。
- `frameworkAssets`(`rules/packages.json`): .NET 向けの資産が .NET Framework 向けと API の違うパッケージは、.NET Framework 向けの DLL を参照する(PetaPoco.Compiled: net45 にだけ `Database(string connectionStringName)` がある)。
- リポジトリのコピーで、プロジェクトがビルドしない bin フォルダー(チェックインされたバイナリ。DNN の `Controls\DotNetNuke.WebControls\bin`)は残す。
- 互換アセンブリ System.Design(`shims/System.Design`): `System.Web.UI.Design` の型を .NET Framework 4.8 のものから生成(継承関係だけ、メンバーなし)。コントロールがデザイナーを属性で指していて、ページのコンパイルが属性を読むと型の読み込みに失敗していた。

### スタブの見直し(2026-09-27)

ビルドエラーの自動処理がスタブにした箇所(本体を `PlatformNotSupportedException` にしたもの)を見直した。

| コーパス | 前 | 後 | 残ったもの |
|---|---|---|---|
| dnn | 24 | 17 | log4net の Windows の偽装(4)、Remoting での受け渡し(`Marshal`・`Disconnect`・`Activator.GetObject`、3)、JwtController(10) |
| n2 | 10 | 3 | `IDataContractSurrogate`、`AssemblyBuilder.Save`、`SqlCommandCacheDependencyEnlister`(別の SqlClient の型) |

残ったものは、どれも .NET では同じ動きにできないもので、コードの移行が要る。
- JwtController: JWT 4.x(System.IdentityModel.Tokens.Jwt)は .NET Framework 専用(System.IdentityModel に依存)。ランタイムの Microsoft.Data.SqlClient 7.1 が JWT 8 を要求するので、4.x のままにはできない(`InMemorySymmetricSecurityKey` などの API の移行)。

減らすために加えたもの:
- 互換アセンブリ `FrameworkOnCore.Compat`(上の「互換アセンブリ」)と、ファサードの公開署名。
- `memberReplacements`(`rules/packages.json`): .NET で削除されたメンバーを、同じ働きのメンバーに書き換える(CS0117 の位置)。`AssemblyBuilderAccess.RunAndSave`・`Save` → `Run`。
- CS9258(C# 14 の `field` キーワード。プロパティのアクセサーの中の `field` という名前が、自動実装のフィールドを指すように変わった): `@field` に書き換える。n2 の `FieldReference.Reference` がプロキシの生成で ArgumentNull になっていた。
  - 同じ問題がページ(実行時にコンパイルされる aspx・ascx・App_Code)に無いことを、6 本のコーパスで確認した。
- 除外するパッケージの判定: .NET 10 の参照パック(`Microsoft.NETCore.App.Ref`、`Microsoft.AspNetCore.App.Ref`)に同名のアセンブリがあるものだけにした。以前は System.* 4.x を一律に外していて、JWT 4.x を黙って落としていた。外したパッケージはレポートに記録する。

### Linux で動かすための書き換え(2026-09-27)

Windows を前提にしたコードは、コンパイルは通るが Linux では動かない。変換器は Roslyn のアナライザー(`src/FrameworkOnCore.Analyzers`)を変換時のビルドに差し込み、コンパイラーの意味モデルで対象を特定して、その位置を書き換える(アナライザーは変換したプロジェクトには残らない)。書き換え先は互換アセンブリ `FrameworkOnCore.Compat` の呼び出しで、Windows では元のソースと同じ動きをする。レポートでは「Linux で動かすために変えたソース」にまとめる。

| 診断 | 対象 | 書き換え | 例 |
|---|---|---|---|
| FOC1001 | パスとして使われる `\` 入りのリテラル。パスかどうかは値の行き先で判定する(ファイル API のパスの引数、パスの名前の変数・メンバーへの格納や結合、パスに対する `IndexOf`・`EndsWith`・`Split`・`Replace`)。正規表現・エスケープ・`Replace('\\', '/')` のように両方を扱うコードは対象外 | `WindowsPath.Native("...")`、文字 `'\\'` は `Path.DirectorySeparatorChar` | n2 `BaseDirectory + "bin\\"`、DNN `string.Format("{0}\\{1}\\", ApplicationMapPath, ...)` |
| FOC1002 | 同じものが定数の中にある | 定数が必須の場所(case・属性・既定値・他の定数)で使われていなければ `static readonly` にする | DNN `glbConfigFolder = "\\Config\\"` |
| FOC1003 | フォルダーに結合する前の区切りの除去(`Path.Combine(root, x.TrimStart('\\', '/'))`) | `WindowsPath.TrimStartRelative`(絶対パスはそのまま。Windows ではドライブ付きなので残る) | DNN `Config.Save` が `/app/app/Config/...` に書こうとした |
| FOC1004 | データ(マニフェスト、DB)から来るパスをファイル API に渡す引数。アプリ自身の他のプロジェクトのメソッドと URL を取るメソッドは対象外 | `WindowsPath.Native(引数)`(区切りと大文字小文字) | DNN のモジュールのマニフェスト `Providers\DataProviders\...`、`resource-skin.zip` と `Resource-Skin.zip` |
| FOC1005 | デリゲートの `BeginInvoke`・`EndInvoke` | `AsyncDelegate`(引数は呼び出し時に評価し、スレッドプールで実行) | DNN のスケジューラー |

ほかに、式のテキストで決まるもの(`rules/packages.json` の `platformReplacements`。ビルドの前に書き換える):
- `PrincipalPolicy.WindowsPrincipal`: Windows 以外では、以後の `Thread.CurrentPrincipal` がすべて例外になる(DNN のスケジューラーが設定し、全リクエストが 500)。
- `WindowsIdentity.GetCurrent().Name`: Windows 以外は例外。`WindowsIdentity.GetCurrent()` だけのときは、Windows 以外では null(Windows のアカウントが無い。YAF はこれを null と比べてから偽装する)。
- `AppDomain.CurrentDomain.RelativeSearchPath`: .NET では常に null。ASP.NET ではアプリのフォルダー(BaseDirectory)の下の `bin`。フォーク 0014 で BaseDirectory をアプリのフォルダーにしたので、YAF がアプリのフォルダーでデータプロバイダーの DLL を探し、見つからなかった(0014 以後の後退)。bin があれば `bin` を返す。
- `Uri.TryCreate`・`Uri.IsWellFormedUriString`・`new Uri(x, UriKind...)`: .NET は Unix で `/Portals/...` を絶対の file URI とみなす(DNN の「絶対 URL か」の判定がすべて真になり、存在しない CSS を登録していた)。dnn・n2・mojo・be で使われている(be だけで 17 か所を書き換えた)。

パッケージ:
- `System.Data.SQLite(.Core)` は 1.0.119 に上げる。1.0.116 より前のネイティブライブラリ(`SQLite.Interop.dll`)は Windows 用だけ(n2 が Linux で DB に接続できなかった)。2.x はネイティブライブラリを含まない。
- 配置済みサイトの .NET Framework 形式のネイティブライブラリ(`bin\x86`、`bin\x64`)は、.NET のビルドが `runtimes\<rid>\native` に同じ名前のものを持つとき削除する(古い版が先に読み込まれていた)。
- QuickIO.NET は互換アセンブリに置き換える(`shimPackages`、上の「互換アセンブリ」)。
- mojo の SQLite 版のデータプロバイダーが使う Mono.Data.Sqlite は OS の `sqlite3` を呼ぶ。SQLite 構成の mojo を Linux で動かすには libsqlite3 が要る(未確認。mojo の既定の構成は SQL Server)。

ルーティングの判定: 拡張子のない URL(DNN の `/Login`)は、Web Forms にすべての要求を渡さないと届かない(IIS では ExtensionlessUrlHandler が ASP.NET に渡す)。Web プロジェクトだけでなく、変換するすべてのプロジェクトのソースでルートの登録(`RouteTable.Routes`)を探すようにした(DNN は DotNetNuke.Web で登録する)。

確認した結果:
- dnn: Linux で空の DB からインストールが完了し、トップページ・ログインのページ・資源がすべて 200。Windows でもインストールが完了し、トップページが 200。
- n2: Linux・Windows とも、空の DB から SQLite でインストールが完了し、トップページがコンテンツ付きで表示される。
- be/wt: 変わらず(be 5/5、wt 6/8)。
- mojo: Windows でトップページ、Linux で空の DB からセットアップ → トップページ・ログインが 200(以前と同じ)。
- yaf: 以前と同じ `FieldAccessException`(Web API 2)。その手前で止まるようになっていた 2 つを直した: 上の RelativeSearchPath(データプロバイダーが見つからない)と、偽装を使うタイマーの処理がスタブ(例外)になり、タイマーの例外でプロセスが終了していたこと(互換アセンブリに `WindowsImpersonationContext` と `WindowsIdentity.Impersonate()` を置いた。Windows では本当に偽装する。dnn の log4net のスタブも 3 件減った)。

mojo・yaf で見つかった誤検出と対応:
- mojo のメニューのアダプター: `item.ValuePath.Replace(menu.PathSeparator, "\\")` は、メニューの項目のパス(ポストバックの引数)で、ファイルのパスではない。名前の規則(Path を含む)に当たっていた。`ValuePath`・`PathSeparator`・`XPath`・`DataPath` は対象外にした。
- yaf の Lucene.Net: `path.IndexOf('\\') < 0` は、どの OS でもバックスラッシュを拒む意図(コメントあり)。元から .NET 向け(netstandard・netcoreapp・net5+)にビルドしていたプロジェクトは、FOC1001〜1004 の対象外にした(クロスプラットフォームのコードで、区切りの扱いは意図したもの)。ただし yaf が同梱する Lucene.Net は net481 だけにしてあるので、この規則では外れない。同じ条件の `Path.GetFileName(...)` も FOC1004 で合わせられるため、結果は変わらない(無害な誤検出として残す)。

変換の結果の再現性:
- 同じ入力でもレポートの件数が変わっていた(dnn のプロジェクトとパッケージが 215・245 など)。原因は 2 つ。
  - NU1605 の版の引き上げ: restore が報告する順は並列の処理で変わり、2.1.1 → 8.0.2 → 10.0.5 と 2 段階で上げる回があった。求められた最も高い版まで一度に上げ、プロジェクトとパッケージごとに 1 件(元の版 → 最後の版)だけ記録する。
  - FOC1004 のファイルごとの例示(括弧の中)が、ビルドの出力の順で変わっていた。並べて出す。
- レポートの各節は、対象とテキストの順に並べる。dnn を続けて 2 回変換して、レポートが完全に一致することを確認した。

テスト(`tests/FrameworkOnCore.Tests`、26 件): アナライザーが見つけるもの・見逃すべきもの(上の誤検出を含め、コーパスで見つかった形)、互換アセンブリの OS ごとの動き。Windows で `dotnet test`、Linux で `run-tests-linux.ps1`(.NET SDK のコンテナ)。どちらも全件成功。

### 言語に依存しない形に(VB 対応の準備、2026-09-28)

VB のプロジェクトに同じ書き換えを使えるように、判定と書き換えを言語から切り離した。変換の出力は、下の意図した差を除いて前と同じ(6 本のコーパスを変換前後で比べた。``snapshot-conversion.ps1``)。

- アナライザー(FOC1001〜1006)は、C# の構文ではなくコンパイラーの操作(IOperation)で判定する。C# と VB の両方で動く(``Language`` が VB のプロジェクトにも差し込む)。
- 診断のメッセージの末尾に、対象のノードの長さと種類を付ける(``[len=12,char]``)。変換器は開始位置と長さでノードを特定する(言語によらない)。
- 書き換えは 1 つのファイルにつき 1 回(``SourceEdits``)。入れ子の書き換え(区切りを除く呼び出しを、さらに ``WindowsPath.Native`` で包む)も 1 回で行う。以前は種類ごとに書き換えては読み直し、同じ行の前の書き換えで列がずれる問題があった(行で探して避けていた)。
- 言語ごとの書き方は ``SourceLanguage``(C# と VB)。呼び出し、配列、キャスト、``global::``/``Global.``、定数の宣言。呼び出しの対象だけを差し替え、引数は書かれたとおり(改行・コメント)に残す。
- 式のテキストで決めていた置き換え(``platformReplacements``)は、シンボルで決める規則にしてアナライザー(FOC1006、PlatformAnalyzer)に移した。規則は変換器が追加ファイルとしてビルドに渡す。``System.Uri.TryCreate`` は ``UriKind`` を取る形だけ(Uri と文字列を取る形には Unix の問題が無い)。
- エラーの位置で書き換えたファイル(スタブ)の、ほかの書き換えの位置は捨てる(次のビルドがまた指す)。
- .NET 9 以降の params の Span(``TrimStart('\\', '/')``、5 個以上の ``Path.Combine``)の引数も読む。
- C# 14 は ``field`` をキーワードとして読むので、CS9258 の書き換えはその形も受け付ける。

変換前後の差(意図したもの):
- dnn: ``TrimStartRelative``・``AsyncDelegate.BeginInvoke`` の引数の区切りに空白(``a, b``)。
- mojo: ``Uri.TryCreate(Uri, string, out Uri)`` は書き換えない。動的な呼び出し(``dynamic`` の引数)のアプリ自身のメソッドは FOC1004 の対象外。
- yaf: 拡張メソッドの受け手(``this.ThemeFile.CombineWith(...)``)も、パスの引数として包む。

テストは 35 件(書き換えの C# と VB を含む。Windows・Linux とも成功)。VB 対応に残るのは、ビルドエラーの自動処理(スタブ・除外など。今は C# の構文とエラーコードだけ)と、VB のプロジェクトの変換そのもの。
変換器を作る過程で直したこと:
- 書き換えの後のビルドで、SYSLIB の警告(`obsoletions`)を消していた。後のビルドは変わったプロジェクトしかコンパイルしないので、ほかのプロジェクトの分がレポートから落ちていた(dnn で 19 件)。
- NuGet の packages フォルダーの判定: DNN のソースフォルダー `Services\Installer\Packages` を除外していた。中身(.nupkg、repositories.config)で判定するようにした。convert-project.ps1(robocopy `/XD packages`)にも同じ問題がある。
- アナライザーのプロジェクト参照(`OutputItemType="Analyzer"`)と Aliases の引き継ぎ: DNN はソースジェネレーターで部分メソッドの定義側を生成する(無いと CS0759)。
- 変換したプロジェクトでは `TreatWarningsAsErrors` を外す: 変換で加えた編集が StyleCop の警告になり、.NET の SYSLIB の警告もエラーになっていた。
- テンプレートのファイル名: `Program.cs.txt` の `cs` が MSBuild にチェコ語のカルチャと解釈され、サテライトアセンブリに回っていた。

### VB のプロジェクト(openIMIS、2026-09-28)

VB のプロジェクト(.vbproj)も、C# と同じ形で変換する。コーパスは openIMIS(`web_app_vb`、VB の Web Forms のアプリ。5 本のプロジェクトがすべて VB。DB は `database_ms_sqlserver` 24.10 のスクリプト。`corpora/fetch.ps1` の `imis`・`imisdb`)。

    .\experiments\wf4c\convert-corpora.ps1 -Only imis     # 元のビルド、変換、DB(.\SQLEXPRESS の imis_w2l)
    .\experiments\wf4c\imis-login.ps1                     # 起動したサイトにログインして主な画面を開く

変換:
- 旧形式の .vbproj は SDK 形式の .vbproj にする(言語は拡張子で決まる)。VB の設定をそのまま移す: `OptionStrict`(Off なら遅延バインディング)・`OptionExplicit`・`OptionCompare`・`OptionInfer`、プロジェクトの `Import`(SDK の既定の一覧は元と違うので `DisableImplicitNamespaceImports`)、`RootNamespace`(VB はすべての型をその中に置く。空も空のまま。SDK は空をプロジェクト名にしてしまう)。
- `DefineConstants` は VB の書き方(カンマ区切り、`BEPHA=1` のような値付き)。SDK の既定の定数は VB では `FinalDefineConstants` にあるので、前に `$(DefineConstants)` を付けない。ソースを読むとき(`#If`)も値付きで読む。
- `MyType`: デスクトップの My(`Windows`・`WindowsForms`・`Console`。My.Application・My.Computer・My.User)は .NET では Windows のデスクトップにしか無く、コンパイラーの My のテンプレートがその型を指すと、直しようのないエラー(ソースの位置が無い)になる。ソースが My を使っていなければ `Empty`、使っていればサーバーの My(`Web`。互換アセンブリの型)にする(DNN の DotNetNuke.WebUtility は `Windows` で、My を使っていない)。
- Web プロジェクトの入口は `Program.vb`(`templates/ProgramTemplate.vb.txt`。ルートの名前空間の外に置くため `Namespace Global`)。
- 同じソースファイルが 2 回並んでいるプロジェクト(openIMIS の `Resource1.designer.vb`)は 1 回だけコンパイルする。Visual Studio のプロジェクトシステムは 1 回にするが、コマンドラインの VB コンパイラーは型を 2 回定義する(BC30179)。元のビルド(ソリューション)でも、コンパイルの前に `Compile` の重複を除くターゲットを差し込む(`CustomAfterMicrosoftCommonTargets`)。
- `--configuration <名前>`: 元のビルドの構成(ソリューションのビルドの既定は Release)と、変換でプロジェクトの条件を読む構成(既定は Debug)。openIMIS は `DemoRelease`(リポジトリに web.config の変換があるのはこれだけ。`#If DEMO` のコードもその構成のもの)。
- ページ(`Language="vb"`)は、フォークの VB のコード プロバイダーが実行時にコンパイルする(変更なし)。

ビルドエラーの自動処理(`BuildFixer.VisualBasic.cs`): C# と同じ判断を VB の構文で行う(Imports・属性を外す、`Overrides` を外す、本体を `Throw New PlatformNotSupportedException` にする、初期化子・宣言を外す、ファイルを除外する)。VB だけのもの:
- BC36908(遅延バインディングの呼び出しに拡張メソッドは使えない): `Option Strict Off` で型の無い引数を渡す呼び出し(`Encoding.UTF8.GetBytes(inputString)`)は、.NET Framework では実行時に受け手のメンバーから選ばれていた。.NET が同じ名前の拡張メソッド(`EncodingExtensions.GetBytes`)を加えたのでエラーになる。受け手を `CObj(...)` にして、元と同じ遅延バインディングにする。

Windows 専用の API の置き換え(`platformReplacements`、FOC1006)に加えたもの:
- `EventLog.WriteEntry`・`SourceExists`・`CreateEventSource` → 互換アセンブリの `EventLogs`。openIMIS はログインのたびにイベントログに書き、`Application_Start` でソースを作る(Linux ではログインが 500 になっていた)。
- インスタンスのメソッドの呼び出し(`log.WriteEntry(m)`)は、受け手を置き換え先の最初の引数にする(`EventLogs.WriteEntry(log, m)`)。構文では型の名前(静的な呼び出し)と区別できないので、アナライザーがシンボルで判定して診断に付ける(`rule7_instance`)。以前は受け手を黙って落とす作りだった(既存の規則は静的なメンバーだけなので影響なし)。

`rules/packages.json` の `noAnswer` に加えたもの: `System.Web.Extensions.Design`(Visual Studio のデザイナー)と `System.Windows.Forms`(デスクトップ)。web.config のページのコンパイルの `<assemblies>` にあると、読み込みに失敗して全ページが構成エラーになる(openIMIS)。ほかのコーパスではプロジェクトの参照にあるだけで、以前から黙って外していた(レポートに 1 行ずつ増える)。

フォーク: Ajax Control Toolkit(サブモジュール `src/WebFormsForCore.AjaxControlToolkit`)もパッケージにする(`pack-frameworkoncore.ps1`、`frameworkoncore.slnx`)。openIMIS が使う。初回は `git submodule update --init src/WebFormsForCore.AjaxControlToolkit`。

確認した結果:
- 元のビルド: 5 本とも成功。
- 変換: 5 本ともビルド成功。変換で変えたソース: 遅延バインディング 1 件(上の BC36908)、Linux のパスの書き換え 123 件(`ReportPath` の `Reports\*.rdlc`、`MapPath` への結合、7-Zip の DLL の場所など)。
- Windows(`.\SQLEXPRESS`)・Linux(`run-linux-site.ps1 -SqlScripts` でコンテナの SQL Server に DB のスクリプトを流す): デモの `Admin` でログインし、主な画面が 200。Linux ではログインの記録がコンテナのログに出る(`IMIS: Information 1: Admin has logged in.`)。
- be/wt: 変換結果は変わらない(`snapshot-conversion.ps1`)。Linux で be 5/5、wt 5/8(以前と同じ)。
- テスト 41 件(EventLog の書き換えの C# と VB、互換アセンブリの EventLogs と VB の My)。Windows・Linux とも成功。

未確認・残り:
- レポートの画面(ReportViewer)で実際にレポートを出すこと。ReportViewer は .NET Framework 向けの DLL のまま参照している(ビルドは通る)。
- 7-Zip(SevenZipSharp)・SQL Server の型(Microsoft.SqlServer.Types)のネイティブライブラリを使う機能(オフラインのデータの書き出しなど)。
- dnn・n2 の再検証(2026-09-28、下の「dnn・n2 の再検証」)。
- Linux の 1 回目の実行で、`/` が 1 度だけエラーページ(`Error.htm`)になった。その後の 4 回は再現しない。`Application_Start` の `EventLog.SourceExists` が Linux で例外を投げていたことと関係する可能性がある(今は置き換えた)。
### dnn・n2 の再検証(2026-09-28)

VB のプロジェクトの変換と `EventLog` の書き換えの後、dnn・n2 を Windows・Linux で動かし直した。

- 変換結果の差(`snapshot-conversion.ps1`): dnn は 17 ファイル。DotNetNuke.WebUtility(VB)をソースから変換するようになり(`MyType Windows` → `Empty`、空の `RootNamespace` のまま)、参照する 13 プロジェクトがそのプロジェクトを参照する。WebUtility のパスの書き換え(ClientAPI・BrowserCaps)と、log4net の EventLogAppender の `EventLog` の書き換え。n2 は差なし。
- dnn: Windows(`dnn-cycle.ps1`)・Linux とも、空の DB からインストールが完了し、トップページ・`/Login`・`/Terms` が 200、ページの資源 21 件がすべて 200。host のログインは資格情報が通り、DNN のパスワードの強制変更の画面に進む(誤ったパスワードでは「Login Failed」)。
- n2: Windows・Linux とも、インストーラー(管理者のパスワード → テーブルの作成 → サンプルのコンテンツの取り込み)が完了し、トップページがコンテンツ付きで表示される。

見つかったこと:
- **Linux で、大文字を含むフォルダーの web.config が読まれていなかった**(フォーク 0027 で直した)。ASP.NET は構成パスを小文字で扱う(`machine/webroot/1/n2`)。その物理パス `/app/n2` は Linux には無いので、そのフォルダーの構成は無いものとされ、親の構成が使われていた。n2 の管理画面(`/N2/Default.aspx`、インストーラー)が、ログインせずに 200 になっていた(Windows はログインの画面へ 302)。同じ仕組みで、wt の `Admin`・`Checkout`、DNN の `Portals`・`Install`・`DesktopModules/MVC`、mojo の `Data`・`Views`、be の `Account` などのフォルダーの web.config も効いていなかったはず(直す前の状態を確かめたのは n2 だけ)。直した後、n2 と wt の保護されたページはログインの画面へ 302。
- n2 の以前の確認(Linux で「インストールが完了」)は、この問題でインストーラーに誰でも入れたために通っていた。リポジトリの `App_Data\n2.sqlite.db` にはサンプルのサイトが入っているので、`/` はインストーラーではなくサイトになる。`n2-install.ps1` は、インストーラーの最初のページを開いてからそのフォームを送るようにした(以前は `/` のフォームを送っていた)。
- DNN のインストーラーは、完了すると `Install\*.aspx` を消す。Windows でインストールしたサイトのフォルダーでは Linux のインストールはできない(変換し直してサイトを組み立て直す)。
- インストーラーが web.config を書き換えるとアプリが再起動し、その間の要求は応答が無い(`000`・503)。続けて要求すれば完了する。
- このマシン(メモリ 8 GB、Docker は 4 GB)では、Docker Desktop のエンジンが何度か止まった(`docker desktop restart` で戻る)。SQL Server のコンテナは要らないときは止める。

be/wt(フォーク 0027 の後): Linux で be 5/5、wt 5/8(以前と同じ)。
### ファイル名の大文字小文字(試作、2026-09-28)

Windows はファイル名の大文字小文字を区別しない。アプリは食い違った名前で書かれていることが多く(be の `Web.Config`・`Global.asax`・`Custom`、ASP.NET 自身の小文字の構成パス)、Linux では見つからない。これまではフォーク(0008・0027)と変換器の書き換え(`WindowsPath.Native`)で箇所ごとに直していたが、サードパーティの DLL やネイティブライブラリには届かない。根本的な対策として、プロセスの中でファイルの操作に割り込む共有ライブラリを試作した(`casefs/`)。

- `libfoccase.so`(`casefs/foccase.c`、C): `LD_PRELOAD` で読み込み、C ライブラリのファイルの関数(open・stat・opendir・mkdir・rename・unlink・realpath・inotify_add_watch・dlopen など。.NET の System.Native・coreclr・hostpolicy が使うものを `nm` で調べて選んだ)を包む。
  - まず頼まれたとおりに呼ぶ。名前が無くて失敗したとき(ENOENT・ENOTDIR)だけ、パスの各部分を大文字小文字を区別せずに探して、もう一度呼ぶ。正しい名前の呼び出しには何もしない。
  - 作成(O_CREAT・mkdir・rename の行き先など)は Windows と同じ: 大文字小文字違いの名前があればそれ、新しい名前は既にあるフォルダーの中に作る。大文字小文字だけの名前の変更もできる。
  - `FOC_CASE_ROOTS`(`:` 区切り)の下だけ。未設定なら何もしない。`FOC_CASE_LOG=1` で、大文字小文字違いで見つけたパスを標準エラーに 1 回ずつ書く(アプリの食い違いの一覧になる)。
  - フォルダーの一覧はキャッシュする(フォルダーの更新時刻で無効にする)。
- 作り方・試し方: `casefs\build.ps1`(linux-x64 と linux-arm64。古い glibc の Ubuntu 20.04 でビルドし、必要な glibc は x64 で 2.14、arm64 で 2.17 以上。.NET 10 が動くディストリビューションはすべて満たす)、`casefs\test.ps1`(`test/CaseProbe` を、ライブラリ無し(Linux の動き)と有り(Windows の動き)で実行。`-Platform linux/arm64` で arm64)。`run-linux.ps1`・`run-linux-site.ps1` の `-CaseInsensitive` で、サイトのプロセスに読み込ませる。`casefs/test-install.sh` は、変換の出力を `install.sh` で入れて起動する(systemd の無いコンテナで)。
- 変換器の配置の出力に組み込む(`--case-insensitive on|off`、既定は on): `deploy/casefs/<linux-x64|linux-arm64>/libfoccase.so`(と `foccase.c`)を置き、`start.sh` が CPU に合うものを、読み込めるか試してから `LD_PRELOAD` に入れる(`FOC_CASE_ROOTS` の既定はサイトのフォルダー)。読み込めないマシン(Alpine の musl など)では警告を出して区別するまま動く。配置した後は `FOC_CASE_INSENSITIVE=0` で止める。Dockerfile は `deploy/casefs` を `/opt/foc/casefs` に、`install.sh` は `<prefix>/casefs` に置く。`off` なら置かない(`casefs\build.ps1` でビルドしていなければ、エラーをレポートに書いて置かない)。

確認した結果:
- CaseProbe の 16 項目: 無しではすべて Linux の動き、有りではすべて Windows の動き。別の大文字小文字での存在確認・読み込み・一覧、既存の名前への書き込み(同じファイル)、大文字小文字違いのフォルダーへの新しいファイル・フォルダー、移動、大文字小文字だけの名前の変更、削除、相対パス、FileSystemWatcher、アセンブリの読み込み、対象外のパスは変わらないこと。
- be(Linux、読み込みあり): 5/5。wt: 5/8(以前と同じ)。wt の `/admin/adminpage`・`/ADMIN/ADMINPAGE.ASPX` はログインの画面へ 302、`/content/SITE.css` は 200。作業プロセス(フォーク 0018)にも環境変数で引き継がれる。
- 速さ(File.Exists 1 回): 正しい名前 2.97 µs(無しで 2.95)、大文字小文字違い 14.5 µs、無い名前 9.4 µs(無しでどちらも約 2 µs)。

途中で見つかったこと: wt を Linux で動かすと `/Admin/AdminPage` にログインせずに入れた。変換がフォーク 0027 より前だったため(bin の System.Web が古い)。変換し直すと 302 になった。0027 より前に変換したサイト(mojo・yaf)は、変換し直さないと同じ状態のまま。

制約・残り:
- 名前の比べ方は Windows と同じ(NTFS の upcase table。`casefs/casemap.h`、`make-casemap.ps1` が Windows の `RtlUpcaseUnicodeChar` から作る。973 文字。基本多言語面の外の文字と UTF-8 でないバイトはそのまま比べる)。`Äpfel`・`Ωmega`・全角の `Ｆｕｌｌ`・`ÿ`/`Ÿ` は同じ名前、`Straße` と `STRASSE`、語末のシグマ `ς` と `Σ` は別の名前(この Windows の NTFS で同じ結果になることを確かめた)。
- glibc だけ(Alpine の musl は未対応。読み込めないので start.sh が外す)。C ライブラリを通らないもの(システムコールを直接呼ぶプログラム、静的リンク、setuid)には効かない。.NET とそのネイティブライブラリは対象。
- 区切り文字(`\`)は扱わない(.NET の `Path` がシステムコールより手前で区切りを解釈するため)。アナライザーの書き換えは引き続き要る。
- フォークの 0008・0027 と `WindowsPath.Native` の大文字小文字の照合は、ライブラリを読み込めないマシンのために残す。`WEBFORMSFORCORE_PATH_CASING=0`(フォーク 0028、互換アセンブリの `WindowsPath` も従う)で止められ、配置の `start.sh` はライブラリを読み込んだときにこれを設定する(ライブラリだけで照合する)。

配置の出力に組み込んだ後の確認:
- be のコンテナ(`docker build`、生成された Dockerfile): 正解データと比べて 5/5。ログに大文字小文字違いで見つけた名前(`web.config` → `Web.Config`、`Default.aspx` → `default.aspx` など)。`FOC_CASE_INSENSITIVE=0` では 1 件も出ない。`--case-insensitive off` の変換では `deploy/casefs` も Dockerfile の COPY も無い。
- be を `install.sh` で(Ubuntu 22.04・glibc 2.35、Debian 12・glibc 2.36): `/` が 200、ライブラリが働く。Alpine(musl)では読み込みの試しでエラーになり、区別するまま動く。
ライブラリだけで足りるかの確認(`casefs\verify-alone.ps1`、Linux、be/wt を正解データと比べる。wt は保護されたフォルダーを大文字小文字を変えて要求する):

| 構成 | be | wt | wt の `/Admin/AdminPage`(ログインなし) | wt の `/admin/adminpage`・`/checkout/checkoutreview` | wt の `/content/SITE.css` |
|---|---|---|---|---|---|
| 既定(フォークが照合、ライブラリなし) | 5/5 | 5/8 | 302(ログイン) | 302 | 200 |
| 照合なし(`WEBFORMSFORCORE_PATH_CASING=0`、ライブラリなし) | **0/5**(`/` が 500) | 5/8 | **200(管理画面が開く)** | 404 | 404 |
| ライブラリだけ(`WEBFORMSFORCORE_PATH_CASING=0`、ライブラリあり) | 5/5 | 5/8 | 302 | 302 | 200 |

- ライブラリだけで、既定と同じ結果になる(wt の 3 件の差は以前からの既知のもの)。照合をすべて外すと be は動かず、wt の `Admin` の承認が効かなくなる(大文字小文字の照合がそれを担っていた)。
- 配置の `start.sh` で、ライブラリを読み込んだときに `WEBFORMSFORCORE_PATH_CASING=0` にした後も、be のコンテナは 5/5。

フォークのパッケージ: Ajax Control Toolkit は net10.0 だけでビルド・パッケージにする(`pack-frameworkoncore.ps1`)。ソリューションの中で net8.0 のビルドが CS7069 で失敗するようになった(net8.0 は 2026-10-11 に全プロジェクトから外した)。`-f net10.0` でビルドし、`--no-restore` でパッケージにする(`TargetFrameworks` をグローバルプロパティで渡すと、参照先のプロジェクトまで net10.0 だけで復元され、そのパッケージが作れなくなる)。ソリューションのビルドの再試行は 3 回まで。
- arm64: CaseProbe(22 項目)が、エミュレーション(Docker Desktop)の arm64 の ASP.NET のイメージで、無しでは Linux、有りでは Windows の動き。be のコンテナを arm64 でビルドして 5/5(`start.sh` が linux-arm64 のライブラリを選ぶ)。見つけた問題: ライブラリの中で呼ぶ古い stat 関数(`__xstat64`)に構造体の版を `1` と書いていた。x86_64 の値で、aarch64 は `0`。arm64 ではすべての呼び出しが失敗し、何も探せていなかった。ビルドする CPU のヘッダーの `_STAT_VER` を使う。- 見つけた問題: glibc 2.35 でライブラリを読み込むと .NET のホストが起動しなかった(`Failed to resolve full path of the current executable`)。glibc の `realpath` には 2 つの版(GLIBC_2.2.5 と 2.3)があり、名前だけで探す `dlsym` が古い版を返した。古い版は結果の置き場所に NULL を受け付けない(.NET のホストは NULL で呼ぶ)。複数の版があるもの(`realpath`、`dlopen`)は版を指定して探す(`dlvsym`)。ほかの包む関数は版が 1 つだけ(Ubuntu 22.04・24.04、Debian 12 で確認)。
- `install.sh` が systemd の無いマシン向けに示す起動のコマンドは、`/etc/<app>/environment`(root だけが読める)を別のユーザーで読もうとして失敗していた。root で読んでから `setpriv` でユーザーを切り替える形にした。
全コーパスの変換し直しと確認(2026-09-29、フォーク 0027・0028、大文字小文字のライブラリの後):

- 変換結果(`snapshot-conversion.ps1`): 7 本とも変換したソースは前と同じ。配置の出力には 7 本とも `deploy/casefs`(x64・arm64)が入る。
- Linux は、配置の既定と同じ構成(ライブラリを読み込み、`WEBFORMSFORCORE_PATH_CASING=0`。`run-linux-site.ps1 -CaseInsensitive -Environment @{ WEBFORMSFORCORE_PATH_CASING = '0' }`)で確かめた。

| コーパス | Linux | Windows |
|---|---|---|
| wt | トップ・About が 200。`/Admin/AdminPage`・`/admin/adminpage`・`/checkout/checkoutreview` はログインへ 302、`/content/SITE.css` は 200 | トップが 200 |
| mojo | 空の DB からセットアップ → トップ・ログイン(`/secure/LOGIN.aspx` も)が 200、`/Admin` はログインへ | トップが 200 |
| yaf | 下 | 下 |
| dnn | 空の DB からインストールが完了、トップ・`/Login`・`/Terms`・資源 21 件が 200、host の資格情報が通る(パスワードの強制変更へ) | インストールが完了、トップが 200 |
| n2 | インストーラーが完了、トップがコンテンツ付き、`/N2/`・`/n2/installation/` はログインなしでログインへ 302 | 同じ |
| imis | ログインし、主な画面 7 つが 200 | 同じ |

- ライブラリが大文字小文字違いで見つけた名前: dnn 1065、n2 197、imis 67、wt 34 など。yaf は出力先が `Bin`(大文字)で、Linux の `bin/YAF.dll` をライブラリが見つけて起動した。yaf の `customErrors` の `Error.aspx` も実際の `error.aspx` で見つかった。
- yaf: Linux・Windows とも同じところで止まる。ServiceStack.OrmLite の `Env` の静的コンストラクターが変換のスタブ(CS0103 `PclExport`: ServiceStack の `#if NET6_0_OR_GREATER` の分岐が、元の net481 構成では除かれていたファイルを要求する)で例外になり、エラーのページになる。以前の記録(`/` で Web API 2 の `FieldAccessException`)とは止まる場所が違う。変換したソースは前と同じで、理由は突き止めていない(`FieldAccessException` は型の初期化の前か後かで出たり出なかったりする可能性がある)。
- yaf を curl で確かめるときは Cookie を保つこと: yaf は `Session_Start` で起動時の確認をするので、Cookie なしの要求は毎回新しいセッションになり、インストーラーへのリダイレクトが繰り返される(ブラウザーでは起きない)。
### nopCommerce 1.90(C#、2026-09-29)

nopCommerce の最後の Web Forms 版(2011、.NET Framework 4.0、56 プロジェクト、Entity Framework 4 の EDMX)。`corpora/fetch.ps1` の `nop`、`convert-corpora.ps1 -Only nop`(元のビルドはソリューション)。DB はアプリのインストーラー(`/install/install.aspx`: SQL Server、新しい DB か空の DB、サンプルデータ)。

変換器に加えたもの(ほかのコーパスにも当てはまる形で):
- 元のビルド: .NET Framework 2.0・3.5・4.0 の参照アセンブリ(nuget.org)。2.0・3.5 のプロジェクトは MSBuild の「.NET Framework 3.5 がインストールされているか」の確認を飛ばす(`BypassFrameworkInstallChecks`)。古い Visual Studio の Web Application のターゲット(`VisualStudio\v10.0`、条件なしの Import)は、インストールされた版のものに(Visual Studio のプロジェクトの変換と同じ。コピーの中で)。
- プロジェクトの参照のパスが無いとき、その GUID のプロジェクト(Visual Studio がソリューションで見つけるのと同じ。nopCommerce の販促プロバイダーは `..\Nop.BusinessLogic` を指すが、実際は `Libraries` の下)。元のビルドと変換の両方。
- Entity Framework 4(.NET Framework の `System.Data.Entity`)→ EF6: ソースが EF4 の名前空間を使っていれば EF6 のパッケージを加え、ビルドが見つけない名前空間(CS0234: `System.Data.Objects` → `System.Data.Entity.Core.Objects` など)と型(CS0246・CS0103: `System.Data` にあった `EntityState` など。`using System.Data` のあるファイルだけ)を、今の場所で書く(`namespaceMoves`・`typeMoves`)。移動を見つけたラウンドは移動だけを行う(それに続くエラーは次のビルドのもの。スタブにしない)。スタブは 876 件 → 39 件。
- `EntityDeploy`(.edmx): .NET の SDK には無い。概念・格納・マッピングのモデルに分け、元のビルドと同じ名前で埋め込む(`Data.NopModel.csdl` など。元の DLL のリソース名と一致を確認)。openIMIS の EF6 のモデル(`Model1.edmx`)も、これまで埋め込まれていなかった。
- `Directory.GetAccessControl` など(.NET では DirectoryInfo の拡張メソッド、CS1929): 互換アセンブリの `DirectoryAcl`・`FileAcl`(Windows のみ、ほかでは .NET と同じく例外)。`memberReplacements` は CS1929 も扱う。
- リポジトリに置かれた DLL にも `replacedPackages` を当てる(nopCommerce の AjaxControlToolkit 4.1 → フォークの Ajax Control Toolkit。.NET の ASP.NET AJAX は古い版を受け付けない)。
- `noAnswer` に `System.Web.DataVisualization`(グラフのコントロール)。web.config からは、ページのコンパイルのアセンブリに加え、それを指す `<pages><controls>`(タグの接頭辞の解決で、その接頭辞のすべてのページが読み込もうとする)、ハンドラー・モジュールも外す。サイトと Web プロジェクトのすべての web.config(フォルダーのものも)。

確認した結果:
- 変換: 56 プロジェクトのビルドが通る。スタブ 47 件(グラフ 2 画面、`ToolkitScriptManager` の宣言 9 件、使われていない `MobileControls` の using)。
- Windows: インストーラーで DB を作り(サンプルデータ)、トップ・ログイン・検索が 200(DB の分類・メーカー・タグが出る)。管理画面はログインが通るが、各ページは 500。
- Linux(ライブラリあり): インストーラーが完了、ログイン・検索が 200。トップは 500(System.Drawing: 商品画像の縮小に GDI+。Linux には無い)。管理画面は同じく 500。

残り:
- `ToolkitScriptManager`: 新しい Ajax Control Toolkit では無くなった(`asp:ScriptManager` を使う)。マークアップ(`<ajaxToolkit:ToolkitScriptManager>`)の書き換えが要る。管理画面のマスターページなどが使う。
- System.Drawing(Linux): 画像の処理。以前に挙げた「Linux で実行時に例外になる API」の最大のもの。
- グラフ(`System.Web.DataVisualization`): .NET に無い。管理画面のレポート 2 つ。
- フォークの kernel32 の直接の呼び出し: `Server.MachineName` は直した(0029)。IIS 以外から呼ばれうるものを機械的に拾うと 100 件ほど(多くは呼ばれないか、呼ぶ側で OS を確かめている)。

### .NET Framework にしかない API(2026-09-29)

8 コーパスの変換レポートとソースから洗い出し、.NET 10 / Linux での実際の挙動を確かめた(`FRAMEWORK-ONLY-APIS.md`、`api-probe/`)。例外になる 3 つを変換器で直した。確認用に `samples/RuntimeProbe`(.NET Framework の Web Forms)を作り、変換して Windows と Linux で開く(`probe-requests.ps1 [-Linux]`)。

- コードページ(Shift_JIS など): .NET はプロバイダーを登録しないと Unicode と ASCII・Latin-1 だけ。**(訂正 2026-09-29)** フォークの `UseWebForms` が以前から登録していた(検索が .gitignore のフォークのソースを見ておらず、見落とした)。Program.cs(.vb)のテンプレートに入れた登録は重複なので外した。
- `Encoding.Default`: .NET Framework はシステムの ANSI コードページ(日本語の Windows では Shift_JIS)、.NET は UTF-8。互換アセンブリの `Platform.DefaultEncoding` に置き換える(Windows は GetACP、Linux は LANG のカルチャの ANSI コードページ。deploy の start.sh は元のサーバーのカルチャを LANG にする)。be・mojo・n2・nop が使う。
- `Thread.ResetAbort`: .NET では例外。フォークの `Response.End`(Redirect・Transfer)は ThreadAbortException を投げるので、DNN の `catch (ThreadAbortException) { Thread.ResetAbort(); }`(URL の書き換え、モジュールの読み込みなど 5 か所)は実際に通る。互換アセンブリの `Platform.ResetAbort` に置き換える(フォークの `HttpResponse.ResetThreadAbort` を実行時に探して呼ぶ。互換アセンブリはフォークを参照しない)。直さないと、リダイレクトが 503 になり、catch の後のコードが動かなかった。
- `BinaryFormatter`: .NET 9 で削除(例外)。ソースが使っていれば、互換パッケージ `System.Runtime.Serialization.Formatters` 10.0.12 を加え、Web プロジェクトに `EnableUnsafeBinaryFormatterSerialization`(`sourcePackages` の `appProperties`: 使うのがライブラリでも、実行されるアプリに付ける)。パッケージのアセンブリ(10.0.0.0)はランタイムのもの(8.1.0.0)より上なので、ランタイムの修正版に関係なくこちらが読み込まれる。.NET Framework 4.8 が書いたデータ(Hashtable、List<string>、DateTime)を読めることを確認した。フォークの `WebFormsForCore.Serialization.Formatters` は別の名前空間で、フォーク自身のためのもの。
- 置き換えた呼び出しは、.NET の廃止の警告(SYSLIB0006)の「実行時に例外」の報告から外す。

確認: テスト 47 件(Windows・Linux)。RuntimeProbe は Windows・Linux とも .NET Framework と同じ(リダイレクトは 302 で、catch の後も動く。Shift_JIS のバイト列。BinaryFormatter の読み書き)。be・wt の変換の差は上の変更の分だけ。Linux の実行は be 5/5、wt は前と同じ 5/8(既知の丸めの差 2 件と error-page)。

残り: ソースのない DLL だけが BinaryFormatter を使う場合(imis の ReportViewer など)は検出しない。`Encoding.GetEncoding(0)` と VB の `FileOpen` 系の既定のエンコーディングは未対応(コーパスでは使われていない)。

System.Drawing を Linux で動かす試み(`api-probe/drawing`、Ubuntu 24.04 の .NET 10 SDK イメージ):
- System.Drawing.Common 10 は GDI+ を `gdiplus.dll` として読む。libgdiplus を入れて名前を合わせても(シンボリックリンク、DllImport のリゾルバー)、見つかった後で「Windows 以外は非対応」の例外を投げる(.NET 7 からの仕様)。自分のリゾルバーも登録するので、アプリのリゾルバーとぶつかる。
- System.Drawing.Common 6.0.0(Unix 実装のある最後の版)+ ランタイムの設定 `System.Drawing.EnableUnixSupport=true` + libgdiplus(Ubuntu の 6.1)なら、試した 12 項目がすべて動く。画像の作成・PNG/JPEG/GIF の保存と読み込み、nop の縮小(HighQualityBicubic、JPEG の品質)、文字の描画(CAPTCHA)、フォント、LockBits、グラデーション、サムネイル、EXIF。
- フォーク(WebFormsForCore.Web、AjaxControlToolkit、WebFormsForCore.Drawing)は 10.0.0 に対してビルドされている。6.0 のファイルを同じ名前で置くと、バージョンが低いので読み込まれない。10 をアプリのアセンブリの一覧(deps.json)から外し、`AssemblyLoadContext.Resolving` で 6.0 を渡すと、10 に対してビルドしたコードもそのまま動く。
- 使うなら: Linux の配置で libgdiplus とフォントを入れ、6.0 の Unix 実装を別のフォルダーに置き、起動時に Resolving で渡す(Windows は 10 のまま)。ただし 6.0 も libgdiplus もサポートが終わっている(Microsoft は非推奨。利用者がアップロードした画像を扱うなら特に)。
### API の使用状況の解析(2026-09-29)

部品ごとに利用者が対応を選ぶ仕組みの第 1 段階。アプリが使う .NET Framework の API を一覧にし、呼び出し回数と .NET 10 での状態を出す(変換はしない)。

    dotnet src\FrameworkOnCore.Converter\bin\Debug\net10.0\FrameworkOnCore.Converter.dll analyze <web project> --out <dir> [--root <dir>] [--configuration <name>]
    .\experiments\wf4c\analyze-corpora.ps1 [-Only be,wt]     # _analysis\<name>\api-analysis.json、API-ANALYSIS.md

作り(`src/FrameworkOnCore.Analysis`、ライブラリ。将来のローカルの Web 画面・サービスからも使えるように、結果はパスをリポジトリからの相対にした JSON、API は Roslyn のドキュメント ID):
- プロジェクト: 古い形式は自前で読む(構成の条件、HintPath、ProjectReference、ほかのプロジェクトの出力への HintPath、PackageReference。復元されていない packages\ の DLL は NuGet のキャッシュ・nuget.org から)。SDK 形式は `dotnet msbuild -getProperty -getItem` で MSBuild に評価させる(Directory.Build.props/targets の DefineConstants・Using、パッケージ)。.NET 向けにもビルドされるものは型のためだけにコンパイルする(数えない)。
- ソース: .NET Framework 4.8 の参照アセンブリに対して C#・VB をコンパイルし、名前が結び付く記号(型、メンバー、コンストラクター、インデクサー、オーバーライドした基底のメンバー、デリゲートの BeginInvoke)を書かれた場所で数える。公開・プロテクトの API だけ。別名の宣言や匿名型のメンバー名は数えない。署名したプロジェクトは公開署名でコンパイルする(InternalsVisibleTo)。SDK 形式の参照は推移的。
- ソースのない DLL: メタデータの型・メンバーの参照を同じ ID の形で読む(呼び出し回数ではなく参照の有無)。変換で置き換わるパッケージ、.NET 版のあるパッケージは読まない。
- .NET 10 側: .NET の参照アセンブリ、変換規則が加えるパッケージ、フォークのパッケージとその依存(System.CodeDom など)、互換アセンブリ(拡張メソッドで補うものも)。基底型に移ったメンバー、インデクサーの名前の違いも探す。`SupportedOSPlatform("windows")` と `Obsolete`(例外を投げる SYSLIB はカタログで判別)を読む。
- 部品: `catalog/components.json`(30 部品。一致しない API はそのアセンブリの部品)。状態は「.NET に無い / 例外(全 OS)/ 例外(Linux)/ 動きが違う / 廃止予定(動く)/ そのまま」。

8 コーパスで確認: 1 本 3〜39 秒。解決できなかった名前は .NET Framework の API の使用に対して 0〜2.9%(be・nop は 0。残りは GAC の ReportViewer、実行時に生成されるソースなど)。変換で分かっていたこと(dnn の ResetAbort 5 か所、n2 の BinaryFormatter、nop の EF4・グラフ、imis の EventLog)が一覧に出る。

誤検出を直したもの: 名前付きタプルの要素(実体は Item1)、内部の型、.NET で基底型に移ったメンバー(DirectoryInfo.FullName)、インデクサー(Roslyn の ID は Item、VB は ItemOf)、Roslyn の ID の戻り値の型(`~System.String`)、フォークの依存パッケージ、互換アセンブリの拡張メソッド。

### 部品ごとの選択(第 2 段階、2026-09-30)

- カタログ(`catalog/components.json`)に部品ごとの選択肢(`options`。既定、予定 `planned` は選べない)と、API でないアプリの設定(`settings`: ファイル名の大文字小文字)。選択肢の無い部品は「対応しない」だけ。
- 選択のファイル `foc-choices.json`(`Choices`: 部品 -> 選択肢、API ごと(ドキュメント ID)、設定)。`analyze` が、対応を選ぶ部品を既定の値で書き出す。
- 変換器の `--choices <file>`: カタログで検証し(無い部品・選択肢、予定の選択肢はエラー)、規則を絞る(`Rules.Choose`)。規則は属する選択肢を `"option": "部品:選択肢"` で持つ(packages.json の 14 規則、EF4 の名前空間の移動は `namespaceMovesOption`)。コードの分岐は `rules.IsChosen`(FOC1005 のデリゲート、EDMX の埋め込み)。ファイル名は `--case-insensitive` を指定しなければ設定の値。既定と違う選択はレポートに残す。
- 確認: 選択なしと既定のファイルは同じ変換(be・wt はスナップショットで一致)。RuntimeProbe を「対応しない」で変換すると、差分は選んだ分だけ(4 ファイル、配置に casefs なし)で、実行すると .NET の既定の動き(ResetAbort の後が動かない、Encoding.Default が UTF-8、BinaryFormatter が 500)。テスト 55 件(Windows・Linux)、be は Linux で 5/5。
- 訂正: コードページはフォークが登録するので、選択肢は「登録する(常に)」だけにした。

### Studio(GUI、2026-09-30)

    dotnet build src\FrameworkOnCore.Studio
    dotnet src\FrameworkOnCore.Studio\bin\Debug\net10.0\FrameworkOnCore.Studio.dll [--data <dir>] [--port 5300]
    # http://127.0.0.1:5300/ (localhost だけで待ち受ける。解析は既定で %LOCALAPPDATA%\FrameworkOnCore\studio に残る)

- `src/FrameworkOnCore.Studio`: ASP.NET Core の Minimal API と、素の HTML・CSS・JS の画面(ビルドの道具は要らない)。画面は HTTP の API だけを使う(将来サービスにするため): `GET /api/catalog`、`GET|POST /api/analyses`、`GET|DELETE /api/analyses/{id}`、`/result`、`GET|PUT /choices`(カタログで検証、エラーは 400)、`/source`(リポジトリの外のファイルは読まない)、`/command`(選択を渡す変換のコマンド)。
- 解析は 1 つずつ裏で動き(メモリ)、画面が進み具合とログを見る。変換器の `AnalyzeCommand.Analyze` を使う。
- 画面: 解析の一覧、数字のタイル(対応を選ぶ部品、API の種類、使用回数、名前の解決率)、状態別の内訳(帯グラフ、凡例、ツールチップ。状態の色は予約された色で、必ず記号と文字を添える)、アプリの設定、部品のカード(選択肢のカード、既定・予定の印、変更の印)、部品の API の一覧(場所をクリックするとソース、API ごとの選択)、保存のバー(未保存の件数、既定に戻す、保存、変換のコマンドのコピー)。ダーク(既定)とライト。部品へのリンク `#/a/<解析>/c/<部品>`。
- 確認: API から be・wt を解析し、選択の保存(不正な選択は 400)、ソース(リポジトリの外は 404)、コマンドを確認。ヘッドレスの Edge で画面を撮って見た目を確認した。

次: 移植した部品(LINQ to SQL、グラフ)や System.Drawing の libgdiplus を、カタログの選択肢として加える(第 4 段階)。

### Studio のウィザード(2026-10-02)

画面が一枚に詰まって流れが分かりにくかったので、① 分析 → ② 方針決定 → ③ 変換・ビルド → ④ デプロイ の 4 手順にした(URL `#/a/<解析>/step/<n>`)。

- ① 分析: 元のアプリのテスト起動、数字、内訳、部品と API(対応は表示だけ)。
  - テスト起動は、変換器の新しい `build-original` サブコマンドで元のアプリをビルドする(`--build-original` と同じ `OriginalBuild`、解析のフォルダーの `original/work`)。
  - 元のアプリのビルドは、毎回新しいフォルダー(`<基>-<日時>`。`out.original-20261002-214647` など)で行う(2026-10-02)。
    - 以前は同じフォルダーを消してから作り直していた。前回のフォルダーが消せないと(yarn が node_modules に作るリンク、開いたままのファイル)、変換が止まった。
    - 前のフォルダーはビルドの後に消す。消せないものは、レポートに残して次回また消す。
    - フォルダーの削除はすべて `FileTrees.Delete` に統一した。リンクはリンク自体を消し、読み取り専用を外し、使用中のファイルは少し待ってやり直す。
  - 配置されたサイトは IIS Express で動かす。無ければ、Studio が管理者のときだけ IIS で動かす(専用のサイトとアプリケーション プール。止めると消す)。
- ② 方針決定: 選べる対応のある部品だけ、アプリの設定、DLL の参照の付け替え。保存バーはここだけに出し、先へ進むときに保存する。
- ③ 変換・ビルド: 変換の設定(`--build-original`)と実行、変換レポート(節ごとに開く)。
- ④ デプロイ: ZIP、ネイティブ起動、コンテナ起動、ECR 発行(予定)。
  - ネイティブ起動は、変換の出力のサイト(Dockerfile の COPY 元)で `dotnet bin/<web>.dll --urls ...` を起動する(deploy/start.sh と同じ)。終了コード 75 なら起動し直す。
  - 変換をやり直すときは、先にネイティブ起動を止める(出力のファイルを掴まない)。
- Studio が起動したプロセスは、Windows のジョブ(閉じると中を終わらせる)に入れる。Studio が強制終了されても、アプリ、変換器、ビルド、docker のコマンドが残らない。
  - IIS に作ったサイトは、次の起動で消す。
- 確認(be、Studio の API と、ヘッドレスの Edge で 4 手順の画面):
  - 元のアプリをビルドして IIS で起動し、200 を返した。止めるとサイトが消えた。
  - 変換、ネイティブ起動も 200 を返した。
  - Studio を強制終了すると、ネイティブのアプリが止まり、IIS のサイトは次の起動で消えた。

### System.Data.SqlClient の移植(`|DataDirectory|`、2026-10-02)

DNN のインストール ウィザードがネイティブ起動で 500 になった(`Invalid value for key 'attachdbfilename'`)。

- 原因: DNN はコードで `"|DataDirectory|" + ファイル名` の接続文字列を組み立てる。.NET の System.Data.SqlClient はこれを拒否する。
- .NET Framework の System.Data.SqlClient は、先頭の `|DataDirectory|` を AppDomain の "DataDirectory"(ASP.NET では App_Data)、無ければ BaseDirectory に展開していた。
- 構成の接続文字列は以前からランタイムが展開している(0013)。コードで組み立てたもの、Entity Framework、DLL が開くものには効かない。

対応: System.Data.SqlClient を移植した(`FrameworkOnCore.Runtime/src/WebFormsForCore.Data.SqlClient`、パッケージ `FrameworkOnCore.Data.SqlClient`)。

- ソース: dotnet/maintenance-packages の 4.9.0 パッケージのコミット(MIT)。System.Drawing.Common と同じく、Windows(ネイティブの SNI)と Unix(マネージドの SNI)を別々にビルドし、一つのパッケージ(runtimes/win、runtimes/unix)に入れる。
- 変更は 1 か所だけ: `SqlConnectionString` が AttachDBFilename の先頭の `|DataDirectory|` を .NET Framework と同じ手順で展開する(`Core/DbConnectionOptions.DataDirectory.cs`)。
  - `..` でフォルダーの外に出るパスは拒否する。
  - 先頭以外の `|DataDirectory|` は、.NET Framework と同じく拒否する。
- アセンブリ: Microsoft の名前と公開鍵(b03f5f7f11d50a3a)、版は 4.6.2.0。Entity Framework 6 が依存で持ち込む Microsoft のパッケージ(4.6.1.6)より上なので、移植版が使われる。
  - wt のホストのトレースで確かめた: TPA に入るのは移植版の `runtimes/win/lib/net10.0` で、Microsoft の方は使われない。
- 変換器: Web プロジェクトに常に付ける(`webPackages`)。接続を開くのがソース、Entity Framework、DLL のどれでも、移植版が使われる。
  - `System.Data.SqlClient` のパッケージ参照とソースの使用(`sourcePackages`)も、移植版に置き換える。
- LINQ to SQL とグラフの移植も、移植版を参照する。版を 1.6.5-w2l.11 に上げた。
- テスト: `tests/SqlClientTests`。
  - maintenance-packages の機能テスト(TDS のテスト サーバーに接続する)を、そのまま移植版に対して動かす。
  - `|DataDirectory|` のテスト 6 件を足した: DataDirectory への展開、区切りの重複、BaseDirectory、`..` の拒否、普通のパス、先頭以外の拒否。
  - `[PlatformSpecific]` のテストは、dotnet のテストの実行と同じく、ほかの OS では除外する(名前付きパイプ、Windows 認証)。
  - Linux のテストは `run-tests-linux.ps1` と packages のワークフローで動かす。

確認:
- DNN のネイティブ起動: インストール ウィザードが 200 で、IIS と同じ。
- テスト: Windows では FrameworkOnCore.Tests 4,337 件と SqlClientTests 195 件。Linux では 4,318 件と 185 件。すべて緑(スキップは SQL Server が要るもの)。
- be: Windows・Linux とも 5/5。
- wt: Windows 6/8、Linux 5/8。以前と同じ既知の差だけ。

#### 移植版か .NET 版かの選択

部品ごとの選択に「SQL Server のクライアント(System.Data.SqlClient)」を足した。

- 選択肢は「移植版を使う」(既定)と「.NET 版を使う」(Microsoft の System.Data.SqlClient 4.9.0)。
- 解析の状態は「動きが違う」。
  - wt では、Entity Framework の DLL が使う API(21 件)で出る。
  - be はこの名前空間を使わないので出ない。出ない部品は既定の移植版になる。
- 変換器の規則 `portOptions`(`rules/packages.json`)は、移植版のパッケージごとに、その選択肢と .NET 版のパッケージを持つ。
  - 移植版を選ばなければ、規則が移植版を付ける所(`webPackages`、`replacedPackages`、`frameworkReferences`、`sourcePackages`)で .NET 版を付ける。
  - `webPackages` からは外す。
  - `|DataDirectory|` の報告(`sourceNotes`)も、選んだ方の文になる。
  - 今後の移植版も、ここに書けば同じく選べる。
- 確認: wt を「.NET 版」で変換すると、`deps.json` には Entity Framework が持ち込む Microsoft の 4.8.6 だけが残った。既定では今までと同じ。
- 気付いたこと: wt の Elmah.dll は、.NET Framework の `SqlConnectionStringBuilder.AsynchronousProcessing`(.NET Framework 4.5 から無視される)を使う。これは移植版にも .NET 版にも無く、使う所で MissingMethodException になる。移植版に足せる候補。
- 移植版の候補は `PORT-CANDIDATES.md` で管理する(見つけ方と、2026-10-02 時点の一覧)。

### WCF のサービスを CoreWCF で動かす(1.6.5-w2l.14、2026-10-03)

.NET の WCF はクライアントだけで、サービス(.svc)は動かなかった。部品 `wcf-server` の選択肢「CoreWCF で動かす」を作り、既定にした。

- ランタイム: パッケージ FrameworkOnCore.ServiceModel(`FrameworkOnCore.Runtime/src/WebFormsForCore.ServiceModel`)。
  - アセンブリの名前は .NET Framework の System.ServiceModel.Activation(公開鍵トークン 31bf3856ad364e35)。CoreWCF に無い `AspNetCompatibilityRequirementsAttribute` を持つ。
  - `AddWebFormsServiceModel()` と `UseWebFormsServiceModel()`(Web Forms より前)が、IIS の WCF の起動と同じくサービスを見つけて CoreWCF に登録する。
    - サイトの .svc(`<%@ ServiceHost Service=... %>`、アドレスはファイルの場所)と、web.config の serviceActivations(ファイルの無いサービス)。
    - web.config の services のエンドポイント(アドレス、バインド、bindingConfiguration、behaviorConfiguration)。無ければ WCF 4 の既定のエンドポイント(実装している契約ごと、protocolMapping のバインド。既定は basicHttpBinding)。WebServiceHostFactory は webHttpBinding。
    - バインド: basicHttpBinding、basicHttpsBinding、wsHttpBinding、webHttpBinding。設定の属性(maxReceivedMessageSize など)と readerQuotas、security の mode。
    - 動作: serviceMetadata(WSDL。CoreWCF では全サービスに一つの設定)、serviceDebug、webHttp。名前の無い動作は既定の動作。
    - 対応しないもの(起動時のログに出す): netTcpBinding など、mex のエンドポイント、独自の ServiceHostFactory(使わずに web.config のとおりに動かす)、aspNetCompatibilityEnabled(操作の中に ASP.NET の文脈が無い)。
    - サービスの見つからない .svc は 404(ファイルを返さない)。
  - サービスの契約は、変換後のソースでは .NET の WCF クライアントの属性(ServiceContract、OperationContract、FaultContract など)のまま。CoreWCF はそれをそのまま受け付ける。
  - クライアントの型の `FaultException`(`FaultException<T>`)は CoreWCF が知らない型で、内部エラーになっていた。エラーハンドラーで CoreWCF の障害(理由、コード、詳細)に変えて返す。
- 変換器:
  - 名前空間と型の移動を、選択ごとのグループ(`moveGroups`)にした。EF4 → EF6 と、WCF のサービス側 → CoreWCF。
    - `System.ServiceModel.Web` → `CoreWCF.Web`(WebGet、WebInvoke、WebMessageFormat など)。
    - `ServiceBehavior`、`OperationBehavior`、`InstanceContextMode`、`ConcurrencyMode` など → CoreWCF の型(using の別名)。
    - 両方にある型(クライアントの `System.ServiceModel.ConcurrencyMode` と CoreWCF の)は、型の不一致(CS0266、CS0029、CS1503)から CoreWCF の方の別名を足す。以前はこの属性ごと消していた。
  - .svc か serviceActivations のある Web プロジェクトに FrameworkOnCore.ServiceModel を付け、Program.cs で呼ぶ。`System.ServiceModel.Activation` の参照もこのパッケージにする。
- サンプル `samples/WcfProbe`: SOAP(足し算、FaultContract の障害、DataContract の注文、InstanceContextMode.Single)、ファイルの無いサービスと既定のエンドポイント、REST(JSON と XML、JSON の POST)、WSDL。ページから WCF のクライアントで呼ぶ。
  - IIS で正解を採った(Windows の機能 WCF-HTTP-Activation45 を有効にした)。Windows・Linux とも 4/4 とリンク 3 件が一致。
  - Linux のコンテナでは、ページが自分のサービスを呼ぶ先を `APPSETTING_ServiceBase`(`samples/WcfProbe/linux.env`、`sample-parity.ps1` が渡す)で与える。ポートの対応(5096 → 8080)の内側と外側でポートが違うため。
  - 気付いたこと: ランタイムの `SERVER_PORT` は Host ヘッダーのポート(IIS は接続のポート)。ポートの対応やリバースプロキシの後ろで、古いアプリが SERVER_PORT からリンクを作っても壊れないように、意図して変えたもの(w2l.10)。そのままにした。
- 確認: テスト Windows 4,348 件・Linux 4,329 件、be Windows 5/5、wt Windows 6/8(既知の差)。

#### ASP.NET AJAX から呼ぶ WCF のサービス(enableWebScript、1.6.5-w2l.15)

Visual Studio の「AJAX 対応 WCF サービス」(webHttpBinding と `<enableWebScript/>`)を、ScriptManager の ServiceReference から呼べるようにした。Web Forms のアプリでよく使われる形。CoreWCF にはこの動作が無い。

- .NET Framework の System.ServiceModel.Web(referencesource)の WebScriptEnablingBehavior を、CoreWCF の WebHttpBehavior の上に移植した。
  - 要求は JSON で、引数を名前で包む(WrappedRequest)。GET の引数は JSON の値(JsonQueryStringConverter を移植)。
  - 応答は `{"d": 結果}` で、オブジェクトには `__type` が付く。void は `{"d":null}`。CoreWCF の JSON の整形には useAspNetAjaxJson があるが、"d" で包む部分が抜けていたので、自前の整形(AspNetAjaxReplyFormatter)で書く。
  - エラーは 500、`jsonerror: true`、本文は JsonFaultDetail(includeExceptionDetailInFaults のときは例外の型、メッセージ、スタック)。
  - CoreWCF は webHttpBinding のエンドポイントに WebHttpBehavior も自動で付け、そのエラー処理(400 の HTML)が後から上書きしていた。enableWebScript の動作がそれを外す。
- スクリプトのプロキシ(`Service.svc/js`、`/jsdebug`): .NET Framework の WCFServiceClientProxyGenerator を移植した。ランタイムの System.Web.Extensions の ClientProxyGenerator(内部の型。System.ServiceModel.Activation に InternalsVisibleTo)で書く。契約は型から読む(ServiceContract の名前と名前空間、操作、引数、WebGet は GET)。
- WebScriptServiceHostFactory も、enableWebScript の既定のエンドポイントにする。
- サンプル `samples/WcfProbe` に Ajax.svc(テンプレートと同じく、契約はクラスに付け、名前空間は空)と Ajax.aspx(ScriptManager から全部の操作を呼ぶ。生の応答も表示する)を足した。
  - IIS の .NET Framework の結果と比べて、Windows・Linux とも 7/7(ページ、`/js` と `/jsdebug` のスクリプトの全文)とリンク 6 件が一致した。
  - WCF の「内部エラー」のメッセージは、サーバーの OS の言語(この PC では日本語)になる。そのため、このサービスでは includeExceptionDetailInFaults を有効にして、例外のメッセージと型で比べる。スタックはサーバーのパスと行番号なので、比べない。
- 確認: テスト Windows 4,348 件・Linux 4,329 件と SqlClientTests 204 件、be Windows 5/5、wt Windows 6/8(既知の差)。

#### SqlClient の残りの小さな違い(1.6.5-w2l.13)

- `TransparentNetworkIPResolution`(.NET Framework 4.6.1 の接続文字列のキーワードとビルダーのプロパティ): 受け付ける。接続の仕方は .NET と同じ(.NET Framework の「最初の IP を短く試す」には SNI の変更が要る)。
- `SqlParameterCollection.Add(string, object)`(.NET Framework 2.0 から廃止予定。AddWithValue と同じ): 足した。
- `System.Data.Sql.SqlDataSourceEnumerator` と `SqlClientFactory.CreateDataSourceEnumerator`: 足した。
  - .NET Framework はネイティブの SNI に問い合わせていた。移植版は、その SNI と同じく SQL Server Browser にブロードキャストで問い合わせる(SSRP、UDP 1434)。表の形は .NET Framework と同じ。
  - この PC では、.NET Framework の方は 36 秒かけて 0 件、移植版は 2 秒で SQLEXPRESS を返した。
  - テストは、テストの中の SQL Server Browser の代わりが答える(ポート 1434 が空いているとき。Linux のコンテナで動くことを確かめた)。
- 確認: テスト Windows(FrameworkOnCore.Tests 4,345、SqlClientTests 214)・Linux(4,326、204)が緑。be Windows 5/5、wt Windows 6/8(既知の差)。

#### .NET が拒否する接続文字列のキーワード(1.6.5-w2l.12)

候補のうち優先度の高い 2 件を、移植版に足した。.NET Framework 4.8(referencesource)と同じ動きにしている。

- 接続文字列: `Asynchronous Processing`(`async`)と `Connection Reset` は、true か false かを確かめるだけで使わない(4.5 からそうだった)。
  - `Network Library`(`net`、`network`)は、ライブラリの名前(`dbmssocn` など)をプロトコル(`tcp:`、`np:`、`lpc:`)にして、サーバー名に付ける。
  - どれも、正しくない値は .NET Framework と同じく拒否する。
  - `Context Connection=true`(SQL CLR の中の接続)は、無いので例外のまま。
- `SqlConnectionStringBuilder` に `AsynchronousProcessing`・`ConnectionReset`(廃止予定の印も同じ)・`NetworkLibrary`・`ContextConnection` を足した。wt の Elmah.dll が使う `AsynchronousProcessing` は、変換レポートの「無いメンバー」から消えた。
- テスト: `tests/SqlClientTests/FrameworkOnCore/FrameworkKeywordTests.cs`。
  - 各キーワードを付けて TDS のテスト サーバーに接続する。`Network Library=DBMSSOCN` は tcp で接続できる。
  - 正しくない値は拒否する。ビルダーのプロパティも確かめる。
- 確認:
  - テスト: Windows では SqlClientTests 208 件と FrameworkOnCore.Tests 4,338 件、Linux では 198 件と 4,319 件。すべて緑。
  - be: Windows 5/5。
  - wt: Windows 6/8、Linux 5/8(以前と同じ既知の差だけ)。

### コンポーネント単位の新旧比較(TextBox、2026-10-10)

- 目的: 不特定のユーザーのシステムを動かすため、コーパスで使われている範囲ではなく、コントロールごとに全メンバー・全プロパティ・関わる機構をすべて比べる。仕組みは `tests/WebControlsParity/README.md`。
- 3 層:
  - ① API(`tests/WebControlsParity`): TextBox と基底クラス・実装インターフェースの 179 メンバーを TextBox のインスタンスで呼ぶ。170 ケース。
  - ② HTML(`samples/WebControlsProbe` の `RenderProbe.aspx`): 設定できる 40 プロパティの全サンプル値、属性・スタイル、相互作用 24、名前付けコンテナ・テーマの文脈。1 ケース 1 リクエストで描画したページ全体と、保存された ViewState の中身。214 ケース。
  - ③ シナリオ(`TextBoxScenario.aspx`): ポスト値、TextChanged、ReadOnly・Enabled、ViewState、AutoPostBack、検証グループ、ブラウザ側の検証、Bind、リクエスト検証。13 スナップショット。
- 結果は `tests/WebControlsParity/reports/TextBox/`(cases.md、results-windows.md、results-linux.md)。**Windows・Linux とも 3 層すべて一致**(既知の差分なし)。
- 比較が差分を見落とさないことは、ポートの出力を書き換えて確かめた(属性の値、API の値、シナリオの失敗がそれぞれ ❌ になり、既知の差分は ⚠️、使われない既知の差分は「古い」と出る)。
- 途中で分かったこと:
  - ポートのページのパーサーも、コードビハインドのフィールドとコントロールの ID が大文字小文字だけ違うときに .NET Framework と同じく「Ambiguous match found」を出す(メッセージはポートの方が詳しい)。
  - ポートの System.Web は、ホストの外(コンソール)では machine.config が無いと `system.web/pages` を読めず ClientID などが例外になる。Web アプリではホストが書くので問題にならない。① では MobileParity と同じく書いてから動かす。
  - .NET Framework の MSBuild は BOM の無い UTF-8 のソースを ANSI として読む。プローブサイトのソースは BOM つきにした。
- ParityTest に操作 `press`(キー。Tab で AutoPostBack の change を起こす)と `setvalue`(スクリプトで値を入れる。readonly でも)を足した。

### コンポーネント単位の新旧比較(Button、2026-10-10)

- ① API 171 ケース(180 メンバー)、② HTML 167 ケース(37 プロパティ、相互作用 21、名前付けコンテナ・テーマ)、③ シナリオ 15 スナップショット。結果は `tests/WebControlsParity/reports/Button/`。**Windows・Linux とも 3 層すべて一致**(既知の差分なし)。
- 機構マトリクスに IButtonControl の機構を足した(コマンドとバブル、UseSubmitBehavior、OnClientClick、PostBackUrl、DefaultButton)。型から判定するので、LinkButton・ImageButton にもそのまま当てはまる。
- 最初の比較で PostBackUrl の 10 ケースが違った。`__PREVIOUSPAGE`(ページのパスをマシンキーで暗号化した値)で、`__VIEWSTATE` と同じくマシンキーの違い。描画の比較でマスクする値に加えた(移動先が PreviousPage と入力値を受け取れることは ③ で一致)。ほかにマシンキーで変わる描画の値は無い(`__VIEWSTATEENCRYPTED` は値が常に空)。

### コンポーネント単位の新旧比較(ListControl 系、2026-10-10)

- DropDownList・ListBox・CheckBoxList・RadioButtonList・BulletedList。**Windows・Linux とも 5 つすべて 3 層一致**(既知の差分なし)。結果は `tests/WebControlsParity/reports/<名前>/`。

| コンポーネント | ① API | ② HTML | ③ シナリオ |
|---|---|---|---|
| DropDownList | 234 | 207 | 11 |
| ListBox | 234 | 214 | 12 |
| CheckBoxList | 250 | 233 | 11 |
| RadioButtonList | 249 | 233 | 11 |
| BulletedList | 243 | 237 | 6 |

- 仕組みに足したもの: 項目(`Items[1].Selected` など)のケースを型から作る、`[data-bound]` の文脈(DataSource と DataBind)、機構(項目、複数選択、繰り返しレイアウト、ポスト値のイベント検証)、ParityTest の操作 `selectmany`・`evaluate`(要素にスクリプト。改ざんしたポストを作る)、parity.ps1 はシナリオの最初の goto からページを決める(5 つで 1 ページを共有)。
- API 層の見本のポストデータを、コンポーネント自身の ID と(リストなら)項目の値にした(以前は txtName 固定でリストには無関係だった)。TextBox・Button の正解は変わらない。
- 検証対象と双方向バインドの機構は IPostBackDataHandler を条件に加えた(BulletedList は ListControl から属性を継ぐが値をポストしない)。CheckBoxList は ValidationProperty を持たず、RequiredFieldValidator を向けると HttpException になる(.NET Framework で確認)。
- .NET Framework の改ざんへの応じ方: 一覧にない値は DropDownList・ListBox が例外、RadioButtonList は無視。無効な項目のチェックは CheckBoxList が無視。無効な弾丸のポストバックは BulletedList が例外。ポートも同じ。
- 描画ページ・ケースの生成を変えた後、TextBox・Button も Windows で再確認して一致。

### コンポーネント単位の新旧比較(基本コントロール 11、2026-10-10)

- 進める順番を、コーパスでの使用(マークアップのタグ数とコードからの参照、`tests/WebControlsParity/control-usage.py`)で決めた。上位から Label、CheckBox、Literal、検証コントロール、LinkButton、HyperLink、GridView、Panel、HiddenField、Repeater、PlaceHolder、Image、FileUpload、ImageButton…。
- Label・CheckBox・RadioButton・Literal・LinkButton・HyperLink・Image・ImageButton・Panel・HiddenField・PlaceHolder。**Windows・Linux とも 11 すべて 3 層一致**(既知の差分なし)。

| コンポーネント | ① API | ② HTML | ③ シナリオ |
|---|---|---|---|
| Label | 154 | 131 | 3 |
| CheckBox | 167 | 146 | 9 |
| RadioButton | 172 | 153 | 9 |
| Literal | 112 | 48 | 3 |
| LinkButton | 173 | 157 | 9 |
| HyperLink | 156 | 152 | 4 |
| Image | 155 | 149 | 3 |
| ImageButton | 188 | 179 | 10 |
| Panel | 157 | 151 | 4 |
| HiddenField | 118 | 41 | 6 |
| PlaceHolder | 106 | 36 | 4 |

- 仕組みに足したもの: 属性・スタイルのケースはその型が持つときだけ、テーマの文脈と機構はテーマを付けられる型だけ(Literal・HiddenField・PlaceHolder のスキンはテーマ全体のパースエラーになる)、`InputAttributes[...]` のようなキーつきの属性コレクション、ケースに要る隣のコントロール(Label の AssociatedControlID の先)、ImageButton・HyperLink・Image 用の 20×10 の画像。
- ケースの生成の不具合: 画像の URL の見本を static フィールドにしていて、それより前に初期化される一覧(All)から読むと null だった(HyperLink のケース一覧が 500)。プロパティにした。
- 基本コントロールのスキンを足したら、CheckBoxList・RadioButtonList の `[themed]` が不一致になった。項目を描く中の CheckBox・RadioButton に既定のスキンが効く(項目が span で囲まれる)ためで、.NET Framework も同じに描く。2 つを採り直し、Windows・Linux とも一致。README に「スキンを足したら、それを中で使うコンポーネントを採り直す」と書いた。
- 既存の 7 つ(TextBox・Button・ListControl 系)も Windows で再確認して一致。

### コンポーネント単位の新旧比較(検証コントロール 6、2026-10-10)

- RequiredFieldValidator・RegularExpressionValidator・CompareValidator・CustomValidator・RangeValidator・ValidationSummary。**Windows・Linux とも 6 すべて 3 層一致**(既知の差分なし)。

| コンポーネント | ① API | ② HTML | ③ シナリオ |
|---|---|---|---|
| RequiredFieldValidator | 191 | 164 | 7 |
| RegularExpressionValidator | 192 | 168 | 8 |
| CompareValidator | 205 | 194 | 6 |
| CustomValidator | 195 | 169 | 7 |
| RangeValidator | 204 | 188 | 7 |
| ValidationSummary | 159 | 148 | 6 |

- 仕組みに足したもの: API 層のサンプルページにも隣のコントロール(検証対象のテキストボックス)、機構(サーバー側の検証、ブラウザ側の検証スクリプト、検証結果のまとめ)。
- ② の 200 以外は .NET Framework の例外(Type に変換できない ValueToCompare・MinimumValue・MaximumValue、見つからない ControlToValidate・ControlToCompare、空の ControlToValidate、MaximumValue < MinimumValue)。ポートも同じ例外とメッセージ。
- 手で書いた相互作用が、型から作る単独のケースと同じ名前になると、ケースの一覧が作られない(重複の検査)。ControlToValidate=other などを相互作用から外した。

### コンポーネント単位の新旧比較(GridView・Repeater、2026-10-10)

- **Windows・Linux とも 2 つとも 3 層一致**(既知の差分なし)。GridView: ① 347、② 408、③ 20。Repeater: ① 150、② 57、③ 7。
- 仕組みに足したもの: データ表示コントロールの全ケースをデータにバインド(7 件)、文脈ごとの準備(列、テンプレート、空のデータ)、コードで作るテンプレート(ProbeTemplate)、スタイルのサブオブジェクトとページャー設定のケース、機構(ページング、並べ替え、編集、選択、行のコマンド、データキー、DataSourceID)、セッションの ProductStore(ObjectDataSource の型)、ParityTest の操作 `clicktextin`(コントロールの中のリンクやボタンを文字で押す)。
- マシンキーで変わる値を 2 つ足した: API 層の GridView.GetCallbackResult の状態(コンソールでは実行ごとに鍵が変わり、.NET Framework でも毎回違う)、コールバックの GridView の並べ替え式(スクリプトの sortExpression とコールバックの引数)。
- **見つけた非互換(セッション)**: シナリオのデータ(Serializable でないクラスのリスト)をセッションに置くと、ポートでは「Unable to serialize the session state」の 500 になった。IIS の既定(InProc)はオブジェクトをそのまま持つので直列化しない。変換器の Program.cs が必ず `UseAspNetCoreSessionProvider()`(ASP.NET Core のセッションにバイト列で保存)にするため。試しに外すと、ポートの InProc(System.Web のメモリ上の保存)で動き、クッキーも IIS と同じ ASP.NET_SessionId だった。元の web.config が InProc(既定)なら InProc にするのが互換。変換器の変更になるので、判断を待つ。コントロールの比較は、データのクラスを Serializable にして続けた。**2026-10-11 に変換器で対応(下の「セッションの状態を web.config どおりに」)**。

### コンポーネント単位の新旧比較(FormView・DetailsView・ListView・DataPager・DataList・DataGrid、2026-10-11)

- **Windows・Linux とも 6 すべて 3 層一致**。DetailsView の ② に既知の差分 1(下)。

| コンポーネント | ① API | ② HTML | ③ シナリオ |
|---|---|---|---|
| FormView | 309 | 320 | 15 |
| DetailsView | 327 | 351 | 15 |
| ListView | 335 | 186 | 18 |
| DataPager | 137 | 60 | 8 |
| DataList | 246 | 256 | 9 |
| DataGrid | 245 | 266 | 12 |

- 仕組み: System.Web.Extensions の型(ListView・DataPager)も扱うようにした(① の API・状態の書き出し・② のプロパティ)。ListView のレイアウトとグループのテンプレート(プレースホルダー)、DataPager の隣の ListView と ObjectDataSource、機構「項目の挿入」、ページングと並べ替えの判定に ListView・DataPager。
- 既知の差分(DetailsView): 挿入モードでデータが無いと、両方とも同じ所で ArgumentNullException(引数 c)を投げる。メッセージは .NET Framework の ArrayList が "Collection cannot be null."、.NET が "Value cannot be null."(ランタイムの文言で、System.Web のものではない)。known-differences.json の最初の項目。
- レポートの不具合を直した: 既知の差分の「古い」の判定が、ほかのコンポーネントの項目まで出していた。そのコンポーネントのケースに当てはまる項目だけにした。ポートの出力をコンポーネントごとに残すようにした(windows/<名前>)。
- 共通の生成器と描画ページを変えたので、32 コンポーネントすべてを Windows で再確認して一致。

### セッションの状態を web.config どおりに(2026-10-11、ユーザーの指示)

GridView の比較で見つけた非互換(上の「見つけた非互換(セッション)」)を変換器で直し、同種の問題を調べ、全コーパスで試験した。

- **原因**: 変換器の Program.cs が、どのアプリでも `AddSession` + `UseSession` + `UseWebForms(o => o.UseAspNetCoreSessionProvider())`(ASP.NET Core のセッションにバイト列で保存。要求ごとに BinaryFormatter で直列化)を書いていた。IIS の既定(InProc)はオブジェクトをそのまま持つので、Serializable でないオブジェクトを置くアプリは 500 になり、置いたオブジェクトをその場で変えても(`((Cart)Session["cart"]).Add(...)`)IIS と同じに見えるのは直列化できる場合だけだった。Session_End も起きなかった。
- **修正**(`HostProgram`、変換器): web.config の `<sessionState mode>`(`<location path=".">` の中も。無ければ ASP.NET の既定の InProc)で決める。
  - InProc・Off・Custom: System.Web 自身のセッション(IIS と同じ)。ASP.NET Core のセッション(`AddDistributedMemoryCache`・`AddSession`・`UseSession`)は書かず、`app.UseWebForms()`。InProc はオブジェクトをそのまま持ち、Session_End が起き、クッキーは ASP.NET_SessionId。Custom はアプリのプロバイダー(WingtipToys の Universal Providers の `DefaultSessionStateProvider`)をそのまま使う。以前は Custom も ASP.NET Core のセッションに置き換えていた。
  - StateServer・SQLServer: 変換後のアプリにはそのサーバーが無いので、従来どおり ASP.NET Core のセッション(どちらも直列化するので振る舞いは同じ)。web.config の `timeout` を `IdleTimeout` に渡す(以前は ASP.NET Core の既定の 20 分のまま)。
  - レポートに「session state: ...」を出す。WCF のサービス(`AddWebFormsServiceModel`)の挿入位置は `AddSession` の後ろから `builder.Build()` の前に変えた(`AddSession` が無い場合があるため)。
  - Program.cs の生成を `HostProgram`(純粋な関数)に切り出し、`HostProgramTests` 30 件(C#・VB × InProc・Off・Custom・StateServer・SQLServer × ルーティング × WCF、location、名前空間、コメントアウト、timeout)。旧スクリプト `convert-project.ps1` も同じ判定にした。
- **確認**(`samples/SessionProbe`、IIS で 14 スナップショットを記録): Serializable でないオブジェクトを置く、次の要求で同じもの、その場の変更が残る、読み取り専用ページ(`EnableSessionState="ReadOnly"`)の書き込み、セッションなしのページ、`IRequiresSessionState` のあるハンドラー・無いハンドラー、Remove・Clear、Abandon と Session_End、Abandon 後の新しいセッション。直す前は Serializable でないオブジェクトを置いた所から 500。直した後は **Windows・Linux とも 14/14 一致**。
- **同種の問題の調査**:
  - ホストが System.Web の仕組みを ASP.NET Core のものに差し替えているのは、テンプレートと `WebFormsOptions` を見る限りセッション(`UseAspNetCoreSessionProvider` の `ProviderOverride`)だけ。
  - ASP.NET Core のセッションの経路(StateServer・SQLServer)の違い: `timeout` を無視していた(直した)。ロックしない(`GetItemExclusive` が locked=false。同じセッションの同時の要求を IIS は順に処理する)。クッキーが 2 つ(System.Web の ASP.NET_SessionId と ASP.NET Core の .AspNetCore.Session)。コーパスにこのモードを使うものは無い(mojo の AppFabric、nop390 の Redis はコメントアウト)。
  - マシンキーの自動生成(ViewState・フォーム認証)は FrameworkOnCore が `LocalApplicationData/WebFormsForCore/AutogenKeys` に保存し、再起動をまたいで同じ(問題なし)。
  - コーパスの判定: be・mojo・yaf・dnn・imis・nop・mvcmovie・nop390 は InProc、wt は Custom、n2 は Off。
- **全コーパスの試験**(10 本を変換し直し、DB は空から作り直してインストーラーを通した。Windows は `.\SQLEXPRESS`、Linux は SQL Server のコンテナ):

| コーパス | Windows | Linux |
|---|---|---|
| be | 5/5 | 5/5 |
| wt | 6/8(既知の丸めの差 2)。セッションは Universal Providers が DB の Sessions テーブルに保存(IIS と同じ) | 5/8(同じ 2 と error-page。以前と同じ)。Sessions テーブルはコンテナの DB に |
| mvcmovie | 11/12(既知の言語パックの差) | 11/12 |
| mojo | 空の DB からセットアップ(105 テーブル)、トップ・ログイン・サイトマップ 200、管理画面はログインへ | 同じ(`/secure/LOGIN.aspx` も) |
| yaf | インストーラーは ServiceStack のスタブ(以前と同じ) | 同じ |
| dnn | インストール完了、トップ・`/Login`・`/Terms`・`/Privacy` 200 | 同じ。ページの資源 21 件すべて 200 |
| n2 | インストーラー完了(セッション Off)、トップがコンテンツ付き、`/N2/` はログインへ | 同じ |
| imis | ログインし、主な画面 7 つが 200 | 同じ |
| nop(1.90) | インストール、店頭 14 画面・商品の詳細・サムネイル・ログイン・管理画面 8 つ・アカウント 200 | 同じ(サムネイルは libgdiplus) |
| nop390 | インストール、店頭 19 画面・商品画像・カートへの追加(AJAX)・管理画面 7 つと一覧の JSON 200 | 同じ |

サンプル(WcfProbe・OrderAdmin・ProductAdmin・DefaultsProbe・MasterProbe・MobileProbe・RuntimeProbe・ToolkitProbe)は Windows・Linux とも全一致。コンポーネント単位の比較のうちセッションを使うデータ表示 7 つ(GridView・DetailsView・FormView・ListView・DataList・DataGrid・Repeater)も Windows・Linux とも一致。テスト Windows 4,402 件・Linux 4,381 件(スキップ 1・22)がすべて緑。

- 検証の手順の不具合を直した: `convert-corpora.ps1` の DNN の接続文字列の書き換えが、改行を含む `<add>` に当たらず、コメントの中の例だけを書き換えていた(DNN は App_Data の Database.mdf にインストールされていた)。空白を `\s+` にした(imis も)。
- 気をつけること: nopCommerce 3.90 のサンプルデータの Apple MacBook Pro は最小数量 2(数量 1 のカートへの追加は断られる)。
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

### 残りのコントロールすべての新旧比較(2026-10-11、ユーザーの指示)

コンポーネント単位の新旧比較(`tests/WebControlsParity`)を、残りのコントロールすべてに広げた。System.Web と System.Web.Extensions の、アプリが置くコントロール 124
(前回までの 32 に、HtmlControls 33、Table 系 6、FileUpload、MultiView・View、Localize、Substitution、Xml、AdRotator、Calendar、ImageMap、LiteralControl、
Content・ContentPlaceHolder、ScriptManager・ScriptManagerProxy・UpdatePanel・UpdateProgress・Timer、TreeView・Menu・SiteMapPath・SiteMapDataSource、ログイン系 7、
Wizard とステップ 5、データソース 5・QueryExtender・ModelErrorMessage、WebParts 13)。対象外は Page・UserControl・MasterPage・Control(アプリが派生させる基底クラス)と
DesignerDataBoundLiteralControl(デザイナー用)。

- **結果: 124 すべてが Windows・Linux とも 3 層一致**(既知の差分 8 件を除く。`tests/WebControlsParity/README.md` の「既知の差分」)。
- 仕組みに足したこと: `parity.ps1 -Control A,B,C`(サイトの構築・変換・コンテナを 1 回で)。ページの designer.cs と csproj の登録を作る `scenario-page.py`、
  共有ページのシナリオを型の機構から作る `html-scenarios.py`・`more-scenarios.py`。親が要るコンポーネント(View、WizardStep、AJAX、Web パーツ)の `Wrap`、
  HtmlForm・HtmlHead をページのフォーム・ヘッダーにする描画、DateTime のサンプル値。ParityTest の `upload`(ファイルの送信)と `maskIds`(乱数の ID を伏せる)。
  プローブサイトに、メモリ上のメンバーシップ・ロール(`ProbeMembership.cs`)、ADO.NET プロバイダー(`ProbeDb.cs`。SqlDataSource をデータベース無しで)、
  Web パーツのパーソナライズ(`ProbePersonalization.cs`)、`Web.sitemap`、マスターページ。
- 比較の仕組みの修正: スクリプトの文字列の中のリソースの URL とサイトのフォルダーを伏せる、StateBag の項目の並びをそろえる(.NET の文字列のハッシュのランダム化)、
  DictionaryEntry をキーと値で書く、API 層の .NET Framework 側にルート web.config のサイトマップ・Web パーツの設定(`App.config`)、マシンキー・乱数の値を伏せる。
- **見つけて直したポートの不具合**(FrameworkOnCore.Runtime、パッケージは 1.6.5-w2l.15 のまま作り直した。公開はしていない):
  - **LinqDataSource が .NET 版に無かった**(`#if NETFRAMEWORK`。LINQ to SQL が無かった頃のまま)。移植済みの System.Data.Linq を参照して net10.0 で有効にした
    (QueryExtender の対象にもなる)。
  - **SqlDataSource の `ProviderName="System.Data.SqlClient"`(Visual Studio が書く既定)が ArgumentException**。.NET の DbProviderFactories は構成を読まず、
    組み込みのプロバイダーも無い。最初の要求の初期化で、.NET Framework の組み込みの 3 つ(SqlClient・OleDb・Odbc)と web.config の
    `system.data/DbProviderFactories` を登録する(`DbProviderFactoriesConfiguration`)。BlogEngine の DB プロバイダー(`DbProviderFactories.GetFactory`)、
    n2 の `Fix.aspx`(SqlDataSource)も同じ所を通る。
  - **ScriptManager の CDN のフォールバックで `</script>` をエスケープしていなかった**(.NET Framework は `<\/script>`)。スクリプトの中の `</script>` で
    script 要素が途中で閉じる。ほかの同じ書き方(ClientScriptManager)は正しかった。
  - AccessDataSource が Linux で NullReferenceException(OLE DB が無い)。PlatformNotSupportedException で知らせるようにした。
- 既知の差分(ランタイム・環境の違い、`known-differences.json`): System.Web.Extensions の版(ScriptPath のフォルダー名、AjaxFrameworkAssembly)、パスに使えない文字の
  例外(ArgumentException と IOException)、パスの区切り(Linux)、コードアクセスセキュリティ(WebPartManager の権限セット)、Hashtable の順(トランスフォーマー)、
  OLE DB(Linux)。
- net8.0 は考慮しない(サポート終了が近い。変換後のアプリは net10.0)。ターゲットからも外した(下の節)。
- 検証: テスト Windows 4,402 件・Linux 4,381 件(スキップ 1・22)がすべて緑。be Windows 5/5、wt Windows 6/8(既知の丸めの差 2、以前と同じ)。

### net8.0 のターゲットを外した(2026-10-11、ユーザーの指示)

FrameworkOnCore.Runtime のプロジェクトから net8.0 を外した。パッケージは lib/net10.0 だけ(1.6.5-w2l.15 のまま作り直した。公開はしていない)。

- プロジェクト(22): `TargetFrameworks` から net8.0、net8.0 だけの設定(参照・パッケージ・定数)、net8.0 向けにパッケージへ入れていたファイル。
  WebFormsForCore.NativeDlls は net10.0 に。
- net8.0 のための分岐: LinqDataSource の `#if NET10_0_OR_GREATER` と System.Data.Linq・SqlClient の参照の条件、Compilers の Roslyn 4.13 のローダー
  (`#if NET8_0`)と `Roslyn.net8.0`(net8.0 用のコンパイラー一式)、FileChangesMonitor の .NET 8/9 の回避策、ビルドタスク(WebFormsForCore.Build)の
  net8.0 の WebFormsForCore.Build.NetCore(`CreateAspDesignerFiles`・`FakeStrongName` は net10.0 のものを使う)。
- `pack-frameworkoncore.ps1`・`frameworkoncore.slnx`: ビルドタスクのビルドから net8.0 を外し、CS7069 の注記を消した。Ajax Control Toolkit は引き続き
  net10.0 だけ(net48 は作らない)。
- 確認: パッケージ 21 個すべて net8.0 無し。FrameworkOnCore.Tests Windows 4,402 件(スキップ 1)・Linux 4,400 件(SQL Server のコンテナあり、スキップ 3)、
  SqlClientTests Windows 214 件・Linux 204 件が緑。WebControlsParity 124 コントロールすべて Windows・Linux とも一致(既知の差分を除く)。
  be Windows 5/5・Linux 5/5、wt Windows 6/8・Linux 5/8(既知の丸めの差 2 と Linux の error-page)。studio-verify(be・wt)は元・ネイティブ・コンテナとも
  リンク一致(14 件・61 件)。.NET Framework 版 MSBuild の経路: Build Tools 2022(MSBuild 17.14)は .NET 10 SDK を扱えない(MSBuild 18 以上が要る)ため、
  Build Tools 2026(MSBuild 18.10、winget の Microsoft.VisualStudio.BuildTools)を入れて WebFormsForCore.Configuration を net10.0 でビルドした。
  FakeStrongName が動き、厳密名(System.Drawing は b03f5f7f11d50a3a、System.Configuration.ConfigurationManager は cc7b13ffcd2ddd51、版 13.5.0.0)は
  dotnet build のパッケージと同じ。
  デザイナーファイルの生成(CreateAspDesignerFiles。ページ 1 つの net10.0 プロジェクトで build\WebFormsForCore.Build.targets を読み込んだ): 既定の
  別プロセス(net10.0 の WebFormsForCore.Build.NetCore を dotnet で起動)は dotnet build・MSBuild 18 とも生成でき、内容も同じ。既定でないプロセス内
  (`SeparateProcess="false"`、ターゲットは使わない)は以前から動かなかった(net8.0 とは無関係)ので直した(2026-10-12、ユーザーの指示):
  - `IsTargetNetFX` が文字列の比較で net10.0 を .NET Framework と判定していた("net10.0" < "net4999")。モニカーの形で判定する(点が無い net48 が .NET Framework)。
  - AssemblyResolve がアセンブリの表示名("System.Web, Version=...")をファイル名にし、見つからないものをすべてエラーにしていた。単純名で探し、無ければ他に任せる。
  - .NET の MSBuild のプロセスには共有フレームワークの System.Web(4.0.0.0、HttpUtility のファサード)があり、FrameworkOnCore の System.Web(10.0.29.0)を
    同じプロセスに読み込めない(MarkupReader の型初期化子で失敗)。プロセス内は .NET Framework の MSBuild で .NET Framework 向けの時だけにし、それ以外は別プロセス
    (ツールの deps.json が FrameworkOnCore のものを選ぶ)。成否を逆に返していたのも直した。
  - 確認: net10.0・net48 × dotnet build・MSBuild 18 × 別プロセス・プロセス内の 7 通りすべてで生成でき、内容は同じ。

### 違う観点の意地悪なコーパス: LegacyRenderProbe・ConfigProbe・LocaleProbe・DataFileProbe(1.6.5-w2l.18、2026-10-19、ユーザーの指示)

前のコーパス(コントロールの入れ子、BCL)と違う観点で、差が出そうなものを 4 つ作り、IIS の .NET Framework 4.8 で記録して変換後を Windows・Linux で
比べた(`sample-parity.ps1`。1 つずつ順に、Linux は Docker を 1 つずつ)。

- `samples/LegacyRenderProbe`(古い描画): web.config の `controlRenderingCompatibilityVersion="3.5"`・`xhtmlConformance mode="Legacy"`・`clientIDMode="AutoID"` で、
  Menu・TreeView・Calendar・Panel(GroupingText・DefaultButton)・検証コントロール・無効なコントロール・リスト・GridView(ページング・並べ替え・画像の ButtonField)・
  DetailsView・Wizard・Table・ImageButton。HTML を比べる(compareSource)14 スナップショット: Windows・Linux とも最初から一致。
- `samples/ConfigProbe`(構成): appSettings の file=、connectionStrings と独自セクションの configSource、machine.config の LocalSqlServer、NameValue・Dictionary・
  SingleTag の各ハンドラーと IConfigurationSectionHandler(コメント・CDATA を含むノード)、検証付きの ConfigurationSection とコレクション(remove・clear)、
  applicationSettings(Settings.Default)、location とフォルダーの web.config(de-DE、別の httpRuntime)、壊れたフォルダーの web.config、system.web の各セクション、
  OpenWebConfiguration。行の比較。
- `samples/LocaleProbe`(カルチャ): Culture="auto:en-US" UICulture="auto" を Accept-Language を変えながら(ParityTest の新しい手順 `acceptlanguage`: q 値は無視して
  先頭、未知の言語、`*`、空白)、App_GlobalResources・App_LocalResources の親カルチャへの戻り(zh-CHS・zh-CHT・zh-Hant・de-CH → de)、ページの値。
  InitializeCulture で ?c= のカルチャにしたフォーム: 仏暦・ヒジュラ暦・ペルシア暦の Calendar、検証コントロールのクライアントの値(小数点・桁区切り・日付の順)と
  通すもの、サーバーでの読み取り、バインドした値の書式。HTML を比べる 32 スナップショット。
- `samples/DataFileProbe`(データとファイル): DataTable・DataView の Locale と CaseSensitive による並べ替えと比較(かな・幅、トルコ語の I、ß、Æ)、RowFilter、
  式と Compute の double、主キーと Merge、DataSet の XML。Directory.GetFiles の順とパターン、別の大文字小文字のパス、Windows のパス、Shift_JIS・BOM・UTF-16 の
  ファイル、日本語のコードページのバイト(WAVE DASH など)、UTF-7。行の比較。

見つけて直したもの:

- **System.Configuration**(ConfigProbe): .NET Framework では System.dll にあったクラスのメッセージ 37 件がリソースに無く、AppSettingsReader の例外が
  `ArgumentNullException (Parameter 'format')` だった(.NET Framework のリソースから足した。ほかの移植のプロジェクトにリソースの欠けは無い)。
  `ConfigurationManagerInternalFactory` が無いアセンブリ名(WebFormsForCore.Configuration)で型を探していて、`Properties.Settings.Default` が動かなかった
  (同じ間違いが BrowserCapabilitiesFactory の Menu のアダプター〈Generic Downlevel〉にもあった)。接続文字列の `|DataDirectory|` を読むときに展開していた:
  自分で展開するプロバイダーには値のまま返す。
- **.NET Framework の言語パック**(LocaleProbe): 日本語の Windows の IIS では、UI カルチャが ja のとき Calendar の title が「カレンダー」などになる
  (System.Web の言語パック)。最初は変換器が Microsoft の言語パックからサテライトを作っていたが、その使用許諾(.NET Framework と言語パックの
  Supplemental License Terms 1.c)が Windows 以外で動かすための配布を禁じるので、やめた(ユーザーの決定)。代わりに、英語の原文から独自に訳した
  `translations/ja/System.Web.ja.resx`(System.Web の全 3,369 件。.NET Framework の日本語の文言より表示幅が広くならないように。GitHub で管理。`translations/check.ps1` と単体テストで検査)を FrameworkOnCore.Web の
  サテライトにし、変換器は元のサーバーに言語パックがあった言語の訳だけ残す(部品 `language-packs`: 既定は FrameworkOnCore の翻訳。Microsoft の言語パックは
  `--deploy none` で選んだときだけ)。サテライトの公開鍵を System.Web のものに書き換える処理が時刻を新しくして訳の変更が取り込まれなくなっていたので、時刻を戻す。
  文言は Microsoft の訳と違うことがある(「前の月を表示」と「先月へ移動」)ので、LocaleProbe の form-ja-JP は差として残る。
  使う言語は Studio の「アプリの設定」の「言語パック」(設定 `languages`)で選ぶ(ユーザーの指示): 英語(原文)と .NET Framework の言語パックの 23 言語から複数。
  既定は英語と元のサーバーに言語パックがあった言語。英語だけを選べば英語のみ(訳の無い言語は画面で「訳はまだ無い」と出し、変換レポートにも出す)。
  訳は System.Web のほか、パッケージにしている 12 アセンブリの全文言(System.Web.Extensions・DataVisualization・DynamicData・Mobile・Services・
  ApplicationServices・Optimization・WebGrease・ConfigurationManager・SqlClient・Drawing.Common・Serialization.Formatters の 4,061 件。合計 7,430 件)に広げた
  (ユーザーの指示「全部やりきってください」。2026-10-20)。置き場所は `translations/<言語>/<アセンブリ>/<リソース>.<言語>.resx`、一覧は `translations/sources.json`。
  各プロジェクトが `FrameworkOnCore.Runtime/src/FrameworkOnCore.Translations.targets` を読み込み、サテライトを作って本体と同じ公開鍵にする。
  幅の上限は .NET Framework の日本語の文言の表示幅、.NET Framework に無い文言(Optimization・WebGrease、FrameworkOnCore で増えたもの)は英語の幅(12 未満なら + 4)。
- **カルチャの名前と親**: Page.Culture・CultureInfo.DisplayName は .NET Framework では mscorlib の名前(「Chinese (Simplified, PRC)」、言語パックの言語ならその言語、
  最初に作った要求の UI カルチャのまま)。.NET は Windows の表示言語の名前(日本語の Windows では de-DE の UI でも「ドイツ語 (ドイツ)」)。FrameworkOnCore の
  System.Web と FrameworkOnCore.Compat の FrameworkCultures が、訳のある言語の UI カルチャでは .NET の名前、ほかは元のサーバーの英語名(カルチャ プロファイル)を
  返す(mscorlib の名前の表は Microsoft の言語パックを選んだ Windows 配置だけ)。
  zh-CN・zh-TW の親(.NET Framework は zh-CHS・zh-CHT)で、App_GlobalResources・App_LocalResources の Strings.zh-CHS.resx が見つからなかった: リソースの
  プロバイダーが .NET Framework の並びで探す(戻りながら探す ResourceManager は親のセットを子の名前で覚えるので、それ用の ResourceManager で 1 つずつ)。
  アプリの `CultureInfo.Parent`・`EnglishName`・`NativeName` も置き換える(platformReplacements がインスタンスのプロパティの読み取りも書き換えるようにした)。
- **Linux のカルチャのデータ**: ICU のデータに入らないもの(th-TH の週の始まり〈Calendar の列〉、ar-SA の短い日付、es-419 の XDR、fa-IR の通貨の桁、
  TwoDigitYearMax)を、FrameworkOnCore の System.Web が作るカルチャに App_Data\culture-profile.json から入れる(CultureOverlay)。capture-culture.ps1 は
  TwoDigitYearMax・EnglishName・NativeName・名前を挙げた中立カルチャも記録する(CultureIcu は中立カルチャの通貨で落ちていた)。
- **ファイルの一覧**(DataFileProbe): .NET Framework の `*.xls` は 8.3 名で book.xlsx も返す(`*.htm` の page.html、`*.` の .hidden)。Linux はフォルダーの持つ順で、
  パターンが大文字小文字を区別する(`*.txt` が readme.TXT を返さない)。FrameworkOnCore.Compat の FrameworkFiles に置き換える(Windows の順・パターン)。
  類似の箇所: FrameworkOnCore 自身の一覧(テーマの CSS・スキン、App_Browsers、bin の `*.dll`、バンドルの IncludeDirectory)も Linux で名前順・大文字小文字を
  区別しないようにした。Path.GetInvalidPathChars・GetInvalidFileNameChars は Windows の 36・41。
- UTF-7: ソースが名指しするときは `System.Text.Encoding.EnableUnsafeUTF7Encoding` を入れる。XmlConvert.ToString(double) は .NET Framework の R(17 桁)に。
- FrameworkOnCore の AppContext.BaseDirectory はサイトのルート(.NET Framework と同じ)なので、bin を見るもの(ServiceModel の型の検索)はアセンブリの場所を使う。

結果(Windows / Linux):

| コーパス | スナップショット | Windows | Linux |
|---|---|---|---|
| LegacyRenderProbe | 14 | 14/14 | 14/14 |
| ConfigProbe | 2 | 0/2 → 2/2 | 2/2 |
| LocaleProbe | 32 | 18/29(最初の 29 件)→ 29/32 | 18/32 → 28/32 |
| DataFileProbe | 2(140 行) | 10 行 → 5 行 | 28 行 → 25 行 |

残り(PORT-CANDIDATES.md の「移植では対応できないもの」): LocaleProbe の独自訳の文言(form-ja-JP)、mscorlib 独自の英語名(zh-CN の「PRC」、zh-HK の「S.A.R.」)、Linux の未知のカルチャ(xx-YY: Windows は「不明なロケール」、ICU は「xx (YY)」)。
DataFileProbe の System.Data の式の中の double と DataSet の XML の R(.NET の System.Data・System.Xml が書く)、Linux の照合(ICU)・改行・ドライブ付きの
Windows のパスのリテラル。回帰: サンプル 29 件(既知の差の BclProbe 115 → 113 行・DataFileProbe・LocaleProbe の Linux 1 件のほかは一致)、be 5/5(Windows・
Linux)、wt 8/8(Windows)・7/8(Linux、error-page は検証環境)。単体テスト 4,428 件。
### 差が出るように作ったコーパス: NestingProbe・BclProbe と double の文字列化(1.6.5-w2l.18、2026-10-19、ユーザーの指示)

変換器の互換性を上げるため、差が出そうなケースを集めたサンプル(コーパス)を 2 つ作り、IIS の .NET Framework 4.8 で記録して変換後を
Windows・Linux で比べた(`sample-parity.ps1`)。1 つのコーパスは変換とビルドが 5 分に収まる大きさまで(NestingProbe は変換・ビルド・比較で 3 分半)。
未着手の機能(Windows 統合認証など)は入れていない。

- `samples/NestingProbe`(コントロールの入れ子、画面の HTML とブラウザーの操作): GridView の各行の GridView(内側のページング・並べ替え・
  テンプレートの Bind と BoundField の編集・検証・削除、外側のページング・並べ替え)、Repeater > DataList > Repeater のコマンド(処理した段で止まる)、
  行ごとの AutoPostBack、コードで作るテンプレート(ITemplate、LoadTemplate の .ascx)、コードでバインドした FormView・DetailsView(双方向の Bind、
  挿入、検証グループ)、入れ子で動的に作るユーザーコントロール(ViewState の追いつき、バブル、EnableViewState=false)、パネルの HTML をそのまま
  `<pre>` に出すページ(DataBind の後と ViewState から作り直した後)。41 スナップショット: Windows・Linux とも最初から一致(41/41、リンク 10/10)。
  作るときに見つけた .NET Framework の動き: httpRuntime 4.5 以上では検証コントロールに jQuery の ScriptResourceMapping が要る(ValidationSettings:UnobtrusiveValidationMode=None)、
  検証コントロールを 2 回描くとスクリプトの属性の登録が重なって例外(ブラウザーがスクリプトを受け取るときだけ)。
- `samples/BclProbe`(実行環境の差: 1 行に 1 件、「名前 = 値」で約 1,370 件): 書式なしの double・float、負のゼロ、無限大、Parse、丸め、decimal
  (Numbers)、カルチャを考慮した検索と並べ替え・大文字小文字・空白と Unicode の分類・Regex・エンコーディング(Strings)、アプリが画面に出す例外の
  メッセージ(Errors)、cultures.txt の 24 のカルチャの日付・数値・暦・タイムゾーン・サーバーのタイムゾーン(Cultures)、JavaScriptSerializer・
  DataContractJsonSerializer・XmlSerializer・DataSet・XML・HttpUtility・Uri(Serialization)、GridView・Eval・Bind・ViewState の double(Bound)。
  行ごとの差は `sample-diff.ps1`(`sample-parity.ps1 -Diff` が変換後の記録を残す)。差の行: Windows 207 → 115、Linux 224 → 162。

直したもの:

- **double・float の文字列化**(いちばん影響が大きい): .NET Core 3.0 から書式なしは最短の桁(0.1 + 0.2 が `0.30000000000000004`)、`-0`、
  桁を決めた書式の偶数丸め(2.675 の N2 が 2.67、22.5 の C0 が ¥22)。.NET Framework の FormatDouble と同じ桁の取り方で書く
  FrameworkNumbers を作り(互換アセンブリと FrameworkOnCore の System.Web)、.NET Framework 4.8 との比較 2,647,080 件(`numbers-parity/`:
  Windows PowerShell 5.1 で .NET Framework の値を採り、.NET 10 で比べる)で一致させた。
  - 変換器: 新しいアナライザー FOC1008(FloatingTextAnalyzer、C#・VB)がアプリの文字列化(ToString、連結、補間、string.Format の引数、
    Convert.ToString、Append・Write、VB の & と CStr、object の ToString〈DataRow の値〉)を見つけ、SourceEdits が FrameworkNumbers に書き換える。
    部品「double・float の文字列化と読み取り」(`floating-point`、既定は .NET Framework と同じ)。
  - FrameworkOnCore の System.Web: DataBinder.Eval、BoundField・BoundColumn・ButtonField・HyperLinkField・ImageField の書式、ListControl の項目、
    データバインド式の生成コード(ConvertToString)、HtmlTextWriter.Write、JavaScriptSerializer(R の 17 桁)。
  - サーバーがコンパイルするコード(ページの式 `<%#: String.Format("{0:c}", Item.UnitPrice) %>`、App_Code、CodeFile): FrameworkOnCore.Compilers が
    csc・vbc の前に同じアナライザーで書き換える(FloatingTextRewriter。ASP.NET の生成ファイルは <auto-generated> なので生成コードも解析する)。
    これで wt の既知の差(¥23 と ¥22)が消え、wt は Windows 6/8 → 8/8、Linux 5/8 → 7/8(残りは検証環境の error-page)。
  - 読み取り: 大きすぎる数の Parse・TryParse・Convert.ToDouble は .NET Framework と同じく例外・false(.NET は無限大)、TimeSpan.From*(double) は
    ミリ秒に丸める(platformReplacements、FrameworkParsing)。
- **Linux のタイムゾーン**: コンテナは UTC で動いていた(IIS はサーバーのタイムゾーン。DateTime.Now、ToLocalTime、JSON の `\/Date(...+0900)\/`)。
  capture-culture.ps1 がサーバーのタイムゾーンを取り、変換器が Dockerfile・systemd の環境に `TZ`(IANA の ID)を書く(無ければ変換したマシンの)。
- **Linux のカルチャのデータ**(CultureIcu): zh-CN・zh-TW を作っていなかった(ICU では zh_CN が zh_Hans_CN の別名)。別名と `%%Parent` をたどる。
  ru-RU の属格の月名(MonthNames は stand-alone、MonthGenitiveNames は format)、パーセントの形、th-TH・ar-SA・fa-IR の暦の日付の形
  (.NET はカルチャの暦〈仏暦、ウンム・アル=クラー、ペルシア暦〉のものを読む)、ar-SA・fa-IR の区切り文字(ICU の既定はアラビア数字の記号:
  ラテンのものにする)。カルチャの差の行: 94 → 36。
- JavaScriptSerializer: .NET が TimeSpan・DateTime・DateTimeOffset に足したプロパティ(Microseconds など)を書かない。大きすぎる数の JSON を
  .NET Framework と同じく受け付けない。
- 変換器の書き換えの不具合(ほかの書き換えにも共通): C# の補間の穴の中に `global::` を書くと穴の書式になっていた。括弧で囲む(SourceLanguage.InPlaceOf)。
  platformReplacements の型の名前が C# のキーワード(`double`)・null 許容の注釈(`string?`)で書かれ、System.Double・TryParse の規則が当たらなかった。
- 道具: ParityTest は失敗した手順の番号・URL・ページの先頭を出す。

残り(既知の差として PORT-CANDIDATES.md の「移植では対応できないもの」に書いた): 例外のメッセージ(約 60 件)、Unicode の版、Linux の照合
(ICU)、Environment.NewLine、タイムゾーンの表示名、XmlSerializer・XmlConvert の書き方、Math の最後の桁、`HttpUtility`(.NET のものへ型転送)。
サーバーがコンパイルするコードには、Windows のパスの書き換え(FOC1001〜1004)はまだ掛かっていない(platformReplacements は下の 4 つ目のコーパスで足した)。

回帰: テスト Windows 4,423 件、be Windows 5/5・Linux 5/5、wt Windows 8/8・Linux 7/8、NestingProbe Windows・Linux 41/41。

3 つ目のコーパスと、画面の HTML の比較:

- ParityTest の `compareSource`(シナリオの設定): スナップショットごとにページの HTML も比べる(ブラウザーが持つ文書を 1 タグ 1 行に。
  スクリプト、hidden の値、すべての属性。ViewState・イベント検証の値、WebResource.axd・ScriptResource.axd の鍵、サーバーのアドレスは両側で伏せる。
  `sourceMasks` で追加)。本文の比較は空白をまとめるので、U+202F と空白の違いは HTML の比較でだけ見える。
- `samples/PageProbe`(`compareSource`、web.config は `controlRenderingCompatibilityVersion="4.0"`): マスター・ユーザーコントロール・独自のコントロール
  (TagKey と属性の WebControl、CompositeControl、IPostBackDataHandler・IPostBackEventHandler・コントロールの状態、テンプレートのデータバインド)の
  イベントの順、変更イベントとクリックの順、Load と PreRender で足したコントロール、ClientIDMode のすべての形(ClientIDRowSuffix、検証コントロールと
  ラベルの参照する ID)、ClientScriptManager のすべての登録とクライアントのコールバック、ViewStateMode と位置で決まる動的コントロールの ViewState、
  式ビルダー・グローバルリソース・インラインのコード、Cookie(複数の値、HttpOnly、パス、期限切れ。サーバーとスクリプトで読む)、Request.Browser、
  応答のフィルター・ページの前の書き込み・text/plain・Clear・Flush・コードで作るヘッダー。31 スナップショット、HTML 2,154 行: Windows は最初から一致。
  Linux は 1 件(時刻の PM の前が ICU の U+202F)。
- 変換器: **カルチャのプロファイルが無いと Linux は ICU の既定のデータ**だった(U+202F、ja-JP の通貨記号など)。Linux に配置するときは、
  無ければこのマシンのものを capture-culture.ps1 で採る(既定のカルチャ、web.config の globalization、ページのディレクティブの Culture・UICulture。
  タイムゾーンも)。元のサーバーのものを `--culture-profile` で渡すほうがよい(変換するマシンがサーバーと違うとき)と変換レポートに出す。PageProbe Linux 31/31。
- 回帰: サンプル 21 件(AuthProbe から WcfProbe まで)Windows・Linux とも一致、NestingProbe Linux 41/41。

4 つ目のコーパス(Visual Basic):

- `samples/VbProbe`(VB の Web アプリケーション、`compareSource`): VB ランタイムの関数(Format・FormatNumber・FormatCurrency・FormatPercent・Str・
  Val・IsNumeric・CDbl などの変換、文字列関数、StrConv の全角・半角・カタカナ・ひらがな、Asc・Chr、Like、日付の関数、Rnd の種、IIf・Choose、Err の番号と文)、
  Option Strict Off の遅延バインド、Handles と WithEvents、AddHandler、XML リテラル、コードがマークアップにあるページ(サーバーの vbc がコンパイル)。
  最初の差: Windows 7 行(Str・Format・FormatNumber・FormatCurrency の double、CDbl("1e400")、Asc("あ")、`<%: 1 / 3 %>`)、Linux はさらに StrConv 5 行
  (.NET は Windows 以外で PlatformNotSupportedException。ProperCase も)。
- 直したもの(互換アセンブリ VisualBasicText、platformReplacements): VB の Str・Format・FormatNumber・FormatCurrency・FormatPercent の double は
  .NET Framework の桁(FormatNumber 系は 15 桁の decimal にして VB ランタイムに渡す)、文字列から Double・Single への VB の変換(CDbl、暗黙)は大きすぎる数で
  OverflowException(FOC1008 の vbparse。サーバーがコンパイルする VB も)、Asc はシステムの ANSI コードページ(.NET は カルチャの)、StrConv は Windows 以外で
  Windows と同じ変換(ロケールの ANSI コードページで往復してから: Shift_JIS に無い文字は最適な文字か「?」、`\` と `＼` はそのまま、ヾ の半角は ヽﾞ。
  Windows の StrConv と全文字の範囲で比べて一致するテスト)。`HttpUtility.HtmlEncode(object)` の double(`<%: d %>`)も FrameworkNumbers。
- 結果: VbProbe Windows・Linux とも 7/7。回帰: テスト 4,426 件、be 5/5、wt 8/8、NestingProbe 41/41、PageProbe 31/31(Windows)。
- `sample-parity.ps1` は VB のサンプル(`<名前>.vbproj`)も扱う。
- サーバーがコンパイルするコード(App_Code、CodeFile のページ、マークアップのコード)にも platformReplacements を掛ける: 変換器が選んだ規則をサイトの bin の
  `frameworkoncore.platform.txt`(PlatformAnalyzer の行と置き換え)に書き、FrameworkOnCore.Compilers の書き換え(FloatingTextRewriter)が PlatformAnalyzer も動かす。
  VbProbe のマークアップの Str・FormatCurrency・FormatNumber・StrConv が Windows・Linux で .NET Framework と同じに(それまで StrConv は Linux で例外)。
  回帰: サンプル 24 件 Windows・23 件 Linux(VbProbe は別に 7/7)、be Windows・Linux 5/5、wt Windows 8/8・Linux 7/8(error-page)。
### パッケージを 1.6.5-w2l.16 に上げて公開(2026-10-17)

別のマシンで Studio から n2 を変換すると、変換後のアプリが起動時に止まった(`'WebApplicationBuilder' does not contain a definition for 'AddWebFormsW3CLogging'`)。
GitHub Release の 1.6.5-w2l.15 は 2026-10-03 に公開したもので、その後の FrameworkOnCore.Runtime の変更(W3C ログ、IIS の機能のホスト層、上の「w2l.15 のまま作り直した」もの)は
この PC で同じ版番号のまま作り直していただけだった。ほかのマシンは Release の古い w2l.15 を取得し、変換器が書く Program.cs の呼び出しが無かった
(変換器はコンパイルできない呼び出しを PlatformNotSupportedException のスタブにする)。

- 版番号を 1.6.5-w2l.16 に上げた(`rules/packages.json`、`pack-frameworkoncore.ps1`、`convert-project.ps1`、`template.csproj.txt`、shim と比較テストのプロジェクト)。
  push で Actions(FrameworkOnCore packages)がビルドし、Linux のテストの後に Release frameworkoncore-1.6.5-w2l.16 を公開する。
- 今後: FrameworkOnCore.Runtime を変えたら版番号を上げる(同じ版番号で作り直すと、ほかのマシンと NuGet のキャッシュは古いままになる)。
  忘れても気付くように、次の 3 つを入れた(ユーザーの指示):
  1. **公開済みの版の作り直しを止める**: `pack-frameworkoncore.ps1` は、版が公開済み(Release のタグがある)で、そのコミットから
     パッケージの元(`FrameworkOnCore.Runtime/`、`frameworkoncore.slnx`。未コミットの変更と新しいファイルも)が変わっていれば、
     作らずに止まる(`frameworkoncore-version.ps1` の `Assert-FrameworkOnCoreVersionUnchanged`)。変わっていなければ作り直せる(テストだけの push の Actions)。
     Actions のビルドも同じスクリプトなので失敗する(以前は警告だけで成功だった)。GitHub に接続できないときは警告だけ。
  2. **版番号を書くのは 1 か所**: `rules/packages.json` の `frameworkOnCoreVersion`。スクリプト(pack・publish・convert-project)は
     `frameworkoncore-version.ps1` で、プロジェクト(shim、比較テスト)は `FrameworkOnCoreVersion.props` で読む。template.csproj.txt は `__FOC_VERSION__`。
  3. **古いパッケージは変換の時点で止める**: 変換器が書いたホスト(Program.cs、Program.vb)がコンパイルできないときは、
     スタブにせず(以前はスタブにしてビルドが通り、起動時に止まった)、「パッケージが変換器より古い」という説明と対処を
     レポートの未解決に出してビルドを失敗にする(`BuildFixer.HostFiles`、`BuildFixerHostTests`)。アプリのソースは今までどおり。
- 確認: この PC で w2l.16 を作り(21 パッケージ)、テスト 4,408 件合格(スキップ 20)、HeadersProbe を w2l.16 で変換して Windows で IIS と 3/3 一致。

### アプリの再起動の間の要求を待たせる(1.6.5-w2l.17、2026-10-17、ユーザーの指示)

別のマシンで Studio のコンテナ起動の n2 をインストールすると、管理者のパスワードの登録の直後に「Service Unavailable」になった。

- 原因: インストーラーが web.config を書き換え、アプリが再起動する(作業プロセスが終了コード 75 で終わり、親が新しい作業プロセスを起動する。約 3.5 秒)。
  その間、古い作業プロセスは 503(Retry-After)を返し、終わってから新しい作業プロセスが待ち受けるまでは接続できなかった。
  インストーラーは登録の直後に次のページへ移るので、ブラウザーに 503 が出た。`n2-install.ps1` は 5 秒待っていたので気付かなかった。
- IIS(観察): ページで web.config を書き換えた直後の要求は、すぐに 200 で、新しい AppDomain が答えた(エラーなし)。
- 直したこと(FrameworkOnCore.Web、`ListenerHandoff`): 作業プロセスを起動する親が待ち受けのソケットを持ち続け、各作業プロセスに渡す
  (Linux・macOS: 継承する記述子。作業プロセスの Kestrel はホスティング スタートアップで `SocketTransportOptions.CreateBoundListenSocket` からそれを使う)。
  再起動の間の新しい接続はソケットの待ち行列で次の作業プロセスを待つ。古い作業プロセスに来た要求(ブラウザーが保った接続)には、
  同じ URL への 307(`Connection: close`。POST の本文も送り直される)を返し、新しい接続で待たせる。
  待ち受けるのは URL(`--urls`、`ASPNETCORE_URLS`、`DOTNET_URLS`、appsettings の `urls`、無ければ `ASPNETCORE_HTTP_PORTS`、無ければ http://localhost:5000)の http だけ。
  https と Kestrel:Endpoints は作業プロセスが今までどおり自分で待ち受ける。
- Windows は今までどおり(503 と、待ち受けまでの接続の失敗)。Windows のソケットは一生に 1 つの I/O 完了ポートにしか結び付けられず、
  WSADuplicateSocket で渡したものは 1 代目の作業プロセスでは使えたが、2 代目では accept が失敗した
  (「Asynchronous operations are not allowed on this socket ... already bound to an IO completion port」)。
- 確認: 再起動を起こすサンプル(ページで web.config を書き換える)を Linux のコンテナで、書き換えの直後の要求(保った接続・新しい接続・POST とそのリダイレクト)が
  すべて 200(2〜3 秒待つ)。n2 のインストーラーを待ち時間なしで進めて、登録の POST から次のページまで 200。
  テスト Windows 4,410 件・Linux 4,408 件と SqlClientTests 204 件、HeadersProbe Windows・Linux 3/3、be Windows・Linux 5/5、wt Windows 6/8・Linux 5/8(以前と同じ)。
- あわせて直したこと: 前回の「版番号を 1 か所に」で、shim(System.Web.Abstractions・Routing)がリポジトリの規則から版番号を読むようにしたが、
  Linux の検証(`run-linux.ps1`)は shim だけをコンテナにコピーしてビルドするので、版番号が無かった(NU1015)。変換器が自分の版番号を
  `shims/FrameworkOnCoreVersion.g.props` に書き(`RuntimeSetup.WriteShimsVersion`。生成物、Git には入れない)、shim はそれを読む。
  `run-tests-linux.ps1` は `FrameworkOnCoreVersion.props` もコピーする。

### フォルダーごとの設定、フォルダーの一覧、httpErrors・圧縮の調査(2026-10-16、ユーザーの指示)

- **フォルダーの web.config・location が効かなかった所を直した**(IisConfiguration で URL のフォルダーごとに重ねる。ドキュメントでどれも
  フォルダーの web.config で設定できる): 既定のドキュメント(defaultDocument)、フォルダーの一覧(directoryBrowse)、要求の上限
  (requestLimits: アップロード用のフォルダーだけ上限を変える使い方)。HeadersProbe に足した `docs/`・`browse/`・`upload/` で確かめた。
- **フォルダーの一覧が無かった**: directoryBrowse を有効にしたフォルダーは 404(コメントの「StaticFileHandler の一覧」は誤り)。IIS の
  DirectoryListingModule のページを作った(showFlags の Date・Time・Size・Extension・LongDate、日付 10 桁・時刻 8 桁・サイズ 12 桁の右詰め、
  `&lt;dir&gt;`、隠しファイルは出さない、web.config は出す、言語と書式はサーバーのもの: 日本語の Windows では `[親ディレクトリへ]` と
  `2026/10/16     7:01`)。IIS の本文と 1 バイトまで一致(326 バイト)。
- **状態の行の理由**: 要求の上限の 413 は IIS では `413 Request Entity Too Large`(Kestrel は `Payload Too Large`)。
- **Cache-Control**: IIS の 404・403.14 などに付く `Cache-Control: private` は、ローカルの要求への詳細エラーページの物で、リモートの要求への
  カスタムのページには付かない。ホストはリモートの動き(付けない)。HeadersProbe はリモートの動き(`httpErrors errorMode="Custom"`)で記録し直した。
- `headers-check.ps1`(状態の行・Cache-Control まで)60/60。
- **httpErrors の調査**(サンプル `samples/ErrorsProbe`、`errors-check.ps1`。31 要求、変換後は 11 件だけ同じ: 未実装): IIS は
  - 自分の誤り(404.0、404.8 など)をカスタムのページ(`%SystemDrive%\inetpub\custerr\<言語>\404.htm`、サブ状態ごとのファイル)にする。
  - アプリの返した 400 以上の応答も置き換える(existingResponse Auto)。置き換えないのは `Response.TrySkipIisCustomErrors = true` と
    ASP.NET 自身の誤りのページ(例外、HttpException)。Replace はそれらも置き換え、PassThrough は何も置き換えない(本文が空でも)。
  - ExecuteURL は、そのページを 200 で返す(ページには `404;http://host/元の URL` のクエリ文字列、RawUrl は元の URL)。File はファイルを元の状態で。
    Redirect は 302 Redirect。
  - DetailedLocalOnly(既定): ローカルの要求には詳細エラーページ(アプリの応答は置き換えない)。
  - ホストは今、存在しない URL を ASP.NET Core の末尾の空の 404 にしている(`/bin/x.dll` の 404.8 も)。
- **httpErrors を IIS 互換で実装した**(ユーザーの決定: 既定は IIS 互換、IIS のページは簡易ページ、ローカルの要求には IIS どおり詳細ページ)。
  FrameworkOnCore.Web の `IisHttpErrors`(新規)、`app.UseWebFormsIis()` の中(ヘッダーの次、URL Rewrite の前)。
  - 設定 `HttpErrors:Mode`: `Iis`(既定)/ `AspNetCore`(ASP.NET Core の UseStatusCodePages: 本文の無い誤りに文字列、アプリの応答はそのまま)/ `Off`。
  - errorMode(DetailedLocalOnly・Custom・Detailed。ローカルはループバックとサーバー自身のアドレス)、existingResponse(Auto: アプリの
    400 以上の応答を置き換える、`TrySkipIisCustomErrors` と ASP.NET の誤りのページは残す。詳細ページは本文の無い応答だけ。Replace・PassThrough)、
    error(File・ExecuteURL・Redirect、clear・remove、フォルダーごと)。IIS 自身の custerr は簡易ページ(IIS と同じ見出しと文、スタイルなし)、
    項目の無い状態は短い文。言語はサーバーのもの(日本語の Windows では日本語: 短い文、詳細ページの見出し、リダイレクトの「ドキュメント移動」)。
  - IIS の動きとして確かめて合わせた所: 置き換えてもアプリのヘッダー(Cache-Control、X-AspNet-Version)は残し本文と型だけ替える。ExecuteURL は
    200 でクエリ文字列 `404;http://host/元の URL`。Redirect はログに元の誤りの状態(403.0、403.14)を書く。CoreWCF など System.Web 以外が
    本文を書いた応答は残す(IIS の WCF が TrySkipIisCustomErrors を立てる)。
  - ホストが自分で書いていた誤りの本文(静的ファイルの 404・403.14、要求フィルタリング、URL Rewrite の CustomResponse など)はやめ、httpErrors が
    作る。bin・App_Data などの隠しセグメントは、物が無くても 404.8(以前は ASP.NET Core の末尾の空の 404.0)。
  - **System.Web の直した所**: 途中のフラッシュ(Response.End など)で、状態が 200 以外だと文字のバッファを確定させる前にヘッダーを書いて
    いて、Content-Type の charset が落ちていた(`text/html`。IIS の統合パイプラインでは `text/html; charset=utf-8`)。
  - Linux の検証: 正解は IIS のマシンから(ローカル: 詳細ページ、アプリの応答はそのまま)記録した。コンテナの前の検証のクライアントは
    リモートなので、検証のスクリプト(sample-parity・run-linux・parity・chart-probe)は `HttpErrors__LocalAddresses=*` を渡す(既定は空: IIS どおり)。
  - `errors-check.ps1` 32/32、`headers-check.ps1` 60/60、`rewrite-check.ps1` 53/53(本文はローカルの詳細ページの説明文の量だけ違う)。
  - 回帰: テスト Windows 4,420 件・Linux 4,418 件・SqlClientTests 204 件、サンプル 21 件(Windows・Linux)、コントロール 24 件(Windows・Linux)、
    be 5/5(Windows・Linux)、wt Windows 6/8・Linux 5/8(既知の差)。
- **圧縮を IIS 互換で実装した**(ユーザーの決定: 既定は IIS 互換)。FrameworkOnCore.Web の `IisCompression`(新規)、`app.UseWebFormsIis()` の中
  (ヘッダーの次、httpErrors の前: 最終的な応答を圧縮)。設定 `Compression:Mode`: `Iis`(既定)/ `AspNetCore`(ASP.NET Core の
  ResponseCompression)/ `Off`(前段のプロキシに任せる)。
  - 調べるため IIS の動的圧縮(IIS-HttpCompressionDynamic)をこのマシンに入れた。**動的圧縮があると、静的ファイルも静的圧縮が効くまで
    (1 回目、2,700 バイト未満も)動的圧縮で圧縮される**。圧縮は 200 だけ(誤り・リダイレクトは圧縮しない)。Content-Length 付き、ETag は同じ。
  - 元のサーバーのモジュール(globalModules の StaticCompressionModule・DynamicCompressionModule。取り込んだ設定が無ければ Windows の既定の
    静的だけ)、httpCompression(scheme の順: gzip・deflate・br、staticTypes・dynamicTypes、minFileSizeForComp、HTTP/1.0・Via・Range は圧縮しない、
    staticCompressionIgnoreHitFrequency)、urlCompression(フォルダーごと)、serverRuntime の frequentHitThreshold・frequentHitTimePeriod。
    静的に圧縮したファイルはメモリに持つ(IIS の一時ファイルの代わり)。応答キャッシュ(StaticResponseCache)は圧縮前を持ち、返す時に圧縮する。
  - **System.Web 側を直した所**: 要求の最後のフラッシュ(finalFlush)で本文を送ってしまっていて、前段からは「アプリが途中でフラッシュした」
    のと区別できなかった。最後のものは送らず要求の終わりに任せた(IIS と同じく Content-Length 付きで圧縮できる。途中のフラッシュは逐次)。
  - `compression-check.ps1`(取り込んだ設定あり、動的圧縮あり)29 中 24 一致。残りは簡易ページ・エラー画面の長さと、Response.Redirect の
    応答が chunked(IIS は Content-Length): .NET の Response.End は途中でフラッシュする(スレッドを中断できないため。外すと End 前に書いた
    本文が送られなくなるので残した)。
  - 性能(PipelineProbe Check.htm、32 接続、Windows): 75,793〜77,274 req/s、圧縮と httpErrors を切って 73,722(同じ範囲: この 2 つの負担は
    見えない)。前に記録した 89,129 より低いが、切っても同じなので、この変更によるものではない(ぶれか、それ以前の変更。要調査)。
  - 回帰: テスト Windows 4,420 件・Linux 4,418 件・SqlClientTests 204 件、サンプル 21 件・コントロール 24 件(Windows・Linux)、be 5/5、
    wt Windows 6/8・Linux 5/8(既知の差)。
- **圧縮の調査**(`compression-check.ps1`、ドキュメント httpCompression・urlCompression): Windows の既定は静的圧縮だけ。IIS は 10 秒に 2 回目の
  要求から gzip で返し(1 回目は圧縮しない)、`Vary: Accept-Encoding`、ETag は同じ。2,700 バイト未満、型の対象外(画像)、HTTP/1.0、Via、Range、
  gzip を受けない要求は圧縮しない。.aspx は動的圧縮が入っていないので圧縮しない。ホストは何も圧縮しない(4 件違う)。
- 回帰: テスト Windows 4,419 件、サンプル 20 件(Windows・Linux)、be 5/5・wt 6/8(Windows、既知の差)。

### 応答ヘッダー(customHeaders・redirectHeaders)と HTTP リダイレクト(httpRedirect)(2026-10-15、ユーザーの指示)

計画の優先度 高の残り 2 つ。どちらも ASP.NET Core に相当する標準のミドルウェアが無い(ヘッダーは自分で付け、リダイレクトは Rewriter で
別の書き方になる)ので、モードは設けず IIS 互換だけにした。IIS の HTTP リダイレクト機能(IIS-HttpRedirect)をこのマシンに入れて記録した。

- **IIS で確かめた動き**(サンプル `samples/HeadersProbe`、`headers-check.ps1`):
  - customHeaders はすべての応答に付く(ページ、静的ファイル、304、404、404.8、500、リダイレクト)。アプリが同じ名前を付けると両方送られる
    (`X-Frame-Options: DENY` と `SAMEORIGIN` の 2 行)。`remove` は継承した設定を外すだけ(アプリの付けたものは外さない)。
  - redirectHeaders は 3xx に付く(304 にも)。
  - httpRedirect: 設定したフォルダーからの残りのパスを、宛先(末尾の `/` を除く)に付ける(`/old/sub/y.aspx?q=1` → `/new/sub/y.aspx`。
    クエリ文字列は捨てる)。exactDestination では `$S`(残り)・`$Q`(`?` 付きのクエリ)・`$V`(パス)。childOnly はフォルダー直下だけ
    (下のフォルダーは対象外。フォルダー自身は対象)。ワイルドカードはフォルダーからの相対で照合。既定は 302(Permanent 301、Temporary 307)。
    子の `enabled="false"` で止まる。ファイルの location は大文字小文字を区別しない。Location は絶対 URL。
  - URL Rewrite が先に動き、httpRedirect は書き換えた後の URL で判断する(`/tonew` を `/old/o.htm` に書き換えると `/new/o.htm` へ 302)。
- **実装**(FrameworkOnCore.Web):
  - `IisConfiguration`(新規): system.webServer の節を IIS の順(サーバー → サイト → web.config → フォルダーの web.config・location)で
    URL のフォルダーごとに重ねる共通部品。URL Rewrite(`IisUrlRewrite`)の設定の読み込みもこれに揃えた(同じ処理が 2 か所にあった)。
  - `IisHttpProtocol`(新規): 応答の開始時に customHeaders・redirectHeaders を足す。取り込んだ設定が無ければ IIS の既定
    (`X-Powered-By: ASP.NET`。web.config の `<remove name="X-Powered-By" />` で IIS と同じく消える)。静的ファイルの応答キャッシュ
    (StaticResponseCache)は足したヘッダーを保存しない(返す時にまた足すので二重にならない。確かめた)。
  - `IisHttpRedirect`(新規): 上の動き。本文は IIS と同じ「Document Moved」。
  - `app.UseWebFormsIis()`(新規): IIS の機能を IIS の順に入れる(ヘッダー → URL Rewrite → HTTP リダイレクト)。変換後の Program.cs の
    `app.UseWebFormsUrlRewrite()` をこれに替えた(C#・VB)。今後の IIS の機能(圧縮、httpErrors など)もここに入れる。
  - 変換レポート: customHeaders・redirectHeaders・httpRedirect を「対応」に。
- **確かめた**: HeadersProbe の画面の比較 3/3(Windows・Linux)、`headers-check.ps1`(状態、Location、X- ヘッダーすべて(重複も)、
  ログの行)**37/37 一致**(取り込んだ設定あり・なしの両方)。URL Rewrite も読み込みを揃えた後で 53/53 のまま。
- 回帰: テスト Windows 4,419 件・Linux 4,417 件・SqlClientTests 204 件、サンプル 20 件(Windows・Linux)、コントロール 24 件
  (Windows・Linux)、be Windows 5/5・Linux 5/5、wt Windows 6/8・Linux 5/8(既知の差)。
- 次(計画の優先度 中): 圧縮、httpErrors(未知の URL の 404 の本文が空)、Windows 認証。
- **ドキュメントとの突き合わせ(2026-10-16、ユーザーの指示)**: 上の実装は IIS での観察だけで決めていた。Microsoft のドキュメント
  (IIS の設定リファレンス `httpRedirect`・`httpRedirect/add`・`customHeaders`・`redirectHeaders`、IIS 6.0 SDK の HttpRedirect、モジュールの作者の
  ブログ "Client-side redirection using HttpRedirection module")と比べ、食い違いと書かれていない所を IIS で確かめて直した:
  - ワイルドカードの `$0`〜`$9`(`*` に一致した部分)が無かった。照合はフォルダーからの残り(先頭の `/` 込み)に対して、`*` は `/` もまたいで
    最短で一致(`/wild2/sub/page.htm` に `*.htm` → `$0=/sub/page`、`*/*.aspx` → `$0=`・`$1=d/e`。パス全体を書いた `/wild2/full/*` は一致しない)。
  - 相対のリダイレクトで足す残りの起点は、destination を設定したフォルダーではなく httpRedirect を持つ最も深いフォルダー(ブログのとおり。
    `base/deep` が状態だけ設定すると `/base/deep/f.htm` → `/new/f.htm`)。
  - 状態の行の理由は IIS 独自: `302 Redirect`、`307 Moved Temporarily`(ドキュメントは "307 Redirect")、`308 Redirected Permanently`。
    URL Rewrite のリダイレクトも同じ。500.50 は `500 URL Rewrite Module Error.`。IIS のリダイレクトを返す所(`IisUrlRewrite.Redirect`)で付ける。
  - 実装どおりだったもの: `$P`、相対での `$V`(その後に残りが付く)、308、`/` で始まらない宛先(Location は相対のまま)。
  - ドキュメントの childOnly の説明は版で食い違う(IIS 6.0 は「下のフォルダーやファイルだけ」)。IIS 10 の実際(フォルダー自身と直下のファイル、
    下のフォルダーは対象外)に合わせたまま。
  - `headers-check.ps1` 53/53、`rewrite-check.ps1` 53/53(どちらも理由まで比べる)、HeadersProbe・RewriteProbe の画面の比較(Windows・Linux)。
    回帰: テスト Windows 4,419 件、サンプル 20 件(Windows)、be 5/5・wt 6/8(Windows、既知の差)。

### URL Rewrite の 2 つのモード: IIS 互換(既定)と ASP.NET Core(2026-10-15、ユーザーの決定)

IIS の URL Rewrite モジュール(web.config と元のサーバーの system.webServer/rewrite)は、これまで変換後に効かなかった(報告だけ)。
ASP.NET Core 標準の書き換え(`UseRewriter` + `AddIISUrlRewrite`、web.config の規則をそのまま読む)を IIS(URL Rewrite 2.1 をこのマシンに
入れた)と比べたところ、同じ答えは 26 件中 2 件だった。ユーザーの決定: W3C ログと同じく 2 つから選べるようにし、既定は IIS 互換。

- **設定** `UrlRewrite:Mode`: `Iis`(既定)/ `AspNetCore`(標準。将来 ASP.NET Core に書き直した時もそのまま使える)/ `Off`。
  変換後の Program.cs は W3C ログの直後(ログの行は要求された URL、IIS と同じ)、System.Web の前に `app.UseWebFormsUrlRewrite()` を呼ぶ。
- **標準(AspNetCore)と IIS の違い**(調べて分かったこと。モードを選ぶ時の判断材料):
  - 読めない書き方があるとアプリが起動しない: `subStatusCode`(IIS マネージャーが既定で書く)、`patternSyntax="Wildcard"`、
    標準に無いサーバー変数(`{HTTP_X_FORWARDED_PROTO}`・`{URL}`・`{SERVER_PORT}`・`{UNENCODED_URL}`・`{APPL_PHYSICAL_PATH}` など)、providers。
    `serverVariables`・`outboundRules`・`configSource` は黙って無視する。
  - 動きの違い: 付け足すクエリ文字列の順(IIS は規則の後に元のもの: `id=5&x=1`。標準は逆で、Request.QueryString やポストバック先も変わる)、
    リダイレクトの Location(IIS は絶対 URL と「Document Moved」の本文、標準は相対で本文なし)、IsDirectory(標準はフォルダーを判定できず
    書き換えてしまう: `/app/` が 403.14 でなく 200)、`{REQUEST_URI}`(IIS はクエリ文字列を含む)、HTTP_X_ORIGINAL_URL が無い、
    CustomResponse の本文、ルートの web.config しか読まない。
- **IIS 互換**(FrameworkOnCore.Web の `IisUrlRewrite`、新規):
  - 規則の出所と継承: 取り込んだサーバーの設定(globalRules、サーバーとサイトの rules、allowedServerVariables)、web.config(ルート、`<location>`、
    サブフォルダーの web.config、`configSource` の外部ファイル)。親の規則が先、`clear`・`remove`。サブフォルダーの規則は、そのフォルダーから
    の相対の URL に当て、相対の URL もそのフォルダーから。書き換えた後の規則は書き換えた URL を見る。
  - パターン(ECMAScript・Wildcard・ExactMatch、ignoreCase、negate)、条件(MatchAll・MatchAny・trackAllCaptures・IsFile・IsDirectory)、
    `{R:n}`・`{C:n}`、IIS のサーバー変数(`HTTP_*` は要求ヘッダー)、`ToLower`・`UrlEncode`・`UrlDecode`、rewriteMap(defaultValue)。
  - serverVariables の設定(`HTTP_*` は要求ヘッダー、`RESPONSE_*` は応答ヘッダー、それ以外は Request.ServerVariables)。取り込んだ設定の
    allowedServerVariables に無いものは IIS と同じ 500.50(取り込んだ設定が無ければ許す)。
  - アクション: Rewrite(X-Original-URL、`IIS_WasUrlRewritten`、logRewrittenUrl。globalRules の書き換えは IIS と同じくログに書き換え後の URL)、
    Redirect(301・302・303・307、絶対 URL と「Document Moved」)、CustomResponse(状態、サブ状態をログに、IIS のエラーページ)、
    AbortRequest(ログに書かない)、None。
  - しないこと(起動時に 1 回表示し、変換レポートにも出す): outboundRules(応答の書き換え)、providers、別のサーバーへの書き換え
    (ARR のリバース プロキシ。502.3 を返す)。
- **ついでに IIS と揃えた所**(Rewrite を切っても違っていた所):
  - Request.ServerVariables に UNENCODED_URL が無かった(IIS は送られたままの URL)。ワーカー要求に追加した(規則が設定した変数も同じ口)。
  - 既定のドキュメントの無いサブフォルダー(`/app/`)が、ASP.NET Core の末尾の本文の無い 404 になっていた。IIS と同じく 403.14
    (`directoryBrowse` が有効なら一覧)。
  - フォルダーへの `/` 付きのリダイレクト(`/app` → `/app/`)の Location が相対だった。IIS と同じく絶対 URL と「Document Moved」の本文。
- **確かめた**: サンプル `samples/RewriteProbe`(書き換え、リダイレクト 4 種、条件、rewriteMap、連鎖、Wildcard・ExactMatch、MatchAny・
  trackAllCaptures、サーバー変数、サブフォルダーの web.config と `remove`、configSource、CustomResponse、Abort)を IIS で記録し、画面の比較
  14/14(Windows・Linux)。`rewrite-check.ps1` で要求を 1 件ずつ(リダイレクトを追わずに状態、Location、ページが見た URL とサーバー変数、
  応答ヘッダー、W3C ログの行)比べ、**IIS 互換は 53/53 一致**。この時、IIS にサーバーの globalRule と許可するサーバー変数を一時的に設定し、
  `capture-iis.ps1` で取得した設定を変換後に置いて、元のサーバーの設定の取り込みも通して確かめた。標準(AspNetCore)は、この規則では
  起動しない(Wildcard を読めない)。本文の文言は比べていない(IIS は日本語で、ローカルの要求には詳しいエラーページ。未知の URL の 404 の
  本文が空なのは httpErrors の課題)。
- 回帰: テスト Windows 4,419 件・Linux 4,417 件・SqlClientTests 204 件、サンプル 19 件(Windows・Linux)、コントロール 24 件(Windows・Linux)、
  be Windows 5/5・Linux 5/5、wt Windows 6/8・Linux 5/8(既知の差)。
- 次(計画の優先度 高): customHeaders(サーバーの `X-Powered-By`)・httpRedirect。

### ホスト層の ASP.NET Core 化の土台: 元の IIS の設定の取り込みと、IIS の機能の報告(2026-10-15、ユーザーの指示)

ホスト層(IIS の代わり)を ASP.NET Core 標準のミドルウェアに置き換えていく計画の優先度 0。ユーザーの決定: 各機能の既定(IIS 互換か標準か)は
調査の後に都度決める、未対応の設定は変換レポートに出す、元のサーバーの設定は取り込む。

- 棚卸しで、IIS にあって変換後に効かない(対応していない)機能が分かった: URL Rewrite、圧縮(IIS は静的ファイルを既定で圧縮する)、
  customHeaders(IIS は既定で `X-Powered-By: ASP.NET` を付ける)、httpRedirect、httpErrors、IIS の URL の認可、ipSecurity、Windows 認証など。
  web.config やサーバーで使われていても、これまでは黙って効かなかった。
- **取得**(`capture-iis.ps1`、元のサーバーで管理者として): applicationHost.config(共有構成なら redirection.config の指す先)から、サイトに効く
  system.webServer(サーバーのものと `<location path="">`、サイトの `<location>`)、サイトのログの設定(logFile)と ID、siteDefaults、IIS に
  入っているモジュール(globalModules)を、web.config と同じ形の XML(`iis-profile.config`)に書く。パスワードの属性と暗号化された値は除く。
  このマシンの IIS で試した(Default Web Site: 22 の節、ハンドラー 56、モジュール 28、秘密の値 0 件)。
- **変換器**(`--iis-profile <file>`): アプリのフォルダーに `iis-profile.config` として置く(`.config` は要求フィルタリングで外に出ない)。
  変換レポートに「IIS の機能」の節を設け、取り込んだ設定と web.config(サブフォルダーのものも)で使われている機能を、対応・一部対応・
  未対応で出す(`IisFeatures`)。クラシック ASP・SSI・PHP のファイルも出す。`IisFeaturesTests` 12 件。
- **実行時**(`IisProfile`): ファイルがあれば読み、IIS と同じく「サーバー → サイト → web.config → フォルダー」の順に重ねる。今使う所:
  静的ファイルの MIME の対応表と clientCache(サーバーの表があればそれが表全体)、要求の上限(requestLimits)、既定のドキュメント
  (サーバーの一覧。Default.asp は除く)、W3C ログ(サイトの logFile の enabled・logExtFileFlags・period・localTimeRollover、s-sitename の
  W3SVC<ID>、サーバーとサイトの httpLogging の dontLog。優先はアプリの設定 > web.config > 取り込んだ設定)。
- **確かめた**: PipelineProbe のサイトをこのマシンの IIS に作って取得し(W3SVC3)、それを渡して変換(`sample-parity.ps1 -IisProfile`):
  画面の比較 18/18、IIS のログとの比較 18/18 行。レポートに、サーバーの `X-Powered-By` の追加(未対応)と静的ファイルの圧縮(未対応)が出た
  (どちらも実際の差)。be・wt のレポートの IIS の節は、管理された modules・handlers と staticContent(対応)だけ。
- 回帰: テスト Windows 4,418 件・Linux 4,416 件・SqlClientTests 204 件、サンプル 18 件(Windows・Linux)、関係するコントロール 24 件
  (Windows・Linux)、be Windows 5/5・Linux 5/5、wt Windows 6/8・Linux 5/8(既知の差)。
- 次(計画の優先度 高): URL Rewrite(標準の `UseRewriter`・`AddIISUrlRewrite` を調べ、IIS と比べて既定を決める)、customHeaders・httpRedirect。

### W3C ログの 2 つのモード: IIS 互換(既定)と ASP.NET Core(2026-10-15、ユーザーの決定)

下の節の ASP.NET Core の W3C ログは、形式が IIS と違い、書き込みに上限がある(ソースで確認: 書き込み待ちは 1,024 件で、満杯なら要求の
側が待つ。1 本のスレッドが 1 行ごとにファイルの状態を確かめて書き出す。毎秒約 1 万件はこのマシンの実測で、普遍的な限界ではない)。
ユーザーの決定: 2 つから選べるようにし、既定は IIS 互換。追いつかない時は要求を待たせてよい(行は捨てない)。

- **設定** `W3CLogging:Mode`: `Iis`(既定)/ `AspNetCore`(下の節のもの。将来 ASP.NET Core に書き直した時もそのまま使える)。
- **IIS 互換**(FrameworkOnCore.Web の `IisLogWriter`・`IisLogMiddleware`、新規):
  - IIS と同じファイル名(`u_exYYMMDD.log`。`W3CLogging:Period` で Hourly・Monthly、`W3CLogging:LocalTimeRollover`)、UTF-8(BOM なし)、CRLF、
    ファイルを開くたびの見出し 4 行(`#Software`・`#Version`・`#Date`・`#Fields`。IIS もプロセスの起動ごとに書く)、IIS の項目の順と値の書き方
    (無ければ `-`、空白は `+`)。項目は IIS 10 の既定(このマシンの IIS と同じ 17 項目)、`W3CLogging:Fields` に IIS の `logExtFileFlags` の書式で。
    `#Software` の行は既定で IIS と同じ(IIS のログを読む道具のため。`W3CLogging:Software` で変えられる)。
  - 1 行ごとに書き出さず、たまった行をまとめて書く。待ちが 65,536 行を超えたら、要求の側(接続の次の要求)が待つ。
  - sc-substatus・sc-win32-status: ホストが IIS の代わりに答える所から渡す(静的ファイルの 404.0/Win32 2、404.3/Win32 50、要求フィルタリングの
    404.7・404.8・404.11・404.14・404.15・413.1、フォルダーの 403.14、アプリの `Response.SubStatusCode`)。http.sys が拒否するもの(区切りが
    260 文字を超える URL)は IIS のサイトのログに出ない(HTTPERR のログ)ので、書かない。
  - cs-bytes: 要求の行(URL は符号化されたまま)、ヘッダー、本文。sc-bytes: 状態の行、ヘッダー、送った本文(数える)。
- **IIS と並べて確かめた**(`iislog-check.ps1`: 同じ要求の組を IIS の記録したサイトと変換後に送り、それぞれのログを項目ごとに比べる。
  PipelineProbe、18 要求: ページ、静的ファイル、クエリ、Referer、404、404.3、web.config、bin、App_Data、.cs、長いクエリ、POST、HEAD、条件付き、
  フォルダー、空白や日本語の URL、/): **18 行すべて一致**(日時と time-taken は除く)。cs-bytes は全行一致。sc-bytes は違う(ヘッダー
  `Server: Microsoft-IIS/10.0`・`X-Powered-By` の分。エラーは IIS がローカルの要求に詳しいエラーページを返す分)。
- **比べて見つけて直した、応答そのものの違い**(ログではなく変換後の動き):
  - **未来の日付の If-Modified-Since で 304** を返していた。IIS は無視して 200(RFC 9110 13.1.3)。IisStaticFile を直した。
  - **既定のドキュメントの無いフォルダー**(`/`)が、System.Web の 404 からアプリの customErrors の 302 になっていた。IIS はフォルダーの一覧の
    モジュールが 403.14(IIS のページ)を返す。IisStaticFile が返す(web.config の `directoryBrowse enabled="true"` なら従来の一覧)。
  - `Response.SubStatusCode` が「IIS の統合モードでない」と PlatformNotSupportedException だった(統合モード向けのアプリが設定すると 500)。
    .NET では設定できるようにした。
- **性能**(PipelineProbe Events.aspx、Windows、32 接続):

| 負荷 | ログなし | IIS 互換 | ASP.NET Core |
|---|---|---|---|
| 全力 | 24,206 req/s、p99 4.1 ms | 22,052 req/s、p99 4.2 ms | 14,707 req/s、p99 23 ms |
| 毎秒 5,000 件 | p99 5.2 ms | p99 5.0 ms | p99 5.9 ms |
| 毎秒 10,000 件 | 9,806 req/s | 9,825 req/s | 3,942 req/s |

  IIS 互換は、全力の負荷でもサーバーの処理能力まで書き込みが追いついた(約 2 万 2 千行/秒)。
- Linux(コンテナ): 一時フォルダー(`/tmp/FrameworkOnCore/LogFiles/app/u_ex261015.log`)に IIS の形式で書かれることを確かめた。
- 回帰: テスト Windows 4,406 件・Linux 4,404 件・SqlClientTests 204 件、サンプル 18 件(Windows・Linux)、関係するコントロール 24 件
  (Windows・Linux)、be Windows 5/5・Linux 5/5、wt Windows 6/8・Linux 5/8(既知の差)。

### IIS の要求のログ(W3C)を ASP.NET Core の W3C ログで書く(2026-10-15、ユーザーの指示)

ホストの IIS の代わりの部分を、ASP.NET Core 標準のミドルウェアで置き換えられるものから(ユーザーの判断: 標準を使い、IIS と比べて確かめる)。
IIS は要求を W3C 拡張ログに書く(既定で有効)。変換後は書いていなかった。

- **ランタイム**(FrameworkOnCore.Web、`WebFormsW3CLoggingExtensions`): `builder.AddWebFormsW3CLogging()` と `app.UseWebFormsW3CLogging()`。
  ASP.NET Core の W3C ログに IIS の既定を渡す。
  - 項目: IIS の既定(date time s-ip cs-method cs-uri-stem cs-uri-query s-port cs-username c-ip cs(User-Agent) cs(Referer) sc-status
    time-taken)。日ごとのファイル、サイズの上限なし、削除しない(IIS と同じ)。ファイル名の前に `u_ex`。
  - web.config の `system.webServer/httpLogging dontLog="true"` で書かない。設定(appsettings.json、環境変数)で上書き:
    `W3CLogging:Enabled`、`W3CLogging:LogDirectory`、`W3CLogging:FlushInterval`。
  - 書く場所: アプリのフォルダーの外(中はファイルの監視 FileCache が書くたびに反応する。設定で中にした場合も、そのフォルダーの変更は
    無視する)。既定は Windows `%ProgramData%\FrameworkOnCore\LogFiles\<フォルダー名>`、Linux `/var/log/frameworkoncore/<フォルダー名>`、
    作れなければ一時フォルダーの `FrameworkOnCore/LogFiles/<フォルダー名>`(コンテナの app ユーザーはこちら: `/tmp/FrameworkOnCore/LogFiles/app`)。
  - cs-username: System.Web が認証したユーザー(フォーム認証)を、要求の終わりに ASP.NET Core 側に渡す(IIS の統合パイプラインも
    フォーム認証のユーザーをログに書く)。
  - c-ip・s-ip: Kestrel のデュアルモードのソケットの `::ffff:a.b.c.d` を IIS と同じ `a.b.c.d` に。
- **変換器**: 生成する Program.cs・Program.vb(テンプレート)に上の 2 行。ログのミドルウェアは最初(System.Web の手前で返す静的ファイルも
  記録する)。`HostProgramTests` に 4 件。サンプル用の `Program.cs.txt` も同じにした。
- **見つけた問題**: ASP.NET Core の W3C ログは、書き込み待ちが空になると `FlushInterval`(既定 1 秒)待ち、その間に 1,024 件で満杯になると
  要求の側を待たせる。毎秒 5,000 件の負荷で、ログありは 1,392 req/s・p99 約 800 ms(なしは 4,981 req/s・p99 6.8 ms)。既定の間隔を
  20 ms にして解消(毎秒 5,000 件: ありは p99 5.3 ms、なしは 4.7 ms)。
- **限界**: 書き込みが追いつくのは 1 インスタンスで毎秒約 1 万件まで(毎秒 1 万件で 2 回中 1 回が 4,535 req/s に落ちた。全力の負荷では
  ログありは約 9,000〜15,000、なしは約 27,000〜33,000 req/s、Events.aspx、Windows)。超える場合はログを止めるか、インスタンスを増やす。
- **IIS のログとの違い**(アプリの動きは変わらない。ログを読む道具には影響する):
  - 項目の並びが ASP.NET Core のもの(`#Fields` の行に書かれる)。time-taken が小数のミリ秒(IIS は整数)。
  - sc-substatus・sc-win32-status・sc-bytes・cs-bytes が無い(ASP.NET Core に無い)。
  - ファイル名 `u_ex20261015.0000.txt`(IIS は `u_ex261015.log`)。`#Software` の行が無い。
  - IIS の `selectiveLogging`、サーバーの設定(applicationHost.config のログの設定)は読まない。
- 確認: AuthProbe でログインしたユーザー名が記録される(Windows)、コンテナで一時フォルダーに書かれ IPv4 の形のアドレス(Linux)。
  回帰: テスト Windows 4,406 件・Linux 4,404 件・SqlClientTests 204 件、サンプル 18 件(Windows・Linux)、関係するコントロール 24 件
  (Windows・Linux)、be Windows 5/5・Linux 5/5、wt Windows 6/8・Linux 5/8(既知の差)。
- 次: IIS の URL Rewrite のルール(web.config の `system.webServer/rewrite`)。IIS 側の正解の記録のため、このマシンの IIS に Microsoft の
  URL Rewrite モジュール 2.1(rewrite_amd64_en-US.msi、Microsoft の署名を確認)を入れた(2026-10-15)。

### MVC 5・Web API 2・Razor 3 は移植せず、Microsoft の DLL を使う(2026-10-14、ユーザーの決定)

MVC 5・Web API 2・Web Pages 3・Razor 3 は .NET Framework 本体ではなく NuGet で配布される部品(ソースは AspNetWebStack、Apache 2.0)。
マネージドの DLL だけでできていて、.NET Framework 向けにビルドされたものを .NET 10 がそのまま読み込み、移植版の System.Web の上で
Windows・Linux とも動く(変換器はビルド後に DLL の参照を付け替えるだけ。MvcMovie、nopCommerce 3.90、be の Web API 2・ウィジェットで確認済み)。

- **決定(案 A)**: 移植はしない。アプリが参照する Microsoft の DLL をそのまま使い、Microsoft が NuGet で出す修正版に追従する
  (Microsoft のサポートは当面続く)。
- 移植(案 B)を検討した理由と、今は採らない理由: ソースから作れば、.NET の制限に当たる箇所(yaf: Web API 2 の
  `HttpControllerRouteHandler._instance`(static readonly)をアプリがリフレクションで書き換え、.NET では FieldAccessException)を直せ、
  サポート終了後も自前で保守できる。サポート中は修正を Microsoft の DLL で受け取れるので急がない。サポート終了の時期が近づいたら
  移植に着手する(取り込む版は AspNetWebStack v3.3.0 = MVC・Web API 5.3.0、Web Pages・Razor 3.3.0 を想定)。
- 案 A で残ること:
  - Microsoft の修正版は、移植版の System.Web の上で動くかを更新のたびに確認する(修正が System.Web の新しい内部に依存する場合がある)。
  - Microsoft のサポートは .NET Framework・IIS の上での動作が対象。移植版の上での不具合は自分たちで扱う。
  - .NET の制限に当たる箇所は、DLL の書き換え(AssemblyRetargeter の拡張)で個別に対応する。

### 性能: 静的ファイルを IIS のカーネル キャッシュのように返す(2026-10-14、ユーザーの指示)

前節の後も、静的ファイルは System.Web のパイプライン(モジュールのイベント、ワーカー スレッドへの受け渡し)を毎回通り、空のページと同じだけ
かかっていた。IIS は静的ファイルの応答を http.sys のカーネル キャッシュに入れ、次からはパイプラインを通さずに返す。カーネル キャッシュ自体は
Windows の http.sys のもので、Kestrel・Linux には無い。同じことをホストでする(StaticResponseCache、FrameworkOnCore.Web)。

- **保存する応答**(http.sys・IIS の既定と同じ条件): IIS の静的ファイル ハンドラー(IisStaticFile)がファイル全体(256 KB まで)を匿名の
  ユーザーに返した 200。要求は GET で、クエリ文字列・Range・条件付きヘッダー・Authorization・本文が無い。応答に Set-Cookie・Vary・
  Content-Encoding・private / no-store の Cache-Control が無く、System.Web が `DisableKernelCache` を呼んでいない(元の仕組みを
  ワーカー要求で受ける)。URL が `frequentHitThreshold`(既定 2 回)を `frequentHitTimePeriod`(既定 10 秒)の内に満たした時に保存する。
- **返し方**: 保存した応答(実際に送ったヘッダーと本文。Date・長さ・接続のヘッダーは除く)を、System.Web の手前(WebFormsMiddleware)で
  返す。IIS のカーネル キャッシュと同じく、モジュールも Global.asax のイベントも通らない。
- **捨てる時**: アプリのファイルが変わった時(FileCache の監視)。web.config の `system.webServer/caching` の `enabled`・
  `enableKernelCache` が false なら保存しない。特定のパスの `<location>` に caching があれば、読まずに全体で保存しない
  (フォルダーの web.config の caching も読んでいない)。
- キャプチャーは IisStaticFile がファイル全体を返すと決めた時に作る(応答はハンドラーの後にまとめて送られる)。初めは GET のすべての
  要求で作っており、ページの応答まで毎回写していた。
- **確かめたこと**: パイプラインを通った応答(1・2 回目)と保存した応答(3・4 回目)のヘッダーと本文が、Date 以外同じ。ファイルの
  書き換え・削除が次の要求に反映される。クエリ文字列付き・条件付きの要求はパイプラインを通る。
- **結果**(32 接続、静的ファイル Check.htm):

| | IIS(カーネル キャッシュ) | 変換後 Windows | 変換後 Linux(コンテナ) |
|---|---|---|---|
| 前節(FileCache のみ) | 78,521 req/s | 46,855 | 32,433 |
| 今回 | ― | **89,129** | **73,453** |

- Linux の Events.aspx は今回の測定で 14,500〜19,700 req/s と、同じビルドでも回ごとに約 3 割ぶれた(監視を止めたビルドでも同じ範囲)。
  変更による低下ではないと判断した(長い負荷でのノート PC の CPU の発熱とみている)。
- 回帰: テスト Windows 4,402 件・Linux 4,400 件・SqlClientTests 204 件(キャプチャーを遅らせる前の版)、サンプル 18 件(Windows・Linux)、
  関係するコントロール 24 件(Windows・Linux)、be Windows 5/5・Linux 5/5、wt Windows 6/8・Linux 5/8(既知の差)。

### 性能: 静的ファイルが遅かった(2026-10-13、ユーザーの指示)

変換後と IIS を比べた時(PipelineProbe、32 接続、Windows)、ページは変換後が速く(空のページ IIS 16,058・変換後 41,545 req/s、Events.aspx
6,570・27,758。`compilation debug` を false にしても IIS は同じ)、**静的ファイル(Check.htm)だけ逆に IIS 78,521・変換後 3,854**だった。

- **原因**(Windows で dotnet-trace の CPU のトレース): 計算ではなくファイル システムの呼び出しが 7 割。1 要求で、フォルダーかどうかを 3 回
  (ホストの IIS の courtesy redirect、既定のドキュメント、IisStaticFile)、ファイルがあるか、web.config の時刻、さらにファイルを 2 回開いていた
  (`TransmitFile` が長さのため、送る時にもう一度)。1 回は約 20〜30 µs(このマシン、1 本で計測)だが、同時に呼ぶと OS の中で詰まり、
  32 接続で 1 要求あたり約 260 µs になった。ページの要求にはファイル システムの呼び出しは無い(Events.aspx のトレースで確認)。
- IIS はファイルの状態と中身をメモリーに持ち(FileCacheModule、URI のキャッシュ。変更の通知で捨てる)、静的ファイルは http.sys の
  カーネルのキャッシュからも返すので、ファイル システムにほとんど触れない。
- **直したこと**(FileCache、FrameworkOnCore.Web。パッケージは 1.6.5-w2l.15 のまま作り直した): アプリのフォルダーの下のパスの状態(ファイルか
  フォルダーか無いか、属性、長さ、時刻)、フォルダーのファイル名の一覧、256 KB までのファイルの中身(IIS の既定と同じ)を持つ。
  FileSystemWatcher(下のフォルダーも)で何か変われば全部捨てる。調べている間に変更があった結果は持たない。件数 10,000・中身 64 MB まで。
  監視できない時とアプリのフォルダーの外は毎回調べる。ホストの判定(`RedirectToFolder`、`IsLegacyRequest`)と `IisStaticFile`(web.config の
  staticContent の読み取りもパスごとに 1 回)が使う。256 KB を超えるファイルは従来どおりファイルから送る。
- **結果**: 静的ファイル Windows 3,854 → **46,855 req/s**、Linux(コンテナの中から)32,433(直す前の Linux は測っていない)。IIS(カーネルの
  キャッシュ)は 78,521 で、まだ IIS が速い(残りは System.Web のパイプラインを通る分。空のページと同程度)。
- 変更の反映: ファイルを作る・書き換える・消す、フォルダーを作る(301 と既定のドキュメント)を、Windows とコンテナ(inotify。casefs で
  大文字小文字の違う名前も)で確かめた。反映は変更の通知が届いてから(IIS と同じ)。
- 回帰: テスト Windows 4,402 件・Linux 4,400 件・SqlClientTests 204 件、サンプル 18 件(Windows・Linux)、関係するコントロール 24 件(Windows・
  Linux)、be Windows 5/5・Linux 5/5、wt Windows 6/8・Linux 5/8(既知の差)。

### 性能: 変換後の Linux が Windows より遅い理由(2026-10-13、ユーザーの質問)

前節の値で、変換後の Linux は Windows の約 7 割(Events.aspx 32 接続: 19,598 対 27,552 req/s)。CPU は同じ(このマシンは論理 8 個、
Docker Desktop の Linux の仮想マシンも 8 個)。System.Web を使わない最小の ASP.NET Core アプリ(`perf-baseline/`、`perf-baseline/load.ps1`。
負荷は `perf-check.ps1` と同じ: bombardier、32 接続 15 秒、準備運転 5 秒、Linux はコンテナのネットワークの中から)で、System.Web の
処理を 1 つずつ真似て比べた。

| パス | 処理 | Windows | Linux(コンテナ) | 1 要求あたりの追加(Windows / Linux) |
|---|---|---|---|---|
| `/plain` | 6 KB を返すだけ | 78,890 req/s | 63,445 | 基準(12.7 µs / 15.8 µs) |
| `/thread` | + 専用スレッドへの受け渡し(`WorkerThreads` と同じ: 待っているスレッドをセマフォで起こす) | 68,226 | 46,211 | +2.0 µs / **+5.9 µs** |
| `/crypto` | + ViewState と同じ AES-CBC + HMACSHA256 を 2 回(鍵を設定したものをスレッドごとに保持) | 55,319 | 41,723 | +5.4 µs / **+8.2 µs** |
| (前の版) | 計算だけ(SHA256 の繰り返し) | 11,367 | 12,013 | ほぼ同じ |

- **計算そのものの速さは同じ**。差は OS とのやり取りと、スレッドの起こし方に出る。
- 要求を受けて返すだけで Linux が約 2 割遅い(ソケット・システムコールが仮想マシンを通る)。ポートの差のうち約 3 µs。
- **専用スレッドへの受け渡しが約 3 倍**。止まっているスレッドを起こすこと(スケジューラー、仮想 CPU の起床)が仮想マシンの中では
  高い。Linux の p99 が大きいのも同じ理由とみている。ポートの差のうち約 4 µs。
- **暗号化が約 1.5 倍**。Windows(CNG)は鍵のハンドルを保持したまま処理し、.NET の OpenSSL 版は一括の API でも 1 回ごとに暗号の
  コンテキストを作る。ポートの差のうち約 3 µs。
- 変換後の Events.aspx は 1 要求あたり Windows 約 36 µs、Linux 約 51 µs で、差は約 15 µs。上の 3 つで約 10 µs(約 3 分の 2)。
  残り約 5 µs は切り分けていない(要求の中のロック、ファイルの確認、割り当てなどが仮想マシンの中で少しずつ高いと推測)。
- **比べているのは、そのまま動く Windows と、仮想マシン(WSL2)の中の Linux**。Linux のサーバーで直接動かせば、スレッドの起こし方と
  システムコールの差は大きく縮むはず(このマシンでは確かめられない)。Windows の値は回によって 2〜3 割ぶれる。
- `/culture`(カルチャを考慮した文字列比較)も測ったが、Windows は日本語、コンテナは不変カルチャで条件が違い、比べていない
  (Windows 18,588、Linux 48,821 req/s)。
- 縮められそうな点(未着手): 専用スレッドへの受け渡しをやめて Kestrel のスレッドで System.Web を動かす(要求がスレッドを止めるので、
  スレッド プールが足りなくならないかを負荷で確かめる必要がある。専用スレッドにした理由はそれ)。暗号化は .NET 側の実装で、効果は小さい。

### 性能: ViewState の暗号化と GC の負担を減らした(2026-10-13、ユーザーの指示)

前節の続き。`perf-check.ps1 -Diagnose`(Linux の実行中の GC の値を dotnet-counters で、終わった後のヒープとファイナライズ待ちを dotnet-dump で取る。
`perf-counters.py` で集計)を足して調べた。dotnet-trace の「CPU 時間」は待っているスレッド(ファイナライザー、GC で止まったスレッド、セマフォ)も数える
ので、決め手は req/s と GC の値にした。

- **直す前**(PipelineProbe Events.aspx、Linux): 1 要求あたり約 141 KB を割り当て、gen0 35 回/秒・**gen1 25 回/秒**、GC の停止が時間の約 1 割。
- 見つけて直したこと(FrameworkOnCore.Web。パッケージは 1.6.5-w2l.15 のまま作り直した):
  - **応答のバッファーがプールに戻っていなかった**: `AspNetCoreWorkerRequest` が送信の終わりの通知(`SetEndOfSendNotification`)を実装しておらず、
    HttpRuntime の `Request.Dispose`・`Response.Dispose` が呼ばれていなかった。応答のバッファー(ArrayPool の 32 KB)が毎回新しく確保され、
    応答のキャッシュ依存の Dispose、大きな送信の一時ファイルの削除も行われていなかった。IIS と同じく要求の終わりに通知する。
  - その結果見つかった既存の不具合: 出力キャッシュが保持するバッファー(`DisableRecycling`)まで、ポートの `HttpResponseBufferElement.Recycle`
    がプールに返していた。キャッシュから返す WebResource.axd が 0 バイトになった(Content-Length の不一致。コントロールの比較の一括実行で
    見つかった)。保持されたバッファーは返さない(ネイティブのバッファーと同じ)。
  - **ViewState の暗号化**: ページの目的(ページの型・フォルダー・ViewStateUserKey)が要求ごとに作られ、毎回 SP800-108 で鍵を導出し、AES と
    HMAC を毎回作って鍵を設定していた。導出した鍵を目的ごとに保持する(1,000 件まで。超えたら全部捨てる)。組み込みのアルゴリズム(AES・
    3DES・DES、HMACSHA1/256/384/512)は鍵を設定したものをスレッドごとに保持し(128 件まで)、.NET の一括の CBC の API で使う。出力の形式
    (IV || 暗号文 || 署名、CBC・PKCS7)は同じ。独自のアルゴリズム(`alg:`)は従来どおり。イベント検証などの SHA256 は一括の API(`SHA256.HashData`)。
  - `ClientImpersonationContext`・`ApplicationImpersonationContext`(要求の各段階で作る)がファイナライザーに回っていた。トークンを保存した時だけ
    ファイナライズする。
  - ハンドラーの照合の正規表現を要求ごとに作っていた(`IntegratedModeConfiguration`)。パスごとに 1 回にした。`SimpleWorkerRequest` の
    `Assembly.GetCallingAssembly()`(スタックをたどる)を、System.Web のロード コンテキストを直接使う形にした。各段階の `() => Context?.Response`
    のクロージャを static のラムダにした。
- **直した後**: 1 要求あたり約 44 KB、gen1 は 0 回/秒。同じマシン・32 接続 20 秒(IIS は前節の値。変えていない):

| ページ | 接続数 | IIS(前節) | 変換後 Windows(前節 → 今回) | 変換後 Linux(前節 → 今回) |
|---|---|---|---|---|
| PipelineProbe Events.aspx | 32 | 6,585 req/s | 20,350 → 27,552 | 12,908 → 19,598 |
| CompileProbe Dynamic.aspx | 32 | 6,206 | 16,738 → 20,730 | 12,042 → 15,169 |
| PipelineProbe Events.aspx | 8 | 5,946 | 15,893 → 25,611 | 9,641 → 16,241 |
| CompileProbe Dynamic.aspx | 8 | 5,400 | 14,710 → 19,791 | 9,225 → 14,145 |
| SessionProbe Default.aspx | 32 | 608 | 793 → 840 | 727 → 739 |

  Windows の値は回によって 2〜3 割ぶれる(同じ版で 20,177〜31,279)。SessionProbe はセッションの一覧が増え続けるページで、ほとんど変わらない。
- 回帰: テスト Windows 4,402 件・Linux 4,400 件・SqlClientTests 204 件、サンプル 18 件(Windows・Linux)、関係するコントロール 24 件(Windows・
  Linux)、be Windows 5/5・Linux 5/5、wt Windows 6/8・Linux 5/8(既知の差)。

### 性能: 要求の処理の負担を減らした(2026-10-13、ユーザーの指示)

`perf-check.ps1`(bombardier で同じ負荷: 32 接続 20 秒、準備運転 5 秒。Linux はアプリのコンテナのネットワークの中から負荷をかけ、Docker
Desktop のポート転送を通さない。`-Trace` で dotnet-trace の CPU のトレース、`perf-frames.py` で集計)で測り、トレースで分かった負担を除いた。

- **直す前**(PipelineProbe の Events.aspx): IIS 6,508 req/s、変換後 Windows 2,404、Linux(コンテナの中から)2,826。ポート転送は主因ではなかった。
  Linux の CPU の内訳: 要求ごとのスレッドの起動(`Task.Factory.StartNew` の LongRunning)31〜34%、ファイナライザー 17%(要求ごとに作って
  使わず捨てていた `AutoResetEvent`)、新しいスレッドのスレッド静的フィールドの初期化 9%、ASP.NET Core の要求ごとのログ(Information、
  コンソール)約 10%、ロックの競合 12%(ほぼ上の 2 つ)、web.config の時刻の確認(要求ごと)3%。ページ自体は 5%。
- 直したこと:
  - **System.Web のワーカー スレッドを使い回す**(`WorkerThreads`。FrameworkOnCore.Web): IIS のワーカー プロセスのように専用のスレッド
    (スレッド プールではない: 要求はスレッドを止め、Kestrel の I/O を完了させるのはスレッド プール)。空いているスレッドが無ければ新しく
    作る(要求は他の要求を待たない)、1 分使われなければ終わる(プロセッサー数は残す)。要求の実行コンテキストで動かし、終わったら
    スレッドの IllogicalCallContext を消す。待ち方は共有のセマフォ(スレッドごとのイベントで LIFO に渡す形も試したが、起こすのが遅く
    3 回平均で 17,800 対 19,400 req/s)。
  - 要求ごとの未使用の `AutoResetEvent` を消した。web.config の時刻の確認は 1 秒に 1 回まで(`IntegratedModeConfiguration`)。
  - **変換器のホスト(Program.cs・Program.vb のテンプレート)**: `Microsoft.AspNetCore` のログを Warning 以上に(アプリの設定
    `Logging:LogLevel:Microsoft.AspNetCore` があればそちら)。IIS は要求を自分のログ ファイルに書く。
- **直した後**(同じマシン。IIS は Windows 11 の IIS: クライアントの OS は同時要求が 10 までに制限される):

| ページ | 接続数 | IIS | 変換後 Windows | 変換後 Linux(コンテナ) |
|---|---|---|---|---|
| PipelineProbe Events.aspx | 32 | 6,585 req/s | 20,350 req/s | 12,908 req/s |
| CompileProbe Dynamic.aspx(Repeater、ユーザーコントロール) | 32 | 6,206 | 16,738 | 12,042 |
| PipelineProbe Events.aspx | 8(IIS の制限の内) | 5,946 | 15,893 | 9,641 |
| CompileProbe Dynamic.aspx | 8 | 5,400 | 14,710 | 9,225 |
| SessionProbe Default.aspx(要求ごとに新しいセッション、Global の一覧が増え続ける) | 32 | 608 | 793 | 727 |

  Windows Server の IIS(制限なし)とは比べていない。Linux は Docker Desktop の Linux の仮想マシン(負荷のツールも同じ中)。
- 回帰: テスト Windows 4,402 件・Linux 4,400 件・SqlClientTests 204 件、サンプル 18 件(Windows・Linux)、関係するコントロール 24 件(Windows・
  Linux)、be Windows 5/5・Linux 5/5、wt Windows 6/8・Linux 5/8(既知の差)。

### 動的コンパイル・ルーティング・サービス・同時アクセス・制限値の新旧比較(2026-10-12、ユーザーの指示)

前回の残り(優先度中・低)を、サンプル単位の新旧比較(`sample-parity.ps1`)で確かめた。プローブを 6 つ足し、IIS 側に正解の無いもの(リバースプロキシ、
負荷)は確認用のスクリプトにした。

- `samples/CompileProbe`: App_Code、CodeFile のページ・ユーザーコントロール(web.config で登録)、`<%: %>`・`<%# %>`・`<%$ %>`、テーマとスキン
  (ディレクティブ、PreInit、スタイルシート テーマ)、グローバル・ローカルのリソース、`culture="auto"` の Accept-Language(ja-JP、fr、de-DE)。
- `samples/RoutingProbe`: `MapPageRoute`(既定値、制約、キャッチオール、DataTokens)、独自の IRouteHandler、`RouteValue`・`RouteUrl` 式、`GetRouteUrl`、
  ルートの URL へのポストバック、`RedirectToRoute(Permanent)`、`RewritePath`、urlMappings。`Where.aspx` はリバースプロキシの確認用。
- `samples/ServicesProbe`: ASMX の JSON(ScriptService、`__type`、セッション、エラー、GET)、SOAP 1.1・1.2(フォルト)、WSDL、`/js` の
  プロキシ、ヘルプページ、ページメソッド、ScriptManager のプロキシからの呼び出し、アプリのコードの System.Web.Services.Description。
- `samples/Toolkit20Probe`: Ajax Control Toolkit 20.1(FrameworkOnCore の Toolkit の版)の TabContainer、Accordion、ウォーターマーク、マスク、
  カレンダー、NumericUpDown、ModalPopup、CollapsiblePanel、ConfirmButton、UpdatePanel。**4.1(nop の版)のページは、変換後(20.1)は DOM
  が違う**(タブの見出し、CSS クラス、z-index など。版の違い)。ToolkitProbe は 4.1 の互換(ToolkitScriptManager)の確認のまま。
- `samples/ConcurrencyProbe`: 同じセッションの同時要求(ブラウザーの並列 fetch): 書き込み同士は順番、読み取り専用同士は同時、書き込みと
  読み取り専用は順番、セッションなしは並列。
- `samples/LimitsProbe`: maxRequestLength(フォルダーごと)、IIS の要求フィルタリング(本文 30,000,000 バイト、クエリ 2,048)、フォームの
  キーの数、http.sys のパスの区切りの長さ(260)、executionTimeout。
- **結果: 6 つとも Windows・Linux とも全件一致**。既存のプローブ・サンプル 12 件も Windows・Linux とも一致(全 18 件)。
- **見つけて直したポートの不具合**(FrameworkOnCore.Web、.Web.Services、.Configuration は変えず。パッケージは 1.6.5-w2l.15 のまま作り直した):
  - **executionTimeout を超えた要求が終わらなかった**(debug="false" の時。既定 110 秒): 時間切れで取り消し状態になり、来ない Thread.Abort を
    待ち続けた。.NET では中断の代わりに印を付け、次の段階(パイプライン、`Response.End`・`Flush`)で "Request timed out." にする(Page_Error と
    HttpUnhandledException を通らないのも .NET Framework と同じ)。ページのコード自体は最後まで動く(止められない)。
  - **ASMX のヘルプページが空**: 既定の設定(ヘルプページ)が「ASP.NET の中か」を `AppDomain.GetData(".appDomain")` で見ていて、ポートは
    その値をロード コンテキストにしか置いていなかった。AppDomain にも置く(System.Configuration・Uri の設定、アプリのライブラリも同じ値を見る)。
    ヘルプページ(`DefaultWsdlHelpGenerator.aspx`)は FrameworkOnCore 独自のものを書いて同梱し、App_Data(machine.config の場所)に書き出す。
  - **ランタイムコンパイルで `ServiceDescription` などがあいまい**(CS0433): CoreWCF が依存する Microsoft の System.Web.Services.Description と
    ポートの System.Web.Services が同じ型を持つ。コンパイラーに渡す参照からポートと重なる前者を外す。
  - **IIS・http.sys がアプリの前で拒否するもの**: 本文が maxAllowedContentLength(既定 30,000,000)を超える(IIS は 413、ポートは Kestrel の例外が
    アプリの 500、ブラウザーは送信中に切断)、クエリが maxQueryString(2,048)を超える(IIS は 404.15、ポートは ASP.NET の 400)、パスの区切りが
    260 文字を超える(http.sys の 400、ポートは 500)。web.config の `requestFiltering/requestLimits` を読んで同じ状態で拒否し、Kestrel の本文の上限を
    それに合わせる(大きな送信を許す設定のアプリ)。413 では本文を読み捨ててから閉じる。
  - **IPv4 の接続元が `::ffff:172.19.0.3`**(Kestrel のデュアルモードのソケット): REMOTE_ADDR・UserHostAddress・IsLocal を IIS と同じ `172.19.0.3` に。
- 変換器の部品の修正: **ICU に無いカルチャ(Accept-Language の未知の言語タグ)で 500**(Linux、カルチャのプロファイルあり): CultureIcu がその
  カルチャの ICU リソースを作り、.NET の月名が短くなって 3 月の日付で ArgumentOutOfRange。ICU に無いカルチャは作らずに報告する。その書式は
  ICU の既定ロケールのもの(.NET Framework は不変カルチャ)で、既知の差としてシナリオでは状態だけを比べる。
- 仕組み: `sample-parity.ps1` に `cultures.txt`(このマシン=IIS 側で `capture-culture.ps1` を取り、`--culture-profile` で変換。Linux の ICU を
  合わせる)。`Check.htm` に要求ヘッダー・メソッド・本文・本文の表示・並列。ParityTest は no-store で network idle にならないページを待つ。
- リバースプロキシ(`proxy-check.ps1`、nginx のコンテナを前に置く): `ASPNETCORE_FORWARDEDHEADERS_ENABLED=true` で、Request.Url が https と
  公開のホスト・ポート、IsSecureConnection が True、UserHostAddress がクライアント、絶対 URL のリダイレクトが https。無しでは http と
  プロキシのアドレス(設計どおり)。ミドルウェアは X-Forwarded-For を使って消す(自分で読むアプリは空を見る。UserHostAddress は正しい)。
- 負荷とメモリ(`load-check.ps1`、curl の並列。参考値): セッションを作るページ(SessionProbe、要求ごとに新しいセッション)とそうでないページ
  (PipelineProbe Events.aspx)で、IIS・Windows・Linux ともエラー 0。最初の応答は IIS 約 0.5 秒、ポート約 3〜4 秒(Roslyn のコンパイル)。
  速さは IIS の約 6〜9 割(Windows)、メモリは約 2 倍(約 220 MB、IIS 約 90 MB)。セッションなしのページは 6 回(各 5,000 件)でメモリが増えない。
- 回帰: テスト Windows 4,402 件・Linux 4,400 件・SqlClientTests 204 件、サンプル 18 件(Windows・Linux)、関係するコントロール 24 件(Windows・
  Linux)、be Windows 5/5・Linux 5/5、wt Windows 6/8・Linux 5/8(既知の差)。

### 認証・要求処理・キャッシュの新旧比較(2026-10-12、ユーザーの指示)

コントロールの外側を、サンプル単位の新旧比較(`sample-parity.ps1`、IIS の .NET Framework 4.8 で記録して変換後を Windows・Linux で比べる)で確かめた。
プローブを 3 つ足した。

- `samples/AuthProbe`: フォーム認証と `<location>` の `<authorization>`、SQL Server の Membership・Role・Profile(aspnet_regsql のスキーマ)、
  Login コントロール(ReturnUrl への戻り)、チケット・ロール・プロファイルの保存、パスワード変更、ロックアウト、サインアウト、永続チケット。
  要求の検証(フォーム・クエリ文字列のマークアップ、`ValidateRequest="false"`、`Request.Unvalidated`)、ViewState の MAC、イベント検証。
- `samples/PipelineProbe`: Global.asax・`system.webServer/modules` のモジュール・ページのイベント順(Render の後も次の要求で)、
  `system.webServer/handlers` のハンドラー(同期・`HttpTaskAsyncHandler`)、async ページ(async void の Page_Load、`RegisterAsyncTask`)、
  `Server.Transfer`(フォームあり・なし)・`Server.Execute`・`Response.End`、customErrors(例外、403・404 の HttpException、無いページ)、
  `Application_Error`・`Page_Error` での処理、応答の状態とヘッダー。
- `samples/CachingProbe`: 出力キャッシュ(VaryByParam、プロファイル、Substitution、`RemoveOutputCacheItem`、`AddCacheItemDependency`)、
  OutputCache の Location とコードのキャッシュ ポリシーのヘッダー、Cache API(絶対・スライディングの期限、キー・ファイルの依存、Add、Remove、
  削除のコールバック)、静的ファイルのヘッダー(`staticContent/clientCache`)と種類(`mimeMap`、IIS が知る種類・知らない種類)、配らないフォルダー。
- 仕組み: `sample-parity.ps1` に `database.json`(実行ごとに SQL Server Express にデータベースを作り直し、aspnet_regsql のスキーマを入れる。
  IIS のプールに権限、コンテナには host.docker.internal と `_verify\sql-login.txt` のログインで `SQLCONNSTR_`)。応答の状態とヘッダーを
  ブラウザーの fetch で書き出す `Check.htm`(日付など実行ごとに変わる値は有無だけ)。ParityTest は no-store の応答を fetch するページで
  network idle にならない(Edge)時も読み込みを待って続ける。WebControlsParity の値の書き出しは今日の日付を `{today}` にする
  (Calendar の `TodaysDate`。記録した日と違う日に差が出ていた)。
- **結果: 3 つとも Windows・Linux とも全件一致**(AuthProbe 22 スナップショット・リンク 9 件、PipelineProbe 18・20 件、CachingProbe 18)。
- **見つけて直したポートの不具合**(FrameworkOnCore.Web。パッケージは 1.6.5-w2l.15 のまま作り直した。公開はしていない):
  - **統合モードのイベント**: `MapRequestHandler`・`LogRequest`・`PostLogRequest` が発生しなかった(ポートはクラシックの手順で動き、モジュールが
    登録すると PlatformNotSupportedException)。IIS の統合モードと同じ位置で発生させる(LogRequest は CompleteRequest の時も)。
  - **await の後の `HttpContext.Current` が null**(HttpTaskAsyncHandler など)。.NET Framework は IllogicalCallContext(スレッドのもの、流れない)に
    置き、await の後は ASP.NET の同期コンテキストが設定し直す。ポートは AsyncLocal の共有の辞書に置いていて、要求がスレッドを離れる時に書いた
    null が await の後に見えていた。`CallContext.HostContext` と `SetData` を .NET Framework と同じくスレッドのものにした(`LogicalSetData` は流れる)。
  - **静的ファイルが IIS の静的ファイル モジュールと違った**(ASP.NET の StaticFileHandler の動き): `Cache-Control: public` と 1 日先の Expires、
    `X-AspNet-Version`、ETag・Last-Modified・Accept-Ranges なし(ホストが捨てていた)、304 なし、`staticContent` の `mimeMap`・`clientCache` を
    読まない、MIME の種類が違う(.json が application/octet-stream、IIS が知らない .webp・.mjs・.webmanifest を配る)。IIS 10 の MIME の表と
    web.config(フォルダーの web.config、`<location>`)の `staticContent` で返す `IisStaticFile` にした。
  - **無い静的ファイルが customErrors の 404 ページへ**(IIS は自分の 404。Application_Error も起きない)。要求フィルタリングの 404(web.config、
    App_Data、bin)は本文なしだった。IIS の 404 ページを返す。
  - **HttpUnhandledException の既定のメッセージ**: .NET の ExternalException は null のメッセージに独自の文("External component has thrown an
    exception.")を入れる。.NET Framework と同じ "Exception of type '...' was thrown." を返す。
  - **ファイル依存のキャッシュ項目が、変わっていないのに消えた**(タイミング): FileSystemWatcher の通知は処理した時刻しか持たず、依存を作る直前の
    書き込みの通知が「監視開始の後の変更」になっていた。監視を始めた時のファイルの属性と同じなら変更なしとする。
  - `SuppressDefaultCacheControlHeader` がクラシックの手順と HttpCachePolicy の既定で効いていなかった(統合モードと同じく効かせる)。
- IIS 側で分かったこと(差分ではない): IIS の実行時コンパイルは C# 5(ページの中の式本体のメンバーはエラー)。Duration だけの OutputCache の
  ページはブラウザーがキャッシュする(`max-age`)ので、サーバーの無効化を見るページには Substitution を置いた。削除のコールバックの順は
  Hashtable の順(.NET Framework でもオブジェクトのハッシュ次第)なので並べて比べる。
- 回帰: テスト Windows 4,402 件・Linux 4,400 件(SQL Server あり)・SqlClientTests 204 件、既存のサンプル 9 件(Windows・Linux)、
  関係するコントロール 24 件(Windows・Linux)、be Windows 5/5・Linux 5/5、wt Windows 6/8・Linux 5/8(既知の差)。

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
