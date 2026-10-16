# XmlDataSource パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-15 13:50
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 141 | 141 | 0 | 0 |
| ② HTML | 79 | 79 | 0 | 0 |
| ③ シナリオ | 15 | 15 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 148 件中 148 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 19 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 15 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 71 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 14 | ✅ |
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
| <code>XmlDataSource.Api.Control.#ctor</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.Adapter</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.ClientID</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.Context</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.Controls</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.DataBind</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.Dispose</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.Disposed</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.Events</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.FindControl</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.Focus</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.HasControls</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.ID</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.Init</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.Load</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.OnInit</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.Page</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.Parent</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.PreRender</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.Render</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.Site</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.SkinID</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.Unload</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.ViewState</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>XmlDataSource.Api.Control.Visible</code> | ✅ 一致 |
| <code>XmlDataSource.Api.HierarchicalDataSourceControl.#ctor</code> | ✅ 一致 |
| <code>XmlDataSource.Api.HierarchicalDataSourceControl.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>XmlDataSource.Api.HierarchicalDataSourceControl.ClientID</code> | ✅ 一致 |
| <code>XmlDataSource.Api.HierarchicalDataSourceControl.ClientIDMode</code> | ✅ 一致 |
| <code>XmlDataSource.Api.HierarchicalDataSourceControl.Controls</code> | ✅ 一致 |
| <code>XmlDataSource.Api.HierarchicalDataSourceControl.CreateControlCollection</code> | ✅ 一致 |
| <code>XmlDataSource.Api.HierarchicalDataSourceControl.EnableTheming</code> | ✅ 一致 |
| <code>XmlDataSource.Api.HierarchicalDataSourceControl.FindControl</code> | ✅ 一致 |
| <code>XmlDataSource.Api.HierarchicalDataSourceControl.Focus</code> | ✅ 一致 |
| <code>XmlDataSource.Api.HierarchicalDataSourceControl.GetHierarchicalView</code> | ✅ 一致 |
| <code>XmlDataSource.Api.HierarchicalDataSourceControl.HasControls</code> | ✅ 一致 |
| <code>XmlDataSource.Api.HierarchicalDataSourceControl.OnDataSourceChanged</code> | ✅ 一致 |
| <code>XmlDataSource.Api.HierarchicalDataSourceControl.RenderControl</code> | ✅ 一致 |
| <code>XmlDataSource.Api.HierarchicalDataSourceControl.SkinID</code> | ✅ 一致 |
| <code>XmlDataSource.Api.HierarchicalDataSourceControl.Visible</code> | ✅ 一致 |
| <code>XmlDataSource.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>XmlDataSource.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>XmlDataSource.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>XmlDataSource.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>XmlDataSource.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>XmlDataSource.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>XmlDataSource.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>XmlDataSource.Api.IDataSource.DataSourceChanged</code> | ✅ 一致 |
| <code>XmlDataSource.Api.IDataSource.GetView</code> | ✅ 一致 |
| <code>XmlDataSource.Api.IDataSource.GetViewNames</code> | ✅ 一致 |
| <code>XmlDataSource.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>XmlDataSource.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>XmlDataSource.Api.IHierarchicalDataSource.DataSourceChanged</code> | ✅ 一致 |
| <code>XmlDataSource.Api.IHierarchicalDataSource.GetHierarchicalView</code> | ✅ 一致 |
| <code>XmlDataSource.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>XmlDataSource.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>XmlDataSource.Api.WebControls.XmlDataSource.#ctor</code> | ✅ 一致 |
| <code>XmlDataSource.Api.WebControls.XmlDataSource.CacheDuration</code> | ✅ 一致 |
| <code>XmlDataSource.Api.WebControls.XmlDataSource.CacheExpirationPolicy</code> | ✅ 一致 |
| <code>XmlDataSource.Api.WebControls.XmlDataSource.CacheKeyContext</code> | ✅ 一致 |
| <code>XmlDataSource.Api.WebControls.XmlDataSource.CacheKeyDependency</code> | ✅ 一致 |
| <code>XmlDataSource.Api.WebControls.XmlDataSource.Data</code> | ✅ 一致 |
| <code>XmlDataSource.Api.WebControls.XmlDataSource.DataFile</code> | ✅ 一致 |
| <code>XmlDataSource.Api.WebControls.XmlDataSource.EnableCaching</code> | ✅ 一致 |
| <code>XmlDataSource.Api.WebControls.XmlDataSource.GetHierarchicalView</code> | ✅ 一致 |
| <code>XmlDataSource.Api.WebControls.XmlDataSource.GetXmlDocument</code> | ✅ 一致 |
| <code>XmlDataSource.Api.WebControls.XmlDataSource.OnTransforming</code> | ✅ 一致 |
| <code>XmlDataSource.Api.WebControls.XmlDataSource.Save</code> | ✅ 一致 |
| <code>XmlDataSource.Api.WebControls.XmlDataSource.Transform</code> | ✅ 一致 |
| <code>XmlDataSource.Api.WebControls.XmlDataSource.TransformArgumentList</code> | ✅ 一致 |
| <code>XmlDataSource.Api.WebControls.XmlDataSource.TransformFile</code> | ✅ 一致 |
| <code>XmlDataSource.Api.WebControls.XmlDataSource.Transforming</code> | ✅ 一致 |
| <code>XmlDataSource.Api.WebControls.XmlDataSource.XPath</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>XmlDataSource.(case list)</code> | ✅ 一致 |
| <code>XmlDataSource.(default)</code> | ✅ 一致 |
| <code>XmlDataSource.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>XmlDataSource.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>XmlDataSource.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>XmlDataSource.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>XmlDataSource.CacheDuration=-1</code> | ✅ 一致 |
| <code>XmlDataSource.CacheDuration=0</code> | ✅ 一致 |
| <code>XmlDataSource.CacheDuration=3</code> | ✅ 一致 |
| <code>XmlDataSource.CacheExpirationPolicy=Absolute</code> | ✅ 一致 |
| <code>XmlDataSource.CacheExpirationPolicy=Sliding</code> | ✅ 一致 |
| <code>XmlDataSource.CacheKeyContext=empty</code> | ✅ 一致 |
| <code>XmlDataSource.CacheKeyContext=japanese</code> | ✅ 一致 |
| <code>XmlDataSource.CacheKeyContext=markup</code> | ✅ 一致 |
| <code>XmlDataSource.CacheKeyContext=text</code> | ✅ 一致 |
| <code>XmlDataSource.CacheKeyDependency=empty</code> | ✅ 一致 |
| <code>XmlDataSource.CacheKeyDependency=japanese</code> | ✅ 一致 |
| <code>XmlDataSource.CacheKeyDependency=markup</code> | ✅ 一致 |
| <code>XmlDataSource.CacheKeyDependency=text</code> | ✅ 一致 |
| <code>XmlDataSource.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>XmlDataSource.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>XmlDataSource.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>XmlDataSource.ClientIDMode=Static</code> | ✅ 一致 |
| <code>XmlDataSource.Data=empty</code> | ✅ 一致 |
| <code>XmlDataSource.Data=japanese</code> | ✅ 一致 |
| <code>XmlDataSource.Data=markup</code> | ✅ 一致 |
| <code>XmlDataSource.Data=text</code> | ✅ 一致 |
| <code>XmlDataSource.DataFile=app-relative</code> | ✅ 一致 |
| <code>XmlDataSource.DataFile=empty</code> | ✅ 一致 |
| <code>XmlDataSource.DataFile=japanese</code> | ✅ 一致 |
| <code>XmlDataSource.DataFile=markup</code> | ✅ 一致 |
| <code>XmlDataSource.DataFile=missing</code> | ✅ 一致 |
| <code>XmlDataSource.DataFile=missing+XPath=products</code> | ✅ 一致 |
| <code>XmlDataSource.DataFile=text</code> | ✅ 一致 |
| <code>XmlDataSource.EnableCaching=false</code> | ✅ 一致 |
| <code>XmlDataSource.EnableCaching=false+CacheDuration=3</code> | ✅ 一致 |
| <code>XmlDataSource.EnableCaching=true</code> | ✅ 一致 |
| <code>XmlDataSource.EnableTheming=false</code> | ✅ 一致 |
| <code>XmlDataSource.EnableTheming=true</code> | ✅ 一致 |
| <code>XmlDataSource.EnableViewState=false</code> | ✅ 一致 |
| <code>XmlDataSource.EnableViewState=true</code> | ✅ 一致 |
| <code>XmlDataSource.ID=empty</code> | ✅ 一致 |
| <code>XmlDataSource.ID=japanese</code> | ✅ 一致 |
| <code>XmlDataSource.ID=markup</code> | ✅ 一致 |
| <code>XmlDataSource.ID=text</code> | ✅ 一致 |
| <code>XmlDataSource.SkinID=empty</code> | ✅ 一致 |
| <code>XmlDataSource.SkinID=japanese</code> | ✅ 一致 |
| <code>XmlDataSource.SkinID=markup</code> | ✅ 一致 |
| <code>XmlDataSource.SkinID=text</code> | ✅ 一致 |
| <code>XmlDataSource.Transform=empty</code> | ✅ 一致 |
| <code>XmlDataSource.Transform=japanese</code> | ✅ 一致 |
| <code>XmlDataSource.Transform=markup</code> | ✅ 一致 |
| <code>XmlDataSource.Transform=text</code> | ✅ 一致 |
| <code>XmlDataSource.TransformFile=app-relative</code> | ✅ 一致 |
| <code>XmlDataSource.TransformFile=empty</code> | ✅ 一致 |
| <code>XmlDataSource.TransformFile=japanese</code> | ✅ 一致 |
| <code>XmlDataSource.TransformFile=markup</code> | ✅ 一致 |
| <code>XmlDataSource.TransformFile=text</code> | ✅ 一致 |
| <code>XmlDataSource.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>XmlDataSource.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>XmlDataSource.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>XmlDataSource.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>XmlDataSource.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>XmlDataSource.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>XmlDataSource.Visible=false</code> | ✅ 一致 |
| <code>XmlDataSource.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>XmlDataSource.Visible=true</code> | ✅ 一致 |
| <code>XmlDataSource.XPath=empty</code> | ✅ 一致 |
| <code>XmlDataSource.XPath=japanese</code> | ✅ 一致 |
| <code>XmlDataSource.XPath=markup</code> | ✅ 一致 |
| <code>XmlDataSource.XPath=products</code> | ✅ 一致 |
| <code>XmlDataSource.XPath=products+TransformFile=app-relative</code> | ✅ 一致 |
| <code>XmlDataSource.XPath=text</code> | ✅ 一致 |
| <code>XmlDataSource[naming container].(default)</code> | ✅ 一致 |
| <code>XmlDataSource[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>XmlDataSource[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>XmlDataSource[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>XmlDataSource[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>XmlDataSource[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| XmlDataSource [render][databind][dynamic] first request: grids bound to each data source, the Access data source selected in code, ones made in code | ✅ 一致 |
| XmlDataSource [viewstate] an object data source&#39;s row updated | ✅ 一致 |
| XmlDataSource [viewstate] an object data source&#39;s row deleted | ✅ 一致 |
| XmlDataSource [viewstate] a SQL data source sorted | ✅ 一致 |
| XmlDataSource [viewstate] a SQL data source&#39;s row updated (its update command&#39;s parameters) | ✅ 一致 |
| XmlDataSource [viewstate] a SQL data source&#39;s row deleted | ✅ 一致 |
| XmlDataSource [viewstate] a SQL data source&#39;s row inserted | ✅ 一致 |
| XmlDataSource [viewstate] a LINQ data source paged | ✅ 一致 |
| XmlDataSource [viewstate] a LINQ data source sorted | ✅ 一致 |
| XmlDataSource [viewstate] a LINQ data source&#39;s where parameter from a text box | ✅ 一致 |
| XmlDataSource [viewstate] a query extender&#39;s search from a text box | ✅ 一致 |
| XmlDataSource [viewstate] a model error shown | ✅ 一致 |
| XmlDataSource [viewstate] no model error | ✅ 一致 |
| XmlDataSource [dynamic][viewstate] a grid sorted by a data source made in code | ✅ 一致 |
| XmlDataSource [visible][viewstate] a hidden grid bound to a data source shown | ✅ 一致 |

</details>
