# mojo(mojoPortal 3.1.6)

[共通の準備](setup.md) が終わっている前提です。コマンドは、とくに書いていなければリポジトリの一番上のフォルダーで実行します。
[コーパスの一覧に戻る](README.md)

このページの手順は記録(2026-10-11)の手順です。この手引きのためには動かしていません(コマンドはスクリプトと照らし合わせて確かめました)。

## 1. 概要

- **どんなアプリ**: CMS(Web Forms、C#)。初回の起動でセットアップ画面がスキーマ(105 テーブル)を作ります。
- **大きさ**: 取得 161 MB、変換の出力 4.1 GB、元のビルド 0.7 GB。
- **特別なこと**: 元のアプリをソリューションでビルドし、配置されるサイト(`Web`)から組み立てます。接続先は `site\user.config` に書きます(`convert-corpora.ps1` が書きます)。

## 2. 前提

| もの | 要るか |
|---|---|
| Visual Studio Build Tools(元のビルド) | **要る**([共通の準備の手順 6](setup.md#手順-6-visual-studio-build-tools-を入れる元のビルドが要るコーパスだけ)) |
| SQL Server Express(`.\SQLEXPRESS`) | **要る**。DB `mojo_w2l` |
| インストーラー | 初回の起動でセットアップ画面(`/Setup/Default.aspx`)が動く |
| Docker Desktop | Linux で動かすとき。casefs([手順 11](setup.md#手順-11-casefs-をビルドするlinux-で動かすときだけ))も |
| ディスク | 取得 161 MB、変換の出力 4.1 GB、元のビルド 0.7 GB |

## 3. 手順

### 3.1 取得する

元の GitHub のリポジトリから、次の版を取ります(このページの結果は、この版で出したものです)。

| | |
|---|---|
| リポジトリ | <https://github.com/i7MEDIA/mojoportal> |
| 版 | タグ `v3.1.6`(コミット `c0ada13952e4078ff2e89fe954511dfe29c1677c`、2026-08-10) |
| ライセンス | 主に Eclipse Public License 1.0(EPL-1.0)。[LICENSE.md](https://github.com/i7MEDIA/mojoportal/blob/v3.1.6/LICENSE.md)。使っているほかのオープンソースのライセンスは `licenses` フォルダー |
| フォルダーの名前 | `mojoportal-3.1.6`(この名前にしておくと、このページのパスがそのまま使えます) |

置く場所はどこでも構いません。ただし:

- **比較のスクリプトを使う場合は `corpora\work\mojoportal-3.1.6` に置きます**(`convert-corpora.ps1`、`corpora\build-original.ps1`、`convert-corpora.ps1` の出力を使う `verify-windows.ps1`・`run-linux.ps1` は、このパスを前提にしています)。
- パスの長さ: 元のビルド(NuGet の復元)のあと、フォルダーの中でいちばん深いファイルは 130 文字です。.NET Framework の MSBuild は Windows の MAX_PATH(260 文字)を超えるパスを扱えないので、**フォルダー自体のパスを 128 文字以内**にします(`<リポジトリ>\corpora\work\mojoportal-3.1.6` や `C:\corpora\mojoportal-3.1.6` なら収まります)。

**近道(`fetch.ps1`)**: 下と同じこと(ダウンロード、展開、フォルダーの名前)をして、`corpora\work` に置きます。取得元(リポジトリとタグ・コミット)も表示します。

```powershell
.\corpora\fetch.ps1 -Only mojo
```

確認: 「すべて取得できました。」と出て、`corpora\work\mojoportal-3.1.6` ができます。

**A. GitHub の画面から(ZIP)**

1. <https://github.com/i7MEDIA/mojoportal/tree/v3.1.6> を開き、「Code」→「Download ZIP」を押します(直接のリンク: <https://github.com/i7MEDIA/mojoportal/archive/refs/tags/v3.1.6.zip>)。
2. ZIP を展開します。中の `mojoportal-3.1.6` フォルダーがコーパスです。

**B. git で**(置きたい場所の一つ上のフォルダーで)

```powershell
git clone -c core.autocrlf=false -c core.eol=lf https://github.com/i7MEDIA/mojoportal.git mojoportal-3.1.6
git -C mojoportal-3.1.6 checkout c0ada13952e4078ff2e89fe954511dfe29c1677c
```

`-c core.autocrlf=false -c core.eol=lf` は、改行を ZIP(と `fetch.ps1`)と同じにするためです。付けないと Git for Windows の既定(`core.autocrlf=true`)でテキストの改行が CRLF に変わり、ファイルの中身が ZIP と違ってしまいます(wt では 178 ファイル)。`.git` フォルダーが増えるのは構いません。

**Studio・変換器に渡すフォルダー**: 「リポジトリのフォルダー」(`--root`)は置いたフォルダー(`mojoportal-3.1.6`)、「プロジェクト」はその中の `Web\mojoPortal.Web.csproj` です。`corpora\work` 以外に置いたときは、3.2 以降の `corpora\work\mojoportal-3.1.6` を置いた場所に読み替えます。

### 3.2 変換する(元のビルドを含む)

**コマンドラインで(まとめて変換。こちらを勧めます)**:

```powershell
Start-Service 'MSSQL$SQLEXPRESS'      # 管理者の PowerShell で
.\experiments\wf4c\convert-corpora.ps1 -Only mojo
```

- 元のビルド(ソリューション)をしてから変換します。元のビルドは一度作ると次からは使い回します(作り直すときは `-Rebuild`)。
- 変換のあと、`convert-corpora.ps1` が `experiments\wf4c\mojo\site\user.config` を書きます: `.\SQLEXPRESS` の `mojo_w2l` を使う接続文字列(`MSSQLConnectionString`)と、`DisableSetup=false`(元の `Web\user.config.sample` から)。

確認: `=== mojo BUILD OK ...` と出て、`experiments\wf4c\mojo\site\bin\mojoPortal.Web.dll` と `experiments\wf4c\mojo\site\user.config` があります。
`snapshot` の違いで失敗扱いになるときは [共通の準備の 10.8](setup.md#108-変換がdiffer-from-their-snapshotで失敗する)。

**Studio で**([使い方](setup.md#41-studio画面)):

| 欄 | 値 |
|---|---|
| プロジェクト | `<リポジトリ>\corpora\work\mojoportal-3.1.6\Web\mojoPortal.Web.csproj` |
| リポジトリのフォルダー | `<リポジトリ>\corpora\work\mojoportal-3.1.6` |
| 構成 | `Release` |
| ステップ 4 | 「元のアプリをそのビルド手順でビルドし、配置されるサイトから組み立てる」にチェック(自動判定で入ります) |

Studio は `user.config` を書きません。接続先は、ステップ 5 の「環境変数」で `APPSETTING_MSSQLConnectionString=...` として渡します(コンテナでの記録 2026-09-27 と同じやり方。Studio でこうして動かした記録はありません)。

**コマンドラインで(変換器を直接)**: `convert-corpora.ps1` と同じ値です。`user.config` は書かれません。

```powershell
.\converter.ps1 corpora\work\mojoportal-3.1.6\Web\mojoPortal.Web.csproj --out C:\work\mojo-out --root corpora\work\mojoportal-3.1.6 --build-original auto --culture-profile experiments\wf4c\_culture\culture-profile.json
```

### 3.3 DB を用意する

DB `mojo_w2l` は、初回の起動でセットアップ画面が作ります(記録と `convert-corpora.ps1` の説明による)。SQL Server Express を起動しておくだけです。

### 3.4 Windows で動かす

```powershell
cd experiments\wf4c\mojo\site
..\..\supervise.ps1 -Dll bin\mojoPortal.Web.dll -Port 5096
```

**http://localhost:5096/** を開くとセットアップ(`/Setup/Default.aspx`)が動き、終わるとトップ(`Home - mojoPortal`)になります。止め方は [共通の準備の 6.2](setup.md#62-止める)。

### 3.5 開くページ

| ページ | 見えるもの |
|---|---|
| `/Setup/Default.aspx` | 初回のセットアップ |
| `/` | トップ(タイトル `Home - mojoPortal`) |
| `/secure/LOGIN.aspx` | ログイン(大文字のまま開いても動く) |
| サイトマップ | 200 |
| 管理画面 | ログインへ転送される |

### 3.6 Linux で動かす

Docker Desktop を起動しておきます。サイトをそのままコンテナで動かし、SQL Server もコンテナ(`w2l-sql`)で動かします。DB `mojo_linux` を作り直します。

```powershell
.\experiments\wf4c\run-linux-site.ps1 -Site mojo\site -Dll mojoPortal.Web -Database mojo_linux -Paths '/Setup/Default.aspx', '/' -CaseInsensitive
```

確認: 開いた 2 つのページの状態コードとタイトルが出ます(セットアップのあと、トップが 200)。コンテナのログは `experiments\wf4c\_linux\mojo-site\container.log`。

変換が書いた Dockerfile で動かすときは、接続文字列を `APPSETTING_MSSQLConnectionString` で渡します([共通の準備の 7.1](setup.md#71-a-変換が書いた-dockerfile))。

## 4. 期待される結果

| 何を | 結果 | 出どころ |
|---|---|---|
| Windows | 空の DB からセットアップ → トップ・ログイン(`/secure/LOGIN.aspx` も)・サイトマップ 200、管理画面はログインへ | 記録 2026-10-11(Windows のトップ 200 は 2026-10-27 に再確認) |
| Linux(`run-linux-site.ps1`) | 同じ | 記録 2026-10-11 |
| コンテナ(変換の Dockerfile) | 接続文字列を `APPSETTING_MSSQLConnectionString` で渡してセットアップ → トップ・ログイン 200 | 記録 2026-09-27 |

記録のあとにパッケージの版が上がっています(今は `1.6.5-w2l.27`)。同じ結果になるはずですが、確かめてはいません。

## 5. 既知の差・制限

- SQLite の構成の mojo は Linux で `libsqlite3` が要ります(未確認。既定は SQL Server)。
- IIS で記録した正解は無いので、自動の比較はできません。ページを開いて確かめます。

## 6. うまくいかないとき

- **元のビルドが失敗する**: Visual Studio Build Tools が入っているか確かめます。ログの見方は [共通の準備の 10.2](setup.md#102-元のビルドが失敗する)。
- **セットアップ画面が DB につながらない**: SQL Server Express が動いているか、`experiments\wf4c\mojo\site\user.config` の `MSSQLConnectionString` が `.\SQLEXPRESS` の `mojo_w2l` を指しているかを見ます([共通の準備の 10.3](setup.md#103-sql-server-につながらない))。
- 共通のことは [共通の準備の 10](setup.md#10-うまくいかないとき)。

## 7. 片付け

```powershell
Get-NetTCPConnection -LocalPort 5096 -State Listen -ErrorAction SilentlyContinue | ForEach-Object { Stop-Process -Id $_.OwningProcess -Force }
docker stop w2l-sql                      # run-linux-site.ps1 が残す SQL Server
Stop-Service 'MSSQL$SQLEXPRESS'          # 管理者の PowerShell で
```

そのあと、Docker Desktop を止めて `wsl --shutdown`([共通の準備の 9](setup.md#9-後片付け))。
