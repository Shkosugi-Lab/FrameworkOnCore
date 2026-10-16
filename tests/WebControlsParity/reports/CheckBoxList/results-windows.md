# CheckBoxList パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-11 23:19
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 250 | 250 | 0 | 0 |
| ② HTML | 234 | 234 | 0 | 0 |
| ③ シナリオ | 11 | 11 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 259 件中 259 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 50 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 11 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 193 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 2 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 7 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 1 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 8 | 0 | ✅ |
| ポストバックデータの取り込み | 0 | 1 | ✅ |
| 変更イベント（*Changed、ServerChange） | 0 | 3 | ✅ |
| リクエスト検証（ValidateRequestMode） | 3 | 1 | ✅ |
| AutoPostBack（クライアントスクリプトによるポストバック） | 6 | 1 | ✅ |
| CausesValidation / ValidationGroup | 8 | 2 | ✅ |
| 双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値） | 0 | 1 | ✅ |
| 項目（ListItem の Selected・Enabled・Text・Value と ViewState） | 20 | 2 | ✅ |
| 複数選択 | 1 | 1 | ✅ |
| 繰り返しレイアウト（RepeatLayout・RepeatColumns・RepeatDirection） | 16 | 0 | ✅ |
| ポスト値のイベント検証（一覧にない値・無効な項目の拒否） | 0 | 1 | ✅ |
| データソース連携（DataSource・DataTextField など） | 10 | 0 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>CheckBoxList.Api.Control.#ctor</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.Adapter</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.ClientID</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.Context</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.Controls</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.DataBind</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.Dispose</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.Disposed</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.Events</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.FindControl</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.Focus</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.HasControls</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.ID</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.Init</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.Load</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.OnInit</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.Page</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.Parent</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.PreRender</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.Render</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.Site</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.SkinID</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.Unload</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.ViewState</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>CheckBoxList.Api.Control.Visible</code> | ✅ 一致 |
| <code>CheckBoxList.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>CheckBoxList.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>CheckBoxList.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>CheckBoxList.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>CheckBoxList.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>CheckBoxList.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>CheckBoxList.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>CheckBoxList.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>CheckBoxList.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>CheckBoxList.Api.IEditableTextControl.TextChanged</code> | ✅ 一致 |
| <code>CheckBoxList.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>CheckBoxList.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>CheckBoxList.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>CheckBoxList.Api.IPostBackDataHandler.LoadPostData</code> | ✅ 一致 |
| <code>CheckBoxList.Api.IPostBackDataHandler.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>CheckBoxList.Api.ITextControl.Text</code> | ✅ 一致 |
| <code>CheckBoxList.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.BaseDataBoundControl.#ctor</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.BaseDataBoundControl.ConfirmInitState</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.BaseDataBoundControl.DataBind</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.BaseDataBoundControl.DataBound</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.BaseDataBoundControl.DataSource</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.BaseDataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.BaseDataBoundControl.EnsureDataBound</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.BaseDataBoundControl.Initialized</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.BaseDataBoundControl.IsBoundUsingDataSourceID</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.BaseDataBoundControl.IsDataBindingAutomatic</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.BaseDataBoundControl.IsUsingModelBinders</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.BaseDataBoundControl.OnDataBound</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.BaseDataBoundControl.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.BaseDataBoundControl.OnInit</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.BaseDataBoundControl.OnPagePreLoad</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.BaseDataBoundControl.OnPreRender</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.BaseDataBoundControl.PerformSelect</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.BaseDataBoundControl.RequiresDataBinding</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.BaseDataBoundControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.BaseDataBoundControl.ValidateDataSource</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.CheckBoxList.#ctor</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.CheckBoxList.CellPadding</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.CheckBoxList.CellSpacing</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.CheckBoxList.CreateControlStyle</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.CheckBoxList.FindControl</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.CheckBoxList.GetItemStyle</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.CheckBoxList.HasFooter</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.CheckBoxList.HasHeader</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.CheckBoxList.HasSeparators</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.CheckBoxList.LoadPostData</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.CheckBoxList.OnInit</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.CheckBoxList.OnPreRender</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.CheckBoxList.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.CheckBoxList.Render</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.CheckBoxList.RenderItem</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.CheckBoxList.RenderWhenDataEmpty</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.CheckBoxList.RepeatColumns</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.CheckBoxList.RepeatDirection</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.CheckBoxList.RepeatLayout</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.CheckBoxList.RepeatedItemCount</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.CheckBoxList.TextAlign</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.DataBoundControl.#ctor</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.DataBoundControl.CallingDataMethods</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.DataBoundControl.CreateDataSourceSelectArguments</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.DataBoundControl.CreatingModelDataSource</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.DataBoundControl.DataMember</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.DataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.DataBoundControl.DataSourceObject</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.DataBoundControl.GetData</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.DataBoundControl.GetDataSource</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.DataBoundControl.IsUsingModelBinders</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.DataBoundControl.ItemType</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.DataBoundControl.LoadViewState</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.DataBoundControl.MarkAsDataBound</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.DataBoundControl.OnCreatingModelDataSource</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.DataBoundControl.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.DataBoundControl.OnDataSourceViewChanged</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.DataBoundControl.OnLoad</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.DataBoundControl.OnPagePreLoad</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.DataBoundControl.PerformDataBinding</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.DataBoundControl.PerformSelect</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.DataBoundControl.SaveViewState</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.DataBoundControl.SelectArguments</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.DataBoundControl.SelectMethod</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.DataBoundControl.TrackViewState</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.DataBoundControl.ValidateDataSource</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.IRepeatInfoUser.GetItemStyle</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.IRepeatInfoUser.HasFooter</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.IRepeatInfoUser.HasHeader</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.IRepeatInfoUser.HasSeparators</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.IRepeatInfoUser.RenderItem</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.IRepeatInfoUser.RepeatedItemCount</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.#ctor</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.AppendDataBoundItems</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.AutoPostBack</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.CausesValidation</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.ClearSelection</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.DataTextField</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.DataTextFormatString</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.DataValueField</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.Items</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.LoadViewState</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.OnDataBinding</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.OnPreRender</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.OnSelectedIndexChanged</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.OnTextChanged</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.PerformDataBinding</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.PerformSelect</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.RenderContents</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.SaveViewState</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.SelectedIndex</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.SelectedIndexChanged</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.SelectedItem</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.SelectedValue</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.SetPostDataSelection</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.TagKey</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.Text</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.TextChanged</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.TrackViewState</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.ValidationGroup</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.ListControl.VerifyMultiSelect</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>CheckBoxList.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>CheckBoxList.(case list)</code> | ✅ 一致 |
| <code>CheckBoxList.(default)</code> | ✅ 一致 |
| <code>CheckBoxList.AccessKey=empty</code> | ✅ 一致 |
| <code>CheckBoxList.AccessKey=japanese</code> | ✅ 一致 |
| <code>CheckBoxList.AccessKey=k</code> | ✅ 一致 |
| <code>CheckBoxList.AccessKey=markup</code> | ✅ 一致 |
| <code>CheckBoxList.AccessKey=text</code> | ✅ 一致 |
| <code>CheckBoxList.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>CheckBoxList.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>CheckBoxList.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>CheckBoxList.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>CheckBoxList.AppendDataBoundItems=false</code> | ✅ 一致 |
| <code>CheckBoxList.AppendDataBoundItems=true</code> | ✅ 一致 |
| <code>CheckBoxList.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>CheckBoxList.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>CheckBoxList.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>CheckBoxList.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>CheckBoxList.AutoPostBack=false</code> | ✅ 一致 |
| <code>CheckBoxList.AutoPostBack=true</code> | ✅ 一致 |
| <code>CheckBoxList.AutoPostBack=true+CausesValidation=true</code> | ✅ 一致 |
| <code>CheckBoxList.AutoPostBack=true+CausesValidation=true+ValidationGroup=text</code> | ✅ 一致 |
| <code>CheckBoxList.AutoPostBack=true+Enabled=false</code> | ✅ 一致 |
| <code>CheckBoxList.AutoPostBack=true+Items[1].Enabled=false</code> | ✅ 一致 |
| <code>CheckBoxList.BackColor=#3366CC</code> | ✅ 一致 |
| <code>CheckBoxList.BackColor=Red</code> | ✅ 一致 |
| <code>CheckBoxList.BackColor=Transparent</code> | ✅ 一致 |
| <code>CheckBoxList.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>CheckBoxList.BorderColor=Red</code> | ✅ 一致 |
| <code>CheckBoxList.BorderColor=Transparent</code> | ✅ 一致 |
| <code>CheckBoxList.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>CheckBoxList.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>CheckBoxList.BorderStyle=Double</code> | ✅ 一致 |
| <code>CheckBoxList.BorderStyle=Groove</code> | ✅ 一致 |
| <code>CheckBoxList.BorderStyle=Inset</code> | ✅ 一致 |
| <code>CheckBoxList.BorderStyle=None</code> | ✅ 一致 |
| <code>CheckBoxList.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>CheckBoxList.BorderStyle=Outset</code> | ✅ 一致 |
| <code>CheckBoxList.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>CheckBoxList.BorderStyle=Solid</code> | ✅ 一致 |
| <code>CheckBoxList.BorderWidth=120px</code> | ✅ 一致 |
| <code>CheckBoxList.BorderWidth=2em</code> | ✅ 一致 |
| <code>CheckBoxList.BorderWidth=50%</code> | ✅ 一致 |
| <code>CheckBoxList.CausesValidation=false</code> | ✅ 一致 |
| <code>CheckBoxList.CausesValidation=true</code> | ✅ 一致 |
| <code>CheckBoxList.CellPadding=-1</code> | ✅ 一致 |
| <code>CheckBoxList.CellPadding=0</code> | ✅ 一致 |
| <code>CheckBoxList.CellPadding=3</code> | ✅ 一致 |
| <code>CheckBoxList.CellSpacing=-1</code> | ✅ 一致 |
| <code>CheckBoxList.CellSpacing=0</code> | ✅ 一致 |
| <code>CheckBoxList.CellSpacing=3</code> | ✅ 一致 |
| <code>CheckBoxList.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>CheckBoxList.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>CheckBoxList.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>CheckBoxList.ClientIDMode=Static</code> | ✅ 一致 |
| <code>CheckBoxList.CssClass=empty</code> | ✅ 一致 |
| <code>CheckBoxList.CssClass=japanese</code> | ✅ 一致 |
| <code>CheckBoxList.CssClass=markup</code> | ✅ 一致 |
| <code>CheckBoxList.CssClass=text</code> | ✅ 一致 |
| <code>CheckBoxList.DataMember=empty</code> | ✅ 一致 |
| <code>CheckBoxList.DataMember=japanese</code> | ✅ 一致 |
| <code>CheckBoxList.DataMember=markup</code> | ✅ 一致 |
| <code>CheckBoxList.DataMember=text</code> | ✅ 一致 |
| <code>CheckBoxList.DataSourceID=empty</code> | ✅ 一致 |
| <code>CheckBoxList.DataSourceID=japanese</code> | ✅ 一致 |
| <code>CheckBoxList.DataSourceID=markup</code> | ✅ 一致 |
| <code>CheckBoxList.DataSourceID=text</code> | ✅ 一致 |
| <code>CheckBoxList.DataTextField=Name</code> | ✅ 一致 |
| <code>CheckBoxList.DataTextField=Price</code> | ✅ 一致 |
| <code>CheckBoxList.DataTextField=empty</code> | ✅ 一致 |
| <code>CheckBoxList.DataTextField=japanese</code> | ✅ 一致 |
| <code>CheckBoxList.DataTextField=markup</code> | ✅ 一致 |
| <code>CheckBoxList.DataTextField=text</code> | ✅ 一致 |
| <code>CheckBoxList.DataTextFormatString=N2</code> | ✅ 一致 |
| <code>CheckBoxList.DataTextFormatString=empty</code> | ✅ 一致 |
| <code>CheckBoxList.DataTextFormatString=japanese</code> | ✅ 一致 |
| <code>CheckBoxList.DataTextFormatString=markup</code> | ✅ 一致 |
| <code>CheckBoxList.DataTextFormatString=text</code> | ✅ 一致 |
| <code>CheckBoxList.DataValueField=Code</code> | ✅ 一致 |
| <code>CheckBoxList.DataValueField=empty</code> | ✅ 一致 |
| <code>CheckBoxList.DataValueField=japanese</code> | ✅ 一致 |
| <code>CheckBoxList.DataValueField=markup</code> | ✅ 一致 |
| <code>CheckBoxList.DataValueField=text</code> | ✅ 一致 |
| <code>CheckBoxList.EnableTheming=false</code> | ✅ 一致 |
| <code>CheckBoxList.EnableTheming=true</code> | ✅ 一致 |
| <code>CheckBoxList.EnableViewState=false</code> | ✅ 一致 |
| <code>CheckBoxList.EnableViewState=true</code> | ✅ 一致 |
| <code>CheckBoxList.Enabled=false</code> | ✅ 一致 |
| <code>CheckBoxList.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>CheckBoxList.Enabled=true</code> | ✅ 一致 |
| <code>CheckBoxList.Font.Bold=false</code> | ✅ 一致 |
| <code>CheckBoxList.Font.Bold=true</code> | ✅ 一致 |
| <code>CheckBoxList.Font.Italic=false</code> | ✅ 一致 |
| <code>CheckBoxList.Font.Italic=true</code> | ✅ 一致 |
| <code>CheckBoxList.Font.Name=empty</code> | ✅ 一致 |
| <code>CheckBoxList.Font.Name=japanese</code> | ✅ 一致 |
| <code>CheckBoxList.Font.Name=markup</code> | ✅ 一致 |
| <code>CheckBoxList.Font.Name=text</code> | ✅ 一致 |
| <code>CheckBoxList.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>CheckBoxList.Font.Overline=false</code> | ✅ 一致 |
| <code>CheckBoxList.Font.Overline=true</code> | ✅ 一致 |
| <code>CheckBoxList.Font.Size=12pt</code> | ✅ 一致 |
| <code>CheckBoxList.Font.Size=150%</code> | ✅ 一致 |
| <code>CheckBoxList.Font.Size=Large</code> | ✅ 一致 |
| <code>CheckBoxList.Font.Strikeout=false</code> | ✅ 一致 |
| <code>CheckBoxList.Font.Strikeout=true</code> | ✅ 一致 |
| <code>CheckBoxList.Font.Underline=false</code> | ✅ 一致 |
| <code>CheckBoxList.Font.Underline=true</code> | ✅ 一致 |
| <code>CheckBoxList.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>CheckBoxList.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>CheckBoxList.ForeColor=Red</code> | ✅ 一致 |
| <code>CheckBoxList.ForeColor=Transparent</code> | ✅ 一致 |
| <code>CheckBoxList.Height=120px</code> | ✅ 一致 |
| <code>CheckBoxList.Height=2em</code> | ✅ 一致 |
| <code>CheckBoxList.Height=50%</code> | ✅ 一致 |
| <code>CheckBoxList.ID=empty</code> | ✅ 一致 |
| <code>CheckBoxList.ID=japanese</code> | ✅ 一致 |
| <code>CheckBoxList.ID=markup</code> | ✅ 一致 |
| <code>CheckBoxList.ID=text</code> | ✅ 一致 |
| <code>CheckBoxList.ItemType=empty</code> | ✅ 一致 |
| <code>CheckBoxList.ItemType=japanese</code> | ✅ 一致 |
| <code>CheckBoxList.ItemType=markup</code> | ✅ 一致 |
| <code>CheckBoxList.ItemType=text</code> | ✅ 一致 |
| <code>CheckBoxList.Items[0].Selected=true+Items[2].Selected=true</code> | ✅ 一致 |
| <code>CheckBoxList.Items[1].Enabled=false</code> | ✅ 一致 |
| <code>CheckBoxList.Items[1].Enabled=false+Enabled=false</code> | ✅ 一致 |
| <code>CheckBoxList.Items[1].Enabled=true</code> | ✅ 一致 |
| <code>CheckBoxList.Items[1].Selected=false</code> | ✅ 一致 |
| <code>CheckBoxList.Items[1].Selected=true</code> | ✅ 一致 |
| <code>CheckBoxList.Items[1].Selected=true+Enabled=false</code> | ✅ 一致 |
| <code>CheckBoxList.Items[1].Text=empty</code> | ✅ 一致 |
| <code>CheckBoxList.Items[1].Text=empty+Items[1].Value=text</code> | ✅ 一致 |
| <code>CheckBoxList.Items[1].Text=japanese</code> | ✅ 一致 |
| <code>CheckBoxList.Items[1].Text=markup</code> | ✅ 一致 |
| <code>CheckBoxList.Items[1].Text=markup+Items[1].Value=markup</code> | ✅ 一致 |
| <code>CheckBoxList.Items[1].Text=spaces</code> | ✅ 一致 |
| <code>CheckBoxList.Items[1].Text=text</code> | ✅ 一致 |
| <code>CheckBoxList.Items[1].Value=empty</code> | ✅ 一致 |
| <code>CheckBoxList.Items[1].Value=japanese</code> | ✅ 一致 |
| <code>CheckBoxList.Items[1].Value=markup</code> | ✅ 一致 |
| <code>CheckBoxList.Items[1].Value=text</code> | ✅ 一致 |
| <code>CheckBoxList.RenderWhenDataEmpty=false</code> | ✅ 一致 |
| <code>CheckBoxList.RenderWhenDataEmpty=true</code> | ✅ 一致 |
| <code>CheckBoxList.RepeatColumns=-1</code> | ✅ 一致 |
| <code>CheckBoxList.RepeatColumns=0</code> | ✅ 一致 |
| <code>CheckBoxList.RepeatColumns=3</code> | ✅ 一致 |
| <code>CheckBoxList.RepeatColumns=3+RepeatDirection=Horizontal</code> | ✅ 一致 |
| <code>CheckBoxList.RepeatColumns=3+RepeatDirection=Vertical</code> | ✅ 一致 |
| <code>CheckBoxList.RepeatDirection=Horizontal</code> | ✅ 一致 |
| <code>CheckBoxList.RepeatDirection=Vertical</code> | ✅ 一致 |
| <code>CheckBoxList.RepeatLayout=Flow</code> | ✅ 一致 |
| <code>CheckBoxList.RepeatLayout=Flow+RepeatColumns=3</code> | ✅ 一致 |
| <code>CheckBoxList.RepeatLayout=Flow+TextAlign=Left</code> | ✅ 一致 |
| <code>CheckBoxList.RepeatLayout=OrderedList</code> | ✅ 一致 |
| <code>CheckBoxList.RepeatLayout=OrderedList+RepeatDirection=Horizontal</code> | ✅ 一致 |
| <code>CheckBoxList.RepeatLayout=Table</code> | ✅ 一致 |
| <code>CheckBoxList.RepeatLayout=Table+CellPadding=3+CellSpacing=3</code> | ✅ 一致 |
| <code>CheckBoxList.RepeatLayout=UnorderedList</code> | ✅ 一致 |
| <code>CheckBoxList.RepeatLayout=UnorderedList+RepeatColumns=3</code> | ✅ 一致 |
| <code>CheckBoxList.SelectMethod=empty</code> | ✅ 一致 |
| <code>CheckBoxList.SelectMethod=japanese</code> | ✅ 一致 |
| <code>CheckBoxList.SelectMethod=markup</code> | ✅ 一致 |
| <code>CheckBoxList.SelectMethod=text</code> | ✅ 一致 |
| <code>CheckBoxList.SelectedIndex=-1</code> | ✅ 一致 |
| <code>CheckBoxList.SelectedIndex=0</code> | ✅ 一致 |
| <code>CheckBoxList.SelectedIndex=1</code> | ✅ 一致 |
| <code>CheckBoxList.SelectedIndex=1+EnableViewState=false</code> | ✅ 一致 |
| <code>CheckBoxList.SelectedIndex=1+Items[1].Enabled=false</code> | ✅ 一致 |
| <code>CheckBoxList.SelectedIndex=1+Visible=false</code> | ✅ 一致 |
| <code>CheckBoxList.SelectedIndex=3</code> | ✅ 一致 |
| <code>CheckBoxList.SelectedValue=B2</code> | ✅ 一致 |
| <code>CheckBoxList.SelectedValue=M</code> | ✅ 一致 |
| <code>CheckBoxList.SelectedValue=M+SelectedIndex=0</code> | ✅ 一致 |
| <code>CheckBoxList.SelectedValue=empty</code> | ✅ 一致 |
| <code>CheckBoxList.SelectedValue=japanese</code> | ✅ 一致 |
| <code>CheckBoxList.SelectedValue=markup</code> | ✅ 一致 |
| <code>CheckBoxList.SelectedValue=text</code> | ✅ 一致 |
| <code>CheckBoxList.SkinID=empty</code> | ✅ 一致 |
| <code>CheckBoxList.SkinID=japanese</code> | ✅ 一致 |
| <code>CheckBoxList.SkinID=markup</code> | ✅ 一致 |
| <code>CheckBoxList.SkinID=text</code> | ✅ 一致 |
| <code>CheckBoxList.Style[color]=1em</code> | ✅ 一致 |
| <code>CheckBoxList.Style[color]=red</code> | ✅ 一致 |
| <code>CheckBoxList.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>CheckBoxList.Style[margin-left]=red</code> | ✅ 一致 |
| <code>CheckBoxList.TabIndex=-1</code> | ✅ 一致 |
| <code>CheckBoxList.TabIndex=0</code> | ✅ 一致 |
| <code>CheckBoxList.TabIndex=3</code> | ✅ 一致 |
| <code>CheckBoxList.Text=empty</code> | ✅ 一致 |
| <code>CheckBoxList.Text=japanese</code> | ✅ 一致 |
| <code>CheckBoxList.Text=markup</code> | ✅ 一致 |
| <code>CheckBoxList.Text=text</code> | ✅ 一致 |
| <code>CheckBoxList.TextAlign=Left</code> | ✅ 一致 |
| <code>CheckBoxList.TextAlign=Right</code> | ✅ 一致 |
| <code>CheckBoxList.ToolTip=empty</code> | ✅ 一致 |
| <code>CheckBoxList.ToolTip=japanese</code> | ✅ 一致 |
| <code>CheckBoxList.ToolTip=markup</code> | ✅ 一致 |
| <code>CheckBoxList.ToolTip=text</code> | ✅ 一致 |
| <code>CheckBoxList.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>CheckBoxList.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>CheckBoxList.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>CheckBoxList.ValidationGroup=empty</code> | ✅ 一致 |
| <code>CheckBoxList.ValidationGroup=japanese</code> | ✅ 一致 |
| <code>CheckBoxList.ValidationGroup=markup</code> | ✅ 一致 |
| <code>CheckBoxList.ValidationGroup=text</code> | ✅ 一致 |
| <code>CheckBoxList.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>CheckBoxList.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>CheckBoxList.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>CheckBoxList.Visible=false</code> | ✅ 一致 |
| <code>CheckBoxList.Visible=true</code> | ✅ 一致 |
| <code>CheckBoxList.Width=120px</code> | ✅ 一致 |
| <code>CheckBoxList.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>CheckBoxList.Width=2em</code> | ✅ 一致 |
| <code>CheckBoxList.Width=50%</code> | ✅ 一致 |
| <code>CheckBoxList[data-bound].(default)</code> | ✅ 一致 |
| <code>CheckBoxList[data-bound].DataTextField=Name</code> | ✅ 一致 |
| <code>CheckBoxList[data-bound].DataTextField=Name+AppendDataBoundItems=true</code> | ✅ 一致 |
| <code>CheckBoxList[data-bound].DataTextField=Name+DataValueField=Code</code> | ✅ 一致 |
| <code>CheckBoxList[data-bound].DataTextField=Name+DataValueField=Code+SelectedIndex=1</code> | ✅ 一致 |
| <code>CheckBoxList[data-bound].DataTextField=Name+DataValueField=Code+SelectedValue=B2</code> | ✅ 一致 |
| <code>CheckBoxList[data-bound].DataTextField=Name+DataValueField=Code+SelectedValue=M</code> | ✅ 一致 |
| <code>CheckBoxList[data-bound].DataTextField=Price+DataTextFormatString=N2</code> | ✅ 一致 |
| <code>CheckBoxList[data-bound].DataTextField=text</code> | ✅ 一致 |
| <code>CheckBoxList[data-bound].DataValueField=Code</code> | ✅ 一致 |
| <code>CheckBoxList[naming container].(default)</code> | ✅ 一致 |
| <code>CheckBoxList[naming container].AutoPostBack=true</code> | ✅ 一致 |
| <code>CheckBoxList[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>CheckBoxList[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>CheckBoxList[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>CheckBoxList[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>CheckBoxList[naming container].ID=text</code> | ✅ 一致 |
| <code>CheckBoxList[themed].(default)</code> | ✅ 一致 |
| <code>CheckBoxList[themed].CssClass=text</code> | ✅ 一致 |
| <code>CheckBoxList[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>CheckBoxList[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| CheckBoxList [render][databind][items] first request | ✅ 一致 |
| CheckBoxList [postdata][change][multiselect][dynamic] boxes checked | ✅ 一致 |
| CheckBoxList [change] a box unchecked | ✅ 一致 |
| CheckBoxList [viewstate] posted again unchanged | ✅ 一致 |
| CheckBoxList [autopostback][change] auto postback | ✅ 一致 |
| CheckBoxList [causesvalidation] group g1 (CustomValidator), none checked | ✅ 一致 |
| CheckBoxList [causesvalidation] group g1 (CustomValidator), checked | ✅ 一致 |
| CheckBoxList [visible][viewstate][items] hidden lists shown with the items added in code | ✅ 一致 |
| CheckBoxList [bind] two-way binding in a template | ✅ 一致 |
| CheckBoxList [eventvalidation] a disabled item posted | ✅ 一致 |
| CheckBoxList [requestvalidation] markup posted | ✅ 一致 |

</details>
