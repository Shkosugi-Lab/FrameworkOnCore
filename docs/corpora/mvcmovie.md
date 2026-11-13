# mvcmovie(MvcMovie)

映画の一覧を扱う ASP.NET MVC 5 のチュートリアルです(C#、EF6、SQL Server)。[共通の準備](setup.md) が終わっている前提で、コマンドは作業フォルダー(下の 0)で実行します。

| | |
|---|---|
| 取得元 | [dotnet/AspNetDocs](https://github.com/dotnet/AspNetDocs) コミット `552c58dd` の `aspnet/mvc/overview/getting-started/introduction/sample/MvcMovie` |
| ライセンス | コードは MIT、文書は CC BY 4.0 |
| 追加で要るもの | Docker Desktop(SQL Server をコンテナで動かします) |

## 0. FrameworkOnCore を取る

作業用の空のフォルダー(例: `C:\foc`。パスは短くします)を作って移り、そこに FrameworkOnCore を取ります。以降のコマンドは、すべてこのフォルダーで、ここからの相対パスで実行します。ほかのコーパスで済ませてあれば、この手順は飛ばします。

```powershell
New-Item -ItemType Directory -Force C:\foc | Out-Null
cd C:\foc
git clone https://github.com/Shkosugi-Lab/FrameworkOnCore.git
```

## 1. GitHub から取得する

大きなリポジトリなので、そのフォルダーだけを取ります。

```powershell
git clone --filter=blob:none --no-checkout --sparse https://github.com/dotnet/AspNetDocs.git AspNetDocs
git -C AspNetDocs sparse-checkout set aspnet/mvc/overview/getting-started/introduction/sample/MvcMovie
git -C AspNetDocs checkout 552c58dd8a809ef8cea02e66c9463ce9c7dcdfe5
Move-Item AspNetDocs\aspnet\mvc\overview\getting-started\introduction\sample\MvcMovie MvcMovie
Remove-Item AspNetDocs -Recurse -Force
```

## 2. 変換する

```powershell
.\FrameworkOnCore\converter.ps1 .\MvcMovie\MvcMovie\MvcMovie.csproj --out .\out\mvcmovie --root .\MvcMovie --build-original auto
```

**Studio で入れる値**([使い方](setup.md#42-studio-で))

| 欄 | 値 |
|---|---|
| プロジェクト | `.\MvcMovie\MvcMovie\MvcMovie.csproj` |
| リポジトリのフォルダー | `.\MvcMovie` |
| 構成 | `Release` |

## 3. DB を用意する

SQL Server のコンテナを起動します([共通の準備の 5](setup.md#5-sql-server-を使うとき))。DB は、アプリ(EF)が最初の要求で作ります。

```powershell
docker run -d --name foc-sql -p 14330:1433 -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=Foc_local_Passw0rd" mcr.microsoft.com/mssql/server:2022-latest
```

ARM の PC(Windows on ARM)では、SQL Server の代わりに Azure SQL Edge を使います([共通の準備の 5](setup.md#arm-の-pcwindows-on-armでは))。2 回目以降は `docker start foc-sql` で起動します。

元の Web.config の接続文字列(LocalDB)は、環境変数で置き換えます。

| 接続文字列 | DB |
|---|---|
| `MovieDBContext` | `MvcMovie` |
| `DefaultConnection` | `MvcMovieIdentity` |

## 4. Windows で動かす

```powershell
Push-Location .\out\mvcmovie\MvcMovie
$sql = 'Data Source=localhost,14330;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True'
$env:SQLCONNSTR_MovieDBContext = "$sql;Initial Catalog=MvcMovie"
$env:SQLCONNSTR_DefaultConnection = "$sql;Initial Catalog=MvcMovieIdentity"
dotnet bin\MvcMovie.dll --urls http://localhost:5000
Pop-Location                  # Ctrl+C で止めたあと、作業フォルダーに戻る
```

http://localhost:5000/Movies を開きます。Studio では、「ネイティブ起動(この PC)」の「環境変数」に、この 2 つを `名前=値` の形で 1 行ずつ書きます。

## 5. Linux で動かす

```powershell
docker build -t mvcmovie .\out\mvcmovie
$sql = 'Data Source=host.docker.internal,14330;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True'
docker run -d -p 8080:8080 --name mvcmovie -e "SQLCONNSTR_MovieDBContext=$sql;Initial Catalog=MvcMovie" -e "SQLCONNSTR_DefaultConnection=$sql;Initial Catalog=MvcMovieIdentity" mvcmovie
```

http://localhost:8080/Movies を開きます。Studio では「コンテナ起動(Linux / Docker)」の「環境変数」に、次の行を書きます(上のコマンドの `-e` のあとの部分です。コマンドは貼らず、引用符も付けません)。

```
SQLCONNSTR_MovieDBContext=Data Source=host.docker.internal,14330;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True;Initial Catalog=MvcMovie
SQLCONNSTR_DefaultConnection=Data Source=host.docker.internal,14330;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True;Initial Catalog=MvcMovieIdentity
```

記録では、Windows・Linux とも 12 画面のうち 11 が元のアプリと同じでした(2026-10-22・10-27)。ただし、このページの手順そのものは試していません。

## 6. 片付け

```powershell
docker rm -f mvcmovie foc-sql
```
