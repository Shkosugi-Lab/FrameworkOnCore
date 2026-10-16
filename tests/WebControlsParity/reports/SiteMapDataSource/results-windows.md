# SiteMapDataSource パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 00:46
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 136 | 136 | 0 | 0 |
| ② HTML | 57 | 57 | 0 | 0 |
| ③ シナリオ | 12 | 12 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 143 件中 143 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 14 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 12 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 49 | 1 | ✅ |
| ViewState の保存と復元 | 5 | 11 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 1 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>SiteMapDataSource.Api.Control.#ctor</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.Adapter</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.ClientID</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.Context</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.Controls</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.DataBind</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.Dispose</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.Disposed</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.Events</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.FindControl</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.Focus</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.HasControls</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.ID</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.Init</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.Load</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.OnInit</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.Page</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.Parent</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.PreRender</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.Render</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.Site</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.SkinID</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.Unload</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.ViewState</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.Control.Visible</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.HierarchicalDataSourceControl.#ctor</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.HierarchicalDataSourceControl.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.HierarchicalDataSourceControl.ClientID</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.HierarchicalDataSourceControl.ClientIDMode</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.HierarchicalDataSourceControl.Controls</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.HierarchicalDataSourceControl.CreateControlCollection</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.HierarchicalDataSourceControl.EnableTheming</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.HierarchicalDataSourceControl.FindControl</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.HierarchicalDataSourceControl.Focus</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.HierarchicalDataSourceControl.GetHierarchicalView</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.HierarchicalDataSourceControl.HasControls</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.HierarchicalDataSourceControl.OnDataSourceChanged</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.HierarchicalDataSourceControl.RenderControl</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.HierarchicalDataSourceControl.SkinID</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.HierarchicalDataSourceControl.Visible</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.IDataSource.DataSourceChanged</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.IDataSource.GetView</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.IDataSource.GetViewNames</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.IHierarchicalDataSource.DataSourceChanged</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.IHierarchicalDataSource.GetHierarchicalView</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.WebControls.SiteMapDataSource.#ctor</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.WebControls.SiteMapDataSource.ContainsListCollection</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.WebControls.SiteMapDataSource.GetHierarchicalView</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.WebControls.SiteMapDataSource.GetList</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.WebControls.SiteMapDataSource.GetView</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.WebControls.SiteMapDataSource.GetViewNames</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.WebControls.SiteMapDataSource.Provider</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.WebControls.SiteMapDataSource.ShowStartingNode</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.WebControls.SiteMapDataSource.SiteMapProvider</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.WebControls.SiteMapDataSource.StartFromCurrentNode</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.WebControls.SiteMapDataSource.StartingNodeOffset</code> | ✅ 一致 |
| <code>SiteMapDataSource.Api.WebControls.SiteMapDataSource.StartingNodeUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>SiteMapDataSource.(case list)</code> | ✅ 一致 |
| <code>SiteMapDataSource.(default)</code> | ✅ 一致 |
| <code>SiteMapDataSource.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>SiteMapDataSource.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>SiteMapDataSource.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>SiteMapDataSource.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>SiteMapDataSource.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>SiteMapDataSource.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>SiteMapDataSource.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>SiteMapDataSource.ClientIDMode=Static</code> | ✅ 一致 |
| <code>SiteMapDataSource.EnableTheming=false</code> | ✅ 一致 |
| <code>SiteMapDataSource.EnableTheming=true</code> | ✅ 一致 |
| <code>SiteMapDataSource.EnableViewState=false</code> | ✅ 一致 |
| <code>SiteMapDataSource.EnableViewState=true</code> | ✅ 一致 |
| <code>SiteMapDataSource.ID=empty</code> | ✅ 一致 |
| <code>SiteMapDataSource.ID=japanese</code> | ✅ 一致 |
| <code>SiteMapDataSource.ID=markup</code> | ✅ 一致 |
| <code>SiteMapDataSource.ID=text</code> | ✅ 一致 |
| <code>SiteMapDataSource.ShowStartingNode=false</code> | ✅ 一致 |
| <code>SiteMapDataSource.ShowStartingNode=false+StartFromCurrentNode=true</code> | ✅ 一致 |
| <code>SiteMapDataSource.ShowStartingNode=false+Visible=false</code> | ✅ 一致 |
| <code>SiteMapDataSource.ShowStartingNode=true</code> | ✅ 一致 |
| <code>SiteMapDataSource.SiteMapProvider=default</code> | ✅ 一致 |
| <code>SiteMapDataSource.SiteMapProvider=empty</code> | ✅ 一致 |
| <code>SiteMapDataSource.SiteMapProvider=japanese</code> | ✅ 一致 |
| <code>SiteMapDataSource.SiteMapProvider=markup</code> | ✅ 一致 |
| <code>SiteMapDataSource.SiteMapProvider=text</code> | ✅ 一致 |
| <code>SiteMapDataSource.SkinID=empty</code> | ✅ 一致 |
| <code>SiteMapDataSource.SkinID=japanese</code> | ✅ 一致 |
| <code>SiteMapDataSource.SkinID=markup</code> | ✅ 一致 |
| <code>SiteMapDataSource.SkinID=text</code> | ✅ 一致 |
| <code>SiteMapDataSource.StartFromCurrentNode=false</code> | ✅ 一致 |
| <code>SiteMapDataSource.StartFromCurrentNode=true</code> | ✅ 一致 |
| <code>SiteMapDataSource.StartingNodeOffset=-1</code> | ✅ 一致 |
| <code>SiteMapDataSource.StartingNodeOffset=-1+StartFromCurrentNode=true</code> | ✅ 一致 |
| <code>SiteMapDataSource.StartingNodeOffset=0</code> | ✅ 一致 |
| <code>SiteMapDataSource.StartingNodeOffset=3</code> | ✅ 一致 |
| <code>SiteMapDataSource.StartingNodeUrl=app-relative</code> | ✅ 一致 |
| <code>SiteMapDataSource.StartingNodeUrl=empty</code> | ✅ 一致 |
| <code>SiteMapDataSource.StartingNodeUrl=japanese</code> | ✅ 一致 |
| <code>SiteMapDataSource.StartingNodeUrl=markup</code> | ✅ 一致 |
| <code>SiteMapDataSource.StartingNodeUrl=missing</code> | ✅ 一致 |
| <code>SiteMapDataSource.StartingNodeUrl=text</code> | ✅ 一致 |
| <code>SiteMapDataSource.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>SiteMapDataSource.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>SiteMapDataSource.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>SiteMapDataSource.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>SiteMapDataSource.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>SiteMapDataSource.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>SiteMapDataSource.Visible=false</code> | ✅ 一致 |
| <code>SiteMapDataSource.Visible=true</code> | ✅ 一致 |
| <code>SiteMapDataSource[naming container].(default)</code> | ✅ 一致 |
| <code>SiteMapDataSource[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>SiteMapDataSource[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>SiteMapDataSource[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>SiteMapDataSource[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>SiteMapDataSource[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| SiteMapDataSource [render][databind][dynamic] first request: the site map as a path, trees and menus; bound to XML; templated; made in code | ✅ 一致 |
| SiteMapDataSource [viewstate] a node expanded by a postback (EnableClientScript=false) | ✅ 一致 |
| SiteMapDataSource [viewstate] a node selected (its value a control parameter&#39;s: ControlValueProperty) | ✅ 一致 |
| SiteMapDataSource [viewstate] a node&#39;s check box posted | ✅ 一致 |
| SiteMapDataSource [viewstate] a node populated on demand by a postback | ✅ 一致 |
| SiteMapDataSource [viewstate] a node populated on demand by a client callback (no postback) | ✅ 一致 |
| SiteMapDataSource [viewstate] a menu&#39;s item clicked | ✅ 一致 |
| SiteMapDataSource [viewstate] a bound menu&#39;s item clicked | ✅ 一致 |
| SiteMapDataSource [viewstate] a templated menu&#39;s item clicked | ✅ 一致 |
| SiteMapDataSource [viewstate] a dynamic tree&#39;s node selected | ✅ 一致 |
| SiteMapDataSource [viewstate] a dynamic menu&#39;s item clicked | ✅ 一致 |
| SiteMapDataSource [visible][viewstate] hidden ones shown with what was added while hidden | ✅ 一致 |

</details>
