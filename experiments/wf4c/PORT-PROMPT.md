# 未移植の部品を移植するプロンプト

未移植の部品(FRAMEWORK-ONLY-APIS.md の A 表でスタブのもの)を referencesource から移植するときに使うプロンプト。
新しいセッションで次のように指示する。

> experiments/wf4c/PORT-PROMPT.md のプロンプトに従って、<部品> を移植して

対象と推奨順:

1. ~~System.Data.Linq(LINQ to SQL)~~ … **移植済み**(2026-09-30、パッチ 0032。experiments/wf4c/README.md の記録)
2. ~~System.Web.DataVisualization(グラフ)~~ … **移植済み**(2026-10-01、パッチ 0039・0040。新旧比較は `tests/DataVisualizationParity`、Web Forms のサイトでの確認は `samples/ChartProbe`。experiments/wf4c/README.md の記録。nop での実地検証は未実施)
3. System.Data.Services.Client … 優先度低。着手前に要否をユーザーに確認
4. ~~System.Drawing(System.Drawing.Common の Linux 実装)~~ … **移植済み**(2026-09-30、パッチ 0034〜0037。dotnet/runtime 6.0 のソース。新旧比較は `tests/DrawingParity`。experiments/wf4c/README.md の記録)

System.ServiceModel(CoreWCF)は「移植」ではなく別の作業なので、このプロンプトの対象外。

新旧比較のケースを作るときは `tests/Parity.Core`(観測値の書き方 Probe、ケースの実行 Runner、ゴールデンの書き出し)を使う。API が多い部品は、System.Drawing の `tests/DrawingParity/ApiCases.cs` のように、ゴールデンの API 一覧から「型とメンバー名ごとに 1 ケース」を作り、呼べない・呼ばないメンバーは理由つきで除外する。環境による差(Linux の描画など)は `known-differences.json` のように理由つきで持ち、使われなくなった項目はテストで失敗させる。

---

## プロンプト本文

あなたは FrameworkOnCore(.NET Framework の Web アプリを .NET 10 / Linux に移す変換器)のリポジトリで作業します。

**課題: <部品> を referencesource(MIT)から移植し、今のスタブを実装に置き換える。**

品質を最優先します。速さのために調査・テスト・記録を省かないでください。ここに書いた基準と CLAUDE.md の決まり(コーパス検証は be/wt のみ、変換器を直したら類似の問題を調査、報告は日本語、コンテナは 1 つずつ・検証後は Docker Desktop 終了と `wsl --shutdown`)を守ってください。

### 1. 調査(実装より先に。結果を最初に報告する)

1. **必須カバー一覧を作る**: コーパスが実際に呼ぶ <部品> の API を列挙する。材料は各コーパスの CONVERSION-REPORT.md・api-analysis.json(スタブ・削除の一覧)と、コーパスのソースの検索。「型・メンバー・使うコーパス・呼ばれ方」の表にする。これがテストの網羅性の物差しになる。
2. **ソースの確認**: microsoft/referencesource の該当ソースの場所と、MIT ライセンスであることを確認する。`_upstream` に同名のプロジェクトが既にあるなら(例: WebFormsForCore.Web.DataVisualization)、それを生かす。
3. **既存スタブとの衝突を洗い出す**: 移植版が入ると重複する型を全部挙げる。見る場所:
   - `src/FrameworkOnCore.Converter/rules/packages.json` の `noAnswer`
   - `experiments/wf4c/shims/FrameworkOnCore.Compat.ForDlls`(例: LinqBinary.cs の System.Data.Linq.Binary)と `FrameworkOnCore.Compat` の RemovedTypes/RemovedMembers
   - `src/FrameworkOnCore.Analysis/catalog/components.json` の該当部品(`port` の選択肢が `planned: true` である)
   - AssemblyRetargeter の優先アセンブリ(付け替え先が変わる)
4. **先例を読む**: フォークの既存の移植のやり方を踏襲する。パッチ 0001(DynamicData をビルドできるようにした)と、既存プロジェクトの csproj(例: WebFormsForCore.Web.Services)がひな形。

調査の結果(必須カバー一覧・衝突一覧・方針)を報告してから実装に入ること。

### 2. コーディングルール

**移植するコード(referencesource 由来):**

- 元のソースを変えないのが原則。整形・改名・「現代風への書き換え」をしない。Microsoft のライセンスヘッダーとコメントは残す。
- 変更してよいのは「.NET でビルド・動作させるため」だけ。upstream と同じ流儀(`#if NETFRAMEWORK` / `#if NETCOREAPP` の分岐)で囲み、変更点には理由を一行コメントで書く。
- `_upstream` への変更はすべてパッチ(`experiments/wf4c/patches` の次の番号)にする。1 パッチ = 1 目的で、件名に目的を書く。

**新しく書くコード(テスト、接続部):**

- リポジトリの既存コードの流儀に合わせる: file-scoped namespace、xUnit、クラスの XML doc は英語で「何を・なぜ」(例: tests/FrameworkOnCore.Tests/SpanInExpressionAnalyzerTests.cs)、テスト名は文(`An_arrays_contains_in_a_query_is_found`)。
- コメントの密度も周囲に合わせる。自明なことは書かない。
- ビルドの警告を増やさない。

**プロジェクト:**

- csproj は既存のフォークのプロジェクトをひな形にし、TFM・署名・版・パッケージの体裁を揃える。

### 3. テスト(網羅性の基準)

1. **テストマトリクスを実装より先に書く**。行 = 機能領域、列 = 正常系 / 境界(null・空・大きい値・文化依存) / 異常系(例外の型と動きが .NET Framework と同じか)。§1 の必須カバー一覧の行は全部埋める。テストしない行には理由を書く(例: コーパス未使用でスタブのまま)。マトリクスはテストのフォルダーか experiments/wf4c/README.md の記録に残す。
2. **.NET Framework との動作互換が要点になる箇所は、期待値を .NET Framework 側で記録して比較する**(net48 の小さなハーネスで実行した結果をゴールデンとして保存。corpora/record-webforms-golden.ps1 と同じ考え方)。
3. **全 API の新旧比較を作る**(System.Data.Linq の `tests/DataLinqParity` が手本): 同じケースのソースを net48(.NET Framework の本物)と net10.0(移植版)でビルドし、旧でゴールデンを採って新と比べる。API 一覧の一致・全 API にケースがあること(Roslyn で機械的に照合)・移植版に Debug.Assert が無いことも検査する。正規化はランタイム・SQL クライアント・サーバーの差に限り、理由を書く。
4. **Windows と Linux の両方で実行する**: `dotnet test tests\FrameworkOnCore.Tests` と `tests\FrameworkOnCore.Tests\run-tests-linux.ps1`。DB が要るテストは、Windows は SQL Server Express(終わったら閉じる)、Linux はコンテナ(1 つずつ)。
4. 既存の 69 件のテストも含めて全部通す。

### 4. 変換器への組み込み

- `rules/packages.json`: `noAnswer` から外し、`frameworkReferences` に `"<アセンブリ名>": [ "<フォークのパッケージ名>", "$fork" ]` を足す。
- `catalog/components.json`: 該当部品の `port` の選択肢から `planned` を外し、既定の選択を見直す。
- 重複する型のスタブ(ForDlls・Compat)を削除し、参照の付け替え先を移植版にする。
- 解析(TargetApis)が移植版の API を「ある」と判定することを確認する。
- 変換器を直したら、類似の問題が他の場所にないか調査する(CLAUDE.md)。

### 5. 検証

- コーパスの実地検証は **be/wt のみ**。<部品> を使う箇所が be/wt に無い場合(DataVisualization は nop のみ)は、勝手に検証せず、どのコーパスで検証するかをユーザーに確認する。
- Linux の検証が終わったら Docker Desktop を終了し、`wsl --shutdown`。

### 6. 配布と記録

- フォークの版を上げる: `pack-fork.ps1 -Build All -Version`。更新箇所は rules/packages.json の `forkVersion`、shims の csproj、template.csproj.txt、convert-project.ps1、pack-fork.ps1 の既定。配るときは `publish-fork.ps1` で GitHub Release に置く。新プロジェクトは pack-fork.ps1 の `$projects` にも足す。
- ドキュメント: FRAMEWORK-ONLY-APIS.md(スタブ → 移植済み)、experiments/wf4c/README.md に記録(日付・やったこと・テストマトリクス・既知の課題)、ルート README.md の「状態」。
- コミット: メッセージは UTF-8(BOM 無し)のファイル(`.git/MSG.txt` に printf)で `git -c user.name=shkosugi -c user.email=sin.kosugi@gmail.com commit -q -F`。`git add -A -- . ':!CLAUDE.md'`(CLAUDE.md は含めない)。末尾はその時の環境の指示の Co-Authored-By 行。push は指示があってから。

### 完了の定義

- [ ] 必須カバー一覧(コーパスが呼ぶ API)がすべて実装され、テストで覆われている
- [ ] テストマトリクスが記録され、空欄には理由がある
- [ ] テストが Windows / Linux とも全緑(既存テスト含む)
- [ ] be/wt(または指定されたコーパス)で該当機能が動く
- [ ] スタブ・カタログ・規則の矛盾が無い(重複型なし、planned 解除、noAnswer から除去)
- [ ] フォークの版上げと配布、ドキュメント更新、コミットが済んでいる
- [ ] Docker Desktop 終了、wsl --shutdown、SQL Server Express 停止

---

## 部品別の補足

### System.Data.Linq(LINQ to SQL)

- 使うコーパス: be(DbFileSystemProvider / FileStoreDb)、yaf、n2。**be で実地検証できる**(DB ファイルシステムの設定を有効にして確かめる)。
- `_upstream` にプロジェクトが無いので、新プロジェクト(例: WebFormsForCore.Data.Linq、アセンブリ名 System.Data.Linq)をパッチで追加する。ソースは referencesource の System.Data.Linq。
- 衝突: ForDlls の LinqBinary.cs(System.Data.Linq.Binary。csproj のコメントに「LINQ to SQL の移植が持つ」と明記済み)。削除して付け替え先を移植版にする。
- System.Web.Extensions で外されている LinqDataSource(LINQ to SQL 前提)を戻せるかも確認する。
- テスト領域の例: 属性マッピング(Table/Column/Association)と DBML 生成コードの互換、DataContext の CRUD と SubmitChanges、クエリ変換(`DataContext.GetCommand` の SQL を確認)、Binary・EntitySet・EntityRef、変更追跡、同時実行(ChangeConflictException)、トランザクション、DataLoadOptions。
- SQL Server の型・接続は Windows(SQL Server Express)と Linux(コンテナの SQL Server)の両方で。

### System.Web.DataVisualization(グラフ)

- `_upstream` に WebFormsForCore.Web.DataVisualization(約 115 ファイル)が既にあるがビルド対象外。DynamicData のパッチ 0001 と同じ方法で、ビルドできるようにするパッチを書き、pack-fork.ps1 に足す。
- 描画は System.Drawing。フォークの WebFormsForCore.Drawing.Common(移植済み。Windows は GDI+、Linux は libgdiplus)を参照する。Linux の描画は Windows と少し違う(アンチエイリアス、フォント)ので、判定は安定した性質で行う。
- 使うコーパスは nop(管理画面のレポート 2 つ)のみ → 実地検証の前にユーザーに確認する。
- テスト領域の例: コーパスが使うグラフの種類の描画(画像はピクセル一致ではなく、サイズ・形式・空でないことなど安定した性質で判定)、ChartImg.axd ハンドラー、ImageStorageMode、web.config の登録が変換で残ること。

### System.Data.Services.Client(WCF Data Services)

- 使うコーパス: be(ギャラリー)、mojo。使われ方が限られ優先度は低い。着手前に、必要な範囲(クライアントだけか)をユーザーと決める。
