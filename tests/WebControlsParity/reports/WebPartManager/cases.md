# WebPartManager テストケース一覧

変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。
このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。

- 正解データの採取: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0
- ① API 層: 219 ケース（229 メンバー）
- ② HTML 層: 52 ケース
- ③ シナリオ層: 12 スナップショット

## 機構マトリクス

型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。

| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |
|---|---|---|---|
| プロパティ → HTML 描画 | すべてのコントロール | 45 | 1 |
| ViewState の保存と復元 | Control | 6 | 10 |
| Visible=false（描画されず状態は保持） | Control.Visible | 2 | 1 |
| 名前付けコンテナと ClientIDMode | Control.ClientIDMode | 6 | 0 |
| 動的に生成したコントロールのライフサイクル | Control | 0 | 1 |
| データバインド（DataBind / <%# %>） | Control.DataBind | 0 | 1 |
| ブラウザ側の検証スクリプト（EnableClientScript） | EnableClientScript プロパティ | 3 | 0 |

該当しない機構: テーマとスキン（SkinID / EnableTheming）、Enabled=false・Disabled=true（描画とポスト値の扱い）、ポストバックデータの取り込み、変更イベント（*Changed、ServerChange）、ReadOnly（ポストされた値を無視）、リクエスト検証（ValidateRequestMode）、ポストバックイベント、コマンドイベントと親へのバブル（CommandName）、送信ボタン／スクリプトによるポストバック（UseSubmitBehavior）、ブラウザ側のクリック処理（OnClientClick）、クロスページポストバック（PostBackUrl）、既定のボタン（Panel の DefaultButton）、AutoPostBack（クライアントスクリプトによるポストバック）、CausesValidation / ValidationGroup、検証対象（サーバー側の検証）、検証対象（ブラウザ側の検証）、双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値）、項目（ListItem の Selected・Enabled・Text・Value と ViewState）、複数選択、繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）、ポスト値のイベント検証（一覧にない値・無効な項目の拒否）、サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）、検証結果のまとめ（ValidationSummary）、ページング、並べ替え、項目の挿入、行・項目の編集（編集・更新・取り消し・削除）、行・項目の選択、行・項目のコマンド（RowCommand・ItemCommand へのバブル）、データキー（DataKeyNames・DataKeys）、データソースコントロールとの連携（DataSourceID）、テンプレート、データソース連携（DataSource・DataTextField など）、クライアントコールバック

## ① API 層

WebPartManager とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の WebPartManager インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。

| ケース | 期待値（.NET Framework） |
|---|---|
| <code>Api.Control.#ctor</code> | <code>Control.#ctor = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMode=In…</code> |
| <code>Api.Control.Adapter</code> | <code>Control.Adapter = null</code> |
| <code>Api.Control.AddParsedSubObject</code> | <code>Control.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>Control.AddParsedSubObject(Object) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this…</code> |
| <code>Api.Control.AddedControl</code> | <code>Control.AddedControl(Control,Int32) = &quot;ok&quot;</code><br><code>Control.AddedControl(Control,Int32) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close thi…</code> |
| <code>Api.Control.AppRelativeTemplateSourceDirectory</code> | <code>Control.AppRelativeTemplateSourceDirectory = &quot;&quot;</code><br><code>Control.AppRelativeTemplateSourceDirectory set = &quot;-&gt; System.ArgumentException: The relative virtual path &#39;Abc 123&#39; is not allowed here.&quot;</code> |
| <code>Api.Control.ApplyStyleSheetSkin</code> | <code>Control.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>Control.ApplyStyleSheetSkin(Page) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this …</code> |
| <code>Api.Control.BeginRenderTracing</code> | <code>Control.BeginRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.BeginRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.BeginRenderTracing(TextWriter,Object) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to…</code> |
| <code>Api.Control.BindingContainer</code> | <code>Control.BindingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Clien…</code> |
| <code>Api.Control.BuildProfileTree</code> | <code>Control.BuildProfileTree(String,Boolean) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.ChildControlsCreated</code> | <code>Control.ChildControlsCreated = &quot;false&quot;</code><br><code>Control.ChildControlsCreated set = &quot;ok&quot;</code> |
| <code>Api.Control.ClearCachedClientID</code> | <code>Control.ClearCachedClientID = &quot;ok&quot;</code><br><code>Control.ClearCachedClientID after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this Web Pa…</code> |
| <code>Api.Control.ClearChildControlState</code> | <code>Control.ClearChildControlState = &quot;ok&quot;</code><br><code>Control.ClearChildControlState after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this Web…</code> |
| <code>Api.Control.ClearChildState</code> | <code>Control.ClearChildState = &quot;ok&quot;</code><br><code>Control.ClearChildState after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this Web Part. …</code> |
| <code>Api.Control.ClearChildViewState</code> | <code>Control.ClearChildViewState = &quot;ok&quot;</code><br><code>Control.ClearChildViewState after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this Web Pa…</code> |
| <code>Api.Control.ClearEffectiveClientIDMode</code> | <code>Control.ClearEffectiveClientIDMode = &quot;ok&quot;</code><br><code>Control.ClearEffectiveClientIDMode after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this…</code> |
| <code>Api.Control.ClientID</code> | <code>Control.ClientID = &quot;wpmProbe&quot;</code> |
| <code>Api.Control.ClientIDMode</code> | <code>Control.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>Control.ClientIDMode set = &quot;ClientIDMode.AutoID&quot;</code> |
| <code>Api.Control.ClientIDSeparator</code> | <code>Control.ClientIDSeparator = &quot;&#39;_&#39;&quot;</code> |
| <code>Api.Control.Context</code> | <code>Control.Context = null</code> |
| <code>Api.Control.Controls</code> | <code>Control.Controls = ControlCollection []</code> |
| <code>Api.Control.CreateChildControls</code> | <code>Control.CreateChildControls = &quot;ok&quot;</code><br><code>Control.CreateChildControls after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this Web Pa…</code> |
| <code>Api.Control.CreateControlCollection</code> | <code>Control.CreateControlCollection = ControlCollection []</code><br><code>Control.CreateControlCollection after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this We…</code> |
| <code>Api.Control.DataBind</code> | <code>Control.DataBind = &quot;ok&quot;</code><br><code>Control.DataBind after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this Web Part.  It is …</code><br><code>Control.DataBind(Boolean) = &quot;ok&quot;</code><br><code>Control.DataBind(Boolean) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this Web Part…</code> |
| <code>Api.Control.DataBindChildren</code> | <code>Control.DataBindChildren = &quot;ok&quot;</code><br><code>Control.DataBindChildren after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this Web Part.…</code> |
| <code>Api.Control.DataBinding</code> | <code>Control.DataBinding = &quot;added and removed&quot;</code> |
| <code>Api.Control.DataItemContainer</code> | <code>Control.DataItemContainer = null</code> |
| <code>Api.Control.DataKeysContainer</code> | <code>Control.DataKeysContainer = null</code> |
| <code>Api.Control.DesignMode</code> | <code>Control.DesignMode = &quot;false&quot;</code> |
| <code>Api.Control.Dispose</code> | <code>Control.Dispose = &quot;ok&quot;</code><br><code>Control.Dispose after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this Web Part.  It is c…</code> |
| <code>Api.Control.Disposed</code> | <code>Control.Disposed = &quot;added and removed&quot;</code> |
| <code>Api.Control.EnableTheming</code> | <code>Control.EnableTheming = &quot;true&quot;</code><br><code>Control.EnableTheming set = &quot;-&gt; System.NotSupportedException: Cannot set the EnableTheming property on WebPartManager.  EnableTheming must be true for the WebPa…</code> |
| <code>Api.Control.EnableViewState</code> | <code>Control.EnableViewState = &quot;true&quot;</code><br><code>Control.EnableViewState set = &quot;true&quot;</code> |
| <code>Api.Control.EndRenderTracing</code> | <code>Control.EndRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.EndRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.EndRenderTracing(TextWriter,Object) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to c…</code> |
| <code>Api.Control.EnsureChildControls</code> | <code>Control.EnsureChildControls = &quot;ok&quot;</code><br><code>Control.EnsureChildControls after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this Web Pa…</code> |
| <code>Api.Control.EnsureID</code> | <code>Control.EnsureID = &quot;ok&quot;</code><br><code>Control.EnsureID after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this Web Part.  It is …</code> |
| <code>Api.Control.Events</code> | <code>Control.Events = &quot;System.ComponentModel.EventHandlerList&quot;</code> |
| <code>Api.Control.FindControl</code> | <code>Control.FindControl(String) = null</code><br><code>Control.FindControl(String) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this Web Pa…</code><br><code>Control.FindControl(String,Int32) = null</code><br><code>Control.FindControl(String,Int32) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this …</code> |
| <code>Api.Control.Focus</code> | <code>Control.Focus = &quot;-&gt; System.NotSupportedException: Control of type &#39;WebPartManager&#39; does not support the Focus operation.&quot;</code> |
| <code>Api.Control.GetDesignModeState</code> | <code>Control.GetDesignModeState = &quot;[]&quot;</code><br><code>Control.GetDesignModeState after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this Web Par…</code> |
| <code>Api.Control.GetRouteUrl</code> | <code>Control.GetRouteUrl(Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.GetUniqueIDRelativeTo</code> | <code>Control.GetUniqueIDRelativeTo(Control) = &quot;wpmProbe&quot;</code><br><code>Control.GetUniqueIDRelativeTo(Control) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close …</code> |
| <code>Api.Control.HasChildViewState</code> | <code>Control.HasChildViewState = &quot;false&quot;</code> |
| <code>Api.Control.HasControls</code> | <code>Control.HasControls = &quot;false&quot;</code><br><code>Control.HasControls after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this Web Part.  It …</code> |
| <code>Api.Control.HasEvents</code> | <code>Control.HasEvents = &quot;false&quot;</code><br><code>Control.HasEvents after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this Web Part.  It is…</code> |
| <code>Api.Control.ID</code> | <code>Control.ID = &quot;wpmProbe&quot;</code><br><code>Control.ID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.IdSeparator</code> | <code>Control.IdSeparator = &quot;&#39;$&#39;&quot;</code> |
| <code>Api.Control.Init</code> | <code>Control.Init = &quot;added and removed&quot;</code> |
| <code>Api.Control.IsChildControlStateCleared</code> | <code>Control.IsChildControlStateCleared = &quot;false&quot;</code> |
| <code>Api.Control.IsLiteralContent</code> | <code>Control.IsLiteralContent = &quot;false&quot;</code><br><code>Control.IsLiteralContent after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this Web Part.…</code> |
| <code>Api.Control.IsTrackingViewState</code> | <code>Control.IsTrackingViewState = &quot;false&quot;</code> |
| <code>Api.Control.IsViewStateEnabled</code> | <code>Control.IsViewStateEnabled = &quot;true&quot;</code> |
| <code>Api.Control.Load</code> | <code>Control.Load = &quot;added and removed&quot;</code> |
| <code>Api.Control.LoadControlState</code> | <code>Control.LoadControlState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Object[]&#39;.&quot;</code> |
| <code>Api.Control.LoadViewState</code> | <code>Control.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Collections.ArrayList&#39;.&quot;</code> |
| <code>Api.Control.LoadViewStateByID</code> | <code>Control.LoadViewStateByID = &quot;true&quot;</code> |
| <code>Api.Control.MapPathSecure</code> | <code>Control.MapPathSecure(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.NamingContainer</code> | <code>Control.NamingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code> |
| <code>Api.Control.OnBubbleEvent</code> | <code>Control.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Control.OnBubbleEvent(Object,EventArgs) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close…</code> |
| <code>Api.Control.OnDataBinding</code> | <code>Control.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnDataBinding(EventArgs) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this W…</code> |
| <code>Api.Control.OnInit</code> | <code>Control.OnInit(EventArgs) = &quot;-&gt; System.InvalidOperationException: Value of the &#39;WebPartManager.Page.Request&#39; property cannot be null.&quot;</code> |
| <code>Api.Control.OnLoad</code> | <code>Control.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnLoad(EventArgs) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this Web Part…</code> |
| <code>Api.Control.OnPreRender</code> | <code>Control.OnPreRender(EventArgs) = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code> |
| <code>Api.Control.OnUnload</code> | <code>Control.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnUnload(EventArgs) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this Web Pa…</code> |
| <code>Api.Control.OpenFile</code> | <code>Control.OpenFile(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.Page</code> | <code>Control.Page = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=&quot;&quot;; …</code><br><code>Control.Page set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=…</code> |
| <code>Api.Control.Parent</code> | <code>Control.Parent = HtmlForm#form1 {Action=&quot;&quot;; ClientID=&quot;form1&quot;; ClientIDMode=Inherit; DefaultButton=&quot;&quot;; DefaultFocus=&quot;&quot;; Disabled=false; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.PreRender</code> | <code>Control.PreRender = &quot;added and removed&quot;</code> |
| <code>Api.Control.RaiseBubbleEvent</code> | <code>Control.RaiseBubbleEvent(Object,EventArgs) = &quot;ok&quot;</code><br><code>Control.RaiseBubbleEvent(Object,EventArgs) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to cl…</code> |
| <code>Api.Control.RemovedControl</code> | <code>Control.RemovedControl(Control) = &quot;ok&quot;</code><br><code>Control.RemovedControl(Control) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this We…</code> |
| <code>Api.Control.Render</code> | <code>Control.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.Render(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.Render(HtmlTextWriter) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this Web…</code> |
| <code>Api.Control.RenderChildren</code> | <code>Control.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderChildren(HtmlTextWriter) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close …</code> |
| <code>Api.Control.RenderControl</code> | <code>Control.RenderControl(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderControl(HtmlTextWriter) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close t…</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;…</code> |
| <code>Api.Control.RenderingCompatibility</code> | <code>Control.RenderingCompatibility = &quot;System.Version 4.0&quot;</code><br><code>Control.RenderingCompatibility set = &quot;System.Version 4.0&quot;</code> |
| <code>Api.Control.ResolveAdapter</code> | <code>Control.ResolveAdapter = null</code><br><code>Control.ResolveAdapter after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this Web Part.  …</code> |
| <code>Api.Control.ResolveClientUrl</code> | <code>Control.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveClientUrl(String) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this W…</code> |
| <code>Api.Control.ResolveUrl</code> | <code>Control.ResolveUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveUrl(String) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this Web Par…</code> |
| <code>Api.Control.SaveControlState</code> | <code>Control.SaveControlState = null</code><br><code>Control.SaveControlState after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this Web Part.…</code> |
| <code>Api.Control.SaveViewState</code> | <code>Control.SaveViewState = null</code><br><code>Control.SaveViewState after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this Web Part.  I…</code> |
| <code>Api.Control.SetDesignModeState</code> | <code>Control.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>Control.SetDesignModeState(IDictionary) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close…</code> |
| <code>Api.Control.SetRenderMethodDelegate</code> | <code>Control.SetRenderMethodDelegate(RenderMethod) = &quot;ok&quot;</code><br><code>Control.SetRenderMethodDelegate(RenderMethod) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to…</code> |
| <code>Api.Control.SetTraceData</code> | <code>Control.SetTraceData(Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close thi…</code><br><code>Control.SetTraceData(Object,Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object,Object) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to cl…</code> |
| <code>Api.Control.Site</code> | <code>Control.Site = null</code><br><code>Control.Site set = null</code> |
| <code>Api.Control.SkinID</code> | <code>Control.SkinID = &quot;&quot;</code><br><code>Control.SkinID set = &quot;-&gt; System.NotSupportedException: Control of type &#39;WebPartManager&#39; does not support theming.&quot;</code> |
| <code>Api.Control.TemplateControl</code> | <code>Control.TemplateControl = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code><br><code>Control.TemplateControl set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Cl…</code> |
| <code>Api.Control.TemplateSourceDirectory</code> | <code>Control.TemplateSourceDirectory = &quot;&quot;</code> |
| <code>Api.Control.TrackViewState</code> | <code>Control.TrackViewState = &quot;ok&quot;</code><br><code>Control.TrackViewState after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this Web Part.  …</code> |
| <code>Api.Control.UniqueID</code> | <code>Control.UniqueID = &quot;wpmProbe&quot;</code> |
| <code>Api.Control.Unload</code> | <code>Control.Unload = &quot;added and removed&quot;</code> |
| <code>Api.Control.ValidateRequestMode</code> | <code>Control.ValidateRequestMode = &quot;ValidateRequestMode.Inherit&quot;</code><br><code>Control.ValidateRequestMode set = &quot;ValidateRequestMode.Disabled&quot;</code> |
| <code>Api.Control.ViewState</code> | <code>Control.ViewState = StateBag []</code> |
| <code>Api.Control.ViewStateIgnoresCase</code> | <code>Control.ViewStateIgnoresCase = &quot;false&quot;</code> |
| <code>Api.Control.ViewStateMode</code> | <code>Control.ViewStateMode = &quot;ViewStateMode.Inherit&quot;</code><br><code>Control.ViewStateMode set = &quot;ViewStateMode.Enabled&quot;</code> |
| <code>Api.Control.Visible</code> | <code>Control.Visible = &quot;true&quot;</code><br><code>Control.Visible set = &quot;-&gt; System.NotSupportedException: Control of type &#39;WebPartManager&#39; is a non-visual control and does not support setting the Visible proper…</code> |
| <code>Api.IControlBuilderAccessor.ControlBuilder</code> | <code>IControlBuilderAccessor.ControlBuilder = null</code> |
| <code>Api.IControlDesignerAccessor.GetDesignModeState</code> | <code>IControlDesignerAccessor.GetDesignModeState = &quot;[]&quot;</code><br><code>IControlDesignerAccessor.GetDesignModeState after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to c…</code> |
| <code>Api.IControlDesignerAccessor.SetDesignModeState</code> | <code>IControlDesignerAccessor.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetDesignModeState(IDictionary) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You a…</code> |
| <code>Api.IControlDesignerAccessor.SetOwnerControl</code> | <code>IControlDesignerAccessor.SetOwnerControl(Control) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetOwnerControl(Control) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are abou…</code> |
| <code>Api.IControlDesignerAccessor.UserData</code> | <code>IControlDesignerAccessor.UserData = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.DataBindings</code> | <code>IDataBindingsAccessor.DataBindings = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.HasDataBindings</code> | <code>IDataBindingsAccessor.HasDataBindings = &quot;false&quot;</code> |
| <code>Api.IExpressionsAccessor.Expressions</code> | <code>IExpressionsAccessor.Expressions = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.HasExpressions</code> | <code>IExpressionsAccessor.HasExpressions = &quot;false&quot;</code> |
| <code>Api.IParserAccessor.AddParsedSubObject</code> | <code>IParserAccessor.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>IParserAccessor.AddParsedSubObject(Object) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to cl…</code> |
| <code>Api.IUrlResolutionService.ResolveClientUrl</code> | <code>IUrlResolutionService.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>IUrlResolutionService.ResolveClientUrl(String) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about t…</code> |
| <code>Api.WebControls.WebParts.IPersonalizable.IsDirty</code> | <code>WebParts.IPersonalizable.IsDirty = &quot;false&quot;</code> |
| <code>Api.WebControls.WebParts.IPersonalizable.Load</code> | <code>WebParts.IPersonalizable.Load(WebParts.PersonalizationDictionary) = &quot;ok&quot;</code><br><code>WebParts.IPersonalizable.Load(WebParts.PersonalizationDictionary) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarni…</code> |
| <code>Api.WebControls.WebParts.IPersonalizable.Save</code> | <code>WebParts.IPersonalizable.Save(WebParts.PersonalizationDictionary) = &quot;ok&quot;</code><br><code>WebParts.IPersonalizable.Save(WebParts.PersonalizationDictionary) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarni…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.#ctor</code> | <code>WebParts.WebPartManager.#ctor = WebPartManager# {ClientID=&quot;&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close this Web Part.  It is currently…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.ActivateConnections</code> | <code>WebParts.WebPartManager.ActivateConnections = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.ActivateConnections after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to c…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.AddWebPart</code> | <code>WebParts.WebPartManager.AddWebPart(WebParts.WebPart,WebParts.WebPartZoneBase,Int32) = &quot;-&gt; System.InvalidOperationException: Personalization is not enabled and/o…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.AuthorizeWebPart</code> | <code>WebParts.WebPartManager.AuthorizeWebPart = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.AvailableTransformers</code> | <code>WebParts.WebPartManager.AvailableTransformers = &quot;[System.RuntimeType System.Web.UI.WebControls.WebParts.RowToFieldTransformer, System.RuntimeType System.Web.UI.…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.BeginWebPartConnecting</code> | <code>WebParts.WebPartManager.BeginWebPartConnecting(WebParts.WebPart) = &quot;-&gt; System.InvalidOperationException: Personalization is not enabled and/or modifiable. The E…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.BeginWebPartEditing</code> | <code>WebParts.WebPartManager.BeginWebPartEditing(WebParts.WebPart) = &quot;-&gt; System.InvalidOperationException: Personalization is not enabled and/or modifiable. The Enab…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.BrowseDisplayMode</code> | <code>WebParts.WebPartManager.BrowseDisplayMode = BrowseWebPartDisplayMode {AllowPageDesign=false; AssociatedWithToolZone=false; Name=&quot;Browse&quot;; RequiresPersonalizatio…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.CanConnectWebParts</code> | <code>WebParts.WebPartManager.CanConnectWebParts(WebParts.WebPart,WebParts.ProviderConnectionPoint,WebParts.WebPart,WebParts.ConsumerConnectionPoint) = &quot;false&quot;</code><br><code>WebParts.WebPartManager.CanConnectWebParts(WebParts.WebPart,WebParts.ProviderConnectionPoint,WebParts.WebPart,WebParts.ConsumerConnectionPoint) after = WebPartM…</code><br><code>WebParts.WebPartManager.CanConnectWebParts(WebParts.WebPart,WebParts.ProviderConnectionPoint,WebParts.WebPart,WebParts.ConsumerConnectionPoint,WebParts.WebPartT…</code><br><code>WebParts.WebPartManager.CanConnectWebParts(WebParts.WebPart,WebParts.ProviderConnectionPoint,WebParts.WebPart,WebParts.ConsumerConnectionPoint,WebParts.WebPartT…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.CatalogDisplayMode</code> | <code>WebParts.WebPartManager.CatalogDisplayMode = CatalogWebPartDisplayMode {AllowPageDesign=true; AssociatedWithToolZone=true; Name=&quot;Catalog&quot;; RequiresPersonalizati…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.CheckRenderClientScript</code> | <code>WebParts.WebPartManager.CheckRenderClientScript = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.CloseProviderWarning</code> | <code>WebParts.WebPartManager.CloseProviderWarning = &quot;You are about to close this Web Part.  It is currently providing data to other Web Parts, and these connections …</code><br><code>WebParts.WebPartManager.CloseProviderWarning set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.CloseWebPart</code> | <code>WebParts.WebPartManager.CloseWebPart(WebParts.WebPart) = &quot;-&gt; System.InvalidOperationException: Personalization is not enabled and/or modifiable. The Enabled pro…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.ConnectDisplayMode</code> | <code>WebParts.WebPartManager.ConnectDisplayMode = ConnectWebPartDisplayMode {AllowPageDesign=true; AssociatedWithToolZone=true; Name=&quot;Connect&quot;; RequiresPersonalizati…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.ConnectWebParts</code> | <code>WebParts.WebPartManager.ConnectWebParts(WebParts.WebPart,WebParts.ProviderConnectionPoint,WebParts.WebPart,WebParts.ConsumerConnectionPoint) = &quot;-&gt; System.Invali…</code><br><code>WebParts.WebPartManager.ConnectWebParts(WebParts.WebPart,WebParts.ProviderConnectionPoint,WebParts.WebPart,WebParts.ConsumerConnectionPoint,WebParts.WebPartTran…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.Connections</code> | <code>WebParts.WebPartManager.Connections = &quot;[]&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.ConnectionsActivated</code> | <code>WebParts.WebPartManager.ConnectionsActivated = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.ConnectionsActivating</code> | <code>WebParts.WebPartManager.ConnectionsActivating = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.Controls</code> | <code>WebParts.WebPartManager.Controls = ControlCollection []</code> |
| <code>Api.WebControls.WebParts.WebPartManager.CopyWebPart</code> | <code>WebParts.WebPartManager.CopyWebPart(WebParts.WebPart) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.CreateAvailableTransformers</code> | <code>WebParts.WebPartManager.CreateAvailableTransformers = &quot;[System.RuntimeType System.Web.UI.WebControls.WebParts.RowToFieldTransformer, System.RuntimeType System.W…</code><br><code>WebParts.WebPartManager.CreateAvailableTransformers after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are ab…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.CreateControlCollection</code> | <code>WebParts.WebPartManager.CreateControlCollection = ControlCollection []</code><br><code>WebParts.WebPartManager.CreateControlCollection after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about …</code> |
| <code>Api.WebControls.WebParts.WebPartManager.CreateDisplayModes</code> | <code>WebParts.WebPartManager.CreateDisplayModes = &quot;[System.Web.UI.WebControls.WebParts.WebPartManager+BrowseWebPartDisplayMode, System.Web.UI.WebControls.WebParts.We…</code><br><code>WebParts.WebPartManager.CreateDisplayModes after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to cl…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.CreateDynamicConnectionID</code> | <code>WebParts.WebPartManager.CreateDynamicConnectionID = &quot;c{random}&quot;</code><br><code>WebParts.WebPartManager.CreateDynamicConnectionID after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are abou…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.CreateDynamicWebPartID</code> | <code>WebParts.WebPartManager.CreateDynamicWebPartID(Type) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.CreateErrorWebPart</code> | <code>WebParts.WebPartManager.CreateErrorWebPart(String,String,String,String,String) = ErrorWebPart#Abc 123 {AccessKey=&quot;&quot;; AllowClose=true; AllowConnect=true; AllowEd…</code><br><code>WebParts.WebPartManager.CreateErrorWebPart(String,String,String,String,String) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; Close…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.CreatePersonalization</code> | <code>WebParts.WebPartManager.CreatePersonalization = WebPartPersonalization {CanEnterSharedScope=false; Enabled=true; HasPersonalizationState=!InvalidOperationExcept…</code><br><code>WebParts.WebPartManager.CreatePersonalization after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.CreateWebPart</code> | <code>WebParts.WebPartManager.CreateWebPart(Control) = GenericWebPart#gwplblTitle {AccessKey=&quot;&quot;; AllowClose=true; AllowConnect=true; AllowEdit=true; AllowHide=true; A…</code><br><code>WebParts.WebPartManager.CreateWebPart(Control) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about t…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.DeleteWarning</code> | <code>WebParts.WebPartManager.DeleteWarning = &quot;You are about to permanently delete this Web Part.  Are you sure you want to do this?  To delete this Web Part, click O…</code><br><code>WebParts.WebPartManager.DeleteWarning set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.DeleteWebPart</code> | <code>WebParts.WebPartManager.DeleteWebPart(WebParts.WebPart) = &quot;-&gt; System.InvalidOperationException: Personalization is not enabled and/or modifiable. The Enabled pr…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.DesignDisplayMode</code> | <code>WebParts.WebPartManager.DesignDisplayMode = DesignWebPartDisplayMode {AllowPageDesign=true; AssociatedWithToolZone=false; Name=&quot;Design&quot;; RequiresPersonalization…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.DisconnectWebPart</code> | <code>WebParts.WebPartManager.DisconnectWebPart(WebParts.WebPart) = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.DisconnectWebPart(WebParts.WebPart) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;Yo…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.DisconnectWebParts</code> | <code>WebParts.WebPartManager.DisconnectWebParts(WebParts.WebPartConnection) = &quot;-&gt; System.InvalidOperationException: Personalization is not enabled and/or modifiable.…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.DisplayMode</code> | <code>WebParts.WebPartManager.DisplayMode = BrowseWebPartDisplayMode {AllowPageDesign=false; AssociatedWithToolZone=false; Name=&quot;Browse&quot;; RequiresPersonalization=fals…</code><br><code>WebParts.WebPartManager.DisplayMode set = &quot;-&gt; System.ArgumentNullException: Value cannot be null. [param value]&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.DisplayModeChanged</code> | <code>WebParts.WebPartManager.DisplayModeChanged = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.DisplayModeChanging</code> | <code>WebParts.WebPartManager.DisplayModeChanging = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.DisplayModes</code> | <code>WebParts.WebPartManager.DisplayModes = &quot;[System.Web.UI.WebControls.WebParts.WebPartManager+BrowseWebPartDisplayMode, System.Web.UI.WebControls.WebParts.WebPartM…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.DynamicConnections</code> | <code>WebParts.WebPartManager.DynamicConnections = &quot;[]&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.EditDisplayMode</code> | <code>WebParts.WebPartManager.EditDisplayMode = EditWebPartDisplayMode {AllowPageDesign=true; AssociatedWithToolZone=true; Name=&quot;Edit&quot;; RequiresPersonalization=true; …</code> |
| <code>Api.WebControls.WebParts.WebPartManager.EnableClientScript</code> | <code>WebParts.WebPartManager.EnableClientScript = &quot;true&quot;</code><br><code>WebParts.WebPartManager.EnableClientScript set = &quot;true&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.EnableTheming</code> | <code>WebParts.WebPartManager.EnableTheming = &quot;true&quot;</code><br><code>WebParts.WebPartManager.EnableTheming set = &quot;-&gt; System.NotSupportedException: Cannot set the EnableTheming property on WebPartManager.  EnableTheming must be tr…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.EndWebPartConnecting</code> | <code>WebParts.WebPartManager.EndWebPartConnecting = &quot;-&gt; System.InvalidOperationException: Personalization is not enabled and/or modifiable. The Enabled property must…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.EndWebPartEditing</code> | <code>WebParts.WebPartManager.EndWebPartEditing = &quot;-&gt; System.InvalidOperationException: Personalization is not enabled and/or modifiable. The Enabled property must be…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.ExportSensitiveDataWarning</code> | <code>WebParts.WebPartManager.ExportSensitiveDataWarning = &quot;This Web Part Page has been personalized. As a result, one or more Web Part properties may contain confide…</code><br><code>WebParts.WebPartManager.ExportSensitiveDataWarning set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.ExportWebPart</code> | <code>WebParts.WebPartManager.ExportWebPart(WebParts.WebPart,Xml.XmlWriter) = &quot;-&gt; System.ArgumentNullException: Value cannot be null. [param webPart]&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.Focus</code> | <code>WebParts.WebPartManager.Focus = &quot;-&gt; System.NotSupportedException: Control of type &#39;WebPartManager&#39; does not support the Focus operation.&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.GetConsumerConnectionPoints</code> | <code>WebParts.WebPartManager.GetConsumerConnectionPoints(WebParts.WebPart) = &quot;-&gt; System.ArgumentNullException: Value cannot be null. [param webPart]&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.GetCurrentWebPartManager</code> | <code>WebParts.WebPartManager.GetCurrentWebPartManager(Page) = null</code> |
| <code>Api.WebControls.WebParts.WebPartManager.GetDisplayTitle</code> | <code>WebParts.WebPartManager.GetDisplayTitle(WebParts.WebPart) = &quot;-&gt; System.ArgumentNullException: Value cannot be null. [param webPart]&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.GetExportUrl</code> | <code>WebParts.WebPartManager.GetExportUrl(WebParts.WebPart) = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.GetGenericWebPart</code> | <code>WebParts.WebPartManager.GetGenericWebPart(Control) = null</code><br><code>WebParts.WebPartManager.GetGenericWebPart(Control) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are abo…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.GetProviderConnectionPoints</code> | <code>WebParts.WebPartManager.GetProviderConnectionPoints(WebParts.WebPart) = &quot;-&gt; System.ArgumentNullException: Value cannot be null. [param webPart]&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.ImportWebPart</code> | <code>WebParts.WebPartManager.ImportWebPart(Xml.XmlReader,String@) = &quot;-&gt; System.InvalidOperationException: Personalization is not enabled and/or modifiable. The Enabl…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.Internals</code> | <code>WebParts.WebPartManager.Internals = WebPartManagerInternals {}</code> |
| <code>Api.WebControls.WebParts.WebPartManager.IsAuthorized</code> | <code>WebParts.WebPartManager.IsAuthorized(Type,String,String,Boolean) = &quot;-&gt; System.ArgumentException: The &quot;path&quot; argument must be empty if the &quot;type&quot; argument is not…</code><br><code>WebParts.WebPartManager.IsAuthorized(WebParts.WebPart) = &quot;-&gt; System.ArgumentNullException: Value cannot be null. [param webPart]&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.IsCustomPersonalizationStateDirty</code> | <code>WebParts.WebPartManager.IsCustomPersonalizationStateDirty = &quot;false&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.LoadControlState</code> | <code>WebParts.WebPartManager.LoadControlState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Object[]&#39;.&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.LoadCustomPersonalizationState</code> | <code>WebParts.WebPartManager.LoadCustomPersonalizationState(WebParts.PersonalizationDictionary) = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.LoadCustomPersonalizationState(WebParts.PersonalizationDictionary) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=In…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.MediumPermissionSet</code> | <code>WebParts.WebPartManager.MediumPermissionSet = &quot;[System.Security.Permissions.SecurityPermission &lt;IPermission class=&quot;System.Security.Permissions.SecurityPermissio…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.MinimalPermissionSet</code> | <code>WebParts.WebPartManager.MinimalPermissionSet = &quot;[System.Security.Permissions.SecurityPermission &lt;IPermission class=&quot;System.Security.Permissions.SecurityPermissi…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.MoveWebPart</code> | <code>WebParts.WebPartManager.MoveWebPart(WebParts.WebPart,WebParts.WebPartZoneBase,Int32) = &quot;-&gt; System.InvalidOperationException: Personalization is not enabled and/…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.OnAuthorizeWebPart</code> | <code>WebParts.WebPartManager.OnAuthorizeWebPart(WebParts.WebPartAuthorizationEventArgs) = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.OnAuthorizeWebPart(WebParts.WebPartAuthorizationEventArgs) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; C…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.OnConnectionsActivated</code> | <code>WebParts.WebPartManager.OnConnectionsActivated(EventArgs) = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.OnConnectionsActivated(EventArgs) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You …</code> |
| <code>Api.WebControls.WebParts.WebPartManager.OnConnectionsActivating</code> | <code>WebParts.WebPartManager.OnConnectionsActivating(EventArgs) = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.OnConnectionsActivating(EventArgs) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.OnDisplayModeChanged</code> | <code>WebParts.WebPartManager.OnDisplayModeChanged(WebParts.WebPartDisplayModeEventArgs) = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.OnDisplayModeChanged(WebParts.WebPartDisplayModeEventArgs) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; C…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.OnDisplayModeChanging</code> | <code>WebParts.WebPartManager.OnDisplayModeChanging(WebParts.WebPartDisplayModeCancelEventArgs) = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.OnDisplayModeChanging(WebParts.WebPartDisplayModeCancelEventArgs) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inh…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.OnInit</code> | <code>WebParts.WebPartManager.OnInit(EventArgs) = &quot;-&gt; System.InvalidOperationException: Value of the &#39;WebPartManager.Page.Request&#39; property cannot be null.&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.OnPreRender</code> | <code>WebParts.WebPartManager.OnPreRender(EventArgs) = &quot;-&gt; System.Web.HttpException: Request is not available in this context&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.OnSelectedWebPartChanged</code> | <code>WebParts.WebPartManager.OnSelectedWebPartChanged(WebParts.WebPartEventArgs) = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.OnSelectedWebPartChanged(WebParts.WebPartEventArgs) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; ClosePro…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.OnSelectedWebPartChanging</code> | <code>WebParts.WebPartManager.OnSelectedWebPartChanging(WebParts.WebPartCancelEventArgs) = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.OnSelectedWebPartChanging(WebParts.WebPartCancelEventArgs) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; C…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.OnUnload</code> | <code>WebParts.WebPartManager.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.OnUnload(EventArgs) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to c…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.OnWebPartAdded</code> | <code>WebParts.WebPartManager.OnWebPartAdded(WebParts.WebPartEventArgs) = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.OnWebPartAdded(WebParts.WebPartEventArgs) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarni…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.OnWebPartAdding</code> | <code>WebParts.WebPartManager.OnWebPartAdding(WebParts.WebPartAddingEventArgs) = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.OnWebPartAdding(WebParts.WebPartAddingEventArgs) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProvid…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.OnWebPartClosed</code> | <code>WebParts.WebPartManager.OnWebPartClosed(WebParts.WebPartEventArgs) = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.OnWebPartClosed(WebParts.WebPartEventArgs) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarn…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.OnWebPartClosing</code> | <code>WebParts.WebPartManager.OnWebPartClosing(WebParts.WebPartCancelEventArgs) = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.OnWebPartClosing(WebParts.WebPartCancelEventArgs) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProvi…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.OnWebPartDeleted</code> | <code>WebParts.WebPartManager.OnWebPartDeleted(WebParts.WebPartEventArgs) = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.OnWebPartDeleted(WebParts.WebPartEventArgs) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWar…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.OnWebPartDeleting</code> | <code>WebParts.WebPartManager.OnWebPartDeleting(WebParts.WebPartCancelEventArgs) = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.OnWebPartDeleting(WebParts.WebPartCancelEventArgs) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProv…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.OnWebPartMoved</code> | <code>WebParts.WebPartManager.OnWebPartMoved(WebParts.WebPartEventArgs) = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.OnWebPartMoved(WebParts.WebPartEventArgs) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarni…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.OnWebPartMoving</code> | <code>WebParts.WebPartManager.OnWebPartMoving(WebParts.WebPartMovingEventArgs) = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.OnWebPartMoving(WebParts.WebPartMovingEventArgs) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProvid…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.OnWebPartsConnected</code> | <code>WebParts.WebPartManager.OnWebPartsConnected(WebParts.WebPartConnectionsEventArgs) = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.OnWebPartsConnected(WebParts.WebPartConnectionsEventArgs) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; Cl…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.OnWebPartsConnecting</code> | <code>WebParts.WebPartManager.OnWebPartsConnecting(WebParts.WebPartConnectionsCancelEventArgs) = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.OnWebPartsConnecting(WebParts.WebPartConnectionsCancelEventArgs) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inhe…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.OnWebPartsDisconnected</code> | <code>WebParts.WebPartManager.OnWebPartsDisconnected(WebParts.WebPartConnectionsEventArgs) = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.OnWebPartsDisconnected(WebParts.WebPartConnectionsEventArgs) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit;…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.OnWebPartsDisconnecting</code> | <code>WebParts.WebPartManager.OnWebPartsDisconnecting(WebParts.WebPartConnectionsCancelEventArgs) = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.OnWebPartsDisconnecting(WebParts.WebPartConnectionsCancelEventArgs) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=I…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.Personalization</code> | <code>WebParts.WebPartManager.Personalization = WebPartPersonalization {CanEnterSharedScope=false; Enabled=true; HasPersonalizationState=!InvalidOperationException; I…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.RegisterClientScript</code> | <code>WebParts.WebPartManager.RegisterClientScript = &quot;-&gt; System.ArgumentNullException: Value cannot be null. [param basepath]&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.Render</code> | <code>WebParts.WebPartManager.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.Render(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>WebParts.WebPartManager.Render(HtmlTextWriter) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about t…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.SaveControlState</code> | <code>WebParts.WebPartManager.SaveControlState = null</code><br><code>WebParts.WebPartManager.SaveControlState after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to clos…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.SaveCustomPersonalizationState</code> | <code>WebParts.WebPartManager.SaveCustomPersonalizationState(WebParts.PersonalizationDictionary) = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.SaveCustomPersonalizationState(WebParts.PersonalizationDictionary) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=In…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.SelectedWebPart</code> | <code>WebParts.WebPartManager.SelectedWebPart = null</code> |
| <code>Api.WebControls.WebParts.WebPartManager.SelectedWebPartChanged</code> | <code>WebParts.WebPartManager.SelectedWebPartChanged = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.SelectedWebPartChanging</code> | <code>WebParts.WebPartManager.SelectedWebPartChanging = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.SetPersonalizationDirty</code> | <code>WebParts.WebPartManager.SetPersonalizationDirty = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.SetPersonalizationDirty after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about …</code> |
| <code>Api.WebControls.WebParts.WebPartManager.SetSelectedWebPart</code> | <code>WebParts.WebPartManager.SetSelectedWebPart(WebParts.WebPart) = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.SetSelectedWebPart(WebParts.WebPart) after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;Y…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.SkinID</code> | <code>WebParts.WebPartManager.SkinID = &quot;&quot;</code><br><code>WebParts.WebPartManager.SkinID set = &quot;-&gt; System.NotSupportedException: Control of type &#39;WebPartManager&#39; does not support theming.&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.StaticConnections</code> | <code>WebParts.WebPartManager.StaticConnections = &quot;[]&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.SupportedDisplayModes</code> | <code>WebParts.WebPartManager.SupportedDisplayModes = &quot;[System.Web.UI.WebControls.WebParts.WebPartManager+BrowseWebPartDisplayMode, System.Web.UI.WebControls.WebParts…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.TrackViewState</code> | <code>WebParts.WebPartManager.TrackViewState = &quot;ok&quot;</code><br><code>WebParts.WebPartManager.TrackViewState after = WebPartManager#wpmProbe {ClientID=&quot;wpmProbe&quot;; ClientIDMode=Inherit; CloseProviderWarning=&quot;You are about to close …</code> |
| <code>Api.WebControls.WebParts.WebPartManager.Visible</code> | <code>WebParts.WebPartManager.Visible = &quot;true&quot;</code><br><code>WebParts.WebPartManager.Visible set = &quot;-&gt; System.NotSupportedException: Control of type &#39;WebPartManager&#39; is a non-visual control and does not support setting th…</code> |
| <code>Api.WebControls.WebParts.WebPartManager.WebPartAdded</code> | <code>WebParts.WebPartManager.WebPartAdded = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.WebPartAdding</code> | <code>WebParts.WebPartManager.WebPartAdding = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.WebPartClosed</code> | <code>WebParts.WebPartManager.WebPartClosed = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.WebPartClosing</code> | <code>WebParts.WebPartManager.WebPartClosing = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.WebPartDeleted</code> | <code>WebParts.WebPartManager.WebPartDeleted = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.WebPartDeleting</code> | <code>WebParts.WebPartManager.WebPartDeleting = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.WebPartMoved</code> | <code>WebParts.WebPartManager.WebPartMoved = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.WebPartMoving</code> | <code>WebParts.WebPartManager.WebPartMoving = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.WebParts</code> | <code>WebParts.WebPartManager.WebParts = &quot;[]&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.WebPartsConnected</code> | <code>WebParts.WebPartManager.WebPartsConnected = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.WebPartsConnecting</code> | <code>WebParts.WebPartManager.WebPartsConnecting = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.WebPartsDisconnected</code> | <code>WebParts.WebPartManager.WebPartsDisconnected = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.WebPartsDisconnecting</code> | <code>WebParts.WebPartManager.WebPartsDisconnecting = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.WebParts.WebPartManager.Zones</code> | <code>WebParts.WebPartManager.Zones = &quot;[]&quot;</code> |

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
| <code>.CloseProviderWarning=text</code> | <code>c.CloseProviderWarning = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.CloseProviderWarning=markup</code> | <code>c.CloseProviderWarning = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.CloseProviderWarning=japanese</code> | <code>c.CloseProviderWarning = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.CloseProviderWarning=empty</code> | <code>c.CloseProviderWarning = &quot;&quot;;</code> | <code></code> |  |
| <code>.DeleteWarning=text</code> | <code>c.DeleteWarning = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.DeleteWarning=markup</code> | <code>c.DeleteWarning = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.DeleteWarning=japanese</code> | <code>c.DeleteWarning = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.DeleteWarning=empty</code> | <code>c.DeleteWarning = &quot;&quot;;</code> | <code></code> |  |
| <code>.EnableClientScript=true</code> | <code>c.EnableClientScript = true;</code> | <code></code> |  |
| <code>.EnableClientScript=false</code> | <code>c.EnableClientScript = false;</code> | <code></code> |  |
| <code>.EnableTheming=true</code> | <code>c.EnableTheming = true;</code> | <code></code> | <code>c.EnableTheming = true; -&gt; System.NotSupportedException: Cannot set the EnableTheming property on WebPartManager.  EnableTheming must be true for the WebParts to be themeable.</code> |
| <code>.EnableTheming=false</code> | <code>c.EnableTheming = false;</code> | <code></code> | <code>c.EnableTheming = false; -&gt; System.NotSupportedException: Cannot set the EnableTheming property on WebPartManager.  EnableTheming must be true for the WebParts to be themeable.</code> |
| <code>.EnableViewState=true</code> | <code>c.EnableViewState = true;</code> | <code></code> |  |
| <code>.EnableViewState=false</code> | <code>c.EnableViewState = false;</code> | <code></code> |  |
| <code>.ExportSensitiveDataWarning=text</code> | <code>c.ExportSensitiveDataWarning = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.ExportSensitiveDataWarning=markup</code> | <code>c.ExportSensitiveDataWarning = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.ExportSensitiveDataWarning=japanese</code> | <code>c.ExportSensitiveDataWarning = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.ExportSensitiveDataWarning=empty</code> | <code>c.ExportSensitiveDataWarning = &quot;&quot;;</code> | <code></code> |  |
| <code>.ID=text</code> | <code>c.ID = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.ID=markup</code> | <code>c.ID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.ID=japanese</code> | <code>c.ID = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.ID=empty</code> | <code>c.ID = &quot;&quot;;</code> | <code></code> |  |
| <code>.SkinID=text</code> | <code>c.SkinID = &quot;probe&quot;;</code> | <code></code> | <code>c.SkinID = &quot;probe&quot;; -&gt; System.NotSupportedException: Control of type &#39;WebPartManager&#39; does not support theming.</code> |
| <code>.SkinID=markup</code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.NotSupportedException: Control of type &#39;WebPartManager&#39; does not support theming.</code> |
| <code>.SkinID=japanese</code> | <code>c.SkinID = &quot;日本語テキスト&quot;;</code> | <code></code> | <code>c.SkinID = &quot;日本語テキスト&quot;; -&gt; System.NotSupportedException: Control of type &#39;WebPartManager&#39; does not support theming.</code> |
| <code>.SkinID=empty</code> | <code>c.SkinID = &quot;&quot;;</code> | <code></code> | <code>c.SkinID = &quot;&quot;; -&gt; System.NotSupportedException: Control of type &#39;WebPartManager&#39; does not support theming.</code> |
| <code>.ValidateRequestMode=Inherit</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Inherit;</code> | <code></code> |  |
| <code>.ValidateRequestMode=Disabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Disabled;</code> | <code></code> |  |
| <code>.ValidateRequestMode=Enabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Enabled;</code> | <code></code> |  |
| <code>.ViewStateMode=Inherit</code> | <code>c.ViewStateMode = ViewStateMode.Inherit;</code> | <code></code> |  |
| <code>.ViewStateMode=Enabled</code> | <code>c.ViewStateMode = ViewStateMode.Enabled;</code> | <code></code> |  |
| <code>.ViewStateMode=Disabled</code> | <code>c.ViewStateMode = ViewStateMode.Disabled;</code> | <code></code> |  |
| <code>.Visible=true</code> | <code>c.Visible = true;</code> | <code></code> | <code>c.Visible = true; -&gt; System.NotSupportedException: Control of type &#39;WebPartManager&#39; is a non-visual control and does not support setting the Visible property.</code> |
| <code>.Visible=false</code> | <code>c.Visible = false;</code> | <code></code> | <code>c.Visible = false; -&gt; System.NotSupportedException: Control of type &#39;WebPartManager&#39; is a non-visual control and does not support setting the Visible property.</code> |
| <code>.EnableClientScript=false+CloseProviderWarning=markup</code> | <code>c.EnableClientScript = false; c.CloseProviderWarning = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.DeleteWarning=markup+ExportSensitiveDataWarning=text</code> | <code>c.DeleteWarning = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; c.ExportSensitiveDataWarning = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.Visible=false+EnableViewState=false</code> | <code>c.Visible = false; c.EnableViewState = false;</code> | <code></code> | <code>c.Visible = false; -&gt; System.NotSupportedException: Control of type &#39;WebPartManager&#39; is a non-visual control and does not support setting the Visible property.</code> |
| <code>[naming container].(default)</code> | <code>[naming container] (nothing set)</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=AutoID</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Static</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Predictable</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Inherit</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ID=text</code> | <code>[naming container] c.ID = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |

## ③ シナリオ層

ブラウザ（Playwright）で WebPartManagerScenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。

| # | 操作 | 対象 | 値 |
|---|---|---|---|
| 1 | goto | /WebPartsScenario.aspx?reset=1 | <code></code> |
| 2 | snapshot |  | **WebPartManager [render][databind][dynamic] first request (browse): the parts in their zones, a zone made in code** |
| 3 | click | btnDesign | <code></code> |
| 4 | snapshot |  | **WebPartManager [viewstate] the design mode** |
| 5 | click | btnCatalog | <code></code> |
| 6 | snapshot |  | **WebPartManager [viewstate] the catalog mode: the catalog zone and its parts** |
| 7 | evaluate | czCatalog | <code>e.querySelector(&#39;input[type=checkbox]&#39;).checked = true;</code> |
| 8 | clicktextin | czCatalog | <code>Add</code> |
| 9 | snapshot |  | **WebPartManager [viewstate] a part added from the catalog (its check box posted, the zone&#39;s Add)** |
| 10 | click | btnEditPart | <code></code> |
| 11 | snapshot |  | **WebPartManager [viewstate] the edit mode: the editor zone and its parts for the first part** |
| 12 | evaluate | ezEditor | <code>e.querySelector(&#39;input[type=text]&#39;).value = &#39;Renamed part&#39;;</code> |
| 13 | clicktextin | ezEditor | <code>OK</code> |
| 14 | snapshot |  | **WebPartManager [viewstate] the first part&#39;s title edited (the editor zone&#39;s OK)** |
| 15 | click | btnConnectPart | <code></code> |
| 16 | snapshot |  | **WebPartManager [viewstate] the connect mode for the first part** |
| 17 | clicktextin | cnzConnections | <code>Close</code> |
| 18 | snapshot |  | **WebPartManager [viewstate] the connections zone closed** |
| 19 | click | btnBrowse | <code></code> |
| 20 | clicktextin | wpzMain | <code>Minimize</code> |
| 21 | snapshot |  | **WebPartManager [postbackevent] a part minimized by its zone&#39;s verb** |
| 22 | clicktextin | wpzMain | <code>Restore</code> |
| 23 | snapshot |  | **WebPartManager [viewstate] restored** |
| 24 | clicktextin | wpzDynamic | <code>Minimize</code> |
| 25 | snapshot |  | **WebPartManager [viewstate] a part of the zone made in code minimized** |
| 26 | click | btnShow | <code></code> |
| 27 | snapshot |  | **WebPartManager [visible][viewstate] a hidden zone shown** |

<details><summary>WebPartManager [render][databind][dynamic] first request (browse): the parts in their zones, a zone made in code（期待される本文）</summary>

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

<details><summary>WebPartManager [viewstate] the design mode（期待される本文）</summary>

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

<details><summary>WebPartManager [viewstate] the catalog mode: the catalog zone and its parts（期待される本文）</summary>

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

<details><summary>WebPartManager [viewstate] a part added from the catalog (its check box posted, the zone&#39;s Add)（期待される本文）</summary>

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

<details><summary>WebPartManager [viewstate] the edit mode: the editor zone and its parts for the first part（期待される本文）</summary>

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

<details><summary>WebPartManager [viewstate] the first part&#39;s title edited (the editor zone&#39;s OK)（期待される本文）</summary>

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

<details><summary>WebPartManager [viewstate] the connect mode for the first part（期待される本文）</summary>

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

<details><summary>WebPartManager [viewstate] the connections zone closed（期待される本文）</summary>

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

<details><summary>WebPartManager [postbackevent] a part minimized by its zone&#39;s verb（期待される本文）</summary>

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

<details><summary>WebPartManager [viewstate] restored（期待される本文）</summary>

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

<details><summary>WebPartManager [viewstate] a part of the zone made in code minimized（期待される本文）</summary>

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

<details><summary>WebPartManager [visible][viewstate] a hidden zone shown（期待される本文）</summary>

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

