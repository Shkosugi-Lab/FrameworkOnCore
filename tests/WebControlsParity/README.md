# コンポーネント単位の新旧比較（WebControlsParity）

System.Web のコントロール（TextBox など）を 1 つずつ取り上げ、**変換前（.NET Framework 4.8）と変換後（FrameworkOnCore の System.Web）で、
すべてのプロパティ・メソッド・描画・WebForms の機構が同じように動くこと**を確かめる仕組みです。

## 考え方

変換器は不特定多数のユーザーの自社システムを動かすことを想定しています。手元のコーパスで使われている機能だけを確かめても、
ユーザーのコードがそれ以外を使っていれば保証になりません。そのため、ケースは**型から機械的に作り**、カバレッジは**測れる形**で出します。

- メンバー: .NET Framework のそのコンポーネント（基底クラス・実装インターフェースを含む）の公開/プロテクトメンバーすべて
- 描画: 設定できるプロパティすべてを、型ごとのサンプル値すべてで 1 つずつ設定したときの HTML
- 機構: そのコンポーネントが関わる WebForms の機構（ポストバックデータ、変更イベント、ViewState、AutoPostBack、検証、テーマ…）。
  どの機構が関わるかは型（実装インターフェース・プロパティ・属性）から判定し、どれかがケースで覆われていなければ「未カバー」と出る

組み合わせ（複数のプロパティを同時に設定したとき）は全網羅できないので、相互作用が知られているもの（TextMode=MultiLine と Rows、
AutoPostBack と CausesValidation など）を手で列挙します。コーパス（be/wt）はカバレッジの根拠ではなく、実アプリでの統合確認の位置づけです。

## 3 つの層

| 層 | どこで | 何を比べるか | 正解データ |
|---|---|---|---|
| ① API | コンソール（`tests/WebControlsParity`、net48 と net10.0 の二重ビルド） | 全メンバーの呼び出し結果・例外・呼んだ後の状態 | net48 で実行した結果 |
| ② HTML | プローブサイト（`samples/WebControlsProbe` の `RenderProbe.aspx`）、実リクエスト内 | 1 ケース 1 リクエストで描画したページ全体と、保存された ViewState の中身 | IIS で動かしたサイトから取得 |
| ③ シナリオ | 同じサイトの `<Component>Scenario.aspx` をブラウザ（Playwright）で操作 | 本文・入力値・要素の属性、サーバー側のイベントのログ | IIS で動かしたサイトで記録 |

### ① API 層

`tests/Parity.Core` の `ApiParity`（DrawingParity・MobileParity と同じ仕組み）を、コンポーネント単位にしたものです（`ComponentApi`）。

- 対象の API は、コンポーネントの型、その System.Web の基底クラス（TextBox なら WebControl、Control）、System.Web の実装インターフェース
  （IPostBackDataHandler、IEditableTextControl…）。インターフェースのメンバーを通して、明示的な実装（`IPostBackDataHandler.LoadPostData` など）も呼ぶ
- Control に宣言されたメンバーも **TextBox のインスタンスで**呼ぶ（ページが呼ぶのと同じく、TextBox のオーバーライドが動く）
- インスタンスはメンバーごとに新しく作るサンプルページ（Page > HtmlForm form1 > Label lblTitle、コンポーネント）の中のもの（`Samples`）。
  引数は型と引数名から決める（`postDataKey` はコンポーネントの UniqueID、など）
- 記録するのは戻り値、例外、参照渡しの引数、描画先の TextWriter に書かれた HTML、呼んだ後のコンポーネントの状態（単純な型の公開プロパティすべて）（`WebDescribe`）
- リクエストの外で動かすので、イベント検証はサンプルページで切る（描画やポストデータの読み込みがリクエスト外でも動くように）。
  イベント検証そのものは ② ③ で確かめる
- .NET 側は、Web アプリのホストと同じく FrameworkOnCore の machine.config を書いて読む（無いと `system.web/pages` が読めず、ClientID などが例外になる）
- 呼ばないメンバーは理由つきで `ComponentApi.Excluded` に書く（今はデリゲートの BeginInvoke/EndInvoke のみ）
- コンソールの .NET Framework にはルート web.config が無い（Web アプリには有る）。サイトマップのプロバイダー、Web パーツのパーソナライズのプロバイダーと
  トランスフォーマーは、ルート web.config と同じものを `App.config` に書く（.NET 側の machine.config には元から有る）。無いと .NET Framework だけ構成の例外になる
- フォーム・ヘッダーのコンポーネント（HtmlForm・HtmlHead）は、サンプルページのフォーム・ヘッダーそのものにする。親が要るもの（View は MultiView、WizardStep は
  Wizard、AJAX のコントロールは ScriptManager の後、Web パーツは WebPartManager の後）は `Component.Wrap` で親を作る
- DictionaryEntry は「キー=値」で書く（.NET 5 で ToString が付いた。MobileParity と同じ `Probe.Custom`）

### ② HTML 層

プローブサイトの `Cases.aspx?component=TextBox` がケースの一覧を、`RenderProbe.aspx?component=TextBox&case=<名前>` が 1 ケースを描画します。
ケースはサイトの `ProbeComponents.cs` が型のリフレクションで作るので、プロパティが増えればケースも増えます。

| ケース名の形 | 内容 |
|---|---|
| `TextBox.(default)` | 何も設定しない |
| `TextBox.MaxLength=3` | 設定できる公開プロパティ（単純な型、サブオブジェクト `Font.Bold` も）を、型のサンプル値（文字列: 通常・HTML 特殊文字・日本語・空、数値: 0/3/-1、列挙型: 全値、Unit、Color、FontUnit）ごとに |
| `TextBox.Attributes[data-probe]=text`、`TextBox.Style[color]=red` | 属性・スタイルのコレクション |
| `TextBox.TextMode=MultiLine+Rows=3+Columns=3` | 既知の相互作用（`ProbeComponent.Interactions`、手で列挙） |
| `DropDownList.Items[1].Selected=true` | 項目を持つコントロール（ListControl）は、項目（`ItemPath`）の設定できるプロパティ（Selected・Enabled・Text・Value）も同じく型から |
| `TextBox[naming container].ClientIDMode=Static`、`TextBox[themed].SkinID=text` | 文脈: 名前付けコンテナの中、テーマ（`App_Themes/Probe`）あり |
| `DropDownList[data-bound].DataTextField=Name+DataValueField=Code` | 文脈: データバインド（Load で `ProbeData.Products()` を DataSource にして DataBind） |

- コンポーネントは PreInit で作り、プロパティを設定してからページに加える（マークアップをパーサーが組み立てるのと同じ順。テーマのスキンがその後で上書きする）
- HtmlForm・HtmlHead のケースは、そのコンポーネントがページのフォーム・ヘッダーの位置に入る（中身は元のものを移す）。親が要るコンポーネントは `Wrap` で
  親ごと置く（MultiView の 2 つ目の View、Wizard の 2 つ目のステップなど）
- 設定で例外になったもの（`MaxLength=-1` など）は、例外の型とメッセージを `probe-errors` に書く
- 保存される ViewState（コントロールの状態とビューステート）は、エンコード前の中身をテキストにして `probe-state` に書く
- ブラウザの User-Agent は両方で同じものを送る（コントロールはブラウザの能力で描き分けるため）

比べるのはページ全体です。マスクするのは、マシンやビルドで変わり実装の違いではないものだけです（`RenderLayer.Normalize`）。

- `__VIEWSTATE`・`__EVENTVALIDATION` の値（マシンキーで暗号化・署名される。中身は `probe-state` で比べる）、`__VIEWSTATEGENERATOR`
- `__PREVIOUSPAGE` の値（PostBackUrl のボタンがあると出る。ページのパスをマシンキーで暗号化したもの。移動先のページが PreviousPage を受け取れることは ③ で確かめる）
- `WebResource.axd`・`ScriptResource.axd` の URL の `d`（マシンキーで暗号化）と `t`（アセンブリのビルド日時）
- API 層の GridView.GetCallbackResult のグリッドの状態（マシンキーで直列化される。コンソールでは実行ごとに鍵が変わり、.NET Framework でも毎回違う）
- コールバックでページングと並べ替えをする GridView の並べ替え式（スクリプトの `sortExpression` とコールバックの引数の 3 つ目。マシンキーで暗号化される。コールバックで並べ替えられることは ③ で確かめる）
- 例外メッセージの末尾の引数名（.NET Framework は「Parameter name: x」、.NET は「(Parameter 'x')」。ランタイムの書き方の違いで、`tests/Parity.Core` の Probe と同じく `[param x]` として別に書く）
- スクリプトの文字列（'...'）の中の `WebResource.axd` の URL（TreeView の画像）も上と同じく
- 例外メッセージの中のサイトのフォルダー（`{site}`。記録した IIS のフォルダーとポートのフォルダーは違う）
- API 層の ScriptManager.GetStateString（履歴の状態。マシンキーで署名される）、WebPartManager.CreateDynamicConnectionID（乱数）

比べるときに、保存された ViewState の説明（probe-state）の中の StateBag の項目（キーと値の組）は、両側ともキーの順に並べ直します（`Report.StateBagsInOrder`）。
項目が多い StateBag は Hashtable で持ち、その順は文字列のハッシュ値で決まるが、.NET はプロセスごとにハッシュ値を変える（.NET Framework は変えない）ため。
中身は同じで、並びだけが違う。

### ③ シナリオ層

機構ごとに 1 つ以上の TextBox を置いたページ（`TextBoxScenario.aspx`）を、`tools/FrameworkOnCore.ParityTest` で操作します（シナリオは
`samples/WebControlsProbe/scenarios/TextBox.json`）。ページの下部には、サーバーが観測したこと（ライフサイクル、発生した TextChanged の順、
Bind が取り出した値、最後の Text、IsValid）をログとして出すので、ブラウザに見える結果だけでなくサーバー側の動きも比べられます。

スナップショット名の `[postdata]` などのタグは、そのスナップショットがカバーする機構です。結果のレポートの機構マトリクスはこのタグから数えます。
シナリオで乱数の ID を持つ要素（実行時に足した Web パーツの「wp + 数字」）は、シナリオの `maskIds`（正規表現）で両側とも伏せます。

TextBox で確かめていること: ポスト値の取り込み、TextChanged の発生と順序、ReadOnly の TextBox にスクリプトで書き換えた値が無視されること、
Enabled=false、パスワードが再描画されないこと、EnableViewState=false の TextBox が毎回 TextChanged を起こすこと、Visible=false の間の Text が ViewState で保たれること、
動的に作った TextBox、AutoPostBack（Tab で change を起こす）、AutoPostBack と CausesValidation/ValidationGroup、サーバー側の検証グループ、
ブラウザ側の検証（ポストバックしない）、テンプレート内の Bind、ValidateRequestMode=Disabled とリクエスト検証の例外。

Button で確かめていること（`ButtonScenario.aspx`、クロスページの移動先 `ButtonTarget.aspx`）: Click と Command の発生と順序、UseSubmitBehavior=false（スクリプトによるポストバック）、
OnClientClick が false を返すとリクエストが出ないこと（ログのリクエスト番号を ViewState に持たせて確かめる）、検証グループと CausesValidation=false（検証しなくても Click は起きる）、
ブラウザ側の検証でポストバックしないこと、Repeater の項目のボタンの Command が ItemCommand にバブルすること、Panel の DefaultButton（Enter キー）、
動的に作ったボタン、Visible=false から表示したボタンの Text が ViewState で保たれること、PostBackUrl で移動した先で PreviousPage とその入力値が読めること。

ListControl 系（DropDownList・ListBox・CheckBoxList・RadioButtonList・BulletedList）は 1 つのページ（`ListControlsScenario.aspx`）を共有し、
シナリオはコンポーネントごと（`scenarios/<Component>.json`）です。確かめていること: 選択のポストと SelectedIndexChanged の順、EnableViewState=false のリストが毎回
SelectedIndexChanged を起こすこと、複数選択（ListBox・CheckBoxList）、AutoPostBack、検証（サーバー・ブラウザ）、コードで足した項目が非表示の間も ViewState で保たれること、
動的に作ったリスト、データバインド（AppendDataBoundItems）、テンプレート内の Bind（SelectedValue）、BulletedList の LinkButton 表示のクリック、
改ざんしたポスト（一覧にない値・無効な項目・無効な弾丸のポストバック）の扱い（DropDownList・ListBox・BulletedList は例外、CheckBoxList・RadioButtonList は無視する。
.NET Framework と同じであること）、マークアップのポストのリクエスト検証。

基本コントロール（Label・CheckBox・RadioButton・Literal・LinkButton・HyperLink・Image・ImageButton・Panel・HiddenField・PlaceHolder）は
`BasicsScenario.aspx` を共有します。各コントロールについて、そのまま、データバインド、コードで設定（ViewState で保たれる）、ViewState なしでコードで設定（失われる）、
非表示の間に設定して後で表示、動的生成、を置いています。確かめていること: 上の保持と喪失、CheckBox・RadioButton・HiddenField のポストと変更イベント
（ViewState なしのものは毎回起きる）、AutoPostBack と検証、テンプレート内の Bind、LinkButton・ImageButton の Click・Command・OnClientClick・検証・Repeater からのバブル・
Panel の DefaultButton（Enter キー）・PostBackUrl、ImageButton のクリック座標（マウスでは画像の中央、DefaultButton では 0,0）、HyperLink をたどる（GET）、
Panel・PlaceHolder の中のテキストボックスのポスト、マークアップのポスト（ImageButton は座標 `ibtnGo.x`）のリクエスト検証。

検証コントロール（RequiredFieldValidator・RegularExpressionValidator・CompareValidator・CustomValidator・RangeValidator・ValidationSummary）は
`ValidatorsScenario.aspx` を共有します。② では、検証対象のテキストボックスを隣に置き（`Siblings`）、`IsValid=false` などで失敗した状態の描画も比べます。
③ で確かめていること: サーバー側の検証（グループ、空のときは RequiredFieldValidator 以外は通る、InitialValue、無効・非表示の検証コントロールは検証しない、
表示すると検証する、ServerValidate と ValidateEmptyText、ControlToValidate の無い CustomValidator、1 つの検証コントロールの Validate()、Page.Validate(グループ)）、
ブラウザ側の検証（種類ごとのグループ。失敗するとリクエストが出ない、通るとポストされる）、ValidationSummary（BulletList・SingleParagraph・List、
Display="None" のメッセージはまとめにだけ出る）、動的に作った検証コントロール、データバインドしたメッセージ。

データ表示コントロールは、② の全ケースを 7 件のデータ（`ProbeData.Many()`。3 件ずつのページングで 3 ページ）にバインドし、文脈で列やテンプレートを変えます
（GridView の `[fields]` は BoundField・CheckBoxField・HyperLinkField・ImageField・ButtonField・CommandField・TemplateField、`[empty]` はデータなし。
Repeater の `[templates]` はヘッダー・フッター・区切り・交互の項目）。スタイルのサブオブジェクト（HeaderStyle.BackColor など）とページャーの設定（PagerSettings.*）も
型からケースにします。③ はコントロールごとのページ（`GridViewScenario.aspx`・`RepeaterScenario.aspx`）で、データはセッションの `ProductStore`
（各シナリオの最初のリクエストで元に戻す。ObjectDataSource の型でもある）です。
GridView で確かめていること: ページが DataSource とハンドラーで行うページング・並べ替え・選択・編集（キー・古い値・新しい値）・取り消し・削除、ButtonField のコマンド、
ObjectDataSource（DataSourceID）による自動のページング・並べ替え（向きはグリッドが持つ）・選択・更新・削除、コールバックによるページングと並べ替え（ポストバックしない）、
空のグリッド、ViewState なしのグリッド（ポストバックで行が消える）、非表示から表示、動的生成。
Repeater で確かめていること: 項目が ViewState から作り直されること（ItemCreated の数）、項目の中のテキストボックスのポスト、リンク・ボタン・入れ子の Repeater の
コマンドの ItemCommand へのバブル、再バインド、DataSourceID、空、ViewState なし、非表示、動的生成。
FormView・DetailsView・ListView（と DataPager）・DataList・DataGrid も、それぞれのページ（`<Component>Scenario.aspx`。DataPager は ListView のページ）で、
ページがハンドラーで行うページング・並べ替え・選択・編集・挿入・削除と、ObjectDataSource による自動の処理、コールバック（DetailsView）、
空・ViewState なし・非表示・動的生成を確かめます。② の文脈は、DetailsView の `[fields]`、DataGrid の `[columns]`、ListView の `[layout]`・`[groups]`・`[editing]`、
DataPager の `[next-previous]`・`[template]` など。ListView と DataPager は System.Web.Extensions の型で、① の API にはそのアセンブリも含めます。

残りのコントロールは、ページを共有し、シナリオを生成スクリプトで作ります（コンポーネントごとに、その型に当てはまる機構のスナップショットだけをタグ付きで含める）。

| ページ | コンポーネント | 確かめていること |
|---|---|---|
| `HtmlScenario.aspx`、`HtmlFormScenario.aspx`（`html-scenarios.py`） | HtmlControls 33（HtmlGenericControl、HtmlAnchor、HtmlForm、HtmlHead、HtmlInput*、HtmlSelect、HtmlTextArea、HtmlTable 系、HtmlMeta・HtmlLink・HtmlTitle、HTML5 の要素など） | そのまま・データバインド・コードで設定・ViewState なし・非表示・動的生成、入力のポストと ServerChange（変えずに再ポストすると起きない）、ボタンの ServerClick（画像の座標）、リセット（リクエストなし）、ファイルの送信（`upload`）、サーバー・ブラウザの検証、HtmlSelect の改ざんしたポストの拒否、フォーム・ヘッダー・タイトルをコードで作るページ（DefaultButton への Enter） |
| `MoreScenario.aspx` | Table 系 6、FileUpload、MultiView・View、Localize、Substitution、Xml、AdRotator、Calendar、ImageMap、LiteralControl | コードで足した表の行や LiteralControl の文字列が保たれないこと、ファイルの送信、View の切り替え（NextView・SwitchViewByID・SwitchViewByIndex のバブル）と Activate・Deactivate、Calendar の日・週（複数日）・翌月、ImageMap のホットスポット、Table へのポストバックイベントの拒否、AdRotator の AdCreated（広告ファイル 1 件: 複数ならランダム）、Calendar の今日は固定 |
| `ContentScenario.aspx`（`Probe.master`） | Content、ContentPlaceHolder | マスターページの置き場所への中身、既定の中身、非表示、コードで足した中身、名前付け（ClientID）、マスターのボタン |
| `AjaxScenario.aspx` | ScriptManager、ScriptManagerProxy、UpdatePanel、UpdateProgress、Timer | 部分描画（パネルのボタン、外のトリガー、PostBackTrigger による全体ポストバック、条件付きのパネルは描き直さない）、遅い要求の間の進行表示、Timer の Tick、履歴点とブラウザの戻る（Navigate）、プロキシのスクリプト、コードで作ったパネル |
| `NavigationScenario.aspx`（`Web.sitemap`） | TreeView、Menu、SiteMapPath、SiteMapDataSource | サイトマップのパス・ツリー・メニュー、ノードの展開・選択・チェックボックスのポスト、要求時の生成（ポストバックとクライアントコールバック）、XML への結び付け、テンプレート、メニューの項目のクリック、SelectedValue を ControlParameter で使う（ControlValueProperty） |
| `LoginScenario.aspx`（`ProbeMembership.cs`） | Login、LoginName、LoginStatus、LoginView、ChangePassword、PasswordRecovery、CreateUserWizard と 2 つのステップ、Wizard、WizardStep、TemplatedWizardStep | フォーム認証（メモリ上のユーザーとロール。`?reset=1` で元に戻す）、誤ったパスワード、ログイン後の名前・状態・ロールのビュー、パスワードの変更・回復（メールは SendingMail で取り消して中身を記録）、ユーザーの作成、Wizard の次・前・サイドバー・完了 |
| `DataSourceScenario.aspx`（`ProbeDb.cs`） | ObjectDataSource、SqlDataSource、AccessDataSource、XmlDataSource、LinqDataSource、QueryExtender、ModelErrorMessage | 更新・削除・挿入・並べ替え・ページング、コントロールの値によるパラメーター、モデルのエラー。SqlDataSource はメモリ上の ADO.NET プロバイダー（web.config の DbProviderFactories に登録）で動かし、Access はプロバイダーが無いことを記録 |
| `WebPartsScenario.aspx`（`ProbePersonalization.cs`） | WebPartManager、ProxyWebPartManager、ゾーン 4（WebPartZone・CatalogZone・EditorZone・ConnectionsZone）、カタログの部品 3、エディターの部品 4 | 表示モード（デザイン・カタログ・編集・接続）、カタログからの追加、タイトルの編集、接続ゾーン、最小化と復元（パーソナライズはメモリ上） |

## 機構マトリクス

`Mechanisms.cs` に機構の一覧と、それぞれがどの型に当てはまるか（判定の式）、どのケースがカバーするか（② はケース名、③ はタグ）を書いています。

| 機構 | 当てはまる型 |
|---|---|
| プロパティ → HTML、ViewState、Visible、名前付けコンテナ、動的生成、データバインド | すべてのコントロール |
| テーマとスキン | テーマを付けられる型（`ThemeableAttribute.IsTypeThemeable`。Literal・HiddenField・PlaceHolder は付けられない） |
| Enabled=false・Disabled=true | WebControl、Disabled プロパティを持つもの（HtmlControls） |
| ポストバックデータ、リクエスト検証 | IPostBackDataHandler |
| 変更イベント | IPostBackDataHandler で *Changed・ServerChange イベントがある |
| ReadOnly | IPostBackDataHandler で ReadOnly プロパティがある |
| ポストバックイベント | IPostBackEventHandler |
| コマンドとバブル、クロスページポストバック、既定のボタン | IButtonControl |
| UseSubmitBehavior、OnClientClick | そのプロパティがある |
| AutoPostBack、CausesValidation | そのプロパティがある |
| サーバー側の検証 | IValidator（検証コントロール） |
| ブラウザ側の検証スクリプト | EnableClientScript プロパティがある |
| 検証結果のまとめ | ValidationSummary |
| 検証対象（サーバー・ブラウザ） | `[ValidationProperty]` と IPostBackDataHandler（CheckBoxList は属性を持たず、RequiredFieldValidator で検証できない） |
| 双方向バインド・ControlParameter | `[ControlValueProperty]` と IPostBackDataHandler |
| 項目 | ListControl、HtmlSelect |
| 複数選択 | SelectionMode・Multiple プロパティがある、CheckBoxList、HtmlSelect |
| 繰り返しレイアウト | IRepeatInfoUser |
| ポスト値のイベント検証 | ListControl・HtmlSelect と IPostBackDataHandler |
| テンプレート | ITemplate のプロパティがある |
| データソース連携 | BaseDataBoundControl、HtmlSelect（② の `[data-bound]` 文脈） |
| ページング、並べ替え | AllowPaging・AllowSorting プロパティがある。ListView は IPageableItemContainer と Sorting イベント、DataPager |
| 行・項目の編集 | EditIndex・EditItemIndex プロパティ、CompositeDataBoundControl の DefaultMode |
| 行・項目の選択 | データ表示コントロールの SelectedIndex |
| 行・項目のコマンド | Command 以外の *Command イベント（RowCommand・ItemCommand） |
| データキー | DataKeyNames プロパティ |
| データソースコントロールとの連携（DataSourceID） | データ表示コントロール（Repeater・CompositeDataBoundControl・BaseDataList・ListView）。ListControl の DataSourceID は ② の単独のケースだけ |
| 項目の挿入 | InsertItemTemplate・AutoGenerateInsertButton プロパティがある |
| クライアントコールバック | ICallbackEventHandler |

## 既知の差分

ポートが .NET Framework と違ってよいところは、理由とともに `known-differences.json` に書きます（DrawingParity・MobileParity と同じ形に、層 `layer` を足したもの）。
`layer`（api / render）のケースのうち `case` に合うものから、`line` に合う行を両方から除いて残りを比べます。`on` は all / windows / linux。
何も除かなかった項目は、結果のレポートに「古くなっていれば削除」として出ます。結果のレポートは、そのコンポーネントのケースに当てはまる項目だけを「適用した」「古い」として出します。
今は 8 件です。

| 項目 | 理由 |
|---|---|
| DetailsView の挿入モードでデータが無いときの例外のメッセージ | ランタイムの ArrayList の文言（System.Web のものではない） |
| ScriptManager.AjaxFrameworkAssembly、ScriptPath のフォルダー名 | System.Web.Extensions のアセンブリの版（ポートは 10.0.29.0、.NET Framework は 4.0.0.0 とインストールされたファイルの版）。ScriptPath にスクリプトを置くアプリは、ポートの版の名前のフォルダーが要る |
| Xml の DocumentSource・TransformSource にパスに使えない文字 | .NET Framework は先に検査して ArgumentException、.NET はシステムに任せる（Windows は IOException、Linux は見つからない） |
| Xml の見つからないファイル（Linux） | パスの区切り（`\` と `/`） |
| WebPartManager の Minimal・MediumPermissionSet | コードアクセスセキュリティ（.NET に無い。空の集合） |
| WebPartManager の AvailableTransformers | 同じ 2 つのトランスフォーマーの並び（Hashtable の順。型のハッシュ値が .NET では違う） |
| AccessDataSource.GetDbProviderFactory（Linux） | OLE DB は Windows のもの。ポートは PlatformNotSupportedException（.NET Framework とポートの Windows は OleDbFactory） |

## 出力（Markdown）

`reports/<Component>/` に出します。どちらも `parity.ps1` が作るので、手で編集しません。

- `cases.md` — テストケース一覧: 機構マトリクス、① 全ケースと .NET Framework での期待値、② 全ケースの設定内容（C#）と期待される描画、③ シナリオの手順と各スナップショットの期待される本文
- `results-windows.md`、`results-linux.md` — 結果: サマリ（層ごとの一致・既知の差分・不一致）、カバレッジ（メンバー数、プロパティ数、機構ごとの結果）、不一致の差分、適用した既知の差分、全ケースの結果

正解データは `golden/<Component>/`（api.golden.txt、cases.golden.json、render.golden.json、render.golden-cases.tsv、scenario.golden.json、recorded.txt）です。

## 実行

```powershell
# 正解データを採る（.NET Framework 4.8 と IIS。管理者のシェル）。cases.md も作る
.\tests\WebControlsParity\parity.ps1 -Control TextBox -Record
# 複数のコンポーネントを 1 回で（サイトの構築・変換・コンテナは 1 回）
.\tests\WebControlsParity\parity.ps1 -Control Table,TableRow,TableCell -Windows

# ポートで比べる（変換器でプローブサイトを変換して動かす）。results-windows.md / results-linux.md を作る
.\tests\WebControlsParity\parity.ps1 -Control TextBox -Windows
.\tests\WebControlsParity\parity.ps1 -Control TextBox -Linux     # Docker Desktop が必要
```

途中のファイルは `experiments/wf4c/_samples/WebControlsProbe/`（変換後のサイト、ログ、ポートの出力 `windows/<Component>`・`linux/<Component>`）に置きます。
ポートの出力が残っていれば、`report --port` でレポートだけを作り直せます。
1 回の実行で時間はおよそ、1 つなら -Record 75〜85 秒、-Windows 95〜105 秒、-Linux 2 分です。複数なら 1 つあたり 30〜60 秒です
（全 124 を Windows で約 2 時間、Linux で約 2 時間 10 分）。

済んでいるコンポーネント（124。System.Web と System.Web.Extensions の、アプリが置くコントロールすべて）。結果は `reports/` の各フォルダー。

- WebControls: TextBox、Button、DropDownList、ListBox、CheckBoxList、RadioButtonList、BulletedList、Label、CheckBox、RadioButton、Literal、LinkButton、
  HyperLink、Image、ImageButton、Panel、HiddenField、PlaceHolder、検証コントロール 6、GridView、Repeater、FormView、DetailsView、ListView、DataPager、DataList、
  DataGrid、Table・TableRow・TableHeaderRow・TableFooterRow・TableCell・TableHeaderCell、FileUpload、MultiView・View、Localize、Substitution、Xml、AdRotator、
  Calendar、ImageMap、Content・ContentPlaceHolder、TreeView、Menu、SiteMapPath、ログイン系 7、Wizard と各ステップ 5、データソース 6（SiteMapDataSource を含む）、
  QueryExtender、ModelErrorMessage
- System.Web.UI: LiteralControl、ScriptManager、ScriptManagerProxy、UpdatePanel、UpdateProgress、Timer
- HtmlControls: 33
- WebParts: 13

対象にしないもの: Page、UserControl、MasterPage、Control（アプリが派生させる基底クラス。シナリオのページ自体がそれ）、
DesignerDataBoundLiteralControl（デザイナー用）。

## 進める順番

コンポーネントは、コーパスでよく使われているものから進めます（`control-usage.py`: マークアップのタグと、API の解析のコードからの参照を数え、
使っているアプリの数、使用数の順に並べる）。どのコンポーネントも、取り上げたら型から全体をカバーするので、順番は先に何をやるかだけを決めます。

| 順 | コンポーネント | 状態 |
|---|---|---|
| 1 | TextBox、Button、ListControl 系 5 つ | 済 |
| 2 | Label、CheckBox・RadioButton、Literal、LinkButton、HyperLink、Panel、HiddenField、PlaceHolder、Image、ImageButton | 済 |
| 3 | 検証コントロール（RequiredFieldValidator、RegularExpressionValidator、CompareValidator、CustomValidator、RangeValidator、ValidationSummary） | 済 |
| 4 | GridView、Repeater、FormView、DetailsView、ListView・DataPager、DataList、DataGrid | 済 |
| 5 | HtmlControls 33、Table 系、FileUpload、Content・ContentPlaceHolder（マスターページ）、MultiView・View、その他の WebControls | 済 |
| 6 | UpdatePanel・ScriptManager など（System.Web.Extensions）、TreeView・Menu・SiteMapPath、ログイン系、Wizard | 済 |
| 7 | データソース、WebParts | 済 |

個別に動かすとき:

```powershell
dotnet tests\WebControlsParity\bin\Debug\net10.0\WebControlsParity.dll api --control TextBox --only "TextMode"   # API 層の一部をコンソールに
dotnet tests\WebControlsParity\bin\Debug\net10.0\WebControlsParity.dll render --control TextBox --url http://localhost:5097 --out render.json
dotnet tests\WebControlsParity\bin\Debug\net10.0\WebControlsParity.dll report --control TextBox --golden tests\WebControlsParity\golden\TextBox --out tests\WebControlsParity\reports\TextBox
```

## コンポーネントを足すには

1. `Components.cs`（① のインスタンスの作り方）と、サイトの `ProbeComponents.cs`（② の作り方、型にないサンプル値 `ExtraValues`、相互作用 `Interactions`、文脈 `Contexts`）に加える
2. テーマを付けられる型なら、テーマのスキン（`App_Themes/Probe` に既定のスキンと SkinID="probe" のスキン）。テーマは全コンポーネントで共通なので、
   ある型のスキンは、それを中で使うほかのコンポーネントの `[themed]` ケースにも効く（CheckBox のスキンは CheckBoxList の項目に効く）。スキンを足したら、
   そうしたコンポーネントの比較が「不一致」になるので（.NET Framework もそう描くかを確かめてから）`-Record` で採り直す
3. シナリオのページ `<Component>Scenario.aspx` と `scenarios/<Component>.json`。機構マトリクスで当てはまる機構それぞれに、タグつきのスナップショットを 1 つ以上。
   ページの designer.cs と csproj への登録は `python tests/WebControlsParity/scenario-page.py <Page>`（マークアップから、Visual Studio と同じフィールドを書く）。
   ページを共有するコンポーネントのシナリオは `html-scenarios.py`・`more-scenarios.py` で生成する
   （新しい型のスキンは SkinID="probe" の名前付きのものだけにする。既定のスキンは、その型を中で使うほかのコンポーネントの描画を変えるため）
4. `-Record`、`-Windows`、`-Linux` を実行し、`cases.md` の「未カバー」が無いこと、結果に不一致が無いことを確かめる。違いがポートの誤りなら FrameworkOnCore.Runtime を直し、
   そうでない（ランタイムの違いなど）なら理由を書いて `known-differences.json` に加える

① と ② のケースは型から作られるので、足すのはインスタンスの作り方と、型から分からないもの（相互作用、シナリオ）だけです。

## カバーしないもの

- プロパティの組み合わせの全網羅（列挙した相互作用のみ）
- web.config の構成の違い（`controlRenderingCompatibilityVersion`、`xhtmlConformance` など）。プローブサイトは .NET Framework 4.8 の既定の構成のみ
- 派生クラスから見た仮想メンバーの呼ばれ方（いつ、どの順で OnPreRender などが呼ばれるか）。シナリオのログが一部を見ているが、網羅はしていない
- ブラウザの種類による描き分け（User-Agent は 1 種類）

## ファイル

| パス | 内容 |
|---|---|
| `tests/WebControlsParity/Program.cs` | コマンド（api / render / report） |
| `tests/WebControlsParity/ComponentApi.cs`、`Samples.cs`、`WebDescribe.cs`、`Components.cs` | ① API 層 |
| `tests/WebControlsParity/RenderLayer.cs` | ② の取得と正規化 |
| `tests/WebControlsParity/Report.cs`、`Mechanisms.cs` | 比較と Markdown、機構マトリクス |
| `tests/WebControlsParity/parity.ps1` | 全体の実行 |
| `samples/WebControlsProbe/` | プローブサイト（`Cases.aspx`、`RenderProbe.aspx`、`ProbeComponents.cs`、`ProbeState.cs`、`<Component>Scenario.aspx`、`scenarios/`、`App_Themes/Probe/`） |
| `tests/WebControlsParity/App.config` | ① の .NET Framework 側に、ルート web.config のサイトマップ・Web パーツの設定 |
| `tests/WebControlsParity/scenario-page.py`、`html-scenarios.py`、`more-scenarios.py` | ページの designer.cs と csproj、共有ページのシナリオの生成 |
| `samples/WebControlsProbe/ProbeMembership.cs`、`ProbeDb.cs`、`ProbePersonalization.cs`、`Web.sitemap`、`App_Data/` | メモリ上のユーザー・ロール、ADO.NET プロバイダー（SqlDataSource）・LINQ のコンテキスト、Web パーツのパーソナライズ、サイトマップ、XML・広告のファイル |
| `tools/FrameworkOnCore.ParityTest` | ③ のブラウザ操作（`press`・`setvalue`・`upload` の操作、`maskIds` を足した） |
