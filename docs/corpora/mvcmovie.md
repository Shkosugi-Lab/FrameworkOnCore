# mvcmovie(MvcMovie、MVC 5)

[共通の準備](setup.md) が終わっている前提です。コマンドは、とくに書いていなければリポジトリの一番上のフォルダーで実行します。
[コーパスの一覧に戻る](README.md)

このページの手順は記録(Windows 2026-10-27、Linux 2026-10-22)によるものです。この手引きのためには動かしていません(コマンドはスクリプトと照らし合わせて確かめました)。

## 1. 概要

- **どんなアプリ**: ASP.NET MVC 5 の公式チュートリアルの映画の一覧(C#、EF6 Code First、ASP.NET Identity)。
- **大きさ**: 取得 97 MB、変換の出力 0.6 GB。
- **特別なこと**: MVC 5 を試すのにいちばん簡単なコーパスです。元の Web.config は LocalDB を指していますが、`fetch.ps1` が取得のときに `.\SQLEXPRESS` に向け直します。

## 2. 前提

| もの | 要るか |
|---|---|
| SQL Server Express(`.\SQLEXPRESS`)と `sqlcmd` | **要る**。DB `MvcMovie`・`MvcMovieIdentity` はアプリが作ります。`sqlcmd` は比較の前に DB を消すのに使います |
| Visual Studio Build Tools(元のビルド) | 要らない |
| インストーラー | 無い |
| Edge | 比較をするとき |
| Docker Desktop | Linux で動かすとき |
| ディスク | 取得 97 MB、変換の出力 0.6 GB |

## 3. 手順

### 3.1 取得する

元の GitHub のリポジトリ(ASP.NET の文書のリポジトリ)の中の、1 つのフォルダーを取ります(このページの結果は、この版で出したものです)。

| | |
|---|---|
| リポジトリ | <https://github.com/dotnet/AspNetDocs> |
| 版 | コミット `552c58dd8a809ef8cea02e66c9463ce9c7dcdfe5`(2023-07-28)の中のフォルダー `aspnet/mvc/overview/getting-started/introduction/sample/MvcMovie` |
| ライセンス | コードは MIT License([LICENSE-CODE](https://github.com/dotnet/AspNetDocs/blob/552c58dd8a809ef8cea02e66c9463ce9c7dcdfe5/LICENSE-CODE))、文書などは CC BY 4.0([LICENSE](https://github.com/dotnet/AspNetDocs/blob/552c58dd8a809ef8cea02e66c9463ce9c7dcdfe5/LICENSE))。[ThirdPartyNotices](https://github.com/dotnet/AspNetDocs/blob/552c58dd8a809ef8cea02e66c9463ce9c7dcdfe5/ThirdPartyNotices) のとおり |
| フォルダーの名前 | `MvcMovie`(この名前にしておくと、このページのパスがそのまま使えます) |

置く場所はどこでも構いません。ただし:

- **比較のスクリプトを使う場合は `corpora\work\MvcMovie` に置きます**(`convert-corpora.ps1`、`corpora\build-original.ps1`、`convert-corpora.ps1` の出力を使う `verify-windows.ps1`・`run-linux.ps1` は、このパスを前提にしています)。
- パスの長さ: 元のビルド(NuGet の復元)のあと、フォルダーの中でいちばん深いファイルは 148 文字です。.NET Framework の MSBuild は Windows の MAX_PATH(260 文字)を超えるパスを扱えないので、**フォルダー自体のパスを 110 文字以内**にします(`<リポジトリ>\corpora\work\MvcMovie` や `C:\corpora\MvcMovie` なら収まります)。
- `git` が要ります(下の B はリポジトリ全体の大きな ZIP になるので、A か `fetch.ps1` をすすめます)。

**近道(`fetch.ps1`)**: 下の A と同じこと(git での取得、フォルダーの名前、Web.config の接続文字列)をして、`corpora\work` に置きます。取得元(リポジトリとタグ・コミット)も表示します。

```powershell
.\corpora\fetch.ps1 -Only mvcmovie
```

確認: 「接続文字列を SQL Server Express(.\SQLEXPRESS)に向けました」と「すべて取得できました。」が出ます。`git clone` で取るので Git が要ります。

**A. git で(すすめる)**: リポジトリは大きい(全体で約 600 MB)ので、`fetch.ps1` と同じく、そのフォルダーだけを取ります。短いパスのフォルダー(`C:\src` など)で:

```powershell
git clone -c core.autocrlf=true --filter=blob:none --no-checkout --sparse https://github.com/dotnet/AspNetDocs.git AspNetDocs
git -C AspNetDocs sparse-checkout set aspnet/mvc/overview/getting-started/introduction/sample/MvcMovie
git -C AspNetDocs checkout 552c58dd8a809ef8cea02e66c9463ce9c7dcdfe5
Move-Item AspNetDocs\aspnet\mvc\overview\getting-started\introduction\sample\MvcMovie MvcMovie
Remove-Item AspNetDocs -Recurse -Force     # 残りは要らない
```

`-c core.autocrlf=true` は、改行を `fetch.ps1` と同じ(テキストは CRLF。比較の基準を作ったときのもの)にするためです。Git の設定が `core.autocrlf=true`(Git for Windows の既定)なら付けなくても同じです。

**B. GitHub の画面から(ZIP)**: このフォルダーだけの ZIP は GitHub にありません。リポジトリ全体の ZIP <https://github.com/dotnet/AspNetDocs/archive/552c58dd8a809ef8cea02e66c9463ce9c7dcdfe5.zip> を取り、展開した `AspNetDocs-552c58dd8a809ef8cea02e66c9463ce9c7dcdfe5\aspnet\mvc\overview\getting-started\introduction\sample\MvcMovie` フォルダーを `MvcMovie` という名前で置きます(フォルダーの中身は <https://github.com/dotnet/AspNetDocs/tree/552c58dd8a809ef8cea02e66c9463ce9c7dcdfe5/aspnet/mvc/overview/getting-started/introduction/sample/MvcMovie> で見られます)。ZIP の改行は LF のままで、比較の基準(CRLF)と違います。結果を記録と比べるなら A か `fetch.ps1` を使います。

**手で直すところ(Web.config の接続文字列)**: `MvcMovie\Web.config` の接続文字列は LocalDB(ユーザーごとのインスタンスで、IIS と変換後のアプリでは別の DB になる)を指しているので、SQL Server Express(`.\SQLEXPRESS`)に向けます。正解(IIS で記録したもの)を記録したときと同じ値です。`fetch.ps1` はこれを自動で行います。

`<add name="DefaultConnection" connectionString="...">` の `connectionString` の値:

```text
元: Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=aspnet-MvcMovie-fefdc1f0-bd81-4ce9-b712-93a062e01031;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|\aspnet-MvcMovie-fefdc1f0-bd81-4ce9-b712-93a062e01031.mdf
後: Data Source=.\SQLEXPRESS;Initial Catalog=MvcMovieIdentity;Integrated Security=True
```

`<add name="MovieDBContext" connectionString="...">` の `connectionString` の値:

```text
元: Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=aspnet-MvcMovie;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|\Movies.mdf
後: Data Source=.\SQLEXPRESS;Initial Catalog=MvcMovie;Integrated Security=True
```

**Studio・変換器に渡すフォルダー**: 「リポジトリのフォルダー」(`--root`)は置いたフォルダー(`MvcMovie`)、「プロジェクト」はその中の `MvcMovie\MvcMovie.csproj` です。`corpora\work` 以外に置いたときは、3.2 以降の `corpora\work\MvcMovie` を置いた場所に読み替えます。

### 3.2 変換する

**コマンドラインで(まとめて変換)**:

```powershell
.\experiments\wf4c\convert-corpora.ps1 -Only mvcmovie
```

確認: `=== mvcmovie BUILD OK ...`。出力は `experiments\wf4c\mvcmovie`。
`snapshot` の違いで失敗扱いになるときは [共通の準備の 10.8](setup.md#108-変換がdiffer-from-their-snapshotで失敗する)。

**Studio で**([使い方](setup.md#41-studio画面)):

| 欄 | 値 |
|---|---|
| プロジェクト | `<リポジトリ>\corpora\work\MvcMovie\MvcMovie\MvcMovie.csproj` |
| リポジトリのフォルダー | `<リポジトリ>\corpora\work\MvcMovie` |
| 構成 | `Release` |

**コマンドラインで(変換器を直接)**:

```powershell
.\converter.ps1 corpora\work\MvcMovie\MvcMovie\MvcMovie.csproj --out C:\work\mvcmovie-out --root corpora\work\MvcMovie --build-original auto --culture-profile experiments\wf4c\_culture\culture-profile.json
```

### 3.3 DB を用意する

SQL Server Express を起動しておくだけです。DB `MvcMovie`・`MvcMovieIdentity` はアプリ(EF)が作ります。

```powershell
Start-Service 'MSSQL$SQLEXPRESS'      # 管理者の PowerShell で
```

### 3.4 Windows で動かす

```powershell
cd experiments\wf4c\mvcmovie\MvcMovie
..\..\supervise.ps1 -Dll bin\MvcMovie.dll -Port 5096
```

**http://localhost:5096/Movies** を開きます。止め方は [共通の準備の 6.2](setup.md#62-止める)。

### 3.5 開くページ

| ページ | 見えるもの |
|---|---|
| `/Movies` | 映画の一覧(作成・編集・詳細・削除へ進める) |

### 3.6 元のアプリと比べる(Windows)

`supervise.ps1` を止めてから実行します。比べる前に DB を消します(EF が作り直します)。

```powershell
.\experiments\wf4c\verify-windows.ps1 -Name mvcmovie
```

確認: 12 画面のうち 11 が一致(下の「4. 期待される結果」)。

### 3.7 Linux で動かす

Docker Desktop を起動しておきます。SQL Server もコンテナ(`w2l-sql`)で動かします。

```powershell
.\experiments\wf4c\run-linux.ps1 -App mvcmovie\MvcMovie -Scenario corpora\regression\mvcmovie.scenario.json -Golden corpora\parity\mvcmovie.golden-webforms.json -SqlServer
```

確認: 12 画面のうち 11 が一致。

## 4. 期待される結果

| 何を | 結果 | 出どころ |
|---|---|---|
| Windows | 12 画面中 11 | 記録 2026-10-27 |
| Linux | 11/12 | 記録 2026-10-22 |

残りの 1 つは、作成の入力の検証のメッセージの言語です(元のサーバーは .NET Framework の日本語の言語パック、変換後は英語だった)。
2026-10-22 に FrameworkOnCore の日本語訳が入ったので、いまは「訳の文言の違い」になるはずです(まだ流していません)。
記録のあとにパッケージの版が上がっています(今は `1.6.5-w2l.27`)。

## 5. 既知の差・制限

- 入力の検証のメッセージの文言(4 のとおり)。

## 6. うまくいかないとき

- **DB につながらない**: 元の Web.config は LocalDB を指しています。`fetch.ps1` が `.\SQLEXPRESS` に向け直したかを見ます(取り直すときは `.\corpora\fetch.ps1 -Only mvcmovie -Force`)。SQL Server Express は起動しておきます。
- **`sqlcmd` が無いと言われる**: `verify-windows.ps1` が DB を消すのに使います([共通の準備の手順 7](setup.md#手順-7-sql-server-express-と-sqlcmd-を入れる))。
- 共通のことは [共通の準備の 10](setup.md#10-うまくいかないとき)。

## 7. 片付け

```powershell
Get-NetTCPConnection -LocalPort 5096 -State Listen -ErrorAction SilentlyContinue | ForEach-Object { Stop-Process -Id $_.OwningProcess -Force }
docker stop w2l-sql                      # run-linux.ps1 -SqlServer が残す SQL Server
Stop-Service 'MSSQL$SQLEXPRESS'          # 管理者の PowerShell で
```

そのあと、Docker Desktop を止めて `wsl --shutdown`、Edge を閉じます([共通の準備の 9](setup.md#9-後片付け))。

## 8. IIS の結果と比べる(任意)

3.6・3.7 の比較が、元のアプリ(IIS で記録した正解 `corpora\parity\mvcmovie.golden-webforms.json`、シナリオ `corpora\regression\mvcmovie.scenario.json`)との比較です。すべての操作の比較はありません。
