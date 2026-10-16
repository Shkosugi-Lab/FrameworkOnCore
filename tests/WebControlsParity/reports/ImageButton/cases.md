# ImageButton テストケース一覧

変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。
このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。

- 正解データの採取: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0
- ① API 層: 188 ケース（197 メンバー）
- ② HTML 層: 179 ケース
- ③ シナリオ層: 10 スナップショット

## 機構マトリクス

型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。

| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |
|---|---|---|---|
| プロパティ → HTML 描画 | すべてのコントロール | 166 | 1 |
| ViewState の保存と復元 | Control | 6 | 2 |
| Visible=false（描画されず状態は保持） | Control.Visible | 2 | 1 |
| 名前付けコンテナと ClientIDMode | Control.ClientIDMode | 8 | 0 |
| テーマとスキン（SkinID / EnableTheming） | テーマを付けられる型（[Themeable]） | 4 | 0 |
| 動的に生成したコントロールのライフサイクル | Control | 0 | 2 |
| データバインド（DataBind / <%# %>） | Control.DataBind | 0 | 2 |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | WebControl.Enabled、HtmlControl.Disabled | 5 | 0 |
| ポストバックデータの取り込み | IPostBackDataHandler | 0 | 1 |
| リクエスト検証（ValidateRequestMode） | IPostBackDataHandler（ポスト値を読む） | 3 | 1 |
| ポストバックイベント | IPostBackEventHandler | 0 | 4 |
| コマンドイベントと親へのバブル（CommandName） | IButtonControl | 9 | 2 |
| ブラウザ側のクリック処理（OnClientClick） | OnClientClick プロパティ | 9 | 1 |
| クロスページポストバック（PostBackUrl） | IButtonControl.PostBackUrl | 11 | 1 |
| 既定のボタン（Panel の DefaultButton） | IButtonControl | 0 | 1 |
| CausesValidation / ValidationGroup | CausesValidation プロパティ | 11 | 1 |

該当しない機構: 変更イベント（*Changed、ServerChange）、ReadOnly（ポストされた値を無視）、送信ボタン／スクリプトによるポストバック（UseSubmitBehavior）、AutoPostBack（クライアントスクリプトによるポストバック）、検証対象（サーバー側の検証）、検証対象（ブラウザ側の検証）、双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値）、項目（ListItem の Selected・Enabled・Text・Value と ViewState）、複数選択、繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）、ポスト値のイベント検証（一覧にない値・無効な項目の拒否）、サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）、ブラウザ側の検証スクリプト（EnableClientScript）、検証結果のまとめ（ValidationSummary）、ページング、並べ替え、項目の挿入、行・項目の編集（編集・更新・取り消し・削除）、行・項目の選択、行・項目のコマンド（RowCommand・ItemCommand へのバブル）、データキー（DataKeyNames・DataKeys）、データソースコントロールとの連携（DataSourceID）、テンプレート、データソース連携（DataSource・DataTextField など）、クライアントコールバック

## ① API 層

ImageButton とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の ImageButton インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。

| ケース | 期待値（.NET Framework） |
|---|---|
| <code>Api.Control.#ctor</code> | <code>Control.#ctor = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMode=In…</code> |
| <code>Api.Control.Adapter</code> | <code>Control.Adapter = null</code> |
| <code>Api.Control.AddParsedSubObject</code> | <code>Control.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>Control.AddParsedSubObject(Object) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000…</code> |
| <code>Api.Control.AddedControl</code> | <code>Control.AddedControl(Control,Int32) = &quot;ok&quot;</code><br><code>Control.AddedControl(Control,Int32) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #0000…</code> |
| <code>Api.Control.AppRelativeTemplateSourceDirectory</code> | <code>Control.AppRelativeTemplateSourceDirectory = &quot;&quot;</code><br><code>Control.AppRelativeTemplateSourceDirectory set = &quot;-&gt; System.ArgumentException: The relative virtual path &#39;Abc 123&#39; is not allowed here.&quot;</code> |
| <code>Api.Control.ApplyStyleSheetSkin</code> | <code>Control.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>Control.ApplyStyleSheetSkin(Page) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #000000…</code> |
| <code>Api.Control.BeginRenderTracing</code> | <code>Control.BeginRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.BeginRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.BeginRenderTracing(TextWriter,Object) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=C…</code> |
| <code>Api.Control.BindingContainer</code> | <code>Control.BindingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Clien…</code> |
| <code>Api.Control.BuildProfileTree</code> | <code>Control.BuildProfileTree(String,Boolean) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.ChildControlsCreated</code> | <code>Control.ChildControlsCreated = &quot;false&quot;</code><br><code>Control.ChildControlsCreated set = &quot;ok&quot;</code> |
| <code>Api.Control.ClearCachedClientID</code> | <code>Control.ClearCachedClientID = &quot;ok&quot;</code><br><code>Control.ClearCachedClientID after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Col…</code> |
| <code>Api.Control.ClearChildControlState</code> | <code>Control.ClearChildControlState = &quot;ok&quot;</code><br><code>Control.ClearChildControlState after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 …</code> |
| <code>Api.Control.ClearChildState</code> | <code>Control.ClearChildState = &quot;ok&quot;</code><br><code>Control.ClearChildState after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code> |
| <code>Api.Control.ClearChildViewState</code> | <code>Control.ClearChildViewState = &quot;ok&quot;</code><br><code>Control.ClearChildViewState after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Col…</code> |
| <code>Api.Control.ClearEffectiveClientIDMode</code> | <code>Control.ClearEffectiveClientIDMode = &quot;ok&quot;</code><br><code>Control.ClearEffectiveClientIDMode after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000…</code> |
| <code>Api.Control.ClientID</code> | <code>Control.ClientID = &quot;ibtnGo&quot;</code> |
| <code>Api.Control.ClientIDMode</code> | <code>Control.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>Control.ClientIDMode set = &quot;ClientIDMode.AutoID&quot;</code> |
| <code>Api.Control.ClientIDSeparator</code> | <code>Control.ClientIDSeparator = &quot;&#39;_&#39;&quot;</code> |
| <code>Api.Control.Context</code> | <code>Control.Context = null</code> |
| <code>Api.Control.Controls</code> | <code>Control.Controls = ControlCollection []</code> |
| <code>Api.Control.CreateChildControls</code> | <code>Control.CreateChildControls = &quot;ok&quot;</code><br><code>Control.CreateChildControls after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Col…</code> |
| <code>Api.Control.CreateControlCollection</code> | <code>Control.CreateControlCollection = ControlCollection []</code><br><code>Control.CreateControlCollection after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000…</code> |
| <code>Api.Control.DataBind</code> | <code>Control.DataBind = &quot;ok&quot;</code><br><code>Control.DataBind after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty];…</code><br><code>Control.DataBind(Boolean) = &quot;ok&quot;</code><br><code>Control.DataBind(Boolean) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color…</code> |
| <code>Api.Control.DataBindChildren</code> | <code>Control.DataBindChildren = &quot;ok&quot;</code><br><code>Control.DataBindChildren after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color …</code> |
| <code>Api.Control.DataBinding</code> | <code>Control.DataBinding = &quot;added and removed&quot;</code> |
| <code>Api.Control.DataItemContainer</code> | <code>Control.DataItemContainer = null</code> |
| <code>Api.Control.DataKeysContainer</code> | <code>Control.DataKeysContainer = null</code> |
| <code>Api.Control.DesignMode</code> | <code>Control.DesignMode = &quot;false&quot;</code> |
| <code>Api.Control.Dispose</code> | <code>Control.Dispose = &quot;ok&quot;</code><br><code>Control.Dispose after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; …</code> |
| <code>Api.Control.Disposed</code> | <code>Control.Disposed = &quot;added and removed&quot;</code> |
| <code>Api.Control.EnableTheming</code> | <code>Control.EnableTheming = &quot;true&quot;</code><br><code>Control.EnableTheming set = &quot;true&quot;</code> |
| <code>Api.Control.EnableViewState</code> | <code>Control.EnableViewState = &quot;true&quot;</code><br><code>Control.EnableViewState set = &quot;true&quot;</code> |
| <code>Api.Control.EndRenderTracing</code> | <code>Control.EndRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.EndRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.EndRenderTracing(TextWriter,Object) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Col…</code> |
| <code>Api.Control.EnsureChildControls</code> | <code>Control.EnsureChildControls = &quot;ok&quot;</code><br><code>Control.EnsureChildControls after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Col…</code> |
| <code>Api.Control.EnsureID</code> | <code>Control.EnsureID = &quot;ok&quot;</code><br><code>Control.EnsureID after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty];…</code> |
| <code>Api.Control.Events</code> | <code>Control.Events = &quot;System.ComponentModel.EventHandlerList&quot;</code> |
| <code>Api.Control.FindControl</code> | <code>Control.FindControl(String) = Label#lblTitle {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code><br><code>Control.FindControl(String) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Col…</code><br><code>Control.FindControl(String,Int32) = null</code><br><code>Control.FindControl(String,Int32) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #000000…</code> |
| <code>Api.Control.Focus</code> | <code>Control.Focus = &quot;-&gt; System.InvalidOperationException: A form tag with runat=server must exist on the Page to use SetFocus() or the Focus property.&quot;</code> |
| <code>Api.Control.GetDesignModeState</code> | <code>Control.GetDesignModeState = &quot;[]&quot;</code><br><code>Control.GetDesignModeState after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Colo…</code> |
| <code>Api.Control.GetRouteUrl</code> | <code>Control.GetRouteUrl(Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.GetUniqueIDRelativeTo</code> | <code>Control.GetUniqueIDRelativeTo(Control) = &quot;ibtnGo&quot;</code><br><code>Control.GetUniqueIDRelativeTo(Control) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #0…</code> |
| <code>Api.Control.HasChildViewState</code> | <code>Control.HasChildViewState = &quot;false&quot;</code> |
| <code>Api.Control.HasControls</code> | <code>Control.HasControls = &quot;false&quot;</code><br><code>Control.HasControls after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empt…</code> |
| <code>Api.Control.HasEvents</code> | <code>Control.HasEvents = &quot;false&quot;</code><br><code>Control.HasEvents after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]…</code> |
| <code>Api.Control.ID</code> | <code>Control.ID = &quot;ibtnGo&quot;</code><br><code>Control.ID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.IdSeparator</code> | <code>Control.IdSeparator = &quot;&#39;$&#39;&quot;</code> |
| <code>Api.Control.Init</code> | <code>Control.Init = &quot;added and removed&quot;</code> |
| <code>Api.Control.IsChildControlStateCleared</code> | <code>Control.IsChildControlStateCleared = &quot;false&quot;</code> |
| <code>Api.Control.IsLiteralContent</code> | <code>Control.IsLiteralContent = &quot;false&quot;</code><br><code>Control.IsLiteralContent after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color …</code> |
| <code>Api.Control.IsTrackingViewState</code> | <code>Control.IsTrackingViewState = &quot;false&quot;</code> |
| <code>Api.Control.IsViewStateEnabled</code> | <code>Control.IsViewStateEnabled = &quot;true&quot;</code> |
| <code>Api.Control.Load</code> | <code>Control.Load = &quot;added and removed&quot;</code> |
| <code>Api.Control.LoadControlState</code> | <code>Control.LoadControlState(Object) = &quot;ok&quot;</code><br><code>Control.LoadControlState(Object) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #0000000…</code> |
| <code>Api.Control.LoadViewState</code> | <code>Control.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Web.UI.Pair&#39;.&quot;</code> |
| <code>Api.Control.LoadViewStateByID</code> | <code>Control.LoadViewStateByID = &quot;false&quot;</code> |
| <code>Api.Control.MapPathSecure</code> | <code>Control.MapPathSecure(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.NamingContainer</code> | <code>Control.NamingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code> |
| <code>Api.Control.OnBubbleEvent</code> | <code>Control.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Control.OnBubbleEvent(Object,EventArgs) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #…</code> |
| <code>Api.Control.OnDataBinding</code> | <code>Control.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnDataBinding(EventArgs) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #0000000…</code> |
| <code>Api.Control.OnInit</code> | <code>Control.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnInit(EventArgs) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color…</code> |
| <code>Api.Control.OnLoad</code> | <code>Control.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnLoad(EventArgs) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color…</code> |
| <code>Api.Control.OnPreRender</code> | <code>Control.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnPreRender(EventArgs) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 …</code> |
| <code>Api.Control.OnUnload</code> | <code>Control.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnUnload(EventArgs) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Col…</code> |
| <code>Api.Control.OpenFile</code> | <code>Control.OpenFile(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.Page</code> | <code>Control.Page = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=&quot;&quot;; …</code><br><code>Control.Page set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=…</code> |
| <code>Api.Control.Parent</code> | <code>Control.Parent = HtmlForm#form1 {Action=&quot;&quot;; ClientID=&quot;form1&quot;; ClientIDMode=Inherit; DefaultButton=&quot;&quot;; DefaultFocus=&quot;&quot;; Disabled=false; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.PreRender</code> | <code>Control.PreRender = &quot;added and removed&quot;</code> |
| <code>Api.Control.RaiseBubbleEvent</code> | <code>Control.RaiseBubbleEvent(Object,EventArgs) = &quot;ok&quot;</code><br><code>Control.RaiseBubbleEvent(Object,EventArgs) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Colo…</code> |
| <code>Api.Control.RemovedControl</code> | <code>Control.RemovedControl(Control) = &quot;ok&quot;</code><br><code>Control.RemovedControl(Control) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000…</code> |
| <code>Api.Control.Render</code> | <code>Control.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.Render(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; src=&quot;~/Images/probe.png&quot; /&gt;</code><br><code>Control.Render(HtmlTextWriter) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 …</code> |
| <code>Api.Control.RenderChildren</code> | <code>Control.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderChildren(HtmlTextWriter) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #0…</code> |
| <code>Api.Control.RenderControl</code> | <code>Control.RenderControl(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; src=&quot;~/Images/probe.png&quot; /&gt;</code><br><code>Control.RenderControl(HtmlTextWriter) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00…</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) writer:</code><br><code>  &#124; &lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; src=&quot;~/Images/probe.png&quot; /&gt;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty…</code> |
| <code>Api.Control.RenderingCompatibility</code> | <code>Control.RenderingCompatibility = &quot;System.Version 4.0&quot;</code><br><code>Control.RenderingCompatibility set = &quot;System.Version 4.0&quot;</code> |
| <code>Api.Control.ResolveAdapter</code> | <code>Control.ResolveAdapter = null</code><br><code>Control.ResolveAdapter after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [E…</code> |
| <code>Api.Control.ResolveClientUrl</code> | <code>Control.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveClientUrl(String) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #0000000…</code> |
| <code>Api.Control.ResolveUrl</code> | <code>Control.ResolveUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveUrl(String) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Colo…</code> |
| <code>Api.Control.SaveControlState</code> | <code>Control.SaveControlState = null</code><br><code>Control.SaveControlState after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color …</code> |
| <code>Api.Control.SaveViewState</code> | <code>Control.SaveViewState = null</code><br><code>Control.SaveViewState after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Em…</code> |
| <code>Api.Control.SetDesignModeState</code> | <code>Control.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>Control.SetDesignModeState(IDictionary) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #…</code> |
| <code>Api.Control.SetRenderMethodDelegate</code> | <code>Control.SetRenderMethodDelegate(RenderMethod) = &quot;ok&quot;</code><br><code>Control.SetRenderMethodDelegate(RenderMethod) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=C…</code> |
| <code>Api.Control.SetTraceData</code> | <code>Control.SetTraceData(Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #0000…</code><br><code>Control.SetTraceData(Object,Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object,Object) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Colo…</code> |
| <code>Api.Control.Site</code> | <code>Control.Site = null</code><br><code>Control.Site set = null</code> |
| <code>Api.Control.SkinID</code> | <code>Control.SkinID = &quot;&quot;</code><br><code>Control.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.TemplateControl</code> | <code>Control.TemplateControl = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code><br><code>Control.TemplateControl set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Cl…</code> |
| <code>Api.Control.TemplateSourceDirectory</code> | <code>Control.TemplateSourceDirectory = &quot;&quot;</code> |
| <code>Api.Control.TrackViewState</code> | <code>Control.TrackViewState = &quot;ok&quot;</code><br><code>Control.TrackViewState after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [E…</code> |
| <code>Api.Control.UniqueID</code> | <code>Control.UniqueID = &quot;ibtnGo&quot;</code> |
| <code>Api.Control.Unload</code> | <code>Control.Unload = &quot;added and removed&quot;</code> |
| <code>Api.Control.ValidateRequestMode</code> | <code>Control.ValidateRequestMode = &quot;ValidateRequestMode.Inherit&quot;</code><br><code>Control.ValidateRequestMode set = &quot;ValidateRequestMode.Disabled&quot;</code> |
| <code>Api.Control.ViewState</code> | <code>Control.ViewState = StateBag [CommandArgument=&quot;1&quot;, CommandName=&quot;go&quot;, ImageUrl=&quot;~/Images/probe.png&quot;]</code> |
| <code>Api.Control.ViewStateIgnoresCase</code> | <code>Control.ViewStateIgnoresCase = &quot;false&quot;</code> |
| <code>Api.Control.ViewStateMode</code> | <code>Control.ViewStateMode = &quot;ViewStateMode.Inherit&quot;</code><br><code>Control.ViewStateMode set = &quot;ViewStateMode.Enabled&quot;</code> |
| <code>Api.Control.Visible</code> | <code>Control.Visible = &quot;true&quot;</code><br><code>Control.Visible set = &quot;true&quot;</code> |
| <code>Api.IAttributeAccessor.GetAttribute</code> | <code>IAttributeAccessor.GetAttribute(String) = null</code><br><code>IAttributeAccessor.GetAttribute(String) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #…</code> |
| <code>Api.IAttributeAccessor.SetAttribute</code> | <code>IAttributeAccessor.SetAttribute(String,String) = &quot;ok&quot;</code><br><code>IAttributeAccessor.SetAttribute(String,String) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=…</code> |
| <code>Api.IControlBuilderAccessor.ControlBuilder</code> | <code>IControlBuilderAccessor.ControlBuilder = null</code> |
| <code>Api.IControlDesignerAccessor.GetDesignModeState</code> | <code>IControlDesignerAccessor.GetDesignModeState = &quot;[]&quot;</code><br><code>IControlDesignerAccessor.GetDesignModeState after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Col…</code> |
| <code>Api.IControlDesignerAccessor.SetDesignModeState</code> | <code>IControlDesignerAccessor.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetDesignModeState(IDictionary) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; Bo…</code> |
| <code>Api.IControlDesignerAccessor.SetOwnerControl</code> | <code>IControlDesignerAccessor.SetOwnerControl(Control) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetOwnerControl(Control) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderCol…</code> |
| <code>Api.IControlDesignerAccessor.UserData</code> | <code>IControlDesignerAccessor.UserData = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.DataBindings</code> | <code>IDataBindingsAccessor.DataBindings = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.HasDataBindings</code> | <code>IDataBindingsAccessor.HasDataBindings = &quot;false&quot;</code> |
| <code>Api.IExpressionsAccessor.Expressions</code> | <code>IExpressionsAccessor.Expressions = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.HasExpressions</code> | <code>IExpressionsAccessor.HasExpressions = &quot;false&quot;</code> |
| <code>Api.IParserAccessor.AddParsedSubObject</code> | <code>IParserAccessor.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>IParserAccessor.AddParsedSubObject(Object) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Colo…</code> |
| <code>Api.IPostBackDataHandler.LoadPostData</code> | <code>IPostBackDataHandler.LoadPostData(String,NameValueCollection) = &quot;false&quot;</code><br><code>IPostBackDataHandler.LoadPostData(String,NameValueCollection) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty…</code> |
| <code>Api.IPostBackDataHandler.RaisePostDataChangedEvent</code> | <code>IPostBackDataHandler.RaisePostDataChangedEvent = &quot;ok&quot;</code><br><code>IPostBackDataHandler.RaisePostDataChangedEvent after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=…</code> |
| <code>Api.IPostBackEventHandler.RaisePostBackEvent</code> | <code>IPostBackEventHandler.RaisePostBackEvent(String) = &quot;ok&quot;</code><br><code>IPostBackEventHandler.RaisePostBackEvent(String) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColo…</code> |
| <code>Api.IUrlResolutionService.ResolveClientUrl</code> | <code>IUrlResolutionService.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>IUrlResolutionService.ResolveClientUrl(String) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=…</code> |
| <code>Api.WebControls.IButtonControl.CausesValidation</code> | <code>IButtonControl.CausesValidation = &quot;true&quot;</code><br><code>IButtonControl.CausesValidation set = &quot;true&quot;</code> |
| <code>Api.WebControls.IButtonControl.Click</code> | <code>IButtonControl.Click = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.IButtonControl.Command</code> | <code>IButtonControl.Command = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.IButtonControl.CommandArgument</code> | <code>IButtonControl.CommandArgument = &quot;1&quot;</code><br><code>IButtonControl.CommandArgument set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.IButtonControl.CommandName</code> | <code>IButtonControl.CommandName = &quot;go&quot;</code><br><code>IButtonControl.CommandName set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.IButtonControl.PostBackUrl</code> | <code>IButtonControl.PostBackUrl = &quot;&quot;</code><br><code>IButtonControl.PostBackUrl set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.IButtonControl.Text</code> | <code>IButtonControl.Text = &quot;&quot;</code><br><code>IButtonControl.Text set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.IButtonControl.ValidationGroup</code> | <code>IButtonControl.ValidationGroup = &quot;&quot;</code><br><code>IButtonControl.ValidationGroup set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.Image.#ctor</code> | <code>Image.#ctor = Image# {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; Bo…</code> |
| <code>Api.WebControls.Image.AddAttributesToRender</code> | <code>Image.AddAttributesToRender(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Image.AddAttributesToRender(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Image.AddAttributesToRender(HtmlTextWriter) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Col…</code> |
| <code>Api.WebControls.Image.AlternateText</code> | <code>Image.AlternateText = &quot;&quot;</code><br><code>Image.AlternateText set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.Image.DescriptionUrl</code> | <code>Image.DescriptionUrl = &quot;&quot;</code><br><code>Image.DescriptionUrl set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.Image.Enabled</code> | <code>Image.Enabled = &quot;true&quot;</code><br><code>Image.Enabled set = &quot;true&quot;</code> |
| <code>Api.WebControls.Image.Font</code> | <code>Image.Font = FontInfo {Bold=false; Italic=false; Name=&quot;&quot;; Overline=false; Size=; Strikeout=false; Underline=false}</code> |
| <code>Api.WebControls.Image.GenerateEmptyAlternateText</code> | <code>Image.GenerateEmptyAlternateText = &quot;false&quot;</code><br><code>Image.GenerateEmptyAlternateText set = &quot;-&gt; System.NotSupportedException: Setting the GenerateEmptyAlternateText property of System.Web.UI.WebControls.ImageButto…</code> |
| <code>Api.WebControls.Image.ImageAlign</code> | <code>Image.ImageAlign = &quot;ImageAlign.NotSet&quot;</code><br><code>Image.ImageAlign set = &quot;ImageAlign.Left&quot;</code> |
| <code>Api.WebControls.Image.ImageUrl</code> | <code>Image.ImageUrl = &quot;~/Images/probe.png&quot;</code><br><code>Image.ImageUrl set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.Image.RenderContents</code> | <code>Image.RenderContents(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Image.RenderContents(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Image.RenderContents(HtmlTextWriter) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #000…</code> |
| <code>Api.WebControls.Image.SupportsDisabledAttribute</code> | <code>Image.SupportsDisabledAttribute = &quot;true&quot;</code> |
| <code>Api.WebControls.ImageButton.#ctor</code> | <code>ImageButton.#ctor = ImageButton# {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyl…</code> |
| <code>Api.WebControls.ImageButton.AddAttributesToRender</code> | <code>ImageButton.AddAttributesToRender(HtmlTextWriter) = &quot;ok&quot;</code><br><code>ImageButton.AddAttributesToRender(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>ImageButton.AddAttributesToRender(HtmlTextWriter) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderCol…</code> |
| <code>Api.WebControls.ImageButton.CausesValidation</code> | <code>ImageButton.CausesValidation = &quot;true&quot;</code><br><code>ImageButton.CausesValidation set = &quot;true&quot;</code> |
| <code>Api.WebControls.ImageButton.Click</code> | <code>ImageButton.Click = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ImageButton.Command</code> | <code>ImageButton.Command = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ImageButton.CommandArgument</code> | <code>ImageButton.CommandArgument = &quot;1&quot;</code><br><code>ImageButton.CommandArgument set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ImageButton.CommandName</code> | <code>ImageButton.CommandName = &quot;go&quot;</code><br><code>ImageButton.CommandName set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ImageButton.Enabled</code> | <code>ImageButton.Enabled = &quot;true&quot;</code><br><code>ImageButton.Enabled set = &quot;true&quot;</code> |
| <code>Api.WebControls.ImageButton.GenerateEmptyAlternateText</code> | <code>ImageButton.GenerateEmptyAlternateText = &quot;false&quot;</code><br><code>ImageButton.GenerateEmptyAlternateText set = &quot;-&gt; System.NotSupportedException: Setting the GenerateEmptyAlternateText property of System.Web.UI.WebControls.Imag…</code> |
| <code>Api.WebControls.ImageButton.GetPostBackOptions</code> | <code>ImageButton.GetPostBackOptions = PostBackOptions {ActionUrl=null; Argument=&quot;&quot;; AutoPostBack=false; ClientSubmit=false; PerformValidation=false; RequiresJavaScri…</code><br><code>ImageButton.GetPostBackOptions after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 …</code> |
| <code>Api.WebControls.ImageButton.LoadPostData</code> | <code>ImageButton.LoadPostData(String,NameValueCollection) = &quot;false&quot;</code><br><code>ImageButton.LoadPostData(String,NameValueCollection) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; Border…</code> |
| <code>Api.WebControls.ImageButton.OnClick</code> | <code>ImageButton.OnClick(ImageClickEventArgs) = &quot;ok&quot;</code><br><code>ImageButton.OnClick(ImageClickEventArgs) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color …</code> |
| <code>Api.WebControls.ImageButton.OnClientClick</code> | <code>ImageButton.OnClientClick = &quot;&quot;</code><br><code>ImageButton.OnClientClick set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ImageButton.OnCommand</code> | <code>ImageButton.OnCommand(CommandEventArgs) = &quot;ok&quot;</code><br><code>ImageButton.OnCommand(CommandEventArgs) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #…</code> |
| <code>Api.WebControls.ImageButton.OnPreRender</code> | <code>ImageButton.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>ImageButton.OnPreRender(EventArgs) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000…</code> |
| <code>Api.WebControls.ImageButton.PostBackUrl</code> | <code>ImageButton.PostBackUrl = &quot;&quot;</code><br><code>ImageButton.PostBackUrl set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ImageButton.RaisePostBackEvent</code> | <code>ImageButton.RaisePostBackEvent(String) = &quot;ok&quot;</code><br><code>ImageButton.RaisePostBackEvent(String) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #0…</code> |
| <code>Api.WebControls.ImageButton.RaisePostDataChangedEvent</code> | <code>ImageButton.RaisePostDataChangedEvent = &quot;ok&quot;</code><br><code>ImageButton.RaisePostDataChangedEvent after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00…</code> |
| <code>Api.WebControls.ImageButton.SupportsDisabledAttribute</code> | <code>ImageButton.SupportsDisabledAttribute = &quot;true&quot;</code> |
| <code>Api.WebControls.ImageButton.TagKey</code> | <code>ImageButton.TagKey = &quot;HtmlTextWriterTag.Input&quot;</code> |
| <code>Api.WebControls.ImageButton.Text</code> | <code>ImageButton.Text = &quot;&quot;</code><br><code>ImageButton.Text set = &quot;ok&quot;</code> |
| <code>Api.WebControls.ImageButton.ValidationGroup</code> | <code>ImageButton.ValidationGroup = &quot;&quot;</code><br><code>ImageButton.ValidationGroup set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.#ctor</code> | <code>WebControl.#ctor = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidt…</code><br><code>WebControl.#ctor(String) = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; Bo…</code><br><code>WebControl.#ctor(HtmlTextWriterTag) = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle…</code> |
| <code>Api.WebControls.WebControl.AccessKey</code> | <code>WebControl.AccessKey = &quot;&quot;</code><br><code>WebControl.AccessKey set = &quot;-&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]&quot;</code> |
| <code>Api.WebControls.WebControl.AddAttributesToRender</code> | <code>WebControl.AddAttributesToRender(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.AddAttributesToRender(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>WebControl.AddAttributesToRender(HtmlTextWriter) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColo…</code> |
| <code>Api.WebControls.WebControl.ApplyStyle</code> | <code>WebControl.ApplyStyle(Style) = &quot;ok&quot;</code><br><code>WebControl.ApplyStyle(Style) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Co…</code> |
| <code>Api.WebControls.WebControl.Attributes</code> | <code>WebControl.Attributes = AttributeCollection []</code> |
| <code>Api.WebControls.WebControl.BackColor</code> | <code>WebControl.BackColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>WebControl.BackColor set = &quot;Color #FF3366CC Color [A=255, R=51, G=102, B=204]&quot;</code> |
| <code>Api.WebControls.WebControl.BorderColor</code> | <code>WebControl.BorderColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>WebControl.BorderColor set = &quot;Color #FF3366CC Color [A=255, R=51, G=102, B=204]&quot;</code> |
| <code>Api.WebControls.WebControl.BorderStyle</code> | <code>WebControl.BorderStyle = &quot;BorderStyle.NotSet&quot;</code><br><code>WebControl.BorderStyle set = &quot;BorderStyle.None&quot;</code> |
| <code>Api.WebControls.WebControl.BorderWidth</code> | <code>WebControl.BorderWidth = Unit </code><br><code>WebControl.BorderWidth set = Unit 120px</code> |
| <code>Api.WebControls.WebControl.ControlStyle</code> | <code>WebControl.ControlStyle = Style {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; CssClass=…</code> |
| <code>Api.WebControls.WebControl.ControlStyleCreated</code> | <code>WebControl.ControlStyleCreated = &quot;false&quot;</code> |
| <code>Api.WebControls.WebControl.CopyBaseAttributes</code> | <code>WebControl.CopyBaseAttributes(WebControl) = &quot;ok&quot;</code><br><code>WebControl.CopyBaseAttributes(WebControl) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color…</code> |
| <code>Api.WebControls.WebControl.CreateControlStyle</code> | <code>WebControl.CreateControlStyle = Style {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; Css…</code><br><code>WebControl.CreateControlStyle after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 C…</code> |
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
| <code>Api.WebControls.WebControl.MergeStyle</code> | <code>WebControl.MergeStyle(Style) = &quot;ok&quot;</code><br><code>WebControl.MergeStyle(Style) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Co…</code> |
| <code>Api.WebControls.WebControl.Render</code> | <code>WebControl.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.Render(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; src=&quot;~/Images/probe.png&quot; /&gt;</code><br><code>WebControl.Render(HtmlTextWriter) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #000000…</code> |
| <code>Api.WebControls.WebControl.RenderBeginTag</code> | <code>WebControl.RenderBeginTag(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.RenderBeginTag(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; src=&quot;~/Images/probe.png&quot; /&gt;</code><br><code>WebControl.RenderBeginTag(HtmlTextWriter) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color…</code> |
| <code>Api.WebControls.WebControl.RenderContents</code> | <code>WebControl.RenderContents(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.RenderContents(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>WebControl.RenderContents(HtmlTextWriter) after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color…</code> |
| <code>Api.WebControls.WebControl.RenderEndTag</code> | <code>WebControl.RenderEndTag(HtmlTextWriter) = &quot;-&gt; System.InvalidOperationException: A PopEndTag was called without a corresponding PushEndTag.&quot;</code> |
| <code>Api.WebControls.WebControl.SaveViewState</code> | <code>WebControl.SaveViewState = null</code><br><code>WebControl.SaveViewState after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color …</code> |
| <code>Api.WebControls.WebControl.SkinID</code> | <code>WebControl.SkinID = &quot;&quot;</code><br><code>WebControl.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.Style</code> | <code>WebControl.Style = CssStyleCollection {Count=Int32 0; Value=null}</code> |
| <code>Api.WebControls.WebControl.SupportsDisabledAttribute</code> | <code>WebControl.SupportsDisabledAttribute = &quot;true&quot;</code> |
| <code>Api.WebControls.WebControl.TabIndex</code> | <code>WebControl.TabIndex = &quot;Int16 0&quot;</code><br><code>WebControl.TabIndex set = &quot;Int16 1&quot;</code> |
| <code>Api.WebControls.WebControl.TagKey</code> | <code>WebControl.TagKey = &quot;HtmlTextWriterTag.Input&quot;</code> |
| <code>Api.WebControls.WebControl.TagName</code> | <code>WebControl.TagName = &quot;input&quot;</code> |
| <code>Api.WebControls.WebControl.ToolTip</code> | <code>WebControl.ToolTip = &quot;&quot;</code><br><code>WebControl.ToolTip set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.TrackViewState</code> | <code>WebControl.TrackViewState = &quot;ok&quot;</code><br><code>WebControl.TrackViewState after = ImageButton#ibtnGo {AccessKey=&quot;&quot;; AlternateText=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color…</code> |
| <code>Api.WebControls.WebControl.Width</code> | <code>WebControl.Width = Unit </code><br><code>WebControl.Width set = Unit 120px</code> |

## ② HTML 層

プローブサイトの RenderProbe.aspx で、1 ケース 1 リクエスト（ページライフサイクル全体）で描画したページ全体を比較します。下表はコンポーネント部分（div#probe）、設定時の例外、保存された ViewState の要約です。

| ケース | 設定 | 期待される描画 | 設定時の例外 |
|---|---|---|---|
| <code>.(default)</code> | <code>(nothing set)</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.AccessKey=text</code> | <code>c.AccessKey = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> | <code>c.AccessKey = &quot;probe&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=markup</code> | <code>c.AccessKey = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> | <code>c.AccessKey = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=japanese</code> | <code>c.AccessKey = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> | <code>c.AccessKey = &quot;日本語テキスト&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=empty</code> | <code>c.AccessKey = &quot;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.AccessKey=k</code> | <code>c.AccessKey = &quot;k&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; accesskey=&quot;k&quot; /&gt;</code> |  |
| <code>.AlternateText=text</code> | <code>c.AlternateText = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; alt=&quot;probe&quot; /&gt;</code> |  |
| <code>.AlternateText=markup</code> | <code>c.AlternateText = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; alt=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; /&gt;</code> |  |
| <code>.AlternateText=japanese</code> | <code>c.AlternateText = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; alt=&quot;日本語テキスト&quot; /&gt;</code> |  |
| <code>.AlternateText=empty</code> | <code>c.AlternateText = &quot;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.AppRelativeTemplateSourceDirectory=text</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;probe&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=markup</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=japanese</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;日本語テキスト&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=empty</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.BackColor=Red</code> | <code>c.BackColor = Color.Red;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;background-color:Red;&quot; /&gt;</code> |  |
| <code>.BackColor=#3366CC</code> | <code>c.BackColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;background-color:#3366CC;&quot; /&gt;</code> |  |
| <code>.BackColor=Transparent</code> | <code>c.BackColor = Color.Transparent;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;background-color:Transparent;&quot; /&gt;</code> |  |
| <code>.BorderColor=Red</code> | <code>c.BorderColor = Color.Red;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;border-color:Red;&quot; /&gt;</code> |  |
| <code>.BorderColor=#3366CC</code> | <code>c.BorderColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;border-color:#3366CC;&quot; /&gt;</code> |  |
| <code>.BorderColor=Transparent</code> | <code>c.BorderColor = Color.Transparent;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;border-color:Transparent;&quot; /&gt;</code> |  |
| <code>.BorderStyle=NotSet</code> | <code>c.BorderStyle = BorderStyle.NotSet;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.BorderStyle=None</code> | <code>c.BorderStyle = BorderStyle.None;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;border-style:None;&quot; /&gt;</code> |  |
| <code>.BorderStyle=Dotted</code> | <code>c.BorderStyle = BorderStyle.Dotted;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;border-style:Dotted;&quot; /&gt;</code> |  |
| <code>.BorderStyle=Dashed</code> | <code>c.BorderStyle = BorderStyle.Dashed;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;border-style:Dashed;&quot; /&gt;</code> |  |
| <code>.BorderStyle=Solid</code> | <code>c.BorderStyle = BorderStyle.Solid;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;border-style:Solid;&quot; /&gt;</code> |  |
| <code>.BorderStyle=Double</code> | <code>c.BorderStyle = BorderStyle.Double;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;border-style:Double;&quot; /&gt;</code> |  |
| <code>.BorderStyle=Groove</code> | <code>c.BorderStyle = BorderStyle.Groove;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;border-style:Groove;&quot; /&gt;</code> |  |
| <code>.BorderStyle=Ridge</code> | <code>c.BorderStyle = BorderStyle.Ridge;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;border-style:Ridge;&quot; /&gt;</code> |  |
| <code>.BorderStyle=Inset</code> | <code>c.BorderStyle = BorderStyle.Inset;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;border-style:Inset;&quot; /&gt;</code> |  |
| <code>.BorderStyle=Outset</code> | <code>c.BorderStyle = BorderStyle.Outset;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;border-style:Outset;&quot; /&gt;</code> |  |
| <code>.BorderWidth=120px</code> | <code>c.BorderWidth = Unit.Pixel(120);</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;border-width:120px;border-style:solid;&quot; /&gt;</code> |  |
| <code>.BorderWidth=50%</code> | <code>c.BorderWidth = Unit.Percentage(50);</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> | <code>c.BorderWidth = Unit.Percentage(50); -&gt; System.ArgumentOutOfRangeException: BorderWidth must be a non-negative number and cannot be a percentage. [param value]</code> |
| <code>.BorderWidth=2em</code> | <code>c.BorderWidth = new Unit(2, UnitType.Em);</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;border-width:2em;border-style:solid;&quot; /&gt;</code> |  |
| <code>.CausesValidation=true</code> | <code>c.CausesValidation = true;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.CausesValidation=false</code> | <code>c.CausesValidation = false;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.ClientIDMode=Inherit</code> | <code>c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.ClientIDMode=AutoID</code> | <code>c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.ClientIDMode=Predictable</code> | <code>c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.ClientIDMode=Static</code> | <code>c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.CommandArgument=text</code> | <code>c.CommandArgument = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.CommandArgument=markup</code> | <code>c.CommandArgument = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.CommandArgument=japanese</code> | <code>c.CommandArgument = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.CommandArgument=empty</code> | <code>c.CommandArgument = &quot;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.CommandName=text</code> | <code>c.CommandName = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.CommandName=markup</code> | <code>c.CommandName = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.CommandName=japanese</code> | <code>c.CommandName = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.CommandName=empty</code> | <code>c.CommandName = &quot;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.CssClass=text</code> | <code>c.CssClass = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; class=&quot;probe&quot; /&gt;</code> |  |
| <code>.CssClass=markup</code> | <code>c.CssClass = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; class=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; /&gt;</code> |  |
| <code>.CssClass=japanese</code> | <code>c.CssClass = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; class=&quot;日本語テキスト&quot; /&gt;</code> |  |
| <code>.CssClass=empty</code> | <code>c.CssClass = &quot;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.DescriptionUrl=text</code> | <code>c.DescriptionUrl = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; longdesc=&quot;probe&quot; /&gt;</code> |  |
| <code>.DescriptionUrl=markup</code> | <code>c.DescriptionUrl = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; longdesc=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; /&gt;</code> |  |
| <code>.DescriptionUrl=japanese</code> | <code>c.DescriptionUrl = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; longdesc=&quot;%e6%97%a5%e6%9c%ac%e8%aa%9e%e3%83%86%e3%82%ad%e3%82%b9%e3%83%88&quot; /&gt;</code> |  |
| <code>.DescriptionUrl=empty</code> | <code>c.DescriptionUrl = &quot;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.EnableTheming=true</code> | <code>c.EnableTheming = true;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.EnableTheming=false</code> | <code>c.EnableTheming = false;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.EnableViewState=true</code> | <code>c.EnableViewState = true;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.EnableViewState=false</code> | <code>c.EnableViewState = false;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.Enabled=true</code> | <code>c.Enabled = true;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.Enabled=false</code> | <code>c.Enabled = false;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; disabled=&quot;disabled&quot; class=&quot;aspNetDisabled&quot; /&gt;</code> |  |
| <code>.Font.Bold=true</code> | <code>c.Font.Bold = true;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;font-weight:bold;&quot; /&gt;</code> |  |
| <code>.Font.Bold=false</code> | <code>c.Font.Bold = false;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;font-weight:normal;&quot; /&gt;</code> |  |
| <code>.Font.Italic=true</code> | <code>c.Font.Italic = true;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;font-style:italic;&quot; /&gt;</code> |  |
| <code>.Font.Italic=false</code> | <code>c.Font.Italic = false;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;font-style:normal;&quot; /&gt;</code> |  |
| <code>.Font.Name=text</code> | <code>c.Font.Name = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;font-family:probe;&quot; /&gt;</code> |  |
| <code>.Font.Name=markup</code> | <code>c.Font.Name = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;font-family:&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;;&quot; /&gt;</code> |  |
| <code>.Font.Name=japanese</code> | <code>c.Font.Name = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;font-family:日本語テキスト;&quot; /&gt;</code> |  |
| <code>.Font.Name=empty</code> | <code>c.Font.Name = &quot;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.Font.Names=Arial,MS-Gothic</code> | <code>c.Font.Names = new[] { &quot;Arial&quot;, &quot;MS Gothic&quot; };</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;font-family:Arial,MS Gothic;&quot; /&gt;</code> |  |
| <code>.Font.Overline=true</code> | <code>c.Font.Overline = true;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;text-decoration: overline;&quot; /&gt;</code> |  |
| <code>.Font.Overline=false</code> | <code>c.Font.Overline = false;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;text-decoration:none;&quot; /&gt;</code> |  |
| <code>.Font.Size=12pt</code> | <code>c.Font.Size = FontUnit.Point(12);</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;font-size:12pt;&quot; /&gt;</code> |  |
| <code>.Font.Size=Large</code> | <code>c.Font.Size = FontUnit.Large;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;font-size:Large;&quot; /&gt;</code> |  |
| <code>.Font.Size=150%</code> | <code>c.Font.Size = new FontUnit(Unit.Percentage(150));</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;font-size:150%;&quot; /&gt;</code> |  |
| <code>.Font.Strikeout=true</code> | <code>c.Font.Strikeout = true;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;text-decoration: line-through;&quot; /&gt;</code> |  |
| <code>.Font.Strikeout=false</code> | <code>c.Font.Strikeout = false;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;text-decoration:none;&quot; /&gt;</code> |  |
| <code>.Font.Underline=true</code> | <code>c.Font.Underline = true;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;text-decoration:underline;&quot; /&gt;</code> |  |
| <code>.Font.Underline=false</code> | <code>c.Font.Underline = false;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;text-decoration:none;&quot; /&gt;</code> |  |
| <code>.ForeColor=Red</code> | <code>c.ForeColor = Color.Red;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;color:Red;&quot; /&gt;</code> |  |
| <code>.ForeColor=#3366CC</code> | <code>c.ForeColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;color:#3366CC;&quot; /&gt;</code> |  |
| <code>.ForeColor=Transparent</code> | <code>c.ForeColor = Color.Transparent;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;color:Transparent;&quot; /&gt;</code> |  |
| <code>.GenerateEmptyAlternateText=true</code> | <code>c.GenerateEmptyAlternateText = true;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> | <code>c.GenerateEmptyAlternateText = true; -&gt; System.NotSupportedException: Setting the GenerateEmptyAlternateText property of System.Web.UI.WebControls.ImageButton is not supported.</code> |
| <code>.GenerateEmptyAlternateText=false</code> | <code>c.GenerateEmptyAlternateText = false;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> | <code>c.GenerateEmptyAlternateText = false; -&gt; System.NotSupportedException: Setting the GenerateEmptyAlternateText property of System.Web.UI.WebControls.ImageButton is not supported.</code> |
| <code>.Height=120px</code> | <code>c.Height = Unit.Pixel(120);</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;height:120px;&quot; /&gt;</code> |  |
| <code>.Height=50%</code> | <code>c.Height = Unit.Percentage(50);</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;height:50%;&quot; /&gt;</code> |  |
| <code>.Height=2em</code> | <code>c.Height = new Unit(2, UnitType.Em);</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;height:2em;&quot; /&gt;</code> |  |
| <code>.ID=text</code> | <code>c.ID = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;probe&quot; id=&quot;probe&quot; /&gt;</code> |  |
| <code>.ID=markup</code> | <code>c.ID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; id=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; /&gt;</code> |  |
| <code>.ID=japanese</code> | <code>c.ID = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;日本語テキスト&quot; id=&quot;日本語テキスト&quot; /&gt;</code> |  |
| <code>.ID=empty</code> | <code>c.ID = &quot;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ctl02&quot; id=&quot;ctl02&quot; /&gt;</code> |  |
| <code>.ImageAlign=NotSet</code> | <code>c.ImageAlign = ImageAlign.NotSet;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.ImageAlign=Left</code> | <code>c.ImageAlign = ImageAlign.Left;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; align=&quot;left&quot; /&gt;</code> |  |
| <code>.ImageAlign=Right</code> | <code>c.ImageAlign = ImageAlign.Right;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; align=&quot;right&quot; /&gt;</code> |  |
| <code>.ImageAlign=Baseline</code> | <code>c.ImageAlign = ImageAlign.Baseline;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; align=&quot;baseline&quot; /&gt;</code> |  |
| <code>.ImageAlign=Top</code> | <code>c.ImageAlign = ImageAlign.Top;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; align=&quot;top&quot; /&gt;</code> |  |
| <code>.ImageAlign=Middle</code> | <code>c.ImageAlign = ImageAlign.Middle;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; align=&quot;middle&quot; /&gt;</code> |  |
| <code>.ImageAlign=Bottom</code> | <code>c.ImageAlign = ImageAlign.Bottom;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; align=&quot;bottom&quot; /&gt;</code> |  |
| <code>.ImageAlign=AbsBottom</code> | <code>c.ImageAlign = ImageAlign.AbsBottom;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; align=&quot;absbottom&quot; /&gt;</code> |  |
| <code>.ImageAlign=AbsMiddle</code> | <code>c.ImageAlign = ImageAlign.AbsMiddle;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; align=&quot;absmiddle&quot; /&gt;</code> |  |
| <code>.ImageAlign=TextTop</code> | <code>c.ImageAlign = ImageAlign.TextTop;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; align=&quot;texttop&quot; /&gt;</code> |  |
| <code>.ImageUrl=text</code> | <code>c.ImageUrl = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; src=&quot;probe&quot; /&gt;</code> |  |
| <code>.ImageUrl=markup</code> | <code>c.ImageUrl = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; src=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; /&gt;</code> |  |
| <code>.ImageUrl=japanese</code> | <code>c.ImageUrl = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; src=&quot;%e6%97%a5%e6%9c%ac%e8%aa%9e%e3%83%86%e3%82%ad%e3%82%b9%e3%83%88&quot; /&gt;</code> |  |
| <code>.ImageUrl=empty</code> | <code>c.ImageUrl = &quot;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.ImageUrl=app-relative</code> | <code>c.ImageUrl = &quot;~/Images/probe.png&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; src=&quot;Images/probe.png&quot; /&gt;</code> |  |
| <code>.ImageUrl=absolute</code> | <code>c.ImageUrl = &quot;http://example.com/images/a b.png?x=1&amp;y=2&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; src=&quot;http://example.com/images/a%20b.png?x=1&amp;y=2&quot; /&gt;</code> |  |
| <code>.OnClientClick=text</code> | <code>c.OnClientClick = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; onclick=&quot;probe;&quot; /&gt;</code> |  |
| <code>.OnClientClick=markup</code> | <code>c.OnClientClick = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; onclick=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;;&quot; /&gt;</code> |  |
| <code>.OnClientClick=japanese</code> | <code>c.OnClientClick = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; onclick=&quot;日本語テキスト;&quot; /&gt;</code> |  |
| <code>.OnClientClick=empty</code> | <code>c.OnClientClick = &quot;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.OnClientClick=confirm</code> | <code>c.OnClientClick = &quot;return confirm(&#39;OK?&#39;);&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; onclick=&quot;return confirm(&#39;OK?&#39;);&quot; /&gt;</code> |  |
| <code>.PostBackUrl=text</code> | <code>c.PostBackUrl = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; onclick=&quot;javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;ibtnGo&quot;, &quot;&quot;, false, &quot;&quot;, &quot;probe&quot;, false, false))&quot; /&gt;</code> |  |
| <code>.PostBackUrl=markup</code> | <code>c.PostBackUrl = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; onclick=&quot;javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;ibtnGo&quot;, &quot;&quot;, false, &quot;&quot;, &quot;&lt;b&gt;\&quot;&amp;\&#39;&lt;/b&gt;&quot;, false, false))&quot; /&gt;</code> |  |
| <code>.PostBackUrl=japanese</code> | <code>c.PostBackUrl = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; onclick=&quot;javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;ibtnGo&quot;, &quot;&quot;, false, &quot;&quot;, &quot;%e6%97%a5%e6%9c%ac%e8%aa%9e%e3%83%86%e3%82%ad%e3%82%b9%e3%83%88&quot;, false, false))&quot; /&gt;</code> |  |
| <code>.PostBackUrl=empty</code> | <code>c.PostBackUrl = &quot;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.PostBackUrl=app-relative</code> | <code>c.PostBackUrl = &quot;~/ButtonTarget.aspx&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; onclick=&quot;javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;ibtnGo&quot;, &quot;&quot;, false, &quot;&quot;, &quot;ButtonTarget.aspx&quot;, false, false))&quot; /&gt;</code> |  |
| <code>.PostBackUrl=absolute</code> | <code>c.PostBackUrl = &quot;http://example.com/target.aspx?a=1&amp;b=2&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; onclick=&quot;javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;ibtnGo&quot;, &quot;&quot;, false, &quot;&quot;, &quot;http://example.com/target.aspx?a=1&amp;b=2&quot;, false, false))&quot; /&gt;</code> |  |
| <code>.SkinID=text</code> | <code>c.SkinID = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.SkinID=markup</code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.SkinID=japanese</code> | <code>c.SkinID = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.SkinID=empty</code> | <code>c.SkinID = &quot;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.TabIndex=0</code> | <code>c.TabIndex = 0;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.TabIndex=3</code> | <code>c.TabIndex = 3;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; tabindex=&quot;3&quot; /&gt;</code> |  |
| <code>.TabIndex=-1</code> | <code>c.TabIndex = -1;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; tabindex=&quot;-1&quot; /&gt;</code> |  |
| <code>.ToolTip=text</code> | <code>c.ToolTip = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; title=&quot;probe&quot; /&gt;</code> |  |
| <code>.ToolTip=markup</code> | <code>c.ToolTip = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; title=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; /&gt;</code> |  |
| <code>.ToolTip=japanese</code> | <code>c.ToolTip = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; title=&quot;日本語テキスト&quot; /&gt;</code> |  |
| <code>.ToolTip=empty</code> | <code>c.ToolTip = &quot;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.ValidateRequestMode=Inherit</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Inherit;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.ValidateRequestMode=Disabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Disabled;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.ValidateRequestMode=Enabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Enabled;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.ValidationGroup=text</code> | <code>c.ValidationGroup = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.ValidationGroup=markup</code> | <code>c.ValidationGroup = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.ValidationGroup=japanese</code> | <code>c.ValidationGroup = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.ValidationGroup=empty</code> | <code>c.ValidationGroup = &quot;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.ViewStateMode=Inherit</code> | <code>c.ViewStateMode = ViewStateMode.Inherit;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.ViewStateMode=Enabled</code> | <code>c.ViewStateMode = ViewStateMode.Enabled;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.ViewStateMode=Disabled</code> | <code>c.ViewStateMode = ViewStateMode.Disabled;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.Visible=true</code> | <code>c.Visible = true;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.Visible=false</code> | <code>c.Visible = false;</code> | <code></code> |  |
| <code>.Width=120px</code> | <code>c.Width = Unit.Pixel(120);</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;width:120px;&quot; /&gt;</code> |  |
| <code>.Width=50%</code> | <code>c.Width = Unit.Percentage(50);</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;width:50%;&quot; /&gt;</code> |  |
| <code>.Width=2em</code> | <code>c.Width = new Unit(2, UnitType.Em);</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;width:2em;&quot; /&gt;</code> |  |
| <code>.Attributes[data-probe]=text</code> | <code>c.Attributes[data-probe] = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; data-probe=&quot;probe&quot; /&gt;</code> |  |
| <code>.Attributes[onfocus]=text</code> | <code>c.Attributes[onfocus] = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; onfocus=&quot;probe&quot; /&gt;</code> |  |
| <code>.Attributes[data-probe]=markup</code> | <code>c.Attributes[data-probe] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; data-probe=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; /&gt;</code> |  |
| <code>.Attributes[onfocus]=markup</code> | <code>c.Attributes[onfocus] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; onfocus=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; /&gt;</code> |  |
| <code>.Style[color]=red</code> | <code>c.Style[color] = &quot;red&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;color:red;&quot; /&gt;</code> |  |
| <code>.Style[margin-left]=red</code> | <code>c.Style[margin-left] = &quot;red&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;margin-left:red;&quot; /&gt;</code> |  |
| <code>.Style[color]=1em</code> | <code>c.Style[color] = &quot;1em&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;color:1em;&quot; /&gt;</code> |  |
| <code>.Style[margin-left]=1em</code> | <code>c.Style[margin-left] = &quot;1em&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;margin-left:1em;&quot; /&gt;</code> |  |
| <code>.OnClientClick=confirm+CausesValidation=false</code> | <code>c.OnClientClick = &quot;return confirm(&#39;OK?&#39;);&quot;; c.CausesValidation = false;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; onclick=&quot;return confirm(&#39;OK?&#39;);&quot; /&gt;</code> |  |
| <code>.OnClientClick=confirm+ValidationGroup=text</code> | <code>c.OnClientClick = &quot;return confirm(&#39;OK?&#39;);&quot;; c.ValidationGroup = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; onclick=&quot;return confirm(&#39;OK?&#39;);&quot; /&gt;</code> |  |
| <code>.OnClientClick=confirm+Enabled=false</code> | <code>c.OnClientClick = &quot;return confirm(&#39;OK?&#39;);&quot;; c.Enabled = false;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; disabled=&quot;disabled&quot; class=&quot;aspNetDisabled&quot; /&gt;</code> |  |
| <code>.PostBackUrl=app-relative+ValidationGroup=text</code> | <code>c.PostBackUrl = &quot;~/ButtonTarget.aspx&quot;; c.ValidationGroup = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; onclick=&quot;javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;ibtnGo&quot;, &quot;&quot;, false, &quot;&quot;, &quot;ButtonTarget.aspx&quot;, false, false))&quot; /&gt;</code> |  |
| <code>.PostBackUrl=app-relative+CausesValidation=false</code> | <code>c.PostBackUrl = &quot;~/ButtonTarget.aspx&quot;; c.CausesValidation = false;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; onclick=&quot;javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;ibtnGo&quot;, &quot;&quot;, false, &quot;&quot;, &quot;ButtonTarget.aspx&quot;, false, false))&quot; /&gt;</code> |  |
| <code>.PostBackUrl=app-relative+OnClientClick=confirm</code> | <code>c.PostBackUrl = &quot;~/ButtonTarget.aspx&quot;; c.OnClientClick = &quot;return confirm(&#39;OK?&#39;);&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; onclick=&quot;return confirm(&#39;OK?&#39;);WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;ibtnGo&quot;, &quot;&quot;, false, &quot;&quot;, &quot;ButtonTarget.aspx&quot;, false, false))&quot; /&gt;</code> |  |
| <code>.PostBackUrl=app-relative+Enabled=false</code> | <code>c.PostBackUrl = &quot;~/ButtonTarget.aspx&quot;; c.Enabled = false;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; disabled=&quot;disabled&quot; class=&quot;aspNetDisabled&quot; /&gt;</code> |  |
| <code>.CommandName=text+CommandArgument=markup</code> | <code>c.CommandName = &quot;probe&quot;; c.CommandArgument = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>.Enabled=false+CssClass=text</code> | <code>c.Enabled = false; c.CssClass = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; disabled=&quot;disabled&quot; class=&quot;aspNetDisabled probe&quot; /&gt;</code> |  |
| <code>.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | <code>c.Width = Unit.Pixel(120); c.Height = new Unit(2, UnitType.Em); c.BorderWidth = Unit.Pixel(120); c.BorderStyle = BorderStyle.Solid; c.BackColor = Color.FromArgb(0x33, 0x66, 0xCC); c.ForeColor = Color.Red; c.Font.Bold = true;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;color:Red;background-color:#3366CC;border-width:120px;border-style:Solid;font-weight:bold;height:2em;width:120px;&quot; /&gt;</code> |  |
| <code>.ForeColor=#3366CC+Style[color]=red</code> | <code>c.ForeColor = Color.FromArgb(0x33, 0x66, 0xCC); c.Style[color] = &quot;red&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; style=&quot;color:#3366CC;color:red;&quot; /&gt;</code> |  |
| <code>.ImageUrl=app-relative+AlternateText=text</code> | <code>c.ImageUrl = &quot;~/Images/probe.png&quot;; c.AlternateText = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; src=&quot;Images/probe.png&quot; alt=&quot;probe&quot; /&gt;</code> |  |
| <code>.ImageUrl=app-relative+Visible=false</code> | <code>c.ImageUrl = &quot;~/Images/probe.png&quot;; c.Visible = false;</code> | <code></code> |  |
| <code>.ImageUrl=app-relative+EnableViewState=false</code> | <code>c.ImageUrl = &quot;~/Images/probe.png&quot;; c.EnableViewState = false;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; src=&quot;Images/probe.png&quot; /&gt;</code> |  |
| <code>.ImageUrl=app-relative+Enabled=false</code> | <code>c.ImageUrl = &quot;~/Images/probe.png&quot;; c.Enabled = false;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; disabled=&quot;disabled&quot; class=&quot;aspNetDisabled&quot; src=&quot;Images/probe.png&quot; /&gt;</code> |  |
| <code>[naming container].(default)</code> | <code>[naming container] (nothing set)</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input type=&quot;image&quot; name=&quot;box$ibtnGo&quot; id=&quot;box_ibtnGo&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=AutoID</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input type=&quot;image&quot; name=&quot;box$ibtnGo&quot; id=&quot;box_ibtnGo&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Static</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input type=&quot;image&quot; name=&quot;box$ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Predictable</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input type=&quot;image&quot; name=&quot;box$ibtnGo&quot; id=&quot;box_ibtnGo&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Inherit</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input type=&quot;image&quot; name=&quot;box$ibtnGo&quot; id=&quot;box_ibtnGo&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ID=text</code> | <code>[naming container] c.ID = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input type=&quot;image&quot; name=&quot;box$probe&quot; id=&quot;box_probe&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].PostBackUrl=app-relative</code> | <code>[naming container] c.PostBackUrl = &quot;~/ButtonTarget.aspx&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input type=&quot;image&quot; name=&quot;box$ibtnGo&quot; id=&quot;box_ibtnGo&quot; onclick=&quot;javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;box$ibtnGo&quot;, &quot;&quot;, false, &quot;&quot;, &quot;ButtonTarget.aspx&quot;, false, false))&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ValidationGroup=text</code> | <code>[naming container] c.ValidationGroup = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input type=&quot;image&quot; name=&quot;box$ibtnGo&quot; id=&quot;box_ibtnGo&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[themed].(default)</code> | <code>[themed] (nothing set)</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; class=&quot;skinned&quot; style=&quot;border-width:1px;border-style:solid;&quot; /&gt;</code> |  |
| <code>[themed].SkinID=text</code> | <code>[themed] c.SkinID = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; class=&quot;skinned-probe&quot; src=&quot;Images/probe.png&quot; /&gt;</code> |  |
| <code>[themed].EnableTheming=false</code> | <code>[themed] c.EnableTheming = false;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; /&gt;</code> |  |
| <code>[themed].CssClass=text</code> | <code>[themed] c.CssClass = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;image&quot; name=&quot;ibtnGo&quot; id=&quot;ibtnGo&quot; class=&quot;skinned&quot; style=&quot;border-width:1px;border-style:solid;&quot; /&gt;</code> |  |

## ③ シナリオ層

ブラウザ（Playwright）で ImageButtonScenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。

| # | 操作 | 対象 | 値 |
|---|---|---|---|
| 1 | goto | /BasicsScenario.aspx | <code></code> |
| 2 | snapshot |  | **ImageButton [render][databind][dynamic] first request** |
| 3 | click | ibtnGo | <code></code> |
| 4 | snapshot |  | **ImageButton [postdata][postbackevent][command][viewstate] an image clicked: its coordinates, Click, then Command** |
| 5 | click | ibtnConfirmNo | <code></code> |
| 6 | snapshot |  | **ImageButton [clientclick] OnClientClick returns false: no request** |
| 7 | click | ibtnValidate | <code></code> |
| 8 | snapshot |  | **ImageButton [causesvalidation][postbackevent] validating g1 (empty)** |
| 9 | click | ibtnPick | <code></code> |
| 10 | snapshot |  | **ImageButton [command][databind] Command bubbled from a repeater item** |
| 11 | fill | txtDefaultImage | <code>enter</code> |
| 12 | press | txtDefaultImage | <code>Enter</code> |
| 13 | snapshot |  | **ImageButton [defaultbutton] Enter in a panel whose DefaultButton is the image** |
| 14 | click | ibtnDynamic | <code></code> |
| 15 | snapshot |  | **ImageButton [dynamic][postbackevent] a dynamically created image button** |
| 16 | click | btnShow | <code></code> |
| 17 | click | ibtnHidden | <code></code> |
| 18 | snapshot |  | **ImageButton [visible][viewstate][postbackevent] the image shown before raises Click** |
| 19 | evaluate | ibtnGo | <code>const i = document.createElement(&#39;input&#39;); i.type = &#39;hidden&#39;; i.name = e.name + &#39;.x&#39;; i.value = &#39;&lt;b&gt;x&lt;/b&gt;&#39;; e.form.appendChild(i);</code> |
| 20 | click | ibtnGo | <code></code> |
| 21 | snapshot |  | **ImageButton [requestvalidation] markup posted as a coordinate** |
| 22 | goto | /BasicsScenario.aspx | <code></code> |
| 23 | fill | txtCross | <code>cross</code> |
| 24 | click | ibtnCross | <code></code> |
| 25 | snapshot |  | **ImageButton [crosspage] cross-page postback (PostBackUrl)** |

<details><summary>ImageButton [render][databind][dynamic] first request（期待される本文）</summary>

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

<details><summary>ImageButton [postdata][postbackevent][command][viewstate] an image clicked: its coordinates, Click, then Command（期待される本文）</summary>

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
Click ibtnGo at 10,5
Command ibtnGo: go 1
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

<details><summary>ImageButton [clientclick] OnClientClick returns false: no request（期待される本文）</summary>

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
Click ibtnGo at 10,5
Command ibtnGo: go 1
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

<details><summary>ImageButton [causesvalidation][postbackevent] validating g1 (empty)（期待される本文）</summary>

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
Required (g1, server): Required (g1) Cross-page:
Request 3: IsPostBack=True
Changed chkNoViewState Checked=True
Changed hidNoViewState Value="set in code, no view state"
Click ibtnValidate at 10,5
IsValid: False
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

<details><summary>ImageButton [command][databind] Command bubbled from a repeater item（期待される本文）</summary>

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
Request 4: IsPostBack=True
Changed chkNoViewState Checked=True
Changed hidNoViewState Value="set in code, no view state"
ItemCommand rptItems item 0: pick-image Pen (from ibtnPick)
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

<details><summary>ImageButton [defaultbutton] Enter in a panel whose DefaultButton is the image（期待される本文）</summary>

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
Request 5: IsPostBack=True
Changed chkNoViewState Checked=True
Changed hidNoViewState Value="set in code, no view state"
Click ibtnDefault at 0,0
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

<details><summary>ImageButton [dynamic][postbackevent] a dynamically created image button（期待される本文）</summary>

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
Request 6: IsPostBack=True
Changed chkNoViewState Checked=True
Changed hidNoViewState Value="set in code, no view state"
Click ibtnDynamic at 10,5
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

<details><summary>ImageButton [visible][viewstate][postbackevent] the image shown before raises Click（期待される本文）</summary>

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
Request 8: IsPostBack=True
Changed chkNoViewState Checked=True
Changed hidNoViewState Value="set in code, no view state"
Click ibtnHidden at 10,5
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

<details><summary>ImageButton [requestvalidation] markup posted as a coordinate（期待される本文）</summary>

```text
System.Web.HttpRequestValidationException
A potentially dangerous Request.Form value was detected from the client (ibtnGo.x="10,<b>x</b>").
```

</details>

<details><summary>ImageButton [crosspage] cross-page postback (PostBackUrl)（期待される本文）</summary>

```text
IsPostBack=False
IsCrossPagePostBack (this page)=False
PreviousPage: ~/BasicsScenario.aspx, IsCrossPagePostBack=True, IsPostBack=True
PreviousPage txtCross: "cross"
```

</details>

