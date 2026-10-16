# DataPager テストケース一覧

変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。
このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。

- 正解データの採取: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0
- ① API 層: 137 ケース（144 メンバー）
- ② HTML 層: 60 ケース
- ③ シナリオ層: 8 スナップショット

## 機構マトリクス

型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。

| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |
|---|---|---|---|
| プロパティ → HTML 描画 | すべてのコントロール | 47 | 1 |
| ViewState の保存と復元 | Control | 6 | 2 |
| Visible=false（描画されず状態は保持） | Control.Visible | 2 | 0 |
| 名前付けコンテナと ClientIDMode | Control.ClientIDMode | 6 | 0 |
| テーマとスキン（SkinID / EnableTheming） | テーマを付けられる型（[Themeable]） | 3 | 0 |
| 動的に生成したコントロールのライフサイクル | Control | 0 | 2 |
| データバインド（DataBind / <%# %>） | Control.DataBind | 0 | 1 |
| ページング | AllowPaging プロパティ、IPageableItemContainer（ListView）、DataPager | 7 | 6 |

該当しない機構: Enabled=false・Disabled=true（描画とポスト値の扱い）、ポストバックデータの取り込み、変更イベント（*Changed、ServerChange）、ReadOnly（ポストされた値を無視）、リクエスト検証（ValidateRequestMode）、ポストバックイベント、コマンドイベントと親へのバブル（CommandName）、送信ボタン／スクリプトによるポストバック（UseSubmitBehavior）、ブラウザ側のクリック処理（OnClientClick）、クロスページポストバック（PostBackUrl）、既定のボタン（Panel の DefaultButton）、AutoPostBack（クライアントスクリプトによるポストバック）、CausesValidation / ValidationGroup、検証対象（サーバー側の検証）、検証対象（ブラウザ側の検証）、双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値）、項目（ListItem の Selected・Enabled・Text・Value と ViewState）、複数選択、繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）、ポスト値のイベント検証（一覧にない値・無効な項目の拒否）、サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）、ブラウザ側の検証スクリプト（EnableClientScript）、検証結果のまとめ（ValidationSummary）、並べ替え、項目の挿入、行・項目の編集（編集・更新・取り消し・削除）、行・項目の選択、行・項目のコマンド（RowCommand・ItemCommand へのバブル）、データキー（DataKeyNames・DataKeys）、データソースコントロールとの連携（DataSourceID）、テンプレート、データソース連携（DataSource・DataTextField など）、クライアントコールバック

## ① API 層

DataPager とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の DataPager インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。

| ケース | 期待値（.NET Framework） |
|---|---|
| <code>Api.Control.#ctor</code> | <code>Control.#ctor = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMode=In…</code> |
| <code>Api.Control.Adapter</code> | <code>Control.Adapter = null</code> |
| <code>Api.Control.AddParsedSubObject</code> | <code>Control.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>Control.AddParsedSubObject(Object) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; …</code> |
| <code>Api.Control.AddedControl</code> | <code>Control.AddedControl(Control,Int32) = &quot;ok&quot;</code><br><code>Control.AddedControl(Control,Int32) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;;…</code> |
| <code>Api.Control.AppRelativeTemplateSourceDirectory</code> | <code>Control.AppRelativeTemplateSourceDirectory = &quot;&quot;</code><br><code>Control.AppRelativeTemplateSourceDirectory set = &quot;-&gt; System.ArgumentException: The relative virtual path &#39;Abc 123&#39; is not allowed here.&quot;</code> |
| <code>Api.Control.ApplyStyleSheetSkin</code> | <code>Control.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>Control.ApplyStyleSheetSkin(Page) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; M…</code> |
| <code>Api.Control.BeginRenderTracing</code> | <code>Control.BeginRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.BeginRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.BeginRenderTracing(TextWriter,Object) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=…</code> |
| <code>Api.Control.BindingContainer</code> | <code>Control.BindingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Clien…</code> |
| <code>Api.Control.BuildProfileTree</code> | <code>Control.BuildProfileTree(String,Boolean) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.ChildControlsCreated</code> | <code>Control.ChildControlsCreated = &quot;false&quot;</code><br><code>Control.ChildControlsCreated set = &quot;ok&quot;</code> |
| <code>Api.Control.ClearCachedClientID</code> | <code>Control.ClearCachedClientID = &quot;ok&quot;</code><br><code>Control.ClearCachedClientID after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; Maximum…</code> |
| <code>Api.Control.ClearChildControlState</code> | <code>Control.ClearChildControlState = &quot;ok&quot;</code><br><code>Control.ClearChildControlState after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; Maxi…</code> |
| <code>Api.Control.ClearChildState</code> | <code>Control.ClearChildState = &quot;ok&quot;</code><br><code>Control.ClearChildState after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; MaximumRows…</code> |
| <code>Api.Control.ClearChildViewState</code> | <code>Control.ClearChildViewState = &quot;ok&quot;</code><br><code>Control.ClearChildViewState after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; Maximum…</code> |
| <code>Api.Control.ClearEffectiveClientIDMode</code> | <code>Control.ClearEffectiveClientIDMode = &quot;ok&quot;</code><br><code>Control.ClearEffectiveClientIDMode after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; …</code> |
| <code>Api.Control.ClientID</code> | <code>Control.ClientID = &quot;dpPager&quot;</code> |
| <code>Api.Control.ClientIDMode</code> | <code>Control.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>Control.ClientIDMode set = &quot;ClientIDMode.AutoID&quot;</code> |
| <code>Api.Control.ClientIDSeparator</code> | <code>Control.ClientIDSeparator = &quot;&#39;_&#39;&quot;</code> |
| <code>Api.Control.Context</code> | <code>Control.Context = null</code> |
| <code>Api.Control.Controls</code> | <code>Control.Controls = ControlCollection []</code> |
| <code>Api.Control.CreateChildControls</code> | <code>Control.CreateChildControls = &quot;ok&quot;</code><br><code>Control.CreateChildControls after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; Maximum…</code> |
| <code>Api.Control.CreateControlCollection</code> | <code>Control.CreateControlCollection = ControlCollection []</code><br><code>Control.CreateControlCollection after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; Max…</code> |
| <code>Api.Control.DataBind</code> | <code>Control.DataBind = &quot;ok&quot;</code><br><code>Control.DataBind after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; MaximumRows=Int32 …</code><br><code>Control.DataBind(Boolean) = &quot;ok&quot;</code><br><code>Control.DataBind(Boolean) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; MaximumRo…</code> |
| <code>Api.Control.DataBindChildren</code> | <code>Control.DataBindChildren = &quot;ok&quot;</code><br><code>Control.DataBindChildren after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; MaximumRow…</code> |
| <code>Api.Control.DataBinding</code> | <code>Control.DataBinding = &quot;added and removed&quot;</code> |
| <code>Api.Control.DataItemContainer</code> | <code>Control.DataItemContainer = null</code> |
| <code>Api.Control.DataKeysContainer</code> | <code>Control.DataKeysContainer = null</code> |
| <code>Api.Control.DesignMode</code> | <code>Control.DesignMode = &quot;false&quot;</code> |
| <code>Api.Control.Dispose</code> | <code>Control.Dispose = &quot;ok&quot;</code><br><code>Control.Dispose after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; MaximumRows=Int32 1…</code> |
| <code>Api.Control.Disposed</code> | <code>Control.Disposed = &quot;added and removed&quot;</code> |
| <code>Api.Control.EnableTheming</code> | <code>Control.EnableTheming = &quot;true&quot;</code><br><code>Control.EnableTheming set = &quot;true&quot;</code> |
| <code>Api.Control.EnableViewState</code> | <code>Control.EnableViewState = &quot;true&quot;</code><br><code>Control.EnableViewState set = &quot;true&quot;</code> |
| <code>Api.Control.EndRenderTracing</code> | <code>Control.EndRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.EndRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.EndRenderTracing(TextWriter,Object) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;d…</code> |
| <code>Api.Control.EnsureChildControls</code> | <code>Control.EnsureChildControls = &quot;ok&quot;</code><br><code>Control.EnsureChildControls after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; Maximum…</code> |
| <code>Api.Control.EnsureID</code> | <code>Control.EnsureID = &quot;ok&quot;</code><br><code>Control.EnsureID after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; MaximumRows=Int32 …</code> |
| <code>Api.Control.Events</code> | <code>Control.Events = &quot;System.ComponentModel.EventHandlerList&quot;</code> |
| <code>Api.Control.FindControl</code> | <code>Control.FindControl(String) = null</code><br><code>Control.FindControl(String) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; Maximum…</code><br><code>Control.FindControl(String,Int32) = null</code><br><code>Control.FindControl(String,Int32) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; M…</code> |
| <code>Api.Control.Focus</code> | <code>Control.Focus = &quot;-&gt; System.InvalidOperationException: A form tag with runat=server must exist on the Page to use SetFocus() or the Focus property.&quot;</code> |
| <code>Api.Control.GetDesignModeState</code> | <code>Control.GetDesignModeState = &quot;[]&quot;</code><br><code>Control.GetDesignModeState after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; MaximumR…</code> |
| <code>Api.Control.GetRouteUrl</code> | <code>Control.GetRouteUrl(Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.GetUniqueIDRelativeTo</code> | <code>Control.GetUniqueIDRelativeTo(Control) = &quot;dpPager&quot;</code><br><code>Control.GetUniqueIDRelativeTo(Control) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPage…</code> |
| <code>Api.Control.HasChildViewState</code> | <code>Control.HasChildViewState = &quot;false&quot;</code> |
| <code>Api.Control.HasControls</code> | <code>Control.HasControls = &quot;false&quot;</code><br><code>Control.HasControls after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; MaximumRows=Int…</code> |
| <code>Api.Control.HasEvents</code> | <code>Control.HasEvents = &quot;false&quot;</code><br><code>Control.HasEvents after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; MaximumRows=Int32…</code> |
| <code>Api.Control.ID</code> | <code>Control.ID = &quot;dpPager&quot;</code><br><code>Control.ID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.IdSeparator</code> | <code>Control.IdSeparator = &quot;&#39;$&#39;&quot;</code> |
| <code>Api.Control.Init</code> | <code>Control.Init = &quot;added and removed&quot;</code> |
| <code>Api.Control.IsChildControlStateCleared</code> | <code>Control.IsChildControlStateCleared = &quot;false&quot;</code> |
| <code>Api.Control.IsLiteralContent</code> | <code>Control.IsLiteralContent = &quot;false&quot;</code><br><code>Control.IsLiteralContent after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; MaximumRow…</code> |
| <code>Api.Control.IsTrackingViewState</code> | <code>Control.IsTrackingViewState = &quot;false&quot;</code> |
| <code>Api.Control.IsViewStateEnabled</code> | <code>Control.IsViewStateEnabled = &quot;true&quot;</code> |
| <code>Api.Control.Load</code> | <code>Control.Load = &quot;added and removed&quot;</code> |
| <code>Api.Control.LoadControlState</code> | <code>Control.LoadControlState(Object) = &quot;ok&quot;</code><br><code>Control.LoadControlState(Object) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; Ma…</code> |
| <code>Api.Control.LoadViewState</code> | <code>Control.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Object[]&#39;.&quot;</code> |
| <code>Api.Control.LoadViewStateByID</code> | <code>Control.LoadViewStateByID = &quot;false&quot;</code> |
| <code>Api.Control.MapPathSecure</code> | <code>Control.MapPathSecure(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.NamingContainer</code> | <code>Control.NamingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code> |
| <code>Api.Control.OnBubbleEvent</code> | <code>Control.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Control.OnBubbleEvent(Object,EventArgs) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPag…</code> |
| <code>Api.Control.OnDataBinding</code> | <code>Control.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnDataBinding(EventArgs) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; Ma…</code> |
| <code>Api.Control.OnInit</code> | <code>Control.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnInit(EventArgs) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; MaximumRo…</code> |
| <code>Api.Control.OnLoad</code> | <code>Control.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnLoad(EventArgs) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; MaximumRo…</code> |
| <code>Api.Control.OnPreRender</code> | <code>Control.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnPreRender(EventArgs) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; Maxi…</code> |
| <code>Api.Control.OnUnload</code> | <code>Control.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnUnload(EventArgs) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; Maximum…</code> |
| <code>Api.Control.OpenFile</code> | <code>Control.OpenFile(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.Page</code> | <code>Control.Page = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=&quot;&quot;; …</code><br><code>Control.Page set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=…</code> |
| <code>Api.Control.Parent</code> | <code>Control.Parent = HtmlForm#form1 {Action=&quot;&quot;; ClientID=&quot;form1&quot;; ClientIDMode=Inherit; DefaultButton=&quot;&quot;; DefaultFocus=&quot;&quot;; Disabled=false; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.PreRender</code> | <code>Control.PreRender = &quot;added and removed&quot;</code> |
| <code>Api.Control.RaiseBubbleEvent</code> | <code>Control.RaiseBubbleEvent(Object,EventArgs) = &quot;ok&quot;</code><br><code>Control.RaiseBubbleEvent(Object,EventArgs) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dp…</code> |
| <code>Api.Control.RemovedControl</code> | <code>Control.RemovedControl(Control) = &quot;ok&quot;</code><br><code>Control.RemovedControl(Control) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; Max…</code> |
| <code>Api.Control.Render</code> | <code>Control.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.Render(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;span id=&quot;dpPager&quot;&gt;&lt;/span&gt;</code><br><code>Control.Render(HtmlTextWriter) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; Maxi…</code> |
| <code>Api.Control.RenderChildren</code> | <code>Control.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderChildren(HtmlTextWriter) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPage…</code> |
| <code>Api.Control.RenderControl</code> | <code>Control.RenderControl(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;span id=&quot;dpPager&quot;&gt;&lt;/span&gt;</code><br><code>Control.RenderControl(HtmlTextWriter) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager…</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) writer:</code><br><code>  &#124; &lt;span id=&quot;dpPager&quot;&gt;&lt;/span&gt;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableVie…</code> |
| <code>Api.Control.RenderingCompatibility</code> | <code>Control.RenderingCompatibility = &quot;System.Version 4.0&quot;</code><br><code>Control.RenderingCompatibility set = &quot;System.Version 4.0&quot;</code> |
| <code>Api.Control.ResolveAdapter</code> | <code>Control.ResolveAdapter = null</code><br><code>Control.ResolveAdapter after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; MaximumRows=…</code> |
| <code>Api.Control.ResolveClientUrl</code> | <code>Control.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveClientUrl(String) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; Ma…</code> |
| <code>Api.Control.ResolveUrl</code> | <code>Control.ResolveUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveUrl(String) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; MaximumR…</code> |
| <code>Api.Control.SaveControlState</code> | <code>Control.SaveControlState = &quot;[null, null, Int32 1, Int32 0]&quot;</code><br><code>Control.SaveControlState after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; MaximumRow…</code> |
| <code>Api.Control.SaveViewState</code> | <code>Control.SaveViewState = &quot;[null, null]&quot;</code><br><code>Control.SaveViewState after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; MaximumRows=I…</code> |
| <code>Api.Control.SetDesignModeState</code> | <code>Control.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>Control.SetDesignModeState(IDictionary) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPag…</code> |
| <code>Api.Control.SetRenderMethodDelegate</code> | <code>Control.SetRenderMethodDelegate(RenderMethod) = &quot;ok&quot;</code><br><code>Control.SetRenderMethodDelegate(RenderMethod) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=…</code> |
| <code>Api.Control.SetTraceData</code> | <code>Control.SetTraceData(Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;;…</code><br><code>Control.SetTraceData(Object,Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object,Object) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dp…</code> |
| <code>Api.Control.Site</code> | <code>Control.Site = null</code><br><code>Control.Site set = null</code> |
| <code>Api.Control.SkinID</code> | <code>Control.SkinID = &quot;&quot;</code><br><code>Control.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.TemplateControl</code> | <code>Control.TemplateControl = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code><br><code>Control.TemplateControl set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Cl…</code> |
| <code>Api.Control.TemplateSourceDirectory</code> | <code>Control.TemplateSourceDirectory = &quot;&quot;</code> |
| <code>Api.Control.TrackViewState</code> | <code>Control.TrackViewState = &quot;ok&quot;</code><br><code>Control.TrackViewState after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; MaximumRows=…</code> |
| <code>Api.Control.UniqueID</code> | <code>Control.UniqueID = &quot;dpPager&quot;</code> |
| <code>Api.Control.Unload</code> | <code>Control.Unload = &quot;added and removed&quot;</code> |
| <code>Api.Control.ValidateRequestMode</code> | <code>Control.ValidateRequestMode = &quot;ValidateRequestMode.Inherit&quot;</code><br><code>Control.ValidateRequestMode set = &quot;ValidateRequestMode.Disabled&quot;</code> |
| <code>Api.Control.ViewState</code> | <code>Control.ViewState = StateBag [PagedControlID=&quot;lvPaged&quot;]</code> |
| <code>Api.Control.ViewStateIgnoresCase</code> | <code>Control.ViewStateIgnoresCase = &quot;false&quot;</code> |
| <code>Api.Control.ViewStateMode</code> | <code>Control.ViewStateMode = &quot;ViewStateMode.Inherit&quot;</code><br><code>Control.ViewStateMode set = &quot;ViewStateMode.Enabled&quot;</code> |
| <code>Api.Control.Visible</code> | <code>Control.Visible = &quot;true&quot;</code><br><code>Control.Visible set = &quot;true&quot;</code> |
| <code>Api.IAttributeAccessor.GetAttribute</code> | <code>IAttributeAccessor.GetAttribute(String) = null</code><br><code>IAttributeAccessor.GetAttribute(String) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPag…</code> |
| <code>Api.IAttributeAccessor.SetAttribute</code> | <code>IAttributeAccessor.SetAttribute(String,String) = &quot;ok&quot;</code><br><code>IAttributeAccessor.SetAttribute(String,String) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID…</code> |
| <code>Api.IControlBuilderAccessor.ControlBuilder</code> | <code>IControlBuilderAccessor.ControlBuilder = null</code> |
| <code>Api.IControlDesignerAccessor.GetDesignModeState</code> | <code>IControlDesignerAccessor.GetDesignModeState = &quot;[]&quot;</code><br><code>IControlDesignerAccessor.GetDesignModeState after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;d…</code> |
| <code>Api.IControlDesignerAccessor.SetDesignModeState</code> | <code>IControlDesignerAccessor.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetDesignModeState(IDictionary) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewStat…</code> |
| <code>Api.IControlDesignerAccessor.SetOwnerControl</code> | <code>IControlDesignerAccessor.SetOwnerControl(Control) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetOwnerControl(Control) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true;…</code> |
| <code>Api.IControlDesignerAccessor.UserData</code> | <code>IControlDesignerAccessor.UserData = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.DataBindings</code> | <code>IDataBindingsAccessor.DataBindings = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.HasDataBindings</code> | <code>IDataBindingsAccessor.HasDataBindings = &quot;false&quot;</code> |
| <code>Api.IExpressionsAccessor.Expressions</code> | <code>IExpressionsAccessor.Expressions = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.HasExpressions</code> | <code>IExpressionsAccessor.HasExpressions = &quot;false&quot;</code> |
| <code>Api.IParserAccessor.AddParsedSubObject</code> | <code>IParserAccessor.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>IParserAccessor.AddParsedSubObject(Object) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dp…</code> |
| <code>Api.IUrlResolutionService.ResolveClientUrl</code> | <code>IUrlResolutionService.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>IUrlResolutionService.ResolveClientUrl(String) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID…</code> |
| <code>Api.WebControls.DataPager.#ctor</code> | <code>DataPager.#ctor = DataPager# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; MaximumRows=Int32 10; PageSize=Int32 10; Pag…</code> |
| <code>Api.WebControls.DataPager.AddAttributesToRender</code> | <code>DataPager.AddAttributesToRender(HtmlTextWriter) = &quot;ok&quot;</code><br><code>DataPager.AddAttributesToRender(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>DataPager.AddAttributesToRender(HtmlTextWriter) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; I…</code> |
| <code>Api.WebControls.DataPager.Attributes</code> | <code>DataPager.Attributes = AttributeCollection []</code> |
| <code>Api.WebControls.DataPager.ConnectToEvents</code> | <code>DataPager.ConnectToEvents(IPageableItemContainer) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.WebControls.DataPager.Controls</code> | <code>DataPager.Controls = ControlCollection []</code> |
| <code>Api.WebControls.DataPager.CreatePagerFields</code> | <code>DataPager.CreatePagerFields = &quot;ok&quot;</code><br><code>DataPager.CreatePagerFields after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; Maximum…</code> |
| <code>Api.WebControls.DataPager.DataBind</code> | <code>DataPager.DataBind = &quot;ok&quot;</code><br><code>DataPager.DataBind after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; MaximumRows=Int3…</code> |
| <code>Api.WebControls.DataPager.Fields</code> | <code>DataPager.Fields = &quot;[System.Web.UI.WebControls.NumericPagerField]&quot;</code> |
| <code>Api.WebControls.DataPager.FindPageableItemContainer</code> | <code>DataPager.FindPageableItemContainer = ListView#lvPaged {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Border…</code><br><code>DataPager.FindPageableItemContainer after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;;…</code> |
| <code>Api.WebControls.DataPager.LoadControlState</code> | <code>DataPager.LoadControlState(Object) = &quot;ok&quot;</code><br><code>DataPager.LoadControlState(Object) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; …</code> |
| <code>Api.WebControls.DataPager.LoadViewState</code> | <code>DataPager.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Object[]&#39;.&quot;</code> |
| <code>Api.WebControls.DataPager.MaximumRows</code> | <code>DataPager.MaximumRows = &quot;Int32 1&quot;</code> |
| <code>Api.WebControls.DataPager.OnBubbleEvent</code> | <code>DataPager.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>DataPager.OnBubbleEvent(Object,EventArgs) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpP…</code> |
| <code>Api.WebControls.DataPager.OnInit</code> | <code>DataPager.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>DataPager.OnInit(EventArgs) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; Maximum…</code> |
| <code>Api.WebControls.DataPager.OnLoad</code> | <code>DataPager.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>DataPager.OnLoad(EventArgs) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; Maximum…</code> |
| <code>Api.WebControls.DataPager.OnTotalRowCountAvailable</code> | <code>DataPager.OnTotalRowCountAvailable(Object,PageEventArgs) = &quot;ok&quot;</code><br><code>DataPager.OnTotalRowCountAvailable(Object,PageEventArgs) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewStat…</code> |
| <code>Api.WebControls.DataPager.PageSize</code> | <code>DataPager.PageSize = &quot;Int32 1&quot;</code><br><code>DataPager.PageSize set = &quot;Int32 1&quot;</code> |
| <code>Api.WebControls.DataPager.PagedControlID</code> | <code>DataPager.PagedControlID = &quot;lvPaged&quot;</code><br><code>DataPager.PagedControlID set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.DataPager.QueryStringField</code> | <code>DataPager.QueryStringField = &quot;&quot;</code><br><code>DataPager.QueryStringField set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.DataPager.RecreateChildControls</code> | <code>DataPager.RecreateChildControls = &quot;ok&quot;</code><br><code>DataPager.RecreateChildControls after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; Max…</code> |
| <code>Api.WebControls.DataPager.Render</code> | <code>DataPager.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>DataPager.Render(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;span id=&quot;dpPager&quot;&gt;&lt;/span&gt;</code><br><code>DataPager.Render(HtmlTextWriter) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; Ma…</code> |
| <code>Api.WebControls.DataPager.RenderBeginTag</code> | <code>DataPager.RenderBeginTag(HtmlTextWriter) = &quot;ok&quot;</code><br><code>DataPager.RenderBeginTag(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;span id=&quot;dpPager&quot;&gt;</code><br><code>DataPager.RenderBeginTag(HtmlTextWriter) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPa…</code> |
| <code>Api.WebControls.DataPager.RenderContents</code> | <code>DataPager.RenderContents(HtmlTextWriter) = &quot;ok&quot;</code><br><code>DataPager.RenderContents(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>DataPager.RenderContents(HtmlTextWriter) after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPa…</code> |
| <code>Api.WebControls.DataPager.SaveControlState</code> | <code>DataPager.SaveControlState = &quot;[null, null, Int32 1, Int32 0]&quot;</code><br><code>DataPager.SaveControlState after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; MaximumR…</code> |
| <code>Api.WebControls.DataPager.SaveViewState</code> | <code>DataPager.SaveViewState = &quot;[null, null]&quot;</code><br><code>DataPager.SaveViewState after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; MaximumRows…</code> |
| <code>Api.WebControls.DataPager.SetPageProperties</code> | <code>DataPager.SetPageProperties(Int32,Int32,Boolean) = &quot;-&gt; System.InvalidOperationException: Page properties cannot be set because no IPageableItemContainer has bee…</code> |
| <code>Api.WebControls.DataPager.StartRowIndex</code> | <code>DataPager.StartRowIndex = &quot;Int32 0&quot;</code> |
| <code>Api.WebControls.DataPager.TagKey</code> | <code>DataPager.TagKey = &quot;HtmlTextWriterTag.Span&quot;</code> |
| <code>Api.WebControls.DataPager.TotalRowCount</code> | <code>DataPager.TotalRowCount = &quot;Int32 0&quot;</code> |
| <code>Api.WebControls.DataPager.TrackViewState</code> | <code>DataPager.TrackViewState = &quot;ok&quot;</code><br><code>DataPager.TrackViewState after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;dpPager&quot;; MaximumRow…</code> |
| <code>Api.WebControls.ICompositeControlDesignerAccessor.RecreateChildControls</code> | <code>ICompositeControlDesignerAccessor.RecreateChildControls = &quot;ok&quot;</code><br><code>ICompositeControlDesignerAccessor.RecreateChildControls after = DataPager#dpPager {ClientID=&quot;dpPager&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState…</code> |

## ② HTML 層

プローブサイトの RenderProbe.aspx で、1 ケース 1 リクエスト（ページライフサイクル全体）で描画したページ全体を比較します。下表はコンポーネント部分（div#probe）、設定時の例外、保存された ViewState の要約です。

| ケース | 設定 | 期待される描画 | 設定時の例外 |
|---|---|---|---|
| <code>.(default)</code> | <code>(nothing set)</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.AppRelativeTemplateSourceDirectory=text</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;probe&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=markup</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=japanese</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;日本語テキスト&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=empty</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&quot;;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.ClientIDMode=Inherit</code> | <code>c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.ClientIDMode=AutoID</code> | <code>c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.ClientIDMode=Predictable</code> | <code>c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.ClientIDMode=Static</code> | <code>c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.EnableTheming=true</code> | <code>c.EnableTheming = true;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.EnableTheming=false</code> | <code>c.EnableTheming = false;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.EnableViewState=true</code> | <code>c.EnableViewState = true;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.EnableViewState=false</code> | <code>c.EnableViewState = false;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.ID=text</code> | <code>c.ID = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;probe&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;probe$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;probe$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.ID=markup</code> | <code>c.ID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.ID=japanese</code> | <code>c.ID = &quot;日本語テキスト&quot;;</code> | <code>&lt;span id=&quot;日本語テキスト&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;日本語テキスト$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;日本語テキスト$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.ID=empty</code> | <code>c.ID = &quot;&quot;;</code> | <code>&lt;span id=&quot;ctl02&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;ctl02$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;ctl02$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.PageSize=0</code> | <code>c.PageSize = 0;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> | <code>c.PageSize = 0; -&gt; System.ArgumentOutOfRangeException: Specified argument was out of the range of valid values. [param value]</code> |
| <code>.PageSize=3</code> | <code>c.PageSize = 3;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.PageSize=-1</code> | <code>c.PageSize = -1;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> | <code>c.PageSize = -1; -&gt; System.ArgumentOutOfRangeException: Specified argument was out of the range of valid values. [param value]</code> |
| <code>.PagedControlID=text</code> | <code>c.PagedControlID = &quot;probe&quot;;</code> | <code>status = 500</code> |  |
| <code>.PagedControlID=markup</code> | <code>c.PagedControlID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>status = 500</code> |  |
| <code>.PagedControlID=japanese</code> | <code>c.PagedControlID = &quot;日本語テキスト&quot;;</code> | <code>status = 500</code> |  |
| <code>.PagedControlID=empty</code> | <code>c.PagedControlID = &quot;&quot;;</code> | <code>status = 500</code> |  |
| <code>.PagedControlID=list</code> | <code>c.PagedControlID = &quot;lvPaged&quot;;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.QueryStringField=text</code> | <code>c.QueryStringField = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;/RenderProbe.aspx?component=DataPager&amp;case=DataPager.QueryStringField%3dtext&amp;probe=2&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;/RenderProbe.aspx?component=DataPager&amp;case=DataPager.QueryStringField%3dtext&amp;probe=3&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.QueryStringField=markup</code> | <code>c.QueryStringField = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;/RenderProbe.aspx?component=DataPager&amp;case=DataPager.QueryStringField%3dmarkup&amp;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;=2&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;/RenderProbe.aspx?component=DataPager&amp;case=DataPager.QueryStringField%3dmarkup&amp;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;=3&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.QueryStringField=japanese</code> | <code>c.QueryStringField = &quot;日本語テキスト&quot;;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;/RenderProbe.aspx?component=DataPager&amp;case=DataPager.QueryStringField%3djapanese&amp;日本語テキスト=2&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;/RenderProbe.aspx?component=DataPager&amp;case=DataPager.QueryStringField%3djapanese&amp;日本語テキスト=3&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.QueryStringField=empty</code> | <code>c.QueryStringField = &quot;&quot;;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.SkinID=text</code> | <code>c.SkinID = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.SkinID=markup</code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.SkinID=japanese</code> | <code>c.SkinID = &quot;日本語テキスト&quot;;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.SkinID=empty</code> | <code>c.SkinID = &quot;&quot;;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.ValidateRequestMode=Inherit</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Inherit;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.ValidateRequestMode=Disabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Disabled;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.ValidateRequestMode=Enabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Enabled;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.ViewStateMode=Inherit</code> | <code>c.ViewStateMode = ViewStateMode.Inherit;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.ViewStateMode=Enabled</code> | <code>c.ViewStateMode = ViewStateMode.Enabled;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.ViewStateMode=Disabled</code> | <code>c.ViewStateMode = ViewStateMode.Disabled;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.Visible=true</code> | <code>c.Visible = true;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.Visible=false</code> | <code>c.Visible = false;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.Attributes[data-probe]=text</code> | <code>c.Attributes[data-probe] = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;dpPager&quot; data-probe=&quot;probe&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.Attributes[onfocus]=text</code> | <code>c.Attributes[onfocus] = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;dpPager&quot; onfocus=&quot;probe&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.Attributes[data-probe]=markup</code> | <code>c.Attributes[data-probe] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;dpPager&quot; data-probe=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.Attributes[onfocus]=markup</code> | <code>c.Attributes[onfocus] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span id=&quot;dpPager&quot; onfocus=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.QueryStringField=text+PageSize=3</code> | <code>c.QueryStringField = &quot;probe&quot;; c.PageSize = 3;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;/RenderProbe.aspx?component=DataPager&amp;case=DataPager.QueryStringField%3dtext%2bPageSize%3d3&amp;probe=2&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;/RenderProbe.aspx?component=DataPager&amp;case=DataPager.QueryStringField%3dtext%2bPageSize%3d3&amp;probe=3&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.PageSize=3+Visible=false</code> | <code>c.PageSize = 3; c.Visible = false;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>.PageSize=3+EnableViewState=false</code> | <code>c.PageSize = 3; c.EnableViewState = false;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>[naming container].(default)</code> | <code>[naming container] (nothing set)</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;span id=&quot;box_dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;box$dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;box$dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=AutoID</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;span id=&quot;box_dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;box$dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;box$dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Static</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;box$dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;box$dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Predictable</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;span id=&quot;box_dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;box$dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;box$dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Inherit</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;span id=&quot;box_dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;box$dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;box$dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]↵&lt;/div&gt;</code> |  |
| <code>[naming container].ID=text</code> | <code>[naming container] c.ID = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;span id=&quot;box_probe&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;box$probe$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;box$probe$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]↵&lt;/div&gt;</code> |  |
| <code>[themed].(default)</code> | <code>[themed] (nothing set)</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span class=&quot;skinned&quot; style=&quot;background-color:#EEEEEE;&quot;&gt;1&lt;/span&gt;&#160;&lt;a class=&quot;skinned&quot; href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot; style=&quot;background-color:#EEEEEE;&quot;&gt;2&lt;/a&gt;&#160;&lt;a class=&quot;skinned&quot; href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot; style=&quot;background-color:#EEEEEE;&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>[themed].SkinID=text</code> | <code>[themed] c.SkinID = &quot;probe&quot;;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span class=&quot;skinned&quot; style=&quot;background-color:#EEEEEE;&quot;&gt;1&lt;/span&gt;&#160;&lt;a class=&quot;skinned&quot; href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot; style=&quot;background-color:#EEEEEE;&quot;&gt;2&lt;/a&gt;&#160;&lt;a class=&quot;skinned&quot; href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot; style=&quot;background-color:#EEEEEE;&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>[themed].EnableTheming=false</code> | <code>[themed] c.EnableTheming = false;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;span&gt;1&lt;/span&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl01&#39;,&#39;&#39;)&quot;&gt;2&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;3&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>[next-previous].(default)</code> | <code>[next-previous] (nothing set)</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;a class=&quot;aspNetDisabled&quot;&gt;First&lt;/a&gt;&#160;&lt;a class=&quot;aspNetDisabled&quot;&gt;Previous&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;Next&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl03&#39;,&#39;&#39;)&quot;&gt;Last&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>[next-previous].PageSize=3</code> | <code>[next-previous] c.PageSize = 3;</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;&lt;a class=&quot;aspNetDisabled&quot;&gt;First&lt;/a&gt;&#160;&lt;a class=&quot;aspNetDisabled&quot;&gt;Previous&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl02&#39;,&#39;&#39;)&quot;&gt;Next&lt;/a&gt;&#160;&lt;a href=&quot;javascript:__doPostBack(&#39;dpPager$ctl00$ctl03&#39;,&#39;&#39;)&quot;&gt;Last&lt;/a&gt;&#160;&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |
| <code>[template].(default)</code> | <code>[template] (nothing set)</code> | <code>&lt;span id=&quot;dpPager&quot;&gt;[pager]&lt;/span&gt;[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)]</code> |  |

## ③ シナリオ層

ブラウザ（Playwright）で DataPagerScenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。

| # | 操作 | 対象 | 値 |
|---|---|---|---|
| 1 | goto | /ListViewScenario.aspx | <code></code> |
| 2 | snapshot |  | **DataPager [render][databind][dynamic] first request** |
| 3 | clicktextin | divManual | <code>3</code> |
| 4 | snapshot |  | **DataPager [paging] a numeric pager field: page 3** |
| 5 | clicktextin | divManual | <code>First</code> |
| 6 | snapshot |  | **DataPager [paging] a next-previous field: the first page** |
| 7 | clicktextin | divManual | <code>Last</code> |
| 8 | snapshot |  | **DataPager [paging] a next-previous field: the last page** |
| 9 | clicktextin | divAuto | <code>Last</code> |
| 10 | snapshot |  | **DataPager [paging] the data source&#39;s list view: the last page** |
| 11 | clicktextin | divAuto | <code>Previous</code> |
| 12 | snapshot |  | **DataPager [paging][viewstate] the previous page** |
| 13 | clicktextin | dpDynamic | <code>2</code> |
| 14 | snapshot |  | **DataPager [dynamic][paging] a dynamic pager: page 2** |
| 15 | click | btnSubmit | <code></code> |
| 16 | snapshot |  | **DataPager [viewstate] posted: the pages kept** |

<details><summary>DataPager [render][databind][dynamic] first request（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Sort by name
Pen Select Edit Delete Like
Bag Select Edit Delete Like
Cup <&> Select Edit Delete Like
Insert
First 1 2 3 Last
Automatic (ObjectDataSource)
Sort by price
Pen 1.25 Select Edit Delete
Bag 24.99 Select Edit Delete
Cup <&> 1234.5 Select Edit Delete
First Previous Next Last
Groups
P1 B2 C3
D4 E5 F6
G7 - -
Empty, without view state, hidden, dynamic
Nothing to show No view state: PenNo view state: Bag [dynamic: Pen (P1)][dynamic: Bag (B2)]1 2 3
Request 1: IsPostBack=False
List lvManual: Items=3 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvAuto: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvGroups: Items=7 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvEmpty: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvNoViewState: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvHidden: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending (hidden)
List lvDynamic: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
Pager dpManual: StartRowIndex=0 MaximumRows=3 TotalRowCount=7
Pager dpAuto: StartRowIndex=0 MaximumRows=3 TotalRowCount=0
Pager dpDynamic: StartRowIndex=0 MaximumRows=2 TotalRowCount=5
Manual sort: ""
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>DataPager [paging] a numeric pager field: page 3（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Sort by name
Glue Select Edit Delete Like
Insert
First 1 2 3 Last
Automatic (ObjectDataSource)
Sort by price
Pen 1.25 Select Edit Delete
Bag 24.99 Select Edit Delete
Cup <&> 1234.5 Select Edit Delete
First Previous Next Last
Groups
P1 B2 C3
D4 E5 F6
G7 - -
Empty, without view state, hidden, dynamic
Nothing to show [dynamic: Pen (P1)][dynamic: Bag (B2)]1 2 3
Request 2: IsPostBack=True
PagePropertiesChanging: StartRowIndex=6 MaximumRows=3
List lvManual: Items=1 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvAuto: Items=3 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvGroups: Items=7 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvEmpty: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvNoViewState: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvHidden: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending (hidden)
List lvDynamic: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
Pager dpManual: StartRowIndex=6 MaximumRows=3 TotalRowCount=7
Pager dpAuto: StartRowIndex=0 MaximumRows=3 TotalRowCount=7
Pager dpDynamic: StartRowIndex=0 MaximumRows=2 TotalRowCount=5
Manual sort: ""
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>DataPager [paging] a next-previous field: the first page（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Sort by name
Pen Select Edit Delete Like
Bag Select Edit Delete Like
Cup <&> Select Edit Delete Like
Insert
First 1 2 3 Last
Automatic (ObjectDataSource)
Sort by price
Pen 1.25 Select Edit Delete
Bag 24.99 Select Edit Delete
Cup <&> 1234.5 Select Edit Delete
First Previous Next Last
Groups
P1 B2 C3
D4 E5 F6
G7 - -
Empty, without view state, hidden, dynamic
Nothing to show [dynamic: Pen (P1)][dynamic: Bag (B2)]1 2 3
Request 3: IsPostBack=True
PagePropertiesChanging: StartRowIndex=0 MaximumRows=3
List lvManual: Items=3 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvAuto: Items=3 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvGroups: Items=7 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvEmpty: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvNoViewState: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvHidden: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending (hidden)
List lvDynamic: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
Pager dpManual: StartRowIndex=0 MaximumRows=3 TotalRowCount=7
Pager dpAuto: StartRowIndex=0 MaximumRows=3 TotalRowCount=7
Pager dpDynamic: StartRowIndex=0 MaximumRows=2 TotalRowCount=5
Manual sort: ""
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>DataPager [paging] a next-previous field: the last page（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Sort by name
Glue Select Edit Delete Like
Insert
First 1 2 3 Last
Automatic (ObjectDataSource)
Sort by price
Pen 1.25 Select Edit Delete
Bag 24.99 Select Edit Delete
Cup <&> 1234.5 Select Edit Delete
First Previous Next Last
Groups
P1 B2 C3
D4 E5 F6
G7 - -
Empty, without view state, hidden, dynamic
Nothing to show [dynamic: Pen (P1)][dynamic: Bag (B2)]1 2 3
Request 4: IsPostBack=True
PagePropertiesChanging: StartRowIndex=6 MaximumRows=3
List lvManual: Items=1 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvAuto: Items=3 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvGroups: Items=7 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvEmpty: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvNoViewState: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvHidden: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending (hidden)
List lvDynamic: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
Pager dpManual: StartRowIndex=6 MaximumRows=3 TotalRowCount=7
Pager dpAuto: StartRowIndex=0 MaximumRows=3 TotalRowCount=7
Pager dpDynamic: StartRowIndex=0 MaximumRows=2 TotalRowCount=5
Manual sort: ""
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>DataPager [paging] the data source&#39;s list view: the last page（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Sort by name
Glue Select Edit Delete Like
Insert
First 1 2 3 Last
Automatic (ObjectDataSource)
Sort by price
Glue 2.75 Select Edit Delete
First Previous Next Last
Groups
P1 B2 C3
D4 E5 F6
G7 - -
Empty, without view state, hidden, dynamic
Nothing to show [dynamic: Pen (P1)][dynamic: Bag (B2)]1 2 3
Request 5: IsPostBack=True
List lvManual: Items=1 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvAuto: Items=3 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvGroups: Items=7 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvEmpty: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvNoViewState: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvHidden: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending (hidden)
List lvDynamic: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
Pager dpManual: StartRowIndex=6 MaximumRows=3 TotalRowCount=7
Pager dpAuto: StartRowIndex=6 MaximumRows=3 TotalRowCount=7
Pager dpDynamic: StartRowIndex=0 MaximumRows=2 TotalRowCount=5
Manual sort: ""
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>DataPager [paging][viewstate] the previous page（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Sort by name
Glue Select Edit Delete Like
Insert
First 1 2 3 Last
Automatic (ObjectDataSource)
Sort by price
Desk 150 Select Edit Delete
Eraser 0.5 Select Edit Delete
File 3 Select Edit Delete
First Previous Next Last
Groups
P1 B2 C3
D4 E5 F6
G7 - -
Empty, without view state, hidden, dynamic
Nothing to show [dynamic: Pen (P1)][dynamic: Bag (B2)]1 2 3
Request 6: IsPostBack=True
List lvManual: Items=1 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvAuto: Items=1 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvGroups: Items=7 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvEmpty: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvNoViewState: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvHidden: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending (hidden)
List lvDynamic: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
Pager dpManual: StartRowIndex=6 MaximumRows=3 TotalRowCount=7
Pager dpAuto: StartRowIndex=3 MaximumRows=3 TotalRowCount=7
Pager dpDynamic: StartRowIndex=0 MaximumRows=2 TotalRowCount=5
Manual sort: ""
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>DataPager [dynamic][paging] a dynamic pager: page 2（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Sort by name
Glue Select Edit Delete Like
Insert
First 1 2 3 Last
Automatic (ObjectDataSource)
Sort by price
Desk 150 Select Edit Delete
Eraser 0.5 Select Edit Delete
File 3 Select Edit Delete
First Previous Next Last
Groups
P1 B2 C3
D4 E5 F6
G7 - -
Empty, without view state, hidden, dynamic
Nothing to show [dynamic: Cup <&> (C3)][dynamic: Desk (D4)]1 2 3
Request 7: IsPostBack=True
lvDynamic PagePropertiesChanging: StartRowIndex=2 MaximumRows=2
List lvManual: Items=1 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvAuto: Items=3 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvGroups: Items=7 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvEmpty: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvNoViewState: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvHidden: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending (hidden)
List lvDynamic: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
Pager dpManual: StartRowIndex=6 MaximumRows=3 TotalRowCount=7
Pager dpAuto: StartRowIndex=3 MaximumRows=3 TotalRowCount=7
Pager dpDynamic: StartRowIndex=2 MaximumRows=2 TotalRowCount=5
Manual sort: ""
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>DataPager [viewstate] posted: the pages kept（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Sort by name
Glue Select Edit Delete Like
Insert
First 1 2 3 Last
Automatic (ObjectDataSource)
Sort by price
Desk 150 Select Edit Delete
Eraser 0.5 Select Edit Delete
File 3 Select Edit Delete
First Previous Next Last
Groups
P1 B2 C3
D4 E5 F6
G7 - -
Empty, without view state, hidden, dynamic
Nothing to show [dynamic: Cup <&> (C3)][dynamic: Desk (D4)]1 2 3
Request 8: IsPostBack=True
List lvManual: Items=1 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvAuto: Items=3 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvGroups: Items=7 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvEmpty: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvNoViewState: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvHidden: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending (hidden)
List lvDynamic: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
Pager dpManual: StartRowIndex=6 MaximumRows=3 TotalRowCount=7
Pager dpAuto: StartRowIndex=3 MaximumRows=3 TotalRowCount=7
Pager dpDynamic: StartRowIndex=2 MaximumRows=2 TotalRowCount=5
Manual sort: ""
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

