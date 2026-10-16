# ProxyWebPartManager テストケース一覧

変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。
このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。

- 正解データの採取: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0
- ① API 層: 114 ケース（121 メンバー）
- ② HTML 層: 36 ケース
- ③ シナリオ層: 12 スナップショット

## 機構マトリクス

型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。

| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |
|---|---|---|---|
| プロパティ → HTML 描画 | すべてのコントロール | 29 | 1 |
| ViewState の保存と復元 | Control | 6 | 11 |
| Visible=false（描画されず状態は保持） | Control.Visible | 2 | 1 |
| 名前付けコンテナと ClientIDMode | Control.ClientIDMode | 6 | 0 |
| 動的に生成したコントロールのライフサイクル | Control | 0 | 1 |
| データバインド（DataBind / <%# %>） | Control.DataBind | 0 | 1 |

該当しない機構: テーマとスキン（SkinID / EnableTheming）、Enabled=false・Disabled=true（描画とポスト値の扱い）、ポストバックデータの取り込み、変更イベント（*Changed、ServerChange）、ReadOnly（ポストされた値を無視）、リクエスト検証（ValidateRequestMode）、ポストバックイベント、コマンドイベントと親へのバブル（CommandName）、送信ボタン／スクリプトによるポストバック（UseSubmitBehavior）、ブラウザ側のクリック処理（OnClientClick）、クロスページポストバック（PostBackUrl）、既定のボタン（Panel の DefaultButton）、AutoPostBack（クライアントスクリプトによるポストバック）、CausesValidation / ValidationGroup、検証対象（サーバー側の検証）、検証対象（ブラウザ側の検証）、双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値）、項目（ListItem の Selected・Enabled・Text・Value と ViewState）、複数選択、繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）、ポスト値のイベント検証（一覧にない値・無効な項目の拒否）、サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）、ブラウザ側の検証スクリプト（EnableClientScript）、検証結果のまとめ（ValidationSummary）、ページング、並べ替え、項目の挿入、行・項目の編集（編集・更新・取り消し・削除）、行・項目の選択、行・項目のコマンド（RowCommand・ItemCommand へのバブル）、データキー（DataKeyNames・DataKeys）、データソースコントロールとの連携（DataSourceID）、テンプレート、データソース連携（DataSource・DataTextField など）、クライアントコールバック

## ① API 層

ProxyWebPartManager とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の ProxyWebPartManager インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。

| ケース | 期待値（.NET Framework） |
|---|---|
| <code>Api.Control.#ctor</code> | <code>Control.#ctor = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMode=In…</code> |
| <code>Api.Control.Adapter</code> | <code>Control.Adapter = null</code> |
| <code>Api.Control.AddParsedSubObject</code> | <code>Control.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>Control.AddParsedSubObject(Object) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true;…</code> |
| <code>Api.Control.AddedControl</code> | <code>Control.AddedControl(Control,Int32) = &quot;ok&quot;</code><br><code>Control.AddedControl(Control,Int32) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true…</code> |
| <code>Api.Control.AppRelativeTemplateSourceDirectory</code> | <code>Control.AppRelativeTemplateSourceDirectory = &quot;&quot;</code><br><code>Control.AppRelativeTemplateSourceDirectory set = &quot;-&gt; System.ArgumentException: The relative virtual path &#39;Abc 123&#39; is not allowed here.&quot;</code> |
| <code>Api.Control.ApplyStyleSheetSkin</code> | <code>Control.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>Control.ApplyStyleSheetSkin(Page) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; …</code> |
| <code>Api.Control.BeginRenderTracing</code> | <code>Control.BeginRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.BeginRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.BeginRenderTracing(TextWriter,Object) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableView…</code> |
| <code>Api.Control.BindingContainer</code> | <code>Control.BindingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Clien…</code> |
| <code>Api.Control.BuildProfileTree</code> | <code>Control.BuildProfileTree(String,Boolean) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.ChildControlsCreated</code> | <code>Control.ChildControlsCreated = &quot;false&quot;</code><br><code>Control.ChildControlsCreated set = &quot;ok&quot;</code> |
| <code>Api.Control.ClearCachedClientID</code> | <code>Control.ClearCachedClientID = &quot;ok&quot;</code><br><code>Control.ClearCachedClientID after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;pw…</code> |
| <code>Api.Control.ClearChildControlState</code> | <code>Control.ClearChildControlState = &quot;ok&quot;</code><br><code>Control.ClearChildControlState after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=…</code> |
| <code>Api.Control.ClearChildState</code> | <code>Control.ClearChildState = &quot;ok&quot;</code><br><code>Control.ClearChildState after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;pwpmPr…</code> |
| <code>Api.Control.ClearChildViewState</code> | <code>Control.ClearChildViewState = &quot;ok&quot;</code><br><code>Control.ClearChildViewState after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;pw…</code> |
| <code>Api.Control.ClearEffectiveClientIDMode</code> | <code>Control.ClearEffectiveClientIDMode = &quot;ok&quot;</code><br><code>Control.ClearEffectiveClientIDMode after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true;…</code> |
| <code>Api.Control.ClientID</code> | <code>Control.ClientID = &quot;pwpmProbe&quot;</code> |
| <code>Api.Control.ClientIDMode</code> | <code>Control.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>Control.ClientIDMode set = &quot;ClientIDMode.AutoID&quot;</code> |
| <code>Api.Control.ClientIDSeparator</code> | <code>Control.ClientIDSeparator = &quot;&#39;_&#39;&quot;</code> |
| <code>Api.Control.Context</code> | <code>Control.Context = null</code> |
| <code>Api.Control.Controls</code> | <code>Control.Controls = ControlCollection []</code> |
| <code>Api.Control.CreateChildControls</code> | <code>Control.CreateChildControls = &quot;ok&quot;</code><br><code>Control.CreateChildControls after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;pw…</code> |
| <code>Api.Control.CreateControlCollection</code> | <code>Control.CreateControlCollection = ControlCollection []</code><br><code>Control.CreateControlCollection after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID…</code> |
| <code>Api.Control.DataBind</code> | <code>Control.DataBind = &quot;ok&quot;</code><br><code>Control.DataBind after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;pwpmProbe&quot;; S…</code><br><code>Control.DataBind(Boolean) = &quot;ok&quot;</code><br><code>Control.DataBind(Boolean) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;pwpm…</code> |
| <code>Api.Control.DataBindChildren</code> | <code>Control.DataBindChildren = &quot;ok&quot;</code><br><code>Control.DataBindChildren after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;pwpmP…</code> |
| <code>Api.Control.DataBinding</code> | <code>Control.DataBinding = &quot;added and removed&quot;</code> |
| <code>Api.Control.DataItemContainer</code> | <code>Control.DataItemContainer = null</code> |
| <code>Api.Control.DataKeysContainer</code> | <code>Control.DataKeysContainer = null</code> |
| <code>Api.Control.DesignMode</code> | <code>Control.DesignMode = &quot;false&quot;</code> |
| <code>Api.Control.Dispose</code> | <code>Control.Dispose = &quot;ok&quot;</code><br><code>Control.Dispose after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;pwpmProbe&quot;; Sk…</code> |
| <code>Api.Control.Disposed</code> | <code>Control.Disposed = &quot;added and removed&quot;</code> |
| <code>Api.Control.EnableTheming</code> | <code>Control.EnableTheming = &quot;false&quot;</code><br><code>Control.EnableTheming set = &quot;-&gt; System.NotSupportedException: Control of type &#39;ProxyWebPartManager&#39; does not support theming.&quot;</code> |
| <code>Api.Control.EnableViewState</code> | <code>Control.EnableViewState = &quot;true&quot;</code><br><code>Control.EnableViewState set = &quot;true&quot;</code> |
| <code>Api.Control.EndRenderTracing</code> | <code>Control.EndRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.EndRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.EndRenderTracing(TextWriter,Object) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewSt…</code> |
| <code>Api.Control.EnsureChildControls</code> | <code>Control.EnsureChildControls = &quot;ok&quot;</code><br><code>Control.EnsureChildControls after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;pw…</code> |
| <code>Api.Control.EnsureID</code> | <code>Control.EnsureID = &quot;ok&quot;</code><br><code>Control.EnsureID after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;pwpmProbe&quot;; S…</code> |
| <code>Api.Control.Events</code> | <code>Control.Events = &quot;System.ComponentModel.EventHandlerList&quot;</code> |
| <code>Api.Control.FindControl</code> | <code>Control.FindControl(String) = Label#lblTitle {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code><br><code>Control.FindControl(String) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;pw…</code><br><code>Control.FindControl(String,Int32) = null</code><br><code>Control.FindControl(String,Int32) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; …</code> |
| <code>Api.Control.Focus</code> | <code>Control.Focus = &quot;-&gt; System.NotSupportedException: Control of type &#39;ProxyWebPartManager&#39; does not support the Focus operation.&quot;</code> |
| <code>Api.Control.GetDesignModeState</code> | <code>Control.GetDesignModeState = &quot;[]&quot;</code><br><code>Control.GetDesignModeState after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;pwp…</code> |
| <code>Api.Control.GetRouteUrl</code> | <code>Control.GetRouteUrl(Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.GetUniqueIDRelativeTo</code> | <code>Control.GetUniqueIDRelativeTo(Control) = &quot;pwpmProbe&quot;</code><br><code>Control.GetUniqueIDRelativeTo(Control) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=t…</code> |
| <code>Api.Control.HasChildViewState</code> | <code>Control.HasChildViewState = &quot;false&quot;</code> |
| <code>Api.Control.HasControls</code> | <code>Control.HasControls = &quot;false&quot;</code><br><code>Control.HasControls after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;pwpmProbe&quot;…</code> |
| <code>Api.Control.HasEvents</code> | <code>Control.HasEvents = &quot;false&quot;</code><br><code>Control.HasEvents after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;pwpmProbe&quot;; …</code> |
| <code>Api.Control.ID</code> | <code>Control.ID = &quot;pwpmProbe&quot;</code><br><code>Control.ID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.IdSeparator</code> | <code>Control.IdSeparator = &quot;&#39;$&#39;&quot;</code> |
| <code>Api.Control.Init</code> | <code>Control.Init = &quot;added and removed&quot;</code> |
| <code>Api.Control.IsChildControlStateCleared</code> | <code>Control.IsChildControlStateCleared = &quot;false&quot;</code> |
| <code>Api.Control.IsLiteralContent</code> | <code>Control.IsLiteralContent = &quot;false&quot;</code><br><code>Control.IsLiteralContent after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;pwpmP…</code> |
| <code>Api.Control.IsTrackingViewState</code> | <code>Control.IsTrackingViewState = &quot;false&quot;</code> |
| <code>Api.Control.IsViewStateEnabled</code> | <code>Control.IsViewStateEnabled = &quot;true&quot;</code> |
| <code>Api.Control.Load</code> | <code>Control.Load = &quot;added and removed&quot;</code> |
| <code>Api.Control.LoadControlState</code> | <code>Control.LoadControlState(Object) = &quot;ok&quot;</code><br><code>Control.LoadControlState(Object) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; I…</code> |
| <code>Api.Control.LoadViewState</code> | <code>Control.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Collections.ArrayList&#39;.&quot;</code> |
| <code>Api.Control.LoadViewStateByID</code> | <code>Control.LoadViewStateByID = &quot;false&quot;</code> |
| <code>Api.Control.MapPathSecure</code> | <code>Control.MapPathSecure(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.NamingContainer</code> | <code>Control.NamingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code> |
| <code>Api.Control.OnBubbleEvent</code> | <code>Control.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Control.OnBubbleEvent(Object,EventArgs) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=…</code> |
| <code>Api.Control.OnDataBinding</code> | <code>Control.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnDataBinding(EventArgs) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; I…</code> |
| <code>Api.Control.OnInit</code> | <code>Control.OnInit(EventArgs) = &quot;-&gt; System.InvalidOperationException: You must enable Web Parts by adding a WebPartManager to your page.  The WebPartManager must be…</code> |
| <code>Api.Control.OnLoad</code> | <code>Control.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnLoad(EventArgs) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;pwpm…</code> |
| <code>Api.Control.OnPreRender</code> | <code>Control.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnPreRender(EventArgs) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=…</code> |
| <code>Api.Control.OnUnload</code> | <code>Control.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnUnload(EventArgs) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;pw…</code> |
| <code>Api.Control.OpenFile</code> | <code>Control.OpenFile(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.Page</code> | <code>Control.Page = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=&quot;&quot;; …</code><br><code>Control.Page set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=…</code> |
| <code>Api.Control.Parent</code> | <code>Control.Parent = PlaceHolder# {ClientID=&quot;ctl00&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;ctl00&quot;; SkinID=&quot;&quot;; UniqueID=&quot;ctl00&quot;; Valida…</code> |
| <code>Api.Control.PreRender</code> | <code>Control.PreRender = &quot;added and removed&quot;</code> |
| <code>Api.Control.RaiseBubbleEvent</code> | <code>Control.RaiseBubbleEvent(Object,EventArgs) = &quot;ok&quot;</code><br><code>Control.RaiseBubbleEvent(Object,EventArgs) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewSta…</code> |
| <code>Api.Control.RemovedControl</code> | <code>Control.RemovedControl(Control) = &quot;ok&quot;</code><br><code>Control.RemovedControl(Control) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID…</code> |
| <code>Api.Control.Render</code> | <code>Control.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.Render(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.Render(HtmlTextWriter) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=…</code> |
| <code>Api.Control.RenderChildren</code> | <code>Control.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderChildren(HtmlTextWriter) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=t…</code> |
| <code>Api.Control.RenderControl</code> | <code>Control.RenderControl(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderControl(HtmlTextWriter) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=tr…</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=f…</code> |
| <code>Api.Control.RenderingCompatibility</code> | <code>Control.RenderingCompatibility = &quot;System.Version 4.0&quot;</code><br><code>Control.RenderingCompatibility set = &quot;System.Version 4.0&quot;</code> |
| <code>Api.Control.ResolveAdapter</code> | <code>Control.ResolveAdapter = null</code><br><code>Control.ResolveAdapter after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;pwpmPro…</code> |
| <code>Api.Control.ResolveClientUrl</code> | <code>Control.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveClientUrl(String) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; I…</code> |
| <code>Api.Control.ResolveUrl</code> | <code>Control.ResolveUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveUrl(String) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;pwp…</code> |
| <code>Api.Control.SaveControlState</code> | <code>Control.SaveControlState = null</code><br><code>Control.SaveControlState after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;pwpmP…</code> |
| <code>Api.Control.SaveViewState</code> | <code>Control.SaveViewState = null</code><br><code>Control.SaveViewState after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;pwpmProb…</code> |
| <code>Api.Control.SetDesignModeState</code> | <code>Control.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>Control.SetDesignModeState(IDictionary) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=…</code> |
| <code>Api.Control.SetRenderMethodDelegate</code> | <code>Control.SetRenderMethodDelegate(RenderMethod) = &quot;ok&quot;</code><br><code>Control.SetRenderMethodDelegate(RenderMethod) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableView…</code> |
| <code>Api.Control.SetTraceData</code> | <code>Control.SetTraceData(Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true…</code><br><code>Control.SetTraceData(Object,Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object,Object) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewSta…</code> |
| <code>Api.Control.Site</code> | <code>Control.Site = null</code><br><code>Control.Site set = null</code> |
| <code>Api.Control.SkinID</code> | <code>Control.SkinID = &quot;&quot;</code><br><code>Control.SkinID set = &quot;-&gt; System.NotSupportedException: Control of type &#39;ProxyWebPartManager&#39; does not support theming.&quot;</code> |
| <code>Api.Control.TemplateControl</code> | <code>Control.TemplateControl = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code><br><code>Control.TemplateControl set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Cl…</code> |
| <code>Api.Control.TemplateSourceDirectory</code> | <code>Control.TemplateSourceDirectory = &quot;&quot;</code> |
| <code>Api.Control.TrackViewState</code> | <code>Control.TrackViewState = &quot;ok&quot;</code><br><code>Control.TrackViewState after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;pwpmPro…</code> |
| <code>Api.Control.UniqueID</code> | <code>Control.UniqueID = &quot;pwpmProbe&quot;</code> |
| <code>Api.Control.Unload</code> | <code>Control.Unload = &quot;added and removed&quot;</code> |
| <code>Api.Control.ValidateRequestMode</code> | <code>Control.ValidateRequestMode = &quot;ValidateRequestMode.Inherit&quot;</code><br><code>Control.ValidateRequestMode set = &quot;ValidateRequestMode.Disabled&quot;</code> |
| <code>Api.Control.ViewState</code> | <code>Control.ViewState = StateBag []</code> |
| <code>Api.Control.ViewStateIgnoresCase</code> | <code>Control.ViewStateIgnoresCase = &quot;false&quot;</code> |
| <code>Api.Control.ViewStateMode</code> | <code>Control.ViewStateMode = &quot;ViewStateMode.Inherit&quot;</code><br><code>Control.ViewStateMode set = &quot;ViewStateMode.Enabled&quot;</code> |
| <code>Api.Control.Visible</code> | <code>Control.Visible = &quot;false&quot;</code><br><code>Control.Visible set = &quot;-&gt; System.NotSupportedException: Control of type &#39;ProxyWebPartManager&#39; is a non-visual control and does not support setting the Visible p…</code> |
| <code>Api.IControlBuilderAccessor.ControlBuilder</code> | <code>IControlBuilderAccessor.ControlBuilder = null</code> |
| <code>Api.IControlDesignerAccessor.GetDesignModeState</code> | <code>IControlDesignerAccessor.GetDesignModeState = &quot;[]&quot;</code><br><code>IControlDesignerAccessor.GetDesignModeState after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewSt…</code> |
| <code>Api.IControlDesignerAccessor.SetDesignModeState</code> | <code>IControlDesignerAccessor.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetDesignModeState(IDictionary) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false;…</code> |
| <code>Api.IControlDesignerAccessor.SetOwnerControl</code> | <code>IControlDesignerAccessor.SetOwnerControl(Control) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetOwnerControl(Control) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; Enable…</code> |
| <code>Api.IControlDesignerAccessor.UserData</code> | <code>IControlDesignerAccessor.UserData = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.DataBindings</code> | <code>IDataBindingsAccessor.DataBindings = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.HasDataBindings</code> | <code>IDataBindingsAccessor.HasDataBindings = &quot;false&quot;</code> |
| <code>Api.IExpressionsAccessor.Expressions</code> | <code>IExpressionsAccessor.Expressions = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.HasExpressions</code> | <code>IExpressionsAccessor.HasExpressions = &quot;false&quot;</code> |
| <code>Api.IParserAccessor.AddParsedSubObject</code> | <code>IParserAccessor.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>IParserAccessor.AddParsedSubObject(Object) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewSta…</code> |
| <code>Api.IUrlResolutionService.ResolveClientUrl</code> | <code>IUrlResolutionService.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>IUrlResolutionService.ResolveClientUrl(String) after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableVie…</code> |
| <code>Api.WebControls.WebParts.ProxyWebPartManager.#ctor</code> | <code>WebParts.ProxyWebPartManager.#ctor = ProxyWebPartManager# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=null; SkinID=&quot;&quot;; Uni…</code> |
| <code>Api.WebControls.WebParts.ProxyWebPartManager.ClientID</code> | <code>WebParts.ProxyWebPartManager.ClientID = &quot;pwpmProbe&quot;</code> |
| <code>Api.WebControls.WebParts.ProxyWebPartManager.Controls</code> | <code>WebParts.ProxyWebPartManager.Controls = ControlCollection []</code> |
| <code>Api.WebControls.WebParts.ProxyWebPartManager.CreateControlCollection</code> | <code>WebParts.ProxyWebPartManager.CreateControlCollection = ControlCollection []</code><br><code>WebParts.ProxyWebPartManager.CreateControlCollection after = ProxyWebPartManager#pwpmProbe {ClientID=&quot;pwpmProbe&quot;; ClientIDMode=Inherit; EnableTheming=false; Ena…</code> |
| <code>Api.WebControls.WebParts.ProxyWebPartManager.EnableTheming</code> | <code>WebParts.ProxyWebPartManager.EnableTheming = &quot;false&quot;</code><br><code>WebParts.ProxyWebPartManager.EnableTheming set = &quot;-&gt; System.NotSupportedException: Control of type &#39;ProxyWebPartManager&#39; does not support theming.&quot;</code> |
| <code>Api.WebControls.WebParts.ProxyWebPartManager.Focus</code> | <code>WebParts.ProxyWebPartManager.Focus = &quot;-&gt; System.NotSupportedException: Control of type &#39;ProxyWebPartManager&#39; does not support the Focus operation.&quot;</code> |
| <code>Api.WebControls.WebParts.ProxyWebPartManager.OnInit</code> | <code>WebParts.ProxyWebPartManager.OnInit(EventArgs) = &quot;-&gt; System.InvalidOperationException: You must enable Web Parts by adding a WebPartManager to your page.  The W…</code> |
| <code>Api.WebControls.WebParts.ProxyWebPartManager.SkinID</code> | <code>WebParts.ProxyWebPartManager.SkinID = &quot;&quot;</code><br><code>WebParts.ProxyWebPartManager.SkinID set = &quot;-&gt; System.NotSupportedException: Control of type &#39;ProxyWebPartManager&#39; does not support theming.&quot;</code> |
| <code>Api.WebControls.WebParts.ProxyWebPartManager.StaticConnections</code> | <code>WebParts.ProxyWebPartManager.StaticConnections = &quot;[]&quot;</code> |
| <code>Api.WebControls.WebParts.ProxyWebPartManager.Visible</code> | <code>WebParts.ProxyWebPartManager.Visible = &quot;false&quot;</code><br><code>WebParts.ProxyWebPartManager.Visible set = &quot;-&gt; System.NotSupportedException: Control of type &#39;ProxyWebPartManager&#39; is a non-visual control and does not support …</code> |

## ② HTML 層

プローブサイトの RenderProbe.aspx で、1 ケース 1 リクエスト（ページライフサイクル全体）で描画したページ全体を比較します。下表はコンポーネント部分（div#probe）、設定時の例外、保存された ViewState の要約です。

| ケース | 設定 | 期待される描画 | 設定時の例外 |
|---|---|---|---|
| <code>.(default)</code> | <code>(nothing set)</code> | <code></code> |  |
| <code>.AppRelativeTemplateSourceDirectory=text</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;;</code> | <code></code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;probe&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=markup</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=japanese</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;;</code> | <code></code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;日本語テキスト&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=empty</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&quot;;</code> | <code></code> |  |
| <code>.ClientIDMode=Inherit</code> | <code>c.ClientIDMode = ClientIDMode.Inherit;</code> | <code></code> |  |
| <code>.ClientIDMode=AutoID</code> | <code>c.ClientIDMode = ClientIDMode.AutoID;</code> | <code></code> |  |
| <code>.ClientIDMode=Predictable</code> | <code>c.ClientIDMode = ClientIDMode.Predictable;</code> | <code></code> |  |
| <code>.ClientIDMode=Static</code> | <code>c.ClientIDMode = ClientIDMode.Static;</code> | <code></code> |  |
| <code>.EnableTheming=true</code> | <code>c.EnableTheming = true;</code> | <code></code> | <code>c.EnableTheming = true; -&gt; System.NotSupportedException: Control of type &#39;ProxyWebPartManager&#39; does not support theming.</code> |
| <code>.EnableTheming=false</code> | <code>c.EnableTheming = false;</code> | <code></code> | <code>c.EnableTheming = false; -&gt; System.NotSupportedException: Control of type &#39;ProxyWebPartManager&#39; does not support theming.</code> |
| <code>.EnableViewState=true</code> | <code>c.EnableViewState = true;</code> | <code></code> |  |
| <code>.EnableViewState=false</code> | <code>c.EnableViewState = false;</code> | <code></code> |  |
| <code>.ID=text</code> | <code>c.ID = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.ID=markup</code> | <code>c.ID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.ID=japanese</code> | <code>c.ID = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.ID=empty</code> | <code>c.ID = &quot;&quot;;</code> | <code></code> |  |
| <code>.SkinID=text</code> | <code>c.SkinID = &quot;probe&quot;;</code> | <code></code> | <code>c.SkinID = &quot;probe&quot;; -&gt; System.NotSupportedException: Control of type &#39;ProxyWebPartManager&#39; does not support theming.</code> |
| <code>.SkinID=markup</code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.NotSupportedException: Control of type &#39;ProxyWebPartManager&#39; does not support theming.</code> |
| <code>.SkinID=japanese</code> | <code>c.SkinID = &quot;日本語テキスト&quot;;</code> | <code></code> | <code>c.SkinID = &quot;日本語テキスト&quot;; -&gt; System.NotSupportedException: Control of type &#39;ProxyWebPartManager&#39; does not support theming.</code> |
| <code>.SkinID=empty</code> | <code>c.SkinID = &quot;&quot;;</code> | <code></code> | <code>c.SkinID = &quot;&quot;; -&gt; System.NotSupportedException: Control of type &#39;ProxyWebPartManager&#39; does not support theming.</code> |
| <code>.ValidateRequestMode=Inherit</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Inherit;</code> | <code></code> |  |
| <code>.ValidateRequestMode=Disabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Disabled;</code> | <code></code> |  |
| <code>.ValidateRequestMode=Enabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Enabled;</code> | <code></code> |  |
| <code>.ViewStateMode=Inherit</code> | <code>c.ViewStateMode = ViewStateMode.Inherit;</code> | <code></code> |  |
| <code>.ViewStateMode=Enabled</code> | <code>c.ViewStateMode = ViewStateMode.Enabled;</code> | <code></code> |  |
| <code>.ViewStateMode=Disabled</code> | <code>c.ViewStateMode = ViewStateMode.Disabled;</code> | <code></code> |  |
| <code>.Visible=true</code> | <code>c.Visible = true;</code> | <code></code> | <code>c.Visible = true; -&gt; System.NotSupportedException: Control of type &#39;ProxyWebPartManager&#39; is a non-visual control and does not support setting the Visible property.</code> |
| <code>.Visible=false</code> | <code>c.Visible = false;</code> | <code></code> | <code>c.Visible = false; -&gt; System.NotSupportedException: Control of type &#39;ProxyWebPartManager&#39; is a non-visual control and does not support setting the Visible property.</code> |
| <code>.Visible=false+EnableViewState=false</code> | <code>c.Visible = false; c.EnableViewState = false;</code> | <code></code> | <code>c.Visible = false; -&gt; System.NotSupportedException: Control of type &#39;ProxyWebPartManager&#39; is a non-visual control and does not support setting the Visible property.</code> |
| <code>[naming container].(default)</code> | <code>[naming container] (nothing set)</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=AutoID</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Static</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Predictable</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Inherit</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ID=text</code> | <code>[naming container] c.ID = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |

## ③ シナリオ層

ブラウザ（Playwright）で ProxyWebPartManagerScenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。

| # | 操作 | 対象 | 値 |
|---|---|---|---|
| 1 | goto | /WebPartsScenario.aspx?reset=1 | <code></code> |
| 2 | snapshot |  | **ProxyWebPartManager [render][databind][dynamic] first request (browse): the parts in their zones, a zone made in code** |
| 3 | click | btnDesign | <code></code> |
| 4 | snapshot |  | **ProxyWebPartManager [viewstate] the design mode** |
| 5 | click | btnCatalog | <code></code> |
| 6 | snapshot |  | **ProxyWebPartManager [viewstate] the catalog mode: the catalog zone and its parts** |
| 7 | evaluate | czCatalog | <code>e.querySelector(&#39;input[type=checkbox]&#39;).checked = true;</code> |
| 8 | clicktextin | czCatalog | <code>Add</code> |
| 9 | snapshot |  | **ProxyWebPartManager [viewstate] a part added from the catalog (its check box posted, the zone&#39;s Add)** |
| 10 | click | btnEditPart | <code></code> |
| 11 | snapshot |  | **ProxyWebPartManager [viewstate] the edit mode: the editor zone and its parts for the first part** |
| 12 | evaluate | ezEditor | <code>e.querySelector(&#39;input[type=text]&#39;).value = &#39;Renamed part&#39;;</code> |
| 13 | clicktextin | ezEditor | <code>OK</code> |
| 14 | snapshot |  | **ProxyWebPartManager [viewstate] the first part&#39;s title edited (the editor zone&#39;s OK)** |
| 15 | click | btnConnectPart | <code></code> |
| 16 | snapshot |  | **ProxyWebPartManager [viewstate] the connect mode for the first part** |
| 17 | clicktextin | cnzConnections | <code>Close</code> |
| 18 | snapshot |  | **ProxyWebPartManager [viewstate] the connections zone closed** |
| 19 | click | btnBrowse | <code></code> |
| 20 | clicktextin | wpzMain | <code>Minimize</code> |
| 21 | snapshot |  | **ProxyWebPartManager [viewstate] a part minimized by its zone&#39;s verb** |
| 22 | clicktextin | wpzMain | <code>Restore</code> |
| 23 | snapshot |  | **ProxyWebPartManager [viewstate] restored** |
| 24 | clicktextin | wpzDynamic | <code>Minimize</code> |
| 25 | snapshot |  | **ProxyWebPartManager [viewstate] a part of the zone made in code minimized** |
| 26 | click | btnShow | <code></code> |
| 27 | snapshot |  | **ProxyWebPartManager [visible][viewstate] a hidden zone shown** |

<details><summary>ProxyWebPartManager [render][databind][dynamic] first request (browse): the parts in their zones, a zone made in code（期待される本文）</summary>

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

<details><summary>ProxyWebPartManager [viewstate] the design mode（期待される本文）</summary>

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

<details><summary>ProxyWebPartManager [viewstate] the catalog mode: the catalog zone and its parts（期待される本文）</summary>

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

<details><summary>ProxyWebPartManager [viewstate] a part added from the catalog (its check box posted, the zone&#39;s Add)（期待される本文）</summary>

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

<details><summary>ProxyWebPartManager [viewstate] the edit mode: the editor zone and its parts for the first part（期待される本文）</summary>

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

<details><summary>ProxyWebPartManager [viewstate] the first part&#39;s title edited (the editor zone&#39;s OK)（期待される本文）</summary>

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

<details><summary>ProxyWebPartManager [viewstate] the connect mode for the first part（期待される本文）</summary>

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

<details><summary>ProxyWebPartManager [viewstate] the connections zone closed（期待される本文）</summary>

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

<details><summary>ProxyWebPartManager [viewstate] a part minimized by its zone&#39;s verb（期待される本文）</summary>

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

<details><summary>ProxyWebPartManager [viewstate] restored（期待される本文）</summary>

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

<details><summary>ProxyWebPartManager [viewstate] a part of the zone made in code minimized（期待される本文）</summary>

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

<details><summary>ProxyWebPartManager [visible][viewstate] a hidden zone shown（期待される本文）</summary>

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

