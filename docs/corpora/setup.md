# 共通の準備と使い方

コーパス(変換の確かめに使っている、実在のオープンソースの .NET Framework の Web アプリ)で FrameworkOnCore の変換を試す前に、
**一度だけ** 行う準備と、どのコーパスでも同じ使い方(変換、レポートの読み方、Windows・Linux での動かし方、後片付け、うまくいかないとき)をまとめます。

コーパスごとの手順は [コーパスの一覧](README.md) から各ページへ進んでください。各ページは、このページの準備が終わっている前提で書いています。

- ASP.NET(Web Forms・MVC)は分かるけれど、このリポジトリの中身は知らない、という人を想定しています。
- 「確認」は、その手順がうまくいったかの確かめ方です。確認が通ってから次の手順へ進んでください。
- この手引きのコマンドは、とくに書いていなければ **リポジトリの一番上のフォルダー** で、Windows PowerShell 5.1 で実行します。

目次

1. [入れるもの(一覧)](#1-入れるもの一覧)
2. [準備の手順](#2-準備の手順)
3. [ディスクの空き](#3-ディスクの空き)
4. [変換のしかた(2 通り)](#4-変換のしかた2-通り)
5. [変換レポートの読み方](#5-変換レポートの読み方)
6. [Windows で動かす](#6-windows-で動かす)
7. [Linux で動かす](#7-linux-で動かす)
8. [動いているかの確かめ方](#8-動いているかの確かめ方)
9. [後片付け](#9-後片付け)
10. [うまくいかないとき](#10-うまくいかないとき)
11. [自分のアプリで試すとき](#11-自分のアプリで試すとき)
12. [関連する資料](#12-関連する資料)

---

## 1. 入れるもの(一覧)

| もの | 要るとき | 入れる手順 |
|---|---|---|
| Windows 10 / 11(x64) | いつも | [手順 1](#手順-1-windows-を確かめる)。変換は Windows で行います(元のアプリのビルドに Windows の MSBuild を使うため) |
| .NET 10 SDK | いつも | [手順 2](#手順-2-net-10-sdk-を入れる) |
| Git | いつも | [手順 3](#手順-3-git-を入れる)。`mvcmovie`・`contoso` の取得(`git clone`)と、Studio・変換器のビルドの取得(コミットを調べる)に使います |
| Windows PowerShell 5.1 | いつも | Windows に入っています。[手順 4](#手順-4-powershell-のスクリプトを動かせるようにする) |
| Visual Studio Build Tools 2022 以降(Web のビルド ツール、.NET Framework 4.8 の Targeting Pack) | 元のアプリのビルドが要るコーパス: mojo、dnn、n2、imis、nop、nop390 | [手順 6](#手順-6-visual-studio-build-tools-を入れる元のビルドが要るコーパスだけ)。be・wt・mvcmovie・contoso・yaf では要りません |
| SQL Server Express(インスタンス名 `SQLEXPRESS`)と `sqlcmd` | wt、mojo、yaf、dnn、imis、nop、nop390、mvcmovie、contoso | [手順 7](#手順-7-sql-server-express-と-sqlcmd-を入れる)。Windows 認証で使います。be と n2 は要りません |
| SQL Server Express の混合モード認証・TCP 1433・テスト用のログイン | 任意(コンテナからこの PC の SQL Server を使うとき、`studio-verify.ps1`・`sample-parity.ps1`) | [手順 8](#手順-8-任意sql-server-express-を混合モードtcp-1433-にしテスト用のログインを作る) |
| Microsoft Edge | 元のアプリとの比較(`verify-windows.ps1`、`run-linux.ps1`) | Windows に入っています。[手順 9](#手順-9-edge-を確かめる比較をするときだけ) |
| Docker Desktop(WSL 2) | Linux で動かすとき | [手順 10](#手順-10-docker-desktop-を入れるlinux-で動かすときだけ) |
| ファイル名の大文字・小文字のライブラリ(casefs) | Linux で動かすとき | [手順 11](#手順-11-casefs-をビルドするlinux-で動かすときだけ)。Docker でビルドします |

要らないもの:

- **Node.js**: DNN の元のビルドが使いますが、無ければ変換器が自分のキャッシュ(`%LOCALAPPDATA%\FrameworkOnCore\tools`)に取ってきます。
- **IIS**: 元のアプリの正解の記録(`record-*.ps1`)を採り直すときだけ要ります。正解はリポジトリに入っているので、試すだけなら要りません。
- **nuget.exe、.NET Framework の参照アセンブリ**: 変換器が自動で取ってきます。
- **Python**: 比較のための操作の棚卸しとシナリオ(`corpora\regression\*-operations-*.py`)を書き直すときだけ使います。試すだけなら要りません。
- **Playwright のブラウザー**: 比較の道具(ParityTest)は Playwright で、この PC の Edge を画面無しで動かします。別のブラウザーを入れる必要はありません。

---

## 2. 準備の手順

### 手順 1. Windows を確かめる

Windows 10 または 11 の 64 ビット版を使います。

確認(PowerShell で):

```powershell
[Environment]::Is64BitOperatingSystem      # True
[Environment]::OSVersion.Version           # 10.0.xxxxx
```

### 手順 2. .NET 10 SDK を入れる

https://dotnet.microsoft.com/download/dotnet/10.0 から「SDK」の Windows x64 のインストーラーを入れます。

確認:

```powershell
dotnet --list-sdks          # 10.0.xxx が出ること
```

### 手順 3. Git を入れる

https://git-scm.com/download/win から入れます。

確認:

```powershell
git --version
```

### 手順 4. PowerShell のスクリプトを動かせるようにする

「このシステムではスクリプトの実行が無効になっている」と出ないように、自分のユーザーだけ許可します。

```powershell
Set-ExecutionPolicy -Scope CurrentUser RemoteSigned
```

確認:

```powershell
$PSVersionTable.PSVersion          # 5.1.xxxxx
Get-ExecutionPolicy -Scope CurrentUser   # RemoteSigned
```

許可したくないときは、`powershell -ExecutionPolicy Bypass -File .\experiments\wf4c\convert-corpora.ps1 -Only be` のように実行します。

### 手順 5. リポジトリを取る

```powershell
git clone https://github.com/Shkosugi-Lab/FrameworkOnCore.git C:\src\FrameworkOnCore
cd C:\src\FrameworkOnCore
```

- フォルダーの場所は **短いほうが安全** です(.NET Framework の MSBuild には 260 文字のパスの上限があります)。
- この手引きのコマンドは、すべてこのフォルダー(リポジトリの一番上)で実行します。
- リポジトリが非公開のときは、GitHub CLI にログインしておくか(`gh auth login`)、環境変数 `GH_TOKEN` を設定しておきます(パッケージと、Studio・変換器のビルドの取得に使います)。

確認:

```powershell
Test-Path .\studio.ps1, .\converter.ps1, .\corpora\fetch.ps1      # どれも True
```

### 手順 6. Visual Studio Build Tools を入れる(元のビルドが要るコーパスだけ)

mojo、dnn、n2、imis、nop、nop390 は、元のアプリをそのビルド手順でビルドしてから変換します。そのために Visual Studio(Build Tools)の MSBuild が要ります。
be・wt・mvcmovie・contoso・yaf だけを試すなら、この手順は飛ばして構いません。

変換器が足りないときに出すメッセージと同じコマンドで入れられます(**管理者の** PowerShell で):

```powershell
winget install Microsoft.VisualStudio.2022.BuildTools --override "--quiet --wait --add Microsoft.VisualStudio.Workload.WebBuildTools --add Microsoft.VisualStudio.Workload.MSBuildTools --add Microsoft.Net.Component.4.8.TargetingPack --includeRecommended"
```

.NET Framework 4.8 より前の版の参照アセンブリは、変換器が自動で取ってきます(別に入れる必要はありません)。

確認:

```powershell
& "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -products * -requires Microsoft.Component.MSBuild -property installationPath
# Build Tools のフォルダー(...\2022\BuildTools など)が出ること
```

入っていないまま元のビルドが要るコーパスを変換すると、「Visual Studio (Build Tools) 2022's MSBuild is not installed」と出ます。

### 手順 7. SQL Server Express と sqlcmd を入れる

wt、mojo、yaf、dnn、imis、nop、nop390、mvcmovie、contoso で使います(be と n2 は要りません)。

1. https://www.microsoft.com/sql-server/sql-server-downloads から SQL Server Express を入れます。**インスタンス名は `SQLEXPRESS`**(Express の既定)のままにします。スクリプトは `.\SQLEXPRESS` という名前を前提にしています。
2. `sqlcmd`(SQL Server のコマンドライン ツール。https://learn.microsoft.com/sql/tools/sqlcmd/sqlcmd-utility )を入れます。convert-corpora(dnn・imis)と verify-windows(wt・mvcmovie・contoso)が使います。スクリプトは `-C`(サーバーの証明書を信頼する)を付けて呼ぶので、それが使える版にします。
3. Windows 認証で使います。アプリを動かす自分のユーザーに、DB を作る権限が要ります(SQL Server Express を入れたユーザーなら持っています)。

確認:

```powershell
Get-Service 'MSSQL$SQLEXPRESS'                                   # Status が Running
sqlcmd -S .\SQLEXPRESS -E -C -Q "SELECT @@VERSION"               # 版が出ること
```

止まっていたら、**管理者の** PowerShell で `Start-Service 'MSSQL$SQLEXPRESS'`。使わないときは止めておきます([9. 後片付け](#9-後片付け))。

### 手順 8. (任意)SQL Server Express を混合モード・TCP 1433 にし、テスト用のログインを作る

ふだんのコーパスの手順は Windows 認証だけで動くので、**この手順は無くても構いません**。次のときに要ります。

- コンテナ(Linux)の中から、この PC の SQL Server を使うとき(`host.docker.internal` と SQL Server の認証で入ります。コンテナからは `.\SQLEXPRESS` や LocalDB は見えません)
- `studio-verify.ps1`、`sample-parity.ps1`(データベースを使うサンプル)を動かすとき。どちらも `experiments\wf4c\_verify\sql-login.txt` に書いたテスト用のログイン(`User ID=foc_verify;Password=...`。このリポジトリのテスト用で、本物のアカウントではありません)を使います([tests/README.md](../../tests/README.md) の「新しいマシンに要るファイル」)

やること:

1. 混合モード認証(SQL Server 認証と Windows 認証)にする: SQL Server Management Studio のサーバーのプロパティの「セキュリティ」で「SQL Server 認証モードと Windows 認証モード」を選びます。
2. TCP 1433 で待ち受ける: 「SQL Server 構成マネージャー」の「SQL Server ネットワークの構成」→「SQLEXPRESS のプロトコル」で TCP/IP を有効にし、TCP/IP のプロパティの「IP アドレス」→「IPAll」の「TCP ポート」を `1433` にします(「TCP 動的ポート」は空にします)。
3. SQL Server Express を再起動します(管理者で `Restart-Service 'MSSQL$SQLEXPRESS'`)。
4. `sql-login.txt` と同じ名前とパスワードのログインを作ります。ログインに要る権限は記録に書かれていません。下の例では、アプリが自分で DB を作れるように `dbcreator` を付けています(`studio-verify.ps1` では、wt・mvcmovie などのアプリがこのログインで DB を作るため)。

```powershell
$login = (Get-Content experiments\wf4c\_verify\sql-login.txt -Raw).Trim()      # User ID=foc_verify;Password=...
$password = ($login -split ';' | Where-Object { $_ -like 'Password=*' }) -replace '^Password=', ''
sqlcmd -S .\SQLEXPRESS -E -C -Q "CREATE LOGIN foc_verify WITH PASSWORD = N'$password', CHECK_POLICY = OFF; ALTER SERVER ROLE dbcreator ADD MEMBER foc_verify"
```

確認:

```powershell
sqlcmd -S "tcp:127.0.0.1,1433" -U foc_verify -P $password -C -Q "SELECT SUSER_NAME()"    # foc_verify が出ること
```

### 手順 9. Edge を確かめる(比較をするときだけ)

元のアプリとの比較(`verify-windows.ps1`、`run-linux.ps1`)は、比較の道具(ParityTest)が Playwright で Edge を画面無しで動かします。Edge は Windows に入っています。

確認:

```powershell
Test-Path "${env:ProgramFiles(x86)}\Microsoft\Edge\Application\msedge.exe"    # True
```

比較が終わったら、残っている Edge のプロセスも閉じます。

### 手順 10. Docker Desktop を入れる(Linux で動かすときだけ)

1. WSL 2 を入れます(管理者の PowerShell で `wsl --install`。再起動を求められたら再起動)。
2. https://docs.docker.com/desktop/setup/install/windows-install/ から Docker Desktop を入れ、WSL 2 のバックエンドを使います。
3. WSL 2 のメモリーの上限を決めます。このリポジトリの開発マシンはメモリー 16 GB で、WSL 2 に 8 GB を割り当てています。`%USERPROFILE%\.wslconfig` に次のように書きます。

```ini
[wsl2]
memory=8GB
```

書いたら `wsl --shutdown` で WSL を止め、Docker Desktop を起動し直すと効きます。

確認:

```powershell
docker version          # Client と Server の両方が出ること(Server が出なければ Docker Desktop が起動していない)
docker info --format "{{.MemTotal}}"   # 8 GB 前後(.wslconfig の値)
```

メモリーについての約束:

- SQL Server のコンテナは 2 GB ほど使います。**コンテナは一度に一つ(DB の要るものは SQL Server とアプリの 2 つ)** にし、終わったら止めます。いくつもの検証をバックグラウンドで重ねて動かさないでください。
- 使っていない SQL Server Express と Edge も閉じます。
- 終わったら Docker Desktop を止めて `wsl --shutdown`(WSL のメモリーが返ります)。[9. 後片付け](#9-後片付け)

### 手順 11. casefs をビルドする(Linux で動かすときだけ)

Linux でもファイル名の大文字・小文字を区別しないで動かすためのライブラリ(`libfoccase.so`)は Git に入っていません。
新しいマシンでは、**変換の前に一度** ビルドしておくと、変換が書く Dockerfile にも入ります(Docker が要ります。Docker Desktop を起動しておきます)。

```powershell
.\experiments\wf4c\casefs\build.ps1
```

確認:

```powershell
Test-Path experiments\wf4c\casefs\out\linux-x64\libfoccase.so, experiments\wf4c\casefs\out\linux-arm64\libfoccase.so    # どちらも True
```

`run-linux-site.ps1 -CaseInsensitive` も、これ(`experiments\wf4c\casefs\out`)を使います。

### 手順 12. コーパスを取る

コーパスはこのリポジトリには入っていません。それぞれ**元の GitHub のリポジトリから、決まったタグかコミットを取ります**(このリポジトリの記録・ベースライン・正解は、その版で作ったものです)。
リポジトリ、タグ・コミット、ライセンスは [一覧](README.md#コーパスの一覧) と各ページの「3.1 取得する」にあります。

取り方は 3 通りで、どれも同じものになります(各ページの「3.1 取得する」に、そのコーパスのコマンドと URL があります)。

- **A. GitHub の画面から(ZIP)**: そのタグ・コミットのページで「Code」→「Download ZIP」を押し、展開します。
- **B. git で**: `git clone` して、そのコミットを `git checkout` します。改行を ZIP と同じにするため、`-c core.autocrlf=false -c core.eol=lf` を付けます(mvcmovie・contoso は `fetch.ps1` と同じ `-c core.autocrlf=true`)。
- **近道: `fetch.ps1`**: A(mvcmovie・contoso は B)と、下の手直しをまとめて行い、`corpora\work\` に置きます。

```powershell
.\corpora\fetch.ps1 -Only be,wt          # 試すものだけ
.\corpora\fetch.ps1                      # 全部(12 個。imis の DB の imisdb を含む)
```

- 取得先は `corpora\work\`(Git には入りません)。取得済みのものは飛ばします。取り直すときは `-Force`。
- 名前: `be`、`wt`、`mojo`、`yaf`、`dnn`、`n2`、`imis`、`imisdb`、`nop`、`mvcmovie`、`nop390`、`contoso`。コーパスごとに、取得元(リポジトリの URL とタグ・コミット)を表示します。
- wt・n2・imis はタグが無いので、ブランチではなくコミットの ZIP を取り、フォルダーの名前を元の名前(`wingtiptoys-master`・`n2cms-master`・`web_app_vb-main`)に変えます。
- wt・mvcmovie・contoso は、取得のときに Web.config の接続文字列を SQL Server Express(`.\SQLEXPRESS`)に向けます(元の wt は存在しない Azure SQL を、mvcmovie・contoso は LocalDB を指しているため。正解を記録したときと同じ値です)。
  「接続文字列を SQL Server Express(.\SQLEXPRESS)に向けました」と出ます。手で取ったときは、各ページの「手で直すところ」のとおりに書き換えます。

**置く場所**: Studio や `converter.ps1` で変換するだけなら、どこに置いても構いません(各ページの「Studio・変換器に渡すフォルダー」)。
ただし、**比較のスクリプトを使う場合は `corpora\work\<フォルダーの名前>` に置きます**。`convert-corpora.ps1`、`corpora\build-original.ps1`、`corpora\record-*.ps1`、`corpora\nop-database.ps1` は `corpora\work` の下のパスを前提にしていて、`verify-windows.ps1`・`run-linux.ps1` は `convert-corpora.ps1` の出力を使います。
パスは短めにします。元のビルドのあとにできる深いファイルで、.NET Framework の MSBuild の上限(Windows の MAX_PATH、260 文字)を超えることがあるためです(各ページに、フォルダー自体のパスの目安があります)。

確認: 最後に「すべて取得できました。」と出ます。`Get-ChildItem corpora\work` に取ったもののフォルダーがあります。

### 手順 13. FrameworkOnCore のパッケージ(自動)

変換後のアプリが使う FrameworkOnCore のパッケージ(`FrameworkOnCore.*`)は、初めて解析や変換をしたときに GitHub Release から `experiments\wf4c\_feed` に **自動で** 取ってきます。
Studio と変換器(`studio.ps1`・`converter.ps1`)も、GitHub Actions がビルドしたものを自動で取ってきます(`%LOCALAPPDATA%\FrameworkOnCore\builds`)。準備は要りません。

確認(初めての変換の後で):

```powershell
Get-ChildItem experiments\wf4c\_feed\FrameworkOnCore.*.nupkg | Select-Object -First 3    # FrameworkOnCore.<名前>.1.6.5-w2l.27.nupkg など
```

今のパッケージの版は `1.6.5-w2l.27` です(`src\FrameworkOnCore.Converter\rules\packages.json` の `frameworkOnCoreVersion`)。

---

## 3. ディスクの空き

このマシン(2026-11-04)での実測です。変換の出力には、元のリポジトリの写しとビルドの結果が入ります。
「元のビルド」は、元のアプリをそのビルド手順でビルドした写し(`experiments\wf4c\<名前>.original-<日時>`)です。

| コーパス | 取得したもの(`corpora\work`) | 変換の出力(`experiments\wf4c\<名前>`) | 元のビルド |
|---|---|---|---|
| be | 94 MB | 0.6 GB | ― |
| wt | 348 MB | 0.6 GB | ― |
| mojo | 161 MB | 4.1 GB | 0.7 GB |
| yaf | 476 MB | 0.8 GB | ― |
| dnn | 165 MB | 15 GB(何度か動かした残りを含む) | 5.0 GB |
| n2 | 66 MB | 5.1 GB | 1.2 GB |
| imis | 115 MB(DB のスクリプト 5 MB は別) | 1.5 GB | 0.6 GB |
| nop | 46 MB | 1.5 GB | 0.4 GB |
| mvcmovie | 97 MB | 0.6 GB | ― |
| nop390 | 334 MB | 2.0 GB | 0.8 GB |
| contoso | 143 MB | 0.6 GB | ― |

Linux で動かすときは、さらに Docker のイメージ(変換したアプリ 1 つで約 1.1 GB。be 1.15 GB、wt 1.13 GB)と、.NET SDK・ASP.NET・SQL Server のイメージで数 GB 使います。
Docker のビルドのキャッシュは使うたびに増えます([10.6 ディスクがいっぱい](#106-ディスクがいっぱいdocker))。

---

## 4. 変換のしかた(2 通り)

| | Studio(画面) | コマンドライン |
|---|---|---|
| 向いているとき | 初めて試す、自分のアプリを試す、何が変わるか見たい | コーパスをまとめて変換する、何度も変換する |
| 出力の場所 | `%LOCALAPPDATA%\FrameworkOnCore\studio\<解析の ID>\conversion\out` | `experiments\wf4c\<名前>`(`convert-corpora.ps1`)、または `--out` で指定 |

コーパスごとの「プロジェクト」「リポジトリのフォルダー」「構成」などの値は、各コーパスのページの「手順」にあります。

### 4.1 Studio(画面)

```powershell
.\studio.ps1
```

確認: 「Studio: http://127.0.0.1:5300/」と出ます。ブラウザーで **http://127.0.0.1:5300/** を開きます(この PC からだけ開けます)。
止めるときは、`studio.ps1` を動かしている PowerShell で Ctrl+C。Studio が起動したアプリも一緒に止まります。

左に解析の一覧、まん中に 5 つの手順(ステップ)が並びます。どの手順にも戻れます。

1. **新しい解析**(右上の「+ 新しい解析」)
   - 「プロジェクト(.csproj / .vbproj)」: Web プロジェクトのファイル。例(be): `C:\src\FrameworkOnCore\corpora\work\BlogEngine.NET-3.3.8.0\BlogEngine\BlogEngine.NET\BlogEngine.NET.csproj`
   - 「リポジトリのフォルダー」: 空なら「.sln のある一番上のフォルダー」になります。プロジェクトが `.sln` のフォルダーの外の DLL を参照しているときは、それを含むフォルダーを指定します(be がそうです。[be のページ](be.md))。
   - 「構成」: ふつうは `Release`。imis は `DemoRelease`。
   - 「解析する」を押します。
2. **ステップ 1 読み込み**: プロジェクトの構成(言語、.NET Framework の版、NuGet パッケージの数、ページの数など)と、「リポジトリのビルド手順」(元のビルドが要るかの自動判定)が出ます。「元のアプリを起動(テスト起動)」で、変換の前のアプリをこの PC で動かすこともできます(IIS Express、無ければ IIS)。
3. **ステップ 2 解析と対応**: 解析が終わると(be で 10 秒ほど)、「概要」に .NET Framework の API の数、「変換で対応する API」「自分で対応が要る部品」「対応を選ぶ部品」の数が出ます。下のタブ「自分で対応する所」「対応を選ぶ部品」「すべての API」で中身を見て、部品ごとに対応を選べます。**既定のままで変換できます。**
4. **ステップ 3 サーバーの設定**: ファイル名の大文字・小文字(Linux でも区別しない〈既定〉/ 区別する)、アクセス ログ、URL Rewrite、エラー ページ、圧縮などの IIS の機能の動かし方です。既定(「IIS 互換」)は元の IIS と同じ動きです。
5. **ステップ 4 変換・ビルド**: 「変換してビルド」を押します。元のビルドが要るアプリ(DNN など)は「元のアプリをそのビルド手順でビルドし、配置されるサイトから組み立てる」にチェックが入ります(自動判定。理由も出ます)。「追加の手順」は n2 のように、元のビルドの後にリポジトリのセットアップが要るときに書きます。
   終わると「ビルド成功」と、下に「変換レポート」(節ごとに開ける)が出ます。be で 1 分ほどです。
6. **ステップ 5 実行・デプロイ**:
   - 「ZIP ダウンロード」: 変換の出力(サイト、Dockerfile、systemd 用のスクリプト)。配置の手順は ZIP の中の `deploy/README.md`。
   - 「ネイティブ起動(この PC)」: 「起動」で、この PC の .NET 10 で動かします(Docker は要りません。be ではポート 8100 で起動しました)。
   - 「コンテナ起動(Linux / Docker)」: Dockerfile からイメージを作ってコンテナで動かします。Docker Desktop が止まっていると「Docker Desktop を起動」ボタンが出ます。コンテナは一度に一つです。
   - どちらも「環境変数」の欄で `SQLCONNSTR_<接続文字列の名前>=...`、`APPSETTING_<キー>=...` を渡せます(web.config を書き換えずに接続先を変える)。コンテナの中からこの PC の SQL Server へは `host.docker.internal` を使います(LocalDB や `localhost` は見えません。[手順 8](#手順-8-任意sql-server-express-を混合モードtcp-1433-にしテスト用のログインを作る))。
   - 「変換のコマンドをコピー」で、同じ変換をするコマンドラインが取れます。

### 4.2 コマンドライン: コーパスをまとめて変換する(`convert-corpora.ps1`)

```powershell
.\experiments\wf4c\convert-corpora.ps1 -Only be,wt
```

- `-Only` に名前を並べます(省略すると 11 本すべて)。
- コーパスごとの Web プロジェクト、リポジトリのフォルダー(`--root`)、元のビルドの要否、構成(`--configuration`)、元のビルドの後の手順(`--original-step`)、カルチャのデータ(`--culture-profile experiments\wf4c\_culture\culture-profile.json`。正解を記録したマシンのもの)は、スクリプトが知っています。
- 元のビルドは一度作ると次からは使い回します(数分かかるため)。作り直すときは `-Rebuild`。
- mojo・dnn・imis では、このマシンの SQL Server Express を使うように接続文字列も書きます(各コーパスのページ)。
- 変換器の記録: `experiments\wf4c\_linux\foc-<名前>.log`
- 変換器は、このリポジトリのソース(`src\FrameworkOnCore.Converter`)をその場でビルドして使います。

出力の例(be・wt、2026-11-04 に実行。1 分 36 秒):

```
=== be BUILD OK (rounds: 4) 未解決(変換で解決できなかったもの)(0 件) / パッケージの大きな版上げ(...)(2 件) / IIS の機能(...)(3 件) / .NET に無い、または Windows 専用の API(37 件) / コンパイルのために変えたソース(スタブ・削除)(23 件) / Linux で動かすために変えたソース(Windows のパス)(198 件) / プロジェクトとパッケージ(16 件)
  snapshot: the same as the baseline (936 entries)
=== wt BUILD OK (rounds: 2) 未解決(...)(0 件) / ... / コンパイルのために変えたソース(スタブ・削除)(0 件) / ...
  snapshot: the same as the baseline (299 entries)
```

確認:

- `BUILD OK` なら変換とビルドが通っています。
- `snapshot: the same as the baseline` は、変換の判断が記録(`corpora\regression\conversion-baseline\<名前>.json`)と同じという意味です。違いがあると最後にスクリプトが失敗し、`experiments\wf4c\_linux\snapshot-diff-<名前>.txt` に違いが出ます。
  **be・wt・nop 以外の記録は前の版(パッケージ `1.6.5-w2l.25`)のとき** のものなので、版の文字列の違いで失敗扱いになることがあります。そのときも `BUILD OK` なら変換の結果は使えます([10.8](#108-変換がdiffer-from-their-snapshotで失敗する))。
- `REGRESSION`(未解決やスタブが記録より増えた)と出たら、変換器かコーパスに変化があります。`CONVERSION-REPORT.md` の「未解決」を見てください。

### 4.3 コマンドライン: 変換器を直接使う(`converter.ps1`)

自分のアプリや、1 本だけを変換するときに使います。

```powershell
# 元のビルドが要るか(理由も出る)
.\converter.ps1 build-original-advice <Web プロジェクト>

# 解析だけ(api-analysis.json、API-ANALYSIS.md、既定の選択の foc-choices.json)
.\converter.ps1 analyze <Web プロジェクト> --out <出力先>

# 変換(--choices は Studio や analyze が書いた選択。省略すると既定)
.\converter.ps1 <Web プロジェクト> --out <出力先> [--root <リポジトリのフォルダー>] [--build-original auto] [--configuration <構成>] [--original-step <手順>] [--choices foc-choices.json] [--culture-profile <file>] [--iis-profile <file>]
```

確認: 最後に `build succeeded; web project: ...; report: <出力先>\CONVERSION-REPORT.md` と出れば成功です。

- `--culture-profile` を付けないと、この PC のカルチャのデータを採って使います(元のサーバーのものがあればそちらが正確です。[11. 自分のアプリで試すとき](#11-自分のアプリで試すとき))。
- `convert-corpora.ps1` が変換の後にする DB の準備(mojo・dnn・imis の接続文字列など)は、`converter.ps1` ではしません。コーパスは `convert-corpora.ps1` を使うほうが簡単です。
- 実際の例は [be のページ](be.md#3-手順) にあります(2026-11-04 に実行)。

---

## 5. 変換レポートの読み方

変換レポート(`CONVERSION-REPORT.md`)は、出力のフォルダーの一番上にあります(Studio ではステップ 4 の「変換レポート」)。見出しの後ろの「(n 件)」が件数です。上から順に大事です。

| 節 | 意味 | どうするか |
|---|---|---|
| 未解決(変換で解決できなかったもの) | 変換で直せなかった所。とくに「stubbed because of the conversion, not of .NET」は、変換が参照を失った(パスが外れた、など)ことを示します | **0 件が目標**。1 件でもあれば、まず理由を読みます(be で「リポジトリのフォルダー」を指定し忘れたときはここに 16 件出ます) |
| パッケージの大きな版上げ | .NET 10 で動く版に上げたパッケージ(例: Newtonsoft.Json 6.0.8 → 13.0.4)と、動きの違い | 書かれた違いがアプリに関係するか確かめます |
| IIS の機能 | 元のサーバーと web.config が使う IIS の機能と、変換後に「対応」しているか | 「未対応」があれば、その機能は変換後に効きません |
| .NET に無い、または Windows 専用の API | .NET 10 に無い・Windows でしか動かない API と、何で補ったか。`SYSLIB....` は .NET で廃止予定・実行時に例外になるもの | その機能を使う画面を実際に開いて確かめます |
| コンパイルのために変えたソース(スタブ・削除) | ビルドを通すために、本体を「例外を投げる」形にしたメンバーや、外した using など | **その機能は変換後に動きません**。使う機能なら、自分で書き換えます |
| Linux で動かすために変えたソース(Windows のパス) | `\` 区切りのパスなどを、Linux でも動く形に書き換えた所 | ふつうは何もしません |
| プロジェクトとパッケージ | プロジェクトの形式、パッケージの置き換え、DLL の参照の付け替え、配置(Dockerfile など)について変換器が決めたこと | 気になる所だけ読みます |

各行の先頭の `[Unsupported]`、`[Error]`、`[Raised]`、`[Project]` は、変換器の記録(ログ)の中の種類です。

---

## 6. Windows で動かす

### 6.1 起動する(`supervise.ps1`)

変換したアプリは、`bin\<Web プロジェクトのアセンブリ>.dll` を `dotnet` で起動します。**サイトのフォルダーで** 起動してください。

サイトのフォルダー:

- 元のビルドから組み立てたコーパス(mojo、dnn、n2、imis、nop、nop390): `experiments\wf4c\<名前>\site`
- それ以外: 出力の中の Web プロジェクトのフォルダー(be は `experiments\wf4c\be\BlogEngine\BlogEngine.NET`)

アプリは web.config や bin が変わると「終了コード 75」で終わり、作り直されることを期待します(IIS のアプリの再起動に当たる)。
そのため、`supervise.ps1`(終了コード 75 なら起動し直す)を通して起動します。

```powershell
cd <サイトのフォルダー>
<リポジトリ>\experiments\wf4c\supervise.ps1 -Dll bin\<アセンブリ>.dll -Port 5096 [-Log <ログのファイル>]
# ログは既定でこのフォルダーの probe.log
```

コーパスごとの正確なコマンドは、各コーパスのページにあります。

確認: 別の PowerShell かブラウザーで **http://localhost:5096/** を開きます。

- 最初の要求は、ページのコンパイルで 10 秒ほどかかります。
- 最初の要求のあとアプリが 1 回再起動することがあります(ログに `application restart (ConfigurationChange)`)。その間の要求は待たされるか、応答がありません。もう一度開けば答えます。

### 6.2 止める

`supervise.ps1` の PowerShell で Ctrl+C のあと、ポートを持っているプロセスも止めます(再起動した作業プロセスが残るため)。

```powershell
Get-NetTCPConnection -LocalPort 5096 -State Listen -ErrorAction SilentlyContinue | ForEach-Object { Stop-Process -Id $_.OwningProcess -Force }
```

確認: `Get-NetTCPConnection -LocalPort 5096 -State Listen -ErrorAction SilentlyContinue` が何も出さないこと。

### 6.3 元のアプリと比べる(`verify-windows.ps1`)

正解(元のアプリを IIS で動かして記録した画面)のあるコーパス(be、wt、mvcmovie、contoso、nop)は、変換したアプリを起動して、画面を正解と比べられます。起動から停止までスクリプトが行います(ポート 5096)。

```powershell
.\experiments\wf4c\verify-windows.ps1 -Name <be | wt | mvcmovie | contoso | nop> [-Suite operations]
```

- `-Suite operations` は「すべての操作」の比較です(be・wt・nop)。
- Edge が要ります([手順 9](#手順-9-edge-を確かめる比較をするときだけ))。

確認: 最後に `RESULT: OK (5 スナップショットすべて変換前と一致、リンク 17 件も一致)` のように出れば一致です。`RESULT: FAIL` なら、違った画面の名前と違いが出ます。

---

## 7. Linux で動かす

Linux では Docker のコンテナで動かします。やり方は 3 つあります。

| やり方 | 何をする | 向いているとき |
|---|---|---|
| A. 変換が書いた `Dockerfile` | 配置と同じイメージを作って動かす | 本番の配置を試す |
| B. `run-linux.ps1` | 出力をコンテナの中でビルドして動かし、正解と比べる(be、wt、mvcmovie、contoso、nop) | 元と同じに動くかを確かめる |
| C. `run-linux-site.ps1` | 組み立てたサイト(`<名前>\site`)をそのままコンテナで動かし、指定したページを開く。SQL Server もコンテナで | DB やインストーラーの要るコーパス(mojo、dnn、n2、imis) |

始める前に Docker Desktop を起動しておきます(確認: `docker version` に Server が出る)。終わったら [9. 後片付け](#9-後片付け) をしてください。

### 7.1 A. 変換が書いた Dockerfile

出力のフォルダー(`Dockerfile` のある所)で:

```powershell
cd experiments\wf4c\<名前>
docker build -t <イメージ名> .
docker run -d -p 8080:8080 --name <コンテナ名> <イメージ名>
```

- **http://localhost:8080/** を開きます。
- 確認: `docker logs <コンテナ名>` に `Now listening on: http://[::]:8080` が出ていれば起動しています。
- Linux でもファイル名の大文字・小文字を区別しません(既定。[手順 11](#手順-11-casefs-をビルドするlinux-で動かすときだけ) の casefs)。
- カルチャ(例: `LANG=ja_JP.UTF-8`)とタイムゾーン(例: `TZ=Asia/Tokyo`)は、カルチャのデータ(元のサーバーのもの)から Dockerfile に書かれています。
- DB を使うアプリは、接続文字列を環境変数 `SQLCONNSTR_<接続文字列の名前>`、アプリの設定は `APPSETTING_<キー>` で渡します。
- 詳しい説明(環境変数、systemd での配置)は出力の `deploy\README.md` にあります。
- 実際の例: [be](be.md)(DB 無し)、[wt](wt.md)(SQL Server もコンテナで動かす)。

### 7.2 B. `run-linux.ps1`(正解と比べる)

```powershell
.\experiments\wf4c\run-linux.ps1 -App <Web プロジェクトのフォルダー> [-Scenario <シナリオ>] [-Golden <正解>] [-SqlServer] [-Suite operations] [-Keep] [-CaseInsensitive]
```

- `-App` は `experiments\wf4c` から見た Web プロジェクトのフォルダーです(例: `be\BlogEngine\BlogEngine.NET`)。
- `-SqlServer`: SQL Server もコンテナ(`w2l-sql`、次の実行のために残す)で動かし、web.config の `.\SQLEXPRESS` などをそこへ向けます。
- 終わるとアプリのコンテナは消えます(`-Keep` で残す)。ポートは 5095。
- 確認: 結果は `RESULT: OK ...` / `RESULT: FAIL`。失敗すれば 0 以外の終了コードで終わります。

コーパスごとの正確なコマンドは、各コーパスのページにあります。

### 7.3 C. `run-linux-site.ps1`(サイトをそのまま動かす)

```powershell
.\experiments\wf4c\run-linux-site.ps1 -Site <名前>\site -Dll <アセンブリ名> [-Database <DB 名>] [-SqlScripts <ファイル>, ...] [-Paths '/', ...] [-CaseInsensitive] [-Keep]
```

- ASP.NET のイメージにサイトを写して起動し、`-Paths` を順に開いて状態とタイトルを出します(ポート 5098)。
- `-Database` を付けると SQL Server のコンテナ(`w2l-sql`)にその DB を作り直し、サイトの `.config` の接続文字列をそこへ向けます(サイトのフォルダーは書き換えません)。`-SqlScripts` は、作り直した DB に流す SQL のファイルです(imis で使います)。
- `-CaseInsensitive`: Linux でもファイル名の大文字・小文字を区別しない(配置の既定と同じ)。`experiments\wf4c\casefs\out` が要ります([手順 11](#手順-11-casefs-をビルドするlinux-で動かすときだけ))。
- `-Keep`: 終わってもコンテナを残す(ブラウザーで http://localhost:5098/ を開いて触れる)。消すときは `docker rm -f w2l-site-<名前>`。
- コンテナのログ: `experiments\wf4c\_linux\<名前>-site\container.log`
- 確認: 開いた各ページの状態コード(200 など)とタイトルが出ます。

---

## 8. 動いているかの確かめ方

- **ページ**: トップと、主な画面(ログイン、一覧、詳細)を開きます。各コーパスのページに、開くページとログインの値があります。
- **ログ**: Windows はサイトのフォルダーの `probe.log`(`supervise.ps1` の `-Log` で変えられる)。コンテナは `docker logs <名前>`。
  ASP.NET のエラー ページ(黄色い画面)と同じ内容が、ブラウザーにも出ます(この PC から開いたとき。`customErrors` が `RemoteOnly` のアプリ)。
- **状態コードだけ見る**: `curl.exe -s -o NUL -w "%{http_code}" http://localhost:5096/`

---

## 9. 後片付け

メモリーとディスクのために、使い終わったら止めます。

```powershell
# Windows で起動したアプリ(6.2)
Get-NetTCPConnection -LocalPort 5096 -State Listen -ErrorAction SilentlyContinue | ForEach-Object { Stop-Process -Id $_.OwningProcess -Force }
# 自分で起動したコンテナ(名前は docker ps -a で確かめる)
docker rm -f <コンテナ名>
docker stop w2l-sql                      # run-linux.ps1 -SqlServer / run-linux-site.ps1 が残す SQL Server
# Docker Desktop を止め、WSL のメモリーを返す
Stop-Process -Name 'Docker Desktop','com.docker.backend' -Force -ErrorAction SilentlyContinue
wsl --shutdown
# SQL Server Express(管理者の PowerShell)
Stop-Service 'MSSQL$SQLEXPRESS'
```

ブラウザー(Edge)も閉じます。コーパスごとに消すもの(コンテナ、ネットワーク、DB)は、各コーパスのページの「片付け」にあります。

確認:

```powershell
docker ps -a                                   # (Docker を止める前に)自分のコンテナが残っていないこと
wsl --list --running                           # 実行中のディストリビューションが無いこと
(Get-Service 'MSSQL$SQLEXPRESS').Status        # Stopped
```

---

## 10. うまくいかないとき

### 10.1 スクリプトが実行できない

「このシステムではスクリプトの実行が無効になっている」と出るときは [手順 4](#手順-4-powershell-のスクリプトを動かせるようにする) をします。

### 10.2 元のビルドが失敗する

- 「Visual Studio (Build Tools) 2022's MSBuild is not installed」: [手順 6](#手順-6-visual-studio-build-tools-を入れる元のビルドが要るコーパスだけ) の `winget` のコマンドで入れます。
- それ以外: 変換器の記録 `experiments\wf4c\_linux\foc-<名前>.log` の `[Error]` の行と、元のビルドの記録(`experiments\wf4c\<名前>.original-<日時>.build.log`、または `<名前>.original.build.log`)を見ます。
- 「the build failed after it deployed the site: ... is used as it is」は、ビルドの一部が失敗したが、サイトはできていたのでそれを使った、という意味です(dnn のパッケージ作り、be のソリューションなど)。そのまま進んで構いません。
- 元のビルドを作り直すときは `convert-corpora.ps1 -Only <名前> -Rebuild`。
- パスが長すぎる: リポジトリを短い場所に置きます([手順 5](#手順-5-リポジトリを取る))。

### 10.3 SQL Server につながらない

- サービスが止まっている: `Get-Service 'MSSQL$SQLEXPRESS'`、管理者で `Start-Service 'MSSQL$SQLEXPRESS'`。
- インスタンス名が違う: スクリプトは `.\SQLEXPRESS` を前提にしています。別の名前なら、SQL Server Express を `SQLEXPRESS` という名前で入れるのが早道です。
- `sqlcmd` が無い: convert-corpora(dnn・imis)と verify-windows(wt・mvcmovie・contoso)が使います。[手順 7](#手順-7-sql-server-express-と-sqlcmd-を入れる)。
- アプリのログに「Login failed for user」: Windows 認証で、アプリを動かしている自分のユーザーに DB を作る権限が要ります(SQL Server Express を入れたユーザーなら持っています)。
- コンテナの中から: `.\SQLEXPRESS` や LocalDB は見えません。`run-linux.ps1 -SqlServer`・`run-linux-site.ps1` は SQL Server のコンテナを使います。この PC の SQL Server を使うなら `host.docker.internal` と SQL Server の認証(混合モード、TCP 1433。[手順 8](#手順-8-任意sql-server-express-を混合モードtcp-1433-にしテスト用のログインを作る))。

### 10.4 ポートが使われている

`Failed to bind to address http://localhost:5096: address already in use` などと出るときは、前に動かしたアプリが残っています(再起動した作業プロセスが残ることがあります)。

```powershell
Get-NetTCPConnection -LocalPort 5096 -State Listen -ErrorAction SilentlyContinue | ForEach-Object { Stop-Process -Id $_.OwningProcess -Force }
```

使うポート: Studio 5300、Studio のネイティブ起動 8100 から、`verify-windows.ps1`・`supervise.ps1`(既定)・`dnn-install.ps1`・`n2-install.ps1` 5096、`imis-login.ps1` 5097、`run-linux.ps1` 5095、`run-linux-site.ps1` 5098、Dockerfile の例 8080。

### 10.5 Docker が遅い・止まる(メモリー)

- このリポジトリの開発マシンは 16 GB で、WSL 2 に 8 GB を割り当てています([手順 10](#手順-10-docker-desktop-を入れるlinux-で動かすときだけ) の `.wslconfig`)。少ないマシンでは、SQL Server のコンテナ(2 GB ほど使う)とアプリのコンテナで足りなくなることがあります。
- **コンテナは一度に一つ(DB の要るものは SQL Server とアプリの 2 つ)にし、終わったら止めます**([9. 後片付け](#9-後片付け))。使っていない SQL Server Express と Edge も閉じます。
- 終わったら Docker Desktop を止めて `wsl --shutdown`(WSL のメモリーが返ります)。

### 10.6 ディスクがいっぱい(Docker)

Linux の実行を重ねると、Docker のビルドのキャッシュとイメージが増えます(記録では 45 GB になり、C ドライブがいっぱいになりました)。

```powershell
docker system df                 # 何がどれだけ使っているか
docker builder prune -af         # ビルドのキャッシュを消す
docker image prune -a            # 使っていないイメージを消す
```

消しても `docker_data.vhdx`(WSL のディスク)は小さくなりません。Docker Desktop を止めて `wsl --shutdown` したあと、管理者で `diskpart` を使い、`select vdisk file="<docker_data.vhdx のパス>"`、`compact vdisk` で縮めます。

### 10.7 比較(verify-windows・run-linux)が失敗する

- Edge が無い・起動できない: 比較の道具(ParityTest)は Playwright で Edge を画面無しで動かします。Edge を入れ直すか更新します。終わったあと Edge のプロセスが残っていたら閉じます。
- Edge が更新されて、User-Agent の版が画面に出る所が違う: wt のすべての操作の正解には Edge 156 の User-Agent が入っています。版が変わると差が出ることがあります。
- `RESULT: FAIL` で、違いが文言や時刻だけ: 違った画面の差が出力に出ます。許される差の一覧(`corpora\parity\*.allowed.json`)にない違いです。
- be の結果がおかしい: [be のページ](be.md#6-うまくいかないとき)。

### 10.8 変換が「differ from their snapshot」で失敗する

[4.2](#42-コマンドライン-コーパスをまとめて変換するconvert-corporaps1) のとおり、be・wt・nop 以外は記録が前の版のものなので、版の違いで失敗扱いになります。`experiments\wf4c\_linux\snapshot-diff-<名前>.txt` を見て、違いが版(`1.6.5-w2l.25` → `1.6.5-w2l.27`)やハッシュだけなら問題ありません。

### 10.9 アプリが 500 になる

- まずエラーの中身を見ます(ブラウザー、`probe.log`、`docker logs`)。
- 変換レポートの「未解決」と「コンパイルのために変えたソース(スタブ・削除)」に、その画面のコードがあれば、変換で動かなくなった所です(`PlatformNotSupportedException`)。
- yaf の `/` の 500 は既知です([yaf のページ](yaf.md))。

---

## 11. 自分のアプリで試すとき

流れはコーパスと同じです。初めては Studio([4.1](#41-studio画面))が分かりやすいです。

1. `.\studio.ps1` で Studio を開き、「+ 新しい解析」で自分の Web プロジェクト(.csproj / .vbproj)を指定します。
   - **「リポジトリのフォルダー」**: プロジェクトが参照する DLL やほかのプロジェクトが、`.sln` のフォルダーの外(リポジトリの `lib` など)にあるときは、それを含むフォルダーを指定します。指定が足りないと、変換レポートの「未解決」に「a reference or package of the original that the conversion lost」が出ます。
2. ステップ 1 の「元のアプリを起動(テスト起動)」で、まず元のアプリがこの PC で動くかを見ておくと、後で比べられます。
3. ステップ 2 で「自分で対応する所」を読みます。変換しても動かない部品(OLE DB、Active Directory、レジストリなど)と、その書き換え方が出ます。
4. ステップ 4 で変換し、変換レポート([5](#5-変換レポートの読み方))の「未解決」が 0 件か、「スタブ」に自分の使う機能が無いかを確かめます。
5. ステップ 5 で「ネイティブ起動」して画面を確かめ、Linux は「コンテナ起動」か ZIP の `Dockerfile`・`deploy/README.md` で。DB は環境変数 `SQLCONNSTR_<名前>` で渡します。

元のサーバー(本番の IIS)の設定を持ってくると、変換後の動きが元に近くなります。どちらも **元のサーバーで** 実行し、できたファイルを変換に渡します:

```powershell
# カルチャのデータ(数値・日付の書式など。Windows PowerShell で)
powershell -File experiments\wf4c\capture-culture.ps1 -Out culture-profile.json -WebConfig <アプリ>\web.config
# IIS のサイトの設定(管理者で)
powershell -File experiments\wf4c\capture-iis.ps1 -Site "Default Web Site" -Out iis-profile.config
# 変換に渡す
.\converter.ps1 <Web プロジェクト> --out <出力先> --culture-profile culture-profile.json --iis-profile iis-profile.config --build-original auto
```

---

## 12. 関連する資料

- [コーパスの一覧](README.md): どのコーパスから試すか、各コーパスのページ
- [README.md](../../README.md): 全体の流れ、Studio の手順、コマンドライン
- [tests/README.md](../../tests/README.md): テストと比較の一覧
- [corpora/README.md](../../corpora/README.md): コーパスの取得と正解の記録
- [experiments/wf4c/README.md](../../experiments/wf4c/README.md): 検証の記録(コーパスごとの経緯)
- 変換の出力の `deploy/README.md`: Linux への配置(コンテナ、systemd)
