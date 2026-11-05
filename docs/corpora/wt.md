# wt(WingtipToys)

おもちゃ屋のチュートリアルです(Web Forms、C#、SQL Server)。[共通の準備](setup.md) が終わっている前提で、コマンドは作業フォルダー(下の 0)で実行します。

| | |
|---|---|
| 取得元 | [corn-mendoza/wingtiptoys](https://github.com/corn-mendoza/wingtiptoys) コミット `4478e0e5` |
| ライセンス | 表示なし |
| 追加で要るもの | Docker Desktop(SQL Server をコンテナで動かします) |

## 0. FrameworkOnCore を取る

作業用の空のフォルダー(例: `C:\foc`。パスは短くします)を作って移り、そこに FrameworkOnCore を取ります。以降のコマンドは、すべてこのフォルダーで、ここからの相対パスで実行します。ほかのコーパスで済ませてあれば、この手順は飛ばします。

```powershell
New-Item -ItemType Directory -Force C:\foc | Out-Null
cd C:\foc
git clone https://github.com/Shkosugi-Lab/FrameworkOnCore.git
.\FrameworkOnCore\experiments\wf4c\casefs\build.ps1     # Docker Desktop を起動してから(Linux 用。一度だけ)
```

## 1. GitHub から取得する

```powershell
curl.exe -L -o wt.zip https://github.com/corn-mendoza/wingtiptoys/archive/4478e0e5794ed709da924569c54bb5f4c714fb85.zip
tar -xf wt.zip
Rename-Item wingtiptoys-4478e0e5794ed709da924569c54bb5f4c714fb85 wingtiptoys
```

## 2. 変換する

```powershell
.\FrameworkOnCore\converter.ps1 .\wingtiptoys\WingtipToys\WingtipToys\WingtipToys.csproj --out .\out\wt --root .\wingtiptoys --build-original auto
```

**Studio で入れる値**([使い方](setup.md#42-studio-で))

| 欄 | 値 |
|---|---|
| プロジェクト | `.\wingtiptoys\WingtipToys\WingtipToys\WingtipToys.csproj` |
| リポジトリのフォルダー | `.\wingtiptoys` |
| 構成 | `Release` |

## 3. DB を用意する

SQL Server のコンテナを起動します([共通の準備の 5](setup.md#5-sql-server-を使うとき))。DB `WingtipToys` とデータは、アプリが最初の要求で作ります。

```powershell
docker run -d --name foc-sql -p 14330:1433 -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=Foc_local_Passw0rd" mcr.microsoft.com/mssql/server:2022-latest
```

元の Web.config の接続文字列は、存在しない Azure SQL を指しています。2 つの接続文字列(`DefaultConnection`、`WingtipToys`)を、どちらも環境変数で置き換えます。

## 4. Windows で動かす

```powershell
Push-Location .\out\wt\WingtipToys\WingtipToys
$env:SQLCONNSTR_DefaultConnection = 'Data Source=localhost,14330;Initial Catalog=WingtipToys;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True;MultipleActiveResultSets=True'
$env:SQLCONNSTR_WingtipToys = $env:SQLCONNSTR_DefaultConnection
dotnet bin\WingtipToys.dll --urls http://localhost:5000
Pop-Location                  # Ctrl+C で止めたあと、作業フォルダーに戻る
```

http://localhost:5000/ を開きます。管理者は `canEditUser@wingtiptoys.com` / `Pa$$word1` です。

Studio では、「ネイティブ起動(この PC)」の「環境変数」に次の 2 行を書き、「起動」を押します。

```
SQLCONNSTR_DefaultConnection=Data Source=localhost,14330;Initial Catalog=WingtipToys;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True;MultipleActiveResultSets=True
SQLCONNSTR_WingtipToys=Data Source=localhost,14330;Initial Catalog=WingtipToys;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True;MultipleActiveResultSets=True
```

## 5. Linux で動かす

```powershell
docker build -t wingtiptoys .\out\wt
$cs = 'Data Source=host.docker.internal,14330;Initial Catalog=WingtipToys;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True;MultipleActiveResultSets=True'
docker run -d -p 8080:8080 --name wingtiptoys -e "SQLCONNSTR_DefaultConnection=$cs" -e "SQLCONNSTR_WingtipToys=$cs" wingtiptoys
```

http://localhost:8080/ を開きます。

Studio では、「コンテナ起動(Linux / Docker)」の「環境変数」に、上の 2 行のサーバー名を `host.docker.internal,14330` にしたものを書き、「起動」を押します。

## 6. 片付け

```powershell
docker rm -f wingtiptoys foc-sql
```
