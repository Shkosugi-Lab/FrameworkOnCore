# Label テストケース一覧

変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。
このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。

- 正解データの採取: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0
- ① API 層: 154 ケース（163 メンバー）
- ② HTML 層: 131 ケース
- ③ シナリオ層: 3 スナップショット

## 機構マトリクス

型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。

| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |
|---|---|---|---|
| プロパティ → HTML 描画 | すべてのコントロール | 119 | 1 |
| ViewState の保存と復元 | Control | 6 | 2 |
| Visible=false（描画されず状態は保持） | Control.Visible | 2 | 1 |
| 名前付けコンテナと ClientIDMode | Control.ClientIDMode | 7 | 0 |
| テーマとスキン（SkinID / EnableTheming） | テーマを付けられる型（[Themeable]） | 4 | 0 |
| 動的に生成したコントロールのライフサイクル | Control | 0 | 2 |
| データバインド（DataBind / <%# %>） | Control.DataBind | 0 | 1 |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | WebControl.Enabled、HtmlControl.Disabled | 4 | 0 |

該当しない機構: ポストバックデータの取り込み、変更イベント（*Changed、ServerChange）、ReadOnly（ポストされた値を無視）、リクエスト検証（ValidateRequestMode）、ポストバックイベント、コマンドイベントと親へのバブル（CommandName）、送信ボタン／スクリプトによるポストバック（UseSubmitBehavior）、ブラウザ側のクリック処理（OnClientClick）、クロスページポストバック（PostBackUrl）、既定のボタン（Panel の DefaultButton）、AutoPostBack（クライアントスクリプトによるポストバック）、CausesValidation / ValidationGroup、検証対象（サーバー側の検証）、検証対象（ブラウザ側の検証）、双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値）、項目（ListItem の Selected・Enabled・Text・Value と ViewState）、複数選択、繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）、ポスト値のイベント検証（一覧にない値・無効な項目の拒否）、サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）、ブラウザ側の検証スクリプト（EnableClientScript）、検証結果のまとめ（ValidationSummary）、ページング、並べ替え、項目の挿入、行・項目の編集（編集・更新・取り消し・削除）、行・項目の選択、行・項目のコマンド（RowCommand・ItemCommand へのバブル）、データキー（DataKeyNames・DataKeys）、データソースコントロールとの連携（DataSourceID）、テンプレート、データソース連携（DataSource・DataTextField など）、クライアントコールバック

## ① API 層

Label とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の Label インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。

| ケース | 期待値（.NET Framework） |
|---|---|
| <code>Api.Control.#ctor</code> | <code>Control.#ctor = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMode=In…</code> |
| <code>Api.Control.Adapter</code> | <code>Control.Adapter = null</code> |
| <code>Api.Control.AddParsedSubObject</code> | <code>Control.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>Control.AddParsedSubObject(Object) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #0000…</code> |
| <code>Api.Control.AddedControl</code> | <code>Control.AddedControl(Control,Int32) = &quot;ok&quot;</code><br><code>Control.AddedControl(Control,Int32) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #000…</code> |
| <code>Api.Control.AppRelativeTemplateSourceDirectory</code> | <code>Control.AppRelativeTemplateSourceDirectory = &quot;&quot;</code><br><code>Control.AppRelativeTemplateSourceDirectory set = &quot;-&gt; System.ArgumentException: The relative virtual path &#39;Abc 123&#39; is not allowed here.&quot;</code> |
| <code>Api.Control.ApplyStyleSheetSkin</code> | <code>Control.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>Control.ApplyStyleSheetSkin(Page) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000…</code> |
| <code>Api.Control.BeginRenderTracing</code> | <code>Control.BeginRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.BeginRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.BeginRenderTracing(TextWriter,Object) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=…</code> |
| <code>Api.Control.BindingContainer</code> | <code>Control.BindingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Clien…</code> |
| <code>Api.Control.BuildProfileTree</code> | <code>Control.BuildProfileTree(String,Boolean) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.ChildControlsCreated</code> | <code>Control.ChildControlsCreated = &quot;false&quot;</code><br><code>Control.ChildControlsCreated set = &quot;ok&quot;</code> |
| <code>Api.Control.ClearCachedClientID</code> | <code>Control.ClearCachedClientID = &quot;ok&quot;</code><br><code>Control.ClearCachedClientID after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Co…</code> |
| <code>Api.Control.ClearChildControlState</code> | <code>Control.ClearChildControlState = &quot;ok&quot;</code><br><code>Control.ClearChildControlState after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000…</code> |
| <code>Api.Control.ClearChildState</code> | <code>Control.ClearChildState = &quot;ok&quot;</code><br><code>Control.ClearChildState after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color …</code> |
| <code>Api.Control.ClearChildViewState</code> | <code>Control.ClearChildViewState = &quot;ok&quot;</code><br><code>Control.ClearChildViewState after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Co…</code> |
| <code>Api.Control.ClearEffectiveClientIDMode</code> | <code>Control.ClearEffectiveClientIDMode = &quot;ok&quot;</code><br><code>Control.ClearEffectiveClientIDMode after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #0000…</code> |
| <code>Api.Control.ClientID</code> | <code>Control.ClientID = &quot;lblName&quot;</code> |
| <code>Api.Control.ClientIDMode</code> | <code>Control.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>Control.ClientIDMode set = &quot;ClientIDMode.AutoID&quot;</code> |
| <code>Api.Control.ClientIDSeparator</code> | <code>Control.ClientIDSeparator = &quot;&#39;_&#39;&quot;</code> |
| <code>Api.Control.Context</code> | <code>Control.Context = null</code> |
| <code>Api.Control.Controls</code> | <code>Control.Controls = ControlCollection []</code> |
| <code>Api.Control.CreateChildControls</code> | <code>Control.CreateChildControls = &quot;ok&quot;</code><br><code>Control.CreateChildControls after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Co…</code> |
| <code>Api.Control.CreateControlCollection</code> | <code>Control.CreateControlCollection = ControlCollection []</code><br><code>Control.CreateControlCollection after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #0000000…</code> |
| <code>Api.Control.DataBind</code> | <code>Control.DataBind = &quot;ok&quot;</code><br><code>Control.DataBind after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]…</code><br><code>Control.DataBind(Boolean) = &quot;ok&quot;</code><br><code>Control.DataBind(Boolean) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Colo…</code> |
| <code>Api.Control.DataBindChildren</code> | <code>Control.DataBindChildren = &quot;ok&quot;</code><br><code>Control.DataBindChildren after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color…</code> |
| <code>Api.Control.DataBinding</code> | <code>Control.DataBinding = &quot;added and removed&quot;</code> |
| <code>Api.Control.DataItemContainer</code> | <code>Control.DataItemContainer = null</code> |
| <code>Api.Control.DataKeysContainer</code> | <code>Control.DataKeysContainer = null</code> |
| <code>Api.Control.DesignMode</code> | <code>Control.DesignMode = &quot;false&quot;</code> |
| <code>Api.Control.Dispose</code> | <code>Control.Dispose = &quot;ok&quot;</code><br><code>Control.Dispose after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty];…</code> |
| <code>Api.Control.Disposed</code> | <code>Control.Disposed = &quot;added and removed&quot;</code> |
| <code>Api.Control.EnableTheming</code> | <code>Control.EnableTheming = &quot;true&quot;</code><br><code>Control.EnableTheming set = &quot;true&quot;</code> |
| <code>Api.Control.EnableViewState</code> | <code>Control.EnableViewState = &quot;true&quot;</code><br><code>Control.EnableViewState set = &quot;true&quot;</code> |
| <code>Api.Control.EndRenderTracing</code> | <code>Control.EndRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.EndRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.EndRenderTracing(TextWriter,Object) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Co…</code> |
| <code>Api.Control.EnsureChildControls</code> | <code>Control.EnsureChildControls = &quot;ok&quot;</code><br><code>Control.EnsureChildControls after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Co…</code> |
| <code>Api.Control.EnsureID</code> | <code>Control.EnsureID = &quot;ok&quot;</code><br><code>Control.EnsureID after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]…</code> |
| <code>Api.Control.Events</code> | <code>Control.Events = &quot;System.ComponentModel.EventHandlerList&quot;</code> |
| <code>Api.Control.FindControl</code> | <code>Control.FindControl(String) = Label#lblTitle {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code><br><code>Control.FindControl(String) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Co…</code><br><code>Control.FindControl(String,Int32) = null</code><br><code>Control.FindControl(String,Int32) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000…</code> |
| <code>Api.Control.Focus</code> | <code>Control.Focus = &quot;-&gt; System.InvalidOperationException: A form tag with runat=server must exist on the Page to use SetFocus() or the Focus property.&quot;</code> |
| <code>Api.Control.GetDesignModeState</code> | <code>Control.GetDesignModeState = &quot;[]&quot;</code><br><code>Control.GetDesignModeState after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Col…</code> |
| <code>Api.Control.GetRouteUrl</code> | <code>Control.GetRouteUrl(Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.GetUniqueIDRelativeTo</code> | <code>Control.GetUniqueIDRelativeTo(Control) = &quot;lblName&quot;</code><br><code>Control.GetUniqueIDRelativeTo(Control) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #…</code> |
| <code>Api.Control.HasChildViewState</code> | <code>Control.HasChildViewState = &quot;false&quot;</code> |
| <code>Api.Control.HasControls</code> | <code>Control.HasControls = &quot;false&quot;</code><br><code>Control.HasControls after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Emp…</code> |
| <code>Api.Control.HasEvents</code> | <code>Control.HasEvents = &quot;false&quot;</code><br><code>Control.HasEvents after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty…</code> |
| <code>Api.Control.ID</code> | <code>Control.ID = &quot;lblName&quot;</code><br><code>Control.ID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.IdSeparator</code> | <code>Control.IdSeparator = &quot;&#39;$&#39;&quot;</code> |
| <code>Api.Control.Init</code> | <code>Control.Init = &quot;added and removed&quot;</code> |
| <code>Api.Control.IsChildControlStateCleared</code> | <code>Control.IsChildControlStateCleared = &quot;false&quot;</code> |
| <code>Api.Control.IsLiteralContent</code> | <code>Control.IsLiteralContent = &quot;false&quot;</code><br><code>Control.IsLiteralContent after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color…</code> |
| <code>Api.Control.IsTrackingViewState</code> | <code>Control.IsTrackingViewState = &quot;false&quot;</code> |
| <code>Api.Control.IsViewStateEnabled</code> | <code>Control.IsViewStateEnabled = &quot;true&quot;</code> |
| <code>Api.Control.Load</code> | <code>Control.Load = &quot;added and removed&quot;</code> |
| <code>Api.Control.LoadControlState</code> | <code>Control.LoadControlState(Object) = &quot;ok&quot;</code><br><code>Control.LoadControlState(Object) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #000000…</code> |
| <code>Api.Control.LoadViewState</code> | <code>Control.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Web.UI.Pair&#39;.&quot;</code> |
| <code>Api.Control.LoadViewStateByID</code> | <code>Control.LoadViewStateByID = &quot;false&quot;</code> |
| <code>Api.Control.MapPathSecure</code> | <code>Control.MapPathSecure(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.NamingContainer</code> | <code>Control.NamingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code> |
| <code>Api.Control.OnBubbleEvent</code> | <code>Control.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Control.OnBubbleEvent(Object,EventArgs) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color …</code> |
| <code>Api.Control.OnDataBinding</code> | <code>Control.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnDataBinding(EventArgs) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #000000…</code> |
| <code>Api.Control.OnInit</code> | <code>Control.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnInit(EventArgs) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Colo…</code> |
| <code>Api.Control.OnLoad</code> | <code>Control.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnLoad(EventArgs) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Colo…</code> |
| <code>Api.Control.OnPreRender</code> | <code>Control.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnPreRender(EventArgs) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000…</code> |
| <code>Api.Control.OnUnload</code> | <code>Control.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnUnload(EventArgs) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Co…</code> |
| <code>Api.Control.OpenFile</code> | <code>Control.OpenFile(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.Page</code> | <code>Control.Page = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=&quot;&quot;; …</code><br><code>Control.Page set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=…</code> |
| <code>Api.Control.Parent</code> | <code>Control.Parent = HtmlForm#form1 {Action=&quot;&quot;; ClientID=&quot;form1&quot;; ClientIDMode=Inherit; DefaultButton=&quot;&quot;; DefaultFocus=&quot;&quot;; Disabled=false; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.PreRender</code> | <code>Control.PreRender = &quot;added and removed&quot;</code> |
| <code>Api.Control.RaiseBubbleEvent</code> | <code>Control.RaiseBubbleEvent(Object,EventArgs) = &quot;ok&quot;</code><br><code>Control.RaiseBubbleEvent(Object,EventArgs) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Col…</code> |
| <code>Api.Control.RemovedControl</code> | <code>Control.RemovedControl(Control) = &quot;ok&quot;</code><br><code>Control.RemovedControl(Control) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #0000000…</code> |
| <code>Api.Control.Render</code> | <code>Control.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.Render(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;span id=&quot;lblName&quot;&gt;Name&lt;/span&gt;</code><br><code>Control.Render(HtmlTextWriter) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000…</code> |
| <code>Api.Control.RenderChildren</code> | <code>Control.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderChildren(HtmlTextWriter) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #…</code> |
| <code>Api.Control.RenderControl</code> | <code>Control.RenderControl(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;span id=&quot;lblName&quot;&gt;Name&lt;/span&gt;</code><br><code>Control.RenderControl(HtmlTextWriter) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #0…</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) writer:</code><br><code>  &#124; &lt;span id=&quot;lblName&quot;&gt;Name&lt;/span&gt;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empt…</code> |
| <code>Api.Control.RenderingCompatibility</code> | <code>Control.RenderingCompatibility = &quot;System.Version 4.0&quot;</code><br><code>Control.RenderingCompatibility set = &quot;System.Version 4.0&quot;</code> |
| <code>Api.Control.ResolveAdapter</code> | <code>Control.ResolveAdapter = null</code><br><code>Control.ResolveAdapter after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code> |
| <code>Api.Control.ResolveClientUrl</code> | <code>Control.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveClientUrl(String) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #000000…</code> |
| <code>Api.Control.ResolveUrl</code> | <code>Control.ResolveUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveUrl(String) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Col…</code> |
| <code>Api.Control.SaveControlState</code> | <code>Control.SaveControlState = null</code><br><code>Control.SaveControlState after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color…</code> |
| <code>Api.Control.SaveViewState</code> | <code>Control.SaveViewState = null</code><br><code>Control.SaveViewState after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [E…</code> |
| <code>Api.Control.SetDesignModeState</code> | <code>Control.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>Control.SetDesignModeState(IDictionary) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color …</code> |
| <code>Api.Control.SetRenderMethodDelegate</code> | <code>Control.SetRenderMethodDelegate(RenderMethod) = &quot;ok&quot;</code><br><code>Control.SetRenderMethodDelegate(RenderMethod) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=…</code> |
| <code>Api.Control.SetTraceData</code> | <code>Control.SetTraceData(Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #000…</code><br><code>Control.SetTraceData(Object,Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object,Object) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Col…</code> |
| <code>Api.Control.Site</code> | <code>Control.Site = null</code><br><code>Control.Site set = null</code> |
| <code>Api.Control.SkinID</code> | <code>Control.SkinID = &quot;&quot;</code><br><code>Control.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.TemplateControl</code> | <code>Control.TemplateControl = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code><br><code>Control.TemplateControl set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Cl…</code> |
| <code>Api.Control.TemplateSourceDirectory</code> | <code>Control.TemplateSourceDirectory = &quot;&quot;</code> |
| <code>Api.Control.TrackViewState</code> | <code>Control.TrackViewState = &quot;ok&quot;</code><br><code>Control.TrackViewState after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code> |
| <code>Api.Control.UniqueID</code> | <code>Control.UniqueID = &quot;lblName&quot;</code> |
| <code>Api.Control.Unload</code> | <code>Control.Unload = &quot;added and removed&quot;</code> |
| <code>Api.Control.ValidateRequestMode</code> | <code>Control.ValidateRequestMode = &quot;ValidateRequestMode.Inherit&quot;</code><br><code>Control.ValidateRequestMode set = &quot;ValidateRequestMode.Disabled&quot;</code> |
| <code>Api.Control.ViewState</code> | <code>Control.ViewState = StateBag [Text=&quot;Name&quot;]</code> |
| <code>Api.Control.ViewStateIgnoresCase</code> | <code>Control.ViewStateIgnoresCase = &quot;false&quot;</code> |
| <code>Api.Control.ViewStateMode</code> | <code>Control.ViewStateMode = &quot;ViewStateMode.Inherit&quot;</code><br><code>Control.ViewStateMode set = &quot;ViewStateMode.Enabled&quot;</code> |
| <code>Api.Control.Visible</code> | <code>Control.Visible = &quot;true&quot;</code><br><code>Control.Visible set = &quot;true&quot;</code> |
| <code>Api.IAttributeAccessor.GetAttribute</code> | <code>IAttributeAccessor.GetAttribute(String) = null</code><br><code>IAttributeAccessor.GetAttribute(String) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color …</code> |
| <code>Api.IAttributeAccessor.SetAttribute</code> | <code>IAttributeAccessor.SetAttribute(String,String) = &quot;ok&quot;</code><br><code>IAttributeAccessor.SetAttribute(String,String) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor…</code> |
| <code>Api.IControlBuilderAccessor.ControlBuilder</code> | <code>IControlBuilderAccessor.ControlBuilder = null</code> |
| <code>Api.IControlDesignerAccessor.GetDesignModeState</code> | <code>IControlDesignerAccessor.GetDesignModeState = &quot;[]&quot;</code><br><code>IControlDesignerAccessor.GetDesignModeState after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Co…</code> |
| <code>Api.IControlDesignerAccessor.SetDesignModeState</code> | <code>IControlDesignerAccessor.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetDesignModeState(IDictionary) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; B…</code> |
| <code>Api.IControlDesignerAccessor.SetOwnerControl</code> | <code>IControlDesignerAccessor.SetOwnerControl(Control) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetOwnerControl(Control) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderCo…</code> |
| <code>Api.IControlDesignerAccessor.UserData</code> | <code>IControlDesignerAccessor.UserData = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.DataBindings</code> | <code>IDataBindingsAccessor.DataBindings = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.HasDataBindings</code> | <code>IDataBindingsAccessor.HasDataBindings = &quot;false&quot;</code> |
| <code>Api.IExpressionsAccessor.Expressions</code> | <code>IExpressionsAccessor.Expressions = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.HasExpressions</code> | <code>IExpressionsAccessor.HasExpressions = &quot;false&quot;</code> |
| <code>Api.IParserAccessor.AddParsedSubObject</code> | <code>IParserAccessor.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>IParserAccessor.AddParsedSubObject(Object) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Col…</code> |
| <code>Api.ITextControl.Text</code> | <code>ITextControl.Text = &quot;Name&quot;</code><br><code>ITextControl.Text set = &quot;Abc 123&quot;</code> |
| <code>Api.IUrlResolutionService.ResolveClientUrl</code> | <code>IUrlResolutionService.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>IUrlResolutionService.ResolveClientUrl(String) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor…</code> |
| <code>Api.WebControls.Label.#ctor</code> | <code>Label.#ctor = Label# {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotS…</code> |
| <code>Api.WebControls.Label.AddAttributesToRender</code> | <code>Label.AddAttributesToRender(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Label.AddAttributesToRender(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Label.AddAttributesToRender(HtmlTextWriter) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Co…</code> |
| <code>Api.WebControls.Label.AddParsedSubObject</code> | <code>Label.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>Label.AddParsedSubObject(Object) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #000000…</code> |
| <code>Api.WebControls.Label.AssociatedControlID</code> | <code>Label.AssociatedControlID = &quot;&quot;</code><br><code>Label.AssociatedControlID set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.Label.LoadViewState</code> | <code>Label.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Web.UI.Pair&#39;.&quot;</code> |
| <code>Api.WebControls.Label.RenderContents</code> | <code>Label.RenderContents(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Label.RenderContents(HtmlTextWriter) writer:</code><br><code>  &#124; Name</code><br><code>Label.RenderContents(HtmlTextWriter) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00…</code> |
| <code>Api.WebControls.Label.SupportsDisabledAttribute</code> | <code>Label.SupportsDisabledAttribute = &quot;false&quot;</code> |
| <code>Api.WebControls.Label.TagKey</code> | <code>Label.TagKey = &quot;HtmlTextWriterTag.Span&quot;</code> |
| <code>Api.WebControls.Label.Text</code> | <code>Label.Text = &quot;Name&quot;</code><br><code>Label.Text set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.#ctor</code> | <code>WebControl.#ctor = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidt…</code><br><code>WebControl.#ctor(String) = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; Bo…</code><br><code>WebControl.#ctor(HtmlTextWriterTag) = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle…</code> |
| <code>Api.WebControls.WebControl.AccessKey</code> | <code>WebControl.AccessKey = &quot;&quot;</code><br><code>WebControl.AccessKey set = &quot;-&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]&quot;</code> |
| <code>Api.WebControls.WebControl.AddAttributesToRender</code> | <code>WebControl.AddAttributesToRender(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.AddAttributesToRender(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>WebControl.AddAttributesToRender(HtmlTextWriter) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderCol…</code> |
| <code>Api.WebControls.WebControl.ApplyStyle</code> | <code>WebControl.ApplyStyle(Style) = &quot;ok&quot;</code><br><code>WebControl.ApplyStyle(Style) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 C…</code> |
| <code>Api.WebControls.WebControl.Attributes</code> | <code>WebControl.Attributes = AttributeCollection []</code> |
| <code>Api.WebControls.WebControl.BackColor</code> | <code>WebControl.BackColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>WebControl.BackColor set = &quot;Color #FF3366CC Color [A=255, R=51, G=102, B=204]&quot;</code> |
| <code>Api.WebControls.WebControl.BorderColor</code> | <code>WebControl.BorderColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>WebControl.BorderColor set = &quot;Color #FF3366CC Color [A=255, R=51, G=102, B=204]&quot;</code> |
| <code>Api.WebControls.WebControl.BorderStyle</code> | <code>WebControl.BorderStyle = &quot;BorderStyle.NotSet&quot;</code><br><code>WebControl.BorderStyle set = &quot;BorderStyle.None&quot;</code> |
| <code>Api.WebControls.WebControl.BorderWidth</code> | <code>WebControl.BorderWidth = Unit </code><br><code>WebControl.BorderWidth set = Unit 120px</code> |
| <code>Api.WebControls.WebControl.ControlStyle</code> | <code>WebControl.ControlStyle = Style {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; CssClass=…</code> |
| <code>Api.WebControls.WebControl.ControlStyleCreated</code> | <code>WebControl.ControlStyleCreated = &quot;false&quot;</code> |
| <code>Api.WebControls.WebControl.CopyBaseAttributes</code> | <code>WebControl.CopyBaseAttributes(WebControl) = &quot;ok&quot;</code><br><code>WebControl.CopyBaseAttributes(WebControl) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Colo…</code> |
| <code>Api.WebControls.WebControl.CreateControlStyle</code> | <code>WebControl.CreateControlStyle = Style {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; Css…</code><br><code>WebControl.CreateControlStyle after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 …</code> |
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
| <code>Api.WebControls.WebControl.MergeStyle</code> | <code>WebControl.MergeStyle(Style) = &quot;ok&quot;</code><br><code>WebControl.MergeStyle(Style) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 C…</code> |
| <code>Api.WebControls.WebControl.Render</code> | <code>WebControl.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.Render(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;span id=&quot;lblName&quot;&gt;Name&lt;/span&gt;</code><br><code>WebControl.Render(HtmlTextWriter) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000…</code> |
| <code>Api.WebControls.WebControl.RenderBeginTag</code> | <code>WebControl.RenderBeginTag(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.RenderBeginTag(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;span id=&quot;lblName&quot;&gt;</code><br><code>WebControl.RenderBeginTag(HtmlTextWriter) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Colo…</code> |
| <code>Api.WebControls.WebControl.RenderContents</code> | <code>WebControl.RenderContents(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.RenderContents(HtmlTextWriter) writer:</code><br><code>  &#124; Name</code><br><code>WebControl.RenderContents(HtmlTextWriter) after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Colo…</code> |
| <code>Api.WebControls.WebControl.RenderEndTag</code> | <code>WebControl.RenderEndTag(HtmlTextWriter) = &quot;-&gt; System.InvalidOperationException: A PopEndTag was called without a corresponding PushEndTag.&quot;</code> |
| <code>Api.WebControls.WebControl.SaveViewState</code> | <code>WebControl.SaveViewState = null</code><br><code>WebControl.SaveViewState after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color…</code> |
| <code>Api.WebControls.WebControl.SkinID</code> | <code>WebControl.SkinID = &quot;&quot;</code><br><code>WebControl.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.Style</code> | <code>WebControl.Style = CssStyleCollection {Count=Int32 0; Value=null}</code> |
| <code>Api.WebControls.WebControl.SupportsDisabledAttribute</code> | <code>WebControl.SupportsDisabledAttribute = &quot;false&quot;</code> |
| <code>Api.WebControls.WebControl.TabIndex</code> | <code>WebControl.TabIndex = &quot;Int16 0&quot;</code><br><code>WebControl.TabIndex set = &quot;Int16 1&quot;</code> |
| <code>Api.WebControls.WebControl.TagKey</code> | <code>WebControl.TagKey = &quot;HtmlTextWriterTag.Span&quot;</code> |
| <code>Api.WebControls.WebControl.TagName</code> | <code>WebControl.TagName = &quot;span&quot;</code> |
| <code>Api.WebControls.WebControl.ToolTip</code> | <code>WebControl.ToolTip = &quot;&quot;</code><br><code>WebControl.ToolTip set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.TrackViewState</code> | <code>WebControl.TrackViewState = &quot;ok&quot;</code><br><code>WebControl.TrackViewState after = Label#lblName {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Colo…</code> |
| <code>Api.WebControls.WebControl.Width</code> | <code>WebControl.Width = Unit </code><br><code>WebControl.Width set = Unit 120px</code> |

## ② HTML 層

プローブサイトの RenderProbe.aspx で、1 ケース 1 リクエスト（ページライフサイクル全体）で描画したページ全体を比較します。下表はコンポーネント部分（div#probe）、設定時の例外、保存された ViewState の要約です。

| ケース | 設定 | 期待される描画 | 設定時の例外 |
|---|---|---|---|
| <code>.(default)</code> | <code>(nothing set)</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.AccessKey=text</code> | <code>c.AccessKey = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> | <code>c.AccessKey = &quot;probe&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=markup</code> | <code>c.AccessKey = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> | <code>c.AccessKey = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=japanese</code> | <code>c.AccessKey = &quot;日本語テキスト&quot;;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> | <code>c.AccessKey = &quot;日本語テキスト&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=empty</code> | <code>c.AccessKey = &quot;&quot;;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.AccessKey=k</code> | <code>c.AccessKey = &quot;k&quot;;</code> | <code>&lt;span id=&quot;lblName&quot; accesskey=&quot;k&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.AppRelativeTemplateSourceDirectory=text</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;probe&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=markup</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=japanese</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;日本語テキスト&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=empty</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&quot;;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.AssociatedControlID=text</code> | <code>c.AssociatedControlID = &quot;probe&quot;;</code> | <code>status = 500</code> |  |
| <code>.AssociatedControlID=markup</code> | <code>c.AssociatedControlID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>status = 500</code> |  |
| <code>.AssociatedControlID=japanese</code> | <code>c.AssociatedControlID = &quot;日本語テキスト&quot;;</code> | <code>status = 500</code> |  |
| <code>.AssociatedControlID=empty</code> | <code>c.AssociatedControlID = &quot;&quot;;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.AssociatedControlID=target</code> | <code>c.AssociatedControlID = &quot;txtTarget&quot;;</code> | <code>&lt;label for=&quot;txtTarget&quot; id=&quot;lblName&quot;&gt;&lt;/label&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.BackColor=Red</code> | <code>c.BackColor = Color.Red;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;background-color:Red;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.BackColor=#3366CC</code> | <code>c.BackColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;background-color:#3366CC;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.BackColor=Transparent</code> | <code>c.BackColor = Color.Transparent;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;background-color:Transparent;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.BorderColor=Red</code> | <code>c.BorderColor = Color.Red;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;border-color:Red;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.BorderColor=#3366CC</code> | <code>c.BorderColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;border-color:#3366CC;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.BorderColor=Transparent</code> | <code>c.BorderColor = Color.Transparent;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;border-color:Transparent;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.BorderStyle=NotSet</code> | <code>c.BorderStyle = BorderStyle.NotSet;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.BorderStyle=None</code> | <code>c.BorderStyle = BorderStyle.None;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;display:inline-block;border-style:None;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.BorderStyle=Dotted</code> | <code>c.BorderStyle = BorderStyle.Dotted;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;display:inline-block;border-style:Dotted;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.BorderStyle=Dashed</code> | <code>c.BorderStyle = BorderStyle.Dashed;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;display:inline-block;border-style:Dashed;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.BorderStyle=Solid</code> | <code>c.BorderStyle = BorderStyle.Solid;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;display:inline-block;border-style:Solid;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.BorderStyle=Double</code> | <code>c.BorderStyle = BorderStyle.Double;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;display:inline-block;border-style:Double;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.BorderStyle=Groove</code> | <code>c.BorderStyle = BorderStyle.Groove;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;display:inline-block;border-style:Groove;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.BorderStyle=Ridge</code> | <code>c.BorderStyle = BorderStyle.Ridge;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;display:inline-block;border-style:Ridge;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.BorderStyle=Inset</code> | <code>c.BorderStyle = BorderStyle.Inset;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;display:inline-block;border-style:Inset;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.BorderStyle=Outset</code> | <code>c.BorderStyle = BorderStyle.Outset;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;display:inline-block;border-style:Outset;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.BorderWidth=120px</code> | <code>c.BorderWidth = Unit.Pixel(120);</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;display:inline-block;border-width:120px;border-style:solid;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.BorderWidth=50%</code> | <code>c.BorderWidth = Unit.Percentage(50);</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> | <code>c.BorderWidth = Unit.Percentage(50); -&gt; System.ArgumentOutOfRangeException: BorderWidth must be a non-negative number and cannot be a percentage. [param value]</code> |
| <code>.BorderWidth=2em</code> | <code>c.BorderWidth = new Unit(2, UnitType.Em);</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;display:inline-block;border-width:2em;border-style:solid;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.ClientIDMode=Inherit</code> | <code>c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.ClientIDMode=AutoID</code> | <code>c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.ClientIDMode=Predictable</code> | <code>c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.ClientIDMode=Static</code> | <code>c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.CssClass=text</code> | <code>c.CssClass = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;lblName&quot; class=&quot;probe&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.CssClass=markup</code> | <code>c.CssClass = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;lblName&quot; class=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.CssClass=japanese</code> | <code>c.CssClass = &quot;日本語テキスト&quot;;</code> | <code>&lt;span id=&quot;lblName&quot; class=&quot;日本語テキスト&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.CssClass=empty</code> | <code>c.CssClass = &quot;&quot;;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.EnableTheming=true</code> | <code>c.EnableTheming = true;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.EnableTheming=false</code> | <code>c.EnableTheming = false;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.EnableViewState=true</code> | <code>c.EnableViewState = true;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.EnableViewState=false</code> | <code>c.EnableViewState = false;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Enabled=true</code> | <code>c.Enabled = true;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Enabled=false</code> | <code>c.Enabled = false;</code> | <code>&lt;span id=&quot;lblName&quot; class=&quot;aspNetDisabled&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Font.Bold=true</code> | <code>c.Font.Bold = true;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;font-weight:bold;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Font.Bold=false</code> | <code>c.Font.Bold = false;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;font-weight:normal;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Font.Italic=true</code> | <code>c.Font.Italic = true;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;font-style:italic;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Font.Italic=false</code> | <code>c.Font.Italic = false;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;font-style:normal;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Font.Name=text</code> | <code>c.Font.Name = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;font-family:probe;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Font.Name=markup</code> | <code>c.Font.Name = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;font-family:&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Font.Name=japanese</code> | <code>c.Font.Name = &quot;日本語テキスト&quot;;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;font-family:日本語テキスト;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Font.Name=empty</code> | <code>c.Font.Name = &quot;&quot;;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Font.Names=Arial,MS-Gothic</code> | <code>c.Font.Names = new[] { &quot;Arial&quot;, &quot;MS Gothic&quot; };</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;font-family:Arial,MS Gothic;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Font.Overline=true</code> | <code>c.Font.Overline = true;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;text-decoration: overline;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Font.Overline=false</code> | <code>c.Font.Overline = false;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;text-decoration:none;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Font.Size=12pt</code> | <code>c.Font.Size = FontUnit.Point(12);</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;font-size:12pt;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Font.Size=Large</code> | <code>c.Font.Size = FontUnit.Large;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;font-size:Large;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Font.Size=150%</code> | <code>c.Font.Size = new FontUnit(Unit.Percentage(150));</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;font-size:150%;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Font.Strikeout=true</code> | <code>c.Font.Strikeout = true;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;text-decoration: line-through;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Font.Strikeout=false</code> | <code>c.Font.Strikeout = false;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;text-decoration:none;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Font.Underline=true</code> | <code>c.Font.Underline = true;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;text-decoration:underline;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Font.Underline=false</code> | <code>c.Font.Underline = false;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;text-decoration:none;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.ForeColor=Red</code> | <code>c.ForeColor = Color.Red;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;color:Red;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.ForeColor=#3366CC</code> | <code>c.ForeColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;color:#3366CC;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.ForeColor=Transparent</code> | <code>c.ForeColor = Color.Transparent;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;color:Transparent;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Height=120px</code> | <code>c.Height = Unit.Pixel(120);</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;display:inline-block;height:120px;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Height=50%</code> | <code>c.Height = Unit.Percentage(50);</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;display:inline-block;height:50%;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Height=2em</code> | <code>c.Height = new Unit(2, UnitType.Em);</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;display:inline-block;height:2em;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.ID=text</code> | <code>c.ID = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;probe&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.ID=markup</code> | <code>c.ID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.ID=japanese</code> | <code>c.ID = &quot;日本語テキスト&quot;;</code> | <code>&lt;span id=&quot;日本語テキスト&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.ID=empty</code> | <code>c.ID = &quot;&quot;;</code> | <code>&lt;span id=&quot;ctl02&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.SkinID=text</code> | <code>c.SkinID = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.SkinID=markup</code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.SkinID=japanese</code> | <code>c.SkinID = &quot;日本語テキスト&quot;;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.SkinID=empty</code> | <code>c.SkinID = &quot;&quot;;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.TabIndex=0</code> | <code>c.TabIndex = 0;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.TabIndex=3</code> | <code>c.TabIndex = 3;</code> | <code>&lt;span id=&quot;lblName&quot; tabindex=&quot;3&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.TabIndex=-1</code> | <code>c.TabIndex = -1;</code> | <code>&lt;span id=&quot;lblName&quot; tabindex=&quot;-1&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Text=text</code> | <code>c.Text = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;probe&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Text=markup</code> | <code>c.Text = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Text=japanese</code> | <code>c.Text = &quot;日本語テキスト&quot;;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;日本語テキスト&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Text=empty</code> | <code>c.Text = &quot;&quot;;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.ToolTip=text</code> | <code>c.ToolTip = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;lblName&quot; title=&quot;probe&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.ToolTip=markup</code> | <code>c.ToolTip = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;lblName&quot; title=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.ToolTip=japanese</code> | <code>c.ToolTip = &quot;日本語テキスト&quot;;</code> | <code>&lt;span id=&quot;lblName&quot; title=&quot;日本語テキスト&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.ToolTip=empty</code> | <code>c.ToolTip = &quot;&quot;;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.ValidateRequestMode=Inherit</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Inherit;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.ValidateRequestMode=Disabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Disabled;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.ValidateRequestMode=Enabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Enabled;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.ViewStateMode=Inherit</code> | <code>c.ViewStateMode = ViewStateMode.Inherit;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.ViewStateMode=Enabled</code> | <code>c.ViewStateMode = ViewStateMode.Enabled;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.ViewStateMode=Disabled</code> | <code>c.ViewStateMode = ViewStateMode.Disabled;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Visible=true</code> | <code>c.Visible = true;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Visible=false</code> | <code>c.Visible = false;</code> | <code>&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Width=120px</code> | <code>c.Width = Unit.Pixel(120);</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;display:inline-block;width:120px;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Width=50%</code> | <code>c.Width = Unit.Percentage(50);</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;display:inline-block;width:50%;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Width=2em</code> | <code>c.Width = new Unit(2, UnitType.Em);</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;display:inline-block;width:2em;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Attributes[data-probe]=text</code> | <code>c.Attributes[data-probe] = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;lblName&quot; data-probe=&quot;probe&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Attributes[onfocus]=text</code> | <code>c.Attributes[onfocus] = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;lblName&quot; onfocus=&quot;probe&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Attributes[data-probe]=markup</code> | <code>c.Attributes[data-probe] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;lblName&quot; data-probe=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Attributes[onfocus]=markup</code> | <code>c.Attributes[onfocus] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;lblName&quot; onfocus=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Style[color]=red</code> | <code>c.Style[color] = &quot;red&quot;;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;color:red;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Style[margin-left]=red</code> | <code>c.Style[margin-left] = &quot;red&quot;;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;margin-left:red;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Style[color]=1em</code> | <code>c.Style[color] = &quot;1em&quot;;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;color:1em;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Style[margin-left]=1em</code> | <code>c.Style[margin-left] = &quot;1em&quot;;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;margin-left:1em;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.AssociatedControlID=target+Text=markup</code> | <code>c.AssociatedControlID = &quot;txtTarget&quot;; c.Text = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;label for=&quot;txtTarget&quot; id=&quot;lblName&quot;&gt;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&lt;/label&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.AssociatedControlID=target+Enabled=false</code> | <code>c.AssociatedControlID = &quot;txtTarget&quot;; c.Enabled = false;</code> | <code>&lt;label for=&quot;txtTarget&quot; id=&quot;lblName&quot; class=&quot;aspNetDisabled&quot;&gt;&lt;/label&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.AssociatedControlID=target+AccessKey=k</code> | <code>c.AssociatedControlID = &quot;txtTarget&quot;; c.AccessKey = &quot;k&quot;;</code> | <code>&lt;label for=&quot;txtTarget&quot; id=&quot;lblName&quot; accesskey=&quot;k&quot;&gt;&lt;/label&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Text=markup+Enabled=false</code> | <code>c.Text = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; c.Enabled = false;</code> | <code>&lt;span id=&quot;lblName&quot; class=&quot;aspNetDisabled&quot;&gt;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Enabled=false+CssClass=text</code> | <code>c.Enabled = false; c.CssClass = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;lblName&quot; class=&quot;aspNetDisabled probe&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Text=empty+ToolTip=text</code> | <code>c.Text = &quot;&quot;; c.ToolTip = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;lblName&quot; title=&quot;probe&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | <code>c.Width = Unit.Pixel(120); c.Height = new Unit(2, UnitType.Em); c.BorderWidth = Unit.Pixel(120); c.BorderStyle = BorderStyle.Solid; c.BackColor = Color.FromArgb(0x33, 0x66, 0xCC); c.ForeColor = Color.Red; c.Font.Bold = true;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;display:inline-block;color:Red;background-color:#3366CC;border-width:120px;border-style:Solid;font-weight:bold;height:2em;width:120px;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.ForeColor=#3366CC+Style[color]=red</code> | <code>c.ForeColor = Color.FromArgb(0x33, 0x66, 0xCC); c.Style[color] = &quot;red&quot;;</code> | <code>&lt;span id=&quot;lblName&quot; style=&quot;color:#3366CC;color:red;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Text=text+EnableViewState=false</code> | <code>c.Text = &quot;probe&quot;; c.EnableViewState = false;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;probe&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>.Text=text+Visible=false</code> | <code>c.Text = &quot;probe&quot;; c.Visible = false;</code> | <code>&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; /&gt;</code> |  |
| <code>[naming container].(default)</code> | <code>[naming container] (nothing set)</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;span id=&quot;box_lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;box$txtTarget&quot; type=&quot;text&quot; id=&quot;box_txtTarget&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=AutoID</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;span id=&quot;box_lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;box$txtTarget&quot; type=&quot;text&quot; id=&quot;box_txtTarget&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Static</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;box$txtTarget&quot; type=&quot;text&quot; id=&quot;box_txtTarget&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Predictable</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;span id=&quot;box_lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;box$txtTarget&quot; type=&quot;text&quot; id=&quot;box_txtTarget&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Inherit</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;span id=&quot;box_lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;box$txtTarget&quot; type=&quot;text&quot; id=&quot;box_txtTarget&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ID=text</code> | <code>[naming container] c.ID = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;span id=&quot;box_probe&quot;&gt;&lt;/span&gt;&lt;input name=&quot;box$txtTarget&quot; type=&quot;text&quot; id=&quot;box_txtTarget&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].AssociatedControlID=target</code> | <code>[naming container] c.AssociatedControlID = &quot;txtTarget&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;label for=&quot;box_txtTarget&quot; id=&quot;box_lblName&quot;&gt;&lt;/label&gt;&lt;input name=&quot;box$txtTarget&quot; type=&quot;text&quot; id=&quot;box_txtTarget&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[themed].(default)</code> | <code>[themed] (nothing set)</code> | <code>&lt;span id=&quot;lblName&quot; class=&quot;skinned&quot; style=&quot;background-color:#EEEEEE;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; class=&quot;skinned&quot; style=&quot;background-color:#EEEEEE;&quot; /&gt;</code> |  |
| <code>[themed].SkinID=text</code> | <code>[themed] c.SkinID = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;lblName&quot; title=&quot;from the skin&quot; class=&quot;skinned-probe&quot; style=&quot;display:inline-block;width:300px;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; class=&quot;skinned&quot; style=&quot;background-color:#EEEEEE;&quot; /&gt;</code> |  |
| <code>[themed].EnableTheming=false</code> | <code>[themed] c.EnableTheming = false;</code> | <code>&lt;span id=&quot;lblName&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; class=&quot;skinned&quot; style=&quot;background-color:#EEEEEE;&quot; /&gt;</code> |  |
| <code>[themed].CssClass=text</code> | <code>[themed] c.CssClass = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;lblName&quot; class=&quot;skinned&quot; style=&quot;background-color:#EEEEEE;&quot;&gt;&lt;/span&gt;&lt;input name=&quot;txtTarget&quot; type=&quot;text&quot; id=&quot;txtTarget&quot; class=&quot;skinned&quot; style=&quot;background-color:#EEEEEE;&quot; /&gt;</code> |  |

## ③ シナリオ層

ブラウザ（Playwright）で LabelScenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。

| # | 操作 | 対象 | 値 |
|---|---|---|---|
| 1 | goto | /BasicsScenario.aspx | <code></code> |
| 2 | snapshot |  | **Label [render][databind][dynamic] first request** |
| 3 | click | btnSubmit | <code></code> |
| 4 | snapshot |  | **Label [viewstate][dynamic] posted (set in code: kept; without view state: lost)** |
| 5 | click | btnShow | <code></code> |
| 6 | snapshot |  | **Label [visible][viewstate] hidden ones shown with what was set while hidden** |

<details><summary>Label [render][databind][dynamic] first request（期待される本文）</summary>

```text
Label: Plain bound Associated set in code set in code, no view state
Literal: <b>encoded</b> pass bound set in code set in code, no view state
CheckBox: Plain Auto Auto validates g1 Disabled Bound No view state
RadioButton: Small Large Auto A Auto B Auto validates g1 Bound
HiddenField:
LinkButton: Submit link Cancelled link Validate g1 by a link Disabled link bound Link to the target
HyperLink: Go to the target bound Disabled hyperlink
Image:
ImageButton:
Group
In a panel:
bound
Bound panel
Default link: Other link Default link
Default image:
PlaceHolder: Created in a place holder: bound
Dynamic: dynamic labeldynamic literalDynamicDynamicDynamic linkdynamic hyperlink
In a dynamic panel
Pen Pick Pen Bag Pick Bag
Taro Flag Choice
Required (g1, server): Cross-page:
Request 1: IsPostBack=False
State lblPlain Text="Plain"
State lblBound Text="bound"
State lblCode Text="set in code"
State lblNoViewState Text="set in code, no view state"
State lblHidden Text="set while hidden" (hidden)
State litBound Text="bound"
State litCode Text="set in code"
State litNoViewState Text="set in code, no view state"
State litHidden Text="set while hidden" (hidden)
State chkPlain Checked=False
State chkAuto Checked=False
State chkCauses Checked=False
State chkDisabled Checked=True
State chkBound Checked=True
State chkNoViewState Checked=True
State chkHidden Checked=True (hidden)
State rdoSmall Checked=False
State rdoLarge Checked=False
State rdoAutoA Checked=False
State rdoAutoB Checked=False
State rdoCauses Checked=False
State rdoBound Checked=True
State rdoHidden Checked=True (hidden)
State hidPlain Value="initial"
State hidBound Value="bound"
State hidNoViewState Value="set in code, no view state"
State hidHidden Value="set while hidden" (hidden)
State lnkBound Text="bound"
State lnkHidden Text="Hidden link (set while hidden)" (hidden)
State hypBound Text="bound" NavigateUrl="~/ButtonTarget.aspx?id=7"
State hypHidden Text="Hidden hyperlink" NavigateUrl="~/ButtonTarget.aspx?hidden=1" (hidden)
State imgBound AlternateText="bound"
State imgCode AlternateText="set in code"
State imgHidden AlternateText="set while hidden" (hidden)
State ibtnBound AlternateText="bound"
State ibtnHidden AlternateText="Hidden image (set while hidden)" (hidden)
State txtInPanel Text=""
State txtInHiddenPanel Text="set while hidden" (hidden)
State txtInHolder Text=""
State lblInHiddenHolder Text="set while hidden" (hidden)
State litInBoundHolder Text="bound"
State lblDynamic Text="dynamic label"
State litDynamic Text="dynamic literal"
State chkDynamic Checked=False
State rdoDynamic Checked=False
State hidDynamic Value="dynamic"
State lnkDynamic Text="Dynamic link"
State hypDynamic Text="dynamic hyperlink" NavigateUrl="~/ButtonTarget.aspx"
State imgDynamic AlternateText="dynamic image"
State ibtnDynamic AlternateText="Dynamic image"
```

</details>

<details><summary>Label [viewstate][dynamic] posted (set in code: kept; without view state: lost)（期待される本文）</summary>

```text
Label: Plain bound Associated set in code
Literal: <b>encoded</b> pass bound set in code
CheckBox: Plain Auto Auto validates g1 Disabled Bound No view state
RadioButton: Small Large Auto A Auto B Auto validates g1 Bound
HiddenField:
LinkButton: Submit link Cancelled link Validate g1 by a link Disabled link bound Link to the target
HyperLink: Go to the target bound Disabled hyperlink
Image:
ImageButton:
Group
In a panel:
bound
Bound panel
Default link: Other link Default link
Default image:
PlaceHolder: Created in a place holder: bound
Dynamic: dynamic labeldynamic literalDynamicDynamicDynamic linkdynamic hyperlink
In a dynamic panel
Pen Pick Pen Bag Pick Bag
Taro Flag Choice
Required (g1, server): Cross-page:
Request 2: IsPostBack=True
Changed chkNoViewState Checked=True
Changed hidNoViewState Value="set in code, no view state"
IsValid: (not validated)
State lblPlain Text="Plain"
State lblBound Text="bound"
State lblCode Text="set in code"
State lblNoViewState Text=""
State lblHidden Text="set while hidden" (hidden)
State litBound Text="bound"
State litCode Text="set in code"
State litNoViewState Text=""
State litHidden Text="set while hidden" (hidden)
State chkPlain Checked=False
State chkAuto Checked=False
State chkCauses Checked=False
State chkDisabled Checked=True
State chkBound Checked=True
State chkNoViewState Checked=True
State chkHidden Checked=True (hidden)
State rdoSmall Checked=False
State rdoLarge Checked=False
State rdoAutoA Checked=False
State rdoAutoB Checked=False
State rdoCauses Checked=False
State rdoBound Checked=True
State rdoHidden Checked=True (hidden)
State hidPlain Value="initial"
State hidBound Value="bound"
State hidNoViewState Value="set in code, no view state"
State hidHidden Value="set while hidden" (hidden)
State lnkBound Text="bound"
State lnkHidden Text="Hidden link (set while hidden)" (hidden)
State hypBound Text="bound" NavigateUrl="~/ButtonTarget.aspx?id=7"
State hypHidden Text="Hidden hyperlink" NavigateUrl="~/ButtonTarget.aspx?hidden=1" (hidden)
State imgBound AlternateText="bound"
State imgCode AlternateText="set in code"
State imgHidden AlternateText="set while hidden" (hidden)
State ibtnBound AlternateText="bound"
State ibtnHidden AlternateText="Hidden image (set while hidden)" (hidden)
State txtInPanel Text=""
State txtInHiddenPanel Text="set while hidden" (hidden)
State txtInHolder Text=""
State lblInHiddenHolder Text="set while hidden" (hidden)
State litInBoundHolder Text="bound"
State lblDynamic Text="dynamic label"
State litDynamic Text="dynamic literal"
State chkDynamic Checked=False
State rdoDynamic Checked=False
State hidDynamic Value="dynamic"
State lnkDynamic Text="Dynamic link"
State hypDynamic Text="dynamic hyperlink" NavigateUrl="~/ButtonTarget.aspx"
State imgDynamic AlternateText="dynamic image"
State ibtnDynamic AlternateText="Dynamic image"
```

</details>

<details><summary>Label [visible][viewstate] hidden ones shown with what was set while hidden（期待される本文）</summary>

```text
Label: Plain bound Associated set in code set while hidden
Literal: <b>encoded</b> pass bound set in code set while hidden
CheckBox: Plain Auto Auto validates g1 Disabled Bound No view state Hidden
RadioButton: Small Large Auto A Auto B Auto validates g1 Bound Hidden
HiddenField:
LinkButton: Submit link Cancelled link Validate g1 by a link Disabled link bound Link to the target Hidden link (set while hidden)
HyperLink: Go to the target bound Disabled hyperlink Hidden hyperlink
Image:
ImageButton:
Group
In a panel:
bound
Bound panel
In a hidden panel:
Default link: Other link Default link
Default image:
PlaceHolder: Created in a place holder: bound set while hidden
Dynamic: dynamic labeldynamic literalDynamicDynamicDynamic linkdynamic hyperlink
In a dynamic panel
Pen Pick Pen Bag Pick Bag
Taro Flag Choice
Required (g1, server): Cross-page:
Request 3: IsPostBack=True
Changed chkNoViewState Checked=True
Changed hidNoViewState Value="set in code, no view state"
Click btnShow
IsValid: (not validated)
State lblPlain Text="Plain"
State lblBound Text="bound"
State lblCode Text="set in code"
State lblNoViewState Text=""
State lblHidden Text="set while hidden"
State litBound Text="bound"
State litCode Text="set in code"
State litNoViewState Text=""
State litHidden Text="set while hidden"
State chkPlain Checked=False
State chkAuto Checked=False
State chkCauses Checked=False
State chkDisabled Checked=True
State chkBound Checked=True
State chkNoViewState Checked=True
State chkHidden Checked=True
State rdoSmall Checked=False
State rdoLarge Checked=False
State rdoAutoA Checked=False
State rdoAutoB Checked=False
State rdoCauses Checked=False
State rdoBound Checked=True
State rdoHidden Checked=True
State hidPlain Value="initial"
State hidBound Value="bound"
State hidNoViewState Value="set in code, no view state"
State hidHidden Value="set while hidden"
State lnkBound Text="bound"
State lnkHidden Text="Hidden link (set while hidden)"
State hypBound Text="bound" NavigateUrl="~/ButtonTarget.aspx?id=7"
State hypHidden Text="Hidden hyperlink" NavigateUrl="~/ButtonTarget.aspx?hidden=1"
State imgBound AlternateText="bound"
State imgCode AlternateText="set in code"
State imgHidden AlternateText="set while hidden"
State ibtnBound AlternateText="bound"
State ibtnHidden AlternateText="Hidden image (set while hidden)"
State txtInPanel Text=""
State txtInHiddenPanel Text="set while hidden"
State txtInHolder Text=""
State lblInHiddenHolder Text="set while hidden"
State litInBoundHolder Text="bound"
State lblDynamic Text="dynamic label"
State litDynamic Text="dynamic literal"
State chkDynamic Checked=False
State rdoDynamic Checked=False
State hidDynamic Value="dynamic"
State lnkDynamic Text="Dynamic link"
State hypDynamic Text="dynamic hyperlink" NavigateUrl="~/ButtonTarget.aspx"
State imgDynamic AlternateText="dynamic image"
State ibtnDynamic AlternateText="Dynamic image"
```

</details>

