# ListView テストケース一覧

変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。
このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。

- 正解データの採取: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0
- ① API 層: 335 ケース（345 メンバー）
- ② HTML 層: 186 ケース
- ③ シナリオ層: 18 スナップショット

## 機構マトリクス

型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。

| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |
|---|---|---|---|
| プロパティ → HTML 描画 | すべてのコントロール | 161 | 1 |
| ViewState の保存と復元 | Control | 6 | 3 |
| Visible=false（描画されず状態は保持） | Control.Visible | 2 | 1 |
| 名前付けコンテナと ClientIDMode | Control.ClientIDMode | 6 | 0 |
| テーマとスキン（SkinID / EnableTheming） | テーマを付けられる型（[Themeable]） | 4 | 0 |
| 動的に生成したコントロールのライフサイクル | Control | 0 | 1 |
| データバインド（DataBind / <%# %>） | Control.DataBind | 0 | 1 |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | WebControl.Enabled、HtmlControl.Disabled | 1 | 0 |
| ページング | AllowPaging プロパティ、IPageableItemContainer（ListView）、DataPager | 0 | 2 |
| 並べ替え | AllowSorting プロパティ、Sorting イベント（ListView） | 0 | 4 |
| 項目の挿入 | InsertItemTemplate・AutoGenerateInsertButton プロパティ | 10 | 1 |
| 行・項目の編集（編集・更新・取り消し・削除） | EditIndex・EditItemIndex・DefaultMode プロパティ | 6 | 5 |
| 行・項目の選択 | データ表示コントロールの SelectedIndex | 8 | 2 |
| 行・項目のコマンド（RowCommand・ItemCommand へのバブル） | *Command イベント（Command 以外） | 0 | 1 |
| データキー（DataKeyNames・DataKeys） | DataKeyNames プロパティ | 4 | 3 |
| データソースコントロールとの連携（DataSourceID） | データ表示コントロール（Repeater・CompositeDataBoundControl・BaseDataList・ListView） | 0 | 7 |
| テンプレート | ITemplate のプロパティ | 0 | 1 |
| データソース連携（DataSource・DataTextField など） | BaseDataBoundControl、HtmlSelect | 2 | 1 |

該当しない機構: ポストバックデータの取り込み、変更イベント（*Changed、ServerChange）、ReadOnly（ポストされた値を無視）、リクエスト検証（ValidateRequestMode）、ポストバックイベント、コマンドイベントと親へのバブル（CommandName）、送信ボタン／スクリプトによるポストバック（UseSubmitBehavior）、ブラウザ側のクリック処理（OnClientClick）、クロスページポストバック（PostBackUrl）、既定のボタン（Panel の DefaultButton）、AutoPostBack（クライアントスクリプトによるポストバック）、CausesValidation / ValidationGroup、検証対象（サーバー側の検証）、検証対象（ブラウザ側の検証）、双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値）、項目（ListItem の Selected・Enabled・Text・Value と ViewState）、複数選択、繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）、ポスト値のイベント検証（一覧にない値・無効な項目の拒否）、サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）、ブラウザ側の検証スクリプト（EnableClientScript）、検証結果のまとめ（ValidationSummary）、クライアントコールバック

## ① API 層

ListView とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の ListView インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。

| ケース | 期待値（.NET Framework） |
|---|---|
| <code>Api.Control.#ctor</code> | <code>Control.#ctor = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMode=In…</code> |
| <code>Api.Control.Adapter</code> | <code>Control.Adapter = null</code> |
| <code>Api.Control.AddParsedSubObject</code> | <code>Control.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>Control.AddParsedSubObject(Object) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; B…</code> |
| <code>Api.Control.AddedControl</code> | <code>Control.AddedControl(Control,Int32) = &quot;ok&quot;</code><br><code>Control.AddedControl(Control,Int32) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; …</code> |
| <code>Api.Control.AppRelativeTemplateSourceDirectory</code> | <code>Control.AppRelativeTemplateSourceDirectory = &quot;&quot;</code><br><code>Control.AppRelativeTemplateSourceDirectory set = &quot;-&gt; System.ArgumentException: The relative virtual path &#39;Abc 123&#39; is not allowed here.&quot;</code> |
| <code>Api.Control.ApplyStyleSheetSkin</code> | <code>Control.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>Control.ApplyStyleSheetSkin(Page) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bo…</code> |
| <code>Api.Control.BeginRenderTracing</code> | <code>Control.BeginRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.BeginRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.BeginRenderTracing(TextWriter,Object) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color…</code> |
| <code>Api.Control.BindingContainer</code> | <code>Control.BindingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Clien…</code> |
| <code>Api.Control.BuildProfileTree</code> | <code>Control.BuildProfileTree(String,Boolean) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.ChildControlsCreated</code> | <code>Control.ChildControlsCreated = &quot;false&quot;</code><br><code>Control.ChildControlsCreated set = &quot;ok&quot;</code> |
| <code>Api.Control.ClearCachedClientID</code> | <code>Control.ClearCachedClientID = &quot;ok&quot;</code><br><code>Control.ClearCachedClientID after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSt…</code> |
| <code>Api.Control.ClearChildControlState</code> | <code>Control.ClearChildControlState = &quot;ok&quot;</code><br><code>Control.ClearChildControlState after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Borde…</code> |
| <code>Api.Control.ClearChildState</code> | <code>Control.ClearChildState = &quot;ok&quot;</code><br><code>Control.ClearChildState after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=…</code> |
| <code>Api.Control.ClearChildViewState</code> | <code>Control.ClearChildViewState = &quot;ok&quot;</code><br><code>Control.ClearChildViewState after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSt…</code> |
| <code>Api.Control.ClearEffectiveClientIDMode</code> | <code>Control.ClearEffectiveClientIDMode = &quot;ok&quot;</code><br><code>Control.ClearEffectiveClientIDMode after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; B…</code> |
| <code>Api.Control.ClientID</code> | <code>Control.ClientID = &quot;lvItems&quot;</code> |
| <code>Api.Control.ClientIDMode</code> | <code>Control.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>Control.ClientIDMode set = &quot;ClientIDMode.AutoID&quot;</code> |
| <code>Api.Control.ClientIDSeparator</code> | <code>Control.ClientIDSeparator = &quot;&#39;_&#39;&quot;</code> |
| <code>Api.Control.Context</code> | <code>Control.Context = null</code> |
| <code>Api.Control.Controls</code> | <code>Control.Controls = ControlCollection []</code> |
| <code>Api.Control.CreateChildControls</code> | <code>Control.CreateChildControls = &quot;ok&quot;</code><br><code>Control.CreateChildControls after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSt…</code> |
| <code>Api.Control.CreateControlCollection</code> | <code>Control.CreateControlCollection = ControlCollection []</code><br><code>Control.CreateControlCollection after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bord…</code> |
| <code>Api.Control.DataBind</code> | <code>Control.DataBind = &quot;ok&quot;</code><br><code>Control.DataBind after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet;…</code><br><code>Control.DataBind(Boolean) = &quot;ok&quot;</code><br><code>Control.DataBind(Boolean) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyl…</code> |
| <code>Api.Control.DataBindChildren</code> | <code>Control.DataBindChildren = &quot;ok&quot;</code><br><code>Control.DataBindChildren after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle…</code> |
| <code>Api.Control.DataBinding</code> | <code>Control.DataBinding = &quot;added and removed&quot;</code> |
| <code>Api.Control.DataItemContainer</code> | <code>Control.DataItemContainer = null</code> |
| <code>Api.Control.DataKeysContainer</code> | <code>Control.DataKeysContainer = null</code> |
| <code>Api.Control.DesignMode</code> | <code>Control.DesignMode = &quot;false&quot;</code> |
| <code>Api.Control.Dispose</code> | <code>Control.Dispose = &quot;ok&quot;</code><br><code>Control.Dispose after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; …</code> |
| <code>Api.Control.Disposed</code> | <code>Control.Disposed = &quot;added and removed&quot;</code> |
| <code>Api.Control.EnableTheming</code> | <code>Control.EnableTheming = &quot;true&quot;</code><br><code>Control.EnableTheming set = &quot;true&quot;</code> |
| <code>Api.Control.EnableViewState</code> | <code>Control.EnableViewState = &quot;true&quot;</code><br><code>Control.EnableViewState set = &quot;true&quot;</code> |
| <code>Api.Control.EndRenderTracing</code> | <code>Control.EndRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.EndRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.EndRenderTracing(TextWriter,Object) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code> |
| <code>Api.Control.EnsureChildControls</code> | <code>Control.EnsureChildControls = &quot;ok&quot;</code><br><code>Control.EnsureChildControls after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSt…</code> |
| <code>Api.Control.EnsureID</code> | <code>Control.EnsureID = &quot;ok&quot;</code><br><code>Control.EnsureID after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet;…</code> |
| <code>Api.Control.Events</code> | <code>Control.Events = &quot;System.ComponentModel.EventHandlerList&quot;</code> |
| <code>Api.Control.FindControl</code> | <code>Control.FindControl(String) = null</code><br><code>Control.FindControl(String) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSt…</code><br><code>Control.FindControl(String,Int32) = null</code><br><code>Control.FindControl(String,Int32) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bo…</code> |
| <code>Api.Control.Focus</code> | <code>Control.Focus = &quot;-&gt; System.InvalidOperationException: A form tag with runat=server must exist on the Page to use SetFocus() or the Focus property.&quot;</code> |
| <code>Api.Control.GetDesignModeState</code> | <code>Control.GetDesignModeState = &quot;[]&quot;</code><br><code>Control.GetDesignModeState after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSty…</code> |
| <code>Api.Control.GetRouteUrl</code> | <code>Control.GetRouteUrl(Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.GetUniqueIDRelativeTo</code> | <code>Control.GetUniqueIDRelativeTo(Control) = &quot;lvItems&quot;</code><br><code>Control.GetUniqueIDRelativeTo(Control) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty…</code> |
| <code>Api.Control.HasChildViewState</code> | <code>Control.HasChildViewState = &quot;false&quot;</code> |
| <code>Api.Control.HasControls</code> | <code>Control.HasControls = &quot;false&quot;</code><br><code>Control.HasControls after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotS…</code> |
| <code>Api.Control.HasEvents</code> | <code>Control.HasEvents = &quot;false&quot;</code><br><code>Control.HasEvents after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet…</code> |
| <code>Api.Control.ID</code> | <code>Control.ID = &quot;lvItems&quot;</code><br><code>Control.ID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.IdSeparator</code> | <code>Control.IdSeparator = &quot;&#39;$&#39;&quot;</code> |
| <code>Api.Control.Init</code> | <code>Control.Init = &quot;added and removed&quot;</code> |
| <code>Api.Control.IsChildControlStateCleared</code> | <code>Control.IsChildControlStateCleared = &quot;false&quot;</code> |
| <code>Api.Control.IsLiteralContent</code> | <code>Control.IsLiteralContent = &quot;false&quot;</code><br><code>Control.IsLiteralContent after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle…</code> |
| <code>Api.Control.IsTrackingViewState</code> | <code>Control.IsTrackingViewState = &quot;false&quot;</code> |
| <code>Api.Control.IsViewStateEnabled</code> | <code>Control.IsViewStateEnabled = &quot;true&quot;</code> |
| <code>Api.Control.Load</code> | <code>Control.Load = &quot;added and removed&quot;</code> |
| <code>Api.Control.LoadControlState</code> | <code>Control.LoadControlState(Object) = &quot;ok&quot;</code><br><code>Control.LoadControlState(Object) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bor…</code> |
| <code>Api.Control.LoadViewState</code> | <code>Control.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Object[]&#39;.&quot;</code> |
| <code>Api.Control.LoadViewStateByID</code> | <code>Control.LoadViewStateByID = &quot;false&quot;</code> |
| <code>Api.Control.MapPathSecure</code> | <code>Control.MapPathSecure(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.NamingContainer</code> | <code>Control.NamingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code> |
| <code>Api.Control.OnBubbleEvent</code> | <code>Control.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Control.OnBubbleEvent(Object,EventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empt…</code> |
| <code>Api.Control.OnDataBinding</code> | <code>Control.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnDataBinding(EventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bor…</code> |
| <code>Api.Control.OnInit</code> | <code>Control.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnInit(EventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyl…</code> |
| <code>Api.Control.OnLoad</code> | <code>Control.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnLoad(EventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyl…</code> |
| <code>Api.Control.OnPreRender</code> | <code>Control.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnPreRender(EventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Borde…</code> |
| <code>Api.Control.OnUnload</code> | <code>Control.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnUnload(EventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSt…</code> |
| <code>Api.Control.OpenFile</code> | <code>Control.OpenFile(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.Page</code> | <code>Control.Page = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=&quot;&quot;; …</code><br><code>Control.Page set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=…</code> |
| <code>Api.Control.Parent</code> | <code>Control.Parent = HtmlForm#form1 {Action=&quot;&quot;; ClientID=&quot;form1&quot;; ClientIDMode=Inherit; DefaultButton=&quot;&quot;; DefaultFocus=&quot;&quot;; Disabled=false; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.PreRender</code> | <code>Control.PreRender = &quot;added and removed&quot;</code> |
| <code>Api.Control.RaiseBubbleEvent</code> | <code>Control.RaiseBubbleEvent(Object,EventArgs) = &quot;ok&quot;</code><br><code>Control.RaiseBubbleEvent(Object,EventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [E…</code> |
| <code>Api.Control.RemovedControl</code> | <code>Control.RemovedControl(Control) = &quot;ok&quot;</code><br><code>Control.RemovedControl(Control) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bord…</code> |
| <code>Api.Control.Render</code> | <code>Control.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.Render(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.Render(HtmlTextWriter) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Borde…</code> |
| <code>Api.Control.RenderChildren</code> | <code>Control.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderChildren(HtmlTextWriter) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty…</code> |
| <code>Api.Control.RenderControl</code> | <code>Control.RenderControl(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderControl(HtmlTextWriter) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]…</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color…</code> |
| <code>Api.Control.RenderingCompatibility</code> | <code>Control.RenderingCompatibility = &quot;System.Version 4.0&quot;</code><br><code>Control.RenderingCompatibility set = &quot;System.Version 4.0&quot;</code> |
| <code>Api.Control.ResolveAdapter</code> | <code>Control.ResolveAdapter = null</code><br><code>Control.ResolveAdapter after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=N…</code> |
| <code>Api.Control.ResolveClientUrl</code> | <code>Control.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveClientUrl(String) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bor…</code> |
| <code>Api.Control.ResolveUrl</code> | <code>Control.ResolveUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveUrl(String) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSty…</code> |
| <code>Api.Control.SaveControlState</code> | <code>Control.SaveControlState = &quot;true&quot;</code><br><code>Control.SaveControlState after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle…</code> |
| <code>Api.Control.SaveViewState</code> | <code>Control.SaveViewState = &quot;[null, null]&quot;</code><br><code>Control.SaveViewState after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=No…</code> |
| <code>Api.Control.SetDesignModeState</code> | <code>Control.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>Control.SetDesignModeState(IDictionary) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empt…</code> |
| <code>Api.Control.SetRenderMethodDelegate</code> | <code>Control.SetRenderMethodDelegate(RenderMethod) = &quot;ok&quot;</code><br><code>Control.SetRenderMethodDelegate(RenderMethod) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color…</code> |
| <code>Api.Control.SetTraceData</code> | <code>Control.SetTraceData(Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; …</code><br><code>Control.SetTraceData(Object,Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object,Object) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [E…</code> |
| <code>Api.Control.Site</code> | <code>Control.Site = null</code><br><code>Control.Site set = null</code> |
| <code>Api.Control.SkinID</code> | <code>Control.SkinID = &quot;&quot;</code><br><code>Control.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.TemplateControl</code> | <code>Control.TemplateControl = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code><br><code>Control.TemplateControl set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Cl…</code> |
| <code>Api.Control.TemplateSourceDirectory</code> | <code>Control.TemplateSourceDirectory = &quot;&quot;</code> |
| <code>Api.Control.TrackViewState</code> | <code>Control.TrackViewState = &quot;ok&quot;</code><br><code>Control.TrackViewState after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=N…</code> |
| <code>Api.Control.UniqueID</code> | <code>Control.UniqueID = &quot;lvItems&quot;</code> |
| <code>Api.Control.Unload</code> | <code>Control.Unload = &quot;added and removed&quot;</code> |
| <code>Api.Control.ValidateRequestMode</code> | <code>Control.ValidateRequestMode = &quot;ValidateRequestMode.Inherit&quot;</code><br><code>Control.ValidateRequestMode set = &quot;ValidateRequestMode.Disabled&quot;</code> |
| <code>Api.Control.ViewState</code> | <code>Control.ViewState = StateBag []</code> |
| <code>Api.Control.ViewStateIgnoresCase</code> | <code>Control.ViewStateIgnoresCase = &quot;false&quot;</code> |
| <code>Api.Control.ViewStateMode</code> | <code>Control.ViewStateMode = &quot;ViewStateMode.Inherit&quot;</code><br><code>Control.ViewStateMode set = &quot;ViewStateMode.Enabled&quot;</code> |
| <code>Api.Control.Visible</code> | <code>Control.Visible = &quot;true&quot;</code><br><code>Control.Visible set = &quot;true&quot;</code> |
| <code>Api.IAttributeAccessor.GetAttribute</code> | <code>IAttributeAccessor.GetAttribute(String) = null</code><br><code>IAttributeAccessor.GetAttribute(String) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empt…</code> |
| <code>Api.IAttributeAccessor.SetAttribute</code> | <code>IAttributeAccessor.SetAttribute(String,String) = &quot;ok&quot;</code><br><code>IAttributeAccessor.SetAttribute(String,String) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Colo…</code> |
| <code>Api.IControlBuilderAccessor.ControlBuilder</code> | <code>IControlBuilderAccessor.ControlBuilder = null</code> |
| <code>Api.IControlDesignerAccessor.GetDesignModeState</code> | <code>IControlDesignerAccessor.GetDesignModeState = &quot;[]&quot;</code><br><code>IControlDesignerAccessor.GetDesignModeState after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code> |
| <code>Api.IControlDesignerAccessor.SetDesignModeState</code> | <code>IControlDesignerAccessor.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetDesignModeState(IDictionary) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #000…</code> |
| <code>Api.IControlDesignerAccessor.SetOwnerControl</code> | <code>IControlDesignerAccessor.SetOwnerControl(Control) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetOwnerControl(Control) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 C…</code> |
| <code>Api.IControlDesignerAccessor.UserData</code> | <code>IControlDesignerAccessor.UserData = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.DataBindings</code> | <code>IDataBindingsAccessor.DataBindings = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.HasDataBindings</code> | <code>IDataBindingsAccessor.HasDataBindings = &quot;false&quot;</code> |
| <code>Api.IDataKeysControl.ClientIDRowSuffix</code> | <code>IDataKeysControl.ClientIDRowSuffix = &quot;[]&quot;</code> |
| <code>Api.IDataKeysControl.ClientIDRowSuffixDataKeys</code> | <code>IDataKeysControl.ClientIDRowSuffixDataKeys = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.Expressions</code> | <code>IExpressionsAccessor.Expressions = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.HasExpressions</code> | <code>IExpressionsAccessor.HasExpressions = &quot;false&quot;</code> |
| <code>Api.IParserAccessor.AddParsedSubObject</code> | <code>IParserAccessor.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>IParserAccessor.AddParsedSubObject(Object) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [E…</code> |
| <code>Api.IUrlResolutionService.ResolveClientUrl</code> | <code>IUrlResolutionService.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>IUrlResolutionService.ResolveClientUrl(String) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Colo…</code> |
| <code>Api.WebControls.BaseDataBoundControl.#ctor</code> | <code>BaseDataBoundControl.#ctor = &quot;-&gt; System.MemberAccessException: Cannot create an instance of System.Web.UI.WebControls.BaseDataBoundControl because it is an abst…</code> |
| <code>Api.WebControls.BaseDataBoundControl.ConfirmInitState</code> | <code>BaseDataBoundControl.ConfirmInitState = &quot;ok&quot;</code><br><code>BaseDataBoundControl.ConfirmInitState after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]…</code> |
| <code>Api.WebControls.BaseDataBoundControl.DataBind</code> | <code>BaseDataBoundControl.DataBind = &quot;ok&quot;</code><br><code>BaseDataBoundControl.DataBind after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Border…</code> |
| <code>Api.WebControls.BaseDataBoundControl.DataBound</code> | <code>BaseDataBoundControl.DataBound = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.BaseDataBoundControl.DataSource</code> | <code>BaseDataBoundControl.DataSource = &quot;[System.String Pen, System.String Bag]&quot;</code><br><code>BaseDataBoundControl.DataSource set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.BaseDataBoundControl.DataSourceID</code> | <code>BaseDataBoundControl.DataSourceID = &quot;&quot;</code><br><code>BaseDataBoundControl.DataSourceID set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.BaseDataBoundControl.EnsureDataBound</code> | <code>BaseDataBoundControl.EnsureDataBound = &quot;ok&quot;</code><br><code>BaseDataBoundControl.EnsureDataBound after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty];…</code> |
| <code>Api.WebControls.BaseDataBoundControl.Initialized</code> | <code>BaseDataBoundControl.Initialized = &quot;false&quot;</code> |
| <code>Api.WebControls.BaseDataBoundControl.IsBoundUsingDataSourceID</code> | <code>BaseDataBoundControl.IsBoundUsingDataSourceID = &quot;false&quot;</code> |
| <code>Api.WebControls.BaseDataBoundControl.IsDataBindingAutomatic</code> | <code>BaseDataBoundControl.IsDataBindingAutomatic = &quot;false&quot;</code> |
| <code>Api.WebControls.BaseDataBoundControl.IsUsingModelBinders</code> | <code>BaseDataBoundControl.IsUsingModelBinders = &quot;false&quot;</code> |
| <code>Api.WebControls.BaseDataBoundControl.OnDataBound</code> | <code>BaseDataBoundControl.OnDataBound(EventArgs) = &quot;ok&quot;</code><br><code>BaseDataBoundControl.OnDataBound(EventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code> |
| <code>Api.WebControls.BaseDataBoundControl.OnDataPropertyChanged</code> | <code>BaseDataBoundControl.OnDataPropertyChanged = &quot;ok&quot;</code><br><code>BaseDataBoundControl.OnDataPropertyChanged after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [E…</code> |
| <code>Api.WebControls.BaseDataBoundControl.OnInit</code> | <code>BaseDataBoundControl.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>BaseDataBoundControl.OnInit(EventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty…</code> |
| <code>Api.WebControls.BaseDataBoundControl.OnPagePreLoad</code> | <code>BaseDataBoundControl.OnPagePreLoad(Object,EventArgs) = &quot;ok&quot;</code><br><code>BaseDataBoundControl.OnPagePreLoad(Object,EventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #0000000…</code> |
| <code>Api.WebControls.BaseDataBoundControl.OnPreRender</code> | <code>BaseDataBoundControl.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>BaseDataBoundControl.OnPreRender(EventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code> |
| <code>Api.WebControls.BaseDataBoundControl.PerformSelect</code> | <code>BaseDataBoundControl.PerformSelect = &quot;ok&quot;</code><br><code>BaseDataBoundControl.PerformSelect after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; B…</code> |
| <code>Api.WebControls.BaseDataBoundControl.RequiresDataBinding</code> | <code>BaseDataBoundControl.RequiresDataBinding = &quot;false&quot;</code><br><code>BaseDataBoundControl.RequiresDataBinding set = &quot;ok&quot;</code> |
| <code>Api.WebControls.BaseDataBoundControl.SupportsDisabledAttribute</code> | <code>BaseDataBoundControl.SupportsDisabledAttribute = &quot;false&quot;</code> |
| <code>Api.WebControls.BaseDataBoundControl.ValidateDataSource</code> | <code>BaseDataBoundControl.ValidateDataSource(Object) = &quot;ok&quot;</code><br><code>BaseDataBoundControl.ValidateDataSource(Object) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Col…</code> |
| <code>Api.WebControls.DataBoundControl.#ctor</code> | <code>DataBoundControl.#ctor = &quot;-&gt; System.MemberAccessException: Cannot create an instance of System.Web.UI.WebControls.DataBoundControl because it is an abstract cla…</code> |
| <code>Api.WebControls.DataBoundControl.CallingDataMethods</code> | <code>DataBoundControl.CallingDataMethods = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.DataBoundControl.CreateDataSourceSelectArguments</code> | <code>DataBoundControl.CreateDataSourceSelectArguments = DataSourceSelectArguments {MaximumRows=Int32 0; RetrieveTotalRowCount=false; SortExpression=&quot;&quot;; StartRowIndex…</code><br><code>DataBoundControl.CreateDataSourceSelectArguments after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Co…</code> |
| <code>Api.WebControls.DataBoundControl.CreatingModelDataSource</code> | <code>DataBoundControl.CreatingModelDataSource = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.DataBoundControl.DataMember</code> | <code>DataBoundControl.DataMember = &quot;&quot;</code><br><code>DataBoundControl.DataMember set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.DataBoundControl.DataSourceID</code> | <code>DataBoundControl.DataSourceID = &quot;&quot;</code><br><code>DataBoundControl.DataSourceID set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.DataBoundControl.DataSourceObject</code> | <code>DataBoundControl.DataSourceObject = null</code> |
| <code>Api.WebControls.DataBoundControl.GetData</code> | <code>DataBoundControl.GetData = ReadOnlyDataSourceView {CanDelete=false; CanInsert=false; CanPage=false; CanRetrieveTotalRowCount=false; CanSort=false; CanUpdate=fal…</code><br><code>DataBoundControl.GetData after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle…</code> |
| <code>Api.WebControls.DataBoundControl.GetDataSource</code> | <code>DataBoundControl.GetDataSource = null</code><br><code>DataBoundControl.GetDataSource after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Borde…</code> |
| <code>Api.WebControls.DataBoundControl.IsUsingModelBinders</code> | <code>DataBoundControl.IsUsingModelBinders = &quot;false&quot;</code> |
| <code>Api.WebControls.DataBoundControl.ItemType</code> | <code>DataBoundControl.ItemType = &quot;&quot;</code><br><code>DataBoundControl.ItemType set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.DataBoundControl.LoadViewState</code> | <code>DataBoundControl.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Object[]&#39;.&quot;</code> |
| <code>Api.WebControls.DataBoundControl.MarkAsDataBound</code> | <code>DataBoundControl.MarkAsDataBound = &quot;ok&quot;</code><br><code>DataBoundControl.MarkAsDataBound after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bor…</code> |
| <code>Api.WebControls.DataBoundControl.OnCreatingModelDataSource</code> | <code>DataBoundControl.OnCreatingModelDataSource(CreatingModelDataSourceEventArgs) = &quot;ok&quot;</code><br><code>DataBoundControl.OnCreatingModelDataSource(CreatingModelDataSourceEventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; Bo…</code> |
| <code>Api.WebControls.DataBoundControl.OnDataPropertyChanged</code> | <code>DataBoundControl.OnDataPropertyChanged = &quot;ok&quot;</code><br><code>DataBoundControl.OnDataPropertyChanged after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty…</code> |
| <code>Api.WebControls.DataBoundControl.OnDataSourceViewChanged</code> | <code>DataBoundControl.OnDataSourceViewChanged(Object,EventArgs) = &quot;ok&quot;</code><br><code>DataBoundControl.OnDataSourceViewChanged(Object,EventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #0…</code> |
| <code>Api.WebControls.DataBoundControl.OnLoad</code> | <code>DataBoundControl.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>DataBoundControl.OnLoad(EventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; B…</code> |
| <code>Api.WebControls.DataBoundControl.OnPagePreLoad</code> | <code>DataBoundControl.OnPagePreLoad(Object,EventArgs) = &quot;ok&quot;</code><br><code>DataBoundControl.OnPagePreLoad(Object,EventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Co…</code> |
| <code>Api.WebControls.DataBoundControl.PerformDataBinding</code> | <code>DataBoundControl.PerformDataBinding(IEnumerable) = &quot;ok&quot;</code><br><code>DataBoundControl.PerformDataBinding(IEnumerable) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Co…</code> |
| <code>Api.WebControls.DataBoundControl.PerformSelect</code> | <code>DataBoundControl.PerformSelect = &quot;ok&quot;</code><br><code>DataBoundControl.PerformSelect after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Borde…</code> |
| <code>Api.WebControls.DataBoundControl.SaveViewState</code> | <code>DataBoundControl.SaveViewState = &quot;[null, null]&quot;</code><br><code>DataBoundControl.SaveViewState after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Borde…</code> |
| <code>Api.WebControls.DataBoundControl.SelectArguments</code> | <code>DataBoundControl.SelectArguments = DataSourceSelectArguments {MaximumRows=Int32 0; RetrieveTotalRowCount=false; SortExpression=&quot;&quot;; StartRowIndex=Int32 0; TotalR…</code> |
| <code>Api.WebControls.DataBoundControl.SelectMethod</code> | <code>DataBoundControl.SelectMethod = &quot;&quot;</code><br><code>DataBoundControl.SelectMethod set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.DataBoundControl.TrackViewState</code> | <code>DataBoundControl.TrackViewState = &quot;ok&quot;</code><br><code>DataBoundControl.TrackViewState after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bord…</code> |
| <code>Api.WebControls.DataBoundControl.ValidateDataSource</code> | <code>DataBoundControl.ValidateDataSource(Object) = &quot;ok&quot;</code><br><code>DataBoundControl.ValidateDataSource(Object) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code> |
| <code>Api.WebControls.IDataBoundControl.DataKeyNames</code> | <code>IDataBoundControl.DataKeyNames = &quot;[]&quot;</code><br><code>IDataBoundControl.DataKeyNames set = &quot;[System.String a, System.String b]&quot;</code> |
| <code>Api.WebControls.IDataBoundControl.DataMember</code> | <code>IDataBoundControl.DataMember = &quot;&quot;</code><br><code>IDataBoundControl.DataMember set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.IDataBoundControl.DataSource</code> | <code>IDataBoundControl.DataSource = &quot;[System.String Pen, System.String Bag]&quot;</code><br><code>IDataBoundControl.DataSource set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.IDataBoundControl.DataSourceID</code> | <code>IDataBoundControl.DataSourceID = &quot;&quot;</code><br><code>IDataBoundControl.DataSourceID set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.IDataBoundControl.DataSourceObject</code> | <code>IDataBoundControl.DataSourceObject = null</code> |
| <code>Api.WebControls.IDataBoundListControl.ClientIDRowSuffix</code> | <code>IDataBoundListControl.ClientIDRowSuffix = &quot;[]&quot;</code><br><code>IDataBoundListControl.ClientIDRowSuffix set = &quot;[System.String a, System.String b]&quot;</code> |
| <code>Api.WebControls.IDataBoundListControl.DataKeys</code> | <code>IDataBoundListControl.DataKeys = &quot;[]&quot;</code> |
| <code>Api.WebControls.IDataBoundListControl.EnablePersistedSelection</code> | <code>IDataBoundListControl.EnablePersistedSelection = &quot;false&quot;</code><br><code>IDataBoundListControl.EnablePersistedSelection set = &quot;true&quot;</code> |
| <code>Api.WebControls.IDataBoundListControl.SelectedDataKey</code> | <code>IDataBoundListControl.SelectedDataKey = &quot;-&gt; System.InvalidOperationException: Data keys must be specified on ListView &#39;lvItems&#39; before the selected data keys ca…</code> |
| <code>Api.WebControls.IDataBoundListControl.SelectedIndex</code> | <code>IDataBoundListControl.SelectedIndex = &quot;Int32 -1&quot;</code><br><code>IDataBoundListControl.SelectedIndex set = &quot;Int32 1&quot;</code> |
| <code>Api.WebControls.IPageableItemContainer.MaximumRows</code> | <code>IPageableItemContainer.MaximumRows = &quot;Int32 -1&quot;</code> |
| <code>Api.WebControls.IPageableItemContainer.SetPageProperties</code> | <code>IPageableItemContainer.SetPageProperties(Int32,Int32,Boolean) = &quot;ok&quot;</code><br><code>IPageableItemContainer.SetPageProperties(Int32,Int32,Boolean) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color…</code> |
| <code>Api.WebControls.IPageableItemContainer.StartRowIndex</code> | <code>IPageableItemContainer.StartRowIndex = &quot;Int32 0&quot;</code> |
| <code>Api.WebControls.IPageableItemContainer.TotalRowCountAvailable</code> | <code>IPageableItemContainer.TotalRowCountAvailable = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.IPersistedSelector.DataKey</code> | <code>IPersistedSelector.DataKey = null</code><br><code>IPersistedSelector.DataKey set = DataKey {}</code> |
| <code>Api.WebControls.ListView.#ctor</code> | <code>ListView.#ctor = ListView# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; …</code> |
| <code>Api.WebControls.ListView.AccessKey</code> | <code>ListView.AccessKey = &quot;&quot;</code><br><code>ListView.AccessKey set = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside …</code> |
| <code>Api.WebControls.ListView.AddControlToContainer</code> | <code>ListView.AddControlToContainer(Control,Control,Int32) = &quot;-&gt; System.ArgumentOutOfRangeException: Specified argument was out of the range of valid values. [param …</code> |
| <code>Api.WebControls.ListView.AlternatingItemTemplate</code> | <code>ListView.AlternatingItemTemplate = null</code><br><code>ListView.AlternatingItemTemplate set = &quot;FrameworkOnCore.WebControlsParity.Component+SampleTemplate&quot;</code> |
| <code>Api.WebControls.ListView.BackColor</code> | <code>ListView.BackColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>ListView.BackColor set = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside …</code> |
| <code>Api.WebControls.ListView.BorderColor</code> | <code>ListView.BorderColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>ListView.BorderColor set = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements insid…</code> |
| <code>Api.WebControls.ListView.BorderStyle</code> | <code>ListView.BorderStyle = &quot;BorderStyle.NotSet&quot;</code><br><code>ListView.BorderStyle set = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements insid…</code> |
| <code>Api.WebControls.ListView.BorderWidth</code> | <code>ListView.BorderWidth = Unit </code><br><code>ListView.BorderWidth set = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements insid…</code> |
| <code>Api.WebControls.ListView.ClientIDRowSuffix</code> | <code>ListView.ClientIDRowSuffix = &quot;[]&quot;</code><br><code>ListView.ClientIDRowSuffix set = &quot;[System.String a, System.String b]&quot;</code> |
| <code>Api.WebControls.ListView.ClientIDRowSuffixDataKeys</code> | <code>ListView.ClientIDRowSuffixDataKeys = &quot;[]&quot;</code> |
| <code>Api.WebControls.ListView.Controls</code> | <code>ListView.Controls = ControlCollection []</code> |
| <code>Api.WebControls.ListView.ConvertEmptyStringToNull</code> | <code>ListView.ConvertEmptyStringToNull = &quot;true&quot;</code><br><code>ListView.ConvertEmptyStringToNull set = &quot;true&quot;</code> |
| <code>Api.WebControls.ListView.CreateChildControls</code> | <code>ListView.CreateChildControls = &quot;ok&quot;</code><br><code>ListView.CreateChildControls after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderS…</code><br><code>ListView.CreateChildControls(IEnumerable,Boolean) = &quot;Int32 2&quot;</code><br><code>ListView.CreateChildControls(IEnumerable,Boolean) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 C…</code> |
| <code>Api.WebControls.ListView.CreateControlStyle</code> | <code>ListView.CreateControlStyle = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.&quot;</code> |
| <code>Api.WebControls.ListView.CreateDataItem</code> | <code>ListView.CreateDataItem(Int32,Int32) = ListViewDataItem# {ClientID=&quot;&quot;; ClientIDMode=Inherit; DataItemIndex=Int32 0; DisplayIndex=Int32 0; EnableTheming=true; En…</code><br><code>ListView.CreateDataItem(Int32,Int32) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty];…</code> |
| <code>Api.WebControls.ListView.CreateDataSourceSelectArguments</code> | <code>ListView.CreateDataSourceSelectArguments = DataSourceSelectArguments {MaximumRows=Int32 0; RetrieveTotalRowCount=false; SortExpression=&quot;&quot;; StartRowIndex=Int32 0…</code><br><code>ListView.CreateDataSourceSelectArguments after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Emp…</code> |
| <code>Api.WebControls.ListView.CreateEmptyDataItem</code> | <code>ListView.CreateEmptyDataItem = &quot;ok&quot;</code><br><code>ListView.CreateEmptyDataItem after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderS…</code> |
| <code>Api.WebControls.ListView.CreateEmptyItem</code> | <code>ListView.CreateEmptyItem = null</code><br><code>ListView.CreateEmptyItem after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle…</code> |
| <code>Api.WebControls.ListView.CreateInsertItem</code> | <code>ListView.CreateInsertItem = &quot;-&gt; System.InvalidOperationException: An InsertItemTemplate must be defined on ListView &#39;lvItems&#39; if InsertItemPosition is set to Fi…</code> |
| <code>Api.WebControls.ListView.CreateItem</code> | <code>ListView.CreateItem(ListViewItemType) = ListViewItem# {ClientID=&quot;&quot;; ClientIDMode=Inherit; DataItemIndex=Int32 -1; DisplayIndex=Int32 -1; EnableTheming=true; Ena…</code><br><code>ListView.CreateItem(ListViewItemType) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]…</code> |
| <code>Api.WebControls.ListView.CreateItemsInGroups</code> | <code>ListView.CreateItemsInGroups(ListViewPagedDataSource,Boolean,InsertItemPosition,ArrayList) = &quot;-&gt; System.InvalidOperationException: An InsertItemTemplate must be…</code> |
| <code>Api.WebControls.ListView.CreateItemsWithoutGroups</code> | <code>ListView.CreateItemsWithoutGroups(ListViewPagedDataSource,Boolean,InsertItemPosition,ArrayList) = &quot;-&gt; System.InvalidOperationException: An InsertItemTemplate mu…</code> |
| <code>Api.WebControls.ListView.CreateLayoutTemplate</code> | <code>ListView.CreateLayoutTemplate = &quot;ok&quot;</code><br><code>ListView.CreateLayoutTemplate after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Border…</code> |
| <code>Api.WebControls.ListView.CreateSuffixArrayList</code> | <code>ListView.CreateSuffixArrayList(ListViewPagedDataSource,ArrayList) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.WebControls.ListView.CssClass</code> | <code>ListView.CssClass = &quot;&quot;</code><br><code>ListView.CssClass set = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside L…</code> |
| <code>Api.WebControls.ListView.DataKeyNames</code> | <code>ListView.DataKeyNames = &quot;[]&quot;</code><br><code>ListView.DataKeyNames set = &quot;[System.String a, System.String b]&quot;</code> |
| <code>Api.WebControls.ListView.DataKeys</code> | <code>ListView.DataKeys = &quot;[]&quot;</code> |
| <code>Api.WebControls.ListView.DeleteItem</code> | <code>ListView.DeleteItem(Int32) = &quot;-&gt; System.InvalidOperationException: The ListView &#39;lvItems&#39; raised event ItemDeleting which wasn&#39;t handled.&quot;</code> |
| <code>Api.WebControls.ListView.DeleteMethod</code> | <code>ListView.DeleteMethod = &quot;&quot;</code><br><code>ListView.DeleteMethod set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ListView.EditIndex</code> | <code>ListView.EditIndex = &quot;Int32 -1&quot;</code><br><code>ListView.EditIndex set = &quot;Int32 1&quot;</code> |
| <code>Api.WebControls.ListView.EditItem</code> | <code>ListView.EditItem = null</code> |
| <code>Api.WebControls.ListView.EditItemTemplate</code> | <code>ListView.EditItemTemplate = null</code><br><code>ListView.EditItemTemplate set = &quot;FrameworkOnCore.WebControlsParity.Component+SampleTemplate&quot;</code> |
| <code>Api.WebControls.ListView.EmptyDataTemplate</code> | <code>ListView.EmptyDataTemplate = null</code><br><code>ListView.EmptyDataTemplate set = &quot;FrameworkOnCore.WebControlsParity.Component+SampleTemplate&quot;</code> |
| <code>Api.WebControls.ListView.EmptyItemTemplate</code> | <code>ListView.EmptyItemTemplate = null</code><br><code>ListView.EmptyItemTemplate set = &quot;FrameworkOnCore.WebControlsParity.Component+SampleTemplate&quot;</code> |
| <code>Api.WebControls.ListView.EnableModelValidation</code> | <code>ListView.EnableModelValidation = &quot;true&quot;</code><br><code>ListView.EnableModelValidation set = &quot;true&quot;</code> |
| <code>Api.WebControls.ListView.EnablePersistedSelection</code> | <code>ListView.EnablePersistedSelection = &quot;false&quot;</code><br><code>ListView.EnablePersistedSelection set = &quot;true&quot;</code> |
| <code>Api.WebControls.ListView.EnsureLayoutTemplate</code> | <code>ListView.EnsureLayoutTemplate = &quot;ok&quot;</code><br><code>ListView.EnsureLayoutTemplate after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Border…</code> |
| <code>Api.WebControls.ListView.ExtractItemValues</code> | <code>ListView.ExtractItemValues(IOrderedDictionary,ListViewItem,Boolean) = &quot;-&gt; System.ArgumentNullException: Value cannot be null. [param itemValues]&quot;</code> |
| <code>Api.WebControls.ListView.FindPlaceholder</code> | <code>ListView.FindPlaceholder(String,Control) = null</code><br><code>ListView.FindPlaceholder(String,Control) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Emp…</code> |
| <code>Api.WebControls.ListView.Font</code> | <code>ListView.Font = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.&quot;</code> |
| <code>Api.WebControls.ListView.ForeColor</code> | <code>ListView.ForeColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>ListView.ForeColor set = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside …</code> |
| <code>Api.WebControls.ListView.GroupItemCount</code> | <code>ListView.GroupItemCount = &quot;Int32 1&quot;</code><br><code>ListView.GroupItemCount set = &quot;Int32 1&quot;</code> |
| <code>Api.WebControls.ListView.GroupPlaceholderID</code> | <code>ListView.GroupPlaceholderID = &quot;groupPlaceholder&quot;</code><br><code>ListView.GroupPlaceholderID set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ListView.GroupSeparatorTemplate</code> | <code>ListView.GroupSeparatorTemplate = null</code><br><code>ListView.GroupSeparatorTemplate set = &quot;FrameworkOnCore.WebControlsParity.Component+SampleTemplate&quot;</code> |
| <code>Api.WebControls.ListView.GroupTemplate</code> | <code>ListView.GroupTemplate = null</code><br><code>ListView.GroupTemplate set = &quot;FrameworkOnCore.WebControlsParity.Component+SampleTemplate&quot;</code> |
| <code>Api.WebControls.ListView.Height</code> | <code>ListView.Height = Unit </code><br><code>ListView.Height set = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside Lis…</code> |
| <code>Api.WebControls.ListView.InsertItem</code> | <code>ListView.InsertItem = null</code> |
| <code>Api.WebControls.ListView.InsertItemPosition</code> | <code>ListView.InsertItemPosition = &quot;InsertItemPosition.None&quot;</code><br><code>ListView.InsertItemPosition set = &quot;InsertItemPosition.FirstItem&quot;</code> |
| <code>Api.WebControls.ListView.InsertItemTemplate</code> | <code>ListView.InsertItemTemplate = null</code><br><code>ListView.InsertItemTemplate set = &quot;FrameworkOnCore.WebControlsParity.Component+SampleTemplate&quot;</code> |
| <code>Api.WebControls.ListView.InsertMethod</code> | <code>ListView.InsertMethod = &quot;&quot;</code><br><code>ListView.InsertMethod set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ListView.InsertNewItem</code> | <code>ListView.InsertNewItem(Boolean) = &quot;-&gt; System.InvalidOperationException: An insert item wasn&#39;t found.&quot;</code> |
| <code>Api.WebControls.ListView.InstantiateEmptyDataTemplate</code> | <code>ListView.InstantiateEmptyDataTemplate(Control) = &quot;ok&quot;</code><br><code>ListView.InstantiateEmptyDataTemplate(Control) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Colo…</code> |
| <code>Api.WebControls.ListView.InstantiateEmptyItemTemplate</code> | <code>ListView.InstantiateEmptyItemTemplate(Control) = &quot;ok&quot;</code><br><code>ListView.InstantiateEmptyItemTemplate(Control) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Colo…</code> |
| <code>Api.WebControls.ListView.InstantiateGroupSeparatorTemplate</code> | <code>ListView.InstantiateGroupSeparatorTemplate(Control) = &quot;ok&quot;</code><br><code>ListView.InstantiateGroupSeparatorTemplate(Control) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000…</code> |
| <code>Api.WebControls.ListView.InstantiateGroupTemplate</code> | <code>ListView.InstantiateGroupTemplate(Control) = &quot;ok&quot;</code><br><code>ListView.InstantiateGroupTemplate(Control) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [E…</code> |
| <code>Api.WebControls.ListView.InstantiateInsertItemTemplate</code> | <code>ListView.InstantiateInsertItemTemplate(Control) = &quot;ok&quot;</code><br><code>ListView.InstantiateInsertItemTemplate(Control) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Col…</code> |
| <code>Api.WebControls.ListView.InstantiateItemSeparatorTemplate</code> | <code>ListView.InstantiateItemSeparatorTemplate(Control) = &quot;ok&quot;</code><br><code>ListView.InstantiateItemSeparatorTemplate(Control) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 …</code> |
| <code>Api.WebControls.ListView.InstantiateItemTemplate</code> | <code>ListView.InstantiateItemTemplate(Control,Int32) = &quot;ok&quot;</code><br><code>ListView.InstantiateItemTemplate(Control,Int32) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Col…</code> |
| <code>Api.WebControls.ListView.IsUsingModelBinders</code> | <code>ListView.IsUsingModelBinders = &quot;false&quot;</code> |
| <code>Api.WebControls.ListView.ItemCanceling</code> | <code>ListView.ItemCanceling = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ListView.ItemCommand</code> | <code>ListView.ItemCommand = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ListView.ItemCreated</code> | <code>ListView.ItemCreated = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ListView.ItemDataBound</code> | <code>ListView.ItemDataBound = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ListView.ItemDeleted</code> | <code>ListView.ItemDeleted = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ListView.ItemDeleting</code> | <code>ListView.ItemDeleting = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ListView.ItemEditing</code> | <code>ListView.ItemEditing = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ListView.ItemInserted</code> | <code>ListView.ItemInserted = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ListView.ItemInserting</code> | <code>ListView.ItemInserting = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ListView.ItemPlaceholderID</code> | <code>ListView.ItemPlaceholderID = &quot;itemPlaceholder&quot;</code><br><code>ListView.ItemPlaceholderID set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ListView.ItemSeparatorTemplate</code> | <code>ListView.ItemSeparatorTemplate = null</code><br><code>ListView.ItemSeparatorTemplate set = &quot;FrameworkOnCore.WebControlsParity.Component+SampleTemplate&quot;</code> |
| <code>Api.WebControls.ListView.ItemTemplate</code> | <code>ListView.ItemTemplate = &quot;FrameworkOnCore.WebControlsParity.Component+SampleTemplate&quot;</code><br><code>ListView.ItemTemplate set = &quot;FrameworkOnCore.WebControlsParity.Component+SampleTemplate&quot;</code> |
| <code>Api.WebControls.ListView.ItemUpdated</code> | <code>ListView.ItemUpdated = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ListView.ItemUpdating</code> | <code>ListView.ItemUpdating = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ListView.Items</code> | <code>ListView.Items = &quot;[]&quot;</code> |
| <code>Api.WebControls.ListView.LayoutCreated</code> | <code>ListView.LayoutCreated = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ListView.LayoutTemplate</code> | <code>ListView.LayoutTemplate = null</code><br><code>ListView.LayoutTemplate set = &quot;FrameworkOnCore.WebControlsParity.Component+SampleTemplate&quot;</code> |
| <code>Api.WebControls.ListView.LoadControlState</code> | <code>ListView.LoadControlState(Object) = &quot;ok&quot;</code><br><code>ListView.LoadControlState(Object) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bo…</code> |
| <code>Api.WebControls.ListView.LoadViewState</code> | <code>ListView.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Object[]&#39;.&quot;</code> |
| <code>Api.WebControls.ListView.MaximumRows</code> | <code>ListView.MaximumRows = &quot;Int32 -1&quot;</code> |
| <code>Api.WebControls.ListView.OnBubbleEvent</code> | <code>ListView.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>ListView.OnBubbleEvent(Object,EventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Emp…</code> |
| <code>Api.WebControls.ListView.OnInit</code> | <code>ListView.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>ListView.OnInit(EventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderSty…</code> |
| <code>Api.WebControls.ListView.OnItemCanceling</code> | <code>ListView.OnItemCanceling(ListViewCancelEventArgs) = &quot;-&gt; System.InvalidOperationException: The ListView &#39;lvItems&#39; raised event ItemCanceling which wasn&#39;t handled…</code> |
| <code>Api.WebControls.ListView.OnItemCommand</code> | <code>ListView.OnItemCommand(ListViewCommandEventArgs) = &quot;ok&quot;</code><br><code>ListView.OnItemCommand(ListViewCommandEventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Co…</code> |
| <code>Api.WebControls.ListView.OnItemCreated</code> | <code>ListView.OnItemCreated(ListViewItemEventArgs) = &quot;ok&quot;</code><br><code>ListView.OnItemCreated(ListViewItemEventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color…</code> |
| <code>Api.WebControls.ListView.OnItemDataBound</code> | <code>ListView.OnItemDataBound(ListViewItemEventArgs) = &quot;ok&quot;</code><br><code>ListView.OnItemDataBound(ListViewItemEventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Col…</code> |
| <code>Api.WebControls.ListView.OnItemDeleted</code> | <code>ListView.OnItemDeleted(ListViewDeletedEventArgs) = &quot;ok&quot;</code><br><code>ListView.OnItemDeleted(ListViewDeletedEventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Co…</code> |
| <code>Api.WebControls.ListView.OnItemDeleting</code> | <code>ListView.OnItemDeleting(ListViewDeleteEventArgs) = &quot;-&gt; System.InvalidOperationException: The ListView &#39;lvItems&#39; raised event ItemDeleting which wasn&#39;t handled.&quot;</code> |
| <code>Api.WebControls.ListView.OnItemEditing</code> | <code>ListView.OnItemEditing(ListViewEditEventArgs) = &quot;-&gt; System.InvalidOperationException: The ListView &#39;lvItems&#39; raised event ItemEditing which wasn&#39;t handled.&quot;</code> |
| <code>Api.WebControls.ListView.OnItemInserted</code> | <code>ListView.OnItemInserted(ListViewInsertedEventArgs) = &quot;ok&quot;</code><br><code>ListView.OnItemInserted(ListViewInsertedEventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 …</code> |
| <code>Api.WebControls.ListView.OnItemInserting</code> | <code>ListView.OnItemInserting(ListViewInsertEventArgs) = &quot;-&gt; System.InvalidOperationException: The ListView &#39;lvItems&#39; raised event ItemInserting which wasn&#39;t handled…</code> |
| <code>Api.WebControls.ListView.OnItemUpdated</code> | <code>ListView.OnItemUpdated(ListViewUpdatedEventArgs) = &quot;ok&quot;</code><br><code>ListView.OnItemUpdated(ListViewUpdatedEventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Co…</code> |
| <code>Api.WebControls.ListView.OnItemUpdating</code> | <code>ListView.OnItemUpdating(ListViewUpdateEventArgs) = &quot;-&gt; System.InvalidOperationException: The ListView &#39;lvItems&#39; raised event ItemUpdating which wasn&#39;t handled.&quot;</code> |
| <code>Api.WebControls.ListView.OnLayoutCreated</code> | <code>ListView.OnLayoutCreated(EventArgs) = &quot;ok&quot;</code><br><code>ListView.OnLayoutCreated(EventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; …</code> |
| <code>Api.WebControls.ListView.OnPagePropertiesChanged</code> | <code>ListView.OnPagePropertiesChanged(EventArgs) = &quot;ok&quot;</code><br><code>ListView.OnPagePropertiesChanged(EventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code> |
| <code>Api.WebControls.ListView.OnPagePropertiesChanging</code> | <code>ListView.OnPagePropertiesChanging(PagePropertiesChangingEventArgs) = &quot;ok&quot;</code><br><code>ListView.OnPagePropertiesChanging(PagePropertiesChangingEventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=…</code> |
| <code>Api.WebControls.ListView.OnSelectedIndexChanged</code> | <code>ListView.OnSelectedIndexChanged(EventArgs) = &quot;ok&quot;</code><br><code>ListView.OnSelectedIndexChanged(EventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [E…</code> |
| <code>Api.WebControls.ListView.OnSelectedIndexChanging</code> | <code>ListView.OnSelectedIndexChanging(ListViewSelectEventArgs) = &quot;-&gt; System.InvalidOperationException: The ListView &#39;lvItems&#39; raised event SelectedIndexChanging whic…</code> |
| <code>Api.WebControls.ListView.OnSorted</code> | <code>ListView.OnSorted(EventArgs) = &quot;ok&quot;</code><br><code>ListView.OnSorted(EventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderS…</code> |
| <code>Api.WebControls.ListView.OnSorting</code> | <code>ListView.OnSorting(ListViewSortEventArgs) = &quot;-&gt; System.InvalidOperationException: The ListView &#39;lvItems&#39; raised event Sorting which wasn&#39;t handled.&quot;</code> |
| <code>Api.WebControls.ListView.OnTotalRowCountAvailable</code> | <code>ListView.OnTotalRowCountAvailable(PageEventArgs) = &quot;ok&quot;</code><br><code>ListView.OnTotalRowCountAvailable(PageEventArgs) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Co…</code> |
| <code>Api.WebControls.ListView.PagePropertiesChanged</code> | <code>ListView.PagePropertiesChanged = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ListView.PagePropertiesChanging</code> | <code>ListView.PagePropertiesChanging = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ListView.PerformDataBinding</code> | <code>ListView.PerformDataBinding(IEnumerable) = &quot;ok&quot;</code><br><code>ListView.PerformDataBinding(IEnumerable) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Emp…</code> |
| <code>Api.WebControls.ListView.PerformSelect</code> | <code>ListView.PerformSelect = &quot;ok&quot;</code><br><code>ListView.PerformSelect after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=N…</code> |
| <code>Api.WebControls.ListView.RemoveItems</code> | <code>ListView.RemoveItems = &quot;ok&quot;</code><br><code>ListView.RemoveItems after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=Not…</code> |
| <code>Api.WebControls.ListView.Render</code> | <code>ListView.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>ListView.Render(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>ListView.Render(HtmlTextWriter) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bord…</code> |
| <code>Api.WebControls.ListView.SaveControlState</code> | <code>ListView.SaveControlState = &quot;true&quot;</code><br><code>ListView.SaveControlState after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyl…</code> |
| <code>Api.WebControls.ListView.SaveViewState</code> | <code>ListView.SaveViewState = &quot;[null, null]&quot;</code><br><code>ListView.SaveViewState after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=N…</code> |
| <code>Api.WebControls.ListView.SelectItem</code> | <code>ListView.SelectItem(Int32) = &quot;-&gt; System.InvalidOperationException: The ListView &#39;lvItems&#39; raised event SelectedIndexChanging which wasn&#39;t handled.&quot;</code> |
| <code>Api.WebControls.ListView.SelectedDataKey</code> | <code>ListView.SelectedDataKey = &quot;-&gt; System.InvalidOperationException: Data keys must be specified on ListView &#39;lvItems&#39; before the selected data keys can be retrieve…</code> |
| <code>Api.WebControls.ListView.SelectedIndex</code> | <code>ListView.SelectedIndex = &quot;Int32 -1&quot;</code><br><code>ListView.SelectedIndex set = &quot;Int32 1&quot;</code> |
| <code>Api.WebControls.ListView.SelectedIndexChanged</code> | <code>ListView.SelectedIndexChanged = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ListView.SelectedIndexChanging</code> | <code>ListView.SelectedIndexChanging = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ListView.SelectedItemTemplate</code> | <code>ListView.SelectedItemTemplate = null</code><br><code>ListView.SelectedItemTemplate set = &quot;FrameworkOnCore.WebControlsParity.Component+SampleTemplate&quot;</code> |
| <code>Api.WebControls.ListView.SelectedPersistedDataKey</code> | <code>ListView.SelectedPersistedDataKey = null</code><br><code>ListView.SelectedPersistedDataKey set = DataKey {}</code> |
| <code>Api.WebControls.ListView.SelectedValue</code> | <code>ListView.SelectedValue = &quot;-&gt; System.InvalidOperationException: Data keys must be specified on ListView &#39;lvItems&#39; before the selected data keys can be retrieved.…</code> |
| <code>Api.WebControls.ListView.SetEditItem</code> | <code>ListView.SetEditItem(Int32) = &quot;-&gt; System.InvalidOperationException: The ListView &#39;lvItems&#39; raised event ItemEditing which wasn&#39;t handled.&quot;</code> |
| <code>Api.WebControls.ListView.SetPageProperties</code> | <code>ListView.SetPageProperties(Int32,Int32,Boolean) = &quot;ok&quot;</code><br><code>ListView.SetPageProperties(Int32,Int32,Boolean) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Col…</code> |
| <code>Api.WebControls.ListView.Sort</code> | <code>ListView.Sort(String,SortDirection) = &quot;-&gt; System.InvalidOperationException: The ListView &#39;lvItems&#39; raised event Sorting which wasn&#39;t handled.&quot;</code> |
| <code>Api.WebControls.ListView.SortDirection</code> | <code>ListView.SortDirection = &quot;SortDirection.Ascending&quot;</code> |
| <code>Api.WebControls.ListView.SortExpression</code> | <code>ListView.SortExpression = &quot;&quot;</code> |
| <code>Api.WebControls.ListView.Sorted</code> | <code>ListView.Sorted = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ListView.Sorting</code> | <code>ListView.Sorting = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.ListView.StartRowIndex</code> | <code>ListView.StartRowIndex = &quot;Int32 0&quot;</code> |
| <code>Api.WebControls.ListView.TabIndex</code> | <code>ListView.TabIndex = &quot;Int16 0&quot;</code><br><code>ListView.TabIndex set = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside L…</code> |
| <code>Api.WebControls.ListView.ToolTip</code> | <code>ListView.ToolTip = &quot;&quot;</code><br><code>ListView.ToolTip set = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside Li…</code> |
| <code>Api.WebControls.ListView.UpdateItem</code> | <code>ListView.UpdateItem(Int32,Boolean) = &quot;-&gt; System.InvalidOperationException: The ListView &#39;lvItems&#39; raised event ItemUpdating which wasn&#39;t handled.&quot;</code> |
| <code>Api.WebControls.ListView.UpdateMethod</code> | <code>ListView.UpdateMethod = &quot;&quot;</code><br><code>ListView.UpdateMethod set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.ListView.Width</code> | <code>ListView.Width = Unit </code><br><code>ListView.Width set = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside List…</code> |
| <code>Api.WebControls.WebControl.#ctor</code> | <code>WebControl.#ctor = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidt…</code><br><code>WebControl.#ctor(String) = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; Bo…</code><br><code>WebControl.#ctor(HtmlTextWriterTag) = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle…</code> |
| <code>Api.WebControls.WebControl.AccessKey</code> | <code>WebControl.AccessKey = &quot;&quot;</code><br><code>WebControl.AccessKey set = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements insid…</code> |
| <code>Api.WebControls.WebControl.AddAttributesToRender</code> | <code>WebControl.AddAttributesToRender(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.AddAttributesToRender(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>WebControl.AddAttributesToRender(HtmlTextWriter) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Co…</code> |
| <code>Api.WebControls.WebControl.ApplyStyle</code> | <code>WebControl.ApplyStyle(Style) = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.&quot;</code> |
| <code>Api.WebControls.WebControl.Attributes</code> | <code>WebControl.Attributes = AttributeCollection []</code> |
| <code>Api.WebControls.WebControl.BackColor</code> | <code>WebControl.BackColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>WebControl.BackColor set = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements insid…</code> |
| <code>Api.WebControls.WebControl.BorderColor</code> | <code>WebControl.BorderColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>WebControl.BorderColor set = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements ins…</code> |
| <code>Api.WebControls.WebControl.BorderStyle</code> | <code>WebControl.BorderStyle = &quot;BorderStyle.NotSet&quot;</code><br><code>WebControl.BorderStyle set = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements ins…</code> |
| <code>Api.WebControls.WebControl.BorderWidth</code> | <code>WebControl.BorderWidth = Unit </code><br><code>WebControl.BorderWidth set = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements ins…</code> |
| <code>Api.WebControls.WebControl.ControlStyle</code> | <code>WebControl.ControlStyle = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.&quot;</code> |
| <code>Api.WebControls.WebControl.ControlStyleCreated</code> | <code>WebControl.ControlStyleCreated = &quot;false&quot;</code> |
| <code>Api.WebControls.WebControl.CopyBaseAttributes</code> | <code>WebControl.CopyBaseAttributes(WebControl) = &quot;ok&quot;</code><br><code>WebControl.CopyBaseAttributes(WebControl) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Em…</code> |
| <code>Api.WebControls.WebControl.CreateControlStyle</code> | <code>WebControl.CreateControlStyle = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.&quot;</code> |
| <code>Api.WebControls.WebControl.CssClass</code> | <code>WebControl.CssClass = &quot;&quot;</code><br><code>WebControl.CssClass set = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside…</code> |
| <code>Api.WebControls.WebControl.DisabledCssClass</code> | <code>WebControl.DisabledCssClass = &quot;aspNetDisabled&quot;</code><br><code>WebControl.DisabledCssClass set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.EnableTheming</code> | <code>WebControl.EnableTheming = &quot;true&quot;</code><br><code>WebControl.EnableTheming set = &quot;true&quot;</code> |
| <code>Api.WebControls.WebControl.Enabled</code> | <code>WebControl.Enabled = &quot;true&quot;</code><br><code>WebControl.Enabled set = &quot;true&quot;</code> |
| <code>Api.WebControls.WebControl.Font</code> | <code>WebControl.Font = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.&quot;</code> |
| <code>Api.WebControls.WebControl.ForeColor</code> | <code>WebControl.ForeColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>WebControl.ForeColor set = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements insid…</code> |
| <code>Api.WebControls.WebControl.HasAttributes</code> | <code>WebControl.HasAttributes = &quot;false&quot;</code> |
| <code>Api.WebControls.WebControl.Height</code> | <code>WebControl.Height = Unit </code><br><code>WebControl.Height set = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside L…</code> |
| <code>Api.WebControls.WebControl.IsEnabled</code> | <code>WebControl.IsEnabled = &quot;true&quot;</code> |
| <code>Api.WebControls.WebControl.LoadViewState</code> | <code>WebControl.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Object[]&#39;.&quot;</code> |
| <code>Api.WebControls.WebControl.MergeStyle</code> | <code>WebControl.MergeStyle(Style) = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.&quot;</code> |
| <code>Api.WebControls.WebControl.Render</code> | <code>WebControl.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.Render(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>WebControl.Render(HtmlTextWriter) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; Bo…</code> |
| <code>Api.WebControls.WebControl.RenderBeginTag</code> | <code>WebControl.RenderBeginTag(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.RenderBeginTag(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;span id=&quot;lvItems&quot;&gt;</code><br><code>WebControl.RenderBeginTag(HtmlTextWriter) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Em…</code> |
| <code>Api.WebControls.WebControl.RenderContents</code> | <code>WebControl.RenderContents(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.RenderContents(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>WebControl.RenderContents(HtmlTextWriter) after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Em…</code> |
| <code>Api.WebControls.WebControl.RenderEndTag</code> | <code>WebControl.RenderEndTag(HtmlTextWriter) = &quot;-&gt; System.InvalidOperationException: A PopEndTag was called without a corresponding PushEndTag.&quot;</code> |
| <code>Api.WebControls.WebControl.SaveViewState</code> | <code>WebControl.SaveViewState = &quot;[null, null]&quot;</code><br><code>WebControl.SaveViewState after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle…</code> |
| <code>Api.WebControls.WebControl.SkinID</code> | <code>WebControl.SkinID = &quot;&quot;</code><br><code>WebControl.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.Style</code> | <code>WebControl.Style = CssStyleCollection {Count=Int32 0; Value=null}</code> |
| <code>Api.WebControls.WebControl.SupportsDisabledAttribute</code> | <code>WebControl.SupportsDisabledAttribute = &quot;false&quot;</code> |
| <code>Api.WebControls.WebControl.TabIndex</code> | <code>WebControl.TabIndex = &quot;Int16 0&quot;</code><br><code>WebControl.TabIndex set = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside…</code> |
| <code>Api.WebControls.WebControl.TagKey</code> | <code>WebControl.TagKey = &quot;HtmlTextWriterTag.Span&quot;</code> |
| <code>Api.WebControls.WebControl.TagName</code> | <code>WebControl.TagName = &quot;span&quot;</code> |
| <code>Api.WebControls.WebControl.ToolTip</code> | <code>WebControl.ToolTip = &quot;&quot;</code><br><code>WebControl.ToolTip set = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside …</code> |
| <code>Api.WebControls.WebControl.TrackViewState</code> | <code>WebControl.TrackViewState = &quot;ok&quot;</code><br><code>WebControl.TrackViewState after = ListView#lvItems {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyl…</code> |
| <code>Api.WebControls.WebControl.Width</code> | <code>WebControl.Width = Unit </code><br><code>WebControl.Width set = &quot;-&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside Li…</code> |

## ② HTML 層

プローブサイトの RenderProbe.aspx で、1 ケース 1 リクエスト（ページライフサイクル全体）で描画したページ全体を比較します。下表はコンポーネント部分（div#probe）、設定時の例外、保存された ViewState の要約です。

| ケース | 設定 | 期待される描画 | 設定時の例外 |
|---|---|---|---|
| <code>.(default)</code> | <code>(nothing set)</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.AccessKey=text</code> | <code>c.AccessKey = &quot;probe&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.AccessKey = &quot;probe&quot;; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.AccessKey=markup</code> | <code>c.AccessKey = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.AccessKey = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.AccessKey=japanese</code> | <code>c.AccessKey = &quot;日本語テキスト&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.AccessKey = &quot;日本語テキスト&quot;; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.AccessKey=empty</code> | <code>c.AccessKey = &quot;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.AccessKey = &quot;&quot;; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.AppRelativeTemplateSourceDirectory=text</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;probe&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=markup</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=japanese</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;日本語テキスト&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=empty</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.BackColor=Red</code> | <code>c.BackColor = Color.Red;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.BackColor = Color.Red; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.BackColor=#3366CC</code> | <code>c.BackColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.BackColor = Color.FromArgb(0x33, 0x66, 0xCC); -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.BackColor=Transparent</code> | <code>c.BackColor = Color.Transparent;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.BackColor = Color.Transparent; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.BorderColor=Red</code> | <code>c.BorderColor = Color.Red;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.BorderColor = Color.Red; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.BorderColor=#3366CC</code> | <code>c.BorderColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.BorderColor = Color.FromArgb(0x33, 0x66, 0xCC); -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.BorderColor=Transparent</code> | <code>c.BorderColor = Color.Transparent;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.BorderColor = Color.Transparent; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.BorderStyle=NotSet</code> | <code>c.BorderStyle = BorderStyle.NotSet;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.BorderStyle = BorderStyle.NotSet; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.BorderStyle=None</code> | <code>c.BorderStyle = BorderStyle.None;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.BorderStyle = BorderStyle.None; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.BorderStyle=Dotted</code> | <code>c.BorderStyle = BorderStyle.Dotted;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.BorderStyle = BorderStyle.Dotted; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.BorderStyle=Dashed</code> | <code>c.BorderStyle = BorderStyle.Dashed;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.BorderStyle = BorderStyle.Dashed; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.BorderStyle=Solid</code> | <code>c.BorderStyle = BorderStyle.Solid;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.BorderStyle = BorderStyle.Solid; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.BorderStyle=Double</code> | <code>c.BorderStyle = BorderStyle.Double;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.BorderStyle = BorderStyle.Double; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.BorderStyle=Groove</code> | <code>c.BorderStyle = BorderStyle.Groove;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.BorderStyle = BorderStyle.Groove; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.BorderStyle=Ridge</code> | <code>c.BorderStyle = BorderStyle.Ridge;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.BorderStyle = BorderStyle.Ridge; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.BorderStyle=Inset</code> | <code>c.BorderStyle = BorderStyle.Inset;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.BorderStyle = BorderStyle.Inset; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.BorderStyle=Outset</code> | <code>c.BorderStyle = BorderStyle.Outset;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.BorderStyle = BorderStyle.Outset; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.BorderWidth=120px</code> | <code>c.BorderWidth = Unit.Pixel(120);</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.BorderWidth = Unit.Pixel(120); -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.BorderWidth=50%</code> | <code>c.BorderWidth = Unit.Percentage(50);</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.BorderWidth = Unit.Percentage(50); -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.BorderWidth=2em</code> | <code>c.BorderWidth = new Unit(2, UnitType.Em);</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.BorderWidth = new Unit(2, UnitType.Em); -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.ClientIDMode=Inherit</code> | <code>c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ClientIDMode=AutoID</code> | <code>c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ClientIDMode=Predictable</code> | <code>c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ClientIDMode=Static</code> | <code>c.ClientIDMode = ClientIDMode.Static;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ClientIDRowSuffix=Arial,MS-Gothic</code> | <code>c.ClientIDRowSuffix = new[] { &quot;Arial&quot;, &quot;MS Gothic&quot; };</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>DataBind -&gt; System.Web.HttpException: DataBinding: &#39;WebControlsProbe.ProbeData+Product&#39; does not contain a property with the name &#39;Arial&#39;.</code> |
| <code>.ConvertEmptyStringToNull=true</code> | <code>c.ConvertEmptyStringToNull = true;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ConvertEmptyStringToNull=false</code> | <code>c.ConvertEmptyStringToNull = false;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.CssClass=text</code> | <code>c.CssClass = &quot;probe&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.CssClass = &quot;probe&quot;; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.CssClass=markup</code> | <code>c.CssClass = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.CssClass = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.CssClass=japanese</code> | <code>c.CssClass = &quot;日本語テキスト&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.CssClass = &quot;日本語テキスト&quot;; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.CssClass=empty</code> | <code>c.CssClass = &quot;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.CssClass = &quot;&quot;; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.DataKeyNames=Arial,MS-Gothic</code> | <code>c.DataKeyNames = new[] { &quot;Arial&quot;, &quot;MS Gothic&quot; };</code> | <code></code> | <code>DataBind -&gt; System.Web.HttpException: DataBinding: &#39;WebControlsProbe.ProbeData+Product&#39; does not contain a property with the name &#39;Arial&#39;.</code> |
| <code>.DataKeyNames=Code</code> | <code>c.DataKeyNames = new[] { &quot;Code&quot; };</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.DataMember=text</code> | <code>c.DataMember = &quot;probe&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.DataMember=markup</code> | <code>c.DataMember = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.DataMember=japanese</code> | <code>c.DataMember = &quot;日本語テキスト&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.DataMember=empty</code> | <code>c.DataMember = &quot;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.DataSourceID=text</code> | <code>c.DataSourceID = &quot;probe&quot;;</code> | <code>status = 500</code> |  |
| <code>.DataSourceID=markup</code> | <code>c.DataSourceID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>status = 500</code> |  |
| <code>.DataSourceID=japanese</code> | <code>c.DataSourceID = &quot;日本語テキスト&quot;;</code> | <code>status = 500</code> |  |
| <code>.DataSourceID=empty</code> | <code>c.DataSourceID = &quot;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.DeleteMethod=text</code> | <code>c.DeleteMethod = &quot;probe&quot;;</code> | <code>status = 500</code> |  |
| <code>.DeleteMethod=markup</code> | <code>c.DeleteMethod = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>status = 500</code> |  |
| <code>.DeleteMethod=japanese</code> | <code>c.DeleteMethod = &quot;日本語テキスト&quot;;</code> | <code>status = 500</code> |  |
| <code>.DeleteMethod=empty</code> | <code>c.DeleteMethod = &quot;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.EditIndex=0</code> | <code>c.EditIndex = 0;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.EditIndex=3</code> | <code>c.EditIndex = 3;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.EditIndex=-1</code> | <code>c.EditIndex = -1;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.EditIndex=1</code> | <code>c.EditIndex = 1;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.EnableModelValidation=true</code> | <code>c.EnableModelValidation = true;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.EnableModelValidation=false</code> | <code>c.EnableModelValidation = false;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.EnablePersistedSelection=true</code> | <code>c.EnablePersistedSelection = true;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>DataBind -&gt; System.InvalidOperationException: DataKeyNames must be specified for persisted selection to work.</code> |
| <code>.EnablePersistedSelection=false</code> | <code>c.EnablePersistedSelection = false;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.EnableTheming=true</code> | <code>c.EnableTheming = true;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.EnableTheming=false</code> | <code>c.EnableTheming = false;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.EnableViewState=true</code> | <code>c.EnableViewState = true;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.EnableViewState=false</code> | <code>c.EnableViewState = false;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.Enabled=true</code> | <code>c.Enabled = true;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.Enabled=false</code> | <code>c.Enabled = false;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.Font.Bold=true</code> | <code>c.Font.Bold = true;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.Font.Bold = true; -&gt; System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation.</code> |
| <code>.Font.Bold=false</code> | <code>c.Font.Bold = false;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.Font.Bold = false; -&gt; System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation.</code> |
| <code>.Font.Italic=true</code> | <code>c.Font.Italic = true;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.Font.Italic = true; -&gt; System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation.</code> |
| <code>.Font.Italic=false</code> | <code>c.Font.Italic = false;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.Font.Italic = false; -&gt; System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation.</code> |
| <code>.Font.Name=text</code> | <code>c.Font.Name = &quot;probe&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.Font.Name = &quot;probe&quot;; -&gt; System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation.</code> |
| <code>.Font.Name=markup</code> | <code>c.Font.Name = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.Font.Name = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation.</code> |
| <code>.Font.Name=japanese</code> | <code>c.Font.Name = &quot;日本語テキスト&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.Font.Name = &quot;日本語テキスト&quot;; -&gt; System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation.</code> |
| <code>.Font.Name=empty</code> | <code>c.Font.Name = &quot;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.Font.Name = &quot;&quot;; -&gt; System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation.</code> |
| <code>.Font.Names=Arial,MS-Gothic</code> | <code>c.Font.Names = new[] { &quot;Arial&quot;, &quot;MS Gothic&quot; };</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.Font.Names = new[] { &quot;Arial&quot;, &quot;MS Gothic&quot; }; -&gt; System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation.</code> |
| <code>.Font.Overline=true</code> | <code>c.Font.Overline = true;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.Font.Overline = true; -&gt; System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation.</code> |
| <code>.Font.Overline=false</code> | <code>c.Font.Overline = false;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.Font.Overline = false; -&gt; System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation.</code> |
| <code>.Font.Size=12pt</code> | <code>c.Font.Size = FontUnit.Point(12);</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.Font.Size = FontUnit.Point(12); -&gt; System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation.</code> |
| <code>.Font.Size=Large</code> | <code>c.Font.Size = FontUnit.Large;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.Font.Size = FontUnit.Large; -&gt; System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation.</code> |
| <code>.Font.Size=150%</code> | <code>c.Font.Size = new FontUnit(Unit.Percentage(150));</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.Font.Size = new FontUnit(Unit.Percentage(150)); -&gt; System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation.</code> |
| <code>.Font.Strikeout=true</code> | <code>c.Font.Strikeout = true;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.Font.Strikeout = true; -&gt; System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation.</code> |
| <code>.Font.Strikeout=false</code> | <code>c.Font.Strikeout = false;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.Font.Strikeout = false; -&gt; System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation.</code> |
| <code>.Font.Underline=true</code> | <code>c.Font.Underline = true;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.Font.Underline = true; -&gt; System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation.</code> |
| <code>.Font.Underline=false</code> | <code>c.Font.Underline = false;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.Font.Underline = false; -&gt; System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation.</code> |
| <code>.ForeColor=Red</code> | <code>c.ForeColor = Color.Red;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.ForeColor = Color.Red; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.ForeColor=#3366CC</code> | <code>c.ForeColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.ForeColor = Color.FromArgb(0x33, 0x66, 0xCC); -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.ForeColor=Transparent</code> | <code>c.ForeColor = Color.Transparent;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.ForeColor = Color.Transparent; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.GroupItemCount=0</code> | <code>c.GroupItemCount = 0;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.GroupItemCount = 0; -&gt; System.ArgumentOutOfRangeException: Specified argument was out of the range of valid values. [param value]</code> |
| <code>.GroupItemCount=3</code> | <code>c.GroupItemCount = 3;</code> | <code></code> | <code>DataBind -&gt; System.InvalidOperationException: ListView &#39;lvItems&#39; has a GroupItemCount specified on it but no GroupTemplate. A GroupTemplate must be present for ListView to render groups.</code> |
| <code>.GroupItemCount=-1</code> | <code>c.GroupItemCount = -1;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.GroupItemCount = -1; -&gt; System.ArgumentOutOfRangeException: Specified argument was out of the range of valid values. [param value]</code> |
| <code>.GroupItemCount=1</code> | <code>c.GroupItemCount = 1;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.GroupPlaceholderID=text</code> | <code>c.GroupPlaceholderID = &quot;probe&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.GroupPlaceholderID=markup</code> | <code>c.GroupPlaceholderID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.GroupPlaceholderID=japanese</code> | <code>c.GroupPlaceholderID = &quot;日本語テキスト&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.GroupPlaceholderID=empty</code> | <code>c.GroupPlaceholderID = &quot;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.GroupPlaceholderID = &quot;&quot;; -&gt; System.ArgumentOutOfRangeException: The value of GroupPlaceholderID must not be empty. [param value]</code> |
| <code>.Height=120px</code> | <code>c.Height = Unit.Pixel(120);</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.Height = Unit.Pixel(120); -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.Height=50%</code> | <code>c.Height = Unit.Percentage(50);</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.Height = Unit.Percentage(50); -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.Height=2em</code> | <code>c.Height = new Unit(2, UnitType.Em);</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.Height = new Unit(2, UnitType.Em); -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.ID=text</code> | <code>c.ID = &quot;probe&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ID=markup</code> | <code>c.ID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ID=japanese</code> | <code>c.ID = &quot;日本語テキスト&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ID=empty</code> | <code>c.ID = &quot;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.InsertItemPosition=None</code> | <code>c.InsertItemPosition = InsertItemPosition.None;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.InsertItemPosition=FirstItem</code> | <code>c.InsertItemPosition = InsertItemPosition.FirstItem;</code> | <code></code> | <code>DataBind -&gt; System.InvalidOperationException: An InsertItemTemplate must be defined on ListView &#39;lvItems&#39; if InsertItemPosition is set to FirstItem or LastItem.</code> |
| <code>.InsertItemPosition=LastItem</code> | <code>c.InsertItemPosition = InsertItemPosition.LastItem;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>DataBind -&gt; System.InvalidOperationException: An InsertItemTemplate must be defined on ListView &#39;lvItems&#39; if InsertItemPosition is set to FirstItem or LastItem.</code> |
| <code>.InsertMethod=text</code> | <code>c.InsertMethod = &quot;probe&quot;;</code> | <code>status = 500</code> |  |
| <code>.InsertMethod=markup</code> | <code>c.InsertMethod = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>status = 500</code> |  |
| <code>.InsertMethod=japanese</code> | <code>c.InsertMethod = &quot;日本語テキスト&quot;;</code> | <code>status = 500</code> |  |
| <code>.InsertMethod=empty</code> | <code>c.InsertMethod = &quot;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ItemPlaceholderID=text</code> | <code>c.ItemPlaceholderID = &quot;probe&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ItemPlaceholderID=markup</code> | <code>c.ItemPlaceholderID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ItemPlaceholderID=japanese</code> | <code>c.ItemPlaceholderID = &quot;日本語テキスト&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ItemPlaceholderID=empty</code> | <code>c.ItemPlaceholderID = &quot;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.ItemPlaceholderID = &quot;&quot;; -&gt; System.ArgumentOutOfRangeException: The value of ItemPlaceholderID must not be empty. [param value]</code> |
| <code>.ItemType=text</code> | <code>c.ItemType = &quot;probe&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ItemType=markup</code> | <code>c.ItemType = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ItemType=japanese</code> | <code>c.ItemType = &quot;日本語テキスト&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ItemType=empty</code> | <code>c.ItemType = &quot;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.SelectMethod=text</code> | <code>c.SelectMethod = &quot;probe&quot;;</code> | <code>status = 500</code> |  |
| <code>.SelectMethod=markup</code> | <code>c.SelectMethod = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>status = 500</code> |  |
| <code>.SelectMethod=japanese</code> | <code>c.SelectMethod = &quot;日本語テキスト&quot;;</code> | <code>status = 500</code> |  |
| <code>.SelectMethod=empty</code> | <code>c.SelectMethod = &quot;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.SelectedIndex=0</code> | <code>c.SelectedIndex = 0;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.SelectedIndex=3</code> | <code>c.SelectedIndex = 3;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.SelectedIndex=-1</code> | <code>c.SelectedIndex = -1;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.SelectedIndex=1</code> | <code>c.SelectedIndex = 1;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.SkinID=text</code> | <code>c.SkinID = &quot;probe&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.SkinID=markup</code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.SkinID=japanese</code> | <code>c.SkinID = &quot;日本語テキスト&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.SkinID=empty</code> | <code>c.SkinID = &quot;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.TabIndex=0</code> | <code>c.TabIndex = 0;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.TabIndex = 0; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.TabIndex=3</code> | <code>c.TabIndex = 3;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.TabIndex = 3; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.TabIndex=-1</code> | <code>c.TabIndex = -1;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.TabIndex = -1; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.ToolTip=text</code> | <code>c.ToolTip = &quot;probe&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.ToolTip = &quot;probe&quot;; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.ToolTip=markup</code> | <code>c.ToolTip = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.ToolTip = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.ToolTip=japanese</code> | <code>c.ToolTip = &quot;日本語テキスト&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.ToolTip = &quot;日本語テキスト&quot;; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.ToolTip=empty</code> | <code>c.ToolTip = &quot;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.ToolTip = &quot;&quot;; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.UpdateMethod=text</code> | <code>c.UpdateMethod = &quot;probe&quot;;</code> | <code>status = 500</code> |  |
| <code>.UpdateMethod=markup</code> | <code>c.UpdateMethod = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>status = 500</code> |  |
| <code>.UpdateMethod=japanese</code> | <code>c.UpdateMethod = &quot;日本語テキスト&quot;;</code> | <code>status = 500</code> |  |
| <code>.UpdateMethod=empty</code> | <code>c.UpdateMethod = &quot;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ValidateRequestMode=Inherit</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Inherit;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ValidateRequestMode=Disabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Disabled;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ValidateRequestMode=Enabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Enabled;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ViewStateMode=Inherit</code> | <code>c.ViewStateMode = ViewStateMode.Inherit;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ViewStateMode=Enabled</code> | <code>c.ViewStateMode = ViewStateMode.Enabled;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.ViewStateMode=Disabled</code> | <code>c.ViewStateMode = ViewStateMode.Disabled;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.Visible=true</code> | <code>c.Visible = true;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.Visible=false</code> | <code>c.Visible = false;</code> | <code></code> |  |
| <code>.Width=120px</code> | <code>c.Width = Unit.Pixel(120);</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.Width = Unit.Pixel(120); -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.Width=50%</code> | <code>c.Width = Unit.Percentage(50);</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.Width = Unit.Percentage(50); -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.Width=2em</code> | <code>c.Width = new Unit(2, UnitType.Em);</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.Width = new Unit(2, UnitType.Em); -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>.Attributes[data-probe]=text</code> | <code>c.Attributes[data-probe] = &quot;probe&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.Attributes[onfocus]=text</code> | <code>c.Attributes[onfocus] = &quot;probe&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.Attributes[data-probe]=markup</code> | <code>c.Attributes[data-probe] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.Attributes[onfocus]=markup</code> | <code>c.Attributes[onfocus] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.Style[color]=red</code> | <code>c.Style[color] = &quot;red&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.Style[margin-left]=red</code> | <code>c.Style[margin-left] = &quot;red&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.Style[color]=1em</code> | <code>c.Style[color] = &quot;1em&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.Style[margin-left]=1em</code> | <code>c.Style[margin-left] = &quot;1em&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.EnableViewState=false+SelectedIndex=1</code> | <code>c.EnableViewState = false; c.SelectedIndex = 1;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>.Visible=false+SelectedIndex=1</code> | <code>c.Visible = false; c.SelectedIndex = 1;</code> | <code></code> |  |
| <code>[naming container].(default)</code> | <code>[naming container] (nothing set)</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=AutoID</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Static</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Predictable</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Inherit</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]↵&lt;/div&gt;</code> |  |
| <code>[naming container].ID=text</code> | <code>[naming container] c.ID = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]↵&lt;/div&gt;</code> |  |
| <code>[themed].(default)</code> | <code>[themed] (nothing set)</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>[themed].SkinID=text</code> | <code>[themed] c.SkinID = &quot;probe&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>[themed].EnableTheming=false</code> | <code>[themed] c.EnableTheming = false;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>[themed].CssClass=text</code> | <code>[themed] c.CssClass = &quot;probe&quot;;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> | <code>c.CssClass = &quot;probe&quot;; -&gt; System.NotSupportedException: Style properties are not supported on ListView.  Apply styling or CSS classes to the elements inside ListView&#39;s templates.</code> |
| <code>[layout].(default)</code> | <code>[layout] (nothing set)</code> | <code>&lt;ul class=&quot;layout&quot;&gt;[item: Pen (P1)][alternating: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][alternating: Desk (D4)][item: Eraser (E5)][alternating: File (F6)][item: Glue (G7)]&lt;/ul&gt;</code> |  |
| <code>[layout].ItemPlaceholderID=text</code> | <code>[layout] c.ItemPlaceholderID = &quot;probe&quot;;</code> | <code>&lt;ul class=&quot;layout&quot;&gt;&lt;/ul&gt;</code> | <code>DataBind -&gt; System.InvalidOperationException: An item placeholder must be specified on ListView &#39;lvItems&#39;. Specify an item placeholder by setting a control&#39;s ID property to &quot;probe&quot;. The item placeholder control must also specify runat=&quot;server&quot;.</code> |
| <code>[groups].(default)</code> | <code>[groups] (nothing set)</code> | <code>&lt;table class=&quot;groups&quot;&gt;&lt;tr&gt;[cell: Pen (P1)][cell: Bag (B2)][cell: Cup &lt;&amp;&gt; (C3)]&lt;/tr&gt;&lt;tr&gt;[cell: Desk (D4)][cell: Eraser (E5)][cell: File (F6)]&lt;/tr&gt;&lt;tr&gt;[cell: Glue (G7)]&lt;/tr&gt;&lt;/table&gt;</code> |  |
| <code>[groups].GroupItemCount=1</code> | <code>[groups] c.GroupItemCount = 1;</code> | <code>&lt;table class=&quot;groups&quot;&gt;&lt;tr&gt;[cell: Pen (P1)]&lt;/tr&gt;&lt;tr&gt;[cell: Bag (B2)]&lt;/tr&gt;&lt;tr&gt;[cell: Cup &lt;&amp;&gt; (C3)]&lt;/tr&gt;&lt;tr&gt;[cell: Desk (D4)]&lt;/tr&gt;&lt;tr&gt;[cell: Eraser (E5)]&lt;/tr&gt;&lt;tr&gt;[cell: File (F6)]&lt;/tr&gt;&lt;tr&gt;[cell: Glue (G7)]&lt;/tr&gt;&lt;/table&gt;</code> |  |
| <code>[groups].GroupPlaceholderID=text</code> | <code>[groups] c.GroupPlaceholderID = &quot;probe&quot;;</code> | <code>&lt;table class=&quot;groups&quot;&gt;&lt;/table&gt;</code> | <code>DataBind -&gt; System.InvalidOperationException: A group placeholder must be specified on ListView &#39;lvItems&#39; when the GroupTemplate is defined. Specify a group placeholder by setting its ID property to &quot;probe&quot;. The group placeholder control must also specify runat=&quot;server&quot;.</code> |
| <code>[editing].(default)</code> | <code>[editing] (nothing set)</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)][insert]</code> |  |
| <code>[editing].EditIndex=1</code> | <code>[editing] c.EditIndex = 1;</code> | <code>[item: Pen (P1)][edit: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)][insert]</code> |  |
| <code>[editing].SelectedIndex=1</code> | <code>[editing] c.SelectedIndex = 1;</code> | <code>[item: Pen (P1)][selected: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)][insert]</code> |  |
| <code>[editing].InsertItemPosition=FirstItem</code> | <code>[editing] c.InsertItemPosition = InsertItemPosition.FirstItem;</code> | <code>[insert][item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>[editing].InsertItemPosition=None</code> | <code>[editing] c.InsertItemPosition = InsertItemPosition.None;</code> | <code>[item: Pen (P1)][item: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)]</code> |  |
| <code>[editing].EditIndex=1+DataKeyNames=Code</code> | <code>[editing] c.EditIndex = 1; c.DataKeyNames = new[] { &quot;Code&quot; };</code> | <code>[item: Pen (P1)][edit: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)][insert]</code> |  |
| <code>[editing].SelectedIndex=1+DataKeyNames=Code</code> | <code>[editing] c.SelectedIndex = 1; c.DataKeyNames = new[] { &quot;Code&quot; };</code> | <code>[item: Pen (P1)][selected: Bag (B2)][item: Cup &lt;&amp;&gt; (C3)][item: Desk (D4)][item: Eraser (E5)][item: File (F6)][item: Glue (G7)][insert]</code> |  |
| <code>[empty].(default)</code> | <code>[empty] (nothing set)</code> | <code></code> |  |
| <code>[empty].InsertItemPosition=FirstItem</code> | <code>[empty] c.InsertItemPosition = InsertItemPosition.FirstItem;</code> | <code></code> | <code>DataBind -&gt; System.InvalidOperationException: An InsertItemTemplate must be defined on ListView &#39;lvItems&#39; if InsertItemPosition is set to FirstItem or LastItem.</code> |

## ③ シナリオ層

ブラウザ（Playwright）で ListViewScenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。

| # | 操作 | 対象 | 値 |
|---|---|---|---|
| 1 | goto | /ListViewScenario.aspx | <code></code> |
| 2 | snapshot |  | **ListView [render][databind][datasource][datasourceid][templates][dynamic] first request** |
| 3 | clicktextin | divManual | <code>2</code> |
| 4 | snapshot |  | **ListView [paging] the page&#39;s list view: page 2 (its pager)** |
| 5 | clicktextin | divManual | <code>Sort by name</code> |
| 6 | snapshot |  | **ListView [sorting] sorted by name** |
| 7 | clicktextin | divManual | <code>Sort by name</code> |
| 8 | snapshot |  | **ListView [sorting][viewstate] sorted by name again: descending (the page&#39;s)** |
| 9 | clicktextin | divManual | <code>Select</code> |
| 10 | snapshot |  | **ListView [selection][datakeys] an item selected** |
| 11 | clicktextin | divManual | <code>Like</code> |
| 12 | snapshot |  | **ListView [rowcommand] a command bubbled to ItemCommand** |
| 13 | clicktextin | divManual | <code>Edit</code> |
| 14 | fill | txtLvName | <code>Renamed</code> |
| 15 | clicktextin | divManual | <code>Update</code> |
| 16 | snapshot |  | **ListView [editing][bind][datakeys] updated: keys, old and new values** |
| 17 | clicktextin | divManual | <code>Edit</code> |
| 18 | clicktextin | divManual | <code>Cancel</code> |
| 19 | snapshot |  | **ListView [editing] edit cancelled** |
| 20 | fill | txtLvNewCode | <code>N9</code> |
| 21 | fill | txtLvNewName | <code>New item</code> |
| 22 | clicktextin | divManual | <code>Insert</code> |
| 23 | snapshot |  | **ListView [inserting][bind] inserted** |
| 24 | clicktextin | divManual | <code>Delete</code> |
| 25 | snapshot |  | **ListView [editing][datakeys] deleted** |
| 26 | clicktextin | divAuto | <code>Next</code> |
| 27 | snapshot |  | **ListView [paging][datasourceid] the data source&#39;s list view: next page** |
| 28 | clicktextin | divAuto | <code>Sort by price</code> |
| 29 | snapshot |  | **ListView [sorting][datasourceid] sorted by price (the list view&#39;s own direction)** |
| 30 | clicktextin | divAuto | <code>Sort by price</code> |
| 31 | snapshot |  | **ListView [sorting][datasourceid] sorted by price again: descending** |
| 32 | clicktextin | divAuto | <code>Select</code> |
| 33 | snapshot |  | **ListView [selection][datasourceid] an item selected** |
| 34 | clicktextin | divAuto | <code>Edit</code> |
| 35 | fill | txtLaName | <code>Auto renamed</code> |
| 36 | clicktextin | divAuto | <code>Update</code> |
| 37 | snapshot |  | **ListView [editing][datasourceid] updated through the data source** |
| 38 | clicktextin | divAuto | <code>Delete</code> |
| 39 | snapshot |  | **ListView [editing][datasourceid] deleted through the data source** |
| 40 | click | btnSubmit | <code></code> |
| 41 | snapshot |  | **ListView [viewstate] posted (the one without view state is empty)** |
| 42 | click | btnShow | <code></code> |
| 43 | snapshot |  | **ListView [visible][viewstate] the hidden list view shown** |

<details><summary>ListView [render][databind][datasource][datasourceid][templates][dynamic] first request（期待される本文）</summary>

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

<details><summary>ListView [paging] the page&#39;s list view: page 2 (its pager)（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Sort by name
Desk Select Edit Delete Like
Eraser Select Edit Delete Like
File Select Edit Delete Like
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
PagePropertiesChanging: StartRowIndex=3 MaximumRows=3
List lvManual: Items=3 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvAuto: Items=3 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvGroups: Items=7 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvEmpty: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvNoViewState: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvHidden: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending (hidden)
List lvDynamic: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
Pager dpManual: StartRowIndex=3 MaximumRows=3 TotalRowCount=7
Pager dpAuto: StartRowIndex=0 MaximumRows=3 TotalRowCount=7
Pager dpDynamic: StartRowIndex=0 MaximumRows=2 TotalRowCount=5
Manual sort: ""
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>ListView [sorting] sorted by name（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Sort by name
Eraser Select Edit Delete Like
File Select Edit Delete Like
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
Request 3: IsPostBack=True
ItemCommand lvManual: Sort Name item EmptyItem
Sorting: SortExpression=Name SortDirection=Ascending -> ascending
List lvManual: Items=3 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvAuto: Items=3 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvGroups: Items=7 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvEmpty: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvNoViewState: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvHidden: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending (hidden)
List lvDynamic: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
Pager dpManual: StartRowIndex=3 MaximumRows=3 TotalRowCount=7
Pager dpAuto: StartRowIndex=0 MaximumRows=3 TotalRowCount=7
Pager dpDynamic: StartRowIndex=0 MaximumRows=2 TotalRowCount=5
Manual sort: "Name"
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>ListView [sorting][viewstate] sorted by name again: descending (the page&#39;s)（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Sort by name
Eraser Select Edit Delete Like
Desk Select Edit Delete Like
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
Request 4: IsPostBack=True
ItemCommand lvManual: Sort Name item EmptyItem
Sorting: SortExpression=Name SortDirection=Ascending -> descending
List lvManual: Items=3 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvAuto: Items=3 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvGroups: Items=7 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvEmpty: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvNoViewState: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvHidden: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending (hidden)
List lvDynamic: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
Pager dpManual: StartRowIndex=3 MaximumRows=3 TotalRowCount=7
Pager dpAuto: StartRowIndex=0 MaximumRows=3 TotalRowCount=7
Pager dpDynamic: StartRowIndex=0 MaximumRows=2 TotalRowCount=5
Manual sort: "Name DESC"
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>ListView [selection][datakeys] an item selected（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Sort by name
Eraser (selected)
Desk Select Edit Delete Like
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
Request 5: IsPostBack=True
ItemCommand lvManual: Select item DataItem
SelectedIndexChanging: NewSelectedIndex=0
SelectedIndexChanged lvManual: SelectedIndex=0 SelectedValue=E5
List lvManual: Items=3 EditIndex=-1 SelectedIndex=0 SortExpression="" SortDirection=Ascending
List lvAuto: Items=3 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvGroups: Items=7 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvEmpty: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvNoViewState: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvHidden: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending (hidden)
List lvDynamic: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
Pager dpManual: StartRowIndex=3 MaximumRows=3 TotalRowCount=7
Pager dpAuto: StartRowIndex=0 MaximumRows=3 TotalRowCount=7
Pager dpDynamic: StartRowIndex=0 MaximumRows=2 TotalRowCount=5
Manual sort: "Name DESC"
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>ListView [rowcommand] a command bubbled to ItemCommand（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Sort by name
Eraser (selected)
Desk Select Edit Delete Like
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
Request 6: IsPostBack=True
ItemCommand lvManual: like D4 item DataItem
List lvManual: Items=3 EditIndex=-1 SelectedIndex=0 SortExpression="" SortDirection=Ascending
List lvAuto: Items=3 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvGroups: Items=7 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvEmpty: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvNoViewState: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvHidden: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending (hidden)
List lvDynamic: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
Pager dpManual: StartRowIndex=3 MaximumRows=3 TotalRowCount=7
Pager dpAuto: StartRowIndex=0 MaximumRows=3 TotalRowCount=7
Pager dpDynamic: StartRowIndex=0 MaximumRows=2 TotalRowCount=5
Manual sort: "Name DESC"
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Desk:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>ListView [editing][bind][datakeys] updated: keys, old and new values（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Sort by name
File (selected)
Eraser Select Edit Delete Like
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
Request 8: IsPostBack=True
ItemCommand lvManual: Update item DataItem
ItemUpdating: ItemIndex=1 Keys={Code=D4} OldValues={} NewValues={Name=Renamed}
List lvManual: Items=3 EditIndex=-1 SelectedIndex=0 SortExpression="" SortDirection=Ascending
List lvAuto: Items=3 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvGroups: Items=7 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvEmpty: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvNoViewState: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvHidden: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending (hidden)
List lvDynamic: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
Pager dpManual: StartRowIndex=3 MaximumRows=3 TotalRowCount=7
Pager dpAuto: StartRowIndex=0 MaximumRows=3 TotalRowCount=7
Pager dpDynamic: StartRowIndex=0 MaximumRows=2 TotalRowCount=5
Manual sort: "Name DESC"
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Renamed:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>ListView [editing] edit cancelled（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Sort by name
File (selected)
Eraser Select Edit Delete Like
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
Request 10: IsPostBack=True
ItemCommand lvManual: Cancel item DataItem
ItemCanceling: ItemIndex=1 CancelMode=CancelingEdit
List lvManual: Items=3 EditIndex=-1 SelectedIndex=0 SortExpression="" SortDirection=Ascending
List lvAuto: Items=3 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvGroups: Items=7 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvEmpty: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvNoViewState: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvHidden: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending (hidden)
List lvDynamic: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
Pager dpManual: StartRowIndex=3 MaximumRows=3 TotalRowCount=7
Pager dpAuto: StartRowIndex=0 MaximumRows=3 TotalRowCount=7
Pager dpDynamic: StartRowIndex=0 MaximumRows=2 TotalRowCount=5
Manual sort: "Name DESC"
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Renamed:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False
```

</details>

<details><summary>ListView [inserting][bind] inserted（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Sort by name
Glue (selected)
File Select Edit Delete Like
Eraser Select Edit Delete Like
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
Request 11: IsPostBack=True
ItemCommand lvManual: Insert item InsertItem
ItemInserting: Values={Code=N9, Name=New item}
List lvManual: Items=3 EditIndex=-1 SelectedIndex=0 SortExpression="" SortDirection=Ascending
List lvAuto: Items=3 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvGroups: Items=7 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvEmpty: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvNoViewState: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvHidden: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending (hidden)
List lvDynamic: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
Pager dpManual: StartRowIndex=3 MaximumRows=3 TotalRowCount=8
Pager dpAuto: StartRowIndex=0 MaximumRows=3 TotalRowCount=7
Pager dpDynamic: StartRowIndex=0 MaximumRows=2 TotalRowCount=5
Manual sort: "Name DESC"
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Renamed:150:True E5:Eraser:0.5:False F6:File:3:True G7:Glue:2.75:False N9:New item:0:False
```

</details>

<details><summary>ListView [editing][datakeys] deleted（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Sort by name
Glue (selected)
Eraser Select Edit Delete Like
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
Request 12: IsPostBack=True
ItemCommand lvManual: Delete item DataItem
ItemDeleting: ItemIndex=1 Keys={Code=F6} Values={}
List lvManual: Items=3 EditIndex=-1 SelectedIndex=0 SortExpression="" SortDirection=Ascending
List lvAuto: Items=3 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvGroups: Items=7 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvEmpty: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvNoViewState: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvHidden: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending (hidden)
List lvDynamic: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
Pager dpManual: StartRowIndex=3 MaximumRows=3 TotalRowCount=7
Pager dpAuto: StartRowIndex=0 MaximumRows=3 TotalRowCount=7
Pager dpDynamic: StartRowIndex=0 MaximumRows=2 TotalRowCount=5
Manual sort: "Name DESC"
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Renamed:150:True E5:Eraser:0.5:False G7:Glue:2.75:False N9:New item:0:False
```

</details>

<details><summary>ListView [paging][datasourceid] the data source&#39;s list view: next page（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Sort by name
Glue (selected)
Eraser Select Edit Delete Like
Cup <&> Select Edit Delete Like
Insert
First 1 2 3 Last
Automatic (ObjectDataSource)
Sort by price
Renamed 150 Select Edit Delete
Eraser 0.5 Select Edit Delete
Glue 2.75 Select Edit Delete
First Previous Next Last
Groups
P1 B2 C3
D4 E5 F6
G7 - -
Empty, without view state, hidden, dynamic
Nothing to show [dynamic: Pen (P1)][dynamic: Bag (B2)]1 2 3
Request 13: IsPostBack=True
List lvManual: Items=3 EditIndex=-1 SelectedIndex=0 SortExpression="" SortDirection=Ascending
List lvAuto: Items=3 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvGroups: Items=7 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvEmpty: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvNoViewState: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvHidden: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending (hidden)
List lvDynamic: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
Pager dpManual: StartRowIndex=3 MaximumRows=3 TotalRowCount=7
Pager dpAuto: StartRowIndex=3 MaximumRows=3 TotalRowCount=7
Pager dpDynamic: StartRowIndex=0 MaximumRows=2 TotalRowCount=5
Manual sort: "Name DESC"
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Renamed:150:True E5:Eraser:0.5:False G7:Glue:2.75:False N9:New item:0:False
```

</details>

<details><summary>ListView [sorting][datasourceid] sorted by price (the list view&#39;s own direction)（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Sort by name
Glue (selected)
Eraser Select Edit Delete Like
Cup <&> Select Edit Delete Like
Insert
First 1 2 3 Last
Automatic (ObjectDataSource)
Sort by price
New item 0 Select Edit Delete
Eraser 0.5 Select Edit Delete
Pen 1.25 Select Edit Delete
First Previous Next Last
Groups
P1 B2 C3
D4 E5 F6
G7 - -
Empty, without view state, hidden, dynamic
Nothing to show [dynamic: Pen (P1)][dynamic: Bag (B2)]1 2 3
Request 14: IsPostBack=True
lvAuto Sorting: SortExpression=Price SortDirection=Ascending
List lvManual: Items=3 EditIndex=-1 SelectedIndex=0 SortExpression="" SortDirection=Ascending
List lvAuto: Items=3 EditIndex=-1 SelectedIndex=-1 SortExpression="Price" SortDirection=Ascending
List lvGroups: Items=7 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvEmpty: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvNoViewState: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvHidden: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending (hidden)
List lvDynamic: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
Pager dpManual: StartRowIndex=3 MaximumRows=3 TotalRowCount=7
Pager dpAuto: StartRowIndex=3 MaximumRows=3 TotalRowCount=7
Pager dpDynamic: StartRowIndex=0 MaximumRows=2 TotalRowCount=5
Manual sort: "Name DESC"
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Renamed:150:True E5:Eraser:0.5:False G7:Glue:2.75:False N9:New item:0:False
```

</details>

<details><summary>ListView [sorting][datasourceid] sorted by price again: descending（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Sort by name
Glue (selected)
Eraser Select Edit Delete Like
Cup <&> Select Edit Delete Like
Insert
First 1 2 3 Last
Automatic (ObjectDataSource)
Sort by price
Cup <&> 1234.5 Select Edit Delete
Renamed 150 Select Edit Delete
Bag 24.99 Select Edit Delete
First Previous Next Last
Groups
P1 B2 C3
D4 E5 F6
G7 - -
Empty, without view state, hidden, dynamic
Nothing to show [dynamic: Pen (P1)][dynamic: Bag (B2)]1 2 3
Request 15: IsPostBack=True
lvAuto Sorting: SortExpression=Price SortDirection=Descending
List lvManual: Items=3 EditIndex=-1 SelectedIndex=0 SortExpression="" SortDirection=Ascending
List lvAuto: Items=3 EditIndex=-1 SelectedIndex=-1 SortExpression="Price" SortDirection=Descending
List lvGroups: Items=7 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvEmpty: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvNoViewState: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvHidden: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending (hidden)
List lvDynamic: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
Pager dpManual: StartRowIndex=3 MaximumRows=3 TotalRowCount=7
Pager dpAuto: StartRowIndex=0 MaximumRows=3 TotalRowCount=7
Pager dpDynamic: StartRowIndex=0 MaximumRows=2 TotalRowCount=5
Manual sort: "Name DESC"
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Renamed:150:True E5:Eraser:0.5:False G7:Glue:2.75:False N9:New item:0:False
```

</details>

<details><summary>ListView [selection][datasourceid] an item selected（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Sort by name
Glue (selected)
Eraser Select Edit Delete Like
Cup <&> Select Edit Delete Like
Insert
First 1 2 3 Last
Automatic (ObjectDataSource)
Sort by price
Cup <&> 1234.5 Select Edit Delete
Renamed 150 Select Edit Delete
Bag 24.99 Select Edit Delete
First Previous Next Last
Groups
P1 B2 C3
D4 E5 F6
G7 - -
Empty, without view state, hidden, dynamic
Nothing to show [dynamic: Pen (P1)][dynamic: Bag (B2)]1 2 3
Request 16: IsPostBack=True
List lvManual: Items=3 EditIndex=-1 SelectedIndex=0 SortExpression="" SortDirection=Ascending
List lvAuto: Items=3 EditIndex=-1 SelectedIndex=0 SortExpression="Price" SortDirection=Descending
List lvGroups: Items=7 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvEmpty: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvNoViewState: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvHidden: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending (hidden)
List lvDynamic: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
Pager dpManual: StartRowIndex=3 MaximumRows=3 TotalRowCount=7
Pager dpAuto: StartRowIndex=0 MaximumRows=3 TotalRowCount=7
Pager dpDynamic: StartRowIndex=0 MaximumRows=2 TotalRowCount=5
Manual sort: "Name DESC"
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Cup <&>:1234.5:True D4:Renamed:150:True E5:Eraser:0.5:False G7:Glue:2.75:False N9:New item:0:False
```

</details>

<details><summary>ListView [editing][datasourceid] updated through the data source（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Sort by name
Glue (selected)
Eraser Select Edit Delete Like
Cup <&> Select Edit Delete Like
Insert
First 1 2 3 Last
Automatic (ObjectDataSource)
Sort by price
Auto renamed 1234.5 Select Edit Delete
Renamed 150 Select Edit Delete
Bag 24.99 Select Edit Delete
First Previous Next Last
Groups
P1 B2 C3
D4 E5 F6
G7 - -
Empty, without view state, hidden, dynamic
Nothing to show [dynamic: Pen (P1)][dynamic: Bag (B2)]1 2 3
Request 18: IsPostBack=True
ObjectDataSource Updating: {Code=C3, Name=Auto renamed, Price=1234.5}
ItemUpdated: AffectedRows=-1 NewValues={Name=Auto renamed, Price=1234.5}
List lvManual: Items=3 EditIndex=-1 SelectedIndex=0 SortExpression="" SortDirection=Ascending
List lvAuto: Items=3 EditIndex=-1 SelectedIndex=0 SortExpression="Price" SortDirection=Descending
List lvGroups: Items=7 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvEmpty: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvNoViewState: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvHidden: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending (hidden)
List lvDynamic: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
Pager dpManual: StartRowIndex=3 MaximumRows=3 TotalRowCount=7
Pager dpAuto: StartRowIndex=0 MaximumRows=3 TotalRowCount=7
Pager dpDynamic: StartRowIndex=0 MaximumRows=2 TotalRowCount=5
Manual sort: "Name DESC"
Store: P1:Pen:1.25:True B2:Bag:24.99:False C3:Auto renamed:1234.5:True D4:Renamed:150:True E5:Eraser:0.5:False G7:Glue:2.75:False N9:New item:0:False
```

</details>

<details><summary>ListView [editing][datasourceid] deleted through the data source（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Sort by name
Glue (selected)
Eraser Select Edit Delete Like
Cup <&> Select Edit Delete Like
Insert
First 1 2 3 Last
Automatic (ObjectDataSource)
Sort by price
Renamed 150 Select Edit Delete
Bag 24.99 Select Edit Delete
Glue 2.75 Select Edit Delete
First Previous Next Last
Groups
P1 B2 C3
D4 E5 F6
G7 - -
Empty, without view state, hidden, dynamic
Nothing to show [dynamic: Pen (P1)][dynamic: Bag (B2)]1 2 3
Request 19: IsPostBack=True
ItemDeleted: AffectedRows=-1 Keys={Code=C3}
List lvManual: Items=3 EditIndex=-1 SelectedIndex=0 SortExpression="" SortDirection=Ascending
List lvAuto: Items=3 EditIndex=-1 SelectedIndex=0 SortExpression="Price" SortDirection=Descending
List lvGroups: Items=7 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvEmpty: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvNoViewState: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvHidden: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending (hidden)
List lvDynamic: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
Pager dpManual: StartRowIndex=3 MaximumRows=3 TotalRowCount=7
Pager dpAuto: StartRowIndex=0 MaximumRows=3 TotalRowCount=7
Pager dpDynamic: StartRowIndex=0 MaximumRows=2 TotalRowCount=5
Manual sort: "Name DESC"
Store: P1:Pen:1.25:True B2:Bag:24.99:False D4:Renamed:150:True E5:Eraser:0.5:False G7:Glue:2.75:False N9:New item:0:False
```

</details>

<details><summary>ListView [viewstate] posted (the one without view state is empty)（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Sort by name
Glue (selected)
Eraser Select Edit Delete Like
Cup <&> Select Edit Delete Like
Insert
First 1 2 3 Last
Automatic (ObjectDataSource)
Sort by price
Renamed 150 Select Edit Delete
Bag 24.99 Select Edit Delete
Glue 2.75 Select Edit Delete
First Previous Next Last
Groups
P1 B2 C3
D4 E5 F6
G7 - -
Empty, without view state, hidden, dynamic
Nothing to show [dynamic: Pen (P1)][dynamic: Bag (B2)]1 2 3
Request 20: IsPostBack=True
List lvManual: Items=3 EditIndex=-1 SelectedIndex=0 SortExpression="" SortDirection=Ascending
List lvAuto: Items=3 EditIndex=-1 SelectedIndex=0 SortExpression="Price" SortDirection=Descending
List lvGroups: Items=7 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvEmpty: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvNoViewState: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvHidden: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending (hidden)
List lvDynamic: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
Pager dpManual: StartRowIndex=3 MaximumRows=3 TotalRowCount=7
Pager dpAuto: StartRowIndex=0 MaximumRows=3 TotalRowCount=6
Pager dpDynamic: StartRowIndex=0 MaximumRows=2 TotalRowCount=5
Manual sort: "Name DESC"
Store: P1:Pen:1.25:True B2:Bag:24.99:False D4:Renamed:150:True E5:Eraser:0.5:False G7:Glue:2.75:False N9:New item:0:False
```

</details>

<details><summary>ListView [visible][viewstate] the hidden list view shown（期待される本文）</summary>

```text
Manual (DataSource, handlers)
Sort by name
Glue (selected)
Eraser Select Edit Delete Like
Cup <&> Select Edit Delete Like
Insert
First 1 2 3 Last
Automatic (ObjectDataSource)
Sort by price
Renamed 150 Select Edit Delete
Bag 24.99 Select Edit Delete
Glue 2.75 Select Edit Delete
First Previous Next Last
Groups
P1 B2 C3
D4 E5 F6
G7 - -
Empty, without view state, hidden, dynamic
Nothing to show Hidden: PenHidden: Bag [dynamic: Pen (P1)][dynamic: Bag (B2)]1 2 3
Request 21: IsPostBack=True
Click btnShow
List lvManual: Items=3 EditIndex=-1 SelectedIndex=0 SortExpression="" SortDirection=Ascending
List lvAuto: Items=3 EditIndex=-1 SelectedIndex=0 SortExpression="Price" SortDirection=Descending
List lvGroups: Items=7 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvEmpty: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvNoViewState: Items=0 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvHidden: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
List lvDynamic: Items=2 EditIndex=-1 SelectedIndex=-1 SortExpression="" SortDirection=Ascending
Pager dpManual: StartRowIndex=3 MaximumRows=3 TotalRowCount=7
Pager dpAuto: StartRowIndex=0 MaximumRows=3 TotalRowCount=6
Pager dpDynamic: StartRowIndex=0 MaximumRows=2 TotalRowCount=5
Manual sort: "Name DESC"
Store: P1:Pen:1.25:True B2:Bag:24.99:False D4:Renamed:150:True E5:Eraser:0.5:False G7:Glue:2.75:False N9:New item:0:False
```

</details>

