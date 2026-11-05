# 共通の準備

どのコーパスでも、流れは同じです。GitHub からソースを取り、FrameworkOnCore の変換器で変換し、変換が書いた Dockerfile で Linux のコンテナとして動かします。
コマンドは Windows PowerShell 5.1 で実行します。

## 1. 入れるもの

| もの | 要るコーパス |
|---|---|
| Windows 10 / 11(x64) | すべて(変換は Windows で行います) |
| [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) | すべて |
| [Git](https://git-scm.com/download/win) | すべて |
| [Docker Desktop](https://docs.docker.com/desktop/setup/install/windows-install/)(WSL 2) | すべて(yaf を除く) |
| [Visual Studio Build Tools](https://visualstudio.microsoft.com/downloads/)(「Web 開発ビルド ツール」と .NET Framework 4.8 の Targeting Pack) | mojo、dnn、n2、imis、nop、nop390(元のアプリのビルドが要るもの) |

SQL Server は、Linux のコンテナで動かします。Windows に入れる必要はありません。

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

- 変換器(`converter.ps1`)は、GitHub Actions がビルドしたものを初めて使うときに自動で取ってきます。変換後のアプリが使う FrameworkOnCore のパッケージも同じく自動です。
- リポジトリが非公開のときは、先に `gh auth login` をしておきます。

Linux でもファイル名の大文字・小文字を区別せずに動かすためのライブラリ(casefs)も、一度だけビルドします。変換がこれを Dockerfile に入れます。Docker Desktop を起動してから実行します。

```powershell
C:\src\FrameworkOnCore\experiments\wf4c\casefs\build.ps1
```

## 3. 作業用のフォルダー

ソースと変換の出力は `C:\foc` に置きます。各ページのコマンドは、このフォルダーで実行します。

```powershell
New-Item -ItemType Directory -Force C:\foc | Out-Null
cd C:\foc
```

パスは短くします。元のアプリのビルドに使う .NET Framework の MSBuild は、260 文字を超えるパスを扱えないためです。

## 4. ソースを取る

各ページの「1. GitHub から取得する」のコマンドで、元の GitHub のリポジトリから、決まったタグかコミットの版を取ります。

- ZIP は `curl.exe` でダウンロードし、`tar` で展開します。
- リポジトリの一部だけを使う mvcmovie・contoso は、`git` でそのフォルダーだけを取ります。

ブラウザーで取るときは、取得元のリンクからそのタグかコミットを開き、「Code」→「Download ZIP」を押します(mvcmovie・contoso を除く)。

## 5. 変換する

```powershell
C:\src\FrameworkOnCore\converter.ps1 <Web プロジェクト> --out C:\foc\out\<名前> --root <ソースのフォルダー> --build-original auto
```

- `--root`: ソースの一番上のフォルダーです。プロジェクトがソリューションの外を参照していても、ここまでは見つけます。
- `--build-original auto`: 元のアプリのビルドが要るかを変換器が判断し、要るときはビルドしてから変換します。
- 各ページには、そのコーパスでの値を入れたコマンドがあります。
- 確認: 最後に `build succeeded; web project: ...; report: C:\foc\out\<名前>\CONVERSION-REPORT.md` と出れば成功です。
- 出力の中身:
  - `CONVERSION-REPORT.md`: 変換の結果です。何を変えたかと、自分で直すところが書かれています。
  - `Dockerfile`: Linux で動かすためのものです。
  - `deploy\README.md`: 配置のしかたです(環境変数、systemd)。

画面で変換するときは、`C:\src\FrameworkOnCore\studio.ps1` を起動して http://127.0.0.1:5300/ を開きます。「プロジェクト」と「リポジトリのフォルダー」に同じ値を入れます(手順は [README](../../README.md))。

## 6. Linux で動かす

Docker Desktop を起動しておき、各ページの「3. Linux で動かす」のコマンドを実行します。

- 変換の出力フォルダーで `docker build` すると、イメージができます。
- コンテナはポート 8080 で待ち受けます。
- DB の接続文字列は、web.config を書き換えずに、環境変数で渡します。
  - `SQLCONNSTR_<接続文字列の名前>`: 接続文字列を置き換えます。
  - `APPSETTING_<キー>`: appSettings の値を設定します。
- SQL Server を使うコーパスでは、SQL Server もコンテナ(`foc-sql`)で動かします。アプリのコンテナと同じネットワーク(`foc-net`)につなぎます。

```powershell
docker network create foc-net
docker run -d --name foc-sql --network foc-net -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=Foc_local_Passw0rd" mcr.microsoft.com/mssql/server:2022-latest
```

SQL Server は、起動してから接続できるまでに 10〜20 秒かかります。

## 7. 片付け

```powershell
docker rm -f <アプリのコンテナ> foc-sql
docker network rm foc-net
Stop-Process -Name 'Docker Desktop' -ErrorAction SilentlyContinue
wsl --shutdown                                     # WSL のメモリーを返す
```

コンテナは一度に一つ(SQL Server を使うときは、SQL Server とアプリの 2 つ)にします。

## 8. うまくいかないとき

| 症状 | 対処 |
|---|---|
| 元のアプリのビルドが失敗する | Build Tools が入っていないか、パスが長すぎます。ソースを短いパスに置きます |
| ポート 8080 が使われている | 前のコンテナを `docker rm -f` で消すか、`-p 8081:8080` のように別のポートを使います |
| Docker が遅い・止まる | ほかのコンテナを止めます。`.wslconfig` の memory を見直します |
| ディスクがいっぱい | 不要なイメージを消します(`docker image prune -a`、`docker builder prune`) |
| アプリが 500 になる・起動しない | `docker logs <コンテナ>` を見ます |
