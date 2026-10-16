# Content パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 01:38
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 112 | 112 | 0 | 0 |
| ② HTML | 42 | 42 | 0 | 0 |
| ③ シナリオ | 4 | 4 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 119 件中 119 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 10 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 4 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 34 | 1 | ✅ |
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
| <code>Content.Api.Control.#ctor</code> | ✅ 一致 |
| <code>Content.Api.Control.Adapter</code> | ✅ 一致 |
| <code>Content.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>Content.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>Content.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>Content.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>Content.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>Content.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>Content.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>Content.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>Content.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>Content.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>Content.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>Content.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>Content.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>Content.Api.Control.ClientID</code> | ✅ 一致 |
| <code>Content.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>Content.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>Content.Api.Control.Context</code> | ✅ 一致 |
| <code>Content.Api.Control.Controls</code> | ✅ 一致 |
| <code>Content.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>Content.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>Content.Api.Control.DataBind</code> | ✅ 一致 |
| <code>Content.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>Content.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>Content.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>Content.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>Content.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>Content.Api.Control.Dispose</code> | ✅ 一致 |
| <code>Content.Api.Control.Disposed</code> | ✅ 一致 |
| <code>Content.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>Content.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>Content.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>Content.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>Content.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>Content.Api.Control.Events</code> | ✅ 一致 |
| <code>Content.Api.Control.FindControl</code> | ✅ 一致 |
| <code>Content.Api.Control.Focus</code> | ✅ 一致 |
| <code>Content.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>Content.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>Content.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>Content.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>Content.Api.Control.HasControls</code> | ✅ 一致 |
| <code>Content.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>Content.Api.Control.ID</code> | ✅ 一致 |
| <code>Content.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>Content.Api.Control.Init</code> | ✅ 一致 |
| <code>Content.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>Content.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>Content.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>Content.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>Content.Api.Control.Load</code> | ✅ 一致 |
| <code>Content.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>Content.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>Content.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>Content.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>Content.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>Content.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>Content.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>Content.Api.Control.OnInit</code> | ✅ 一致 |
| <code>Content.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>Content.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>Content.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>Content.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>Content.Api.Control.Page</code> | ✅ 一致 |
| <code>Content.Api.Control.Parent</code> | ✅ 一致 |
| <code>Content.Api.Control.PreRender</code> | ✅ 一致 |
| <code>Content.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>Content.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>Content.Api.Control.Render</code> | ✅ 一致 |
| <code>Content.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>Content.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>Content.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>Content.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>Content.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>Content.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>Content.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>Content.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>Content.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>Content.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>Content.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>Content.Api.Control.Site</code> | ✅ 一致 |
| <code>Content.Api.Control.SkinID</code> | ✅ 一致 |
| <code>Content.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>Content.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>Content.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>Content.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>Content.Api.Control.Unload</code> | ✅ 一致 |
| <code>Content.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>Content.Api.Control.ViewState</code> | ✅ 一致 |
| <code>Content.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>Content.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>Content.Api.Control.Visible</code> | ✅ 一致 |
| <code>Content.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>Content.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>Content.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>Content.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>Content.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>Content.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>Content.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>Content.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>Content.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>Content.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>Content.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>Content.Api.WebControls.Content.#ctor</code> | ✅ 一致 |
| <code>Content.Api.WebControls.Content.ContentPlaceHolderID</code> | ✅ 一致 |
| <code>Content.Api.WebControls.Content.DataBinding</code> | ✅ 一致 |
| <code>Content.Api.WebControls.Content.Disposed</code> | ✅ 一致 |
| <code>Content.Api.WebControls.Content.Init</code> | ✅ 一致 |
| <code>Content.Api.WebControls.Content.Load</code> | ✅ 一致 |
| <code>Content.Api.WebControls.Content.PreRender</code> | ✅ 一致 |
| <code>Content.Api.WebControls.Content.Unload</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>Content.(case list)</code> | ✅ 一致 |
| <code>Content.(default)</code> | ✅ 一致 |
| <code>Content.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>Content.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>Content.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>Content.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>Content.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Content.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Content.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Content.ClientIDMode=Static</code> | ✅ 一致 |
| <code>Content.ContentPlaceHolderID=empty</code> | ✅ 一致 |
| <code>Content.ContentPlaceHolderID=japanese</code> | ✅ 一致 |
| <code>Content.ContentPlaceHolderID=main</code> | ✅ 一致 |
| <code>Content.ContentPlaceHolderID=markup</code> | ✅ 一致 |
| <code>Content.ContentPlaceHolderID=text</code> | ✅ 一致 |
| <code>Content.EnableTheming=false</code> | ✅ 一致 |
| <code>Content.EnableTheming=true</code> | ✅ 一致 |
| <code>Content.EnableViewState=false</code> | ✅ 一致 |
| <code>Content.EnableViewState=true</code> | ✅ 一致 |
| <code>Content.ID=empty</code> | ✅ 一致 |
| <code>Content.ID=japanese</code> | ✅ 一致 |
| <code>Content.ID=markup</code> | ✅ 一致 |
| <code>Content.ID=text</code> | ✅ 一致 |
| <code>Content.SkinID=empty</code> | ✅ 一致 |
| <code>Content.SkinID=japanese</code> | ✅ 一致 |
| <code>Content.SkinID=markup</code> | ✅ 一致 |
| <code>Content.SkinID=text</code> | ✅ 一致 |
| <code>Content.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>Content.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>Content.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>Content.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>Content.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>Content.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>Content.Visible=false</code> | ✅ 一致 |
| <code>Content.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>Content.Visible=true</code> | ✅ 一致 |
| <code>Content[naming container].(default)</code> | ✅ 一致 |
| <code>Content[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Content[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Content[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Content[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>Content[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| Content [render][databind][dynamic][naming] first request: the page&#39;s content in the master&#39;s place holders, a default, a hidden one, one filled in code | ✅ 一致 |
| Content [viewstate][dynamic] posted: the content&#39;s and the dynamic text boxes, view state | ✅ 一致 |
| Content [visible][viewstate] hidden ones shown (the content&#39;s panel, the master&#39;s place holder) | ✅ 一致 |
| Content [viewstate] the master&#39;s own button | ✅ 一致 |

</details>
