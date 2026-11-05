# imis(openIMIS 24.10)

健康保険の管理システムです(Web Forms、VB、SQL Server)。[共通の準備](setup.md) が終わっている前提で、コマンドは `C:\foc` で実行します。

| | |
|---|---|
| 取得元 | アプリ: [openimis/web_app_vb](https://github.com/openimis/web_app_vb) コミット `f21970b3`<br>DB: [openimis/database_ms_sqlserver](https://github.com/openimis/database_ms_sqlserver) タグ `24.10` |
| ライセンス | AGPL-3.0 |
| 追加で要るもの | Build Tools。ディスク約 2 GB |

## 1. GitHub から取得する

アプリと、DB のスクリプトの 2 つを取ります。

```powershell
curl.exe -L -o imis.zip https://github.com/openimis/web_app_vb/archive/f21970b3136d8083f2da6bb774652614c1ffd247.zip
tar -xf imis.zip
Rename-Item web_app_vb-f21970b3136d8083f2da6bb774652614c1ffd247 web_app_vb
curl.exe -L -o imisdb.zip https://github.com/openimis/database_ms_sqlserver/archive/refs/tags/24.10.zip
tar -xf imisdb.zip          # database_ms_sqlserver-24.10 ができる
```

## 2. 変換する

元のアプリを、構成 `DemoRelease` でビルドしてから変換します(`--build-original auto` が判断します)。

```powershell
C:\src\FrameworkOnCore\converter.ps1 C:\foc\web_app_vb\IMIS\IMIS.vbproj --out C:\foc\out\imis --root C:\foc\web_app_vb --build-original auto --configuration DemoRelease
```

## 3. Linux で動かす

DB は、スキーマ、ストアド プロシージャ、デモのデータの順にスクリプトを流して作ります(数分かかります)。

```powershell
docker network create foc-net
docker run -d --name foc-sql --network foc-net -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=Foc_local_Passw0rd" mcr.microsoft.com/mssql/server:2022-latest
Start-Sleep 20
$sqlcmd = '/opt/mssql-tools18/bin/sqlcmd', '-S', 'localhost', '-U', 'sa', '-P', 'Foc_local_Passw0rd', '-C'
docker exec foc-sql @sqlcmd -Q "CREATE DATABASE imis"
$sql = 'C:\foc\database_ms_sqlserver-24.10\sql'
foreach ($f in @(Get-ChildItem "$sql\base\*.sql") + @(Get-ChildItem "$sql\stored_procedures\*.sql") + @(Get-ChildItem "$sql\demo\*.sql")) {
    docker cp $f.FullName foc-sql:/tmp/script.sql
    docker exec foc-sql @sqlcmd -d imis -f 65001 -i /tmp/script.sql | Out-Null
}
cd C:\foc\out\imis
docker build -t imis .
docker run -d -p 8080:8080 --name imis --network foc-net -e "SQLCONNSTR_IMISConnectionString=Data Source=foc-sql;Initial Catalog=imis;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True" imis
```

http://localhost:8080/ を開き、デモの `Admin` / `admin123` でログインします。主な画面は `/Home.aspx`、`/FindFamily.aspx`、`/FindInsuree.aspx`、`/FindPolicy.aspx`、`/FindClaims.aspx`、`/FindUser.aspx`、`/Reports.aspx` です。

記録では、Linux でログインし、この 7 画面が 200 でした(2026-10-11)。ただし、この Dockerfile での手順そのものは試していません。

## 4. 片付け

```powershell
docker rm -f imis foc-sql
docker network rm foc-net
```
