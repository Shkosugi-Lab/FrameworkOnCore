# DetailsView パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-16 17:52
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 327 | 327 | 0 | 0 |
| ② HTML | 352 | 351 | 1 | 0 |
| ③ シナリオ | 15 | 15 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 337 件中 337 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 148 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 15 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 328 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 2 | ✅ |
| Visible=false（描画されず状態は保持） | 3 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 7 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 1 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 2 | 0 | ✅ |
| ポストバックイベント | 0 | 1 | ✅ |
| ページング | 14 | 3 | ✅ |
| 項目の挿入 | 18 | 3 | ✅ |
| 行・項目の編集（編集・更新・取り消し・削除） | 8 | 5 | ✅ |
| 行・項目のコマンド（RowCommand・ItemCommand へのバブル） | 0 | 1 | ✅ |
| データキー（DataKeyNames・DataKeys） | 4 | 2 | ✅ |
| データソースコントロールとの連携（DataSourceID） | 0 | 5 | ✅ |
| テンプレート | 0 | 1 | ✅ |
| データソース連携（DataSource・DataTextField など） | 9 | 1 | ✅ |
| クライアントコールバック | 0 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

| 層 | ケース | 行 | 理由 |
|---|---|---|---|
| render | <code>^DetailsView\[empty\]\.DefaultMode=Insert$</code> | <code>ArgumentNullException: (Collection&#124;Value) cannot be null\. \[param c\]</code> | A details view in insert mode with no data throws the same ArgumentNullException (parameter c) at the same place on both: a collection made from no collection. Its message is the runtime&#39;s own resource (.NET Framework&#39;s ArrayList words it &quot;Collection cannot be null.&quot;, .NET&#39;s &quot;Value cannot be null.&quot;), not System.Web&#39;s. |

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>DetailsView.Api.Control.#ctor</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.Adapter</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.ClientID</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.Context</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.Controls</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.DataBind</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.Dispose</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.Disposed</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.Events</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.FindControl</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.Focus</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.HasControls</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.ID</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.Init</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.Load</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.OnInit</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.Page</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.Parent</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.PreRender</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.Render</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.Site</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.SkinID</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.Unload</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.ViewState</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>DetailsView.Api.Control.Visible</code> | ✅ 一致 |
| <code>DetailsView.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>DetailsView.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>DetailsView.Api.ICallbackEventHandler.GetCallbackResult</code> | ✅ 一致 |
| <code>DetailsView.Api.ICallbackEventHandler.RaiseCallbackEvent</code> | ✅ 一致 |
| <code>DetailsView.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>DetailsView.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>DetailsView.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>DetailsView.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>DetailsView.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>DetailsView.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>DetailsView.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>DetailsView.Api.IDataItemContainer.DataItem</code> | ✅ 一致 |
| <code>DetailsView.Api.IDataItemContainer.DataItemIndex</code> | ✅ 一致 |
| <code>DetailsView.Api.IDataItemContainer.DisplayIndex</code> | ✅ 一致 |
| <code>DetailsView.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>DetailsView.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>DetailsView.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>DetailsView.Api.IPostBackEventHandler.RaisePostBackEvent</code> | ✅ 一致 |
| <code>DetailsView.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.BaseDataBoundControl.#ctor</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.BaseDataBoundControl.ConfirmInitState</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.BaseDataBoundControl.DataBind</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.BaseDataBoundControl.DataBound</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.BaseDataBoundControl.DataSource</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.BaseDataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.BaseDataBoundControl.EnsureDataBound</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.BaseDataBoundControl.Initialized</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.BaseDataBoundControl.IsBoundUsingDataSourceID</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.BaseDataBoundControl.IsDataBindingAutomatic</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.BaseDataBoundControl.IsUsingModelBinders</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.BaseDataBoundControl.OnDataBound</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.BaseDataBoundControl.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.BaseDataBoundControl.OnInit</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.BaseDataBoundControl.OnPagePreLoad</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.BaseDataBoundControl.OnPreRender</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.BaseDataBoundControl.PerformSelect</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.BaseDataBoundControl.RequiresDataBinding</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.BaseDataBoundControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.BaseDataBoundControl.ValidateDataSource</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.CompositeDataBoundControl.#ctor</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.CompositeDataBoundControl.Controls</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.CompositeDataBoundControl.CreateChildControls</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.CompositeDataBoundControl.DeleteMethod</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.CompositeDataBoundControl.InsertMethod</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.CompositeDataBoundControl.IsUsingModelBinders</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.CompositeDataBoundControl.PerformDataBinding</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.CompositeDataBoundControl.UpdateMethod</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DataBoundControl.#ctor</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DataBoundControl.CallingDataMethods</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DataBoundControl.CreateDataSourceSelectArguments</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DataBoundControl.CreatingModelDataSource</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DataBoundControl.DataMember</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DataBoundControl.DataSourceObject</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DataBoundControl.GetData</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DataBoundControl.GetDataSource</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DataBoundControl.IsUsingModelBinders</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DataBoundControl.ItemType</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DataBoundControl.LoadViewState</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DataBoundControl.MarkAsDataBound</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DataBoundControl.OnCreatingModelDataSource</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DataBoundControl.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DataBoundControl.OnDataSourceViewChanged</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DataBoundControl.OnLoad</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DataBoundControl.OnPagePreLoad</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DataBoundControl.PerformDataBinding</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DataBoundControl.PerformSelect</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DataBoundControl.SaveViewState</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DataBoundControl.SelectArguments</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DataBoundControl.SelectMethod</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DataBoundControl.TrackViewState</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DataBoundControl.ValidateDataSource</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.#ctor</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.AllowPaging</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.AlternatingRowStyle</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.AutoGenerateDeleteButton</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.AutoGenerateEditButton</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.AutoGenerateInsertButton</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.AutoGenerateRows</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.BackImageUrl</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.BottomPagerRow</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.Caption</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.CaptionAlign</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.CellPadding</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.CellSpacing</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.ChangeMode</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.CommandRowStyle</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.CreateAutoGeneratedRow</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.CreateAutoGeneratedRows</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.CreateChildControls</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.CreateControlStyle</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.CreateDataSourceSelectArguments</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.CreateFieldSet</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.CreateRow</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.CreateTable</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.CurrentMode</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.DataBind</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.DataItem</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.DataItemCount</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.DataItemIndex</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.DataKey</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.DataKeyNames</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.DefaultMode</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.DeleteItem</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.DeleteMethod</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.EditRowStyle</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.EmptyDataRowStyle</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.EmptyDataTemplate</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.EmptyDataText</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.EnableModelValidation</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.EnablePagingCallbacks</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.EnsureDataBound</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.ExtractRowValues</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.FieldHeaderStyle</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.Fields</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.FooterRow</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.FooterStyle</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.FooterTemplate</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.FooterText</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.GetCallbackResult</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.GetCallbackScript</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.GridLines</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.HeaderRow</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.HeaderStyle</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.HeaderTemplate</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.HeaderText</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.HorizontalAlign</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.InitializePager</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.InitializeRow</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.InsertItem</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.InsertMethod</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.InsertRowStyle</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.IsBindableType</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.ItemCommand</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.ItemCreated</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.ItemDeleted</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.ItemDeleting</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.ItemInserted</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.ItemInserting</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.ItemUpdated</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.ItemUpdating</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.LoadControlState</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.LoadViewState</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.ModeChanged</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.ModeChanging</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.OnBubbleEvent</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.OnDataSourceViewChanged</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.OnInit</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.OnItemCommand</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.OnItemCreated</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.OnItemDeleted</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.OnItemDeleting</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.OnItemInserted</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.OnItemInserting</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.OnItemUpdated</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.OnItemUpdating</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.OnModeChanged</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.OnModeChanging</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.OnPageIndexChanged</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.OnPageIndexChanging</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.OnPagePreLoad</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.OnPreRender</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.PageCount</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.PageIndex</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.PageIndexChanged</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.PageIndexChanging</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.PagerSettings</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.PagerStyle</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.PagerTemplate</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.PerformDataBinding</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.PrepareControlHierarchy</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.RaiseCallbackEvent</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.RaisePostBackEvent</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.Render</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.RowStyle</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.Rows</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.RowsGenerator</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.SaveControlState</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.SaveViewState</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.SelectedValue</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.SetPageIndex</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.TagKey</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.TopPagerRow</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.TrackViewState</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.UpdateItem</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.DetailsView.UpdateMethod</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.ICallbackContainer.GetCallbackScript</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.IDataBoundControl.DataKeyNames</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.IDataBoundControl.DataMember</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.IDataBoundControl.DataSource</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.IDataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.IDataBoundControl.DataSourceObject</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.IDataBoundItemControl.DataKey</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.IDataBoundItemControl.Mode</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.IFieldControl.FieldsGenerator</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.IPostBackContainer.GetPostBackOptions</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>DetailsView.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>DetailsView.(case list)</code> | ✅ 一致 |
| <code>DetailsView.(default)</code> | ✅ 一致 |
| <code>DetailsView.AccessKey=empty</code> | ✅ 一致 |
| <code>DetailsView.AccessKey=japanese</code> | ✅ 一致 |
| <code>DetailsView.AccessKey=k</code> | ✅ 一致 |
| <code>DetailsView.AccessKey=markup</code> | ✅ 一致 |
| <code>DetailsView.AccessKey=text</code> | ✅ 一致 |
| <code>DetailsView.AllowPaging=false</code> | ✅ 一致 |
| <code>DetailsView.AllowPaging=true</code> | ✅ 一致 |
| <code>DetailsView.AllowPaging=true+EnablePagingCallbacks=true</code> | ✅ 一致 |
| <code>DetailsView.AllowPaging=true+EnableViewState=false</code> | ✅ 一致 |
| <code>DetailsView.AllowPaging=true+PageIndex=1</code> | ✅ 一致 |
| <code>DetailsView.AllowPaging=true+PagerSettings.Mode=NextPrevious</code> | ✅ 一致 |
| <code>DetailsView.AllowPaging=true+Visible=false</code> | ✅ 一致 |
| <code>DetailsView.AlternatingRowStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DetailsView.AlternatingRowStyle.CssClass=text</code> | ✅ 一致 |
| <code>DetailsView.AlternatingRowStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>DetailsView.AlternatingRowStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>DetailsView.AlternatingRowStyle.Height=120px</code> | ✅ 一致 |
| <code>DetailsView.AlternatingRowStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DetailsView.AlternatingRowStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>DetailsView.AlternatingRowStyle.Wrap=true</code> | ✅ 一致 |
| <code>DetailsView.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>DetailsView.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>DetailsView.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>DetailsView.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>DetailsView.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>DetailsView.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>DetailsView.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>DetailsView.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>DetailsView.AutoGenerateDeleteButton=false</code> | ✅ 一致 |
| <code>DetailsView.AutoGenerateDeleteButton=true</code> | ✅ 一致 |
| <code>DetailsView.AutoGenerateEditButton=false</code> | ✅ 一致 |
| <code>DetailsView.AutoGenerateEditButton=true</code> | ✅ 一致 |
| <code>DetailsView.AutoGenerateEditButton=true+AutoGenerateInsertButton=true+AutoGenerateDeleteButton=true</code> | ✅ 一致 |
| <code>DetailsView.AutoGenerateInsertButton=false</code> | ✅ 一致 |
| <code>DetailsView.AutoGenerateInsertButton=true</code> | ✅ 一致 |
| <code>DetailsView.AutoGenerateRows=false</code> | ✅ 一致 |
| <code>DetailsView.AutoGenerateRows=true</code> | ✅ 一致 |
| <code>DetailsView.BackColor=#3366CC</code> | ✅ 一致 |
| <code>DetailsView.BackColor=Red</code> | ✅ 一致 |
| <code>DetailsView.BackColor=Transparent</code> | ✅ 一致 |
| <code>DetailsView.BackImageUrl=empty</code> | ✅ 一致 |
| <code>DetailsView.BackImageUrl=japanese</code> | ✅ 一致 |
| <code>DetailsView.BackImageUrl=markup</code> | ✅ 一致 |
| <code>DetailsView.BackImageUrl=text</code> | ✅ 一致 |
| <code>DetailsView.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>DetailsView.BorderColor=Red</code> | ✅ 一致 |
| <code>DetailsView.BorderColor=Transparent</code> | ✅ 一致 |
| <code>DetailsView.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>DetailsView.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>DetailsView.BorderStyle=Double</code> | ✅ 一致 |
| <code>DetailsView.BorderStyle=Groove</code> | ✅ 一致 |
| <code>DetailsView.BorderStyle=Inset</code> | ✅ 一致 |
| <code>DetailsView.BorderStyle=None</code> | ✅ 一致 |
| <code>DetailsView.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>DetailsView.BorderStyle=Outset</code> | ✅ 一致 |
| <code>DetailsView.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>DetailsView.BorderStyle=Solid</code> | ✅ 一致 |
| <code>DetailsView.BorderWidth=120px</code> | ✅ 一致 |
| <code>DetailsView.BorderWidth=2em</code> | ✅ 一致 |
| <code>DetailsView.BorderWidth=50%</code> | ✅ 一致 |
| <code>DetailsView.Caption=empty</code> | ✅ 一致 |
| <code>DetailsView.Caption=japanese</code> | ✅ 一致 |
| <code>DetailsView.Caption=markup</code> | ✅ 一致 |
| <code>DetailsView.Caption=text</code> | ✅ 一致 |
| <code>DetailsView.Caption=text+CaptionAlign=Bottom</code> | ✅ 一致 |
| <code>DetailsView.CaptionAlign=Bottom</code> | ✅ 一致 |
| <code>DetailsView.CaptionAlign=Left</code> | ✅ 一致 |
| <code>DetailsView.CaptionAlign=NotSet</code> | ✅ 一致 |
| <code>DetailsView.CaptionAlign=Right</code> | ✅ 一致 |
| <code>DetailsView.CaptionAlign=Top</code> | ✅ 一致 |
| <code>DetailsView.CellPadding=-1</code> | ✅ 一致 |
| <code>DetailsView.CellPadding=0</code> | ✅ 一致 |
| <code>DetailsView.CellPadding=3</code> | ✅ 一致 |
| <code>DetailsView.CellSpacing=-1</code> | ✅ 一致 |
| <code>DetailsView.CellSpacing=0</code> | ✅ 一致 |
| <code>DetailsView.CellSpacing=3</code> | ✅ 一致 |
| <code>DetailsView.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>DetailsView.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>DetailsView.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>DetailsView.ClientIDMode=Static</code> | ✅ 一致 |
| <code>DetailsView.CommandRowStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DetailsView.CommandRowStyle.CssClass=text</code> | ✅ 一致 |
| <code>DetailsView.CommandRowStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>DetailsView.CommandRowStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>DetailsView.CommandRowStyle.Height=120px</code> | ✅ 一致 |
| <code>DetailsView.CommandRowStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DetailsView.CommandRowStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>DetailsView.CommandRowStyle.Wrap=true</code> | ✅ 一致 |
| <code>DetailsView.CssClass=empty</code> | ✅ 一致 |
| <code>DetailsView.CssClass=japanese</code> | ✅ 一致 |
| <code>DetailsView.CssClass=markup</code> | ✅ 一致 |
| <code>DetailsView.CssClass=text</code> | ✅ 一致 |
| <code>DetailsView.DataKeyNames=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>DetailsView.DataKeyNames=Code</code> | ✅ 一致 |
| <code>DetailsView.DataMember=empty</code> | ✅ 一致 |
| <code>DetailsView.DataMember=japanese</code> | ✅ 一致 |
| <code>DetailsView.DataMember=markup</code> | ✅ 一致 |
| <code>DetailsView.DataMember=text</code> | ✅ 一致 |
| <code>DetailsView.DataSourceID=empty</code> | ✅ 一致 |
| <code>DetailsView.DataSourceID=japanese</code> | ✅ 一致 |
| <code>DetailsView.DataSourceID=markup</code> | ✅ 一致 |
| <code>DetailsView.DataSourceID=text</code> | ✅ 一致 |
| <code>DetailsView.DefaultMode=Edit</code> | ✅ 一致 |
| <code>DetailsView.DefaultMode=Edit+DataKeyNames=Code</code> | ✅ 一致 |
| <code>DetailsView.DefaultMode=Insert</code> | ✅ 一致 |
| <code>DetailsView.DefaultMode=ReadOnly</code> | ✅ 一致 |
| <code>DetailsView.DeleteMethod=empty</code> | ✅ 一致 |
| <code>DetailsView.DeleteMethod=japanese</code> | ✅ 一致 |
| <code>DetailsView.DeleteMethod=markup</code> | ✅ 一致 |
| <code>DetailsView.DeleteMethod=text</code> | ✅ 一致 |
| <code>DetailsView.EditRowStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DetailsView.EditRowStyle.CssClass=text</code> | ✅ 一致 |
| <code>DetailsView.EditRowStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>DetailsView.EditRowStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>DetailsView.EditRowStyle.Height=120px</code> | ✅ 一致 |
| <code>DetailsView.EditRowStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DetailsView.EditRowStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>DetailsView.EditRowStyle.Wrap=true</code> | ✅ 一致 |
| <code>DetailsView.EmptyDataRowStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DetailsView.EmptyDataRowStyle.CssClass=text</code> | ✅ 一致 |
| <code>DetailsView.EmptyDataRowStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>DetailsView.EmptyDataRowStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>DetailsView.EmptyDataRowStyle.Height=120px</code> | ✅ 一致 |
| <code>DetailsView.EmptyDataRowStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DetailsView.EmptyDataRowStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>DetailsView.EmptyDataRowStyle.Wrap=true</code> | ✅ 一致 |
| <code>DetailsView.EmptyDataText=empty</code> | ✅ 一致 |
| <code>DetailsView.EmptyDataText=japanese</code> | ✅ 一致 |
| <code>DetailsView.EmptyDataText=markup</code> | ✅ 一致 |
| <code>DetailsView.EmptyDataText=text</code> | ✅ 一致 |
| <code>DetailsView.EnableModelValidation=false</code> | ✅ 一致 |
| <code>DetailsView.EnableModelValidation=true</code> | ✅ 一致 |
| <code>DetailsView.EnablePagingCallbacks=false</code> | ✅ 一致 |
| <code>DetailsView.EnablePagingCallbacks=true</code> | ✅ 一致 |
| <code>DetailsView.EnableTheming=false</code> | ✅ 一致 |
| <code>DetailsView.EnableTheming=true</code> | ✅ 一致 |
| <code>DetailsView.EnableViewState=false</code> | ✅ 一致 |
| <code>DetailsView.EnableViewState=true</code> | ✅ 一致 |
| <code>DetailsView.Enabled=false</code> | ✅ 一致 |
| <code>DetailsView.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>DetailsView.Enabled=true</code> | ✅ 一致 |
| <code>DetailsView.FieldHeaderStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DetailsView.FieldHeaderStyle.CssClass=text</code> | ✅ 一致 |
| <code>DetailsView.FieldHeaderStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>DetailsView.FieldHeaderStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>DetailsView.FieldHeaderStyle.Height=120px</code> | ✅ 一致 |
| <code>DetailsView.FieldHeaderStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DetailsView.FieldHeaderStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>DetailsView.FieldHeaderStyle.Wrap=true</code> | ✅ 一致 |
| <code>DetailsView.Font.Bold=false</code> | ✅ 一致 |
| <code>DetailsView.Font.Bold=true</code> | ✅ 一致 |
| <code>DetailsView.Font.Italic=false</code> | ✅ 一致 |
| <code>DetailsView.Font.Italic=true</code> | ✅ 一致 |
| <code>DetailsView.Font.Name=empty</code> | ✅ 一致 |
| <code>DetailsView.Font.Name=japanese</code> | ✅ 一致 |
| <code>DetailsView.Font.Name=markup</code> | ✅ 一致 |
| <code>DetailsView.Font.Name=text</code> | ✅ 一致 |
| <code>DetailsView.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>DetailsView.Font.Overline=false</code> | ✅ 一致 |
| <code>DetailsView.Font.Overline=true</code> | ✅ 一致 |
| <code>DetailsView.Font.Size=12pt</code> | ✅ 一致 |
| <code>DetailsView.Font.Size=150%</code> | ✅ 一致 |
| <code>DetailsView.Font.Size=Large</code> | ✅ 一致 |
| <code>DetailsView.Font.Strikeout=false</code> | ✅ 一致 |
| <code>DetailsView.Font.Strikeout=true</code> | ✅ 一致 |
| <code>DetailsView.Font.Underline=false</code> | ✅ 一致 |
| <code>DetailsView.Font.Underline=true</code> | ✅ 一致 |
| <code>DetailsView.FooterStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DetailsView.FooterStyle.CssClass=text</code> | ✅ 一致 |
| <code>DetailsView.FooterStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>DetailsView.FooterStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>DetailsView.FooterStyle.Height=120px</code> | ✅ 一致 |
| <code>DetailsView.FooterStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DetailsView.FooterStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>DetailsView.FooterStyle.Wrap=true</code> | ✅ 一致 |
| <code>DetailsView.FooterText=empty</code> | ✅ 一致 |
| <code>DetailsView.FooterText=japanese</code> | ✅ 一致 |
| <code>DetailsView.FooterText=markup</code> | ✅ 一致 |
| <code>DetailsView.FooterText=text</code> | ✅ 一致 |
| <code>DetailsView.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>DetailsView.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>DetailsView.ForeColor=Red</code> | ✅ 一致 |
| <code>DetailsView.ForeColor=Transparent</code> | ✅ 一致 |
| <code>DetailsView.GridLines=Both</code> | ✅ 一致 |
| <code>DetailsView.GridLines=Horizontal</code> | ✅ 一致 |
| <code>DetailsView.GridLines=None</code> | ✅ 一致 |
| <code>DetailsView.GridLines=None+CellPadding=3</code> | ✅ 一致 |
| <code>DetailsView.GridLines=Vertical</code> | ✅ 一致 |
| <code>DetailsView.HeaderStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DetailsView.HeaderStyle.CssClass=text</code> | ✅ 一致 |
| <code>DetailsView.HeaderStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>DetailsView.HeaderStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>DetailsView.HeaderStyle.Height=120px</code> | ✅ 一致 |
| <code>DetailsView.HeaderStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DetailsView.HeaderStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>DetailsView.HeaderStyle.Wrap=true</code> | ✅ 一致 |
| <code>DetailsView.HeaderText=empty</code> | ✅ 一致 |
| <code>DetailsView.HeaderText=japanese</code> | ✅ 一致 |
| <code>DetailsView.HeaderText=markup</code> | ✅ 一致 |
| <code>DetailsView.HeaderText=text</code> | ✅ 一致 |
| <code>DetailsView.HeaderText=text+FooterText=markup</code> | ✅ 一致 |
| <code>DetailsView.Height=120px</code> | ✅ 一致 |
| <code>DetailsView.Height=2em</code> | ✅ 一致 |
| <code>DetailsView.Height=50%</code> | ✅ 一致 |
| <code>DetailsView.HorizontalAlign=Center</code> | ✅ 一致 |
| <code>DetailsView.HorizontalAlign=Justify</code> | ✅ 一致 |
| <code>DetailsView.HorizontalAlign=Left</code> | ✅ 一致 |
| <code>DetailsView.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DetailsView.HorizontalAlign=Right</code> | ✅ 一致 |
| <code>DetailsView.ID=empty</code> | ✅ 一致 |
| <code>DetailsView.ID=japanese</code> | ✅ 一致 |
| <code>DetailsView.ID=markup</code> | ✅ 一致 |
| <code>DetailsView.ID=text</code> | ✅ 一致 |
| <code>DetailsView.InsertMethod=empty</code> | ✅ 一致 |
| <code>DetailsView.InsertMethod=japanese</code> | ✅ 一致 |
| <code>DetailsView.InsertMethod=markup</code> | ✅ 一致 |
| <code>DetailsView.InsertMethod=text</code> | ✅ 一致 |
| <code>DetailsView.InsertRowStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DetailsView.InsertRowStyle.CssClass=text</code> | ✅ 一致 |
| <code>DetailsView.InsertRowStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>DetailsView.InsertRowStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>DetailsView.InsertRowStyle.Height=120px</code> | ✅ 一致 |
| <code>DetailsView.InsertRowStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DetailsView.InsertRowStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>DetailsView.InsertRowStyle.Wrap=true</code> | ✅ 一致 |
| <code>DetailsView.ItemType=empty</code> | ✅ 一致 |
| <code>DetailsView.ItemType=japanese</code> | ✅ 一致 |
| <code>DetailsView.ItemType=markup</code> | ✅ 一致 |
| <code>DetailsView.ItemType=text</code> | ✅ 一致 |
| <code>DetailsView.PageIndex=-1</code> | ✅ 一致 |
| <code>DetailsView.PageIndex=0</code> | ✅ 一致 |
| <code>DetailsView.PageIndex=1</code> | ✅ 一致 |
| <code>DetailsView.PageIndex=3</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.FirstPageImageUrl=empty</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.FirstPageImageUrl=japanese</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.FirstPageImageUrl=markup</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.FirstPageImageUrl=text</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.FirstPageText=empty</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.FirstPageText=japanese</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.FirstPageText=markup</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.FirstPageText=text</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.LastPageImageUrl=empty</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.LastPageImageUrl=japanese</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.LastPageImageUrl=markup</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.LastPageImageUrl=text</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.LastPageText=empty</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.LastPageText=japanese</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.LastPageText=markup</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.LastPageText=text</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.Mode=NextPrevious</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.Mode=NextPreviousFirstLast</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.Mode=Numeric</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.Mode=NumericFirstLast</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.NextPageImageUrl=empty</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.NextPageImageUrl=japanese</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.NextPageImageUrl=markup</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.NextPageImageUrl=text</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.NextPageText=empty</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.NextPageText=japanese</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.NextPageText=markup</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.NextPageText=text</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.PageButtonCount=-1</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.PageButtonCount=0</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.PageButtonCount=3</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.Position=Bottom</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.Position=Top</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.Position=TopAndBottom</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.PreviousPageImageUrl=empty</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.PreviousPageImageUrl=japanese</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.PreviousPageImageUrl=markup</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.PreviousPageImageUrl=text</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.PreviousPageText=empty</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.PreviousPageText=japanese</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.PreviousPageText=markup</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.PreviousPageText=text</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.Visible=false</code> | ✅ 一致 |
| <code>DetailsView.PagerSettings.Visible=true</code> | ✅ 一致 |
| <code>DetailsView.PagerStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DetailsView.PagerStyle.CssClass=text</code> | ✅ 一致 |
| <code>DetailsView.PagerStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>DetailsView.PagerStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>DetailsView.PagerStyle.Height=120px</code> | ✅ 一致 |
| <code>DetailsView.PagerStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DetailsView.PagerStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>DetailsView.PagerStyle.Wrap=true</code> | ✅ 一致 |
| <code>DetailsView.RowStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DetailsView.RowStyle.CssClass=text</code> | ✅ 一致 |
| <code>DetailsView.RowStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>DetailsView.RowStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>DetailsView.RowStyle.Height=120px</code> | ✅ 一致 |
| <code>DetailsView.RowStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DetailsView.RowStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>DetailsView.RowStyle.Wrap=true</code> | ✅ 一致 |
| <code>DetailsView.SelectMethod=empty</code> | ✅ 一致 |
| <code>DetailsView.SelectMethod=japanese</code> | ✅ 一致 |
| <code>DetailsView.SelectMethod=markup</code> | ✅ 一致 |
| <code>DetailsView.SelectMethod=text</code> | ✅ 一致 |
| <code>DetailsView.SkinID=empty</code> | ✅ 一致 |
| <code>DetailsView.SkinID=japanese</code> | ✅ 一致 |
| <code>DetailsView.SkinID=markup</code> | ✅ 一致 |
| <code>DetailsView.SkinID=text</code> | ✅ 一致 |
| <code>DetailsView.Style[color]=1em</code> | ✅ 一致 |
| <code>DetailsView.Style[color]=red</code> | ✅ 一致 |
| <code>DetailsView.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>DetailsView.Style[margin-left]=red</code> | ✅ 一致 |
| <code>DetailsView.TabIndex=-1</code> | ✅ 一致 |
| <code>DetailsView.TabIndex=0</code> | ✅ 一致 |
| <code>DetailsView.TabIndex=3</code> | ✅ 一致 |
| <code>DetailsView.ToolTip=empty</code> | ✅ 一致 |
| <code>DetailsView.ToolTip=japanese</code> | ✅ 一致 |
| <code>DetailsView.ToolTip=markup</code> | ✅ 一致 |
| <code>DetailsView.ToolTip=text</code> | ✅ 一致 |
| <code>DetailsView.UpdateMethod=empty</code> | ✅ 一致 |
| <code>DetailsView.UpdateMethod=japanese</code> | ✅ 一致 |
| <code>DetailsView.UpdateMethod=markup</code> | ✅ 一致 |
| <code>DetailsView.UpdateMethod=text</code> | ✅ 一致 |
| <code>DetailsView.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>DetailsView.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>DetailsView.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>DetailsView.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>DetailsView.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>DetailsView.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>DetailsView.Visible=false</code> | ✅ 一致 |
| <code>DetailsView.Visible=true</code> | ✅ 一致 |
| <code>DetailsView.Width=120px</code> | ✅ 一致 |
| <code>DetailsView.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>DetailsView.Width=2em</code> | ✅ 一致 |
| <code>DetailsView.Width=50%</code> | ✅ 一致 |
| <code>DetailsView[empty].(default)</code> | ✅ 一致 |
| <code>DetailsView[empty].DefaultMode=Insert</code> | ⚠️ 既知の差分 |
| <code>DetailsView[empty].EmptyDataText=text</code> | ✅ 一致 |
| <code>DetailsView[fields].(default)</code> | ✅ 一致 |
| <code>DetailsView[fields].AllowPaging=true</code> | ✅ 一致 |
| <code>DetailsView[fields].AllowPaging=true+PageIndex=1</code> | ✅ 一致 |
| <code>DetailsView[fields].DefaultMode=Edit</code> | ✅ 一致 |
| <code>DetailsView[fields].DefaultMode=Edit+DataKeyNames=Code</code> | ✅ 一致 |
| <code>DetailsView[fields].DefaultMode=Insert</code> | ✅ 一致 |
| <code>DetailsView[naming container].(default)</code> | ✅ 一致 |
| <code>DetailsView[naming container].AllowPaging=true</code> | ✅ 一致 |
| <code>DetailsView[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>DetailsView[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>DetailsView[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>DetailsView[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>DetailsView[naming container].ID=text</code> | ✅ 一致 |
| <code>DetailsView[templates].(default)</code> | ✅ 一致 |
| <code>DetailsView[templates].AllowPaging=true</code> | ✅ 一致 |
| <code>DetailsView[themed].(default)</code> | ✅ 一致 |
| <code>DetailsView[themed].CssClass=text</code> | ✅ 一致 |
| <code>DetailsView[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>DetailsView[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| DetailsView [render][databind][datasource][datasourceid][templates][dynamic] first request | ✅ 一致 |
| DetailsView [paging][postbackevent] the page&#39;s details view: page 2 | ✅ 一致 |
| DetailsView [rowcommand] a ButtonField&#39;s command | ✅ 一致 |
| DetailsView [editing] edit mode | ✅ 一致 |
| DetailsView [editing][bind][datakeys] updated: keys, old and new values | ✅ 一致 |
| DetailsView [inserting] insert mode | ✅ 一致 |
| DetailsView [inserting][bind] inserted | ✅ 一致 |
| DetailsView [editing][datakeys] deleted | ✅ 一致 |
| DetailsView [paging][datasourceid] the data source&#39;s details view: page 3 | ✅ 一致 |
| DetailsView [editing][datasourceid] updated through the data source | ✅ 一致 |
| DetailsView [inserting][datasourceid] inserted through the data source | ✅ 一致 |
| DetailsView [editing][datasourceid] deleted through the data source | ✅ 一致 |
| DetailsView [callback][paging] paged by a callback: no postback | ✅ 一致 |
| DetailsView [viewstate] posted (the one without view state is empty) | ✅ 一致 |
| DetailsView [visible][viewstate] the hidden details view shown | ✅ 一致 |

</details>
