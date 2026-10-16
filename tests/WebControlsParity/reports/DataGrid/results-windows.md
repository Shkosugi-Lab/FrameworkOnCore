# DataGrid パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-16 16:41
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 245 | 245 | 0 | 0 |
| ② HTML | 267 | 267 | 0 | 0 |
| ③ シナリオ | 12 | 12 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 254 件中 254 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 107 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 12 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 245 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 3 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 7 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 1 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 2 | 0 | ✅ |
| ページング | 18 | 1 | ✅ |
| 並べ替え | 3 | 2 | ✅ |
| 行・項目の編集（編集・更新・取り消し・削除） | 7 | 4 | ✅ |
| 行・項目の選択 | 6 | 1 | ✅ |
| 行・項目のコマンド（RowCommand・ItemCommand へのバブル） | 0 | 5 | ✅ |
| データソースコントロールとの連携（DataSourceID） | 0 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>DataGrid.Api.Control.#ctor</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.Adapter</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.ClientID</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.Context</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.Controls</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.DataBind</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.Dispose</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.Disposed</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.Events</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.FindControl</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.Focus</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.HasControls</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.ID</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.Init</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.Load</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.OnInit</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.Page</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.Parent</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.PreRender</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.Render</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.Site</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.SkinID</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.Unload</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.ViewState</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>DataGrid.Api.Control.Visible</code> | ✅ 一致 |
| <code>DataGrid.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>DataGrid.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>DataGrid.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>DataGrid.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>DataGrid.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>DataGrid.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>DataGrid.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>DataGrid.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>DataGrid.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>DataGrid.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>DataGrid.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>DataGrid.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>DataGrid.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.#ctor</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.AddParsedSubObject</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.Caption</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.CaptionAlign</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.CellPadding</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.CellSpacing</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.Controls</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.CreateChildControls</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.CreateControlHierarchy</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.CreateDataSourceSelectArguments</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.DataBind</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.DataKeyField</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.DataKeys</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.DataKeysArray</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.DataMember</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.DataSource</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.DataSourceID</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.EnsureDataBound</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.GetData</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.GridLines</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.HorizontalAlign</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.Initialized</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.IsBindableType</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.IsBoundUsingDataSourceID</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.OnDataBinding</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.OnDataSourceViewChanged</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.OnInit</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.OnLoad</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.OnPreRender</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.OnSelectedIndexChanged</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.PrepareControlHierarchy</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.Render</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.RequiresDataBinding</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.SelectArguments</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.SelectedIndexChanged</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.BaseDataList.UseAccessibleHeader</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.#ctor</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.AllowCustomPaging</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.AllowPaging</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.AllowSorting</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.AlternatingItemStyle</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.AutoGenerateColumns</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.BackImageUrl</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.CancelCommand</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.CancelCommandName</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.Columns</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.CreateColumnSet</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.CreateControlHierarchy</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.CreateControlStyle</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.CreateItem</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.CurrentPageIndex</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.DeleteCommand</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.DeleteCommandName</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.EditCommand</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.EditCommandName</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.EditItemIndex</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.EditItemStyle</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.FooterStyle</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.HeaderStyle</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.InitializeItem</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.InitializePager</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.ItemCommand</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.ItemCreated</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.ItemDataBound</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.ItemStyle</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.Items</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.LoadViewState</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.NextPageCommandArgument</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.OnBubbleEvent</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.OnCancelCommand</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.OnDeleteCommand</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.OnEditCommand</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.OnItemCommand</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.OnItemCreated</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.OnItemDataBound</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.OnPageIndexChanged</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.OnSortCommand</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.OnUpdateCommand</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.PageCommandName</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.PageCount</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.PageIndexChanged</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.PageSize</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.PagerStyle</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.PrepareControlHierarchy</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.PrevPageCommandArgument</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.SaveViewState</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.SelectCommandName</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.SelectedIndex</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.SelectedItem</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.SelectedItemStyle</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.ShowFooter</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.ShowHeader</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.SortCommand</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.SortCommandName</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.TagKey</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.TrackViewState</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.UpdateCommand</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.UpdateCommandName</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.DataGrid.VirtualItemCount</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>DataGrid.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>DataGrid.(case list)</code> | ✅ 一致 |
| <code>DataGrid.(default)</code> | ✅ 一致 |
| <code>DataGrid.AccessKey=empty</code> | ✅ 一致 |
| <code>DataGrid.AccessKey=japanese</code> | ✅ 一致 |
| <code>DataGrid.AccessKey=k</code> | ✅ 一致 |
| <code>DataGrid.AccessKey=markup</code> | ✅ 一致 |
| <code>DataGrid.AccessKey=text</code> | ✅ 一致 |
| <code>DataGrid.AllowCustomPaging=false</code> | ✅ 一致 |
| <code>DataGrid.AllowCustomPaging=true</code> | ✅ 一致 |
| <code>DataGrid.AllowCustomPaging=true+AllowPaging=true+PageSize=3+VirtualItemCount=3</code> | ✅ 一致 |
| <code>DataGrid.AllowPaging=false</code> | ✅ 一致 |
| <code>DataGrid.AllowPaging=true</code> | ✅ 一致 |
| <code>DataGrid.AllowPaging=true+PageSize=3</code> | ✅ 一致 |
| <code>DataGrid.AllowPaging=true+PageSize=3+CurrentPageIndex=1</code> | ✅ 一致 |
| <code>DataGrid.AllowPaging=true+PageSize=3+EnableViewState=false</code> | ✅ 一致 |
| <code>DataGrid.AllowPaging=true+PageSize=3+PagerStyle.Mode=NumericPages</code> | ✅ 一致 |
| <code>DataGrid.AllowPaging=true+PageSize=3+PagerStyle.Position=TopAndBottom</code> | ✅ 一致 |
| <code>DataGrid.AllowPaging=true+Visible=false</code> | ✅ 一致 |
| <code>DataGrid.AllowSorting=false</code> | ✅ 一致 |
| <code>DataGrid.AllowSorting=true</code> | ✅ 一致 |
| <code>DataGrid.AllowSorting=true+AllowPaging=true</code> | ✅ 一致 |
| <code>DataGrid.AlternatingItemStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DataGrid.AlternatingItemStyle.CssClass=text</code> | ✅ 一致 |
| <code>DataGrid.AlternatingItemStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>DataGrid.AlternatingItemStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>DataGrid.AlternatingItemStyle.Height=120px</code> | ✅ 一致 |
| <code>DataGrid.AlternatingItemStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DataGrid.AlternatingItemStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>DataGrid.AlternatingItemStyle.Wrap=true</code> | ✅ 一致 |
| <code>DataGrid.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>DataGrid.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>DataGrid.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>DataGrid.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>DataGrid.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>DataGrid.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>DataGrid.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>DataGrid.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>DataGrid.AutoGenerateColumns=false</code> | ✅ 一致 |
| <code>DataGrid.AutoGenerateColumns=true</code> | ✅ 一致 |
| <code>DataGrid.BackColor=#3366CC</code> | ✅ 一致 |
| <code>DataGrid.BackColor=Red</code> | ✅ 一致 |
| <code>DataGrid.BackColor=Transparent</code> | ✅ 一致 |
| <code>DataGrid.BackImageUrl=empty</code> | ✅ 一致 |
| <code>DataGrid.BackImageUrl=japanese</code> | ✅ 一致 |
| <code>DataGrid.BackImageUrl=markup</code> | ✅ 一致 |
| <code>DataGrid.BackImageUrl=text</code> | ✅ 一致 |
| <code>DataGrid.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>DataGrid.BorderColor=Red</code> | ✅ 一致 |
| <code>DataGrid.BorderColor=Transparent</code> | ✅ 一致 |
| <code>DataGrid.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>DataGrid.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>DataGrid.BorderStyle=Double</code> | ✅ 一致 |
| <code>DataGrid.BorderStyle=Groove</code> | ✅ 一致 |
| <code>DataGrid.BorderStyle=Inset</code> | ✅ 一致 |
| <code>DataGrid.BorderStyle=None</code> | ✅ 一致 |
| <code>DataGrid.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>DataGrid.BorderStyle=Outset</code> | ✅ 一致 |
| <code>DataGrid.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>DataGrid.BorderStyle=Solid</code> | ✅ 一致 |
| <code>DataGrid.BorderWidth=120px</code> | ✅ 一致 |
| <code>DataGrid.BorderWidth=2em</code> | ✅ 一致 |
| <code>DataGrid.BorderWidth=50%</code> | ✅ 一致 |
| <code>DataGrid.Caption=empty</code> | ✅ 一致 |
| <code>DataGrid.Caption=japanese</code> | ✅ 一致 |
| <code>DataGrid.Caption=markup</code> | ✅ 一致 |
| <code>DataGrid.Caption=text</code> | ✅ 一致 |
| <code>DataGrid.Caption=text+CaptionAlign=Bottom</code> | ✅ 一致 |
| <code>DataGrid.CaptionAlign=Bottom</code> | ✅ 一致 |
| <code>DataGrid.CaptionAlign=Left</code> | ✅ 一致 |
| <code>DataGrid.CaptionAlign=NotSet</code> | ✅ 一致 |
| <code>DataGrid.CaptionAlign=Right</code> | ✅ 一致 |
| <code>DataGrid.CaptionAlign=Top</code> | ✅ 一致 |
| <code>DataGrid.CellPadding=-1</code> | ✅ 一致 |
| <code>DataGrid.CellPadding=0</code> | ✅ 一致 |
| <code>DataGrid.CellPadding=3</code> | ✅ 一致 |
| <code>DataGrid.CellSpacing=-1</code> | ✅ 一致 |
| <code>DataGrid.CellSpacing=0</code> | ✅ 一致 |
| <code>DataGrid.CellSpacing=3</code> | ✅ 一致 |
| <code>DataGrid.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>DataGrid.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>DataGrid.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>DataGrid.ClientIDMode=Static</code> | ✅ 一致 |
| <code>DataGrid.CssClass=empty</code> | ✅ 一致 |
| <code>DataGrid.CssClass=japanese</code> | ✅ 一致 |
| <code>DataGrid.CssClass=markup</code> | ✅ 一致 |
| <code>DataGrid.CssClass=text</code> | ✅ 一致 |
| <code>DataGrid.CurrentPageIndex=-1</code> | ✅ 一致 |
| <code>DataGrid.CurrentPageIndex=0</code> | ✅ 一致 |
| <code>DataGrid.CurrentPageIndex=1</code> | ✅ 一致 |
| <code>DataGrid.CurrentPageIndex=3</code> | ✅ 一致 |
| <code>DataGrid.DataKeyField=Code</code> | ✅ 一致 |
| <code>DataGrid.DataKeyField=empty</code> | ✅ 一致 |
| <code>DataGrid.DataKeyField=japanese</code> | ✅ 一致 |
| <code>DataGrid.DataKeyField=markup</code> | ✅ 一致 |
| <code>DataGrid.DataKeyField=text</code> | ✅ 一致 |
| <code>DataGrid.DataMember=empty</code> | ✅ 一致 |
| <code>DataGrid.DataMember=japanese</code> | ✅ 一致 |
| <code>DataGrid.DataMember=markup</code> | ✅ 一致 |
| <code>DataGrid.DataMember=text</code> | ✅ 一致 |
| <code>DataGrid.DataSourceID=empty</code> | ✅ 一致 |
| <code>DataGrid.DataSourceID=japanese</code> | ✅ 一致 |
| <code>DataGrid.DataSourceID=markup</code> | ✅ 一致 |
| <code>DataGrid.DataSourceID=text</code> | ✅ 一致 |
| <code>DataGrid.EditItemIndex=-1</code> | ✅ 一致 |
| <code>DataGrid.EditItemIndex=0</code> | ✅ 一致 |
| <code>DataGrid.EditItemIndex=1</code> | ✅ 一致 |
| <code>DataGrid.EditItemIndex=1+EditItemStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DataGrid.EditItemIndex=3</code> | ✅ 一致 |
| <code>DataGrid.EditItemStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DataGrid.EditItemStyle.CssClass=text</code> | ✅ 一致 |
| <code>DataGrid.EditItemStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>DataGrid.EditItemStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>DataGrid.EditItemStyle.Height=120px</code> | ✅ 一致 |
| <code>DataGrid.EditItemStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DataGrid.EditItemStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>DataGrid.EditItemStyle.Wrap=true</code> | ✅ 一致 |
| <code>DataGrid.EnableTheming=false</code> | ✅ 一致 |
| <code>DataGrid.EnableTheming=true</code> | ✅ 一致 |
| <code>DataGrid.EnableViewState=false</code> | ✅ 一致 |
| <code>DataGrid.EnableViewState=true</code> | ✅ 一致 |
| <code>DataGrid.Enabled=false</code> | ✅ 一致 |
| <code>DataGrid.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>DataGrid.Enabled=true</code> | ✅ 一致 |
| <code>DataGrid.Font.Bold=false</code> | ✅ 一致 |
| <code>DataGrid.Font.Bold=true</code> | ✅ 一致 |
| <code>DataGrid.Font.Italic=false</code> | ✅ 一致 |
| <code>DataGrid.Font.Italic=true</code> | ✅ 一致 |
| <code>DataGrid.Font.Name=empty</code> | ✅ 一致 |
| <code>DataGrid.Font.Name=japanese</code> | ✅ 一致 |
| <code>DataGrid.Font.Name=markup</code> | ✅ 一致 |
| <code>DataGrid.Font.Name=text</code> | ✅ 一致 |
| <code>DataGrid.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>DataGrid.Font.Overline=false</code> | ✅ 一致 |
| <code>DataGrid.Font.Overline=true</code> | ✅ 一致 |
| <code>DataGrid.Font.Size=12pt</code> | ✅ 一致 |
| <code>DataGrid.Font.Size=150%</code> | ✅ 一致 |
| <code>DataGrid.Font.Size=Large</code> | ✅ 一致 |
| <code>DataGrid.Font.Strikeout=false</code> | ✅ 一致 |
| <code>DataGrid.Font.Strikeout=true</code> | ✅ 一致 |
| <code>DataGrid.Font.Underline=false</code> | ✅ 一致 |
| <code>DataGrid.Font.Underline=true</code> | ✅ 一致 |
| <code>DataGrid.FooterStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DataGrid.FooterStyle.CssClass=text</code> | ✅ 一致 |
| <code>DataGrid.FooterStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>DataGrid.FooterStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>DataGrid.FooterStyle.Height=120px</code> | ✅ 一致 |
| <code>DataGrid.FooterStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DataGrid.FooterStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>DataGrid.FooterStyle.Wrap=true</code> | ✅ 一致 |
| <code>DataGrid.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>DataGrid.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>DataGrid.ForeColor=Red</code> | ✅ 一致 |
| <code>DataGrid.ForeColor=Transparent</code> | ✅ 一致 |
| <code>DataGrid.GridLines=Both</code> | ✅ 一致 |
| <code>DataGrid.GridLines=Horizontal</code> | ✅ 一致 |
| <code>DataGrid.GridLines=None</code> | ✅ 一致 |
| <code>DataGrid.GridLines=None+CellPadding=3</code> | ✅ 一致 |
| <code>DataGrid.GridLines=Vertical</code> | ✅ 一致 |
| <code>DataGrid.HeaderStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DataGrid.HeaderStyle.CssClass=text</code> | ✅ 一致 |
| <code>DataGrid.HeaderStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>DataGrid.HeaderStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>DataGrid.HeaderStyle.Height=120px</code> | ✅ 一致 |
| <code>DataGrid.HeaderStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DataGrid.HeaderStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>DataGrid.HeaderStyle.Wrap=true</code> | ✅ 一致 |
| <code>DataGrid.Height=120px</code> | ✅ 一致 |
| <code>DataGrid.Height=2em</code> | ✅ 一致 |
| <code>DataGrid.Height=50%</code> | ✅ 一致 |
| <code>DataGrid.HorizontalAlign=Center</code> | ✅ 一致 |
| <code>DataGrid.HorizontalAlign=Justify</code> | ✅ 一致 |
| <code>DataGrid.HorizontalAlign=Left</code> | ✅ 一致 |
| <code>DataGrid.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DataGrid.HorizontalAlign=Right</code> | ✅ 一致 |
| <code>DataGrid.ID=empty</code> | ✅ 一致 |
| <code>DataGrid.ID=japanese</code> | ✅ 一致 |
| <code>DataGrid.ID=markup</code> | ✅ 一致 |
| <code>DataGrid.ID=text</code> | ✅ 一致 |
| <code>DataGrid.ItemStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DataGrid.ItemStyle.CssClass=text</code> | ✅ 一致 |
| <code>DataGrid.ItemStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>DataGrid.ItemStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>DataGrid.ItemStyle.Height=120px</code> | ✅ 一致 |
| <code>DataGrid.ItemStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DataGrid.ItemStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>DataGrid.ItemStyle.Wrap=true</code> | ✅ 一致 |
| <code>DataGrid.PageSize=-1</code> | ✅ 一致 |
| <code>DataGrid.PageSize=0</code> | ✅ 一致 |
| <code>DataGrid.PageSize=3</code> | ✅ 一致 |
| <code>DataGrid.PagerStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DataGrid.PagerStyle.CssClass=text</code> | ✅ 一致 |
| <code>DataGrid.PagerStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>DataGrid.PagerStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>DataGrid.PagerStyle.Height=120px</code> | ✅ 一致 |
| <code>DataGrid.PagerStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DataGrid.PagerStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>DataGrid.PagerStyle.Wrap=true</code> | ✅ 一致 |
| <code>DataGrid.SelectedIndex=-1</code> | ✅ 一致 |
| <code>DataGrid.SelectedIndex=0</code> | ✅ 一致 |
| <code>DataGrid.SelectedIndex=1</code> | ✅ 一致 |
| <code>DataGrid.SelectedIndex=1+SelectedItemStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DataGrid.SelectedIndex=3</code> | ✅ 一致 |
| <code>DataGrid.SelectedItemStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DataGrid.SelectedItemStyle.CssClass=text</code> | ✅ 一致 |
| <code>DataGrid.SelectedItemStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>DataGrid.SelectedItemStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>DataGrid.SelectedItemStyle.Height=120px</code> | ✅ 一致 |
| <code>DataGrid.SelectedItemStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DataGrid.SelectedItemStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>DataGrid.SelectedItemStyle.Wrap=true</code> | ✅ 一致 |
| <code>DataGrid.ShowFooter=false</code> | ✅ 一致 |
| <code>DataGrid.ShowFooter=true</code> | ✅ 一致 |
| <code>DataGrid.ShowFooter=true+FooterStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DataGrid.ShowHeader=false</code> | ✅ 一致 |
| <code>DataGrid.ShowHeader=true</code> | ✅ 一致 |
| <code>DataGrid.SkinID=empty</code> | ✅ 一致 |
| <code>DataGrid.SkinID=japanese</code> | ✅ 一致 |
| <code>DataGrid.SkinID=markup</code> | ✅ 一致 |
| <code>DataGrid.SkinID=text</code> | ✅ 一致 |
| <code>DataGrid.Style[color]=1em</code> | ✅ 一致 |
| <code>DataGrid.Style[color]=red</code> | ✅ 一致 |
| <code>DataGrid.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>DataGrid.Style[margin-left]=red</code> | ✅ 一致 |
| <code>DataGrid.TabIndex=-1</code> | ✅ 一致 |
| <code>DataGrid.TabIndex=0</code> | ✅ 一致 |
| <code>DataGrid.TabIndex=3</code> | ✅ 一致 |
| <code>DataGrid.ToolTip=empty</code> | ✅ 一致 |
| <code>DataGrid.ToolTip=japanese</code> | ✅ 一致 |
| <code>DataGrid.ToolTip=markup</code> | ✅ 一致 |
| <code>DataGrid.ToolTip=text</code> | ✅ 一致 |
| <code>DataGrid.UseAccessibleHeader=false</code> | ✅ 一致 |
| <code>DataGrid.UseAccessibleHeader=true</code> | ✅ 一致 |
| <code>DataGrid.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>DataGrid.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>DataGrid.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>DataGrid.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>DataGrid.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>DataGrid.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>DataGrid.VirtualItemCount=-1</code> | ✅ 一致 |
| <code>DataGrid.VirtualItemCount=0</code> | ✅ 一致 |
| <code>DataGrid.VirtualItemCount=3</code> | ✅ 一致 |
| <code>DataGrid.Visible=false</code> | ✅ 一致 |
| <code>DataGrid.Visible=true</code> | ✅ 一致 |
| <code>DataGrid.Width=120px</code> | ✅ 一致 |
| <code>DataGrid.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>DataGrid.Width=2em</code> | ✅ 一致 |
| <code>DataGrid.Width=50%</code> | ✅ 一致 |
| <code>DataGrid[columns].(default)</code> | ✅ 一致 |
| <code>DataGrid[columns].AllowPaging=true+PageSize=3</code> | ✅ 一致 |
| <code>DataGrid[columns].AllowSorting=true</code> | ✅ 一致 |
| <code>DataGrid[columns].DataKeyField=Code+EditItemIndex=1</code> | ✅ 一致 |
| <code>DataGrid[columns].EditItemIndex=1</code> | ✅ 一致 |
| <code>DataGrid[columns].SelectedIndex=1</code> | ✅ 一致 |
| <code>DataGrid[columns].ShowFooter=true</code> | ✅ 一致 |
| <code>DataGrid[empty].(default)</code> | ✅ 一致 |
| <code>DataGrid[empty].ShowHeader=false</code> | ✅ 一致 |
| <code>DataGrid[naming container].(default)</code> | ✅ 一致 |
| <code>DataGrid[naming container].AllowPaging=true+PageSize=3</code> | ✅ 一致 |
| <code>DataGrid[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>DataGrid[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>DataGrid[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>DataGrid[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>DataGrid[naming container].ID=text</code> | ✅ 一致 |
| <code>DataGrid[themed].(default)</code> | ✅ 一致 |
| <code>DataGrid[themed].CssClass=text</code> | ✅ 一致 |
| <code>DataGrid[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>DataGrid[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| DataGrid [render][databind][datasource][datasourceid][dynamic] first request | ✅ 一致 |
| DataGrid [paging][postbackevent] page 2: PageIndexChanged | ✅ 一致 |
| DataGrid [sorting] sorted by name: SortCommand | ✅ 一致 |
| DataGrid [sorting][viewstate] sorted by name again: descending (the page&#39;s) | ✅ 一致 |
| DataGrid [selection][rowcommand] the first row selected | ✅ 一致 |
| DataGrid [rowcommand] a ButtonColumn&#39;s command | ✅ 一致 |
| DataGrid [editing][rowcommand] edit: EditCommand | ✅ 一致 |
| DataGrid [editing][rowcommand] updated: UpdateCommand, the data key | ✅ 一致 |
| DataGrid [editing] cancelled: CancelCommand | ✅ 一致 |
| DataGrid [editing][rowcommand] deleted: DeleteCommand | ✅ 一致 |
| DataGrid [viewstate] posted (the one without view state is empty) | ✅ 一致 |
| DataGrid [visible][viewstate] the hidden data grid shown | ✅ 一致 |

</details>
