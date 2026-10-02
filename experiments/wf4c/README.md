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

ASP.NET 1.1 のモバイル コントロール(referencesource の System.Web.Mobile、MIT)。上流にプロジェクト(`src/WebFormsForCore.Web.Mobile`)があり、ビルドはできたが動かなかった。それを直して FrameworkOnCore.Web.Mobile として配る。アセンブリは System.Web.Mobile 10.0.29.0、公開キーは .NET Framework と同じ(b03f5f7f11d50a3a)、net8.0・net10.0。

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

フォークのパッケージ: Ajax Control Toolkit は net10.0 だけでビルド・パッケージにする(`pack-frameworkoncore.ps1`)。ソリューションの中で net8.0 のビルドが CS7069 で失敗するようになった。`-f net10.0` でビルドし、`--no-restore` でパッケージにする(`TargetFrameworks` をグローバルプロパティで渡すと、参照先のプロジェクトまで net10.0 だけで復元され、そのパッケージが作れなくなる)。ソリューションのビルドの再試行は 3 回まで。
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
