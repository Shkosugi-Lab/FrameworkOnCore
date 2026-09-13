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
