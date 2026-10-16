# GridView パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-16 16:35
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 347 | 347 | 0 | 0 |
| ② HTML | 408 | 408 | 0 | 0 |
| ③ シナリオ | 20 | 20 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 357 件中 357 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 173 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 20 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 375 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 3 | ✅ |
| Visible=false（描画されず状態は保持） | 4 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 9 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 1 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 3 | 0 | ✅ |
| ポストバックイベント | 0 | 1 | ✅ |
| ページング | 23 | 3 | ✅ |
| 並べ替え | 4 | 5 | ✅ |
| 行・項目の編集（編集・更新・取り消し・削除） | 8 | 6 | ✅ |
| 行・項目の選択 | 8 | 2 | ✅ |
| 行・項目のコマンド（RowCommand・ItemCommand へのバブル） | 0 | 1 | ✅ |
| データキー（DataKeyNames・DataKeys） | 6 | 3 | ✅ |
| データソースコントロールとの連携（DataSourceID） | 0 | 7 | ✅ |
| テンプレート | 0 | 1 | ✅ |
| データソース連携（DataSource・DataTextField など） | 18 | 1 | ✅ |
| クライアントコールバック | 0 | 2 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>GridView.Api.Control.#ctor</code> | ✅ 一致 |
| <code>GridView.Api.Control.Adapter</code> | ✅ 一致 |
| <code>GridView.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>GridView.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>GridView.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>GridView.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>GridView.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>GridView.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>GridView.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>GridView.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>GridView.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>GridView.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>GridView.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>GridView.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>GridView.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>GridView.Api.Control.ClientID</code> | ✅ 一致 |
| <code>GridView.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>GridView.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>GridView.Api.Control.Context</code> | ✅ 一致 |
| <code>GridView.Api.Control.Controls</code> | ✅ 一致 |
| <code>GridView.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>GridView.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>GridView.Api.Control.DataBind</code> | ✅ 一致 |
| <code>GridView.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>GridView.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>GridView.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>GridView.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>GridView.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>GridView.Api.Control.Dispose</code> | ✅ 一致 |
| <code>GridView.Api.Control.Disposed</code> | ✅ 一致 |
| <code>GridView.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>GridView.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>GridView.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>GridView.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>GridView.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>GridView.Api.Control.Events</code> | ✅ 一致 |
| <code>GridView.Api.Control.FindControl</code> | ✅ 一致 |
| <code>GridView.Api.Control.Focus</code> | ✅ 一致 |
| <code>GridView.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>GridView.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>GridView.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>GridView.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>GridView.Api.Control.HasControls</code> | ✅ 一致 |
| <code>GridView.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>GridView.Api.Control.ID</code> | ✅ 一致 |
| <code>GridView.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>GridView.Api.Control.Init</code> | ✅ 一致 |
| <code>GridView.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>GridView.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>GridView.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>GridView.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>GridView.Api.Control.Load</code> | ✅ 一致 |
| <code>GridView.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>GridView.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>GridView.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>GridView.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>GridView.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>GridView.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>GridView.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>GridView.Api.Control.OnInit</code> | ✅ 一致 |
| <code>GridView.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>GridView.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>GridView.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>GridView.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>GridView.Api.Control.Page</code> | ✅ 一致 |
| <code>GridView.Api.Control.Parent</code> | ✅ 一致 |
| <code>GridView.Api.Control.PreRender</code> | ✅ 一致 |
| <code>GridView.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>GridView.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>GridView.Api.Control.Render</code> | ✅ 一致 |
| <code>GridView.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>GridView.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>GridView.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>GridView.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>GridView.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>GridView.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>GridView.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>GridView.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>GridView.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>GridView.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>GridView.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>GridView.Api.Control.Site</code> | ✅ 一致 |
| <code>GridView.Api.Control.SkinID</code> | ✅ 一致 |
| <code>GridView.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>GridView.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>GridView.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>GridView.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>GridView.Api.Control.Unload</code> | ✅ 一致 |
| <code>GridView.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>GridView.Api.Control.ViewState</code> | ✅ 一致 |
| <code>GridView.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>GridView.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>GridView.Api.Control.Visible</code> | ✅ 一致 |
| <code>GridView.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>GridView.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>GridView.Api.ICallbackEventHandler.GetCallbackResult</code> | ✅ 一致 |
| <code>GridView.Api.ICallbackEventHandler.RaiseCallbackEvent</code> | ✅ 一致 |
| <code>GridView.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>GridView.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>GridView.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>GridView.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>GridView.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>GridView.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>GridView.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>GridView.Api.IDataKeysControl.ClientIDRowSuffix</code> | ✅ 一致 |
| <code>GridView.Api.IDataKeysControl.ClientIDRowSuffixDataKeys</code> | ✅ 一致 |
| <code>GridView.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>GridView.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>GridView.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>GridView.Api.IPostBackEventHandler.RaisePostBackEvent</code> | ✅ 一致 |
| <code>GridView.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.BaseDataBoundControl.#ctor</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.BaseDataBoundControl.ConfirmInitState</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.BaseDataBoundControl.DataBind</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.BaseDataBoundControl.DataBound</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.BaseDataBoundControl.DataSource</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.BaseDataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.BaseDataBoundControl.EnsureDataBound</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.BaseDataBoundControl.Initialized</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.BaseDataBoundControl.IsBoundUsingDataSourceID</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.BaseDataBoundControl.IsDataBindingAutomatic</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.BaseDataBoundControl.IsUsingModelBinders</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.BaseDataBoundControl.OnDataBound</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.BaseDataBoundControl.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.BaseDataBoundControl.OnInit</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.BaseDataBoundControl.OnPagePreLoad</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.BaseDataBoundControl.OnPreRender</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.BaseDataBoundControl.PerformSelect</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.BaseDataBoundControl.RequiresDataBinding</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.BaseDataBoundControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.BaseDataBoundControl.ValidateDataSource</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.CompositeDataBoundControl.#ctor</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.CompositeDataBoundControl.Controls</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.CompositeDataBoundControl.CreateChildControls</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.CompositeDataBoundControl.DeleteMethod</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.CompositeDataBoundControl.InsertMethod</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.CompositeDataBoundControl.IsUsingModelBinders</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.CompositeDataBoundControl.PerformDataBinding</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.CompositeDataBoundControl.UpdateMethod</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.DataBoundControl.#ctor</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.DataBoundControl.CallingDataMethods</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.DataBoundControl.CreateDataSourceSelectArguments</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.DataBoundControl.CreatingModelDataSource</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.DataBoundControl.DataMember</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.DataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.DataBoundControl.DataSourceObject</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.DataBoundControl.GetData</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.DataBoundControl.GetDataSource</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.DataBoundControl.IsUsingModelBinders</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.DataBoundControl.ItemType</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.DataBoundControl.LoadViewState</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.DataBoundControl.MarkAsDataBound</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.DataBoundControl.OnCreatingModelDataSource</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.DataBoundControl.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.DataBoundControl.OnDataSourceViewChanged</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.DataBoundControl.OnLoad</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.DataBoundControl.OnPagePreLoad</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.DataBoundControl.PerformDataBinding</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.DataBoundControl.PerformSelect</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.DataBoundControl.SaveViewState</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.DataBoundControl.SelectArguments</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.DataBoundControl.SelectMethod</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.DataBoundControl.TrackViewState</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.DataBoundControl.ValidateDataSource</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.#ctor</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.AllowCustomPaging</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.AllowPaging</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.AllowSorting</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.AlternatingRowStyle</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.AutoGenerateColumns</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.AutoGenerateDeleteButton</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.AutoGenerateEditButton</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.AutoGenerateSelectButton</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.BackImageUrl</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.BottomPagerRow</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.Caption</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.CaptionAlign</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.CellPadding</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.CellSpacing</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.ClientIDRowSuffix</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.ClientIDRowSuffixDataKeys</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.Columns</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.ColumnsGenerator</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.CreateAutoGeneratedColumn</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.CreateChildControls</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.CreateChildTable</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.CreateColumns</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.CreateControlStyle</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.CreateDataSourceSelectArguments</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.CreateRow</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.DataBind</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.DataKeyNames</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.DataKeys</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.DeleteMethod</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.DeleteRow</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.EditIndex</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.EditRowStyle</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.EmptyDataRowStyle</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.EmptyDataTemplate</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.EmptyDataText</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.EnableModelValidation</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.EnablePersistedSelection</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.EnableSortingAndPagingCallbacks</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.ExtractRowValues</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.FooterRow</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.FooterStyle</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.GetCallbackResult</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.GetCallbackScript</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.GridLines</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.HeaderRow</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.HeaderStyle</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.HorizontalAlign</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.InitializePager</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.InitializeRow</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.IsBindableType</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.LoadControlState</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.LoadViewState</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.OnBubbleEvent</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.OnDataSourceViewChanged</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.OnInit</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.OnPageIndexChanged</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.OnPageIndexChanging</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.OnPagePreLoad</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.OnPreRender</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.OnRowCancelingEdit</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.OnRowCommand</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.OnRowCreated</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.OnRowDataBound</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.OnRowDeleted</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.OnRowDeleting</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.OnRowEditing</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.OnRowUpdated</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.OnRowUpdating</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.OnSelectedIndexChanged</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.OnSelectedIndexChanging</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.OnSorted</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.OnSorting</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.PageCount</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.PageIndex</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.PageIndexChanged</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.PageIndexChanging</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.PageSize</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.PagerSettings</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.PagerStyle</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.PagerTemplate</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.PerformDataBinding</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.PrepareControlHierarchy</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.RaiseCallbackEvent</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.RaisePostBackEvent</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.Render</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.RowCancelingEdit</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.RowCommand</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.RowCreated</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.RowDataBound</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.RowDeleted</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.RowDeleting</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.RowEditing</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.RowHeaderColumn</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.RowStyle</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.RowUpdated</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.RowUpdating</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.Rows</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.SaveControlState</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.SaveViewState</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.SelectRow</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.SelectedDataKey</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.SelectedIndex</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.SelectedIndexChanged</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.SelectedIndexChanging</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.SelectedPersistedDataKey</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.SelectedRow</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.SelectedRowStyle</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.SelectedValue</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.SetEditRow</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.SetPageIndex</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.ShowFooter</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.ShowHeader</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.ShowHeaderWhenEmpty</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.Sort</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.SortDirection</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.SortExpression</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.Sorted</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.SortedAscendingCellStyle</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.SortedAscendingHeaderStyle</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.SortedDescendingCellStyle</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.SortedDescendingHeaderStyle</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.Sorting</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.TagKey</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.TopPagerRow</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.TrackViewState</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.UpdateMethod</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.UpdateRow</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.UseAccessibleHeader</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.GridView.VirtualItemCount</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.ICallbackContainer.GetCallbackScript</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.IDataBoundControl.DataKeyNames</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.IDataBoundControl.DataMember</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.IDataBoundControl.DataSource</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.IDataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.IDataBoundControl.DataSourceObject</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.IDataBoundListControl.ClientIDRowSuffix</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.IDataBoundListControl.DataKeys</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.IDataBoundListControl.EnablePersistedSelection</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.IDataBoundListControl.SelectedDataKey</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.IDataBoundListControl.SelectedIndex</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.IFieldControl.FieldsGenerator</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.IPersistedSelector.DataKey</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.IPostBackContainer.GetPostBackOptions</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>GridView.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>GridView.(case list)</code> | ✅ 一致 |
| <code>GridView.(default)</code> | ✅ 一致 |
| <code>GridView.AccessKey=empty</code> | ✅ 一致 |
| <code>GridView.AccessKey=japanese</code> | ✅ 一致 |
| <code>GridView.AccessKey=k</code> | ✅ 一致 |
| <code>GridView.AccessKey=markup</code> | ✅ 一致 |
| <code>GridView.AccessKey=text</code> | ✅ 一致 |
| <code>GridView.AllowCustomPaging=false</code> | ✅ 一致 |
| <code>GridView.AllowCustomPaging=true</code> | ✅ 一致 |
| <code>GridView.AllowPaging=false</code> | ✅ 一致 |
| <code>GridView.AllowPaging=true</code> | ✅ 一致 |
| <code>GridView.AllowPaging=true+PageSize=3</code> | ✅ 一致 |
| <code>GridView.AllowPaging=true+PageSize=3+EnableViewState=false</code> | ✅ 一致 |
| <code>GridView.AllowPaging=true+PageSize=3+PageIndex=1</code> | ✅ 一致 |
| <code>GridView.AllowPaging=true+PageSize=3+PageIndex=3</code> | ✅ 一致 |
| <code>GridView.AllowPaging=true+PageSize=3+PagerSettings.Mode=NextPrevious</code> | ✅ 一致 |
| <code>GridView.AllowPaging=true+PageSize=3+PagerSettings.Mode=NumericFirstLast</code> | ✅ 一致 |
| <code>GridView.AllowPaging=true+PageSize=3+PagerSettings.Position=TopAndBottom</code> | ✅ 一致 |
| <code>GridView.AllowPaging=true+PageSize=3+PagerSettings.Visible=false</code> | ✅ 一致 |
| <code>GridView.AllowPaging=true+PageSize=3+PagerStyle.BackColor=Red</code> | ✅ 一致 |
| <code>GridView.AllowPaging=true+Visible=false</code> | ✅ 一致 |
| <code>GridView.AllowSorting=false</code> | ✅ 一致 |
| <code>GridView.AllowSorting=true</code> | ✅ 一致 |
| <code>GridView.AllowSorting=true+AllowPaging=true</code> | ✅ 一致 |
| <code>GridView.AlternatingRowStyle.BackColor=Red</code> | ✅ 一致 |
| <code>GridView.AlternatingRowStyle.BackColor=Red+RowStyle.BackColor=#3366CC</code> | ✅ 一致 |
| <code>GridView.AlternatingRowStyle.CssClass=text</code> | ✅ 一致 |
| <code>GridView.AlternatingRowStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>GridView.AlternatingRowStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>GridView.AlternatingRowStyle.Height=120px</code> | ✅ 一致 |
| <code>GridView.AlternatingRowStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>GridView.AlternatingRowStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>GridView.AlternatingRowStyle.Wrap=true</code> | ✅ 一致 |
| <code>GridView.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>GridView.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>GridView.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>GridView.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>GridView.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>GridView.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>GridView.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>GridView.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>GridView.AutoGenerateColumns=false</code> | ✅ 一致 |
| <code>GridView.AutoGenerateColumns=true</code> | ✅ 一致 |
| <code>GridView.AutoGenerateDeleteButton=false</code> | ✅ 一致 |
| <code>GridView.AutoGenerateDeleteButton=true</code> | ✅ 一致 |
| <code>GridView.AutoGenerateEditButton=false</code> | ✅ 一致 |
| <code>GridView.AutoGenerateEditButton=true</code> | ✅ 一致 |
| <code>GridView.AutoGenerateEditButton=true+AutoGenerateDeleteButton=true+AutoGenerateSelectButton=true</code> | ✅ 一致 |
| <code>GridView.AutoGenerateSelectButton=false</code> | ✅ 一致 |
| <code>GridView.AutoGenerateSelectButton=true</code> | ✅ 一致 |
| <code>GridView.BackColor=#3366CC</code> | ✅ 一致 |
| <code>GridView.BackColor=Red</code> | ✅ 一致 |
| <code>GridView.BackColor=Transparent</code> | ✅ 一致 |
| <code>GridView.BackImageUrl=empty</code> | ✅ 一致 |
| <code>GridView.BackImageUrl=japanese</code> | ✅ 一致 |
| <code>GridView.BackImageUrl=markup</code> | ✅ 一致 |
| <code>GridView.BackImageUrl=text</code> | ✅ 一致 |
| <code>GridView.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>GridView.BorderColor=Red</code> | ✅ 一致 |
| <code>GridView.BorderColor=Transparent</code> | ✅ 一致 |
| <code>GridView.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>GridView.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>GridView.BorderStyle=Double</code> | ✅ 一致 |
| <code>GridView.BorderStyle=Groove</code> | ✅ 一致 |
| <code>GridView.BorderStyle=Inset</code> | ✅ 一致 |
| <code>GridView.BorderStyle=None</code> | ✅ 一致 |
| <code>GridView.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>GridView.BorderStyle=Outset</code> | ✅ 一致 |
| <code>GridView.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>GridView.BorderStyle=Solid</code> | ✅ 一致 |
| <code>GridView.BorderWidth=120px</code> | ✅ 一致 |
| <code>GridView.BorderWidth=2em</code> | ✅ 一致 |
| <code>GridView.BorderWidth=50%</code> | ✅ 一致 |
| <code>GridView.Caption=empty</code> | ✅ 一致 |
| <code>GridView.Caption=japanese</code> | ✅ 一致 |
| <code>GridView.Caption=markup</code> | ✅ 一致 |
| <code>GridView.Caption=text</code> | ✅ 一致 |
| <code>GridView.Caption=text+CaptionAlign=Bottom</code> | ✅ 一致 |
| <code>GridView.CaptionAlign=Bottom</code> | ✅ 一致 |
| <code>GridView.CaptionAlign=Left</code> | ✅ 一致 |
| <code>GridView.CaptionAlign=NotSet</code> | ✅ 一致 |
| <code>GridView.CaptionAlign=Right</code> | ✅ 一致 |
| <code>GridView.CaptionAlign=Top</code> | ✅ 一致 |
| <code>GridView.CellPadding=-1</code> | ✅ 一致 |
| <code>GridView.CellPadding=0</code> | ✅ 一致 |
| <code>GridView.CellPadding=3</code> | ✅ 一致 |
| <code>GridView.CellSpacing=-1</code> | ✅ 一致 |
| <code>GridView.CellSpacing=0</code> | ✅ 一致 |
| <code>GridView.CellSpacing=3</code> | ✅ 一致 |
| <code>GridView.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>GridView.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>GridView.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>GridView.ClientIDMode=Static</code> | ✅ 一致 |
| <code>GridView.ClientIDRowSuffix=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>GridView.CssClass=empty</code> | ✅ 一致 |
| <code>GridView.CssClass=japanese</code> | ✅ 一致 |
| <code>GridView.CssClass=markup</code> | ✅ 一致 |
| <code>GridView.CssClass=text</code> | ✅ 一致 |
| <code>GridView.DataKeyNames=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>GridView.DataKeyNames=Code</code> | ✅ 一致 |
| <code>GridView.DataKeyNames=Code+EditIndex=1</code> | ✅ 一致 |
| <code>GridView.DataKeyNames=Code+SelectedIndex=1</code> | ✅ 一致 |
| <code>GridView.DataMember=empty</code> | ✅ 一致 |
| <code>GridView.DataMember=japanese</code> | ✅ 一致 |
| <code>GridView.DataMember=markup</code> | ✅ 一致 |
| <code>GridView.DataMember=text</code> | ✅ 一致 |
| <code>GridView.DataSourceID=empty</code> | ✅ 一致 |
| <code>GridView.DataSourceID=japanese</code> | ✅ 一致 |
| <code>GridView.DataSourceID=markup</code> | ✅ 一致 |
| <code>GridView.DataSourceID=text</code> | ✅ 一致 |
| <code>GridView.DeleteMethod=empty</code> | ✅ 一致 |
| <code>GridView.DeleteMethod=japanese</code> | ✅ 一致 |
| <code>GridView.DeleteMethod=markup</code> | ✅ 一致 |
| <code>GridView.DeleteMethod=text</code> | ✅ 一致 |
| <code>GridView.EditIndex=-1</code> | ✅ 一致 |
| <code>GridView.EditIndex=0</code> | ✅ 一致 |
| <code>GridView.EditIndex=1</code> | ✅ 一致 |
| <code>GridView.EditIndex=1+EditRowStyle.BackColor=Red</code> | ✅ 一致 |
| <code>GridView.EditIndex=3</code> | ✅ 一致 |
| <code>GridView.EditRowStyle.BackColor=Red</code> | ✅ 一致 |
| <code>GridView.EditRowStyle.CssClass=text</code> | ✅ 一致 |
| <code>GridView.EditRowStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>GridView.EditRowStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>GridView.EditRowStyle.Height=120px</code> | ✅ 一致 |
| <code>GridView.EditRowStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>GridView.EditRowStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>GridView.EditRowStyle.Wrap=true</code> | ✅ 一致 |
| <code>GridView.EmptyDataRowStyle.BackColor=Red</code> | ✅ 一致 |
| <code>GridView.EmptyDataRowStyle.CssClass=text</code> | ✅ 一致 |
| <code>GridView.EmptyDataRowStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>GridView.EmptyDataRowStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>GridView.EmptyDataRowStyle.Height=120px</code> | ✅ 一致 |
| <code>GridView.EmptyDataRowStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>GridView.EmptyDataRowStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>GridView.EmptyDataRowStyle.Wrap=true</code> | ✅ 一致 |
| <code>GridView.EmptyDataText=empty</code> | ✅ 一致 |
| <code>GridView.EmptyDataText=japanese</code> | ✅ 一致 |
| <code>GridView.EmptyDataText=markup</code> | ✅ 一致 |
| <code>GridView.EmptyDataText=text</code> | ✅ 一致 |
| <code>GridView.EnableModelValidation=false</code> | ✅ 一致 |
| <code>GridView.EnableModelValidation=true</code> | ✅ 一致 |
| <code>GridView.EnablePersistedSelection=false</code> | ✅ 一致 |
| <code>GridView.EnablePersistedSelection=true</code> | ✅ 一致 |
| <code>GridView.EnableSortingAndPagingCallbacks=false</code> | ✅ 一致 |
| <code>GridView.EnableSortingAndPagingCallbacks=true</code> | ✅ 一致 |
| <code>GridView.EnableTheming=false</code> | ✅ 一致 |
| <code>GridView.EnableTheming=true</code> | ✅ 一致 |
| <code>GridView.EnableViewState=false</code> | ✅ 一致 |
| <code>GridView.EnableViewState=true</code> | ✅ 一致 |
| <code>GridView.Enabled=false</code> | ✅ 一致 |
| <code>GridView.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>GridView.Enabled=true</code> | ✅ 一致 |
| <code>GridView.Font.Bold=false</code> | ✅ 一致 |
| <code>GridView.Font.Bold=true</code> | ✅ 一致 |
| <code>GridView.Font.Italic=false</code> | ✅ 一致 |
| <code>GridView.Font.Italic=true</code> | ✅ 一致 |
| <code>GridView.Font.Name=empty</code> | ✅ 一致 |
| <code>GridView.Font.Name=japanese</code> | ✅ 一致 |
| <code>GridView.Font.Name=markup</code> | ✅ 一致 |
| <code>GridView.Font.Name=text</code> | ✅ 一致 |
| <code>GridView.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>GridView.Font.Overline=false</code> | ✅ 一致 |
| <code>GridView.Font.Overline=true</code> | ✅ 一致 |
| <code>GridView.Font.Size=12pt</code> | ✅ 一致 |
| <code>GridView.Font.Size=150%</code> | ✅ 一致 |
| <code>GridView.Font.Size=Large</code> | ✅ 一致 |
| <code>GridView.Font.Strikeout=false</code> | ✅ 一致 |
| <code>GridView.Font.Strikeout=true</code> | ✅ 一致 |
| <code>GridView.Font.Underline=false</code> | ✅ 一致 |
| <code>GridView.Font.Underline=true</code> | ✅ 一致 |
| <code>GridView.FooterStyle.BackColor=Red</code> | ✅ 一致 |
| <code>GridView.FooterStyle.CssClass=text</code> | ✅ 一致 |
| <code>GridView.FooterStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>GridView.FooterStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>GridView.FooterStyle.Height=120px</code> | ✅ 一致 |
| <code>GridView.FooterStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>GridView.FooterStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>GridView.FooterStyle.Wrap=true</code> | ✅ 一致 |
| <code>GridView.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>GridView.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>GridView.ForeColor=Red</code> | ✅ 一致 |
| <code>GridView.ForeColor=Transparent</code> | ✅ 一致 |
| <code>GridView.GridLines=Both</code> | ✅ 一致 |
| <code>GridView.GridLines=Horizontal</code> | ✅ 一致 |
| <code>GridView.GridLines=None</code> | ✅ 一致 |
| <code>GridView.GridLines=None+CellPadding=3</code> | ✅ 一致 |
| <code>GridView.GridLines=Vertical</code> | ✅ 一致 |
| <code>GridView.HeaderStyle.BackColor=Red</code> | ✅ 一致 |
| <code>GridView.HeaderStyle.CssClass=text</code> | ✅ 一致 |
| <code>GridView.HeaderStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>GridView.HeaderStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>GridView.HeaderStyle.Height=120px</code> | ✅ 一致 |
| <code>GridView.HeaderStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>GridView.HeaderStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>GridView.HeaderStyle.Wrap=true</code> | ✅ 一致 |
| <code>GridView.Height=120px</code> | ✅ 一致 |
| <code>GridView.Height=2em</code> | ✅ 一致 |
| <code>GridView.Height=50%</code> | ✅ 一致 |
| <code>GridView.HorizontalAlign=Center</code> | ✅ 一致 |
| <code>GridView.HorizontalAlign=Justify</code> | ✅ 一致 |
| <code>GridView.HorizontalAlign=Left</code> | ✅ 一致 |
| <code>GridView.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>GridView.HorizontalAlign=Right</code> | ✅ 一致 |
| <code>GridView.ID=empty</code> | ✅ 一致 |
| <code>GridView.ID=japanese</code> | ✅ 一致 |
| <code>GridView.ID=markup</code> | ✅ 一致 |
| <code>GridView.ID=text</code> | ✅ 一致 |
| <code>GridView.ItemType=empty</code> | ✅ 一致 |
| <code>GridView.ItemType=japanese</code> | ✅ 一致 |
| <code>GridView.ItemType=markup</code> | ✅ 一致 |
| <code>GridView.ItemType=text</code> | ✅ 一致 |
| <code>GridView.PageIndex=-1</code> | ✅ 一致 |
| <code>GridView.PageIndex=0</code> | ✅ 一致 |
| <code>GridView.PageIndex=1</code> | ✅ 一致 |
| <code>GridView.PageIndex=3</code> | ✅ 一致 |
| <code>GridView.PageSize=-1</code> | ✅ 一致 |
| <code>GridView.PageSize=0</code> | ✅ 一致 |
| <code>GridView.PageSize=3</code> | ✅ 一致 |
| <code>GridView.PagerSettings.FirstPageImageUrl=empty</code> | ✅ 一致 |
| <code>GridView.PagerSettings.FirstPageImageUrl=japanese</code> | ✅ 一致 |
| <code>GridView.PagerSettings.FirstPageImageUrl=markup</code> | ✅ 一致 |
| <code>GridView.PagerSettings.FirstPageImageUrl=text</code> | ✅ 一致 |
| <code>GridView.PagerSettings.FirstPageText=empty</code> | ✅ 一致 |
| <code>GridView.PagerSettings.FirstPageText=japanese</code> | ✅ 一致 |
| <code>GridView.PagerSettings.FirstPageText=markup</code> | ✅ 一致 |
| <code>GridView.PagerSettings.FirstPageText=text</code> | ✅ 一致 |
| <code>GridView.PagerSettings.LastPageImageUrl=empty</code> | ✅ 一致 |
| <code>GridView.PagerSettings.LastPageImageUrl=japanese</code> | ✅ 一致 |
| <code>GridView.PagerSettings.LastPageImageUrl=markup</code> | ✅ 一致 |
| <code>GridView.PagerSettings.LastPageImageUrl=text</code> | ✅ 一致 |
| <code>GridView.PagerSettings.LastPageText=empty</code> | ✅ 一致 |
| <code>GridView.PagerSettings.LastPageText=japanese</code> | ✅ 一致 |
| <code>GridView.PagerSettings.LastPageText=markup</code> | ✅ 一致 |
| <code>GridView.PagerSettings.LastPageText=text</code> | ✅ 一致 |
| <code>GridView.PagerSettings.Mode=NextPrevious</code> | ✅ 一致 |
| <code>GridView.PagerSettings.Mode=NextPreviousFirstLast</code> | ✅ 一致 |
| <code>GridView.PagerSettings.Mode=Numeric</code> | ✅ 一致 |
| <code>GridView.PagerSettings.Mode=NumericFirstLast</code> | ✅ 一致 |
| <code>GridView.PagerSettings.NextPageImageUrl=empty</code> | ✅ 一致 |
| <code>GridView.PagerSettings.NextPageImageUrl=japanese</code> | ✅ 一致 |
| <code>GridView.PagerSettings.NextPageImageUrl=markup</code> | ✅ 一致 |
| <code>GridView.PagerSettings.NextPageImageUrl=text</code> | ✅ 一致 |
| <code>GridView.PagerSettings.NextPageText=empty</code> | ✅ 一致 |
| <code>GridView.PagerSettings.NextPageText=japanese</code> | ✅ 一致 |
| <code>GridView.PagerSettings.NextPageText=markup</code> | ✅ 一致 |
| <code>GridView.PagerSettings.NextPageText=text</code> | ✅ 一致 |
| <code>GridView.PagerSettings.PageButtonCount=-1</code> | ✅ 一致 |
| <code>GridView.PagerSettings.PageButtonCount=0</code> | ✅ 一致 |
| <code>GridView.PagerSettings.PageButtonCount=3</code> | ✅ 一致 |
| <code>GridView.PagerSettings.Position=Bottom</code> | ✅ 一致 |
| <code>GridView.PagerSettings.Position=Top</code> | ✅ 一致 |
| <code>GridView.PagerSettings.Position=TopAndBottom</code> | ✅ 一致 |
| <code>GridView.PagerSettings.PreviousPageImageUrl=empty</code> | ✅ 一致 |
| <code>GridView.PagerSettings.PreviousPageImageUrl=japanese</code> | ✅ 一致 |
| <code>GridView.PagerSettings.PreviousPageImageUrl=markup</code> | ✅ 一致 |
| <code>GridView.PagerSettings.PreviousPageImageUrl=text</code> | ✅ 一致 |
| <code>GridView.PagerSettings.PreviousPageText=empty</code> | ✅ 一致 |
| <code>GridView.PagerSettings.PreviousPageText=japanese</code> | ✅ 一致 |
| <code>GridView.PagerSettings.PreviousPageText=markup</code> | ✅ 一致 |
| <code>GridView.PagerSettings.PreviousPageText=text</code> | ✅ 一致 |
| <code>GridView.PagerSettings.Visible=false</code> | ✅ 一致 |
| <code>GridView.PagerSettings.Visible=true</code> | ✅ 一致 |
| <code>GridView.PagerStyle.BackColor=Red</code> | ✅ 一致 |
| <code>GridView.PagerStyle.CssClass=text</code> | ✅ 一致 |
| <code>GridView.PagerStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>GridView.PagerStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>GridView.PagerStyle.Height=120px</code> | ✅ 一致 |
| <code>GridView.PagerStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>GridView.PagerStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>GridView.PagerStyle.Wrap=true</code> | ✅ 一致 |
| <code>GridView.RowHeaderColumn=Name</code> | ✅ 一致 |
| <code>GridView.RowHeaderColumn=empty</code> | ✅ 一致 |
| <code>GridView.RowHeaderColumn=japanese</code> | ✅ 一致 |
| <code>GridView.RowHeaderColumn=markup</code> | ✅ 一致 |
| <code>GridView.RowHeaderColumn=text</code> | ✅ 一致 |
| <code>GridView.RowStyle.BackColor=Red</code> | ✅ 一致 |
| <code>GridView.RowStyle.CssClass=text</code> | ✅ 一致 |
| <code>GridView.RowStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>GridView.RowStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>GridView.RowStyle.Height=120px</code> | ✅ 一致 |
| <code>GridView.RowStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>GridView.RowStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>GridView.RowStyle.Wrap=true</code> | ✅ 一致 |
| <code>GridView.SelectMethod=empty</code> | ✅ 一致 |
| <code>GridView.SelectMethod=japanese</code> | ✅ 一致 |
| <code>GridView.SelectMethod=markup</code> | ✅ 一致 |
| <code>GridView.SelectMethod=text</code> | ✅ 一致 |
| <code>GridView.SelectedIndex=-1</code> | ✅ 一致 |
| <code>GridView.SelectedIndex=0</code> | ✅ 一致 |
| <code>GridView.SelectedIndex=1</code> | ✅ 一致 |
| <code>GridView.SelectedIndex=1+SelectedRowStyle.BackColor=Red</code> | ✅ 一致 |
| <code>GridView.SelectedIndex=3</code> | ✅ 一致 |
| <code>GridView.SelectedRowStyle.BackColor=Red</code> | ✅ 一致 |
| <code>GridView.SelectedRowStyle.CssClass=text</code> | ✅ 一致 |
| <code>GridView.SelectedRowStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>GridView.SelectedRowStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>GridView.SelectedRowStyle.Height=120px</code> | ✅ 一致 |
| <code>GridView.SelectedRowStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>GridView.SelectedRowStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>GridView.SelectedRowStyle.Wrap=true</code> | ✅ 一致 |
| <code>GridView.ShowFooter=false</code> | ✅ 一致 |
| <code>GridView.ShowFooter=true</code> | ✅ 一致 |
| <code>GridView.ShowFooter=true+FooterStyle.BackColor=Red</code> | ✅ 一致 |
| <code>GridView.ShowHeader=false</code> | ✅ 一致 |
| <code>GridView.ShowHeader=false+ShowFooter=true</code> | ✅ 一致 |
| <code>GridView.ShowHeader=true</code> | ✅ 一致 |
| <code>GridView.ShowHeaderWhenEmpty=false</code> | ✅ 一致 |
| <code>GridView.ShowHeaderWhenEmpty=true</code> | ✅ 一致 |
| <code>GridView.SkinID=empty</code> | ✅ 一致 |
| <code>GridView.SkinID=japanese</code> | ✅ 一致 |
| <code>GridView.SkinID=markup</code> | ✅ 一致 |
| <code>GridView.SkinID=text</code> | ✅ 一致 |
| <code>GridView.SortedAscendingCellStyle.BackColor=Red</code> | ✅ 一致 |
| <code>GridView.SortedAscendingCellStyle.CssClass=text</code> | ✅ 一致 |
| <code>GridView.SortedAscendingCellStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>GridView.SortedAscendingCellStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>GridView.SortedAscendingCellStyle.Height=120px</code> | ✅ 一致 |
| <code>GridView.SortedAscendingCellStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>GridView.SortedAscendingCellStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>GridView.SortedAscendingCellStyle.Wrap=true</code> | ✅ 一致 |
| <code>GridView.SortedAscendingHeaderStyle.BackColor=Red</code> | ✅ 一致 |
| <code>GridView.SortedAscendingHeaderStyle.CssClass=text</code> | ✅ 一致 |
| <code>GridView.SortedAscendingHeaderStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>GridView.SortedAscendingHeaderStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>GridView.SortedAscendingHeaderStyle.Height=120px</code> | ✅ 一致 |
| <code>GridView.SortedAscendingHeaderStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>GridView.SortedAscendingHeaderStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>GridView.SortedAscendingHeaderStyle.Wrap=true</code> | ✅ 一致 |
| <code>GridView.SortedDescendingCellStyle.BackColor=Red</code> | ✅ 一致 |
| <code>GridView.SortedDescendingCellStyle.CssClass=text</code> | ✅ 一致 |
| <code>GridView.SortedDescendingCellStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>GridView.SortedDescendingCellStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>GridView.SortedDescendingCellStyle.Height=120px</code> | ✅ 一致 |
| <code>GridView.SortedDescendingCellStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>GridView.SortedDescendingCellStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>GridView.SortedDescendingCellStyle.Wrap=true</code> | ✅ 一致 |
| <code>GridView.SortedDescendingHeaderStyle.BackColor=Red</code> | ✅ 一致 |
| <code>GridView.SortedDescendingHeaderStyle.CssClass=text</code> | ✅ 一致 |
| <code>GridView.SortedDescendingHeaderStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>GridView.SortedDescendingHeaderStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>GridView.SortedDescendingHeaderStyle.Height=120px</code> | ✅ 一致 |
| <code>GridView.SortedDescendingHeaderStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>GridView.SortedDescendingHeaderStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>GridView.SortedDescendingHeaderStyle.Wrap=true</code> | ✅ 一致 |
| <code>GridView.Style[color]=1em</code> | ✅ 一致 |
| <code>GridView.Style[color]=red</code> | ✅ 一致 |
| <code>GridView.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>GridView.Style[margin-left]=red</code> | ✅ 一致 |
| <code>GridView.TabIndex=-1</code> | ✅ 一致 |
| <code>GridView.TabIndex=0</code> | ✅ 一致 |
| <code>GridView.TabIndex=3</code> | ✅ 一致 |
| <code>GridView.ToolTip=empty</code> | ✅ 一致 |
| <code>GridView.ToolTip=japanese</code> | ✅ 一致 |
| <code>GridView.ToolTip=markup</code> | ✅ 一致 |
| <code>GridView.ToolTip=text</code> | ✅ 一致 |
| <code>GridView.UpdateMethod=empty</code> | ✅ 一致 |
| <code>GridView.UpdateMethod=japanese</code> | ✅ 一致 |
| <code>GridView.UpdateMethod=markup</code> | ✅ 一致 |
| <code>GridView.UpdateMethod=text</code> | ✅ 一致 |
| <code>GridView.UseAccessibleHeader=false</code> | ✅ 一致 |
| <code>GridView.UseAccessibleHeader=true</code> | ✅ 一致 |
| <code>GridView.UseAccessibleHeader=true+RowHeaderColumn=Name</code> | ✅ 一致 |
| <code>GridView.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>GridView.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>GridView.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>GridView.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>GridView.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>GridView.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>GridView.VirtualItemCount=-1</code> | ✅ 一致 |
| <code>GridView.VirtualItemCount=0</code> | ✅ 一致 |
| <code>GridView.VirtualItemCount=3</code> | ✅ 一致 |
| <code>GridView.Visible=false</code> | ✅ 一致 |
| <code>GridView.Visible=true</code> | ✅ 一致 |
| <code>GridView.Width=120px</code> | ✅ 一致 |
| <code>GridView.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>GridView.Width=2em</code> | ✅ 一致 |
| <code>GridView.Width=50%</code> | ✅ 一致 |
| <code>GridView[empty].(default)</code> | ✅ 一致 |
| <code>GridView[empty].EmptyDataText=markup</code> | ✅ 一致 |
| <code>GridView[empty].EmptyDataText=text</code> | ✅ 一致 |
| <code>GridView[empty].ShowHeaderWhenEmpty=true</code> | ✅ 一致 |
| <code>GridView[empty].ShowHeaderWhenEmpty=true+EmptyDataText=text</code> | ✅ 一致 |
| <code>GridView[fields].(default)</code> | ✅ 一致 |
| <code>GridView[fields].AllowPaging=true+PageSize=3</code> | ✅ 一致 |
| <code>GridView[fields].AllowPaging=true+PageSize=3+PageIndex=1</code> | ✅ 一致 |
| <code>GridView[fields].AllowSorting=true</code> | ✅ 一致 |
| <code>GridView[fields].AllowSorting=true+AllowPaging=true+PageSize=3</code> | ✅ 一致 |
| <code>GridView[fields].DataKeyNames=Code</code> | ✅ 一致 |
| <code>GridView[fields].EditIndex=1</code> | ✅ 一致 |
| <code>GridView[fields].EditIndex=1+DataKeyNames=Code</code> | ✅ 一致 |
| <code>GridView[fields].Enabled=false</code> | ✅ 一致 |
| <code>GridView[fields].SelectedIndex=1</code> | ✅ 一致 |
| <code>GridView[fields].ShowFooter=true</code> | ✅ 一致 |
| <code>GridView[fields].ShowHeader=false</code> | ✅ 一致 |
| <code>GridView[fields].UseAccessibleHeader=true+RowHeaderColumn=Name</code> | ✅ 一致 |
| <code>GridView[naming container].(default)</code> | ✅ 一致 |
| <code>GridView[naming container].AllowPaging=true+PageSize=3</code> | ✅ 一致 |
| <code>GridView[naming container].AutoGenerateSelectButton=true</code> | ✅ 一致 |
| <code>GridView[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>GridView[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>GridView[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>GridView[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>GridView[naming container].ID=text</code> | ✅ 一致 |
| <code>GridView[naming container].SelectedIndex=1</code> | ✅ 一致 |
| <code>GridView[themed].(default)</code> | ✅ 一致 |
| <code>GridView[themed].CssClass=text</code> | ✅ 一致 |
| <code>GridView[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>GridView[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| GridView [render][databind][datasource][datasourceid][templates][dynamic] first request | ✅ 一致 |
| GridView [paging][postbackevent] the page&#39;s grid: page 2 | ✅ 一致 |
| GridView [sorting] sorted by name | ✅ 一致 |
| GridView [sorting][viewstate] sorted by name again: descending (the page&#39;s) | ✅ 一致 |
| GridView [selection][datakeys] the first row selected | ✅ 一致 |
| GridView [rowcommand] a ButtonField&#39;s command | ✅ 一致 |
| GridView [editing] the first row edited | ✅ 一致 |
| GridView [editing][bind][datakeys] updated: keys, old and new values | ✅ 一致 |
| GridView [editing] edit cancelled | ✅ 一致 |
| GridView [editing][datakeys] the first row deleted | ✅ 一致 |
| GridView [paging][datasourceid] the data source&#39;s grid: page 2 | ✅ 一致 |
| GridView [sorting][datasourceid] sorted by price (the grid&#39;s own direction) | ✅ 一致 |
| GridView [sorting][datasourceid] sorted by price again: descending | ✅ 一致 |
| GridView [selection][datasourceid] a row selected | ✅ 一致 |
| GridView [editing][datasourceid] updated through the data source | ✅ 一致 |
| GridView [editing][datasourceid] deleted through the data source | ✅ 一致 |
| GridView [callback][paging] paged by a callback: no postback | ✅ 一致 |
| GridView [callback][sorting] sorted by a callback | ✅ 一致 |
| GridView [viewstate] posted (the grid without view state is empty) | ✅ 一致 |
| GridView [visible][viewstate] the hidden grid shown with its rows | ✅ 一致 |

</details>
