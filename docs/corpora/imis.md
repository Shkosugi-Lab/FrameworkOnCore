# imis(openIMIS 24.10)

健康保険の管理システムです(Web Forms、VB、SQL Server)。[共通の準備](setup.md) が終わっている前提で、コマンドは作業フォルダー(下の 0)で実行します。

| | |
|---|---|
| 取得元 | アプリ: [openimis/web_app_vb](https://github.com/openimis/web_app_vb) コミット `f21970b3`<br>DB: [openimis/database_ms_sqlserver](https://github.com/openimis/database_ms_sqlserver) タグ `24.10` |
| ライセンス | AGPL-3.0 |
| 追加で要るもの | Visual Studio か Build Tools、Docker Desktop(SQL Server)。ディスク約 2 GB |

## 0. FrameworkOnCore を取る

作業用の空のフォルダー(例: `C:\foc`。パスは短くします)を作って移り、そこに FrameworkOnCore を取ります。以降のコマンドは、すべてこのフォルダーで、ここからの相対パスで実行します。ほかのコーパスで済ませてあれば、この手順は飛ばします。

```powershell
New-Item -ItemType Directory -Force C:\foc | Out-Null
cd C:\foc
git clone https://github.com/Shkosugi-Lab/FrameworkOnCore.git
```

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
.\FrameworkOnCore\converter.ps1 .\web_app_vb\IMIS\IMIS.vbproj --out .\out\imis --root .\web_app_vb --build-original auto --configuration DemoRelease
```

**Studio で入れる値**([使い方](setup.md#42-studio-で))

| 欄 | 値 |
|---|---|
| プロジェクト | `.\web_app_vb\IMIS\IMIS.vbproj` |
| リポジトリのフォルダー | `.\web_app_vb` |
| 構成 | **`DemoRelease`** |

## 3. DB を用意する

SQL Server のコンテナを起動し([共通の準備の 5](setup.md#5-sql-server-を使うとき))、DB `imis` を作ります。スキーマ、ストアド プロシージャ、デモのデータの順にスクリプトを流します(数分かかります)。Windows と Linux で同じ DB を使えます。

```powershell
docker run -d --name foc-sql -p 14330:1433 -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=Foc_local_Passw0rd" mcr.microsoft.com/mssql/server:2022-latest
Start-Sleep 20
$sqlcmd = '/opt/mssql-tools18/bin/sqlcmd', '-S', 'localhost', '-U', 'sa', '-P', 'Foc_local_Passw0rd', '-C'
docker exec foc-sql @sqlcmd -Q "CREATE DATABASE imis"
$sql = '.\database_ms_sqlserver-24.10\sql'
foreach ($f in @(Get-ChildItem "$sql\base\*.sql") + @(Get-ChildItem "$sql\stored_procedures\*.sql") + @(Get-ChildItem "$sql\demo\*.sql")) {
    docker cp $f.FullName foc-sql:/tmp/script.sql
    docker exec foc-sql @sqlcmd -d imis -f 65001 -i /tmp/script.sql | Out-Null
}
```

接続文字列は `IMISConnectionString` です。

## 4. Windows で動かす

```powershell
Push-Location .\out\imis\site
$env:SQLCONNSTR_IMISConnectionString = 'Data Source=localhost,14330;Initial Catalog=imis;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True'
dotnet bin\IMIS.dll --urls http://localhost:5000
Pop-Location                  # Ctrl+C で止めたあと、作業フォルダーに戻る
```

- http://localhost:5000/ を開き、デモの `Admin` / `admin123` でログインします。
- 主な画面は `/Home.aspx`、`/FindFamily.aspx`、`/FindInsuree.aspx`、`/FindPolicy.aspx`、`/FindClaims.aspx`、`/FindUser.aspx`、`/Reports.aspx` です。
- Studio では、「ネイティブ起動(この PC)」の「環境変数」に `SQLCONNSTR_IMISConnectionString=<上の値>` と書きます。

## 5. Linux で動かす

```powershell
docker build -t imis .\out\imis
docker run -d -p 8080:8080 --name imis -e "SQLCONNSTR_IMISConnectionString=Data Source=host.docker.internal,14330;Initial Catalog=imis;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True" imis
```

http://localhost:8080/ を開きます。Studio では「コンテナ起動(Linux / Docker)」の「環境変数」に、この値を書きます。

記録では、Windows・Linux ともログインし、主な 7 画面が 200 でした(2026-10-11)。ただし、このページの手順そのものは試していません。

## 6. 片付け

```powershell
docker rm -f imis foc-sql
```
