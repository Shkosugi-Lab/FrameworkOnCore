# RadioButtonList パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 02:59
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 249 | 249 | 0 | 0 |
| ② HTML | 234 | 234 | 0 | 0 |
| ③ シナリオ | 11 | 11 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 258 件中 258 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
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
| 変更イベント（*Changed、ServerChange） | 0 | 2 | ✅ |
| リクエスト検証（ValidateRequestMode） | 3 | 1 | ✅ |
| AutoPostBack（クライアントスクリプトによるポストバック） | 6 | 1 | ✅ |
| CausesValidation / ValidationGroup | 8 | 0 | ✅ |
| 検証対象（サーバー側の検証） | 0 | 2 | ✅ |
| 検証対象（ブラウザ側の検証） | 0 | 1 | ✅ |
| 双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値） | 0 | 1 | ✅ |
| 項目（ListItem の Selected・Enabled・Text・Value と ViewState） | 20 | 2 | ✅ |
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
| <code>RadioButtonList.Api.Control.#ctor</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.Adapter</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.ClientID</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.Context</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.Controls</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.DataBind</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.Dispose</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.Disposed</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.Events</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.FindControl</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.Focus</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.HasControls</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.ID</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.Init</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.Load</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.OnInit</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.Page</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.Parent</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.PreRender</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.Render</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.Site</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.SkinID</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.Unload</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.ViewState</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>RadioButtonList.Api.Control.Visible</code> | ✅ 一致 |
| <code>RadioButtonList.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>RadioButtonList.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>RadioButtonList.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>RadioButtonList.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>RadioButtonList.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>RadioButtonList.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>RadioButtonList.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>RadioButtonList.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>RadioButtonList.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>RadioButtonList.Api.IEditableTextControl.TextChanged</code> | ✅ 一致 |
| <code>RadioButtonList.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>RadioButtonList.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>RadioButtonList.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>RadioButtonList.Api.IPostBackDataHandler.LoadPostData</code> | ✅ 一致 |
| <code>RadioButtonList.Api.IPostBackDataHandler.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>RadioButtonList.Api.ITextControl.Text</code> | ✅ 一致 |
| <code>RadioButtonList.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.BaseDataBoundControl.#ctor</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.BaseDataBoundControl.ConfirmInitState</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.BaseDataBoundControl.DataBind</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.BaseDataBoundControl.DataBound</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.BaseDataBoundControl.DataSource</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.BaseDataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.BaseDataBoundControl.EnsureDataBound</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.BaseDataBoundControl.Initialized</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.BaseDataBoundControl.IsBoundUsingDataSourceID</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.BaseDataBoundControl.IsDataBindingAutomatic</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.BaseDataBoundControl.IsUsingModelBinders</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.BaseDataBoundControl.OnDataBound</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.BaseDataBoundControl.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.BaseDataBoundControl.OnInit</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.BaseDataBoundControl.OnPagePreLoad</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.BaseDataBoundControl.OnPreRender</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.BaseDataBoundControl.PerformSelect</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.BaseDataBoundControl.RequiresDataBinding</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.BaseDataBoundControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.BaseDataBoundControl.ValidateDataSource</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.DataBoundControl.#ctor</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.DataBoundControl.CallingDataMethods</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.DataBoundControl.CreateDataSourceSelectArguments</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.DataBoundControl.CreatingModelDataSource</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.DataBoundControl.DataMember</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.DataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.DataBoundControl.DataSourceObject</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.DataBoundControl.GetData</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.DataBoundControl.GetDataSource</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.DataBoundControl.IsUsingModelBinders</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.DataBoundControl.ItemType</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.DataBoundControl.LoadViewState</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.DataBoundControl.MarkAsDataBound</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.DataBoundControl.OnCreatingModelDataSource</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.DataBoundControl.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.DataBoundControl.OnDataSourceViewChanged</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.DataBoundControl.OnLoad</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.DataBoundControl.OnPagePreLoad</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.DataBoundControl.PerformDataBinding</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.DataBoundControl.PerformSelect</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.DataBoundControl.SaveViewState</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.DataBoundControl.SelectArguments</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.DataBoundControl.SelectMethod</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.DataBoundControl.TrackViewState</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.DataBoundControl.ValidateDataSource</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.IRepeatInfoUser.GetItemStyle</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.IRepeatInfoUser.HasFooter</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.IRepeatInfoUser.HasHeader</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.IRepeatInfoUser.HasSeparators</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.IRepeatInfoUser.RenderItem</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.IRepeatInfoUser.RepeatedItemCount</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.#ctor</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.AppendDataBoundItems</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.AutoPostBack</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.CausesValidation</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.ClearSelection</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.DataTextField</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.DataTextFormatString</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.DataValueField</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.Items</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.LoadViewState</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.OnDataBinding</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.OnPreRender</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.OnSelectedIndexChanged</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.OnTextChanged</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.PerformDataBinding</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.PerformSelect</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.RenderContents</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.SaveViewState</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.SelectedIndex</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.SelectedIndexChanged</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.SelectedItem</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.SelectedValue</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.SetPostDataSelection</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.TagKey</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.Text</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.TextChanged</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.TrackViewState</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.ValidationGroup</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.ListControl.VerifyMultiSelect</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.RadioButtonList.#ctor</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.RadioButtonList.CellPadding</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.RadioButtonList.CellSpacing</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.RadioButtonList.CreateControlStyle</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.RadioButtonList.FindControl</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.RadioButtonList.GetItemStyle</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.RadioButtonList.HasFooter</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.RadioButtonList.HasHeader</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.RadioButtonList.HasSeparators</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.RadioButtonList.LoadPostData</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.RadioButtonList.OnInit</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.RadioButtonList.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.RadioButtonList.Render</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.RadioButtonList.RenderItem</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.RadioButtonList.RenderWhenDataEmpty</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.RadioButtonList.RepeatColumns</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.RadioButtonList.RepeatDirection</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.RadioButtonList.RepeatLayout</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.RadioButtonList.RepeatedItemCount</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.RadioButtonList.TextAlign</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>RadioButtonList.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>RadioButtonList.(case list)</code> | ✅ 一致 |
| <code>RadioButtonList.(default)</code> | ✅ 一致 |
| <code>RadioButtonList.AccessKey=empty</code> | ✅ 一致 |
| <code>RadioButtonList.AccessKey=japanese</code> | ✅ 一致 |
| <code>RadioButtonList.AccessKey=k</code> | ✅ 一致 |
| <code>RadioButtonList.AccessKey=markup</code> | ✅ 一致 |
| <code>RadioButtonList.AccessKey=text</code> | ✅ 一致 |
| <code>RadioButtonList.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>RadioButtonList.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>RadioButtonList.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>RadioButtonList.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>RadioButtonList.AppendDataBoundItems=false</code> | ✅ 一致 |
| <code>RadioButtonList.AppendDataBoundItems=true</code> | ✅ 一致 |
| <code>RadioButtonList.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>RadioButtonList.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>RadioButtonList.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>RadioButtonList.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>RadioButtonList.AutoPostBack=false</code> | ✅ 一致 |
| <code>RadioButtonList.AutoPostBack=true</code> | ✅ 一致 |
| <code>RadioButtonList.AutoPostBack=true+CausesValidation=true</code> | ✅ 一致 |
| <code>RadioButtonList.AutoPostBack=true+CausesValidation=true+ValidationGroup=text</code> | ✅ 一致 |
| <code>RadioButtonList.AutoPostBack=true+Enabled=false</code> | ✅ 一致 |
| <code>RadioButtonList.AutoPostBack=true+Items[1].Enabled=false</code> | ✅ 一致 |
| <code>RadioButtonList.BackColor=#3366CC</code> | ✅ 一致 |
| <code>RadioButtonList.BackColor=Red</code> | ✅ 一致 |
| <code>RadioButtonList.BackColor=Transparent</code> | ✅ 一致 |
| <code>RadioButtonList.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>RadioButtonList.BorderColor=Red</code> | ✅ 一致 |
| <code>RadioButtonList.BorderColor=Transparent</code> | ✅ 一致 |
| <code>RadioButtonList.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>RadioButtonList.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>RadioButtonList.BorderStyle=Double</code> | ✅ 一致 |
| <code>RadioButtonList.BorderStyle=Groove</code> | ✅ 一致 |
| <code>RadioButtonList.BorderStyle=Inset</code> | ✅ 一致 |
| <code>RadioButtonList.BorderStyle=None</code> | ✅ 一致 |
| <code>RadioButtonList.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>RadioButtonList.BorderStyle=Outset</code> | ✅ 一致 |
| <code>RadioButtonList.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>RadioButtonList.BorderStyle=Solid</code> | ✅ 一致 |
| <code>RadioButtonList.BorderWidth=120px</code> | ✅ 一致 |
| <code>RadioButtonList.BorderWidth=2em</code> | ✅ 一致 |
| <code>RadioButtonList.BorderWidth=50%</code> | ✅ 一致 |
| <code>RadioButtonList.CausesValidation=false</code> | ✅ 一致 |
| <code>RadioButtonList.CausesValidation=true</code> | ✅ 一致 |
| <code>RadioButtonList.CellPadding=-1</code> | ✅ 一致 |
| <code>RadioButtonList.CellPadding=0</code> | ✅ 一致 |
| <code>RadioButtonList.CellPadding=3</code> | ✅ 一致 |
| <code>RadioButtonList.CellSpacing=-1</code> | ✅ 一致 |
| <code>RadioButtonList.CellSpacing=0</code> | ✅ 一致 |
| <code>RadioButtonList.CellSpacing=3</code> | ✅ 一致 |
| <code>RadioButtonList.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>RadioButtonList.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>RadioButtonList.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>RadioButtonList.ClientIDMode=Static</code> | ✅ 一致 |
| <code>RadioButtonList.CssClass=empty</code> | ✅ 一致 |
| <code>RadioButtonList.CssClass=japanese</code> | ✅ 一致 |
| <code>RadioButtonList.CssClass=markup</code> | ✅ 一致 |
| <code>RadioButtonList.CssClass=text</code> | ✅ 一致 |
| <code>RadioButtonList.DataMember=empty</code> | ✅ 一致 |
| <code>RadioButtonList.DataMember=japanese</code> | ✅ 一致 |
| <code>RadioButtonList.DataMember=markup</code> | ✅ 一致 |
| <code>RadioButtonList.DataMember=text</code> | ✅ 一致 |
| <code>RadioButtonList.DataSourceID=empty</code> | ✅ 一致 |
| <code>RadioButtonList.DataSourceID=japanese</code> | ✅ 一致 |
| <code>RadioButtonList.DataSourceID=markup</code> | ✅ 一致 |
| <code>RadioButtonList.DataSourceID=text</code> | ✅ 一致 |
| <code>RadioButtonList.DataTextField=Name</code> | ✅ 一致 |
| <code>RadioButtonList.DataTextField=Price</code> | ✅ 一致 |
| <code>RadioButtonList.DataTextField=empty</code> | ✅ 一致 |
| <code>RadioButtonList.DataTextField=japanese</code> | ✅ 一致 |
| <code>RadioButtonList.DataTextField=markup</code> | ✅ 一致 |
| <code>RadioButtonList.DataTextField=text</code> | ✅ 一致 |
| <code>RadioButtonList.DataTextFormatString=N2</code> | ✅ 一致 |
| <code>RadioButtonList.DataTextFormatString=empty</code> | ✅ 一致 |
| <code>RadioButtonList.DataTextFormatString=japanese</code> | ✅ 一致 |
| <code>RadioButtonList.DataTextFormatString=markup</code> | ✅ 一致 |
| <code>RadioButtonList.DataTextFormatString=text</code> | ✅ 一致 |
| <code>RadioButtonList.DataValueField=Code</code> | ✅ 一致 |
| <code>RadioButtonList.DataValueField=empty</code> | ✅ 一致 |
| <code>RadioButtonList.DataValueField=japanese</code> | ✅ 一致 |
| <code>RadioButtonList.DataValueField=markup</code> | ✅ 一致 |
| <code>RadioButtonList.DataValueField=text</code> | ✅ 一致 |
| <code>RadioButtonList.EnableTheming=false</code> | ✅ 一致 |
| <code>RadioButtonList.EnableTheming=true</code> | ✅ 一致 |
| <code>RadioButtonList.EnableViewState=false</code> | ✅ 一致 |
| <code>RadioButtonList.EnableViewState=true</code> | ✅ 一致 |
| <code>RadioButtonList.Enabled=false</code> | ✅ 一致 |
| <code>RadioButtonList.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>RadioButtonList.Enabled=true</code> | ✅ 一致 |
| <code>RadioButtonList.Font.Bold=false</code> | ✅ 一致 |
| <code>RadioButtonList.Font.Bold=true</code> | ✅ 一致 |
| <code>RadioButtonList.Font.Italic=false</code> | ✅ 一致 |
| <code>RadioButtonList.Font.Italic=true</code> | ✅ 一致 |
| <code>RadioButtonList.Font.Name=empty</code> | ✅ 一致 |
| <code>RadioButtonList.Font.Name=japanese</code> | ✅ 一致 |
| <code>RadioButtonList.Font.Name=markup</code> | ✅ 一致 |
| <code>RadioButtonList.Font.Name=text</code> | ✅ 一致 |
| <code>RadioButtonList.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>RadioButtonList.Font.Overline=false</code> | ✅ 一致 |
| <code>RadioButtonList.Font.Overline=true</code> | ✅ 一致 |
| <code>RadioButtonList.Font.Size=12pt</code> | ✅ 一致 |
| <code>RadioButtonList.Font.Size=150%</code> | ✅ 一致 |
| <code>RadioButtonList.Font.Size=Large</code> | ✅ 一致 |
| <code>RadioButtonList.Font.Strikeout=false</code> | ✅ 一致 |
| <code>RadioButtonList.Font.Strikeout=true</code> | ✅ 一致 |
| <code>RadioButtonList.Font.Underline=false</code> | ✅ 一致 |
| <code>RadioButtonList.Font.Underline=true</code> | ✅ 一致 |
| <code>RadioButtonList.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>RadioButtonList.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>RadioButtonList.ForeColor=Red</code> | ✅ 一致 |
| <code>RadioButtonList.ForeColor=Transparent</code> | ✅ 一致 |
| <code>RadioButtonList.Height=120px</code> | ✅ 一致 |
| <code>RadioButtonList.Height=2em</code> | ✅ 一致 |
| <code>RadioButtonList.Height=50%</code> | ✅ 一致 |
| <code>RadioButtonList.ID=empty</code> | ✅ 一致 |
| <code>RadioButtonList.ID=japanese</code> | ✅ 一致 |
| <code>RadioButtonList.ID=markup</code> | ✅ 一致 |
| <code>RadioButtonList.ID=text</code> | ✅ 一致 |
| <code>RadioButtonList.ItemType=empty</code> | ✅ 一致 |
| <code>RadioButtonList.ItemType=japanese</code> | ✅ 一致 |
| <code>RadioButtonList.ItemType=markup</code> | ✅ 一致 |
| <code>RadioButtonList.ItemType=text</code> | ✅ 一致 |
| <code>RadioButtonList.Items[0].Selected=true+Items[2].Selected=true</code> | ✅ 一致 |
| <code>RadioButtonList.Items[1].Enabled=false</code> | ✅ 一致 |
| <code>RadioButtonList.Items[1].Enabled=false+Enabled=false</code> | ✅ 一致 |
| <code>RadioButtonList.Items[1].Enabled=true</code> | ✅ 一致 |
| <code>RadioButtonList.Items[1].Selected=false</code> | ✅ 一致 |
| <code>RadioButtonList.Items[1].Selected=true</code> | ✅ 一致 |
| <code>RadioButtonList.Items[1].Selected=true+Enabled=false</code> | ✅ 一致 |
| <code>RadioButtonList.Items[1].Text=empty</code> | ✅ 一致 |
| <code>RadioButtonList.Items[1].Text=empty+Items[1].Value=text</code> | ✅ 一致 |
| <code>RadioButtonList.Items[1].Text=japanese</code> | ✅ 一致 |
| <code>RadioButtonList.Items[1].Text=markup</code> | ✅ 一致 |
| <code>RadioButtonList.Items[1].Text=markup+Items[1].Value=markup</code> | ✅ 一致 |
| <code>RadioButtonList.Items[1].Text=spaces</code> | ✅ 一致 |
| <code>RadioButtonList.Items[1].Text=text</code> | ✅ 一致 |
| <code>RadioButtonList.Items[1].Value=empty</code> | ✅ 一致 |
| <code>RadioButtonList.Items[1].Value=japanese</code> | ✅ 一致 |
| <code>RadioButtonList.Items[1].Value=markup</code> | ✅ 一致 |
| <code>RadioButtonList.Items[1].Value=text</code> | ✅ 一致 |
| <code>RadioButtonList.RenderWhenDataEmpty=false</code> | ✅ 一致 |
| <code>RadioButtonList.RenderWhenDataEmpty=true</code> | ✅ 一致 |
| <code>RadioButtonList.RepeatColumns=-1</code> | ✅ 一致 |
| <code>RadioButtonList.RepeatColumns=0</code> | ✅ 一致 |
| <code>RadioButtonList.RepeatColumns=3</code> | ✅ 一致 |
| <code>RadioButtonList.RepeatColumns=3+RepeatDirection=Horizontal</code> | ✅ 一致 |
| <code>RadioButtonList.RepeatColumns=3+RepeatDirection=Vertical</code> | ✅ 一致 |
| <code>RadioButtonList.RepeatDirection=Horizontal</code> | ✅ 一致 |
| <code>RadioButtonList.RepeatDirection=Vertical</code> | ✅ 一致 |
| <code>RadioButtonList.RepeatLayout=Flow</code> | ✅ 一致 |
| <code>RadioButtonList.RepeatLayout=Flow+RepeatColumns=3</code> | ✅ 一致 |
| <code>RadioButtonList.RepeatLayout=Flow+TextAlign=Left</code> | ✅ 一致 |
| <code>RadioButtonList.RepeatLayout=OrderedList</code> | ✅ 一致 |
| <code>RadioButtonList.RepeatLayout=OrderedList+RepeatDirection=Horizontal</code> | ✅ 一致 |
| <code>RadioButtonList.RepeatLayout=Table</code> | ✅ 一致 |
| <code>RadioButtonList.RepeatLayout=Table+CellPadding=3+CellSpacing=3</code> | ✅ 一致 |
| <code>RadioButtonList.RepeatLayout=UnorderedList</code> | ✅ 一致 |
| <code>RadioButtonList.RepeatLayout=UnorderedList+RepeatColumns=3</code> | ✅ 一致 |
| <code>RadioButtonList.SelectMethod=empty</code> | ✅ 一致 |
| <code>RadioButtonList.SelectMethod=japanese</code> | ✅ 一致 |
| <code>RadioButtonList.SelectMethod=markup</code> | ✅ 一致 |
| <code>RadioButtonList.SelectMethod=text</code> | ✅ 一致 |
| <code>RadioButtonList.SelectedIndex=-1</code> | ✅ 一致 |
| <code>RadioButtonList.SelectedIndex=0</code> | ✅ 一致 |
| <code>RadioButtonList.SelectedIndex=1</code> | ✅ 一致 |
| <code>RadioButtonList.SelectedIndex=1+EnableViewState=false</code> | ✅ 一致 |
| <code>RadioButtonList.SelectedIndex=1+Items[1].Enabled=false</code> | ✅ 一致 |
| <code>RadioButtonList.SelectedIndex=1+Visible=false</code> | ✅ 一致 |
| <code>RadioButtonList.SelectedIndex=3</code> | ✅ 一致 |
| <code>RadioButtonList.SelectedValue=B2</code> | ✅ 一致 |
| <code>RadioButtonList.SelectedValue=M</code> | ✅ 一致 |
| <code>RadioButtonList.SelectedValue=M+SelectedIndex=0</code> | ✅ 一致 |
| <code>RadioButtonList.SelectedValue=empty</code> | ✅ 一致 |
| <code>RadioButtonList.SelectedValue=japanese</code> | ✅ 一致 |
| <code>RadioButtonList.SelectedValue=markup</code> | ✅ 一致 |
| <code>RadioButtonList.SelectedValue=text</code> | ✅ 一致 |
| <code>RadioButtonList.SkinID=empty</code> | ✅ 一致 |
| <code>RadioButtonList.SkinID=japanese</code> | ✅ 一致 |
| <code>RadioButtonList.SkinID=markup</code> | ✅ 一致 |
| <code>RadioButtonList.SkinID=text</code> | ✅ 一致 |
| <code>RadioButtonList.Style[color]=1em</code> | ✅ 一致 |
| <code>RadioButtonList.Style[color]=red</code> | ✅ 一致 |
| <code>RadioButtonList.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>RadioButtonList.Style[margin-left]=red</code> | ✅ 一致 |
| <code>RadioButtonList.TabIndex=-1</code> | ✅ 一致 |
| <code>RadioButtonList.TabIndex=0</code> | ✅ 一致 |
| <code>RadioButtonList.TabIndex=3</code> | ✅ 一致 |
| <code>RadioButtonList.Text=empty</code> | ✅ 一致 |
| <code>RadioButtonList.Text=japanese</code> | ✅ 一致 |
| <code>RadioButtonList.Text=markup</code> | ✅ 一致 |
| <code>RadioButtonList.Text=text</code> | ✅ 一致 |
| <code>RadioButtonList.TextAlign=Left</code> | ✅ 一致 |
| <code>RadioButtonList.TextAlign=Right</code> | ✅ 一致 |
| <code>RadioButtonList.ToolTip=empty</code> | ✅ 一致 |
| <code>RadioButtonList.ToolTip=japanese</code> | ✅ 一致 |
| <code>RadioButtonList.ToolTip=markup</code> | ✅ 一致 |
| <code>RadioButtonList.ToolTip=text</code> | ✅ 一致 |
| <code>RadioButtonList.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>RadioButtonList.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>RadioButtonList.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>RadioButtonList.ValidationGroup=empty</code> | ✅ 一致 |
| <code>RadioButtonList.ValidationGroup=japanese</code> | ✅ 一致 |
| <code>RadioButtonList.ValidationGroup=markup</code> | ✅ 一致 |
| <code>RadioButtonList.ValidationGroup=text</code> | ✅ 一致 |
| <code>RadioButtonList.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>RadioButtonList.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>RadioButtonList.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>RadioButtonList.Visible=false</code> | ✅ 一致 |
| <code>RadioButtonList.Visible=true</code> | ✅ 一致 |
| <code>RadioButtonList.Width=120px</code> | ✅ 一致 |
| <code>RadioButtonList.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>RadioButtonList.Width=2em</code> | ✅ 一致 |
| <code>RadioButtonList.Width=50%</code> | ✅ 一致 |
| <code>RadioButtonList[data-bound].(default)</code> | ✅ 一致 |
| <code>RadioButtonList[data-bound].DataTextField=Name</code> | ✅ 一致 |
| <code>RadioButtonList[data-bound].DataTextField=Name+AppendDataBoundItems=true</code> | ✅ 一致 |
| <code>RadioButtonList[data-bound].DataTextField=Name+DataValueField=Code</code> | ✅ 一致 |
| <code>RadioButtonList[data-bound].DataTextField=Name+DataValueField=Code+SelectedIndex=1</code> | ✅ 一致 |
| <code>RadioButtonList[data-bound].DataTextField=Name+DataValueField=Code+SelectedValue=B2</code> | ✅ 一致 |
| <code>RadioButtonList[data-bound].DataTextField=Name+DataValueField=Code+SelectedValue=M</code> | ✅ 一致 |
| <code>RadioButtonList[data-bound].DataTextField=Price+DataTextFormatString=N2</code> | ✅ 一致 |
| <code>RadioButtonList[data-bound].DataTextField=text</code> | ✅ 一致 |
| <code>RadioButtonList[data-bound].DataValueField=Code</code> | ✅ 一致 |
| <code>RadioButtonList[naming container].(default)</code> | ✅ 一致 |
| <code>RadioButtonList[naming container].AutoPostBack=true</code> | ✅ 一致 |
| <code>RadioButtonList[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>RadioButtonList[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>RadioButtonList[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>RadioButtonList[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>RadioButtonList[naming container].ID=text</code> | ✅ 一致 |
| <code>RadioButtonList[themed].(default)</code> | ✅ 一致 |
| <code>RadioButtonList[themed].CssClass=text</code> | ✅ 一致 |
| <code>RadioButtonList[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>RadioButtonList[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| RadioButtonList [render][databind][items] first request | ✅ 一致 |
| RadioButtonList [validation] group g2, nothing chosen | ✅ 一致 |
| RadioButtonList [postdata][change][dynamic] radios chosen | ✅ 一致 |
| RadioButtonList [viewstate] posted again unchanged | ✅ 一致 |
| RadioButtonList [validation] group g2, chosen | ✅ 一致 |
| RadioButtonList [autopostback][change] auto postback | ✅ 一致 |
| RadioButtonList [clientvalidation] group g3 in the browser: no request | ✅ 一致 |
| RadioButtonList [visible][viewstate][items] hidden lists shown with the items added in code | ✅ 一致 |
| RadioButtonList [bind] two-way binding in a template | ✅ 一致 |
| RadioButtonList [eventvalidation] a value the list does not have posted | ✅ 一致 |
| RadioButtonList [requestvalidation] markup posted | ✅ 一致 |

</details>
