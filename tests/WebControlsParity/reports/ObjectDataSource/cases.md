# ObjectDataSource テストケース一覧

変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。
このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。

- 正解データの採取: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0
- ① API 層: 172 ケース（180 メンバー）
- ② HTML 層: 114 ケース
- ③ シナリオ層: 15 スナップショット

## 機構マトリクス

型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。

| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |
|---|---|---|---|
| プロパティ → HTML 描画 | すべてのコントロール | 107 | 1 |
| ViewState の保存と復元 | Control | 6 | 14 |
| Visible=false（描画されず状態は保持） | Control.Visible | 2 | 1 |
| 名前付けコンテナと ClientIDMode | Control.ClientIDMode | 6 | 0 |
| 動的に生成したコントロールのライフサイクル | Control | 0 | 2 |
| データバインド（DataBind / <%# %>） | Control.DataBind | 0 | 1 |

該当しない機構: テーマとスキン（SkinID / EnableTheming）、Enabled=false・Disabled=true（描画とポスト値の扱い）、ポストバックデータの取り込み、変更イベント（*Changed、ServerChange）、ReadOnly（ポストされた値を無視）、リクエスト検証（ValidateRequestMode）、ポストバックイベント、コマンドイベントと親へのバブル（CommandName）、送信ボタン／スクリプトによるポストバック（UseSubmitBehavior）、ブラウザ側のクリック処理（OnClientClick）、クロスページポストバック（PostBackUrl）、既定のボタン（Panel の DefaultButton）、AutoPostBack（クライアントスクリプトによるポストバック）、CausesValidation / ValidationGroup、検証対象（サーバー側の検証）、検証対象（ブラウザ側の検証）、双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値）、項目（ListItem の Selected・Enabled・Text・Value と ViewState）、複数選択、繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）、ポスト値のイベント検証（一覧にない値・無効な項目の拒否）、サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）、ブラウザ側の検証スクリプト（EnableClientScript）、検証結果のまとめ（ValidationSummary）、ページング、並べ替え、項目の挿入、行・項目の編集（編集・更新・取り消し・削除）、行・項目の選択、行・項目のコマンド（RowCommand・ItemCommand へのバブル）、データキー（DataKeyNames・DataKeys）、データソースコントロールとの連携（DataSourceID）、テンプレート、データソース連携（DataSource・DataTextField など）、クライアントコールバック

## ① API 層

ObjectDataSource とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の ObjectDataSource インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。

| ケース | 期待値（.NET Framework） |
|---|---|
| <code>Api.Control.#ctor</code> | <code>Control.#ctor = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMode=In…</code> |
| <code>Api.Control.Adapter</code> | <code>Control.Adapter = null</code> |
| <code>Api.Control.AddParsedSubObject</code> | <code>Control.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>Control.AddParsedSubObject(Object) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=…</code> |
| <code>Api.Control.AddedControl</code> | <code>Control.AddedControl(Control,Int32) = &quot;ok&quot;</code><br><code>Control.AddedControl(Control,Int32) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID…</code> |
| <code>Api.Control.AppRelativeTemplateSourceDirectory</code> | <code>Control.AppRelativeTemplateSourceDirectory = &quot;&quot;</code><br><code>Control.AppRelativeTemplateSourceDirectory set = &quot;-&gt; System.ArgumentException: The relative virtual path &#39;Abc 123&#39; is not allowed here.&quot;</code> |
| <code>Api.Control.ApplyStyleSheetSkin</code> | <code>Control.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>Control.ApplyStyleSheetSkin(Page) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;…</code> |
| <code>Api.Control.BeginRenderTracing</code> | <code>Control.BeginRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.BeginRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.BeginRenderTracing(TextWriter,Object) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;…</code> |
| <code>Api.Control.BindingContainer</code> | <code>Control.BindingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Clien…</code> |
| <code>Api.Control.BuildProfileTree</code> | <code>Control.BuildProfileTree(String,Boolean) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.ChildControlsCreated</code> | <code>Control.ChildControlsCreated = &quot;false&quot;</code><br><code>Control.ChildControlsCreated set = &quot;ok&quot;</code> |
| <code>Api.Control.ClearCachedClientID</code> | <code>Control.ClearCachedClientID = &quot;ok&quot;</code><br><code>Control.ClearCachedClientID after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;odsPro…</code> |
| <code>Api.Control.ClearChildControlState</code> | <code>Control.ClearChildControlState = &quot;ok&quot;</code><br><code>Control.ClearChildControlState after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;ods…</code> |
| <code>Api.Control.ClearChildState</code> | <code>Control.ClearChildState = &quot;ok&quot;</code><br><code>Control.ClearChildState after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;odsProduct…</code> |
| <code>Api.Control.ClearChildViewState</code> | <code>Control.ClearChildViewState = &quot;ok&quot;</code><br><code>Control.ClearChildViewState after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;odsPro…</code> |
| <code>Api.Control.ClearEffectiveClientIDMode</code> | <code>Control.ClearEffectiveClientIDMode = &quot;ok&quot;</code><br><code>Control.ClearEffectiveClientIDMode after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=…</code> |
| <code>Api.Control.ClientID</code> | <code>Control.ClientID = &quot;odsProducts&quot;</code> |
| <code>Api.Control.ClientIDMode</code> | <code>Control.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>Control.ClientIDMode set = &quot;-&gt; System.NotSupportedException: Specified method is not supported.&quot;</code> |
| <code>Api.Control.ClientIDSeparator</code> | <code>Control.ClientIDSeparator = &quot;&#39;_&#39;&quot;</code> |
| <code>Api.Control.Context</code> | <code>Control.Context = null</code> |
| <code>Api.Control.Controls</code> | <code>Control.Controls = ControlCollection []</code> |
| <code>Api.Control.CreateChildControls</code> | <code>Control.CreateChildControls = &quot;ok&quot;</code><br><code>Control.CreateChildControls after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;odsPro…</code> |
| <code>Api.Control.CreateControlCollection</code> | <code>Control.CreateControlCollection = ControlCollection []</code><br><code>Control.CreateControlCollection after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;od…</code> |
| <code>Api.Control.DataBind</code> | <code>Control.DataBind = &quot;ok&quot;</code><br><code>Control.DataBind after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;odsProducts&quot;; Cli…</code><br><code>Control.DataBind(Boolean) = &quot;ok&quot;</code><br><code>Control.DataBind(Boolean) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;odsProdu…</code> |
| <code>Api.Control.DataBindChildren</code> | <code>Control.DataBindChildren = &quot;ok&quot;</code><br><code>Control.DataBindChildren after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;odsProduc…</code> |
| <code>Api.Control.DataBinding</code> | <code>Control.DataBinding = &quot;added and removed&quot;</code> |
| <code>Api.Control.DataItemContainer</code> | <code>Control.DataItemContainer = null</code> |
| <code>Api.Control.DataKeysContainer</code> | <code>Control.DataKeysContainer = null</code> |
| <code>Api.Control.DesignMode</code> | <code>Control.DesignMode = &quot;false&quot;</code> |
| <code>Api.Control.Dispose</code> | <code>Control.Dispose = &quot;ok&quot;</code><br><code>Control.Dispose after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;odsProducts&quot;; Clie…</code> |
| <code>Api.Control.Disposed</code> | <code>Control.Disposed = &quot;added and removed&quot;</code> |
| <code>Api.Control.EnableTheming</code> | <code>Control.EnableTheming = &quot;false&quot;</code><br><code>Control.EnableTheming set = &quot;-&gt; System.NotSupportedException: Control of type &#39;ObjectDataSource&#39; does not support theming.&quot;</code> |
| <code>Api.Control.EnableViewState</code> | <code>Control.EnableViewState = &quot;true&quot;</code><br><code>Control.EnableViewState set = &quot;true&quot;</code> |
| <code>Api.Control.EndRenderTracing</code> | <code>Control.EndRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.EndRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.EndRenderTracing(TextWriter,Object) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; …</code> |
| <code>Api.Control.EnsureChildControls</code> | <code>Control.EnsureChildControls = &quot;ok&quot;</code><br><code>Control.EnsureChildControls after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;odsPro…</code> |
| <code>Api.Control.EnsureID</code> | <code>Control.EnsureID = &quot;ok&quot;</code><br><code>Control.EnsureID after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;odsProducts&quot;; Cli…</code> |
| <code>Api.Control.Events</code> | <code>Control.Events = &quot;System.ComponentModel.EventHandlerList&quot;</code> |
| <code>Api.Control.FindControl</code> | <code>Control.FindControl(String) = Label#lblTitle {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code><br><code>Control.FindControl(String) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;odsPro…</code><br><code>Control.FindControl(String,Int32) = null</code><br><code>Control.FindControl(String,Int32) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;…</code> |
| <code>Api.Control.Focus</code> | <code>Control.Focus = &quot;-&gt; System.NotSupportedException: Control of type &#39;ObjectDataSource&#39; does not support the Focus operation.&quot;</code> |
| <code>Api.Control.GetDesignModeState</code> | <code>Control.GetDesignModeState = &quot;[]&quot;</code><br><code>Control.GetDesignModeState after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;odsProd…</code> |
| <code>Api.Control.GetRouteUrl</code> | <code>Control.GetRouteUrl(Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.GetUniqueIDRelativeTo</code> | <code>Control.GetUniqueIDRelativeTo(Control) = &quot;odsProducts&quot;</code><br><code>Control.GetUniqueIDRelativeTo(Control) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; Clien…</code> |
| <code>Api.Control.HasChildViewState</code> | <code>Control.HasChildViewState = &quot;false&quot;</code> |
| <code>Api.Control.HasControls</code> | <code>Control.HasControls = &quot;false&quot;</code><br><code>Control.HasControls after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;odsProducts&quot;; …</code> |
| <code>Api.Control.HasEvents</code> | <code>Control.HasEvents = &quot;true&quot;</code><br><code>Control.HasEvents after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;odsProducts&quot;; Cl…</code> |
| <code>Api.Control.ID</code> | <code>Control.ID = &quot;odsProducts&quot;</code><br><code>Control.ID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.IdSeparator</code> | <code>Control.IdSeparator = &quot;&#39;$&#39;&quot;</code> |
| <code>Api.Control.Init</code> | <code>Control.Init = &quot;added and removed&quot;</code> |
| <code>Api.Control.IsChildControlStateCleared</code> | <code>Control.IsChildControlStateCleared = &quot;false&quot;</code> |
| <code>Api.Control.IsLiteralContent</code> | <code>Control.IsLiteralContent = &quot;false&quot;</code><br><code>Control.IsLiteralContent after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;odsProduc…</code> |
| <code>Api.Control.IsTrackingViewState</code> | <code>Control.IsTrackingViewState = &quot;false&quot;</code> |
| <code>Api.Control.IsViewStateEnabled</code> | <code>Control.IsViewStateEnabled = &quot;true&quot;</code> |
| <code>Api.Control.Load</code> | <code>Control.Load = &quot;added and removed&quot;</code> |
| <code>Api.Control.LoadControlState</code> | <code>Control.LoadControlState(Object) = &quot;ok&quot;</code><br><code>Control.LoadControlState(Object) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;o…</code> |
| <code>Api.Control.LoadViewState</code> | <code>Control.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Web.UI.Pair&#39;.&quot;</code> |
| <code>Api.Control.LoadViewStateByID</code> | <code>Control.LoadViewStateByID = &quot;false&quot;</code> |
| <code>Api.Control.MapPathSecure</code> | <code>Control.MapPathSecure(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.NamingContainer</code> | <code>Control.NamingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code> |
| <code>Api.Control.OnBubbleEvent</code> | <code>Control.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Control.OnBubbleEvent(Object,EventArgs) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; Clie…</code> |
| <code>Api.Control.OnDataBinding</code> | <code>Control.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnDataBinding(EventArgs) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;o…</code> |
| <code>Api.Control.OnInit</code> | <code>Control.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnInit(EventArgs) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;odsProdu…</code> |
| <code>Api.Control.OnLoad</code> | <code>Control.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnLoad(EventArgs) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;odsProdu…</code> |
| <code>Api.Control.OnPreRender</code> | <code>Control.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnPreRender(EventArgs) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;ods…</code> |
| <code>Api.Control.OnUnload</code> | <code>Control.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnUnload(EventArgs) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;odsPro…</code> |
| <code>Api.Control.OpenFile</code> | <code>Control.OpenFile(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.Page</code> | <code>Control.Page = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=&quot;&quot;; …</code><br><code>Control.Page set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=…</code> |
| <code>Api.Control.Parent</code> | <code>Control.Parent = HtmlForm#form1 {Action=&quot;&quot;; ClientID=&quot;form1&quot;; ClientIDMode=Inherit; DefaultButton=&quot;&quot;; DefaultFocus=&quot;&quot;; Disabled=false; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.PreRender</code> | <code>Control.PreRender = &quot;added and removed&quot;</code> |
| <code>Api.Control.RaiseBubbleEvent</code> | <code>Control.RaiseBubbleEvent(Object,EventArgs) = &quot;ok&quot;</code><br><code>Control.RaiseBubbleEvent(Object,EventArgs) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; C…</code> |
| <code>Api.Control.RemovedControl</code> | <code>Control.RemovedControl(Control) = &quot;ok&quot;</code><br><code>Control.RemovedControl(Control) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;od…</code> |
| <code>Api.Control.Render</code> | <code>Control.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.Render(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.Render(HtmlTextWriter) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;ods…</code> |
| <code>Api.Control.RenderChildren</code> | <code>Control.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderChildren(HtmlTextWriter) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; Clien…</code> |
| <code>Api.Control.RenderControl</code> | <code>Control.RenderControl(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderControl(HtmlTextWriter) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; Client…</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; Cache…</code> |
| <code>Api.Control.RenderingCompatibility</code> | <code>Control.RenderingCompatibility = &quot;System.Version 4.0&quot;</code><br><code>Control.RenderingCompatibility set = &quot;System.Version 4.0&quot;</code> |
| <code>Api.Control.ResolveAdapter</code> | <code>Control.ResolveAdapter = null</code><br><code>Control.ResolveAdapter after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;odsProducts…</code> |
| <code>Api.Control.ResolveClientUrl</code> | <code>Control.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveClientUrl(String) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;o…</code> |
| <code>Api.Control.ResolveUrl</code> | <code>Control.ResolveUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveUrl(String) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;odsProd…</code> |
| <code>Api.Control.SaveControlState</code> | <code>Control.SaveControlState = null</code><br><code>Control.SaveControlState after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;odsProduc…</code> |
| <code>Api.Control.SaveViewState</code> | <code>Control.SaveViewState = null</code><br><code>Control.SaveViewState after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;odsProducts&quot;…</code> |
| <code>Api.Control.SetDesignModeState</code> | <code>Control.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>Control.SetDesignModeState(IDictionary) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; Clie…</code> |
| <code>Api.Control.SetRenderMethodDelegate</code> | <code>Control.SetRenderMethodDelegate(RenderMethod) = &quot;ok&quot;</code><br><code>Control.SetRenderMethodDelegate(RenderMethod) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;…</code> |
| <code>Api.Control.SetTraceData</code> | <code>Control.SetTraceData(Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID…</code><br><code>Control.SetTraceData(Object,Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object,Object) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; C…</code> |
| <code>Api.Control.Site</code> | <code>Control.Site = null</code><br><code>Control.Site set = null</code> |
| <code>Api.Control.SkinID</code> | <code>Control.SkinID = &quot;&quot;</code><br><code>Control.SkinID set = &quot;-&gt; System.NotSupportedException: Control of type &#39;ObjectDataSource&#39; does not support theming.&quot;</code> |
| <code>Api.Control.TemplateControl</code> | <code>Control.TemplateControl = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code><br><code>Control.TemplateControl set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Cl…</code> |
| <code>Api.Control.TemplateSourceDirectory</code> | <code>Control.TemplateSourceDirectory = &quot;&quot;</code> |
| <code>Api.Control.TrackViewState</code> | <code>Control.TrackViewState = &quot;ok&quot;</code><br><code>Control.TrackViewState after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;odsProducts…</code> |
| <code>Api.Control.UniqueID</code> | <code>Control.UniqueID = &quot;odsProducts&quot;</code> |
| <code>Api.Control.Unload</code> | <code>Control.Unload = &quot;added and removed&quot;</code> |
| <code>Api.Control.ValidateRequestMode</code> | <code>Control.ValidateRequestMode = &quot;ValidateRequestMode.Inherit&quot;</code><br><code>Control.ValidateRequestMode set = &quot;ValidateRequestMode.Disabled&quot;</code> |
| <code>Api.Control.ViewState</code> | <code>Control.ViewState = StateBag []</code> |
| <code>Api.Control.ViewStateIgnoresCase</code> | <code>Control.ViewStateIgnoresCase = &quot;false&quot;</code> |
| <code>Api.Control.ViewStateMode</code> | <code>Control.ViewStateMode = &quot;ViewStateMode.Inherit&quot;</code><br><code>Control.ViewStateMode set = &quot;ViewStateMode.Enabled&quot;</code> |
| <code>Api.Control.Visible</code> | <code>Control.Visible = &quot;false&quot;</code><br><code>Control.Visible set = &quot;-&gt; System.NotSupportedException: Control of type &#39;ObjectDataSource&#39; is a non-visual control and does not support setting the Visible prop…</code> |
| <code>Api.DataSourceControl.#ctor</code> | <code>DataSourceControl.#ctor = &quot;-&gt; System.MemberAccessException: Cannot create an instance of System.Web.UI.DataSourceControl because it is an abstract class.&quot;</code> |
| <code>Api.DataSourceControl.ApplyStyleSheetSkin</code> | <code>DataSourceControl.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>DataSourceControl.ApplyStyleSheetSkin(Page) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; …</code> |
| <code>Api.DataSourceControl.ClientID</code> | <code>DataSourceControl.ClientID = &quot;odsProducts&quot;</code> |
| <code>Api.DataSourceControl.ClientIDMode</code> | <code>DataSourceControl.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>DataSourceControl.ClientIDMode set = &quot;-&gt; System.NotSupportedException: Specified method is not supported.&quot;</code> |
| <code>Api.DataSourceControl.Controls</code> | <code>DataSourceControl.Controls = ControlCollection []</code> |
| <code>Api.DataSourceControl.CreateControlCollection</code> | <code>DataSourceControl.CreateControlCollection = ControlCollection []</code><br><code>DataSourceControl.CreateControlCollection after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; Cl…</code> |
| <code>Api.DataSourceControl.EnableTheming</code> | <code>DataSourceControl.EnableTheming = &quot;false&quot;</code><br><code>DataSourceControl.EnableTheming set = &quot;-&gt; System.NotSupportedException: Control of type &#39;ObjectDataSource&#39; does not support theming.&quot;</code> |
| <code>Api.DataSourceControl.FindControl</code> | <code>DataSourceControl.FindControl(String) = Label#lblTitle {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #000000…</code><br><code>DataSourceControl.FindControl(String) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; Client…</code> |
| <code>Api.DataSourceControl.Focus</code> | <code>DataSourceControl.Focus = &quot;-&gt; System.NotSupportedException: Control of type &#39;ObjectDataSource&#39; does not support the Focus operation.&quot;</code> |
| <code>Api.DataSourceControl.GetView</code> | <code>DataSourceControl.GetView(String) = &quot;-&gt; System.ArgumentException: The data source &#39;odsProducts&#39; only supports a single view named &#39;DefaultView&#39;. You may also le…</code> |
| <code>Api.DataSourceControl.GetViewNames</code> | <code>DataSourceControl.GetViewNames = &quot;[System.String DefaultView]&quot;</code><br><code>DataSourceControl.GetViewNames after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;ods…</code> |
| <code>Api.DataSourceControl.HasControls</code> | <code>DataSourceControl.HasControls = &quot;false&quot;</code><br><code>DataSourceControl.HasControls after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;odsP…</code> |
| <code>Api.DataSourceControl.RaiseDataSourceChangedEvent</code> | <code>DataSourceControl.RaiseDataSourceChangedEvent(EventArgs) = &quot;ok&quot;</code><br><code>DataSourceControl.RaiseDataSourceChangedEvent(EventArgs) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDe…</code> |
| <code>Api.DataSourceControl.RenderControl</code> | <code>DataSourceControl.RenderControl(HtmlTextWriter) = &quot;ok&quot;</code><br><code>DataSourceControl.RenderControl(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>DataSourceControl.RenderControl(HtmlTextWriter) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=…</code> |
| <code>Api.DataSourceControl.SkinID</code> | <code>DataSourceControl.SkinID = &quot;&quot;</code><br><code>DataSourceControl.SkinID set = &quot;-&gt; System.NotSupportedException: Control of type &#39;ObjectDataSource&#39; does not support theming.&quot;</code> |
| <code>Api.DataSourceControl.Visible</code> | <code>DataSourceControl.Visible = &quot;false&quot;</code><br><code>DataSourceControl.Visible set = &quot;-&gt; System.NotSupportedException: Control of type &#39;ObjectDataSource&#39; is a non-visual control and does not support setting the Vi…</code> |
| <code>Api.IControlBuilderAccessor.ControlBuilder</code> | <code>IControlBuilderAccessor.ControlBuilder = null</code> |
| <code>Api.IControlDesignerAccessor.GetDesignModeState</code> | <code>IControlDesignerAccessor.GetDesignModeState = &quot;[]&quot;</code><br><code>IControlDesignerAccessor.GetDesignModeState after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; …</code> |
| <code>Api.IControlDesignerAccessor.SetDesignModeState</code> | <code>IControlDesignerAccessor.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetDesignModeState(IDictionary) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDe…</code> |
| <code>Api.IControlDesignerAccessor.SetOwnerControl</code> | <code>IControlDesignerAccessor.SetOwnerControl(Control) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetOwnerControl(Control) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependenc…</code> |
| <code>Api.IControlDesignerAccessor.UserData</code> | <code>IControlDesignerAccessor.UserData = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.DataBindings</code> | <code>IDataBindingsAccessor.DataBindings = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.HasDataBindings</code> | <code>IDataBindingsAccessor.HasDataBindings = &quot;false&quot;</code> |
| <code>Api.IDataSource.DataSourceChanged</code> | <code>IDataSource.DataSourceChanged = &quot;added and removed&quot;</code> |
| <code>Api.IDataSource.GetView</code> | <code>IDataSource.GetView(String) = &quot;-&gt; System.ArgumentException: The data source &#39;odsProducts&#39; only supports a single view named &#39;DefaultView&#39;. You may also leave th…</code> |
| <code>Api.IDataSource.GetViewNames</code> | <code>IDataSource.GetViewNames = &quot;[System.String DefaultView]&quot;</code><br><code>IDataSource.GetViewNames after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;odsProduc…</code> |
| <code>Api.IExpressionsAccessor.Expressions</code> | <code>IExpressionsAccessor.Expressions = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.HasExpressions</code> | <code>IExpressionsAccessor.HasExpressions = &quot;false&quot;</code> |
| <code>Api.IParserAccessor.AddParsedSubObject</code> | <code>IParserAccessor.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>IParserAccessor.AddParsedSubObject(Object) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; C…</code> |
| <code>Api.IUrlResolutionService.ResolveClientUrl</code> | <code>IUrlResolutionService.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>IUrlResolutionService.ResolveClientUrl(String) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;…</code> |
| <code>Api.WebControls.ObjectDataSource.#ctor</code> | <code>ObjectDataSource.#ctor = ObjectDataSource# {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;&quot;; ClientIDMode=Inherit; Con…</code><br><code>ObjectDataSource.#ctor(String,String) = ObjectDataSource# {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;&quot;; ClientIDMo…</code> |
| <code>Api.WebControls.ObjectDataSource.CacheDuration</code> | <code>ObjectDataSource.CacheDuration = &quot;Int32 0&quot;</code><br><code>ObjectDataSource.CacheDuration set = &quot;Int32 1&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.CacheExpirationPolicy</code> | <code>ObjectDataSource.CacheExpirationPolicy = &quot;DataSourceCacheExpiry.Absolute&quot;</code><br><code>ObjectDataSource.CacheExpirationPolicy set = &quot;DataSourceCacheExpiry.Sliding&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.CacheKeyDependency</code> | <code>ObjectDataSource.CacheKeyDependency = &quot;&quot;</code><br><code>ObjectDataSource.CacheKeyDependency set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.ConflictDetection</code> | <code>ObjectDataSource.ConflictDetection = &quot;ConflictOptions.OverwriteChanges&quot;</code><br><code>ObjectDataSource.ConflictDetection set = &quot;ConflictOptions.CompareAllValues&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.ConvertNullToDBNull</code> | <code>ObjectDataSource.ConvertNullToDBNull = &quot;false&quot;</code><br><code>ObjectDataSource.ConvertNullToDBNull set = &quot;true&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.DataObjectTypeName</code> | <code>ObjectDataSource.DataObjectTypeName = &quot;&quot;</code><br><code>ObjectDataSource.DataObjectTypeName set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.Delete</code> | <code>ObjectDataSource.Delete = &quot;-&gt; System.NotSupportedException: Deleting is not supported by ObjectDataSource &#39;odsProducts&#39; unless the DeleteMethod is specified.&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.DeleteMethod</code> | <code>ObjectDataSource.DeleteMethod = &quot;&quot;</code><br><code>ObjectDataSource.DeleteMethod set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.DeleteParameters</code> | <code>ObjectDataSource.DeleteParameters = &quot;[]&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.Deleted</code> | <code>ObjectDataSource.Deleted = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.Deleting</code> | <code>ObjectDataSource.Deleting = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.EnableCaching</code> | <code>ObjectDataSource.EnableCaching = &quot;false&quot;</code><br><code>ObjectDataSource.EnableCaching set = &quot;true&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.EnablePaging</code> | <code>ObjectDataSource.EnablePaging = &quot;false&quot;</code><br><code>ObjectDataSource.EnablePaging set = &quot;true&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.FilterExpression</code> | <code>ObjectDataSource.FilterExpression = &quot;&quot;</code><br><code>ObjectDataSource.FilterExpression set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.FilterParameters</code> | <code>ObjectDataSource.FilterParameters = &quot;[]&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.Filtering</code> | <code>ObjectDataSource.Filtering = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.GetView</code> | <code>ObjectDataSource.GetView(String) = &quot;-&gt; System.ArgumentException: The data source &#39;odsProducts&#39; only supports a single view named &#39;DefaultView&#39;. You may also lea…</code> |
| <code>Api.WebControls.ObjectDataSource.GetViewNames</code> | <code>ObjectDataSource.GetViewNames = &quot;[System.String DefaultView]&quot;</code><br><code>ObjectDataSource.GetViewNames after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;odsP…</code> |
| <code>Api.WebControls.ObjectDataSource.Insert</code> | <code>ObjectDataSource.Insert = &quot;-&gt; System.NotSupportedException: Inserting is not supported by ObjectDataSource &#39;odsProducts&#39; unless the InsertMethod is specified.&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.InsertMethod</code> | <code>ObjectDataSource.InsertMethod = &quot;&quot;</code><br><code>ObjectDataSource.InsertMethod set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.InsertParameters</code> | <code>ObjectDataSource.InsertParameters = &quot;[]&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.Inserted</code> | <code>ObjectDataSource.Inserted = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.Inserting</code> | <code>ObjectDataSource.Inserting = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.LoadViewState</code> | <code>ObjectDataSource.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Web.UI.Pair&#39;.&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.MaximumRowsParameterName</code> | <code>ObjectDataSource.MaximumRowsParameterName = &quot;maximumRows&quot;</code><br><code>ObjectDataSource.MaximumRowsParameterName set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.ObjectCreated</code> | <code>ObjectDataSource.ObjectCreated = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.ObjectCreating</code> | <code>ObjectDataSource.ObjectCreating = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.ObjectDisposing</code> | <code>ObjectDataSource.ObjectDisposing = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.OldValuesParameterFormatString</code> | <code>ObjectDataSource.OldValuesParameterFormatString = &quot;{0}&quot;</code><br><code>ObjectDataSource.OldValuesParameterFormatString set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.OnInit</code> | <code>ObjectDataSource.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>ObjectDataSource.OnInit(EventArgs) after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=…</code> |
| <code>Api.WebControls.ObjectDataSource.ParsingCulture</code> | <code>ObjectDataSource.ParsingCulture = &quot;ParsingCulture.Invariant&quot;</code><br><code>ObjectDataSource.ParsingCulture set = &quot;ParsingCulture.Current&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.SaveViewState</code> | <code>ObjectDataSource.SaveViewState = null</code><br><code>ObjectDataSource.SaveViewState after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;ods…</code> |
| <code>Api.WebControls.ObjectDataSource.Select</code> | <code>ObjectDataSource.Select = &quot;[]&quot;</code><br><code>ObjectDataSource.Select after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;odsProduct…</code> |
| <code>Api.WebControls.ObjectDataSource.SelectCountMethod</code> | <code>ObjectDataSource.SelectCountMethod = &quot;&quot;</code><br><code>ObjectDataSource.SelectCountMethod set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.SelectMethod</code> | <code>ObjectDataSource.SelectMethod = &quot;ToArray&quot;</code><br><code>ObjectDataSource.SelectMethod set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.SelectParameters</code> | <code>ObjectDataSource.SelectParameters = &quot;[]&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.Selected</code> | <code>ObjectDataSource.Selected = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.Selecting</code> | <code>ObjectDataSource.Selecting = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.SortParameterName</code> | <code>ObjectDataSource.SortParameterName = &quot;&quot;</code><br><code>ObjectDataSource.SortParameterName set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.SqlCacheDependency</code> | <code>ObjectDataSource.SqlCacheDependency = &quot;&quot;</code><br><code>ObjectDataSource.SqlCacheDependency set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.StartRowIndexParameterName</code> | <code>ObjectDataSource.StartRowIndexParameterName = &quot;startRowIndex&quot;</code><br><code>ObjectDataSource.StartRowIndexParameterName set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.TrackViewState</code> | <code>ObjectDataSource.TrackViewState = &quot;ok&quot;</code><br><code>ObjectDataSource.TrackViewState after = ObjectDataSource#odsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; ClientID=&quot;od…</code> |
| <code>Api.WebControls.ObjectDataSource.TypeName</code> | <code>ObjectDataSource.TypeName = &quot;System.Collections.Generic.List`1[System.String]&quot;</code><br><code>ObjectDataSource.TypeName set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.Update</code> | <code>ObjectDataSource.Update = &quot;-&gt; System.NotSupportedException: Updating is not supported by ObjectDataSource &#39;odsProducts&#39; unless the UpdateMethod is specified.&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.UpdateMethod</code> | <code>ObjectDataSource.UpdateMethod = &quot;&quot;</code><br><code>ObjectDataSource.UpdateMethod set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.UpdateParameters</code> | <code>ObjectDataSource.UpdateParameters = &quot;[]&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.Updated</code> | <code>ObjectDataSource.Updated = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ObjectDataSource.Updating</code> | <code>ObjectDataSource.Updating = &quot;added and removed&quot;</code> |

## ② HTML 層

プローブサイトの RenderProbe.aspx で、1 ケース 1 リクエスト（ページライフサイクル全体）で描画したページ全体を比較します。下表はコンポーネント部分（div#probe）、設定時の例外、保存された ViewState の要約です。

| ケース | 設定 | 期待される描画 | 設定時の例外 |
|---|---|---|---|
| <code>.(default)</code> | <code>(nothing set)</code> | <code></code> |  |
| <code>.AppRelativeTemplateSourceDirectory=text</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;;</code> | <code></code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;probe&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=markup</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=japanese</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;;</code> | <code></code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;日本語テキスト&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=empty</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&quot;;</code> | <code></code> |  |
| <code>.CacheDuration=0</code> | <code>c.CacheDuration = 0;</code> | <code></code> |  |
| <code>.CacheDuration=3</code> | <code>c.CacheDuration = 3;</code> | <code></code> |  |
| <code>.CacheDuration=-1</code> | <code>c.CacheDuration = -1;</code> | <code></code> | <code>c.CacheDuration = -1; -&gt; System.ArgumentOutOfRangeException: The duration must be non-negative. [param value]</code> |
| <code>.CacheExpirationPolicy=Absolute</code> | <code>c.CacheExpirationPolicy = DataSourceCacheExpiry.Absolute;</code> | <code></code> |  |
| <code>.CacheExpirationPolicy=Sliding</code> | <code>c.CacheExpirationPolicy = DataSourceCacheExpiry.Sliding;</code> | <code></code> |  |
| <code>.CacheKeyDependency=text</code> | <code>c.CacheKeyDependency = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.CacheKeyDependency=markup</code> | <code>c.CacheKeyDependency = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.CacheKeyDependency=japanese</code> | <code>c.CacheKeyDependency = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.CacheKeyDependency=empty</code> | <code>c.CacheKeyDependency = &quot;&quot;;</code> | <code></code> |  |
| <code>.ClientIDMode=Inherit</code> | <code>c.ClientIDMode = ClientIDMode.Inherit;</code> | <code></code> | <code>c.ClientIDMode = ClientIDMode.Inherit; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>.ClientIDMode=AutoID</code> | <code>c.ClientIDMode = ClientIDMode.AutoID;</code> | <code></code> | <code>c.ClientIDMode = ClientIDMode.AutoID; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>.ClientIDMode=Predictable</code> | <code>c.ClientIDMode = ClientIDMode.Predictable;</code> | <code></code> | <code>c.ClientIDMode = ClientIDMode.Predictable; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>.ClientIDMode=Static</code> | <code>c.ClientIDMode = ClientIDMode.Static;</code> | <code></code> | <code>c.ClientIDMode = ClientIDMode.Static; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>.ConflictDetection=OverwriteChanges</code> | <code>c.ConflictDetection = ConflictOptions.OverwriteChanges;</code> | <code></code> |  |
| <code>.ConflictDetection=CompareAllValues</code> | <code>c.ConflictDetection = ConflictOptions.CompareAllValues;</code> | <code></code> |  |
| <code>.ConvertNullToDBNull=true</code> | <code>c.ConvertNullToDBNull = true;</code> | <code></code> |  |
| <code>.ConvertNullToDBNull=false</code> | <code>c.ConvertNullToDBNull = false;</code> | <code></code> |  |
| <code>.DataObjectTypeName=text</code> | <code>c.DataObjectTypeName = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.DataObjectTypeName=markup</code> | <code>c.DataObjectTypeName = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.DataObjectTypeName=japanese</code> | <code>c.DataObjectTypeName = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.DataObjectTypeName=empty</code> | <code>c.DataObjectTypeName = &quot;&quot;;</code> | <code></code> |  |
| <code>.DeleteMethod=text</code> | <code>c.DeleteMethod = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.DeleteMethod=markup</code> | <code>c.DeleteMethod = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.DeleteMethod=japanese</code> | <code>c.DeleteMethod = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.DeleteMethod=empty</code> | <code>c.DeleteMethod = &quot;&quot;;</code> | <code></code> |  |
| <code>.EnableCaching=true</code> | <code>c.EnableCaching = true;</code> | <code></code> |  |
| <code>.EnableCaching=false</code> | <code>c.EnableCaching = false;</code> | <code></code> |  |
| <code>.EnablePaging=true</code> | <code>c.EnablePaging = true;</code> | <code></code> |  |
| <code>.EnablePaging=false</code> | <code>c.EnablePaging = false;</code> | <code></code> |  |
| <code>.EnableTheming=true</code> | <code>c.EnableTheming = true;</code> | <code></code> | <code>c.EnableTheming = true; -&gt; System.NotSupportedException: Control of type &#39;ObjectDataSource&#39; does not support theming.</code> |
| <code>.EnableTheming=false</code> | <code>c.EnableTheming = false;</code> | <code></code> | <code>c.EnableTheming = false; -&gt; System.NotSupportedException: Control of type &#39;ObjectDataSource&#39; does not support theming.</code> |
| <code>.EnableViewState=true</code> | <code>c.EnableViewState = true;</code> | <code></code> |  |
| <code>.EnableViewState=false</code> | <code>c.EnableViewState = false;</code> | <code></code> |  |
| <code>.FilterExpression=text</code> | <code>c.FilterExpression = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.FilterExpression=markup</code> | <code>c.FilterExpression = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.FilterExpression=japanese</code> | <code>c.FilterExpression = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.FilterExpression=empty</code> | <code>c.FilterExpression = &quot;&quot;;</code> | <code></code> |  |
| <code>.ID=text</code> | <code>c.ID = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.ID=markup</code> | <code>c.ID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.ID=japanese</code> | <code>c.ID = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.ID=empty</code> | <code>c.ID = &quot;&quot;;</code> | <code></code> |  |
| <code>.InsertMethod=text</code> | <code>c.InsertMethod = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.InsertMethod=markup</code> | <code>c.InsertMethod = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.InsertMethod=japanese</code> | <code>c.InsertMethod = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.InsertMethod=empty</code> | <code>c.InsertMethod = &quot;&quot;;</code> | <code></code> |  |
| <code>.MaximumRowsParameterName=text</code> | <code>c.MaximumRowsParameterName = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.MaximumRowsParameterName=markup</code> | <code>c.MaximumRowsParameterName = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.MaximumRowsParameterName=japanese</code> | <code>c.MaximumRowsParameterName = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.MaximumRowsParameterName=empty</code> | <code>c.MaximumRowsParameterName = &quot;&quot;;</code> | <code></code> |  |
| <code>.OldValuesParameterFormatString=text</code> | <code>c.OldValuesParameterFormatString = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.OldValuesParameterFormatString=markup</code> | <code>c.OldValuesParameterFormatString = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.OldValuesParameterFormatString=japanese</code> | <code>c.OldValuesParameterFormatString = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.OldValuesParameterFormatString=empty</code> | <code>c.OldValuesParameterFormatString = &quot;&quot;;</code> | <code></code> |  |
| <code>.ParsingCulture=Invariant</code> | <code>c.ParsingCulture = ParsingCulture.Invariant;</code> | <code></code> |  |
| <code>.ParsingCulture=Current</code> | <code>c.ParsingCulture = ParsingCulture.Current;</code> | <code></code> |  |
| <code>.SelectCountMethod=text</code> | <code>c.SelectCountMethod = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.SelectCountMethod=markup</code> | <code>c.SelectCountMethod = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.SelectCountMethod=japanese</code> | <code>c.SelectCountMethod = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.SelectCountMethod=empty</code> | <code>c.SelectCountMethod = &quot;&quot;;</code> | <code></code> |  |
| <code>.SelectMethod=text</code> | <code>c.SelectMethod = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.SelectMethod=markup</code> | <code>c.SelectMethod = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.SelectMethod=japanese</code> | <code>c.SelectMethod = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.SelectMethod=empty</code> | <code>c.SelectMethod = &quot;&quot;;</code> | <code></code> |  |
| <code>.SelectMethod=SelectAll</code> | <code>c.SelectMethod = &quot;SelectAll&quot;;</code> | <code></code> |  |
| <code>.SkinID=text</code> | <code>c.SkinID = &quot;probe&quot;;</code> | <code></code> | <code>c.SkinID = &quot;probe&quot;; -&gt; System.NotSupportedException: Control of type &#39;ObjectDataSource&#39; does not support theming.</code> |
| <code>.SkinID=markup</code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.NotSupportedException: Control of type &#39;ObjectDataSource&#39; does not support theming.</code> |
| <code>.SkinID=japanese</code> | <code>c.SkinID = &quot;日本語テキスト&quot;;</code> | <code></code> | <code>c.SkinID = &quot;日本語テキスト&quot;; -&gt; System.NotSupportedException: Control of type &#39;ObjectDataSource&#39; does not support theming.</code> |
| <code>.SkinID=empty</code> | <code>c.SkinID = &quot;&quot;;</code> | <code></code> | <code>c.SkinID = &quot;&quot;; -&gt; System.NotSupportedException: Control of type &#39;ObjectDataSource&#39; does not support theming.</code> |
| <code>.SortParameterName=text</code> | <code>c.SortParameterName = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.SortParameterName=markup</code> | <code>c.SortParameterName = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.SortParameterName=japanese</code> | <code>c.SortParameterName = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.SortParameterName=empty</code> | <code>c.SortParameterName = &quot;&quot;;</code> | <code></code> |  |
| <code>.SqlCacheDependency=text</code> | <code>c.SqlCacheDependency = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.SqlCacheDependency=markup</code> | <code>c.SqlCacheDependency = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.SqlCacheDependency=japanese</code> | <code>c.SqlCacheDependency = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.SqlCacheDependency=empty</code> | <code>c.SqlCacheDependency = &quot;&quot;;</code> | <code></code> |  |
| <code>.StartRowIndexParameterName=text</code> | <code>c.StartRowIndexParameterName = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.StartRowIndexParameterName=markup</code> | <code>c.StartRowIndexParameterName = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.StartRowIndexParameterName=japanese</code> | <code>c.StartRowIndexParameterName = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.StartRowIndexParameterName=empty</code> | <code>c.StartRowIndexParameterName = &quot;&quot;;</code> | <code></code> |  |
| <code>.TypeName=text</code> | <code>c.TypeName = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.TypeName=markup</code> | <code>c.TypeName = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.TypeName=japanese</code> | <code>c.TypeName = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.TypeName=empty</code> | <code>c.TypeName = &quot;&quot;;</code> | <code></code> |  |
| <code>.TypeName=store</code> | <code>c.TypeName = &quot;WebControlsProbe.ProductStore&quot;;</code> | <code></code> |  |
| <code>.TypeName=missing</code> | <code>c.TypeName = &quot;WebControlsProbe.NoSuchType&quot;;</code> | <code></code> |  |
| <code>.UpdateMethod=text</code> | <code>c.UpdateMethod = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.UpdateMethod=markup</code> | <code>c.UpdateMethod = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.UpdateMethod=japanese</code> | <code>c.UpdateMethod = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.UpdateMethod=empty</code> | <code>c.UpdateMethod = &quot;&quot;;</code> | <code></code> |  |
| <code>.ValidateRequestMode=Inherit</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Inherit;</code> | <code></code> |  |
| <code>.ValidateRequestMode=Disabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Disabled;</code> | <code></code> |  |
| <code>.ValidateRequestMode=Enabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Enabled;</code> | <code></code> |  |
| <code>.ViewStateMode=Inherit</code> | <code>c.ViewStateMode = ViewStateMode.Inherit;</code> | <code></code> |  |
| <code>.ViewStateMode=Enabled</code> | <code>c.ViewStateMode = ViewStateMode.Enabled;</code> | <code></code> |  |
| <code>.ViewStateMode=Disabled</code> | <code>c.ViewStateMode = ViewStateMode.Disabled;</code> | <code></code> |  |
| <code>.Visible=true</code> | <code>c.Visible = true;</code> | <code></code> | <code>c.Visible = true; -&gt; System.NotSupportedException: Control of type &#39;ObjectDataSource&#39; is a non-visual control and does not support setting the Visible property.</code> |
| <code>.Visible=false</code> | <code>c.Visible = false;</code> | <code></code> | <code>c.Visible = false; -&gt; System.NotSupportedException: Control of type &#39;ObjectDataSource&#39; is a non-visual control and does not support setting the Visible property.</code> |
| <code>.EnablePaging=true+SelectCountMethod=text</code> | <code>c.EnablePaging = true; c.SelectCountMethod = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.EnableCaching=true+CacheDuration=3</code> | <code>c.EnableCaching = true; c.CacheDuration = 3;</code> | <code></code> |  |
| <code>.TypeName=missing+SelectMethod=SelectAll</code> | <code>c.TypeName = &quot;WebControlsProbe.NoSuchType&quot;; c.SelectMethod = &quot;SelectAll&quot;;</code> | <code></code> |  |
| <code>.ConflictDetection=CompareAllValues+OldValuesParameterFormatString=text</code> | <code>c.ConflictDetection = ConflictOptions.CompareAllValues; c.OldValuesParameterFormatString = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.Visible=false+EnableViewState=false</code> | <code>c.Visible = false; c.EnableViewState = false;</code> | <code></code> | <code>c.Visible = false; -&gt; System.NotSupportedException: Control of type &#39;ObjectDataSource&#39; is a non-visual control and does not support setting the Visible property.</code> |
| <code>[naming container].(default)</code> | <code>[naming container] (nothing set)</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=AutoID</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> | <code>c.ClientIDMode = ClientIDMode.AutoID; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>[naming container].ClientIDMode=Static</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> | <code>c.ClientIDMode = ClientIDMode.Static; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>[naming container].ClientIDMode=Predictable</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> | <code>c.ClientIDMode = ClientIDMode.Predictable; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>[naming container].ClientIDMode=Inherit</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> | <code>c.ClientIDMode = ClientIDMode.Inherit; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>[naming container].ID=text</code> | <code>[naming container] c.ID = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |

## ③ シナリオ層

ブラウザ（Playwright）で ObjectDataSourceScenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。

| # | 操作 | 対象 | 値 |
|---|---|---|---|
| 1 | goto | /DataSourceScenario.aspx?reset=1 | <code></code> |
| 2 | snapshot |  | **ObjectDataSource [render][databind][dynamic] first request: grids bound to each data source, the Access data source selected in code, ones made in code** |
| 3 | clicktextin | gvObject | <code>Edit</code> |
| 4 | evaluate | gvObject | <code>const inputs = e.querySelectorAll(&quot;input[type=text]&quot;); inputs[0].value = &quot;Pen (edited)&quot;;</code> |
| 5 | clicktextin | gvObject | <code>Update</code> |
| 6 | snapshot |  | **ObjectDataSource [viewstate] an object data source&#39;s row updated** |
| 7 | clicktextin | gvObject | <code>Delete</code> |
| 8 | snapshot |  | **ObjectDataSource [viewstate] an object data source&#39;s row deleted** |
| 9 | clicktextin | gvSql | <code>Name</code> |
| 10 | snapshot |  | **ObjectDataSource [viewstate] a SQL data source sorted** |
| 11 | clicktextin | gvSql | <code>Edit</code> |
| 12 | evaluate | gvSql | <code>const inputs = e.querySelectorAll(&quot;input[type=text]&quot;); inputs[0].value = &quot;Bag (edited)&quot;; inputs[1].value = &quot;25.5&quot;;</code> |
| 13 | clicktextin | gvSql | <code>Update</code> |
| 14 | snapshot |  | **ObjectDataSource [viewstate] a SQL data source&#39;s row updated (its update command&#39;s parameters)** |
| 15 | clicktextin | gvSql | <code>Delete</code> |
| 16 | snapshot |  | **ObjectDataSource [viewstate] a SQL data source&#39;s row deleted** |
| 17 | evaluate | dvSqlInsert | <code>const inputs = e.querySelectorAll(&quot;input[type=text]&quot;); inputs[0].value = &quot;Z9&quot;; inputs[1].value = &quot;Zipper&quot;; inputs[2].value = &quot;9.5&quot;;</code> |
| 18 | clicktextin | dvSqlInsert | <code>Insert</code> |
| 19 | snapshot |  | **ObjectDataSource [viewstate] a SQL data source&#39;s row inserted** |
| 20 | clicktextin | gvLinq | <code>2</code> |
| 21 | snapshot |  | **ObjectDataSource [viewstate] a LINQ data source paged** |
| 22 | clicktextin | gvLinq | <code>Price</code> |
| 23 | snapshot |  | **ObjectDataSource [viewstate] a LINQ data source sorted** |
| 24 | fill | txtMinimum | <code>100</code> |
| 25 | click | btnSubmit | <code></code> |
| 26 | snapshot |  | **ObjectDataSource [viewstate] a LINQ data source&#39;s where parameter from a text box** |
| 27 | fill | txtSearch | <code>a</code> |
| 28 | click | btnSubmit | <code></code> |
| 29 | snapshot |  | **ObjectDataSource [viewstate] a query extender&#39;s search from a text box** |
| 30 | click | btnModel | <code></code> |
| 31 | snapshot |  | **ObjectDataSource [viewstate] a model error shown** |
| 32 | fill | txtModelName | <code>named</code> |
| 33 | click | btnModel | <code></code> |
| 34 | snapshot |  | **ObjectDataSource [viewstate] no model error** |
| 35 | clicktextin | gvDynamic | <code>Name</code> |
| 36 | snapshot |  | **ObjectDataSource [dynamic][viewstate] a grid sorted by a data source made in code** |
| 37 | click | btnShow | <code></code> |
| 38 | snapshot |  | **ObjectDataSource [visible][viewstate] a hidden grid bound to a data source shown** |

<details><summary>ObjectDataSource [render][databind][dynamic] first request: grids bound to each data source, the Access data source selected in code, ones made in code（期待される本文）</summary>

```text
Code Name Price
Edit Delete P1 Pen 1.25
Edit Delete B2 Bag 24.99
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Code Name Price
Edit Delete B2 Bag 24.99
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Edit Delete P1 Pen 1.25
Code
Name
Price
Insert Cancel
code name price
P1 Pen 1.25
B2 Bag 24.99
C3 Cup <&> 1234.5
Minimum price:
Code Name Price
B2 Bag 24.99
C3 Cup <&> 1234.5
D4 Desk 150
1 2
Search:
Code Name
D4 Desk
E5 Eraser
F6 File
G7 Glue
P1 Pen
Model state:
Dynamic:
Name Code Price Active Image
Pen P1 1.25 ~/Images/probe.png
Bag B2 24.99 ~/Images/probe.png
Cup <&> C3 1234.5 ~/Images/probe.png
Desk D4 150 ~/Images/probe.png
Eraser E5 0.5 ~/Images/probe.png
File F6 3 ~/Images/probe.png
Glue G7 2.75 ~/Images/probe.png
code name price
B2 Bag 24.99
C3 Cup <&> 1234.5
Request 1: IsPostBack=False
ObjectDataSource Selecting ObjectDataSourceView ExecutingSelectCount=False
ObjectDataSource Selected ObjectDataSourceView rows=7
SqlDataSource Selecting select SortExpression=
SqlDataSource Selected rows=7
SqlDataSource Selecting select SortExpression=
SqlDataSource Selected rows=7
LinqDataSource Selecting Minimum=2
ObjectDataSource Selected ObjectDataSourceView rows=7
AccessDataSource: no OLE DB provider for Access files
State P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>ObjectDataSource [viewstate] an object data source&#39;s row updated（期待される本文）</summary>

```text
Code Name Price
Edit Delete P1 Pen (edited) 1.25
Edit Delete B2 Bag 24.99
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Code Name Price
Edit Delete B2 Bag 24.99
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Edit Delete P1 Pen 1.25
Code
Name
Price
Insert Cancel
code name price
P1 Pen 1.25
B2 Bag 24.99
C3 Cup <&> 1234.5
Minimum price:
Code Name Price
B2 Bag 24.99
C3 Cup <&> 1234.5
D4 Desk 150
1 2
Search:
Code Name
D4 Desk
E5 Eraser
F6 File
G7 Glue
P1 Pen
Model state:
Dynamic:
Name Code Price Active Image
Pen P1 1.25 ~/Images/probe.png
Bag B2 24.99 ~/Images/probe.png
Cup <&> C3 1234.5 ~/Images/probe.png
Desk D4 150 ~/Images/probe.png
Eraser E5 0.5 ~/Images/probe.png
File F6 3 ~/Images/probe.png
Glue G7 2.75 ~/Images/probe.png
code name price
B2 Bag 24.99
C3 Cup <&> 1234.5
Request 3: IsPostBack=True
ObjectDataSource Updating Code=P1, Name=Pen (edited), Price=1.25
ObjectDataSource Updated
State P1:Pen (edited):1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>ObjectDataSource [viewstate] an object data source&#39;s row deleted（期待される本文）</summary>

```text
Code Name Price
Edit Delete B2 Bag 24.99
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Code Name Price
Edit Delete B2 Bag 24.99
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Edit Delete P1 Pen 1.25
Code
Name
Price
Insert Cancel
code name price
P1 Pen 1.25
B2 Bag 24.99
C3 Cup <&> 1234.5
Minimum price:
Code Name Price
B2 Bag 24.99
C3 Cup <&> 1234.5
D4 Desk 150
1 2
Search:
Code Name
D4 Desk
E5 Eraser
F6 File
G7 Glue
P1 Pen
Model state:
Dynamic:
Name Code Price Active Image
Pen P1 1.25 ~/Images/probe.png
Bag B2 24.99 ~/Images/probe.png
Cup <&> C3 1234.5 ~/Images/probe.png
Desk D4 150 ~/Images/probe.png
Eraser E5 0.5 ~/Images/probe.png
File F6 3 ~/Images/probe.png
Glue G7 2.75 ~/Images/probe.png
code name price
B2 Bag 24.99
C3 Cup <&> 1234.5
Request 4: IsPostBack=True
ObjectDataSource Deleted
State B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>ObjectDataSource [viewstate] a SQL data source sorted（期待される本文）</summary>

```text
Code Name Price
Edit Delete B2 Bag 24.99
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Code Name Price
Edit Delete B2 Bag 24.99
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Edit Delete P1 Pen 1.25
Code
Name
Price
Insert Cancel
code name price
P1 Pen 1.25
B2 Bag 24.99
C3 Cup <&> 1234.5
Minimum price:
Code Name Price
B2 Bag 24.99
C3 Cup <&> 1234.5
D4 Desk 150
1 2
Search:
Code Name
D4 Desk
E5 Eraser
F6 File
G7 Glue
P1 Pen
Model state:
Dynamic:
Name Code Price Active Image
Pen P1 1.25 ~/Images/probe.png
Bag B2 24.99 ~/Images/probe.png
Cup <&> C3 1234.5 ~/Images/probe.png
Desk D4 150 ~/Images/probe.png
Eraser E5 0.5 ~/Images/probe.png
File F6 3 ~/Images/probe.png
Glue G7 2.75 ~/Images/probe.png
code name price
B2 Bag 24.99
C3 Cup <&> 1234.5
Request 5: IsPostBack=True
State B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>ObjectDataSource [viewstate] a SQL data source&#39;s row updated (its update command&#39;s parameters)（期待される本文）</summary>

```text
Code Name Price
Edit Delete B2 Bag 24.99
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Code Name Price
Edit Delete B2 Bag (edited) 25.5
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Edit Delete P1 Pen 1.25
Code
Name
Price
Insert Cancel
code name price
P1 Pen 1.25
B2 Bag 24.99
C3 Cup <&> 1234.5
Minimum price:
Code Name Price
B2 Bag 24.99
C3 Cup <&> 1234.5
D4 Desk 150
1 2
Search:
Code Name
D4 Desk
E5 Eraser
F6 File
G7 Glue
P1 Pen
Model state:
Dynamic:
Name Code Price Active Image
Pen P1 1.25 ~/Images/probe.png
Bag B2 24.99 ~/Images/probe.png
Cup <&> C3 1234.5 ~/Images/probe.png
Desk D4 150 ~/Images/probe.png
Eraser E5 0.5 ~/Images/probe.png
File F6 3 ~/Images/probe.png
Glue G7 2.75 ~/Images/probe.png
code name price
B2 Bag 24.99
C3 Cup <&> 1234.5
Request 7: IsPostBack=True
SqlDataSource Updated update (Name=Bag (edited), Price=25.5, Code=B2) rows=1
State B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>ObjectDataSource [viewstate] a SQL data source&#39;s row deleted（期待される本文）</summary>

```text
Code Name Price
Edit Delete B2 Bag 24.99
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Code Name Price
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Edit Delete P1 Pen 1.25
Code
Name
Price
Insert Cancel
code name price
P1 Pen 1.25
B2 Bag 24.99
C3 Cup <&> 1234.5
Minimum price:
Code Name Price
B2 Bag 24.99
C3 Cup <&> 1234.5
D4 Desk 150
1 2
Search:
Code Name
D4 Desk
E5 Eraser
F6 File
G7 Glue
P1 Pen
Model state:
Dynamic:
Name Code Price Active Image
Pen P1 1.25 ~/Images/probe.png
Bag B2 24.99 ~/Images/probe.png
Cup <&> C3 1234.5 ~/Images/probe.png
Desk D4 150 ~/Images/probe.png
Eraser E5 0.5 ~/Images/probe.png
File F6 3 ~/Images/probe.png
Glue G7 2.75 ~/Images/probe.png
code name price
B2 Bag 24.99
C3 Cup <&> 1234.5
Request 8: IsPostBack=True
SqlDataSource Deleted delete (Code=B2) rows=1
State B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>ObjectDataSource [viewstate] a SQL data source&#39;s row inserted（期待される本文）</summary>

```text
Code Name Price
Edit Delete B2 Bag 24.99
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Code Name Price
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Edit Delete P1 Pen 1.25
Edit Delete Z9 Zipper 9.5
Code
Name
Price
Insert Cancel
code name price
P1 Pen 1.25
B2 Bag 24.99
C3 Cup <&> 1234.5
Minimum price:
Code Name Price
B2 Bag 24.99
C3 Cup <&> 1234.5
D4 Desk 150
1 2
Search:
Code Name
D4 Desk
E5 Eraser
F6 File
G7 Glue
P1 Pen
Model state:
Dynamic:
Name Code Price Active Image
Pen P1 1.25 ~/Images/probe.png
Bag B2 24.99 ~/Images/probe.png
Cup <&> C3 1234.5 ~/Images/probe.png
Desk D4 150 ~/Images/probe.png
Eraser E5 0.5 ~/Images/probe.png
File F6 3 ~/Images/probe.png
Glue G7 2.75 ~/Images/probe.png
code name price
B2 Bag 24.99
C3 Cup <&> 1234.5
Request 9: IsPostBack=True
SqlDataSource Inserted insert (Code=Z9, Name=Zipper, Price=9.5) rows=1
State B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>ObjectDataSource [viewstate] a LINQ data source paged（期待される本文）</summary>

```text
Code Name Price
Edit Delete B2 Bag 24.99
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Code Name Price
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Edit Delete P1 Pen 1.25
Edit Delete Z9 Zipper 9.5
Code
Name
Price
Insert Cancel
code name price
P1 Pen 1.25
B2 Bag 24.99
C3 Cup <&> 1234.5
Minimum price:
Code Name Price
F6 File 3
G7 Glue 2.75
1 2
Search:
Code Name
D4 Desk
E5 Eraser
F6 File
G7 Glue
P1 Pen
Model state:
Dynamic:
Name Code Price Active Image
Pen P1 1.25 ~/Images/probe.png
Bag B2 24.99 ~/Images/probe.png
Cup <&> C3 1234.5 ~/Images/probe.png
Desk D4 150 ~/Images/probe.png
Eraser E5 0.5 ~/Images/probe.png
File F6 3 ~/Images/probe.png
Glue G7 2.75 ~/Images/probe.png
code name price
B2 Bag 24.99
C3 Cup <&> 1234.5
Request 10: IsPostBack=True
State B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>ObjectDataSource [viewstate] a LINQ data source sorted（期待される本文）</summary>

```text
Code Name Price
Edit Delete B2 Bag 24.99
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Code Name Price
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Edit Delete P1 Pen 1.25
Edit Delete Z9 Zipper 9.5
Code
Name
Price
Insert Cancel
code name price
P1 Pen 1.25
B2 Bag 24.99
C3 Cup <&> 1234.5
Minimum price:
Code Name Price
G7 Glue 2.75
F6 File 3
B2 Bag 24.99
1 2
Search:
Code Name
D4 Desk
E5 Eraser
F6 File
G7 Glue
P1 Pen
Model state:
Dynamic:
Name Code Price Active Image
Pen P1 1.25 ~/Images/probe.png
Bag B2 24.99 ~/Images/probe.png
Cup <&> C3 1234.5 ~/Images/probe.png
Desk D4 150 ~/Images/probe.png
Eraser E5 0.5 ~/Images/probe.png
File F6 3 ~/Images/probe.png
Glue G7 2.75 ~/Images/probe.png
code name price
B2 Bag 24.99
C3 Cup <&> 1234.5
Request 11: IsPostBack=True
State B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>ObjectDataSource [viewstate] a LINQ data source&#39;s where parameter from a text box（期待される本文）</summary>

```text
Code Name Price
Edit Delete B2 Bag 24.99
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Code Name Price
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Edit Delete P1 Pen 1.25
Edit Delete Z9 Zipper 9.5
Code
Name
Price
Insert Cancel
code name price
P1 Pen 1.25
B2 Bag 24.99
C3 Cup <&> 1234.5
Minimum price:
Code Name Price
D4 Desk 150
C3 Cup <&> 1234.5
Search:
Code Name
D4 Desk
E5 Eraser
F6 File
G7 Glue
P1 Pen
Model state:
Dynamic:
Name Code Price Active Image
Pen P1 1.25 ~/Images/probe.png
Bag B2 24.99 ~/Images/probe.png
Cup <&> C3 1234.5 ~/Images/probe.png
Desk D4 150 ~/Images/probe.png
Eraser E5 0.5 ~/Images/probe.png
File F6 3 ~/Images/probe.png
Glue G7 2.75 ~/Images/probe.png
code name price
B2 Bag 24.99
C3 Cup <&> 1234.5
Request 12: IsPostBack=True
State B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>ObjectDataSource [viewstate] a query extender&#39;s search from a text box（期待される本文）</summary>

```text
Code Name Price
Edit Delete B2 Bag 24.99
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Code Name Price
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Edit Delete P1 Pen 1.25
Edit Delete Z9 Zipper 9.5
Code
Name
Price
Insert Cancel
code name price
P1 Pen 1.25
B2 Bag 24.99
C3 Cup <&> 1234.5
Minimum price:
Code Name Price
D4 Desk 150
C3 Cup <&> 1234.5
Search:
Code Name
B2 Bag
E5 Eraser
Model state:
Dynamic:
Name Code Price Active Image
Pen P1 1.25 ~/Images/probe.png
Bag B2 24.99 ~/Images/probe.png
Cup <&> C3 1234.5 ~/Images/probe.png
Desk D4 150 ~/Images/probe.png
Eraser E5 0.5 ~/Images/probe.png
File F6 3 ~/Images/probe.png
Glue G7 2.75 ~/Images/probe.png
code name price
B2 Bag 24.99
C3 Cup <&> 1234.5
Request 13: IsPostBack=True
State B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>ObjectDataSource [viewstate] a model error shown（期待される本文）</summary>

```text
Code Name Price
Edit Delete B2 Bag 24.99
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Code Name Price
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Edit Delete P1 Pen 1.25
Edit Delete Z9 Zipper 9.5
Code
Name
Price
Insert Cancel
code name price
P1 Pen 1.25
B2 Bag 24.99
C3 Cup <&> 1234.5
Minimum price:
Code Name Price
D4 Desk 150
C3 Cup <&> 1234.5
Search:
Code Name
B2 Bag
E5 Eraser
Model state: The name <b>is</b> required. The name <b>is</b> required.
Dynamic:
Name Code Price Active Image
Pen P1 1.25 ~/Images/probe.png
Bag B2 24.99 ~/Images/probe.png
Cup <&> C3 1234.5 ~/Images/probe.png
Desk D4 150 ~/Images/probe.png
Eraser E5 0.5 ~/Images/probe.png
File F6 3 ~/Images/probe.png
Glue G7 2.75 ~/Images/probe.png
code name price
B2 Bag 24.99
C3 Cup <&> 1234.5
Request 14: IsPostBack=True
Model valid=False
State B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>ObjectDataSource [viewstate] no model error（期待される本文）</summary>

```text
Code Name Price
Edit Delete B2 Bag 24.99
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Code Name Price
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Edit Delete P1 Pen 1.25
Edit Delete Z9 Zipper 9.5
Code
Name
Price
Insert Cancel
code name price
P1 Pen 1.25
B2 Bag 24.99
C3 Cup <&> 1234.5
Minimum price:
Code Name Price
D4 Desk 150
C3 Cup <&> 1234.5
Search:
Code Name
B2 Bag
E5 Eraser
Model state:
Dynamic:
Name Code Price Active Image
Pen P1 1.25 ~/Images/probe.png
Bag B2 24.99 ~/Images/probe.png
Cup <&> C3 1234.5 ~/Images/probe.png
Desk D4 150 ~/Images/probe.png
Eraser E5 0.5 ~/Images/probe.png
File F6 3 ~/Images/probe.png
Glue G7 2.75 ~/Images/probe.png
code name price
B2 Bag 24.99
C3 Cup <&> 1234.5
Request 15: IsPostBack=True
Model valid=True
State B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>ObjectDataSource [dynamic][viewstate] a grid sorted by a data source made in code（期待される本文）</summary>

```text
Code Name Price
Edit Delete B2 Bag 24.99
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Code Name Price
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Edit Delete P1 Pen 1.25
Edit Delete Z9 Zipper 9.5
Code
Name
Price
Insert Cancel
code name price
P1 Pen 1.25
B2 Bag 24.99
C3 Cup <&> 1234.5
Minimum price:
Code Name Price
D4 Desk 150
C3 Cup <&> 1234.5
Search:
Code Name
B2 Bag
E5 Eraser
Model state:
Dynamic:
Name Code Price Active Image
Bag B2 24.99 ~/Images/probe.png
Cup <&> C3 1234.5 ~/Images/probe.png
Desk D4 150 ~/Images/probe.png
Eraser E5 0.5 ~/Images/probe.png
File F6 3 ~/Images/probe.png
Glue G7 2.75 ~/Images/probe.png
code name price
B2 Bag 24.99
C3 Cup <&> 1234.5
Request 16: IsPostBack=True
State B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>ObjectDataSource [visible][viewstate] a hidden grid bound to a data source shown（期待される本文）</summary>

```text
Code Name Price
Edit Delete B2 Bag 24.99
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Code Name Price
Edit Delete C3 Cup <&> 1234.5
Edit Delete D4 Desk 150
Edit Delete E5 Eraser 0.5
Edit Delete F6 File 3
Edit Delete G7 Glue 2.75
Edit Delete P1 Pen 1.25
Edit Delete Z9 Zipper 9.5
Code
Name
Price
Insert Cancel
code name price
P1 Pen 1.25
B2 Bag 24.99
C3 Cup <&> 1234.5
code name price
P1 Pen 1.25
B2 Bag 24.99
C3 Cup <&> 1234.5
Minimum price:
Code Name Price
D4 Desk 150
C3 Cup <&> 1234.5
Search:
Code Name
B2 Bag
E5 Eraser
Model state:
Dynamic:
Name Code Price Active Image
Bag B2 24.99 ~/Images/probe.png
Cup <&> C3 1234.5 ~/Images/probe.png
Desk D4 150 ~/Images/probe.png
Eraser E5 0.5 ~/Images/probe.png
File F6 3 ~/Images/probe.png
Glue G7 2.75 ~/Images/probe.png
code name price
B2 Bag 24.99
C3 Cup <&> 1234.5
Request 17: IsPostBack=True
Click btnShow
State B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

