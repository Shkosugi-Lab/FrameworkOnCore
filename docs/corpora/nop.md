# nop(nopCommerce 1.90)

[共通の準備](setup.md) が終わっている前提です。コマンドは、とくに書いていなければリポジトリの一番上のフォルダーで実行します。
[コーパスの一覧に戻る](README.md)

このページの手順は記録(2026-11-03)の手順です。この手引きのためには動かしていません(コマンドはスクリプトと照らし合わせて確かめました)。

## 1. 概要

- **どんなアプリ**: 最後の Web Forms 版の nopCommerce(EC サイト、C#、56 プロジェクト、Entity Framework 4、Ajax Control Toolkit)。
- **大きさ**: 取得 46 MB、変換の出力 1.5 GB、元のビルド 0.4 GB。
- **特別なこと**: いちばん大きな比較(すべての操作 727 件)があります。元のアプリをソリューションでビルドし、配置されるサイト(`NopCommerceStore`)から組み立てます。

## 2. 前提

| もの | 要るか |
|---|---|
| Visual Studio Build Tools(元のビルド) | **要る**([共通の準備の手順 6](setup.md#手順-6-visual-studio-build-tools-を入れる元のビルドが要るコーパスだけ)) |
| SQL Server Express(`.\SQLEXPRESS`) | **要る**。DB `nopCommerceOps`(`corpora\nop-database.ps1` が作る) |
| インストーラー | アプリのインストーラー(`/install/install.aspx`)でも DB を作れる |
| Edge | 比較をするとき |
| Docker Desktop | Linux で動かすとき。casefs([手順 11](setup.md#手順-11-casefs-をビルドするlinux-で動かすときだけ))も |
| ディスク | 取得 46 MB、変換の出力 1.5 GB、元のビルド 0.4 GB |

## 3. 手順

### 3.1 取得する

元の GitHub のリポジトリから、次の版を取ります(このページの結果は、この版で出したものです)。

| | |
|---|---|
| リポジトリ | <https://github.com/nopSolutions/nopCommerce> |
| 版 | タグ `release-1.90`(コミット `88f2c98d158a85b281b6a757e578e2f1cf00a2de`、2010-12-07) |
| ライセンス | nopCommerce Public License Version 1.0(この版の [NopCommerceStore/Licenses.txt](https://github.com/nopSolutions/nopCommerce/blob/release-1.90/NopCommerceStore/Licenses.txt) と各ソースの先頭)。リポジトリの今の LICENSE.md(NPL 3.0)は、この版には入っていません |
| フォルダーの名前 | `nopCommerce-release-1.90`(この名前にしておくと、このページのパスがそのまま使えます) |

置く場所はどこでも構いません。ただし:

- **比較のスクリプトを使う場合は `corpora\work\nopCommerce-release-1.90` に置きます**(`convert-corpora.ps1`、`corpora\build-original.ps1`、`convert-corpora.ps1` の出力を使う `verify-windows.ps1`・`run-linux.ps1`、`corpora\nop-database.ps1`、`corpora\record-nop-operations.ps1` は、このパスを前提にしています)。
- パスの長さ: 元のビルド(NuGet の復元)のあと、フォルダーの中でいちばん深いファイルは 121 文字です。.NET Framework の MSBuild は Windows の MAX_PATH(260 文字)を超えるパスを扱えないので、**フォルダー自体のパスを 137 文字以内**にします(`<リポジトリ>\corpora\work\nopCommerce-release-1.90` や `C:\corpora\nopCommerce-release-1.90` なら収まります)。

**近道(`fetch.ps1`)**: 下と同じこと(ダウンロード、展開、フォルダーの名前)をして、`corpora\work` に置きます。取得元(リポジトリとタグ・コミット)も表示します。

```powershell
.\corpora\fetch.ps1 -Only nop
```

確認: 「すべて取得できました。」と出て、`corpora\work\nopCommerce-release-1.90` ができます。

**A. GitHub の画面から(ZIP)**

1. <https://github.com/nopSolutions/nopCommerce/tree/release-1.90> を開き、「Code」→「Download ZIP」を押します(直接のリンク: <https://github.com/nopSolutions/nopCommerce/archive/refs/tags/release-1.90.zip>)。
2. ZIP を展開します。中の `nopCommerce-release-1.90` フォルダーがコーパスです。

**B. git で**(置きたい場所の一つ上のフォルダーで)

```powershell
git clone --filter=blob:none -c core.autocrlf=false -c core.eol=lf https://github.com/nopSolutions/nopCommerce.git nopCommerce-release-1.90
git -C nopCommerce-release-1.90 checkout 88f2c98d158a85b281b6a757e578e2f1cf00a2de
```

`-c core.autocrlf=false -c core.eol=lf` は、改行を ZIP(と `fetch.ps1`)と同じにするためです。付けないと Git for Windows の既定(`core.autocrlf=true`)でテキストの改行が CRLF に変わり、ファイルの中身が ZIP と違ってしまいます(wt では 178 ファイル)。`.git` フォルダーが増えるのは構いません。
`--filter=blob:none` は、リポジトリが大きい(全体で約 1690 MB)ので、そのコミットのファイルだけを取るためです。

**Studio・変換器に渡すフォルダー**: 「リポジトリのフォルダー」(`--root`)は置いたフォルダー(`nopCommerce-release-1.90`)、「プロジェクト」はその中の `NopCommerceStore\NopCommerceStore.csproj` です。`corpora\work` 以外に置いたときは、3.2 以降の `corpora\work\nopCommerce-release-1.90` を置いた場所に読み替えます。

### 3.2 変換する(元のビルドを含む)

**コマンドラインで(まとめて変換。こちらを勧めます)**:

```powershell
.\experiments\wf4c\convert-corpora.ps1 -Only nop
```

確認: `=== nop BUILD OK ...` と `snapshot: the same as the baseline`(nop の記録は今の版のものです)。`experiments\wf4c\nop\site\bin\NopCommerceStore.dll` があります。

**Studio で**([使い方](setup.md#41-studio画面)):

| 欄 | 値 |
|---|---|
| プロジェクト | `<リポジトリ>\corpora\work\nopCommerce-release-1.90\NopCommerceStore\NopCommerceStore.csproj` |
| リポジトリのフォルダー | `<リポジトリ>\corpora\work\nopCommerce-release-1.90` |
| 構成 | `Release` |
| ステップ 4 | 「元のアプリをそのビルド手順でビルドし、配置されるサイトから組み立てる」にチェック(自動判定で入ります) |

**コマンドラインで(変換器を直接)**: `convert-corpora.ps1` と同じ値です。

```powershell
.\converter.ps1 corpora\work\nopCommerce-release-1.90\NopCommerceStore\NopCommerceStore.csproj --out C:\work\nop-out --root corpora\work\nopCommerce-release-1.90 --build-original auto --culture-profile experiments\wf4c\_culture\culture-profile.json
```

### 3.3 DB を用意する

```powershell
Start-Service 'MSSQL$SQLEXPRESS'            # 管理者の PowerShell で
.\corpora\nop-database.ps1                  # サンプルのデータの DB nopCommerceOps を作る(初回は数分。2 回目からはバックアップから戻す)
```

サイトの `ConnectionStrings.config` を DB に向けます(インストーラーが書くのと同じ形。変換直後は中身の無い `<connectionStrings>` です):

```powershell
Set-Content experiments\wf4c\nop\site\ConnectionStrings.config '<connectionStrings><add name="NopSqlConnection" connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=nopCommerceOps;Integrated Security=True;Persist Security Info=False;MultipleActiveResultSets=True;Connect Timeout=120" /></connectionStrings>'
```

アプリのインストーラー(`/install/install.aspx`: SQL Server、新しい DB、サンプルデータ)で DB を作ることもできます。

### 3.4 Windows で動かす

```powershell
cd experiments\wf4c\nop\site
..\..\supervise.ps1 -Dll bin\NopCommerceStore.dll -Port 5096
```

**http://localhost:5096/** を開きます。止め方は [共通の準備の 6.2](setup.md#62-止める)。

### 3.5 開くページとログイン

- 店頭のトップ(`/`)、商品、カート、ログイン、管理画面。
- 管理者は **`admin@yourstore.com` / `admin`**。

### 3.6 Linux で動かす

nop の Linux での実行は、すべての操作の比較(下の「8. IIS の結果と比べる」)で行います。`run-linux.ps1` が、変換が組み立てたサイト(`nop\site`)をそのまま動かし、SQL Server のコンテナに DB を戻します。

```powershell
.\experiments\wf4c\run-linux.ps1 -App nop\NopCommerceStore -Suite operations   # 約 40 分
```

- 管理画面のグラフ(レポート)は、Linux では `APPSETTING_ChartImageHandler` で一時フォルダーを Linux のパスにします(web.config は `dir=c:\TempImageFiles\`)。
- 確認: `RESULT: OK`。

## 4. 期待される結果

| 何を | 結果 | 出どころ |
|---|---|---|
| Windows のすべての操作 | 1,000 スナップショット。一致 708・許容 292(操作: 一致 440、許容 278、実行できない 9)。`RESULT: OK` | 記録 2026-11-03(w2l.27) |
| Linux のすべての操作 | 一致 696・許容 304(操作: 一致 429、許容 289、実行できない 9)。`RESULT: OK` | 記録 2026-11-03(w2l.27) |

## 5. 既知の差・制限

- Ajax Control Toolkit が 4.1 から 20.1 になったことによる HTML の違い(モーダル ポップアップ、タブ、吹き出し)。動きは同じ。
- **Linux では PDF を作れません**(PdfSharp 1.3 が Windows の user32・gdi32 を直接呼ぶ)。変換レポートに `[Unsupported]` と出ます。ほかに Linux だけ: 縮小画像の JPEG・PNG のバイト、タイムゾーンの ID、OLE DB。
- 実行できない 9 件: PayPal などの外部のサービス、Jet/Excel の取り込み、DB のバックアップ、インストーラーそのもの。
- グラフ(管理画面のレポート)は Linux では `APPSETTING_ChartImageHandler` で一時フォルダーを Linux のパスにします。

## 6. うまくいかないとき

- **DB につながらない**: `experiments\wf4c\nop\site\ConnectionStrings.config` が 3.3 のとおりか、SQL Server Express が動いているかを見ます([共通の準備の 10.3](setup.md#103-sql-server-につながらない))。
- **データを最初の状態に戻したい**: `.\corpora\nop-database.ps1`(バックアップから戻す)。
- **元のビルドが失敗する**: [共通の準備の 10.2](setup.md#102-元のビルドが失敗する)。
- 共通のことは [共通の準備の 10](setup.md#10-うまくいかないとき)。

## 7. 片付け

```powershell
Get-NetTCPConnection -LocalPort 5096 -State Listen -ErrorAction SilentlyContinue | ForEach-Object { Stop-Process -Id $_.OwningProcess -Force }
.\corpora\nop-database.ps1 -Drop            # DB を消すとき
# ConnectionStrings.config を元に戻す(変換直後は中身の無い <connectionStrings>。元のリポジトリのものと同じ)
Copy-Item corpora\work\nopCommerce-release-1.90\NopCommerceStore\ConnectionStrings.config experiments\wf4c\nop\site\ConnectionStrings.config
docker stop w2l-sql                         # run-linux.ps1 が残す SQL Server
Stop-Service 'MSSQL$SQLEXPRESS'             # 管理者の PowerShell で
```

そのあと、Docker Desktop を止めて `wsl --shutdown`、Edge を閉じます([共通の準備の 9](setup.md#9-後片付け))。

## 8. IIS の結果と比べる(任意)

すべての操作(727 件)を元のアプリ(IIS で記録した正解)と比べます。DB の用意から後片付けまでスクリプトが行います(SQL Server Express の起動と停止も)。

```powershell
.\experiments\wf4c\verify-windows.ps1 -Name nop -Suite operations      # 約 33 分
.\experiments\wf4c\run-linux.ps1 -App nop\NopCommerceStore -Suite operations   # 約 40 分
```

- 正解: `corpora\parity\nop.operations.golden.json`(1,000 スナップショット)、許される差 `nop.operations.allowed.json`。結果は `experiments\wf4c\_results\nop.operations.windows.{md,json}`。
- nop には画面の比較(`-Suite` を付けない比較)の正解はありません。
- 期待される結果は「4. 期待される結果」。詳しくは [tests/README.md](../../tests/README.md) の「4. コーパス」。
