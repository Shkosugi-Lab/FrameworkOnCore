# be(BlogEngine.NET 3.3.8)をコマンドラインで変換する

ブログのエンジンです(Web Forms、C#)。データは DB ではなく `App_Data` の XML に置きます。[共通の準備](setup.md) が終わっている前提で、コマンドは作業フォルダー(下の 0)で実行します。Studio(画面)での手順は [こちら](studio/be.md) です。

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

```powershell
docker rm -f blogengine-net
docker volume rm blogengine-net-data
```
