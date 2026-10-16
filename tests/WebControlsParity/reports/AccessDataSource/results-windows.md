# AccessDataSource パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-11 23:08
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 178 | 178 | 0 | 0 |
| ② HTML | 112 | 112 | 0 | 0 |
| ③ シナリオ | 15 | 15 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 188 件中 188 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 31 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 15 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 104 | 1 | ✅ |
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
| <code>AccessDataSource.Api.Control.#ctor</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.Adapter</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.ClientID</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.Context</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.Controls</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.DataBind</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.Dispose</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.Disposed</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.Events</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.FindControl</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.Focus</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.HasControls</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.ID</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.Init</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.Load</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.OnInit</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.Page</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.Parent</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.PreRender</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.Render</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.Site</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.SkinID</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.Unload</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.ViewState</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>AccessDataSource.Api.Control.Visible</code> | ✅ 一致 |
| <code>AccessDataSource.Api.DataSourceControl.#ctor</code> | ✅ 一致 |
| <code>AccessDataSource.Api.DataSourceControl.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>AccessDataSource.Api.DataSourceControl.ClientID</code> | ✅ 一致 |
| <code>AccessDataSource.Api.DataSourceControl.ClientIDMode</code> | ✅ 一致 |
| <code>AccessDataSource.Api.DataSourceControl.Controls</code> | ✅ 一致 |
| <code>AccessDataSource.Api.DataSourceControl.CreateControlCollection</code> | ✅ 一致 |
| <code>AccessDataSource.Api.DataSourceControl.EnableTheming</code> | ✅ 一致 |
| <code>AccessDataSource.Api.DataSourceControl.FindControl</code> | ✅ 一致 |
| <code>AccessDataSource.Api.DataSourceControl.Focus</code> | ✅ 一致 |
| <code>AccessDataSource.Api.DataSourceControl.GetView</code> | ✅ 一致 |
| <code>AccessDataSource.Api.DataSourceControl.GetViewNames</code> | ✅ 一致 |
| <code>AccessDataSource.Api.DataSourceControl.HasControls</code> | ✅ 一致 |
| <code>AccessDataSource.Api.DataSourceControl.RaiseDataSourceChangedEvent</code> | ✅ 一致 |
| <code>AccessDataSource.Api.DataSourceControl.RenderControl</code> | ✅ 一致 |
| <code>AccessDataSource.Api.DataSourceControl.SkinID</code> | ✅ 一致 |
| <code>AccessDataSource.Api.DataSourceControl.Visible</code> | ✅ 一致 |
| <code>AccessDataSource.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>AccessDataSource.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>AccessDataSource.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>AccessDataSource.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>AccessDataSource.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>AccessDataSource.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>AccessDataSource.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>AccessDataSource.Api.IDataSource.DataSourceChanged</code> | ✅ 一致 |
| <code>AccessDataSource.Api.IDataSource.GetView</code> | ✅ 一致 |
| <code>AccessDataSource.Api.IDataSource.GetViewNames</code> | ✅ 一致 |
| <code>AccessDataSource.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>AccessDataSource.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>AccessDataSource.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>AccessDataSource.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.AccessDataSource.#ctor</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.AccessDataSource.ConnectionString</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.AccessDataSource.CreateDataSourceView</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.AccessDataSource.DataFile</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.AccessDataSource.GetDbProviderFactory</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.AccessDataSource.ProviderName</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.AccessDataSource.SqlCacheDependency</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.#ctor</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.CacheDuration</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.CacheExpirationPolicy</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.CacheKeyDependency</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.CancelSelectOnNullParameter</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.ConflictDetection</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.ConnectionString</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.CreateDataSourceView</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.DataSourceMode</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.Delete</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.DeleteCommand</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.DeleteCommandType</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.DeleteParameters</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.Deleted</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.Deleting</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.EnableCaching</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.FilterExpression</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.FilterParameters</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.Filtering</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.GetDbProviderFactory</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.GetView</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.GetViewNames</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.Insert</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.InsertCommand</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.InsertCommandType</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.InsertParameters</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.Inserted</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.Inserting</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.LoadViewState</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.OldValuesParameterFormatString</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.OnInit</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.ProviderName</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.SaveViewState</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.Select</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.SelectCommand</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.SelectCommandType</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.SelectParameters</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.Selected</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.Selecting</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.SortParameterName</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.SqlCacheDependency</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.TrackViewState</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.Update</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.UpdateCommand</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.UpdateCommandType</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.UpdateParameters</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.Updated</code> | ✅ 一致 |
| <code>AccessDataSource.Api.WebControls.SqlDataSource.Updating</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>AccessDataSource.(case list)</code> | ✅ 一致 |
| <code>AccessDataSource.(default)</code> | ✅ 一致 |
| <code>AccessDataSource.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>AccessDataSource.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>AccessDataSource.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>AccessDataSource.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>AccessDataSource.CacheDuration=-1</code> | ✅ 一致 |
| <code>AccessDataSource.CacheDuration=0</code> | ✅ 一致 |
| <code>AccessDataSource.CacheDuration=3</code> | ✅ 一致 |
| <code>AccessDataSource.CacheExpirationPolicy=Absolute</code> | ✅ 一致 |
| <code>AccessDataSource.CacheExpirationPolicy=Sliding</code> | ✅ 一致 |
| <code>AccessDataSource.CacheKeyDependency=empty</code> | ✅ 一致 |
| <code>AccessDataSource.CacheKeyDependency=japanese</code> | ✅ 一致 |
| <code>AccessDataSource.CacheKeyDependency=markup</code> | ✅ 一致 |
| <code>AccessDataSource.CacheKeyDependency=text</code> | ✅ 一致 |
| <code>AccessDataSource.CancelSelectOnNullParameter=false</code> | ✅ 一致 |
| <code>AccessDataSource.CancelSelectOnNullParameter=true</code> | ✅ 一致 |
| <code>AccessDataSource.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>AccessDataSource.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>AccessDataSource.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>AccessDataSource.ClientIDMode=Static</code> | ✅ 一致 |
| <code>AccessDataSource.ConflictDetection=CompareAllValues</code> | ✅ 一致 |
| <code>AccessDataSource.ConflictDetection=OverwriteChanges</code> | ✅ 一致 |
| <code>AccessDataSource.ConnectionString=empty</code> | ✅ 一致 |
| <code>AccessDataSource.ConnectionString=japanese</code> | ✅ 一致 |
| <code>AccessDataSource.ConnectionString=markup</code> | ✅ 一致 |
| <code>AccessDataSource.ConnectionString=text</code> | ✅ 一致 |
| <code>AccessDataSource.DataFile=app-relative</code> | ✅ 一致 |
| <code>AccessDataSource.DataFile=empty</code> | ✅ 一致 |
| <code>AccessDataSource.DataFile=japanese</code> | ✅ 一致 |
| <code>AccessDataSource.DataFile=markup</code> | ✅ 一致 |
| <code>AccessDataSource.DataFile=missing</code> | ✅ 一致 |
| <code>AccessDataSource.DataFile=text</code> | ✅ 一致 |
| <code>AccessDataSource.DataSourceMode=DataReader</code> | ✅ 一致 |
| <code>AccessDataSource.DataSourceMode=DataReader+EnableCaching=true</code> | ✅ 一致 |
| <code>AccessDataSource.DataSourceMode=DataSet</code> | ✅ 一致 |
| <code>AccessDataSource.DeleteCommand=empty</code> | ✅ 一致 |
| <code>AccessDataSource.DeleteCommand=japanese</code> | ✅ 一致 |
| <code>AccessDataSource.DeleteCommand=markup</code> | ✅ 一致 |
| <code>AccessDataSource.DeleteCommand=text</code> | ✅ 一致 |
| <code>AccessDataSource.DeleteCommandType=StoredProcedure</code> | ✅ 一致 |
| <code>AccessDataSource.DeleteCommandType=Text</code> | ✅ 一致 |
| <code>AccessDataSource.EnableCaching=false</code> | ✅ 一致 |
| <code>AccessDataSource.EnableCaching=true</code> | ✅ 一致 |
| <code>AccessDataSource.EnableTheming=false</code> | ✅ 一致 |
| <code>AccessDataSource.EnableTheming=true</code> | ✅ 一致 |
| <code>AccessDataSource.EnableViewState=false</code> | ✅ 一致 |
| <code>AccessDataSource.EnableViewState=true</code> | ✅ 一致 |
| <code>AccessDataSource.FilterExpression=empty</code> | ✅ 一致 |
| <code>AccessDataSource.FilterExpression=japanese</code> | ✅ 一致 |
| <code>AccessDataSource.FilterExpression=markup</code> | ✅ 一致 |
| <code>AccessDataSource.FilterExpression=text</code> | ✅ 一致 |
| <code>AccessDataSource.ID=empty</code> | ✅ 一致 |
| <code>AccessDataSource.ID=japanese</code> | ✅ 一致 |
| <code>AccessDataSource.ID=markup</code> | ✅ 一致 |
| <code>AccessDataSource.ID=text</code> | ✅ 一致 |
| <code>AccessDataSource.InsertCommand=empty</code> | ✅ 一致 |
| <code>AccessDataSource.InsertCommand=japanese</code> | ✅ 一致 |
| <code>AccessDataSource.InsertCommand=markup</code> | ✅ 一致 |
| <code>AccessDataSource.InsertCommand=text</code> | ✅ 一致 |
| <code>AccessDataSource.InsertCommandType=StoredProcedure</code> | ✅ 一致 |
| <code>AccessDataSource.InsertCommandType=Text</code> | ✅ 一致 |
| <code>AccessDataSource.OldValuesParameterFormatString=empty</code> | ✅ 一致 |
| <code>AccessDataSource.OldValuesParameterFormatString=japanese</code> | ✅ 一致 |
| <code>AccessDataSource.OldValuesParameterFormatString=markup</code> | ✅ 一致 |
| <code>AccessDataSource.OldValuesParameterFormatString=text</code> | ✅ 一致 |
| <code>AccessDataSource.ProviderName=empty</code> | ✅ 一致 |
| <code>AccessDataSource.ProviderName=japanese</code> | ✅ 一致 |
| <code>AccessDataSource.ProviderName=markup</code> | ✅ 一致 |
| <code>AccessDataSource.ProviderName=missing</code> | ✅ 一致 |
| <code>AccessDataSource.ProviderName=probe</code> | ✅ 一致 |
| <code>AccessDataSource.ProviderName=text</code> | ✅ 一致 |
| <code>AccessDataSource.SelectCommand=empty</code> | ✅ 一致 |
| <code>AccessDataSource.SelectCommand=japanese</code> | ✅ 一致 |
| <code>AccessDataSource.SelectCommand=markup</code> | ✅ 一致 |
| <code>AccessDataSource.SelectCommand=select</code> | ✅ 一致 |
| <code>AccessDataSource.SelectCommand=text</code> | ✅ 一致 |
| <code>AccessDataSource.SelectCommandType=StoredProcedure</code> | ✅ 一致 |
| <code>AccessDataSource.SelectCommandType=Text</code> | ✅ 一致 |
| <code>AccessDataSource.SkinID=empty</code> | ✅ 一致 |
| <code>AccessDataSource.SkinID=japanese</code> | ✅ 一致 |
| <code>AccessDataSource.SkinID=markup</code> | ✅ 一致 |
| <code>AccessDataSource.SkinID=text</code> | ✅ 一致 |
| <code>AccessDataSource.SortParameterName=empty</code> | ✅ 一致 |
| <code>AccessDataSource.SortParameterName=japanese</code> | ✅ 一致 |
| <code>AccessDataSource.SortParameterName=markup</code> | ✅ 一致 |
| <code>AccessDataSource.SortParameterName=text</code> | ✅ 一致 |
| <code>AccessDataSource.SqlCacheDependency=empty</code> | ✅ 一致 |
| <code>AccessDataSource.SqlCacheDependency=japanese</code> | ✅ 一致 |
| <code>AccessDataSource.SqlCacheDependency=markup</code> | ✅ 一致 |
| <code>AccessDataSource.SqlCacheDependency=text</code> | ✅ 一致 |
| <code>AccessDataSource.UpdateCommand=empty</code> | ✅ 一致 |
| <code>AccessDataSource.UpdateCommand=japanese</code> | ✅ 一致 |
| <code>AccessDataSource.UpdateCommand=markup</code> | ✅ 一致 |
| <code>AccessDataSource.UpdateCommand=text</code> | ✅ 一致 |
| <code>AccessDataSource.UpdateCommandType=StoredProcedure</code> | ✅ 一致 |
| <code>AccessDataSource.UpdateCommandType=Text</code> | ✅ 一致 |
| <code>AccessDataSource.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>AccessDataSource.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>AccessDataSource.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>AccessDataSource.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>AccessDataSource.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>AccessDataSource.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>AccessDataSource.Visible=false</code> | ✅ 一致 |
| <code>AccessDataSource.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>AccessDataSource.Visible=true</code> | ✅ 一致 |
| <code>AccessDataSource[naming container].(default)</code> | ✅ 一致 |
| <code>AccessDataSource[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>AccessDataSource[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>AccessDataSource[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>AccessDataSource[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>AccessDataSource[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| AccessDataSource [render][databind][dynamic] first request: grids bound to each data source, the Access data source selected in code, ones made in code | ✅ 一致 |
| AccessDataSource [viewstate] an object data source&#39;s row updated | ✅ 一致 |
| AccessDataSource [viewstate] an object data source&#39;s row deleted | ✅ 一致 |
| AccessDataSource [viewstate] a SQL data source sorted | ✅ 一致 |
| AccessDataSource [viewstate] a SQL data source&#39;s row updated (its update command&#39;s parameters) | ✅ 一致 |
| AccessDataSource [viewstate] a SQL data source&#39;s row deleted | ✅ 一致 |
| AccessDataSource [viewstate] a SQL data source&#39;s row inserted | ✅ 一致 |
| AccessDataSource [viewstate] a LINQ data source paged | ✅ 一致 |
| AccessDataSource [viewstate] a LINQ data source sorted | ✅ 一致 |
| AccessDataSource [viewstate] a LINQ data source&#39;s where parameter from a text box | ✅ 一致 |
| AccessDataSource [viewstate] a query extender&#39;s search from a text box | ✅ 一致 |
| AccessDataSource [viewstate] a model error shown | ✅ 一致 |
| AccessDataSource [viewstate] no model error | ✅ 一致 |
| AccessDataSource [dynamic][viewstate] a grid sorted by a data source made in code | ✅ 一致 |
| AccessDataSource [visible][viewstate] a hidden grid bound to a data source shown | ✅ 一致 |

</details>
