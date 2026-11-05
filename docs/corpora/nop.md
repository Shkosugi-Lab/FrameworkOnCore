# nop(nopCommerce 1.90)

EC サイトです(Web Forms、C#、SQL Server)。[共通の準備](setup.md) が終わっている前提で、コマンドは `C:\foc` で実行します。

| | |
|---|---|
| 取得元 | [nopSolutions/nopCommerce](https://github.com/nopSolutions/nopCommerce) タグ `release-1.90` |
| ライセンス | nopCommerce Public License 1.0 |
| 追加で要るもの | Build Tools。ディスク約 2 GB |

## 1. GitHub から取得する

```powershell
curl.exe -L -o nop.zip https://github.com/nopSolutions/nopCommerce/archive/refs/tags/release-1.90.zip
tar -xf nop.zip          # nopCommerce-release-1.90 ができる
```

## 2. 変換する

元のアプリをソリューションでビルドしてから変換します(`--build-original auto` が判断します)。

```powershell
C:\src\FrameworkOnCore\converter.ps1 C:\foc\nopCommerce-release-1.90\NopCommerceStore\NopCommerceStore.csproj --out C:\foc\out\nop --root C:\foc\nopCommerce-release-1.90 --build-original auto
```

## 3. Linux で動かす

```powershell
docker network create foc-net
docker run -d --name foc-sql --network foc-net -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=Foc_local_Passw0rd" mcr.microsoft.com/mssql/server:2022-latest
cd C:\foc\out\nop
docker build -t nop .
docker run -d -p 8080:8080 --name nop --network foc-net nop
```

http://localhost:8080/install/install.aspx を開き、次の値でインストールします。

| 欄 | 値 |
|---|---|
| SQL Server | `foc-sql` |
| 認証 | SQL Server 認証。ユーザー `sa`、パスワード `Foc_local_Passw0rd` |
| データベース | 新しく作る(名前は例えば `nop`)。サンプルのデータも作る |

インストールが終わると、店頭(`/`)と管理画面が開けます。管理者は `admin@yourstore.com` / `admin` です。

Linux で動くことは、すべての操作を元のアプリと比べるテストで確かめています(727 操作、記録 2026-11-03)。ただし、この Dockerfile とインストーラーでの手順そのものは試していません。

## 4. 片付け

```powershell
docker rm -f nop foc-sql
docker network rm foc-net
```
