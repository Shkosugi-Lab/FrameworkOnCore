# Bootstrap2-VB-MVC をコマンドラインで変換する

Bootstrap 2 の文書を中国語(繁体字)に訳したサイトのソースです(MVC 4、**Visual Basic**)。ビューは VB の Razor(`.vbhtml`)で書かれています。[共通の準備](setup.md) が終わっている前提で、コマンドは作業フォルダー(下の 0)で実行します。Studio(画面)での手順は [Studio 版](studio/bootstrap2-vb-mvc.md) です。

| | |
|---|---|
| 取得元 | [kkbruce/Bootstrap2-VB-MVC](https://github.com/kkbruce/Bootstrap2-VB-MVC) コミット `ca621388` |
| ライセンス | MIT |
| 追加で要るもの | Visual Studio か Build Tools(元のアプリをビルドします) |

DB は使いません。

## 0. FrameworkOnCore を取る

作業用の空のフォルダー(例: `C:\foc`。パスは短くします)を作って移り、そこに FrameworkOnCore を取ります。以降のコマンドは、すべてこのフォルダーで、ここからの相対パスで実行します。ほかのコーパスで済ませてあれば、この手順は飛ばします。

```powershell
New-Item -ItemType Directory -Force C:\foc | Out-Null
cd C:\foc
git clone https://github.com/Shkosugi-Lab/FrameworkOnCore.git
```

## 1. GitHub から取得する

```powershell
curl.exe -L -o bootstrap2-vb-mvc.zip https://github.com/kkbruce/Bootstrap2-VB-MVC/archive/ca621388bfdaedf1d69a505e863fd5ffd52ad0b5.zip
tar -xf bootstrap2-vb-mvc.zip
Rename-Item Bootstrap2-VB-MVC-ca621388bfdaedf1d69a505e863fd5ffd52ad0b5 Bootstrap2-VB-MVC
```

## 2. 変換する

```powershell
.\FrameworkOnCore\converter.ps1 .\Bootstrap2-VB-MVC\kkbruce.tw\kkbruce.tw.vbproj --out .\out\bootstrap2-vb-mvc --root .\Bootstrap2-VB-MVC --build-original
```

1 分ほどかかります。変換レポートの「未解決」は 0 です。

- 元のアプリは MVC 4(Razor 2)です。変換後は MVC 5(Razor 3)になるので、変換レポートの「パッケージの大きな版上げ」に、そのとき変わることが出ます(下の 5)。

## 3. Windows で動かす

```powershell
Push-Location .\out\bootstrap2-vb-mvc\site
dotnet bin\kkbruce.tw.dll --urls http://localhost:5000
Pop-Location                  # Ctrl+C で止めたあと、作業フォルダーに戻る
```

http://localhost:5000/ を開きます。上のメニューから、各ページ(`/Bootstrap/GettingStarted`、`/Bootstrap/BaseCSS`、`/Bootstrap/Components` など)に移れます。

## 4. Linux で動かす

```powershell
docker build -t bootstrap2-vb-mvc .\out\bootstrap2-vb-mvc
docker run -d -p 8080:8080 --name bootstrap2-vb-mvc bootstrap2-vb-mvc
```

http://localhost:8080/ を開きます。

## 5. 結果

元のアプリ(IIS、.NET Framework)と比べて、10 画面とも同じ内容でした(2026-11-15)。違いは次のとおりです。

- IE 用の条件付きコメント(`<!--[if lt IE 9]>`)の中の `~/Scripts/...` を、元(Razor 2)は `/Scripts/...` に直して出し、変換後(Razor 3)はそのまま出します。Microsoft の Razor 3 でも同じ動きです。今のブラウザーはこのコメントの中を読まないので、表示は変わりません。
- Linux では、`Styles.Render`・`Scripts.Render` が並べるタグの間の改行が `\n` になります(Windows と元のアプリは `\r\n`)。表示は変わりません。

## 6. 片付け

```powershell
docker rm -f bootstrap2-vb-mvc
```
