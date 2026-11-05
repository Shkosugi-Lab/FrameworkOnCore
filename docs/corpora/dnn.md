# dnn(DNN Platform 9.13.10)

大きな CMS です(Web Forms、C# と VB、SQL Server)。[共通の準備](setup.md) が終わっている前提で、コマンドは作業フォルダー(下の 0)で実行します。

| | |
|---|---|
| 取得元 | [dnnsoftware/Dnn.Platform](https://github.com/dnnsoftware/Dnn.Platform) タグ `v9.13.10` |
| ライセンス | MIT |
| 追加で要るもの | Visual Studio か Build Tools、Docker Desktop(SQL Server)。ディスク約 20 GB |

## 0. FrameworkOnCore を取る

作業用の空のフォルダー(例: `C:\foc`。パスは短くします)を作って移り、そこに FrameworkOnCore を取ります。以降のコマンドは、すべてこのフォルダーで、ここからの相対パスで実行します。ほかのコーパスで済ませてあれば、この手順は飛ばします。

```powershell
New-Item -ItemType Directory -Force C:\foc | Out-Null
cd C:\foc
git clone https://github.com/Shkosugi-Lab/FrameworkOnCore.git
.\FrameworkOnCore\experiments\wf4c\casefs\build.ps1     # Docker Desktop を起動してから(Linux 用。一度だけ)
```

## 1. GitHub から取得する

```powershell
curl.exe -L -o dnn.zip https://github.com/dnnsoftware/Dnn.Platform/archive/refs/tags/v9.13.10.zip
tar -xf dnn.zip          # Dnn.Platform-9.13.10 ができる
```

## 2. 変換する

元のアプリを DNN の Cake のビルドでビルドしてから変換します(`--build-original auto` が判断します)。初回は長くかかります。

```powershell
.\FrameworkOnCore\converter.ps1 ".\Dnn.Platform-9.13.10\DNN Platform\Website\DotNetNuke.Website.csproj" --out .\out\dnn --root .\Dnn.Platform-9.13.10 --build-original auto
```

`the build failed after it deployed the site: ... is used as it is` と出ても、そのまま進めて構いません。ビルドの一部(パッケージ作り)が失敗しただけで、サイトはできています。

**Studio で入れる値**([使い方](setup.md#42-studio-で))

| 欄 | 値 |
|---|---|
| プロジェクト | `.\Dnn.Platform-9.13.10\DNN Platform\Website\DotNetNuke.Website.csproj` |
| リポジトリのフォルダー | `.\Dnn.Platform-9.13.10` |
| 構成 | `Release` |

## 3. DB を用意する

SQL Server のコンテナを起動し([共通の準備の 5](setup.md#5-sql-server-を使うとき))、空の DB を Windows 用と Linux 用に 1 つずつ作ります。テーブルは DNN のインストールが作ります。

```powershell
docker run -d --name foc-sql -p 14330:1433 -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=Foc_local_Passw0rd" mcr.microsoft.com/mssql/server:2022-latest
Start-Sleep 20
docker exec foc-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Foc_local_Passw0rd -C -Q "CREATE DATABASE dnn_windows; CREATE DATABASE dnn_linux"
```

接続文字列は `SiteSqlServer` です。

## 4. Windows で動かす

インストールでサイトのファイルが書き換わります。Linux でも試すときは、先に 5 の `docker build` を済ませておきます([共通の準備の 7](setup.md#7-linux-で動かす))。

```powershell
Push-Location .\out\dnn\site
$env:SQLCONNSTR_SiteSqlServer = 'Data Source=localhost,14330;Initial Catalog=dnn_windows;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True'
dotnet bin\DotNetNuke.Website.dll --urls http://localhost:5000
Pop-Location                  # Ctrl+C で止めたあと、作業フォルダーに戻る
```

http://localhost:5000/Install/Install.aspx?mode=install を開くと、インストールが始まります(数分かかります)。終わるとトップ(`/`)、`/Login`、`/Terms`、`/Privacy` が開けます。
Studio では、「ネイティブ起動(この PC)」の「環境変数」に `SQLCONNSTR_SiteSqlServer=<上の値>` と書きます。

## 5. Linux で動かす

```powershell
docker build -t dnn .\out\dnn
docker run -d -p 8080:8080 --name dnn -e "SQLCONNSTR_SiteSqlServer=Data Source=host.docker.internal,14330;Initial Catalog=dnn_linux;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True" dnn
```

http://localhost:8080/Install/Install.aspx?mode=install を開いてインストールします。Studio では「コンテナ起動(Linux / Docker)」の「環境変数」に、この値を書きます。

記録では、Windows・Linux ともインストールが終わり、これらのページが 200 でした(2026-10-11)。ただし、このページの手順そのものは試していません。

## 6. 片付け

```powershell
docker rm -f dnn foc-sql
```
