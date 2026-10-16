# HtmlInputSubmit テストケース一覧

変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。
このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。

- 正解データの採取: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0
- ① API 層: 133 ケース（143 メンバー）
- ② HTML 層: 69 ケース
- ③ シナリオ層: 5 スナップショット

## 機構マトリクス

型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。

| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |
|---|---|---|---|
| プロパティ → HTML 描画 | すべてのコントロール | 62 | 1 |
| ViewState の保存と復元 | Control | 7 | 2 |
| Visible=false（描画されず状態は保持） | Control.Visible | 3 | 1 |
| 名前付けコンテナと ClientIDMode | Control.ClientIDMode | 6 | 0 |
| 動的に生成したコントロールのライフサイクル | Control | 0 | 3 |
| データバインド（DataBind / <%# %>） | Control.DataBind | 0 | 1 |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | WebControl.Enabled、HtmlControl.Disabled | 4 | 0 |
| ポストバックイベント | IPostBackEventHandler | 0 | 3 |
| CausesValidation / ValidationGroup | CausesValidation プロパティ | 8 | 0 |

該当しない機構: テーマとスキン（SkinID / EnableTheming）、ポストバックデータの取り込み、変更イベント（*Changed、ServerChange）、ReadOnly（ポストされた値を無視）、リクエスト検証（ValidateRequestMode）、コマンドイベントと親へのバブル（CommandName）、送信ボタン／スクリプトによるポストバック（UseSubmitBehavior）、ブラウザ側のクリック処理（OnClientClick）、クロスページポストバック（PostBackUrl）、既定のボタン（Panel の DefaultButton）、AutoPostBack（クライアントスクリプトによるポストバック）、検証対象（サーバー側の検証）、検証対象（ブラウザ側の検証）、双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値）、項目（ListItem の Selected・Enabled・Text・Value と ViewState）、複数選択、繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection）、ポスト値のイベント検証（一覧にない値・無効な項目の拒否）、サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ）、ブラウザ側の検証スクリプト（EnableClientScript）、検証結果のまとめ（ValidationSummary）、ページング、並べ替え、項目の挿入、行・項目の編集（編集・更新・取り消し・削除）、行・項目の選択、行・項目のコマンド（RowCommand・ItemCommand へのバブル）、データキー（DataKeyNames・DataKeys）、データソースコントロールとの連携（DataSourceID）、テンプレート、データソース連携（DataSource・DataTextField など）、クライアントコールバック

## ① API 層

HtmlInputSubmit とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の HtmlInputSubmit インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。

| ケース | 期待値（.NET Framework） |
|---|---|
| <code>Api.Control.#ctor</code> | <code>Control.#ctor = Control# {ClientID=&quot;&quot;; ClientIDMode=Inherit; EnableTheming=true; EnableViewState=true; ID=null; SkinID=&quot;&quot;; UniqueID=null; ValidateRequestMode=In…</code> |
| <code>Api.Control.Adapter</code> | <code>Control.Adapter = null</code> |
| <code>Api.Control.AddParsedSubObject</code> | <code>Control.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>Control.AddParsedSubObject(Object) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableT…</code> |
| <code>Api.Control.AddedControl</code> | <code>Control.AddedControl(Control,Int32) = &quot;ok&quot;</code><br><code>Control.AddedControl(Control,Int32) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; Enable…</code> |
| <code>Api.Control.AppRelativeTemplateSourceDirectory</code> | <code>Control.AppRelativeTemplateSourceDirectory = &quot;&quot;</code><br><code>Control.AppRelativeTemplateSourceDirectory set = &quot;-&gt; System.ArgumentException: The relative virtual path &#39;Abc 123&#39; is not allowed here.&quot;</code> |
| <code>Api.Control.ApplyStyleSheetSkin</code> | <code>Control.ApplyStyleSheetSkin(Page) = &quot;ok&quot;</code><br><code>Control.ApplyStyleSheetSkin(Page) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableTh…</code> |
| <code>Api.Control.BeginRenderTracing</code> | <code>Control.BeginRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.BeginRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.BeginRenderTracing(TextWriter,Object) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=fal…</code> |
| <code>Api.Control.BindingContainer</code> | <code>Control.BindingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Clien…</code> |
| <code>Api.Control.BuildProfileTree</code> | <code>Control.BuildProfileTree(String,Boolean) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.ChildControlsCreated</code> | <code>Control.ChildControlsCreated = &quot;false&quot;</code><br><code>Control.ChildControlsCreated set = &quot;ok&quot;</code> |
| <code>Api.Control.ClearCachedClientID</code> | <code>Control.ClearCachedClientID = &quot;ok&quot;</code><br><code>Control.ClearCachedClientID after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableTheming=…</code> |
| <code>Api.Control.ClearChildControlState</code> | <code>Control.ClearChildControlState = &quot;ok&quot;</code><br><code>Control.ClearChildControlState after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableThemi…</code> |
| <code>Api.Control.ClearChildState</code> | <code>Control.ClearChildState = &quot;ok&quot;</code><br><code>Control.ClearChildState after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableTheming=true…</code> |
| <code>Api.Control.ClearChildViewState</code> | <code>Control.ClearChildViewState = &quot;ok&quot;</code><br><code>Control.ClearChildViewState after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableTheming=…</code> |
| <code>Api.Control.ClearEffectiveClientIDMode</code> | <code>Control.ClearEffectiveClientIDMode = &quot;ok&quot;</code><br><code>Control.ClearEffectiveClientIDMode after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableT…</code> |
| <code>Api.Control.ClientID</code> | <code>Control.ClientID = &quot;inpSubmit&quot;</code> |
| <code>Api.Control.ClientIDMode</code> | <code>Control.ClientIDMode = &quot;ClientIDMode.Inherit&quot;</code><br><code>Control.ClientIDMode set = &quot;ClientIDMode.AutoID&quot;</code> |
| <code>Api.Control.ClientIDSeparator</code> | <code>Control.ClientIDSeparator = &quot;&#39;_&#39;&quot;</code> |
| <code>Api.Control.Context</code> | <code>Control.Context = null</code> |
| <code>Api.Control.Controls</code> | <code>Control.Controls = ControlCollection []</code> |
| <code>Api.Control.CreateChildControls</code> | <code>Control.CreateChildControls = &quot;ok&quot;</code><br><code>Control.CreateChildControls after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableTheming=…</code> |
| <code>Api.Control.CreateControlCollection</code> | <code>Control.CreateControlCollection = ControlCollection []</code><br><code>Control.CreateControlCollection after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableThem…</code> |
| <code>Api.Control.DataBind</code> | <code>Control.DataBind = &quot;ok&quot;</code><br><code>Control.DataBind after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableTheming=true; Enabl…</code><br><code>Control.DataBind(Boolean) = &quot;ok&quot;</code><br><code>Control.DataBind(Boolean) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableTheming=tr…</code> |
| <code>Api.Control.DataBindChildren</code> | <code>Control.DataBindChildren = &quot;ok&quot;</code><br><code>Control.DataBindChildren after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableTheming=tru…</code> |
| <code>Api.Control.DataBinding</code> | <code>Control.DataBinding = &quot;added and removed&quot;</code> |
| <code>Api.Control.DataItemContainer</code> | <code>Control.DataItemContainer = null</code> |
| <code>Api.Control.DataKeysContainer</code> | <code>Control.DataKeysContainer = null</code> |
| <code>Api.Control.DesignMode</code> | <code>Control.DesignMode = &quot;false&quot;</code> |
| <code>Api.Control.Dispose</code> | <code>Control.Dispose = &quot;ok&quot;</code><br><code>Control.Dispose after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableTheming=true; Enable…</code> |
| <code>Api.Control.Disposed</code> | <code>Control.Disposed = &quot;added and removed&quot;</code> |
| <code>Api.Control.EnableTheming</code> | <code>Control.EnableTheming = &quot;true&quot;</code><br><code>Control.EnableTheming set = &quot;true&quot;</code> |
| <code>Api.Control.EnableViewState</code> | <code>Control.EnableViewState = &quot;true&quot;</code><br><code>Control.EnableViewState set = &quot;true&quot;</code> |
| <code>Api.Control.EndRenderTracing</code> | <code>Control.EndRenderTracing(TextWriter,Object) = &quot;ok&quot;</code><br><code>Control.EndRenderTracing(TextWriter,Object) writer:</code><br><code>  &#124; </code><br><code>Control.EndRenderTracing(TextWriter,Object) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false…</code> |
| <code>Api.Control.EnsureChildControls</code> | <code>Control.EnsureChildControls = &quot;ok&quot;</code><br><code>Control.EnsureChildControls after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableTheming=…</code> |
| <code>Api.Control.EnsureID</code> | <code>Control.EnsureID = &quot;ok&quot;</code><br><code>Control.EnsureID after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.Events</code> | <code>Control.Events = &quot;System.ComponentModel.EventHandlerList&quot;</code> |
| <code>Api.Control.FindControl</code> | <code>Control.FindControl(String) = Label#lblTitle {AccessKey=&quot;&quot;; AssociatedControlID=&quot;&quot;; BackColor=Color #00000000 Color [Empty]; BorderColor=Color #00000000 Color […</code><br><code>Control.FindControl(String) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableTheming=…</code><br><code>Control.FindControl(String,Int32) = null</code><br><code>Control.FindControl(String,Int32) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableTh…</code> |
| <code>Api.Control.Focus</code> | <code>Control.Focus = &quot;-&gt; System.InvalidOperationException: A form tag with runat=server must exist on the Page to use SetFocus() or the Focus property.&quot;</code> |
| <code>Api.Control.GetDesignModeState</code> | <code>Control.GetDesignModeState = &quot;[]&quot;</code><br><code>Control.GetDesignModeState after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableTheming=t…</code> |
| <code>Api.Control.GetRouteUrl</code> | <code>Control.GetRouteUrl(Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Object) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(String,Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code><br><code>Control.GetRouteUrl(Routing.RouteValueDictionary) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.GetUniqueIDRelativeTo</code> | <code>Control.GetUniqueIDRelativeTo(Control) = &quot;inpSubmit&quot;</code><br><code>Control.GetUniqueIDRelativeTo(Control) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; Ena…</code> |
| <code>Api.Control.HasChildViewState</code> | <code>Control.HasChildViewState = &quot;false&quot;</code> |
| <code>Api.Control.HasControls</code> | <code>Control.HasControls = &quot;false&quot;</code><br><code>Control.HasControls after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableTheming=true; En…</code> |
| <code>Api.Control.HasEvents</code> | <code>Control.HasEvents = &quot;false&quot;</code><br><code>Control.HasEvents after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableTheming=true; Enab…</code> |
| <code>Api.Control.ID</code> | <code>Control.ID = &quot;inpSubmit&quot;</code><br><code>Control.ID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.IdSeparator</code> | <code>Control.IdSeparator = &quot;&#39;$&#39;&quot;</code> |
| <code>Api.Control.Init</code> | <code>Control.Init = &quot;added and removed&quot;</code> |
| <code>Api.Control.IsChildControlStateCleared</code> | <code>Control.IsChildControlStateCleared = &quot;false&quot;</code> |
| <code>Api.Control.IsLiteralContent</code> | <code>Control.IsLiteralContent = &quot;false&quot;</code><br><code>Control.IsLiteralContent after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableTheming=tru…</code> |
| <code>Api.Control.IsTrackingViewState</code> | <code>Control.IsTrackingViewState = &quot;false&quot;</code> |
| <code>Api.Control.IsViewStateEnabled</code> | <code>Control.IsViewStateEnabled = &quot;true&quot;</code> |
| <code>Api.Control.Load</code> | <code>Control.Load = &quot;added and removed&quot;</code> |
| <code>Api.Control.LoadControlState</code> | <code>Control.LoadControlState(Object) = &quot;ok&quot;</code><br><code>Control.LoadControlState(Object) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableThe…</code> |
| <code>Api.Control.LoadViewState</code> | <code>Control.LoadViewState(Object) = &quot;-&gt; System.InvalidCastException: Unable to cast object of type &#39;System.String&#39; to type &#39;System.Collections.ArrayList&#39;.&quot;</code> |
| <code>Api.Control.LoadViewStateByID</code> | <code>Control.LoadViewStateByID = &quot;false&quot;</code> |
| <code>Api.Control.MapPathSecure</code> | <code>Control.MapPathSecure(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.NamingContainer</code> | <code>Control.NamingContainer = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code> |
| <code>Api.Control.OnBubbleEvent</code> | <code>Control.OnBubbleEvent(Object,EventArgs) = &quot;false&quot;</code><br><code>Control.OnBubbleEvent(Object,EventArgs) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; En…</code> |
| <code>Api.Control.OnDataBinding</code> | <code>Control.OnDataBinding(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnDataBinding(EventArgs) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableThe…</code> |
| <code>Api.Control.OnInit</code> | <code>Control.OnInit(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnInit(EventArgs) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableTheming=tr…</code> |
| <code>Api.Control.OnLoad</code> | <code>Control.OnLoad(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnLoad(EventArgs) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableTheming=tr…</code> |
| <code>Api.Control.OnPreRender</code> | <code>Control.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnPreRender(EventArgs) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableThemi…</code> |
| <code>Api.Control.OnUnload</code> | <code>Control.OnUnload(EventArgs) = &quot;ok&quot;</code><br><code>Control.OnUnload(EventArgs) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableTheming=…</code> |
| <code>Api.Control.OpenFile</code> | <code>Control.OpenFile(String) = &quot;-&gt; System.NullReferenceException: Object reference not set to an instance of an object.&quot;</code> |
| <code>Api.Control.Page</code> | <code>Control.Page = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=&quot;&quot;; …</code><br><code>Control.Page set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; ClientTarget=…</code> |
| <code>Api.Control.Parent</code> | <code>Control.Parent = HtmlForm#form1 {Action=&quot;&quot;; ClientID=&quot;form1&quot;; ClientIDMode=Inherit; DefaultButton=&quot;&quot;; DefaultFocus=&quot;&quot;; Disabled=false; EnableTheming=true; Enabl…</code> |
| <code>Api.Control.PreRender</code> | <code>Control.PreRender = &quot;added and removed&quot;</code> |
| <code>Api.Control.RaiseBubbleEvent</code> | <code>Control.RaiseBubbleEvent(Object,EventArgs) = &quot;ok&quot;</code><br><code>Control.RaiseBubbleEvent(Object,EventArgs) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false;…</code> |
| <code>Api.Control.RemovedControl</code> | <code>Control.RemovedControl(Control) = &quot;ok&quot;</code><br><code>Control.RemovedControl(Control) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableThem…</code> |
| <code>Api.Control.Render</code> | <code>Control.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.Render(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code><br><code>Control.Render(HtmlTextWriter) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableThemi…</code> |
| <code>Api.Control.RenderChildren</code> | <code>Control.RenderChildren(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderChildren(HtmlTextWriter) writer:</code><br><code>  &#124; </code><br><code>Control.RenderChildren(HtmlTextWriter) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; Ena…</code> |
| <code>Api.Control.RenderControl</code> | <code>Control.RenderControl(HtmlTextWriter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code><br><code>Control.RenderControl(HtmlTextWriter) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; Enab…</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) = &quot;ok&quot;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) writer:</code><br><code>  &#124; &lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code><br><code>Control.RenderControl(HtmlTextWriter,Adapters.ControlAdapter) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inher…</code> |
| <code>Api.Control.RenderingCompatibility</code> | <code>Control.RenderingCompatibility = &quot;System.Version 4.0&quot;</code><br><code>Control.RenderingCompatibility set = &quot;System.Version 4.0&quot;</code> |
| <code>Api.Control.ResolveAdapter</code> | <code>Control.ResolveAdapter = null</code><br><code>Control.ResolveAdapter after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableTheming=true;…</code> |
| <code>Api.Control.ResolveClientUrl</code> | <code>Control.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveClientUrl(String) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableThe…</code> |
| <code>Api.Control.ResolveUrl</code> | <code>Control.ResolveUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>Control.ResolveUrl(String) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableTheming=t…</code> |
| <code>Api.Control.SaveControlState</code> | <code>Control.SaveControlState = null</code><br><code>Control.SaveControlState after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableTheming=tru…</code> |
| <code>Api.Control.SaveViewState</code> | <code>Control.SaveViewState = null</code><br><code>Control.SaveViewState after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableTheming=true; …</code> |
| <code>Api.Control.SetDesignModeState</code> | <code>Control.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>Control.SetDesignModeState(IDictionary) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; En…</code> |
| <code>Api.Control.SetRenderMethodDelegate</code> | <code>Control.SetRenderMethodDelegate(RenderMethod) = &quot;ok&quot;</code><br><code>Control.SetRenderMethodDelegate(RenderMethod) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=fal…</code> |
| <code>Api.Control.SetTraceData</code> | <code>Control.SetTraceData(Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; Enable…</code><br><code>Control.SetTraceData(Object,Object,Object) = &quot;ok&quot;</code><br><code>Control.SetTraceData(Object,Object,Object) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false;…</code> |
| <code>Api.Control.Site</code> | <code>Control.Site = null</code><br><code>Control.Site set = null</code> |
| <code>Api.Control.SkinID</code> | <code>Control.SkinID = &quot;&quot;</code><br><code>Control.SkinID set = &quot;Abc 123&quot;</code> |
| <code>Api.Control.TemplateControl</code> | <code>Control.TemplateControl = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Client…</code><br><code>Control.TemplateControl set = Page#__Page {AppRelativeVirtualPath=null; Buffer=!HttpException; ClientID=&quot;__Page&quot;; ClientIDMode=Inherit; ClientQueryString=&quot;&quot;; Cl…</code> |
| <code>Api.Control.TemplateSourceDirectory</code> | <code>Control.TemplateSourceDirectory = &quot;&quot;</code> |
| <code>Api.Control.TrackViewState</code> | <code>Control.TrackViewState = &quot;ok&quot;</code><br><code>Control.TrackViewState after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; EnableTheming=true;…</code> |
| <code>Api.Control.UniqueID</code> | <code>Control.UniqueID = &quot;inpSubmit&quot;</code> |
| <code>Api.Control.Unload</code> | <code>Control.Unload = &quot;added and removed&quot;</code> |
| <code>Api.Control.ValidateRequestMode</code> | <code>Control.ValidateRequestMode = &quot;ValidateRequestMode.Inherit&quot;</code><br><code>Control.ValidateRequestMode set = &quot;ValidateRequestMode.Disabled&quot;</code> |
| <code>Api.Control.ViewState</code> | <code>Control.ViewState = StateBag [type=&quot;submit&quot;, value=&quot;Go&quot;]</code> |
| <code>Api.Control.ViewStateIgnoresCase</code> | <code>Control.ViewStateIgnoresCase = &quot;true&quot;</code> |
| <code>Api.Control.ViewStateMode</code> | <code>Control.ViewStateMode = &quot;ViewStateMode.Inherit&quot;</code><br><code>Control.ViewStateMode set = &quot;ViewStateMode.Enabled&quot;</code> |
| <code>Api.Control.Visible</code> | <code>Control.Visible = &quot;true&quot;</code><br><code>Control.Visible set = &quot;true&quot;</code> |
| <code>Api.HtmlControls.HtmlControl.#ctor</code> | <code>HtmlControls.HtmlControl.#ctor = &quot;-&gt; System.MemberAccessException: Cannot create an instance of System.Web.UI.HtmlControls.HtmlControl because it is an abstract…</code><br><code>HtmlControls.HtmlControl.#ctor(String) = &quot;-&gt; System.MemberAccessException: Cannot create an instance of System.Web.UI.HtmlControls.HtmlControl because it is an …</code> |
| <code>Api.HtmlControls.HtmlControl.Attributes</code> | <code>HtmlControls.HtmlControl.Attributes = AttributeCollection [type=&quot;submit&quot;, value=&quot;Go&quot;]</code> |
| <code>Api.HtmlControls.HtmlControl.CreateControlCollection</code> | <code>HtmlControls.HtmlControl.CreateControlCollection = ControlCollection []</code><br><code>HtmlControls.HtmlControl.CreateControlCollection after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=…</code> |
| <code>Api.HtmlControls.HtmlControl.Disabled</code> | <code>HtmlControls.HtmlControl.Disabled = &quot;false&quot;</code><br><code>HtmlControls.HtmlControl.Disabled set = &quot;true&quot;</code> |
| <code>Api.HtmlControls.HtmlControl.GetAttribute</code> | <code>HtmlControls.HtmlControl.GetAttribute(String) = null</code><br><code>HtmlControls.HtmlControl.GetAttribute(String) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=fal…</code> |
| <code>Api.HtmlControls.HtmlControl.Render</code> | <code>HtmlControls.HtmlControl.Render(HtmlTextWriter) = &quot;ok&quot;</code><br><code>HtmlControls.HtmlControl.Render(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code><br><code>HtmlControls.HtmlControl.Render(HtmlTextWriter) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=f…</code> |
| <code>Api.HtmlControls.HtmlControl.RenderAttributes</code> | <code>HtmlControls.HtmlControl.RenderAttributes(HtmlTextWriter) = &quot;ok&quot;</code><br><code>HtmlControls.HtmlControl.RenderAttributes(HtmlTextWriter) writer:</code><br><code>  &#124;  name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /</code><br><code>HtmlControls.HtmlControl.RenderAttributes(HtmlTextWriter) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; …</code> |
| <code>Api.HtmlControls.HtmlControl.RenderBeginTag</code> | <code>HtmlControls.HtmlControl.RenderBeginTag(HtmlTextWriter) = &quot;ok&quot;</code><br><code>HtmlControls.HtmlControl.RenderBeginTag(HtmlTextWriter) writer:</code><br><code>  &#124; &lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code><br><code>HtmlControls.HtmlControl.RenderBeginTag(HtmlTextWriter) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Di…</code> |
| <code>Api.HtmlControls.HtmlControl.SetAttribute</code> | <code>HtmlControls.HtmlControl.SetAttribute(String,String) = &quot;ok&quot;</code><br><code>HtmlControls.HtmlControl.SetAttribute(String,String) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disab…</code> |
| <code>Api.HtmlControls.HtmlControl.Style</code> | <code>HtmlControls.HtmlControl.Style = CssStyleCollection {Count=Int32 0; Value=null}</code> |
| <code>Api.HtmlControls.HtmlControl.TagName</code> | <code>HtmlControls.HtmlControl.TagName = &quot;input&quot;</code> |
| <code>Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | <code>HtmlControls.HtmlControl.ViewStateIgnoresCase = &quot;true&quot;</code> |
| <code>Api.HtmlControls.HtmlInputButton.#ctor</code> | <code>HtmlControls.HtmlInputButton.#ctor = HtmlInputButton# {CausesValidation=true; ClientID=&quot;&quot;; ClientIDMode=Inherit; Disabled=false; EnableTheming=true; EnableViewS…</code><br><code>HtmlControls.HtmlInputButton.#ctor(String) = HtmlInputButton# {CausesValidation=true; ClientID=&quot;&quot;; ClientIDMode=Inherit; Disabled=false; EnableTheming=true; Ena…</code> |
| <code>Api.HtmlControls.HtmlInputButton.CausesValidation</code> | <code>HtmlControls.HtmlInputButton.CausesValidation = &quot;true&quot;</code><br><code>HtmlControls.HtmlInputButton.CausesValidation set = &quot;true&quot;</code> |
| <code>Api.HtmlControls.HtmlInputButton.OnPreRender</code> | <code>HtmlControls.HtmlInputButton.OnPreRender(EventArgs) = &quot;ok&quot;</code><br><code>HtmlControls.HtmlInputButton.OnPreRender(EventArgs) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabl…</code> |
| <code>Api.HtmlControls.HtmlInputButton.OnServerClick</code> | <code>HtmlControls.HtmlInputButton.OnServerClick(EventArgs) = &quot;ok&quot;</code><br><code>HtmlControls.HtmlInputButton.OnServerClick(EventArgs) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disa…</code> |
| <code>Api.HtmlControls.HtmlInputButton.RaisePostBackEvent</code> | <code>HtmlControls.HtmlInputButton.RaisePostBackEvent(String) = &quot;ok&quot;</code><br><code>HtmlControls.HtmlInputButton.RaisePostBackEvent(String) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Di…</code> |
| <code>Api.HtmlControls.HtmlInputButton.RenderAttributes</code> | <code>HtmlControls.HtmlInputButton.RenderAttributes(HtmlTextWriter) = &quot;ok&quot;</code><br><code>HtmlControls.HtmlInputButton.RenderAttributes(HtmlTextWriter) writer:</code><br><code>  &#124;  name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /</code><br><code>HtmlControls.HtmlInputButton.RenderAttributes(HtmlTextWriter) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inher…</code> |
| <code>Api.HtmlControls.HtmlInputButton.ServerClick</code> | <code>HtmlControls.HtmlInputButton.ServerClick = &quot;added and removed&quot;</code> |
| <code>Api.HtmlControls.HtmlInputButton.ValidationGroup</code> | <code>HtmlControls.HtmlInputButton.ValidationGroup = &quot;&quot;</code><br><code>HtmlControls.HtmlInputButton.ValidationGroup set = &quot;Abc 123&quot;</code> |
| <code>Api.HtmlControls.HtmlInputControl.#ctor</code> | <code>HtmlControls.HtmlInputControl.#ctor(String) = &quot;-&gt; System.MemberAccessException: Cannot create an instance of System.Web.UI.HtmlControls.HtmlInputControl because…</code> |
| <code>Api.HtmlControls.HtmlInputControl.Name</code> | <code>HtmlControls.HtmlInputControl.Name = &quot;inpSubmit&quot;</code><br><code>HtmlControls.HtmlInputControl.Name set = &quot;inpSubmit&quot;</code> |
| <code>Api.HtmlControls.HtmlInputControl.RenderAttributes</code> | <code>HtmlControls.HtmlInputControl.RenderAttributes(HtmlTextWriter) = &quot;ok&quot;</code><br><code>HtmlControls.HtmlInputControl.RenderAttributes(HtmlTextWriter) writer:</code><br><code>  &#124;  name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /</code><br><code>HtmlControls.HtmlInputControl.RenderAttributes(HtmlTextWriter) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inhe…</code> |
| <code>Api.HtmlControls.HtmlInputControl.Type</code> | <code>HtmlControls.HtmlInputControl.Type = &quot;submit&quot;</code> |
| <code>Api.HtmlControls.HtmlInputControl.Value</code> | <code>HtmlControls.HtmlInputControl.Value = &quot;Go&quot;</code><br><code>HtmlControls.HtmlInputControl.Value set = &quot;Abc 123&quot;</code> |
| <code>Api.HtmlControls.HtmlInputSubmit.#ctor</code> | <code>HtmlControls.HtmlInputSubmit.#ctor = HtmlInputSubmit# {CausesValidation=true; ClientID=&quot;&quot;; ClientIDMode=Inherit; Disabled=false; EnableTheming=true; EnableViewS…</code><br><code>HtmlControls.HtmlInputSubmit.#ctor(String) = HtmlInputSubmit# {CausesValidation=true; ClientID=&quot;&quot;; ClientIDMode=Inherit; Disabled=false; EnableTheming=true; Ena…</code> |
| <code>Api.IAttributeAccessor.GetAttribute</code> | <code>IAttributeAccessor.GetAttribute(String) = null</code><br><code>IAttributeAccessor.GetAttribute(String) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false; En…</code> |
| <code>Api.IAttributeAccessor.SetAttribute</code> | <code>IAttributeAccessor.SetAttribute(String,String) = &quot;ok&quot;</code><br><code>IAttributeAccessor.SetAttribute(String,String) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=fa…</code> |
| <code>Api.IControlBuilderAccessor.ControlBuilder</code> | <code>IControlBuilderAccessor.ControlBuilder = null</code> |
| <code>Api.IControlDesignerAccessor.GetDesignModeState</code> | <code>IControlDesignerAccessor.GetDesignModeState = &quot;[]&quot;</code><br><code>IControlDesignerAccessor.GetDesignModeState after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false…</code> |
| <code>Api.IControlDesignerAccessor.SetDesignModeState</code> | <code>IControlDesignerAccessor.SetDesignModeState(IDictionary) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetDesignModeState(IDictionary) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; D…</code> |
| <code>Api.IControlDesignerAccessor.SetOwnerControl</code> | <code>IControlDesignerAccessor.SetOwnerControl(Control) = &quot;ok&quot;</code><br><code>IControlDesignerAccessor.SetOwnerControl(Control) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled…</code> |
| <code>Api.IControlDesignerAccessor.UserData</code> | <code>IControlDesignerAccessor.UserData = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.DataBindings</code> | <code>IDataBindingsAccessor.DataBindings = &quot;[]&quot;</code> |
| <code>Api.IDataBindingsAccessor.HasDataBindings</code> | <code>IDataBindingsAccessor.HasDataBindings = &quot;false&quot;</code> |
| <code>Api.IExpressionsAccessor.Expressions</code> | <code>IExpressionsAccessor.Expressions = &quot;[]&quot;</code> |
| <code>Api.IExpressionsAccessor.HasExpressions</code> | <code>IExpressionsAccessor.HasExpressions = &quot;false&quot;</code> |
| <code>Api.IParserAccessor.AddParsedSubObject</code> | <code>IParserAccessor.AddParsedSubObject(Object) = &quot;ok&quot;</code><br><code>IParserAccessor.AddParsedSubObject(Object) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=false;…</code> |
| <code>Api.IPostBackEventHandler.RaisePostBackEvent</code> | <code>IPostBackEventHandler.RaisePostBackEvent(String) = &quot;ok&quot;</code><br><code>IPostBackEventHandler.RaisePostBackEvent(String) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=…</code> |
| <code>Api.IUrlResolutionService.ResolveClientUrl</code> | <code>IUrlResolutionService.ResolveClientUrl(String) = &quot;~/Default.aspx&quot;</code><br><code>IUrlResolutionService.ResolveClientUrl(String) after = HtmlInputSubmit#inpSubmit {CausesValidation=true; ClientID=&quot;inpSubmit&quot;; ClientIDMode=Inherit; Disabled=fa…</code> |

## ② HTML 層

プローブサイトの RenderProbe.aspx で、1 ケース 1 リクエスト（ページライフサイクル全体）で描画したページ全体を比較します。下表はコンポーネント部分（div#probe）、設定時の例外、保存された ViewState の要約です。

| ケース | 設定 | 期待される描画 | 設定時の例外 |
|---|---|---|---|
| <code>.(default)</code> | <code>(nothing set)</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.AppRelativeTemplateSourceDirectory=text</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;probe&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;probe&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=markup</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=japanese</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;日本語テキスト&quot;; -&gt; System.ArgumentException: The relative virtual path &#39;日本語テキスト&#39; is not allowed here.</code> |
| <code>.AppRelativeTemplateSourceDirectory=empty</code> | <code>c.AppRelativeTemplateSourceDirectory = &quot;&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.CausesValidation=true</code> | <code>c.CausesValidation = true;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.CausesValidation=false</code> | <code>c.CausesValidation = false;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.ClientIDMode=Inherit</code> | <code>c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.ClientIDMode=AutoID</code> | <code>c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.ClientIDMode=Predictable</code> | <code>c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.ClientIDMode=Static</code> | <code>c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.Disabled=true</code> | <code>c.Disabled = true;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; disabled=&quot;disabled&quot; /&gt;</code> |  |
| <code>.Disabled=false</code> | <code>c.Disabled = false;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.EnableTheming=true</code> | <code>c.EnableTheming = true;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.EnableTheming=false</code> | <code>c.EnableTheming = false;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.EnableViewState=true</code> | <code>c.EnableViewState = true;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.EnableViewState=false</code> | <code>c.EnableViewState = false;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.ID=text</code> | <code>c.ID = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;probe&quot; type=&quot;submit&quot; id=&quot;probe&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.ID=markup</code> | <code>c.ID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input name=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; type=&quot;submit&quot; id=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.ID=japanese</code> | <code>c.ID = &quot;日本語テキスト&quot;;</code> | <code>&lt;input name=&quot;日本語テキスト&quot; type=&quot;submit&quot; id=&quot;日本語テキスト&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.ID=empty</code> | <code>c.ID = &quot;&quot;;</code> | <code>&lt;input name=&quot;ctl02&quot; type=&quot;submit&quot; id=&quot;ctl02&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.Name=text</code> | <code>c.Name = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.Name=markup</code> | <code>c.Name = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.Name=japanese</code> | <code>c.Name = &quot;日本語テキスト&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.Name=empty</code> | <code>c.Name = &quot;&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.Name=group</code> | <code>c.Name = &quot;group&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.SkinID=text</code> | <code>c.SkinID = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.SkinID=markup</code> | <code>c.SkinID = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.SkinID=japanese</code> | <code>c.SkinID = &quot;日本語テキスト&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.SkinID=empty</code> | <code>c.SkinID = &quot;&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.ValidateRequestMode=Inherit</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Inherit;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.ValidateRequestMode=Disabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Disabled;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.ValidateRequestMode=Enabled</code> | <code>c.ValidateRequestMode = ValidateRequestMode.Enabled;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.ValidationGroup=text</code> | <code>c.ValidationGroup = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; ValidationGroup=&quot;probe&quot; /&gt;</code> |  |
| <code>.ValidationGroup=markup</code> | <code>c.ValidationGroup = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; ValidationGroup=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; /&gt;</code> |  |
| <code>.ValidationGroup=japanese</code> | <code>c.ValidationGroup = &quot;日本語テキスト&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; ValidationGroup=&quot;日本語テキスト&quot; /&gt;</code> |  |
| <code>.ValidationGroup=empty</code> | <code>c.ValidationGroup = &quot;&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; ValidationGroup=&quot;&quot; /&gt;</code> |  |
| <code>.Value=text</code> | <code>c.Value = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;probe&quot; /&gt;</code> |  |
| <code>.Value=markup</code> | <code>c.Value = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; /&gt;</code> |  |
| <code>.Value=japanese</code> | <code>c.Value = &quot;日本語テキスト&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;日本語テキスト&quot; /&gt;</code> |  |
| <code>.Value=empty</code> | <code>c.Value = &quot;&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; /&gt;</code> |  |
| <code>.ViewStateMode=Inherit</code> | <code>c.ViewStateMode = ViewStateMode.Inherit;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.ViewStateMode=Enabled</code> | <code>c.ViewStateMode = ViewStateMode.Enabled;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.ViewStateMode=Disabled</code> | <code>c.ViewStateMode = ViewStateMode.Disabled;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.Visible=true</code> | <code>c.Visible = true;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;</code> |  |
| <code>.Visible=false</code> | <code>c.Visible = false;</code> | <code></code> |  |
| <code>.Attributes[data-probe]=text</code> | <code>c.Attributes[data-probe] = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; data-probe=&quot;probe&quot; /&gt;</code> |  |
| <code>.Attributes[onfocus]=text</code> | <code>c.Attributes[onfocus] = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; onfocus=&quot;probe&quot; /&gt;</code> |  |
| <code>.Attributes[data-probe]=markup</code> | <code>c.Attributes[data-probe] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; data-probe=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; /&gt;</code> |  |
| <code>.Attributes[onfocus]=markup</code> | <code>c.Attributes[onfocus] = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; onfocus=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; /&gt;</code> |  |
| <code>.Style[color]=red</code> | <code>c.Style[color] = &quot;red&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; style=&quot;color:red;&quot; /&gt;</code> |  |
| <code>.Style[margin-left]=red</code> | <code>c.Style[margin-left] = &quot;red&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; style=&quot;margin-left:red;&quot; /&gt;</code> |  |
| <code>.Style[color]=1em</code> | <code>c.Style[color] = &quot;1em&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; style=&quot;color:1em;&quot; /&gt;</code> |  |
| <code>.Style[margin-left]=1em</code> | <code>c.Style[margin-left] = &quot;1em&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; style=&quot;margin-left:1em;&quot; /&gt;</code> |  |
| <code>.Value=markup+Disabled=true</code> | <code>c.Value = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; c.Disabled = true;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; disabled=&quot;disabled&quot; /&gt;</code> |  |
| <code>.Value=text+Visible=false</code> | <code>c.Value = &quot;probe&quot;; c.Visible = false;</code> | <code></code> |  |
| <code>.Value=markup+EnableViewState=false</code> | <code>c.Value = &quot;&lt;b&gt;\&quot;&amp;&#39;&lt;/b&gt;&quot;; c.EnableViewState = false;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;&lt;b&gt;&quot;&amp;&#39;&lt;/b&gt;&quot; /&gt;</code> |  |
| <code>.Name=group+Value=text</code> | <code>c.Name = &quot;group&quot;; c.Value = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;probe&quot; /&gt;</code> |  |
| <code>.CausesValidation=false+ValidationGroup=text</code> | <code>c.CausesValidation = false; c.ValidationGroup = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; ValidationGroup=&quot;probe&quot; /&gt;</code> |  |
| <code>.Disabled=true+CausesValidation=false</code> | <code>c.Disabled = true; c.CausesValidation = false;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; disabled=&quot;disabled&quot; /&gt;</code> |  |
| <code>.Disabled=true+Attributes[data-probe]=text</code> | <code>c.Disabled = true; c.Attributes[data-probe] = &quot;probe&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; disabled=&quot;disabled&quot; data-probe=&quot;probe&quot; /&gt;</code> |  |
| <code>.Attributes[class]=text+Style[color]=red</code> | <code>c.Attributes[class] = &quot;probe&quot;; c.Style[color] = &quot;red&quot;;</code> | <code>&lt;input name=&quot;inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; class=&quot;probe&quot; style=&quot;color:red;&quot; /&gt;</code> |  |
| <code>.Visible=false+EnableViewState=false</code> | <code>c.Visible = false; c.EnableViewState = false;</code> | <code></code> |  |
| <code>[naming container].(default)</code> | <code>[naming container] (nothing set)</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input name=&quot;box$inpSubmit&quot; type=&quot;submit&quot; id=&quot;box_inpSubmit&quot; value=&quot;Go&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=AutoID</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.AutoID;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input name=&quot;box$inpSubmit&quot; type=&quot;submit&quot; id=&quot;box_inpSubmit&quot; value=&quot;Go&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Static</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Static;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input name=&quot;box$inpSubmit&quot; type=&quot;submit&quot; id=&quot;inpSubmit&quot; value=&quot;Go&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Predictable</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Predictable;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input name=&quot;box$inpSubmit&quot; type=&quot;submit&quot; id=&quot;box_inpSubmit&quot; value=&quot;Go&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ClientIDMode=Inherit</code> | <code>[naming container] c.ClientIDMode = ClientIDMode.Inherit;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input name=&quot;box$inpSubmit&quot; type=&quot;submit&quot; id=&quot;box_inpSubmit&quot; value=&quot;Go&quot; /&gt;↵&lt;/div&gt;</code> |  |
| <code>[naming container].ID=text</code> | <code>[naming container] c.ID = &quot;probe&quot;;</code> | <code>&lt;div id=&quot;box&quot;&gt;↵	&lt;input name=&quot;box$probe&quot; type=&quot;submit&quot; id=&quot;box_probe&quot; value=&quot;Go&quot; /&gt;↵&lt;/div&gt;</code> |  |

## ③ シナリオ層

ブラウザ（Playwright）で HtmlInputSubmitScenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。

| # | 操作 | 対象 | 値 |
|---|---|---|---|
| 1 | goto | /HtmlScenario.aspx | <code></code> |
| 2 | snapshot |  | **HtmlInputSubmit [render][databind][dynamic] first request: plain, bound, set in code, made in code** |
| 3 | fill | inpText | <code>posted text</code> |
| 4 | fill | inpPassword | <code>secret</code> |
| 5 | setvalue | inpHidden | <code>changed</code> |
| 6 | check | inpCheck | <code></code> |
| 7 | check | inpRadioLarge | <code></code> |
| 8 | fill | inpEmail | <code>a@example.com</code> |
| 9 | fill | txaNote | <code>posted note</code> |
| 10 | select | selSize | <code>L</code> |
| 11 | selectmany | selMulti | <code>S,L</code> |
| 12 | select | selBound | <code>B2</code> |
| 13 | upload | inpFile | <code>probe.txt:file content</code> |
| 14 | fill | inpDynamicText | <code>dynamic posted</code> |
| 15 | fill | inpDynamicPassword | <code>dynamic secret</code> |
| 16 | setvalue | inpDynamicHidden | <code>dynamic changed</code> |
| 17 | check | inpDynamicCheck | <code></code> |
| 18 | check | inpDynamicRadio | <code></code> |
| 19 | fill | inpDynamicEmail | <code>b@example.com</code> |
| 20 | fill | txaDynamic | <code>dynamic note</code> |
| 21 | select | selDynamic | <code>2</code> |
| 22 | upload | inpDynamicFile | <code>dynamic.txt:dynamic content</code> |
| 23 | click | inpSubmit | <code></code> |
| 24 | snapshot |  | **HtmlInputSubmit [viewstate][dynamic][postbackevent] every input posted by a submit input** |
| 25 | click | inpSubmit | <code></code> |
| 26 | snapshot |  | **HtmlInputSubmit [postbackevent] its server click** |
| 27 | click | inpDynamicSubmit | <code></code> |
| 28 | snapshot |  | **HtmlInputSubmit [postbackevent][dynamic] a dynamic one&#39;s server click** |
| 29 | click | btnShow | <code></code> |
| 30 | snapshot |  | **HtmlInputSubmit [visible][viewstate] hidden ones shown with what was set while hidden** |

<details><summary>HtmlInputSubmit [render][databind][dynamic] first request: plain, bound, set in code, made in code（期待される本文）</summary>

```text
Generic: Plain bound set in code set in code, no view state
Anchor: Go to the target Submit anchor Validate g1 by an anchor Disabled anchor bound
Image:
Plain cell set in code
bound
row added in code
Inputs:
Small
Medium
Large
Small
Medium
Large
Pen
Bag
Cup <&>
Buttons: HTML button bound Validate g1 by a button
Server validation (g1):
Client validation (g2):
(choose)
A
Dynamic: dynamicDynamic anchor
dynamic cell
One
Two
Dynamic button
Request 1: IsPostBack=False
State elmPage lang="en"
State hdPage Title="HTML controls scenario (set in code)" Controls=5
State ttlPage Text="HTML controls scenario (set in code)"
State metaBound Name="description" Content="bound" Scheme="set in code"
State lnkBound Href="~/Images/probe.css"
State form1 title="bound"
State genPlain InnerText="Plain" title=""
State genBound InnerText="bound" title="bound"
State genCode InnerText="set in code" title=""
State genNoViewState InnerText="set in code, no view state" title=""
State genHidden InnerText="set while hidden" title="" (hidden)
State ancTarget HRef="~/ButtonTarget.aspx" InnerText="Go to the target"
State ancBound HRef="~/ButtonTarget.aspx?id=7" InnerText="bound"
State ancHidden HRef="" InnerText="Hidden anchor (set while hidden)" (hidden)
State imgPlain Src="~/Images/probe.png" Alt="Logo"
State imgBound Src="~/Images/probe.png" Alt="bound"
State imgCode Src="~/Images/probe.png" Alt="set in code"
State imgHidden Src="~/Images/probe.png" Alt="set while hidden" (hidden)
State ifrPlain Src="~/Images/probe.png"
State areaPlain Href="~/ButtonTarget.aspx" Alt="bound"
State audPlain Src="~/Images/probe.png"
State vidPlain Src="" Poster="~/Images/probe.png"
State srcPlain Src="~/Images/probe.png"
State trkPlain Src="~/Images/probe.png"
State embPlain Src="~/Images/probe.png"
State tblPlain Rows=2 [Plain cell|set in code|bound|Hidden cell (set while hidden)]
State trPlain Cells=2 Title=""
State trBound Cells=2 Title="bound"
State tdPlain InnerText="Plain cell" title=""
State tdCode InnerText="set in code" title=""
State tdBound InnerText="bound" title=""
State tdHiddenCell InnerText="Hidden cell (set while hidden)" title="" (hidden)
State tblCode Rows=1 [row added in code]
State tblHidden Rows=0 [] (hidden)
State inpText Value=""
State inpTextBound Value="bound"
State inpTextNoViewState Value="set in code, no view state"
State inpTextDisabled Value="disabled"
State inpTextHidden Value="set while hidden" (hidden)
State inpPassword Value=""
State inpHidden Value="initial"
State inpHiddenBound Value="bound"
State inpCheck Checked=False
State inpCheckBound Checked=True
State inpRadioSmall Checked=False Value="inpRadioSmall"
State inpRadioLarge Checked=False Value="inpRadioLarge"
State inpEmail Value=""
State inpEmailBound Value="7"
State txaNote Value=""
State txaBound Value="bound"
State selSize SelectedIndex=1 Value="M" Selected=[M] Items=3
State selMulti SelectedIndex=-1 Value="" Selected=[] Items=3
State selBound SelectedIndex=0 Value="P1" Selected=[P1] Items=3
State inpFile PostedFile=null Value=""
State btnHtml InnerText="HTML button" title=""
State btnHtmlBound InnerText="bound" title=""
State inpButton Value="Input button"
State inpSubmit Value="Input submit"
State inpSubmitBound Value="bound"
State inpReset Value="Reset"
State inpResetBound Value="bound"
State inpImage Alt="Input image"
State inpImageBound Alt="bound"
State inpClient Value=""
State inpClientPassword Value=""
State inpClientEmail Value=""
State txaClient Value=""
State selClient SelectedIndex=0 Value="" Selected=[] Items=2
State inpClientFile PostedFile=null Value=""
State genDynamic InnerText="dynamic" title=""
State ancDynamic HRef="" InnerText="Dynamic anchor"
State imgDynamic Src="~/Images/probe.png" Alt="dynamic"
State tblDynamic Rows=1 [dynamic cell]
State inpDynamicText Value="dynamic"
State inpDynamicPassword Value="dynamic"
State inpDynamicHidden Value="dynamic"
State inpDynamicCheck Checked=False
State inpDynamicRadio Checked=False Value="dynamic"
State inpDynamicEmail Value="dynamic"
State txaDynamic Value="dynamic"
State selDynamic SelectedIndex=0 Value="1" Selected=[1] Items=2
State inpDynamicFile PostedFile=null Value=""
State btnDynamic InnerText="Dynamic button" title=""
State inpDynamicButton Value="Dynamic input button"
State inpDynamicSubmit Value="Dynamic submit"
State inpDynamicReset Value="Dynamic reset"
State inpDynamicImage Alt="Dynamic image"
State ifrDynamic Src="~/Images/probe.png"
State areaDynamic Href="~/ButtonTarget.aspx" Alt=""
State audDynamic Src="~/Images/probe.png"
State vidDynamic Src="" Poster="~/Images/probe.png"
State embDynamic Src="~/Images/probe.png"
State ttlDynamic Text="Dynamic title (set in code)"
State metaDynamic Name="keywords" Content="dynamic" Scheme=""
State lnkDynamic Href="~/Images/probe.css"
```

</details>

<details><summary>HtmlInputSubmit [viewstate][dynamic][postbackevent] every input posted by a submit input（期待される本文）</summary>

```text
Generic: Plain bound set in code
Anchor: Go to the target Submit anchor Validate g1 by an anchor Disabled anchor bound
Image:
Plain cell set in code
bound
Inputs:
Small
Medium
Large
Small
Medium
Large
Pen
Bag
Cup <&>
Buttons: HTML button bound Validate g1 by a button
Server validation (g1):
Client validation (g2):
(choose)
A
Dynamic: dynamicDynamic anchor
dynamic cell
One
Two
Dynamic button
Request 2: IsPostBack=True
ServerChange inpText Value="posted text"
ServerChange inpTextNoViewState Value="set in code, no view state"
ServerChange inpPassword Value="secret"
ServerChange inpHidden Value="changed"
ServerChange inpCheck Checked=True
ServerChange inpEmail Value="a@example.com"
ServerChange txaNote Value="posted note"
ServerChange selSize SelectedIndex=2 Value="L" Selected=[L] Items=3
ServerChange selMulti SelectedIndex=0 Value="S" Selected=[S,L] Items=3
ServerChange selBound SelectedIndex=1 Value="B2" Selected=[B2] Items=3
ServerChange inpDynamicText Value="dynamic posted"
ServerChange inpDynamicPassword Value="dynamic secret"
ServerChange inpDynamicHidden Value="dynamic changed"
ServerChange inpDynamicCheck Checked=True
ServerChange inpDynamicEmail Value="b@example.com"
ServerChange txaDynamic Value="dynamic note"
ServerChange selDynamic SelectedIndex=1 Value="2" Selected=[2] Items=2
ServerChange inpRadioLarge Checked=True Value="inpRadioLarge"
ServerChange inpDynamicRadio Checked=True Value="dynamic"
ServerClick inpSubmit
IsValid: (not validated)
State elmPage lang="en"
State hdPage Title="HTML controls scenario" Controls=5
State ttlPage Text="HTML controls scenario"
State metaBound Name="description" Content="bound" Scheme="set in code"
State lnkBound Href="~/Images/probe.css"
State form1 title="bound"
State genPlain InnerText="Plain" title=""
State genBound InnerText="bound" title="bound"
State genCode InnerText="set in code" title=""
State genNoViewState InnerText="" title=""
State genHidden InnerText="set while hidden" title="" (hidden)
State ancTarget HRef="~/ButtonTarget.aspx" InnerText="Go to the target"
State ancBound HRef="~/ButtonTarget.aspx?id=7" InnerText="bound"
State ancHidden HRef="" InnerText="Hidden anchor (set while hidden)" (hidden)
State imgPlain Src="~/Images/probe.png" Alt="Logo"
State imgBound Src="~/Images/probe.png" Alt="bound"
State imgCode Src="~/Images/probe.png" Alt="set in code"
State imgHidden Src="~/Images/probe.png" Alt="set while hidden" (hidden)
State ifrPlain Src="~/Images/probe.png"
State areaPlain Href="~/ButtonTarget.aspx" Alt="bound"
State audPlain Src="~/Images/probe.png"
State vidPlain Src="" Poster="~/Images/probe.png"
State srcPlain Src="~/Images/probe.png"
State trkPlain Src="~/Images/probe.png"
State embPlain Src="~/Images/probe.png"
State tblPlain Rows=2 [Plain cell|set in code|bound|Hidden cell (set while hidden)]
State trPlain Cells=2 Title=""
State trBound Cells=2 Title="bound"
State tdPlain InnerText="Plain cell" title=""
State tdCode InnerText="set in code" title=""
State tdBound InnerText="bound" title=""
State tdHiddenCell InnerText="Hidden cell (set while hidden)" title="" (hidden)
State tblCode Rows=0 []
State tblHidden Rows=0 [] (hidden)
State inpText Value="posted text"
State inpTextBound Value="bound"
State inpTextNoViewState Value="set in code, no view state"
State inpTextDisabled Value="disabled"
State inpTextHidden Value="set while hidden" (hidden)
State inpPassword Value="secret"
State inpHidden Value="changed"
State inpHiddenBound Value="bound"
State inpCheck Checked=True
State inpCheckBound Checked=True
State inpRadioSmall Checked=False Value="inpRadioSmall"
State inpRadioLarge Checked=True Value="inpRadioLarge"
State inpEmail Value="a@example.com"
State inpEmailBound Value="7"
State txaNote Value="posted note"
State txaBound Value="bound"
State selSize SelectedIndex=2 Value="L" Selected=[L] Items=3
State selMulti SelectedIndex=0 Value="S" Selected=[S,L] Items=3
State selBound SelectedIndex=1 Value="B2" Selected=[B2] Items=3
State inpFile PostedFile="probe.txt" 12 bytes "file content" Value="probe.txt"
State btnHtml InnerText="HTML button" title=""
State btnHtmlBound InnerText="bound" title=""
State inpButton Value="Input button"
State inpSubmit Value="Input submit"
State inpSubmitBound Value="bound"
State inpReset Value="Reset"
State inpResetBound Value="bound"
State inpImage Alt="Input image"
State inpImageBound Alt="bound"
State inpClient Value=""
State inpClientPassword Value=""
State inpClientEmail Value=""
State txaClient Value=""
State selClient SelectedIndex=0 Value="" Selected=[] Items=2
State inpClientFile PostedFile="" 0 bytes "" Value=""
State genDynamic InnerText="dynamic" title=""
State ancDynamic HRef="" InnerText="Dynamic anchor"
State imgDynamic Src="~/Images/probe.png" Alt="dynamic"
State tblDynamic Rows=1 [dynamic cell]
State inpDynamicText Value="dynamic posted"
State inpDynamicPassword Value="dynamic secret"
State inpDynamicHidden Value="dynamic changed"
State inpDynamicCheck Checked=True
State inpDynamicRadio Checked=True Value="dynamic"
State inpDynamicEmail Value="b@example.com"
State txaDynamic Value="dynamic note"
State selDynamic SelectedIndex=1 Value="2" Selected=[2] Items=2
State inpDynamicFile PostedFile="dynamic.txt" 15 bytes "dynamic content" Value="dynamic.txt"
State btnDynamic InnerText="Dynamic button" title=""
State inpDynamicButton Value="Dynamic input button"
State inpDynamicSubmit Value="Dynamic submit"
State inpDynamicReset Value="Dynamic reset"
State inpDynamicImage Alt="Dynamic image"
State ifrDynamic Src="~/Images/probe.png"
State areaDynamic Href="~/ButtonTarget.aspx" Alt=""
State audDynamic Src="~/Images/probe.png"
State vidDynamic Src="" Poster="~/Images/probe.png"
State embDynamic Src="~/Images/probe.png"
State ttlDynamic Text="Dynamic title"
State metaDynamic Name="keywords" Content="dynamic" Scheme=""
State lnkDynamic Href="~/Images/probe.css"
```

</details>

<details><summary>HtmlInputSubmit [postbackevent] its server click（期待される本文）</summary>

```text
Generic: Plain bound set in code
Anchor: Go to the target Submit anchor Validate g1 by an anchor Disabled anchor bound
Image:
Plain cell set in code
bound
Inputs:
Small
Medium
Large
Small
Medium
Large
Pen
Bag
Cup <&>
Buttons: HTML button bound Validate g1 by a button
Server validation (g1):
Client validation (g2):
(choose)
A
Dynamic: dynamicDynamic anchor
dynamic cell
One
Two
Dynamic button
Request 3: IsPostBack=True
ServerChange inpTextNoViewState Value="set in code, no view state"
ServerClick inpSubmit
IsValid: (not validated)
State elmPage lang="en"
State hdPage Title="HTML controls scenario" Controls=5
State ttlPage Text="HTML controls scenario"
State metaBound Name="description" Content="bound" Scheme="set in code"
State lnkBound Href="~/Images/probe.css"
State form1 title="bound"
State genPlain InnerText="Plain" title=""
State genBound InnerText="bound" title="bound"
State genCode InnerText="set in code" title=""
State genNoViewState InnerText="" title=""
State genHidden InnerText="set while hidden" title="" (hidden)
State ancTarget HRef="~/ButtonTarget.aspx" InnerText="Go to the target"
State ancBound HRef="~/ButtonTarget.aspx?id=7" InnerText="bound"
State ancHidden HRef="" InnerText="Hidden anchor (set while hidden)" (hidden)
State imgPlain Src="~/Images/probe.png" Alt="Logo"
State imgBound Src="~/Images/probe.png" Alt="bound"
State imgCode Src="~/Images/probe.png" Alt="set in code"
State imgHidden Src="~/Images/probe.png" Alt="set while hidden" (hidden)
State ifrPlain Src="~/Images/probe.png"
State areaPlain Href="~/ButtonTarget.aspx" Alt="bound"
State audPlain Src="~/Images/probe.png"
State vidPlain Src="" Poster="~/Images/probe.png"
State srcPlain Src="~/Images/probe.png"
State trkPlain Src="~/Images/probe.png"
State embPlain Src="~/Images/probe.png"
State tblPlain Rows=2 [Plain cell|set in code|bound|Hidden cell (set while hidden)]
State trPlain Cells=2 Title=""
State trBound Cells=2 Title="bound"
State tdPlain InnerText="Plain cell" title=""
State tdCode InnerText="set in code" title=""
State tdBound InnerText="bound" title=""
State tdHiddenCell InnerText="Hidden cell (set while hidden)" title="" (hidden)
State tblCode Rows=0 []
State tblHidden Rows=0 [] (hidden)
State inpText Value="posted text"
State inpTextBound Value="bound"
State inpTextNoViewState Value="set in code, no view state"
State inpTextDisabled Value="disabled"
State inpTextHidden Value="set while hidden" (hidden)
State inpPassword Value=""
State inpHidden Value="changed"
State inpHiddenBound Value="bound"
State inpCheck Checked=True
State inpCheckBound Checked=True
State inpRadioSmall Checked=False Value="inpRadioSmall"
State inpRadioLarge Checked=True Value="inpRadioLarge"
State inpEmail Value="a@example.com"
State inpEmailBound Value="7"
State txaNote Value="posted note"
State txaBound Value="bound"
State selSize SelectedIndex=2 Value="L" Selected=[L] Items=3
State selMulti SelectedIndex=0 Value="S" Selected=[S,L] Items=3
State selBound SelectedIndex=1 Value="B2" Selected=[B2] Items=3
State inpFile PostedFile="" 0 bytes "" Value=""
State btnHtml InnerText="HTML button" title=""
State btnHtmlBound InnerText="bound" title=""
State inpButton Value="Input button"
State inpSubmit Value="Input submit"
State inpSubmitBound Value="bound"
State inpReset Value="Reset"
State inpResetBound Value="bound"
State inpImage Alt="Input image"
State inpImageBound Alt="bound"
State inpClient Value=""
State inpClientPassword Value=""
State inpClientEmail Value=""
State txaClient Value=""
State selClient SelectedIndex=0 Value="" Selected=[] Items=2
State inpClientFile PostedFile="" 0 bytes "" Value=""
State genDynamic InnerText="dynamic" title=""
State ancDynamic HRef="" InnerText="Dynamic anchor"
State imgDynamic Src="~/Images/probe.png" Alt="dynamic"
State tblDynamic Rows=1 [dynamic cell]
State inpDynamicText Value="dynamic posted"
State inpDynamicPassword Value=""
State inpDynamicHidden Value="dynamic changed"
State inpDynamicCheck Checked=True
State inpDynamicRadio Checked=True Value="dynamic"
State inpDynamicEmail Value="b@example.com"
State txaDynamic Value="dynamic note"
State selDynamic SelectedIndex=1 Value="2" Selected=[2] Items=2
State inpDynamicFile PostedFile="" 0 bytes "" Value=""
State btnDynamic InnerText="Dynamic button" title=""
State inpDynamicButton Value="Dynamic input button"
State inpDynamicSubmit Value="Dynamic submit"
State inpDynamicReset Value="Dynamic reset"
State inpDynamicImage Alt="Dynamic image"
State ifrDynamic Src="~/Images/probe.png"
State areaDynamic Href="~/ButtonTarget.aspx" Alt=""
State audDynamic Src="~/Images/probe.png"
State vidDynamic Src="" Poster="~/Images/probe.png"
State embDynamic Src="~/Images/probe.png"
State ttlDynamic Text="Dynamic title"
State metaDynamic Name="keywords" Content="dynamic" Scheme=""
State lnkDynamic Href="~/Images/probe.css"
```

</details>

<details><summary>HtmlInputSubmit [postbackevent][dynamic] a dynamic one&#39;s server click（期待される本文）</summary>

```text
Generic: Plain bound set in code
Anchor: Go to the target Submit anchor Validate g1 by an anchor Disabled anchor bound
Image:
Plain cell set in code
bound
Inputs:
Small
Medium
Large
Small
Medium
Large
Pen
Bag
Cup <&>
Buttons: HTML button bound Validate g1 by a button
Server validation (g1):
Client validation (g2):
(choose)
A
Dynamic: dynamicDynamic anchor
dynamic cell
One
Two
Dynamic button
Request 4: IsPostBack=True
ServerChange inpTextNoViewState Value="set in code, no view state"
ServerClick inpDynamicSubmit
IsValid: (not validated)
State elmPage lang="en"
State hdPage Title="HTML controls scenario" Controls=5
State ttlPage Text="HTML controls scenario"
State metaBound Name="description" Content="bound" Scheme="set in code"
State lnkBound Href="~/Images/probe.css"
State form1 title="bound"
State genPlain InnerText="Plain" title=""
State genBound InnerText="bound" title="bound"
State genCode InnerText="set in code" title=""
State genNoViewState InnerText="" title=""
State genHidden InnerText="set while hidden" title="" (hidden)
State ancTarget HRef="~/ButtonTarget.aspx" InnerText="Go to the target"
State ancBound HRef="~/ButtonTarget.aspx?id=7" InnerText="bound"
State ancHidden HRef="" InnerText="Hidden anchor (set while hidden)" (hidden)
State imgPlain Src="~/Images/probe.png" Alt="Logo"
State imgBound Src="~/Images/probe.png" Alt="bound"
State imgCode Src="~/Images/probe.png" Alt="set in code"
State imgHidden Src="~/Images/probe.png" Alt="set while hidden" (hidden)
State ifrPlain Src="~/Images/probe.png"
State areaPlain Href="~/ButtonTarget.aspx" Alt="bound"
State audPlain Src="~/Images/probe.png"
State vidPlain Src="" Poster="~/Images/probe.png"
State srcPlain Src="~/Images/probe.png"
State trkPlain Src="~/Images/probe.png"
State embPlain Src="~/Images/probe.png"
State tblPlain Rows=2 [Plain cell|set in code|bound|Hidden cell (set while hidden)]
State trPlain Cells=2 Title=""
State trBound Cells=2 Title="bound"
State tdPlain InnerText="Plain cell" title=""
State tdCode InnerText="set in code" title=""
State tdBound InnerText="bound" title=""
State tdHiddenCell InnerText="Hidden cell (set while hidden)" title="" (hidden)
State tblCode Rows=0 []
State tblHidden Rows=0 [] (hidden)
State inpText Value="posted text"
State inpTextBound Value="bound"
State inpTextNoViewState Value="set in code, no view state"
State inpTextDisabled Value="disabled"
State inpTextHidden Value="set while hidden" (hidden)
State inpPassword Value=""
State inpHidden Value="changed"
State inpHiddenBound Value="bound"
State inpCheck Checked=True
State inpCheckBound Checked=True
State inpRadioSmall Checked=False Value="inpRadioSmall"
State inpRadioLarge Checked=True Value="inpRadioLarge"
State inpEmail Value="a@example.com"
State inpEmailBound Value="7"
State txaNote Value="posted note"
State txaBound Value="bound"
State selSize SelectedIndex=2 Value="L" Selected=[L] Items=3
State selMulti SelectedIndex=0 Value="S" Selected=[S,L] Items=3
State selBound SelectedIndex=1 Value="B2" Selected=[B2] Items=3
State inpFile PostedFile="" 0 bytes "" Value=""
State btnHtml InnerText="HTML button" title=""
State btnHtmlBound InnerText="bound" title=""
State inpButton Value="Input button"
State inpSubmit Value="Input submit"
State inpSubmitBound Value="bound"
State inpReset Value="Reset"
State inpResetBound Value="bound"
State inpImage Alt="Input image"
State inpImageBound Alt="bound"
State inpClient Value=""
State inpClientPassword Value=""
State inpClientEmail Value=""
State txaClient Value=""
State selClient SelectedIndex=0 Value="" Selected=[] Items=2
State inpClientFile PostedFile="" 0 bytes "" Value=""
State genDynamic InnerText="dynamic" title=""
State ancDynamic HRef="" InnerText="Dynamic anchor"
State imgDynamic Src="~/Images/probe.png" Alt="dynamic"
State tblDynamic Rows=1 [dynamic cell]
State inpDynamicText Value="dynamic posted"
State inpDynamicPassword Value=""
State inpDynamicHidden Value="dynamic changed"
State inpDynamicCheck Checked=True
State inpDynamicRadio Checked=True Value="dynamic"
State inpDynamicEmail Value="b@example.com"
State txaDynamic Value="dynamic note"
State selDynamic SelectedIndex=1 Value="2" Selected=[2] Items=2
State inpDynamicFile PostedFile="" 0 bytes "" Value=""
State btnDynamic InnerText="Dynamic button" title=""
State inpDynamicButton Value="Dynamic input button"
State inpDynamicSubmit Value="Dynamic submit"
State inpDynamicReset Value="Dynamic reset"
State inpDynamicImage Alt="Dynamic image"
State ifrDynamic Src="~/Images/probe.png"
State areaDynamic Href="~/ButtonTarget.aspx" Alt=""
State audDynamic Src="~/Images/probe.png"
State vidDynamic Src="" Poster="~/Images/probe.png"
State embDynamic Src="~/Images/probe.png"
State ttlDynamic Text="Dynamic title"
State metaDynamic Name="keywords" Content="dynamic" Scheme=""
State lnkDynamic Href="~/Images/probe.css"
```

</details>

<details><summary>HtmlInputSubmit [visible][viewstate] hidden ones shown with what was set while hidden（期待される本文）</summary>

```text
Generic: Plain bound set in code
set while hidden
Anchor: Go to the target Submit anchor Validate g1 by an anchor Disabled anchor bound Hidden anchor (set while hidden)
Image:
Plain cell set in code
bound Hidden cell (set while hidden)
Inputs:
Small
Medium
Large
Small
Medium
Large
Pen
Bag
Cup <&>
Buttons: HTML button bound Validate g1 by a button
Server validation (g1):
Client validation (g2):
(choose)
A
Dynamic: dynamicDynamic anchor
dynamic cell
One
Two
Dynamic button
Request 5: IsPostBack=True
ServerChange inpTextNoViewState Value="set in code, no view state"
Click btnShow
IsValid: (not validated)
State elmPage lang="en"
State hdPage Title="HTML controls scenario" Controls=5
State ttlPage Text="HTML controls scenario"
State metaBound Name="description" Content="bound" Scheme="set in code"
State lnkBound Href="~/Images/probe.css"
State form1 title="bound"
State genPlain InnerText="Plain" title=""
State genBound InnerText="bound" title="bound"
State genCode InnerText="set in code" title=""
State genNoViewState InnerText="" title=""
State genHidden InnerText="set while hidden" title=""
State ancTarget HRef="~/ButtonTarget.aspx" InnerText="Go to the target"
State ancBound HRef="~/ButtonTarget.aspx?id=7" InnerText="bound"
State ancHidden HRef="" InnerText="Hidden anchor (set while hidden)"
State imgPlain Src="~/Images/probe.png" Alt="Logo"
State imgBound Src="~/Images/probe.png" Alt="bound"
State imgCode Src="~/Images/probe.png" Alt="set in code"
State imgHidden Src="~/Images/probe.png" Alt="set while hidden"
State ifrPlain Src="~/Images/probe.png"
State areaPlain Href="~/ButtonTarget.aspx" Alt="bound"
State audPlain Src="~/Images/probe.png"
State vidPlain Src="" Poster="~/Images/probe.png"
State srcPlain Src="~/Images/probe.png"
State trkPlain Src="~/Images/probe.png"
State embPlain Src="~/Images/probe.png"
State tblPlain Rows=2 [Plain cell|set in code|bound|Hidden cell (set while hidden)]
State trPlain Cells=2 Title=""
State trBound Cells=2 Title="bound"
State tdPlain InnerText="Plain cell" title=""
State tdCode InnerText="set in code" title=""
State tdBound InnerText="bound" title=""
State tdHiddenCell InnerText="Hidden cell (set while hidden)" title=""
State tblCode Rows=0 []
State tblHidden Rows=0 []
State inpText Value="posted text"
State inpTextBound Value="bound"
State inpTextNoViewState Value="set in code, no view state"
State inpTextDisabled Value="disabled"
State inpTextHidden Value="set while hidden"
State inpPassword Value=""
State inpHidden Value="changed"
State inpHiddenBound Value="bound"
State inpCheck Checked=True
State inpCheckBound Checked=True
State inpRadioSmall Checked=False Value="inpRadioSmall"
State inpRadioLarge Checked=True Value="inpRadioLarge"
State inpEmail Value="a@example.com"
State inpEmailBound Value="7"
State txaNote Value="posted note"
State txaBound Value="bound"
State selSize SelectedIndex=2 Value="L" Selected=[L] Items=3
State selMulti SelectedIndex=0 Value="S" Selected=[S,L] Items=3
State selBound SelectedIndex=1 Value="B2" Selected=[B2] Items=3
State inpFile PostedFile="" 0 bytes "" Value=""
State btnHtml InnerText="HTML button" title=""
State btnHtmlBound InnerText="bound" title=""
State inpButton Value="Input button"
State inpSubmit Value="Input submit"
State inpSubmitBound Value="bound"
State inpReset Value="Reset"
State inpResetBound Value="bound"
State inpImage Alt="Input image"
State inpImageBound Alt="bound"
State inpClient Value=""
State inpClientPassword Value=""
State inpClientEmail Value=""
State txaClient Value=""
State selClient SelectedIndex=0 Value="" Selected=[] Items=2
State inpClientFile PostedFile="" 0 bytes "" Value=""
State genDynamic InnerText="dynamic" title=""
State ancDynamic HRef="" InnerText="Dynamic anchor"
State imgDynamic Src="~/Images/probe.png" Alt="dynamic"
State tblDynamic Rows=1 [dynamic cell]
State inpDynamicText Value="dynamic posted"
State inpDynamicPassword Value=""
State inpDynamicHidden Value="dynamic changed"
State inpDynamicCheck Checked=True
State inpDynamicRadio Checked=True Value="dynamic"
State inpDynamicEmail Value="b@example.com"
State txaDynamic Value="dynamic note"
State selDynamic SelectedIndex=1 Value="2" Selected=[2] Items=2
State inpDynamicFile PostedFile="" 0 bytes "" Value=""
State btnDynamic InnerText="Dynamic button" title=""
State inpDynamicButton Value="Dynamic input button"
State inpDynamicSubmit Value="Dynamic submit"
State inpDynamicReset Value="Dynamic reset"
State inpDynamicImage Alt="Dynamic image"
State ifrDynamic Src="~/Images/probe.png"
State areaDynamic Href="~/ButtonTarget.aspx" Alt=""
State audDynamic Src="~/Images/probe.png"
State vidDynamic Src="" Poster="~/Images/probe.png"
State embDynamic Src="~/Images/probe.png"
State ttlDynamic Text="Dynamic title"
State metaDynamic Name="keywords" Content="dynamic" Scheme=""
State lnkDynamic Href="~/Images/probe.css"
```

</details>

