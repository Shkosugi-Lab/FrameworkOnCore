# TextBox テストケース一覧

変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。
このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。

- 正解データの採取: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0
- ① API 層: 170 ケース（179 メンバー）
- ② HTML 層: 214 ケース
- ③ シナリオ層: 13 スナップショット

## 機構マトリクス

型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。

| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |
|---|---|---|---|
| プロパティ → HTML 描画 | すべてのコントロール | 201 | 1 |
| ViewState の保存と復元 | Control | 6 | 2 |
| Visible=false（描画されず状態は保持） | Control.Visible | 3 | 1 |
| 名前付けコンテナと ClientIDMode | Control.ClientIDMode | 7 | 0 |
| テーマとスキン（SkinID / EnableTheming） | テーマを付けられる型（[Themeable]） | 5 | 0 |
| 動的に生成したコントロールのライフサイクル | Control | 0 | 1 |
| データバインド（DataBind / <%# %>） | Control.DataBind | 0 | 1 |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | WebControl.Enabled、HtmlControl.Disabled | 4 | 1 |
| ポストバックデータの取り込み | IPostBackDataHandler | 0 | 1 |
| 変更イベント（*Changed、ServerChange） | IPostBackDataHandler と *Changed・ServerChange イベント | 0 | 3 |
| ReadOnly（ポストされた値を無視） | ReadOnly プロパティ | 3 | 1 |
| リクエスト検証（ValidateRequestMode） | IPostBackDataHandler（ポスト値を読む） | 3 | 2 |
| AutoPostBack（クライアントスクリプトによるポストバック） | AutoPostBack プロパティ | 7 | 3 |
| CausesValidation / ValidationGroup | CausesValidation プロパティ | 8 | 2 |
| 検証対象（サーバー側の検証） | [ValidationProperty] と IPostBackDataHandler | 0 | 3 |
| 検証対象（ブラウザ側の検証） | [ValidationProperty] と IPostBackDataHandler | 0 | 1 |
| 双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値） | [ControlValueProperty] と IPostBackDataHandler | 0 | 1 |

該当しない機構: ポストバックイベント、コマンドイベントと親へのバブル（CommandName）、送信ボタン／スクリプトによるポストバック（UseSubmitBehavior）、ブラウザ側のクリック処理（OnClientClick）、クロスページポストバック（PostBackUrl）、既定のボタン（Panel の DefaultButton）、項目（ListItem の Selected・Enabled・Text・Value と ViewState）、複数選択、繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）、ポスト値のイベント検証（一覧にない値・無効な項目の拒否）、サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）、ブラウザ側の検証スクリプト（EnableClientScript）、検証結果のまとめ（ValidationSummary）、ページング、並べ替え、項目の挿入、行・項目の編集（編集・更新・取り消し・削除）、行・項目の選択、行・項目のコマンド（RowCommand・ItemCommand へのバブル）、データキー（DataKeyNames・DataKeys）、データソースコントロールとの連携（DataSourceID）、テンプレート、データソース連携（DataSource・DataTextField など）、クライアントコールバック

## ① API 層

TextBox とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の TextBox インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。

| ケース | 期待値（.NET Framework） |
|---|---|
| <code>Api.Control.#ctor</code> | <code>Control.#ctor = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMode=In…</code> |
| <code>Api.Control.Adapter</code> | <code>Control.Adapter = null</code> |
| <code>Api.Control.AddParsedSubObject</code> | <code>Control.AddParsedSubObject(Object) = &quot;-&gt; System.Web.HttpException: &#39;TextBox&#39; cannot have children of type &#39;String&#39;.&quot;</code> |
| <code>Api.Control.AddedControl</code> | <code>Control.AddedControl(Control,Int32) = &quot;ok&quot;</code><br><code>Control.AddedControl(Control,Int32) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; B…</code> |
| <code>Api.Control.AppRelativeTemplateSourceDirectory</code> | <code>Control.AppRelativeTemplateSourceDirectory = &quot;&quot;</code><br><code>Control.AppRelativeTemplateSourceDirectory set = &quot;-&gt; System.ArgumentException: The relative virtual path &#39;Abc 123&#39; is not allowed here.&quot;</code> |
| <code>Api.Control.ApplyStyleSheetSkin</code> | <code>Control.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>Control.ApplyStyleSheetSkin(Page) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; Bor…</code> |
| <code>Api.Control.BeginRenderTracing</code> | <code>Control.BeginRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.BeginRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.BeginRenderTracing(TextWriter,Object) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color …</code> |
| <code>Api.Control.BindingContainer</code> | <code>Control.BindingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Clien…</code> |
| <code>Api.Control.BuildProfileTree</code> | <code>Control.BuildProfileTree(String,Boolean) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.ChildControlsCreated</code> | <code>Control.ChildControlsCreated = &quot;false&quot;</code><br><code>Control.ChildControlsCreated set = &quot;ok&quot;</code> |
| <code>Api.Control.ClearCachedClientID</code> | <code>Control.ClearCachedClientID = &quot;ok&quot;</code><br><code>Control.ClearCachedClientID after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderCol…</code> |
| <code>Api.Control.ClearChildControlState</code> | <code>Control.ClearChildControlState = &quot;ok&quot;</code><br><code>Control.ClearChildControlState after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; Border…</code> |
| <code>Api.Control.ClearChildState</code> | <code>Control.ClearChildState = &quot;ok&quot;</code><br><code>Control.ClearChildState after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderColor=C…</code> |
| <code>Api.Control.ClearChildViewState</code> | <code>Control.ClearChildViewState = &quot;ok&quot;</code><br><code>Control.ClearChildViewState after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderCol…</code> |
| <code>Api.Control.ClearEffectiveClientIDMode</code> | <code>Control.ClearEffectiveClientIDMode = &quot;ok&quot;</code><br><code>Control.ClearEffectiveClientIDMode after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; Bo…</code> |
| <code>Api.Control.ClientID</code> | <code>Control.ClientID = &quot;txtName&quot;</code> |
| <code>Api.Control.ClientIDMode</code> | <code>Control.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>Control.ClientIDMode set = &quot;ClientIDMode.AutoID&quot;</code> |
| <code>Api.Control.ClientIDSeparator</code> | <code>Control.ClientIDSeparator = &quot;&#39;_&#39;&quot;</code> |
| <code>Api.Control.Context</code> | <code>Control.Context = null</code> |
| <code>Api.Control.Controls</code> | <code>Control.Controls = ControlCollection []</code> |
| <code>Api.Control.CreateChildControls</code> | <code>Control.CreateChildControls = &quot;ok&quot;</code><br><code>Control.CreateChildControls after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderCol…</code> |
| <code>Api.Control.CreateControlCollection</code> | <code>Control.CreateControlCollection = ControlCollection []</code><br><code>Control.CreateControlCollection after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; Borde…</code> |
| <code>Api.Control.DataBind</code> | <code>Control.DataBind = &quot;ok&quot;</code><br><code>Control.DataBind after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #0…</code><br><code>Control.DataBind(Boolean) = &quot;ok&quot;</code><br><code>Control.DataBind(Boolean) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderColor…</code> |
| <code>Api.Control.DataBindChildren</code> | <code>Control.DataBindChildren = &quot;ok&quot;</code><br><code>Control.DataBindChildren after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderColor=…</code> |
| <code>Api.Control.DataBinding</code> | <code>Control.DataBinding = &quot;added and removed&quot;</code> |
| <code>Api.Control.DataItemContainer</code> | <code>Control.DataItemContainer = null</code> |
| <code>Api.Control.DataKeysContainer</code> | <code>Control.DataKeysContainer = null</code> |
| <code>Api.Control.DesignMode</code> | <code>Control.DesignMode = &quot;false&quot;</code> |
| <code>Api.Control.Dispose</code> | <code>Control.Dispose = &quot;ok&quot;</code><br><code>Control.Dispose after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00…</code> |
| <code>Api.Control.Disposed</code> | <code>Control.Disposed = &quot;added and removed&quot;</code> |
| <code>Api.Control.EnableTheming</code> | <code>Control.EnableTheming = &quot;true&quot;</code><br><code>Control.EnableTheming set = &quot;true&quot;</code> |
| <code>Api.Control.EnableViewState</code> | <code>Control.EnableViewState = &quot;true&quot;</code><br><code>Control.EnableViewState set = &quot;true&quot;</code> |
| <code>Api.Control.EndRenderTracing</code> | <code>Control.EndRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.EndRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.EndRenderTracing(TextWriter,Object) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [E…</code> |
| <code>Api.Control.EnsureChildControls</code> | <code>Control.EnsureChildControls = &quot;ok&quot;</code><br><code>Control.EnsureChildControls after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderCol…</code> |
| <code>Api.Control.EnsureID</code> | <code>Control.EnsureID = &quot;ok&quot;</code><br><code>Control.EnsureID after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #0…</code> |
| <code>Api.Control.Events</code> | <code>Control.Events = &quot;System.ComponentModel.EventHandlerList&quot;</code> |
| <code>Api.Control.FindControl</code> | <code>Control.FindControl(String) = Label#lblTitle {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code><br><code>Control.FindControl(String) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderCol…</code><br><code>Control.FindControl(String,Int32) = null</code><br><code>Control.FindControl(String,Int32) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; Bor…</code> |
| <code>Api.Control.Focus</code> | <code>Control.Focus = &quot;-&gt; System.InvalidOperationException: A form tag with runat=server must exist on the Page to use SetFocus() or the Focus property.&quot;</code> |
| <code>Api.Control.GetDesignModeState</code> | <code>Control.GetDesignModeState = &quot;[]&quot;</code><br><code>Control.GetDesignModeState after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderColo…</code> |
| <code>Api.Control.GetRouteUrl</code> | <code>Control.GetRouteUrl(Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.GetUniqueIDRelativeTo</code> | <code>Control.GetUniqueIDRelativeTo(Control) = &quot;txtName&quot;</code><br><code>Control.GetUniqueIDRelativeTo(Control) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]…</code> |
| <code>Api.Control.HasChildViewState</code> | <code>Control.HasChildViewState = &quot;false&quot;</code> |
| <code>Api.Control.HasControls</code> | <code>Control.HasControls = &quot;false&quot;</code><br><code>Control.HasControls after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderColor=Color…</code> |
| <code>Api.Control.HasEvents</code> | <code>Control.HasEvents = &quot;false&quot;</code><br><code>Control.HasEvents after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #…</code> |
| <code>Api.Control.ID</code> | <code>Control.ID = &quot;txtName&quot;</code><br><code>Control.ID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.IdSeparator</code> | <code>Control.IdSeparator = &quot;&#39;$&#39;&quot;</code> |
| <code>Api.Control.Init</code> | <code>Control.Init = &quot;added and removed&quot;</code> |
| <code>Api.Control.IsChildControlStateCleared</code> | <code>Control.IsChildControlStateCleared = &quot;false&quot;</code> |
| <code>Api.Control.IsLiteralContent</code> | <code>Control.IsLiteralContent = &quot;false&quot;</code><br><code>Control.IsLiteralContent after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderColor=…</code> |
| <code>Api.Control.IsTrackingViewState</code> | <code>Control.IsTrackingViewState = &quot;false&quot;</code> |
| <code>Api.Control.IsViewStateEnabled</code> | <code>Control.IsViewStateEnabled = &quot;true&quot;</code> |
| <code>Api.Control.Load</code> | <code>Control.Load = &quot;added and removed&quot;</code> |
| <code>Api.Control.LoadControlState</code> | <code>Control.LoadControlState(Object) = &quot;ok&quot;</code><br><code>Control.LoadControlState(Object) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; Bord…</code> |
| <code>Api.Control.LoadViewState</code> | <code>Control.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Web.UI.Pair&#39;.&quot;</code> |
| <code>Api.Control.LoadViewStateByID</code> | <code>Control.LoadViewStateByID = &quot;false&quot;</code> |
| <code>Api.Control.MapPathSecure</code> | <code>Control.MapPathSecure(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.NamingContainer</code> | <code>Control.NamingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code> |
| <code>Api.Control.OnBubbleEvent</code> | <code>Control.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Control.OnBubbleEvent(Object,EventArgs) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty…</code> |
| <code>Api.Control.OnDataBinding</code> | <code>Control.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnDataBinding(EventArgs) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; Bord…</code> |
| <code>Api.Control.OnInit</code> | <code>Control.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnInit(EventArgs) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderColor…</code> |
| <code>Api.Control.OnLoad</code> | <code>Control.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnLoad(EventArgs) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderColor…</code> |
| <code>Api.Control.OnPreRender</code> | <code>Control.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnPreRender(EventArgs) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; Border…</code> |
| <code>Api.Control.OnUnload</code> | <code>Control.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnUnload(EventArgs) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderCol…</code> |
| <code>Api.Control.OpenFile</code> | <code>Control.OpenFile(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.Page</code> | <code>Control.Page = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=&quot;&quot;; …</code><br><code>Control.Page set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=…</code> |
| <code>Api.Control.Parent</code> | <code>Control.Parent = HtmlForm#form1 {Action=&quot;&quot;; ClientID=&quot;form1&quot;; ClientIDMode=Inherit; DefaultButton=&quot;&quot;; DefaultFocus=&quot;&quot;; Disabled=false; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.PreRender</code> | <code>Control.PreRender = &quot;added and removed&quot;</code> |
| <code>Api.Control.RaiseBubbleEvent</code> | <code>Control.RaiseBubbleEvent(Object,EventArgs) = &quot;ok&quot;</code><br><code>Control.RaiseBubbleEvent(Object,EventArgs) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Em…</code> |
| <code>Api.Control.RemovedControl</code> | <code>Control.RemovedControl(Control) = &quot;ok&quot;</code><br><code>Control.RemovedControl(Control) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; Borde…</code> |
| <code>Api.Control.Render</code> | <code>Control.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.Render(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;input name=&quot;txtName&quot; type=&quot;text&quot; value=&quot;Taro&quot; id=&quot;txtName&quot; /&gt;</code><br><code>Control.Render(HtmlTextWriter) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; Border…</code> |
| <code>Api.Control.RenderChildren</code> | <code>Control.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderChildren(HtmlTextWriter) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]…</code> |
| <code>Api.Control.RenderControl</code> | <code>Control.RenderControl(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;input name=&quot;txtName&quot; type=&quot;text&quot; value=&quot;Taro&quot; id=&quot;txtName&quot; /&gt;</code><br><code>Control.RenderControl(HtmlTextWriter) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty];…</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) writer:</code><br><code>  &#124; &lt;input name=&quot;txtName&quot; type=&quot;text&quot; value=&quot;Taro&quot; id=&quot;txtName&quot; /&gt;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color …</code> |
| <code>Api.Control.RenderingCompatibility</code> | <code>Control.RenderingCompatibility = &quot;System.Version 4.0&quot;</code><br><code>Control.RenderingCompatibility set = &quot;System.Version 4.0&quot;</code> |
| <code>Api.Control.ResolveAdapter</code> | <code>Control.ResolveAdapter = null</code><br><code>Control.ResolveAdapter after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderColor=Co…</code> |
| <code>Api.Control.ResolveClientUrl</code> | <code>Control.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveClientUrl(String) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; Bord…</code> |
| <code>Api.Control.ResolveUrl</code> | <code>Control.ResolveUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveUrl(String) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderColo…</code> |
| <code>Api.Control.SaveControlState</code> | <code>Control.SaveControlState = null</code><br><code>Control.SaveControlState after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderColor=…</code> |
| <code>Api.Control.SaveViewState</code> | <code>Control.SaveViewState = null</code><br><code>Control.SaveViewState after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderColor=Col…</code> |
| <code>Api.Control.SetDesignModeState</code> | <code>Control.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>Control.SetDesignModeState(IDictionary) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty…</code> |
| <code>Api.Control.SetRenderMethodDelegate</code> | <code>Control.SetRenderMethodDelegate(RenderMethod) = &quot;ok&quot;</code><br><code>Control.SetRenderMethodDelegate(RenderMethod) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color …</code> |
| <code>Api.Control.SetTraceData</code> | <code>Control.SetTraceData(Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; B…</code><br><code>Control.SetTraceData(Object,Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object,Object) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Em…</code> |
| <code>Api.Control.Site</code> | <code>Control.Site = null</code><br><code>Control.Site set = null</code> |
| <code>Api.Control.SkinID</code> | <code>Control.SkinID = &quot;&quot;</code><br><code>Control.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.TemplateControl</code> | <code>Control.TemplateControl = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code><br><code>Control.TemplateControl set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Cl…</code> |
| <code>Api.Control.TemplateSourceDirectory</code> | <code>Control.TemplateSourceDirectory = &quot;&quot;</code> |
| <code>Api.Control.TrackViewState</code> | <code>Control.TrackViewState = &quot;ok&quot;</code><br><code>Control.TrackViewState after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderColor=Co…</code> |
| <code>Api.Control.UniqueID</code> | <code>Control.UniqueID = &quot;txtName&quot;</code> |
| <code>Api.Control.Unload</code> | <code>Control.Unload = &quot;added and removed&quot;</code> |
| <code>Api.Control.ValidateRequestMode</code> | <code>Control.ValidateRequestMode = &quot;ValidateRequestMode.Inherit&quot;</code><br><code>Control.ValidateRequestMode set = &quot;ValidateRequestMode.Disabled&quot;</code> |
| <code>Api.Control.ViewState</code> | <code>Control.ViewState = StateBag [Text=&quot;Taro&quot;]</code> |
| <code>Api.Control.ViewStateIgnoresCase</code> | <code>Control.ViewStateIgnoresCase = &quot;false&quot;</code> |
| <code>Api.Control.ViewStateMode</code> | <code>Control.ViewStateMode = &quot;ViewStateMode.Inherit&quot;</code><br><code>Control.ViewStateMode set = &quot;ViewStateMode.Enabled&quot;</code> |
| <code>Api.Control.Visible</code> | <code>Control.Visible = &quot;true&quot;</code><br><code>Control.Visible set = &quot;true&quot;</code> |
| <code>Api.IAttributeAccessor.GetAttribute</code> | <code>IAttributeAccessor.GetAttribute(String) = null</code><br><code>IAttributeAccessor.GetAttribute(String) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty…</code> |
| <code>Api.IAttributeAccessor.SetAttribute</code> | <code>IAttributeAccessor.SetAttribute(String,String) = &quot;ok&quot;</code><br><code>IAttributeAccessor.SetAttribute(String,String) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color…</code> |
| <code>Api.IControlBuilderAccessor.ControlBuilder</code> | <code>IControlBuilderAccessor.ControlBuilder = null</code> |
| <code>Api.IControlDesignerAccessor.GetDesignModeState</code> | <code>IControlDesignerAccessor.GetDesignModeState = &quot;[]&quot;</code><br><code>IControlDesignerAccessor.GetDesignModeState after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [E…</code> |
| <code>Api.IControlDesignerAccessor.SetDesignModeState</code> | <code>IControlDesignerAccessor.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetDesignModeState(IDictionary) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #0000…</code> |
| <code>Api.IControlDesignerAccessor.SetOwnerControl</code> | <code>IControlDesignerAccessor.SetOwnerControl(Control) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetOwnerControl(Control) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Co…</code> |
| <code>Api.IControlDesignerAccessor.UserData</code> | <code>IControlDesignerAccessor.UserData = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.DataBindings</code> | <code>IDataBindingsAccessor.DataBindings = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.HasDataBindings</code> | <code>IDataBindingsAccessor.HasDataBindings = &quot;false&quot;</code> |
| <code>Api.IEditableTextControl.TextChanged</code> | <code>IEditableTextControl.TextChanged = &quot;added and removed&quot;</code> |
| <code>Api.IExpressionsAccessor.Expressions</code> | <code>IExpressionsAccessor.Expressions = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.HasExpressions</code> | <code>IExpressionsAccessor.HasExpressions = &quot;false&quot;</code> |
| <code>Api.IParserAccessor.AddParsedSubObject</code> | <code>IParserAccessor.AddParsedSubObject(Object) = &quot;-&gt; System.Web.HttpException: &#39;TextBox&#39; cannot have children of type &#39;String&#39;.&quot;</code> |
| <code>Api.IPostBackDataHandler.LoadPostData</code> | <code>IPostBackDataHandler.LoadPostData(String,NameValueCollection) = &quot;true&quot;</code><br><code>IPostBackDataHandler.LoadPostData(String,NameValueCollection) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color …</code> |
| <code>Api.IPostBackDataHandler.RaisePostDataChangedEvent</code> | <code>IPostBackDataHandler.RaisePostDataChangedEvent = &quot;ok&quot;</code><br><code>IPostBackDataHandler.RaisePostDataChangedEvent after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color…</code> |
| <code>Api.ITextControl.Text</code> | <code>ITextControl.Text = &quot;Taro&quot;</code><br><code>ITextControl.Text set = &quot;Abc 123&quot;</code> |
| <code>Api.IUrlResolutionService.ResolveClientUrl</code> | <code>IUrlResolutionService.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>IUrlResolutionService.ResolveClientUrl(String) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color…</code> |
| <code>Api.WebControls.TextBox.#ctor</code> | <code>TextBox.#ctor = TextBox# {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [E…</code> |
| <code>Api.WebControls.TextBox.AddAttributesToRender</code> | <code>TextBox.AddAttributesToRender(HtmlTextWriter) = &quot;ok&quot;</code><br><code>TextBox.AddAttributesToRender(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>TextBox.AddAttributesToRender(HtmlTextWriter) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color …</code> |
| <code>Api.WebControls.TextBox.AddParsedSubObject</code> | <code>TextBox.AddParsedSubObject(Object) = &quot;-&gt; System.Web.HttpException: &#39;TextBox&#39; cannot have children of type &#39;String&#39;.&quot;</code> |
| <code>Api.WebControls.TextBox.AutoCompleteType</code> | <code>TextBox.AutoCompleteType = &quot;AutoCompleteType.None&quot;</code><br><code>TextBox.AutoCompleteType set = &quot;AutoCompleteType.Disabled&quot;</code> |
| <code>Api.WebControls.TextBox.AutoPostBack</code> | <code>TextBox.AutoPostBack = &quot;false&quot;</code><br><code>TextBox.AutoPostBack set = &quot;true&quot;</code> |
| <code>Api.WebControls.TextBox.CausesValidation</code> | <code>TextBox.CausesValidation = &quot;false&quot;</code><br><code>TextBox.CausesValidation set = &quot;true&quot;</code> |
| <code>Api.WebControls.TextBox.Columns</code> | <code>TextBox.Columns = &quot;Int32 0&quot;</code><br><code>TextBox.Columns set = &quot;Int32 1&quot;</code> |
| <code>Api.WebControls.TextBox.LoadPostData</code> | <code>TextBox.LoadPostData(String,NameValueCollection) = &quot;true&quot;</code><br><code>TextBox.LoadPostData(String,NameValueCollection) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Col…</code> |
| <code>Api.WebControls.TextBox.MaxLength</code> | <code>TextBox.MaxLength = &quot;Int32 0&quot;</code><br><code>TextBox.MaxLength set = &quot;Int32 1&quot;</code> |
| <code>Api.WebControls.TextBox.OnPreRender</code> | <code>TextBox.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>TextBox.OnPreRender(EventArgs) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; Border…</code> |
| <code>Api.WebControls.TextBox.OnTextChanged</code> | <code>TextBox.OnTextChanged(EventArgs) = &quot;ok&quot;</code><br><code>TextBox.OnTextChanged(EventArgs) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; Bord…</code> |
| <code>Api.WebControls.TextBox.RaisePostDataChangedEvent</code> | <code>TextBox.RaisePostDataChangedEvent = &quot;ok&quot;</code><br><code>TextBox.RaisePostDataChangedEvent after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; Bor…</code> |
| <code>Api.WebControls.TextBox.ReadOnly</code> | <code>TextBox.ReadOnly = &quot;false&quot;</code><br><code>TextBox.ReadOnly set = &quot;true&quot;</code> |
| <code>Api.WebControls.TextBox.Render</code> | <code>TextBox.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>TextBox.Render(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;input name=&quot;txtName&quot; type=&quot;text&quot; value=&quot;Taro&quot; id=&quot;txtName&quot; /&gt;</code><br><code>TextBox.Render(HtmlTextWriter) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; Border…</code> |
| <code>Api.WebControls.TextBox.Rows</code> | <code>TextBox.Rows = &quot;Int32 0&quot;</code><br><code>TextBox.Rows set = &quot;Int32 1&quot;</code> |
| <code>Api.WebControls.TextBox.SaveViewState</code> | <code>TextBox.SaveViewState = null</code><br><code>TextBox.SaveViewState after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderColor=Col…</code> |
| <code>Api.WebControls.TextBox.TagKey</code> | <code>TextBox.TagKey = &quot;HtmlTextWriterTag.Input&quot;</code> |
| <code>Api.WebControls.TextBox.Text</code> | <code>TextBox.Text = &quot;Taro&quot;</code><br><code>TextBox.Text set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.TextBox.TextChanged</code> | <code>TextBox.TextChanged = &quot;added and removed&quot;</code> |
| <code>Api.WebControls.TextBox.TextMode</code> | <code>TextBox.TextMode = &quot;TextBoxMode.SingleLine&quot;</code><br><code>TextBox.TextMode set = &quot;TextBoxMode.MultiLine&quot;</code> |
| <code>Api.WebControls.TextBox.ValidationGroup</code> | <code>TextBox.ValidationGroup = &quot;&quot;</code><br><code>TextBox.ValidationGroup set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.TextBox.Wrap</code> | <code>TextBox.Wrap = &quot;true&quot;</code><br><code>TextBox.Wrap set = &quot;true&quot;</code> |
| <code>Api.WebControls.WebControl.#ctor</code> | <code>WebControl.#ctor = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidt…</code><br><code>WebControl.#ctor(String) = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; Bo…</code><br><code>WebControl.#ctor(HtmlTextWriterTag) = WebControl# {AccessKey=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle…</code> |
| <code>Api.WebControls.WebControl.AccessKey</code> | <code>WebControl.AccessKey = &quot;&quot;</code><br><code>WebControl.AccessKey set = &quot;-&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]&quot;</code> |
| <code>Api.WebControls.WebControl.AddAttributesToRender</code> | <code>WebControl.AddAttributesToRender(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.AddAttributesToRender(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>WebControl.AddAttributesToRender(HtmlTextWriter) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Col…</code> |
| <code>Api.WebControls.WebControl.ApplyStyle</code> | <code>WebControl.ApplyStyle(Style) = &quot;ok&quot;</code><br><code>WebControl.ApplyStyle(Style) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderCo…</code> |
| <code>Api.WebControls.WebControl.Attributes</code> | <code>WebControl.Attributes = AttributeCollection []</code> |
| <code>Api.WebControls.WebControl.BackColor</code> | <code>WebControl.BackColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>WebControl.BackColor set = &quot;Color #FF3366CC Color [A=255, R=51, G=102, B=204]&quot;</code> |
| <code>Api.WebControls.WebControl.BorderColor</code> | <code>WebControl.BorderColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>WebControl.BorderColor set = &quot;Color #FF3366CC Color [A=255, R=51, G=102, B=204]&quot;</code> |
| <code>Api.WebControls.WebControl.BorderStyle</code> | <code>WebControl.BorderStyle = &quot;BorderStyle.NotSet&quot;</code><br><code>WebControl.BorderStyle set = &quot;BorderStyle.None&quot;</code> |
| <code>Api.WebControls.WebControl.BorderWidth</code> | <code>WebControl.BorderWidth = Unit </code><br><code>WebControl.BorderWidth set = Unit 120px</code> |
| <code>Api.WebControls.WebControl.ControlStyle</code> | <code>WebControl.ControlStyle = Style {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; CssClass=…</code> |
| <code>Api.WebControls.WebControl.ControlStyleCreated</code> | <code>WebControl.ControlStyleCreated = &quot;false&quot;</code> |
| <code>Api.WebControls.WebControl.CopyBaseAttributes</code> | <code>WebControl.CopyBaseAttributes(WebControl) = &quot;ok&quot;</code><br><code>WebControl.CopyBaseAttributes(WebControl) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Emp…</code> |
| <code>Api.WebControls.WebControl.CreateControlStyle</code> | <code>WebControl.CreateControlStyle = Style {BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color [Empty]; BorderStyle=NotSet; BorderWidth=; Css…</code><br><code>WebControl.CreateControlStyle after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderC…</code> |
| <code>Api.WebControls.WebControl.CssClass</code> | <code>WebControl.CssClass = &quot;&quot;</code><br><code>WebControl.CssClass set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.DisabledCssClass</code> | <code>WebControl.DisabledCssClass = &quot;aspNetDisabled&quot;</code><br><code>WebControl.DisabledCssClass set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.EnableTheming</code> | <code>WebControl.EnableTheming = &quot;true&quot;</code><br><code>WebControl.EnableTheming set = &quot;true&quot;</code> |
| <code>Api.WebControls.WebControl.Enabled</code> | <code>WebControl.Enabled = &quot;true&quot;</code><br><code>WebControl.Enabled set = &quot;true&quot;</code> |
| <code>Api.WebControls.WebControl.Font</code> | <code>WebControl.Font = FontInfo {Bold=false; Italic=false; Name=&quot;&quot;; Overline=false; Size=; Strikeout=false; Underline=false}</code> |
| <code>Api.WebControls.WebControl.ForeColor</code> | <code>WebControl.ForeColor = &quot;Color #00000000 Color [Empty]&quot;</code><br><code>WebControl.ForeColor set = &quot;Color #FF3366CC Color [A=255, R=51, G=102, B=204]&quot;</code> |
| <code>Api.WebControls.WebControl.HasAttributes</code> | <code>WebControl.HasAttributes = &quot;false&quot;</code> |
| <code>Api.WebControls.WebControl.Height</code> | <code>WebControl.Height = Unit </code><br><code>WebControl.Height set = Unit 120px</code> |
| <code>Api.WebControls.WebControl.IsEnabled</code> | <code>WebControl.IsEnabled = &quot;true&quot;</code> |
| <code>Api.WebControls.WebControl.LoadViewState</code> | <code>WebControl.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Web.UI.Pair&#39;.&quot;</code> |
| <code>Api.WebControls.WebControl.MergeStyle</code> | <code>WebControl.MergeStyle(Style) = &quot;ok&quot;</code><br><code>WebControl.MergeStyle(Style) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderCo…</code> |
| <code>Api.WebControls.WebControl.Render</code> | <code>WebControl.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.Render(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;input name=&quot;txtName&quot; type=&quot;text&quot; value=&quot;Taro&quot; id=&quot;txtName&quot; /&gt;</code><br><code>WebControl.Render(HtmlTextWriter) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; Bor…</code> |
| <code>Api.WebControls.WebControl.RenderBeginTag</code> | <code>WebControl.RenderBeginTag(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.RenderBeginTag(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;input name=&quot;txtName&quot; type=&quot;text&quot; value=&quot;Taro&quot; id=&quot;txtName&quot; /&gt;</code><br><code>WebControl.RenderBeginTag(HtmlTextWriter) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Emp…</code> |
| <code>Api.WebControls.WebControl.RenderContents</code> | <code>WebControl.RenderContents(HtmlTextWriter) = &quot;ok&quot;</code><br><code>WebControl.RenderContents(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>WebControl.RenderContents(HtmlTextWriter) after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Emp…</code> |
| <code>Api.WebControls.WebControl.RenderEndTag</code> | <code>WebControl.RenderEndTag(HtmlTextWriter) = &quot;-&gt; System.InvalidOperationException: A PopEndTag was called without a corresponding PushEndTag.&quot;</code> |
| <code>Api.WebControls.WebControl.SaveViewState</code> | <code>WebControl.SaveViewState = null</code><br><code>WebControl.SaveViewState after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderColor=…</code> |
| <code>Api.WebControls.WebControl.SkinID</code> | <code>WebControl.SkinID = &quot;&quot;</code><br><code>WebControl.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.Style</code> | <code>WebControl.Style = CssStyleCollection {Count=Int32 0; Value=null}</code> |
| <code>Api.WebControls.WebControl.SupportsDisabledAttribute</code> | <code>WebControl.SupportsDisabledAttribute = &quot;true&quot;</code> |
| <code>Api.WebControls.WebControl.TabIndex</code> | <code>WebControl.TabIndex = &quot;Int16 0&quot;</code><br><code>WebControl.TabIndex set = &quot;Int16 1&quot;</code> |
| <code>Api.WebControls.WebControl.TagKey</code> | <code>WebControl.TagKey = &quot;HtmlTextWriterTag.Input&quot;</code> |
| <code>Api.WebControls.WebControl.TagName</code> | <code>WebControl.TagName = &quot;input&quot;</code> |
| <code>Api.WebControls.WebControl.ToolTip</code> | <code>WebControl.ToolTip = &quot;&quot;</code><br><code>WebControl.ToolTip set = &quot;Abc 123&quot;</code> |
| <code>Api.WebControls.WebControl.TrackViewState</code> | <code>WebControl.TrackViewState = &quot;ok&quot;</code><br><code>WebControl.TrackViewState after = TextBox#txtName {AccessKey=&quot;&quot;; AutoCompleteType=None; AutoPostBack=false; BackColor=Color #00000000 Color [Empty]; BorderColor…</code> |
| <code>Api.WebControls.WebControl.Width</code> | <code>WebControl.Width = Unit </code><br><code>WebControl.Width set = Unit 120px</code> |

## ② HTML 層

プローブサイトの RenderProbe.aspx で、1 ケース 1 リクエスト（ページライフサイクル全体）で描画したページ全体を比較します。下表はコンポーネント部分（div#probe）、設定時の例外、保存された ViewState の要約です。

| ケース | 設定 | 期待される描画 | 設定時の例外 |
|---|---|---|---|
| <code>.(default)</code> | <code>(nothing set)</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AccessKey=text</code> | <code>c.AccessKey = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> | <code>c.AccessKey = &quot;probe&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=markup</code> | <code>c.AccessKey = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> | <code>c.AccessKey = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=japanese</code> | <code>c.AccessKey = &quot;日本語テキスト&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> | <code>c.AccessKey = &quot;日本語テキスト&quot;; -&gt; System.ArgumentOutOfRangeException: AccessKey too long, cannot be more than one character. [param value]</code> |
| <code>.AccessKey=empty</code> | <code>c.AccessKey = &quot;&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AccessKey=k</code> | <code>c.AccessKey = &quot;k&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; accesskey=&quot;k&quot; /&gt;</code> |  |
| <code>.AppRelativeTemplateSourceDirectory=text</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;probe&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=markup</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=japanese</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;日本語テキスト&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=empty</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=None</code> | <code>c.AutoCompleteType = AutoCompleteType.None;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=Disabled</code> | <code>c.AutoCompleteType = AutoCompleteType.Disabled;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; autocomplete=&quot;off&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=Cellular</code> | <code>c.AutoCompleteType = AutoCompleteType.Cellular;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=Company</code> | <code>c.AutoCompleteType = AutoCompleteType.Company;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=Department</code> | <code>c.AutoCompleteType = AutoCompleteType.Department;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=DisplayName</code> | <code>c.AutoCompleteType = AutoCompleteType.DisplayName;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=Email</code> | <code>c.AutoCompleteType = AutoCompleteType.Email;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=FirstName</code> | <code>c.AutoCompleteType = AutoCompleteType.FirstName;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=Gender</code> | <code>c.AutoCompleteType = AutoCompleteType.Gender;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=HomeCity</code> | <code>c.AutoCompleteType = AutoCompleteType.HomeCity;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=HomeCountryRegion</code> | <code>c.AutoCompleteType = AutoCompleteType.HomeCountryRegion;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=HomeFax</code> | <code>c.AutoCompleteType = AutoCompleteType.HomeFax;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=HomePhone</code> | <code>c.AutoCompleteType = AutoCompleteType.HomePhone;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=HomeState</code> | <code>c.AutoCompleteType = AutoCompleteType.HomeState;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=HomeStreetAddress</code> | <code>c.AutoCompleteType = AutoCompleteType.HomeStreetAddress;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=HomeZipCode</code> | <code>c.AutoCompleteType = AutoCompleteType.HomeZipCode;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=Homepage</code> | <code>c.AutoCompleteType = AutoCompleteType.Homepage;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=JobTitle</code> | <code>c.AutoCompleteType = AutoCompleteType.JobTitle;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=LastName</code> | <code>c.AutoCompleteType = AutoCompleteType.LastName;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=MiddleName</code> | <code>c.AutoCompleteType = AutoCompleteType.MiddleName;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=Notes</code> | <code>c.AutoCompleteType = AutoCompleteType.Notes;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=Office</code> | <code>c.AutoCompleteType = AutoCompleteType.Office;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=Pager</code> | <code>c.AutoCompleteType = AutoCompleteType.Pager;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=BusinessCity</code> | <code>c.AutoCompleteType = AutoCompleteType.BusinessCity;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=BusinessCountryRegion</code> | <code>c.AutoCompleteType = AutoCompleteType.BusinessCountryRegion;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=BusinessFax</code> | <code>c.AutoCompleteType = AutoCompleteType.BusinessFax;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=BusinessPhone</code> | <code>c.AutoCompleteType = AutoCompleteType.BusinessPhone;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=BusinessState</code> | <code>c.AutoCompleteType = AutoCompleteType.BusinessState;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=BusinessStreetAddress</code> | <code>c.AutoCompleteType = AutoCompleteType.BusinessStreetAddress;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=BusinessUrl</code> | <code>c.AutoCompleteType = AutoCompleteType.BusinessUrl;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=BusinessZipCode</code> | <code>c.AutoCompleteType = AutoCompleteType.BusinessZipCode;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=Search</code> | <code>c.AutoCompleteType = AutoCompleteType.Search;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoCompleteType=Enabled</code> | <code>c.AutoCompleteType = AutoCompleteType.Enabled;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; autocomplete=&quot;on&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoPostBack=true</code> | <code>c.AutoPostBack = true;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; onchange=&quot;javascript:setTimeout(&#39;__doPostBack(\&#39;txtName\&#39;,\&#39;\&#39;)&#39;, 0)&quot; onkeypress=&quot;if (WebForm_TextBoxKeyHandler(event) == false) return false;&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoPostBack=false</code> | <code>c.AutoPostBack = false;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.BackColor=Red</code> | <code>c.BackColor = Color.Red;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;background-color:Red;&quot; /&gt;</code> |  |
| <code>.BackColor=#3366CC</code> | <code>c.BackColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;background-color:#3366CC;&quot; /&gt;</code> |  |
| <code>.BackColor=Transparent</code> | <code>c.BackColor = Color.Transparent;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;background-color:Transparent;&quot; /&gt;</code> |  |
| <code>.BorderColor=Red</code> | <code>c.BorderColor = Color.Red;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;border-color:Red;&quot; /&gt;</code> |  |
| <code>.BorderColor=#3366CC</code> | <code>c.BorderColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;border-color:#3366CC;&quot; /&gt;</code> |  |
| <code>.BorderColor=Transparent</code> | <code>c.BorderColor = Color.Transparent;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;border-color:Transparent;&quot; /&gt;</code> |  |
| <code>.BorderStyle=NotSet</code> | <code>c.BorderStyle = BorderStyle.NotSet;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.BorderStyle=None</code> | <code>c.BorderStyle = BorderStyle.None;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;border-style:None;&quot; /&gt;</code> |  |
| <code>.BorderStyle=Dotted</code> | <code>c.BorderStyle = BorderStyle.Dotted;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;border-style:Dotted;&quot; /&gt;</code> |  |
| <code>.BorderStyle=Dashed</code> | <code>c.BorderStyle = BorderStyle.Dashed;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;border-style:Dashed;&quot; /&gt;</code> |  |
| <code>.BorderStyle=Solid</code> | <code>c.BorderStyle = BorderStyle.Solid;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;border-style:Solid;&quot; /&gt;</code> |  |
| <code>.BorderStyle=Double</code> | <code>c.BorderStyle = BorderStyle.Double;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;border-style:Double;&quot; /&gt;</code> |  |
| <code>.BorderStyle=Groove</code> | <code>c.BorderStyle = BorderStyle.Groove;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;border-style:Groove;&quot; /&gt;</code> |  |
| <code>.BorderStyle=Ridge</code> | <code>c.BorderStyle = BorderStyle.Ridge;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;border-style:Ridge;&quot; /&gt;</code> |  |
| <code>.BorderStyle=Inset</code> | <code>c.BorderStyle = BorderStyle.Inset;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;border-style:Inset;&quot; /&gt;</code> |  |
| <code>.BorderStyle=Outset</code> | <code>c.BorderStyle = BorderStyle.Outset;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;border-style:Outset;&quot; /&gt;</code> |  |
| <code>.BorderWidth=120px</code> | <code>c.BorderWidth = Unit.Pixel(120);</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;border-width:120px;border-style:solid;&quot; /&gt;</code> |  |
| <code>.BorderWidth=50%</code> | <code>c.BorderWidth = Unit.Percentage(50);</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> | <code>c.BorderWidth = Unit.Percentage(50); -&gt; System.ArgumentOutOfRangeException: BorderWidth must be a non-negative number and cannot be a percentage. [param value]</code> |
| <code>.BorderWidth=2em</code> | <code>c.BorderWidth = new Unit(2, UnitType.Em);</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;border-width:2em;border-style:solid;&quot; /&gt;</code> |  |
| <code>.CausesValidation=true</code> | <code>c.CausesValidation = true;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.CausesValidation=false</code> | <code>c.CausesValidation = false;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.ClientIDMode=Inherit</code> | <code>c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.ClientIDMode=AutoID</code> | <code>c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.ClientIDMode=Predictable</code> | <code>c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.ClientIDMode=Static</code> | <code>c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.Columns=0</code> | <code>c.Columns = 0;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.Columns=3</code> | <code>c.Columns = 3;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; size=&quot;3&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.Columns=-1</code> | <code>c.Columns = -1;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> | <code>c.Columns = -1; -&gt; System.ArgumentOutOfRangeException: Columns must be greater than -1. [param Columns]</code> |
| <code>.CssClass=text</code> | <code>c.CssClass = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; class=&quot;probe&quot; /&gt;</code> |  |
| <code>.CssClass=markup</code> | <code>c.CssClass = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; class=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; /&gt;</code> |  |
| <code>.CssClass=japanese</code> | <code>c.CssClass = &quot;日本語テキスト&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; class=&quot;日本語テキスト&quot; /&gt;</code> |  |
| <code>.CssClass=empty</code> | <code>c.CssClass = &quot;&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.EnableTheming=true</code> | <code>c.EnableTheming = true;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.EnableTheming=false</code> | <code>c.EnableTheming = false;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.EnableViewState=true</code> | <code>c.EnableViewState = true;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.EnableViewState=false</code> | <code>c.EnableViewState = false;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.Enabled=true</code> | <code>c.Enabled = true;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.Enabled=false</code> | <code>c.Enabled = false;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; disabled=&quot;disabled&quot; class=&quot;aspNetDisabled&quot; /&gt;</code> |  |
| <code>.Font.Bold=true</code> | <code>c.Font.Bold = true;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;font-weight:bold;&quot; /&gt;</code> |  |
| <code>.Font.Bold=false</code> | <code>c.Font.Bold = false;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;font-weight:normal;&quot; /&gt;</code> |  |
| <code>.Font.Italic=true</code> | <code>c.Font.Italic = true;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;font-style:italic;&quot; /&gt;</code> |  |
| <code>.Font.Italic=false</code> | <code>c.Font.Italic = false;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;font-style:normal;&quot; /&gt;</code> |  |
| <code>.Font.Name=text</code> | <code>c.Font.Name = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;font-family:probe;&quot; /&gt;</code> |  |
| <code>.Font.Name=markup</code> | <code>c.Font.Name = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;font-family:&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;;&quot; /&gt;</code> |  |
| <code>.Font.Name=japanese</code> | <code>c.Font.Name = &quot;日本語テキスト&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;font-family:日本語テキスト;&quot; /&gt;</code> |  |
| <code>.Font.Name=empty</code> | <code>c.Font.Name = &quot;&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.Font.Names=Arial,MS-Gothic</code> | <code>c.Font.Names = new[] { &quot;Arial&quot;, &quot;MS Gothic&quot; };</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;font-family:Arial,MS Gothic;&quot; /&gt;</code> |  |
| <code>.Font.Overline=true</code> | <code>c.Font.Overline = true;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;text-decoration: overline;&quot; /&gt;</code> |  |
| <code>.Font.Overline=false</code> | <code>c.Font.Overline = false;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;text-decoration:none;&quot; /&gt;</code> |  |
| <code>.Font.Size=12pt</code> | <code>c.Font.Size = FontUnit.Point(12);</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;font-size:12pt;&quot; /&gt;</code> |  |
| <code>.Font.Size=Large</code> | <code>c.Font.Size = FontUnit.Large;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;font-size:Large;&quot; /&gt;</code> |  |
| <code>.Font.Size=150%</code> | <code>c.Font.Size = new FontUnit(Unit.Percentage(150));</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;font-size:150%;&quot; /&gt;</code> |  |
| <code>.Font.Strikeout=true</code> | <code>c.Font.Strikeout = true;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;text-decoration: line-through;&quot; /&gt;</code> |  |
| <code>.Font.Strikeout=false</code> | <code>c.Font.Strikeout = false;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;text-decoration:none;&quot; /&gt;</code> |  |
| <code>.Font.Underline=true</code> | <code>c.Font.Underline = true;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;text-decoration:underline;&quot; /&gt;</code> |  |
| <code>.Font.Underline=false</code> | <code>c.Font.Underline = false;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;text-decoration:none;&quot; /&gt;</code> |  |
| <code>.ForeColor=Red</code> | <code>c.ForeColor = Color.Red;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;color:Red;&quot; /&gt;</code> |  |
| <code>.ForeColor=#3366CC</code> | <code>c.ForeColor = Color.FromArgb(0x33, 0x66, 0xCC);</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;color:#3366CC;&quot; /&gt;</code> |  |
| <code>.ForeColor=Transparent</code> | <code>c.ForeColor = Color.Transparent;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;color:Transparent;&quot; /&gt;</code> |  |
| <code>.Height=120px</code> | <code>c.Height = Unit.Pixel(120);</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;height:120px;&quot; /&gt;</code> |  |
| <code>.Height=50%</code> | <code>c.Height = Unit.Percentage(50);</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;height:50%;&quot; /&gt;</code> |  |
| <code>.Height=2em</code> | <code>c.Height = new Unit(2, UnitType.Em);</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;height:2em;&quot; /&gt;</code> |  |
| <code>.ID=text</code> | <code>c.ID = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;probe&quot; type=&quot;text&quot; id=&quot;probe&quot; /&gt;</code> |  |
| <code>.ID=markup</code> | <code>c.ID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input name=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; type=&quot;text&quot; id=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; /&gt;</code> |  |
| <code>.ID=japanese</code> | <code>c.ID = &quot;日本語テキスト&quot;;</code> | <code>&lt;input name=&quot;日本語テキスト&quot; type=&quot;text&quot; id=&quot;日本語テキスト&quot; /&gt;</code> |  |
| <code>.ID=empty</code> | <code>c.ID = &quot;&quot;;</code> | <code>&lt;input name=&quot;ctl02&quot; type=&quot;text&quot; id=&quot;ctl02&quot; /&gt;</code> |  |
| <code>.MaxLength=0</code> | <code>c.MaxLength = 0;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.MaxLength=3</code> | <code>c.MaxLength = 3;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; maxlength=&quot;3&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.MaxLength=-1</code> | <code>c.MaxLength = -1;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> | <code>c.MaxLength = -1; -&gt; System.ArgumentOutOfRangeException: Specified argument was out of the range of valid values. [param value]</code> |
| <code>.ReadOnly=true</code> | <code>c.ReadOnly = true;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; readonly=&quot;readonly&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.ReadOnly=false</code> | <code>c.ReadOnly = false;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.Rows=0</code> | <code>c.Rows = 0;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.Rows=3</code> | <code>c.Rows = 3;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.Rows=-1</code> | <code>c.Rows = -1;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> | <code>c.Rows = -1; -&gt; System.ArgumentOutOfRangeException: Rows must be greater than -1. [param Rows]</code> |
| <code>.SkinID=text</code> | <code>c.SkinID = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.SkinID=markup</code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.SkinID=japanese</code> | <code>c.SkinID = &quot;日本語テキスト&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.SkinID=empty</code> | <code>c.SkinID = &quot;&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.TabIndex=0</code> | <code>c.TabIndex = 0;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.TabIndex=3</code> | <code>c.TabIndex = 3;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; tabindex=&quot;3&quot; /&gt;</code> |  |
| <code>.TabIndex=-1</code> | <code>c.TabIndex = -1;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; tabindex=&quot;-1&quot; /&gt;</code> |  |
| <code>.Text=text</code> | <code>c.Text = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; value=&quot;probe&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.Text=markup</code> | <code>c.Text = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; value=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.Text=japanese</code> | <code>c.Text = &quot;日本語テキスト&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; value=&quot;日本語テキスト&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.Text=empty</code> | <code>c.Text = &quot;&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.Text=lines</code> | <code>c.Text = &quot;Line1\r\nLine2&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; value=&quot;Line1↵Line2&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.Text=leading-newline</code> | <code>c.Text = &quot;\r\nabc&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; value=&quot;↵abc&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.Text=spaces</code> | <code>c.Text = &quot;  a  b  &quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; value=&quot;  a  b  &quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.TextMode=SingleLine</code> | <code>c.TextMode = TextBoxMode.SingleLine;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.TextMode=MultiLine</code> | <code>c.TextMode = TextBoxMode.MultiLine;</code> | <code>&lt;textarea name=&quot;txtName&quot; rows=&quot;2&quot; cols=&quot;20&quot; id=&quot;txtName&quot;&gt;↵&lt;/textarea&gt;</code> |  |
| <code>.TextMode=Password</code> | <code>c.TextMode = TextBoxMode.Password;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;password&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.TextMode=Color</code> | <code>c.TextMode = TextBoxMode.Color;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;color&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.TextMode=Date</code> | <code>c.TextMode = TextBoxMode.Date;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;date&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.TextMode=DateTime</code> | <code>c.TextMode = TextBoxMode.DateTime;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;datetime&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.TextMode=DateTimeLocal</code> | <code>c.TextMode = TextBoxMode.DateTimeLocal;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;datetime-local&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.TextMode=Email</code> | <code>c.TextMode = TextBoxMode.Email;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;email&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.TextMode=Month</code> | <code>c.TextMode = TextBoxMode.Month;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;month&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.TextMode=Number</code> | <code>c.TextMode = TextBoxMode.Number;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;number&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.TextMode=Range</code> | <code>c.TextMode = TextBoxMode.Range;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;range&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.TextMode=Search</code> | <code>c.TextMode = TextBoxMode.Search;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;search&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.TextMode=Phone</code> | <code>c.TextMode = TextBoxMode.Phone;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;tel&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.TextMode=Time</code> | <code>c.TextMode = TextBoxMode.Time;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;time&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.TextMode=Url</code> | <code>c.TextMode = TextBoxMode.Url;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;url&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.TextMode=Week</code> | <code>c.TextMode = TextBoxMode.Week;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;week&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.ToolTip=text</code> | <code>c.ToolTip = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; title=&quot;probe&quot; /&gt;</code> |  |
| <code>.ToolTip=markup</code> | <code>c.ToolTip = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; title=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; /&gt;</code> |  |
| <code>.ToolTip=japanese</code> | <code>c.ToolTip = &quot;日本語テキスト&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; title=&quot;日本語テキスト&quot; /&gt;</code> |  |
| <code>.ToolTip=empty</code> | <code>c.ToolTip = &quot;&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.ValidateRequestMode=Inherit</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Inherit;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.ValidateRequestMode=Disabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Disabled;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.ValidateRequestMode=Enabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Enabled;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.ValidationGroup=text</code> | <code>c.ValidationGroup = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.ValidationGroup=markup</code> | <code>c.ValidationGroup = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.ValidationGroup=japanese</code> | <code>c.ValidationGroup = &quot;日本語テキスト&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.ValidationGroup=empty</code> | <code>c.ValidationGroup = &quot;&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.ViewStateMode=Inherit</code> | <code>c.ViewStateMode = ViewStateMode.Inherit;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.ViewStateMode=Enabled</code> | <code>c.ViewStateMode = ViewStateMode.Enabled;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.ViewStateMode=Disabled</code> | <code>c.ViewStateMode = ViewStateMode.Disabled;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.Visible=true</code> | <code>c.Visible = true;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.Visible=false</code> | <code>c.Visible = false;</code> | <code></code> |  |
| <code>.Width=120px</code> | <code>c.Width = Unit.Pixel(120);</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;width:120px;&quot; /&gt;</code> |  |
| <code>.Width=50%</code> | <code>c.Width = Unit.Percentage(50);</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;width:50%;&quot; /&gt;</code> |  |
| <code>.Width=2em</code> | <code>c.Width = new Unit(2, UnitType.Em);</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;width:2em;&quot; /&gt;</code> |  |
| <code>.Wrap=true</code> | <code>c.Wrap = true;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.Wrap=false</code> | <code>c.Wrap = false;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.Attributes[data-probe]=text</code> | <code>c.Attributes[data-probe] = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; data-probe=&quot;probe&quot; /&gt;</code> |  |
| <code>.Attributes[onfocus]=text</code> | <code>c.Attributes[onfocus] = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; onfocus=&quot;probe&quot; /&gt;</code> |  |
| <code>.Attributes[data-probe]=markup</code> | <code>c.Attributes[data-probe] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; data-probe=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; /&gt;</code> |  |
| <code>.Attributes[onfocus]=markup</code> | <code>c.Attributes[onfocus] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; onfocus=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; /&gt;</code> |  |
| <code>.Style[color]=red</code> | <code>c.Style[color] = &quot;red&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;color:red;&quot; /&gt;</code> |  |
| <code>.Style[margin-left]=red</code> | <code>c.Style[margin-left] = &quot;red&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;margin-left:red;&quot; /&gt;</code> |  |
| <code>.Style[color]=1em</code> | <code>c.Style[color] = &quot;1em&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;color:1em;&quot; /&gt;</code> |  |
| <code>.Style[margin-left]=1em</code> | <code>c.Style[margin-left] = &quot;1em&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;margin-left:1em;&quot; /&gt;</code> |  |
| <code>.TextMode=MultiLine+Text=lines</code> | <code>c.TextMode = TextBoxMode.MultiLine; c.Text = &quot;Line1\r\nLine2&quot;;</code> | <code>&lt;textarea name=&quot;txtName&quot; rows=&quot;2&quot; cols=&quot;20&quot; id=&quot;txtName&quot;&gt;↵Line1↵Line2&lt;/textarea&gt;</code> |  |
| <code>.TextMode=MultiLine+Text=leading-newline</code> | <code>c.TextMode = TextBoxMode.MultiLine; c.Text = &quot;\r\nabc&quot;;</code> | <code>&lt;textarea name=&quot;txtName&quot; rows=&quot;2&quot; cols=&quot;20&quot; id=&quot;txtName&quot;&gt;↵↵abc&lt;/textarea&gt;</code> |  |
| <code>.TextMode=MultiLine+Text=markup</code> | <code>c.TextMode = TextBoxMode.MultiLine; c.Text = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;textarea name=&quot;txtName&quot; rows=&quot;2&quot; cols=&quot;20&quot; id=&quot;txtName&quot;&gt;↵&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&lt;/textarea&gt;</code> |  |
| <code>.TextMode=MultiLine+Rows=3+Columns=3</code> | <code>c.TextMode = TextBoxMode.MultiLine; c.Rows = 3; c.Columns = 3;</code> | <code>&lt;textarea name=&quot;txtName&quot; rows=&quot;3&quot; cols=&quot;3&quot; id=&quot;txtName&quot;&gt;↵&lt;/textarea&gt;</code> |  |
| <code>.TextMode=MultiLine+Wrap=false</code> | <code>c.TextMode = TextBoxMode.MultiLine; c.Wrap = false;</code> | <code>&lt;textarea name=&quot;txtName&quot; rows=&quot;2&quot; cols=&quot;20&quot; wrap=&quot;off&quot; id=&quot;txtName&quot;&gt;↵&lt;/textarea&gt;</code> |  |
| <code>.TextMode=MultiLine+MaxLength=3</code> | <code>c.TextMode = TextBoxMode.MultiLine; c.MaxLength = 3;</code> | <code>&lt;textarea name=&quot;txtName&quot; rows=&quot;2&quot; cols=&quot;20&quot; maxlength=&quot;3&quot; id=&quot;txtName&quot;&gt;↵&lt;/textarea&gt;</code> |  |
| <code>.TextMode=MultiLine+ReadOnly=true</code> | <code>c.TextMode = TextBoxMode.MultiLine; c.ReadOnly = true;</code> | <code>&lt;textarea name=&quot;txtName&quot; rows=&quot;2&quot; cols=&quot;20&quot; readonly=&quot;readonly&quot; id=&quot;txtName&quot;&gt;↵&lt;/textarea&gt;</code> |  |
| <code>.TextMode=MultiLine+AutoPostBack=true</code> | <code>c.TextMode = TextBoxMode.MultiLine; c.AutoPostBack = true;</code> | <code>&lt;textarea name=&quot;txtName&quot; rows=&quot;2&quot; cols=&quot;20&quot; onchange=&quot;javascript:setTimeout(&#39;__doPostBack(\&#39;txtName\&#39;,\&#39;\&#39;)&#39;, 0)&quot; id=&quot;txtName&quot;&gt;↵&lt;/textarea&gt;</code> |  |
| <code>.TextMode=Password+Text=text</code> | <code>c.TextMode = TextBoxMode.Password; c.Text = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;password&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.TextMode=Password+AutoCompleteType=Disabled</code> | <code>c.TextMode = TextBoxMode.Password; c.AutoCompleteType = AutoCompleteType.Disabled;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;password&quot; autocomplete=&quot;off&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.TextMode=Number+MaxLength=3</code> | <code>c.TextMode = TextBoxMode.Number; c.MaxLength = 3;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;number&quot; maxlength=&quot;3&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.TextMode=Date+Text=text</code> | <code>c.TextMode = TextBoxMode.Date; c.Text = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;date&quot; value=&quot;probe&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.ReadOnly=true+Enabled=false</code> | <code>c.ReadOnly = true; c.Enabled = false;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; readonly=&quot;readonly&quot; id=&quot;txtName&quot; disabled=&quot;disabled&quot; class=&quot;aspNetDisabled&quot; /&gt;</code> |  |
| <code>.Enabled=false+CssClass=text</code> | <code>c.Enabled = false; c.CssClass = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; disabled=&quot;disabled&quot; class=&quot;aspNetDisabled probe&quot; /&gt;</code> |  |
| <code>.AutoPostBack=true+CausesValidation=true</code> | <code>c.AutoPostBack = true; c.CausesValidation = true;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; onchange=&quot;javascript:setTimeout(&#39;WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;txtName&quot;, &quot;&quot;, true, &quot;&quot;, &quot;&quot;, false, true))&#39;, 0)&quot; onkeypress=&quot;if (WebForm_TextBoxKeyHandler(event) == false) return false;&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoPostBack=true+CausesValidation=true+ValidationGroup=text</code> | <code>c.AutoPostBack = true; c.CausesValidation = true; c.ValidationGroup = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; onchange=&quot;javascript:setTimeout(&#39;WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;txtName&quot;, &quot;&quot;, true, &quot;probe&quot;, &quot;&quot;, false, true))&#39;, 0)&quot; onkeypress=&quot;if (WebForm_TextBoxKeyHandler(event) == false) return false;&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.AutoPostBack=true+Enabled=false</code> | <code>c.AutoPostBack = true; c.Enabled = false;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; onchange=&quot;javascript:setTimeout(&#39;__doPostBack(\&#39;txtName\&#39;,\&#39;\&#39;)&#39;, 0)&quot; onkeypress=&quot;if (WebForm_TextBoxKeyHandler(event) == false) return false;&quot; id=&quot;txtName&quot; disabled=&quot;disabled&quot; class=&quot;aspNetDisabled&quot; /&gt;</code> |  |
| <code>.AutoPostBack=true+Visible=false</code> | <code>c.AutoPostBack = true; c.Visible = false;</code> | <code></code> |  |
| <code>.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | <code>c.Width = Unit.Pixel(120); c.Height = new Unit(2, UnitType.Em); c.BorderWidth = Unit.Pixel(120); c.BorderStyle = BorderStyle.Solid; c.BackColor = Color.FromArgb(0x33, 0x66, 0xCC); c.ForeColor = Color.Red; c.Font.Bold = true;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;color:Red;background-color:#3366CC;border-width:120px;border-style:Solid;font-weight:bold;height:2em;width:120px;&quot; /&gt;</code> |  |
| <code>.Columns=3+Width=120px</code> | <code>c.Columns = 3; c.Width = Unit.Pixel(120);</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; size=&quot;3&quot; id=&quot;txtName&quot; style=&quot;width:120px;&quot; /&gt;</code> |  |
| <code>.ToolTip=text+Attributes[data-probe]=text</code> | <code>c.ToolTip = &quot;probe&quot;; c.Attributes[data-probe] = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; title=&quot;probe&quot; data-probe=&quot;probe&quot; /&gt;</code> |  |
| <code>.ForeColor=#3366CC+Style[color]=red</code> | <code>c.ForeColor = Color.FromArgb(0x33, 0x66, 0xCC); c.Style[color] = &quot;red&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; style=&quot;color:#3366CC;color:red;&quot; /&gt;</code> |  |
| <code>.Text=markup+EnableViewState=false</code> | <code>c.Text = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; c.EnableViewState = false;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; value=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>.Text=text+Visible=false</code> | <code>c.Text = &quot;probe&quot;; c.Visible = false;</code> | <code></code> |  |
| <code>[naming container].(default)</code> | <code>[naming container] (nothing set)</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input name=&quot;box$txtName&quot; type=&quot;text&quot; id=&quot;box_txtName&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=AutoID</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input name=&quot;box$txtName&quot; type=&quot;text&quot; id=&quot;box_txtName&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Static</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input name=&quot;box$txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Predictable</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input name=&quot;box$txtName&quot; type=&quot;text&quot; id=&quot;box_txtName&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Inherit</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input name=&quot;box$txtName&quot; type=&quot;text&quot; id=&quot;box_txtName&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].AutoPostBack=true</code> | <code>[naming container] c.AutoPostBack = true;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input name=&quot;box$txtName&quot; type=&quot;text&quot; onchange=&quot;javascript:setTimeout(&#39;__doPostBack(\&#39;box$txtName\&#39;,\&#39;\&#39;)&#39;, 0)&quot; onkeypress=&quot;if (WebForm_TextBoxKeyHandler(event) == false) return false;&quot; id=&quot;box_txtName&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ID=text</code> | <code>[naming container] c.ID = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input name=&quot;box$probe&quot; type=&quot;text&quot; id=&quot;box_probe&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[themed].(default)</code> | <code>[themed] (nothing set)</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; class=&quot;skinned&quot; style=&quot;background-color:#EEEEEE;&quot; /&gt;</code> |  |
| <code>[themed].SkinID=text</code> | <code>[themed] c.SkinID = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; class=&quot;skinned-probe&quot; style=&quot;width:300px;&quot; /&gt;</code> |  |
| <code>[themed].EnableTheming=false</code> | <code>[themed] c.EnableTheming = false;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; /&gt;</code> |  |
| <code>[themed].CssClass=text</code> | <code>[themed] c.CssClass = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; class=&quot;skinned&quot; style=&quot;background-color:#EEEEEE;&quot; /&gt;</code> |  |
| <code>[themed].Width=120px</code> | <code>[themed] c.Width = Unit.Pixel(120);</code> | <code>&lt;input name=&quot;txtName&quot; type=&quot;text&quot; id=&quot;txtName&quot; class=&quot;skinned&quot; style=&quot;background-color:#EEEEEE;width:120px;&quot; /&gt;</code> |  |

## ③ シナリオ層

ブラウザ（Playwright）で TextBoxScenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。

| # | 操作 | 対象 | 値 |
|---|---|---|---|
| 1 | goto | /TextBoxScenario.aspx | <code></code> |
| 2 | snapshot |  | **TextBox [render][databind] first request** |
| 3 | fill | txtPlain | <code>Hanako</code> |
| 4 | fill | txtPassword | <code>secret</code> |
| 5 | fill | txtMulti | <code>Line1↵Line2</code> |
| 6 | fill | txtNoViewState | <code>changed</code> |
| 7 | fill | txtRaw | <code>&lt;b&gt;raw&lt;/b&gt;</code> |
| 8 | fill | txtDynamic | <code>dyn</code> |
| 9 | setvalue | txtReadOnly | <code>tampered</code> |
| 10 | click | btnSubmit | <code></code> |
| 11 | snapshot |  | **TextBox [postdata][change][readonly][disabled][password][dynamic][requestvalidation] posted values** |
| 12 | click | btnSubmit | <code></code> |
| 13 | snapshot |  | **TextBox [viewstate][change] posted again unchanged** |
| 14 | click | btnShow | <code></code> |
| 15 | snapshot |  | **TextBox [viewstate][visible] hidden text box shown** |
| 16 | fill | txtAuto | <code>auto</code> |
| 17 | press | txtAuto | <code>Tab</code> |
| 18 | snapshot |  | **TextBox [autopostback][change] auto postback** |
| 19 | fill | txtAutoValidated | <code>x</code> |
| 20 | press | txtAutoValidated | <code>Tab</code> |
| 21 | snapshot |  | **TextBox [autopostback][causesvalidation] auto postback, valid** |
| 22 | fill | txtAutoValidated | <code></code> |
| 23 | press | txtAutoValidated | <code>Tab</code> |
| 24 | snapshot |  | **TextBox [autopostback][causesvalidation] auto postback, emptied** |
| 25 | click | btnG1 | <code></code> |
| 26 | snapshot |  | **TextBox [validation] group g1, empty** |
| 27 | fill | txtValidated | <code>abc</code> |
| 28 | click | btnG1 | <code></code> |
| 29 | snapshot |  | **TextBox [validation] group g1, not digits** |
| 30 | fill | txtValidated | <code>123</code> |
| 31 | click | btnG1 | <code></code> |
| 32 | snapshot |  | **TextBox [validation] group g1, valid** |
| 33 | click | btnG3 | <code></code> |
| 34 | snapshot |  | **TextBox [clientvalidation] group g3, empty (no postback)** |
| 35 | fill | txtBound | <code>Jiro</code> |
| 36 | click | btnUpdate | <code></code> |
| 37 | snapshot |  | **TextBox [bind] two-way binding in a template** |
| 38 | fill | txtPlain | <code>&lt;b&gt;x&lt;/b&gt;</code> |
| 39 | click | btnSubmit | <code></code> |
| 40 | snapshot |  | **TextBox [requestvalidation] markup posted** |

<details><summary>TextBox [render][databind] first request（期待される本文）</summary>

```text
Plain:
Auto:
ReadOnly:
Disabled:
Password:
Multi:
NoViewState:
Hidden:
Raw:
Dynamic:
Validated (g1):
Auto validated (g2):
Client validated (g3):
Bound:
Load: IsPostBack=False
Text txtPlain: "Taro"
Text txtAuto: ""
Text txtReadOnly: "fixed"
Text txtDisabled: "disabled"
Text txtPassword: ""
Text txtMulti: ""
Text txtNoViewState: "initial"
Text txtHidden: "set while hidden" (hidden)
Text txtRaw: ""
Text txtDynamic: "dynamic"
Text txtValidated: ""
Text txtAutoValidated: ""
Text txtClient: ""
```

</details>

<details><summary>TextBox [postdata][change][readonly][disabled][password][dynamic][requestvalidation] posted values（期待される本文）</summary>

```text
Plain:
Auto:
ReadOnly:
Disabled:
Password:
Multi:
NoViewState:
Hidden:
Raw:
Dynamic:
Validated (g1):
Auto validated (g2):
Client validated (g3):
Bound:
Load: IsPostBack=True
TextChanged txtPlain: "Hanako"
TextChanged txtPassword: "secret"
TextChanged txtMulti: "Line1\r\nLine2"
TextChanged txtNoViewState: "changed"
TextChanged txtRaw: "<b>raw</b>"
TextChanged txtDynamic: "dyn"
IsValid: (not validated)
Text txtPlain: "Hanako"
Text txtAuto: ""
Text txtReadOnly: "fixed"
Text txtDisabled: "disabled"
Text txtPassword: "secret"
Text txtMulti: "Line1\r\nLine2"
Text txtNoViewState: "changed"
Text txtHidden: "set while hidden" (hidden)
Text txtRaw: "<b>raw</b>"
Text txtDynamic: "dyn"
Text txtValidated: ""
Text txtAutoValidated: ""
Text txtClient: ""
```

</details>

<details><summary>TextBox [viewstate][change] posted again unchanged（期待される本文）</summary>

```text
Plain:
Auto:
ReadOnly:
Disabled:
Password:
Multi:
NoViewState:
Hidden:
Raw:
Dynamic:
Validated (g1):
Auto validated (g2):
Client validated (g3):
Bound:
Load: IsPostBack=True
TextChanged txtNoViewState: "changed"
IsValid: (not validated)
Text txtPlain: "Hanako"
Text txtAuto: ""
Text txtReadOnly: "fixed"
Text txtDisabled: "disabled"
Text txtPassword: ""
Text txtMulti: "Line1\r\nLine2"
Text txtNoViewState: "changed"
Text txtHidden: "set while hidden" (hidden)
Text txtRaw: "<b>raw</b>"
Text txtDynamic: "dyn"
Text txtValidated: ""
Text txtAutoValidated: ""
Text txtClient: ""
```

</details>

<details><summary>TextBox [viewstate][visible] hidden text box shown（期待される本文）</summary>

```text
Plain:
Auto:
ReadOnly:
Disabled:
Password:
Multi:
NoViewState:
Hidden:
Raw:
Dynamic:
Validated (g1):
Auto validated (g2):
Client validated (g3):
Bound:
Load: IsPostBack=True
TextChanged txtNoViewState: "changed"
Click btnShow
IsValid: (not validated)
Text txtPlain: "Hanako"
Text txtAuto: ""
Text txtReadOnly: "fixed"
Text txtDisabled: "disabled"
Text txtPassword: ""
Text txtMulti: "Line1\r\nLine2"
Text txtNoViewState: "changed"
Text txtHidden: "set while hidden"
Text txtRaw: "<b>raw</b>"
Text txtDynamic: "dyn"
Text txtValidated: ""
Text txtAutoValidated: ""
Text txtClient: ""
```

</details>

<details><summary>TextBox [autopostback][change] auto postback（期待される本文）</summary>

```text
Plain:
Auto:
ReadOnly:
Disabled:
Password:
Multi:
NoViewState:
Hidden:
Raw:
Dynamic:
Validated (g1):
Auto validated (g2):
Client validated (g3):
Bound:
Load: IsPostBack=True
TextChanged txtAuto: "auto"
TextChanged txtNoViewState: "changed"
IsValid: (not validated)
Text txtPlain: "Hanako"
Text txtAuto: "auto"
Text txtReadOnly: "fixed"
Text txtDisabled: "disabled"
Text txtPassword: ""
Text txtMulti: "Line1\r\nLine2"
Text txtNoViewState: "changed"
Text txtHidden: "set while hidden"
Text txtRaw: "<b>raw</b>"
Text txtDynamic: "dyn"
Text txtValidated: ""
Text txtAutoValidated: ""
Text txtClient: ""
```

</details>

<details><summary>TextBox [autopostback][causesvalidation] auto postback, valid（期待される本文）</summary>

```text
Plain:
Auto:
ReadOnly:
Disabled:
Password:
Multi:
NoViewState:
Hidden:
Raw:
Dynamic:
Validated (g1):
Auto validated (g2):
Client validated (g3):
Bound:
Load: IsPostBack=True
TextChanged txtNoViewState: "changed"
TextChanged txtAutoValidated: "x"
IsValid: True
Text txtPlain: "Hanako"
Text txtAuto: "auto"
Text txtReadOnly: "fixed"
Text txtDisabled: "disabled"
Text txtPassword: ""
Text txtMulti: "Line1\r\nLine2"
Text txtNoViewState: "changed"
Text txtHidden: "set while hidden"
Text txtRaw: "<b>raw</b>"
Text txtDynamic: "dyn"
Text txtValidated: ""
Text txtAutoValidated: "x"
Text txtClient: ""
```

</details>

<details><summary>TextBox [autopostback][causesvalidation] auto postback, emptied（期待される本文）</summary>

```text
Plain:
Auto:
ReadOnly:
Disabled:
Password:
Multi:
NoViewState:
Hidden:
Raw:
Dynamic:
Validated (g1):
Auto validated (g2): Required (g2)
Client validated (g3):
Bound:
Load: IsPostBack=True
TextChanged txtNoViewState: "changed"
TextChanged txtAutoValidated: ""
IsValid: False
Text txtPlain: "Hanako"
Text txtAuto: "auto"
Text txtReadOnly: "fixed"
Text txtDisabled: "disabled"
Text txtPassword: ""
Text txtMulti: "Line1\r\nLine2"
Text txtNoViewState: "changed"
Text txtHidden: "set while hidden"
Text txtRaw: "<b>raw</b>"
Text txtDynamic: "dyn"
Text txtValidated: ""
Text txtAutoValidated: ""
Text txtClient: ""
```

</details>

<details><summary>TextBox [validation] group g1, empty（期待される本文）</summary>

```text
Plain:
Auto:
ReadOnly:
Disabled:
Password:
Multi:
NoViewState:
Hidden:
Raw:
Dynamic:
Validated (g1): Required (g1)
Auto validated (g2):
Client validated (g3):
Bound:
Load: IsPostBack=True
TextChanged txtNoViewState: "changed"
IsValid: False
Text txtPlain: "Hanako"
Text txtAuto: "auto"
Text txtReadOnly: "fixed"
Text txtDisabled: "disabled"
Text txtPassword: ""
Text txtMulti: "Line1\r\nLine2"
Text txtNoViewState: "changed"
Text txtHidden: "set while hidden"
Text txtRaw: "<b>raw</b>"
Text txtDynamic: "dyn"
Text txtValidated: ""
Text txtAutoValidated: ""
Text txtClient: ""
```

</details>

<details><summary>TextBox [validation] group g1, not digits（期待される本文）</summary>

```text
Plain:
Auto:
ReadOnly:
Disabled:
Password:
Multi:
NoViewState:
Hidden:
Raw:
Dynamic:
Validated (g1): Digits only (g1)
Auto validated (g2):
Client validated (g3):
Bound:
Load: IsPostBack=True
TextChanged txtNoViewState: "changed"
IsValid: False
Text txtPlain: "Hanako"
Text txtAuto: "auto"
Text txtReadOnly: "fixed"
Text txtDisabled: "disabled"
Text txtPassword: ""
Text txtMulti: "Line1\r\nLine2"
Text txtNoViewState: "changed"
Text txtHidden: "set while hidden"
Text txtRaw: "<b>raw</b>"
Text txtDynamic: "dyn"
Text txtValidated: "abc"
Text txtAutoValidated: ""
Text txtClient: ""
```

</details>

<details><summary>TextBox [validation] group g1, valid（期待される本文）</summary>

```text
Plain:
Auto:
ReadOnly:
Disabled:
Password:
Multi:
NoViewState:
Hidden:
Raw:
Dynamic:
Validated (g1):
Auto validated (g2):
Client validated (g3):
Bound:
Load: IsPostBack=True
TextChanged txtNoViewState: "changed"
IsValid: True
Text txtPlain: "Hanako"
Text txtAuto: "auto"
Text txtReadOnly: "fixed"
Text txtDisabled: "disabled"
Text txtPassword: ""
Text txtMulti: "Line1\r\nLine2"
Text txtNoViewState: "changed"
Text txtHidden: "set while hidden"
Text txtRaw: "<b>raw</b>"
Text txtDynamic: "dyn"
Text txtValidated: "123"
Text txtAutoValidated: ""
Text txtClient: ""
```

</details>

<details><summary>TextBox [clientvalidation] group g3, empty (no postback)（期待される本文）</summary>

```text
Plain:
Auto:
ReadOnly:
Disabled:
Password:
Multi:
NoViewState:
Hidden:
Raw:
Dynamic:
Validated (g1):
Auto validated (g2):
Client validated (g3): Required (g3, in the browser)
Bound:
Load: IsPostBack=True
TextChanged txtNoViewState: "changed"
IsValid: True
Text txtPlain: "Hanako"
Text txtAuto: "auto"
Text txtReadOnly: "fixed"
Text txtDisabled: "disabled"
Text txtPassword: ""
Text txtMulti: "Line1\r\nLine2"
Text txtNoViewState: "changed"
Text txtHidden: "set while hidden"
Text txtRaw: "<b>raw</b>"
Text txtDynamic: "dyn"
Text txtValidated: "123"
Text txtAutoValidated: ""
Text txtClient: ""
```

</details>

<details><summary>TextBox [bind] two-way binding in a template（期待される本文）</summary>

```text
Plain:
Auto:
ReadOnly:
Disabled:
Password:
Multi:
NoViewState:
Hidden:
Raw:
Dynamic:
Validated (g1):
Auto validated (g2):
Client validated (g3):
Bound:
Load: IsPostBack=True
TextChanged txtNoViewState: "changed"
ItemUpdating: Name="Jiro"
IsValid: (not validated)
Text txtPlain: "Hanako"
Text txtAuto: "auto"
Text txtReadOnly: "fixed"
Text txtDisabled: "disabled"
Text txtPassword: ""
Text txtMulti: "Line1\r\nLine2"
Text txtNoViewState: "changed"
Text txtHidden: "set while hidden"
Text txtRaw: "<b>raw</b>"
Text txtDynamic: "dyn"
Text txtValidated: "123"
Text txtAutoValidated: ""
Text txtClient: ""
```

</details>

<details><summary>TextBox [requestvalidation] markup posted（期待される本文）</summary>

```text
System.Web.HttpRequestValidationException
```

</details>

