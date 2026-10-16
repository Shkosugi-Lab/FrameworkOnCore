# BulletedList テストケース一覧

変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。
このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。

- 正解データの採取: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0
- ① API 層: 243 ケース（252 メンバー）
- ② HTML 層: 237 ケース
- ③ シナリオ層: 6 スナップショット

## 機構マトリクス

型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。

| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |
|---|---|---|---|
| プロパティ → HTML 描画 | すべてのコントロール | 199 | 1 |
| ViewState の保存と復元 | Control | 6 | 1 |
| Visible=false（描画されず状態は保持） | Control.Visible | 2 | 1 |
| 名前付けコンテナと ClientIDMode | Control.ClientIDMode | 7 | 0 |
| テーマとスキン（SkinID / EnableTheming） | テーマを付けられる型（[Themeable]） | 4 | 0 |
| 動的に生成したコントロールのライフサイクル | Control | 0 | 1 |
| データバインド（DataBind / <%# %>） | Control.DataBind | 0 | 1 |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | WebControl.Enabled、HtmlControl.Disabled | 8 | 0 |
| ポストバックイベント | IPostBackEventHandler | 0 | 4 |
| AutoPostBack（クライアントスクリプトによるポストバック） | AutoPostBack プロパティ | 1 | 0 |
| CausesValidation / ValidationGroup | CausesValidation プロパティ | 8 | 1 |
| 項目（ListItem の Selected・Enabled・Text・Value と ViewState） | ListControl、HtmlSelect | 20 | 2 |
| データソース連携（DataSource・DataTextField など） | BaseDataBoundControl、HtmlSelect | 10 | 0 |

該当しない機構: ポストバックデータの取り込み、変更イベント（*Changed、ServerChange）、ReadOnly（ポストされた値を無視）、リクエスト検証（ValidateRequestMode）、コマンドイベントと親へのバブル（CommandName）、送信ボタン／スクリプトによるポストバック（UseSubmitBehavior）、ブラウザ側のクリック処理（OnClientClick）、クロスページポストバック（PostBackUrl）、既定のボタン（Panel の DefaultButton）、検証対象（サーバー側の検証）、検証対象（ブラウザ側の検証）、双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値）、複数選択、繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）、ポスト値のイベント検証（一覧にない値・無効な項目の拒否）、サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）、ブラウザ側の検証スクリプト（EnableClientScript）、検証結果のまとめ（ValidationSummary）、ページング、並べ替え、項目の挿入、行・項目の編集（編集・更新・取り消し・削除）、行・項目の選択、行・項目のコマンド（RowCommand・ItemCommand へのバブル）、データキー（DataKeyNames・DataKeys）、データソースコントロールとの連携（DataSourceID）、テンプレート、クライアントコールバック

## ① API 層

BulletedList とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の BulletedList インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。

| ケース | 期待値（.NET Framework） |
|---|---|
| <code>Api.Control.#ctor</code> | <code>Control.#ctor = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMode=In…</code> |
| <code>Api.Control.Adapter</code> | <code>Control.Adapter = null</code> |
| <code>Api.Control.AddParsedSubObject</code> | <code>Control.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>Control.AddParsedSubObject(Object) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color […</code> |
| <code>Api.Control.AddedControl</code> | <code>Control.AddedControl(Control,Int32) = &quot;ok&quot;</code><br><code>Control.AddedControl(Control,Int32) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color …</code> |
| <code>Api.Control.AppRelativeTemplateSourceDirectory</code> | <code>Control.AppRelativeTemplateSourceDirectory = &quot;&quot;</code><br><code>Control.AppRelativeTemplateSourceDirectory set = &quot;-&gt; System.ArgumentException: The relative virtual path &#39;Abc 123&#39; is not allowed here.&quot;</code> |
| <code>Api.Control.ApplyStyleSheetSkin</code> | <code>Control.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>Control.ApplyStyleSheetSkin(Page) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [E…</code> |
| <code>Api.Control.BeginRenderTracing</code> | <code>Control.BeginRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.BeginRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.BeginRenderTracing(TextWriter,Object) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000…</code> |
| <code>Api.Control.BindingContainer</code> | <code>Control.BindingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Clien…</code> |
| <code>Api.Control.BuildProfileTree</code> | <code>Control.BuildProfileTree(String,Boolean) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.ChildControlsCreated</code> | <code>Control.ChildControlsCreated = &quot;false&quot;</code><br><code>Control.ChildControlsCreated set = &quot;ok&quot;</code> |
| <code>Api.Control.ClearCachedClientID</code> | <code>Control.ClearCachedClientID = &quot;ok&quot;</code><br><code>Control.ClearCachedClientID after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty];…</code> |
| <code>Api.Control.ClearChildControlState</code> | <code>Control.ClearChildControlState = &quot;ok&quot;</code><br><code>Control.ClearChildControlState after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empt…</code> |
| <code>Api.Control.ClearChildState</code> | <code>Control.ClearChildState = &quot;ok&quot;</code><br><code>Control.ClearChildState after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; Bor…</code> |
| <code>Api.Control.ClearChildViewState</code> | <code>Control.ClearChildViewState = &quot;ok&quot;</code><br><code>Control.ClearChildViewState after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty];…</code> |
| <code>Api.Control.ClearEffectiveClientIDMode</code> | <code>Control.ClearEffectiveClientIDMode = &quot;ok&quot;</code><br><code>Control.ClearEffectiveClientIDMode after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color […</code> |
| <code>Api.Control.ClientID</code> | <code>Control.ClientID = &quot;bltSize&quot;</code> |
| <code>Api.Control.ClientIDMode</code> | <code>Control.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>Control.ClientIDMode set = &quot;ClientIDMode.AutoID&quot;</code> |
| <code>Api.Control.ClientIDSeparator</code> | <code>Control.ClientIDSeparator = &quot;&#39;_&#39;&quot;</code> |
| <code>Api.Control.Context</code> | <code>Control.Context = null</code> |
| <code>Api.Control.Controls</code> | <code>Control.Controls = ControlCollection []</code> |
| <code>Api.Control.CreateChildControls</code> | <code>Control.CreateChildControls = &quot;ok&quot;</code><br><code>Control.CreateChildControls after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty];…</code> |
| <code>Api.Control.CreateControlCollection</code> | <code>Control.CreateControlCollection = ControlCollection []</code><br><code>Control.CreateControlCollection after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Emp…</code> |
| <code>Api.Control.DataBind</code> | <code>Control.DataBind = &quot;ok&quot;</code><br><code>Control.DataBind after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderColo…</code><br><code>Control.DataBind(Boolean) = &quot;ok&quot;</code><br><code>Control.DataBind(Boolean) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; B…</code> |
| <code>Api.Control.DataBindChildren</code> | <code>Control.DataBindChildren = &quot;ok&quot;</code><br><code>Control.DataBindChildren after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; Bo…</code> |
| <code>Api.Control.DataBinding</code> | <code>Control.DataBinding = &quot;added and removed&quot;</code> |
| <code>Api.Control.DataItemContainer</code> | <code>Control.DataItemContainer = null</code> |
| <code>Api.Control.DataKeysContainer</code> | <code>Control.DataKeysContainer = null</code> |
| <code>Api.Control.DesignMode</code> | <code>Control.DesignMode = &quot;false&quot;</code> |
| <code>Api.Control.Dispose</code> | <code>Control.Dispose = &quot;ok&quot;</code><br><code>Control.Dispose after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderColor…</code> |
| <code>Api.Control.Disposed</code> | <code>Control.Disposed = &quot;added and removed&quot;</code> |
| <code>Api.Control.EnableTheming</code> | <code>Control.EnableTheming = &quot;true&quot;</code><br><code>Control.EnableTheming set = &quot;true&quot;</code> |
| <code>Api.Control.EnableViewState</code> | <code>Control.EnableViewState = &quot;true&quot;</code><br><code>Control.EnableViewState set = &quot;true&quot;</code> |
| <code>Api.Control.EndRenderTracing</code> | <code>Control.EndRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.EndRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.EndRenderTracing(TextWriter,Object) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #0000000…</code> |
| <code>Api.Control.EnsureChildControls</code> | <code>Control.EnsureChildControls = &quot;ok&quot;</code><br><code>Control.EnsureChildControls after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty];…</code> |
| <code>Api.Control.EnsureID</code> | <code>Control.EnsureID = &quot;ok&quot;</code><br><code>Control.EnsureID after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderColo…</code> |
| <code>Api.Control.Events</code> | <code>Control.Events = &quot;System.ComponentModel.EventHandlerList&quot;</code> |
| <code>Api.Control.FindControl</code> | <code>Control.FindControl(String) = Label#lblTitle {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code><br><code>Control.FindControl(String) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty];…</code><br><code>Control.FindControl(String,Int32) = null</code><br><code>Control.FindControl(String,Int32) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [E…</code> |
| <code>Api.Control.Focus</code> | <code>Control.Focus = &quot;-&gt; System.InvalidOperationException: A form tag with runat=server must exist on the Page to use SetFocus() or the Focus property.&quot;</code> |
| <code>Api.Control.GetDesignModeState</code> | <code>Control.GetDesignModeState = &quot;[]&quot;</code><br><code>Control.GetDesignModeState after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; …</code> |
| <code>Api.Control.GetRouteUrl</code> | <code>Control.GetRouteUrl(Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.GetUniqueIDRelativeTo</code> | <code>Control.GetUniqueIDRelativeTo(Control) = &quot;bltSize&quot;</code><br><code>Control.GetUniqueIDRelativeTo(Control) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Col…</code> |
| <code>Api.Control.HasChildViewState</code> | <code>Control.HasChildViewState = &quot;false&quot;</code> |
| <code>Api.Control.HasControls</code> | <code>Control.HasControls = &quot;false&quot;</code><br><code>Control.HasControls after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderC…</code> |
| <code>Api.Control.HasEvents</code> | <code>Control.HasEvents = &quot;false&quot;</code><br><code>Control.HasEvents after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderCol…</code> |
| <code>Api.Control.ID</code> | <code>Control.ID = &quot;bltSize&quot;</code><br><code>Control.ID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.IdSeparator</code> | <code>Control.IdSeparator = &quot;&#39;$&#39;&quot;</code> |
| <code>Api.Control.Init</code> | <code>Control.Init = &quot;added and removed&quot;</code> |
| <code>Api.Control.IsChildControlStateCleared</code> | <code>Control.IsChildControlStateCleared = &quot;false&quot;</code> |
| <code>Api.Control.IsLiteralContent</code> | <code>Control.IsLiteralContent = &quot;false&quot;</code><br><code>Control.IsLiteralContent after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; Bo…</code> |
| <code>Api.Control.IsTrackingViewState</code> | <code>Control.IsTrackingViewState = &quot;false&quot;</code> |
| <code>Api.Control.IsViewStateEnabled</code> | <code>Control.IsViewStateEnabled = &quot;true&quot;</code> |
| <code>Api.Control.Load</code> | <code>Control.Load = &quot;added and removed&quot;</code> |
| <code>Api.Control.LoadControlState</code> | <code>Control.LoadControlState(Object) = &quot;ok&quot;</code><br><code>Control.LoadControlState(Object) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Em…</code> |
| <code>Api.Control.LoadViewState</code> | <code>Control.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Web.UI.Triplet&#39;.&quot;</code> |
| <code>Api.Control.LoadViewStateByID</code> | <code>Control.LoadViewStateByID = &quot;false&quot;</code> |
| <code>Api.Control.MapPathSecure</code> | <code>Control.MapPathSecure(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.NamingContainer</code> | <code>Control.NamingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code> |
| <code>Api.Control.OnBubbleEvent</code> | <code>Control.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Control.OnBubbleEvent(Object,EventArgs) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Co…</code> |
| <code>Api.Control.OnDataBinding</code> | <code>Control.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnDataBinding(EventArgs) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Em…</code> |
| <code>Api.Control.OnInit</code> | <code>Control.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnInit(EventArgs) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; B…</code> |
| <code>Api.Control.OnLoad</code> | <code>Control.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnLoad(EventArgs) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; B…</code> |
| <code>Api.Control.OnPreRender</code> | <code>Control.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnPreRender(EventArgs) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empt…</code> |
| <code>Api.Control.OnUnload</code> | <code>Control.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnUnload(EventArgs) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty];…</code> |
| <code>Api.Control.OpenFile</code> | <code>Control.OpenFile(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.Page</code> | <code>Control.Page = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=&quot;&quot;; …</code><br><code>Control.Page set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=…</code> |
| <code>Api.Control.Parent</code> | <code>Control.Parent = HtmlForm#form1 {Action=&quot;&quot;; ClientID=&quot;form1&quot;; ClientIDMode=Inherit; DefaultButton=&quot;&quot;; DefaultFocus=&quot;&quot;; Disabled=false; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.PreRender</code> | <code>Control.PreRender = &quot;added and removed&quot;</code> |
| <code>Api.Control.RaiseBubbleEvent</code> | <code>Control.RaiseBubbleEvent(Object,EventArgs) = &quot;ok&quot;</code><br><code>Control.RaiseBubbleEvent(Object,EventArgs) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000…</code> |
| <code>Api.Control.RemovedControl</code> | <code>Control.RemovedControl(Control) = &quot;ok&quot;</code><br><code>Control.RemovedControl(Control) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Emp…</code> |
| <code>Api.Control.Render</code> | <code>Control.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.Render(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;ul id=&quot;bltSize&quot;&gt;</code><br><code>  &#124; 	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;</code><br><code>  &#124; &lt;/ul&gt;</code><br><code>Control.Render(HtmlTextWriter) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empt…</code> |
| <code>Api.Control.RenderChildren</code> | <code>Control.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderChildren(HtmlTextWriter) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Col…</code> |
| <code>Api.Control.RenderControl</code> | <code>Control.RenderControl(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;ul id=&quot;bltSize&quot;&gt;</code><br><code>  &#124; 	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;</code><br><code>  &#124; &lt;/ul&gt;</code><br><code>Control.RenderControl(HtmlTextWriter) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Colo…</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) writer:</code><br><code>  &#124; &lt;ul id=&quot;bltSize&quot;&gt;</code><br><code>  &#124; 	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;</code><br><code>  &#124; &lt;/ul&gt;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackCo…</code> |
| <code>Api.Control.RenderingCompatibility</code> | <code>Control.RenderingCompatibility = &quot;System.Version 4.0&quot;</code><br><code>Control.RenderingCompatibility set = &quot;System.Version 4.0&quot;</code> |
| <code>Api.Control.ResolveAdapter</code> | <code>Control.ResolveAdapter = null</code><br><code>Control.ResolveAdapter after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; Bord…</code> |
| <code>Api.Control.ResolveClientUrl</code> | <code>Control.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveClientUrl(String) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Em…</code> |
| <code>Api.Control.ResolveUrl</code> | <code>Control.ResolveUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveUrl(String) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; …</code> |
| <code>Api.Control.SaveControlState</code> | <code>Control.SaveControlState = null</code><br><code>Control.SaveControlState after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; Bo…</code> |
| <code>Api.Control.SaveViewState</code> | <code>Control.SaveViewState = Triplet {}</code><br><code>Control.SaveViewState after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; Borde…</code> |
| <code>Api.Control.SetDesignModeState</code> | <code>Control.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>Control.SetDesignModeState(IDictionary) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Co…</code> |
| <code>Api.Control.SetRenderMethodDelegate</code> | <code>Control.SetRenderMethodDelegate(RenderMethod) = &quot;ok&quot;</code><br><code>Control.SetRenderMethodDelegate(RenderMethod) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000…</code> |
| <code>Api.Control.SetTraceData</code> | <code>Control.SetTraceData(Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color …</code><br><code>Control.SetTraceData(Object,Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object,Object) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000…</code> |
| <code>Api.Control.Site</code> | <code>Control.Site = null</code><br><code>Control.Site set = null</code> |
| <code>Api.Control.SkinID</code> | <code>Control.SkinID = &quot;&quot;</code><br><code>Control.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.TemplateControl</code> | <code>Control.TemplateControl = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code><br><code>Control.TemplateControl set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Cl…</code> |
| <code>Api.Control.TemplateSourceDirectory</code> | <code>Control.TemplateSourceDirectory = &quot;&quot;</code> |
| <code>Api.Control.TrackViewState</code> | <code>Control.TrackViewState = &quot;ok&quot;</code><br><code>Control.TrackViewState after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; Bord…</code> |
| <code>Api.Control.UniqueID</code> | <code>Control.UniqueID = &quot;bltSize&quot;</code> |
| <code>Api.Control.Unload</code> | <code>Control.Unload = &quot;added and removed&quot;</code> |
| <code>Api.Control.ValidateRequestMode</code> | <code>Control.ValidateRequestMode = &quot;ValidateRequestMode.Inherit&quot;</code><br><code>Control.ValidateRequestMode set = &quot;ValidateRequestMode.Disabled&quot;</code> |
| <code>Api.Control.ViewState</code> | <code>Control.ViewState = StateBag []</code> |
| <code>Api.Control.ViewStateIgnoresCase</code> | <code>Control.ViewStateIgnoresCase = &quot;false&quot;</code> |
| <code>Api.Control.ViewStateMode</code> | <code>Control.ViewStateMode = &quot;ViewStateMode.Inherit&quot;</code><br><code>Control.ViewStateMode set = &quot;ViewStateMode.Enabled&quot;</code> |
| <code>Api.Control.Visible</code> | <code>Control.Visible = &quot;true&quot;</code><br><code>Control.Visible set = &quot;true&quot;</code> |
| <code>Api.IAttributeAccessor.GetAttribute</code> | <code>IAttributeAccessor.GetAttribute(String) = null</code><br><code>IAttributeAccessor.GetAttribute(String) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Co…</code> |
| <code>Api.IAttributeAccessor.SetAttribute</code> | <code>IAttributeAccessor.SetAttribute(String,String) = &quot;ok&quot;</code><br><code>IAttributeAccessor.SetAttribute(String,String) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #0000…</code> |
| <code>Api.IControlBuilderAccessor.ControlBuilder</code> | <code>IControlBuilderAccessor.ControlBuilder = null</code> |
| <code>Api.IControlDesignerAccessor.GetDesignModeState</code> | <code>IControlDesignerAccessor.GetDesignModeState = &quot;[]&quot;</code><br><code>IControlDesignerAccessor.GetDesignModeState after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #0000000…</code> |
| <code>Api.IControlDesignerAccessor.SetDesignModeState</code> | <code>IControlDesignerAccessor.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetDesignModeState(IDictionary) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=C…</code> |
| <code>Api.IControlDesignerAccessor.SetOwnerControl</code> | <code>IControlDesignerAccessor.SetOwnerControl(Control) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetOwnerControl(Control) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #0…</code> |
| <code>Api.IControlDesignerAccessor.UserData</code> | <code>IControlDesignerAccessor.UserData = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.DataBindings</code> | <code>IDataBindingsAccessor.DataBindings = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.HasDataBindings</code> | <code>IDataBindingsAccessor.HasDataBindings = &quot;false&quot;</code> |
| <code>Api.IEditableTextControl.TextChanged</code> | <code>IEditableTextControl.TextChanged = &quot;added and removed&quot;</code> |
| <code>Api.IExpressionsAccessor.Expressions</code> | <code>IExpressionsAccessor.Expressions = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.HasExpressions</code> | <code>IExpressionsAccessor.HasExpressions = &quot;false&quot;</code> |
| <code>Api.IParserAccessor.AddParsedSubObject</code> | <code>IParserAccessor.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>IParserAccessor.AddParsedSubObject(Object) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000…</code> |
| <code>Api.IPostBackEventHandler.RaisePostBackEvent</code> | <code>IPostBackEventHandler.RaisePostBackEvent(String) = &quot;-&gt; System.FormatException: (the runtime&#39;s parse error)&quot;</code> |
| <code>Api.ITextControl.Text</code> | <code>ITextControl.Text = &quot;M&quot;</code><br><code>ITextControl.Text set = &quot;-&gt; System.NotSupportedException: Setting the Text property of BulletedList is not supported.&quot;</code> |
| <code>Api.IUrlResolutionService.ResolveClientUrl</code> | <code>IUrlResolutionService.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>IUrlResolutionService.ResolveClientUrl(String) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #0000…</code> |
| <code>Api.WebControls.BaseDataBoundControl.#ctor</code> | <code>BaseDataBoundControl.#ctor = &quot;-&gt; System.MemberAccessException: Cannot create an instance of System.Web.UI.WebControls.BaseDataBoundControl because it is an abst…</code> |
| <code>Api.WebControls.BaseDataBoundControl.ConfirmInitState</code> | <code>BaseDataBoundControl.ConfirmInitState = &quot;ok&quot;</code><br><code>BaseDataBoundControl.ConfirmInitState after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Colo…</code> |
| <code>Api.WebControls.BaseDataBoundControl.DataBind</code> | <code>BaseDataBoundControl.DataBind = &quot;ok&quot;</code><br><code>BaseDataBoundControl.DataBind after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty…</code> |
| <code>Api.WebControls.BaseDataBoundControl.DataBound</code> | <code>BaseDataBoundControl.DataBound = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.BaseDataBoundControl.DataSource</code> | <code>BaseDataBoundControl.DataSource = null</code><br><code>BaseDataBoundControl.DataSource set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.BaseDataBoundControl.DataSourceID</code> | <code>BaseDataBoundControl.DataSourceID = &quot;&quot;</code><br><code>BaseDataBoundControl.DataSourceID set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.BaseDataBoundControl.EnsureDataBound</code> | <code>BaseDataBoundControl.EnsureDataBound = &quot;ok&quot;</code><br><code>BaseDataBoundControl.EnsureDataBound after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color…</code> |
| <code>Api.WebControls.BaseDataBoundControl.Initialized</code> | <code>BaseDataBoundControl.Initialized = &quot;false&quot;</code> |
| <code>Api.WebControls.BaseDataBoundControl.IsBoundUsingDataSourceID</code> | <code>BaseDataBoundControl.IsBoundUsingDataSourceID = &quot;false&quot;</code> |
| <code>Api.WebControls.BaseDataBoundControl.IsDataBindingAutomatic</code> | <code>BaseDataBoundControl.IsDataBindingAutomatic = &quot;false&quot;</code> |
| <code>Api.WebControls.BaseDataBoundControl.IsUsingModelBinders</code> | <code>BaseDataBoundControl.IsUsingModelBinders = &quot;false&quot;</code> |
| <code>Api.WebControls.BaseDataBoundControl.OnDataBound</code> | <code>BaseDataBoundControl.OnDataBound(EventArgs) = &quot;ok&quot;</code><br><code>BaseDataBoundControl.OnDataBound(EventArgs) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #0000000…</code> |
| <code>Api.WebControls.BaseDataBoundControl.OnDataPropertyChanged</code> | <code>BaseDataBoundControl.OnDataPropertyChanged = &quot;ok&quot;</code><br><code>BaseDataBoundControl.OnDataPropertyChanged after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000…</code> |
| <code>Api.WebControls.BaseDataBoundControl.OnInit</code> | <code>BaseDataBoundControl.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>BaseDataBoundControl.OnInit(EventArgs) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Col…</code> |
| <code>Api.WebControls.BaseDataBoundControl.OnPagePreLoad</code> | <code>BaseDataBoundControl.OnPagePreLoad(Object,EventArgs) = &quot;ok&quot;</code><br><code>BaseDataBoundControl.OnPagePreLoad(Object,EventArgs) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color…</code> |
| <code>Api.WebControls.BaseDataBoundControl.OnPreRender</code> | <code>BaseDataBoundControl.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>BaseDataBoundControl.OnPreRender(EventArgs) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #0000000…</code> |
| <code>Api.WebControls.BaseDataBoundControl.PerformSelect</code> | <code>BaseDataBoundControl.PerformSelect = &quot;ok&quot;</code><br><code>BaseDataBoundControl.PerformSelect after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color […</code> |
| <code>Api.WebControls.BaseDataBoundControl.RequiresDataBinding</code> | <code>BaseDataBoundControl.RequiresDataBinding = &quot;false&quot;</code><br><code>BaseDataBoundControl.RequiresDataBinding set = &quot;ok&quot;</code> |
| <code>Api.WebControls.BaseDataBoundControl.SupportsDisabledAttribute</code> | <code>BaseDataBoundControl.SupportsDisabledAttribute = &quot;false&quot;</code> |
| <code>Api.WebControls.BaseDataBoundControl.ValidateDataSource</code> | <code>BaseDataBoundControl.ValidateDataSource(Object) = &quot;ok&quot;</code><br><code>BaseDataBoundControl.ValidateDataSource(Object) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #000…</code> |
| <code>Api.WebControls.BulletedList.#ctor</code> | <code>BulletedList.#ctor = BulletedList# {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00…</code> |
| <code>Api.WebControls.BulletedList.AddAttributesToRender</code> | <code>BulletedList.AddAttributesToRender(HtmlTextWriter) = &quot;ok&quot;</code><br><code>BulletedList.AddAttributesToRender(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>BulletedList.AddAttributesToRender(HtmlTextWriter) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #…</code> |
| <code>Api.WebControls.BulletedList.AutoPostBack</code> | <code>BulletedList.AutoPostBack = &quot;false&quot;</code><br><code>BulletedList.AutoPostBack set = &quot;-&gt; System.NotSupportedException: Setting the AutoPostBack property of System.Web.UI.WebControls.BulletedList is not supported.&quot;</code> |
| <code>Api.WebControls.BulletedList.BulletImageUrl</code> | <code>BulletedList.BulletImageUrl = &quot;&quot;</code><br><code>BulletedList.BulletImageUrl set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.BulletedList.BulletStyle</code> | <code>BulletedList.BulletStyle = &quot;BulletStyle.NotSet&quot;</code><br><code>BulletedList.BulletStyle set = &quot;BulletStyle.Numbered&quot;</code> |
| <code>Api.WebControls.BulletedList.Click</code> | <code>BulletedList.Click = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.BulletedList.Controls</code> | <code>BulletedList.Controls = ControlCollection []</code> |
| <code>Api.WebControls.BulletedList.DisplayMode</code> | <code>BulletedList.DisplayMode = &quot;BulletedListDisplayMode.Text&quot;</code><br><code>BulletedList.DisplayMode set = &quot;BulletedListDisplayMode.HyperLink&quot;</code> |
| <code>Api.WebControls.BulletedList.FirstBulletNumber</code> | <code>BulletedList.FirstBulletNumber = &quot;Int32 1&quot;</code><br><code>BulletedList.FirstBulletNumber set = &quot;Int32 1&quot;</code> |
| <code>Api.WebControls.BulletedList.OnClick</code> | <code>BulletedList.OnClick(BulletedListEventArgs) = &quot;ok&quot;</code><br><code>BulletedList.OnClick(BulletedListEventArgs) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #0000000…</code> |
| <code>Api.WebControls.BulletedList.RaisePostBackEvent</code> | <code>BulletedList.RaisePostBackEvent(String) = &quot;-&gt; System.FormatException: (the runtime&#39;s parse error)&quot;</code> |
| <code>Api.WebControls.BulletedList.Render</code> | <code>BulletedList.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>BulletedList.Render(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;ul id=&quot;bltSize&quot;&gt;</code><br><code>  &#124; 	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;</code><br><code>  &#124; &lt;/ul&gt;</code><br><code>BulletedList.Render(HtmlTextWriter) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color …</code> |
| <code>Api.WebControls.BulletedList.RenderBulletText</code> | <code>BulletedList.RenderBulletText(ListItem,Int32,HtmlTextWriter) = &quot;ok&quot;</code><br><code>BulletedList.RenderBulletText(ListItem,Int32,HtmlTextWriter) writer:</code><br><code>  &#124; Medium</code><br><code>BulletedList.RenderBulletText(ListItem,Int32,HtmlTextWriter) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackCol…</code> |
| <code>Api.WebControls.BulletedList.RenderContents</code> | <code>BulletedList.RenderContents(HtmlTextWriter) = &quot;ok&quot;</code><br><code>BulletedList.RenderContents(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;</code><br><code>BulletedList.RenderContents(HtmlTextWriter) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #0000000…</code> |
| <code>Api.WebControls.BulletedList.RenderWhenDataEmpty</code> | <code>BulletedList.RenderWhenDataEmpty = &quot;false&quot;</code><br><code>BulletedList.RenderWhenDataEmpty set = &quot;true&quot;</code> |
| <code>Api.WebControls.BulletedList.SelectedIndex</code> | <code>BulletedList.SelectedIndex = &quot;Int32 1&quot;</code><br><code>BulletedList.SelectedIndex set = &quot;-&gt; System.NotSupportedException: Setting the SelectedIndex or SelectedValue properties of BulletedList is not supported.&quot;</code> |
| <code>Api.WebControls.BulletedList.SelectedItem</code> | <code>BulletedList.SelectedItem = ListItem {Enabled=true; Selected=true; Text=&quot;Medium&quot;; Value=&quot;M&quot;}</code> |
| <code>Api.WebControls.BulletedList.SelectedValue</code> | <code>BulletedList.SelectedValue = &quot;M&quot;</code><br><code>BulletedList.SelectedValue set = &quot;-&gt; System.NotSupportedException: Setting the SelectedIndex or SelectedValue properties of BulletedList is not supported.&quot;</code> |
| <code>Api.WebControls.BulletedList.TagKey</code> | <code>BulletedList.TagKey = &quot;HtmlTextWriterTag.Ul&quot;</code> |
| <code>Api.WebControls.BulletedList.Target</code> | <code>BulletedList.Target = &quot;&quot;</code><br><code>BulletedList.Target set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.BulletedList.Text</code> | <code>BulletedList.Text = &quot;M&quot;</code><br><code>BulletedList.Text set = &quot;-&gt; System.NotSupportedException: Setting the Text property of BulletedList is not supported.&quot;</code> |
| <code>Api.WebControls.DataBoundControl.#ctor</code> | <code>DataBoundControl.#ctor = &quot;-&gt; System.MemberAccessException: Cannot create an instance of System.Web.UI.WebControls.DataBoundControl because it is an abstract cla…</code> |
| <code>Api.WebControls.DataBoundControl.CallingDataMethods</code> | <code>DataBoundControl.CallingDataMethods = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.DataBoundControl.CreateDataSourceSelectArguments</code> | <code>DataBoundControl.CreateDataSourceSelectArguments = DataSourceSelectArguments {MaximumRows=Int32 0; RetrieveTotalRowCount=false; SortExpression=&quot;&quot;; StartRowIndex…</code><br><code>DataBoundControl.CreateDataSourceSelectArguments after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00…</code> |
| <code>Api.WebControls.DataBoundControl.CreatingModelDataSource</code> | <code>DataBoundControl.CreatingModelDataSource = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.DataBoundControl.DataMember</code> | <code>DataBoundControl.DataMember = &quot;&quot;</code><br><code>DataBoundControl.DataMember set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.DataBoundControl.DataSourceID</code> | <code>DataBoundControl.DataSourceID = &quot;&quot;</code><br><code>DataBoundControl.DataSourceID set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.DataBoundControl.DataSourceObject</code> | <code>DataBoundControl.DataSourceObject = null</code> |
| <code>Api.WebControls.DataBoundControl.GetData</code> | <code>DataBoundControl.GetData = ReadOnlyDataSourceView {CanDelete=false; CanInsert=false; CanPage=false; CanRetrieveTotalRowCount=false; CanSort=false; CanUpdate=fal…</code><br><code>DataBoundControl.GetData after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; Bo…</code> |
| <code>Api.WebControls.DataBoundControl.GetDataSource</code> | <code>DataBoundControl.GetDataSource = null</code><br><code>DataBoundControl.GetDataSource after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empt…</code> |
| <code>Api.WebControls.DataBoundControl.IsUsingModelBinders</code> | <code>DataBoundControl.IsUsingModelBinders = &quot;false&quot;</code> |
| <code>Api.WebControls.DataBoundControl.ItemType</code> | <code>DataBoundControl.ItemType = &quot;&quot;</code><br><code>DataBoundControl.ItemType set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.DataBoundControl.LoadViewState</code> | <code>DataBoundControl.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Web.UI.Triplet&#39;.&quot;</code> |
| <code>Api.WebControls.DataBoundControl.MarkAsDataBound</code> | <code>DataBoundControl.MarkAsDataBound = &quot;ok&quot;</code><br><code>DataBoundControl.MarkAsDataBound after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Em…</code> |
| <code>Api.WebControls.DataBoundControl.OnCreatingModelDataSource</code> | <code>DataBoundControl.OnCreatingModelDataSource(CreatingModelDataSourceEventArgs) = &quot;ok&quot;</code><br><code>DataBoundControl.OnCreatingModelDataSource(CreatingModelDataSourceEventArgs) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBac…</code> |
| <code>Api.WebControls.DataBoundControl.OnDataPropertyChanged</code> | <code>DataBoundControl.OnDataPropertyChanged = &quot;ok&quot;</code><br><code>DataBoundControl.OnDataPropertyChanged after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Col…</code> |
| <code>Api.WebControls.DataBoundControl.OnDataSourceViewChanged</code> | <code>DataBoundControl.OnDataSourceViewChanged(Object,EventArgs) = &quot;ok&quot;</code><br><code>DataBoundControl.OnDataSourceViewChanged(Object,EventArgs) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor…</code> |
| <code>Api.WebControls.DataBoundControl.OnLoad</code> | <code>DataBoundControl.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>DataBoundControl.OnLoad(EventArgs) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color […</code> |
| <code>Api.WebControls.DataBoundControl.OnPagePreLoad</code> | <code>DataBoundControl.OnPagePreLoad(Object,EventArgs) = &quot;ok&quot;</code><br><code>DataBoundControl.OnPagePreLoad(Object,EventArgs) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00…</code> |
| <code>Api.WebControls.DataBoundControl.PerformDataBinding</code> | <code>DataBoundControl.PerformDataBinding(IEnumerable) = &quot;ok&quot;</code><br><code>DataBoundControl.PerformDataBinding(IEnumerable) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00…</code> |
| <code>Api.WebControls.DataBoundControl.PerformSelect</code> | <code>DataBoundControl.PerformSelect = &quot;ok&quot;</code><br><code>DataBoundControl.PerformSelect after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empt…</code> |
| <code>Api.WebControls.DataBoundControl.SaveViewState</code> | <code>DataBoundControl.SaveViewState = Triplet {}</code><br><code>DataBoundControl.SaveViewState after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empt…</code> |
| <code>Api.WebControls.DataBoundControl.SelectArguments</code> | <code>DataBoundControl.SelectArguments = DataSourceSelectArguments {MaximumRows=Int32 0; RetrieveTotalRowCount=false; SortExpression=&quot;&quot;; StartRowIndex=Int32 0; TotalR…</code> |
| <code>Api.WebControls.DataBoundControl.SelectMethod</code> | <code>DataBoundControl.SelectMethod = &quot;&quot;</code><br><code>DataBoundControl.SelectMethod set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.DataBoundControl.TrackViewState</code> | <code>DataBoundControl.TrackViewState = &quot;ok&quot;</code><br><code>DataBoundControl.TrackViewState after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Emp…</code> |
| <code>Api.WebControls.DataBoundControl.ValidateDataSource</code> | <code>DataBoundControl.ValidateDataSource(Object) = &quot;ok&quot;</code><br><code>DataBoundControl.ValidateDataSource(Object) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #0000000…</code> |
| <code>Api.WebControls.ListControl.#ctor</code> | <code>ListControl.#ctor = &quot;-&gt; System.MemberAccessException: Cannot create an instance of System.Web.UI.WebControls.ListControl because it is an abstract class.&quot;</code> |
| <code>Api.WebControls.ListControl.AddAttributesToRender</code> | <code>ListControl.AddAttributesToRender(HtmlTextWriter) = &quot;ok&quot;</code><br><code>ListControl.AddAttributesToRender(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>ListControl.AddAttributesToRender(HtmlTextWriter) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #0…</code> |
| <code>Api.WebControls.ListControl.AppendDataBoundItems</code> | <code>ListControl.AppendDataBoundItems = &quot;false&quot;</code><br><code>ListControl.AppendDataBoundItems set = &quot;true&quot;</code> |
| <code>Api.WebControls.ListControl.AutoPostBack</code> | <code>ListControl.AutoPostBack = &quot;false&quot;</code><br><code>ListControl.AutoPostBack set = &quot;-&gt; System.NotSupportedException: Setting the AutoPostBack property of System.Web.UI.WebControls.BulletedList is not supported.&quot;</code> |
| <code>Api.WebControls.ListControl.CausesValidation</code> | <code>ListControl.CausesValidation = &quot;false&quot;</code><br><code>ListControl.CausesValidation set = &quot;true&quot;</code> |
| <code>Api.WebControls.ListControl.ClearSelection</code> | <code>ListControl.ClearSelection = &quot;ok&quot;</code><br><code>ListControl.ClearSelection after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; …</code> |
| <code>Api.WebControls.ListControl.DataTextField</code> | <code>ListControl.DataTextField = &quot;&quot;</code><br><code>ListControl.DataTextField set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ListControl.DataTextFormatString</code> | <code>ListControl.DataTextFormatString = &quot;&quot;</code><br><code>ListControl.DataTextFormatString set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ListControl.DataValueField</code> | <code>ListControl.DataValueField = &quot;&quot;</code><br><code>ListControl.DataValueField set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ListControl.Items</code> | <code>ListControl.Items = &quot;[System.Web.UI.WebControls.ListItem Small, System.Web.UI.WebControls.ListItem Medium, System.Web.UI.WebControls.ListItem Large]&quot;</code> |
| <code>Api.WebControls.ListControl.LoadViewState</code> | <code>ListControl.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Web.UI.Triplet&#39;.&quot;</code> |
| <code>Api.WebControls.ListControl.OnDataBinding</code> | <code>ListControl.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>ListControl.OnDataBinding(EventArgs) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color…</code> |
| <code>Api.WebControls.ListControl.OnPreRender</code> | <code>ListControl.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>ListControl.OnPreRender(EventArgs) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color […</code> |
| <code>Api.WebControls.ListControl.OnSelectedIndexChanged</code> | <code>ListControl.OnSelectedIndexChanged(EventArgs) = &quot;ok&quot;</code><br><code>ListControl.OnSelectedIndexChanged(EventArgs) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000…</code> |
| <code>Api.WebControls.ListControl.OnTextChanged</code> | <code>ListControl.OnTextChanged(EventArgs) = &quot;ok&quot;</code><br><code>ListControl.OnTextChanged(EventArgs) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color…</code> |
| <code>Api.WebControls.ListControl.PerformDataBinding</code> | <code>ListControl.PerformDataBinding(IEnumerable) = &quot;ok&quot;</code><br><code>ListControl.PerformDataBinding(IEnumerable) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #0000000…</code> |
| <code>Api.WebControls.ListControl.PerformSelect</code> | <code>ListControl.PerformSelect = &quot;ok&quot;</code><br><code>ListControl.PerformSelect after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; B…</code> |
| <code>Api.WebControls.ListControl.RenderContents</code> | <code>ListControl.RenderContents(HtmlTextWriter) = &quot;ok&quot;</code><br><code>ListControl.RenderContents(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;</code><br><code>ListControl.RenderContents(HtmlTextWriter) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000…</code> |
| <code>Api.WebControls.ListControl.SaveViewState</code> | <code>ListControl.SaveViewState = Triplet {}</code><br><code>ListControl.SaveViewState after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; B…</code> |
| <code>Api.WebControls.ListControl.SelectedIndex</code> | <code>ListControl.SelectedIndex = &quot;Int32 1&quot;</code><br><code>ListControl.SelectedIndex set = &quot;-&gt; System.NotSupportedException: Setting the SelectedIndex or SelectedValue properties of BulletedList is not supported.&quot;</code> |
| <code>Api.WebControls.ListControl.SelectedIndexChanged</code> | <code>ListControl.SelectedIndexChanged = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ListControl.SelectedItem</code> | <code>ListControl.SelectedItem = ListItem {Enabled=true; Selected=true; Text=&quot;Medium&quot;; Value=&quot;M&quot;}</code> |
| <code>Api.WebControls.ListControl.SelectedValue</code> | <code>ListControl.SelectedValue = &quot;M&quot;</code><br><code>ListControl.SelectedValue set = &quot;-&gt; System.NotSupportedException: Setting the SelectedIndex or SelectedValue properties of BulletedList is not supported.&quot;</code> |
| <code>Api.WebControls.ListControl.SetPostDataSelection</code> | <code>ListControl.SetPostDataSelection(Int32) = &quot;ok&quot;</code><br><code>ListControl.SetPostDataSelection(Int32) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Co…</code> |
| <code>Api.WebControls.ListControl.TagKey</code> | <code>ListControl.TagKey = &quot;HtmlTextWriterTag.Ul&quot;</code> |
| <code>Api.WebControls.ListControl.Text</code> | <code>ListControl.Text = &quot;M&quot;</code><br><code>ListControl.Text set = &quot;-&gt; System.NotSupportedException: Setting the Text property of BulletedList is not supported.&quot;</code> |
| <code>Api.WebControls.ListControl.TextChanged</code> | <code>ListControl.TextChanged = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ListControl.TrackViewState</code> | <code>ListControl.TrackViewState = &quot;ok&quot;</code><br><code>ListControl.TrackViewState after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; …</code> |
| <code>Api.WebControls.ListControl.ValidationGroup</code> | <code>ListControl.ValidationGroup = &quot;&quot;</code><br><code>ListControl.ValidationGroup set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ListControl.VerifyMultiSelect</code> | <code>ListControl.VerifyMultiSelect = &quot;-&gt; System.Web.HttpException: Cannot have multiple items selected when the SelectionMode is Single.&quot;</code> |
| <code>Api.WebControls.WebControl.#ctor</code> | <code>WebControl.#ctor = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidt…</code><br><code>WebControl.#ctor(String) = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; Bo…</code><br><code>WebControl.#ctor(HtmlTextWriterTag) = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle…</code> |
| <code>Api.WebControls.WebControl.AccessKey</code> | <code>WebControl.AccessKey = &quot;&quot;</code><br><code>WebControl.AccessKey set = &quot;-&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]&quot;</code> |
| <code>Api.WebControls.WebControl.AddAttributesToRender</code> | <code>WebControl.AddAttributesToRender(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.AddAttributesToRender(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>WebControl.AddAttributesToRender(HtmlTextWriter) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00…</code> |
| <code>Api.WebControls.WebControl.ApplyStyle</code> | <code>WebControl.ApplyStyle(Style) = &quot;ok&quot;</code><br><code>WebControl.ApplyStyle(Style) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]…</code> |
| <code>Api.WebControls.WebControl.Attributes</code> | <code>WebControl.Attributes = AttributeCollection []</code> |
| <code>Api.WebControls.WebControl.BackColor</code> | <code>WebControl.BackColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>WebControl.BackColor set = &quot;Color #FF3366CC Color [A=255, R=51, G=102, B=204]&quot;</code> |
| <code>Api.WebControls.WebControl.BorderColor</code> | <code>WebControl.BorderColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>WebControl.BorderColor set = &quot;Color #FF3366CC Color [A=255, R=51, G=102, B=204]&quot;</code> |
| <code>Api.WebControls.WebControl.BorderStyle</code> | <code>WebControl.BorderStyle = &quot;BorderStyle.NotSet&quot;</code><br><code>WebControl.BorderStyle set = &quot;BorderStyle.None&quot;</code> |
| <code>Api.WebControls.WebControl.BorderWidth</code> | <code>WebControl.BorderWidth = Unit </code><br><code>WebControl.BorderWidth set = Unit 120px</code> |
| <code>Api.WebControls.WebControl.ControlStyle</code> | <code>WebControl.ControlStyle = Style {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; CssClass=…</code> |
| <code>Api.WebControls.WebControl.ControlStyleCreated</code> | <code>WebControl.ControlStyleCreated = &quot;false&quot;</code> |
| <code>Api.WebControls.WebControl.CopyBaseAttributes</code> | <code>WebControl.CopyBaseAttributes(WebControl) = &quot;ok&quot;</code><br><code>WebControl.CopyBaseAttributes(WebControl) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 …</code> |
| <code>Api.WebControls.WebControl.CreateControlStyle</code> | <code>WebControl.CreateControlStyle = Style {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; Css…</code><br><code>WebControl.CreateControlStyle after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty…</code> |
| <code>Api.WebControls.WebControl.CssClass</code> | <code>WebControl.CssClass = &quot;&quot;</code><br><code>WebControl.CssClass set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.DisabledCssClass</code> | <code>WebControl.DisabledCssClass = &quot;aspNetDisabled&quot;</code><br><code>WebControl.DisabledCssClass set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.EnableTheming</code> | <code>WebControl.EnableTheming = &quot;true&quot;</code><br><code>WebControl.EnableTheming set = &quot;true&quot;</code> |
| <code>Api.WebControls.WebControl.Enabled</code> | <code>WebControl.Enabled = &quot;true&quot;</code><br><code>WebControl.Enabled set = &quot;true&quot;</code> |
| <code>Api.WebControls.WebControl.Font</code> | <code>WebControl.Font = FontInfo {Bold=false; Italic=false; Name=&quot;&quot;; Overline=false; Size=; Strikeout=false; Underline=false}</code> |
| <code>Api.WebControls.WebControl.ForeColor</code> | <code>WebControl.ForeColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>WebControl.ForeColor set = &quot;Color #FF3366CC Color [A=255, R=51, G=102, B=204]&quot;</code> |
| <code>Api.WebControls.WebControl.HasAttributes</code> | <code>WebControl.HasAttributes = &quot;false&quot;</code> |
| <code>Api.WebControls.WebControl.Height</code> | <code>WebControl.Height = Unit </code><br><code>WebControl.Height set = Unit 120px</code> |
| <code>Api.WebControls.WebControl.IsEnabled</code> | <code>WebControl.IsEnabled = &quot;true&quot;</code> |
| <code>Api.WebControls.WebControl.LoadViewState</code> | <code>WebControl.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Web.UI.Triplet&#39;.&quot;</code> |
| <code>Api.WebControls.WebControl.MergeStyle</code> | <code>WebControl.MergeStyle(Style) = &quot;ok&quot;</code><br><code>WebControl.MergeStyle(Style) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]…</code> |
| <code>Api.WebControls.WebControl.Render</code> | <code>WebControl.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.Render(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;ul id=&quot;bltSize&quot;&gt;</code><br><code>  &#124; 	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;</code><br><code>  &#124; &lt;/ul&gt;</code><br><code>WebControl.Render(HtmlTextWriter) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [E…</code> |
| <code>Api.WebControls.WebControl.RenderBeginTag</code> | <code>WebControl.RenderBeginTag(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.RenderBeginTag(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;ul id=&quot;bltSize&quot;&gt;</code><br><code>  &#124; </code><br><code>WebControl.RenderBeginTag(HtmlTextWriter) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 …</code> |
| <code>Api.WebControls.WebControl.RenderContents</code> | <code>WebControl.RenderContents(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.RenderContents(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;</code><br><code>WebControl.RenderContents(HtmlTextWriter) after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 …</code> |
| <code>Api.WebControls.WebControl.RenderEndTag</code> | <code>WebControl.RenderEndTag(HtmlTextWriter) = &quot;-&gt; System.InvalidOperationException: A PopEndTag was called without a corresponding PushEndTag.&quot;</code> |
| <code>Api.WebControls.WebControl.SaveViewState</code> | <code>WebControl.SaveViewState = Triplet {}</code><br><code>WebControl.SaveViewState after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; Bo…</code> |
| <code>Api.WebControls.WebControl.SkinID</code> | <code>WebControl.SkinID = &quot;&quot;</code><br><code>WebControl.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.Style</code> | <code>WebControl.Style = CssStyleCollection {Count=Int32 0; Value=null}</code> |
| <code>Api.WebControls.WebControl.SupportsDisabledAttribute</code> | <code>WebControl.SupportsDisabledAttribute = &quot;false&quot;</code> |
| <code>Api.WebControls.WebControl.TabIndex</code> | <code>WebControl.TabIndex = &quot;Int16 0&quot;</code><br><code>WebControl.TabIndex set = &quot;Int16 1&quot;</code> |
| <code>Api.WebControls.WebControl.TagKey</code> | <code>WebControl.TagKey = &quot;HtmlTextWriterTag.Ul&quot;</code> |
| <code>Api.WebControls.WebControl.TagName</code> | <code>WebControl.TagName = &quot;ul&quot;</code> |
| <code>Api.WebControls.WebControl.ToolTip</code> | <code>WebControl.ToolTip = &quot;&quot;</code><br><code>WebControl.ToolTip set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.TrackViewState</code> | <code>WebControl.TrackViewState = &quot;ok&quot;</code><br><code>WebControl.TrackViewState after = BulletedList#bltSize {AccessKey=&quot;&quot;; AppendDataBoundItems=false; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; B…</code> |
| <code>Api.WebControls.WebControl.Width</code> | <code>WebControl.Width = Unit </code><br><code>WebControl.Width set = Unit 120px</code> |

## ② HTML 層

プローブサイトの RenderProbe.aspx で、1 ケース 1 リクエスト（ページライフサイクル全体）で描画したページ全体を比較します。下表はコンポーネント部分（div#probe）、設定時の例外、保存された ViewState の要約です。

| ケース | 設定 | 期待される描画 | 設定時の例外 |
|---|---|---|---|
| <code>.(default)</code> | <code>(nothing set)</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.AccessKey=text</code> | <code>c.AccessKey = &quot;probe&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.AccessKey = &quot;probe&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=markup</code> | <code>c.AccessKey = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.AccessKey = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=japanese</code> | <code>c.AccessKey = &quot;日本語テキスト&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.AccessKey = &quot;日本語テキスト&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=empty</code> | <code>c.AccessKey = &quot;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.AccessKey=k</code> | <code>c.AccessKey = &quot;k&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot; accesskey=&quot;k&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.AppRelativeTemplateSourceDirectory=text</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;probe&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=markup</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=japanese</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;日本語テキスト&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=empty</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.AppendDataBoundItems=true</code> | <code>c.AppendDataBoundItems = true;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.AppendDataBoundItems=false</code> | <code>c.AppendDataBoundItems = false;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.AutoPostBack=true</code> | <code>c.AutoPostBack = true;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.AutoPostBack = true; -&gt; System.NotSupportedException: Setting the AutoPostBack property of System.Web.UI.WebControls.BulletedList is not supported.</code> |
| <code>.AutoPostBack=false</code> | <code>c.AutoPostBack = false;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.AutoPostBack = false; -&gt; System.NotSupportedException: Setting the AutoPostBack property of System.Web.UI.WebControls.BulletedList is not supported.</code> |
| <code>.BackColor=Red</code> | <code>c.BackColor = Color.Red;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;background-color:Red;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.BackColor=#3366CC</code> | <code>c.BackColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;background-color:#3366CC;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.BackColor=Transparent</code> | <code>c.BackColor = Color.Transparent;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;background-color:Transparent;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.BorderColor=Red</code> | <code>c.BorderColor = Color.Red;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;border-color:Red;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.BorderColor=#3366CC</code> | <code>c.BorderColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;border-color:#3366CC;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.BorderColor=Transparent</code> | <code>c.BorderColor = Color.Transparent;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;border-color:Transparent;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.BorderStyle=NotSet</code> | <code>c.BorderStyle = BorderStyle.NotSet;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.BorderStyle=None</code> | <code>c.BorderStyle = BorderStyle.None;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;border-style:None;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.BorderStyle=Dotted</code> | <code>c.BorderStyle = BorderStyle.Dotted;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;border-style:Dotted;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.BorderStyle=Dashed</code> | <code>c.BorderStyle = BorderStyle.Dashed;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;border-style:Dashed;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.BorderStyle=Solid</code> | <code>c.BorderStyle = BorderStyle.Solid;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;border-style:Solid;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.BorderStyle=Double</code> | <code>c.BorderStyle = BorderStyle.Double;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;border-style:Double;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.BorderStyle=Groove</code> | <code>c.BorderStyle = BorderStyle.Groove;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;border-style:Groove;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.BorderStyle=Ridge</code> | <code>c.BorderStyle = BorderStyle.Ridge;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;border-style:Ridge;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.BorderStyle=Inset</code> | <code>c.BorderStyle = BorderStyle.Inset;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;border-style:Inset;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.BorderStyle=Outset</code> | <code>c.BorderStyle = BorderStyle.Outset;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;border-style:Outset;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.BorderWidth=120px</code> | <code>c.BorderWidth = Unit.Pixel(120);</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;border-width:120px;border-style:solid;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.BorderWidth=50%</code> | <code>c.BorderWidth = Unit.Percentage(50);</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.BorderWidth = Unit.Percentage(50); -&gt; System.ArgumentOutOfRangeException: BorderWidth must be a non-negative number and cannot be a percentage. [param value]</code> |
| <code>.BorderWidth=2em</code> | <code>c.BorderWidth = new Unit(2, UnitType.Em);</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;border-width:2em;border-style:solid;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.BulletImageUrl=text</code> | <code>c.BulletImageUrl = &quot;probe&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.BulletImageUrl=markup</code> | <code>c.BulletImageUrl = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.BulletImageUrl=japanese</code> | <code>c.BulletImageUrl = &quot;日本語テキスト&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.BulletImageUrl=empty</code> | <code>c.BulletImageUrl = &quot;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.BulletStyle=NotSet</code> | <code>c.BulletStyle = BulletStyle.NotSet;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.BulletStyle=Numbered</code> | <code>c.BulletStyle = BulletStyle.Numbered;</code> | <code>&lt;ol id=&quot;bltSize&quot; style=&quot;list-style-type:decimal;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ol&gt;</code> |  |
| <code>.BulletStyle=LowerAlpha</code> | <code>c.BulletStyle = BulletStyle.LowerAlpha;</code> | <code>&lt;ol id=&quot;bltSize&quot; style=&quot;list-style-type:lower-alpha;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ol&gt;</code> |  |
| <code>.BulletStyle=UpperAlpha</code> | <code>c.BulletStyle = BulletStyle.UpperAlpha;</code> | <code>&lt;ol id=&quot;bltSize&quot; style=&quot;list-style-type:upper-alpha;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ol&gt;</code> |  |
| <code>.BulletStyle=LowerRoman</code> | <code>c.BulletStyle = BulletStyle.LowerRoman;</code> | <code>&lt;ol id=&quot;bltSize&quot; style=&quot;list-style-type:lower-roman;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ol&gt;</code> |  |
| <code>.BulletStyle=UpperRoman</code> | <code>c.BulletStyle = BulletStyle.UpperRoman;</code> | <code>&lt;ol id=&quot;bltSize&quot; style=&quot;list-style-type:upper-roman;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ol&gt;</code> |  |
| <code>.BulletStyle=Disc</code> | <code>c.BulletStyle = BulletStyle.Disc;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;list-style-type:disc;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.BulletStyle=Circle</code> | <code>c.BulletStyle = BulletStyle.Circle;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;list-style-type:circle;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.BulletStyle=Square</code> | <code>c.BulletStyle = BulletStyle.Square;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;list-style-type:square;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.BulletStyle=CustomImage</code> | <code>c.BulletStyle = BulletStyle.CustomImage;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;list-style-image:url();&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.CausesValidation=true</code> | <code>c.CausesValidation = true;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.CausesValidation=false</code> | <code>c.CausesValidation = false;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ClientIDMode=Inherit</code> | <code>c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ClientIDMode=AutoID</code> | <code>c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ClientIDMode=Predictable</code> | <code>c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ClientIDMode=Static</code> | <code>c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.CssClass=text</code> | <code>c.CssClass = &quot;probe&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot; class=&quot;probe&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.CssClass=markup</code> | <code>c.CssClass = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot; class=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.CssClass=japanese</code> | <code>c.CssClass = &quot;日本語テキスト&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot; class=&quot;日本語テキスト&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.CssClass=empty</code> | <code>c.CssClass = &quot;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DataMember=text</code> | <code>c.DataMember = &quot;probe&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DataMember=markup</code> | <code>c.DataMember = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DataMember=japanese</code> | <code>c.DataMember = &quot;日本語テキスト&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DataMember=empty</code> | <code>c.DataMember = &quot;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DataSourceID=text</code> | <code>c.DataSourceID = &quot;probe&quot;;</code> | <code>status = 500</code> |  |
| <code>.DataSourceID=markup</code> | <code>c.DataSourceID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>status = 500</code> |  |
| <code>.DataSourceID=japanese</code> | <code>c.DataSourceID = &quot;日本語テキスト&quot;;</code> | <code>status = 500</code> |  |
| <code>.DataSourceID=empty</code> | <code>c.DataSourceID = &quot;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DataTextField=text</code> | <code>c.DataTextField = &quot;probe&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DataTextField=markup</code> | <code>c.DataTextField = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DataTextField=japanese</code> | <code>c.DataTextField = &quot;日本語テキスト&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DataTextField=empty</code> | <code>c.DataTextField = &quot;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DataTextField=Name</code> | <code>c.DataTextField = &quot;Name&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DataTextField=Price</code> | <code>c.DataTextField = &quot;Price&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DataTextFormatString=text</code> | <code>c.DataTextFormatString = &quot;probe&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DataTextFormatString=markup</code> | <code>c.DataTextFormatString = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DataTextFormatString=japanese</code> | <code>c.DataTextFormatString = &quot;日本語テキスト&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DataTextFormatString=empty</code> | <code>c.DataTextFormatString = &quot;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DataTextFormatString=N2</code> | <code>c.DataTextFormatString = &quot;{0:N2}&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DataValueField=text</code> | <code>c.DataValueField = &quot;probe&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DataValueField=markup</code> | <code>c.DataValueField = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DataValueField=japanese</code> | <code>c.DataValueField = &quot;日本語テキスト&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DataValueField=empty</code> | <code>c.DataValueField = &quot;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DataValueField=Code</code> | <code>c.DataValueField = &quot;Code&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DisplayMode=Text</code> | <code>c.DisplayMode = BulletedListDisplayMode.Text;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DisplayMode=HyperLink</code> | <code>c.DisplayMode = BulletedListDisplayMode.HyperLink;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;&lt;a href=&quot;S&quot;&gt;Small&lt;/a&gt;&lt;/li&gt;&lt;li&gt;&lt;a href=&quot;M&quot;&gt;Medium&lt;/a&gt;&lt;/li&gt;&lt;li&gt;&lt;a href=&quot;L&quot;&gt;Large&lt;/a&gt;&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DisplayMode=LinkButton</code> | <code>c.DisplayMode = BulletedListDisplayMode.LinkButton;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;bltSize&#39;,&#39;0&#39;)&quot;&gt;Small&lt;/a&gt;&lt;/li&gt;&lt;li&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;bltSize&#39;,&#39;1&#39;)&quot;&gt;Medium&lt;/a&gt;&lt;/li&gt;&lt;li&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;bltSize&#39;,&#39;2&#39;)&quot;&gt;Large&lt;/a&gt;&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.EnableTheming=true</code> | <code>c.EnableTheming = true;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.EnableTheming=false</code> | <code>c.EnableTheming = false;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.EnableViewState=true</code> | <code>c.EnableViewState = true;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.EnableViewState=false</code> | <code>c.EnableViewState = false;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Enabled=true</code> | <code>c.Enabled = true;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Enabled=false</code> | <code>c.Enabled = false;</code> | <code>&lt;ul id=&quot;bltSize&quot; class=&quot;aspNetDisabled&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.FirstBulletNumber=0</code> | <code>c.FirstBulletNumber = 0;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.FirstBulletNumber=3</code> | <code>c.FirstBulletNumber = 3;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.FirstBulletNumber=-1</code> | <code>c.FirstBulletNumber = -1;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Font.Bold=true</code> | <code>c.Font.Bold = true;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;font-weight:bold;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Font.Bold=false</code> | <code>c.Font.Bold = false;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;font-weight:normal;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Font.Italic=true</code> | <code>c.Font.Italic = true;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;font-style:italic;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Font.Italic=false</code> | <code>c.Font.Italic = false;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;font-style:normal;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Font.Name=text</code> | <code>c.Font.Name = &quot;probe&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;font-family:probe;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Font.Name=markup</code> | <code>c.Font.Name = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;font-family:&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Font.Name=japanese</code> | <code>c.Font.Name = &quot;日本語テキスト&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;font-family:日本語テキスト;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Font.Name=empty</code> | <code>c.Font.Name = &quot;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Font.Names=Arial,MS-Gothic</code> | <code>c.Font.Names = new[] { &quot;Arial&quot;, &quot;MS Gothic&quot; };</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;font-family:Arial,MS Gothic;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Font.Overline=true</code> | <code>c.Font.Overline = true;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;text-decoration: overline;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Font.Overline=false</code> | <code>c.Font.Overline = false;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;text-decoration:none;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Font.Size=12pt</code> | <code>c.Font.Size = FontUnit.Point(12);</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;font-size:12pt;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Font.Size=Large</code> | <code>c.Font.Size = FontUnit.Large;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;font-size:Large;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Font.Size=150%</code> | <code>c.Font.Size = new FontUnit(Unit.Percentage(150));</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;font-size:150%;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Font.Strikeout=true</code> | <code>c.Font.Strikeout = true;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;text-decoration: line-through;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Font.Strikeout=false</code> | <code>c.Font.Strikeout = false;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;text-decoration:none;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Font.Underline=true</code> | <code>c.Font.Underline = true;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;text-decoration:underline;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Font.Underline=false</code> | <code>c.Font.Underline = false;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;text-decoration:none;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ForeColor=Red</code> | <code>c.ForeColor = Color.Red;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;color:Red;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ForeColor=#3366CC</code> | <code>c.ForeColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;color:#3366CC;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ForeColor=Transparent</code> | <code>c.ForeColor = Color.Transparent;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;color:Transparent;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Height=120px</code> | <code>c.Height = Unit.Pixel(120);</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;height:120px;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Height=50%</code> | <code>c.Height = Unit.Percentage(50);</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;height:50%;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Height=2em</code> | <code>c.Height = new Unit(2, UnitType.Em);</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;height:2em;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ID=text</code> | <code>c.ID = &quot;probe&quot;;</code> | <code>&lt;ul id=&quot;probe&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ID=markup</code> | <code>c.ID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;ul id=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ID=japanese</code> | <code>c.ID = &quot;日本語テキスト&quot;;</code> | <code>&lt;ul id=&quot;日本語テキスト&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ID=empty</code> | <code>c.ID = &quot;&quot;;</code> | <code>&lt;ul id=&quot;ctl02&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ItemType=text</code> | <code>c.ItemType = &quot;probe&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ItemType=markup</code> | <code>c.ItemType = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ItemType=japanese</code> | <code>c.ItemType = &quot;日本語テキスト&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ItemType=empty</code> | <code>c.ItemType = &quot;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.RenderWhenDataEmpty=true</code> | <code>c.RenderWhenDataEmpty = true;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.RenderWhenDataEmpty=false</code> | <code>c.RenderWhenDataEmpty = false;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.SelectMethod=text</code> | <code>c.SelectMethod = &quot;probe&quot;;</code> | <code>status = 500</code> |  |
| <code>.SelectMethod=markup</code> | <code>c.SelectMethod = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>status = 500</code> |  |
| <code>.SelectMethod=japanese</code> | <code>c.SelectMethod = &quot;日本語テキスト&quot;;</code> | <code>status = 500</code> |  |
| <code>.SelectMethod=empty</code> | <code>c.SelectMethod = &quot;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.SelectedIndex=0</code> | <code>c.SelectedIndex = 0;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.SelectedIndex = 0; -&gt; System.NotSupportedException: Setting the SelectedIndex or SelectedValue properties of BulletedList is not supported.</code> |
| <code>.SelectedIndex=3</code> | <code>c.SelectedIndex = 3;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.SelectedIndex = 3; -&gt; System.NotSupportedException: Setting the SelectedIndex or SelectedValue properties of BulletedList is not supported.</code> |
| <code>.SelectedIndex=-1</code> | <code>c.SelectedIndex = -1;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.SelectedIndex = -1; -&gt; System.NotSupportedException: Setting the SelectedIndex or SelectedValue properties of BulletedList is not supported.</code> |
| <code>.SelectedIndex=1</code> | <code>c.SelectedIndex = 1;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.SelectedIndex = 1; -&gt; System.NotSupportedException: Setting the SelectedIndex or SelectedValue properties of BulletedList is not supported.</code> |
| <code>.SelectedValue=text</code> | <code>c.SelectedValue = &quot;probe&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.SelectedValue = &quot;probe&quot;; -&gt; System.NotSupportedException: Setting the SelectedIndex or SelectedValue properties of BulletedList is not supported.</code> |
| <code>.SelectedValue=markup</code> | <code>c.SelectedValue = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.SelectedValue = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.NotSupportedException: Setting the SelectedIndex or SelectedValue properties of BulletedList is not supported.</code> |
| <code>.SelectedValue=japanese</code> | <code>c.SelectedValue = &quot;日本語テキスト&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.SelectedValue = &quot;日本語テキスト&quot;; -&gt; System.NotSupportedException: Setting the SelectedIndex or SelectedValue properties of BulletedList is not supported.</code> |
| <code>.SelectedValue=empty</code> | <code>c.SelectedValue = &quot;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.SelectedValue = &quot;&quot;; -&gt; System.NotSupportedException: Setting the SelectedIndex or SelectedValue properties of BulletedList is not supported.</code> |
| <code>.SelectedValue=M</code> | <code>c.SelectedValue = &quot;M&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.SelectedValue = &quot;M&quot;; -&gt; System.NotSupportedException: Setting the SelectedIndex or SelectedValue properties of BulletedList is not supported.</code> |
| <code>.SelectedValue=B2</code> | <code>c.SelectedValue = &quot;B2&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.SelectedValue = &quot;B2&quot;; -&gt; System.NotSupportedException: Setting the SelectedIndex or SelectedValue properties of BulletedList is not supported.</code> |
| <code>.SkinID=text</code> | <code>c.SkinID = &quot;probe&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.SkinID=markup</code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.SkinID=japanese</code> | <code>c.SkinID = &quot;日本語テキスト&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.SkinID=empty</code> | <code>c.SkinID = &quot;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.TabIndex=0</code> | <code>c.TabIndex = 0;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.TabIndex=3</code> | <code>c.TabIndex = 3;</code> | <code>&lt;ul id=&quot;bltSize&quot; tabindex=&quot;3&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.TabIndex=-1</code> | <code>c.TabIndex = -1;</code> | <code>&lt;ul id=&quot;bltSize&quot; tabindex=&quot;-1&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Target=text</code> | <code>c.Target = &quot;probe&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Target=markup</code> | <code>c.Target = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Target=japanese</code> | <code>c.Target = &quot;日本語テキスト&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Target=empty</code> | <code>c.Target = &quot;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Target=blank</code> | <code>c.Target = &quot;_blank&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Text=text</code> | <code>c.Text = &quot;probe&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.Text = &quot;probe&quot;; -&gt; System.NotSupportedException: Setting the Text property of BulletedList is not supported.</code> |
| <code>.Text=markup</code> | <code>c.Text = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.Text = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.NotSupportedException: Setting the Text property of BulletedList is not supported.</code> |
| <code>.Text=japanese</code> | <code>c.Text = &quot;日本語テキスト&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.Text = &quot;日本語テキスト&quot;; -&gt; System.NotSupportedException: Setting the Text property of BulletedList is not supported.</code> |
| <code>.Text=empty</code> | <code>c.Text = &quot;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.Text = &quot;&quot;; -&gt; System.NotSupportedException: Setting the Text property of BulletedList is not supported.</code> |
| <code>.ToolTip=text</code> | <code>c.ToolTip = &quot;probe&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot; title=&quot;probe&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ToolTip=markup</code> | <code>c.ToolTip = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot; title=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ToolTip=japanese</code> | <code>c.ToolTip = &quot;日本語テキスト&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot; title=&quot;日本語テキスト&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ToolTip=empty</code> | <code>c.ToolTip = &quot;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ValidateRequestMode=Inherit</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Inherit;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ValidateRequestMode=Disabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Disabled;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ValidateRequestMode=Enabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Enabled;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ValidationGroup=text</code> | <code>c.ValidationGroup = &quot;probe&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ValidationGroup=markup</code> | <code>c.ValidationGroup = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ValidationGroup=japanese</code> | <code>c.ValidationGroup = &quot;日本語テキスト&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ValidationGroup=empty</code> | <code>c.ValidationGroup = &quot;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ViewStateMode=Inherit</code> | <code>c.ViewStateMode = ViewStateMode.Inherit;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ViewStateMode=Enabled</code> | <code>c.ViewStateMode = ViewStateMode.Enabled;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ViewStateMode=Disabled</code> | <code>c.ViewStateMode = ViewStateMode.Disabled;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Visible=true</code> | <code>c.Visible = true;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Visible=false</code> | <code>c.Visible = false;</code> | <code></code> |  |
| <code>.Width=120px</code> | <code>c.Width = Unit.Pixel(120);</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;width:120px;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Width=50%</code> | <code>c.Width = Unit.Percentage(50);</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;width:50%;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Width=2em</code> | <code>c.Width = new Unit(2, UnitType.Em);</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;width:2em;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Items[1].Enabled=true</code> | <code>c.Items[1].Enabled = true;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Items[1].Enabled=false</code> | <code>c.Items[1].Enabled = false;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;&lt;span class=&quot;aspNetDisabled&quot;&gt;Medium&lt;/span&gt;&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Items[1].Selected=true</code> | <code>c.Items[1].Selected = true;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Items[1].Selected=false</code> | <code>c.Items[1].Selected = false;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Items[1].Text=text</code> | <code>c.Items[1].Text = &quot;probe&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;probe&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Items[1].Text=markup</code> | <code>c.Items[1].Text = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Items[1].Text=japanese</code> | <code>c.Items[1].Text = &quot;日本語テキスト&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;日本語テキスト&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Items[1].Text=empty</code> | <code>c.Items[1].Text = &quot;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Items[1].Text=spaces</code> | <code>c.Items[1].Text = &quot;  a  b  &quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;  a  b  &lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Items[1].Value=text</code> | <code>c.Items[1].Value = &quot;probe&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Items[1].Value=markup</code> | <code>c.Items[1].Value = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Items[1].Value=japanese</code> | <code>c.Items[1].Value = &quot;日本語テキスト&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Items[1].Value=empty</code> | <code>c.Items[1].Value = &quot;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Attributes[data-probe]=text</code> | <code>c.Attributes[data-probe] = &quot;probe&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot; data-probe=&quot;probe&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Attributes[onfocus]=text</code> | <code>c.Attributes[onfocus] = &quot;probe&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot; onfocus=&quot;probe&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Attributes[data-probe]=markup</code> | <code>c.Attributes[data-probe] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot; data-probe=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Attributes[onfocus]=markup</code> | <code>c.Attributes[onfocus] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot; onfocus=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Style[color]=red</code> | <code>c.Style[color] = &quot;red&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;color:red;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Style[margin-left]=red</code> | <code>c.Style[margin-left] = &quot;red&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;margin-left:red;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Style[color]=1em</code> | <code>c.Style[color] = &quot;1em&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;color:1em;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Style[margin-left]=1em</code> | <code>c.Style[margin-left] = &quot;1em&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;margin-left:1em;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.SelectedIndex=1+Items[1].Enabled=false</code> | <code>c.SelectedIndex = 1; c.Items[1].Enabled = false;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;&lt;span class=&quot;aspNetDisabled&quot;&gt;Medium&lt;/span&gt;&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.SelectedIndex = 1; -&gt; System.NotSupportedException: Setting the SelectedIndex or SelectedValue properties of BulletedList is not supported.</code> |
| <code>.Items[1].Selected=true+Enabled=false</code> | <code>c.Items[1].Selected = true; c.Enabled = false;</code> | <code>&lt;ul id=&quot;bltSize&quot; class=&quot;aspNetDisabled&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.SelectedValue=M+SelectedIndex=0</code> | <code>c.SelectedValue = &quot;M&quot;; c.SelectedIndex = 0;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.SelectedValue = &quot;M&quot;; -&gt; System.NotSupportedException: Setting the SelectedIndex or SelectedValue properties of BulletedList is not supported.↵c.SelectedIndex = 0; -&gt; System.NotSupportedException: Setting the SelectedIndex or SelectedValue properties of BulletedList is not supported.</code> |
| <code>.Items[1].Text=markup+Items[1].Value=markup</code> | <code>c.Items[1].Text = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; c.Items[1].Value = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Items[1].Text=empty+Items[1].Value=text</code> | <code>c.Items[1].Text = &quot;&quot;; c.Items[1].Value = &quot;probe&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Enabled=false+CssClass=text</code> | <code>c.Enabled = false; c.CssClass = &quot;probe&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot; class=&quot;aspNetDisabled probe&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | <code>c.Width = Unit.Pixel(120); c.Height = new Unit(2, UnitType.Em); c.BorderWidth = Unit.Pixel(120); c.BorderStyle = BorderStyle.Solid; c.BackColor = Color.FromArgb(0x33, 0x66, 0xCC); c.ForeColor = Color.Red; c.Font.Bold = true;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;color:Red;background-color:#3366CC;border-width:120px;border-style:Solid;font-weight:bold;height:2em;width:120px;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.ForeColor=#3366CC+Style[color]=red</code> | <code>c.ForeColor = Color.FromArgb(0x33, 0x66, 0xCC); c.Style[color] = &quot;red&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;color:#3366CC;color:red;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.SelectedIndex=1+EnableViewState=false</code> | <code>c.SelectedIndex = 1; c.EnableViewState = false;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.SelectedIndex = 1; -&gt; System.NotSupportedException: Setting the SelectedIndex or SelectedValue properties of BulletedList is not supported.</code> |
| <code>.SelectedIndex=1+Visible=false</code> | <code>c.SelectedIndex = 1; c.Visible = false;</code> | <code></code> | <code>c.SelectedIndex = 1; -&gt; System.NotSupportedException: Setting the SelectedIndex or SelectedValue properties of BulletedList is not supported.</code> |
| <code>.DisplayMode=LinkButton+Items[1].Enabled=false</code> | <code>c.DisplayMode = BulletedListDisplayMode.LinkButton; c.Items[1].Enabled = false;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;bltSize&#39;,&#39;0&#39;)&quot;&gt;Small&lt;/a&gt;&lt;/li&gt;&lt;li&gt;&lt;a class=&quot;aspNetDisabled&quot;&gt;Medium&lt;/a&gt;&lt;/li&gt;&lt;li&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;bltSize&#39;,&#39;2&#39;)&quot;&gt;Large&lt;/a&gt;&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DisplayMode=HyperLink+Items[1].Enabled=false</code> | <code>c.DisplayMode = BulletedListDisplayMode.HyperLink; c.Items[1].Enabled = false;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;&lt;a href=&quot;S&quot;&gt;Small&lt;/a&gt;&lt;/li&gt;&lt;li&gt;&lt;a class=&quot;aspNetDisabled&quot;&gt;Medium&lt;/a&gt;&lt;/li&gt;&lt;li&gt;&lt;a href=&quot;L&quot;&gt;Large&lt;/a&gt;&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DisplayMode=HyperLink+Target=blank</code> | <code>c.DisplayMode = BulletedListDisplayMode.HyperLink; c.Target = &quot;_blank&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;&lt;a href=&quot;S&quot; target=&quot;_blank&quot;&gt;Small&lt;/a&gt;&lt;/li&gt;&lt;li&gt;&lt;a href=&quot;M&quot; target=&quot;_blank&quot;&gt;Medium&lt;/a&gt;&lt;/li&gt;&lt;li&gt;&lt;a href=&quot;L&quot; target=&quot;_blank&quot;&gt;Large&lt;/a&gt;&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DisplayMode=LinkButton+CausesValidation=false</code> | <code>c.DisplayMode = BulletedListDisplayMode.LinkButton; c.CausesValidation = false;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;bltSize&#39;,&#39;0&#39;)&quot;&gt;Small&lt;/a&gt;&lt;/li&gt;&lt;li&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;bltSize&#39;,&#39;1&#39;)&quot;&gt;Medium&lt;/a&gt;&lt;/li&gt;&lt;li&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;bltSize&#39;,&#39;2&#39;)&quot;&gt;Large&lt;/a&gt;&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DisplayMode=LinkButton+ValidationGroup=text</code> | <code>c.DisplayMode = BulletedListDisplayMode.LinkButton; c.ValidationGroup = &quot;probe&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;bltSize&#39;,&#39;0&#39;)&quot;&gt;Small&lt;/a&gt;&lt;/li&gt;&lt;li&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;bltSize&#39;,&#39;1&#39;)&quot;&gt;Medium&lt;/a&gt;&lt;/li&gt;&lt;li&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;bltSize&#39;,&#39;2&#39;)&quot;&gt;Large&lt;/a&gt;&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DisplayMode=LinkButton+Enabled=false</code> | <code>c.DisplayMode = BulletedListDisplayMode.LinkButton; c.Enabled = false;</code> | <code>&lt;ul id=&quot;bltSize&quot; class=&quot;aspNetDisabled&quot;&gt;↵	&lt;li&gt;&lt;a&gt;Small&lt;/a&gt;&lt;/li&gt;&lt;li&gt;&lt;a&gt;Medium&lt;/a&gt;&lt;/li&gt;&lt;li&gt;&lt;a&gt;Large&lt;/a&gt;&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.BulletStyle=Numbered+FirstBulletNumber=3</code> | <code>c.BulletStyle = BulletStyle.Numbered; c.FirstBulletNumber = 3;</code> | <code>&lt;ol start=&quot;3&quot; id=&quot;bltSize&quot; style=&quot;list-style-type:decimal;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ol&gt;</code> |  |
| <code>.BulletStyle=CustomImage+BulletImageUrl=text</code> | <code>c.BulletStyle = BulletStyle.CustomImage; c.BulletImageUrl = &quot;probe&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot; style=&quot;list-style-image:url(probe);&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>.DisplayMode=LinkButton+Items[1].Text=markup</code> | <code>c.DisplayMode = BulletedListDisplayMode.LinkButton; c.Items[1].Text = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;bltSize&#39;,&#39;0&#39;)&quot;&gt;Small&lt;/a&gt;&lt;/li&gt;&lt;li&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;bltSize&#39;,&#39;1&#39;)&quot;&gt;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&lt;/a&gt;&lt;/li&gt;&lt;li&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;bltSize&#39;,&#39;2&#39;)&quot;&gt;Large&lt;/a&gt;&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>[naming container].(default)</code> | <code>[naming container] (nothing set)</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;ul id=&quot;box_bltSize&quot;&gt;↵		&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵	&lt;/ul&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=AutoID</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;ul id=&quot;box_bltSize&quot;&gt;↵		&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵	&lt;/ul&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Static</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;ul id=&quot;bltSize&quot;&gt;↵		&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵	&lt;/ul&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Predictable</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;ul id=&quot;box_bltSize&quot;&gt;↵		&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵	&lt;/ul&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Inherit</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;ul id=&quot;box_bltSize&quot;&gt;↵		&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵	&lt;/ul&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ID=text</code> | <code>[naming container] c.ID = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;ul id=&quot;box_probe&quot;&gt;↵		&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵	&lt;/ul&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].DisplayMode=LinkButton</code> | <code>[naming container] c.DisplayMode = BulletedListDisplayMode.LinkButton;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;ul id=&quot;box_bltSize&quot;&gt;↵		&lt;li&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;box$bltSize&#39;,&#39;0&#39;)&quot;&gt;Small&lt;/a&gt;&lt;/li&gt;&lt;li&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;box$bltSize&#39;,&#39;1&#39;)&quot;&gt;Medium&lt;/a&gt;&lt;/li&gt;&lt;li&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;box$bltSize&#39;,&#39;2&#39;)&quot;&gt;Large&lt;/a&gt;&lt;/li&gt;↵	&lt;/ul&gt;↵&lt;/div&gt;</code> |  |
| <code>[themed].(default)</code> | <code>[themed] (nothing set)</code> | <code>&lt;ul id=&quot;bltSize&quot; class=&quot;skinned&quot; style=&quot;list-style-type:square;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>[themed].SkinID=text</code> | <code>[themed] c.SkinID = &quot;probe&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot; class=&quot;skinned-probe&quot;&gt;↵	&lt;li&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;bltSize&#39;,&#39;0&#39;)&quot;&gt;Small&lt;/a&gt;&lt;/li&gt;&lt;li&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;bltSize&#39;,&#39;1&#39;)&quot;&gt;Medium&lt;/a&gt;&lt;/li&gt;&lt;li&gt;&lt;a href=&quot;javascript:__doPostBack(&#39;bltSize&#39;,&#39;2&#39;)&quot;&gt;Large&lt;/a&gt;&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>[themed].EnableTheming=false</code> | <code>[themed] c.EnableTheming = false;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>[themed].CssClass=text</code> | <code>[themed] c.CssClass = &quot;probe&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot; class=&quot;skinned&quot; style=&quot;list-style-type:square;&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>[data-bound].(default)</code> | <code>[data-bound] (nothing set)</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;WebControlsProbe.ProbeData+Product&lt;/li&gt;&lt;li&gt;WebControlsProbe.ProbeData+Product&lt;/li&gt;&lt;li&gt;WebControlsProbe.ProbeData+Product&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>[data-bound].DataTextField=Name</code> | <code>[data-bound] c.DataTextField = &quot;Name&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Pen&lt;/li&gt;&lt;li&gt;Bag&lt;/li&gt;&lt;li&gt;Cup &lt;&amp;&gt;&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>[data-bound].DataValueField=Code</code> | <code>[data-bound] c.DataValueField = &quot;Code&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;P1&lt;/li&gt;&lt;li&gt;B2&lt;/li&gt;&lt;li&gt;C3&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>[data-bound].DataTextField=Name+DataValueField=Code</code> | <code>[data-bound] c.DataTextField = &quot;Name&quot;; c.DataValueField = &quot;Code&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Pen&lt;/li&gt;&lt;li&gt;Bag&lt;/li&gt;&lt;li&gt;Cup &lt;&amp;&gt;&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>[data-bound].DataTextField=Price+DataTextFormatString=N2</code> | <code>[data-bound] c.DataTextField = &quot;Price&quot;; c.DataTextFormatString = &quot;{0:N2}&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;1.25&lt;/li&gt;&lt;li&gt;24.99&lt;/li&gt;&lt;li&gt;1,234.50&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>[data-bound].DataTextField=Name+AppendDataBoundItems=true</code> | <code>[data-bound] c.DataTextField = &quot;Name&quot;; c.AppendDataBoundItems = true;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Small&lt;/li&gt;&lt;li&gt;Medium&lt;/li&gt;&lt;li&gt;Large&lt;/li&gt;&lt;li&gt;Pen&lt;/li&gt;&lt;li&gt;Bag&lt;/li&gt;&lt;li&gt;Cup &lt;&amp;&gt;&lt;/li&gt;↵&lt;/ul&gt;</code> |  |
| <code>[data-bound].DataTextField=text</code> | <code>[data-bound] c.DataTextField = &quot;probe&quot;;</code> | <code></code> | <code>DataBind -&gt; System.Web.HttpException: DataBinding: &#39;WebControlsProbe.ProbeData+Product&#39; does not contain a property with the name &#39;probe&#39;.</code> |
| <code>[data-bound].DataTextField=Name+DataValueField=Code+SelectedValue=B2</code> | <code>[data-bound] c.DataTextField = &quot;Name&quot;; c.DataValueField = &quot;Code&quot;; c.SelectedValue = &quot;B2&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Pen&lt;/li&gt;&lt;li&gt;Bag&lt;/li&gt;&lt;li&gt;Cup &lt;&amp;&gt;&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.SelectedValue = &quot;B2&quot;; -&gt; System.NotSupportedException: Setting the SelectedIndex or SelectedValue properties of BulletedList is not supported.</code> |
| <code>[data-bound].DataTextField=Name+DataValueField=Code+SelectedValue=M</code> | <code>[data-bound] c.DataTextField = &quot;Name&quot;; c.DataValueField = &quot;Code&quot;; c.SelectedValue = &quot;M&quot;;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Pen&lt;/li&gt;&lt;li&gt;Bag&lt;/li&gt;&lt;li&gt;Cup &lt;&amp;&gt;&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.SelectedValue = &quot;M&quot;; -&gt; System.NotSupportedException: Setting the SelectedIndex or SelectedValue properties of BulletedList is not supported.</code> |
| <code>[data-bound].DataTextField=Name+DataValueField=Code+SelectedIndex=1</code> | <code>[data-bound] c.DataTextField = &quot;Name&quot;; c.DataValueField = &quot;Code&quot;; c.SelectedIndex = 1;</code> | <code>&lt;ul id=&quot;bltSize&quot;&gt;↵	&lt;li&gt;Pen&lt;/li&gt;&lt;li&gt;Bag&lt;/li&gt;&lt;li&gt;Cup &lt;&amp;&gt;&lt;/li&gt;↵&lt;/ul&gt;</code> | <code>c.SelectedIndex = 1; -&gt; System.NotSupportedException: Setting the SelectedIndex or SelectedValue properties of BulletedList is not supported.</code> |

## ③ シナリオ層

ブラウザ（Playwright）で BulletedListScenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。

| # | 操作 | 対象 | 値 |
|---|---|---|---|
| 1 | goto | /ListControlsScenario.aspx | <code></code> |
| 2 | snapshot |  | **BulletedList [render][databind][items] first request (a bound list)** |
| 3 | clicktext |  | <code>Second</code> |
| 4 | snapshot |  | **BulletedList [postbackevent] a bullet clicked (LinkButton)** |
| 5 | clicktext |  | <code>Validate g1 by a bullet</code> |
| 6 | snapshot |  | **BulletedList [causesvalidation][postbackevent] a bullet with ValidationGroup g1** |
| 7 | clicktext |  | <code>Two</code> |
| 8 | snapshot |  | **BulletedList [dynamic][postbackevent] a dynamically created list&#39;s bullet** |
| 9 | click | btnShow | <code></code> |
| 10 | snapshot |  | **BulletedList [visible][viewstate][items] hidden list shown with the items added in code** |
| 11 | evaluate | bltLinks | <code>__doPostBack(&#39;bltLinks&#39;, &#39;2&#39;);</code> |
| 12 | waitms |  | <code></code> |
| 13 | snapshot |  | **BulletedList [eventvalidation][postbackevent] a disabled bullet&#39;s postback forged** |

<details><summary>BulletedList [render][databind][items] first request (a bound list)（期待される本文）</summary>

```text
DropDownList:
Small
Medium
Large
Auto:
Small
Medium
Large
Bound:
(choose)
Pen
Bag
Cup <&>
No view state:
Small
Medium
ListBox (multiple):
Red
Green
Blue
Auto:
Red
Green
CheckBoxList:
Red Green Blue
Auto: X
Y
RadioButtonList:
Small Medium Large
Auto: X
Y
Validated (g1, server):
(choose)
A
A
B
A
Validated (g3, in the browser):
(choose)
A
A
A
BulletedList:
First
Second
Third
Validate g1 by a bullet
Pen
Bag
Cup <&>
Dynamic:
One
Two
One
Two
One
TwoOne
Two
One
Two
Hidden:
Pen
Bag
Pen
Bag
Pen
Bag Pen
Bag
Request 1: IsPostBack=False
List ddlSize: SelectedIndex=0 SelectedValue="S" selected [S] items 3
List ddlAuto: SelectedIndex=0 SelectedValue="S" selected [S] items 3
List ddlBound: SelectedIndex=0 SelectedValue="" selected [] items 4
List ddlNoViewState: SelectedIndex=0 SelectedValue="S" selected [S] items 2
List lstMulti: SelectedIndex=-1 SelectedValue="" selected [] items 3
List lstAuto: SelectedIndex=-1 SelectedValue="" selected [] items 2
List cblColors: SelectedIndex=-1 SelectedValue="" selected [] items 3
List cblAuto: SelectedIndex=-1 SelectedValue="" selected [] items 2
List rblSize: SelectedIndex=-1 SelectedValue="" selected [] items 3
List rblAuto: SelectedIndex=-1 SelectedValue="" selected [] items 2
List ddlValidated: SelectedIndex=0 SelectedValue="" selected [] items 2
List lstValidated: SelectedIndex=-1 SelectedValue="" selected [] items 2
List cblValidated: SelectedIndex=-1 SelectedValue="" selected [] items 1
List ddlClient: SelectedIndex=0 SelectedValue="" selected [] items 2
List lstClient: SelectedIndex=-1 SelectedValue="" selected [] items 1
List rblClient: SelectedIndex=-1 SelectedValue="" selected [] items 1
List ddlDynamic: SelectedIndex=0 SelectedValue="1" selected [1] items 2
List lstDynamic: SelectedIndex=-1 SelectedValue="" selected [] items 2
List cblDynamic: SelectedIndex=-1 SelectedValue="" selected [] items 2
List rblDynamic: SelectedIndex=-1 SelectedValue="" selected [] items 2
List bltDynamic: SelectedIndex=- SelectedValue="" selected [] items 2
List ddlHidden: SelectedIndex=1 SelectedValue="k2" selected [k2] items 2 (hidden)
List lstHidden: SelectedIndex=1 SelectedValue="k2" selected [k2] items 2 (hidden)
List cblHidden: SelectedIndex=1 SelectedValue="k2" selected [k2] items 2 (hidden)
List rblHidden: SelectedIndex=1 SelectedValue="k2" selected [k2] items 2 (hidden)
List bltHidden: SelectedIndex=- SelectedValue="" selected [] items 2 (hidden)
List bltBound: SelectedIndex=- SelectedValue="" selected [] items 3
```

</details>

<details><summary>BulletedList [postbackevent] a bullet clicked (LinkButton)（期待される本文）</summary>

```text
DropDownList:
Small
Medium
Large
Auto:
Small
Medium
Large
Bound:
(choose)
Pen
Bag
Cup <&>
No view state:
Small
Medium
ListBox (multiple):
Red
Green
Blue
Auto:
Red
Green
CheckBoxList:
Red Green Blue
Auto: X
Y
RadioButtonList:
Small Medium Large
Auto: X
Y
Validated (g1, server):
(choose)
A
A
B
A
Validated (g3, in the browser):
(choose)
A
A
A
BulletedList:
First
Second
Third
Validate g1 by a bullet
Pen
Bag
Cup <&>
Dynamic:
One
Two
One
Two
One
TwoOne
Two
One
Two
Hidden:
Pen
Bag
Pen
Bag
Pen
Bag Pen
Bag
Request 2: IsPostBack=True
Click bltLinks index 1
IsValid: (not validated)
List ddlSize: SelectedIndex=0 SelectedValue="S" selected [S] items 3
List ddlAuto: SelectedIndex=0 SelectedValue="S" selected [S] items 3
List ddlBound: SelectedIndex=0 SelectedValue="" selected [] items 4
List ddlNoViewState: SelectedIndex=0 SelectedValue="S" selected [S] items 2
List lstMulti: SelectedIndex=-1 SelectedValue="" selected [] items 3
List lstAuto: SelectedIndex=-1 SelectedValue="" selected [] items 2
List cblColors: SelectedIndex=-1 SelectedValue="" selected [] items 3
List cblAuto: SelectedIndex=-1 SelectedValue="" selected [] items 2
List rblSize: SelectedIndex=-1 SelectedValue="" selected [] items 3
List rblAuto: SelectedIndex=-1 SelectedValue="" selected [] items 2
List ddlValidated: SelectedIndex=0 SelectedValue="" selected [] items 2
List lstValidated: SelectedIndex=-1 SelectedValue="" selected [] items 2
List cblValidated: SelectedIndex=-1 SelectedValue="" selected [] items 1
List ddlClient: SelectedIndex=0 SelectedValue="" selected [] items 2
List lstClient: SelectedIndex=-1 SelectedValue="" selected [] items 1
List rblClient: SelectedIndex=-1 SelectedValue="" selected [] items 1
List ddlDynamic: SelectedIndex=0 SelectedValue="1" selected [1] items 2
List lstDynamic: SelectedIndex=-1 SelectedValue="" selected [] items 2
List cblDynamic: SelectedIndex=-1 SelectedValue="" selected [] items 2
List rblDynamic: SelectedIndex=-1 SelectedValue="" selected [] items 2
List bltDynamic: SelectedIndex=- SelectedValue="" selected [] items 2
List ddlHidden: SelectedIndex=1 SelectedValue="k2" selected [k2] items 2 (hidden)
List lstHidden: SelectedIndex=1 SelectedValue="k2" selected [k2] items 2 (hidden)
List cblHidden: SelectedIndex=1 SelectedValue="k2" selected [k2] items 2 (hidden)
List rblHidden: SelectedIndex=1 SelectedValue="k2" selected [k2] items 2 (hidden)
List bltHidden: SelectedIndex=- SelectedValue="" selected [] items 2 (hidden)
List bltBound: SelectedIndex=- SelectedValue="" selected [] items 3
```

</details>

<details><summary>BulletedList [causesvalidation][postbackevent] a bullet with ValidationGroup g1（期待される本文）</summary>

```text
DropDownList:
Small
Medium
Large
Auto:
Small
Medium
Large
Bound:
(choose)
Pen
Bag
Cup <&>
No view state:
Small
Medium
ListBox (multiple):
Red
Green
Blue
Auto:
Red
Green
CheckBoxList:
Red Green Blue
Auto: X
Y
RadioButtonList:
Small Medium Large
Auto: X
Y
Validated (g1, server):
(choose)
A
A
B
A
Validated (g3, in the browser):
(choose)
A
A
A
BulletedList:
First
Second
Third
Validate g1 by a bullet
Pen
Bag
Cup <&>
Dynamic:
One
Two
One
Two
One
TwoOne
Two
One
Two
Hidden:
Pen
Bag
Pen
Bag
Pen
Bag Pen
Bag
Request 3: IsPostBack=True
Click bltValidated index 0
IsValid: (not validated)
List ddlSize: SelectedIndex=0 SelectedValue="S" selected [S] items 3
List ddlAuto: SelectedIndex=0 SelectedValue="S" selected [S] items 3
List ddlBound: SelectedIndex=0 SelectedValue="" selected [] items 4
List ddlNoViewState: SelectedIndex=0 SelectedValue="S" selected [S] items 2
List lstMulti: SelectedIndex=-1 SelectedValue="" selected [] items 3
List lstAuto: SelectedIndex=-1 SelectedValue="" selected [] items 2
List cblColors: SelectedIndex=-1 SelectedValue="" selected [] items 3
List cblAuto: SelectedIndex=-1 SelectedValue="" selected [] items 2
List rblSize: SelectedIndex=-1 SelectedValue="" selected [] items 3
List rblAuto: SelectedIndex=-1 SelectedValue="" selected [] items 2
List ddlValidated: SelectedIndex=0 SelectedValue="" selected [] items 2
List lstValidated: SelectedIndex=-1 SelectedValue="" selected [] items 2
List cblValidated: SelectedIndex=-1 SelectedValue="" selected [] items 1
List ddlClient: SelectedIndex=0 SelectedValue="" selected [] items 2
List lstClient: SelectedIndex=-1 SelectedValue="" selected [] items 1
List rblClient: SelectedIndex=-1 SelectedValue="" selected [] items 1
List ddlDynamic: SelectedIndex=0 SelectedValue="1" selected [1] items 2
List lstDynamic: SelectedIndex=-1 SelectedValue="" selected [] items 2
List cblDynamic: SelectedIndex=-1 SelectedValue="" selected [] items 2
List rblDynamic: SelectedIndex=-1 SelectedValue="" selected [] items 2
List bltDynamic: SelectedIndex=- SelectedValue="" selected [] items 2
List ddlHidden: SelectedIndex=1 SelectedValue="k2" selected [k2] items 2 (hidden)
List lstHidden: SelectedIndex=1 SelectedValue="k2" selected [k2] items 2 (hidden)
List cblHidden: SelectedIndex=1 SelectedValue="k2" selected [k2] items 2 (hidden)
List rblHidden: SelectedIndex=1 SelectedValue="k2" selected [k2] items 2 (hidden)
List bltHidden: SelectedIndex=- SelectedValue="" selected [] items 2 (hidden)
List bltBound: SelectedIndex=- SelectedValue="" selected [] items 3
```

</details>

<details><summary>BulletedList [dynamic][postbackevent] a dynamically created list&#39;s bullet（期待される本文）</summary>

```text
DropDownList:
Small
Medium
Large
Auto:
Small
Medium
Large
Bound:
(choose)
Pen
Bag
Cup <&>
No view state:
Small
Medium
ListBox (multiple):
Red
Green
Blue
Auto:
Red
Green
CheckBoxList:
Red Green Blue
Auto: X
Y
RadioButtonList:
Small Medium Large
Auto: X
Y
Validated (g1, server):
(choose)
A
A
B
A
Validated (g3, in the browser):
(choose)
A
A
A
BulletedList:
First
Second
Third
Validate g1 by a bullet
Pen
Bag
Cup <&>
Dynamic:
One
Two
One
Two
One
TwoOne
Two
One
Two
Hidden:
Pen
Bag
Pen
Bag
Pen
Bag Pen
Bag
Request 4: IsPostBack=True
Click bltDynamic index 1
IsValid: (not validated)
List ddlSize: SelectedIndex=0 SelectedValue="S" selected [S] items 3
List ddlAuto: SelectedIndex=0 SelectedValue="S" selected [S] items 3
List ddlBound: SelectedIndex=0 SelectedValue="" selected [] items 4
List ddlNoViewState: SelectedIndex=0 SelectedValue="S" selected [S] items 2
List lstMulti: SelectedIndex=-1 SelectedValue="" selected [] items 3
List lstAuto: SelectedIndex=-1 SelectedValue="" selected [] items 2
List cblColors: SelectedIndex=-1 SelectedValue="" selected [] items 3
List cblAuto: SelectedIndex=-1 SelectedValue="" selected [] items 2
List rblSize: SelectedIndex=-1 SelectedValue="" selected [] items 3
List rblAuto: SelectedIndex=-1 SelectedValue="" selected [] items 2
List ddlValidated: SelectedIndex=0 SelectedValue="" selected [] items 2
List lstValidated: SelectedIndex=-1 SelectedValue="" selected [] items 2
List cblValidated: SelectedIndex=-1 SelectedValue="" selected [] items 1
List ddlClient: SelectedIndex=0 SelectedValue="" selected [] items 2
List lstClient: SelectedIndex=-1 SelectedValue="" selected [] items 1
List rblClient: SelectedIndex=-1 SelectedValue="" selected [] items 1
List ddlDynamic: SelectedIndex=0 SelectedValue="1" selected [1] items 2
List lstDynamic: SelectedIndex=-1 SelectedValue="" selected [] items 2
List cblDynamic: SelectedIndex=-1 SelectedValue="" selected [] items 2
List rblDynamic: SelectedIndex=-1 SelectedValue="" selected [] items 2
List bltDynamic: SelectedIndex=- SelectedValue="" selected [] items 2
List ddlHidden: SelectedIndex=1 SelectedValue="k2" selected [k2] items 2 (hidden)
List lstHidden: SelectedIndex=1 SelectedValue="k2" selected [k2] items 2 (hidden)
List cblHidden: SelectedIndex=1 SelectedValue="k2" selected [k2] items 2 (hidden)
List rblHidden: SelectedIndex=1 SelectedValue="k2" selected [k2] items 2 (hidden)
List bltHidden: SelectedIndex=- SelectedValue="" selected [] items 2 (hidden)
List bltBound: SelectedIndex=- SelectedValue="" selected [] items 3
```

</details>

<details><summary>BulletedList [visible][viewstate][items] hidden list shown with the items added in code（期待される本文）</summary>

```text
DropDownList:
Small
Medium
Large
Auto:
Small
Medium
Large
Bound:
(choose)
Pen
Bag
Cup <&>
No view state:
Small
Medium
ListBox (multiple):
Red
Green
Blue
Auto:
Red
Green
CheckBoxList:
Red Green Blue
Auto: X
Y
RadioButtonList:
Small Medium Large
Auto: X
Y
Validated (g1, server):
(choose)
A
A
B
A
Validated (g3, in the browser):
(choose)
A
A
A
BulletedList:
First
Second
Third
Validate g1 by a bullet
Pen
Bag
Cup <&>
Dynamic:
One
Two
One
Two
One
TwoOne
Two
One
Two
Hidden:
Kept 1
Kept 2
Kept 1
Kept 2
Kept 1
Kept 2
Kept 1
Kept 2
Kept 1
Kept 2
Pen
Bag
Pen
Bag
Pen
Bag Pen
Bag
Request 5: IsPostBack=True
Click btnShow
IsValid: (not validated)
List ddlSize: SelectedIndex=0 SelectedValue="S" selected [S] items 3
List ddlAuto: SelectedIndex=0 SelectedValue="S" selected [S] items 3
List ddlBound: SelectedIndex=0 SelectedValue="" selected [] items 4
List ddlNoViewState: SelectedIndex=0 SelectedValue="S" selected [S] items 2
List lstMulti: SelectedIndex=-1 SelectedValue="" selected [] items 3
List lstAuto: SelectedIndex=-1 SelectedValue="" selected [] items 2
List cblColors: SelectedIndex=-1 SelectedValue="" selected [] items 3
List cblAuto: SelectedIndex=-1 SelectedValue="" selected [] items 2
List rblSize: SelectedIndex=-1 SelectedValue="" selected [] items 3
List rblAuto: SelectedIndex=-1 SelectedValue="" selected [] items 2
List ddlValidated: SelectedIndex=0 SelectedValue="" selected [] items 2
List lstValidated: SelectedIndex=-1 SelectedValue="" selected [] items 2
List cblValidated: SelectedIndex=-1 SelectedValue="" selected [] items 1
List ddlClient: SelectedIndex=0 SelectedValue="" selected [] items 2
List lstClient: SelectedIndex=-1 SelectedValue="" selected [] items 1
List rblClient: SelectedIndex=-1 SelectedValue="" selected [] items 1
List ddlDynamic: SelectedIndex=0 SelectedValue="1" selected [1] items 2
List lstDynamic: SelectedIndex=-1 SelectedValue="" selected [] items 2
List cblDynamic: SelectedIndex=-1 SelectedValue="" selected [] items 2
List rblDynamic: SelectedIndex=-1 SelectedValue="" selected [] items 2
List bltDynamic: SelectedIndex=- SelectedValue="" selected [] items 2
List ddlHidden: SelectedIndex=1 SelectedValue="k2" selected [k2] items 2
List lstHidden: SelectedIndex=1 SelectedValue="k2" selected [k2] items 2
List cblHidden: SelectedIndex=1 SelectedValue="k2" selected [k2] items 2
List rblHidden: SelectedIndex=1 SelectedValue="k2" selected [k2] items 2
List bltHidden: SelectedIndex=- SelectedValue="" selected [] items 2
List bltBound: SelectedIndex=- SelectedValue="" selected [] items 3
```

</details>

<details><summary>BulletedList [eventvalidation][postbackevent] a disabled bullet&#39;s postback forged（期待される本文）</summary>

```text
System.ArgumentException
Invalid postback or callback argument. Event validation is enabled using <pages enableEventValidation="true"/> in configuration or <%@ Page EnableEventValidation="true" %> in a page. For security purposes, this feature verifies that arguments to postback or callback events originate from the server control that originally rendered them. If the data is valid and expected, use the ClientScriptManager.RegisterForEventValidation method in order to register the postback or callback data for validation.
```

</details>

