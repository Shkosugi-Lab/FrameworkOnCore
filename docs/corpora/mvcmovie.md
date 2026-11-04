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

```powershell
.\corpora\fetch.ps1 -Only mvcmovie
```

確認: 「接続文字列を SQL Server Express(.\SQLEXPRESS)に向けました」と「すべて取得できました。」が出ます。`git clone` で取るので Git が要ります。

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
