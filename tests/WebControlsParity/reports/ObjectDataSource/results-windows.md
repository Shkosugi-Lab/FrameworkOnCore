# ObjectDataSource パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 00:32
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 172 | 172 | 0 | 0 |
| ② HTML | 115 | 115 | 0 | 0 |
| ③ シナリオ | 15 | 15 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 180 件中 180 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 30 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 15 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 107 | 1 | ✅ |
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
| <code>ObjectDataSource.Api.Control.#ctor</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.Adapter</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.ClientID</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.Context</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.Controls</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.DataBind</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.Dispose</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.Disposed</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.Events</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.FindControl</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.Focus</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.HasControls</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.ID</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.Init</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.Load</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.OnInit</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.Page</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.Parent</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.PreRender</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.Render</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.Site</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.SkinID</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.Unload</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.ViewState</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.Control.Visible</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.DataSourceControl.#ctor</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.DataSourceControl.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.DataSourceControl.ClientID</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.DataSourceControl.ClientIDMode</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.DataSourceControl.Controls</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.DataSourceControl.CreateControlCollection</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.DataSourceControl.EnableTheming</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.DataSourceControl.FindControl</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.DataSourceControl.Focus</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.DataSourceControl.GetView</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.DataSourceControl.GetViewNames</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.DataSourceControl.HasControls</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.DataSourceControl.RaiseDataSourceChangedEvent</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.DataSourceControl.RenderControl</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.DataSourceControl.SkinID</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.DataSourceControl.Visible</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.IDataSource.DataSourceChanged</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.IDataSource.GetView</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.IDataSource.GetViewNames</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.#ctor</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.CacheDuration</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.CacheExpirationPolicy</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.CacheKeyDependency</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.ConflictDetection</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.ConvertNullToDBNull</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.DataObjectTypeName</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.Delete</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.DeleteMethod</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.DeleteParameters</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.Deleted</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.Deleting</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.EnableCaching</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.EnablePaging</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.FilterExpression</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.FilterParameters</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.Filtering</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.GetView</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.GetViewNames</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.Insert</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.InsertMethod</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.InsertParameters</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.Inserted</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.Inserting</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.LoadViewState</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.MaximumRowsParameterName</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.ObjectCreated</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.ObjectCreating</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.ObjectDisposing</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.OldValuesParameterFormatString</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.OnInit</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.ParsingCulture</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.SaveViewState</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.Select</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.SelectCountMethod</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.SelectMethod</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.SelectParameters</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.Selected</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.Selecting</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.SortParameterName</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.SqlCacheDependency</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.StartRowIndexParameterName</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.TrackViewState</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.TypeName</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.Update</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.UpdateMethod</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.UpdateParameters</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.Updated</code> | ✅ 一致 |
| <code>ObjectDataSource.Api.WebControls.ObjectDataSource.Updating</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>ObjectDataSource.(case list)</code> | ✅ 一致 |
| <code>ObjectDataSource.(default)</code> | ✅ 一致 |
| <code>ObjectDataSource.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>ObjectDataSource.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>ObjectDataSource.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>ObjectDataSource.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>ObjectDataSource.CacheDuration=-1</code> | ✅ 一致 |
| <code>ObjectDataSource.CacheDuration=0</code> | ✅ 一致 |
| <code>ObjectDataSource.CacheDuration=3</code> | ✅ 一致 |
| <code>ObjectDataSource.CacheExpirationPolicy=Absolute</code> | ✅ 一致 |
| <code>ObjectDataSource.CacheExpirationPolicy=Sliding</code> | ✅ 一致 |
| <code>ObjectDataSource.CacheKeyDependency=empty</code> | ✅ 一致 |
| <code>ObjectDataSource.CacheKeyDependency=japanese</code> | ✅ 一致 |
| <code>ObjectDataSource.CacheKeyDependency=markup</code> | ✅ 一致 |
| <code>ObjectDataSource.CacheKeyDependency=text</code> | ✅ 一致 |
| <code>ObjectDataSource.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ObjectDataSource.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ObjectDataSource.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ObjectDataSource.ClientIDMode=Static</code> | ✅ 一致 |
| <code>ObjectDataSource.ConflictDetection=CompareAllValues</code> | ✅ 一致 |
| <code>ObjectDataSource.ConflictDetection=CompareAllValues+OldValuesParameterFormatString=text</code> | ✅ 一致 |
| <code>ObjectDataSource.ConflictDetection=OverwriteChanges</code> | ✅ 一致 |
| <code>ObjectDataSource.ConvertNullToDBNull=false</code> | ✅ 一致 |
| <code>ObjectDataSource.ConvertNullToDBNull=true</code> | ✅ 一致 |
| <code>ObjectDataSource.DataObjectTypeName=empty</code> | ✅ 一致 |
| <code>ObjectDataSource.DataObjectTypeName=japanese</code> | ✅ 一致 |
| <code>ObjectDataSource.DataObjectTypeName=markup</code> | ✅ 一致 |
| <code>ObjectDataSource.DataObjectTypeName=text</code> | ✅ 一致 |
| <code>ObjectDataSource.DeleteMethod=empty</code> | ✅ 一致 |
| <code>ObjectDataSource.DeleteMethod=japanese</code> | ✅ 一致 |
| <code>ObjectDataSource.DeleteMethod=markup</code> | ✅ 一致 |
| <code>ObjectDataSource.DeleteMethod=text</code> | ✅ 一致 |
| <code>ObjectDataSource.EnableCaching=false</code> | ✅ 一致 |
| <code>ObjectDataSource.EnableCaching=true</code> | ✅ 一致 |
| <code>ObjectDataSource.EnableCaching=true+CacheDuration=3</code> | ✅ 一致 |
| <code>ObjectDataSource.EnablePaging=false</code> | ✅ 一致 |
| <code>ObjectDataSource.EnablePaging=true</code> | ✅ 一致 |
| <code>ObjectDataSource.EnablePaging=true+SelectCountMethod=text</code> | ✅ 一致 |
| <code>ObjectDataSource.EnableTheming=false</code> | ✅ 一致 |
| <code>ObjectDataSource.EnableTheming=true</code> | ✅ 一致 |
| <code>ObjectDataSource.EnableViewState=false</code> | ✅ 一致 |
| <code>ObjectDataSource.EnableViewState=true</code> | ✅ 一致 |
| <code>ObjectDataSource.FilterExpression=empty</code> | ✅ 一致 |
| <code>ObjectDataSource.FilterExpression=japanese</code> | ✅ 一致 |
| <code>ObjectDataSource.FilterExpression=markup</code> | ✅ 一致 |
| <code>ObjectDataSource.FilterExpression=text</code> | ✅ 一致 |
| <code>ObjectDataSource.ID=empty</code> | ✅ 一致 |
| <code>ObjectDataSource.ID=japanese</code> | ✅ 一致 |
| <code>ObjectDataSource.ID=markup</code> | ✅ 一致 |
| <code>ObjectDataSource.ID=text</code> | ✅ 一致 |
| <code>ObjectDataSource.InsertMethod=empty</code> | ✅ 一致 |
| <code>ObjectDataSource.InsertMethod=japanese</code> | ✅ 一致 |
| <code>ObjectDataSource.InsertMethod=markup</code> | ✅ 一致 |
| <code>ObjectDataSource.InsertMethod=text</code> | ✅ 一致 |
| <code>ObjectDataSource.MaximumRowsParameterName=empty</code> | ✅ 一致 |
| <code>ObjectDataSource.MaximumRowsParameterName=japanese</code> | ✅ 一致 |
| <code>ObjectDataSource.MaximumRowsParameterName=markup</code> | ✅ 一致 |
| <code>ObjectDataSource.MaximumRowsParameterName=text</code> | ✅ 一致 |
| <code>ObjectDataSource.OldValuesParameterFormatString=empty</code> | ✅ 一致 |
| <code>ObjectDataSource.OldValuesParameterFormatString=japanese</code> | ✅ 一致 |
| <code>ObjectDataSource.OldValuesParameterFormatString=markup</code> | ✅ 一致 |
| <code>ObjectDataSource.OldValuesParameterFormatString=text</code> | ✅ 一致 |
| <code>ObjectDataSource.ParsingCulture=Current</code> | ✅ 一致 |
| <code>ObjectDataSource.ParsingCulture=Invariant</code> | ✅ 一致 |
| <code>ObjectDataSource.SelectCountMethod=empty</code> | ✅ 一致 |
| <code>ObjectDataSource.SelectCountMethod=japanese</code> | ✅ 一致 |
| <code>ObjectDataSource.SelectCountMethod=markup</code> | ✅ 一致 |
| <code>ObjectDataSource.SelectCountMethod=text</code> | ✅ 一致 |
| <code>ObjectDataSource.SelectMethod=SelectAll</code> | ✅ 一致 |
| <code>ObjectDataSource.SelectMethod=empty</code> | ✅ 一致 |
| <code>ObjectDataSource.SelectMethod=japanese</code> | ✅ 一致 |
| <code>ObjectDataSource.SelectMethod=markup</code> | ✅ 一致 |
| <code>ObjectDataSource.SelectMethod=text</code> | ✅ 一致 |
| <code>ObjectDataSource.SkinID=empty</code> | ✅ 一致 |
| <code>ObjectDataSource.SkinID=japanese</code> | ✅ 一致 |
| <code>ObjectDataSource.SkinID=markup</code> | ✅ 一致 |
| <code>ObjectDataSource.SkinID=text</code> | ✅ 一致 |
| <code>ObjectDataSource.SortParameterName=empty</code> | ✅ 一致 |
| <code>ObjectDataSource.SortParameterName=japanese</code> | ✅ 一致 |
| <code>ObjectDataSource.SortParameterName=markup</code> | ✅ 一致 |
| <code>ObjectDataSource.SortParameterName=text</code> | ✅ 一致 |
| <code>ObjectDataSource.SqlCacheDependency=empty</code> | ✅ 一致 |
| <code>ObjectDataSource.SqlCacheDependency=japanese</code> | ✅ 一致 |
| <code>ObjectDataSource.SqlCacheDependency=markup</code> | ✅ 一致 |
| <code>ObjectDataSource.SqlCacheDependency=text</code> | ✅ 一致 |
| <code>ObjectDataSource.StartRowIndexParameterName=empty</code> | ✅ 一致 |
| <code>ObjectDataSource.StartRowIndexParameterName=japanese</code> | ✅ 一致 |
| <code>ObjectDataSource.StartRowIndexParameterName=markup</code> | ✅ 一致 |
| <code>ObjectDataSource.StartRowIndexParameterName=text</code> | ✅ 一致 |
| <code>ObjectDataSource.TypeName=empty</code> | ✅ 一致 |
| <code>ObjectDataSource.TypeName=japanese</code> | ✅ 一致 |
| <code>ObjectDataSource.TypeName=markup</code> | ✅ 一致 |
| <code>ObjectDataSource.TypeName=missing</code> | ✅ 一致 |
| <code>ObjectDataSource.TypeName=missing+SelectMethod=SelectAll</code> | ✅ 一致 |
| <code>ObjectDataSource.TypeName=store</code> | ✅ 一致 |
| <code>ObjectDataSource.TypeName=text</code> | ✅ 一致 |
| <code>ObjectDataSource.UpdateMethod=empty</code> | ✅ 一致 |
| <code>ObjectDataSource.UpdateMethod=japanese</code> | ✅ 一致 |
| <code>ObjectDataSource.UpdateMethod=markup</code> | ✅ 一致 |
| <code>ObjectDataSource.UpdateMethod=text</code> | ✅ 一致 |
| <code>ObjectDataSource.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>ObjectDataSource.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>ObjectDataSource.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>ObjectDataSource.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>ObjectDataSource.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>ObjectDataSource.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>ObjectDataSource.Visible=false</code> | ✅ 一致 |
| <code>ObjectDataSource.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>ObjectDataSource.Visible=true</code> | ✅ 一致 |
| <code>ObjectDataSource[naming container].(default)</code> | ✅ 一致 |
| <code>ObjectDataSource[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ObjectDataSource[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ObjectDataSource[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ObjectDataSource[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>ObjectDataSource[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| ObjectDataSource [render][databind][dynamic] first request: grids bound to each data source, the Access data source selected in code, ones made in code | ✅ 一致 |
| ObjectDataSource [viewstate] an object data source&#39;s row updated | ✅ 一致 |
| ObjectDataSource [viewstate] an object data source&#39;s row deleted | ✅ 一致 |
| ObjectDataSource [viewstate] a SQL data source sorted | ✅ 一致 |
| ObjectDataSource [viewstate] a SQL data source&#39;s row updated (its update command&#39;s parameters) | ✅ 一致 |
| ObjectDataSource [viewstate] a SQL data source&#39;s row deleted | ✅ 一致 |
| ObjectDataSource [viewstate] a SQL data source&#39;s row inserted | ✅ 一致 |
| ObjectDataSource [viewstate] a LINQ data source paged | ✅ 一致 |
| ObjectDataSource [viewstate] a LINQ data source sorted | ✅ 一致 |
| ObjectDataSource [viewstate] a LINQ data source&#39;s where parameter from a text box | ✅ 一致 |
| ObjectDataSource [viewstate] a query extender&#39;s search from a text box | ✅ 一致 |
| ObjectDataSource [viewstate] a model error shown | ✅ 一致 |
| ObjectDataSource [viewstate] no model error | ✅ 一致 |
| ObjectDataSource [dynamic][viewstate] a grid sorted by a data source made in code | ✅ 一致 |
| ObjectDataSource [visible][viewstate] a hidden grid bound to a data source shown | ✅ 一致 |

</details>
