# mojo(mojoPortal 3.1.6)

CMS です(Web Forms、C#、SQL Server)。[共通の準備](setup.md) が終わっている前提で、コマンドは `C:\foc` で実行します。

| | |
|---|---|
| 取得元 | [i7MEDIA/mojoportal](https://github.com/i7MEDIA/mojoportal) タグ `v3.1.6` |
| ライセンス | EPL-1.0 |
| 追加で要るもの | Build Tools。ディスク約 5 GB |

## 1. GitHub から取得する

```powershell
curl.exe -L -o mojo.zip https://github.com/i7MEDIA/mojoportal/archive/refs/tags/v3.1.6.zip
tar -xf mojo.zip          # mojoportal-3.1.6 ができる
```

## 2. 変換する

元のアプリをソリューションでビルドしてから変換します(`--build-original auto` が判断します)。

```powershell
C:\src\FrameworkOnCore\converter.ps1 C:\foc\mojoportal-3.1.6\Web\mojoPortal.Web.csproj --out C:\foc\out\mojo --root C:\foc\mojoportal-3.1.6 --build-original auto
```

## 3. Linux で動かす

空の DB を作っておき、接続文字列は appSettings の `MSSQLConnectionString` として渡します。

```powershell
docker network create foc-net
docker run -d --name foc-sql --network foc-net -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=Foc_local_Passw0rd" mcr.microsoft.com/mssql/server:2022-latest
Start-Sleep 20
docker exec foc-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Foc_local_Passw0rd -C -Q "CREATE DATABASE mojo"
cd C:\foc\out\mojo
docker build -t mojo .
docker run -d -p 8080:8080 --name mojo --network foc-net -e "APPSETTING_MSSQLConnectionString=Data Source=foc-sql;Initial Catalog=mojo;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True" mojo
```

http://localhost:8080/ を開くと、セットアップ(`/Setup/Default.aspx`)がテーブルを作ります。終わるとトップ(`Home - mojoPortal`)が開きます。

記録では、Linux でセットアップ → トップ・ログイン・サイトマップが 200 でした(2026-10-11)。ただし、この Dockerfile での手順そのものは試していません。

## 4. 片付け

```powershell
docker rm -f mojo foc-sql
docker network rm foc-net
```
