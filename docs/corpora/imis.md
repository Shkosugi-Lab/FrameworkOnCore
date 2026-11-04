# imis(openIMIS、VB)

[共通の準備](setup.md) が終わっている前提です。コマンドは、とくに書いていなければリポジトリの一番上のフォルダーで実行します。
[コーパスの一覧に戻る](README.md)

このページの手順は記録(2026-10-11)の手順です。この手引きのためには動かしていません(コマンドはスクリプトと照らし合わせて確かめました)。

## 1. 概要

- **どんなアプリ**: 健康保険の管理システム openIMIS(24.10 で開発終了)。**VB の Web Forms**、5 プロジェクト。
- **大きさ**: 取得 115 MB(DB のスクリプト 5 MB は別)、変換の出力 1.5 GB、元のビルド 0.6 GB。
- **特別なこと**: 11 本の中で唯一、VB だけのアプリです。DB は別のリポジトリ(`imisdb`)のスクリプトから作ります。元のビルドの構成は `DemoRelease` です。

## 2. 前提

| もの | 要るか |
|---|---|
| Visual Studio Build Tools(元のビルド) | **要る**([共通の準備の手順 6](setup.md#手順-6-visual-studio-build-tools-を入れる元のビルドが要るコーパスだけ)) |
| SQL Server Express(`.\SQLEXPRESS`)と `sqlcmd` | **要る**。DB `imis_w2l`(`convert-corpora.ps1` が `sqlcmd` で作る) |
| 取得 | `imis` と `imisdb` の両方 |
| Docker Desktop | Linux で動かすとき |
| ディスク | 取得 115 MB+DB のスクリプト 5 MB、変換の出力 1.5 GB、元のビルド 0.6 GB |

## 3. 手順

### 3.1 取得する

元の GitHub のリポジトリから、アプリ(`imis`)と DB のスクリプト(`imisdb`)の 2 つを取ります(このページの結果は、この版で出したものです)。

**アプリ(`imis`)**

| | |
|---|---|
| リポジトリ | <https://github.com/openimis/web_app_vb> |
| 版 | `main` ブランチのコミット `f21970b3136d8083f2da6bb774652614c1ffd247`(2025-01-08)。タグが無いので、コミットで固定しています(いまも main の最後のコミット) |
| ライセンス | GNU AGPL 3.0。[LICENSE.md](https://github.com/openimis/web_app_vb/blob/f21970b3136d8083f2da6bb774652614c1ffd247/LICENSE.md)。リポジトリはアーカイブ済み(読み取り専用) |
| フォルダーの名前 | `web_app_vb-main`(この名前にしておくと、このページのパスがそのまま使えます) |

**DB のスクリプト(`imisdb`)**

| | |
|---|---|
| リポジトリ | <https://github.com/openimis/database_ms_sqlserver> |
| 版 | タグ `24.10`(コミット `166bf13f70d55c6fc2a9c1d0fbf8d661671c6cad`、2024-09-03) |
| ライセンス | GNU AGPL 3.0。[LICENSE.md](https://github.com/openimis/database_ms_sqlserver/blob/24.10/LICENSE.md) |
| フォルダーの名前 | `database_ms_sqlserver-24.10`(この名前にしておくと、このページのパスがそのまま使えます) |

置く場所はどこでも構いません。ただし:

- **比較のスクリプトを使う場合は `corpora\work\web_app_vb-main` と `corpora\work\database_ms_sqlserver-24.10` に置きます**(`convert-corpora.ps1`、`corpora\build-original.ps1`、`convert-corpora.ps1` の出力を使う `verify-windows.ps1`・`run-linux.ps1` は、このパスを前提にしています)。
- パスの長さ: フォルダーの中でいちばん深いファイルは 58 文字なので、置き場所の長さは気にしなくて構いません。

**近道(`fetch.ps1`)**: 下と同じこと(ダウンロード、展開、フォルダーの名前)をして、`corpora\work` に置きます。取得元(リポジトリとタグ・コミット)も表示します。

```powershell
.\corpora\fetch.ps1 -Only imis,imisdb
```

確認: 「すべて取得できました。」と出て、`corpora\work\web_app_vb-main` と `corpora\work\database_ms_sqlserver-24.10` ができます。

**アプリ(`imis`)を手で取る**

**A. GitHub の画面から(ZIP)**

1. <https://github.com/openimis/web_app_vb/tree/f21970b3136d8083f2da6bb774652614c1ffd247> を開き、「Code」→「Download ZIP」を押します(直接のリンク: <https://github.com/openimis/web_app_vb/archive/f21970b3136d8083f2da6bb774652614c1ffd247.zip>)。
2. ZIP を展開します。中にできる `web_app_vb-f21970b3136d8083f2da6bb774652614c1ffd247` フォルダーを `web_app_vb-main` に名前を変えます(コミットの ZIP はフォルダー名にコミットが付くため。`fetch.ps1` も同じ名前に変えます)。

**B. git で**(置きたい場所の一つ上のフォルダーで)

```powershell
git clone -c core.autocrlf=false -c core.eol=lf https://github.com/openimis/web_app_vb.git web_app_vb-main
git -C web_app_vb-main checkout f21970b3136d8083f2da6bb774652614c1ffd247
```

`-c core.autocrlf=false -c core.eol=lf` は、改行を ZIP(と `fetch.ps1`)と同じにするためです。付けないと Git for Windows の既定(`core.autocrlf=true`)でテキストの改行が CRLF に変わり、ファイルの中身が ZIP と違ってしまいます(wt では 178 ファイル)。`.git` フォルダーが増えるのは構いません。

ZIP の中には、大文字小文字だけが違う `IMIS_EN\Web.Config` と `IMIS_EN\Web.config` の 2 つが入っています(中身は同じ)。Windows で展開すると「置き換えますか」と聞かれますが、どちらを選んでも同じです。git でも同じ理由で警告が出ることがありますが、そのままで構いません。

**DB のスクリプト(`imisdb`)を手で取る**

**A. GitHub の画面から(ZIP)**

1. <https://github.com/openimis/database_ms_sqlserver/tree/24.10> を開き、「Code」→「Download ZIP」を押します(直接のリンク: <https://github.com/openimis/database_ms_sqlserver/archive/refs/tags/24.10.zip>)。
2. ZIP を展開します。中の `database_ms_sqlserver-24.10` フォルダーがコーパスです。

**B. git で**(置きたい場所の一つ上のフォルダーで)

```powershell
git clone -c core.autocrlf=false -c core.eol=lf https://github.com/openimis/database_ms_sqlserver.git database_ms_sqlserver-24.10
git -C database_ms_sqlserver-24.10 checkout 166bf13f70d55c6fc2a9c1d0fbf8d661671c6cad
```

`-c core.autocrlf=false -c core.eol=lf` は、改行を ZIP(と `fetch.ps1`)と同じにするためです。付けないと Git for Windows の既定(`core.autocrlf=true`)でテキストの改行が CRLF に変わり、ファイルの中身が ZIP と違ってしまいます(wt では 178 ファイル)。`.git` フォルダーが増えるのは構いません。

**Studio・変換器に渡すフォルダー**: 「リポジトリのフォルダー」(`--root`)は置いたフォルダー(`web_app_vb-main`)、「プロジェクト」はその中の `IMIS\IMIS.vbproj` です。`corpora\work` 以外に置いたときは、3.2 以降の `corpora\work\web_app_vb-main` を置いた場所に読み替えます。

### 3.2 変換する(元のビルドと DB の作成を含む)

**コマンドラインで(まとめて変換。こちらを勧めます)**:

```powershell
Start-Service 'MSSQL$SQLEXPRESS'      # 管理者の PowerShell で
.\experiments\wf4c\convert-corpora.ps1 -Only imis
```

`convert-corpora.ps1` が次を行います。

1. 元のビルド(構成 `DemoRelease`)と変換。
2. DB `imis_w2l` の作成(無ければ): `imisdb` のスクリプトを、スキーマ(`sql\base`)→ ストアド プロシージャ(`sql\stored_procedures`)→ デモのデータ(`sql\demo`)の順に `sqlcmd` で流します。各スクリプトの記録は `experiments\wf4c\_linux\imis-db-<名前>.log`。
3. `experiments\wf4c\imis\site\web.config` の接続文字列(`IMISConnectionString`)を `.\SQLEXPRESS` の `imis_w2l` に向けます。

確認: `=== imis BUILD OK ...` と出て、`imis database: ... failed` の警告が出ないこと。`experiments\wf4c\imis\site\bin\IMIS.dll` があります。
`snapshot` の違いで失敗扱いになるときは [共通の準備の 10.8](setup.md#108-変換がdiffer-from-their-snapshotで失敗する)。

**Studio で**([使い方](setup.md#41-studio画面)):

| 欄 | 値 |
|---|---|
| プロジェクト | `<リポジトリ>\corpora\work\web_app_vb-main\IMIS\IMIS.vbproj` |
| リポジトリのフォルダー | `<リポジトリ>\corpora\work\web_app_vb-main` |
| 構成 | **`DemoRelease`** |
| ステップ 4 | 「元のアプリをそのビルド手順でビルドし、配置されるサイトから組み立てる」にチェック(自動判定で入ります) |

Studio は DB を作らず、web.config も書き換えません。DB は一度 `convert-corpora.ps1 -Only imis` で作っておき、接続先はステップ 5 の「環境変数」で `SQLCONNSTR_IMISConnectionString=...` として渡します(この手引きでは確かめていません)。

**コマンドラインで(変換器を直接)**: `convert-corpora.ps1` と同じ値です。DB の作成と web.config の書き換えはされません。

```powershell
.\converter.ps1 corpora\work\web_app_vb-main\IMIS\IMIS.vbproj --out C:\work\imis-out --root corpora\work\web_app_vb-main --build-original auto --configuration DemoRelease --culture-profile experiments\wf4c\_culture\culture-profile.json
```

### 3.3 DB

3.2 の `convert-corpora.ps1` が作ります。もう一度作り直すときは、DB `imis_w2l` を消してから `convert-corpora.ps1 -Only imis` を動かします(DB があるときは作りません)。

```powershell
sqlcmd -S .\SQLEXPRESS -E -C -Q "DROP DATABASE imis_w2l"
```

### 3.4 Windows で動かす

```powershell
cd experiments\wf4c\imis\site
..\..\supervise.ps1 -Dll bin\IMIS.dll -Port 5097
```

ポートは **5097** です(`imis-login.ps1` の既定)。止め方は [共通の準備の 6.2](setup.md#62-止める)(ポートを 5097 にします)。

### 3.5 ログインして主な画面を開く

別の PowerShell で、リポジトリの一番上から:

```powershell
.\experiments\wf4c\imis-login.ps1                      # 既定 http://localhost:5097
```

- ログインはデモの **`Admin` / `admin123`**(デモのビルドのログイン画面に入っている値)。ブラウザーで http://localhost:5097/ を開いてログインすることもできます。
- 開く画面: `/Home.aspx`、`/FindFamily.aspx`、`/FindInsuree.aspx`、`/FindPolicy.aspx`、`/FindClaims.aspx`、`/FindUser.aspx`、`/Reports.aspx`。
- 確認: 7 つの画面がどれも 200。

### 3.6 Linux で動かす

Docker Desktop を起動しておきます。サイトをそのままコンテナで動かし、SQL Server のコンテナ(`w2l-sql`)に DB `imis_linux` を作って、同じスクリプトを流します(ポート 5098)。

```powershell
$sql = 'corpora\work\database_ms_sqlserver-24.10\sql'
$scripts = @(Get-ChildItem "$sql\base\*.sql") + @(Get-ChildItem "$sql\stored_procedures\*.sql") + @(Get-ChildItem "$sql\demo\*.sql") | ForEach-Object FullName
.\experiments\wf4c\run-linux-site.ps1 -Site imis\site -Dll IMIS -Database imis_linux -SqlScripts $scripts -Keep
.\experiments\wf4c\imis-login.ps1 -Base http://localhost:5098
```

確認: `imis-login.ps1` で 7 つの画面がどれも 200。コンテナのログは `experiments\wf4c\_linux\imis-site\container.log`。

## 4. 期待される結果

| 何を | 結果 | 出どころ |
|---|---|---|
| Windows | ログインし、主な画面 7 つ(`/Home.aspx`、`/FindFamily.aspx`、`/FindInsuree.aspx`、`/FindPolicy.aspx`、`/FindClaims.aspx`、`/FindUser.aspx`、`/Reports.aspx`)が 200 | 記録 2026-10-11 |
| Linux | 同じ | 記録 2026-10-11 |

記録のあとにパッケージの版が上がっています(今は `1.6.5-w2l.27`)。同じ結果になるはずですが、確かめてはいません。

## 5. 既知の差・制限

- 画面は、メニューからの移動でないと(Referer が無いと)「You can't copy and paste or modify URL」と断ります。アプリの仕様です。
- **確かめていないこと**: レポートの画面(ReportViewer)で実際にレポートを出すこと、7-Zip・SQL Server の型(ネイティブのライブラリ)を使う機能。
- IIS で記録した正解は無いので、自動の比較はできません。

## 6. うまくいかないとき

- **「You can't copy and paste or modify URL」と出る**: URL を直接開いたためです。メニューからたどるか、`imis-login.ps1` を使います。
- **ログインできない・DB のエラー**: `experiments\wf4c\_linux\imis-db-*.log` で、DB のスクリプトが失敗していないかを見ます。失敗していたら 3.3 のとおり作り直します。
- **`imis-login.ps1` がつながらない**: アプリを 5097 で起動しているか確かめます(`supervise.ps1` の既定は 5096)。
- 共通のことは [共通の準備の 10](setup.md#10-うまくいかないとき)。

## 7. 片付け

```powershell
Get-NetTCPConnection -LocalPort 5097 -State Listen -ErrorAction SilentlyContinue | ForEach-Object { Stop-Process -Id $_.OwningProcess -Force }
docker rm -f w2l-site-imis               # run-linux-site.ps1 -Keep で残したコンテナ
docker stop w2l-sql                      # run-linux-site.ps1 が残す SQL Server
Stop-Service 'MSSQL$SQLEXPRESS'          # 管理者の PowerShell で
```

そのあと、Docker Desktop を止めて `wsl --shutdown`([共通の準備の 9](setup.md#9-後片付け))。
