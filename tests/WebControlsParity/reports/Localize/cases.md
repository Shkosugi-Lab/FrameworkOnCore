# Localize テストケース一覧

変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。
このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。

- 正解データの採取: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0
- ① API 層: 113 ケース（120 メンバー）
- ② HTML 層: 47 ケース
- ③ シナリオ層: 14 スナップショット

## 機構マトリクス

型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。

| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |
|---|---|---|---|
| プロパティ → HTML 描画 | すべてのコントロール | 40 | 1 |
| ViewState の保存と復元 | Control | 6 | 13 |
| Visible=false（描画されず状態は保持） | Control.Visible | 2 | 1 |
| 名前付けコンテナと ClientIDMode | Control.ClientIDMode | 6 | 0 |
| 動的に生成したコントロールのライフサイクル | Control | 0 | 3 |
| データバインド（DataBind / <%# %>） | Control.DataBind | 0 | 1 |

該当しない機構: テーマとスキン（SkinID / EnableTheming）、Enabled=false・Disabled=true（描画とポスト値の扱い）、ポストバックデータの取り込み、変更イベント（*Changed、ServerChange）、ReadOnly（ポストされた値を無視）、リクエスト検証（ValidateRequestMode）、ポストバックイベント、コマンドイベントと親へのバブル（CommandName）、送信ボタン／スクリプトによるポストバック（UseSubmitBehavior）、ブラウザ側のクリック処理（OnClientClick）、クロスページポストバック（PostBackUrl）、既定のボタン（Panel の DefaultButton）、AutoPostBack（クライアントスクリプトによるポストバック）、CausesValidation / ValidationGroup、検証対象（サーバー側の検証）、検証対象（ブラウザ側の検証）、双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値）、項目（ListItem の Selected・Enabled・Text・Value と ViewState）、複数選択、繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）、ポスト値のイベント検証（一覧にない値・無効な項目の拒否）、サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）、ブラウザ側の検証スクリプト（EnableClientScript）、検証結果のまとめ（ValidationSummary）、ページング、並べ替え、項目の挿入、行・項目の編集（編集・更新・取り消し・削除）、行・項目の選択、行・項目のコマンド（RowCommand・ItemCommand へのバブル）、データキー（DataKeyNames・DataKeys）、データソースコントロールとの連携（DataSourceID）、テンプレート、データソース連携（DataSource・DataTextField など）、クライアントコールバック

## ① API 層

Localize とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の Localize インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。

| ケース | 期待値（.NET Framework） |
|---|---|
| <code>Api.Control.#ctor</code> | <code>Control.#ctor = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMode=In…</code> |
| <code>Api.Control.Adapter</code> | <code>Control.Adapter = null</code> |
| <code>Api.Control.AddParsedSubObject</code> | <code>Control.AddParsedSubObject(Object) = &quot;-&gt; System.Web.HttpException: &#39;Literal&#39; cannot have children of type &#39;String&#39;.&quot;</code> |
| <code>Api.Control.AddedControl</code> | <code>Control.AddedControl(Control,Int32) = &quot;ok&quot;</code><br><code>Control.AddedControl(Control,Int32) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; …</code> |
| <code>Api.Control.AppRelativeTemplateSourceDirectory</code> | <code>Control.AppRelativeTemplateSourceDirectory = &quot;&quot;</code><br><code>Control.AppRelativeTemplateSourceDirectory set = &quot;-&gt; System.ArgumentException: The relative virtual path &#39;Abc 123&#39; is not allowed here.&quot;</code> |
| <code>Api.Control.ApplyStyleSheetSkin</code> | <code>Control.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>Control.ApplyStyleSheetSkin(Page) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mo…</code> |
| <code>Api.Control.BeginRenderTracing</code> | <code>Control.BeginRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.BeginRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.BeginRenderTracing(TextWriter,Object) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;…</code> |
| <code>Api.Control.BindingContainer</code> | <code>Control.BindingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Clien…</code> |
| <code>Api.Control.BuildProfileTree</code> | <code>Control.BuildProfileTree(String,Boolean) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.ChildControlsCreated</code> | <code>Control.ChildControlsCreated = &quot;false&quot;</code><br><code>Control.ChildControlsCreated set = &quot;ok&quot;</code> |
| <code>Api.Control.ClearCachedClientID</code> | <code>Control.ClearCachedClientID = &quot;ok&quot;</code><br><code>Control.ClearCachedClientID after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode=Tra…</code> |
| <code>Api.Control.ClearChildControlState</code> | <code>Control.ClearChildControlState = &quot;ok&quot;</code><br><code>Control.ClearChildControlState after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode=…</code> |
| <code>Api.Control.ClearChildState</code> | <code>Control.ClearChildState = &quot;ok&quot;</code><br><code>Control.ClearChildState after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode=Transfo…</code> |
| <code>Api.Control.ClearChildViewState</code> | <code>Control.ClearChildViewState = &quot;ok&quot;</code><br><code>Control.ClearChildViewState after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode=Tra…</code> |
| <code>Api.Control.ClearEffectiveClientIDMode</code> | <code>Control.ClearEffectiveClientIDMode = &quot;ok&quot;</code><br><code>Control.ClearEffectiveClientIDMode after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; M…</code> |
| <code>Api.Control.ClientID</code> | <code>Control.ClientID = &quot;locText&quot;</code> |
| <code>Api.Control.ClientIDMode</code> | <code>Control.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>Control.ClientIDMode set = &quot;ClientIDMode.AutoID&quot;</code> |
| <code>Api.Control.ClientIDSeparator</code> | <code>Control.ClientIDSeparator = &quot;&#39;_&#39;&quot;</code> |
| <code>Api.Control.Context</code> | <code>Control.Context = null</code> |
| <code>Api.Control.Controls</code> | <code>Control.Controls = ControlCollection []</code> |
| <code>Api.Control.CreateChildControls</code> | <code>Control.CreateChildControls = &quot;ok&quot;</code><br><code>Control.CreateChildControls after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode=Tra…</code> |
| <code>Api.Control.CreateControlCollection</code> | <code>Control.CreateControlCollection = ControlCollection []</code><br><code>Control.CreateControlCollection after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode…</code> |
| <code>Api.Control.DataBind</code> | <code>Control.DataBind = &quot;ok&quot;</code><br><code>Control.DataBind after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode=Transform; Ski…</code><br><code>Control.DataBind(Boolean) = &quot;ok&quot;</code><br><code>Control.DataBind(Boolean) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode=Trans…</code> |
| <code>Api.Control.DataBindChildren</code> | <code>Control.DataBindChildren = &quot;ok&quot;</code><br><code>Control.DataBindChildren after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode=Transf…</code> |
| <code>Api.Control.DataBinding</code> | <code>Control.DataBinding = &quot;added and removed&quot;</code> |
| <code>Api.Control.DataItemContainer</code> | <code>Control.DataItemContainer = null</code> |
| <code>Api.Control.DataKeysContainer</code> | <code>Control.DataKeysContainer = null</code> |
| <code>Api.Control.DesignMode</code> | <code>Control.DesignMode = &quot;false&quot;</code> |
| <code>Api.Control.Dispose</code> | <code>Control.Dispose = &quot;ok&quot;</code><br><code>Control.Dispose after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode=Transform; Skin…</code> |
| <code>Api.Control.Disposed</code> | <code>Control.Disposed = &quot;added and removed&quot;</code> |
| <code>Api.Control.EnableTheming</code> | <code>Control.EnableTheming = &quot;true&quot;</code><br><code>Control.EnableTheming set = &quot;true&quot;</code> |
| <code>Api.Control.EnableViewState</code> | <code>Control.EnableViewState = &quot;true&quot;</code><br><code>Control.EnableViewState set = &quot;true&quot;</code> |
| <code>Api.Control.EndRenderTracing</code> | <code>Control.EndRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.EndRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.EndRenderTracing(TextWriter,Object) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lo…</code> |
| <code>Api.Control.EnsureChildControls</code> | <code>Control.EnsureChildControls = &quot;ok&quot;</code><br><code>Control.EnsureChildControls after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode=Tra…</code> |
| <code>Api.Control.EnsureID</code> | <code>Control.EnsureID = &quot;ok&quot;</code><br><code>Control.EnsureID after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode=Transform; Ski…</code> |
| <code>Api.Control.Events</code> | <code>Control.Events = &quot;System.ComponentModel.EventHandlerList&quot;</code> |
| <code>Api.Control.FindControl</code> | <code>Control.FindControl(String) = Label#lblTitle {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code><br><code>Control.FindControl(String) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode=Tra…</code><br><code>Control.FindControl(String,Int32) = null</code><br><code>Control.FindControl(String,Int32) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mo…</code> |
| <code>Api.Control.Focus</code> | <code>Control.Focus = &quot;-&gt; System.NotSupportedException: Control of type &#39;Localize&#39; does not support the Focus operation.&quot;</code> |
| <code>Api.Control.GetDesignModeState</code> | <code>Control.GetDesignModeState = &quot;[]&quot;</code><br><code>Control.GetDesignModeState after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode=Tran…</code> |
| <code>Api.Control.GetRouteUrl</code> | <code>Control.GetRouteUrl(Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.GetUniqueIDRelativeTo</code> | <code>Control.GetUniqueIDRelativeTo(Control) = &quot;locText&quot;</code><br><code>Control.GetUniqueIDRelativeTo(Control) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText…</code> |
| <code>Api.Control.HasChildViewState</code> | <code>Control.HasChildViewState = &quot;false&quot;</code> |
| <code>Api.Control.HasControls</code> | <code>Control.HasControls = &quot;false&quot;</code><br><code>Control.HasControls after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode=Transform; …</code> |
| <code>Api.Control.HasEvents</code> | <code>Control.HasEvents = &quot;false&quot;</code><br><code>Control.HasEvents after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode=Transform; Sk…</code> |
| <code>Api.Control.ID</code> | <code>Control.ID = &quot;locText&quot;</code><br><code>Control.ID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.IdSeparator</code> | <code>Control.IdSeparator = &quot;&#39;$&#39;&quot;</code> |
| <code>Api.Control.Init</code> | <code>Control.Init = &quot;added and removed&quot;</code> |
| <code>Api.Control.IsChildControlStateCleared</code> | <code>Control.IsChildControlStateCleared = &quot;false&quot;</code> |
| <code>Api.Control.IsLiteralContent</code> | <code>Control.IsLiteralContent = &quot;false&quot;</code><br><code>Control.IsLiteralContent after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode=Transf…</code> |
| <code>Api.Control.IsTrackingViewState</code> | <code>Control.IsTrackingViewState = &quot;false&quot;</code> |
| <code>Api.Control.IsViewStateEnabled</code> | <code>Control.IsViewStateEnabled = &quot;true&quot;</code> |
| <code>Api.Control.Load</code> | <code>Control.Load = &quot;added and removed&quot;</code> |
| <code>Api.Control.LoadControlState</code> | <code>Control.LoadControlState(Object) = &quot;ok&quot;</code><br><code>Control.LoadControlState(Object) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mod…</code> |
| <code>Api.Control.LoadViewState</code> | <code>Control.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Collections.ArrayList&#39;.&quot;</code> |
| <code>Api.Control.LoadViewStateByID</code> | <code>Control.LoadViewStateByID = &quot;false&quot;</code> |
| <code>Api.Control.MapPathSecure</code> | <code>Control.MapPathSecure(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.NamingContainer</code> | <code>Control.NamingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code> |
| <code>Api.Control.OnBubbleEvent</code> | <code>Control.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Control.OnBubbleEvent(Object,EventArgs) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locTex…</code> |
| <code>Api.Control.OnDataBinding</code> | <code>Control.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnDataBinding(EventArgs) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mod…</code> |
| <code>Api.Control.OnInit</code> | <code>Control.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnInit(EventArgs) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode=Trans…</code> |
| <code>Api.Control.OnLoad</code> | <code>Control.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnLoad(EventArgs) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode=Trans…</code> |
| <code>Api.Control.OnPreRender</code> | <code>Control.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnPreRender(EventArgs) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode=…</code> |
| <code>Api.Control.OnUnload</code> | <code>Control.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnUnload(EventArgs) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode=Tra…</code> |
| <code>Api.Control.OpenFile</code> | <code>Control.OpenFile(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.Page</code> | <code>Control.Page = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=&quot;&quot;; …</code><br><code>Control.Page set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=…</code> |
| <code>Api.Control.Parent</code> | <code>Control.Parent = HtmlForm#form1 {Action=&quot;&quot;; ClientID=&quot;form1&quot;; ClientIDMode=Inherit; DefaultButton=&quot;&quot;; DefaultFocus=&quot;&quot;; Disabled=false; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.PreRender</code> | <code>Control.PreRender = &quot;added and removed&quot;</code> |
| <code>Api.Control.RaiseBubbleEvent</code> | <code>Control.RaiseBubbleEvent(Object,EventArgs) = &quot;ok&quot;</code><br><code>Control.RaiseBubbleEvent(Object,EventArgs) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;loc…</code> |
| <code>Api.Control.RemovedControl</code> | <code>Control.RemovedControl(Control) = &quot;ok&quot;</code><br><code>Control.RemovedControl(Control) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode…</code> |
| <code>Api.Control.Render</code> | <code>Control.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.Render(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;b&gt;localized&lt;/b&gt;</code><br><code>Control.Render(HtmlTextWriter) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode=…</code> |
| <code>Api.Control.RenderChildren</code> | <code>Control.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderChildren(HtmlTextWriter) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText…</code> |
| <code>Api.Control.RenderControl</code> | <code>Control.RenderControl(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;b&gt;localized&lt;/b&gt;</code><br><code>Control.RenderControl(HtmlTextWriter) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;…</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) writer:</code><br><code>  &#124; &lt;b&gt;localized&lt;/b&gt;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableView…</code> |
| <code>Api.Control.RenderingCompatibility</code> | <code>Control.RenderingCompatibility = &quot;System.Version 4.0&quot;</code><br><code>Control.RenderingCompatibility set = &quot;System.Version 4.0&quot;</code> |
| <code>Api.Control.ResolveAdapter</code> | <code>Control.ResolveAdapter = null</code><br><code>Control.ResolveAdapter after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode=Transfor…</code> |
| <code>Api.Control.ResolveClientUrl</code> | <code>Control.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveClientUrl(String) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mod…</code> |
| <code>Api.Control.ResolveUrl</code> | <code>Control.ResolveUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveUrl(String) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode=Tran…</code> |
| <code>Api.Control.SaveControlState</code> | <code>Control.SaveControlState = null</code><br><code>Control.SaveControlState after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode=Transf…</code> |
| <code>Api.Control.SaveViewState</code> | <code>Control.SaveViewState = null</code><br><code>Control.SaveViewState after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode=Transform…</code> |
| <code>Api.Control.SetDesignModeState</code> | <code>Control.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>Control.SetDesignModeState(IDictionary) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locTex…</code> |
| <code>Api.Control.SetRenderMethodDelegate</code> | <code>Control.SetRenderMethodDelegate(RenderMethod) = &quot;ok&quot;</code><br><code>Control.SetRenderMethodDelegate(RenderMethod) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;…</code> |
| <code>Api.Control.SetTraceData</code> | <code>Control.SetTraceData(Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; …</code><br><code>Control.SetTraceData(Object,Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object,Object) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;loc…</code> |
| <code>Api.Control.Site</code> | <code>Control.Site = null</code><br><code>Control.Site set = null</code> |
| <code>Api.Control.SkinID</code> | <code>Control.SkinID = &quot;&quot;</code><br><code>Control.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.TemplateControl</code> | <code>Control.TemplateControl = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code><br><code>Control.TemplateControl set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Cl…</code> |
| <code>Api.Control.TemplateSourceDirectory</code> | <code>Control.TemplateSourceDirectory = &quot;&quot;</code> |
| <code>Api.Control.TrackViewState</code> | <code>Control.TrackViewState = &quot;ok&quot;</code><br><code>Control.TrackViewState after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode=Transfor…</code> |
| <code>Api.Control.UniqueID</code> | <code>Control.UniqueID = &quot;locText&quot;</code> |
| <code>Api.Control.Unload</code> | <code>Control.Unload = &quot;added and removed&quot;</code> |
| <code>Api.Control.ValidateRequestMode</code> | <code>Control.ValidateRequestMode = &quot;ValidateRequestMode.Inherit&quot;</code><br><code>Control.ValidateRequestMode set = &quot;ValidateRequestMode.Disabled&quot;</code> |
| <code>Api.Control.ViewState</code> | <code>Control.ViewState = StateBag [Text=&quot;&lt;b&gt;localized&lt;/b&gt;&quot;]</code> |
| <code>Api.Control.ViewStateIgnoresCase</code> | <code>Control.ViewStateIgnoresCase = &quot;false&quot;</code> |
| <code>Api.Control.ViewStateMode</code> | <code>Control.ViewStateMode = &quot;ViewStateMode.Inherit&quot;</code><br><code>Control.ViewStateMode set = &quot;ViewStateMode.Enabled&quot;</code> |
| <code>Api.Control.Visible</code> | <code>Control.Visible = &quot;true&quot;</code><br><code>Control.Visible set = &quot;true&quot;</code> |
| <code>Api.IControlBuilderAccessor.ControlBuilder</code> | <code>IControlBuilderAccessor.ControlBuilder = null</code> |
| <code>Api.IControlDesignerAccessor.GetDesignModeState</code> | <code>IControlDesignerAccessor.GetDesignModeState = &quot;[]&quot;</code><br><code>IControlDesignerAccessor.GetDesignModeState after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lo…</code> |
| <code>Api.IControlDesignerAccessor.SetDesignModeState</code> | <code>IControlDesignerAccessor.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetDesignModeState(IDictionary) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState…</code> |
| <code>Api.IControlDesignerAccessor.SetOwnerControl</code> | <code>IControlDesignerAccessor.SetOwnerControl(Control) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetOwnerControl(Control) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; …</code> |
| <code>Api.IControlDesignerAccessor.UserData</code> | <code>IControlDesignerAccessor.UserData = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.DataBindings</code> | <code>IDataBindingsAccessor.DataBindings = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.HasDataBindings</code> | <code>IDataBindingsAccessor.HasDataBindings = &quot;false&quot;</code> |
| <code>Api.IExpressionsAccessor.Expressions</code> | <code>IExpressionsAccessor.Expressions = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.HasExpressions</code> | <code>IExpressionsAccessor.HasExpressions = &quot;false&quot;</code> |
| <code>Api.IParserAccessor.AddParsedSubObject</code> | <code>IParserAccessor.AddParsedSubObject(Object) = &quot;-&gt; System.Web.HttpException: &#39;Literal&#39; cannot have children of type &#39;String&#39;.&quot;</code> |
| <code>Api.ITextControl.Text</code> | <code>ITextControl.Text = &quot;&lt;b&gt;localized&lt;/b&gt;&quot;</code><br><code>ITextControl.Text set = &quot;Abc 123&quot;</code> |
| <code>Api.IUrlResolutionService.ResolveClientUrl</code> | <code>IUrlResolutionService.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>IUrlResolutionService.ResolveClientUrl(String) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=…</code> |
| <code>Api.WebControls.Literal.#ctor</code> | <code>Literal.#ctor = Literal# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; Mode=Transform; SkinID=&quot;&quot;; Text=&quot;&quot;; UniqueID=nul…</code> |
| <code>Api.WebControls.Literal.AddParsedSubObject</code> | <code>Literal.AddParsedSubObject(Object) = &quot;-&gt; System.Web.HttpException: &#39;Literal&#39; cannot have children of type &#39;String&#39;.&quot;</code> |
| <code>Api.WebControls.Literal.CreateControlCollection</code> | <code>Literal.CreateControlCollection = ControlCollection []</code><br><code>Literal.CreateControlCollection after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode…</code> |
| <code>Api.WebControls.Literal.Focus</code> | <code>Literal.Focus = &quot;-&gt; System.NotSupportedException: Control of type &#39;Localize&#39; does not support the Focus operation.&quot;</code> |
| <code>Api.WebControls.Literal.Mode</code> | <code>Literal.Mode = &quot;LiteralMode.Transform&quot;</code><br><code>Literal.Mode set = &quot;LiteralMode.PassThrough&quot;</code> |
| <code>Api.WebControls.Literal.Render</code> | <code>Literal.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Literal.Render(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;b&gt;localized&lt;/b&gt;</code><br><code>Literal.Render(HtmlTextWriter) after = Localize#locText {ClientID=&quot;locText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;locText&quot;; Mode=…</code> |
| <code>Api.WebControls.Literal.Text</code> | <code>Literal.Text = &quot;&lt;b&gt;localized&lt;/b&gt;&quot;</code><br><code>Literal.Text set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.Localize.#ctor</code> | <code>Localize.#ctor = Localize# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; Mode=Transform; SkinID=&quot;&quot;; Text=&quot;&quot;; UniqueID=n…</code> |

## ② HTML 層

プローブサイトの RenderProbe.aspx で、1 ケース 1 リクエスト（ページライフサイクル全体）で描画したページ全体を比較します。下表はコンポーネント部分（div#probe）、設定時の例外、保存された ViewState の要約です。

| ケース | 設定 | 期待される描画 | 設定時の例外 |
|---|---|---|---|
| <code>.(default)</code> | <code>(nothing set)</code> | <code></code> |  |
| <code>.AppRelativeTemplateSourceDirectory=text</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;;</code> | <code></code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;probe&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=markup</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=japanese</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;;</code> | <code></code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;日本語テキスト&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=empty</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&quot;;</code> | <code></code> |  |
| <code>.ClientIDMode=Inherit</code> | <code>c.ClientIDMode = ClientIDMode.Inherit;</code> | <code></code> |  |
| <code>.ClientIDMode=AutoID</code> | <code>c.ClientIDMode = ClientIDMode.AutoID;</code> | <code></code> |  |
| <code>.ClientIDMode=Predictable</code> | <code>c.ClientIDMode = ClientIDMode.Predictable;</code> | <code></code> |  |
| <code>.ClientIDMode=Static</code> | <code>c.ClientIDMode = ClientIDMode.Static;</code> | <code></code> |  |
| <code>.EnableTheming=true</code> | <code>c.EnableTheming = true;</code> | <code></code> |  |
| <code>.EnableTheming=false</code> | <code>c.EnableTheming = false;</code> | <code></code> |  |
| <code>.EnableViewState=true</code> | <code>c.EnableViewState = true;</code> | <code></code> |  |
| <code>.EnableViewState=false</code> | <code>c.EnableViewState = false;</code> | <code></code> |  |
| <code>.ID=text</code> | <code>c.ID = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.ID=markup</code> | <code>c.ID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.ID=japanese</code> | <code>c.ID = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.ID=empty</code> | <code>c.ID = &quot;&quot;;</code> | <code></code> |  |
| <code>.Mode=Transform</code> | <code>c.Mode = LiteralMode.Transform;</code> | <code></code> |  |
| <code>.Mode=PassThrough</code> | <code>c.Mode = LiteralMode.PassThrough;</code> | <code></code> |  |
| <code>.Mode=Encode</code> | <code>c.Mode = LiteralMode.Encode;</code> | <code></code> |  |
| <code>.SkinID=text</code> | <code>c.SkinID = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.SkinID=markup</code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.SkinID=japanese</code> | <code>c.SkinID = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.SkinID=empty</code> | <code>c.SkinID = &quot;&quot;;</code> | <code></code> |  |
| <code>.Text=text</code> | <code>c.Text = &quot;probe&quot;;</code> | <code>probe</code> |  |
| <code>.Text=markup</code> | <code>c.Text = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;</code> |  |
| <code>.Text=japanese</code> | <code>c.Text = &quot;日本語テキスト&quot;;</code> | <code>日本語テキスト</code> |  |
| <code>.Text=empty</code> | <code>c.Text = &quot;&quot;;</code> | <code></code> |  |
| <code>.ValidateRequestMode=Inherit</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Inherit;</code> | <code></code> |  |
| <code>.ValidateRequestMode=Disabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Disabled;</code> | <code></code> |  |
| <code>.ValidateRequestMode=Enabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Enabled;</code> | <code></code> |  |
| <code>.ViewStateMode=Inherit</code> | <code>c.ViewStateMode = ViewStateMode.Inherit;</code> | <code></code> |  |
| <code>.ViewStateMode=Enabled</code> | <code>c.ViewStateMode = ViewStateMode.Enabled;</code> | <code></code> |  |
| <code>.ViewStateMode=Disabled</code> | <code>c.ViewStateMode = ViewStateMode.Disabled;</code> | <code></code> |  |
| <code>.Visible=true</code> | <code>c.Visible = true;</code> | <code></code> |  |
| <code>.Visible=false</code> | <code>c.Visible = false;</code> | <code></code> |  |
| <code>.Mode=Encode+Text=markup</code> | <code>c.Mode = LiteralMode.Encode; c.Text = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;</code> |  |
| <code>.Mode=PassThrough+Text=markup</code> | <code>c.Mode = LiteralMode.PassThrough; c.Text = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;</code> |  |
| <code>.Mode=Transform+Text=markup</code> | <code>c.Mode = LiteralMode.Transform; c.Text = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;</code> |  |
| <code>.Text=text+EnableViewState=false</code> | <code>c.Text = &quot;probe&quot;; c.EnableViewState = false;</code> | <code>probe</code> |  |
| <code>.Text=text+Visible=false</code> | <code>c.Text = &quot;probe&quot;; c.Visible = false;</code> | <code></code> |  |
| <code>[naming container].(default)</code> | <code>[naming container] (nothing set)</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=AutoID</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Static</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Predictable</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Inherit</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ID=text</code> | <code>[naming container] c.ID = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |

## ③ シナリオ層

ブラウザ（Playwright）で LocalizeScenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。

| # | 操作 | 対象 | 値 |
|---|---|---|---|
| 1 | goto | /MoreScenario.aspx | <code></code> |
| 2 | snapshot |  | **Localize [render][databind][dynamic] first request: plain, bound, set in code, made in code** |
| 3 | upload | fupPlain | <code>probe.txt:file content</code> |
| 4 | upload | fupMulti | <code>multi.txt:multi content</code> |
| 5 | upload | fupDynamic | <code>dynamic.txt:dynamic content</code> |
| 6 | fill | txtInFirst | <code>in the first view</code> |
| 7 | click | btnSubmit | <code></code> |
| 8 | snapshot |  | **Localize [viewstate][dynamic] posted: files, view state, what code set (a table&#39;s rows and a literal control&#39;s text are not kept)** |
| 9 | click | btnNext | <code></code> |
| 10 | snapshot |  | **Localize [viewstate] the next view (NextView bubbled from a button)** |
| 11 | click | btnToThird | <code></code> |
| 12 | click | btnToFirst | <code></code> |
| 13 | snapshot |  | **Localize [viewstate] switched by ID and by index: the first view&#39;s text box kept its value** |
| 14 | click | btnDynamicOne | <code></code> |
| 15 | snapshot |  | **Localize [dynamic][viewstate] a dynamic multi-view&#39;s next view** |
| 16 | clicktextin | calPlain | <code>20</code> |
| 17 | snapshot |  | **Localize [viewstate] a calendar&#39;s day chosen** |
| 18 | clicktextin | calPlain | <code>Week</code> |
| 19 | snapshot |  | **Localize [viewstate] a calendar&#39;s week chosen (several dates)** |
| 20 | clicktextin | calPlain | <code>&gt;</code> |
| 21 | snapshot |  | **Localize [viewstate] a calendar&#39;s next month** |
| 22 | clicktextin | calDynamic | <code>21</code> |
| 23 | snapshot |  | **Localize [viewstate] a dynamic calendar&#39;s day chosen** |
| 24 | evaluate | imapPlain | <code>document.getElementById(&#39;ImageMap&#39; + e.id).querySelector(&#39;area&#39;).click();</code> |
| 25 | waitms |  | <code></code> |
| 26 | snapshot |  | **Localize [viewstate] an image map&#39;s hot spot clicked (PostBack)** |
| 27 | evaluate | imapDynamic | <code>document.getElementById(&#39;ImageMap&#39; + e.id).querySelector(&#39;area&#39;).click();</code> |
| 28 | waitms |  | <code></code> |
| 29 | snapshot |  | **Localize [viewstate] a dynamic image map&#39;s hot spot clicked** |
| 30 | click | btnShow | <code></code> |
| 31 | snapshot |  | **Localize [visible][viewstate] hidden ones shown with what was set while hidden** |
| 32 | click | btnUploadValidate | <code></code> |
| 33 | snapshot |  | **Localize [viewstate] a file upload validated on the server (g1): no file posted** |
| 34 | evaluate | tblPlain | <code>__doPostBack(e.id, &#39;probe&#39;);</code> |
| 35 | waitms |  | <code></code> |
| 36 | snapshot |  | **Localize [viewstate] a postback event to a table (not registered for event validation): rejected** |

<details><summary>Localize [render][databind][dynamic] first request: plain, bound, set in code, made in code（期待される本文）</summary>

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

<details><summary>Localize [viewstate][dynamic] posted: files, view state, what code set (a table&#39;s rows and a literal control&#39;s text are not kept)（期待される本文）</summary>

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

<details><summary>Localize [viewstate] the next view (NextView bubbled from a button)（期待される本文）</summary>

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

<details><summary>Localize [viewstate] switched by ID and by index: the first view&#39;s text box kept its value（期待される本文）</summary>

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

<details><summary>Localize [dynamic][viewstate] a dynamic multi-view&#39;s next view（期待される本文）</summary>

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

<details><summary>Localize [viewstate] a calendar&#39;s day chosen（期待される本文）</summary>

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

<details><summary>Localize [viewstate] a calendar&#39;s week chosen（期待される本文）</summary>

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

<details><summary>Localize [viewstate] a calendar&#39;s next month（期待される本文）</summary>

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

<details><summary>Localize [viewstate] a dynamic calendar&#39;s day chosen（期待される本文）</summary>

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

<details><summary>Localize [viewstate] an image map&#39;s hot spot clicked (PostBack)（期待される本文）</summary>

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

<details><summary>Localize [viewstate] a dynamic image map&#39;s hot spot clicked（期待される本文）</summary>

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

<details><summary>Localize [visible][viewstate] hidden ones shown with what was set while hidden（期待される本文）</summary>

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

<details><summary>Localize [viewstate] a file upload validated on the server (g1): no file posted（期待される本文）</summary>

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

<details><summary>Localize [viewstate] a postback event to a table (not registered for event validation): rejected（期待される本文）</summary>

```text
System.ArgumentException
Invalid postback or callback argument. Event validation is enabled using <pages enableEventValidation="true"/> in configuration or <%@ Page EnableEventValidation="true" %> in a page. For security purposes, this feature verifies that arguments to postback or callback events originate from the server control that originally rendered them. If the data is valid and expected, use the ClientScriptManager.RegisterForEventValidation method in order to register the postback or callback data for validation.
```

</details>

