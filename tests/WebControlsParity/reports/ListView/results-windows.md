# ListView パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-16 16:39
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 335 | 335 | 0 | 0 |
| ② HTML | 187 | 187 | 0 | 0 |
| ③ シナリオ | 18 | 18 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 345 件中 345 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 47 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 18 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 161 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 3 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 1 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 1 | 0 | ✅ |
| ページング | 0 | 2 | ✅ |
| 並べ替え | 0 | 4 | ✅ |
| 項目の挿入 | 10 | 1 | ✅ |
| 行・項目の編集（編集・更新・取り消し・削除） | 6 | 5 | ✅ |
| 行・項目の選択 | 8 | 2 | ✅ |
| 行・項目のコマンド（RowCommand・ItemCommand へのバブル） | 0 | 1 | ✅ |
| データキー（DataKeyNames・DataKeys） | 4 | 3 | ✅ |
| データソースコントロールとの連携（DataSourceID） | 0 | 7 | ✅ |
| テンプレート | 0 | 1 | ✅ |
| データソース連携（DataSource・DataTextField など） | 2 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>ListView.Api.Control.#ctor</code> | ✅ 一致 |
| <code>ListView.Api.Control.Adapter</code> | ✅ 一致 |
| <code>ListView.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>ListView.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>ListView.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>ListView.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>ListView.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>ListView.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>ListView.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>ListView.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>ListView.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>ListView.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>ListView.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>ListView.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>ListView.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>ListView.Api.Control.ClientID</code> | ✅ 一致 |
| <code>ListView.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>ListView.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>ListView.Api.Control.Context</code> | ✅ 一致 |
| <code>ListView.Api.Control.Controls</code> | ✅ 一致 |
| <code>ListView.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>ListView.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>ListView.Api.Control.DataBind</code> | ✅ 一致 |
| <code>ListView.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>ListView.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>ListView.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>ListView.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>ListView.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>ListView.Api.Control.Dispose</code> | ✅ 一致 |
| <code>ListView.Api.Control.Disposed</code> | ✅ 一致 |
| <code>ListView.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>ListView.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>ListView.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>ListView.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>ListView.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>ListView.Api.Control.Events</code> | ✅ 一致 |
| <code>ListView.Api.Control.FindControl</code> | ✅ 一致 |
| <code>ListView.Api.Control.Focus</code> | ✅ 一致 |
| <code>ListView.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>ListView.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>ListView.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>ListView.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>ListView.Api.Control.HasControls</code> | ✅ 一致 |
| <code>ListView.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>ListView.Api.Control.ID</code> | ✅ 一致 |
| <code>ListView.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>ListView.Api.Control.Init</code> | ✅ 一致 |
| <code>ListView.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>ListView.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>ListView.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>ListView.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>ListView.Api.Control.Load</code> | ✅ 一致 |
| <code>ListView.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>ListView.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>ListView.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>ListView.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>ListView.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>ListView.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>ListView.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>ListView.Api.Control.OnInit</code> | ✅ 一致 |
| <code>ListView.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>ListView.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>ListView.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>ListView.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>ListView.Api.Control.Page</code> | ✅ 一致 |
| <code>ListView.Api.Control.Parent</code> | ✅ 一致 |
| <code>ListView.Api.Control.PreRender</code> | ✅ 一致 |
| <code>ListView.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>ListView.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>ListView.Api.Control.Render</code> | ✅ 一致 |
| <code>ListView.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>ListView.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>ListView.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>ListView.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>ListView.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>ListView.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>ListView.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>ListView.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>ListView.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>ListView.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>ListView.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>ListView.Api.Control.Site</code> | ✅ 一致 |
| <code>ListView.Api.Control.SkinID</code> | ✅ 一致 |
| <code>ListView.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>ListView.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>ListView.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>ListView.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>ListView.Api.Control.Unload</code> | ✅ 一致 |
| <code>ListView.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>ListView.Api.Control.ViewState</code> | ✅ 一致 |
| <code>ListView.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>ListView.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>ListView.Api.Control.Visible</code> | ✅ 一致 |
| <code>ListView.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>ListView.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>ListView.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>ListView.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>ListView.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>ListView.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>ListView.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>ListView.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>ListView.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>ListView.Api.IDataKeysControl.ClientIDRowSuffix</code> | ✅ 一致 |
| <code>ListView.Api.IDataKeysControl.ClientIDRowSuffixDataKeys</code> | ✅ 一致 |
| <code>ListView.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>ListView.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>ListView.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>ListView.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.BaseDataBoundControl.#ctor</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.BaseDataBoundControl.ConfirmInitState</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.BaseDataBoundControl.DataBind</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.BaseDataBoundControl.DataBound</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.BaseDataBoundControl.DataSource</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.BaseDataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.BaseDataBoundControl.EnsureDataBound</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.BaseDataBoundControl.Initialized</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.BaseDataBoundControl.IsBoundUsingDataSourceID</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.BaseDataBoundControl.IsDataBindingAutomatic</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.BaseDataBoundControl.IsUsingModelBinders</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.BaseDataBoundControl.OnDataBound</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.BaseDataBoundControl.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.BaseDataBoundControl.OnInit</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.BaseDataBoundControl.OnPagePreLoad</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.BaseDataBoundControl.OnPreRender</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.BaseDataBoundControl.PerformSelect</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.BaseDataBoundControl.RequiresDataBinding</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.BaseDataBoundControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.BaseDataBoundControl.ValidateDataSource</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.DataBoundControl.#ctor</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.DataBoundControl.CallingDataMethods</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.DataBoundControl.CreateDataSourceSelectArguments</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.DataBoundControl.CreatingModelDataSource</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.DataBoundControl.DataMember</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.DataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.DataBoundControl.DataSourceObject</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.DataBoundControl.GetData</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.DataBoundControl.GetDataSource</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.DataBoundControl.IsUsingModelBinders</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.DataBoundControl.ItemType</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.DataBoundControl.LoadViewState</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.DataBoundControl.MarkAsDataBound</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.DataBoundControl.OnCreatingModelDataSource</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.DataBoundControl.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.DataBoundControl.OnDataSourceViewChanged</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.DataBoundControl.OnLoad</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.DataBoundControl.OnPagePreLoad</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.DataBoundControl.PerformDataBinding</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.DataBoundControl.PerformSelect</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.DataBoundControl.SaveViewState</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.DataBoundControl.SelectArguments</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.DataBoundControl.SelectMethod</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.DataBoundControl.TrackViewState</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.DataBoundControl.ValidateDataSource</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.IDataBoundControl.DataKeyNames</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.IDataBoundControl.DataMember</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.IDataBoundControl.DataSource</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.IDataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.IDataBoundControl.DataSourceObject</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.IDataBoundListControl.ClientIDRowSuffix</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.IDataBoundListControl.DataKeys</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.IDataBoundListControl.EnablePersistedSelection</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.IDataBoundListControl.SelectedDataKey</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.IDataBoundListControl.SelectedIndex</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.IPageableItemContainer.MaximumRows</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.IPageableItemContainer.SetPageProperties</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.IPageableItemContainer.StartRowIndex</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.IPageableItemContainer.TotalRowCountAvailable</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.IPersistedSelector.DataKey</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.#ctor</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.AccessKey</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.AddControlToContainer</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.AlternatingItemTemplate</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.BackColor</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.BorderColor</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.BorderStyle</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.BorderWidth</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.ClientIDRowSuffix</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.ClientIDRowSuffixDataKeys</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.Controls</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.ConvertEmptyStringToNull</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.CreateChildControls</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.CreateControlStyle</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.CreateDataItem</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.CreateDataSourceSelectArguments</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.CreateEmptyDataItem</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.CreateEmptyItem</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.CreateInsertItem</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.CreateItem</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.CreateItemsInGroups</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.CreateItemsWithoutGroups</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.CreateLayoutTemplate</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.CreateSuffixArrayList</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.CssClass</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.DataKeyNames</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.DataKeys</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.DeleteItem</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.DeleteMethod</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.EditIndex</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.EditItem</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.EditItemTemplate</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.EmptyDataTemplate</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.EmptyItemTemplate</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.EnableModelValidation</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.EnablePersistedSelection</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.EnsureLayoutTemplate</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.ExtractItemValues</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.FindPlaceholder</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.Font</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.ForeColor</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.GroupItemCount</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.GroupPlaceholderID</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.GroupSeparatorTemplate</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.GroupTemplate</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.Height</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.InsertItem</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.InsertItemPosition</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.InsertItemTemplate</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.InsertMethod</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.InsertNewItem</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.InstantiateEmptyDataTemplate</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.InstantiateEmptyItemTemplate</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.InstantiateGroupSeparatorTemplate</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.InstantiateGroupTemplate</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.InstantiateInsertItemTemplate</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.InstantiateItemSeparatorTemplate</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.InstantiateItemTemplate</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.IsUsingModelBinders</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.ItemCanceling</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.ItemCommand</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.ItemCreated</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.ItemDataBound</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.ItemDeleted</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.ItemDeleting</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.ItemEditing</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.ItemInserted</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.ItemInserting</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.ItemPlaceholderID</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.ItemSeparatorTemplate</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.ItemTemplate</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.ItemUpdated</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.ItemUpdating</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.Items</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.LayoutCreated</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.LayoutTemplate</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.LoadControlState</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.LoadViewState</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.MaximumRows</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.OnBubbleEvent</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.OnInit</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.OnItemCanceling</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.OnItemCommand</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.OnItemCreated</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.OnItemDataBound</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.OnItemDeleted</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.OnItemDeleting</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.OnItemEditing</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.OnItemInserted</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.OnItemInserting</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.OnItemUpdated</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.OnItemUpdating</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.OnLayoutCreated</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.OnPagePropertiesChanged</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.OnPagePropertiesChanging</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.OnSelectedIndexChanged</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.OnSelectedIndexChanging</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.OnSorted</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.OnSorting</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.OnTotalRowCountAvailable</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.PagePropertiesChanged</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.PagePropertiesChanging</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.PerformDataBinding</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.PerformSelect</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.RemoveItems</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.Render</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.SaveControlState</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.SaveViewState</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.SelectItem</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.SelectedDataKey</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.SelectedIndex</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.SelectedIndexChanged</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.SelectedIndexChanging</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.SelectedItemTemplate</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.SelectedPersistedDataKey</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.SelectedValue</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.SetEditItem</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.SetPageProperties</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.Sort</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.SortDirection</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.SortExpression</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.Sorted</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.Sorting</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.StartRowIndex</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.TabIndex</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.ToolTip</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.UpdateItem</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.UpdateMethod</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.ListView.Width</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>ListView.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>ListView.(case list)</code> | ✅ 一致 |
| <code>ListView.(default)</code> | ✅ 一致 |
| <code>ListView.AccessKey=empty</code> | ✅ 一致 |
| <code>ListView.AccessKey=japanese</code> | ✅ 一致 |
| <code>ListView.AccessKey=markup</code> | ✅ 一致 |
| <code>ListView.AccessKey=text</code> | ✅ 一致 |
| <code>ListView.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>ListView.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>ListView.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>ListView.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>ListView.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>ListView.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>ListView.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>ListView.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>ListView.BackColor=#3366CC</code> | ✅ 一致 |
| <code>ListView.BackColor=Red</code> | ✅ 一致 |
| <code>ListView.BackColor=Transparent</code> | ✅ 一致 |
| <code>ListView.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>ListView.BorderColor=Red</code> | ✅ 一致 |
| <code>ListView.BorderColor=Transparent</code> | ✅ 一致 |
| <code>ListView.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>ListView.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>ListView.BorderStyle=Double</code> | ✅ 一致 |
| <code>ListView.BorderStyle=Groove</code> | ✅ 一致 |
| <code>ListView.BorderStyle=Inset</code> | ✅ 一致 |
| <code>ListView.BorderStyle=None</code> | ✅ 一致 |
| <code>ListView.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>ListView.BorderStyle=Outset</code> | ✅ 一致 |
| <code>ListView.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>ListView.BorderStyle=Solid</code> | ✅ 一致 |
| <code>ListView.BorderWidth=120px</code> | ✅ 一致 |
| <code>ListView.BorderWidth=2em</code> | ✅ 一致 |
| <code>ListView.BorderWidth=50%</code> | ✅ 一致 |
| <code>ListView.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ListView.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ListView.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ListView.ClientIDMode=Static</code> | ✅ 一致 |
| <code>ListView.ClientIDRowSuffix=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>ListView.ConvertEmptyStringToNull=false</code> | ✅ 一致 |
| <code>ListView.ConvertEmptyStringToNull=true</code> | ✅ 一致 |
| <code>ListView.CssClass=empty</code> | ✅ 一致 |
| <code>ListView.CssClass=japanese</code> | ✅ 一致 |
| <code>ListView.CssClass=markup</code> | ✅ 一致 |
| <code>ListView.CssClass=text</code> | ✅ 一致 |
| <code>ListView.DataKeyNames=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>ListView.DataKeyNames=Code</code> | ✅ 一致 |
| <code>ListView.DataMember=empty</code> | ✅ 一致 |
| <code>ListView.DataMember=japanese</code> | ✅ 一致 |
| <code>ListView.DataMember=markup</code> | ✅ 一致 |
| <code>ListView.DataMember=text</code> | ✅ 一致 |
| <code>ListView.DataSourceID=empty</code> | ✅ 一致 |
| <code>ListView.DataSourceID=japanese</code> | ✅ 一致 |
| <code>ListView.DataSourceID=markup</code> | ✅ 一致 |
| <code>ListView.DataSourceID=text</code> | ✅ 一致 |
| <code>ListView.DeleteMethod=empty</code> | ✅ 一致 |
| <code>ListView.DeleteMethod=japanese</code> | ✅ 一致 |
| <code>ListView.DeleteMethod=markup</code> | ✅ 一致 |
| <code>ListView.DeleteMethod=text</code> | ✅ 一致 |
| <code>ListView.EditIndex=-1</code> | ✅ 一致 |
| <code>ListView.EditIndex=0</code> | ✅ 一致 |
| <code>ListView.EditIndex=1</code> | ✅ 一致 |
| <code>ListView.EditIndex=3</code> | ✅ 一致 |
| <code>ListView.EnableModelValidation=false</code> | ✅ 一致 |
| <code>ListView.EnableModelValidation=true</code> | ✅ 一致 |
| <code>ListView.EnablePersistedSelection=false</code> | ✅ 一致 |
| <code>ListView.EnablePersistedSelection=true</code> | ✅ 一致 |
| <code>ListView.EnableTheming=false</code> | ✅ 一致 |
| <code>ListView.EnableTheming=true</code> | ✅ 一致 |
| <code>ListView.EnableViewState=false</code> | ✅ 一致 |
| <code>ListView.EnableViewState=false+SelectedIndex=1</code> | ✅ 一致 |
| <code>ListView.EnableViewState=true</code> | ✅ 一致 |
| <code>ListView.Enabled=false</code> | ✅ 一致 |
| <code>ListView.Enabled=true</code> | ✅ 一致 |
| <code>ListView.Font.Bold=false</code> | ✅ 一致 |
| <code>ListView.Font.Bold=true</code> | ✅ 一致 |
| <code>ListView.Font.Italic=false</code> | ✅ 一致 |
| <code>ListView.Font.Italic=true</code> | ✅ 一致 |
| <code>ListView.Font.Name=empty</code> | ✅ 一致 |
| <code>ListView.Font.Name=japanese</code> | ✅ 一致 |
| <code>ListView.Font.Name=markup</code> | ✅ 一致 |
| <code>ListView.Font.Name=text</code> | ✅ 一致 |
| <code>ListView.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>ListView.Font.Overline=false</code> | ✅ 一致 |
| <code>ListView.Font.Overline=true</code> | ✅ 一致 |
| <code>ListView.Font.Size=12pt</code> | ✅ 一致 |
| <code>ListView.Font.Size=150%</code> | ✅ 一致 |
| <code>ListView.Font.Size=Large</code> | ✅ 一致 |
| <code>ListView.Font.Strikeout=false</code> | ✅ 一致 |
| <code>ListView.Font.Strikeout=true</code> | ✅ 一致 |
| <code>ListView.Font.Underline=false</code> | ✅ 一致 |
| <code>ListView.Font.Underline=true</code> | ✅ 一致 |
| <code>ListView.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>ListView.ForeColor=Red</code> | ✅ 一致 |
| <code>ListView.ForeColor=Transparent</code> | ✅ 一致 |
| <code>ListView.GroupItemCount=-1</code> | ✅ 一致 |
| <code>ListView.GroupItemCount=0</code> | ✅ 一致 |
| <code>ListView.GroupItemCount=1</code> | ✅ 一致 |
| <code>ListView.GroupItemCount=3</code> | ✅ 一致 |
| <code>ListView.GroupPlaceholderID=empty</code> | ✅ 一致 |
| <code>ListView.GroupPlaceholderID=japanese</code> | ✅ 一致 |
| <code>ListView.GroupPlaceholderID=markup</code> | ✅ 一致 |
| <code>ListView.GroupPlaceholderID=text</code> | ✅ 一致 |
| <code>ListView.Height=120px</code> | ✅ 一致 |
| <code>ListView.Height=2em</code> | ✅ 一致 |
| <code>ListView.Height=50%</code> | ✅ 一致 |
| <code>ListView.ID=empty</code> | ✅ 一致 |
| <code>ListView.ID=japanese</code> | ✅ 一致 |
| <code>ListView.ID=markup</code> | ✅ 一致 |
| <code>ListView.ID=text</code> | ✅ 一致 |
| <code>ListView.InsertItemPosition=FirstItem</code> | ✅ 一致 |
| <code>ListView.InsertItemPosition=LastItem</code> | ✅ 一致 |
| <code>ListView.InsertItemPosition=None</code> | ✅ 一致 |
| <code>ListView.InsertMethod=empty</code> | ✅ 一致 |
| <code>ListView.InsertMethod=japanese</code> | ✅ 一致 |
| <code>ListView.InsertMethod=markup</code> | ✅ 一致 |
| <code>ListView.InsertMethod=text</code> | ✅ 一致 |
| <code>ListView.ItemPlaceholderID=empty</code> | ✅ 一致 |
| <code>ListView.ItemPlaceholderID=japanese</code> | ✅ 一致 |
| <code>ListView.ItemPlaceholderID=markup</code> | ✅ 一致 |
| <code>ListView.ItemPlaceholderID=text</code> | ✅ 一致 |
| <code>ListView.ItemType=empty</code> | ✅ 一致 |
| <code>ListView.ItemType=japanese</code> | ✅ 一致 |
| <code>ListView.ItemType=markup</code> | ✅ 一致 |
| <code>ListView.ItemType=text</code> | ✅ 一致 |
| <code>ListView.SelectMethod=empty</code> | ✅ 一致 |
| <code>ListView.SelectMethod=japanese</code> | ✅ 一致 |
| <code>ListView.SelectMethod=markup</code> | ✅ 一致 |
| <code>ListView.SelectMethod=text</code> | ✅ 一致 |
| <code>ListView.SelectedIndex=-1</code> | ✅ 一致 |
| <code>ListView.SelectedIndex=0</code> | ✅ 一致 |
| <code>ListView.SelectedIndex=1</code> | ✅ 一致 |
| <code>ListView.SelectedIndex=3</code> | ✅ 一致 |
| <code>ListView.SkinID=empty</code> | ✅ 一致 |
| <code>ListView.SkinID=japanese</code> | ✅ 一致 |
| <code>ListView.SkinID=markup</code> | ✅ 一致 |
| <code>ListView.SkinID=text</code> | ✅ 一致 |
| <code>ListView.Style[color]=1em</code> | ✅ 一致 |
| <code>ListView.Style[color]=red</code> | ✅ 一致 |
| <code>ListView.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>ListView.Style[margin-left]=red</code> | ✅ 一致 |
| <code>ListView.TabIndex=-1</code> | ✅ 一致 |
| <code>ListView.TabIndex=0</code> | ✅ 一致 |
| <code>ListView.TabIndex=3</code> | ✅ 一致 |
| <code>ListView.ToolTip=empty</code> | ✅ 一致 |
| <code>ListView.ToolTip=japanese</code> | ✅ 一致 |
| <code>ListView.ToolTip=markup</code> | ✅ 一致 |
| <code>ListView.ToolTip=text</code> | ✅ 一致 |
| <code>ListView.UpdateMethod=empty</code> | ✅ 一致 |
| <code>ListView.UpdateMethod=japanese</code> | ✅ 一致 |
| <code>ListView.UpdateMethod=markup</code> | ✅ 一致 |
| <code>ListView.UpdateMethod=text</code> | ✅ 一致 |
| <code>ListView.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>ListView.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>ListView.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>ListView.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>ListView.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>ListView.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>ListView.Visible=false</code> | ✅ 一致 |
| <code>ListView.Visible=false+SelectedIndex=1</code> | ✅ 一致 |
| <code>ListView.Visible=true</code> | ✅ 一致 |
| <code>ListView.Width=120px</code> | ✅ 一致 |
| <code>ListView.Width=2em</code> | ✅ 一致 |
| <code>ListView.Width=50%</code> | ✅ 一致 |
| <code>ListView[editing].(default)</code> | ✅ 一致 |
| <code>ListView[editing].EditIndex=1</code> | ✅ 一致 |
| <code>ListView[editing].EditIndex=1+DataKeyNames=Code</code> | ✅ 一致 |
| <code>ListView[editing].InsertItemPosition=FirstItem</code> | ✅ 一致 |
| <code>ListView[editing].InsertItemPosition=None</code> | ✅ 一致 |
| <code>ListView[editing].SelectedIndex=1</code> | ✅ 一致 |
| <code>ListView[editing].SelectedIndex=1+DataKeyNames=Code</code> | ✅ 一致 |
| <code>ListView[empty].(default)</code> | ✅ 一致 |
| <code>ListView[empty].InsertItemPosition=FirstItem</code> | ✅ 一致 |
| <code>ListView[groups].(default)</code> | ✅ 一致 |
| <code>ListView[groups].GroupItemCount=1</code> | ✅ 一致 |
| <code>ListView[groups].GroupPlaceholderID=text</code> | ✅ 一致 |
| <code>ListView[layout].(default)</code> | ✅ 一致 |
| <code>ListView[layout].ItemPlaceholderID=text</code> | ✅ 一致 |
| <code>ListView[naming container].(default)</code> | ✅ 一致 |
| <code>ListView[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ListView[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ListView[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ListView[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>ListView[naming container].ID=text</code> | ✅ 一致 |
| <code>ListView[themed].(default)</code> | ✅ 一致 |
| <code>ListView[themed].CssClass=text</code> | ✅ 一致 |
| <code>ListView[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>ListView[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| ListView [render][databind][datasource][datasourceid][templates][dynamic] first request | ✅ 一致 |
| ListView [paging] the page&#39;s list view: page 2 (its pager) | ✅ 一致 |
| ListView [sorting] sorted by name | ✅ 一致 |
| ListView [sorting][viewstate] sorted by name again: descending (the page&#39;s) | ✅ 一致 |
| ListView [selection][datakeys] an item selected | ✅ 一致 |
| ListView [rowcommand] a command bubbled to ItemCommand | ✅ 一致 |
| ListView [editing][bind][datakeys] updated: keys, old and new values | ✅ 一致 |
| ListView [editing] edit cancelled | ✅ 一致 |
| ListView [inserting][bind] inserted | ✅ 一致 |
| ListView [editing][datakeys] deleted | ✅ 一致 |
| ListView [paging][datasourceid] the data source&#39;s list view: next page | ✅ 一致 |
| ListView [sorting][datasourceid] sorted by price (the list view&#39;s own direction) | ✅ 一致 |
| ListView [sorting][datasourceid] sorted by price again: descending | ✅ 一致 |
| ListView [selection][datasourceid] an item selected | ✅ 一致 |
| ListView [editing][datasourceid] updated through the data source | ✅ 一致 |
| ListView [editing][datasourceid] deleted through the data source | ✅ 一致 |
| ListView [viewstate] posted (the one without view state is empty) | ✅ 一致 |
| ListView [visible][viewstate] the hidden list view shown | ✅ 一致 |

</details>
