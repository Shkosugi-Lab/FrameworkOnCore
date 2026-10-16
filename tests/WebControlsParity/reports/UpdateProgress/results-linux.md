# UpdateProgress パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-15 14:43
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 121 | 121 | 0 | 0 |
| ② HTML | 57 | 57 | 0 | 0 |
| ③ シナリオ | 10 | 10 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 128 件中 128 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 12 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 10 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 48 | 2 | ✅ |
| ViewState の保存と復元 | 6 | 8 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 7 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| テンプレート | 0 | 3 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>UpdateProgress.Api.Control.#ctor</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.Adapter</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.ClientID</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.Context</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.Controls</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.DataBind</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.Dispose</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.Disposed</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.Events</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.FindControl</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.Focus</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.HasControls</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.ID</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.Init</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.Load</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.OnInit</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.Page</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.Parent</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.PreRender</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.Render</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.Site</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.SkinID</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.Unload</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.ViewState</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>UpdateProgress.Api.Control.Visible</code> | ✅ 一致 |
| <code>UpdateProgress.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>UpdateProgress.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>UpdateProgress.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>UpdateProgress.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>UpdateProgress.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>UpdateProgress.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>UpdateProgress.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>UpdateProgress.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>UpdateProgress.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>UpdateProgress.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>UpdateProgress.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>UpdateProgress.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>UpdateProgress.Api.IScriptControl.GetScriptDescriptors</code> | ✅ 一致 |
| <code>UpdateProgress.Api.IScriptControl.GetScriptReferences</code> | ✅ 一致 |
| <code>UpdateProgress.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>UpdateProgress.Api.UpdateProgress.#ctor</code> | ✅ 一致 |
| <code>UpdateProgress.Api.UpdateProgress.AssociatedUpdatePanelID</code> | ✅ 一致 |
| <code>UpdateProgress.Api.UpdateProgress.Attributes</code> | ✅ 一致 |
| <code>UpdateProgress.Api.UpdateProgress.Controls</code> | ✅ 一致 |
| <code>UpdateProgress.Api.UpdateProgress.CreateChildControls</code> | ✅ 一致 |
| <code>UpdateProgress.Api.UpdateProgress.DataBind</code> | ✅ 一致 |
| <code>UpdateProgress.Api.UpdateProgress.DisplayAfter</code> | ✅ 一致 |
| <code>UpdateProgress.Api.UpdateProgress.DynamicLayout</code> | ✅ 一致 |
| <code>UpdateProgress.Api.UpdateProgress.GetScriptDescriptors</code> | ✅ 一致 |
| <code>UpdateProgress.Api.UpdateProgress.GetScriptReferences</code> | ✅ 一致 |
| <code>UpdateProgress.Api.UpdateProgress.OnPreRender</code> | ✅ 一致 |
| <code>UpdateProgress.Api.UpdateProgress.ProgressTemplate</code> | ✅ 一致 |
| <code>UpdateProgress.Api.UpdateProgress.Render</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>UpdateProgress.(case list)</code> | ✅ 一致 |
| <code>UpdateProgress.(default)</code> | ✅ 一致 |
| <code>UpdateProgress.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>UpdateProgress.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>UpdateProgress.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>UpdateProgress.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>UpdateProgress.AssociatedUpdatePanelID=empty</code> | ✅ 一致 |
| <code>UpdateProgress.AssociatedUpdatePanelID=japanese</code> | ✅ 一致 |
| <code>UpdateProgress.AssociatedUpdatePanelID=markup</code> | ✅ 一致 |
| <code>UpdateProgress.AssociatedUpdatePanelID=missing</code> | ✅ 一致 |
| <code>UpdateProgress.AssociatedUpdatePanelID=target</code> | ✅ 一致 |
| <code>UpdateProgress.AssociatedUpdatePanelID=target+DisplayAfter=0</code> | ✅ 一致 |
| <code>UpdateProgress.AssociatedUpdatePanelID=target+DynamicLayout=false</code> | ✅ 一致 |
| <code>UpdateProgress.AssociatedUpdatePanelID=text</code> | ✅ 一致 |
| <code>UpdateProgress.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>UpdateProgress.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>UpdateProgress.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>UpdateProgress.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>UpdateProgress.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>UpdateProgress.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>UpdateProgress.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>UpdateProgress.ClientIDMode=Static</code> | ✅ 一致 |
| <code>UpdateProgress.DisplayAfter=-1</code> | ✅ 一致 |
| <code>UpdateProgress.DisplayAfter=0</code> | ✅ 一致 |
| <code>UpdateProgress.DisplayAfter=3</code> | ✅ 一致 |
| <code>UpdateProgress.DisplayAfter=3+EnableViewState=false</code> | ✅ 一致 |
| <code>UpdateProgress.DisplayAfter=3+Visible=false</code> | ✅ 一致 |
| <code>UpdateProgress.DynamicLayout=false</code> | ✅ 一致 |
| <code>UpdateProgress.DynamicLayout=false+DisplayAfter=3</code> | ✅ 一致 |
| <code>UpdateProgress.DynamicLayout=true</code> | ✅ 一致 |
| <code>UpdateProgress.EnableTheming=false</code> | ✅ 一致 |
| <code>UpdateProgress.EnableTheming=true</code> | ✅ 一致 |
| <code>UpdateProgress.EnableViewState=false</code> | ✅ 一致 |
| <code>UpdateProgress.EnableViewState=true</code> | ✅ 一致 |
| <code>UpdateProgress.ID=empty</code> | ✅ 一致 |
| <code>UpdateProgress.ID=japanese</code> | ✅ 一致 |
| <code>UpdateProgress.ID=markup</code> | ✅ 一致 |
| <code>UpdateProgress.ID=text</code> | ✅ 一致 |
| <code>UpdateProgress.SkinID=empty</code> | ✅ 一致 |
| <code>UpdateProgress.SkinID=japanese</code> | ✅ 一致 |
| <code>UpdateProgress.SkinID=markup</code> | ✅ 一致 |
| <code>UpdateProgress.SkinID=text</code> | ✅ 一致 |
| <code>UpdateProgress.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>UpdateProgress.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>UpdateProgress.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>UpdateProgress.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>UpdateProgress.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>UpdateProgress.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>UpdateProgress.Visible=false</code> | ✅ 一致 |
| <code>UpdateProgress.Visible=true</code> | ✅ 一致 |
| <code>UpdateProgress[naming container].(default)</code> | ✅ 一致 |
| <code>UpdateProgress[naming container].AssociatedUpdatePanelID=target</code> | ✅ 一致 |
| <code>UpdateProgress[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>UpdateProgress[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>UpdateProgress[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>UpdateProgress[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>UpdateProgress[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| UpdateProgress [render][databind][dynamic][templates] first request: the panels, the proxy&#39;s script run, a panel made in code | ✅ 一致 |
| UpdateProgress [viewstate] an asynchronous postback of the main panel: the conditional panel outside left as it was | ✅ 一致 |
| UpdateProgress [viewstate] a button outside triggers the second panel | ✅ 一致 |
| UpdateProgress [viewstate] a postback trigger in the panel: a full postback | ✅ 一致 |
| UpdateProgress [templates][render] while a slow request runs: the progress shown | ✅ 一致 |
| UpdateProgress [templates][viewstate] after the slow request: the progress hidden | ✅ 一致 |
| UpdateProgress [viewstate] the timer started: one tick, then stopped | ✅ 一致 |
| UpdateProgress [viewstate] a history point, then the browser&#39;s back: the script manager&#39;s Navigate | ✅ 一致 |
| UpdateProgress [dynamic][viewstate] an asynchronous postback of the panel made in code | ✅ 一致 |
| UpdateProgress [visible][viewstate] a hidden panel shown with what was set while hidden | ✅ 一致 |

</details>
