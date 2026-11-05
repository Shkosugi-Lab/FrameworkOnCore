# be(BlogEngine.NET 3.3.8)

ブログのエンジンです(Web Forms、C#)。データは DB ではなく `App_Data` の XML に置きます。[共通の準備](setup.md) が終わっている前提で、コマンドは作業フォルダー(下の 0)で実行します。

| | |
|---|---|
| 取得元 | [rxtur/BlogEngine.NET](https://github.com/rxtur/BlogEngine.NET) タグ `v3.3.8.0` |
| ライセンス | 表示なし |
| 追加で要るもの | なし |

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
curl.exe -L -o be.zip https://github.com/rxtur/BlogEngine.NET/archive/refs/tags/v3.3.8.0.zip
tar -xf be.zip          # BlogEngine.NET-3.3.8.0 ができる
```

## 2. 変換する

```powershell
.\FrameworkOnCore\converter.ps1 .\BlogEngine.NET-3.3.8.0\BlogEngine\BlogEngine.NET\BlogEngine.NET.csproj --out .\out\be --root .\BlogEngine.NET-3.3.8.0 --build-original auto
```

- 約 1 分かかります。元のアプリのビルドは要りません(ソースに入っている `bin` をそのまま使います)。
- `--root` は省けません。このアプリはソリューションの外の `lib` の DLL を参照しているためで、付けないと参照が見つからず、変換レポートに「未解決」が 16 件出ます。

**Studio で入れる値**([使い方](setup.md#42-studio-で))

| 欄 | 値 |
|---|---|
| プロジェクト | `.\BlogEngine.NET-3.3.8.0\BlogEngine\BlogEngine.NET\BlogEngine.NET.csproj` |
| リポジトリのフォルダー | `.\BlogEngine.NET-3.3.8.0`(必ず入れます) |
| 構成 | `Release` |

## 3. Windows で動かす

```powershell
Push-Location .\out\be\BlogEngine\BlogEngine.NET
dotnet bin\BlogEngine.NET.dll --urls http://localhost:5000
Pop-Location                  # Ctrl+C で止めたあと、作業フォルダーに戻る
```

http://localhost:5000/ を開きます。最初の表示は、ページのコンパイルで 10 秒ほどかかります。管理者は `admin` / `admin` です(ログインは `/Account/login.aspx`)。

Studio では「ネイティブ起動(この PC)」の「起動」を押します。環境変数は要りません。

## 4. Linux で動かす

```powershell
docker build -t blogengine-net .\out\be
docker run -d -p 8080:8080 --name blogengine-net -v blogengine-net-data:/app/App_Data blogengine-net
```

http://localhost:8080/ を開きます。`App_Data` はボリューム `blogengine-net-data` に置かれます。

Studio では「コンテナ起動(Linux / Docker)」の「起動」を押します。

## 5. 片付け

```powershell
docker rm -f blogengine-net
docker volume rm blogengine-net-data
```
