# HiddenField テストケース一覧

変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。
このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。

- 正解データの採取: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0
- ① API 層: 118 ケース（125 メンバー）
- ② HTML 層: 41 ケース
- ③ シナリオ層: 6 スナップショット

## 機構マトリクス

型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。

| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |
|---|---|---|---|
| プロパティ → HTML 描画 | すべてのコントロール | 34 | 1 |
| ViewState の保存と復元 | Control | 6 | 2 |
| Visible=false（描画されず状態は保持） | Control.Visible | 2 | 1 |
| 名前付けコンテナと ClientIDMode | Control.ClientIDMode | 6 | 0 |
| 動的に生成したコントロールのライフサイクル | Control | 0 | 2 |
| データバインド（DataBind / <%# %>） | Control.DataBind | 0 | 1 |
| ポストバックデータの取り込み | IPostBackDataHandler | 0 | 1 |
| 変更イベント（*Changed、ServerChange） | IPostBackDataHandler と *Changed・ServerChange イベント | 0 | 2 |
| リクエスト検証（ValidateRequestMode） | IPostBackDataHandler（ポスト値を読む） | 3 | 1 |
| 双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値） | [ControlValueProperty] と IPostBackDataHandler | 0 | 1 |

該当しない機構: テーマとスキン（SkinID / EnableTheming）、Enabled=false・Disabled=true（描画とポスト値の扱い）、ReadOnly（ポストされた値を無視）、ポストバックイベント、コマンドイベントと親へのバブル（CommandName）、送信ボタン／スクリプトによるポストバック（UseSubmitBehavior）、ブラウザ側のクリック処理（OnClientClick）、クロスページポストバック（PostBackUrl）、既定のボタン（Panel の DefaultButton）、AutoPostBack（クライアントスクリプトによるポストバック）、CausesValidation / ValidationGroup、検証対象（サーバー側の検証）、検証対象（ブラウザ側の検証）、項目（ListItem の Selected・Enabled・Text・Value と ViewState）、複数選択、繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）、ポスト値のイベント検証（一覧にない値・無効な項目の拒否）、サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）、ブラウザ側の検証スクリプト（EnableClientScript）、検証結果のまとめ（ValidationSummary）、ページング、並べ替え、項目の挿入、行・項目の編集（編集・更新・取り消し・削除）、行・項目の選択、行・項目のコマンド（RowCommand・ItemCommand へのバブル）、データキー（DataKeyNames・DataKeys）、データソースコントロールとの連携（DataSourceID）、テンプレート、データソース連携（DataSource・DataTextField など）、クライアントコールバック

## ① API 層

HiddenField とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の HiddenField インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。

| ケース | 期待値（.NET Framework） |
|---|---|
| <code>Api.Control.#ctor</code> | <code>Control.#ctor = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMode=In…</code> |
| <code>Api.Control.Adapter</code> | <code>Control.Adapter = null</code> |
| <code>Api.Control.AddParsedSubObject</code> | <code>Control.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>Control.AddParsedSubObject(Object) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidVa…</code> |
| <code>Api.Control.AddedControl</code> | <code>Control.AddedControl(Control,Int32) = &quot;ok&quot;</code><br><code>Control.AddedControl(Control,Int32) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidV…</code> |
| <code>Api.Control.AppRelativeTemplateSourceDirectory</code> | <code>Control.AppRelativeTemplateSourceDirectory = &quot;&quot;</code><br><code>Control.AppRelativeTemplateSourceDirectory set = &quot;-&gt; System.ArgumentException: The relative virtual path &#39;Abc 123&#39; is not allowed here.&quot;</code> |
| <code>Api.Control.ApplyStyleSheetSkin</code> | <code>Control.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>Control.ApplyStyleSheetSkin(Page) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidVal…</code> |
| <code>Api.Control.BeginRenderTracing</code> | <code>Control.BeginRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.BeginRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.BeginRenderTracing(TextWriter,Object) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true…</code> |
| <code>Api.Control.BindingContainer</code> | <code>Control.BindingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Clien…</code> |
| <code>Api.Control.BuildProfileTree</code> | <code>Control.BuildProfileTree(String,Boolean) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.ChildControlsCreated</code> | <code>Control.ChildControlsCreated = &quot;false&quot;</code><br><code>Control.ChildControlsCreated set = &quot;ok&quot;</code> |
| <code>Api.Control.ClearCachedClientID</code> | <code>Control.ClearCachedClientID = &quot;ok&quot;</code><br><code>Control.ClearCachedClientID after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue&quot;; S…</code> |
| <code>Api.Control.ClearChildControlState</code> | <code>Control.ClearChildControlState = &quot;ok&quot;</code><br><code>Control.ClearChildControlState after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue&quot;…</code> |
| <code>Api.Control.ClearChildState</code> | <code>Control.ClearChildState = &quot;ok&quot;</code><br><code>Control.ClearChildState after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue&quot;; SkinI…</code> |
| <code>Api.Control.ClearChildViewState</code> | <code>Control.ClearChildViewState = &quot;ok&quot;</code><br><code>Control.ClearChildViewState after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue&quot;; S…</code> |
| <code>Api.Control.ClearEffectiveClientIDMode</code> | <code>Control.ClearEffectiveClientIDMode = &quot;ok&quot;</code><br><code>Control.ClearEffectiveClientIDMode after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidVa…</code> |
| <code>Api.Control.ClientID</code> | <code>Control.ClientID = &quot;hidValue&quot;</code> |
| <code>Api.Control.ClientIDMode</code> | <code>Control.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>Control.ClientIDMode set = &quot;ClientIDMode.AutoID&quot;</code> |
| <code>Api.Control.ClientIDSeparator</code> | <code>Control.ClientIDSeparator = &quot;&#39;_&#39;&quot;</code> |
| <code>Api.Control.Context</code> | <code>Control.Context = null</code> |
| <code>Api.Control.Controls</code> | <code>Control.Controls = ControlCollection []</code> |
| <code>Api.Control.CreateChildControls</code> | <code>Control.CreateChildControls = &quot;ok&quot;</code><br><code>Control.CreateChildControls after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue&quot;; S…</code> |
| <code>Api.Control.CreateControlCollection</code> | <code>Control.CreateControlCollection = ControlCollection []</code><br><code>Control.CreateControlCollection after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue…</code> |
| <code>Api.Control.DataBind</code> | <code>Control.DataBind = &quot;ok&quot;</code><br><code>Control.DataBind after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue&quot;; SkinID=&quot;&quot;; U…</code><br><code>Control.DataBind(Boolean) = &quot;ok&quot;</code><br><code>Control.DataBind(Boolean) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue&quot;; Ski…</code> |
| <code>Api.Control.DataBindChildren</code> | <code>Control.DataBindChildren = &quot;ok&quot;</code><br><code>Control.DataBindChildren after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue&quot;; Skin…</code> |
| <code>Api.Control.DataBinding</code> | <code>Control.DataBinding = &quot;added and removed&quot;</code> |
| <code>Api.Control.DataItemContainer</code> | <code>Control.DataItemContainer = null</code> |
| <code>Api.Control.DataKeysContainer</code> | <code>Control.DataKeysContainer = null</code> |
| <code>Api.Control.DesignMode</code> | <code>Control.DesignMode = &quot;false&quot;</code> |
| <code>Api.Control.Dispose</code> | <code>Control.Dispose = &quot;ok&quot;</code><br><code>Control.Dispose after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue&quot;; SkinID=&quot;&quot;; Un…</code> |
| <code>Api.Control.Disposed</code> | <code>Control.Disposed = &quot;added and removed&quot;</code> |
| <code>Api.Control.EnableTheming</code> | <code>Control.EnableTheming = &quot;false&quot;</code><br><code>Control.EnableTheming set = &quot;-&gt; System.NotSupportedException: Control of type &#39;HiddenField&#39; does not support theming.&quot;</code> |
| <code>Api.Control.EnableViewState</code> | <code>Control.EnableViewState = &quot;true&quot;</code><br><code>Control.EnableViewState set = &quot;true&quot;</code> |
| <code>Api.Control.EndRenderTracing</code> | <code>Control.EndRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.EndRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.EndRenderTracing(TextWriter,Object) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; …</code> |
| <code>Api.Control.EnsureChildControls</code> | <code>Control.EnsureChildControls = &quot;ok&quot;</code><br><code>Control.EnsureChildControls after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue&quot;; S…</code> |
| <code>Api.Control.EnsureID</code> | <code>Control.EnsureID = &quot;ok&quot;</code><br><code>Control.EnsureID after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue&quot;; SkinID=&quot;&quot;; U…</code> |
| <code>Api.Control.Events</code> | <code>Control.Events = &quot;System.ComponentModel.EventHandlerList&quot;</code> |
| <code>Api.Control.FindControl</code> | <code>Control.FindControl(String) = Label#lblTitle {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code><br><code>Control.FindControl(String) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue&quot;; S…</code><br><code>Control.FindControl(String,Int32) = null</code><br><code>Control.FindControl(String,Int32) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidVal…</code> |
| <code>Api.Control.Focus</code> | <code>Control.Focus = &quot;-&gt; System.NotSupportedException: Control of type &#39;HiddenField&#39; does not support the Focus operation.&quot;</code> |
| <code>Api.Control.GetDesignModeState</code> | <code>Control.GetDesignModeState = &quot;[]&quot;</code><br><code>Control.GetDesignModeState after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue&quot;; Sk…</code> |
| <code>Api.Control.GetRouteUrl</code> | <code>Control.GetRouteUrl(Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.GetUniqueIDRelativeTo</code> | <code>Control.GetUniqueIDRelativeTo(Control) = &quot;hidValue&quot;</code><br><code>Control.GetUniqueIDRelativeTo(Control) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;h…</code> |
| <code>Api.Control.HasChildViewState</code> | <code>Control.HasChildViewState = &quot;false&quot;</code> |
| <code>Api.Control.HasControls</code> | <code>Control.HasControls = &quot;false&quot;</code><br><code>Control.HasControls after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue&quot;; SkinID=&quot;&quot;…</code> |
| <code>Api.Control.HasEvents</code> | <code>Control.HasEvents = &quot;false&quot;</code><br><code>Control.HasEvents after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue&quot;; SkinID=&quot;&quot;; …</code> |
| <code>Api.Control.ID</code> | <code>Control.ID = &quot;hidValue&quot;</code><br><code>Control.ID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.IdSeparator</code> | <code>Control.IdSeparator = &quot;&#39;$&#39;&quot;</code> |
| <code>Api.Control.Init</code> | <code>Control.Init = &quot;added and removed&quot;</code> |
| <code>Api.Control.IsChildControlStateCleared</code> | <code>Control.IsChildControlStateCleared = &quot;false&quot;</code> |
| <code>Api.Control.IsLiteralContent</code> | <code>Control.IsLiteralContent = &quot;false&quot;</code><br><code>Control.IsLiteralContent after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue&quot;; Skin…</code> |
| <code>Api.Control.IsTrackingViewState</code> | <code>Control.IsTrackingViewState = &quot;false&quot;</code> |
| <code>Api.Control.IsViewStateEnabled</code> | <code>Control.IsViewStateEnabled = &quot;true&quot;</code> |
| <code>Api.Control.Load</code> | <code>Control.Load = &quot;added and removed&quot;</code> |
| <code>Api.Control.LoadControlState</code> | <code>Control.LoadControlState(Object) = &quot;ok&quot;</code><br><code>Control.LoadControlState(Object) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValu…</code> |
| <code>Api.Control.LoadViewState</code> | <code>Control.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Collections.ArrayList&#39;.&quot;</code> |
| <code>Api.Control.LoadViewStateByID</code> | <code>Control.LoadViewStateByID = &quot;false&quot;</code> |
| <code>Api.Control.MapPathSecure</code> | <code>Control.MapPathSecure(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.NamingContainer</code> | <code>Control.NamingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code> |
| <code>Api.Control.OnBubbleEvent</code> | <code>Control.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Control.OnBubbleEvent(Object,EventArgs) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;…</code> |
| <code>Api.Control.OnDataBinding</code> | <code>Control.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnDataBinding(EventArgs) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValu…</code> |
| <code>Api.Control.OnInit</code> | <code>Control.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnInit(EventArgs) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue&quot;; Ski…</code> |
| <code>Api.Control.OnLoad</code> | <code>Control.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnLoad(EventArgs) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue&quot;; Ski…</code> |
| <code>Api.Control.OnPreRender</code> | <code>Control.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnPreRender(EventArgs) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue&quot;…</code> |
| <code>Api.Control.OnUnload</code> | <code>Control.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnUnload(EventArgs) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue&quot;; S…</code> |
| <code>Api.Control.OpenFile</code> | <code>Control.OpenFile(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.Page</code> | <code>Control.Page = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=&quot;&quot;; …</code><br><code>Control.Page set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=…</code> |
| <code>Api.Control.Parent</code> | <code>Control.Parent = HtmlForm#form1 {Action=&quot;&quot;; ClientID=&quot;form1&quot;; ClientIDMode=Inherit; DefaultButton=&quot;&quot;; DefaultFocus=&quot;&quot;; Disabled=false; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.PreRender</code> | <code>Control.PreRender = &quot;added and removed&quot;</code> |
| <code>Api.Control.RaiseBubbleEvent</code> | <code>Control.RaiseBubbleEvent(Object,EventArgs) = &quot;ok&quot;</code><br><code>Control.RaiseBubbleEvent(Object,EventArgs) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; I…</code> |
| <code>Api.Control.RemovedControl</code> | <code>Control.RemovedControl(Control) = &quot;ok&quot;</code><br><code>Control.RemovedControl(Control) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue…</code> |
| <code>Api.Control.Render</code> | <code>Control.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.Render(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; value=&quot;initial&quot; /&gt;</code><br><code>Control.Render(HtmlTextWriter) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue&quot;…</code> |
| <code>Api.Control.RenderChildren</code> | <code>Control.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderChildren(HtmlTextWriter) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;h…</code> |
| <code>Api.Control.RenderControl</code> | <code>Control.RenderControl(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; value=&quot;initial&quot; /&gt;</code><br><code>Control.RenderControl(HtmlTextWriter) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hi…</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) writer:</code><br><code>  &#124; &lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; value=&quot;initial&quot; /&gt;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; Enab…</code> |
| <code>Api.Control.RenderingCompatibility</code> | <code>Control.RenderingCompatibility = &quot;System.Version 4.0&quot;</code><br><code>Control.RenderingCompatibility set = &quot;System.Version 4.0&quot;</code> |
| <code>Api.Control.ResolveAdapter</code> | <code>Control.ResolveAdapter = null</code><br><code>Control.ResolveAdapter after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue&quot;; SkinID…</code> |
| <code>Api.Control.ResolveClientUrl</code> | <code>Control.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveClientUrl(String) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValu…</code> |
| <code>Api.Control.ResolveUrl</code> | <code>Control.ResolveUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveUrl(String) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue&quot;; Sk…</code> |
| <code>Api.Control.SaveControlState</code> | <code>Control.SaveControlState = null</code><br><code>Control.SaveControlState after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue&quot;; Skin…</code> |
| <code>Api.Control.SaveViewState</code> | <code>Control.SaveViewState = null</code><br><code>Control.SaveViewState after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue&quot;; SkinID=…</code> |
| <code>Api.Control.SetDesignModeState</code> | <code>Control.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>Control.SetDesignModeState(IDictionary) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;…</code> |
| <code>Api.Control.SetRenderMethodDelegate</code> | <code>Control.SetRenderMethodDelegate(RenderMethod) = &quot;ok&quot;</code><br><code>Control.SetRenderMethodDelegate(RenderMethod) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true…</code> |
| <code>Api.Control.SetTraceData</code> | <code>Control.SetTraceData(Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidV…</code><br><code>Control.SetTraceData(Object,Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object,Object) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; I…</code> |
| <code>Api.Control.Site</code> | <code>Control.Site = null</code><br><code>Control.Site set = null</code> |
| <code>Api.Control.SkinID</code> | <code>Control.SkinID = &quot;&quot;</code><br><code>Control.SkinID set = &quot;-&gt; System.NotSupportedException: Control of type &#39;HiddenField&#39; does not support theming.&quot;</code> |
| <code>Api.Control.TemplateControl</code> | <code>Control.TemplateControl = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code><br><code>Control.TemplateControl set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Cl…</code> |
| <code>Api.Control.TemplateSourceDirectory</code> | <code>Control.TemplateSourceDirectory = &quot;&quot;</code> |
| <code>Api.Control.TrackViewState</code> | <code>Control.TrackViewState = &quot;ok&quot;</code><br><code>Control.TrackViewState after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidValue&quot;; SkinID…</code> |
| <code>Api.Control.UniqueID</code> | <code>Control.UniqueID = &quot;hidValue&quot;</code> |
| <code>Api.Control.Unload</code> | <code>Control.Unload = &quot;added and removed&quot;</code> |
| <code>Api.Control.ValidateRequestMode</code> | <code>Control.ValidateRequestMode = &quot;ValidateRequestMode.Inherit&quot;</code><br><code>Control.ValidateRequestMode set = &quot;ValidateRequestMode.Disabled&quot;</code> |
| <code>Api.Control.ViewState</code> | <code>Control.ViewState = StateBag [Value=&quot;initial&quot;]</code> |
| <code>Api.Control.ViewStateIgnoresCase</code> | <code>Control.ViewStateIgnoresCase = &quot;false&quot;</code> |
| <code>Api.Control.ViewStateMode</code> | <code>Control.ViewStateMode = &quot;ViewStateMode.Inherit&quot;</code><br><code>Control.ViewStateMode set = &quot;ViewStateMode.Enabled&quot;</code> |
| <code>Api.Control.Visible</code> | <code>Control.Visible = &quot;true&quot;</code><br><code>Control.Visible set = &quot;true&quot;</code> |
| <code>Api.IControlBuilderAccessor.ControlBuilder</code> | <code>IControlBuilderAccessor.ControlBuilder = null</code> |
| <code>Api.IControlDesignerAccessor.GetDesignModeState</code> | <code>IControlDesignerAccessor.GetDesignModeState = &quot;[]&quot;</code><br><code>IControlDesignerAccessor.GetDesignModeState after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; …</code> |
| <code>Api.IControlDesignerAccessor.SetDesignModeState</code> | <code>IControlDesignerAccessor.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetDesignModeState(IDictionary) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableVie…</code> |
| <code>Api.IControlDesignerAccessor.SetOwnerControl</code> | <code>IControlDesignerAccessor.SetOwnerControl(Control) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetOwnerControl(Control) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=…</code> |
| <code>Api.IControlDesignerAccessor.UserData</code> | <code>IControlDesignerAccessor.UserData = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.DataBindings</code> | <code>IDataBindingsAccessor.DataBindings = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.HasDataBindings</code> | <code>IDataBindingsAccessor.HasDataBindings = &quot;false&quot;</code> |
| <code>Api.IExpressionsAccessor.Expressions</code> | <code>IExpressionsAccessor.Expressions = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.HasExpressions</code> | <code>IExpressionsAccessor.HasExpressions = &quot;false&quot;</code> |
| <code>Api.IParserAccessor.AddParsedSubObject</code> | <code>IParserAccessor.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>IParserAccessor.AddParsedSubObject(Object) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; I…</code> |
| <code>Api.IPostBackDataHandler.LoadPostData</code> | <code>IPostBackDataHandler.LoadPostData(String,NameValueCollection) = &quot;true&quot;</code><br><code>IPostBackDataHandler.LoadPostData(String,NameValueCollection) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; Enab…</code> |
| <code>Api.IPostBackDataHandler.RaisePostDataChangedEvent</code> | <code>IPostBackDataHandler.RaisePostDataChangedEvent = &quot;ok&quot;</code><br><code>IPostBackDataHandler.RaisePostDataChangedEvent after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=tru…</code> |
| <code>Api.IUrlResolutionService.ResolveClientUrl</code> | <code>IUrlResolutionService.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>IUrlResolutionService.ResolveClientUrl(String) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=tru…</code> |
| <code>Api.WebControls.HiddenField.#ctor</code> | <code>HiddenField.#ctor = HiddenField# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateReque…</code> |
| <code>Api.WebControls.HiddenField.CreateControlCollection</code> | <code>HiddenField.CreateControlCollection = ControlCollection []</code><br><code>HiddenField.CreateControlCollection after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidV…</code> |
| <code>Api.WebControls.HiddenField.EnableTheming</code> | <code>HiddenField.EnableTheming = &quot;false&quot;</code><br><code>HiddenField.EnableTheming set = &quot;-&gt; System.NotSupportedException: Control of type &#39;HiddenField&#39; does not support theming.&quot;</code> |
| <code>Api.WebControls.HiddenField.Focus</code> | <code>HiddenField.Focus = &quot;-&gt; System.NotSupportedException: Control of type &#39;HiddenField&#39; does not support the Focus operation.&quot;</code> |
| <code>Api.WebControls.HiddenField.LoadPostData</code> | <code>HiddenField.LoadPostData(String,NameValueCollection) = &quot;true&quot;</code><br><code>HiddenField.LoadPostData(String,NameValueCollection) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewSta…</code> |
| <code>Api.WebControls.HiddenField.OnPreRender</code> | <code>HiddenField.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>HiddenField.OnPreRender(EventArgs) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidVa…</code> |
| <code>Api.WebControls.HiddenField.OnValueChanged</code> | <code>HiddenField.OnValueChanged(EventArgs) = &quot;ok&quot;</code><br><code>HiddenField.OnValueChanged(EventArgs) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hi…</code> |
| <code>Api.WebControls.HiddenField.RaisePostDataChangedEvent</code> | <code>HiddenField.RaisePostDataChangedEvent = &quot;ok&quot;</code><br><code>HiddenField.RaisePostDataChangedEvent after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hi…</code> |
| <code>Api.WebControls.HiddenField.Render</code> | <code>HiddenField.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>HiddenField.Render(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; value=&quot;initial&quot; /&gt;</code><br><code>HiddenField.Render(HtmlTextWriter) after = HiddenField#hidValue {ClientID=&quot;hidValue&quot;; ClientIDMode=Inherit; EnableTheming=false; EnableViewState=true; ID=&quot;hidVa…</code> |
| <code>Api.WebControls.HiddenField.SkinID</code> | <code>HiddenField.SkinID = &quot;&quot;</code><br><code>HiddenField.SkinID set = &quot;-&gt; System.NotSupportedException: Control of type &#39;HiddenField&#39; does not support theming.&quot;</code> |
| <code>Api.WebControls.HiddenField.Value</code> | <code>HiddenField.Value = &quot;initial&quot;</code><br><code>HiddenField.Value set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.HiddenField.ValueChanged</code> | <code>HiddenField.ValueChanged = &quot;added and removed&quot;</code> |

## ② HTML 層

プローブサイトの RenderProbe.aspx で、1 ケース 1 リクエスト（ページライフサイクル全体）で描画したページ全体を比較します。下表はコンポーネント部分（div#probe）、設定時の例外、保存された ViewState の要約です。

| ケース | 設定 | 期待される描画 | 設定時の例外 |
|---|---|---|---|
| <code>.(default)</code> | <code>(nothing set)</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; /&gt;</code> |  |
| <code>.AppRelativeTemplateSourceDirectory=text</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; /&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;probe&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=markup</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; /&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=japanese</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; /&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;日本語テキスト&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=empty</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&quot;;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; /&gt;</code> |  |
| <code>.ClientIDMode=Inherit</code> | <code>c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; /&gt;</code> |  |
| <code>.ClientIDMode=AutoID</code> | <code>c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; /&gt;</code> |  |
| <code>.ClientIDMode=Predictable</code> | <code>c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; /&gt;</code> |  |
| <code>.ClientIDMode=Static</code> | <code>c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; /&gt;</code> |  |
| <code>.EnableTheming=true</code> | <code>c.EnableTheming = true;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; /&gt;</code> | <code>c.EnableTheming = true; -&gt; System.NotSupportedException: Control of type &#39;HiddenField&#39; does not support theming.</code> |
| <code>.EnableTheming=false</code> | <code>c.EnableTheming = false;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; /&gt;</code> | <code>c.EnableTheming = false; -&gt; System.NotSupportedException: Control of type &#39;HiddenField&#39; does not support theming.</code> |
| <code>.EnableViewState=true</code> | <code>c.EnableViewState = true;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; /&gt;</code> |  |
| <code>.EnableViewState=false</code> | <code>c.EnableViewState = false;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; /&gt;</code> |  |
| <code>.ID=text</code> | <code>c.ID = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;probe&quot; id=&quot;probe&quot; /&gt;</code> |  |
| <code>.ID=markup</code> | <code>c.ID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; id=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; /&gt;</code> |  |
| <code>.ID=japanese</code> | <code>c.ID = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;日本語テキスト&quot; id=&quot;日本語テキスト&quot; /&gt;</code> |  |
| <code>.ID=empty</code> | <code>c.ID = &quot;&quot;;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;ctl02&quot; id=&quot;ctl02&quot; /&gt;</code> |  |
| <code>.SkinID=text</code> | <code>c.SkinID = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; /&gt;</code> | <code>c.SkinID = &quot;probe&quot;; -&gt; System.NotSupportedException: Control of type &#39;HiddenField&#39; does not support theming.</code> |
| <code>.SkinID=markup</code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; /&gt;</code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.NotSupportedException: Control of type &#39;HiddenField&#39; does not support theming.</code> |
| <code>.SkinID=japanese</code> | <code>c.SkinID = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; /&gt;</code> | <code>c.SkinID = &quot;日本語テキスト&quot;; -&gt; System.NotSupportedException: Control of type &#39;HiddenField&#39; does not support theming.</code> |
| <code>.SkinID=empty</code> | <code>c.SkinID = &quot;&quot;;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; /&gt;</code> | <code>c.SkinID = &quot;&quot;; -&gt; System.NotSupportedException: Control of type &#39;HiddenField&#39; does not support theming.</code> |
| <code>.ValidateRequestMode=Inherit</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Inherit;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; /&gt;</code> |  |
| <code>.ValidateRequestMode=Disabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Disabled;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; /&gt;</code> |  |
| <code>.ValidateRequestMode=Enabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Enabled;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; /&gt;</code> |  |
| <code>.Value=text</code> | <code>c.Value = &quot;probe&quot;;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; value=&quot;probe&quot; /&gt;</code> |  |
| <code>.Value=markup</code> | <code>c.Value = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; value=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; /&gt;</code> |  |
| <code>.Value=japanese</code> | <code>c.Value = &quot;日本語テキスト&quot;;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; value=&quot;日本語テキスト&quot; /&gt;</code> |  |
| <code>.Value=empty</code> | <code>c.Value = &quot;&quot;;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; /&gt;</code> |  |
| <code>.ViewStateMode=Inherit</code> | <code>c.ViewStateMode = ViewStateMode.Inherit;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; /&gt;</code> |  |
| <code>.ViewStateMode=Enabled</code> | <code>c.ViewStateMode = ViewStateMode.Enabled;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; /&gt;</code> |  |
| <code>.ViewStateMode=Disabled</code> | <code>c.ViewStateMode = ViewStateMode.Disabled;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; /&gt;</code> |  |
| <code>.Visible=true</code> | <code>c.Visible = true;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; /&gt;</code> |  |
| <code>.Visible=false</code> | <code>c.Visible = false;</code> | <code></code> |  |
| <code>.Value=markup+EnableViewState=false</code> | <code>c.Value = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; c.EnableViewState = false;</code> | <code>&lt;input type=&quot;hidden&quot; name=&quot;hidValue&quot; id=&quot;hidValue&quot; value=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; /&gt;</code> |  |
| <code>.Value=text+Visible=false</code> | <code>c.Value = &quot;probe&quot;; c.Visible = false;</code> | <code></code> |  |
| <code>[naming container].(default)</code> | <code>[naming container] (nothing set)</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input type=&quot;hidden&quot; name=&quot;box$hidValue&quot; id=&quot;box_hidValue&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=AutoID</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input type=&quot;hidden&quot; name=&quot;box$hidValue&quot; id=&quot;box_hidValue&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Static</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input type=&quot;hidden&quot; name=&quot;box$hidValue&quot; id=&quot;hidValue&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Predictable</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input type=&quot;hidden&quot; name=&quot;box$hidValue&quot; id=&quot;box_hidValue&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Inherit</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input type=&quot;hidden&quot; name=&quot;box$hidValue&quot; id=&quot;box_hidValue&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ID=text</code> | <code>[naming container] c.ID = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input type=&quot;hidden&quot; name=&quot;box$probe&quot; id=&quot;box_probe&quot; /&gt;↵&lt;/div&gt;</code> |  |

## ③ シナリオ層

ブラウザ（Playwright）で HiddenFieldScenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。

| # | 操作 | 対象 | 値 |
|---|---|---|---|
| 1 | goto | /BasicsScenario.aspx | <code></code> |
| 2 | snapshot |  | **HiddenField [render][databind][dynamic] first request** |
| 3 | setvalue | hidPlain | <code>changed</code> |
| 4 | setvalue | hidDynamic | <code>changed dynamic</code> |
| 5 | click | btnSubmit | <code></code> |
| 6 | snapshot |  | **HiddenField [postdata][change][dynamic] values changed by script** |
| 7 | click | btnSubmit | <code></code> |
| 8 | snapshot |  | **HiddenField [viewstate][change] posted again unchanged** |
| 9 | click | btnShow | <code></code> |
| 10 | snapshot |  | **HiddenField [visible][viewstate] hidden ones shown with what was set while hidden** |
| 11 | setvalue | hidBind | <code>s2</code> |
| 12 | click | btnUpdate | <code></code> |
| 13 | snapshot |  | **HiddenField [bind] two-way binding in a template** |
| 14 | setvalue | hidPlain | <code>&lt;b&gt;x&lt;/b&gt;</code> |
| 15 | click | btnSubmit | <code></code> |
| 16 | snapshot |  | **HiddenField [requestvalidation] markup posted** |

<details><summary>HiddenField [render][databind][dynamic] first request（期待される本文）</summary>

```text
Label: Plain bound Associated set in code set in code, no view state
Literal: <b>encoded</b> pass bound set in code set in code, no view state
CheckBox: Plain Auto Auto validates g1 Disabled Bound No view state
RadioButton: Small Large Auto A Auto B Auto validates g1 Bound
HiddenField:
LinkButton: Submit link Cancelled link Validate g1 by a link Disabled link bound Link to the target
HyperLink: Go to the target bound Disabled hyperlink
Image:
ImageButton:
Group
In a panel:
bound
Bound panel
Default link: Other link Default link
Default image:
PlaceHolder: Created in a place holder: bound
Dynamic: dynamic labeldynamic literalDynamicDynamicDynamic linkdynamic hyperlink
In a dynamic panel
Pen Pick Pen Bag Pick Bag
Taro Flag Choice
Required (g1, server): Cross-page:
Request 1: IsPostBack=False
State lblPlain Text="Plain"
State lblBound Text="bound"
State lblCode Text="set in code"
State lblNoViewState Text="set in code, no view state"
State lblHidden Text="set while hidden" (hidden)
State litBound Text="bound"
State litCode Text="set in code"
State litNoViewState Text="set in code, no view state"
State litHidden Text="set while hidden" (hidden)
State chkPlain Checked=False
State chkAuto Checked=False
State chkCauses Checked=False
State chkDisabled Checked=True
State chkBound Checked=True
State chkNoViewState Checked=True
State chkHidden Checked=True (hidden)
State rdoSmall Checked=False
State rdoLarge Checked=False
State rdoAutoA Checked=False
State rdoAutoB Checked=False
State rdoCauses Checked=False
State rdoBound Checked=True
State rdoHidden Checked=True (hidden)
State hidPlain Value="initial"
State hidBound Value="bound"
State hidNoViewState Value="set in code, no view state"
State hidHidden Value="set while hidden" (hidden)
State lnkBound Text="bound"
State lnkHidden Text="Hidden link (set while hidden)" (hidden)
State hypBound Text="bound" NavigateUrl="~/ButtonTarget.aspx?id=7"
State hypHidden Text="Hidden hyperlink" NavigateUrl="~/ButtonTarget.aspx?hidden=1" (hidden)
State imgBound AlternateText="bound"
State imgCode AlternateText="set in code"
State imgHidden AlternateText="set while hidden" (hidden)
State ibtnBound AlternateText="bound"
State ibtnHidden AlternateText="Hidden image (set while hidden)" (hidden)
State txtInPanel Text=""
State txtInHiddenPanel Text="set while hidden" (hidden)
State txtInHolder Text=""
State lblInHiddenHolder Text="set while hidden" (hidden)
State litInBoundHolder Text="bound"
State lblDynamic Text="dynamic label"
State litDynamic Text="dynamic literal"
State chkDynamic Checked=False
State rdoDynamic Checked=False
State hidDynamic Value="dynamic"
State lnkDynamic Text="Dynamic link"
State hypDynamic Text="dynamic hyperlink" NavigateUrl="~/ButtonTarget.aspx"
State imgDynamic AlternateText="dynamic image"
State ibtnDynamic AlternateText="Dynamic image"
```

</details>

<details><summary>HiddenField [postdata][change][dynamic] values changed by script（期待される本文）</summary>

```text
Label: Plain bound Associated set in code
Literal: <b>encoded</b> pass bound set in code
CheckBox: Plain Auto Auto validates g1 Disabled Bound No view state
RadioButton: Small Large Auto A Auto B Auto validates g1 Bound
HiddenField:
LinkButton: Submit link Cancelled link Validate g1 by a link Disabled link bound Link to the target
HyperLink: Go to the target bound Disabled hyperlink
Image:
ImageButton:
Group
In a panel:
bound
Bound panel
Default link: Other link Default link
Default image:
PlaceHolder: Created in a place holder: bound
Dynamic: dynamic labeldynamic literalDynamicDynamicDynamic linkdynamic hyperlink
In a dynamic panel
Pen Pick Pen Bag Pick Bag
Taro Flag Choice
Required (g1, server): Cross-page:
Request 2: IsPostBack=True
Changed chkNoViewState Checked=True
Changed hidPlain Value="changed"
Changed hidNoViewState Value="set in code, no view state"
Changed hidDynamic Value="changed dynamic"
IsValid: (not validated)
State lblPlain Text="Plain"
State lblBound Text="bound"
State lblCode Text="set in code"
State lblNoViewState Text=""
State lblHidden Text="set while hidden" (hidden)
State litBound Text="bound"
State litCode Text="set in code"
State litNoViewState Text=""
State litHidden Text="set while hidden" (hidden)
State chkPlain Checked=False
State chkAuto Checked=False
State chkCauses Checked=False
State chkDisabled Checked=True
State chkBound Checked=True
State chkNoViewState Checked=True
State chkHidden Checked=True (hidden)
State rdoSmall Checked=False
State rdoLarge Checked=False
State rdoAutoA Checked=False
State rdoAutoB Checked=False
State rdoCauses Checked=False
State rdoBound Checked=True
State rdoHidden Checked=True (hidden)
State hidPlain Value="changed"
State hidBound Value="bound"
State hidNoViewState Value="set in code, no view state"
State hidHidden Value="set while hidden" (hidden)
State lnkBound Text="bound"
State lnkHidden Text="Hidden link (set while hidden)" (hidden)
State hypBound Text="bound" NavigateUrl="~/ButtonTarget.aspx?id=7"
State hypHidden Text="Hidden hyperlink" NavigateUrl="~/ButtonTarget.aspx?hidden=1" (hidden)
State imgBound AlternateText="bound"
State imgCode AlternateText="set in code"
State imgHidden AlternateText="set while hidden" (hidden)
State ibtnBound AlternateText="bound"
State ibtnHidden AlternateText="Hidden image (set while hidden)" (hidden)
State txtInPanel Text=""
State txtInHiddenPanel Text="set while hidden" (hidden)
State txtInHolder Text=""
State lblInHiddenHolder Text="set while hidden" (hidden)
State litInBoundHolder Text="bound"
State lblDynamic Text="dynamic label"
State litDynamic Text="dynamic literal"
State chkDynamic Checked=False
State rdoDynamic Checked=False
State hidDynamic Value="changed dynamic"
State lnkDynamic Text="Dynamic link"
State hypDynamic Text="dynamic hyperlink" NavigateUrl="~/ButtonTarget.aspx"
State imgDynamic AlternateText="dynamic image"
State ibtnDynamic AlternateText="Dynamic image"
```

</details>

<details><summary>HiddenField [viewstate][change] posted again unchanged（期待される本文）</summary>

```text
Label: Plain bound Associated set in code
Literal: <b>encoded</b> pass bound set in code
CheckBox: Plain Auto Auto validates g1 Disabled Bound No view state
RadioButton: Small Large Auto A Auto B Auto validates g1 Bound
HiddenField:
LinkButton: Submit link Cancelled link Validate g1 by a link Disabled link bound Link to the target
HyperLink: Go to the target bound Disabled hyperlink
Image:
ImageButton:
Group
In a panel:
bound
Bound panel
Default link: Other link Default link
Default image:
PlaceHolder: Created in a place holder: bound
Dynamic: dynamic labeldynamic literalDynamicDynamicDynamic linkdynamic hyperlink
In a dynamic panel
Pen Pick Pen Bag Pick Bag
Taro Flag Choice
Required (g1, server): Cross-page:
Request 3: IsPostBack=True
Changed chkNoViewState Checked=True
Changed hidNoViewState Value="set in code, no view state"
IsValid: (not validated)
State lblPlain Text="Plain"
State lblBound Text="bound"
State lblCode Text="set in code"
State lblNoViewState Text=""
State lblHidden Text="set while hidden" (hidden)
State litBound Text="bound"
State litCode Text="set in code"
State litNoViewState Text=""
State litHidden Text="set while hidden" (hidden)
State chkPlain Checked=False
State chkAuto Checked=False
State chkCauses Checked=False
State chkDisabled Checked=True
State chkBound Checked=True
State chkNoViewState Checked=True
State chkHidden Checked=True (hidden)
State rdoSmall Checked=False
State rdoLarge Checked=False
State rdoAutoA Checked=False
State rdoAutoB Checked=False
State rdoCauses Checked=False
State rdoBound Checked=True
State rdoHidden Checked=True (hidden)
State hidPlain Value="changed"
State hidBound Value="bound"
State hidNoViewState Value="set in code, no view state"
State hidHidden Value="set while hidden" (hidden)
State lnkBound Text="bound"
State lnkHidden Text="Hidden link (set while hidden)" (hidden)
State hypBound Text="bound" NavigateUrl="~/ButtonTarget.aspx?id=7"
State hypHidden Text="Hidden hyperlink" NavigateUrl="~/ButtonTarget.aspx?hidden=1" (hidden)
State imgBound AlternateText="bound"
State imgCode AlternateText="set in code"
State imgHidden AlternateText="set while hidden" (hidden)
State ibtnBound AlternateText="bound"
State ibtnHidden AlternateText="Hidden image (set while hidden)" (hidden)
State txtInPanel Text=""
State txtInHiddenPanel Text="set while hidden" (hidden)
State txtInHolder Text=""
State lblInHiddenHolder Text="set while hidden" (hidden)
State litInBoundHolder Text="bound"
State lblDynamic Text="dynamic label"
State litDynamic Text="dynamic literal"
State chkDynamic Checked=False
State rdoDynamic Checked=False
State hidDynamic Value="changed dynamic"
State lnkDynamic Text="Dynamic link"
State hypDynamic Text="dynamic hyperlink" NavigateUrl="~/ButtonTarget.aspx"
State imgDynamic AlternateText="dynamic image"
State ibtnDynamic AlternateText="Dynamic image"
```

</details>

<details><summary>HiddenField [visible][viewstate] hidden ones shown with what was set while hidden（期待される本文）</summary>

```text
Label: Plain bound Associated set in code set while hidden
Literal: <b>encoded</b> pass bound set in code set while hidden
CheckBox: Plain Auto Auto validates g1 Disabled Bound No view state Hidden
RadioButton: Small Large Auto A Auto B Auto validates g1 Bound Hidden
HiddenField:
LinkButton: Submit link Cancelled link Validate g1 by a link Disabled link bound Link to the target Hidden link (set while hidden)
HyperLink: Go to the target bound Disabled hyperlink Hidden hyperlink
Image:
ImageButton:
Group
In a panel:
bound
Bound panel
In a hidden panel:
Default link: Other link Default link
Default image:
PlaceHolder: Created in a place holder: bound set while hidden
Dynamic: dynamic labeldynamic literalDynamicDynamicDynamic linkdynamic hyperlink
In a dynamic panel
Pen Pick Pen Bag Pick Bag
Taro Flag Choice
Required (g1, server): Cross-page:
Request 4: IsPostBack=True
Changed chkNoViewState Checked=True
Changed hidNoViewState Value="set in code, no view state"
Click btnShow
IsValid: (not validated)
State lblPlain Text="Plain"
State lblBound Text="bound"
State lblCode Text="set in code"
State lblNoViewState Text=""
State lblHidden Text="set while hidden"
State litBound Text="bound"
State litCode Text="set in code"
State litNoViewState Text=""
State litHidden Text="set while hidden"
State chkPlain Checked=False
State chkAuto Checked=False
State chkCauses Checked=False
State chkDisabled Checked=True
State chkBound Checked=True
State chkNoViewState Checked=True
State chkHidden Checked=True
State rdoSmall Checked=False
State rdoLarge Checked=False
State rdoAutoA Checked=False
State rdoAutoB Checked=False
State rdoCauses Checked=False
State rdoBound Checked=True
State rdoHidden Checked=True
State hidPlain Value="changed"
State hidBound Value="bound"
State hidNoViewState Value="set in code, no view state"
State hidHidden Value="set while hidden"
State lnkBound Text="bound"
State lnkHidden Text="Hidden link (set while hidden)"
State hypBound Text="bound" NavigateUrl="~/ButtonTarget.aspx?id=7"
State hypHidden Text="Hidden hyperlink" NavigateUrl="~/ButtonTarget.aspx?hidden=1"
State imgBound AlternateText="bound"
State imgCode AlternateText="set in code"
State imgHidden AlternateText="set while hidden"
State ibtnBound AlternateText="bound"
State ibtnHidden AlternateText="Hidden image (set while hidden)"
State txtInPanel Text=""
State txtInHiddenPanel Text="set while hidden"
State txtInHolder Text=""
State lblInHiddenHolder Text="set while hidden"
State litInBoundHolder Text="bound"
State lblDynamic Text="dynamic label"
State litDynamic Text="dynamic literal"
State chkDynamic Checked=False
State rdoDynamic Checked=False
State hidDynamic Value="changed dynamic"
State lnkDynamic Text="Dynamic link"
State hypDynamic Text="dynamic hyperlink" NavigateUrl="~/ButtonTarget.aspx"
State imgDynamic AlternateText="dynamic image"
State ibtnDynamic AlternateText="Dynamic image"
```

</details>

<details><summary>HiddenField [bind] two-way binding in a template（期待される本文）</summary>

```text
Label: Plain bound Associated set in code set while hidden
Literal: <b>encoded</b> pass bound set in code set while hidden
CheckBox: Plain Auto Auto validates g1 Disabled Bound No view state Hidden
RadioButton: Small Large Auto A Auto B Auto validates g1 Bound Hidden
HiddenField:
LinkButton: Submit link Cancelled link Validate g1 by a link Disabled link bound Link to the target Hidden link (set while hidden)
HyperLink: Go to the target bound Disabled hyperlink Hidden hyperlink
Image:
ImageButton:
Group
In a panel:
bound
Bound panel
In a hidden panel:
Default link: Other link Default link
Default image:
PlaceHolder: Created in a place holder: bound set while hidden
Dynamic: dynamic labeldynamic literalDynamicDynamicDynamic linkdynamic hyperlink
In a dynamic panel
Pen Pick Pen Bag Pick Bag
Taro Flag Choice
Required (g1, server): Cross-page:
Request 5: IsPostBack=True
Changed chkNoViewState Checked=True
Changed hidNoViewState Value="set in code, no view state"
ItemUpdating: Choice=False, Flag=True, Name=Taro, Secret=s2
IsValid: (not validated)
State lblPlain Text="Plain"
State lblBound Text="bound"
State lblCode Text="set in code"
State lblNoViewState Text=""
State lblHidden Text="set while hidden"
State litBound Text="bound"
State litCode Text="set in code"
State litNoViewState Text=""
State litHidden Text="set while hidden"
State chkPlain Checked=False
State chkAuto Checked=False
State chkCauses Checked=False
State chkDisabled Checked=True
State chkBound Checked=True
State chkNoViewState Checked=True
State chkHidden Checked=True
State rdoSmall Checked=False
State rdoLarge Checked=False
State rdoAutoA Checked=False
State rdoAutoB Checked=False
State rdoCauses Checked=False
State rdoBound Checked=True
State rdoHidden Checked=True
State hidPlain Value="changed"
State hidBound Value="bound"
State hidNoViewState Value="set in code, no view state"
State hidHidden Value="set while hidden"
State lnkBound Text="bound"
State lnkHidden Text="Hidden link (set while hidden)"
State hypBound Text="bound" NavigateUrl="~/ButtonTarget.aspx?id=7"
State hypHidden Text="Hidden hyperlink" NavigateUrl="~/ButtonTarget.aspx?hidden=1"
State imgBound AlternateText="bound"
State imgCode AlternateText="set in code"
State imgHidden AlternateText="set while hidden"
State ibtnBound AlternateText="bound"
State ibtnHidden AlternateText="Hidden image (set while hidden)"
State txtInPanel Text=""
State txtInHiddenPanel Text="set while hidden"
State txtInHolder Text=""
State lblInHiddenHolder Text="set while hidden"
State litInBoundHolder Text="bound"
State lblDynamic Text="dynamic label"
State litDynamic Text="dynamic literal"
State chkDynamic Checked=False
State rdoDynamic Checked=False
State hidDynamic Value="changed dynamic"
State lnkDynamic Text="Dynamic link"
State hypDynamic Text="dynamic hyperlink" NavigateUrl="~/ButtonTarget.aspx"
State imgDynamic AlternateText="dynamic image"
State ibtnDynamic AlternateText="Dynamic image"
```

</details>

<details><summary>HiddenField [requestvalidation] markup posted（期待される本文）</summary>

```text
System.Web.HttpRequestValidationException
A potentially dangerous Request.Form value was detected from the client (hidPlain="<b>x</b>").
```

</details>

