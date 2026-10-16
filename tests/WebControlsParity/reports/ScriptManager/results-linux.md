# ScriptManager パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-16 18:07
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 179 | 178 | 1 | 0 |
| ② HTML | 98 | 94 | 4 | 0 |
| ③ シナリオ | 10 | 10 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 199 件中 199 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 27 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 10 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 89 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 8 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 7 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| ポストバックデータの取り込み | 0 | 3 | ✅ |
| リクエスト検証（ValidateRequestMode） | 3 | 0 | ✅ |
| ポストバックイベント | 0 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

| 層 | ケース | 行 | 理由 |
|---|---|---|---|
| api | <code>^ScriptManager\.Api\.ScriptManager\.AjaxFrameworkAssembly$</code> | <code>System\.Web\.Extensions, Version=(4\.0\.0\.0&#124;10\.0\.29\.0),</code> | The AJAX framework&#39;s assembly is System.Web.Extensions itself: its version is the port&#39;s (10.0.29.0), not .NET Framework&#39;s (4.0.0.0). |
| render | <code>^ScriptManager(\[[^\]]+\])?\.ScriptPath=</code> | <code>/System\.Web\.Extensions/(4\.0\.0\.0/4\.8\.[0-9]+\.[0-9]+&#124;10\.0\.29\.0/10\.0\.29\.0)/</code> | ScriptPath&#39;s scripts are looked for in a folder named after System.Web.Extensions&#39; assembly and file versions (ScriptPath/System.Web.Extensions/&lt;version&gt;/&lt;file version&gt;/): the port&#39;s (10.0.29.0), not .NET Framework&#39;s (4.0.0.0 and the installed file version, which differs between .NET Framework machines too). An application that deploys the scripts under ScriptPath needs that folder named after the port&#39;s versions. |

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>ScriptManager.Api.Control.#ctor</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.Adapter</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.ClientID</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.Context</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.Controls</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.DataBind</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.Dispose</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.Disposed</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.Events</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.FindControl</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.Focus</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.HasControls</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.ID</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.Init</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.Load</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.OnInit</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.Page</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.Parent</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.PreRender</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.Render</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.Site</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.SkinID</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.Unload</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.ViewState</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>ScriptManager.Api.Control.Visible</code> | ✅ 一致 |
| <code>ScriptManager.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>ScriptManager.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>ScriptManager.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>ScriptManager.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>ScriptManager.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>ScriptManager.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>ScriptManager.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>ScriptManager.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>ScriptManager.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>ScriptManager.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>ScriptManager.Api.IPostBackDataHandler.LoadPostData</code> | ✅ 一致 |
| <code>ScriptManager.Api.IPostBackDataHandler.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>ScriptManager.Api.IPostBackEventHandler.RaisePostBackEvent</code> | ✅ 一致 |
| <code>ScriptManager.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.#ctor</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.AddHistoryPoint</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.AjaxFrameworkAssembly</code> | ⚠️ 既知の差分 |
| <code>ScriptManager.Api.ScriptManager.AjaxFrameworkMode</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.AllowCustomErrorsRedirect</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.AsyncPostBackError</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.AsyncPostBackErrorMessage</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.AsyncPostBackSourceElementID</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.AsyncPostBackTimeout</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.AuthenticationService</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.ClientNavigateHandler</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.CompositeScript</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.EmptyPageUrl</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.EnableCdn</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.EnableCdnFallback</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.EnableHistory</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.EnablePageMethods</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.EnablePartialRendering</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.EnableScriptGlobalization</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.EnableScriptLocalization</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.EnableSecureHistoryState</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.GetCurrent</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.GetRegisteredArrayDeclarations</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.GetRegisteredClientScriptBlocks</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.GetRegisteredDisposeScripts</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.GetRegisteredExpandoAttributes</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.GetRegisteredHiddenFields</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.GetRegisteredOnSubmitStatements</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.GetRegisteredStartupScripts</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.GetStateString</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.IsDebuggingEnabled</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.IsInAsyncPostBack</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.IsNavigating</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.LoadPostData</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.LoadScriptsBeforeUI</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.Navigate</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.OnAsyncPostBackError</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.OnInit</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.OnPreRender</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.OnResolveCompositeScriptReference</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.OnResolveScriptReference</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.ProfileService</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.RaisePostBackEvent</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.RegisterArrayDeclaration</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.RegisterAsyncPostBackControl</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.RegisterClientScriptBlock</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.RegisterClientScriptInclude</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.RegisterClientScriptResource</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.RegisterDataItem</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.RegisterDispose</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.RegisterExpandoAttribute</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.RegisterExtenderControl``1</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.RegisterHiddenField</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.RegisterNamedClientScriptResource</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.RegisterOnSubmitStatement</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.RegisterPostBackControl</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.RegisterScriptControl``1</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.RegisterScriptDescriptors</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.RegisterStartupScript</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.Render</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.ResolveCompositeScriptReference</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.ResolveScriptReference</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.RoleService</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.ScriptMode</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.ScriptPath</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.ScriptResourceMapping</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.Scripts</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.Services</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.SetFocus</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.SupportsPartialRendering</code> | ✅ 一致 |
| <code>ScriptManager.Api.ScriptManager.Visible</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>ScriptManager.(case list)</code> | ✅ 一致 |
| <code>ScriptManager.(default)</code> | ✅ 一致 |
| <code>ScriptManager.AjaxFrameworkMode=Disabled</code> | ✅ 一致 |
| <code>ScriptManager.AjaxFrameworkMode=Disabled+EnablePartialRendering=false</code> | ✅ 一致 |
| <code>ScriptManager.AjaxFrameworkMode=Enabled</code> | ✅ 一致 |
| <code>ScriptManager.AjaxFrameworkMode=Explicit</code> | ✅ 一致 |
| <code>ScriptManager.AllowCustomErrorsRedirect=false</code> | ✅ 一致 |
| <code>ScriptManager.AllowCustomErrorsRedirect=true</code> | ✅ 一致 |
| <code>ScriptManager.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>ScriptManager.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>ScriptManager.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>ScriptManager.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>ScriptManager.AsyncPostBackErrorMessage=empty</code> | ✅ 一致 |
| <code>ScriptManager.AsyncPostBackErrorMessage=japanese</code> | ✅ 一致 |
| <code>ScriptManager.AsyncPostBackErrorMessage=markup</code> | ✅ 一致 |
| <code>ScriptManager.AsyncPostBackErrorMessage=text</code> | ✅ 一致 |
| <code>ScriptManager.AsyncPostBackTimeout=-1</code> | ✅ 一致 |
| <code>ScriptManager.AsyncPostBackTimeout=0</code> | ✅ 一致 |
| <code>ScriptManager.AsyncPostBackTimeout=3</code> | ✅ 一致 |
| <code>ScriptManager.AsyncPostBackTimeout=3+AsyncPostBackErrorMessage=text</code> | ✅ 一致 |
| <code>ScriptManager.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ScriptManager.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ScriptManager.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ScriptManager.ClientIDMode=Static</code> | ✅ 一致 |
| <code>ScriptManager.ClientNavigateHandler=empty</code> | ✅ 一致 |
| <code>ScriptManager.ClientNavigateHandler=handler</code> | ✅ 一致 |
| <code>ScriptManager.ClientNavigateHandler=japanese</code> | ✅ 一致 |
| <code>ScriptManager.ClientNavigateHandler=markup</code> | ✅ 一致 |
| <code>ScriptManager.ClientNavigateHandler=text</code> | ✅ 一致 |
| <code>ScriptManager.EmptyPageUrl=app-relative</code> | ✅ 一致 |
| <code>ScriptManager.EmptyPageUrl=empty</code> | ✅ 一致 |
| <code>ScriptManager.EmptyPageUrl=japanese</code> | ✅ 一致 |
| <code>ScriptManager.EmptyPageUrl=markup</code> | ✅ 一致 |
| <code>ScriptManager.EmptyPageUrl=text</code> | ✅ 一致 |
| <code>ScriptManager.EnableCdn=false</code> | ✅ 一致 |
| <code>ScriptManager.EnableCdn=true</code> | ✅ 一致 |
| <code>ScriptManager.EnableCdn=true+EnableCdnFallback=false</code> | ✅ 一致 |
| <code>ScriptManager.EnableCdnFallback=false</code> | ✅ 一致 |
| <code>ScriptManager.EnableCdnFallback=true</code> | ✅ 一致 |
| <code>ScriptManager.EnableHistory=false</code> | ✅ 一致 |
| <code>ScriptManager.EnableHistory=true</code> | ✅ 一致 |
| <code>ScriptManager.EnableHistory=true+ClientNavigateHandler=handler</code> | ✅ 一致 |
| <code>ScriptManager.EnableHistory=true+EnableSecureHistoryState=false</code> | ✅ 一致 |
| <code>ScriptManager.EnablePageMethods=false</code> | ✅ 一致 |
| <code>ScriptManager.EnablePageMethods=true</code> | ✅ 一致 |
| <code>ScriptManager.EnablePageMethods=true+EnableViewState=false</code> | ✅ 一致 |
| <code>ScriptManager.EnablePartialRendering=false</code> | ✅ 一致 |
| <code>ScriptManager.EnablePartialRendering=false+EnablePageMethods=true</code> | ✅ 一致 |
| <code>ScriptManager.EnablePartialRendering=false+Visible=false</code> | ✅ 一致 |
| <code>ScriptManager.EnablePartialRendering=true</code> | ✅ 一致 |
| <code>ScriptManager.EnableScriptGlobalization=false</code> | ✅ 一致 |
| <code>ScriptManager.EnableScriptGlobalization=true</code> | ✅ 一致 |
| <code>ScriptManager.EnableScriptLocalization=false</code> | ✅ 一致 |
| <code>ScriptManager.EnableScriptLocalization=true</code> | ✅ 一致 |
| <code>ScriptManager.EnableSecureHistoryState=false</code> | ✅ 一致 |
| <code>ScriptManager.EnableSecureHistoryState=true</code> | ✅ 一致 |
| <code>ScriptManager.EnableTheming=false</code> | ✅ 一致 |
| <code>ScriptManager.EnableTheming=true</code> | ✅ 一致 |
| <code>ScriptManager.EnableViewState=false</code> | ✅ 一致 |
| <code>ScriptManager.EnableViewState=true</code> | ✅ 一致 |
| <code>ScriptManager.ID=empty</code> | ✅ 一致 |
| <code>ScriptManager.ID=japanese</code> | ✅ 一致 |
| <code>ScriptManager.ID=markup</code> | ✅ 一致 |
| <code>ScriptManager.ID=text</code> | ✅ 一致 |
| <code>ScriptManager.LoadScriptsBeforeUI=false</code> | ✅ 一致 |
| <code>ScriptManager.LoadScriptsBeforeUI=true</code> | ✅ 一致 |
| <code>ScriptManager.ScriptMode=Auto</code> | ✅ 一致 |
| <code>ScriptManager.ScriptMode=Debug</code> | ✅ 一致 |
| <code>ScriptManager.ScriptMode=Debug+LoadScriptsBeforeUI=false</code> | ✅ 一致 |
| <code>ScriptManager.ScriptMode=Inherit</code> | ✅ 一致 |
| <code>ScriptManager.ScriptMode=Release</code> | ✅ 一致 |
| <code>ScriptManager.ScriptMode=Release+EnableScriptGlobalization=true</code> | ✅ 一致 |
| <code>ScriptManager.ScriptPath=app-relative</code> | ⚠️ 既知の差分 |
| <code>ScriptManager.ScriptPath=empty</code> | ✅ 一致 |
| <code>ScriptManager.ScriptPath=japanese</code> | ⚠️ 既知の差分 |
| <code>ScriptManager.ScriptPath=markup</code> | ⚠️ 既知の差分 |
| <code>ScriptManager.ScriptPath=text</code> | ⚠️ 既知の差分 |
| <code>ScriptManager.SkinID=empty</code> | ✅ 一致 |
| <code>ScriptManager.SkinID=japanese</code> | ✅ 一致 |
| <code>ScriptManager.SkinID=markup</code> | ✅ 一致 |
| <code>ScriptManager.SkinID=text</code> | ✅ 一致 |
| <code>ScriptManager.SupportsPartialRendering=false</code> | ✅ 一致 |
| <code>ScriptManager.SupportsPartialRendering=true</code> | ✅ 一致 |
| <code>ScriptManager.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>ScriptManager.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>ScriptManager.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>ScriptManager.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>ScriptManager.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>ScriptManager.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>ScriptManager.Visible=false</code> | ✅ 一致 |
| <code>ScriptManager.Visible=true</code> | ✅ 一致 |
| <code>ScriptManager[naming container].(default)</code> | ✅ 一致 |
| <code>ScriptManager[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ScriptManager[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ScriptManager[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ScriptManager[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>ScriptManager[naming container].EnableHistory=true</code> | ✅ 一致 |
| <code>ScriptManager[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| ScriptManager [render][databind][dynamic] first request: the panels, the proxy&#39;s script run, a panel made in code | ✅ 一致 |
| ScriptManager [viewstate][postdata][templates] an asynchronous postback of the main panel: the conditional panel outside left as it was | ✅ 一致 |
| ScriptManager [viewstate][postdata] a button outside triggers the second panel | ✅ 一致 |
| ScriptManager [viewstate][postdata] a postback trigger in the panel: a full postback | ✅ 一致 |
| ScriptManager [viewstate] while a slow request runs: the progress shown | ✅ 一致 |
| ScriptManager [viewstate] after the slow request: the progress hidden | ✅ 一致 |
| ScriptManager [viewstate] the timer started: one tick, then stopped | ✅ 一致 |
| ScriptManager [postbackevent] a history point, then the browser&#39;s back: the script manager&#39;s Navigate | ✅ 一致 |
| ScriptManager [dynamic][viewstate] an asynchronous postback of the panel made in code | ✅ 一致 |
| ScriptManager [visible][viewstate] a hidden panel shown with what was set while hidden | ✅ 一致 |

</details>
