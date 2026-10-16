# UpdateProgress テストケース一覧

変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。
このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。

- 正解データの採取: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0
- ① API 層: 121 ケース（128 メンバー）
- ② HTML 層: 56 ケース
- ③ シナリオ層: 10 スナップショット

## 機構マトリクス

型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。

| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |
|---|---|---|---|
| プロパティ → HTML 描画 | すべてのコントロール | 48 | 2 |
| ViewState の保存と復元 | Control | 6 | 8 |
| Visible=false（描画されず状態は保持） | Control.Visible | 2 | 1 |
| 名前付けコンテナと ClientIDMode | Control.ClientIDMode | 7 | 0 |
| 動的に生成したコントロールのライフサイクル | Control | 0 | 2 |
| データバインド（DataBind / <%# %>） | Control.DataBind | 0 | 1 |
| テンプレート | ITemplate のプロパティ | 0 | 3 |

該当しない機構: テーマとスキン（SkinID / EnableTheming）、Enabled=false・Disabled=true（描画とポスト値の扱い）、ポストバックデータの取り込み、変更イベント（*Changed、ServerChange）、ReadOnly（ポストされた値を無視）、リクエスト検証（ValidateRequestMode）、ポストバックイベント、コマンドイベントと親へのバブル（CommandName）、送信ボタン／スクリプトによるポストバック（UseSubmitBehavior）、ブラウザ側のクリック処理（OnClientClick）、クロスページポストバック（PostBackUrl）、既定のボタン（Panel の DefaultButton）、AutoPostBack（クライアントスクリプトによるポストバック）、CausesValidation / ValidationGroup、検証対象（サーバー側の検証）、検証対象（ブラウザ側の検証）、双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値）、項目（ListItem の Selected・Enabled・Text・Value と ViewState）、複数選択、繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）、ポスト値のイベント検証（一覧にない値・無効な項目の拒否）、サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）、ブラウザ側の検証スクリプト（EnableClientScript）、検証結果のまとめ（ValidationSummary）、ページング、並べ替え、項目の挿入、行・項目の編集（編集・更新・取り消し・削除）、行・項目の選択、行・項目のコマンド（RowCommand・ItemCommand へのバブル）、データキー（DataKeyNames・DataKeys）、データソースコントロールとの連携（DataSourceID）、データソース連携（DataSource・DataTextField など）、クライアントコールバック

## ① API 層

UpdateProgress とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の UpdateProgress インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。

| ケース | 期待値（.NET Framework） |
|---|---|
| <code>Api.Control.#ctor</code> | <code>Control.#ctor = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMode=In…</code> |
| <code>Api.Control.Adapter</code> | <code>Control.Adapter = null</code> |
| <code>Api.Control.AddParsedSubObject</code> | <code>Control.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>Control.AddParsedSubObject(Object) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=…</code> |
| <code>Api.Control.AddedControl</code> | <code>Control.AddedControl(Control,Int32) = &quot;ok&quot;</code><br><code>Control.AddedControl(Control,Int32) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter…</code> |
| <code>Api.Control.AppRelativeTemplateSourceDirectory</code> | <code>Control.AppRelativeTemplateSourceDirectory = &quot;&quot;</code><br><code>Control.AppRelativeTemplateSourceDirectory set = &quot;-&gt; System.ArgumentException: The relative virtual path &#39;Abc 123&#39; is not allowed here.&quot;</code> |
| <code>Api.Control.ApplyStyleSheetSkin</code> | <code>Control.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>Control.ApplyStyleSheetSkin(Page) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=I…</code> |
| <code>Api.Control.BeginRenderTracing</code> | <code>Control.BeginRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.BeginRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.BeginRenderTracing(TextWriter,Object) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; Di…</code> |
| <code>Api.Control.BindingContainer</code> | <code>Control.BindingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Clien…</code> |
| <code>Api.Control.BuildProfileTree</code> | <code>Control.BuildProfileTree(String,Boolean) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.ChildControlsCreated</code> | <code>Control.ChildControlsCreated = &quot;false&quot;</code><br><code>Control.ChildControlsCreated set = &quot;ok&quot;</code> |
| <code>Api.Control.ClearCachedClientID</code> | <code>Control.ClearCachedClientID = &quot;ok&quot;</code><br><code>Control.ClearCachedClientID after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=Int32 5…</code> |
| <code>Api.Control.ClearChildControlState</code> | <code>Control.ClearChildControlState = &quot;ok&quot;</code><br><code>Control.ClearChildControlState after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=Int3…</code> |
| <code>Api.Control.ClearChildState</code> | <code>Control.ClearChildState = &quot;ok&quot;</code><br><code>Control.ClearChildState after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=Int32 500; …</code> |
| <code>Api.Control.ClearChildViewState</code> | <code>Control.ClearChildViewState = &quot;ok&quot;</code><br><code>Control.ClearChildViewState after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=Int32 5…</code> |
| <code>Api.Control.ClearEffectiveClientIDMode</code> | <code>Control.ClearEffectiveClientIDMode = &quot;ok&quot;</code><br><code>Control.ClearEffectiveClientIDMode after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=…</code> |
| <code>Api.Control.ClientID</code> | <code>Control.ClientID = &quot;uprProbe&quot;</code> |
| <code>Api.Control.ClientIDMode</code> | <code>Control.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>Control.ClientIDMode set = &quot;ClientIDMode.AutoID&quot;</code> |
| <code>Api.Control.ClientIDSeparator</code> | <code>Control.ClientIDSeparator = &quot;&#39;_&#39;&quot;</code> |
| <code>Api.Control.Context</code> | <code>Control.Context = null</code> |
| <code>Api.Control.Controls</code> | <code>Control.Controls = ControlCollection []</code> |
| <code>Api.Control.CreateChildControls</code> | <code>Control.CreateChildControls = &quot;ok&quot;</code><br><code>Control.CreateChildControls after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=Int32 5…</code> |
| <code>Api.Control.CreateControlCollection</code> | <code>Control.CreateControlCollection = ControlCollection []</code><br><code>Control.CreateControlCollection after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=Int…</code> |
| <code>Api.Control.DataBind</code> | <code>Control.DataBind = &quot;ok&quot;</code><br><code>Control.DataBind after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=Int32 500; Dynamic…</code><br><code>Control.DataBind(Boolean) = &quot;ok&quot;</code><br><code>Control.DataBind(Boolean) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=Int32 500…</code> |
| <code>Api.Control.DataBindChildren</code> | <code>Control.DataBindChildren = &quot;ok&quot;</code><br><code>Control.DataBindChildren after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=Int32 500;…</code> |
| <code>Api.Control.DataBinding</code> | <code>Control.DataBinding = &quot;added and removed&quot;</code> |
| <code>Api.Control.DataItemContainer</code> | <code>Control.DataItemContainer = null</code> |
| <code>Api.Control.DataKeysContainer</code> | <code>Control.DataKeysContainer = null</code> |
| <code>Api.Control.DesignMode</code> | <code>Control.DesignMode = &quot;false&quot;</code> |
| <code>Api.Control.Dispose</code> | <code>Control.Dispose = &quot;ok&quot;</code><br><code>Control.Dispose after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=Int32 500; DynamicL…</code> |
| <code>Api.Control.Disposed</code> | <code>Control.Disposed = &quot;added and removed&quot;</code> |
| <code>Api.Control.EnableTheming</code> | <code>Control.EnableTheming = &quot;true&quot;</code><br><code>Control.EnableTheming set = &quot;true&quot;</code> |
| <code>Api.Control.EnableViewState</code> | <code>Control.EnableViewState = &quot;true&quot;</code><br><code>Control.EnableViewState set = &quot;true&quot;</code> |
| <code>Api.Control.EndRenderTracing</code> | <code>Control.EndRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.EndRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.EndRenderTracing(TextWriter,Object) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; Disp…</code> |
| <code>Api.Control.EnsureChildControls</code> | <code>Control.EnsureChildControls = &quot;ok&quot;</code><br><code>Control.EnsureChildControls after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=Int32 5…</code> |
| <code>Api.Control.EnsureID</code> | <code>Control.EnsureID = &quot;ok&quot;</code><br><code>Control.EnsureID after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=Int32 500; Dynamic…</code> |
| <code>Api.Control.Events</code> | <code>Control.Events = &quot;System.ComponentModel.EventHandlerList&quot;</code> |
| <code>Api.Control.FindControl</code> | <code>Control.FindControl(String) = Label#lblTitle {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code><br><code>Control.FindControl(String) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=Int32 5…</code><br><code>Control.FindControl(String,Int32) = null</code><br><code>Control.FindControl(String,Int32) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=I…</code> |
| <code>Api.Control.Focus</code> | <code>Control.Focus = &quot;-&gt; System.InvalidOperationException: A form tag with runat=server must exist on the Page to use SetFocus() or the Focus property.&quot;</code> |
| <code>Api.Control.GetDesignModeState</code> | <code>Control.GetDesignModeState = &quot;[]&quot;</code><br><code>Control.GetDesignModeState after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=Int32 50…</code> |
| <code>Api.Control.GetRouteUrl</code> | <code>Control.GetRouteUrl(Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.GetUniqueIDRelativeTo</code> | <code>Control.GetUniqueIDRelativeTo(Control) = &quot;uprProbe&quot;</code><br><code>Control.GetUniqueIDRelativeTo(Control) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAf…</code> |
| <code>Api.Control.HasChildViewState</code> | <code>Control.HasChildViewState = &quot;false&quot;</code> |
| <code>Api.Control.HasControls</code> | <code>Control.HasControls = &quot;false&quot;</code><br><code>Control.HasControls after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=Int32 500; Dyna…</code> |
| <code>Api.Control.HasEvents</code> | <code>Control.HasEvents = &quot;false&quot;</code><br><code>Control.HasEvents after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=Int32 500; Dynami…</code> |
| <code>Api.Control.ID</code> | <code>Control.ID = &quot;uprProbe&quot;</code><br><code>Control.ID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.IdSeparator</code> | <code>Control.IdSeparator = &quot;&#39;$&#39;&quot;</code> |
| <code>Api.Control.Init</code> | <code>Control.Init = &quot;added and removed&quot;</code> |
| <code>Api.Control.IsChildControlStateCleared</code> | <code>Control.IsChildControlStateCleared = &quot;false&quot;</code> |
| <code>Api.Control.IsLiteralContent</code> | <code>Control.IsLiteralContent = &quot;false&quot;</code><br><code>Control.IsLiteralContent after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=Int32 500;…</code> |
| <code>Api.Control.IsTrackingViewState</code> | <code>Control.IsTrackingViewState = &quot;false&quot;</code> |
| <code>Api.Control.IsViewStateEnabled</code> | <code>Control.IsViewStateEnabled = &quot;true&quot;</code> |
| <code>Api.Control.Load</code> | <code>Control.Load = &quot;added and removed&quot;</code> |
| <code>Api.Control.LoadControlState</code> | <code>Control.LoadControlState(Object) = &quot;ok&quot;</code><br><code>Control.LoadControlState(Object) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=In…</code> |
| <code>Api.Control.LoadViewState</code> | <code>Control.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Collections.ArrayList&#39;.&quot;</code> |
| <code>Api.Control.LoadViewStateByID</code> | <code>Control.LoadViewStateByID = &quot;false&quot;</code> |
| <code>Api.Control.MapPathSecure</code> | <code>Control.MapPathSecure(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.NamingContainer</code> | <code>Control.NamingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code> |
| <code>Api.Control.OnBubbleEvent</code> | <code>Control.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Control.OnBubbleEvent(Object,EventArgs) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayA…</code> |
| <code>Api.Control.OnDataBinding</code> | <code>Control.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnDataBinding(EventArgs) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=In…</code> |
| <code>Api.Control.OnInit</code> | <code>Control.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnInit(EventArgs) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=Int32 500…</code> |
| <code>Api.Control.OnLoad</code> | <code>Control.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnLoad(EventArgs) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=Int32 500…</code> |
| <code>Api.Control.OnPreRender</code> | <code>Control.OnPreRender(EventArgs) = &quot;-&gt; System.InvalidOperationException: The control with ID &#39;uprProbe&#39; requires a ScriptManager on the page. The ScriptManager mu…</code> |
| <code>Api.Control.OnUnload</code> | <code>Control.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnUnload(EventArgs) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=Int32 5…</code> |
| <code>Api.Control.OpenFile</code> | <code>Control.OpenFile(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.Page</code> | <code>Control.Page = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=&quot;&quot;; …</code><br><code>Control.Page set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=…</code> |
| <code>Api.Control.Parent</code> | <code>Control.Parent = PlaceHolder# {ClientID=&quot;ctl00&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=&quot;ctl00&quot;; SkinID=&quot;&quot;; UniqueID=&quot;ctl00&quot;; Valida…</code> |
| <code>Api.Control.PreRender</code> | <code>Control.PreRender = &quot;added and removed&quot;</code> |
| <code>Api.Control.RaiseBubbleEvent</code> | <code>Control.RaiseBubbleEvent(Object,EventArgs) = &quot;ok&quot;</code><br><code>Control.RaiseBubbleEvent(Object,EventArgs) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; Displ…</code> |
| <code>Api.Control.RemovedControl</code> | <code>Control.RemovedControl(Control) = &quot;ok&quot;</code><br><code>Control.RemovedControl(Control) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=Int…</code> |
| <code>Api.Control.Render</code> | <code>Control.Render(HtmlTextWriter) = &quot;-&gt; System.InvalidOperationException: The control with ID &#39;uprProbe&#39; requires a ScriptManager on the page. The ScriptManager mu…</code> |
| <code>Api.Control.RenderChildren</code> | <code>Control.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderChildren(HtmlTextWriter) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAf…</code> |
| <code>Api.Control.RenderControl</code> | <code>Control.RenderControl(HtmlTextWriter) = &quot;-&gt; System.InvalidOperationException: The control with ID &#39;uprProbe&#39; requires a ScriptManager on the page. The ScriptMan…</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) = &quot;-&gt; System.InvalidOperationException: The control with ID &#39;uprProbe&#39; requires a ScriptManager on…</code> |
| <code>Api.Control.RenderingCompatibility</code> | <code>Control.RenderingCompatibility = &quot;System.Version 4.0&quot;</code><br><code>Control.RenderingCompatibility set = &quot;System.Version 4.0&quot;</code> |
| <code>Api.Control.ResolveAdapter</code> | <code>Control.ResolveAdapter = null</code><br><code>Control.ResolveAdapter after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=Int32 500; D…</code> |
| <code>Api.Control.ResolveClientUrl</code> | <code>Control.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveClientUrl(String) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=In…</code> |
| <code>Api.Control.ResolveUrl</code> | <code>Control.ResolveUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveUrl(String) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=Int32 50…</code> |
| <code>Api.Control.SaveControlState</code> | <code>Control.SaveControlState = null</code><br><code>Control.SaveControlState after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=Int32 500;…</code> |
| <code>Api.Control.SaveViewState</code> | <code>Control.SaveViewState = null</code><br><code>Control.SaveViewState after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=Int32 500; Dy…</code> |
| <code>Api.Control.SetDesignModeState</code> | <code>Control.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>Control.SetDesignModeState(IDictionary) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayA…</code> |
| <code>Api.Control.SetRenderMethodDelegate</code> | <code>Control.SetRenderMethodDelegate(RenderMethod) = &quot;ok&quot;</code><br><code>Control.SetRenderMethodDelegate(RenderMethod) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; Di…</code> |
| <code>Api.Control.SetTraceData</code> | <code>Control.SetTraceData(Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter…</code><br><code>Control.SetTraceData(Object,Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object,Object) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; Displ…</code> |
| <code>Api.Control.Site</code> | <code>Control.Site = null</code><br><code>Control.Site set = null</code> |
| <code>Api.Control.SkinID</code> | <code>Control.SkinID = &quot;&quot;</code><br><code>Control.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.TemplateControl</code> | <code>Control.TemplateControl = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code><br><code>Control.TemplateControl set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Cl…</code> |
| <code>Api.Control.TemplateSourceDirectory</code> | <code>Control.TemplateSourceDirectory = &quot;&quot;</code> |
| <code>Api.Control.TrackViewState</code> | <code>Control.TrackViewState = &quot;ok&quot;</code><br><code>Control.TrackViewState after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=Int32 500; D…</code> |
| <code>Api.Control.UniqueID</code> | <code>Control.UniqueID = &quot;uprProbe&quot;</code> |
| <code>Api.Control.Unload</code> | <code>Control.Unload = &quot;added and removed&quot;</code> |
| <code>Api.Control.ValidateRequestMode</code> | <code>Control.ValidateRequestMode = &quot;ValidateRequestMode.Inherit&quot;</code><br><code>Control.ValidateRequestMode set = &quot;ValidateRequestMode.Disabled&quot;</code> |
| <code>Api.Control.ViewState</code> | <code>Control.ViewState = StateBag []</code> |
| <code>Api.Control.ViewStateIgnoresCase</code> | <code>Control.ViewStateIgnoresCase = &quot;false&quot;</code> |
| <code>Api.Control.ViewStateMode</code> | <code>Control.ViewStateMode = &quot;ViewStateMode.Inherit&quot;</code><br><code>Control.ViewStateMode set = &quot;ViewStateMode.Enabled&quot;</code> |
| <code>Api.Control.Visible</code> | <code>Control.Visible = &quot;true&quot;</code><br><code>Control.Visible set = &quot;true&quot;</code> |
| <code>Api.IAttributeAccessor.GetAttribute</code> | <code>IAttributeAccessor.GetAttribute(String) = null</code><br><code>IAttributeAccessor.GetAttribute(String) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayA…</code> |
| <code>Api.IAttributeAccessor.SetAttribute</code> | <code>IAttributeAccessor.SetAttribute(String,String) = &quot;ok&quot;</code><br><code>IAttributeAccessor.SetAttribute(String,String) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; D…</code> |
| <code>Api.IControlBuilderAccessor.ControlBuilder</code> | <code>IControlBuilderAccessor.ControlBuilder = null</code> |
| <code>Api.IControlDesignerAccessor.GetDesignModeState</code> | <code>IControlDesignerAccessor.GetDesignModeState = &quot;[]&quot;</code><br><code>IControlDesignerAccessor.GetDesignModeState after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; Disp…</code> |
| <code>Api.IControlDesignerAccessor.SetDesignModeState</code> | <code>IControlDesignerAccessor.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetDesignModeState(IDictionary) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=…</code> |
| <code>Api.IControlDesignerAccessor.SetOwnerControl</code> | <code>IControlDesignerAccessor.SetOwnerControl(Control) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetOwnerControl(Control) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit…</code> |
| <code>Api.IControlDesignerAccessor.UserData</code> | <code>IControlDesignerAccessor.UserData = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.DataBindings</code> | <code>IDataBindingsAccessor.DataBindings = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.HasDataBindings</code> | <code>IDataBindingsAccessor.HasDataBindings = &quot;false&quot;</code> |
| <code>Api.IExpressionsAccessor.Expressions</code> | <code>IExpressionsAccessor.Expressions = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.HasExpressions</code> | <code>IExpressionsAccessor.HasExpressions = &quot;false&quot;</code> |
| <code>Api.IParserAccessor.AddParsedSubObject</code> | <code>IParserAccessor.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>IParserAccessor.AddParsedSubObject(Object) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; Displ…</code> |
| <code>Api.IScriptControl.GetScriptDescriptors</code> | <code>IScriptControl.GetScriptDescriptors = &quot;-&gt; System.InvalidOperationException: The control with ID &#39;uprProbe&#39; requires a ScriptManager on the page. The ScriptManag…</code> |
| <code>Api.IScriptControl.GetScriptReferences</code> | <code>IScriptControl.GetScriptReferences = &quot;[]&quot;</code><br><code>IScriptControl.GetScriptReferences after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=…</code> |
| <code>Api.IUrlResolutionService.ResolveClientUrl</code> | <code>IUrlResolutionService.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>IUrlResolutionService.ResolveClientUrl(String) after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; D…</code> |
| <code>Api.UpdateProgress.#ctor</code> | <code>UpdateProgress.#ctor = UpdateProgress# {AssociatedUpdatePanelID=&quot;&quot;; ClientID=&quot;&quot;; ClientIDMode=Inherit; DisplayAfter=Int32 500; DynamicLayout=true; EnableTheming…</code> |
| <code>Api.UpdateProgress.AssociatedUpdatePanelID</code> | <code>UpdateProgress.AssociatedUpdatePanelID = &quot;upTarget&quot;</code><br><code>UpdateProgress.AssociatedUpdatePanelID set = &quot;Abc 123&quot;</code> |
| <code>Api.UpdateProgress.Attributes</code> | <code>UpdateProgress.Attributes = AttributeCollection []</code> |
| <code>Api.UpdateProgress.Controls</code> | <code>UpdateProgress.Controls = ControlCollection []</code> |
| <code>Api.UpdateProgress.CreateChildControls</code> | <code>UpdateProgress.CreateChildControls = &quot;ok&quot;</code><br><code>UpdateProgress.CreateChildControls after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=…</code> |
| <code>Api.UpdateProgress.DataBind</code> | <code>UpdateProgress.DataBind = &quot;ok&quot;</code><br><code>UpdateProgress.DataBind after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=Int32 500; …</code> |
| <code>Api.UpdateProgress.DisplayAfter</code> | <code>UpdateProgress.DisplayAfter = &quot;Int32 500&quot;</code><br><code>UpdateProgress.DisplayAfter set = &quot;Int32 1&quot;</code> |
| <code>Api.UpdateProgress.DynamicLayout</code> | <code>UpdateProgress.DynamicLayout = &quot;true&quot;</code><br><code>UpdateProgress.DynamicLayout set = &quot;true&quot;</code> |
| <code>Api.UpdateProgress.GetScriptDescriptors</code> | <code>UpdateProgress.GetScriptDescriptors = &quot;-&gt; System.InvalidOperationException: The control with ID &#39;uprProbe&#39; requires a ScriptManager on the page. The ScriptManag…</code> |
| <code>Api.UpdateProgress.GetScriptReferences</code> | <code>UpdateProgress.GetScriptReferences = &quot;[]&quot;</code><br><code>UpdateProgress.GetScriptReferences after = UpdateProgress#uprProbe {AssociatedUpdatePanelID=&quot;upTarget&quot;; ClientID=&quot;uprProbe&quot;; ClientIDMode=Inherit; DisplayAfter=…</code> |
| <code>Api.UpdateProgress.OnPreRender</code> | <code>UpdateProgress.OnPreRender(EventArgs) = &quot;-&gt; System.InvalidOperationException: The control with ID &#39;uprProbe&#39; requires a ScriptManager on the page. The ScriptMan…</code> |
| <code>Api.UpdateProgress.ProgressTemplate</code> | <code>UpdateProgress.ProgressTemplate = null</code><br><code>UpdateProgress.ProgressTemplate set = null</code> |
| <code>Api.UpdateProgress.Render</code> | <code>UpdateProgress.Render(HtmlTextWriter) = &quot;-&gt; System.InvalidOperationException: The control with ID &#39;uprProbe&#39; requires a ScriptManager on the page. The ScriptMan…</code> |

## ② HTML 層

プローブサイトの RenderProbe.aspx で、1 ケース 1 リクエスト（ページライフサイクル全体）で描画したページ全体を比較します。下表はコンポーネント部分（div#probe）、設定時の例外、保存された ViewState の要約です。

| ケース | 設定 | 期待される描画 | 設定時の例外 |
|---|---|---|---|
| <code>.(default)</code> | <code>(nothing set)</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.AppRelativeTemplateSourceDirectory=text</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;probe&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=markup</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=japanese</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;日本語テキスト&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=empty</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.AssociatedUpdatePanelID=text</code> | <code>c.AssociatedUpdatePanelID = &quot;probe&quot;;</code> | <code>status = 500</code> |  |
| <code>.AssociatedUpdatePanelID=markup</code> | <code>c.AssociatedUpdatePanelID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>status = 500</code> |  |
| <code>.AssociatedUpdatePanelID=japanese</code> | <code>c.AssociatedUpdatePanelID = &quot;日本語テキスト&quot;;</code> | <code>status = 500</code> |  |
| <code>.AssociatedUpdatePanelID=empty</code> | <code>c.AssociatedUpdatePanelID = &quot;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.AssociatedUpdatePanelID=target</code> | <code>c.AssociatedUpdatePanelID = &quot;upTarget&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.AssociatedUpdatePanelID=missing</code> | <code>c.AssociatedUpdatePanelID = &quot;upMissing&quot;;</code> | <code>status = 500</code> |  |
| <code>.ClientIDMode=Inherit</code> | <code>c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ClientIDMode=AutoID</code> | <code>c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ClientIDMode=Predictable</code> | <code>c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ClientIDMode=Static</code> | <code>c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.DisplayAfter=0</code> | <code>c.DisplayAfter = 0;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.DisplayAfter=3</code> | <code>c.DisplayAfter = 3;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.DisplayAfter=-1</code> | <code>c.DisplayAfter = -1;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> | <code>c.DisplayAfter = -1; -&gt; System.ArgumentOutOfRangeException: Specified argument was out of the range of valid values. [param DisplayAfter must be a non negative integer.]</code> |
| <code>.DynamicLayout=true</code> | <code>c.DynamicLayout = true;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.DynamicLayout=false</code> | <code>c.DynamicLayout = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;visibility:hidden;display:block;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.EnableTheming=true</code> | <code>c.EnableTheming = true;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.EnableTheming=false</code> | <code>c.EnableTheming = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.EnableViewState=true</code> | <code>c.EnableViewState = true;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.EnableViewState=false</code> | <code>c.EnableViewState = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ID=text</code> | <code>c.ID = &quot;probe&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;probe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ID=markup</code> | <code>c.ID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ID=japanese</code> | <code>c.ID = &quot;日本語テキスト&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;日本語テキスト&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ID=empty</code> | <code>c.ID = &quot;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;ctl04&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.SkinID=text</code> | <code>c.SkinID = &quot;probe&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.SkinID=markup</code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.SkinID=japanese</code> | <code>c.SkinID = &quot;日本語テキスト&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.SkinID=empty</code> | <code>c.SkinID = &quot;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ValidateRequestMode=Inherit</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Inherit;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ValidateRequestMode=Disabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Disabled;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ValidateRequestMode=Enabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Enabled;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ViewStateMode=Inherit</code> | <code>c.ViewStateMode = ViewStateMode.Inherit;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ViewStateMode=Enabled</code> | <code>c.ViewStateMode = ViewStateMode.Enabled;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.ViewStateMode=Disabled</code> | <code>c.ViewStateMode = ViewStateMode.Disabled;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.Visible=true</code> | <code>c.Visible = true;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.Visible=false</code> | <code>c.Visible = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.Attributes[data-probe]=text</code> | <code>c.Attributes[data-probe] = &quot;probe&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; data-probe=&quot;probe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.Attributes[onfocus]=text</code> | <code>c.Attributes[onfocus] = &quot;probe&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; onfocus=&quot;probe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.Attributes[data-probe]=markup</code> | <code>c.Attributes[data-probe] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; data-probe=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.Attributes[onfocus]=markup</code> | <code>c.Attributes[onfocus] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; onfocus=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.AssociatedUpdatePanelID=target+DisplayAfter=0</code> | <code>c.AssociatedUpdatePanelID = &quot;upTarget&quot;; c.DisplayAfter = 0;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.DynamicLayout=false+DisplayAfter=3</code> | <code>c.DynamicLayout = false; c.DisplayAfter = 3;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;visibility:hidden;display:block;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.AssociatedUpdatePanelID=target+DynamicLayout=false</code> | <code>c.AssociatedUpdatePanelID = &quot;upTarget&quot;; c.DynamicLayout = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;visibility:hidden;display:block;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.DisplayAfter=3+Visible=false</code> | <code>c.DisplayAfter = 3; c.Visible = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>.DisplayAfter=3+EnableViewState=false</code> | <code>c.DisplayAfter = 3; c.EnableViewState = false;</code> | <code>&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;smPage&#39;, &#39;form1&#39;, [&#39;tupTarget&#39;,&#39;upTarget&#39;], [], [], 90, &#39;&#39;);↵//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;upTarget&quot;&gt;↵	&lt;span&gt;Target&lt;/span&gt;↵&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵	&lt;span&gt;Working...&lt;/span&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].(default)</code> | <code>[naming container] (nothing set)</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;box$smPage&#39;, &#39;form1&#39;, [&#39;tbox$upTarget&#39;,&#39;box_upTarget&#39;], [], [], 90, &#39;&#39;);↵	//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;box_upTarget&quot;&gt;↵		&lt;span&gt;Target&lt;/span&gt;↵	&lt;/div&gt;&lt;div id=&quot;box_uprProbe&quot; style=&quot;display:none;&quot;&gt;↵		&lt;span&gt;Working...&lt;/span&gt;↵	&lt;/div&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=AutoID</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;box$smPage&#39;, &#39;form1&#39;, [&#39;tbox$upTarget&#39;,&#39;box_upTarget&#39;], [], [], 90, &#39;&#39;);↵	//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;box_upTarget&quot;&gt;↵		&lt;span&gt;Target&lt;/span&gt;↵	&lt;/div&gt;&lt;div id=&quot;box_uprProbe&quot; style=&quot;display:none;&quot;&gt;↵		&lt;span&gt;Working...&lt;/span&gt;↵	&lt;/div&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Static</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;box$smPage&#39;, &#39;form1&#39;, [&#39;tbox$upTarget&#39;,&#39;box_upTarget&#39;], [], [], 90, &#39;&#39;);↵	//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;box_upTarget&quot;&gt;↵		&lt;span&gt;Target&lt;/span&gt;↵	&lt;/div&gt;&lt;div id=&quot;uprProbe&quot; style=&quot;display:none;&quot;&gt;↵		&lt;span&gt;Working...&lt;/span&gt;↵	&lt;/div&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Predictable</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;box$smPage&#39;, &#39;form1&#39;, [&#39;tbox$upTarget&#39;,&#39;box_upTarget&#39;], [], [], 90, &#39;&#39;);↵	//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;box_upTarget&quot;&gt;↵		&lt;span&gt;Target&lt;/span&gt;↵	&lt;/div&gt;&lt;div id=&quot;box_uprProbe&quot; style=&quot;display:none;&quot;&gt;↵		&lt;span&gt;Working...&lt;/span&gt;↵	&lt;/div&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Inherit</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;box$smPage&#39;, &#39;form1&#39;, [&#39;tbox$upTarget&#39;,&#39;box_upTarget&#39;], [], [], 90, &#39;&#39;);↵	//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;box_upTarget&quot;&gt;↵		&lt;span&gt;Target&lt;/span&gt;↵	&lt;/div&gt;&lt;div id=&quot;box_uprProbe&quot; style=&quot;display:none;&quot;&gt;↵		&lt;span&gt;Working...&lt;/span&gt;↵	&lt;/div&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ID=text</code> | <code>[naming container] c.ID = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;box$smPage&#39;, &#39;form1&#39;, [&#39;tbox$upTarget&#39;,&#39;box_upTarget&#39;], [], [], 90, &#39;&#39;);↵	//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;box_upTarget&quot;&gt;↵		&lt;span&gt;Target&lt;/span&gt;↵	&lt;/div&gt;&lt;div id=&quot;box_probe&quot; style=&quot;display:none;&quot;&gt;↵		&lt;span&gt;Working...&lt;/span&gt;↵	&lt;/div&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].AssociatedUpdatePanelID=target</code> | <code>[naming container] c.AssociatedUpdatePanelID = &quot;upTarget&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;script type=&quot;text/javascript&quot;&gt;↵//&lt;![CDATA[↵Sys.WebForms.PageRequestManager._initialize(&#39;box$smPage&#39;, &#39;form1&#39;, [&#39;tbox$upTarget&#39;,&#39;box_upTarget&#39;], [], [], 90, &#39;&#39;);↵	//]]&gt;↵&lt;/script&gt;↵&lt;div id=&quot;box_upTarget&quot;&gt;↵		&lt;span&gt;Target&lt;/span&gt;↵	&lt;/div&gt;&lt;div id=&quot;box_uprProbe&quot; style=&quot;display:none;&quot;&gt;↵		&lt;span&gt;Working...&lt;/span&gt;↵	&lt;/div&gt;↵&lt;/div&gt;</code> |  |

## ③ シナリオ層

ブラウザ（Playwright）で UpdateProgressScenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。

| # | 操作 | 対象 | 値 |
|---|---|---|---|
| 1 | goto | /AjaxScenario.aspx | <code></code> |
| 2 | snapshot |  | **UpdateProgress [render][databind][dynamic][templates] first request: the panels, the proxy&#39;s script run, a panel made in code** |
| 3 | fill | txtInPanel | <code>in the panel</code> |
| 4 | click | btnInPanel | <code></code> |
| 5 | snapshot |  | **UpdateProgress [viewstate] an asynchronous postback of the main panel: the conditional panel outside left as it was** |
| 6 | click | btnOutside | <code></code> |
| 7 | snapshot |  | **UpdateProgress [viewstate] a button outside triggers the second panel** |
| 8 | click | btnFull | <code></code> |
| 9 | snapshot |  | **UpdateProgress [viewstate] a postback trigger in the panel: a full postback** |
| 10 | evaluate | btnSlow | <code>e.click();</code> |
| 11 | snapshot |  | **UpdateProgress [templates][render] while a slow request runs: the progress shown** |
| 12 | waitms |  | <code></code> |
| 13 | snapshot |  | **UpdateProgress [templates][viewstate] after the slow request: the progress hidden** |
| 14 | click | btnStartTimer | <code></code> |
| 15 | waitms |  | <code></code> |
| 16 | snapshot |  | **UpdateProgress [viewstate] the timer started: one tick, then stopped** |
| 17 | click | btnHistory | <code></code> |
| 18 | evaluate | btnInPanel | <code>history.back();</code> |
| 19 | waitms |  | <code></code> |
| 20 | snapshot |  | **UpdateProgress [viewstate] a history point, then the browser&#39;s back: the script manager&#39;s Navigate** |
| 21 | click | btnDynamic | <code></code> |
| 22 | snapshot |  | **UpdateProgress [dynamic][viewstate] an asynchronous postback of the panel made in code** |
| 23 | click | btnShow | <code></code> |
| 24 | snapshot |  | **UpdateProgress [visible][viewstate] a hidden panel shown with what was set while hidden** |

<details><summary>UpdateProgress [render][databind][dynamic][templates] first request: the panels, the proxy&#39;s script run, a panel made in code（期待される本文）</summary>

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

<details><summary>UpdateProgress [viewstate] an asynchronous postback of the main panel: the conditional panel outside left as it was（期待される本文）</summary>

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

<details><summary>UpdateProgress [viewstate] a button outside triggers the second panel（期待される本文）</summary>

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

<details><summary>UpdateProgress [viewstate] a postback trigger in the panel: a full postback（期待される本文）</summary>

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

<details><summary>UpdateProgress [templates][render] while a slow request runs: the progress shown（期待される本文）</summary>

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

<details><summary>UpdateProgress [templates][viewstate] after the slow request: the progress hidden（期待される本文）</summary>

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

<details><summary>UpdateProgress [viewstate] the timer started: one tick, then stopped（期待される本文）</summary>

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

<details><summary>UpdateProgress [viewstate] a history point, then the browser&#39;s back: the script manager&#39;s Navigate（期待される本文）</summary>

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

<details><summary>UpdateProgress [dynamic][viewstate] an asynchronous postback of the panel made in code（期待される本文）</summary>

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

<details><summary>UpdateProgress [visible][viewstate] a hidden panel shown with what was set while hidden（期待される本文）</summary>

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

