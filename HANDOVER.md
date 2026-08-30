# WebForm2Blazor 引き継ぎ

このドキュメントは**開発を引き継ぐ人**向けです。使い方・変換対応表・コマンドは
[README.md](README.md) にあります。ここには README に書いていないもの
——「なぜそう作ったか」「いまどこまで動くか」「何が壊れているか」——を書きます。

最終更新: 2026-08-14 時点の実測値。

---

## 1. 目的

.NET Framework の **ASP.NET WebForms アプリを Blazor Server へ変換する汎用ツール**です。
特定の 1 アプリを移植するスクリプトではなく、任意の WebForms アプリを入力に取れることを狙っています。

「汎用」を測るために、実在する 6 つの OSS WebForms アプリをコーパスとして変換し、
**変換できなかった箇所(残差)の件数**を指標にしています。残差が減る = 適用範囲が広がる。

意図的に**やらない**こと:

- 完全自動移行の約束。WebForms の設計判断(ポストバック、動的コントロール生成、
  HttpModule)には Blazor に自然な対応物がないものがあり、それらは人間が決めるべき残差として報告します。
- 「それらしく動く」出力。**動かないコードを黙って出すより、見える残差を出す**方を常に選びます。

---

## 2. 設計上の判断

引き継ぐ上でこれだけは踏襲してほしい、という順に並べます。

### 2.1 正解は「旧アプリが生成する HTML」

変換が正しいかどうかは、仕様書でも記憶でもなく、
**IIS Express で実際に動いている変換前の WebForms アプリの描画**で決めます。

`tools/WebForm2Blazor.ParityTest` が旧アプリを Playwright で操作して DOM スナップショットを
ゴールデンマスターとして記録し(`record`)、変換後の Blazor アプリに同じ操作をして
突き合わせます(`verify`)。現在 **30 スナップショットが完全一致**しています。

この規律から派生する重要な帰結:

- **既定値は実測から採る。** 「WebForms の `CellPadding` の既定は確か -1」ではなく、
  4.8 で実際に描かせて確認する。`samples/DefaultsProbe` はそのための実験台で、
  1 ページに 60 以上のコントロールを並べて既定レンダリングを固定しています。
- **書かれない既定値は残差にも出ない。** マークアップに `GridLines` が書かれていなければ
  「未対応属性」の残差は立ちません。属性のパリティ層(DefaultsProbe + PROPERTY-COVERAGE.md)だけが
  この穴を塞ぎます。残差 0 件は「完璧」ではなく「書かれた構文は全部読めた」の意味です。

### 2.2 決定的変換が主、AI は残差だけ

変換は 4 層構成です。

| 層 | 役割 | 実体 |
|---|---|---|
| 1. 棚卸し | 入力プロジェクトの走査、タグプレフィックス解決、基底クラス登録 | `Project/WebFormsProject.cs`, `BaseClassRegistry` |
| 2. 決定的変換 | ASPX パース → Razor 生成、Roslyn によるコードビハインド書き換え | `Parsing/`, `Emit/`, `Convert/` |
| 3. AI 残差層 | 決定的層が扱えなかった構文を LLM に投げる | `AI-TASKS.json` を出力(実行は外部) |
| 4. 検証 | bUnit / ビルド検証 / パリティ | `tests/`, `tools/` |

**AI は主役ではありません。** LLM に投げるのは「決定的層の穴」だけで、それが
`ResidualDisposition.Convertible` の定義です。AI 層で繰り返し出るパターンを見つけたら、
`ControlMappings` に「昇格」させて決定的層の守備範囲を広げる、という運用を想定しています。

### 2.3 残差を 3 分類する

`ResidualKind`(何が起きたか)とは別に `ResidualDisposition`(誰が対処すべきか)を持ちます。
これがないと「残差 1,272 件」という数字が意味を失います。

- **Convertible** — 変換できるはずのものを決定的層が扱えなかった。こちらの宿題。AI 層の対象。
- **ManualMigration** — 設計判断や外部依存。人間が決める。件数が多くても品質の問題ではない。
- **Informational** — 対処不要の通知。

コーパスの残差の大半は ManualMigration です(コーパスが持ち込む依存の量を測っているだけ)。
**追うべきは Convertible の数だけ**です。

### 2.4 認証は fail closed

`Compat/SystemWebStatics.cs` の `Membership.ValidateUser` は常に `false`、
`Roles.IsUserInRole` も常に `false` を返します。`FormsAuthentication.SetAuthCookie` は何もしません。

半分移行したサイトが**誰でも入れる状態になるより、誰も入れない方が安全**だからです。
ここを「とりあえず true」に変えないでください。

### 2.5 ClientID は変換時に静的計算する

WebForms の `ClientIDMode=Predictable`(4.x の既定)を再現します。
ContentPlaceHolder とユーザーコントロールは naming container なので、その ID が
子孫の `ClientID` に連結されます(`cphMain_ctrlWidget_pLabel`)。

Blazor には naming container の概念がないので、**変換時にプレフィックスを算出して
`<WebFormsNamingContainer Prefix="...">` として埋め込みます**。実測で確認済みの細部:

- マスターの自動 ID(`ctl00`)は `ClientID` には出ない(`name` 属性 = `UniqueID` にだけ出る)
- 素の HTML の `id` は決してプレフィックスされない
- ヘッダー/フッターテンプレートは行番号をずらさない

### 2.6 互換コンポーネントの「正直な no-op」

`System.Web` の API を Blazor 上で成立させるため `src/WebForm2Blazor.Components` に
互換層を置いています。対応物が存在しない API は**黙って近似せず、無害な no-op にして
その理由を XML ドキュメントコメントに書く**方針です。例:

- `Page.Master` → `dynamic` で `null`。WebForms ではマスターページのクラス型なので
  `Master.SetStatus(...)` のような呼び出しがコンパイルを通り、最初の使用で明示的に落ちます。
  ビルド全体を止めるより良く、かつ「動いているふり」もしない。
- `EnableViewState` / `ViewStateMode` → 受け取るが何もしない。Blazor はコンポーネントの
  フィールドがそのまま状態なので、切るべき往復ストアが存在しない。
- `HttpApplication` のパイプラインイベント → 購読できるが発火しない。
- `FileIOPermission.Demand()` → 何もしない。CAS は .NET Core で廃止済みで、
  実際のアクセス制御はファイルシステムが 1 行後に行う。

近似すると DOM が変わるもの(`RepeatLayout=Flow`、`ExtractTemplateRows`)は
**残差のまま残します**。これは 2.1 の帰結です。

---

## 3. 現状

### 3.1 構成

```
src/WebForm2Blazor.Converter          変換器        約 8,500 行 / コントロールマッピング 43 種
src/WebForm2Blazor.Components         互換ランタイム 約 8,900 行 / Razor コンポーネント 40 個
tools/WebForm2Blazor.ParityTest       Playwright ゴールデンマスター照合
tools/WebForm2Blazor.BrowserSmokeTest 実ブラウザ操作検証
tools/WebForm2Blazor.PropertyCatalog  実ランタイムから属性カタログ採取
tests/WebForm2Blazor.ConvertedAppTests bUnit
samples/                              変換の実験台 5 つ
```

### 3.2 全体回帰(`tools/verify-all.ps1`、exit 0)

| 段階 | 結果 |
|---|---|
| bUnit | 30 / 30 合格 |
| 変換出力のビルド検証 | 5 アプリすべてエラー 0 件 |
| パリティ(旧アプリ照合) | DefaultsProbe 4 / MasterProbe 3 / ProductAdmin 13 / OrderAdmin 10 = **30 スナップショット全一致** |

パリティは**生 ID 比較が既定**(`ParityScenario.CompareRawIds = true`)です。
ID を正規化して比較すると naming container の実装ミスを取り逃がすので、オプトアウト方式にしてあります。

サンプルの起動ポート(`verify-all.ps1` が使用):

| サンプル | 旧アプリ(IIS Express) | Blazor |
|---|---|---|
| DefaultsProbe | 8093 | 5096 |
| MasterProbe | 8094 | 5097 |
| ProductAdmin | 8091 | 5090 |
| OrderAdmin | 8092 | 5095 |
| HelloWebForms | — | 5080 |

### 3.3 コーパス実測

再現条件を揃えた 5 コーパスの変換結果です。

| コーパス | 移植 .cs | 総残差 | うち Convertible |
|---|---:|---:|---:|
| BlogEngine 3.3.8 | 252 | 82 | 6 |
| mojoPortal 3.1.6 | 569 | 274 | 20 |
| YAF.NET 3.2.15 | 662 | 77 | 5 |
| DNN Platform 9.13.10 | 1,308 | 346 | 12 |
| WingtipToys | 12 | 38 | 3 |
| **計** | | **817** | **46** |

直前の計測は総残差 880 / Convertible 110 だったので、
**Convertible を 110 → 46 に削減**しています。
**生成 Razor の RZ(Razor 構文)エラーは全コーパスで 0 件**です。

nopCommerce 3.8 は Convertible 32 件でしたが、今回は再計測していません(下記 4.3)。

### 3.4 BlogEngine の実ビルド

コーパスの中で唯一「変換出力を実際にビルドして起動まで持っていく」対象にしています。

**ビルドエラー 230 件 → 66 件**(クリーンビルド、`--no-incremental`)。変換エラーは 0 件。

大きかった修正 2 つ:

1. **partial クラスの分断(約 130 件)。** `post.aspx` は `Inherits="post"` で、
   コードビハインドが `Post` 型のメンバを持つため、変換器はコンポーネント名を
   `PostComponent` に退避していました。ところがコードビハインド側はクラス名 `post` のままで、
   `.razor` が生成する `PostComponent` と別クラスになっていた。`@ref` フィールドが全滅し、
   `@inherits` も本来の `BlogBasePage` ではなく既定の `WebFormsPage` になっていました。
   → `ConvertedComponent.SourceClassName`(コードビハインドが実際に宣言している名前)を
   持ち回り、リライタと `ResolveInheritsBase` の双方がそれで検索するようにして解決。

2. **csproj 非参照の孤児ファイル(約 30 件)。** `BlogEngine.Core/Profile.cs` は
   csproj に含まれていないのに `--include` がディレクトリ走査で移植していました。
   C# は同一名前空間の型を using より優先するため、本物の
   `BlogEngine.Core.Data.Models.Profile` を覆い隠していた。
   → `WebFormsProject.ReadCompiledFiles` が `<Compile Include>` を読んで絞り込みます。
   **ただし SDK 形式(`<Project Sdk=...>`)は既定で `**/*.cs` をグロブする**ので、
   明示 Include だけを見ると逆にほぼ全滅します(YAF で 662 → 17 件に激減させた)。
   SDK 形式・ワイルドカード・csproj が一意でない場合は走査にフォールバックします。

---

## 4. 未解決の問題

### 4.1 BlogEngine の残り 66 件は性質が違う

上位は `UserControlSettings.razor.cs` 17 / `_default.razor.cs` 5 /
`RazorHostSite.razor.cs` 4 / `CommentList.razor.cs` 4 / `WidgetContainer.cs` 4。

いずれも**実行時にコントロールツリーを走査・組み立てる**コードで、`(TextBox)ctl` のような
キャストが `Control`(レガシー描画系)と `TextBox`(コンポーネント系)という
**別の型階層をまたぐ**ために落ちています。互換メンバを足しても埋まりません。
Blazor は子ツリーをマークアップが持つので、`@foreach` などマークアップ側への書き換えが要ります。

残りは BlogEngine 自身が除外した型(`RazorHelpers`、`Search.Hits`、`BlogSettings.StorageLocation`)への参照です。

**つまり機械的に潰せる分はほぼ出し切っています。** 起動到達には手動移行を書くか、
該当ページを一時的に除外する判断が必要です。

### 4.2 残り 46 件の Convertible 残差

| 件数 | 内容 |
|---:|---|
| 9 | `<script runat="server">` ブロック(コードビハインドへの移動が必要) |
| 7 | `<asp:ListItem resourcekey="...">` で対応する .resx が存在しない(mojo/yaf) |
| 6 | テンプレート外のデータバインド式 |
| 5 | タグ境界を跨ぐインラインコード `<% if (...) { %>` |
| 3 | enum 型パラメータ(`<dnn:Profile EditorMode="View">` など、下記) |
| 2 | `Register` ディレクティブの `Src` に対応する .ascx が無い |
| 14 | その他個別 |

**意図的に残しているもの:**

- **enum 型パラメータ 3 件。** 生成スタブの型が enum かどうか変換時に判定できず、
  当てずっぽうで `@(T.View)` を出すとコンパイルが通らなくなります。
- **`ExtractTemplateRows` 2 件。** 近似すると DOM が変わります(2.6 の方針)。

### 4.3 コーパス測定の再現性

**残差の前後比較は変換オプションを揃えて初めて成立します。** `--input` だけ合わせても比較になりません。
実際に踏んだ罠:

- `--include` を渡さないと YAF の移植 .cs が 662 → 17 になり、`<YAF:LocalizedLabel>` などが
  LegacyRenderHost に解決されず UnmappedControl が 2,000 件超に爆発する
- YAF はサイトルートに `Web.config` が無く、`recommended.web.config` を `--web-config` で
  指定しないと `tagPrefix` を読まない
- mojoPortal は `--control-map` が無いと `<mp:mojoGridView>` が未対応コントロール扱いになる
- WingtipToys の入力ルートは `wingtiptoys-master\WingtipToys\WingtipToys`(1 階層深い)

**判定方法: 変換レポート冒頭の「そのまま移植した .cs」が前回と一致すること。**
一致しなければオプションが違います。

再現手順は `corpora/` にあります(下記 5.1 で解消済み)。

```powershell
.\corpora\fetch.ps1          # コーパス取得(約 115MB)
.\corpora\convert-all.ps1    # 全件変換 + ベースライン比較。一致で exit 0
```

オプションは `convert-all.ps1` に固定してあるので、コマンドラインから崩せません。
詳細は [corpora/README.md](corpora/README.md)。

nopCommerce 1.90 だけは自動取得できません。1.x は CodePlex 時代のリリースで、
GitHub の `nopSolutions/nopCommerce` に 1.x タグが存在せずミラーも見つかりませんでした。
なお以前 `samples/nopCommerce3.8` にあったものは **3.8 = ASP.NET MVC** で、
このコーパス(1.90 = WebForms)とは別物です。272MB あり変換対象にもならないため
Git 管理から外しました(`.gitignore`)。

### 4.4 計測時の落とし穴(実際に誤報告した)

- **インクリメンタルビルドが汚染される。** 変換出力のビルドエラー数を測るときは
  **必ず `--no-incremental`**。ハングしたビルドを kill すると stale な `obj/` が残り、
  「391 → 39 → 3 → 1 件」と報告した後にクリーンビルドで 705 件出たことがあります。
- **出力ディレクトリの残骸。** 前回実行の生成物が残っていると変換レポートが混ざります。
  比較計測は必ず新しい出力先に対して行ってください。
- **PowerShell 5.1 は BOM 無し UTF-8 を ANSI として読む。** 日本語を含む `.ps1` は
  BOM 付き UTF-8 で保存しないと文字化けし、文字列リテラルが壊れます。
- **bUnit と HTML 文字列チェックだけでは不十分。** Blazor Server は「HTML が返る」と
  「操作が効く」の間に JS 配信と SignalR 接続という故障点があります。
  必ず `BrowserSmokeTest`(Playwright + Edge channel)で実ブラウザの操作まで通してください。

### 4.5 README の末尾 2 節が古い

`## 現状の変換実績` の表は DefaultsProbe / MasterProbe が抜けています。
`## 既知の制約` に挙がっている項目のうち、ValidationGroup / RangeValidator /
ListView / FormView / DetailsView / パリティテストは**実装済み**です。更新が必要です。

---

## 5. 次にやること

優先度順です。

### 5.1 コーパス再現環境をリポジトリに入れる — **完了**

`corpora/` に入れました。素の状態(fetch → convert)から 3.3 の数字が再現することを確認済みです。

- `corpora/fetch.ps1` — 5 コーパスを既知のタグで取得
- `corpora/convert-all.ps1` — オプション固定で変換し、ベースラインと比較(差異があれば exit 1)
- `corpora/expected.json` — ベースライン(移植 .cs / 総残差 / 変換可能)
- `corpora/mojo-control-map.json` — mojoPortal 用コントロールマップ
- `corpora/README.md` — 罠と判定の勘所

nopCommerce 1.90 のみ自動取得できません(4.3 参照)。

### 5.2 BlogEngine を起動まで到達させる

4.1 の 66 件。コントロールツリー走査コードの手動移行が本体です。
「変換ツールが吐いた Blazor アプリが実際に起動してブラウザで動く」という
最初の実例になるので、価値は高いです。

### 5.3 AI 残差層を実運用する

`AI-TASKS.json` は出力されていますが、**それを LLM に投げて適用するループが未実装**です。
46 件の Convertible 残差が最初の入力になります。
適用後は必ず `verify-all.ps1` で回帰を確認してください。

### 5.4 属性パリティの拡充

2.1 の通り、書かれていない既定値は残差に出ません。`PROPERTY-COVERAGE.md` と
`webforms-property-catalog.json` が唯一の網です。`PropertyCatalog` ツールで
実ランタイムから採取した属性と、実装済みパラメータの差分監査を定期的に回してください。

### 5.5 CI

現状 `verify-all.ps1` は手元でしか回りません。IIS Express と .NET Framework の
MSBuild が要るのでパリティ層は Windows ランナー必須です。
最低限 bUnit + `--verify-build` だけでも CI に載せる価値があります。

---

## 6. 引き継ぎメモ

- **コミット前に必ず `tools/verify-all.ps1` を通す。** exit 0 が最低ライン。
- **変換器を触ったら `corpora/convert-all.ps1` も通す。** こちらも exit 0 が最低ライン。
- 互換コンポーネントに API を足すときは、**動く実装か、理由を書いた no-op か**の
  どちらかにしてください。中途半端な近似が一番たちが悪い。
- 数字が動いたら、**まず「移植 .cs」を見る。** そこが動いていれば原因は変換器ではなく
  オプションかコーパス側です(4.3)。
- 迷ったら「旧アプリが実際に何を出しているか」を見に行く。それが唯一の正解です。
