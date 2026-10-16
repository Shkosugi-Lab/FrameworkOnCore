# LinqDataSource パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-15 14:01
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 192 | 192 | 0 | 0 |
| ② HTML | 93 | 93 | 0 | 0 |
| ③ シナリオ | 15 | 15 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 199 件中 199 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 26 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 15 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 85 | 1 | ✅ |
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
| <code>LinqDataSource.Api.Control.#ctor</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.Adapter</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.ClientID</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.Context</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.Controls</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.DataBind</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.Dispose</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.Disposed</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.Events</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.FindControl</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.Focus</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.HasControls</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.ID</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.Init</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.Load</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.OnInit</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.Page</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.Parent</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.PreRender</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.Render</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.Site</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.SkinID</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.Unload</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.ViewState</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>LinqDataSource.Api.Control.Visible</code> | ✅ 一致 |
| <code>LinqDataSource.Api.DataSourceControl.#ctor</code> | ✅ 一致 |
| <code>LinqDataSource.Api.DataSourceControl.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>LinqDataSource.Api.DataSourceControl.ClientID</code> | ✅ 一致 |
| <code>LinqDataSource.Api.DataSourceControl.ClientIDMode</code> | ✅ 一致 |
| <code>LinqDataSource.Api.DataSourceControl.Controls</code> | ✅ 一致 |
| <code>LinqDataSource.Api.DataSourceControl.CreateControlCollection</code> | ✅ 一致 |
| <code>LinqDataSource.Api.DataSourceControl.EnableTheming</code> | ✅ 一致 |
| <code>LinqDataSource.Api.DataSourceControl.FindControl</code> | ✅ 一致 |
| <code>LinqDataSource.Api.DataSourceControl.Focus</code> | ✅ 一致 |
| <code>LinqDataSource.Api.DataSourceControl.GetView</code> | ✅ 一致 |
| <code>LinqDataSource.Api.DataSourceControl.GetViewNames</code> | ✅ 一致 |
| <code>LinqDataSource.Api.DataSourceControl.HasControls</code> | ✅ 一致 |
| <code>LinqDataSource.Api.DataSourceControl.RaiseDataSourceChangedEvent</code> | ✅ 一致 |
| <code>LinqDataSource.Api.DataSourceControl.RenderControl</code> | ✅ 一致 |
| <code>LinqDataSource.Api.DataSourceControl.SkinID</code> | ✅ 一致 |
| <code>LinqDataSource.Api.DataSourceControl.Visible</code> | ✅ 一致 |
| <code>LinqDataSource.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>LinqDataSource.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>LinqDataSource.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>LinqDataSource.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>LinqDataSource.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>LinqDataSource.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>LinqDataSource.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>LinqDataSource.Api.IDataSource.DataSourceChanged</code> | ✅ 一致 |
| <code>LinqDataSource.Api.IDataSource.GetView</code> | ✅ 一致 |
| <code>LinqDataSource.Api.IDataSource.GetViewNames</code> | ✅ 一致 |
| <code>LinqDataSource.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>LinqDataSource.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>LinqDataSource.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>LinqDataSource.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>LinqDataSource.Api.System.Web.DynamicData.IDynamicDataSource.AutoGenerateWhereClause</code> | ✅ 一致 |
| <code>LinqDataSource.Api.System.Web.DynamicData.IDynamicDataSource.ContextType</code> | ✅ 一致 |
| <code>LinqDataSource.Api.System.Web.DynamicData.IDynamicDataSource.EnableDelete</code> | ✅ 一致 |
| <code>LinqDataSource.Api.System.Web.DynamicData.IDynamicDataSource.EnableInsert</code> | ✅ 一致 |
| <code>LinqDataSource.Api.System.Web.DynamicData.IDynamicDataSource.EnableUpdate</code> | ✅ 一致 |
| <code>LinqDataSource.Api.System.Web.DynamicData.IDynamicDataSource.EntitySetName</code> | ✅ 一致 |
| <code>LinqDataSource.Api.System.Web.DynamicData.IDynamicDataSource.Exception</code> | ✅ 一致 |
| <code>LinqDataSource.Api.System.Web.DynamicData.IDynamicDataSource.Where</code> | ✅ 一致 |
| <code>LinqDataSource.Api.System.Web.DynamicData.IDynamicDataSource.WhereParameters</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.ContextDataSource.#ctor</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.ContextDataSource.ContextTypeName</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.ContextDataSource.EntitySetName</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.ContextDataSource.EntityTypeName</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.IQueryableDataSource.QueryCreated</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.IQueryableDataSource.RaiseViewChanged</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.#ctor</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.AutoGenerateOrderByClause</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.AutoGenerateWhereClause</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.AutoPage</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.AutoSort</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.ContextCreated</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.ContextCreating</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.ContextDisposing</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.ContextTypeName</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.CreateQueryableView</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.CreateView</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.Delete</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.DeleteParameters</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.Deleted</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.Deleting</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.EnableDelete</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.EnableInsert</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.EnableObjectTracking</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.EnableUpdate</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.GroupBy</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.GroupByParameters</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.Insert</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.InsertParameters</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.Inserted</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.Inserting</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.OnInit</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.OnUnload</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.OrderBy</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.OrderByParameters</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.OrderGroupsBy</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.OrderGroupsByParameters</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.Select</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.SelectParameters</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.Selected</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.Selecting</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.StoreOriginalValuesInViewState</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.TableName</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.Update</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.UpdateParameters</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.Updated</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.Updating</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.Where</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.LinqDataSource.WhereParameters</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.QueryableDataSource.#ctor</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.QueryableDataSource.CreateQueryableView</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.QueryableDataSource.GetView</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.QueryableDataSource.GetViewNames</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.QueryableDataSource.LoadViewState</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.QueryableDataSource.OnInit</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.QueryableDataSource.QueryCreated</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.QueryableDataSource.RaiseViewChanged</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.QueryableDataSource.SaveViewState</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.QueryableDataSource.TrackViewState</code> | ✅ 一致 |
| <code>LinqDataSource.Api.WebControls.QueryableDataSource.UpdateParameterVales</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>LinqDataSource.(case list)</code> | ✅ 一致 |
| <code>LinqDataSource.(default)</code> | ✅ 一致 |
| <code>LinqDataSource.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>LinqDataSource.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>LinqDataSource.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>LinqDataSource.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>LinqDataSource.AutoGenerateOrderByClause=false</code> | ✅ 一致 |
| <code>LinqDataSource.AutoGenerateOrderByClause=true</code> | ✅ 一致 |
| <code>LinqDataSource.AutoGenerateWhereClause=false</code> | ✅ 一致 |
| <code>LinqDataSource.AutoGenerateWhereClause=true</code> | ✅ 一致 |
| <code>LinqDataSource.AutoPage=false</code> | ✅ 一致 |
| <code>LinqDataSource.AutoPage=false+AutoSort=false</code> | ✅ 一致 |
| <code>LinqDataSource.AutoPage=true</code> | ✅ 一致 |
| <code>LinqDataSource.AutoSort=false</code> | ✅ 一致 |
| <code>LinqDataSource.AutoSort=true</code> | ✅ 一致 |
| <code>LinqDataSource.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>LinqDataSource.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>LinqDataSource.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>LinqDataSource.ClientIDMode=Static</code> | ✅ 一致 |
| <code>LinqDataSource.ContextTypeName=context</code> | ✅ 一致 |
| <code>LinqDataSource.ContextTypeName=empty</code> | ✅ 一致 |
| <code>LinqDataSource.ContextTypeName=japanese</code> | ✅ 一致 |
| <code>LinqDataSource.ContextTypeName=markup</code> | ✅ 一致 |
| <code>LinqDataSource.ContextTypeName=text</code> | ✅ 一致 |
| <code>LinqDataSource.EnableDelete=false</code> | ✅ 一致 |
| <code>LinqDataSource.EnableDelete=true</code> | ✅ 一致 |
| <code>LinqDataSource.EnableInsert=false</code> | ✅ 一致 |
| <code>LinqDataSource.EnableInsert=true</code> | ✅ 一致 |
| <code>LinqDataSource.EnableObjectTracking=false</code> | ✅ 一致 |
| <code>LinqDataSource.EnableObjectTracking=true</code> | ✅ 一致 |
| <code>LinqDataSource.EnableTheming=false</code> | ✅ 一致 |
| <code>LinqDataSource.EnableTheming=true</code> | ✅ 一致 |
| <code>LinqDataSource.EnableUpdate=false</code> | ✅ 一致 |
| <code>LinqDataSource.EnableUpdate=true</code> | ✅ 一致 |
| <code>LinqDataSource.EnableUpdate=true+EnableDelete=true+EnableInsert=true</code> | ✅ 一致 |
| <code>LinqDataSource.EnableViewState=false</code> | ✅ 一致 |
| <code>LinqDataSource.EnableViewState=true</code> | ✅ 一致 |
| <code>LinqDataSource.EntityTypeName=empty</code> | ✅ 一致 |
| <code>LinqDataSource.EntityTypeName=japanese</code> | ✅ 一致 |
| <code>LinqDataSource.EntityTypeName=markup</code> | ✅ 一致 |
| <code>LinqDataSource.EntityTypeName=text</code> | ✅ 一致 |
| <code>LinqDataSource.GroupBy=empty</code> | ✅ 一致 |
| <code>LinqDataSource.GroupBy=japanese</code> | ✅ 一致 |
| <code>LinqDataSource.GroupBy=markup</code> | ✅ 一致 |
| <code>LinqDataSource.GroupBy=text</code> | ✅ 一致 |
| <code>LinqDataSource.ID=empty</code> | ✅ 一致 |
| <code>LinqDataSource.ID=japanese</code> | ✅ 一致 |
| <code>LinqDataSource.ID=markup</code> | ✅ 一致 |
| <code>LinqDataSource.ID=text</code> | ✅ 一致 |
| <code>LinqDataSource.OrderBy=empty</code> | ✅ 一致 |
| <code>LinqDataSource.OrderBy=japanese</code> | ✅ 一致 |
| <code>LinqDataSource.OrderBy=markup</code> | ✅ 一致 |
| <code>LinqDataSource.OrderBy=text</code> | ✅ 一致 |
| <code>LinqDataSource.OrderGroupsBy=empty</code> | ✅ 一致 |
| <code>LinqDataSource.OrderGroupsBy=japanese</code> | ✅ 一致 |
| <code>LinqDataSource.OrderGroupsBy=markup</code> | ✅ 一致 |
| <code>LinqDataSource.OrderGroupsBy=text</code> | ✅ 一致 |
| <code>LinqDataSource.Select=empty</code> | ✅ 一致 |
| <code>LinqDataSource.Select=japanese</code> | ✅ 一致 |
| <code>LinqDataSource.Select=markup</code> | ✅ 一致 |
| <code>LinqDataSource.Select=text</code> | ✅ 一致 |
| <code>LinqDataSource.SkinID=empty</code> | ✅ 一致 |
| <code>LinqDataSource.SkinID=japanese</code> | ✅ 一致 |
| <code>LinqDataSource.SkinID=markup</code> | ✅ 一致 |
| <code>LinqDataSource.SkinID=text</code> | ✅ 一致 |
| <code>LinqDataSource.StoreOriginalValuesInViewState=false</code> | ✅ 一致 |
| <code>LinqDataSource.StoreOriginalValuesInViewState=false+Select=text</code> | ✅ 一致 |
| <code>LinqDataSource.StoreOriginalValuesInViewState=true</code> | ✅ 一致 |
| <code>LinqDataSource.TableName=Products</code> | ✅ 一致 |
| <code>LinqDataSource.TableName=empty</code> | ✅ 一致 |
| <code>LinqDataSource.TableName=japanese</code> | ✅ 一致 |
| <code>LinqDataSource.TableName=markup</code> | ✅ 一致 |
| <code>LinqDataSource.TableName=text</code> | ✅ 一致 |
| <code>LinqDataSource.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>LinqDataSource.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>LinqDataSource.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>LinqDataSource.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>LinqDataSource.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>LinqDataSource.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>LinqDataSource.Visible=false</code> | ✅ 一致 |
| <code>LinqDataSource.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>LinqDataSource.Visible=true</code> | ✅ 一致 |
| <code>LinqDataSource.Where=empty</code> | ✅ 一致 |
| <code>LinqDataSource.Where=japanese</code> | ✅ 一致 |
| <code>LinqDataSource.Where=markup</code> | ✅ 一致 |
| <code>LinqDataSource.Where=text</code> | ✅ 一致 |
| <code>LinqDataSource.Where=text+OrderBy=text</code> | ✅ 一致 |
| <code>LinqDataSource[naming container].(default)</code> | ✅ 一致 |
| <code>LinqDataSource[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>LinqDataSource[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>LinqDataSource[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>LinqDataSource[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>LinqDataSource[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| LinqDataSource [render][databind][dynamic] first request: grids bound to each data source, the Access data source selected in code, ones made in code | ✅ 一致 |
| LinqDataSource [viewstate] an object data source&#39;s row updated | ✅ 一致 |
| LinqDataSource [viewstate] an object data source&#39;s row deleted | ✅ 一致 |
| LinqDataSource [viewstate] a SQL data source sorted | ✅ 一致 |
| LinqDataSource [viewstate] a SQL data source&#39;s row updated (its update command&#39;s parameters) | ✅ 一致 |
| LinqDataSource [viewstate] a SQL data source&#39;s row deleted | ✅ 一致 |
| LinqDataSource [viewstate] a SQL data source&#39;s row inserted | ✅ 一致 |
| LinqDataSource [viewstate] a LINQ data source paged | ✅ 一致 |
| LinqDataSource [viewstate] a LINQ data source sorted | ✅ 一致 |
| LinqDataSource [viewstate] a LINQ data source&#39;s where parameter from a text box | ✅ 一致 |
| LinqDataSource [viewstate] a query extender&#39;s search from a text box | ✅ 一致 |
| LinqDataSource [viewstate] a model error shown | ✅ 一致 |
| LinqDataSource [viewstate] no model error | ✅ 一致 |
| LinqDataSource [dynamic][viewstate] a grid sorted by a data source made in code | ✅ 一致 |
| LinqDataSource [visible][viewstate] a hidden grid bound to a data source shown | ✅ 一致 |

</details>
