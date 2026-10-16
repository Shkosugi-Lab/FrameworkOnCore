# FormView パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-16 16:37
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 309 | 309 | 0 | 0 |
| ② HTML | 321 | 321 | 0 | 0 |
| ③ シナリオ | 15 | 15 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 319 件中 319 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 120 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 15 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 301 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 2 | ✅ |
| Visible=false（描画されず状態は保持） | 3 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 7 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 2 | 0 | ✅ |
| ポストバックイベント | 0 | 1 | ✅ |
| ページング | 16 | 3 | ✅ |
| 項目の挿入 | 15 | 3 | ✅ |
| 行・項目の編集（編集・更新・取り消し・削除） | 8 | 5 | ✅ |
| 行・項目のコマンド（RowCommand・ItemCommand へのバブル） | 0 | 1 | ✅ |
| データキー（DataKeyNames・DataKeys） | 3 | 2 | ✅ |
| データソースコントロールとの連携（DataSourceID） | 0 | 5 | ✅ |
| テンプレート | 0 | 1 | ✅ |
| データソース連携（DataSource・DataTextField など） | 3 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>FormView.Api.Control.#ctor</code> | ✅ 一致 |
| <code>FormView.Api.Control.Adapter</code> | ✅ 一致 |
| <code>FormView.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>FormView.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>FormView.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>FormView.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>FormView.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>FormView.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>FormView.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>FormView.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>FormView.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>FormView.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>FormView.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>FormView.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>FormView.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>FormView.Api.Control.ClientID</code> | ✅ 一致 |
| <code>FormView.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>FormView.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>FormView.Api.Control.Context</code> | ✅ 一致 |
| <code>FormView.Api.Control.Controls</code> | ✅ 一致 |
| <code>FormView.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>FormView.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>FormView.Api.Control.DataBind</code> | ✅ 一致 |
| <code>FormView.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>FormView.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>FormView.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>FormView.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>FormView.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>FormView.Api.Control.Dispose</code> | ✅ 一致 |
| <code>FormView.Api.Control.Disposed</code> | ✅ 一致 |
| <code>FormView.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>FormView.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>FormView.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>FormView.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>FormView.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>FormView.Api.Control.Events</code> | ✅ 一致 |
| <code>FormView.Api.Control.FindControl</code> | ✅ 一致 |
| <code>FormView.Api.Control.Focus</code> | ✅ 一致 |
| <code>FormView.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>FormView.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>FormView.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>FormView.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>FormView.Api.Control.HasControls</code> | ✅ 一致 |
| <code>FormView.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>FormView.Api.Control.ID</code> | ✅ 一致 |
| <code>FormView.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>FormView.Api.Control.Init</code> | ✅ 一致 |
| <code>FormView.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>FormView.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>FormView.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>FormView.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>FormView.Api.Control.Load</code> | ✅ 一致 |
| <code>FormView.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>FormView.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>FormView.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>FormView.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>FormView.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>FormView.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>FormView.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>FormView.Api.Control.OnInit</code> | ✅ 一致 |
| <code>FormView.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>FormView.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>FormView.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>FormView.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>FormView.Api.Control.Page</code> | ✅ 一致 |
| <code>FormView.Api.Control.Parent</code> | ✅ 一致 |
| <code>FormView.Api.Control.PreRender</code> | ✅ 一致 |
| <code>FormView.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>FormView.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>FormView.Api.Control.Render</code> | ✅ 一致 |
| <code>FormView.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>FormView.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>FormView.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>FormView.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>FormView.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>FormView.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>FormView.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>FormView.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>FormView.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>FormView.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>FormView.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>FormView.Api.Control.Site</code> | ✅ 一致 |
| <code>FormView.Api.Control.SkinID</code> | ✅ 一致 |
| <code>FormView.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>FormView.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>FormView.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>FormView.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>FormView.Api.Control.Unload</code> | ✅ 一致 |
| <code>FormView.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>FormView.Api.Control.ViewState</code> | ✅ 一致 |
| <code>FormView.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>FormView.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>FormView.Api.Control.Visible</code> | ✅ 一致 |
| <code>FormView.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>FormView.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>FormView.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>FormView.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>FormView.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>FormView.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>FormView.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>FormView.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>FormView.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>FormView.Api.IDataItemContainer.DataItem</code> | ✅ 一致 |
| <code>FormView.Api.IDataItemContainer.DataItemIndex</code> | ✅ 一致 |
| <code>FormView.Api.IDataItemContainer.DisplayIndex</code> | ✅ 一致 |
| <code>FormView.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>FormView.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>FormView.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>FormView.Api.IPostBackEventHandler.RaisePostBackEvent</code> | ✅ 一致 |
| <code>FormView.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.BaseDataBoundControl.#ctor</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.BaseDataBoundControl.ConfirmInitState</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.BaseDataBoundControl.DataBind</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.BaseDataBoundControl.DataBound</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.BaseDataBoundControl.DataSource</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.BaseDataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.BaseDataBoundControl.EnsureDataBound</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.BaseDataBoundControl.Initialized</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.BaseDataBoundControl.IsBoundUsingDataSourceID</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.BaseDataBoundControl.IsDataBindingAutomatic</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.BaseDataBoundControl.IsUsingModelBinders</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.BaseDataBoundControl.OnDataBound</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.BaseDataBoundControl.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.BaseDataBoundControl.OnInit</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.BaseDataBoundControl.OnPagePreLoad</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.BaseDataBoundControl.OnPreRender</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.BaseDataBoundControl.PerformSelect</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.BaseDataBoundControl.RequiresDataBinding</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.BaseDataBoundControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.BaseDataBoundControl.ValidateDataSource</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.CompositeDataBoundControl.#ctor</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.CompositeDataBoundControl.Controls</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.CompositeDataBoundControl.CreateChildControls</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.CompositeDataBoundControl.DeleteMethod</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.CompositeDataBoundControl.InsertMethod</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.CompositeDataBoundControl.IsUsingModelBinders</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.CompositeDataBoundControl.PerformDataBinding</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.CompositeDataBoundControl.UpdateMethod</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.DataBoundControl.#ctor</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.DataBoundControl.CallingDataMethods</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.DataBoundControl.CreateDataSourceSelectArguments</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.DataBoundControl.CreatingModelDataSource</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.DataBoundControl.DataMember</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.DataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.DataBoundControl.DataSourceObject</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.DataBoundControl.GetData</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.DataBoundControl.GetDataSource</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.DataBoundControl.IsUsingModelBinders</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.DataBoundControl.ItemType</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.DataBoundControl.LoadViewState</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.DataBoundControl.MarkAsDataBound</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.DataBoundControl.OnCreatingModelDataSource</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.DataBoundControl.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.DataBoundControl.OnDataSourceViewChanged</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.DataBoundControl.OnLoad</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.DataBoundControl.OnPagePreLoad</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.DataBoundControl.PerformDataBinding</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.DataBoundControl.PerformSelect</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.DataBoundControl.SaveViewState</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.DataBoundControl.SelectArguments</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.DataBoundControl.SelectMethod</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.DataBoundControl.TrackViewState</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.DataBoundControl.ValidateDataSource</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.#ctor</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.AllowPaging</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.BackImageUrl</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.BottomPagerRow</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.Caption</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.CaptionAlign</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.CellPadding</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.CellSpacing</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.ChangeMode</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.CreateChildControls</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.CreateControlStyle</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.CreateDataSourceSelectArguments</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.CreateRow</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.CreateTable</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.CurrentMode</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.DataBind</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.DataItem</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.DataItemCount</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.DataItemIndex</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.DataKey</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.DataKeyNames</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.DefaultMode</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.DeleteItem</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.DeleteMethod</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.EditItemTemplate</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.EditRowStyle</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.EmptyDataRowStyle</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.EmptyDataTemplate</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.EmptyDataText</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.EnableModelValidation</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.EnsureDataBound</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.ExtractRowValues</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.FooterRow</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.FooterStyle</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.FooterTemplate</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.FooterText</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.GridLines</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.HeaderRow</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.HeaderStyle</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.HeaderTemplate</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.HeaderText</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.HorizontalAlign</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.InitializePager</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.InitializeRow</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.InsertItem</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.InsertItemTemplate</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.InsertMethod</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.InsertRowStyle</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.IsBindableType</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.ItemCommand</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.ItemCreated</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.ItemDeleted</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.ItemDeleting</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.ItemInserted</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.ItemInserting</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.ItemTemplate</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.ItemUpdated</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.ItemUpdating</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.LoadControlState</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.LoadViewState</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.ModeChanged</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.ModeChanging</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.ModifiedOuterTableStylePropertyName</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.OnBubbleEvent</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.OnInit</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.OnItemCommand</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.OnItemCreated</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.OnItemDeleted</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.OnItemDeleting</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.OnItemInserted</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.OnItemInserting</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.OnItemUpdated</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.OnItemUpdating</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.OnModeChanged</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.OnModeChanging</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.OnPageIndexChanged</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.OnPageIndexChanging</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.PageCount</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.PageIndex</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.PageIndexChanged</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.PageIndexChanging</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.PagerSettings</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.PagerStyle</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.PagerTemplate</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.PerformDataBinding</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.PrepareControlHierarchy</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.RaisePostBackEvent</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.Render</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.RenderOuterTable</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.Row</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.RowStyle</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.SaveControlState</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.SaveViewState</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.SelectedValue</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.SetPageIndex</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.TagKey</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.TopPagerRow</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.TrackViewState</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.UpdateItem</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.FormView.UpdateMethod</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.IDataBoundControl.DataKeyNames</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.IDataBoundControl.DataMember</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.IDataBoundControl.DataSource</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.IDataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.IDataBoundControl.DataSourceObject</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.IDataBoundItemControl.DataKey</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.IDataBoundItemControl.Mode</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.IPostBackContainer.GetPostBackOptions</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>FormView.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>FormView.(case list)</code> | ✅ 一致 |
| <code>FormView.(default)</code> | ✅ 一致 |
| <code>FormView.AccessKey=empty</code> | ✅ 一致 |
| <code>FormView.AccessKey=japanese</code> | ✅ 一致 |
| <code>FormView.AccessKey=k</code> | ✅ 一致 |
| <code>FormView.AccessKey=markup</code> | ✅ 一致 |
| <code>FormView.AccessKey=text</code> | ✅ 一致 |
| <code>FormView.AllowPaging=false</code> | ✅ 一致 |
| <code>FormView.AllowPaging=true</code> | ✅ 一致 |
| <code>FormView.AllowPaging=true+EnableViewState=false</code> | ✅ 一致 |
| <code>FormView.AllowPaging=true+PageIndex=1</code> | ✅ 一致 |
| <code>FormView.AllowPaging=true+PageIndex=3</code> | ✅ 一致 |
| <code>FormView.AllowPaging=true+PagerSettings.Mode=NextPreviousFirstLast</code> | ✅ 一致 |
| <code>FormView.AllowPaging=true+PagerSettings.Mode=NumericFirstLast</code> | ✅ 一致 |
| <code>FormView.AllowPaging=true+PagerSettings.Position=Top</code> | ✅ 一致 |
| <code>FormView.AllowPaging=true+PagerStyle.BackColor=Red</code> | ✅ 一致 |
| <code>FormView.AllowPaging=true+Visible=false</code> | ✅ 一致 |
| <code>FormView.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>FormView.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>FormView.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>FormView.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>FormView.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>FormView.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>FormView.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>FormView.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>FormView.BackColor=#3366CC</code> | ✅ 一致 |
| <code>FormView.BackColor=Red</code> | ✅ 一致 |
| <code>FormView.BackColor=Transparent</code> | ✅ 一致 |
| <code>FormView.BackImageUrl=empty</code> | ✅ 一致 |
| <code>FormView.BackImageUrl=japanese</code> | ✅ 一致 |
| <code>FormView.BackImageUrl=markup</code> | ✅ 一致 |
| <code>FormView.BackImageUrl=text</code> | ✅ 一致 |
| <code>FormView.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>FormView.BorderColor=Red</code> | ✅ 一致 |
| <code>FormView.BorderColor=Transparent</code> | ✅ 一致 |
| <code>FormView.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>FormView.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>FormView.BorderStyle=Double</code> | ✅ 一致 |
| <code>FormView.BorderStyle=Groove</code> | ✅ 一致 |
| <code>FormView.BorderStyle=Inset</code> | ✅ 一致 |
| <code>FormView.BorderStyle=None</code> | ✅ 一致 |
| <code>FormView.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>FormView.BorderStyle=Outset</code> | ✅ 一致 |
| <code>FormView.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>FormView.BorderStyle=Solid</code> | ✅ 一致 |
| <code>FormView.BorderWidth=120px</code> | ✅ 一致 |
| <code>FormView.BorderWidth=2em</code> | ✅ 一致 |
| <code>FormView.BorderWidth=50%</code> | ✅ 一致 |
| <code>FormView.Caption=empty</code> | ✅ 一致 |
| <code>FormView.Caption=japanese</code> | ✅ 一致 |
| <code>FormView.Caption=markup</code> | ✅ 一致 |
| <code>FormView.Caption=text</code> | ✅ 一致 |
| <code>FormView.Caption=text+CaptionAlign=Bottom</code> | ✅ 一致 |
| <code>FormView.CaptionAlign=Bottom</code> | ✅ 一致 |
| <code>FormView.CaptionAlign=Left</code> | ✅ 一致 |
| <code>FormView.CaptionAlign=NotSet</code> | ✅ 一致 |
| <code>FormView.CaptionAlign=Right</code> | ✅ 一致 |
| <code>FormView.CaptionAlign=Top</code> | ✅ 一致 |
| <code>FormView.CellPadding=-1</code> | ✅ 一致 |
| <code>FormView.CellPadding=0</code> | ✅ 一致 |
| <code>FormView.CellPadding=3</code> | ✅ 一致 |
| <code>FormView.CellSpacing=-1</code> | ✅ 一致 |
| <code>FormView.CellSpacing=0</code> | ✅ 一致 |
| <code>FormView.CellSpacing=3</code> | ✅ 一致 |
| <code>FormView.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>FormView.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>FormView.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>FormView.ClientIDMode=Static</code> | ✅ 一致 |
| <code>FormView.CssClass=empty</code> | ✅ 一致 |
| <code>FormView.CssClass=japanese</code> | ✅ 一致 |
| <code>FormView.CssClass=markup</code> | ✅ 一致 |
| <code>FormView.CssClass=text</code> | ✅ 一致 |
| <code>FormView.DataKeyNames=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>FormView.DataKeyNames=Code</code> | ✅ 一致 |
| <code>FormView.DataMember=empty</code> | ✅ 一致 |
| <code>FormView.DataMember=japanese</code> | ✅ 一致 |
| <code>FormView.DataMember=markup</code> | ✅ 一致 |
| <code>FormView.DataMember=text</code> | ✅ 一致 |
| <code>FormView.DataSourceID=empty</code> | ✅ 一致 |
| <code>FormView.DataSourceID=japanese</code> | ✅ 一致 |
| <code>FormView.DataSourceID=markup</code> | ✅ 一致 |
| <code>FormView.DataSourceID=text</code> | ✅ 一致 |
| <code>FormView.DefaultMode=Edit</code> | ✅ 一致 |
| <code>FormView.DefaultMode=Edit+DataKeyNames=Code</code> | ✅ 一致 |
| <code>FormView.DefaultMode=Edit+EditRowStyle.BackColor=Red</code> | ✅ 一致 |
| <code>FormView.DefaultMode=Insert</code> | ✅ 一致 |
| <code>FormView.DefaultMode=Insert+InsertRowStyle.BackColor=Red</code> | ✅ 一致 |
| <code>FormView.DefaultMode=ReadOnly</code> | ✅ 一致 |
| <code>FormView.DeleteMethod=empty</code> | ✅ 一致 |
| <code>FormView.DeleteMethod=japanese</code> | ✅ 一致 |
| <code>FormView.DeleteMethod=markup</code> | ✅ 一致 |
| <code>FormView.DeleteMethod=text</code> | ✅ 一致 |
| <code>FormView.EditRowStyle.BackColor=Red</code> | ✅ 一致 |
| <code>FormView.EditRowStyle.CssClass=text</code> | ✅ 一致 |
| <code>FormView.EditRowStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>FormView.EditRowStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>FormView.EditRowStyle.Height=120px</code> | ✅ 一致 |
| <code>FormView.EditRowStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>FormView.EditRowStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>FormView.EditRowStyle.Wrap=true</code> | ✅ 一致 |
| <code>FormView.EmptyDataRowStyle.BackColor=Red</code> | ✅ 一致 |
| <code>FormView.EmptyDataRowStyle.CssClass=text</code> | ✅ 一致 |
| <code>FormView.EmptyDataRowStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>FormView.EmptyDataRowStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>FormView.EmptyDataRowStyle.Height=120px</code> | ✅ 一致 |
| <code>FormView.EmptyDataRowStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>FormView.EmptyDataRowStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>FormView.EmptyDataRowStyle.Wrap=true</code> | ✅ 一致 |
| <code>FormView.EmptyDataText=empty</code> | ✅ 一致 |
| <code>FormView.EmptyDataText=japanese</code> | ✅ 一致 |
| <code>FormView.EmptyDataText=markup</code> | ✅ 一致 |
| <code>FormView.EmptyDataText=text</code> | ✅ 一致 |
| <code>FormView.EnableModelValidation=false</code> | ✅ 一致 |
| <code>FormView.EnableModelValidation=true</code> | ✅ 一致 |
| <code>FormView.EnableTheming=false</code> | ✅ 一致 |
| <code>FormView.EnableTheming=true</code> | ✅ 一致 |
| <code>FormView.EnableViewState=false</code> | ✅ 一致 |
| <code>FormView.EnableViewState=true</code> | ✅ 一致 |
| <code>FormView.Enabled=false</code> | ✅ 一致 |
| <code>FormView.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>FormView.Enabled=true</code> | ✅ 一致 |
| <code>FormView.Font.Bold=false</code> | ✅ 一致 |
| <code>FormView.Font.Bold=true</code> | ✅ 一致 |
| <code>FormView.Font.Italic=false</code> | ✅ 一致 |
| <code>FormView.Font.Italic=true</code> | ✅ 一致 |
| <code>FormView.Font.Name=empty</code> | ✅ 一致 |
| <code>FormView.Font.Name=japanese</code> | ✅ 一致 |
| <code>FormView.Font.Name=markup</code> | ✅ 一致 |
| <code>FormView.Font.Name=text</code> | ✅ 一致 |
| <code>FormView.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>FormView.Font.Overline=false</code> | ✅ 一致 |
| <code>FormView.Font.Overline=true</code> | ✅ 一致 |
| <code>FormView.Font.Size=12pt</code> | ✅ 一致 |
| <code>FormView.Font.Size=150%</code> | ✅ 一致 |
| <code>FormView.Font.Size=Large</code> | ✅ 一致 |
| <code>FormView.Font.Strikeout=false</code> | ✅ 一致 |
| <code>FormView.Font.Strikeout=true</code> | ✅ 一致 |
| <code>FormView.Font.Underline=false</code> | ✅ 一致 |
| <code>FormView.Font.Underline=true</code> | ✅ 一致 |
| <code>FormView.FooterStyle.BackColor=Red</code> | ✅ 一致 |
| <code>FormView.FooterStyle.CssClass=text</code> | ✅ 一致 |
| <code>FormView.FooterStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>FormView.FooterStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>FormView.FooterStyle.Height=120px</code> | ✅ 一致 |
| <code>FormView.FooterStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>FormView.FooterStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>FormView.FooterStyle.Wrap=true</code> | ✅ 一致 |
| <code>FormView.FooterText=empty</code> | ✅ 一致 |
| <code>FormView.FooterText=japanese</code> | ✅ 一致 |
| <code>FormView.FooterText=markup</code> | ✅ 一致 |
| <code>FormView.FooterText=text</code> | ✅ 一致 |
| <code>FormView.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>FormView.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>FormView.ForeColor=Red</code> | ✅ 一致 |
| <code>FormView.ForeColor=Transparent</code> | ✅ 一致 |
| <code>FormView.GridLines=Both</code> | ✅ 一致 |
| <code>FormView.GridLines=Both+CellPadding=3</code> | ✅ 一致 |
| <code>FormView.GridLines=Horizontal</code> | ✅ 一致 |
| <code>FormView.GridLines=None</code> | ✅ 一致 |
| <code>FormView.GridLines=Vertical</code> | ✅ 一致 |
| <code>FormView.HeaderStyle.BackColor=Red</code> | ✅ 一致 |
| <code>FormView.HeaderStyle.CssClass=text</code> | ✅ 一致 |
| <code>FormView.HeaderStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>FormView.HeaderStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>FormView.HeaderStyle.Height=120px</code> | ✅ 一致 |
| <code>FormView.HeaderStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>FormView.HeaderStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>FormView.HeaderStyle.Wrap=true</code> | ✅ 一致 |
| <code>FormView.HeaderText=empty</code> | ✅ 一致 |
| <code>FormView.HeaderText=japanese</code> | ✅ 一致 |
| <code>FormView.HeaderText=markup</code> | ✅ 一致 |
| <code>FormView.HeaderText=text</code> | ✅ 一致 |
| <code>FormView.HeaderText=text+FooterText=markup</code> | ✅ 一致 |
| <code>FormView.Height=120px</code> | ✅ 一致 |
| <code>FormView.Height=2em</code> | ✅ 一致 |
| <code>FormView.Height=50%</code> | ✅ 一致 |
| <code>FormView.HorizontalAlign=Center</code> | ✅ 一致 |
| <code>FormView.HorizontalAlign=Justify</code> | ✅ 一致 |
| <code>FormView.HorizontalAlign=Left</code> | ✅ 一致 |
| <code>FormView.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>FormView.HorizontalAlign=Right</code> | ✅ 一致 |
| <code>FormView.ID=empty</code> | ✅ 一致 |
| <code>FormView.ID=japanese</code> | ✅ 一致 |
| <code>FormView.ID=markup</code> | ✅ 一致 |
| <code>FormView.ID=text</code> | ✅ 一致 |
| <code>FormView.InsertMethod=empty</code> | ✅ 一致 |
| <code>FormView.InsertMethod=japanese</code> | ✅ 一致 |
| <code>FormView.InsertMethod=markup</code> | ✅ 一致 |
| <code>FormView.InsertMethod=text</code> | ✅ 一致 |
| <code>FormView.InsertRowStyle.BackColor=Red</code> | ✅ 一致 |
| <code>FormView.InsertRowStyle.CssClass=text</code> | ✅ 一致 |
| <code>FormView.InsertRowStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>FormView.InsertRowStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>FormView.InsertRowStyle.Height=120px</code> | ✅ 一致 |
| <code>FormView.InsertRowStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>FormView.InsertRowStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>FormView.InsertRowStyle.Wrap=true</code> | ✅ 一致 |
| <code>FormView.ItemType=empty</code> | ✅ 一致 |
| <code>FormView.ItemType=japanese</code> | ✅ 一致 |
| <code>FormView.ItemType=markup</code> | ✅ 一致 |
| <code>FormView.ItemType=text</code> | ✅ 一致 |
| <code>FormView.PageIndex=-1</code> | ✅ 一致 |
| <code>FormView.PageIndex=0</code> | ✅ 一致 |
| <code>FormView.PageIndex=1</code> | ✅ 一致 |
| <code>FormView.PageIndex=3</code> | ✅ 一致 |
| <code>FormView.PagerSettings.FirstPageImageUrl=empty</code> | ✅ 一致 |
| <code>FormView.PagerSettings.FirstPageImageUrl=japanese</code> | ✅ 一致 |
| <code>FormView.PagerSettings.FirstPageImageUrl=markup</code> | ✅ 一致 |
| <code>FormView.PagerSettings.FirstPageImageUrl=text</code> | ✅ 一致 |
| <code>FormView.PagerSettings.FirstPageText=empty</code> | ✅ 一致 |
| <code>FormView.PagerSettings.FirstPageText=japanese</code> | ✅ 一致 |
| <code>FormView.PagerSettings.FirstPageText=markup</code> | ✅ 一致 |
| <code>FormView.PagerSettings.FirstPageText=text</code> | ✅ 一致 |
| <code>FormView.PagerSettings.LastPageImageUrl=empty</code> | ✅ 一致 |
| <code>FormView.PagerSettings.LastPageImageUrl=japanese</code> | ✅ 一致 |
| <code>FormView.PagerSettings.LastPageImageUrl=markup</code> | ✅ 一致 |
| <code>FormView.PagerSettings.LastPageImageUrl=text</code> | ✅ 一致 |
| <code>FormView.PagerSettings.LastPageText=empty</code> | ✅ 一致 |
| <code>FormView.PagerSettings.LastPageText=japanese</code> | ✅ 一致 |
| <code>FormView.PagerSettings.LastPageText=markup</code> | ✅ 一致 |
| <code>FormView.PagerSettings.LastPageText=text</code> | ✅ 一致 |
| <code>FormView.PagerSettings.Mode=NextPrevious</code> | ✅ 一致 |
| <code>FormView.PagerSettings.Mode=NextPreviousFirstLast</code> | ✅ 一致 |
| <code>FormView.PagerSettings.Mode=Numeric</code> | ✅ 一致 |
| <code>FormView.PagerSettings.Mode=NumericFirstLast</code> | ✅ 一致 |
| <code>FormView.PagerSettings.NextPageImageUrl=empty</code> | ✅ 一致 |
| <code>FormView.PagerSettings.NextPageImageUrl=japanese</code> | ✅ 一致 |
| <code>FormView.PagerSettings.NextPageImageUrl=markup</code> | ✅ 一致 |
| <code>FormView.PagerSettings.NextPageImageUrl=text</code> | ✅ 一致 |
| <code>FormView.PagerSettings.NextPageText=empty</code> | ✅ 一致 |
| <code>FormView.PagerSettings.NextPageText=japanese</code> | ✅ 一致 |
| <code>FormView.PagerSettings.NextPageText=markup</code> | ✅ 一致 |
| <code>FormView.PagerSettings.NextPageText=text</code> | ✅ 一致 |
| <code>FormView.PagerSettings.PageButtonCount=-1</code> | ✅ 一致 |
| <code>FormView.PagerSettings.PageButtonCount=0</code> | ✅ 一致 |
| <code>FormView.PagerSettings.PageButtonCount=3</code> | ✅ 一致 |
| <code>FormView.PagerSettings.Position=Bottom</code> | ✅ 一致 |
| <code>FormView.PagerSettings.Position=Top</code> | ✅ 一致 |
| <code>FormView.PagerSettings.Position=TopAndBottom</code> | ✅ 一致 |
| <code>FormView.PagerSettings.PreviousPageImageUrl=empty</code> | ✅ 一致 |
| <code>FormView.PagerSettings.PreviousPageImageUrl=japanese</code> | ✅ 一致 |
| <code>FormView.PagerSettings.PreviousPageImageUrl=markup</code> | ✅ 一致 |
| <code>FormView.PagerSettings.PreviousPageImageUrl=text</code> | ✅ 一致 |
| <code>FormView.PagerSettings.PreviousPageText=empty</code> | ✅ 一致 |
| <code>FormView.PagerSettings.PreviousPageText=japanese</code> | ✅ 一致 |
| <code>FormView.PagerSettings.PreviousPageText=markup</code> | ✅ 一致 |
| <code>FormView.PagerSettings.PreviousPageText=text</code> | ✅ 一致 |
| <code>FormView.PagerSettings.Visible=false</code> | ✅ 一致 |
| <code>FormView.PagerSettings.Visible=true</code> | ✅ 一致 |
| <code>FormView.PagerStyle.BackColor=Red</code> | ✅ 一致 |
| <code>FormView.PagerStyle.CssClass=text</code> | ✅ 一致 |
| <code>FormView.PagerStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>FormView.PagerStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>FormView.PagerStyle.Height=120px</code> | ✅ 一致 |
| <code>FormView.PagerStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>FormView.PagerStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>FormView.PagerStyle.Wrap=true</code> | ✅ 一致 |
| <code>FormView.RenderOuterTable=false</code> | ✅ 一致 |
| <code>FormView.RenderOuterTable=false+AllowPaging=true</code> | ✅ 一致 |
| <code>FormView.RenderOuterTable=true</code> | ✅ 一致 |
| <code>FormView.RowStyle.BackColor=Red</code> | ✅ 一致 |
| <code>FormView.RowStyle.CssClass=text</code> | ✅ 一致 |
| <code>FormView.RowStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>FormView.RowStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>FormView.RowStyle.Height=120px</code> | ✅ 一致 |
| <code>FormView.RowStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>FormView.RowStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>FormView.RowStyle.Wrap=true</code> | ✅ 一致 |
| <code>FormView.SelectMethod=empty</code> | ✅ 一致 |
| <code>FormView.SelectMethod=japanese</code> | ✅ 一致 |
| <code>FormView.SelectMethod=markup</code> | ✅ 一致 |
| <code>FormView.SelectMethod=text</code> | ✅ 一致 |
| <code>FormView.SkinID=empty</code> | ✅ 一致 |
| <code>FormView.SkinID=japanese</code> | ✅ 一致 |
| <code>FormView.SkinID=markup</code> | ✅ 一致 |
| <code>FormView.SkinID=text</code> | ✅ 一致 |
| <code>FormView.Style[color]=1em</code> | ✅ 一致 |
| <code>FormView.Style[color]=red</code> | ✅ 一致 |
| <code>FormView.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>FormView.Style[margin-left]=red</code> | ✅ 一致 |
| <code>FormView.TabIndex=-1</code> | ✅ 一致 |
| <code>FormView.TabIndex=0</code> | ✅ 一致 |
| <code>FormView.TabIndex=3</code> | ✅ 一致 |
| <code>FormView.ToolTip=empty</code> | ✅ 一致 |
| <code>FormView.ToolTip=japanese</code> | ✅ 一致 |
| <code>FormView.ToolTip=markup</code> | ✅ 一致 |
| <code>FormView.ToolTip=text</code> | ✅ 一致 |
| <code>FormView.UpdateMethod=empty</code> | ✅ 一致 |
| <code>FormView.UpdateMethod=japanese</code> | ✅ 一致 |
| <code>FormView.UpdateMethod=markup</code> | ✅ 一致 |
| <code>FormView.UpdateMethod=text</code> | ✅ 一致 |
| <code>FormView.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>FormView.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>FormView.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>FormView.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>FormView.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>FormView.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>FormView.Visible=false</code> | ✅ 一致 |
| <code>FormView.Visible=true</code> | ✅ 一致 |
| <code>FormView.Width=120px</code> | ✅ 一致 |
| <code>FormView.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>FormView.Width=2em</code> | ✅ 一致 |
| <code>FormView.Width=50%</code> | ✅ 一致 |
| <code>FormView[empty].(default)</code> | ✅ 一致 |
| <code>FormView[empty].DefaultMode=Insert</code> | ✅ 一致 |
| <code>FormView[empty].EmptyDataText=text</code> | ✅ 一致 |
| <code>FormView[naming container].(default)</code> | ✅ 一致 |
| <code>FormView[naming container].AllowPaging=true</code> | ✅ 一致 |
| <code>FormView[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>FormView[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>FormView[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>FormView[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>FormView[naming container].ID=text</code> | ✅ 一致 |
| <code>FormView[templates].(default)</code> | ✅ 一致 |
| <code>FormView[templates].AllowPaging=true</code> | ✅ 一致 |
| <code>FormView[templates].DefaultMode=Edit</code> | ✅ 一致 |
| <code>FormView[templates].HeaderText=text+FooterText=text</code> | ✅ 一致 |
| <code>FormView[themed].(default)</code> | ✅ 一致 |
| <code>FormView[themed].CssClass=text</code> | ✅ 一致 |
| <code>FormView[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>FormView[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| FormView [render][databind][datasource][datasourceid][templates][dynamic] first request | ✅ 一致 |
| FormView [paging][postbackevent] the page&#39;s form view: page 2 | ✅ 一致 |
| FormView [rowcommand] a command bubbled to ItemCommand | ✅ 一致 |
| FormView [editing] edit mode | ✅ 一致 |
| FormView [editing][bind][datakeys] updated: keys, old and new values | ✅ 一致 |
| FormView [inserting] insert mode | ✅ 一致 |
| FormView [inserting][bind] inserted | ✅ 一致 |
| FormView [editing][datakeys] deleted | ✅ 一致 |
| FormView [paging][datasourceid] the data source&#39;s form view: page 3 | ✅ 一致 |
| FormView [editing][datasourceid] updated through the data source | ✅ 一致 |
| FormView [inserting][datasourceid] inserted through the data source | ✅ 一致 |
| FormView [editing][datasourceid] deleted through the data source | ✅ 一致 |
| FormView [dynamic][paging] the dynamic form view: page 2 | ✅ 一致 |
| FormView [viewstate] posted (the one without view state is empty) | ✅ 一致 |
| FormView [visible][viewstate] the hidden form view shown | ✅ 一致 |

</details>
