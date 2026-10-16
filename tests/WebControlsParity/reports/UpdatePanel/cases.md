# UpdatePanel テストケース一覧

変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。
このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。

- 正解データの採取: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0
- ① API 層: 127 ケース（134 メンバー）
- ② HTML 層: 51 ケース
- ③ シナリオ層: 10 スナップショット

## 機構マトリクス

型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。

| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |
|---|---|---|---|
| プロパティ → HTML 描画 | すべてのコントロール | 43 | 1 |
| ViewState の保存と復元 | Control | 6 | 9 |
| Visible=false（描画されず状態は保持） | Control.Visible | 2 | 1 |
| 名前付けコンテナと ClientIDMode | Control.ClientIDMode | 7 | 0 |
| 動的に生成したコントロールのライフサイクル | Control | 0 | 2 |
| データバインド（DataBind / <%# %>） | Control.DataBind | 0 | 1 |
| テンプレート | ITemplate のプロパティ | 0 | 2 |

該当しない機構: テーマとスキン（SkinID / EnableTheming）、Enabled=false・Disabled=true（描画とポスト値の扱い）、ポストバックデータの取り込み、変更イベント（*Changed、ServerChange）、ReadOnly（ポストされた値を無視）、リクエスト検証（ValidateRequestMode）、ポストバックイベント、コマンドイベントと親へのバブル（CommandName）、送信ボタン／スクリプトによるポストバック（UseSubmitBehavior）、ブラウザ側のクリック処理（OnClientClick）、クロスページポストバック（PostBackUrl）、既定のボタン（Panel の DefaultButton）、AutoPostBack（クライアントスクリプトによるポストバック）、CausesValidation / ValidationGroup、検証対象（サーバー側の検証）、検証対象（ブラウザ側の検証）、双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値）、項目（ListItem の Selected・Enabled・Text・Value と ViewState）、複数選択、繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）、ポスト値のイベント検証（一覧にない値・無効な項目の拒否）、サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）、ブラウザ側の検証スクリプト（EnableClientScript）、検証結果のまとめ（ValidationSummary）、ページング、並べ替え、項目の挿入、行・項目の編集（編集・更新・取り消し・削除）、行・項目の選択、行・項目のコマンド（RowCommand・ItemCommand へのバブル）、データキー（DataKeyNames・DataKeys）、データソースコントロールとの連携（DataSourceID）、データソース連携（DataSource・DataTextField など）、クライアントコールバック

## ① API 層

UpdatePanel とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の UpdatePanel インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。

| ケース | 期待値（.NET Framework） |
|---|---|
| <code>Api.Control.#ctor</code> | <code>Control.#ctor = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMode=In…</code> |
| <code>Api.Control.Adapter</code> | <code>Control.Adapter = null</code> |
| <code>Api.Control.AddParsedSubObject</code> | <code>Control.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>Control.AddParsedSubObject(Object) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableVie…</code> |
| <code>Api.Control.AddedControl</code> | <code>Control.AddedControl(Control,Int32) = &quot;ok&quot;</code><br><code>Control.AddedControl(Control,Int32) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableVi…</code> |
| <code>Api.Control.AppRelativeTemplateSourceDirectory</code> | <code>Control.AppRelativeTemplateSourceDirectory = &quot;&quot;</code><br><code>Control.AppRelativeTemplateSourceDirectory set = &quot;-&gt; System.ArgumentException: The relative virtual path &#39;Abc 123&#39; is not allowed here.&quot;</code> |
| <code>Api.Control.ApplyStyleSheetSkin</code> | <code>Control.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>Control.ApplyStyleSheetSkin(Page) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableView…</code> |
| <code>Api.Control.BeginRenderTracing</code> | <code>Control.BeginRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.BeginRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.BeginRenderTracing(TextWriter,Object) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true…</code> |
| <code>Api.Control.BindingContainer</code> | <code>Control.BindingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Clien…</code> |
| <code>Api.Control.BuildProfileTree</code> | <code>Control.BuildProfileTree(String,Boolean) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.ChildControlsCreated</code> | <code>Control.ChildControlsCreated = &quot;false&quot;</code><br><code>Control.ChildControlsCreated set = &quot;ok&quot;</code> |
| <code>Api.Control.ClearCachedClientID</code> | <code>Control.ClearCachedClientID = &quot;ok&quot;</code><br><code>Control.ClearCachedClientID after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=…</code> |
| <code>Api.Control.ClearChildControlState</code> | <code>Control.ClearChildControlState = &quot;ok&quot;</code><br><code>Control.ClearChildControlState after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewSta…</code> |
| <code>Api.Control.ClearChildState</code> | <code>Control.ClearChildState = &quot;ok&quot;</code><br><code>Control.ClearChildState after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true…</code> |
| <code>Api.Control.ClearChildViewState</code> | <code>Control.ClearChildViewState = &quot;ok&quot;</code><br><code>Control.ClearChildViewState after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=…</code> |
| <code>Api.Control.ClearEffectiveClientIDMode</code> | <code>Control.ClearEffectiveClientIDMode = &quot;ok&quot;</code><br><code>Control.ClearEffectiveClientIDMode after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableVie…</code> |
| <code>Api.Control.ClientID</code> | <code>Control.ClientID = &quot;upProbe&quot;</code> |
| <code>Api.Control.ClientIDMode</code> | <code>Control.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>Control.ClientIDMode set = &quot;ClientIDMode.AutoID&quot;</code> |
| <code>Api.Control.ClientIDSeparator</code> | <code>Control.ClientIDSeparator = &quot;&#39;_&#39;&quot;</code> |
| <code>Api.Control.Context</code> | <code>Control.Context = null</code> |
| <code>Api.Control.Controls</code> | <code>Control.Controls = ControlCollection []</code> |
| <code>Api.Control.CreateChildControls</code> | <code>Control.CreateChildControls = &quot;ok&quot;</code><br><code>Control.CreateChildControls after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=…</code> |
| <code>Api.Control.CreateControlCollection</code> | <code>Control.CreateControlCollection = ControlCollection []</code><br><code>Control.CreateControlCollection after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewSt…</code> |
| <code>Api.Control.DataBind</code> | <code>Control.DataBind = &quot;ok&quot;</code><br><code>Control.DataBind after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;u…</code><br><code>Control.DataBind(Boolean) = &quot;ok&quot;</code><br><code>Control.DataBind(Boolean) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=tr…</code> |
| <code>Api.Control.DataBindChildren</code> | <code>Control.DataBindChildren = &quot;ok&quot;</code><br><code>Control.DataBindChildren after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=tru…</code> |
| <code>Api.Control.DataBinding</code> | <code>Control.DataBinding = &quot;added and removed&quot;</code> |
| <code>Api.Control.DataItemContainer</code> | <code>Control.DataItemContainer = null</code> |
| <code>Api.Control.DataKeysContainer</code> | <code>Control.DataKeysContainer = null</code> |
| <code>Api.Control.DesignMode</code> | <code>Control.DesignMode = &quot;false&quot;</code> |
| <code>Api.Control.Dispose</code> | <code>Control.Dispose = &quot;ok&quot;</code><br><code>Control.Dispose after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;up…</code> |
| <code>Api.Control.Disposed</code> | <code>Control.Disposed = &quot;added and removed&quot;</code> |
| <code>Api.Control.EnableTheming</code> | <code>Control.EnableTheming = &quot;true&quot;</code><br><code>Control.EnableTheming set = &quot;true&quot;</code> |
| <code>Api.Control.EnableViewState</code> | <code>Control.EnableViewState = &quot;true&quot;</code><br><code>Control.EnableViewState set = &quot;true&quot;</code> |
| <code>Api.Control.EndRenderTracing</code> | <code>Control.EndRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.EndRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.EndRenderTracing(TextWriter,Object) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; …</code> |
| <code>Api.Control.EnsureChildControls</code> | <code>Control.EnsureChildControls = &quot;ok&quot;</code><br><code>Control.EnsureChildControls after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=…</code> |
| <code>Api.Control.EnsureID</code> | <code>Control.EnsureID = &quot;ok&quot;</code><br><code>Control.EnsureID after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;u…</code> |
| <code>Api.Control.Events</code> | <code>Control.Events = &quot;System.ComponentModel.EventHandlerList&quot;</code> |
| <code>Api.Control.FindControl</code> | <code>Control.FindControl(String) = Label#lblTitle {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code><br><code>Control.FindControl(String) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=…</code><br><code>Control.FindControl(String,Int32) = null</code><br><code>Control.FindControl(String,Int32) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableView…</code> |
| <code>Api.Control.Focus</code> | <code>Control.Focus = &quot;-&gt; System.InvalidOperationException: A form tag with runat=server must exist on the Page to use SetFocus() or the Focus property.&quot;</code> |
| <code>Api.Control.GetDesignModeState</code> | <code>Control.GetDesignModeState = &quot;[]&quot;</code><br><code>Control.GetDesignModeState after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=t…</code> |
| <code>Api.Control.GetRouteUrl</code> | <code>Control.GetRouteUrl(Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.GetUniqueIDRelativeTo</code> | <code>Control.GetUniqueIDRelativeTo(Control) = &quot;upProbe&quot;</code><br><code>Control.GetUniqueIDRelativeTo(Control) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.HasChildViewState</code> | <code>Control.HasChildViewState = &quot;false&quot;</code> |
| <code>Api.Control.HasControls</code> | <code>Control.HasControls = &quot;false&quot;</code><br><code>Control.HasControls after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID…</code> |
| <code>Api.Control.HasEvents</code> | <code>Control.HasEvents = &quot;false&quot;</code><br><code>Control.HasEvents after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;…</code> |
| <code>Api.Control.ID</code> | <code>Control.ID = &quot;upProbe&quot;</code><br><code>Control.ID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.IdSeparator</code> | <code>Control.IdSeparator = &quot;&#39;$&#39;&quot;</code> |
| <code>Api.Control.Init</code> | <code>Control.Init = &quot;added and removed&quot;</code> |
| <code>Api.Control.IsChildControlStateCleared</code> | <code>Control.IsChildControlStateCleared = &quot;false&quot;</code> |
| <code>Api.Control.IsLiteralContent</code> | <code>Control.IsLiteralContent = &quot;false&quot;</code><br><code>Control.IsLiteralContent after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=tru…</code> |
| <code>Api.Control.IsTrackingViewState</code> | <code>Control.IsTrackingViewState = &quot;false&quot;</code> |
| <code>Api.Control.IsViewStateEnabled</code> | <code>Control.IsViewStateEnabled = &quot;true&quot;</code> |
| <code>Api.Control.Load</code> | <code>Control.Load = &quot;added and removed&quot;</code> |
| <code>Api.Control.LoadControlState</code> | <code>Control.LoadControlState(Object) = &quot;ok&quot;</code><br><code>Control.LoadControlState(Object) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewS…</code> |
| <code>Api.Control.LoadViewState</code> | <code>Control.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Collections.ArrayList&#39;.&quot;</code> |
| <code>Api.Control.LoadViewStateByID</code> | <code>Control.LoadViewStateByID = &quot;false&quot;</code> |
| <code>Api.Control.MapPathSecure</code> | <code>Control.MapPathSecure(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.NamingContainer</code> | <code>Control.NamingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code> |
| <code>Api.Control.OnBubbleEvent</code> | <code>Control.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Control.OnBubbleEvent(Object,EventArgs) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; Enab…</code> |
| <code>Api.Control.OnDataBinding</code> | <code>Control.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnDataBinding(EventArgs) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewS…</code> |
| <code>Api.Control.OnInit</code> | <code>Control.OnInit(EventArgs) = &quot;-&gt; System.InvalidOperationException: The control with ID &#39;upProbe&#39; requires a ScriptManager on the page. The ScriptManager must app…</code> |
| <code>Api.Control.OnLoad</code> | <code>Control.OnLoad(EventArgs) = &quot;-&gt; System.InvalidOperationException: The control with ID &#39;upProbe&#39; requires a ScriptManager on the page. The ScriptManager must app…</code> |
| <code>Api.Control.OnPreRender</code> | <code>Control.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnPreRender(EventArgs) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewSta…</code> |
| <code>Api.Control.OnUnload</code> | <code>Control.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnUnload(EventArgs) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=…</code> |
| <code>Api.Control.OpenFile</code> | <code>Control.OpenFile(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.Page</code> | <code>Control.Page = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=&quot;&quot;; …</code><br><code>Control.Page set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=…</code> |
| <code>Api.Control.Parent</code> | <code>Control.Parent = PlaceHolder# {ClientID=&quot;ctl00&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;ctl00&quot;; SkinID=&quot;&quot;; UniqueID=&quot;ctl00&quot;; Valida…</code> |
| <code>Api.Control.PreRender</code> | <code>Control.PreRender = &quot;added and removed&quot;</code> |
| <code>Api.Control.RaiseBubbleEvent</code> | <code>Control.RaiseBubbleEvent(Object,EventArgs) = &quot;ok&quot;</code><br><code>Control.RaiseBubbleEvent(Object,EventArgs) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; E…</code> |
| <code>Api.Control.RemovedControl</code> | <code>Control.RemovedControl(Control) = &quot;ok&quot;</code><br><code>Control.RemovedControl(Control) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewSt…</code> |
| <code>Api.Control.Render</code> | <code>Control.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.Render(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;div id=&quot;upProbe&quot;&gt;</code><br><code>  &#124; </code><br><code>  &#124; &lt;/div&gt;</code><br><code>Control.Render(HtmlTextWriter) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewSta…</code> |
| <code>Api.Control.RenderChildren</code> | <code>Control.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;div id=&quot;upProbe&quot;&gt;</code><br><code>  &#124; </code><br><code>  &#124; &lt;/div&gt;</code><br><code>Control.RenderChildren(HtmlTextWriter) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.RenderControl</code> | <code>Control.RenderControl(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;div id=&quot;upProbe&quot;&gt;</code><br><code>  &#124; </code><br><code>  &#124; &lt;/div&gt;</code><br><code>Control.RenderControl(HtmlTextWriter) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; Enable…</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) writer:</code><br><code>  &#124; &lt;div id=&quot;upProbe&quot;&gt;</code><br><code>  &#124; </code><br><code>  &#124; &lt;/div&gt;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; En…</code> |
| <code>Api.Control.RenderingCompatibility</code> | <code>Control.RenderingCompatibility = &quot;System.Version 4.0&quot;</code><br><code>Control.RenderingCompatibility set = &quot;System.Version 4.0&quot;</code> |
| <code>Api.Control.ResolveAdapter</code> | <code>Control.ResolveAdapter = null</code><br><code>Control.ResolveAdapter after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true;…</code> |
| <code>Api.Control.ResolveClientUrl</code> | <code>Control.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveClientUrl(String) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewS…</code> |
| <code>Api.Control.ResolveUrl</code> | <code>Control.ResolveUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveUrl(String) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=t…</code> |
| <code>Api.Control.SaveControlState</code> | <code>Control.SaveControlState = null</code><br><code>Control.SaveControlState after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=tru…</code> |
| <code>Api.Control.SaveViewState</code> | <code>Control.SaveViewState = null</code><br><code>Control.SaveViewState after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; …</code> |
| <code>Api.Control.SetDesignModeState</code> | <code>Control.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>Control.SetDesignModeState(IDictionary) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; Enab…</code> |
| <code>Api.Control.SetRenderMethodDelegate</code> | <code>Control.SetRenderMethodDelegate(RenderMethod) = &quot;ok&quot;</code><br><code>Control.SetRenderMethodDelegate(RenderMethod) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true…</code> |
| <code>Api.Control.SetTraceData</code> | <code>Control.SetTraceData(Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableVi…</code><br><code>Control.SetTraceData(Object,Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object,Object) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; E…</code> |
| <code>Api.Control.Site</code> | <code>Control.Site = null</code><br><code>Control.Site set = null</code> |
| <code>Api.Control.SkinID</code> | <code>Control.SkinID = &quot;&quot;</code><br><code>Control.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.TemplateControl</code> | <code>Control.TemplateControl = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code><br><code>Control.TemplateControl set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Cl…</code> |
| <code>Api.Control.TemplateSourceDirectory</code> | <code>Control.TemplateSourceDirectory = &quot;&quot;</code> |
| <code>Api.Control.TrackViewState</code> | <code>Control.TrackViewState = &quot;ok&quot;</code><br><code>Control.TrackViewState after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true;…</code> |
| <code>Api.Control.UniqueID</code> | <code>Control.UniqueID = &quot;upProbe&quot;</code> |
| <code>Api.Control.Unload</code> | <code>Control.Unload = &quot;added and removed&quot;</code> |
| <code>Api.Control.ValidateRequestMode</code> | <code>Control.ValidateRequestMode = &quot;ValidateRequestMode.Inherit&quot;</code><br><code>Control.ValidateRequestMode set = &quot;ValidateRequestMode.Disabled&quot;</code> |
| <code>Api.Control.ViewState</code> | <code>Control.ViewState = StateBag []</code> |
| <code>Api.Control.ViewStateIgnoresCase</code> | <code>Control.ViewStateIgnoresCase = &quot;false&quot;</code> |
| <code>Api.Control.ViewStateMode</code> | <code>Control.ViewStateMode = &quot;ViewStateMode.Inherit&quot;</code><br><code>Control.ViewStateMode set = &quot;ViewStateMode.Enabled&quot;</code> |
| <code>Api.Control.Visible</code> | <code>Control.Visible = &quot;true&quot;</code><br><code>Control.Visible set = &quot;true&quot;</code> |
| <code>Api.IAttributeAccessor.GetAttribute</code> | <code>IAttributeAccessor.GetAttribute(String) = null</code><br><code>IAttributeAccessor.GetAttribute(String) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; Enab…</code> |
| <code>Api.IAttributeAccessor.SetAttribute</code> | <code>IAttributeAccessor.SetAttribute(String,String) = &quot;ok&quot;</code><br><code>IAttributeAccessor.SetAttribute(String,String) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=tru…</code> |
| <code>Api.IControlBuilderAccessor.ControlBuilder</code> | <code>IControlBuilderAccessor.ControlBuilder = null</code> |
| <code>Api.IControlDesignerAccessor.GetDesignModeState</code> | <code>IControlDesignerAccessor.GetDesignModeState = &quot;[]&quot;</code><br><code>IControlDesignerAccessor.GetDesignModeState after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; …</code> |
| <code>Api.IControlDesignerAccessor.SetDesignModeState</code> | <code>IControlDesignerAccessor.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetDesignModeState(IDictionary) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableT…</code> |
| <code>Api.IControlDesignerAccessor.SetOwnerControl</code> | <code>IControlDesignerAccessor.SetOwnerControl(Control) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetOwnerControl(Control) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=…</code> |
| <code>Api.IControlDesignerAccessor.UserData</code> | <code>IControlDesignerAccessor.UserData = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.DataBindings</code> | <code>IDataBindingsAccessor.DataBindings = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.HasDataBindings</code> | <code>IDataBindingsAccessor.HasDataBindings = &quot;false&quot;</code> |
| <code>Api.IExpressionsAccessor.Expressions</code> | <code>IExpressionsAccessor.Expressions = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.HasExpressions</code> | <code>IExpressionsAccessor.HasExpressions = &quot;false&quot;</code> |
| <code>Api.IParserAccessor.AddParsedSubObject</code> | <code>IParserAccessor.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>IParserAccessor.AddParsedSubObject(Object) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; E…</code> |
| <code>Api.IUrlResolutionService.ResolveClientUrl</code> | <code>IUrlResolutionService.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>IUrlResolutionService.ResolveClientUrl(String) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=tru…</code> |
| <code>Api.UpdatePanel.#ctor</code> | <code>UpdatePanel.#ctor = UpdatePanel# {ChildrenAsTriggers=true; ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; IsInPartialRend…</code> |
| <code>Api.UpdatePanel.Attributes</code> | <code>UpdatePanel.Attributes = AttributeCollection []</code> |
| <code>Api.UpdatePanel.ChildrenAsTriggers</code> | <code>UpdatePanel.ChildrenAsTriggers = &quot;true&quot;</code><br><code>UpdatePanel.ChildrenAsTriggers set = &quot;true&quot;</code> |
| <code>Api.UpdatePanel.ContentTemplate</code> | <code>UpdatePanel.ContentTemplate = null</code><br><code>UpdatePanel.ContentTemplate set = null</code> |
| <code>Api.UpdatePanel.ContentTemplateContainer</code> | <code>UpdatePanel.ContentTemplateContainer = Control# {ClientID=&quot;ctl01&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;ctl01&quot;; SkinID=&quot;&quot;; Unique…</code> |
| <code>Api.UpdatePanel.Controls</code> | <code>UpdatePanel.Controls = ControlCollection []</code> |
| <code>Api.UpdatePanel.CreateContentTemplateContainer</code> | <code>UpdatePanel.CreateContentTemplateContainer = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID…</code><br><code>UpdatePanel.CreateContentTemplateContainer after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; E…</code> |
| <code>Api.UpdatePanel.CreateControlCollection</code> | <code>UpdatePanel.CreateControlCollection = ControlCollection []</code><br><code>UpdatePanel.CreateControlCollection after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableVi…</code> |
| <code>Api.UpdatePanel.Initialize</code> | <code>UpdatePanel.Initialize = &quot;ok&quot;</code><br><code>UpdatePanel.Initialize after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true;…</code> |
| <code>Api.UpdatePanel.IsInPartialRendering</code> | <code>UpdatePanel.IsInPartialRendering = &quot;false&quot;</code> |
| <code>Api.UpdatePanel.OnInit</code> | <code>UpdatePanel.OnInit(EventArgs) = &quot;-&gt; System.InvalidOperationException: The control with ID &#39;upProbe&#39; requires a ScriptManager on the page. The ScriptManager must…</code> |
| <code>Api.UpdatePanel.OnLoad</code> | <code>UpdatePanel.OnLoad(EventArgs) = &quot;-&gt; System.InvalidOperationException: The control with ID &#39;upProbe&#39; requires a ScriptManager on the page. The ScriptManager must…</code> |
| <code>Api.UpdatePanel.OnPreRender</code> | <code>UpdatePanel.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>UpdatePanel.OnPreRender(EventArgs) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableVie…</code> |
| <code>Api.UpdatePanel.OnUnload</code> | <code>UpdatePanel.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>UpdatePanel.OnUnload(EventArgs) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewSt…</code> |
| <code>Api.UpdatePanel.Render</code> | <code>UpdatePanel.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>UpdatePanel.Render(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;div id=&quot;upProbe&quot;&gt;</code><br><code>  &#124; </code><br><code>  &#124; &lt;/div&gt;</code><br><code>UpdatePanel.Render(HtmlTextWriter) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableVie…</code> |
| <code>Api.UpdatePanel.RenderChildren</code> | <code>UpdatePanel.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>UpdatePanel.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;div id=&quot;upProbe&quot;&gt;</code><br><code>  &#124; </code><br><code>  &#124; &lt;/div&gt;</code><br><code>UpdatePanel.RenderChildren(HtmlTextWriter) after = UpdatePanel#upProbe {ChildrenAsTriggers=true; ClientID=&quot;upProbe&quot;; ClientIDMode=Inherit; EnableTheming=true; E…</code> |
| <code>Api.UpdatePanel.RenderMode</code> | <code>UpdatePanel.RenderMode = &quot;UpdatePanelRenderMode.Block&quot;</code><br><code>UpdatePanel.RenderMode set = &quot;UpdatePanelRenderMode.Inline&quot;</code> |
| <code>Api.UpdatePanel.RequiresUpdate</code> | <code>UpdatePanel.RequiresUpdate = &quot;true&quot;</code> |
| <code>Api.UpdatePanel.Triggers</code> | <code>UpdatePanel.Triggers = &quot;[]&quot;</code> |
| <code>Api.UpdatePanel.Update</code> | <code>UpdatePanel.Update = &quot;-&gt; System.InvalidOperationException: The Update method can only be called on UpdatePanel with ID &#39;upProbe&#39; when UpdateMode is set to Condi…</code> |
| <code>Api.UpdatePanel.UpdateMode</code> | <code>UpdatePanel.UpdateMode = &quot;UpdatePanelUpdateMode.Always&quot;</code><br><code>UpdatePanel.UpdateMode set = &quot;UpdatePanelUpdateMode.Conditional&quot;</code> |

## ② HTML 層

プローブサイトの RenderProbe.aspx で、1 ケース 1 リクエスト（ページライフサイクル全体）で描画したページ全体を比較します。下表はコンポーネント部分（div#probe）、設定時の例外、保存された ViewState の要約です。

| ケース | 設定 | 期待される描画 | 設定時の例外 |
|---|---|---|---|
| <code>.(default)</code> | <code>(nothing set)</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.AppRelativeTemplateSourceDirectory=text</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;probe&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=markup</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=japanese</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;日本語テキスト&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=empty</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ChildrenAsTriggers=true</code> | <code>c.ChildrenAsTriggers = true;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ChildrenAsTriggers=false</code> | <code>c.ChildrenAsTriggers = false;</code> | <code>status = 500</code> |  |
| <code>.ClientIDMode=Inherit</code> | <code>c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ClientIDMode=AutoID</code> | <code>c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ClientIDMode=Predictable</code> | <code>c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ClientIDMode=Static</code> | <code>c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.EnableTheming=true</code> | <code>c.EnableTheming = true;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.EnableTheming=false</code> | <code>c.EnableTheming = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.EnableViewState=true</code> | <code>c.EnableViewState = true;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.EnableViewState=false</code> | <code>c.EnableViewState = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ID=text</code> | <code>c.ID = &quot;probe&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tprobe&#39;,&#39;probe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;probe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ID=markup</code> | <code>c.ID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;t&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39;,&#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ID=japanese</code> | <code>c.ID = &quot;日本語テキスト&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;t日本語テキスト&#39;,&#39;日本語テキスト&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;日本語テキスト&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ID=empty</code> | <code>c.ID = &quot;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tctl03&#39;,&#39;ctl03&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;ctl03&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.RenderMode=Block</code> | <code>c.RenderMode = UpdatePanelRenderMode.Block;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.RenderMode=Inline</code> | <code>c.RenderMode = UpdatePanelRenderMode.Inline;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;span id=&quot;upProbe&quot;&gt;&lt;span&gt;In the panel&lt;/span&gt;&lt;/span&gt;</code> |  |
| <code>.SkinID=text</code> | <code>c.SkinID = &quot;probe&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.SkinID=markup</code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.SkinID=japanese</code> | <code>c.SkinID = &quot;日本語テキスト&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.SkinID=empty</code> | <code>c.SkinID = &quot;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.UpdateMode=Always</code> | <code>c.UpdateMode = UpdatePanelUpdateMode.Always;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.UpdateMode=Conditional</code> | <code>c.UpdateMode = UpdatePanelUpdateMode.Conditional;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ValidateRequestMode=Inherit</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Inherit;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ValidateRequestMode=Disabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Disabled;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ValidateRequestMode=Enabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Enabled;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ViewStateMode=Inherit</code> | <code>c.ViewStateMode = ViewStateMode.Inherit;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ViewStateMode=Enabled</code> | <code>c.ViewStateMode = ViewStateMode.Enabled;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ViewStateMode=Disabled</code> | <code>c.ViewStateMode = ViewStateMode.Disabled;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.Visible=true</code> | <code>c.Visible = true;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.Visible=false</code> | <code>c.Visible = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.Attributes[data-probe]=text</code> | <code>c.Attributes[data-probe] = &quot;probe&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot; data-probe=&quot;probe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.Attributes[onfocus]=text</code> | <code>c.Attributes[onfocus] = &quot;probe&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot; onfocus=&quot;probe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.Attributes[data-probe]=markup</code> | <code>c.Attributes[data-probe] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot; data-probe=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.Attributes[onfocus]=markup</code> | <code>c.Attributes[onfocus] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot; onfocus=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.UpdateMode=Conditional+ChildrenAsTriggers=false</code> | <code>c.UpdateMode = UpdatePanelUpdateMode.Conditional; c.ChildrenAsTriggers = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;fupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.UpdateMode=Always+ChildrenAsTriggers=false</code> | <code>c.UpdateMode = UpdatePanelUpdateMode.Always; c.ChildrenAsTriggers = false;</code> | <code>status = 500</code> |  |
| <code>.RenderMode=Inline+UpdateMode=Conditional</code> | <code>c.RenderMode = UpdatePanelRenderMode.Inline; c.UpdateMode = UpdatePanelUpdateMode.Conditional;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;span id=&quot;upProbe&quot;&gt;&lt;span&gt;In the panel&lt;/span&gt;&lt;/span&gt;</code> |  |
| <code>.RenderMode=Inline+Visible=false</code> | <code>c.RenderMode = UpdatePanelRenderMode.Inline; c.Visible = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.UpdateMode=Conditional+EnableViewState=false</code> | <code>c.UpdateMode = UpdatePanelUpdateMode.Conditional; c.EnableViewState = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵	&lt;span&gt;In the panel&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].(default)</code> | <code>[naming container] (nothing set)</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;box$smPage&#39;, &#39;form1&#39;, [&#39;tbox$upProbe&#39;,&#39;box_upProbe&#39;], [], [], 90, &#39;&#39;);↵	//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;box_upProbe&quot;&gt;↵		&lt;span&gt;In the panel&lt;/span&gt;↵	&lt;/div&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=AutoID</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;box$smPage&#39;, &#39;form1&#39;, [&#39;tbox$upProbe&#39;,&#39;&#39;], [], [], 90, &#39;&#39;);↵	//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;box_upProbe&quot;&gt;↵		&lt;span&gt;In the panel&lt;/span&gt;↵	&lt;/div&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Static</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;box$smPage&#39;, &#39;form1&#39;, [&#39;tbox$upProbe&#39;,&#39;upProbe&#39;], [], [], 90, &#39;&#39;);↵	//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upProbe&quot;&gt;↵		&lt;span&gt;In the panel&lt;/span&gt;↵	&lt;/div&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Predictable</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;box$smPage&#39;, &#39;form1&#39;, [&#39;tbox$upProbe&#39;,&#39;box_upProbe&#39;], [], [], 90, &#39;&#39;);↵	//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;box_upProbe&quot;&gt;↵		&lt;span&gt;In the panel&lt;/span&gt;↵	&lt;/div&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Inherit</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;box$smPage&#39;, &#39;form1&#39;, [&#39;tbox$upProbe&#39;,&#39;box_upProbe&#39;], [], [], 90, &#39;&#39;);↵	//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;box_upProbe&quot;&gt;↵		&lt;span&gt;In the panel&lt;/span&gt;↵	&lt;/div&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ID=text</code> | <code>[naming container] c.ID = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;box$smPage&#39;, &#39;form1&#39;, [&#39;tbox$probe&#39;,&#39;box_probe&#39;], [], [], 90, &#39;&#39;);↵	//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;box_probe&quot;&gt;↵		&lt;span&gt;In the panel&lt;/span&gt;↵	&lt;/div&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].RenderMode=Inline</code> | <code>[naming container] c.RenderMode = UpdatePanelRenderMode.Inline;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;box$smPage&#39;, &#39;form1&#39;, [&#39;tbox$upProbe&#39;,&#39;box_upProbe&#39;], [], [], 90, &#39;&#39;);↵	//]]&gt;↵&lt;/script&gt;↵&lt;span id=&quot;box_upProbe&quot;&gt;&lt;span&gt;In the panel&lt;/span&gt;&lt;/span&gt;↵&lt;/div&gt;</code> |  |

## ③ シナリオ層

ブラウザ（Playwright）で UpdatePanelScenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。

| # | 操作 | 対象 | 値 |
|---|---|---|---|
| 1 | goto | /AjaxScenario.aspx | <code></code> |
| 2 | snapshot |  | **UpdatePanel [render][databind][dynamic][templates] first request: the panels, the proxy&#39;s script run, a panel made in code** |
| 3 | fill | txtInPanel | <code>in the panel</code> |
| 4 | click | btnInPanel | <code></code> |
| 5 | snapshot |  | **UpdatePanel [viewstate][postdata][templates] an asynchronous postback of the main panel: the conditional panel outside left as it was** |
| 6 | click | btnOutside | <code></code> |
| 7 | snapshot |  | **UpdatePanel [viewstate][postdata] a button outside triggers the second panel** |
| 8 | click | btnFull | <code></code> |
| 9 | snapshot |  | **UpdatePanel [viewstate][postdata] a postback trigger in the panel: a full postback** |
| 10 | evaluate | btnSlow | <code>e.click();</code> |
| 11 | snapshot |  | **UpdatePanel [viewstate] while a slow request runs: the progress shown** |
| 12 | waitms |  | <code></code> |
| 13 | snapshot |  | **UpdatePanel [viewstate] after the slow request: the progress hidden** |
| 14 | click | btnStartTimer | <code></code> |
| 15 | waitms |  | <code></code> |
| 16 | snapshot |  | **UpdatePanel [viewstate] the timer started: one tick, then stopped** |
| 17 | click | btnHistory | <code></code> |
| 18 | evaluate | btnInPanel | <code>history.back();</code> |
| 19 | waitms |  | <code></code> |
| 20 | snapshot |  | **UpdatePanel [viewstate] a history point, then the browser&#39;s back: the script manager&#39;s Navigate** |
| 21 | click | btnDynamic | <code></code> |
| 22 | snapshot |  | **UpdatePanel [dynamic][viewstate] an asynchronous postback of the panel made in code** |
| 23 | click | btnShow | <code></code> |
| 24 | snapshot |  | **UpdatePanel [visible][viewstate] a hidden panel shown with what was set while hidden** |

<details><summary>UpdatePanel [render][databind][dynamic][templates] first request: the panels, the proxy&#39;s script run, a panel made in code（期待される本文）</summary>

```text
Script: probe.js ran
Main panel, rendered by request 1
Triggered panel, rendered by request 1
bound
Timer panel, ticks: 0
Dynamic:
dynamic
Request 1: IsPostBack=False IsInAsyncPostBack=False
State upMain UpdateMode=Conditional IsInPartialRendering=False
State upTriggered IsInPartialRendering=False
State txtInPanel Text=""
State lblBound Text="bound"
State lblInHidden Text="set while hidden" (hidden)
State tmrMain Enabled=False Interval=1500
State lblDynamic Text="dynamic"
State smPage EnablePartialRendering=True SupportsPartialRendering=True EnableHistory=True
```

</details>

<details><summary>UpdatePanel [viewstate][postdata][templates] an asynchronous postback of the main panel: the conditional panel outside left as it was（期待される本文）</summary>

```text
Script: probe.js ran
Main panel, rendered by request 2
Triggered panel, rendered by request 1
bound
Timer panel, ticks: 0
Dynamic:
dynamic
Request 2: IsPostBack=True IsInAsyncPostBack=True AsyncPostBackSourceElementID=btnInPanel
Click btnInPanel
State upMain UpdateMode=Conditional IsInPartialRendering=False
State upTriggered IsInPartialRendering=False
State txtInPanel Text="in the panel"
State lblBound Text="bound"
State lblInHidden Text="set while hidden" (hidden)
State tmrMain Enabled=False Interval=1500
State lblDynamic Text="dynamic"
State smPage EnablePartialRendering=True SupportsPartialRendering=True EnableHistory=True
```

</details>

<details><summary>UpdatePanel [viewstate][postdata] a button outside triggers the second panel（期待される本文）</summary>

```text
Script: probe.js ran
Main panel, rendered by request 2
Triggered panel, rendered by request 3
bound
Timer panel, ticks: 0
Dynamic:
dynamic
Request 3: IsPostBack=True IsInAsyncPostBack=True AsyncPostBackSourceElementID=btnOutside
Click btnOutside
State upMain UpdateMode=Conditional IsInPartialRendering=False
State upTriggered IsInPartialRendering=False
State txtInPanel Text="in the panel"
State lblBound Text="bound"
State lblInHidden Text="set while hidden" (hidden)
State tmrMain Enabled=False Interval=1500
State lblDynamic Text="dynamic"
State smPage EnablePartialRendering=True SupportsPartialRendering=True EnableHistory=True
```

</details>

<details><summary>UpdatePanel [viewstate][postdata] a postback trigger in the panel: a full postback（期待される本文）</summary>

```text
Script: probe.js ran
Main panel, rendered by request 4
Triggered panel, rendered by request 4
bound
Timer panel, ticks: 0
Dynamic:
dynamic
Navigate step=(none) IsNavigating=False
Request 4: IsPostBack=True IsInAsyncPostBack=False
Click btnFull
State upMain UpdateMode=Conditional IsInPartialRendering=False
State upTriggered IsInPartialRendering=False
State txtInPanel Text="in the panel"
State lblBound Text="bound"
State lblInHidden Text="set while hidden" (hidden)
State tmrMain Enabled=False Interval=1500
State lblDynamic Text="dynamic"
State smPage EnablePartialRendering=True SupportsPartialRendering=True EnableHistory=True
```

</details>

<details><summary>UpdatePanel [viewstate] while a slow request runs: the progress shown（期待される本文）</summary>

```text
Script: probe.js ran
Main panel, rendered by request 4
Working...
Triggered panel, rendered by request 4
bound
Timer panel, ticks: 0
Dynamic:
dynamic
Navigate step=(none) IsNavigating=False
Request 4: IsPostBack=True IsInAsyncPostBack=False
Click btnFull
State upMain UpdateMode=Conditional IsInPartialRendering=False
State upTriggered IsInPartialRendering=False
State txtInPanel Text="in the panel"
State lblBound Text="bound"
State lblInHidden Text="set while hidden" (hidden)
State tmrMain Enabled=False Interval=1500
State lblDynamic Text="dynamic"
State smPage EnablePartialRendering=True SupportsPartialRendering=True EnableHistory=True
```

</details>

<details><summary>UpdatePanel [viewstate] after the slow request: the progress hidden（期待される本文）</summary>

```text
Script: probe.js ran
Main panel, rendered by request 5
Triggered panel, rendered by request 4
bound
Timer panel, ticks: 0
Dynamic:
dynamic
Request 5: IsPostBack=True IsInAsyncPostBack=True AsyncPostBackSourceElementID=btnSlow
Click btnSlow
State upMain UpdateMode=Conditional IsInPartialRendering=False
State upTriggered IsInPartialRendering=False
State txtInPanel Text="in the panel"
State lblBound Text="bound"
State lblInHidden Text="set while hidden" (hidden)
State tmrMain Enabled=False Interval=1500
State lblDynamic Text="dynamic"
State smPage EnablePartialRendering=True SupportsPartialRendering=True EnableHistory=True
```

</details>

<details><summary>UpdatePanel [viewstate] the timer started: one tick, then stopped（期待される本文）</summary>

```text
Script: probe.js ran
Main panel, rendered by request 5
Triggered panel, rendered by request 4
bound
Timer panel, ticks: 1
Dynamic:
dynamic
Request 7: IsPostBack=True IsInAsyncPostBack=True AsyncPostBackSourceElementID=tmrMain
Tick 1
State upMain UpdateMode=Conditional IsInPartialRendering=False
State upTriggered IsInPartialRendering=False
State txtInPanel Text="in the panel"
State lblBound Text="bound"
State lblInHidden Text="set while hidden" (hidden)
State tmrMain Enabled=False Interval=1500
State lblDynamic Text="dynamic"
State smPage EnablePartialRendering=True SupportsPartialRendering=True EnableHistory=True
```

</details>

<details><summary>UpdatePanel [viewstate] a history point, then the browser&#39;s back: the script manager&#39;s Navigate（期待される本文）</summary>

```text
Script: probe.js ran
Main panel, rendered by request 8
Triggered panel, rendered by request 4
bound
Timer panel, ticks: 1
Dynamic:
dynamic
Request 9: IsPostBack=True IsInAsyncPostBack=True AsyncPostBackSourceElementID=smPage
Navigate step=(none) IsNavigating=True
State upMain UpdateMode=Conditional IsInPartialRendering=False
State upTriggered IsInPartialRendering=False
State txtInPanel Text="in the panel"
State lblBound Text="bound"
State lblInHidden Text="set while hidden" (hidden)
State tmrMain Enabled=False Interval=1500
State lblDynamic Text="dynamic"
State smPage EnablePartialRendering=True SupportsPartialRendering=True EnableHistory=True
```

</details>

<details><summary>UpdatePanel [dynamic][viewstate] an asynchronous postback of the panel made in code（期待される本文）</summary>

```text
Script: probe.js ran
Main panel, rendered by request 8
Triggered panel, rendered by request 4
bound
Timer panel, ticks: 1
Dynamic:
dynamic, posted
Request 10: IsPostBack=True IsInAsyncPostBack=True AsyncPostBackSourceElementID=btnDynamic
Click btnDynamic
State upMain UpdateMode=Conditional IsInPartialRendering=False
State upTriggered IsInPartialRendering=False
State txtInPanel Text="in the panel"
State lblBound Text="bound"
State lblInHidden Text="set while hidden" (hidden)
State tmrMain Enabled=False Interval=1500
State lblDynamic Text="dynamic, posted"
State smPage EnablePartialRendering=True SupportsPartialRendering=True EnableHistory=True
```

</details>

<details><summary>UpdatePanel [visible][viewstate] a hidden panel shown with what was set while hidden（期待される本文）</summary>

```text
Script: probe.js ran
Main panel, rendered by request 11
Triggered panel, rendered by request 11
bound
Hidden panel set while hidden
Timer panel, ticks: 1
Dynamic:
dynamic, posted
Navigate step=(none) IsNavigating=False
Request 11: IsPostBack=True IsInAsyncPostBack=False
Click btnShow
State upMain UpdateMode=Conditional IsInPartialRendering=False
State upTriggered IsInPartialRendering=False
State txtInPanel Text="in the panel"
State lblBound Text="bound"
State lblInHidden Text="set while hidden"
State tmrMain Enabled=False Interval=1500
State lblDynamic Text="dynamic, posted"
State smPage EnablePartialRendering=True SupportsPartialRendering=True EnableHistory=True
```

</details>

