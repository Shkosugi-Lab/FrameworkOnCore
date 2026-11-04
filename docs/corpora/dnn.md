# dnn(DNN Platform 9.13.10)

[共通の準備](setup.md) が終わっている前提です。コマンドは、とくに書いていなければリポジトリの一番上のフォルダーで実行します。
[コーパスの一覧に戻る](README.md)

このページの手順は記録(2026-10-11)の手順です。この手引きのためには動かしていません(コマンドはスクリプトと照らし合わせて確かめました)。

## 1. 概要

- **どんなアプリ**: 大きな CMS(Web Forms、C# と VB のプロジェクト)。元のビルドは DNN 自身の Cake のビルドで、サイトはそのビルドが組み立てます。
- **大きさ**: 取得 165 MB、変換の出力 15 GB(何度か動かした残りを含む)、元のビルド 5.0 GB。
- **特別なこと**: 11 本の中でいちばん大きく、準備も時間もかかります。元のビルドには長い時間がかかります(npm のパッケージも作るため)。大文字・小文字の違うファイル名が多い(1,065 件)ので、Linux では大文字・小文字を区別しないで動かします。

## 2. 前提

| もの | 要るか |
|---|---|
| Visual Studio Build Tools(元のビルド) | **要る**([共通の準備の手順 6](setup.md#手順-6-visual-studio-build-tools-を入れる元のビルドが要るコーパスだけ)) |
| Node.js | 要らない(無ければ変換器が `%LOCALAPPDATA%\FrameworkOnCore\tools` に取ってくる) |
| SQL Server Express(`.\SQLEXPRESS`)と `sqlcmd` | **要る**。DB `dnn_w2l`(`convert-corpora.ps1` が `sqlcmd` で作る) |
| インストーラー | DNN のインストーラー(`/Install/Install.aspx?mode=install`)。`dnn-install.ps1` が無人で動かす |
| Docker Desktop | Linux で動かすとき。casefs([手順 11](setup.md#手順-11-casefs-をビルドするlinux-で動かすときだけ))も |
| ディスク | 出力と元のビルドで 20 GB 近く(取得 165 MB、変換の出力 15 GB、元のビルド 5.0 GB) |

## 3. 手順

### 3.1 取得する

元の GitHub のリポジトリから、次の版を取ります(このページの結果は、この版で出したものです)。

| | |
|---|---|
| リポジトリ | <https://github.com/dnnsoftware/Dnn.Platform> |
| 版 | タグ `v9.13.10`(コミット `53cdf4750477293b4e6316b4d91e1d5e43610127`、2025-11-22) |
| ライセンス | MIT License。[LICENSE](https://github.com/dnnsoftware/Dnn.Platform/blob/v9.13.10/LICENSE) |
| フォルダーの名前 | `Dnn.Platform-9.13.10`(この名前にしておくと、このページのパスがそのまま使えます) |

置く場所はどこでも構いません。ただし:

- **比較のスクリプトを使う場合は `corpora\work\Dnn.Platform-9.13.10` に置きます**(`convert-corpora.ps1`、`corpora\build-original.ps1`、`convert-corpora.ps1` の出力を使う `verify-windows.ps1`・`run-linux.ps1` は、このパスを前提にしています)。
- パスの長さ: 元のビルド(NuGet の復元)のあと、フォルダーの中でいちばん深いファイルは 162 文字です。.NET Framework の MSBuild は Windows の MAX_PATH(260 文字)を超えるパスを扱えないので、**フォルダー自体のパスを 96 文字以内**にします(`<リポジトリ>\corpora\work\Dnn.Platform-9.13.10` や `C:\corpora\Dnn.Platform-9.13.10` なら収まります)。

**近道(`fetch.ps1`)**: 下と同じこと(ダウンロード、展開、フォルダーの名前)をして、`corpora\work` に置きます。取得元(リポジトリとタグ・コミット)も表示します。

```powershell
.\corpora\fetch.ps1 -Only dnn
```

確認: 「すべて取得できました。」と出て、`corpora\work\Dnn.Platform-9.13.10` ができます。

**A. GitHub の画面から(ZIP)**

1. <https://github.com/dnnsoftware/Dnn.Platform/tree/v9.13.10> を開き、「Code」→「Download ZIP」を押します(直接のリンク: <https://github.com/dnnsoftware/Dnn.Platform/archive/refs/tags/v9.13.10.zip>)。
2. ZIP を展開します。中の `Dnn.Platform-9.13.10` フォルダーがコーパスです。

**B. git で**(置きたい場所の一つ上のフォルダーで)

```powershell
git clone --filter=blob:none -c core.autocrlf=false -c core.eol=lf https://github.com/dnnsoftware/Dnn.Platform.git Dnn.Platform-9.13.10
git -C Dnn.Platform-9.13.10 checkout 53cdf4750477293b4e6316b4d91e1d5e43610127
```

`-c core.autocrlf=false -c core.eol=lf` は、改行を ZIP(と `fetch.ps1`)と同じにするためです。付けないと Git for Windows の既定(`core.autocrlf=true`)でテキストの改行が CRLF に変わり、ファイルの中身が ZIP と違ってしまいます(wt では 178 ファイル)。`.git` フォルダーが増えるのは構いません。
`--filter=blob:none` は、リポジトリが大きい(全体で約 930 MB)ので、そのコミットのファイルだけを取るためです。

**Studio・変換器に渡すフォルダー**: 「リポジトリのフォルダー」(`--root`)は置いたフォルダー(`Dnn.Platform-9.13.10`)、「プロジェクト」はその中の `DNN Platform\Website\DotNetNuke.Website.csproj` です。`corpora\work` 以外に置いたときは、3.2 以降の `corpora\work\Dnn.Platform-9.13.10` を置いた場所に読み替えます。

### 3.2 変換する(元のビルドを含む)

**コマンドラインで(まとめて変換。こちらを勧めます)**:

```powershell
Start-Service 'MSSQL$SQLEXPRESS'      # 管理者の PowerShell で
.\experiments\wf4c\convert-corpora.ps1 -Only dnn
```

- 元のビルド(DNN の Cake のビルド)をしてから変換します。元のビルドは一度作ると次からは使い回します(作り直すときは `-Rebuild`)。
- 変換のあと、`convert-corpora.ps1` が DB `dnn_w2l` を作り(無ければ)、`experiments\wf4c\dnn\site\web.config` の `SiteSqlServer` をそこへ向けます(元の web.config は .NET の SqlClient が開けない、添付する DB のファイル `|DataDirectory|Database.mdf` を指しているため)。
- 「the build failed after it deployed the site: ... is used as it is」と出ることがあります。ビルドの一部(パッケージ作り)が失敗したが、サイトはできていたのでそれを使った、という意味です。そのまま進んで構いません。

確認: `=== dnn BUILD OK ...` と出て、`experiments\wf4c\dnn\site\bin\DotNetNuke.Website.dll` があります。
`snapshot` の違いで失敗扱いになるときは [共通の準備の 10.8](setup.md#108-変換がdiffer-from-their-snapshotで失敗する)。

**Studio で**([使い方](setup.md#41-studio画面)):

| 欄 | 値 |
|---|---|
| プロジェクト | `<リポジトリ>\corpora\work\Dnn.Platform-9.13.10\DNN Platform\Website\DotNetNuke.Website.csproj` |
| リポジトリのフォルダー | `<リポジトリ>\corpora\work\Dnn.Platform-9.13.10` |
| 構成 | `Release` |
| ステップ 4 | 「元のアプリをそのビルド手順でビルドし、配置されるサイトから組み立てる」にチェック(自動判定で入ります) |

Studio は DB を作らず、web.config も書き換えません。接続先はステップ 5 の「環境変数」で `SQLCONNSTR_SiteSqlServer=...` として渡します(この手引きでは確かめていません)。

**コマンドラインで(変換器を直接)**: `convert-corpora.ps1` と同じ値です。DB の作成と web.config の書き換えはされません。

```powershell
.\converter.ps1 "corpora\work\Dnn.Platform-9.13.10\DNN Platform\Website\DotNetNuke.Website.csproj" --out C:\work\dnn-out --root corpora\work\Dnn.Platform-9.13.10 --build-original auto --culture-profile experiments\wf4c\_culture\culture-profile.json
```

### 3.3 インストールする

```powershell
.\experiments\wf4c\dnn-install.ps1
```

- サイトを起動して(ポート 5096)、無人インストール(`Install/Install.aspx?mode=install`)を行います。**サイトは動かしたまま** 残ります(最後に `supervisor pid ... (left running)`)。
- インストールには数分かかります。
- このスクリプトを最近動かした記録はありません(2026-10-29 の確認はインストーラーの画面が 200 になるところまで)。うまくいかないときは、サイトを 3.4 のとおり起動して、ブラウザーで http://localhost:5096/Install/Install.aspx?mode=install を開きます。

確認: `install: 200 in ...` と、インストールの結果の画面の文字が出ます。

### 3.4 Windows で動かす

`dnn-install.ps1` のあとは、サイトが動いているので **http://localhost:5096/** を開くだけです。自分で起動するときは:

```powershell
cd experiments\wf4c\dnn\site
..\..\supervise.ps1 -Dll bin\DotNetNuke.Website.dll -Port 5096
```

止め方は [共通の準備の 6.2](setup.md#62-止める)(`dnn-install.ps1` が起動したサイトも同じコマンドで止めます)。

### 3.5 開くページとログイン

| ページ | 見えるもの |
|---|---|
| `/` | トップ |
| `/Login` | ログイン(インストールで作った host のユーザーでログインできる) |
| `/Terms` | 利用規約 |
| `/Privacy` | プライバシー |

### 3.6 Linux で動かす

Docker Desktop を起動しておきます。サイトをそのままコンテナで動かし、SQL Server もコンテナ(`w2l-sql`)で動かします。DB `dnn_linux` を作り直し、インストーラーを開いてからトップを開きます。

```powershell
.\experiments\wf4c\run-linux-site.ps1 -Site dnn\site -Dll DotNetNuke.Website -Database dnn_linux -Paths '/Install/Install.aspx?mode=install', '/' -CaseInsensitive
```

- `run-linux-site.ps1` はサイトをコンテナに写すので、Linux 側の実行は `dnn\site` を変えません。ただし、**Windows でインストールした後の `dnn\site` は、もうインストールに使えません**(下の「5. 既知の差・制限」)。
- 確認: 2 つのページの状態コードとタイトルが出ます。コンテナのログは `experiments\wf4c\_linux\dnn-site\container.log`。

## 4. 期待される結果

| 何を | 結果 | 出どころ |
|---|---|---|
| Windows | 空の DB からインストール完了、トップ・`/Login`・`/Terms`・`/Privacy` 200 | 記録 2026-10-11(インストーラーが 200 なのは 2026-10-29 に再確認) |
| Linux | 同じ。ページの CSS・JS・画像 21 件すべて 200、host でのログインが通る | 記録 2026-10-11 |

記録のあとにパッケージの版が上がっています(今は `1.6.5-w2l.27`)。同じ結果になるはずですが、確かめてはいません。

## 5. 既知の差・制限

- JWT の API(JwtController の 7 メソッド)は動きません(System.IdentityModel.Tokens.Jwt 4.0.2 が .NET Framework の System.IdentityModel に依存。5.0 以降の API への書き換えが要る)。
- インストーラーは `Install\*.aspx` を消します。Windows でインストールしたサイトのフォルダーは、もうインストールに使えません。Windows と Linux の両方で試すときは、変換直後の `dnn\site` を写しておいて戻すか、変換し直します。
- インストールや設定の変更の直後はアプリが再起動し、その間の要求は応答が無い(000・503)ことがあります。続けて要求すれば答えます。
- 大文字・小文字の違うファイル名が多い(1,065 件)ので、Linux では `-CaseInsensitive`(配置の既定)で動かします。
- 以前の Windows の一巡のスクリプト `dnn-cycle.ps1` は 2026-10-31 に削除されました(記録にその名前が出てきても使えません)。
- IIS で記録した正解は無いので、自動の比較はできません。

## 6. うまくいかないとき

- **元のビルドが失敗する・とても長い**: 長いのは npm のパッケージも作るためです。失敗したときは [共通の準備の 10.2](setup.md#102-元のビルドが失敗する)。
- **ディスクが足りない**: 出力と元のビルドで 20 GB 近く使います([共通の準備の 3](setup.md#3-ディスクの空き))。
- **インストールの直後に応答が無い(000・503)**: アプリの再起動中です。続けて要求すれば答えます。
- **インストールの画面が出ない(Install のページが無い)**: 前にインストールしたサイトです。変換直後の写しに戻すか、`.\experiments\wf4c\convert-corpora.ps1 -Only dnn` で変換し直します。
- **`dnn-install.ps1` がうまくいかない**: 3.3 のとおり、自分で起動してブラウザーでインストーラーを開きます。
- 共通のことは [共通の準備の 10](setup.md#10-うまくいかないとき)。

## 7. 片付け

```powershell
# Windows で起動したサイト(dnn-install.ps1 が残したものも)
Get-NetTCPConnection -LocalPort 5096 -State Listen -ErrorAction SilentlyContinue | ForEach-Object { Stop-Process -Id $_.OwningProcess -Force }
docker stop w2l-sql                      # run-linux-site.ps1 が残す SQL Server
Stop-Service 'MSSQL$SQLEXPRESS'          # 管理者の PowerShell で
```

そのあと、Docker Desktop を止めて `wsl --shutdown`([共通の準備の 9](setup.md#9-後片付け))。ディスクを空けるときは、`experiments\wf4c\dnn` と `experiments\wf4c\dnn.original-<日時>` を消します(次の変換で元のビルドからやり直しになります)。
