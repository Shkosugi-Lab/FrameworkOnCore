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
| [Visual Studio Build Tools](https://visualstudio.microsoft.com/downloads/)(「Web 開発ビルド ツール」と .NET Framework 4.8 の Targeting Pack) | mojo、dnn、n2、imis、nop、nop390(元のアプリのビルドが要るもの) |

SQL Server は、Docker のコンテナで動かします([5](#5-sql-server-を使うとき))。Windows に入れる必要はありません。

WSL 2 のメモリーの上限は `%USERPROFILE%\.wslconfig` に書いておきます(16 GB の PC なら 8 GB)。

```ini
[wsl2]
memory=8GB
```

## 2. FrameworkOnCore を取る

```powershell
git clone https://github.com/Shkosugi-Lab/FrameworkOnCore.git C:\src\FrameworkOnCore
Set-ExecutionPolicy -Scope CurrentUser RemoteSigned     # スクリプトを動かせるようにする(一度だけ)
```

- 変換器(`converter.ps1`)と Studio(`studio.ps1`)は、GitHub Actions がビルドしたものを、初めて使うときに自動で取ってきます。変換後のアプリが使う FrameworkOnCore のパッケージも同じく自動です。
- リポジトリが非公開のときは、先に `gh auth login` をしておきます。

Linux でもファイル名の大文字・小文字を区別せずに動かすためのライブラリ(casefs)を、一度だけビルドします。変換がこれを Dockerfile に入れるので、変換の前に済ませます。Docker Desktop を起動してから実行します。

```powershell
C:\src\FrameworkOnCore\experiments\wf4c\casefs\build.ps1
```

## 3. ソースを取る

ソースと変換の出力は `C:\foc` に置きます。各ページのコマンドは、このフォルダーで実行します。

```powershell
New-Item -ItemType Directory -Force C:\foc | Out-Null
cd C:\foc
```

各ページの「GitHub から取得する」のコマンドで、元の GitHub のリポジトリから、決まったタグかコミットの版を取ります。

- ZIP は `curl.exe` でダウンロードし、`tar` で展開します。
- リポジトリの一部だけを使う mvcmovie・contoso は、`git` でそのフォルダーだけを取ります。
- ブラウザーで取るときは、取得元のリンクからそのタグかコミットを開き、「Code」→「Download ZIP」を押します(mvcmovie・contoso を除く)。
- パスは短くします。元のアプリのビルドに使う .NET Framework の MSBuild は、260 文字を超えるパスを扱えないためです。

## 4. 変換する

### 4.1 コマンドラインで

```powershell
C:\src\FrameworkOnCore\converter.ps1 <Web プロジェクト> --out C:\foc\out\<名前> --root <ソースのフォルダー> --build-original auto
```

- `--root`: ソースの一番上のフォルダーです。プロジェクトがソリューションの外を参照していても、ここまでは見つけます。
- `--build-original auto`: 元のアプリのビルドが要るかを変換器が判断し、要るときはビルドしてから変換します。
- 各ページの「変換する」に、そのコーパスでの値を入れたコマンドがあります。
- 確認: 最後に `build succeeded; web project: ...; report: C:\foc\out\<名前>\CONVERSION-REPORT.md` と出れば成功です。
- 出力の中身:
  - `CONVERSION-REPORT.md`: 変換の結果です。何を変えたかと、自分で直すところが書かれています。
  - `Dockerfile`: Linux で動かすためのものです。
  - `deploy\README.md`: 配置のしかたです(環境変数、systemd)。

### 4.2 Studio で

```powershell
C:\src\FrameworkOnCore\studio.ps1
```

「Studio: http://127.0.0.1:5300/」と出たら、ブラウザーで http://127.0.0.1:5300/ を開きます。止めるときは、`studio.ps1` を動かしている PowerShell で Ctrl+C を押します。

1. 右上の「新しい解析」を押し、次の値を入れて「解析する」を押します。値は各ページの「Studio で入れる値」にあります。
   - 「プロジェクト(.csproj / .vbproj)」
   - 「リポジトリのフォルダー」
   - 「構成」
2. 解析が終わると、「自分で対応が要る部品」や「対応を選ぶ部品」が出ます。**既定のままで変換できます。**
3. 「サーバーの設定」も既定のまま(元の IIS と同じ動き)で進みます。
4. 「変換してビルド」を押します。終わると「ビルド成功」と、変換レポートが出ます。
   - 元のアプリのビルドが要るコーパスでは、「元のアプリをそのビルド手順でビルドし、配置されるサイトから組み立てる」に自動でチェックが入ります。
   - n2 は「追加の手順」も入れます。
5. 最後のステップで、変換したアプリを動かします。
   - 「ネイティブ起動(この PC)」: Windows で動かします([6](#6-windows-で動かす))。
   - 「コンテナ起動(Linux / Docker)」: Linux のコンテナで動かします([7](#7-linux-で動かす))。
   - どちらも「環境変数」の欄に、1 行に 1 つ `名前=値` を書いて、接続文字列などを渡せます。
   - 「ZIP ダウンロード」で、変換の出力を取り出せます。

Studio の出力は `%LOCALAPPDATA%\FrameworkOnCore\studio\` に置かれます(`C:\foc\out` ではありません)。

## 5. SQL Server を使うとき

SQL Server をコンテナで動かし、この PC のポート 14330 で公開します。Windows で動かすアプリも、Linux のコンテナで動かすアプリも、ここにつなぎます。

```powershell
docker run -d --name foc-sql -p 14330:1433 -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=Foc_local_Passw0rd" mcr.microsoft.com/mssql/server:2022-latest
```

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
cd <サイトのフォルダー>
dotnet bin\<Web アプリ>.dll --urls http://localhost:5000
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

変換の出力フォルダー(`Dockerfile` のあるところ)でイメージを作り、コンテナを起動します。

```powershell
cd C:\foc\out\<名前>
docker build -t <名前> .
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
| 元のアプリのビルドが失敗する | Build Tools が入っていないか、パスが長すぎます。ソースを短いパスに置きます |
| ポート(5000、8080、14330)が使われている | 前のアプリやコンテナを止めます。または別のポートを使います(`--urls http://localhost:5001`、`-p 8081:8080`) |
| Docker が遅い・止まる | ほかのコンテナを止めます。`.wslconfig` の memory を見直します |
| ディスクがいっぱい | 不要なイメージを消します(`docker image prune -a`、`docker builder prune`) |
| アプリが 500 になる・起動しない | Windows では PowerShell に出るログ、コンテナでは `docker logs <コンテナ>`、Studio では画面のログを見ます。多くは接続文字列の誤りです |
