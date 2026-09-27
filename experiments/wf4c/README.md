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

    .\experiments\wf4c\convert-corpora.ps1          # 6 本を変換してビルド(レポートは <out>\CONVERSION-REPORT.md)
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
| dnn | 成功。元のビルドは DNN 自身の Cake ビルド(`--build-original`)。VB の DotNetNuke.WebUtility は配置済みサイトの .NET Framework の DLL をそのまま参照する | Windows: **インストール(`Install.aspx?mode=install`)が完了し、トップページが表示される**(`Home`。DB は `.\SQLEXPRESS` の `dnn_w2l`。`dnn-cycle.ps1` で DB の作成から通す)。**Linux でも**、空の DB からインストールが完了し(サイトの作成、スキンなどのモジュールの導入)、トップページ・`/Login`・`/Terms` が 200、ページの CSS・JS・画像 21 件がすべて 200、host でのログインが通る(`run-linux-site.ps1`、SQL Server のコンテナ。下の「Linux で動かすための書き換え」) |
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
- 変換しないプロジェクト(VB)は、配置済みサイトにあるその DLL を参照する(出力の `.deployed` にコピー)。
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
- `WindowsIdentity.GetCurrent().Name`: Windows 以外は例外。
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
- mojo・yaf は確認していない(変換器の変更は影響しうる)。

変換器を作る過程で直したこと:
- 書き換えの後のビルドで、SYSLIB の警告(`obsoletions`)を消していた。後のビルドは変わったプロジェクトしかコンパイルしないので、ほかのプロジェクトの分がレポートから落ちていた(dnn で 19 件)。
- NuGet の packages フォルダーの判定: DNN のソースフォルダー `Services\Installer\Packages` を除外していた。中身(.nupkg、repositories.config)で判定するようにした。convert-project.ps1(robocopy `/XD packages`)にも同じ問題がある。
- アナライザーのプロジェクト参照(`OutputItemType="Analyzer"`)と Aliases の引き継ぎ: DNN はソースジェネレーターで部分メソッドの定義側を生成する(無いと CS0759)。
- 変換したプロジェクトでは `TreatWarningsAsErrors` を外す: 変換で加えた編集が StyleCop の警告になり、.NET の SYSLIB の警告もエラーになっていた。
- テンプレートのファイル名: `Program.cs.txt` の `cs` が MSBuild にチェコ語のカルチャと解釈され、サテライトアセンブリに回っていた。

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
