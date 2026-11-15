# eShopLegacyMVC

Microsoft がモダナイズの題材として公開している、カタログ管理のサンプルです(MVC 5 と Web API 2、C#)。画面(MVC)と Web API が 1 つのアプリに入っています。[共通の準備](setup.md) が終わっている前提で、コマンドは作業フォルダー(下の 0)で実行します。

| | |
|---|---|
| 取得元 | [dotnet-architecture/eShopModernizing](https://github.com/dotnet-architecture/eShopModernizing) コミット `63bc9ec4`(の `eShopLegacyMVCSolution`) |
| ライセンス | MIT |
| 追加で要るもの | Visual Studio か Build Tools(元のアプリをビルドします) |

DB は使いません。設定 `UseMockData` を `true` にすると、アプリは組み込みのデータ(モック)で動きます。

## 0. FrameworkOnCore を取る

作業用の空のフォルダー(例: `C:\foc`。パスは短くします)を作って移り、そこに FrameworkOnCore を取ります。以降のコマンドは、すべてこのフォルダーで、ここからの相対パスで実行します。ほかのコーパスで済ませてあれば、この手順は飛ばします。

```powershell
New-Item -ItemType Directory -Force C:\foc | Out-Null
cd C:\foc
git clone https://github.com/Shkosugi-Lab/FrameworkOnCore.git
```

## 1. GitHub から取得する

リポジトリが大きい(約 130 MB)ので、`eShopLegacyMVCSolution` のフォルダーだけを取ります。

```powershell
git clone --filter=blob:none --no-checkout https://github.com/dotnet-architecture/eShopModernizing.git
git -C eShopModernizing sparse-checkout set eShopLegacyMVCSolution
git -C eShopModernizing checkout 63bc9ec4414d7e281dc9f9d7bdcf70030950457d
```

## 2. 変換する

```powershell
.\FrameworkOnCore\converter.ps1 .\eShopModernizing\eShopLegacyMVCSolution\src\eShopLegacyMVC\eShopLegacyMVC.csproj --out .\out\eshop-legacy-mvc --root .\eShopModernizing\eShopLegacyMVCSolution --build-original
```

1〜2 分かかります。変換レポートの「未解決」は 0 です。

**Studio で入れる値**([使い方](setup.md#42-studio-で))

| 欄 | 値 |
|---|---|
| プロジェクト | `.\eShopModernizing\eShopLegacyMVCSolution\src\eShopLegacyMVC\eShopLegacyMVC.csproj` |
| リポジトリのフォルダー | `.\eShopModernizing\eShopLegacyMVCSolution` |
| 構成 | `Release` |

## 3. Windows で動かす

```powershell
Push-Location .\out\eshop-legacy-mvc\site
$env:APPSETTING_UseMockData = 'true'
dotnet bin\eShopLegacyMVC.dll --urls http://localhost:5000
Pop-Location                  # Ctrl+C で止めたあと、作業フォルダーに戻る
```

次を開きます。

- http://localhost:5000/ (カタログの一覧。商品の画像は `/items/<番号>/pic`)
- http://localhost:5000/Catalog/Details/1 (商品の詳細)
- http://localhost:5000/api/brands (Web API。ブランドの一覧の JSON)

Studio では、「ネイティブ起動(この PC)」の「環境変数」に次の行を書き、「起動」を押します。

```
APPSETTING_UseMockData=true
```

## 4. Linux で動かす

```powershell
docker build -t eshop-legacy-mvc .\out\eshop-legacy-mvc
docker run -d -p 8080:8080 --name eshop-legacy-mvc -e APPSETTING_UseMockData=true eshop-legacy-mvc
```

http://localhost:8080/ を開きます。Studio では「コンテナ起動(Linux / Docker)」の「環境変数」に、上と同じ `APPSETTING_UseMockData=true` を書きます。

## 5. 結果

元のアプリ(IIS、.NET Framework)と比べて、上の画面と Web API は同じ応答でした(2026-11-15)。違いは次の 1 つです。

- `/api/files` は一覧を BinaryFormatter で書いて返します。データは同じですが、.NET の `List` の内部の配列の大きさが違うため、バイト列が少し違います。

## 6. 片付け

```powershell
docker rm -f eshop-legacy-mvc
```
