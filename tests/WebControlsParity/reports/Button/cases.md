# Button テストケース一覧

変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。
このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。

- 正解データの採取: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0
- ① API 層: 171 ケース（180 メンバー）
- ② HTML 層: 167 ケース
- ③ シナリオ層: 15 スナップショット

## 機構マトリクス

型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。

| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |
|---|---|---|---|
| プロパティ → HTML 描画 | すべてのコントロール | 152 | 1 |
| ViewState の保存と復元 | Control | 6 | 3 |
| Visible=false（描画されず状態は保持） | Control.Visible | 3 | 2 |
| 名前付けコンテナと ClientIDMode | Control.ClientIDMode | 9 | 0 |
| テーマとスキン（SkinID / EnableTheming） | テーマを付けられる型（[Themeable]） | 5 | 0 |
| 動的に生成したコントロールのライフサイクル | Control | 0 | 1 |
| データバインド（DataBind / <%# %>） | Control.DataBind | 0 | 2 |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | WebControl.Enabled、HtmlControl.Disabled | 5 | 1 |
| ポストバックイベント | IPostBackEventHandler | 0 | 5 |
| コマンドイベントと親へのバブル（CommandName） | IButtonControl | 9 | 2 |
| 送信ボタン／スクリプトによるポストバック（UseSubmitBehavior） | UseSubmitBehavior プロパティ | 10 | 1 |
| ブラウザ側のクリック処理（OnClientClick） | OnClientClick プロパティ | 10 | 2 |
| クロスページポストバック（PostBackUrl） | IButtonControl.PostBackUrl | 12 | 1 |
| 既定のボタン（Panel の DefaultButton） | IButtonControl | 0 | 1 |
| CausesValidation / ValidationGroup | CausesValidation プロパティ | 13 | 4 |

該当しない機構: ポストバックデータの取り込み、変更イベント（*Changed、ServerChange）、ReadOnly（ポストされた値を無視）、リクエスト検証（ValidateRequestMode）、AutoPostBack（クライアントスクリプトによるポストバック）、検証対象（サーバー側の検証）、検証対象（ブラウザ側の検証）、双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値）、項目（ListItem の Selected・Enabled・Text・Value と ViewState）、複数選択、繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）、ポスト値のイベント検証（一覧にない値・無効な項目の拒否）、サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）、ブラウザ側の検証スクリプト（EnableClientScript）、検証結果のまとめ（ValidationSummary）、ページング、並べ替え、項目の挿入、行・項目の編集（編集・更新・取り消し・削除）、行・項目の選択、行・項目のコマンド（RowCommand・ItemCommand へのバブル）、データキー（DataKeyNames・DataKeys）、データソースコントロールとの連携（DataSourceID）、テンプレート、データソース連携（DataSource・DataTextField など）、クライアントコールバック

## ① API 層

Button とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の Button インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。

| ケース | 期待値（.NET Framework） |
|---|---|
| <code>Api.Control.#ctor</code> | <code>Control.#ctor = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMode=In…</code> |
| <code>Api.Control.Adapter</code> | <code>Control.Adapter = null</code> |
| <code>Api.Control.AddParsedSubObject</code> | <code>Control.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>Control.AddParsedSubObject(Object) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Borde…</code> |
| <code>Api.Control.AddedControl</code> | <code>Control.AddedControl(Control,Int32) = &quot;ok&quot;</code><br><code>Control.AddedControl(Control,Int32) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bord…</code> |
| <code>Api.Control.AppRelativeTemplateSourceDirectory</code> | <code>Control.AppRelativeTemplateSourceDirectory = &quot;&quot;</code><br><code>Control.AppRelativeTemplateSourceDirectory set = &quot;-&gt; System.ArgumentException: The relative virtual path &#39;Abc 123&#39; is not allowed here.&quot;</code> |
| <code>Api.Control.ApplyStyleSheetSkin</code> | <code>Control.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>Control.ApplyStyleSheetSkin(Page) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Border…</code> |
| <code>Api.Control.BeginRenderTracing</code> | <code>Control.BeginRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.BeginRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.BeginRenderTracing(TextWriter,Object) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Em…</code> |
| <code>Api.Control.BindingContainer</code> | <code>Control.BindingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Clien…</code> |
| <code>Api.Control.BuildProfileTree</code> | <code>Control.BuildProfileTree(String,Boolean) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.ChildControlsCreated</code> | <code>Control.ChildControlsCreated = &quot;false&quot;</code><br><code>Control.ChildControlsCreated set = &quot;ok&quot;</code> |
| <code>Api.Control.ClearCachedClientID</code> | <code>Control.ClearCachedClientID = &quot;ok&quot;</code><br><code>Control.ClearCachedClientID after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=…</code> |
| <code>Api.Control.ClearChildControlState</code> | <code>Control.ClearChildControlState = &quot;ok&quot;</code><br><code>Control.ClearChildControlState after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSty…</code> |
| <code>Api.Control.ClearChildState</code> | <code>Control.ClearChildState = &quot;ok&quot;</code><br><code>Control.ClearChildState after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotS…</code> |
| <code>Api.Control.ClearChildViewState</code> | <code>Control.ClearChildViewState = &quot;ok&quot;</code><br><code>Control.ClearChildViewState after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=…</code> |
| <code>Api.Control.ClearEffectiveClientIDMode</code> | <code>Control.ClearEffectiveClientIDMode = &quot;ok&quot;</code><br><code>Control.ClearEffectiveClientIDMode after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Borde…</code> |
| <code>Api.Control.ClientID</code> | <code>Control.ClientID = &quot;btnGo&quot;</code> |
| <code>Api.Control.ClientIDMode</code> | <code>Control.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>Control.ClientIDMode set = &quot;ClientIDMode.AutoID&quot;</code> |
| <code>Api.Control.ClientIDSeparator</code> | <code>Control.ClientIDSeparator = &quot;&#39;_&#39;&quot;</code> |
| <code>Api.Control.Context</code> | <code>Control.Context = null</code> |
| <code>Api.Control.Controls</code> | <code>Control.Controls = ControlCollection []</code> |
| <code>Api.Control.CreateChildControls</code> | <code>Control.CreateChildControls = &quot;ok&quot;</code><br><code>Control.CreateChildControls after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=…</code> |
| <code>Api.Control.CreateControlCollection</code> | <code>Control.CreateControlCollection = ControlCollection []</code><br><code>Control.CreateControlCollection after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSt…</code> |
| <code>Api.Control.DataBind</code> | <code>Control.DataBind = &quot;ok&quot;</code><br><code>Control.DataBind after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; Bor…</code><br><code>Control.DataBind(Boolean) = &quot;ok&quot;</code><br><code>Control.DataBind(Boolean) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=No…</code> |
| <code>Api.Control.DataBindChildren</code> | <code>Control.DataBindChildren = &quot;ok&quot;</code><br><code>Control.DataBindChildren after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=Not…</code> |
| <code>Api.Control.DataBinding</code> | <code>Control.DataBinding = &quot;added and removed&quot;</code> |
| <code>Api.Control.DataItemContainer</code> | <code>Control.DataItemContainer = null</code> |
| <code>Api.Control.DataKeysContainer</code> | <code>Control.DataKeysContainer = null</code> |
| <code>Api.Control.DesignMode</code> | <code>Control.DesignMode = &quot;false&quot;</code> |
| <code>Api.Control.Dispose</code> | <code>Control.Dispose = &quot;ok&quot;</code><br><code>Control.Dispose after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; Bord…</code> |
| <code>Api.Control.Disposed</code> | <code>Control.Disposed = &quot;added and removed&quot;</code> |
| <code>Api.Control.EnableTheming</code> | <code>Control.EnableTheming = &quot;true&quot;</code><br><code>Control.EnableTheming set = &quot;true&quot;</code> |
| <code>Api.Control.EnableViewState</code> | <code>Control.EnableViewState = &quot;true&quot;</code><br><code>Control.EnableViewState set = &quot;true&quot;</code> |
| <code>Api.Control.EndRenderTracing</code> | <code>Control.EndRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.EndRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.EndRenderTracing(TextWriter,Object) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empt…</code> |
| <code>Api.Control.EnsureChildControls</code> | <code>Control.EnsureChildControls = &quot;ok&quot;</code><br><code>Control.EnsureChildControls after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=…</code> |
| <code>Api.Control.EnsureID</code> | <code>Control.EnsureID = &quot;ok&quot;</code><br><code>Control.EnsureID after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; Bor…</code> |
| <code>Api.Control.Events</code> | <code>Control.Events = &quot;System.ComponentModel.EventHandlerList&quot;</code> |
| <code>Api.Control.FindControl</code> | <code>Control.FindControl(String) = Label#lblTitle {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code><br><code>Control.FindControl(String) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=…</code><br><code>Control.FindControl(String,Int32) = null</code><br><code>Control.FindControl(String,Int32) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Border…</code> |
| <code>Api.Control.Focus</code> | <code>Control.Focus = &quot;-&gt; System.InvalidOperationException: A form tag with runat=server must exist on the Page to use SetFocus() or the Focus property.&quot;</code> |
| <code>Api.Control.GetDesignModeState</code> | <code>Control.GetDesignModeState = &quot;[]&quot;</code><br><code>Control.GetDesignModeState after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=N…</code> |
| <code>Api.Control.GetRouteUrl</code> | <code>Control.GetRouteUrl(Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.GetUniqueIDRelativeTo</code> | <code>Control.GetUniqueIDRelativeTo(Control) = &quot;btnGo&quot;</code><br><code>Control.GetUniqueIDRelativeTo(Control) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; B…</code> |
| <code>Api.Control.HasChildViewState</code> | <code>Control.HasChildViewState = &quot;false&quot;</code> |
| <code>Api.Control.HasControls</code> | <code>Control.HasControls = &quot;false&quot;</code><br><code>Control.HasControls after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; …</code> |
| <code>Api.Control.HasEvents</code> | <code>Control.HasEvents = &quot;false&quot;</code><br><code>Control.HasEvents after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; Bo…</code> |
| <code>Api.Control.ID</code> | <code>Control.ID = &quot;btnGo&quot;</code><br><code>Control.ID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.IdSeparator</code> | <code>Control.IdSeparator = &quot;&#39;$&#39;&quot;</code> |
| <code>Api.Control.Init</code> | <code>Control.Init = &quot;added and removed&quot;</code> |
| <code>Api.Control.IsChildControlStateCleared</code> | <code>Control.IsChildControlStateCleared = &quot;false&quot;</code> |
| <code>Api.Control.IsLiteralContent</code> | <code>Control.IsLiteralContent = &quot;false&quot;</code><br><code>Control.IsLiteralContent after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=Not…</code> |
| <code>Api.Control.IsTrackingViewState</code> | <code>Control.IsTrackingViewState = &quot;false&quot;</code> |
| <code>Api.Control.IsViewStateEnabled</code> | <code>Control.IsViewStateEnabled = &quot;true&quot;</code> |
| <code>Api.Control.Load</code> | <code>Control.Load = &quot;added and removed&quot;</code> |
| <code>Api.Control.LoadControlState</code> | <code>Control.LoadControlState(Object) = &quot;ok&quot;</code><br><code>Control.LoadControlState(Object) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderS…</code> |
| <code>Api.Control.LoadViewState</code> | <code>Control.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Web.UI.Pair&#39;.&quot;</code> |
| <code>Api.Control.LoadViewStateByID</code> | <code>Control.LoadViewStateByID = &quot;false&quot;</code> |
| <code>Api.Control.MapPathSecure</code> | <code>Control.MapPathSecure(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.NamingContainer</code> | <code>Control.NamingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code> |
| <code>Api.Control.OnBubbleEvent</code> | <code>Control.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Control.OnBubbleEvent(Object,EventArgs) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; …</code> |
| <code>Api.Control.OnDataBinding</code> | <code>Control.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnDataBinding(EventArgs) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderS…</code> |
| <code>Api.Control.OnInit</code> | <code>Control.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnInit(EventArgs) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=No…</code> |
| <code>Api.Control.OnLoad</code> | <code>Control.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnLoad(EventArgs) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=No…</code> |
| <code>Api.Control.OnPreRender</code> | <code>Control.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnPreRender(EventArgs) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSty…</code> |
| <code>Api.Control.OnUnload</code> | <code>Control.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnUnload(EventArgs) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=…</code> |
| <code>Api.Control.OpenFile</code> | <code>Control.OpenFile(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.Page</code> | <code>Control.Page = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=&quot;&quot;; …</code><br><code>Control.Page set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=…</code> |
| <code>Api.Control.Parent</code> | <code>Control.Parent = HtmlForm#form1 {Action=&quot;&quot;; ClientID=&quot;form1&quot;; ClientIDMode=Inherit; DefaultButton=&quot;&quot;; DefaultFocus=&quot;&quot;; Disabled=false; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.PreRender</code> | <code>Control.PreRender = &quot;added and removed&quot;</code> |
| <code>Api.Control.RaiseBubbleEvent</code> | <code>Control.RaiseBubbleEvent(Object,EventArgs) = &quot;ok&quot;</code><br><code>Control.RaiseBubbleEvent(Object,EventArgs) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty…</code> |
| <code>Api.Control.RemovedControl</code> | <code>Control.RemovedControl(Control) = &quot;ok&quot;</code><br><code>Control.RemovedControl(Control) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSt…</code> |
| <code>Api.Control.Render</code> | <code>Control.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.Render(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;Go&quot; id=&quot;btnGo&quot; /&gt;</code><br><code>Control.Render(HtmlTextWriter) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSty…</code> |
| <code>Api.Control.RenderChildren</code> | <code>Control.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderChildren(HtmlTextWriter) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; B…</code> |
| <code>Api.Control.RenderControl</code> | <code>Control.RenderControl(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;Go&quot; id=&quot;btnGo&quot; /&gt;</code><br><code>Control.RenderControl(HtmlTextWriter) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bo…</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) writer:</code><br><code>  &#124; &lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;Go&quot; id=&quot;btnGo&quot; /&gt;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00…</code> |
| <code>Api.Control.RenderingCompatibility</code> | <code>Control.RenderingCompatibility = &quot;System.Version 4.0&quot;</code><br><code>Control.RenderingCompatibility set = &quot;System.Version 4.0&quot;</code> |
| <code>Api.Control.ResolveAdapter</code> | <code>Control.ResolveAdapter = null</code><br><code>Control.ResolveAdapter after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSe…</code> |
| <code>Api.Control.ResolveClientUrl</code> | <code>Control.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveClientUrl(String) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderS…</code> |
| <code>Api.Control.ResolveUrl</code> | <code>Control.ResolveUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveUrl(String) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=N…</code> |
| <code>Api.Control.SaveControlState</code> | <code>Control.SaveControlState = null</code><br><code>Control.SaveControlState after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=Not…</code> |
| <code>Api.Control.SaveViewState</code> | <code>Control.SaveViewState = null</code><br><code>Control.SaveViewState after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet…</code> |
| <code>Api.Control.SetDesignModeState</code> | <code>Control.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>Control.SetDesignModeState(IDictionary) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; …</code> |
| <code>Api.Control.SetRenderMethodDelegate</code> | <code>Control.SetRenderMethodDelegate(RenderMethod) = &quot;ok&quot;</code><br><code>Control.SetRenderMethodDelegate(RenderMethod) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Em…</code> |
| <code>Api.Control.SetTraceData</code> | <code>Control.SetTraceData(Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bord…</code><br><code>Control.SetTraceData(Object,Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object,Object) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty…</code> |
| <code>Api.Control.Site</code> | <code>Control.Site = null</code><br><code>Control.Site set = null</code> |
| <code>Api.Control.SkinID</code> | <code>Control.SkinID = &quot;&quot;</code><br><code>Control.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.TemplateControl</code> | <code>Control.TemplateControl = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code><br><code>Control.TemplateControl set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Cl…</code> |
| <code>Api.Control.TemplateSourceDirectory</code> | <code>Control.TemplateSourceDirectory = &quot;&quot;</code> |
| <code>Api.Control.TrackViewState</code> | <code>Control.TrackViewState = &quot;ok&quot;</code><br><code>Control.TrackViewState after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSe…</code> |
| <code>Api.Control.UniqueID</code> | <code>Control.UniqueID = &quot;btnGo&quot;</code> |
| <code>Api.Control.Unload</code> | <code>Control.Unload = &quot;added and removed&quot;</code> |
| <code>Api.Control.ValidateRequestMode</code> | <code>Control.ValidateRequestMode = &quot;ValidateRequestMode.Inherit&quot;</code><br><code>Control.ValidateRequestMode set = &quot;ValidateRequestMode.Disabled&quot;</code> |
| <code>Api.Control.ViewState</code> | <code>Control.ViewState = StateBag [CommandArgument=&quot;1&quot;, CommandName=&quot;go&quot;, Text=&quot;Go&quot;]</code> |
| <code>Api.Control.ViewStateIgnoresCase</code> | <code>Control.ViewStateIgnoresCase = &quot;false&quot;</code> |
| <code>Api.Control.ViewStateMode</code> | <code>Control.ViewStateMode = &quot;ViewStateMode.Inherit&quot;</code><br><code>Control.ViewStateMode set = &quot;ViewStateMode.Enabled&quot;</code> |
| <code>Api.Control.Visible</code> | <code>Control.Visible = &quot;true&quot;</code><br><code>Control.Visible set = &quot;true&quot;</code> |
| <code>Api.IAttributeAccessor.GetAttribute</code> | <code>IAttributeAccessor.GetAttribute(String) = null</code><br><code>IAttributeAccessor.GetAttribute(String) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; …</code> |
| <code>Api.IAttributeAccessor.SetAttribute</code> | <code>IAttributeAccessor.SetAttribute(String,String) = &quot;ok&quot;</code><br><code>IAttributeAccessor.SetAttribute(String,String) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [E…</code> |
| <code>Api.IControlBuilderAccessor.ControlBuilder</code> | <code>IControlBuilderAccessor.ControlBuilder = null</code> |
| <code>Api.IControlDesignerAccessor.GetDesignModeState</code> | <code>IControlDesignerAccessor.GetDesignModeState = &quot;[]&quot;</code><br><code>IControlDesignerAccessor.GetDesignModeState after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empt…</code> |
| <code>Api.IControlDesignerAccessor.SetDesignModeState</code> | <code>IControlDesignerAccessor.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetDesignModeState(IDictionary) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #0000000…</code> |
| <code>Api.IControlDesignerAccessor.SetOwnerControl</code> | <code>IControlDesignerAccessor.SetOwnerControl(Control) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetOwnerControl(Control) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color…</code> |
| <code>Api.IControlDesignerAccessor.UserData</code> | <code>IControlDesignerAccessor.UserData = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.DataBindings</code> | <code>IDataBindingsAccessor.DataBindings = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.HasDataBindings</code> | <code>IDataBindingsAccessor.HasDataBindings = &quot;false&quot;</code> |
| <code>Api.IExpressionsAccessor.Expressions</code> | <code>IExpressionsAccessor.Expressions = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.HasExpressions</code> | <code>IExpressionsAccessor.HasExpressions = &quot;false&quot;</code> |
| <code>Api.IParserAccessor.AddParsedSubObject</code> | <code>IParserAccessor.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>IParserAccessor.AddParsedSubObject(Object) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty…</code> |
| <code>Api.IPostBackEventHandler.RaisePostBackEvent</code> | <code>IPostBackEventHandler.RaisePostBackEvent(String) = &quot;ok&quot;</code><br><code>IPostBackEventHandler.RaisePostBackEvent(String) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color …</code> |
| <code>Api.IUrlResolutionService.ResolveClientUrl</code> | <code>IUrlResolutionService.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>IUrlResolutionService.ResolveClientUrl(String) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [E…</code> |
| <code>Api.WebControls.Button.#ctor</code> | <code>Button.#ctor = Button# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; Caus…</code> |
| <code>Api.WebControls.Button.AddAttributesToRender</code> | <code>Button.AddAttributesToRender(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Button.AddAttributesToRender(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Button.AddAttributesToRender(HtmlTextWriter) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Emp…</code> |
| <code>Api.WebControls.Button.CausesValidation</code> | <code>Button.CausesValidation = &quot;true&quot;</code><br><code>Button.CausesValidation set = &quot;true&quot;</code> |
| <code>Api.WebControls.Button.Click</code> | <code>Button.Click = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.Button.Command</code> | <code>Button.Command = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.Button.CommandArgument</code> | <code>Button.CommandArgument = &quot;1&quot;</code><br><code>Button.CommandArgument set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.Button.CommandName</code> | <code>Button.CommandName = &quot;go&quot;</code><br><code>Button.CommandName set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.Button.GetPostBackOptions</code> | <code>Button.GetPostBackOptions = PostBackOptions {ActionUrl=null; Argument=&quot;&quot;; AutoPostBack=false; ClientSubmit=false; PerformValidation=false; RequiresJavaScriptPro…</code><br><code>Button.GetPostBackOptions after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=No…</code> |
| <code>Api.WebControls.Button.OnClick</code> | <code>Button.OnClick(EventArgs) = &quot;ok&quot;</code><br><code>Button.OnClick(EventArgs) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=No…</code> |
| <code>Api.WebControls.Button.OnClientClick</code> | <code>Button.OnClientClick = &quot;&quot;</code><br><code>Button.OnClientClick set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.Button.OnCommand</code> | <code>Button.OnCommand(CommandEventArgs) = &quot;ok&quot;</code><br><code>Button.OnCommand(CommandEventArgs) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Borde…</code> |
| <code>Api.WebControls.Button.OnPreRender</code> | <code>Button.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>Button.OnPreRender(EventArgs) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyl…</code> |
| <code>Api.WebControls.Button.PostBackUrl</code> | <code>Button.PostBackUrl = &quot;&quot;</code><br><code>Button.PostBackUrl set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.Button.RaisePostBackEvent</code> | <code>Button.RaisePostBackEvent(String) = &quot;ok&quot;</code><br><code>Button.RaisePostBackEvent(String) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Border…</code> |
| <code>Api.WebControls.Button.RenderContents</code> | <code>Button.RenderContents(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Button.RenderContents(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Button.RenderContents(HtmlTextWriter) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bo…</code> |
| <code>Api.WebControls.Button.Text</code> | <code>Button.Text = &quot;Go&quot;</code><br><code>Button.Text set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.Button.UseSubmitBehavior</code> | <code>Button.UseSubmitBehavior = &quot;true&quot;</code><br><code>Button.UseSubmitBehavior set = &quot;true&quot;</code> |
| <code>Api.WebControls.Button.ValidationGroup</code> | <code>Button.ValidationGroup = &quot;&quot;</code><br><code>Button.ValidationGroup set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.IButtonControl.CausesValidation</code> | <code>IButtonControl.CausesValidation = &quot;true&quot;</code><br><code>IButtonControl.CausesValidation set = &quot;true&quot;</code> |
| <code>Api.WebControls.IButtonControl.Click</code> | <code>IButtonControl.Click = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.IButtonControl.Command</code> | <code>IButtonControl.Command = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.IButtonControl.CommandArgument</code> | <code>IButtonControl.CommandArgument = &quot;1&quot;</code><br><code>IButtonControl.CommandArgument set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.IButtonControl.CommandName</code> | <code>IButtonControl.CommandName = &quot;go&quot;</code><br><code>IButtonControl.CommandName set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.IButtonControl.PostBackUrl</code> | <code>IButtonControl.PostBackUrl = &quot;&quot;</code><br><code>IButtonControl.PostBackUrl set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.IButtonControl.Text</code> | <code>IButtonControl.Text = &quot;Go&quot;</code><br><code>IButtonControl.Text set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.IButtonControl.ValidationGroup</code> | <code>IButtonControl.ValidationGroup = &quot;&quot;</code><br><code>IButtonControl.ValidationGroup set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.#ctor</code> | <code>WebControl.#ctor = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidt…</code><br><code>WebControl.#ctor(String) = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; Bo…</code><br><code>WebControl.#ctor(HtmlTextWriterTag) = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle…</code> |
| <code>Api.WebControls.WebControl.AccessKey</code> | <code>WebControl.AccessKey = &quot;&quot;</code><br><code>WebControl.AccessKey set = &quot;-&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]&quot;</code> |
| <code>Api.WebControls.WebControl.AddAttributesToRender</code> | <code>WebControl.AddAttributesToRender(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.AddAttributesToRender(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>WebControl.AddAttributesToRender(HtmlTextWriter) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color …</code> |
| <code>Api.WebControls.WebControl.ApplyStyle</code> | <code>WebControl.ApplyStyle(Style) = &quot;ok&quot;</code><br><code>WebControl.ApplyStyle(Style) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle…</code> |
| <code>Api.WebControls.WebControl.Attributes</code> | <code>WebControl.Attributes = AttributeCollection []</code> |
| <code>Api.WebControls.WebControl.BackColor</code> | <code>WebControl.BackColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>WebControl.BackColor set = &quot;Color #FF3366CC Color [A=255, R=51, G=102, B=204]&quot;</code> |
| <code>Api.WebControls.WebControl.BorderColor</code> | <code>WebControl.BorderColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>WebControl.BorderColor set = &quot;Color #FF3366CC Color [A=255, R=51, G=102, B=204]&quot;</code> |
| <code>Api.WebControls.WebControl.BorderStyle</code> | <code>WebControl.BorderStyle = &quot;BorderStyle.NotSet&quot;</code><br><code>WebControl.BorderStyle set = &quot;BorderStyle.None&quot;</code> |
| <code>Api.WebControls.WebControl.BorderWidth</code> | <code>WebControl.BorderWidth = Unit </code><br><code>WebControl.BorderWidth set = Unit 120px</code> |
| <code>Api.WebControls.WebControl.ControlStyle</code> | <code>WebControl.ControlStyle = Style {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; CssClass=…</code> |
| <code>Api.WebControls.WebControl.ControlStyleCreated</code> | <code>WebControl.ControlStyleCreated = &quot;false&quot;</code> |
| <code>Api.WebControls.WebControl.CopyBaseAttributes</code> | <code>WebControl.CopyBaseAttributes(WebControl) = &quot;ok&quot;</code><br><code>WebControl.CopyBaseAttributes(WebControl) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]…</code> |
| <code>Api.WebControls.WebControl.CreateControlStyle</code> | <code>WebControl.CreateControlStyle = Style {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; Css…</code><br><code>WebControl.CreateControlStyle after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyl…</code> |
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
| <code>Api.WebControls.WebControl.MergeStyle</code> | <code>WebControl.MergeStyle(Style) = &quot;ok&quot;</code><br><code>WebControl.MergeStyle(Style) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle…</code> |
| <code>Api.WebControls.WebControl.Render</code> | <code>WebControl.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.Render(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;Go&quot; id=&quot;btnGo&quot; /&gt;</code><br><code>WebControl.Render(HtmlTextWriter) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Border…</code> |
| <code>Api.WebControls.WebControl.RenderBeginTag</code> | <code>WebControl.RenderBeginTag(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.RenderBeginTag(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;Go&quot; id=&quot;btnGo&quot; /&gt;</code><br><code>WebControl.RenderBeginTag(HtmlTextWriter) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]…</code> |
| <code>Api.WebControls.WebControl.RenderContents</code> | <code>WebControl.RenderContents(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.RenderContents(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>WebControl.RenderContents(HtmlTextWriter) after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]…</code> |
| <code>Api.WebControls.WebControl.RenderEndTag</code> | <code>WebControl.RenderEndTag(HtmlTextWriter) = &quot;-&gt; System.InvalidOperationException: A PopEndTag was called without a corresponding PushEndTag.&quot;</code> |
| <code>Api.WebControls.WebControl.SaveViewState</code> | <code>WebControl.SaveViewState = null</code><br><code>WebControl.SaveViewState after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=Not…</code> |
| <code>Api.WebControls.WebControl.SkinID</code> | <code>WebControl.SkinID = &quot;&quot;</code><br><code>WebControl.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.Style</code> | <code>WebControl.Style = CssStyleCollection {Count=Int32 0; Value=null}</code> |
| <code>Api.WebControls.WebControl.SupportsDisabledAttribute</code> | <code>WebControl.SupportsDisabledAttribute = &quot;true&quot;</code> |
| <code>Api.WebControls.WebControl.TabIndex</code> | <code>WebControl.TabIndex = &quot;Int16 0&quot;</code><br><code>WebControl.TabIndex set = &quot;Int16 1&quot;</code> |
| <code>Api.WebControls.WebControl.TagKey</code> | <code>WebControl.TagKey = &quot;HtmlTextWriterTag.Input&quot;</code> |
| <code>Api.WebControls.WebControl.TagName</code> | <code>WebControl.TagName = &quot;input&quot;</code> |
| <code>Api.WebControls.WebControl.ToolTip</code> | <code>WebControl.ToolTip = &quot;&quot;</code><br><code>WebControl.ToolTip set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.TrackViewState</code> | <code>WebControl.TrackViewState = &quot;ok&quot;</code><br><code>WebControl.TrackViewState after = Button#btnGo {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=No…</code> |
| <code>Api.WebControls.WebControl.Width</code> | <code>WebControl.Width = Unit </code><br><code>WebControl.Width set = Unit 120px</code> |

## ② HTML 層

プローブサイトの RenderProbe.aspx で、1 ケース 1 リクエスト（ページライフサイクル全体）で描画したページ全体を比較します。下表はコンポーネント部分（div#probe）、設定時の例外、保存された ViewState の要約です。

| ケース | 設定 | 期待される描画 | 設定時の例外 |
|---|---|---|---|
| <code>.(default)</code> | <code>(nothing set)</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.AccessKey=text</code> | <code>c.AccessKey = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> | <code>c.AccessKey = &quot;probe&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=markup</code> | <code>c.AccessKey = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> | <code>c.AccessKey = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=japanese</code> | <code>c.AccessKey = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> | <code>c.AccessKey = &quot;日本語テキスト&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=empty</code> | <code>c.AccessKey = &quot;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.AccessKey=k</code> | <code>c.AccessKey = &quot;k&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; accesskey=&quot;k&quot; /&gt;</code> |  |
| <code>.AppRelativeTemplateSourceDirectory=text</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;probe&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=markup</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=japanese</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;日本語テキスト&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=empty</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.BackColor=Red</code> | <code>c.BackColor = Color.Red;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;background-color:Red;&quot; /&gt;</code> |  |
| <code>.BackColor=#3366CC</code> | <code>c.BackColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;background-color:#3366CC;&quot; /&gt;</code> |  |
| <code>.BackColor=Transparent</code> | <code>c.BackColor = Color.Transparent;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;background-color:Transparent;&quot; /&gt;</code> |  |
| <code>.BorderColor=Red</code> | <code>c.BorderColor = Color.Red;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;border-color:Red;&quot; /&gt;</code> |  |
| <code>.BorderColor=#3366CC</code> | <code>c.BorderColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;border-color:#3366CC;&quot; /&gt;</code> |  |
| <code>.BorderColor=Transparent</code> | <code>c.BorderColor = Color.Transparent;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;border-color:Transparent;&quot; /&gt;</code> |  |
| <code>.BorderStyle=NotSet</code> | <code>c.BorderStyle = BorderStyle.NotSet;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.BorderStyle=None</code> | <code>c.BorderStyle = BorderStyle.None;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;border-style:None;&quot; /&gt;</code> |  |
| <code>.BorderStyle=Dotted</code> | <code>c.BorderStyle = BorderStyle.Dotted;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;border-style:Dotted;&quot; /&gt;</code> |  |
| <code>.BorderStyle=Dashed</code> | <code>c.BorderStyle = BorderStyle.Dashed;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;border-style:Dashed;&quot; /&gt;</code> |  |
| <code>.BorderStyle=Solid</code> | <code>c.BorderStyle = BorderStyle.Solid;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;border-style:Solid;&quot; /&gt;</code> |  |
| <code>.BorderStyle=Double</code> | <code>c.BorderStyle = BorderStyle.Double;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;border-style:Double;&quot; /&gt;</code> |  |
| <code>.BorderStyle=Groove</code> | <code>c.BorderStyle = BorderStyle.Groove;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;border-style:Groove;&quot; /&gt;</code> |  |
| <code>.BorderStyle=Ridge</code> | <code>c.BorderStyle = BorderStyle.Ridge;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;border-style:Ridge;&quot; /&gt;</code> |  |
| <code>.BorderStyle=Inset</code> | <code>c.BorderStyle = BorderStyle.Inset;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;border-style:Inset;&quot; /&gt;</code> |  |
| <code>.BorderStyle=Outset</code> | <code>c.BorderStyle = BorderStyle.Outset;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;border-style:Outset;&quot; /&gt;</code> |  |
| <code>.BorderWidth=120px</code> | <code>c.BorderWidth = Unit.Pixel(120);</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;border-width:120px;border-style:solid;&quot; /&gt;</code> |  |
| <code>.BorderWidth=50%</code> | <code>c.BorderWidth = Unit.Percentage(50);</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> | <code>c.BorderWidth = Unit.Percentage(50); -&gt; System.ArgumentOutOfRangeException: BorderWidth must be a non-negative number and cannot be a percentage. [param value]</code> |
| <code>.BorderWidth=2em</code> | <code>c.BorderWidth = new Unit(2, UnitType.Em);</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;border-width:2em;border-style:solid;&quot; /&gt;</code> |  |
| <code>.CausesValidation=true</code> | <code>c.CausesValidation = true;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.CausesValidation=false</code> | <code>c.CausesValidation = false;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.ClientIDMode=Inherit</code> | <code>c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.ClientIDMode=AutoID</code> | <code>c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.ClientIDMode=Predictable</code> | <code>c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.ClientIDMode=Static</code> | <code>c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.CommandArgument=text</code> | <code>c.CommandArgument = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.CommandArgument=markup</code> | <code>c.CommandArgument = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.CommandArgument=japanese</code> | <code>c.CommandArgument = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.CommandArgument=empty</code> | <code>c.CommandArgument = &quot;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.CommandName=text</code> | <code>c.CommandName = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.CommandName=markup</code> | <code>c.CommandName = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.CommandName=japanese</code> | <code>c.CommandName = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.CommandName=empty</code> | <code>c.CommandName = &quot;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.CssClass=text</code> | <code>c.CssClass = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; class=&quot;probe&quot; /&gt;</code> |  |
| <code>.CssClass=markup</code> | <code>c.CssClass = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; class=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; /&gt;</code> |  |
| <code>.CssClass=japanese</code> | <code>c.CssClass = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; class=&quot;日本語テキスト&quot; /&gt;</code> |  |
| <code>.CssClass=empty</code> | <code>c.CssClass = &quot;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.EnableTheming=true</code> | <code>c.EnableTheming = true;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.EnableTheming=false</code> | <code>c.EnableTheming = false;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.EnableViewState=true</code> | <code>c.EnableViewState = true;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.EnableViewState=false</code> | <code>c.EnableViewState = false;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.Enabled=true</code> | <code>c.Enabled = true;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.Enabled=false</code> | <code>c.Enabled = false;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; disabled=&quot;disabled&quot; class=&quot;aspNetDisabled&quot; /&gt;</code> |  |
| <code>.Font.Bold=true</code> | <code>c.Font.Bold = true;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;font-weight:bold;&quot; /&gt;</code> |  |
| <code>.Font.Bold=false</code> | <code>c.Font.Bold = false;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;font-weight:normal;&quot; /&gt;</code> |  |
| <code>.Font.Italic=true</code> | <code>c.Font.Italic = true;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;font-style:italic;&quot; /&gt;</code> |  |
| <code>.Font.Italic=false</code> | <code>c.Font.Italic = false;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;font-style:normal;&quot; /&gt;</code> |  |
| <code>.Font.Name=text</code> | <code>c.Font.Name = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;font-family:probe;&quot; /&gt;</code> |  |
| <code>.Font.Name=markup</code> | <code>c.Font.Name = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;font-family:&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;;&quot; /&gt;</code> |  |
| <code>.Font.Name=japanese</code> | <code>c.Font.Name = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;font-family:日本語テキスト;&quot; /&gt;</code> |  |
| <code>.Font.Name=empty</code> | <code>c.Font.Name = &quot;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.Font.Names=Arial,MS-Gothic</code> | <code>c.Font.Names = new[] { &quot;Arial&quot;, &quot;MS Gothic&quot; };</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;font-family:Arial,MS Gothic;&quot; /&gt;</code> |  |
| <code>.Font.Overline=true</code> | <code>c.Font.Overline = true;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;text-decoration: overline;&quot; /&gt;</code> |  |
| <code>.Font.Overline=false</code> | <code>c.Font.Overline = false;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;text-decoration:none;&quot; /&gt;</code> |  |
| <code>.Font.Size=12pt</code> | <code>c.Font.Size = FontUnit.Point(12);</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;font-size:12pt;&quot; /&gt;</code> |  |
| <code>.Font.Size=Large</code> | <code>c.Font.Size = FontUnit.Large;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;font-size:Large;&quot; /&gt;</code> |  |
| <code>.Font.Size=150%</code> | <code>c.Font.Size = new FontUnit(Unit.Percentage(150));</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;font-size:150%;&quot; /&gt;</code> |  |
| <code>.Font.Strikeout=true</code> | <code>c.Font.Strikeout = true;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;text-decoration: line-through;&quot; /&gt;</code> |  |
| <code>.Font.Strikeout=false</code> | <code>c.Font.Strikeout = false;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;text-decoration:none;&quot; /&gt;</code> |  |
| <code>.Font.Underline=true</code> | <code>c.Font.Underline = true;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;text-decoration:underline;&quot; /&gt;</code> |  |
| <code>.Font.Underline=false</code> | <code>c.Font.Underline = false;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;text-decoration:none;&quot; /&gt;</code> |  |
| <code>.ForeColor=Red</code> | <code>c.ForeColor = Color.Red;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;color:Red;&quot; /&gt;</code> |  |
| <code>.ForeColor=#3366CC</code> | <code>c.ForeColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;color:#3366CC;&quot; /&gt;</code> |  |
| <code>.ForeColor=Transparent</code> | <code>c.ForeColor = Color.Transparent;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;color:Transparent;&quot; /&gt;</code> |  |
| <code>.Height=120px</code> | <code>c.Height = Unit.Pixel(120);</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;height:120px;&quot; /&gt;</code> |  |
| <code>.Height=50%</code> | <code>c.Height = Unit.Percentage(50);</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;height:50%;&quot; /&gt;</code> |  |
| <code>.Height=2em</code> | <code>c.Height = new Unit(2, UnitType.Em);</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;height:2em;&quot; /&gt;</code> |  |
| <code>.ID=text</code> | <code>c.ID = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;probe&quot; value=&quot;&quot; id=&quot;probe&quot; /&gt;</code> |  |
| <code>.ID=markup</code> | <code>c.ID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; value=&quot;&quot; id=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; /&gt;</code> |  |
| <code>.ID=japanese</code> | <code>c.ID = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;日本語テキスト&quot; value=&quot;&quot; id=&quot;日本語テキスト&quot; /&gt;</code> |  |
| <code>.ID=empty</code> | <code>c.ID = &quot;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;ctl02&quot; value=&quot;&quot; id=&quot;ctl02&quot; /&gt;</code> |  |
| <code>.OnClientClick=text</code> | <code>c.OnClientClick = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; onclick=&quot;probe;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.OnClientClick=markup</code> | <code>c.OnClientClick = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; onclick=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.OnClientClick=japanese</code> | <code>c.OnClientClick = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; onclick=&quot;日本語テキスト;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.OnClientClick=empty</code> | <code>c.OnClientClick = &quot;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.OnClientClick=confirm</code> | <code>c.OnClientClick = &quot;return confirm(&#39;OK?&#39;);&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; onclick=&quot;return confirm(&#39;OK?&#39;);&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.PostBackUrl=text</code> | <code>c.PostBackUrl = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; onclick=&quot;javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;btnGo&quot;, &quot;&quot;, false, &quot;&quot;, &quot;probe&quot;, false, false))&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.PostBackUrl=markup</code> | <code>c.PostBackUrl = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; onclick=&quot;javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;btnGo&quot;, &quot;&quot;, false, &quot;&quot;, &quot;&lt;b&gt;\&quot;&amp;\&#39;&lt;/b&gt;&quot;, false, false))&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.PostBackUrl=japanese</code> | <code>c.PostBackUrl = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; onclick=&quot;javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;btnGo&quot;, &quot;&quot;, false, &quot;&quot;, &quot;%e6%97%a5%e6%9c%ac%e8%aa%9e%e3%83%86%e3%82%ad%e3%82%b9%e3%83%88&quot;, false, false))&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.PostBackUrl=empty</code> | <code>c.PostBackUrl = &quot;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.PostBackUrl=app-relative</code> | <code>c.PostBackUrl = &quot;~/ButtonTarget.aspx&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; onclick=&quot;javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;btnGo&quot;, &quot;&quot;, false, &quot;&quot;, &quot;ButtonTarget.aspx&quot;, false, false))&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.PostBackUrl=absolute</code> | <code>c.PostBackUrl = &quot;http://example.com/target.aspx?a=1&amp;b=2&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; onclick=&quot;javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;btnGo&quot;, &quot;&quot;, false, &quot;&quot;, &quot;http://example.com/target.aspx?a=1&amp;b=2&quot;, false, false))&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.SkinID=text</code> | <code>c.SkinID = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.SkinID=markup</code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.SkinID=japanese</code> | <code>c.SkinID = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.SkinID=empty</code> | <code>c.SkinID = &quot;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.TabIndex=0</code> | <code>c.TabIndex = 0;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.TabIndex=3</code> | <code>c.TabIndex = 3;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; tabindex=&quot;3&quot; /&gt;</code> |  |
| <code>.TabIndex=-1</code> | <code>c.TabIndex = -1;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; tabindex=&quot;-1&quot; /&gt;</code> |  |
| <code>.Text=text</code> | <code>c.Text = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;probe&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.Text=markup</code> | <code>c.Text = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.Text=japanese</code> | <code>c.Text = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;日本語テキスト&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.Text=empty</code> | <code>c.Text = &quot;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.ToolTip=text</code> | <code>c.ToolTip = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; title=&quot;probe&quot; /&gt;</code> |  |
| <code>.ToolTip=markup</code> | <code>c.ToolTip = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; title=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; /&gt;</code> |  |
| <code>.ToolTip=japanese</code> | <code>c.ToolTip = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; title=&quot;日本語テキスト&quot; /&gt;</code> |  |
| <code>.ToolTip=empty</code> | <code>c.ToolTip = &quot;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.UseSubmitBehavior=true</code> | <code>c.UseSubmitBehavior = true;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.UseSubmitBehavior=false</code> | <code>c.UseSubmitBehavior = false;</code> | <code>&lt;input type=&quot;button&quot; name=&quot;btnGo&quot; value=&quot;&quot; onclick=&quot;javascript:__doPostBack(&#39;btnGo&#39;,&#39;&#39;)&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.ValidateRequestMode=Inherit</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Inherit;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.ValidateRequestMode=Disabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Disabled;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.ValidateRequestMode=Enabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Enabled;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.ValidationGroup=text</code> | <code>c.ValidationGroup = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.ValidationGroup=markup</code> | <code>c.ValidationGroup = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.ValidationGroup=japanese</code> | <code>c.ValidationGroup = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.ValidationGroup=empty</code> | <code>c.ValidationGroup = &quot;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.ViewStateMode=Inherit</code> | <code>c.ViewStateMode = ViewStateMode.Inherit;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.ViewStateMode=Enabled</code> | <code>c.ViewStateMode = ViewStateMode.Enabled;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.ViewStateMode=Disabled</code> | <code>c.ViewStateMode = ViewStateMode.Disabled;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.Visible=true</code> | <code>c.Visible = true;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.Visible=false</code> | <code>c.Visible = false;</code> | <code></code> |  |
| <code>.Width=120px</code> | <code>c.Width = Unit.Pixel(120);</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;width:120px;&quot; /&gt;</code> |  |
| <code>.Width=50%</code> | <code>c.Width = Unit.Percentage(50);</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;width:50%;&quot; /&gt;</code> |  |
| <code>.Width=2em</code> | <code>c.Width = new Unit(2, UnitType.Em);</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;width:2em;&quot; /&gt;</code> |  |
| <code>.Attributes[data-probe]=text</code> | <code>c.Attributes[data-probe] = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; data-probe=&quot;probe&quot; /&gt;</code> |  |
| <code>.Attributes[onfocus]=text</code> | <code>c.Attributes[onfocus] = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; onfocus=&quot;probe&quot; /&gt;</code> |  |
| <code>.Attributes[data-probe]=markup</code> | <code>c.Attributes[data-probe] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; data-probe=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; /&gt;</code> |  |
| <code>.Attributes[onfocus]=markup</code> | <code>c.Attributes[onfocus] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; onfocus=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; /&gt;</code> |  |
| <code>.Style[color]=red</code> | <code>c.Style[color] = &quot;red&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;color:red;&quot; /&gt;</code> |  |
| <code>.Style[margin-left]=red</code> | <code>c.Style[margin-left] = &quot;red&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;margin-left:red;&quot; /&gt;</code> |  |
| <code>.Style[color]=1em</code> | <code>c.Style[color] = &quot;1em&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;color:1em;&quot; /&gt;</code> |  |
| <code>.Style[margin-left]=1em</code> | <code>c.Style[margin-left] = &quot;1em&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;margin-left:1em;&quot; /&gt;</code> |  |
| <code>.UseSubmitBehavior=false+Text=text</code> | <code>c.UseSubmitBehavior = false; c.Text = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;button&quot; name=&quot;btnGo&quot; value=&quot;probe&quot; onclick=&quot;javascript:__doPostBack(&#39;btnGo&#39;,&#39;&#39;)&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.UseSubmitBehavior=false+CausesValidation=false</code> | <code>c.UseSubmitBehavior = false; c.CausesValidation = false;</code> | <code>&lt;input type=&quot;button&quot; name=&quot;btnGo&quot; value=&quot;&quot; onclick=&quot;javascript:__doPostBack(&#39;btnGo&#39;,&#39;&#39;)&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.UseSubmitBehavior=false+ValidationGroup=text</code> | <code>c.UseSubmitBehavior = false; c.ValidationGroup = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;button&quot; name=&quot;btnGo&quot; value=&quot;&quot; onclick=&quot;javascript:__doPostBack(&#39;btnGo&#39;,&#39;&#39;)&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.UseSubmitBehavior=false+OnClientClick=confirm</code> | <code>c.UseSubmitBehavior = false; c.OnClientClick = &quot;return confirm(&#39;OK?&#39;);&quot;;</code> | <code>&lt;input type=&quot;button&quot; name=&quot;btnGo&quot; value=&quot;&quot; onclick=&quot;return confirm(&#39;OK?&#39;);__doPostBack(&#39;btnGo&#39;,&#39;&#39;)&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.UseSubmitBehavior=false+PostBackUrl=app-relative</code> | <code>c.UseSubmitBehavior = false; c.PostBackUrl = &quot;~/ButtonTarget.aspx&quot;;</code> | <code>&lt;input type=&quot;button&quot; name=&quot;btnGo&quot; value=&quot;&quot; onclick=&quot;javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;btnGo&quot;, &quot;&quot;, false, &quot;&quot;, &quot;ButtonTarget.aspx&quot;, false, true))&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.UseSubmitBehavior=false+Enabled=false</code> | <code>c.UseSubmitBehavior = false; c.Enabled = false;</code> | <code>&lt;input type=&quot;button&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; disabled=&quot;disabled&quot; class=&quot;aspNetDisabled&quot; /&gt;</code> |  |
| <code>.UseSubmitBehavior=false+Visible=false</code> | <code>c.UseSubmitBehavior = false; c.Visible = false;</code> | <code></code> |  |
| <code>.OnClientClick=confirm+CausesValidation=false</code> | <code>c.OnClientClick = &quot;return confirm(&#39;OK?&#39;);&quot;; c.CausesValidation = false;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; onclick=&quot;return confirm(&#39;OK?&#39;);&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.OnClientClick=confirm+ValidationGroup=text</code> | <code>c.OnClientClick = &quot;return confirm(&#39;OK?&#39;);&quot;; c.ValidationGroup = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; onclick=&quot;return confirm(&#39;OK?&#39;);&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.OnClientClick=confirm+Enabled=false</code> | <code>c.OnClientClick = &quot;return confirm(&#39;OK?&#39;);&quot;; c.Enabled = false;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; disabled=&quot;disabled&quot; class=&quot;aspNetDisabled&quot; /&gt;</code> |  |
| <code>.PostBackUrl=app-relative+ValidationGroup=text</code> | <code>c.PostBackUrl = &quot;~/ButtonTarget.aspx&quot;; c.ValidationGroup = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; onclick=&quot;javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;btnGo&quot;, &quot;&quot;, false, &quot;&quot;, &quot;ButtonTarget.aspx&quot;, false, false))&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.PostBackUrl=app-relative+CausesValidation=false</code> | <code>c.PostBackUrl = &quot;~/ButtonTarget.aspx&quot;; c.CausesValidation = false;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; onclick=&quot;javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;btnGo&quot;, &quot;&quot;, false, &quot;&quot;, &quot;ButtonTarget.aspx&quot;, false, false))&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.PostBackUrl=app-relative+OnClientClick=confirm</code> | <code>c.PostBackUrl = &quot;~/ButtonTarget.aspx&quot;; c.OnClientClick = &quot;return confirm(&#39;OK?&#39;);&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; onclick=&quot;return confirm(&#39;OK?&#39;);WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;btnGo&quot;, &quot;&quot;, false, &quot;&quot;, &quot;ButtonTarget.aspx&quot;, false, false))&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.PostBackUrl=app-relative+Enabled=false</code> | <code>c.PostBackUrl = &quot;~/ButtonTarget.aspx&quot;; c.Enabled = false;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; disabled=&quot;disabled&quot; class=&quot;aspNetDisabled&quot; /&gt;</code> |  |
| <code>.CommandName=text+CommandArgument=markup</code> | <code>c.CommandName = &quot;probe&quot;; c.CommandArgument = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>.Enabled=false+CssClass=text</code> | <code>c.Enabled = false; c.CssClass = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; disabled=&quot;disabled&quot; class=&quot;aspNetDisabled probe&quot; /&gt;</code> |  |
| <code>.Text=markup+ToolTip=markup</code> | <code>c.Text = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; c.ToolTip = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; id=&quot;btnGo&quot; title=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; /&gt;</code> |  |
| <code>.Text=text+Visible=false</code> | <code>c.Text = &quot;probe&quot;; c.Visible = false;</code> | <code></code> |  |
| <code>.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | <code>c.Width = Unit.Pixel(120); c.Height = new Unit(2, UnitType.Em); c.BorderWidth = Unit.Pixel(120); c.BorderStyle = BorderStyle.Solid; c.BackColor = Color.FromArgb(0x33, 0x66, 0xCC); c.ForeColor = Color.Red; c.Font.Bold = true;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;color:Red;background-color:#3366CC;border-width:120px;border-style:Solid;font-weight:bold;height:2em;width:120px;&quot; /&gt;</code> |  |
| <code>.ForeColor=#3366CC+Style[color]=red</code> | <code>c.ForeColor = Color.FromArgb(0x33, 0x66, 0xCC); c.Style[color] = &quot;red&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; style=&quot;color:#3366CC;color:red;&quot; /&gt;</code> |  |
| <code>.Text=markup+EnableViewState=false</code> | <code>c.Text = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; c.EnableViewState = false;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>[naming container].(default)</code> | <code>[naming container] (nothing set)</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input type=&quot;submit&quot; name=&quot;box$btnGo&quot; value=&quot;&quot; id=&quot;box_btnGo&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=AutoID</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input type=&quot;submit&quot; name=&quot;box$btnGo&quot; value=&quot;&quot; id=&quot;box_btnGo&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Static</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input type=&quot;submit&quot; name=&quot;box$btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Predictable</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input type=&quot;submit&quot; name=&quot;box$btnGo&quot; value=&quot;&quot; id=&quot;box_btnGo&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Inherit</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input type=&quot;submit&quot; name=&quot;box$btnGo&quot; value=&quot;&quot; id=&quot;box_btnGo&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].UseSubmitBehavior=false</code> | <code>[naming container] c.UseSubmitBehavior = false;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input type=&quot;button&quot; name=&quot;box$btnGo&quot; value=&quot;&quot; onclick=&quot;javascript:__doPostBack(&#39;box$btnGo&#39;,&#39;&#39;)&quot; id=&quot;box_btnGo&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].PostBackUrl=app-relative</code> | <code>[naming container] c.PostBackUrl = &quot;~/ButtonTarget.aspx&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input type=&quot;submit&quot; name=&quot;box$btnGo&quot; value=&quot;&quot; onclick=&quot;javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;box$btnGo&quot;, &quot;&quot;, false, &quot;&quot;, &quot;ButtonTarget.aspx&quot;, false, false))&quot; id=&quot;box_btnGo&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ValidationGroup=text</code> | <code>[naming container] c.ValidationGroup = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input type=&quot;submit&quot; name=&quot;box$btnGo&quot; value=&quot;&quot; id=&quot;box_btnGo&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ID=text</code> | <code>[naming container] c.ID = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input type=&quot;submit&quot; name=&quot;box$probe&quot; value=&quot;&quot; id=&quot;box_probe&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[themed].(default)</code> | <code>[themed] (nothing set)</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; class=&quot;skinned&quot; style=&quot;background-color:#EEEEEE;&quot; /&gt;</code> |  |
| <code>[themed].SkinID=text</code> | <code>[themed] c.SkinID = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; title=&quot;from the skin&quot; class=&quot;skinned-probe&quot; style=&quot;width:300px;&quot; /&gt;</code> |  |
| <code>[themed].EnableTheming=false</code> | <code>[themed] c.EnableTheming = false;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; /&gt;</code> |  |
| <code>[themed].CssClass=text</code> | <code>[themed] c.CssClass = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; class=&quot;skinned&quot; style=&quot;background-color:#EEEEEE;&quot; /&gt;</code> |  |
| <code>[themed].Width=120px</code> | <code>[themed] c.Width = Unit.Pixel(120);</code> | <code>&lt;input type=&quot;submit&quot; name=&quot;btnGo&quot; value=&quot;&quot; id=&quot;btnGo&quot; class=&quot;skinned&quot; style=&quot;background-color:#EEEEEE;width:120px;&quot; /&gt;</code> |  |

## ③ シナリオ層

ブラウザ（Playwright）で ButtonScenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。

| # | 操作 | 対象 | 値 |
|---|---|---|---|
| 1 | goto | /ButtonScenario.aspx | <code></code> |
| 2 | snapshot |  | **Button [render][databind][disabled] first request** |
| 3 | click | btnSubmit | <code></code> |
| 4 | snapshot |  | **Button [postbackevent][command][viewstate] submit button: Click, then Command** |
| 5 | click | btnScript | <code></code> |
| 6 | snapshot |  | **Button [postbackevent][submitbehavior] script postback (UseSubmitBehavior=false)** |
| 7 | click | btnConfirmNo | <code></code> |
| 8 | snapshot |  | **Button [clientclick] OnClientClick returns false: no request** |
| 9 | click | btnConfirmYes | <code></code> |
| 10 | snapshot |  | **Button [clientclick][postbackevent] OnClientClick returns true** |
| 11 | click | btnValidate | <code></code> |
| 12 | snapshot |  | **Button [causesvalidation] group g1, empty: invalid, Click raised** |
| 13 | fill | txtRequired | <code>x</code> |
| 14 | click | btnValidate | <code></code> |
| 15 | snapshot |  | **Button [causesvalidation] group g1, filled: valid** |
| 16 | fill | txtRequired | <code></code> |
| 17 | click | btnNoValidate | <code></code> |
| 18 | snapshot |  | **Button [causesvalidation] CausesValidation=false: not validated** |
| 19 | click | btnClient | <code></code> |
| 20 | snapshot |  | **Button [causesvalidation] group g2 validated in the browser: no request** |
| 21 | click | btnPick | <code></code> |
| 22 | snapshot |  | **Button [command][databind] Command bubbled from a repeater item** |
| 23 | fill | txtDefault | <code>enter</code> |
| 24 | press | txtDefault | <code>Enter</code> |
| 25 | snapshot |  | **Button [defaultbutton] Enter in a panel with DefaultButton** |
| 26 | click | btnDynamic | <code></code> |
| 27 | snapshot |  | **Button [dynamic][postbackevent] dynamically created button** |
| 28 | click | btnShow | <code></code> |
| 29 | snapshot |  | **Button [visible][viewstate] hidden button shown, its text changed** |
| 30 | click | btnHidden | <code></code> |
| 31 | snapshot |  | **Button [visible][viewstate][postbackevent] the button shown before raises Click, its text kept** |
| 32 | fill | txtCross | <code>cross</code> |
| 33 | click | btnCross | <code></code> |
| 34 | snapshot |  | **Button [crosspage] cross-page postback (PostBackUrl)** |

<details><summary>Button [render][databind][disabled] first request（期待される本文）</summary>

```text
Required (g1):
Required (g2, in the browser):
Pen Bag
Default button:
Dynamic:
Cross-page:
Request 1: IsPostBack=False
btnHidden: hidden, Text "Hidden"
```

</details>

<details><summary>Button [postbackevent][command][viewstate] submit button: Click, then Command（期待される本文）</summary>

```text
Required (g1):
Required (g2, in the browser):
Pen Bag
Default button:
Dynamic:
Cross-page:
Request 2: IsPostBack=True
Click btnSubmit
Command btnSubmit: save 1
IsValid: (not validated)
btnHidden: hidden, Text "Hidden"
```

</details>

<details><summary>Button [postbackevent][submitbehavior] script postback (UseSubmitBehavior=false)（期待される本文）</summary>

```text
Required (g1):
Required (g2, in the browser):
Pen Bag
Default button:
Dynamic:
Cross-page:
Request 3: IsPostBack=True
Click btnScript
IsValid: (not validated)
btnHidden: hidden, Text "Hidden"
```

</details>

<details><summary>Button [clientclick] OnClientClick returns false: no request（期待される本文）</summary>

```text
Required (g1):
Required (g2, in the browser):
Pen Bag
Default button:
Dynamic:
Cross-page:
Request 3: IsPostBack=True
Click btnScript
IsValid: (not validated)
btnHidden: hidden, Text "Hidden"
```

</details>

<details><summary>Button [clientclick][postbackevent] OnClientClick returns true（期待される本文）</summary>

```text
Required (g1):
Required (g2, in the browser):
Pen Bag
Default button:
Dynamic:
Cross-page:
Request 4: IsPostBack=True
Click btnConfirmYes
IsValid: (not validated)
btnHidden: hidden, Text "Hidden"
```

</details>

<details><summary>Button [causesvalidation] group g1, empty: invalid, Click raised（期待される本文）</summary>

```text
Required (g1): Required (g1)
Required (g2, in the browser):
Pen Bag
Default button:
Dynamic:
Cross-page:
Request 5: IsPostBack=True
Click btnValidate
IsValid: False
btnHidden: hidden, Text "Hidden"
```

</details>

<details><summary>Button [causesvalidation] group g1, filled: valid（期待される本文）</summary>

```text
Required (g1):
Required (g2, in the browser):
Pen Bag
Default button:
Dynamic:
Cross-page:
Request 6: IsPostBack=True
Click btnValidate
IsValid: True
btnHidden: hidden, Text "Hidden"
```

</details>

<details><summary>Button [causesvalidation] CausesValidation=false: not validated（期待される本文）</summary>

```text
Required (g1):
Required (g2, in the browser):
Pen Bag
Default button:
Dynamic:
Cross-page:
Request 7: IsPostBack=True
Click btnNoValidate
IsValid: (not validated)
btnHidden: hidden, Text "Hidden"
```

</details>

<details><summary>Button [causesvalidation] group g2 validated in the browser: no request（期待される本文）</summary>

```text
Required (g1):
Required (g2, in the browser): Required (g2, in the browser)
Pen Bag
Default button:
Dynamic:
Cross-page:
Request 7: IsPostBack=True
Click btnNoValidate
IsValid: (not validated)
btnHidden: hidden, Text "Hidden"
```

</details>

<details><summary>Button [command][databind] Command bubbled from a repeater item（期待される本文）</summary>

```text
Required (g1):
Required (g2, in the browser):
Pen Bag
Default button:
Dynamic:
Cross-page:
Request 8: IsPostBack=True
ItemCommand rptItems item 0: pick Pen (from btnPick)
IsValid: (not validated)
btnHidden: hidden, Text "Hidden"
```

</details>

<details><summary>Button [defaultbutton] Enter in a panel with DefaultButton（期待される本文）</summary>

```text
Required (g1):
Required (g2, in the browser):
Pen Bag
Default button:
Dynamic:
Cross-page:
Request 9: IsPostBack=True
Click btnDefault
IsValid: (not validated)
btnHidden: hidden, Text "Hidden"
```

</details>

<details><summary>Button [dynamic][postbackevent] dynamically created button（期待される本文）</summary>

```text
Required (g1):
Required (g2, in the browser):
Pen Bag
Default button:
Dynamic:
Cross-page:
Request 10: IsPostBack=True
Click btnDynamic
IsValid: (not validated)
btnHidden: hidden, Text "Hidden"
```

</details>

<details><summary>Button [visible][viewstate] hidden button shown, its text changed（期待される本文）</summary>

```text
Required (g1):
Required (g2, in the browser):
Pen Bag
Default button:
Dynamic:
Cross-page:
Request 11: IsPostBack=True
Click btnShow
IsValid: (not validated)
btnHidden: visible, Text "Shown"
```

</details>

<details><summary>Button [visible][viewstate][postbackevent] the button shown before raises Click, its text kept（期待される本文）</summary>

```text
Required (g1):
Required (g2, in the browser):
Pen Bag
Default button:
Dynamic:
Cross-page:
Request 12: IsPostBack=True
Click btnHidden
IsValid: (not validated)
btnHidden: visible, Text "Shown"
```

</details>

<details><summary>Button [crosspage] cross-page postback (PostBackUrl)（期待される本文）</summary>

```text
IsPostBack=False
IsCrossPagePostBack (this page)=False
PreviousPage: ~/ButtonScenario.aspx, IsCrossPagePostBack=True, IsPostBack=True
PreviousPage txtCross: "cross"
```

</details>

