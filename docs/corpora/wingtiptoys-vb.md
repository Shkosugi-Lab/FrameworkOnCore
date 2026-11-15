# WingtipToys(VB) をコマンドラインで変換する

おもちゃ屋のチュートリアルの **Visual Basic** 版です(Web Forms、VB、SQL Server)。Microsoft の「ASP.NET 4.5 Web Forms Tutorial Series in Visual Basic」のサンプルで、C# の [WingtipToys](wingtiptoys.md) と同じアプリです。[共通の準備](setup.md) が終わっている前提で、コマンドは作業フォルダー(下の 0)で実行します。Studio(画面)での手順は [Studio 版](studio/wingtiptoys-vb.md) です。

| | |
|---|---|
| 取得元 | [microsoftarchive/msdn-code-gallery-community-a-c](https://github.com/microsoftarchive/msdn-code-gallery-community-a-c) の「ASP.NET 4.5 Web Forms Tutorial Series in Visual Basic」 コミット `bbfca35d` |
| ライセンス | Apache License 2.0 |
| 追加で要るもの | Docker Desktop(SQL Server をコンテナで動かします) |

## 0. FrameworkOnCore を取る

作業用の空のフォルダー(例: `C:\foc`。パスは短くします)を作って移り、そこに FrameworkOnCore を取ります。以降のコマンドは、すべてこのフォルダーで、ここからの相対パスで実行します。ほかのコーパスで済ませてあれば、この手順は飛ばします。

```powershell
New-Item -ItemType Directory -Force C:\foc | Out-Null
cd C:\foc
git clone https://github.com/Shkosugi-Lab/FrameworkOnCore.git
```

## 1. GitHub から取得する

取得元は多くのサンプルを集めた大きなリポジトリなので、このサンプルのフォルダーだけを取ります(数秒、十数 MB)。フォルダーの名前が長く `[ ]` を含むので、中の `VB` フォルダーを `.\wingtiptoys-vb` に写して使います。

```powershell
git clone -c core.longpaths=true --filter=blob:none --no-checkout https://github.com/microsoftarchive/msdn-code-gallery-community-a-c.git msdn-code-gallery-a-c
git -C msdn-code-gallery-a-c sparse-checkout set "ASP.NET 4.5 Web Forms Tutorial Series in Visual Basic"
git -C msdn-code-gallery-a-c checkout bbfca35de1c811927fbd019d51969aa878dbd032
Copy-Item -LiteralPath '.\msdn-code-gallery-a-c\ASP.NET 4.5 Web Forms Tutorial Series in Visual Basic\[VB]-ASP.NET 4.5 Web Forms Tutorial Series in Visual Basic\VB' -Destination .\wingtiptoys-vb -Recurse
```

## 2. 変換する

```powershell
.\FrameworkOnCore\converter.ps1 .\wingtiptoys-vb\WingTipToys\WingTipToys.vbproj --out .\out\wingtiptoys-vb --root .\wingtiptoys-vb --build-original auto
```

1 分ほどかかります。変換レポートの「未解決」は 0 です。

## 3. DB を用意する

SQL Server のコンテナを起動します([共通の準備の 5](setup.md#5-sql-server-を使うとき))。DB `WingtipToysVb` とデータ(商品・管理者のユーザー)は、アプリが最初の要求で作ります。

```powershell
docker run -d --name foc-sql -p 14330:1433 -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=Foc_local_Passw0rd" mcr.microsoft.com/mssql/server:2022-latest
```

ARM の PC(Windows on ARM)では、SQL Server の代わりに Azure SQL Edge を使います([共通の準備の 5](setup.md#arm-の-pcwindows-on-armでは))。2 回目以降は `docker start foc-sql` で起動します。C# の WingtipToys と同じコンテナを使えます(DB の名前が違います)。

元の Web.config の接続文字列は LocalDB(Visual Studio 2012 の `(LocalDb)\v11.0`)を指しています。2 つの接続文字列(`DefaultConnection`、`WingtipToys`)を、どちらも環境変数で置き換えます。

## 4. Windows で動かす

```powershell
Push-Location .\out\wingtiptoys-vb\WingTipToys
$env:SQLCONNSTR_DefaultConnection = 'Data Source=localhost,14330;Initial Catalog=WingtipToysVb;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True;MultipleActiveResultSets=True'
$env:SQLCONNSTR_WingtipToys = $env:SQLCONNSTR_DefaultConnection
dotnet bin\WingTipToys.dll --urls http://localhost:5000
Pop-Location                  # Ctrl+C で止めたあと、作業フォルダーに戻る
```

http://localhost:5000/ を開きます。管理者は `Admin` / `Pa$$word` です(ログインすると上のメニューに「Admin」が出て、商品を追加・削除できます)。

## 5. Linux で動かす

```powershell
docker build -t wingtiptoys-vb .\out\wingtiptoys-vb
$cs = 'Data Source=host.docker.internal,14330;Initial Catalog=WingtipToysVb;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True;MultipleActiveResultSets=True'
docker run -d -p 8080:8080 --name wingtiptoys-vb -e "SQLCONNSTR_DefaultConnection=$cs" -e "SQLCONNSTR_WingtipToys=$cs" wingtiptoys-vb
```

http://localhost:8080/ を開きます。

## 6. 結果

元のアプリ(IIS、.NET Framework)と比べて、13 の URL(トップ、商品の一覧・カテゴリー・詳細、About、Contact、ログイン、登録、カート、カートへの追加、管理画面への転送)で、状態コードと内容が同じでした(Windows と Linux。2026-11-15)。違いは次のとおりです。

- `__VIEWSTATE` などの隠しフィールドの値。暗号化の鍵がマシンごとに違うためで、元のアプリでもマシンが変われば変わります。
- `/bundles/...?v=` のハッシュ。変換後は新しい版の縮小ツール(WebGrease)を使うので、縮小した結果とそのハッシュが違います。C# の WingtipToys と同じです。
- Linux では、`Styles.Render`・`Scripts.Render` が並べるタグの間と、隠しフィールドのあとの改行が `\n` になります(Windows と元のアプリは `\r\n`)。表示は変わりません。

## 7. 片付け

```powershell
docker rm -f wingtiptoys-vb foc-sql
```
