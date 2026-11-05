# contoso(Contoso University)

学生・講座を扱う EF6 のチュートリアルです(ASP.NET MVC 5、C#、SQL Server)。[共通の準備](setup.md) が終わっている前提で、コマンドは `C:\foc` で実行します。

| | |
|---|---|
| 取得元 | [tigerbluejay/ContosoUniversity](https://github.com/tigerbluejay/ContosoUniversity) コミット `2ee1ad2a` の `ContosoUniversitySolution` |
| ライセンス | 表示なし(手元で試すだけにし、配り直さないでください) |
| 追加で要るもの | なし(SQL Server はコンテナで動かします) |

## 1. GitHub から取得する

ソリューションのフォルダーだけを取ります。

```powershell
git clone --filter=blob:none --no-checkout --sparse https://github.com/tigerbluejay/ContosoUniversity.git ContosoUniversity-repo
git -C ContosoUniversity-repo sparse-checkout set ContosoUniversitySolution
git -C ContosoUniversity-repo checkout 2ee1ad2aa66fbef012fe280e4552176489dcf0f3
Move-Item ContosoUniversity-repo\ContosoUniversitySolution ContosoUniversity
Remove-Item ContosoUniversity-repo -Recurse -Force
```

## 2. 変換する

```powershell
C:\src\FrameworkOnCore\converter.ps1 C:\foc\ContosoUniversity\ContosoUniversity\ContosoUniversity.csproj --out C:\foc\out\contoso --root C:\foc\ContosoUniversity --build-original auto
```

## 3. Linux で動かす

```powershell
docker network create foc-net
docker run -d --name foc-sql --network foc-net -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=Foc_local_Passw0rd" mcr.microsoft.com/mssql/server:2022-latest
cd C:\foc\out\contoso
docker build -t contoso .
docker run -d -p 8080:8080 --name contoso --network foc-net -e "SQLCONNSTR_SchoolContext=Data Source=foc-sql;Initial Catalog=ContosoUniversity1;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True" contoso
```

- http://localhost:8080/Student を開きます。DB は、アプリが最初の要求で初期データ付きで作ります。
- 元の Web.config の接続文字列(LocalDB)は、環境変数で置き換えます。
- 記録では、Linux で 12 画面のうち 9 が元のアプリと同じでした(2026-10-22)。ただし、この Dockerfile での手順そのものは試していません。

## 4. 片付け

```powershell
docker rm -f contoso foc-sql
docker network rm foc-net
```
