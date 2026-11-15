# nopCommerce 3.90 をコマンドラインで変換する

EC サイトです(ASP.NET MVC 5、C#、SQL Server)。[共通の準備](setup.md) が終わっている前提で、コマンドは作業フォルダー(下の 0)で実行します。Studio(画面)での手順は [Studio 版](studio/nopcommerce-3.90.md) です。

| | |
|---|---|
| 取得元 | [nopSolutions/nopCommerce](https://github.com/nopSolutions/nopCommerce) タグ `release-3.90` |
| ライセンス | nopCommerce Public License 3.0 |
| 追加で要るもの | Visual Studio か Build Tools、Docker Desktop(SQL Server)。ディスク約 3 GB |

## 0. FrameworkOnCore を取る

作業用の空のフォルダー(例: `C:\foc`。パスは短くします)を作って移り、そこに FrameworkOnCore を取ります。以降のコマンドは、すべてこのフォルダーで、ここからの相対パスで実行します。ほかのコーパスで済ませてあれば、この手順は飛ばします。

```powershell
New-Item -ItemType Directory -Force C:\foc | Out-Null
cd C:\foc
git clone https://github.com/Shkosugi-Lab/FrameworkOnCore.git
```

## 1. GitHub から取得する

```powershell
curl.exe -L -o nopcommerce-3.90.zip https://github.com/nopSolutions/nopCommerce/archive/refs/tags/release-3.90.zip
tar -xf nopcommerce-3.90.zip          # nopCommerce-release-3.90 ができる
```

## 2. 変換する

元のアプリをソリューションでビルドしてから変換します(`--build-original auto` が判断します)。

```powershell
.\FrameworkOnCore\converter.ps1 .\nopCommerce-release-3.90\src\Presentation\Nop.Web\Nop.Web.csproj --out .\out\nopcommerce-3.90 --root .\nopCommerce-release-3.90 --build-original auto
```

## 3. DB を用意する

SQL Server のコンテナを起動します([共通の準備の 5](setup.md#5-sql-server-を使うとき))。DB は、アプリのインストールの画面が作ります。

```powershell
docker run -d --name foc-sql -p 14330:1433 -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=Foc_local_Passw0rd" mcr.microsoft.com/mssql/server:2022-latest
```

ARM の PC(Windows on ARM)では、SQL Server の代わりに Azure SQL Edge を使います([共通の準備の 5](setup.md#arm-の-pcwindows-on-armでは))。2 回目以降は `docker start foc-sql` で起動します。

## 4. Windows で動かす

インストールでサイトの `App_Data` が書き換わります。Linux でも試すときは、先に 5 の `docker build` を済ませておきます([共通の準備の 7](setup.md#7-linux-で動かす))。

```powershell
Push-Location .\out\nopcommerce-3.90\site
dotnet bin\Nop.Web.dll --urls http://localhost:5000
Pop-Location                  # Ctrl+C で止めたあと、作業フォルダーに戻る
```

http://localhost:5000/ を開くと、インストールの画面が出ます。次の値でインストールします。

| 欄 | 値 |
|---|---|
| SQL Server | `localhost,14330`(Linux のコンテナでは `host.docker.internal,14330`) |
| 認証 | SQL Server 認証。ユーザー `sa`、パスワード `Foc_local_Passw0rd` |
| データベース | Windows と Linux で別の名前にします(`nopcommerce390_windows`、`nopcommerce390_linux` など) |
| サンプルのデータ | 入れる |

- インストールが終わると、アプリが再起動し、店頭と管理画面が開けます。
- 管理画面には、インストールの画面で入れたメールとパスワードでログインします。

## 5. Linux で動かす

```powershell
docker build -t nopcommerce-3.90 .\out\nopcommerce-3.90
docker run -d -p 8080:8080 --name nopcommerce-3.90 nopcommerce-3.90
```

http://localhost:8080/ を開き、上の表の Linux の値でインストールします。

記録では、Windows・Linux とも店頭 19 画面、カートへの追加、管理画面 7 画面がどれも 200 でした(2026-10-11)。ただし、このページの手順そのものは試していません。

## 6. 片付け

```powershell
docker rm -f nopcommerce-3.90 foc-sql
```
