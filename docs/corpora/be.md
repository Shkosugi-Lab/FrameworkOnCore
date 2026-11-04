# be(BlogEngine.NET 3.3.8)

ブログのエンジンです(Web Forms、C#)。データは DB ではなく `App_Data` の XML に置きます。[共通の準備](setup.md) が終わっている前提です。

| | |
|---|---|
| 取得元 | [rxtur/BlogEngine.NET](https://github.com/rxtur/BlogEngine.NET) タグ `v3.3.8.0`(ZIP: <https://github.com/rxtur/BlogEngine.NET/archive/refs/tags/v3.3.8.0.zip>) |
| フォルダー | `corpora\work\BlogEngine.NET-3.3.8.0` |
| ライセンス | 表示なし |
| 追加で要るもの | なし |

## 1. 取得して変換する

```powershell
.\corpora\fetch.ps1 -Only be
.\experiments\wf4c\convert-corpora.ps1 -Only be        # 約 1 分
```

Studio で変換するときは、「リポジトリのフォルダー」を必ず指定します。このアプリはソリューションの外にある DLL を参照しているためで、指定しないと参照が見つからない「未解決」が 16 件出ます。

## 2. Linux で動かす

```powershell
cd experiments\wf4c\be
docker build -t blogengine-net .
docker run -d -p 8080:8080 --name blogengine-net -v blogengine-net-data:/app/App_Data blogengine-net
```

http://localhost:8080/ を開きます。管理者は `admin` / `admin` です(ログインは `/Account/login.aspx`)。

## 3. 片付け

```powershell
docker rm -f blogengine-net
docker volume rm blogengine-net-data
```
