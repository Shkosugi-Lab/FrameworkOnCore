# MultipleEndpoints(C# と VB) を Studio で変換する

Microsoft の WCF のサンプルの、IIS で動く電卓のサービスです(`.svc`)。**同じサービスの C# 版と VB 版**があるので、両方を変換します。Studio(画面)で解析・変換し、Windows と Linux のコンテナで動かします。コマンドラインでの手順は [コマンド版](../wcf-multiple-endpoints.md) です。

| | |
|---|---|
| 取得元 | [dotnet/samples](https://github.com/dotnet/samples) コミット `38105588` の `framework/wcf/Basic/Services/MultipleEndpoints` |
| ライセンス | コードは MIT(文書は CC BY 4.0) |

入れておくもの(.NET 10 SDK、Git、Docker Desktop、**Visual Studio または Visual Studio Build Tools**)は [共通の準備の 1](../setup.md#1-入れるもの) にあります。Studio はステップ 1 で元のアプリをビルドするので、Visual Studio か Build Tools が要ります。DB は要りません。

## 1. FrameworkOnCore とソースを取る

作業用の空のフォルダー(例: `C:\foc`。パスは短くします)を作り、FrameworkOnCore を取ります。ほかのシナリオで済ませてあれば、`cd C:\foc` だけにします。

```powershell
New-Item -ItemType Directory -Force C:\foc | Out-Null
cd C:\foc
git clone https://github.com/Shkosugi-Lab/FrameworkOnCore.git
```

同じフォルダーに、アプリのソースを取ります。

リポジトリがとても大きい(約 300 MB)ので、このサンプルのフォルダーだけを取ります。

```powershell
git clone --filter=blob:none --no-checkout https://github.com/dotnet/samples.git dotnet-samples
git -C dotnet-samples sparse-checkout set framework/wcf/Basic/Services/MultipleEndpoints
git -C dotnet-samples checkout 3810558800760fc39282cc939bac645735514209
```

`.\dotnet-samples\framework\wcf\Basic\Services\MultipleEndpoints` の下に `CS`(C#)と `VB` ができます。どちらも `service`(サービス)と `client`(コンソールのクライアント)のプロジェクトです。

## 2. Studio を起動する

ソースを取ったフォルダーで起動します。こうすると、Studio に入れるパスをこのフォルダーからの相対パスで書けます。

```powershell
.\FrameworkOnCore\studio.ps1
```

「Studio: http://127.0.0.1:5300/」と出たら、ブラウザーで http://127.0.0.1:5300/ を開きます。止めるときは、この PowerShell で Ctrl+C を押します。

右上の **「新しい解析」** を押します。

![Studio の最初の画面](images/wcf-multiple-endpoints/01-home.png)

## 3. 新しい解析を作る

次の値を入れて、**「解析する」** を押します。

| 欄 | 値 |
|---|---|
| プロジェクト(.csproj / .vbproj) | `.\dotnet-samples\framework\wcf\Basic\Services\MultipleEndpoints\CS\service\service.csproj` |
| リポジトリのフォルダー | `.\dotnet-samples\framework\wcf\Basic\Services\MultipleEndpoints\CS` |
| 構成 | `Release` |

VB 版は、プロジェクトの `CS` を `VB`、`service.csproj` を `service.vbproj` に、リポジトリのフォルダーの `CS` を `VB` にします。手順は同じです。

![新しい解析](images/wcf-multiple-endpoints/02-new-analysis.png)

## 4. ステップ 1 読み込み

プロジェクトの構成(言語、.NET Framework の版、NuGet パッケージの数、ページの数など)が出ます。解析は裏で進みます。

下の「元のアプリのビルド」で、**「ビルドに使う MSBuild」**(Visual Studio か Build Tools。入っていないほうは選べません)を確かめて、**「ビルド」** を押します。元のアプリを、そのリポジトリのビルド手順でビルドします。**ビルドが通るまで、ステップ 2 以降には進めません。**「追加の手順」は空のままです。

![元のアプリのビルド(ビルド前)](images/wcf-multiple-endpoints/04-step1-build.png)

ビルド中は、その下の「ログ」にビルドの様子が出ます。**「ビルド成功」** と出たら、いちばん下の **「次へ」** を押します。

ビルドが通ると「元のアプリを起動(テスト起動)」も出ます。変換前の動きを確かめたいときに使います(IIS Express が要ります。この手順では使いません)。

![元のアプリのビルド(ビルド成功)](images/wcf-multiple-endpoints/05-step1-built.png)

## 5. ステップ 2 解析と対応

アプリが使う .NET Framework の API と、変換での扱いが出ます。「自分で対応する所」のタブは変換だけでは動くようにならない所、「対応を選ぶ部品」のタブは部品ごとに直し方を選べる所です。

![ステップ 2 解析と対応](images/wcf-multiple-endpoints/06-step2.png)

**既定のままで変換できます。** いちばん下の **「次へ」** を押します。

![ステップ 2 の「次へ」](images/wcf-multiple-endpoints/06b-step2-next.png)

## 6. ステップ 3 サーバーの設定

IIS の機能(ファイル名の大文字・小文字、アクセス ログ、URL Rewrite、エラー ページなど)の動かし方を選べます。既定は、元の IIS と同じ動きです。既定のまま **「次へ」** を押します。

![ステップ 3 サーバーの設定](images/wcf-multiple-endpoints/07-step3.png)

## 7. ステップ 4 変換・ビルド

**「変換してビルド」** を押します。元のアプリをステップ 1 と同じ手順でまっさらな状態からビルドし直し、.NET 10 に変換してビルドします(数分かかります)。

![ステップ 4 変換・ビルド(実行前)](images/wcf-multiple-endpoints/08-step4.png)

**「ビルド成功」** になり、下に変換レポートが出ます。**「未解決」が 0** であることを確かめて、**「次へ: 実行・デプロイ」** を押します。

![ステップ 4 変換・ビルド(ビルド成功)](images/wcf-multiple-endpoints/09-step4-done.png)

## 8. ステップ 5 実行・デプロイ

変換したアプリを動かします。「ZIP ダウンロード」では、変換の出力を Linux 用・Windows 用に取り出せます。

### 8.1 Windows で動かす(ネイティブ起動)

「ネイティブ起動(この PC)」の **「起動」** を押します。環境変数は要りません。

![ネイティブ起動](images/wcf-multiple-endpoints/11-native.png)

「実行中」になったら、**「開く」** を押します(開いたページの URL の後ろに `/service.svc` を付けます)。最初の表示は、ページのコンパイルで時間がかかることがあります。止めるときは「停止」を押します。

![ネイティブ起動(実行中)](images/wcf-multiple-endpoints/12-native-running.png)

### 8.2 Linux で動かす(コンテナ起動)

Docker Desktop を起動しておきます(止まっていると「Docker Desktop を起動」ボタンが出るので、それを押しても構いません)。

「コンテナ起動(Linux / Docker)」の **「ビルドして起動」** を押します。環境変数は要りません。

![コンテナ起動](images/wcf-multiple-endpoints/14-container.png)

Dockerfile からイメージを作るので、初回は数分かかります。「実行中」になったら、**「開く」** を押します(URL の後ろに `/service.svc` を付けます)。止めるときは「停止」を押します。

![コンテナ起動(実行中)](images/wcf-multiple-endpoints/15-container-running.png)

サービスの説明のページです(URL の後ろに `service.svc` を付けずに開くと 403 になります。元の IIS と同じく、フォルダーの一覧は出しません)。サービスの呼び出し(SOAP)は、[コマンド版の 3](../wcf-multiple-endpoints.md#3-windows-で動かす) の PowerShell で、URL を Studio の URL(`http://localhost:8100/service.svc`、コンテナは `http://localhost:8080/service.svc`)にして確かめます。

![コンテナで動いている MultipleEndpoints(C# と VB)](images/wcf-multiple-endpoints/16-container-app.png)

元のアプリと比べた結果は、[コマンド版の「結果」](../wcf-multiple-endpoints.md#5-結果) にあります。

## 9. 片付け

- アプリは「停止」で止めます。Studio は、起動した PowerShell で Ctrl+C を押して止めます。Studio を止めると、Studio が起動したアプリも止まります。
- 終わったら Docker Desktop を止め、`wsl --shutdown` で WSL のメモリーを返します。
- Studio の出力は `%LOCALAPPDATA%\FrameworkOnCore\studio\` にあります。不要になった解析は、画面右上の「削除」で消せます。
