# ScriptManager テストケース一覧

変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。
このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。

- 正解データの採取: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0
- ① API 層: 179 ケース（199 メンバー）
- ② HTML 層: 97 ケース
- ③ シナリオ層: 10 スナップショット

## 機構マトリクス

型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。

| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |
|---|---|---|---|
| プロパティ → HTML 描画 | すべてのコントロール | 89 | 1 |
| ViewState の保存と復元 | Control | 6 | 8 |
| Visible=false（描画されず状態は保持） | Control.Visible | 2 | 1 |
| 名前付けコンテナと ClientIDMode | Control.ClientIDMode | 7 | 0 |
| 動的に生成したコントロールのライフサイクル | Control | 0 | 2 |
| データバインド（DataBind / <%# %>） | Control.DataBind | 0 | 1 |
| ポストバックデータの取り込み | IPostBackDataHandler | 0 | 3 |
| リクエスト検証（ValidateRequestMode） | IPostBackDataHandler（ポスト値を読む） | 3 | 0 |
| ポストバックイベント | IPostBackEventHandler | 0 | 1 |

該当しない機構: テーマとスキン（SkinID / EnableTheming）、Enabled=false・Disabled=true（描画とポスト値の扱い）、変更イベント（*Changed、ServerChange）、ReadOnly（ポストされた値を無視）、コマンドイベントと親へのバブル（CommandName）、送信ボタン／スクリプトによるポストバック（UseSubmitBehavior）、ブラウザ側のクリック処理（OnClientClick）、クロスページポストバック（PostBackUrl）、既定のボタン（Panel の DefaultButton）、AutoPostBack（クライアントスクリプトによるポストバック）、CausesValidation / ValidationGroup、検証対象（サーバー側の検証）、検証対象（ブラウザ側の検証）、双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値）、項目（ListItem の Selected・Enabled・Text・Value と ViewState）、複数選択、繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）、ポスト値のイベント検証（一覧にない値・無効な項目の拒否）、サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）、ブラウザ側の検証スクリプト（EnableClientScript）、検証結果のまとめ（ValidationSummary）、ページング、並べ替え、項目の挿入、行・項目の編集（編集・更新・取り消し・削除）、行・項目の選択、行・項目のコマンド（RowCommand・ItemCommand へのバブル）、データキー（DataKeyNames・DataKeys）、データソースコントロールとの連携（DataSourceID）、テンプレート、データソース連携（DataSource・DataTextField など）、クライアントコールバック

## ① API 層

ScriptManager とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の ScriptManager インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。

| ケース | 期待値（.NET Framework） |
|---|---|
| <code>Api.Control.#ctor</code> | <code>Control.#ctor = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMode=In…</code> |
| <code>Api.Control.Adapter</code> | <code>Control.Adapter = null</code> |
| <code>Api.Control.AddParsedSubObject</code> | <code>Control.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>Control.AddParsedSubObject(Object) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; Async…</code> |
| <code>Api.Control.AddedControl</code> | <code>Control.AddedControl(Control,Int32) = &quot;ok&quot;</code><br><code>Control.AddedControl(Control,Int32) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; Asyn…</code> |
| <code>Api.Control.AppRelativeTemplateSourceDirectory</code> | <code>Control.AppRelativeTemplateSourceDirectory = &quot;&quot;</code><br><code>Control.AppRelativeTemplateSourceDirectory set = &quot;-&gt; System.ArgumentException: The relative virtual path &#39;Abc 123&#39; is not allowed here.&quot;</code> |
| <code>Api.Control.ApplyStyleSheetSkin</code> | <code>Control.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>Control.ApplyStyleSheetSkin(Page) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncP…</code> |
| <code>Api.Control.BeginRenderTracing</code> | <code>Control.BeginRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.BeginRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.BeginRenderTracing(TextWriter,Object) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessag…</code> |
| <code>Api.Control.BindingContainer</code> | <code>Control.BindingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Clien…</code> |
| <code>Api.Control.BuildProfileTree</code> | <code>Control.BuildProfileTree(String,Boolean) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.ChildControlsCreated</code> | <code>Control.ChildControlsCreated = &quot;false&quot;</code><br><code>Control.ChildControlsCreated set = &quot;ok&quot;</code> |
| <code>Api.Control.ClearCachedClientID</code> | <code>Control.ClearCachedClientID = &quot;ok&quot;</code><br><code>Control.ClearCachedClientID after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPostBac…</code> |
| <code>Api.Control.ClearChildControlState</code> | <code>Control.ClearChildControlState = &quot;ok&quot;</code><br><code>Control.ClearChildControlState after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPost…</code> |
| <code>Api.Control.ClearChildState</code> | <code>Control.ClearChildState = &quot;ok&quot;</code><br><code>Control.ClearChildState after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPostBackSou…</code> |
| <code>Api.Control.ClearChildViewState</code> | <code>Control.ClearChildViewState = &quot;ok&quot;</code><br><code>Control.ClearChildViewState after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPostBac…</code> |
| <code>Api.Control.ClearEffectiveClientIDMode</code> | <code>Control.ClearEffectiveClientIDMode = &quot;ok&quot;</code><br><code>Control.ClearEffectiveClientIDMode after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; Async…</code> |
| <code>Api.Control.ClientID</code> | <code>Control.ClientID = &quot;smProbe&quot;</code> |
| <code>Api.Control.ClientIDMode</code> | <code>Control.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>Control.ClientIDMode set = &quot;ClientIDMode.AutoID&quot;</code> |
| <code>Api.Control.ClientIDSeparator</code> | <code>Control.ClientIDSeparator = &quot;&#39;_&#39;&quot;</code> |
| <code>Api.Control.Context</code> | <code>Control.Context = null</code> |
| <code>Api.Control.Controls</code> | <code>Control.Controls = ControlCollection []</code> |
| <code>Api.Control.CreateChildControls</code> | <code>Control.CreateChildControls = &quot;ok&quot;</code><br><code>Control.CreateChildControls after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPostBac…</code> |
| <code>Api.Control.CreateControlCollection</code> | <code>Control.CreateControlCollection = ControlCollection []</code><br><code>Control.CreateControlCollection after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPos…</code> |
| <code>Api.Control.DataBind</code> | <code>Control.DataBind = &quot;ok&quot;</code><br><code>Control.DataBind after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPostBackSourceElem…</code><br><code>Control.DataBind(Boolean) = &quot;ok&quot;</code><br><code>Control.DataBind(Boolean) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPostBackS…</code> |
| <code>Api.Control.DataBindChildren</code> | <code>Control.DataBindChildren = &quot;ok&quot;</code><br><code>Control.DataBindChildren after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPostBackSo…</code> |
| <code>Api.Control.DataBinding</code> | <code>Control.DataBinding = &quot;added and removed&quot;</code> |
| <code>Api.Control.DataItemContainer</code> | <code>Control.DataItemContainer = null</code> |
| <code>Api.Control.DataKeysContainer</code> | <code>Control.DataKeysContainer = null</code> |
| <code>Api.Control.DesignMode</code> | <code>Control.DesignMode = &quot;false&quot;</code> |
| <code>Api.Control.Dispose</code> | <code>Control.Dispose = &quot;ok&quot;</code><br><code>Control.Dispose after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPostBackSourceEleme…</code> |
| <code>Api.Control.Disposed</code> | <code>Control.Disposed = &quot;added and removed&quot;</code> |
| <code>Api.Control.EnableTheming</code> | <code>Control.EnableTheming = &quot;true&quot;</code><br><code>Control.EnableTheming set = &quot;true&quot;</code> |
| <code>Api.Control.EnableViewState</code> | <code>Control.EnableViewState = &quot;true&quot;</code><br><code>Control.EnableViewState set = &quot;true&quot;</code> |
| <code>Api.Control.EndRenderTracing</code> | <code>Control.EndRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.EndRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.EndRenderTracing(TextWriter,Object) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=…</code> |
| <code>Api.Control.EnsureChildControls</code> | <code>Control.EnsureChildControls = &quot;ok&quot;</code><br><code>Control.EnsureChildControls after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPostBac…</code> |
| <code>Api.Control.EnsureID</code> | <code>Control.EnsureID = &quot;ok&quot;</code><br><code>Control.EnsureID after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPostBackSourceElem…</code> |
| <code>Api.Control.Events</code> | <code>Control.Events = &quot;System.ComponentModel.EventHandlerList&quot;</code> |
| <code>Api.Control.FindControl</code> | <code>Control.FindControl(String) = Label#lblTitle {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code><br><code>Control.FindControl(String) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPostBac…</code><br><code>Control.FindControl(String,Int32) = null</code><br><code>Control.FindControl(String,Int32) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncP…</code> |
| <code>Api.Control.Focus</code> | <code>Control.Focus = &quot;-&gt; System.InvalidOperationException: A form tag with runat=server must exist on the Page to use SetFocus() or the Focus property.&quot;</code> |
| <code>Api.Control.GetDesignModeState</code> | <code>Control.GetDesignModeState = &quot;[]&quot;</code><br><code>Control.GetDesignModeState after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPostBack…</code> |
| <code>Api.Control.GetRouteUrl</code> | <code>Control.GetRouteUrl(Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.GetUniqueIDRelativeTo</code> | <code>Control.GetUniqueIDRelativeTo(Control) = &quot;smProbe&quot;</code><br><code>Control.GetUniqueIDRelativeTo(Control) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; A…</code> |
| <code>Api.Control.HasChildViewState</code> | <code>Control.HasChildViewState = &quot;false&quot;</code> |
| <code>Api.Control.HasControls</code> | <code>Control.HasControls = &quot;false&quot;</code><br><code>Control.HasControls after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPostBackSourceE…</code> |
| <code>Api.Control.HasEvents</code> | <code>Control.HasEvents = &quot;false&quot;</code><br><code>Control.HasEvents after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPostBackSourceEle…</code> |
| <code>Api.Control.ID</code> | <code>Control.ID = &quot;smProbe&quot;</code><br><code>Control.ID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.IdSeparator</code> | <code>Control.IdSeparator = &quot;&#39;$&#39;&quot;</code> |
| <code>Api.Control.Init</code> | <code>Control.Init = &quot;added and removed&quot;</code> |
| <code>Api.Control.IsChildControlStateCleared</code> | <code>Control.IsChildControlStateCleared = &quot;false&quot;</code> |
| <code>Api.Control.IsLiteralContent</code> | <code>Control.IsLiteralContent = &quot;false&quot;</code><br><code>Control.IsLiteralContent after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPostBackSo…</code> |
| <code>Api.Control.IsTrackingViewState</code> | <code>Control.IsTrackingViewState = &quot;false&quot;</code> |
| <code>Api.Control.IsViewStateEnabled</code> | <code>Control.IsViewStateEnabled = &quot;true&quot;</code> |
| <code>Api.Control.Load</code> | <code>Control.Load = &quot;added and removed&quot;</code> |
| <code>Api.Control.LoadControlState</code> | <code>Control.LoadControlState(Object) = &quot;ok&quot;</code><br><code>Control.LoadControlState(Object) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPo…</code> |
| <code>Api.Control.LoadViewState</code> | <code>Control.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Collections.ArrayList&#39;.&quot;</code> |
| <code>Api.Control.LoadViewStateByID</code> | <code>Control.LoadViewStateByID = &quot;false&quot;</code> |
| <code>Api.Control.MapPathSecure</code> | <code>Control.MapPathSecure(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.NamingContainer</code> | <code>Control.NamingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code> |
| <code>Api.Control.OnBubbleEvent</code> | <code>Control.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Control.OnBubbleEvent(Object,EventArgs) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; …</code> |
| <code>Api.Control.OnDataBinding</code> | <code>Control.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnDataBinding(EventArgs) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPo…</code> |
| <code>Api.Control.OnInit</code> | <code>Control.OnInit(EventArgs) = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code> |
| <code>Api.Control.OnLoad</code> | <code>Control.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnLoad(EventArgs) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPostBackS…</code> |
| <code>Api.Control.OnPreRender</code> | <code>Control.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnPreRender(EventArgs) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPost…</code> |
| <code>Api.Control.OnUnload</code> | <code>Control.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnUnload(EventArgs) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPostBac…</code> |
| <code>Api.Control.OpenFile</code> | <code>Control.OpenFile(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.Page</code> | <code>Control.Page = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=&quot;&quot;; …</code><br><code>Control.Page set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=…</code> |
| <code>Api.Control.Parent</code> | <code>Control.Parent = HtmlForm#form1 {Action=&quot;&quot;; ClientID=&quot;form1&quot;; ClientIDMode=Inherit; DefaultButton=&quot;&quot;; DefaultFocus=&quot;&quot;; Disabled=false; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.PreRender</code> | <code>Control.PreRender = &quot;added and removed&quot;</code> |
| <code>Api.Control.RaiseBubbleEvent</code> | <code>Control.RaiseBubbleEvent(Object,EventArgs) = &quot;ok&quot;</code><br><code>Control.RaiseBubbleEvent(Object,EventArgs) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;…</code> |
| <code>Api.Control.RemovedControl</code> | <code>Control.RemovedControl(Control) = &quot;ok&quot;</code><br><code>Control.RemovedControl(Control) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPos…</code> |
| <code>Api.Control.Render</code> | <code>Control.Render(HtmlTextWriter) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.RenderChildren</code> | <code>Control.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderChildren(HtmlTextWriter) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; A…</code> |
| <code>Api.Control.RenderControl</code> | <code>Control.RenderControl(HtmlTextWriter) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.RenderingCompatibility</code> | <code>Control.RenderingCompatibility = &quot;System.Version 4.0&quot;</code><br><code>Control.RenderingCompatibility set = &quot;System.Version 4.0&quot;</code> |
| <code>Api.Control.ResolveAdapter</code> | <code>Control.ResolveAdapter = null</code><br><code>Control.ResolveAdapter after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPostBackSour…</code> |
| <code>Api.Control.ResolveClientUrl</code> | <code>Control.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveClientUrl(String) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPo…</code> |
| <code>Api.Control.ResolveUrl</code> | <code>Control.ResolveUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveUrl(String) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPostBack…</code> |
| <code>Api.Control.SaveControlState</code> | <code>Control.SaveControlState = null</code><br><code>Control.SaveControlState after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPostBackSo…</code> |
| <code>Api.Control.SaveViewState</code> | <code>Control.SaveViewState = null</code><br><code>Control.SaveViewState after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPostBackSourc…</code> |
| <code>Api.Control.SetDesignModeState</code> | <code>Control.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>Control.SetDesignModeState(IDictionary) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; …</code> |
| <code>Api.Control.SetRenderMethodDelegate</code> | <code>Control.SetRenderMethodDelegate(RenderMethod) = &quot;ok&quot;</code><br><code>Control.SetRenderMethodDelegate(RenderMethod) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessag…</code> |
| <code>Api.Control.SetTraceData</code> | <code>Control.SetTraceData(Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; Asyn…</code><br><code>Control.SetTraceData(Object,Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object,Object) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;…</code> |
| <code>Api.Control.Site</code> | <code>Control.Site = null</code><br><code>Control.Site set = null</code> |
| <code>Api.Control.SkinID</code> | <code>Control.SkinID = &quot;&quot;</code><br><code>Control.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.TemplateControl</code> | <code>Control.TemplateControl = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code><br><code>Control.TemplateControl set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Cl…</code> |
| <code>Api.Control.TemplateSourceDirectory</code> | <code>Control.TemplateSourceDirectory = &quot;&quot;</code> |
| <code>Api.Control.TrackViewState</code> | <code>Control.TrackViewState = &quot;ok&quot;</code><br><code>Control.TrackViewState after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPostBackSour…</code> |
| <code>Api.Control.UniqueID</code> | <code>Control.UniqueID = &quot;smProbe&quot;</code> |
| <code>Api.Control.Unload</code> | <code>Control.Unload = &quot;added and removed&quot;</code> |
| <code>Api.Control.ValidateRequestMode</code> | <code>Control.ValidateRequestMode = &quot;ValidateRequestMode.Inherit&quot;</code><br><code>Control.ValidateRequestMode set = &quot;ValidateRequestMode.Disabled&quot;</code> |
| <code>Api.Control.ViewState</code> | <code>Control.ViewState = StateBag []</code> |
| <code>Api.Control.ViewStateIgnoresCase</code> | <code>Control.ViewStateIgnoresCase = &quot;false&quot;</code> |
| <code>Api.Control.ViewStateMode</code> | <code>Control.ViewStateMode = &quot;ViewStateMode.Inherit&quot;</code><br><code>Control.ViewStateMode set = &quot;ViewStateMode.Enabled&quot;</code> |
| <code>Api.Control.Visible</code> | <code>Control.Visible = &quot;true&quot;</code><br><code>Control.Visible set = &quot;-&gt; System.NotImplementedException: The method or operation is not implemented.&quot;</code> |
| <code>Api.IControlBuilderAccessor.ControlBuilder</code> | <code>IControlBuilderAccessor.ControlBuilder = null</code> |
| <code>Api.IControlDesignerAccessor.GetDesignModeState</code> | <code>IControlDesignerAccessor.GetDesignModeState = &quot;[]&quot;</code><br><code>IControlDesignerAccessor.GetDesignModeState after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=…</code> |
| <code>Api.IControlDesignerAccessor.SetDesignModeState</code> | <code>IControlDesignerAccessor.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetDesignModeState(IDictionary) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBack…</code> |
| <code>Api.IControlDesignerAccessor.SetOwnerControl</code> | <code>IControlDesignerAccessor.SetOwnerControl(Control) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetOwnerControl(Control) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMe…</code> |
| <code>Api.IControlDesignerAccessor.UserData</code> | <code>IControlDesignerAccessor.UserData = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.DataBindings</code> | <code>IDataBindingsAccessor.DataBindings = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.HasDataBindings</code> | <code>IDataBindingsAccessor.HasDataBindings = &quot;false&quot;</code> |
| <code>Api.IExpressionsAccessor.Expressions</code> | <code>IExpressionsAccessor.Expressions = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.HasExpressions</code> | <code>IExpressionsAccessor.HasExpressions = &quot;false&quot;</code> |
| <code>Api.IParserAccessor.AddParsedSubObject</code> | <code>IParserAccessor.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>IParserAccessor.AddParsedSubObject(Object) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;…</code> |
| <code>Api.IPostBackDataHandler.LoadPostData</code> | <code>IPostBackDataHandler.LoadPostData(String,NameValueCollection) = &quot;false&quot;</code><br><code>IPostBackDataHandler.LoadPostData(String,NameValueCollection) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPos…</code> |
| <code>Api.IPostBackDataHandler.RaisePostDataChangedEvent</code> | <code>IPostBackDataHandler.RaisePostDataChangedEvent = &quot;ok&quot;</code><br><code>IPostBackDataHandler.RaisePostDataChangedEvent after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessa…</code> |
| <code>Api.IPostBackEventHandler.RaisePostBackEvent</code> | <code>IPostBackEventHandler.RaisePostBackEvent(String) = &quot;-&gt; System.FormatException: (the runtime&#39;s parse error)&quot;</code> |
| <code>Api.IUrlResolutionService.ResolveClientUrl</code> | <code>IUrlResolutionService.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>IUrlResolutionService.ResolveClientUrl(String) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessa…</code> |
| <code>Api.ScriptManager.#ctor</code> | <code>ScriptManager.#ctor = ScriptManager# {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPostBackSourceElementID=&quot;&quot;; …</code> |
| <code>Api.ScriptManager.AddHistoryPoint</code> | <code>ScriptManager.AddHistoryPoint(NameValueCollection,String) = &quot;-&gt; System.InvalidOperationException: A history point can only be added if EnableHistory is set to t…</code><br><code>ScriptManager.AddHistoryPoint(String,String) = &quot;-&gt; System.InvalidOperationException: A history point can only be added if EnableHistory is set to true.&quot;</code><br><code>ScriptManager.AddHistoryPoint(String,String,String) = &quot;-&gt; System.InvalidOperationException: A history point can only be added if EnableHistory is set to true.&quot;</code> |
| <code>Api.ScriptManager.AjaxFrameworkAssembly</code> | <code>ScriptManager.AjaxFrameworkAssembly = &quot;System.Reflection.RuntimeAssembly System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e3…</code> |
| <code>Api.ScriptManager.AjaxFrameworkMode</code> | <code>ScriptManager.AjaxFrameworkMode = &quot;AjaxFrameworkMode.Enabled&quot;</code><br><code>ScriptManager.AjaxFrameworkMode set = &quot;AjaxFrameworkMode.Disabled&quot;</code> |
| <code>Api.ScriptManager.AllowCustomErrorsRedirect</code> | <code>ScriptManager.AllowCustomErrorsRedirect = &quot;true&quot;</code><br><code>ScriptManager.AllowCustomErrorsRedirect set = &quot;true&quot;</code> |
| <code>Api.ScriptManager.AsyncPostBackError</code> | <code>ScriptManager.AsyncPostBackError = &quot;added and removed&quot;</code> |
| <code>Api.ScriptManager.AsyncPostBackErrorMessage</code> | <code>ScriptManager.AsyncPostBackErrorMessage = &quot;&quot;</code><br><code>ScriptManager.AsyncPostBackErrorMessage set = &quot;Abc 123&quot;</code> |
| <code>Api.ScriptManager.AsyncPostBackSourceElementID</code> | <code>ScriptManager.AsyncPostBackSourceElementID = &quot;&quot;</code> |
| <code>Api.ScriptManager.AsyncPostBackTimeout</code> | <code>ScriptManager.AsyncPostBackTimeout = &quot;Int32 90&quot;</code><br><code>ScriptManager.AsyncPostBackTimeout set = &quot;Int32 1&quot;</code> |
| <code>Api.ScriptManager.AuthenticationService</code> | <code>ScriptManager.AuthenticationService = AuthenticationServiceManager {Path=&quot;&quot;}</code> |
| <code>Api.ScriptManager.ClientNavigateHandler</code> | <code>ScriptManager.ClientNavigateHandler = &quot;&quot;</code><br><code>ScriptManager.ClientNavigateHandler set = &quot;Abc 123&quot;</code> |
| <code>Api.ScriptManager.CompositeScript</code> | <code>ScriptManager.CompositeScript = CompositeScriptReference {NotifyScriptLoaded=true; Path=&quot;&quot;; ScriptMode=Auto}</code> |
| <code>Api.ScriptManager.EmptyPageUrl</code> | <code>ScriptManager.EmptyPageUrl = &quot;&quot;</code><br><code>ScriptManager.EmptyPageUrl set = &quot;Abc 123&quot;</code> |
| <code>Api.ScriptManager.EnableCdn</code> | <code>ScriptManager.EnableCdn = &quot;false&quot;</code><br><code>ScriptManager.EnableCdn set = &quot;true&quot;</code> |
| <code>Api.ScriptManager.EnableCdnFallback</code> | <code>ScriptManager.EnableCdnFallback = &quot;true&quot;</code><br><code>ScriptManager.EnableCdnFallback set = &quot;true&quot;</code> |
| <code>Api.ScriptManager.EnableHistory</code> | <code>ScriptManager.EnableHistory = &quot;false&quot;</code><br><code>ScriptManager.EnableHistory set = &quot;true&quot;</code> |
| <code>Api.ScriptManager.EnablePageMethods</code> | <code>ScriptManager.EnablePageMethods = &quot;false&quot;</code><br><code>ScriptManager.EnablePageMethods set = &quot;true&quot;</code> |
| <code>Api.ScriptManager.EnablePartialRendering</code> | <code>ScriptManager.EnablePartialRendering = &quot;true&quot;</code><br><code>ScriptManager.EnablePartialRendering set = &quot;true&quot;</code> |
| <code>Api.ScriptManager.EnableScriptGlobalization</code> | <code>ScriptManager.EnableScriptGlobalization = &quot;false&quot;</code><br><code>ScriptManager.EnableScriptGlobalization set = &quot;true&quot;</code> |
| <code>Api.ScriptManager.EnableScriptLocalization</code> | <code>ScriptManager.EnableScriptLocalization = &quot;true&quot;</code><br><code>ScriptManager.EnableScriptLocalization set = &quot;true&quot;</code> |
| <code>Api.ScriptManager.EnableSecureHistoryState</code> | <code>ScriptManager.EnableSecureHistoryState = &quot;true&quot;</code><br><code>ScriptManager.EnableSecureHistoryState set = &quot;true&quot;</code> |
| <code>Api.ScriptManager.GetCurrent</code> | <code>ScriptManager.GetCurrent(Page) = null</code> |
| <code>Api.ScriptManager.GetRegisteredArrayDeclarations</code> | <code>ScriptManager.GetRegisteredArrayDeclarations = &quot;[]&quot;</code><br><code>ScriptManager.GetRegisteredArrayDeclarations after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage…</code> |
| <code>Api.ScriptManager.GetRegisteredClientScriptBlocks</code> | <code>ScriptManager.GetRegisteredClientScriptBlocks = &quot;[]&quot;</code><br><code>ScriptManager.GetRegisteredClientScriptBlocks after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessag…</code> |
| <code>Api.ScriptManager.GetRegisteredDisposeScripts</code> | <code>ScriptManager.GetRegisteredDisposeScripts = &quot;[]&quot;</code><br><code>ScriptManager.GetRegisteredDisposeScripts after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;…</code> |
| <code>Api.ScriptManager.GetRegisteredExpandoAttributes</code> | <code>ScriptManager.GetRegisteredExpandoAttributes = &quot;[]&quot;</code><br><code>ScriptManager.GetRegisteredExpandoAttributes after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage…</code> |
| <code>Api.ScriptManager.GetRegisteredHiddenFields</code> | <code>ScriptManager.GetRegisteredHiddenFields = &quot;[]&quot;</code><br><code>ScriptManager.GetRegisteredHiddenFields after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; …</code> |
| <code>Api.ScriptManager.GetRegisteredOnSubmitStatements</code> | <code>ScriptManager.GetRegisteredOnSubmitStatements = &quot;[]&quot;</code><br><code>ScriptManager.GetRegisteredOnSubmitStatements after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessag…</code> |
| <code>Api.ScriptManager.GetRegisteredStartupScripts</code> | <code>ScriptManager.GetRegisteredStartupScripts = &quot;[]&quot;</code><br><code>ScriptManager.GetRegisteredStartupScripts after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;…</code> |
| <code>Api.ScriptManager.GetStateString</code> | <code>ScriptManager.GetStateString = &quot;{protected}&quot;</code><br><code>ScriptManager.GetStateString after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; AsyncPostBa…</code> |
| <code>Api.ScriptManager.IsDebuggingEnabled</code> | <code>ScriptManager.IsDebuggingEnabled = &quot;false&quot;</code> |
| <code>Api.ScriptManager.IsInAsyncPostBack</code> | <code>ScriptManager.IsInAsyncPostBack = &quot;false&quot;</code> |
| <code>Api.ScriptManager.IsNavigating</code> | <code>ScriptManager.IsNavigating = &quot;false&quot;</code> |
| <code>Api.ScriptManager.LoadPostData</code> | <code>ScriptManager.LoadPostData(String,NameValueCollection) = &quot;false&quot;</code><br><code>ScriptManager.LoadPostData(String,NameValueCollection) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackEr…</code> |
| <code>Api.ScriptManager.LoadScriptsBeforeUI</code> | <code>ScriptManager.LoadScriptsBeforeUI = &quot;true&quot;</code><br><code>ScriptManager.LoadScriptsBeforeUI set = &quot;true&quot;</code> |
| <code>Api.ScriptManager.Navigate</code> | <code>ScriptManager.Navigate = &quot;added and removed&quot;</code> |
| <code>Api.ScriptManager.OnAsyncPostBackError</code> | <code>ScriptManager.OnAsyncPostBackError(AsyncPostBackErrorEventArgs) = &quot;ok&quot;</code><br><code>ScriptManager.OnAsyncPostBackError(AsyncPostBackErrorEventArgs) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncP…</code> |
| <code>Api.ScriptManager.OnInit</code> | <code>ScriptManager.OnInit(EventArgs) = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code> |
| <code>Api.ScriptManager.OnPreRender</code> | <code>ScriptManager.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>ScriptManager.OnPreRender(EventArgs) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; Asy…</code> |
| <code>Api.ScriptManager.OnResolveCompositeScriptReference</code> | <code>ScriptManager.OnResolveCompositeScriptReference(CompositeScriptReferenceEventArgs) = &quot;ok&quot;</code><br><code>ScriptManager.OnResolveCompositeScriptReference(CompositeScriptReferenceEventArgs) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRe…</code> |
| <code>Api.ScriptManager.OnResolveScriptReference</code> | <code>ScriptManager.OnResolveScriptReference(ScriptReferenceEventArgs) = &quot;ok&quot;</code><br><code>ScriptManager.OnResolveScriptReference(ScriptReferenceEventArgs) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; Async…</code> |
| <code>Api.ScriptManager.ProfileService</code> | <code>ScriptManager.ProfileService = ProfileServiceManager {Path=&quot;&quot;}</code> |
| <code>Api.ScriptManager.RaisePostBackEvent</code> | <code>ScriptManager.RaisePostBackEvent(String) = &quot;-&gt; System.FormatException: (the runtime&#39;s parse error)&quot;</code> |
| <code>Api.ScriptManager.RaisePostDataChangedEvent</code> | <code>ScriptManager.RaisePostDataChangedEvent = &quot;ok&quot;</code><br><code>ScriptManager.RaisePostDataChangedEvent after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessage=&quot;&quot;; …</code> |
| <code>Api.ScriptManager.RegisterArrayDeclaration</code> | <code>ScriptManager.RegisterArrayDeclaration(Control,String,String) = &quot;ok&quot;</code><br><code>ScriptManager.RegisterArrayDeclaration(Page,String,String) = &quot;ok&quot;</code> |
| <code>Api.ScriptManager.RegisterAsyncPostBackControl</code> | <code>ScriptManager.RegisterAsyncPostBackControl(Control) = &quot;-&gt; System.ArgumentException: Control with ID &#39;lblTitle&#39; being registered through RegisterAsyncPostBackCon…</code> |
| <code>Api.ScriptManager.RegisterClientScriptBlock</code> | <code>ScriptManager.RegisterClientScriptBlock(Control,Type,String,String,Boolean) = &quot;ok&quot;</code><br><code>ScriptManager.RegisterClientScriptBlock(Page,Type,String,String,Boolean) = &quot;ok&quot;</code> |
| <code>Api.ScriptManager.RegisterClientScriptInclude</code> | <code>ScriptManager.RegisterClientScriptInclude(Control,Type,String,String) = &quot;ok&quot;</code><br><code>ScriptManager.RegisterClientScriptInclude(Page,Type,String,String) = &quot;ok&quot;</code> |
| <code>Api.ScriptManager.RegisterClientScriptResource</code> | <code>ScriptManager.RegisterClientScriptResource(Control,Type,String) = &quot;-&gt; System.ArgumentNullException: Value cannot be null. [param basepath]&quot;</code><br><code>ScriptManager.RegisterClientScriptResource(Page,Type,String) = &quot;-&gt; System.ArgumentNullException: Value cannot be null. [param basepath]&quot;</code> |
| <code>Api.ScriptManager.RegisterDataItem</code> | <code>ScriptManager.RegisterDataItem(Control,String) = &quot;-&gt; System.InvalidOperationException: RegisterDataItem can only be called during an async postback.&quot;</code><br><code>ScriptManager.RegisterDataItem(Control,String,Boolean) = &quot;-&gt; System.InvalidOperationException: RegisterDataItem can only be called during an async postback.&quot;</code> |
| <code>Api.ScriptManager.RegisterDispose</code> | <code>ScriptManager.RegisterDispose(Control,String) = &quot;ok&quot;</code><br><code>ScriptManager.RegisterDispose(Control,String) after = ScriptManager#smProbe {AjaxFrameworkMode=Enabled; AllowCustomErrorsRedirect=true; AsyncPostBackErrorMessag…</code> |
| <code>Api.ScriptManager.RegisterExpandoAttribute</code> | <code>ScriptManager.RegisterExpandoAttribute(Control,String,String,String,Boolean) = &quot;ok&quot;</code> |
| <code>Api.ScriptManager.RegisterExtenderControl``1</code> | <code>ScriptManager.RegisterExtenderControl``1(``0,Control) = &quot;-&gt; System.InvalidOperationException: Late bound operations cannot be performed on types or methods for …</code> |
| <code>Api.ScriptManager.RegisterHiddenField</code> | <code>ScriptManager.RegisterHiddenField(Control,String,String) = &quot;ok&quot;</code><br><code>ScriptManager.RegisterHiddenField(Page,String,String) = &quot;ok&quot;</code> |
| <code>Api.ScriptManager.RegisterNamedClientScriptResource</code> | <code>ScriptManager.RegisterNamedClientScriptResource(Control,String) = &quot;-&gt; System.ArgumentNullException: Value cannot be null. [param basepath]&quot;</code><br><code>ScriptManager.RegisterNamedClientScriptResource(Page,String) = &quot;-&gt; System.ArgumentNullException: Value cannot be null. [param basepath]&quot;</code> |
| <code>Api.ScriptManager.RegisterOnSubmitStatement</code> | <code>ScriptManager.RegisterOnSubmitStatement(Control,Type,String,String) = &quot;ok&quot;</code><br><code>ScriptManager.RegisterOnSubmitStatement(Page,Type,String,String) = &quot;ok&quot;</code> |
| <code>Api.ScriptManager.RegisterPostBackControl</code> | <code>ScriptManager.RegisterPostBackControl(Control) = &quot;-&gt; System.ArgumentException: Control with ID &#39;lblTitle&#39; being registered through RegisterAsyncPostBackControl …</code> |
| <code>Api.ScriptManager.RegisterScriptControl``1</code> | <code>ScriptManager.RegisterScriptControl``1(``0) = &quot;-&gt; System.InvalidOperationException: Late bound operations cannot be performed on types or methods for which Cont…</code> |
| <code>Api.ScriptManager.RegisterScriptDescriptors</code> | <code>ScriptManager.RegisterScriptDescriptors(IExtenderControl) = &quot;-&gt; System.ArgumentNullException: Value cannot be null. [param extenderControl]&quot;</code><br><code>ScriptManager.RegisterScriptDescriptors(IScriptControl) = &quot;-&gt; System.ArgumentNullException: Value cannot be null. [param scriptControl]&quot;</code> |
| <code>Api.ScriptManager.RegisterStartupScript</code> | <code>ScriptManager.RegisterStartupScript(Control,Type,String,String,Boolean) = &quot;ok&quot;</code><br><code>ScriptManager.RegisterStartupScript(Page,Type,String,String,Boolean) = &quot;ok&quot;</code> |
| <code>Api.ScriptManager.Render</code> | <code>ScriptManager.Render(HtmlTextWriter) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.ScriptManager.ResolveCompositeScriptReference</code> | <code>ScriptManager.ResolveCompositeScriptReference = &quot;added and removed&quot;</code> |
| <code>Api.ScriptManager.ResolveScriptReference</code> | <code>ScriptManager.ResolveScriptReference = &quot;added and removed&quot;</code> |
| <code>Api.ScriptManager.RoleService</code> | <code>ScriptManager.RoleService = RoleServiceManager {LoadRoles=false; Path=&quot;&quot;}</code> |
| <code>Api.ScriptManager.ScriptMode</code> | <code>ScriptManager.ScriptMode = &quot;ScriptMode.Auto&quot;</code><br><code>ScriptManager.ScriptMode set = &quot;ScriptMode.Inherit&quot;</code> |
| <code>Api.ScriptManager.ScriptPath</code> | <code>ScriptManager.ScriptPath = &quot;&quot;</code><br><code>ScriptManager.ScriptPath set = &quot;Abc 123&quot;</code> |
| <code>Api.ScriptManager.ScriptResourceMapping</code> | <code>ScriptManager.ScriptResourceMapping = ScriptResourceMapping {}</code> |
| <code>Api.ScriptManager.Scripts</code> | <code>ScriptManager.Scripts = &quot;[]&quot;</code> |
| <code>Api.ScriptManager.Services</code> | <code>ScriptManager.Services = &quot;[]&quot;</code> |
| <code>Api.ScriptManager.SetFocus</code> | <code>ScriptManager.SetFocus(String) = &quot;-&gt; System.InvalidOperationException: A form tag with runat=server must exist on the Page to use SetFocus() or the Focus proper…</code><br><code>ScriptManager.SetFocus(Control) = &quot;-&gt; System.InvalidOperationException: A form tag with runat=server must exist on the Page to use SetFocus() or the Focus prope…</code> |
| <code>Api.ScriptManager.SupportsPartialRendering</code> | <code>ScriptManager.SupportsPartialRendering = &quot;true&quot;</code><br><code>ScriptManager.SupportsPartialRendering set = &quot;true&quot;</code> |
| <code>Api.ScriptManager.Visible</code> | <code>ScriptManager.Visible = &quot;true&quot;</code><br><code>ScriptManager.Visible set = &quot;-&gt; System.NotImplementedException: The method or operation is not implemented.&quot;</code> |

## ② HTML 層

プローブサイトの RenderProbe.aspx で、1 ケース 1 リクエスト（ページライフサイクル全体）で描画したページ全体を比較します。下表はコンポーネント部分（div#probe）、設定時の例外、保存された ViewState の要約です。

| ケース | 設定 | 期待される描画 | 設定時の例外 |
|---|---|---|---|
| <code>.(default)</code> | <code>(nothing set)</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.AjaxFrameworkMode=Enabled</code> | <code>c.AjaxFrameworkMode = AjaxFrameworkMode.Enabled;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.AjaxFrameworkMode=Disabled</code> | <code>c.AjaxFrameworkMode = AjaxFrameworkMode.Disabled;</code> | <code></code> |  |
| <code>.AjaxFrameworkMode=Explicit</code> | <code>c.AjaxFrameworkMode = AjaxFrameworkMode.Explicit;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.AllowCustomErrorsRedirect=true</code> | <code>c.AllowCustomErrorsRedirect = true;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.AllowCustomErrorsRedirect=false</code> | <code>c.AllowCustomErrorsRedirect = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.AppRelativeTemplateSourceDirectory=text</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;probe&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=markup</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=japanese</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;日本語テキスト&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=empty</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.AsyncPostBackErrorMessage=text</code> | <code>c.AsyncPostBackErrorMessage = &quot;probe&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.AsyncPostBackErrorMessage=markup</code> | <code>c.AsyncPostBackErrorMessage = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.AsyncPostBackErrorMessage=japanese</code> | <code>c.AsyncPostBackErrorMessage = &quot;日本語テキスト&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.AsyncPostBackErrorMessage=empty</code> | <code>c.AsyncPostBackErrorMessage = &quot;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.AsyncPostBackTimeout=0</code> | <code>c.AsyncPostBackTimeout = 0;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 0, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.AsyncPostBackTimeout=3</code> | <code>c.AsyncPostBackTimeout = 3;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 3, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.AsyncPostBackTimeout=-1</code> | <code>c.AsyncPostBackTimeout = -1;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> | <code>c.AsyncPostBackTimeout = -1; -&gt; System.ArgumentOutOfRangeException: Specified argument was out of the range of valid values. [param value]</code> |
| <code>.ClientIDMode=Inherit</code> | <code>c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ClientIDMode=AutoID</code> | <code>c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ClientIDMode=Predictable</code> | <code>c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ClientIDMode=Static</code> | <code>c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ClientNavigateHandler=text</code> | <code>c.ClientNavigateHandler = &quot;probe&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ClientNavigateHandler=markup</code> | <code>c.ClientNavigateHandler = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ClientNavigateHandler=japanese</code> | <code>c.ClientNavigateHandler = &quot;日本語テキスト&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ClientNavigateHandler=empty</code> | <code>c.ClientNavigateHandler = &quot;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ClientNavigateHandler=handler</code> | <code>c.ClientNavigateHandler = &quot;onNavigate&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.EmptyPageUrl=text</code> | <code>c.EmptyPageUrl = &quot;probe&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.EmptyPageUrl=markup</code> | <code>c.EmptyPageUrl = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.EmptyPageUrl=japanese</code> | <code>c.EmptyPageUrl = &quot;日本語テキスト&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.EmptyPageUrl=empty</code> | <code>c.EmptyPageUrl = &quot;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.EmptyPageUrl=app-relative</code> | <code>c.EmptyPageUrl = &quot;~/ButtonTarget.aspx&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.EnableCdn=true</code> | <code>c.EnableCdn = true;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.EnableCdn=false</code> | <code>c.EnableCdn = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.EnableCdnFallback=true</code> | <code>c.EnableCdnFallback = true;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.EnableCdnFallback=false</code> | <code>c.EnableCdnFallback = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.EnableHistory=true</code> | <code>c.EnableHistory = true;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [&#39;smProbe&#39;,&#39;smProbe&#39;], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;input type=&quot;hidden&quot; name=&quot;smProbe&quot; id=&quot;smProbe&quot; /&gt;↵&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.Application.setServerId(&quot;smProbe&quot;, &quot;smProbe&quot;);↵Sys.Application._enableHistoryInScriptManager();↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.EnableHistory=false</code> | <code>c.EnableHistory = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.EnablePageMethods=true</code> | <code>c.EnablePageMethods = true;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.EnablePageMethods=false</code> | <code>c.EnablePageMethods = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.EnablePartialRendering=true</code> | <code>c.EnablePartialRendering = true;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.EnablePartialRendering=false</code> | <code>c.EnablePartialRendering = false;</code> | <code></code> |  |
| <code>.EnableScriptGlobalization=true</code> | <code>c.EnableScriptGlobalization = true;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.EnableScriptGlobalization=false</code> | <code>c.EnableScriptGlobalization = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.EnableScriptLocalization=true</code> | <code>c.EnableScriptLocalization = true;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.EnableScriptLocalization=false</code> | <code>c.EnableScriptLocalization = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.EnableSecureHistoryState=true</code> | <code>c.EnableSecureHistoryState = true;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.EnableSecureHistoryState=false</code> | <code>c.EnableSecureHistoryState = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.EnableTheming=true</code> | <code>c.EnableTheming = true;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.EnableTheming=false</code> | <code>c.EnableTheming = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.EnableViewState=true</code> | <code>c.EnableViewState = true;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.EnableViewState=false</code> | <code>c.EnableViewState = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ID=text</code> | <code>c.ID = &quot;probe&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;probe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ID=markup</code> | <code>c.ID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ID=japanese</code> | <code>c.ID = &quot;日本語テキスト&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;日本語テキスト&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ID=empty</code> | <code>c.ID = &quot;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;ctl02&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.LoadScriptsBeforeUI=true</code> | <code>c.LoadScriptsBeforeUI = true;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.LoadScriptsBeforeUI=false</code> | <code>c.LoadScriptsBeforeUI = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ScriptMode=Auto</code> | <code>c.ScriptMode = ScriptMode.Auto;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ScriptMode=Inherit</code> | <code>c.ScriptMode = ScriptMode.Inherit;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ScriptMode=Debug</code> | <code>c.ScriptMode = ScriptMode.Debug;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ScriptMode=Release</code> | <code>c.ScriptMode = ScriptMode.Release;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ScriptPath=text</code> | <code>c.ScriptPath = &quot;probe&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ScriptPath=markup</code> | <code>c.ScriptPath = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ScriptPath=japanese</code> | <code>c.ScriptPath = &quot;日本語テキスト&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ScriptPath=empty</code> | <code>c.ScriptPath = &quot;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ScriptPath=app-relative</code> | <code>c.ScriptPath = &quot;~/Scripts&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.SkinID=text</code> | <code>c.SkinID = &quot;probe&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.SkinID=markup</code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.SkinID=japanese</code> | <code>c.SkinID = &quot;日本語テキスト&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.SkinID=empty</code> | <code>c.SkinID = &quot;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.SupportsPartialRendering=true</code> | <code>c.SupportsPartialRendering = true;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.SupportsPartialRendering=false</code> | <code>c.SupportsPartialRendering = false;</code> | <code></code> |  |
| <code>.ValidateRequestMode=Inherit</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Inherit;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ValidateRequestMode=Disabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Disabled;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ValidateRequestMode=Enabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Enabled;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ViewStateMode=Inherit</code> | <code>c.ViewStateMode = ViewStateMode.Inherit;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ViewStateMode=Enabled</code> | <code>c.ViewStateMode = ViewStateMode.Enabled;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ViewStateMode=Disabled</code> | <code>c.ViewStateMode = ViewStateMode.Disabled;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.Visible=true</code> | <code>c.Visible = true;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> | <code>c.Visible = true; -&gt; System.NotImplementedException: The method or operation is not implemented.</code> |
| <code>.Visible=false</code> | <code>c.Visible = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> | <code>c.Visible = false; -&gt; System.NotImplementedException: The method or operation is not implemented.</code> |
| <code>.EnablePartialRendering=false+EnablePageMethods=true</code> | <code>c.EnablePartialRendering = false; c.EnablePageMethods = true;</code> | <code></code> |  |
| <code>.EnableHistory=true+EnableSecureHistoryState=false</code> | <code>c.EnableHistory = true; c.EnableSecureHistoryState = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [&#39;smProbe&#39;,&#39;smProbe&#39;], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;input type=&quot;hidden&quot; name=&quot;smProbe&quot; id=&quot;smProbe&quot; /&gt;↵&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.Application.setServerId(&quot;smProbe&quot;, &quot;smProbe&quot;);↵Sys.Application._enableHistoryInScriptManager();↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.EnableHistory=true+ClientNavigateHandler=handler</code> | <code>c.EnableHistory = true; c.ClientNavigateHandler = &quot;onNavigate&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [&#39;smProbe&#39;,&#39;smProbe&#39;], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;input type=&quot;hidden&quot; name=&quot;smProbe&quot; id=&quot;smProbe&quot; /&gt;↵&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.Application.setServerId(&quot;smProbe&quot;, &quot;smProbe&quot;);↵Sys.Application._enableHistoryInScriptManager();↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ScriptMode=Debug+LoadScriptsBeforeUI=false</code> | <code>c.ScriptMode = ScriptMode.Debug; c.LoadScriptsBeforeUI = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.ScriptMode=Release+EnableScriptGlobalization=true</code> | <code>c.ScriptMode = ScriptMode.Release; c.EnableScriptGlobalization = true;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.EnableCdn=true+EnableCdnFallback=false</code> | <code>c.EnableCdn = true; c.EnableCdnFallback = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.AjaxFrameworkMode=Disabled+EnablePartialRendering=false</code> | <code>c.AjaxFrameworkMode = AjaxFrameworkMode.Disabled; c.EnablePartialRendering = false;</code> | <code></code> |  |
| <code>.AsyncPostBackTimeout=3+AsyncPostBackErrorMessage=text</code> | <code>c.AsyncPostBackTimeout = 3; c.AsyncPostBackErrorMessage = &quot;probe&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 3, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>.EnablePartialRendering=false+Visible=false</code> | <code>c.EnablePartialRendering = false; c.Visible = false;</code> | <code></code> | <code>c.Visible = false; -&gt; System.NotImplementedException: The method or operation is not implemented.</code> |
| <code>.EnablePageMethods=true+EnableViewState=false</code> | <code>c.EnablePageMethods = true; c.EnableViewState = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;</code> |  |
| <code>[naming container].(default)</code> | <code>[naming container] (nothing set)</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;box$smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵	//]]&gt;↵&lt;/script&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=AutoID</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;box$smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵	//]]&gt;↵&lt;/script&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Static</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;box$smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵	//]]&gt;↵&lt;/script&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Predictable</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;box$smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵	//]]&gt;↵&lt;/script&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Inherit</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;box$smProbe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵	//]]&gt;↵&lt;/script&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ID=text</code> | <code>[naming container] c.ID = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;box$probe&#39;, &#39;form1&#39;, [], [], [], 90, &#39;&#39;);↵	//]]&gt;↵&lt;/script&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].EnableHistory=true</code> | <code>[naming container] c.EnableHistory = true;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;box$smProbe&#39;, &#39;form1&#39;, [], [&#39;box$smProbe&#39;,&#39;box_smProbe&#39;], [], 90, &#39;&#39;);↵	//]]&gt;↵&lt;/script&gt;↵&lt;input type=&quot;hidden&quot; name=&quot;box$smProbe&quot; id=&quot;box_smProbe&quot; /&gt;↵&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.Application.setServerId(&quot;box_smProbe&quot;, &quot;box$smProbe&quot;);↵	Sys.Application._enableHistoryInScriptManager();↵	//]]&gt;↵&lt;/script&gt;↵↵&lt;/div&gt;</code> |  |

## ③ シナリオ層

ブラウザ（Playwright）で ScriptManagerScenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。

| # | 操作 | 対象 | 値 |
|---|---|---|---|
| 1 | goto | /AjaxScenario.aspx | <code></code> |
| 2 | snapshot |  | **ScriptManager [render][databind][dynamic] first request: the panels, the proxy&#39;s script run, a panel made in code** |
| 3 | fill | txtInPanel | <code>in the panel</code> |
| 4 | click | btnInPanel | <code></code> |
| 5 | snapshot |  | **ScriptManager [viewstate][postdata][templates] an asynchronous postback of the main panel: the conditional panel outside left as it was** |
| 6 | click | btnOutside | <code></code> |
| 7 | snapshot |  | **ScriptManager [viewstate][postdata] a button outside triggers the second panel** |
| 8 | click | btnFull | <code></code> |
| 9 | snapshot |  | **ScriptManager [viewstate][postdata] a postback trigger in the panel: a full postback** |
| 10 | evaluate | btnSlow | <code>e.click();</code> |
| 11 | snapshot |  | **ScriptManager [viewstate] while a slow request runs: the progress shown** |
| 12 | waitms |  | <code></code> |
| 13 | snapshot |  | **ScriptManager [viewstate] after the slow request: the progress hidden** |
| 14 | click | btnStartTimer | <code></code> |
| 15 | waitms |  | <code></code> |
| 16 | snapshot |  | **ScriptManager [viewstate] the timer started: one tick, then stopped** |
| 17 | click | btnHistory | <code></code> |
| 18 | evaluate | btnInPanel | <code>history.back();</code> |
| 19 | waitms |  | <code></code> |
| 20 | snapshot |  | **ScriptManager [postbackevent] a history point, then the browser&#39;s back: the script manager&#39;s Navigate** |
| 21 | click | btnDynamic | <code></code> |
| 22 | snapshot |  | **ScriptManager [dynamic][viewstate] an asynchronous postback of the panel made in code** |
| 23 | click | btnShow | <code></code> |
| 24 | snapshot |  | **ScriptManager [visible][viewstate] a hidden panel shown with what was set while hidden** |

<details><summary>ScriptManager [render][databind][dynamic] first request: the panels, the proxy&#39;s script run, a panel made in code（期待される本文）</summary>

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

<details><summary>ScriptManager [viewstate][postdata][templates] an asynchronous postback of the main panel: the conditional panel outside left as it was（期待される本文）</summary>

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

<details><summary>ScriptManager [viewstate][postdata] a button outside triggers the second panel（期待される本文）</summary>

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

<details><summary>ScriptManager [viewstate][postdata] a postback trigger in the panel: a full postback（期待される本文）</summary>

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

<details><summary>ScriptManager [viewstate] while a slow request runs: the progress shown（期待される本文）</summary>

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

<details><summary>ScriptManager [viewstate] after the slow request: the progress hidden（期待される本文）</summary>

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

<details><summary>ScriptManager [viewstate] the timer started: one tick, then stopped（期待される本文）</summary>

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

<details><summary>ScriptManager [postbackevent] a history point, then the browser&#39;s back: the script manager&#39;s Navigate（期待される本文）</summary>

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

<details><summary>ScriptManager [dynamic][viewstate] an asynchronous postback of the panel made in code（期待される本文）</summary>

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

<details><summary>ScriptManager [visible][viewstate] a hidden panel shown with what was set while hidden（期待される本文）</summary>

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

