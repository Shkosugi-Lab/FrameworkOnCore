# PlaceHolder パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 02:53
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 106 | 106 | 0 | 0 |
| ② HTML | 37 | 37 | 0 | 0 |
| ③ シナリオ | 4 | 4 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 113 件中 113 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 9 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 4 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 29 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 3 | ✅ |
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
| <code>PlaceHolder.Api.Control.#ctor</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.Adapter</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.ClientID</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.Context</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.Controls</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.DataBind</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.Dispose</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.Disposed</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.Events</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.FindControl</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.Focus</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.HasControls</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.ID</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.Init</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.Load</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.OnInit</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.Page</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.Parent</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.PreRender</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.Render</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.Site</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.SkinID</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.Unload</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.ViewState</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>PlaceHolder.Api.Control.Visible</code> | ✅ 一致 |
| <code>PlaceHolder.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>PlaceHolder.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>PlaceHolder.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>PlaceHolder.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>PlaceHolder.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>PlaceHolder.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>PlaceHolder.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>PlaceHolder.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>PlaceHolder.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>PlaceHolder.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>PlaceHolder.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>PlaceHolder.Api.WebControls.PlaceHolder.#ctor</code> | ✅ 一致 |
| <code>PlaceHolder.Api.WebControls.PlaceHolder.EnableTheming</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>PlaceHolder.(case list)</code> | ✅ 一致 |
| <code>PlaceHolder.(default)</code> | ✅ 一致 |
| <code>PlaceHolder.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>PlaceHolder.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>PlaceHolder.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>PlaceHolder.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>PlaceHolder.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>PlaceHolder.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>PlaceHolder.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>PlaceHolder.ClientIDMode=Static</code> | ✅ 一致 |
| <code>PlaceHolder.EnableTheming=false</code> | ✅ 一致 |
| <code>PlaceHolder.EnableTheming=true</code> | ✅ 一致 |
| <code>PlaceHolder.EnableViewState=false</code> | ✅ 一致 |
| <code>PlaceHolder.EnableViewState=true</code> | ✅ 一致 |
| <code>PlaceHolder.ID=empty</code> | ✅ 一致 |
| <code>PlaceHolder.ID=japanese</code> | ✅ 一致 |
| <code>PlaceHolder.ID=markup</code> | ✅ 一致 |
| <code>PlaceHolder.ID=text</code> | ✅ 一致 |
| <code>PlaceHolder.SkinID=empty</code> | ✅ 一致 |
| <code>PlaceHolder.SkinID=japanese</code> | ✅ 一致 |
| <code>PlaceHolder.SkinID=markup</code> | ✅ 一致 |
| <code>PlaceHolder.SkinID=text</code> | ✅ 一致 |
| <code>PlaceHolder.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>PlaceHolder.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>PlaceHolder.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>PlaceHolder.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>PlaceHolder.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>PlaceHolder.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>PlaceHolder.Visible=false</code> | ✅ 一致 |
| <code>PlaceHolder.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>PlaceHolder.Visible=true</code> | ✅ 一致 |
| <code>PlaceHolder[naming container].(default)</code> | ✅ 一致 |
| <code>PlaceHolder[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>PlaceHolder[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>PlaceHolder[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>PlaceHolder[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>PlaceHolder[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| PlaceHolder [render][databind][dynamic] first request | ✅ 一致 |
| PlaceHolder [dynamic][viewstate] a text box created in a place holder posts its value | ✅ 一致 |
| PlaceHolder [viewstate] posted again | ✅ 一致 |
| PlaceHolder [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
