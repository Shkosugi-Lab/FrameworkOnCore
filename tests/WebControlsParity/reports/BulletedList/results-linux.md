# BulletedList パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 01:27
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 243 | 243 | 0 | 0 |
| ② HTML | 238 | 238 | 0 | 0 |
| ③ シナリオ | 6 | 6 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 252 件中 252 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 49 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 6 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 199 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 1 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 7 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 1 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 8 | 0 | ✅ |
| ポストバックイベント | 0 | 4 | ✅ |
| AutoPostBack（クライアントスクリプトによるポストバック） | 1 | 0 | ✅ |
| CausesValidation / ValidationGroup | 8 | 1 | ✅ |
| 項目（ListItem の Selected・Enabled・Text・Value と ViewState） | 20 | 2 | ✅ |
| データソース連携（DataSource・DataTextField など） | 10 | 0 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>BulletedList.Api.Control.#ctor</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.Adapter</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.ClientID</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.Context</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.Controls</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.DataBind</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.Dispose</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.Disposed</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.Events</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.FindControl</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.Focus</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.HasControls</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.ID</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.Init</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.Load</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.OnInit</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.Page</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.Parent</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.PreRender</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.Render</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.Site</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.SkinID</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.Unload</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.ViewState</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>BulletedList.Api.Control.Visible</code> | ✅ 一致 |
| <code>BulletedList.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>BulletedList.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>BulletedList.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>BulletedList.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>BulletedList.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>BulletedList.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>BulletedList.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>BulletedList.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>BulletedList.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>BulletedList.Api.IEditableTextControl.TextChanged</code> | ✅ 一致 |
| <code>BulletedList.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>BulletedList.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>BulletedList.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>BulletedList.Api.IPostBackEventHandler.RaisePostBackEvent</code> | ✅ 一致 |
| <code>BulletedList.Api.ITextControl.Text</code> | ✅ 一致 |
| <code>BulletedList.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BaseDataBoundControl.#ctor</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BaseDataBoundControl.ConfirmInitState</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BaseDataBoundControl.DataBind</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BaseDataBoundControl.DataBound</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BaseDataBoundControl.DataSource</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BaseDataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BaseDataBoundControl.EnsureDataBound</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BaseDataBoundControl.Initialized</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BaseDataBoundControl.IsBoundUsingDataSourceID</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BaseDataBoundControl.IsDataBindingAutomatic</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BaseDataBoundControl.IsUsingModelBinders</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BaseDataBoundControl.OnDataBound</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BaseDataBoundControl.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BaseDataBoundControl.OnInit</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BaseDataBoundControl.OnPagePreLoad</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BaseDataBoundControl.OnPreRender</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BaseDataBoundControl.PerformSelect</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BaseDataBoundControl.RequiresDataBinding</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BaseDataBoundControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BaseDataBoundControl.ValidateDataSource</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BulletedList.#ctor</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BulletedList.AddAttributesToRender</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BulletedList.AutoPostBack</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BulletedList.BulletImageUrl</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BulletedList.BulletStyle</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BulletedList.Click</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BulletedList.Controls</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BulletedList.DisplayMode</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BulletedList.FirstBulletNumber</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BulletedList.OnClick</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BulletedList.RaisePostBackEvent</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BulletedList.Render</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BulletedList.RenderBulletText</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BulletedList.RenderContents</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BulletedList.RenderWhenDataEmpty</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BulletedList.SelectedIndex</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BulletedList.SelectedItem</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BulletedList.SelectedValue</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BulletedList.TagKey</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BulletedList.Target</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.BulletedList.Text</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.DataBoundControl.#ctor</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.DataBoundControl.CallingDataMethods</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.DataBoundControl.CreateDataSourceSelectArguments</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.DataBoundControl.CreatingModelDataSource</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.DataBoundControl.DataMember</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.DataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.DataBoundControl.DataSourceObject</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.DataBoundControl.GetData</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.DataBoundControl.GetDataSource</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.DataBoundControl.IsUsingModelBinders</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.DataBoundControl.ItemType</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.DataBoundControl.LoadViewState</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.DataBoundControl.MarkAsDataBound</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.DataBoundControl.OnCreatingModelDataSource</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.DataBoundControl.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.DataBoundControl.OnDataSourceViewChanged</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.DataBoundControl.OnLoad</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.DataBoundControl.OnPagePreLoad</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.DataBoundControl.PerformDataBinding</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.DataBoundControl.PerformSelect</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.DataBoundControl.SaveViewState</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.DataBoundControl.SelectArguments</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.DataBoundControl.SelectMethod</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.DataBoundControl.TrackViewState</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.DataBoundControl.ValidateDataSource</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.#ctor</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.AppendDataBoundItems</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.AutoPostBack</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.CausesValidation</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.ClearSelection</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.DataTextField</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.DataTextFormatString</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.DataValueField</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.Items</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.LoadViewState</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.OnDataBinding</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.OnPreRender</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.OnSelectedIndexChanged</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.OnTextChanged</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.PerformDataBinding</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.PerformSelect</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.RenderContents</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.SaveViewState</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.SelectedIndex</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.SelectedIndexChanged</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.SelectedItem</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.SelectedValue</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.SetPostDataSelection</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.TagKey</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.Text</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.TextChanged</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.TrackViewState</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.ValidationGroup</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.ListControl.VerifyMultiSelect</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>BulletedList.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>BulletedList.(case list)</code> | ✅ 一致 |
| <code>BulletedList.(default)</code> | ✅ 一致 |
| <code>BulletedList.AccessKey=empty</code> | ✅ 一致 |
| <code>BulletedList.AccessKey=japanese</code> | ✅ 一致 |
| <code>BulletedList.AccessKey=k</code> | ✅ 一致 |
| <code>BulletedList.AccessKey=markup</code> | ✅ 一致 |
| <code>BulletedList.AccessKey=text</code> | ✅ 一致 |
| <code>BulletedList.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>BulletedList.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>BulletedList.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>BulletedList.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>BulletedList.AppendDataBoundItems=false</code> | ✅ 一致 |
| <code>BulletedList.AppendDataBoundItems=true</code> | ✅ 一致 |
| <code>BulletedList.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>BulletedList.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>BulletedList.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>BulletedList.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>BulletedList.AutoPostBack=false</code> | ✅ 一致 |
| <code>BulletedList.AutoPostBack=true</code> | ✅ 一致 |
| <code>BulletedList.BackColor=#3366CC</code> | ✅ 一致 |
| <code>BulletedList.BackColor=Red</code> | ✅ 一致 |
| <code>BulletedList.BackColor=Transparent</code> | ✅ 一致 |
| <code>BulletedList.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>BulletedList.BorderColor=Red</code> | ✅ 一致 |
| <code>BulletedList.BorderColor=Transparent</code> | ✅ 一致 |
| <code>BulletedList.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>BulletedList.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>BulletedList.BorderStyle=Double</code> | ✅ 一致 |
| <code>BulletedList.BorderStyle=Groove</code> | ✅ 一致 |
| <code>BulletedList.BorderStyle=Inset</code> | ✅ 一致 |
| <code>BulletedList.BorderStyle=None</code> | ✅ 一致 |
| <code>BulletedList.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>BulletedList.BorderStyle=Outset</code> | ✅ 一致 |
| <code>BulletedList.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>BulletedList.BorderStyle=Solid</code> | ✅ 一致 |
| <code>BulletedList.BorderWidth=120px</code> | ✅ 一致 |
| <code>BulletedList.BorderWidth=2em</code> | ✅ 一致 |
| <code>BulletedList.BorderWidth=50%</code> | ✅ 一致 |
| <code>BulletedList.BulletImageUrl=empty</code> | ✅ 一致 |
| <code>BulletedList.BulletImageUrl=japanese</code> | ✅ 一致 |
| <code>BulletedList.BulletImageUrl=markup</code> | ✅ 一致 |
| <code>BulletedList.BulletImageUrl=text</code> | ✅ 一致 |
| <code>BulletedList.BulletStyle=Circle</code> | ✅ 一致 |
| <code>BulletedList.BulletStyle=CustomImage</code> | ✅ 一致 |
| <code>BulletedList.BulletStyle=CustomImage+BulletImageUrl=text</code> | ✅ 一致 |
| <code>BulletedList.BulletStyle=Disc</code> | ✅ 一致 |
| <code>BulletedList.BulletStyle=LowerAlpha</code> | ✅ 一致 |
| <code>BulletedList.BulletStyle=LowerRoman</code> | ✅ 一致 |
| <code>BulletedList.BulletStyle=NotSet</code> | ✅ 一致 |
| <code>BulletedList.BulletStyle=Numbered</code> | ✅ 一致 |
| <code>BulletedList.BulletStyle=Numbered+FirstBulletNumber=3</code> | ✅ 一致 |
| <code>BulletedList.BulletStyle=Square</code> | ✅ 一致 |
| <code>BulletedList.BulletStyle=UpperAlpha</code> | ✅ 一致 |
| <code>BulletedList.BulletStyle=UpperRoman</code> | ✅ 一致 |
| <code>BulletedList.CausesValidation=false</code> | ✅ 一致 |
| <code>BulletedList.CausesValidation=true</code> | ✅ 一致 |
| <code>BulletedList.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>BulletedList.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>BulletedList.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>BulletedList.ClientIDMode=Static</code> | ✅ 一致 |
| <code>BulletedList.CssClass=empty</code> | ✅ 一致 |
| <code>BulletedList.CssClass=japanese</code> | ✅ 一致 |
| <code>BulletedList.CssClass=markup</code> | ✅ 一致 |
| <code>BulletedList.CssClass=text</code> | ✅ 一致 |
| <code>BulletedList.DataMember=empty</code> | ✅ 一致 |
| <code>BulletedList.DataMember=japanese</code> | ✅ 一致 |
| <code>BulletedList.DataMember=markup</code> | ✅ 一致 |
| <code>BulletedList.DataMember=text</code> | ✅ 一致 |
| <code>BulletedList.DataSourceID=empty</code> | ✅ 一致 |
| <code>BulletedList.DataSourceID=japanese</code> | ✅ 一致 |
| <code>BulletedList.DataSourceID=markup</code> | ✅ 一致 |
| <code>BulletedList.DataSourceID=text</code> | ✅ 一致 |
| <code>BulletedList.DataTextField=Name</code> | ✅ 一致 |
| <code>BulletedList.DataTextField=Price</code> | ✅ 一致 |
| <code>BulletedList.DataTextField=empty</code> | ✅ 一致 |
| <code>BulletedList.DataTextField=japanese</code> | ✅ 一致 |
| <code>BulletedList.DataTextField=markup</code> | ✅ 一致 |
| <code>BulletedList.DataTextField=text</code> | ✅ 一致 |
| <code>BulletedList.DataTextFormatString=N2</code> | ✅ 一致 |
| <code>BulletedList.DataTextFormatString=empty</code> | ✅ 一致 |
| <code>BulletedList.DataTextFormatString=japanese</code> | ✅ 一致 |
| <code>BulletedList.DataTextFormatString=markup</code> | ✅ 一致 |
| <code>BulletedList.DataTextFormatString=text</code> | ✅ 一致 |
| <code>BulletedList.DataValueField=Code</code> | ✅ 一致 |
| <code>BulletedList.DataValueField=empty</code> | ✅ 一致 |
| <code>BulletedList.DataValueField=japanese</code> | ✅ 一致 |
| <code>BulletedList.DataValueField=markup</code> | ✅ 一致 |
| <code>BulletedList.DataValueField=text</code> | ✅ 一致 |
| <code>BulletedList.DisplayMode=HyperLink</code> | ✅ 一致 |
| <code>BulletedList.DisplayMode=HyperLink+Items[1].Enabled=false</code> | ✅ 一致 |
| <code>BulletedList.DisplayMode=HyperLink+Target=blank</code> | ✅ 一致 |
| <code>BulletedList.DisplayMode=LinkButton</code> | ✅ 一致 |
| <code>BulletedList.DisplayMode=LinkButton+CausesValidation=false</code> | ✅ 一致 |
| <code>BulletedList.DisplayMode=LinkButton+Enabled=false</code> | ✅ 一致 |
| <code>BulletedList.DisplayMode=LinkButton+Items[1].Enabled=false</code> | ✅ 一致 |
| <code>BulletedList.DisplayMode=LinkButton+Items[1].Text=markup</code> | ✅ 一致 |
| <code>BulletedList.DisplayMode=LinkButton+ValidationGroup=text</code> | ✅ 一致 |
| <code>BulletedList.DisplayMode=Text</code> | ✅ 一致 |
| <code>BulletedList.EnableTheming=false</code> | ✅ 一致 |
| <code>BulletedList.EnableTheming=true</code> | ✅ 一致 |
| <code>BulletedList.EnableViewState=false</code> | ✅ 一致 |
| <code>BulletedList.EnableViewState=true</code> | ✅ 一致 |
| <code>BulletedList.Enabled=false</code> | ✅ 一致 |
| <code>BulletedList.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>BulletedList.Enabled=true</code> | ✅ 一致 |
| <code>BulletedList.FirstBulletNumber=-1</code> | ✅ 一致 |
| <code>BulletedList.FirstBulletNumber=0</code> | ✅ 一致 |
| <code>BulletedList.FirstBulletNumber=3</code> | ✅ 一致 |
| <code>BulletedList.Font.Bold=false</code> | ✅ 一致 |
| <code>BulletedList.Font.Bold=true</code> | ✅ 一致 |
| <code>BulletedList.Font.Italic=false</code> | ✅ 一致 |
| <code>BulletedList.Font.Italic=true</code> | ✅ 一致 |
| <code>BulletedList.Font.Name=empty</code> | ✅ 一致 |
| <code>BulletedList.Font.Name=japanese</code> | ✅ 一致 |
| <code>BulletedList.Font.Name=markup</code> | ✅ 一致 |
| <code>BulletedList.Font.Name=text</code> | ✅ 一致 |
| <code>BulletedList.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>BulletedList.Font.Overline=false</code> | ✅ 一致 |
| <code>BulletedList.Font.Overline=true</code> | ✅ 一致 |
| <code>BulletedList.Font.Size=12pt</code> | ✅ 一致 |
| <code>BulletedList.Font.Size=150%</code> | ✅ 一致 |
| <code>BulletedList.Font.Size=Large</code> | ✅ 一致 |
| <code>BulletedList.Font.Strikeout=false</code> | ✅ 一致 |
| <code>BulletedList.Font.Strikeout=true</code> | ✅ 一致 |
| <code>BulletedList.Font.Underline=false</code> | ✅ 一致 |
| <code>BulletedList.Font.Underline=true</code> | ✅ 一致 |
| <code>BulletedList.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>BulletedList.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>BulletedList.ForeColor=Red</code> | ✅ 一致 |
| <code>BulletedList.ForeColor=Transparent</code> | ✅ 一致 |
| <code>BulletedList.Height=120px</code> | ✅ 一致 |
| <code>BulletedList.Height=2em</code> | ✅ 一致 |
| <code>BulletedList.Height=50%</code> | ✅ 一致 |
| <code>BulletedList.ID=empty</code> | ✅ 一致 |
| <code>BulletedList.ID=japanese</code> | ✅ 一致 |
| <code>BulletedList.ID=markup</code> | ✅ 一致 |
| <code>BulletedList.ID=text</code> | ✅ 一致 |
| <code>BulletedList.ItemType=empty</code> | ✅ 一致 |
| <code>BulletedList.ItemType=japanese</code> | ✅ 一致 |
| <code>BulletedList.ItemType=markup</code> | ✅ 一致 |
| <code>BulletedList.ItemType=text</code> | ✅ 一致 |
| <code>BulletedList.Items[1].Enabled=false</code> | ✅ 一致 |
| <code>BulletedList.Items[1].Enabled=true</code> | ✅ 一致 |
| <code>BulletedList.Items[1].Selected=false</code> | ✅ 一致 |
| <code>BulletedList.Items[1].Selected=true</code> | ✅ 一致 |
| <code>BulletedList.Items[1].Selected=true+Enabled=false</code> | ✅ 一致 |
| <code>BulletedList.Items[1].Text=empty</code> | ✅ 一致 |
| <code>BulletedList.Items[1].Text=empty+Items[1].Value=text</code> | ✅ 一致 |
| <code>BulletedList.Items[1].Text=japanese</code> | ✅ 一致 |
| <code>BulletedList.Items[1].Text=markup</code> | ✅ 一致 |
| <code>BulletedList.Items[1].Text=markup+Items[1].Value=markup</code> | ✅ 一致 |
| <code>BulletedList.Items[1].Text=spaces</code> | ✅ 一致 |
| <code>BulletedList.Items[1].Text=text</code> | ✅ 一致 |
| <code>BulletedList.Items[1].Value=empty</code> | ✅ 一致 |
| <code>BulletedList.Items[1].Value=japanese</code> | ✅ 一致 |
| <code>BulletedList.Items[1].Value=markup</code> | ✅ 一致 |
| <code>BulletedList.Items[1].Value=text</code> | ✅ 一致 |
| <code>BulletedList.RenderWhenDataEmpty=false</code> | ✅ 一致 |
| <code>BulletedList.RenderWhenDataEmpty=true</code> | ✅ 一致 |
| <code>BulletedList.SelectMethod=empty</code> | ✅ 一致 |
| <code>BulletedList.SelectMethod=japanese</code> | ✅ 一致 |
| <code>BulletedList.SelectMethod=markup</code> | ✅ 一致 |
| <code>BulletedList.SelectMethod=text</code> | ✅ 一致 |
| <code>BulletedList.SelectedIndex=-1</code> | ✅ 一致 |
| <code>BulletedList.SelectedIndex=0</code> | ✅ 一致 |
| <code>BulletedList.SelectedIndex=1</code> | ✅ 一致 |
| <code>BulletedList.SelectedIndex=1+EnableViewState=false</code> | ✅ 一致 |
| <code>BulletedList.SelectedIndex=1+Items[1].Enabled=false</code> | ✅ 一致 |
| <code>BulletedList.SelectedIndex=1+Visible=false</code> | ✅ 一致 |
| <code>BulletedList.SelectedIndex=3</code> | ✅ 一致 |
| <code>BulletedList.SelectedValue=B2</code> | ✅ 一致 |
| <code>BulletedList.SelectedValue=M</code> | ✅ 一致 |
| <code>BulletedList.SelectedValue=M+SelectedIndex=0</code> | ✅ 一致 |
| <code>BulletedList.SelectedValue=empty</code> | ✅ 一致 |
| <code>BulletedList.SelectedValue=japanese</code> | ✅ 一致 |
| <code>BulletedList.SelectedValue=markup</code> | ✅ 一致 |
| <code>BulletedList.SelectedValue=text</code> | ✅ 一致 |
| <code>BulletedList.SkinID=empty</code> | ✅ 一致 |
| <code>BulletedList.SkinID=japanese</code> | ✅ 一致 |
| <code>BulletedList.SkinID=markup</code> | ✅ 一致 |
| <code>BulletedList.SkinID=text</code> | ✅ 一致 |
| <code>BulletedList.Style[color]=1em</code> | ✅ 一致 |
| <code>BulletedList.Style[color]=red</code> | ✅ 一致 |
| <code>BulletedList.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>BulletedList.Style[margin-left]=red</code> | ✅ 一致 |
| <code>BulletedList.TabIndex=-1</code> | ✅ 一致 |
| <code>BulletedList.TabIndex=0</code> | ✅ 一致 |
| <code>BulletedList.TabIndex=3</code> | ✅ 一致 |
| <code>BulletedList.Target=blank</code> | ✅ 一致 |
| <code>BulletedList.Target=empty</code> | ✅ 一致 |
| <code>BulletedList.Target=japanese</code> | ✅ 一致 |
| <code>BulletedList.Target=markup</code> | ✅ 一致 |
| <code>BulletedList.Target=text</code> | ✅ 一致 |
| <code>BulletedList.Text=empty</code> | ✅ 一致 |
| <code>BulletedList.Text=japanese</code> | ✅ 一致 |
| <code>BulletedList.Text=markup</code> | ✅ 一致 |
| <code>BulletedList.Text=text</code> | ✅ 一致 |
| <code>BulletedList.ToolTip=empty</code> | ✅ 一致 |
| <code>BulletedList.ToolTip=japanese</code> | ✅ 一致 |
| <code>BulletedList.ToolTip=markup</code> | ✅ 一致 |
| <code>BulletedList.ToolTip=text</code> | ✅ 一致 |
| <code>BulletedList.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>BulletedList.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>BulletedList.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>BulletedList.ValidationGroup=empty</code> | ✅ 一致 |
| <code>BulletedList.ValidationGroup=japanese</code> | ✅ 一致 |
| <code>BulletedList.ValidationGroup=markup</code> | ✅ 一致 |
| <code>BulletedList.ValidationGroup=text</code> | ✅ 一致 |
| <code>BulletedList.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>BulletedList.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>BulletedList.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>BulletedList.Visible=false</code> | ✅ 一致 |
| <code>BulletedList.Visible=true</code> | ✅ 一致 |
| <code>BulletedList.Width=120px</code> | ✅ 一致 |
| <code>BulletedList.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>BulletedList.Width=2em</code> | ✅ 一致 |
| <code>BulletedList.Width=50%</code> | ✅ 一致 |
| <code>BulletedList[data-bound].(default)</code> | ✅ 一致 |
| <code>BulletedList[data-bound].DataTextField=Name</code> | ✅ 一致 |
| <code>BulletedList[data-bound].DataTextField=Name+AppendDataBoundItems=true</code> | ✅ 一致 |
| <code>BulletedList[data-bound].DataTextField=Name+DataValueField=Code</code> | ✅ 一致 |
| <code>BulletedList[data-bound].DataTextField=Name+DataValueField=Code+SelectedIndex=1</code> | ✅ 一致 |
| <code>BulletedList[data-bound].DataTextField=Name+DataValueField=Code+SelectedValue=B2</code> | ✅ 一致 |
| <code>BulletedList[data-bound].DataTextField=Name+DataValueField=Code+SelectedValue=M</code> | ✅ 一致 |
| <code>BulletedList[data-bound].DataTextField=Price+DataTextFormatString=N2</code> | ✅ 一致 |
| <code>BulletedList[data-bound].DataTextField=text</code> | ✅ 一致 |
| <code>BulletedList[data-bound].DataValueField=Code</code> | ✅ 一致 |
| <code>BulletedList[naming container].(default)</code> | ✅ 一致 |
| <code>BulletedList[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>BulletedList[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>BulletedList[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>BulletedList[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>BulletedList[naming container].DisplayMode=LinkButton</code> | ✅ 一致 |
| <code>BulletedList[naming container].ID=text</code> | ✅ 一致 |
| <code>BulletedList[themed].(default)</code> | ✅ 一致 |
| <code>BulletedList[themed].CssClass=text</code> | ✅ 一致 |
| <code>BulletedList[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>BulletedList[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| BulletedList [render][databind][items] first request (a bound list) | ✅ 一致 |
| BulletedList [postbackevent] a bullet clicked (LinkButton) | ✅ 一致 |
| BulletedList [causesvalidation][postbackevent] a bullet with ValidationGroup g1 | ✅ 一致 |
| BulletedList [dynamic][postbackevent] a dynamically created list&#39;s bullet | ✅ 一致 |
| BulletedList [visible][viewstate][items] hidden list shown with the items added in code | ✅ 一致 |
| BulletedList [eventvalidation][postbackevent] a disabled bullet&#39;s postback forged | ✅ 一致 |

</details>
