# mvcmovie(MvcMovie)

映画の一覧を扱う ASP.NET MVC 5 のチュートリアルです(C#、EF6、SQL Server)。[共通の準備](setup.md) が終わっている前提で、コマンドは `C:\foc` で実行します。

| | |
|---|---|
| 取得元 | [dotnet/AspNetDocs](https://github.com/dotnet/AspNetDocs) コミット `552c58dd` の `aspnet/mvc/overview/getting-started/introduction/sample/MvcMovie` |
| ライセンス | コードは MIT、文書は CC BY 4.0 |
| 追加で要るもの | なし(SQL Server はコンテナで動かします) |

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
C:\src\FrameworkOnCore\converter.ps1 C:\foc\MvcMovie\MvcMovie\MvcMovie.csproj --out C:\foc\out\mvcmovie --root C:\foc\MvcMovie --build-original auto
```

## 3. Linux で動かす

```powershell
docker network create foc-net
docker run -d --name foc-sql --network foc-net -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=Foc_local_Passw0rd" mcr.microsoft.com/mssql/server:2022-latest
cd C:\foc\out\mvcmovie
docker build -t mvcmovie .
$sql = 'Data Source=foc-sql;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True'
docker run -d -p 8080:8080 --name mvcmovie --network foc-net -e "SQLCONNSTR_MovieDBContext=$sql;Initial Catalog=MvcMovie" -e "SQLCONNSTR_DefaultConnection=$sql;Initial Catalog=MvcMovieIdentity" mvcmovie
```

- http://localhost:8080/Movies を開きます。DB は、アプリ(EF)が最初の要求で作ります。
- 元の Web.config の接続文字列(LocalDB)は、環境変数で置き換えます。
- 記録では、Linux で 12 画面のうち 11 が元のアプリと同じでした(2026-10-22)。ただし、この Dockerfile での手順そのものは試していません。

## 4. 片付け

```powershell
docker rm -f mvcmovie foc-sql
docker network rm foc-net
```
