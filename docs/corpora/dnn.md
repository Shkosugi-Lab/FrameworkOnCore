# dnn(DNN Platform 9.13.10)

大きな CMS です(Web Forms、C# と VB、SQL Server)。[共通の準備](setup.md) が終わっている前提で、コマンドは `C:\foc` で実行します。

| | |
|---|---|
| 取得元 | [dnnsoftware/Dnn.Platform](https://github.com/dnnsoftware/Dnn.Platform) タグ `v9.13.10` |
| ライセンス | MIT |
| 追加で要るもの | Build Tools。ディスク約 20 GB |

## 1. GitHub から取得する

```powershell
curl.exe -L -o dnn.zip https://github.com/dnnsoftware/Dnn.Platform/archive/refs/tags/v9.13.10.zip
tar -xf dnn.zip          # Dnn.Platform-9.13.10 ができる
```

## 2. 変換する

元のアプリを DNN の Cake のビルドでビルドしてから変換します(`--build-original auto` が判断します)。初回は長くかかります。

```powershell
C:\src\FrameworkOnCore\converter.ps1 "C:\foc\Dnn.Platform-9.13.10\DNN Platform\Website\DotNetNuke.Website.csproj" --out C:\foc\out\dnn --root C:\foc\Dnn.Platform-9.13.10 --build-original auto
```

`the build failed after it deployed the site: ... is used as it is` と出ても、そのまま進めて構いません。ビルドの一部(パッケージ作り)が失敗しただけで、サイトはできています。

## 3. Linux で動かす

空の DB を作っておき、接続文字列 `SiteSqlServer` を渡します。

```powershell
docker network create foc-net
docker run -d --name foc-sql --network foc-net -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=Foc_local_Passw0rd" mcr.microsoft.com/mssql/server:2022-latest
Start-Sleep 20
docker exec foc-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Foc_local_Passw0rd -C -Q "CREATE DATABASE dnn"
cd C:\foc\out\dnn
docker build -t dnn .
docker run -d -p 8080:8080 --name dnn --network foc-net -e "SQLCONNSTR_SiteSqlServer=Data Source=foc-sql;Initial Catalog=dnn;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True" dnn
```

http://localhost:8080/Install/Install.aspx?mode=install を開くと、インストールが始まります(数分かかります)。終わるとトップ(`/`)、`/Login`、`/Terms`、`/Privacy` が開けます。

記録では、Linux でインストールが終わり、これらのページが 200 でした(2026-10-11)。ただし、この Dockerfile での手順そのものは試していません。

## 4. 片付け

```powershell
docker rm -f dnn foc-sql
docker network rm foc-net
```
