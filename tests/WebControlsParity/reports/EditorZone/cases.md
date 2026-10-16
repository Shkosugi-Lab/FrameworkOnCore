# EditorZone テストケース一覧

変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。
このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。

- 正解データの採取: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0
- ① API 層: 235 ケース（245 メンバー）
- ② HTML 層: 221 ケース
- ③ シナリオ層: 12 スナップショット

## 機構マトリクス

型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。

| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |
|---|---|---|---|
| プロパティ → HTML 描画 | すべてのコントロール | 210 | 1 |
| ViewState の保存と復元 | Control | 6 | 10 |
| Visible=false（描画されず状態は保持） | Control.Visible | 2 | 1 |
| 名前付けコンテナと ClientIDMode | Control.ClientIDMode | 6 | 0 |
| テーマとスキン（SkinID / EnableTheming） | テーマを付けられる型（[Themeable]） | 4 | 0 |
| 動的に生成したコントロールのライフサイクル | Control | 0 | 1 |
| データバインド（DataBind / <%# %>） | Control.DataBind | 0 | 1 |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | WebControl.Enabled、HtmlControl.Disabled | 2 | 0 |
| ポストバックイベント | IPostBackEventHandler | 0 | 1 |
| テンプレート | ITemplate のプロパティ | 0 | 1 |

該当しない機構: ポストバックデータの取り込み、変更イベント（*Changed、ServerChange）、ReadOnly（ポストされた値を無視）、リクエスト検証（ValidateRequestMode）、コマンドイベントと親へのバブル（CommandName）、送信ボタン／スクリプトによるポストバック（UseSubmitBehavior）、ブラウザ側のクリック処理（OnClientClick）、クロスページポストバック（PostBackUrl）、既定のボタン（Panel の DefaultButton）、AutoPostBack（クライアントスクリプトによるポストバック）、CausesValidation / ValidationGroup、検証対象（サーバー側の検証）、検証対象（ブラウザ側の検証）、双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値）、項目（ListItem の Selected・Enabled・Text・Value と ViewState）、複数選択、繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）、ポスト値のイベント検証（一覧にない値・無効な項目の拒否）、サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）、ブラウザ側の検証スクリプト（EnableClientScript）、検証結果のまとめ（ValidationSummary）、ページング、並べ替え、項目の挿入、行・項目の編集（編集・更新・取り消し・削除）、行・項目の選択、行・項目のコマンド（RowCommand・ItemCommand へのバブル）、データキー（DataKeyNames・DataKeys）、データソースコントロールとの連携（DataSourceID）、データソース連携（DataSource・DataTextField など）、クライアントコールバック

## ① API 層

EditorZone とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の EditorZone インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。

| ケース | 期待値（.NET Framework） |
|---|---|
| <code>Api.Control.#ctor</code> | <code>Control.#ctor = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMode=In…</code> |
| <code>Api.Control.Adapter</code> | <code>Control.Adapter = null</code> |
| <code>Api.Control.AddParsedSubObject</code> | <code>Control.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>Control.AddParsedSubObject(Object) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #000000…</code> |
| <code>Api.Control.AddedControl</code> | <code>Control.AddedControl(Control,Int32) = &quot;ok&quot;</code><br><code>Control.AddedControl(Control,Int32) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000…</code> |
| <code>Api.Control.AppRelativeTemplateSourceDirectory</code> | <code>Control.AppRelativeTemplateSourceDirectory = &quot;&quot;</code><br><code>Control.AppRelativeTemplateSourceDirectory set = &quot;-&gt; System.ArgumentException: The relative virtual path &#39;Abc 123&#39; is not allowed here.&quot;</code> |
| <code>Api.Control.ApplyStyleSheetSkin</code> | <code>Control.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>Control.ApplyStyleSheetSkin(Page) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #0000000…</code> |
| <code>Api.Control.BeginRenderTracing</code> | <code>Control.BeginRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.BeginRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.BeginRenderTracing(TextWriter,Object) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Co…</code> |
| <code>Api.Control.BindingContainer</code> | <code>Control.BindingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Clien…</code> |
| <code>Api.Control.BuildProfileTree</code> | <code>Control.BuildProfileTree(String,Boolean) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.ChildControlsCreated</code> | <code>Control.ChildControlsCreated = &quot;false&quot;</code><br><code>Control.ChildControlsCreated set = &quot;ok&quot;</code> |
| <code>Api.Control.ClearCachedClientID</code> | <code>Control.ClearCachedClientID = &quot;ok&quot;</code><br><code>Control.ClearCachedClientID after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Colo…</code> |
| <code>Api.Control.ClearChildControlState</code> | <code>Control.ClearChildControlState = &quot;ok&quot;</code><br><code>Control.ClearChildControlState after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 C…</code> |
| <code>Api.Control.ClearChildState</code> | <code>Control.ClearChildState = &quot;ok&quot;</code><br><code>Control.ClearChildState after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Color [E…</code> |
| <code>Api.Control.ClearChildViewState</code> | <code>Control.ClearChildViewState = &quot;ok&quot;</code><br><code>Control.ClearChildViewState after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Colo…</code> |
| <code>Api.Control.ClearEffectiveClientIDMode</code> | <code>Control.ClearEffectiveClientIDMode = &quot;ok&quot;</code><br><code>Control.ClearEffectiveClientIDMode after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #000000…</code> |
| <code>Api.Control.ClientID</code> | <code>Control.ClientID = &quot;ezProbe&quot;</code> |
| <code>Api.Control.ClientIDMode</code> | <code>Control.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>Control.ClientIDMode set = &quot;ClientIDMode.AutoID&quot;</code> |
| <code>Api.Control.ClientIDSeparator</code> | <code>Control.ClientIDSeparator = &quot;&#39;_&#39;&quot;</code> |
| <code>Api.Control.Context</code> | <code>Control.Context = null</code> |
| <code>Api.Control.Controls</code> | <code>Control.Controls = ControlCollection []</code> |
| <code>Api.Control.CreateChildControls</code> | <code>Control.CreateChildControls = &quot;ok&quot;</code><br><code>Control.CreateChildControls after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Colo…</code> |
| <code>Api.Control.CreateControlCollection</code> | <code>Control.CreateControlCollection = ControlCollection []</code><br><code>Control.CreateControlCollection after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 …</code> |
| <code>Api.Control.DataBind</code> | <code>Control.DataBind = &quot;ok&quot;</code><br><code>Control.DataBind after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Color [Empty]; …</code><br><code>Control.DataBind(Boolean) = &quot;ok&quot;</code><br><code>Control.DataBind(Boolean) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Color …</code> |
| <code>Api.Control.DataBindChildren</code> | <code>Control.DataBindChildren = &quot;ok&quot;</code><br><code>Control.DataBindChildren after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Color […</code> |
| <code>Api.Control.DataBinding</code> | <code>Control.DataBinding = &quot;added and removed&quot;</code> |
| <code>Api.Control.DataItemContainer</code> | <code>Control.DataItemContainer = null</code> |
| <code>Api.Control.DataKeysContainer</code> | <code>Control.DataKeysContainer = null</code> |
| <code>Api.Control.DesignMode</code> | <code>Control.DesignMode = &quot;false&quot;</code> |
| <code>Api.Control.Dispose</code> | <code>Control.Dispose = &quot;ok&quot;</code><br><code>Control.Dispose after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Color [Empty]; B…</code> |
| <code>Api.Control.Disposed</code> | <code>Control.Disposed = &quot;added and removed&quot;</code> |
| <code>Api.Control.EnableTheming</code> | <code>Control.EnableTheming = &quot;true&quot;</code><br><code>Control.EnableTheming set = &quot;true&quot;</code> |
| <code>Api.Control.EnableViewState</code> | <code>Control.EnableViewState = &quot;true&quot;</code><br><code>Control.EnableViewState set = &quot;true&quot;</code> |
| <code>Api.Control.EndRenderTracing</code> | <code>Control.EndRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.EndRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.EndRenderTracing(TextWriter,Object) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Colo…</code> |
| <code>Api.Control.EnsureChildControls</code> | <code>Control.EnsureChildControls = &quot;ok&quot;</code><br><code>Control.EnsureChildControls after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Colo…</code> |
| <code>Api.Control.EnsureID</code> | <code>Control.EnsureID = &quot;ok&quot;</code><br><code>Control.EnsureID after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Color [Empty]; …</code> |
| <code>Api.Control.Events</code> | <code>Control.Events = &quot;System.ComponentModel.EventHandlerList&quot;</code> |
| <code>Api.Control.FindControl</code> | <code>Control.FindControl(String) = null</code><br><code>Control.FindControl(String) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Colo…</code><br><code>Control.FindControl(String,Int32) = null</code><br><code>Control.FindControl(String,Int32) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #0000000…</code> |
| <code>Api.Control.Focus</code> | <code>Control.Focus = &quot;-&gt; System.InvalidOperationException: A form tag with runat=server must exist on the Page to use SetFocus() or the Focus property.&quot;</code> |
| <code>Api.Control.GetDesignModeState</code> | <code>Control.GetDesignModeState = &quot;[]&quot;</code><br><code>Control.GetDesignModeState after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Color…</code> |
| <code>Api.Control.GetRouteUrl</code> | <code>Control.GetRouteUrl(Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.GetUniqueIDRelativeTo</code> | <code>Control.GetUniqueIDRelativeTo(Control) = &quot;ezProbe&quot;</code><br><code>Control.GetUniqueIDRelativeTo(Control) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00…</code> |
| <code>Api.Control.HasChildViewState</code> | <code>Control.HasChildViewState = &quot;false&quot;</code> |
| <code>Api.Control.HasControls</code> | <code>Control.HasControls = &quot;false&quot;</code><br><code>Control.HasControls after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Color [Empty…</code> |
| <code>Api.Control.HasEvents</code> | <code>Control.HasEvents = &quot;false&quot;</code><br><code>Control.HasEvents after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Color [Empty];…</code> |
| <code>Api.Control.ID</code> | <code>Control.ID = &quot;ezProbe&quot;</code><br><code>Control.ID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.IdSeparator</code> | <code>Control.IdSeparator = &quot;&#39;$&#39;&quot;</code> |
| <code>Api.Control.Init</code> | <code>Control.Init = &quot;added and removed&quot;</code> |
| <code>Api.Control.IsChildControlStateCleared</code> | <code>Control.IsChildControlStateCleared = &quot;false&quot;</code> |
| <code>Api.Control.IsLiteralContent</code> | <code>Control.IsLiteralContent = &quot;false&quot;</code><br><code>Control.IsLiteralContent after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Color […</code> |
| <code>Api.Control.IsTrackingViewState</code> | <code>Control.IsTrackingViewState = &quot;false&quot;</code> |
| <code>Api.Control.IsViewStateEnabled</code> | <code>Control.IsViewStateEnabled = &quot;true&quot;</code> |
| <code>Api.Control.Load</code> | <code>Control.Load = &quot;added and removed&quot;</code> |
| <code>Api.Control.LoadControlState</code> | <code>Control.LoadControlState(Object) = &quot;ok&quot;</code><br><code>Control.LoadControlState(Object) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000…</code> |
| <code>Api.Control.LoadViewState</code> | <code>Control.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Object[]&#39;.&quot;</code> |
| <code>Api.Control.LoadViewStateByID</code> | <code>Control.LoadViewStateByID = &quot;false&quot;</code> |
| <code>Api.Control.MapPathSecure</code> | <code>Control.MapPathSecure(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.NamingContainer</code> | <code>Control.NamingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code> |
| <code>Api.Control.OnBubbleEvent</code> | <code>Control.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Control.OnBubbleEvent(Object,EventArgs) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #0…</code> |
| <code>Api.Control.OnDataBinding</code> | <code>Control.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnDataBinding(EventArgs) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000…</code> |
| <code>Api.Control.OnInit</code> | <code>Control.OnInit(EventArgs) = &quot;-&gt; System.InvalidOperationException: You must enable Web Parts by adding a WebPartManager to your page.  The WebPartManager must be…</code> |
| <code>Api.Control.OnLoad</code> | <code>Control.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnLoad(EventArgs) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Color …</code> |
| <code>Api.Control.OnPreRender</code> | <code>Control.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnPreRender(EventArgs) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 C…</code> |
| <code>Api.Control.OnUnload</code> | <code>Control.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnUnload(EventArgs) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Colo…</code> |
| <code>Api.Control.OpenFile</code> | <code>Control.OpenFile(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.Page</code> | <code>Control.Page = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=&quot;&quot;; …</code><br><code>Control.Page set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=…</code> |
| <code>Api.Control.Parent</code> | <code>Control.Parent = PlaceHolder# {ClientID=&quot;ctl00&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;ctl00&quot;; SkinID=&quot;&quot;; UniqueID=&quot;ctl00&quot;; Valida…</code> |
| <code>Api.Control.PreRender</code> | <code>Control.PreRender = &quot;added and removed&quot;</code> |
| <code>Api.Control.RaiseBubbleEvent</code> | <code>Control.RaiseBubbleEvent(Object,EventArgs) = &quot;ok&quot;</code><br><code>Control.RaiseBubbleEvent(Object,EventArgs) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color…</code> |
| <code>Api.Control.RemovedControl</code> | <code>Control.RemovedControl(Control) = &quot;ok&quot;</code><br><code>Control.RemovedControl(Control) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 …</code> |
| <code>Api.Control.Render</code> | <code>Control.Render(HtmlTextWriter) = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code> |
| <code>Api.Control.RenderChildren</code> | <code>Control.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderChildren(HtmlTextWriter) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00…</code> |
| <code>Api.Control.RenderControl</code> | <code>Control.RenderControl(HtmlTextWriter) = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code> |
| <code>Api.Control.RenderingCompatibility</code> | <code>Control.RenderingCompatibility = &quot;System.Version 4.0&quot;</code><br><code>Control.RenderingCompatibility set = &quot;System.Version 4.0&quot;</code> |
| <code>Api.Control.ResolveAdapter</code> | <code>Control.ResolveAdapter = null</code><br><code>Control.ResolveAdapter after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Color [Em…</code> |
| <code>Api.Control.ResolveClientUrl</code> | <code>Control.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveClientUrl(String) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000…</code> |
| <code>Api.Control.ResolveUrl</code> | <code>Control.ResolveUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveUrl(String) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Color…</code> |
| <code>Api.Control.SaveControlState</code> | <code>Control.SaveControlState = null</code><br><code>Control.SaveControlState after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Color […</code> |
| <code>Api.Control.SaveViewState</code> | <code>Control.SaveViewState = null</code><br><code>Control.SaveViewState after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Color [Emp…</code> |
| <code>Api.Control.SetDesignModeState</code> | <code>Control.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>Control.SetDesignModeState(IDictionary) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #0…</code> |
| <code>Api.Control.SetRenderMethodDelegate</code> | <code>Control.SetRenderMethodDelegate(RenderMethod) = &quot;ok&quot;</code><br><code>Control.SetRenderMethodDelegate(RenderMethod) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Co…</code> |
| <code>Api.Control.SetTraceData</code> | <code>Control.SetTraceData(Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000…</code><br><code>Control.SetTraceData(Object,Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object,Object) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color…</code> |
| <code>Api.Control.Site</code> | <code>Control.Site = null</code><br><code>Control.Site set = null</code> |
| <code>Api.Control.SkinID</code> | <code>Control.SkinID = &quot;&quot;</code><br><code>Control.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.TemplateControl</code> | <code>Control.TemplateControl = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code><br><code>Control.TemplateControl set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Cl…</code> |
| <code>Api.Control.TemplateSourceDirectory</code> | <code>Control.TemplateSourceDirectory = &quot;&quot;</code> |
| <code>Api.Control.TrackViewState</code> | <code>Control.TrackViewState = &quot;ok&quot;</code><br><code>Control.TrackViewState after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Color [Em…</code> |
| <code>Api.Control.UniqueID</code> | <code>Control.UniqueID = &quot;ezProbe&quot;</code> |
| <code>Api.Control.Unload</code> | <code>Control.Unload = &quot;added and removed&quot;</code> |
| <code>Api.Control.ValidateRequestMode</code> | <code>Control.ValidateRequestMode = &quot;ValidateRequestMode.Inherit&quot;</code><br><code>Control.ValidateRequestMode set = &quot;ValidateRequestMode.Disabled&quot;</code> |
| <code>Api.Control.ViewState</code> | <code>Control.ViewState = StateBag []</code> |
| <code>Api.Control.ViewStateIgnoresCase</code> | <code>Control.ViewStateIgnoresCase = &quot;false&quot;</code> |
| <code>Api.Control.ViewStateMode</code> | <code>Control.ViewStateMode = &quot;ViewStateMode.Inherit&quot;</code><br><code>Control.ViewStateMode set = &quot;ViewStateMode.Enabled&quot;</code> |
| <code>Api.Control.Visible</code> | <code>Control.Visible = &quot;false&quot;</code><br><code>Control.Visible set = &quot;-&gt; System.InvalidOperationException: Cannot set the Visible property of a Tool Zone.  Override the ToolZone.Display property instead.&quot;</code> |
| <code>Api.IAttributeAccessor.GetAttribute</code> | <code>IAttributeAccessor.GetAttribute(String) = null</code><br><code>IAttributeAccessor.GetAttribute(String) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #0…</code> |
| <code>Api.IAttributeAccessor.SetAttribute</code> | <code>IAttributeAccessor.SetAttribute(String,String) = &quot;ok&quot;</code><br><code>IAttributeAccessor.SetAttribute(String,String) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=C…</code> |
| <code>Api.IControlBuilderAccessor.ControlBuilder</code> | <code>IControlBuilderAccessor.ControlBuilder = null</code> |
| <code>Api.IControlDesignerAccessor.GetDesignModeState</code> | <code>IControlDesignerAccessor.GetDesignModeState = &quot;[]&quot;</code><br><code>IControlDesignerAccessor.GetDesignModeState after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Colo…</code> |
| <code>Api.IControlDesignerAccessor.SetDesignModeState</code> | <code>IControlDesignerAccessor.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetDesignModeState(IDictionary) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; Bor…</code> |
| <code>Api.IControlDesignerAccessor.SetOwnerControl</code> | <code>IControlDesignerAccessor.SetOwnerControl(Control) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetOwnerControl(Control) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColo…</code> |
| <code>Api.IControlDesignerAccessor.UserData</code> | <code>IControlDesignerAccessor.UserData = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.DataBindings</code> | <code>IDataBindingsAccessor.DataBindings = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.HasDataBindings</code> | <code>IDataBindingsAccessor.HasDataBindings = &quot;false&quot;</code> |
| <code>Api.IExpressionsAccessor.Expressions</code> | <code>IExpressionsAccessor.Expressions = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.HasExpressions</code> | <code>IExpressionsAccessor.HasExpressions = &quot;false&quot;</code> |
| <code>Api.IParserAccessor.AddParsedSubObject</code> | <code>IParserAccessor.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>IParserAccessor.AddParsedSubObject(Object) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color…</code> |
| <code>Api.IPostBackEventHandler.RaisePostBackEvent</code> | <code>IPostBackEventHandler.RaisePostBackEvent(String) = &quot;ok&quot;</code><br><code>IPostBackEventHandler.RaisePostBackEvent(String) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor…</code> |
| <code>Api.IUrlResolutionService.ResolveClientUrl</code> | <code>IUrlResolutionService.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>IUrlResolutionService.ResolveClientUrl(String) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=C…</code> |
| <code>Api.WebControls.CompositeControl.#ctor</code> | <code>CompositeControl.#ctor = &quot;-&gt; System.MemberAccessException: Cannot create an instance of System.Web.UI.WebControls.CompositeControl because it is an abstract cla…</code> |
| <code>Api.WebControls.CompositeControl.Controls</code> | <code>CompositeControl.Controls = ControlCollection []</code> |
| <code>Api.WebControls.CompositeControl.DataBind</code> | <code>CompositeControl.DataBind = &quot;ok&quot;</code><br><code>CompositeControl.DataBind after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Color …</code> |
| <code>Api.WebControls.CompositeControl.RecreateChildControls</code> | <code>CompositeControl.RecreateChildControls = &quot;ok&quot;</code><br><code>CompositeControl.RecreateChildControls after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00…</code> |
| <code>Api.WebControls.CompositeControl.Render</code> | <code>CompositeControl.Render(HtmlTextWriter) = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code> |
| <code>Api.WebControls.CompositeControl.SupportsDisabledAttribute</code> | <code>CompositeControl.SupportsDisabledAttribute = &quot;false&quot;</code> |
| <code>Api.WebControls.ICompositeControlDesignerAccessor.RecreateChildControls</code> | <code>ICompositeControlDesignerAccessor.RecreateChildControls = &quot;ok&quot;</code><br><code>ICompositeControlDesignerAccessor.RecreateChildControls after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; Bord…</code> |
| <code>Api.WebControls.WebControl.#ctor</code> | <code>WebControl.#ctor = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidt…</code><br><code>WebControl.#ctor(String) = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; Bo…</code><br><code>WebControl.#ctor(HtmlTextWriterTag) = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle…</code> |
| <code>Api.WebControls.WebControl.AccessKey</code> | <code>WebControl.AccessKey = &quot;&quot;</code><br><code>WebControl.AccessKey set = &quot;-&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]&quot;</code> |
| <code>Api.WebControls.WebControl.AddAttributesToRender</code> | <code>WebControl.AddAttributesToRender(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.AddAttributesToRender(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>WebControl.AddAttributesToRender(HtmlTextWriter) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor…</code> |
| <code>Api.WebControls.WebControl.ApplyStyle</code> | <code>WebControl.ApplyStyle(Style) = &quot;ok&quot;</code><br><code>WebControl.ApplyStyle(Style) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Col…</code> |
| <code>Api.WebControls.WebControl.Attributes</code> | <code>WebControl.Attributes = AttributeCollection []</code> |
| <code>Api.WebControls.WebControl.BackColor</code> | <code>WebControl.BackColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>WebControl.BackColor set = &quot;Color #FF3366CC Color [A=255, R=51, G=102, B=204]&quot;</code> |
| <code>Api.WebControls.WebControl.BorderColor</code> | <code>WebControl.BorderColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>WebControl.BorderColor set = &quot;Color #FF3366CC Color [A=255, R=51, G=102, B=204]&quot;</code> |
| <code>Api.WebControls.WebControl.BorderStyle</code> | <code>WebControl.BorderStyle = &quot;BorderStyle.NotSet&quot;</code><br><code>WebControl.BorderStyle set = &quot;BorderStyle.None&quot;</code> |
| <code>Api.WebControls.WebControl.BorderWidth</code> | <code>WebControl.BorderWidth = Unit </code><br><code>WebControl.BorderWidth set = Unit 120px</code> |
| <code>Api.WebControls.WebControl.ControlStyle</code> | <code>WebControl.ControlStyle = Style {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; CssClass=…</code> |
| <code>Api.WebControls.WebControl.ControlStyleCreated</code> | <code>WebControl.ControlStyleCreated = &quot;false&quot;</code> |
| <code>Api.WebControls.WebControl.CopyBaseAttributes</code> | <code>WebControl.CopyBaseAttributes(WebControl) = &quot;ok&quot;</code><br><code>WebControl.CopyBaseAttributes(WebControl) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color …</code> |
| <code>Api.WebControls.WebControl.CreateControlStyle</code> | <code>WebControl.CreateControlStyle = Style {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; Css…</code><br><code>WebControl.CreateControlStyle after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Co…</code> |
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
| <code>Api.WebControls.WebControl.MergeStyle</code> | <code>WebControl.MergeStyle(Style) = &quot;ok&quot;</code><br><code>WebControl.MergeStyle(Style) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Col…</code> |
| <code>Api.WebControls.WebControl.Render</code> | <code>WebControl.Render(HtmlTextWriter) = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code> |
| <code>Api.WebControls.WebControl.RenderBeginTag</code> | <code>WebControl.RenderBeginTag(HtmlTextWriter) = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code> |
| <code>Api.WebControls.WebControl.RenderContents</code> | <code>WebControl.RenderContents(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.RenderContents(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;tr&gt;</code><br><code>  &#124; 	&lt;td style=&quot;white-space:nowrap;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;</code><br><code>  &#124; 		&lt;tr&gt;</code><br><code>  &#124; 			&lt;td style=&quot;width:100%;white-space:nowrap;&quot;&gt;Editor Zone&lt;/td&gt;&lt;td style=&quot;white-space:nowrap;&quot;&gt;&lt;a title=&quot;Closes Zone&quot; href=&quot;javascript:__doPostBack(&amp;#39;ezPr…</code><br><code>  &#124; 		&lt;/tr&gt;</code><br><code>  &#124; 	&lt;/table&gt;&lt;/td&gt;</code><br><code>  &#124; &lt;/tr&gt;&lt;tr&gt;</code><br><code>  &#124; 	&lt;td style=&quot;height:100%;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;height:100%;&quot;&gt;</code><br><code>  &#124; 		&lt;tr&gt;</code><br><code>  &#124; 			&lt;td valign=&quot;top&quot;&gt;Editor Zone contains no Editor Parts.&lt;/td&gt;</code><br><code>  &#124; 		&lt;/tr&gt;</code><br><code>  &#124; 	&lt;/table&gt;&lt;/td&gt;</code><br><code>  &#124; &lt;/tr&gt;&lt;tr&gt;</code><br><code>  &#124; 	&lt;td style=&quot;white-space:nowrap;&quot;&gt;&lt;div style=&quot;margin:4px;&quot;&gt;</code><br><code>  &#124; 		&lt;input type=&quot;button&quot; value=&quot;OK&quot; onclick=&quot;javascript:__doPostBack(&amp;#39;ezProbe&amp;#39;,&amp;#39;ok&amp;#39;)&quot; title=&quot;Applies changes and closes editor&quot; /&gt;&amp;nbsp;&lt;input…</code><br><code>  &#124; 	&lt;/div&gt;&lt;/td&gt;</code><br><code>  &#124; &lt;/tr&gt;</code><br><code>WebControl.RenderContents(HtmlTextWriter) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color …</code> |
| <code>Api.WebControls.WebControl.RenderEndTag</code> | <code>WebControl.RenderEndTag(HtmlTextWriter) = &quot;-&gt; System.InvalidOperationException: A PopEndTag was called without a corresponding PushEndTag.&quot;</code> |
| <code>Api.WebControls.WebControl.SaveViewState</code> | <code>WebControl.SaveViewState = null</code><br><code>WebControl.SaveViewState after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Color […</code> |
| <code>Api.WebControls.WebControl.SkinID</code> | <code>WebControl.SkinID = &quot;&quot;</code><br><code>WebControl.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.Style</code> | <code>WebControl.Style = CssStyleCollection {Count=Int32 0; Value=null}</code> |
| <code>Api.WebControls.WebControl.SupportsDisabledAttribute</code> | <code>WebControl.SupportsDisabledAttribute = &quot;false&quot;</code> |
| <code>Api.WebControls.WebControl.TabIndex</code> | <code>WebControl.TabIndex = &quot;Int16 0&quot;</code><br><code>WebControl.TabIndex set = &quot;Int16 1&quot;</code> |
| <code>Api.WebControls.WebControl.TagKey</code> | <code>WebControl.TagKey = &quot;HtmlTextWriterTag.Table&quot;</code> |
| <code>Api.WebControls.WebControl.TagName</code> | <code>WebControl.TagName = &quot;table&quot;</code> |
| <code>Api.WebControls.WebControl.ToolTip</code> | <code>WebControl.ToolTip = &quot;&quot;</code><br><code>WebControl.ToolTip set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.TrackViewState</code> | <code>WebControl.TrackViewState = &quot;ok&quot;</code><br><code>WebControl.TrackViewState after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Color …</code> |
| <code>Api.WebControls.WebControl.Width</code> | <code>WebControl.Width = Unit </code><br><code>WebControl.Width set = Unit 120px</code> |
| <code>Api.WebControls.WebParts.EditorZone.#ctor</code> | <code>WebParts.EditorZone.#ctor = EditorZone# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Color [Empty]; Bord…</code> |
| <code>Api.WebControls.WebParts.EditorZone.CreateEditorParts</code> | <code>WebParts.EditorZone.CreateEditorParts = &quot;[]&quot;</code><br><code>WebParts.EditorZone.CreateEditorParts after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #000…</code> |
| <code>Api.WebControls.WebParts.EditorZone.ZoneTemplate</code> | <code>WebParts.EditorZone.ZoneTemplate = null</code><br><code>WebParts.EditorZone.ZoneTemplate set = null</code> |
| <code>Api.WebControls.WebParts.EditorZoneBase.#ctor</code> | <code>WebParts.EditorZoneBase.#ctor = &quot;-&gt; System.MemberAccessException: Cannot create an instance of System.Web.UI.WebControls.WebParts.EditorZoneBase because it is a…</code> |
| <code>Api.WebControls.WebParts.EditorZoneBase.ApplyVerb</code> | <code>WebParts.EditorZoneBase.ApplyVerb = WebPartEditorApplyVerb {Checked=false; ClientClickHandler=&quot;&quot;; Description=&quot;Applies changes&quot;; Enabled=true; ID=&quot;&quot;; ImageUrl=&quot;…</code> |
| <code>Api.WebControls.WebParts.EditorZoneBase.CancelVerb</code> | <code>WebParts.EditorZoneBase.CancelVerb = WebPartEditorCancelVerb {Checked=false; ClientClickHandler=&quot;&quot;; Description=&quot;Cancels changes&quot;; Enabled=true; ID=&quot;&quot;; ImageUrl…</code> |
| <code>Api.WebControls.WebParts.EditorZoneBase.Close</code> | <code>WebParts.EditorZoneBase.Close = &quot;ok&quot;</code><br><code>WebParts.EditorZoneBase.Close after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Co…</code> |
| <code>Api.WebControls.WebParts.EditorZoneBase.CreateChildControls</code> | <code>WebParts.EditorZoneBase.CreateChildControls = &quot;ok&quot;</code><br><code>WebParts.EditorZoneBase.CreateChildControls after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Colo…</code> |
| <code>Api.WebControls.WebParts.EditorZoneBase.CreateEditorPartChrome</code> | <code>WebParts.EditorZoneBase.CreateEditorPartChrome = EditorPartChrome {}</code><br><code>WebParts.EditorZoneBase.CreateEditorPartChrome after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=C…</code> |
| <code>Api.WebControls.WebParts.EditorZoneBase.CreateEditorParts</code> | <code>WebParts.EditorZoneBase.CreateEditorParts = &quot;[]&quot;</code><br><code>WebParts.EditorZoneBase.CreateEditorParts after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color …</code> |
| <code>Api.WebControls.WebParts.EditorZoneBase.Display</code> | <code>WebParts.EditorZoneBase.Display = &quot;false&quot;</code> |
| <code>Api.WebControls.WebParts.EditorZoneBase.EditorPartChrome</code> | <code>WebParts.EditorZoneBase.EditorPartChrome = EditorPartChrome {}</code> |
| <code>Api.WebControls.WebParts.EditorZoneBase.EditorParts</code> | <code>WebParts.EditorZoneBase.EditorParts = &quot;[]&quot;</code> |
| <code>Api.WebControls.WebParts.EditorZoneBase.EmptyZoneText</code> | <code>WebParts.EditorZoneBase.EmptyZoneText = &quot;Editor Zone contains no Editor Parts.&quot;</code><br><code>WebParts.EditorZoneBase.EmptyZoneText set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebParts.EditorZoneBase.ErrorText</code> | <code>WebParts.EditorZoneBase.ErrorText = &quot;There was an error applying one or more Editor Parts.&quot;</code><br><code>WebParts.EditorZoneBase.ErrorText set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebParts.EditorZoneBase.HeaderText</code> | <code>WebParts.EditorZoneBase.HeaderText = &quot;Editor Zone&quot;</code><br><code>WebParts.EditorZoneBase.HeaderText set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebParts.EditorZoneBase.InstructionText</code> | <code>WebParts.EditorZoneBase.InstructionText = &quot;Modify the properties of the Web Part, then click OK or Apply to apply your changes.&quot;</code><br><code>WebParts.EditorZoneBase.InstructionText set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebParts.EditorZoneBase.InvalidateEditorParts</code> | <code>WebParts.EditorZoneBase.InvalidateEditorParts = &quot;ok&quot;</code><br><code>WebParts.EditorZoneBase.InvalidateEditorParts after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Co…</code> |
| <code>Api.WebControls.WebParts.EditorZoneBase.LoadViewState</code> | <code>WebParts.EditorZoneBase.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Object[]&#39;.&quot;</code> |
| <code>Api.WebControls.WebParts.EditorZoneBase.OKVerb</code> | <code>WebParts.EditorZoneBase.OKVerb = WebPartEditorOKVerb {Checked=false; ClientClickHandler=&quot;&quot;; Description=&quot;Applies changes and closes editor&quot;; Enabled=true; ID=&quot;&quot;…</code> |
| <code>Api.WebControls.WebParts.EditorZoneBase.OnDisplayModeChanged</code> | <code>WebParts.EditorZoneBase.OnDisplayModeChanged(Object,WebParts.WebPartDisplayModeEventArgs) = &quot;ok&quot;</code><br><code>WebParts.EditorZoneBase.OnDisplayModeChanged(Object,WebParts.WebPartDisplayModeEventArgs) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Co…</code> |
| <code>Api.WebControls.WebParts.EditorZoneBase.OnPreRender</code> | <code>WebParts.EditorZoneBase.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>WebParts.EditorZoneBase.OnPreRender(EventArgs) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=C…</code> |
| <code>Api.WebControls.WebParts.EditorZoneBase.OnSelectedWebPartChanged</code> | <code>WebParts.EditorZoneBase.OnSelectedWebPartChanged(Object,WebParts.WebPartEventArgs) = &quot;ok&quot;</code><br><code>WebParts.EditorZoneBase.OnSelectedWebPartChanged(Object,WebParts.WebPartEventArgs) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Em…</code> |
| <code>Api.WebControls.WebParts.EditorZoneBase.RaisePostBackEvent</code> | <code>WebParts.EditorZoneBase.RaisePostBackEvent(String) = &quot;ok&quot;</code><br><code>WebParts.EditorZoneBase.RaisePostBackEvent(String) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderCol…</code> |
| <code>Api.WebControls.WebParts.EditorZoneBase.Render</code> | <code>WebParts.EditorZoneBase.Render(HtmlTextWriter) = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code> |
| <code>Api.WebControls.WebParts.EditorZoneBase.RenderBody</code> | <code>WebParts.EditorZoneBase.RenderBody(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebParts.EditorZoneBase.RenderBody(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;height:100%;&quot;&gt;</code><br><code>  &#124; 	&lt;tr&gt;</code><br><code>  &#124; 		&lt;td valign=&quot;top&quot;&gt;Editor Zone contains no Editor Parts.&lt;/td&gt;</code><br><code>  &#124; 	&lt;/tr&gt;</code><br><code>  &#124; &lt;/table&gt;</code><br><code>WebParts.EditorZoneBase.RenderBody(HtmlTextWriter) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderCol…</code> |
| <code>Api.WebControls.WebParts.EditorZoneBase.RenderVerbs</code> | <code>WebParts.EditorZoneBase.RenderVerbs(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebParts.EditorZoneBase.RenderVerbs(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;input type=&quot;button&quot; value=&quot;OK&quot; onclick=&quot;javascript:__doPostBack(&amp;#39;ezProbe&amp;#39;,&amp;#39;ok&amp;#39;)&quot; title=&quot;Applies changes and closes editor&quot; /&gt;&amp;nbsp;&lt;input t…</code><br><code>WebParts.EditorZoneBase.RenderVerbs(HtmlTextWriter) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderCo…</code> |
| <code>Api.WebControls.WebParts.EditorZoneBase.SaveViewState</code> | <code>WebParts.EditorZoneBase.SaveViewState = null</code><br><code>WebParts.EditorZoneBase.SaveViewState after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #000…</code> |
| <code>Api.WebControls.WebParts.EditorZoneBase.TrackViewState</code> | <code>WebParts.EditorZoneBase.TrackViewState = &quot;ok&quot;</code><br><code>WebParts.EditorZoneBase.TrackViewState after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00…</code> |
| <code>Api.WebControls.WebParts.EditorZoneBase.WebPartToEdit</code> | <code>WebParts.EditorZoneBase.WebPartToEdit = null</code> |
| <code>Api.WebControls.WebParts.ToolZone.#ctor</code> | <code>WebParts.ToolZone.#ctor(ICollection) = &quot;-&gt; System.MemberAccessException: Cannot create an instance of System.Web.UI.WebControls.WebParts.ToolZone because it is …</code><br><code>WebParts.ToolZone.#ctor(WebParts.WebPartDisplayMode) = &quot;-&gt; System.MemberAccessException: Cannot create an instance of System.Web.UI.WebControls.WebParts.ToolZon…</code> |
| <code>Api.WebControls.WebParts.ToolZone.AssociatedDisplayModes</code> | <code>WebParts.ToolZone.AssociatedDisplayModes = &quot;[System.Web.UI.WebControls.WebParts.WebPartManager+EditWebPartDisplayMode]&quot;</code> |
| <code>Api.WebControls.WebParts.ToolZone.Close</code> | <code>WebParts.ToolZone.Close = &quot;ok&quot;</code><br><code>WebParts.ToolZone.Close after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Color [E…</code> |
| <code>Api.WebControls.WebParts.ToolZone.Display</code> | <code>WebParts.ToolZone.Display = &quot;false&quot;</code> |
| <code>Api.WebControls.WebParts.ToolZone.EditUIStyle</code> | <code>WebParts.ToolZone.EditUIStyle = Style {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; Css…</code> |
| <code>Api.WebControls.WebParts.ToolZone.HeaderCloseVerb</code> | <code>WebParts.ToolZone.HeaderCloseVerb = WebPartHeaderCloseVerb {Checked=false; ClientClickHandler=&quot;&quot;; Description=&quot;Closes Zone&quot;; Enabled=true; ID=&quot;&quot;; ImageUrl=&quot;&quot;; T…</code> |
| <code>Api.WebControls.WebParts.ToolZone.HeaderVerbStyle</code> | <code>WebParts.ToolZone.HeaderVerbStyle = Style {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=;…</code> |
| <code>Api.WebControls.WebParts.ToolZone.InstructionText</code> | <code>WebParts.ToolZone.InstructionText = &quot;Modify the properties of the Web Part, then click OK or Apply to apply your changes.&quot;</code><br><code>WebParts.ToolZone.InstructionText set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebParts.ToolZone.InstructionTextStyle</code> | <code>WebParts.ToolZone.InstructionTextStyle = Style {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWi…</code> |
| <code>Api.WebControls.WebParts.ToolZone.LabelStyle</code> | <code>WebParts.ToolZone.LabelStyle = Style {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; CssC…</code> |
| <code>Api.WebControls.WebParts.ToolZone.LoadViewState</code> | <code>WebParts.ToolZone.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Object[]&#39;.&quot;</code> |
| <code>Api.WebControls.WebParts.ToolZone.OnDisplayModeChanged</code> | <code>WebParts.ToolZone.OnDisplayModeChanged(Object,WebParts.WebPartDisplayModeEventArgs) = &quot;ok&quot;</code><br><code>WebParts.ToolZone.OnDisplayModeChanged(Object,WebParts.WebPartDisplayModeEventArgs) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [E…</code> |
| <code>Api.WebControls.WebParts.ToolZone.OnInit</code> | <code>WebParts.ToolZone.OnInit(EventArgs) = &quot;-&gt; System.InvalidOperationException: You must enable Web Parts by adding a WebPartManager to your page.  The WebPartManag…</code> |
| <code>Api.WebControls.WebParts.ToolZone.OnSelectedWebPartChanged</code> | <code>WebParts.ToolZone.OnSelectedWebPartChanged(Object,WebParts.WebPartEventArgs) = &quot;ok&quot;</code><br><code>WebParts.ToolZone.OnSelectedWebPartChanged(Object,WebParts.WebPartEventArgs) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; …</code> |
| <code>Api.WebControls.WebParts.ToolZone.RaisePostBackEvent</code> | <code>WebParts.ToolZone.RaisePostBackEvent(String) = &quot;ok&quot;</code><br><code>WebParts.ToolZone.RaisePostBackEvent(String) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Col…</code> |
| <code>Api.WebControls.WebParts.ToolZone.RenderFooter</code> | <code>WebParts.ToolZone.RenderFooter(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebParts.ToolZone.RenderFooter(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;div style=&quot;margin:4px;&quot;&gt;</code><br><code>  &#124; 	&lt;input type=&quot;button&quot; value=&quot;OK&quot; onclick=&quot;javascript:__doPostBack(&amp;#39;ezProbe&amp;#39;,&amp;#39;ok&amp;#39;)&quot; title=&quot;Applies changes and closes editor&quot; /&gt;&amp;nbsp;&lt;input …</code><br><code>  &#124; &lt;/div&gt;</code><br><code>WebParts.ToolZone.RenderFooter(HtmlTextWriter) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=C…</code> |
| <code>Api.WebControls.WebParts.ToolZone.RenderHeader</code> | <code>WebParts.ToolZone.RenderHeader(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebParts.ToolZone.RenderHeader(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;</code><br><code>  &#124; 	&lt;tr&gt;</code><br><code>  &#124; 		&lt;td style=&quot;width:100%;white-space:nowrap;&quot;&gt;Editor Zone&lt;/td&gt;&lt;td style=&quot;white-space:nowrap;&quot;&gt;&lt;a title=&quot;Closes Zone&quot; href=&quot;javascript:__doPostBack(&amp;#39;ezPro…</code><br><code>  &#124; 	&lt;/tr&gt;</code><br><code>  &#124; &lt;/table&gt;</code><br><code>WebParts.ToolZone.RenderHeader(HtmlTextWriter) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=C…</code> |
| <code>Api.WebControls.WebParts.ToolZone.RenderVerb</code> | <code>WebParts.ToolZone.RenderVerb(HtmlTextWriter,WebParts.WebPartVerb) = &quot;ok&quot;</code><br><code>WebParts.ToolZone.RenderVerb(HtmlTextWriter,WebParts.WebPartVerb) writer:</code><br><code>  &#124; &lt;input type=&quot;button&quot; value=&quot;&quot; onclick=&quot;javascript:__doPostBack(&amp;#39;&amp;#39;,&amp;#39;&amp;#39;)&quot; /&gt;</code><br><code>WebParts.ToolZone.RenderVerb(HtmlTextWriter,WebParts.WebPartVerb) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUr…</code> |
| <code>Api.WebControls.WebParts.ToolZone.RenderVerbs</code> | <code>WebParts.ToolZone.RenderVerbs(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebParts.ToolZone.RenderVerbs(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;input type=&quot;button&quot; value=&quot;OK&quot; onclick=&quot;javascript:__doPostBack(&amp;#39;ezProbe&amp;#39;,&amp;#39;ok&amp;#39;)&quot; title=&quot;Applies changes and closes editor&quot; /&gt;&amp;nbsp;&lt;input t…</code><br><code>WebParts.ToolZone.RenderVerbs(HtmlTextWriter) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Co…</code> |
| <code>Api.WebControls.WebParts.ToolZone.SaveViewState</code> | <code>WebParts.ToolZone.SaveViewState = null</code><br><code>WebParts.ToolZone.SaveViewState after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 …</code> |
| <code>Api.WebControls.WebParts.ToolZone.TrackViewState</code> | <code>WebParts.ToolZone.TrackViewState = &quot;ok&quot;</code><br><code>WebParts.ToolZone.TrackViewState after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000…</code> |
| <code>Api.WebControls.WebParts.ToolZone.Visible</code> | <code>WebParts.ToolZone.Visible = &quot;false&quot;</code><br><code>WebParts.ToolZone.Visible set = &quot;-&gt; System.InvalidOperationException: Cannot set the Visible property of a Tool Zone.  Override the ToolZone.Display property in…</code> |
| <code>Api.WebControls.WebParts.WebZone.BackImageUrl</code> | <code>WebParts.WebZone.BackImageUrl = &quot;&quot;</code><br><code>WebParts.WebZone.BackImageUrl set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebParts.WebZone.EmptyZoneText</code> | <code>WebParts.WebZone.EmptyZoneText = &quot;Editor Zone contains no Editor Parts.&quot;</code><br><code>WebParts.WebZone.EmptyZoneText set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebParts.WebZone.EmptyZoneTextStyle</code> | <code>WebParts.WebZone.EmptyZoneTextStyle = Style {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth…</code> |
| <code>Api.WebControls.WebParts.WebZone.ErrorStyle</code> | <code>WebParts.WebZone.ErrorStyle = ErrorStyle {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; …</code> |
| <code>Api.WebControls.WebParts.WebZone.FooterStyle</code> | <code>WebParts.WebZone.FooterStyle = TitleStyle {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=;…</code> |
| <code>Api.WebControls.WebParts.WebZone.GetEffectiveChromeType</code> | <code>WebParts.WebZone.GetEffectiveChromeType(WebParts.Part) = &quot;-&gt; System.ArgumentNullException: Value cannot be null. [param part]&quot;</code> |
| <code>Api.WebControls.WebParts.WebZone.HasFooter</code> | <code>WebParts.WebZone.HasFooter = &quot;true&quot;</code> |
| <code>Api.WebControls.WebParts.WebZone.HasHeader</code> | <code>WebParts.WebZone.HasHeader = &quot;true&quot;</code> |
| <code>Api.WebControls.WebParts.WebZone.HeaderStyle</code> | <code>WebParts.WebZone.HeaderStyle = TitleStyle {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=;…</code> |
| <code>Api.WebControls.WebParts.WebZone.HeaderText</code> | <code>WebParts.WebZone.HeaderText = &quot;Editor Zone&quot;</code><br><code>WebParts.WebZone.HeaderText set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebParts.WebZone.LoadViewState</code> | <code>WebParts.WebZone.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Object[]&#39;.&quot;</code> |
| <code>Api.WebControls.WebParts.WebZone.OnInit</code> | <code>WebParts.WebZone.OnInit(EventArgs) = &quot;-&gt; System.InvalidOperationException: You must enable Web Parts by adding a WebPartManager to your page.  The WebPartManage…</code> |
| <code>Api.WebControls.WebParts.WebZone.OnPreRender</code> | <code>WebParts.WebZone.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>WebParts.WebZone.OnPreRender(EventArgs) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #0…</code> |
| <code>Api.WebControls.WebParts.WebZone.Padding</code> | <code>WebParts.WebZone.Padding = &quot;Int32 2&quot;</code><br><code>WebParts.WebZone.Padding set = &quot;Int32 1&quot;</code> |
| <code>Api.WebControls.WebParts.WebZone.PartChromePadding</code> | <code>WebParts.WebZone.PartChromePadding = Unit 5px</code><br><code>WebParts.WebZone.PartChromePadding set = Unit 120px</code> |
| <code>Api.WebControls.WebParts.WebZone.PartChromeStyle</code> | <code>WebParts.WebZone.PartChromeStyle = Style {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; …</code> |
| <code>Api.WebControls.WebParts.WebZone.PartChromeType</code> | <code>WebParts.WebZone.PartChromeType = &quot;PartChromeType.Default&quot;</code><br><code>WebParts.WebZone.PartChromeType set = &quot;PartChromeType.TitleAndBorder&quot;</code> |
| <code>Api.WebControls.WebParts.WebZone.PartStyle</code> | <code>WebParts.WebZone.PartStyle = TableStyle {BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet…</code> |
| <code>Api.WebControls.WebParts.WebZone.PartTitleStyle</code> | <code>WebParts.WebZone.PartTitleStyle = TitleStyle {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidt…</code> |
| <code>Api.WebControls.WebParts.WebZone.RenderBeginTag</code> | <code>WebParts.WebZone.RenderBeginTag(HtmlTextWriter) = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code> |
| <code>Api.WebControls.WebParts.WebZone.RenderBody</code> | <code>WebParts.WebZone.RenderBody(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebParts.WebZone.RenderBody(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;height:100%;&quot;&gt;</code><br><code>  &#124; 	&lt;tr&gt;</code><br><code>  &#124; 		&lt;td valign=&quot;top&quot;&gt;Editor Zone contains no Editor Parts.&lt;/td&gt;</code><br><code>  &#124; 	&lt;/tr&gt;</code><br><code>  &#124; &lt;/table&gt;</code><br><code>WebParts.WebZone.RenderBody(HtmlTextWriter) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Colo…</code> |
| <code>Api.WebControls.WebParts.WebZone.RenderClientScript</code> | <code>WebParts.WebZone.RenderClientScript = &quot;false&quot;</code> |
| <code>Api.WebControls.WebParts.WebZone.RenderContents</code> | <code>WebParts.WebZone.RenderContents(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebParts.WebZone.RenderContents(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;tr&gt;</code><br><code>  &#124; 	&lt;td style=&quot;white-space:nowrap;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;</code><br><code>  &#124; 		&lt;tr&gt;</code><br><code>  &#124; 			&lt;td style=&quot;width:100%;white-space:nowrap;&quot;&gt;Editor Zone&lt;/td&gt;&lt;td style=&quot;white-space:nowrap;&quot;&gt;&lt;a title=&quot;Closes Zone&quot; href=&quot;javascript:__doPostBack(&amp;#39;ezPr…</code><br><code>  &#124; 		&lt;/tr&gt;</code><br><code>  &#124; 	&lt;/table&gt;&lt;/td&gt;</code><br><code>  &#124; &lt;/tr&gt;&lt;tr&gt;</code><br><code>  &#124; 	&lt;td style=&quot;height:100%;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;height:100%;&quot;&gt;</code><br><code>  &#124; 		&lt;tr&gt;</code><br><code>  &#124; 			&lt;td valign=&quot;top&quot;&gt;Editor Zone contains no Editor Parts.&lt;/td&gt;</code><br><code>  &#124; 		&lt;/tr&gt;</code><br><code>  &#124; 	&lt;/table&gt;&lt;/td&gt;</code><br><code>  &#124; &lt;/tr&gt;&lt;tr&gt;</code><br><code>  &#124; 	&lt;td style=&quot;white-space:nowrap;&quot;&gt;&lt;div style=&quot;margin:4px;&quot;&gt;</code><br><code>  &#124; 		&lt;input type=&quot;button&quot; value=&quot;OK&quot; onclick=&quot;javascript:__doPostBack(&amp;#39;ezProbe&amp;#39;,&amp;#39;ok&amp;#39;)&quot; title=&quot;Applies changes and closes editor&quot; /&gt;&amp;nbsp;&lt;input…</code><br><code>  &#124; 	&lt;/div&gt;&lt;/td&gt;</code><br><code>  &#124; &lt;/tr&gt;</code><br><code>WebParts.WebZone.RenderContents(HtmlTextWriter) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=…</code> |
| <code>Api.WebControls.WebParts.WebZone.RenderFooter</code> | <code>WebParts.WebZone.RenderFooter(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebParts.WebZone.RenderFooter(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;div style=&quot;margin:4px;&quot;&gt;</code><br><code>  &#124; 	&lt;input type=&quot;button&quot; value=&quot;OK&quot; onclick=&quot;javascript:__doPostBack(&amp;#39;ezProbe&amp;#39;,&amp;#39;ok&amp;#39;)&quot; title=&quot;Applies changes and closes editor&quot; /&gt;&amp;nbsp;&lt;input …</code><br><code>  &#124; &lt;/div&gt;</code><br><code>WebParts.WebZone.RenderFooter(HtmlTextWriter) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Co…</code> |
| <code>Api.WebControls.WebParts.WebZone.RenderHeader</code> | <code>WebParts.WebZone.RenderHeader(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebParts.WebZone.RenderHeader(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;</code><br><code>  &#124; 	&lt;tr&gt;</code><br><code>  &#124; 		&lt;td style=&quot;width:100%;white-space:nowrap;&quot;&gt;Editor Zone&lt;/td&gt;&lt;td style=&quot;white-space:nowrap;&quot;&gt;&lt;a title=&quot;Closes Zone&quot; href=&quot;javascript:__doPostBack(&amp;#39;ezPro…</code><br><code>  &#124; 	&lt;/tr&gt;</code><br><code>  &#124; &lt;/table&gt;</code><br><code>WebParts.WebZone.RenderHeader(HtmlTextWriter) after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Co…</code> |
| <code>Api.WebControls.WebParts.WebZone.SaveViewState</code> | <code>WebParts.WebZone.SaveViewState = null</code><br><code>WebParts.WebZone.SaveViewState after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 C…</code> |
| <code>Api.WebControls.WebParts.WebZone.TagKey</code> | <code>WebParts.WebZone.TagKey = &quot;HtmlTextWriterTag.Table&quot;</code> |
| <code>Api.WebControls.WebParts.WebZone.TrackViewState</code> | <code>WebParts.WebZone.TrackViewState = &quot;ok&quot;</code><br><code>WebParts.WebZone.TrackViewState after = EditorZone#ezProbe {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 …</code> |
| <code>Api.WebControls.WebParts.WebZone.VerbButtonType</code> | <code>WebParts.WebZone.VerbButtonType = &quot;ButtonType.Button&quot;</code><br><code>WebParts.WebZone.VerbButtonType set = &quot;ButtonType.Image&quot;</code> |
| <code>Api.WebControls.WebParts.WebZone.VerbStyle</code> | <code>WebParts.WebZone.VerbStyle = Style {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; CssCla…</code> |
| <code>Api.WebControls.WebParts.WebZone.WebPartManager</code> | <code>WebParts.WebZone.WebPartManager = null</code> |

## ② HTML 層

プローブサイトの RenderProbe.aspx で、1 ケース 1 リクエスト（ページライフサイクル全体）で描画したページ全体を比較します。下表はコンポーネント部分（div#probe）、設定時の例外、保存された ViewState の要約です。

| ケース | 設定 | 期待される描画 | 設定時の例外 |
|---|---|---|---|
| <code>.(default)</code> | <code>(nothing set)</code> | <code></code> |  |
| <code>.AccessKey=text</code> | <code>c.AccessKey = &quot;probe&quot;;</code> | <code></code> | <code>c.AccessKey = &quot;probe&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=markup</code> | <code>c.AccessKey = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> | <code>c.AccessKey = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=japanese</code> | <code>c.AccessKey = &quot;日本語テキスト&quot;;</code> | <code></code> | <code>c.AccessKey = &quot;日本語テキスト&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=empty</code> | <code>c.AccessKey = &quot;&quot;;</code> | <code></code> |  |
| <code>.AccessKey=k</code> | <code>c.AccessKey = &quot;k&quot;;</code> | <code></code> |  |
| <code>.AppRelativeTemplateSourceDirectory=text</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;;</code> | <code></code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;probe&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=markup</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=japanese</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;;</code> | <code></code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;日本語テキスト&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=empty</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&quot;;</code> | <code></code> |  |
| <code>.BackColor=Red</code> | <code>c.BackColor = Color.Red;</code> | <code></code> |  |
| <code>.BackColor=#3366CC</code> | <code>c.BackColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code></code> |  |
| <code>.BackColor=Transparent</code> | <code>c.BackColor = Color.Transparent;</code> | <code></code> |  |
| <code>.BackImageUrl=text</code> | <code>c.BackImageUrl = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.BackImageUrl=markup</code> | <code>c.BackImageUrl = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.BackImageUrl=japanese</code> | <code>c.BackImageUrl = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.BackImageUrl=empty</code> | <code>c.BackImageUrl = &quot;&quot;;</code> | <code></code> |  |
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
| <code>.EditUIStyle.BackColor=Red</code> | <code>c.EditUIStyle.BackColor = Color.Red;</code> | <code></code> |  |
| <code>.EditUIStyle.CssClass=text</code> | <code>c.EditUIStyle.CssClass = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.EditUIStyle.Font.Bold=true</code> | <code>c.EditUIStyle.Font.Bold = true;</code> | <code></code> |  |
| <code>.EditUIStyle.ForeColor=Red</code> | <code>c.EditUIStyle.ForeColor = Color.Red;</code> | <code></code> |  |
| <code>.EditUIStyle.Height=120px</code> | <code>c.EditUIStyle.Height = Unit.Pixel(120);</code> | <code></code> |  |
| <code>.EmptyZoneText=text</code> | <code>c.EmptyZoneText = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.EmptyZoneText=markup</code> | <code>c.EmptyZoneText = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.EmptyZoneText=japanese</code> | <code>c.EmptyZoneText = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.EmptyZoneText=empty</code> | <code>c.EmptyZoneText = &quot;&quot;;</code> | <code></code> |  |
| <code>.EmptyZoneTextStyle.BackColor=Red</code> | <code>c.EmptyZoneTextStyle.BackColor = Color.Red;</code> | <code></code> |  |
| <code>.EmptyZoneTextStyle.CssClass=text</code> | <code>c.EmptyZoneTextStyle.CssClass = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.EmptyZoneTextStyle.Font.Bold=true</code> | <code>c.EmptyZoneTextStyle.Font.Bold = true;</code> | <code></code> |  |
| <code>.EmptyZoneTextStyle.ForeColor=Red</code> | <code>c.EmptyZoneTextStyle.ForeColor = Color.Red;</code> | <code></code> |  |
| <code>.EmptyZoneTextStyle.Height=120px</code> | <code>c.EmptyZoneTextStyle.Height = Unit.Pixel(120);</code> | <code></code> |  |
| <code>.EnableTheming=true</code> | <code>c.EnableTheming = true;</code> | <code></code> |  |
| <code>.EnableTheming=false</code> | <code>c.EnableTheming = false;</code> | <code></code> |  |
| <code>.EnableViewState=true</code> | <code>c.EnableViewState = true;</code> | <code></code> |  |
| <code>.EnableViewState=false</code> | <code>c.EnableViewState = false;</code> | <code></code> |  |
| <code>.Enabled=true</code> | <code>c.Enabled = true;</code> | <code></code> |  |
| <code>.Enabled=false</code> | <code>c.Enabled = false;</code> | <code></code> |  |
| <code>.ErrorStyle.BackColor=Red</code> | <code>c.ErrorStyle.BackColor = Color.Red;</code> | <code></code> |  |
| <code>.ErrorStyle.CssClass=text</code> | <code>c.ErrorStyle.CssClass = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.ErrorStyle.Font.Bold=true</code> | <code>c.ErrorStyle.Font.Bold = true;</code> | <code></code> |  |
| <code>.ErrorStyle.ForeColor=Red</code> | <code>c.ErrorStyle.ForeColor = Color.Red;</code> | <code></code> |  |
| <code>.ErrorStyle.Height=120px</code> | <code>c.ErrorStyle.Height = Unit.Pixel(120);</code> | <code></code> |  |
| <code>.ErrorText=text</code> | <code>c.ErrorText = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.ErrorText=markup</code> | <code>c.ErrorText = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.ErrorText=japanese</code> | <code>c.ErrorText = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.ErrorText=empty</code> | <code>c.ErrorText = &quot;&quot;;</code> | <code></code> |  |
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
| <code>.FooterStyle.BackColor=Red</code> | <code>c.FooterStyle.BackColor = Color.Red;</code> | <code></code> |  |
| <code>.FooterStyle.CssClass=text</code> | <code>c.FooterStyle.CssClass = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.FooterStyle.Font.Bold=true</code> | <code>c.FooterStyle.Font.Bold = true;</code> | <code></code> |  |
| <code>.FooterStyle.ForeColor=Red</code> | <code>c.FooterStyle.ForeColor = Color.Red;</code> | <code></code> |  |
| <code>.FooterStyle.Height=120px</code> | <code>c.FooterStyle.Height = Unit.Pixel(120);</code> | <code></code> |  |
| <code>.FooterStyle.HorizontalAlign=NotSet</code> | <code>c.FooterStyle.HorizontalAlign = HorizontalAlign.NotSet;</code> | <code></code> |  |
| <code>.FooterStyle.VerticalAlign=NotSet</code> | <code>c.FooterStyle.VerticalAlign = VerticalAlign.NotSet;</code> | <code></code> |  |
| <code>.FooterStyle.Wrap=true</code> | <code>c.FooterStyle.Wrap = true;</code> | <code></code> |  |
| <code>.ForeColor=Red</code> | <code>c.ForeColor = Color.Red;</code> | <code></code> |  |
| <code>.ForeColor=#3366CC</code> | <code>c.ForeColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code></code> |  |
| <code>.ForeColor=Transparent</code> | <code>c.ForeColor = Color.Transparent;</code> | <code></code> |  |
| <code>.HeaderStyle.BackColor=Red</code> | <code>c.HeaderStyle.BackColor = Color.Red;</code> | <code></code> |  |
| <code>.HeaderStyle.CssClass=text</code> | <code>c.HeaderStyle.CssClass = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.HeaderStyle.Font.Bold=true</code> | <code>c.HeaderStyle.Font.Bold = true;</code> | <code></code> |  |
| <code>.HeaderStyle.ForeColor=Red</code> | <code>c.HeaderStyle.ForeColor = Color.Red;</code> | <code></code> |  |
| <code>.HeaderStyle.Height=120px</code> | <code>c.HeaderStyle.Height = Unit.Pixel(120);</code> | <code></code> |  |
| <code>.HeaderStyle.HorizontalAlign=NotSet</code> | <code>c.HeaderStyle.HorizontalAlign = HorizontalAlign.NotSet;</code> | <code></code> |  |
| <code>.HeaderStyle.VerticalAlign=NotSet</code> | <code>c.HeaderStyle.VerticalAlign = VerticalAlign.NotSet;</code> | <code></code> |  |
| <code>.HeaderStyle.Wrap=true</code> | <code>c.HeaderStyle.Wrap = true;</code> | <code></code> |  |
| <code>.HeaderText=text</code> | <code>c.HeaderText = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.HeaderText=markup</code> | <code>c.HeaderText = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.HeaderText=japanese</code> | <code>c.HeaderText = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.HeaderText=empty</code> | <code>c.HeaderText = &quot;&quot;;</code> | <code></code> |  |
| <code>.HeaderVerbStyle.BackColor=Red</code> | <code>c.HeaderVerbStyle.BackColor = Color.Red;</code> | <code></code> |  |
| <code>.HeaderVerbStyle.CssClass=text</code> | <code>c.HeaderVerbStyle.CssClass = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.HeaderVerbStyle.Font.Bold=true</code> | <code>c.HeaderVerbStyle.Font.Bold = true;</code> | <code></code> |  |
| <code>.HeaderVerbStyle.ForeColor=Red</code> | <code>c.HeaderVerbStyle.ForeColor = Color.Red;</code> | <code></code> |  |
| <code>.HeaderVerbStyle.Height=120px</code> | <code>c.HeaderVerbStyle.Height = Unit.Pixel(120);</code> | <code></code> |  |
| <code>.Height=120px</code> | <code>c.Height = Unit.Pixel(120);</code> | <code></code> |  |
| <code>.Height=50%</code> | <code>c.Height = Unit.Percentage(50);</code> | <code></code> |  |
| <code>.Height=2em</code> | <code>c.Height = new Unit(2, UnitType.Em);</code> | <code></code> |  |
| <code>.ID=text</code> | <code>c.ID = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.ID=markup</code> | <code>c.ID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.ID=japanese</code> | <code>c.ID = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.ID=empty</code> | <code>c.ID = &quot;&quot;;</code> | <code></code> |  |
| <code>.InstructionText=text</code> | <code>c.InstructionText = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.InstructionText=markup</code> | <code>c.InstructionText = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.InstructionText=japanese</code> | <code>c.InstructionText = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.InstructionText=empty</code> | <code>c.InstructionText = &quot;&quot;;</code> | <code></code> |  |
| <code>.InstructionTextStyle.BackColor=Red</code> | <code>c.InstructionTextStyle.BackColor = Color.Red;</code> | <code></code> |  |
| <code>.InstructionTextStyle.CssClass=text</code> | <code>c.InstructionTextStyle.CssClass = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.InstructionTextStyle.Font.Bold=true</code> | <code>c.InstructionTextStyle.Font.Bold = true;</code> | <code></code> |  |
| <code>.InstructionTextStyle.ForeColor=Red</code> | <code>c.InstructionTextStyle.ForeColor = Color.Red;</code> | <code></code> |  |
| <code>.InstructionTextStyle.Height=120px</code> | <code>c.InstructionTextStyle.Height = Unit.Pixel(120);</code> | <code></code> |  |
| <code>.LabelStyle.BackColor=Red</code> | <code>c.LabelStyle.BackColor = Color.Red;</code> | <code></code> |  |
| <code>.LabelStyle.CssClass=text</code> | <code>c.LabelStyle.CssClass = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.LabelStyle.Font.Bold=true</code> | <code>c.LabelStyle.Font.Bold = true;</code> | <code></code> |  |
| <code>.LabelStyle.ForeColor=Red</code> | <code>c.LabelStyle.ForeColor = Color.Red;</code> | <code></code> |  |
| <code>.LabelStyle.Height=120px</code> | <code>c.LabelStyle.Height = Unit.Pixel(120);</code> | <code></code> |  |
| <code>.Padding=0</code> | <code>c.Padding = 0;</code> | <code></code> |  |
| <code>.Padding=3</code> | <code>c.Padding = 3;</code> | <code></code> |  |
| <code>.Padding=-1</code> | <code>c.Padding = -1;</code> | <code></code> |  |
| <code>.PartChromePadding=120px</code> | <code>c.PartChromePadding = Unit.Pixel(120);</code> | <code></code> |  |
| <code>.PartChromePadding=50%</code> | <code>c.PartChromePadding = Unit.Percentage(50);</code> | <code></code> |  |
| <code>.PartChromePadding=2em</code> | <code>c.PartChromePadding = new Unit(2, UnitType.Em);</code> | <code></code> |  |
| <code>.PartChromeStyle.BackColor=Red</code> | <code>c.PartChromeStyle.BackColor = Color.Red;</code> | <code></code> |  |
| <code>.PartChromeStyle.CssClass=text</code> | <code>c.PartChromeStyle.CssClass = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.PartChromeStyle.Font.Bold=true</code> | <code>c.PartChromeStyle.Font.Bold = true;</code> | <code></code> |  |
| <code>.PartChromeStyle.ForeColor=Red</code> | <code>c.PartChromeStyle.ForeColor = Color.Red;</code> | <code></code> |  |
| <code>.PartChromeStyle.Height=120px</code> | <code>c.PartChromeStyle.Height = Unit.Pixel(120);</code> | <code></code> |  |
| <code>.PartChromeType=Default</code> | <code>c.PartChromeType = PartChromeType.Default;</code> | <code></code> |  |
| <code>.PartChromeType=TitleAndBorder</code> | <code>c.PartChromeType = PartChromeType.TitleAndBorder;</code> | <code></code> |  |
| <code>.PartChromeType=None</code> | <code>c.PartChromeType = PartChromeType.None;</code> | <code></code> |  |
| <code>.PartChromeType=TitleOnly</code> | <code>c.PartChromeType = PartChromeType.TitleOnly;</code> | <code></code> |  |
| <code>.PartChromeType=BorderOnly</code> | <code>c.PartChromeType = PartChromeType.BorderOnly;</code> | <code></code> |  |
| <code>.PartStyle.BackColor=Red</code> | <code>c.PartStyle.BackColor = Color.Red;</code> | <code></code> |  |
| <code>.PartStyle.CssClass=text</code> | <code>c.PartStyle.CssClass = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.PartStyle.Font.Bold=true</code> | <code>c.PartStyle.Font.Bold = true;</code> | <code></code> |  |
| <code>.PartStyle.ForeColor=Red</code> | <code>c.PartStyle.ForeColor = Color.Red;</code> | <code></code> |  |
| <code>.PartStyle.Height=120px</code> | <code>c.PartStyle.Height = Unit.Pixel(120);</code> | <code></code> |  |
| <code>.PartStyle.HorizontalAlign=NotSet</code> | <code>c.PartStyle.HorizontalAlign = HorizontalAlign.NotSet;</code> | <code></code> |  |
| <code>.PartTitleStyle.BackColor=Red</code> | <code>c.PartTitleStyle.BackColor = Color.Red;</code> | <code></code> |  |
| <code>.PartTitleStyle.CssClass=text</code> | <code>c.PartTitleStyle.CssClass = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.PartTitleStyle.Font.Bold=true</code> | <code>c.PartTitleStyle.Font.Bold = true;</code> | <code></code> |  |
| <code>.PartTitleStyle.ForeColor=Red</code> | <code>c.PartTitleStyle.ForeColor = Color.Red;</code> | <code></code> |  |
| <code>.PartTitleStyle.Height=120px</code> | <code>c.PartTitleStyle.Height = Unit.Pixel(120);</code> | <code></code> |  |
| <code>.PartTitleStyle.HorizontalAlign=NotSet</code> | <code>c.PartTitleStyle.HorizontalAlign = HorizontalAlign.NotSet;</code> | <code></code> |  |
| <code>.PartTitleStyle.VerticalAlign=NotSet</code> | <code>c.PartTitleStyle.VerticalAlign = VerticalAlign.NotSet;</code> | <code></code> |  |
| <code>.PartTitleStyle.Wrap=true</code> | <code>c.PartTitleStyle.Wrap = true;</code> | <code></code> |  |
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
| <code>.VerbButtonType=Button</code> | <code>c.VerbButtonType = ButtonType.Button;</code> | <code></code> |  |
| <code>.VerbButtonType=Image</code> | <code>c.VerbButtonType = ButtonType.Image;</code> | <code></code> |  |
| <code>.VerbButtonType=Link</code> | <code>c.VerbButtonType = ButtonType.Link;</code> | <code></code> |  |
| <code>.VerbStyle.BackColor=Red</code> | <code>c.VerbStyle.BackColor = Color.Red;</code> | <code></code> |  |
| <code>.VerbStyle.CssClass=text</code> | <code>c.VerbStyle.CssClass = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.VerbStyle.Font.Bold=true</code> | <code>c.VerbStyle.Font.Bold = true;</code> | <code></code> |  |
| <code>.VerbStyle.ForeColor=Red</code> | <code>c.VerbStyle.ForeColor = Color.Red;</code> | <code></code> |  |
| <code>.VerbStyle.Height=120px</code> | <code>c.VerbStyle.Height = Unit.Pixel(120);</code> | <code></code> |  |
| <code>.ViewStateMode=Inherit</code> | <code>c.ViewStateMode = ViewStateMode.Inherit;</code> | <code></code> |  |
| <code>.ViewStateMode=Enabled</code> | <code>c.ViewStateMode = ViewStateMode.Enabled;</code> | <code></code> |  |
| <code>.ViewStateMode=Disabled</code> | <code>c.ViewStateMode = ViewStateMode.Disabled;</code> | <code></code> |  |
| <code>.Visible=true</code> | <code>c.Visible = true;</code> | <code></code> | <code>c.Visible = true; -&gt; System.InvalidOperationException: Cannot set the Visible property of a Tool Zone.  Override the ToolZone.Display property instead.</code> |
| <code>.Visible=false</code> | <code>c.Visible = false;</code> | <code></code> | <code>c.Visible = false; -&gt; System.InvalidOperationException: Cannot set the Visible property of a Tool Zone.  Override the ToolZone.Display property instead.</code> |
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
| <code>.HeaderText=markup+InstructionText=markup</code> | <code>c.HeaderText = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; c.InstructionText = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.ApplyVerb.Text=text+OKVerb.Text=markup</code> | <code>c.ApplyVerb.Text = &quot;probe&quot;; c.OKVerb.Text = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.Visible=false+EnableViewState=false</code> | <code>c.Visible = false; c.EnableViewState = false;</code> | <code></code> | <code>c.Visible = false; -&gt; System.InvalidOperationException: Cannot set the Visible property of a Tool Zone.  Override the ToolZone.Display property instead.</code> |
| <code>.Enabled=false+CssClass=text</code> | <code>c.Enabled = false; c.CssClass = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | <code>c.Width = Unit.Pixel(120); c.Height = new Unit(2, UnitType.Em); c.BorderWidth = Unit.Pixel(120); c.BorderStyle = BorderStyle.Solid; c.BackColor = Color.FromArgb(0x33, 0x66, 0xCC); c.ForeColor = Color.Red; c.Font.Bold = true;</code> | <code></code> |  |
| <code>.ForeColor=#3366CC+Style[color]=red</code> | <code>c.ForeColor = Color.FromArgb(0x33, 0x66, 0xCC); c.Style[color] = &quot;red&quot;;</code> | <code></code> |  |
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

## ③ シナリオ層

ブラウザ（Playwright）で EditorZoneScenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。

| # | 操作 | 対象 | 値 |
|---|---|---|---|
| 1 | goto | /WebPartsScenario.aspx?reset=1 | <code></code> |
| 2 | snapshot |  | **EditorZone [render][databind][dynamic] first request (browse): the parts in their zones, a zone made in code** |
| 3 | click | btnDesign | <code></code> |
| 4 | snapshot |  | **EditorZone [viewstate] the design mode** |
| 5 | click | btnCatalog | <code></code> |
| 6 | snapshot |  | **EditorZone [viewstate] the catalog mode: the catalog zone and its parts** |
| 7 | evaluate | czCatalog | <code>e.querySelector(&#39;input[type=checkbox]&#39;).checked = true;</code> |
| 8 | clicktextin | czCatalog | <code>Add</code> |
| 9 | snapshot |  | **EditorZone [viewstate] a part added from the catalog (its check box posted, the zone&#39;s Add)** |
| 10 | click | btnEditPart | <code></code> |
| 11 | snapshot |  | **EditorZone [viewstate][templates] the edit mode: the editor zone and its parts for the first part** |
| 12 | evaluate | ezEditor | <code>e.querySelector(&#39;input[type=text]&#39;).value = &#39;Renamed part&#39;;</code> |
| 13 | clicktextin | ezEditor | <code>OK</code> |
| 14 | snapshot |  | **EditorZone [postbackevent] the first part&#39;s title edited (the editor zone&#39;s OK)** |
| 15 | click | btnConnectPart | <code></code> |
| 16 | snapshot |  | **EditorZone [viewstate] the connect mode for the first part** |
| 17 | clicktextin | cnzConnections | <code>Close</code> |
| 18 | snapshot |  | **EditorZone [viewstate] the connections zone closed** |
| 19 | click | btnBrowse | <code></code> |
| 20 | clicktextin | wpzMain | <code>Minimize</code> |
| 21 | snapshot |  | **EditorZone [viewstate] a part minimized by its zone&#39;s verb** |
| 22 | clicktextin | wpzMain | <code>Restore</code> |
| 23 | snapshot |  | **EditorZone [viewstate] restored** |
| 24 | clicktextin | wpzDynamic | <code>Minimize</code> |
| 25 | snapshot |  | **EditorZone [viewstate] a part of the zone made in code minimized** |
| 26 | click | btnShow | <code></code> |
| 27 | snapshot |  | **EditorZone [visible][viewstate] a hidden zone shown** |

<details><summary>EditorZone [render][databind][dynamic] first request (browse): the parts in their zones, a zone made in code（期待される本文）</summary>

```text
Display mode: Browse
Label part Minimize Close
bound
Text part Minimize Close
Dynamic:
Untitled Minimize Close
A dynamic part
Request 1: IsPostBack=False User=taro
State DisplayMode=Browse Scope=User IsModifiable=True
State parts: Label part (wpzMain), Text part (wpzMain), (wpzDynamic)
State supported modes: Browse, Design, Catalog, Edit, Connect
```

</details>

<details><summary>EditorZone [viewstate] the design mode（期待される本文）</summary>

```text
Display mode: Design
Main zone
Label part Minimize Close
bound
Text part Minimize Close
Side zone
The side zone is empty.
Dynamic:
Dynamic zone
Untitled Minimize Close
A dynamic part
Request 2: IsPostBack=True User=taro
DisplayModeChanged Browse -> Design
State DisplayMode=Design Scope=User IsModifiable=True
State parts: Label part (wpzMain), Text part (wpzMain), (wpzDynamic)
State supported modes: Browse, Design, Catalog, Edit, Connect
```

</details>

<details><summary>EditorZone [viewstate] the catalog mode: the catalog zone and its parts（期待される本文）</summary>

```text
Display mode: Catalog
Main zone
Label part Minimize Close
bound
Text part Minimize Close
Side zone
The side zone is empty.
Catalog Close
Select the catalog you would like to browse.
Declared parts (1)
Page Catalog (0)
Imported Web Part Catalog (0)
Declared parts
Catalog label
Add to:
Main zone
Side zone
Hidden zone
Dynamic zone
Dynamic:
Dynamic zone
Untitled Minimize Close
A dynamic part
Request 3: IsPostBack=True User=taro
DisplayModeChanged Design -> Catalog
State DisplayMode=Catalog Scope=User IsModifiable=True
State parts: Label part (wpzMain), Text part (wpzMain), (wpzDynamic)
State supported modes: Browse, Design, Catalog, Edit, Connect
```

</details>

<details><summary>EditorZone [viewstate] a part added from the catalog (its check box posted, the zone&#39;s Add)（期待される本文）</summary>

```text
Display mode: Catalog
Main zone
Catalog label Minimize Close Delete
Label part Minimize Close
bound
Text part Minimize Close
Side zone
The side zone is empty.
Catalog Close
Select the catalog you would like to browse.
Declared parts (1)
Page Catalog (0)
Imported Web Part Catalog (0)
Declared parts
Catalog label
Add to:
Main zone
Side zone
Hidden zone
Dynamic zone
Dynamic:
Dynamic zone
Untitled Minimize Close
A dynamic part
Request 4: IsPostBack=True User=taro
WebPartAdded Catalog label in wpzMain
State DisplayMode=Catalog Scope=User IsModifiable=True
State parts: Label part (wpzMain), Text part (wpzMain), (wpzDynamic), Catalog label (wpzMain)
State supported modes: Browse, Design, Catalog, Edit, Connect
```

</details>

<details><summary>EditorZone [viewstate][templates] the edit mode: the editor zone and its parts for the first part（期待される本文）</summary>

```text
Display mode: Edit
Main zone
Catalog label Minimize Close Delete Edit
Label part Minimize Close
bound
Text part Minimize Close Edit
Side zone
The side zone is empty.
Editor Close
Modify the properties of the Web Part, then click OK or Apply to apply your changes.
Appearance
Title:
Chrome Type:
Default
Title and Border
Title Only
Border Only
None
Direction:
Not Set
Left to Right
Right to Left
Height:
pixels
points
picas
inches
millimeters
centimeters
percent
em
ex
Width:
pixels
points
picas
inches
millimeters
centimeters
percent
em
ex
Hidden
Layout
Chrome State:
Normal
Minimized
Zone:
Main zone
Side zone
Hidden zone
Dynamic zone
Zone Index:
Dynamic:
Dynamic zone
Untitled Minimize Close Edit
A dynamic part
Request 5: IsPostBack=True User=taro
DisplayModeChanged Catalog -> Edit
State DisplayMode=Edit Scope=User IsModifiable=True
State parts: Label part (wpzMain), Text part (wpzMain), (wpzDynamic), Catalog label (wpzMain)
State supported modes: Browse, Design, Catalog, Edit, Connect
```

</details>

<details><summary>EditorZone [postbackevent] the first part&#39;s title edited (the editor zone&#39;s OK)（期待される本文）</summary>

```text
Display mode: Edit
Main zone
Catalog label Minimize Close Delete Edit
Renamed part Minimize Close Edit
bound
Text part Minimize Close Edit
Side zone
The side zone is empty.
Dynamic:
Dynamic zone
Untitled Minimize Close Edit
A dynamic part
Request 6: IsPostBack=True User=taro
State DisplayMode=Edit Scope=User IsModifiable=True
State parts: Renamed part (wpzMain), Text part (wpzMain), (wpzDynamic), Catalog label (wpzMain)
State supported modes: Browse, Design, Catalog, Edit, Connect
```

</details>

<details><summary>EditorZone [viewstate] the connect mode for the first part（期待される本文）</summary>

```text
Display mode: Connect
Main zone
Catalog label Minimize Close Delete
Renamed part Minimize Close
bound
Text part Minimize Close
Side zone
The side zone is empty.
Connections Close
No active connections
There are no active connections available in your Web Part. You may create a new connection by selecting the links above if there are compatible Web Parts on the page.
Dynamic:
Dynamic zone
Untitled Minimize Close
A dynamic part
Request 7: IsPostBack=True User=taro
DisplayModeChanged Edit -> Connect
State DisplayMode=Connect Scope=User IsModifiable=True
State parts: Renamed part (wpzMain), Text part (wpzMain), (wpzDynamic), Catalog label (wpzMain)
State supported modes: Browse, Design, Catalog, Edit, Connect
```

</details>

<details><summary>EditorZone [viewstate] the connections zone closed（期待される本文）</summary>

```text
Display mode: Connect
Main zone
Catalog label Minimize Close Delete
Renamed part Minimize Close
bound
Text part Minimize Close
Side zone
The side zone is empty.
Dynamic:
Dynamic zone
Untitled Minimize Close
A dynamic part
Request 8: IsPostBack=True User=taro
State DisplayMode=Connect Scope=User IsModifiable=True
State parts: Renamed part (wpzMain), Text part (wpzMain), (wpzDynamic), Catalog label (wpzMain)
State supported modes: Browse, Design, Catalog, Edit, Connect
```

</details>

<details><summary>EditorZone [viewstate] a part minimized by its zone&#39;s verb（期待される本文）</summary>

```text
Display mode: Browse
Catalog label Restore Close
Renamed part Minimize Close
bound
Text part Minimize Close
Dynamic:
Untitled Minimize Close
A dynamic part
Request 10: IsPostBack=True User=taro
State DisplayMode=Browse Scope=User IsModifiable=True
State parts: Renamed part (wpzMain), Text part (wpzMain), (wpzDynamic), Catalog label (wpzMain)
State supported modes: Browse, Design, Catalog, Edit, Connect
```

</details>

<details><summary>EditorZone [viewstate] restored（期待される本文）</summary>

```text
Display mode: Browse
Catalog label Minimize Close
Renamed part Minimize Close
bound
Text part Minimize Close
Dynamic:
Untitled Minimize Close
A dynamic part
Request 11: IsPostBack=True User=taro
State DisplayMode=Browse Scope=User IsModifiable=True
State parts: Renamed part (wpzMain), Text part (wpzMain), (wpzDynamic), Catalog label (wpzMain)
State supported modes: Browse, Design, Catalog, Edit, Connect
```

</details>

<details><summary>EditorZone [viewstate] a part of the zone made in code minimized（期待される本文）</summary>

```text
Display mode: Browse
Catalog label Minimize Close
Renamed part Minimize Close
bound
Text part Minimize Close
Dynamic:
Untitled Restore Close
Request 12: IsPostBack=True User=taro
State DisplayMode=Browse Scope=User IsModifiable=True
State parts: Renamed part (wpzMain), Text part (wpzMain), (wpzDynamic), Catalog label (wpzMain)
State supported modes: Browse, Design, Catalog, Edit, Connect
```

</details>

<details><summary>EditorZone [visible][viewstate] a hidden zone shown（期待される本文）</summary>

```text
Display mode: Browse
Catalog label Minimize Close
Renamed part Minimize Close
bound
Text part Minimize Close
Dynamic:
Untitled Restore Close
Request 13: IsPostBack=True User=taro
Click btnShow
State DisplayMode=Browse Scope=User IsModifiable=True
State parts: Renamed part (wpzMain), Text part (wpzMain), (wpzDynamic), Catalog label (wpzMain)
State supported modes: Browse, Design, Catalog, Edit, Connect
```

</details>

