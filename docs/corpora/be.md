# be(BlogEngine.NET 3.3.8)

ブログのエンジンです(Web Forms、C#)。データは DB ではなく `App_Data` の XML に置きます。[共通の準備](setup.md) が終わっている前提で、コマンドは `C:\foc` で実行します。

| | |
|---|---|
| 取得元 | [rxtur/BlogEngine.NET](https://github.com/rxtur/BlogEngine.NET) タグ `v3.3.8.0` |
| ライセンス | 表示なし |
| 追加で要るもの | なし |

## 1. GitHub から取得する

```powershell
curl.exe -L -o be.zip https://github.com/rxtur/BlogEngine.NET/archive/refs/tags/v3.3.8.0.zip
tar -xf be.zip          # BlogEngine.NET-3.3.8.0 ができる
```

## 2. 変換する

```powershell
C:\src\FrameworkOnCore\converter.ps1 C:\foc\BlogEngine.NET-3.3.8.0\BlogEngine\BlogEngine.NET\BlogEngine.NET.csproj --out C:\foc\out\be --root C:\foc\BlogEngine.NET-3.3.8.0 --build-original auto
```

- 約 1 分かかります。元のアプリのビルドは要りません(ソースに入っている `bin` をそのまま使います)。
- `--root` は省けません。このアプリはソリューションの外の `lib` の DLL を参照しているためで、付けないと参照が見つからず、変換レポートに「未解決」が 16 件出ます。

## 3. Linux で動かす

```powershell
cd C:\foc\out\be
docker build -t blogengine-net .
docker run -d -p 8080:8080 --name blogengine-net -v blogengine-net-data:/app/App_Data blogengine-net
```

http://localhost:8080/ を開きます。管理者は `admin` / `admin` です(ログインは `/Account/login.aspx`)。

## 4. 片付け

```powershell
docker rm -f blogengine-net
docker volume rm blogengine-net-data
```
