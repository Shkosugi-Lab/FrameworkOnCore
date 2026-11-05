# contoso(Contoso University)

学生・講座を扱う EF6 のチュートリアルです(ASP.NET MVC 5、C#、SQL Server)。[共通の準備](setup.md) が終わっている前提で、コマンドは作業フォルダー(下の 0)で実行します。

| | |
|---|---|
| 取得元 | [tigerbluejay/ContosoUniversity](https://github.com/tigerbluejay/ContosoUniversity) コミット `2ee1ad2a` の `ContosoUniversitySolution` |
| ライセンス | 表示なし(手元で試すだけにし、配り直さないでください) |
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
.\FrameworkOnCore\converter.ps1 .\ContosoUniversity\ContosoUniversity\ContosoUniversity.csproj --out .\out\contoso --root .\ContosoUniversity --build-original auto
```

**Studio で入れる値**([使い方](setup.md#42-studio-で))

| 欄 | 値 |
|---|---|
| プロジェクト | `.\ContosoUniversity\ContosoUniversity\ContosoUniversity.csproj` |
| リポジトリのフォルダー | `.\ContosoUniversity` |
| 構成 | `Release` |

## 3. DB を用意する

SQL Server のコンテナを起動します([共通の準備の 5](setup.md#5-sql-server-を使うとき))。DB は、アプリが最初の要求で初期データ付きで作ります。

```powershell
docker run -d --name foc-sql -p 14330:1433 -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=Foc_local_Passw0rd" mcr.microsoft.com/mssql/server:2022-latest
```

元の Web.config の接続文字列 `SchoolContext`(LocalDB)は、環境変数で置き換えます。

## 4. Windows で動かす

```powershell
Push-Location .\out\contoso\ContosoUniversity
$env:SQLCONNSTR_SchoolContext = 'Data Source=localhost,14330;Initial Catalog=ContosoUniversity1;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True'
dotnet bin\ContosoUniversity.dll --urls http://localhost:5000
Pop-Location                  # Ctrl+C で止めたあと、作業フォルダーに戻る
```

http://localhost:5000/Student を開きます。Studio では、「ネイティブ起動(この PC)」の「環境変数」に `SQLCONNSTR_SchoolContext=<上の値>` と書きます。

## 5. Linux で動かす

```powershell
docker build -t contoso .\out\contoso
docker run -d -p 8080:8080 --name contoso -e "SQLCONNSTR_SchoolContext=Data Source=host.docker.internal,14330;Initial Catalog=ContosoUniversity1;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True" contoso
```

http://localhost:8080/Student を開きます。Studio では「コンテナ起動(Linux / Docker)」の「環境変数」に、サーバー名を `host.docker.internal,14330` にして書きます。

記録では、Windows・Linux とも 12 画面のうち 9 が元のアプリと同じでした(2026-10-22・10-27)。ただし、このページの手順そのものは試していません。

## 6. 片付け

```powershell
docker rm -f contoso foc-sql
```
