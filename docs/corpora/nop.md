# nop(nopCommerce 1.90)

EC サイトです(Web Forms、C#、SQL Server)。[共通の準備](setup.md) が終わっている前提で、コマンドは作業フォルダー(下の 0)で実行します。

| | |
|---|---|
| 取得元 | [nopSolutions/nopCommerce](https://github.com/nopSolutions/nopCommerce) タグ `release-1.90` |
| ライセンス | nopCommerce Public License 1.0 |
| 追加で要るもの | Visual Studio か Build Tools、Docker Desktop(SQL Server)。ディスク約 2 GB |

## 0. FrameworkOnCore を取る

作業用の空のフォルダー(例: `C:\foc`。パスは短くします)を作って移り、そこに FrameworkOnCore を取ります。以降のコマンドは、すべてこのフォルダーで、ここからの相対パスで実行します。ほかのコーパスで済ませてあれば、この手順は飛ばします。

```powershell
New-Item -ItemType Directory -Force C:\foc | Out-Null
cd C:\foc
git clone https://github.com/Shkosugi-Lab/FrameworkOnCore.git
```

## 1. GitHub から取得する

```powershell
curl.exe -L -o nop.zip https://github.com/nopSolutions/nopCommerce/archive/refs/tags/release-1.90.zip
tar -xf nop.zip          # nopCommerce-release-1.90 ができる
```

## 2. 変換する

元のアプリをソリューションでビルドしてから変換します(`--build-original auto` が判断します)。

```powershell
.\FrameworkOnCore\converter.ps1 .\nopCommerce-release-1.90\NopCommerceStore\NopCommerceStore.csproj --out .\out\nop --root .\nopCommerce-release-1.90 --build-original auto
```

**Studio で入れる値**([使い方](setup.md#42-studio-で))

| 欄 | 値 |
|---|---|
| プロジェクト | `.\nopCommerce-release-1.90\NopCommerceStore\NopCommerceStore.csproj` |
| リポジトリのフォルダー | `.\nopCommerce-release-1.90` |
| 構成 | `Release` |

## 3. DB を用意する

SQL Server のコンテナを起動します([共通の準備の 5](setup.md#5-sql-server-を使うとき))。DB は、アプリのインストーラーが作ります。

```powershell
docker run -d --name foc-sql -p 14330:1433 -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=Foc_local_Passw0rd" mcr.microsoft.com/mssql/server:2022-latest
```

ARM の PC(Windows on ARM)では、SQL Server の代わりに Azure SQL Edge を使います([共通の準備の 5](setup.md#arm-の-pcwindows-on-armでは))。2 回目以降は `docker start foc-sql` で起動します。

## 4. Windows で動かす

インストーラーがサイトの `ConnectionStrings.config` を書きます。Linux でも試すときは、先に 5 の `docker build` を済ませておきます([共通の準備の 7](setup.md#7-linux-で動かす))。

```powershell
Push-Location .\out\nop\site
dotnet bin\NopCommerceStore.dll --urls http://localhost:5000
Pop-Location                  # Ctrl+C で止めたあと、作業フォルダーに戻る
```

http://localhost:5000/install/install.aspx を開き、次の値でインストールします。

| 欄 | 値 |
|---|---|
| SQL Server | `localhost,14330`(Linux のコンテナでは `host.docker.internal,14330`) |
| 認証 | SQL Server 認証。ユーザー `sa`、パスワード `Foc_local_Passw0rd` |
| データベース | 新しく作る(Windows は `nop_windows`、Linux は `nop_linux` など、別の名前にします)。サンプルのデータも作る |

- インストールが終わると、店頭(`/`)と管理画面が開けます。管理者は `admin@yourstore.com` / `admin` です。
- Studio では「ネイティブ起動(この PC)」の「起動」を押します。環境変数は要りません。

## 5. Linux で動かす

```powershell
docker build -t nop .\out\nop
docker run -d -p 8080:8080 --name nop nop
```

http://localhost:8080/install/install.aspx を開き、上の表の Linux の値でインストールします。Studio では「コンテナ起動(Linux / Docker)」の「起動」を押します。

Windows・Linux で動くことは、すべての操作を元のアプリと比べるテストで確かめています(727 操作、記録 2026-11-03)。ただし、このページの手順(インストーラーを使う)そのものは試していません。

## 6. 片付け

```powershell
docker rm -f nop foc-sql
```
