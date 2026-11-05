# mojo(mojoPortal 3.1.6)

CMS です(Web Forms、C#、SQL Server)。[共通の準備](setup.md) が終わっている前提で、コマンドは作業フォルダー(下の 0)で実行します。

| | |
|---|---|
| 取得元 | [i7MEDIA/mojoportal](https://github.com/i7MEDIA/mojoportal) タグ `v3.1.6` |
| ライセンス | EPL-1.0 |
| 追加で要るもの | Visual Studio か Build Tools、Docker Desktop(SQL Server)。ディスク約 5 GB |

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
curl.exe -L -o mojo.zip https://github.com/i7MEDIA/mojoportal/archive/refs/tags/v3.1.6.zip
tar -xf mojo.zip          # mojoportal-3.1.6 ができる
```

## 2. 変換する

元のアプリをソリューションでビルドしてから変換します(`--build-original auto` が判断します)。

```powershell
.\FrameworkOnCore\converter.ps1 .\mojoportal-3.1.6\Web\mojoPortal.Web.csproj --out .\out\mojo --root .\mojoportal-3.1.6 --build-original auto
```

**Studio で入れる値**([使い方](setup.md#42-studio-で))

| 欄 | 値 |
|---|---|
| プロジェクト | `.\mojoportal-3.1.6\Web\mojoPortal.Web.csproj` |
| リポジトリのフォルダー | `.\mojoportal-3.1.6` |
| 構成 | `Release` |

## 3. DB を用意する

SQL Server のコンテナを起動し([共通の準備の 5](setup.md#5-sql-server-を使うとき))、空の DB を Windows 用と Linux 用に 1 つずつ作ります。テーブルは、最初に開くセットアップの画面が作ります。

```powershell
docker run -d --name foc-sql -p 14330:1433 -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=Foc_local_Passw0rd" mcr.microsoft.com/mssql/server:2022-latest
Start-Sleep 20
docker exec foc-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Foc_local_Passw0rd -C -Q "CREATE DATABASE mojo_windows; CREATE DATABASE mojo_linux"
```

接続文字列は、appSettings の `MSSQLConnectionString` として渡します(`APPSETTING_MSSQLConnectionString`)。

## 4. Windows で動かす

Linux でも試すときは、先に 5 の `docker build` を済ませておきます([共通の準備の 7](setup.md#7-linux-で動かす))。

```powershell
Push-Location .\out\mojo\site
$env:APPSETTING_MSSQLConnectionString = 'Data Source=localhost,14330;Initial Catalog=mojo_windows;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True'
dotnet bin\mojoPortal.Web.dll --urls http://localhost:5000
Pop-Location                  # Ctrl+C で止めたあと、作業フォルダーに戻る
```

http://localhost:5000/ を開くと、セットアップ(`/Setup/Default.aspx`)がテーブルを作ります。終わるとトップ(`Home - mojoPortal`)が開きます。
Studio では、「ネイティブ起動(この PC)」の「環境変数」に `APPSETTING_MSSQLConnectionString=<上の値>` と書きます。

## 5. Linux で動かす

```powershell
docker build -t mojo .\out\mojo
docker run -d -p 8080:8080 --name mojo -e "APPSETTING_MSSQLConnectionString=Data Source=host.docker.internal,14330;Initial Catalog=mojo_linux;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True" mojo
```

http://localhost:8080/ を開くと、同じようにセットアップが動きます。Studio では「コンテナ起動(Linux / Docker)」の「環境変数」に、この値を書きます。

記録では、Windows・Linux ともセットアップのあと、トップ・ログイン・サイトマップが 200 でした(2026-10-11)。ただし、このページの手順そのものは試していません。

## 6. 片付け

```powershell
docker rm -f mojo foc-sql
```
