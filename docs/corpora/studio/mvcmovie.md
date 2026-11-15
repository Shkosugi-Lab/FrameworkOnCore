# MvcMovie を Studio で変換する

映画の一覧を扱う ASP.NET MVC 5 のチュートリアルです(C#、EF6、SQL Server)。Studio(画面)で解析・変換し、Windows と Linux のコンテナで動かします。コマンドラインでの手順は [コマンド版](../mvcmovie.md) です。

| | |
|---|---|
| 取得元 | [dotnet/AspNetDocs](https://github.com/dotnet/AspNetDocs) コミット `552c58dd` の `aspnet/mvc/overview/getting-started/introduction/sample/MvcMovie` |
| ライセンス | コードは MIT、文書は CC BY 4.0 |

入れておくもの(.NET 10 SDK、Git、Docker Desktop、**Visual Studio または Visual Studio Build Tools**)は [共通の準備の 1](../setup.md#1-入れるもの) にあります。Studio はステップ 1 で元のアプリをビルドするので、Visual Studio か Build Tools が要ります。SQL Server はコンテナで動かします(下の 2)。

## 1. FrameworkOnCore とソースを取る

作業用の空のフォルダー(例: `C:\foc`。パスは短くします)を作り、FrameworkOnCore を取ります。ほかのシナリオで済ませてあれば、`cd C:\foc` だけにします。

```powershell
New-Item -ItemType Directory -Force C:\foc | Out-Null
cd C:\foc
git clone https://github.com/Shkosugi-Lab/FrameworkOnCore.git
```

同じフォルダーに、アプリのソースを取ります。

大きなリポジトリなので、そのフォルダーだけを取ります。

```powershell
git clone --filter=blob:none --no-checkout --sparse https://github.com/dotnet/AspNetDocs.git AspNetDocs
git -C AspNetDocs sparse-checkout set aspnet/mvc/overview/getting-started/introduction/sample/MvcMovie
git -C AspNetDocs checkout 552c58dd8a809ef8cea02e66c9463ce9c7dcdfe5
Move-Item AspNetDocs\aspnet\mvc\overview\getting-started\introduction\sample\MvcMovie MvcMovie
Remove-Item AspNetDocs -Recurse -Force
```

### MvcMovie だけの準備: packages.config を直す

このサンプルは、packages.config の 4 つのパッケージの版だけが新しくなっていて(.csproj と web.config は前の版のまま)、そのままでは Visual Studio でもビルドできません。Studio はステップ 1 で元のアプリをビルドするので、packages.config を .csproj の版に戻します(コマンドラインの手順は元のアプリをビルドしないので、この準備は要りません)。

```powershell
$p = '.\MvcMovie\MvcMovie\packages.config'
$c = Get-Content $p -Raw
foreach ($v in @(('Microsoft.AspNet.Identity.Owin', '2.2.4', '2.2.1'), ('Microsoft.Owin', '4.2.2', '3.0.1'), ('Microsoft.Owin.Security.Cookies', '4.2.2', '3.0.1'), ('Newtonsoft.Json', '13.0.1', '6.0.4'))) { $c = $c.Replace("id=""$($v[0])"" version=""$($v[1])""", "id=""$($v[0])"" version=""$($v[2])""") }
Set-Content $p $c -NoNewline -Encoding UTF8
```


## 2. DB を用意する

SQL Server のコンテナを起動します([共通の準備の 5](../setup.md#5-sql-server-を使うとき))。DB は、アプリ(EF)が最初の要求で作ります。

```powershell
docker run -d --name foc-sql -p 14330:1433 -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=Foc_local_Passw0rd" mcr.microsoft.com/mssql/server:2022-latest
```

ARM の PC(Windows on ARM)では、SQL Server の代わりに Azure SQL Edge を使います([共通の準備の 5](../setup.md#arm-の-pcwindows-on-armでは))。2 回目以降は `docker start foc-sql` で起動します。

元の Web.config の接続文字列(LocalDB)は、環境変数で置き換えます。

| 接続文字列 | DB |
|---|---|
| `MovieDBContext` | `MvcMovie` |
| `DefaultConnection` | `MvcMovieIdentity` |

## 3. Studio を起動する

ソースを取ったフォルダーで起動します。こうすると、Studio に入れるパスをこのフォルダーからの相対パスで書けます。

```powershell
.\FrameworkOnCore\studio.ps1
```

「Studio: http://127.0.0.1:5300/」と出たら、ブラウザーで http://127.0.0.1:5300/ を開きます。止めるときは、この PowerShell で Ctrl+C を押します。

右上の **「新しい解析」** を押します。

![Studio の最初の画面](images/mvcmovie/01-home.png)

## 4. 新しい解析を作る

次の値を入れて、**「解析する」** を押します。

| 欄 | 値 |
|---|---|
| プロジェクト(.csproj / .vbproj) | `.\MvcMovie\MvcMovie\MvcMovie.csproj` |
| リポジトリのフォルダー | `.\MvcMovie` |
| 構成 | `Release` |

![新しい解析](images/mvcmovie/02-new-analysis.png)

## 5. ステップ 1 読み込み

プロジェクトの構成(言語、.NET Framework の版、NuGet パッケージの数、ページの数など)が出ます。解析は裏で進みます。

下の「元のアプリのビルド」で、**「ビルドに使う MSBuild」**(Visual Studio か Build Tools。入っていないほうは選べません)を確かめて、**「ビルド」** を押します。元のアプリを、そのリポジトリのビルド手順でビルドします。**ビルドが通るまで、ステップ 2 以降には進めません。**「追加の手順」は空のままです。

![元のアプリのビルド(ビルド前)](images/mvcmovie/04-step1-build.png)

ビルド中は、その下の「ログ」にビルドの様子が出ます。**「ビルド成功」** と出たら、いちばん下の **「次へ」** を押します。

ビルドが通ると「元のアプリを起動(テスト起動)」も出ます。変換前の動きを確かめたいときに使います(IIS Express が要ります。この手順では使いません)。

![元のアプリのビルド(ビルド成功)](images/mvcmovie/05-step1-built.png)

## 6. ステップ 2 解析と対応

アプリが使う .NET Framework の API と、変換での扱いが出ます。「自分で対応する所」のタブは変換だけでは動くようにならない所、「対応を選ぶ部品」のタブは部品ごとに直し方を選べる所です。

![ステップ 2 解析と対応](images/mvcmovie/06-step2.png)

**既定のままで変換できます。** いちばん下の **「次へ」** を押します。

![ステップ 2 の「次へ」](images/mvcmovie/06b-step2-next.png)

## 7. ステップ 3 サーバーの設定

IIS の機能(ファイル名の大文字・小文字、アクセス ログ、URL Rewrite、エラー ページなど)の動かし方を選べます。既定は、元の IIS と同じ動きです。既定のまま **「次へ」** を押します。

![ステップ 3 サーバーの設定](images/mvcmovie/07-step3.png)

## 8. ステップ 4 変換・ビルド

**「変換してビルド」** を押します。元のアプリをステップ 1 と同じ手順でまっさらな状態からビルドし直し、.NET 10 に変換してビルドします(数分かかります)。

![ステップ 4 変換・ビルド(実行前)](images/mvcmovie/08-step4.png)

**「ビルド成功」** になり、下に変換レポートが出ます。**「未解決」が 0** であることを確かめて、**「次へ: 実行・デプロイ」** を押します。

![ステップ 4 変換・ビルド(ビルド成功)](images/mvcmovie/09-step4-done.png)

## 9. ステップ 5 実行・デプロイ

変換したアプリを動かします。「ZIP ダウンロード」では、変換の出力を Linux 用・Windows 用に取り出せます。

### 9.1 Windows で動かす(ネイティブ起動)

「ネイティブ起動(この PC)」の **「環境変数」** に、次の行を書きます(`SQLCONNSTR_<名前>` が接続文字列を置き換えます。入っている例の行は消して構いません)。そして **「起動」** を押します。

```
SQLCONNSTR_MovieDBContext=Data Source=localhost,14330;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True;Initial Catalog=MvcMovie
SQLCONNSTR_DefaultConnection=Data Source=localhost,14330;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True;Initial Catalog=MvcMovieIdentity
```

![ネイティブ起動](images/mvcmovie/11-native.png)

「実行中」になったら、**「開く」** を押します(開いたページの URL の後ろに `/Movies` を付けます)。最初の表示は、ページのコンパイルや DB の作成で時間がかかることがあります。止めるときは「停止」を押します。

![ネイティブ起動(実行中)](images/mvcmovie/12-native-running.png)

### 9.2 Linux で動かす(コンテナ起動)

Docker Desktop を起動しておきます(止まっていると「Docker Desktop を起動」ボタンが出るので、それを押しても構いません)。

「コンテナ起動(Linux / Docker)」の **「環境変数」** に、次の行を書きます(コンテナから見たこの PC は `host.docker.internal` です)。そして **「ビルドして起動」** を押します。

```
SQLCONNSTR_MovieDBContext=Data Source=host.docker.internal,14330;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True;Initial Catalog=MvcMovie
SQLCONNSTR_DefaultConnection=Data Source=host.docker.internal,14330;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True;Initial Catalog=MvcMovieIdentity
```

![コンテナ起動](images/mvcmovie/14-container.png)

Dockerfile からイメージを作るので、初回は数分かかります。「実行中」になったら、**「開く」** を押します(URL の後ろに `/Movies` を付けます)。止めるときは「停止」を押します。

![コンテナ起動(実行中)](images/mvcmovie/15-container-running.png)

映画の一覧(`/Movies`)です。「Create New」で映画を登録できます。

![コンテナで動いている MvcMovie](images/mvcmovie/16-container-app.png)

## 10. 片付け

- アプリは「停止」で止めます。Studio は、起動した PowerShell で Ctrl+C を押して止めます。Studio を止めると、Studio が起動したアプリも止まります。
- SQL Server のコンテナは `docker rm -f foc-sql` で消します(DB も消えます)。
- 終わったら Docker Desktop を止め、`wsl --shutdown` で WSL のメモリーを返します。
- Studio の出力は `%LOCALAPPDATA%\FrameworkOnCore\studio\` にあります。不要になった解析は、画面右上の「削除」で消せます。
