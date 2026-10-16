# LoginView テストケース一覧

変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。
このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。

- 正解データの採取: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0
- ① API 層: 124 ケース（131 メンバー）
- ② HTML 層: 43 ケース
- ③ シナリオ層: 15 スナップショット

## 機構マトリクス

型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。

| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |
|---|---|---|---|
| プロパティ → HTML 描画 | すべてのコントロール | 29 | 1 |
| ViewState の保存と復元 | Control | 7 | 14 |
| Visible=false（描画されず状態は保持） | Control.Visible | 3 | 1 |
| 名前付けコンテナと ClientIDMode | Control.ClientIDMode | 6 | 0 |
| テーマとスキン（SkinID / EnableTheming） | テーマを付けられる型（[Themeable]） | 3 | 0 |
| 動的に生成したコントロールのライフサイクル | Control | 0 | 2 |
| データバインド（DataBind / <%# %>） | Control.DataBind | 0 | 1 |
| テンプレート | ITemplate のプロパティ | 0 | 3 |

該当しない機構: Enabled=false・Disabled=true（描画とポスト値の扱い）、ポストバックデータの取り込み、変更イベント（*Changed、ServerChange）、ReadOnly（ポストされた値を無視）、リクエスト検証（ValidateRequestMode）、ポストバックイベント、コマンドイベントと親へのバブル（CommandName）、送信ボタン／スクリプトによるポストバック（UseSubmitBehavior）、ブラウザ側のクリック処理（OnClientClick）、クロスページポストバック（PostBackUrl）、既定のボタン（Panel の DefaultButton）、AutoPostBack（クライアントスクリプトによるポストバック）、CausesValidation / ValidationGroup、検証対象（サーバー側の検証）、検証対象（ブラウザ側の検証）、双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値）、項目（ListItem の Selected・Enabled・Text・Value と ViewState）、複数選択、繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）、ポスト値のイベント検証（一覧にない値・無効な項目の拒否）、サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）、ブラウザ側の検証スクリプト（EnableClientScript）、検証結果のまとめ（ValidationSummary）、ページング、並べ替え、項目の挿入、行・項目の編集（編集・更新・取り消し・削除）、行・項目の選択、行・項目のコマンド（RowCommand・ItemCommand へのバブル）、データキー（DataKeyNames・DataKeys）、データソースコントロールとの連携（DataSourceID）、データソース連携（DataSource・DataTextField など）、クライアントコールバック

## ① API 層

LoginView とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の LoginView インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。

| ケース | 期待値（.NET Framework） |
|---|---|
| <code>Api.Control.#ctor</code> | <code>Control.#ctor = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMode=In…</code> |
| <code>Api.Control.Adapter</code> | <code>Control.Adapter = null</code> |
| <code>Api.Control.AddParsedSubObject</code> | <code>Control.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>Control.AddParsedSubObject(Object) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus…</code> |
| <code>Api.Control.AddedControl</code> | <code>Control.AddedControl(Control,Int32) = &quot;ok&quot;</code><br><code>Control.AddedControl(Control,Int32) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatu…</code> |
| <code>Api.Control.AppRelativeTemplateSourceDirectory</code> | <code>Control.AppRelativeTemplateSourceDirectory = &quot;&quot;</code><br><code>Control.AppRelativeTemplateSourceDirectory set = &quot;-&gt; System.ArgumentException: The relative virtual path &#39;Abc 123&#39; is not allowed here.&quot;</code> |
| <code>Api.Control.ApplyStyleSheetSkin</code> | <code>Control.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>Control.ApplyStyleSheetSkin(Page) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus&quot;…</code> |
| <code>Api.Control.BeginRenderTracing</code> | <code>Control.BeginRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.BeginRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.BeginRenderTracing(TextWriter,Object) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; I…</code> |
| <code>Api.Control.BindingContainer</code> | <code>Control.BindingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Clien…</code> |
| <code>Api.Control.BuildProfileTree</code> | <code>Control.BuildProfileTree(String,Boolean) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.ChildControlsCreated</code> | <code>Control.ChildControlsCreated = &quot;false&quot;</code><br><code>Control.ChildControlsCreated set = &quot;ok&quot;</code> |
| <code>Api.Control.ClearCachedClientID</code> | <code>Control.ClearCachedClientID = &quot;ok&quot;</code><br><code>Control.ClearCachedClientID after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus&quot;; Skin…</code> |
| <code>Api.Control.ClearChildControlState</code> | <code>Control.ClearChildControlState = &quot;ok&quot;</code><br><code>Control.ClearChildControlState after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus&quot;; S…</code> |
| <code>Api.Control.ClearChildState</code> | <code>Control.ClearChildState = &quot;ok&quot;</code><br><code>Control.ClearChildState after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus&quot;; SkinID=&quot;…</code> |
| <code>Api.Control.ClearChildViewState</code> | <code>Control.ClearChildViewState = &quot;ok&quot;</code><br><code>Control.ClearChildViewState after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus&quot;; Skin…</code> |
| <code>Api.Control.ClearEffectiveClientIDMode</code> | <code>Control.ClearEffectiveClientIDMode = &quot;ok&quot;</code><br><code>Control.ClearEffectiveClientIDMode after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus…</code> |
| <code>Api.Control.ClientID</code> | <code>Control.ClientID = &quot;lvStatus&quot;</code> |
| <code>Api.Control.ClientIDMode</code> | <code>Control.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>Control.ClientIDMode set = &quot;ClientIDMode.AutoID&quot;</code> |
| <code>Api.Control.ClientIDSeparator</code> | <code>Control.ClientIDSeparator = &quot;&#39;_&#39;&quot;</code> |
| <code>Api.Control.Context</code> | <code>Control.Context = null</code> |
| <code>Api.Control.Controls</code> | <code>Control.Controls = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code> |
| <code>Api.Control.CreateChildControls</code> | <code>Control.CreateChildControls = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code> |
| <code>Api.Control.CreateControlCollection</code> | <code>Control.CreateControlCollection = ControlCollection []</code><br><code>Control.CreateControlCollection after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus&quot;; …</code> |
| <code>Api.Control.DataBind</code> | <code>Control.DataBind = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code><br><code>Control.DataBind(Boolean) = &quot;ok&quot;</code><br><code>Control.DataBind(Boolean) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus&quot;; SkinID…</code> |
| <code>Api.Control.DataBindChildren</code> | <code>Control.DataBindChildren = &quot;ok&quot;</code><br><code>Control.DataBindChildren after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus&quot;; SkinID=…</code> |
| <code>Api.Control.DataBinding</code> | <code>Control.DataBinding = &quot;added and removed&quot;</code> |
| <code>Api.Control.DataItemContainer</code> | <code>Control.DataItemContainer = null</code> |
| <code>Api.Control.DataKeysContainer</code> | <code>Control.DataKeysContainer = null</code> |
| <code>Api.Control.DesignMode</code> | <code>Control.DesignMode = &quot;false&quot;</code> |
| <code>Api.Control.Dispose</code> | <code>Control.Dispose = &quot;ok&quot;</code><br><code>Control.Dispose after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus&quot;; SkinID=&quot;&quot;; Uniqu…</code> |
| <code>Api.Control.Disposed</code> | <code>Control.Disposed = &quot;added and removed&quot;</code> |
| <code>Api.Control.EnableTheming</code> | <code>Control.EnableTheming = &quot;true&quot;</code><br><code>Control.EnableTheming set = &quot;true&quot;</code> |
| <code>Api.Control.EnableViewState</code> | <code>Control.EnableViewState = &quot;true&quot;</code><br><code>Control.EnableViewState set = &quot;true&quot;</code> |
| <code>Api.Control.EndRenderTracing</code> | <code>Control.EndRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.EndRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.EndRenderTracing(TextWriter,Object) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=…</code> |
| <code>Api.Control.EnsureChildControls</code> | <code>Control.EnsureChildControls = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code> |
| <code>Api.Control.EnsureID</code> | <code>Control.EnsureID = &quot;ok&quot;</code><br><code>Control.EnsureID after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus&quot;; SkinID=&quot;&quot;; Uniq…</code> |
| <code>Api.Control.Events</code> | <code>Control.Events = &quot;System.ComponentModel.EventHandlerList&quot;</code> |
| <code>Api.Control.FindControl</code> | <code>Control.FindControl(String) = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code><br><code>Control.FindControl(String,Int32) = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code> |
| <code>Api.Control.Focus</code> | <code>Control.Focus = &quot;-&gt; System.NotSupportedException: Control of type &#39;LoginView&#39; does not support the Focus operation.&quot;</code> |
| <code>Api.Control.GetDesignModeState</code> | <code>Control.GetDesignModeState = &quot;[]&quot;</code><br><code>Control.GetDesignModeState after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus&quot;; SkinI…</code> |
| <code>Api.Control.GetRouteUrl</code> | <code>Control.GetRouteUrl(Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.GetUniqueIDRelativeTo</code> | <code>Control.GetUniqueIDRelativeTo(Control) = &quot;lvStatus&quot;</code><br><code>Control.GetUniqueIDRelativeTo(Control) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvSt…</code> |
| <code>Api.Control.HasChildViewState</code> | <code>Control.HasChildViewState = &quot;false&quot;</code> |
| <code>Api.Control.HasControls</code> | <code>Control.HasControls = &quot;false&quot;</code><br><code>Control.HasControls after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus&quot;; SkinID=&quot;&quot;; U…</code> |
| <code>Api.Control.HasEvents</code> | <code>Control.HasEvents = &quot;false&quot;</code><br><code>Control.HasEvents after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus&quot;; SkinID=&quot;&quot;; Uni…</code> |
| <code>Api.Control.ID</code> | <code>Control.ID = &quot;lvStatus&quot;</code><br><code>Control.ID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.IdSeparator</code> | <code>Control.IdSeparator = &quot;&#39;$&#39;&quot;</code> |
| <code>Api.Control.Init</code> | <code>Control.Init = &quot;added and removed&quot;</code> |
| <code>Api.Control.IsChildControlStateCleared</code> | <code>Control.IsChildControlStateCleared = &quot;false&quot;</code> |
| <code>Api.Control.IsLiteralContent</code> | <code>Control.IsLiteralContent = &quot;false&quot;</code><br><code>Control.IsLiteralContent after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus&quot;; SkinID=…</code> |
| <code>Api.Control.IsTrackingViewState</code> | <code>Control.IsTrackingViewState = &quot;false&quot;</code> |
| <code>Api.Control.IsViewStateEnabled</code> | <code>Control.IsViewStateEnabled = &quot;true&quot;</code> |
| <code>Api.Control.Load</code> | <code>Control.Load = &quot;added and removed&quot;</code> |
| <code>Api.Control.LoadControlState</code> | <code>Control.LoadControlState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Web.UI.Pair&#39;.&quot;</code> |
| <code>Api.Control.LoadViewState</code> | <code>Control.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Collections.ArrayList&#39;.&quot;</code> |
| <code>Api.Control.LoadViewStateByID</code> | <code>Control.LoadViewStateByID = &quot;false&quot;</code> |
| <code>Api.Control.MapPathSecure</code> | <code>Control.MapPathSecure(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.NamingContainer</code> | <code>Control.NamingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code> |
| <code>Api.Control.OnBubbleEvent</code> | <code>Control.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Control.OnBubbleEvent(Object,EventArgs) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvS…</code> |
| <code>Api.Control.OnDataBinding</code> | <code>Control.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnDataBinding(EventArgs) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus&quot;;…</code> |
| <code>Api.Control.OnInit</code> | <code>Control.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnInit(EventArgs) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus&quot;; SkinID…</code> |
| <code>Api.Control.OnLoad</code> | <code>Control.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnLoad(EventArgs) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus&quot;; SkinID…</code> |
| <code>Api.Control.OnPreRender</code> | <code>Control.OnPreRender(EventArgs) = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code> |
| <code>Api.Control.OnUnload</code> | <code>Control.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnUnload(EventArgs) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus&quot;; Skin…</code> |
| <code>Api.Control.OpenFile</code> | <code>Control.OpenFile(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.Page</code> | <code>Control.Page = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=&quot;&quot;; …</code><br><code>Control.Page set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=…</code> |
| <code>Api.Control.Parent</code> | <code>Control.Parent = HtmlForm#form1 {Action=&quot;&quot;; ClientID=&quot;form1&quot;; ClientIDMode=Inherit; DefaultButton=&quot;&quot;; DefaultFocus=&quot;&quot;; Disabled=false; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.PreRender</code> | <code>Control.PreRender = &quot;added and removed&quot;</code> |
| <code>Api.Control.RaiseBubbleEvent</code> | <code>Control.RaiseBubbleEvent(Object,EventArgs) = &quot;ok&quot;</code><br><code>Control.RaiseBubbleEvent(Object,EventArgs) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;…</code> |
| <code>Api.Control.RemovedControl</code> | <code>Control.RemovedControl(Control) = &quot;ok&quot;</code><br><code>Control.RemovedControl(Control) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus&quot;; …</code> |
| <code>Api.Control.Render</code> | <code>Control.Render(HtmlTextWriter) = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code> |
| <code>Api.Control.RenderChildren</code> | <code>Control.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderChildren(HtmlTextWriter) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvSt…</code> |
| <code>Api.Control.RenderControl</code> | <code>Control.RenderControl(HtmlTextWriter) = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code> |
| <code>Api.Control.RenderingCompatibility</code> | <code>Control.RenderingCompatibility = &quot;System.Version 4.0&quot;</code><br><code>Control.RenderingCompatibility set = &quot;System.Version 4.0&quot;</code> |
| <code>Api.Control.ResolveAdapter</code> | <code>Control.ResolveAdapter = null</code><br><code>Control.ResolveAdapter after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus&quot;; SkinID=&quot;&quot;…</code> |
| <code>Api.Control.ResolveClientUrl</code> | <code>Control.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveClientUrl(String) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus&quot;;…</code> |
| <code>Api.Control.ResolveUrl</code> | <code>Control.ResolveUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveUrl(String) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus&quot;; SkinI…</code> |
| <code>Api.Control.SaveControlState</code> | <code>Control.SaveControlState = null</code><br><code>Control.SaveControlState after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus&quot;; SkinID=…</code> |
| <code>Api.Control.SaveViewState</code> | <code>Control.SaveViewState = null</code><br><code>Control.SaveViewState after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus&quot;; SkinID=&quot;&quot;;…</code> |
| <code>Api.Control.SetDesignModeState</code> | <code>Control.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>Control.SetDesignModeState(IDictionary) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvS…</code> |
| <code>Api.Control.SetRenderMethodDelegate</code> | <code>Control.SetRenderMethodDelegate(RenderMethod) = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code> |
| <code>Api.Control.SetTraceData</code> | <code>Control.SetTraceData(Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatu…</code><br><code>Control.SetTraceData(Object,Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object,Object) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;…</code> |
| <code>Api.Control.Site</code> | <code>Control.Site = null</code><br><code>Control.Site set = null</code> |
| <code>Api.Control.SkinID</code> | <code>Control.SkinID = &quot;&quot;</code><br><code>Control.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.TemplateControl</code> | <code>Control.TemplateControl = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code><br><code>Control.TemplateControl set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Cl…</code> |
| <code>Api.Control.TemplateSourceDirectory</code> | <code>Control.TemplateSourceDirectory = &quot;&quot;</code> |
| <code>Api.Control.TrackViewState</code> | <code>Control.TrackViewState = &quot;ok&quot;</code><br><code>Control.TrackViewState after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus&quot;; SkinID=&quot;&quot;…</code> |
| <code>Api.Control.UniqueID</code> | <code>Control.UniqueID = &quot;lvStatus&quot;</code> |
| <code>Api.Control.Unload</code> | <code>Control.Unload = &quot;added and removed&quot;</code> |
| <code>Api.Control.ValidateRequestMode</code> | <code>Control.ValidateRequestMode = &quot;ValidateRequestMode.Inherit&quot;</code><br><code>Control.ValidateRequestMode set = &quot;ValidateRequestMode.Disabled&quot;</code> |
| <code>Api.Control.ViewState</code> | <code>Control.ViewState = StateBag []</code> |
| <code>Api.Control.ViewStateIgnoresCase</code> | <code>Control.ViewStateIgnoresCase = &quot;false&quot;</code> |
| <code>Api.Control.ViewStateMode</code> | <code>Control.ViewStateMode = &quot;ViewStateMode.Inherit&quot;</code><br><code>Control.ViewStateMode set = &quot;ViewStateMode.Enabled&quot;</code> |
| <code>Api.Control.Visible</code> | <code>Control.Visible = &quot;true&quot;</code><br><code>Control.Visible set = &quot;true&quot;</code> |
| <code>Api.IControlBuilderAccessor.ControlBuilder</code> | <code>IControlBuilderAccessor.ControlBuilder = null</code> |
| <code>Api.IControlDesignerAccessor.GetDesignModeState</code> | <code>IControlDesignerAccessor.GetDesignModeState = &quot;[]&quot;</code><br><code>IControlDesignerAccessor.GetDesignModeState after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=…</code> |
| <code>Api.IControlDesignerAccessor.SetDesignModeState</code> | <code>IControlDesignerAccessor.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetDesignModeState(IDictionary) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewSt…</code> |
| <code>Api.IControlDesignerAccessor.SetOwnerControl</code> | <code>IControlDesignerAccessor.SetOwnerControl(Control) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetOwnerControl(Control) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=tru…</code> |
| <code>Api.IControlDesignerAccessor.UserData</code> | <code>IControlDesignerAccessor.UserData = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.DataBindings</code> | <code>IDataBindingsAccessor.DataBindings = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.HasDataBindings</code> | <code>IDataBindingsAccessor.HasDataBindings = &quot;false&quot;</code> |
| <code>Api.IExpressionsAccessor.Expressions</code> | <code>IExpressionsAccessor.Expressions = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.HasExpressions</code> | <code>IExpressionsAccessor.HasExpressions = &quot;false&quot;</code> |
| <code>Api.IParserAccessor.AddParsedSubObject</code> | <code>IParserAccessor.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>IParserAccessor.AddParsedSubObject(Object) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;…</code> |
| <code>Api.IUrlResolutionService.ResolveClientUrl</code> | <code>IUrlResolutionService.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>IUrlResolutionService.ResolveClientUrl(String) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; …</code> |
| <code>Api.WebControls.LoginView.#ctor</code> | <code>LoginView.#ctor = LoginView# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMod…</code> |
| <code>Api.WebControls.LoginView.AnonymousTemplate</code> | <code>LoginView.AnonymousTemplate = null</code><br><code>LoginView.AnonymousTemplate set = null</code> |
| <code>Api.WebControls.LoginView.Controls</code> | <code>LoginView.Controls = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code> |
| <code>Api.WebControls.LoginView.CreateChildControls</code> | <code>LoginView.CreateChildControls = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code> |
| <code>Api.WebControls.LoginView.DataBind</code> | <code>LoginView.DataBind = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code> |
| <code>Api.WebControls.LoginView.EnableTheming</code> | <code>LoginView.EnableTheming = &quot;true&quot;</code><br><code>LoginView.EnableTheming set = &quot;true&quot;</code> |
| <code>Api.WebControls.LoginView.Focus</code> | <code>LoginView.Focus = &quot;-&gt; System.NotSupportedException: Control of type &#39;LoginView&#39; does not support the Focus operation.&quot;</code> |
| <code>Api.WebControls.LoginView.LoadControlState</code> | <code>LoginView.LoadControlState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Web.UI.Pair&#39;.&quot;</code> |
| <code>Api.WebControls.LoginView.LoggedInTemplate</code> | <code>LoginView.LoggedInTemplate = null</code><br><code>LoginView.LoggedInTemplate set = null</code> |
| <code>Api.WebControls.LoginView.OnInit</code> | <code>LoginView.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>LoginView.OnInit(EventArgs) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus&quot;; Skin…</code> |
| <code>Api.WebControls.LoginView.OnPreRender</code> | <code>LoginView.OnPreRender(EventArgs) = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code> |
| <code>Api.WebControls.LoginView.OnViewChanged</code> | <code>LoginView.OnViewChanged(EventArgs) = &quot;ok&quot;</code><br><code>LoginView.OnViewChanged(EventArgs) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus…</code> |
| <code>Api.WebControls.LoginView.OnViewChanging</code> | <code>LoginView.OnViewChanging(EventArgs) = &quot;ok&quot;</code><br><code>LoginView.OnViewChanging(EventArgs) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatu…</code> |
| <code>Api.WebControls.LoginView.Render</code> | <code>LoginView.Render(HtmlTextWriter) = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code> |
| <code>Api.WebControls.LoginView.RoleGroups</code> | <code>LoginView.RoleGroups = &quot;[]&quot;</code> |
| <code>Api.WebControls.LoginView.SaveControlState</code> | <code>LoginView.SaveControlState = null</code><br><code>LoginView.SaveControlState after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;lvStatus&quot;; SkinI…</code> |
| <code>Api.WebControls.LoginView.SetDesignModeState</code> | <code>LoginView.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>LoginView.SetDesignModeState(IDictionary) after = LoginView#lvStatus {ClientID=&quot;lvStatus&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;l…</code> |
| <code>Api.WebControls.LoginView.SkinID</code> | <code>LoginView.SkinID = &quot;&quot;</code><br><code>LoginView.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.LoginView.ViewChanged</code> | <code>LoginView.ViewChanged = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.LoginView.ViewChanging</code> | <code>LoginView.ViewChanging = &quot;added and removed&quot;</code> |

## ② HTML 層

プローブサイトの RenderProbe.aspx で、1 ケース 1 リクエスト（ページライフサイクル全体）で描画したページ全体を比較します。下表はコンポーネント部分（div#probe）、設定時の例外、保存された ViewState の要約です。

| ケース | 設定 | 期待される描画 | 設定時の例外 |
|---|---|---|---|
| <code>.(default)</code> | <code>(nothing set)</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>.AppRelativeTemplateSourceDirectory=text</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;probe&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=markup</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=japanese</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;日本語テキスト&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=empty</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&quot;;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>.ClientIDMode=Inherit</code> | <code>c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>.ClientIDMode=AutoID</code> | <code>c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>.ClientIDMode=Predictable</code> | <code>c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>.ClientIDMode=Static</code> | <code>c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>.EnableTheming=true</code> | <code>c.EnableTheming = true;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>.EnableTheming=false</code> | <code>c.EnableTheming = false;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>.EnableViewState=true</code> | <code>c.EnableViewState = true;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>.EnableViewState=false</code> | <code>c.EnableViewState = false;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>.ID=text</code> | <code>c.ID = &quot;probe&quot;;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>.ID=markup</code> | <code>c.ID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>.ID=japanese</code> | <code>c.ID = &quot;日本語テキスト&quot;;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>.ID=empty</code> | <code>c.ID = &quot;&quot;;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>.SkinID=text</code> | <code>c.SkinID = &quot;probe&quot;;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>.SkinID=markup</code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>.SkinID=japanese</code> | <code>c.SkinID = &quot;日本語テキスト&quot;;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>.SkinID=empty</code> | <code>c.SkinID = &quot;&quot;;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>.ValidateRequestMode=Inherit</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Inherit;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>.ValidateRequestMode=Disabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Disabled;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>.ValidateRequestMode=Enabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Enabled;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>.ViewStateMode=Inherit</code> | <code>c.ViewStateMode = ViewStateMode.Inherit;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>.ViewStateMode=Enabled</code> | <code>c.ViewStateMode = ViewStateMode.Enabled;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>.ViewStateMode=Disabled</code> | <code>c.ViewStateMode = ViewStateMode.Disabled;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>.Visible=true</code> | <code>c.Visible = true;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>.Visible=false</code> | <code>c.Visible = false;</code> | <code></code> |  |
| <code>.Visible=false+EnableViewState=false</code> | <code>c.Visible = false; c.EnableViewState = false;</code> | <code></code> |  |
| <code>[naming container].(default)</code> | <code>[naming container] (nothing set)</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;span&gt;Anonymous&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=AutoID</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;span&gt;Anonymous&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Static</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;span&gt;Anonymous&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Predictable</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;span&gt;Anonymous&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Inherit</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;span&gt;Anonymous&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ID=text</code> | <code>[naming container] c.ID = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;span&gt;Anonymous&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>[themed].(default)</code> | <code>[themed] (nothing set)</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>[themed].SkinID=text</code> | <code>[themed] c.SkinID = &quot;probe&quot;;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>[themed].EnableTheming=false</code> | <code>[themed] c.EnableTheming = false;</code> | <code>&lt;span&gt;Anonymous&lt;/span&gt;</code> |  |
| <code>[logged in].(default)</code> | <code>[logged in] (nothing set)</code> | <code>&lt;span&gt;An admin&lt;/span&gt;</code> |  |
| <code>[logged in].Visible=false</code> | <code>[logged in] c.Visible = false;</code> | <code></code> |  |
| <code>[logged in].EnableViewState=false</code> | <code>[logged in] c.EnableViewState = false;</code> | <code>&lt;span&gt;An admin&lt;/span&gt;</code> |  |
| <code>[no templates].(default)</code> | <code>[no templates] (nothing set)</code> | <code></code> |  |

## ③ シナリオ層

ブラウザ（Playwright）で LoginViewScenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。

| # | 操作 | 対象 | 値 |
|---|---|---|---|
| 1 | goto | /LoginScenario.aspx?reset=1 | <code></code> |
| 2 | snapshot |  | **LoginView [render][databind][dynamic][templates] first request, anonymous: the login forms, the status, the views, the wizards** |
| 3 | fill | lgnMain_UserName | <code>taro</code> |
| 4 | fill | lgnMain_Password | <code>wrong</code> |
| 5 | click | lgnMain_LoginButton | <code></code> |
| 6 | snapshot |  | **LoginView [viewstate] a login with a wrong password: its error** |
| 7 | fill | lgnMain_UserName | <code>taro</code> |
| 8 | fill | lgnMain_Password | <code>Passw0rd!</code> |
| 9 | click | lgnMain_LoginButton | <code></code> |
| 10 | snapshot |  | **LoginView [viewstate][templates] logged in (redirected to the page): the name, the status, the role&#39;s view** |
| 11 | fill | cpwChange_ChangePasswordContainerID_CurrentPassword | <code>Passw0rd!</code> |
| 12 | fill | cpwChange_ChangePasswordContainerID_NewPassword | <code>Changed1!</code> |
| 13 | fill | cpwChange_ChangePasswordContainerID_ConfirmNewPassword | <code>Changed1!</code> |
| 14 | click | cpwChange_ChangePasswordContainerID_ChangePasswordPushButton | <code></code> |
| 15 | snapshot |  | **LoginView [viewstate] the password changed: the success template** |
| 16 | click | lsStatus | <code></code> |
| 17 | snapshot |  | **LoginView [viewstate][templates] logged out by the status** |
| 18 | fill | prRecover_UserNameContainerID_UserName | <code>nobody</code> |
| 19 | click | prRecover_UserNameContainerID_SubmitButton | <code></code> |
| 20 | snapshot |  | **LoginView [viewstate] a recovery for a user who does not exist** |
| 21 | fill | prRecover_UserNameContainerID_UserName | <code>taro</code> |
| 22 | click | prRecover_UserNameContainerID_SubmitButton | <code></code> |
| 23 | snapshot |  | **LoginView [viewstate] the user&#39;s question** |
| 24 | fill | prRecover_QuestionContainerID_Answer | <code>blue</code> |
| 25 | click | prRecover_QuestionContainerID_SubmitButton | <code></code> |
| 26 | snapshot |  | **LoginView [viewstate] the answer right: the password reset, its mail prepared (cancelled), the success template** |
| 27 | fill | cuwCreate_CreateUserStepContainer_UserName | <code>hanako</code> |
| 28 | fill | cuwCreate_CreateUserStepContainer_Password | <code>Hanako1!</code> |
| 29 | fill | cuwCreate_CreateUserStepContainer_ConfirmPassword | <code>Hanako1!</code> |
| 30 | fill | cuwCreate_CreateUserStepContainer_Email | <code>hanako@example.com</code> |
| 31 | fill | cuwCreate_CreateUserStepContainer_Question | <code>pet</code> |
| 32 | fill | cuwCreate_CreateUserStepContainer_Answer | <code>cat</code> |
| 33 | clicksubmit |  | <code>Create the user</code> |
| 34 | snapshot |  | **LoginView [viewstate] a user created: the complete step** |
| 35 | fill | txtFirst | <code>first step</code> |
| 36 | clicktextin | wizSteps | <code>Next</code> |
| 37 | snapshot |  | **LoginView [viewstate] the wizard&#39;s next step (the templated one)** |
| 38 | clicktextin | wizSteps | <code>Next</code> |
| 39 | clicktextin | wizSteps | <code>Previous</code> |
| 40 | snapshot |  | **LoginView [viewstate] next, then previous** |
| 41 | clicktextin | wizSteps | <code>Last</code> |
| 42 | snapshot |  | **LoginView [viewstate] a step chosen in the side bar** |
| 43 | clicktextin | wizSteps | <code>Finish</code> |
| 44 | snapshot |  | **LoginView [viewstate] finished: the complete step, the first step&#39;s value kept** |
| 45 | fill | txtDynamic | <code>in a dynamic wizard</code> |
| 46 | clicktextin | wizDynamic | <code>Next</code> |
| 47 | snapshot |  | **LoginView [dynamic][viewstate] a wizard made in code: its next step** |
| 48 | click | btnShow | <code></code> |
| 49 | snapshot |  | **LoginView [visible][viewstate] a hidden wizard shown** |

<details><summary>LoginView [render][databind][dynamic][templates] first request, anonymous: the login forms, the status, the views, the wizards（期待される本文）</summary>

```text
Status: Login Name:
Anonymous view
Log In
User Name:
Password:
Remember me next time.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
Forgot Your Password?
Enter your User Name to receive your password.
User Name:
Create
User Name:
Password:
Confirm Password:
E-mail:
Security Question:
Security Answer:
Skip Navigation Links.
First
Second
Last
Done
[wizard header]
First:
Dynamic: Login
Skip Navigation Links.
Dynamic first
Dynamic last
Request 1: IsPostBack=False User=(anonymous)
State users: taro
State wizSteps ActiveStepIndex=0 txtFirst=""
State wizDynamic ActiveStepIndex=0
State cuwCreate ActiveStepIndex=0
State User=(anonymous)
```

</details>

<details><summary>LoginView [viewstate] a login with a wrong password: its error（期待される本文）</summary>

```text
Status: Login Name:
Anonymous view
Log In
User Name:
Password:
Remember me next time.
Your login attempt was not successful. Please try again.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
Forgot Your Password?
Enter your User Name to receive your password.
User Name:
Create
User Name:
Password:
Confirm Password:
E-mail:
Security Question:
Security Answer:
Skip Navigation Links.
First
Second
Last
Done
[wizard header]
First:
Dynamic: Login
Skip Navigation Links.
Dynamic first
Dynamic last
Request 2: IsPostBack=True User=(anonymous)
LoggingIn taro
Authenticate taro Authenticated=False
LoginError taro
State users: taro
State wizSteps ActiveStepIndex=0 txtFirst=""
State wizDynamic ActiveStepIndex=0
State cuwCreate ActiveStepIndex=0
State User=(anonymous)
```

</details>

<details><summary>LoginView [viewstate][templates] logged in (redirected to the page): the name, the status, the role&#39;s view（期待される本文）</summary>

```text
Status: Logout Name: Hello, taro
Admins' view: taro
Log In
User Name:
Password:
Remember me next time.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
Forgot Your Password?
Enter your User Name to receive your password.
User Name:
Create
User Name:
Password:
Confirm Password:
E-mail:
Security Question:
Security Answer:
Skip Navigation Links.
First
Second
Last
Done
[wizard header]
First:
Dynamic: Logout
Skip Navigation Links.
Dynamic first
Dynamic last
Request 1: IsPostBack=False User=taro
State users: taro
State wizSteps ActiveStepIndex=0 txtFirst=""
State wizDynamic ActiveStepIndex=0
State cuwCreate ActiveStepIndex=0
State User=taro admins=True
```

</details>

<details><summary>LoginView [viewstate] the password changed: the success template（期待される本文）</summary>

```text
Status: Logout Name: Hello, taro
Admins' view: taro
Log In
User Name:
Password:
Remember me next time.
Templated login:
The password changed.
Forgot Your Password?
Enter your User Name to receive your password.
User Name:
Create
User Name:
Password:
Confirm Password:
E-mail:
Security Question:
Security Answer:
Skip Navigation Links.
First
Second
Last
Done
[wizard header]
First:
Dynamic: Logout
Skip Navigation Links.
Dynamic first
Dynamic last
Request 2: IsPostBack=True User=taro
ChangingPassword taro
ChangedPassword taro valid with the new one=True
State users: taro
State wizSteps ActiveStepIndex=0 txtFirst=""
State wizDynamic ActiveStepIndex=0
State cuwCreate ActiveStepIndex=0
State User=taro admins=True
```

</details>

<details><summary>LoginView [viewstate][templates] logged out by the status（期待される本文）</summary>

```text
Status: Login Name:
Anonymous view
Log In
User Name:
Password:
Remember me next time.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
Forgot Your Password?
Enter your User Name to receive your password.
User Name:
Create
User Name:
Password:
Confirm Password:
E-mail:
Security Question:
Security Answer:
Skip Navigation Links.
First
Second
Last
Done
[wizard header]
First:
Dynamic: Login
Skip Navigation Links.
Dynamic first
Dynamic last
Request 1: IsPostBack=False User=(anonymous)
State users: taro
State wizSteps ActiveStepIndex=0 txtFirst=""
State wizDynamic ActiveStepIndex=0
State cuwCreate ActiveStepIndex=0
State User=(anonymous)
```

</details>

<details><summary>LoginView [viewstate] a recovery for a user who does not exist（期待される本文）</summary>

```text
Status: Login Name:
Anonymous view
Log In
User Name:
Password:
Remember me next time.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
Forgot Your Password?
Enter your User Name to receive your password.
User Name:
We were unable to access your information. Please try again.
Create
User Name:
Password:
Confirm Password:
E-mail:
Security Question:
Security Answer:
Skip Navigation Links.
First
Second
Last
Done
[wizard header]
First:
Dynamic: Login
Skip Navigation Links.
Dynamic first
Dynamic last
Request 2: IsPostBack=True User=(anonymous)
VerifyingUser nobody
UserLookupError nobody
State users: taro
State wizSteps ActiveStepIndex=0 txtFirst=""
State wizDynamic ActiveStepIndex=0
State cuwCreate ActiveStepIndex=0
State User=(anonymous)
```

</details>

<details><summary>LoginView [viewstate] the user&#39;s question（期待される本文）</summary>

```text
Status: Login Name:
Anonymous view
Log In
User Name:
Password:
Remember me next time.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
Identity Confirmation
Answer the following question to receive your password.
User Name: taro
Question: color
Answer:
Create
User Name:
Password:
Confirm Password:
E-mail:
Security Question:
Security Answer:
Skip Navigation Links.
First
Second
Last
Done
[wizard header]
First:
Dynamic: Login
Skip Navigation Links.
Dynamic first
Dynamic last
Request 3: IsPostBack=True User=(anonymous)
VerifyingUser taro
State users: taro
State wizSteps ActiveStepIndex=0 txtFirst=""
State wizDynamic ActiveStepIndex=0
State cuwCreate ActiveStepIndex=0
State User=(anonymous)
```

</details>

<details><summary>LoginView [viewstate] the answer right: the password reset, its mail prepared (cancelled), the success template（期待される本文）</summary>

```text
Status: Login Name:
Anonymous view
Log In
User Name:
Password:
Remember me next time.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
The recovery mail prepared.
Create
User Name:
Password:
Confirm Password:
E-mail:
Security Question:
Security Answer:
Skip Navigation Links.
First
Second
Last
Done
[wizard header]
First:
Dynamic: Login
Skip Navigation Links.
Dynamic first
Dynamic last
Request 4: IsPostBack=True User=(anonymous)
VerifyingAnswer taro Question=color
SendingMail To=taro@example.com Subject=Your password Body=Please return to the site and log in using the following information. User Name: taro Password: NewPass1!
State users: taro
State wizSteps ActiveStepIndex=0 txtFirst=""
State wizDynamic ActiveStepIndex=0
State cuwCreate ActiveStepIndex=0
State User=(anonymous)
```

</details>

<details><summary>LoginView [viewstate] a user created: the complete step（期待される本文）</summary>

```text
Status: Login Name:
Anonymous view
Log In
User Name:
Password:
Remember me next time.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
The recovery mail prepared.
The user was created.
Skip Navigation Links.
First
Second
Last
Done
[wizard header]
First:
Dynamic: Login
Skip Navigation Links.
Dynamic first
Dynamic last
Request 5: IsPostBack=True User=(anonymous)
CreatingUser hanako
CreatedUser hanako exists=True
ActiveStepChanged cuwCreate ActiveStepIndex=1
State users: hanako, taro
State wizSteps ActiveStepIndex=0 txtFirst=""
State wizDynamic ActiveStepIndex=0
State cuwCreate ActiveStepIndex=1
State User=(anonymous)
```

</details>

<details><summary>LoginView [viewstate] the wizard&#39;s next step (the templated one)（期待される本文）</summary>

```text
Status: Login Name:
Anonymous view
Log In
User Name:
Password:
Remember me next time.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
The recovery mail prepared.
The user was created.
Skip Navigation Links.
First
Second
Last
Done
[wizard header]
Second (templated): bound
Dynamic: Login
Skip Navigation Links.
Dynamic first
Dynamic last
Request 6: IsPostBack=True User=(anonymous)
NextButtonClick 0 -> 1
Deactivate wsFirst
Activate twsSecond
ActiveStepChanged wizSteps ActiveStepIndex=1
State users: hanako, taro
State wizSteps ActiveStepIndex=1 txtFirst="first step"
State wizDynamic ActiveStepIndex=0
State cuwCreate ActiveStepIndex=1
State User=(anonymous)
```

</details>

<details><summary>LoginView [viewstate] next, then previous（期待される本文）</summary>

```text
Status: Login Name:
Anonymous view
Log In
User Name:
Password:
Remember me next time.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
The recovery mail prepared.
The user was created.
Skip Navigation Links.
First
Second
Last
Done
[wizard header]
Second (templated): bound
Dynamic: Login
Skip Navigation Links.
Dynamic first
Dynamic last
Request 8: IsPostBack=True User=(anonymous)
PreviousButtonClick 2 -> 1
Deactivate wsLast
Activate twsSecond
ActiveStepChanged wizSteps ActiveStepIndex=1
State users: hanako, taro
State wizSteps ActiveStepIndex=1 txtFirst="first step"
State wizDynamic ActiveStepIndex=0
State cuwCreate ActiveStepIndex=1
State User=(anonymous)
```

</details>

<details><summary>LoginView [viewstate] a step chosen in the side bar（期待される本文）</summary>

```text
Status: Login Name:
Anonymous view
Log In
User Name:
Password:
Remember me next time.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
The recovery mail prepared.
The user was created.
Skip Navigation Links.
First
Second
Last
Done
[wizard header]
Last: set in code
Dynamic: Login
Skip Navigation Links.
Dynamic first
Dynamic last
Request 9: IsPostBack=True User=(anonymous)
SideBarButtonClick 1 -> 2
Deactivate twsSecond
Activate wsLast
ActiveStepChanged wizSteps ActiveStepIndex=2
State users: hanako, taro
State wizSteps ActiveStepIndex=2 txtFirst="first step"
State wizDynamic ActiveStepIndex=0
State cuwCreate ActiveStepIndex=1
State User=(anonymous)
```

</details>

<details><summary>LoginView [viewstate] finished: the complete step, the first step&#39;s value kept（期待される本文）</summary>

```text
Status: Login Name:
Anonymous view
Log In
User Name:
Password:
Remember me next time.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
The recovery mail prepared.
The user was created.
Done.
Dynamic: Login
Skip Navigation Links.
Dynamic first
Dynamic last
Request 10: IsPostBack=True User=(anonymous)
FinishButtonClick 2 -> 3 First="first step"
Deactivate wsLast
Activate wsDone
ActiveStepChanged wizSteps ActiveStepIndex=3
State users: hanako, taro
State wizSteps ActiveStepIndex=3 txtFirst="first step"
State wizDynamic ActiveStepIndex=0
State cuwCreate ActiveStepIndex=1
State User=(anonymous)
```

</details>

<details><summary>LoginView [dynamic][viewstate] a wizard made in code: its next step（期待される本文）</summary>

```text
Status: Login Name:
Anonymous view
Log In
User Name:
Password:
Remember me next time.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
The recovery mail prepared.
The user was created.
Done.
Dynamic: Login
Skip Navigation Links.
Dynamic first
Dynamic last
Request 11: IsPostBack=True User=(anonymous)
ActiveStepChanged wizDynamic ActiveStepIndex=1
State users: hanako, taro
State wizSteps ActiveStepIndex=3 txtFirst="first step"
State wizDynamic ActiveStepIndex=1
State cuwCreate ActiveStepIndex=1
State User=(anonymous)
```

</details>

<details><summary>LoginView [visible][viewstate] a hidden wizard shown（期待される本文）</summary>

```text
Status: Login Name:
Anonymous view
Log In
User Name:
Password:
Remember me next time.
Templated login:
Change Your Password
Password:
New Password:
Confirm New Password:
The recovery mail prepared.
The user was created.
Done.
Skip Navigation Links.
Hidden
Hidden wizard
Dynamic: Login
Skip Navigation Links.
Dynamic first
Dynamic last
Request 12: IsPostBack=True User=(anonymous)
Click btnShow
State users: hanako, taro
State wizSteps ActiveStepIndex=3 txtFirst="first step"
State wizDynamic ActiveStepIndex=1
State cuwCreate ActiveStepIndex=1
State User=(anonymous)
```

</details>

