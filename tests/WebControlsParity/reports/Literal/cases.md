# Literal テストケース一覧

変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。
このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。

- 正解データの採取: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0
- ① API 層: 112 ケース（119 メンバー）
- ② HTML 層: 48 ケース
- ③ シナリオ層: 3 スナップショット

## 機構マトリクス

型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。

| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |
|---|---|---|---|
| プロパティ → HTML 描画 | すべてのコントロール | 41 | 1 |
| ViewState の保存と復元 | Control | 6 | 2 |
| Visible=false（描画されず状態は保持） | Control.Visible | 2 | 1 |
| 名前付けコンテナと ClientIDMode | Control.ClientIDMode | 6 | 0 |
| 動的に生成したコントロールのライフサイクル | Control | 0 | 2 |
| データバインド（DataBind / <%# %>） | Control.DataBind | 0 | 1 |

該当しない機構: テーマとスキン（SkinID / EnableTheming）、Enabled=false・Disabled=true（描画とポスト値の扱い）、ポストバックデータの取り込み、変更イベント（*Changed、ServerChange）、ReadOnly（ポストされた値を無視）、リクエスト検証（ValidateRequestMode）、ポストバックイベント、コマンドイベントと親へのバブル（CommandName）、送信ボタン／スクリプトによるポストバック（UseSubmitBehavior）、ブラウザ側のクリック処理（OnClientClick）、クロスページポストバック（PostBackUrl）、既定のボタン（Panel の DefaultButton）、AutoPostBack（クライアントスクリプトによるポストバック）、CausesValidation / ValidationGroup、検証対象（サーバー側の検証）、検証対象（ブラウザ側の検証）、双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値）、項目（ListItem の Selected・Enabled・Text・Value と ViewState）、複数選択、繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）、ポスト値のイベント検証（一覧にない値・無効な項目の拒否）、サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）、ブラウザ側の検証スクリプト（EnableClientScript）、検証結果のまとめ（ValidationSummary）、ページング、並べ替え、項目の挿入、行・項目の編集（編集・更新・取り消し・削除）、行・項目の選択、行・項目のコマンド（RowCommand・ItemCommand へのバブル）、データキー（DataKeyNames・DataKeys）、データソースコントロールとの連携（DataSourceID）、テンプレート、データソース連携（DataSource・DataTextField など）、クライアントコールバック

## ① API 層

Literal とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の Literal インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。

| ケース | 期待値（.NET Framework） |
|---|---|
| <code>Api.Control.#ctor</code> | <code>Control.#ctor = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMode=In…</code> |
| <code>Api.Control.Adapter</code> | <code>Control.Adapter = null</code> |
| <code>Api.Control.AddParsedSubObject</code> | <code>Control.AddParsedSubObject(Object) = &quot;-&gt; System.Web.HttpException: &#39;Literal&#39; cannot have children of type &#39;String&#39;.&quot;</code> |
| <code>Api.Control.AddedControl</code> | <code>Control.AddedControl(Control,Int32) = &quot;ok&quot;</code><br><code>Control.AddedControl(Control,Int32) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; M…</code> |
| <code>Api.Control.AppRelativeTemplateSourceDirectory</code> | <code>Control.AppRelativeTemplateSourceDirectory = &quot;&quot;</code><br><code>Control.AppRelativeTemplateSourceDirectory set = &quot;-&gt; System.ArgumentException: The relative virtual path &#39;Abc 123&#39; is not allowed here.&quot;</code> |
| <code>Api.Control.ApplyStyleSheetSkin</code> | <code>Control.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>Control.ApplyStyleSheetSkin(Page) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mod…</code> |
| <code>Api.Control.BeginRenderTracing</code> | <code>Control.BeginRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.BeginRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.BeginRenderTracing(TextWriter,Object) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;l…</code> |
| <code>Api.Control.BindingContainer</code> | <code>Control.BindingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Clien…</code> |
| <code>Api.Control.BuildProfileTree</code> | <code>Control.BuildProfileTree(String,Boolean) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.ChildControlsCreated</code> | <code>Control.ChildControlsCreated = &quot;false&quot;</code><br><code>Control.ChildControlsCreated set = &quot;ok&quot;</code> |
| <code>Api.Control.ClearCachedClientID</code> | <code>Control.ClearCachedClientID = &quot;ok&quot;</code><br><code>Control.ClearCachedClientID after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=Tran…</code> |
| <code>Api.Control.ClearChildControlState</code> | <code>Control.ClearChildControlState = &quot;ok&quot;</code><br><code>Control.ClearChildControlState after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=T…</code> |
| <code>Api.Control.ClearChildState</code> | <code>Control.ClearChildState = &quot;ok&quot;</code><br><code>Control.ClearChildState after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=Transfor…</code> |
| <code>Api.Control.ClearChildViewState</code> | <code>Control.ClearChildViewState = &quot;ok&quot;</code><br><code>Control.ClearChildViewState after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=Tran…</code> |
| <code>Api.Control.ClearEffectiveClientIDMode</code> | <code>Control.ClearEffectiveClientIDMode = &quot;ok&quot;</code><br><code>Control.ClearEffectiveClientIDMode after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mo…</code> |
| <code>Api.Control.ClientID</code> | <code>Control.ClientID = &quot;litText&quot;</code> |
| <code>Api.Control.ClientIDMode</code> | <code>Control.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>Control.ClientIDMode set = &quot;ClientIDMode.AutoID&quot;</code> |
| <code>Api.Control.ClientIDSeparator</code> | <code>Control.ClientIDSeparator = &quot;&#39;_&#39;&quot;</code> |
| <code>Api.Control.Context</code> | <code>Control.Context = null</code> |
| <code>Api.Control.Controls</code> | <code>Control.Controls = ControlCollection []</code> |
| <code>Api.Control.CreateChildControls</code> | <code>Control.CreateChildControls = &quot;ok&quot;</code><br><code>Control.CreateChildControls after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=Tran…</code> |
| <code>Api.Control.CreateControlCollection</code> | <code>Control.CreateControlCollection = ControlCollection []</code><br><code>Control.CreateControlCollection after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=…</code> |
| <code>Api.Control.DataBind</code> | <code>Control.DataBind = &quot;ok&quot;</code><br><code>Control.DataBind after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=Transform; Skin…</code><br><code>Control.DataBind(Boolean) = &quot;ok&quot;</code><br><code>Control.DataBind(Boolean) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=Transf…</code> |
| <code>Api.Control.DataBindChildren</code> | <code>Control.DataBindChildren = &quot;ok&quot;</code><br><code>Control.DataBindChildren after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=Transfo…</code> |
| <code>Api.Control.DataBinding</code> | <code>Control.DataBinding = &quot;added and removed&quot;</code> |
| <code>Api.Control.DataItemContainer</code> | <code>Control.DataItemContainer = null</code> |
| <code>Api.Control.DataKeysContainer</code> | <code>Control.DataKeysContainer = null</code> |
| <code>Api.Control.DesignMode</code> | <code>Control.DesignMode = &quot;false&quot;</code> |
| <code>Api.Control.Dispose</code> | <code>Control.Dispose = &quot;ok&quot;</code><br><code>Control.Dispose after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=Transform; SkinI…</code> |
| <code>Api.Control.Disposed</code> | <code>Control.Disposed = &quot;added and removed&quot;</code> |
| <code>Api.Control.EnableTheming</code> | <code>Control.EnableTheming = &quot;true&quot;</code><br><code>Control.EnableTheming set = &quot;true&quot;</code> |
| <code>Api.Control.EnableViewState</code> | <code>Control.EnableViewState = &quot;true&quot;</code><br><code>Control.EnableViewState set = &quot;true&quot;</code> |
| <code>Api.Control.EndRenderTracing</code> | <code>Control.EndRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.EndRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.EndRenderTracing(TextWriter,Object) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lit…</code> |
| <code>Api.Control.EnsureChildControls</code> | <code>Control.EnsureChildControls = &quot;ok&quot;</code><br><code>Control.EnsureChildControls after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=Tran…</code> |
| <code>Api.Control.EnsureID</code> | <code>Control.EnsureID = &quot;ok&quot;</code><br><code>Control.EnsureID after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=Transform; Skin…</code> |
| <code>Api.Control.Events</code> | <code>Control.Events = &quot;System.ComponentModel.EventHandlerList&quot;</code> |
| <code>Api.Control.FindControl</code> | <code>Control.FindControl(String) = Label#lblTitle {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code><br><code>Control.FindControl(String) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=Tran…</code><br><code>Control.FindControl(String,Int32) = null</code><br><code>Control.FindControl(String,Int32) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mod…</code> |
| <code>Api.Control.Focus</code> | <code>Control.Focus = &quot;-&gt; System.NotSupportedException: Control of type &#39;Literal&#39; does not support the Focus operation.&quot;</code> |
| <code>Api.Control.GetDesignModeState</code> | <code>Control.GetDesignModeState = &quot;[]&quot;</code><br><code>Control.GetDesignModeState after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=Trans…</code> |
| <code>Api.Control.GetRouteUrl</code> | <code>Control.GetRouteUrl(Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.GetUniqueIDRelativeTo</code> | <code>Control.GetUniqueIDRelativeTo(Control) = &quot;litText&quot;</code><br><code>Control.GetUniqueIDRelativeTo(Control) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;…</code> |
| <code>Api.Control.HasChildViewState</code> | <code>Control.HasChildViewState = &quot;false&quot;</code> |
| <code>Api.Control.HasControls</code> | <code>Control.HasControls = &quot;false&quot;</code><br><code>Control.HasControls after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=Transform; S…</code> |
| <code>Api.Control.HasEvents</code> | <code>Control.HasEvents = &quot;false&quot;</code><br><code>Control.HasEvents after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=Transform; Ski…</code> |
| <code>Api.Control.ID</code> | <code>Control.ID = &quot;litText&quot;</code><br><code>Control.ID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.IdSeparator</code> | <code>Control.IdSeparator = &quot;&#39;$&#39;&quot;</code> |
| <code>Api.Control.Init</code> | <code>Control.Init = &quot;added and removed&quot;</code> |
| <code>Api.Control.IsChildControlStateCleared</code> | <code>Control.IsChildControlStateCleared = &quot;false&quot;</code> |
| <code>Api.Control.IsLiteralContent</code> | <code>Control.IsLiteralContent = &quot;false&quot;</code><br><code>Control.IsLiteralContent after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=Transfo…</code> |
| <code>Api.Control.IsTrackingViewState</code> | <code>Control.IsTrackingViewState = &quot;false&quot;</code> |
| <code>Api.Control.IsViewStateEnabled</code> | <code>Control.IsViewStateEnabled = &quot;true&quot;</code> |
| <code>Api.Control.Load</code> | <code>Control.Load = &quot;added and removed&quot;</code> |
| <code>Api.Control.LoadControlState</code> | <code>Control.LoadControlState(Object) = &quot;ok&quot;</code><br><code>Control.LoadControlState(Object) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode…</code> |
| <code>Api.Control.LoadViewState</code> | <code>Control.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Collections.ArrayList&#39;.&quot;</code> |
| <code>Api.Control.LoadViewStateByID</code> | <code>Control.LoadViewStateByID = &quot;false&quot;</code> |
| <code>Api.Control.MapPathSecure</code> | <code>Control.MapPathSecure(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.NamingContainer</code> | <code>Control.NamingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code> |
| <code>Api.Control.OnBubbleEvent</code> | <code>Control.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Control.OnBubbleEvent(Object,EventArgs) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText…</code> |
| <code>Api.Control.OnDataBinding</code> | <code>Control.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnDataBinding(EventArgs) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode…</code> |
| <code>Api.Control.OnInit</code> | <code>Control.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnInit(EventArgs) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=Transf…</code> |
| <code>Api.Control.OnLoad</code> | <code>Control.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnLoad(EventArgs) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=Transf…</code> |
| <code>Api.Control.OnPreRender</code> | <code>Control.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnPreRender(EventArgs) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=T…</code> |
| <code>Api.Control.OnUnload</code> | <code>Control.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnUnload(EventArgs) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=Tran…</code> |
| <code>Api.Control.OpenFile</code> | <code>Control.OpenFile(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.Page</code> | <code>Control.Page = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=&quot;&quot;; …</code><br><code>Control.Page set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=…</code> |
| <code>Api.Control.Parent</code> | <code>Control.Parent = HtmlForm#form1 {Action=&quot;&quot;; ClientID=&quot;form1&quot;; ClientIDMode=Inherit; DefaultButton=&quot;&quot;; DefaultFocus=&quot;&quot;; Disabled=false; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.PreRender</code> | <code>Control.PreRender = &quot;added and removed&quot;</code> |
| <code>Api.Control.RaiseBubbleEvent</code> | <code>Control.RaiseBubbleEvent(Object,EventArgs) = &quot;ok&quot;</code><br><code>Control.RaiseBubbleEvent(Object,EventArgs) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litT…</code> |
| <code>Api.Control.RemovedControl</code> | <code>Control.RemovedControl(Control) = &quot;ok&quot;</code><br><code>Control.RemovedControl(Control) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=…</code> |
| <code>Api.Control.Render</code> | <code>Control.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.Render(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;b&gt;literal&lt;/b&gt;</code><br><code>Control.Render(HtmlTextWriter) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=T…</code> |
| <code>Api.Control.RenderChildren</code> | <code>Control.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderChildren(HtmlTextWriter) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;…</code> |
| <code>Api.Control.RenderControl</code> | <code>Control.RenderControl(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;b&gt;literal&lt;/b&gt;</code><br><code>Control.RenderControl(HtmlTextWriter) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;;…</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) writer:</code><br><code>  &#124; &lt;b&gt;literal&lt;/b&gt;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewS…</code> |
| <code>Api.Control.RenderingCompatibility</code> | <code>Control.RenderingCompatibility = &quot;System.Version 4.0&quot;</code><br><code>Control.RenderingCompatibility set = &quot;System.Version 4.0&quot;</code> |
| <code>Api.Control.ResolveAdapter</code> | <code>Control.ResolveAdapter = null</code><br><code>Control.ResolveAdapter after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=Transform…</code> |
| <code>Api.Control.ResolveClientUrl</code> | <code>Control.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveClientUrl(String) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode…</code> |
| <code>Api.Control.ResolveUrl</code> | <code>Control.ResolveUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveUrl(String) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=Trans…</code> |
| <code>Api.Control.SaveControlState</code> | <code>Control.SaveControlState = null</code><br><code>Control.SaveControlState after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=Transfo…</code> |
| <code>Api.Control.SaveViewState</code> | <code>Control.SaveViewState = null</code><br><code>Control.SaveViewState after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=Transform;…</code> |
| <code>Api.Control.SetDesignModeState</code> | <code>Control.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>Control.SetDesignModeState(IDictionary) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText…</code> |
| <code>Api.Control.SetRenderMethodDelegate</code> | <code>Control.SetRenderMethodDelegate(RenderMethod) = &quot;ok&quot;</code><br><code>Control.SetRenderMethodDelegate(RenderMethod) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;l…</code> |
| <code>Api.Control.SetTraceData</code> | <code>Control.SetTraceData(Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; M…</code><br><code>Control.SetTraceData(Object,Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object,Object) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litT…</code> |
| <code>Api.Control.Site</code> | <code>Control.Site = null</code><br><code>Control.Site set = null</code> |
| <code>Api.Control.SkinID</code> | <code>Control.SkinID = &quot;&quot;</code><br><code>Control.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.TemplateControl</code> | <code>Control.TemplateControl = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code><br><code>Control.TemplateControl set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Cl…</code> |
| <code>Api.Control.TemplateSourceDirectory</code> | <code>Control.TemplateSourceDirectory = &quot;&quot;</code> |
| <code>Api.Control.TrackViewState</code> | <code>Control.TrackViewState = &quot;ok&quot;</code><br><code>Control.TrackViewState after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=Transform…</code> |
| <code>Api.Control.UniqueID</code> | <code>Control.UniqueID = &quot;litText&quot;</code> |
| <code>Api.Control.Unload</code> | <code>Control.Unload = &quot;added and removed&quot;</code> |
| <code>Api.Control.ValidateRequestMode</code> | <code>Control.ValidateRequestMode = &quot;ValidateRequestMode.Inherit&quot;</code><br><code>Control.ValidateRequestMode set = &quot;ValidateRequestMode.Disabled&quot;</code> |
| <code>Api.Control.ViewState</code> | <code>Control.ViewState = StateBag [Text=&quot;&lt;b&gt;literal&lt;/b&gt;&quot;]</code> |
| <code>Api.Control.ViewStateIgnoresCase</code> | <code>Control.ViewStateIgnoresCase = &quot;false&quot;</code> |
| <code>Api.Control.ViewStateMode</code> | <code>Control.ViewStateMode = &quot;ViewStateMode.Inherit&quot;</code><br><code>Control.ViewStateMode set = &quot;ViewStateMode.Enabled&quot;</code> |
| <code>Api.Control.Visible</code> | <code>Control.Visible = &quot;true&quot;</code><br><code>Control.Visible set = &quot;true&quot;</code> |
| <code>Api.IControlBuilderAccessor.ControlBuilder</code> | <code>IControlBuilderAccessor.ControlBuilder = null</code> |
| <code>Api.IControlDesignerAccessor.GetDesignModeState</code> | <code>IControlDesignerAccessor.GetDesignModeState = &quot;[]&quot;</code><br><code>IControlDesignerAccessor.GetDesignModeState after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lit…</code> |
| <code>Api.IControlDesignerAccessor.SetDesignModeState</code> | <code>IControlDesignerAccessor.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetDesignModeState(IDictionary) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=…</code> |
| <code>Api.IControlDesignerAccessor.SetOwnerControl</code> | <code>IControlDesignerAccessor.SetOwnerControl(Control) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetOwnerControl(Control) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; I…</code> |
| <code>Api.IControlDesignerAccessor.UserData</code> | <code>IControlDesignerAccessor.UserData = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.DataBindings</code> | <code>IDataBindingsAccessor.DataBindings = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.HasDataBindings</code> | <code>IDataBindingsAccessor.HasDataBindings = &quot;false&quot;</code> |
| <code>Api.IExpressionsAccessor.Expressions</code> | <code>IExpressionsAccessor.Expressions = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.HasExpressions</code> | <code>IExpressionsAccessor.HasExpressions = &quot;false&quot;</code> |
| <code>Api.IParserAccessor.AddParsedSubObject</code> | <code>IParserAccessor.AddParsedSubObject(Object) = &quot;-&gt; System.Web.HttpException: &#39;Literal&#39; cannot have children of type &#39;String&#39;.&quot;</code> |
| <code>Api.ITextControl.Text</code> | <code>ITextControl.Text = &quot;&lt;b&gt;literal&lt;/b&gt;&quot;</code><br><code>ITextControl.Text set = &quot;Abc 123&quot;</code> |
| <code>Api.IUrlResolutionService.ResolveClientUrl</code> | <code>IUrlResolutionService.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>IUrlResolutionService.ResolveClientUrl(String) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;…</code> |
| <code>Api.WebControls.Literal.#ctor</code> | <code>Literal.#ctor = Literal# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; Mode=Transform; SkinID=&quot;&quot;; Text=&quot;&quot;; UniqueID=nul…</code> |
| <code>Api.WebControls.Literal.AddParsedSubObject</code> | <code>Literal.AddParsedSubObject(Object) = &quot;-&gt; System.Web.HttpException: &#39;Literal&#39; cannot have children of type &#39;String&#39;.&quot;</code> |
| <code>Api.WebControls.Literal.CreateControlCollection</code> | <code>Literal.CreateControlCollection = ControlCollection []</code><br><code>Literal.CreateControlCollection after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=…</code> |
| <code>Api.WebControls.Literal.Focus</code> | <code>Literal.Focus = &quot;-&gt; System.NotSupportedException: Control of type &#39;Literal&#39; does not support the Focus operation.&quot;</code> |
| <code>Api.WebControls.Literal.Mode</code> | <code>Literal.Mode = &quot;LiteralMode.Transform&quot;</code><br><code>Literal.Mode set = &quot;LiteralMode.PassThrough&quot;</code> |
| <code>Api.WebControls.Literal.Render</code> | <code>Literal.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Literal.Render(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;b&gt;literal&lt;/b&gt;</code><br><code>Literal.Render(HtmlTextWriter) after = Literal#litText {ClientID=&quot;litText&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;litText&quot;; Mode=T…</code> |
| <code>Api.WebControls.Literal.Text</code> | <code>Literal.Text = &quot;&lt;b&gt;literal&lt;/b&gt;&quot;</code><br><code>Literal.Text set = &quot;Abc 123&quot;</code> |

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
| <code>.Mode=Encode+Text=japanese</code> | <code>c.Mode = LiteralMode.Encode; c.Text = &quot;日本語テキスト&quot;;</code> | <code>日本語テキスト</code> |  |
| <code>.Text=text+EnableViewState=false</code> | <code>c.Text = &quot;probe&quot;; c.EnableViewState = false;</code> | <code>probe</code> |  |
| <code>.Text=text+Visible=false</code> | <code>c.Text = &quot;probe&quot;; c.Visible = false;</code> | <code></code> |  |
| <code>[naming container].(default)</code> | <code>[naming container] (nothing set)</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=AutoID</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Static</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Predictable</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Inherit</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ID=text</code> | <code>[naming container] c.ID = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |

## ③ シナリオ層

ブラウザ（Playwright）で LiteralScenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。

| # | 操作 | 対象 | 値 |
|---|---|---|---|
| 1 | goto | /BasicsScenario.aspx | <code></code> |
| 2 | snapshot |  | **Literal [render][databind][dynamic] first request** |
| 3 | click | btnSubmit | <code></code> |
| 4 | snapshot |  | **Literal [viewstate][dynamic] posted (set in code: kept; without view state: lost)** |
| 5 | click | btnShow | <code></code> |
| 6 | snapshot |  | **Literal [visible][viewstate] hidden ones shown with what was set while hidden** |

<details><summary>Literal [render][databind][dynamic] first request（期待される本文）</summary>

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

<details><summary>Literal [viewstate][dynamic] posted (set in code: kept; without view state: lost)（期待される本文）</summary>

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

<details><summary>Literal [visible][viewstate] hidden ones shown with what was set while hidden（期待される本文）</summary>

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

