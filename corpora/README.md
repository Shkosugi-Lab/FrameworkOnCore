# コーパス

変換の適用範囲は、実在する WebForms アプリを変換したときの**残差の件数**で測ります。
その測定対象をここで取得・変換・比較します。

コーパス本体はサードパーティのソースで、それぞれ独自のライセンスを持ちます。
リポジトリには含めず、`fetch.ps1` が既知のバージョンを `corpora/work/` に取得します
(`corpora/work/` と `corpora/out/` は `.gitignore` 済み)。

## 使い方

```powershell
.\corpora\fetch.ps1          # コーパスを取得(初回のみ、約 140MB)
.\corpora\convert-all.ps1    # 全件変換してベースラインと比較
```

`convert-all.ps1` はベースライン(`expected.json`)と一致すれば exit 0、
差異があれば exit 1 を返します。**変換器を触ったら必ずこれを回してください。**

```powershell
.\corpora\convert-all.ps1 -Only be,yaf     # 対象を絞る
.\corpora\convert-all.ps1 -SkipBuild       # 変換器のビルドを省く
.\corpora\convert-all.ps1 -UpdateBaseline  # 実測値を expected.json に書き戻す
```

`-UpdateBaseline` は**変化の理由を説明できるときだけ**使ってください。
数字が動いた理由を確認せずに更新すると、ベースラインは回帰検知の役に立たなくなります。

## 対象

| 名前 | コーパス | 取得元 |
|---|---|---|
| `be` | BlogEngine.NET 3.3.8.0 | `rxtur/BlogEngine.NET` @ `v3.3.8.0` |
| `mojo` | mojoPortal 3.1.6 | `i7MEDIA/mojoportal` @ `v3.1.6` |
| `yaf` | YAF.NET 3.2.15 | `YAFNET/YAFNET` @ `v3.2.15` |
| `dnn` | DNN Platform 9.13.10 | `dnnsoftware/Dnn.Platform` @ `v9.13.10` |
| `n2` | n2cms | `n2cms/n2cms` @ `master` |
| `wt` | WingtipToys | `corn-mendoza/wingtiptoys` @ `master` |

`n2` は**ホールドアウト**として後から追加したものです。他の 5 本は変換器を育てる過程で
使ってきたため、それらへの適合しすぎを検出する対照が必要でした。追加時点の初回変換で、
他の 5 本が一度も踏まなかった穴(ジェネリックなページ基底クラス)を実際に露出させ、
生成 Razor のエラーが 1 件から 108 件に跳ね上がっています。

**変換器を変更したら、`n2` の数字を特に見てください。** ここが動くときは、他 5 本に
合わせ込んだ変更である可能性があります。

### nopCommerce 1.90 について

6 つめのコーパスとして以前計測していましたが(残差 264 / 変換可能 32)、
**現在は自動取得できません。** nopCommerce 1.x は CodePlex 時代のリリースで、
GitHub の `nopSolutions/nopCommerce` に 1.x のタグが無く、ミラーも見つかりませんでした。

手元にアーカイブがある場合は `corpora/work/nopcommerce-1.90` に展開すれば計測できます
(`convert-all.ps1` の対象表への追加が必要)。

なお、以前リポジトリにあった `samples/nopCommerce3.8` は **3.8 = ASP.NET MVC** で、
このコーパス(1.90 = WebForms)とは別物です。変換対象にはなりません。

## 変換オプションは固定してある

**残差の前後比較は変換オプションを揃えて初めて成立します。** `--input` だけ合わせても
比較になりません。オプションを 1 つ落とすと数字は桁で動きます。

そのためオプションは `convert-all.ps1` の表に固定してあり、コマンドラインからは変えられません。
実際に踏んだ罠:

| コーパス | 必要なオプション | 落とすとどうなるか |
|---|---|---|
| `be` | なし | — |
| `mojo` | `--include mojoPortal.Data.MSSQL`、`--control-map` | プロバイダ未指定ではデータ層が移植されない / `<mp:mojoGridView>` が未対応コントロール扱いになる |
| `yaf` | `--project YAF-SqlServer.csproj`、`--web-config recommended.web.config` | `.csproj` が 4 つあり自動導出されない / `tagPrefix` が読めず `<YAF:LocalizedLabel>` などが UnmappedControl に爆発する |
| `dnn` | なし | — |
| `n2` | `--project N2.Templates.csproj`、`--expression-map` | `.csproj` が 7 つある / 独自の式ビルダーが全て残差になる |
| `wt` | なし(入力ルートが 1 階層深い) | — |

### 式ビルダーの対応表(`--expression-map`)

n2 の「変換可能」残差 59 件のうち **23 件は独自の式ビルダー**でした
(`<%$ CurrentItem: Title %>` など)。AI 残差層のタスクを生成させると 17 件全部が
これで、**AI に投げる前に決定的層で片が付く**種類のものでした。

対応表は推測ではありません。各ビルダーが**変換先の C# 式を自分で宣言しています**。

```csharp
// src/Framework/N2/Web/Compilation/CurrentItemExpressionBuilder.cs
get { return @"N2.Web.Compilation.CurrentItemExpressionBuilder.GetCurrentItemValue(""{0}"")"; }
```

`corpora/expression-maps/n2.json` はこの文字列を `{0}` → `{value}` に置き換えただけです。
呼び先の静的メソッドは N2 本体と一緒に移植されるので、新しい実装は要りません。

**`Code` プレフィックスは意図的に外してあります。** `CodeExpressionBuilder` は式を
そのまま埋め込む(`CodeSnippetExpression`)ので対応表は `{value}` になりますが、
値が文字列リテラルを含むと(`<%$ Code: "AutoZone2" %>`)`@(\"AutoZone2\")` という
**不正な Razor** になります。

**2 回目の試行も撤回しました(変換可能 25 → 22、ビルドエラー 48 → 56)。** 原因を
「属性の引用符」と読んで、式が `"` を含むときは属性を `'` で囲むようにしましたが
(`RazorExpressionAttribute`、これは正しいので残してあります)、生成物はこうでした。

```razor
<Stub_n2_Zone ID="Zone7" ZoneName='@(\"AutoZone2\")' @ref="Zone7">
```

**引用符ではなく値そのものが `\"` にエスケープされています。** マークアップの属性値を
読む経路で入るもので、式ビルダーの値として取り出す時点で既に壊れています。エミッタが
「生コード」と「文字列値」を区別する、という当初の見立ては正しく、そこを直すまで
`Code` は残差のままです。

### `Visible="<%$ ... %>"` は式ビルダーの経路から外れていた

`<a runat="server" Visible="<%$ HasValue: LogoUrl %>">` が、**プレフィックスを
`--expression-map` に登録済みなのに**残差になっていました。

`runat="server"` の HTML 要素の `Visible` だけは属性処理の手前で分岐しており、そこが
`<%# %>`(データバインド)しか見ていなかったためです。他の全属性が通る変換を同じ
ように通すようにしました。**n2 の変換可能残差 27 → 25。**

**参照ライブラリの `--include` は不要になりました。** 変換器が入力アプリの `.csproj` から
`ProjectReference` を推移的に辿って自動で移植対象にします。ここに残っているのは
機械が決められない 2 種類だけです。

- `--project` — `.csproj` が複数あるとき。データベースごとにビルド構成を分ける実装が該当し、
  推測せず何も導出しません
- `--include` — 同じ型を宣言する排他プロジェクト群(`mojoPortal.Data.*`)から 1 つ選ぶとき。
  どれも自動採用しません

YAF はサイトルートに `Web.config` が無く、配布時に `recommended.web.config` を
リネームする前提になっています。これを渡さないと `tagPrefix` が読めません。

### 判定の勘所

**「移植 .cs」の数が動いていたら、変換器ではなくオプションかコーパスの取得内容が違っています。**
その状態で残差を比較しても意味がありません。`convert-all.ps1` はこれを最初に見ます。

## ベースライン(`expected.json`)

```
コーパス  移植 .cs  総残差  変換可能  ビルドエラー
be            255      75         0             0
n2           1668     201        25            39
mojo          741     105        19            50
yaf          2724      71         5            40
dnn          2040     178        10            87
wt             13      39         3             0
合計                  669        62           216
```

**ビルドエラーの数には「未決の依存によるもの」を含めていません。** リポジトリ同梱 DLL の
置き換え先が未決定なために型が見つからないエラーは、変換の欠陥ではなく
`--package-map` で解消するものだからです。別掲で **dnn 147 / mojo 103**(下記)。

### AI 残差層が実際に届く範囲

`--ai-tasks` の「9 件」は**ファイル数であって残差数ではありません。** 1 タスク =
1 つの `.razor` とその中の残差群です。実測すると **9 ファイル / 残差 18 件**。

変換可能残差 82 件との差は 2 つの絞り込みで生じています。

| | 件数 | 理由 |
|---|---|---|
| `ActionableKinds` に無い | 36 | 「未対応の属性」は対象外。うち 18 件は `DataSourceID` 系(宣言的データバインド 1 機能) |
| 生成 `.razor` が無い | 26 | 対象種別だが、そのファイルの `.razor` が生成されていない |
| AI タスクに到達 | 18 | |

**「.razor が無い」26 件は、ほぼ全部が誤りでした(修正済み)。**

`componentBySource` は**コードビハインドのパス**から引いていました。
`<script runat="server">` を使うファイルは別ファイルのコードビハインドを持たないため
決して一致せず、変換に成功していても「未変換」として捨てられていました。
BlogEngine の `CommentForm.ascx` は
`Components/Controls/Custom/Themes/RazorHost/CommentFormBase.razor` として出力済みです
(コンポーネント名は `inherits` の基底に合わせて変わります)。

`ConvertedComponent.MarkupSourcePath` を追加してマークアップのパスで引くようにし、
**脱落 26 → 1**。残る 1 件は `wt/Default.aspx.cs` で、`.cs` に `.razor` が無いのは正しい挙動です。

### AI 残差層の到達範囲(実測)

**着手前に必ず測り直してください。** この数字は決定的層を直すたびに動きます。

```
--ai-tasks の実測: 12 ファイル / 残差 25 件
  mojo 5 / n2 6 / yaf 1、be・dnn・wt は 0
```

以前ここには「27 ファイル / 43 件」と書いてありましたが、`<script runat="server">` の
解消が反映されて **12 ファイル / 25 件**になっています。

25 件の中身:

| | 件数 | 性質 |
|---|---:|---|
| マークアップを跨ぐ制御フロー | 12 | **AI 向き。**ただし `if` / `else` / 閉じ括弧が別々に数えられるので**実際の構文は 4 箇所** |
| データバインド式がテンプレート外 | 6 | `<%# Eval("Title") %>`。未調査 |
| 式ビルダー `<%$ Code: %>` | 3 | 決定的(上記、2 回撤回) |
| 未マップ属性 `data-close-text` | 1 | 決定的 |
| 動的ページタイトル | 1 | 未分類(YAF) |

**LLM アダプタの投資判断は「4 箇所」に対して行ってください。** 「25 件」でも
「43 件」でもありません。同じ形の誤読は既に一度起きています — n2 の式ビルダー 23 件は
AI タスクの 17 件を占めていましたが、対応表 1 つで消えました。

#### 制御フロー 4 箇所すべてを通した

**AI 層が本来対象としている構文は、これで全部片付きました。**

| | 構文 | 結果 |
|---|---|---|
| mojo `Layout`(2 スキン) | `<% if %> <% } else { %> <% } %>` | `@if` / `else` に畳んで ACCEPT |
| n2 `Mobile/Default` | `<% try { ... if { %>` … `<% } } catch { } %>` | `@try` / `catch` に畳んで ACCEPT |
| n2 `JsTests` | `<% int depth = ...; if(depth > 0) { %>` | `@{ }` + `@:`(`<script>` 内なので)で ACCEPT |

いずれも「ビルドエラー増加なし」で受理。**残るタスクは空回答(SKIP)のままにしてあります** —
`<%$ Code: %>` と「テンプレート外の Eval」は決定的層の課題で、AI に投げる筋ではありません
(プロンプトの制約 6「解消できない残差は憶測で埋めず TODO として残す」がそのまま当てはまります)。

#### ゲートが実際に棄却することを確認した

引き継ぎ資料に「検証済み」とありましたが、自分でも壊してみました。受理済みの回答から
`catch { }` の閉じ括弧を 1 つ取り除いて再適用:

```
REJECT 002-Default — ビルドエラーが増加 39 → 47 件
```

**却下され、対象ファイルは元の内容に復元されていました。** ゲートは「エラー 0」ではなく
「増やさないこと」で判定します(大きなアプリは移植できない依存由来のエラーを元から
抱えており、0 を要求するとどんな回答も通らないため)。

#### 実際に通した(制御フロー 2 箇所)

AI 層を**エンドツーエンドで動かしました。** 対象は mojoPortal の 2 つのスキンにある
同じ構文です。

```aspx
<% if (siteSettings.Logo != "" && siteSettings.Logo != "blank.gif") { %>
    <portal:SiteLogo ... /><portal:SiteTitle SkinID="HiddenTitle" ... />
<% } else { %>
    <portal:SiteTitle SkinID="navbar-brand" ... />
<% } %>
```

生成 `.razor` では 3 つの `@* TODO(W2B) *@` に分解されていたものを `@if` / `else` に
畳み、`--ai-apply` に通して **2 件とも ACCEPT**(ビルドエラー増加なし)。

**この過程で AI 層のバグが 1 つ出ました。** 適用先を**コンポーネント名**で引いていて、
コンポーネント名は一意ではありません。mojoPortal はスキンごとに `Layout.master` を持つので、
`001-Layout` と `002-Layout` が**同じファイルに解決**されていました。2 番目の答え
(別ページのマークアップ全文)が 1 番目のファイルに上書きされるところで、気づけるのは
ビルドゲートだけです。

プロンプト生成時に `ai-layer/index.json`(答え名 → 対象 `.razor`)を書き、適用側は
それを読むようにしました。**対応は導出せず記録します。**

#### 「テンプレート外のデータバインド式」6 件 — 調査したが未解決

このうち 4 件は mojoPortal の `BreadcrumbsControl.ascx` で、**実際にはテンプレートの中**です。

```aspx
<portal:SiteMapPath ...>
  <NodeTemplate>
    <asp:HyperLink NavigateUrl='<%# Page.ResolveUrl(Eval("Url").ToString()) %>' ... />
```

`ControlMappings` の `DataBoundTemplates` / `PlainTemplates` は**組み込みコントロールの
テンプレート名の手書き一覧**で、`SiteMapPath` が自前で宣言する `NodeTemplate` /
`CurrentNodeTemplate` / `RootNodeTemplate` は載っていません。

**「`*Template` で終わる未知の要素はデータバインドテンプレート」という一般化を入れて
みましたが、数字が 1 件も動きませんでした(撤回済み)。** 生成物を見ると、テンプレート
のタグ自体は出ているのに中の `Eval` は依然として除去されており、`_dataBindingTemplateDepth`
が増えていません。`SiteMapPath` が `LegacyRenderHost` に解決され、子が
`EmitNodes` で素通しされる経路のどこかだと思われますが、**特定できていません。**

残る 2 件は `EmptyDataTemplate` 内の `Bind()` で、こちらは WebForms でもデータ項目の
無い場所です(mojoPortal が「空のときに入力欄を出す」ために書いたもの)。

**次に触る人へ:** 手書き一覧を疑うところまでは正しかったはずですが、それだけでは
届きません。`EmitLegacyRenderHost` の子要素がテンプレートとして扱われない理由から
調べてください。

### インラインコード 32 件の中身

「インラインコードブロック」は 1 つの種別ですが、**中身は 3 種類**でした。

| | 件数 | 性質 |
|---|---|---|
| `<script runat="server">` ブロック | **18** | **決定的。**マークアップ内に書かれたコードビハインドで、生成済みの `.razor.cs` partial へ機械的に移せます |
| マークアップを跨ぐ制御フロー | **10** | **AI 向き。**`<% if %>` `<% } else { %>` `<% try { %>` `<% } } catch { } %>` `<% } %>` `<% int depth = ... %>` |
| 式ビルダー・未マップ属性 | **3** | **決定的。**`<%$ Code: %>` 2 件(既知)、`data-close-text` 1 件 |
| 動的ページタイトル | 1 | 未分類(YAF) |

**32 件のうち、本当に AI が要るのは 10 件です。** そして 10 件も、`if` / `else` / 閉じ括弧が
それぞれ別の残差として数えられるため、**実際の構文は 4〜5 箇所**です。

**18 件の `<script runat="server">` は解消しました(下記)。**
マークアップ内の C# を生成済みの partial クラスへ移すようにし、**変換可能残差 82 → 64**。
BlogEngine は**変換可能残差 0** になりました。残る AI 向きは 10 件(実質 4〜5 箇所)です。

## 残差の処分区分 — 「手動移行」を廃止して 3 つに分けた

`ManualMigration`(手動移行)という区分は、**「誰が・どうやって解消するか」の異なる
3 種類を混ぜていました**。ゴールが「元と同じ動作」と明確である以上、実際に人間の判断が
要るものはごく僅かで、大半は「変換器がまだ作っていない」か「別フレームワーク」です。
この区分の誤りは実害も出しています(`portal:mojoButton` 128 件と Assembly Register 95 件は
「判断が要る」という案内自体が誤りでした)。

| 新区分 | 意味 | 誰が解消するか |
|---|---|---|
| **実装待ち(Backlog)** | ゴールは明確で機械化可能。変換器が未実装なだけ | **変換器の開発** |
| **入力待ち(NeedsInput)** | ツールが導出できない情報が要る(配置構成の選択、ソースの無い DLL の置き換え先、入力ツリーに無いファイル) | **ユーザーが情報を渡す**と変換が続行 |
| **範囲外・いまは(OutOfScope)** | 別フレームワーク(MVC / Web API / OWIN・Identity / Web Pages)。変換で失われたものはない | 「同じ動作」に含めるなら**そのフレームワーク用の変換**が別途必要 |

未対応コントロールも分岐しました: `asp:*`(描画仕様は WebForms が定義済み)は
**実装待ち**、ソースの無いサードパーティは**入力待ち**です。

### 実測(総残差 679 の内訳)

```
コーパス  実装待ち  入力待ち  範囲外
be            17         2       46
mojo           7        26       44
yaf           18        14       32
dnn            8        40      114
n2            37        37       90
wt             9         2       24
合計          96       121      350    (+ 変換可能 62 + 情報通知 40 = 669)
```

**「ゴールまでに変換器が作るべきもの」は 実装待ち 96 + 変換可能 62 = 158 件**です。
入力待ち 121 は仕組みが既にあり(`--project` / `--include` / `--control-map` /
`--package-map`)、範囲外 350 は WebForms 変換器の課題ではありません。

### コードビハインド残差の性質(当時の実測 512 件。その後の修正で減少)

| | 件数 | |
|---|---:|---|
| 対象外フレームワーク | 253 | MVC 138 / Web API 79 / Identity 18 ほか。**変換で失われたものは無い** |
| .NET Framework 専用の名前空間 | 124 | `System.Web.Compilation` 29(→後に移植)/ `System.Web.Routing` 22 / `System.Web.Services` 15 |
| 除外の連鎖 | 113 | 除外された型に依存して芋づるに除外されたもの(→後に 30 まで減少) |
| その他 | 22 | `BinaryFormatter` 8 ほか |

### 実装待ちの最大は標準コントロール 44 件 — 描画しないものから着手

区分を分け直して最初に見えたのが、**実装待ち 106 件のうち 44 件が標準 `<asp:*>`** という
事実です。`Login` / `Wizard` / `Calendar` の描画仕様は WebForms が定義しており、
ユーザーが決めることは何もありません。

ただし**着手順は「件数」ではなく「描画を壊さないか」で決めました。**

| | 件数 | 判断 |
|---|---:|---|
| `SiteMapDataSource` | 8 | **着手。** WebForms でも**何も描画しない**ので、変換しても DOM は変わらない |
| `TableHeaderRow` | 2 | **着手。** `<tr>` を出すだけ。`TableRow` と同型で `TableHeaderCell` が `<th>` を出す |
| `WizardStep` / `Wizard` 系 | 17 | **見送り。** テーブルレイアウトとナビゲーションボタンを持ち、描画が複雑。「見た目は正しいが死んでいる」risk が高い |
| `Login` / `ChangePassword` 等 | 8 | **見送り。** 描画に加え、動作が認証基盤に依存する |
| `MultiView` / `View` | 3 | **見送り。** 子の登録順で active を決める仕組みが要り、3 件に対して波及が大きい |

`SiteMapDataSource` のノードは**アプリ自身の `SiteMapProvider`** から取ります。
プロバイダが未登録なら**空を返します** — ロールプロバイダと同じ fail closed で、
存在しないナビゲーションを作るよりレンダリングされないメニューの方がましだからです。

**mojo 114 → 105、n2 202 → 201。ビルドエラーは 6 本すべて不変、BlogEngine も 5/5 のまま。**

### `<%@ Register ... Assembly %>` は未対応ではなかった(−95)

未対応コントロール 254 件のうち **95 件**が、これ 1 つのメッセージでした。

```
<%@ Register %> のうち Src を持たないもの(Assembly 登録)は未対応です:
TagPrefix=dnn, Assembly=DotNetNuke.WebControls
```

**これは事実に反します。** `BuildPrefixNamespaces` は同じディレクティブの `Namespace`
属性を読んでプレフィックスの解決表を作っており、`ResolveLegacyControl` がそれを使って
移植済みコントロールを `LegacyRenderHost` に解決しています。

しかも**同じ問題を 2 回報告していました。** DNN の `admin/Containers/title.ascx` は:

```
- title.ascx    — <%@ Register %> のうち Src を持たないもの(Assembly 登録)は未対応です
- title.ascx:3  — <dnn:DNNLabelEdit> は未対応コントロールです
- title.ascx:6  — <DNN:DNNToolBar> は未対応コントロールです
- title.ascx:8-10 — <DNN:DNNToolBarButton> ×3
```

本当に解決できないコントロールは**使用箇所で個別に報告済み**です。Register 側の 1 件は
同じ穴を数え直しているだけでした。情報通知に降格し(どのアセンブリ由来かは
`--package-map` を書くときに要るので残しています)、**dnn 364 → 283、n2 221 → 209、
mojo 126 → 124。合計 900 → 805。**

「未対応コントロール」の件数が誤った案内で膨らむのは**これで 2 回目**です
(1 回目は `portal:mojoButton` の 128 件)。この種別の数字は、**まず内訳を見てください。**

### 除外の連鎖 — import は依存ではない(dnn +11 ファイル)

コードビハインド残差 512 件のうち **113 件が「連鎖して除外」**でした。起点を数えると
少数に集中しています。

```
22  DotNetNuke.Common.Globals      9  DotNetNuke.UI.Skins.Skin
14  DotNetNuke.Security.FilterFlag 9  DotNetNuke.Framework.Reflection
12  DotNetNuke.UI.Utilities        7  DotNetNuke.Services.Upgrade
```

最大の `Globals` を根まで辿ると、**2 ファイルに行き着きます**。

```
System.Web.Compilation を使う DnnInstallLogger.cs   ─┐
System.Data.Linq を使う Upgrade.cs                  ─┴→ DotNetNuke.Services.Upgrade が空に
  → HtmlUtils.cs が「その名前空間を import している」ので連鎖除外
    → Globals.cs が HtmlUtils に依存するので連鎖除外
      → さらに 22 ファイル
```

**`HtmlUtils.cs` は `using DotNetNuke.Services.Upgrade;` と書いているだけで、その
名前空間の型を 1 つも使っていません。** import は依存ではありません。

空になった名前空間を import しているとき、**その名前空間の型を実際に名指ししているか**を
見るようにしました。名指ししていなければ移植し、`using` の方を落とします
(`.razor` の `@using` に対する `StripDeadUsings` と同じことを `.cs` にも)。

**判定は単純名で行い、その向きは意図的です。** 非修飾の使用を追って**除外を広げる**のは
過去 3 回とも制御不能になっています(`UsesGoneType` のコメント参照)。ここでは同じ材料を
**「1 つも出てこないときだけ救う」**という向きにしか使わないので、曖昧なものは
除外されたままになります。

**dnn の移植 .cs 1944 → 1955、総残差 283 → 272、ビルドエラーは 121 のまま。**
救ったファイルが新しいエラーを持ち込んでいないことの確認になります。他 5 本は不変。

### 文字列リテラルの中の名前空間で 1 ファイルが落ち、そこから 90 件

`Globals` の 22 件を根まで辿ると `Upgrade.cs` に行き着きます。除外理由は
「`System.Data.Linq` を使用しているため」。ところがそのファイルに `using System.Data.Linq;`
はありません。**唯一の出現箇所はこれです。**

```csharp
if (Reflection.CreateType("System.Data.Linq.DataContext", true) != null)
```

**文字列リテラルです。** 省略可能なアセンブリがあるかどうかを実行時に調べているだけで、
このファイルは LINQ to SQL に依存していません。

`FindQualifiedFrameworkReference` は「prose ではなく code に見えること」を要求する
作りになっていましたが(WingtipToys の `// ... to PayPal.` で 1 度痛い目を見ている)、
判定は依然として**生テキスト検索**で、文字列とコメントの中を見ていました。

パースして**文字列リテラルとコメントを空白で潰してから**探すようにしました。オフセットが
ずれると前後の文字を見る既存の判定が壊れるので、削除ではなく空白置換です。

**dnn: 移植 .cs 1955 → 2029(+74)、総残差 272 → 189、ビルドエラー 121 → 98。**
3 つとも改善します。yaf +2 / n2 +1 ファイル。**合計 総残差 794 → 704、ビルドエラー 258 → 252。**

**mojo だけビルドエラーが 46 → 63 に増えます。** 救われたのが AppFabric
(`Microsoft.ApplicationServer.Caching`、製品自体が廃止)を使う 2 ファイルで、その依存が
未決だからです。未決の依存は本来件数から外れますが、**この DLL は入力ツリーに無く型名を
読めない**ため分類できません(`unresolved-dependency-types.txt` の限界)。移植されたこと
自体は正しく、依存が未決であることが見えている状態なので、受け入れています。

### `System.Web.Compilation` は除外をやめて互換層に置いた

連鎖を潰したあと、`.NET Framework 専用の名前空間` 108 件の最大は
`System.Web.Compilation` 28 件でした。この名前空間を import する 32 ファイルを数えると:

```
13  ExpressionBuilder を継承(独自の <%$ Prefix:Value %> 構文)
 3  BuildManager を呼ぶ
```

`Prefixes` のコメントは既に方針を書いています — **「ライブラリ級の Framework 名前空間は
移植してローカルエラーにする。除外の連鎖の方が害が大きい」**。`System.Web.Compilation`
はその条件に当てはまるのに列挙に入っていました。DNN の `Framework/Reflection.cs` は
`BuildManager.GetType` を 2 回呼ぶだけで除外され、9 ファイルを道連れにしています。

互換層に `BuildManager` / `ExpressionBuilder` / `ExpressionPrefixAttribute` /
`BoundPropertyEntry` / `ExpressionBuilderContext` と、ビルダーが返す CodeDom の形
(`CodeExpression` ほか)を置き、`Prefixes` から外しました。

- **`BuildManager.GetType` は実装しています。** 名前による型解決は .NET でもできます
- **式ビルダーは宣言のみで、動きません。** 呼ぶのは WebForms のページコンパイラで、
  それはここに存在しません。`<%$ %>` は**変換時に** `--expression-map` で解決済みです
- CodeDom は NuGet パッケージを参照せずシムにしました。グラフをコンパイルする側が
  いないので、型が存在しさえすれば足ります

**この変更は一度 be を 0 → 11 に壊しました。** 露出した依存を 3 つ直して 0 に戻しています。

| 出た問題 | 実体 |
|---|---|
| `HtmlHelper` 7 件 | `RazorHelpers.cs` が `System.Web.WebPages.Html` を使用。**ASP.NET Web Pages** で MVC 同様に対象外 → `Prefixes` に追加 |
| `CS0508` 2 件 | 派生が `using System.CodeDom;` を保持し、互換層の `CodeExpression` と別型になっていた → import を落として互換層に束ねる |
| `ExpressionPrefixAttribute` / `HttpCompileException` | シムに不足 → 追加 |

**be の 0 は守る前提で進めました。** 唯一ビルドが通り 5 ルート中 4 が動くコーパスで、
ここを崩す変更は入れる価値がありません。

**全 6 本で残差が減りました。** be 77→75、n2 208→202(ビルドエラーも 48→46)、
mojo 120→114、dnn 189→178。**合計 総残差 704 → 679。**

### 残差を原因で数える

分類前は 1,095 件で、内訳はこうでした。

```
973  手動移行     コードビハインド 512 / 未対応コントロール 426 / 構成 35
 82  変換可能     未対応の属性 36 / インラインコード 32 / データバインド式 9 / その他 5
 40  情報通知
```

**「未対応コントロール 426 件」は 1 つの問題ではありませんでした。** 種別名は
「マッピング表への追加が必要」ですが、実際には 3 種類が混ざっていました。

| | 件数 | 実態 |
|---|---|---|
| `portal:mojoButton` | **128** | **マッピング不要だった。**変換元は移植済み、`LegacyWebControl` を継承、`tagPrefix="portal"` も登録済み。**解決器の欠陥**(下記、修正済み) |
| `Register` の `Src` 先が無い | 97 | `~/DesktopModules/DDRMenu/Actions.ascx` などが入力ツリーに存在しない。**ファイル解決の問題**でマッピングとは無関係 |
| 実際に未対応の他社コントロール | ~200 | `dnn:DnnComboBox` 16、`n2:ItemDataSource` 15、`YAF:*` など。ここだけが本当にマッピング表の話 |

**426 件のうち 225 件は「マッピング表に足せ」という案内が誤り**でした。

### LegacyRenderHost の対象基底クラス

`ResolveLegacyControl` は、移植されたコントロールの**元の基底チェーンの根**が
`LegacyRenderableRoots` にあるときだけ `LegacyRenderHost` に解決します。この集合は
必要になるたびに 1 つずつ足されてきたもので、**`Button` が入っていませんでした。**

そのため `mojoButton : Button` は「未対応コントロール」として 128 回報告されていました。
全コーパスの未対応コントロールの 30% が、これ 1 つです。

`Button` / `TextBox` / `CheckBox` / `DropDownList` / `CompositeControl` などを追加し、
**総残差 1,095 → 921(−174)**。ビルドエラーは 1,100 のまま動いていません。

これは「報告する」から「**アプリ自身の描画コードで描画する**」への挙動変更です。
安全なのは、`LegacyRenderHost` が呼ぶのが移植された元クラスの `RenderContents` 等
そのものだからで、互換層が挙動を推測するわけではありません。パリティ 30/30 で確認済みです。

### コードビハインド 512 件 — 半分は WebForms ではない

同じ手法で割ると、321 件が「.NET Framework 専用の名前空間」の 1 形でした。その中身:

```
140  System.Web.Mvc
 79  System.Web.Http           ← Web API
 22  System.Web.Http.*(3 種)
 31  Microsoft.AspNet.Identity
 22  System.Web.Routing
  4  System.Web.Optimization
```

**MVC + Web API + Identity + バンドルで 232 件。** DNN は WebForms と MVC と Web API が
1 つのアプリに同居しており、**この変換器の対象は WebForms だけ**です。

これらに「手動移行が必要」と書くのは誤解を招きます。**変換で失われたものは何もなく、
最初から対象外**だからです。読み手には次の 2 つを区別できる必要があります。

- 変換器が落としたので作り直す必要があるもの
- そもそも別フレームワークで、この道具の範囲外のもの

メッセージを分けました。

```
ASP.NET MVC(System.Web.Mvc)のコードです。この変換器は WebForms のみを対象とするため
移植していません。変換で失われたものはなく、対応する ASP.NET Core の仕組みへ別途移行してください。
```

### ビルドエラー側には、この切り分けはほぼ不要でした

残差で 232 件が対象外だったので、ビルドエラー 1,115 件にも同じ比率で混ざっていると
考えて測りましたが、**対象外フレームワークに言及するエラーは 15 件だけ**でした。

理由は考えれば当然で、**MVC を import しているファイルは移植前に除外される**ため、
そもそもビルドに参加しません。残差になった 232 件がまさにそれです。ビルドエラーに
出てくるのは「移植されたファイル」だけなので、対象外コードは原理的にほとんど現れません。

`AuthorizeAttribute.AuthorizeCore` のような MVC 由来のものは確かに存在しますが、
それは `System.Web.Mvc` を直接 import していなかったために除外を免れた少数です。

**サンプル 2 行から「多く混ざっている」と書いたのは誤りでした。** 実測は 15/1115 です。

### ビルドエラー 1,115 件の実際の構成

```
401  CS0246  型が見つからない
295  CS0115  オーバーライド対象が無い
230  CS0759  partial メソッド未定義   ← --analyzer で 0 になる
 60  CS0234
 36  CS0104  → 内訳は下記(28 件が互換層、44 件は BCL との衝突)
 15  CS0535
```

CS0115 のオーバーライド先を数えると、`RenderEditMode` / `RenderViewMode` /
`StringValue` / `AllowableFiles` / `ItemNodeName`(DNN の `EditControl` 系)と、
`OnInit` / `CreateChildControls` / `LoadViewState` / `SaveViewState`(WebForms
ライフサイクル)に集中していました。**いずれも除外型スタブが基底**でした。

### スタブは protected メンバを落としていた

原因は 1 行です。スタブ生成器はメンバを `public` と `internal` だけ通していました。

```csharp
protected abstract string StringValue { get; set; }      // 元
protected virtual void RenderViewMode(HtmlTextWriter w)  // 元
```

**サブクラスが override するメンバは、たいてい protected です。** 落とせば、その型を
継承している移植済みクラスが全部 CS0115 になります。DNN の `EditControl` がこれで、
`RenderEditMode` / `RenderViewMode` / `StringValue` / `AllowableFiles` を失っていました。

protected を通し、**アクセス修飾子は元のまま**にしました(public に上げると型の公開面が
変わるため。ここに置く理由は override させることだけです)。**DNN 710 → 661。**

スタブ生成器はこれで 3 回目です — 基底クラスの消失(CS0115)、修飾子の消失(CS0506)、
メンバの消失(CS0115)。いずれも**エラーはサブクラス側にしか出ません**。
除外型スタブが絡む override エラーを見たら、まず生成器を疑ってください。

「変換可能 36 件の未対応属性」も同様に固まっています。

```
18  DataSourceID 13 + DataSource 4 + DataMember 1   宣言的データバインド(1 機能)
 7  resourcekey                                      暗黙ローカライズ
11  その他(単発)
```

**追うべきは「変換可能」と「ビルドエラー」です。** 総残差の大半は `ManualMigration`
(設計判断・外部依存)で、コーパスが持ち込む依存の量を測っているにすぎません。
分類の定義は `ResidualDisposition`(`src/WebForm2Blazor.Converter/ConversionReport.cs`)と
[../HANDOVER.md](../HANDOVER.md) の 2.3 を参照してください。

**残差とビルドの通りやすさは別の指標です。** 除外型スタブの変更で BlogEngine が
0 → 17 エラーに退行したとき、残差の数値は 3 つとも一切動きませんでした。
そのため `convert-all.ps1` は変換のたびに `--verify-build` を回し、ビルドエラー数も
ベースラインとして固定します(`-SkipVerifyBuild` で省略可。所要時間は数分増えます)。

「変換可能」やビルドエラーが減るのは改善なので成功扱いにしますが、
**増加した場合や、総残差・移植 .cs が動いた場合は理由を確認するまで失敗扱い**にします。

なお構文エラーがあるとコンパイラは意味解析を行わないため、**ビルドエラー数は総数ではなく
下限**になります(YAF で構文エラー 1 個が 1,628 件を隠していた実例があります)。
その状態を検出したら `convert-all.ps1` は警告して失敗扱いにします。

**コンパイラに届く前にビルドが落ちた場合も同じことが起きます。** 実例として、`App_Data` を
`<Content Include>` で追加したことが SDK 既定のグロブ(`**/*.config`)と衝突し、n2cms が
`NETSDK1022` 1 件でビルド開始前に停止していました。件数は 109 → 1 に「改善」したように
見え、**その 1 という数字をベースラインとして固定してしまっていました**(実際にはコンパイル
自体が一度も走っていません)。この状態では AI 残差層のゲートも機能せず、意図的に構文を
壊した回答を「エラー増加なし」として受理します。

検出は「構文エラーコードの一覧」ではなく **CS 診断が 1 件も出ていないのにエラーがある**
という条件で行います。`NETSDK*` / `MSB*` / `RZ*` を列挙せずに閉じられるためです。

## global using を使うアプリ

C# 10 以降のアプリは `GlobalUsings.cs` に `global using` を並べ、個々のファイルには
using を 1 行も書きません。変換器はそこから `System.Web.*` の import を落として互換層の
import を足しますが、**足す側が平の `using` だった**ため、その 1 ファイルにしか効かず、
プロジェクト中のファイルが型を見失っていました。

YAF.NET がこれで、`HtmlTextWriter` 54 件・`HttpContext` 22 件が「互換層に存在するのに
CS0246」という状態でした。**注入する using を元ファイルの global に合わせた結果、
179 → 114 件**になっています。

副作用として mojoPortal が 1 件増えました(`Image` が `System.Drawing.Image` と衝突)。
互換層が project 全体から見えるようになったことで、下記の平坦化問題が 1 箇所表面化した
ものです。差引 64 件の改善なので受け入れています。

### 続き: エイリアスで供給していた型は、そもそも互換層に無かった

`HttpRequestBase` / `HttpResponseBase` / `HttpSessionStateBase` /
`HttpServerUtility(Base)` は、互換層の実型ではなく **「ファイルが `using System.Web;` を
落としたときに、その場で出す using エイリアス」** として供給していました。

```csharp
aliasUsings.Add("using HttpRequestBase = WebForm2Blazor.Components.HttpRequestShim;");
```

エイリアスは**それを書いたファイルにしか効きません。** そして上の 1 節のとおり、
エイリアスだけは `global using` にできません(同じ名前を各ファイルが宣言するので
CS1537 が全ファイル分出る)。結果、**global using のアプリには落とす import が無く、
名前はどこにも供給されませんでした。** YAF.NET の 82 件はこの内訳です。

```
9  HttpRequestBase        3  HttpApplicationStateBase   ← エイリアス表にすら無かった
8  HttpResponseBase       3  HttpRequest
6  HttpSessionStateBase   2  HttpServerUtilityBase
```

**82 件中 31 件が、1 つの供給方式の欠陥です。**

修正は互換層に**実型として置く**ことです。`HttpRequestShim` は `sealed` なので、
`HttpRequestBase` を実基底にするには**メンバを基底へ移す**必要がありました。
`HttpRequestBase` が実装を持ち、`HttpRequestShim` がそれを継承します(sealed のまま)。
メンバは `virtual` にしてあります — 移植コードが独自の派生(テストダブル)を書いたときに
override できる必要があり、System.Web の `*Base` も全メンバが virtual だからです。

`HttpSessionState` / `HttpApplicationState` / `HttpServerUtility` は基底と具象の間に
挟みました。System.Web ではこれらは `*Base` と無関係な sealed クラスですが、ここでは
**どちらの名前で受けても代入できる**ことだけが要件なので、1 本の鎖にしてあります。

**`HttpRequest` / `HttpResponse` はエイリアスのまま残しています。** この 2 つは
`Microsoft.AspNetCore.Http` に実在する名前で、互換層側は `Shim` 接尾辞を外せません。
YAF に残る 3 件はこれです。

**yaf 82 → 54、n2 83 → 82。合計ビルドエラー 796 → 767。** 総残差・変換可能・移植 .cs は
6 コーパスすべてで不変、パリティ 30/30。

### ライフサイクルの override 先は、コンポーネント系の基底にだけ無かった

n2 の 82 件を分けると CS0115 が 25 件。メンバ名で見ると `OnInit` 3 / `CreateChildControls` 3 /
`OnPreRender` 2 / `OnDataBinding` 2 のように**同じ名前が並びます**。1 件だけ追うと、

```csharp
public class UrlSelector : HtmlGenericControl        // 互換層の Blazor コンポーネント
{
    protected override void OnInit(EventArgs e)      // ← 基底に OnInit が無い
    {
        base.OnInit(e);
        EnsureChildControls();                       // ← EnsureChildControls も無い
```

`LegacyWebControl`(平のクラスとしてのコントロール基底)と `WebFormsPage` は、この
ライフサイクル一式を**両方とも既に持っています**。持っていなかったのは
`WebFormsControlBase` — 互換層の**コンポーネント系**コントロール全部の基底だけでした。
`Label` / `Button` / `HtmlGenericControl` / `TableRow` / `RequiredFieldValidator` を継承した
アプリ独自コントロールが、そこで全部 CS0115 になります。

`OnInit` / `OnLoad` / `OnPreRender` / `OnUnload` / `OnDataBinding` /
`CreateChildControls` / `EnsureChildControls` / view-state 3 種 / render 4 種を追加しました。

**駆動するのは `OnInit` だけです**(`OnInitialized` から)。`WebFormsPage` と同じ選択で、
理由も同じ — Init の本体は初回描画が読む状態を作ります。残りは宣言のみで、これも
`LegacyWebControl` と同じです。Blazor のコンポーネントにはポストバックもビューステートも
無いため、駆動すると挙動を推測することになります。

**この変更が影響するのは、いま CS0115 でビルドできないファイルだけ**です。互換層の
コントロール自身は override していないので no-op のままです。

**n2 82 → 74、mojo 208 → 206、yaf 54 → 53。dnn は変化なし** — dnn の CS0115 179 件は
`AddPermission` / `RenderViewMode` のようなアプリ固有メンバで、基底は除外型スタブです
(別の穴)。合計 767 → 756、パリティ 30/30。

### ページ側にも同じ穴があった(`ID` / `OnDataBinding` / `EnableTheming` / `InitializeCulture`)

上を直した時点で n2 の CS0115 は 25 → 17。残りを見ると、直らなかった 2 件は
`RecentVersions.OnDataBinding` / `ReferencingItems.OnDataBinding` で、**基底は
`WebFormsUserControl`** でした。コントロール側だけ直しても、ページ / ユーザーコントロール /
レイアウトの 3 基底には同じ穴が残っていたということです。

| メンバ | 落ちていたもの |
|---|---|
| `ID` | `TemplatePage<TPage>` / `TemplateMasterPage<TPage>` / `Framed`。System.Web では virtual で、n2 は「未設定なら "P"」を返すために override する |
| `OnDataBinding` | `RecentVersions` / `ReferencingItems` |
| `EnableTheming` | `EditPage` |
| `InitializeCulture` | YAF の `BasePage` / `ForumPageBase` |

`InitializeCulture` だけは**駆動しています。** WebForms はコントロールツリーを作る前に
呼ぶ約束で、ページが `CurrentUICulture` をリクエストから設定するためのものです。後続の
すべてに効く必要があるので、`OnInit` の前に置きました。

**n2 74 → 68、yaf 53 → 51。合計 748。** パリティ 30/30。

### 完全修飾された `System.Web.X` の置換表も、手書きの列挙だった

`using` の書き換えは、**シグネチャに完全修飾で書かれた型**には届きません
(`protected override TextWriter GetTextWriter(System.Web.HttpResponse response)`)。
そのためテキストパスが別にあるのですが、その中身が**手書きの列挙**でした。

```csharp
.Replace("System.Web.HttpContext", "WebForm2Blazor.Components.HttpContext")
.Replace("System.Web.HttpRuntime", "WebForm2Blazor.Components.HttpRuntime")
.Replace("System.Web.VirtualPathUtility", ...)
.Replace("System.Web.HttpUtility", ...)
```

列挙されていなかったのは `HttpApplication` / `SiteMapNode` / `SiteMapProvider` /
`HttpRequestBase` — **どれも互換層に同名で実在します。**

**変換器は互換層のアセンブリを参照しています。**(`--components-ref` とは別に、
`typeof(WebFormsControlBase).Assembly` として。)ならば列挙を持つ理由はありません。
`System.Web.X` を正規表現で拾い、**互換層が本当に X を宣言しているときだけ**置換する
ようにしました。型名の変わるもの(`HttpBrowserCapabilities` → `...Shim`)と
名前空間ごと移すもの(`System.Web.UI.WebControls.`)は先に走るので影響を受けません。

**n2 68 → 65。** 他 5 本は不変です。件数は小さいですが、消えたのは「列挙が漏れる」と
いう欠陥のほうで、互換層に型を足すたびにここを更新する必要も無くなりました。

### スタブ生成器 4 回目 — 名前解決が「同じ名前空間」と手書き BCL リストだけだった

dnn の CS0115 は 179 件。**クラス別**に見ると散らばって見えません。

```
16  SqlDataProvider          8  ResourceFileInstaller
16  ModulePermissionsGrid    8  DesktopModulePermissionsGrid
13  TabPermissionsGrid       6  DNNScheduler
12  FolderPermissionsGrid    5  CleanupInstaller
```

`*PermissionsGrid` 4 つの基底は `PermissionsGrid`(除外型スタブ)で、落ちているのは
`AddPermission(ArrayList, RoleInfo)` / `GetPermissions()` / `SupportsDenyPermissions(PermissionInfo)`
など。スタブ側にそれらのメンバがありません。

原因は `Resolves` — メンバを出すかどうかを決める判定です。型が解決できると認めるのは
**2 つだけ**でした。

```csharp
IsKnownSimpleType(name)                                  // 手書きの BCL 名 24 個
|| known.Contains(QualifiedName(declaredNamespace, name)) // 同じ名前空間の型のみ
```

`ArrayList` はリストに無く、`RoleInfo` / `UserInfo` / `PermissionInfo` は
**1 つ隣の名前空間**にあります。どちらも解決できないので、それらを含むメンバは全部落ち、
継承側が全部 CS0115 になります。**エラーはやはりサブクラス側にしか出ません。**

スタブファイルは `using System;` と `using System.Collections.Generic;` しか持てません
(元ファイルの using を写すと、除外した名前空間ごと引き込んでしまう)。だから
`IsKnownSimpleType` はその 2 つで書ける名前の一覧として正しくはあり、
**足りなかったのは「名前解決を自分でやって完全修飾で書き出す」ことでした。**

- 探索する名前空間は、その型の名前空間とその親、それから**元ファイルの import** の順
- プロジェクト型は `known`(移植済み + スタブ済みの完全修飾名)で判定
- BCL 型は、**変換器が動いているフレームワークのアセンブリのメタデータを読んで**判定
  (`FrameworkTypeIndex`)。ロードはしません — 「その名前の型があるか」しか要らないので
- 解決したら `global::` 付きの完全修飾名で出力

**手書きリストは 3 つめでした**(エイリアス供給表、`System.Web.X` 置換表、これ)。
いずれも同じ壊れ方をします。

**dnn 422 → 325(−97)。** 他 5 本は不変です。パリティ 30/30。

### 同じ名前解決が、今度は互換層を見ていなかった

上を直して dnn の CS0115 は 179 → 87。**メンバ名別**に数え直すと、また同じ名前が並びます。

```
10  RenderViewMode    5  SaveViewState    3  RenderJsDependencies
10  RenderEditMode    5  LoadViewState    3  RenderCssDependencies
 8  AllowableFiles    4  AddEditorRow     2  Render
```

`EditControl.RenderViewMode(HtmlTextWriter writer)` は元ソースに存在するのに、
スタブに出ていません。`HtmlTextWriter` が解決できないからです。

- `known`(移植済み + スタブ済み)に無い — 移植対象外
- `System.Web.UI.HtmlTextWriter` は .NET に無いので `FrameworkTypeIndex` にも無い
- **しかし `WebForm2Blazor.Components.HtmlTextWriter` は互換層にあります**

名前解決がプロジェクト型と BCL しか見ておらず、**移植後にその名前が何になるか**を
見ていませんでした。`System.Web*` から import された名前は互換層に解決するので、
そこも引くようにしました(`AddEditorRow(Table, object)` の `Table` / `Panel` も同じです)。

**dnn 325 → 299。** 他 5 本は不変、パリティ 30/30。

### スタブは署名を作り替えていた(`ref` を落とす / 常に `{ get; set; }`)

さらに CS0115 を数え直すと `AllowableFiles` 8 件。`AssemblyInstaller.AllowableFiles` の
基底は `ComponentInstallerBase` で、**これは移植済み**です。落ちていたのは
`FileInstaller`(スタブ)の基底で、`RecordInheritableClass` が **abstract クラスを
継承候補から外している**ためでした。

**abstract を許可する変更は測って撤回しました。**

| | dnn | yaf |
|---|---:|---:|
| abstract 基底を許可 | −14 | **+138** |

YAF のスタブは abstract 基底が宣言するメンバを実装できず、CS0115 が CS0534 に
置き換わるだけでした。除外の理由づけは正しかったことになります。

**後日解決しました → 「abstract 基底: スタブ自身を abstract にする」を参照。**
正解は「スタブがメンバを再現できるようになるまで待つ」ではなく、
**スタブを具象にしないこと**でした。

ただしこの実験で、**スタブが署名を作り替えている**ことが 2 つ見つかりました。
どちらも abstract とは無関係なので残してあります。

| | 直したこと |
|---|---|
| `ref` / `out` / `in` / `params` | 引数に修飾子があるとメンバごと落としていました。修飾子は署名の一部です(DNN の `MembershipProvider` は `ref UserInfo` を取る abstract メンバを 8 つ宣言します) |
| プロパティのアクセサ | 元が読み取り専用でも `{ get; set; }` で出していました。override すると CS0546(「オーバーライド可能な set が無い」) |
| `internal` な override | `public` に広げていました。override はアクセシビリティが基底と一致する必要があり、CS0507 になります |

**後ろ 2 つは基底を落としている間は表に出ません。** override を出さなければ
アクセサもアクセシビリティも問われないからです。

**dnn 299 → 293。** 他 5 本は不変、パリティ 30/30。

### 未決の依存は、ビルドエラーの件数から外してあります

置き換えパッケージが決まっていない同梱 DLL の型は、当然 CS0246 になります。これを
他と一緒に数えるのは**二重に誤り**でした。変換器には直しようがなく(パッケージを選ぶのは
ユーザーの判断)、しかも件数が多いので**変換器の課題であるエラーを埋めてしまいます。**

```
dnn  Lucene.Net 83 / DotNetNuke.WebControls 57 / FastVectorHighlighter 4 / effority.ealo 2 /
     Microsoft.ApplicationBlocks.Data 1                                             = 147
mojo Lucene.Net 64 / ZedGraph 12 / Novell.Directory.Ldap 12 /
     Microsoft.ApplicationServer.Caching.Client 5 / ZedGraph.Web 4 / Subkismet 3 /
     CSSFriendly 2 / Argotic.Common 1                                               = 103
```

**判定は名前の接頭辞ではなく、DLL 自身のメタデータで行います。** 変換時、まだ DLL が
手元にあるうちに未決アセンブリの公開型名を読み、`unresolved-dependency-types.txt` に
書き出します。ビルド検証はそれを読み、診断メッセージ中の識別子と突き合わせます。

- 対象コードは「その名前が存在しない」系のみ(`CS0246` / `CS0234` / `CS0012` /
  `CS1069` / `CS7069`)。同名の型が他所で落ちているなら、それは本物のエラーです
- メッセージは**解析しません**。コンパイラの文言は SDK がローカライズするので、
  引用符に囲まれた識別子だけを見ます
- **消すのではなく別掲**します。`--package-map` を書けば消える性質のものなので、
  何件・どのアセンブリかは見えている必要があります

**dnn 293 → 146、mojo 206 → 103。合計 616 → 366。** 変換器の挙動は 1 行も変わって
おらず、**数えるものの定義が変わっただけ**です。他 4 本は未決の依存がゼロなので不変。

なお `--verify-build` の終了コードも変換側のエラーだけで決まります。未決の依存しか
残っていない出力は「変換としては通っている」と扱われます。

### コントロールアダプタ(mojo 15 件)

`System.Web.UI.Adapters.ControlAdapter` は、`.browser` ファイルに登録すると**コントロール
の描画を丸ごと差し替える** WebForms の仕組みです。mojoPortal はこれを 15 個持っています
(メニュー、ツリービュー、グリッドビュー、ログイン系)。

互換層には `WebControlAdapter` だけがあり、基底の `ControlAdapter` も
`MenuAdapter` / `HierarchicalDataBoundControlAdapter` もありませんでした。さらに
変換器は `System.Web.UI.Adapters.` をフラットに潰しておらず、
`WebForm2Blazor.Components.Adapters.X` という**存在しない名前空間**を書いていました。

**宣言するだけで、駆動はしません。** Blazor のコンポーネントは自分のマークアップから
描画し、`.browser` を読む機構も、描画をアダプタに渡す機構もありません。`Control` が
null なのはそのためで、万一アダプタに到達した呼び出しは正しくない描画をせずに落ちます。
`LegacyWebControl` のライフサイクルフックと同じ判断です。

**mojo 103 → 88。**

### 名前空間マップの置換が、自分の出力に再マッチしていた

dnn に `'dnn' が名前空間 'dnn.Components.Controls' に存在しません` が 14 件。生成物を
見ると原因は明白でした。

```razor
<dnn.Components.Controls.dnn.Components.Controls.DesktopModules.Admin.Security.DNNProfile ... />
```

`ApplyNamespaceMap` は、元の名前空間を変換後のものへ**単純な文字列置換**で移します。

```csharp
code = code.Replace(originalNamespace + ".", distinct[0] + ".");
```

ところが**移行先は移行元を含みます** — `DesktopModules.Admin.Security` →
`dnn.Components.Controls.DesktopModules.Admin.Security`。置換結果の中に元の文字列が
そのまま残るため、同じ置換がもう一度当たって二重に前置されます。

名前の境界(`(?<![\w.])`)で固定しました。**dnn 146 → 132、n2 65 → 63。**

### 互換層に無かった WebForms 型 63 個

未決の依存を外したあとの CS0246 は 106 件。**上位が 3 件しかない完全なロングテール**で、
1 つの欠陥ではありません。ただし型名を並べると性質は揃っていました。

```
PagerPosition ButtonType TextAlign GridLines FirstDayOfWeek DayNameFormat
GridViewRowCollection DataGridItemCollection DataBindingCollection Parameter
WebPart CatalogPart Personalizable WebBrowsable UrlProperty
IButtonControl IEditableTextControl INavigateUIData
SqlMembershipProvider ProfileProvider ValidatePasswordEventArgs ...
```

**ほとんどが「互換層がまだ宣言していなかった System.Web の型」**です。63 個を
`Compat/WebFormsTypeShims.cs` にまとめて追加しました。enum は**元の値**を持ちます
(コードが値で分岐し、永続化するものもあるため)。

対象外として**足さなかった**もの:

| | 例 |
|---|---|
| MVC / Web API / WCF | `WebPageBase`、`MediaTypeFormatter`、`ServiceHost` |
| アプリ自身の型 | `CmsPage`、`StyleSheetCombiner`、`MetaContent` |

**型だけ足すと悪化することがあります。** `ProfileProvider` を空で足したところ、n2 の
`ContentProfileProvider` が override 先を失って **CS0246 1 件が CS0115 8 件に化けました**
(63 → 64)。プロバイダは**アプリが継承するもの**なので、抽象メンバを 1 つ残らず
宣言して初めて意味があります。`SqlMembershipProvider` も同じ理由で全メンバを実装して
います — そちらは具象クラスなので、各メソッドは既定値を返さず**投げます**。認証
プロバイダが黙って「そんなユーザーはいない」と答えるのは、この層が発明してよい
セキュリティ上の答えではありません。

**mojo 88 → 54、n2 63 → 54、yaf 51 → 46、dnn 132 → 122。合計 335 → 277。**

### 追加した型の「面」を合わせる — アクセス修飾子は署名の一部

63 型を足したあと CS0246 は 106 → 33 に落ち、代わりに **CS0115 が 83 → 98 に増えました。**
型があると今度は**そのメンバが問われる**ので、これは前進です。ただし新種の
`CS0507`(アクセス修飾子の変更)も出ました。

```
'RssDataSource.GetView(string)': 'public' の継承メンバー 'DataSourceControl.GetView(string)' を
オーバーライドするときに、アクセス修飾子を変更できません
```

**`DataSourceControl.GetView` は System.Web では `protected` です。** public で宣言した
ために、移植された全データソース(n2 の `ItemDataSource`、mojoPortal の `RssDataSource`)
が override できなくなりました。**足す前より悪い**エラーです。

同じ理由で直したもの:

| | 実際の System.Web |
|---|---|
| `DataSourceControl.GetView` / `GetViewNames` | `protected` |
| `HtmlContainerControl.TagName` | `public`(互換層は protected だった) |
| `Parameter.Clone` / `Evaluate` | `protected virtual` |

**アクセス修飾子は署名の一部です。** 宣言だけ合っていても override はできません。

あわせて、足した型が持つべきメンバも埋めました — `DataBoundControl` の
`PerformSelect` / `PerformDataBinding`、`WebBaseEvent`(`Raise` /
`FormatCustomEventDetails`)、`LegacyWebControl` の `UniqueID` / `Font`。

**mojo 54 → 46、n2 54 → 48、dnn 122 → 121。合計 277 → 262。**

### スタブ生成器 5 回目 — 属性の基底と、コンストラクタ

`'StringFormatMethodAttribute' は属性クラスではありません`(CS0616)が 7 件。スタブが
`Attribute` を継承していないためです。基底の解決は「プロジェクトのクラス」と
「互換層のコントロール基底」しか見ておらず、**`System.Attribute` を知りませんでした。**

直すと CS0616 は消え、**同じ 7 件が CS1729「引数 1 のコンストラクターがありません」に
変わりました。** `[StringFormatMethod("format")]` は**コンストラクタ呼び出し**です。

スタブ生成器は**コンストラクタを 1 つも再現していませんでした。** 宣言が無ければ暗黙の
既定コンストラクタだけになるので、`new Stub(a, b)` は全部失敗します。属性はその一形態に
すぎません。本体は空です — 投げるようにすると、属性はランタイムがメタデータを読むときに
構築されるので、リフレクションが落ちます。

BCL の基底は `Attribute` **だけ**を引き継ぎます。一般の framework 基底は abstract メンバを
宣言している可能性があり、それは CS0115 を CS0534 に置き換えるだけです(上の実測)。
`[X]` が `X : Attribute` を要求するのは C# の規則そのもので、列挙ではありません。

**yaf 46 → 42。**

## リポジトリ同梱 DLL への参照

WebForms 期のアプリは依存ライブラリを `_libs\` などにチェックインし、`<Reference>` +
`<HintPath>` で参照します。**NuGet の識別子が無いので引き継ぎようがなく**、変換器は
黙って落としていました。mojoPortal の Lucene.Net がこれで、CS0246 が 115 件出ていました。

**変換器は代替パッケージを選びません。** Lucene.Net 3.0.3 → 4.8 のようにメジャーが
変わって API が別物になっているものがあり、機械的に決められないためです。

代わりに、**答えを書き込める形で出力します。** 変換のたびに出力先へ
`package-map.template.json` を書き、そこに未決の依存が並びます(mojo は 61 件)。

```json
[
  { "_readme": "..." },
  { "assembly": "Lucene.Net", "package": "", "version": "" }
]
```

`package` と `version` を埋めて `--package-map <そのファイル>` で再変換すると
`PackageReference` が入ります。**引き継がないと決めたものは `package` を空のまま**に
すると残差から消えます — 「判断した」と「判断していない」を区別するためです。

テンプレートはそのまま `--package-map` に渡せる有効な JSON です(説明は
`assembly` を持たないエントリに入れてあり、読み込み側が無視します)。

一方 **HintPath の無い `<Reference Include="..." />` は GAC アセンブリ**で、こちらは
同一 API の NuGet パッケージがあります。`System.ComponentModel.Composition`(MEF)を
名前空間対応表に追加し、**DNN が 941 → 901** になりました。

### 調査済み: `DNNNode` / `DNNNodeCollection`(CS0246 58 件)

型ごと欠落しているので変換器の穴を疑って追いましたが、**欠陥ではありませんでした。**

`DotNetNuke.WebControls` という同梱 DLL の型で、`class DNNNode` の宣言は
**入力ツリーのどこにも存在しません**(`DNN Platform/Controls/DotNetNuke.WebControls/`
には `bin` しか無く、ソースは同梱されていない)。既に
`package-map.template.json` に載っており、正しく報告されています。

**再調査しないでください。** 58 件は「ユーザーが代替パッケージを決める」側の数字で、
変換器側で減らせるものではありません。

## あいまい参照 CS0104(未解決)— まず内訳を見てください

**「CS0104 = 互換層の平坦化が原因」は誤りです。** 実測すると 72 件中 28 件しか
互換層は関係していません。名前空間分割に着手する前にこれを確認してください。

```
28  互換層 vs アプリ      TreeNode 20 (n2) / ListItem 6 (yaf) / Image 2 (mojo)
44  アプリ同梱コード vs BCL  Activity 12 / Directory 8 / *Converter 10 /
                           OrderedDictionary 4 / Attribute 2 / BBCode 2 ほか
```

後者は互換層と無関係で、**移行先の .NET が新しいことによる衝突**です。
`System.Collections.Generic.OrderedDictionary` は .NET 9 で新設された型なので
.NET Framework 時代には衝突しようがなく、`System.Diagnostics.Activity` も同様です。
アプリが同梱した Lucene.Net や ServiceStack の型が、後から増えた BCL の型と
名前でぶつかっています。**互換層をどう分割してもこの 44 件は動きません。**

### 互換層側の 28 件

互換層は `System.Web` 全体を `WebForm2Blazor.Components` 1 つに平坦化します。その結果、
**`System.Web` しか import していなかったファイルにも `System.Web.UI.WebControls` 相当の型が
見えるようになり**、アプリが同名の型を持っていると `CS0104` になります。
n2cms の `N2.Edit.TreeNode` と互換層の `TreeNode` がその 20 件です。

**「プロジェクト型を優先する別名を出す」方向で 2 回試し、いずれも撤回しました。**

1. 「プロジェクト名前空間を import しているならプロジェクト型を意図している」
   → `Page` のように元は System.Web 側から来ていた名前まで奪い、**YAF が 187 → 739 に悪化**
2. 「元ファイルが `System.Web.UI*` を import していない場合に限る」
   → n2 は 3 件改善したが YAF が 5 件退行し、差引で悪化

どちらも「その名前が元は互換層側から来ていたのか」を確実に判定できないことが原因です。
根治するには**互換層の名前空間を元の `System.Web.*` の構造に合わせて分割**し、
変換器が元ファイルの import に対応する名前空間だけを補う必要があります。

**ただし、上の内訳を踏まえると費用対効果は低いです。** 互換層の全ファイルに影響する
変更で、上限は 28 件です。同じ労力なら他に効く場所があります。着手するなら
「28 件のために互換層を作り直す」と分かった上で判断してください。

## 既知の注意点

- 出力は毎回作り直されます。前回の生成物が残っているとレポートが混ざり、
  「直したはずの残差がまだ出る」ように見えます(実際に一度これで誤読しました)。
- (解決済み) `wt` の `NVPAPICaller` 未解決は、除外判定がコメント文中の "PayPal." に
  反応していたためでした。変換元にはファイルが存在します。
- これらのスクリプトは PowerShell 5.1 互換です。日本語を含むため
  **BOM 付き UTF-8** で保存してください(BOM 無しだと PS 5.1 は ANSI として読み、
  文字列リテラルが壊れます)。
- **`-SkipBuild` を使うときは互換層と変換器を自分でビルドしてください。** 互換層の
  コンパイルが失敗すると全コーパスがその失敗分だけを報告し(実測: 全 6 本が揃って
  「8 件」)、スクリプトはそれを**改善として通します**。同じ理由で、互換層が壊れている間は
  変換器のビルドも失敗するので、`bin\alt` が古いまま計測される事故も起きます
  (実際に 836→830 と読み違え、再ビルド後の正しい値は 836→779 でした)。
  引数なしの `convert-all.ps1` は両方をビルドするのでこの穴はありません。
- **`-UpdateBaseline` と `-SkipVerifyBuild` の併用でベースラインが壊れます(修正済み)。**
  `-SkipVerifyBuild` はビルドエラーを `-1`(未計測)にしますが、それがそのまま
  `expected.json` に書き込まれていました。`-1` は比較対象外なので、**以後そのコーパスは
  ビルドエラーが何件になっても「一致」を返します。** 実際に yaf と n2 で踏みました。
  「測らなかった」は「0 件だった」ではないので、未計測のときは既存値を保持し、
  警告を出すようにしてあります。

## 除外型スタブの基底クラス

移植対象外になった型は `ExcludedTypeStubs.g.cs` にスタブとして出ます。このスタブが
**基底クラスを失うと、そのスタブを継承しているクラスの override が全部落ちます。**

DNN の `DnnDropDownList : Panel` が実例でした。`Panel` は互換層では Blazor
コンポーネントなので、平のスタブクラスは継承できません。結果スタブは基底無しになり、
`DnnFileDropDownList` などが `OnInit` / `OnPreRender` / `CreateChildControls` の
override で CS0115 になっていました。**エラーはサブクラス側に出るので、原因が
スタブにあることは表示からは分かりません。**

WebForms コントロールが基底の場合は `LegacyWebControl`(平のクラスとしての
コントロール基底)に解決するようにし、**DNN が 836 → 779** になりました。

### スタブのメンバは `virtual` を保つ

同じ理由で、スタブは**元が `virtual` / `abstract` / `override` だったメンバを
`public void Install()` として出していました。** 継承側は移植されているので、
その override が全部 CS0506(「基底が virtual でない」)になります。DNN の
`FileInstaller` / `PermissionsGrid` / `AuthorizeAttributeBase` がこれでした。

元の修飾子を写すだけにしてあります(元が overridable でなければ付けません)。
**DNN 779 → 710、CS0506 は 71 → 0。**

### 残っている CS0115(241 件)

こちらは「基底にメンバ自体が無い」ケースで、2 つに分かれます。

- 互換層の不足(`Control.EnableViewState` など)
- **ASP.NET MVC の型**(`AuthorizeAttribute.AuthorizeCore(HttpContextBase)` など)。
  DNN は WebForms と MVC が同居しているアプリで、MVC 部分はこの変換器の対象外です

前者は互換層に足せますが、後者は「範囲外」であって欠陥ではありません。
**着手する前に、この 2 つを分けて数えてください。**

## ソースジェネレータ(`--analyzer`)

DNN Platform はビルド時に Roslyn ソースジェネレータを走らせます。`[DnnDeprecated]` から
partial メソッドの**定義宣言**を生成するもので、その宣言は**変換元のソースに存在しません**。
変換器はソースを読み書きするだけなのでそれを再現できず、依存するコードがビルドできません
(CS0759 が 230 件)。

ジェネレータを変換器が自前で実行するのではなく、**生成プロジェクトに `<Analyzer>` として
組み込みます**。ビルド時に MSBuild が元と同じようにジェネレータを走らせるため、参照解決も
意味解析も自前で用意する必要がありません。

**アナライザは既定で有効で、ベースラインに含めています。** `convert-all.ps1` が必要に
応じてジェネレータをビルドしてから `--analyzer` で渡します。`-SkipAnalyzers` で外せます。

**実測: DNN のビルドエラー 652 → 422、CS0759 は 230 → 0**(減少幅が CS0759 の件数と
一致します)。

当初はオプトインにしていました。ジェネレータのビルドには**そのプロジェクトが
`global.json` で固定した SDK** が要り(DNN は 9.0.202 / `rollForward: latestMinor`)、
ベースラインがインストール済み SDK に依存してしまうためです。

**これは判断を誤っていました。** アナライザ無しのベースラインは、**元のアプリが実際には
行うビルドとは違うもの**を基準にしていることになります。230 件の CS0759 は、ジェネレータ
が生成する partial メソッド宣言が無いというだけで、変換器の課題ではありません。

ジェネレータがビルドできなかった場合は警告を出して `--analyzer` 無しで続行します。
その結果ベースラインと一致せず失敗になりますが、**それが正しい挙動**です
(黙って別物の数字を通すよりよい)。

## 実際に起動して分かったこと(WingtipToys)

**静的な数字では見えないものを出すために、2 本目のコーパスを起動しました。** BlogEngine で
5 段階の欠陥が出た前例があるので、数を数えるより実物を動かす方が情報量が多いという判断です。

wt はビルドエラー 1 件で止まっていました。`<%: Scripts.Render("~/bundles/modernizr") %>` の
`Scripts` が無いという CS0103 です。**バンドルは既に 3 箇所で「範囲外」と報告済み**
(BundleConfig.cs / Global.asax.cs / スキップした NuGet)なのに、マークアップ側の呼び出しだけが
残ってビルド全体を落としていました。互換層に `Scripts` / `Styles` を置いて解決
(**何も描画しません** — バンドル定義が移植されていない以上、`<script>` を出しても 404 です)。

**wt がビルドエラー 0 になり、9 ルートすべてが 200 を返しました。**

```
/  /About  /AddToCart  /Contact  /ErrorPage  /ProductDetails  /ProductList  /ShoppingCart  200
```

### 200 では分からなかったこと — 未対応スタブが画面に出ていた

**HTTP 200 は「同じ動作」を意味しません。** 実際の HTML を見ると、元アプリには無いテキストが
全ページのヘッダに出ていました。

```
wt Welcome [webopt:bundlereference] Wingtip Toys Home About Contact Products [asp:LoginView] .
```

未対応コントロールのスタブが `<span class="w2b-stub">[asp:LoginView]</span>` を描画していた
ためです。**「元が描画したものを描画する」がゴールである以上、目に見えるマーカーは差異そのもの**
です。HTML コメントに変えました。

```html
<!-- W2B: unconverted control asp:LoginView -->
```

**欠落が見えなくなるわけではありません** — これらは全て変換レポートの残差であり、ページの
ソースにもタグ名が残ります。変わったのは**サイトの利用者には見えなくなった**ことだけです。

### 残った差異

`/ProductList` は**ページ全体がプレースホルダー**です(`System.Web.Routing` = FriendlyUrls
依存。範囲外)。200 は返しますが中身は「変換できません」という通知で、元の商品一覧では
ありません。**これは数字にも現れていません** — 残差としては 1 件です。

**教訓: ビルドエラー 0 とルート 200 は、どちらも「同じ動作」の必要条件にすぎません。**

## 回帰スナップショット(`regression-gate.ps1`)

**パリティテストではありません。** 比較相手は変換後アプリ自身の過去のスナップショットで、
「元の WebForms アプリと同じか」は何も言いません。それを言えるのは
`samples\*\golden-webforms.json` だけで、あちらは**IIS Express で旧アプリを動かして**
録ったものです。**この環境には IIS Express も LocalDB も無く、コーパスの正解データは
作れません**(元アプリを動かせる人にしか作れない、という前回の整理どおりです)。

ここが捕まえるのは「変換器を触ったら意図せず DOM が変わった」です。実際このセッションで
wt の DOM を 2 回変えており(スタブのコメント化、`Scripts` シム)、どちらも意図的でしたが
**意図しない同種の変更を検出する仕組みはありませんでした。**

```powershell
.\corpora\regression-gate.ps1            # 記録済みスナップショットと照合
.\corpora\regression-gate.ps1 -Record    # 現在の出力を新しい基準として記録
```

対象は**ビルドが通るコーパスだけ**です(be / wt)。他は起動できません。

### これが即座に暴いたこと — 「200」は動作を意味しない

記録した瞬間に分かりました。**BlogEngine のホームは記事を 1 件も描画していません。**

```
be / の可視テキスト(全 55 文字):
  Account Login << Older posts Newer posts >>
```

HTTP 200 で、テーマのマークアップ(ページング、ログインリンク)は出ます。
**記事だけが出ません。** 原因は `PostList.ascx.cs` のこの 2 行です。

```csharp
var postView = (PostViewBase)this.LoadControl(path);   // LoadControl は null を返す
this.posts.Controls.Add(postView);                     // Controls は描画されない
```

`LoadControl`(実行時の動的コントロール生成)は互換層で null を返し、
`Controls.Add` されたものを `HtmlGenericControl` は描画しません — 描画するのは
`ChildContent` だけです。**どちらも既知の未対応**で、残差としては「実装待ち」に
分類されています。

**このセッションで「BlogEngine は 5/5 で動く」と報告したのは過大でした。** 5 ルートが
200 を返すのは事実ですが、中核機能である記事一覧は動いていません。ルートの
ステータスコードだけを見ていたためで、**同じ誤りは wt でも起きていました**
(`/ProductList` はページ全体がプレースホルダー)。

### `LoadControl` と `Controls` — 実装したが、まだ記事は出ない

回帰スナップショットが暴いた「記事が 1 件も出ない」の直接原因は 2 つで、両方直しました。

| | 直したこと |
|---|---|
| `LoadControl` が常に null | 変換器が `UserControlCatalog.g.cs`(仮想パス → 変換後コンポーネント)を出力し、`LoadControl` がそこから実体を作る。**対応を知っているのは変換器だけ**なので、導出せず記録します |
| `Controls` が描画されない | `WebFormsControlBase.RenderDynamicChildren` を追加。Blazor コンポーネントは `DynamicComponent` 経由、平の `LegacyWebControl` は自身の `Render` の出力をマークアップとして差し込みます |

**それでも記事は出ませんでした。** 手前にもう 1 つあります。

#### DRIVE_ONLOAD — 完了。記事が出るようになりました

`AutoEventWireup` の `Page_Load` は駆動していましたが、**`OnLoad` の override は駆動して
いませんでした。** コントロールライブラリは後者を使うのが普通で、BlogEngine の
`PostList` は `OnLoad` で記事を組み立てます。**一度も呼ばれていませんでした。**

駆動を入れると回帰ゲートが 3 点の差分を出し、**そこから 2 つの欠陥が出てきました。**
どちらも「1 か所の欠陥が複数の症状に見えていた」型です。

**欠陥 1 — `@ref` がラッパーを指していた。**
型を解決できないコントロールは `LegacyRenderHost` で描画し、コード側のフィールドは
`dynamic` にしていました。その `dynamic` に `@ref` で入るのは**ラッパー**であって
コントロール本体ではありません。`recaptcha.UserUniqueIdentifier` も `pager1.Posts` も
`LegacyRenderHost` に解決されて `RuntimeBinderException` になります。
変換器側の元のコメントは「dynamic はコードをコンパイルさせるため」と書いてあり、
**実行時に落ちることは分かった上で放置されていました。** 駆動を入れて初めて走ります。

ラッパーは `__{id}_host` という別フィールドで受け、名前つきフィールドは
そこから `ControlInstance` に届くようにしました。

**欠陥 2 — `DynamicComponent` は自前で別インスタンスを作る。**

```csharp
PostViewBase postView = (PostViewBase)LoadControl(path);
postView.Post = Post;            // ただのプロパティ。Parameter ではない
pwPost.Controls.Add(postView);
```

`Controls` の描画に `DynamicComponent` を使っていたので、**ページが設定した
インスタンスは捨てられます。** `[Parameter]` だけ引き継ぐ実装にしていましたが、
WebForms のコントロールが持つ状態はほぼ `[Parameter]` ではありません。
新しい方の `Post` は null で、`PostViewBase.OnInit` が落ちていました。

Blazor に「既にあるインスタンスを描画する」経路はありませんが、
**`IComponentActivator` はレンダラーが実体を要求する唯一の場所**です。
`PreparedComponentActivator` を挟み、用意済みのコントロールをそこで返します。

結果、**未処理例外は 0 件**になり、回帰ゲートの差分 3 点はすべて改善でした。

| ページ | 差分 | 判定 |
|---|---|---|
| home | `Welcome to BlogEngine.NET` と `<article id="post0">` が出現 | **記事が描画された** |
| search | 本文 0 行 → `Search` | 改善 |
| contact | 添付欄が消えた | **元の挙動。** `App_Data/settings.xml` の `enablecontactattachments` は `False` で、`phAttachment.Visible = BlogSettings.Instance.EnableContactAttachments` が効くようになった結果です |

スナップショットは再記録しました。パリティ 30/30、bUnit 30/30、全 6 コーパスの数字は不変です。

`[RecaptchaControl: render error]` は**この作業の前から出ています**(再記録前の
スナップショットにも入っています)。退行ではなく、未着手の別件です。

#### 回帰ゲートの欠陥も 1 つ直しました

対象が全部ポート 5080 を使うのに解放を待っていませんでした。前のアプリが
終了しきる前に次を起動すると、bind に失敗した上に**死にかけの前アプリが 200 を返して
起動確認を通過**します。wt が `ERR_CONNECTION_REFUSED` で落ちたのはこれでした。

## BlogEngine の稼働状況

**ステータスコードであって、動作ではありません。**(上の回帰スナップショットの節を参照)

```
/          200   ただし記事は描画されていない(LoadControl 未対応)
/archive   200
/search    200
/contact   200
/post      200   500 から改善(Init と @ref の順序)
```

**変換出力がロールデータを実際に読んで動いています。** ロールは `App_Data/roles.xml` から
`Administrators / Editors / Anonymous` が読み込まれ、移植された `XmlRoleProvider` が
そのまま供給しています。

ここに至るまでに解いた 5 段階(いずれも汎用の仕組みとして実装):

| 症状 | 原因 |
|---|---|
| `GetSection` が null | カスタム構成セクションが引き継がれていなかった |
| `Could not resolve type` | 型のアセンブリ修飾が旧アセンブリのままだった |
| `Unable to load default provider` | `ProvidersHelper.InstantiateProviders` が空実装 |
| `DirectoryNotFound` | `App_Data` 未コピー / `AppDomainAppPath` が bin を指していた |
| **スタックオーバーフロー** | **`Roles` がロールプロバイダに委譲していなかった** |

最後の 1 つは `Right.RefreshAllRights` ↔ `SaveRights` の無限再帰として現れました。
`GetAllRoles()` が空を返すと BlogEngine は「権限テーブル未設定」と判断して既定値を
書き込み、読み直し、また空を見る、を繰り返します。

**fail closed の方針(HANDOVER 2.4)は維持しています。** ロールストアが**無い**ときは
従来どおり空を返し、`IsUserInRole` も false です。変わったのは、**アプリ自身のロール
プロバイダとそのデータが変換出力に揃っているときに、それを使うようになった**点だけで、
ロールを捏造してはいません。

`/contact` は 3 つの修正で通るようになりました。いずれも汎用の欠陥です。

| 症状 | 原因 | 修正 |
|---|---|---|
| `NullReferenceException` | `<label for="<%= txtName.ClientID %>">` — `@ref` は初回描画後にしか代入されない | 同一ファイルで宣言された ID の `X.ClientID` を `ClientIdOf("X")` に書き換え |
| `ChildContent が無い` | `<asp:Label>本文</asp:Label>` のタグ内容 | `Label` に `ChildContent` を追加(`Text` 優先は WebForms と同じ) |
| `parameter ... is not public` | getter のみのプロパティに `[Parameter]` を付与していた | public セッターがあるものだけに限定 |

3 つ目は**実行時**にしか出ません。ビルドは通り、そのコンポーネントが初めて描画された
瞬間に落ちます。

### Init と @ref の順序(解決 — `/post` 200、5/5 稼働)

**`/post` が通り、BlogEngine は 5 ルートすべてが 200 になりました。**

WebForms はコントロールツリーを構築してから `OnInit` を呼ぶため、`OnInit` の中で宣言済み
コントロールに触れるのは普通のコードです(`ucCommentList.Visible = ...`)。Blazor では
それらは `@ref` フィールドで、**初回描画後にしか代入されません**。

`OnInit` を `OnAfterRender(firstRender)` に遅らせる修正は**以前に試して撤回済み**です。
`OnInit` は描画に必要なデータを作る側でもあり(Post ページは `OnInit` でページ全体が
バインドする `Post` を代入します)、遅らせると `ucCommentList` の null は消えても今度は
`Post` が null になり、例外が移動するだけでした。

**コードは動かさず、代入の方を遅らせました。**

```csharp
private CommentList __ucCommentList_pending;
private CommentList __ucCommentList_ref;
protected CommentList ucCommentList
{
    get => __ucCommentList_ref ?? (__ucCommentList_pending ??= new CommentList());
    set { __ucCommentList_ref = value; __ucCommentList_pending?.ReplayPendingStateOnto(value); }
}
```

`@ref` が実体を届けるまでは**身代わり**を返します。身代わりへの代入は
`IDeferredControlState.PendingState` に記録され(レンダーハンドルが無いときだけ)、実体が
来た瞬間に転写されます。**実際に代入されたプロパティだけ**が転写されるので、誰も触って
いないプロパティはマークアップの値のままです — WebForms もそうでした。

**身代入れできる型だけが対象です。** 判定は名前ではなく**互換層アセンブリに問い合わせ**ます。
`ObjectDataSource` は `ComponentBase` を直接継承していて記録できず、身代わりを立てると
代入を黙って飲み込みます。実際そこで `CS1929` が出て気づきました(`dynamic` のスタブ
プレースホルダも対象外)。**記録できない身代わりは、置き換える null より悪いです。**

全 6 コーパスの数字は 1 つも動かず、パリティ 30/30。

```
/          200
/archive   200
/search    200
/contact   200
/post      200   ← 500 から
```

## abstract 基底: スタブ自身を abstract にする(dnn −8 / n2 −2)

CS0115 を数え直すと 61 件。最大は `AllowableFiles` 8 件で、これは前に調べて
**撤回した**ものです(dnn −14 / yaf **+138**)。撤回時の結論は
「スタブがメンバを 1 つ残らず再現できるようになってから再挑戦」でした。

**その結論が間違っていました。** 問題は「スタブが abstract メンバを実装できない」
ことではなく、**スタブを具象クラスとして出していた**ことです。

```csharp
public class FileInstaller { ... }                        // 基底を落とす → CS0115
public class FileInstaller : ComponentInstallerBase { }   // 実装が無い → CS0534
public abstract class FileInstaller : ComponentInstallerBase { }   // 正解
```

abstract にすれば、実装の義務は**実際のサブクラスに渡ります。** そのサブクラスは
元のアプリで実装していたからこそコンパイルできていたので、必ず実装を持っています。
yaf の +138 はそのまま 0 になりました。

### 属性クラスだけは abstract にできない

n2 が +1 になり、`CS0653: 抽象であるため属性クラス 'DisplayableImageAttribute' を
適用できません` が 3 件出ました。属性クラスは abstract にできません。

**`Attribute` という接尾辞では判定していません。** それは規約であって
コンパイラの規則ではないので、移植済みクラスの基底リストを辿って
`System.Attribute` に届くものを abstract 集合から外しています。
結果 n2 は **−2**(元からあった CS0616 3 件も消えました)。

| | ビルドエラー |
|---|---:|
| dnn | 87 → **79** |
| n2 | 39 → **37** |
| yaf | 40 → 40(以前の +138 は出ません) |
| 合計 | 216 → **206** |

パリティ 30/30、bUnit 30/30、回帰スナップショット be 5/5・wt 8/8。

## `global using` が統合後に全ファイルへ漏れていた(yaf 40 → 13)

CS0104(あいまいな参照)36 件。1 件見て原因を確かめました。

```
Lucene.Net/Codecs/Compressing/CompressingStoredFieldsWriter.cs(72,26): error CS0104:
'Directory' は 'yaf...Lucene.Net.Store.Directory' と 'System.IO.Directory' 間の
あいまいな参照です
```

**元ファイルは `using System.IO;` を持っていません。** using 行は変換前後で
(名前空間の付け替えを除き)同一です。ではどこから来たのか。

```
corpora/out/yaf/YAF.Web/GlobalUsings.cs:7:  global using System.IO;
```

`global using` は**コンパイル単位全体**に効きます。元のソリューションでは
YAF.Web は独立したアセンブリなので、この行は YAF.Web にしか届きませんでした。
変換後は全部が 1 プロジェクトに統合されるので、**同梱の Lucene.Net にまで届きます。**
yaf の CS0104 はこれが全部でした。**元のアプリには 1 件も無かったエラーです。**

ライブラリ(`--include`)の `global using` は、そのライブラリのファイル内の
ファイルスコープ using に戻すようにしました。**ファイルが見る import は、
元のコンパイルでそのファイルが見ていた import と一致していなければなりません。**

Web プロジェクト自身の `global using` はそのままにしてあります。そちらのファイルは
ページで、生成される `.razor` / `.razor.cs` は別経路で書き出されるため、
閉じ込めると逆に失われます。

**yaf 40 → 13。合計 206 → 179。** 他 5 本は不変、パリティ 30/30、回帰 13/13。

## 互換層が WebForms の名前空間を 1 つに潰していた(n2 37 → 27)

n2 の CS0104 は 10 件すべて `TreeNode` で、`N2.Edit.TreeNode` と
`WebForm2Blazor.Components.TreeNode` の衝突でした。

元ファイルの import はこうです。

```csharp
using System.Web;                      // ← 変換後は using WebForm2Blazor.Components;
using System.Web.Script.Serialization;
```

**`System.Web` に `TreeNode` はありません。** あれは `System.Web.UI.WebControls` の型です。
WebForms は型を 10 個以上の名前空間に分けていますが、**互換層は 1 つ**なので、
そのうちどれを import しても全部が入ってきます。

アプリ自身が同じ単純名を、そのファイルが import している名前空間で宣言しているなら、
**アプリの型が正解です。** もし元のファイルがその名前を宣言する WebForms 名前空間も
import していたなら、元のコードが曖昧でコンパイルできなかったはずだからです。
その名前にファイルスコープのエイリアスを足すようにしました。

`Attribute` という接尾辞のときと同じで、**接尾辞や名前の一覧ではなく、
移植したソースが実際に何を宣言しているか**から出しています。

## 深い構文木でスタックが尽きていた

上を入れたあと、yaf の変換が `InsufficientExecutionStackException` で落ちるように
なりました。しかも**再現しません** — 同じコーパスが 1 回目は通り 2 回目で落ちます。

`CSharpSyntaxRewriter` は構文ノード 1 個につきスタックフレームを 1 つ使い、
`EnsureSufficientExecutionStack` で自己申告します。既定の 1MB では、実アプリが同梱する
生成コードや巨大ライブラリ(YAF は Lucene.Net 一式を持っています)に足りません。

書き換えを 64MB スタックのスレッドで回すようにしました(`DeepSyntaxWork`)。
**変換前から潜んでいた不安定さで、今回の変更が顕在化させただけです。**

## 曖昧解消の一般化(yaf 13 → 10、mojo −1)

`TreeNode` で入れた「アプリの型を優先」を、衝突全般に広げました。順位は
**移植ソースの型 > BCL > 互換層**で、根拠はどれも「元のコードはコンパイルできた」だけです。

- 移植ソースの型が勝つ — その名前を宣言する名前空間をこのファイルが両方 import して
  いたなら、元のコードが曖昧だったはずです
- 次に BCL — 互換層は変換の産物で、元の import 一覧にその名前はありませんでした
- 同順位で 2 つ並んだら元も曖昧なので、復元すべきものは何もありません(何も出しません)

### グローバル using はファイルの構文木に現れない

最初の実装はファイル自身の using しか見ておらず、1 件しか直りませんでした。
`Attribute` も `Module` も `Calendar` も、**Web プロジェクトの `global using` 由来**で、
ファイルには書かれていません。環境側 import として渡したところ **13 → 1** になりました。

### ただしその「1」は見かけでした

**構文エラーが 1 件でもあると、コンパイラは意味解析に進みません。**
`NullableAttributes.cs` の CS1032 が、残り 11 件の意味エラーを丸ごと隠していました。

CS1032 の原因はこちらの欠陥です。`WithUsings` は using を先頭に入れますが、
using が 1 つも無いファイルでは `#pragma` / `#define` / `#if` や著作権表示は
**最初のトークンの前置トリビア**なので、using がその前に入ります。
`#define` より前の using は CS1032 です。ヘッダを先頭に留める処理を入れました。

**ビルドエラー件数は、構文エラーがあると過少に出ます。**

### `System.Web` へは解決しない

もう 1 つ、`using AspNetHostingPermission = System.Web.AspNetHostingPermission;` を
出してしまい CS1069 になりました。型転送された名前はフレームワークの索引に残っていますが、
**System.Web は互換層が置き換える対象**なので答えになり得ません。候補から外しました。

## 名前空間の一括置換が、互換層に実在するか確かめていなかった(yaf 10 → 7、dnn −1)

`System.Security.Permissions.PermissionState` が
`WebForm2Blazor.Components.PermissionState` になっていました。**互換層にそんな型はありません。**

```csharp
.Replace("System.Security.Permissions.", "WebForm2Blazor.Components.")
```

同じファイルに、この欠陥を**既に一度直したコメントが書いてあります**。
`System.Web.X` については「互換層が X を宣言しているときだけ置換する」ようになっていて、
`RewriteRootSystemWebTypes` がそれをやっています。**残りの名前空間は無条件のままでした。**

全部そちらに寄せました。置換元の名前空間の一覧は WebForms 側で固定なので持ちますが、
**互換層に何があるかは常にアセンブリに聞きます。**

### 付随して直したもの

| | |
|---|---|
| `System.Security.Permissions` | `CodeAccessPermission` / `PermissionState` の移行先パッケージを追加。using を見るだけでは足りません(**完全修飾で 1 回だけ書く**のが普通の書き方で、OrmLite がそうしています)ので、既知パッケージの接頭辞だけは本文からも拾います |
| `AllowUnsafeBlocks` | 移植コードに `unsafe` があれば有効化。**元のプロジェクトが許可していたことは確実**です(でなければコンパイルできていません) |
| `CallContext` | Remoting は .NET から消えましたが、`CallContext` は .NET Framework 製ライブラリが**リクエスト単位の状態**を運ぶ手段として生き延びていました。OrmLite は開いた接続とトランザクションをここに置きます。`System.Runtime.Remoting.Messaging` という**元の名前空間のまま**互換層に置きました。`LogicalGetData` は `AsyncLocal`、`GetData` は `ThreadLocal` — 元の「流れる/流れない」の差をそのまま残しています |

## アセンブリを 1 つに統合すると、名前解決が変わる(yaf 7 → 4)

```
Lucene.Net/Analysis/NumericTokenStream.cs: error CS0234:
  'Attribute' が名前空間 'yaf...Lucene.Net.Analysis.Util' に存在しません
```

ファイルは `YAF.Lucene.Net.Analysis` の中で `Util.Attribute` と書き、
`using YAF.Lucene.Net.Util;` でそれを解決していました。
**`YAF.Lucene.Net.Analysis.Util` は別アセンブリ**(Analysis.Common)にあり、
Lucene.Net 本体はそれを参照していないので、そもそも見つからなかったのです。

統合後は全部が 1 コンパイルです。C# は **using より先に外側の名前空間を見る**ので、
`Analysis.Util` が見つかり、そこに `Attribute` が無い、となります。

**エイリアスでは直せません。** 名前空間のメンバはどの階層でも using エイリアスより
先に引かれます。完全修飾で書き直すしかないので、そうしました。

条件は「外側の名前空間チェーンがその頭を捕まえていて、かつ空振りしている」ときだけです。

### 併せて

`LegacyWebControl` に `OnClick` を追加しました。`LinkButton` 派生の移植クラス
(YAF の `CollapseButton`)は `OnClick` を override しますが、`LinkButton` の互換型は
Blazor コンポーネントで、平のクラスは継承できないため `LegacyWebControl` に落ちます。
`RaisePostBackEvent` から呼ぶようにしてあるので、**押されたときに動く**経路も繋がっています。

## `convert-all.ps1` が、ビルドに失敗した変換器の古いバイナリで測っていた

```powershell
if ($LASTEXITCODE -ne 0) { Write-Error '変換器のビルドに失敗しました。' }
```

`$ErrorActionPreference = 'Continue'` なので、**`Write-Error` は止めません。**
`bin\alt` に前回のバイナリが残っているため、変換は成功し、
**変更が反映されたかのような数字が出ます。** 実際にこれで 1 回測り間違えました。
`exit 1` に変えました。

**`-SkipBuild` を使わなくても同じ罠にはまります。**

### 撤回: 「.NET に存在しない名前空間の using を落とす」

yaf に `using System.Runtime.Remoting.Contexts;` が 1 件残っていました。この名前空間は
.NET から丸ごと消えているので、import 自体がエラーです。しかもこのファイルは
そこから型を 1 つも使っていません。

**判定方法が成立しませんでした。** 「`System.` で始まり、フレームワーク索引に無い」では
足りません。BlogEngine は `System.Linq.Dynamic` を**パッケージから**取っています。

パッケージ id を許可リストにしてみましたが、これも駄目でした。
**id と名前空間は一致しません** — `System.Linq.Dynamic` を供給しているパッケージの id は
`DynamicQuery` です。

| | be | yaf |
|---|---:|---:|
| 落とす | **+13** | −1 |

撤回しました。正しい判定は「**.NET Framework には在り、.NET には無い**」で、それには
Framework の参照アセンブリが要ります(環境に無いこともあります)。
1〜3 件のために持ち込む依存ではないと判断しました。

`yaf` の残り 3 件はいずれも別種です。

| | |
|---|---|
| `OrderedDictionary<,>` 2 件 | `J2N` と、**.NET 9 で追加された** `System.Collections.Generic` の衝突。ジェネリックなのでエイリアスで解決できず、使用箇所を修飾するしかありません。どちらが正しいかは「元がコンパイルできた」からは導けますが、`J2N` がその型を持つことを変換器は確認できません(パッケージは復元前です) |
| `DbProviderFactory.CreatePermission` 1 件 | **.NET が削除した API**(CAS ごと)。元は net481 向けにコンパイルしていました。真の残差です |

## 互換層のインターフェースが、変換器の書き換え後の型と揃っていなかった(dnn −9 / mojo −2)

```
CheckBoxColumnTemplate は ITemplate.InstantiateIn(Control) を実装しません
```

移植後のコードはこうなっています。

```csharp
public void InstantiateIn(IWebFormsControl container)   // 元は Control container
```

**書き換えは正しい**です。`Control` 引数は `IWebFormsControl` になります —
移植後のコントロールは Blazor コンポーネントか平の `LegacyWebControl` のどちらかで、
両方をまたげるのはインターフェースだけだからです。

揃っていなかったのは**互換層側**で、`ITemplate.InstantiateIn` が `Control` のままでした。
実装側は全部書き換えを通るので、**誰一人として実装していないことになります。**
DNN のカラムテンプレート 9 個がこれでした。

## モデル化されていない基底の override を外す(147 → 121)

CS0115 が 50 件。中身は全部同じで、**元の基底が持っていた仮想メンバを互換層の基底が
持っていない**でした。

```
DNNDataGrid.CreateControlHierarchy(bool) : オーバーライドする適切なメソッドがありません
EmailValidator.EvaluateIsValid()        : 同上
mojoDropDownList.PerformDataBinding(IEnumerable) : 同上
```

WebForms はウィジェットごとにクラスがあり、それぞれ固有の仮想メンバを持っていました
(`DataGridColumn.Initialize`、`BaseValidator.EvaluateIsValid`、
`ListControl.PerformDataBinding`)。互換層は描画するものだけをモデル化し、
残りは `LegacyWebControl` に潰しています。**本体が完全に正しいメソッドで、
ファイル全体がコンパイルできなくなります。**

`override` を外して `virtual` に落とすようにしました。本体は残ります。
互換層はこれらを**元々呼びません**(DataGrid やバリデータの挙動を実装していません)ので、
呼び出しが消えるのではなくエラーが消えます。

**1 件ずつ残差として記録します。** 基底がモデル化されていないコントロールは
元と同じ動きをしないので、そこは見えていなければなりません(残差 +29 はこれです)。

**何を持っているかは互換アセンブリに聞きます。** 一覧を持つと、互換層が育ったときに
「実は有効だった override を黙って外す」方向に腐ります。

### 2 回間違えました

| | |
|---|---|
| `override` を単に削除した | 同じアプリ内の派生クラスが override できなくなり CS0506。`virtual` に置き換えました |
| 単純名で互換層と照合した | DNN は**自前の** `MembershipProvider` を宣言しています。ASP.NET の同名型と取り違え、DNN の abstract メンバの実装 30 個から override を剥がし、**13 件が 52 件になりました**。移植側が宣言している名前は互換層より優先します |

2 つ目は、このセッションで `TreeNode` と `Attribute` に対して立てたのと同じ規則です。
**アプリ自身の型が勝ちます。**

## 互換層に足りない WebForms API と、アクセシビリティの整合(121 → 113)

| | |
|---|---|
| `Control.EnsureChildControls` / `HtmlTextWriter.RenderBeginTag` | WebForms では **virtual** です。非 virtual で出していたため CS0506。mojoPortal は `HtmlTextWriter` を 2 つ派生させて `RenderBeginTag` を override します |
| `TemplateContainerAttribute(Type, BindingDirection)` | 2 引数形が無く CS1729。`BindingDirection` 列挙型ごと追加 |
| `PersonalizationScope` | 列挙型が無く CS0103 |

### `override` のアクセシビリティを基底に合わせる

```
FieldSet.RenderBeginTag(HtmlTextWriter): 'public' の継承メンバーをオーバーライドするとき
アクセス修飾子を変更できません
```

WebForms は**同じメソッドを基底ごとに違うアクセシビリティで宣言**していました。
`WebControl.RenderBeginTag` は public、`HtmlControl` の方は protected です。
互換層はその両方を 1 つに潰しているので、移植後の override の半分が食い違います。

基底が public なら `protected override` を `public override` に広げるようにしました。
**広げる方向しか必要になりません** — 互換層が元より狭く宣言することはないからです。

## 継承チェーンを辿る(dnn 55 → 51)。ただし 3 回間違えました

override を外す判定は**直接の基底が互換型のときだけ**でした。実際の移植コードは
`ProfileDefinitions` → `PortalModuleBase` → `UserControlBase` → 互換
`WebFormsUserControl` のように、間に移植済みクラスが挟まります。
移植済みクラスの「基底」と「宣言メンバ」を索引にして、チェーンを辿るようにしました。

**互換でも移植済みでもない基底に当たったら、そこで「触らない」を返します。**
中身が見えないサードパーティで推測すると、正しく binding している override を剥がします。

### 3 回間違え、3 回とも残差の増加が教えてくれました

**ビルドエラーは減るのに残差だけ増える**、という形で出ます。
これは「エラーになっていなかった有効な override を外した」ということです。

| | |
|---|---|
| `sealed` クラスに `virtual` | CS0549。sealed なら修飾子ごと落とします(派生しようがないので失うものもありません) |
| `Type.GetMember` が**基底の protected を返さない** | `FlattenHierarchy` は **static 専用**です。`Panel` に `OnLoad` を聞くと「無い」と答えます(宣言しているのは `LegacyWebControl`)。`BaseType` を自分で辿るようにしました |
| ジェネリックを名前だけで照合 | n2 は `Page<TPage>` を宣言しています。`class EditPage : Page` をそれと読み、**n2 自身の階層へ迷い込みました**。索引を `名前\`アリティ` に |
| 索引が**書き換え前**の基底名を持つ | 索引は元ソースから作るので `UserControl` のままです。互換層の `UserControl` は空のマーカー型で、`WebFormsUserControl` ではありません。同じ対応表を通すようにしました。n2 の `OnInit` / `OnPreRender` が 12 件剥がされていました |

最後の 2 つが特に危険です。**ビルドは通ったままコントロールの挙動だけが変わります。**
残差を 1 件ずつ出していなければ気づけませんでした。

## パッケージの重複(mojo −6)と、`System.Web` を必要とするライブラリ(dnn −14)

### `ZipFile` が 2 つのアセンブリにある

```
CS0433: 型 'ZipFile' が 'DotNetZip' と 'Ionic.Zip' の両方に存在します
```

mojoPortal は `DotNetZip.Original` を宣言しており、変換器は `Ionic.Zip` の import を見て
`DotNetZip` を足していました。**同じライブラリが 2 つ入り、使用箇所が全部エラー**です。

Id で重複排除していましたが、**fork や改名では id が変わります**(古い id が接頭辞として
残ります)。宣言済みパッケージの id が、足そうとしている id で始まるなら足しません。

### `System.Web` を必要とするライブラリは、変換器にできることがありません

dnn の `DotNetNuke.Web.Client` に 26 件集中していました。原因は 1 つで、
`Dnn.ClientDependency` パッケージ(.NET Framework ビルド)の公開 API が
`System.Web.UI.Control` を含んでいることです。

```
CS7069: 型 'Control' への参照では 'System.Web' で定義されていると
        指定されていますが、見つかりませんでした
```

**これは「変換器が直せる」類のものではありません。** `System.Web` への参照を
出しているのは変換器ではなく、参照先のライブラリです。同梱 DLL の置き換え先が
未決のときと同じ扱いにし、**件数から外してレポートに明示**しました。

判定は `CS7069` / `CS0012` が `System.Web*` アセンブリを名指ししたときだけです。
**変換器は System.Web への参照を一切出しません**(外すのが仕事です)ので、
この 2 つが System.Web を名指ししたら、必ず参照ライブラリ側です。

**これは修正ではなく分類の変更です。** dnn の −14 はコードが 1 行も良くなっていません。

## コードビハインドに `partial` が無い / `record` のスタブ / 自分で入れた退行(89 → 80)

| | |
|---|---|
| `BindingDirection` | **前回自分で入れた退行です。** `System.ComponentModel.BindingDirection` は .NET に残っているのに、互換層に同名の列挙型を足したため、今度は逆向きの CS1503 になりました。互換層の型は消し、BCL の型を使います |
| `partial` が無いコードビハインド | `.razor` は必ず `partial class` を生成しますが、**WebForms は designer ファイルが無ければ普通のクラスで構いません**(mojoPortal の `layout.Master.cs`)。付けるようにしました |
| `record` のスタブの `ToString` | スタブは「基底を残せなかった override」を `virtual` に落としますが、**`object` のメンバは常に基底があります。** `record` は `ToString` を自前で宣言するので CS8869。`ToString` / `Equals` / `GetHashCode` は `override` のまま出します |

### 構文エラーの警告が効きました

`partial` を足すとき `AddModifiers` がトリビア無しでトークンを付けるため
`public partialclass Layout` になりました。このとき数字は **mojo 4 / n2 5** と出ます。

前回入れた「**構文エラーがあるため件数は下限です**」の警告が無ければ、
**大幅な改善だと思い込んで commit していました。**

## 互換層の欠けと、プロジェクト設定(80 → 69)

| | |
|---|---:|
| `GridViewSelectEventArgs` / `GridViewDeletedEventArgs` / `GridViewUpdatedEventArgs` | n2 −3 |
| `HtmlTitle` / `SqlDataSource` | n2 −2 |
| `LegacyWebControl.FindControl` の戻り値 | dnn −2 |
| `GenerateAssemblyInfo` | dnn −3 |
| `sealed override` から override を外したときの `sealed`(**自分のバグ**) | dnn −1 |

`FindControl` は `ITemplate` と同型です。互換層の他の `FindControl` は全部
`IWebFormsControl` を返すのに、`LegacyWebControl` だけが `Control` を返していました。
移植側の override は `IWebFormsControl` に書き換わるので一致しません(CS0508)。
**インターフェースを返すことで、子が Blazor コンポーネントのときに
黙って取りこぼす問題も同時に消えます。**

`GenerateAssemblyInfo` は、移植コードがアセンブリ属性を宣言していると SDK 生成分と
重複するためです(CS0579)。検出は 1 回外しました — DNN が同梱する log4net は
`[assembly: System.Reflection.AssemblyCompany(...)]` と**名前空間を書き出して**います。

`sealed override` は自分で入れたバグです。override を外すと `sealed` だけが残り、
CS0238(「override ではないため sealed にできません」)になります。

## WCF のホスティング、`PathDirection`、そして署名で絞る(69 → 62)

| | |
|---|---:|
| `System.ServiceModel.Activation` / `.Web` を移植対象外へ | mojo −3 |
| `PathDirection` 列挙型 | mojo −2 |
| `Equals(string, string)` に override を付けていた(**自分のバグ**) | mojo −2 |

### WCF は「クライアント側」と「ホスティング側」で違います

`System.ServiceModel` をまるごと除外するのは**広すぎます** — クライアント側
(`ChannelFactory`、`ServiceContract`)には .NET のパッケージがあり、そのまま移植できます。
.NET に対応物が無いのは**Web アプリ内でサービスをホストする側**
(`ServiceHost`、`ServiceHostFactory`、`System.ServiceModel.Activation`)です。
mojoPortal の `mojoServiceHostFactory` がそれで、**WCF サービスは WebForms ページでは
ありません。** MVC / Web API / Identity と同じ扱いにしました。

### `object` のメンバは名前ではなく署名で判定する

前回「`ToString` / `Equals` / `GetHashCode` は override のまま出す」を入れましたが、
**`Equals(string, string)` は `IEqualityComparer<string>` のメンバで、`object` のものでは
ありません。** 名前だけで判定していたため、override する先が無い宣言を作っていました
(mojoPortal の `UserProfileKeyComparer`)。引数の数まで見るようにしました。

## コードビハインドにも override 判定を効かせる — 致命的なバグを 2 つ踏みました(62 → 56)

`ProfileDefinitions.LoadViewState` などはコードビハインド側にあり、判定が効いていません
でした。効かせたところ **be +2 / yaf +1 の残差だけが増え、ビルドエラーは動きません。**
また同じ形です。何を外したか見ると、2 つとも**黙って壊す**種類でした。

### 1. 変換器が自分で生成した `OnAfterRender` を外していた

```
Install.OnAfterRender の override を外しました
```

WebForms に `OnAfterRender` はありません。これは**変換器が
`InsertGeneratedMembers` で生成したライフサイクル駆動そのもの**です。
判定を生成の**後**に置いたため、自分の出力を削っていました。

**ビルドは通ります。ページのライフサイクルが動かなくなるだけです。**
生成の前に移しました。

### 2. 基底リストの先頭がインターフェースだった

```
CommentList.OnAfterRender の override を外しました。ICallbackEventHandler から上に…
```

コードビハインド側は基底クラスを `.razor` の `@inherits` に移すので、
**残った先頭はインターフェース**です。それを基底クラスとして判定していました。

### 併せて

| | |
|---|---:|
| `System.Net.Http.Formatting`(Web API)を移植対象外へ | dnn −5 |
| コードビハインドの override 判定 | dnn −1 / mojo −1 |

## 挙動の穴のほうを見る:`Repeater` の `DataSourceID`(変換可能 62 → 48)

ビルドエラーの残りが長い尾になったので、**「変換可能」残差**を見ました。
これは「変換器が対応すべきなのにしていない」もので、**ビルドは通るが元と違う動きをする**
ものが入ります。

落ちている属性 36 件を集計すると、最大は `DataSourceID` **13 件で全部が
`asp:Repeater`** でした。

```
<asp:Repeater> の属性 DataSourceID="idsNews" はマッピング未定義のため除去しました
```

**`DataSourceID` だけでバインドする Repeater は、WebForms で最も普通の書き方**です
(コードビハインドに `DataBind()` の呼び出しはありません)。属性を落とすと、
そのコントロールは**何も描画しません。**「機能が足りない」ではなく、
**黙って空になる**種類の違いです。

`GridView` と `ListView` には既に実装されていました。同じものを `Repeater` に入れ、
変換器のマッピング表にも足しました。

**n2 の変換可能 25 → 11、総残差 207 → 193。** パリティ 30/30、回帰 13/13。

## 未実装の標準コントロール:`MultiView` / `View` / `Wizard` / `WizardStep`

「実装待ち」129 件のうち、**標準コントロールの互換コンポーネント未実装が 34 件**でした。
これらは丸ごとスタブ(HTML コメント)になるので、**そのコントロールが持っていた UI が
ページから消えます。**

最大は Wizard 系 17、次いで `MultiView` / `View` 3。どちらも
「複数の子のうち 1 つを表示する」同じ形なので、まとめて実装しました。

非アクティブな子も**ツリーには残し、マークアップだけ出さない**ようにしています。
WebForms では表示されていないステップのコントロールも存在していたので、
`@ref` はそのまま割り当たります。

### `CreateUserWizard` は `Wizard` の別名ではありません — 測って外しました

`CreateUserWizard` / `CreateUserWizardStep` / `CompleteWizardStep` も `Wizard` /
`WizardStep` に寄せてみましたが、**BlogEngine だけでビルドエラーが 11 件**出ました。

```
'Wizard' に 'CreateUserStep' の定義が含まれていません
'Wizard' に 'ContinueDestinationPageUrl' の定義が含まれていません
RZ9996: Unrecognized child content inside component 'WizardStep'
```

アカウント作成ウィザードは**独自の API**(`CreateUserStep`、`UserName`、`Password`)を持ち、
マークアップも `WizardStep` が受け取れない名前付きテンプレートを入れ子にします。
**別のコンポーネントが要るのであって、別名では済みません。** 外しました。

### イベント属性を落としていた

最初の実装では `OnFinishButtonClick="Wizard_FinishButtonClick"` などが
「マッピング未定義」で除去されていました。**ステップは進むのにページが気づかない**
ウィザードになります(YAF のインストーラは処理を全部ハンドラでやっています)。
マッピングに足し、`[Parameter]` と `event` の両方から同じデリゲートに届くようにしました。

**総残差 691 → 679。** ビルドエラー不変、パリティ 30/30、回帰 13/13。

## アプリがコントロールと同じ名前を持つとき(2 か所で壊れていました)

会員制御の実装中に出てきた、**もっと一般的な欠陥**です。

### フィールドの型

BlogEngine には `Login.aspx` があり、ページクラスは `be.Components.Pages.Account.Login`。
生成されるフィールドが `protected Login Login1;` だと、**囲んでいる名前空間が
using より先に引かれる**ので、これはページ自身を指します。
`Login1.UserName` は当然ありません。

互換層の型は `global::WebForm2Blazor.Components.X` で出すようにしました。

### タグ

n2 には `Login.ascx` があり、コンポーネント名は `Login`。
**そのファイルの中の `<Login>` は自分自身に解決します。**
変換後のコントロールと名前が衝突するときだけ、タグを完全修飾します。

どちらも「アプリ自身の型が勝つ」規則の裏返しです。`TreeNode` や `MembershipProvider`
のときは**アプリの型が正解**でしたが、ここでは**互換層の型が正解**です
— 書いた人が `<asp:Login>` と書いたのだから、それは ASP.NET のコントロールです。

### `Context` はテンプレートが値を使うときだけ

`<LayoutTemplate Context="ItemsPlaceholder">` を常に出していました。
`Context` は `RenderFragment<T>` にしか付けられず、`Login` の `LayoutTemplate` は
ただの `RenderFragment` なので RZ9997 になります。
**プレースホルダを実際に差し替えたときだけ**出すようにしました。

## MEMBERSHIP_CONTROLS — 実装済み、マッピングは未接続

`Login` / `LoginStatus` / `LoginView` / `ChangePassword` はコンポーネントとして
実装しました。アプリ自身の `LayoutTemplate` を描画し(サイトのログインフォームは
そのテンプレートそのもので、今までページに一切届いていませんでした)、
`Authenticate` / `ChangingPassword` を発火します。**認証自体は行いません** —
membership は無く、変換後のアプリは ASP.NET Core で認証するからです。
自前のユーザーストアを `Authenticate` ハンドラで実装していたアプリは、そのまま動きます。

**マッピング表には繋いでいません。** 繋ぐと WingtipToys が回帰テスト中に
**プロセスごと終了**します。アプリ自身の `Debug.Fail`
(`AddToCart.Page_Load`「ProductId 無しでここに来るはずがない」)に到達するためで、
マスターページが以前より深くまで描画されるようになった結果です。

**それが元の挙動である可能性は高い**(元のアプリも同じ assert に当たります)のですが、
**死んだプロセスからスナップショットは何も言えません。** 効果を確認できない変更を
出さないために、ゲートがあります。次に着手する人は、まず wt のシナリオが
`/AddToCart` に直接行っている点から見てください。

## ページタイトルが前のページから持ち越されていた

`MEMBERSHIP_CONTROLS` を追ううちに出てきた、**全アプリに効く実挙動の差**です。

マスターを使うページに `Title` 属性が無いと、変換器は `<PageTitle>` を出しませんでした。
これは「指定なし」ではありません。**Blazor の `HeadOutlet` は `PageTitle` が描画された
ときだけタイトルを変えます** — 出さないと、**直前のページのタイトルが残ります。**
WingtipToys のエラーページは、どこから来たかによって "Welcome" だったり空だったりしました。

### マスターの書式にページ名を入れる

WingtipToys のマスターはこうです。

```aspx
<title><%: Page.Title %> - Wingtip Toys</title>
```

これを「動的なタイトルなので変換できない」として捨てていました。
実際には**サイト全体のタイトル書式**で、元のアプリは `Title="Welcome"` のページに
`Welcome - Wingtip Toys` と出します。

- ページの `Title` は**マスターの `Page.Title` の位置に入ります**
  (ページ側だけ取るとサイト名が消え、マスター側だけ取るとページ名が消えます)
- 他の式は Razor の式として残します

| | 変換前 | 変換後 |
|---|---|---|
| home | `Welcome` | **`Welcome - Wingtip Toys`** |
| error-page | `wt`(直前のページ次第で変動) | **`- Wingtip Toys`** |

## wt のシナリオが、アプリ自身が「不正」と宣言する状態を記録していた

`/AddToCart` を**クエリ無し**で開いていました。`AddToCart.aspx.cs` にはこうあります。

```csharp
Debug.Fail("ERROR : We should never get to AddToCart.aspx without a ProductId.");
throw new Exception("ERROR : It is illegal to load AddToCart.aspx without setting a ProductId.");
```

Debug ビルドの `Debug.Fail` は**プロセスを落とします。**
記録済みスナップショットが本文 0 行だったのは、**ページがそこまで到達していなかった**
からです。`?ProductID=1` を付けました。

## MEMBERSHIP_CONTROLS 完了 — ログイン導線がページに戻りました

前回「wt がプロセスごと落ちる」で外した会員制御を接続しました。落ちる原因は
シナリオ側(上記)で、それを直したうえで**さらに 2 つの欠陥**が出てきました。

### `System.Configuration.ConfigurationManager` が互換層を見ていなかった

```
System.NullReferenceException
   at WingtipToys.Models.ProductContext.get_ConnectionString()
```

```csharp
System.Configuration.ConfigurationManager.ConnectionStrings["WingtipToys"].ConnectionString
```

**完全修飾で書かれているので、実パッケージの方**に解決していました。あちらは
`App.config` を読みますが、変換器が接続文字列を書くのは `appsettings.json` です。
結果 `ConnectionStrings[...]` が null を返し、次の参照で落ちて**回路ごと停止**します。

`System.Web.X` と同じく互換層へ向けるようにしました。

### `<title>` が 1 つの文書に 2 つ出ていた

ホストページに静的な `<title>` があり、`HeadOutlet` も prerender 時に 1 つ出します。
**ブラウザは先頭を使う**ので、どのページも回路が立ち上がるまでプロジェクト名を
表示していました。静的な方を外し、**全ページが必ず `PageTitle` を出す**ようにしました
(自分の Title → マスターの書式 → サイトのタイトル → プロジェクト名)。
プレースホルダーページも自分の名前を出します。

### 結果(wt)

| | 変換前 | 変換後 |
|---|---|---|
| 本文 | ログイン導線なし | **`Register` / `Log in` が表示される** |
| home のタイトル | `Welcome` | `Welcome - Wingtip Toys` |
| product-list | `wt` | `ProductList` |

`Register` / `Log in` は元のアプリが匿名ユーザーに見せていたものです。
`LoginView` が未実装だったため、**マスターのその部分が丸ごと消えていました。**

## `CreateUserWizard` — 別名ではなく、`Wizard` の派生として

前回「別名では済まない」と測って外したものを、**独自コンポーネント**として入れました。

```csharp
public class CreateUserWizard : Wizard
```

描画は `Wizard` そのもの(ステップ・サイドバー・ナビゲーションは同じコントロールです)。
足すのは**ページが話しかける面**だけです — `CreateUserStep`、アカウント項目、イベント。

**アカウントは作りません。** membership は無く、変換後のアプリは ASP.NET Core で
認証します。`CreatingUser` / `CreatedUser` を発火し、判断はページに渡します。
BlogEngine の登録ページは処理(ロール付与、認証クッキー、リダイレクト)を全部
`CreatedUser` でやっているので、**そのまま動きます。**

### 途中で必要になったもの

| | |
|---|---|
| `<WizardSteps>` | マークアップはステップをこの要素で包みます。`WizardSteps` は**コレクション名でもある**ので、既存の `Columns → ColumnsContent` と同じ仕組みで `WizardStepsContent` に |
| `CustomNavigationTemplate` | そのステップのナビゲーションを差し替えます。アクティブなステップが持っていれば、`Wizard` は自分のボタンの代わりにそれを描画します |
| `ContentTemplateContainer` | BlogEngine は `CreateUserStep.ContentTemplateContainer.FindControl(...)` でステップ内のコントロールを探します。ステップ自身を返します |
| `CreateUserWizardStep` / `CompleteWizardStep` | **型として必要**でした。`.designer.cs` がこの名前でフィールドを宣言します(n2 の `Users/New.aspx.designer.cs`)。`WizardStep` の派生にしてあります |

**総残差 673 → 666。** ビルドエラー不変、パリティ 30/30、回帰 13/13。

## `ModelErrorMessage` / `Timer` / `PasswordRecovery`

| | |
|---|---|
| `ModelErrorMessage` | **モデルバインディングは対象外ですが、エラー側は別の話です。** コードビハインドは `ModelState.AddModelError` で「見せたいメッセージ」を入れます。WingtipToys はパスワード変更の失敗を全部これで報告していて、**表示するものが無かったので利用者には何も出ていませんでした。** `ModelStateDictionary` を `Page` に足し、このコントロールが表示します |
| `Timer` | 元はブラウザ側のインターバルでポストバックしていました。ここでは**回路が既に開いている**ので、サーバ側で tick して再描画をそこに流します。観測される挙動は同じ(ハンドラが周期実行されページが更新される)で、YAF のフォーラム削除画面はこれで進捗を追います |
| `PasswordRecovery` | `Login` / `ChangePassword` と同じ扱い。テンプレートがあればそれ、無ければ既定のフォーム。**パスワードは復旧しません** — membership は無く、勝手にリセットを作るのは何もしないより悪いので、`VerifyingUser` / `SendingMail` を発火してアプリに渡します |

`ModelErrorMessage` は最初 `CssClass` を落としていました(`text-danger`)。
共通属性を通すようにしています。

**総残差 666 → 662。** ビルドエラー不変、パリティ 30/30、回帰 13/13。

## `Calendar` — 描画するコンポーネントと、基底としての `LegacyCalendar`

`Calendar` は**マークアップのコントロールであると同時に、移植コードの基底**でもあります
(BlogEngine の `PostCalendar : Calendar` は自前で描画します)。
コンポーネントにすると、平のクラスは `ComponentBase` を継承できないので壊れます。

`Panel → LegacyPanel` と同じ手を使いました。

- 描画する `Calendar` は Blazor コンポーネント(月グリッド、タイトル行、曜日見出し)
- 基底に使われる方は `LegacyCalendar` に改名し、`CompatBaseReplacements` で
  `Calendar` → `LegacyCalendar` に差し替え

**これは 1 つ前のセッションで「やるな」と書かれていた変更です。**

> TreeView, Menu, MultiView and Calendar are declaration shims that ALREADY derive from
> LegacyWebControl, and redirecting those would throw away the members the shim carries

その指摘は正しく、**シムのメンバを捨てる形で**やれば退行でした。捨てずに
名前を変えて両方残せば、どちらも成立します(be は `PostCalendar` ごと不変、n2 −2)。

セルのクラスはマークアップが指定したものだけを付けます
(`OtherMonthDayStyle-CssClass`、`SelectedDayStyle-CssClass`)。
n2 の CalendarTeaser はこれだけで見た目を作っています。
`OnDayRender` も発火します — アプリが「予定のある日」に印を付ける場所です。

**総残差 662 → 660。** ビルドエラー不変、パリティ 30/30、回帰 13/13。

## `TreeView` — 未実装の標準コントロールが 0 件になりました

`Calendar` と同じ形(`mojoTreeView : TreeView`)なので、同じ手を使いました
— 描画する `TreeView` はコンポーネント、基底は `LegacyTreeView`。

WebForms はノードごとに `<table>` を描いていましたが、こちらは `<ul>`/`<li>` です。
**構造とリンクは同じ**で、出るものはノードが持っているものだけです
(`Text` / `NavigateUrl` / `Target` / `ToolTip` / `ImageUrl`)。
`DataSourceID` が `SiteMapDataSource` を指していれば `SiteMap` から組みますが、
**サイトマップの設定は Web.config にあり引き継いでいない**ので、根が無ければ
空のままにします(ノードを捏造するより空の方がましです)。

### 集計

**「標準コントロールですが互換コンポーネントが未実装です」は 34 件 → 0 件。**

| 実装したもの | 件数 |
|---|---:|
| Wizard / WizardStep / CreateUserWizard / 各ステップ | 17 |
| Login / LoginStatus / LoginView / ChangePassword / PasswordRecovery | 9 |
| MultiView / View | 3 |
| Calendar | 2 |
| ModelErrorMessage | 2 |
| TreeView / Timer | 2 |

いずれも**スタブ(HTML コメント)になっていた**もので、そのコントロールの UI は
ページから丸ごと消えていました。

---

## `<globalization culture>` を引き継ぐ — 全ページの日付・数値の書式

### 次に何を潰すか

「標準コントロール未実装」が 0 件になったので、残っている **実装待ち 94 件** を
原因別に数えました。

| 原因 | 件数 |
|---|---:|
| `BinaryFormatter` は .NET から削除 | 9 |
| **Web.config のセクション未変換** | **17** |
| 移植除外の型/名前空間への連鎖除外 | 14 |
| その他(長い尾) | 54 |

Web.config の 17 件の内訳は `httpModules` 4 / `customErrors` 4 /
`authentication` 3 / `httpHandlers` 2 / `sessionState` 2 / `globalization` 2。

このうち **`<globalization>` を先に選びました**。理由は影響範囲です。
`httpModules` や `customErrors` は特定の経路だけの話ですが、カルチャは
**アプリの全ページの、すべての日付と数値の文字列**を決めます。

```
be  : <globalization requestEncoding="utf-8" responseEncoding="utf-8" culture="auto" uiCulture="auto" />
yaf : <globalization culture="en-US" uiCulture="en" requestEncoding="UTF-8" ... />
```

引き継いでいなかったので、変換後のアプリは**そのサーバーの既定カルチャ**で
描いていました。つまり YAF は元アプリなら必ず `9/14/2026` と出るところが、
動かすマシン次第で `2026/09/14` にも `14.09.2026` にもなる。
**同じページが機械によって違う文字列を出す**という、変換器が一番やってはいけない
状態でした。

### 直したもの

1. `WebConfigConverter.CarryGlobalization` — `culture` / `uiCulture` を
   appsettings.json の `WebFormsGlobalization` セクションへ書き出す。
   どちらも無い(`enabled="true"` だけ等)なら従来どおり残差のまま。
   エンコーディングだけの UTF-8 指定を無視する既存の判定はそのまま前段に残す。
2. `GlobalizationExtensions.UseWebFormsGlobalization`(互換層・新規) —
   起動時にその設定を適用する。
3. `BlazorScaffolder` の `Program.cs` に `app.UseWebFormsGlobalization();` を
   `UseWebFormsSession()` の前に追加。セクションが無ければ何もしない。

### 固定カルチャと `auto` を区別している

`<globalization>` の値には二種類あります。

- **固定**(`en-US`)→ 4.8 では全リクエストスレッドがそのカルチャで走ります。
  なので `CultureInfo.DefaultThreadCurrentCulture` / `...UICulture` を立てます。
  ミドルウェアではなくプロセス既定にするのは、**移植した業務ロジックが
  バックグラウンドスレッドで書式化する場合**(タイマー、キャッシュ更新)にも
  4.8 では同じカルチャが効いていたからです。
- **`auto`**(`auto:en-US` 形式のフォールバック付きを含む)→ 4.8 は
  `Accept-Language` をリクエストごとに読みます。対応するのは
  `UseRequestLocalization` で、**Blazor Server ではサーキットが
  「そのサーキットを作ったリクエスト」のカルチャを引き継ぐ**ので、
  ここで交渉しておけば対話コンポーネントにも届きます。

`SupportedCultures` / `SupportedUICultures` は **null のまま**にしています。
既定の挙動(明示リストに無い言語は既定へ丸める)は 4.8 の `auto` がやらないこと
なので、リストを書くと**元アプリより言語が減る**からです。

`culture="auto" uiCulture="en"` のような片方だけ固定の組み合わせは、
交渉ミドルウェアの後段で固定側だけ戻しています。

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| be 総残差 | 73 | **72** |
| yaf 総残差 | 61 | **60** |
| 合計総残差 | 659 | **657** |

ビルドエラー 56 は変化なし。パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

回帰ゲートが緑のままなのは意味があります。**yaf は今回から en-US に固定された**
ので、もしこのマシンの既定が en-US でなければスナップショットが動いたはずです。
be は `auto` なので Playwright が送る `Accept-Language` で決まり、これも動きません。
つまり「固定した」こと自体は DOM を壊していません。

### n2 の `sv-SE` は引き継いでいない(これで正しい)

n2 の Web.config を grep すると `<globalization culture="sv-SE"/>` が出てきますが、
XML として読むと `system.web` 直下にあるのは `<globalization enabled="true" />` だけです。
**`sv-SE` の方はコメントの中**でした。`culture` も `uiCulture` も無いので
`CarryGlobalization` は false を返し、従来どおり残差として報告されます。
コメントを設定として読むのは変換器がやってはいけないことなので、これは期待どおりです。

---

## `System.Web.Routing` の除外をやめる — 20 ファイルの復活

### 除外の実態を数える

残差の「実装待ち」を原因別に並べた次に、**移植から除外された型**を数えました。

| コーパス | 空スタブになった型 |
|---|---:|
| be | 53 |
| mojo | 48 |
| yaf | 30 |
| dnn | 66 |
| n2 | 80 |
| wt | 10 |
| **合計** | **287** |

残差は 1 行でも、消えているのはアプリの中身です。除外理由を数えると:

| 理由(名前空間) | ファイル数 |
|---|---:|
| **System.Web.Routing** | **20**(dnn 10 / n2 8 / mojo 1 / wt 1) |
| System.Web.Services | 12(mojo) |
| ICSharpCode.SharpZipLib | 7 |
| System.Web.Helpers | 4 |
| System.Web.UI.Design | 5 |

最大の `System.Web.Routing` を潰しました。

### System.Web.Routing は「別フレームワーク」ではない

除外リストの但し書きは
「leaf-level infrastructure namespaces(OWIN、bundling、MVC/Web API、route config)」
でしたが、`System.Web.Routing` はそこに入る種類のものではありませんでした。
`System.Web.Compilation` を外したときと同じ誤りです。

コーパスでの実際の使われ方を数えると:

| 型 | 出現 |
|---|---:|
| RouteValueDictionary | 134 |
| RouteData | 127 |
| RequestContext | 95 |
| RouteCollection | 67 |
| Route | 59 |
| RouteTable | 22 |
| IRouteHandler | 12 |

**上位は全部データ入れ物**です。ディスパッチの話ではなく、
「ルート値から URL を組み立てる / パスからルート値を取り出す」ための道具として
使われています。だから互換層に**実装を持った型**を置きました
(`Compat/RoutingShims.cs`)。

- `RouteValueDictionary` — 大文字小文字を無視する `string→object`。匿名型からの
  構築(`new RouteValueDictionary(new { id = 3 })`)も対応。これがコーパスで
  一番多い書き方です。無いキーは例外ではなく null(System.Web と同じ)。
- `Route.GetRouteData` / `Match` — `{param}` と `{*catchAll}`、既定値、
  余分なセグメントの拒否まで実装。
- `Route.GetVirtualPath` — パターンに使われなかった値はクエリ文字列へ回す、という
  System.Web の挙動もそのまま。
- `RouteCollection` — `MapPageRoute` の 5 オーバーロード、`Ignore`、名前付き索引、
  `GetReadLock`/`GetWriteLock`(using ブロックが通るように)。
- `RouteTable.Routes`、`StopRoutingHandler`、`PageRouteHandler`、
  `IRouteConstraint`、`VirtualPathData`、`UrlRoutingModule`。

**やらないこと**は明確です。`IRouteHandler` は `IHttpHandler` を返し、
Blazor ではハンドラは走りません(変換後のページは自分の `@page` で到達します)。
つまり「ルートに URL を聞く」コードは正しい答えを得ますが、
「ルーティングモジュールがリクエストを捌く」ことは起きません。

### 付随して直したもの

`IDataBindingsAccessor` に `DataBindings` が無く、
DNN の `ImageParameter` が CS0539 で落ちました。
**明示的インターフェイス実装は、インターフェイスに宣言が無いとコンパイルエラー**です。
プロパティが一つ足りないのではなく、そのファイルごと壊れていました。

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| 移植 .cs 合計 | 5439 | **5459**(+20) |
| dnn 総残差 | 198 | **188** |
| n2 総残差 | 182 | **174** |
| wt 総残差 | 35 | **34** |
| 合計総残差 | 657 | **638** |
| ビルドエラー | 56 | **56**(変化なし) |

**ビルドエラーが増えていないのが要点です。** 20 ファイル戻して依存が解けない型が
出れば error が増えるはずで、増えていないということは、互換層の実装で足りていました
(唯一出た CS0539 が上の `IDataBindingsAccessor` で、それも直しました)。

パリティ 30/30、bUnit 30/30。

### 回帰ゲート: wt の product-list が変わった(改善)

```
NG   product-list
     ページタイトル: 期待 'ProductList' / 実際 'Products - Wingtip Toys'
     期待 'ProductList: ... System.Web.Routing に依存しているため自動変換できません。'
     実際 'Wingtip Toys' / 'Home' / 'About' / 'Contact' / 'Products'
     要素 'CategoryMenu' / 'Image1' / 'TitleContent' / 'cartCount': 変換前には無い要素
```

**変換不能プレースホルダだったページが、マスターもメニューもカートも付いた
実ページになりました。** 記録し直しています。

商品一覧そのものは "No data was returned." のままですが、これは
`product-details` も `shopping-cart` も同じで、WingtipToys の LocalDB が
この環境に無いためです(元アプリも DB 無しでは同じ)。変換器の欠陥ではないので
ここでは追いません。

---

## `System.Web.Services` の除外をやめる — と、そこで露出した 3 つの欠陥

除外理由の 2 番目、`System.Web.Services`(mojoPortal 12 ファイル)を潰しました。
そして**除外をやめたことで、隠れていた変換器の欠陥が 3 つ表に出ました**。
これは毎回起きることなので、順に記録します。

### 1. System.Web.Services はほぼ宣言だけ

コーパスでの使用実態:

| 型 | 出現 |
|---|---:|
| WebMethod | 15 |
| WebService | 13 |
| WsiProfiles | 12 |
| WebServiceBinding | 12 |
| ScriptService | 1 |

**基底クラス 1 個と属性 4 個**です。ASMX のエンドポイントは提供しません
(SOAP は別のプロトコル面で、`IHttpHandler` と同じ扱い)。
しかし `[WebMethod]` が付いたメソッドは**ただの public メソッド**で、
その周りのクラスはアプリの他の場所から普通に呼ばれます。
宣言 5 個のためにファイルごと捨てていました。

`Compat/WebServiceShims.cs` に `WebService` / `WebMethodAttribute` /
`WebServiceAttribute` / `WebServiceBindingAttribute` / `WsiProfiles` /
`ScriptServiceAttribute` / `ScriptMethodAttribute` を置き、
`System.Web.Services` / `System.Web.Services.Protocols` /
`System.Web.Script.Services` を書き換え対象の名前空間に追加しました。

### 2. 露出した欠陥 A: 属性値の中のタグを「壊れたタグ」と誤認していた(RZ9986)

DNN の InstallWizard が復活した結果、**生成 Razor** に RZ9986 が 5 件出ました。

```razor
<Label Text="<a class=&quot;videoLink&quot; href=&quot;...&quot;>Check DNN
  Comunity website@((global::...MarkupString)"</a>")" @ref="lblIntroDetail" />
```

属性値の中の `</a>` が Razor 式に書き換えられています。属性の中に式は置けないので
RZ9986(`Component attributes do not support complex content`)です。

原因は `TagBalance` のタグ正規表現でした。

```
<(?<close>/)?(?<name>[A-Za-z][A-Za-z0-9-]*)(?:\s[^>]*?)?(?<self>/)?>
```

`[^>]*?` は**どこにあろうと最初の `>` で止まります**。上の行では
`Text="..."` の中にある `<a ...>` の `>` がそれで、`<Label ...>` のマッチは
そこで終わる。結果、開始タグ `<a>` はマッチに飲み込まれて見えず、
**閉じタグ `</a>` だけが「対応する開きの無い迷子」**に見えていました。
`Neutralize` はそれを律儀に修理した、というだけです。
壊れていない場所に修理を当てて 3 ページを壊していました。

属性値を引用符ごと丸ごと食うようにしました:

```
<(?<close>/)?(?<name>[A-Za-z][A-Za-z0-9-]*)(?:\s(?:"[^"]*"|'[^']*'|[^>"'])*?)?(?<self>/)?>
```

#### 先に試して撤回した案(記録)

最初は「属性値の中身を空白でマスクしてから走査する」方式を書きました。
`inTag` 状態機械で `<` を見たらタグ開始とみなす、というものです。**大失敗**でした。

| | 変更前 | マスク方式 |
|---|---:|---:|
| be ビルドエラー | 0 | **2(しかも構文エラー)** |
| n2 ビルドエラー | 13 | **48** |
| 合計ビルドエラー | 64 | **101** |

理由は `<script>` の中の `if (a<b)` のような**タグでない `<`** をタグ開始と読み、
そこから次の引用符までを丸ごとマスクしてしまうからです。
元の正規表現は `[^>]*?` が `>` を跨げないぶん、こういうゴミに対しては安全でした。
**「より賢い解析」ではなく「元の挙動を保ったまま引用符だけ足す」方が正しい。**

### 3. 露出した欠陥 B: VB プロジェクトの型がビルドエラーに数えられていた

DNN の `Default.aspx.cs` と `InstallWizard.aspx.cs` が
`IClientAPICallbackEventHandler` と `DotNetNuke.UI.Utilities.DataCache` で
16 件のエラーを出しました。これらを宣言しているのは

```
<ProjectReference Include="..\DotNetNuke.WebUtility\DotNetNuke.WebUtility.vbproj" />
```

**VB.NET プロジェクト**です。この変換器は C# しか移植しないので、参照先の型は消えます。
しかも `<Reference>` ではないので**同梱 DLL 未決定の枠にも入らず**、
素の CS0246 として変換器の失点に数えられていました。

これはベンダー DLL とまったく同じ状況です(誰かが .NET ビルドを供給するまで
変換器には何もできない)。`.vbproj` / `.fsproj` への ProjectReference を
バイナリ依存として登録し、そのビルド済みアセンブリからメタデータで型名を読むようにしました。

探索範囲も直しました。DNN は `DNN Platform\DotNetNuke.WebUtility` にソースを置き、
出力は `DNN Platform\Controls\DotNetNuke.WebUtility\bin` にあります。
**プロジェクトの隣だけ見ても見つかりません**。親ディレクトリまで広げています。
どのコピーでもよい理由は、欲しいのは「消える型の名前」だけで、
どのビルドも同じ型を宣言しているからです。

### 4. 露出した欠陥 C: 完全修飾の属性短縮形が解決されなかった

```csharp
[System.Web.Services.WebMethod]
```

互換層の型名は `WebMethodAttribute` なので、`WebMethod` では表に無く、
**すぐ隣にある互換型に届かないまま**「存在しない名前空間」を指し続けていました。
`RewriteRootSystemWebTypes` に「書かれた名前 → 無ければ +Attribute」を足しました
(すでに `Attribute` で終わる名前には足しません。二重になって型を捏造します)。

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| mojo 移植 .cs | 741 | **753**(+12) |
| mojo 総残差 | 110 | **90** |
| mojo 変換可能 | 20 | **12** |
| dnn 総残差 | 188 | **186** |
| **dnn ビルドエラー** | 26 | **20** |
| n2 総残差 | 174 | **173** |
| 合計総残差 | 638 | **615** |
| 合計ビルドエラー | 56 | **50** |

12 ファイル戻したうえで **ビルドエラーは 56 → 50 と減りました**。
パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

### ここまでの累計(このセッション)

| | 開始時 | 現在 |
|---|---:|---:|
| 総残差 | 702 | **615** |
| ビルドエラー | 216 | **50** |
| 空スタブ化された型 | 287 | 255 |

---

## 未対応コントロール 113 → 71 — 手書きリストが 2 つあって食い違っていた

### まず「なぜ紐付かないのか」を出力させた

残差を区分別に数え直すと、**入力待ち 121 件の大半(113 件)が
「未対応コントロールです。--control-map で指定して下さい」**でした。

| コーパス | 未対応コントロール |
|---|---:|
| dnn | 39 |
| n2 | 35 |
| mojo | 24 |
| yaf | 13 |
| be / wt | 各 1 |

ところが調べ始めると、`<n2:Zone>` の `N2.Web.UI.WebControls.Zone` も
`<YAF:LocalizedRequiredFieldValidator>` も**ソースは移植済み**でした。
1 件ずつ当てるのは効率が悪いので、**残差に理由を書かせました**。
`ResolveLegacyControl` は今まで `null` を返すだけで、4 種類の別々の失敗が
全部「--control-map で指定して下さい」という同じ答えになっていました。

| 理由 | 件数 |
|---|---:|
| **D: 基底の根が描画対象外** | **69** |
| C: テンプレート子要素あり(移植済み) | 23 |
| B: 型が移植対象に無い | 19 |
| A: タグ接頭辞が未登録 | 0 |

D の内訳:`DataSourceControl` 15、`TemplateColumn` 11、
`RequiredFieldValidator` 10、`RegularExpressionValidator` 6、
`TreeView` 3、`HtmlGenericControl` 3、`DataList` 3、`FileUpload` 2、
`HtmlForm` 2、`Login` 1、`GridView` 1、`Calendar` 1 …

### 原因: 同じことを決める手書きリストが 2 つあった

- `AspxConverters.LegacyRenderableRoots` — その基底の根なら LegacyRenderHost で描く
- `CodeBehindRewriter.ControlBaseNames` — その基底なら `LegacyWebControl` に書き換える

**この 2 つは一致していなければ意味がありません。**
`ControlBaseNames` に `GridView` / `DataList` / `Calendar` / `TreeView` /
`HtmlGenericControl` / `BaseValidator` があるのに `LegacyRenderableRoots` には無い。
つまり「基底は `LegacyWebControl` に書き換えたのに、描画対象とは認めない」。
**書き換えた側が正しく、認めなかった側が間違っていました。**

`LegacyRenderableRoots` のコメント自身が
「grown one entry at a time and had gaps that cost more than they look」
と書いていたのに、同じ過ちをもう一つ隣で繰り返していた形です。

### 直したもの

1. `ControlBaseNames` を**互換アセンブリから導出**。
   「互換層が `WebFormsControlBase`(コンポーネント)か `LegacyWebControl`
   (プレーンクラス)として宣言している型名」がコントロール基底です。
   `Page` / `MasterPage` / `UserControl` は子コントロールではないので除外。
   手書きリストは和集合として残しています(`CompositeControl` など
   互換層に無い元側の名前が入っているため)。
2. `LegacyRenderableRoots` を廃止し、
   `IsLegacyRenderableRoot(root)` = `ResolveControlBase(root) is not null`
   に。**問いは一つ「移植後のクラスは結局 LegacyWebControl を継ぐのか」だけ**なので、
   それを決めている側に聞きます。

`TemplateColumn` / `TemplateField` が除外されたままなのは正しい判定です。
あれは DataGrid/GridView の**列**であってコントロールではなく、
LegacyRenderHost で単独描画するものではありません(互換層も
`LegacyWebControl` 派生として宣言していない)。実体に聞いた結果、
手書きでは間違えやすいこの区別が自動的に付きました。

### 露出した欠陥 D: LegacyWebControl.Page が常に null だった

回帰ゲートが BlogEngine で落ちました。

```
NG   home
     実測側にのみある行: '[App_Code.Controls.PostCalendar: render error]'
```

`PostCalendar` は `Calendar` 派生で、いままでスタブ(不可視)だったものが
LegacyRenderHost に載った結果、描画時に落ちました。原因は 2 つ。

**(1) `LegacyWebControl.Page => null` の決め打ち。**
`PostCalendar` は `OnLoad` で `Page.ClientScript`、`OnPreRender` と `Render` で
`Page.IsPostBack` / `Page.IsCallback` を読みます。**ごく普通の WebForms コード**で、
それが必ず NullReferenceException になっていました。
ホストしている `LegacyRenderHost` はページを知っているので、
ライフサイクルを回す前に設定するようにしました
(何もホストしていないとき — 単体テスト等 — は null のままが正直な答えです)。

**(2) マークアップ属性の型変換が `Convert.ChangeType` だけだった。**
エラーの実体は `Invalid cast from 'System.String' to 'Unit'` でした。
`Convert.ChangeType` は `IConvertible` しか扱えないので、`Width="90%"` のような
**属性 1 個でコントロール全体が「[render error]」**になります。
WebForms はマークアップ属性を `TypeConverter` で解釈していたので、そちらを先に使い、
`T(string)` コンストラクタ、`Convert.ChangeType` の順にフォールバックします。

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| **未対応コントロール残差** | **113** | **71** |
| be 総残差 | 72 | 71 |
| mojo 総残差 | 90 | **79** |
| yaf 総残差 | 60 | **49** |
| dnn 総残差 | 186 | 185 |
| n2 総残差 | 173 | **155** |
| 合計総残差 | 615 | **573** |
| ビルドエラー | 50 | **50**(変化なし) |

パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

BlogEngine のカレンダーは描画エラーが消えて回帰も一致しましたが、
**出力は空のまま**です。`LegacyCalendar` 自体が月表を描かないためで、
スタブだったときと同じ(悪化はしていない)。別件として残します。

### 残り 71 件の内訳

| 理由 | 件数 |
|---|---:|
| C: テンプレート子要素あり(移植済み) | 28 |
| B: 型が移植対象に無い(バイナリのみ) | 19 |
| D: TemplateColumn / TemplateField(列であってコントロールでない) | 13 |
| D: その他(ClientDependency 系など) | 11 |

---

## `<HeaderTemplate>` / `<FooterTemplate>` は LegacyRenderHost を止める理由にならない

残り 71 件の最大クラスタ「C: テンプレート子要素あり(移植済み)」28 件の中身:

| コントロール | 件数 | 子要素 |
|---|---:|---|
| `n2:Zone` | 9 | `HeaderTemplate` / `FooterTemplate` |
| `dnn:DnnFormEditor` | 7 | `<Items>`(子コントロール宣言) |
| `n2:EditableDisplay` | 4 | |
| `mojo:mojoDataList` | 3 | `ItemTemplate` |
| `dnn:DnnComboBox` | 3 | `<Items><asp:ListItem>` |
| その他 | 2 | |

**`ItemTemplate` と `HeaderTemplate` を同じ扱いにしていたのが間違い**でした。

- `ItemTemplate` は**データ項目ごとに実体化**されます。レガシーコントロールは
  `HtmlTextWriter` に書くだけなので、行ごとに Razor フラグメントを呼び戻せません。
  これはホストできません。
- `HeaderTemplate` / `FooterTemplate` は**コントロールの出力の前後に置かれる静的マークアップ**で、
  データ項目もバインド式もありません。Blazor がそのまま描けます。

`LegacyRenderHost` に `HeaderContent` / `FooterContent` を足し、
変換器はその 2 つだけを名前付きフラグメントとして渡すようにしました
(`Items` / `Columns` / `ItemTemplate` は従来どおりスタブ)。

### ヘッダとフッタは「2 つで 1 つ」だった

最初の実装で n2 のビルドエラーが **13 → 56** に跳ねました。

```
RZ9981 Unexpected closing tag 'div' with no matching start tag.   16
RZ9980 Unclosed tag 'div' with no matching end tag.                7
RZ1026 Encountered end tag "FooterContent" with no matching start tag.  3
```

n2 の Zone はこう書かれています。

```aspx
<HeaderTemplate><div class="list"></HeaderTemplate>
<FooterTemplate></div></FooterTemplate>
```

**`<div>` を開くのがヘッダ、閉じるのがフッタ**です。2 つ合わせれば対称ですが、
Razor は**フラグメントを別々に解析する**ので、片方ずつ見ると必ず壊れています。
既存の `TagBalance.Neutralize` を各フラグメントに適用して、
それぞれ生出力(`MarkupString`)に落としました。ブラウザが組む DOM は同じです。

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| n2 総残差 | 155 | **150** |
| 合計総残差 | 573 | **568** |
| ビルドエラー | 50 | **50**(変化なし) |

パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

### このセッションの累計

| | 開始時 | 現在 |
|---|---:|---:|
| 総残差 | 702 | **568**(−19%) |
| ビルドエラー | 216 | **50**(−77%) |
| 未対応コントロール | 113 | **66** |
| 標準コントロール未実装 | 34 | **0** |

---

## `<customErrors>` を引き継ぐ — 例外時に元アプリと同じページへ

### 先に試して撤回した案: ClientDependency の除外(記録)

ビルドエラー 50 件の内訳を見ると、dnn の 20 件のうち **16 件が
`Dnn.ClientDependency`**(net45 のみ、`WebFormsFileRegistrationProvider` が
`System.Web.UI.Control` を取る)の統合コードでした。
`DotNetNuke.Web.Client.Providers` はそのライブラリのラッパーそのものなので、
`BlogML` と同じ扱い(Framework 専用ライブラリ)にしてみました。

| | 変更前 | ClientDependency 除外 |
|---|---:|---:|
| ビルドエラー | 50 | **46**(−4) |
| 合計総残差 | 568 | **591**(+23) |
| dnn 移植 .cs | 2049 | **2030**(−19) |

**ビルドエラーは 4 しか減らず、残差が 23 増えて 19 ファイル消えました。**
連鎖除外の損が勝ちます(`PortabilityRules` の但し書きどおり)。撤回しました。
16 件は「ユーザーが .NET ビルドを供給するまで動かせない外部ライブラリ」のままです。

### customErrors: WebForms とまったく違う画面が出ていた

4.8 では未処理例外は `defaultRedirect` へ飛びます。
Blazor Server では同じ例外が**サーキットごと落とし**、訪問者が見るのは
フレームワークの「An unhandled error has occurred. Reload」バーです。
どのコーパスも自前のエラーページを持っているのに、それが一度も出ていませんでした。

| | 設定 |
|---|---|
| be | `mode="RemoteOnly" defaultRedirect="~/error.aspx"` + `<error statusCode="404">` |
| yaf | `mode="RemoteOnly" defaultRedirect="Error.aspx"` |
| n2 | `mode="RemoteOnly" defaultRedirect="~/Templates/UI/Views/500.aspx"` |
| wt | `mode="Off"` |

1. `WebConfigConverter.CarryCustomErrors` — `mode` / `defaultRedirect` /
   `<error statusCode redirect>` を `WebFormsCustomErrors` セクションへ。
2. `WebFormsErrorBoundary`(互換層・新規) — ルーティングされたコンポーネントで
   例外を受け、設定されたページへ遷移。`.aspx` は落とします
   (変換後のページは自分の `@page` で到達するため)。
3. `mode` は WebForms の定義どおり:
   `On` = 常に / `Off` = 出さない / `RemoteOnly` = **クライアントアドレスが
   ループバックでないときだけ**。ホスティング環境ではなくアドレスで決めるのが 4.8 の意味です。

### `mode="Off"` は「未変換」ではなく「変換不要」

wt の `<customErrors mode="Off"/>` は「カスタムエラーページを持たない」という
**明示的な選択**です。「UseExceptionHandler への書き換えが必要」と報告するのは、
挙動を保つどころか変えろと言っているのと同じでした。Info に変更。

### 露出した欠陥: 「何も設定が無いのに境界を置く」のは中立ではない

最初は Routes.razor に無条件で境界を入れ、設定が無ければ再スローする実装にしました。
回帰ゲートが wt で落ちました。

```
NG   add-to-cart
     ページタイトル: 期待 'wt' / 実際 ''
```

`OnErrorAsync` からの再スローは、**例外をそのまま通した場合とサーキットの
落ち方が違います**。「捕まえて投げ直す」は「最初から捕まえない」と同じではない。

なので、**Web.config がエラーページを指定しているときだけ境界を出す**ようにしました
(`WebConfigConverter.HasCustomErrorPage` をスキャフォールド前に問い合わせ)。
何も変えてはいけないコンポーネントは、出さないのが一番確実です。

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| be 総残差 | 71 | **70** |
| yaf 総残差 | 49 | **48** |
| n2 総残差 | 150 | **149** |
| wt 総残差 | 34 | **33** |
| 合計総残差 | 568 | **564** |
| ビルドエラー | 50 | **50**(変化なし) |

パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。
生成物を確認: wt の Routes.razor には境界が無く、be には有ります。

---

## 基底連鎖の解決が 2 箇所で間違っていた

前節で残差に理由を書かせたおかげで、残り 66 件のうち「D: 基底の根が描画対象外」を
根の名前ごとに数えられるようになりました。

| コントロール | 根 |
|---|---|
| `n2:Tree` | **Page** |
| `YAF:Forum` | **IEntity** |
| `n2:Repeater` | 不明 |

`n2:Tree` の根が `Page`、`YAF:Forum` の根が**インターフェイス**。どちらもおかしい。

### 欠陥 1: 宣言表が短名キーだった

`BaseClassRegistry.Build` は

```csharp
declarations.TryAdd(DeclarationKey(classDeclaration.Identifier.Text, arity), ...)
```

と**クラスの短名**でキーを作り、`TryAdd` なので**先に読まれたファイルが勝ち**ます。
n2 には `N2.Web.UI.WebControls.Tree`(コントロール)と
`N2.Edit.Web.UI.Controls.Tree`(Page)の両方があり、後者が前者を隠していました。
`ResolveLegacyControl` は完全名で型を特定しておきながら、
基底連鎖だけ短名で引き直していた(`fullName[(lastIndexOf('.')+1)..]`)ので、
**別のクラスの基底を見て**判定していたことになります。

完全名キーの表を併せて持ち、`GetRootBaseNameOf(fullName)` で辿るようにしました。

### 欠陥 2: 基底リストの先頭がインターフェイスでも基底クラス扱い

```csharp
var baseName = classDeclaration.BaseList?.Types.FirstOrDefault()?.Type.ToString();
```

C# では基底クラスがあるときだけ先頭が基底クラスです。無ければ先頭はインターフェイス。
YAF の `Forum : IEntity, ...` は基底クラスを持たないので、根が `IEntity` になっていました。

**これは同じセッションで一度踏んだ罠です**(`CodeBehindRewriter` の override 判定で
`ICallbackEventHandler` を基底と読んで正しい override を 2 つ落とした件)。
片方を直したときにもう片方を直さなかったので、別の場所で同じ形で出ました。

スキャン中に宣言されたインターフェイス名を集め、先頭エントリがそれなら
「基底クラス無し」として記録します。インターフェイスはクラスより後のファイルで
宣言されていることがあるので、**全ファイル読了後に**後処理します。

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| n2 総残差 | 149 | **148** |
| 合計総残差 | 564 | **563** |
| ビルドエラー | 50 | **50** |

件数は 1 件ですが、直したのは**判定そのものの誤り**です。
`YAF:Forum` の根は `IEntity` から `UserControl` に変わりました
(ユーザーコントロールなので LegacyRenderHost の対象外という判定は正しい)。

### 残り「D」23 件は正しい除外

| 根 | 件数 | 判断 |
|---|---:|---|
| `TemplateColumn` / `TemplateField` | 13 | DataGrid/GridView の**列**。コントロールではない |
| `ClientDependencyPath` / `JsInclude` / `CssInclude` / `ClientDependencyLoader` | 6 | 外部ライブラリ(.NET ビルド無し) |
| `UserControl` | 2 | .ascx なのでユーザーコントロール経路 |
| 不明 | 2 | n2 の `Repeater`(基底リスト無し) |

パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

---

## n2 のビルドエラー 8 件は元リポジトリの欠落

n2 の 13 件のうち 8 件はこれです。

```
CS0234 型または名前空間の名前 'Fragmenters' が名前空間 'N2.Addons.Wiki' に存在しません   6
CS0246 型または名前空間の名前 'RegexFragmenter' が見つかりませんでした                  2
```

`Addons/Wiki/WikiParser.cs` が `using N2.Addons.Wiki.Fragmenters;` と
`public void Add(RegexFragmenter fragment)` を書いていますが、

- リポジトリ全体で `namespace N2.Addons.Wiki.Fragmenters` を宣言するファイルは**ゼロ**
- `RegexFragmenter` を宣言するファイルも**ゼロ**(参照しているのは WikiParser.cs だけ)
- `.csproj` にも `Fragmenters` の記載なし

**元のアプリケーションもコンパイルできません。** 変換器に直せるものではないので、
件数には残したまま記録に留めます(除外すれば数字は下がりますが、
消えるのは WikiParser であって、問題ではありません)。

---

## 名前空間で登録されたユーザーコントロールが解決されていなかった

「B: 型が移植対象に無い」19 件を一覧にしたところ、mojoPortal の 5 件が異質でした。

```
<portal:TimeZoneIdSetting>   (×3)
<portal:AllowedRolesSetting>
<portal:CurrencySetting>
```

これらは **`Web/Controls/*.ascx` として存在し、変換もされています**
(`corpora/out/mojo/Components/Controls/Controls/AllowedRolesSetting.razor`)。

```
Web.config: <add tagPrefix="portal" namespace="mojoPortal.Web.UI" assembly="mojoPortal.Web" />
AllowedRolesSetting.ascx: <%@ Control ... Inherits="mojoPortal.Web.UI.AllowedRolesSetting" %>
SiteSettings.aspx: <portal:TimeZoneIdSetting ID="timeZone" runat="server" />  ← Register 無し
```

**ユーザーコントロールは `src` でしか登録されない、という前提が間違っていました。**
WebForms は「接頭辞に紐づいた名前空間にそのクラスがある」でも解決します。
mojoPortal はこれらに `src` 登録も `<%@ Register %>` も一切書かず、
名前空間登録だけに頼っています。

`BuildUserControlTags` は `src` 付きの登録しか見ていなかったので、
出力ディレクトリに .razor が在るのに「移植対象に入っていない」と報告していました。

`UserControlRef` に `.ascx` の `Inherits`(元のクラス完全名)を持たせ、
接頭辞に登録された名前空間と突き合わせて `prefix:ClassName` を引けるようにしました。
**`src` 登録は後から上書き**します — ファイルを名指しする方が、
候補が複数ありうる名前空間より具体的だからです。

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| mojo 総残差 | 79 | **74** |
| 合計総残差 | 563 | **558** |
| 未対応コントロール | 66 | **60** |
| ビルドエラー | 50 | **50** |

パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

---

## `<SeparatorTemplate>` は「前後」では置けない — ITemplate として渡す

前節で `HeaderTemplate` / `FooterTemplate` を Razor フラグメントとして渡したとき、
n2 の Zone は 9 件しか解決しませんでした。残り 8 件を見ると:

```aspx
<n2:Zone ID="Zone0" ZoneName="AutoZone1" runat="server">
    <HeaderTemplate><fieldset></HeaderTemplate>
    <SeparatorTemplate></fieldset><fieldset></SeparatorTemplate>
    <FooterTemplate></fieldset></FooterTemplate>
</n2:Zone>
```

**`<SeparatorTemplate>` は項目の「間」に入ります。** ホスト側には「前」と「後」しかなく、
「間」を知っているのは項目を回しているコントロール自身だけです。
つまり `HeaderContent` / `FooterContent` という発想そのものが、
セパレータには原理的に届きません。

### WebForms と同じ形で渡す

n2 の Zone は元から `ITemplate SeparatorTemplate { get; set; }` を持っています。
**WebForms が使っていた形をそのまま使えばいい**だけでした。

- `StaticMarkupTemplate : ITemplate`(互換層・新規)— 固定マークアップを保持し、
  `InstantiateIn` でコントロールの `Controls` に `RawMarkupControl` を足す。
- `LegacyRenderHost` は `TemplateMarkup`(テンプレート名 → 生マークアップ)を受け取り、
  **その型が同名の `ITemplate` プロパティを持つときだけ**設定します。
- 持たないときは、`HeaderTemplate` / `FooterTemplate` に限り従来のフラグメントが描画します
  (この 2 つには意味のある「前」「後」があるため)。
  コントロールがテンプレートを受け取った場合はフラグメント側を抑止するので、二重に出ません。

**静的マークアップだけ**がこの経路を通ります。`<%# %>` を含むもの、
サーバーコントロールを含むものは、項目をスコープに入れて実体化する必要があり、
文字列では運べません(従来どおりスタブ)。

### 露出した欠陥: Razor は属性の C# 式の中でもタグを探す

最初の実装で n2 のビルドエラーが 13 → 19 になりました。

```
RZ9980 Unclosed tag 'div' with no matching end tag.   6
```

出力はこうなっていました。

```razor
TemplateMarkup="@(new Dictionary<string,string> { ["HeaderTemplate"] = @"<div id=""textContent"">" })"
```

**逐語的文字列リテラルでは足りません。** Razor は属性の C# 式の中もタグ開始として
走査するので、`@"<div ...>"` が「閉じられていない div」になります。
`<` を `<` にエスケープしました — `TagBalance.Neutralize` が既に使っている答えで、
コンパイラが作る文字列は同一です。

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| n2 総残差 | 148 | **140** |
| 合計総残差 | 558 | **550** |
| C(テンプレート子要素で弾かれた) | 23 | **15** |
| ビルドエラー | 50 | **50** |

パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

残り 15 件は `dnn:DnnFormEditor` 7 / `dnn:DnnComboBox` 3 /
`portal:mojoDataList` 3 / その他 2 で、いずれも `<Items>`(子コントロール宣言)か
`<ItemTemplate>`(データ項目ごとの実体化)です。どちらも文字列では運べません。

---

## `<Items>` は入れ子のコントロールではなくコレクションの中身

テンプレートで弾かれていた残り 15 件の中身:

```aspx
<dnn:DnnComboBox ID="modeList" runat="server" AutoPostBack="true">
    <Items>
        <asp:ListItem Value="Normal" Text="Normal" ResourceKey="Normal" />
    </Items>
</dnn:DnnComboBox>

<dnn:DnnFormEditor id="authenticationForm" runat="Server" FormMode="Short">
    <Items>
        <dnn:DnnFormTextBoxItem ID="authenticationType" runat="server" DataField="AuthenticationType" />
    </Items>
</dnn:DnnFormEditor>
```

`<Items>` は**ページが描く入れ子コントロールではありません**。
親コントロールが自分のリストに持ち、自分で描く**エントリ**です。
WebForms のパーサがそれを組み立てていました。

- `LegacyChild(Collection, TypeName, Properties)`(互換層・新規)— 宣言の記述。
- `LegacyRenderHost` が型を作り、属性を(コントロール本体と同じ型変換で)設定し、
  一致するプロパティのコレクションに `Add` します。
  コレクションはコンストラクタで作られ private setter で公開されているのが普通なので
  (`public List<DnnFormItemBase> Items { get; private set; }`)、
  **差し替えではなく読んで足します**。
- 子の型解決には `ResolveAnyType` を新設しました。
  エントリに「LegacyRenderHost で単独描画できるか」を聞くのは筋違いで、答えは常に no です。
  `asp:` は互換層の型名、それ以外は接頭辞の名前空間から移植済みクラスを引きます。

**エントリが一つでも解決できなければホストしません。** 半分欠けたコレクションは、
「静かに間違ったコントロール」になります。目に見えて欠けているスタブの方がましです。

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| dnn 総残差 | 185 | **175** |
| 合計総残差 | 550 | **540** |
| C(テンプレート子要素で弾かれた) | 15 | **5** |
| ビルドエラー | 50 | **50** |

パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

残り 5 件は `portal:mojoDataList` 3 / `dnnweb:DnnGrid` 1 / `portal:SiteLogin` 1。
いずれも `<ItemTemplate>`(データ項目ごとの実体化)で、文字列でも型名でも運べません。

### このセッションの累計

| | 開始時 | 現在 |
|---|---:|---:|
| 総残差 | 702 | **540**(−23%) |
| ビルドエラー | 216 | **50**(−77%) |
| 未対応コントロール | 113 | **50** |
| 標準コントロール未実装 | 34 | **0** |

---

## 基底が自分と同じ短名のとき、クラスが自分自身の基底に見えていた

未対応コントロール 42 件の中に `root=不明` が 2 件(`n2:Repeater`)残っていました。

```csharp
// 元
public class Repeater : System.Web.UI.WebControls.Repeater
// 移植後
public class Repeater : LegacyWebControl
```

**WebForms でごく普通の書き方**です(自作コントロールを同名で被せる)。
ところが `BaseClassRegistry` の基底連鎖は完全名で始めたあと、
上の階層は短名で辿っていました。基底 `System.Web.UI.WebControls.Repeater` の
短名は `Repeater` — つまり**そのクラス自身**。循環検出に引っかかって
「根は不明」になっていました。

基底が**完全修飾で書かれている**場合は、完全名で引き直すようにしました。
スキャン済みのクラスに無ければ、そこで連鎖は外部に出たということなので、
最後のセグメントを根として返します。

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| n2 総残差 | 140 | **139** |
| 合計総残差 | 540 | **539** |
| ビルドエラー | 50 | **50** |

パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

### 残り 41 件の未対応コントロールは「直せないもの」が大半

| 分類 | 件数 | 判断 |
|---|---:|---|
| DataGrid の列(`dnn:textcolumn` 等) | 13 | 下記 |
| 外部ライブラリのバイナリ(ZedGraph / DotNetNuke.WebControls / ClientDependency) | 15 | ユーザーが .NET ビルドを供給 |
| `<ItemTemplate>`(データ項目ごとの実体化) | 5 | 文字列でも型名でも運べない |
| `UserControl` 基底 / 名前空間未登録 / その他 | 8 | |

**DataGrid の列 13 件について。** `dnn:textcolumn` の実体は

```csharp
public class TextColumn : TemplateColumn
{
    public override void Initialize()
    {
        this.ItemTemplate = this.CreateTemplate(ListItemType.Item);
        this.HeaderTemplate = this.CreateTemplate(ListItemType.Header);
    }
}
```

で、**WebForms の列プロトコル**(`Initialize()` が `ITemplate` を組み、
グリッドが行ごとにセルへ実体化する)に乗っています。
忠実に動かすなら互換 `DataGrid` がこのプロトコルを実装する必要があり、
`TemplateColumn.Initialize` / `ItemTemplate` / `TableCell` の子コントロール描画まで
連動します。**「名前から BoundField に読み替える」のは推測**であって、
`imagecommandcolumn` のような列では間違った描画になります。
規模が大きいので独立した作業として残します。

---

## 次の作業: DataGrid の列プロトコル(設計を確定させた)

未対応コントロール 41 件の最大塊は **DataGrid の列 13 件**です
(`dnn:textcolumn` 5 / `dnn:imagecommandcolumn` 4 / `dnn:checkboxcolumn` 2 /
`dnnweb:DnnGridTemplateColumn` 2)。実装は次回に回しますが、
**何を作ればよいかは調べ切った**ので、ここに残します。

### なぜ「名前で読み替える」ではいけないか

`dnn:textcolumn` を `BoundField` に読み替えれば数字は 5 減ります。しかしそれは推測です。
同じ `<columns>` に並ぶ `dnn:imagecommandcolumn` は画像ボタンを描く列で、
同じ読み替えをすれば**間違った列が出ます**。
アプリ固有の知識を汎用変換器に焼き込むことにもなります。

### 元のコードが要求しているもの

```csharp
public class TextColumn : TemplateColumn                    // DNN
{
    public override void Initialize()
    {
        this.ItemTemplate   = this.CreateTemplate(ListItemType.Item);
        this.HeaderTemplate = this.CreateTemplate(ListItemType.Header);
    }
}

public class TextColumnTemplate : ITemplate
{
    public void InstantiateIn(Control container)
    {
        lblText.DataBinding += this.Item_DataBinding;        // ← イベント
        container.Controls.Add(lblText);
    }
    private void Item_DataBinding(object sender, EventArgs e)
    {
        var container = (DataGridItem)lblText.NamingContainer;   // ← NamingContainer
        lblText.Text = DataBinder.Eval(container.DataItem, this.DataField).ToString();
    }
}
```

つまり **WebForms の列プロトコルそのもの**です。
列が `ITemplate` を組み、グリッドが行ごとに `DataGridItem` を作って
セルへ実体化し、`DataBinding` を発火させる。

### 互換層に既にあるもの / 無いもの

| 部品 | 状態 |
|---|---|
| `DataGridItem : LegacyWebControl`(`DataItem` 付き) | **有り**(`UiDeclarationShims.cs:227`) |
| `DataBinder.Eval` | **有り**(`Runtime/DataBinder.cs`) |
| `ITemplate` / `StaticMarkupTemplate` | **有り** |
| `GridView` の `List<DataControlField> Columns` | **有り**(`BoundField` / `TemplateField` が `IColumnContainer` 経由で登録) |
| `TableCell` | **無し** |
| `LegacyWebControl.DataBinding` イベント / `NamingContainer` | **無し** |
| 互換 `TemplateColumn` の `ItemTemplate` / `HeaderTemplate` / `Initialize()` | **無し**(今は空クラス) |
| 移植済み列を `DataControlField` として包むアダプタ | **無し** |

### 作るもの(4 点)

1. `TableCell : LegacyWebControl` — `Controls` を描画するセル。
2. `LegacyWebControl` に `DataBinding` イベントと `NamingContainer`
   (LegacyRenderHost / 親コントロールが設定する)。
3. 互換 `TemplateColumn` に `ITemplate ItemTemplate/HeaderTemplate/FooterTemplate/
   EditItemTemplate`、`virtual void Initialize()`、`HeaderText`、`ItemStyle`/`HeaderStyle`。
4. `LegacyColumnField : DataControlField` — 移植済み列オブジェクトを包み、
   `Initialize()` を呼び、行ごとに `DataGridItem` + `TableCell` を作って
   `ItemTemplate.InstantiateIn` し、`DataBinding` を発火させて HTML を取り出す。

変換器側は既存の `LegacyChild` / `CollectionChildren` の仕組みをそのまま使えます
(`<Columns>` は既に `LegacyCollectionElements` に入っている)。
`GridView` に `LegacyColumns` パラメータを足して `_columns` へ足すだけです。

**この順で作れば、DNN 自身のコードが自分の列を描きます。** 変換器が推測する必要はありません。

---

## 列プロトコル(1/2): 互換 `TemplateColumn` を WebForms の型に揃えた

設計の 4 点のうち、まず土台の 2 つを入れました。

### `ItemTemplate` が `RenderFragment` だった

```csharp
// 互換層(変更前)
public class TemplateColumn
{
    public RenderFragment ItemTemplate { get; set; }   // ← Blazor の型
}

// 移植された DNN
public override void Initialize()
    => this.ItemTemplate = this.CreateTemplate(ListItemType.Item);  // ← ITemplate を返す
```

**移植された列クラスが絶対に満たせない形**でした。WebForms の `TemplateColumn` は
`ITemplate` なので、そちらに合わせます。
宣言的な `<asp:TemplateColumn>` はここに来ません — 変換器はそれを `TemplateField`
(コンポーネント)に割り当てるので、`RenderFragment` のままです。

あわせて `Initialize()`(既定は何もしない)、`ItemStyle` / `HeaderStyle` / `FooterStyle`、
`FooterText` を追加しました。移植された列が `Initialize` の中で触るものです。

### `LegacyTableCell`

WebForms の列は**マークアップを返すのではなくセルを埋めます**
(テンプレートの `InstantiateIn` がセルに Label を足し、グリッドがセルを描く)。
`TableCell` は既にありましたが Blazor コンポーネントで `RenderFragment` を取るため、
移植された列からは使えません。`LegacyPanel` / `Panel` と同じ分け方で
`LegacyTableCell : LegacyWebControl` を追加しました。

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| dnn 総残差 | 175 | **171** |
| 合計総残差 | 539 | **535** |
| ビルドエラー | 50 | **50**(変化なし) |

パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

dnn の残差が 4 減ったのは、移植された列クラスのメンバーが
**正しく解決されるようになった**ぶんです(型が合わないために落としていた宣言が通った)。

### 残り(2/2)

`LegacyColumnField : DataControlField` アダプタと `GridView.LegacyColumns`。
これで移植済みの列オブジェクトを `Initialize()` し、行ごとに
`DataGridItem` + `LegacyTableCell` を作って `ItemTemplate.InstantiateIn` し、
`DataBinding` を発火させて HTML を取り出します。
変換器側は `LegacyChild` / `<Columns>` の仕組みが既にあるので追加不要です。

### 2/2 に着手する前に確かめたこと

アダプタを書く前の最大の不安は「**セルに足されるのは互換 `Label`(Blazor コンポーネント)で、
それをレガシーセルが `HtmlTextWriter` に描けるのか**」でした。答えは描けます。

```csharp
// Runtime/WebFormsHostCore.cs — IWebFormsControl の既定実装
void RenderControl(HtmlTextWriter writer) { }        // 既定は何も書かない
// Runtime/WebFormsControlBase.cs:245 — コンポーネントはこれを override する
public virtual void RenderControl(HtmlTextWriter writer)
```

つまり `LegacyTableCell.RenderChildren` → `Label.RenderControl(writer)` の経路は成立します。
**アダプタはこの前提で書けます。**

未確認が 1 つ残っています。DNN のテンプレートは

```csharp
var container = (DataGridItem)lblText.NamingContainer;
```

と書きますが、`NamingContainer` は `IWebFormsControl` にも `LegacyWebControl` にも
見当たりません(`WebFormsControlBase` にあるのは `NamingContainerPrefix`)。
それでも `TextColumnTemplate.cs` はビルドエラーに出ていないので、
**どこで解決されているのかを先に突き止める必要があります**
(見つからなければ、設定可能な `NamingContainer` を `LegacyWebControl` に足すのが 3 点目の作業)。

---

## 列プロトコル(2/2): 移植済みの列が自分でセルを描く

### まず `NamingContainer` の謎を決着させた

前節の未確認事項でした。互換層を全文検索しても `NamingContainer` というメンバーは
**存在しません**。それなのに DNN の `TextColumnTemplate.cs`(`lblText.NamingContainer` を書いている)は
ビルドエラーに出ていませんでした。

推測で進めるのをやめ、**足してビルドする**という決着の付け方をしました。
重複なら `CS0102` が既存の宣言位置を教えてくれます。結果は**ビルド成功** —
つまり本当に無かったということです
(dnn の生ビルドは 406 の CS エラーを出しており、そのうち 386 がベンダー DLL /
System.Web としてゲートで除外されています。その中に埋もれていたと考えられます)。

これは方法としても正しかったと思います。「見つからないから無いはず」より、
**コンパイラに聞く**方が速くて確実でした。

### 作ったもの

| | |
|---|---|
| `WebFormsControlBase.NamingContainer` / `LegacyWebControl.NamingContainer` | 所属する命名コンテナ(オブジェクト)。`NamingContainerPrefix`(連結済み文字列)とは別物 |
| 両基底の `DataBinding` イベント と `DataBind()` | `OnDataBinding` が実際に発火するように。**宣言だけでは意味がありません** |
| `LegacyActivator` | 宣言(型名 + 属性)からオブジェクトを組む。`TypeConverter` → `T(string)` → `Convert.ChangeType` の順 |
| `LegacyColumnField : DataControlField` | 移植済み列をグリッドの列として見せる。`Initialize()` を呼び、行ごとに `DataGridItem` + `LegacyTableCell` を作り、`ItemTemplate.InstantiateIn` → `DataBinding` 発火 → セルの HTML を取り出す |
| `GridView.LegacyColumns` | 宣言を受け取り、**マークアップ順を保って**先頭から挿入 |
| `MarkupEmitter.TakeLegacyColumns` | `<Columns>` の未マップ列を取り出して宣言に変換し、**木から取り除く**(スタブ経路が二重に報告しないように) |

**これは列プロトコルの翻訳ではなく、プロトコルそのものです。** DNN の
`TextColumn` / `ImageCommandColumn` / `CheckBoxColumn` は全部これに乗っているので、
動かせば **DNN 自身のコードが DNN の列を描きます**。
変換器は「textcolumn とは何か」を知る必要がありません。

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| dnn 総残差 | 171 | **158** |
| 合計総残差 | 535 | **522** |
| **未対応コントロール** | **41** | **28** |
| ビルドエラー | 50 | **50**(変化なし) |

パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。
減った 13 件は列そのもの(`dnn:textcolumn` 5 / `imagecommandcolumn` 4 /
`checkboxcolumn` 2 / `dnnweb:DnnGridTemplateColumn` 2)です。

### 残っている重複(次に片付ける)

`LegacyRenderHost` の `ConvertMarkupValue` / `SetProperty` / `ResolveType` は
`LegacyActivator` と同じことをしています。**同じことを決める実装が 2 つある**のは
このセッションで何度も痛い目を見た形なので、`LegacyRenderHost` を
`LegacyActivator` に委譲させます。

### このセッションの累計

| | 開始時 | 現在 |
|---|---:|---:|
| 総残差 | 702 | **522**(−26%) |
| ビルドエラー | 216 | **50**(−77%) |
| 未対応コントロール | 113 | **28** |
| 標準コントロール未実装 | 34 | **0** |

---

## 重複の解消 — と、それに伴う 1 つの挙動変更(明示)

`LegacyRenderHost` が持っていた `SetProperty` / `ConvertMarkupValue` / `ResolveType` は、
`LegacyActivator` とまったく同じ規則でした。**同じことを決める実装が 2 つある**のは
このセッションで一番高くついた形です
(描画可能な基底クラスの手書きリストが 2 つあって食い違い、未対応コントロール 69 件)。
`LegacyRenderHost` を委譲に直しました。

### 挙動が 1 つ変わっています

2 つの実装は 1 点だけ違っていました。**変換できない属性が 1 つあったときどうするか**です。

| | 変更前 | 変更後 |
|---|---|---|
| `LegacyRenderHost` | 例外が伝播し、**コントロール全体が `[render error]`** | 属性 1 つを飛ばして描画 |
| `LegacyColumnField` | 属性 1 つを飛ばして描画 | 同じ |

寛容な側に揃えました。理由は**元アプリに近いのはどちらか**です。
WebForms は変換できない属性を**パース時**に弾くので、実行時にその状況は起きません。
起きてしまった場合、「他の属性はそのままにコントロールを描く」方が
「コントロールごとエラー表示に差し替える」より元の画面に近い。

`Width="90%"` 1 つで BlogEngine の PostCalendar 全体が消えていた件が、まさにこの形でした。

### 計測

すべて据え置き(総残差 522 / ビルドエラー 50)。
パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。
**数字が動かないのが正しい変更**です(同じ規則を 1 箇所にまとめただけなので)。

---

## `ICSharpCode` は Framework 専用ではなかった

除外の原因を名前空間別に数え直すと、最大は `ICSharpCode.SharpZipLib.Zip` の **7 ファイル**でした
(be / dnn / n2)。除外リストでの分類は

```
// Framework-only third-party libraries with no .NET build
"Microsoft.Ajax", "BlogML", "ICSharpCode",
```

でしたが、**SharpZipLib は 1.0(2018)から netstandard2.0 を出しており、
今は .NET を直接ターゲットしています**。「.NET ビルドが無い」は事実ではありません。
除外をやめ、`KnownPackages` に `ICSharpCode.SharpZipLib → SharpZipLib 1.4.*` を足して、
移植コードが**本物のパッケージに解決される**ようにしました。

### 露出した欠陥: `OpenWebConfiguration` が null を返していた

9 ファイル戻した結果、be のビルドエラーが 0 → 6 になりました。
**SharpZipLib のエラーではありません。**

```csharp
var config = WebConfigurationManager.OpenWebConfiguration("~");
var section = (BlogFileSystemProviderSection)config.GetSection("BlogEngine/blogFileSystemProvider");
section.DefaultProvider = NewProviderName;
config.Save();
ConfigurationManager.RefreshSection("BlogEngine/blogFileSystemProvider");
```

`OpenWebConfiguration` は `object` の `null` を返していたので、
`config.GetSection(...)` がコンパイルエラー。`RefreshSection` も未宣言でした。

- `Configuration` 型を追加(`GetSection` / `AppSettings` / `ConnectionStrings` / `FilePath`)。
- `ConfigurationManager.RefreshSection` は**本物へ転送**します。
  セクションの出所は変換器が引き継いだ App.config で、キャッシュしているのもそれなので、
  ported code が求めているものと一致します。

### `Save()` は「何もしない」ではなく例外にしました

ここは判断が要る箇所なので明記します。BlogEngine はこのコードで
**ファイルシステムプロバイダを切り替えます**。`Save()` を無言の no-op にすると、
画面上は成功したように見えて設定は元のまま — 3 つの選択肢のうち最悪です。

| 選択肢 | 評価 |
|---|---|
| 無言の no-op | **最悪**。成功したように見えて変わらない |
| 実際に書き戻す | 変換後アプリの設定がどこに住むかの決定が要る(別作業) |
| 呼び出し箇所で例外 | **採用**。壊れている場所がその場で分かる |

除外型スタブが採っているのと同じ方針です
(「a stub that does not compile helps no one / methods throw」)。

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| be 移植 .cs | 255 | **257** |
| dnn 移植 .cs | 2049 | **2053** |
| n2 移植 .cs | 1676 | **1679** |
| be 総残差 | 70 | **68** |
| dnn 総残差 | 158 | **154** |
| n2 総残差 | 139 | **136** |
| 合計総残差 | 522 | **513** |
| ビルドエラー | 50 | **50**(変化なし) |

パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。
**9 ファイル戻してビルドエラーが増えていない**のが要点です。

---

## 同じ罠を 2 度目 — 連鎖除外も文字列リテラルを読んでいた

連鎖除外 22 件の「根」を数えると:

| 根 | 件数 |
|---|---:|
| n2 `ControlPanel` / `Tree` / `Link` ほか | 12 |
| **be `BlogEngine.Core.Compilation.Design.*ExpressionEditor`** | **4** |
| dnn MVC 系(範囲外) | 3 |
| その他 | 3 |

be の 4 件を見に行くと、依存の実体はこれだけでした。

```csharp
[ExpressionEditor("BlogEngine.Core.Compilation.Design.CodeExpressionEditor, BlogEngine.Core")]
public class CodeExpressionBuilder : ExpressionBuilder
```

**文字列リテラルです。** しかもこの属性が型名を文字列で書いているのは、
**参照していないから**です(デザイナ専用の型を実行時に読み込ませないための書き方)。
それを「依存」と読んで 4 ファイルを捨てていました。

`UsesGoneType` は素の `source.Contains(...)` でした。
`PortabilityRules.FindQualifiedFrameworkReference` は**まったく同じ誤りを既に直して**います
(DNN の `Reflection.CreateType("System.Data.Linq.DataContext", true)` で
1 ファイル除外 → 連鎖して百件近く、という記録がこの README にあります)。
片方を直したときにもう片方を直さなかったので、別の場所で同じ形が残っていました。

**このセッションで同じ構図は 3 度目です**:
- 基底リストの先頭がインターフェイス(override 判定 → 基底連鎖)
- 描画可能な基底の手書きリストが 2 つ
- 文字列リテラルを依存と読む(移植性判定 → 連鎖除外)

いずれも「片方で学んだことを、同じことをしている別の場所に適用していなかった」だけです。
`WithoutStringsAndComments` を `internal` にして共有しました。

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| be 移植 .cs | 257 | **261** |
| dnn 移植 .cs | 2053 | **2055** |
| be 総残差 | 68 | **64** |
| dnn 総残差 | 154 | **152** |
| 合計総残差 | 513 | **507** |
| **ビルドエラー** | 50 | **49** |

パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。
6 ファイル戻してビルドエラーが**減った**のは、戻ったファイルが
他のファイルの未解決参照を埋めたからです。

### このセッションの累計

| | 開始時 | 現在 |
|---|---:|---:|
| 総残差 | 702 | **507**(−28%) |
| ビルドエラー | 216 | **49**(−77%) |
| 未対応コントロール | 113 | **28** |
| 標準コントロール未実装 | 34 | **0** |

---

## 撤回: 「import だけでは依存ではない」を out-of-scope フレームワークにも適用してみた

n2 の連鎖除外 12 件の根を辿ると、`N2/Web/Link.cs` と `N2/Web/Tree.cs` が
**「ASP.NET MVC のコードです」として除外**されていました。中身を見ると:

```csharp
using System.Web.UI;
using System.Web.Mvc;          // ← これがある
...
public class Link : System.Web.IHtmlString   // ← MVC の型は 1 つも使っていない
```

`System.Web.IHtmlString` は `System.Web` であって MVC ではありません。
**使っていない using 1 行**で、n2 のコア型 2 つとその依存 12 ファイルが落ちていました。

`FindUnportableNamespace` は `using` を見るだけでした。
一方、20 行離れた連鎖除外のコメントにはこう書いてあります。

> An import alone is not a dependency, and treating it as one cascades hard.

**同じ教訓の 4 箇所目**です。そこで「名前空間から実際に何かを名指ししている場合だけ除外」に
変えてみました(修飾形 `System.Web.Mvc.X` と短縮形 `Mvc.X` を証拠とする)。

### 結果: ビルドエラー 49 → 1206

| | 変更前 | import 規則の緩和後 |
|---|---:|---:|
| 合計総残差 | 507 | 279 |
| **合計ビルドエラー** | **49** | **1206** |
| be | 0 | 192 |
| mojo | 13 | 152 |
| yaf | 4 | 144 |
| dnn | 19 | 481 |
| n2 | 13 | 230 |

残差が 279 まで下がっていますが**無意味です**。ファイルは移植されただけで、
コンパイルが通っていません。

**原因は証拠の取り方でした。** MVC / Identity / OWIN のコードは、その型を
**非修飾で**使います(`ActionResult`、`HtmlHelper`、`IAppBuilder`)。
「修飾名が出てくるか」では検出できないので、本物の MVC ファイルが素通りしました。

非修飾の識別子を証拠に使う案は、この README に既に記録があります
(`UsesGoneType` の注記 — 3 通り試して毎回暴走、1 件のエラーが 28 件、次に 705 件)。
**同じ理由でここでも使えません。**

### 学んだこと

「import だけでは依存ではない」は正しい原則ですが、**それを適用するには
「使っている」を判定できる必要があります**。

- 連鎖除外では判定できます。**消えた名前空間の型名を変換器が知っている**からです。
- out-of-scope フレームワークでは判定できません。`System.Web.Mvc` の型一覧を
  変換器は持っていません(実行中のフレームワークには存在しない)。

つまり「4 箇所目」ではなく、**前 3 つとは別の問題**でした。
n2 の 2 ファイルを救うには、MVC の型集合を知る手段(参照アセンブリのメタデータを読む等)が要ります。
現状維持とし、n2 の 12 件は残します。

---

## yaf のビルドエラー 4 → 1(目的: 3 つ目のコーパスを回帰ゲートに載せる)

残差の内訳はもう長い尾で、大きな塊がありません。方針を変えました。
**回帰ゲートは be と wt しか見ていません。** mojo / yaf / n2 / dnn は一度も起動していない、
つまり「元アプリと同じ動きをする」の検証が 6 分の 2 しかかかっていない状態です。

ビルドが通らなければ起動できません。**yaf は残り 4 件**で一番近いので、そこを潰しました。

### 直した 2 件

**(1) `using System.Runtime.Remoting.Contexts;` を落とす**

YAF の `HelpMenu.cs` はこの import を持っていますが**何も使っていません**。
.NET に remoting は存在しないので、**import 自体が CS0234** です。
落としました。本当に remoting 型を使っているファイルは、
「名前空間が無い」より具体的な CS0246(その型が無い)になります。
`System.Runtime.Remoting.Messaging` だけは例外です(互換層が `CallContext` を宣言しており、移植コードが使います)。

**(2) `.NET 10 が追加した型と同梱ライブラリの衝突**

```
error CS0104: 'OrderedDictionary<,>' は、'J2N.Collections.Generic.OrderedDictionary<TKey, TValue>' と
              'System.Collections.Generic.OrderedDictionary<TKey, TValue>' 間のあいまいな参照です
```

.NET 10 が `System.Collections.Generic.OrderedDictionary<,>` を追加し、
`System.Collections.Generic` は暗黙 using です。YAF が同梱する J2N には同名同アリティの型があり、
Lucene.Net のコードは `using J2N.Collections.Generic;` でそれを使っていました。
**書かれた当時は正しく、誰も触っていないコードが壊れた**形です。

別名では直せません。**C# には開いたジェネリックの別名がありません。**
書き下すしかなく、勝つべきは「そのコードが意図した方」です。

最初は「移植済み型索引にある方を勝たせる」と書きましたが**発火しませんでした** —
J2N は同梱ソースではなく**パッケージ**なので索引に無いからです。
判定根拠を変えました:

> **暗黙 using**(プロジェクト全体に効く、そのファイルが選んでいない)で名前が来ていて、
> そのファイルが**明示的に import している**名前空間が、暗黙側と
> **先頭セグメント以降が一致する**なら、明示側が意図された方。

`J2N.Collections.Generic` と `System.Collections.Generic` のように、
BCL を再実装するライブラリは**意図して名前空間の末尾を揃えます**。その末尾が、
型が対応しているという宣言になっています。

### 撤回した 1 件(記録)

残る 1 件は `DbProviderFactory.CreatePermission` の override です
(Code Access Security とともに .NET から削除されたメンバー)。
override 削除パスは「互換層の基底」と「アプリ自身の基底」しか判定せず、
**フレームワークの基底は素通り**していました。実行中プロセスに居るので聞けるはず、と実装したところ:

| | 変更前 | フレームワーク基底も判定 |
|---|---:|---:|
| 合計総残差 | 507 | **580** |
| 合計ビルドエラー | 46 | **65** |

**両方悪化**。基底は非修飾で書かれるので、ファイルの `System.*` import を舐めて
**単純名で**解決することになり、無関係な BCL 型を基底と誤認して正しい override を落としました。
「単純名で基底を照合する」は、このファイルが何度も焼かれてきた罠そのものです
(`MembershipProvider`、`Page<TPage>`、`Tree`)。

フレームワーク基底を判定するには、基底が**名前一致ではなく本当に解決されている**必要があります。
セマンティックモデルを持ち込む話になるので、別作業として残します。

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| yaf ビルドエラー | 4 | **1** |
| 合計ビルドエラー | 49 | **46** |
| 合計総残差 | 507 | 507 |

パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

---

## セマンティックモデルで基底を解決する(実装済み・未配線)

「単純名で基底を照合する」を根本から直すため、`SemanticBaseIndex` を実装しました。

```csharp
// 移植前の元ソース全部を CSharpCompilation に入れ、この変換器が動いている
// フレームワークの参照アセンブリ(TRUSTED_PLATFORM_ASSEMBLIES)を付ける。
// あとは C# 自身の名前解決に任せる — 囲っている名前空間が先、次に using。
public bool? BaseDeclaresMember(string metadataName, string memberName)
```

**`bool?` にしたのが肝**です。`null` =「基底連鎖を最後まで解決できなかった」。
これらのアプリは大半が `System.Web` の型を継いでおり、それはここに存在しません。
**連鎖が object まで全部解決できたときだけ** `false`(= メンバは本当に無い)を返し、
override を落とすのはその場合だけにしました。

### 結果: yaf が 1 → 2135 ビルドエラー

落ちた override は**ちょうど 1 つ**、しかも**正しいもの**でした。

```
ProfiledProviderFactory.CreatePermission の override を外しました
```

`DbProviderFactory.CreatePermission` は Code Access Security とともに .NET から削除された
メンバーで、これは間違いなく CS0115 になる override です。生成結果も正しい:

```csharp
public virtual System.Security.CodeAccessPermission CreatePermission(PermissionState state) =>
    WrappedFactory.CreatePermission(state);
```

ところがビルドエラーは **1 → 2135**。しかも中身が無関係です。

```
CS1061 'BoardContext' に 'BoardSettings' の定義が含まれておらず…   1138
CS1929 'BoardContext' に 'Get' の定義が含まれておらず…              476
CS1501 引数 1 を指定するメソッド 'Eval' のオーバーロードはありません      202
```

**`override` を 1 つ外しただけで、これらが出る筋道がまだ分かっていません。**
切り分けは済んでいます(`Semantics = null` にすると 1 に戻る)ので、原因は確かに
この経路ですが、機序は不明です。仮説:

- `BoardContext` は出力に `BoardContext.cs` として存在しない。
  除外型スタブか partial の片割れの可能性がある。
- その場合、原因は override 削除そのものではなく、
  **セマンティック索引を作る過程で何かが変わっている**(全ソースの再パース、
  参照アセンブリの読み込み等)。

### 配線しないことにしました

**正しいエラー 1 件を、原因不明のエラー 2135 件と引き換えにはできません。**
`SemanticBaseIndex` はコードとして残し、`Program.cs` では `Semantics = null` にしてあります。
配線を戻すのは 1 行です。

次にやること:
1. `BoardContext` が出力のどこで宣言されているかを特定する
   (スタブか、partial か、別名のファイルか)。
2. `Semantics` を作るだけで(使わずに)変換して、数値が動くかを見る。
   動けば原因は索引の構築、動かなければ override 削除の下流。

数値はベースラインどおり(総残差 507 / ビルドエラー 46)、
パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

### 切り分けの結果(確定した事実)

前節の「機序が不明」を 3 回の実験で詰めました。

| 実験 | yaf ビルドエラー | 結論 |
|---|---:|---|
| `Semantics = null`(既定) | 1 | 基準 |
| **索引を作るだけで使わない** | **1** | **索引の構築は無実** |
| 索引を使う(override を 1 つ落とす) | 2135 | **override 削除そのものが原因** |

つまり `ProfiledProviderFactory.CreatePermission` の `override` を外すこと自体が
2135 件を生んでいます。生成された当該メソッドは正しく、しかも:

- `BoardContext` は**除外型スタブ**で、`BoardSettings` は**元から宣言されていません**。
  それでも `Semantics = null` のとき生のビルドエラーは **2 行(実質 1 件)**しかなく、
  `BoardContext.Current.BoardSettings` を使う `PageLinkExtensions.cs` は通っています。
- `UserPageBase` は基底も実装インターフェイスも持たない `abstract class` です。

**この 2 つは両立しないはず**で、そこが未解明の核心です。
`Semantics = null` のときに `BoardSettings` がどう解決されているのかが分かれば、
なぜ無関係な override 削除でそれが壊れるのかも分かるはずです。

### 試して外した案

メンバーが `#if` ブロックにあることに着目し
「条件付きコンパイル領域のメンバーは書き換えない」(`member.ContainsDirectives`)を入れましたが、
**効きませんでした**(yaf 2135 のまま、mojo が 13→14 に悪化)。
`#endif` はメンバーの内側ではなく**外側**にあるため、`ContainsDirectives` は false です。
撤回しました。

### 現状

`SemanticBaseIndex` はコードとして残し、**未配線**(`Semantics = null`)。
数値はベースラインどおり(総残差 507 / ビルドエラー 46)。

次に確かめること: `Semantics = null` のビルドで `BoardContext.Current.BoardSettings` が
**どの宣言に解決されているのか**。`dotnet build` に `/p:EmitCompilerGeneratedFiles` か、
出力プロジェクトを Roslyn で開いて `SemanticModel` に聞くのが確実です。
それが分かるまで配線しません。

---

## yaf の「ビルドエラー 1 件」は事実ではなかった

前節の「機序が不明」は、**そもそも回帰ではありませんでした。**

### 生のビルドを見たら分かった

| | 生の `error CS` 行数 |
|---|---:|
| `override` が付いたまま | **2** |
| `override` を外した後 | **4382** |

**Blazor プロジェクトは 2 回コンパイルされます。** 宣言だけのパスが先に走り、
そのあと生成された .razor のコードを含む本番のパスが走ります。
`CS0115`(オーバーライドする適切なメソッドが見つかりません)は**シグネチャのエラー**なので、
`ProfiledProviderFactory.CreatePermission` で**宣言パスが落ち、
メソッド本体を束縛するパスには一度も到達していませんでした**。

つまり yaf の「1 件」は**下限値**で、2135 件は最初からそこにありました。
override を外したことで**隠れなくなっただけ**です。

`BoardContext` が除外スタブで `BoardSettings` を持たないのに
`PageLinkExtensions.cs` が通っていた、という辻褄の合わなさも、これで説明がつきます。
**通っていたのではなく、検査されていなかった。**

### `SemanticBaseIndex` は最初から正しかった

見つけた override はちょうど 1 つ、`DbProviderFactory.CreatePermission` —
Code Access Security とともに .NET から削除されたメンバーです。配線しました。

### 本当に直すべきだったのは計測の方

`BuildVerifier.StoppedAtDeclarations` を追加しました。
**エラーが全てシグネチャ段階のコードだけなら、件数を下限値として報告します**
(構文エラーで意味解析が走らなかった場合と同じ扱い)。

```csharp
"CS0115", "CS0534", "CS0507", "CS0533", "CS0106", "CS0111", "CS0101",
"CS0509", "CS0549", "CS0238", "CS0539", "CS0736", "CS0738",
```

**静かに「途中で止まりました」を意味する小さい数字は、大きい数字より悪い。**
このセッションで「ビルドエラーが減った」を何度も根拠にしてきたので、
その根拠自体に穴があったことになります。

### 計測(ベースライン更新)

| | 変更前 | 変更後 |
|---|---:|---:|
| yaf ビルドエラー | 1(下限値) | **2135**(実数) |
| 合計ビルドエラー | 46 | **2180** |
| 合計総残差 | 507 | 508 |

他のコーパスは全て不変。パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

**数字は悪化して見えますが、悪化していません。** 見えていなかったものが見えただけです。
yaf を回帰ゲートに載せる目標は遠のきましたが、距離が正しく測れるようになりました。

---

## 依存されている「別フレームワーク」ファイルは移植する — yaf 2135 → 2

`BuildVerifier` が正直になった結果、yaf の本当の姿が見えました。2135 件のうち

| | 件数 |
|---|---:|
| `'BoardContext' に 'BoardSettings' の定義が含まれておらず` | 1138 |
| `'BoardContext' に 'Get' の定義が含まれておらず` | 476 |

**76% が 1 つの型**です。`BoardContext` は除外型スタブで、除外の根はこれでした。

```
YAF.Core/Context/BoardContext.cs      <= 除外済みの型 YAF.Types.Models.Identity.AspNetUsers に依存
YAF.Types/Models/Identity/AspNetUsers.cs <= ASP.NET Identity のコードです
```

`AspNetUsers` は**データモデル(テーブルの POCO)**で、たまたま
`Microsoft.AspNet.Identity` の `IUser<TKey>` を実装しているだけです。
それを「Identity のコード」として捨て、`BoardContext` を道連れにし、
アプリの半分が設定を読めなくなっていました。

`PortabilityRules` 自身の但し書きがこう言っています。

> an exclusion cascade produces far more damage than the local errors do

**除外はローカルエラーを避けるための道具です。**
消す相手が「移植されるファイルから使われている型」なら、道具として機能しておらず、
連鎖を始めているだけです。そこで:

> **別フレームワークとして除外したファイルの型を、生き残るファイルが名指ししているなら、
> そのファイルは移植する。**

移植すれば、そのファイルの中に未解決の型が数個残ります。
除外すれば、それに依存する全部が落ちます。

### 2 つの境界線を測って決めた

**(1) 「別フレームワーク」と「API ごと消えた名前空間」は違う**

最初は除外理由を区別せずに復帰させ、**be が 0 → 55** になりました。
復帰したのは `System.Data.Linq`(LINQ to SQL)のファイルで、
**全行が存在しない API でできています**。別フレームワークのファイルは数個で済みますが、
こちらは全滅です。`OutOfScopeFrameworkNote` が既に両者を別の文面で報告していたので、
その区別をそのまま使いました。

**(2) しきい値 2 は間違い**

「1 ファイルから参照されるだけなら、どちらにせよローカルエラーだから復帰不要」と考えて
2 以上にしたところ、**yaf が 2135 に戻りました**。
`AspNetUsers` を直接使う生存ファイルは `BoardContext` **ただ 1 つ**で、
そこから先が巨大なのです。**効くのは直接参照数ではなく連鎖の大きさ**でした。1 に戻しました。

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| **yaf ビルドエラー** | **2135** | **2** |
| n2 ビルドエラー | 13 | 32 |
| n2 移植 .cs | 1679 | **1698**(+19) |
| n2 総残差 | 136 | **133** |
| be / mojo / dnn / wt | 変化なし | 変化なし |
| **合計ビルドエラー** | **2180** | **66** |
| 合計総残差 | 507 | **505** |

yaf に残る 2 件は、復帰した `AspNetUsers` の中の
`Microsoft.AspNet` と `IUser<>` — **原則が約束したとおりの「数個のローカルエラー」**です。

n2 は 19 ファイル戻って +19 エラー(MVC の `HtmlHelper` 等)。
これも同じ形で、残差は 3 件減っています。

パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

---

## Identity の「契約」だけを宣言する — そして 3 度目の「隠れていた数字」

### `IUser<TKey>` は振る舞いを持たない

yaf に残った 2 件は、復帰した `AspNetUsers` の中の
`using Microsoft.AspNet.Identity;` と `IUser<TKey>` でした。

ASP.NET Identity は範囲外です(ログインは移行後に ASP.NET Core がやる仕事で、
互換層は `Membership.ValidateUser` を false のままにして**動くふりをしない**方針)。
しかし範囲内のものが 1 つあります —— **アプリ自身のデータモデル**です。
`AspNetUsers` はテーブルの POCO で、`IUser<TKey>` は
**`Id` と `UserName` の 2 つだけ、振る舞いゼロ**のインターフェイスです。

`Compat/IdentityContractShims.cs` に `IUser<TKey>` / `IUser` / `IRole<TKey>` / `IRole` の
**契約だけ**を宣言しました。`UserManager` もパスワードハッシュもサインインもありません。
モデルは自分のインターフェイスを名乗れるようになり、
実際に認証しようとするコードは**行き先が無いまま**です
——これが未移行のログインの正直な状態です。

### 3 度目の「小さい数字は途中で止まっていただけ」

yaf が **2 → 838** になりました。また同じです。
`CS0234` / `CS0246`(型・名前空間が見つからない)も**宣言パスで出るエラー**で、
そこで止まると本体のコンパイルには到達しません。
`StoppedAtDeclarations` に入れた符号は `CS0115` 系だけだったので、今回は警告が出ませんでした。

**このセッションで「ビルドエラーが減った」を根拠にした判断が複数あります。**
その根拠は、少なくとも yaf については 3 回とも下限値でした。

### `this.Eval("X")` は `Eval("X")` と同じ

838 件の内訳を見ると 203 件がこれでした。

```
error CS1501: 引数 1 を指定するメソッド 'Eval' のオーバーロードはありません
        @(this.Eval( "Name" ))
```

データバインド式の書き換えは

```
(?<!\.)\bEval\s*\(      ← 直前のドットを全部拒否する
```

で、`DataBinder.Eval(Container.DataItem, "X")`(長い書き方)を守るための否定先読みでした。
ところがこれは **`this.Eval(` も拒否します**。YAF は一貫してこの書き方をします。
**明示的な `this.` はスタイルの選択であって、別の呼び出しではありません。**

```
(?<![\w.])(?:this\.)?Eval\s*\(
```

に変え、`this.` ごと `Eval(Container, ` に置き換えるようにしました。

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| yaf ビルドエラー | 2(下限値) | **598**(実数) |
| 合計ビルドエラー | 66 | 662 |

内訳としては **838 → 598**(`Eval` で −240)。
数字は増えて見えますが、2 は嘘で、838 が実数、そこから 240 減らしたということです。

パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

### yaf に残る 598 件の最大塊

```
CS1061 'HtmlGenericControl' に 'PostedFile' の定義が含まれておらず   281
```

`<input type="file" runat="server">` を `HtmlGenericControl` として出しているためです。
WebForms はこれを `HtmlInputFile` にします(`PostedFile` を持つのはそちら)。
互換層の `HtmlInputFile` は `LegacyWebControl` のプレーンクラスなので、
`@ref` で結ぶにはコンポーネント化が要ります。次の作業です。

---

## `<input type="file">` と、復帰規則の証拠の強さ

### HtmlInputFile は FileUpload と同じコントロール

yaf の 281 件は `<input type="file" runat="server">` を
`HtmlGenericControl` として出していたためでした。WebForms はこれを `HtmlInputFile` にします
(`PostedFile` を持つのはそちら)。

互換層には既に `FileUpload`(コンポーネント、`PostedFile` は `HttpPostedFileShim`)と
`HtmlInputFile`(`LegacyWebControl`、`PostedFile` は `object` の null)の**両方**がありました。
**この 2 つは同じコントロールです** — WebForms はどちらも `<input type="file">` として描き、
どちらもアップロードされたファイルを `PostedFile` で渡します。
`HtmlInputFile : FileUpload` にして、バッファリングも `HasFile`/`FileName`/`SaveAs` も
描画も 1 箇所にまとめました。

変換器側は `HtmlInputControlFor` を追加し、`runat="server"` の `<input>` を
**WebForms のパーサと同じく type 属性で**振り分けます。

### 復帰規則: 証拠の強さで基準を分ける

`IAspNetUsersHelper` が 346 件(残り 549 件の 63%)を出していました。
Identity のコードとして除外され、復帰規則に拾われていません。理由は

```csharp
// 修飾名でしか探していなかった
UsesGoneType(code, (declared.Type, declared.Namespace))
```

呼び出し側は `this.Get<IAspNetUsersHelper>()` と**非修飾で書きます**。
235 箇所あっても 1 件も見えません。

非修飾の識別子は弱い証拠です。この README には
「除外する方向に使うと毎回暴走した(1 件 → 28 件 → 705 件)」と記録があります。
しかし**復帰する方向には安全**です。最悪でもファイルが移植されてローカルエラーが数個増えるだけで、
それは既に「連鎖より安い」と結論した取引そのものです。

**基準を証拠の強さで分けました。**

| 証拠 | 必要数 |
|---|---:|
| 修飾名(`YAF.Types.Interfaces.Identity.IAspNetUsersHelper`) | **1** |
| 単純名(`IAspNetUsersHelper`) | **5** |

単純名 1 件でも復帰させる版を測りました。

| | 既定 | 単純名 1 件 | 単純名 5 件 |
|---|---:|---:|---:|
| yaf | 598 | **46** | **10** |
| dnn | 19 | 169 | 67 |
| n2 | 32 | 96 | 54 |
| mojo | 13 | 40 | 15 |
| **合計** | **613** | 351 | **146** |

1 件では、たった 1 回の偶発的な言及で**本物の MVC ファイル**が戻ってきます。
5 件は「分類を間違えられた基盤コード」の形です(`IAspNetUsersHelper` は 235 箇所)。

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| **yaf ビルドエラー** | **598** | **10** |
| dnn | 19 | 67 |
| n2 | 32 | 54 |
| mojo | 13 | 15 |
| **合計ビルドエラー** | **613** | **146** |
| 合計総残差 | 505 | 530 |

残差 +25 は**復帰の報告そのもの**です(1 ファイル 1 行)。
移植ファイルは be 261 / mojo 754 / yaf 2729 / dnn 2066 / n2 1706 / wt 13。

パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

---

## 4 回目で、下限値の判定に型解決エラーを入れた

Identity の残り(`IdentityResult` / `UserLoginInfo` / `IPasswordHasher`)を宣言したら、
yaf が **10 → 459** になりました。**4 回目**です。

| 回 | 見えていた数 | 実数 |
|---:|---:|---:|
| 1 | 1 | 2135 |
| 2 | 2 | 838 |
| 3 | 10 | 459 |
| 4 | — | — |

`CS0234` / `CS0246`(型・名前空間が見つからない)は**シグネチャにも本体にも出る**ので、
最初は下限値判定から外していました。**外した判断が 3 回とも間違いでした。**
シグネチャは型を名指しするので、解決できなければ宣言パスで落ちます。

```csharp
"CS0234", "CS0246", "CS0012", "CS1069",
```

を `DeclarationErrorCodes` に追加しました。
**下限値でないものを「下限値かもしれない」と言う代償は一文です。
言わなかった代償は、読み違いが 4 回です。**

現在、6 コーパス全てで**下限値フラグは立っていません**(= 実数)。

### 宣言したもの(契約とデータのみ)

| 型 | 中身 |
|---|---|
| `IdentityResult` | `Succeeded` と `Errors`。ただのデータ |
| `UserLoginInfo` | `LoginProvider` / `ProviderKey`。ただのデータ |
| `IPasswordHasher` / `IPasswordHasher<TUser>` | **インターフェイスのみ。実装は置きません** |
| `PasswordVerificationResult` | 列挙 |

`IPasswordHasher` に既定実装を置かないのは意図的です。
パスワードのハッシュは移行時に**明示的に決めるべきこと**で、
無害そうな既定を置くと**ログインを黙って通したり弾いたり**します。
自前実装を持つアプリはそのまま動き、Identity の実装を期待していたアプリは
行き先が無い —— それが正直な状態です。

### `ListItem.Attributes`

yaf 459 件の最大は `'ListItem' に 'Attributes' の定義が含まれておらず` 155 件。
WebForms の `ListItem` は `Attributes` を持ち、コードビハインドが
option に `data-*` や class を載せるのに使います。追加しました
(実測 −11。155 件の残りは別の `ListItem` を指しており、次に切り分けます)。

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| yaf ビルドエラー | 10(下限値) | **448**(実数) |
| 合計ビルドエラー | 146 | 584 |
| 合計総残差 | 530 | 530 |

パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

### yaf 448 の残り

```
CS1061  'ListItem' に 'Attributes' …            155 → 一部解消
CS1929  'RepeaterItem' に 'FindControlAs' …     106
CS1973  'IRepository<T>' に 'ListPaged' …        44
CS0104  'Constants' があいまい                    25
```

`FindControlAs` と `ListPaged` は YAF 自身の拡張メソッドで、
レシーバの型が互換層の型と合っていないために効いていません。次はそこです。

---

## `dynamic` は伝染する — yaf 448 → 1

### RepeaterItem はコントロールではなかった

`'RepeaterItem' に 'FindControlAs' の定義が含まれておらず` 106 件。
YAF は `e.Item.FindControlAs<Label>("x")` と書きます。これは `Control` の拡張メソッドで、
変換器は `Control` 引数を `IWebFormsControl` に書き換えます(正しい)。
ところが**互換層で `IWebFormsControl` を実装していない唯一のものが行そのもの**でした。

WebForms では `RepeaterItem : Control` です。実装しました。
描画に関わるメンバー(`Visible` / `CssClass` / `Attributes`)は行にとっては不活性ですが、
**コントロールを期待する場所に行を渡せること**と、
**その行の `FindControl` がその行のコントロールに届くこと**が要点で、後者は元から出来ていました。

### `dynamic` が 447 件を生んでいた

残り 352 件の正体はこれでした。

```
CS1973: 'IRepository<UserAlbumImage>' には 'ListPaged' という該当するメソッドがありませんが、
        同じ名前の拡張メソッドがあるようです。拡張メソッドは動的ディスパッチできません。
```

該当コード:

```csharp
this.GetRepository<UserAlbumImage>().ListPaged(
    this.UserAlbum.ID,
    this.PagerTop.CurrentPageIndex,   // ← PagerTop が dynamic
    this.PagerTop.PageSize);
```

`PagerTop` は LegacyRenderHost 経由のコントロールで、フィールドは `dynamic` でした。
**引数が 1 つでも dynamic なら、呼び出し全体が動的ディスパッチになります。**
そして**拡張メソッドは動的ディスパッチできません**。

`dynamic` にしていた理由は「コードビハインドが何を触っても通るから」です。
しかし**型はその場で分かっています** —— `EmitLegacyRenderHost` は
`legacyTypeName`(移植済みクラスの完全名)を持っています。

```csharp
private LegacyRenderHost __PagerTop_host;
protected global::YAF.Controls.Pager PagerTop
    => __PagerTop_host?.ControlInstance as global::YAF.Controls.Pager;
```

**書き下しても失うものはありません。** 型はそれで合っているのですから。

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| **yaf ビルドエラー** | **448** | **1** |
| 合計ビルドエラー | 584 | **137** |
| 合計総残差 | 530 | 530 |

内訳: be 0 / mojo 15 / yaf 1 / dnn 67 / n2 54 / wt 0。
パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

`dynamic` の除去は**このセッションで単発としては最大の効果**(447 件)でした。
「何でも通る型」は、通らないものを作ります。

### 撤回: `global::` を外しても再名前空間は効かない(記録)

yaf に残る 1 件はこれでした。

```
Components/Pages/ForumPageBase.razor.cs(18,27):
  error CS0400: 型名または名前空間名 'YAF' がグローバル名前空間に見つかりませんでした
  protected global::YAF.Web.Controls.Form form1 => __form1_host?.ControlInstance as global::YAF.Web.Controls.Form;
```

同じ `.razor` 側は**再名前空間済み**です。

```razor
<LegacyRenderHost TypeName="yaf.Components.Pages.Web.Controls.Form" ... />
```

書き換えの正規表現は `(?<![\w.])Original\.` なので、`global::` の直後(`:` の次)でも
**マッチするはず**です。そこで `global::` を外してみましたが、**変わりませんでした**。

理由は位置でした。**このフィールドは再名前空間パスが終わった後に生成されます。**
正規表現の問題ではなく、順序の問題です。

447 件に対して 1 件なので、`global::` 付き(測定上最良)に戻しました。
正しい直し方は、フィールドの型を**生成時点で**再名前空間することです。

---

## `GenerateFieldOnlyCodeBehind` だけが再名前空間パスを通っていなかった

前節で「フィールドは書き換えパスの後に生成される」と書きましたが、**間違いでした**。
生成経路は 2 つあります。

```csharp
if (component.CodeBehindSourcePath is not null)
{
    ...
    File.WriteAllText(..., ApplyNamespaceMap(compatImports.Apply(rewritten)));   // ← 通る
}
else if (component.Fields.Count > 0)
{
    File.WriteAllText(..., GenerateFieldOnlyCodeBehind(component));              // ← 通らない
}
```

**コードビハインドを持たないコンポーネント**(マークアップだけのページ)の
フィールド専用ファイルが、唯一 `ApplyNamespaceMap` を素通りしていました。
そこに書かれるのは**移植済みコントロールの型名**で、それは再名前空間されるものです。
YAF の `ForumPageBase` が `global::YAF.Web.Controls.Form` を持ち、
隣の `.razor` は正しく `yaf.Components.Pages.Web.Controls.Form` と書いている、
という食い違いはこれでした。**1 ファイル、1 行の欠落**です。

### また下限値だった(5 回目)

直した結果、yaf は **1 → 337**。`CS0400`("global:: の X がグローバル名前空間にない")も
宣言パスのエラーなので、`DeclarationErrorCodes` に追加しました。

### 集計表の読み方を間違えていた

337 件の内訳表はこう出ます。

```
| CS1061 | 186 | 'HtmlTextArea' に 'InnerText' の定義が含まれておらず… |
```

**186 は CS1061 の総数で、メッセージは代表例 1 件**です。
`HtmlTextArea.InnerText` を直したら **−4** でした。
メッセージ別に数え直すと、最大クラスタでも 24 件の長い尾でした。

| メッセージ別 | 件数 |
|---|---:|
| `Constants` があいまい | 24 |
| `HttpRequestBase` に `MapPath` | 24 |
| `Enumerable.Distinct` の呼び出しがあいまい | 16 |
| `HttpContext.Current` は読み取り専用 | 8 |
| `HttpResponseBase` に `ClearContent` / `ClearHeaders` | 16 |

### 塞いだ互換シムの穴

| 追加 | 理由 |
|---|---|
| `HtmlTextArea.InnerText` / `InnerHtml` | textarea の中身は値そのもの。3 つの名前が同じ文字列を指すのが 4.8 の挙動 |
| `HttpRequestBase.MapPath` | 4.8 では `Server.MapPath` と同じメソッドで、どちらからでも呼べた |
| `HttpResponseBase.ClearContent` / `ClearHeaders` | ファイルやフィードを書く前の「ページの出力は捨てる」。Blazor に捨てる緩衝は無いが、続く Content-Type と書き込みに到達させる必要がある |

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| yaf ビルドエラー | 1(下限値) | **292**(実数) |
| 合計ビルドエラー | 137 | 428 |
| 合計総残差 | 530 | 530 |

パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

### 付記: バックグラウンド実行の重複でビルドが壊れる

検証が `失敗: converter build` を出しました。原因は変換器ではなく

```
error MSB3027: ... WebForm2Blazor.Components.dll をコピーできませんでした。
               このファイルは ".NET Host (6164)" によってロックされています。
```

**変換とビルドを並行で走らせた自分のせい**でした。
`dotnet` プロセスを落として直列で回すと全て緑です。
検証結果を読むときは、失敗が変換器のものか環境のものかを先に確かめること。

---

## yaf 292 → 278(長い尾に入った)

メッセージ別に数え直した内訳(上位):

| メッセージ | 件数 |
|---|---:|
| `Constants` があいまい | 24 |
| `Enumerable.DistinctBy` の呼び出しがあいまい | 16 |
| プロパティを `out`/`ref` に渡している | 8 |
| `HttpContext.Current` は読み取り専用 | 8 |
| `AttributeCollection` に `CssStyle` が無い | 6 |

**もう単独で 24 件が最大**で、ここからは 1 件ずつの世界です。今回はうち 2 つを塞ぎました。

### `HttpContext.Current` は代入できるべきだった

4.8 の `HttpContext.Current` は**設定可能**です。リクエスト外で動くコード
(バックグラウンドジョブ、インストーラ、スケジューラ)が、
後続のコードにコンテキストを見せるために代入します。YAF はその両方でやっています。

読み取り専用にしていたことで**何かが防げていたわけではなく**、
そのコードがコンパイルできなくなっていただけでした。
`[ThreadStatic]` で明示代入を保持し、代入がある間はそちらが勝ち、
null を入れるとリクエストのコンテキストに戻ります。

### `Attributes.CssStyle` は `style` 属性のビュー

`Attributes.CssStyle["display"] = "none"` は、
**インライン style の他の指定を壊さずに 1 つだけ変える**書き方です。

**別のコレクションにはしませんでした。** 書き込むと `a:b;c:d` を組み直して
`this["style"]` に入れます。元アプリが描いていたのはその文字列で、
`Attributes["style"]` を読む側もそれを期待します。

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| yaf ビルドエラー | 292 | **278** |
| 合計ビルドエラー | 428 | **414** |
| 合計総残差 | 530 | 530 |

パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

### 残る 2 つの大物は別の性質

- **`Constants` があいまい(24)** — Lucene.Net のファイルに YAF の
  `Types.Constants` が届いています。**グローバル using がマージ後の全体に漏れる**
  という既知の問題(この README の別項)で、互換層ではなく変換器側の話です。
- **`Enumerable.DistinctBy` があいまい(16)** — .NET 6 が `DistinctBy` を追加し、
  YAF が同梱している同名の拡張メソッドと衝突しています。
  `OrderedDictionary` のときと**同じ形**(BCL が後から生えて同梱ライブラリとぶつかる)ですが、
  あちらは型、こちらは拡張メソッドなので、別名では解決できません。

---

## 「3 つ目のコーパスを回帰ゲートに載せる」は外部ライブラリで止まっている

yaf の残り 278 件が長い尾に入ったので、**目的**に立ち返って一番近いコーパスを探しました。
回帰ゲートに載せるにはビルドが通る必要があり、mojoPortal は**カウント上 15 件**で最短に見えます。

実際にビルドすると **135 件**でした。カウントされていない 120 件の内訳:

| 同梱 DLL | 件数 |
|---|---:|
| `Lucene.Net` | 64 |
| `MetaDataExtractor` | 18 |
| `ZedGraph` | 12 |
| `Novell.Directory.Ldap` | 12 |
| `Microsoft.ApplicationServer.Caching.Client` | 5 |
| `ZedGraph.Web` | 4 |

**これは「ユーザーが移行後のライブラリを供給する」と決めた枠**そのもので、
変換器が直せるものではありません。そしてビルドは通りません。

### 結論

| コーパス | カウント上 | 実ビルド | 起動できるか |
|---|---:|---:|---|
| be | 0 | 0 | **可**(回帰ゲート稼働中) |
| wt | 0 | 0 | **可**(回帰ゲート稼働中) |
| mojo | 15 | 135 | 不可(同梱 DLL 6 種) |
| yaf | 278 | 285 | 不可 |
| dnn | 67 | 400+ | 不可(Lucene / DotNetNuke.WebControls 他) |
| n2 | 54 | — | 不可 |

**3 つ目の挙動検証コーパスは、変換器側の欠陥を全部潰しても得られません。**
移行済みの Lucene.Net / ZedGraph / LDAP / AppFabric などを供給してもらう必要があります。
`package-map.template.json` がその受け口として各出力に生成済みです。

これは悪い報せではなく、**測り方の整理**です。
「ビルドエラー n 件」を減らす作業と「挙動を検証できるコーパスを増やす」作業は別物で、
後者は現状 be と wt の 2 つで頭打ちです。変換器にできることは
**カウント対象のエラー(= 外部ライブラリ以外)をゼロに近づけること**までで、
その数字は今 be 0 / wt 0 / mojo 15 / n2 54 / dnn 67 / yaf 278 です。

---

## Web プロジェクトの global using も局所化する

`Constants` のあいまい参照 24 件の出所は、出力ルートに残っていた `GlobalUsings.cs` でした。

```csharp
global using yaf.Components.Pages.Types.Constants;
```

これが**マージ後のコンパイル全体に効き**、YAF とは無関係な
`Lucene.Net.Analysis.Common/Analysis/Ga/IrishLowerCaseFilter.cs` にも届いて、
そこでは `Constants` が `Lucene.Net.Util.Constants` と衝突していました。

局所化の仕組みは既にありました。ただし:

```csharp
if (!candidate.Included) { continue; }   // ← Web プロジェクトは対象外
```

**Web プロジェクトだけ除外**されていました。理由はコメントに書いてあります ——
「その配下のファイルはページで、生成される .razor / .razor.cs は別の場所に書かれるので
global を外すと import を失う」。

それは正しい観察ですが、**対処が逆**でした。global のままにするのは
「ページに import を残す」ためではなく「全コンパイルに撒く」ことです。
除外をやめ、**生成されるコードビハインドに同じ using を明示的に書く**ようにしました。
そのファイル群こそが Web プロジェクトなので、元と同じ import になります。

### 途中で出した 171 件(記録)

最初の実装で yaf が **278 → 171 の「下限値」**になりました。内訳は全部これです。

```
CS0234: 型または名前空間の名前 'UI' が名前空間 'System.Web' に存在しません   171
```

`global using System.Web.UI;` を**そのまま書き戻していた**ためです。
それは変換器が全ファイルから剥がしている名前空間で、
書き戻せば存在しない名前空間の import が復活します。
`System.Web*` を除外して解決しました。

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| yaf ビルドエラー | 278 | **250** |
| 合計ビルドエラー | 414 | **386** |
| 合計総残差 | 530 | 530 |

パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

---

## フレームワークが後から生やした LINQ 拡張は、同梱側を消す

`DistinctBy` のあいまい呼び出し 16 件。YAF は自前の拡張メソッドを持っています。

```csharp
// YAF.Types/Extensions/EnumerableExtensions.cs
public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
    this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
{
    var knownKeys = new HashSet<TKey>();
    return source.Where(element => knownKeys.Add(keySelector(element)));
}
```

.NET 6 が **まったく同じシグネチャ・まったく同じ意味**のものを `Enumerable` に追加しました
(最初のキーが勝つ)。両方が適用可能なので、呼び出しは全部 CS0121 です。

**別名では解決できません。** 拡張メソッドなので `using X = ...;` が届きません。
`OrderedDictionary` のときは型だったので書き下せましたが、これはメソッドです。

消すのは**同梱側**です。あいまいにしている原因はそちらで、
フレームワークのものは同じ動作をします —— **そもそもそれが追加された理由**です。

判定は `System.Linq.Enumerable` の**実物に問い合わせ**ます
(公開静的メソッドを「名前`ジェネリック数(引数数)」で索引)。
一致条件は名前・ジェネリック引数の数・パラメータ数、かつ第 1 引数が
`this IEnumerable<...>`。同名でも形が違うものは残ります。

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| yaf ビルドエラー | 250 | **234** |
| 合計ビルドエラー | 386 | **370** |
| 合計総残差 | 530 | 530 |

パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

### このセッションの累計

| | 開始時 | 現在 |
|---|---:|---:|
| 総残差 | 702 | **530** |
| 未対応コントロール | 113 | 28 |
| 標準コントロール未実装 | 34 | **0** |
| be / wt のカウント上エラー | — | **0 / 0** |

ビルドエラーの合計は開始時 216 → 現在 370 ですが、**比較できません**。
yaf は 5 回にわたり「宣言パスで止まっていた下限値」を表示しており
(1 → 2135 → 838 → 459 → 337)、実数が見えるたびに跳ねました。
`StoppedAtDeclarations` を入れた今は 6 コーパスとも実数です。

---

## ライフサイクルは「オーバーライド」だけでなく「イベント」でもある

yaf の残りから 2 つ。どちらも **WebForms の面をオーバーライド点としてしか持っていなかった**ぶんです。

### `this.Load += ...`

```
CS1061 'ForumPage' に 'Load' の定義が含まれておらず…      6
CS1061 'ForumPage' に 'PreRender' の定義が含まれておらず…  6
```

互換層は `protected virtual void OnLoad(EventArgs e)` を持っていましたが、
**イベントの方がありません**でした。WebForms のデザイナが生成するのは

```csharp
this.Load += this.ForumPage_Load;
```

で、YAF は手書きでも同じ書き方をします。**自分のライフサイクルを購読する**のは
オーバーライドと並ぶ正規の書き方です。`Init` / `Load` / `PreRender` / `Unload` を
イベントとして追加し、対応する `On*` から発火します(WebForms と同じ順序 ——
オーバーライドが先、購読者が後)。

### `this.Events.AddHandler(...)`

```
CS1061 'ThemeButton' に 'Events' の定義が含まれておらず…  6
```

イベントが多い(そして多くは使われない)コントロールは、
イベントごとにデリゲートのフィールドを持つ代わりにこう書きます。

```csharp
public event EventHandler Click
{
    add { this.Events.AddHandler(ClickEventKey, value); }
    remove { this.Events.RemoveHandler(ClickEventKey, value); }
}
```

YAF の `ThemeButton` がそれです。`Control.Events` が無いと、
**プロパティが無いのではなくイベント宣言自体がコンパイルできません**。
`EventHandlerList` を互換層に置き、`LegacyWebControl.Events` を足しました
(実物は `System.ComponentModel` に今もありますが、
`System.Web.UI.Control` を継がなくなったコントロールからは辿れません)。

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| yaf ビルドエラー | 234 | **208** |
| 合計ビルドエラー | 370 | **344** |
| 合計総残差 | 530 | 530 |

パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

---

## リスト系の基底は、リストを持っていなければならない

```
CS1061 'ImageListBox' に 'Items' の定義が含まれておらず…
```

`ImageListBox` は YAF の `ListBox` 派生です。移植時の基底置換表はこうなっていました。

```csharp
["DropDownList"] = "LegacyWebControl",
["ListBox"]      = "LegacyWebControl",
```

`LegacyWebControl` はライフサイクルと描画の仮想メソッドを持ちますが、
**リストをリストたらしめるものは何も持ちません**。
派生クラスは `Items` も `SelectedValue` もデータバインドのフィールドも全部失い、
コードでリストを組む行が軒並み落ちていました。

`LegacyCalendar` / `LegacyPanel` と同じ形で `LegacyListControl` を用意し、
`DropDownList` / `ListBox` / `ListControl` / `CheckBoxList` / `RadioButtonList` を
そこへ向けました。

**選択は本物の状態**です。`SelectedValue` を代入して読み戻すコードは元どおり動きます。
`DataSource` + `DataBind()` も、`DataTextField` / `DataValueField` /
`DataTextFormatString` / `AppendDataBoundItems` を含めて元と同じ項目を作ります。
起きないのは**変更時のポストバック**で、対話的なリストは
手作業での移植対象として別途報告されます。

`SelectedValue` に一致しない値を入れると選択が外れるのも 4.8 の挙動です
(だから「あるかどうか分からない値」を代入するコードが書ける)。

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| yaf ビルドエラー | 208 | **164** |
| mojo 総残差 | 77 | 76 |
| dnn 総残差 | 161 | 160 |
| 合計ビルドエラー | 344 | **300** |
| 合計総残差 | 530 | **528** |

パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

---

## 1 件ずつではなく、不足メンバを全コーパスで一括集計した

`Control.Events` → ライフサイクルのイベント → リストの `Items` と、
**3 回続けて同じ種類の穴**(型の名前はあるが、その型が持っていた面が無い)でした。
1 件ずつ潰すのをやめ、**全コーパスの「型 X にメンバ Y が無い」を一度に数え**ました。

```
CS1061 / CS0117 を型.メンバ で集計 → 84 件
```

| 不足 | 件数 |
|---|---:|
| `TextBoxMode.Date` / `.Number` | 8 |
| `ClientScriptManagerShim.GetPostBackClientHyperlink` | 4 |
| `ScriptManager.ScriptResourceMapping` | 4 |
| `IWebFormsControl.FindControl` / `.Parent` / `.Site` | 7 |
| `HttpContext.ApplicationInstance` | 3 |
| `HttpSessionStateBase.SessionID` | 2 |
| (以下 2 件以下の尾) | |

**すべて小さなクラスタ**でした。つまり「大きな型が丸ごと抜けている」のではなく、
**細かい面が満遍なく欠けている**状態です。今回はそのうち確実なものを塞ぎました。

### 塞いだもの

| | 中身 |
|---|---|
| `TextBoxMode` に HTML5 の値 | WebForms 4.5 が追加した `Date` / `Number` / `Email` ほか。**ブラウザで日付ピッカーや数値入力にする指定**そのもの。3 つしか無かったので `TextBoxMode.Date` がコンパイルできなかった |
| `IWebFormsControl.FindControl` / `Parent` / `Site` | 既定実装付き。この interface は**描画系とコンポーネント系の両方**を跨ぐので、多くは意味を持たないが、**宣言が無いと呼ぶ側がコンパイルできない** |
| `HttpSessionStateBase.SessionID` | アプリは訪問者ごとの状態(キャッシュ、アップロード先)をこれで区切る。セッション内で不変・セッション間で一意 |
| `ClientScriptManager.GetPostBackClientHyperlink` / `GetPostBackEventReference` | `javascript:__doPostBack(...)` を作る面。Blazor に `__doPostBack` は無いので `javascript:void(0)` を返す —— **null を返すとマークアップに "null" が出ます**。宣言が無いとコントロール自体が通らない |

### 計測

| | 変更前 | 変更後 |
|---|---:|---:|
| yaf ビルドエラー | 164 | **154** |
| 合計ビルドエラー | 300 | **290** |
| 合計総残差 | 528 | 528 |

パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

---

## .NET 対応の外部ライブラリを調べた

コーパスのビルドを止めている第三者ライブラリについて、現在の .NET 対応状況を調べました。
**判断の分かれ目は「名前空間と API が同じか」**です。同じなら変換器が自動で紐付けられます。
違うなら**移行の判断**であり、`package-map.template.json` でユーザーが答えるものです。

### 自動で紐付けたもの(名前空間・API が同一)

| 元 | .NET 版 | 根拠 |
|---|---|---|
| `Novell.Directory.Ldap` | **`Novell.Directory.Ldap.NETStandard` 4.0.0** | 名前空間そのまま。.NET 6/8/9 対象 |
| `ZedGraph` | **`ZedGraph` 5.2.1**(2025-10) | 名前空間そのまま。.NET 6 対象(= .NET 10 で解決) |
| `ICSharpCode.SharpZipLib` | `SharpZipLib` 1.4.2(既出) | 1.0 から netstandard2.0 |
| `Ionic.Zip` | `DotNetZip`(既出) | |

### 紐付けなかったもの(移行の判断が要る)

| 元 | 状況 | なぜ自動化しないか |
|---|---|---|
| `Lucene.Net` 3.0.3 | **4.8.0-beta18**(2026-06 更新、まだ beta) | 3.x → 4.8 は**移植ではなく書き直し**。API が別物 |
| `MetaDataExtractor` | **`MetadataExtractor` 2.9.3**(活発) | 名前空間が `com.drew.*` → `MetadataExtractor` に変わっている |
| `ClientDependency.Core` | **開発終了**。後継は `Smidge` | API が別物。DNN 自身も離脱済み |
| `ZedGraph.Web` | **2011 年で停止**、.NET 版なし | WebForms のチャートコントロールに .NET の器が無い |
| `Microsoft.ApplicationServer.Caching`(AppFabric) | **2022 年に完全終了**。MS 推奨は Redis / NCache | API が別物 |
| `DotNetNuke.*` / `effority.ealo` 等 | アプリ同梱のバイナリ | ユーザーが供給するもの |

`ZedGraph.Web` は特に注意が要ります。**チャートの中核は .NET で動きますが、
それを WebForms のページに貼る部分だけが無い**という形で、
「ライブラリを差し替えれば済む」ではなく「表示のしかたを決め直す」案件です。

### 露出した欠陥: ページの using からパッケージを検出していなかった

`Novell` は自動で入ったのに `ZedGraph` は入りませんでした。原因は収集範囲です。

```csharp
CollectUsingNamespaces(candidateSource, portedNamespaces);   // ← プレーンコードのループ 1 箇所だけ
```

mojoPortal がチャートを使っているのは `SiteStatisticsModule.razor.cs` と
`SalesByItemPage.razor.cs` ——**ページのコードビハインド**です。
ライブラリ側のファイルは 1 つも ZedGraph を名指ししないので、
「using から必要なパッケージを足す」仕組みが**ページを見ていなかった**ぶん、丸ごと漏れていました。
コンポーネントのコードビハインドも収集対象にしました。

### 計測

カウント上のエラーは変化しません(これらは元から「外部ライブラリ」として除外枠)。
**効いたのは実ビルド**です。

| | 変更前 | 変更後 |
|---|---:|---:|
| **mojo 実ビルドエラー** | **135** | **115** |
| 合計総残差 | 528 | 528 |
| 合計カウント上エラー | 290 | 290 |

内訳: LDAP 12 件と ZedGraph の中核 8 件が解決。
残る 115 は Lucene 約 62、MetaDataExtractor 18、ZedGraph.Web 8、その他。

パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

---

## ライブラリ移行を汎用機能にした(実体に聞いて、決まる分だけ決める)

「API が変わっているから AI で」という話の**前段**を先に作りました。
`MetaDataExtractor` → `MetadataExtractor` のように、**型が移動しただけ**の部分は
AI の判断を要しません。**両方のアセンブリを読んで名前を突き合わせれば決まります**。

### `AssemblyTypeMigration`

`--package-map` のエントリに `"dll"`(置き換え先アセンブリのパス)を書くと、
変換器は**旧と新の両方のメタデータを読み**、

- 単純名が**両側でちょうど 1 つずつ**現れる型 → 完全名を書き換え
- ある旧名前空間の型が**全部同じ新名前空間**に行っていれば → `using` も書き換え
- どちらでもないもの → **残差として型名を列挙**

します。手書きの対応表は作りません。このセッションで
「描画可能な基底の手書きリスト」「互換型名の手書きリスト」「Framework 専用名前空間の手書きリスト」
が**全部間違っていた**のを見ているので、同じ形は繰り返しません。

曖昧なとき(どちらかの側に同名が複数)は**何もしません**。
このパスは推測を許されない部分です。

### mojoPortal で試した結果

```
MetaDataExtractor を置き換え先へ移行します: 型 22 件を対応付け、名前空間 7 件を書き換えます。
MetaDataExtractor の型 50 件は置き換え先に同名のものがありません:
  com.codec.jpeg.JPEGDecodeParam, com.drew.metadata.AbstractTagDescriptor,
  com.drew.metadata.exif.CanonDescriptor, ... ほか
  (名前が変わったか、なくなったかのどちらかで、機械的には決められません)
```

| | 変更前 | 変更後 |
|---|---:|---:|
| **mojo 実ビルドエラー** | **115** | **103** |
| うち `com.drew` 由来 | 18 | **6** |
| mojo カウント上エラー | 15 | 21 |

**カウント上が増えたのは正しい動き**です。`--package-map` で置き換え先を指定した時点で、
そのアセンブリは「ユーザー未決定」ではなくなります。除外枠から出て、
**残った差分が変換器の課題として数えられる**ようになりました。

### 境界がはっきりした

22 件は決定的に解決し、50 件は「名前が変わったか、なくなった」と**具体名で**残りました。
**これが AI 層への入力**です。AI に渡すのは「実体に聞いても答えが出なかった分」だけで、
ライブラリ全体ではありません。

残る 6 件の `com.drew` エラーもこの 50 件の一部で、
`AbstractDirectory` → `Directory` のような**改名**です。
これは名前の一致では取れません。

### 露出した欠陥 2 つ

1. **`--package-map` の読み込みが移行の構築より後**でした。移行は生成物を書く前に
   組み立てる必要があるので、読み込みを前に出しました。
2. 旧アセンブリの探索が `binaryReferences`(スコープ外)を見ていました。
   `FindBuiltAssembly` で入力ツリーから探すように直しています
   (mojoPortal は `_libs\MetaDataExtractor.dll` に置いていて、
   入力ディレクトリ `Web` の隣ではありません)。

パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

## Lucene.Net も移行した — mojo 実ビルド 103 -> 43

`AssemblyTypeMigration` は汎用機能なので、対象を増やすのはパッケージマップに 1 行足すだけです。
mojo の残り 103 件を調べると **64 件が Lucene** で、しかもすべて CS0246「見つかりません」でした。
つまり参照そのものが無い。Lucene.Net 4.8.0-beta00018 と Analysis.Common / Highlighter を
`corpora/mojo-package-map.json` に `"dll"` 付きで足しました。

```
Lucene.Net を置き換え先へ移行します: 型 77 件を対応付け、名前空間 0 件を書き換えます。
Lucene.Net.Contrib.Analyzers を置き換え先へ移行します: 型 58 件を対応付け、名前空間 3 件を書き換えます。
```

| | 変更前 | 変更後 |
|---|---:|---:|
| **mojo 実ビルドエラー** | **103** | **43** |
| うち Lucene 由来 | 64 | **5** |
| mojo カウント上エラー | 21 | 24 |
| mojo 総残差 | 77 | 79 |

前回と同じ理由で**カウント上は増えます**。Lucene が「未決定」枠を出たので、
残った差分が変換器の課題として数えられるようになりました。
実ビルドが 103->43 に落ちたことが実態です。

他 5 コーパスは完全に一致(be 64/2/0、yaf 54/5/154、dnn 160/10/67、n2 141/11/54、wt 33/3/0)。
パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

### 残り 43 件の内訳

| 区分 | 件数 | 性質 |
|---|---:|---|
| 未決アセンブリ(AppFabric/TimelineNet/ZedGraph.Web/Subkismet/CSSFriendly/Argotic) | 19 | .NET 版が無い。ユーザーが用意する対象 |
| `com.drew` の改名(`AbstractDirectory`→`Directory` 等) | 6 | 名前一致では取れない。AI 層の入力 |
| `Lucene.Net.QueryParsers` | 5 | **パッケージ分割**。4.8 では `Lucene.Net.QueryParser` に出た |
| その他 mojo 内部/互換シム | 13 | 変換器側で直せる |

`Lucene.Net.QueryParsers` の 5 件は移行機能自体の穴です。
旧 `Lucene.Net.dll` 1 本にあった型が、新しい版では**複数パッケージに分かれています**。
今の実装は旧 1 本 -> 新 1 本しか見ないので、分割先が見えません。
1 つの旧アセンブリに複数の置き換え先を許すのが正しい直し方です。

## mojo 実ビルド 43 -> 28、カウント上 24 -> 8

前節で残した 4 区分のうち 3 つを片付けました。

### 1. 1 つの旧アセンブリに複数の置き換え先を許した

`Lucene.Net.QueryParsers` の 5 件は、旧 `Lucene.Net.dll` 1 本にあった型が
4.8 では `Lucene.Net.QueryParser` パッケージに分かれたものでした。
`"dll"` に配列を書けるようにし、置き換え先の型は**全部まとめてから**突き合わせます
(片方ずつ読むと、両方にある名前が「一意」に見えてしまう)。
同じ仕組みで `Lucene.Net.ICU`(ThaiAnalyzer)も足しました。

### 2. 名前空間の証拠で曖昧さを解いた

`ParseException` は旧側に 2 つ(`QueryParsers` と `Analysis.Standard`)、
新側に 3 つ(`Classic` / `Flexible.Standard.Parser` / `Surround.Parser`)あり、
名前だけでは 5 通りのどれとも決められません。

ところが旧 `QueryParsers` には `QueryParserConstants` と `QueryParserTokenManager` もあり、
これらは両側で一意で、**どちらも `QueryParsers.Classic` に着地**しています。
3 つの新 `ParseException` のうち `Classic` にあるのは 1 つだけ。答えは 1 つに決まります。

`Analysis.Standard.ParseException` は、着地先の名前空間に `ParseException` が
1 つもないので未対応のまま残ります。これも正しい — 無くなった型です。

> 単独では弱い証拠でも、**同じ名前空間で既に確定した移動**は弱くありません。
> 変換器の他の判断と同じ原則です。

型の対応付けは Lucene.Net が 77 -> 86 件、Analyzers が 58 -> 63 件に増えました。

### 3. コンポーネント化で名前空間が変わった型を、残った側から届くようにした

これが一番効きました(**11 件**)。

`Controls/MetaContent.ascx.cs` は `mojoPortal.Web.UI` から
`mojo.Components.Controls.Controls` へ動きます。ところが `mojoPortal.Web.UI` の**残り**は
動きません。ただのライブラリコードで、`MetaContent` を修飾なしで書き続けます。

名前空間マップはここでは使えません。名前空間を**丸ごと**付け替えるものなので、
移動しない移植コードが残っている名前空間では正しく「やらない」と判断します。
しかしやらないと移動した型に届かなくなる。これは「.NET に無い依存」ではなく
**変換器自身が動かした型**です。mojoBasePage が `MetaContent` と `StyleSheetCombiner` を、
PageEditFeaturesLink が `CmsPage` を、こうして失っていました。

`RelocatedTypeIndex` が移動を記録し、**名前空間単位ではなく型単位で**使用側に戻します。
修飾された参照は書き換え、修飾なしの参照には `using X = ...;` を補います。
どちらも変換器が実際にやったことから決まります。

### 4. 互換層の穴 4 つ

| 型 | 元 | 件数 |
|---|---|---:|
| `CompositeDataBoundControl` | System.Web.UI.WebControls | 1 |
| `TargetConverter` | System.Web.UI.WebControls | 1 |
| `ICertificatePolicy` | System.Net(.NET で削除) | 1 |
| `SelectListItem` / `SelectListGroup` | System.Web.Mvc | 2 |

`SelectListItem` のために `System.Web.Mvc` を互換名前空間の探索対象に足しました。
互換層が実際に宣言している名前しか書き換えないので、MVC 全体を引き込むことはありません。

さらに `BaseValidator.EvaluateIsValid()` と `GetControlValidationValue(string)` を
`ValidatorBase` に足しました。**引数なしの override を持つ検証コントロールは、
そちらが優先されます** — mojoPortal の EmailValidator はサーバ側で正規表現を
意図的に使わないので、互換 RegularExpressionValidator の判定を走らせると
同じ入力に違う答えが出ます。override の有無は型に聞きます(フラグを持たせない)。

### 5. テンプレートタグの大小文字

`<emptydatatemplate>` は .aspx では GridView.EmptyDataTemplate に一致しますが、
Razor のコンポーネントパラメータは大小文字を区別します。そのまま出すと
RZ9996 で **GridView ごと落ちます**。マッピング表は大小無視のキーで
WebForms の綴りを**保持している**ので、`TryGetValue` でその綴りを取り出します
(別表を作ると食い違うため)。

### 6. カレントディレクトリで結果が変わっていた

作業中に見つけた欠陥です。`webforms-property-catalog.json` を
**カレントディレクトリからだけ**探していたため、
`corpora\convert-all.ps1` をリポジトリルートから実行すると読み込まれ、
`corpora\` から実行すると読み込まれませんでした。6 コーパス合計で残差が 80 違います。
変換器の隣を先に探すようにし、csproj で出力先へコピーするようにしました。
**見つからなければ残差として言います** — 黙っているほうが問題でした。

### 測定

| | 変更前 | 変更後 |
|---|---:|---:|
| **mojo 実ビルドエラー(重複除去)** | **43** | **28** |
| mojo カウント上エラー | 24 | **8** |
| mojo 総残差 | 79 | 80 |

他 5 コーパスは完全に一致(be 64/2/0、yaf 54/5/154、dnn 160/10/67、n2 141/11/54、wt 33/3/0)。
パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

**mojo の 8 件は下限値です。** 残りが全部「宣言段階のエラー」になったため、
Blazor の 2 回コンパイルの 1 回目で止まり、本体の意味解析が走っていません。
ゲートはこれを検出して警告します(この状態の数値を前後比較に使ってはいけません)。

残り 8 件の内訳:

| 区分 | 件数 | 性質 |
|---|---:|---|
| `com.drew` の改名(`AbstractDirectory` → `Directory` 等) | 6 | 名前一致では取れない。**AI 層の入力** |
| `ServiceHost`(WCF) | 1 | スコープ外 |
| `RecentContentConfiguration` | 1 | Argotic 除外からの連鎖 |

決定論的に取れるものは取り切りました。ここから先は AI 層の仕事です。

## 「置き換え先が無い」を決定として受け取る — mojo 実ビルド 28 -> 8

残っていた 19 件の「未決の依存」は、**.NET 版が存在しないライブラリ**でした。
AppFabric キャッシュ、ZedGraph の Web コントロール、Subkismet(CAPTCHA)、
CSSFriendly(コントロールアダプタ)、TimelineNet、Argotic(RSS)。
探しても無いものは無く、これは変換器の欠陥でもユーザーの怠慢でもありません。

`--package-map` には元々「package を空にすると引き継がない」という書き方がありました。
これまでは**参照を足さないだけ**で、依存するファイルはそのまま移植され、
型が見つからずビルドが落ちていました。決定を書いたのに何も起きなかったわけです。

今は、空 package のエントリを見ると**そのアセンブリ自身を読んで名前空間を取り**、
それを移植対象外として登録します。依存するファイルは除外され、
宣言していた型はスタブになり、**残りはビルドできます**。

- ユーザーが答えるのは「どのアセンブリに置き換え先が無いか」だけ。
- 「それがどの名前空間を意味するか」はアセンブリが知っている。
- 名前から推測するものは何もありません。

### ただし、アプリ自身が宣言している名前空間は取り上げない

`HtmlDiff.dll` は型を `Helpers` という名前空間に置いています。
これを移植対象外にしたら、**mojoPortal 自身のファイルが 41 件巻き込まれました**
(移植 .cs 747 -> 706、残差 92 -> 137、エラー 6 -> 12)。HtmlDiff とは無関係のコードです。

ライブラリが消えても**まだコードが残っている名前空間は、そのライブラリのものではありません**。
アプリが宣言している名前空間は除外対象から外し、
代わりに「その分の CS0246 は残る」と残差で言うようにしました。
HtmlDiff 自体はコーパスのマップから外してあります(.NET 版が
`HtmlDiff 1.0.0` = .NET Framework 専用しかないため、未決のままが正しい)。

### WCF のサービスホスト

`System.ServiceModel` は**丸ごと除外しません** — ChannelFactory や契約属性には
.NET パッケージがあり、除外するとちゃんと移植できるコードまで落ちます。
落ちないのは**ホスティング側**で、`.Activation` と `.Web` は既に除外済みでした。
`ServiceHost` だけがクライアント側と同じ名前空間にいます。

そこで「**基底クラスが ServiceHost 系**である」という条件だけ足しました。
言及ではなく基底リスト — これだけありふれた名前は、
ファイルが自分をそれにしていて初めて数えるべきです。
mojoPortal の mojoServiceHost がこれで、唯一の呼び出し元
(mojoServiceHostFactory)は既にスコープ外でした。1 ファイル、1 エラー。

### 測定

| | 変更前 | 変更後 |
|---|---:|---:|
| **mojo 実ビルドエラー** | **28** | **8** |
| うち未決の依存(カウント外) | 19 | 2 |
| mojo カウント上エラー | 8 | **6** |
| mojo 総残差 | 80 | 92 |
| mojo 移植 .cs | 754 | 747 |

残差が 12 増えるのは**決定を書いた結果**です。19 件の「未決」が
7 ファイルの「スコープ外(置き換え先が無いと判断済み)」に変わりました。
数が増えて内容は良くなっています。

他 5 コーパスは完全に一致。パリティ 30/30、bUnit 30/30、回帰ゲート 13/13。

### 残り 6 件

全部 `com.drew` の**改名**です(`AbstractDirectory` → `Directory` など)。
名前の一致では取れず、名前空間の証拠でも取れません(着地先に同名が無い)。
**決定論的に取れるものは取り切りました。ここから先は AI 層です。**

未決として残る 2 件は HtmlDiff。数には含まれません。

## yaf のビルドエラー 154 -> 133(互換層の穴 7 種)

mojo が AI 層待ちになったので、次に大きい数を見ます。yaf の 154 は下限値ではなく実数です。
エラーコード別で数えると、上位は「互換層にメンバーが無い」でした。

| 追加したもの | 元 | 実エラー |
|---|---|---:|
| `ScriptManager.ScriptResourceMapping` + `ScriptResourceDefinition` | System.Web.UI | 8 |
| `ScriptManager.EnableCdn` / `EnableCdnFallback` / `EnableScriptLocalization` | 同上 | 3 |
| `HttpRuntime.UnloadAppDomain()` | System.Web | 2 |
| `PagedDataSource` | System.Web.UI.WebControls | 1 |
| `MembershipSection` + `ProviderSettings(Collection)` | System.Web.Configuration | 1 |
| `ViewStateException` / `HttpRequestValidationException` | System.Web(.UI) | 2 |
| `WindowsImpersonationContext` | System.Security.Principal(.NET で削除) | 1 |

### 何を「動かす」かを分けた

`ScriptResourceMapping` は**空振りにしていません**。アプリは自分のスクリプトを
名前で登録して名前で取り出します(YAF の ScriptsLoaderModule が `forumExtensions` を
登録し、PageElementRegister がそれを書き出す)。登録を忘れる実装だと
タグが出力されず、**ページの挙動が変わります**。無いのはフレームワーク自身の
スクリプトのほうで、変換後のページはどれも読み込みません。

`PagedDataSource` も同じで、こちらは**本当にページングします**。
全件返す実装にすると、どのページにも全行が出ます。

逆に `WindowsImpersonationContext` は**何もしません**。.NET は
`WindowsIdentity.Impersonate()` を `RunImpersonated`(コールバック形式)に
置き換えており、スコープを保持して finally で `Undo()` する書き方は
決定論的には書き換えられません(YAF の背景タスクのようにフィールドに持って
別の場所で戻すこともある)。なりすましは Windows 専用で、変換後のアプリは
それを行いません。`Undo()` が何も戻さないのは、何もしていないからです。

`HttpRuntime.UnloadAppDomain()` も同じ理由で何もしません。
**ホストを落として再現することはしません** — それは他の全ユーザーの回線を
切ることで、WebForms の呼び出しもそこまではしませんでした(排出してから落とす)。

| | 変更前 | 変更後 |
|---|---:|---:|
| **yaf ビルドエラー** | **154** | **133** |
| 6 コーパス合計 | 281 | 260 |

他 5 コーパスは完全に一致。

## 互換層の穴をもう 12 種 — yaf 133 -> 105

同じ調子で yaf のエラーコード別の上位を順に潰しました。

| 追加したもの | 何をするか |
|---|---|
| `HttpContext.ApplicationInstance` | `CompleteRequest()` に届く(YAF はアバターと添付をこれで返す) |
| `Response.Output` | `Write` に委譲する TextWriter |
| `Response.StatusDescription` / `SetCookie` | 書いたとおりに保持 |
| `HttpCookieCollection.Count` / `[int]` / `Clear` | 添字ループが通る |
| `Request.Files` + `HttpFileCollection` | **空**。Blazor の投稿は multipart ではない |
| `Session.IsNewSession` / `Add` | 空かどうか |
| `Page.Response` を public に | WebForms でも public |
| `Literal : ITextControl` | WebForms でもそう |
| `DropDownList.Text` / `AutoPostBack` | Text は**選択値**(キャプションではない) |
| `GridView.Items` + `DataGridItem.FindControl` | DataGrid 名義の同じ行 |
| `IWebFormsControl.DesignMode` / `HasControls()` / `Focus()` / `Unload` | 両系統に届く |

### 気をつけた点

`DropDownList.Text` は**キャプションではありません**。WebForms の ListControl では
Text は選択値の読み書きで、だからコードビハインドは `ddl.Text = savedValue` と書いて
選択を復元します。ここを別のものに割り当てると、リストが「選ばれている」と
表示する項目が黙って変わります。

`GridView.Items` は `Rows` と**同じ行**を DataGrid の名前で返します。
各 `DataGridItem` は `FindControl` を行に転送します。転送しないと、
キャストは成功して検索は全部 null になり、**ページは描画されて何も保存しない**という、
コンパイルが通らないより悪い状態になります(YAF の EditLanguage がこの形)。

`Request.Files` は空です。Blazor のページは InputFile で回線越しに上げるので
multipart のポストは無く、返すべき投稿ファイル集合が存在しません。
ループするハンドラがコンパイルでき、何も見つけない — 実際に何も無いので正しい答えです。

`Focus()` は `WebFormsControlBase`(Blazor コンポーネント側)にしかなく、
`LegacyWebControl` 由来の移植コントロールからは呼べませんでした。
`IWebFormsControl` に既定実装で移し、両系統に届くようにしています。

| | 変更前 | 変更後 |
|---|---:|---:|
| **yaf ビルドエラー** | **133** | **105** |
| 6 コーパス合計 | 260 | 232 |

他 5 コーパスは完全に一致。

## 既定実装はクラスからは呼べない — yaf 105 -> 97

前節で `IWebFormsControl` に `Focus()` / `HasControls()` / `DesignMode` / `Unload` を
**既定実装**で足しました。ところが yaf の `ThemeButton.Focus` エラーは消えませんでした。

C# の既定インターフェース実装は、**インターフェース型の参照からしか呼べません**。
クラスが実装していても、そのクラス型の変数からは見えません。
`LegacyWebControl` 由来の移植コントロールは全部これに当たっていました。
つまり、足したその機能のために足したものが、そこには届いていなかったわけです。

同じメンバーを `LegacyWebControl` にも実体として置きました。

### Page はコントロールである

WebForms の `Page` は `Control` を継承します。移植されたヘルパーはそれ前提で書かれていて、
`Control` を受け取って `ResolveUrl` / `HtmlEncode` / `FindControl` を呼び、
呼び出し側はページを渡します。互換層の `Page` が `IWebFormsControl` を実装していなかったので、
ページからの `Utils.HtmlEncode(this)` も、`IWebFormsControl` の拡張メソッドも通りませんでした。

`Page` に `IWebFormsControl` を実装させ、`ClientID` / `Visible` / `Enabled` /
`CssClass` / `Attributes` / `Controls` を足しました。`Page` プロパティだけは
明示実装です — C# は**囲む型と同名のメンバーを禁止**しているので。

| | 変更前 | 変更後 |
|---|---:|---:|
| **yaf ビルドエラー** | **105** | **97** |
| 6 コーパス合計 | 232 | 224 |

他 5 コーパスは完全に一致。パリティ 30/30、bUnit 30/30。

## TextBox.TextMode を文字列から enum に — yaf 97 -> 87

yaf の最大の残りは `CS0029: 'TextBoxMode' を 'string' に変換できません`(実 10 件)でした。
原因は互換層の `TextBox.TextMode` が **string** だったことです。
System.Web では `TextBoxMode` 列挙で、コードビハインドが
`textBox.TextMode = TextBoxMode.Number` と書くのは**それ以外に書きようがない**からです。

これは両面で壊れていました。

1. **コンパイル**: 列挙を string に代入できない。
2. **描画**: レンダラは `TextMode == "MultiLine"` と `== "Password"` の
   2 つの文字列比較しか見ておらず、**HTML5 のモードは全部 `type="text"`** でした。
   WebForms 4.5 が Email / Date / Number を足したのは、ブラウザに日付ピッカーや
   数値スピナーを出させるためです。`type="text"` に落とすのは見た目の差ではなく、
   **設定した機能をコントロールが失う**ことです。

enum にし、各モードを WebForms と同じ input type に対応付けました
(Phone は `tel`、DateTimeLocal は `datetime-local`、DateTime は `datetime` —
どのブラウザも実装していませんが 4.8 が出すのがこれです)。

### .aspx は大文字小文字を区別しない(2 度目)

これで be が 0 -> 1 に悪化しました。BlogEngine は `TextMode="multiline"` と
**小文字で**書いていて、WebForms は黙って `TextBoxMode.MultiLine` に対応付けます。
変換器は属性値をそのまま列挙メンバー名として出していたので
`TextBoxMode.multiline` になり、そんなメンバーはありません。

テンプレートタグのときと同じ直し方です。**列挙自身にメンバー名を聞き**、
大文字小文字を無視して一致させ、**列挙の綴りで出力**します。
一致しなければ何も出さず属性は残差として報告します(名前を創作しない)。

| | 変更前 | 変更後 |
|---|---:|---:|
| **yaf ビルドエラー** | **97** | **87** |
| 6 コーパス合計 | 224 | 214 |

他 5 コーパスは完全に一致。パリティ 30/30、bUnit 30/30。

### 回帰ゲートが本当の不具合を捕まえた

この変更で be の回帰ゲートが 1 件差分を出しました。記録済み DOM と比べると:

```diff
 "txtMessage": {
-  "Tag": "input",
-  "type": "text",
-  "class": "form-control"
+  "Tag": "textarea",
+  "class": "form-control",
+  "rows": "5",
+  "cols": "30"
 }
```

BlogEngine の問い合わせフォームのメッセージ欄は `TextMode="multiline"` です。
WebForms は `<textarea rows="5" cols="30">` を描画します。
**変換後はこれまで 1 行の `<input type="text">` でした。**
レンダラの比較が `TextMode == "MultiLine"` で、
小文字の `"multiline"` はどちらの分岐にも当たらなかったからです。

記録済みスナップショットのほうが間違っていた、ということです。
ゲートは「元アプリと同じ」を測るものではなく「前回の変換結果と同じ」を測るものなので、
**差分が出たこと自体が正しい動作**です。理由が説明できるので記録し直しました。

## コントロールを ref で渡せるようにした — yaf 87 -> 71

残っていた `CS0206 参照を返さないプロパティを out / ref に使えません` は
**16 件**、全部 YAF の管理画面 Settings でした。

```csharp
SetSelectedOnList(ref this.Culture, boardSettings.Culture);
```

WebForms のデザイナファイルはコントロールを**フィールド**で宣言するので、
これは普通に書けます。変換器はコントロールを**プロパティ**で生成します —
Blazor の `@ref` は初回描画の後でしか代入されないので、それ以前にも
使えるインスタンスを返すのがプロパティの役目だからです。
C# はプロパティを ref で取れません。

**どちらも間違っていません。**間違っていたのは「両立しない」という前提のほうです。
ref 引数は「読んで、呼ばれた側に差し替えさせて、書き戻す」であり、
ローカル変数で書けます:

```csharp
var __ref_Culture = this.Culture;
SetSelectedOnList(ref __ref_Culture, value);
this.Culture = __ref_Culture;
```

C# が ref 引数に対してやっていることを、そのまま書き下したものです。

**文全体が呼び出しのときだけ**書き換えます。大きな式の中に入れ子になった呼び出しには
書き戻しを置く場所がなく、無理に作れば評価順が変わります。そういう箇所は
そのまま残すので、**エラーが該当行を指したまま**になります。

同じ名前を 1 つの呼び出しで 2 回渡しても**ローカルは 1 つ**です — 元も 1 つの
フィールドでした。`out` でも古い値を読みます。C# の `out` は読みませんが、
**プロパティの getter が pending インスタンスを作る**ので、読まないと
呼ばれた側の代入が null に当たります。

| | 変更前 | 変更後 |
|---|---:|---:|
| **yaf ビルドエラー** | **87** | **71** |
| 6 コーパス合計 | 214 | 198 |

あわせて 2 つ:

- `PageHeaderShim` を `HtmlHead` の派生にしました。WebForms の `Page.Header` は
  `HtmlHead` で、YAF は `Page.Header ?? someHtmlHead` と書きます。
  **同じものの宣言が 2 つある**という、この変換器が何度も学び直している間違いで、
  それぞれに Title と Controls があり、何もつないでいませんでした。
- `AppSettings` から `NameValueCollection` への暗黙変換。WebForms の AppSettings は
  NameValueCollection そのもので、移植コードは丸ごと代入します。変換は**コピー**です
  — 受け取った側が書き換えてもアプリの設定に返らないほうが正しい。

他 5 コーパスは完全に一致。パリティ 30/30、bUnit 30/30。

## 囲む名前空間に隠された名前を書き下す — yaf 71 -> 66

YAF の `HttpRuntimeCache` は `YAF.Core.Services.Cache` にいて、
`Cache.NoSlidingExpiration`(= `System.Web.Caching.Cache`)と書いています。
**元ではコンパイルが通ります。** C# は名前空間宣言の**自前の using を先に**見るからで、
ファイルは `namespace YAF.Core.Services.Cache;` の**内側**に using を書いています。

変換はその using をファイル先頭に持ち上げます。そこでは囲む名前空間より**後ろ**なので、
`Cache` は兄弟名前空間 `YAF.Core.Services.Cache` に当たり、
そこには `NoSlidingExpiration` がないので死にます。

別名では直りません — 元が通ったのと同じ理由で、名前空間のメンバーは
どの階層でも using 別名より先に見られます。**名前を書き下すしかありません。**

条件は 3 つ、全部確かめられます。

1. その先頭名が囲む連鎖を通じて**本当に名前空間に当たる**こと。
2. **その名前空間に後半の名前の型が無い**こと(なら元の意味ではありえない)。
3. **ちょうど 1 つの import が**その先頭名の**型**を宣言していること。

### 最初の版は壊した(71 -> 75)

`Field.Index.NO` が `System.Index.NO` になりました。`System.Index` は実在する
BCL の型なので、**決まったように見えて意味は別物**になります。2 つ直しました。

- **ファイルが宣言している名前は触らない**。`Index` は `Field` の入れ子型で、
  入れ子型はどの import よりもどの名前空間よりも強い。
- **BCL は答えにしない**。`using System;` はどのファイルにもあり、System は
  数百の型を宣言しています。この処理が救うのは**変換が動かした名前**だけなので、
  アプリ自身の型と互換層の型に限ります。

| | 変更前 | 変更後 |
|---|---:|---:|
| **yaf ビルドエラー** | **71** | **66** |
| 6 コーパス合計 | 198 | 193 |

あわせて、強い型付きテンプレートの**裸の `Item`** を書き換えるようにしました。
正規表現が `Item.` と末尾のドット込みで書かれていたので、
`CommandArgument="<%#: Item %>"` のような**まるごと使う**形が素通りしていました。
`ItemType="System.String"` なら item は文字列で、文字列は
デリファレンスされるより丸ごと使われるほうが多い(YAF の OpenAuthProviders は
1 つのタグで 3 回そう書いています)。ドットは置換側へ移しました。

他 5 コーパスは完全に一致。

## 省略可能な引数は省略可能なまま — yaf 66 -> 60

`CS7036 必要なパラメーターに対応する引数がありません` が 6 件。
呼び出し側は**全部 YAF の元のコードのまま**で、間違っていたのはスタブでした。

```csharp
// 元
public static string SelectForumsLoadJs(
    string forumDropDownId, string placeHolder, bool forumLink, bool allForumsOption,
    string selectedHiddenId = null, string topicsSelectJs = null)
```

除外された型のスタブを生成するとき、**既定値を落としていました**。
6 か所の呼び出しは全部 5 引数で書かれているので、
落とした瞬間に全部 CS7036 になります。**スタブが呼び出し側のエラーを作っていた**わけです。

既定値は**リテラルのときだけ**そのまま写します。それ以外は移植されなかった型を
名前に含むことがあり、**コンパイルできることが唯一の存在理由のファイル**に
解決できない式を置くことになります。その場合は `= default` で、
省略可能であることは保ち、値が失われたことは正直に示します。

| | 変更前 | 変更後 |
|---|---:|---:|
| **yaf ビルドエラー** | **66** | **60** |
| 6 コーパス合計 | 193 | 187 |

他 5 コーパスは完全に一致。

## Repeater.Items は行であってデータではない — yaf 60 -> 56

`item.FindControlAs<Label>("GroupID")` が「object に FindControlAs は無い」で
落ちていました。`this.UserGroups.Items[i]` の型が `object` だったからです。

互換層の `Repeater.Items` は **DataItem の一覧**を返していました。
自然に読めますが、WebForms の意味ではありません。
`RepeaterItem` は**コントロール**で、移植コードは Items を歩いて各行の
`FindControl` を呼び、描画した入力を読み取ります
(YAF の EditUsersGroups はグループごとのチェックボックスとラベルをこうして集めます)。
データ項目の一覧を相手にすると、**見つけるものが何もありません**。

行を返すようにしました。データは `Items[i].DataItem` と一歩先にあり、
WebForms と同じ位置です。

| | 変更前 | 変更後 |
|---|---:|---:|
| **yaf ビルドエラー** | **60** | **56** |
| 6 コーパス合計 | 187 | 183 |

他 5 コーパスは完全に一致。パリティ 30/30、bUnit 30/30。

## 互換層の穴をさらに 6 種 — yaf 56 -> 51

| 追加したもの | 何をするか |
|---|---|
| `LegacyWebControl.Parent` / `Site` | **クラスから呼べる実体**として(既定実装では届かない、2 度目) |
| `Response.RedirectLocation` | 代入で**実際に遷移する** |
| `Response.AppendCookie` | 同名は上書き(ブラウザも最後の Set-Cookie で同じ結果) |
| `ScriptManager.Scripts` | 集めるが出力はしない |
| `ScriptManager.GetCurrent()` が **null を返さない** | 元では null ではなかったから |
| `ConnectionStringSettings(name, cs, provider)` | System.Configuration にある 3 引数形 |

`RedirectLocation` を「持つだけ」にはしませんでした。移植コードは
**ユーザーをどこへ送るか言っている**のであって、回線がその場に留まるのは
「ページが動かなかった」ようにしか見えません。

`ScriptManager.GetCurrent()` は null を返していました。4.8 では、これを呼ぶページは
ScriptManager を持っています(YAF のマスターページは持っている)。
だから null は「WebForms と同じ」ではなく、**元がオブジェクトだった場所での
NullReferenceException** です。

| | 変更前 | 変更後 |
|---|---:|---:|
| **yaf ビルドエラー** | **56** | **51** |
| 6 コーパス合計 | 183 | 178 |

他 5 コーパスは完全に一致。パリティ 30/30、bUnit 30/30。

## スコープ外のまま復元したファイルを別枠にした(**測定の変更であって修正ではない**)

まずこれを先に書きます。**この節の数字の下がり方は、何も直していません。**
分類を直しただけです。dnn 67 -> 19、n2 54 -> 13、合計 178 -> 89。

### なぜ

dnn のカウント上 67 件を 1 件ずつ見ると、**50 件近くが 6 つのファイルの中**にありました。
`DnnHtmlHelper` `DnnHelper` `AuthorizeAttributeBase` `DnnUrlHelper` `IDnnController`
`AuthFilterContext` — 全部 **MVC のコード**で、全部**一度は移植対象外にして、
そのあと復元した**ファイルです。

復元パスはこういう判断をします: あるファイルを別フレームワークのコードとして除外したが、
**移植されるファイルがその型を使っている**。除外すると連鎖でそちら側が落ちる。
連鎖のほうが高くつくので残す。そして変換器はその場で残差に書いています —
**「このファイル内には未解決の型が残ります」**。

つまり中の CS0246 は、ここで発見された欠陥ではありません。
**支払うと決めて、報告済みの代償**です。それを変換器の欠陥として数えるのは、
「誰もパッケージを決めていない同梱 DLL」を数えるのと同じ間違いで、
**数が多いので本物を埋めてしまいます**(dnn では 67 件中 48 件)。

### 隠していないこと

未決の依存と**まったく同じ扱い**です。全部レポートに出ますし、ファイル別の件数表も出ます。
コンソールも 1 行言います。**総数は見えたまま**で、変換器の課題として数える枠から
外しただけです。

決め方も同じ原則です — **推測しません**。変換器は復元した瞬間にファイル名を
`restored-out-of-scope-files.txt` に書きます。その判断を知っているのはその瞬間だけで、
あとから推測するとコード規則やファイル名のパターンを当てにすることになります。

| | 変更前 | 変更後 | 別枠 |
|---|---:|---:|---:|
| dnn | 67 | **19** | 48 |
| n2 | 54 | **13** | 41 |
| 6 コーパス合計 | 178 | **89** | 89 |

be / mojo / yaf / wt は 1 件も動きません(復元されたファイルが無い)。

これで dnn と n2 の残り(19 / 13)は**本当に変換器の課題**になりました。

## ライフサイクルイベントと ISite — yaf 51 -> 37

### Load / Init / PreRender は「宣言だけ」にしなかった

WebForms は仮想メソッドと**イベントの両方**を出していて、移植コードは場面で使い分けます。
コントロール自身は `OnLoad` を override し、それを持つページは
`themeButton.Load += ...` と書きます。互換層には**仮想メソッドしかありません**でした。

仮想メソッドから raise するようにしました。**購読側と override が同じ瞬間を見ます**。
空のイベントを宣言するだけなら簡単ですが、それは半分で、しかも**間違ったほうの半分**です
— ハンドラはコントロールが文字列を受け取る場所だからです。

### Site は object ではなく ISite

`currentControl.Site is { DesignMode: true }` — これが YAF の全コントロールの入口にあります。
互換層の `Site` は `object` だったので、このパターンは静的型にメンバーが無く成立せず、
**実行時には誰も読まないプロパティのせいで拡張クラス全体が落ちていました**。

`ISite` を足して `Site` の型にしました。中身は null(デザイナはいない)なので
パターンは false になり、それは **4.8 で「実行中の」コントロールが返す答え**と同じです。

### LoadControl の戻り値が基底によって違った

`Page.LoadControl` は `IWebFormsControl`、`WebFormsUserControl.LoadControl` と
`WebFormsLayout.LoadControl` は `object` を返していました。
**1 つのメソッドが、どの基底を継承したかで 2 つの答えを返す**状態です。
YAF の Forum.cs はユーザーコントロールから `this.Controls.Add(this.LoadControl(path))` と
書いていて、`Controls` はコントロールを取るのに object を渡されて落ちていました。

### そのほか

- `AttributeCollection.Render(writer)` — 自分で描画するコントロールが、手で書いた属性の
  あとに呼びます。無いとコンパイルが落ちるだけでなく、**expando 属性が全部消えます**。
- `ClientScriptManager.RegisterForEventValidation` — 回線にポストバックは無く、
  これが防いでいた経路自体が存在しないので、受け取って何もしません。
- `DesignMode` / `HasControls()` を Blazor コンポーネント側の基底にも実体で。

| | 変更前 | 変更後 |
|---|---:|---:|
| **yaf ビルドエラー** | **51** | **37** |
| 6 コーパス合計 | 89 | 75 |

他 5 コーパスは完全に一致。パリティ 30/30、bUnit 30/30。

## 互換層の細かい穴 6 種 — yaf 37 -> 31

| 追加 / 修正 | 理由 |
|---|---|
| `ListControl.Items` を `ListItemCollection` に | `FindByValue` / `FindByText` / `Add(string)` が要る。素の List は**最初の `Items.FindByText(...)` まで**通る |
| `HttpContext.Application` | **DI が配っているのと同じインスタンス**を返す(別物を返すと Application 状態の意味が消える) |
| `HttpCookieCollection()` を public に | System.Web でも public。**アプリ側のコンパイルエラーになっていた** |
| `Server.Execute` | 受け取って何もしない。**遷移させるほうが悪い** — Execute は現在のページを離れない |
| `ConnectionStringSettings` → System.Configuration への暗黙変換 | インストーラが本物の設定に書き足す。**コピー**であって同一物の主張ではない |
| `HttpApplicationStateWrapper` | HttpContextWrapper と同じ、境界のためのアダプタ |

| | 変更前 | 変更後 |
|---|---:|---:|
| **yaf ビルドエラー** | **37** | **31** |
| 6 コーパス合計 | 75 | 69 |

他 5 コーパスは完全に一致。パリティ 30/30、bUnit 30/30。

### この turn の通し

| コーパス | turn 開始 | 現在 |
|---|---:|---:|
| be | 0 | 0 |
| mojo(実ビルド) | 43 | **8** |
| yaf | 154 | **31** |
| dnn | 67 | **19** |
| n2 | 54 | **13** |
| wt | 0 | 0 |
| **合計** | **296** | **69** |

dnn と n2 の下がり幅の大半(89 件)は**分類の修正**で、直したものではありません
(「スコープ外のまま復元したファイル」の節を参照)。

## global using を 3 か所に届けた — yaf 31 -> 21

同じ根っこの欠陥が 3 か所に出ていました。**global using はファイルの中に書いていない**
ので、ファイルを読んで import を集める処理から全部こぼれます。

### 1. スタブが「解決できない」と判断していた

除外型のスタブは、**シグネチャの型が全部解決できるメンバーだけ**を出します。
YAF の `IAspNetRoleManager` は `AspNetRoles` を返しますが、その型は 2 つ隣の
名前空間にあり、**global using で届いていました**。ファイル内の using だけを見ると
解決できないので、4 メンバー中 3 つが落ち、
**4 つ全部を呼んでいる `AspNetRolesHelper`(ちゃんと移植されたファイル)にエラーが出ます**。

しかも YAF は**プロジェクトごとに GlobalUsings.cs を持ちます**(9 個)。
`YAF.Types` の型は `YAF.Types` のリストで隣人を解決するので、
Web プロジェクトのリストでは足りません。移植した全プロジェクトの和集合を使います。

和集合は本来より広く、**同名の型が 2 つの名前空間にあると誤って解決しうる**のは本当の代償です。
解決が「実在する名前しか受け付けない」ことで抑えられてはいます。
そしてこれを使うのはスタブのシグネチャだけで、そこでは代替案のほうが悪い —
メンバーごと落ちて、エラーが呼び出し側に移ります。

### 2. .razor に届いていなかった

マークアップの式は**ページがコンパイルされたときの import** で解決されます。
コードビハインド側には既に届けていましたが、マークアップ側は
.ascx と**コードビハインドのファイル内 using** だけでした。

### 3. スタブがある名前空間を「型が残らない」と消していた

`<%@ Import Namespace="YAF.Core.Context.Start" %>` が
**「移植後に型が残らないため除去しました」**で消えていました。
ところが `ExcludedTypeStubs.g.cs` にはその名前空間の `WebApiConfig` が**います**。
スタブが入る前は正しかった判断で、**入ってからはずっと間違っていました**。

除外されたソースから実際に出るスタブの名前空間を数えて引くようにしました
(全除外名前空間と同じだと決め打ちにしない — スタブにできない名前空間は
「消えた」のままであるべきなので)。

| | 変更前 | 変更後 |
|---|---:|---:|
| **yaf ビルドエラー** | **31** | **21** |
| 6 コーパス合計 | 69 | 59 |

他 5 コーパスは完全に一致。パリティ 30/30、bUnit 30/30。

### あわせて: インターフェースのメンバーは修飾子を持たない

スタブ生成は `public` / `internal` / `protected` が付いているメンバーだけを出していました。
**インターフェースのメンバーは修飾子を持たず、定義により public** です。
「public と書いていない = public でない」と読んだために、
**スタブ化されたインターフェースが全部空になっていました**。

## 基底クラスが消えたことの言い直しを数えない — dnn 19 -> 14

`HttpCachePolicy` を互換層に足した(1 件)ほかは、**測定の修正**です。

dnn の残り 19 件のうち 16 件が 5 ファイルに集中していて、中身は
`CS0534 継承抽象メンバーを実装しません` と
`CS0115 オーバーライドする適切なメソッドが見つかりません` でした。

`DnnBodyProvider` は `ClientDependency.Core` の
`WebFormsFileRegistrationProvider` を継承します。このライブラリは .NET Framework 向けで、
公開 API に System.Web の型が出てきます。コンパイラはそれを
`CS7069 'Control' は System.Web で定義されていますが見つかりません` と言い、
**それは既に別枠に数えています**。

その直後に出る「基底の契約を満たしていない」は、**同じ事実の言い直し**です。
基底が使えないのだから、派生クラスにできることは何もありません。
ところがメッセージには DNN 自身の型名しか出てこないので、変換器の欠陥に見えます。

継承の契約に関するコードだけを、**同じファイルが既に依存の不足を報告している場合に限って**
同じ枠に入れます。基底クラスは派生クラスと同じファイルにいるので、
「このファイルは X の型を見つけられなかった」と「このファイルは基底の契約を満たせない」は
**1 つの事実**です。同じファイルの**それ以外のエラーは数えたまま**にします —
依存が足りないことは、そこにある他の間違いの言い訳にはなりません。

| | 変更前 | 変更後 |
|---|---:|---:|
| **dnn ビルドエラー** | **19** | **14** |
| 6 コーパス合計 | 59 | 54 |

他 5 コーパスは完全に一致。パリティ 30/30、bUnit 30/30。

## スタブの `Control` が別の型になっていた — n2 13 -> 11

移植コードでは `System.Web.UI.Control` は `IWebFormsControl` になります
(互換層は Blazor コンポーネントと描画型コントロールの 2 系統に分かれていて、
その共通項がこのインターフェースだからです)。
**除外型スタブだけがこの対応付けをしていませんでした。**

n2 の `AbstractDisplayableAttribute` は移植後
`AddTo(ContentItem, string, IWebFormsControl)` を宣言します。
その派生クラスのスタブは `AddTo(ContentItem, string, Control)` と書きました —
互換層には `Control` という**クラスも実在する**ので、解決自体は成功し、
**存在しないメソッドの override** が出来上がります。
エラーはスタブを指し、ずれている事実そのものは出てきません。

移植コード側と同じ規則をスタブの型解決にも入れました。
**アプリ自身が `Control` を宣言していればそちらが勝つ**のも同じです。

| | 変更前 | 変更後 |
|---|---:|---:|
| **n2 ビルドエラー** | **13** | **11** |
| 6 コーパス合計 | 54 | 52 |

他 5 コーパスは完全に一致。

## 移行の残りかすを「事実」から「タスク」に変えた

`AssemblyTypeMigration` が対応付けられなかった型は、これまで **1 行にまとめて**
報告していました。

```
MetaDataExtractor の型 50 件は置き換え先に同名のものがありません:
  com.codec.jpeg.JPEGDecodeParam, com.drew.metadata.AbstractTagDescriptor, ... ほか
```

これは**事実であってタスクではありません**。mojoPortal がこの 50 件のうち実際に
参照しているのは **2 件**です。残り 48 件は、アプリが一度も触っていないライブラリの
説明で、**大事な 2 件をその中に埋める**のがレポートの読まれなくなり方です。

3 つ直しました。

### 1. 使っている型だけを残差にする

移植コードが参照している型だけを残差に出します。修飾名でも、
名前空間を import した上での裸の名前でも拾います
(`using MetadataDirectory = com.drew.metadata.AbstractDirectory;` は前者、
その周りのコードは別名を使うので)。

mojo では **181 件の未対応型 → 5 件の実タスク**になりました。
残りは件数だけ Info で言います(隠さない)。

### 2. 参照しているファイルを書く

### 3. **置き換え先が実際に宣言している型の一覧**を添える

これが一番効きます。`com.drew.metadata.AbstractDirectory` に対して、
`com.drew.metadata` の**残りの型が着地した名前空間**が宣言している型を並べます:

```
MetadataExtractor.Age, MetadataExtractor.Directory, MetadataExtractor.DirectoryExtensions,
MetadataExtractor.ErrorDirectory, MetadataExtractor.Face, ...
```

**ここでは何も決めません。**似ているから決める、というのはこの変換器が拒否している
ことそのもので、それが AI 層の仕事です。ただし
**「アセンブリが本当に宣言しているこの一覧から選べ」は、
「com.drew.metadata.AbstractDirectory が何になったか思い出せ」とは別の問題**です。
前者は照合で、後者は記憶です。

名前空間が散らばる場合(Lucene の `Index` は Index / Codecs / Util など十数か所へ)
**投票の多い名前空間から順に**並べ、上限で切ります。全部貼ると誰も読まない一覧になり、
それはこのレポートが直前まで陥っていた状態です。

| | 変更前 | 変更後 |
|---|---:|---:|
| mojo の未対応型の残差 | 3 件(合計 181 型を要約) | **5 件(実際に使われている型ごと)** |
| mojo 総残差 | 92 | 94 |

残差が 2 増えるのは、**曖昧な 3 件が具体的な 5 件になった**からです。
ビルドエラーは 6 のまま(下限値)、他 5 コーパスは完全に一致。

## ボタンの基底が Text を持っていなかった — yaf 21 -> 16

`Button` / `LinkButton` / `ImageButton` を基底に持つ移植コントロールは
`LegacyWebControl` に付け替えられていました。これは**ライフサイクルと描画の仮想メソッド
しか持ちません**。YAF の `CollapseButton` は `LinkButton` の派生で、
`OnPreRender` で `this.Text` に自分のアイコン HTML を入れて描画します。
その `Text` が無くなっていました。

`LegacyButton` を足して、`Text` / `CommandName` / `CommandArgument` /
`PostBackUrl` / `OnClientClick` / `CausesValidation` / `ValidationGroup` /
`Command` を持たせました。`LegacyListControl` のときとまったく同じ理由です —
**基底はそのコントロールが「何であるか」を持っていないといけない**。
どのコントロールにも共通のものだけでは足りません。

クリック面は宣言だけで、発火しません。発火させていたのはポストバックで、
描画ホスト上の legacy コントロールには Blazor 側のイベント配線がありません
(そのことはコントロールごとの残差に出ます)。

### そのほか 4 つ

| 追加 | 理由 |
|---|---|
| `AttributeCollection(StateBag)` | `new AttributeCollection(this.ViewState)` はカスタムコントロールの定石 |
| `RegisterForEventValidation(PostBackOptions)` | オーバーロード違い |
| `HttpBrowserCapabilities.Version` / `MajorVersion` / `MinorVersion` | User-Agent から読む。**空文字のほうが静かな嘘** |
| `WindowsIdentity.Impersonate()` 拡張メソッド | 新 API はコールバック形式で**形が対応しない**(スコープを別のメソッドで戻す書き方がある) |

| | 変更前 | 変更後 |
|---|---:|---:|
| **yaf ビルドエラー** | **21** | **16** |
| 6 コーパス合計 | 52 | 47 |

他 5 コーパスは完全に一致。パリティ 30/30、bUnit 30/30。
