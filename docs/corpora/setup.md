# 共通の準備

どのコーパスでも、一度だけ行う準備と、同じ流れ(取得 → 変換 → Linux のコンテナで動かす)です。
コマンドは、リポジトリの一番上のフォルダーで、Windows PowerShell 5.1 で実行します。

## 1. 入れるもの

| もの | 要るコーパス |
|---|---|
| Windows 10 / 11(x64) | すべて(変換は Windows で行います) |
| [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) | すべて |
| [Git](https://git-scm.com/download/win) | すべて |
| [Docker Desktop](https://docs.docker.com/desktop/setup/install/windows-install/)(WSL 2) | すべて(yaf を除く) |
| [Visual Studio Build Tools](https://visualstudio.microsoft.com/downloads/)(「Web 開発ビルド ツール」と .NET Framework 4.8 の Targeting Pack) | mojo、dnn、n2、imis、nop、nop390(元のアプリのビルドが要るもの) |
| [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads)(インスタンス名 `SQLEXPRESS`)と `sqlcmd` | mojo、dnn、imis(変換のときに DB を作るもの) |

Linux 側の SQL Server は、コンテナで動かします(入れる必要はありません)。

WSL 2 のメモリーの上限は `%USERPROFILE%\.wslconfig` に書いておきます(16 GB の PC なら 8 GB)。

```ini
[wsl2]
memory=8GB
```

## 2. リポジトリを取る

```powershell
git clone https://github.com/Shkosugi-Lab/FrameworkOnCore.git C:\src\FrameworkOnCore
cd C:\src\FrameworkOnCore
Set-ExecutionPolicy -Scope CurrentUser RemoteSigned     # スクリプトを動かせるようにする(一度だけ)
```

- 置く場所のパスは短くします(元のアプリのビルドに使う .NET Framework の MSBuild は、260 文字を超えるパスを扱えません)。
- リポジトリが非公開のときは `gh auth login` をしておきます。FrameworkOnCore のパッケージは、初めて変換したときに GitHub Release から自動で取ってきます。

## 3. casefs をビルドする(一度だけ)

Linux でもファイル名の大文字・小文字を区別せずに動かすためのライブラリです。Docker Desktop を起動してから実行します。

```powershell
.\experiments\wf4c\casefs\build.ps1
```

## 4. コーパスを取る

コーパスはこのリポジトリには入っていません。各ページの「GitHub から取得して変換する」のコマンドで、元の GitHub のリポジトリから取ります。

- 取るのは、各ページに書いたタグかコミットの版です。ZIP を `curl.exe` でダウンロードし、`tar` で展開します。リポジトリの一部だけを使う mvcmovie・contoso は、`git` でそのフォルダーだけを取ります。
- 置き場所は `corpora\work\<フォルダー>` です(変換のスクリプトがこのパスを使います)。先に作っておきます。

```powershell
New-Item -ItemType Directory -Force corpora\work | Out-Null
```

ブラウザーで取るとき(mvcmovie・contoso を除く)は、各ページの取得元のリンクからそのタグかコミットを開き、「Code」→「Download ZIP」でダウンロードします。展開したフォルダーは、`corpora\work\` に各ページの名前で置きます。
`.\corpora\fetch.ps1 -Only <名前>` を使えば、同じことを 1 行で行えます。

## 5. 変換する

```powershell
.\experiments\wf4c\convert-corpora.ps1 -Only be
```

- 出力は `experiments\wf4c\<名前>` です(変換の結果は `CONVERSION-REPORT.md`)。元のアプリのビルドが要るコーパスは、最初の 1 回だけ数分〜数十分かかります。
- 確認: `=== <名前> BUILD OK` と出れば成功です。
- 最後に `differ from their snapshot` で失敗扱いになることがあります。これは変換の判断が以前の記録と違うという意味で、`BUILD OK` が出ていれば、変換の結果はそのまま使えます。

画面で試すときは `.\studio.ps1` を起動し、http://127.0.0.1:5300/ を開きます(手順は [README](../../README.md))。
「プロジェクト」には各ページの Web プロジェクト、「リポジトリのフォルダー」には `corpora\work\<フォルダー>` を指定します。

## 6. Linux で動かす

Docker Desktop を起動しておき、各ページの「Linux で動かす」のコマンドを実行します。動かし方は 2 通りです。

- **変換が書いた Dockerfile**(be、wt、nop390): 本番と同じイメージを作ります。接続文字列は環境変数 `SQLCONNSTR_<名前>` で渡します。
- **`run-linux-site.ps1`**(mojo、dnn、n2、imis、nop): 変換が組み立てたサイトを、そのままコンテナで動かします。SQL Server もコンテナ(`w2l-sql`)で動かし、接続文字列をそこへ向けます。`-Keep` を付けるとコンテナが残るので、http://localhost:5098/ をブラウザーで開けます。

mvcmovie・contoso は `run-linux.ps1` を使います。

コンテナは一度に一つ(DB の要るコーパスは、SQL Server とアプリの 2 つ)にします。

## 7. 片付け

```powershell
docker rm -f <アプリのコンテナ> w2l-sql foc-sql     # docker ps -a で名前を確かめる
Stop-Process -Name 'Docker Desktop' -ErrorAction SilentlyContinue
wsl --shutdown                                     # WSL のメモリーを返す
```

SQL Server Express を使ったときは、管理者の PowerShell で `Stop-Service 'MSSQL$SQLEXPRESS'` を実行して止めます。

## 8. うまくいかないとき

| 症状 | 対処 |
|---|---|
| 元のアプリのビルドが失敗する | Build Tools の入れ忘れか、パスが長すぎます。リポジトリを短いパス(`C:\src\...`)に置きます |
| ポートが使われている | `Get-NetTCPConnection -LocalPort 5098 -State Listen` で使っているプロセスを調べて止めます。前のコンテナが残っているときは `docker rm -f` で消します |
| Docker が遅い・止まる | 動いているほかのコンテナを止めます。`.wslconfig` の memory を見直します |
| ディスクがいっぱい | 不要なイメージを消します(`docker image prune -a`、`docker builder prune`) |
| アプリが 500 になる | `docker logs <コンテナ>`、または `experiments\wf4c\_linux\<名前>-site\container.log` を見ます |
