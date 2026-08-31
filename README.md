# WebForm2Blazor

.NET Framework (ASP.NET WebForms) アプリを .NET (Blazor Server) へ「限りなくストレートコンバージョン」する変換ツール。

## 設計方針

「AI に丸ごと翻訳させる」のではなく、4 層構成で決定性と汎用性を両立する。

1. **解析・棚卸し** — 変換対象の分類とレポート
2. **決定的変換(本リポジトリの中核)** — 構文木ベースのルール変換
3. **AI 変換** — 決定的変換で処理しきれなかった「残差」だけを担当(未実装)
4. **検証ループ** — ビルド → bUnit → 実ブラウザ

ストレートコンバージョンの鍵は **WebForms 互換ランタイム + 互換コンポーネント**
(`src/WebForm2Blazor.Components`)。WebForms と同じ API 名・シグネチャを提供するため、
**コードビハインドのメソッド本体を書き換えずに移植できる**。

## 構成

```
src/
  WebForm2Blazor.Components/   WebForms 互換ランタイム + Blazor コンポーネント
    Runtime/                     ViewState / Session / Request / Response / DataBinder / 基底クラス
    Compat/                      ConfigurationManager シム
    *.razor                      TextBox, Button, Label, Literal, HyperLink, LinkButton,
                                 CheckBox, Panel, DropDownList, GridView, Repeater,
                                 RequiredFieldValidator, ValidationSummary
  WebForm2Blazor.Converter/    変換ツール本体 (CLI)
    Parsing/                     ASPX パーサー(手書きの構文解析。正規表現ではない)
    Mapping/                     コントロール・属性マッピング表(ここを育てて汎用化する)
    Emit/                        構文木 → Razor マークアップ、プロジェクト骨格生成
    Convert/                     ページ/マスター/ユーザーコントロール変換、
                                 コードビハインド変換 (Roslyn)、Web.config 変換
samples/
  DefaultsProbe/               既定レンダリング適合スイート。対応全コントロールを
                               属性ほぼ未指定で並べたページ。旧ランタイムの実描画を
                               golden-webforms.json(既定値カタログ)として固定し、
                               変換後と属性レベルで照合する。
                               ルール: 変換器にコントロール/プロパティを追加したら必ずここにも足す。
  MasterProbe/                 マスターページと naming container の適合スイート。
                               ContentPlaceHolder / ユーザーコントロールが ClientID に
                               連結するプレフィックスを、旧ランタイムの実描画と照合する。
  HelloWebForms/               最小サンプル(TextBox + Button + Label)
  ProductAdmin/                実践的サンプル(マスターページ、ユーザーコントロール、
                               GridView、Repeater、検証、ViewState、Session、Web.config)
  OrderAdmin/                  複雑サンプル(UpdatePanel/ScriptManager、TemplateField +
                               RowCommand、RadioButtonList、Page_Init/PreRender、
                               複数 ContentPlaceHolder(サイドバー)、QueryString 遷移、
                               ページ遷移をまたぐ Session、全種バリデータの実戦投入)
output/                        変換ツールの出力(生成物・再生成可能)
tests/
  WebForm2Blazor.ConvertedAppTests/  変換後アプリのコンポーネント検証 (bUnit)
tools/
  WebForm2Blazor.BrowserSmokeTest/   実ブラウザ検証 (Playwright + インストール済み Edge)
                                     --auto で変換器の自動生成シナリオを実行(第2層)
  WebForm2Blazor.ParityTest/         変換前アプリとの動作パリティテスト(第3層)
                                     record で正解を記録、verify で突き合わせ
  WebForm2Blazor.PropertyCatalog/    実行中の .NET Framework ランタイムから
                                     コントロールの属性と既定値を採取(net48)
  verify-all.ps1                     全体回帰(変換 → bUnit → ビルド検証 → パリティ)
corpora/                       実在 OSS アプリでの残差計測。fetch.ps1 で取得し
                               convert-all.ps1 でベースライン(expected.json)と比較
```

## 使い方(エンドツーエンド)

```powershell
# 1. 変換
dotnet run --project src\WebForm2Blazor.Converter -- `
  --input samples\ProductAdmin `
  --output output\ProductAdminBlazor `
  --name ProductAdminBlazor `
  --components-ref ..\..\src\WebForm2Blazor.Components\WebForm2Blazor.Components.csproj `
  --port 5090

# 2. ビルド
dotnet build output\ProductAdminBlazor\ProductAdminBlazor.csproj

# 3. コンポーネント検証
dotnet test tests\WebForm2Blazor.ConvertedAppTests

# 4. 起動
dotnet run --project output\ProductAdminBlazor       # → http://localhost:5090

# 5. 実ブラウザスモーク(第2層・自動生成シナリオ。アプリを起動したまま別ターミナルで)
dotnet run --project tools\WebForm2Blazor.BrowserSmokeTest -- `
  --url http://localhost:5090/ --auto output\ProductAdminBlazor\smoke-scenario.json

# 6. 動作パリティ(第3層・ゴールデンマスター)
#    正解は「変換前の WebForms アプリ」の実際の描画から採る。

# 6-1. 旧アプリをビルド(VS Build Tools の MSBuild。bin\ 直下に出力するのが重要)
& "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe" `
  samples\ProductAdmin\ProductAdmin.csproj /p:Configuration=Debug /p:OutputPath=bin\

# 6-2. 旧アプリを IIS Express で起動(別ターミナルで起動したままにする)
& "C:\Program Files\IIS Express\iisexpress.exe" `
  /path:"$PWD\samples\ProductAdmin" /port:8091

# 6-3. 旧アプリから正解を記録 → 変換後アプリと照合
dotnet run --project tools\WebForm2Blazor.ParityTest -- record `
  --url http://localhost:8091/ --scenario samples\ProductAdmin\parity-scenario.json `
  --out samples\ProductAdmin\golden-webforms.json
dotnet run --project tools\WebForm2Blazor.ParityTest -- verify `
  --url http://localhost:5090/ --scenario samples\ProductAdmin\parity-scenario.json `
  --golden samples\ProductAdmin\golden-webforms.json
```

最小サンプルは `--input samples\HelloWebForms --output output\HelloBlazor --name HelloBlazor --port 5080`。

### 参照プロジェクトは自動で決まる

実アプリはページだけで完結せず、基底クラスや業務ロジックを別プロジェクトに置きます。
それらを移植し損ねると、**変換器や互換層の不具合に見える大量の CS0246** になります
(実測: DNN Platform で約 1,100 件、YAF.NET で約 1,600 件)。

これは判断の余地がある話ではなく `.csproj` に書いてあるので、**変換器が入力アプリの
`ProjectReference` を推移的に辿って自動で移植対象にします**。指定は不要です。

機械が決められない 2 つだけ、オプションで指定します。

| オプション | いつ必要か |
|---|---|
| `--project <csproj>` | 入力ディレクトリに `.csproj` が複数あるとき。データベースごとにビルド構成を分ける実装(YAF.NET の `YAF-SqlServer` / `YAF-MySql` / …)が該当。**推測せず何も導出しない**ので明示が要る |
| `--include <dir>` | 同じ型を宣言する排他プロジェクト群から 1 つ選ぶとき。mojoPortal の `mojoPortal.Data.MSSQL` / `MySql` / `pgsql` / `SQLite` が該当。**どれも自動採用しない**(選ぶと配置先データベースを暗黙に決めてしまうため) |
| `--no-derive-includes` | 自動導出を止めて `--include` だけで組みたいとき |

排他プロジェクトを自動判定しているのは、変換器が全ソースを**単一プロジェクトに平坦化**
するためです。MSBuild は別アセンブリとして扱い実行時に 1 つだけ読み込みますが、
平坦化すると共通の型がすべて重複定義になります(mojoPortal で実測 2,822 件、
1 つに絞れば 599 件)。型名を 5 個以上共有するプロジェクト群は代替関係と判定し、
いずれも採用せずレポートで通知します。

`ProjectReference` のうち `OutputItemType="Analyzer"`(Roslyn ソースジェネレータ)は
**移植対象から外します**。ビルド時に動くコードであり、かつ生成される宣言は変換出力に
存在しないため、それに依存するコードはビルドできません。これは変換器の不具合ではないので、
残差レポートが手動移行として明示します。

### 検証は 4 段構え

| 検証 | 生成元 | 検出できるもの |
|---|---|---|
| `dotnet build` | — | コンパイルエラー |
| bUnit テスト | 人が管理 | コンポーネントのロジック(データバインド、検証、イベント) |
| 自動スモーク(第2層) | **変換器が決定的に自動生成**(smoke-scenario.json) | 全ページの表示、全コントロールの存在、全イベントの発火、blazor.web.js 404、SignalR 未接続、未処理例外 |
| パリティ(第3層) | **正解は変換前アプリ自身**(golden.json) | 業務的な挙動の差(表示内容・件数・遷移・検証メッセージの相違) |

- 第2層のシナリオは変換器が構文木から出力するため、**ページを追加しても手書きのテストは増えない**。
- 第3層の正解データは AI でも変換器でもなく**変換前アプリの実際の描画**から採る。
  ID の差(WebForms の `MainContent_txtName` と Blazor の `txtName`)や URL の差
  (`/Edit.aspx` と `/Edit`)はツール側で正規化して吸収する。
- パリティは可視テキスト・テーブル・入力値に加えて、**ID を持つ要素のタグと
  プレゼンテーション属性(border / rules / cellspacing / cols / class / style 等)**も比較する。
  これは「マークアップに書かれない既定レンダリング」(GridView の GridLines=Both が出す
  `border="1" rules="all"` など)の再現漏れを検出するための層。残差レポートも
  コンパイルエラーも「書かれているもの」しか捕捉できないため、既定値の漏れは
  旧アプリの実描画と突き合わせる以外に検出手段がない。実際にこの層が
  GridView の罫線・ValidationSummary の DOM 構造(div>ul)・FormView の
  border-collapse・MultiLine TextBox の cols="20" の4件の既定値漏れを検出した。

### 既定値の網羅保証(DefaultsProbe 適合スイート)

「未設定プロパティの既定値」を体系的に保証する仕組み。既定値をドキュメントや記憶から
列挙すると列挙自体が間違う(ValidationSummary の ForeColor=Red は思い込みで、
実ランタイムでは描画されなかった)ため、**既定値の正解は実行中の .NET Framework
ランタイムから機械的に採取する**:

1. `samples/DefaultsProbe` — 対応全コントロールを属性ほぼ未指定で並べたページ
   (通常表示と検証エラー表示の 2 状態)
2. 旧ランタイム(IIS Express)でレンダリングした実描画を
   `golden-webforms.json` に記録 = **既定値カタログ**
3. 変換後アプリと属性レベルのパリティで照合 → 全コントロールの既定レンダリングが
   「明示的に指定された場合と同じ」であることを機械検証

```powershell
# 適合スイートの実行
& "...\MSBuild.exe" samples\DefaultsProbe\DefaultsProbe.csproj /p:OutputPath=bin\
& "C:\Program Files\IIS Express\iisexpress.exe" /path:"$PWD\samples\DefaultsProbe" /port:8093
dotnet run --project tools\WebForm2Blazor.ParityTest -- record --url http://localhost:8093/ `
  --scenario samples\DefaultsProbe\parity-scenario.json --out samples\DefaultsProbe\golden-webforms.json
dotnet run --project output\DefaultsProbeBlazor    # 変換後(port 5096)
dotnet run --project tools\WebForm2Blazor.ParityTest -- verify --url http://localhost:5096/ `
  --scenario samples\DefaultsProbe\parity-scenario.json --golden samples\DefaultsProbe\golden-webforms.json
```

適用範囲の注意: この仕組みが保証するのは**描画に現れる既定値**。動作系の既定値
(CausesValidation=true 等)は bUnit と動作パリティが担う。ランタイムのバージョンで
既定が変わる場合(4.0 と 4.8 の差など)も、実機から採取するためカタログが正になる。

### プロパティカバレッジ監査(未対応を「使われる前に」列挙)

DefaultsProbe(実描画)を補完する静的な網。3点セットで守備範囲が完成する:

| 仕組み | 手段 | 検出できるもの |
|---|---|---|
| プロパティカタログ | リフレクション(`tools/WebForm2Blazor.PropertyCatalog`、net48) | 実 4.8 ランタイムで各コントロールを生成し、**全プロパティの実行時既定値**を JSON 化 |
| カバレッジ監査 | 変換器 `--coverage` モード | カタログ vs(マッピング表+互換 API)の突き合わせ → **未対応プロパティの事前列挙**(PROPERTY-COVERAGE.md) |
| 使用 API 棚卸し | Roslyn(変換器に組み込み・常時実行) | コードビハインドが触る全コントロール API を列挙し、未対応は**変換時点で残差レポート**へ |

```powershell
# カタログ生成(実ランタイムから採取)→ カバレッジ監査
dotnet build tools\WebForm2Blazor.PropertyCatalog
& tools\WebForm2Blazor.PropertyCatalog\bin\Debug\net48\WebForm2Blazor.PropertyCatalog.exe webforms-property-catalog.json
dotnet run --project src\WebForm2Blazor.Converter -- --coverage webforms-property-catalog.json --output PROPERTY-COVERAGE.md
```

Roslyn を既定値の「値」抽出に使わない理由: WebForms の既定値は
`return (o == null) ? GridLines.Both : ...` のようにゲッターのコード内にあり、
[DefaultValue] 属性は不完全。ソース解析より「実機で new して読む」方が確実で完全。
Roslyn は「アプリが何を使っているか」の解析(使用棚卸し)に充てる。
- シナリオがデータを書き換える場合、record / verify の前にそれぞれアプリを初期状態に戻すこと。
- golden.json は変換のたびに消える output/ ではなく、シナリオと同じ場所に保管するのを推奨。

実証済み: **本物の旧 WebForms アプリ(IIS Express + .NET Framework 4.8)から記録した正解と、
変換後 Blazor アプリが 30 スナップショットすべて一致**(絞り込み・並べ替え・検証エラー・保存→
リダイレクトまで)。設定を意図的に壊した状態では差分を検出することも確認済み。

記録済みの `golden-webforms.json` はリポジトリに含めてあるため、**`verify` を回すだけなら
IIS Express も .NET Framework も不要**です(必要なのは .NET SDK のみ)。旧ランタイムが要るのは
正解を録り直す `record` のときだけです。

### 旧アプリを動かすときの WebForms 定番トラブル(サンプルで対処済み)

| 症状 | 原因と対処 |
|---|---|
| パーサーエラー(日本語が文字化けしてタグが壊れる) | ASP.NET は既定で OS の ANSI コードページ(日本語 Windows は Shift-JIS)で .aspx を読む。UTF-8(BOMなし)のファイルは `<globalization fileEncoding="utf-8" />` を Web.config に追加する |
| 「'jquery' の ScriptResourceMapping が必要」(500) | .NET 4.5+ はバリデータが既定で jQuery を要求する。`<add key="ValidationSettings:UnobtrusiveValidationMode" value="None" />` を appSettings に追加する |
| 拡張子なし URL が 404 | WebForms は物理パス。ParityTest の goto は 404 時に .aspx を付けて自動再試行する |

## 変換の対応関係

### ページ構造

| WebForms | Blazor 変換後 |
|---|---|
| `.aspx` | `Components/Pages/*.razor`(`@page` は物理パスから生成、`Default` は `/` も割当) |
| `.master` | `Components/Layout/*.razor`(`@inherits WebFormsLayout`) |
| `.ascx` | `Components/Controls/*.razor`(`@inherits WebFormsUserControl`) |
| `<asp:ContentPlaceHolder>`(本文) | `@Body`。2 つ目以降は `<SectionOutlet>` |
| `<asp:ContentPlaceHolder>`(`<head>` 内) | ページ側の `<HeadContent>` に変換 |
| `<asp:Content>` | 対応するプレースホルダの内容として展開 |
| `<%@ Register Src=... %>` + `<uc:X />` | ユーザーコントロールのコンポーネント参照 |
| `.ascx.cs` の公開プロパティ | `[Parameter]` を自動付与 |
| `Web.config` の appSettings / connectionStrings | `appsettings.json` |
| コードビハインドでない `.cs` | using のみ差し替えてそのまま移植 |

### コードビハインド(本体は無変更)

| WebForms | Blazor 変換後 |
|---|---|
| `Page_Init` → `Page_Load` → `Page_PreRender` | `OnAfterRender(firstRender)` から順に呼び出し(本体は無変更) |
| `Page_PreRender` の「イベント後に毎回実行」 | 互換コントロールがイベント処理完了を通知し、`OnPreRenderCompat` 経由で毎回再実行 |
| `IsPostBack` | 互換プロパティ(初回 Page_Load 完了後に true) |
| `ViewState["x"]` | 互換 `StateBag`(サーキットが状態を保持するため意味論が一致) |
| `Session["x"]` | Cookie のセッション ID をキーにしたストア(**ページ遷移=サーキット再生成をまたいで維持**) |
| `Request.QueryString["id"]` | `NavigationManager` から解決する互換シム |
| `Response.Redirect("~/Edit.aspx?id=3")` | URL を `/Edit?id=3` に解決して `NavigateTo` |
| `Page.IsValid` / バリデータ | 互換の検証パイプライン(`CausesValidation` も再現) |
| `ConfigurationManager.AppSettings["x"]` | `appsettings.json` を読む互換シム(型エイリアスで取り込み) |
| `System.Web.UI.Page` 継承 | `WebFormsPage` 継承(`@inherits` は .razor 側) |

### コントロール

| WebForms | 互換コンポーネント |
|---|---|
| TextBox / Button / Label / Literal | 同名プロパティ・同シグネチャのイベント |
| HyperLink / LinkButton / Panel / CheckBox | NavigateUrl は `~/Foo.aspx` のまま解決 |
| DropDownList + ListItem | `Items` / `SelectedValue` / `DataSource` + `DataBind()` |
| GridView + BoundField | `DataFormatString` / `AutoGenerateColumns` / **`AllowPaging` + `PageSize` + `PageIndex` + `OnPageIndexChanging`** / **`AllowSorting` + `SortExpression` + `OnSorting`**(ページャーは WebForms と同じ入れ子テーブル DOM) |
| Repeater + 各テンプレート | `<%# Eval("X") %>` → `@(Eval(Container, "X"))` |
| ListView | `LayoutTemplate` の `itemPlaceholder` に明細を展開(`ItemPlaceholderID` 対応)、`ItemTemplate` / `AlternatingItemTemplate` / `ItemSeparatorTemplate` / `EmptyDataTemplate` |
| FormView | `ItemTemplate` + `DataSource` 先頭 1 件を WebForms 同様 table でラップして表示 |
| PlaceHolder | 子コンテンツの素通し |
| 全バリデータ + ValidationSummary | RequiredField / **Range(Type 変換付き)/ Compare(Operator / ControlToCompare)/ RegularExpression(暗黙アンカー)/ Custom(`OnServerValidate`)**。`Text` 未指定時は WebForms 同様 ErrorMessage をインライン表示 |
| GridView 追加 | `DataKeyNames` + `DataKeys`、`Caption`、`ShowHeader`、行スタイル(`HeaderStyle-CssClass` 属性形式・`<HeaderStyle CssClass=.../>` 子要素形式の両方) |
| バリデータ `Display` | Static(既定: 有効時も hidden で場所確保)/ Dynamic / None |
| ValidationSummary 追加 | `HeaderText` / `DisplayMode`(BulletList・List・SingleParagraph)/ `ShowSummary` / `ShowMessageBox`(alert) |
| Panel 追加 | `GroupingText`(div 内 fieldset+legend)/ `DefaultButton`(Enter で実行) |
| その他頻出 | Label `AssociatedControlID`、TextBox `Wrap`、CheckBox `TextAlign`、DropDownList / RadioButtonList `AppendDataBoundItems` |

### WebControl 共通プロパティ(全 WebControl 系互換コントロールで利用可能)

基底クラス `WebFormsControlBase` が一括実装し、ルート要素に style / 属性として描画する。
マークアップ属性・コードビハインドからの代入のどちらでも使え、代入時は自己再描画する。

| WebForms | 描画先 |
|---|---|
| `Width` / `Height`(`"100"` は px 扱い、`"50%"` 等はそのまま) | `style="width:...;height:..."` |
| `BackColor` / `ForeColor` / `BorderColor` / `BorderWidth` / `BorderStyle` | `style` |
| `Font-Bold` / `Font-Italic` / `Font-Underline` / `Font-Size` / `Font-Names` | `style`(font-weight 等) |
| `ToolTip` / `TabIndex`(0 は非描画)/ `AccessKey` | `title` / `tabindex` / `accesskey` |
| `Attributes["..."]` コレクション | 任意の HTML 属性(placeholder, data-* 等) |
| `Style["..."]` コレクション | `style` にマージ |
| `Visible` / `Enabled`(コードビハインドから代入可) | 非描画 / `disabled` |

### ボタン系の拡張プロパティ

| WebForms | 互換実装 |
|---|---|
| `OnClientClick`(`return confirm('...')`) | JS interop で実行し、false ならサーバー処理(検証・Click・Command)を中止 |
| `ValidationGroup` | ボタンは自グループのバリデータだけを実行。ValidationSummary も自グループのみ表示 |
| `CommandName` / `CommandArgument` + `OnCommand` | Click の後に `CommandEventArgs` で発火(`e.CommandArgument` は WebForms 同様 object) |
| Repeater テンプレート内のコマンド | WebForms のイベントバブリング互換。`OnItemCommand` に `RepeaterCommandEventArgs`(Item / CommandSource 付き)で届く |
| GridView の `TemplateField` + `OnRowCommand` | テンプレート内の Button / LinkButton のコマンドが `GridViewCommandEventArgs` でバブリング |
| テンプレート内の `CommandArgument='<%# Eval("Id") %>'` | `@((Eval(Container, "Id"))?.ToString())` に変換 |
| RadioButtonList | WebForms 既定の table レイアウトで描画(Horizontal / Vertical 対応) |
| `UpdatePanel` / `ScriptManager` / `UpdateProgress` | 除去・展開(Blazor は常に差分描画のため不要) |
| 2 つ目以降の ContentPlaceHolder(サイドバー等) | `SectionOutlet` / `SectionContent` に変換 |
| `@` を含む属性値(メール用正規表現等) | C# 逐語的文字列式として出力(Razor の式開始記号と衝突するため) |

テンプレート内のコントロールは WebForms 同様、ページのフィールド(designer.cs 相当)を生成しない。

未カバーのプロパティは今も存在する(TextBox.Columns、Button.PostBackUrl、GridView の編集列、
ListView の GroupTemplate / DataPager 等)。ただし未カバーは必ず表面化する: マークアップ側は
変換レポートに残差として記録され、コードビハインド側はコンパイルエラーになる。黙って無視される経路はない。

既知の制約: テンプレート内コントロールの ID は行ごとに複製されるため DOM 上で重複する
(WebForms は `..._lnkDelete_0` と連番を振る)。パリティ/スモークのセレクタは先頭一致で吸収している。

## 設計上の判断(WebForms と意図的に異なる点)

- **Page_Load の実行タイミング**: Blazor では子コンポーネントの `@ref` が確定するのが初回描画後のため、
  `OnInitialized` ではなく `OnAfterRender(firstRender)` から呼ぶ。描画が 1 回余分に走る。
- **`!IsPostBack` ガードの外のコード**: WebForms では毎リクエスト実行されるが、Blazor には
  ポストバックがないため初期化時 1 回だけ実行される(多くの再バインド処理は Blazor では不要)。
- **マークアップ属性は「状態が変更されるまで」適用**: WebForms の優先順位
  (宣言的属性は初期値、コードビハインド/ユーザー入力による変更後はコントロール状態が優先)を
  そのまま実装している。無条件凍結だと GridView / Repeater の再バインドで行テンプレートの
  コントロールが古い行の CommandArgument を持ち続けるバグになる(パリティテストが検出した)。
- **クライアント検証の 2 段階ゲートは再現しない**: WebForms はクライアント検証が通るまで
  CustomValidator(サーバー)が実行されないが、変換後は全バリデータがサーバーで同時に実行される。
  「クライアント検証エラーとサーバー検証エラーが同時に表示されるか」に差が出るため、
  パリティシナリオは段階を分けて検証する。
- **`Label.Text` は HTML エンコードする**: WebForms は生 HTML として出力するが XSS を避けるため。
  HTML を埋め込む用途は `Literal`(こちらは生出力)を使う。
- **HeaderTemplate/FooterTemplate のタグ分割**: WebForms 定番の「Header で `<ul>` を開き
  Footer で閉じる」書き方は Razor では表現できないため、1 つの `WrapperTemplate` に組み替える。

## 現状の変換実績

### サンプル(`tools\verify-all.ps1`、exit 0)

| サンプル | 残差 | bUnit | 自動スモーク | パリティ(旧アプリ正解) |
|---|---:|---:|---:|---|
| DefaultsProbe(全対応コントロールの既定レンダリング) | 0 件 | — | 66 | **4/4 一致** |
| MasterProbe(マスター/UC の naming container) | 1 件 (通知) | — | 9 | **3/3 一致** |
| HelloWebForms | 0 件 | 3 件 | 3 | — |
| ProductAdmin(3 ページ + マスター + UC、42 コントロール) | 0 件 | 9 件 | 23 | **13/13 一致** |
| OrderAdmin(3 ページ + マスター、42 コントロール、UpdatePanel/TemplateField/全バリデータ) | 0 件 | 5 件 | 26 | **10/10 一致** |
| 横断(NewControlTests / FrequentPropertyTests) | — | 13 件 | — | — |
| **合計** | | **30 件合格** | | **30 スナップショット全一致** |

MasterProbe の 1 件は対処不要の通知(`Informational`)で、変換の失敗ではありません。

### コーパス(実在 OSS アプリ、`corpora\convert-all.ps1`、exit 0)

| コーパス | 移植 .cs | 総残差 | うち変換可能 | ビルドエラー |
|---|---:|---:|---:|---:|
| BlogEngine.NET 3.3.8 | 253 | 82 | 6 | **0** |
| mojoPortal 3.1.6 | 731 | 262 | 20 | 247 |
| YAF.NET 3.2.15 | 2,722 | 80 | 5 | 179 |
| DNN Platform 9.13.10 | 1,944 | 392 | 12 | 941 |
| n2cms | 1,661 | 258 | 59 | 117 |
| WingtipToys | 13 | 38 | 3 | **1** |
| **計** | | **1,112** | **105** | **1,485** |

n2cms は**ホールドアウト**として後から追加したものです。他の 5 本は変換器を育てる過程で
使ってきたため、適合しすぎていないかを測る対照が必要でした。実際、初回変換では他の 5 本が
一度も踏まなかった穴(ジェネリックなページ基底クラス)を露出させ、生成 Razor のエラーが
1 件から 108 件に跳ね上がりました。変換可能残差 59 件が他 5 本の合計より多いのも同じ理由です。

総残差の大半は `ManualMigration` = 設計判断や外部依存で、人間が決めるべきものです。
品質指標として追うのは **変換可能(`Convertible`)の 46 件**だけです。
生成 Razor の RZ(Razor 構文)エラーは全コーパスで 0 件。

**残差と「ビルドが通ること」は別物です。残差 0 件は「書かれた構文を全部読めた」で
あって「動く」ではありません。** 変換出力を実際にビルドしたのが右端の列で、
`convert-all.ps1` がベースラインとして固定しています。

BlogEngine は **0 エラーでビルドが通ります**。依存パッケージがすべて .NET に移植済み
だからで、外部依存が軽いアプリなら変換出力がそのままビルドできることを示しています。

残る 1,485 件の内訳は、大半が変換器の不具合ではありません。大半は互換層に無いコントロール
(`Menu` / `TreeView` 一族)と、.NET に持ち込めない依存です。DNN Platform だけは
別要因で、ビルド時に Roslyn ソースジェネレータが生成していた宣言が変換出力には
存在しないため、それに依存するコードは原理的にビルドできません(変換レポートが
アナライザ参照として報告します)。

## 既知の制約(次マイルストーン候補)

- `Page_Init` / `Page_PreRender` など Page_Load 以外のライフサイクルイベント(検出して残差報告のみ)
- 動的コントロール生成(`Controls.Add`)、`FindControl` 経由の型付き操作
- インラインコードブロック `<% %>`、テンプレート外のデータバインド式(残差報告のみ)
- `<%@ Register Assembly=... %>`(サードパーティコントロール)
- Forms 認証 / HttpModule / HttpHandler(残差報告のみ)
- TreeView(残りのデータコントロール。ListView / FormView / DetailsView は対応済み)
- AI 変換層: 残差の抽出(`AI-TASKS.json`)、プロンプト生成(`--ai-tasks`)、
  ビルド/パリティをゲートにした適用とロールバック(`--ai-apply`)までは実装済み。
  **プロンプトを LLM に投げて回答を書き出す部分だけが未実装**で、そこは外部に委ねている。

実装済みになったもの: `ValidationGroup` と全バリデータ(`RangeValidator` 含む)、
ListView / FormView / DetailsView、変換前アプリとの自動パリティテスト(ゴールデンマスター)。
