# LoginName テストケース一覧

変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。
このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。

- 正解データの採取: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0
- ① API 層: 151 ケース（160 メンバー）
- ② HTML 層: 126 ケース
- ③ シナリオ層: 15 スナップショット

## 機構マトリクス

型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。

| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |
|---|---|---|---|
| プロパティ → HTML 描画 | すべてのコントロール | 109 | 1 |
| ViewState の保存と復元 | Control | 6 | 14 |
| Visible=false（描画されず状態は保持） | Control.Visible | 2 | 1 |
| 名前付けコンテナと ClientIDMode | Control.ClientIDMode | 6 | 0 |
| テーマとスキン（SkinID / EnableTheming） | テーマを付けられる型（[Themeable]） | 4 | 0 |
| 動的に生成したコントロールのライフサイクル | Control | 0 | 2 |
| データバインド（DataBind / <%# %>） | Control.DataBind | 0 | 1 |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | WebControl.Enabled、HtmlControl.Disabled | 2 | 0 |

該当しない機構: ポストバックデータの取り込み、変更イベント（*Changed、ServerChange）、ReadOnly（ポストされた値を無視）、リクエスト検証（ValidateRequestMode）、ポストバックイベント、コマンドイベントと親へのバブル（CommandName）、送信ボタン／スクリプトによるポストバック（UseSubmitBehavior）、ブラウザ側のクリック処理（OnClientClick）、クロスページポストバック（PostBackUrl）、既定のボタン（Panel の DefaultButton）、AutoPostBack（クライアントスクリプトによるポストバック）、CausesValidation / ValidationGroup、検証対象（サーバー側の検証）、検証対象（ブラウザ側の検証）、双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値）、項目（ListItem の Selected・Enabled・Text・Value と ViewState）、複数選択、繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）、ポスト値のイベント検証（一覧にない値・無効な項目の拒否）、サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）、ブラウザ側の検証スクリプト（EnableClientScript）、検証結果のまとめ（ValidationSummary）、ページング、並べ替え、項目の挿入、行・項目の編集（編集・更新・取り消し・削除）、行・項目の選択、行・項目のコマンド（RowCommand・ItemCommand へのバブル）、データキー（DataKeyNames・DataKeys）、データソースコントロールとの連携（DataSourceID）、テンプレート、データソース連携（DataSource・DataTextField など）、クライアントコールバック

## ① API 層

LoginName とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の LoginName インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。

| ケース | 期待値（.NET Framework） |
|---|---|
| <code>Api.Control.#ctor</code> | <code>Control.#ctor = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMode=In…</code> |
| <code>Api.Control.Adapter</code> | <code>Control.Adapter = null</code> |
| <code>Api.Control.AddParsedSubObject</code> | <code>Control.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>Control.AddParsedSubObject(Object) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; B…</code> |
| <code>Api.Control.AddedControl</code> | <code>Control.AddedControl(Control,Int32) = &quot;ok&quot;</code><br><code>Control.AddedControl(Control,Int32) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; …</code> |
| <code>Api.Control.AppRelativeTemplateSourceDirectory</code> | <code>Control.AppRelativeTemplateSourceDirectory = &quot;&quot;</code><br><code>Control.AppRelativeTemplateSourceDirectory set = &quot;-&gt; System.ArgumentException: The relative virtual path &#39;Abc 123&#39; is not allowed here.&quot;</code> |
| <code>Api.Control.ApplyStyleSheetSkin</code> | <code>Control.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>Control.ApplyStyleSheetSkin(Page) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bo…</code> |
| <code>Api.Control.BeginRenderTracing</code> | <code>Control.BeginRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.BeginRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.BeginRenderTracing(TextWriter,Object) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color…</code> |
| <code>Api.Control.BindingContainer</code> | <code>Control.BindingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Clien…</code> |
| <code>Api.Control.BuildProfileTree</code> | <code>Control.BuildProfileTree(String,Boolean) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.ChildControlsCreated</code> | <code>Control.ChildControlsCreated = &quot;false&quot;</code><br><code>Control.ChildControlsCreated set = &quot;ok&quot;</code> |
| <code>Api.Control.ClearCachedClientID</code> | <code>Control.ClearCachedClientID = &quot;ok&quot;</code><br><code>Control.ClearCachedClientID after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSt…</code> |
| <code>Api.Control.ClearChildControlState</code> | <code>Control.ClearChildControlState = &quot;ok&quot;</code><br><code>Control.ClearChildControlState after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Borde…</code> |
| <code>Api.Control.ClearChildState</code> | <code>Control.ClearChildState = &quot;ok&quot;</code><br><code>Control.ClearChildState after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=…</code> |
| <code>Api.Control.ClearChildViewState</code> | <code>Control.ClearChildViewState = &quot;ok&quot;</code><br><code>Control.ClearChildViewState after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSt…</code> |
| <code>Api.Control.ClearEffectiveClientIDMode</code> | <code>Control.ClearEffectiveClientIDMode = &quot;ok&quot;</code><br><code>Control.ClearEffectiveClientIDMode after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; B…</code> |
| <code>Api.Control.ClientID</code> | <code>Control.ClientID = &quot;lnName&quot;</code> |
| <code>Api.Control.ClientIDMode</code> | <code>Control.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>Control.ClientIDMode set = &quot;ClientIDMode.AutoID&quot;</code> |
| <code>Api.Control.ClientIDSeparator</code> | <code>Control.ClientIDSeparator = &quot;&#39;_&#39;&quot;</code> |
| <code>Api.Control.Context</code> | <code>Control.Context = null</code> |
| <code>Api.Control.Controls</code> | <code>Control.Controls = ControlCollection []</code> |
| <code>Api.Control.CreateChildControls</code> | <code>Control.CreateChildControls = &quot;ok&quot;</code><br><code>Control.CreateChildControls after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSt…</code> |
| <code>Api.Control.CreateControlCollection</code> | <code>Control.CreateControlCollection = ControlCollection []</code><br><code>Control.CreateControlCollection after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bord…</code> |
| <code>Api.Control.DataBind</code> | <code>Control.DataBind = &quot;ok&quot;</code><br><code>Control.DataBind after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet;…</code><br><code>Control.DataBind(Boolean) = &quot;ok&quot;</code><br><code>Control.DataBind(Boolean) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyl…</code> |
| <code>Api.Control.DataBindChildren</code> | <code>Control.DataBindChildren = &quot;ok&quot;</code><br><code>Control.DataBindChildren after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle…</code> |
| <code>Api.Control.DataBinding</code> | <code>Control.DataBinding = &quot;added and removed&quot;</code> |
| <code>Api.Control.DataItemContainer</code> | <code>Control.DataItemContainer = null</code> |
| <code>Api.Control.DataKeysContainer</code> | <code>Control.DataKeysContainer = null</code> |
| <code>Api.Control.DesignMode</code> | <code>Control.DesignMode = &quot;false&quot;</code> |
| <code>Api.Control.Dispose</code> | <code>Control.Dispose = &quot;ok&quot;</code><br><code>Control.Dispose after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; …</code> |
| <code>Api.Control.Disposed</code> | <code>Control.Disposed = &quot;added and removed&quot;</code> |
| <code>Api.Control.EnableTheming</code> | <code>Control.EnableTheming = &quot;true&quot;</code><br><code>Control.EnableTheming set = &quot;true&quot;</code> |
| <code>Api.Control.EnableViewState</code> | <code>Control.EnableViewState = &quot;true&quot;</code><br><code>Control.EnableViewState set = &quot;true&quot;</code> |
| <code>Api.Control.EndRenderTracing</code> | <code>Control.EndRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.EndRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.EndRenderTracing(TextWriter,Object) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code> |
| <code>Api.Control.EnsureChildControls</code> | <code>Control.EnsureChildControls = &quot;ok&quot;</code><br><code>Control.EnsureChildControls after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSt…</code> |
| <code>Api.Control.EnsureID</code> | <code>Control.EnsureID = &quot;ok&quot;</code><br><code>Control.EnsureID after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet;…</code> |
| <code>Api.Control.Events</code> | <code>Control.Events = &quot;System.ComponentModel.EventHandlerList&quot;</code> |
| <code>Api.Control.FindControl</code> | <code>Control.FindControl(String) = Label#lblTitle {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code><br><code>Control.FindControl(String) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSt…</code><br><code>Control.FindControl(String,Int32) = null</code><br><code>Control.FindControl(String,Int32) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bo…</code> |
| <code>Api.Control.Focus</code> | <code>Control.Focus = &quot;-&gt; System.InvalidOperationException: A form tag with runat=server must exist on the Page to use SetFocus() or the Focus property.&quot;</code> |
| <code>Api.Control.GetDesignModeState</code> | <code>Control.GetDesignModeState = &quot;[]&quot;</code><br><code>Control.GetDesignModeState after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSty…</code> |
| <code>Api.Control.GetRouteUrl</code> | <code>Control.GetRouteUrl(Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.GetUniqueIDRelativeTo</code> | <code>Control.GetUniqueIDRelativeTo(Control) = &quot;lnName&quot;</code><br><code>Control.GetUniqueIDRelativeTo(Control) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty…</code> |
| <code>Api.Control.HasChildViewState</code> | <code>Control.HasChildViewState = &quot;false&quot;</code> |
| <code>Api.Control.HasControls</code> | <code>Control.HasControls = &quot;false&quot;</code><br><code>Control.HasControls after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotS…</code> |
| <code>Api.Control.HasEvents</code> | <code>Control.HasEvents = &quot;false&quot;</code><br><code>Control.HasEvents after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet…</code> |
| <code>Api.Control.ID</code> | <code>Control.ID = &quot;lnName&quot;</code><br><code>Control.ID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.IdSeparator</code> | <code>Control.IdSeparator = &quot;&#39;$&#39;&quot;</code> |
| <code>Api.Control.Init</code> | <code>Control.Init = &quot;added and removed&quot;</code> |
| <code>Api.Control.IsChildControlStateCleared</code> | <code>Control.IsChildControlStateCleared = &quot;false&quot;</code> |
| <code>Api.Control.IsLiteralContent</code> | <code>Control.IsLiteralContent = &quot;false&quot;</code><br><code>Control.IsLiteralContent after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle…</code> |
| <code>Api.Control.IsTrackingViewState</code> | <code>Control.IsTrackingViewState = &quot;false&quot;</code> |
| <code>Api.Control.IsViewStateEnabled</code> | <code>Control.IsViewStateEnabled = &quot;true&quot;</code> |
| <code>Api.Control.Load</code> | <code>Control.Load = &quot;added and removed&quot;</code> |
| <code>Api.Control.LoadControlState</code> | <code>Control.LoadControlState(Object) = &quot;ok&quot;</code><br><code>Control.LoadControlState(Object) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bor…</code> |
| <code>Api.Control.LoadViewState</code> | <code>Control.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Web.UI.Pair&#39;.&quot;</code> |
| <code>Api.Control.LoadViewStateByID</code> | <code>Control.LoadViewStateByID = &quot;false&quot;</code> |
| <code>Api.Control.MapPathSecure</code> | <code>Control.MapPathSecure(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.NamingContainer</code> | <code>Control.NamingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code> |
| <code>Api.Control.OnBubbleEvent</code> | <code>Control.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Control.OnBubbleEvent(Object,EventArgs) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empt…</code> |
| <code>Api.Control.OnDataBinding</code> | <code>Control.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnDataBinding(EventArgs) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bor…</code> |
| <code>Api.Control.OnInit</code> | <code>Control.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnInit(EventArgs) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyl…</code> |
| <code>Api.Control.OnLoad</code> | <code>Control.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnLoad(EventArgs) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyl…</code> |
| <code>Api.Control.OnPreRender</code> | <code>Control.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnPreRender(EventArgs) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Borde…</code> |
| <code>Api.Control.OnUnload</code> | <code>Control.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnUnload(EventArgs) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSt…</code> |
| <code>Api.Control.OpenFile</code> | <code>Control.OpenFile(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.Page</code> | <code>Control.Page = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=&quot;&quot;; …</code><br><code>Control.Page set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=…</code> |
| <code>Api.Control.Parent</code> | <code>Control.Parent = HtmlForm#form1 {Action=&quot;&quot;; ClientID=&quot;form1&quot;; ClientIDMode=Inherit; DefaultButton=&quot;&quot;; DefaultFocus=&quot;&quot;; Disabled=false; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.PreRender</code> | <code>Control.PreRender = &quot;added and removed&quot;</code> |
| <code>Api.Control.RaiseBubbleEvent</code> | <code>Control.RaiseBubbleEvent(Object,EventArgs) = &quot;ok&quot;</code><br><code>Control.RaiseBubbleEvent(Object,EventArgs) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [E…</code> |
| <code>Api.Control.RemovedControl</code> | <code>Control.RemovedControl(Control) = &quot;ok&quot;</code><br><code>Control.RemovedControl(Control) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bord…</code> |
| <code>Api.Control.Render</code> | <code>Control.Render(HtmlTextWriter) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.RenderChildren</code> | <code>Control.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderChildren(HtmlTextWriter) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty…</code> |
| <code>Api.Control.RenderControl</code> | <code>Control.RenderControl(HtmlTextWriter) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.RenderingCompatibility</code> | <code>Control.RenderingCompatibility = &quot;System.Version 4.0&quot;</code><br><code>Control.RenderingCompatibility set = &quot;System.Version 4.0&quot;</code> |
| <code>Api.Control.ResolveAdapter</code> | <code>Control.ResolveAdapter = null</code><br><code>Control.ResolveAdapter after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=N…</code> |
| <code>Api.Control.ResolveClientUrl</code> | <code>Control.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveClientUrl(String) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bor…</code> |
| <code>Api.Control.ResolveUrl</code> | <code>Control.ResolveUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveUrl(String) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSty…</code> |
| <code>Api.Control.SaveControlState</code> | <code>Control.SaveControlState = null</code><br><code>Control.SaveControlState after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle…</code> |
| <code>Api.Control.SaveViewState</code> | <code>Control.SaveViewState = null</code><br><code>Control.SaveViewState after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=No…</code> |
| <code>Api.Control.SetDesignModeState</code> | <code>Control.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>Control.SetDesignModeState(IDictionary) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empt…</code> |
| <code>Api.Control.SetRenderMethodDelegate</code> | <code>Control.SetRenderMethodDelegate(RenderMethod) = &quot;ok&quot;</code><br><code>Control.SetRenderMethodDelegate(RenderMethod) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color…</code> |
| <code>Api.Control.SetTraceData</code> | <code>Control.SetTraceData(Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; …</code><br><code>Control.SetTraceData(Object,Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object,Object) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [E…</code> |
| <code>Api.Control.Site</code> | <code>Control.Site = null</code><br><code>Control.Site set = null</code> |
| <code>Api.Control.SkinID</code> | <code>Control.SkinID = &quot;&quot;</code><br><code>Control.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.TemplateControl</code> | <code>Control.TemplateControl = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code><br><code>Control.TemplateControl set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Cl…</code> |
| <code>Api.Control.TemplateSourceDirectory</code> | <code>Control.TemplateSourceDirectory = &quot;&quot;</code> |
| <code>Api.Control.TrackViewState</code> | <code>Control.TrackViewState = &quot;ok&quot;</code><br><code>Control.TrackViewState after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=N…</code> |
| <code>Api.Control.UniqueID</code> | <code>Control.UniqueID = &quot;lnName&quot;</code> |
| <code>Api.Control.Unload</code> | <code>Control.Unload = &quot;added and removed&quot;</code> |
| <code>Api.Control.ValidateRequestMode</code> | <code>Control.ValidateRequestMode = &quot;ValidateRequestMode.Inherit&quot;</code><br><code>Control.ValidateRequestMode set = &quot;ValidateRequestMode.Disabled&quot;</code> |
| <code>Api.Control.ViewState</code> | <code>Control.ViewState = StateBag []</code> |
| <code>Api.Control.ViewStateIgnoresCase</code> | <code>Control.ViewStateIgnoresCase = &quot;false&quot;</code> |
| <code>Api.Control.ViewStateMode</code> | <code>Control.ViewStateMode = &quot;ViewStateMode.Inherit&quot;</code><br><code>Control.ViewStateMode set = &quot;ViewStateMode.Enabled&quot;</code> |
| <code>Api.Control.Visible</code> | <code>Control.Visible = &quot;true&quot;</code><br><code>Control.Visible set = &quot;true&quot;</code> |
| <code>Api.IAttributeAccessor.GetAttribute</code> | <code>IAttributeAccessor.GetAttribute(String) = null</code><br><code>IAttributeAccessor.GetAttribute(String) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empt…</code> |
| <code>Api.IAttributeAccessor.SetAttribute</code> | <code>IAttributeAccessor.SetAttribute(String,String) = &quot;ok&quot;</code><br><code>IAttributeAccessor.SetAttribute(String,String) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Colo…</code> |
| <code>Api.IControlBuilderAccessor.ControlBuilder</code> | <code>IControlBuilderAccessor.ControlBuilder = null</code> |
| <code>Api.IControlDesignerAccessor.GetDesignModeState</code> | <code>IControlDesignerAccessor.GetDesignModeState = &quot;[]&quot;</code><br><code>IControlDesignerAccessor.GetDesignModeState after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code> |
| <code>Api.IControlDesignerAccessor.SetDesignModeState</code> | <code>IControlDesignerAccessor.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetDesignModeState(IDictionary) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #000…</code> |
| <code>Api.IControlDesignerAccessor.SetOwnerControl</code> | <code>IControlDesignerAccessor.SetOwnerControl(Control) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetOwnerControl(Control) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 C…</code> |
| <code>Api.IControlDesignerAccessor.UserData</code> | <code>IControlDesignerAccessor.UserData = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.DataBindings</code> | <code>IDataBindingsAccessor.DataBindings = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.HasDataBindings</code> | <code>IDataBindingsAccessor.HasDataBindings = &quot;false&quot;</code> |
| <code>Api.IExpressionsAccessor.Expressions</code> | <code>IExpressionsAccessor.Expressions = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.HasExpressions</code> | <code>IExpressionsAccessor.HasExpressions = &quot;false&quot;</code> |
| <code>Api.IParserAccessor.AddParsedSubObject</code> | <code>IParserAccessor.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>IParserAccessor.AddParsedSubObject(Object) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [E…</code> |
| <code>Api.IUrlResolutionService.ResolveClientUrl</code> | <code>IUrlResolutionService.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>IUrlResolutionService.ResolveClientUrl(String) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Colo…</code> |
| <code>Api.WebControls.LoginName.#ctor</code> | <code>LoginName.#ctor = LoginName# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=…</code> |
| <code>Api.WebControls.LoginName.FormatString</code> | <code>LoginName.FormatString = &quot;{0}&quot;</code><br><code>LoginName.FormatString set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.LoginName.Render</code> | <code>LoginName.Render(HtmlTextWriter) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.WebControls.LoginName.RenderBeginTag</code> | <code>LoginName.RenderBeginTag(HtmlTextWriter) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.WebControls.LoginName.RenderContents</code> | <code>LoginName.RenderContents(HtmlTextWriter) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.WebControls.LoginName.RenderEndTag</code> | <code>LoginName.RenderEndTag(HtmlTextWriter) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.WebControls.LoginName.SupportsDisabledAttribute</code> | <code>LoginName.SupportsDisabledAttribute = &quot;false&quot;</code> |
| <code>Api.WebControls.WebControl.#ctor</code> | <code>WebControl.#ctor = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidt…</code><br><code>WebControl.#ctor(String) = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; Bo…</code><br><code>WebControl.#ctor(HtmlTextWriterTag) = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle…</code> |
| <code>Api.WebControls.WebControl.AccessKey</code> | <code>WebControl.AccessKey = &quot;&quot;</code><br><code>WebControl.AccessKey set = &quot;-&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]&quot;</code> |
| <code>Api.WebControls.WebControl.AddAttributesToRender</code> | <code>WebControl.AddAttributesToRender(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.AddAttributesToRender(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>WebControl.AddAttributesToRender(HtmlTextWriter) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Co…</code> |
| <code>Api.WebControls.WebControl.ApplyStyle</code> | <code>WebControl.ApplyStyle(Style) = &quot;ok&quot;</code><br><code>WebControl.ApplyStyle(Style) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderS…</code> |
| <code>Api.WebControls.WebControl.Attributes</code> | <code>WebControl.Attributes = AttributeCollection []</code> |
| <code>Api.WebControls.WebControl.BackColor</code> | <code>WebControl.BackColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>WebControl.BackColor set = &quot;Color #FF3366CC Color [A=255, R=51, G=102, B=204]&quot;</code> |
| <code>Api.WebControls.WebControl.BorderColor</code> | <code>WebControl.BorderColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>WebControl.BorderColor set = &quot;Color #FF3366CC Color [A=255, R=51, G=102, B=204]&quot;</code> |
| <code>Api.WebControls.WebControl.BorderStyle</code> | <code>WebControl.BorderStyle = &quot;BorderStyle.NotSet&quot;</code><br><code>WebControl.BorderStyle set = &quot;BorderStyle.None&quot;</code> |
| <code>Api.WebControls.WebControl.BorderWidth</code> | <code>WebControl.BorderWidth = Unit </code><br><code>WebControl.BorderWidth set = Unit 120px</code> |
| <code>Api.WebControls.WebControl.ControlStyle</code> | <code>WebControl.ControlStyle = Style {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; CssClass=…</code> |
| <code>Api.WebControls.WebControl.ControlStyleCreated</code> | <code>WebControl.ControlStyleCreated = &quot;false&quot;</code> |
| <code>Api.WebControls.WebControl.CopyBaseAttributes</code> | <code>WebControl.CopyBaseAttributes(WebControl) = &quot;ok&quot;</code><br><code>WebControl.CopyBaseAttributes(WebControl) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Em…</code> |
| <code>Api.WebControls.WebControl.CreateControlStyle</code> | <code>WebControl.CreateControlStyle = Style {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; Css…</code><br><code>WebControl.CreateControlStyle after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Border…</code> |
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
| <code>Api.WebControls.WebControl.MergeStyle</code> | <code>WebControl.MergeStyle(Style) = &quot;ok&quot;</code><br><code>WebControl.MergeStyle(Style) after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderS…</code> |
| <code>Api.WebControls.WebControl.Render</code> | <code>WebControl.Render(HtmlTextWriter) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.WebControls.WebControl.RenderBeginTag</code> | <code>WebControl.RenderBeginTag(HtmlTextWriter) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.WebControls.WebControl.RenderContents</code> | <code>WebControl.RenderContents(HtmlTextWriter) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.WebControls.WebControl.RenderEndTag</code> | <code>WebControl.RenderEndTag(HtmlTextWriter) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.WebControls.WebControl.SaveViewState</code> | <code>WebControl.SaveViewState = null</code><br><code>WebControl.SaveViewState after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle…</code> |
| <code>Api.WebControls.WebControl.SkinID</code> | <code>WebControl.SkinID = &quot;&quot;</code><br><code>WebControl.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.Style</code> | <code>WebControl.Style = CssStyleCollection {Count=Int32 0; Value=null}</code> |
| <code>Api.WebControls.WebControl.SupportsDisabledAttribute</code> | <code>WebControl.SupportsDisabledAttribute = &quot;false&quot;</code> |
| <code>Api.WebControls.WebControl.TabIndex</code> | <code>WebControl.TabIndex = &quot;Int16 0&quot;</code><br><code>WebControl.TabIndex set = &quot;Int16 1&quot;</code> |
| <code>Api.WebControls.WebControl.TagKey</code> | <code>WebControl.TagKey = &quot;HtmlTextWriterTag.Span&quot;</code> |
| <code>Api.WebControls.WebControl.TagName</code> | <code>WebControl.TagName = &quot;span&quot;</code> |
| <code>Api.WebControls.WebControl.ToolTip</code> | <code>WebControl.ToolTip = &quot;&quot;</code><br><code>WebControl.ToolTip set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.TrackViewState</code> | <code>WebControl.TrackViewState = &quot;ok&quot;</code><br><code>WebControl.TrackViewState after = LoginName#lnName {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyl…</code> |
| <code>Api.WebControls.WebControl.Width</code> | <code>WebControl.Width = Unit </code><br><code>WebControl.Width set = Unit 120px</code> |

## ② HTML 層

プローブサイトの RenderProbe.aspx で、1 ケース 1 リクエスト（ページライフサイクル全体）で描画したページ全体を比較します。下表はコンポーネント部分（div#probe）、設定時の例外、保存された ViewState の要約です。

| ケース | 設定 | 期待される描画 | 設定時の例外 |
|---|---|---|---|
| <code>.(default)</code> | <code>(nothing set)</code> | <code></code> |  |
| <code>.AccessKey=text</code> | <code>c.AccessKey = &quot;probe&quot;;</code> | <code></code> | <code>c.AccessKey = &quot;probe&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=markup</code> | <code>c.AccessKey = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> | <code>c.AccessKey = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=japanese</code> | <code>c.AccessKey = &quot;日本語テキスト&quot;;</code> | <code></code> | <code>c.AccessKey = &quot;日本語テキスト&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=empty</code> | <code>c.AccessKey = &quot;&quot;;</code> | <code></code> |  |
| <code>.AppRelativeTemplateSourceDirectory=text</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;;</code> | <code></code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;probe&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=markup</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=japanese</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;;</code> | <code></code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;日本語テキスト&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=empty</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&quot;;</code> | <code></code> |  |
| <code>.BackColor=Red</code> | <code>c.BackColor = Color.Red;</code> | <code></code> |  |
| <code>.BackColor=#3366CC</code> | <code>c.BackColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code></code> |  |
| <code>.BackColor=Transparent</code> | <code>c.BackColor = Color.Transparent;</code> | <code></code> |  |
| <code>.BorderColor=Red</code> | <code>c.BorderColor = Color.Red;</code> | <code></code> |  |
| <code>.BorderColor=#3366CC</code> | <code>c.BorderColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code></code> |  |
| <code>.BorderColor=Transparent</code> | <code>c.BorderColor = Color.Transparent;</code> | <code></code> |  |
| <code>.BorderStyle=NotSet</code> | <code>c.BorderStyle = BorderStyle.NotSet;</code> | <code></code> |  |
| <code>.BorderStyle=None</code> | <code>c.BorderStyle = BorderStyle.None;</code> | <code></code> |  |
| <code>.BorderStyle=Dotted</code> | <code>c.BorderStyle = BorderStyle.Dotted;</code> | <code></code> |  |
| <code>.BorderStyle=Dashed</code> | <code>c.BorderStyle = BorderStyle.Dashed;</code> | <code></code> |  |
| <code>.BorderStyle=Solid</code> | <code>c.BorderStyle = BorderStyle.Solid;</code> | <code></code> |  |
| <code>.BorderStyle=Double</code> | <code>c.BorderStyle = BorderStyle.Double;</code> | <code></code> |  |
| <code>.BorderStyle=Groove</code> | <code>c.BorderStyle = BorderStyle.Groove;</code> | <code></code> |  |
| <code>.BorderStyle=Ridge</code> | <code>c.BorderStyle = BorderStyle.Ridge;</code> | <code></code> |  |
| <code>.BorderStyle=Inset</code> | <code>c.BorderStyle = BorderStyle.Inset;</code> | <code></code> |  |
| <code>.BorderStyle=Outset</code> | <code>c.BorderStyle = BorderStyle.Outset;</code> | <code></code> |  |
| <code>.BorderWidth=120px</code> | <code>c.BorderWidth = Unit.Pixel(120);</code> | <code></code> |  |
| <code>.BorderWidth=50%</code> | <code>c.BorderWidth = Unit.Percentage(50);</code> | <code></code> | <code>c.BorderWidth = Unit.Percentage(50); -&gt; System.ArgumentOutOfRangeException: BorderWidth must be a non-negative number and cannot be a percentage. [param value]</code> |
| <code>.BorderWidth=2em</code> | <code>c.BorderWidth = new Unit(2, UnitType.Em);</code> | <code></code> |  |
| <code>.ClientIDMode=Inherit</code> | <code>c.ClientIDMode = ClientIDMode.Inherit;</code> | <code></code> |  |
| <code>.ClientIDMode=AutoID</code> | <code>c.ClientIDMode = ClientIDMode.AutoID;</code> | <code></code> |  |
| <code>.ClientIDMode=Predictable</code> | <code>c.ClientIDMode = ClientIDMode.Predictable;</code> | <code></code> |  |
| <code>.ClientIDMode=Static</code> | <code>c.ClientIDMode = ClientIDMode.Static;</code> | <code></code> |  |
| <code>.CssClass=text</code> | <code>c.CssClass = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.CssClass=markup</code> | <code>c.CssClass = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.CssClass=japanese</code> | <code>c.CssClass = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.CssClass=empty</code> | <code>c.CssClass = &quot;&quot;;</code> | <code></code> |  |
| <code>.EnableTheming=true</code> | <code>c.EnableTheming = true;</code> | <code></code> |  |
| <code>.EnableTheming=false</code> | <code>c.EnableTheming = false;</code> | <code></code> |  |
| <code>.EnableViewState=true</code> | <code>c.EnableViewState = true;</code> | <code></code> |  |
| <code>.EnableViewState=false</code> | <code>c.EnableViewState = false;</code> | <code></code> |  |
| <code>.Enabled=true</code> | <code>c.Enabled = true;</code> | <code></code> |  |
| <code>.Enabled=false</code> | <code>c.Enabled = false;</code> | <code></code> |  |
| <code>.Font.Bold=true</code> | <code>c.Font.Bold = true;</code> | <code></code> |  |
| <code>.Font.Bold=false</code> | <code>c.Font.Bold = false;</code> | <code></code> |  |
| <code>.Font.Italic=true</code> | <code>c.Font.Italic = true;</code> | <code></code> |  |
| <code>.Font.Italic=false</code> | <code>c.Font.Italic = false;</code> | <code></code> |  |
| <code>.Font.Name=text</code> | <code>c.Font.Name = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.Font.Name=markup</code> | <code>c.Font.Name = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.Font.Name=japanese</code> | <code>c.Font.Name = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.Font.Name=empty</code> | <code>c.Font.Name = &quot;&quot;;</code> | <code></code> |  |
| <code>.Font.Names=Arial,MS-Gothic</code> | <code>c.Font.Names = new[] { &quot;Arial&quot;, &quot;MS Gothic&quot; };</code> | <code></code> |  |
| <code>.Font.Overline=true</code> | <code>c.Font.Overline = true;</code> | <code></code> |  |
| <code>.Font.Overline=false</code> | <code>c.Font.Overline = false;</code> | <code></code> |  |
| <code>.Font.Size=12pt</code> | <code>c.Font.Size = FontUnit.Point(12);</code> | <code></code> |  |
| <code>.Font.Size=Large</code> | <code>c.Font.Size = FontUnit.Large;</code> | <code></code> |  |
| <code>.Font.Size=150%</code> | <code>c.Font.Size = new FontUnit(Unit.Percentage(150));</code> | <code></code> |  |
| <code>.Font.Strikeout=true</code> | <code>c.Font.Strikeout = true;</code> | <code></code> |  |
| <code>.Font.Strikeout=false</code> | <code>c.Font.Strikeout = false;</code> | <code></code> |  |
| <code>.Font.Underline=true</code> | <code>c.Font.Underline = true;</code> | <code></code> |  |
| <code>.Font.Underline=false</code> | <code>c.Font.Underline = false;</code> | <code></code> |  |
| <code>.ForeColor=Red</code> | <code>c.ForeColor = Color.Red;</code> | <code></code> |  |
| <code>.ForeColor=#3366CC</code> | <code>c.ForeColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code></code> |  |
| <code>.ForeColor=Transparent</code> | <code>c.ForeColor = Color.Transparent;</code> | <code></code> |  |
| <code>.FormatString=text</code> | <code>c.FormatString = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.FormatString=markup</code> | <code>c.FormatString = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.FormatString=japanese</code> | <code>c.FormatString = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.FormatString=empty</code> | <code>c.FormatString = &quot;&quot;;</code> | <code></code> |  |
| <code>.FormatString=welcome</code> | <code>c.FormatString = &quot;Welcome, {0}!&quot;;</code> | <code></code> |  |
| <code>.FormatString=invalid</code> | <code>c.FormatString = &quot;{1}&quot;;</code> | <code></code> |  |
| <code>.Height=120px</code> | <code>c.Height = Unit.Pixel(120);</code> | <code></code> |  |
| <code>.Height=50%</code> | <code>c.Height = Unit.Percentage(50);</code> | <code></code> |  |
| <code>.Height=2em</code> | <code>c.Height = new Unit(2, UnitType.Em);</code> | <code></code> |  |
| <code>.ID=text</code> | <code>c.ID = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.ID=markup</code> | <code>c.ID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.ID=japanese</code> | <code>c.ID = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.ID=empty</code> | <code>c.ID = &quot;&quot;;</code> | <code></code> |  |
| <code>.SkinID=text</code> | <code>c.SkinID = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.SkinID=markup</code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.SkinID=japanese</code> | <code>c.SkinID = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.SkinID=empty</code> | <code>c.SkinID = &quot;&quot;;</code> | <code></code> |  |
| <code>.TabIndex=0</code> | <code>c.TabIndex = 0;</code> | <code></code> |  |
| <code>.TabIndex=3</code> | <code>c.TabIndex = 3;</code> | <code></code> |  |
| <code>.TabIndex=-1</code> | <code>c.TabIndex = -1;</code> | <code></code> |  |
| <code>.ToolTip=text</code> | <code>c.ToolTip = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.ToolTip=markup</code> | <code>c.ToolTip = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.ToolTip=japanese</code> | <code>c.ToolTip = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.ToolTip=empty</code> | <code>c.ToolTip = &quot;&quot;;</code> | <code></code> |  |
| <code>.ValidateRequestMode=Inherit</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Inherit;</code> | <code></code> |  |
| <code>.ValidateRequestMode=Disabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Disabled;</code> | <code></code> |  |
| <code>.ValidateRequestMode=Enabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Enabled;</code> | <code></code> |  |
| <code>.ViewStateMode=Inherit</code> | <code>c.ViewStateMode = ViewStateMode.Inherit;</code> | <code></code> |  |
| <code>.ViewStateMode=Enabled</code> | <code>c.ViewStateMode = ViewStateMode.Enabled;</code> | <code></code> |  |
| <code>.ViewStateMode=Disabled</code> | <code>c.ViewStateMode = ViewStateMode.Disabled;</code> | <code></code> |  |
| <code>.Visible=true</code> | <code>c.Visible = true;</code> | <code></code> |  |
| <code>.Visible=false</code> | <code>c.Visible = false;</code> | <code></code> |  |
| <code>.Width=120px</code> | <code>c.Width = Unit.Pixel(120);</code> | <code></code> |  |
| <code>.Width=50%</code> | <code>c.Width = Unit.Percentage(50);</code> | <code></code> |  |
| <code>.Width=2em</code> | <code>c.Width = new Unit(2, UnitType.Em);</code> | <code></code> |  |
| <code>.Attributes[data-probe]=text</code> | <code>c.Attributes[data-probe] = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.Attributes[onfocus]=text</code> | <code>c.Attributes[onfocus] = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.Attributes[data-probe]=markup</code> | <code>c.Attributes[data-probe] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.Attributes[onfocus]=markup</code> | <code>c.Attributes[onfocus] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.Style[color]=red</code> | <code>c.Style[color] = &quot;red&quot;;</code> | <code></code> |  |
| <code>.Style[margin-left]=red</code> | <code>c.Style[margin-left] = &quot;red&quot;;</code> | <code></code> |  |
| <code>.Style[color]=1em</code> | <code>c.Style[color] = &quot;1em&quot;;</code> | <code></code> |  |
| <code>.Style[margin-left]=1em</code> | <code>c.Style[margin-left] = &quot;1em&quot;;</code> | <code></code> |  |
| <code>.Enabled=false+CssClass=text</code> | <code>c.Enabled = false; c.CssClass = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | <code>c.Width = Unit.Pixel(120); c.Height = new Unit(2, UnitType.Em); c.BorderWidth = Unit.Pixel(120); c.BorderStyle = BorderStyle.Solid; c.BackColor = Color.FromArgb(0x33, 0x66, 0xCC); c.ForeColor = Color.Red; c.Font.Bold = true;</code> | <code></code> |  |
| <code>.ForeColor=#3366CC+Style[color]=red</code> | <code>c.ForeColor = Color.FromArgb(0x33, 0x66, 0xCC); c.Style[color] = &quot;red&quot;;</code> | <code></code> |  |
| <code>.FormatString=welcome+Visible=false</code> | <code>c.FormatString = &quot;Welcome, {0}!&quot;; c.Visible = false;</code> | <code></code> |  |
| <code>[naming container].(default)</code> | <code>[naming container] (nothing set)</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=AutoID</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Static</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Predictable</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Inherit</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ID=text</code> | <code>[naming container] c.ID = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[themed].(default)</code> | <code>[themed] (nothing set)</code> | <code></code> |  |
| <code>[themed].SkinID=text</code> | <code>[themed] c.SkinID = &quot;probe&quot;;</code> | <code></code> |  |
| <code>[themed].EnableTheming=false</code> | <code>[themed] c.EnableTheming = false;</code> | <code></code> |  |
| <code>[themed].CssClass=text</code> | <code>[themed] c.CssClass = &quot;probe&quot;;</code> | <code></code> |  |
| <code>[logged in].(default)</code> | <code>[logged in] (nothing set)</code> | <code>&lt;span id=&quot;lnName&quot;&gt;taro&lt;/span&gt;</code> |  |
| <code>[logged in].FormatString=welcome</code> | <code>[logged in] c.FormatString = &quot;Welcome, {0}!&quot;;</code> | <code>&lt;span id=&quot;lnName&quot;&gt;Welcome, taro!&lt;/span&gt;</code> |  |
| <code>[logged in].FormatString=markup</code> | <code>[logged in] c.FormatString = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;lnName&quot;&gt;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&lt;/span&gt;</code> |  |
| <code>[logged in].FormatString=invalid</code> | <code>[logged in] c.FormatString = &quot;{1}&quot;;</code> | <code>status = 500</code> |  |
| <code>[logged in].FormatString=welcome+CssClass=text</code> | <code>[logged in] c.FormatString = &quot;Welcome, {0}!&quot;; c.CssClass = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;lnName&quot; class=&quot;probe&quot;&gt;Welcome, taro!&lt;/span&gt;</code> |  |
| <code>[logged in].FormatString=welcome+EnableViewState=false</code> | <code>[logged in] c.FormatString = &quot;Welcome, {0}!&quot;; c.EnableViewState = false;</code> | <code>&lt;span id=&quot;lnName&quot;&gt;Welcome, taro!&lt;/span&gt;</code> |  |

## ③ シナリオ層

ブラウザ（Playwright）で LoginNameScenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。

| # | 操作 | 対象 | 値 |
|---|---|---|---|
| 1 | goto | /LoginScenario.aspx?reset=1 | <code></code> |
| 2 | snapshot |  | **LoginName [render][databind][dynamic] first request, anonymous: the login forms, the status, the views, the wizards** |
| 3 | fill | lgnMain_UserName | <code>taro</code> |
| 4 | fill | lgnMain_Password | <code>wrong</code> |
| 5 | click | lgnMain_LoginButton | <code></code> |
| 6 | snapshot |  | **LoginName [viewstate] a login with a wrong password: its error** |
| 7 | fill | lgnMain_UserName | <code>taro</code> |
| 8 | fill | lgnMain_Password | <code>Passw0rd!</code> |
| 9 | click | lgnMain_LoginButton | <code></code> |
| 10 | snapshot |  | **LoginName [viewstate] logged in (redirected to the page): the name, the status, the role&#39;s view** |
| 11 | fill | cpwChange_ChangePasswordContainerID_CurrentPassword | <code>Passw0rd!</code> |
| 12 | fill | cpwChange_ChangePasswordContainerID_NewPassword | <code>Changed1!</code> |
| 13 | fill | cpwChange_ChangePasswordContainerID_ConfirmNewPassword | <code>Changed1!</code> |
| 14 | click | cpwChange_ChangePasswordContainerID_ChangePasswordPushButton | <code></code> |
| 15 | snapshot |  | **LoginName [viewstate] the password changed: the success template** |
| 16 | click | lsStatus | <code></code> |
| 17 | snapshot |  | **LoginName [viewstate] logged out by the status** |
| 18 | fill | prRecover_UserNameContainerID_UserName | <code>nobody</code> |
| 19 | click | prRecover_UserNameContainerID_SubmitButton | <code></code> |
| 20 | snapshot |  | **LoginName [viewstate] a recovery for a user who does not exist** |
| 21 | fill | prRecover_UserNameContainerID_UserName | <code>taro</code> |
| 22 | click | prRecover_UserNameContainerID_SubmitButton | <code></code> |
| 23 | snapshot |  | **LoginName [viewstate] the user&#39;s question** |
| 24 | fill | prRecover_QuestionContainerID_Answer | <code>blue</code> |
| 25 | click | prRecover_QuestionContainerID_SubmitButton | <code></code> |
| 26 | snapshot |  | **LoginName [viewstate] the answer right: the password reset, its mail prepared (cancelled), the success template** |
| 27 | fill | cuwCreate_CreateUserStepContainer_UserName | <code>hanako</code> |
| 28 | fill | cuwCreate_CreateUserStepContainer_Password | <code>Hanako1!</code> |
| 29 | fill | cuwCreate_CreateUserStepContainer_ConfirmPassword | <code>Hanako1!</code> |
| 30 | fill | cuwCreate_CreateUserStepContainer_Email | <code>hanako@example.com</code> |
| 31 | fill | cuwCreate_CreateUserStepContainer_Question | <code>pet</code> |
| 32 | fill | cuwCreate_CreateUserStepContainer_Answer | <code>cat</code> |
| 33 | clicksubmit |  | <code>Create the user</code> |
| 34 | snapshot |  | **LoginName [viewstate] a user created: the complete step** |
| 35 | fill | txtFirst | <code>first step</code> |
| 36 | clicktextin | wizSteps | <code>Next</code> |
| 37 | snapshot |  | **LoginName [viewstate] the wizard&#39;s next step (the templated one)** |
| 38 | clicktextin | wizSteps | <code>Next</code> |
| 39 | clicktextin | wizSteps | <code>Previous</code> |
| 40 | snapshot |  | **LoginName [viewstate] next, then previous** |
| 41 | clicktextin | wizSteps | <code>Last</code> |
| 42 | snapshot |  | **LoginName [viewstate] a step chosen in the side bar** |
| 43 | clicktextin | wizSteps | <code>Finish</code> |
| 44 | snapshot |  | **LoginName [viewstate] finished: the complete step, the first step&#39;s value kept** |
| 45 | fill | txtDynamic | <code>in a dynamic wizard</code> |
| 46 | clicktextin | wizDynamic | <code>Next</code> |
| 47 | snapshot |  | **LoginName [dynamic][viewstate] a wizard made in code: its next step** |
| 48 | click | btnShow | <code></code> |
| 49 | snapshot |  | **LoginName [visible][viewstate] a hidden wizard shown** |

<details><summary>LoginName [render][databind][dynamic] first request, anonymous: the login forms, the status, the views, the wizards（期待される本文）</summary>

```text
Status: Login Name:
Anonymous view
Log In
User Name:
Password:
Remember me next time.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
Forgot Your Password?
Enter your User Name to receive your password.
User Name:
Create
User Name:
Password:
Confirm Password:
E-mail:
Security Question:
Security Answer:
Skip Navigation Links.
First
Second
Last
Done
[wizard header]
First:
Dynamic: Login
Skip Navigation Links.
Dynamic first
Dynamic last
Request 1: IsPostBack=False User=(anonymous)
State users: taro
State wizSteps ActiveStepIndex=0 txtFirst=""
State wizDynamic ActiveStepIndex=0
State cuwCreate ActiveStepIndex=0
State User=(anonymous)
```

</details>

<details><summary>LoginName [viewstate] a login with a wrong password: its error（期待される本文）</summary>

```text
Status: Login Name:
Anonymous view
Log In
User Name:
Password:
Remember me next time.
Your login attempt was not successful. Please try again.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
Forgot Your Password?
Enter your User Name to receive your password.
User Name:
Create
User Name:
Password:
Confirm Password:
E-mail:
Security Question:
Security Answer:
Skip Navigation Links.
First
Second
Last
Done
[wizard header]
First:
Dynamic: Login
Skip Navigation Links.
Dynamic first
Dynamic last
Request 2: IsPostBack=True User=(anonymous)
LoggingIn taro
Authenticate taro Authenticated=False
LoginError taro
State users: taro
State wizSteps ActiveStepIndex=0 txtFirst=""
State wizDynamic ActiveStepIndex=0
State cuwCreate ActiveStepIndex=0
State User=(anonymous)
```

</details>

<details><summary>LoginName [viewstate] logged in (redirected to the page): the name, the status, the role&#39;s view（期待される本文）</summary>

```text
Status: Logout Name: Hello, taro
Admins' view: taro
Log In
User Name:
Password:
Remember me next time.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
Forgot Your Password?
Enter your User Name to receive your password.
User Name:
Create
User Name:
Password:
Confirm Password:
E-mail:
Security Question:
Security Answer:
Skip Navigation Links.
First
Second
Last
Done
[wizard header]
First:
Dynamic: Logout
Skip Navigation Links.
Dynamic first
Dynamic last
Request 1: IsPostBack=False User=taro
State users: taro
State wizSteps ActiveStepIndex=0 txtFirst=""
State wizDynamic ActiveStepIndex=0
State cuwCreate ActiveStepIndex=0
State User=taro admins=True
```

</details>

<details><summary>LoginName [viewstate] the password changed: the success template（期待される本文）</summary>

```text
Status: Logout Name: Hello, taro
Admins' view: taro
Log In
User Name:
Password:
Remember me next time.
Templated login:
The password changed.
Forgot Your Password?
Enter your User Name to receive your password.
User Name:
Create
User Name:
Password:
Confirm Password:
E-mail:
Security Question:
Security Answer:
Skip Navigation Links.
First
Second
Last
Done
[wizard header]
First:
Dynamic: Logout
Skip Navigation Links.
Dynamic first
Dynamic last
Request 2: IsPostBack=True User=taro
ChangingPassword taro
ChangedPassword taro valid with the new one=True
State users: taro
State wizSteps ActiveStepIndex=0 txtFirst=""
State wizDynamic ActiveStepIndex=0
State cuwCreate ActiveStepIndex=0
State User=taro admins=True
```

</details>

<details><summary>LoginName [viewstate] logged out by the status（期待される本文）</summary>

```text
Status: Login Name:
Anonymous view
Log In
User Name:
Password:
Remember me next time.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
Forgot Your Password?
Enter your User Name to receive your password.
User Name:
Create
User Name:
Password:
Confirm Password:
E-mail:
Security Question:
Security Answer:
Skip Navigation Links.
First
Second
Last
Done
[wizard header]
First:
Dynamic: Login
Skip Navigation Links.
Dynamic first
Dynamic last
Request 1: IsPostBack=False User=(anonymous)
State users: taro
State wizSteps ActiveStepIndex=0 txtFirst=""
State wizDynamic ActiveStepIndex=0
State cuwCreate ActiveStepIndex=0
State User=(anonymous)
```

</details>

<details><summary>LoginName [viewstate] a recovery for a user who does not exist（期待される本文）</summary>

```text
Status: Login Name:
Anonymous view
Log In
User Name:
Password:
Remember me next time.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
Forgot Your Password?
Enter your User Name to receive your password.
User Name:
We were unable to access your information. Please try again.
Create
User Name:
Password:
Confirm Password:
E-mail:
Security Question:
Security Answer:
Skip Navigation Links.
First
Second
Last
Done
[wizard header]
First:
Dynamic: Login
Skip Navigation Links.
Dynamic first
Dynamic last
Request 2: IsPostBack=True User=(anonymous)
VerifyingUser nobody
UserLookupError nobody
State users: taro
State wizSteps ActiveStepIndex=0 txtFirst=""
State wizDynamic ActiveStepIndex=0
State cuwCreate ActiveStepIndex=0
State User=(anonymous)
```

</details>

<details><summary>LoginName [viewstate] the user&#39;s question（期待される本文）</summary>

```text
Status: Login Name:
Anonymous view
Log In
User Name:
Password:
Remember me next time.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
Identity Confirmation
Answer the following question to receive your password.
User Name: taro
Question: color
Answer:
Create
User Name:
Password:
Confirm Password:
E-mail:
Security Question:
Security Answer:
Skip Navigation Links.
First
Second
Last
Done
[wizard header]
First:
Dynamic: Login
Skip Navigation Links.
Dynamic first
Dynamic last
Request 3: IsPostBack=True User=(anonymous)
VerifyingUser taro
State users: taro
State wizSteps ActiveStepIndex=0 txtFirst=""
State wizDynamic ActiveStepIndex=0
State cuwCreate ActiveStepIndex=0
State User=(anonymous)
```

</details>

<details><summary>LoginName [viewstate] the answer right: the password reset, its mail prepared (cancelled), the success template（期待される本文）</summary>

```text
Status: Login Name:
Anonymous view
Log In
User Name:
Password:
Remember me next time.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
The recovery mail prepared.
Create
User Name:
Password:
Confirm Password:
E-mail:
Security Question:
Security Answer:
Skip Navigation Links.
First
Second
Last
Done
[wizard header]
First:
Dynamic: Login
Skip Navigation Links.
Dynamic first
Dynamic last
Request 4: IsPostBack=True User=(anonymous)
VerifyingAnswer taro Question=color
SendingMail To=taro@example.com Subject=Your password Body=Please return to the site and log in using the following information. User Name: taro Password: NewPass1!
State users: taro
State wizSteps ActiveStepIndex=0 txtFirst=""
State wizDynamic ActiveStepIndex=0
State cuwCreate ActiveStepIndex=0
State User=(anonymous)
```

</details>

<details><summary>LoginName [viewstate] a user created: the complete step（期待される本文）</summary>

```text
Status: Login Name:
Anonymous view
Log In
User Name:
Password:
Remember me next time.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
The recovery mail prepared.
The user was created.
Skip Navigation Links.
First
Second
Last
Done
[wizard header]
First:
Dynamic: Login
Skip Navigation Links.
Dynamic first
Dynamic last
Request 5: IsPostBack=True User=(anonymous)
CreatingUser hanako
CreatedUser hanako exists=True
ActiveStepChanged cuwCreate ActiveStepIndex=1
State users: hanako, taro
State wizSteps ActiveStepIndex=0 txtFirst=""
State wizDynamic ActiveStepIndex=0
State cuwCreate ActiveStepIndex=1
State User=(anonymous)
```

</details>

<details><summary>LoginName [viewstate] the wizard&#39;s next step (the templated one)（期待される本文）</summary>

```text
Status: Login Name:
Anonymous view
Log In
User Name:
Password:
Remember me next time.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
The recovery mail prepared.
The user was created.
Skip Navigation Links.
First
Second
Last
Done
[wizard header]
Second (templated): bound
Dynamic: Login
Skip Navigation Links.
Dynamic first
Dynamic last
Request 6: IsPostBack=True User=(anonymous)
NextButtonClick 0 -> 1
Deactivate wsFirst
Activate twsSecond
ActiveStepChanged wizSteps ActiveStepIndex=1
State users: hanako, taro
State wizSteps ActiveStepIndex=1 txtFirst="first step"
State wizDynamic ActiveStepIndex=0
State cuwCreate ActiveStepIndex=1
State User=(anonymous)
```

</details>

<details><summary>LoginName [viewstate] next, then previous（期待される本文）</summary>

```text
Status: Login Name:
Anonymous view
Log In
User Name:
Password:
Remember me next time.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
The recovery mail prepared.
The user was created.
Skip Navigation Links.
First
Second
Last
Done
[wizard header]
Second (templated): bound
Dynamic: Login
Skip Navigation Links.
Dynamic first
Dynamic last
Request 8: IsPostBack=True User=(anonymous)
PreviousButtonClick 2 -> 1
Deactivate wsLast
Activate twsSecond
ActiveStepChanged wizSteps ActiveStepIndex=1
State users: hanako, taro
State wizSteps ActiveStepIndex=1 txtFirst="first step"
State wizDynamic ActiveStepIndex=0
State cuwCreate ActiveStepIndex=1
State User=(anonymous)
```

</details>

<details><summary>LoginName [viewstate] a step chosen in the side bar（期待される本文）</summary>

```text
Status: Login Name:
Anonymous view
Log In
User Name:
Password:
Remember me next time.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
The recovery mail prepared.
The user was created.
Skip Navigation Links.
First
Second
Last
Done
[wizard header]
Last: set in code
Dynamic: Login
Skip Navigation Links.
Dynamic first
Dynamic last
Request 9: IsPostBack=True User=(anonymous)
SideBarButtonClick 1 -> 2
Deactivate twsSecond
Activate wsLast
ActiveStepChanged wizSteps ActiveStepIndex=2
State users: hanako, taro
State wizSteps ActiveStepIndex=2 txtFirst="first step"
State wizDynamic ActiveStepIndex=0
State cuwCreate ActiveStepIndex=1
State User=(anonymous)
```

</details>

<details><summary>LoginName [viewstate] finished: the complete step, the first step&#39;s value kept（期待される本文）</summary>

```text
Status: Login Name:
Anonymous view
Log In
User Name:
Password:
Remember me next time.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
The recovery mail prepared.
The user was created.
Done.
Dynamic: Login
Skip Navigation Links.
Dynamic first
Dynamic last
Request 10: IsPostBack=True User=(anonymous)
FinishButtonClick 2 -> 3 First="first step"
Deactivate wsLast
Activate wsDone
ActiveStepChanged wizSteps ActiveStepIndex=3
State users: hanako, taro
State wizSteps ActiveStepIndex=3 txtFirst="first step"
State wizDynamic ActiveStepIndex=0
State cuwCreate ActiveStepIndex=1
State User=(anonymous)
```

</details>

<details><summary>LoginName [dynamic][viewstate] a wizard made in code: its next step（期待される本文）</summary>

```text
Status: Login Name:
Anonymous view
Log In
User Name:
Password:
Remember me next time.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
The recovery mail prepared.
The user was created.
Done.
Dynamic: Login
Skip Navigation Links.
Dynamic first
Dynamic last
Request 11: IsPostBack=True User=(anonymous)
ActiveStepChanged wizDynamic ActiveStepIndex=1
State users: hanako, taro
State wizSteps ActiveStepIndex=3 txtFirst="first step"
State wizDynamic ActiveStepIndex=1
State cuwCreate ActiveStepIndex=1
State User=(anonymous)
```

</details>

<details><summary>LoginName [visible][viewstate] a hidden wizard shown（期待される本文）</summary>

```text
Status: Login Name:
Anonymous view
Log In
User Name:
Password:
Remember me next time.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
The recovery mail prepared.
The user was created.
Done.
Skip Navigation Links.
Hidden
Hidden wizard
Dynamic: Login
Skip Navigation Links.
Dynamic first
Dynamic last
Request 12: IsPostBack=True User=(anonymous)
Click btnShow
State users: hanako, taro
State wizSteps ActiveStepIndex=3 txtFirst="first step"
State wizDynamic ActiveStepIndex=1
State cuwCreate ActiveStepIndex=1
State User=(anonymous)
```

</details>

