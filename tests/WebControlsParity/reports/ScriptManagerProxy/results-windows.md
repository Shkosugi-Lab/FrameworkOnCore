# ScriptManagerProxy パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 00:46
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 114 | 114 | 0 | 0 |
| ② HTML | 37 | 37 | 0 | 0 |
| ③ シナリオ | 10 | 10 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 121 件中 121 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 9 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 10 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 29 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 9 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>ScriptManagerProxy.Api.Control.#ctor</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.Adapter</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.ClientID</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.Context</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.Controls</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.DataBind</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.Dispose</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.Disposed</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.Events</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.FindControl</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.Focus</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.HasControls</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.ID</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.Init</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.Load</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.OnInit</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.Page</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.Parent</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.PreRender</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.Render</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.Site</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.SkinID</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.Unload</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.ViewState</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.Control.Visible</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.ScriptManagerProxy.#ctor</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.ScriptManagerProxy.AuthenticationService</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.ScriptManagerProxy.CompositeScript</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.ScriptManagerProxy.Navigate</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.ScriptManagerProxy.OnInit</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.ScriptManagerProxy.ProfileService</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.ScriptManagerProxy.RoleService</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.ScriptManagerProxy.Scripts</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.ScriptManagerProxy.Services</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Api.ScriptManagerProxy.Visible</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>ScriptManagerProxy.(case list)</code> | ✅ 一致 |
| <code>ScriptManagerProxy.(default)</code> | ✅ 一致 |
| <code>ScriptManagerProxy.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>ScriptManagerProxy.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>ScriptManagerProxy.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>ScriptManagerProxy.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>ScriptManagerProxy.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ScriptManagerProxy.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ScriptManagerProxy.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ScriptManagerProxy.ClientIDMode=Static</code> | ✅ 一致 |
| <code>ScriptManagerProxy.EnableTheming=false</code> | ✅ 一致 |
| <code>ScriptManagerProxy.EnableTheming=true</code> | ✅ 一致 |
| <code>ScriptManagerProxy.EnableViewState=false</code> | ✅ 一致 |
| <code>ScriptManagerProxy.EnableViewState=true</code> | ✅ 一致 |
| <code>ScriptManagerProxy.ID=empty</code> | ✅ 一致 |
| <code>ScriptManagerProxy.ID=japanese</code> | ✅ 一致 |
| <code>ScriptManagerProxy.ID=markup</code> | ✅ 一致 |
| <code>ScriptManagerProxy.ID=text</code> | ✅ 一致 |
| <code>ScriptManagerProxy.SkinID=empty</code> | ✅ 一致 |
| <code>ScriptManagerProxy.SkinID=japanese</code> | ✅ 一致 |
| <code>ScriptManagerProxy.SkinID=markup</code> | ✅ 一致 |
| <code>ScriptManagerProxy.SkinID=text</code> | ✅ 一致 |
| <code>ScriptManagerProxy.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>ScriptManagerProxy.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>ScriptManagerProxy.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>ScriptManagerProxy.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>ScriptManagerProxy.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>ScriptManagerProxy.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Visible=false</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>ScriptManagerProxy.Visible=true</code> | ✅ 一致 |
| <code>ScriptManagerProxy[naming container].(default)</code> | ✅ 一致 |
| <code>ScriptManagerProxy[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ScriptManagerProxy[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ScriptManagerProxy[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ScriptManagerProxy[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>ScriptManagerProxy[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| ScriptManagerProxy [render][databind][dynamic] first request: the panels, the proxy&#39;s script run, a panel made in code | ✅ 一致 |
| ScriptManagerProxy [viewstate] an asynchronous postback of the main panel: the conditional panel outside left as it was | ✅ 一致 |
| ScriptManagerProxy [viewstate] a button outside triggers the second panel | ✅ 一致 |
| ScriptManagerProxy [viewstate] a postback trigger in the panel: a full postback | ✅ 一致 |
| ScriptManagerProxy [viewstate] while a slow request runs: the progress shown | ✅ 一致 |
| ScriptManagerProxy [viewstate] after the slow request: the progress hidden | ✅ 一致 |
| ScriptManagerProxy [viewstate] the timer started: one tick, then stopped | ✅ 一致 |
| ScriptManagerProxy [viewstate] a history point, then the browser&#39;s back: the script manager&#39;s Navigate | ✅ 一致 |
| ScriptManagerProxy [dynamic][viewstate] an asynchronous postback of the panel made in code | ✅ 一致 |
| ScriptManagerProxy [visible][viewstate] a hidden panel shown with what was set while hidden | ✅ 一致 |

</details>
