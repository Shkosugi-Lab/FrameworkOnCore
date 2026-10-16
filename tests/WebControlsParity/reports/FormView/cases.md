# FormView テストケース一覧

変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。
このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。

- 正解データの採取: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0
- ① API 層: 309 ケース（319 メンバー）
- ② HTML 層: 320 ケース
- ③ シナリオ層: 15 スナップショット

## 機構マトリクス

型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。

| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |
|---|---|---|---|
| プロパティ → HTML 描画 | すべてのコントロール | 301 | 1 |
| ViewState の保存と復元 | Control | 6 | 2 |
| Visible=false（描画されず状態は保持） | Control.Visible | 3 | 1 |
| 名前付けコンテナと ClientIDMode | Control.ClientIDMode | 7 | 0 |
| テーマとスキン（SkinID / EnableTheming） | テーマを付けられる型（[Themeable]） | 4 | 0 |
| 動的に生成したコントロールのライフサイクル | Control | 0 | 2 |
| データバインド（DataBind / <%# %>） | Control.DataBind | 0 | 1 |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | WebControl.Enabled、HtmlControl.Disabled | 2 | 0 |
| ポストバックイベント | IPostBackEventHandler | 0 | 1 |
| ページング | AllowPaging プロパティ、IPageableItemContainer（ListView）、DataPager | 16 | 3 |
| 項目の挿入 | InsertItemTemplate・AutoGenerateInsertButton プロパティ | 15 | 3 |
| 行・項目の編集（編集・更新・取り消し・削除） | EditIndex・EditItemIndex・DefaultMode プロパティ | 8 | 5 |
| 行・項目のコマンド（RowCommand・ItemCommand へのバブル） | *Command イベント（Command 以外） | 0 | 1 |
| データキー（DataKeyNames・DataKeys） | DataKeyNames プロパティ | 3 | 2 |
| データソースコントロールとの連携（DataSourceID） | データ表示コントロール（Repeater・CompositeDataBoundControl・BaseDataList・ListView） | 0 | 5 |
| テンプレート | ITemplate のプロパティ | 0 | 1 |
| データソース連携（DataSource・DataTextField など） | BaseDataBoundControl、HtmlSelect | 3 | 1 |

該当しない機構: ポストバックデータの取り込み、変更イベント（*Changed、ServerChange）、ReadOnly（ポストされた値を無視）、リクエスト検証（ValidateRequestMode）、コマンドイベントと親へのバブル（CommandName）、送信ボタン／スクリプトによるポストバック（UseSubmitBehavior）、ブラウザ側のクリック処理（OnClientClick）、クロスページポストバック（PostBackUrl）、既定のボタン（Panel の DefaultButton）、AutoPostBack（クライアントスクリプトによるポストバック）、CausesValidation / ValidationGroup、検証対象（サーバー側の検証）、検証対象（ブラウザ側の検証）、双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値）、項目（ListItem の Selected・Enabled・Text・Value と ViewState）、複数選択、繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）、ポスト値のイベント検証（一覧にない値・無効な項目の拒否）、サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）、ブラウザ側の検証スクリプト（EnableClientScript）、検証結果のまとめ（ValidationSummary）、並べ替え、行・項目の選択、クライアントコールバック

## ① API 層

FormView とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の FormView インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。

| ケース | 期待値（.NET Framework） |
|---|---|
| <code>Api.Control.#ctor</code> | <code>Control.#ctor = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMode=In…</code> |
| <code>Api.Control.Adapter</code> | <code>Control.Adapter = null</code> |
| <code>Api.Control.AddParsedSubObject</code> | <code>Control.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>Control.AddParsedSubObject(Object) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderCol…</code> |
| <code>Api.Control.AddedControl</code> | <code>Control.AddedControl(Control,Int32) = &quot;ok&quot;</code><br><code>Control.AddedControl(Control,Int32) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderCo…</code> |
| <code>Api.Control.AppRelativeTemplateSourceDirectory</code> | <code>Control.AppRelativeTemplateSourceDirectory = &quot;&quot;</code><br><code>Control.AppRelativeTemplateSourceDirectory set = &quot;-&gt; System.ArgumentException: The relative virtual path &#39;Abc 123&#39; is not allowed here.&quot;</code> |
| <code>Api.Control.ApplyStyleSheetSkin</code> | <code>Control.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>Control.ApplyStyleSheetSkin(Page) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColo…</code> |
| <code>Api.Control.BeginRenderTracing</code> | <code>Control.BeginRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.BeginRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.BeginRenderTracing(TextWriter,Object) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;…</code> |
| <code>Api.Control.BindingContainer</code> | <code>Control.BindingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Clien…</code> |
| <code>Api.Control.BuildProfileTree</code> | <code>Control.BuildProfileTree(String,Boolean) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.ChildControlsCreated</code> | <code>Control.ChildControlsCreated = &quot;false&quot;</code><br><code>Control.ChildControlsCreated set = &quot;ok&quot;</code> |
| <code>Api.Control.ClearCachedClientID</code> | <code>Control.ClearCachedClientID = &quot;ok&quot;</code><br><code>Control.ClearCachedClientID after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Colo…</code> |
| <code>Api.Control.ClearChildControlState</code> | <code>Control.ClearChildControlState = &quot;ok&quot;</code><br><code>Control.ClearChildControlState after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=C…</code> |
| <code>Api.Control.ClearChildState</code> | <code>Control.ClearChildState = &quot;ok&quot;</code><br><code>Control.ClearChildState after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #0…</code> |
| <code>Api.Control.ClearChildViewState</code> | <code>Control.ClearChildViewState = &quot;ok&quot;</code><br><code>Control.ClearChildViewState after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Colo…</code> |
| <code>Api.Control.ClearEffectiveClientIDMode</code> | <code>Control.ClearEffectiveClientIDMode = &quot;ok&quot;</code><br><code>Control.ClearEffectiveClientIDMode after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderCol…</code> |
| <code>Api.Control.ClientID</code> | <code>Control.ClientID = &quot;fvItem&quot;</code> |
| <code>Api.Control.ClientIDMode</code> | <code>Control.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>Control.ClientIDMode set = &quot;ClientIDMode.AutoID&quot;</code> |
| <code>Api.Control.ClientIDSeparator</code> | <code>Control.ClientIDSeparator = &quot;&#39;_&#39;&quot;</code> |
| <code>Api.Control.Context</code> | <code>Control.Context = null</code> |
| <code>Api.Control.Controls</code> | <code>Control.Controls = ControlCollection []</code> |
| <code>Api.Control.CreateChildControls</code> | <code>Control.CreateChildControls = &quot;ok&quot;</code><br><code>Control.CreateChildControls after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Colo…</code> |
| <code>Api.Control.CreateControlCollection</code> | <code>Control.CreateControlCollection = ControlCollection []</code><br><code>Control.CreateControlCollection after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=…</code> |
| <code>Api.Control.DataBind</code> | <code>Control.DataBind = &quot;ok&quot;</code><br><code>Control.DataBind after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000…</code><br><code>Control.DataBind(Boolean) = &quot;ok&quot;</code><br><code>Control.DataBind(Boolean) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color …</code> |
| <code>Api.Control.DataBindChildren</code> | <code>Control.DataBindChildren = &quot;ok&quot;</code><br><code>Control.DataBindChildren after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #…</code> |
| <code>Api.Control.DataBinding</code> | <code>Control.DataBinding = &quot;added and removed&quot;</code> |
| <code>Api.Control.DataItemContainer</code> | <code>Control.DataItemContainer = null</code> |
| <code>Api.Control.DataKeysContainer</code> | <code>Control.DataKeysContainer = null</code> |
| <code>Api.Control.DesignMode</code> | <code>Control.DesignMode = &quot;false&quot;</code> |
| <code>Api.Control.Dispose</code> | <code>Control.Dispose = &quot;ok&quot;</code><br><code>Control.Dispose after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 …</code> |
| <code>Api.Control.Disposed</code> | <code>Control.Disposed = &quot;added and removed&quot;</code> |
| <code>Api.Control.EnableTheming</code> | <code>Control.EnableTheming = &quot;true&quot;</code><br><code>Control.EnableTheming set = &quot;true&quot;</code> |
| <code>Api.Control.EnableViewState</code> | <code>Control.EnableViewState = &quot;true&quot;</code><br><code>Control.EnableViewState set = &quot;true&quot;</code> |
| <code>Api.Control.EndRenderTracing</code> | <code>Control.EndRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.EndRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.EndRenderTracing(TextWriter,Object) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; …</code> |
| <code>Api.Control.EnsureChildControls</code> | <code>Control.EnsureChildControls = &quot;ok&quot;</code><br><code>Control.EnsureChildControls after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Colo…</code> |
| <code>Api.Control.EnsureID</code> | <code>Control.EnsureID = &quot;ok&quot;</code><br><code>Control.EnsureID after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000…</code> |
| <code>Api.Control.Events</code> | <code>Control.Events = &quot;System.ComponentModel.EventHandlerList&quot;</code> |
| <code>Api.Control.FindControl</code> | <code>Control.FindControl(String) = null</code><br><code>Control.FindControl(String) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Colo…</code><br><code>Control.FindControl(String,Int32) = null</code><br><code>Control.FindControl(String,Int32) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColo…</code> |
| <code>Api.Control.Focus</code> | <code>Control.Focus = &quot;-&gt; System.InvalidOperationException: A form tag with runat=server must exist on the Page to use SetFocus() or the Focus property.&quot;</code> |
| <code>Api.Control.GetDesignModeState</code> | <code>Control.GetDesignModeState = &quot;[]&quot;</code><br><code>Control.GetDesignModeState after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color…</code> |
| <code>Api.Control.GetRouteUrl</code> | <code>Control.GetRouteUrl(Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.GetUniqueIDRelativeTo</code> | <code>Control.GetUniqueIDRelativeTo(Control) = &quot;fvItem&quot;</code><br><code>Control.GetUniqueIDRelativeTo(Control) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; Borde…</code> |
| <code>Api.Control.HasChildViewState</code> | <code>Control.HasChildViewState = &quot;false&quot;</code> |
| <code>Api.Control.HasControls</code> | <code>Control.HasControls = &quot;false&quot;</code><br><code>Control.HasControls after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000…</code> |
| <code>Api.Control.HasEvents</code> | <code>Control.HasEvents = &quot;false&quot;</code><br><code>Control.HasEvents after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #0000000…</code> |
| <code>Api.Control.ID</code> | <code>Control.ID = &quot;fvItem&quot;</code><br><code>Control.ID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.IdSeparator</code> | <code>Control.IdSeparator = &quot;&#39;$&#39;&quot;</code> |
| <code>Api.Control.Init</code> | <code>Control.Init = &quot;added and removed&quot;</code> |
| <code>Api.Control.IsChildControlStateCleared</code> | <code>Control.IsChildControlStateCleared = &quot;false&quot;</code> |
| <code>Api.Control.IsLiteralContent</code> | <code>Control.IsLiteralContent = &quot;false&quot;</code><br><code>Control.IsLiteralContent after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #…</code> |
| <code>Api.Control.IsTrackingViewState</code> | <code>Control.IsTrackingViewState = &quot;false&quot;</code> |
| <code>Api.Control.IsViewStateEnabled</code> | <code>Control.IsViewStateEnabled = &quot;true&quot;</code> |
| <code>Api.Control.Load</code> | <code>Control.Load = &quot;added and removed&quot;</code> |
| <code>Api.Control.LoadControlState</code> | <code>Control.LoadControlState(Object) = &quot;ok&quot;</code><br><code>Control.LoadControlState(Object) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor…</code> |
| <code>Api.Control.LoadViewState</code> | <code>Control.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Object[]&#39;.&quot;</code> |
| <code>Api.Control.LoadViewStateByID</code> | <code>Control.LoadViewStateByID = &quot;false&quot;</code> |
| <code>Api.Control.MapPathSecure</code> | <code>Control.MapPathSecure(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.NamingContainer</code> | <code>Control.NamingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code> |
| <code>Api.Control.OnBubbleEvent</code> | <code>Control.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Control.OnBubbleEvent(Object,EventArgs) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; Bord…</code> |
| <code>Api.Control.OnDataBinding</code> | <code>Control.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnDataBinding(EventArgs) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor…</code> |
| <code>Api.Control.OnInit</code> | <code>Control.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnInit(EventArgs) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color …</code> |
| <code>Api.Control.OnLoad</code> | <code>Control.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnLoad(EventArgs) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color …</code> |
| <code>Api.Control.OnPreRender</code> | <code>Control.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnPreRender(EventArgs) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=C…</code> |
| <code>Api.Control.OnUnload</code> | <code>Control.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnUnload(EventArgs) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Colo…</code> |
| <code>Api.Control.OpenFile</code> | <code>Control.OpenFile(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.Page</code> | <code>Control.Page = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=&quot;&quot;; …</code><br><code>Control.Page set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=…</code> |
| <code>Api.Control.Parent</code> | <code>Control.Parent = HtmlForm#form1 {Action=&quot;&quot;; ClientID=&quot;form1&quot;; ClientIDMode=Inherit; DefaultButton=&quot;&quot;; DefaultFocus=&quot;&quot;; Disabled=false; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.PreRender</code> | <code>Control.PreRender = &quot;added and removed&quot;</code> |
| <code>Api.Control.RaiseBubbleEvent</code> | <code>Control.RaiseBubbleEvent(Object,EventArgs) = &quot;ok&quot;</code><br><code>Control.RaiseBubbleEvent(Object,EventArgs) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; B…</code> |
| <code>Api.Control.RemovedControl</code> | <code>Control.RemovedControl(Control) = &quot;ok&quot;</code><br><code>Control.RemovedControl(Control) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=…</code> |
| <code>Api.Control.Render</code> | <code>Control.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.Render(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.Render(HtmlTextWriter) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=C…</code> |
| <code>Api.Control.RenderChildren</code> | <code>Control.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderChildren(HtmlTextWriter) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; Borde…</code> |
| <code>Api.Control.RenderControl</code> | <code>Control.RenderControl(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderControl(HtmlTextWriter) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; Border…</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty];…</code> |
| <code>Api.Control.RenderingCompatibility</code> | <code>Control.RenderingCompatibility = &quot;System.Version 4.0&quot;</code><br><code>Control.RenderingCompatibility set = &quot;System.Version 4.0&quot;</code> |
| <code>Api.Control.ResolveAdapter</code> | <code>Control.ResolveAdapter = null</code><br><code>Control.ResolveAdapter after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00…</code> |
| <code>Api.Control.ResolveClientUrl</code> | <code>Control.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveClientUrl(String) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor…</code> |
| <code>Api.Control.ResolveUrl</code> | <code>Control.ResolveUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveUrl(String) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color…</code> |
| <code>Api.Control.SaveControlState</code> | <code>Control.SaveControlState = &quot;true&quot;</code><br><code>Control.SaveControlState after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #…</code> |
| <code>Api.Control.SaveViewState</code> | <code>Control.SaveViewState = &quot;[null, null, null, null, null, null, null, null, null, null]&quot;</code><br><code>Control.SaveViewState after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #000…</code> |
| <code>Api.Control.SetDesignModeState</code> | <code>Control.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>Control.SetDesignModeState(IDictionary) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; Bord…</code> |
| <code>Api.Control.SetRenderMethodDelegate</code> | <code>Control.SetRenderMethodDelegate(RenderMethod) = &quot;ok&quot;</code><br><code>Control.SetRenderMethodDelegate(RenderMethod) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;…</code> |
| <code>Api.Control.SetTraceData</code> | <code>Control.SetTraceData(Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderCo…</code><br><code>Control.SetTraceData(Object,Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object,Object) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; B…</code> |
| <code>Api.Control.Site</code> | <code>Control.Site = null</code><br><code>Control.Site set = null</code> |
| <code>Api.Control.SkinID</code> | <code>Control.SkinID = &quot;&quot;</code><br><code>Control.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.TemplateControl</code> | <code>Control.TemplateControl = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code><br><code>Control.TemplateControl set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Cl…</code> |
| <code>Api.Control.TemplateSourceDirectory</code> | <code>Control.TemplateSourceDirectory = &quot;&quot;</code> |
| <code>Api.Control.TrackViewState</code> | <code>Control.TrackViewState = &quot;ok&quot;</code><br><code>Control.TrackViewState after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00…</code> |
| <code>Api.Control.UniqueID</code> | <code>Control.UniqueID = &quot;fvItem&quot;</code> |
| <code>Api.Control.Unload</code> | <code>Control.Unload = &quot;added and removed&quot;</code> |
| <code>Api.Control.ValidateRequestMode</code> | <code>Control.ValidateRequestMode = &quot;ValidateRequestMode.Inherit&quot;</code><br><code>Control.ValidateRequestMode set = &quot;ValidateRequestMode.Disabled&quot;</code> |
| <code>Api.Control.ViewState</code> | <code>Control.ViewState = StateBag []</code> |
| <code>Api.Control.ViewStateIgnoresCase</code> | <code>Control.ViewStateIgnoresCase = &quot;false&quot;</code> |
| <code>Api.Control.ViewStateMode</code> | <code>Control.ViewStateMode = &quot;ViewStateMode.Inherit&quot;</code><br><code>Control.ViewStateMode set = &quot;ViewStateMode.Enabled&quot;</code> |
| <code>Api.Control.Visible</code> | <code>Control.Visible = &quot;true&quot;</code><br><code>Control.Visible set = &quot;true&quot;</code> |
| <code>Api.IAttributeAccessor.GetAttribute</code> | <code>IAttributeAccessor.GetAttribute(String) = null</code><br><code>IAttributeAccessor.GetAttribute(String) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; Bord…</code> |
| <code>Api.IAttributeAccessor.SetAttribute</code> | <code>IAttributeAccessor.SetAttribute(String,String) = &quot;ok&quot;</code><br><code>IAttributeAccessor.SetAttribute(String,String) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;…</code> |
| <code>Api.IControlBuilderAccessor.ControlBuilder</code> | <code>IControlBuilderAccessor.ControlBuilder = null</code> |
| <code>Api.IControlDesignerAccessor.GetDesignModeState</code> | <code>IControlDesignerAccessor.GetDesignModeState = &quot;[]&quot;</code><br><code>IControlDesignerAccessor.GetDesignModeState after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; …</code> |
| <code>Api.IControlDesignerAccessor.SetDesignModeState</code> | <code>IControlDesignerAccessor.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetDesignModeState(IDictionary) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; Back…</code> |
| <code>Api.IControlDesignerAccessor.SetOwnerControl</code> | <code>IControlDesignerAccessor.SetOwnerControl(Control) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetOwnerControl(Control) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUr…</code> |
| <code>Api.IControlDesignerAccessor.UserData</code> | <code>IControlDesignerAccessor.UserData = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.DataBindings</code> | <code>IDataBindingsAccessor.DataBindings = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.HasDataBindings</code> | <code>IDataBindingsAccessor.HasDataBindings = &quot;false&quot;</code> |
| <code>Api.IDataItemContainer.DataItem</code> | <code>IDataItemContainer.DataItem = null</code> |
| <code>Api.IDataItemContainer.DataItemIndex</code> | <code>IDataItemContainer.DataItemIndex = &quot;Int32 0&quot;</code> |
| <code>Api.IDataItemContainer.DisplayIndex</code> | <code>IDataItemContainer.DisplayIndex = &quot;Int32 0&quot;</code> |
| <code>Api.IExpressionsAccessor.Expressions</code> | <code>IExpressionsAccessor.Expressions = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.HasExpressions</code> | <code>IExpressionsAccessor.HasExpressions = &quot;false&quot;</code> |
| <code>Api.IParserAccessor.AddParsedSubObject</code> | <code>IParserAccessor.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>IParserAccessor.AddParsedSubObject(Object) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; B…</code> |
| <code>Api.IPostBackEventHandler.RaisePostBackEvent</code> | <code>IPostBackEventHandler.RaisePostBackEvent(String) = &quot;ok&quot;</code><br><code>IPostBackEventHandler.RaisePostBackEvent(String) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl…</code> |
| <code>Api.IUrlResolutionService.ResolveClientUrl</code> | <code>IUrlResolutionService.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>IUrlResolutionService.ResolveClientUrl(String) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;…</code> |
| <code>Api.WebControls.BaseDataBoundControl.#ctor</code> | <code>BaseDataBoundControl.#ctor = &quot;-&gt; System.MemberAccessException: Cannot create an instance of System.Web.UI.WebControls.BaseDataBoundControl because it is an abst…</code> |
| <code>Api.WebControls.BaseDataBoundControl.ConfirmInitState</code> | <code>BaseDataBoundControl.ConfirmInitState = &quot;ok&quot;</code><br><code>BaseDataBoundControl.ConfirmInitState after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; Border…</code> |
| <code>Api.WebControls.BaseDataBoundControl.DataBind</code> | <code>BaseDataBoundControl.DataBind = &quot;ok&quot;</code><br><code>BaseDataBoundControl.DataBind after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Co…</code> |
| <code>Api.WebControls.BaseDataBoundControl.DataBound</code> | <code>BaseDataBoundControl.DataBound = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.BaseDataBoundControl.DataSource</code> | <code>BaseDataBoundControl.DataSource = &quot;[System.String Pen, System.String Bag]&quot;</code><br><code>BaseDataBoundControl.DataSource set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.BaseDataBoundControl.DataSourceID</code> | <code>BaseDataBoundControl.DataSourceID = &quot;&quot;</code><br><code>BaseDataBoundControl.DataSourceID set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.BaseDataBoundControl.EnsureDataBound</code> | <code>BaseDataBoundControl.EnsureDataBound = &quot;ok&quot;</code><br><code>BaseDataBoundControl.EnsureDataBound after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderC…</code> |
| <code>Api.WebControls.BaseDataBoundControl.Initialized</code> | <code>BaseDataBoundControl.Initialized = &quot;false&quot;</code> |
| <code>Api.WebControls.BaseDataBoundControl.IsBoundUsingDataSourceID</code> | <code>BaseDataBoundControl.IsBoundUsingDataSourceID = &quot;false&quot;</code> |
| <code>Api.WebControls.BaseDataBoundControl.IsDataBindingAutomatic</code> | <code>BaseDataBoundControl.IsDataBindingAutomatic = &quot;false&quot;</code> |
| <code>Api.WebControls.BaseDataBoundControl.IsUsingModelBinders</code> | <code>BaseDataBoundControl.IsUsingModelBinders = &quot;false&quot;</code> |
| <code>Api.WebControls.BaseDataBoundControl.OnDataBound</code> | <code>BaseDataBoundControl.OnDataBound(EventArgs) = &quot;ok&quot;</code><br><code>BaseDataBoundControl.OnDataBound(EventArgs) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; …</code> |
| <code>Api.WebControls.BaseDataBoundControl.OnDataPropertyChanged</code> | <code>BaseDataBoundControl.OnDataPropertyChanged = &quot;ok&quot;</code><br><code>BaseDataBoundControl.OnDataPropertyChanged after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; B…</code> |
| <code>Api.WebControls.BaseDataBoundControl.OnInit</code> | <code>BaseDataBoundControl.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>BaseDataBoundControl.OnInit(EventArgs) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; Borde…</code> |
| <code>Api.WebControls.BaseDataBoundControl.OnPagePreLoad</code> | <code>BaseDataBoundControl.OnPagePreLoad(Object,EventArgs) = &quot;ok&quot;</code><br><code>BaseDataBoundControl.OnPagePreLoad(Object,EventArgs) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImag…</code> |
| <code>Api.WebControls.BaseDataBoundControl.OnPreRender</code> | <code>BaseDataBoundControl.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>BaseDataBoundControl.OnPreRender(EventArgs) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; …</code> |
| <code>Api.WebControls.BaseDataBoundControl.PerformSelect</code> | <code>BaseDataBoundControl.PerformSelect = &quot;ok&quot;</code><br><code>BaseDataBoundControl.PerformSelect after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderCol…</code> |
| <code>Api.WebControls.BaseDataBoundControl.RequiresDataBinding</code> | <code>BaseDataBoundControl.RequiresDataBinding = &quot;false&quot;</code><br><code>BaseDataBoundControl.RequiresDataBinding set = &quot;ok&quot;</code> |
| <code>Api.WebControls.BaseDataBoundControl.SupportsDisabledAttribute</code> | <code>BaseDataBoundControl.SupportsDisabledAttribute = &quot;false&quot;</code> |
| <code>Api.WebControls.BaseDataBoundControl.ValidateDataSource</code> | <code>BaseDataBoundControl.ValidateDataSource(Object) = &quot;ok&quot;</code><br><code>BaseDataBoundControl.ValidateDataSource(Object) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=…</code> |
| <code>Api.WebControls.CompositeDataBoundControl.#ctor</code> | <code>CompositeDataBoundControl.#ctor = &quot;-&gt; System.MemberAccessException: Cannot create an instance of System.Web.UI.WebControls.CompositeDataBoundControl because it …</code> |
| <code>Api.WebControls.CompositeDataBoundControl.Controls</code> | <code>CompositeDataBoundControl.Controls = ControlCollection []</code> |
| <code>Api.WebControls.CompositeDataBoundControl.CreateChildControls</code> | <code>CompositeDataBoundControl.CreateChildControls = &quot;ok&quot;</code><br><code>CompositeDataBoundControl.CreateChildControls after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;…</code><br><code>CompositeDataBoundControl.CreateChildControls(IEnumerable,Boolean) = &quot;Int32 2&quot;</code><br><code>CompositeDataBoundControl.CreateChildControls(IEnumerable,Boolean) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Em…</code> |
| <code>Api.WebControls.CompositeDataBoundControl.DeleteMethod</code> | <code>CompositeDataBoundControl.DeleteMethod = &quot;&quot;</code><br><code>CompositeDataBoundControl.DeleteMethod set = &quot;ok&quot;</code> |
| <code>Api.WebControls.CompositeDataBoundControl.InsertMethod</code> | <code>CompositeDataBoundControl.InsertMethod = &quot;&quot;</code><br><code>CompositeDataBoundControl.InsertMethod set = &quot;ok&quot;</code> |
| <code>Api.WebControls.CompositeDataBoundControl.IsUsingModelBinders</code> | <code>CompositeDataBoundControl.IsUsingModelBinders = &quot;false&quot;</code> |
| <code>Api.WebControls.CompositeDataBoundControl.PerformDataBinding</code> | <code>CompositeDataBoundControl.PerformDataBinding(IEnumerable) = &quot;ok&quot;</code><br><code>CompositeDataBoundControl.PerformDataBinding(IEnumerable) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; Bac…</code> |
| <code>Api.WebControls.CompositeDataBoundControl.UpdateMethod</code> | <code>CompositeDataBoundControl.UpdateMethod = &quot;&quot;</code><br><code>CompositeDataBoundControl.UpdateMethod set = &quot;ok&quot;</code> |
| <code>Api.WebControls.DataBoundControl.#ctor</code> | <code>DataBoundControl.#ctor = &quot;-&gt; System.MemberAccessException: Cannot create an instance of System.Web.UI.WebControls.DataBoundControl because it is an abstract cla…</code> |
| <code>Api.WebControls.DataBoundControl.CallingDataMethods</code> | <code>DataBoundControl.CallingDataMethods = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.DataBoundControl.CreateDataSourceSelectArguments</code> | <code>DataBoundControl.CreateDataSourceSelectArguments = DataSourceSelectArguments {MaximumRows=Int32 0; RetrieveTotalRowCount=false; SortExpression=&quot;&quot;; StartRowIndex…</code><br><code>DataBoundControl.CreateDataSourceSelectArguments after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl…</code> |
| <code>Api.WebControls.DataBoundControl.CreatingModelDataSource</code> | <code>DataBoundControl.CreatingModelDataSource = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.DataBoundControl.DataMember</code> | <code>DataBoundControl.DataMember = &quot;&quot;</code><br><code>DataBoundControl.DataMember set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.DataBoundControl.DataSourceID</code> | <code>DataBoundControl.DataSourceID = &quot;&quot;</code><br><code>DataBoundControl.DataSourceID set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.DataBoundControl.DataSourceObject</code> | <code>DataBoundControl.DataSourceObject = null</code> |
| <code>Api.WebControls.DataBoundControl.GetData</code> | <code>DataBoundControl.GetData = ReadOnlyDataSourceView {CanDelete=false; CanInsert=false; CanPage=false; CanRetrieveTotalRowCount=false; CanSort=false; CanUpdate=fal…</code><br><code>DataBoundControl.GetData after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #…</code> |
| <code>Api.WebControls.DataBoundControl.GetDataSource</code> | <code>DataBoundControl.GetDataSource = null</code><br><code>DataBoundControl.GetDataSource after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=C…</code> |
| <code>Api.WebControls.DataBoundControl.IsUsingModelBinders</code> | <code>DataBoundControl.IsUsingModelBinders = &quot;false&quot;</code> |
| <code>Api.WebControls.DataBoundControl.ItemType</code> | <code>DataBoundControl.ItemType = &quot;&quot;</code><br><code>DataBoundControl.ItemType set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.DataBoundControl.LoadViewState</code> | <code>DataBoundControl.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Object[]&#39;.&quot;</code> |
| <code>Api.WebControls.DataBoundControl.MarkAsDataBound</code> | <code>DataBoundControl.MarkAsDataBound = &quot;ok&quot;</code><br><code>DataBoundControl.MarkAsDataBound after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor…</code> |
| <code>Api.WebControls.DataBoundControl.OnCreatingModelDataSource</code> | <code>DataBoundControl.OnCreatingModelDataSource(CreatingModelDataSourceEventArgs) = &quot;ok&quot;</code><br><code>DataBoundControl.OnCreatingModelDataSource(CreatingModelDataSourceEventArgs) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000…</code> |
| <code>Api.WebControls.DataBoundControl.OnDataPropertyChanged</code> | <code>DataBoundControl.OnDataPropertyChanged = &quot;ok&quot;</code><br><code>DataBoundControl.OnDataPropertyChanged after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; Borde…</code> |
| <code>Api.WebControls.DataBoundControl.OnDataSourceViewChanged</code> | <code>DataBoundControl.OnDataSourceViewChanged(Object,EventArgs) = &quot;ok&quot;</code><br><code>DataBoundControl.OnDataSourceViewChanged(Object,EventArgs) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; Ba…</code> |
| <code>Api.WebControls.DataBoundControl.OnLoad</code> | <code>DataBoundControl.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>DataBoundControl.OnLoad(EventArgs) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderCol…</code> |
| <code>Api.WebControls.DataBoundControl.OnPagePreLoad</code> | <code>DataBoundControl.OnPagePreLoad(Object,EventArgs) = &quot;ok&quot;</code><br><code>DataBoundControl.OnPagePreLoad(Object,EventArgs) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl…</code> |
| <code>Api.WebControls.DataBoundControl.PerformDataBinding</code> | <code>DataBoundControl.PerformDataBinding(IEnumerable) = &quot;ok&quot;</code><br><code>DataBoundControl.PerformDataBinding(IEnumerable) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl…</code> |
| <code>Api.WebControls.DataBoundControl.PerformSelect</code> | <code>DataBoundControl.PerformSelect = &quot;ok&quot;</code><br><code>DataBoundControl.PerformSelect after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=C…</code> |
| <code>Api.WebControls.DataBoundControl.SaveViewState</code> | <code>DataBoundControl.SaveViewState = &quot;[null, null, null, null, null, null, null, null, null, null]&quot;</code><br><code>DataBoundControl.SaveViewState after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=C…</code> |
| <code>Api.WebControls.DataBoundControl.SelectArguments</code> | <code>DataBoundControl.SelectArguments = DataSourceSelectArguments {MaximumRows=Int32 0; RetrieveTotalRowCount=false; SortExpression=&quot;&quot;; StartRowIndex=Int32 0; TotalR…</code> |
| <code>Api.WebControls.DataBoundControl.SelectMethod</code> | <code>DataBoundControl.SelectMethod = &quot;&quot;</code><br><code>DataBoundControl.SelectMethod set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.DataBoundControl.TrackViewState</code> | <code>DataBoundControl.TrackViewState = &quot;ok&quot;</code><br><code>DataBoundControl.TrackViewState after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=…</code> |
| <code>Api.WebControls.DataBoundControl.ValidateDataSource</code> | <code>DataBoundControl.ValidateDataSource(Object) = &quot;ok&quot;</code><br><code>DataBoundControl.ValidateDataSource(Object) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; …</code> |
| <code>Api.WebControls.FormView.#ctor</code> | <code>FormView.#ctor = FormView# {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Color [Empty]…</code> |
| <code>Api.WebControls.FormView.AllowPaging</code> | <code>FormView.AllowPaging = &quot;false&quot;</code><br><code>FormView.AllowPaging set = &quot;true&quot;</code> |
| <code>Api.WebControls.FormView.BackImageUrl</code> | <code>FormView.BackImageUrl = &quot;&quot;</code><br><code>FormView.BackImageUrl set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.FormView.BottomPagerRow</code> | <code>FormView.BottomPagerRow = null</code> |
| <code>Api.WebControls.FormView.Caption</code> | <code>FormView.Caption = &quot;&quot;</code><br><code>FormView.Caption set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.FormView.CaptionAlign</code> | <code>FormView.CaptionAlign = &quot;TableCaptionAlign.NotSet&quot;</code><br><code>FormView.CaptionAlign set = &quot;TableCaptionAlign.Top&quot;</code> |
| <code>Api.WebControls.FormView.CellPadding</code> | <code>FormView.CellPadding = &quot;Int32 -1&quot;</code><br><code>FormView.CellPadding set = &quot;Int32 1&quot;</code> |
| <code>Api.WebControls.FormView.CellSpacing</code> | <code>FormView.CellSpacing = &quot;Int32 0&quot;</code><br><code>FormView.CellSpacing set = &quot;Int32 1&quot;</code> |
| <code>Api.WebControls.FormView.ChangeMode</code> | <code>FormView.ChangeMode(FormViewMode) = &quot;ok&quot;</code><br><code>FormView.ChangeMode(FormViewMode) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColo…</code> |
| <code>Api.WebControls.FormView.CreateChildControls</code> | <code>FormView.CreateChildControls(IEnumerable,Boolean) = &quot;Int32 2&quot;</code><br><code>FormView.CreateChildControls(IEnumerable,Boolean) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUr…</code> |
| <code>Api.WebControls.FormView.CreateControlStyle</code> | <code>FormView.CreateControlStyle = TableStyle {BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSe…</code><br><code>FormView.CreateControlStyle after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Colo…</code> |
| <code>Api.WebControls.FormView.CreateDataSourceSelectArguments</code> | <code>FormView.CreateDataSourceSelectArguments = DataSourceSelectArguments {MaximumRows=Int32 0; RetrieveTotalRowCount=false; SortExpression=&quot;&quot;; StartRowIndex=Int32 0…</code><br><code>FormView.CreateDataSourceSelectArguments after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; Bor…</code> |
| <code>Api.WebControls.FormView.CreateRow</code> | <code>FormView.CreateRow(Int32,DataControlRowType,DataControlRowState) = FormViewRow# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000…</code><br><code>FormView.CreateRow(Int32,DataControlRowType,DataControlRowState) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empt…</code> |
| <code>Api.WebControls.FormView.CreateTable</code> | <code>FormView.CreateTable = ChildTable# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Color [Empty]; BorderSty…</code><br><code>FormView.CreateTable after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #0000…</code> |
| <code>Api.WebControls.FormView.CurrentMode</code> | <code>FormView.CurrentMode = &quot;FormViewMode.ReadOnly&quot;</code> |
| <code>Api.WebControls.FormView.DataBind</code> | <code>FormView.DataBind = &quot;ok&quot;</code><br><code>FormView.DataBind after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #0000000…</code> |
| <code>Api.WebControls.FormView.DataItem</code> | <code>FormView.DataItem = null</code> |
| <code>Api.WebControls.FormView.DataItemCount</code> | <code>FormView.DataItemCount = &quot;Int32 0&quot;</code> |
| <code>Api.WebControls.FormView.DataItemIndex</code> | <code>FormView.DataItemIndex = &quot;Int32 0&quot;</code> |
| <code>Api.WebControls.FormView.DataKey</code> | <code>FormView.DataKey = DataKey {}</code> |
| <code>Api.WebControls.FormView.DataKeyNames</code> | <code>FormView.DataKeyNames = &quot;[]&quot;</code><br><code>FormView.DataKeyNames set = &quot;[System.String a, System.String b]&quot;</code> |
| <code>Api.WebControls.FormView.DefaultMode</code> | <code>FormView.DefaultMode = &quot;FormViewMode.ReadOnly&quot;</code><br><code>FormView.DefaultMode set = &quot;FormViewMode.Edit&quot;</code> |
| <code>Api.WebControls.FormView.DeleteItem</code> | <code>FormView.DeleteItem = &quot;-&gt; System.Web.HttpException: The FormView &#39;fvItem&#39; fired event ItemDeleting which wasn&#39;t handled.&quot;</code> |
| <code>Api.WebControls.FormView.DeleteMethod</code> | <code>FormView.DeleteMethod = &quot;&quot;</code><br><code>FormView.DeleteMethod set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.FormView.EditItemTemplate</code> | <code>FormView.EditItemTemplate = null</code><br><code>FormView.EditItemTemplate set = &quot;FrameworkOnCore.WebControlsParity.Component+SampleTemplate&quot;</code> |
| <code>Api.WebControls.FormView.EditRowStyle</code> | <code>FormView.EditRowStyle = TableItemStyle {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; Cs…</code> |
| <code>Api.WebControls.FormView.EmptyDataRowStyle</code> | <code>FormView.EmptyDataRowStyle = TableItemStyle {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth…</code> |
| <code>Api.WebControls.FormView.EmptyDataTemplate</code> | <code>FormView.EmptyDataTemplate = null</code><br><code>FormView.EmptyDataTemplate set = &quot;FrameworkOnCore.WebControlsParity.Component+SampleTemplate&quot;</code> |
| <code>Api.WebControls.FormView.EmptyDataText</code> | <code>FormView.EmptyDataText = &quot;&quot;</code><br><code>FormView.EmptyDataText set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.FormView.EnableModelValidation</code> | <code>FormView.EnableModelValidation = &quot;true&quot;</code><br><code>FormView.EnableModelValidation set = &quot;true&quot;</code> |
| <code>Api.WebControls.FormView.EnsureDataBound</code> | <code>FormView.EnsureDataBound = &quot;ok&quot;</code><br><code>FormView.EnsureDataBound after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #…</code> |
| <code>Api.WebControls.FormView.ExtractRowValues</code> | <code>FormView.ExtractRowValues(IOrderedDictionary,Boolean) = &quot;ok&quot;</code><br><code>FormView.ExtractRowValues(IOrderedDictionary,Boolean) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackIma…</code> |
| <code>Api.WebControls.FormView.FooterRow</code> | <code>FormView.FooterRow = null</code> |
| <code>Api.WebControls.FormView.FooterStyle</code> | <code>FormView.FooterStyle = TableItemStyle {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; Css…</code> |
| <code>Api.WebControls.FormView.FooterTemplate</code> | <code>FormView.FooterTemplate = null</code><br><code>FormView.FooterTemplate set = &quot;FrameworkOnCore.WebControlsParity.Component+SampleTemplate&quot;</code> |
| <code>Api.WebControls.FormView.FooterText</code> | <code>FormView.FooterText = &quot;&quot;</code><br><code>FormView.FooterText set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.FormView.GridLines</code> | <code>FormView.GridLines = &quot;GridLines.None&quot;</code><br><code>FormView.GridLines set = &quot;GridLines.Horizontal&quot;</code> |
| <code>Api.WebControls.FormView.HeaderRow</code> | <code>FormView.HeaderRow = null</code> |
| <code>Api.WebControls.FormView.HeaderStyle</code> | <code>FormView.HeaderStyle = TableItemStyle {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; Css…</code> |
| <code>Api.WebControls.FormView.HeaderTemplate</code> | <code>FormView.HeaderTemplate = null</code><br><code>FormView.HeaderTemplate set = &quot;FrameworkOnCore.WebControlsParity.Component+SampleTemplate&quot;</code> |
| <code>Api.WebControls.FormView.HeaderText</code> | <code>FormView.HeaderText = &quot;&quot;</code><br><code>FormView.HeaderText set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.FormView.HorizontalAlign</code> | <code>FormView.HorizontalAlign = &quot;HorizontalAlign.NotSet&quot;</code><br><code>FormView.HorizontalAlign set = &quot;HorizontalAlign.Left&quot;</code> |
| <code>Api.WebControls.FormView.InitializePager</code> | <code>FormView.InitializePager(FormViewRow,PagedDataSource) = &quot;ok&quot;</code><br><code>FormView.InitializePager(FormViewRow,PagedDataSource) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackIma…</code> |
| <code>Api.WebControls.FormView.InitializeRow</code> | <code>FormView.InitializeRow(FormViewRow) = &quot;ok&quot;</code><br><code>FormView.InitializeRow(FormViewRow) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderCo…</code> |
| <code>Api.WebControls.FormView.InsertItem</code> | <code>FormView.InsertItem(Boolean) = &quot;-&gt; System.Web.HttpException: FormView &#39;fvItem&#39; must be in insert mode to insert a new record.&quot;</code> |
| <code>Api.WebControls.FormView.InsertItemTemplate</code> | <code>FormView.InsertItemTemplate = null</code><br><code>FormView.InsertItemTemplate set = &quot;FrameworkOnCore.WebControlsParity.Component+SampleTemplate&quot;</code> |
| <code>Api.WebControls.FormView.InsertMethod</code> | <code>FormView.InsertMethod = &quot;&quot;</code><br><code>FormView.InsertMethod set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.FormView.InsertRowStyle</code> | <code>FormView.InsertRowStyle = TableItemStyle {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; …</code> |
| <code>Api.WebControls.FormView.IsBindableType</code> | <code>FormView.IsBindableType(Type) = &quot;false&quot;</code><br><code>FormView.IsBindableType(Type) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Co…</code> |
| <code>Api.WebControls.FormView.ItemCommand</code> | <code>FormView.ItemCommand = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.FormView.ItemCreated</code> | <code>FormView.ItemCreated = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.FormView.ItemDeleted</code> | <code>FormView.ItemDeleted = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.FormView.ItemDeleting</code> | <code>FormView.ItemDeleting = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.FormView.ItemInserted</code> | <code>FormView.ItemInserted = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.FormView.ItemInserting</code> | <code>FormView.ItemInserting = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.FormView.ItemTemplate</code> | <code>FormView.ItemTemplate = &quot;FrameworkOnCore.WebControlsParity.Component+SampleTemplate&quot;</code><br><code>FormView.ItemTemplate set = &quot;FrameworkOnCore.WebControlsParity.Component+SampleTemplate&quot;</code> |
| <code>Api.WebControls.FormView.ItemUpdated</code> | <code>FormView.ItemUpdated = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.FormView.ItemUpdating</code> | <code>FormView.ItemUpdating = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.FormView.LoadControlState</code> | <code>FormView.LoadControlState(Object) = &quot;ok&quot;</code><br><code>FormView.LoadControlState(Object) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColo…</code> |
| <code>Api.WebControls.FormView.LoadViewState</code> | <code>FormView.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Object[]&#39;.&quot;</code> |
| <code>Api.WebControls.FormView.ModeChanged</code> | <code>FormView.ModeChanged = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.FormView.ModeChanging</code> | <code>FormView.ModeChanging = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.FormView.ModifiedOuterTableStylePropertyName</code> | <code>FormView.ModifiedOuterTableStylePropertyName = &quot;&quot;</code><br><code>FormView.ModifiedOuterTableStylePropertyName after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;;…</code> |
| <code>Api.WebControls.FormView.OnBubbleEvent</code> | <code>FormView.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>FormView.OnBubbleEvent(Object,EventArgs) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; Bor…</code> |
| <code>Api.WebControls.FormView.OnInit</code> | <code>FormView.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>FormView.OnInit(EventArgs) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color…</code> |
| <code>Api.WebControls.FormView.OnItemCommand</code> | <code>FormView.OnItemCommand(FormViewCommandEventArgs) = &quot;ok&quot;</code><br><code>FormView.OnItemCommand(FormViewCommandEventArgs) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl…</code> |
| <code>Api.WebControls.FormView.OnItemCreated</code> | <code>FormView.OnItemCreated(EventArgs) = &quot;ok&quot;</code><br><code>FormView.OnItemCreated(EventArgs) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColo…</code> |
| <code>Api.WebControls.FormView.OnItemDeleted</code> | <code>FormView.OnItemDeleted(FormViewDeletedEventArgs) = &quot;ok&quot;</code><br><code>FormView.OnItemDeleted(FormViewDeletedEventArgs) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl…</code> |
| <code>Api.WebControls.FormView.OnItemDeleting</code> | <code>FormView.OnItemDeleting(FormViewDeleteEventArgs) = &quot;-&gt; System.Web.HttpException: The FormView &#39;fvItem&#39; fired event ItemDeleting which wasn&#39;t handled.&quot;</code> |
| <code>Api.WebControls.FormView.OnItemInserted</code> | <code>FormView.OnItemInserted(FormViewInsertedEventArgs) = &quot;ok&quot;</code><br><code>FormView.OnItemInserted(FormViewInsertedEventArgs) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageU…</code> |
| <code>Api.WebControls.FormView.OnItemInserting</code> | <code>FormView.OnItemInserting(FormViewInsertEventArgs) = &quot;-&gt; System.Web.HttpException: The FormView &#39;fvItem&#39; fired event ItemInserting which wasn&#39;t handled.&quot;</code> |
| <code>Api.WebControls.FormView.OnItemUpdated</code> | <code>FormView.OnItemUpdated(FormViewUpdatedEventArgs) = &quot;ok&quot;</code><br><code>FormView.OnItemUpdated(FormViewUpdatedEventArgs) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl…</code> |
| <code>Api.WebControls.FormView.OnItemUpdating</code> | <code>FormView.OnItemUpdating(FormViewUpdateEventArgs) = &quot;-&gt; System.Web.HttpException: The FormView &#39;fvItem&#39; fired event ItemUpdating which wasn&#39;t handled.&quot;</code> |
| <code>Api.WebControls.FormView.OnModeChanged</code> | <code>FormView.OnModeChanged(EventArgs) = &quot;ok&quot;</code><br><code>FormView.OnModeChanged(EventArgs) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColo…</code> |
| <code>Api.WebControls.FormView.OnModeChanging</code> | <code>FormView.OnModeChanging(FormViewModeEventArgs) = &quot;-&gt; System.Web.HttpException: The FormView &#39;fvItem&#39; fired event ModeChanging which wasn&#39;t handled.&quot;</code> |
| <code>Api.WebControls.FormView.OnPageIndexChanged</code> | <code>FormView.OnPageIndexChanged(EventArgs) = &quot;ok&quot;</code><br><code>FormView.OnPageIndexChanged(EventArgs) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; Borde…</code> |
| <code>Api.WebControls.FormView.OnPageIndexChanging</code> | <code>FormView.OnPageIndexChanging(FormViewPageEventArgs) = &quot;-&gt; System.Web.HttpException: The FormView &#39;fvItem&#39; fired event PageIndexChanging which wasn&#39;t handled.&quot;</code> |
| <code>Api.WebControls.FormView.PageCount</code> | <code>FormView.PageCount = &quot;Int32 0&quot;</code> |
| <code>Api.WebControls.FormView.PageIndex</code> | <code>FormView.PageIndex = &quot;Int32 0&quot;</code><br><code>FormView.PageIndex set = &quot;Int32 1&quot;</code> |
| <code>Api.WebControls.FormView.PageIndexChanged</code> | <code>FormView.PageIndexChanged = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.FormView.PageIndexChanging</code> | <code>FormView.PageIndexChanging = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.FormView.PagerSettings</code> | <code>FormView.PagerSettings = PagerSettings {FirstPageImageUrl=&quot;&quot;; FirstPageText=&quot;&amp;lt;&amp;lt;&quot;; LastPageImageUrl=&quot;&quot;; LastPageText=&quot;&amp;gt;&amp;gt;&quot;; Mode=Numeric; NextPageImag…</code> |
| <code>Api.WebControls.FormView.PagerStyle</code> | <code>FormView.PagerStyle = TableItemStyle {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; CssC…</code> |
| <code>Api.WebControls.FormView.PagerTemplate</code> | <code>FormView.PagerTemplate = null</code><br><code>FormView.PagerTemplate set = &quot;FrameworkOnCore.WebControlsParity.Component+SampleTemplate&quot;</code> |
| <code>Api.WebControls.FormView.PerformDataBinding</code> | <code>FormView.PerformDataBinding(IEnumerable) = &quot;ok&quot;</code><br><code>FormView.PerformDataBinding(IEnumerable) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; Bor…</code> |
| <code>Api.WebControls.FormView.PrepareControlHierarchy</code> | <code>FormView.PrepareControlHierarchy = &quot;ok&quot;</code><br><code>FormView.PrepareControlHierarchy after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor…</code> |
| <code>Api.WebControls.FormView.RaisePostBackEvent</code> | <code>FormView.RaisePostBackEvent(String) = &quot;ok&quot;</code><br><code>FormView.RaisePostBackEvent(String) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderCo…</code> |
| <code>Api.WebControls.FormView.Render</code> | <code>FormView.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>FormView.Render(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>FormView.Render(HtmlTextWriter) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=…</code> |
| <code>Api.WebControls.FormView.RenderOuterTable</code> | <code>FormView.RenderOuterTable = &quot;true&quot;</code><br><code>FormView.RenderOuterTable set = &quot;true&quot;</code> |
| <code>Api.WebControls.FormView.Row</code> | <code>FormView.Row = null</code> |
| <code>Api.WebControls.FormView.RowStyle</code> | <code>FormView.RowStyle = TableItemStyle {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; CssCla…</code> |
| <code>Api.WebControls.FormView.SaveControlState</code> | <code>FormView.SaveControlState = &quot;true&quot;</code><br><code>FormView.SaveControlState after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color …</code> |
| <code>Api.WebControls.FormView.SaveViewState</code> | <code>FormView.SaveViewState = &quot;[null, null, null, null, null, null, null, null, null, null]&quot;</code><br><code>FormView.SaveViewState after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00…</code> |
| <code>Api.WebControls.FormView.SelectedValue</code> | <code>FormView.SelectedValue = null</code> |
| <code>Api.WebControls.FormView.SetPageIndex</code> | <code>FormView.SetPageIndex(Int32) = &quot;ok&quot;</code><br><code>FormView.SetPageIndex(Int32) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Col…</code> |
| <code>Api.WebControls.FormView.TagKey</code> | <code>FormView.TagKey = &quot;HtmlTextWriterTag.Table&quot;</code> |
| <code>Api.WebControls.FormView.TopPagerRow</code> | <code>FormView.TopPagerRow = null</code> |
| <code>Api.WebControls.FormView.TrackViewState</code> | <code>FormView.TrackViewState = &quot;ok&quot;</code><br><code>FormView.TrackViewState after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #0…</code> |
| <code>Api.WebControls.FormView.UpdateItem</code> | <code>FormView.UpdateItem(Boolean) = &quot;-&gt; System.Web.HttpException: FormView &#39;fvItem&#39; must be in edit mode to update a record.&quot;</code> |
| <code>Api.WebControls.FormView.UpdateMethod</code> | <code>FormView.UpdateMethod = &quot;&quot;</code><br><code>FormView.UpdateMethod set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.IDataBoundControl.DataKeyNames</code> | <code>IDataBoundControl.DataKeyNames = &quot;[]&quot;</code><br><code>IDataBoundControl.DataKeyNames set = &quot;[System.String a, System.String b]&quot;</code> |
| <code>Api.WebControls.IDataBoundControl.DataMember</code> | <code>IDataBoundControl.DataMember = &quot;&quot;</code><br><code>IDataBoundControl.DataMember set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.IDataBoundControl.DataSource</code> | <code>IDataBoundControl.DataSource = &quot;[System.String Pen, System.String Bag]&quot;</code><br><code>IDataBoundControl.DataSource set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.IDataBoundControl.DataSourceID</code> | <code>IDataBoundControl.DataSourceID = &quot;&quot;</code><br><code>IDataBoundControl.DataSourceID set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.IDataBoundControl.DataSourceObject</code> | <code>IDataBoundControl.DataSourceObject = null</code> |
| <code>Api.WebControls.IDataBoundItemControl.DataKey</code> | <code>IDataBoundItemControl.DataKey = DataKey {}</code> |
| <code>Api.WebControls.IDataBoundItemControl.Mode</code> | <code>IDataBoundItemControl.Mode = &quot;DataBoundControlMode.ReadOnly&quot;</code> |
| <code>Api.WebControls.IPostBackContainer.GetPostBackOptions</code> | <code>IPostBackContainer.GetPostBackOptions(IButtonControl) = &quot;-&gt; System.ArgumentNullException: Value cannot be null. [param buttonControl]&quot;</code> |
| <code>Api.WebControls.WebControl.#ctor</code> | <code>WebControl.#ctor = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidt…</code><br><code>WebControl.#ctor(String) = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; Bo…</code><br><code>WebControl.#ctor(HtmlTextWriterTag) = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle…</code> |
| <code>Api.WebControls.WebControl.AccessKey</code> | <code>WebControl.AccessKey = &quot;&quot;</code><br><code>WebControl.AccessKey set = &quot;-&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]&quot;</code> |
| <code>Api.WebControls.WebControl.AddAttributesToRender</code> | <code>WebControl.AddAttributesToRender(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.AddAttributesToRender(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>WebControl.AddAttributesToRender(HtmlTextWriter) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl…</code> |
| <code>Api.WebControls.WebControl.ApplyStyle</code> | <code>WebControl.ApplyStyle(Style) = &quot;ok&quot;</code><br><code>WebControl.ApplyStyle(Style) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Col…</code> |
| <code>Api.WebControls.WebControl.Attributes</code> | <code>WebControl.Attributes = AttributeCollection []</code> |
| <code>Api.WebControls.WebControl.BackColor</code> | <code>WebControl.BackColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>WebControl.BackColor set = &quot;Color #FF3366CC Color [A=255, R=51, G=102, B=204]&quot;</code> |
| <code>Api.WebControls.WebControl.BorderColor</code> | <code>WebControl.BorderColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>WebControl.BorderColor set = &quot;Color #FF3366CC Color [A=255, R=51, G=102, B=204]&quot;</code> |
| <code>Api.WebControls.WebControl.BorderStyle</code> | <code>WebControl.BorderStyle = &quot;BorderStyle.NotSet&quot;</code><br><code>WebControl.BorderStyle set = &quot;BorderStyle.None&quot;</code> |
| <code>Api.WebControls.WebControl.BorderWidth</code> | <code>WebControl.BorderWidth = Unit </code><br><code>WebControl.BorderWidth set = Unit 120px</code> |
| <code>Api.WebControls.WebControl.ControlStyle</code> | <code>WebControl.ControlStyle = TableStyle {BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; B…</code> |
| <code>Api.WebControls.WebControl.ControlStyleCreated</code> | <code>WebControl.ControlStyleCreated = &quot;false&quot;</code> |
| <code>Api.WebControls.WebControl.CopyBaseAttributes</code> | <code>WebControl.CopyBaseAttributes(WebControl) = &quot;ok&quot;</code><br><code>WebControl.CopyBaseAttributes(WebControl) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; Bo…</code> |
| <code>Api.WebControls.WebControl.CreateControlStyle</code> | <code>WebControl.CreateControlStyle = TableStyle {BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #00000000 Color [Empty]; BorderStyle=Not…</code><br><code>WebControl.CreateControlStyle after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Co…</code> |
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
| <code>Api.WebControls.WebControl.MergeStyle</code> | <code>WebControl.MergeStyle(Style) = &quot;ok&quot;</code><br><code>WebControl.MergeStyle(Style) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Col…</code> |
| <code>Api.WebControls.WebControl.Render</code> | <code>WebControl.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.Render(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>WebControl.Render(HtmlTextWriter) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColo…</code> |
| <code>Api.WebControls.WebControl.RenderBeginTag</code> | <code>WebControl.RenderBeginTag(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.RenderBeginTag(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;table id=&quot;fvItem&quot;&gt;</code><br><code>  &#124; </code><br><code>WebControl.RenderBeginTag(HtmlTextWriter) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; Bo…</code> |
| <code>Api.WebControls.WebControl.RenderContents</code> | <code>WebControl.RenderContents(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.RenderContents(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>WebControl.RenderContents(HtmlTextWriter) after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; Bo…</code> |
| <code>Api.WebControls.WebControl.RenderEndTag</code> | <code>WebControl.RenderEndTag(HtmlTextWriter) = &quot;-&gt; System.InvalidOperationException: A PopEndTag was called without a corresponding PushEndTag.&quot;</code> |
| <code>Api.WebControls.WebControl.SaveViewState</code> | <code>WebControl.SaveViewState = &quot;[null, null, null, null, null, null, null, null, null, null]&quot;</code><br><code>WebControl.SaveViewState after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color #…</code> |
| <code>Api.WebControls.WebControl.SkinID</code> | <code>WebControl.SkinID = &quot;&quot;</code><br><code>WebControl.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.Style</code> | <code>WebControl.Style = CssStyleCollection {Count=Int32 0; Value=null}</code> |
| <code>Api.WebControls.WebControl.SupportsDisabledAttribute</code> | <code>WebControl.SupportsDisabledAttribute = &quot;false&quot;</code> |
| <code>Api.WebControls.WebControl.TabIndex</code> | <code>WebControl.TabIndex = &quot;Int16 0&quot;</code><br><code>WebControl.TabIndex set = &quot;Int16 1&quot;</code> |
| <code>Api.WebControls.WebControl.TagKey</code> | <code>WebControl.TagKey = &quot;HtmlTextWriterTag.Table&quot;</code> |
| <code>Api.WebControls.WebControl.TagName</code> | <code>WebControl.TagName = &quot;table&quot;</code> |
| <code>Api.WebControls.WebControl.ToolTip</code> | <code>WebControl.ToolTip = &quot;&quot;</code><br><code>WebControl.ToolTip set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.TrackViewState</code> | <code>WebControl.TrackViewState = &quot;ok&quot;</code><br><code>WebControl.TrackViewState after = FormView#fvItem {AccessKey=&quot;&quot;; AllowPaging=false; BackColor=Color #00000000 Color [Empty]; BackImageUrl=&quot;&quot;; BorderColor=Color …</code> |
| <code>Api.WebControls.WebControl.Width</code> | <code>WebControl.Width = Unit </code><br><code>WebControl.Width set = Unit 120px</code> |

## ② HTML 層

プローブサイトの RenderProbe.aspx で、1 ケース 1 リクエスト（ページライフサイクル全体）で描画したページ全体を比較します。下表はコンポーネント部分（div#probe）、設定時の例外、保存された ViewState の要約です。

| ケース | 設定 | 期待される描画 | 設定時の例外 |
|---|---|---|---|
| <code>.(default)</code> | <code>(nothing set)</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.AccessKey=text</code> | <code>c.AccessKey = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> | <code>c.AccessKey = &quot;probe&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=markup</code> | <code>c.AccessKey = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> | <code>c.AccessKey = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=japanese</code> | <code>c.AccessKey = &quot;日本語テキスト&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> | <code>c.AccessKey = &quot;日本語テキスト&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=empty</code> | <code>c.AccessKey = &quot;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.AccessKey=k</code> | <code>c.AccessKey = &quot;k&quot;;</code> | <code>&lt;table accesskey=&quot;k&quot; cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.AllowPaging=true</code> | <code>c.AllowPaging = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;&lt;table&gt;↵			&lt;tr&gt;↵				&lt;td&gt;&lt;span&gt;1&lt;/span&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$2&#39;)&quot;&gt;2&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$3&#39;)&quot;&gt;3&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$4&#39;)&quot;&gt;4&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$5&#39;)&quot;&gt;5&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$6&#39;)&quot;&gt;6&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$7&#39;)&quot;&gt;7&lt;/a&gt;&lt;/td&gt;↵			&lt;/tr&gt;↵		&lt;/table&gt;&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.AllowPaging=false</code> | <code>c.AllowPaging = false;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.AppRelativeTemplateSourceDirectory=text</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;probe&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=markup</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=japanese</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;日本語テキスト&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=empty</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.BackColor=Red</code> | <code>c.BackColor = Color.Red;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;background-color:Red;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.BackColor=#3366CC</code> | <code>c.BackColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;background-color:#3366CC;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.BackColor=Transparent</code> | <code>c.BackColor = Color.Transparent;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;background-color:Transparent;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.BackImageUrl=text</code> | <code>c.BackImageUrl = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;background-image:url(probe);border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.BackImageUrl=markup</code> | <code>c.BackImageUrl = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;background-image:url(&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;);border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.BackImageUrl=japanese</code> | <code>c.BackImageUrl = &quot;日本語テキスト&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;background-image:url(%e6%97%a5%e6%9c%ac%e8%aa%9e%e3%83%86%e3%82%ad%e3%82%b9%e3%83%88);border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.BackImageUrl=empty</code> | <code>c.BackImageUrl = &quot;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.BorderColor=Red</code> | <code>c.BorderColor = Color.Red;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-color:Red;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.BorderColor=#3366CC</code> | <code>c.BorderColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-color:#3366CC;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.BorderColor=Transparent</code> | <code>c.BorderColor = Color.Transparent;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-color:Transparent;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.BorderStyle=NotSet</code> | <code>c.BorderStyle = BorderStyle.NotSet;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.BorderStyle=None</code> | <code>c.BorderStyle = BorderStyle.None;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-style:None;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.BorderStyle=Dotted</code> | <code>c.BorderStyle = BorderStyle.Dotted;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-style:Dotted;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.BorderStyle=Dashed</code> | <code>c.BorderStyle = BorderStyle.Dashed;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-style:Dashed;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.BorderStyle=Solid</code> | <code>c.BorderStyle = BorderStyle.Solid;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-style:Solid;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.BorderStyle=Double</code> | <code>c.BorderStyle = BorderStyle.Double;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-style:Double;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.BorderStyle=Groove</code> | <code>c.BorderStyle = BorderStyle.Groove;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-style:Groove;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.BorderStyle=Ridge</code> | <code>c.BorderStyle = BorderStyle.Ridge;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-style:Ridge;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.BorderStyle=Inset</code> | <code>c.BorderStyle = BorderStyle.Inset;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-style:Inset;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.BorderStyle=Outset</code> | <code>c.BorderStyle = BorderStyle.Outset;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-style:Outset;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.BorderWidth=120px</code> | <code>c.BorderWidth = Unit.Pixel(120);</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-width:120px;border-style:solid;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.BorderWidth=50%</code> | <code>c.BorderWidth = Unit.Percentage(50);</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> | <code>c.BorderWidth = Unit.Percentage(50); -&gt; System.ArgumentOutOfRangeException: BorderWidth must be a non-negative number and cannot be a percentage. [param value]</code> |
| <code>.BorderWidth=2em</code> | <code>c.BorderWidth = new Unit(2, UnitType.Em);</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-width:2em;border-style:solid;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Caption=text</code> | <code>c.Caption = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;caption&gt;↵		probe↵	&lt;/caption&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Caption=markup</code> | <code>c.Caption = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;caption&gt;↵		&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;↵	&lt;/caption&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Caption=japanese</code> | <code>c.Caption = &quot;日本語テキスト&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;caption&gt;↵		日本語テキスト↵	&lt;/caption&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Caption=empty</code> | <code>c.Caption = &quot;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.CaptionAlign=NotSet</code> | <code>c.CaptionAlign = TableCaptionAlign.NotSet;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.CaptionAlign=Top</code> | <code>c.CaptionAlign = TableCaptionAlign.Top;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.CaptionAlign=Bottom</code> | <code>c.CaptionAlign = TableCaptionAlign.Bottom;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.CaptionAlign=Left</code> | <code>c.CaptionAlign = TableCaptionAlign.Left;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.CaptionAlign=Right</code> | <code>c.CaptionAlign = TableCaptionAlign.Right;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.CellPadding=0</code> | <code>c.CellPadding = 0;</code> | <code>&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.CellPadding=3</code> | <code>c.CellPadding = 3;</code> | <code>&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;3&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.CellPadding=-1</code> | <code>c.CellPadding = -1;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.CellSpacing=0</code> | <code>c.CellSpacing = 0;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.CellSpacing=3</code> | <code>c.CellSpacing = 3;</code> | <code>&lt;table cellspacing=&quot;3&quot; id=&quot;fvItem&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.CellSpacing=-1</code> | <code>c.CellSpacing = -1;</code> | <code>&lt;table id=&quot;fvItem&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.ClientIDMode=Inherit</code> | <code>c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.ClientIDMode=AutoID</code> | <code>c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.ClientIDMode=Predictable</code> | <code>c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.ClientIDMode=Static</code> | <code>c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.CssClass=text</code> | <code>c.CssClass = &quot;probe&quot;;</code> | <code>&lt;table class=&quot;probe&quot; cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.CssClass=markup</code> | <code>c.CssClass = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;table class=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.CssClass=japanese</code> | <code>c.CssClass = &quot;日本語テキスト&quot;;</code> | <code>&lt;table class=&quot;日本語テキスト&quot; cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.CssClass=empty</code> | <code>c.CssClass = &quot;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.DataKeyNames=Arial,MS-Gothic</code> | <code>c.DataKeyNames = new[] { &quot;Arial&quot;, &quot;MS Gothic&quot; };</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> | <code>DataBind -&gt; System.Web.HttpException: DataBinding: &#39;WebControlsProbe.ProbeData+Product&#39; does not contain a property with the name &#39;Arial&#39;.</code> |
| <code>.DataKeyNames=Code</code> | <code>c.DataKeyNames = new[] { &quot;Code&quot; };</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.DataMember=text</code> | <code>c.DataMember = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.DataMember=markup</code> | <code>c.DataMember = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.DataMember=japanese</code> | <code>c.DataMember = &quot;日本語テキスト&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.DataMember=empty</code> | <code>c.DataMember = &quot;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.DataSourceID=text</code> | <code>c.DataSourceID = &quot;probe&quot;;</code> | <code>status = 500</code> |  |
| <code>.DataSourceID=markup</code> | <code>c.DataSourceID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>status = 500</code> |  |
| <code>.DataSourceID=japanese</code> | <code>c.DataSourceID = &quot;日本語テキスト&quot;;</code> | <code>status = 500</code> |  |
| <code>.DataSourceID=empty</code> | <code>c.DataSourceID = &quot;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.DefaultMode=ReadOnly</code> | <code>c.DefaultMode = FormViewMode.ReadOnly;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.DefaultMode=Edit</code> | <code>c.DefaultMode = FormViewMode.Edit;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[edit: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.DefaultMode=Insert</code> | <code>c.DefaultMode = FormViewMode.Insert;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[insert]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.DeleteMethod=text</code> | <code>c.DeleteMethod = &quot;probe&quot;;</code> | <code>status = 500</code> |  |
| <code>.DeleteMethod=markup</code> | <code>c.DeleteMethod = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>status = 500</code> |  |
| <code>.DeleteMethod=japanese</code> | <code>c.DeleteMethod = &quot;日本語テキスト&quot;;</code> | <code>status = 500</code> |  |
| <code>.DeleteMethod=empty</code> | <code>c.DeleteMethod = &quot;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.EditRowStyle.BackColor=Red</code> | <code>c.EditRowStyle.BackColor = Color.Red;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.EditRowStyle.CssClass=text</code> | <code>c.EditRowStyle.CssClass = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.EditRowStyle.Font.Bold=true</code> | <code>c.EditRowStyle.Font.Bold = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.EditRowStyle.ForeColor=Red</code> | <code>c.EditRowStyle.ForeColor = Color.Red;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.EditRowStyle.Height=120px</code> | <code>c.EditRowStyle.Height = Unit.Pixel(120);</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.EditRowStyle.HorizontalAlign=NotSet</code> | <code>c.EditRowStyle.HorizontalAlign = HorizontalAlign.NotSet;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.EditRowStyle.VerticalAlign=NotSet</code> | <code>c.EditRowStyle.VerticalAlign = VerticalAlign.NotSet;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.EditRowStyle.Wrap=true</code> | <code>c.EditRowStyle.Wrap = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.EmptyDataRowStyle.BackColor=Red</code> | <code>c.EmptyDataRowStyle.BackColor = Color.Red;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.EmptyDataRowStyle.CssClass=text</code> | <code>c.EmptyDataRowStyle.CssClass = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.EmptyDataRowStyle.Font.Bold=true</code> | <code>c.EmptyDataRowStyle.Font.Bold = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.EmptyDataRowStyle.ForeColor=Red</code> | <code>c.EmptyDataRowStyle.ForeColor = Color.Red;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.EmptyDataRowStyle.Height=120px</code> | <code>c.EmptyDataRowStyle.Height = Unit.Pixel(120);</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.EmptyDataRowStyle.HorizontalAlign=NotSet</code> | <code>c.EmptyDataRowStyle.HorizontalAlign = HorizontalAlign.NotSet;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.EmptyDataRowStyle.VerticalAlign=NotSet</code> | <code>c.EmptyDataRowStyle.VerticalAlign = VerticalAlign.NotSet;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.EmptyDataRowStyle.Wrap=true</code> | <code>c.EmptyDataRowStyle.Wrap = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.EmptyDataText=text</code> | <code>c.EmptyDataText = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.EmptyDataText=markup</code> | <code>c.EmptyDataText = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.EmptyDataText=japanese</code> | <code>c.EmptyDataText = &quot;日本語テキスト&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.EmptyDataText=empty</code> | <code>c.EmptyDataText = &quot;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.EnableModelValidation=true</code> | <code>c.EnableModelValidation = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.EnableModelValidation=false</code> | <code>c.EnableModelValidation = false;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.EnableTheming=true</code> | <code>c.EnableTheming = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.EnableTheming=false</code> | <code>c.EnableTheming = false;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.EnableViewState=true</code> | <code>c.EnableViewState = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.EnableViewState=false</code> | <code>c.EnableViewState = false;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Enabled=true</code> | <code>c.Enabled = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Enabled=false</code> | <code>c.Enabled = false;</code> | <code>&lt;table class=&quot;aspNetDisabled&quot; cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Font.Bold=true</code> | <code>c.Font.Bold = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;font-weight:bold;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Font.Bold=false</code> | <code>c.Font.Bold = false;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;font-weight:normal;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Font.Italic=true</code> | <code>c.Font.Italic = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;font-style:italic;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Font.Italic=false</code> | <code>c.Font.Italic = false;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;font-style:normal;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Font.Name=text</code> | <code>c.Font.Name = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;font-family:probe;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Font.Name=markup</code> | <code>c.Font.Name = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;font-family:&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Font.Name=japanese</code> | <code>c.Font.Name = &quot;日本語テキスト&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;font-family:日本語テキスト;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Font.Name=empty</code> | <code>c.Font.Name = &quot;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Font.Names=Arial,MS-Gothic</code> | <code>c.Font.Names = new[] { &quot;Arial&quot;, &quot;MS Gothic&quot; };</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;font-family:Arial,MS Gothic;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Font.Overline=true</code> | <code>c.Font.Overline = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;text-decoration: overline;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Font.Overline=false</code> | <code>c.Font.Overline = false;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;text-decoration:none;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Font.Size=12pt</code> | <code>c.Font.Size = FontUnit.Point(12);</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;font-size:12pt;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Font.Size=Large</code> | <code>c.Font.Size = FontUnit.Large;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;font-size:Large;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Font.Size=150%</code> | <code>c.Font.Size = new FontUnit(Unit.Percentage(150));</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;font-size:150%;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Font.Strikeout=true</code> | <code>c.Font.Strikeout = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;text-decoration: line-through;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Font.Strikeout=false</code> | <code>c.Font.Strikeout = false;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;text-decoration:none;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Font.Underline=true</code> | <code>c.Font.Underline = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;text-decoration:underline;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Font.Underline=false</code> | <code>c.Font.Underline = false;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;text-decoration:none;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.FooterStyle.BackColor=Red</code> | <code>c.FooterStyle.BackColor = Color.Red;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.FooterStyle.CssClass=text</code> | <code>c.FooterStyle.CssClass = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.FooterStyle.Font.Bold=true</code> | <code>c.FooterStyle.Font.Bold = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.FooterStyle.ForeColor=Red</code> | <code>c.FooterStyle.ForeColor = Color.Red;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.FooterStyle.Height=120px</code> | <code>c.FooterStyle.Height = Unit.Pixel(120);</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.FooterStyle.HorizontalAlign=NotSet</code> | <code>c.FooterStyle.HorizontalAlign = HorizontalAlign.NotSet;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.FooterStyle.VerticalAlign=NotSet</code> | <code>c.FooterStyle.VerticalAlign = VerticalAlign.NotSet;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.FooterStyle.Wrap=true</code> | <code>c.FooterStyle.Wrap = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.FooterText=text</code> | <code>c.FooterText = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;probe&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.FooterText=markup</code> | <code>c.FooterText = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.FooterText=japanese</code> | <code>c.FooterText = &quot;日本語テキスト&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;日本語テキスト&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.FooterText=empty</code> | <code>c.FooterText = &quot;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.ForeColor=Red</code> | <code>c.ForeColor = Color.Red;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;color:Red;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.ForeColor=#3366CC</code> | <code>c.ForeColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;color:#3366CC;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.ForeColor=Transparent</code> | <code>c.ForeColor = Color.Transparent;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;color:Transparent;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.GridLines=None</code> | <code>c.GridLines = GridLines.None;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.GridLines=Horizontal</code> | <code>c.GridLines = GridLines.Horizontal;</code> | <code>&lt;table cellspacing=&quot;0&quot; rules=&quot;rows&quot; border=&quot;1&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.GridLines=Vertical</code> | <code>c.GridLines = GridLines.Vertical;</code> | <code>&lt;table cellspacing=&quot;0&quot; rules=&quot;cols&quot; border=&quot;1&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.GridLines=Both</code> | <code>c.GridLines = GridLines.Both;</code> | <code>&lt;table cellspacing=&quot;0&quot; rules=&quot;all&quot; border=&quot;1&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.HeaderStyle.BackColor=Red</code> | <code>c.HeaderStyle.BackColor = Color.Red;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.HeaderStyle.CssClass=text</code> | <code>c.HeaderStyle.CssClass = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.HeaderStyle.Font.Bold=true</code> | <code>c.HeaderStyle.Font.Bold = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.HeaderStyle.ForeColor=Red</code> | <code>c.HeaderStyle.ForeColor = Color.Red;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.HeaderStyle.Height=120px</code> | <code>c.HeaderStyle.Height = Unit.Pixel(120);</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.HeaderStyle.HorizontalAlign=NotSet</code> | <code>c.HeaderStyle.HorizontalAlign = HorizontalAlign.NotSet;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.HeaderStyle.VerticalAlign=NotSet</code> | <code>c.HeaderStyle.VerticalAlign = VerticalAlign.NotSet;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.HeaderStyle.Wrap=true</code> | <code>c.HeaderStyle.Wrap = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.HeaderText=text</code> | <code>c.HeaderText = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;probe&lt;/td&gt;↵	&lt;/tr&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.HeaderText=markup</code> | <code>c.HeaderText = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&lt;/td&gt;↵	&lt;/tr&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.HeaderText=japanese</code> | <code>c.HeaderText = &quot;日本語テキスト&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;日本語テキスト&lt;/td&gt;↵	&lt;/tr&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.HeaderText=empty</code> | <code>c.HeaderText = &quot;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Height=120px</code> | <code>c.Height = Unit.Pixel(120);</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;height:120px;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Height=50%</code> | <code>c.Height = Unit.Percentage(50);</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;height:50%;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Height=2em</code> | <code>c.Height = new Unit(2, UnitType.Em);</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;height:2em;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.HorizontalAlign=NotSet</code> | <code>c.HorizontalAlign = HorizontalAlign.NotSet;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.HorizontalAlign=Left</code> | <code>c.HorizontalAlign = HorizontalAlign.Left;</code> | <code>&lt;table cellspacing=&quot;0&quot; align=&quot;Left&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.HorizontalAlign=Center</code> | <code>c.HorizontalAlign = HorizontalAlign.Center;</code> | <code>&lt;table cellspacing=&quot;0&quot; align=&quot;Center&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.HorizontalAlign=Right</code> | <code>c.HorizontalAlign = HorizontalAlign.Right;</code> | <code>&lt;table cellspacing=&quot;0&quot; align=&quot;Right&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.HorizontalAlign=Justify</code> | <code>c.HorizontalAlign = HorizontalAlign.Justify;</code> | <code>&lt;table cellspacing=&quot;0&quot; align=&quot;Justify&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.ID=text</code> | <code>c.ID = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;probe&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.ID=markup</code> | <code>c.ID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.ID=japanese</code> | <code>c.ID = &quot;日本語テキスト&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;日本語テキスト&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.ID=empty</code> | <code>c.ID = &quot;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;ctl02&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.InsertMethod=text</code> | <code>c.InsertMethod = &quot;probe&quot;;</code> | <code>status = 500</code> |  |
| <code>.InsertMethod=markup</code> | <code>c.InsertMethod = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>status = 500</code> |  |
| <code>.InsertMethod=japanese</code> | <code>c.InsertMethod = &quot;日本語テキスト&quot;;</code> | <code>status = 500</code> |  |
| <code>.InsertMethod=empty</code> | <code>c.InsertMethod = &quot;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.InsertRowStyle.BackColor=Red</code> | <code>c.InsertRowStyle.BackColor = Color.Red;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.InsertRowStyle.CssClass=text</code> | <code>c.InsertRowStyle.CssClass = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.InsertRowStyle.Font.Bold=true</code> | <code>c.InsertRowStyle.Font.Bold = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.InsertRowStyle.ForeColor=Red</code> | <code>c.InsertRowStyle.ForeColor = Color.Red;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.InsertRowStyle.Height=120px</code> | <code>c.InsertRowStyle.Height = Unit.Pixel(120);</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.InsertRowStyle.HorizontalAlign=NotSet</code> | <code>c.InsertRowStyle.HorizontalAlign = HorizontalAlign.NotSet;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.InsertRowStyle.VerticalAlign=NotSet</code> | <code>c.InsertRowStyle.VerticalAlign = VerticalAlign.NotSet;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.InsertRowStyle.Wrap=true</code> | <code>c.InsertRowStyle.Wrap = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.ItemType=text</code> | <code>c.ItemType = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.ItemType=markup</code> | <code>c.ItemType = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.ItemType=japanese</code> | <code>c.ItemType = &quot;日本語テキスト&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.ItemType=empty</code> | <code>c.ItemType = &quot;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PageIndex=0</code> | <code>c.PageIndex = 0;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PageIndex=3</code> | <code>c.PageIndex = 3;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Desk (D4)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PageIndex=-1</code> | <code>c.PageIndex = -1;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PageIndex=1</code> | <code>c.PageIndex = 1;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Bag (B2)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.FirstPageImageUrl=text</code> | <code>c.PagerSettings.FirstPageImageUrl = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.FirstPageImageUrl=markup</code> | <code>c.PagerSettings.FirstPageImageUrl = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.FirstPageImageUrl=japanese</code> | <code>c.PagerSettings.FirstPageImageUrl = &quot;日本語テキスト&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.FirstPageImageUrl=empty</code> | <code>c.PagerSettings.FirstPageImageUrl = &quot;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.FirstPageText=text</code> | <code>c.PagerSettings.FirstPageText = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.FirstPageText=markup</code> | <code>c.PagerSettings.FirstPageText = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.FirstPageText=japanese</code> | <code>c.PagerSettings.FirstPageText = &quot;日本語テキスト&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.FirstPageText=empty</code> | <code>c.PagerSettings.FirstPageText = &quot;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.LastPageImageUrl=text</code> | <code>c.PagerSettings.LastPageImageUrl = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.LastPageImageUrl=markup</code> | <code>c.PagerSettings.LastPageImageUrl = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.LastPageImageUrl=japanese</code> | <code>c.PagerSettings.LastPageImageUrl = &quot;日本語テキスト&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.LastPageImageUrl=empty</code> | <code>c.PagerSettings.LastPageImageUrl = &quot;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.LastPageText=text</code> | <code>c.PagerSettings.LastPageText = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.LastPageText=markup</code> | <code>c.PagerSettings.LastPageText = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.LastPageText=japanese</code> | <code>c.PagerSettings.LastPageText = &quot;日本語テキスト&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.LastPageText=empty</code> | <code>c.PagerSettings.LastPageText = &quot;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.Mode=NextPrevious</code> | <code>c.PagerSettings.Mode = PagerButtons.NextPrevious;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.Mode=Numeric</code> | <code>c.PagerSettings.Mode = PagerButtons.Numeric;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.Mode=NextPreviousFirstLast</code> | <code>c.PagerSettings.Mode = PagerButtons.NextPreviousFirstLast;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.Mode=NumericFirstLast</code> | <code>c.PagerSettings.Mode = PagerButtons.NumericFirstLast;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.NextPageImageUrl=text</code> | <code>c.PagerSettings.NextPageImageUrl = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.NextPageImageUrl=markup</code> | <code>c.PagerSettings.NextPageImageUrl = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.NextPageImageUrl=japanese</code> | <code>c.PagerSettings.NextPageImageUrl = &quot;日本語テキスト&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.NextPageImageUrl=empty</code> | <code>c.PagerSettings.NextPageImageUrl = &quot;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.NextPageText=text</code> | <code>c.PagerSettings.NextPageText = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.NextPageText=markup</code> | <code>c.PagerSettings.NextPageText = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.NextPageText=japanese</code> | <code>c.PagerSettings.NextPageText = &quot;日本語テキスト&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.NextPageText=empty</code> | <code>c.PagerSettings.NextPageText = &quot;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.PageButtonCount=0</code> | <code>c.PagerSettings.PageButtonCount = 0;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> | <code>c.PagerSettings.PageButtonCount = 0; -&gt; System.ArgumentOutOfRangeException: Specified argument was out of the range of valid values. [param value]</code> |
| <code>.PagerSettings.PageButtonCount=3</code> | <code>c.PagerSettings.PageButtonCount = 3;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.PageButtonCount=-1</code> | <code>c.PagerSettings.PageButtonCount = -1;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> | <code>c.PagerSettings.PageButtonCount = -1; -&gt; System.ArgumentOutOfRangeException: Specified argument was out of the range of valid values. [param value]</code> |
| <code>.PagerSettings.Position=Bottom</code> | <code>c.PagerSettings.Position = PagerPosition.Bottom;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.Position=Top</code> | <code>c.PagerSettings.Position = PagerPosition.Top;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.Position=TopAndBottom</code> | <code>c.PagerSettings.Position = PagerPosition.TopAndBottom;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.PreviousPageImageUrl=text</code> | <code>c.PagerSettings.PreviousPageImageUrl = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.PreviousPageImageUrl=markup</code> | <code>c.PagerSettings.PreviousPageImageUrl = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.PreviousPageImageUrl=japanese</code> | <code>c.PagerSettings.PreviousPageImageUrl = &quot;日本語テキスト&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.PreviousPageImageUrl=empty</code> | <code>c.PagerSettings.PreviousPageImageUrl = &quot;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.PreviousPageText=text</code> | <code>c.PagerSettings.PreviousPageText = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.PreviousPageText=markup</code> | <code>c.PagerSettings.PreviousPageText = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.PreviousPageText=japanese</code> | <code>c.PagerSettings.PreviousPageText = &quot;日本語テキスト&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.PreviousPageText=empty</code> | <code>c.PagerSettings.PreviousPageText = &quot;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.Visible=true</code> | <code>c.PagerSettings.Visible = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerSettings.Visible=false</code> | <code>c.PagerSettings.Visible = false;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerStyle.BackColor=Red</code> | <code>c.PagerStyle.BackColor = Color.Red;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerStyle.CssClass=text</code> | <code>c.PagerStyle.CssClass = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerStyle.Font.Bold=true</code> | <code>c.PagerStyle.Font.Bold = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerStyle.ForeColor=Red</code> | <code>c.PagerStyle.ForeColor = Color.Red;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerStyle.Height=120px</code> | <code>c.PagerStyle.Height = Unit.Pixel(120);</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerStyle.HorizontalAlign=NotSet</code> | <code>c.PagerStyle.HorizontalAlign = HorizontalAlign.NotSet;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerStyle.VerticalAlign=NotSet</code> | <code>c.PagerStyle.VerticalAlign = VerticalAlign.NotSet;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.PagerStyle.Wrap=true</code> | <code>c.PagerStyle.Wrap = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.RenderOuterTable=true</code> | <code>c.RenderOuterTable = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.RenderOuterTable=false</code> | <code>c.RenderOuterTable = false;</code> | <code>[item: Pen (P1)]</code> |  |
| <code>.RowStyle.BackColor=Red</code> | <code>c.RowStyle.BackColor = Color.Red;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr style=&quot;background-color:Red;&quot;&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.RowStyle.CssClass=text</code> | <code>c.RowStyle.CssClass = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr class=&quot;probe&quot;&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.RowStyle.Font.Bold=true</code> | <code>c.RowStyle.Font.Bold = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr style=&quot;font-weight:bold;&quot;&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.RowStyle.ForeColor=Red</code> | <code>c.RowStyle.ForeColor = Color.Red;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr style=&quot;color:Red;&quot;&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.RowStyle.Height=120px</code> | <code>c.RowStyle.Height = Unit.Pixel(120);</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr style=&quot;height:120px;&quot;&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.RowStyle.HorizontalAlign=NotSet</code> | <code>c.RowStyle.HorizontalAlign = HorizontalAlign.NotSet;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.RowStyle.VerticalAlign=NotSet</code> | <code>c.RowStyle.VerticalAlign = VerticalAlign.NotSet;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.RowStyle.Wrap=true</code> | <code>c.RowStyle.Wrap = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.SelectMethod=text</code> | <code>c.SelectMethod = &quot;probe&quot;;</code> | <code>status = 500</code> |  |
| <code>.SelectMethod=markup</code> | <code>c.SelectMethod = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>status = 500</code> |  |
| <code>.SelectMethod=japanese</code> | <code>c.SelectMethod = &quot;日本語テキスト&quot;;</code> | <code>status = 500</code> |  |
| <code>.SelectMethod=empty</code> | <code>c.SelectMethod = &quot;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.SkinID=text</code> | <code>c.SkinID = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.SkinID=markup</code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.SkinID=japanese</code> | <code>c.SkinID = &quot;日本語テキスト&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.SkinID=empty</code> | <code>c.SkinID = &quot;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.TabIndex=0</code> | <code>c.TabIndex = 0;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.TabIndex=3</code> | <code>c.TabIndex = 3;</code> | <code>&lt;table tabindex=&quot;3&quot; cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.TabIndex=-1</code> | <code>c.TabIndex = -1;</code> | <code>&lt;table tabindex=&quot;-1&quot; cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.ToolTip=text</code> | <code>c.ToolTip = &quot;probe&quot;;</code> | <code>&lt;table title=&quot;probe&quot; cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.ToolTip=markup</code> | <code>c.ToolTip = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;table title=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.ToolTip=japanese</code> | <code>c.ToolTip = &quot;日本語テキスト&quot;;</code> | <code>&lt;table title=&quot;日本語テキスト&quot; cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.ToolTip=empty</code> | <code>c.ToolTip = &quot;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.UpdateMethod=text</code> | <code>c.UpdateMethod = &quot;probe&quot;;</code> | <code>status = 500</code> |  |
| <code>.UpdateMethod=markup</code> | <code>c.UpdateMethod = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>status = 500</code> |  |
| <code>.UpdateMethod=japanese</code> | <code>c.UpdateMethod = &quot;日本語テキスト&quot;;</code> | <code>status = 500</code> |  |
| <code>.UpdateMethod=empty</code> | <code>c.UpdateMethod = &quot;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.ValidateRequestMode=Inherit</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Inherit;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.ValidateRequestMode=Disabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Disabled;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.ValidateRequestMode=Enabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Enabled;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.ViewStateMode=Inherit</code> | <code>c.ViewStateMode = ViewStateMode.Inherit;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.ViewStateMode=Enabled</code> | <code>c.ViewStateMode = ViewStateMode.Enabled;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.ViewStateMode=Disabled</code> | <code>c.ViewStateMode = ViewStateMode.Disabled;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Visible=true</code> | <code>c.Visible = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Visible=false</code> | <code>c.Visible = false;</code> | <code></code> |  |
| <code>.Width=120px</code> | <code>c.Width = Unit.Pixel(120);</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;width:120px;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Width=50%</code> | <code>c.Width = Unit.Percentage(50);</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;width:50%;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Width=2em</code> | <code>c.Width = new Unit(2, UnitType.Em);</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;width:2em;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Attributes[data-probe]=text</code> | <code>c.Attributes[data-probe] = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; data-probe=&quot;probe&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Attributes[onfocus]=text</code> | <code>c.Attributes[onfocus] = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; onfocus=&quot;probe&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Attributes[data-probe]=markup</code> | <code>c.Attributes[data-probe] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; data-probe=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Attributes[onfocus]=markup</code> | <code>c.Attributes[onfocus] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; onfocus=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Style[color]=red</code> | <code>c.Style[color] = &quot;red&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;color:red;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Style[margin-left]=red</code> | <code>c.Style[margin-left] = &quot;red&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;margin-left:red;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Style[color]=1em</code> | <code>c.Style[color] = &quot;1em&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;color:1em;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Style[margin-left]=1em</code> | <code>c.Style[margin-left] = &quot;1em&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;margin-left:1em;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.AllowPaging=true+PageIndex=1</code> | <code>c.AllowPaging = true; c.PageIndex = 1;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Bag (B2)]&lt;/td&gt;↵	&lt;/tr&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;&lt;table&gt;↵			&lt;tr&gt;↵				&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$1&#39;)&quot;&gt;1&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;span&gt;2&lt;/span&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$3&#39;)&quot;&gt;3&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$4&#39;)&quot;&gt;4&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$5&#39;)&quot;&gt;5&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$6&#39;)&quot;&gt;6&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$7&#39;)&quot;&gt;7&lt;/a&gt;&lt;/td&gt;↵			&lt;/tr&gt;↵		&lt;/table&gt;&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.AllowPaging=true+PageIndex=3</code> | <code>c.AllowPaging = true; c.PageIndex = 3;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Desk (D4)]&lt;/td&gt;↵	&lt;/tr&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;&lt;table&gt;↵			&lt;tr&gt;↵				&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$1&#39;)&quot;&gt;1&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$2&#39;)&quot;&gt;2&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$3&#39;)&quot;&gt;3&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;span&gt;4&lt;/span&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$5&#39;)&quot;&gt;5&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$6&#39;)&quot;&gt;6&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$7&#39;)&quot;&gt;7&lt;/a&gt;&lt;/td&gt;↵			&lt;/tr&gt;↵		&lt;/table&gt;&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.AllowPaging=true+PagerSettings.Mode=NumericFirstLast</code> | <code>c.AllowPaging = true; c.PagerSettings.Mode = PagerButtons.NumericFirstLast;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;&lt;table&gt;↵			&lt;tr&gt;↵				&lt;td&gt;&lt;span&gt;1&lt;/span&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$2&#39;)&quot;&gt;2&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$3&#39;)&quot;&gt;3&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$4&#39;)&quot;&gt;4&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$5&#39;)&quot;&gt;5&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$6&#39;)&quot;&gt;6&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$7&#39;)&quot;&gt;7&lt;/a&gt;&lt;/td&gt;↵			&lt;/tr&gt;↵		&lt;/table&gt;&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.AllowPaging=true+PagerSettings.Mode=NextPreviousFirstLast</code> | <code>c.AllowPaging = true; c.PagerSettings.Mode = PagerButtons.NextPreviousFirstLast;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;&lt;table&gt;↵			&lt;tr&gt;↵				&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$Next&#39;)&quot;&gt;&gt;&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$Last&#39;)&quot;&gt;&gt;&gt;&lt;/a&gt;&lt;/td&gt;↵			&lt;/tr&gt;↵		&lt;/table&gt;&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.AllowPaging=true+PagerSettings.Position=Top</code> | <code>c.AllowPaging = true; c.PagerSettings.Position = PagerPosition.Top;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;&lt;table&gt;↵			&lt;tr&gt;↵				&lt;td&gt;&lt;span&gt;1&lt;/span&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$2&#39;)&quot;&gt;2&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$3&#39;)&quot;&gt;3&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$4&#39;)&quot;&gt;4&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$5&#39;)&quot;&gt;5&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$6&#39;)&quot;&gt;6&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$7&#39;)&quot;&gt;7&lt;/a&gt;&lt;/td&gt;↵			&lt;/tr&gt;↵		&lt;/table&gt;&lt;/td&gt;↵	&lt;/tr&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.AllowPaging=true+PagerStyle.BackColor=Red</code> | <code>c.AllowPaging = true; c.PagerStyle.BackColor = Color.Red;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;&lt;tr style=&quot;background-color:Red;&quot;&gt;↵		&lt;td colspan=&quot;2&quot;&gt;&lt;table&gt;↵			&lt;tr&gt;↵				&lt;td&gt;&lt;span&gt;1&lt;/span&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$2&#39;)&quot;&gt;2&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$3&#39;)&quot;&gt;3&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$4&#39;)&quot;&gt;4&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$5&#39;)&quot;&gt;5&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$6&#39;)&quot;&gt;6&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$7&#39;)&quot;&gt;7&lt;/a&gt;&lt;/td&gt;↵			&lt;/tr&gt;↵		&lt;/table&gt;&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.DefaultMode=Edit+DataKeyNames=Code</code> | <code>c.DefaultMode = FormViewMode.Edit; c.DataKeyNames = new[] { &quot;Code&quot; };</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[edit: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.HeaderText=text+FooterText=markup</code> | <code>c.HeaderText = &quot;probe&quot;; c.FooterText = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;probe&lt;/td&gt;↵	&lt;/tr&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.RenderOuterTable=false+AllowPaging=true</code> | <code>c.RenderOuterTable = false; c.AllowPaging = true;</code> | <code>[item: Pen (P1)]&lt;table&gt;↵	&lt;tr&gt;↵		&lt;td&gt;&lt;span&gt;1&lt;/span&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$2&#39;)&quot;&gt;2&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$3&#39;)&quot;&gt;3&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$4&#39;)&quot;&gt;4&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$5&#39;)&quot;&gt;5&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$6&#39;)&quot;&gt;6&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$7&#39;)&quot;&gt;7&lt;/a&gt;&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.GridLines=Both+CellPadding=3</code> | <code>c.GridLines = GridLines.Both; c.CellPadding = 3;</code> | <code>&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;3&quot; rules=&quot;all&quot; border=&quot;1&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Caption=text+CaptionAlign=Bottom</code> | <code>c.Caption = &quot;probe&quot;; c.CaptionAlign = TableCaptionAlign.Bottom;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;caption align=&quot;Bottom&quot;&gt;↵		probe↵	&lt;/caption&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.DefaultMode=Edit+EditRowStyle.BackColor=Red</code> | <code>c.DefaultMode = FormViewMode.Edit; c.EditRowStyle.BackColor = Color.Red;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr style=&quot;background-color:Red;&quot;&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[edit: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.DefaultMode=Insert+InsertRowStyle.BackColor=Red</code> | <code>c.DefaultMode = FormViewMode.Insert; c.InsertRowStyle.BackColor = Color.Red;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr style=&quot;background-color:Red;&quot;&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[insert]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Enabled=false+CssClass=text</code> | <code>c.Enabled = false; c.CssClass = &quot;probe&quot;;</code> | <code>&lt;table class=&quot;aspNetDisabled probe&quot; cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | <code>c.Width = Unit.Pixel(120); c.Height = new Unit(2, UnitType.Em); c.BorderWidth = Unit.Pixel(120); c.BorderStyle = BorderStyle.Solid; c.BackColor = Color.FromArgb(0x33, 0x66, 0xCC); c.ForeColor = Color.Red; c.Font.Bold = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;color:Red;background-color:#3366CC;border-width:120px;border-style:Solid;font-weight:bold;height:2em;width:120px;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.ForeColor=#3366CC+Style[color]=red</code> | <code>c.ForeColor = Color.FromArgb(0x33, 0x66, 0xCC); c.Style[color] = &quot;red&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;color:#3366CC;border-collapse:collapse;color:red;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.AllowPaging=true+EnableViewState=false</code> | <code>c.AllowPaging = true; c.EnableViewState = false;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;&lt;table&gt;↵			&lt;tr&gt;↵				&lt;td&gt;&lt;span&gt;1&lt;/span&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$2&#39;)&quot;&gt;2&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$3&#39;)&quot;&gt;3&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$4&#39;)&quot;&gt;4&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$5&#39;)&quot;&gt;5&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$6&#39;)&quot;&gt;6&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;fvItem&#39;,&#39;Page$7&#39;)&quot;&gt;7&lt;/a&gt;&lt;/td&gt;↵			&lt;/tr&gt;↵		&lt;/table&gt;&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>.AllowPaging=true+Visible=false</code> | <code>c.AllowPaging = true; c.Visible = false;</code> | <code></code> |  |
| <code>[naming container].(default)</code> | <code>[naming container] (nothing set)</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;table cellspacing=&quot;0&quot; id=&quot;box_fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵		&lt;tr&gt;↵			&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵		&lt;/tr&gt;↵	&lt;/table&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=AutoID</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;table cellspacing=&quot;0&quot; id=&quot;box_fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵		&lt;tr&gt;↵			&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵		&lt;/tr&gt;↵	&lt;/table&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Static</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵		&lt;tr&gt;↵			&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵		&lt;/tr&gt;↵	&lt;/table&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Predictable</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;table cellspacing=&quot;0&quot; id=&quot;box_fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵		&lt;tr&gt;↵			&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵		&lt;/tr&gt;↵	&lt;/table&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Inherit</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;table cellspacing=&quot;0&quot; id=&quot;box_fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵		&lt;tr&gt;↵			&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵		&lt;/tr&gt;↵	&lt;/table&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ID=text</code> | <code>[naming container] c.ID = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;table cellspacing=&quot;0&quot; id=&quot;box_probe&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵		&lt;tr&gt;↵			&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵		&lt;/tr&gt;↵	&lt;/table&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].AllowPaging=true</code> | <code>[naming container] c.AllowPaging = true;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;table cellspacing=&quot;0&quot; id=&quot;box_fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵		&lt;tr&gt;↵			&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵		&lt;/tr&gt;&lt;tr&gt;↵			&lt;td colspan=&quot;2&quot;&gt;&lt;table&gt;↵				&lt;tr&gt;↵					&lt;td&gt;&lt;span&gt;1&lt;/span&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;box$fvItem&#39;,&#39;Page$2&#39;)&quot;&gt;2&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;box$fvItem&#39;,&#39;Page$3&#39;)&quot;&gt;3&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;box$fvItem&#39;,&#39;Page$4&#39;)&quot;&gt;4&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;box$fvItem&#39;,&#39;Page$5&#39;)&quot;&gt;5&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;box$fvItem&#39;,&#39;Page$6&#39;)&quot;&gt;6&lt;/a&gt;&lt;/td&gt;&lt;td&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;box$fvItem&#39;,&#39;Page$7&#39;)&quot;&gt;7&lt;/a&gt;&lt;/td&gt;↵				&lt;/tr&gt;↵			&lt;/table&gt;&lt;/td&gt;↵		&lt;/tr&gt;↵	&lt;/table&gt;↵&lt;/div&gt;</code> |  |
| <code>[themed].(default)</code> | <code>[themed] (nothing set)</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>[themed].SkinID=text</code> | <code>[themed] c.SkinID = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>[themed].EnableTheming=false</code> | <code>[themed] c.EnableTheming = false;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>[themed].CssClass=text</code> | <code>[themed] c.CssClass = &quot;probe&quot;;</code> | <code>&lt;table class=&quot;probe&quot; cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>[templates].(default)</code> | <code>[templates] (nothing set)</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[header: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[footer: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>[templates].AllowPaging=true</code> | <code>[templates] c.AllowPaging = true;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[header: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[footer: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[pager]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>[templates].DefaultMode=Edit</code> | <code>[templates] c.DefaultMode = FormViewMode.Edit;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[header: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[edit: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[footer: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>[templates].HeaderText=text+FooterText=text</code> | <code>[templates] c.HeaderText = &quot;probe&quot;; c.FooterText = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[header: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[item: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[footer: Pen (P1)]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>[empty].(default)</code> | <code>[empty] (nothing set)</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td&gt;[empty]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>[empty].EmptyDataText=text</code> | <code>[empty] c.EmptyDataText = &quot;probe&quot;;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td&gt;[empty]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |
| <code>[empty].DefaultMode=Insert</code> | <code>[empty] c.DefaultMode = FormViewMode.Insert;</code> | <code>&lt;table cellspacing=&quot;0&quot; id=&quot;fvItem&quot; style=&quot;border-collapse:collapse;&quot;&gt;↵	&lt;tr&gt;↵		&lt;td colspan=&quot;2&quot;&gt;[insert]&lt;/td&gt;↵	&lt;/tr&gt;↵&lt;/table&gt;</code> |  |

## ③ シナリオ層

ブラウザ（Playwright）で FormViewScenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。

| # | 操作 | 対象 | 値 |
|---|---|---|---|
| 1 | goto | /FormViewScenario.aspx | <code></code> |
| 2 | snapshot |  | **FormView [render][databind][datasource][datasourceid][templates][dynamic] first request** |
| 3 | clicktextin | fvManual | <code>2</code> |
| 4 | snapshot |  | **FormView [paging][postbackevent] the page&#39;s form view: page 2** |
| 5 | clicktextin | fvManual | <code>Like</code> |
| 6 | snapshot |  | **FormView [rowcommand] a command bubbled to ItemCommand** |
| 7 | clicktextin | fvManual | <code>Edit</code> |
| 8 | snapshot |  | **FormView [editing] edit mode** |
| 9 | fill | txtFvName | <code>Renamed</code> |
| 10 | fill | txtFvPrice | <code>9.5</code> |
| 11 | clicktextin | fvManual | <code>Update</code> |
| 12 | snapshot |  | **FormView [editing][bind][datakeys] updated: keys, old and new values** |
| 13 | clicktextin | fvManual | <code>New</code> |
| 14 | snapshot |  | **FormView [inserting] insert mode** |
| 15 | fill | txtFvNewCode | <code>N9</code> |
| 16 | fill | txtFvNewName | <code>New item</code> |
| 17 | fill | txtFvNewPrice | <code>4</code> |
| 18 | clicktextin | fvManual | <code>Insert</code> |
| 19 | snapshot |  | **FormView [inserting][bind] inserted** |
| 20 | clicktextin | fvManual | <code>Delete</code> |
| 21 | snapshot |  | **FormView [editing][datakeys] deleted** |
| 22 | clicktextin | fvAuto | <code>3</code> |
| 23 | snapshot |  | **FormView [paging][datasourceid] the data source&#39;s form view: page 3** |
| 24 | clicktextin | fvAuto | <code>Edit</code> |
| 25 | fill | txtFaName | <code>Auto renamed</code> |
| 26 | clicktextin | fvAuto | <code>Update</code> |
| 27 | snapshot |  | **FormView [editing][datasourceid] updated through the data source** |
| 28 | clicktextin | fvAuto | <code>New</code> |
| 29 | fill | txtFaNewCode | <code>A8</code> |
| 30 | fill | txtFaNewName | <code>Auto new</code> |
| 31 | fill | txtFaNewPrice | <code>7</code> |
| 32 | clicktextin | fvAuto | <code>Insert</code> |
| 33 | snapshot |  | **FormView [inserting][datasourceid] inserted through the data source** |
| 34 | clicktextin | fvAuto | <code>Delete</code> |
| 35 | snapshot |  | **FormView [editing][datasourceid] deleted through the data source** |
| 36 | clicktextin | fvDynamic | <code>2</code> |
| 37 | snapshot |  | **FormView [dynamic][paging] the dynamic form view: page 2** |
| 38 | click | btnSubmit | <code></code> |
| 39 | snapshot |  | **FormView [viewstate] posted (the one without view state is empty)** |
| 40 | click | btnShow | <code></code> |
| 41 | snapshot |  | **FormView [visible][viewstate] the hidden form view shown** |

<details><summary>FormView [render][databind][datasource][datasourceid][templates][dynamic] first request（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Product
Pen 1.25 Edit New Delete Like
1 2 3 4 5 6 7
Automatic (ObjectDataSource)
Pen Edit New Delete
1 2 3 4 5 6 7
Empty, without view state, hidden, dynamic
Nothing to show
No view state: Pen
[dynamic: Pen (P1)]
1 2 3
Request 1: IsPostBack=False
Form fvManual: PageIndex=0 PageCount=7 CurrentMode=ReadOnly DataKey=P1
Form fvAuto: PageIndex=0 PageCount=0 CurrentMode=ReadOnly DataKey=
Form fvEmpty: PageIndex=0 PageCount=0 CurrentMode=ReadOnly DataKey=
Form fvNoViewState: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey=
Form fvHidden: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey= (hidden)
Form fvDynamic: PageIndex=0 PageCount=3 CurrentMode=ReadOnly DataKey=
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>FormView [paging][postbackevent] the page&#39;s form view: page 2（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Product
Bag 24.99 Edit New Delete Like
1 2 3 4 5 6 7
Automatic (ObjectDataSource)
Pen Edit New Delete
1 2 3 4 5 6 7
Empty, without view state, hidden, dynamic
Nothing to show
[dynamic: Pen (P1)]
1 2 3
Request 2: IsPostBack=True
ItemCommand fvManual: Page 2
PageIndexChanging: NewPageIndex=1
Form fvManual: PageIndex=1 PageCount=7 CurrentMode=ReadOnly DataKey=B2
Form fvAuto: PageIndex=0 PageCount=7 CurrentMode=ReadOnly DataKey=P1
Form fvEmpty: PageIndex=0 PageCount=0 CurrentMode=ReadOnly DataKey=
Form fvNoViewState: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey=
Form fvHidden: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey= (hidden)
Form fvDynamic: PageIndex=0 PageCount=3 CurrentMode=ReadOnly DataKey=
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>FormView [rowcommand] a command bubbled to ItemCommand（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Product
Bag 24.99 Edit New Delete Like
1 2 3 4 5 6 7
Automatic (ObjectDataSource)
Pen Edit New Delete
1 2 3 4 5 6 7
Empty, without view state, hidden, dynamic
Nothing to show
[dynamic: Pen (P1)]
1 2 3
Request 3: IsPostBack=True
ItemCommand fvManual: like B2
Form fvManual: PageIndex=1 PageCount=7 CurrentMode=ReadOnly DataKey=
Form fvAuto: PageIndex=0 PageCount=7 CurrentMode=ReadOnly DataKey=P1
Form fvEmpty: PageIndex=0 PageCount=0 CurrentMode=ReadOnly DataKey=
Form fvNoViewState: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey=
Form fvHidden: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey= (hidden)
Form fvDynamic: PageIndex=0 PageCount=3 CurrentMode=ReadOnly DataKey=
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>FormView [editing] edit mode（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Product
Update Cancel
1 2 3 4 5 6 7
Automatic (ObjectDataSource)
Pen Edit New Delete
1 2 3 4 5 6 7
Empty, without view state, hidden, dynamic
Nothing to show
[dynamic: Pen (P1)]
1 2 3
Request 4: IsPostBack=True
ItemCommand fvManual: Edit
ModeChanging: NewMode=Edit CancelingEdit=False
Form fvManual: PageIndex=1 PageCount=7 CurrentMode=Edit DataKey=B2
Form fvAuto: PageIndex=0 PageCount=7 CurrentMode=ReadOnly DataKey=P1
Form fvEmpty: PageIndex=0 PageCount=0 CurrentMode=ReadOnly DataKey=
Form fvNoViewState: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey=
Form fvHidden: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey= (hidden)
Form fvDynamic: PageIndex=0 PageCount=3 CurrentMode=ReadOnly DataKey=
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>FormView [editing][bind][datakeys] updated: keys, old and new values（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Product
Renamed 9.50 Edit New Delete Like
1 2 3 4 5 6 7
Automatic (ObjectDataSource)
Pen Edit New Delete
1 2 3 4 5 6 7
Empty, without view state, hidden, dynamic
Nothing to show
[dynamic: Pen (P1)]
1 2 3
Request 5: IsPostBack=True
ItemCommand fvManual: Update
ItemUpdating: Keys={Code=B2} OldValues={} NewValues={Name=Renamed, Price=9.5}
Form fvManual: PageIndex=1 PageCount=7 CurrentMode=ReadOnly DataKey=B2
Form fvAuto: PageIndex=0 PageCount=7 CurrentMode=ReadOnly DataKey=P1
Form fvEmpty: PageIndex=0 PageCount=0 CurrentMode=ReadOnly DataKey=
Form fvNoViewState: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey=
Form fvHidden: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey= (hidden)
Form fvDynamic: PageIndex=0 PageCount=3 CurrentMode=ReadOnly DataKey=
Store: P1:Pen:1.25:True B2:Renamed:9.5:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>FormView [inserting] insert mode（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Product
Insert Cancel
Automatic (ObjectDataSource)
Pen Edit New Delete
1 2 3 4 5 6 7
Empty, without view state, hidden, dynamic
Nothing to show
[dynamic: Pen (P1)]
1 2 3
Request 6: IsPostBack=True
ItemCommand fvManual: New
ModeChanging: NewMode=Insert CancelingEdit=False
Form fvManual: PageIndex=-1 PageCount=0 CurrentMode=Insert DataKey=
Form fvAuto: PageIndex=0 PageCount=7 CurrentMode=ReadOnly DataKey=P1
Form fvEmpty: PageIndex=0 PageCount=0 CurrentMode=ReadOnly DataKey=
Form fvNoViewState: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey=
Form fvHidden: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey= (hidden)
Form fvDynamic: PageIndex=0 PageCount=3 CurrentMode=ReadOnly DataKey=
Store: P1:Pen:1.25:True B2:Renamed:9.5:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>FormView [inserting][bind] inserted（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Product
Renamed 9.50 Edit New Delete Like
1 2 3 4 5 6 7 8
Automatic (ObjectDataSource)
Pen Edit New Delete
1 2 3 4 5 6 7
Empty, without view state, hidden, dynamic
Nothing to show
[dynamic: Pen (P1)]
1 2 3
Request 7: IsPostBack=True
ItemCommand fvManual: Insert
ItemInserting: Values={Code=N9, Name=New item, Price=4}
Form fvManual: PageIndex=1 PageCount=8 CurrentMode=ReadOnly DataKey=B2
Form fvAuto: PageIndex=0 PageCount=7 CurrentMode=ReadOnly DataKey=P1
Form fvEmpty: PageIndex=0 PageCount=0 CurrentMode=ReadOnly DataKey=
Form fvNoViewState: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey=
Form fvHidden: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey= (hidden)
Form fvDynamic: PageIndex=0 PageCount=3 CurrentMode=ReadOnly DataKey=
Store: P1:Pen:1.25:True B2:Renamed:9.5:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False N9:New item:4:False
```

</details>

<details><summary>FormView [editing][datakeys] deleted（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Product
Cup <&> 1,234.50 Edit New Delete Like
1 2 3 4 5 6 7
Automatic (ObjectDataSource)
Pen Edit New Delete
1 2 3 4 5 6 7
Empty, without view state, hidden, dynamic
Nothing to show
[dynamic: Pen (P1)]
1 2 3
Request 8: IsPostBack=True
ItemCommand fvManual: Delete
ItemDeleting: Keys={Code=B2} Values={}
Form fvManual: PageIndex=1 PageCount=7 CurrentMode=ReadOnly DataKey=C3
Form fvAuto: PageIndex=0 PageCount=7 CurrentMode=ReadOnly DataKey=P1
Form fvEmpty: PageIndex=0 PageCount=0 CurrentMode=ReadOnly DataKey=
Form fvNoViewState: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey=
Form fvHidden: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey= (hidden)
Form fvDynamic: PageIndex=0 PageCount=3 CurrentMode=ReadOnly DataKey=
Store: P1:Pen:1.25:True C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False N9:New item:4:False
```

</details>

<details><summary>FormView [paging][datasourceid] the data source&#39;s form view: page 3（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Product
Cup <&> 1,234.50 Edit New Delete Like
1 2 3 4 5 6 7
Automatic (ObjectDataSource)
Desk Edit New Delete
1 2 3 4 5 6 7
Empty, without view state, hidden, dynamic
Nothing to show
[dynamic: Pen (P1)]
1 2 3
Request 9: IsPostBack=True
Form fvManual: PageIndex=1 PageCount=7 CurrentMode=ReadOnly DataKey=C3
Form fvAuto: PageIndex=2 PageCount=7 CurrentMode=ReadOnly DataKey=
Form fvEmpty: PageIndex=0 PageCount=0 CurrentMode=ReadOnly DataKey=
Form fvNoViewState: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey=
Form fvHidden: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey= (hidden)
Form fvDynamic: PageIndex=0 PageCount=3 CurrentMode=ReadOnly DataKey=
Store: P1:Pen:1.25:True C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False N9:New item:4:False
```

</details>

<details><summary>FormView [editing][datasourceid] updated through the data source（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Product
Cup <&> 1,234.50 Edit New Delete Like
1 2 3 4 5 6 7
Automatic (ObjectDataSource)
Auto renamed Edit New Delete
1 2 3 4 5 6 7
Empty, without view state, hidden, dynamic
Nothing to show
[dynamic: Pen (P1)]
1 2 3
Request 11: IsPostBack=True
ObjectDataSource Updating: {Code=D4, Name=Auto renamed, Price=150}
ItemUpdated: AffectedRows=-1 Keys={Code=D4} NewValues={Name=Auto renamed, Price=150}
Form fvManual: PageIndex=1 PageCount=7 CurrentMode=ReadOnly DataKey=C3
Form fvAuto: PageIndex=2 PageCount=7 CurrentMode=ReadOnly DataKey=D4
Form fvEmpty: PageIndex=0 PageCount=0 CurrentMode=ReadOnly DataKey=
Form fvNoViewState: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey=
Form fvHidden: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey= (hidden)
Form fvDynamic: PageIndex=0 PageCount=3 CurrentMode=ReadOnly DataKey=
Store: P1:Pen:1.25:True C3:Cup <&>:1234.5:True D4:Auto renamed:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False N9:New item:4:False
```

</details>

<details><summary>FormView [inserting][datasourceid] inserted through the data source（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Product
Cup <&> 1,234.50 Edit New Delete Like
1 2 3 4 5 6 7
Automatic (ObjectDataSource)
Auto renamed Edit New Delete
1 2 3 4 5 6 7 8
Empty, without view state, hidden, dynamic
Nothing to show
[dynamic: Pen (P1)]
1 2 3
Request 13: IsPostBack=True
ObjectDataSource Inserting: {Code=A8, Name=Auto new, Price=7}
ItemInserted: AffectedRows=-1 Values={Code=A8, Name=Auto new, Price=7}
Form fvManual: PageIndex=1 PageCount=7 CurrentMode=ReadOnly DataKey=C3
Form fvAuto: PageIndex=2 PageCount=0 CurrentMode=ReadOnly DataKey=
Form fvEmpty: PageIndex=0 PageCount=0 CurrentMode=ReadOnly DataKey=
Form fvNoViewState: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey=
Form fvHidden: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey= (hidden)
Form fvDynamic: PageIndex=0 PageCount=3 CurrentMode=ReadOnly DataKey=
Store: P1:Pen:1.25:True C3:Cup <&>:1234.5:True D4:Auto renamed:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False N9:New item:4:False A8:Auto new:7:False
```

</details>

<details><summary>FormView [editing][datasourceid] deleted through the data source（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Product
Cup <&> 1,234.50 Edit New Delete Like
1 2 3 4 5 6 7
Automatic (ObjectDataSource)
Eraser Edit New Delete
1 2 3 4 5 6 7
Empty, without view state, hidden, dynamic
Nothing to show
[dynamic: Pen (P1)]
1 2 3
Request 14: IsPostBack=True
ItemDeleted: AffectedRows=-1 Keys={Code=D4}
Form fvManual: PageIndex=1 PageCount=7 CurrentMode=ReadOnly DataKey=C3
Form fvAuto: PageIndex=2 PageCount=8 CurrentMode=ReadOnly DataKey=D4
Form fvEmpty: PageIndex=0 PageCount=0 CurrentMode=ReadOnly DataKey=
Form fvNoViewState: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey=
Form fvHidden: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey= (hidden)
Form fvDynamic: PageIndex=0 PageCount=3 CurrentMode=ReadOnly DataKey=
Store: P1:Pen:1.25:True C3:Cup <&>:1234.5:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False N9:New item:4:False A8:Auto new:7:False
```

</details>

<details><summary>FormView [dynamic][paging] the dynamic form view: page 2（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Product
Cup <&> 1,234.50 Edit New Delete Like
1 2 3 4 5 6 7
Automatic (ObjectDataSource)
Eraser Edit New Delete
1 2 3 4 5 6 7
Empty, without view state, hidden, dynamic
Nothing to show
[dynamic: Bag (B2)]
1 2 3
Request 15: IsPostBack=True
Form fvManual: PageIndex=1 PageCount=7 CurrentMode=ReadOnly DataKey=C3
Form fvAuto: PageIndex=2 PageCount=7 CurrentMode=ReadOnly DataKey=E5
Form fvEmpty: PageIndex=0 PageCount=0 CurrentMode=ReadOnly DataKey=
Form fvNoViewState: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey=
Form fvHidden: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey= (hidden)
Form fvDynamic: PageIndex=1 PageCount=3 CurrentMode=ReadOnly DataKey=
Store: P1:Pen:1.25:True C3:Cup <&>:1234.5:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False N9:New item:4:False A8:Auto new:7:False
```

</details>

<details><summary>FormView [viewstate] posted (the one without view state is empty)（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Product
Cup <&> 1,234.50 Edit New Delete Like
1 2 3 4 5 6 7
Automatic (ObjectDataSource)
Eraser Edit New Delete
1 2 3 4 5 6 7
Empty, without view state, hidden, dynamic
Nothing to show
[dynamic: Bag (B2)]
1 2 3
Request 16: IsPostBack=True
Form fvManual: PageIndex=1 PageCount=7 CurrentMode=ReadOnly DataKey=C3
Form fvAuto: PageIndex=2 PageCount=7 CurrentMode=ReadOnly DataKey=E5
Form fvEmpty: PageIndex=0 PageCount=0 CurrentMode=ReadOnly DataKey=
Form fvNoViewState: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey=
Form fvHidden: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey= (hidden)
Form fvDynamic: PageIndex=1 PageCount=3 CurrentMode=ReadOnly DataKey=
Store: P1:Pen:1.25:True C3:Cup <&>:1234.5:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False N9:New item:4:False A8:Auto new:7:False
```

</details>

<details><summary>FormView [visible][viewstate] the hidden form view shown（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Product
Cup <&> 1,234.50 Edit New Delete Like
1 2 3 4 5 6 7
Automatic (ObjectDataSource)
Eraser Edit New Delete
1 2 3 4 5 6 7
Empty, without view state, hidden, dynamic
Nothing to show
Hidden: Pen
[dynamic: Bag (B2)]
1 2 3
Request 17: IsPostBack=True
Click btnShow
Form fvManual: PageIndex=1 PageCount=7 CurrentMode=ReadOnly DataKey=C3
Form fvAuto: PageIndex=2 PageCount=7 CurrentMode=ReadOnly DataKey=E5
Form fvEmpty: PageIndex=0 PageCount=0 CurrentMode=ReadOnly DataKey=
Form fvNoViewState: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey=
Form fvHidden: PageIndex=0 PageCount=2 CurrentMode=ReadOnly DataKey=
Form fvDynamic: PageIndex=1 PageCount=3 CurrentMode=ReadOnly DataKey=
Store: P1:Pen:1.25:True C3:Cup <&>:1234.5:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False N9:New item:4:False A8:Auto new:7:False
```

</details>

