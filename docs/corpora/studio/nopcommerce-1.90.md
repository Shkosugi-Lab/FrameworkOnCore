# nopCommerce 1.90 を Studio で変換する

EC サイトです(Web Forms、C#、SQL Server)。Studio(画面)で解析・変換し、Windows と Linux のコンテナで動かします。コマンドラインでの手順は [コマンド版](../nopcommerce-1.90.md) です。

| | |
|---|---|
| 取得元 | [nopSolutions/nopCommerce](https://github.com/nopSolutions/nopCommerce) タグ `release-1.90` |
| ライセンス | nopCommerce Public License 1.0 |

入れておくもの(.NET 10 SDK、Git、Docker Desktop、**Visual Studio または Visual Studio Build Tools**)は [共通の準備の 1](../setup.md#1-入れるもの) にあります。Studio はステップ 1 で元のアプリをビルドするので、Visual Studio か Build Tools が要ります。SQL Server はコンテナで動かします(下の 2)。

## 1. FrameworkOnCore とソースを取る

作業用の空のフォルダー(例: `C:\foc`。パスは短くします)を作り、FrameworkOnCore を取ります。ほかのシナリオで済ませてあれば、`cd C:\foc` だけにします。

```powershell
New-Item -ItemType Directory -Force C:\foc | Out-Null
cd C:\foc
git clone https://github.com/Shkosugi-Lab/FrameworkOnCore.git
```

同じフォルダーに、アプリのソースを取ります。

```powershell
curl.exe -L -o nopcommerce-1.90.zip https://github.com/nopSolutions/nopCommerce/archive/refs/tags/release-1.90.zip
tar -xf nopcommerce-1.90.zip          # nopCommerce-release-1.90 ができる
```

## 2. DB を用意する

SQL Server のコンテナを起動します([共通の準備の 5](../setup.md#5-sql-server-を使うとき))。DB は、アプリのインストーラーが作ります。

```powershell
docker run -d --name foc-sql -p 14330:1433 -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=Foc_local_Passw0rd" mcr.microsoft.com/mssql/server:2022-latest
```

ARM の PC(Windows on ARM)では、SQL Server の代わりに Azure SQL Edge を使います([共通の準備の 5](../setup.md#arm-の-pcwindows-on-armでは))。2 回目以降は `docker start foc-sql` で起動します。

## 3. Studio を起動する

ソースを取ったフォルダーで起動します。こうすると、Studio に入れるパスをこのフォルダーからの相対パスで書けます。

```powershell
.\FrameworkOnCore\studio.ps1
```

「Studio: http://127.0.0.1:5300/」と出たら、ブラウザーで http://127.0.0.1:5300/ を開きます。止めるときは、この PowerShell で Ctrl+C を押します。

右上の **「新しい解析」** を押します。

![Studio の最初の画面](images/nopcommerce-1.90/01-home.png)

## 4. 新しい解析を作る

次の値を入れて、**「解析する」** を押します。

| 欄 | 値 |
|---|---|
| プロジェクト(.csproj / .vbproj) | `.\nopCommerce-release-1.90\NopCommerceStore\NopCommerceStore.csproj` |
| リポジトリのフォルダー | `.\nopCommerce-release-1.90` |
| 構成 | `Release` |

![新しい解析](images/nopcommerce-1.90/02-new-analysis.png)

## 5. ステップ 1 読み込み

プロジェクトの構成(言語、.NET Framework の版、NuGet パッケージの数、ページの数など)が出ます。解析は裏で進みます。

下の「元のアプリのビルド」で、**「ビルドに使う MSBuild」**(Visual Studio か Build Tools。入っていないほうは選べません)を確かめて、**「ビルド」** を押します。元のアプリを、そのリポジトリのビルド手順でビルドします。**ビルドが通るまで、ステップ 2 以降には進めません。**「追加の手順」は空のままです。

![元のアプリのビルド(ビルド前)](images/nopcommerce-1.90/04-step1-build.png)

ビルド中は、その下の「ログ」にビルドの様子が出ます。**「ビルド成功」** と出たら、いちばん下の **「次へ」** を押します。

ビルドが通ると「元のアプリを起動(テスト起動)」も出ます。変換前の動きを確かめたいときに使います(IIS Express が要ります。この手順では使いません)。

![元のアプリのビルド(ビルド成功)](images/nopcommerce-1.90/05-step1-built.png)

## 6. ステップ 2 解析と対応

アプリが使う .NET Framework の API と、変換での扱いが出ます。「自分で対応する所」のタブは変換だけでは動くようにならない所、「対応を選ぶ部品」のタブは部品ごとに直し方を選べる所です。

![ステップ 2 解析と対応](images/nopcommerce-1.90/06-step2.png)

**既定のままで変換できます。** いちばん下の **「次へ」** を押します。

![ステップ 2 の「次へ」](images/nopcommerce-1.90/06b-step2-next.png)

## 7. ステップ 3 サーバーの設定

IIS の機能(ファイル名の大文字・小文字、アクセス ログ、URL Rewrite、エラー ページなど)の動かし方を選べます。既定は、元の IIS と同じ動きです。既定のまま **「次へ」** を押します。

![ステップ 3 サーバーの設定](images/nopcommerce-1.90/07-step3.png)

## 8. ステップ 4 変換・ビルド

**「変換してビルド」** を押します。元のアプリをステップ 1 と同じ手順でまっさらな状態からビルドし直し、.NET 10 に変換してビルドします(数分かかります)。

![ステップ 4 変換・ビルド(実行前)](images/nopcommerce-1.90/08-step4.png)

**「ビルド成功」** になり、下に変換レポートが出ます。**「未解決」が 0** であることを確かめて、**「次へ: 実行・デプロイ」** を押します。

![ステップ 4 変換・ビルド(ビルド成功)](images/nopcommerce-1.90/09-step4-done.png)

## 9. ステップ 5 実行・デプロイ

変換したアプリを動かします。「ZIP ダウンロード」では、変換の出力を Linux 用・Windows 用に取り出せます。

このアプリは、最初の起動でインストールの画面が出て、その設定(接続文字列)をサイトに書きます。Windows と Linux の両方で試すときは、Windows でインストールする前に、コンテナ起動(下の Linux)を済ませておきます。

### 9.1 Windows で動かす(ネイティブ起動)

「ネイティブ起動(この PC)」の **「起動」** を押します。環境変数は要りません。

![ネイティブ起動](images/nopcommerce-1.90/11-native.png)

「実行中」になったら、**「開く」** を押します(開いたページの URL の後ろに `/install/install.aspx` を付けます)。最初の表示は、ページのコンパイルで時間がかかることがあります。止めるときは「停止」を押します。

![ネイティブ起動(実行中)](images/nopcommerce-1.90/12-native-running.png)

### 9.2 Linux で動かす(コンテナ起動)

Docker Desktop を起動しておきます(止まっていると「Docker Desktop を起動」ボタンが出るので、それを押しても構いません)。

「コンテナ起動(Linux / Docker)」の **「ビルドして起動」** を押します。環境変数は要りません。

![コンテナ起動](images/nopcommerce-1.90/14-container.png)

Dockerfile からイメージを作るので、初回は数分かかります。「実行中」になったら、**「開く」** を押します(URL の後ろに `/install/install.aspx` を付けます)。止めるときは「停止」を押します。

![コンテナ起動(実行中)](images/nopcommerce-1.90/15-container-running.png)

インストールの画面です。[コマンド版の 4](../nopcommerce-1.90.md#4-windows-で動かす) の表の値でインストールします(SQL Server は、ネイティブ起動では `localhost,14330`、コンテナでは `host.docker.internal,14330`。DB の名前は Windows と Linux で別にします)。管理者は `admin@yourstore.com` / `admin` です。

![コンテナで動いている nopCommerce 1.90](images/nopcommerce-1.90/16-container-app.png)

## 10. 片付け

- アプリは「停止」で止めます。Studio は、起動した PowerShell で Ctrl+C を押して止めます。Studio を止めると、Studio が起動したアプリも止まります。
- SQL Server のコンテナは `docker rm -f foc-sql` で消します(DB も消えます)。
- 終わったら Docker Desktop を止め、`wsl --shutdown` で WSL のメモリーを返します。
- Studio の出力は `%LOCALAPPDATA%\FrameworkOnCore\studio\` にあります。不要になった解析は、画面右上の「削除」で消せます。
