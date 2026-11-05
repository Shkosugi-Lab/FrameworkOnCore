# wt(WingtipToys)

おもちゃ屋のチュートリアルです(Web Forms、C#、SQL Server)。[共通の準備](setup.md) が終わっている前提で、コマンドは `C:\foc` で実行します。

| | |
|---|---|
| 取得元 | [corn-mendoza/wingtiptoys](https://github.com/corn-mendoza/wingtiptoys) コミット `4478e0e5` |
| ライセンス | 表示なし |
| 追加で要るもの | なし(SQL Server はコンテナで動かします) |

## 1. GitHub から取得する

```powershell
curl.exe -L -o wt.zip https://github.com/corn-mendoza/wingtiptoys/archive/4478e0e5794ed709da924569c54bb5f4c714fb85.zip
tar -xf wt.zip
Rename-Item wingtiptoys-4478e0e5794ed709da924569c54bb5f4c714fb85 wingtiptoys
```

## 2. 変換する

```powershell
C:\src\FrameworkOnCore\converter.ps1 C:\foc\wingtiptoys\WingtipToys\WingtipToys\WingtipToys.csproj --out C:\foc\out\wt --root C:\foc\wingtiptoys --build-original auto
```

## 3. Linux で動かす

SQL Server もコンテナで動かします(共通の準備の [6](setup.md#6-linux-で動かす) の `foc-net`・`foc-sql`)。

```powershell
docker network create foc-net
docker run -d --name foc-sql --network foc-net -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=Foc_local_Passw0rd" mcr.microsoft.com/mssql/server:2022-latest
cd C:\foc\out\wt
docker build -t wingtiptoys .
$cs = 'Data Source=foc-sql;Initial Catalog=WingtipToys;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True;MultipleActiveResultSets=True'
docker run -d -p 8080:8080 --name wingtiptoys --network foc-net -e "SQLCONNSTR_DefaultConnection=$cs" -e "SQLCONNSTR_WingtipToys=$cs" wingtiptoys
```

- 元の Web.config の接続文字列(存在しない Azure SQL)は、環境変数 `SQLCONNSTR_<名前>` で置き換えます。
- http://localhost:8080/ を開きます。最初の要求で、アプリが DB を作ってデータを入れます。
- 管理者は `canEditUser@wingtiptoys.com` / `Pa$$word1` です。

## 4. 片付け

```powershell
docker rm -f wingtiptoys foc-sql
docker network rm foc-net
```
