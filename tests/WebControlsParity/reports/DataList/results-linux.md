# DataList パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-16 17:56
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 246 | 246 | 0 | 0 |
| ② HTML | 257 | 257 | 0 | 0 |
| ③ シナリオ | 9 | 9 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 255 件中 255 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 103 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 9 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 231 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 2 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 7 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 1 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 2 | 0 | ✅ |
| 繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection） | 20 | 1 | ✅ |
| 行・項目の編集（編集・更新・取り消し・削除） | 5 | 4 | ✅ |
| 行・項目の選択 | 7 | 1 | ✅ |
| 行・項目のコマンド（RowCommand・ItemCommand へのバブル） | 0 | 5 | ✅ |
| データソースコントロールとの連携（DataSourceID） | 0 | 1 | ✅ |
| テンプレート | 0 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>DataList.Api.Control.#ctor</code> | ✅ 一致 |
| <code>DataList.Api.Control.Adapter</code> | ✅ 一致 |
| <code>DataList.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>DataList.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>DataList.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>DataList.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>DataList.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>DataList.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>DataList.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>DataList.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>DataList.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>DataList.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>DataList.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>DataList.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>DataList.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>DataList.Api.Control.ClientID</code> | ✅ 一致 |
| <code>DataList.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>DataList.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>DataList.Api.Control.Context</code> | ✅ 一致 |
| <code>DataList.Api.Control.Controls</code> | ✅ 一致 |
| <code>DataList.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>DataList.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>DataList.Api.Control.DataBind</code> | ✅ 一致 |
| <code>DataList.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>DataList.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>DataList.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>DataList.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>DataList.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>DataList.Api.Control.Dispose</code> | ✅ 一致 |
| <code>DataList.Api.Control.Disposed</code> | ✅ 一致 |
| <code>DataList.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>DataList.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>DataList.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>DataList.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>DataList.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>DataList.Api.Control.Events</code> | ✅ 一致 |
| <code>DataList.Api.Control.FindControl</code> | ✅ 一致 |
| <code>DataList.Api.Control.Focus</code> | ✅ 一致 |
| <code>DataList.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>DataList.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>DataList.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>DataList.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>DataList.Api.Control.HasControls</code> | ✅ 一致 |
| <code>DataList.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>DataList.Api.Control.ID</code> | ✅ 一致 |
| <code>DataList.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>DataList.Api.Control.Init</code> | ✅ 一致 |
| <code>DataList.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>DataList.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>DataList.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>DataList.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>DataList.Api.Control.Load</code> | ✅ 一致 |
| <code>DataList.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>DataList.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>DataList.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>DataList.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>DataList.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>DataList.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>DataList.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>DataList.Api.Control.OnInit</code> | ✅ 一致 |
| <code>DataList.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>DataList.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>DataList.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>DataList.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>DataList.Api.Control.Page</code> | ✅ 一致 |
| <code>DataList.Api.Control.Parent</code> | ✅ 一致 |
| <code>DataList.Api.Control.PreRender</code> | ✅ 一致 |
| <code>DataList.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>DataList.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>DataList.Api.Control.Render</code> | ✅ 一致 |
| <code>DataList.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>DataList.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>DataList.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>DataList.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>DataList.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>DataList.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>DataList.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>DataList.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>DataList.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>DataList.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>DataList.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>DataList.Api.Control.Site</code> | ✅ 一致 |
| <code>DataList.Api.Control.SkinID</code> | ✅ 一致 |
| <code>DataList.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>DataList.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>DataList.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>DataList.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>DataList.Api.Control.Unload</code> | ✅ 一致 |
| <code>DataList.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>DataList.Api.Control.ViewState</code> | ✅ 一致 |
| <code>DataList.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>DataList.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>DataList.Api.Control.Visible</code> | ✅ 一致 |
| <code>DataList.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>DataList.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>DataList.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>DataList.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>DataList.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>DataList.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>DataList.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>DataList.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>DataList.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>DataList.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>DataList.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>DataList.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>DataList.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.#ctor</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.AddParsedSubObject</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.Caption</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.CaptionAlign</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.CellPadding</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.CellSpacing</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.Controls</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.CreateChildControls</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.CreateControlHierarchy</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.CreateDataSourceSelectArguments</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.DataBind</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.DataKeyField</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.DataKeys</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.DataKeysArray</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.DataMember</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.DataSource</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.DataSourceID</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.EnsureDataBound</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.GetData</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.GridLines</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.HorizontalAlign</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.Initialized</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.IsBindableType</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.IsBoundUsingDataSourceID</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.OnDataBinding</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.OnDataSourceViewChanged</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.OnInit</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.OnLoad</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.OnPreRender</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.OnSelectedIndexChanged</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.PrepareControlHierarchy</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.Render</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.RequiresDataBinding</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.SelectArguments</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.SelectedIndexChanged</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.BaseDataList.UseAccessibleHeader</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.#ctor</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.AlternatingItemStyle</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.AlternatingItemTemplate</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.CancelCommand</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.CancelCommandName</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.CreateControlHierarchy</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.CreateControlStyle</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.CreateItem</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.DeleteCommand</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.DeleteCommandName</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.EditCommand</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.EditCommandName</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.EditItemIndex</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.EditItemStyle</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.EditItemTemplate</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.ExtractTemplateRows</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.FooterStyle</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.FooterTemplate</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.GridLines</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.HeaderStyle</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.HeaderTemplate</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.InitializeItem</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.ItemCommand</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.ItemCreated</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.ItemDataBound</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.ItemStyle</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.ItemTemplate</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.Items</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.LoadViewState</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.OnBubbleEvent</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.OnCancelCommand</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.OnDeleteCommand</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.OnEditCommand</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.OnInit</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.OnItemCommand</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.OnItemCreated</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.OnItemDataBound</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.OnUpdateCommand</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.PrepareControlHierarchy</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.RenderContents</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.RepeatColumns</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.RepeatDirection</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.RepeatLayout</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.SaveViewState</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.SelectCommandName</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.SelectedIndex</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.SelectedItem</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.SelectedItemStyle</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.SelectedItemTemplate</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.SelectedValue</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.SeparatorStyle</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.SeparatorTemplate</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.ShowFooter</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.ShowHeader</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.TagKey</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.TrackViewState</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.UpdateCommand</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.DataList.UpdateCommandName</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.IRepeatInfoUser.GetItemStyle</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.IRepeatInfoUser.HasFooter</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.IRepeatInfoUser.HasHeader</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.IRepeatInfoUser.HasSeparators</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.IRepeatInfoUser.RenderItem</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.IRepeatInfoUser.RepeatedItemCount</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>DataList.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>DataList.(case list)</code> | ✅ 一致 |
| <code>DataList.(default)</code> | ✅ 一致 |
| <code>DataList.AccessKey=empty</code> | ✅ 一致 |
| <code>DataList.AccessKey=japanese</code> | ✅ 一致 |
| <code>DataList.AccessKey=k</code> | ✅ 一致 |
| <code>DataList.AccessKey=markup</code> | ✅ 一致 |
| <code>DataList.AccessKey=text</code> | ✅ 一致 |
| <code>DataList.AlternatingItemStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DataList.AlternatingItemStyle.CssClass=text</code> | ✅ 一致 |
| <code>DataList.AlternatingItemStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>DataList.AlternatingItemStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>DataList.AlternatingItemStyle.Height=120px</code> | ✅ 一致 |
| <code>DataList.AlternatingItemStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DataList.AlternatingItemStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>DataList.AlternatingItemStyle.Wrap=true</code> | ✅ 一致 |
| <code>DataList.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>DataList.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>DataList.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>DataList.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>DataList.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>DataList.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>DataList.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>DataList.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>DataList.BackColor=#3366CC</code> | ✅ 一致 |
| <code>DataList.BackColor=Red</code> | ✅ 一致 |
| <code>DataList.BackColor=Transparent</code> | ✅ 一致 |
| <code>DataList.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>DataList.BorderColor=Red</code> | ✅ 一致 |
| <code>DataList.BorderColor=Transparent</code> | ✅ 一致 |
| <code>DataList.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>DataList.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>DataList.BorderStyle=Double</code> | ✅ 一致 |
| <code>DataList.BorderStyle=Groove</code> | ✅ 一致 |
| <code>DataList.BorderStyle=Inset</code> | ✅ 一致 |
| <code>DataList.BorderStyle=None</code> | ✅ 一致 |
| <code>DataList.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>DataList.BorderStyle=Outset</code> | ✅ 一致 |
| <code>DataList.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>DataList.BorderStyle=Solid</code> | ✅ 一致 |
| <code>DataList.BorderWidth=120px</code> | ✅ 一致 |
| <code>DataList.BorderWidth=2em</code> | ✅ 一致 |
| <code>DataList.BorderWidth=50%</code> | ✅ 一致 |
| <code>DataList.Caption=empty</code> | ✅ 一致 |
| <code>DataList.Caption=japanese</code> | ✅ 一致 |
| <code>DataList.Caption=markup</code> | ✅ 一致 |
| <code>DataList.Caption=text</code> | ✅ 一致 |
| <code>DataList.Caption=text+CaptionAlign=Bottom</code> | ✅ 一致 |
| <code>DataList.CaptionAlign=Bottom</code> | ✅ 一致 |
| <code>DataList.CaptionAlign=Left</code> | ✅ 一致 |
| <code>DataList.CaptionAlign=NotSet</code> | ✅ 一致 |
| <code>DataList.CaptionAlign=Right</code> | ✅ 一致 |
| <code>DataList.CaptionAlign=Top</code> | ✅ 一致 |
| <code>DataList.CellPadding=-1</code> | ✅ 一致 |
| <code>DataList.CellPadding=0</code> | ✅ 一致 |
| <code>DataList.CellPadding=3</code> | ✅ 一致 |
| <code>DataList.CellSpacing=-1</code> | ✅ 一致 |
| <code>DataList.CellSpacing=0</code> | ✅ 一致 |
| <code>DataList.CellSpacing=3</code> | ✅ 一致 |
| <code>DataList.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>DataList.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>DataList.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>DataList.ClientIDMode=Static</code> | ✅ 一致 |
| <code>DataList.CssClass=empty</code> | ✅ 一致 |
| <code>DataList.CssClass=japanese</code> | ✅ 一致 |
| <code>DataList.CssClass=markup</code> | ✅ 一致 |
| <code>DataList.CssClass=text</code> | ✅ 一致 |
| <code>DataList.DataKeyField=Code</code> | ✅ 一致 |
| <code>DataList.DataKeyField=empty</code> | ✅ 一致 |
| <code>DataList.DataKeyField=japanese</code> | ✅ 一致 |
| <code>DataList.DataKeyField=markup</code> | ✅ 一致 |
| <code>DataList.DataKeyField=text</code> | ✅ 一致 |
| <code>DataList.DataMember=empty</code> | ✅ 一致 |
| <code>DataList.DataMember=japanese</code> | ✅ 一致 |
| <code>DataList.DataMember=markup</code> | ✅ 一致 |
| <code>DataList.DataMember=text</code> | ✅ 一致 |
| <code>DataList.DataSourceID=empty</code> | ✅ 一致 |
| <code>DataList.DataSourceID=japanese</code> | ✅ 一致 |
| <code>DataList.DataSourceID=markup</code> | ✅ 一致 |
| <code>DataList.DataSourceID=text</code> | ✅ 一致 |
| <code>DataList.EditItemIndex=-1</code> | ✅ 一致 |
| <code>DataList.EditItemIndex=0</code> | ✅ 一致 |
| <code>DataList.EditItemIndex=1</code> | ✅ 一致 |
| <code>DataList.EditItemIndex=3</code> | ✅ 一致 |
| <code>DataList.EditItemStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DataList.EditItemStyle.CssClass=text</code> | ✅ 一致 |
| <code>DataList.EditItemStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>DataList.EditItemStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>DataList.EditItemStyle.Height=120px</code> | ✅ 一致 |
| <code>DataList.EditItemStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DataList.EditItemStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>DataList.EditItemStyle.Wrap=true</code> | ✅ 一致 |
| <code>DataList.EnableTheming=false</code> | ✅ 一致 |
| <code>DataList.EnableTheming=true</code> | ✅ 一致 |
| <code>DataList.EnableViewState=false</code> | ✅ 一致 |
| <code>DataList.EnableViewState=true</code> | ✅ 一致 |
| <code>DataList.Enabled=false</code> | ✅ 一致 |
| <code>DataList.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>DataList.Enabled=true</code> | ✅ 一致 |
| <code>DataList.ExtractTemplateRows=false</code> | ✅ 一致 |
| <code>DataList.ExtractTemplateRows=true</code> | ✅ 一致 |
| <code>DataList.Font.Bold=false</code> | ✅ 一致 |
| <code>DataList.Font.Bold=true</code> | ✅ 一致 |
| <code>DataList.Font.Italic=false</code> | ✅ 一致 |
| <code>DataList.Font.Italic=true</code> | ✅ 一致 |
| <code>DataList.Font.Name=empty</code> | ✅ 一致 |
| <code>DataList.Font.Name=japanese</code> | ✅ 一致 |
| <code>DataList.Font.Name=markup</code> | ✅ 一致 |
| <code>DataList.Font.Name=text</code> | ✅ 一致 |
| <code>DataList.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>DataList.Font.Overline=false</code> | ✅ 一致 |
| <code>DataList.Font.Overline=true</code> | ✅ 一致 |
| <code>DataList.Font.Size=12pt</code> | ✅ 一致 |
| <code>DataList.Font.Size=150%</code> | ✅ 一致 |
| <code>DataList.Font.Size=Large</code> | ✅ 一致 |
| <code>DataList.Font.Strikeout=false</code> | ✅ 一致 |
| <code>DataList.Font.Strikeout=true</code> | ✅ 一致 |
| <code>DataList.Font.Underline=false</code> | ✅ 一致 |
| <code>DataList.Font.Underline=true</code> | ✅ 一致 |
| <code>DataList.FooterStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DataList.FooterStyle.CssClass=text</code> | ✅ 一致 |
| <code>DataList.FooterStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>DataList.FooterStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>DataList.FooterStyle.Height=120px</code> | ✅ 一致 |
| <code>DataList.FooterStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DataList.FooterStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>DataList.FooterStyle.Wrap=true</code> | ✅ 一致 |
| <code>DataList.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>DataList.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>DataList.ForeColor=Red</code> | ✅ 一致 |
| <code>DataList.ForeColor=Transparent</code> | ✅ 一致 |
| <code>DataList.GridLines=Both</code> | ✅ 一致 |
| <code>DataList.GridLines=Both+CellPadding=3</code> | ✅ 一致 |
| <code>DataList.GridLines=Horizontal</code> | ✅ 一致 |
| <code>DataList.GridLines=None</code> | ✅ 一致 |
| <code>DataList.GridLines=Vertical</code> | ✅ 一致 |
| <code>DataList.HeaderStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DataList.HeaderStyle.CssClass=text</code> | ✅ 一致 |
| <code>DataList.HeaderStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>DataList.HeaderStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>DataList.HeaderStyle.Height=120px</code> | ✅ 一致 |
| <code>DataList.HeaderStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DataList.HeaderStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>DataList.HeaderStyle.Wrap=true</code> | ✅ 一致 |
| <code>DataList.Height=120px</code> | ✅ 一致 |
| <code>DataList.Height=2em</code> | ✅ 一致 |
| <code>DataList.Height=50%</code> | ✅ 一致 |
| <code>DataList.HorizontalAlign=Center</code> | ✅ 一致 |
| <code>DataList.HorizontalAlign=Justify</code> | ✅ 一致 |
| <code>DataList.HorizontalAlign=Left</code> | ✅ 一致 |
| <code>DataList.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DataList.HorizontalAlign=Right</code> | ✅ 一致 |
| <code>DataList.ID=empty</code> | ✅ 一致 |
| <code>DataList.ID=japanese</code> | ✅ 一致 |
| <code>DataList.ID=markup</code> | ✅ 一致 |
| <code>DataList.ID=text</code> | ✅ 一致 |
| <code>DataList.ItemStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DataList.ItemStyle.CssClass=text</code> | ✅ 一致 |
| <code>DataList.ItemStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>DataList.ItemStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>DataList.ItemStyle.Height=120px</code> | ✅ 一致 |
| <code>DataList.ItemStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DataList.ItemStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>DataList.ItemStyle.Wrap=true</code> | ✅ 一致 |
| <code>DataList.RepeatColumns=-1</code> | ✅ 一致 |
| <code>DataList.RepeatColumns=0</code> | ✅ 一致 |
| <code>DataList.RepeatColumns=3</code> | ✅ 一致 |
| <code>DataList.RepeatColumns=3+EnableViewState=false</code> | ✅ 一致 |
| <code>DataList.RepeatColumns=3+RepeatDirection=Horizontal</code> | ✅ 一致 |
| <code>DataList.RepeatColumns=3+RepeatDirection=Vertical</code> | ✅ 一致 |
| <code>DataList.RepeatColumns=3+Visible=false</code> | ✅ 一致 |
| <code>DataList.RepeatDirection=Horizontal</code> | ✅ 一致 |
| <code>DataList.RepeatDirection=Vertical</code> | ✅ 一致 |
| <code>DataList.RepeatLayout=Flow</code> | ✅ 一致 |
| <code>DataList.RepeatLayout=Flow+GridLines=Both</code> | ✅ 一致 |
| <code>DataList.RepeatLayout=Flow+RepeatColumns=3</code> | ✅ 一致 |
| <code>DataList.RepeatLayout=OrderedList</code> | ✅ 一致 |
| <code>DataList.RepeatLayout=Table</code> | ✅ 一致 |
| <code>DataList.RepeatLayout=UnorderedList</code> | ✅ 一致 |
| <code>DataList.RepeatLayout=UnorderedList+RepeatColumns=3</code> | ✅ 一致 |
| <code>DataList.SelectedIndex=-1</code> | ✅ 一致 |
| <code>DataList.SelectedIndex=0</code> | ✅ 一致 |
| <code>DataList.SelectedIndex=1</code> | ✅ 一致 |
| <code>DataList.SelectedIndex=1+SelectedItemStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DataList.SelectedIndex=3</code> | ✅ 一致 |
| <code>DataList.SelectedItemStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DataList.SelectedItemStyle.CssClass=text</code> | ✅ 一致 |
| <code>DataList.SelectedItemStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>DataList.SelectedItemStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>DataList.SelectedItemStyle.Height=120px</code> | ✅ 一致 |
| <code>DataList.SelectedItemStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DataList.SelectedItemStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>DataList.SelectedItemStyle.Wrap=true</code> | ✅ 一致 |
| <code>DataList.SeparatorStyle.BackColor=Red</code> | ✅ 一致 |
| <code>DataList.SeparatorStyle.CssClass=text</code> | ✅ 一致 |
| <code>DataList.SeparatorStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>DataList.SeparatorStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>DataList.SeparatorStyle.Height=120px</code> | ✅ 一致 |
| <code>DataList.SeparatorStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DataList.SeparatorStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>DataList.SeparatorStyle.Wrap=true</code> | ✅ 一致 |
| <code>DataList.ShowFooter=false</code> | ✅ 一致 |
| <code>DataList.ShowFooter=true</code> | ✅ 一致 |
| <code>DataList.ShowHeader=false</code> | ✅ 一致 |
| <code>DataList.ShowHeader=true</code> | ✅ 一致 |
| <code>DataList.SkinID=empty</code> | ✅ 一致 |
| <code>DataList.SkinID=japanese</code> | ✅ 一致 |
| <code>DataList.SkinID=markup</code> | ✅ 一致 |
| <code>DataList.SkinID=text</code> | ✅ 一致 |
| <code>DataList.Style[color]=1em</code> | ✅ 一致 |
| <code>DataList.Style[color]=red</code> | ✅ 一致 |
| <code>DataList.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>DataList.Style[margin-left]=red</code> | ✅ 一致 |
| <code>DataList.TabIndex=-1</code> | ✅ 一致 |
| <code>DataList.TabIndex=0</code> | ✅ 一致 |
| <code>DataList.TabIndex=3</code> | ✅ 一致 |
| <code>DataList.ToolTip=empty</code> | ✅ 一致 |
| <code>DataList.ToolTip=japanese</code> | ✅ 一致 |
| <code>DataList.ToolTip=markup</code> | ✅ 一致 |
| <code>DataList.ToolTip=text</code> | ✅ 一致 |
| <code>DataList.UseAccessibleHeader=false</code> | ✅ 一致 |
| <code>DataList.UseAccessibleHeader=true</code> | ✅ 一致 |
| <code>DataList.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>DataList.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>DataList.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>DataList.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>DataList.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>DataList.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>DataList.Visible=false</code> | ✅ 一致 |
| <code>DataList.Visible=true</code> | ✅ 一致 |
| <code>DataList.Width=120px</code> | ✅ 一致 |
| <code>DataList.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>DataList.Width=2em</code> | ✅ 一致 |
| <code>DataList.Width=50%</code> | ✅ 一致 |
| <code>DataList[empty].(default)</code> | ✅ 一致 |
| <code>DataList[empty].ShowHeader=false</code> | ✅ 一致 |
| <code>DataList[naming container].(default)</code> | ✅ 一致 |
| <code>DataList[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>DataList[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>DataList[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>DataList[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>DataList[naming container].ID=text</code> | ✅ 一致 |
| <code>DataList[naming container].RepeatColumns=3</code> | ✅ 一致 |
| <code>DataList[templates].(default)</code> | ✅ 一致 |
| <code>DataList[templates].DataKeyField=Code+SelectedIndex=1</code> | ✅ 一致 |
| <code>DataList[templates].EditItemIndex=1</code> | ✅ 一致 |
| <code>DataList[templates].ExtractTemplateRows=true</code> | ✅ 一致 |
| <code>DataList[templates].RepeatColumns=3</code> | ✅ 一致 |
| <code>DataList[templates].RepeatLayout=Flow</code> | ✅ 一致 |
| <code>DataList[templates].RepeatLayout=UnorderedList</code> | ✅ 一致 |
| <code>DataList[templates].SelectedIndex=1</code> | ✅ 一致 |
| <code>DataList[templates].ShowFooter=false</code> | ✅ 一致 |
| <code>DataList[templates].ShowHeader=false</code> | ✅ 一致 |
| <code>DataList[templates].UseAccessibleHeader=true</code> | ✅ 一致 |
| <code>DataList[themed].(default)</code> | ✅ 一致 |
| <code>DataList[themed].CssClass=text</code> | ✅ 一致 |
| <code>DataList[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>DataList[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| DataList [render][databind][datasource][datasourceid][templates][repeat][dynamic] first request | ✅ 一致 |
| DataList [selection][rowcommand] the first item selected | ✅ 一致 |
| DataList [rowcommand] a command bubbled to ItemCommand | ✅ 一致 |
| DataList [editing][rowcommand] edit: EditCommand | ✅ 一致 |
| DataList [editing][rowcommand] updated: UpdateCommand, the data key | ✅ 一致 |
| DataList [editing] cancelled: CancelCommand | ✅ 一致 |
| DataList [editing][rowcommand] deleted: DeleteCommand | ✅ 一致 |
| DataList [viewstate] posted (the one without view state is empty) | ✅ 一致 |
| DataList [visible][viewstate] the hidden data list shown | ✅ 一致 |

</details>
