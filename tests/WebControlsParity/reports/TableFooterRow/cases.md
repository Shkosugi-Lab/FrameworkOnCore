# TableFooterRow テストケース一覧

変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。
このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。

- 正解データの採取: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0
- ① API 層: 153 ケース（162 メンバー）
- ② HTML 層: 242 ケース
- ③ シナリオ層: 14 スナップショット

## 機構マトリクス

型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。

| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |
|---|---|---|---|
| プロパティ → HTML 描画 | すべてのコントロール | 118 | 1 |
| ViewState の保存と復元 | Control | 11 | 13 |
| Visible=false（描画されず状態は保持） | Control.Visible | 3 | 1 |
| 名前付けコンテナと ClientIDMode | Control.ClientIDMode | 6 | 0 |
| テーマとスキン（SkinID / EnableTheming） | テーマを付けられる型（[Themeable]） | 4 | 0 |
| 動的に生成したコントロールのライフサイクル | Control | 0 | 3 |
| データバインド（DataBind / <%# %>） | Control.DataBind | 0 | 1 |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | WebControl.Enabled、HtmlControl.Disabled | 3 | 0 |

該当しない機構: ポストバックデータの取り込み、変更イベント（*Changed、ServerChange）、ReadOnly（ポストされた値を無視）、リクエスト検証（ValidateRequestMode）、ポストバックイベント、コマンドイベントと親へのバブル（CommandName）、送信ボタン／スクリプトによるポストバック（UseSubmitBehavior）、ブラウザ側のクリック処理（OnClientClick）、クロスページポストバック（PostBackUrl）、既定のボタン（Panel の DefaultButton）、AutoPostBack（クライアントスクリプトによるポストバック）、CausesValidation / ValidationGroup、検証対象（サーバー側の検証）、検証対象（ブラウザ側の検証）、双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値）、項目（ListItem の Selected・Enabled・Text・Value と ViewState）、複数選択、繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）、ポスト値のイベント検証（一覧にない値・無効な項目の拒否）、サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）、ブラウザ側の検証スクリプト（EnableClientScript）、検証結果のまとめ（ValidationSummary）、ページング、並べ替え、項目の挿入、行・項目の編集（編集・更新・取り消し・削除）、行・項目の選択、行・項目のコマンド（RowCommand・ItemCommand へのバブル）、データキー（DataKeyNames・DataKeys）、データソースコントロールとの連携（DataSourceID）、テンプレート、データソース連携（DataSource・DataTextField など）、クライアントコールバック

## ① API 層

TableFooterRow とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の TableFooterRow インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。

| ケース | 期待値（.NET Framework） |
|---|---|
| <code>Api.Control.#ctor</code> | <code>Control.#ctor = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMode=In…</code> |
| <code>Api.Control.Adapter</code> | <code>Control.Adapter = null</code> |
| <code>Api.Control.AddParsedSubObject</code> | <code>Control.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>Control.AddParsedSubObject(Object) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty…</code> |
| <code>Api.Control.AddedControl</code> | <code>Control.AddedControl(Control,Int32) = &quot;ok&quot;</code><br><code>Control.AddedControl(Control,Int32) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empt…</code> |
| <code>Api.Control.AppRelativeTemplateSourceDirectory</code> | <code>Control.AppRelativeTemplateSourceDirectory = &quot;&quot;</code><br><code>Control.AppRelativeTemplateSourceDirectory set = &quot;-&gt; System.ArgumentException: The relative virtual path &#39;Abc 123&#39; is not allowed here.&quot;</code> |
| <code>Api.Control.ApplyStyleSheetSkin</code> | <code>Control.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>Control.ApplyStyleSheetSkin(Page) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]…</code> |
| <code>Api.Control.BeginRenderTracing</code> | <code>Control.BeginRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.BeginRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.BeginRenderTracing(TextWriter,Object) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 C…</code> |
| <code>Api.Control.BindingContainer</code> | <code>Control.BindingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Clien…</code> |
| <code>Api.Control.BuildProfileTree</code> | <code>Control.BuildProfileTree(String,Boolean) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.ChildControlsCreated</code> | <code>Control.ChildControlsCreated = &quot;false&quot;</code><br><code>Control.ChildControlsCreated set = &quot;ok&quot;</code> |
| <code>Api.Control.ClearCachedClientID</code> | <code>Control.ClearCachedClientID = &quot;ok&quot;</code><br><code>Control.ClearCachedClientID after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bord…</code> |
| <code>Api.Control.ClearChildControlState</code> | <code>Control.ClearChildControlState = &quot;ok&quot;</code><br><code>Control.ClearChildControlState after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; B…</code> |
| <code>Api.Control.ClearChildState</code> | <code>Control.ClearChildState = &quot;ok&quot;</code><br><code>Control.ClearChildState after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSt…</code> |
| <code>Api.Control.ClearChildViewState</code> | <code>Control.ClearChildViewState = &quot;ok&quot;</code><br><code>Control.ClearChildViewState after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bord…</code> |
| <code>Api.Control.ClearEffectiveClientIDMode</code> | <code>Control.ClearEffectiveClientIDMode = &quot;ok&quot;</code><br><code>Control.ClearEffectiveClientIDMode after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty…</code> |
| <code>Api.Control.ClientID</code> | <code>Control.ClientID = &quot;trRow&quot;</code> |
| <code>Api.Control.ClientIDMode</code> | <code>Control.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>Control.ClientIDMode set = &quot;ClientIDMode.AutoID&quot;</code> |
| <code>Api.Control.ClientIDSeparator</code> | <code>Control.ClientIDSeparator = &quot;&#39;_&#39;&quot;</code> |
| <code>Api.Control.Context</code> | <code>Control.Context = null</code> |
| <code>Api.Control.Controls</code> | <code>Control.Controls = ControlCollection [TableCell#, TableCell#]</code> |
| <code>Api.Control.CreateChildControls</code> | <code>Control.CreateChildControls = &quot;ok&quot;</code><br><code>Control.CreateChildControls after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bord…</code> |
| <code>Api.Control.CreateControlCollection</code> | <code>Control.CreateControlCollection = ControlCollection []</code><br><code>Control.CreateControlCollection after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; …</code> |
| <code>Api.Control.DataBind</code> | <code>Control.DataBind = &quot;ok&quot;</code><br><code>Control.DataBind after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=Not…</code><br><code>Control.DataBind(Boolean) = &quot;ok&quot;</code><br><code>Control.DataBind(Boolean) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Border…</code> |
| <code>Api.Control.DataBindChildren</code> | <code>Control.DataBindChildren = &quot;ok&quot;</code><br><code>Control.DataBindChildren after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderS…</code> |
| <code>Api.Control.DataBinding</code> | <code>Control.DataBinding = &quot;added and removed&quot;</code> |
| <code>Api.Control.DataItemContainer</code> | <code>Control.DataItemContainer = null</code> |
| <code>Api.Control.DataKeysContainer</code> | <code>Control.DataKeysContainer = null</code> |
| <code>Api.Control.DesignMode</code> | <code>Control.DesignMode = &quot;false&quot;</code> |
| <code>Api.Control.Dispose</code> | <code>Control.Dispose = &quot;ok&quot;</code><br><code>Control.Dispose after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotS…</code> |
| <code>Api.Control.Disposed</code> | <code>Control.Disposed = &quot;added and removed&quot;</code> |
| <code>Api.Control.EnableTheming</code> | <code>Control.EnableTheming = &quot;true&quot;</code><br><code>Control.EnableTheming set = &quot;true&quot;</code> |
| <code>Api.Control.EnableViewState</code> | <code>Control.EnableViewState = &quot;true&quot;</code><br><code>Control.EnableViewState set = &quot;true&quot;</code> |
| <code>Api.Control.EndRenderTracing</code> | <code>Control.EndRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.EndRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.EndRenderTracing(TextWriter,Object) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Col…</code> |
| <code>Api.Control.EnsureChildControls</code> | <code>Control.EnsureChildControls = &quot;ok&quot;</code><br><code>Control.EnsureChildControls after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bord…</code> |
| <code>Api.Control.EnsureID</code> | <code>Control.EnsureID = &quot;ok&quot;</code><br><code>Control.EnsureID after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=Not…</code> |
| <code>Api.Control.Events</code> | <code>Control.Events = &quot;System.ComponentModel.EventHandlerList&quot;</code> |
| <code>Api.Control.FindControl</code> | <code>Control.FindControl(String) = Label#lblTitle {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code><br><code>Control.FindControl(String) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bord…</code><br><code>Control.FindControl(String,Int32) = null</code><br><code>Control.FindControl(String,Int32) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]…</code> |
| <code>Api.Control.Focus</code> | <code>Control.Focus = &quot;-&gt; System.InvalidOperationException: A form tag with runat=server must exist on the Page to use SetFocus() or the Focus property.&quot;</code> |
| <code>Api.Control.GetDesignModeState</code> | <code>Control.GetDesignModeState = &quot;[]&quot;</code><br><code>Control.GetDesignModeState after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Borde…</code> |
| <code>Api.Control.GetRouteUrl</code> | <code>Control.GetRouteUrl(Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.GetUniqueIDRelativeTo</code> | <code>Control.GetUniqueIDRelativeTo(Control) = &quot;trRow&quot;</code><br><code>Control.GetUniqueIDRelativeTo(Control) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [E…</code> |
| <code>Api.Control.HasChildViewState</code> | <code>Control.HasChildViewState = &quot;false&quot;</code> |
| <code>Api.Control.HasControls</code> | <code>Control.HasControls = &quot;true&quot;</code><br><code>Control.HasControls after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=…</code> |
| <code>Api.Control.HasEvents</code> | <code>Control.HasEvents = &quot;false&quot;</code><br><code>Control.HasEvents after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=No…</code> |
| <code>Api.Control.ID</code> | <code>Control.ID = &quot;trRow&quot;</code><br><code>Control.ID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.IdSeparator</code> | <code>Control.IdSeparator = &quot;&#39;$&#39;&quot;</code> |
| <code>Api.Control.Init</code> | <code>Control.Init = &quot;added and removed&quot;</code> |
| <code>Api.Control.IsChildControlStateCleared</code> | <code>Control.IsChildControlStateCleared = &quot;false&quot;</code> |
| <code>Api.Control.IsLiteralContent</code> | <code>Control.IsLiteralContent = &quot;false&quot;</code><br><code>Control.IsLiteralContent after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderS…</code> |
| <code>Api.Control.IsTrackingViewState</code> | <code>Control.IsTrackingViewState = &quot;false&quot;</code> |
| <code>Api.Control.IsViewStateEnabled</code> | <code>Control.IsViewStateEnabled = &quot;true&quot;</code> |
| <code>Api.Control.Load</code> | <code>Control.Load = &quot;added and removed&quot;</code> |
| <code>Api.Control.LoadControlState</code> | <code>Control.LoadControlState(Object) = &quot;ok&quot;</code><br><code>Control.LoadControlState(Object) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty];…</code> |
| <code>Api.Control.LoadViewState</code> | <code>Control.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Web.UI.Pair&#39;.&quot;</code> |
| <code>Api.Control.LoadViewStateByID</code> | <code>Control.LoadViewStateByID = &quot;false&quot;</code> |
| <code>Api.Control.MapPathSecure</code> | <code>Control.MapPathSecure(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.NamingContainer</code> | <code>Control.NamingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code> |
| <code>Api.Control.OnBubbleEvent</code> | <code>Control.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Control.OnBubbleEvent(Object,EventArgs) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code> |
| <code>Api.Control.OnDataBinding</code> | <code>Control.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnDataBinding(EventArgs) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty];…</code> |
| <code>Api.Control.OnInit</code> | <code>Control.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnInit(EventArgs) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Border…</code> |
| <code>Api.Control.OnLoad</code> | <code>Control.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnLoad(EventArgs) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Border…</code> |
| <code>Api.Control.OnPreRender</code> | <code>Control.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnPreRender(EventArgs) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; B…</code> |
| <code>Api.Control.OnUnload</code> | <code>Control.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnUnload(EventArgs) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bord…</code> |
| <code>Api.Control.OpenFile</code> | <code>Control.OpenFile(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.Page</code> | <code>Control.Page = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=&quot;&quot;; …</code><br><code>Control.Page set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=…</code> |
| <code>Api.Control.Parent</code> | <code>Control.Parent = HtmlForm#form1 {Action=&quot;&quot;; ClientID=&quot;form1&quot;; ClientIDMode=Inherit; DefaultButton=&quot;&quot;; DefaultFocus=&quot;&quot;; Disabled=false; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.PreRender</code> | <code>Control.PreRender = &quot;added and removed&quot;</code> |
| <code>Api.Control.RaiseBubbleEvent</code> | <code>Control.RaiseBubbleEvent(Object,EventArgs) = &quot;ok&quot;</code><br><code>Control.RaiseBubbleEvent(Object,EventArgs) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Colo…</code> |
| <code>Api.Control.RemovedControl</code> | <code>Control.RemovedControl(Control) = &quot;ok&quot;</code><br><code>Control.RemovedControl(Control) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; …</code> |
| <code>Api.Control.Render</code> | <code>Control.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.Render(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;tr id=&quot;trRow&quot;&gt;</code><br><code>  &#124; 	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;</code><br><code>  &#124; &lt;/tr&gt;</code><br><code>Control.Render(HtmlTextWriter) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; B…</code> |
| <code>Api.Control.RenderChildren</code> | <code>Control.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;</code><br><code>Control.RenderChildren(HtmlTextWriter) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [E…</code> |
| <code>Api.Control.RenderControl</code> | <code>Control.RenderControl(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;tr id=&quot;trRow&quot;&gt;</code><br><code>  &#124; 	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;</code><br><code>  &#124; &lt;/tr&gt;</code><br><code>Control.RenderControl(HtmlTextWriter) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Em…</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) writer:</code><br><code>  &#124; &lt;tr id=&quot;trRow&quot;&gt;</code><br><code>  &#124; 	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;</code><br><code>  &#124; &lt;/tr&gt;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=C…</code> |
| <code>Api.Control.RenderingCompatibility</code> | <code>Control.RenderingCompatibility = &quot;System.Version 4.0&quot;</code><br><code>Control.RenderingCompatibility set = &quot;System.Version 4.0&quot;</code> |
| <code>Api.Control.ResolveAdapter</code> | <code>Control.ResolveAdapter = null</code><br><code>Control.ResolveAdapter after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSty…</code> |
| <code>Api.Control.ResolveClientUrl</code> | <code>Control.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveClientUrl(String) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty];…</code> |
| <code>Api.Control.ResolveUrl</code> | <code>Control.ResolveUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveUrl(String) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Borde…</code> |
| <code>Api.Control.SaveControlState</code> | <code>Control.SaveControlState = null</code><br><code>Control.SaveControlState after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderS…</code> |
| <code>Api.Control.SaveViewState</code> | <code>Control.SaveViewState = null</code><br><code>Control.SaveViewState after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyl…</code> |
| <code>Api.Control.SetDesignModeState</code> | <code>Control.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>Control.SetDesignModeState(IDictionary) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code> |
| <code>Api.Control.SetRenderMethodDelegate</code> | <code>Control.SetRenderMethodDelegate(RenderMethod) = &quot;ok&quot;</code><br><code>Control.SetRenderMethodDelegate(RenderMethod) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 C…</code> |
| <code>Api.Control.SetTraceData</code> | <code>Control.SetTraceData(Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empt…</code><br><code>Control.SetTraceData(Object,Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object,Object) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Colo…</code> |
| <code>Api.Control.Site</code> | <code>Control.Site = null</code><br><code>Control.Site set = null</code> |
| <code>Api.Control.SkinID</code> | <code>Control.SkinID = &quot;&quot;</code><br><code>Control.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.TemplateControl</code> | <code>Control.TemplateControl = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code><br><code>Control.TemplateControl set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Cl…</code> |
| <code>Api.Control.TemplateSourceDirectory</code> | <code>Control.TemplateSourceDirectory = &quot;&quot;</code> |
| <code>Api.Control.TrackViewState</code> | <code>Control.TrackViewState = &quot;ok&quot;</code><br><code>Control.TrackViewState after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSty…</code> |
| <code>Api.Control.UniqueID</code> | <code>Control.UniqueID = &quot;trRow&quot;</code> |
| <code>Api.Control.Unload</code> | <code>Control.Unload = &quot;added and removed&quot;</code> |
| <code>Api.Control.ValidateRequestMode</code> | <code>Control.ValidateRequestMode = &quot;ValidateRequestMode.Inherit&quot;</code><br><code>Control.ValidateRequestMode set = &quot;ValidateRequestMode.Disabled&quot;</code> |
| <code>Api.Control.ViewState</code> | <code>Control.ViewState = StateBag []</code> |
| <code>Api.Control.ViewStateIgnoresCase</code> | <code>Control.ViewStateIgnoresCase = &quot;false&quot;</code> |
| <code>Api.Control.ViewStateMode</code> | <code>Control.ViewStateMode = &quot;ViewStateMode.Inherit&quot;</code><br><code>Control.ViewStateMode set = &quot;ViewStateMode.Enabled&quot;</code> |
| <code>Api.Control.Visible</code> | <code>Control.Visible = &quot;true&quot;</code><br><code>Control.Visible set = &quot;true&quot;</code> |
| <code>Api.IAttributeAccessor.GetAttribute</code> | <code>IAttributeAccessor.GetAttribute(String) = null</code><br><code>IAttributeAccessor.GetAttribute(String) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code> |
| <code>Api.IAttributeAccessor.SetAttribute</code> | <code>IAttributeAccessor.SetAttribute(String,String) = &quot;ok&quot;</code><br><code>IAttributeAccessor.SetAttribute(String,String) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 …</code> |
| <code>Api.IControlBuilderAccessor.ControlBuilder</code> | <code>IControlBuilderAccessor.ControlBuilder = null</code> |
| <code>Api.IControlDesignerAccessor.GetDesignModeState</code> | <code>IControlDesignerAccessor.GetDesignModeState = &quot;[]&quot;</code><br><code>IControlDesignerAccessor.GetDesignModeState after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Col…</code> |
| <code>Api.IControlDesignerAccessor.SetDesignModeState</code> | <code>IControlDesignerAccessor.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetDesignModeState(IDictionary) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color …</code> |
| <code>Api.IControlDesignerAccessor.SetOwnerControl</code> | <code>IControlDesignerAccessor.SetOwnerControl(Control) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetOwnerControl(Control) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #000000…</code> |
| <code>Api.IControlDesignerAccessor.UserData</code> | <code>IControlDesignerAccessor.UserData = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.DataBindings</code> | <code>IDataBindingsAccessor.DataBindings = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.HasDataBindings</code> | <code>IDataBindingsAccessor.HasDataBindings = &quot;false&quot;</code> |
| <code>Api.IExpressionsAccessor.Expressions</code> | <code>IExpressionsAccessor.Expressions = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.HasExpressions</code> | <code>IExpressionsAccessor.HasExpressions = &quot;false&quot;</code> |
| <code>Api.IParserAccessor.AddParsedSubObject</code> | <code>IParserAccessor.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>IParserAccessor.AddParsedSubObject(Object) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Colo…</code> |
| <code>Api.IUrlResolutionService.ResolveClientUrl</code> | <code>IUrlResolutionService.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>IUrlResolutionService.ResolveClientUrl(String) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 …</code> |
| <code>Api.WebControls.TableFooterRow.#ctor</code> | <code>TableFooterRow.#ctor = TableFooterRow# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; Bo…</code> |
| <code>Api.WebControls.TableRow.#ctor</code> | <code>TableRow.#ctor = TableRow# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; …</code> |
| <code>Api.WebControls.TableRow.Cells</code> | <code>TableRow.Cells = &quot;[System.Web.UI.WebControls.TableCell, System.Web.UI.WebControls.TableCell]&quot;</code> |
| <code>Api.WebControls.TableRow.CreateControlCollection</code> | <code>TableRow.CreateControlCollection = ControlCollection []</code><br><code>TableRow.CreateControlCollection after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty];…</code> |
| <code>Api.WebControls.TableRow.CreateControlStyle</code> | <code>TableRow.CreateControlStyle = TableItemStyle {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidt…</code><br><code>TableRow.CreateControlStyle after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bord…</code> |
| <code>Api.WebControls.TableRow.HorizontalAlign</code> | <code>TableRow.HorizontalAlign = &quot;HorizontalAlign.NotSet&quot;</code><br><code>TableRow.HorizontalAlign set = &quot;HorizontalAlign.Left&quot;</code> |
| <code>Api.WebControls.TableRow.SupportsDisabledAttribute</code> | <code>TableRow.SupportsDisabledAttribute = &quot;false&quot;</code> |
| <code>Api.WebControls.TableRow.TableSection</code> | <code>TableRow.TableSection = &quot;TableRowSection.TableBody&quot;</code><br><code>TableRow.TableSection set = &quot;TableRowSection.TableBody&quot;</code> |
| <code>Api.WebControls.TableRow.VerticalAlign</code> | <code>TableRow.VerticalAlign = &quot;VerticalAlign.NotSet&quot;</code><br><code>TableRow.VerticalAlign set = &quot;VerticalAlign.Top&quot;</code> |
| <code>Api.WebControls.WebControl.#ctor</code> | <code>WebControl.#ctor = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidt…</code><br><code>WebControl.#ctor(String) = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; Bo…</code><br><code>WebControl.#ctor(HtmlTextWriterTag) = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle…</code> |
| <code>Api.WebControls.WebControl.AccessKey</code> | <code>WebControl.AccessKey = &quot;&quot;</code><br><code>WebControl.AccessKey set = &quot;-&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]&quot;</code> |
| <code>Api.WebControls.WebControl.AddAttributesToRender</code> | <code>WebControl.AddAttributesToRender(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.AddAttributesToRender(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>WebControl.AddAttributesToRender(HtmlTextWriter) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #0000000…</code> |
| <code>Api.WebControls.WebControl.ApplyStyle</code> | <code>WebControl.ApplyStyle(Style) = &quot;ok&quot;</code><br><code>WebControl.ApplyStyle(Style) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bor…</code> |
| <code>Api.WebControls.WebControl.Attributes</code> | <code>WebControl.Attributes = AttributeCollection []</code> |
| <code>Api.WebControls.WebControl.BackColor</code> | <code>WebControl.BackColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>WebControl.BackColor set = &quot;Color #FF3366CC Color [A=255, R=51, G=102, B=204]&quot;</code> |
| <code>Api.WebControls.WebControl.BorderColor</code> | <code>WebControl.BorderColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>WebControl.BorderColor set = &quot;Color #FF3366CC Color [A=255, R=51, G=102, B=204]&quot;</code> |
| <code>Api.WebControls.WebControl.BorderStyle</code> | <code>WebControl.BorderStyle = &quot;BorderStyle.NotSet&quot;</code><br><code>WebControl.BorderStyle set = &quot;BorderStyle.None&quot;</code> |
| <code>Api.WebControls.WebControl.BorderWidth</code> | <code>WebControl.BorderWidth = Unit </code><br><code>WebControl.BorderWidth set = Unit 120px</code> |
| <code>Api.WebControls.WebControl.ControlStyle</code> | <code>WebControl.ControlStyle = TableItemStyle {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; …</code> |
| <code>Api.WebControls.WebControl.ControlStyleCreated</code> | <code>WebControl.ControlStyleCreated = &quot;false&quot;</code> |
| <code>Api.WebControls.WebControl.CopyBaseAttributes</code> | <code>WebControl.CopyBaseAttributes(WebControl) = &quot;ok&quot;</code><br><code>WebControl.CopyBaseAttributes(WebControl) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color…</code> |
| <code>Api.WebControls.WebControl.CreateControlStyle</code> | <code>WebControl.CreateControlStyle = TableItemStyle {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWi…</code><br><code>WebControl.CreateControlStyle after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bo…</code> |
| <code>Api.WebControls.WebControl.CssClass</code> | <code>WebControl.CssClass = &quot;&quot;</code><br><code>WebControl.CssClass set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.DisabledCssClass</code> | <code>WebControl.DisabledCssClass = &quot;aspNetDisabled&quot;</code><br><code>WebControl.DisabledCssClass set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.EnableTheming</code> | <code>WebControl.EnableTheming = &quot;true&quot;</code><br><code>WebControl.EnableTheming set = &quot;true&quot;</code> |
| <code>Api.WebControls.WebControl.Enabled</code> | <code>WebControl.Enabled = &quot;true&quot;</code><br><code>WebControl.Enabled set = &quot;true&quot;</code> |
| <code>Api.WebControls.WebControl.Font</code> | <code>WebControl.Font = FontInfo {Bold=false; Italic=false; Name=&quot;&quot;; Overline=false; Size=; Strikeout=false; Underline=false}</code> |
| <code>Api.WebControls.WebControl.ForeColor</code> | <code>WebControl.ForeColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>WebControl.ForeColor set = &quot;Color #FF3366CC Color [A=255, R=51, G=102, B=204]&quot;</code> |
| <code>Api.WebControls.WebControl.HasAttributes</code> | <code>WebControl.HasAttributes = &quot;false&quot;</code> |
| <code>Api.WebControls.WebControl.Height</code> | <code>WebControl.Height = Unit </code><br><code>WebControl.Height set = Unit 120px</code> |
| <code>Api.WebControls.WebControl.IsEnabled</code> | <code>WebControl.IsEnabled = &quot;true&quot;</code> |
| <code>Api.WebControls.WebControl.LoadViewState</code> | <code>WebControl.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Web.UI.Pair&#39;.&quot;</code> |
| <code>Api.WebControls.WebControl.MergeStyle</code> | <code>WebControl.MergeStyle(Style) = &quot;ok&quot;</code><br><code>WebControl.MergeStyle(Style) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bor…</code> |
| <code>Api.WebControls.WebControl.Render</code> | <code>WebControl.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.Render(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;tr id=&quot;trRow&quot;&gt;</code><br><code>  &#124; 	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;</code><br><code>  &#124; &lt;/tr&gt;</code><br><code>WebControl.Render(HtmlTextWriter) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]…</code> |
| <code>Api.WebControls.WebControl.RenderBeginTag</code> | <code>WebControl.RenderBeginTag(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.RenderBeginTag(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;tr id=&quot;trRow&quot;&gt;</code><br><code>  &#124; </code><br><code>WebControl.RenderBeginTag(HtmlTextWriter) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color…</code> |
| <code>Api.WebControls.WebControl.RenderContents</code> | <code>WebControl.RenderContents(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.RenderContents(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;</code><br><code>WebControl.RenderContents(HtmlTextWriter) after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color…</code> |
| <code>Api.WebControls.WebControl.RenderEndTag</code> | <code>WebControl.RenderEndTag(HtmlTextWriter) = &quot;-&gt; System.InvalidOperationException: A PopEndTag was called without a corresponding PushEndTag.&quot;</code> |
| <code>Api.WebControls.WebControl.SaveViewState</code> | <code>WebControl.SaveViewState = null</code><br><code>WebControl.SaveViewState after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderS…</code> |
| <code>Api.WebControls.WebControl.SkinID</code> | <code>WebControl.SkinID = &quot;&quot;</code><br><code>WebControl.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.Style</code> | <code>WebControl.Style = CssStyleCollection {Count=Int32 0; Value=null}</code> |
| <code>Api.WebControls.WebControl.SupportsDisabledAttribute</code> | <code>WebControl.SupportsDisabledAttribute = &quot;false&quot;</code> |
| <code>Api.WebControls.WebControl.TabIndex</code> | <code>WebControl.TabIndex = &quot;Int16 0&quot;</code><br><code>WebControl.TabIndex set = &quot;Int16 1&quot;</code> |
| <code>Api.WebControls.WebControl.TagKey</code> | <code>WebControl.TagKey = &quot;HtmlTextWriterTag.Tr&quot;</code> |
| <code>Api.WebControls.WebControl.TagName</code> | <code>WebControl.TagName = &quot;tr&quot;</code> |
| <code>Api.WebControls.WebControl.ToolTip</code> | <code>WebControl.ToolTip = &quot;&quot;</code><br><code>WebControl.ToolTip set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.TrackViewState</code> | <code>WebControl.TrackViewState = &quot;ok&quot;</code><br><code>WebControl.TrackViewState after = TableFooterRow#trRow {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Border…</code> |
| <code>Api.WebControls.WebControl.Width</code> | <code>WebControl.Width = Unit </code><br><code>WebControl.Width set = Unit 120px</code> |

## ② HTML 層

プローブサイトの RenderProbe.aspx で、1 ケース 1 リクエスト（ページライフサイクル全体）で描画したページ全体を比較します。下表はコンポーネント部分（div#probe）、設定時の例外、保存された ViewState の要約です。

| ケース | 設定 | 期待される描画 | 設定時の例外 |
|---|---|---|---|
| <code>.(default)</code> | <code>(nothing set)</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.AccessKey=text</code> | <code>c.AccessKey = &quot;probe&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> | <code>c.AccessKey = &quot;probe&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=markup</code> | <code>c.AccessKey = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> | <code>c.AccessKey = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=japanese</code> | <code>c.AccessKey = &quot;日本語テキスト&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> | <code>c.AccessKey = &quot;日本語テキスト&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=empty</code> | <code>c.AccessKey = &quot;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.AccessKey=k</code> | <code>c.AccessKey = &quot;k&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot; accesskey=&quot;k&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.AppRelativeTemplateSourceDirectory=text</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;probe&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=markup</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=japanese</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;日本語テキスト&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=empty</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.BackColor=Red</code> | <code>c.BackColor = Color.Red;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;background-color:Red;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.BackColor=#3366CC</code> | <code>c.BackColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;background-color:#3366CC;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.BackColor=Transparent</code> | <code>c.BackColor = Color.Transparent;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;background-color:Transparent;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.BorderColor=Red</code> | <code>c.BorderColor = Color.Red;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;border-color:Red;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.BorderColor=#3366CC</code> | <code>c.BorderColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;border-color:#3366CC;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.BorderColor=Transparent</code> | <code>c.BorderColor = Color.Transparent;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;border-color:Transparent;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.BorderStyle=NotSet</code> | <code>c.BorderStyle = BorderStyle.NotSet;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.BorderStyle=None</code> | <code>c.BorderStyle = BorderStyle.None;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;border-style:None;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.BorderStyle=Dotted</code> | <code>c.BorderStyle = BorderStyle.Dotted;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;border-style:Dotted;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.BorderStyle=Dashed</code> | <code>c.BorderStyle = BorderStyle.Dashed;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;border-style:Dashed;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.BorderStyle=Solid</code> | <code>c.BorderStyle = BorderStyle.Solid;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;border-style:Solid;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.BorderStyle=Double</code> | <code>c.BorderStyle = BorderStyle.Double;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;border-style:Double;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.BorderStyle=Groove</code> | <code>c.BorderStyle = BorderStyle.Groove;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;border-style:Groove;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.BorderStyle=Ridge</code> | <code>c.BorderStyle = BorderStyle.Ridge;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;border-style:Ridge;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.BorderStyle=Inset</code> | <code>c.BorderStyle = BorderStyle.Inset;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;border-style:Inset;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.BorderStyle=Outset</code> | <code>c.BorderStyle = BorderStyle.Outset;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;border-style:Outset;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.BorderWidth=120px</code> | <code>c.BorderWidth = Unit.Pixel(120);</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;border-width:120px;border-style:solid;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.BorderWidth=50%</code> | <code>c.BorderWidth = Unit.Percentage(50);</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> | <code>c.BorderWidth = Unit.Percentage(50); -&gt; System.ArgumentOutOfRangeException: BorderWidth must be a non-negative number and cannot be a percentage. [param value]</code> |
| <code>.BorderWidth=2em</code> | <code>c.BorderWidth = new Unit(2, UnitType.Em);</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;border-width:2em;border-style:solid;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.ClientIDMode=Inherit</code> | <code>c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.ClientIDMode=AutoID</code> | <code>c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.ClientIDMode=Predictable</code> | <code>c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.ClientIDMode=Static</code> | <code>c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.CssClass=text</code> | <code>c.CssClass = &quot;probe&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot; class=&quot;probe&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.CssClass=markup</code> | <code>c.CssClass = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot; class=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.CssClass=japanese</code> | <code>c.CssClass = &quot;日本語テキスト&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot; class=&quot;日本語テキスト&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.CssClass=empty</code> | <code>c.CssClass = &quot;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.EnableTheming=true</code> | <code>c.EnableTheming = true;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.EnableTheming=false</code> | <code>c.EnableTheming = false;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.EnableViewState=true</code> | <code>c.EnableViewState = true;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.EnableViewState=false</code> | <code>c.EnableViewState = false;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Enabled=true</code> | <code>c.Enabled = true;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Enabled=false</code> | <code>c.Enabled = false;</code> | <code>&lt;tr id=&quot;trRow&quot; class=&quot;aspNetDisabled&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Font.Bold=true</code> | <code>c.Font.Bold = true;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;font-weight:bold;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Font.Bold=false</code> | <code>c.Font.Bold = false;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;font-weight:normal;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Font.Italic=true</code> | <code>c.Font.Italic = true;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;font-style:italic;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Font.Italic=false</code> | <code>c.Font.Italic = false;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;font-style:normal;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Font.Name=text</code> | <code>c.Font.Name = &quot;probe&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;font-family:probe;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Font.Name=markup</code> | <code>c.Font.Name = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;font-family:&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Font.Name=japanese</code> | <code>c.Font.Name = &quot;日本語テキスト&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;font-family:日本語テキスト;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Font.Name=empty</code> | <code>c.Font.Name = &quot;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Font.Names=Arial,MS-Gothic</code> | <code>c.Font.Names = new[] { &quot;Arial&quot;, &quot;MS Gothic&quot; };</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;font-family:Arial,MS Gothic;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Font.Overline=true</code> | <code>c.Font.Overline = true;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;text-decoration: overline;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Font.Overline=false</code> | <code>c.Font.Overline = false;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;text-decoration:none;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Font.Size=12pt</code> | <code>c.Font.Size = FontUnit.Point(12);</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;font-size:12pt;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Font.Size=Large</code> | <code>c.Font.Size = FontUnit.Large;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;font-size:Large;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Font.Size=150%</code> | <code>c.Font.Size = new FontUnit(Unit.Percentage(150));</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;font-size:150%;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Font.Strikeout=true</code> | <code>c.Font.Strikeout = true;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;text-decoration: line-through;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Font.Strikeout=false</code> | <code>c.Font.Strikeout = false;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;text-decoration:none;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Font.Underline=true</code> | <code>c.Font.Underline = true;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;text-decoration:underline;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Font.Underline=false</code> | <code>c.Font.Underline = false;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;text-decoration:none;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.ForeColor=Red</code> | <code>c.ForeColor = Color.Red;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;color:Red;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.ForeColor=#3366CC</code> | <code>c.ForeColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;color:#3366CC;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.ForeColor=Transparent</code> | <code>c.ForeColor = Color.Transparent;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;color:Transparent;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Height=120px</code> | <code>c.Height = Unit.Pixel(120);</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;height:120px;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Height=50%</code> | <code>c.Height = Unit.Percentage(50);</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;height:50%;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Height=2em</code> | <code>c.Height = new Unit(2, UnitType.Em);</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;height:2em;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.HorizontalAlign=NotSet</code> | <code>c.HorizontalAlign = HorizontalAlign.NotSet;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.HorizontalAlign=Left</code> | <code>c.HorizontalAlign = HorizontalAlign.Left;</code> | <code>&lt;tr id=&quot;trRow&quot; align=&quot;left&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.HorizontalAlign=Center</code> | <code>c.HorizontalAlign = HorizontalAlign.Center;</code> | <code>&lt;tr id=&quot;trRow&quot; align=&quot;center&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.HorizontalAlign=Right</code> | <code>c.HorizontalAlign = HorizontalAlign.Right;</code> | <code>&lt;tr id=&quot;trRow&quot; align=&quot;right&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.HorizontalAlign=Justify</code> | <code>c.HorizontalAlign = HorizontalAlign.Justify;</code> | <code>&lt;tr id=&quot;trRow&quot; align=&quot;justify&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.ID=text</code> | <code>c.ID = &quot;probe&quot;;</code> | <code>&lt;tr id=&quot;probe&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.ID=markup</code> | <code>c.ID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;tr id=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.ID=japanese</code> | <code>c.ID = &quot;日本語テキスト&quot;;</code> | <code>&lt;tr id=&quot;日本語テキスト&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.ID=empty</code> | <code>c.ID = &quot;&quot;;</code> | <code>&lt;tr&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.SkinID=text</code> | <code>c.SkinID = &quot;probe&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.SkinID=markup</code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.SkinID=japanese</code> | <code>c.SkinID = &quot;日本語テキスト&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.SkinID=empty</code> | <code>c.SkinID = &quot;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.TabIndex=0</code> | <code>c.TabIndex = 0;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.TabIndex=3</code> | <code>c.TabIndex = 3;</code> | <code>&lt;tr id=&quot;trRow&quot; tabindex=&quot;3&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.TabIndex=-1</code> | <code>c.TabIndex = -1;</code> | <code>&lt;tr id=&quot;trRow&quot; tabindex=&quot;-1&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.TableSection=TableHeader</code> | <code>c.TableSection = TableRowSection.TableHeader;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.TableSection=TableBody</code> | <code>c.TableSection = TableRowSection.TableBody;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.TableSection=TableFooter</code> | <code>c.TableSection = TableRowSection.TableFooter;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.ToolTip=text</code> | <code>c.ToolTip = &quot;probe&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot; title=&quot;probe&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.ToolTip=markup</code> | <code>c.ToolTip = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot; title=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.ToolTip=japanese</code> | <code>c.ToolTip = &quot;日本語テキスト&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot; title=&quot;日本語テキスト&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.ToolTip=empty</code> | <code>c.ToolTip = &quot;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.ValidateRequestMode=Inherit</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Inherit;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.ValidateRequestMode=Disabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Disabled;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.ValidateRequestMode=Enabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Enabled;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.VerticalAlign=NotSet</code> | <code>c.VerticalAlign = VerticalAlign.NotSet;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.VerticalAlign=Top</code> | <code>c.VerticalAlign = VerticalAlign.Top;</code> | <code>&lt;tr id=&quot;trRow&quot; valign=&quot;top&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.VerticalAlign=Middle</code> | <code>c.VerticalAlign = VerticalAlign.Middle;</code> | <code>&lt;tr id=&quot;trRow&quot; valign=&quot;middle&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.VerticalAlign=Bottom</code> | <code>c.VerticalAlign = VerticalAlign.Bottom;</code> | <code>&lt;tr id=&quot;trRow&quot; valign=&quot;bottom&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.ViewStateMode=Inherit</code> | <code>c.ViewStateMode = ViewStateMode.Inherit;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.ViewStateMode=Enabled</code> | <code>c.ViewStateMode = ViewStateMode.Enabled;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.ViewStateMode=Disabled</code> | <code>c.ViewStateMode = ViewStateMode.Disabled;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Visible=true</code> | <code>c.Visible = true;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Visible=false</code> | <code>c.Visible = false;</code> | <code></code> |  |
| <code>.Width=120px</code> | <code>c.Width = Unit.Pixel(120);</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;width:120px;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Width=50%</code> | <code>c.Width = Unit.Percentage(50);</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;width:50%;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Width=2em</code> | <code>c.Width = new Unit(2, UnitType.Em);</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;width:2em;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].AccessKey=text</code> | <code>c.Cells[1].AccessKey = &quot;probe&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> | <code>c.Cells[1].AccessKey = &quot;probe&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.Cells[1].AccessKey=markup</code> | <code>c.Cells[1].AccessKey = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> | <code>c.Cells[1].AccessKey = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.Cells[1].AccessKey=japanese</code> | <code>c.Cells[1].AccessKey = &quot;日本語テキスト&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> | <code>c.Cells[1].AccessKey = &quot;日本語テキスト&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.Cells[1].AccessKey=empty</code> | <code>c.Cells[1].AccessKey = &quot;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].AppRelativeTemplateSourceDirectory=text</code> | <code>c.Cells[1].AppRelativeTemplateSourceDirectory = &quot;probe&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> | <code>c.Cells[1].AppRelativeTemplateSourceDirectory = &quot;probe&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;probe&#39; is not allowed here.</code> |
| <code>.Cells[1].AppRelativeTemplateSourceDirectory=markup</code> | <code>c.Cells[1].AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> | <code>c.Cells[1].AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39; is not allowed here.</code> |
| <code>.Cells[1].AppRelativeTemplateSourceDirectory=japanese</code> | <code>c.Cells[1].AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> | <code>c.Cells[1].AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;日本語テキスト&#39; is not allowed here.</code> |
| <code>.Cells[1].AppRelativeTemplateSourceDirectory=empty</code> | <code>c.Cells[1].AppRelativeTemplateSourceDirectory = &quot;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].AssociatedHeaderCellID=Arial,MS-Gothic</code> | <code>c.Cells[1].AssociatedHeaderCellID = new[] { &quot;Arial&quot;, &quot;MS Gothic&quot; };</code> | <code>status = 500</code> |  |
| <code>.Cells[1].BackColor=Red</code> | <code>c.Cells[1].BackColor = Color.Red;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;background-color:Red;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].BackColor=#3366CC</code> | <code>c.Cells[1].BackColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;background-color:#3366CC;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].BackColor=Transparent</code> | <code>c.Cells[1].BackColor = Color.Transparent;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;background-color:Transparent;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].BorderColor=Red</code> | <code>c.Cells[1].BorderColor = Color.Red;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;border-color:Red;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].BorderColor=#3366CC</code> | <code>c.Cells[1].BorderColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;border-color:#3366CC;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].BorderColor=Transparent</code> | <code>c.Cells[1].BorderColor = Color.Transparent;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;border-color:Transparent;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].BorderStyle=NotSet</code> | <code>c.Cells[1].BorderStyle = BorderStyle.NotSet;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].BorderStyle=None</code> | <code>c.Cells[1].BorderStyle = BorderStyle.None;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;border-style:None;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].BorderStyle=Dotted</code> | <code>c.Cells[1].BorderStyle = BorderStyle.Dotted;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;border-style:Dotted;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].BorderStyle=Dashed</code> | <code>c.Cells[1].BorderStyle = BorderStyle.Dashed;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;border-style:Dashed;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].BorderStyle=Solid</code> | <code>c.Cells[1].BorderStyle = BorderStyle.Solid;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;border-style:Solid;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].BorderStyle=Double</code> | <code>c.Cells[1].BorderStyle = BorderStyle.Double;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;border-style:Double;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].BorderStyle=Groove</code> | <code>c.Cells[1].BorderStyle = BorderStyle.Groove;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;border-style:Groove;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].BorderStyle=Ridge</code> | <code>c.Cells[1].BorderStyle = BorderStyle.Ridge;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;border-style:Ridge;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].BorderStyle=Inset</code> | <code>c.Cells[1].BorderStyle = BorderStyle.Inset;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;border-style:Inset;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].BorderStyle=Outset</code> | <code>c.Cells[1].BorderStyle = BorderStyle.Outset;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;border-style:Outset;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].BorderWidth=120px</code> | <code>c.Cells[1].BorderWidth = Unit.Pixel(120);</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;border-width:120px;border-style:solid;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].BorderWidth=50%</code> | <code>c.Cells[1].BorderWidth = Unit.Percentage(50);</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> | <code>c.Cells[1].BorderWidth = Unit.Percentage(50); -&gt; System.ArgumentOutOfRangeException: BorderWidth must be a non-negative number and cannot be a percentage. [param value]</code> |
| <code>.Cells[1].BorderWidth=2em</code> | <code>c.Cells[1].BorderWidth = new Unit(2, UnitType.Em);</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;border-width:2em;border-style:solid;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].ClientIDMode=Inherit</code> | <code>c.Cells[1].ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].ClientIDMode=AutoID</code> | <code>c.Cells[1].ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].ClientIDMode=Predictable</code> | <code>c.Cells[1].ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].ClientIDMode=Static</code> | <code>c.Cells[1].ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].ColumnSpan=0</code> | <code>c.Cells[1].ColumnSpan = 0;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].ColumnSpan=3</code> | <code>c.Cells[1].ColumnSpan = 3;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td colspan=&quot;3&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].ColumnSpan=-1</code> | <code>c.Cells[1].ColumnSpan = -1;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> | <code>c.Cells[1].ColumnSpan = -1; -&gt; System.ArgumentOutOfRangeException: Specified argument was out of the range of valid values. [param value]</code> |
| <code>.Cells[1].CssClass=text</code> | <code>c.Cells[1].CssClass = &quot;probe&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td class=&quot;probe&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].CssClass=markup</code> | <code>c.Cells[1].CssClass = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td class=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].CssClass=japanese</code> | <code>c.Cells[1].CssClass = &quot;日本語テキスト&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td class=&quot;日本語テキスト&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].CssClass=empty</code> | <code>c.Cells[1].CssClass = &quot;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].EnableTheming=true</code> | <code>c.Cells[1].EnableTheming = true;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].EnableTheming=false</code> | <code>c.Cells[1].EnableTheming = false;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].EnableViewState=true</code> | <code>c.Cells[1].EnableViewState = true;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].EnableViewState=false</code> | <code>c.Cells[1].EnableViewState = false;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Enabled=true</code> | <code>c.Cells[1].Enabled = true;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Enabled=false</code> | <code>c.Cells[1].Enabled = false;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td class=&quot;aspNetDisabled&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Font.Bold=true</code> | <code>c.Cells[1].Font.Bold = true;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;font-weight:bold;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Font.Bold=false</code> | <code>c.Cells[1].Font.Bold = false;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;font-weight:normal;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Font.Italic=true</code> | <code>c.Cells[1].Font.Italic = true;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;font-style:italic;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Font.Italic=false</code> | <code>c.Cells[1].Font.Italic = false;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;font-style:normal;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Font.Name=text</code> | <code>c.Cells[1].Font.Name = &quot;probe&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;font-family:probe;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Font.Name=markup</code> | <code>c.Cells[1].Font.Name = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;font-family:&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Font.Name=japanese</code> | <code>c.Cells[1].Font.Name = &quot;日本語テキスト&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;font-family:日本語テキスト;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Font.Name=empty</code> | <code>c.Cells[1].Font.Name = &quot;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Font.Names=Arial,MS-Gothic</code> | <code>c.Cells[1].Font.Names = new[] { &quot;Arial&quot;, &quot;MS Gothic&quot; };</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;font-family:Arial,MS Gothic;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Font.Overline=true</code> | <code>c.Cells[1].Font.Overline = true;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;text-decoration: overline;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Font.Overline=false</code> | <code>c.Cells[1].Font.Overline = false;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;text-decoration:none;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Font.Size=12pt</code> | <code>c.Cells[1].Font.Size = FontUnit.Point(12);</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;font-size:12pt;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Font.Size=Large</code> | <code>c.Cells[1].Font.Size = FontUnit.Large;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;font-size:Large;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Font.Size=150%</code> | <code>c.Cells[1].Font.Size = new FontUnit(Unit.Percentage(150));</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;font-size:150%;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Font.Strikeout=true</code> | <code>c.Cells[1].Font.Strikeout = true;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;text-decoration: line-through;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Font.Strikeout=false</code> | <code>c.Cells[1].Font.Strikeout = false;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;text-decoration:none;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Font.Underline=true</code> | <code>c.Cells[1].Font.Underline = true;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;text-decoration:underline;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Font.Underline=false</code> | <code>c.Cells[1].Font.Underline = false;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;text-decoration:none;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].ForeColor=Red</code> | <code>c.Cells[1].ForeColor = Color.Red;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;color:Red;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].ForeColor=#3366CC</code> | <code>c.Cells[1].ForeColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;color:#3366CC;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].ForeColor=Transparent</code> | <code>c.Cells[1].ForeColor = Color.Transparent;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;color:Transparent;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Height=120px</code> | <code>c.Cells[1].Height = Unit.Pixel(120);</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;height:120px;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Height=50%</code> | <code>c.Cells[1].Height = Unit.Percentage(50);</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;height:50%;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Height=2em</code> | <code>c.Cells[1].Height = new Unit(2, UnitType.Em);</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;height:2em;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].HorizontalAlign=NotSet</code> | <code>c.Cells[1].HorizontalAlign = HorizontalAlign.NotSet;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].HorizontalAlign=Left</code> | <code>c.Cells[1].HorizontalAlign = HorizontalAlign.Left;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td align=&quot;left&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].HorizontalAlign=Center</code> | <code>c.Cells[1].HorizontalAlign = HorizontalAlign.Center;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td align=&quot;center&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].HorizontalAlign=Right</code> | <code>c.Cells[1].HorizontalAlign = HorizontalAlign.Right;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td align=&quot;right&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].HorizontalAlign=Justify</code> | <code>c.Cells[1].HorizontalAlign = HorizontalAlign.Justify;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td align=&quot;justify&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].ID=text</code> | <code>c.Cells[1].ID = &quot;probe&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td id=&quot;probe&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].ID=markup</code> | <code>c.Cells[1].ID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td id=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].ID=japanese</code> | <code>c.Cells[1].ID = &quot;日本語テキスト&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td id=&quot;日本語テキスト&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].ID=empty</code> | <code>c.Cells[1].ID = &quot;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].RowSpan=0</code> | <code>c.Cells[1].RowSpan = 0;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].RowSpan=3</code> | <code>c.Cells[1].RowSpan = 3;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td rowspan=&quot;3&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].RowSpan=-1</code> | <code>c.Cells[1].RowSpan = -1;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> | <code>c.Cells[1].RowSpan = -1; -&gt; System.ArgumentOutOfRangeException: Specified argument was out of the range of valid values. [param value]</code> |
| <code>.Cells[1].SkinID=text</code> | <code>c.Cells[1].SkinID = &quot;probe&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].SkinID=markup</code> | <code>c.Cells[1].SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].SkinID=japanese</code> | <code>c.Cells[1].SkinID = &quot;日本語テキスト&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].SkinID=empty</code> | <code>c.Cells[1].SkinID = &quot;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].TabIndex=0</code> | <code>c.Cells[1].TabIndex = 0;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].TabIndex=3</code> | <code>c.Cells[1].TabIndex = 3;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td tabindex=&quot;3&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].TabIndex=-1</code> | <code>c.Cells[1].TabIndex = -1;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td tabindex=&quot;-1&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Text=text</code> | <code>c.Cells[1].Text = &quot;probe&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;probe&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Text=markup</code> | <code>c.Cells[1].Text = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Text=japanese</code> | <code>c.Cells[1].Text = &quot;日本語テキスト&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;日本語テキスト&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Text=empty</code> | <code>c.Cells[1].Text = &quot;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].ToolTip=text</code> | <code>c.Cells[1].ToolTip = &quot;probe&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td title=&quot;probe&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].ToolTip=markup</code> | <code>c.Cells[1].ToolTip = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td title=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].ToolTip=japanese</code> | <code>c.Cells[1].ToolTip = &quot;日本語テキスト&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td title=&quot;日本語テキスト&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].ToolTip=empty</code> | <code>c.Cells[1].ToolTip = &quot;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].ValidateRequestMode=Inherit</code> | <code>c.Cells[1].ValidateRequestMode = ValidateRequestMode.Inherit;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].ValidateRequestMode=Disabled</code> | <code>c.Cells[1].ValidateRequestMode = ValidateRequestMode.Disabled;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].ValidateRequestMode=Enabled</code> | <code>c.Cells[1].ValidateRequestMode = ValidateRequestMode.Enabled;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].VerticalAlign=NotSet</code> | <code>c.Cells[1].VerticalAlign = VerticalAlign.NotSet;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].VerticalAlign=Top</code> | <code>c.Cells[1].VerticalAlign = VerticalAlign.Top;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td valign=&quot;top&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].VerticalAlign=Middle</code> | <code>c.Cells[1].VerticalAlign = VerticalAlign.Middle;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td valign=&quot;middle&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].VerticalAlign=Bottom</code> | <code>c.Cells[1].VerticalAlign = VerticalAlign.Bottom;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td valign=&quot;bottom&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].ViewStateMode=Inherit</code> | <code>c.Cells[1].ViewStateMode = ViewStateMode.Inherit;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].ViewStateMode=Enabled</code> | <code>c.Cells[1].ViewStateMode = ViewStateMode.Enabled;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].ViewStateMode=Disabled</code> | <code>c.Cells[1].ViewStateMode = ViewStateMode.Disabled;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Visible=true</code> | <code>c.Cells[1].Visible = true;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Visible=false</code> | <code>c.Cells[1].Visible = false;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Width=120px</code> | <code>c.Cells[1].Width = Unit.Pixel(120);</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;width:120px;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Width=50%</code> | <code>c.Cells[1].Width = Unit.Percentage(50);</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;width:50%;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Width=2em</code> | <code>c.Cells[1].Width = new Unit(2, UnitType.Em);</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;width:2em;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Wrap=true</code> | <code>c.Cells[1].Wrap = true;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Cells[1].Wrap=false</code> | <code>c.Cells[1].Wrap = false;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td style=&quot;white-space:nowrap;&quot;&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Attributes[data-probe]=text</code> | <code>c.Attributes[data-probe] = &quot;probe&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot; data-probe=&quot;probe&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Attributes[onfocus]=text</code> | <code>c.Attributes[onfocus] = &quot;probe&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot; onfocus=&quot;probe&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Attributes[data-probe]=markup</code> | <code>c.Attributes[data-probe] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot; data-probe=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Attributes[onfocus]=markup</code> | <code>c.Attributes[onfocus] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot; onfocus=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Style[color]=red</code> | <code>c.Style[color] = &quot;red&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;color:red;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Style[margin-left]=red</code> | <code>c.Style[margin-left] = &quot;red&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;margin-left:red;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Style[color]=1em</code> | <code>c.Style[color] = &quot;1em&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;color:1em;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Style[margin-left]=1em</code> | <code>c.Style[margin-left] = &quot;1em&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;margin-left:1em;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.TableSection=TableBody+HorizontalAlign=Center</code> | <code>c.TableSection = TableRowSection.TableBody; c.HorizontalAlign = HorizontalAlign.Center;</code> | <code>&lt;tr id=&quot;trRow&quot; align=&quot;center&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Enabled=false+CssClass=text</code> | <code>c.Enabled = false; c.CssClass = &quot;probe&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot; class=&quot;aspNetDisabled probe&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | <code>c.Width = Unit.Pixel(120); c.Height = new Unit(2, UnitType.Em); c.BorderWidth = Unit.Pixel(120); c.BorderStyle = BorderStyle.Solid; c.BackColor = Color.FromArgb(0x33, 0x66, 0xCC); c.ForeColor = Color.Red; c.Font.Bold = true;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;color:Red;background-color:#3366CC;border-width:120px;border-style:Solid;font-weight:bold;height:2em;width:120px;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.ForeColor=#3366CC+Style[color]=red</code> | <code>c.ForeColor = Color.FromArgb(0x33, 0x66, 0xCC); c.Style[color] = &quot;red&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot; style=&quot;color:#3366CC;color:red;&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>.CssClass=text+Visible=false</code> | <code>c.CssClass = &quot;probe&quot;; c.Visible = false;</code> | <code></code> |  |
| <code>.CssClass=text+EnableViewState=false</code> | <code>c.CssClass = &quot;probe&quot;; c.EnableViewState = false;</code> | <code>&lt;tr id=&quot;trRow&quot; class=&quot;probe&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>[naming container].(default)</code> | <code>[naming container] (nothing set)</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;tr id=&quot;box_trRow&quot;&gt;↵		&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=AutoID</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;tr id=&quot;box_trRow&quot;&gt;↵		&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Static</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;tr id=&quot;trRow&quot;&gt;↵		&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Predictable</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;tr id=&quot;box_trRow&quot;&gt;↵		&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Inherit</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;tr id=&quot;box_trRow&quot;&gt;↵		&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ID=text</code> | <code>[naming container] c.ID = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;tr id=&quot;box_probe&quot;&gt;↵		&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/div&gt;</code> |  |
| <code>[themed].(default)</code> | <code>[themed] (nothing set)</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>[themed].SkinID=text</code> | <code>[themed] c.SkinID = &quot;probe&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot; class=&quot;skinned-probe&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>[themed].EnableTheming=false</code> | <code>[themed] c.EnableTheming = false;</code> | <code>&lt;tr id=&quot;trRow&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |
| <code>[themed].CssClass=text</code> | <code>[themed] c.CssClass = &quot;probe&quot;;</code> | <code>&lt;tr id=&quot;trRow&quot; class=&quot;probe&quot;&gt;↵	&lt;td&gt;A&lt;/td&gt;&lt;td&gt;B&lt;/td&gt;↵&lt;/tr&gt;</code> |  |

## ③ シナリオ層

ブラウザ（Playwright）で TableFooterRowScenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。

| # | 操作 | 対象 | 値 |
|---|---|---|---|
| 1 | goto | /MoreScenario.aspx | <code></code> |
| 2 | snapshot |  | **TableFooterRow [render][databind][dynamic] first request: plain, bound, set in code, made in code** |
| 3 | upload | fupPlain | <code>probe.txt:file content</code> |
| 4 | upload | fupMulti | <code>multi.txt:multi content</code> |
| 5 | upload | fupDynamic | <code>dynamic.txt:dynamic content</code> |
| 6 | fill | txtInFirst | <code>in the first view</code> |
| 7 | click | btnSubmit | <code></code> |
| 8 | snapshot |  | **TableFooterRow [viewstate][dynamic] posted: files, view state, what code set (a table&#39;s rows and a literal control&#39;s text are not kept)** |
| 9 | click | btnNext | <code></code> |
| 10 | snapshot |  | **TableFooterRow [viewstate] the next view (NextView bubbled from a button)** |
| 11 | click | btnToThird | <code></code> |
| 12 | click | btnToFirst | <code></code> |
| 13 | snapshot |  | **TableFooterRow [viewstate] switched by ID and by index: the first view&#39;s text box kept its value** |
| 14 | click | btnDynamicOne | <code></code> |
| 15 | snapshot |  | **TableFooterRow [dynamic][viewstate] a dynamic multi-view&#39;s next view** |
| 16 | clicktextin | calPlain | <code>20</code> |
| 17 | snapshot |  | **TableFooterRow [viewstate] a calendar&#39;s day chosen** |
| 18 | clicktextin | calPlain | <code>Week</code> |
| 19 | snapshot |  | **TableFooterRow [viewstate] a calendar&#39;s week chosen (several dates)** |
| 20 | clicktextin | calPlain | <code>&gt;</code> |
| 21 | snapshot |  | **TableFooterRow [viewstate] a calendar&#39;s next month** |
| 22 | clicktextin | calDynamic | <code>21</code> |
| 23 | snapshot |  | **TableFooterRow [viewstate] a dynamic calendar&#39;s day chosen** |
| 24 | evaluate | imapPlain | <code>document.getElementById(&#39;ImageMap&#39; + e.id).querySelector(&#39;area&#39;).click();</code> |
| 25 | waitms |  | <code></code> |
| 26 | snapshot |  | **TableFooterRow [viewstate] an image map&#39;s hot spot clicked (PostBack)** |
| 27 | evaluate | imapDynamic | <code>document.getElementById(&#39;ImageMap&#39; + e.id).querySelector(&#39;area&#39;).click();</code> |
| 28 | waitms |  | <code></code> |
| 29 | snapshot |  | **TableFooterRow [viewstate] a dynamic image map&#39;s hot spot clicked** |
| 30 | click | btnShow | <code></code> |
| 31 | snapshot |  | **TableFooterRow [visible][viewstate] hidden ones shown with what was set while hidden** |
| 32 | click | btnUploadValidate | <code></code> |
| 33 | snapshot |  | **TableFooterRow [viewstate] a file upload validated on the server (g1): no file posted** |
| 34 | evaluate | tblPlain | <code>__doPostBack(e.id, &#39;probe&#39;);</code> |
| 35 | waitms |  | <code></code> |
| 36 | snapshot |  | **TableFooterRow [viewstate] a postback event to a table (not registered for event validation): rejected** |

<details><summary>TableFooterRow [render][databind][dynamic] first request: plain, bound, set in code, made in code（期待される本文）</summary>

```text
Plain table
Name bound
First bound
set in code
Foot
row added in code
FileUpload:
First view:
Localize: localized <b>encoded</b> bound set in code
Substitution: substituted GET
Pen
Bag & box
bound
set in code
AdRotator:
< January 2020 >
Month Sun Mon Tue Wed Thu Fri Sat
Week 29 30 31 1 2 3 4
Week 5 6 7 8 9 10 11
Week 12 13 14 15 16 17 18
Week 19 20 * 21 22 23 24 25
Week 26 27 28 29 30 31 1
Week 2 3 4 5 6 7 8
bound
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
ImageMap:
Dynamic:
Dynamic header
dynamic cell (set in code)
Dynamic foot
Dynamic view Onedynamic localizesubstituted GETPenBag & box
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
dynamic literal (set in code)
Activate vwFirst
ActiveViewChanged mvSteps ActiveViewIndex=0
Request 1: IsPostBack=False
State tblPlain Rows=4 [TableHeaderRow:Name,bound|TableRow:First,bound|TableRow:set in code,set while hidden|TableFooterRow:Foot]
State thBound Text="bound"
State tdBound Text="bound"
State tdCode Text="set in code"
State tdHidden Text="set while hidden" (hidden)
State tblCode Rows=1 [TableRow:row added in code]
State tblHidden Rows=0 [] (hidden)
State fupPlain HasFile=False FileName="" Files=0
State fupMulti HasFile=False FileName="" Files=0
State fupDisabled HasFile=False FileName="" Files=0
State fupHidden HasFile=False FileName="" Files=0 (hidden)
State mvSteps ActiveViewIndex=0 Active=vwFirst
State txtInFirst Text=""
State lblInSecond Text="bound" (hidden)
State mvHidden ActiveViewIndex=0 Active=vwInHidden (hidden)
State locPlain Text="<b>localized</b>"
State locBound Text="bound"
State locCode Text="set in code"
State locHidden Text="set while hidden" (hidden)
State xmlPlain DocumentContent="" DocumentSource="~/App_Data/probe.xml"
State xmlBound DocumentContent="<items><item>bound</item></items>" DocumentSource=""
State xmlCode DocumentContent="<items><item>set in code</item></items>" DocumentSource=""
State adPlain AdvertisementFile="~/App_Data/ads.xml"
State adBound AdvertisementFile=""
State adHidden AdvertisementFile="~/App_Data/ads.xml" (hidden)
State calPlain VisibleDate=2020-01-15 SelectedDate=0001-01-01 Selected=0
State calBound VisibleDate=2020-01-15 SelectedDate=0001-01-01 Selected=0
State calHidden VisibleDate=0001-01-01 SelectedDate=0001-01-01 Selected=0 (hidden)
State imapPlain HotSpots=2 AlternateText="Areas"
State imapBound HotSpots=0 AlternateText="bound"
State imapHidden HotSpots=0 AlternateText="" (hidden)
State tblDynamic Rows=3 [TableHeaderRow:Dynamic header|TableRow:dynamic cell (set in code)|TableFooterRow:Dynamic foot]
State fupDynamic HasFile=False FileName="" Files=0
State mvDynamic ActiveViewIndex=0 Active=vwDynamicOne
State locDynamic Text="dynamic localize"
State subDynamic
State xmlDynamic DocumentContent="" DocumentSource="~/App_Data/probe.xml"
State adDynamic AdvertisementFile="~/App_Data/ads.xml"
State calDynamic VisibleDate=2020-01-15 SelectedDate=0001-01-01 Selected=0
State imapDynamic HotSpots=1 AlternateText=""
State litDynamic Text="dynamic literal (set in code)"
```

</details>

<details><summary>TableFooterRow [viewstate][dynamic] posted: files, view state, what code set (a table&#39;s rows and a literal control&#39;s text are not kept)（期待される本文）</summary>

```text
Plain table
Name bound
First bound
set in code
Foot
FileUpload:
First view:
Localize: localized <b>encoded</b> bound set in code
Substitution: substituted POST
Pen
Bag & box
AdRotator:
< January 2020 >
Month Sun Mon Tue Wed Thu Fri Sat
Week 29 30 31 1 2 3 4
Week 5 6 7 8 9 10 11
Week 12 13 14 15 16 17 18
Week 19 20 * 21 22 23 24 25
Week 26 27 28 29 30 31 1
Week 2 3 4 5 6 7 8
bound
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
ImageMap:
Dynamic:
Dynamic header
dynamic cell (set in code)
Dynamic foot
Dynamic view Onedynamic localizesubstituted POSTPenBag & box
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
dynamic literal
Request 2: IsPostBack=True
IsValid: (not validated)
State tblPlain Rows=4 [TableHeaderRow:Name,bound|TableRow:First,bound|TableRow:set in code,set while hidden|TableFooterRow:Foot]
State thBound Text="bound"
State tdBound Text="bound"
State tdCode Text="set in code"
State tdHidden Text="set while hidden" (hidden)
State tblCode Rows=0 []
State tblHidden Rows=0 [] (hidden)
State fupPlain HasFile=True FileName="probe.txt" Files=1 12 bytes "file content"
State fupMulti HasFile=True FileName="multi.txt" Files=1 13 bytes "multi content"
State fupDisabled HasFile=False FileName="" Files=0
State fupHidden HasFile=False FileName="" Files=0 (hidden)
State mvSteps ActiveViewIndex=0 Active=vwFirst
State txtInFirst Text="in the first view"
State lblInSecond Text="bound" (hidden)
State mvHidden ActiveViewIndex=0 Active=vwInHidden (hidden)
State locPlain Text="<b>localized</b>"
State locBound Text="bound"
State locCode Text="set in code"
State locHidden Text="set while hidden" (hidden)
State xmlPlain DocumentContent="" DocumentSource="~/App_Data/probe.xml"
State xmlBound DocumentContent="" DocumentSource=""
State xmlCode DocumentContent="" DocumentSource=""
State adPlain AdvertisementFile="~/App_Data/ads.xml"
State adBound AdvertisementFile=""
State adHidden AdvertisementFile="~/App_Data/ads.xml" (hidden)
State calPlain VisibleDate=2020-01-15 SelectedDate=0001-01-01 Selected=0
State calBound VisibleDate=2020-01-15 SelectedDate=0001-01-01 Selected=0
State calHidden VisibleDate=0001-01-01 SelectedDate=0001-01-01 Selected=0 (hidden)
State imapPlain HotSpots=2 AlternateText="Areas"
State imapBound HotSpots=0 AlternateText="bound"
State imapHidden HotSpots=0 AlternateText="" (hidden)
State tblDynamic Rows=3 [TableHeaderRow:Dynamic header|TableRow:dynamic cell (set in code)|TableFooterRow:Dynamic foot]
State fupDynamic HasFile=True FileName="dynamic.txt" Files=1 15 bytes "dynamic content"
State mvDynamic ActiveViewIndex=0 Active=vwDynamicOne
State locDynamic Text="dynamic localize"
State subDynamic
State xmlDynamic DocumentContent="" DocumentSource="~/App_Data/probe.xml"
State adDynamic AdvertisementFile="~/App_Data/ads.xml"
State calDynamic VisibleDate=2020-01-15 SelectedDate=0001-01-01 Selected=0
State imapDynamic HotSpots=1 AlternateText=""
State litDynamic Text="dynamic literal"
```

</details>

<details><summary>TableFooterRow [viewstate] the next view (NextView bubbled from a button)（期待される本文）</summary>

```text
Plain table
Name bound
First bound
set in code
Foot
FileUpload:
Second view: bound
Localize: localized <b>encoded</b> bound set in code
Substitution: substituted POST
Pen
Bag & box
AdRotator:
< January 2020 >
Month Sun Mon Tue Wed Thu Fri Sat
Week 29 30 31 1 2 3 4
Week 5 6 7 8 9 10 11
Week 12 13 14 15 16 17 18
Week 19 20 * 21 22 23 24 25
Week 26 27 28 29 30 31 1
Week 2 3 4 5 6 7 8
bound
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
ImageMap:
Dynamic:
Dynamic header
dynamic cell (set in code)
Dynamic foot
Dynamic view Onedynamic localizesubstituted POSTPenBag & box
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
dynamic literal
Request 3: IsPostBack=True
Deactivate vwFirst
Activate vwSecond
ActiveViewChanged mvSteps ActiveViewIndex=1
IsValid: (not validated)
State tblPlain Rows=4 [TableHeaderRow:Name,bound|TableRow:First,bound|TableRow:set in code,set while hidden|TableFooterRow:Foot]
State thBound Text="bound"
State tdBound Text="bound"
State tdCode Text="set in code"
State tdHidden Text="set while hidden" (hidden)
State tblCode Rows=0 []
State tblHidden Rows=0 [] (hidden)
State fupPlain HasFile=False FileName="" Files=1
State fupMulti HasFile=False FileName="" Files=1
State fupDisabled HasFile=False FileName="" Files=0
State fupHidden HasFile=False FileName="" Files=0 (hidden)
State mvSteps ActiveViewIndex=1 Active=vwSecond
State txtInFirst Text="in the first view" (hidden)
State lblInSecond Text="bound"
State mvHidden ActiveViewIndex=0 Active=vwInHidden (hidden)
State locPlain Text="<b>localized</b>"
State locBound Text="bound"
State locCode Text="set in code"
State locHidden Text="set while hidden" (hidden)
State xmlPlain DocumentContent="" DocumentSource="~/App_Data/probe.xml"
State xmlBound DocumentContent="" DocumentSource=""
State xmlCode DocumentContent="" DocumentSource=""
State adPlain AdvertisementFile="~/App_Data/ads.xml"
State adBound AdvertisementFile=""
State adHidden AdvertisementFile="~/App_Data/ads.xml" (hidden)
State calPlain VisibleDate=2020-01-15 SelectedDate=0001-01-01 Selected=0
State calBound VisibleDate=2020-01-15 SelectedDate=0001-01-01 Selected=0
State calHidden VisibleDate=0001-01-01 SelectedDate=0001-01-01 Selected=0 (hidden)
State imapPlain HotSpots=2 AlternateText="Areas"
State imapBound HotSpots=0 AlternateText="bound"
State imapHidden HotSpots=0 AlternateText="" (hidden)
State tblDynamic Rows=3 [TableHeaderRow:Dynamic header|TableRow:dynamic cell (set in code)|TableFooterRow:Dynamic foot]
State fupDynamic HasFile=False FileName="" Files=1
State mvDynamic ActiveViewIndex=0 Active=vwDynamicOne
State locDynamic Text="dynamic localize"
State subDynamic
State xmlDynamic DocumentContent="" DocumentSource="~/App_Data/probe.xml"
State adDynamic AdvertisementFile="~/App_Data/ads.xml"
State calDynamic VisibleDate=2020-01-15 SelectedDate=0001-01-01 Selected=0
State imapDynamic HotSpots=1 AlternateText=""
State litDynamic Text="dynamic literal"
```

</details>

<details><summary>TableFooterRow [viewstate] switched by ID and by index: the first view&#39;s text box kept its value（期待される本文）</summary>

```text
Plain table
Name bound
First bound
set in code
Foot
FileUpload:
First view:
Localize: localized <b>encoded</b> bound set in code
Substitution: substituted POST
Pen
Bag & box
AdRotator:
< January 2020 >
Month Sun Mon Tue Wed Thu Fri Sat
Week 29 30 31 1 2 3 4
Week 5 6 7 8 9 10 11
Week 12 13 14 15 16 17 18
Week 19 20 * 21 22 23 24 25
Week 26 27 28 29 30 31 1
Week 2 3 4 5 6 7 8
bound
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
ImageMap:
Dynamic:
Dynamic header
dynamic cell (set in code)
Dynamic foot
Dynamic view Onedynamic localizesubstituted POSTPenBag & box
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
dynamic literal
Request 5: IsPostBack=True
Deactivate vwThird
Activate vwFirst
ActiveViewChanged mvSteps ActiveViewIndex=0
IsValid: (not validated)
State tblPlain Rows=4 [TableHeaderRow:Name,bound|TableRow:First,bound|TableRow:set in code,set while hidden|TableFooterRow:Foot]
State thBound Text="bound"
State tdBound Text="bound"
State tdCode Text="set in code"
State tdHidden Text="set while hidden" (hidden)
State tblCode Rows=0 []
State tblHidden Rows=0 [] (hidden)
State fupPlain HasFile=False FileName="" Files=1
State fupMulti HasFile=False FileName="" Files=1
State fupDisabled HasFile=False FileName="" Files=0
State fupHidden HasFile=False FileName="" Files=0 (hidden)
State mvSteps ActiveViewIndex=0 Active=vwFirst
State txtInFirst Text="in the first view"
State lblInSecond Text="bound" (hidden)
State mvHidden ActiveViewIndex=0 Active=vwInHidden (hidden)
State locPlain Text="<b>localized</b>"
State locBound Text="bound"
State locCode Text="set in code"
State locHidden Text="set while hidden" (hidden)
State xmlPlain DocumentContent="" DocumentSource="~/App_Data/probe.xml"
State xmlBound DocumentContent="" DocumentSource=""
State xmlCode DocumentContent="" DocumentSource=""
State adPlain AdvertisementFile="~/App_Data/ads.xml"
State adBound AdvertisementFile=""
State adHidden AdvertisementFile="~/App_Data/ads.xml" (hidden)
State calPlain VisibleDate=2020-01-15 SelectedDate=0001-01-01 Selected=0
State calBound VisibleDate=2020-01-15 SelectedDate=0001-01-01 Selected=0
State calHidden VisibleDate=0001-01-01 SelectedDate=0001-01-01 Selected=0 (hidden)
State imapPlain HotSpots=2 AlternateText="Areas"
State imapBound HotSpots=0 AlternateText="bound"
State imapHidden HotSpots=0 AlternateText="" (hidden)
State tblDynamic Rows=3 [TableHeaderRow:Dynamic header|TableRow:dynamic cell (set in code)|TableFooterRow:Dynamic foot]
State fupDynamic HasFile=False FileName="" Files=1
State mvDynamic ActiveViewIndex=0 Active=vwDynamicOne
State locDynamic Text="dynamic localize"
State subDynamic
State xmlDynamic DocumentContent="" DocumentSource="~/App_Data/probe.xml"
State adDynamic AdvertisementFile="~/App_Data/ads.xml"
State calDynamic VisibleDate=2020-01-15 SelectedDate=0001-01-01 Selected=0
State imapDynamic HotSpots=1 AlternateText=""
State litDynamic Text="dynamic literal"
```

</details>

<details><summary>TableFooterRow [dynamic][viewstate] a dynamic multi-view&#39;s next view（期待される本文）</summary>

```text
Plain table
Name bound
First bound
set in code
Foot
FileUpload:
First view:
Localize: localized <b>encoded</b> bound set in code
Substitution: substituted POST
Pen
Bag & box
AdRotator:
< January 2020 >
Month Sun Mon Tue Wed Thu Fri Sat
Week 29 30 31 1 2 3 4
Week 5 6 7 8 9 10 11
Week 12 13 14 15 16 17 18
Week 19 20 * 21 22 23 24 25
Week 26 27 28 29 30 31 1
Week 2 3 4 5 6 7 8
bound
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
ImageMap:
Dynamic:
Dynamic header
dynamic cell (set in code)
Dynamic foot
Dynamic view Twodynamic localizesubstituted POSTPenBag & box
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
dynamic literal
Request 6: IsPostBack=True
ActiveViewChanged mvDynamic ActiveViewIndex=1
IsValid: (not validated)
State tblPlain Rows=4 [TableHeaderRow:Name,bound|TableRow:First,bound|TableRow:set in code,set while hidden|TableFooterRow:Foot]
State thBound Text="bound"
State tdBound Text="bound"
State tdCode Text="set in code"
State tdHidden Text="set while hidden" (hidden)
State tblCode Rows=0 []
State tblHidden Rows=0 [] (hidden)
State fupPlain HasFile=False FileName="" Files=1
State fupMulti HasFile=False FileName="" Files=1
State fupDisabled HasFile=False FileName="" Files=0
State fupHidden HasFile=False FileName="" Files=0 (hidden)
State mvSteps ActiveViewIndex=0 Active=vwFirst
State txtInFirst Text="in the first view"
State lblInSecond Text="bound" (hidden)
State mvHidden ActiveViewIndex=0 Active=vwInHidden (hidden)
State locPlain Text="<b>localized</b>"
State locBound Text="bound"
State locCode Text="set in code"
State locHidden Text="set while hidden" (hidden)
State xmlPlain DocumentContent="" DocumentSource="~/App_Data/probe.xml"
State xmlBound DocumentContent="" DocumentSource=""
State xmlCode DocumentContent="" DocumentSource=""
State adPlain AdvertisementFile="~/App_Data/ads.xml"
State adBound AdvertisementFile=""
State adHidden AdvertisementFile="~/App_Data/ads.xml" (hidden)
State calPlain VisibleDate=2020-01-15 SelectedDate=0001-01-01 Selected=0
State calBound VisibleDate=2020-01-15 SelectedDate=0001-01-01 Selected=0
State calHidden VisibleDate=0001-01-01 SelectedDate=0001-01-01 Selected=0 (hidden)
State imapPlain HotSpots=2 AlternateText="Areas"
State imapBound HotSpots=0 AlternateText="bound"
State imapHidden HotSpots=0 AlternateText="" (hidden)
State tblDynamic Rows=3 [TableHeaderRow:Dynamic header|TableRow:dynamic cell (set in code)|TableFooterRow:Dynamic foot]
State fupDynamic HasFile=False FileName="" Files=1
State mvDynamic ActiveViewIndex=1 Active=vwDynamicTwo
State locDynamic Text="dynamic localize"
State subDynamic
State xmlDynamic DocumentContent="" DocumentSource="~/App_Data/probe.xml"
State adDynamic AdvertisementFile="~/App_Data/ads.xml"
State calDynamic VisibleDate=2020-01-15 SelectedDate=0001-01-01 Selected=0
State imapDynamic HotSpots=1 AlternateText=""
State litDynamic Text="dynamic literal"
```

</details>

<details><summary>TableFooterRow [viewstate] a calendar&#39;s day chosen（期待される本文）</summary>

```text
Plain table
Name bound
First bound
set in code
Foot
FileUpload:
First view:
Localize: localized <b>encoded</b> bound set in code
Substitution: substituted POST
Pen
Bag & box
AdRotator:
< January 2020 >
Month Sun Mon Tue Wed Thu Fri Sat
Week 29 30 31 1 2 3 4
Week 5 6 7 8 9 10 11
Week 12 13 14 15 16 17 18
Week 19 20 * 21 22 23 24 25
Week 26 27 28 29 30 31 1
Week 2 3 4 5 6 7 8
bound
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
ImageMap:
Dynamic:
Dynamic header
dynamic cell (set in code)
Dynamic foot
Dynamic view Twodynamic localizesubstituted POSTPenBag & box
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
dynamic literal
Request 7: IsPostBack=True
SelectionChanged calPlain 2020-01-20
IsValid: (not validated)
State tblPlain Rows=4 [TableHeaderRow:Name,bound|TableRow:First,bound|TableRow:set in code,set while hidden|TableFooterRow:Foot]
State thBound Text="bound"
State tdBound Text="bound"
State tdCode Text="set in code"
State tdHidden Text="set while hidden" (hidden)
State tblCode Rows=0 []
State tblHidden Rows=0 [] (hidden)
State fupPlain HasFile=False FileName="" Files=1
State fupMulti HasFile=False FileName="" Files=1
State fupDisabled HasFile=False FileName="" Files=0
State fupHidden HasFile=False FileName="" Files=0 (hidden)
State mvSteps ActiveViewIndex=0 Active=vwFirst
State txtInFirst Text="in the first view"
State lblInSecond Text="bound" (hidden)
State mvHidden ActiveViewIndex=0 Active=vwInHidden (hidden)
State locPlain Text="<b>localized</b>"
State locBound Text="bound"
State locCode Text="set in code"
State locHidden Text="set while hidden" (hidden)
State xmlPlain DocumentContent="" DocumentSource="~/App_Data/probe.xml"
State xmlBound DocumentContent="" DocumentSource=""
State xmlCode DocumentContent="" DocumentSource=""
State adPlain AdvertisementFile="~/App_Data/ads.xml"
State adBound AdvertisementFile=""
State adHidden AdvertisementFile="~/App_Data/ads.xml" (hidden)
State calPlain VisibleDate=2020-01-15 SelectedDate=2020-01-20 Selected=1
State calBound VisibleDate=2020-01-15 SelectedDate=0001-01-01 Selected=0
State calHidden VisibleDate=0001-01-01 SelectedDate=0001-01-01 Selected=0 (hidden)
State imapPlain HotSpots=2 AlternateText="Areas"
State imapBound HotSpots=0 AlternateText="bound"
State imapHidden HotSpots=0 AlternateText="" (hidden)
State tblDynamic Rows=3 [TableHeaderRow:Dynamic header|TableRow:dynamic cell (set in code)|TableFooterRow:Dynamic foot]
State fupDynamic HasFile=False FileName="" Files=1
State mvDynamic ActiveViewIndex=1 Active=vwDynamicTwo
State locDynamic Text="dynamic localize"
State subDynamic
State xmlDynamic DocumentContent="" DocumentSource="~/App_Data/probe.xml"
State adDynamic AdvertisementFile="~/App_Data/ads.xml"
State calDynamic VisibleDate=2020-01-15 SelectedDate=0001-01-01 Selected=0
State imapDynamic HotSpots=1 AlternateText=""
State litDynamic Text="dynamic literal"
```

</details>

<details><summary>TableFooterRow [viewstate] a calendar&#39;s week chosen（期待される本文）</summary>

```text
Plain table
Name bound
First bound
set in code
Foot
FileUpload:
First view:
Localize: localized <b>encoded</b> bound set in code
Substitution: substituted POST
Pen
Bag & box
AdRotator:
< January 2020 >
Month Sun Mon Tue Wed Thu Fri Sat
Week 29 30 31 1 2 3 4
Week 5 6 7 8 9 10 11
Week 12 13 14 15 16 17 18
Week 19 20 * 21 22 23 24 25
Week 26 27 28 29 30 31 1
Week 2 3 4 5 6 7 8
bound
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
ImageMap:
Dynamic:
Dynamic header
dynamic cell (set in code)
Dynamic foot
Dynamic view Twodynamic localizesubstituted POSTPenBag & box
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
dynamic literal
Request 8: IsPostBack=True
SelectionChanged calPlain 2019-12-29,2019-12-30,2019-12-31,2020-01-01,2020-01-02,2020-01-03,2020-01-04
IsValid: (not validated)
State tblPlain Rows=4 [TableHeaderRow:Name,bound|TableRow:First,bound|TableRow:set in code,set while hidden|TableFooterRow:Foot]
State thBound Text="bound"
State tdBound Text="bound"
State tdCode Text="set in code"
State tdHidden Text="set while hidden" (hidden)
State tblCode Rows=0 []
State tblHidden Rows=0 [] (hidden)
State fupPlain HasFile=False FileName="" Files=1
State fupMulti HasFile=False FileName="" Files=1
State fupDisabled HasFile=False FileName="" Files=0
State fupHidden HasFile=False FileName="" Files=0 (hidden)
State mvSteps ActiveViewIndex=0 Active=vwFirst
State txtInFirst Text="in the first view"
State lblInSecond Text="bound" (hidden)
State mvHidden ActiveViewIndex=0 Active=vwInHidden (hidden)
State locPlain Text="<b>localized</b>"
State locBound Text="bound"
State locCode Text="set in code"
State locHidden Text="set while hidden" (hidden)
State xmlPlain DocumentContent="" DocumentSource="~/App_Data/probe.xml"
State xmlBound DocumentContent="" DocumentSource=""
State xmlCode DocumentContent="" DocumentSource=""
State adPlain AdvertisementFile="~/App_Data/ads.xml"
State adBound AdvertisementFile=""
State adHidden AdvertisementFile="~/App_Data/ads.xml" (hidden)
State calPlain VisibleDate=2020-01-15 SelectedDate=2019-12-29 Selected=7
State calBound VisibleDate=2020-01-15 SelectedDate=0001-01-01 Selected=0
State calHidden VisibleDate=0001-01-01 SelectedDate=0001-01-01 Selected=0 (hidden)
State imapPlain HotSpots=2 AlternateText="Areas"
State imapBound HotSpots=0 AlternateText="bound"
State imapHidden HotSpots=0 AlternateText="" (hidden)
State tblDynamic Rows=3 [TableHeaderRow:Dynamic header|TableRow:dynamic cell (set in code)|TableFooterRow:Dynamic foot]
State fupDynamic HasFile=False FileName="" Files=1
State mvDynamic ActiveViewIndex=1 Active=vwDynamicTwo
State locDynamic Text="dynamic localize"
State subDynamic
State xmlDynamic DocumentContent="" DocumentSource="~/App_Data/probe.xml"
State adDynamic AdvertisementFile="~/App_Data/ads.xml"
State calDynamic VisibleDate=2020-01-15 SelectedDate=0001-01-01 Selected=0
State imapDynamic HotSpots=1 AlternateText=""
State litDynamic Text="dynamic literal"
```

</details>

<details><summary>TableFooterRow [viewstate] a calendar&#39;s next month（期待される本文）</summary>

```text
Plain table
Name bound
First bound
set in code
Foot
FileUpload:
First view:
Localize: localized <b>encoded</b> bound set in code
Substitution: substituted POST
Pen
Bag & box
AdRotator:
< February 2020 >
Month Sun Mon Tue Wed Thu Fri Sat
Week 26 27 28 29 30 31 1
Week 2 3 4 5 6 7 8
Week 9 10 11 12 13 14 15
Week 16 17 18 19 20 * 21 22
Week 23 24 25 26 27 28 29
Week 1 2 3 4 5 6 7
bound
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
ImageMap:
Dynamic:
Dynamic header
dynamic cell (set in code)
Dynamic foot
Dynamic view Twodynamic localizesubstituted POSTPenBag & box
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
dynamic literal
Request 9: IsPostBack=True
VisibleMonthChanged calPlain 2020-01 -> 2020-02
IsValid: (not validated)
State tblPlain Rows=4 [TableHeaderRow:Name,bound|TableRow:First,bound|TableRow:set in code,set while hidden|TableFooterRow:Foot]
State thBound Text="bound"
State tdBound Text="bound"
State tdCode Text="set in code"
State tdHidden Text="set while hidden" (hidden)
State tblCode Rows=0 []
State tblHidden Rows=0 [] (hidden)
State fupPlain HasFile=False FileName="" Files=1
State fupMulti HasFile=False FileName="" Files=1
State fupDisabled HasFile=False FileName="" Files=0
State fupHidden HasFile=False FileName="" Files=0 (hidden)
State mvSteps ActiveViewIndex=0 Active=vwFirst
State txtInFirst Text="in the first view"
State lblInSecond Text="bound" (hidden)
State mvHidden ActiveViewIndex=0 Active=vwInHidden (hidden)
State locPlain Text="<b>localized</b>"
State locBound Text="bound"
State locCode Text="set in code"
State locHidden Text="set while hidden" (hidden)
State xmlPlain DocumentContent="" DocumentSource="~/App_Data/probe.xml"
State xmlBound DocumentContent="" DocumentSource=""
State xmlCode DocumentContent="" DocumentSource=""
State adPlain AdvertisementFile="~/App_Data/ads.xml"
State adBound AdvertisementFile=""
State adHidden AdvertisementFile="~/App_Data/ads.xml" (hidden)
State calPlain VisibleDate=2020-02-01 SelectedDate=2019-12-29 Selected=7
State calBound VisibleDate=2020-01-15 SelectedDate=0001-01-01 Selected=0
State calHidden VisibleDate=0001-01-01 SelectedDate=0001-01-01 Selected=0 (hidden)
State imapPlain HotSpots=2 AlternateText="Areas"
State imapBound HotSpots=0 AlternateText="bound"
State imapHidden HotSpots=0 AlternateText="" (hidden)
State tblDynamic Rows=3 [TableHeaderRow:Dynamic header|TableRow:dynamic cell (set in code)|TableFooterRow:Dynamic foot]
State fupDynamic HasFile=False FileName="" Files=1
State mvDynamic ActiveViewIndex=1 Active=vwDynamicTwo
State locDynamic Text="dynamic localize"
State subDynamic
State xmlDynamic DocumentContent="" DocumentSource="~/App_Data/probe.xml"
State adDynamic AdvertisementFile="~/App_Data/ads.xml"
State calDynamic VisibleDate=2020-01-15 SelectedDate=0001-01-01 Selected=0
State imapDynamic HotSpots=1 AlternateText=""
State litDynamic Text="dynamic literal"
```

</details>

<details><summary>TableFooterRow [viewstate] a dynamic calendar&#39;s day chosen（期待される本文）</summary>

```text
Plain table
Name bound
First bound
set in code
Foot
FileUpload:
First view:
Localize: localized <b>encoded</b> bound set in code
Substitution: substituted POST
Pen
Bag & box
AdRotator:
< February 2020 >
Month Sun Mon Tue Wed Thu Fri Sat
Week 26 27 28 29 30 31 1
Week 2 3 4 5 6 7 8
Week 9 10 11 12 13 14 15
Week 16 17 18 19 20 * 21 22
Week 23 24 25 26 27 28 29
Week 1 2 3 4 5 6 7
bound
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
ImageMap:
Dynamic:
Dynamic header
dynamic cell (set in code)
Dynamic foot
Dynamic view Twodynamic localizesubstituted POSTPenBag & box
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
dynamic literal
Request 10: IsPostBack=True
SelectionChanged calDynamic 2020-01-21
IsValid: (not validated)
State tblPlain Rows=4 [TableHeaderRow:Name,bound|TableRow:First,bound|TableRow:set in code,set while hidden|TableFooterRow:Foot]
State thBound Text="bound"
State tdBound Text="bound"
State tdCode Text="set in code"
State tdHidden Text="set while hidden" (hidden)
State tblCode Rows=0 []
State tblHidden Rows=0 [] (hidden)
State fupPlain HasFile=False FileName="" Files=1
State fupMulti HasFile=False FileName="" Files=1
State fupDisabled HasFile=False FileName="" Files=0
State fupHidden HasFile=False FileName="" Files=0 (hidden)
State mvSteps ActiveViewIndex=0 Active=vwFirst
State txtInFirst Text="in the first view"
State lblInSecond Text="bound" (hidden)
State mvHidden ActiveViewIndex=0 Active=vwInHidden (hidden)
State locPlain Text="<b>localized</b>"
State locBound Text="bound"
State locCode Text="set in code"
State locHidden Text="set while hidden" (hidden)
State xmlPlain DocumentContent="" DocumentSource="~/App_Data/probe.xml"
State xmlBound DocumentContent="" DocumentSource=""
State xmlCode DocumentContent="" DocumentSource=""
State adPlain AdvertisementFile="~/App_Data/ads.xml"
State adBound AdvertisementFile=""
State adHidden AdvertisementFile="~/App_Data/ads.xml" (hidden)
State calPlain VisibleDate=2020-02-01 SelectedDate=2019-12-29 Selected=7
State calBound VisibleDate=2020-01-15 SelectedDate=0001-01-01 Selected=0
State calHidden VisibleDate=0001-01-01 SelectedDate=0001-01-01 Selected=0 (hidden)
State imapPlain HotSpots=2 AlternateText="Areas"
State imapBound HotSpots=0 AlternateText="bound"
State imapHidden HotSpots=0 AlternateText="" (hidden)
State tblDynamic Rows=3 [TableHeaderRow:Dynamic header|TableRow:dynamic cell (set in code)|TableFooterRow:Dynamic foot]
State fupDynamic HasFile=False FileName="" Files=1
State mvDynamic ActiveViewIndex=1 Active=vwDynamicTwo
State locDynamic Text="dynamic localize"
State subDynamic
State xmlDynamic DocumentContent="" DocumentSource="~/App_Data/probe.xml"
State adDynamic AdvertisementFile="~/App_Data/ads.xml"
State calDynamic VisibleDate=2020-01-15 SelectedDate=2020-01-21 Selected=1
State imapDynamic HotSpots=1 AlternateText=""
State litDynamic Text="dynamic literal"
```

</details>

<details><summary>TableFooterRow [viewstate] an image map&#39;s hot spot clicked (PostBack)（期待される本文）</summary>

```text
Plain table
Name bound
First bound
set in code
Foot
FileUpload:
First view:
Localize: localized <b>encoded</b> bound set in code
Substitution: substituted POST
Pen
Bag & box
AdRotator:
< February 2020 >
Month Sun Mon Tue Wed Thu Fri Sat
Week 26 27 28 29 30 31 1
Week 2 3 4 5 6 7 8
Week 9 10 11 12 13 14 15
Week 16 17 18 19 20 * 21 22
Week 23 24 25 26 27 28 29
Week 1 2 3 4 5 6 7
bound
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
ImageMap:
Dynamic:
Dynamic header
dynamic cell (set in code)
Dynamic foot
Dynamic view Twodynamic localizesubstituted POSTPenBag & box
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
dynamic literal
Request 11: IsPostBack=True
Click imapPlain: rect
IsValid: (not validated)
State tblPlain Rows=4 [TableHeaderRow:Name,bound|TableRow:First,bound|TableRow:set in code,set while hidden|TableFooterRow:Foot]
State thBound Text="bound"
State tdBound Text="bound"
State tdCode Text="set in code"
State tdHidden Text="set while hidden" (hidden)
State tblCode Rows=0 []
State tblHidden Rows=0 [] (hidden)
State fupPlain HasFile=False FileName="" Files=1
State fupMulti HasFile=False FileName="" Files=1
State fupDisabled HasFile=False FileName="" Files=0
State fupHidden HasFile=False FileName="" Files=0 (hidden)
State mvSteps ActiveViewIndex=0 Active=vwFirst
State txtInFirst Text="in the first view"
State lblInSecond Text="bound" (hidden)
State mvHidden ActiveViewIndex=0 Active=vwInHidden (hidden)
State locPlain Text="<b>localized</b>"
State locBound Text="bound"
State locCode Text="set in code"
State locHidden Text="set while hidden" (hidden)
State xmlPlain DocumentContent="" DocumentSource="~/App_Data/probe.xml"
State xmlBound DocumentContent="" DocumentSource=""
State xmlCode DocumentContent="" DocumentSource=""
State adPlain AdvertisementFile="~/App_Data/ads.xml"
State adBound AdvertisementFile=""
State adHidden AdvertisementFile="~/App_Data/ads.xml" (hidden)
State calPlain VisibleDate=2020-02-01 SelectedDate=2019-12-29 Selected=7
State calBound VisibleDate=2020-01-15 SelectedDate=0001-01-01 Selected=0
State calHidden VisibleDate=0001-01-01 SelectedDate=0001-01-01 Selected=0 (hidden)
State imapPlain HotSpots=2 AlternateText="Areas"
State imapBound HotSpots=0 AlternateText="bound"
State imapHidden HotSpots=0 AlternateText="" (hidden)
State tblDynamic Rows=3 [TableHeaderRow:Dynamic header|TableRow:dynamic cell (set in code)|TableFooterRow:Dynamic foot]
State fupDynamic HasFile=False FileName="" Files=1
State mvDynamic ActiveViewIndex=1 Active=vwDynamicTwo
State locDynamic Text="dynamic localize"
State subDynamic
State xmlDynamic DocumentContent="" DocumentSource="~/App_Data/probe.xml"
State adDynamic AdvertisementFile="~/App_Data/ads.xml"
State calDynamic VisibleDate=2020-01-15 SelectedDate=2020-01-21 Selected=1
State imapDynamic HotSpots=1 AlternateText=""
State litDynamic Text="dynamic literal"
```

</details>

<details><summary>TableFooterRow [viewstate] a dynamic image map&#39;s hot spot clicked（期待される本文）</summary>

```text
Plain table
Name bound
First bound
set in code
Foot
FileUpload:
First view:
Localize: localized <b>encoded</b> bound set in code
Substitution: substituted POST
Pen
Bag & box
AdRotator:
< February 2020 >
Month Sun Mon Tue Wed Thu Fri Sat
Week 26 27 28 29 30 31 1
Week 2 3 4 5 6 7 8
Week 9 10 11 12 13 14 15
Week 16 17 18 19 20 * 21 22
Week 23 24 25 26 27 28 29
Week 1 2 3 4 5 6 7
bound
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
ImageMap:
Dynamic:
Dynamic header
dynamic cell (set in code)
Dynamic foot
Dynamic view Twodynamic localizesubstituted POSTPenBag & box
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
dynamic literal
Request 12: IsPostBack=True
Click imapDynamic: dynamic
IsValid: (not validated)
State tblPlain Rows=4 [TableHeaderRow:Name,bound|TableRow:First,bound|TableRow:set in code,set while hidden|TableFooterRow:Foot]
State thBound Text="bound"
State tdBound Text="bound"
State tdCode Text="set in code"
State tdHidden Text="set while hidden" (hidden)
State tblCode Rows=0 []
State tblHidden Rows=0 [] (hidden)
State fupPlain HasFile=False FileName="" Files=1
State fupMulti HasFile=False FileName="" Files=1
State fupDisabled HasFile=False FileName="" Files=0
State fupHidden HasFile=False FileName="" Files=0 (hidden)
State mvSteps ActiveViewIndex=0 Active=vwFirst
State txtInFirst Text="in the first view"
State lblInSecond Text="bound" (hidden)
State mvHidden ActiveViewIndex=0 Active=vwInHidden (hidden)
State locPlain Text="<b>localized</b>"
State locBound Text="bound"
State locCode Text="set in code"
State locHidden Text="set while hidden" (hidden)
State xmlPlain DocumentContent="" DocumentSource="~/App_Data/probe.xml"
State xmlBound DocumentContent="" DocumentSource=""
State xmlCode DocumentContent="" DocumentSource=""
State adPlain AdvertisementFile="~/App_Data/ads.xml"
State adBound AdvertisementFile=""
State adHidden AdvertisementFile="~/App_Data/ads.xml" (hidden)
State calPlain VisibleDate=2020-02-01 SelectedDate=2019-12-29 Selected=7
State calBound VisibleDate=2020-01-15 SelectedDate=0001-01-01 Selected=0
State calHidden VisibleDate=0001-01-01 SelectedDate=0001-01-01 Selected=0 (hidden)
State imapPlain HotSpots=2 AlternateText="Areas"
State imapBound HotSpots=0 AlternateText="bound"
State imapHidden HotSpots=0 AlternateText="" (hidden)
State tblDynamic Rows=3 [TableHeaderRow:Dynamic header|TableRow:dynamic cell (set in code)|TableFooterRow:Dynamic foot]
State fupDynamic HasFile=False FileName="" Files=1
State mvDynamic ActiveViewIndex=1 Active=vwDynamicTwo
State locDynamic Text="dynamic localize"
State subDynamic
State xmlDynamic DocumentContent="" DocumentSource="~/App_Data/probe.xml"
State adDynamic AdvertisementFile="~/App_Data/ads.xml"
State calDynamic VisibleDate=2020-01-15 SelectedDate=2020-01-21 Selected=1
State imapDynamic HotSpots=1 AlternateText=""
State litDynamic Text="dynamic literal"
```

</details>

<details><summary>TableFooterRow [visible][viewstate] hidden ones shown with what was set while hidden（期待される本文）</summary>

```text
Plain table
Name bound
First bound
set in code set while hidden
Foot
FileUpload:
First view: In a hidden multi-view
Localize: localized <b>encoded</b> bound set in code set while hidden
Substitution: substituted POST
Pen
Bag & box
AdRotator:
< February 2020 >
Month Sun Mon Tue Wed Thu Fri Sat
Week 26 27 28 29 30 31 1
Week 2 3 4 5 6 7 8
Week 9 10 11 12 13 14 15
Week 16 17 18 19 20 * 21 22
Week 23 24 25 26 27 28 29
Week 1 2 3 4 5 6 7
bound
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
ImageMap:
Dynamic:
Dynamic header
dynamic cell (set in code)
Dynamic foot
Dynamic view Twodynamic localizesubstituted POSTPenBag & box
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
dynamic literal
Request 13: IsPostBack=True
Click btnShow
IsValid: (not validated)
State tblPlain Rows=4 [TableHeaderRow:Name,bound|TableRow:First,bound|TableRow:set in code,set while hidden|TableFooterRow:Foot]
State thBound Text="bound"
State tdBound Text="bound"
State tdCode Text="set in code"
State tdHidden Text="set while hidden"
State tblCode Rows=0 []
State tblHidden Rows=0 []
State fupPlain HasFile=False FileName="" Files=1
State fupMulti HasFile=False FileName="" Files=1
State fupDisabled HasFile=False FileName="" Files=0
State fupHidden HasFile=False FileName="" Files=0
State mvSteps ActiveViewIndex=0 Active=vwFirst
State txtInFirst Text="in the first view"
State lblInSecond Text="bound" (hidden)
State mvHidden ActiveViewIndex=0 Active=vwInHidden
State locPlain Text="<b>localized</b>"
State locBound Text="bound"
State locCode Text="set in code"
State locHidden Text="set while hidden"
State xmlPlain DocumentContent="" DocumentSource="~/App_Data/probe.xml"
State xmlBound DocumentContent="" DocumentSource=""
State xmlCode DocumentContent="" DocumentSource=""
State adPlain AdvertisementFile="~/App_Data/ads.xml"
State adBound AdvertisementFile=""
State adHidden AdvertisementFile="~/App_Data/ads.xml"
State calPlain VisibleDate=2020-02-01 SelectedDate=2019-12-29 Selected=7
State calBound VisibleDate=2020-01-15 SelectedDate=0001-01-01 Selected=0
State calHidden VisibleDate=0001-01-01 SelectedDate=0001-01-01 Selected=0
State imapPlain HotSpots=2 AlternateText="Areas"
State imapBound HotSpots=0 AlternateText="bound"
State imapHidden HotSpots=0 AlternateText=""
State tblDynamic Rows=3 [TableHeaderRow:Dynamic header|TableRow:dynamic cell (set in code)|TableFooterRow:Dynamic foot]
State fupDynamic HasFile=False FileName="" Files=1
State mvDynamic ActiveViewIndex=1 Active=vwDynamicTwo
State locDynamic Text="dynamic localize"
State subDynamic
State xmlDynamic DocumentContent="" DocumentSource="~/App_Data/probe.xml"
State adDynamic AdvertisementFile="~/App_Data/ads.xml"
State calDynamic VisibleDate=2020-01-15 SelectedDate=2020-01-21 Selected=1
State imapDynamic HotSpots=1 AlternateText=""
State litDynamic Text="dynamic literal"
```

</details>

<details><summary>TableFooterRow [viewstate] a file upload validated on the server (g1): no file posted（期待される本文）</summary>

```text
Plain table
Name bound
First bound
set in code set while hidden
Foot
FileUpload: A file is required
First view: In a hidden multi-view
Localize: localized <b>encoded</b> bound set in code set while hidden
Substitution: substituted POST
Pen
Bag & box
AdRotator:
< February 2020 >
Month Sun Mon Tue Wed Thu Fri Sat
Week 26 27 28 29 30 31 1
Week 2 3 4 5 6 7 8
Week 9 10 11 12 13 14 15
Week 16 17 18 19 20 * 21 22
Week 23 24 25 26 27 28 29
Week 1 2 3 4 5 6 7
bound
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
ImageMap:
Dynamic:
Dynamic header
dynamic cell (set in code)
Dynamic foot
Dynamic view Twodynamic localizesubstituted POSTPenBag & box
< January 2020 >
Sun Mon Tue Wed Thu Fri Sat
29 30 31 1 2 3 4
5 6 7 8 9 10 11
12 13 14 15 16 17 18
19 20 21 22 23 24 25
26 27 28 29 30 31 1
2 3 4 5 6 7 8
dynamic literal
Request 14: IsPostBack=True
IsValid: False
State tblPlain Rows=4 [TableHeaderRow:Name,bound|TableRow:First,bound|TableRow:set in code,set while hidden|TableFooterRow:Foot]
State thBound Text="bound"
State tdBound Text="bound"
State tdCode Text="set in code"
State tdHidden Text="set while hidden"
State tblCode Rows=0 []
State tblHidden Rows=0 []
State fupPlain HasFile=False FileName="" Files=1
State fupMulti HasFile=False FileName="" Files=1
State fupDisabled HasFile=False FileName="" Files=0
State fupHidden HasFile=False FileName="" Files=1
State mvSteps ActiveViewIndex=0 Active=vwFirst
State txtInFirst Text="in the first view"
State lblInSecond Text="bound" (hidden)
State mvHidden ActiveViewIndex=0 Active=vwInHidden
State locPlain Text="<b>localized</b>"
State locBound Text="bound"
State locCode Text="set in code"
State locHidden Text="set while hidden"
State xmlPlain DocumentContent="" DocumentSource="~/App_Data/probe.xml"
State xmlBound DocumentContent="" DocumentSource=""
State xmlCode DocumentContent="" DocumentSource=""
State adPlain AdvertisementFile="~/App_Data/ads.xml"
State adBound AdvertisementFile=""
State adHidden AdvertisementFile="~/App_Data/ads.xml"
State calPlain VisibleDate=2020-02-01 SelectedDate=2019-12-29 Selected=7
State calBound VisibleDate=2020-01-15 SelectedDate=0001-01-01 Selected=0
State calHidden VisibleDate=0001-01-01 SelectedDate=0001-01-01 Selected=0
State imapPlain HotSpots=2 AlternateText="Areas"
State imapBound HotSpots=0 AlternateText="bound"
State imapHidden HotSpots=0 AlternateText=""
State tblDynamic Rows=3 [TableHeaderRow:Dynamic header|TableRow:dynamic cell (set in code)|TableFooterRow:Dynamic foot]
State fupDynamic HasFile=False FileName="" Files=1
State mvDynamic ActiveViewIndex=1 Active=vwDynamicTwo
State locDynamic Text="dynamic localize"
State subDynamic
State xmlDynamic DocumentContent="" DocumentSource="~/App_Data/probe.xml"
State adDynamic AdvertisementFile="~/App_Data/ads.xml"
State calDynamic VisibleDate=2020-01-15 SelectedDate=2020-01-21 Selected=1
State imapDynamic HotSpots=1 AlternateText=""
State litDynamic Text="dynamic literal"
```

</details>

<details><summary>TableFooterRow [viewstate] a postback event to a table (not registered for event validation): rejected（期待される本文）</summary>

```text
System.ArgumentException
Invalid postback or callback argument. Event validation is enabled using <pages enableEventValidation="true"/> in configuration or <%@ Page EnableEventValidation="true" %> in a page. For security purposes, this feature verifies that arguments to postback or callback events originate from the server control that originally rendered them. If the data is valid and expected, use the ClientScriptManager.RegisterForEventValidation method in order to register the postback or callback data for validation.
```

</details>

