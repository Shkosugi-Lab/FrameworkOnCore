# SiteMapDataSource テストケース一覧

変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。
このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。

- 正解データの採取: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0
- ① API 層: 136 ケース（143 メンバー）
- ② HTML 層: 56 ケース
- ③ シナリオ層: 12 スナップショット

## 機構マトリクス

型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。

| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |
|---|---|---|---|
| プロパティ → HTML 描画 | すべてのコントロール | 49 | 1 |
| ViewState の保存と復元 | Control | 5 | 11 |
| Visible=false（描画されず状態は保持） | Control.Visible | 2 | 1 |
| 名前付けコンテナと ClientIDMode | Control.ClientIDMode | 6 | 0 |
| 動的に生成したコントロールのライフサイクル | Control | 0 | 1 |
| データバインド（DataBind / <%# %>） | Control.DataBind | 0 | 1 |

該当しない機構: テーマとスキン（SkinID / EnableTheming）、Enabled=false・Disabled=true（描画とポスト値の扱い）、ポストバックデータの取り込み、変更イベント（*Changed、ServerChange）、ReadOnly（ポストされた値を無視）、リクエスト検証（ValidateRequestMode）、ポストバックイベント、コマンドイベントと親へのバブル（CommandName）、送信ボタン／スクリプトによるポストバック（UseSubmitBehavior）、ブラウザ側のクリック処理（OnClientClick）、クロスページポストバック（PostBackUrl）、既定のボタン（Panel の DefaultButton）、AutoPostBack（クライアントスクリプトによるポストバック）、CausesValidation / ValidationGroup、検証対象（サーバー側の検証）、検証対象（ブラウザ側の検証）、双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値）、項目（ListItem の Selected・Enabled・Text・Value と ViewState）、複数選択、繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）、ポスト値のイベント検証（一覧にない値・無効な項目の拒否）、サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）、ブラウザ側の検証スクリプト（EnableClientScript）、検証結果のまとめ（ValidationSummary）、ページング、並べ替え、項目の挿入、行・項目の編集（編集・更新・取り消し・削除）、行・項目の選択、行・項目のコマンド（RowCommand・ItemCommand へのバブル）、データキー（DataKeyNames・DataKeys）、データソースコントロールとの連携（DataSourceID）、テンプレート、データソース連携（DataSource・DataTextField など）、クライアントコールバック

## ① API 層

SiteMapDataSource とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の SiteMapDataSource インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。

| ケース | 期待値（.NET Framework） |
|---|---|
| <code>Api.Control.#ctor</code> | <code>Control.#ctor = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMode=In…</code> |
| <code>Api.Control.Adapter</code> | <code>Control.Adapter = null</code> |
| <code>Api.Control.AddParsedSubObject</code> | <code>Control.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>Control.AddParsedSubObject(Object) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=fal…</code> |
| <code>Api.Control.AddedControl</code> | <code>Control.AddedControl(Control,Int32) = &quot;ok&quot;</code><br><code>Control.AddedControl(Control,Int32) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=fa…</code> |
| <code>Api.Control.AppRelativeTemplateSourceDirectory</code> | <code>Control.AppRelativeTemplateSourceDirectory = &quot;&quot;</code><br><code>Control.AppRelativeTemplateSourceDirectory set = &quot;-&gt; System.ArgumentException: The relative virtual path &#39;Abc 123&#39; is not allowed here.&quot;</code> |
| <code>Api.Control.ApplyStyleSheetSkin</code> | <code>Control.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>Control.ApplyStyleSheetSkin(Page) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=fals…</code> |
| <code>Api.Control.BeginRenderTracing</code> | <code>Control.BeginRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.BeginRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.BeginRenderTracing(TextWriter,Object) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; Enable…</code> |
| <code>Api.Control.BindingContainer</code> | <code>Control.BindingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Clien…</code> |
| <code>Api.Control.BuildProfileTree</code> | <code>Control.BuildProfileTree(String,Boolean) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.ChildControlsCreated</code> | <code>Control.ChildControlsCreated = &quot;false&quot;</code><br><code>Control.ChildControlsCreated set = &quot;ok&quot;</code> |
| <code>Api.Control.ClearCachedClientID</code> | <code>Control.ClearCachedClientID = &quot;ok&quot;</code><br><code>Control.ClearCachedClientID after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; Ena…</code> |
| <code>Api.Control.ClearChildControlState</code> | <code>Control.ClearChildControlState = &quot;ok&quot;</code><br><code>Control.ClearChildControlState after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; …</code> |
| <code>Api.Control.ClearChildState</code> | <code>Control.ClearChildState = &quot;ok&quot;</code><br><code>Control.ClearChildState after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; EnableV…</code> |
| <code>Api.Control.ClearChildViewState</code> | <code>Control.ClearChildViewState = &quot;ok&quot;</code><br><code>Control.ClearChildViewState after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; Ena…</code> |
| <code>Api.Control.ClearEffectiveClientIDMode</code> | <code>Control.ClearEffectiveClientIDMode = &quot;ok&quot;</code><br><code>Control.ClearEffectiveClientIDMode after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=fal…</code> |
| <code>Api.Control.ClientID</code> | <code>Control.ClientID = &quot;smdsSite&quot;</code> |
| <code>Api.Control.ClientIDMode</code> | <code>Control.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>Control.ClientIDMode set = &quot;-&gt; System.NotSupportedException: Specified method is not supported.&quot;</code> |
| <code>Api.Control.ClientIDSeparator</code> | <code>Control.ClientIDSeparator = &quot;&#39;_&#39;&quot;</code> |
| <code>Api.Control.Context</code> | <code>Control.Context = null</code> |
| <code>Api.Control.Controls</code> | <code>Control.Controls = ControlCollection []</code> |
| <code>Api.Control.CreateChildControls</code> | <code>Control.CreateChildControls = &quot;ok&quot;</code><br><code>Control.CreateChildControls after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; Ena…</code> |
| <code>Api.Control.CreateControlCollection</code> | <code>Control.CreateControlCollection = ControlCollection []</code><br><code>Control.CreateControlCollection after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false;…</code> |
| <code>Api.Control.DataBind</code> | <code>Control.DataBind = &quot;ok&quot;</code><br><code>Control.DataBind after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; EnableViewStat…</code><br><code>Control.DataBind(Boolean) = &quot;ok&quot;</code><br><code>Control.DataBind(Boolean) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; Enabl…</code> |
| <code>Api.Control.DataBindChildren</code> | <code>Control.DataBindChildren = &quot;ok&quot;</code><br><code>Control.DataBindChildren after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; Enable…</code> |
| <code>Api.Control.DataBinding</code> | <code>Control.DataBinding = &quot;added and removed&quot;</code> |
| <code>Api.Control.DataItemContainer</code> | <code>Control.DataItemContainer = null</code> |
| <code>Api.Control.DataKeysContainer</code> | <code>Control.DataKeysContainer = null</code> |
| <code>Api.Control.DesignMode</code> | <code>Control.DesignMode = &quot;false&quot;</code> |
| <code>Api.Control.Dispose</code> | <code>Control.Dispose = &quot;ok&quot;</code><br><code>Control.Dispose after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; EnableViewState…</code> |
| <code>Api.Control.Disposed</code> | <code>Control.Disposed = &quot;added and removed&quot;</code> |
| <code>Api.Control.EnableTheming</code> | <code>Control.EnableTheming = &quot;false&quot;</code><br><code>Control.EnableTheming set = &quot;-&gt; System.NotSupportedException: Control of type &#39;SiteMapDataSource&#39; does not support theming.&quot;</code> |
| <code>Api.Control.EnableViewState</code> | <code>Control.EnableViewState = &quot;true&quot;</code><br><code>Control.EnableViewState set = &quot;true&quot;</code> |
| <code>Api.Control.EndRenderTracing</code> | <code>Control.EndRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.EndRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.EndRenderTracing(TextWriter,Object) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTh…</code> |
| <code>Api.Control.EnsureChildControls</code> | <code>Control.EnsureChildControls = &quot;ok&quot;</code><br><code>Control.EnsureChildControls after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; Ena…</code> |
| <code>Api.Control.EnsureID</code> | <code>Control.EnsureID = &quot;ok&quot;</code><br><code>Control.EnsureID after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; EnableViewStat…</code> |
| <code>Api.Control.Events</code> | <code>Control.Events = &quot;System.ComponentModel.EventHandlerList&quot;</code> |
| <code>Api.Control.FindControl</code> | <code>Control.FindControl(String) = Label#lblTitle {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code><br><code>Control.FindControl(String) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; Ena…</code><br><code>Control.FindControl(String,Int32) = null</code><br><code>Control.FindControl(String,Int32) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=fals…</code> |
| <code>Api.Control.Focus</code> | <code>Control.Focus = &quot;-&gt; System.NotSupportedException: Control of type &#39;SiteMapDataSource&#39; does not support the Focus operation.&quot;</code> |
| <code>Api.Control.GetDesignModeState</code> | <code>Control.GetDesignModeState = &quot;[]&quot;</code><br><code>Control.GetDesignModeState after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; Enab…</code> |
| <code>Api.Control.GetRouteUrl</code> | <code>Control.GetRouteUrl(Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.GetUniqueIDRelativeTo</code> | <code>Control.GetUniqueIDRelativeTo(Control) = &quot;smdsSite&quot;</code><br><code>Control.GetUniqueIDRelativeTo(Control) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming…</code> |
| <code>Api.Control.HasChildViewState</code> | <code>Control.HasChildViewState = &quot;false&quot;</code> |
| <code>Api.Control.HasControls</code> | <code>Control.HasControls = &quot;false&quot;</code><br><code>Control.HasControls after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; EnableViewS…</code> |
| <code>Api.Control.HasEvents</code> | <code>Control.HasEvents = &quot;false&quot;</code><br><code>Control.HasEvents after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; EnableViewSta…</code> |
| <code>Api.Control.ID</code> | <code>Control.ID = &quot;smdsSite&quot;</code><br><code>Control.ID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.IdSeparator</code> | <code>Control.IdSeparator = &quot;&#39;$&#39;&quot;</code> |
| <code>Api.Control.Init</code> | <code>Control.Init = &quot;added and removed&quot;</code> |
| <code>Api.Control.IsChildControlStateCleared</code> | <code>Control.IsChildControlStateCleared = &quot;false&quot;</code> |
| <code>Api.Control.IsLiteralContent</code> | <code>Control.IsLiteralContent = &quot;false&quot;</code><br><code>Control.IsLiteralContent after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; Enable…</code> |
| <code>Api.Control.IsTrackingViewState</code> | <code>Control.IsTrackingViewState = &quot;false&quot;</code> |
| <code>Api.Control.IsViewStateEnabled</code> | <code>Control.IsViewStateEnabled = &quot;true&quot;</code> |
| <code>Api.Control.Load</code> | <code>Control.Load = &quot;added and removed&quot;</code> |
| <code>Api.Control.LoadControlState</code> | <code>Control.LoadControlState(Object) = &quot;ok&quot;</code><br><code>Control.LoadControlState(Object) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false…</code> |
| <code>Api.Control.LoadViewState</code> | <code>Control.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Collections.ArrayList&#39;.&quot;</code> |
| <code>Api.Control.LoadViewStateByID</code> | <code>Control.LoadViewStateByID = &quot;false&quot;</code> |
| <code>Api.Control.MapPathSecure</code> | <code>Control.MapPathSecure(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.NamingContainer</code> | <code>Control.NamingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code> |
| <code>Api.Control.OnBubbleEvent</code> | <code>Control.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Control.OnBubbleEvent(Object,EventArgs) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableThemin…</code> |
| <code>Api.Control.OnDataBinding</code> | <code>Control.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnDataBinding(EventArgs) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false…</code> |
| <code>Api.Control.OnInit</code> | <code>Control.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnInit(EventArgs) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; Enabl…</code> |
| <code>Api.Control.OnLoad</code> | <code>Control.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnLoad(EventArgs) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; Enabl…</code> |
| <code>Api.Control.OnPreRender</code> | <code>Control.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnPreRender(EventArgs) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; …</code> |
| <code>Api.Control.OnUnload</code> | <code>Control.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnUnload(EventArgs) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; Ena…</code> |
| <code>Api.Control.OpenFile</code> | <code>Control.OpenFile(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.Page</code> | <code>Control.Page = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=&quot;&quot;; …</code><br><code>Control.Page set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=…</code> |
| <code>Api.Control.Parent</code> | <code>Control.Parent = HtmlForm#form1 {Action=&quot;&quot;; ClientID=&quot;form1&quot;; ClientIDMode=Inherit; DefaultButton=&quot;&quot;; DefaultFocus=&quot;&quot;; Disabled=false; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.PreRender</code> | <code>Control.PreRender = &quot;added and removed&quot;</code> |
| <code>Api.Control.RaiseBubbleEvent</code> | <code>Control.RaiseBubbleEvent(Object,EventArgs) = &quot;ok&quot;</code><br><code>Control.RaiseBubbleEvent(Object,EventArgs) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableThe…</code> |
| <code>Api.Control.RemovedControl</code> | <code>Control.RemovedControl(Control) = &quot;ok&quot;</code><br><code>Control.RemovedControl(Control) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false;…</code> |
| <code>Api.Control.Render</code> | <code>Control.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.Render(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.Render(HtmlTextWriter) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; …</code> |
| <code>Api.Control.RenderChildren</code> | <code>Control.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderChildren(HtmlTextWriter) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming…</code> |
| <code>Api.Control.RenderControl</code> | <code>Control.RenderControl(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderControl(HtmlTextWriter) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=…</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollect…</code> |
| <code>Api.Control.RenderingCompatibility</code> | <code>Control.RenderingCompatibility = &quot;System.Version 4.0&quot;</code><br><code>Control.RenderingCompatibility set = &quot;System.Version 4.0&quot;</code> |
| <code>Api.Control.ResolveAdapter</code> | <code>Control.ResolveAdapter = null</code><br><code>Control.ResolveAdapter after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; EnableVi…</code> |
| <code>Api.Control.ResolveClientUrl</code> | <code>Control.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveClientUrl(String) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false…</code> |
| <code>Api.Control.ResolveUrl</code> | <code>Control.ResolveUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveUrl(String) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; Enab…</code> |
| <code>Api.Control.SaveControlState</code> | <code>Control.SaveControlState = null</code><br><code>Control.SaveControlState after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; Enable…</code> |
| <code>Api.Control.SaveViewState</code> | <code>Control.SaveViewState = null</code><br><code>Control.SaveViewState after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; EnableVie…</code> |
| <code>Api.Control.SetDesignModeState</code> | <code>Control.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>Control.SetDesignModeState(IDictionary) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableThemin…</code> |
| <code>Api.Control.SetRenderMethodDelegate</code> | <code>Control.SetRenderMethodDelegate(RenderMethod) = &quot;ok&quot;</code><br><code>Control.SetRenderMethodDelegate(RenderMethod) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; Enable…</code> |
| <code>Api.Control.SetTraceData</code> | <code>Control.SetTraceData(Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=fa…</code><br><code>Control.SetTraceData(Object,Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object,Object) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableThe…</code> |
| <code>Api.Control.Site</code> | <code>Control.Site = null</code><br><code>Control.Site set = null</code> |
| <code>Api.Control.SkinID</code> | <code>Control.SkinID = &quot;&quot;</code><br><code>Control.SkinID set = &quot;-&gt; System.NotSupportedException: Control of type &#39;SiteMapDataSource&#39; does not support theming.&quot;</code> |
| <code>Api.Control.TemplateControl</code> | <code>Control.TemplateControl = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code><br><code>Control.TemplateControl set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Cl…</code> |
| <code>Api.Control.TemplateSourceDirectory</code> | <code>Control.TemplateSourceDirectory = &quot;&quot;</code> |
| <code>Api.Control.TrackViewState</code> | <code>Control.TrackViewState = &quot;ok&quot;</code><br><code>Control.TrackViewState after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; EnableVi…</code> |
| <code>Api.Control.UniqueID</code> | <code>Control.UniqueID = &quot;smdsSite&quot;</code> |
| <code>Api.Control.Unload</code> | <code>Control.Unload = &quot;added and removed&quot;</code> |
| <code>Api.Control.ValidateRequestMode</code> | <code>Control.ValidateRequestMode = &quot;ValidateRequestMode.Inherit&quot;</code><br><code>Control.ValidateRequestMode set = &quot;ValidateRequestMode.Disabled&quot;</code> |
| <code>Api.Control.ViewState</code> | <code>Control.ViewState = StateBag []</code> |
| <code>Api.Control.ViewStateIgnoresCase</code> | <code>Control.ViewStateIgnoresCase = &quot;false&quot;</code> |
| <code>Api.Control.ViewStateMode</code> | <code>Control.ViewStateMode = &quot;ViewStateMode.Inherit&quot;</code><br><code>Control.ViewStateMode set = &quot;ViewStateMode.Enabled&quot;</code> |
| <code>Api.Control.Visible</code> | <code>Control.Visible = &quot;false&quot;</code><br><code>Control.Visible set = &quot;-&gt; System.NotSupportedException: Control of type &#39;SiteMapDataSource&#39; is a non-visual control and does not support setting the Visible pro…</code> |
| <code>Api.HierarchicalDataSourceControl.#ctor</code> | <code>HierarchicalDataSourceControl.#ctor = &quot;-&gt; System.MemberAccessException: Cannot create an instance of System.Web.UI.HierarchicalDataSourceControl because it is a…</code> |
| <code>Api.HierarchicalDataSourceControl.ApplyStyleSheetSkin</code> | <code>HierarchicalDataSourceControl.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>HierarchicalDataSourceControl.ApplyStyleSheetSkin(Page) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=tr…</code> |
| <code>Api.HierarchicalDataSourceControl.ClientID</code> | <code>HierarchicalDataSourceControl.ClientID = &quot;smdsSite&quot;</code> |
| <code>Api.HierarchicalDataSourceControl.ClientIDMode</code> | <code>HierarchicalDataSourceControl.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>HierarchicalDataSourceControl.ClientIDMode set = &quot;-&gt; System.NotSupportedException: Specified method is not supported.&quot;</code> |
| <code>Api.HierarchicalDataSourceControl.Controls</code> | <code>HierarchicalDataSourceControl.Controls = ControlCollection []</code> |
| <code>Api.HierarchicalDataSourceControl.CreateControlCollection</code> | <code>HierarchicalDataSourceControl.CreateControlCollection = ControlCollection []</code><br><code>HierarchicalDataSourceControl.CreateControlCollection after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true…</code> |
| <code>Api.HierarchicalDataSourceControl.EnableTheming</code> | <code>HierarchicalDataSourceControl.EnableTheming = &quot;false&quot;</code><br><code>HierarchicalDataSourceControl.EnableTheming set = &quot;-&gt; System.NotSupportedException: Control of type &#39;SiteMapDataSource&#39; does not support theming.&quot;</code> |
| <code>Api.HierarchicalDataSourceControl.FindControl</code> | <code>HierarchicalDataSourceControl.FindControl(String) = Label#lblTitle {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=C…</code><br><code>HierarchicalDataSourceControl.FindControl(String) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; En…</code> |
| <code>Api.HierarchicalDataSourceControl.Focus</code> | <code>HierarchicalDataSourceControl.Focus = &quot;-&gt; System.NotSupportedException: Control of type &#39;SiteMapDataSource&#39; does not support the Focus operation.&quot;</code> |
| <code>Api.HierarchicalDataSourceControl.GetHierarchicalView</code> | <code>HierarchicalDataSourceControl.GetHierarchicalView(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.HierarchicalDataSourceControl.HasControls</code> | <code>HierarchicalDataSourceControl.HasControls = &quot;false&quot;</code><br><code>HierarchicalDataSourceControl.HasControls after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableThem…</code> |
| <code>Api.HierarchicalDataSourceControl.OnDataSourceChanged</code> | <code>HierarchicalDataSourceControl.OnDataSourceChanged(EventArgs) = &quot;ok&quot;</code><br><code>HierarchicalDataSourceControl.OnDataSourceChanged(EventArgs) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollecti…</code> |
| <code>Api.HierarchicalDataSourceControl.RenderControl</code> | <code>HierarchicalDataSourceControl.RenderControl(HtmlTextWriter) = &quot;ok&quot;</code><br><code>HierarchicalDataSourceControl.RenderControl(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>HierarchicalDataSourceControl.RenderControl(HtmlTextWriter) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollectio…</code> |
| <code>Api.HierarchicalDataSourceControl.SkinID</code> | <code>HierarchicalDataSourceControl.SkinID = &quot;&quot;</code><br><code>HierarchicalDataSourceControl.SkinID set = &quot;-&gt; System.NotSupportedException: Control of type &#39;SiteMapDataSource&#39; does not support theming.&quot;</code> |
| <code>Api.HierarchicalDataSourceControl.Visible</code> | <code>HierarchicalDataSourceControl.Visible = &quot;false&quot;</code><br><code>HierarchicalDataSourceControl.Visible set = &quot;-&gt; System.NotSupportedException: Control of type &#39;SiteMapDataSource&#39; is a non-visual control and does not support s…</code> |
| <code>Api.IControlBuilderAccessor.ControlBuilder</code> | <code>IControlBuilderAccessor.ControlBuilder = null</code> |
| <code>Api.IControlDesignerAccessor.GetDesignModeState</code> | <code>IControlDesignerAccessor.GetDesignModeState = &quot;[]&quot;</code><br><code>IControlDesignerAccessor.GetDesignModeState after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTh…</code> |
| <code>Api.IControlDesignerAccessor.SetDesignModeState</code> | <code>IControlDesignerAccessor.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetDesignModeState(IDictionary) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=t…</code> |
| <code>Api.IControlDesignerAccessor.SetOwnerControl</code> | <code>IControlDesignerAccessor.SetOwnerControl(Control) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetOwnerControl(Control) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; En…</code> |
| <code>Api.IControlDesignerAccessor.UserData</code> | <code>IControlDesignerAccessor.UserData = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.DataBindings</code> | <code>IDataBindingsAccessor.DataBindings = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.HasDataBindings</code> | <code>IDataBindingsAccessor.HasDataBindings = &quot;false&quot;</code> |
| <code>Api.IDataSource.DataSourceChanged</code> | <code>IDataSource.DataSourceChanged = &quot;added and removed&quot;</code> |
| <code>Api.IDataSource.GetView</code> | <code>IDataSource.GetView(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.IDataSource.GetViewNames</code> | <code>IDataSource.GetViewNames = &quot;[System.String DefaultView]&quot;</code><br><code>IDataSource.GetViewNames after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; Enable…</code> |
| <code>Api.IExpressionsAccessor.Expressions</code> | <code>IExpressionsAccessor.Expressions = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.HasExpressions</code> | <code>IExpressionsAccessor.HasExpressions = &quot;false&quot;</code> |
| <code>Api.IHierarchicalDataSource.DataSourceChanged</code> | <code>IHierarchicalDataSource.DataSourceChanged = &quot;added and removed&quot;</code> |
| <code>Api.IHierarchicalDataSource.GetHierarchicalView</code> | <code>IHierarchicalDataSource.GetHierarchicalView(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.IParserAccessor.AddParsedSubObject</code> | <code>IParserAccessor.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>IParserAccessor.AddParsedSubObject(Object) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableThe…</code> |
| <code>Api.IUrlResolutionService.ResolveClientUrl</code> | <code>IUrlResolutionService.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>IUrlResolutionService.ResolveClientUrl(String) after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; Enabl…</code> |
| <code>Api.WebControls.SiteMapDataSource.#ctor</code> | <code>SiteMapDataSource.#ctor = SiteMapDataSource# {ClientID=&quot;&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; EnableViewState=true; ID=null…</code> |
| <code>Api.WebControls.SiteMapDataSource.ContainsListCollection</code> | <code>SiteMapDataSource.ContainsListCollection = &quot;true&quot;</code> |
| <code>Api.WebControls.SiteMapDataSource.GetHierarchicalView</code> | <code>SiteMapDataSource.GetHierarchicalView(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.WebControls.SiteMapDataSource.GetList</code> | <code>SiteMapDataSource.GetList = &quot;[System.Web.UI.ListSourceHelper+ListSourceRow]&quot;</code><br><code>SiteMapDataSource.GetList after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; Enabl…</code> |
| <code>Api.WebControls.SiteMapDataSource.GetView</code> | <code>SiteMapDataSource.GetView(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.WebControls.SiteMapDataSource.GetViewNames</code> | <code>SiteMapDataSource.GetViewNames = &quot;[System.String DefaultView]&quot;</code><br><code>SiteMapDataSource.GetViewNames after = SiteMapDataSource#smdsSite {ClientID=&quot;smdsSite&quot;; ClientIDMode=Inherit; ContainsListCollection=true; EnableTheming=false; …</code> |
| <code>Api.WebControls.SiteMapDataSource.Provider</code> | <code>SiteMapDataSource.Provider = XmlSiteMapProvider {EnableLocalization=false; ResourceKey=null; SecurityTrimmingEnabled=false}</code><br><code>SiteMapDataSource.Provider set = XmlSiteMapProvider {EnableLocalization=false; ResourceKey=null; SecurityTrimmingEnabled=false}</code> |
| <code>Api.WebControls.SiteMapDataSource.ShowStartingNode</code> | <code>SiteMapDataSource.ShowStartingNode = &quot;true&quot;</code><br><code>SiteMapDataSource.ShowStartingNode set = &quot;true&quot;</code> |
| <code>Api.WebControls.SiteMapDataSource.SiteMapProvider</code> | <code>SiteMapDataSource.SiteMapProvider = &quot;&quot;</code><br><code>SiteMapDataSource.SiteMapProvider set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.SiteMapDataSource.StartFromCurrentNode</code> | <code>SiteMapDataSource.StartFromCurrentNode = &quot;false&quot;</code><br><code>SiteMapDataSource.StartFromCurrentNode set = &quot;true&quot;</code> |
| <code>Api.WebControls.SiteMapDataSource.StartingNodeOffset</code> | <code>SiteMapDataSource.StartingNodeOffset = &quot;Int32 0&quot;</code><br><code>SiteMapDataSource.StartingNodeOffset set = &quot;Int32 1&quot;</code> |
| <code>Api.WebControls.SiteMapDataSource.StartingNodeUrl</code> | <code>SiteMapDataSource.StartingNodeUrl = &quot;&quot;</code><br><code>SiteMapDataSource.StartingNodeUrl set = &quot;Abc 123&quot;</code> |

## ② HTML 層

プローブサイトの RenderProbe.aspx で、1 ケース 1 リクエスト（ページライフサイクル全体）で描画したページ全体を比較します。下表はコンポーネント部分（div#probe）、設定時の例外、保存された ViewState の要約です。

| ケース | 設定 | 期待される描画 | 設定時の例外 |
|---|---|---|---|
| <code>.(default)</code> | <code>(nothing set)</code> | <code></code> |  |
| <code>.AppRelativeTemplateSourceDirectory=text</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;;</code> | <code></code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;probe&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=markup</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=japanese</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;;</code> | <code></code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;日本語テキスト&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=empty</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&quot;;</code> | <code></code> |  |
| <code>.ClientIDMode=Inherit</code> | <code>c.ClientIDMode = ClientIDMode.Inherit;</code> | <code></code> | <code>c.ClientIDMode = ClientIDMode.Inherit; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>.ClientIDMode=AutoID</code> | <code>c.ClientIDMode = ClientIDMode.AutoID;</code> | <code></code> | <code>c.ClientIDMode = ClientIDMode.AutoID; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>.ClientIDMode=Predictable</code> | <code>c.ClientIDMode = ClientIDMode.Predictable;</code> | <code></code> | <code>c.ClientIDMode = ClientIDMode.Predictable; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>.ClientIDMode=Static</code> | <code>c.ClientIDMode = ClientIDMode.Static;</code> | <code></code> | <code>c.ClientIDMode = ClientIDMode.Static; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>.EnableTheming=true</code> | <code>c.EnableTheming = true;</code> | <code></code> | <code>c.EnableTheming = true; -&gt; System.NotSupportedException: Control of type &#39;SiteMapDataSource&#39; does not support theming.</code> |
| <code>.EnableTheming=false</code> | <code>c.EnableTheming = false;</code> | <code></code> | <code>c.EnableTheming = false; -&gt; System.NotSupportedException: Control of type &#39;SiteMapDataSource&#39; does not support theming.</code> |
| <code>.EnableViewState=true</code> | <code>c.EnableViewState = true;</code> | <code></code> |  |
| <code>.EnableViewState=false</code> | <code>c.EnableViewState = false;</code> | <code></code> |  |
| <code>.ID=text</code> | <code>c.ID = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.ID=markup</code> | <code>c.ID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.ID=japanese</code> | <code>c.ID = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.ID=empty</code> | <code>c.ID = &quot;&quot;;</code> | <code></code> |  |
| <code>.ShowStartingNode=true</code> | <code>c.ShowStartingNode = true;</code> | <code></code> |  |
| <code>.ShowStartingNode=false</code> | <code>c.ShowStartingNode = false;</code> | <code></code> |  |
| <code>.SiteMapProvider=text</code> | <code>c.SiteMapProvider = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.SiteMapProvider=markup</code> | <code>c.SiteMapProvider = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.SiteMapProvider=japanese</code> | <code>c.SiteMapProvider = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.SiteMapProvider=empty</code> | <code>c.SiteMapProvider = &quot;&quot;;</code> | <code></code> |  |
| <code>.SiteMapProvider=default</code> | <code>c.SiteMapProvider = &quot;AspNetXmlSiteMapProvider&quot;;</code> | <code></code> |  |
| <code>.SkinID=text</code> | <code>c.SkinID = &quot;probe&quot;;</code> | <code></code> | <code>c.SkinID = &quot;probe&quot;; -&gt; System.NotSupportedException: Control of type &#39;SiteMapDataSource&#39; does not support theming.</code> |
| <code>.SkinID=markup</code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.NotSupportedException: Control of type &#39;SiteMapDataSource&#39; does not support theming.</code> |
| <code>.SkinID=japanese</code> | <code>c.SkinID = &quot;日本語テキスト&quot;;</code> | <code></code> | <code>c.SkinID = &quot;日本語テキスト&quot;; -&gt; System.NotSupportedException: Control of type &#39;SiteMapDataSource&#39; does not support theming.</code> |
| <code>.SkinID=empty</code> | <code>c.SkinID = &quot;&quot;;</code> | <code></code> | <code>c.SkinID = &quot;&quot;; -&gt; System.NotSupportedException: Control of type &#39;SiteMapDataSource&#39; does not support theming.</code> |
| <code>.StartFromCurrentNode=true</code> | <code>c.StartFromCurrentNode = true;</code> | <code></code> |  |
| <code>.StartFromCurrentNode=false</code> | <code>c.StartFromCurrentNode = false;</code> | <code></code> |  |
| <code>.StartingNodeOffset=0</code> | <code>c.StartingNodeOffset = 0;</code> | <code></code> |  |
| <code>.StartingNodeOffset=3</code> | <code>c.StartingNodeOffset = 3;</code> | <code></code> |  |
| <code>.StartingNodeOffset=-1</code> | <code>c.StartingNodeOffset = -1;</code> | <code></code> |  |
| <code>.StartingNodeUrl=text</code> | <code>c.StartingNodeUrl = &quot;probe&quot;;</code> | <code></code> |  |
| <code>.StartingNodeUrl=markup</code> | <code>c.StartingNodeUrl = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code></code> |  |
| <code>.StartingNodeUrl=japanese</code> | <code>c.StartingNodeUrl = &quot;日本語テキスト&quot;;</code> | <code></code> |  |
| <code>.StartingNodeUrl=empty</code> | <code>c.StartingNodeUrl = &quot;&quot;;</code> | <code></code> |  |
| <code>.StartingNodeUrl=app-relative</code> | <code>c.StartingNodeUrl = &quot;~/NavigationScenario.aspx&quot;;</code> | <code></code> |  |
| <code>.StartingNodeUrl=missing</code> | <code>c.StartingNodeUrl = &quot;~/None.aspx&quot;;</code> | <code></code> |  |
| <code>.ValidateRequestMode=Inherit</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Inherit;</code> | <code></code> |  |
| <code>.ValidateRequestMode=Disabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Disabled;</code> | <code></code> |  |
| <code>.ValidateRequestMode=Enabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Enabled;</code> | <code></code> |  |
| <code>.ViewStateMode=Inherit</code> | <code>c.ViewStateMode = ViewStateMode.Inherit;</code> | <code></code> |  |
| <code>.ViewStateMode=Enabled</code> | <code>c.ViewStateMode = ViewStateMode.Enabled;</code> | <code></code> |  |
| <code>.ViewStateMode=Disabled</code> | <code>c.ViewStateMode = ViewStateMode.Disabled;</code> | <code></code> |  |
| <code>.Visible=true</code> | <code>c.Visible = true;</code> | <code></code> | <code>c.Visible = true; -&gt; System.NotSupportedException: Control of type &#39;SiteMapDataSource&#39; is a non-visual control and does not support setting the Visible property.</code> |
| <code>.Visible=false</code> | <code>c.Visible = false;</code> | <code></code> | <code>c.Visible = false; -&gt; System.NotSupportedException: Control of type &#39;SiteMapDataSource&#39; is a non-visual control and does not support setting the Visible property.</code> |
| <code>.ShowStartingNode=false+StartFromCurrentNode=true</code> | <code>c.ShowStartingNode = false; c.StartFromCurrentNode = true;</code> | <code></code> |  |
| <code>.StartingNodeOffset=-1+StartFromCurrentNode=true</code> | <code>c.StartingNodeOffset = -1; c.StartFromCurrentNode = true;</code> | <code></code> |  |
| <code>.ShowStartingNode=false+Visible=false</code> | <code>c.ShowStartingNode = false; c.Visible = false;</code> | <code></code> | <code>c.Visible = false; -&gt; System.NotSupportedException: Control of type &#39;SiteMapDataSource&#39; is a non-visual control and does not support setting the Visible property.</code> |
| <code>[naming container].(default)</code> | <code>[naming container] (nothing set)</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=AutoID</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> | <code>c.ClientIDMode = ClientIDMode.AutoID; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>[naming container].ClientIDMode=Static</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> | <code>c.ClientIDMode = ClientIDMode.Static; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>[naming container].ClientIDMode=Predictable</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> | <code>c.ClientIDMode = ClientIDMode.Predictable; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>[naming container].ClientIDMode=Inherit</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> | <code>c.ClientIDMode = ClientIDMode.Inherit; -&gt; System.NotSupportedException: Specified method is not supported.</code> |
| <code>[naming container].ID=text</code> | <code>[naming container] c.ID = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵↵&lt;/div&gt;</code> |  |

## ③ シナリオ層

ブラウザ（Playwright）で SiteMapDataSourceScenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。

| # | 操作 | 対象 | 値 |
|---|---|---|---|
| 1 | goto | /NavigationScenario.aspx | <code></code> |
| 2 | snapshot |  | **SiteMapDataSource [render][databind][dynamic] first request: the site map as a path, trees and menus; bound to XML; templated; made in code** |
| 3 | evaluate | tvStatic | <code>Array.from(e.querySelectorAll(&#39;a&#39;)).find(a =&gt; a.href.indexOf(&quot;&#39;tf&#39;&quot;) &gt;= 0).click();</code> |
| 4 | waitms |  | <code></code> |
| 5 | snapshot |  | **SiteMapDataSource [viewstate] a node expanded by a postback (EnableClientScript=false)** |
| 6 | clicktextin | tvStatic | <code>Apple</code> |
| 7 | snapshot |  | **SiteMapDataSource [viewstate] a node selected (its value a control parameter&#39;s: ControlValueProperty)** |
| 8 | evaluate | tvStatic | <code>e.querySelectorAll(&#39;input[type=checkbox]&#39;)[2].checked = true;</code> |
| 9 | click | btnSubmit | <code></code> |
| 10 | snapshot |  | **SiteMapDataSource [viewstate] a node&#39;s check box posted** |
| 11 | evaluate | tvStatic | <code>Array.from(e.querySelectorAll(&#39;a&#39;)).find(a =&gt; a.href.indexOf(&quot;&#39;tv&#39;&quot;) &gt;= 0).click();</code> |
| 12 | waitms |  | <code></code> |
| 13 | snapshot |  | **SiteMapDataSource [viewstate] a node populated on demand by a postback** |
| 14 | evaluate | tvCallback | <code>Array.from(e.querySelectorAll(&#39;a&#39;)).find(a =&gt; a.href.indexOf(&#39;TreeView_PopulateNode&#39;) &gt;= 0).click();</code> |
| 15 | waitms |  | <code></code> |
| 16 | snapshot |  | **SiteMapDataSource [viewstate] a node populated on demand by a client callback (no postback)** |
| 17 | clicktextin | mnuStatic | <code>Help</code> |
| 18 | snapshot |  | **SiteMapDataSource [viewstate] a menu&#39;s item clicked** |
| 19 | clicktextin | mnuBound | <code>Bound two</code> |
| 20 | snapshot |  | **SiteMapDataSource [viewstate] a bound menu&#39;s item clicked** |
| 21 | clicktextin | mnuTemplated | <code>Bound child</code> |
| 22 | snapshot |  | **SiteMapDataSource [viewstate] a templated menu&#39;s item clicked** |
| 23 | clicktextin | tvDynamic | <code>Dynamic node</code> |
| 24 | snapshot |  | **SiteMapDataSource [viewstate] a dynamic tree&#39;s node selected** |
| 25 | clicktextin | mnuDynamic | <code>Dynamic child</code> |
| 26 | snapshot |  | **SiteMapDataSource [viewstate] a dynamic menu&#39;s item clicked** |
| 27 | click | btnShow | <code></code> |
| 28 | snapshot |  | **SiteMapDataSource [visible][viewstate] hidden ones shown with what was added while hidden** |

<details><summary>SiteMapDataSource [render][databind][dynamic] first request: the site map as a path, trees and menus; bound to XML; templated; made in code（期待される本文）</summary>

```text
Path:
Skip Navigation Links
Home / Navigation
Templated path:
Skip Navigation Links
[node: Home] | [current: Navigation]
Skip Navigation Links.
Home
Render probe
Navigation
Skip Navigation Links.
Fruits
Vegetables
Plain
The tree's selected value (a control parameter)
Value
(null)
Skip Navigation Links.
On demand
Skip Navigation Links.
Bound one
Bound child
Bound two
Skip Navigation Links
Home
Render probe
Navigation
Skip Navigation Links
File
Open
Save
Help
Skip Navigation Links
Bound one
Bound child
Bound two
Skip Navigation Links
[item: Bound one]
[item: Bound child]
[item: Bound two]
Dynamic:
Skip Navigation Links.
Dynamic node
Dynamic leaf
Skip Navigation Links
Dynamic item
Dynamic child
Skip Navigation Links
Home > Navigation
Skip Navigation Links
Render probe
Navigation
Request 1: IsPostBack=False IsCallback=False
State tvSite Nodes=[]
State tvStatic Nodes=[f, f/apple, f/banana, v, plain]
State tvCallback Nodes=[d]
State tvBound Nodes=[]
State tvHidden Nodes=[h] (hidden)
State tvDynamic Nodes=[dn, dn/dl]
State mnuSite Items=[]
State mnuStatic Items=[file, file/open, file/save, help]
State mnuBound Items=[]
State mnuTemplated Items=[]
State mnuHidden Items=[h] (hidden)
State mnuDynamic Items=[di, di/dc]
State current node: Navigation /NavigationScenario.aspx
```

</details>

<details><summary>SiteMapDataSource [viewstate] a node expanded by a postback (EnableClientScript=false)（期待される本文）</summary>

```text
Path:
Skip Navigation Links
Home / Navigation
Templated path:
Skip Navigation Links
[node: Home] | [current: Navigation]
Skip Navigation Links.
Home
Render probe
Navigation
Skip Navigation Links.
Fruits
Apple
Banana
Vegetables
Plain
The tree's selected value (a control parameter)
Value
(null)
Skip Navigation Links.
On demand
Skip Navigation Links.
Bound one
Bound child
Bound two
Skip Navigation Links
Home
Render probe
Navigation
Skip Navigation Links
File
Open
Save
Help
Skip Navigation Links
Bound one
Bound child
Bound two
Skip Navigation Links
[item: Bound one]
[item: Bound child]
[item: Bound two]
Dynamic:
Skip Navigation Links.
Dynamic node
Dynamic leaf
Skip Navigation Links
Dynamic item
Dynamic child
Skip Navigation Links
Home > Navigation
Skip Navigation Links
Render probe
Navigation
Request 2: IsPostBack=True IsCallback=False
TreeNodeExpanded tvStatic f
State tvSite Nodes=[Home expanded, Home/Render probe, Home/Navigation selected]
State tvStatic Nodes=[f expanded, f/apple, f/banana, v, plain]
State tvCallback Nodes=[d]
State tvBound Nodes=[b1 expanded, b1/b1c, b2 expanded]
State tvHidden Nodes=[h] (hidden)
State tvDynamic Nodes=[dn expanded, dn/dl expanded]
State mnuSite Items=[Home, Home/Render probe, Home/Navigation selected, Home/Navigation/Target & more]
State mnuStatic Items=[file, file/open, file/save, help]
State mnuBound Items=[b1, b1/b1c, b2]
State mnuTemplated Items=[b1, b1/b1c, b2]
State mnuHidden Items=[h] (hidden)
State mnuDynamic Items=[di, di/dc]
State current node: Navigation /NavigationScenario.aspx
```

</details>

<details><summary>SiteMapDataSource [viewstate] a node selected (its value a control parameter&#39;s: ControlValueProperty)（期待される本文）</summary>

```text
Path:
Skip Navigation Links
Home / Navigation
Templated path:
Skip Navigation Links
[node: Home] | [current: Navigation]
Skip Navigation Links.
Home
Render probe
Navigation
Skip Navigation Links.
Fruits
Apple
Banana
Vegetables
Plain
The tree's selected value (a control parameter)
Value
apple
Skip Navigation Links.
On demand
Skip Navigation Links.
Bound one
Bound child
Bound two
Skip Navigation Links
Home
Render probe
Navigation
Skip Navigation Links
File
Open
Save
Help
Skip Navigation Links
Bound one
Bound child
Bound two
Skip Navigation Links
[item: Bound one]
[item: Bound child]
[item: Bound two]
Dynamic:
Skip Navigation Links.
Dynamic node
Dynamic leaf
Skip Navigation Links
Dynamic item
Dynamic child
Skip Navigation Links
Home > Navigation
Skip Navigation Links
Render probe
Navigation
Request 3: IsPostBack=True IsCallback=False
SelectedNodeChanged tvStatic f/apple
State tvSite Nodes=[Home expanded, Home/Render probe, Home/Navigation selected]
State tvStatic Nodes=[f expanded, f/apple selected, f/banana, v, plain]
State tvCallback Nodes=[d]
State tvBound Nodes=[b1 expanded, b1/b1c, b2 expanded]
State tvHidden Nodes=[h] (hidden)
State tvDynamic Nodes=[dn expanded, dn/dl expanded]
State mnuSite Items=[Home, Home/Render probe, Home/Navigation selected, Home/Navigation/Target & more]
State mnuStatic Items=[file, file/open, file/save, help]
State mnuBound Items=[b1, b1/b1c, b2]
State mnuTemplated Items=[b1, b1/b1c, b2]
State mnuHidden Items=[h] (hidden)
State mnuDynamic Items=[di, di/dc]
State current node: Navigation /NavigationScenario.aspx
```

</details>

<details><summary>SiteMapDataSource [viewstate] a node&#39;s check box posted（期待される本文）</summary>

```text
Path:
Skip Navigation Links
Home / Navigation
Templated path:
Skip Navigation Links
[node: Home] | [current: Navigation]
Skip Navigation Links.
Home
Render probe
Navigation
Skip Navigation Links.
Fruits
Apple
Banana
Vegetables
Plain
The tree's selected value (a control parameter)
Value
apple
Skip Navigation Links.
On demand
Skip Navigation Links.
Bound one
Bound child
Bound two
Skip Navigation Links
Home
Render probe
Navigation
Skip Navigation Links
File
Open
Save
Help
Skip Navigation Links
Bound one
Bound child
Bound two
Skip Navigation Links
[item: Bound one]
[item: Bound child]
[item: Bound two]
Dynamic:
Skip Navigation Links.
Dynamic node
Dynamic leaf
Skip Navigation Links
Dynamic item
Dynamic child
Skip Navigation Links
Home > Navigation
Skip Navigation Links
Render probe
Navigation
Request 4: IsPostBack=True IsCallback=False
TreeNodeCheckChanged tvStatic f/banana Checked=True
State tvSite Nodes=[Home expanded, Home/Render probe, Home/Navigation selected]
State tvStatic Nodes=[f expanded, f/apple selected, f/banana checked, v, plain]
State tvCallback Nodes=[d]
State tvBound Nodes=[b1 expanded, b1/b1c, b2 expanded]
State tvHidden Nodes=[h] (hidden)
State tvDynamic Nodes=[dn expanded, dn/dl expanded]
State mnuSite Items=[Home, Home/Render probe, Home/Navigation selected, Home/Navigation/Target & more]
State mnuStatic Items=[file, file/open, file/save, help]
State mnuBound Items=[b1, b1/b1c, b2]
State mnuTemplated Items=[b1, b1/b1c, b2]
State mnuHidden Items=[h] (hidden)
State mnuDynamic Items=[di, di/dc]
State current node: Navigation /NavigationScenario.aspx
```

</details>

<details><summary>SiteMapDataSource [viewstate] a node populated on demand by a postback（期待される本文）</summary>

```text
Path:
Skip Navigation Links
Home / Navigation
Templated path:
Skip Navigation Links
[node: Home] | [current: Navigation]
Skip Navigation Links.
Home
Render probe
Navigation
Skip Navigation Links.
Fruits
Apple
Banana
Vegetables
Populated by postback 1
Populated by postback 2
Plain
The tree's selected value (a control parameter)
Value
apple
Skip Navigation Links.
On demand
Skip Navigation Links.
Bound one
Bound child
Bound two
Skip Navigation Links
Home
Render probe
Navigation
Skip Navigation Links
File
Open
Save
Help
Skip Navigation Links
Bound one
Bound child
Bound two
Skip Navigation Links
[item: Bound one]
[item: Bound child]
[item: Bound two]
Dynamic:
Skip Navigation Links.
Dynamic node
Dynamic leaf
Skip Navigation Links
Dynamic item
Dynamic child
Skip Navigation Links
Home > Navigation
Skip Navigation Links
Render probe
Navigation
Request 5: IsPostBack=True IsCallback=False
TreeNodePopulate tvStatic v
TreeNodeExpanded tvStatic v
State tvSite Nodes=[Home expanded, Home/Render probe, Home/Navigation selected]
State tvStatic Nodes=[f expanded, f/apple selected, f/banana checked, v expanded, v/p1, v/p2, plain]
State tvCallback Nodes=[d]
State tvBound Nodes=[b1 expanded, b1/b1c, b2 expanded]
State tvHidden Nodes=[h] (hidden)
State tvDynamic Nodes=[dn expanded, dn/dl expanded]
State mnuSite Items=[Home, Home/Render probe, Home/Navigation selected, Home/Navigation/Target & more]
State mnuStatic Items=[file, file/open, file/save, help]
State mnuBound Items=[b1, b1/b1c, b2]
State mnuTemplated Items=[b1, b1/b1c, b2]
State mnuHidden Items=[h] (hidden)
State mnuDynamic Items=[di, di/dc]
State current node: Navigation /NavigationScenario.aspx
```

</details>

<details><summary>SiteMapDataSource [viewstate] a node populated on demand by a client callback (no postback)（期待される本文）</summary>

```text
Path:
Skip Navigation Links
Home / Navigation
Templated path:
Skip Navigation Links
[node: Home] | [current: Navigation]
Skip Navigation Links.
Home
Render probe
Navigation
Skip Navigation Links.
Fruits
Apple
Banana
Vegetables
Populated by postback 1
Populated by postback 2
Plain
The tree's selected value (a control parameter)
Value
apple
Skip Navigation Links.
On demand
Populated by callback 1
Populated by callback 2
Skip Navigation Links.
Bound one
Bound child
Bound two
Skip Navigation Links
Home
Render probe
Navigation
Skip Navigation Links
File
Open
Save
Help
Skip Navigation Links
Bound one
Bound child
Bound two
Skip Navigation Links
[item: Bound one]
[item: Bound child]
[item: Bound two]
Dynamic:
Skip Navigation Links.
Dynamic node
Dynamic leaf
Skip Navigation Links
Dynamic item
Dynamic child
Skip Navigation Links
Home > Navigation
Skip Navigation Links
Render probe
Navigation
Request 5: IsPostBack=True IsCallback=False
TreeNodePopulate tvStatic v
TreeNodeExpanded tvStatic v
State tvSite Nodes=[Home expanded, Home/Render probe, Home/Navigation selected]
State tvStatic Nodes=[f expanded, f/apple selected, f/banana checked, v expanded, v/p1, v/p2, plain]
State tvCallback Nodes=[d]
State tvBound Nodes=[b1 expanded, b1/b1c, b2 expanded]
State tvHidden Nodes=[h] (hidden)
State tvDynamic Nodes=[dn expanded, dn/dl expanded]
State mnuSite Items=[Home, Home/Render probe, Home/Navigation selected, Home/Navigation/Target & more]
State mnuStatic Items=[file, file/open, file/save, help]
State mnuBound Items=[b1, b1/b1c, b2]
State mnuTemplated Items=[b1, b1/b1c, b2]
State mnuHidden Items=[h] (hidden)
State mnuDynamic Items=[di, di/dc]
State current node: Navigation /NavigationScenario.aspx
```

</details>

<details><summary>SiteMapDataSource [viewstate] a menu&#39;s item clicked（期待される本文）</summary>

```text
Path:
Skip Navigation Links
Home / Navigation
Templated path:
Skip Navigation Links
[node: Home] | [current: Navigation]
Skip Navigation Links.
Home
Render probe
Navigation
Skip Navigation Links.
Fruits
Apple
Banana
Vegetables
Populated by postback 1
Populated by postback 2
Plain
The tree's selected value (a control parameter)
Value
apple
Skip Navigation Links.
On demand
Populated by postback 1
Populated by postback 2
Skip Navigation Links.
Bound one
Bound child
Bound two
Skip Navigation Links
Home
Render probe
Navigation
Skip Navigation Links
File
Open
Save
Help
Skip Navigation Links
Bound one
Bound child
Bound two
Skip Navigation Links
[item: Bound one]
[item: Bound child]
[item: Bound two]
Dynamic:
Skip Navigation Links.
Dynamic node
Dynamic leaf
Skip Navigation Links
Dynamic item
Dynamic child
Skip Navigation Links
Home > Navigation
Skip Navigation Links
Render probe
Navigation
TreeNodePopulate tvCallback d
Request 6: IsPostBack=True IsCallback=False
MenuItemClick mnuStatic help
State tvSite Nodes=[Home expanded, Home/Render probe, Home/Navigation selected]
State tvStatic Nodes=[f expanded, f/apple selected, f/banana checked, v expanded, v/p1, v/p2, plain]
State tvCallback Nodes=[d expanded, d/p1, d/p2]
State tvBound Nodes=[b1 expanded, b1/b1c, b2 expanded]
State tvHidden Nodes=[h] (hidden)
State tvDynamic Nodes=[dn expanded, dn/dl expanded]
State mnuSite Items=[Home, Home/Render probe, Home/Navigation selected, Home/Navigation/Target & more]
State mnuStatic Items=[file, file/open, file/save, help selected]
State mnuBound Items=[b1, b1/b1c, b2]
State mnuTemplated Items=[b1, b1/b1c, b2]
State mnuHidden Items=[h] (hidden)
State mnuDynamic Items=[di, di/dc]
State current node: Navigation /NavigationScenario.aspx
```

</details>

<details><summary>SiteMapDataSource [viewstate] a bound menu&#39;s item clicked（期待される本文）</summary>

```text
Path:
Skip Navigation Links
Home / Navigation
Templated path:
Skip Navigation Links
[node: Home] | [current: Navigation]
Skip Navigation Links.
Home
Render probe
Navigation
Skip Navigation Links.
Fruits
Apple
Banana
Vegetables
Populated by postback 1
Populated by postback 2
Plain
The tree's selected value (a control parameter)
Value
apple
Skip Navigation Links.
On demand
Populated by postback 1
Populated by postback 2
Skip Navigation Links.
Bound one
Bound child
Bound two
Skip Navigation Links
Home
Render probe
Navigation
Skip Navigation Links
File
Open
Save
Help
Skip Navigation Links
Bound one
Bound child
Bound two
Skip Navigation Links
[item: Bound one]
[item: Bound child]
[item: Bound two]
Dynamic:
Skip Navigation Links.
Dynamic node
Dynamic leaf
Skip Navigation Links
Dynamic item
Dynamic child
Skip Navigation Links
Home > Navigation
Skip Navigation Links
Render probe
Navigation
Request 7: IsPostBack=True IsCallback=False
MenuItemClick mnuBound b2
State tvSite Nodes=[Home expanded, Home/Render probe, Home/Navigation selected]
State tvStatic Nodes=[f expanded, f/apple selected, f/banana checked, v expanded, v/p1, v/p2, plain]
State tvCallback Nodes=[d expanded, d/p1, d/p2]
State tvBound Nodes=[b1 expanded, b1/b1c, b2 expanded]
State tvHidden Nodes=[h] (hidden)
State tvDynamic Nodes=[dn expanded, dn/dl expanded]
State mnuSite Items=[Home, Home/Render probe, Home/Navigation selected, Home/Navigation/Target & more]
State mnuStatic Items=[file, file/open, file/save, help selected]
State mnuBound Items=[b1, b1/b1c, b2 selected]
State mnuTemplated Items=[b1, b1/b1c, b2]
State mnuHidden Items=[h] (hidden)
State mnuDynamic Items=[di, di/dc]
State current node: Navigation /NavigationScenario.aspx
```

</details>

<details><summary>SiteMapDataSource [viewstate] a templated menu&#39;s item clicked（期待される本文）</summary>

```text
Path:
Skip Navigation Links
Home / Navigation
Templated path:
Skip Navigation Links
[node: Home] | [current: Navigation]
Skip Navigation Links.
Home
Render probe
Navigation
Skip Navigation Links.
Fruits
Apple
Banana
Vegetables
Populated by postback 1
Populated by postback 2
Plain
The tree's selected value (a control parameter)
Value
apple
Skip Navigation Links.
On demand
Populated by postback 1
Populated by postback 2
Skip Navigation Links.
Bound one
Bound child
Bound two
Skip Navigation Links
Home
Render probe
Navigation
Skip Navigation Links
File
Open
Save
Help
Skip Navigation Links
Bound one
Bound child
Bound two
Skip Navigation Links
[item: Bound one]
[item: Bound child]
[item: Bound two]
Dynamic:
Skip Navigation Links.
Dynamic node
Dynamic leaf
Skip Navigation Links
Dynamic item
Dynamic child
Skip Navigation Links
Home > Navigation
Skip Navigation Links
Render probe
Navigation
Request 8: IsPostBack=True IsCallback=False
MenuItemClick mnuTemplated b1/b1c
State tvSite Nodes=[Home expanded, Home/Render probe, Home/Navigation selected]
State tvStatic Nodes=[f expanded, f/apple selected, f/banana checked, v expanded, v/p1, v/p2, plain]
State tvCallback Nodes=[d expanded, d/p1, d/p2]
State tvBound Nodes=[b1 expanded, b1/b1c, b2 expanded]
State tvHidden Nodes=[h] (hidden)
State tvDynamic Nodes=[dn expanded, dn/dl expanded]
State mnuSite Items=[Home, Home/Render probe, Home/Navigation selected, Home/Navigation/Target & more]
State mnuStatic Items=[file, file/open, file/save, help selected]
State mnuBound Items=[b1, b1/b1c, b2 selected]
State mnuTemplated Items=[b1, b1/b1c selected, b2]
State mnuHidden Items=[h] (hidden)
State mnuDynamic Items=[di, di/dc]
State current node: Navigation /NavigationScenario.aspx
```

</details>

<details><summary>SiteMapDataSource [viewstate] a dynamic tree&#39;s node selected（期待される本文）</summary>

```text
Path:
Skip Navigation Links
Home / Navigation
Templated path:
Skip Navigation Links
[node: Home] | [current: Navigation]
Skip Navigation Links.
Home
Render probe
Navigation
Skip Navigation Links.
Fruits
Apple
Banana
Vegetables
Populated by postback 1
Populated by postback 2
Plain
The tree's selected value (a control parameter)
Value
apple
Skip Navigation Links.
On demand
Populated by postback 1
Populated by postback 2
Skip Navigation Links.
Bound one
Bound child
Bound two
Skip Navigation Links
Home
Render probe
Navigation
Skip Navigation Links
File
Open
Save
Help
Skip Navigation Links
Bound one
Bound child
Bound two
Skip Navigation Links
[item: Bound one]
[item: Bound child]
[item: Bound two]
Dynamic:
Skip Navigation Links.
Dynamic node
Dynamic leaf
Skip Navigation Links
Dynamic item
Dynamic child
Skip Navigation Links
Home > Navigation
Skip Navigation Links
Render probe
Navigation
Request 9: IsPostBack=True IsCallback=False
SelectedNodeChanged tvDynamic dn
State tvSite Nodes=[Home expanded, Home/Render probe, Home/Navigation selected]
State tvStatic Nodes=[f expanded, f/apple selected, f/banana checked, v expanded, v/p1, v/p2, plain]
State tvCallback Nodes=[d expanded, d/p1, d/p2]
State tvBound Nodes=[b1 expanded, b1/b1c, b2 expanded]
State tvHidden Nodes=[h] (hidden)
State tvDynamic Nodes=[dn expanded selected, dn/dl expanded]
State mnuSite Items=[Home, Home/Render probe, Home/Navigation selected, Home/Navigation/Target & more]
State mnuStatic Items=[file, file/open, file/save, help selected]
State mnuBound Items=[b1, b1/b1c, b2 selected]
State mnuTemplated Items=[b1, b1/b1c selected, b2]
State mnuHidden Items=[h] (hidden)
State mnuDynamic Items=[di, di/dc]
State current node: Navigation /NavigationScenario.aspx
```

</details>

<details><summary>SiteMapDataSource [viewstate] a dynamic menu&#39;s item clicked（期待される本文）</summary>

```text
Path:
Skip Navigation Links
Home / Navigation
Templated path:
Skip Navigation Links
[node: Home] | [current: Navigation]
Skip Navigation Links.
Home
Render probe
Navigation
Skip Navigation Links.
Fruits
Apple
Banana
Vegetables
Populated by postback 1
Populated by postback 2
Plain
The tree's selected value (a control parameter)
Value
apple
Skip Navigation Links.
On demand
Populated by postback 1
Populated by postback 2
Skip Navigation Links.
Bound one
Bound child
Bound two
Skip Navigation Links
Home
Render probe
Navigation
Skip Navigation Links
File
Open
Save
Help
Skip Navigation Links
Bound one
Bound child
Bound two
Skip Navigation Links
[item: Bound one]
[item: Bound child]
[item: Bound two]
Dynamic:
Skip Navigation Links.
Dynamic node
Dynamic leaf
Skip Navigation Links
Dynamic item
Dynamic child
Skip Navigation Links
Home > Navigation
Skip Navigation Links
Render probe
Navigation
Request 10: IsPostBack=True IsCallback=False
MenuItemClick mnuDynamic di/dc
State tvSite Nodes=[Home expanded, Home/Render probe, Home/Navigation selected]
State tvStatic Nodes=[f expanded, f/apple selected, f/banana checked, v expanded, v/p1, v/p2, plain]
State tvCallback Nodes=[d expanded, d/p1, d/p2]
State tvBound Nodes=[b1 expanded, b1/b1c, b2 expanded]
State tvHidden Nodes=[h] (hidden)
State tvDynamic Nodes=[dn expanded selected, dn/dl expanded]
State mnuSite Items=[Home, Home/Render probe, Home/Navigation selected, Home/Navigation/Target & more]
State mnuStatic Items=[file, file/open, file/save, help selected]
State mnuBound Items=[b1, b1/b1c, b2 selected]
State mnuTemplated Items=[b1, b1/b1c selected, b2]
State mnuHidden Items=[h] (hidden)
State mnuDynamic Items=[di, di/dc selected]
State current node: Navigation /NavigationScenario.aspx
```

</details>

<details><summary>SiteMapDataSource [visible][viewstate] hidden ones shown with what was added while hidden（期待される本文）</summary>

```text
Path:
Skip Navigation Links
Home / Navigation
Templated path:
Skip Navigation Links
[node: Home] | [current: Navigation]
Skip Navigation Links.
Home
Render probe
Navigation
Skip Navigation Links.
Fruits
Apple
Banana
Vegetables
Populated by postback 1
Populated by postback 2
Plain
The tree's selected value (a control parameter)
Value
apple
Skip Navigation Links.
On demand
Populated by postback 1
Populated by postback 2
Skip Navigation Links.
Bound one
Bound child
Bound two
Skip Navigation Links.
Added while hidden
Skip Navigation Links
Home
Render probe
Navigation
Skip Navigation Links
File
Open
Save
Help
Skip Navigation Links
Bound one
Bound child
Bound two
Skip Navigation Links
[item: Bound one]
[item: Bound child]
[item: Bound two]
Skip Navigation Links
Added while hidden
Dynamic:
Skip Navigation Links.
Dynamic node
Dynamic leaf
Skip Navigation Links
Dynamic item
Dynamic child
Skip Navigation Links
Home > Navigation
Skip Navigation Links
Render probe
Navigation
Request 11: IsPostBack=True IsCallback=False
Click btnShow
State tvSite Nodes=[Home expanded, Home/Render probe, Home/Navigation selected]
State tvStatic Nodes=[f expanded, f/apple selected, f/banana checked, v expanded, v/p1, v/p2, plain]
State tvCallback Nodes=[d expanded, d/p1, d/p2]
State tvBound Nodes=[b1 expanded, b1/b1c, b2 expanded]
State tvHidden Nodes=[h expanded]
State tvDynamic Nodes=[dn expanded selected, dn/dl expanded]
State mnuSite Items=[Home, Home/Render probe, Home/Navigation selected, Home/Navigation/Target & more]
State mnuStatic Items=[file, file/open, file/save, help selected]
State mnuBound Items=[b1, b1/b1c, b2 selected]
State mnuTemplated Items=[b1, b1/b1c selected, b2]
State mnuHidden Items=[h]
State mnuDynamic Items=[di, di/dc selected]
State current node: Navigation /NavigationScenario.aspx
```

</details>

