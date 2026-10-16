# LinqDataSource テストケース一覧

変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。
このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。

- 正解データの採取: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0
- ① API 層: 192 ケース（199 メンバー）
- ② HTML 層: 92 ケース
- ③ シナリオ層: 15 スナップショット

## 機構マトリクス

型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。

| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |
|---|---|---|---|
| プロパティ → HTML 描画 | すべてのコントロール | 85 | 1 |
| ViewState の保存と復元 | Control | 6 | 14 |
| Visible=false（描画されず状態は保持） | Control.Visible | 2 | 1 |
| 名前付けコンテナと ClientIDMode | Control.ClientIDMode | 6 | 0 |
| 動的に生成したコントロールのライフサイクル | Control | 0 | 2 |
| データバインド（DataBind / <%# %>） | Control.DataBind | 0 | 1 |

該当しない機構: テーマとスキン（SkinID / EnableTheming）、Enabled=false・Disabled=true（描画とポスト値の扱い）、ポストバックデータの取り込み、変更イベント（*Changed、ServerChange）、ReadOnly（ポストされた値を無視）、リクエスト検証（ValidateRequestMode）、ポストバックイベント、コマンドイベントと親へのバブル（CommandName）、送信ボタン／スクリプトによるポストバック（UseSubmitBehavior）、ブラウザ側のクリック処理（OnClientClick）、クロスページポストバック（PostBackUrl）、既定のボタン（Panel の DefaultButton）、AutoPostBack（クライアントスクリプトによるポストバック）、CausesValidation / ValidationGroup、検証対象（サーバー側の検証）、検証対象（ブラウザ側の検証）、双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値）、項目（ListItem の Selected・Enabled・Text・Value と ViewState）、複数選択、繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）、ポスト値のイベント検証（一覧にない値・無効な項目の拒否）、サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）、ブラウザ側の検証スクリプト（EnableClientScript）、検証結果のまとめ（ValidationSummary）、ページング、並べ替え、項目の挿入、行・項目の編集（編集・更新・取り消し・削除）、行・項目の選択、行・項目のコマンド（RowCommand・ItemCommand へのバブル）、データキー（DataKeyNames・DataKeys）、データソースコントロールとの連携（DataSourceID）、テンプレート、データソース連携（DataSource・DataTextField など）、クライアントコールバック

## ① API 層

LinqDataSource とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の LinqDataSource インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。

| ケース | 期待値（.NET Framework） |
|---|---|
| <code>Api.Control.#ctor</code> | <code>Control.#ctor = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMode=In…</code> |
| <code>Api.Control.Adapter</code> | <code>Control.Adapter = null</code> |
| <code>Api.Control.AddParsedSubObject</code> | <code>Control.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>Control.AddParsedSubObject(Object) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=t…</code> |
| <code>Api.Control.AddedControl</code> | <code>Control.AddedControl(Control,Int32) = &quot;ok&quot;</code><br><code>Control.AddedControl(Control,Int32) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=…</code> |
| <code>Api.Control.AppRelativeTemplateSourceDirectory</code> | <code>Control.AppRelativeTemplateSourceDirectory = &quot;&quot;</code><br><code>Control.AppRelativeTemplateSourceDirectory set = &quot;-&gt; System.ArgumentException: The relative virtual path &#39;Abc 123&#39; is not allowed here.&quot;</code> |
| <code>Api.Control.ApplyStyleSheetSkin</code> | <code>Control.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>Control.ApplyStyleSheetSkin(Page) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=tr…</code> |
| <code>Api.Control.BeginRenderTracing</code> | <code>Control.BeginRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.BeginRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.BeginRenderTracing(TextWriter,Object) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true;…</code> |
| <code>Api.Control.BindingContainer</code> | <code>Control.BindingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Clien…</code> |
| <code>Api.Control.BuildProfileTree</code> | <code>Control.BuildProfileTree(String,Boolean) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.ChildControlsCreated</code> | <code>Control.ChildControlsCreated = &quot;false&quot;</code><br><code>Control.ChildControlsCreated set = &quot;ok&quot;</code> |
| <code>Api.Control.ClearCachedClientID</code> | <code>Control.ClearCachedClientID = &quot;ok&quot;</code><br><code>Control.ClearCachedClientID after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; Cl…</code> |
| <code>Api.Control.ClearChildControlState</code> | <code>Control.ClearChildControlState = &quot;ok&quot;</code><br><code>Control.ClearChildControlState after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true;…</code> |
| <code>Api.Control.ClearChildState</code> | <code>Control.ClearChildState = &quot;ok&quot;</code><br><code>Control.ClearChildState after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; Client…</code> |
| <code>Api.Control.ClearChildViewState</code> | <code>Control.ClearChildViewState = &quot;ok&quot;</code><br><code>Control.ClearChildViewState after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; Cl…</code> |
| <code>Api.Control.ClearEffectiveClientIDMode</code> | <code>Control.ClearEffectiveClientIDMode = &quot;ok&quot;</code><br><code>Control.ClearEffectiveClientIDMode after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=t…</code> |
| <code>Api.Control.ClientID</code> | <code>Control.ClientID = &quot;ldsProducts&quot;</code> |
| <code>Api.Control.ClientIDMode</code> | <code>Control.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>Control.ClientIDMode set = &quot;-&gt; System.NotSupportedException: Specified method is not supported.&quot;</code> |
| <code>Api.Control.ClientIDSeparator</code> | <code>Control.ClientIDSeparator = &quot;&#39;_&#39;&quot;</code> |
| <code>Api.Control.Context</code> | <code>Control.Context = null</code> |
| <code>Api.Control.Controls</code> | <code>Control.Controls = ControlCollection []</code> |
| <code>Api.Control.CreateChildControls</code> | <code>Control.CreateChildControls = &quot;ok&quot;</code><br><code>Control.CreateChildControls after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; Cl…</code> |
| <code>Api.Control.CreateControlCollection</code> | <code>Control.CreateControlCollection = ControlCollection []</code><br><code>Control.CreateControlCollection after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true…</code> |
| <code>Api.Control.DataBind</code> | <code>Control.DataBind = &quot;ok&quot;</code><br><code>Control.DataBind after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; ClientID=&quot;lds…</code><br><code>Control.DataBind(Boolean) = &quot;ok&quot;</code><br><code>Control.DataBind(Boolean) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; Clie…</code> |
| <code>Api.Control.DataBindChildren</code> | <code>Control.DataBindChildren = &quot;ok&quot;</code><br><code>Control.DataBindChildren after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; Clien…</code> |
| <code>Api.Control.DataBinding</code> | <code>Control.DataBinding = &quot;added and removed&quot;</code> |
| <code>Api.Control.DataItemContainer</code> | <code>Control.DataItemContainer = null</code> |
| <code>Api.Control.DataKeysContainer</code> | <code>Control.DataKeysContainer = null</code> |
| <code>Api.Control.DesignMode</code> | <code>Control.DesignMode = &quot;false&quot;</code> |
| <code>Api.Control.Dispose</code> | <code>Control.Dispose = &quot;ok&quot;</code><br><code>Control.Dispose after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; ClientID=&quot;ldsP…</code> |
| <code>Api.Control.Disposed</code> | <code>Control.Disposed = &quot;added and removed&quot;</code> |
| <code>Api.Control.EnableTheming</code> | <code>Control.EnableTheming = &quot;false&quot;</code><br><code>Control.EnableTheming set = &quot;-&gt; System.NotSupportedException: Control of type &#39;LinqDataSource&#39; does not support theming.&quot;</code> |
| <code>Api.Control.EnableViewState</code> | <code>Control.EnableViewState = &quot;true&quot;</code><br><code>Control.EnableViewState set = &quot;true&quot;</code> |
| <code>Api.Control.EndRenderTracing</code> | <code>Control.EndRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.EndRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.EndRenderTracing(TextWriter,Object) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; A…</code> |
| <code>Api.Control.EnsureChildControls</code> | <code>Control.EnsureChildControls = &quot;ok&quot;</code><br><code>Control.EnsureChildControls after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; Cl…</code> |
| <code>Api.Control.EnsureID</code> | <code>Control.EnsureID = &quot;ok&quot;</code><br><code>Control.EnsureID after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; ClientID=&quot;lds…</code> |
| <code>Api.Control.Events</code> | <code>Control.Events = &quot;System.ComponentModel.EventHandlerList&quot;</code> |
| <code>Api.Control.FindControl</code> | <code>Control.FindControl(String) = Label#lblTitle {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code><br><code>Control.FindControl(String) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; Cl…</code><br><code>Control.FindControl(String,Int32) = null</code><br><code>Control.FindControl(String,Int32) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=tr…</code> |
| <code>Api.Control.Focus</code> | <code>Control.Focus = &quot;-&gt; System.NotSupportedException: Control of type &#39;LinqDataSource&#39; does not support the Focus operation.&quot;</code> |
| <code>Api.Control.GetDesignModeState</code> | <code>Control.GetDesignModeState = &quot;[]&quot;</code><br><code>Control.GetDesignModeState after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; Cli…</code> |
| <code>Api.Control.GetRouteUrl</code> | <code>Control.GetRouteUrl(Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.GetUniqueIDRelativeTo</code> | <code>Control.GetUniqueIDRelativeTo(Control) = &quot;ldsProducts&quot;</code><br><code>Control.GetUniqueIDRelativeTo(Control) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSo…</code> |
| <code>Api.Control.HasChildViewState</code> | <code>Control.HasChildViewState = &quot;false&quot;</code> |
| <code>Api.Control.HasControls</code> | <code>Control.HasControls = &quot;false&quot;</code><br><code>Control.HasControls after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; ClientID=&quot;…</code> |
| <code>Api.Control.HasEvents</code> | <code>Control.HasEvents = &quot;true&quot;</code><br><code>Control.HasEvents after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; ClientID=&quot;ld…</code> |
| <code>Api.Control.ID</code> | <code>Control.ID = &quot;ldsProducts&quot;</code><br><code>Control.ID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.IdSeparator</code> | <code>Control.IdSeparator = &quot;&#39;$&#39;&quot;</code> |
| <code>Api.Control.Init</code> | <code>Control.Init = &quot;added and removed&quot;</code> |
| <code>Api.Control.IsChildControlStateCleared</code> | <code>Control.IsChildControlStateCleared = &quot;false&quot;</code> |
| <code>Api.Control.IsLiteralContent</code> | <code>Control.IsLiteralContent = &quot;false&quot;</code><br><code>Control.IsLiteralContent after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; Clien…</code> |
| <code>Api.Control.IsTrackingViewState</code> | <code>Control.IsTrackingViewState = &quot;false&quot;</code> |
| <code>Api.Control.IsViewStateEnabled</code> | <code>Control.IsViewStateEnabled = &quot;true&quot;</code> |
| <code>Api.Control.Load</code> | <code>Control.Load = &quot;added and removed&quot;</code> |
| <code>Api.Control.LoadControlState</code> | <code>Control.LoadControlState(Object) = &quot;ok&quot;</code><br><code>Control.LoadControlState(Object) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=tru…</code> |
| <code>Api.Control.LoadViewState</code> | <code>Control.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Web.UI.Pair&#39;.&quot;</code> |
| <code>Api.Control.LoadViewStateByID</code> | <code>Control.LoadViewStateByID = &quot;false&quot;</code> |
| <code>Api.Control.MapPathSecure</code> | <code>Control.MapPathSecure(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.NamingContainer</code> | <code>Control.NamingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code> |
| <code>Api.Control.OnBubbleEvent</code> | <code>Control.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Control.OnBubbleEvent(Object,EventArgs) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoS…</code> |
| <code>Api.Control.OnDataBinding</code> | <code>Control.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnDataBinding(EventArgs) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=tru…</code> |
| <code>Api.Control.OnInit</code> | <code>Control.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnInit(EventArgs) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; Clie…</code> |
| <code>Api.Control.OnLoad</code> | <code>Control.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnLoad(EventArgs) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; Clie…</code> |
| <code>Api.Control.OnPreRender</code> | <code>Control.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnPreRender(EventArgs) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true;…</code> |
| <code>Api.Control.OnUnload</code> | <code>Control.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnUnload(EventArgs) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; Cl…</code> |
| <code>Api.Control.OpenFile</code> | <code>Control.OpenFile(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.Page</code> | <code>Control.Page = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=&quot;&quot;; …</code><br><code>Control.Page set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=…</code> |
| <code>Api.Control.Parent</code> | <code>Control.Parent = HtmlForm#form1 {Action=&quot;&quot;; ClientID=&quot;form1&quot;; ClientIDMode=Inherit; DefaultButton=&quot;&quot;; DefaultFocus=&quot;&quot;; Disabled=false; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.PreRender</code> | <code>Control.PreRender = &quot;added and removed&quot;</code> |
| <code>Api.Control.RaiseBubbleEvent</code> | <code>Control.RaiseBubbleEvent(Object,EventArgs) = &quot;ok&quot;</code><br><code>Control.RaiseBubbleEvent(Object,EventArgs) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; Au…</code> |
| <code>Api.Control.RemovedControl</code> | <code>Control.RemovedControl(Control) = &quot;ok&quot;</code><br><code>Control.RemovedControl(Control) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true…</code> |
| <code>Api.Control.Render</code> | <code>Control.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.Render(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.Render(HtmlTextWriter) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true;…</code> |
| <code>Api.Control.RenderChildren</code> | <code>Control.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderChildren(HtmlTextWriter) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSo…</code> |
| <code>Api.Control.RenderControl</code> | <code>Control.RenderControl(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderControl(HtmlTextWriter) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSor…</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false…</code> |
| <code>Api.Control.RenderingCompatibility</code> | <code>Control.RenderingCompatibility = &quot;System.Version 4.0&quot;</code><br><code>Control.RenderingCompatibility set = &quot;System.Version 4.0&quot;</code> |
| <code>Api.Control.ResolveAdapter</code> | <code>Control.ResolveAdapter = null</code><br><code>Control.ResolveAdapter after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; ClientI…</code> |
| <code>Api.Control.ResolveClientUrl</code> | <code>Control.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveClientUrl(String) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=tru…</code> |
| <code>Api.Control.ResolveUrl</code> | <code>Control.ResolveUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveUrl(String) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; Cli…</code> |
| <code>Api.Control.SaveControlState</code> | <code>Control.SaveControlState = null</code><br><code>Control.SaveControlState after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; Clien…</code> |
| <code>Api.Control.SaveViewState</code> | <code>Control.SaveViewState = Pair {}</code><br><code>Control.SaveViewState after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; ClientID…</code> |
| <code>Api.Control.SetDesignModeState</code> | <code>Control.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>Control.SetDesignModeState(IDictionary) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoS…</code> |
| <code>Api.Control.SetRenderMethodDelegate</code> | <code>Control.SetRenderMethodDelegate(RenderMethod) = &quot;ok&quot;</code><br><code>Control.SetRenderMethodDelegate(RenderMethod) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true;…</code> |
| <code>Api.Control.SetTraceData</code> | <code>Control.SetTraceData(Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=…</code><br><code>Control.SetTraceData(Object,Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object,Object) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; Au…</code> |
| <code>Api.Control.Site</code> | <code>Control.Site = null</code><br><code>Control.Site set = null</code> |
| <code>Api.Control.SkinID</code> | <code>Control.SkinID = &quot;&quot;</code><br><code>Control.SkinID set = &quot;-&gt; System.NotSupportedException: Control of type &#39;LinqDataSource&#39; does not support theming.&quot;</code> |
| <code>Api.Control.TemplateControl</code> | <code>Control.TemplateControl = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code><br><code>Control.TemplateControl set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Cl…</code> |
| <code>Api.Control.TemplateSourceDirectory</code> | <code>Control.TemplateSourceDirectory = &quot;&quot;</code> |
| <code>Api.Control.TrackViewState</code> | <code>Control.TrackViewState = &quot;ok&quot;</code><br><code>Control.TrackViewState after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; ClientI…</code> |
| <code>Api.Control.UniqueID</code> | <code>Control.UniqueID = &quot;ldsProducts&quot;</code> |
| <code>Api.Control.Unload</code> | <code>Control.Unload = &quot;added and removed&quot;</code> |
| <code>Api.Control.ValidateRequestMode</code> | <code>Control.ValidateRequestMode = &quot;ValidateRequestMode.Inherit&quot;</code><br><code>Control.ValidateRequestMode set = &quot;ValidateRequestMode.Disabled&quot;</code> |
| <code>Api.Control.ViewState</code> | <code>Control.ViewState = StateBag []</code> |
| <code>Api.Control.ViewStateIgnoresCase</code> | <code>Control.ViewStateIgnoresCase = &quot;false&quot;</code> |
| <code>Api.Control.ViewStateMode</code> | <code>Control.ViewStateMode = &quot;ViewStateMode.Inherit&quot;</code><br><code>Control.ViewStateMode set = &quot;ViewStateMode.Enabled&quot;</code> |
| <code>Api.Control.Visible</code> | <code>Control.Visible = &quot;false&quot;</code><br><code>Control.Visible set = &quot;-&gt; System.NotSupportedException: Control of type &#39;LinqDataSource&#39; is a non-visual control and does not support setting the Visible proper…</code> |
| <code>Api.DataSourceControl.#ctor</code> | <code>DataSourceControl.#ctor = &quot;-&gt; System.MemberAccessException: Cannot create an instance of System.Web.UI.DataSourceControl because it is an abstract class.&quot;</code> |
| <code>Api.DataSourceControl.ApplyStyleSheetSkin</code> | <code>DataSourceControl.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>DataSourceControl.ApplyStyleSheetSkin(Page) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; A…</code> |
| <code>Api.DataSourceControl.ClientID</code> | <code>DataSourceControl.ClientID = &quot;ldsProducts&quot;</code> |
| <code>Api.DataSourceControl.ClientIDMode</code> | <code>DataSourceControl.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>DataSourceControl.ClientIDMode set = &quot;-&gt; System.NotSupportedException: Specified method is not supported.&quot;</code> |
| <code>Api.DataSourceControl.Controls</code> | <code>DataSourceControl.Controls = ControlCollection []</code> |
| <code>Api.DataSourceControl.CreateControlCollection</code> | <code>DataSourceControl.CreateControlCollection = ControlCollection []</code><br><code>DataSourceControl.CreateControlCollection after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; Aut…</code> |
| <code>Api.DataSourceControl.EnableTheming</code> | <code>DataSourceControl.EnableTheming = &quot;false&quot;</code><br><code>DataSourceControl.EnableTheming set = &quot;-&gt; System.NotSupportedException: Control of type &#39;LinqDataSource&#39; does not support theming.&quot;</code> |
| <code>Api.DataSourceControl.FindControl</code> | <code>DataSourceControl.FindControl(String) = Label#lblTitle {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #000000…</code><br><code>DataSourceControl.FindControl(String) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSor…</code> |
| <code>Api.DataSourceControl.Focus</code> | <code>DataSourceControl.Focus = &quot;-&gt; System.NotSupportedException: Control of type &#39;LinqDataSource&#39; does not support the Focus operation.&quot;</code> |
| <code>Api.DataSourceControl.GetView</code> | <code>DataSourceControl.GetView(String) = &quot;-&gt; System.ArgumentException: LinqDataSource &#39;ldsProducts&#39; only supports a single view named &#39;DefaultView&#39;. You may also lea…</code> |
| <code>Api.DataSourceControl.GetViewNames</code> | <code>DataSourceControl.GetViewNames = &quot;[System.String DefaultView]&quot;</code><br><code>DataSourceControl.GetViewNames after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true;…</code> |
| <code>Api.DataSourceControl.HasControls</code> | <code>DataSourceControl.HasControls = &quot;false&quot;</code><br><code>DataSourceControl.HasControls after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; …</code> |
| <code>Api.DataSourceControl.RaiseDataSourceChangedEvent</code> | <code>DataSourceControl.RaiseDataSourceChangedEvent(EventArgs) = &quot;ok&quot;</code><br><code>DataSourceControl.RaiseDataSourceChangedEvent(EventArgs) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; Aut…</code> |
| <code>Api.DataSourceControl.RenderControl</code> | <code>DataSourceControl.RenderControl(HtmlTextWriter) = &quot;ok&quot;</code><br><code>DataSourceControl.RenderControl(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>DataSourceControl.RenderControl(HtmlTextWriter) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=tru…</code> |
| <code>Api.DataSourceControl.SkinID</code> | <code>DataSourceControl.SkinID = &quot;&quot;</code><br><code>DataSourceControl.SkinID set = &quot;-&gt; System.NotSupportedException: Control of type &#39;LinqDataSource&#39; does not support theming.&quot;</code> |
| <code>Api.DataSourceControl.Visible</code> | <code>DataSourceControl.Visible = &quot;false&quot;</code><br><code>DataSourceControl.Visible set = &quot;-&gt; System.NotSupportedException: Control of type &#39;LinqDataSource&#39; is a non-visual control and does not support setting the Visi…</code> |
| <code>Api.IControlBuilderAccessor.ControlBuilder</code> | <code>IControlBuilderAccessor.ControlBuilder = null</code> |
| <code>Api.IControlDesignerAccessor.GetDesignModeState</code> | <code>IControlDesignerAccessor.GetDesignModeState = &quot;[]&quot;</code><br><code>IControlDesignerAccessor.GetDesignModeState after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; A…</code> |
| <code>Api.IControlDesignerAccessor.SetDesignModeState</code> | <code>IControlDesignerAccessor.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetDesignModeState(IDictionary) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; Aut…</code> |
| <code>Api.IControlDesignerAccessor.SetOwnerControl</code> | <code>IControlDesignerAccessor.SetOwnerControl(Control) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetOwnerControl(Control) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=t…</code> |
| <code>Api.IControlDesignerAccessor.UserData</code> | <code>IControlDesignerAccessor.UserData = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.DataBindings</code> | <code>IDataBindingsAccessor.DataBindings = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.HasDataBindings</code> | <code>IDataBindingsAccessor.HasDataBindings = &quot;false&quot;</code> |
| <code>Api.IDataSource.DataSourceChanged</code> | <code>IDataSource.DataSourceChanged = &quot;added and removed&quot;</code> |
| <code>Api.IDataSource.GetView</code> | <code>IDataSource.GetView(String) = &quot;-&gt; System.ArgumentException: LinqDataSource &#39;ldsProducts&#39; only supports a single view named &#39;DefaultView&#39;. You may also leave the…</code> |
| <code>Api.IDataSource.GetViewNames</code> | <code>IDataSource.GetViewNames = &quot;[System.String DefaultView]&quot;</code><br><code>IDataSource.GetViewNames after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; Clien…</code> |
| <code>Api.IExpressionsAccessor.Expressions</code> | <code>IExpressionsAccessor.Expressions = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.HasExpressions</code> | <code>IExpressionsAccessor.HasExpressions = &quot;false&quot;</code> |
| <code>Api.IParserAccessor.AddParsedSubObject</code> | <code>IParserAccessor.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>IParserAccessor.AddParsedSubObject(Object) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; Au…</code> |
| <code>Api.IUrlResolutionService.ResolveClientUrl</code> | <code>IUrlResolutionService.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>IUrlResolutionService.ResolveClientUrl(String) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true…</code> |
| <code>Api.System.Web.DynamicData.IDynamicDataSource.AutoGenerateWhereClause</code> | <code>DynamicData.IDynamicDataSource.AutoGenerateWhereClause = &quot;false&quot;</code><br><code>DynamicData.IDynamicDataSource.AutoGenerateWhereClause set = &quot;true&quot;</code> |
| <code>Api.System.Web.DynamicData.IDynamicDataSource.ContextType</code> | <code>DynamicData.IDynamicDataSource.ContextType = &quot;System.RuntimeType System.Object&quot;</code><br><code>DynamicData.IDynamicDataSource.ContextType set = &quot;System.RuntimeType System.Web.UI.WebControls.Label&quot;</code> |
| <code>Api.System.Web.DynamicData.IDynamicDataSource.EnableDelete</code> | <code>DynamicData.IDynamicDataSource.EnableDelete = &quot;false&quot;</code><br><code>DynamicData.IDynamicDataSource.EnableDelete set = &quot;true&quot;</code> |
| <code>Api.System.Web.DynamicData.IDynamicDataSource.EnableInsert</code> | <code>DynamicData.IDynamicDataSource.EnableInsert = &quot;false&quot;</code><br><code>DynamicData.IDynamicDataSource.EnableInsert set = &quot;true&quot;</code> |
| <code>Api.System.Web.DynamicData.IDynamicDataSource.EnableUpdate</code> | <code>DynamicData.IDynamicDataSource.EnableUpdate = &quot;false&quot;</code><br><code>DynamicData.IDynamicDataSource.EnableUpdate set = &quot;true&quot;</code> |
| <code>Api.System.Web.DynamicData.IDynamicDataSource.EntitySetName</code> | <code>DynamicData.IDynamicDataSource.EntitySetName = &quot;Products&quot;</code><br><code>DynamicData.IDynamicDataSource.EntitySetName set = &quot;Abc 123&quot;</code> |
| <code>Api.System.Web.DynamicData.IDynamicDataSource.Exception</code> | <code>DynamicData.IDynamicDataSource.Exception = &quot;added and removed&quot;</code> |
| <code>Api.System.Web.DynamicData.IDynamicDataSource.Where</code> | <code>DynamicData.IDynamicDataSource.Where = &quot;&quot;</code><br><code>DynamicData.IDynamicDataSource.Where set = &quot;Abc 123&quot;</code> |
| <code>Api.System.Web.DynamicData.IDynamicDataSource.WhereParameters</code> | <code>DynamicData.IDynamicDataSource.WhereParameters = &quot;[]&quot;</code> |
| <code>Api.WebControls.ContextDataSource.#ctor</code> | <code>ContextDataSource.#ctor = &quot;-&gt; System.MemberAccessException: Cannot create an instance of System.Web.UI.WebControls.ContextDataSource because it is an abstract c…</code> |
| <code>Api.WebControls.ContextDataSource.ContextTypeName</code> | <code>ContextDataSource.ContextTypeName = &quot;System.Object&quot;</code><br><code>ContextDataSource.ContextTypeName set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ContextDataSource.EntitySetName</code> | <code>ContextDataSource.EntitySetName = &quot;Products&quot;</code><br><code>ContextDataSource.EntitySetName set = &quot;ok&quot;</code> |
| <code>Api.WebControls.ContextDataSource.EntityTypeName</code> | <code>ContextDataSource.EntityTypeName = &quot;&quot;</code><br><code>ContextDataSource.EntityTypeName set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.IQueryableDataSource.QueryCreated</code> | <code>IQueryableDataSource.QueryCreated = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.IQueryableDataSource.RaiseViewChanged</code> | <code>IQueryableDataSource.RaiseViewChanged = &quot;ok&quot;</code><br><code>IQueryableDataSource.RaiseViewChanged after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSor…</code> |
| <code>Api.WebControls.LinqDataSource.#ctor</code> | <code>LinqDataSource.#ctor = LinqDataSource# {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; ClientID=&quot;&quot;; ClientIDMode=…</code> |
| <code>Api.WebControls.LinqDataSource.AutoGenerateOrderByClause</code> | <code>LinqDataSource.AutoGenerateOrderByClause = &quot;false&quot;</code><br><code>LinqDataSource.AutoGenerateOrderByClause set = &quot;true&quot;</code> |
| <code>Api.WebControls.LinqDataSource.AutoGenerateWhereClause</code> | <code>LinqDataSource.AutoGenerateWhereClause = &quot;false&quot;</code><br><code>LinqDataSource.AutoGenerateWhereClause set = &quot;true&quot;</code> |
| <code>Api.WebControls.LinqDataSource.AutoPage</code> | <code>LinqDataSource.AutoPage = &quot;true&quot;</code><br><code>LinqDataSource.AutoPage set = &quot;true&quot;</code> |
| <code>Api.WebControls.LinqDataSource.AutoSort</code> | <code>LinqDataSource.AutoSort = &quot;true&quot;</code><br><code>LinqDataSource.AutoSort set = &quot;true&quot;</code> |
| <code>Api.WebControls.LinqDataSource.ContextCreated</code> | <code>LinqDataSource.ContextCreated = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.LinqDataSource.ContextCreating</code> | <code>LinqDataSource.ContextCreating = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.LinqDataSource.ContextDisposing</code> | <code>LinqDataSource.ContextDisposing = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.LinqDataSource.ContextTypeName</code> | <code>LinqDataSource.ContextTypeName = &quot;System.Object&quot;</code><br><code>LinqDataSource.ContextTypeName set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.LinqDataSource.CreateQueryableView</code> | <code>LinqDataSource.CreateQueryableView = LinqDataSourceView {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; CanDelete…</code><br><code>LinqDataSource.CreateQueryableView after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=t…</code> |
| <code>Api.WebControls.LinqDataSource.CreateView</code> | <code>LinqDataSource.CreateView = LinqDataSourceView {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; CanDelete=false; C…</code><br><code>LinqDataSource.CreateView after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; Clie…</code> |
| <code>Api.WebControls.LinqDataSource.Delete</code> | <code>LinqDataSource.Delete(IDictionary,IDictionary) = &quot;-&gt; System.NotSupportedException: LinqDataSource &#39;ldsProducts&#39; does not support the Delete operation unless Ena…</code> |
| <code>Api.WebControls.LinqDataSource.DeleteParameters</code> | <code>LinqDataSource.DeleteParameters = &quot;[]&quot;</code> |
| <code>Api.WebControls.LinqDataSource.Deleted</code> | <code>LinqDataSource.Deleted = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.LinqDataSource.Deleting</code> | <code>LinqDataSource.Deleting = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.LinqDataSource.EnableDelete</code> | <code>LinqDataSource.EnableDelete = &quot;false&quot;</code><br><code>LinqDataSource.EnableDelete set = &quot;true&quot;</code> |
| <code>Api.WebControls.LinqDataSource.EnableInsert</code> | <code>LinqDataSource.EnableInsert = &quot;false&quot;</code><br><code>LinqDataSource.EnableInsert set = &quot;true&quot;</code> |
| <code>Api.WebControls.LinqDataSource.EnableObjectTracking</code> | <code>LinqDataSource.EnableObjectTracking = &quot;true&quot;</code><br><code>LinqDataSource.EnableObjectTracking set = &quot;true&quot;</code> |
| <code>Api.WebControls.LinqDataSource.EnableUpdate</code> | <code>LinqDataSource.EnableUpdate = &quot;false&quot;</code><br><code>LinqDataSource.EnableUpdate set = &quot;true&quot;</code> |
| <code>Api.WebControls.LinqDataSource.GroupBy</code> | <code>LinqDataSource.GroupBy = &quot;&quot;</code><br><code>LinqDataSource.GroupBy set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.LinqDataSource.GroupByParameters</code> | <code>LinqDataSource.GroupByParameters = &quot;[]&quot;</code> |
| <code>Api.WebControls.LinqDataSource.Insert</code> | <code>LinqDataSource.Insert(IDictionary) = &quot;-&gt; System.NotSupportedException: LinqDataSource &#39;ldsProducts&#39; does not support the Insert operation unless EnableInsert is…</code> |
| <code>Api.WebControls.LinqDataSource.InsertParameters</code> | <code>LinqDataSource.InsertParameters = &quot;[]&quot;</code> |
| <code>Api.WebControls.LinqDataSource.Inserted</code> | <code>LinqDataSource.Inserted = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.LinqDataSource.Inserting</code> | <code>LinqDataSource.Inserting = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.LinqDataSource.OnInit</code> | <code>LinqDataSource.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>LinqDataSource.OnInit(EventArgs) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=tru…</code> |
| <code>Api.WebControls.LinqDataSource.OnUnload</code> | <code>LinqDataSource.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>LinqDataSource.OnUnload(EventArgs) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=t…</code> |
| <code>Api.WebControls.LinqDataSource.OrderBy</code> | <code>LinqDataSource.OrderBy = &quot;&quot;</code><br><code>LinqDataSource.OrderBy set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.LinqDataSource.OrderByParameters</code> | <code>LinqDataSource.OrderByParameters = &quot;[]&quot;</code> |
| <code>Api.WebControls.LinqDataSource.OrderGroupsBy</code> | <code>LinqDataSource.OrderGroupsBy = &quot;&quot;</code><br><code>LinqDataSource.OrderGroupsBy set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.LinqDataSource.OrderGroupsByParameters</code> | <code>LinqDataSource.OrderGroupsByParameters = &quot;[]&quot;</code> |
| <code>Api.WebControls.LinqDataSource.Select</code> | <code>LinqDataSource.Select = &quot;&quot;</code><br><code>LinqDataSource.Select set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.LinqDataSource.SelectParameters</code> | <code>LinqDataSource.SelectParameters = &quot;[]&quot;</code> |
| <code>Api.WebControls.LinqDataSource.Selected</code> | <code>LinqDataSource.Selected = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.LinqDataSource.Selecting</code> | <code>LinqDataSource.Selecting = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.LinqDataSource.StoreOriginalValuesInViewState</code> | <code>LinqDataSource.StoreOriginalValuesInViewState = &quot;true&quot;</code><br><code>LinqDataSource.StoreOriginalValuesInViewState set = &quot;true&quot;</code> |
| <code>Api.WebControls.LinqDataSource.TableName</code> | <code>LinqDataSource.TableName = &quot;Products&quot;</code><br><code>LinqDataSource.TableName set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.LinqDataSource.Update</code> | <code>LinqDataSource.Update(IDictionary,IDictionary,IDictionary) = &quot;-&gt; System.NotSupportedException: LinqDataSource &#39;ldsProducts&#39; does not support the Update operatio…</code> |
| <code>Api.WebControls.LinqDataSource.UpdateParameters</code> | <code>LinqDataSource.UpdateParameters = &quot;[]&quot;</code> |
| <code>Api.WebControls.LinqDataSource.Updated</code> | <code>LinqDataSource.Updated = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.LinqDataSource.Updating</code> | <code>LinqDataSource.Updating = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.LinqDataSource.Where</code> | <code>LinqDataSource.Where = &quot;&quot;</code><br><code>LinqDataSource.Where set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.LinqDataSource.WhereParameters</code> | <code>LinqDataSource.WhereParameters = &quot;[]&quot;</code> |
| <code>Api.WebControls.QueryableDataSource.#ctor</code> | <code>QueryableDataSource.#ctor = &quot;-&gt; System.MemberAccessException: Cannot create an instance of System.Web.UI.WebControls.QueryableDataSource because it is an abstra…</code> |
| <code>Api.WebControls.QueryableDataSource.CreateQueryableView</code> | <code>QueryableDataSource.CreateQueryableView = LinqDataSourceView {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=true; CanD…</code><br><code>QueryableDataSource.CreateQueryableView after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoS…</code> |
| <code>Api.WebControls.QueryableDataSource.GetView</code> | <code>QueryableDataSource.GetView(String) = &quot;-&gt; System.ArgumentException: LinqDataSource &#39;ldsProducts&#39; only supports a single view named &#39;DefaultView&#39;. You may also l…</code> |
| <code>Api.WebControls.QueryableDataSource.GetViewNames</code> | <code>QueryableDataSource.GetViewNames = &quot;[System.String DefaultView]&quot;</code><br><code>QueryableDataSource.GetViewNames after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=tru…</code> |
| <code>Api.WebControls.QueryableDataSource.LoadViewState</code> | <code>QueryableDataSource.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Web.UI.Pair&#39;.&quot;</code> |
| <code>Api.WebControls.QueryableDataSource.OnInit</code> | <code>QueryableDataSource.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>QueryableDataSource.OnInit(EventArgs) after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSor…</code> |
| <code>Api.WebControls.QueryableDataSource.QueryCreated</code> | <code>QueryableDataSource.QueryCreated = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.QueryableDataSource.RaiseViewChanged</code> | <code>QueryableDataSource.RaiseViewChanged = &quot;ok&quot;</code><br><code>QueryableDataSource.RaiseViewChanged after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort…</code> |
| <code>Api.WebControls.QueryableDataSource.SaveViewState</code> | <code>QueryableDataSource.SaveViewState = Pair {}</code><br><code>QueryableDataSource.SaveViewState after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=tr…</code> |
| <code>Api.WebControls.QueryableDataSource.TrackViewState</code> | <code>QueryableDataSource.TrackViewState = &quot;ok&quot;</code><br><code>QueryableDataSource.TrackViewState after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; AutoSort=t…</code> |
| <code>Api.WebControls.QueryableDataSource.UpdateParameterVales</code> | <code>QueryableDataSource.UpdateParameterVales = &quot;ok&quot;</code><br><code>QueryableDataSource.UpdateParameterVales after = LinqDataSource#ldsProducts {AutoGenerateOrderByClause=false; AutoGenerateWhereClause=false; AutoPage=true; Auto…</code> |

## ② HTML 層

プローブサイトの RenderProbe.aspx で、1 ケース 1 リクエスト（ページライフサイクル全体）で描画したページ全体を比較します。下表はコンポーネント部分（div#probe）、設定時の例外、保存された ViewState の要約です。

| ケース | 設定 | 期待される描画 | 設定時の例外 |
|---|---|---|---|
| <code>.(default)</code> | <code>(nothing set)</code> | <code></code> |  |
| <code>.AppRelativeTemplateSourceDirectory=text</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;;</code> | <code></code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;probe&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=markup</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=japanese</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;;</code> | <code></code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;日本語テキスト&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=empty</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&quot;;</code> | <code></code> |  |
| <code>.AutoGenerateOrderByClause=true</code> | <code>c.AutoGenerateOrderByClause = true;</code> | <code></code> |  |
| <code>.AutoGenerateOrderByClause=false</code> | <code>c.AutoGenerateOrderByClause = false;</code> | <code></code> |  |
| <code>.AutoGenerateWhereClause=true</code> | <code>c.AutoGenerateWhereClause = true;</code> | <code></code> |  |
| <code>.AutoGenerateWhereClause=false</code> | <code>c.AutoGenerateWhereClause = false;</code> | <code></code> |  |
| <code>.AutoPage=true</code> | <code>c.AutoPage = true;</code> | <code></code> |  |
| <code>.AutoPage=false</code> | <code>c.AutoPage = false;</code> | <code></code> |  |
| <code>.AutoSort=true</code> | <code>c.AutoSort = true;</code> | <code></code> |  |
| <code>.AutoSort=false</code> | <code>c.AutoSort = false;</code> | <code></code> |  |
| <code>.ClientIDMode=Inherit</code> | <code>c.ClientIDMode = ClientIDMode.Inherit;</code> | <code></code> | <code>c.ClientIDMode = ClientIDMode.Inherit; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>.ClientIDMode=AutoID</code> | <code>c.ClientIDMode = ClientIDMode.AutoID;</code> | <code></code> | <code>c.ClientIDMode = ClientIDMode.AutoID; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>.ClientIDMode=Predictable</code> | <code>c.ClientIDMode = ClientIDMode.Predictable;</code> | <code></code> | <code>c.ClientIDMode = ClientIDMode.Predictable; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>.ClientIDMode=Static</code> | <code>c.ClientIDMode = ClientIDMode.Static;</code> | <code></code> | <code>c.ClientIDMode = ClientIDMode.Static; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>.ContextTypeName=text</code> | <code>c.ContextTypeName = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.ContextTypeName=markup</code> | <code>c.ContextTypeName = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.ContextTypeName=japanese</code> | <code>c.ContextTypeName = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.ContextTypeName=empty</code> | <code>c.ContextTypeName = &quot;&quot;;</code> | <code></code> |  |
| <code>.ContextTypeName=context</code> | <code>c.ContextTypeName = &quot;WebControlsProbe.ProbeLinqContext&quot;;</code> | <code></code> |  |
| <code>.EnableDelete=true</code> | <code>c.EnableDelete = true;</code> | <code></code> |  |
| <code>.EnableDelete=false</code> | <code>c.EnableDelete = false;</code> | <code></code> |  |
| <code>.EnableInsert=true</code> | <code>c.EnableInsert = true;</code> | <code></code> |  |
| <code>.EnableInsert=false</code> | <code>c.EnableInsert = false;</code> | <code></code> |  |
| <code>.EnableObjectTracking=true</code> | <code>c.EnableObjectTracking = true;</code> | <code></code> |  |
| <code>.EnableObjectTracking=false</code> | <code>c.EnableObjectTracking = false;</code> | <code></code> |  |
| <code>.EnableTheming=true</code> | <code>c.EnableTheming = true;</code> | <code></code> | <code>c.EnableTheming = true; -&gt; System.NotSupportedException: Control of type &#39;LinqDataSource&#39; does not support theming.</code> |
| <code>.EnableTheming=false</code> | <code>c.EnableTheming = false;</code> | <code></code> | <code>c.EnableTheming = false; -&gt; System.NotSupportedException: Control of type &#39;LinqDataSource&#39; does not support theming.</code> |
| <code>.EnableUpdate=true</code> | <code>c.EnableUpdate = true;</code> | <code></code> |  |
| <code>.EnableUpdate=false</code> | <code>c.EnableUpdate = false;</code> | <code></code> |  |
| <code>.EnableViewState=true</code> | <code>c.EnableViewState = true;</code> | <code></code> |  |
| <code>.EnableViewState=false</code> | <code>c.EnableViewState = false;</code> | <code></code> |  |
| <code>.EntityTypeName=text</code> | <code>c.EntityTypeName = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.EntityTypeName=markup</code> | <code>c.EntityTypeName = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.EntityTypeName=japanese</code> | <code>c.EntityTypeName = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.EntityTypeName=empty</code> | <code>c.EntityTypeName = &quot;&quot;;</code> | <code></code> |  |
| <code>.GroupBy=text</code> | <code>c.GroupBy = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.GroupBy=markup</code> | <code>c.GroupBy = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.GroupBy=japanese</code> | <code>c.GroupBy = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.GroupBy=empty</code> | <code>c.GroupBy = &quot;&quot;;</code> | <code></code> |  |
| <code>.ID=text</code> | <code>c.ID = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.ID=markup</code> | <code>c.ID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.ID=japanese</code> | <code>c.ID = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.ID=empty</code> | <code>c.ID = &quot;&quot;;</code> | <code></code> |  |
| <code>.OrderBy=text</code> | <code>c.OrderBy = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.OrderBy=markup</code> | <code>c.OrderBy = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.OrderBy=japanese</code> | <code>c.OrderBy = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.OrderBy=empty</code> | <code>c.OrderBy = &quot;&quot;;</code> | <code></code> |  |
| <code>.OrderGroupsBy=text</code> | <code>c.OrderGroupsBy = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.OrderGroupsBy=markup</code> | <code>c.OrderGroupsBy = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.OrderGroupsBy=japanese</code> | <code>c.OrderGroupsBy = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.OrderGroupsBy=empty</code> | <code>c.OrderGroupsBy = &quot;&quot;;</code> | <code></code> |  |
| <code>.Select=text</code> | <code>c.Select = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.Select=markup</code> | <code>c.Select = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.Select=japanese</code> | <code>c.Select = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.Select=empty</code> | <code>c.Select = &quot;&quot;;</code> | <code></code> |  |
| <code>.SkinID=text</code> | <code>c.SkinID = &quot;probe&quot;;</code> | <code></code> | <code>c.SkinID = &quot;probe&quot;; -&gt; System.NotSupportedException: Control of type &#39;LinqDataSource&#39; does not support theming.</code> |
| <code>.SkinID=markup</code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.NotSupportedException: Control of type &#39;LinqDataSource&#39; does not support theming.</code> |
| <code>.SkinID=japanese</code> | <code>c.SkinID = &quot;日本語テキスト&quot;;</code> | <code></code> | <code>c.SkinID = &quot;日本語テキスト&quot;; -&gt; System.NotSupportedException: Control of type &#39;LinqDataSource&#39; does not support theming.</code> |
| <code>.SkinID=empty</code> | <code>c.SkinID = &quot;&quot;;</code> | <code></code> | <code>c.SkinID = &quot;&quot;; -&gt; System.NotSupportedException: Control of type &#39;LinqDataSource&#39; does not support theming.</code> |
| <code>.StoreOriginalValuesInViewState=true</code> | <code>c.StoreOriginalValuesInViewState = true;</code> | <code></code> |  |
| <code>.StoreOriginalValuesInViewState=false</code> | <code>c.StoreOriginalValuesInViewState = false;</code> | <code></code> |  |
| <code>.TableName=text</code> | <code>c.TableName = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.TableName=markup</code> | <code>c.TableName = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.TableName=japanese</code> | <code>c.TableName = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.TableName=empty</code> | <code>c.TableName = &quot;&quot;;</code> | <code></code> |  |
| <code>.TableName=Products</code> | <code>c.TableName = &quot;Products&quot;;</code> | <code></code> |  |
| <code>.ValidateRequestMode=Inherit</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Inherit;</code> | <code></code> |  |
| <code>.ValidateRequestMode=Disabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Disabled;</code> | <code></code> |  |
| <code>.ValidateRequestMode=Enabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Enabled;</code> | <code></code> |  |
| <code>.ViewStateMode=Inherit</code> | <code>c.ViewStateMode = ViewStateMode.Inherit;</code> | <code></code> |  |
| <code>.ViewStateMode=Enabled</code> | <code>c.ViewStateMode = ViewStateMode.Enabled;</code> | <code></code> |  |
| <code>.ViewStateMode=Disabled</code> | <code>c.ViewStateMode = ViewStateMode.Disabled;</code> | <code></code> |  |
| <code>.Visible=true</code> | <code>c.Visible = true;</code> | <code></code> | <code>c.Visible = true; -&gt; System.NotSupportedException: Control of type &#39;LinqDataSource&#39; is a non-visual control and does not support setting the Visible property.</code> |
| <code>.Visible=false</code> | <code>c.Visible = false;</code> | <code></code> | <code>c.Visible = false; -&gt; System.NotSupportedException: Control of type &#39;LinqDataSource&#39; is a non-visual control and does not support setting the Visible property.</code> |
| <code>.Where=text</code> | <code>c.Where = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.Where=markup</code> | <code>c.Where = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.Where=japanese</code> | <code>c.Where = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.Where=empty</code> | <code>c.Where = &quot;&quot;;</code> | <code></code> |  |
| <code>.Where=text+OrderBy=text</code> | <code>c.Where = &quot;probe&quot;; c.OrderBy = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.AutoPage=false+AutoSort=false</code> | <code>c.AutoPage = false; c.AutoSort = false;</code> | <code></code> |  |
| <code>.EnableUpdate=true+EnableDelete=true+EnableInsert=true</code> | <code>c.EnableUpdate = true; c.EnableDelete = true; c.EnableInsert = true;</code> | <code></code> |  |
| <code>.StoreOriginalValuesInViewState=false+Select=text</code> | <code>c.StoreOriginalValuesInViewState = false; c.Select = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.Visible=false+EnableViewState=false</code> | <code>c.Visible = false; c.EnableViewState = false;</code> | <code></code> | <code>c.Visible = false; -&gt; System.NotSupportedException: Control of type &#39;LinqDataSource&#39; is a non-visual control and does not support setting the Visible property.</code> |
| <code>[naming container].(default)</code> | <code>[naming container] (nothing set)</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=AutoID</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> | <code>c.ClientIDMode = ClientIDMode.AutoID; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>[naming container].ClientIDMode=Static</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> | <code>c.ClientIDMode = ClientIDMode.Static; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>[naming container].ClientIDMode=Predictable</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> | <code>c.ClientIDMode = ClientIDMode.Predictable; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>[naming container].ClientIDMode=Inherit</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> | <code>c.ClientIDMode = ClientIDMode.Inherit; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>[naming container].ID=text</code> | <code>[naming container] c.ID = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |

## ③ シナリオ層

ブラウザ（Playwright）で LinqDataSourceScenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。

| # | 操作 | 対象 | 値 |
|---|---|---|---|
| 1 | goto | /DataSourceScenario.aspx?reset=1 | <code></code> |
| 2 | snapshot |  | **LinqDataSource [render][databind][dynamic] first request: grids bound to each data source, the Access data source selected in code, ones made in code** |
| 3 | clicktextin | gvObject | <code>Edit</code> |
| 4 | evaluate | gvObject | <code>const inputs = e.querySelectorAll(&quot;input[type=text]&quot;); inputs[0].value = &quot;Pen (edited)&quot;;</code> |
| 5 | clicktextin | gvObject | <code>Update</code> |
| 6 | snapshot |  | **LinqDataSource [viewstate] an object data source&#39;s row updated** |
| 7 | clicktextin | gvObject | <code>Delete</code> |
| 8 | snapshot |  | **LinqDataSource [viewstate] an object data source&#39;s row deleted** |
| 9 | clicktextin | gvSql | <code>Name</code> |
| 10 | snapshot |  | **LinqDataSource [viewstate] a SQL data source sorted** |
| 11 | clicktextin | gvSql | <code>Edit</code> |
| 12 | evaluate | gvSql | <code>const inputs = e.querySelectorAll(&quot;input[type=text]&quot;); inputs[0].value = &quot;Bag (edited)&quot;; inputs[1].value = &quot;25.5&quot;;</code> |
| 13 | clicktextin | gvSql | <code>Update</code> |
| 14 | snapshot |  | **LinqDataSource [viewstate] a SQL data source&#39;s row updated (its update command&#39;s parameters)** |
| 15 | clicktextin | gvSql | <code>Delete</code> |
| 16 | snapshot |  | **LinqDataSource [viewstate] a SQL data source&#39;s row deleted** |
| 17 | evaluate | dvSqlInsert | <code>const inputs = e.querySelectorAll(&quot;input[type=text]&quot;); inputs[0].value = &quot;Z9&quot;; inputs[1].value = &quot;Zipper&quot;; inputs[2].value = &quot;9.5&quot;;</code> |
| 18 | clicktextin | dvSqlInsert | <code>Insert</code> |
| 19 | snapshot |  | **LinqDataSource [viewstate] a SQL data source&#39;s row inserted** |
| 20 | clicktextin | gvLinq | <code>2</code> |
| 21 | snapshot |  | **LinqDataSource [viewstate] a LINQ data source paged** |
| 22 | clicktextin | gvLinq | <code>Price</code> |
| 23 | snapshot |  | **LinqDataSource [viewstate] a LINQ data source sorted** |
| 24 | fill | txtMinimum | <code>100</code> |
| 25 | click | btnSubmit | <code></code> |
| 26 | snapshot |  | **LinqDataSource [viewstate] a LINQ data source&#39;s where parameter from a text box** |
| 27 | fill | txtSearch | <code>a</code> |
| 28 | click | btnSubmit | <code></code> |
| 29 | snapshot |  | **LinqDataSource [viewstate] a query extender&#39;s search from a text box** |
| 30 | click | btnModel | <code></code> |
| 31 | snapshot |  | **LinqDataSource [viewstate] a model error shown** |
| 32 | fill | txtModelName | <code>named</code> |
| 33 | click | btnModel | <code></code> |
| 34 | snapshot |  | **LinqDataSource [viewstate] no model error** |
| 35 | clicktextin | gvDynamic | <code>Name</code> |
| 36 | snapshot |  | **LinqDataSource [dynamic][viewstate] a grid sorted by a data source made in code** |
| 37 | click | btnShow | <code></code> |
| 38 | snapshot |  | **LinqDataSource [visible][viewstate] a hidden grid bound to a data source shown** |

<details><summary>LinqDataSource [render][databind][dynamic] first request: grids bound to each data source, the Access data source selected in code, ones made in code（期待される本文）</summary>

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

<details><summary>LinqDataSource [viewstate] an object data source&#39;s row updated（期待される本文）</summary>

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

<details><summary>LinqDataSource [viewstate] an object data source&#39;s row deleted（期待される本文）</summary>

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

<details><summary>LinqDataSource [viewstate] a SQL data source sorted（期待される本文）</summary>

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

<details><summary>LinqDataSource [viewstate] a SQL data source&#39;s row updated (its update command&#39;s parameters)（期待される本文）</summary>

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

<details><summary>LinqDataSource [viewstate] a SQL data source&#39;s row deleted（期待される本文）</summary>

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

<details><summary>LinqDataSource [viewstate] a SQL data source&#39;s row inserted（期待される本文）</summary>

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

<details><summary>LinqDataSource [viewstate] a LINQ data source paged（期待される本文）</summary>

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

<details><summary>LinqDataSource [viewstate] a LINQ data source sorted（期待される本文）</summary>

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

<details><summary>LinqDataSource [viewstate] a LINQ data source&#39;s where parameter from a text box（期待される本文）</summary>

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

<details><summary>LinqDataSource [viewstate] a query extender&#39;s search from a text box（期待される本文）</summary>

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

<details><summary>LinqDataSource [viewstate] a model error shown（期待される本文）</summary>

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

<details><summary>LinqDataSource [viewstate] no model error（期待される本文）</summary>

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

<details><summary>LinqDataSource [dynamic][viewstate] a grid sorted by a data source made in code（期待される本文）</summary>

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

<details><summary>LinqDataSource [visible][viewstate] a hidden grid bound to a data source shown（期待される本文）</summary>

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

