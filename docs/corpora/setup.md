# 共通の準備

どのコーパスでも、流れは同じです。

1. GitHub からソースを取ります。
2. FrameworkOnCore で変換します。変換はコマンドライン(`converter.ps1`)でも、画面(Studio)でもできます。
3. 変換したアプリを Windows で動かします。
4. 変換が書いた Dockerfile で、Linux のコンテナとして動かします。

コマンドは Windows PowerShell 5.1 で実行します。

## 1. 入れるもの

| もの | 要るコーパス |
|---|---|
| Windows 10 / 11(x64) | すべて(変換は Windows で行います) |
| [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) | すべて(変換したアプリを Windows で動かすのにも使います) |
| [Git](https://git-scm.com/download/win) | すべて |
| [Docker Desktop](https://docs.docker.com/desktop/setup/install/windows-install/)(WSL 2) | Linux で動かすとき。SQL Server を使うコーパスでは、Windows で動かすときも使います |
| [Visual Studio](https://visualstudio.microsoft.com/downloads/)(「ASP.NET と Web 開発」)**または** [Visual Studio Build Tools](https://visualstudio.microsoft.com/downloads/)(「Web 開発ビルド ツール」と .NET Framework 4.8 の Targeting Pack) | mojo、dnn、n2、imis、nop、nop390(元のアプリのビルドが要るもの)。Studio を使うときは、すべて(ステップ 1 で元のアプリをビルドするため) |

SQL Server は、Docker のコンテナで動かします([5](#5-sql-server-を使うとき))。Windows に入れる必要はありません。

元のアプリのビルドには、Visual Studio か Build Tools のどちらかが要ります。ビルドの仕組み(MSBuild)は同じですが、一緒に入る部品が違います。

- どちらか一方だけが入っていれば、それを使います(版が複数あれば新しいほう)。
- **両方入っているときは、どちらを使うかを選びます。**
  - コマンドライン: `--msbuild visualstudio` か `--msbuild buildtools`(またはインストール先のフォルダー)を付けます。付けないと、選び方を表示して止まります。
  - Studio: ステップ 1 の「ビルドに使う MSBuild」で選びます(この欄はいつも出ます。入っていないほうは選べません)。ステップ 2 以降は、最後にビルドに成功したときの選択を使います。
- Cake でビルドするアプリ(dnn)は、そのビルドスクリプトが自分で MSBuild を探すので、この選択は効きません。

Visual Studio(Community など)とBuild Tools の使用条件は、それぞれのライセンスで確認してください。

WSL 2 のメモリーの上限は `%USERPROFILE%\.wslconfig` に書いておきます(16 GB の PC なら 8 GB)。

```ini
[wsl2]
memory=8GB
```

## 2. 作業フォルダーと FrameworkOnCore

各ページは「0. FrameworkOnCore を取る」から始まります。作業用の空のフォルダー(例: `C:\foc`)に FrameworkOnCore を `git clone` し、以降のコマンドは、すべてそのフォルダーから相対パスで実行します。

作業フォルダーの中は、次のようになります。

```
C:\foc
├─ FrameworkOnCore        FrameworkOnCore(変換器、Studio)
├─ <コーパスのソース>      各ページの「1. GitHub から取得する」で取るもの
└─ out
   └─ <名前>             変換の出力
```

- 作業フォルダーのパスは短くします。元のアプリのビルドに使う .NET Framework の MSBuild は、260 文字を超えるパスを扱えないためです。
- 変換器(`converter.ps1`)と Studio(`studio.ps1`)は、GitHub Actions がビルドしたものを、初めて使うときに自動で取ってきます。変換後のアプリが使う FrameworkOnCore のパッケージも同じく自動です。
- リポジトリが非公開のときは、先に `gh auth login` をしておきます。
- PowerShell のスクリプトを動かせないときは、`Set-ExecutionPolicy -Scope CurrentUser RemoteSigned` を一度だけ実行します。
- 「0」では casefs もビルドします。Linux でもファイル名の大文字・小文字を区別せずに動かすためのライブラリで、変換がこれを Dockerfile に入れるので、変換の前に済ませます。

## 3. ソースを取る

各ページの「1. GitHub から取得する」のコマンドで、元の GitHub のリポジトリから、決まったタグかコミットの版を作業フォルダーに取ります。

- ZIP は `curl.exe` でダウンロードし、`tar` で展開します。
- リポジトリの一部だけを使う mvcmovie・contoso は、`git` でそのフォルダーだけを取ります。
- ブラウザーで取るときは、取得元のリンクからそのタグかコミットを開き、「Code」→「Download ZIP」を押します(mvcmovie・contoso を除く)。

## 4. 変換する

### 4.1 コマンドラインで

```powershell
.\FrameworkOnCore\converter.ps1 <Web プロジェクト> --out .\out\<名前> --root <ソースのフォルダー> --build-original auto
```

- `--root`: ソースの一番上のフォルダーです。プロジェクトがソリューションの外を参照していても、ここまでは見つけます。
- `--build-original auto`: 元のアプリのビルドが要るかを変換器が判断し、要るときはビルドしてから変換します。
- 各ページの「変換する」に、そのコーパスでの値を入れたコマンドがあります。
- 確認: 最後に `build succeeded; web project: ...; report: ...\out\<名前>\CONVERSION-REPORT.md` と出れば成功です。
- 出力の中身:
  - `CONVERSION-REPORT.md`: 変換の結果です。何を変えたかと、自分で直すところが書かれています。
  - `Dockerfile`: Linux で動かすためのものです。
  - `deploy\README.md`: 配置のしかたです(環境変数、systemd)。

### 4.2 Studio で

```powershell
.\FrameworkOnCore\studio.ps1
```

作業フォルダーで起動します。Studio に入れるパスは、このフォルダーからの相対パスで構いません。

「Studio: http://127.0.0.1:5300/」と出たら、ブラウザーで http://127.0.0.1:5300/ を開きます。止めるときは、`studio.ps1` を動かしている PowerShell で Ctrl+C を押します。

1. 右上の「新しい解析」を押し、次の値を入れて「解析する」を押します。値は各ページの「Studio で入れる値」にあります。
   - 「プロジェクト(.csproj / .vbproj)」
   - 「リポジトリのフォルダー」
   - 「構成」
2. ステップ 1 の「元のアプリのビルド」で「ビルド」を押します。**ビルドが通るまで、ステップ 2 以降には進めません**(Studio を使うときは、どのコーパスでも Visual Studio か Build Tools が要ります)。
   - 「ビルドに使う MSBuild」で Visual Studio か Build Tools を選んでから押します(入っていないほうは選べません)。ステップ 2 以降は、最後にビルドに成功したときの選択を使います。
   - n2 は「追加の手順」を開いて入れます。ほかのコーパスは空のままです。
3. 解析が終わると、「自分で対応する所」や「対応を選ぶ部品」が出ます。**既定のままで変換できます。**
4. 「サーバーの設定」も既定のまま(元の IIS と同じ動き)で進みます。
5. 「変換してビルド」を押します。終わると「ビルド成功」と、変換レポートが出ます。
   - 変換では毎回、元のアプリをステップ 1 と同じ手順(MSBuild、追加の手順)で、まっさらな状態からビルドし直します(`--build-original`)。
6. 最後のステップで、変換したアプリを動かします。
   - 「ネイティブ起動(この PC)」: Windows で動かします([6](#6-windows-で動かす))。
   - 「コンテナ起動(Linux / Docker)」: Linux のコンテナで動かします([7](#7-linux-で動かす))。
   - どちらも「環境変数」の欄に、1 行に 1 つ `名前=値` を書いて、接続文字列などを渡せます。
   - 「ZIP ダウンロード」で、変換の出力を取り出せます。

Studio の出力は `%LOCALAPPDATA%\FrameworkOnCore\studio\` に置かれます(作業フォルダーの `out` ではありません)。

## 5. SQL Server を使うとき

SQL Server をコンテナで動かし、この PC のポート 14330 で公開します。Windows で動かすアプリも、Linux のコンテナで動かすアプリも、ここにつなぎます。

```powershell
docker run -d --name foc-sql -p 14330:1433 -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=Foc_local_Passw0rd" mcr.microsoft.com/mssql/server:2022-latest
```

- 2 回目以降(PC の再起動のあとなど)は、`docker start foc-sql` で起動します。同じ名前のコンテナがあるので、`docker run` は「Conflict」で止まります。
- 動いているかは `docker ps -a --filter name=foc-sql` で確かめます(`docker ps` は止まっているコンテナを出しません)。STATUS が `Exited` なら止まっています。

### ARM の PC(Windows on ARM)では

SQL Server 2022 のイメージは x64 版しかなく、ARM の PC ではエミュレーションで起動してもすぐ止まります(`docker run` で「The requested image's platform (linux/amd64) does not match ... (linux/arm64/v8)」と出ます)。代わりに、arm64 版のある Azure SQL Edge(SQL Server と同じエンジン)を使います。名前・ポート・パスワードは同じなので、接続文字列はそのまま使えます。

```powershell
docker run -d --name foc-sql -p 14330:1433 -e ACCEPT_EULA=1 -e "MSSQL_SA_PASSWORD=Foc_local_Passw0rd" mcr.microsoft.com/azure-sql-edge
```

- Azure SQL Edge は 2025 年 9 月でサポートが終わっています(イメージは取れますが、更新されません)。この PC での動作確認に使います。
- Azure SQL Edge のコンテナには `sqlcmd` が入っていません。DB をスクリプトで作るページ(dnn、mojo、imis)の `docker exec foc-sql /opt/mssql-tools18/bin/sqlcmd ...` は動かないので、Windows の `sqlcmd` を入れて(`winget install sqlcmd`)、各ページの「ARM の PC では」の手順を使います。

接続文字列のサーバー名は、アプリをどこで動かすかで変わります。

| アプリを動かす場所 | サーバー名(`Data Source`) |
|---|---|
| Windows(コマンドライン、Studio の「ネイティブ起動」) | `localhost,14330` |
| Linux のコンテナ(`docker run`、Studio の「コンテナ起動」) | `host.docker.internal,14330` |

- ユーザーは `sa`、パスワードは `Foc_local_Passw0rd` です。接続文字列には `TrustServerCertificate=True` を付けます。
- SQL Server は、起動してから接続できるまでに 10〜20 秒かかります。

## 6. Windows で動かす

変換の出力のサイトのフォルダーで、Web アプリの DLL を `dotnet` で起動します。サイトのフォルダーと DLL は、各ページの「Windows で動かす」にあります。

```powershell
Push-Location .\out\<名前>\<サイトのフォルダー>
dotnet bin\<Web アプリ>.dll --urls http://localhost:5000
Pop-Location         # Ctrl+C で止めたあと、作業フォルダーに戻る
```

- http://localhost:5000/ を開きます。止めるときは Ctrl+C を押します。
- 接続文字列などは、起動の前に環境変数で渡します(web.config は書き換えません)。
  - `SQLCONNSTR_<接続文字列の名前>`: 接続文字列を置き換えます。
  - `APPSETTING_<キー>`: appSettings の値を設定します。

  ```powershell
  $env:SQLCONNSTR_DefaultConnection = 'Data Source=localhost,14330;Initial Catalog=...;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True'
  ```

- web.config や bin が変わると、アプリは自分で再起動します(IIS がアプリを再起動するのと同じです)。
- Studio では「ネイティブ起動(この PC)」の「起動」を押します。開く URL は画面に出ます(ポートは 8100 から空いているもの)。

## 7. Linux で動かす

変換の出力フォルダー(`Dockerfile` のあるところ)からイメージを作り、コンテナを起動します。

```powershell
docker build -t <名前> .\out\<名前>
docker run -d -p 8080:8080 --name <名前> <名前>
```

- http://localhost:8080/ を開きます。
- 接続文字列などは、`docker run` の `-e SQLCONNSTR_<名前>=...` と `-e APPSETTING_<キー>=...` で渡します。
- Studio では「コンテナ起動(Linux / Docker)」の「起動」を押します。開く URL は画面に出ます。Studio のコンテナは一度に一つです。

インストーラーのあるアプリ(mojo、dnn、n2、nop、nop390)は、インストールでサイトのファイル(設定ファイルや `App_Data`)が書き換わることがあります。
Windows と Linux の両方で試すときは、Windows で動かす前に `docker build` を済ませておきます(イメージは、作ったときのサイトのファイルから作られるためです)。

## 8. 片付け

```powershell
docker rm -f <アプリのコンテナ> foc-sql
Stop-Process -Name 'Docker Desktop' -ErrorAction SilentlyContinue
wsl --shutdown                                     # WSL のメモリーを返す
```

Windows で動かしたアプリは Ctrl+C で止めます。Studio は、止めるとそれが起動したアプリも止まります。
コンテナは一度に一つ(SQL Server を使うときは、SQL Server とアプリの 2 つ)にします。

## 9. うまくいかないとき

| 症状 | 対処 |
|---|---|
| 元のアプリのビルドが失敗する | Visual Studio か Build Tools が入っていないか、必要な部品(上の表)が足りないか、パスが長すぎます。作業フォルダーを短いパスにします |
| ポート(5000、8080、14330)が使われている | 前のアプリやコンテナを止めます。または別のポートを使います(`--urls http://localhost:5001`、`-p 8081:8080`) |
| Docker が遅い・止まる | ほかのコンテナを止めます。`.wslconfig` の memory を見直します |
| ディスクがいっぱい | 不要なイメージを消します(`docker image prune -a`、`docker builder prune`) |
| アプリが 500 になる・起動しない | Windows では PowerShell に出るログ、コンテナでは `docker logs <コンテナ>`、Studio では画面のログを見ます。多くは接続文字列の誤りです |
