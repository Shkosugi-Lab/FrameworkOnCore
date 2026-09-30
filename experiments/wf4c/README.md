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
`pack-fork.ps1` で `1.6.5-w2l.3` として `_feed/` にパッケージ化し、テンプレートはそれを参照する。

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
| 0024 | マシンキー: 自動生成の検証・暗号化キーを、再起動の後も同じものにする(.NET Framework はレジストリに保存する。ここでは `LocalApplicationData/WebFormsForCore/AutogenKeys`、Unix ではモード 600) | dnn のインストール後の再起動で、ViewState の MAC の検証に失敗した |
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

フォークのビルドは `pack-fork.ps1 -Build All`(`fork.slnx`)で行う。WebFormsForCore.Build は先に一度だけビルドしておく。
fork.slnx に含めると、読み込み済みのタスク DLL とコピーがぶつかって失敗する。

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

nopCommerce 3.90(`nop390`、MVC 5 の最後の版)は取得と表への追加まで。

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
  - 足りない道具は、キャッシュ(`%LOCALAPPDATA%\FrameworkOnCore\tools`)に用意する。global.json の .NET SDK(rollForward を見て、インストール済みのもので足りなければ dotnet-install)、package.json の `packageManager`(corepack。Node.js 25 以降は同梱されない)。
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

フォーク: Ajax Control Toolkit(サブモジュール `src/WebFormsForCore.AjaxControlToolkit`)もパッケージにする(`pack-fork.ps1`、`fork.slnx`)。openIMIS が使う。初回は `git submodule update --init src/WebFormsForCore.AjaxControlToolkit`。

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

フォークのパッケージ: Ajax Control Toolkit は net10.0 だけでビルド・パッケージにする(`pack-fork.ps1`)。ソリューションの中で net8.0 のビルドが CS7069 で失敗するようになった。`-f net10.0` でビルドし、`--no-restore` でパッケージにする(`TargetFrameworks` をグローバルプロパティで渡すと、参照先のプロジェクトまで net10.0 だけで復元され、そのパッケージが作れなくなる)。ソリューションのビルドの再試行は 3 回まで。
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
