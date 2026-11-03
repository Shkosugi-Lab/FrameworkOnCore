# テストの地図

FrameworkOnCore のテストは、xunit の単体テストからコーパス(実在の OSS アプリ)の新旧比較まで、層ごとに場所と正解が違います。
ここはその一覧と「いつ何を動かすか」の 1 か所です。各テストを作った経緯と結果の記録は [experiments/wf4c/README.md](../experiments/wf4c/README.md)
(以下「記録」)の各節にあり、ここでは繰り返しません。

まとめて動かすには `tests/run-tests.ps1` を使います(下の「いつ何を動かすか」のコマンドを順に呼ぶだけの薄いスクリプト)。

```powershell
.\tests\run-tests.ps1 -Set quick      # 約 4 分。準備なし
.\tests\run-tests.ps1 -Set commit     # 変換器を変えたとき。約 5 分(be/wt の変換と IIS との比較を含む)
.\tests\run-tests.ps1 -Set runtime    # FrameworkOnCore.Runtime を変えたとき(パッケージを作り直す)
.\tests\run-tests.ps1 -Set full       # リリースの前。Linux を含む
.\tests\run-tests.ps1 -Set commit -Linux   # commit・runtime に Linux の実行を足す
```

## 層

| 層 | 何を比べるか | 正解(オラクル) | 場所 |
|---|---|---|---|
| 1. 単体テスト | 変換器・アナライザー・Studio・互換アセンブリ、移植で書いたコード(ホスト、IIS の機能、ResX、CodeDom、暗号) | ドキュメント(IIS の構成リファレンスなど)、参照ソース、不変条件 | `tests/FrameworkOnCore.Tests`、`tests/RuntimeUnitTests` |
| 2. .NET Framework との新旧比較 | 移植したライブラリの全 API・入力 | このマシンの .NET Framework 4.8 で記録したゴールデン | `tests/*Parity`(xunit から実行)、`RuntimeUnitTests` の Fuzz・RoundTrip・Security、`SqlClientTests` |
| 3. IIS との新旧比較(サイト) | 変換したサイトの画面・応答 | このマシンの IIS(.NET Framework 4.8)で記録したゴールデン | `samples/*Probe` + `sample-parity.ps1`、`tests/WebControlsParity`、IIS の機能の `*-check.ps1`、`AttackTests`(IIS 10 の応答) |
| 4. コーパス | 実在のアプリの変換結果と動作 | 変換の基準(前回の変換)と IIS のゴールデン | `corpora/`、`convert-corpora.ps1`、`verify-windows.ps1`、`run-linux.ps1` |
| 5. 環境 | 1・2 をほかのカルチャ・タイムゾーン・Linux で | 1・2 と同じ | `tests/run-matrix.ps1`、`run-tests-linux.*` |
| 6. CI | Linux での 3 プロジェクト、Studio・変換器の起動 | 1・2 と同じ | `.github/workflows` |

## 1・2. xunit のプロジェクト

Windows での件数と時間は 2026-10-31 の実行(`dotnet test`、ビルド込み。RuntimeUnitTests は 2026-11-01)。Linux の件数は 2026-11-01 の `run-tests.ps1 -Set runtime -Linux`。

| プロジェクト | 中身 | 件数(Windows) | 時間 | CI(Linux) |
|---|---|---|---|---|
| `tests/FrameworkOnCore.Tests` | 下の表 | 4,630(スキップ 1。SQL Server が無いとさらに 25) | 約 1 分 | 実行(Linux 4,583) |
| `tests/RuntimeUnitTests` | 下の表 | 1,980(スキップ 1) | 約 4 分(Stress が 1 分 40 秒) | 実行(Linux 1,977、うち成功 1,972) |
| `tests/SqlClientTests` | Microsoft の System.Data.SqlClient の機能テスト(TDS のテスト サーバー)と、移植の追加分(`FrameworkOnCore/`: DataDirectory の展開、接続文字列のキーワード 81 件を .NET Framework 4.8 のゴールデンと) | 326(スキップ 5: SQL Server Browser・接続文字列・名前付きパイプが要るもの) | 約 1 分 | 実行(Linux 309) |

どれも準備なしで動きます。SQL Server Express(`.\SQLEXPRESS`)か `FOC_TEST_SQLSERVER` があると、LINQ to SQL のデータベースのテスト 25 件も動きます(無ければスキップと報告)。

```powershell
dotnet test tests\FrameworkOnCore.Tests
dotnet test tests\RuntimeUnitTests
dotnet test tests\RuntimeUnitTests --filter "Category!=Stress"    # 並行性・ストレスを除く(約 2 分)
dotnet test tests\SqlClientTests
```

### FrameworkOnCore.Tests の中身

| 区分 | クラス(件数) | 正解 |
|---|---|---|
| System.Drawing の新旧比較 | `Drawing/DrawingParityTests`(1,621) | `tests/DrawingParity/golden`(.NET Framework 4.8)。Linux は `known-differences.json` の理由つきの差 |
| モバイル コントロール | `Mobile/MobileParityTests`(1,500) | `tests/MobileParity/golden` |
| グラフ(Chart) | `DataVisualization/DataVisualizationParityTests`(1,044) | `tests/DataVisualizationParity/golden` |
| LINQ to SQL | `DataLinq/DataLinqParityTests`(57)、`DataLinqSqlGoldenTests`(生成する SQL)、`DataLinqDatabaseTests`・`Types`・`Context`・`Mapping`(計 26) | `tests/DataLinqParity/golden`、`DataLinq/golden/dlinq-sql.golden.json` |
| 変換器の書き換え | `RewriteTests`、`RewriteCompilesTests`(書き換えた結果がコンパイルできるか)、`StubCausesTests`、`SourceEncodingTests`(Shift_JIS)、`FloatingTextTests`、`EndInCatchTests`、`ProjectConverterTests`、`ProjectFileTests`、`PackageRaisesTests`、`AssemblyRetargeterTests`、`BuildFixer*`、`BuildConfigurationTests`、`DllBindingTests`、`FrameworkAssetsFlowTests`、`OriginalBuild*`、`DeployWriterTests`、`HostProgramTests`、`IisFeaturesTests`、`WcfServicesTests`、`PathsTests`、`FileTreesTests`、`NodeSetupTests`、`RuntimeSetupTests`、`ProcessOutputTests` | 不変条件、手で書いた期待値 |
| アナライザー | `WindowsPathAnalyzerTests`、`AsyncDelegateAnalyzerTests`、`SpanInExpressionAnalyzerTests` | コーパスで見つかった形 |
| 解析・Studio | `ApiAnalysisTests`、`ChoicesTests`、`CatalogEnglishTests`、`Studio*Tests`、`LogTailTests` | 不変条件 |
| 互換アセンブリ・訳・パッケージ | `CompatTests`(OS ごとの動き)、`CultureStampsTests`、`TranslationsTests`、`RuntimeBoundaryTests`(パック済みパッケージの公開シグネチャ。`_feed` に無ければスキップ) | .NET Framework の動き、`translations/sources.json` |

スキップされるもの: `NodeSetupTests` の 1 件(Node.js をダウンロードする。`FOC_NODE_SETUP_TEST=1`)、Windows 専用・日本語の Windows 専用のもの(Linux で)、SQL Server の要るもの(上)。

### RuntimeUnitTests の中身

FrameworkOnCore の System.Web をソースからビルドして内部の型を直接テストします。分岐の網羅は `experiments/wf4c/coverage`(記録「移植で書いたコードの分岐網羅の単体テスト」)。

| フォルダー | 件数 | 中身と正解 |
|---|---|---|
| `Hosting` | 865 | ASP.NET Core のホスト、IIS の機能(URL Rewrite・httpErrors・圧縮・静的ファイル・ログ・httpRedirect・httpProtocol)。IIS の構成リファレンス |
| `Security` | 690 | `AttackTests`(415 件、このマシンの IIS 10 の応答 `Security/iis-golden/iis.json`。`Record` の 1 件は採り直し用で常にスキップ)、machineKey・チケット・DataProtector(`Security/golden/net48.json`) |
| `Resources` | 133 | ResX・.resources の置き換え |
| `CodeDom` | 97 | CodeDom の置き換え |
| `RoundTrip` | 54 | ViewState・セッション・BinaryFormatter・ResX を .NET Framework 4.8 が書いたバイトと(`RoundTrip/golden`) |
| `Web` | 51 | 応答・ナビゲーション・セッションなど |
| `Fuzz` | 50 | HttpUtility などを 31,300 入力で .NET Framework 4.8 と(`Fuzz/golden/*.txt.gz`) |
| `Stress` | 37 | 並行性とストレス(`Category=Stress`。`FOC_STRESS` で回数を増やせる) |

## 2. ゴールデンの採り直し(.NET Framework 4.8・IIS)

正解はこのマシン(日本語の Windows、IIS 10、SQL Server Express)で採ったものです。テストの入力やケースを変えたときだけ採り直し、差分を確かめてからコミットします。

| ゴールデン | 使うテスト | 採り直し | 必要なもの |
|---|---|---|---|
| `tests/DrawingParity/golden` | DrawingParityTests | `.\tests\DrawingParity\record.ps1` | .NET Framework 4.8 |
| `tests/MobileParity/golden` | MobileParityTests | `.\tests\MobileParity\record.ps1` | 同上 |
| `tests/DataVisualizationParity/golden` | DataVisualizationParityTests | `.\tests\DataVisualizationParity\record.ps1` | 同上 |
| `tests/DataLinqParity/golden` | DataLinqParityTests | `.\tests\DataLinqParity\record.ps1` | 同上、SQL Server Express |
| `tests/FrameworkOnCore.Tests/DataLinq/golden/dlinq-sql.golden.json` | DataLinqSqlGoldenTests | `.\tests\FrameworkOnCore.Tests\DataLinq\golden\record.ps1` | 同上 |
| `tests/RuntimeUnitTests/Security/iis-golden/iis.json` | AttackTests | `powershell -ExecutionPolicy Bypass -File tests/RuntimeUnitTests/Security/iis-golden/record.ps1` | 管理者、IIS 10、URL Rewrite 2.1 |
| `tests/RuntimeUnitTests/Security/golden/net48.json` | Security の各テスト | `golden/Program.cs` の先頭のコメント(Recorder.csproj をビルドして実行) | .NET Framework 4.8、テストと同じユーザー(DPAPI) |
| `tests/RuntimeUnitTests/Fuzz/golden/*.txt.gz` | FuzzTests | `Fuzz/golden/Program.cs` の先頭のコメント | .NET Framework 4.8 |
| `tests/RuntimeUnitTests/RoundTrip/golden/*.txt.gz` | RoundTripTests | `RoundTrip/golden/Program.cs` の先頭のコメント(逆向きの `verify` も) | .NET Framework 4.8 |
| `tests/SqlClientTests/FrameworkOnCore/golden/framework.golden.json` | FrameworkGoldenTests | `FrameworkOnCore/golden/Record.cs` の先頭のコメント | .NET Framework 4.8 |
| `experiments/wf4c/numbers-parity/framework-golden.txt`(115 MB、コミットしない) | `numbers-parity`(手で実行。double・float の文字列化 2,647,080 件) | Windows PowerShell 5.1 で `framework-golden.ps1 -Out <file>`、`dotnet run --project experiments\wf4c\numbers-parity -- <file>` | .NET Framework 4.8 |

## 3. IIS とのサイトの比較

変換したサイトを動かし、`tools/FrameworkOnCore.ParityTest`(Playwright で Edge を動かす。headless)で IIS の記録と比べます。どれも手で実行し、CI では動きません。

| 対象 | コマンド | 正解 | 必要なもの | 時間 |
|---|---|---|---|---|
| サンプル 32 本(`samples/*Probe`、`OrderAdmin`、`ProductAdmin`) | `.\experiments\wf4c\sample-parity.ps1 -Name <名前> -Record` / `-Windows` / `-Linux` | `samples/<名前>/golden-webforms.json`(シナリオは `parity-scenario.json`) | 記録: 管理者・IIS。`-Linux`: Docker。AuthProbe: SQL Server Express | 1 本 1〜5 分(NavigationProbe は長い) |
| ChartProbe | `.\experiments\wf4c\chart-probe.ps1 -Record` / `-Windows` / `-Linux` | `samples/ChartProbe/golden-chart-probe.txt` | 同上 | |
| コントロール 124(WebControlsProbe) | `.\tests\WebControlsParity\parity.ps1 -Control TextBox -Record` / `-Windows` / `-Linux` | `tests/WebControlsParity/golden/<コントロール>`、結果は `reports/<コントロール>/` | 同上(仕組みは [WebControlsParity/README.md](WebControlsParity/README.md)) | 1 つ約 1.5 分、全部で約 2 時間 |
| IIS の機能の生の応答 | `errors-check.ps1`(ErrorsProbe)、`headers-check.ps1`(HeadersProbe)、`rewrite-check.ps1`(RewriteProbe)、`compression-check.ps1`、`iislog-check.ps1`(PipelineProbe) | IIS の応答そのもの(状態行・ヘッダー・ログの行) | 管理者・IIS。先にそのサンプルの `sample-parity.ps1 -Record` と `-Windows` | |
| 負荷(数値を見るだけ) | `load-check.ps1`(curl の並列)、`proxy-check.ps1`(nginx の後ろ。不変条件) | なし | IIS・Docker | |
| 元のサンプルをそのまま動かす(初期の実験) | `.\experiments\wf4c\run-sample.ps1 -Name OrderAdmin` | サンプルのゴールデン | Edge | 約 1 分 |

サンプルの一覧と最新の結果は記録の各節(「動的コンパイル・ルーティング・サービス・同時アクセス・制限値の新旧比較」「認証・要求処理・キャッシュの新旧比較」「違う観点の意地悪なコーパス」「差が出るように作ったコーパス」「意地悪な画面遷移のコーパス: NavigationProbe」「フォルダーごとの設定、フォルダーの一覧、httpErrors・圧縮の調査」「応答ヘッダー」「URL Rewrite の 2 つのモード」)。

## 4. コーパス

日常に使うのは **be(BlogEngine.NET)と wt(WingtipToys)だけ**です(CLAUDE.md。ほかは指示があるときに `-Only` で)。取得は `.\corpora\fetch.ps1`(初回だけ)。

| 何を | コマンド | 正解 | 必要なもの | 時間(2026-10-31) |
|---|---|---|---|---|
| 変換が前と同じか | `.\experiments\wf4c\convert-corpora.ps1 -Only be,wt` | `corpora/regression/conversion-baseline/<名前>.json`(書いたプロジェクト・レポート・変えたファイルのハッシュ)と `conversion-baseline.json`(未解決・スタブの数)。違えば失敗。意図した違いは `-UpdateBaseline` で記録し直す | 元のビルドに Visual Studio の MSBuild | be・wt で 1.5 分 |
| be の画面 | `.\experiments\wf4c\verify-windows.ps1 -Name be` | `corpora/parity/be.golden-webforms.json`(5 画面、リンク 17) | Edge | 2 分 |
| wt の画面 | `.\experiments\wf4c\verify-windows.ps1 -Name wt` | `corpora/parity/wt.golden-webforms.json`(8 画面、リンク 64) | SQL Server Express を起動しておく | 1 分 |
| wt のすべての操作 | `.\experiments\wf4c\verify-windows.ps1 -Name wt -Suite operations` | `corpora/parity/wt.operations.golden.json`(223 スナップショット)、許される差 `wt.operations.allowed.json`。結果は `experiments/wf4c/_results/` | SQL Server Express(起動と停止はスクリプトが行う) | 約 5 分 |
| be のすべての操作 | `.\experiments\wf4c\verify-windows.ps1 -Name be -Suite operations` | `corpora/parity/be.operations.golden.json`(473 スナップショット。操作 201 件の棚卸しは `corpora/regression/be.operations.json`・`.md`、シナリオは `be.operations.scenario.json`)、許される差 `be.operations.allowed.json`。どの実行も App_Data をリリースのもの(`corpora/be-app-data.ps1`)に戻してから始め、`files` の手順で App_Data の変化も比べる。結果は `experiments/wf4c/_results/be.operations.windows.{md,json}` | Edge | 約 13 分 |
| nop のすべての操作(コーパスの例外: 指示があるときだけ) | `.\experiments\wf4c\verify-windows.ps1 -Name nop -Suite operations` | `corpora/parity/nop.operations.golden.json`(1,000 スナップショット。操作 727 件の棚卸しは `corpora/regression/nop.operations.json`・`.md`、シナリオは `nop.operations.scenario.json`)、許される差 `nop.operations.allowed.json`。どの実行もサンプル データのデータベース(`corpora/nop-database.ps1`)から始める。結果は `experiments/wf4c/_results/nop.operations.windows.{md,json}` | SQL Server Express(起動と停止はスクリプトが行う)、Edge | 約 33 分 |
| Linux の be・wt | `.\experiments\wf4c\run-linux.ps1 -App be\BlogEngine\BlogEngine.NET -Scenario corpora\regression\be.scenario.json -Golden corpora\parity\be.golden-webforms.json`<br>`.\experiments\wf4c\run-linux.ps1 -App wt\WingtipToys\WingtipToys -Scenario corpora\regression\wt.scenario.json -Golden corpora\parity\wt.golden-webforms.json -SqlServer`<br>`.\experiments\wf4c\run-linux.ps1 -App wt\WingtipToys\WingtipToys -SqlServer -Suite operations`<br>`.\experiments\wf4c\run-linux.ps1 -App be\BlogEngine\BlogEngine.NET -Suite operations` | 同上。wt の Linux だけの許される差は `corpora/parity/wt.allowed.json`(8 画面)と `wt.operations.allowed.json`(すべての操作)、be のすべての操作のものは `be.operations.allowed.json` の `platforms: linux` | Docker | be 3 分、wt 2 分、be のすべての操作 約 14 分 |

期待される結果: be 5/5、wt 8/8(Windows)。Linux は be 5/5、wt 8/8 のうち error-page が「許容」(Docker のポート転送で `Request.IsLocal` が偽。
以前からの既知の差で、2026-10-31 に `wt.allowed.json` に理由つきで記録した。`run-linux.ps1` は `--platform linux` を渡すので、この差だけが許される)。
ゴールデンの採り直し(IIS、管理者): `.\corpora\record-webforms-golden.ps1 -Only be`、`.\corpora\record-wt-operations.ps1`(詳しくは [corpora/README.md](../corpora/README.md))。

be のすべての操作(2026-11-02): 期待される結果は Windows・Linux とも `RESULT: OK`(終了コード 0)。操作 201 件のうち、実行できない 4 件(メールの中身、
インターネット上のギャラリーと更新が要るもの。理由は棚卸しに書いてある)を除く 197 件を実行する。Windows は同じ 180・許される差 17、Linux は同じ 170・許される差 27
(Linux だけの差: タイムゾーンの一覧が IANA のもの、ICU のカルチャのネイティブ名、Environment.NewLine、物理パス、PNG のエンコーダーのバイト。理由は許される差のファイルに書いてある)。
be では、`run-linux.ps1 -Suite operations` が次を既定で行う: リリースの App_Data をコンテナに入れる。アプリをコンテナの 127.0.0.1 で動かし、socat で公開する
(`-LocalClient`。接続元が 127.0.0.1 になり、`Request.IsLocal` とコメントの IP が IIS と同じになる)。`TZ` をこのマシン(カルチャ プロファイル)のタイムゾーンにする
(`-TimeZone`)。変換がビルドの後に bin に加えたもの(`frameworkoncore.*.txt`、言語別のサテライト)をそろえる。
採り直し(IIS、管理者、約 10 分): `.\corpora\record-be-operations.ps1`。`-Discover` を付けると 3 つの立場で巡回し、棚卸しの漏れを `corpora/regression/be.discovered.iis.json` に書く。
`-Keep` でサイトを残し、`-Remove` で消す。IIS のサイト `foc-beops` と同名のアプリケーション プールを 127.0.0.1:8100〜8199 に作り、終われば消す。
棚卸しとシナリオを変えるときは JSON を手で直さない。`python corpora\regression\be-operations-inventory.py`・`python corpora\regression\be-operations-scenario.py`
(同じ入力から同じ JSON を書く)で書き直してから採り直す。シナリオの `{today}` は実行した日の日付(採った日と違う日にも比べられるようにするため)。

nop(nopCommerce 1.90)のすべての操作(2026-11-03。CLAUDE.md の be・wt だけの例外で、指示があるときだけ動かす): 期待される結果は Windows・Linux とも `RESULT: OK`。
操作 727 件のうち、実行できない 9 件(インターネット上のサービス: PayPal Express・外部の決済サービス・ECB の為替レート・SMS・QuickBooks・Froogle の FTP、Excel の取り込み〈64 ビットに Jet が無い〉、
データベースのバックアップ〈SQL Server がサイトのフォルダーに書く〉、インストーラー。理由は棚卸しに書いてある)を除く 718 件を実行する。
Windows は同じ 440・許される差 278(ほとんどが Ajax Control Toolkit の版: 4.1 と 20.1 のタブ・モーダル ポップアップ・検証の吹き出しの DOM)。
Linux は `.\experiments\wf4c\run-linux.ps1 -App nop\NopCommerceStore -Suite operations`(Docker。約 40 分): 変換が組み立てたサイト(`nop\site`)をそのまま動かし、
SQL Server のコンテナにデータベースを戻し、`-LocalClient`・`-TimeZone`・casefs を既定で使う。Linux は同じ 429・許される差 289。Linux だけの許される差: サムネイルの JPEG・PNG のバイト、
タイムゾーン(IANA の ID)、カルチャの一覧(ICU)、OLE DB、PDF(PdfSharp 1.3 が user32・gdi32 を呼ぶ)。
採り直し(IIS、管理者、約 35 分): `.\corpora\record-nop-operations.ps1`(`-Discover` で 3 つの立場の巡回と棚卸しの漏れの確認、`-Keep`・`-Remove`)。
ビルドした元(`experiments\wf4c\nop.original\NopCommerceStore`: `convert-corpora.ps1 -Only nop` が作る)を `corpora\work\_iis\nop` に写して IIS のサイト `foc-nopops` で動かし、
終われば消す(サイト・プール・SQL のログイン・写し・データベース)。棚卸しとシナリオは `python corpora\regression\nop-operations-inventory.py`・`nop-operations-scenario.py` で書き直す
(共通の一覧は `nop_operations_common.py`)。IIS どうしで 2 回比べて一致することを確かめてある(メール送信のタスクが時刻しだいで書くログ、登録した利用者のソルトは比べない)。

## 5. 環境のマトリクスと Linux

| 何を | コマンド | 時間 |
|---|---|---|
| Windows で 6 カルチャ(ja-JP・en-US・invariant・tr-TR・de-DE・ar-SA)× 3 プロジェクト | `.\tests\run-matrix.ps1` | 約 30 分(推定) |
| Linux で 3 カルチャ × 3 タイムゾーン | `.\tests\run-matrix.ps1 -Os Linux` | 長い(推定 1 時間以上) |
| Linux の FrameworkOnCore.Tests と SqlClientTests | `.\tests\FrameworkOnCore.Tests\run-tests-linux.ps1 -SqlServer`(SQL Server のコンテナ w2l-sql も) | |
| Linux の RuntimeUnitTests | `docker run --rm -m 6g -v "${PWD}:/repo:ro" mcr.microsoft.com/dotnet/sdk:10.0 bash /repo/tests/RuntimeUnitTests/run-tests-linux.sh` | |

Linux の実行の約束(CLAUDE.md、メモリ 16 GB): コンテナは 1 つずつ順番に(バックグラウンドで重ねない)。終わったら Docker Desktop を終了して `wsl --shutdown`
(`run-matrix.ps1` と `run-tests.ps1` は自分で行う)。使わない SQL Server Express と Edge は閉じる。

### 終了コード(2026-10-31 から)

Linux のスクリプトも、失敗すれば 0 以外で終わります(以前はテストや比較が失敗しても 0 で、出力を見る必要があった)。出力は前と同じです。

| スクリプト | 0 | 0 以外 |
|---|---|---|
| `experiments/wf4c/run-linux.ps1` | ParityTest の `RESULT: OK`(すべてのスナップショットが同じか、シナリオの `allowed` の Linux の許される差。リンクもすべて同じ) | `RESULT: FAIL` で 1。ビルドの失敗・起動しないときは例外(`powershell -File` なら 1) |
| `tests/FrameworkOnCore.Tests/run-tests-linux.ps1` | ビルドとテストがすべて成功 | 1。FrameworkOnCore.Tests が失敗しても SqlClientTests は動かす。`-Matrix` も全部動かしてから 1 |
| `tests/RuntimeUnitTests/run-tests-linux.sh`(コンテナの終了コード) | ビルドとテストが成功 | ビルドの失敗はその終了コード、テストの失敗は `dotnet test` の終了コード。`MATRIX` は全部動かしてから 1 |
| `tests/run-tests.ps1` | すべての段階が成功 | Linux の段階は失敗しても次を動かし、最後に失敗した段階の名前で例外。ほかは最初の失敗で止まる |

出力を grep などで絞っても、判定は `PIPESTATUS` でコマンドそのものの終了コードを使います(`head` は `awk 'NR <= n'` に替えた:
`head` が先に終わると `dotnet test` が SIGPIPE で終わるため)。`run-matrix.ps1` は終了コードでなく trx を集計します(変えていない)。
確かめたこと(2026-10-31): be・wt の `run-linux.ps1` が 0(wt は error-page が許容)、`run-tests-linux.ps1 -SqlServer` が 0、
`-ParityStrict -Filter FullyQualifiedName~DrawingParityTests`(Linux の既知の差も失敗にする)が 1、`run-tests-linux.sh` が 0、
`MATRIX=invariant@No/SuchZone`(無いタイムゾーンで全件失敗)が 1。

## 6. CI(GitHub Actions)

| ワークフロー | いつ | 何を |
|---|---|---|
| `frameworkoncore-packages.yml` | `FrameworkOnCore.Runtime/`・`translations/`・`rules/packages.json`・上の 3 プロジェクト・`tests/*Parity`・`Parity.Core`・`Shared` の変更の push | パッケージのビルド(Windows)、Linux のコンテナで 3 プロジェクトの `dotnet test`(SQL Server なし: LINQ to SQL のデータベースのテストはスキップ)、まだ無い版の公開。約 10 分 |
| `frameworkoncore-tools.yml` | `src/` の変更の push | Studio と変換器の publish、RuntimeProbe の解析と Studio の起動の煙テスト |

Windows のテスト(GDI+ の描画、IIS、SQL Server Express、日本語の Windows のゴールデン)と、層 3・4 は CI では動きません。このマシンで動かします。

## いつ何を動かすか

| 場面 | 動かすもの | `run-tests.ps1` |
|---|---|---|
| いつでも(数分、準備なし) | 3 プロジェクト(Stress を除く)、`translations\check.ps1` | `-Set quick`(約 4 分) |
| 変換器・アナライザー・Studio を変えた(コミットの前) | `FrameworkOnCore.Tests`、be・wt の変換と基準の比較、be・wt の Windows での画面の比較 | `-Set commit`(約 5 分) |
| FrameworkOnCore.Runtime を変えた | パッケージを作り直し(`pack-frameworkoncore.ps1 -Build All`)、3 プロジェクト(Stress を含む)、be・wt の変換と比較。直した所に関わるサンプルは `sample-parity.ps1 -Windows`/`-Linux` で個別に | `-Set runtime` |
| リリースの前 | runtime に加えて、wt・be のすべての操作、Windows のマトリクス、Linux(3 プロジェクト、be・wt、be のすべての操作) | `-Set full` |
| Linux も | commit・runtime に Linux の実行を足す | `-Linux` |

**ランタイムの版**: `FrameworkOnCore.Runtime/` を変えたら、`src/FrameworkOnCore.Converter/rules/packages.json` の `frameworkOnCoreVersion`(今は `1.6.5-w2l.27`。2026-11-02 の時点で未公開。w2l.26 は 2026-11-01 に公開済み)を
次の `w2l.N` に上げます。公開済みの版のまま作り直すと、ほかのマシンが古いパッケージを使い続けます(`pack-frameworkoncore.ps1` は公開済みの版の元が変わっていると止まる)。
テストだけの変更なら版は上げません。

## 新しいマシンに要るファイル

`experiments/wf4c/.gitignore` は `experiments/wf4c` の直下のフォルダーを生成物として無視しますが、次の 3 つは入力なので Git に入れています
(フォルダーのほかのファイルは生成物のまま無視)。

| ファイル | 中身 | 使うもの |
|---|---|---|
| `experiments/wf4c/_verify/sql-login.txt` | SQL Server Express のテスト用の SQL 認証のログイン(`User ID=foc_verify;Password=...`。このマシンだけのテスト用で、本物のアカウントではない) | `studio-verify.ps1`、`sample-parity.ps1`(データベースを使うサンプル: コンテナから host.docker.internal へ `SQLCONNSTR_` で)。新しいマシンでは SQL Server Express を混合モード認証・TCP 1433 にし、同じログインを作る |
| `experiments/wf4c/_culture/culture-profile.json` | このマシン(日本語の Windows、.NET Framework 4.8)のカルチャのデータ(`capture-culture.ps1` で採った。ゴールデンを採ったのと同じマシン) | `convert-corpora.ps1`(`--culture-profile`。変換結果の `App_Data/culture-profile.json` になり、変換の基準のハッシュにも入っている)、`probe-requests.ps1` の例 |
| `experiments/wf4c/_linux/page.ps1` | ページを curl で要求し(Cookie を保つ)、状態・タイトル・本文・フォームの欄を出す。`-Submit` でフォームを送り返す | `n2-install.ps1`(n2 のインストール。ふだんは動かさない) |

## 削除したもの(2026-10-31)

棚卸しで、どこからもビルド・実行されていない、または動かないと分かったもの。必要なら git の履歴(774856c0 以前)から戻せます。

| 削除したもの | 理由 | 合わせて直した参照 |
|---|---|---|
| `FrameworkOnCore.Runtime/src/WebFormsForCore.AjaxControlToolkit/AjaxControlToolkit.Tests` | 上流の net48 の NUnit のテスト。パッケージのフォルダーが無く、ビルドできなかった | 上流の `AjaxControlToolkit.sln` と `appveyor.yml` から外した |
| `FrameworkOnCore.Runtime/src/WebFormsForCore.TestApp`(csproj 2 つ) | 上流の試験用のアプリ。どのテストも使っていなかった | `FrameworkOnCore.Runtime/WebFormsForCore.slnx` から外した |
| `experiments/wf4c/iis-dump.ps1` | どこからも使われていない | なし |
| `experiments/wf4c/dnn-cycle.ps1` | dnn(be・wt 以外。ふだん動かさない)の Windows の一巡。動かなくなっていた | 記録(`experiments/wf4c/README.md`)に削除したと書いた |
| `experiments/wf4c/coverage/summary.py` | どこからも使われていない(アセンブリごとの網羅は `measure.py` が出す) | なし |
| `experiments/wf4c/perf-check.ps1`、`perf-baseline/`(`load.ps1` とその ASP.NET Core のアプリ)、`perf-counters.py`・`perf-frames.py`(`perf-check.ps1 -Diagnose`・`-Trace` の集計) | bombardier が要り、このマシンに無い。2026-10-13 の性能の調査のあと使っていない | 記録の性能の節に削除したと書いた。`iislog-check.ps1` の説明 |

`experiments/wf4c/coverage/file_coverage.py` は残しました(記録の「カバレッジ」に書いてある、ファイルごとの通っていない分岐を出す道具。
`--no-run` で今の結果から動くことを確かめた)。`load-check.ps1`(curl。bombardier は要らない)も残しました。

パッケージの版の確認(`experiments/wf4c/frameworkoncore-version.ps1`)は、パッケージにならないもの(`FrameworkOnCore.Runtime` の中の
`.sln`・`.slnx`・`appveyor.yml`、削除した 2 つのテストのフォルダー)を見ないようにしました。パックは `experiments/wf4c/frameworkoncore.slnx`
をビルドするので、パッケージの中身は変わりません。これで、公開済みの 1.6.5-w2l.25 のまま `pack-frameworkoncore.ps1 -Build All` が通ります
(GitHub Actions のビルドも)。

## IIS との比較の現状(2026-11-01、1.6.5-w2l.26)

w2l.18(2026-10-16〜19)のあと初めて、層 3 をすべて流し直しました(経緯と直したものは記録「IIS との比較の流し直し」)。前回と同じか、理由のある差だけです。

| 対象 | Windows(前回 → 今回) | Linux(前回 → 今回) |
|---|---|---|
| サンプル 33 本(ChartProbe を含む) | 30 本一致・既知の差 3 本 → 同じ | 30 本一致・既知の差 3 本 → 同じ(ChartProbe は SkiaSharp で初めて: 27 行一致) |
| 既知の差の 3 本 | BclProbe 2/7、DataFileProbe 1/2、LocaleProbe 29/32 → 同じ | BclProbe 2/7、DataFileProbe 0/2、LocaleProbe 28/32 → 同じ |
| NavigationProbe | 449(許容 5)→ 同じ | 449(許容 6)→ 同じ |
| WebControlsParity 124 | 不一致 0 → 0(既知の差 12 → 12。3 層とも件数も前回どおり) | 不一致 0 → 0(既知の差 16 → 14: WebPartManager の Hashtable の順が今回は偶然一致) |
| errors-check | 32/32 → 32/32(w2l.25 では 22/32: 直した) | ― |
| headers-check・rewrite-check(Iis)・iislog-check | 60/60・53/53・18/18 → 同じ | ― |
| compression-check | 24/29 → 25/29(残りは簡易ページの長さ: 決定どおり) | ― |
| load-check・proxy-check | エラー 0 → 0、不変条件どおり | エラー 0 → 0、不変条件どおり |

## 気をつけること(2026-11-01 更新)

- be・wt 以外のコーパスの変換の基準は、最後の変換器の変更(2026-10-31、FOC1009)と w2l.26 の前のものです。次に変換すると違いが出ます(版の違いだけのはず)。
- `verify-windows.ps1 -Name wt` は SQL Server Express を起動しません(`-Suite operations` は起動します)。
- ブラウザー(Edge)は自分で更新されます。User-Agent の版が出る画面は、シナリオで User-Agent を固定する(PageProbe は最初の手順の
  `newcontext`)。ブラウザーの能力は User-Agent の先頭 64 文字でキャッシュされる(.NET Framework も同じ)ので、途中で変えても効かない。
  `corpora/parity/wt.operations.golden.json` のエラーページには Edge 156 の User-Agent が入っている(Edge が 157 になると差が出るかもしれない)。
- sample-parity・check のスクリプトが使う IIS 側のサイト(`experiments/wf4c/_samples/<名前>/iis`)は `-Record` の時の写しです。サンプルを
  チェックアウトし直す(改行コードが変わるなど)と古くなるので、`*-check.ps1` の前に `-Record` で作り直す(ゴールデンは `git checkout` で戻す)。
- Linux の実行を重ねると Docker の build cache が増えます(今回 45 GB、ディスクがいっぱいになった)。`docker builder prune -af` と、Docker を止めて
  `docker_data.vhdx` を diskpart の `compact vdisk` で縮める。
