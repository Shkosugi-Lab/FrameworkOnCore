# ContentPlaceHolder パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 01:39
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 105 | 105 | 0 | 0 |
| ② HTML | 37 | 37 | 0 | 0 |
| ③ シナリオ | 4 | 4 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 112 件中 112 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 9 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 4 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 29 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 3 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 1 | ✅ |
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
| <code>ContentPlaceHolder.Api.Control.#ctor</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.Adapter</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.ClientID</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.Context</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.Controls</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.DataBind</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.Dispose</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.Disposed</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.Events</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.FindControl</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.Focus</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.HasControls</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.ID</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.Init</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.Load</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.OnInit</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.Page</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.Parent</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.PreRender</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.Render</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.Site</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.SkinID</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.Unload</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.ViewState</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.Control.Visible</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Api.WebControls.ContentPlaceHolder.#ctor</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>ContentPlaceHolder.(case list)</code> | ✅ 一致 |
| <code>ContentPlaceHolder.(default)</code> | ✅ 一致 |
| <code>ContentPlaceHolder.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>ContentPlaceHolder.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>ContentPlaceHolder.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>ContentPlaceHolder.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>ContentPlaceHolder.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ContentPlaceHolder.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ContentPlaceHolder.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ContentPlaceHolder.ClientIDMode=Static</code> | ✅ 一致 |
| <code>ContentPlaceHolder.EnableTheming=false</code> | ✅ 一致 |
| <code>ContentPlaceHolder.EnableTheming=true</code> | ✅ 一致 |
| <code>ContentPlaceHolder.EnableViewState=false</code> | ✅ 一致 |
| <code>ContentPlaceHolder.EnableViewState=true</code> | ✅ 一致 |
| <code>ContentPlaceHolder.ID=empty</code> | ✅ 一致 |
| <code>ContentPlaceHolder.ID=japanese</code> | ✅ 一致 |
| <code>ContentPlaceHolder.ID=markup</code> | ✅ 一致 |
| <code>ContentPlaceHolder.ID=text</code> | ✅ 一致 |
| <code>ContentPlaceHolder.SkinID=empty</code> | ✅ 一致 |
| <code>ContentPlaceHolder.SkinID=japanese</code> | ✅ 一致 |
| <code>ContentPlaceHolder.SkinID=markup</code> | ✅ 一致 |
| <code>ContentPlaceHolder.SkinID=text</code> | ✅ 一致 |
| <code>ContentPlaceHolder.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>ContentPlaceHolder.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>ContentPlaceHolder.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>ContentPlaceHolder.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>ContentPlaceHolder.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>ContentPlaceHolder.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Visible=false</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>ContentPlaceHolder.Visible=true</code> | ✅ 一致 |
| <code>ContentPlaceHolder[naming container].(default)</code> | ✅ 一致 |
| <code>ContentPlaceHolder[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ContentPlaceHolder[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ContentPlaceHolder[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ContentPlaceHolder[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>ContentPlaceHolder[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| ContentPlaceHolder [render][databind][dynamic][naming] first request: the page&#39;s content in the master&#39;s place holders, a default, a hidden one, one filled in code | ✅ 一致 |
| ContentPlaceHolder [viewstate][dynamic] posted: the content&#39;s and the dynamic text boxes, view state | ✅ 一致 |
| ContentPlaceHolder [visible][viewstate] hidden ones shown (the content&#39;s panel, the master&#39;s place holder) | ✅ 一致 |
| ContentPlaceHolder [viewstate] the master&#39;s own button | ✅ 一致 |

</details>
