# QueryExtender パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 00:39
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 112 | 112 | 0 | 0 |
| ② HTML | 43 | 43 | 0 | 0 |
| ③ シナリオ | 15 | 15 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 119 件中 119 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 10 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 15 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 35 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 14 | ✅ |
| Visible=false（描画されず状態は保持） | 3 | 1 | ✅ |
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
| <code>QueryExtender.Api.Control.#ctor</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.Adapter</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.ClientID</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.Context</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.Controls</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.DataBind</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.Dispose</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.Disposed</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.Events</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.FindControl</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.Focus</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.HasControls</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.ID</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.Init</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.Load</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.OnInit</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.Page</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.Parent</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.PreRender</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.Render</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.Site</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.SkinID</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.Unload</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.ViewState</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>QueryExtender.Api.Control.Visible</code> | ✅ 一致 |
| <code>QueryExtender.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>QueryExtender.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>QueryExtender.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>QueryExtender.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>QueryExtender.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>QueryExtender.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>QueryExtender.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>QueryExtender.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>QueryExtender.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>QueryExtender.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>QueryExtender.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>QueryExtender.Api.WebControls.QueryExtender.#ctor</code> | ✅ 一致 |
| <code>QueryExtender.Api.WebControls.QueryExtender.DataSource</code> | ✅ 一致 |
| <code>QueryExtender.Api.WebControls.QueryExtender.Expressions</code> | ✅ 一致 |
| <code>QueryExtender.Api.WebControls.QueryExtender.LoadViewState</code> | ✅ 一致 |
| <code>QueryExtender.Api.WebControls.QueryExtender.OnInit</code> | ✅ 一致 |
| <code>QueryExtender.Api.WebControls.QueryExtender.SaveViewState</code> | ✅ 一致 |
| <code>QueryExtender.Api.WebControls.QueryExtender.TargetControlID</code> | ✅ 一致 |
| <code>QueryExtender.Api.WebControls.QueryExtender.TrackViewState</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>QueryExtender.(case list)</code> | ✅ 一致 |
| <code>QueryExtender.(default)</code> | ✅ 一致 |
| <code>QueryExtender.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>QueryExtender.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>QueryExtender.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>QueryExtender.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>QueryExtender.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>QueryExtender.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>QueryExtender.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>QueryExtender.ClientIDMode=Static</code> | ✅ 一致 |
| <code>QueryExtender.EnableTheming=false</code> | ✅ 一致 |
| <code>QueryExtender.EnableTheming=true</code> | ✅ 一致 |
| <code>QueryExtender.EnableViewState=false</code> | ✅ 一致 |
| <code>QueryExtender.EnableViewState=true</code> | ✅ 一致 |
| <code>QueryExtender.ID=empty</code> | ✅ 一致 |
| <code>QueryExtender.ID=japanese</code> | ✅ 一致 |
| <code>QueryExtender.ID=markup</code> | ✅ 一致 |
| <code>QueryExtender.ID=text</code> | ✅ 一致 |
| <code>QueryExtender.SkinID=empty</code> | ✅ 一致 |
| <code>QueryExtender.SkinID=japanese</code> | ✅ 一致 |
| <code>QueryExtender.SkinID=markup</code> | ✅ 一致 |
| <code>QueryExtender.SkinID=text</code> | ✅ 一致 |
| <code>QueryExtender.TargetControlID=empty</code> | ✅ 一致 |
| <code>QueryExtender.TargetControlID=japanese</code> | ✅ 一致 |
| <code>QueryExtender.TargetControlID=linq</code> | ✅ 一致 |
| <code>QueryExtender.TargetControlID=linq+Visible=false</code> | ✅ 一致 |
| <code>QueryExtender.TargetControlID=markup</code> | ✅ 一致 |
| <code>QueryExtender.TargetControlID=text</code> | ✅ 一致 |
| <code>QueryExtender.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>QueryExtender.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>QueryExtender.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>QueryExtender.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>QueryExtender.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>QueryExtender.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>QueryExtender.Visible=false</code> | ✅ 一致 |
| <code>QueryExtender.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>QueryExtender.Visible=true</code> | ✅ 一致 |
| <code>QueryExtender[naming container].(default)</code> | ✅ 一致 |
| <code>QueryExtender[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>QueryExtender[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>QueryExtender[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>QueryExtender[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>QueryExtender[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| QueryExtender [render][databind][dynamic] first request: grids bound to each data source, the Access data source selected in code, ones made in code | ✅ 一致 |
| QueryExtender [viewstate] an object data source&#39;s row updated | ✅ 一致 |
| QueryExtender [viewstate] an object data source&#39;s row deleted | ✅ 一致 |
| QueryExtender [viewstate] a SQL data source sorted | ✅ 一致 |
| QueryExtender [viewstate] a SQL data source&#39;s row updated (its update command&#39;s parameters) | ✅ 一致 |
| QueryExtender [viewstate] a SQL data source&#39;s row deleted | ✅ 一致 |
| QueryExtender [viewstate] a SQL data source&#39;s row inserted | ✅ 一致 |
| QueryExtender [viewstate] a LINQ data source paged | ✅ 一致 |
| QueryExtender [viewstate] a LINQ data source sorted | ✅ 一致 |
| QueryExtender [viewstate] a LINQ data source&#39;s where parameter from a text box | ✅ 一致 |
| QueryExtender [viewstate] a query extender&#39;s search from a text box | ✅ 一致 |
| QueryExtender [viewstate] a model error shown | ✅ 一致 |
| QueryExtender [viewstate] no model error | ✅ 一致 |
| QueryExtender [dynamic][viewstate] a grid sorted by a data source made in code | ✅ 一致 |
| QueryExtender [visible][viewstate] a hidden grid bound to a data source shown | ✅ 一致 |

</details>
