# yaf(YAF.NET 3.2.15)

[共通の準備](setup.md) が終わっている前提です。コマンドは、とくに書いていなければリポジトリの一番上のフォルダーで実行します。
[コーパスの一覧に戻る](README.md)

このページの手順は記録(2026-10-27・10-29)によるものです。この手引きのためには動かしていません(コマンドはスクリプトと照らし合わせて確かめました)。

## 1. 概要

- **どんなアプリ**: 掲示板(Web Forms、C#)。
- **大きさ**: 取得 476 MB、変換の出力 0.8 GB。
- **いまの状態**: **変換とビルドは通りますが、`/` が 500 です**(「System.Data.SqlClient のプロバイダーが見つからない」。記録 2026-10-27・10-29)。動く状態にはなっていません。**試すなら変換とレポートまで** です。

## 2. 前提

| もの | 要るか |
|---|---|
| Visual Studio Build Tools(元のビルド) | 要らない |
| SQL Server Express | 起動まで試すなら。接続文字列は `.\SQLEXPRESS` の `yafnet` |
| インストーラー | アプリのインストーラー(Cookie が無いと毎回そこへ転送される) |
| ディスク | 取得 476 MB、変換の出力 0.8 GB |

## 3. 手順

### 3.1 取得する

元の GitHub のリポジトリから、次の版を取ります(このページの結果は、この版で出したものです)。

| | |
|---|---|
| リポジトリ | <https://github.com/YAFNET/YAFNET> |
| 版 | タグ `v3.2.15`(コミット `4b785802ce3bc688e2685fe624e554cf90824f89`、2026-08-09) |
| ライセンス | Apache License 2.0。[LICENSE.md](https://github.com/YAFNET/YAFNET/blob/v3.2.15/LICENSE.md) |
| フォルダーの名前 | `YAFNET-3.2.15`(この名前にしておくと、このページのパスがそのまま使えます) |

置く場所はどこでも構いません。ただし:

- **比較のスクリプトを使う場合は `corpora\work\YAFNET-3.2.15` に置きます**(`convert-corpora.ps1`、`corpora\build-original.ps1`、`convert-corpora.ps1` の出力を使う `verify-windows.ps1`・`run-linux.ps1` は、このパスを前提にしています)。
- パスの長さ: 元のビルド(NuGet の復元)のあと、フォルダーの中でいちばん深いファイルは 145 文字です。.NET Framework の MSBuild は Windows の MAX_PATH(260 文字)を超えるパスを扱えないので、**フォルダー自体のパスを 113 文字以内**にします(`<リポジトリ>\corpora\work\YAFNET-3.2.15` や `C:\corpora\YAFNET-3.2.15` なら収まります)。

**近道(`fetch.ps1`)**: 下と同じこと(ダウンロード、展開、フォルダーの名前)をして、`corpora\work` に置きます。取得元(リポジトリとタグ・コミット)も表示します。

```powershell
.\corpora\fetch.ps1 -Only yaf
```

確認: 「すべて取得できました。」と出て、`corpora\work\YAFNET-3.2.15` ができます。

**A. GitHub の画面から(ZIP)**

1. <https://github.com/YAFNET/YAFNET/tree/v3.2.15> を開き、「Code」→「Download ZIP」を押します(直接のリンク: <https://github.com/YAFNET/YAFNET/archive/refs/tags/v3.2.15.zip>)。
2. ZIP を展開します。中の `YAFNET-3.2.15` フォルダーがコーパスです。

**B. git で**(置きたい場所の一つ上のフォルダーで)

```powershell
git clone --filter=blob:none -c core.autocrlf=false -c core.eol=lf https://github.com/YAFNET/YAFNET.git YAFNET-3.2.15
git -C YAFNET-3.2.15 checkout 4b785802ce3bc688e2685fe624e554cf90824f89
```

`-c core.autocrlf=false -c core.eol=lf` は、改行を ZIP(と `fetch.ps1`)と同じにするためです。付けないと Git for Windows の既定(`core.autocrlf=true`)でテキストの改行が CRLF に変わり、ファイルの中身が ZIP と違ってしまいます(wt では 178 ファイル)。`.git` フォルダーが増えるのは構いません。
`--filter=blob:none` は、リポジトリが大きい(全体で約 760 MB)ので、そのコミットのファイルだけを取るためです。

**Studio・変換器に渡すフォルダー**: 「リポジトリのフォルダー」(`--root`)は置いたフォルダー(`YAFNET-3.2.15`)、「プロジェクト」はその中の `yafsrc\YetAnotherForum.NET\YAF-SqlServer.csproj` です。`corpora\work` 以外に置いたときは、3.2 以降の `corpora\work\YAFNET-3.2.15` を置いた場所に読み替えます。

### 3.2 変換する

**コマンドラインで(まとめて変換)**:

```powershell
.\experiments\wf4c\convert-corpora.ps1 -Only yaf
```

確認: `=== yaf BUILD OK ...`。変換レポート(`experiments\wf4c\yaf\CONVERSION-REPORT.md`)は 未解決 1、スタブ 43(下の「4. 期待される結果」)。
`snapshot` の違いで失敗扱いになるときは [共通の準備の 10.8](setup.md#108-変換がdiffer-from-their-snapshotで失敗する)。

**Studio で**([使い方](setup.md#41-studio画面)):

| 欄 | 値 |
|---|---|
| プロジェクト | `<リポジトリ>\corpora\work\YAFNET-3.2.15\yafsrc\YetAnotherForum.NET\YAF-SqlServer.csproj` |
| リポジトリのフォルダー | `<リポジトリ>\corpora\work\YAFNET-3.2.15` |
| 構成 | `Release` |

**コマンドラインで(変換器を直接)**:

```powershell
.\converter.ps1 corpora\work\YAFNET-3.2.15\yafsrc\YetAnotherForum.NET\YAF-SqlServer.csproj --out C:\work\yaf-out --root corpora\work\YAFNET-3.2.15 --build-original auto --culture-profile experiments\wf4c\_culture\culture-profile.json
```

### 3.3 DB・インストーラー

接続文字列は `.\SQLEXPRESS` の `yafnet` です。`/` が 500 になるので、インストーラーまでは進めません。

### 3.4 Windows で起動する(500 になるのを見るだけ)

```powershell
Start-Service 'MSSQL$SQLEXPRESS'      # 管理者の PowerShell で
cd experiments\wf4c\yaf\yafsrc\YetAnotherForum.NET
..\..\..\supervise.ps1 -Dll Bin\YAF.dll -Port 5096
```

- 出力のフォルダーは `Bin`(大文字の B)です。
- **http://localhost:5096/** を開くと 500(「System.Data.SqlClient のプロバイダーが見つからない」)。中身は `probe.log` にも出ます。
- 止め方は [共通の準備の 6.2](setup.md#62-止める)。

### 3.5 Linux

Linux の最後の記録(2026-10-11)は、インストーラーが止まるところまでです(その原因は後で直りましたが、再確認していません)。いまの状態では、Linux で試す手順はありません。

## 4. 期待される結果

| 何を | 結果 | 出どころ |
|---|---|---|
| 変換 | `BUILD OK`。変換レポート: 未解決 1(ServiceStack の `LicenseUtils.FromXml`。害は無い)、スタブ 43 | 記録 |
| Windows | **`/` が 500**(System.Data.SqlClient のプロバイダーが見つからない) | 記録 2026-10-27・10-29 |
| Linux | インストーラーが止まる(その原因は後で直った。再確認していない) | 記録 2026-10-11 |

## 5. 既知の差・制限

- `/` の 500 は既知です。動く状態にはなっていません。
- 経緯: 以前は Web API 2 の FieldAccessException(2026-10-22 に直った)、ServiceStack のスタブ(2026-10-27 に直った)で、もっと手前で止まっていました。Linux の最後の記録(2026-10-11)はその前のものです。
- IIS で記録した正解は無いので、自動の比較はできません。

## 6. うまくいかないとき

- **curl で試すと毎回インストーラーへ転送される**: Cookie を保ってください(Cookie が無いと、毎回インストーラーへ転送されます)。例: `curl.exe -s -c yaf.cookies -b yaf.cookies -o NUL -w "%{http_code}" http://localhost:5096/`
- **`bin\YAF.dll` が無い**: フォルダーは `Bin`(大文字の B)です。
- 共通のことは [共通の準備の 10](setup.md#10-うまくいかないとき)。

## 7. 片付け

```powershell
Get-NetTCPConnection -LocalPort 5096 -State Listen -ErrorAction SilentlyContinue | ForEach-Object { Stop-Process -Id $_.OwningProcess -Force }
Stop-Service 'MSSQL$SQLEXPRESS'          # 管理者の PowerShell で
```

[共通の準備の 9](setup.md#9-後片付け) も見てください。
