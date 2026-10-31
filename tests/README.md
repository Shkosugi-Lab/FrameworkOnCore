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

Windows での件数と時間は 2026-10-31 の実行(`dotnet test`、ビルド込み)。Linux の件数は記録の最新(同日の NavigationProbe の節)。

| プロジェクト | 中身 | 件数(Windows) | 時間 | CI(Linux) |
|---|---|---|---|---|
| `tests/FrameworkOnCore.Tests` | 下の表 | 4,630(スキップ 1。SQL Server が無いとさらに 25) | 約 1 分 | 実行(Linux 4,583) |
| `tests/RuntimeUnitTests` | 下の表 | 1,977(スキップ 1) | 約 4 分(Stress が 1 分 40 秒) | 実行(Linux 1,969) |
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
| 負荷・性能(数値を見るだけ) | `load-check.ps1`、`perf-check.ps1`、`perf-baseline/load.ps1`、`proxy-check.ps1`(nginx の後ろ。不変条件) | なし | IIS・Docker。perf は bombardier(`%TEMP%\perf`、このマシンには無い) | |
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
| Linux の be・wt | `.\experiments\wf4c\run-linux.ps1 -App be\BlogEngine\BlogEngine.NET -Scenario corpora\regression\be.scenario.json -Golden corpora\parity\be.golden-webforms.json`<br>`.\experiments\wf4c\run-linux.ps1 -App wt\WingtipToys\WingtipToys -Scenario corpora\regression\wt.scenario.json -Golden corpora\parity\wt.golden-webforms.json -SqlServer`<br>`.\experiments\wf4c\run-linux.ps1 -App wt\WingtipToys\WingtipToys -SqlServer -Suite operations` | 同上 | Docker | |

期待される結果: be 5/5、wt 8/8(Windows)。Linux は be 5/5、wt 7/8(error-page: Docker のポート転送で `Request.IsLocal` が偽。以前からの既知の差)。
ゴールデンの採り直し(IIS、管理者): `.\corpora\record-webforms-golden.ps1 -Only be`、`.\corpora\record-wt-operations.ps1`(詳しくは [corpora/README.md](../corpora/README.md))。

## 5. 環境のマトリクスと Linux

| 何を | コマンド | 時間 |
|---|---|---|
| Windows で 6 カルチャ(ja-JP・en-US・invariant・tr-TR・de-DE・ar-SA)× 3 プロジェクト | `.\tests\run-matrix.ps1` | 約 30 分(推定) |
| Linux で 3 カルチャ × 3 タイムゾーン | `.\tests\run-matrix.ps1 -Os Linux` | 長い(推定 1 時間以上) |
| Linux の FrameworkOnCore.Tests と SqlClientTests | `.\tests\FrameworkOnCore.Tests\run-tests-linux.ps1 -SqlServer`(SQL Server のコンテナ w2l-sql も) | |
| Linux の RuntimeUnitTests | `docker run --rm -m 6g -v "${PWD}:/repo:ro" mcr.microsoft.com/dotnet/sdk:10.0 bash /repo/tests/RuntimeUnitTests/run-tests-linux.sh` | |

Linux の実行の約束(CLAUDE.md、メモリ 16 GB): コンテナは 1 つずつ順番に(バックグラウンドで重ねない)。終わったら Docker Desktop を終了して `wsl --shutdown`
(`run-matrix.ps1` と `run-tests.ps1` は自分で行う)。使わない SQL Server Express と Edge は閉じる。
`run-tests-linux.*` は、テストが失敗しても終了コード 0 で終わります。出力の `Failed!` を見てください(`run-tests.ps1` は出力で判定します)。

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
| リリースの前 | runtime に加えて、wt のすべての操作、Windows のマトリクス、Linux(3 プロジェクト、be・wt) | `-Set full` |
| Linux も | commit・runtime に Linux の実行を足す | `-Linux` |

**ランタイムの版**: `FrameworkOnCore.Runtime/` を変えたら、`src/FrameworkOnCore.Converter/rules/packages.json` の `frameworkOnCoreVersion`(今は `1.6.5-w2l.25`、公開済み)を
次の `w2l.N` に上げます。公開済みの版のまま作り直すと、ほかのマシンが古いパッケージを使い続けます(`pack-frameworkoncore.ps1` は公開済みの版の元が変わっていると止まる)。
テストだけの変更なら版は上げません。

## 気をつけること(2026-10-31 の棚卸し)

- サンプル 32 本をすべて比べたのは w2l.18(2026-10-19)が最後です。その後は be・wt と、ShiftJisProbe・ShiftJisVbProbe・NavigationProbe だけ。
  ChartProbe の Linux の結果は libgdiplus のとき(SkiaSharp に変える前)のもの。
- WebControlsParity の `reports/` は 2026-10-16 の実行です。IIS の機能の `*-check.ps1` も 2026-10-16 から動かしていません。
- be・wt 以外のコーパスの変換の基準は、最後の変換器の変更(2026-10-31、FOC1009)の前のものです。次に変換すると違いが出るかもしれません。
- `verify-windows.ps1 -Name wt` は SQL Server Express を起動しません(`-Suite operations` は起動します)。
