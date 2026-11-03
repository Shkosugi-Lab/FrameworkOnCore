# コーパスで変換を試す手引き

この手引きは、FrameworkOnCore の変換を **自分の手で試したい人** のためのものです。
ASP.NET(Web Forms・MVC)は分かるけれど、このリポジトリの中身は知らない、という人を想定しています。

「コーパス」は、変換の確かめに使っている実在のオープンソースの .NET Framework の Web アプリです(11 本)。
ここでは、コーパスを取ってきて、変換し、Windows と Linux で動かすまでを、コーパスごとに書きます。

- **実際にこの手引きのとおりに動かして確かめたもの**(2026-11-04): be(BlogEngine.NET)と wt(WingtipToys)の取得 → 変換 → Windows で起動・比較 → Linux(Docker)で起動・比較、Studio の画面。
- **それ以外のコーパス**: コマンドはスクリプトと照らし合わせて確かめましたが、この手引きのために動かしてはいません。結果は記録(`experiments/wf4c/README.md`、`tests/README.md`)の、日付つきの最新のものです。

目次

1. [共通の準備](#1-共通の準備)
2. [変換のしかた(2 通り)](#2-変換のしかた2-通り)
3. [動かしかた](#3-動かしかた)
4. [コーパスごと](#4-コーパスごと)
5. [うまくいかないとき](#5-うまくいかないとき)
6. [自分のアプリで試すとき](#6-自分のアプリで試すとき)

---

## 1. 共通の準備

### 1.1 入れるもの

| もの | 要るとき | メモ |
|---|---|---|
| Windows 10 / 11(x64) | いつも | 変換は Windows で行います(元のアプリのビルドに Windows の MSBuild を使うため) |
| .NET 10 SDK | いつも | `dotnet --list-sdks` に `10.0.xxx` が出ること |
| Git | いつも | `mvcmovie`・`contoso` の取得(`git clone`)と、Studio・変換器のビルドの取得(コミットを調べる)に使います |
| Windows PowerShell 5.1 | いつも | Windows に入っています。スクリプトが動かないときは「[5.1 スクリプトが実行できない](#51-スクリプトが実行できない)」 |
| SQL Server Express(インスタンス名 `SQLEXPRESS`)と `sqlcmd` | wt、mojo、yaf、dnn、imis、nop、nop390、mvcmovie、contoso | Windows 認証で使います。`.\SQLEXPRESS` という名前を前提にしています。be と n2 は要りません |
| Visual Studio Build Tools 2022 以降(Web のビルド ツール、.NET Framework 4.8 の Targeting Pack) | 元のアプリのビルドが要るコーパス: mojo、dnn、n2、imis、nop、nop390 | be・wt・mvcmovie・contoso では要りません。入れ方は下 |
| Microsoft Edge | 元のアプリとの比較(`verify-windows.ps1`、`run-linux.ps1`) | Windows に入っています。比較の道具が画面の無い Edge を動かします |
| Docker Desktop(WSL 2) | Linux で動かすとき | |

Visual Studio Build Tools は、変換器が足りないときに出すメッセージと同じコマンドで入れられます(管理者の PowerShell で):

```powershell
winget install Microsoft.VisualStudio.2022.BuildTools --override "--quiet --wait --add Microsoft.VisualStudio.Workload.WebBuildTools --add Microsoft.VisualStudio.Workload.MSBuildTools --add Microsoft.Net.Component.4.8.TargetingPack --includeRecommended"
```

要らないもの:

- **Node.js**: DNN の元のビルドが使いますが、無ければ変換器が自分のキャッシュ(`%LOCALAPPDATA%\FrameworkOnCore\tools`)に取ってきます。
- **IIS**: 元のアプリの正解の記録(`record-*.ps1`)を採り直すときだけ要ります。正解はリポジトリに入っているので、試すだけなら要りません。
- **nuget.exe、.NET Framework の参照アセンブリ**: 変換器が自動で取ってきます。

### 1.2 リポジトリを取る

```powershell
git clone https://github.com/Shkosugi-Lab/FrameworkOnCore.git C:\src\FrameworkOnCore
cd C:\src\FrameworkOnCore
```

この手引きのコマンドは、すべて **リポジトリの一番上のフォルダー** で実行します。
フォルダーの場所は短いほうが安全です(.NET Framework の MSBuild には 260 文字のパスの上限があります)。

### 1.3 コーパスを取る

```powershell
.\corpora\fetch.ps1 -Only be,wt          # 試すものだけ
.\corpora\fetch.ps1                      # 全部(12 個。imis の DB の imisdb を含む)
```

- 取得先は `corpora\work\`(Git には入りません)。取得済みのものは飛ばします。取り直すときは `-Force`。
- 名前: `be`、`wt`、`mojo`、`yaf`、`dnn`、`n2`、`imis`、`imisdb`、`nop`、`mvcmovie`、`nop390`、`contoso`。
- wt・mvcmovie・contoso は、取得のときに Web.config の接続文字列を SQL Server Express(`.\SQLEXPRESS`)に向けます(元の wt は存在しない Azure SQL を、mvcmovie・contoso は LocalDB を指しているため。正解を記録したときと同じ値です)。
  「接続文字列を SQL Server Express(.\SQLEXPRESS)に向けました」と出ます。

成功すると、最後に「すべて取得できました。」と出ます。

### 1.4 FrameworkOnCore のパッケージ

変換後のアプリが使う FrameworkOnCore のパッケージ(`FrameworkOnCore.*`)は、初めて解析や変換をしたときに GitHub Release から `experiments\wf4c\_feed` に **自動で** 取ってきます。
Studio と変換器(`studio.ps1`・`converter.ps1`)も、GitHub Actions がビルドしたものを自動で取ってきます(`%LOCALAPPDATA%\FrameworkOnCore\builds`)。準備は要りません。

### 1.5 ディスクの空き

このマシン(2026-11-04)での実測です。変換の出力には、元のリポジトリの写しとビルドの結果が入ります。
「元のビルド」は、元のアプリをそのビルド手順でビルドした写し(`experiments\wf4c\<名前>.original-<日時>`)です。

| コーパス | 取得したもの(`corpora\work`) | 変換の出力(`experiments\wf4c\<名前>`) | 元のビルド |
|---|---|---|---|
| be | 94 MB | 0.6 GB | ― |
| wt | 348 MB | 0.6 GB | ― |
| mojo | 161 MB | 4.1 GB | 0.7 GB |
| yaf | 476 MB | 0.8 GB | ― |
| dnn | 165 MB | 15 GB(何度か動かした残りを含む) | 5.0 GB |
| n2 | 66 MB | 5.1 GB | 1.2 GB |
| imis | 115 MB(DB のスクリプト 5 MB は別) | 1.5 GB | 0.6 GB |
| nop | 46 MB | 1.5 GB | 0.4 GB |
| mvcmovie | 97 MB | 0.6 GB | ― |
| nop390 | 334 MB | 2.0 GB | 0.8 GB |
| contoso | 143 MB | 0.6 GB | ― |

Linux で動かすときは、さらに Docker のイメージ(変換したアプリ 1 つで約 1.1 GB。be 1.15 GB、wt 1.13 GB)と、.NET SDK・ASP.NET・SQL Server のイメージで数 GB 使います。
Docker のビルドのキャッシュは使うたびに増えます(「[5.6 ディスクがいっぱい](#56-ディスクがいっぱいdocker)」)。

---

## 2. 変換のしかた(2 通り)

| | Studio(画面) | コマンドライン |
|---|---|---|
| 向いているとき | 初めて試す、自分のアプリを試す、何が変わるか見たい | コーパスをまとめて変換する、何度も変換する |
| 出力の場所 | `%LOCALAPPDATA%\FrameworkOnCore\studio\<解析の ID>\conversion\out` | `experiments\wf4c\<名前>`(`convert-corpora.ps1`)、または `--out` で指定 |

### 2.1 Studio(画面)

```powershell
.\studio.ps1
```

「Studio: http://127.0.0.1:5300/」と出たら、ブラウザーで **http://127.0.0.1:5300/** を開きます(この PC からだけ開けます)。
止めるときは、`studio.ps1` を動かしている PowerShell で Ctrl+C。Studio が起動したアプリも一緒に止まります。

左に解析の一覧、まん中に 5 つの手順(ステップ)が並びます。どの手順にも戻れます。

1. **新しい解析**(右上の「+ 新しい解析」)
   - 「プロジェクト(.csproj / .vbproj)」: Web プロジェクトのファイル。例(be): `C:\src\FrameworkOnCore\corpora\work\BlogEngine.NET-3.3.8.0\BlogEngine\BlogEngine.NET\BlogEngine.NET.csproj`
   - 「リポジトリのフォルダー」: 空なら「.sln のある一番上のフォルダー」になります。**be はここに `corpora\work\BlogEngine.NET-3.3.8.0` を指定してください**(be は `.sln` のフォルダーの外の `lib` の DLL を参照していて、空のままだと変換でその参照が失われ、「未解決」が 16 件出ます。コマンドラインの `convert-corpora.ps1` は指定済みです)。コーパスごとの値は「[4. コーパスごと](#4-コーパスごと)」の表にあります。
   - 「構成」: ふつうは `Release`。imis は `DemoRelease`。
   - 「解析する」を押します。
2. **ステップ 1 読み込み**: プロジェクトの構成(言語、.NET Framework の版、NuGet パッケージの数、ページの数など)と、「リポジトリのビルド手順」(元のビルドが要るかの自動判定)が出ます。「元のアプリを起動(テスト起動)」で、変換の前のアプリをこの PC で動かすこともできます(IIS Express、無ければ IIS)。
3. **ステップ 2 解析と対応**: 解析が終わると(be で 10 秒ほど)、「概要」に .NET Framework の API の数、「変換で対応する API」「自分で対応が要る部品」「対応を選ぶ部品」の数が出ます。下のタブ「自分で対応する所」「対応を選ぶ部品」「すべての API」で中身を見て、部品ごとに対応を選べます。**既定のままで変換できます。**
4. **ステップ 3 サーバーの設定**: ファイル名の大文字・小文字(Linux でも区別しない〈既定〉/ 区別する)、アクセス ログ、URL Rewrite、エラー ページ、圧縮などの IIS の機能の動かし方です。既定(「IIS 互換」)は元の IIS と同じ動きです。
5. **ステップ 4 変換・ビルド**: 「変換してビルド」を押します。元のビルドが要るアプリ(DNN など)は「元のアプリをそのビルド手順でビルドし、配置されるサイトから組み立てる」にチェックが入ります(自動判定。理由も出ます)。「追加の手順」は n2 のように、元のビルドの後にリポジトリのセットアップが要るときに書きます。
   終わると「ビルド成功」と、下に「変換レポート」(節ごとに開ける)が出ます。be で 1 分ほどです。
6. **ステップ 5 実行・デプロイ**:
   - 「ZIP ダウンロード」: 変換の出力(サイト、Dockerfile、systemd 用のスクリプト)。配置の手順は ZIP の中の `deploy/README.md`。
   - 「ネイティブ起動(この PC)」: 「起動」で、この PC の .NET 10 で動かします(Docker は要りません。be ではポート 8100 で起動しました)。
   - 「コンテナ起動(Linux / Docker)」: Dockerfile からイメージを作ってコンテナで動かします。Docker Desktop が止まっていると「Docker Desktop を起動」ボタンが出ます。コンテナは一度に一つです。
   - どちらも「環境変数」の欄で `SQLCONNSTR_<接続文字列の名前>=...`、`APPSETTING_<キー>=...` を渡せます(web.config を書き換えずに接続先を変える)。コンテナの中からこの PC の SQL Server へは `host.docker.internal` を使います(LocalDB や `localhost` は見えません)。
   - 「変換のコマンドをコピー」で、同じ変換をするコマンドラインが取れます。

### 2.2 コマンドライン: コーパスをまとめて変換する(`convert-corpora.ps1`)

```powershell
.\experiments\wf4c\convert-corpora.ps1 -Only be,wt
```

- コーパスごとの Web プロジェクト、リポジトリのフォルダー(`--root`)、元のビルドの要否、構成(`--configuration`)、元のビルドの後の手順(`--original-step`)、カルチャのデータ(`--culture-profile experiments\wf4c\_culture\culture-profile.json`。正解を記録したマシンのもの)は、スクリプトが知っています。
- 元のビルドは一度作ると次からは使い回します(数分かかるため)。作り直すときは `-Rebuild`。
- mojo・dnn・imis では、このマシンの SQL Server Express を使うように接続文字列も書きます(4 章)。
- 変換器の記録: `experiments\wf4c\_linux\foc-<名前>.log`

出力の例(be・wt、2026-11-04 に実行。1 分 36 秒):

```
=== be BUILD OK (rounds: 4) 未解決(変換で解決できなかったもの)(0 件) / パッケージの大きな版上げ(...)(2 件) / IIS の機能(...)(3 件) / .NET に無い、または Windows 専用の API(37 件) / コンパイルのために変えたソース(スタブ・削除)(23 件) / Linux で動かすために変えたソース(Windows のパス)(198 件) / プロジェクトとパッケージ(16 件)
  snapshot: the same as the baseline (936 entries)
=== wt BUILD OK (rounds: 2) 未解決(...)(0 件) / ... / コンパイルのために変えたソース(スタブ・削除)(0 件) / ...
  snapshot: the same as the baseline (299 entries)
```

- `BUILD OK` なら変換とビルドが通っています。
- `snapshot: the same as the baseline` は、変換の判断が記録(`corpora\regression\conversion-baseline\<名前>.json`)と同じという意味です。違いがあると最後にスクリプトが失敗し、`experiments\wf4c\_linux\snapshot-diff-<名前>.txt` に違いが出ます。
  **be・wt・nop 以外の記録は前の版(パッケージ `1.6.5-w2l.25`)のとき** のものなので、版の文字列の違いで失敗扱いになることがあります。そのときも `BUILD OK` なら変換の結果は使えます。
- `REGRESSION`(未解決やスタブが記録より増えた)と出たら、変換器かコーパスに変化があります。`CONVERSION-REPORT.md` の「未解決」を見てください。

### 2.3 コマンドライン: 変換器を直接使う(`converter.ps1`)

自分のアプリや、1 本だけを変換するときに使います。

```powershell
# 元のビルドが要るか(理由も出る)
.\converter.ps1 build-original-advice <Web プロジェクト>

# 解析だけ(api-analysis.json、API-ANALYSIS.md、既定の選択の foc-choices.json)
.\converter.ps1 analyze <Web プロジェクト> --out <出力先>

# 変換(--choices は Studio や analyze が書いた選択。省略すると既定)
.\converter.ps1 <Web プロジェクト> --out <出力先> [--root <リポジトリのフォルダー>] [--build-original auto] [--configuration <構成>] [--choices foc-choices.json] [--culture-profile <file>] [--iis-profile <file>]
```

例(be。2026-11-04 に実行、約 1 分):

```powershell
.\converter.ps1 corpora\work\BlogEngine.NET-3.3.8.0\BlogEngine\BlogEngine.NET\BlogEngine.NET.csproj --out C:\work\be-out --root corpora\work\BlogEngine.NET-3.3.8.0 --build-original auto
```

最後に `build succeeded; web project: ...; report: <出力先>\CONVERSION-REPORT.md` と出れば成功です。
`--culture-profile` を付けないと、この PC のカルチャのデータを採って使います(元のサーバーのものがあればそちらが正確です。「[6. 自分のアプリで試すとき](#6-自分のアプリで試すとき)」)。

### 2.4 変換レポート(`CONVERSION-REPORT.md`)の読み方

出力のフォルダーの一番上にあります(Studio ではステップ 4 の「変換レポート」)。見出しの後ろの「(n 件)」が件数です。上から順に大事です。

| 節 | 意味 | どうするか |
|---|---|---|
| 未解決(変換で解決できなかったもの) | 変換で直せなかった所。とくに「stubbed because of the conversion, not of .NET」は、変換が参照を失った(パスが外れた、など)ことを示します | **0 件が目標**。1 件でもあれば、まず理由を読みます(be で「リポジトリのフォルダー」を指定し忘れたときはここに 16 件出ます) |
| パッケージの大きな版上げ | .NET 10 で動く版に上げたパッケージ(例: Newtonsoft.Json 6.0.8 → 13.0.4)と、動きの違い | 書かれた違いがアプリに関係するか確かめます |
| IIS の機能 | 元のサーバーと web.config が使う IIS の機能と、変換後に「対応」しているか | 「未対応」があれば、その機能は変換後に効きません |
| .NET に無い、または Windows 専用の API | .NET 10 に無い・Windows でしか動かない API と、何で補ったか。`SYSLIB....` は .NET で廃止予定・実行時に例外になるもの | その機能を使う画面を実際に開いて確かめます |
| コンパイルのために変えたソース(スタブ・削除) | ビルドを通すために、本体を「例外を投げる」形にしたメンバーや、外した using など | **その機能は変換後に動きません**。使う機能なら、自分で書き換えます |
| Linux で動かすために変えたソース(Windows のパス) | `\` 区切りのパスなどを、Linux でも動く形に書き換えた所 | ふつうは何もしません |
| プロジェクトとパッケージ | プロジェクトの形式、パッケージの置き換え、DLL の参照の付け替え、配置(Dockerfile など)について変換器が決めたこと | 気になる所だけ読みます |

各行の先頭の `[Unsupported]`、`[Error]`、`[Raised]`、`[Project]` は、変換器の記録(ログ)の中の種類です。

---

## 3. 動かしかた

### 3.1 Windows で動かす

変換したアプリは、`bin\<Web プロジェクトのアセンブリ>.dll` を `dotnet` で起動します。**サイトのフォルダーで** 起動してください。

サイトのフォルダー:

- 元のビルドから組み立てたコーパス(mojo、dnn、n2、imis、nop、nop390): `experiments\wf4c\<名前>\site`
- それ以外: 出力の中の Web プロジェクトのフォルダー(be は `experiments\wf4c\be\BlogEngine\BlogEngine.NET`)

アプリは web.config や bin が変わると「終了コード 75」で終わり、作り直されることを期待します(IIS のアプリの再起動に当たる)。
そのため、`supervise.ps1`(終了コード 75 なら起動し直す)を通して起動します。

```powershell
cd experiments\wf4c\be\BlogEngine\BlogEngine.NET
..\..\..\supervise.ps1 -Dll bin\BlogEngine.NET.dll -Port 5096
# ログはこのフォルダーの probe.log
```

別の PowerShell かブラウザーで **http://localhost:5096/** を開きます。

- 最初の要求は、ページのコンパイルで 10 秒ほどかかります。
- 最初の要求のあとアプリが 1 回再起動することがあります(ログに `application restart (ConfigurationChange)`)。その間の要求は待たされるか、応答がありません。もう一度開けば答えます。
- 止めるとき: `supervise.ps1` の PowerShell で Ctrl+C のあと、ポートを持っているプロセスも止めます(再起動した作業プロセスが残るため)。

```powershell
Get-NetTCPConnection -LocalPort 5096 -State Listen -ErrorAction SilentlyContinue | ForEach-Object { Stop-Process -Id $_.OwningProcess -Force }
```

#### 元のアプリと比べる(`verify-windows.ps1`)

正解(元のアプリを IIS で動かして記録した画面)のあるコーパス(be、wt、mvcmovie、contoso、nop)は、変換したアプリを起動して、画面を正解と比べられます。起動から停止までスクリプトが行います(ポート 5096)。

```powershell
.\experiments\wf4c\verify-windows.ps1 -Name be
```

最後に `RESULT: OK (5 スナップショットすべて変換前と一致、リンク 17 件も一致)` のように出れば一致です。`RESULT: FAIL` なら、違った画面の名前と違いが出ます。

### 3.2 Linux で動かす

Linux では Docker のコンテナで動かします。やり方は 3 つあります。

| やり方 | 何をする | 向いているとき |
|---|---|---|
| A. 変換が書いた `Dockerfile` | 配置と同じイメージを作って動かす | 本番の配置を試す |
| B. `run-linux.ps1` | 出力をコンテナの中でビルドして動かし、正解と比べる(be、wt、mvcmovie、contoso、nop) | 元と同じに動くかを確かめる |
| C. `run-linux-site.ps1` | 組み立てたサイト(`<名前>\site`)をそのままコンテナで動かし、指定したページを開く。SQL Server もコンテナで | DB やインストーラーの要るコーパス(mojo、dnn、n2、imis) |

始める前に Docker Desktop を起動しておきます。終わったら「[3.3 後片付け](#33-後片付け)」をしてください。

#### A. 変換が書いた Dockerfile(be の例)

出力のフォルダー(`Dockerfile` のある所)で:

```powershell
cd experiments\wf4c\be
docker build -t blogengine-net .
docker run -d -p 8080:8080 --name blogengine-net -v blogengine-net-data:/app/App_Data blogengine-net
```

- イメージ作り: be で約 1 分(2026-11-04)。
- **http://localhost:8080/** を開きます。`docker logs blogengine-net` に `Now listening on: http://[::]:8080` が出ていれば起動しています。
- Linux でもファイル名の大文字・小文字を区別しません(既定)。be では `/POST/Welcome-To-BlogEngine-NET` も正しいページへ転送されました。
- カルチャ(`LANG=ja_JP.UTF-8`)とタイムゾーン(`TZ=Asia/Tokyo`)は、カルチャのデータ(元のサーバーのもの)から Dockerfile に書かれています。
- 詳しい説明(環境変数、systemd での配置)は出力の `deploy\README.md` にあります。

DB を使うアプリは、接続文字列を環境変数で渡します。wt の例(SQL Server もコンテナで動かす。2026-11-04 に実行):

```powershell
docker network create foc-net
docker run -d --name foc-sql --network foc-net -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=Foc_local_Passw0rd" mcr.microsoft.com/mssql/server:2022-latest
cd experiments\wf4c\wt
docker build -t wingtiptoys .
$cs = 'Data Source=foc-sql;Initial Catalog=WingtipToys;User ID=sa;Password=Foc_local_Passw0rd;TrustServerCertificate=True;MultipleActiveResultSets=True'
docker run -d -p 8080:8080 --name wingtiptoys --network foc-net -e "SQLCONNSTR_DefaultConnection=$cs" -e "SQLCONNSTR_WingtipToys=$cs" wingtiptoys
```

(SQL Server が起動するまで 10〜20 秒かかります。wt は最初の要求で DB を作ってデータを入れます。)

#### B. `run-linux.ps1`(正解と比べる)

```powershell
.\experiments\wf4c\run-linux.ps1 -App be\BlogEngine\BlogEngine.NET -Scenario corpora\regression\be.scenario.json -Golden corpora\parity\be.golden-webforms.json
.\experiments\wf4c\run-linux.ps1 -App wt\WingtipToys\WingtipToys -Scenario corpora\regression\wt.scenario.json -Golden corpora\parity\wt.golden-webforms.json -SqlServer
```

- `-App` は `experiments\wf4c` から見た Web プロジェクトのフォルダーです。
- `-SqlServer`: SQL Server もコンテナ(`w2l-sql`、次の実行のために残す)で動かし、web.config の `.\SQLEXPRESS` などをそこへ向けます。
- 終わるとアプリのコンテナは消えます(`-Keep` で残す)。結果は `RESULT: OK ...` / `RESULT: FAIL`。

#### C. `run-linux-site.ps1`(サイトをそのまま動かす)

```powershell
.\experiments\wf4c\run-linux-site.ps1 -Site <名前>\site -Dll <アセンブリ名> [-Database <DB 名>] [-Paths '/', ...] [-CaseInsensitive] [-Keep]
```

- ASP.NET のイメージにサイトを写して起動し、`-Paths` を順に開いて状態とタイトルを出します(ポート 5098)。
- `-Database` を付けると SQL Server のコンテナ(`w2l-sql`)にその DB を作り直し、サイトの `.config` の接続文字列をそこへ向けます(サイトのフォルダーは書き換えません)。
- `-CaseInsensitive`: Linux でもファイル名の大文字・小文字を区別しない(配置の既定と同じ)。`experiments\wf4c\casefs\out` が要ります(下)。
- `-Keep`: 終わってもコンテナを残す(ブラウザーで http://localhost:5098/ を開いて触れる)。消すときは `docker rm -f w2l-site-<名前>`。
- コンテナのログ: `experiments\wf4c\_linux\<名前>-site\container.log`

**ファイル名の大文字・小文字のライブラリ**(`libfoccase.so`)は Git に入っていません。新しいマシンでは、変換の前に一度ビルドしておくと、Dockerfile にも入ります(Docker が要ります):

```powershell
.\experiments\wf4c\casefs\build.ps1
```

### 3.3 後片付け

メモリーとディスクのために、使い終わったら止めます。

```powershell
# 自分で起動したコンテナ(名前は docker ps -a で確かめる)
docker rm -f blogengine-net wingtiptoys foc-sql
docker stop w2l-sql                      # run-linux.ps1 -SqlServer / run-linux-site.ps1 が残す SQL Server
docker network rm foc-net
# Docker Desktop を止め、WSL のメモリーを返す
Stop-Process -Name 'Docker Desktop','com.docker.backend' -Force -ErrorAction SilentlyContinue
wsl --shutdown
# SQL Server Express(管理者の PowerShell)
Stop-Service 'MSSQL$SQLEXPRESS'
```

ブラウザー(Edge)も閉じます。

### 3.4 動いているかの確かめ方

- **ページ**: トップと、主な画面(ログイン、一覧、詳細)を開きます。4 章に、コーパスごとの開くページがあります。
- **ログ**: Windows はサイトのフォルダーの `probe.log`(`supervise.ps1` の `-Log` で変えられる)。コンテナは `docker logs <名前>`。
  ASP.NET のエラー ページ(黄色い画面)と同じ内容が、ブラウザーにも出ます(この PC から開いたとき。`customErrors` が `RemoteOnly` のアプリ)。
- **状態コードだけ見る**: `curl.exe -s -o NUL -w "%{http_code}" http://localhost:5096/`

---

## 4. コーパスごと

### 一覧

| 名前 | アプリ | 種類 | DB | 元のビルド | リポジトリのフォルダー(Studio の「リポジトリのフォルダー」) | Web プロジェクト(`corpora\work\` から) |
|---|---|---|---|---|---|---|
| be | BlogEngine.NET 3.3.8 | Web Forms(C#) | 無し(App_Data の XML) | 不要 | `BlogEngine.NET-3.3.8.0` | `BlogEngine.NET-3.3.8.0\BlogEngine\BlogEngine.NET\BlogEngine.NET.csproj` |
| wt | WingtipToys | Web Forms(C#) | SQL Server Express `WingtipToys` | 不要 | `wingtiptoys-master` | `wingtiptoys-master\WingtipToys\WingtipToys\WingtipToys.csproj` |
| mojo | mojoPortal 3.1.6 | Web Forms(C#) | SQL Server Express `mojo_w2l` | 要(ソリューション) | `mojoportal-3.1.6` | `mojoportal-3.1.6\Web\mojoPortal.Web.csproj` |
| yaf | YAF.NET 3.2.15 | Web Forms(C#) | SQL Server | 不要 | `YAFNET-3.2.15` | `YAFNET-3.2.15\yafsrc\YetAnotherForum.NET\YAF-SqlServer.csproj` |
| dnn | DNN Platform 9.13.10 | Web Forms(C#、VB) | SQL Server Express `dnn_w2l` | 要(DNN の Cake のビルド) | `Dnn.Platform-9.13.10` | `Dnn.Platform-9.13.10\DNN Platform\Website\DotNetNuke.Website.csproj` |
| n2 | N2 CMS | Web Forms(C#) | SQLite(App_Data) | 要(ソリューション+手順 1 つ) | `n2cms-master` | `n2cms-master\src\WebForms\WebFormsTemplates\N2.Templates.csproj` |
| imis | openIMIS(24.10) | Web Forms(VB) | SQL Server Express `imis_w2l` | 要(構成 `DemoRelease`) | `web_app_vb-main` | `web_app_vb-main\IMIS\IMIS.vbproj` |
| nop | nopCommerce 1.90 | Web Forms(C#) | SQL Server Express | 要(ソリューション) | `nopCommerce-release-1.90` | `nopCommerce-release-1.90\NopCommerceStore\NopCommerceStore.csproj` |
| mvcmovie | MvcMovie | MVC 5 | SQL Server Express `MvcMovie`・`MvcMovieIdentity` | 不要 | `MvcMovie` | `MvcMovie\MvcMovie\MvcMovie.csproj` |
| nop390 | nopCommerce 3.90 | MVC 5 | SQL Server Express | 要(ソリューション) | `nopCommerce-release-3.90` | `nopCommerce-release-3.90\src\Presentation\Nop.Web\Nop.Web.csproj` |
| contoso | Contoso University | MVC 5 | SQL Server Express `ContosoUniversity1` | 不要 | `ContosoUniversity` | `ContosoUniversity\ContosoUniversity\ContosoUniversity.csproj` |

(この表の値は `experiments\wf4c\convert-corpora.ps1` の一覧と同じです。)

### 最新の結果のまとめ

「確認」は、この手引きのために 2026-11-04 に動かしたもの。「記録」は、記録にある最新のもの(日付)。

| 名前 | Windows | Linux | 出どころ |
|---|---|---|---|
| be | 画面 5/5、リンク 17/17 | 5/5、17/17 | **確認** 2026-11-04(すべての操作 201 件は記録 2026-11-02: Windows・Linux とも `RESULT: OK`) |
| wt | 画面 8/8、リンク 64/64 | 8/8(うち error-page は理由のある「許容」)、64/64 | **確認** 2026-11-04(すべての操作は記録 2026-10-31・11-01) |
| mojo | 空の DB からセットアップ → トップ・ログイン・サイトマップ 200 | 同じ | 記録 2026-10-11(Windows のトップ 200 は 2026-10-27) |
| yaf | **`/` が 500**(System.Data.SqlClient のプロバイダーが見つからない) | 最後の記録はインストーラーが止まる(その原因は後で直った。再確認していない) | 記録 2026-10-27・10-29(Linux 2026-10-11) |
| dnn | 空の DB からインストール完了、トップ・`/Login`・`/Terms`・`/Privacy` 200 | 同じ。ページの CSS・JS・画像 21 件すべて 200 | 記録 2026-10-11(Windows のインストーラー 200 は 2026-10-29) |
| n2 | インストーラー完了、トップにコンテンツ、`/N2/` はログインへ | 同じ | 記録 2026-10-11(Windows のトップ 200 は 2026-10-27) |
| imis | ログインし、主な画面 7 つが 200 | 同じ | 記録 2026-10-11 |
| nop | すべての操作 727 件(1,000 スナップショット)`RESULT: OK` | 同じ(PDF の作成など Linux だけの既知の差あり) | 記録 2026-11-03(パッケージ w2l.27) |
| mvcmovie | 12 画面中 11 | 11/12 | 記録 Windows 2026-10-27、Linux 2026-10-22 |
| nop390 | インストール(サンプルデータ)、店頭 19 画面、カートへの追加、管理画面 7 画面 200 | 同じ | 記録 2026-10-11(変換とビルドは 2026-10-29) |
| contoso | 12 画面中 9 | 9/12 | 記録 Windows 2026-10-27、Linux 2026-10-22 |

be・wt・nop 以外は、記録のあとにパッケージの版が上がっています(今は `1.6.5-w2l.27`)。同じ結果になるはずですが、確かめてはいません。

---

### 4.1 be(BlogEngine.NET 3.3.8)

**どんなアプリ**: ブログのエンジン。データは DB ではなく `App_Data` の XML ファイル。Web API 2、Web Pages(Razor)のウィジェット、拡張機能(bin の DLL)を使います。
いちばん準備が少なく、**最初に試すのに向いています**(SQL Server も元のビルドも要りません)。

**手順**(2026-11-04 にこのとおり実行):

```powershell
.\corpora\fetch.ps1 -Only be
.\experiments\wf4c\convert-corpora.ps1 -Only be                 # 約 1 分
# Windows で起動して開く
cd experiments\wf4c\be\BlogEngine\BlogEngine.NET
..\..\..\supervise.ps1 -Dll bin\BlogEngine.NET.dll -Port 5096    # http://localhost:5096/
# (止めたら) リポジトリの一番上に戻って、正解と比べる
.\experiments\wf4c\verify-windows.ps1 -Name be                  # 約 2 分
# Linux
.\experiments\wf4c\run-linux.ps1 -App be\BlogEngine\BlogEngine.NET -Scenario corpora\regression\be.scenario.json -Golden corpora\parity\be.golden-webforms.json   # 約 3 分
```

**見るページ**: `/`(タイトル「Name of the blog | Short description of the blog」)、`/post/welcome-to-blogengine-net`、`/archive`、`/page/Example-page`、`/Account/login.aspx`。`/admin/` はログイン画面へ転送されます。
管理者は BlogEngine の既定の `admin` / `admin` です。

**結果**(確認 2026-11-04): Windows・Linux とも `RESULT: OK (5 スナップショットすべて変換前と一致、リンク 17 件も一致)`。
変換レポート: 未解決 0、スタブ 23(LINQ to SQL のファイル システム、WCF Data Services のギャラリー、デザイナー用の式エディターなど。既定の構成では使われません)。

**すべての操作の比較**(記録 2026-11-02、w2l.27): 操作 201 件(実行できないもの 4 件)。Windows は一致 180・許容 17、Linux は一致 170・許容 27 で、どちらも `RESULT: OK`。

```powershell
.\experiments\wf4c\verify-windows.ps1 -Name be -Suite operations     # 約 13 分
.\experiments\wf4c\run-linux.ps1 -App be\BlogEngine\BlogEngine.NET -Suite operations   # 約 14 分
```

許容される差: 文言(元のサーバーの .NET Framework の日本語の言語パックと、FrameworkOnCore の訳)、IIS の詳細エラー ページ(簡略版で書く)、TLS。Linux だけ: タイムゾーンの一覧(IANA)、カルチャの名前(ICU)、改行、物理パス、プロフィール画像の PNG のバイト。

**気をつけること**:
- アプリを動かすと `App_Data` が書き換わります(投稿、ログなど)。最初の状態に戻すには:
  `.\corpora\be-app-data.ps1 -Target experiments\wf4c\be\BlogEngine\BlogEngine.NET\App_Data`
- Studio では「リポジトリのフォルダー」に `corpora\work\BlogEngine.NET-3.3.8.0` を指定します(2.1)。
- 元のアプリのソリューションのビルド(`converter.ps1 build-original`)は BlogEngine.Core のビルドで失敗しますが、リポジトリに入っている `bin` をそのままサイトとして使うので、変換には影響しません(元のビルドは不要です)。

### 4.2 wt(WingtipToys)

**どんなアプリ**: Web Forms のチュートリアルのおもちゃ屋。EF6 Code First(起動時に DB とデータを作る)、ASP.NET Identity と OWIN、FriendlyUrls、ELMAH。

**準備**: SQL Server Express を起動しておきます(管理者の PowerShell で `Start-Service 'MSSQL$SQLEXPRESS'`)。DB `WingtipToys` はアプリが作ります。

**手順**(2026-11-04 にこのとおり実行):

```powershell
.\corpora\fetch.ps1 -Only wt
.\experiments\wf4c\convert-corpora.ps1 -Only wt
cd experiments\wf4c\wt\WingtipToys\WingtipToys
..\..\..\supervise.ps1 -Dll bin\WingtipToys.dll -Port 5096        # http://localhost:5096/
# (止めたら) リポジトリの一番上で
.\experiments\wf4c\verify-windows.ps1 -Name wt                   # 約 1 分。SQL Server Express は起動しておく
.\experiments\wf4c\run-linux.ps1 -App wt\WingtipToys\WingtipToys -Scenario corpora\regression\wt.scenario.json -Golden corpora\parity\wt.golden-webforms.json -SqlServer   # 約 2 分
```

**見るページ**: `/`、`/ProductList`、`/Product/Fast%20Car`、`/ShoppingCart`、`/Account/Login`。管理者は `canEditUser@wingtiptoys.com` / `Pa$$word1`(アプリが起動時に作る)で、`/Admin/AdminPage` が開けます。

**結果**(確認 2026-11-04): Windows `RESULT: OK (8 スナップショットすべて変換前と一致、リンク 64 件も一致)`。
Linux は 8 画面のうち error-page が「許容」(Docker のポート転送で、アプリから見て要求がこの PC からでなくなり〈`Request.IsLocal` が偽〉、エラーの詳細が出ない。理由つきで `corpora\parity\wt.allowed.json` に記録)、ほかは一致。

**すべての操作の比較**(記録 2026-10-31、2026-11-01 に Windows を再実行して同じ): 152 件(実行できないもの 10 件)。Windows は 223 スナップショット中 一致 192・許容 31、Linux は一致 186・許容 37。

```powershell
.\experiments\wf4c\verify-windows.ps1 -Name wt -Suite operations     # 約 5 分。SQL Server Express の起動・停止もスクリプトが行う
.\experiments\wf4c\run-linux.ps1 -App wt\WingtipToys\WingtipToys -SqlServer -Suite operations
```

**気をつけること**:
- `verify-windows.ps1 -Name wt` は SQL Server Express を起動しません(`-Suite operations` は起動します)。
- 元の Web.config は存在しない Azure SQL を指しています。`fetch.ps1` が `.\SQLEXPRESS` に向け直します(1.3)。それより前に取ったコーパスで起動が止まるときは、`.\corpora\fetch.ps1 -Only wt -Force` で取り直してください。
- Docker の Dockerfile で動かす例は 3.2 の A にあります。

### 4.3 mojo(mojoPortal 3.1.6)

**どんなアプリ**: CMS。初回の起動でセットアップ画面がスキーマ(105 テーブル)を作ります。

**準備**: Visual Studio Build Tools(元のビルド)、SQL Server Express。

**手順**(記録 2026-10-11 の手順。この手引きのためには動かしていません):

```powershell
.\corpora\fetch.ps1 -Only mojo
Start-Service 'MSSQL$SQLEXPRESS'
.\experiments\wf4c\convert-corpora.ps1 -Only mojo
# convert-corpora.ps1 が site\user.config を書く: .\SQLEXPRESS の mojo_w2l、DisableSetup=false
cd experiments\wf4c\mojo\site
..\..\supervise.ps1 -Dll bin\mojoPortal.Web.dll -Port 5096
```

- DB `mojo_w2l` は、初回の起動でセットアップ画面が作ります(記録と `convert-corpora.ps1` の説明による)。
- http://localhost:5096/ を開くとセットアップ(`/Setup/Default.aspx`)が動き、終わるとトップ(`Home - mojoPortal`)になります。
- Linux: `.\experiments\wf4c\run-linux-site.ps1 -Site mojo\site -Dll mojoPortal.Web -Database mojo_linux -Paths '/Setup/Default.aspx', '/' -CaseInsensitive`

**結果**(記録): 空の DB からセットアップ → トップ・ログイン(`/secure/LOGIN.aspx` も)・サイトマップ 200、管理画面はログインへ(Windows・Linux とも 2026-10-11)。コンテナ(変換の Dockerfile)でも、接続文字列を `APPSETTING_MSSQLConnectionString` で渡してセットアップ → トップ・ログイン 200(2026-09-27)。

**知られている制限**: SQLite の構成の mojo は Linux で `libsqlite3` が要ります(未確認。既定は SQL Server)。

### 4.4 yaf(YAF.NET 3.2.15)

**どんなアプリ**: 掲示板。

**いまの状態**: **変換とビルドは通りますが、`/` が 500 です**(「System.Data.SqlClient のプロバイダーが見つからない」。記録 2026-10-27・10-29)。動く状態にはなっていません。試すなら変換とレポートまでです。

```powershell
.\corpora\fetch.ps1 -Only yaf
.\experiments\wf4c\convert-corpora.ps1 -Only yaf
# 起動するなら(500 になるのを見るだけ)
cd experiments\wf4c\yaf\yafsrc\YetAnotherForum.NET
..\..\..\supervise.ps1 -Dll Bin\YAF.dll -Port 5096
```

- 出力のフォルダーは `Bin`(大文字の B)です。接続文字列は `.\SQLEXPRESS` の `yafnet`。
- 変換レポート: 未解決 1(ServiceStack の `LicenseUtils.FromXml`。害は無い)、スタブ 43。
- 経緯: 以前は Web API 2 の FieldAccessException(2026-10-22 に直った)、ServiceStack のスタブ(2026-10-27 に直った)で、もっと手前で止まっていました。Linux の最後の記録(2026-10-11)はその前のものです。
- curl で試すときは Cookie を保ってください(Cookie が無いと、毎回インストーラーへ転送されます)。

### 4.5 dnn(DNN Platform 9.13.10)

**どんなアプリ**: 大きな CMS(C# と VB のプロジェクト)。元のビルドは DNN 自身の Cake のビルドで、サイトはそのビルドが組み立てます。

**準備**: Visual Studio Build Tools、SQL Server Express、ディスク(出力と元のビルドで 20 GB 近く)。元のビルドには長い時間がかかります(npm のパッケージも作るため)。

**手順**(記録 2026-10-11。動かしていません):

```powershell
.\corpora\fetch.ps1 -Only dnn
Start-Service 'MSSQL$SQLEXPRESS'
.\experiments\wf4c\convert-corpora.ps1 -Only dnn
# convert-corpora.ps1 が DB dnn_w2l を作り(無ければ)、site\web.config の SiteSqlServer をそこへ向ける
.\experiments\wf4c\dnn-install.ps1          # サイトを起動して無人インストール(Install/Install.aspx?mode=install)。サイトは動かしたまま
# http://localhost:5096/ を開く
```

- `dnn-install.ps1` は起動したサイトを残します。止めるときは 3.1 のポートのコマンド。このスクリプトを最近動かした記録はありません(2026-10-29 の確認はインストーラーの画面が 200 になるところまで)。うまくいかないときは、サイトを 3.1 のとおり起動して、ブラウザーで http://localhost:5096/Install/Install.aspx?mode=install を開きます(インストールには数分かかります)。
- Linux: `.\experiments\wf4c\run-linux-site.ps1 -Site dnn\site -Dll DotNetNuke.Website -Database dnn_linux -Paths '/Install/Install.aspx?mode=install', '/' -CaseInsensitive`

**結果**(記録 2026-10-11): 空の DB からインストール完了、トップ・`/Login`・`/Terms`・`/Privacy` 200(Windows・Linux)。Linux でページの CSS・JS・画像 21 件すべて 200、host でのログインが通る。2026-10-29 にインストーラーが 200 なのを Windows で再確認。

**知られている制限・気をつけること**:
- JWT の API(JwtController の 7 メソッド)は動きません(System.IdentityModel.Tokens.Jwt 4.0.2 が .NET Framework の System.IdentityModel に依存。5.0 以降の API への書き換えが要る)。
- インストーラーは `Install\*.aspx` を消します。Windows でインストールしたサイトのフォルダーは、もうインストールに使えません。Windows と Linux の両方で試すときは、変換直後の `dnn\site` を写しておいて戻すか、変換し直します(`run-linux-site.ps1` はサイトをコンテナに写すので、Linux 側の実行はフォルダーを変えません)。
- インストールや設定の変更の直後はアプリが再起動し、その間の要求は応答が無い(000・503)ことがあります。続けて要求すれば答えます。
- 大文字・小文字の違うファイル名が多い(1,065 件)ので、Linux では `-CaseInsensitive`(配置の既定)で動かします。
- 以前の Windows の一巡のスクリプト `dnn-cycle.ps1` は 2026-10-31 に削除されました。

### 4.6 n2(N2 CMS)

**どんなアプリ**: CMS。DB は SQLite(`App_Data\n2.sqlite.db`)。元のビルドの後に、リポジトリのセットアップの手順(`build\n2.proj;Templates-PrepareDependencies`)が要ります(`convert-corpora.ps1` が指定済み。Studio ではステップ 4 の「追加の手順」に書きます)。

**準備**: Visual Studio Build Tools。SQL Server は要りません。

**手順**(記録 2026-10-11。動かしていません):

```powershell
.\corpora\fetch.ps1 -Only n2
.\experiments\wf4c\convert-corpora.ps1 -Only n2
.\experiments\wf4c\n2-install.ps1        # 起動 → 管理者のパスワード(N2admin!2026)→ ログイン → テーブル → サンプルのコンテンツ → "/"
# Linux: サイトを残して起動し、同じインストールをコンテナに対して行う
.\experiments\wf4c\run-linux-site.ps1 -Site n2\site -Dll N2.Templates -Keep
.\experiments\wf4c\n2-install.ps1 -Port 5098 -Running
```

**結果**(記録 2026-10-11): インストーラー完了、トップがコンテンツ付き、`/N2/` はログインへ(302)。Windows・Linux とも。2026-10-27 に Windows のトップ 200 を再確認。

**気をつけること**: インストーラーは SQLite の DB を書き換えます(Windows と Linux の両方で試すときは、変換直後のサイトを写しておく)。リポジトリの DB にはもともとサンプルのサイトが入っているので、`/` がそのまま表示されることもあります。変換レポートのスタブ 6 件(`IDataContractSurrogate`、`AssemblyBuilder.Save` など)。

### 4.7 imis(openIMIS、VB)

**どんなアプリ**: 健康保険の管理システム(VB の Web Forms、5 プロジェクト)。DB は別のリポジトリ(`imisdb`)のスクリプトから作ります。

**準備**: Visual Studio Build Tools、SQL Server Express、`sqlcmd`。取得は `imis` と `imisdb` の両方。

**手順**(記録 2026-10-11。動かしていません):

```powershell
.\corpora\fetch.ps1 -Only imis,imisdb
Start-Service 'MSSQL$SQLEXPRESS'
.\experiments\wf4c\convert-corpora.ps1 -Only imis
# 元のビルド(構成 DemoRelease)、変換、DB imis_w2l の作成(スキーマ → ストアド プロシージャ → デモのデータ)、site\web.config の接続文字列
cd experiments\wf4c\imis\site
..\..\supervise.ps1 -Dll bin\IMIS.dll -Port 5097
# 別の PowerShell で、リポジトリの一番上から: ログインして主な画面を開く
.\experiments\wf4c\imis-login.ps1                      # 既定 http://localhost:5097
```

- ログインはデモの `Admin` / `admin123`(デモのビルドのログイン画面に入っている値)。ブラウザーで開くこともできます。
- 画面は、メニューからの移動でないと(Referer が無いと)「You can't copy and paste or modify URL」と断ります。アプリの仕様です。
- Linux:

```powershell
$sql = 'corpora\work\database_ms_sqlserver-24.10\sql'
$scripts = @(Get-ChildItem "$sql\base\*.sql") + @(Get-ChildItem "$sql\stored_procedures\*.sql") + @(Get-ChildItem "$sql\demo\*.sql") | ForEach-Object FullName
.\experiments\wf4c\run-linux-site.ps1 -Site imis\site -Dll IMIS -Database imis_linux -SqlScripts $scripts -Keep
.\experiments\wf4c\imis-login.ps1 -Base http://localhost:5098
```

**結果**(記録 2026-10-11): ログインし、主な画面 7 つ(`/Home.aspx`、`/FindFamily.aspx`、`/FindInsuree.aspx`、`/FindPolicy.aspx`、`/FindClaims.aspx`、`/FindUser.aspx`、`/Reports.aspx`)が 200。Windows・Linux とも。

**確かめていないこと**: レポートの画面(ReportViewer)で実際にレポートを出すこと、7-Zip・SQL Server の型(ネイティブのライブラリ)を使う機能。

### 4.8 nop(nopCommerce 1.90)

**どんなアプリ**: 最後の Web Forms 版の nopCommerce(EC サイト、56 プロジェクト、Entity Framework 4、Ajax Control Toolkit)。いちばん大きな比較(すべての操作 727 件)があります。

**準備**: Visual Studio Build Tools、SQL Server Express、Edge。

**手順**(記録 2026-11-03。動かしていません):

```powershell
.\corpora\fetch.ps1 -Only nop
.\experiments\wf4c\convert-corpora.ps1 -Only nop
# すべての操作を元のアプリ(IIS で記録した正解)と比べる。DB の用意から後片付けまでスクリプトが行う
.\experiments\wf4c\verify-windows.ps1 -Name nop -Suite operations      # 約 33 分
.\experiments\wf4c\run-linux.ps1 -App nop\NopCommerceStore -Suite operations   # 約 40 分
```

自分で起動して触るとき:

```powershell
Start-Service 'MSSQL$SQLEXPRESS'
.\corpora\nop-database.ps1                  # サンプルのデータの DB nopCommerceOps を作る(初回は数分。2 回目からはバックアップから戻す)
# site\ConnectionStrings.config を DB に向ける(インストーラーが書くのと同じ形)
Set-Content experiments\wf4c\nop\site\ConnectionStrings.config '<connectionStrings><add name="NopSqlConnection" connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=nopCommerceOps;Integrated Security=True;Persist Security Info=False;MultipleActiveResultSets=True;Connect Timeout=120" /></connectionStrings>'
cd experiments\wf4c\nop\site
..\..\supervise.ps1 -Dll bin\NopCommerceStore.dll -Port 5096
```

- アプリのインストーラー(`/install/install.aspx`: SQL Server、新しい DB、サンプルデータ)で DB を作ることもできます。
- 管理者: `admin@yourstore.com` / `admin`。
- 戻すとき: `.\corpora\nop-database.ps1`(バックアップから戻す)、DB を消すときは `-Drop`。`ConnectionStrings.config` は元に戻しておきます(変換直後は空)。

**結果**(記録 2026-11-03、w2l.27): 1,000 スナップショット。Windows は一致 708・許容 292(操作: 一致 440、許容 278、実行できない 9)、Linux は一致 696・許容 304(同 429、289、9)で、どちらも `RESULT: OK`。

**知られている差**:
- Ajax Control Toolkit が 4.1 から 20.1 になったことによる HTML の違い(モーダル ポップアップ、タブ、吹き出し)。動きは同じ。
- **Linux では PDF を作れません**(PdfSharp 1.3 が Windows の user32・gdi32 を直接呼ぶ)。変換レポートに `[Unsupported]` と出ます。ほかに Linux だけ: 縮小画像の JPEG・PNG のバイト、タイムゾーンの ID、OLE DB。
- 実行できない 9 件: PayPal などの外部のサービス、Jet/Excel の取り込み、DB のバックアップ、インストーラーそのもの。
- グラフ(管理画面のレポート)は Linux では `APPSETTING_ChartImageHandler` で一時フォルダーを Linux のパスにします(web.config の `dir=c:\TempImageFiles\`)。

### 4.9 mvcmovie(MvcMovie、MVC 5)

**どんなアプリ**: MVC 5 の公式チュートリアルの映画の一覧(EF6 Code First、ASP.NET Identity)。

**準備**: SQL Server Express(DB `MvcMovie`・`MvcMovieIdentity` はアプリが作ります)。元のビルドは要りません。

**手順**(記録。動かしていません):

```powershell
.\corpora\fetch.ps1 -Only mvcmovie
Start-Service 'MSSQL$SQLEXPRESS'
.\experiments\wf4c\convert-corpora.ps1 -Only mvcmovie
.\experiments\wf4c\verify-windows.ps1 -Name mvcmovie           # 比べる前に DB を消す(EF が作り直す)
.\experiments\wf4c\run-linux.ps1 -App mvcmovie\MvcMovie -Scenario corpora\regression\mvcmovie.scenario.json -Golden corpora\parity\mvcmovie.golden-webforms.json -SqlServer
```

自分で起動するときは `experiments\wf4c\mvcmovie\MvcMovie` で `..\..\supervise.ps1 -Dll bin\MvcMovie.dll -Port 5096`。見るページは `/Movies`。

**結果**(記録: Windows 2026-10-27、Linux 2026-10-22): 12 画面中 11。残りの 1 つは、作成の入力の検証のメッセージの言語(元のサーバーは .NET Framework の日本語の言語パック、変換後は英語だった)。2026-10-22 に FrameworkOnCore の日本語訳が入ったので、いまは「訳の文言の違い」になるはずです(まだ流していません)。

### 4.10 nop390(nopCommerce 3.90、MVC 5)

**どんなアプリ**: 最後の MVC 5 版の nopCommerce(プラグイン、Autofac、AutoMapper、EF6)。元のビルドがプラグインを `Plugins` に置きます。

**準備**: Visual Studio Build Tools、SQL Server Express。IIS の正解は **ありません**(比べる道具は使えず、ページを開いて確かめます)。

**手順**(記録 2026-10-11。動かしていません):

```powershell
.\corpora\fetch.ps1 -Only nop390
Start-Service 'MSSQL$SQLEXPRESS'
.\experiments\wf4c\convert-corpora.ps1 -Only nop390
cd experiments\wf4c\nop390\site
..\..\supervise.ps1 -Dll bin\Nop.Web.dll -Port 5096
# http://localhost:5096/ を開くとインストールの画面。SQL Server(.\SQLEXPRESS)、サンプルのデータ付きでインストール
```

- Linux は、変換が書いた Dockerfile のイメージ(3.2 の A)で動かし、空の DB からインストーラーを通しました(記録 2026-10-01・10-11。SQL Server はコンテナ)。DB の接続先はインストールの画面で入れます(コンテナの中からは `.\SQLEXPRESS` は見えないので、SQL Server のコンテナの名前か `host.docker.internal` と、SQL Server の認証)。

**結果**(記録 2026-10-11): インストール(サンプルデータ)完了、店頭 19 画面、商品の画像、カートへの追加(AJAX)、管理画面 7 画面と一覧の JSON が 200。Windows・Linux とも。2026-10-29 に変換し直してビルドが通ることを確認。

**気をつけること**:
- インストーラーは `App_Data` を書き換えます。もう一度インストールするときは、変換直後のサイトを写しておいて戻すか、変換し直します。
- **curl などで試すと、カートとログインが断られます**(nopCommerce 3.90 は curl の User-Agent を検索エンジンとみなす)。ブラウザーで、またはブラウザーの User-Agent を付けて要求します。
- サンプルの「Apple MacBook Pro」は最小の数量が 2 です(1 ではカートに入りません。アプリの仕様)。

### 4.11 contoso(Contoso University、MVC 5)

**どんなアプリ**: EF6 のチュートリアルの大学のアプリ(学生の一覧・作成・編集・削除、並べ替え・検索・ページ送り)。

**準備**: SQL Server Express(DB `ContosoUniversity1` はアプリが初期データ付きで作ります)。元のビルドは要りません。

**手順**(記録。動かしていません):

```powershell
.\corpora\fetch.ps1 -Only contoso
Start-Service 'MSSQL$SQLEXPRESS'
.\experiments\wf4c\convert-corpora.ps1 -Only contoso
.\experiments\wf4c\verify-windows.ps1 -Name contoso
.\experiments\wf4c\run-linux.ps1 -App contoso\ContosoUniversity -Scenario corpora\regression\contoso.scenario.json -Golden corpora\parity\contoso.golden-webforms.json -SqlServer
```

自分で起動するときは `experiments\wf4c\contoso\ContosoUniversity` で `..\..\supervise.ps1 -Dll bin\ContosoUniversity.dll -Port 5096`。見るページは `/Student`。

**結果**(記録: Windows 2026-10-27、Linux 2026-10-22): 12 画面中 9。差の 3 つはすべて入力の検証のメッセージ(必須の入力。元は「EnrollmentDate フィールドが必要です。」、変換後は英語だった)で、mvcmovie と同じ理由です。MVC 自身の文言と日付の表示は一致しています。

**気をつけること**: リポジトリにライセンスの表示が無いので、取得して使うだけです(再配布しない)。取得は、パスが長くなりすぎないようにソリューションのフォルダーだけを `ContosoUniversity` という名前で置きます。

---

## 5. うまくいかないとき

### 5.1 スクリプトが実行できない

「このシステムではスクリプトの実行が無効になっている」と出るときは、自分のユーザーだけ許可します:

```powershell
Set-ExecutionPolicy -Scope CurrentUser RemoteSigned
```

または `powershell -ExecutionPolicy Bypass -File .\experiments\wf4c\convert-corpora.ps1 -Only be` のように実行します。

### 5.2 元のビルドが失敗する

- 「Visual Studio (Build Tools) 2022's MSBuild is not installed」: 1.1 の `winget` のコマンドで入れます。
- それ以外: 変換器の記録 `experiments\wf4c\_linux\foc-<名前>.log` の `[Error]` の行と、元のビルドの記録(`experiments\wf4c\<名前>.original-<日時>.build.log`、または `<名前>.original.build.log`)を見ます。
- 「the build failed after it deployed the site: ... is used as it is」は、ビルドの一部が失敗したが、サイトはできていたのでそれを使った、という意味です(dnn のパッケージ作り、be のソリューションなど)。そのまま進んで構いません。
- 元のビルドを作り直すときは `convert-corpora.ps1 -Only <名前> -Rebuild`。

### 5.3 SQL Server につながらない

- サービスが止まっている: `Get-Service 'MSSQL$SQLEXPRESS'`、管理者で `Start-Service 'MSSQL$SQLEXPRESS'`。
- インスタンス名が違う: スクリプトは `.\SQLEXPRESS` を前提にしています。別の名前なら、SQL Server Express を `SQLEXPRESS` という名前で入れるのが早道です。
- `sqlcmd` が無い: convert-corpora(dnn・imis)と verify-windows(wt・mvcmovie・contoso)が使います。SQL Server のコマンドライン ツールを入れます。
- アプリのログに「Login failed for user」: Windows 認証で、アプリを動かしている自分のユーザーに DB を作る権限が要ります(SQL Server Express を入れたユーザーなら持っています)。
- コンテナの中から: `.\SQLEXPRESS` や LocalDB は見えません。`run-linux.ps1 -SqlServer`・`run-linux-site.ps1` は SQL Server のコンテナを使います。この PC の SQL Server を使うなら `host.docker.internal` と SQL Server の認証(混合モード、TCP 1433)。

### 5.4 ポートが使われている

`Failed to bind to address http://localhost:5096: address already in use` などと出るときは、前に動かしたアプリが残っています(再起動した作業プロセスが残ることがあります)。

```powershell
Get-NetTCPConnection -LocalPort 5096 -State Listen -ErrorAction SilentlyContinue | ForEach-Object { Stop-Process -Id $_.OwningProcess -Force }
```

使うポート: Studio 5300、Studio のネイティブ起動 8100 から、`verify-windows.ps1`・`dnn-install.ps1`・`n2-install.ps1` 5096、`imis-login.ps1` 5097、`run-linux.ps1` 5095、`run-linux-site.ps1` 5098、Dockerfile の例 8080。

### 5.5 Docker が遅い・止まる(メモリー)

- このリポジトリの開発マシンは 16 GB で、WSL 2 に 8 GB を割り当てています(`%USERPROFILE%\.wslconfig` の `[wsl2]` に `memory=8GB`)。少ないマシンでは、SQL Server のコンテナ(2 GB ほど使う)とアプリのコンテナで足りなくなることがあります。
- **コンテナは一度に一つ(DB の要るものは SQL Server とアプリの 2 つ)にし、終わったら止めます**(3.3)。使っていない SQL Server Express と Edge も閉じます。
- 終わったら Docker Desktop を止めて `wsl --shutdown`(WSL のメモリーが返ります)。

### 5.6 ディスクがいっぱい(Docker)

Linux の実行を重ねると、Docker のビルドのキャッシュとイメージが増えます(記録では 45 GB になり、C ドライブがいっぱいになりました)。

```powershell
docker system df                 # 何がどれだけ使っているか
docker builder prune -af         # ビルドのキャッシュを消す
docker image prune -a            # 使っていないイメージを消す
```

消しても `docker_data.vhdx`(WSL のディスク)は小さくなりません。Docker Desktop を止めて `wsl --shutdown` したあと、管理者で `diskpart` を使い、`select vdisk file="<docker_data.vhdx のパス>"`、`compact vdisk` で縮めます。

### 5.7 比較(verify-windows・run-linux)が失敗する

- Edge が無い・起動できない: 比較の道具(ParityTest)は Playwright で Edge を画面無しで動かします。Edge を入れ直すか更新します。終わったあと Edge のプロセスが残っていたら閉じます。
- Edge が更新されて、User-Agent の版が画面に出る所が違う: wt のすべての操作の正解には Edge 156 の User-Agent が入っています。版が変わると差が出ることがあります。
- `RESULT: FAIL` で、違いが文言や時刻だけ: 違った画面の差が出力に出ます。許される差の一覧(`corpora\parity\*.allowed.json`)にない違いです。
- be のすべての操作は、`App_Data` をリリースの状態に戻してから始めます(スクリプトが行う)。それ以外の比較で be の結果がおかしいときは、`corpora\be-app-data.ps1` で `App_Data` を戻します。

### 5.8 変換が「differ from their snapshot」で失敗する

2.2 のとおり、be・wt・nop 以外は記録が前の版のものなので、版の違いで失敗扱いになります。`experiments\wf4c\_linux\snapshot-diff-<名前>.txt` を見て、違いが版(`1.6.5-w2l.25` → `1.6.5-w2l.27`)やハッシュだけなら問題ありません。

### 5.9 アプリが 500 になる

- まずエラーの中身を見ます(ブラウザー、`probe.log`、`docker logs`)。
- 変換レポートの「未解決」と「コンパイルのために変えたソース(スタブ・削除)」に、その画面のコードがあれば、変換で動かなくなった所です(`PlatformNotSupportedException`)。
- yaf の `/` の 500 は既知です(4.4)。

---

## 6. 自分のアプリで試すとき

流れはコーパスと同じです。初めては Studio(2.1)が分かりやすいです。

1. `.\studio.ps1` で Studio を開き、「+ 新しい解析」で自分の Web プロジェクト(.csproj / .vbproj)を指定します。
   - **「リポジトリのフォルダー」**: プロジェクトが参照する DLL やほかのプロジェクトが、`.sln` のフォルダーの外(リポジトリの `lib` など)にあるときは、それを含むフォルダーを指定します。指定が足りないと、変換レポートの「未解決」に「a reference or package of the original that the conversion lost」が出ます。
2. ステップ 1 の「元のアプリを起動(テスト起動)」で、まず元のアプリがこの PC で動くかを見ておくと、後で比べられます。
3. ステップ 2 で「自分で対応する所」を読みます。変換しても動かない部品(OLE DB、Active Directory、レジストリなど)と、その書き換え方が出ます。
4. ステップ 4 で変換し、変換レポート(2.4)の「未解決」が 0 件か、「スタブ」に自分の使う機能が無いかを確かめます。
5. ステップ 5 で「ネイティブ起動」して画面を確かめ、Linux は「コンテナ起動」か ZIP の `Dockerfile`・`deploy/README.md` で。DB は環境変数 `SQLCONNSTR_<名前>` で渡します。

元のサーバー(本番の IIS)の設定を持ってくると、変換後の動きが元に近くなります。どちらも **元のサーバーで** 実行し、できたファイルを変換に渡します:

```powershell
# カルチャのデータ(数値・日付の書式など。Windows PowerShell で)
powershell -File experiments\wf4c\capture-culture.ps1 -Out culture-profile.json -WebConfig <アプリ>\web.config
# IIS のサイトの設定(管理者で)
powershell -File experiments\wf4c\capture-iis.ps1 -Site "Default Web Site" -Out iis-profile.config
# 変換に渡す
.\converter.ps1 <Web プロジェクト> --out <出力先> --culture-profile culture-profile.json --iis-profile iis-profile.config --build-original auto
```

関連する資料:

- [README.md](../README.md): 全体の流れ、Studio の手順、コマンドライン
- [tests/README.md](../tests/README.md): テストと比較の一覧
- [corpora/README.md](../corpora/README.md): コーパスの取得と正解の記録
- [experiments/wf4c/README.md](../experiments/wf4c/README.md): 検証の記録(コーパスごとの経緯)
- 変換の出力の `deploy/README.md`: Linux への配置(コンテナ、systemd)
