# imis(openIMIS 24.10)

健康保険の管理システムです(Web Forms、VB、SQL Server)。[共通の準備](setup.md) が終わっている前提です。

| | |
|---|---|
| 取得元 | アプリ: [openimis/web_app_vb](https://github.com/openimis/web_app_vb) コミット `f21970b3`<br>DB: [openimis/database_ms_sqlserver](https://github.com/openimis/database_ms_sqlserver) タグ `24.10` |
| フォルダー | `corpora\work\web_app_vb-main`、`corpora\work\database_ms_sqlserver-24.10` |
| ライセンス | AGPL-3.0 |
| 追加で要るもの | Build Tools、SQL Server Express と `sqlcmd`。ディスク約 2 GB |

## 1. GitHub から取得して変換する

元のアプリを構成 `DemoRelease` でビルドしてから変換します。

```powershell
cd corpora\work
curl.exe -L -o imis.zip https://github.com/openimis/web_app_vb/archive/f21970b3136d8083f2da6bb774652614c1ffd247.zip
tar -xf imis.zip
Rename-Item web_app_vb-f21970b3136d8083f2da6bb774652614c1ffd247 web_app_vb-main
curl.exe -L -o imisdb.zip https://github.com/openimis/database_ms_sqlserver/archive/refs/tags/24.10.zip
tar -xf imisdb.zip          # database_ms_sqlserver-24.10 ができる
cd ..\..
Start-Service 'MSSQL$SQLEXPRESS'                              # 管理者の PowerShell で
.\experiments\wf4c\convert-corpora.ps1 -Only imis
```

## 2. Linux で動かす

サイトと SQL Server をコンテナで動かします。DB `imis_linux` には、スキーマ、ストアド プロシージャ、デモのデータの順にスクリプトを流します。

```powershell
$sql = 'corpora\work\database_ms_sqlserver-24.10\sql'
$scripts = @(Get-ChildItem "$sql\base\*.sql") + @(Get-ChildItem "$sql\stored_procedures\*.sql") + @(Get-ChildItem "$sql\demo\*.sql") | ForEach-Object FullName
.\experiments\wf4c\run-linux-site.ps1 -Site imis\site -Dll IMIS -Database imis_linux -SqlScripts $scripts -Keep
.\experiments\wf4c\imis-login.ps1 -Base http://localhost:5098
```

- 確認: `imis-login.ps1` がログインし、主な画面 7 つがどれも 200 になります(記録 2026-10-11)。
- そのまま http://localhost:5098/ を開けます。ログインは `Admin` / `admin123` です。

## 3. 片付け

```powershell
docker rm -f w2l-site-imis w2l-sql
```
