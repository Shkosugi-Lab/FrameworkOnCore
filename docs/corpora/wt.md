# wt(WingtipToys)

おもちゃ屋のチュートリアルです(Web Forms、C#、SQL Server)。[共通の準備](setup.md) が終わっている前提です。

| | |
|---|---|
| 取得元 | [corn-mendoza/wingtiptoys](https://github.com/corn-mendoza/wingtiptoys) コミット `4478e0e5`(ZIP: <https://github.com/corn-mendoza/wingtiptoys/archive/4478e0e5794ed709da924569c54bb5f4c714fb85.zip>) |
| フォルダー | `corpora\work\wingtiptoys-master` |
| ライセンス | 表示なし |
| 追加で要るもの | なし(SQL Server はコンテナで動かします) |

## 1. 取得して変換する

```powershell
.\corpora\fetch.ps1 -Only wt
.\experiments\wf4c\convert-corpora.ps1 -Only wt
```

## 2. Linux で動かす

SQL Server もコンテナで動かします。

```powershell
docker network create foc-net
docker run -d --name foc-sql --network foc-net -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=Foc_local_Passw0rd" mcr.microsoft.com/mssql/server:2022-latest
cd experiments\wf4c\wt
docker build -t wingtiptoys .
$cs = 'Data Source=foc-sql;Initial Catalog=WingtipToys;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True;MultipleActiveResultSets=True'
docker run -d -p 8080:8080 --name wingtiptoys --network foc-net -e "SQLCONNSTR_DefaultConnection=$cs" -e "SQLCONNSTR_WingtipToys=$cs" wingtiptoys
```

http://localhost:8080/ を開きます。最初の要求で、アプリが DB を作ってデータを入れます。
管理者は `canEditUser@wingtiptoys.com` / `Pa$$word1` です。

## 3. 片付け

```powershell
docker rm -f wingtiptoys foc-sql
docker network rm foc-net
```
