# wt(WingtipToys)

[共通の準備](setup.md) が終わっている前提です。コマンドは、とくに書いていなければリポジトリの一番上のフォルダーで実行します。
[コーパスの一覧に戻る](README.md)

## 1. 概要

- **どんなアプリ**: Web Forms のチュートリアルのおもちゃ屋(C#)。EF6 Code First(起動時に DB とデータを作る)、ASP.NET Identity と OWIN、FriendlyUrls、ELMAH。
- **大きさ**: 取得 348 MB、変換の出力 0.6 GB。
- **特別なこと**: SQL Server Express を使うアプリの、いちばん簡単な例です。元の Web.config は存在しない Azure SQL を指していますが、`fetch.ps1` が取得のときに `.\SQLEXPRESS` に向け直します。

## 2. 前提

| もの | 要るか |
|---|---|
| SQL Server Express(`.\SQLEXPRESS`) | **要る**。DB `WingtipToys` はアプリが作ります([共通の準備の手順 7](setup.md#手順-7-sql-server-express-と-sqlcmd-を入れる)) |
| `sqlcmd` | 比較(`verify-windows.ps1`)で使う |
| Visual Studio Build Tools(元のビルド) | 要らない |
| インストーラー | 無い |
| Edge | 比較をするとき |
| Docker Desktop | Linux で動かすとき |
| ディスク | 取得 348 MB、変換の出力 0.6 GB。Linux のイメージ 1.13 GB |

## 3. 手順

この節の手順は、2026-11-04 にこのとおり実行して確かめました。

### 3.1 取得する

```powershell
.\corpora\fetch.ps1 -Only wt
```

確認: 「接続文字列を SQL Server Express(.\SQLEXPRESS)に向けました」と「すべて取得できました。」が出ます。

### 3.2 変換する

元のビルドは要りません。

**Studio で**([使い方](setup.md#41-studio画面)):

| 欄 | 値 |
|---|---|
| プロジェクト | `<リポジトリ>\corpora\work\wingtiptoys-master\WingtipToys\WingtipToys\WingtipToys.csproj` |
| リポジトリのフォルダー | `<リポジトリ>\corpora\work\wingtiptoys-master` |
| 構成 | `Release` |

**コマンドラインで(まとめて変換)**:

```powershell
.\experiments\wf4c\convert-corpora.ps1 -Only wt
```

確認: `=== wt BUILD OK (rounds: 2) 未解決(...)(0 件) / ... / コンパイルのために変えたソース(スタブ・削除)(0 件) / ...` と `snapshot: the same as the baseline (299 entries)`。出力は `experiments\wf4c\wt`。

**コマンドラインで(変換器を直接)**: `convert-corpora.ps1` と同じ値です(この手引きのためには動かしていません)。

```powershell
.\converter.ps1 corpora\work\wingtiptoys-master\WingtipToys\WingtipToys\WingtipToys.csproj --out C:\work\wt-out --root corpora\work\wingtiptoys-master --build-original auto --culture-profile experiments\wf4c\_culture\culture-profile.json
```

### 3.3 DB を用意する

SQL Server Express を起動しておくだけです。DB `WingtipToys` とデータは、アプリが最初の要求で作ります。

```powershell
Start-Service 'MSSQL$SQLEXPRESS'     # 管理者の PowerShell で
```

確認: `(Get-Service 'MSSQL$SQLEXPRESS').Status` が `Running`。

### 3.4 Windows で動かす

```powershell
cd experiments\wf4c\wt\WingtipToys\WingtipToys
..\..\..\supervise.ps1 -Dll bin\WingtipToys.dll -Port 5096
```

**http://localhost:5096/** を開きます。止め方は [共通の準備の 6.2](setup.md#62-止める)。

### 3.5 開くページとログイン

| ページ | 見えるもの |
|---|---|
| `/` | トップ |
| `/ProductList` | 商品の一覧 |
| `/Product/Fast%20Car` | 商品の詳細 |
| `/ShoppingCart` | カート |
| `/Account/Login` | ログイン |
| `/Admin/AdminPage` | 管理者でログインすると開ける |

管理者は **`canEditUser@wingtiptoys.com` / `Pa$$word1`**(アプリが起動時に作る)です。

### 3.6 元のアプリと比べる(Windows)

`supervise.ps1` を止めてから、リポジトリの一番上で実行します。約 1 分。**SQL Server Express は起動しておきます**(このスクリプトは起動しません)。

```powershell
.\experiments\wf4c\verify-windows.ps1 -Name wt
```

確認: `RESULT: OK (8 スナップショットすべて変換前と一致、リンク 64 件も一致)`。

### 3.7 Linux で動かす

Docker Desktop を起動しておきます。

**正解と比べる(`run-linux.ps1`)**: 約 2 分。SQL Server もコンテナ(`w2l-sql`)で動かします。

```powershell
.\experiments\wf4c\run-linux.ps1 -App wt\WingtipToys\WingtipToys -Scenario corpora\regression\wt.scenario.json -Golden corpora\parity\wt.golden-webforms.json -SqlServer
```

確認: `RESULT: OK`。8 画面のうち error-page が「許容」、ほかは一致、リンク 64/64(下の「5. 既知の差・制限」)。

**変換が書いた Dockerfile で動かす**(SQL Server もコンテナで動かす。2026-11-04 に実行):

```powershell
docker network create foc-net
docker run -d --name foc-sql --network foc-net -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=Foc_local_Passw0rd" mcr.microsoft.com/mssql/server:2022-latest
cd experiments\wf4c\wt
docker build -t wingtiptoys .
$cs = 'Data Source=foc-sql;Initial Catalog=WingtipToys;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True;MultipleActiveResultSets=True'
docker run -d -p 8080:8080 --name wingtiptoys --network foc-net -e "SQLCONNSTR_DefaultConnection=$cs" -e "SQLCONNSTR_WingtipToys=$cs" wingtiptoys
```

- SQL Server が起動するまで 10〜20 秒かかります。wt は最初の要求で DB を作ってデータを入れます。
- **http://localhost:8080/** を開きます。確認: `docker logs wingtiptoys` に `Now listening on: http://[::]:8080`。
- 接続文字列は環境変数 `SQLCONNSTR_<接続文字列の名前>` で渡しています(web.config は書き換えません)。

## 4. 期待される結果

| 何を | 結果 | 出どころ |
|---|---|---|
| 変換 | `BUILD OK`。未解決 0、スタブ 0 | **確認** 2026-11-04 |
| Windows の画面の比較 | `RESULT: OK (8 スナップショットすべて変換前と一致、リンク 64 件も一致)` | **確認** 2026-11-04 |
| Linux の画面の比較 | 8/8(うち error-page は理由のある「許容」)、リンク 64/64 | **確認** 2026-11-04 |
| すべての操作の比較 | 152 件(実行できないもの 10 件)。Windows は 223 スナップショット中 一致 192・許容 31、Linux は一致 186・許容 37 | 記録 2026-10-31(2026-11-01 に Windows を再実行して同じ) |

## 5. 既知の差・制限

- **Linux の error-page**: Docker のポート転送で、アプリから見て要求がこの PC からでなくなり(`Request.IsLocal` が偽)、エラーの詳細が出ません。理由つきで `corpora\parity\wt.allowed.json` に記録してある「許容」です。
- すべての操作の正解には Edge 156 の User-Agent が入っています。Edge の版が変わると差が出ることがあります。

## 6. うまくいかないとき

- **起動が止まる・DB につながらない**: 元の Web.config は存在しない Azure SQL を指しています。`fetch.ps1` が `.\SQLEXPRESS` に向け直しますが、それより前に取ったコーパスでは向いていません。取り直します。

  ```powershell
  .\corpora\fetch.ps1 -Only wt -Force
  ```

- **`verify-windows.ps1 -Name wt` が DB につながらない**: このスクリプトは SQL Server Express を起動しません(`-Suite operations` は起動します)。3.3 のとおり起動しておきます。
- 共通のこと(SQL Server、ポート、Docker など)は [共通の準備の 10](setup.md#10-うまくいかないとき)。

## 7. 片付け

```powershell
# Windows で起動したアプリ
Get-NetTCPConnection -LocalPort 5096 -State Listen -ErrorAction SilentlyContinue | ForEach-Object { Stop-Process -Id $_.OwningProcess -Force }
# Dockerfile で動かしたコンテナとネットワーク
docker rm -f wingtiptoys foc-sql
docker network rm foc-net
# run-linux.ps1 -SqlServer が残す SQL Server
docker stop w2l-sql
# SQL Server Express(管理者の PowerShell)
Stop-Service 'MSSQL$SQLEXPRESS'
```

そのあと、Docker Desktop を止めて `wsl --shutdown`、Edge を閉じます([共通の準備の 9](setup.md#9-後片付け))。

## 8. IIS の結果と比べる(任意)

画面の比較(8 画面。3.6・3.7)のほかに、**すべての操作**(152 件)を元のアプリ(IIS で記録した正解)と比べられます。

```powershell
.\experiments\wf4c\verify-windows.ps1 -Name wt -Suite operations     # 約 5 分。SQL Server Express の起動・停止もスクリプトが行う
.\experiments\wf4c\run-linux.ps1 -App wt\WingtipToys\WingtipToys -SqlServer -Suite operations
```

- 正解: `corpora\parity\wt.operations.golden.json`(223 スナップショット)、許される差 `wt.operations.allowed.json`。結果は `experiments\wf4c\_results\`。
- 期待される結果は「4. 期待される結果」。詳しくは [tests/README.md](../../tests/README.md) の「4. コーパス」。
