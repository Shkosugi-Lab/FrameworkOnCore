# UpdatePanel パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-16 18:06
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 127 | 127 | 0 | 0 |
| ② HTML | 52 | 52 | 0 | 0 |
| ③ シナリオ | 10 | 10 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 134 件中 134 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 12 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 10 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 43 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 9 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 7 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| テンプレート | 0 | 2 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>UpdatePanel.Api.Control.#ctor</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.Adapter</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.ClientID</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.Context</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.Controls</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.DataBind</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.Dispose</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.Disposed</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.Events</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.FindControl</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.Focus</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.HasControls</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.ID</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.Init</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.Load</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.OnInit</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.Page</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.Parent</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.PreRender</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.Render</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.Site</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.SkinID</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.Unload</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.ViewState</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>UpdatePanel.Api.Control.Visible</code> | ✅ 一致 |
| <code>UpdatePanel.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>UpdatePanel.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>UpdatePanel.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>UpdatePanel.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>UpdatePanel.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>UpdatePanel.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>UpdatePanel.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>UpdatePanel.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>UpdatePanel.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>UpdatePanel.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>UpdatePanel.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>UpdatePanel.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>UpdatePanel.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>UpdatePanel.Api.UpdatePanel.#ctor</code> | ✅ 一致 |
| <code>UpdatePanel.Api.UpdatePanel.Attributes</code> | ✅ 一致 |
| <code>UpdatePanel.Api.UpdatePanel.ChildrenAsTriggers</code> | ✅ 一致 |
| <code>UpdatePanel.Api.UpdatePanel.ContentTemplate</code> | ✅ 一致 |
| <code>UpdatePanel.Api.UpdatePanel.ContentTemplateContainer</code> | ✅ 一致 |
| <code>UpdatePanel.Api.UpdatePanel.Controls</code> | ✅ 一致 |
| <code>UpdatePanel.Api.UpdatePanel.CreateContentTemplateContainer</code> | ✅ 一致 |
| <code>UpdatePanel.Api.UpdatePanel.CreateControlCollection</code> | ✅ 一致 |
| <code>UpdatePanel.Api.UpdatePanel.Initialize</code> | ✅ 一致 |
| <code>UpdatePanel.Api.UpdatePanel.IsInPartialRendering</code> | ✅ 一致 |
| <code>UpdatePanel.Api.UpdatePanel.OnInit</code> | ✅ 一致 |
| <code>UpdatePanel.Api.UpdatePanel.OnLoad</code> | ✅ 一致 |
| <code>UpdatePanel.Api.UpdatePanel.OnPreRender</code> | ✅ 一致 |
| <code>UpdatePanel.Api.UpdatePanel.OnUnload</code> | ✅ 一致 |
| <code>UpdatePanel.Api.UpdatePanel.Render</code> | ✅ 一致 |
| <code>UpdatePanel.Api.UpdatePanel.RenderChildren</code> | ✅ 一致 |
| <code>UpdatePanel.Api.UpdatePanel.RenderMode</code> | ✅ 一致 |
| <code>UpdatePanel.Api.UpdatePanel.RequiresUpdate</code> | ✅ 一致 |
| <code>UpdatePanel.Api.UpdatePanel.Triggers</code> | ✅ 一致 |
| <code>UpdatePanel.Api.UpdatePanel.Update</code> | ✅ 一致 |
| <code>UpdatePanel.Api.UpdatePanel.UpdateMode</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>UpdatePanel.(case list)</code> | ✅ 一致 |
| <code>UpdatePanel.(default)</code> | ✅ 一致 |
| <code>UpdatePanel.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>UpdatePanel.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>UpdatePanel.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>UpdatePanel.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>UpdatePanel.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>UpdatePanel.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>UpdatePanel.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>UpdatePanel.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>UpdatePanel.ChildrenAsTriggers=false</code> | ✅ 一致 |
| <code>UpdatePanel.ChildrenAsTriggers=true</code> | ✅ 一致 |
| <code>UpdatePanel.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>UpdatePanel.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>UpdatePanel.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>UpdatePanel.ClientIDMode=Static</code> | ✅ 一致 |
| <code>UpdatePanel.EnableTheming=false</code> | ✅ 一致 |
| <code>UpdatePanel.EnableTheming=true</code> | ✅ 一致 |
| <code>UpdatePanel.EnableViewState=false</code> | ✅ 一致 |
| <code>UpdatePanel.EnableViewState=true</code> | ✅ 一致 |
| <code>UpdatePanel.ID=empty</code> | ✅ 一致 |
| <code>UpdatePanel.ID=japanese</code> | ✅ 一致 |
| <code>UpdatePanel.ID=markup</code> | ✅ 一致 |
| <code>UpdatePanel.ID=text</code> | ✅ 一致 |
| <code>UpdatePanel.RenderMode=Block</code> | ✅ 一致 |
| <code>UpdatePanel.RenderMode=Inline</code> | ✅ 一致 |
| <code>UpdatePanel.RenderMode=Inline+UpdateMode=Conditional</code> | ✅ 一致 |
| <code>UpdatePanel.RenderMode=Inline+Visible=false</code> | ✅ 一致 |
| <code>UpdatePanel.SkinID=empty</code> | ✅ 一致 |
| <code>UpdatePanel.SkinID=japanese</code> | ✅ 一致 |
| <code>UpdatePanel.SkinID=markup</code> | ✅ 一致 |
| <code>UpdatePanel.SkinID=text</code> | ✅ 一致 |
| <code>UpdatePanel.UpdateMode=Always</code> | ✅ 一致 |
| <code>UpdatePanel.UpdateMode=Always+ChildrenAsTriggers=false</code> | ✅ 一致 |
| <code>UpdatePanel.UpdateMode=Conditional</code> | ✅ 一致 |
| <code>UpdatePanel.UpdateMode=Conditional+ChildrenAsTriggers=false</code> | ✅ 一致 |
| <code>UpdatePanel.UpdateMode=Conditional+EnableViewState=false</code> | ✅ 一致 |
| <code>UpdatePanel.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>UpdatePanel.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>UpdatePanel.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>UpdatePanel.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>UpdatePanel.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>UpdatePanel.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>UpdatePanel.Visible=false</code> | ✅ 一致 |
| <code>UpdatePanel.Visible=true</code> | ✅ 一致 |
| <code>UpdatePanel[naming container].(default)</code> | ✅ 一致 |
| <code>UpdatePanel[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>UpdatePanel[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>UpdatePanel[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>UpdatePanel[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>UpdatePanel[naming container].ID=text</code> | ✅ 一致 |
| <code>UpdatePanel[naming container].RenderMode=Inline</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| UpdatePanel [render][databind][dynamic][templates] first request: the panels, the proxy&#39;s script run, a panel made in code | ✅ 一致 |
| UpdatePanel [viewstate][postdata][templates] an asynchronous postback of the main panel: the conditional panel outside left as it was | ✅ 一致 |
| UpdatePanel [viewstate][postdata] a button outside triggers the second panel | ✅ 一致 |
| UpdatePanel [viewstate][postdata] a postback trigger in the panel: a full postback | ✅ 一致 |
| UpdatePanel [viewstate] while a slow request runs: the progress shown | ✅ 一致 |
| UpdatePanel [viewstate] after the slow request: the progress hidden | ✅ 一致 |
| UpdatePanel [viewstate] the timer started: one tick, then stopped | ✅ 一致 |
| UpdatePanel [viewstate] a history point, then the browser&#39;s back: the script manager&#39;s Navigate | ✅ 一致 |
| UpdatePanel [dynamic][viewstate] an asynchronous postback of the panel made in code | ✅ 一致 |
| UpdatePanel [visible][viewstate] a hidden panel shown with what was set while hidden | ✅ 一致 |

</details>
