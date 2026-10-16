# Repeater テストケース一覧

変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。
このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。

- 正解データの採取: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0
- ① API 層: 150 ケース（157 メンバー）
- ② HTML 層: 57 ケース
- ③ シナリオ層: 7 スナップショット

## 機構マトリクス

型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。

| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |
|---|---|---|---|
| プロパティ → HTML 描画 | すべてのコントロール | 45 | 1 |
| ViewState の保存と復元 | Control | 7 | 2 |
| Visible=false（描画されず状態は保持） | Control.Visible | 3 | 1 |
| 名前付けコンテナと ClientIDMode | Control.ClientIDMode | 6 | 0 |
| 動的に生成したコントロールのライフサイクル | Control | 0 | 2 |
| データバインド（DataBind / <%# %>） | Control.DataBind | 0 | 2 |
| 行・項目のコマンド（RowCommand・ItemCommand へのバブル） | *Command イベント（Command 以外） | 0 | 3 |
| データソースコントロールとの連携（DataSourceID） | データ表示コントロール（Repeater・CompositeDataBoundControl・BaseDataList・ListView） | 0 | 1 |
| テンプレート | ITemplate のプロパティ | 0 | 1 |

該当しない機構: テーマとスキン（SkinID / EnableTheming）、Enabled=false・Disabled=true（描画とポスト値の扱い）、ポストバックデータの取り込み、変更イベント（*Changed、ServerChange）、ReadOnly（ポストされた値を無視）、リクエスト検証（ValidateRequestMode）、ポストバックイベント、コマンドイベントと親へのバブル（CommandName）、送信ボタン／スクリプトによるポストバック（UseSubmitBehavior）、ブラウザ側のクリック処理（OnClientClick）、クロスページポストバック（PostBackUrl）、既定のボタン（Panel の DefaultButton）、AutoPostBack（クライアントスクリプトによるポストバック）、CausesValidation / ValidationGroup、検証対象（サーバー側の検証）、検証対象（ブラウザ側の検証）、双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値）、項目（ListItem の Selected・Enabled・Text・Value と ViewState）、複数選択、繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）、ポスト値のイベント検証（一覧にない値・無効な項目の拒否）、サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）、ブラウザ側の検証スクリプト（EnableClientScript）、検証結果のまとめ（ValidationSummary）、ページング、並べ替え、項目の挿入、行・項目の編集（編集・更新・取り消し・削除）、行・項目の選択、データキー（DataKeyNames・DataKeys）、データソース連携（DataSource・DataTextField など）、クライアントコールバック

## ① API 層

Repeater とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の Repeater インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。

| ケース | 期待値（.NET Framework） |
|---|---|
| <code>Api.Control.#ctor</code> | <code>Control.#ctor = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMode=In…</code> |
| <code>Api.Control.Adapter</code> | <code>Control.Adapter = null</code> |
| <code>Api.Control.AddParsedSubObject</code> | <code>Control.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>Control.AddParsedSubObject(Object) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; Ena…</code> |
| <code>Api.Control.AddedControl</code> | <code>Control.AddedControl(Control,Int32) = &quot;ok&quot;</code><br><code>Control.AddedControl(Control,Int32) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; En…</code> |
| <code>Api.Control.AppRelativeTemplateSourceDirectory</code> | <code>Control.AppRelativeTemplateSourceDirectory = &quot;&quot;</code><br><code>Control.AppRelativeTemplateSourceDirectory set = &quot;-&gt; System.ArgumentException: The relative virtual path &#39;Abc 123&#39; is not allowed here.&quot;</code> |
| <code>Api.Control.ApplyStyleSheetSkin</code> | <code>Control.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>Control.ApplyStyleSheetSkin(Page) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; Enab…</code> |
| <code>Api.Control.BeginRenderTracing</code> | <code>Control.BeginRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.BeginRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.BeginRenderTracing(TextWriter,Object) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableThemin…</code> |
| <code>Api.Control.BindingContainer</code> | <code>Control.BindingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Clien…</code> |
| <code>Api.Control.BuildProfileTree</code> | <code>Control.BuildProfileTree(String,Boolean) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.ChildControlsCreated</code> | <code>Control.ChildControlsCreated = &quot;false&quot;</code><br><code>Control.ChildControlsCreated set = &quot;ok&quot;</code> |
| <code>Api.Control.ClearCachedClientID</code> | <code>Control.ClearCachedClientID = &quot;ok&quot;</code><br><code>Control.ClearCachedClientID after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableView…</code> |
| <code>Api.Control.ClearChildControlState</code> | <code>Control.ClearChildControlState = &quot;ok&quot;</code><br><code>Control.ClearChildControlState after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableV…</code> |
| <code>Api.Control.ClearChildState</code> | <code>Control.ClearChildState = &quot;ok&quot;</code><br><code>Control.ClearChildState after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableViewStat…</code> |
| <code>Api.Control.ClearChildViewState</code> | <code>Control.ClearChildViewState = &quot;ok&quot;</code><br><code>Control.ClearChildViewState after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableView…</code> |
| <code>Api.Control.ClearEffectiveClientIDMode</code> | <code>Control.ClearEffectiveClientIDMode = &quot;ok&quot;</code><br><code>Control.ClearEffectiveClientIDMode after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; Ena…</code> |
| <code>Api.Control.ClientID</code> | <code>Control.ClientID = &quot;rptItems&quot;</code> |
| <code>Api.Control.ClientIDMode</code> | <code>Control.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>Control.ClientIDMode set = &quot;ClientIDMode.AutoID&quot;</code> |
| <code>Api.Control.ClientIDSeparator</code> | <code>Control.ClientIDSeparator = &quot;&#39;_&#39;&quot;</code> |
| <code>Api.Control.Context</code> | <code>Control.Context = null</code> |
| <code>Api.Control.Controls</code> | <code>Control.Controls = ControlCollection []</code> |
| <code>Api.Control.CreateChildControls</code> | <code>Control.CreateChildControls = &quot;ok&quot;</code><br><code>Control.CreateChildControls after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableView…</code> |
| <code>Api.Control.CreateControlCollection</code> | <code>Control.CreateControlCollection = ControlCollection []</code><br><code>Control.CreateControlCollection after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; Enable…</code> |
| <code>Api.Control.DataBind</code> | <code>Control.DataBind = &quot;ok&quot;</code><br><code>Control.DataBind after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableViewState=true;…</code><br><code>Control.DataBind(Boolean) = &quot;ok&quot;</code><br><code>Control.DataBind(Boolean) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableViewSt…</code> |
| <code>Api.Control.DataBindChildren</code> | <code>Control.DataBindChildren = &quot;ok&quot;</code><br><code>Control.DataBindChildren after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableViewSta…</code> |
| <code>Api.Control.DataBinding</code> | <code>Control.DataBinding = &quot;added and removed&quot;</code> |
| <code>Api.Control.DataItemContainer</code> | <code>Control.DataItemContainer = null</code> |
| <code>Api.Control.DataKeysContainer</code> | <code>Control.DataKeysContainer = null</code> |
| <code>Api.Control.DesignMode</code> | <code>Control.DesignMode = &quot;false&quot;</code> |
| <code>Api.Control.Dispose</code> | <code>Control.Dispose = &quot;ok&quot;</code><br><code>Control.Dispose after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableViewState=true; …</code> |
| <code>Api.Control.Disposed</code> | <code>Control.Disposed = &quot;added and removed&quot;</code> |
| <code>Api.Control.EnableTheming</code> | <code>Control.EnableTheming = &quot;true&quot;</code><br><code>Control.EnableTheming set = &quot;true&quot;</code> |
| <code>Api.Control.EnableViewState</code> | <code>Control.EnableViewState = &quot;true&quot;</code><br><code>Control.EnableViewState set = &quot;true&quot;</code> |
| <code>Api.Control.EndRenderTracing</code> | <code>Control.EndRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.EndRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.EndRenderTracing(TextWriter,Object) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=…</code> |
| <code>Api.Control.EnsureChildControls</code> | <code>Control.EnsureChildControls = &quot;ok&quot;</code><br><code>Control.EnsureChildControls after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableView…</code> |
| <code>Api.Control.EnsureID</code> | <code>Control.EnsureID = &quot;ok&quot;</code><br><code>Control.EnsureID after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableViewState=true;…</code> |
| <code>Api.Control.Events</code> | <code>Control.Events = &quot;System.ComponentModel.EventHandlerList&quot;</code> |
| <code>Api.Control.FindControl</code> | <code>Control.FindControl(String) = null</code><br><code>Control.FindControl(String) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableView…</code><br><code>Control.FindControl(String,Int32) = null</code><br><code>Control.FindControl(String,Int32) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; Enab…</code> |
| <code>Api.Control.Focus</code> | <code>Control.Focus = &quot;-&gt; System.InvalidOperationException: A form tag with runat=server must exist on the Page to use SetFocus() or the Focus property.&quot;</code> |
| <code>Api.Control.GetDesignModeState</code> | <code>Control.GetDesignModeState = &quot;[]&quot;</code><br><code>Control.GetDesignModeState after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableViewS…</code> |
| <code>Api.Control.GetRouteUrl</code> | <code>Control.GetRouteUrl(Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.GetUniqueIDRelativeTo</code> | <code>Control.GetUniqueIDRelativeTo(Control) = &quot;rptItems&quot;</code><br><code>Control.GetUniqueIDRelativeTo(Control) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true;…</code> |
| <code>Api.Control.HasChildViewState</code> | <code>Control.HasChildViewState = &quot;false&quot;</code> |
| <code>Api.Control.HasControls</code> | <code>Control.HasControls = &quot;false&quot;</code><br><code>Control.HasControls after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableViewState=tr…</code> |
| <code>Api.Control.HasEvents</code> | <code>Control.HasEvents = &quot;false&quot;</code><br><code>Control.HasEvents after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableViewState=true…</code> |
| <code>Api.Control.ID</code> | <code>Control.ID = &quot;rptItems&quot;</code><br><code>Control.ID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.IdSeparator</code> | <code>Control.IdSeparator = &quot;&#39;$&#39;&quot;</code> |
| <code>Api.Control.Init</code> | <code>Control.Init = &quot;added and removed&quot;</code> |
| <code>Api.Control.IsChildControlStateCleared</code> | <code>Control.IsChildControlStateCleared = &quot;false&quot;</code> |
| <code>Api.Control.IsLiteralContent</code> | <code>Control.IsLiteralContent = &quot;false&quot;</code><br><code>Control.IsLiteralContent after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableViewSta…</code> |
| <code>Api.Control.IsTrackingViewState</code> | <code>Control.IsTrackingViewState = &quot;false&quot;</code> |
| <code>Api.Control.IsViewStateEnabled</code> | <code>Control.IsViewStateEnabled = &quot;true&quot;</code> |
| <code>Api.Control.Load</code> | <code>Control.Load = &quot;added and removed&quot;</code> |
| <code>Api.Control.LoadControlState</code> | <code>Control.LoadControlState(Object) = &quot;ok&quot;</code><br><code>Control.LoadControlState(Object) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.LoadViewState</code> | <code>Control.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Collections.ArrayList&#39;.&quot;</code> |
| <code>Api.Control.LoadViewStateByID</code> | <code>Control.LoadViewStateByID = &quot;false&quot;</code> |
| <code>Api.Control.MapPathSecure</code> | <code>Control.MapPathSecure(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.NamingContainer</code> | <code>Control.NamingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code> |
| <code>Api.Control.OnBubbleEvent</code> | <code>Control.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Control.OnBubbleEvent(Object,EventArgs) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true…</code> |
| <code>Api.Control.OnDataBinding</code> | <code>Control.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnDataBinding(EventArgs) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.OnInit</code> | <code>Control.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnInit(EventArgs) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableViewSt…</code> |
| <code>Api.Control.OnLoad</code> | <code>Control.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnLoad(EventArgs) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableViewSt…</code> |
| <code>Api.Control.OnPreRender</code> | <code>Control.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnPreRender(EventArgs) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableV…</code> |
| <code>Api.Control.OnUnload</code> | <code>Control.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnUnload(EventArgs) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableView…</code> |
| <code>Api.Control.OpenFile</code> | <code>Control.OpenFile(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.Page</code> | <code>Control.Page = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=&quot;&quot;; …</code><br><code>Control.Page set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=…</code> |
| <code>Api.Control.Parent</code> | <code>Control.Parent = HtmlForm#form1 {Action=&quot;&quot;; ClientID=&quot;form1&quot;; ClientIDMode=Inherit; DefaultButton=&quot;&quot;; DefaultFocus=&quot;&quot;; Disabled=false; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.PreRender</code> | <code>Control.PreRender = &quot;added and removed&quot;</code> |
| <code>Api.Control.RaiseBubbleEvent</code> | <code>Control.RaiseBubbleEvent(Object,EventArgs) = &quot;ok&quot;</code><br><code>Control.RaiseBubbleEvent(Object,EventArgs) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=t…</code> |
| <code>Api.Control.RemovedControl</code> | <code>Control.RemovedControl(Control) = &quot;ok&quot;</code><br><code>Control.RemovedControl(Control) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; Enable…</code> |
| <code>Api.Control.Render</code> | <code>Control.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.Render(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.Render(HtmlTextWriter) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableV…</code> |
| <code>Api.Control.RenderChildren</code> | <code>Control.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderChildren(HtmlTextWriter) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true;…</code> |
| <code>Api.Control.RenderControl</code> | <code>Control.RenderControl(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderControl(HtmlTextWriter) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; …</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=…</code> |
| <code>Api.Control.RenderingCompatibility</code> | <code>Control.RenderingCompatibility = &quot;System.Version 4.0&quot;</code><br><code>Control.RenderingCompatibility set = &quot;System.Version 4.0&quot;</code> |
| <code>Api.Control.ResolveAdapter</code> | <code>Control.ResolveAdapter = null</code><br><code>Control.ResolveAdapter after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableViewState…</code> |
| <code>Api.Control.ResolveClientUrl</code> | <code>Control.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveClientUrl(String) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.ResolveUrl</code> | <code>Control.ResolveUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveUrl(String) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableViewS…</code> |
| <code>Api.Control.SaveControlState</code> | <code>Control.SaveControlState = null</code><br><code>Control.SaveControlState after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableViewSta…</code> |
| <code>Api.Control.SaveViewState</code> | <code>Control.SaveViewState = null</code><br><code>Control.SaveViewState after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableViewState=…</code> |
| <code>Api.Control.SetDesignModeState</code> | <code>Control.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>Control.SetDesignModeState(IDictionary) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true…</code> |
| <code>Api.Control.SetRenderMethodDelegate</code> | <code>Control.SetRenderMethodDelegate(RenderMethod) = &quot;ok&quot;</code><br><code>Control.SetRenderMethodDelegate(RenderMethod) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableThemin…</code> |
| <code>Api.Control.SetTraceData</code> | <code>Control.SetTraceData(Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; En…</code><br><code>Control.SetTraceData(Object,Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object,Object) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=t…</code> |
| <code>Api.Control.Site</code> | <code>Control.Site = null</code><br><code>Control.Site set = null</code> |
| <code>Api.Control.SkinID</code> | <code>Control.SkinID = &quot;&quot;</code><br><code>Control.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.TemplateControl</code> | <code>Control.TemplateControl = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code><br><code>Control.TemplateControl set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Cl…</code> |
| <code>Api.Control.TemplateSourceDirectory</code> | <code>Control.TemplateSourceDirectory = &quot;&quot;</code> |
| <code>Api.Control.TrackViewState</code> | <code>Control.TrackViewState = &quot;ok&quot;</code><br><code>Control.TrackViewState after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableViewState…</code> |
| <code>Api.Control.UniqueID</code> | <code>Control.UniqueID = &quot;rptItems&quot;</code> |
| <code>Api.Control.Unload</code> | <code>Control.Unload = &quot;added and removed&quot;</code> |
| <code>Api.Control.ValidateRequestMode</code> | <code>Control.ValidateRequestMode = &quot;ValidateRequestMode.Inherit&quot;</code><br><code>Control.ValidateRequestMode set = &quot;ValidateRequestMode.Disabled&quot;</code> |
| <code>Api.Control.ViewState</code> | <code>Control.ViewState = StateBag []</code> |
| <code>Api.Control.ViewStateIgnoresCase</code> | <code>Control.ViewStateIgnoresCase = &quot;false&quot;</code> |
| <code>Api.Control.ViewStateMode</code> | <code>Control.ViewStateMode = &quot;ViewStateMode.Inherit&quot;</code><br><code>Control.ViewStateMode set = &quot;ViewStateMode.Enabled&quot;</code> |
| <code>Api.Control.Visible</code> | <code>Control.Visible = &quot;true&quot;</code><br><code>Control.Visible set = &quot;true&quot;</code> |
| <code>Api.IControlBuilderAccessor.ControlBuilder</code> | <code>IControlBuilderAccessor.ControlBuilder = null</code> |
| <code>Api.IControlDesignerAccessor.GetDesignModeState</code> | <code>IControlDesignerAccessor.GetDesignModeState = &quot;[]&quot;</code><br><code>IControlDesignerAccessor.GetDesignModeState after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=…</code> |
| <code>Api.IControlDesignerAccessor.SetDesignModeState</code> | <code>IControlDesignerAccessor.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetDesignModeState(IDictionary) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; E…</code> |
| <code>Api.IControlDesignerAccessor.SetOwnerControl</code> | <code>IControlDesignerAccessor.SetOwnerControl(Control) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetOwnerControl(Control) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTh…</code> |
| <code>Api.IControlDesignerAccessor.UserData</code> | <code>IControlDesignerAccessor.UserData = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.DataBindings</code> | <code>IDataBindingsAccessor.DataBindings = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.HasDataBindings</code> | <code>IDataBindingsAccessor.HasDataBindings = &quot;false&quot;</code> |
| <code>Api.IExpressionsAccessor.Expressions</code> | <code>IExpressionsAccessor.Expressions = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.HasExpressions</code> | <code>IExpressionsAccessor.HasExpressions = &quot;false&quot;</code> |
| <code>Api.IParserAccessor.AddParsedSubObject</code> | <code>IParserAccessor.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>IParserAccessor.AddParsedSubObject(Object) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=t…</code> |
| <code>Api.IUrlResolutionService.ResolveClientUrl</code> | <code>IUrlResolutionService.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>IUrlResolutionService.ResolveClientUrl(String) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableThemi…</code> |
| <code>Api.WebControls.Repeater.#ctor</code> | <code>Repeater.#ctor = Repeater# {ClientID=&quot;&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableViewState=true; ID=null; ItemType=&quot;&quot;; S…</code> |
| <code>Api.WebControls.Repeater.AlternatingItemTemplate</code> | <code>Repeater.AlternatingItemTemplate = null</code><br><code>Repeater.AlternatingItemTemplate set = null</code> |
| <code>Api.WebControls.Repeater.CallingDataMethods</code> | <code>Repeater.CallingDataMethods = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.Repeater.Controls</code> | <code>Repeater.Controls = ControlCollection []</code> |
| <code>Api.WebControls.Repeater.CreateChildControls</code> | <code>Repeater.CreateChildControls = &quot;ok&quot;</code><br><code>Repeater.CreateChildControls after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableVie…</code> |
| <code>Api.WebControls.Repeater.CreateControlHierarchy</code> | <code>Repeater.CreateControlHierarchy(Boolean) = &quot;ok&quot;</code><br><code>Repeater.CreateControlHierarchy(Boolean) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=tru…</code> |
| <code>Api.WebControls.Repeater.CreateDataSourceSelectArguments</code> | <code>Repeater.CreateDataSourceSelectArguments = DataSourceSelectArguments {MaximumRows=Int32 0; RetrieveTotalRowCount=false; SortExpression=&quot;&quot;; StartRowIndex=Int32 0…</code><br><code>Repeater.CreateDataSourceSelectArguments after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=tru…</code> |
| <code>Api.WebControls.Repeater.CreateItem</code> | <code>Repeater.CreateItem(Int32,ListItemType) = RepeaterItem# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; ItemIndex=Int32 0…</code><br><code>Repeater.CreateItem(Int32,ListItemType) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true…</code> |
| <code>Api.WebControls.Repeater.CreatingModelDataSource</code> | <code>Repeater.CreatingModelDataSource = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.Repeater.DataBind</code> | <code>Repeater.DataBind = &quot;ok&quot;</code><br><code>Repeater.DataBind after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableViewState=true…</code> |
| <code>Api.WebControls.Repeater.DataMember</code> | <code>Repeater.DataMember = &quot;&quot;</code><br><code>Repeater.DataMember set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.Repeater.DataSource</code> | <code>Repeater.DataSource = &quot;[System.String Pen, System.String Bag]&quot;</code><br><code>Repeater.DataSource set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.Repeater.DataSourceID</code> | <code>Repeater.DataSourceID = &quot;&quot;</code><br><code>Repeater.DataSourceID set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.Repeater.EnableTheming</code> | <code>Repeater.EnableTheming = &quot;true&quot;</code><br><code>Repeater.EnableTheming set = &quot;true&quot;</code> |
| <code>Api.WebControls.Repeater.EnsureDataBound</code> | <code>Repeater.EnsureDataBound = &quot;ok&quot;</code><br><code>Repeater.EnsureDataBound after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableViewSta…</code> |
| <code>Api.WebControls.Repeater.FooterTemplate</code> | <code>Repeater.FooterTemplate = null</code><br><code>Repeater.FooterTemplate set = null</code> |
| <code>Api.WebControls.Repeater.GetData</code> | <code>Repeater.GetData = &quot;[System.String Pen, System.String Bag]&quot;</code><br><code>Repeater.GetData after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableViewState=true;…</code> |
| <code>Api.WebControls.Repeater.HeaderTemplate</code> | <code>Repeater.HeaderTemplate = null</code><br><code>Repeater.HeaderTemplate set = null</code> |
| <code>Api.WebControls.Repeater.InitializeItem</code> | <code>Repeater.InitializeItem(RepeaterItem) = &quot;ok&quot;</code><br><code>Repeater.InitializeItem(RepeaterItem) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; …</code> |
| <code>Api.WebControls.Repeater.Initialized</code> | <code>Repeater.Initialized = &quot;false&quot;</code> |
| <code>Api.WebControls.Repeater.IsBoundUsingDataSourceID</code> | <code>Repeater.IsBoundUsingDataSourceID = &quot;false&quot;</code> |
| <code>Api.WebControls.Repeater.IsDataBindingAutomatic</code> | <code>Repeater.IsDataBindingAutomatic = &quot;false&quot;</code> |
| <code>Api.WebControls.Repeater.ItemCommand</code> | <code>Repeater.ItemCommand = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.Repeater.ItemCreated</code> | <code>Repeater.ItemCreated = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.Repeater.ItemDataBound</code> | <code>Repeater.ItemDataBound = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.Repeater.ItemTemplate</code> | <code>Repeater.ItemTemplate = &quot;FrameworkOnCore.WebControlsParity.Component+SampleTemplate&quot;</code><br><code>Repeater.ItemTemplate set = null</code> |
| <code>Api.WebControls.Repeater.ItemType</code> | <code>Repeater.ItemType = &quot;&quot;</code><br><code>Repeater.ItemType set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.Repeater.Items</code> | <code>Repeater.Items = &quot;[]&quot;</code> |
| <code>Api.WebControls.Repeater.LoadViewState</code> | <code>Repeater.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Collections.ArrayList&#39;.&quot;</code> |
| <code>Api.WebControls.Repeater.OnBubbleEvent</code> | <code>Repeater.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Repeater.OnBubbleEvent(Object,EventArgs) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=tru…</code> |
| <code>Api.WebControls.Repeater.OnCreatingModelDataSource</code> | <code>Repeater.OnCreatingModelDataSource(CreatingModelDataSourceEventArgs) = &quot;ok&quot;</code><br><code>Repeater.OnCreatingModelDataSource(CreatingModelDataSourceEventArgs) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSo…</code> |
| <code>Api.WebControls.Repeater.OnDataBinding</code> | <code>Repeater.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Repeater.OnDataBinding(EventArgs) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; Enab…</code> |
| <code>Api.WebControls.Repeater.OnDataPropertyChanged</code> | <code>Repeater.OnDataPropertyChanged = &quot;ok&quot;</code><br><code>Repeater.OnDataPropertyChanged after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableV…</code> |
| <code>Api.WebControls.Repeater.OnDataSourceViewChanged</code> | <code>Repeater.OnDataSourceViewChanged(Object,EventArgs) = &quot;ok&quot;</code><br><code>Repeater.OnDataSourceViewChanged(Object,EventArgs) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableT…</code> |
| <code>Api.WebControls.Repeater.OnInit</code> | <code>Repeater.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>Repeater.OnInit(EventArgs) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableViewS…</code> |
| <code>Api.WebControls.Repeater.OnItemCommand</code> | <code>Repeater.OnItemCommand(RepeaterCommandEventArgs) = &quot;ok&quot;</code><br><code>Repeater.OnItemCommand(RepeaterCommandEventArgs) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableThe…</code> |
| <code>Api.WebControls.Repeater.OnItemCreated</code> | <code>Repeater.OnItemCreated(RepeaterItemEventArgs) = &quot;ok&quot;</code><br><code>Repeater.OnItemCreated(RepeaterItemEventArgs) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableThemin…</code> |
| <code>Api.WebControls.Repeater.OnItemDataBound</code> | <code>Repeater.OnItemDataBound(RepeaterItemEventArgs) = &quot;ok&quot;</code><br><code>Repeater.OnItemDataBound(RepeaterItemEventArgs) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableThem…</code> |
| <code>Api.WebControls.Repeater.OnLoad</code> | <code>Repeater.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>Repeater.OnLoad(EventArgs) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableViewS…</code> |
| <code>Api.WebControls.Repeater.OnPreRender</code> | <code>Repeater.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>Repeater.OnPreRender(EventArgs) after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; Enable…</code> |
| <code>Api.WebControls.Repeater.RequiresDataBinding</code> | <code>Repeater.RequiresDataBinding = &quot;false&quot;</code><br><code>Repeater.RequiresDataBinding set = &quot;ok&quot;</code> |
| <code>Api.WebControls.Repeater.SaveViewState</code> | <code>Repeater.SaveViewState = null</code><br><code>Repeater.SaveViewState after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableViewState…</code> |
| <code>Api.WebControls.Repeater.SelectArguments</code> | <code>Repeater.SelectArguments = DataSourceSelectArguments {MaximumRows=Int32 0; RetrieveTotalRowCount=false; SortExpression=&quot;&quot;; StartRowIndex=Int32 0; TotalRowCount=…</code> |
| <code>Api.WebControls.Repeater.SelectMethod</code> | <code>Repeater.SelectMethod = &quot;&quot;</code><br><code>Repeater.SelectMethod set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.Repeater.SeparatorTemplate</code> | <code>Repeater.SeparatorTemplate = null</code><br><code>Repeater.SeparatorTemplate set = null</code> |
| <code>Api.WebControls.Repeater.TrackViewState</code> | <code>Repeater.TrackViewState = &quot;ok&quot;</code><br><code>Repeater.TrackViewState after = Repeater#rptItems {ClientID=&quot;rptItems&quot;; ClientIDMode=Inherit; DataMember=&quot;&quot;; DataSourceID=&quot;&quot;; EnableTheming=true; EnableViewStat…</code> |

## ② HTML 層

プローブサイトの RenderProbe.aspx で、1 ケース 1 リクエスト（ページライフサイクル全体）で描画したページ全体を比較します。下表はコンポーネント部分（div#probe）、設定時の例外、保存された ViewState の要約です。

| ケース | 設定 | 期待される描画 | 設定時の例外 |
|---|---|---|---|
| <code>.(default)</code> | <code>(nothing set)</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.AppRelativeTemplateSourceDirectory=text</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;probe&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=markup</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=japanese</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;日本語テキスト&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=empty</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ClientIDMode=Inherit</code> | <code>c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ClientIDMode=AutoID</code> | <code>c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ClientIDMode=Predictable</code> | <code>c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ClientIDMode=Static</code> | <code>c.ClientIDMode = ClientIDMode.Static;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.DataMember=text</code> | <code>c.DataMember = &quot;probe&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.DataMember=markup</code> | <code>c.DataMember = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.DataMember=japanese</code> | <code>c.DataMember = &quot;日本語テキスト&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.DataMember=empty</code> | <code>c.DataMember = &quot;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.DataSourceID=text</code> | <code>c.DataSourceID = &quot;probe&quot;;</code> | <code>status = 500</code> |  |
| <code>.DataSourceID=markup</code> | <code>c.DataSourceID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>status = 500</code> |  |
| <code>.DataSourceID=japanese</code> | <code>c.DataSourceID = &quot;日本語テキスト&quot;;</code> | <code>status = 500</code> |  |
| <code>.DataSourceID=empty</code> | <code>c.DataSourceID = &quot;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.EnableTheming=true</code> | <code>c.EnableTheming = true;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.EnableTheming=false</code> | <code>c.EnableTheming = false;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.EnableViewState=true</code> | <code>c.EnableViewState = true;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.EnableViewState=false</code> | <code>c.EnableViewState = false;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ID=text</code> | <code>c.ID = &quot;probe&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ID=markup</code> | <code>c.ID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ID=japanese</code> | <code>c.ID = &quot;日本語テキスト&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ID=empty</code> | <code>c.ID = &quot;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ItemType=text</code> | <code>c.ItemType = &quot;probe&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ItemType=markup</code> | <code>c.ItemType = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ItemType=japanese</code> | <code>c.ItemType = &quot;日本語テキスト&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ItemType=empty</code> | <code>c.ItemType = &quot;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.SelectMethod=text</code> | <code>c.SelectMethod = &quot;probe&quot;;</code> | <code>status = 500</code> |  |
| <code>.SelectMethod=markup</code> | <code>c.SelectMethod = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>status = 500</code> |  |
| <code>.SelectMethod=japanese</code> | <code>c.SelectMethod = &quot;日本語テキスト&quot;;</code> | <code>status = 500</code> |  |
| <code>.SelectMethod=empty</code> | <code>c.SelectMethod = &quot;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.SkinID=text</code> | <code>c.SkinID = &quot;probe&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.SkinID=markup</code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.SkinID=japanese</code> | <code>c.SkinID = &quot;日本語テキスト&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.SkinID=empty</code> | <code>c.SkinID = &quot;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ValidateRequestMode=Inherit</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Inherit;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ValidateRequestMode=Disabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Disabled;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ValidateRequestMode=Enabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Enabled;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ViewStateMode=Inherit</code> | <code>c.ViewStateMode = ViewStateMode.Inherit;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ViewStateMode=Enabled</code> | <code>c.ViewStateMode = ViewStateMode.Enabled;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ViewStateMode=Disabled</code> | <code>c.ViewStateMode = ViewStateMode.Disabled;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.Visible=true</code> | <code>c.Visible = true;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.Visible=false</code> | <code>c.Visible = false;</code> | <code></code> |  |
| <code>.EnableViewState=false+Visible=false</code> | <code>c.EnableViewState = false; c.Visible = false;</code> | <code></code> |  |
| <code>[naming container].(default)</code> | <code>[naming container] (nothing set)</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=AutoID</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Static</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Predictable</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Inherit</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]↵&lt;/div&gt;</code> |  |
| <code>[naming container].ID=text</code> | <code>[naming container] c.ID = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]↵&lt;/div&gt;</code> |  |
| <code>[templates].(default)</code> | <code>[templates] (nothing set)</code> | <code>[header][item: Pen (P1)][separator][alternating: Bag (B2)][separator][item: Cup &lt;&amp;&gt; (C3)][separator][alternating: Desk (D4)][separator][item: Eraser (E5)][separator][alternating: File (F6)][separator][item: Glue (G7)][footer]</code> |  |
| <code>[templates].EnableViewState=false</code> | <code>[templates] c.EnableViewState = false;</code> | <code>[header][item: Pen (P1)][separator][alternating: Bag (B2)][separator][item: Cup &lt;&amp;&gt; (C3)][separator][alternating: Desk (D4)][separator][item: Eraser (E5)][separator][alternating: File (F6)][separator][item: Glue (G7)][footer]</code> |  |
| <code>[templates].Visible=false</code> | <code>[templates] c.Visible = false;</code> | <code></code> |  |
| <code>[empty].(default)</code> | <code>[empty] (nothing set)</code> | <code>[header][footer]</code> |  |
| <code>[no template].(default)</code> | <code>[no template] (nothing set)</code> | <code></code> |  |

## ③ シナリオ層

ブラウザ（Playwright）で RepeaterScenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。

| # | 操作 | 対象 | 値 |
|---|---|---|---|
| 1 | goto | /RepeaterScenario.aspx | <code></code> |
| 2 | snapshot |  | **Repeater [render][databind][templates][datasourceid][dynamic] first request** |
| 3 | fill | txtQty | <code>5</code> |
| 4 | click | btnSubmit | <code></code> |
| 5 | snapshot |  | **Repeater [viewstate][dynamic] posted: items made again from view state, the text box&#39;s value** |
| 6 | clicktextin | divItems | <code>Remove</code> |
| 7 | snapshot |  | **Repeater [rowcommand] a link&#39;s command bubbled to ItemCommand; rebound** |
| 8 | clicktextin | divItems | <code>More</code> |
| 9 | snapshot |  | **Repeater [rowcommand] a button&#39;s command bubbled to ItemCommand** |
| 10 | clicktextin | divOuter | <code>Inner pick Banana</code> |
| 11 | snapshot |  | **Repeater [rowcommand] a nested repeater&#39;s command** |
| 12 | click | btnRebind | <code></code> |
| 13 | snapshot |  | **Repeater [databind] rebound** |
| 14 | click | btnShow | <code></code> |
| 15 | snapshot |  | **Repeater [visible][viewstate] the hidden repeater shown with its items** |

<details><summary>Repeater [render][databind][templates][datasourceid][dynamic] first request（期待される本文）</summary>

```text
Header
Pen Remove
Bag Remove
Cup <&> Remove
Desk Remove
Footer
Fruit: Inner pick AppleInner pick Banana
Tool: Inner pick Hammer
P1B2C3D4E5F6G7
Empty header
Empty footer
No view state: PenNo view state: Bag [dynamic: Pen (P1)][dynamic: Bag (B2)]
Request 1: IsPostBack=False (items created from view state: 0)
rptItems bound: ItemCreated 9, ItemDataBound 9
Repeater rptItems: Items=4
Repeater rptOuter: Items=2
Repeater rptAuto: Items=0
Repeater rptEmpty: Items=0
Repeater rptNoViewState: Items=2
Repeater rptHidden: Items=2 (hidden)
Repeater rptDynamic: Items=2
Item 0 Item: Pen qty ""
Item 1 AlternatingItem: Bag qty ""
Item 2 Item: Cup <&> qty ""
Item 3 AlternatingItem: Desk qty ""
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>Repeater [viewstate][dynamic] posted: items made again from view state, the text box&#39;s value（期待される本文）</summary>

```text
Header
Pen Remove
Bag Remove
Cup <&> Remove
Desk Remove
Footer
Fruit: Inner pick AppleInner pick Banana
Tool: Inner pick Hammer
P1B2C3D4E5F6G7
Empty header
Empty footer
[dynamic: Pen (P1)][dynamic: Bag (B2)]
Request 2: IsPostBack=True (items created from view state: 9)
Repeater rptItems: Items=4
Repeater rptOuter: Items=2
Repeater rptAuto: Items=7
Repeater rptEmpty: Items=0
Repeater rptNoViewState: Items=0
Repeater rptHidden: Items=2 (hidden)
Repeater rptDynamic: Items=2
Item 0 Item: Pen qty "5"
Item 1 AlternatingItem: Bag qty ""
Item 2 Item: Cup <&> qty ""
Item 3 AlternatingItem: Desk qty ""
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>Repeater [rowcommand] a link&#39;s command bubbled to ItemCommand; rebound（期待される本文）</summary>

```text
Header
Bag Remove
Cup <&> Remove
Desk Remove
Eraser Remove
Footer
Fruit: Inner pick AppleInner pick Banana
Tool: Inner pick Hammer
P1B2C3D4E5F6G7
Empty header
Empty footer
[dynamic: Pen (P1)][dynamic: Bag (B2)]
Request 3: IsPostBack=True (items created from view state: 9)
ItemCommand rptItems item 0 (Item): remove P1 from lnkRemove
rptItems bound: ItemCreated 18, ItemDataBound 9
Repeater rptItems: Items=4
Repeater rptOuter: Items=2
Repeater rptAuto: Items=7
Repeater rptEmpty: Items=0
Repeater rptNoViewState: Items=0
Repeater rptHidden: Items=2 (hidden)
Repeater rptDynamic: Items=2
Item 0 Item: Bag qty ""
Item 1 AlternatingItem: Cup <&> qty ""
Item 2 Item: Desk qty ""
Item 3 AlternatingItem: Eraser qty ""
Store: B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>Repeater [rowcommand] a button&#39;s command bubbled to ItemCommand（期待される本文）</summary>

```text
Header
Bag Remove
Cup <&> Remove
Desk Remove
Eraser Remove
Footer
Fruit: Inner pick AppleInner pick Banana
Tool: Inner pick Hammer
P1B2C3D4E5F6G7
Empty header
Empty footer
[dynamic: Pen (P1)][dynamic: Bag (B2)]
Request 4: IsPostBack=True (items created from view state: 9)
ItemCommand rptItems item 0 (Item): more 0 from btnMore
Repeater rptItems: Items=4
Repeater rptOuter: Items=2
Repeater rptAuto: Items=7
Repeater rptEmpty: Items=0
Repeater rptNoViewState: Items=0
Repeater rptHidden: Items=2 (hidden)
Repeater rptDynamic: Items=2
Item 0 Item: Bag qty ""
Item 1 AlternatingItem: Cup <&> qty ""
Item 2 Item: Desk qty ""
Item 3 AlternatingItem: Eraser qty ""
Store: B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>Repeater [rowcommand] a nested repeater&#39;s command（期待される本文）</summary>

```text
Header
Bag Remove
Cup <&> Remove
Desk Remove
Eraser Remove
Footer
Fruit: Inner pick AppleInner pick Banana
Tool: Inner pick Hammer
P1B2C3D4E5F6G7
Empty header
Empty footer
[dynamic: Pen (P1)][dynamic: Bag (B2)]
Request 5: IsPostBack=True (items created from view state: 9)
ItemCommand rptInner item 1 (AlternatingItem): inner Banana from lnkInner
Repeater rptItems: Items=4
Repeater rptOuter: Items=2
Repeater rptAuto: Items=7
Repeater rptEmpty: Items=0
Repeater rptNoViewState: Items=0
Repeater rptHidden: Items=2 (hidden)
Repeater rptDynamic: Items=2
Item 0 Item: Bag qty ""
Item 1 AlternatingItem: Cup <&> qty ""
Item 2 Item: Desk qty ""
Item 3 AlternatingItem: Eraser qty ""
Store: B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>Repeater [databind] rebound（期待される本文）</summary>

```text
Header
Bag Remove
Cup <&> Remove
Desk Remove
Eraser Remove
Footer
Fruit: Inner pick AppleInner pick Banana
Tool: Inner pick Hammer
P1B2C3D4E5F6G7
Empty header
Empty footer
[dynamic: Pen (P1)][dynamic: Bag (B2)]
Request 6: IsPostBack=True (items created from view state: 9)
Click btnRebind
rptItems bound: ItemCreated 18, ItemDataBound 9
Repeater rptItems: Items=4
Repeater rptOuter: Items=2
Repeater rptAuto: Items=7
Repeater rptEmpty: Items=0
Repeater rptNoViewState: Items=0
Repeater rptHidden: Items=2 (hidden)
Repeater rptDynamic: Items=2
Item 0 Item: Bag qty ""
Item 1 AlternatingItem: Cup <&> qty ""
Item 2 Item: Desk qty ""
Item 3 AlternatingItem: Eraser qty ""
Store: B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>Repeater [visible][viewstate] the hidden repeater shown with its items（期待される本文）</summary>

```text
Header
Bag Remove
Cup <&> Remove
Desk Remove
Eraser Remove
Footer
Fruit: Inner pick AppleInner pick Banana
Tool: Inner pick Hammer
P1B2C3D4E5F6G7
Empty header
Empty footer
Hidden: PenHidden: Bag [dynamic: Pen (P1)][dynamic: Bag (B2)]
Request 7: IsPostBack=True (items created from view state: 9)
Click btnShow
Repeater rptItems: Items=4
Repeater rptOuter: Items=2
Repeater rptAuto: Items=7
Repeater rptEmpty: Items=0
Repeater rptNoViewState: Items=0
Repeater rptHidden: Items=2
Repeater rptDynamic: Items=2
Item 0 Item: Bag qty ""
Item 1 AlternatingItem: Cup <&> qty ""
Item 2 Item: Desk qty ""
Item 3 AlternatingItem: Eraser qty ""
Store: B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

