# SqlDataSource パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-15 15:07
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 171 | 171 | 0 | 0 |
| ② HTML | 108 | 108 | 0 | 0 |
| ③ シナリオ | 15 | 15 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 180 件中 180 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 30 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 15 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 100 | 1 | ✅ |
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
| <code>SqlDataSource.Api.Control.#ctor</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.Adapter</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.ClientID</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.Context</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.Controls</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.DataBind</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.Dispose</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.Disposed</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.Events</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.FindControl</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.Focus</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.HasControls</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.ID</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.Init</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.Load</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.OnInit</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.Page</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.Parent</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.PreRender</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.Render</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.Site</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.SkinID</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.Unload</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.ViewState</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>SqlDataSource.Api.Control.Visible</code> | ✅ 一致 |
| <code>SqlDataSource.Api.DataSourceControl.#ctor</code> | ✅ 一致 |
| <code>SqlDataSource.Api.DataSourceControl.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>SqlDataSource.Api.DataSourceControl.ClientID</code> | ✅ 一致 |
| <code>SqlDataSource.Api.DataSourceControl.ClientIDMode</code> | ✅ 一致 |
| <code>SqlDataSource.Api.DataSourceControl.Controls</code> | ✅ 一致 |
| <code>SqlDataSource.Api.DataSourceControl.CreateControlCollection</code> | ✅ 一致 |
| <code>SqlDataSource.Api.DataSourceControl.EnableTheming</code> | ✅ 一致 |
| <code>SqlDataSource.Api.DataSourceControl.FindControl</code> | ✅ 一致 |
| <code>SqlDataSource.Api.DataSourceControl.Focus</code> | ✅ 一致 |
| <code>SqlDataSource.Api.DataSourceControl.GetView</code> | ✅ 一致 |
| <code>SqlDataSource.Api.DataSourceControl.GetViewNames</code> | ✅ 一致 |
| <code>SqlDataSource.Api.DataSourceControl.HasControls</code> | ✅ 一致 |
| <code>SqlDataSource.Api.DataSourceControl.RaiseDataSourceChangedEvent</code> | ✅ 一致 |
| <code>SqlDataSource.Api.DataSourceControl.RenderControl</code> | ✅ 一致 |
| <code>SqlDataSource.Api.DataSourceControl.SkinID</code> | ✅ 一致 |
| <code>SqlDataSource.Api.DataSourceControl.Visible</code> | ✅ 一致 |
| <code>SqlDataSource.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>SqlDataSource.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>SqlDataSource.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>SqlDataSource.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>SqlDataSource.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>SqlDataSource.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>SqlDataSource.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>SqlDataSource.Api.IDataSource.DataSourceChanged</code> | ✅ 一致 |
| <code>SqlDataSource.Api.IDataSource.GetView</code> | ✅ 一致 |
| <code>SqlDataSource.Api.IDataSource.GetViewNames</code> | ✅ 一致 |
| <code>SqlDataSource.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>SqlDataSource.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>SqlDataSource.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>SqlDataSource.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.#ctor</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.CacheDuration</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.CacheExpirationPolicy</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.CacheKeyDependency</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.CancelSelectOnNullParameter</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.ConflictDetection</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.ConnectionString</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.CreateDataSourceView</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.DataSourceMode</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.Delete</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.DeleteCommand</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.DeleteCommandType</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.DeleteParameters</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.Deleted</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.Deleting</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.EnableCaching</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.FilterExpression</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.FilterParameters</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.Filtering</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.GetDbProviderFactory</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.GetView</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.GetViewNames</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.Insert</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.InsertCommand</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.InsertCommandType</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.InsertParameters</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.Inserted</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.Inserting</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.LoadViewState</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.OldValuesParameterFormatString</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.OnInit</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.ProviderName</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.SaveViewState</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.Select</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.SelectCommand</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.SelectCommandType</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.SelectParameters</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.Selected</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.Selecting</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.SortParameterName</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.SqlCacheDependency</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.TrackViewState</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.Update</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.UpdateCommand</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.UpdateCommandType</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.UpdateParameters</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.Updated</code> | ✅ 一致 |
| <code>SqlDataSource.Api.WebControls.SqlDataSource.Updating</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>SqlDataSource.(case list)</code> | ✅ 一致 |
| <code>SqlDataSource.(default)</code> | ✅ 一致 |
| <code>SqlDataSource.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>SqlDataSource.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>SqlDataSource.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>SqlDataSource.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>SqlDataSource.CacheDuration=-1</code> | ✅ 一致 |
| <code>SqlDataSource.CacheDuration=0</code> | ✅ 一致 |
| <code>SqlDataSource.CacheDuration=3</code> | ✅ 一致 |
| <code>SqlDataSource.CacheExpirationPolicy=Absolute</code> | ✅ 一致 |
| <code>SqlDataSource.CacheExpirationPolicy=Sliding</code> | ✅ 一致 |
| <code>SqlDataSource.CacheKeyDependency=empty</code> | ✅ 一致 |
| <code>SqlDataSource.CacheKeyDependency=japanese</code> | ✅ 一致 |
| <code>SqlDataSource.CacheKeyDependency=markup</code> | ✅ 一致 |
| <code>SqlDataSource.CacheKeyDependency=text</code> | ✅ 一致 |
| <code>SqlDataSource.CancelSelectOnNullParameter=false</code> | ✅ 一致 |
| <code>SqlDataSource.CancelSelectOnNullParameter=false+FilterExpression=text</code> | ✅ 一致 |
| <code>SqlDataSource.CancelSelectOnNullParameter=true</code> | ✅ 一致 |
| <code>SqlDataSource.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>SqlDataSource.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>SqlDataSource.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>SqlDataSource.ClientIDMode=Static</code> | ✅ 一致 |
| <code>SqlDataSource.ConflictDetection=CompareAllValues</code> | ✅ 一致 |
| <code>SqlDataSource.ConflictDetection=OverwriteChanges</code> | ✅ 一致 |
| <code>SqlDataSource.ConnectionString=empty</code> | ✅ 一致 |
| <code>SqlDataSource.ConnectionString=japanese</code> | ✅ 一致 |
| <code>SqlDataSource.ConnectionString=markup</code> | ✅ 一致 |
| <code>SqlDataSource.ConnectionString=text</code> | ✅ 一致 |
| <code>SqlDataSource.DataSourceMode=DataReader</code> | ✅ 一致 |
| <code>SqlDataSource.DataSourceMode=DataReader+EnableCaching=true</code> | ✅ 一致 |
| <code>SqlDataSource.DataSourceMode=DataSet</code> | ✅ 一致 |
| <code>SqlDataSource.DeleteCommand=empty</code> | ✅ 一致 |
| <code>SqlDataSource.DeleteCommand=japanese</code> | ✅ 一致 |
| <code>SqlDataSource.DeleteCommand=markup</code> | ✅ 一致 |
| <code>SqlDataSource.DeleteCommand=text</code> | ✅ 一致 |
| <code>SqlDataSource.DeleteCommandType=StoredProcedure</code> | ✅ 一致 |
| <code>SqlDataSource.DeleteCommandType=Text</code> | ✅ 一致 |
| <code>SqlDataSource.EnableCaching=false</code> | ✅ 一致 |
| <code>SqlDataSource.EnableCaching=true</code> | ✅ 一致 |
| <code>SqlDataSource.EnableTheming=false</code> | ✅ 一致 |
| <code>SqlDataSource.EnableTheming=true</code> | ✅ 一致 |
| <code>SqlDataSource.EnableViewState=false</code> | ✅ 一致 |
| <code>SqlDataSource.EnableViewState=true</code> | ✅ 一致 |
| <code>SqlDataSource.FilterExpression=empty</code> | ✅ 一致 |
| <code>SqlDataSource.FilterExpression=japanese</code> | ✅ 一致 |
| <code>SqlDataSource.FilterExpression=markup</code> | ✅ 一致 |
| <code>SqlDataSource.FilterExpression=text</code> | ✅ 一致 |
| <code>SqlDataSource.ID=empty</code> | ✅ 一致 |
| <code>SqlDataSource.ID=japanese</code> | ✅ 一致 |
| <code>SqlDataSource.ID=markup</code> | ✅ 一致 |
| <code>SqlDataSource.ID=text</code> | ✅ 一致 |
| <code>SqlDataSource.InsertCommand=empty</code> | ✅ 一致 |
| <code>SqlDataSource.InsertCommand=japanese</code> | ✅ 一致 |
| <code>SqlDataSource.InsertCommand=markup</code> | ✅ 一致 |
| <code>SqlDataSource.InsertCommand=text</code> | ✅ 一致 |
| <code>SqlDataSource.InsertCommandType=StoredProcedure</code> | ✅ 一致 |
| <code>SqlDataSource.InsertCommandType=Text</code> | ✅ 一致 |
| <code>SqlDataSource.OldValuesParameterFormatString=empty</code> | ✅ 一致 |
| <code>SqlDataSource.OldValuesParameterFormatString=japanese</code> | ✅ 一致 |
| <code>SqlDataSource.OldValuesParameterFormatString=markup</code> | ✅ 一致 |
| <code>SqlDataSource.OldValuesParameterFormatString=text</code> | ✅ 一致 |
| <code>SqlDataSource.ProviderName=empty</code> | ✅ 一致 |
| <code>SqlDataSource.ProviderName=japanese</code> | ✅ 一致 |
| <code>SqlDataSource.ProviderName=markup</code> | ✅ 一致 |
| <code>SqlDataSource.ProviderName=missing</code> | ✅ 一致 |
| <code>SqlDataSource.ProviderName=missing+SelectCommand=select</code> | ✅ 一致 |
| <code>SqlDataSource.ProviderName=probe</code> | ✅ 一致 |
| <code>SqlDataSource.ProviderName=text</code> | ✅ 一致 |
| <code>SqlDataSource.SelectCommand=empty</code> | ✅ 一致 |
| <code>SqlDataSource.SelectCommand=japanese</code> | ✅ 一致 |
| <code>SqlDataSource.SelectCommand=markup</code> | ✅ 一致 |
| <code>SqlDataSource.SelectCommand=select</code> | ✅ 一致 |
| <code>SqlDataSource.SelectCommand=text</code> | ✅ 一致 |
| <code>SqlDataSource.SelectCommandType=StoredProcedure</code> | ✅ 一致 |
| <code>SqlDataSource.SelectCommandType=Text</code> | ✅ 一致 |
| <code>SqlDataSource.SkinID=empty</code> | ✅ 一致 |
| <code>SqlDataSource.SkinID=japanese</code> | ✅ 一致 |
| <code>SqlDataSource.SkinID=markup</code> | ✅ 一致 |
| <code>SqlDataSource.SkinID=text</code> | ✅ 一致 |
| <code>SqlDataSource.SortParameterName=empty</code> | ✅ 一致 |
| <code>SqlDataSource.SortParameterName=japanese</code> | ✅ 一致 |
| <code>SqlDataSource.SortParameterName=markup</code> | ✅ 一致 |
| <code>SqlDataSource.SortParameterName=text</code> | ✅ 一致 |
| <code>SqlDataSource.SqlCacheDependency=empty</code> | ✅ 一致 |
| <code>SqlDataSource.SqlCacheDependency=japanese</code> | ✅ 一致 |
| <code>SqlDataSource.SqlCacheDependency=markup</code> | ✅ 一致 |
| <code>SqlDataSource.SqlCacheDependency=text</code> | ✅ 一致 |
| <code>SqlDataSource.UpdateCommand=empty</code> | ✅ 一致 |
| <code>SqlDataSource.UpdateCommand=japanese</code> | ✅ 一致 |
| <code>SqlDataSource.UpdateCommand=markup</code> | ✅ 一致 |
| <code>SqlDataSource.UpdateCommand=text</code> | ✅ 一致 |
| <code>SqlDataSource.UpdateCommandType=StoredProcedure</code> | ✅ 一致 |
| <code>SqlDataSource.UpdateCommandType=Text</code> | ✅ 一致 |
| <code>SqlDataSource.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>SqlDataSource.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>SqlDataSource.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>SqlDataSource.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>SqlDataSource.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>SqlDataSource.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>SqlDataSource.Visible=false</code> | ✅ 一致 |
| <code>SqlDataSource.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>SqlDataSource.Visible=true</code> | ✅ 一致 |
| <code>SqlDataSource[naming container].(default)</code> | ✅ 一致 |
| <code>SqlDataSource[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>SqlDataSource[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>SqlDataSource[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>SqlDataSource[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>SqlDataSource[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| SqlDataSource [render][databind][dynamic] first request: grids bound to each data source, the Access data source selected in code, ones made in code | ✅ 一致 |
| SqlDataSource [viewstate] an object data source&#39;s row updated | ✅ 一致 |
| SqlDataSource [viewstate] an object data source&#39;s row deleted | ✅ 一致 |
| SqlDataSource [viewstate] a SQL data source sorted | ✅ 一致 |
| SqlDataSource [viewstate] a SQL data source&#39;s row updated (its update command&#39;s parameters) | ✅ 一致 |
| SqlDataSource [viewstate] a SQL data source&#39;s row deleted | ✅ 一致 |
| SqlDataSource [viewstate] a SQL data source&#39;s row inserted | ✅ 一致 |
| SqlDataSource [viewstate] a LINQ data source paged | ✅ 一致 |
| SqlDataSource [viewstate] a LINQ data source sorted | ✅ 一致 |
| SqlDataSource [viewstate] a LINQ data source&#39;s where parameter from a text box | ✅ 一致 |
| SqlDataSource [viewstate] a query extender&#39;s search from a text box | ✅ 一致 |
| SqlDataSource [viewstate] a model error shown | ✅ 一致 |
| SqlDataSource [viewstate] no model error | ✅ 一致 |
| SqlDataSource [dynamic][viewstate] a grid sorted by a data source made in code | ✅ 一致 |
| SqlDataSource [visible][viewstate] a hidden grid bound to a data source shown | ✅ 一致 |

</details>
