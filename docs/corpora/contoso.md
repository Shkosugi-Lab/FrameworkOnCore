# contoso(Contoso University、MVC 5)

[共通の準備](setup.md) が終わっている前提です。コマンドは、とくに書いていなければリポジトリの一番上のフォルダーで実行します。
[コーパスの一覧に戻る](README.md)

このページの手順は記録(Windows 2026-10-27、Linux 2026-10-22)によるものです。この手引きのためには動かしていません(コマンドはスクリプトと照らし合わせて確かめました)。

## 1. 概要

- **どんなアプリ**: EF6 のチュートリアルの大学のアプリ(MVC 5、C#。学生の一覧・作成・編集・削除、並べ替え・検索・ページ送り、日付の表示と入力)。
- **大きさ**: 取得 143 MB、変換の出力 0.6 GB。
- **特別なこと**: リポジトリにライセンスの表示が無いので、取得して使うだけです(再配布しない)。取得は、パスが長くなりすぎないようにソリューションのフォルダーだけを `ContosoUniversity` という名前で置きます。元の Web.config は LocalDB を指していますが、`fetch.ps1` が `.\SQLEXPRESS` に向け直します。

## 2. 前提

| もの | 要るか |
|---|---|
| SQL Server Express(`.\SQLEXPRESS`)と `sqlcmd` | **要る**。DB `ContosoUniversity1` はアプリが初期データ付きで作ります。`sqlcmd` は比較の前に DB を消すのに使います |
| Visual Studio Build Tools(元のビルド) | 要らない |
| インストーラー | 無い |
| Edge | 比較をするとき |
| Docker Desktop | Linux で動かすとき |
| ディスク | 取得 143 MB、変換の出力 0.6 GB |

## 3. 手順

### 3.1 取得する

元の GitHub のリポジトリの中の、ソリューションのフォルダー `ContosoUniversitySolution` を `ContosoUniversity` という名前で置きます(このページの結果は、この版で出したものです)。

| | |
|---|---|
| リポジトリ | <https://github.com/tigerbluejay/ContosoUniversity> |
| 版 | コミット `2ee1ad2aa66fbef012fe280e4552176489dcf0f3`(2025-04-13)の中のフォルダー `ContosoUniversitySolution` |
| ライセンス | **ライセンスの表示がありません**(LICENSE のファイルも、GitHub の表示も無し)。手元で試すだけにし、配り直さないでください |
| フォルダーの名前 | `ContosoUniversity`(この名前にしておくと、このページのパスがそのまま使えます) |

置く場所はどこでも構いません。ただし:

- **比較のスクリプトを使う場合は `corpora\work\ContosoUniversity` に置きます**(`convert-corpora.ps1`、`corpora\build-original.ps1`、`convert-corpora.ps1` の出力を使う `verify-windows.ps1`・`run-linux.ps1` は、このパスを前提にしています)。
- パスの長さ: 元のビルド(NuGet の復元)のあと、フォルダーの中でいちばん深いファイルは 145 文字です。.NET Framework の MSBuild は Windows の MAX_PATH(260 文字)を超えるパスを扱えないので、**フォルダー自体のパスを 113 文字以内**にします(`<リポジトリ>\corpora\work\ContosoUniversity` や `C:\corpora\ContosoUniversity` なら収まります)。
- この名前(`ContosoUniversity`)にするのは、パスを短くするためでもあります。ZIP のフォルダー名(`ContosoUniversity-<コミット>\ContosoUniversitySolution`)のままだと、リポジトリに入っている packages のパスが .NET Framework の MSBuild の上限(260 文字)を超え、元のビルドが「パッケージが無い」で失敗しました。

**近道(`fetch.ps1`)**: 下の A と同じこと(git での取得、フォルダーの名前、Web.config の接続文字列)をして、`corpora\work` に置きます。取得元(リポジトリとタグ・コミット)も表示します。

```powershell
.\corpora\fetch.ps1 -Only contoso
```

確認: 「接続文字列を SQL Server Express(.\SQLEXPRESS)に向けました」と「すべて取得できました。」が出ます。`git clone` で取るので Git が要ります。

**A. git で(すすめる)**: `fetch.ps1` と同じく、そのフォルダーだけを取ります。短いパスのフォルダー(`C:\src` など)で:

```powershell
git clone -c core.autocrlf=true --filter=blob:none --no-checkout --sparse https://github.com/tigerbluejay/ContosoUniversity.git ContosoUniversity-repo
git -C ContosoUniversity-repo sparse-checkout set ContosoUniversitySolution
git -C ContosoUniversity-repo checkout 2ee1ad2aa66fbef012fe280e4552176489dcf0f3
Move-Item ContosoUniversity-repo\ContosoUniversitySolution ContosoUniversity
Remove-Item ContosoUniversity-repo -Recurse -Force     # 残りは要らない
```

`-c core.autocrlf=true` は、改行を `fetch.ps1` と同じ(テキストは CRLF。比較の基準を作ったときのもの)にするためです。Git の設定が `core.autocrlf=true`(Git for Windows の既定)なら付けなくても同じです。

**B. GitHub の画面から(ZIP)**: <https://github.com/tigerbluejay/ContosoUniversity/tree/2ee1ad2aa66fbef012fe280e4552176489dcf0f3> の「Code」→「Download ZIP」(直接のリンク: <https://github.com/tigerbluejay/ContosoUniversity/archive/2ee1ad2aa66fbef012fe280e4552176489dcf0f3.zip>。約 60 MB)。**短いパスのフォルダー(`C:\src` など)に展開します**(ダウンロードのフォルダーの下では、パスが長すぎて展開に失敗することがあります)。展開した `ContosoUniversity-2ee1ad2aa66fbef012fe280e4552176489dcf0f3\ContosoUniversitySolution` を `ContosoUniversity` という名前で置きます。ZIP の改行は LF のままで、比較の基準(CRLF)と違います(190 ファイル。確かめた違いは改行だけ)。結果を記録と比べるなら A か `fetch.ps1` を使います。

**手で直すところ(Web.config の接続文字列)**: `ContosoUniversity\Web.config` の接続文字列は LocalDB(ユーザーごとのインスタンスで、IIS と変換後のアプリでは別の DB になる)を指しているので、SQL Server Express(`.\SQLEXPRESS`)に向けます。正解(IIS で記録したもの)を記録したときと同じ値です。`fetch.ps1` はこれを自動で行います。

`<add name="SchoolContext" connectionString="...">` の `connectionString` の値:

```text
元: Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=ContosoUniversity1;Integrated Security=SSPI;
後: Data Source=.\SQLEXPRESS;Initial Catalog=ContosoUniversity1;Integrated Security=True
```

**Studio・変換器に渡すフォルダー**: 「リポジトリのフォルダー」(`--root`)は置いたフォルダー(`ContosoUniversity`)、「プロジェクト」はその中の `ContosoUniversity\ContosoUniversity.csproj` です。`corpora\work` 以外に置いたときは、3.2 以降の `corpora\work\ContosoUniversity` を置いた場所に読み替えます。

### 3.2 変換する

**コマンドラインで(まとめて変換)**:

```powershell
.\experiments\wf4c\convert-corpora.ps1 -Only contoso
```

確認: `=== contoso BUILD OK ...`。出力は `experiments\wf4c\contoso`。
`snapshot` の違いで失敗扱いになるときは [共通の準備の 10.8](setup.md#108-変換がdiffer-from-their-snapshotで失敗する)。

**Studio で**([使い方](setup.md#41-studio画面)):

| 欄 | 値 |
|---|---|
| プロジェクト | `<リポジトリ>\corpora\work\ContosoUniversity\ContosoUniversity\ContosoUniversity.csproj` |
| リポジトリのフォルダー | `<リポジトリ>\corpora\work\ContosoUniversity` |
| 構成 | `Release` |

**コマンドラインで(変換器を直接)**:

```powershell
.\converter.ps1 corpora\work\ContosoUniversity\ContosoUniversity\ContosoUniversity.csproj --out C:\work\contoso-out --root corpora\work\ContosoUniversity --build-original auto --culture-profile experiments\wf4c\_culture\culture-profile.json
```

### 3.3 DB を用意する

SQL Server Express を起動しておくだけです。DB `ContosoUniversity1` はアプリが初期データ付きで作ります。

```powershell
Start-Service 'MSSQL$SQLEXPRESS'      # 管理者の PowerShell で
```

### 3.4 Windows で動かす

```powershell
cd experiments\wf4c\contoso\ContosoUniversity
..\..\supervise.ps1 -Dll bin\ContosoUniversity.dll -Port 5096
```

**http://localhost:5096/Student** を開きます。止め方は [共通の準備の 6.2](setup.md#62-止める)。

### 3.5 開くページ

| ページ | 見えるもの |
|---|---|
| `/Student` | 学生の一覧(並べ替え・検索・ページ送り、作成・編集・削除へ進める) |

### 3.6 元のアプリと比べる(Windows)

`supervise.ps1` を止めてから実行します。比べる前に DB を消します(EF が作り直します)。

```powershell
.\experiments\wf4c\verify-windows.ps1 -Name contoso
```

確認: 12 画面のうち 9 が一致(下の「4. 期待される結果」)。

### 3.7 Linux で動かす

Docker Desktop を起動しておきます。SQL Server もコンテナ(`w2l-sql`)で動かします。

```powershell
.\experiments\wf4c\run-linux.ps1 -App contoso\ContosoUniversity -Scenario corpora\regression\contoso.scenario.json -Golden corpora\parity\contoso.golden-webforms.json -SqlServer
```

確認: 12 画面のうち 9 が一致。

## 4. 期待される結果

| 何を | 結果 | 出どころ |
|---|---|---|
| Windows | 12 画面中 9 | 記録 2026-10-27 |
| Linux | 9/12 | 記録 2026-10-22 |

差の 3 つはすべて入力の検証のメッセージ(必須の入力。元は「EnrollmentDate フィールドが必要です。」、変換後は英語だった)で、[mvcmovie](mvcmovie.md) と同じ理由です。MVC 自身の文言と日付の表示は一致しています。
記録のあとにパッケージの版が上がっています(今は `1.6.5-w2l.27`)。

## 5. 既知の差・制限

- 入力の検証のメッセージの文言(4 のとおり)。

## 6. うまくいかないとき

- **DB につながらない**: 元の Web.config は LocalDB を指しています。`fetch.ps1` が `.\SQLEXPRESS` に向け直したかを見ます(取り直すときは `.\corpora\fetch.ps1 -Only contoso -Force`)。SQL Server Express は起動しておきます。
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

3.6・3.7 の比較が、元のアプリ(IIS で記録した正解 `corpora\parity\contoso.golden-webforms.json`、シナリオ `corpora\regression\contoso.scenario.json`)との比較です。すべての操作の比較はありません。
