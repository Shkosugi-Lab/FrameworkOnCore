# Timer パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-16 16:51
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 118 | 118 | 0 | 0 |
| ② HTML | 46 | 46 | 0 | 0 |
| ③ シナリオ | 10 | 10 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 125 件中 125 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 11 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 10 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 37 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 8 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 7 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| ポストバックイベント | 0 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>Timer.Api.Control.#ctor</code> | ✅ 一致 |
| <code>Timer.Api.Control.Adapter</code> | ✅ 一致 |
| <code>Timer.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>Timer.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>Timer.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>Timer.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>Timer.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>Timer.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>Timer.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>Timer.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>Timer.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>Timer.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>Timer.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>Timer.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>Timer.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>Timer.Api.Control.ClientID</code> | ✅ 一致 |
| <code>Timer.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>Timer.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>Timer.Api.Control.Context</code> | ✅ 一致 |
| <code>Timer.Api.Control.Controls</code> | ✅ 一致 |
| <code>Timer.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>Timer.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>Timer.Api.Control.DataBind</code> | ✅ 一致 |
| <code>Timer.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>Timer.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>Timer.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>Timer.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>Timer.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>Timer.Api.Control.Dispose</code> | ✅ 一致 |
| <code>Timer.Api.Control.Disposed</code> | ✅ 一致 |
| <code>Timer.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>Timer.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>Timer.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>Timer.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>Timer.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>Timer.Api.Control.Events</code> | ✅ 一致 |
| <code>Timer.Api.Control.FindControl</code> | ✅ 一致 |
| <code>Timer.Api.Control.Focus</code> | ✅ 一致 |
| <code>Timer.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>Timer.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>Timer.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>Timer.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>Timer.Api.Control.HasControls</code> | ✅ 一致 |
| <code>Timer.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>Timer.Api.Control.ID</code> | ✅ 一致 |
| <code>Timer.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>Timer.Api.Control.Init</code> | ✅ 一致 |
| <code>Timer.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>Timer.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>Timer.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>Timer.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>Timer.Api.Control.Load</code> | ✅ 一致 |
| <code>Timer.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>Timer.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>Timer.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>Timer.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>Timer.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>Timer.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>Timer.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>Timer.Api.Control.OnInit</code> | ✅ 一致 |
| <code>Timer.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>Timer.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>Timer.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>Timer.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>Timer.Api.Control.Page</code> | ✅ 一致 |
| <code>Timer.Api.Control.Parent</code> | ✅ 一致 |
| <code>Timer.Api.Control.PreRender</code> | ✅ 一致 |
| <code>Timer.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>Timer.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>Timer.Api.Control.Render</code> | ✅ 一致 |
| <code>Timer.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>Timer.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>Timer.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>Timer.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>Timer.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>Timer.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>Timer.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>Timer.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>Timer.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>Timer.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>Timer.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>Timer.Api.Control.Site</code> | ✅ 一致 |
| <code>Timer.Api.Control.SkinID</code> | ✅ 一致 |
| <code>Timer.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>Timer.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>Timer.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>Timer.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>Timer.Api.Control.Unload</code> | ✅ 一致 |
| <code>Timer.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>Timer.Api.Control.ViewState</code> | ✅ 一致 |
| <code>Timer.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>Timer.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>Timer.Api.Control.Visible</code> | ✅ 一致 |
| <code>Timer.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>Timer.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>Timer.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>Timer.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>Timer.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>Timer.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>Timer.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>Timer.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>Timer.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>Timer.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>Timer.Api.IPostBackEventHandler.RaisePostBackEvent</code> | ✅ 一致 |
| <code>Timer.Api.IScriptControl.GetScriptDescriptors</code> | ✅ 一致 |
| <code>Timer.Api.IScriptControl.GetScriptReferences</code> | ✅ 一致 |
| <code>Timer.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>Timer.Api.Timer.#ctor</code> | ✅ 一致 |
| <code>Timer.Api.Timer.Enabled</code> | ✅ 一致 |
| <code>Timer.Api.Timer.GetScriptDescriptors</code> | ✅ 一致 |
| <code>Timer.Api.Timer.GetScriptReferences</code> | ✅ 一致 |
| <code>Timer.Api.Timer.Interval</code> | ✅ 一致 |
| <code>Timer.Api.Timer.OnPreRender</code> | ✅ 一致 |
| <code>Timer.Api.Timer.OnTick</code> | ✅ 一致 |
| <code>Timer.Api.Timer.RaisePostBackEvent</code> | ✅ 一致 |
| <code>Timer.Api.Timer.Render</code> | ✅ 一致 |
| <code>Timer.Api.Timer.Tick</code> | ✅ 一致 |
| <code>Timer.Api.Timer.Visible</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>Timer.(case list)</code> | ✅ 一致 |
| <code>Timer.(default)</code> | ✅ 一致 |
| <code>Timer.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>Timer.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>Timer.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>Timer.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>Timer.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Timer.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Timer.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Timer.ClientIDMode=Static</code> | ✅ 一致 |
| <code>Timer.EnableTheming=false</code> | ✅ 一致 |
| <code>Timer.EnableTheming=true</code> | ✅ 一致 |
| <code>Timer.EnableViewState=false</code> | ✅ 一致 |
| <code>Timer.EnableViewState=true</code> | ✅ 一致 |
| <code>Timer.Enabled=false</code> | ✅ 一致 |
| <code>Timer.Enabled=true</code> | ✅ 一致 |
| <code>Timer.ID=empty</code> | ✅ 一致 |
| <code>Timer.ID=japanese</code> | ✅ 一致 |
| <code>Timer.ID=markup</code> | ✅ 一致 |
| <code>Timer.ID=text</code> | ✅ 一致 |
| <code>Timer.Interval=-1</code> | ✅ 一致 |
| <code>Timer.Interval=0</code> | ✅ 一致 |
| <code>Timer.Interval=1000</code> | ✅ 一致 |
| <code>Timer.Interval=1000+EnableViewState=false</code> | ✅ 一致 |
| <code>Timer.Interval=1000+Enabled=false</code> | ✅ 一致 |
| <code>Timer.Interval=1000+Visible=false</code> | ✅ 一致 |
| <code>Timer.Interval=3</code> | ✅ 一致 |
| <code>Timer.SkinID=empty</code> | ✅ 一致 |
| <code>Timer.SkinID=japanese</code> | ✅ 一致 |
| <code>Timer.SkinID=markup</code> | ✅ 一致 |
| <code>Timer.SkinID=text</code> | ✅ 一致 |
| <code>Timer.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>Timer.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>Timer.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>Timer.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>Timer.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>Timer.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>Timer.Visible=false</code> | ✅ 一致 |
| <code>Timer.Visible=true</code> | ✅ 一致 |
| <code>Timer[naming container].(default)</code> | ✅ 一致 |
| <code>Timer[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Timer[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Timer[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Timer[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>Timer[naming container].ID=text</code> | ✅ 一致 |
| <code>Timer[naming container].Interval=1000</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| Timer [render][databind][dynamic] first request: the panels, the proxy&#39;s script run, a panel made in code | ✅ 一致 |
| Timer [viewstate] an asynchronous postback of the main panel: the conditional panel outside left as it was | ✅ 一致 |
| Timer [viewstate] a button outside triggers the second panel | ✅ 一致 |
| Timer [viewstate] a postback trigger in the panel: a full postback | ✅ 一致 |
| Timer [viewstate] while a slow request runs: the progress shown | ✅ 一致 |
| Timer [viewstate] after the slow request: the progress hidden | ✅ 一致 |
| Timer [postbackevent] the timer started: one tick, then stopped | ✅ 一致 |
| Timer [viewstate] a history point, then the browser&#39;s back: the script manager&#39;s Navigate | ✅ 一致 |
| Timer [dynamic][viewstate] an asynchronous postback of the panel made in code | ✅ 一致 |
| Timer [visible][viewstate] a hidden panel shown with what was set while hidden | ✅ 一致 |

</details>
