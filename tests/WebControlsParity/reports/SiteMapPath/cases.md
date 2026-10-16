# SiteMapPath テストケース一覧

変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。
このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。

- 正解データの採取: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0
- ① API 層: 182 ケース（191 メンバー）
- ② HTML 層: 171 ケース
- ③ シナリオ層: 12 スナップショット

## 機構マトリクス

型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。

| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |
|---|---|---|---|
| プロパティ → HTML 描画 | すべてのコントロール | 157 | 1 |
| ViewState の保存と復元 | Control | 6 | 11 |
| Visible=false（描画されず状態は保持） | Control.Visible | 2 | 1 |
| 名前付けコンテナと ClientIDMode | Control.ClientIDMode | 7 | 0 |
| テーマとスキン（SkinID / EnableTheming） | テーマを付けられる型（[Themeable]） | 4 | 0 |
| 動的に生成したコントロールのライフサイクル | Control | 0 | 1 |
| データバインド（DataBind / <%# %>） | Control.DataBind | 0 | 1 |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | WebControl.Enabled、HtmlControl.Disabled | 3 | 0 |
| テンプレート | ITemplate のプロパティ | 0 | 1 |

該当しない機構: ポストバックデータの取り込み、変更イベント（*Changed、ServerChange）、ReadOnly（ポストされた値を無視）、リクエスト検証（ValidateRequestMode）、ポストバックイベント、コマンドイベントと親へのバブル（CommandName）、送信ボタン／スクリプトによるポストバック（UseSubmitBehavior）、ブラウザ側のクリック処理（OnClientClick）、クロスページポストバック（PostBackUrl）、既定のボタン（Panel の DefaultButton）、AutoPostBack（クライアントスクリプトによるポストバック）、CausesValidation / ValidationGroup、検証対象（サーバー側の検証）、検証対象（ブラウザ側の検証）、双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値）、項目（ListItem の Selected・Enabled・Text・Value と ViewState）、複数選択、繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）、ポスト値のイベント検証（一覧にない値・無効な項目の拒否）、サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）、ブラウザ側の検証スクリプト（EnableClientScript）、検証結果のまとめ（ValidationSummary）、ページング、並べ替え、項目の挿入、行・項目の編集（編集・更新・取り消し・削除）、行・項目の選択、行・項目のコマンド（RowCommand・ItemCommand へのバブル）、データキー（DataKeyNames・DataKeys）、データソースコントロールとの連携（DataSourceID）、データソース連携（DataSource・DataTextField など）、クライアントコールバック

## ① API 層

SiteMapPath とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の SiteMapPath インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。

| ケース | 期待値（.NET Framework） |
|---|---|
| <code>Api.Control.#ctor</code> | <code>Control.#ctor = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMode=In…</code> |
| <code>Api.Control.Adapter</code> | <code>Control.Adapter = null</code> |
| <code>Api.Control.AddParsedSubObject</code> | <code>Control.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>Control.AddParsedSubObject(Object) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]…</code> |
| <code>Api.Control.AddedControl</code> | <code>Control.AddedControl(Control,Int32) = &quot;ok&quot;</code><br><code>Control.AddedControl(Control,Int32) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty…</code> |
| <code>Api.Control.AppRelativeTemplateSourceDirectory</code> | <code>Control.AppRelativeTemplateSourceDirectory = &quot;&quot;</code><br><code>Control.AppRelativeTemplateSourceDirectory set = &quot;-&gt; System.ArgumentException: The relative virtual path &#39;Abc 123&#39; is not allowed here.&quot;</code> |
| <code>Api.Control.ApplyStyleSheetSkin</code> | <code>Control.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>Control.ApplyStyleSheetSkin(Page) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty];…</code> |
| <code>Api.Control.BeginRenderTracing</code> | <code>Control.BeginRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.BeginRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.BeginRenderTracing(TextWriter,Object) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Co…</code> |
| <code>Api.Control.BindingContainer</code> | <code>Control.BindingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Clien…</code> |
| <code>Api.Control.BuildProfileTree</code> | <code>Control.BuildProfileTree(String,Boolean) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.ChildControlsCreated</code> | <code>Control.ChildControlsCreated = &quot;false&quot;</code><br><code>Control.ChildControlsCreated set = &quot;ok&quot;</code> |
| <code>Api.Control.ClearCachedClientID</code> | <code>Control.ClearCachedClientID = &quot;ok&quot;</code><br><code>Control.ClearCachedClientID after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Borde…</code> |
| <code>Api.Control.ClearChildControlState</code> | <code>Control.ClearChildControlState = &quot;ok&quot;</code><br><code>Control.ClearChildControlState after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bo…</code> |
| <code>Api.Control.ClearChildState</code> | <code>Control.ClearChildState = &quot;ok&quot;</code><br><code>Control.ClearChildState after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSty…</code> |
| <code>Api.Control.ClearChildViewState</code> | <code>Control.ClearChildViewState = &quot;ok&quot;</code><br><code>Control.ClearChildViewState after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Borde…</code> |
| <code>Api.Control.ClearEffectiveClientIDMode</code> | <code>Control.ClearEffectiveClientIDMode = &quot;ok&quot;</code><br><code>Control.ClearEffectiveClientIDMode after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]…</code> |
| <code>Api.Control.ClientID</code> | <code>Control.ClientID = &quot;smpPath&quot;</code> |
| <code>Api.Control.ClientIDMode</code> | <code>Control.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>Control.ClientIDMode set = &quot;ClientIDMode.AutoID&quot;</code> |
| <code>Api.Control.ClientIDSeparator</code> | <code>Control.ClientIDSeparator = &quot;&#39;_&#39;&quot;</code> |
| <code>Api.Control.Context</code> | <code>Control.Context = null</code> |
| <code>Api.Control.Controls</code> | <code>Control.Controls = ControlCollection []</code> |
| <code>Api.Control.CreateChildControls</code> | <code>Control.CreateChildControls = &quot;ok&quot;</code><br><code>Control.CreateChildControls after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Borde…</code> |
| <code>Api.Control.CreateControlCollection</code> | <code>Control.CreateControlCollection = ControlCollection []</code><br><code>Control.CreateControlCollection after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; B…</code> |
| <code>Api.Control.DataBind</code> | <code>Control.DataBind = &quot;ok&quot;</code><br><code>Control.DataBind after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotS…</code><br><code>Control.DataBind(Boolean) = &quot;ok&quot;</code><br><code>Control.DataBind(Boolean) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderS…</code> |
| <code>Api.Control.DataBindChildren</code> | <code>Control.DataBindChildren = &quot;ok&quot;</code><br><code>Control.DataBindChildren after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSt…</code> |
| <code>Api.Control.DataBinding</code> | <code>Control.DataBinding = &quot;added and removed&quot;</code> |
| <code>Api.Control.DataItemContainer</code> | <code>Control.DataItemContainer = null</code> |
| <code>Api.Control.DataKeysContainer</code> | <code>Control.DataKeysContainer = null</code> |
| <code>Api.Control.DesignMode</code> | <code>Control.DesignMode = &quot;false&quot;</code> |
| <code>Api.Control.Dispose</code> | <code>Control.Dispose = &quot;ok&quot;</code><br><code>Control.Dispose after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSe…</code> |
| <code>Api.Control.Disposed</code> | <code>Control.Disposed = &quot;added and removed&quot;</code> |
| <code>Api.Control.EnableTheming</code> | <code>Control.EnableTheming = &quot;true&quot;</code><br><code>Control.EnableTheming set = &quot;true&quot;</code> |
| <code>Api.Control.EnableViewState</code> | <code>Control.EnableViewState = &quot;true&quot;</code><br><code>Control.EnableViewState set = &quot;true&quot;</code> |
| <code>Api.Control.EndRenderTracing</code> | <code>Control.EndRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.EndRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.EndRenderTracing(TextWriter,Object) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Colo…</code> |
| <code>Api.Control.EnsureChildControls</code> | <code>Control.EnsureChildControls = &quot;ok&quot;</code><br><code>Control.EnsureChildControls after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Borde…</code> |
| <code>Api.Control.EnsureID</code> | <code>Control.EnsureID = &quot;ok&quot;</code><br><code>Control.EnsureID after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotS…</code> |
| <code>Api.Control.Events</code> | <code>Control.Events = &quot;System.ComponentModel.EventHandlerList&quot;</code> |
| <code>Api.Control.FindControl</code> | <code>Control.FindControl(String) = null</code><br><code>Control.FindControl(String) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Borde…</code><br><code>Control.FindControl(String,Int32) = null</code><br><code>Control.FindControl(String,Int32) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty];…</code> |
| <code>Api.Control.Focus</code> | <code>Control.Focus = &quot;-&gt; System.InvalidOperationException: A form tag with runat=server must exist on the Page to use SetFocus() or the Focus property.&quot;</code> |
| <code>Api.Control.GetDesignModeState</code> | <code>Control.GetDesignModeState = &quot;[]&quot;</code><br><code>Control.GetDesignModeState after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Border…</code> |
| <code>Api.Control.GetRouteUrl</code> | <code>Control.GetRouteUrl(Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.GetUniqueIDRelativeTo</code> | <code>Control.GetUniqueIDRelativeTo(Control) = &quot;smpPath&quot;</code><br><code>Control.GetUniqueIDRelativeTo(Control) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Em…</code> |
| <code>Api.Control.HasChildViewState</code> | <code>Control.HasChildViewState = &quot;false&quot;</code> |
| <code>Api.Control.HasControls</code> | <code>Control.HasControls = &quot;false&quot;</code><br><code>Control.HasControls after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=N…</code> |
| <code>Api.Control.HasEvents</code> | <code>Control.HasEvents = &quot;false&quot;</code><br><code>Control.HasEvents after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=Not…</code> |
| <code>Api.Control.ID</code> | <code>Control.ID = &quot;smpPath&quot;</code><br><code>Control.ID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.IdSeparator</code> | <code>Control.IdSeparator = &quot;&#39;$&#39;&quot;</code> |
| <code>Api.Control.Init</code> | <code>Control.Init = &quot;added and removed&quot;</code> |
| <code>Api.Control.IsChildControlStateCleared</code> | <code>Control.IsChildControlStateCleared = &quot;false&quot;</code> |
| <code>Api.Control.IsLiteralContent</code> | <code>Control.IsLiteralContent = &quot;false&quot;</code><br><code>Control.IsLiteralContent after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSt…</code> |
| <code>Api.Control.IsTrackingViewState</code> | <code>Control.IsTrackingViewState = &quot;false&quot;</code> |
| <code>Api.Control.IsViewStateEnabled</code> | <code>Control.IsViewStateEnabled = &quot;true&quot;</code> |
| <code>Api.Control.Load</code> | <code>Control.Load = &quot;added and removed&quot;</code> |
| <code>Api.Control.LoadControlState</code> | <code>Control.LoadControlState(Object) = &quot;ok&quot;</code><br><code>Control.LoadControlState(Object) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; …</code> |
| <code>Api.Control.LoadViewState</code> | <code>Control.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Object[]&#39;.&quot;</code> |
| <code>Api.Control.LoadViewStateByID</code> | <code>Control.LoadViewStateByID = &quot;false&quot;</code> |
| <code>Api.Control.MapPathSecure</code> | <code>Control.MapPathSecure(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.NamingContainer</code> | <code>Control.NamingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code> |
| <code>Api.Control.OnBubbleEvent</code> | <code>Control.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Control.OnBubbleEvent(Object,EventArgs) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [E…</code> |
| <code>Api.Control.OnDataBinding</code> | <code>Control.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnDataBinding(EventArgs) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; …</code> |
| <code>Api.Control.OnInit</code> | <code>Control.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnInit(EventArgs) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderS…</code> |
| <code>Api.Control.OnLoad</code> | <code>Control.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnLoad(EventArgs) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderS…</code> |
| <code>Api.Control.OnPreRender</code> | <code>Control.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnPreRender(EventArgs) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bo…</code> |
| <code>Api.Control.OnUnload</code> | <code>Control.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnUnload(EventArgs) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Borde…</code> |
| <code>Api.Control.OpenFile</code> | <code>Control.OpenFile(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.Page</code> | <code>Control.Page = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=&quot;&quot;; …</code><br><code>Control.Page set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=…</code> |
| <code>Api.Control.Parent</code> | <code>Control.Parent = HtmlForm#form1 {Action=&quot;&quot;; ClientID=&quot;form1&quot;; ClientIDMode=Inherit; DefaultButton=&quot;&quot;; DefaultFocus=&quot;&quot;; Disabled=false; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.PreRender</code> | <code>Control.PreRender = &quot;added and removed&quot;</code> |
| <code>Api.Control.RaiseBubbleEvent</code> | <code>Control.RaiseBubbleEvent(Object,EventArgs) = &quot;ok&quot;</code><br><code>Control.RaiseBubbleEvent(Object,EventArgs) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color…</code> |
| <code>Api.Control.RemovedControl</code> | <code>Control.RemovedControl(Control) = &quot;ok&quot;</code><br><code>Control.RemovedControl(Control) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; B…</code> |
| <code>Api.Control.Render</code> | <code>Control.Render(HtmlTextWriter) = &quot;-&gt; System.ArgumentNullException: Value cannot be null. [param basepath]&quot;</code> |
| <code>Api.Control.RenderChildren</code> | <code>Control.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderChildren(HtmlTextWriter) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Em…</code> |
| <code>Api.Control.RenderControl</code> | <code>Control.RenderControl(HtmlTextWriter) = &quot;-&gt; System.ArgumentNullException: Value cannot be null. [param basepath]&quot;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) = &quot;-&gt; System.ArgumentNullException: Value cannot be null. [param basepath]&quot;</code> |
| <code>Api.Control.RenderingCompatibility</code> | <code>Control.RenderingCompatibility = &quot;System.Version 4.0&quot;</code><br><code>Control.RenderingCompatibility set = &quot;System.Version 4.0&quot;</code> |
| <code>Api.Control.ResolveAdapter</code> | <code>Control.ResolveAdapter = null</code><br><code>Control.ResolveAdapter after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyl…</code> |
| <code>Api.Control.ResolveClientUrl</code> | <code>Control.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveClientUrl(String) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; …</code> |
| <code>Api.Control.ResolveUrl</code> | <code>Control.ResolveUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveUrl(String) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Border…</code> |
| <code>Api.Control.SaveControlState</code> | <code>Control.SaveControlState = null</code><br><code>Control.SaveControlState after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSt…</code> |
| <code>Api.Control.SaveViewState</code> | <code>Control.SaveViewState = null</code><br><code>Control.SaveViewState after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle…</code> |
| <code>Api.Control.SetDesignModeState</code> | <code>Control.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>Control.SetDesignModeState(IDictionary) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [E…</code> |
| <code>Api.Control.SetRenderMethodDelegate</code> | <code>Control.SetRenderMethodDelegate(RenderMethod) = &quot;ok&quot;</code><br><code>Control.SetRenderMethodDelegate(RenderMethod) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Co…</code> |
| <code>Api.Control.SetTraceData</code> | <code>Control.SetTraceData(Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty…</code><br><code>Control.SetTraceData(Object,Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object,Object) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color…</code> |
| <code>Api.Control.Site</code> | <code>Control.Site = null</code><br><code>Control.Site set = null</code> |
| <code>Api.Control.SkinID</code> | <code>Control.SkinID = &quot;&quot;</code><br><code>Control.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.TemplateControl</code> | <code>Control.TemplateControl = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code><br><code>Control.TemplateControl set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Cl…</code> |
| <code>Api.Control.TemplateSourceDirectory</code> | <code>Control.TemplateSourceDirectory = &quot;&quot;</code> |
| <code>Api.Control.TrackViewState</code> | <code>Control.TrackViewState = &quot;ok&quot;</code><br><code>Control.TrackViewState after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyl…</code> |
| <code>Api.Control.UniqueID</code> | <code>Control.UniqueID = &quot;smpPath&quot;</code> |
| <code>Api.Control.Unload</code> | <code>Control.Unload = &quot;added and removed&quot;</code> |
| <code>Api.Control.ValidateRequestMode</code> | <code>Control.ValidateRequestMode = &quot;ValidateRequestMode.Inherit&quot;</code><br><code>Control.ValidateRequestMode set = &quot;ValidateRequestMode.Disabled&quot;</code> |
| <code>Api.Control.ViewState</code> | <code>Control.ViewState = StateBag []</code> |
| <code>Api.Control.ViewStateIgnoresCase</code> | <code>Control.ViewStateIgnoresCase = &quot;false&quot;</code> |
| <code>Api.Control.ViewStateMode</code> | <code>Control.ViewStateMode = &quot;ViewStateMode.Inherit&quot;</code><br><code>Control.ViewStateMode set = &quot;ViewStateMode.Enabled&quot;</code> |
| <code>Api.Control.Visible</code> | <code>Control.Visible = &quot;true&quot;</code><br><code>Control.Visible set = &quot;true&quot;</code> |
| <code>Api.IAttributeAccessor.GetAttribute</code> | <code>IAttributeAccessor.GetAttribute(String) = null</code><br><code>IAttributeAccessor.GetAttribute(String) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [E…</code> |
| <code>Api.IAttributeAccessor.SetAttribute</code> | <code>IAttributeAccessor.SetAttribute(String,String) = &quot;ok&quot;</code><br><code>IAttributeAccessor.SetAttribute(String,String) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 C…</code> |
| <code>Api.IControlBuilderAccessor.ControlBuilder</code> | <code>IControlBuilderAccessor.ControlBuilder = null</code> |
| <code>Api.IControlDesignerAccessor.GetDesignModeState</code> | <code>IControlDesignerAccessor.GetDesignModeState = &quot;[]&quot;</code><br><code>IControlDesignerAccessor.GetDesignModeState after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Colo…</code> |
| <code>Api.IControlDesignerAccessor.SetDesignModeState</code> | <code>IControlDesignerAccessor.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetDesignModeState(IDictionary) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #…</code> |
| <code>Api.IControlDesignerAccessor.SetOwnerControl</code> | <code>IControlDesignerAccessor.SetOwnerControl(Control) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetOwnerControl(Control) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #0000000…</code> |
| <code>Api.IControlDesignerAccessor.UserData</code> | <code>IControlDesignerAccessor.UserData = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.DataBindings</code> | <code>IDataBindingsAccessor.DataBindings = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.HasDataBindings</code> | <code>IDataBindingsAccessor.HasDataBindings = &quot;false&quot;</code> |
| <code>Api.IExpressionsAccessor.Expressions</code> | <code>IExpressionsAccessor.Expressions = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.HasExpressions</code> | <code>IExpressionsAccessor.HasExpressions = &quot;false&quot;</code> |
| <code>Api.IParserAccessor.AddParsedSubObject</code> | <code>IParserAccessor.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>IParserAccessor.AddParsedSubObject(Object) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color…</code> |
| <code>Api.IUrlResolutionService.ResolveClientUrl</code> | <code>IUrlResolutionService.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>IUrlResolutionService.ResolveClientUrl(String) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 C…</code> |
| <code>Api.WebControls.CompositeControl.#ctor</code> | <code>CompositeControl.#ctor = &quot;-&gt; System.MemberAccessException: Cannot create an instance of System.Web.UI.WebControls.CompositeControl because it is an abstract cla…</code> |
| <code>Api.WebControls.CompositeControl.Controls</code> | <code>CompositeControl.Controls = ControlCollection []</code> |
| <code>Api.WebControls.CompositeControl.DataBind</code> | <code>CompositeControl.DataBind = &quot;ok&quot;</code><br><code>CompositeControl.DataBind after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderS…</code> |
| <code>Api.WebControls.CompositeControl.RecreateChildControls</code> | <code>CompositeControl.RecreateChildControls = &quot;ok&quot;</code><br><code>CompositeControl.RecreateChildControls after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Em…</code> |
| <code>Api.WebControls.CompositeControl.Render</code> | <code>CompositeControl.Render(HtmlTextWriter) = &quot;-&gt; System.ArgumentNullException: Value cannot be null. [param basepath]&quot;</code> |
| <code>Api.WebControls.CompositeControl.SupportsDisabledAttribute</code> | <code>CompositeControl.SupportsDisabledAttribute = &quot;false&quot;</code> |
| <code>Api.WebControls.ICompositeControlDesignerAccessor.RecreateChildControls</code> | <code>ICompositeControlDesignerAccessor.RecreateChildControls = &quot;ok&quot;</code><br><code>ICompositeControlDesignerAccessor.RecreateChildControls after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #0…</code> |
| <code>Api.WebControls.SiteMapPath.#ctor</code> | <code>SiteMapPath.#ctor = SiteMapPath# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWi…</code> |
| <code>Api.WebControls.SiteMapPath.CreateChildControls</code> | <code>SiteMapPath.CreateChildControls = &quot;ok&quot;</code><br><code>SiteMapPath.CreateChildControls after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; B…</code> |
| <code>Api.WebControls.SiteMapPath.CreateControlHierarchy</code> | <code>SiteMapPath.CreateControlHierarchy = &quot;ok&quot;</code><br><code>SiteMapPath.CreateControlHierarchy after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]…</code> |
| <code>Api.WebControls.SiteMapPath.CurrentNodeStyle</code> | <code>SiteMapPath.CurrentNodeStyle = Style {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; CssC…</code> |
| <code>Api.WebControls.SiteMapPath.CurrentNodeTemplate</code> | <code>SiteMapPath.CurrentNodeTemplate = null</code><br><code>SiteMapPath.CurrentNodeTemplate set = null</code> |
| <code>Api.WebControls.SiteMapPath.DataBind</code> | <code>SiteMapPath.DataBind = &quot;ok&quot;</code><br><code>SiteMapPath.DataBind after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=…</code> |
| <code>Api.WebControls.SiteMapPath.InitializeItem</code> | <code>SiteMapPath.InitializeItem(SiteMapNodeItem) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.WebControls.SiteMapPath.ItemCreated</code> | <code>SiteMapPath.ItemCreated = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.SiteMapPath.ItemDataBound</code> | <code>SiteMapPath.ItemDataBound = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.SiteMapPath.LoadViewState</code> | <code>SiteMapPath.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Object[]&#39;.&quot;</code> |
| <code>Api.WebControls.SiteMapPath.NodeStyle</code> | <code>SiteMapPath.NodeStyle = Style {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; CssClass=&quot;&quot;…</code> |
| <code>Api.WebControls.SiteMapPath.NodeTemplate</code> | <code>SiteMapPath.NodeTemplate = null</code><br><code>SiteMapPath.NodeTemplate set = null</code> |
| <code>Api.WebControls.SiteMapPath.OnDataBinding</code> | <code>SiteMapPath.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>SiteMapPath.OnDataBinding(EventArgs) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empt…</code> |
| <code>Api.WebControls.SiteMapPath.OnItemCreated</code> | <code>SiteMapPath.OnItemCreated(SiteMapNodeItemEventArgs) = &quot;ok&quot;</code><br><code>SiteMapPath.OnItemCreated(SiteMapNodeItemEventArgs) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000…</code> |
| <code>Api.WebControls.SiteMapPath.OnItemDataBound</code> | <code>SiteMapPath.OnItemDataBound(SiteMapNodeItemEventArgs) = &quot;ok&quot;</code><br><code>SiteMapPath.OnItemDataBound(SiteMapNodeItemEventArgs) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #000…</code> |
| <code>Api.WebControls.SiteMapPath.ParentLevelsDisplayed</code> | <code>SiteMapPath.ParentLevelsDisplayed = &quot;Int32 -1&quot;</code><br><code>SiteMapPath.ParentLevelsDisplayed set = &quot;Int32 1&quot;</code> |
| <code>Api.WebControls.SiteMapPath.PathDirection</code> | <code>SiteMapPath.PathDirection = &quot;PathDirection.RootToCurrent&quot;</code><br><code>SiteMapPath.PathDirection set = &quot;PathDirection.CurrentToRoot&quot;</code> |
| <code>Api.WebControls.SiteMapPath.PathSeparator</code> | <code>SiteMapPath.PathSeparator = &quot; &gt; &quot;</code><br><code>SiteMapPath.PathSeparator set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.SiteMapPath.PathSeparatorStyle</code> | <code>SiteMapPath.PathSeparatorStyle = Style {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; Cs…</code> |
| <code>Api.WebControls.SiteMapPath.PathSeparatorTemplate</code> | <code>SiteMapPath.PathSeparatorTemplate = null</code><br><code>SiteMapPath.PathSeparatorTemplate set = null</code> |
| <code>Api.WebControls.SiteMapPath.Provider</code> | <code>SiteMapPath.Provider = XmlSiteMapProvider {EnableLocalization=false; ResourceKey=null; SecurityTrimmingEnabled=false}</code><br><code>SiteMapPath.Provider set = XmlSiteMapProvider {EnableLocalization=false; ResourceKey=null; SecurityTrimmingEnabled=false}</code> |
| <code>Api.WebControls.SiteMapPath.Render</code> | <code>SiteMapPath.Render(HtmlTextWriter) = &quot;-&gt; System.ArgumentNullException: Value cannot be null. [param basepath]&quot;</code> |
| <code>Api.WebControls.SiteMapPath.RenderContents</code> | <code>SiteMapPath.RenderContents(HtmlTextWriter) = &quot;-&gt; System.ArgumentNullException: Value cannot be null. [param basepath]&quot;</code> |
| <code>Api.WebControls.SiteMapPath.RenderCurrentNodeAsLink</code> | <code>SiteMapPath.RenderCurrentNodeAsLink = &quot;false&quot;</code><br><code>SiteMapPath.RenderCurrentNodeAsLink set = &quot;true&quot;</code> |
| <code>Api.WebControls.SiteMapPath.RootNodeStyle</code> | <code>SiteMapPath.RootNodeStyle = Style {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; CssClas…</code> |
| <code>Api.WebControls.SiteMapPath.RootNodeTemplate</code> | <code>SiteMapPath.RootNodeTemplate = null</code><br><code>SiteMapPath.RootNodeTemplate set = null</code> |
| <code>Api.WebControls.SiteMapPath.SaveViewState</code> | <code>SiteMapPath.SaveViewState = null</code><br><code>SiteMapPath.SaveViewState after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderS…</code> |
| <code>Api.WebControls.SiteMapPath.ShowToolTips</code> | <code>SiteMapPath.ShowToolTips = &quot;true&quot;</code><br><code>SiteMapPath.ShowToolTips set = &quot;true&quot;</code> |
| <code>Api.WebControls.SiteMapPath.SiteMapProvider</code> | <code>SiteMapPath.SiteMapProvider = &quot;&quot;</code><br><code>SiteMapPath.SiteMapProvider set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.SiteMapPath.SkipLinkText</code> | <code>SiteMapPath.SkipLinkText = &quot;Skip Navigation Links&quot;</code><br><code>SiteMapPath.SkipLinkText set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.SiteMapPath.TrackViewState</code> | <code>SiteMapPath.TrackViewState = &quot;ok&quot;</code><br><code>SiteMapPath.TrackViewState after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Border…</code> |
| <code>Api.WebControls.WebControl.#ctor</code> | <code>WebControl.#ctor = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidt…</code><br><code>WebControl.#ctor(String) = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; Bo…</code><br><code>WebControl.#ctor(HtmlTextWriterTag) = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle…</code> |
| <code>Api.WebControls.WebControl.AccessKey</code> | <code>WebControl.AccessKey = &quot;&quot;</code><br><code>WebControl.AccessKey set = &quot;-&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]&quot;</code> |
| <code>Api.WebControls.WebControl.AddAttributesToRender</code> | <code>WebControl.AddAttributesToRender(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.AddAttributesToRender(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>WebControl.AddAttributesToRender(HtmlTextWriter) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000…</code> |
| <code>Api.WebControls.WebControl.ApplyStyle</code> | <code>WebControl.ApplyStyle(Style) = &quot;ok&quot;</code><br><code>WebControl.ApplyStyle(Style) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bord…</code> |
| <code>Api.WebControls.WebControl.Attributes</code> | <code>WebControl.Attributes = AttributeCollection []</code> |
| <code>Api.WebControls.WebControl.BackColor</code> | <code>WebControl.BackColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>WebControl.BackColor set = &quot;Color #FF3366CC Color [A=255, R=51, G=102, B=204]&quot;</code> |
| <code>Api.WebControls.WebControl.BorderColor</code> | <code>WebControl.BorderColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>WebControl.BorderColor set = &quot;Color #FF3366CC Color [A=255, R=51, G=102, B=204]&quot;</code> |
| <code>Api.WebControls.WebControl.BorderStyle</code> | <code>WebControl.BorderStyle = &quot;BorderStyle.NotSet&quot;</code><br><code>WebControl.BorderStyle set = &quot;BorderStyle.None&quot;</code> |
| <code>Api.WebControls.WebControl.BorderWidth</code> | <code>WebControl.BorderWidth = Unit </code><br><code>WebControl.BorderWidth set = Unit 120px</code> |
| <code>Api.WebControls.WebControl.ControlStyle</code> | <code>WebControl.ControlStyle = Style {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; CssClass=…</code> |
| <code>Api.WebControls.WebControl.ControlStyleCreated</code> | <code>WebControl.ControlStyleCreated = &quot;false&quot;</code> |
| <code>Api.WebControls.WebControl.CopyBaseAttributes</code> | <code>WebControl.CopyBaseAttributes(WebControl) = &quot;ok&quot;</code><br><code>WebControl.CopyBaseAttributes(WebControl) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color …</code> |
| <code>Api.WebControls.WebControl.CreateControlStyle</code> | <code>WebControl.CreateControlStyle = Style {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; Css…</code><br><code>WebControl.CreateControlStyle after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bor…</code> |
| <code>Api.WebControls.WebControl.CssClass</code> | <code>WebControl.CssClass = &quot;&quot;</code><br><code>WebControl.CssClass set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.DisabledCssClass</code> | <code>WebControl.DisabledCssClass = &quot;aspNetDisabled&quot;</code><br><code>WebControl.DisabledCssClass set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.EnableTheming</code> | <code>WebControl.EnableTheming = &quot;true&quot;</code><br><code>WebControl.EnableTheming set = &quot;true&quot;</code> |
| <code>Api.WebControls.WebControl.Enabled</code> | <code>WebControl.Enabled = &quot;true&quot;</code><br><code>WebControl.Enabled set = &quot;true&quot;</code> |
| <code>Api.WebControls.WebControl.Font</code> | <code>WebControl.Font = FontInfo {Bold=false; Italic=false; Name=&quot;&quot;; Overline=false; Size=; Strikeout=false; Underline=false}</code> |
| <code>Api.WebControls.WebControl.ForeColor</code> | <code>WebControl.ForeColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>WebControl.ForeColor set = &quot;Color #FF3366CC Color [A=255, R=51, G=102, B=204]&quot;</code> |
| <code>Api.WebControls.WebControl.HasAttributes</code> | <code>WebControl.HasAttributes = &quot;false&quot;</code> |
| <code>Api.WebControls.WebControl.Height</code> | <code>WebControl.Height = Unit </code><br><code>WebControl.Height set = Unit 120px</code> |
| <code>Api.WebControls.WebControl.IsEnabled</code> | <code>WebControl.IsEnabled = &quot;true&quot;</code> |
| <code>Api.WebControls.WebControl.LoadViewState</code> | <code>WebControl.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Object[]&#39;.&quot;</code> |
| <code>Api.WebControls.WebControl.MergeStyle</code> | <code>WebControl.MergeStyle(Style) = &quot;ok&quot;</code><br><code>WebControl.MergeStyle(Style) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bord…</code> |
| <code>Api.WebControls.WebControl.Render</code> | <code>WebControl.Render(HtmlTextWriter) = &quot;-&gt; System.ArgumentNullException: Value cannot be null. [param basepath]&quot;</code> |
| <code>Api.WebControls.WebControl.RenderBeginTag</code> | <code>WebControl.RenderBeginTag(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.RenderBeginTag(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;span id=&quot;smpPath&quot;&gt;</code><br><code>WebControl.RenderBeginTag(HtmlTextWriter) after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color …</code> |
| <code>Api.WebControls.WebControl.RenderContents</code> | <code>WebControl.RenderContents(HtmlTextWriter) = &quot;-&gt; System.ArgumentNullException: Value cannot be null. [param basepath]&quot;</code> |
| <code>Api.WebControls.WebControl.RenderEndTag</code> | <code>WebControl.RenderEndTag(HtmlTextWriter) = &quot;-&gt; System.InvalidOperationException: A PopEndTag was called without a corresponding PushEndTag.&quot;</code> |
| <code>Api.WebControls.WebControl.SaveViewState</code> | <code>WebControl.SaveViewState = null</code><br><code>WebControl.SaveViewState after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSt…</code> |
| <code>Api.WebControls.WebControl.SkinID</code> | <code>WebControl.SkinID = &quot;&quot;</code><br><code>WebControl.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.Style</code> | <code>WebControl.Style = CssStyleCollection {Count=Int32 0; Value=null}</code> |
| <code>Api.WebControls.WebControl.SupportsDisabledAttribute</code> | <code>WebControl.SupportsDisabledAttribute = &quot;false&quot;</code> |
| <code>Api.WebControls.WebControl.TabIndex</code> | <code>WebControl.TabIndex = &quot;Int16 0&quot;</code><br><code>WebControl.TabIndex set = &quot;Int16 1&quot;</code> |
| <code>Api.WebControls.WebControl.TagKey</code> | <code>WebControl.TagKey = &quot;HtmlTextWriterTag.Span&quot;</code> |
| <code>Api.WebControls.WebControl.TagName</code> | <code>WebControl.TagName = &quot;span&quot;</code> |
| <code>Api.WebControls.WebControl.ToolTip</code> | <code>WebControl.ToolTip = &quot;&quot;</code><br><code>WebControl.ToolTip set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.TrackViewState</code> | <code>WebControl.TrackViewState = &quot;ok&quot;</code><br><code>WebControl.TrackViewState after = SiteMapPath#smpPath {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderS…</code> |
| <code>Api.WebControls.WebControl.Width</code> | <code>WebControl.Width = Unit </code><br><code>WebControl.Width set = Unit 120px</code> |

## ② HTML 層

プローブサイトの RenderProbe.aspx で、1 ケース 1 リクエスト（ページライフサイクル全体）で描画したページ全体を比較します。下表はコンポーネント部分（div#probe）、設定時の例外、保存された ViewState の要約です。

| ケース | 設定 | 期待される描画 | 設定時の例外 |
|---|---|---|---|
| <code>.(default)</code> | <code>(nothing set)</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.AccessKey=text</code> | <code>c.AccessKey = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> | <code>c.AccessKey = &quot;probe&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=markup</code> | <code>c.AccessKey = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> | <code>c.AccessKey = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=japanese</code> | <code>c.AccessKey = &quot;日本語テキスト&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> | <code>c.AccessKey = &quot;日本語テキスト&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=empty</code> | <code>c.AccessKey = &quot;&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.AccessKey=k</code> | <code>c.AccessKey = &quot;k&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot; accesskey=&quot;k&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.AppRelativeTemplateSourceDirectory=text</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;probe&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=markup</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=japanese</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;日本語テキスト&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=empty</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.BackColor=Red</code> | <code>c.BackColor = Color.Red;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;background-color:Red;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.BackColor=#3366CC</code> | <code>c.BackColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;background-color:#3366CC;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.BackColor=Transparent</code> | <code>c.BackColor = Color.Transparent;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;background-color:Transparent;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.BorderColor=Red</code> | <code>c.BorderColor = Color.Red;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;border-color:Red;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.BorderColor=#3366CC</code> | <code>c.BorderColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;border-color:#3366CC;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.BorderColor=Transparent</code> | <code>c.BorderColor = Color.Transparent;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;border-color:Transparent;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.BorderStyle=NotSet</code> | <code>c.BorderStyle = BorderStyle.NotSet;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.BorderStyle=None</code> | <code>c.BorderStyle = BorderStyle.None;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;display:inline-block;border-style:None;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.BorderStyle=Dotted</code> | <code>c.BorderStyle = BorderStyle.Dotted;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;display:inline-block;border-style:Dotted;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.BorderStyle=Dashed</code> | <code>c.BorderStyle = BorderStyle.Dashed;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;display:inline-block;border-style:Dashed;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.BorderStyle=Solid</code> | <code>c.BorderStyle = BorderStyle.Solid;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;display:inline-block;border-style:Solid;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.BorderStyle=Double</code> | <code>c.BorderStyle = BorderStyle.Double;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;display:inline-block;border-style:Double;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.BorderStyle=Groove</code> | <code>c.BorderStyle = BorderStyle.Groove;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;display:inline-block;border-style:Groove;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.BorderStyle=Ridge</code> | <code>c.BorderStyle = BorderStyle.Ridge;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;display:inline-block;border-style:Ridge;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.BorderStyle=Inset</code> | <code>c.BorderStyle = BorderStyle.Inset;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;display:inline-block;border-style:Inset;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.BorderStyle=Outset</code> | <code>c.BorderStyle = BorderStyle.Outset;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;display:inline-block;border-style:Outset;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.BorderWidth=120px</code> | <code>c.BorderWidth = Unit.Pixel(120);</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;display:inline-block;border-width:120px;border-style:solid;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.BorderWidth=50%</code> | <code>c.BorderWidth = Unit.Percentage(50);</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> | <code>c.BorderWidth = Unit.Percentage(50); -&gt; System.ArgumentOutOfRangeException: BorderWidth must be a non-negative number and cannot be a percentage. [param value]</code> |
| <code>.BorderWidth=2em</code> | <code>c.BorderWidth = new Unit(2, UnitType.Em);</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;display:inline-block;border-width:2em;border-style:solid;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ClientIDMode=Inherit</code> | <code>c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ClientIDMode=AutoID</code> | <code>c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ClientIDMode=Predictable</code> | <code>c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ClientIDMode=Static</code> | <code>c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.CssClass=text</code> | <code>c.CssClass = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot; class=&quot;probe&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.CssClass=markup</code> | <code>c.CssClass = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot; class=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.CssClass=japanese</code> | <code>c.CssClass = &quot;日本語テキスト&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot; class=&quot;日本語テキスト&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.CssClass=empty</code> | <code>c.CssClass = &quot;&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.CurrentNodeStyle.BackColor=Red</code> | <code>c.CurrentNodeStyle.BackColor = Color.Red;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span style=&quot;background-color:Red;&quot;&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.CurrentNodeStyle.CssClass=text</code> | <code>c.CurrentNodeStyle.CssClass = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span class=&quot;probe&quot;&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.CurrentNodeStyle.Font.Bold=true</code> | <code>c.CurrentNodeStyle.Font.Bold = true;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span style=&quot;font-weight:bold;&quot;&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.CurrentNodeStyle.ForeColor=Red</code> | <code>c.CurrentNodeStyle.ForeColor = Color.Red;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span style=&quot;color:Red;&quot;&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.CurrentNodeStyle.Height=120px</code> | <code>c.CurrentNodeStyle.Height = Unit.Pixel(120);</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span style=&quot;display:inline-block;height:120px;&quot;&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.EnableTheming=true</code> | <code>c.EnableTheming = true;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.EnableTheming=false</code> | <code>c.EnableTheming = false;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.EnableViewState=true</code> | <code>c.EnableViewState = true;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.EnableViewState=false</code> | <code>c.EnableViewState = false;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Enabled=true</code> | <code>c.Enabled = true;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Enabled=false</code> | <code>c.Enabled = false;</code> | <code>&lt;span id=&quot;smpPath&quot; class=&quot;aspNetDisabled&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; class=&quot;aspNetDisabled&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Font.Bold=true</code> | <code>c.Font.Bold = true;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;font-weight:bold;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Font.Bold=false</code> | <code>c.Font.Bold = false;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;font-weight:normal;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Font.Italic=true</code> | <code>c.Font.Italic = true;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;font-style:italic;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Font.Italic=false</code> | <code>c.Font.Italic = false;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;font-style:normal;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Font.Name=text</code> | <code>c.Font.Name = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;font-family:probe;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Font.Name=markup</code> | <code>c.Font.Name = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;font-family:&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Font.Name=japanese</code> | <code>c.Font.Name = &quot;日本語テキスト&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;font-family:日本語テキスト;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Font.Name=empty</code> | <code>c.Font.Name = &quot;&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Font.Names=Arial,MS-Gothic</code> | <code>c.Font.Names = new[] { &quot;Arial&quot;, &quot;MS Gothic&quot; };</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;font-family:Arial,MS Gothic;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Font.Overline=true</code> | <code>c.Font.Overline = true;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;text-decoration: overline;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Font.Overline=false</code> | <code>c.Font.Overline = false;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;text-decoration:none;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Font.Size=12pt</code> | <code>c.Font.Size = FontUnit.Point(12);</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;font-size:12pt;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Font.Size=Large</code> | <code>c.Font.Size = FontUnit.Large;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;font-size:Large;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Font.Size=150%</code> | <code>c.Font.Size = new FontUnit(Unit.Percentage(150));</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;font-size:150%;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Font.Strikeout=true</code> | <code>c.Font.Strikeout = true;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;text-decoration: line-through;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Font.Strikeout=false</code> | <code>c.Font.Strikeout = false;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;text-decoration:none;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Font.Underline=true</code> | <code>c.Font.Underline = true;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;text-decoration:underline;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Font.Underline=false</code> | <code>c.Font.Underline = false;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;text-decoration:none;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ForeColor=Red</code> | <code>c.ForeColor = Color.Red;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;color:Red;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ForeColor=#3366CC</code> | <code>c.ForeColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;color:#3366CC;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ForeColor=Transparent</code> | <code>c.ForeColor = Color.Transparent;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;color:Transparent;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Height=120px</code> | <code>c.Height = Unit.Pixel(120);</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;display:inline-block;height:120px;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Height=50%</code> | <code>c.Height = Unit.Percentage(50);</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;display:inline-block;height:50%;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Height=2em</code> | <code>c.Height = new Unit(2, UnitType.Em);</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;display:inline-block;height:2em;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ID=text</code> | <code>c.ID = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;probe&quot;&gt;&lt;a href=&quot;#probe_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;probe_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ID=markup</code> | <code>c.ID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;&lt;a href=&quot;#&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ID=japanese</code> | <code>c.ID = &quot;日本語テキスト&quot;;</code> | <code>&lt;span id=&quot;日本語テキスト&quot;&gt;&lt;a href=&quot;#%e6%97%a5%e6%9c%ac%e8%aa%9e%e3%83%86%e3%82%ad%e3%82%b9%e3%83%88_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;日本語テキスト_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ID=empty</code> | <code>c.ID = &quot;&quot;;</code> | <code>&lt;span id=&quot;ctl02&quot;&gt;&lt;a href=&quot;#ctl02_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;ctl02_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.NodeStyle.BackColor=Red</code> | <code>c.NodeStyle.BackColor = Color.Red;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot; style=&quot;background-color:Red;&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span style=&quot;background-color:Red;&quot;&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.NodeStyle.CssClass=text</code> | <code>c.NodeStyle.CssClass = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; class=&quot;probe&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span class=&quot;probe&quot;&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.NodeStyle.Font.Bold=true</code> | <code>c.NodeStyle.Font.Bold = true;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot; style=&quot;font-weight:bold;&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span style=&quot;font-weight:bold;&quot;&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.NodeStyle.ForeColor=Red</code> | <code>c.NodeStyle.ForeColor = Color.Red;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot; style=&quot;color:Red;&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span style=&quot;color:Red;&quot;&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.NodeStyle.Height=120px</code> | <code>c.NodeStyle.Height = Unit.Pixel(120);</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot; style=&quot;display:inline-block;height:120px;&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span style=&quot;display:inline-block;height:120px;&quot;&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ParentLevelsDisplayed=0</code> | <code>c.ParentLevelsDisplayed = 0;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ParentLevelsDisplayed=3</code> | <code>c.ParentLevelsDisplayed = 3;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ParentLevelsDisplayed=-1</code> | <code>c.ParentLevelsDisplayed = -1;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ParentLevelsDisplayed=1</code> | <code>c.ParentLevelsDisplayed = 1;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.PathDirection=RootToCurrent</code> | <code>c.PathDirection = PathDirection.RootToCurrent;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.PathDirection=CurrentToRoot</code> | <code>c.PathDirection = PathDirection.CurrentToRoot;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.PathSeparator=text</code> | <code>c.PathSeparator = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt;probe&lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.PathSeparator=markup</code> | <code>c.PathSeparator = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.PathSeparator=japanese</code> | <code>c.PathSeparator = &quot;日本語テキスト&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt;日本語テキスト&lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.PathSeparator=empty</code> | <code>c.PathSeparator = &quot;&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt;&lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.PathSeparator=arrow</code> | <code>c.PathSeparator = &quot; &amp;gt; &quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &amp;gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.PathSeparatorStyle.BackColor=Red</code> | <code>c.PathSeparatorStyle.BackColor = Color.Red;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span style=&quot;background-color:Red;&quot;&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.PathSeparatorStyle.CssClass=text</code> | <code>c.PathSeparatorStyle.CssClass = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span class=&quot;probe&quot;&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.PathSeparatorStyle.Font.Bold=true</code> | <code>c.PathSeparatorStyle.Font.Bold = true;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span style=&quot;font-weight:bold;&quot;&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.PathSeparatorStyle.ForeColor=Red</code> | <code>c.PathSeparatorStyle.ForeColor = Color.Red;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span style=&quot;color:Red;&quot;&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.PathSeparatorStyle.Height=120px</code> | <code>c.PathSeparatorStyle.Height = Unit.Pixel(120);</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span style=&quot;display:inline-block;height:120px;&quot;&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.RenderCurrentNodeAsLink=true</code> | <code>c.RenderCurrentNodeAsLink = true;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;&lt;a title=&quot;One case&quot; href=&quot;/RenderProbe.aspx&quot;&gt;Render probe&lt;/a&gt;&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.RenderCurrentNodeAsLink=false</code> | <code>c.RenderCurrentNodeAsLink = false;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.RootNodeStyle.BackColor=Red</code> | <code>c.RootNodeStyle.BackColor = Color.Red;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot; style=&quot;background-color:Red;&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.RootNodeStyle.CssClass=text</code> | <code>c.RootNodeStyle.CssClass = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; class=&quot;probe&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.RootNodeStyle.Font.Bold=true</code> | <code>c.RootNodeStyle.Font.Bold = true;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot; style=&quot;font-weight:bold;&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.RootNodeStyle.ForeColor=Red</code> | <code>c.RootNodeStyle.ForeColor = Color.Red;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot; style=&quot;color:Red;&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.RootNodeStyle.Height=120px</code> | <code>c.RootNodeStyle.Height = Unit.Pixel(120);</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot; style=&quot;display:inline-block;height:120px;&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ShowToolTips=true</code> | <code>c.ShowToolTips = true;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ShowToolTips=false</code> | <code>c.ShowToolTips = false;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.SiteMapProvider=text</code> | <code>c.SiteMapProvider = &quot;probe&quot;;</code> | <code>status = 500</code> |  |
| <code>.SiteMapProvider=markup</code> | <code>c.SiteMapProvider = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>status = 500</code> |  |
| <code>.SiteMapProvider=japanese</code> | <code>c.SiteMapProvider = &quot;日本語テキスト&quot;;</code> | <code>status = 500</code> |  |
| <code>.SiteMapProvider=empty</code> | <code>c.SiteMapProvider = &quot;&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.SiteMapProvider=default</code> | <code>c.SiteMapProvider = &quot;AspNetXmlSiteMapProvider&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.SkinID=text</code> | <code>c.SkinID = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.SkinID=markup</code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.SkinID=japanese</code> | <code>c.SkinID = &quot;日本語テキスト&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.SkinID=empty</code> | <code>c.SkinID = &quot;&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.SkipLinkText=text</code> | <code>c.SkipLinkText = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;probe&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.SkipLinkText=markup</code> | <code>c.SkipLinkText = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.SkipLinkText=japanese</code> | <code>c.SkipLinkText = &quot;日本語テキスト&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;日本語テキスト&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.SkipLinkText=empty</code> | <code>c.SkipLinkText = &quot;&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;/span&gt;</code> |  |
| <code>.TabIndex=0</code> | <code>c.TabIndex = 0;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.TabIndex=3</code> | <code>c.TabIndex = 3;</code> | <code>&lt;span id=&quot;smpPath&quot; tabindex=&quot;3&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.TabIndex=-1</code> | <code>c.TabIndex = -1;</code> | <code>&lt;span id=&quot;smpPath&quot; tabindex=&quot;-1&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ToolTip=text</code> | <code>c.ToolTip = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot; title=&quot;probe&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ToolTip=markup</code> | <code>c.ToolTip = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot; title=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ToolTip=japanese</code> | <code>c.ToolTip = &quot;日本語テキスト&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot; title=&quot;日本語テキスト&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ToolTip=empty</code> | <code>c.ToolTip = &quot;&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ValidateRequestMode=Inherit</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Inherit;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ValidateRequestMode=Disabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Disabled;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ValidateRequestMode=Enabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Enabled;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ViewStateMode=Inherit</code> | <code>c.ViewStateMode = ViewStateMode.Inherit;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ViewStateMode=Enabled</code> | <code>c.ViewStateMode = ViewStateMode.Enabled;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ViewStateMode=Disabled</code> | <code>c.ViewStateMode = ViewStateMode.Disabled;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Visible=true</code> | <code>c.Visible = true;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Visible=false</code> | <code>c.Visible = false;</code> | <code></code> |  |
| <code>.Width=120px</code> | <code>c.Width = Unit.Pixel(120);</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;display:inline-block;width:120px;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Width=50%</code> | <code>c.Width = Unit.Percentage(50);</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;display:inline-block;width:50%;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Width=2em</code> | <code>c.Width = new Unit(2, UnitType.Em);</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;display:inline-block;width:2em;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Attributes[data-probe]=text</code> | <code>c.Attributes[data-probe] = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot; data-probe=&quot;probe&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Attributes[onfocus]=text</code> | <code>c.Attributes[onfocus] = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot; onfocus=&quot;probe&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Attributes[data-probe]=markup</code> | <code>c.Attributes[data-probe] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot; data-probe=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Attributes[onfocus]=markup</code> | <code>c.Attributes[onfocus] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot; onfocus=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Style[color]=red</code> | <code>c.Style[color] = &quot;red&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;color:red;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Style[margin-left]=red</code> | <code>c.Style[margin-left] = &quot;red&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;margin-left:red;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Style[color]=1em</code> | <code>c.Style[color] = &quot;1em&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;color:1em;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Style[margin-left]=1em</code> | <code>c.Style[margin-left] = &quot;1em&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;margin-left:1em;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.PathDirection=CurrentToRoot+PathSeparator=arrow</code> | <code>c.PathDirection = PathDirection.CurrentToRoot; c.PathSeparator = &quot; &amp;gt; &quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;span&gt; &amp;gt; &lt;/span&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.RenderCurrentNodeAsLink=true+ShowToolTips=false</code> | <code>c.RenderCurrentNodeAsLink = true; c.ShowToolTips = false;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;&lt;a href=&quot;/RenderProbe.aspx&quot;&gt;Render probe&lt;/a&gt;&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ParentLevelsDisplayed=0+PathSeparator=arrow</code> | <code>c.ParentLevelsDisplayed = 0; c.PathSeparator = &quot; &amp;gt; &quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ParentLevelsDisplayed=1+RenderCurrentNodeAsLink=true</code> | <code>c.ParentLevelsDisplayed = 1; c.RenderCurrentNodeAsLink = true;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;&lt;a title=&quot;One case&quot; href=&quot;/RenderProbe.aspx&quot;&gt;Render probe&lt;/a&gt;&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.SkipLinkText=empty+PathSeparator=arrow</code> | <code>c.SkipLinkText = &quot;&quot;; c.PathSeparator = &quot; &amp;gt; &quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &amp;gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;/span&gt;</code> |  |
| <code>.CurrentNodeStyle.ForeColor=Red+NodeStyle.BackColor=#3366CC</code> | <code>c.CurrentNodeStyle.ForeColor = Color.Red; c.NodeStyle.BackColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot; style=&quot;background-color:#3366CC;&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span style=&quot;color:Red;background-color:#3366CC;&quot;&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.RootNodeStyle.ForeColor=Red+PathSeparatorStyle.BackColor=#3366CC</code> | <code>c.RootNodeStyle.ForeColor = Color.Red; c.PathSeparatorStyle.BackColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot; style=&quot;color:Red;&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span style=&quot;background-color:#3366CC;&quot;&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Enabled=false+RenderCurrentNodeAsLink=true</code> | <code>c.Enabled = false; c.RenderCurrentNodeAsLink = true;</code> | <code>&lt;span id=&quot;smpPath&quot; class=&quot;aspNetDisabled&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; class=&quot;aspNetDisabled&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;&lt;a title=&quot;One case&quot; class=&quot;aspNetDisabled&quot;&gt;Render probe&lt;/a&gt;&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Enabled=false+CssClass=text</code> | <code>c.Enabled = false; c.CssClass = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot; class=&quot;aspNetDisabled probe&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; class=&quot;aspNetDisabled&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | <code>c.Width = Unit.Pixel(120); c.Height = new Unit(2, UnitType.Em); c.BorderWidth = Unit.Pixel(120); c.BorderStyle = BorderStyle.Solid; c.BackColor = Color.FromArgb(0x33, 0x66, 0xCC); c.ForeColor = Color.Red; c.Font.Bold = true;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;display:inline-block;color:Red;background-color:#3366CC;border-width:120px;border-style:Solid;font-weight:bold;height:2em;width:120px;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.ForeColor=#3366CC+Style[color]=red</code> | <code>c.ForeColor = Color.FromArgb(0x33, 0x66, 0xCC); c.Style[color] = &quot;red&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot; style=&quot;color:#3366CC;color:red;&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>.PathSeparator=arrow+Visible=false</code> | <code>c.PathSeparator = &quot; &amp;gt; &quot;; c.Visible = false;</code> | <code></code> |  |
| <code>.PathSeparator=arrow+EnableViewState=false</code> | <code>c.PathSeparator = &quot; &amp;gt; &quot;; c.EnableViewState = false;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &amp;gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>[naming container].(default)</code> | <code>[naming container] (nothing set)</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;span id=&quot;box_smpPath&quot;&gt;&lt;a href=&quot;#box_smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;box_smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=AutoID</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;span id=&quot;box_smpPath&quot;&gt;&lt;a href=&quot;#box_smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;box_smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Static</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Predictable</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;span id=&quot;box_smpPath&quot;&gt;&lt;a href=&quot;#box_smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;box_smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Inherit</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;span id=&quot;box_smpPath&quot;&gt;&lt;a href=&quot;#box_smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;box_smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ID=text</code> | <code>[naming container] c.ID = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;span id=&quot;box_probe&quot;&gt;&lt;a href=&quot;#box_probe_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;box_probe_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].RenderCurrentNodeAsLink=true</code> | <code>[naming container] c.RenderCurrentNodeAsLink = true;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;span id=&quot;box_smpPath&quot;&gt;&lt;a href=&quot;#box_smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;&lt;a title=&quot;One case&quot; href=&quot;/RenderProbe.aspx&quot;&gt;Render probe&lt;/a&gt;&lt;/span&gt;&lt;a id=&quot;box_smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>[themed].(default)</code> | <code>[themed] (nothing set)</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>[themed].SkinID=text</code> | <code>[themed] c.SkinID = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>[themed].EnableTheming=false</code> | <code>[themed] c.EnableTheming = false;</code> | <code>&lt;span id=&quot;smpPath&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>[themed].CssClass=text</code> | <code>[themed] c.CssClass = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;smpPath&quot; class=&quot;probe&quot;&gt;&lt;a href=&quot;#smpPath_SkipLink&quot; style=&quot;position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;&quot;&gt;Skip Navigation Links&lt;/a&gt;&lt;span&gt;&lt;a title=&quot;The cases&quot; href=&quot;/Cases.aspx&quot;&gt;Home&lt;/a&gt;&lt;/span&gt;&lt;span&gt; &gt; &lt;/span&gt;&lt;span&gt;Render probe&lt;/span&gt;&lt;a id=&quot;smpPath_SkipLink&quot;&gt;&lt;/a&gt;&lt;/span&gt;</code> |  |
| <code>[templates].(default)</code> | <code>[templates] (nothing set)</code> | <code>status = 500</code> |  |
| <code>[templates].PathDirection=CurrentToRoot</code> | <code>[templates] c.PathDirection = PathDirection.CurrentToRoot;</code> | <code>status = 500</code> |  |

## ③ シナリオ層

ブラウザ（Playwright）で SiteMapPathScenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。

| # | 操作 | 対象 | 値 |
|---|---|---|---|
| 1 | goto | /NavigationScenario.aspx | <code></code> |
| 2 | snapshot |  | **SiteMapPath [render][databind][dynamic][templates] first request: the site map as a path, trees and menus; bound to XML; templated; made in code** |
| 3 | evaluate | tvStatic | <code>Array.from(e.querySelectorAll(&#39;a&#39;)).find(a =&gt; a.href.indexOf(&quot;&#39;tf&#39;&quot;) &gt;= 0).click();</code> |
| 4 | waitms |  | <code></code> |
| 5 | snapshot |  | **SiteMapPath [viewstate] a node expanded by a postback (EnableClientScript=false)** |
| 6 | clicktextin | tvStatic | <code>Apple</code> |
| 7 | snapshot |  | **SiteMapPath [viewstate] a node selected (its value a control parameter&#39;s: ControlValueProperty)** |
| 8 | evaluate | tvStatic | <code>e.querySelectorAll(&#39;input[type=checkbox]&#39;)[2].checked = true;</code> |
| 9 | click | btnSubmit | <code></code> |
| 10 | snapshot |  | **SiteMapPath [viewstate] a node&#39;s check box posted** |
| 11 | evaluate | tvStatic | <code>Array.from(e.querySelectorAll(&#39;a&#39;)).find(a =&gt; a.href.indexOf(&quot;&#39;tv&#39;&quot;) &gt;= 0).click();</code> |
| 12 | waitms |  | <code></code> |
| 13 | snapshot |  | **SiteMapPath [viewstate] a node populated on demand by a postback** |
| 14 | evaluate | tvCallback | <code>Array.from(e.querySelectorAll(&#39;a&#39;)).find(a =&gt; a.href.indexOf(&#39;TreeView_PopulateNode&#39;) &gt;= 0).click();</code> |
| 15 | waitms |  | <code></code> |
| 16 | snapshot |  | **SiteMapPath [viewstate] a node populated on demand by a client callback (no postback)** |
| 17 | clicktextin | mnuStatic | <code>Help</code> |
| 18 | snapshot |  | **SiteMapPath [viewstate] a menu&#39;s item clicked** |
| 19 | clicktextin | mnuBound | <code>Bound two</code> |
| 20 | snapshot |  | **SiteMapPath [viewstate] a bound menu&#39;s item clicked** |
| 21 | clicktextin | mnuTemplated | <code>Bound child</code> |
| 22 | snapshot |  | **SiteMapPath [viewstate] a templated menu&#39;s item clicked** |
| 23 | clicktextin | tvDynamic | <code>Dynamic node</code> |
| 24 | snapshot |  | **SiteMapPath [viewstate] a dynamic tree&#39;s node selected** |
| 25 | clicktextin | mnuDynamic | <code>Dynamic child</code> |
| 26 | snapshot |  | **SiteMapPath [viewstate] a dynamic menu&#39;s item clicked** |
| 27 | click | btnShow | <code></code> |
| 28 | snapshot |  | **SiteMapPath [visible][viewstate] hidden ones shown with what was added while hidden** |

<details><summary>SiteMapPath [render][databind][dynamic][templates] first request: the site map as a path, trees and menus; bound to XML; templated; made in code（期待される本文）</summary>

```text
Path:
Skip Navigation Links
Home / Navigation
Templated path:
Skip Navigation Links
[node: Home] | [current: Navigation]
Skip Navigation Links.
Home
Render probe
Navigation
Skip Navigation Links.
Fruits
Vegetables
Plain
The tree's selected value (a control parameter)
Value
(null)
Skip Navigation Links.
On demand
Skip Navigation Links.
Bound one
Bound child
Bound two
Skip Navigation Links
Home
Render probe
Navigation
Skip Navigation Links
File
Open
Save
Help
Skip Navigation Links
Bound one
Bound child
Bound two
Skip Navigation Links
[item: Bound one]
[item: Bound child]
[item: Bound two]
Dynamic:
Skip Navigation Links.
Dynamic node
Dynamic leaf
Skip Navigation Links
Dynamic item
Dynamic child
Skip Navigation Links
Home > Navigation
Skip Navigation Links
Render probe
Navigation
Request 1: IsPostBack=False IsCallback=False
State tvSite Nodes=[]
State tvStatic Nodes=[f, f/apple, f/banana, v, plain]
State tvCallback Nodes=[d]
State tvBound Nodes=[]
State tvHidden Nodes=[h] (hidden)
State tvDynamic Nodes=[dn, dn/dl]
State mnuSite Items=[]
State mnuStatic Items=[file, file/open, file/save, help]
State mnuBound Items=[]
State mnuTemplated Items=[]
State mnuHidden Items=[h] (hidden)
State mnuDynamic Items=[di, di/dc]
State current node: Navigation /NavigationScenario.aspx
```

</details>

<details><summary>SiteMapPath [viewstate] a node expanded by a postback (EnableClientScript=false)（期待される本文）</summary>

```text
Path:
Skip Navigation Links
Home / Navigation
Templated path:
Skip Navigation Links
[node: Home] | [current: Navigation]
Skip Navigation Links.
Home
Render probe
Navigation
Skip Navigation Links.
Fruits
Apple
Banana
Vegetables
Plain
The tree's selected value (a control parameter)
Value
(null)
Skip Navigation Links.
On demand
Skip Navigation Links.
Bound one
Bound child
Bound two
Skip Navigation Links
Home
Render probe
Navigation
Skip Navigation Links
File
Open
Save
Help
Skip Navigation Links
Bound one
Bound child
Bound two
Skip Navigation Links
[item: Bound one]
[item: Bound child]
[item: Bound two]
Dynamic:
Skip Navigation Links.
Dynamic node
Dynamic leaf
Skip Navigation Links
Dynamic item
Dynamic child
Skip Navigation Links
Home > Navigation
Skip Navigation Links
Render probe
Navigation
Request 2: IsPostBack=True IsCallback=False
TreeNodeExpanded tvStatic f
State tvSite Nodes=[Home expanded, Home/Render probe, Home/Navigation selected]
State tvStatic Nodes=[f expanded, f/apple, f/banana, v, plain]
State tvCallback Nodes=[d]
State tvBound Nodes=[b1 expanded, b1/b1c, b2 expanded]
State tvHidden Nodes=[h] (hidden)
State tvDynamic Nodes=[dn expanded, dn/dl expanded]
State mnuSite Items=[Home, Home/Render probe, Home/Navigation selected, Home/Navigation/Target & more]
State mnuStatic Items=[file, file/open, file/save, help]
State mnuBound Items=[b1, b1/b1c, b2]
State mnuTemplated Items=[b1, b1/b1c, b2]
State mnuHidden Items=[h] (hidden)
State mnuDynamic Items=[di, di/dc]
State current node: Navigation /NavigationScenario.aspx
```

</details>

<details><summary>SiteMapPath [viewstate] a node selected (its value a control parameter&#39;s: ControlValueProperty)（期待される本文）</summary>

```text
Path:
Skip Navigation Links
Home / Navigation
Templated path:
Skip Navigation Links
[node: Home] | [current: Navigation]
Skip Navigation Links.
Home
Render probe
Navigation
Skip Navigation Links.
Fruits
Apple
Banana
Vegetables
Plain
The tree's selected value (a control parameter)
Value
apple
Skip Navigation Links.
On demand
Skip Navigation Links.
Bound one
Bound child
Bound two
Skip Navigation Links
Home
Render probe
Navigation
Skip Navigation Links
File
Open
Save
Help
Skip Navigation Links
Bound one
Bound child
Bound two
Skip Navigation Links
[item: Bound one]
[item: Bound child]
[item: Bound two]
Dynamic:
Skip Navigation Links.
Dynamic node
Dynamic leaf
Skip Navigation Links
Dynamic item
Dynamic child
Skip Navigation Links
Home > Navigation
Skip Navigation Links
Render probe
Navigation
Request 3: IsPostBack=True IsCallback=False
SelectedNodeChanged tvStatic f/apple
State tvSite Nodes=[Home expanded, Home/Render probe, Home/Navigation selected]
State tvStatic Nodes=[f expanded, f/apple selected, f/banana, v, plain]
State tvCallback Nodes=[d]
State tvBound Nodes=[b1 expanded, b1/b1c, b2 expanded]
State tvHidden Nodes=[h] (hidden)
State tvDynamic Nodes=[dn expanded, dn/dl expanded]
State mnuSite Items=[Home, Home/Render probe, Home/Navigation selected, Home/Navigation/Target & more]
State mnuStatic Items=[file, file/open, file/save, help]
State mnuBound Items=[b1, b1/b1c, b2]
State mnuTemplated Items=[b1, b1/b1c, b2]
State mnuHidden Items=[h] (hidden)
State mnuDynamic Items=[di, di/dc]
State current node: Navigation /NavigationScenario.aspx
```

</details>

<details><summary>SiteMapPath [viewstate] a node&#39;s check box posted（期待される本文）</summary>

```text
Path:
Skip Navigation Links
Home / Navigation
Templated path:
Skip Navigation Links
[node: Home] | [current: Navigation]
Skip Navigation Links.
Home
Render probe
Navigation
Skip Navigation Links.
Fruits
Apple
Banana
Vegetables
Plain
The tree's selected value (a control parameter)
Value
apple
Skip Navigation Links.
On demand
Skip Navigation Links.
Bound one
Bound child
Bound two
Skip Navigation Links
Home
Render probe
Navigation
Skip Navigation Links
File
Open
Save
Help
Skip Navigation Links
Bound one
Bound child
Bound two
Skip Navigation Links
[item: Bound one]
[item: Bound child]
[item: Bound two]
Dynamic:
Skip Navigation Links.
Dynamic node
Dynamic leaf
Skip Navigation Links
Dynamic item
Dynamic child
Skip Navigation Links
Home > Navigation
Skip Navigation Links
Render probe
Navigation
Request 4: IsPostBack=True IsCallback=False
TreeNodeCheckChanged tvStatic f/banana Checked=True
State tvSite Nodes=[Home expanded, Home/Render probe, Home/Navigation selected]
State tvStatic Nodes=[f expanded, f/apple selected, f/banana checked, v, plain]
State tvCallback Nodes=[d]
State tvBound Nodes=[b1 expanded, b1/b1c, b2 expanded]
State tvHidden Nodes=[h] (hidden)
State tvDynamic Nodes=[dn expanded, dn/dl expanded]
State mnuSite Items=[Home, Home/Render probe, Home/Navigation selected, Home/Navigation/Target & more]
State mnuStatic Items=[file, file/open, file/save, help]
State mnuBound Items=[b1, b1/b1c, b2]
State mnuTemplated Items=[b1, b1/b1c, b2]
State mnuHidden Items=[h] (hidden)
State mnuDynamic Items=[di, di/dc]
State current node: Navigation /NavigationScenario.aspx
```

</details>

<details><summary>SiteMapPath [viewstate] a node populated on demand by a postback（期待される本文）</summary>

```text
Path:
Skip Navigation Links
Home / Navigation
Templated path:
Skip Navigation Links
[node: Home] | [current: Navigation]
Skip Navigation Links.
Home
Render probe
Navigation
Skip Navigation Links.
Fruits
Apple
Banana
Vegetables
Populated by postback 1
Populated by postback 2
Plain
The tree's selected value (a control parameter)
Value
apple
Skip Navigation Links.
On demand
Skip Navigation Links.
Bound one
Bound child
Bound two
Skip Navigation Links
Home
Render probe
Navigation
Skip Navigation Links
File
Open
Save
Help
Skip Navigation Links
Bound one
Bound child
Bound two
Skip Navigation Links
[item: Bound one]
[item: Bound child]
[item: Bound two]
Dynamic:
Skip Navigation Links.
Dynamic node
Dynamic leaf
Skip Navigation Links
Dynamic item
Dynamic child
Skip Navigation Links
Home > Navigation
Skip Navigation Links
Render probe
Navigation
Request 5: IsPostBack=True IsCallback=False
TreeNodePopulate tvStatic v
TreeNodeExpanded tvStatic v
State tvSite Nodes=[Home expanded, Home/Render probe, Home/Navigation selected]
State tvStatic Nodes=[f expanded, f/apple selected, f/banana checked, v expanded, v/p1, v/p2, plain]
State tvCallback Nodes=[d]
State tvBound Nodes=[b1 expanded, b1/b1c, b2 expanded]
State tvHidden Nodes=[h] (hidden)
State tvDynamic Nodes=[dn expanded, dn/dl expanded]
State mnuSite Items=[Home, Home/Render probe, Home/Navigation selected, Home/Navigation/Target & more]
State mnuStatic Items=[file, file/open, file/save, help]
State mnuBound Items=[b1, b1/b1c, b2]
State mnuTemplated Items=[b1, b1/b1c, b2]
State mnuHidden Items=[h] (hidden)
State mnuDynamic Items=[di, di/dc]
State current node: Navigation /NavigationScenario.aspx
```

</details>

<details><summary>SiteMapPath [viewstate] a node populated on demand by a client callback (no postback)（期待される本文）</summary>

```text
Path:
Skip Navigation Links
Home / Navigation
Templated path:
Skip Navigation Links
[node: Home] | [current: Navigation]
Skip Navigation Links.
Home
Render probe
Navigation
Skip Navigation Links.
Fruits
Apple
Banana
Vegetables
Populated by postback 1
Populated by postback 2
Plain
The tree's selected value (a control parameter)
Value
apple
Skip Navigation Links.
On demand
Populated by callback 1
Populated by callback 2
Skip Navigation Links.
Bound one
Bound child
Bound two
Skip Navigation Links
Home
Render probe
Navigation
Skip Navigation Links
File
Open
Save
Help
Skip Navigation Links
Bound one
Bound child
Bound two
Skip Navigation Links
[item: Bound one]
[item: Bound child]
[item: Bound two]
Dynamic:
Skip Navigation Links.
Dynamic node
Dynamic leaf
Skip Navigation Links
Dynamic item
Dynamic child
Skip Navigation Links
Home > Navigation
Skip Navigation Links
Render probe
Navigation
Request 5: IsPostBack=True IsCallback=False
TreeNodePopulate tvStatic v
TreeNodeExpanded tvStatic v
State tvSite Nodes=[Home expanded, Home/Render probe, Home/Navigation selected]
State tvStatic Nodes=[f expanded, f/apple selected, f/banana checked, v expanded, v/p1, v/p2, plain]
State tvCallback Nodes=[d]
State tvBound Nodes=[b1 expanded, b1/b1c, b2 expanded]
State tvHidden Nodes=[h] (hidden)
State tvDynamic Nodes=[dn expanded, dn/dl expanded]
State mnuSite Items=[Home, Home/Render probe, Home/Navigation selected, Home/Navigation/Target & more]
State mnuStatic Items=[file, file/open, file/save, help]
State mnuBound Items=[b1, b1/b1c, b2]
State mnuTemplated Items=[b1, b1/b1c, b2]
State mnuHidden Items=[h] (hidden)
State mnuDynamic Items=[di, di/dc]
State current node: Navigation /NavigationScenario.aspx
```

</details>

<details><summary>SiteMapPath [viewstate] a menu&#39;s item clicked（期待される本文）</summary>

```text
Path:
Skip Navigation Links
Home / Navigation
Templated path:
Skip Navigation Links
[node: Home] | [current: Navigation]
Skip Navigation Links.
Home
Render probe
Navigation
Skip Navigation Links.
Fruits
Apple
Banana
Vegetables
Populated by postback 1
Populated by postback 2
Plain
The tree's selected value (a control parameter)
Value
apple
Skip Navigation Links.
On demand
Populated by postback 1
Populated by postback 2
Skip Navigation Links.
Bound one
Bound child
Bound two
Skip Navigation Links
Home
Render probe
Navigation
Skip Navigation Links
File
Open
Save
Help
Skip Navigation Links
Bound one
Bound child
Bound two
Skip Navigation Links
[item: Bound one]
[item: Bound child]
[item: Bound two]
Dynamic:
Skip Navigation Links.
Dynamic node
Dynamic leaf
Skip Navigation Links
Dynamic item
Dynamic child
Skip Navigation Links
Home > Navigation
Skip Navigation Links
Render probe
Navigation
TreeNodePopulate tvCallback d
Request 6: IsPostBack=True IsCallback=False
MenuItemClick mnuStatic help
State tvSite Nodes=[Home expanded, Home/Render probe, Home/Navigation selected]
State tvStatic Nodes=[f expanded, f/apple selected, f/banana checked, v expanded, v/p1, v/p2, plain]
State tvCallback Nodes=[d expanded, d/p1, d/p2]
State tvBound Nodes=[b1 expanded, b1/b1c, b2 expanded]
State tvHidden Nodes=[h] (hidden)
State tvDynamic Nodes=[dn expanded, dn/dl expanded]
State mnuSite Items=[Home, Home/Render probe, Home/Navigation selected, Home/Navigation/Target & more]
State mnuStatic Items=[file, file/open, file/save, help selected]
State mnuBound Items=[b1, b1/b1c, b2]
State mnuTemplated Items=[b1, b1/b1c, b2]
State mnuHidden Items=[h] (hidden)
State mnuDynamic Items=[di, di/dc]
State current node: Navigation /NavigationScenario.aspx
```

</details>

<details><summary>SiteMapPath [viewstate] a bound menu&#39;s item clicked（期待される本文）</summary>

```text
Path:
Skip Navigation Links
Home / Navigation
Templated path:
Skip Navigation Links
[node: Home] | [current: Navigation]
Skip Navigation Links.
Home
Render probe
Navigation
Skip Navigation Links.
Fruits
Apple
Banana
Vegetables
Populated by postback 1
Populated by postback 2
Plain
The tree's selected value (a control parameter)
Value
apple
Skip Navigation Links.
On demand
Populated by postback 1
Populated by postback 2
Skip Navigation Links.
Bound one
Bound child
Bound two
Skip Navigation Links
Home
Render probe
Navigation
Skip Navigation Links
File
Open
Save
Help
Skip Navigation Links
Bound one
Bound child
Bound two
Skip Navigation Links
[item: Bound one]
[item: Bound child]
[item: Bound two]
Dynamic:
Skip Navigation Links.
Dynamic node
Dynamic leaf
Skip Navigation Links
Dynamic item
Dynamic child
Skip Navigation Links
Home > Navigation
Skip Navigation Links
Render probe
Navigation
Request 7: IsPostBack=True IsCallback=False
MenuItemClick mnuBound b2
State tvSite Nodes=[Home expanded, Home/Render probe, Home/Navigation selected]
State tvStatic Nodes=[f expanded, f/apple selected, f/banana checked, v expanded, v/p1, v/p2, plain]
State tvCallback Nodes=[d expanded, d/p1, d/p2]
State tvBound Nodes=[b1 expanded, b1/b1c, b2 expanded]
State tvHidden Nodes=[h] (hidden)
State tvDynamic Nodes=[dn expanded, dn/dl expanded]
State mnuSite Items=[Home, Home/Render probe, Home/Navigation selected, Home/Navigation/Target & more]
State mnuStatic Items=[file, file/open, file/save, help selected]
State mnuBound Items=[b1, b1/b1c, b2 selected]
State mnuTemplated Items=[b1, b1/b1c, b2]
State mnuHidden Items=[h] (hidden)
State mnuDynamic Items=[di, di/dc]
State current node: Navigation /NavigationScenario.aspx
```

</details>

<details><summary>SiteMapPath [viewstate] a templated menu&#39;s item clicked（期待される本文）</summary>

```text
Path:
Skip Navigation Links
Home / Navigation
Templated path:
Skip Navigation Links
[node: Home] | [current: Navigation]
Skip Navigation Links.
Home
Render probe
Navigation
Skip Navigation Links.
Fruits
Apple
Banana
Vegetables
Populated by postback 1
Populated by postback 2
Plain
The tree's selected value (a control parameter)
Value
apple
Skip Navigation Links.
On demand
Populated by postback 1
Populated by postback 2
Skip Navigation Links.
Bound one
Bound child
Bound two
Skip Navigation Links
Home
Render probe
Navigation
Skip Navigation Links
File
Open
Save
Help
Skip Navigation Links
Bound one
Bound child
Bound two
Skip Navigation Links
[item: Bound one]
[item: Bound child]
[item: Bound two]
Dynamic:
Skip Navigation Links.
Dynamic node
Dynamic leaf
Skip Navigation Links
Dynamic item
Dynamic child
Skip Navigation Links
Home > Navigation
Skip Navigation Links
Render probe
Navigation
Request 8: IsPostBack=True IsCallback=False
MenuItemClick mnuTemplated b1/b1c
State tvSite Nodes=[Home expanded, Home/Render probe, Home/Navigation selected]
State tvStatic Nodes=[f expanded, f/apple selected, f/banana checked, v expanded, v/p1, v/p2, plain]
State tvCallback Nodes=[d expanded, d/p1, d/p2]
State tvBound Nodes=[b1 expanded, b1/b1c, b2 expanded]
State tvHidden Nodes=[h] (hidden)
State tvDynamic Nodes=[dn expanded, dn/dl expanded]
State mnuSite Items=[Home, Home/Render probe, Home/Navigation selected, Home/Navigation/Target & more]
State mnuStatic Items=[file, file/open, file/save, help selected]
State mnuBound Items=[b1, b1/b1c, b2 selected]
State mnuTemplated Items=[b1, b1/b1c selected, b2]
State mnuHidden Items=[h] (hidden)
State mnuDynamic Items=[di, di/dc]
State current node: Navigation /NavigationScenario.aspx
```

</details>

<details><summary>SiteMapPath [viewstate] a dynamic tree&#39;s node selected（期待される本文）</summary>

```text
Path:
Skip Navigation Links
Home / Navigation
Templated path:
Skip Navigation Links
[node: Home] | [current: Navigation]
Skip Navigation Links.
Home
Render probe
Navigation
Skip Navigation Links.
Fruits
Apple
Banana
Vegetables
Populated by postback 1
Populated by postback 2
Plain
The tree's selected value (a control parameter)
Value
apple
Skip Navigation Links.
On demand
Populated by postback 1
Populated by postback 2
Skip Navigation Links.
Bound one
Bound child
Bound two
Skip Navigation Links
Home
Render probe
Navigation
Skip Navigation Links
File
Open
Save
Help
Skip Navigation Links
Bound one
Bound child
Bound two
Skip Navigation Links
[item: Bound one]
[item: Bound child]
[item: Bound two]
Dynamic:
Skip Navigation Links.
Dynamic node
Dynamic leaf
Skip Navigation Links
Dynamic item
Dynamic child
Skip Navigation Links
Home > Navigation
Skip Navigation Links
Render probe
Navigation
Request 9: IsPostBack=True IsCallback=False
SelectedNodeChanged tvDynamic dn
State tvSite Nodes=[Home expanded, Home/Render probe, Home/Navigation selected]
State tvStatic Nodes=[f expanded, f/apple selected, f/banana checked, v expanded, v/p1, v/p2, plain]
State tvCallback Nodes=[d expanded, d/p1, d/p2]
State tvBound Nodes=[b1 expanded, b1/b1c, b2 expanded]
State tvHidden Nodes=[h] (hidden)
State tvDynamic Nodes=[dn expanded selected, dn/dl expanded]
State mnuSite Items=[Home, Home/Render probe, Home/Navigation selected, Home/Navigation/Target & more]
State mnuStatic Items=[file, file/open, file/save, help selected]
State mnuBound Items=[b1, b1/b1c, b2 selected]
State mnuTemplated Items=[b1, b1/b1c selected, b2]
State mnuHidden Items=[h] (hidden)
State mnuDynamic Items=[di, di/dc]
State current node: Navigation /NavigationScenario.aspx
```

</details>

<details><summary>SiteMapPath [viewstate] a dynamic menu&#39;s item clicked（期待される本文）</summary>

```text
Path:
Skip Navigation Links
Home / Navigation
Templated path:
Skip Navigation Links
[node: Home] | [current: Navigation]
Skip Navigation Links.
Home
Render probe
Navigation
Skip Navigation Links.
Fruits
Apple
Banana
Vegetables
Populated by postback 1
Populated by postback 2
Plain
The tree's selected value (a control parameter)
Value
apple
Skip Navigation Links.
On demand
Populated by postback 1
Populated by postback 2
Skip Navigation Links.
Bound one
Bound child
Bound two
Skip Navigation Links
Home
Render probe
Navigation
Skip Navigation Links
File
Open
Save
Help
Skip Navigation Links
Bound one
Bound child
Bound two
Skip Navigation Links
[item: Bound one]
[item: Bound child]
[item: Bound two]
Dynamic:
Skip Navigation Links.
Dynamic node
Dynamic leaf
Skip Navigation Links
Dynamic item
Dynamic child
Skip Navigation Links
Home > Navigation
Skip Navigation Links
Render probe
Navigation
Request 10: IsPostBack=True IsCallback=False
MenuItemClick mnuDynamic di/dc
State tvSite Nodes=[Home expanded, Home/Render probe, Home/Navigation selected]
State tvStatic Nodes=[f expanded, f/apple selected, f/banana checked, v expanded, v/p1, v/p2, plain]
State tvCallback Nodes=[d expanded, d/p1, d/p2]
State tvBound Nodes=[b1 expanded, b1/b1c, b2 expanded]
State tvHidden Nodes=[h] (hidden)
State tvDynamic Nodes=[dn expanded selected, dn/dl expanded]
State mnuSite Items=[Home, Home/Render probe, Home/Navigation selected, Home/Navigation/Target & more]
State mnuStatic Items=[file, file/open, file/save, help selected]
State mnuBound Items=[b1, b1/b1c, b2 selected]
State mnuTemplated Items=[b1, b1/b1c selected, b2]
State mnuHidden Items=[h] (hidden)
State mnuDynamic Items=[di, di/dc selected]
State current node: Navigation /NavigationScenario.aspx
```

</details>

<details><summary>SiteMapPath [visible][viewstate] hidden ones shown with what was added while hidden（期待される本文）</summary>

```text
Path:
Skip Navigation Links
Home / Navigation
Templated path:
Skip Navigation Links
[node: Home] | [current: Navigation]
Skip Navigation Links.
Home
Render probe
Navigation
Skip Navigation Links.
Fruits
Apple
Banana
Vegetables
Populated by postback 1
Populated by postback 2
Plain
The tree's selected value (a control parameter)
Value
apple
Skip Navigation Links.
On demand
Populated by postback 1
Populated by postback 2
Skip Navigation Links.
Bound one
Bound child
Bound two
Skip Navigation Links.
Added while hidden
Skip Navigation Links
Home
Render probe
Navigation
Skip Navigation Links
File
Open
Save
Help
Skip Navigation Links
Bound one
Bound child
Bound two
Skip Navigation Links
[item: Bound one]
[item: Bound child]
[item: Bound two]
Skip Navigation Links
Added while hidden
Dynamic:
Skip Navigation Links.
Dynamic node
Dynamic leaf
Skip Navigation Links
Dynamic item
Dynamic child
Skip Navigation Links
Home > Navigation
Skip Navigation Links
Render probe
Navigation
Request 11: IsPostBack=True IsCallback=False
Click btnShow
State tvSite Nodes=[Home expanded, Home/Render probe, Home/Navigation selected]
State tvStatic Nodes=[f expanded, f/apple selected, f/banana checked, v expanded, v/p1, v/p2, plain]
State tvCallback Nodes=[d expanded, d/p1, d/p2]
State tvBound Nodes=[b1 expanded, b1/b1c, b2 expanded]
State tvHidden Nodes=[h expanded]
State tvDynamic Nodes=[dn expanded selected, dn/dl expanded]
State mnuSite Items=[Home, Home/Render probe, Home/Navigation selected, Home/Navigation/Target & more]
State mnuStatic Items=[file, file/open, file/save, help selected]
State mnuBound Items=[b1, b1/b1c, b2 selected]
State mnuTemplated Items=[b1, b1/b1c selected, b2]
State mnuHidden Items=[h]
State mnuDynamic Items=[di, di/dc selected]
State current node: Navigation /NavigationScenario.aspx
```

</details>

