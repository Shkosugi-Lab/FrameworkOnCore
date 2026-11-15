# wcf(WCF のサービス: MultipleEndpoints、C# と VB)

Microsoft の WCF のサンプルの、IIS で動く電卓のサービスです(`.svc`)。**同じサービスの C# 版と VB 版**があるので、両方を変換します。[共通の準備](setup.md) が終わっている前提で、コマンドは作業フォルダー(下の 0)で実行します。

| | |
|---|---|
| 取得元 | [dotnet/samples](https://github.com/dotnet/samples) コミット `38105588` の `framework/wcf/Basic/Services/MultipleEndpoints` |
| ライセンス | コードは MIT(文書は CC BY 4.0) |
| 追加で要るもの | Visual Studio か Build Tools(元のアプリをビルドします) |

サービスには口(エンドポイント)が 2 つあります。

| 口 | 通信の方式 | 変換後 |
|---|---|---|
| `service.svc` | basicHttpBinding(SOAP 1.1) | 動きます(CoreWCF) |
| `service.svc/secure` | wsHttpBinding(既定のメッセージ セキュリティ) | **動きません**。CoreWCF がメッセージ セキュリティに対応していないためで、変換レポートの「.NET に無い、または Windows 専用の API」に出ます。ほかの口は動きます |

## 0. FrameworkOnCore を取る

作業用の空のフォルダー(例: `C:\foc`。パスは短くします)を作って移り、そこに FrameworkOnCore を取ります。以降のコマンドは、すべてこのフォルダーで、ここからの相対パスで実行します。ほかのコーパスで済ませてあれば、この手順は飛ばします。

```powershell
New-Item -ItemType Directory -Force C:\foc | Out-Null
cd C:\foc
git clone https://github.com/Shkosugi-Lab/FrameworkOnCore.git
```

## 1. GitHub から取得する

リポジトリがとても大きい(約 300 MB)ので、このサンプルのフォルダーだけを取ります。

```powershell
git clone --filter=blob:none --no-checkout https://github.com/dotnet/samples.git wcf
git -C wcf sparse-checkout set framework/wcf/Basic/Services/MultipleEndpoints
git -C wcf checkout 3810558800760fc39282cc939bac645735514209
```

`.\wcf\framework\wcf\Basic\Services\MultipleEndpoints` の下に `CS`(C#)と `VB` ができます。どちらも `service`(サービス)と `client`(コンソールのクライアント)のプロジェクトです。

## 2. 変換する

```powershell
$sample = '.\wcf\framework\wcf\Basic\Services\MultipleEndpoints'
.\FrameworkOnCore\converter.ps1 "$sample\CS\service\service.csproj" --out .\out\wcf-cs --root "$sample\CS" --build-original
.\FrameworkOnCore\converter.ps1 "$sample\VB\service\service.vbproj" --out .\out\wcf-vb --root "$sample\VB" --build-original
```

それぞれ 1 分ほどかかります。変換レポートの「未解決」は 0 です。

**Studio で入れる値**([使い方](setup.md#42-studio-で))。VB 版は `CS` を `VB`、`service.csproj` を `service.vbproj` にします。

| 欄 | 値 |
|---|---|
| プロジェクト | `.\wcf\framework\wcf\Basic\Services\MultipleEndpoints\CS\service\service.csproj` |
| リポジトリのフォルダー | `.\wcf\framework\wcf\Basic\Services\MultipleEndpoints\CS` |
| 構成 | `Release` |

## 3. Windows で動かす

C# 版の例です(VB 版は `wcf-cs` を `wcf-vb` にします)。

```powershell
Push-Location .\out\wcf-cs\site
dotnet bin\service.dll --urls http://localhost:5000
Pop-Location                  # Ctrl+C で止めたあと、作業フォルダーに戻る
```

- http://localhost:5000/service.svc を開くと、サービスの説明のページが出ます。
- http://localhost:5000/service.svc?wsdl で WSDL が取れます。

足し算を呼んでみます(別の PowerShell で)。

```powershell
$body = '<s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/"><s:Body><Add xmlns="http://Microsoft.ServiceModel.Samples"><n1>100</n1><n2>15.99</n2></Add></s:Body></s:Envelope>'
(Invoke-WebRequest http://localhost:5000/service.svc -Method Post -Body $body -ContentType 'text/xml; charset=utf-8' -Headers @{ SOAPAction = 'http://Microsoft.ServiceModel.Samples/ICalculator/Add' } -UseBasicParsing).Content
```

応答の `<AddResult>115.99</AddResult>` が答えです。`Subtract`、`Multiply`、`Divide` も同じように呼べます(`Add` の 3 か所を置き換えます)。

Studio では、「ネイティブ起動(この PC)」の「起動」を押します(環境変数は要りません)。

## 4. Linux で動かす

```powershell
docker build -t wcf-cs .\out\wcf-cs
docker run -d -p 8080:8080 --name wcf-cs wcf-cs
```

http://localhost:8080/service.svc を開きます。上の呼び出しも、URL を `http://localhost:8080/service.svc` にすれば同じです。

## 5. 結果

元のアプリ(IIS、.NET Framework)と比べて、C# 版・VB 版とも、basicHttpBinding の口の 4 つの演算は同じ値を返しました(2026-11-15)。違いは次のとおりです。

- `secure` の口(wsHttpBinding)は動きません(上の表)。
- 割り算の結果の書き方が違います。元は `3.1428571428571428`、変換後は `3.142857142857143` です。同じ double の値で、.NET が往復できる最短の桁で書くためです(受け取った側では同じ値になります)。
- WSDL の内容は CoreWCF のもので、元とは違います(`secure` の口も載りません)。`service.svc/mex`(メタデータ交換の口)は動きません。WSDL は `?wsdl` で取れます。

## 6. 片付け

```powershell
docker rm -f wcf-cs wcf-vb
```
