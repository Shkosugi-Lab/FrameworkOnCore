# SqlDataSource テストケース一覧

変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。
このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。

- 正解データの採取: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0
- ① API 層: 171 ケース（180 メンバー）
- ② HTML 層: 107 ケース
- ③ シナリオ層: 15 スナップショット

## 機構マトリクス

型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。

| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |
|---|---|---|---|
| プロパティ → HTML 描画 | すべてのコントロール | 100 | 1 |
| ViewState の保存と復元 | Control | 6 | 14 |
| Visible=false（描画されず状態は保持） | Control.Visible | 2 | 1 |
| 名前付けコンテナと ClientIDMode | Control.ClientIDMode | 6 | 0 |
| 動的に生成したコントロールのライフサイクル | Control | 0 | 2 |
| データバインド（DataBind / <%# %>） | Control.DataBind | 0 | 1 |

該当しない機構: テーマとスキン（SkinID / EnableTheming）、Enabled=false・Disabled=true（描画とポスト値の扱い）、ポストバックデータの取り込み、変更イベント（*Changed、ServerChange）、ReadOnly（ポストされた値を無視）、リクエスト検証（ValidateRequestMode）、ポストバックイベント、コマンドイベントと親へのバブル（CommandName）、送信ボタン／スクリプトによるポストバック（UseSubmitBehavior）、ブラウザ側のクリック処理（OnClientClick）、クロスページポストバック（PostBackUrl）、既定のボタン（Panel の DefaultButton）、AutoPostBack（クライアントスクリプトによるポストバック）、CausesValidation / ValidationGroup、検証対象（サーバー側の検証）、検証対象（ブラウザ側の検証）、双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値）、項目（ListItem の Selected・Enabled・Text・Value と ViewState）、複数選択、繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）、ポスト値のイベント検証（一覧にない値・無効な項目の拒否）、サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）、ブラウザ側の検証スクリプト（EnableClientScript）、検証結果のまとめ（ValidationSummary）、ページング、並べ替え、項目の挿入、行・項目の編集（編集・更新・取り消し・削除）、行・項目の選択、行・項目のコマンド（RowCommand・ItemCommand へのバブル）、データキー（DataKeyNames・DataKeys）、データソースコントロールとの連携（DataSourceID）、テンプレート、データソース連携（DataSource・DataTextField など）、クライアントコールバック

## ① API 層

SqlDataSource とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の SqlDataSource インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。

| ケース | 期待値（.NET Framework） |
|---|---|
| <code>Api.Control.#ctor</code> | <code>Control.#ctor = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMode=In…</code> |
| <code>Api.Control.Adapter</code> | <code>Control.Adapter = null</code> |
| <code>Api.Control.AddParsedSubObject</code> | <code>Control.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>Control.AddParsedSubObject(Object) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelect…</code> |
| <code>Api.Control.AddedControl</code> | <code>Control.AddedControl(Control,Int32) = &quot;ok&quot;</code><br><code>Control.AddedControl(Control,Int32) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelec…</code> |
| <code>Api.Control.AppRelativeTemplateSourceDirectory</code> | <code>Control.AppRelativeTemplateSourceDirectory = &quot;&quot;</code><br><code>Control.AppRelativeTemplateSourceDirectory set = &quot;-&gt; System.ArgumentException: The relative virtual path &#39;Abc 123&#39; is not allowed here.&quot;</code> |
| <code>Api.Control.ApplyStyleSheetSkin</code> | <code>Control.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>Control.ApplyStyleSheetSkin(Page) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectO…</code> |
| <code>Api.Control.BeginRenderTracing</code> | <code>Control.BeginRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.BeginRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.BeginRenderTracing(TextWriter,Object) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; C…</code> |
| <code>Api.Control.BindingContainer</code> | <code>Control.BindingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Clien…</code> |
| <code>Api.Control.BuildProfileTree</code> | <code>Control.BuildProfileTree(String,Boolean) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.ChildControlsCreated</code> | <code>Control.ChildControlsCreated = &quot;false&quot;</code><br><code>Control.ChildControlsCreated set = &quot;ok&quot;</code> |
| <code>Api.Control.ClearCachedClientID</code> | <code>Control.ClearCachedClientID = &quot;ok&quot;</code><br><code>Control.ClearCachedClientID after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullP…</code> |
| <code>Api.Control.ClearChildControlState</code> | <code>Control.ClearChildControlState = &quot;ok&quot;</code><br><code>Control.ClearChildControlState after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNu…</code> |
| <code>Api.Control.ClearChildState</code> | <code>Control.ClearChildState = &quot;ok&quot;</code><br><code>Control.ClearChildState after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullParam…</code> |
| <code>Api.Control.ClearChildViewState</code> | <code>Control.ClearChildViewState = &quot;ok&quot;</code><br><code>Control.ClearChildViewState after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullP…</code> |
| <code>Api.Control.ClearEffectiveClientIDMode</code> | <code>Control.ClearEffectiveClientIDMode = &quot;ok&quot;</code><br><code>Control.ClearEffectiveClientIDMode after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelect…</code> |
| <code>Api.Control.ClientID</code> | <code>Control.ClientID = &quot;sdsProducts&quot;</code> |
| <code>Api.Control.ClientIDMode</code> | <code>Control.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>Control.ClientIDMode set = &quot;-&gt; System.NotSupportedException: Specified method is not supported.&quot;</code> |
| <code>Api.Control.ClientIDSeparator</code> | <code>Control.ClientIDSeparator = &quot;&#39;_&#39;&quot;</code> |
| <code>Api.Control.Context</code> | <code>Control.Context = null</code> |
| <code>Api.Control.Controls</code> | <code>Control.Controls = ControlCollection []</code> |
| <code>Api.Control.CreateChildControls</code> | <code>Control.CreateChildControls = &quot;ok&quot;</code><br><code>Control.CreateChildControls after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullP…</code> |
| <code>Api.Control.CreateControlCollection</code> | <code>Control.CreateControlCollection = ControlCollection []</code><br><code>Control.CreateControlCollection after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnN…</code> |
| <code>Api.Control.DataBind</code> | <code>Control.DataBind = &quot;ok&quot;</code><br><code>Control.DataBind after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullParameter=tr…</code><br><code>Control.DataBind(Boolean) = &quot;ok&quot;</code><br><code>Control.DataBind(Boolean) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullPar…</code> |
| <code>Api.Control.DataBindChildren</code> | <code>Control.DataBindChildren = &quot;ok&quot;</code><br><code>Control.DataBindChildren after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullPara…</code> |
| <code>Api.Control.DataBinding</code> | <code>Control.DataBinding = &quot;added and removed&quot;</code> |
| <code>Api.Control.DataItemContainer</code> | <code>Control.DataItemContainer = null</code> |
| <code>Api.Control.DataKeysContainer</code> | <code>Control.DataKeysContainer = null</code> |
| <code>Api.Control.DesignMode</code> | <code>Control.DesignMode = &quot;false&quot;</code> |
| <code>Api.Control.Dispose</code> | <code>Control.Dispose = &quot;ok&quot;</code><br><code>Control.Dispose after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullParameter=tru…</code> |
| <code>Api.Control.Disposed</code> | <code>Control.Disposed = &quot;added and removed&quot;</code> |
| <code>Api.Control.EnableTheming</code> | <code>Control.EnableTheming = &quot;false&quot;</code><br><code>Control.EnableTheming set = &quot;-&gt; System.NotSupportedException: Control of type &#39;SqlDataSource&#39; does not support theming.&quot;</code> |
| <code>Api.Control.EnableViewState</code> | <code>Control.EnableViewState = &quot;true&quot;</code><br><code>Control.EnableViewState set = &quot;true&quot;</code> |
| <code>Api.Control.EndRenderTracing</code> | <code>Control.EndRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.EndRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.EndRenderTracing(TextWriter,Object) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; Can…</code> |
| <code>Api.Control.EnsureChildControls</code> | <code>Control.EnsureChildControls = &quot;ok&quot;</code><br><code>Control.EnsureChildControls after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullP…</code> |
| <code>Api.Control.EnsureID</code> | <code>Control.EnsureID = &quot;ok&quot;</code><br><code>Control.EnsureID after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullParameter=tr…</code> |
| <code>Api.Control.Events</code> | <code>Control.Events = &quot;System.ComponentModel.EventHandlerList&quot;</code> |
| <code>Api.Control.FindControl</code> | <code>Control.FindControl(String) = Label#lblTitle {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code><br><code>Control.FindControl(String) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullP…</code><br><code>Control.FindControl(String,Int32) = null</code><br><code>Control.FindControl(String,Int32) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectO…</code> |
| <code>Api.Control.Focus</code> | <code>Control.Focus = &quot;-&gt; System.NotSupportedException: Control of type &#39;SqlDataSource&#39; does not support the Focus operation.&quot;</code> |
| <code>Api.Control.GetDesignModeState</code> | <code>Control.GetDesignModeState = &quot;[]&quot;</code><br><code>Control.GetDesignModeState after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullPa…</code> |
| <code>Api.Control.GetRouteUrl</code> | <code>Control.GetRouteUrl(Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.GetUniqueIDRelativeTo</code> | <code>Control.GetUniqueIDRelativeTo(Control) = &quot;sdsProducts&quot;</code><br><code>Control.GetUniqueIDRelativeTo(Control) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSe…</code> |
| <code>Api.Control.HasChildViewState</code> | <code>Control.HasChildViewState = &quot;false&quot;</code> |
| <code>Api.Control.HasControls</code> | <code>Control.HasControls = &quot;false&quot;</code><br><code>Control.HasControls after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullParameter…</code> |
| <code>Api.Control.HasEvents</code> | <code>Control.HasEvents = &quot;true&quot;</code><br><code>Control.HasEvents after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullParameter=t…</code> |
| <code>Api.Control.ID</code> | <code>Control.ID = &quot;sdsProducts&quot;</code><br><code>Control.ID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.IdSeparator</code> | <code>Control.IdSeparator = &quot;&#39;$&#39;&quot;</code> |
| <code>Api.Control.Init</code> | <code>Control.Init = &quot;added and removed&quot;</code> |
| <code>Api.Control.IsChildControlStateCleared</code> | <code>Control.IsChildControlStateCleared = &quot;false&quot;</code> |
| <code>Api.Control.IsLiteralContent</code> | <code>Control.IsLiteralContent = &quot;false&quot;</code><br><code>Control.IsLiteralContent after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullPara…</code> |
| <code>Api.Control.IsTrackingViewState</code> | <code>Control.IsTrackingViewState = &quot;false&quot;</code> |
| <code>Api.Control.IsViewStateEnabled</code> | <code>Control.IsViewStateEnabled = &quot;true&quot;</code> |
| <code>Api.Control.Load</code> | <code>Control.Load = &quot;added and removed&quot;</code> |
| <code>Api.Control.LoadControlState</code> | <code>Control.LoadControlState(Object) = &quot;ok&quot;</code><br><code>Control.LoadControlState(Object) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOn…</code> |
| <code>Api.Control.LoadViewState</code> | <code>Control.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Web.UI.Pair&#39;.&quot;</code> |
| <code>Api.Control.LoadViewStateByID</code> | <code>Control.LoadViewStateByID = &quot;false&quot;</code> |
| <code>Api.Control.MapPathSecure</code> | <code>Control.MapPathSecure(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.NamingContainer</code> | <code>Control.NamingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code> |
| <code>Api.Control.OnBubbleEvent</code> | <code>Control.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Control.OnBubbleEvent(Object,EventArgs) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelS…</code> |
| <code>Api.Control.OnDataBinding</code> | <code>Control.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnDataBinding(EventArgs) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOn…</code> |
| <code>Api.Control.OnInit</code> | <code>Control.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnInit(EventArgs) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullPar…</code> |
| <code>Api.Control.OnLoad</code> | <code>Control.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnLoad(EventArgs) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullPar…</code> |
| <code>Api.Control.OnPreRender</code> | <code>Control.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnPreRender(EventArgs) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNu…</code> |
| <code>Api.Control.OnUnload</code> | <code>Control.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnUnload(EventArgs) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullP…</code> |
| <code>Api.Control.OpenFile</code> | <code>Control.OpenFile(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.Page</code> | <code>Control.Page = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=&quot;&quot;; …</code><br><code>Control.Page set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=…</code> |
| <code>Api.Control.Parent</code> | <code>Control.Parent = HtmlForm#form1 {Action=&quot;&quot;; ClientID=&quot;form1&quot;; ClientIDMode=Inherit; DefaultButton=&quot;&quot;; DefaultFocus=&quot;&quot;; Disabled=false; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.PreRender</code> | <code>Control.PreRender = &quot;added and removed&quot;</code> |
| <code>Api.Control.RaiseBubbleEvent</code> | <code>Control.RaiseBubbleEvent(Object,EventArgs) = &quot;ok&quot;</code><br><code>Control.RaiseBubbleEvent(Object,EventArgs) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; Canc…</code> |
| <code>Api.Control.RemovedControl</code> | <code>Control.RemovedControl(Control) = &quot;ok&quot;</code><br><code>Control.RemovedControl(Control) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnN…</code> |
| <code>Api.Control.Render</code> | <code>Control.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.Render(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.Render(HtmlTextWriter) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNu…</code> |
| <code>Api.Control.RenderChildren</code> | <code>Control.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderChildren(HtmlTextWriter) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSe…</code> |
| <code>Api.Control.RenderControl</code> | <code>Control.RenderControl(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderControl(HtmlTextWriter) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSel…</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKey…</code> |
| <code>Api.Control.RenderingCompatibility</code> | <code>Control.RenderingCompatibility = &quot;System.Version 4.0&quot;</code><br><code>Control.RenderingCompatibility set = &quot;System.Version 4.0&quot;</code> |
| <code>Api.Control.ResolveAdapter</code> | <code>Control.ResolveAdapter = null</code><br><code>Control.ResolveAdapter after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullParame…</code> |
| <code>Api.Control.ResolveClientUrl</code> | <code>Control.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveClientUrl(String) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOn…</code> |
| <code>Api.Control.ResolveUrl</code> | <code>Control.ResolveUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveUrl(String) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullPa…</code> |
| <code>Api.Control.SaveControlState</code> | <code>Control.SaveControlState = null</code><br><code>Control.SaveControlState after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullPara…</code> |
| <code>Api.Control.SaveViewState</code> | <code>Control.SaveViewState = null</code><br><code>Control.SaveViewState after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullParamet…</code> |
| <code>Api.Control.SetDesignModeState</code> | <code>Control.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>Control.SetDesignModeState(IDictionary) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelS…</code> |
| <code>Api.Control.SetRenderMethodDelegate</code> | <code>Control.SetRenderMethodDelegate(RenderMethod) = &quot;ok&quot;</code><br><code>Control.SetRenderMethodDelegate(RenderMethod) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; C…</code> |
| <code>Api.Control.SetTraceData</code> | <code>Control.SetTraceData(Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelec…</code><br><code>Control.SetTraceData(Object,Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object,Object) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; Canc…</code> |
| <code>Api.Control.Site</code> | <code>Control.Site = null</code><br><code>Control.Site set = null</code> |
| <code>Api.Control.SkinID</code> | <code>Control.SkinID = &quot;&quot;</code><br><code>Control.SkinID set = &quot;-&gt; System.NotSupportedException: Control of type &#39;SqlDataSource&#39; does not support theming.&quot;</code> |
| <code>Api.Control.TemplateControl</code> | <code>Control.TemplateControl = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code><br><code>Control.TemplateControl set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Cl…</code> |
| <code>Api.Control.TemplateSourceDirectory</code> | <code>Control.TemplateSourceDirectory = &quot;&quot;</code> |
| <code>Api.Control.TrackViewState</code> | <code>Control.TrackViewState = &quot;ok&quot;</code><br><code>Control.TrackViewState after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullParame…</code> |
| <code>Api.Control.UniqueID</code> | <code>Control.UniqueID = &quot;sdsProducts&quot;</code> |
| <code>Api.Control.Unload</code> | <code>Control.Unload = &quot;added and removed&quot;</code> |
| <code>Api.Control.ValidateRequestMode</code> | <code>Control.ValidateRequestMode = &quot;ValidateRequestMode.Inherit&quot;</code><br><code>Control.ValidateRequestMode set = &quot;ValidateRequestMode.Disabled&quot;</code> |
| <code>Api.Control.ViewState</code> | <code>Control.ViewState = StateBag []</code> |
| <code>Api.Control.ViewStateIgnoresCase</code> | <code>Control.ViewStateIgnoresCase = &quot;false&quot;</code> |
| <code>Api.Control.ViewStateMode</code> | <code>Control.ViewStateMode = &quot;ViewStateMode.Inherit&quot;</code><br><code>Control.ViewStateMode set = &quot;ViewStateMode.Enabled&quot;</code> |
| <code>Api.Control.Visible</code> | <code>Control.Visible = &quot;false&quot;</code><br><code>Control.Visible set = &quot;-&gt; System.NotSupportedException: Control of type &#39;SqlDataSource&#39; is a non-visual control and does not support setting the Visible propert…</code> |
| <code>Api.DataSourceControl.#ctor</code> | <code>DataSourceControl.#ctor = &quot;-&gt; System.MemberAccessException: Cannot create an instance of System.Web.UI.DataSourceControl because it is an abstract class.&quot;</code> |
| <code>Api.DataSourceControl.ApplyStyleSheetSkin</code> | <code>DataSourceControl.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>DataSourceControl.ApplyStyleSheetSkin(Page) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; Can…</code> |
| <code>Api.DataSourceControl.ClientID</code> | <code>DataSourceControl.ClientID = &quot;sdsProducts&quot;</code> |
| <code>Api.DataSourceControl.ClientIDMode</code> | <code>DataSourceControl.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>DataSourceControl.ClientIDMode set = &quot;-&gt; System.NotSupportedException: Specified method is not supported.&quot;</code> |
| <code>Api.DataSourceControl.Controls</code> | <code>DataSourceControl.Controls = ControlCollection []</code> |
| <code>Api.DataSourceControl.CreateControlCollection</code> | <code>DataSourceControl.CreateControlCollection = ControlCollection []</code><br><code>DataSourceControl.CreateControlCollection after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; Cance…</code> |
| <code>Api.DataSourceControl.EnableTheming</code> | <code>DataSourceControl.EnableTheming = &quot;false&quot;</code><br><code>DataSourceControl.EnableTheming set = &quot;-&gt; System.NotSupportedException: Control of type &#39;SqlDataSource&#39; does not support theming.&quot;</code> |
| <code>Api.DataSourceControl.FindControl</code> | <code>DataSourceControl.FindControl(String) = Label#lblTitle {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #000000…</code><br><code>DataSourceControl.FindControl(String) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSel…</code> |
| <code>Api.DataSourceControl.Focus</code> | <code>DataSourceControl.Focus = &quot;-&gt; System.NotSupportedException: Control of type &#39;SqlDataSource&#39; does not support the Focus operation.&quot;</code> |
| <code>Api.DataSourceControl.GetView</code> | <code>DataSourceControl.GetView(String) = &quot;-&gt; System.ArgumentException: The data source &#39;sdsProducts&#39; only supports a single view named &#39;DefaultView&#39;. You may also le…</code> |
| <code>Api.DataSourceControl.GetViewNames</code> | <code>DataSourceControl.GetViewNames = &quot;[System.String DefaultView]&quot;</code><br><code>DataSourceControl.GetViewNames after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNu…</code> |
| <code>Api.DataSourceControl.HasControls</code> | <code>DataSourceControl.HasControls = &quot;false&quot;</code><br><code>DataSourceControl.HasControls after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNul…</code> |
| <code>Api.DataSourceControl.RaiseDataSourceChangedEvent</code> | <code>DataSourceControl.RaiseDataSourceChangedEvent(EventArgs) = &quot;ok&quot;</code><br><code>DataSourceControl.RaiseDataSourceChangedEvent(EventArgs) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDepen…</code> |
| <code>Api.DataSourceControl.RenderControl</code> | <code>DataSourceControl.RenderControl(HtmlTextWriter) = &quot;ok&quot;</code><br><code>DataSourceControl.RenderControl(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>DataSourceControl.RenderControl(HtmlTextWriter) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;;…</code> |
| <code>Api.DataSourceControl.SkinID</code> | <code>DataSourceControl.SkinID = &quot;&quot;</code><br><code>DataSourceControl.SkinID set = &quot;-&gt; System.NotSupportedException: Control of type &#39;SqlDataSource&#39; does not support theming.&quot;</code> |
| <code>Api.DataSourceControl.Visible</code> | <code>DataSourceControl.Visible = &quot;false&quot;</code><br><code>DataSourceControl.Visible set = &quot;-&gt; System.NotSupportedException: Control of type &#39;SqlDataSource&#39; is a non-visual control and does not support setting the Visib…</code> |
| <code>Api.IControlBuilderAccessor.ControlBuilder</code> | <code>IControlBuilderAccessor.ControlBuilder = null</code> |
| <code>Api.IControlDesignerAccessor.GetDesignModeState</code> | <code>IControlDesignerAccessor.GetDesignModeState = &quot;[]&quot;</code><br><code>IControlDesignerAccessor.GetDesignModeState after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; Can…</code> |
| <code>Api.IControlDesignerAccessor.SetDesignModeState</code> | <code>IControlDesignerAccessor.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetDesignModeState(IDictionary) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDepen…</code> |
| <code>Api.IControlDesignerAccessor.SetOwnerControl</code> | <code>IControlDesignerAccessor.SetOwnerControl(Control) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetOwnerControl(Control) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;…</code> |
| <code>Api.IControlDesignerAccessor.UserData</code> | <code>IControlDesignerAccessor.UserData = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.DataBindings</code> | <code>IDataBindingsAccessor.DataBindings = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.HasDataBindings</code> | <code>IDataBindingsAccessor.HasDataBindings = &quot;false&quot;</code> |
| <code>Api.IDataSource.DataSourceChanged</code> | <code>IDataSource.DataSourceChanged = &quot;added and removed&quot;</code> |
| <code>Api.IDataSource.GetView</code> | <code>IDataSource.GetView(String) = &quot;-&gt; System.ArgumentException: The data source &#39;sdsProducts&#39; only supports a single view named &#39;DefaultView&#39;. You may also leave th…</code> |
| <code>Api.IDataSource.GetViewNames</code> | <code>IDataSource.GetViewNames = &quot;[System.String DefaultView]&quot;</code><br><code>IDataSource.GetViewNames after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullPara…</code> |
| <code>Api.IExpressionsAccessor.Expressions</code> | <code>IExpressionsAccessor.Expressions = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.HasExpressions</code> | <code>IExpressionsAccessor.HasExpressions = &quot;false&quot;</code> |
| <code>Api.IParserAccessor.AddParsedSubObject</code> | <code>IParserAccessor.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>IParserAccessor.AddParsedSubObject(Object) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; Canc…</code> |
| <code>Api.IUrlResolutionService.ResolveClientUrl</code> | <code>IUrlResolutionService.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>IUrlResolutionService.ResolveClientUrl(String) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; …</code> |
| <code>Api.WebControls.SqlDataSource.#ctor</code> | <code>SqlDataSource.#ctor = SqlDataSource# {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullParameter=true; ClientID=&quot;…</code><br><code>SqlDataSource.#ctor(String,String) = SqlDataSource# {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullParameter=t…</code><br><code>SqlDataSource.#ctor(String,String,String) = SqlDataSource# {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullPara…</code> |
| <code>Api.WebControls.SqlDataSource.CacheDuration</code> | <code>SqlDataSource.CacheDuration = &quot;Int32 0&quot;</code><br><code>SqlDataSource.CacheDuration set = &quot;Int32 1&quot;</code> |
| <code>Api.WebControls.SqlDataSource.CacheExpirationPolicy</code> | <code>SqlDataSource.CacheExpirationPolicy = &quot;DataSourceCacheExpiry.Absolute&quot;</code><br><code>SqlDataSource.CacheExpirationPolicy set = &quot;DataSourceCacheExpiry.Sliding&quot;</code> |
| <code>Api.WebControls.SqlDataSource.CacheKeyDependency</code> | <code>SqlDataSource.CacheKeyDependency = &quot;&quot;</code><br><code>SqlDataSource.CacheKeyDependency set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.SqlDataSource.CancelSelectOnNullParameter</code> | <code>SqlDataSource.CancelSelectOnNullParameter = &quot;true&quot;</code><br><code>SqlDataSource.CancelSelectOnNullParameter set = &quot;true&quot;</code> |
| <code>Api.WebControls.SqlDataSource.ConflictDetection</code> | <code>SqlDataSource.ConflictDetection = &quot;ConflictOptions.OverwriteChanges&quot;</code><br><code>SqlDataSource.ConflictDetection set = &quot;ConflictOptions.CompareAllValues&quot;</code> |
| <code>Api.WebControls.SqlDataSource.ConnectionString</code> | <code>SqlDataSource.ConnectionString = &quot;&quot;</code><br><code>SqlDataSource.ConnectionString set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.SqlDataSource.CreateDataSourceView</code> | <code>SqlDataSource.CreateDataSourceView(String) = SqlDataSourceView {CanDelete=false; CanInsert=false; CanPage=false; CanRetrieveTotalRowCount=false; CanSort=true; C…</code><br><code>SqlDataSource.CreateDataSourceView(String) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; Canc…</code> |
| <code>Api.WebControls.SqlDataSource.DataSourceMode</code> | <code>SqlDataSource.DataSourceMode = &quot;SqlDataSourceMode.DataSet&quot;</code><br><code>SqlDataSource.DataSourceMode set = &quot;SqlDataSourceMode.DataSet&quot;</code> |
| <code>Api.WebControls.SqlDataSource.Delete</code> | <code>SqlDataSource.Delete = &quot;-&gt; System.NotSupportedException: Deleting is not supported by data source &#39;sdsProducts&#39; unless DeleteCommand is specified.&quot;</code> |
| <code>Api.WebControls.SqlDataSource.DeleteCommand</code> | <code>SqlDataSource.DeleteCommand = &quot;&quot;</code><br><code>SqlDataSource.DeleteCommand set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.SqlDataSource.DeleteCommandType</code> | <code>SqlDataSource.DeleteCommandType = &quot;SqlDataSourceCommandType.Text&quot;</code><br><code>SqlDataSource.DeleteCommandType set = &quot;SqlDataSourceCommandType.StoredProcedure&quot;</code> |
| <code>Api.WebControls.SqlDataSource.DeleteParameters</code> | <code>SqlDataSource.DeleteParameters = &quot;[]&quot;</code> |
| <code>Api.WebControls.SqlDataSource.Deleted</code> | <code>SqlDataSource.Deleted = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.SqlDataSource.Deleting</code> | <code>SqlDataSource.Deleting = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.SqlDataSource.EnableCaching</code> | <code>SqlDataSource.EnableCaching = &quot;false&quot;</code><br><code>SqlDataSource.EnableCaching set = &quot;true&quot;</code> |
| <code>Api.WebControls.SqlDataSource.FilterExpression</code> | <code>SqlDataSource.FilterExpression = &quot;&quot;</code><br><code>SqlDataSource.FilterExpression set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.SqlDataSource.FilterParameters</code> | <code>SqlDataSource.FilterParameters = &quot;[]&quot;</code> |
| <code>Api.WebControls.SqlDataSource.Filtering</code> | <code>SqlDataSource.Filtering = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.SqlDataSource.GetDbProviderFactory</code> | <code>SqlDataSource.GetDbProviderFactory = &quot;System.Data.SqlClient.SqlClientFactory&quot;</code><br><code>SqlDataSource.GetDbProviderFactory after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelect…</code> |
| <code>Api.WebControls.SqlDataSource.GetView</code> | <code>SqlDataSource.GetView(String) = &quot;-&gt; System.ArgumentException: The data source &#39;sdsProducts&#39; only supports a single view named &#39;DefaultView&#39;. You may also leave …</code> |
| <code>Api.WebControls.SqlDataSource.GetViewNames</code> | <code>SqlDataSource.GetViewNames = &quot;[System.String DefaultView]&quot;</code><br><code>SqlDataSource.GetViewNames after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullPa…</code> |
| <code>Api.WebControls.SqlDataSource.Insert</code> | <code>SqlDataSource.Insert = &quot;-&gt; System.NotSupportedException: Inserting is not supported by data source &#39;sdsProducts&#39; unless InsertCommand is specified.&quot;</code> |
| <code>Api.WebControls.SqlDataSource.InsertCommand</code> | <code>SqlDataSource.InsertCommand = &quot;&quot;</code><br><code>SqlDataSource.InsertCommand set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.SqlDataSource.InsertCommandType</code> | <code>SqlDataSource.InsertCommandType = &quot;SqlDataSourceCommandType.Text&quot;</code><br><code>SqlDataSource.InsertCommandType set = &quot;SqlDataSourceCommandType.StoredProcedure&quot;</code> |
| <code>Api.WebControls.SqlDataSource.InsertParameters</code> | <code>SqlDataSource.InsertParameters = &quot;[]&quot;</code> |
| <code>Api.WebControls.SqlDataSource.Inserted</code> | <code>SqlDataSource.Inserted = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.SqlDataSource.Inserting</code> | <code>SqlDataSource.Inserting = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.SqlDataSource.LoadViewState</code> | <code>SqlDataSource.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Web.UI.Pair&#39;.&quot;</code> |
| <code>Api.WebControls.SqlDataSource.OldValuesParameterFormatString</code> | <code>SqlDataSource.OldValuesParameterFormatString = &quot;{0}&quot;</code><br><code>SqlDataSource.OldValuesParameterFormatString set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.SqlDataSource.OnInit</code> | <code>SqlDataSource.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>SqlDataSource.OnInit(EventArgs) after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnN…</code> |
| <code>Api.WebControls.SqlDataSource.ProviderName</code> | <code>SqlDataSource.ProviderName = &quot;System.Data.SqlClient&quot;</code><br><code>SqlDataSource.ProviderName set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.SqlDataSource.SaveViewState</code> | <code>SqlDataSource.SaveViewState = null</code><br><code>SqlDataSource.SaveViewState after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNullP…</code> |
| <code>Api.WebControls.SqlDataSource.Select</code> | <code>SqlDataSource.Select(DataSourceSelectArguments) = &quot;-&gt; System.InvalidOperationException: The ConnectionString property has not been initialized.&quot;</code> |
| <code>Api.WebControls.SqlDataSource.SelectCommand</code> | <code>SqlDataSource.SelectCommand = &quot;SELECT 1&quot;</code><br><code>SqlDataSource.SelectCommand set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.SqlDataSource.SelectCommandType</code> | <code>SqlDataSource.SelectCommandType = &quot;SqlDataSourceCommandType.Text&quot;</code><br><code>SqlDataSource.SelectCommandType set = &quot;SqlDataSourceCommandType.StoredProcedure&quot;</code> |
| <code>Api.WebControls.SqlDataSource.SelectParameters</code> | <code>SqlDataSource.SelectParameters = &quot;[]&quot;</code> |
| <code>Api.WebControls.SqlDataSource.Selected</code> | <code>SqlDataSource.Selected = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.SqlDataSource.Selecting</code> | <code>SqlDataSource.Selecting = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.SqlDataSource.SortParameterName</code> | <code>SqlDataSource.SortParameterName = &quot;&quot;</code><br><code>SqlDataSource.SortParameterName set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.SqlDataSource.SqlCacheDependency</code> | <code>SqlDataSource.SqlCacheDependency = &quot;&quot;</code><br><code>SqlDataSource.SqlCacheDependency set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.SqlDataSource.TrackViewState</code> | <code>SqlDataSource.TrackViewState = &quot;ok&quot;</code><br><code>SqlDataSource.TrackViewState after = SqlDataSource#sdsProducts {CacheDuration=Int32 0; CacheExpirationPolicy=Absolute; CacheKeyDependency=&quot;&quot;; CancelSelectOnNull…</code> |
| <code>Api.WebControls.SqlDataSource.Update</code> | <code>SqlDataSource.Update = &quot;-&gt; System.NotSupportedException: Updating is not supported by data source &#39;sdsProducts&#39; unless UpdateCommand is specified.&quot;</code> |
| <code>Api.WebControls.SqlDataSource.UpdateCommand</code> | <code>SqlDataSource.UpdateCommand = &quot;&quot;</code><br><code>SqlDataSource.UpdateCommand set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.SqlDataSource.UpdateCommandType</code> | <code>SqlDataSource.UpdateCommandType = &quot;SqlDataSourceCommandType.Text&quot;</code><br><code>SqlDataSource.UpdateCommandType set = &quot;SqlDataSourceCommandType.StoredProcedure&quot;</code> |
| <code>Api.WebControls.SqlDataSource.UpdateParameters</code> | <code>SqlDataSource.UpdateParameters = &quot;[]&quot;</code> |
| <code>Api.WebControls.SqlDataSource.Updated</code> | <code>SqlDataSource.Updated = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.SqlDataSource.Updating</code> | <code>SqlDataSource.Updating = &quot;added and removed&quot;</code> |

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
| <code>.CancelSelectOnNullParameter=true</code> | <code>c.CancelSelectOnNullParameter = true;</code> | <code></code> |  |
| <code>.CancelSelectOnNullParameter=false</code> | <code>c.CancelSelectOnNullParameter = false;</code> | <code></code> |  |
| <code>.ClientIDMode=Inherit</code> | <code>c.ClientIDMode = ClientIDMode.Inherit;</code> | <code></code> | <code>c.ClientIDMode = ClientIDMode.Inherit; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>.ClientIDMode=AutoID</code> | <code>c.ClientIDMode = ClientIDMode.AutoID;</code> | <code></code> | <code>c.ClientIDMode = ClientIDMode.AutoID; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>.ClientIDMode=Predictable</code> | <code>c.ClientIDMode = ClientIDMode.Predictable;</code> | <code></code> | <code>c.ClientIDMode = ClientIDMode.Predictable; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>.ClientIDMode=Static</code> | <code>c.ClientIDMode = ClientIDMode.Static;</code> | <code></code> | <code>c.ClientIDMode = ClientIDMode.Static; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>.ConflictDetection=OverwriteChanges</code> | <code>c.ConflictDetection = ConflictOptions.OverwriteChanges;</code> | <code></code> |  |
| <code>.ConflictDetection=CompareAllValues</code> | <code>c.ConflictDetection = ConflictOptions.CompareAllValues;</code> | <code></code> |  |
| <code>.ConnectionString=text</code> | <code>c.ConnectionString = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.ConnectionString=markup</code> | <code>c.ConnectionString = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.ConnectionString=japanese</code> | <code>c.ConnectionString = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.ConnectionString=empty</code> | <code>c.ConnectionString = &quot;&quot;;</code> | <code></code> |  |
| <code>.DataSourceMode=DataReader</code> | <code>c.DataSourceMode = SqlDataSourceMode.DataReader;</code> | <code></code> |  |
| <code>.DataSourceMode=DataSet</code> | <code>c.DataSourceMode = SqlDataSourceMode.DataSet;</code> | <code></code> |  |
| <code>.DeleteCommand=text</code> | <code>c.DeleteCommand = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.DeleteCommand=markup</code> | <code>c.DeleteCommand = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.DeleteCommand=japanese</code> | <code>c.DeleteCommand = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.DeleteCommand=empty</code> | <code>c.DeleteCommand = &quot;&quot;;</code> | <code></code> |  |
| <code>.DeleteCommandType=Text</code> | <code>c.DeleteCommandType = SqlDataSourceCommandType.Text;</code> | <code></code> |  |
| <code>.DeleteCommandType=StoredProcedure</code> | <code>c.DeleteCommandType = SqlDataSourceCommandType.StoredProcedure;</code> | <code></code> |  |
| <code>.EnableCaching=true</code> | <code>c.EnableCaching = true;</code> | <code></code> |  |
| <code>.EnableCaching=false</code> | <code>c.EnableCaching = false;</code> | <code></code> |  |
| <code>.EnableTheming=true</code> | <code>c.EnableTheming = true;</code> | <code></code> | <code>c.EnableTheming = true; -&gt; System.NotSupportedException: Control of type &#39;SqlDataSource&#39; does not support theming.</code> |
| <code>.EnableTheming=false</code> | <code>c.EnableTheming = false;</code> | <code></code> | <code>c.EnableTheming = false; -&gt; System.NotSupportedException: Control of type &#39;SqlDataSource&#39; does not support theming.</code> |
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
| <code>.InsertCommand=text</code> | <code>c.InsertCommand = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.InsertCommand=markup</code> | <code>c.InsertCommand = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.InsertCommand=japanese</code> | <code>c.InsertCommand = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.InsertCommand=empty</code> | <code>c.InsertCommand = &quot;&quot;;</code> | <code></code> |  |
| <code>.InsertCommandType=Text</code> | <code>c.InsertCommandType = SqlDataSourceCommandType.Text;</code> | <code></code> |  |
| <code>.InsertCommandType=StoredProcedure</code> | <code>c.InsertCommandType = SqlDataSourceCommandType.StoredProcedure;</code> | <code></code> |  |
| <code>.OldValuesParameterFormatString=text</code> | <code>c.OldValuesParameterFormatString = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.OldValuesParameterFormatString=markup</code> | <code>c.OldValuesParameterFormatString = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.OldValuesParameterFormatString=japanese</code> | <code>c.OldValuesParameterFormatString = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.OldValuesParameterFormatString=empty</code> | <code>c.OldValuesParameterFormatString = &quot;&quot;;</code> | <code></code> |  |
| <code>.ProviderName=text</code> | <code>c.ProviderName = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.ProviderName=markup</code> | <code>c.ProviderName = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.ProviderName=japanese</code> | <code>c.ProviderName = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.ProviderName=empty</code> | <code>c.ProviderName = &quot;&quot;;</code> | <code></code> |  |
| <code>.ProviderName=probe</code> | <code>c.ProviderName = &quot;WebControlsProbe.Data&quot;;</code> | <code></code> |  |
| <code>.ProviderName=missing</code> | <code>c.ProviderName = &quot;No.Such.Provider&quot;;</code> | <code></code> |  |
| <code>.SelectCommand=text</code> | <code>c.SelectCommand = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.SelectCommand=markup</code> | <code>c.SelectCommand = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.SelectCommand=japanese</code> | <code>c.SelectCommand = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.SelectCommand=empty</code> | <code>c.SelectCommand = &quot;&quot;;</code> | <code></code> |  |
| <code>.SelectCommand=select</code> | <code>c.SelectCommand = &quot;select&quot;;</code> | <code></code> |  |
| <code>.SelectCommandType=Text</code> | <code>c.SelectCommandType = SqlDataSourceCommandType.Text;</code> | <code></code> |  |
| <code>.SelectCommandType=StoredProcedure</code> | <code>c.SelectCommandType = SqlDataSourceCommandType.StoredProcedure;</code> | <code></code> |  |
| <code>.SkinID=text</code> | <code>c.SkinID = &quot;probe&quot;;</code> | <code></code> | <code>c.SkinID = &quot;probe&quot;; -&gt; System.NotSupportedException: Control of type &#39;SqlDataSource&#39; does not support theming.</code> |
| <code>.SkinID=markup</code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.NotSupportedException: Control of type &#39;SqlDataSource&#39; does not support theming.</code> |
| <code>.SkinID=japanese</code> | <code>c.SkinID = &quot;日本語テキスト&quot;;</code> | <code></code> | <code>c.SkinID = &quot;日本語テキスト&quot;; -&gt; System.NotSupportedException: Control of type &#39;SqlDataSource&#39; does not support theming.</code> |
| <code>.SkinID=empty</code> | <code>c.SkinID = &quot;&quot;;</code> | <code></code> | <code>c.SkinID = &quot;&quot;; -&gt; System.NotSupportedException: Control of type &#39;SqlDataSource&#39; does not support theming.</code> |
| <code>.SortParameterName=text</code> | <code>c.SortParameterName = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.SortParameterName=markup</code> | <code>c.SortParameterName = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.SortParameterName=japanese</code> | <code>c.SortParameterName = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.SortParameterName=empty</code> | <code>c.SortParameterName = &quot;&quot;;</code> | <code></code> |  |
| <code>.SqlCacheDependency=text</code> | <code>c.SqlCacheDependency = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.SqlCacheDependency=markup</code> | <code>c.SqlCacheDependency = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.SqlCacheDependency=japanese</code> | <code>c.SqlCacheDependency = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.SqlCacheDependency=empty</code> | <code>c.SqlCacheDependency = &quot;&quot;;</code> | <code></code> |  |
| <code>.UpdateCommand=text</code> | <code>c.UpdateCommand = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.UpdateCommand=markup</code> | <code>c.UpdateCommand = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.UpdateCommand=japanese</code> | <code>c.UpdateCommand = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.UpdateCommand=empty</code> | <code>c.UpdateCommand = &quot;&quot;;</code> | <code></code> |  |
| <code>.UpdateCommandType=Text</code> | <code>c.UpdateCommandType = SqlDataSourceCommandType.Text;</code> | <code></code> |  |
| <code>.UpdateCommandType=StoredProcedure</code> | <code>c.UpdateCommandType = SqlDataSourceCommandType.StoredProcedure;</code> | <code></code> |  |
| <code>.ValidateRequestMode=Inherit</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Inherit;</code> | <code></code> |  |
| <code>.ValidateRequestMode=Disabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Disabled;</code> | <code></code> |  |
| <code>.ValidateRequestMode=Enabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Enabled;</code> | <code></code> |  |
| <code>.ViewStateMode=Inherit</code> | <code>c.ViewStateMode = ViewStateMode.Inherit;</code> | <code></code> |  |
| <code>.ViewStateMode=Enabled</code> | <code>c.ViewStateMode = ViewStateMode.Enabled;</code> | <code></code> |  |
| <code>.ViewStateMode=Disabled</code> | <code>c.ViewStateMode = ViewStateMode.Disabled;</code> | <code></code> |  |
| <code>.Visible=true</code> | <code>c.Visible = true;</code> | <code></code> | <code>c.Visible = true; -&gt; System.NotSupportedException: Control of type &#39;SqlDataSource&#39; is a non-visual control and does not support setting the Visible property.</code> |
| <code>.Visible=false</code> | <code>c.Visible = false;</code> | <code></code> | <code>c.Visible = false; -&gt; System.NotSupportedException: Control of type &#39;SqlDataSource&#39; is a non-visual control and does not support setting the Visible property.</code> |
| <code>.DataSourceMode=DataReader+EnableCaching=true</code> | <code>c.DataSourceMode = SqlDataSourceMode.DataReader; c.EnableCaching = true;</code> | <code></code> |  |
| <code>.ProviderName=missing+SelectCommand=select</code> | <code>c.ProviderName = &quot;No.Such.Provider&quot;; c.SelectCommand = &quot;select&quot;;</code> | <code></code> |  |
| <code>.CancelSelectOnNullParameter=false+FilterExpression=text</code> | <code>c.CancelSelectOnNullParameter = false; c.FilterExpression = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.Visible=false+EnableViewState=false</code> | <code>c.Visible = false; c.EnableViewState = false;</code> | <code></code> | <code>c.Visible = false; -&gt; System.NotSupportedException: Control of type &#39;SqlDataSource&#39; is a non-visual control and does not support setting the Visible property.</code> |
| <code>[naming container].(default)</code> | <code>[naming container] (nothing set)</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=AutoID</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> | <code>c.ClientIDMode = ClientIDMode.AutoID; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>[naming container].ClientIDMode=Static</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> | <code>c.ClientIDMode = ClientIDMode.Static; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>[naming container].ClientIDMode=Predictable</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> | <code>c.ClientIDMode = ClientIDMode.Predictable; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>[naming container].ClientIDMode=Inherit</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> | <code>c.ClientIDMode = ClientIDMode.Inherit; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>[naming container].ID=text</code> | <code>[naming container] c.ID = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |

## ③ シナリオ層

ブラウザ（Playwright）で SqlDataSourceScenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。

| # | 操作 | 対象 | 値 |
|---|---|---|---|
| 1 | goto | /DataSourceScenario.aspx?reset=1 | <code></code> |
| 2 | snapshot |  | **SqlDataSource [render][databind][dynamic] first request: grids bound to each data source, the Access data source selected in code, ones made in code** |
| 3 | clicktextin | gvObject | <code>Edit</code> |
| 4 | evaluate | gvObject | <code>const inputs = e.querySelectorAll(&quot;input[type=text]&quot;); inputs[0].value = &quot;Pen (edited)&quot;;</code> |
| 5 | clicktextin | gvObject | <code>Update</code> |
| 6 | snapshot |  | **SqlDataSource [viewstate] an object data source&#39;s row updated** |
| 7 | clicktextin | gvObject | <code>Delete</code> |
| 8 | snapshot |  | **SqlDataSource [viewstate] an object data source&#39;s row deleted** |
| 9 | clicktextin | gvSql | <code>Name</code> |
| 10 | snapshot |  | **SqlDataSource [viewstate] a SQL data source sorted** |
| 11 | clicktextin | gvSql | <code>Edit</code> |
| 12 | evaluate | gvSql | <code>const inputs = e.querySelectorAll(&quot;input[type=text]&quot;); inputs[0].value = &quot;Bag (edited)&quot;; inputs[1].value = &quot;25.5&quot;;</code> |
| 13 | clicktextin | gvSql | <code>Update</code> |
| 14 | snapshot |  | **SqlDataSource [viewstate] a SQL data source&#39;s row updated (its update command&#39;s parameters)** |
| 15 | clicktextin | gvSql | <code>Delete</code> |
| 16 | snapshot |  | **SqlDataSource [viewstate] a SQL data source&#39;s row deleted** |
| 17 | evaluate | dvSqlInsert | <code>const inputs = e.querySelectorAll(&quot;input[type=text]&quot;); inputs[0].value = &quot;Z9&quot;; inputs[1].value = &quot;Zipper&quot;; inputs[2].value = &quot;9.5&quot;;</code> |
| 18 | clicktextin | dvSqlInsert | <code>Insert</code> |
| 19 | snapshot |  | **SqlDataSource [viewstate] a SQL data source&#39;s row inserted** |
| 20 | clicktextin | gvLinq | <code>2</code> |
| 21 | snapshot |  | **SqlDataSource [viewstate] a LINQ data source paged** |
| 22 | clicktextin | gvLinq | <code>Price</code> |
| 23 | snapshot |  | **SqlDataSource [viewstate] a LINQ data source sorted** |
| 24 | fill | txtMinimum | <code>100</code> |
| 25 | click | btnSubmit | <code></code> |
| 26 | snapshot |  | **SqlDataSource [viewstate] a LINQ data source&#39;s where parameter from a text box** |
| 27 | fill | txtSearch | <code>a</code> |
| 28 | click | btnSubmit | <code></code> |
| 29 | snapshot |  | **SqlDataSource [viewstate] a query extender&#39;s search from a text box** |
| 30 | click | btnModel | <code></code> |
| 31 | snapshot |  | **SqlDataSource [viewstate] a model error shown** |
| 32 | fill | txtModelName | <code>named</code> |
| 33 | click | btnModel | <code></code> |
| 34 | snapshot |  | **SqlDataSource [viewstate] no model error** |
| 35 | clicktextin | gvDynamic | <code>Name</code> |
| 36 | snapshot |  | **SqlDataSource [dynamic][viewstate] a grid sorted by a data source made in code** |
| 37 | click | btnShow | <code></code> |
| 38 | snapshot |  | **SqlDataSource [visible][viewstate] a hidden grid bound to a data source shown** |

<details><summary>SqlDataSource [render][databind][dynamic] first request: grids bound to each data source, the Access data source selected in code, ones made in code（期待される本文）</summary>

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

<details><summary>SqlDataSource [viewstate] an object data source&#39;s row updated（期待される本文）</summary>

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

<details><summary>SqlDataSource [viewstate] an object data source&#39;s row deleted（期待される本文）</summary>

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

<details><summary>SqlDataSource [viewstate] a SQL data source sorted（期待される本文）</summary>

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

<details><summary>SqlDataSource [viewstate] a SQL data source&#39;s row updated (its update command&#39;s parameters)（期待される本文）</summary>

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

<details><summary>SqlDataSource [viewstate] a SQL data source&#39;s row deleted（期待される本文）</summary>

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

<details><summary>SqlDataSource [viewstate] a SQL data source&#39;s row inserted（期待される本文）</summary>

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

<details><summary>SqlDataSource [viewstate] a LINQ data source paged（期待される本文）</summary>

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

<details><summary>SqlDataSource [viewstate] a LINQ data source sorted（期待される本文）</summary>

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

<details><summary>SqlDataSource [viewstate] a LINQ data source&#39;s where parameter from a text box（期待される本文）</summary>

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

<details><summary>SqlDataSource [viewstate] a query extender&#39;s search from a text box（期待される本文）</summary>

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

<details><summary>SqlDataSource [viewstate] a model error shown（期待される本文）</summary>

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

<details><summary>SqlDataSource [viewstate] no model error（期待される本文）</summary>

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

<details><summary>SqlDataSource [dynamic][viewstate] a grid sorted by a data source made in code（期待される本文）</summary>

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

<details><summary>SqlDataSource [visible][viewstate] a hidden grid bound to a data source shown（期待される本文）</summary>

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

