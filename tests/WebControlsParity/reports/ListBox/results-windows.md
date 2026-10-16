# ListBox パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 00:18
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 234 | 234 | 0 | 0 |
| ② HTML | 215 | 215 | 0 | 0 |
| ③ シナリオ | 12 | 12 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 243 件中 243 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 45 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 12 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 176 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 2 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 7 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 1 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 7 | 0 | ✅ |
| ポストバックデータの取り込み | 0 | 1 | ✅ |
| 変更イベント（*Changed、ServerChange） | 0 | 3 | ✅ |
| リクエスト検証（ValidateRequestMode） | 3 | 1 | ✅ |
| AutoPostBack（クライアントスクリプトによるポストバック） | 7 | 1 | ✅ |
| CausesValidation / ValidationGroup | 8 | 0 | ✅ |
| 検証対象（サーバー側の検証） | 0 | 2 | ✅ |
| 検証対象（ブラウザ側の検証） | 0 | 1 | ✅ |
| 双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値） | 0 | 1 | ✅ |
| 項目（ListItem の Selected・Enabled・Text・Value と ViewState） | 20 | 2 | ✅ |
| 複数選択 | 5 | 2 | ✅ |
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
| <code>ListBox.Api.Control.#ctor</code> | ✅ 一致 |
| <code>ListBox.Api.Control.Adapter</code> | ✅ 一致 |
| <code>ListBox.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>ListBox.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>ListBox.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>ListBox.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>ListBox.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>ListBox.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>ListBox.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>ListBox.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>ListBox.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>ListBox.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>ListBox.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>ListBox.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>ListBox.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>ListBox.Api.Control.ClientID</code> | ✅ 一致 |
| <code>ListBox.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>ListBox.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>ListBox.Api.Control.Context</code> | ✅ 一致 |
| <code>ListBox.Api.Control.Controls</code> | ✅ 一致 |
| <code>ListBox.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>ListBox.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>ListBox.Api.Control.DataBind</code> | ✅ 一致 |
| <code>ListBox.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>ListBox.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>ListBox.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>ListBox.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>ListBox.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>ListBox.Api.Control.Dispose</code> | ✅ 一致 |
| <code>ListBox.Api.Control.Disposed</code> | ✅ 一致 |
| <code>ListBox.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>ListBox.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>ListBox.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>ListBox.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>ListBox.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>ListBox.Api.Control.Events</code> | ✅ 一致 |
| <code>ListBox.Api.Control.FindControl</code> | ✅ 一致 |
| <code>ListBox.Api.Control.Focus</code> | ✅ 一致 |
| <code>ListBox.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>ListBox.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>ListBox.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>ListBox.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>ListBox.Api.Control.HasControls</code> | ✅ 一致 |
| <code>ListBox.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>ListBox.Api.Control.ID</code> | ✅ 一致 |
| <code>ListBox.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>ListBox.Api.Control.Init</code> | ✅ 一致 |
| <code>ListBox.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>ListBox.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>ListBox.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>ListBox.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>ListBox.Api.Control.Load</code> | ✅ 一致 |
| <code>ListBox.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>ListBox.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>ListBox.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>ListBox.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>ListBox.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>ListBox.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>ListBox.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>ListBox.Api.Control.OnInit</code> | ✅ 一致 |
| <code>ListBox.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>ListBox.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>ListBox.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>ListBox.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>ListBox.Api.Control.Page</code> | ✅ 一致 |
| <code>ListBox.Api.Control.Parent</code> | ✅ 一致 |
| <code>ListBox.Api.Control.PreRender</code> | ✅ 一致 |
| <code>ListBox.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>ListBox.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>ListBox.Api.Control.Render</code> | ✅ 一致 |
| <code>ListBox.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>ListBox.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>ListBox.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>ListBox.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>ListBox.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>ListBox.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>ListBox.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>ListBox.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>ListBox.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>ListBox.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>ListBox.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>ListBox.Api.Control.Site</code> | ✅ 一致 |
| <code>ListBox.Api.Control.SkinID</code> | ✅ 一致 |
| <code>ListBox.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>ListBox.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>ListBox.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>ListBox.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>ListBox.Api.Control.Unload</code> | ✅ 一致 |
| <code>ListBox.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>ListBox.Api.Control.ViewState</code> | ✅ 一致 |
| <code>ListBox.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>ListBox.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>ListBox.Api.Control.Visible</code> | ✅ 一致 |
| <code>ListBox.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>ListBox.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>ListBox.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>ListBox.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>ListBox.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>ListBox.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>ListBox.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>ListBox.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>ListBox.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>ListBox.Api.IEditableTextControl.TextChanged</code> | ✅ 一致 |
| <code>ListBox.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>ListBox.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>ListBox.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>ListBox.Api.IPostBackDataHandler.LoadPostData</code> | ✅ 一致 |
| <code>ListBox.Api.IPostBackDataHandler.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>ListBox.Api.ITextControl.Text</code> | ✅ 一致 |
| <code>ListBox.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.BaseDataBoundControl.#ctor</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.BaseDataBoundControl.ConfirmInitState</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.BaseDataBoundControl.DataBind</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.BaseDataBoundControl.DataBound</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.BaseDataBoundControl.DataSource</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.BaseDataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.BaseDataBoundControl.EnsureDataBound</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.BaseDataBoundControl.Initialized</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.BaseDataBoundControl.IsBoundUsingDataSourceID</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.BaseDataBoundControl.IsDataBindingAutomatic</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.BaseDataBoundControl.IsUsingModelBinders</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.BaseDataBoundControl.OnDataBound</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.BaseDataBoundControl.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.BaseDataBoundControl.OnInit</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.BaseDataBoundControl.OnPagePreLoad</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.BaseDataBoundControl.OnPreRender</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.BaseDataBoundControl.PerformSelect</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.BaseDataBoundControl.RequiresDataBinding</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.BaseDataBoundControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.BaseDataBoundControl.ValidateDataSource</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.DataBoundControl.#ctor</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.DataBoundControl.CallingDataMethods</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.DataBoundControl.CreateDataSourceSelectArguments</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.DataBoundControl.CreatingModelDataSource</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.DataBoundControl.DataMember</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.DataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.DataBoundControl.DataSourceObject</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.DataBoundControl.GetData</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.DataBoundControl.GetDataSource</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.DataBoundControl.IsUsingModelBinders</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.DataBoundControl.ItemType</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.DataBoundControl.LoadViewState</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.DataBoundControl.MarkAsDataBound</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.DataBoundControl.OnCreatingModelDataSource</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.DataBoundControl.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.DataBoundControl.OnDataSourceViewChanged</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.DataBoundControl.OnLoad</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.DataBoundControl.OnPagePreLoad</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.DataBoundControl.PerformDataBinding</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.DataBoundControl.PerformSelect</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.DataBoundControl.SaveViewState</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.DataBoundControl.SelectArguments</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.DataBoundControl.SelectMethod</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.DataBoundControl.TrackViewState</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.DataBoundControl.ValidateDataSource</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListBox.#ctor</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListBox.AddAttributesToRender</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListBox.BorderColor</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListBox.BorderStyle</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListBox.BorderWidth</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListBox.GetSelectedIndices</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListBox.LoadPostData</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListBox.OnPreRender</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListBox.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListBox.Rows</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListBox.SelectionMode</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.#ctor</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.AppendDataBoundItems</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.AutoPostBack</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.CausesValidation</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.ClearSelection</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.DataTextField</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.DataTextFormatString</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.DataValueField</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.Items</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.LoadViewState</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.OnDataBinding</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.OnPreRender</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.OnSelectedIndexChanged</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.OnTextChanged</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.PerformDataBinding</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.PerformSelect</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.RenderContents</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.SaveViewState</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.SelectedIndex</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.SelectedIndexChanged</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.SelectedItem</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.SelectedValue</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.SetPostDataSelection</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.TagKey</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.Text</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.TextChanged</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.TrackViewState</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.ValidationGroup</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.ListControl.VerifyMultiSelect</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>ListBox.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>ListBox.(case list)</code> | ✅ 一致 |
| <code>ListBox.(default)</code> | ✅ 一致 |
| <code>ListBox.AccessKey=empty</code> | ✅ 一致 |
| <code>ListBox.AccessKey=japanese</code> | ✅ 一致 |
| <code>ListBox.AccessKey=k</code> | ✅ 一致 |
| <code>ListBox.AccessKey=markup</code> | ✅ 一致 |
| <code>ListBox.AccessKey=text</code> | ✅ 一致 |
| <code>ListBox.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>ListBox.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>ListBox.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>ListBox.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>ListBox.AppendDataBoundItems=false</code> | ✅ 一致 |
| <code>ListBox.AppendDataBoundItems=true</code> | ✅ 一致 |
| <code>ListBox.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>ListBox.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>ListBox.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>ListBox.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>ListBox.AutoPostBack=false</code> | ✅ 一致 |
| <code>ListBox.AutoPostBack=true</code> | ✅ 一致 |
| <code>ListBox.AutoPostBack=true+CausesValidation=true</code> | ✅ 一致 |
| <code>ListBox.AutoPostBack=true+CausesValidation=true+ValidationGroup=text</code> | ✅ 一致 |
| <code>ListBox.AutoPostBack=true+Enabled=false</code> | ✅ 一致 |
| <code>ListBox.AutoPostBack=true+Items[1].Enabled=false</code> | ✅ 一致 |
| <code>ListBox.BackColor=#3366CC</code> | ✅ 一致 |
| <code>ListBox.BackColor=Red</code> | ✅ 一致 |
| <code>ListBox.BackColor=Transparent</code> | ✅ 一致 |
| <code>ListBox.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>ListBox.BorderColor=Red</code> | ✅ 一致 |
| <code>ListBox.BorderColor=Transparent</code> | ✅ 一致 |
| <code>ListBox.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>ListBox.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>ListBox.BorderStyle=Double</code> | ✅ 一致 |
| <code>ListBox.BorderStyle=Groove</code> | ✅ 一致 |
| <code>ListBox.BorderStyle=Inset</code> | ✅ 一致 |
| <code>ListBox.BorderStyle=None</code> | ✅ 一致 |
| <code>ListBox.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>ListBox.BorderStyle=Outset</code> | ✅ 一致 |
| <code>ListBox.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>ListBox.BorderStyle=Solid</code> | ✅ 一致 |
| <code>ListBox.BorderWidth=120px</code> | ✅ 一致 |
| <code>ListBox.BorderWidth=2em</code> | ✅ 一致 |
| <code>ListBox.BorderWidth=50%</code> | ✅ 一致 |
| <code>ListBox.CausesValidation=false</code> | ✅ 一致 |
| <code>ListBox.CausesValidation=true</code> | ✅ 一致 |
| <code>ListBox.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ListBox.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ListBox.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ListBox.ClientIDMode=Static</code> | ✅ 一致 |
| <code>ListBox.CssClass=empty</code> | ✅ 一致 |
| <code>ListBox.CssClass=japanese</code> | ✅ 一致 |
| <code>ListBox.CssClass=markup</code> | ✅ 一致 |
| <code>ListBox.CssClass=text</code> | ✅ 一致 |
| <code>ListBox.DataMember=empty</code> | ✅ 一致 |
| <code>ListBox.DataMember=japanese</code> | ✅ 一致 |
| <code>ListBox.DataMember=markup</code> | ✅ 一致 |
| <code>ListBox.DataMember=text</code> | ✅ 一致 |
| <code>ListBox.DataSourceID=empty</code> | ✅ 一致 |
| <code>ListBox.DataSourceID=japanese</code> | ✅ 一致 |
| <code>ListBox.DataSourceID=markup</code> | ✅ 一致 |
| <code>ListBox.DataSourceID=text</code> | ✅ 一致 |
| <code>ListBox.DataTextField=Name</code> | ✅ 一致 |
| <code>ListBox.DataTextField=Price</code> | ✅ 一致 |
| <code>ListBox.DataTextField=empty</code> | ✅ 一致 |
| <code>ListBox.DataTextField=japanese</code> | ✅ 一致 |
| <code>ListBox.DataTextField=markup</code> | ✅ 一致 |
| <code>ListBox.DataTextField=text</code> | ✅ 一致 |
| <code>ListBox.DataTextFormatString=N2</code> | ✅ 一致 |
| <code>ListBox.DataTextFormatString=empty</code> | ✅ 一致 |
| <code>ListBox.DataTextFormatString=japanese</code> | ✅ 一致 |
| <code>ListBox.DataTextFormatString=markup</code> | ✅ 一致 |
| <code>ListBox.DataTextFormatString=text</code> | ✅ 一致 |
| <code>ListBox.DataValueField=Code</code> | ✅ 一致 |
| <code>ListBox.DataValueField=empty</code> | ✅ 一致 |
| <code>ListBox.DataValueField=japanese</code> | ✅ 一致 |
| <code>ListBox.DataValueField=markup</code> | ✅ 一致 |
| <code>ListBox.DataValueField=text</code> | ✅ 一致 |
| <code>ListBox.EnableTheming=false</code> | ✅ 一致 |
| <code>ListBox.EnableTheming=true</code> | ✅ 一致 |
| <code>ListBox.EnableViewState=false</code> | ✅ 一致 |
| <code>ListBox.EnableViewState=true</code> | ✅ 一致 |
| <code>ListBox.Enabled=false</code> | ✅ 一致 |
| <code>ListBox.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>ListBox.Enabled=true</code> | ✅ 一致 |
| <code>ListBox.Font.Bold=false</code> | ✅ 一致 |
| <code>ListBox.Font.Bold=true</code> | ✅ 一致 |
| <code>ListBox.Font.Italic=false</code> | ✅ 一致 |
| <code>ListBox.Font.Italic=true</code> | ✅ 一致 |
| <code>ListBox.Font.Name=empty</code> | ✅ 一致 |
| <code>ListBox.Font.Name=japanese</code> | ✅ 一致 |
| <code>ListBox.Font.Name=markup</code> | ✅ 一致 |
| <code>ListBox.Font.Name=text</code> | ✅ 一致 |
| <code>ListBox.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>ListBox.Font.Overline=false</code> | ✅ 一致 |
| <code>ListBox.Font.Overline=true</code> | ✅ 一致 |
| <code>ListBox.Font.Size=12pt</code> | ✅ 一致 |
| <code>ListBox.Font.Size=150%</code> | ✅ 一致 |
| <code>ListBox.Font.Size=Large</code> | ✅ 一致 |
| <code>ListBox.Font.Strikeout=false</code> | ✅ 一致 |
| <code>ListBox.Font.Strikeout=true</code> | ✅ 一致 |
| <code>ListBox.Font.Underline=false</code> | ✅ 一致 |
| <code>ListBox.Font.Underline=true</code> | ✅ 一致 |
| <code>ListBox.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>ListBox.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>ListBox.ForeColor=Red</code> | ✅ 一致 |
| <code>ListBox.ForeColor=Transparent</code> | ✅ 一致 |
| <code>ListBox.Height=120px</code> | ✅ 一致 |
| <code>ListBox.Height=2em</code> | ✅ 一致 |
| <code>ListBox.Height=50%</code> | ✅ 一致 |
| <code>ListBox.ID=empty</code> | ✅ 一致 |
| <code>ListBox.ID=japanese</code> | ✅ 一致 |
| <code>ListBox.ID=markup</code> | ✅ 一致 |
| <code>ListBox.ID=text</code> | ✅ 一致 |
| <code>ListBox.ItemType=empty</code> | ✅ 一致 |
| <code>ListBox.ItemType=japanese</code> | ✅ 一致 |
| <code>ListBox.ItemType=markup</code> | ✅ 一致 |
| <code>ListBox.ItemType=text</code> | ✅ 一致 |
| <code>ListBox.Items[1].Enabled=false</code> | ✅ 一致 |
| <code>ListBox.Items[1].Enabled=true</code> | ✅ 一致 |
| <code>ListBox.Items[1].Selected=false</code> | ✅ 一致 |
| <code>ListBox.Items[1].Selected=true</code> | ✅ 一致 |
| <code>ListBox.Items[1].Selected=true+Enabled=false</code> | ✅ 一致 |
| <code>ListBox.Items[1].Text=empty</code> | ✅ 一致 |
| <code>ListBox.Items[1].Text=empty+Items[1].Value=text</code> | ✅ 一致 |
| <code>ListBox.Items[1].Text=japanese</code> | ✅ 一致 |
| <code>ListBox.Items[1].Text=markup</code> | ✅ 一致 |
| <code>ListBox.Items[1].Text=markup+Items[1].Value=markup</code> | ✅ 一致 |
| <code>ListBox.Items[1].Text=spaces</code> | ✅ 一致 |
| <code>ListBox.Items[1].Text=text</code> | ✅ 一致 |
| <code>ListBox.Items[1].Value=empty</code> | ✅ 一致 |
| <code>ListBox.Items[1].Value=japanese</code> | ✅ 一致 |
| <code>ListBox.Items[1].Value=markup</code> | ✅ 一致 |
| <code>ListBox.Items[1].Value=text</code> | ✅ 一致 |
| <code>ListBox.Rows=-1</code> | ✅ 一致 |
| <code>ListBox.Rows=0</code> | ✅ 一致 |
| <code>ListBox.Rows=3</code> | ✅ 一致 |
| <code>ListBox.SelectMethod=empty</code> | ✅ 一致 |
| <code>ListBox.SelectMethod=japanese</code> | ✅ 一致 |
| <code>ListBox.SelectMethod=markup</code> | ✅ 一致 |
| <code>ListBox.SelectMethod=text</code> | ✅ 一致 |
| <code>ListBox.SelectedIndex=-1</code> | ✅ 一致 |
| <code>ListBox.SelectedIndex=0</code> | ✅ 一致 |
| <code>ListBox.SelectedIndex=1</code> | ✅ 一致 |
| <code>ListBox.SelectedIndex=1+EnableViewState=false</code> | ✅ 一致 |
| <code>ListBox.SelectedIndex=1+Items[1].Enabled=false</code> | ✅ 一致 |
| <code>ListBox.SelectedIndex=1+Visible=false</code> | ✅ 一致 |
| <code>ListBox.SelectedIndex=3</code> | ✅ 一致 |
| <code>ListBox.SelectedValue=B2</code> | ✅ 一致 |
| <code>ListBox.SelectedValue=M</code> | ✅ 一致 |
| <code>ListBox.SelectedValue=M+SelectedIndex=0</code> | ✅ 一致 |
| <code>ListBox.SelectedValue=empty</code> | ✅ 一致 |
| <code>ListBox.SelectedValue=japanese</code> | ✅ 一致 |
| <code>ListBox.SelectedValue=markup</code> | ✅ 一致 |
| <code>ListBox.SelectedValue=text</code> | ✅ 一致 |
| <code>ListBox.SelectionMode=Multiple</code> | ✅ 一致 |
| <code>ListBox.SelectionMode=Multiple+AutoPostBack=true</code> | ✅ 一致 |
| <code>ListBox.SelectionMode=Multiple+Items[0].Selected=true+Items[2].Selected=true</code> | ✅ 一致 |
| <code>ListBox.SelectionMode=Multiple+Rows=3</code> | ✅ 一致 |
| <code>ListBox.SelectionMode=Single</code> | ✅ 一致 |
| <code>ListBox.SelectionMode=Single+Items[0].Selected=true+Items[2].Selected=true</code> | ✅ 一致 |
| <code>ListBox.SkinID=empty</code> | ✅ 一致 |
| <code>ListBox.SkinID=japanese</code> | ✅ 一致 |
| <code>ListBox.SkinID=markup</code> | ✅ 一致 |
| <code>ListBox.SkinID=text</code> | ✅ 一致 |
| <code>ListBox.Style[color]=1em</code> | ✅ 一致 |
| <code>ListBox.Style[color]=red</code> | ✅ 一致 |
| <code>ListBox.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>ListBox.Style[margin-left]=red</code> | ✅ 一致 |
| <code>ListBox.TabIndex=-1</code> | ✅ 一致 |
| <code>ListBox.TabIndex=0</code> | ✅ 一致 |
| <code>ListBox.TabIndex=3</code> | ✅ 一致 |
| <code>ListBox.Text=empty</code> | ✅ 一致 |
| <code>ListBox.Text=japanese</code> | ✅ 一致 |
| <code>ListBox.Text=markup</code> | ✅ 一致 |
| <code>ListBox.Text=text</code> | ✅ 一致 |
| <code>ListBox.ToolTip=empty</code> | ✅ 一致 |
| <code>ListBox.ToolTip=japanese</code> | ✅ 一致 |
| <code>ListBox.ToolTip=markup</code> | ✅ 一致 |
| <code>ListBox.ToolTip=text</code> | ✅ 一致 |
| <code>ListBox.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>ListBox.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>ListBox.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>ListBox.ValidationGroup=empty</code> | ✅ 一致 |
| <code>ListBox.ValidationGroup=japanese</code> | ✅ 一致 |
| <code>ListBox.ValidationGroup=markup</code> | ✅ 一致 |
| <code>ListBox.ValidationGroup=text</code> | ✅ 一致 |
| <code>ListBox.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>ListBox.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>ListBox.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>ListBox.Visible=false</code> | ✅ 一致 |
| <code>ListBox.Visible=true</code> | ✅ 一致 |
| <code>ListBox.Width=120px</code> | ✅ 一致 |
| <code>ListBox.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>ListBox.Width=2em</code> | ✅ 一致 |
| <code>ListBox.Width=50%</code> | ✅ 一致 |
| <code>ListBox[data-bound].(default)</code> | ✅ 一致 |
| <code>ListBox[data-bound].DataTextField=Name</code> | ✅ 一致 |
| <code>ListBox[data-bound].DataTextField=Name+AppendDataBoundItems=true</code> | ✅ 一致 |
| <code>ListBox[data-bound].DataTextField=Name+DataValueField=Code</code> | ✅ 一致 |
| <code>ListBox[data-bound].DataTextField=Name+DataValueField=Code+SelectedIndex=1</code> | ✅ 一致 |
| <code>ListBox[data-bound].DataTextField=Name+DataValueField=Code+SelectedValue=B2</code> | ✅ 一致 |
| <code>ListBox[data-bound].DataTextField=Name+DataValueField=Code+SelectedValue=M</code> | ✅ 一致 |
| <code>ListBox[data-bound].DataTextField=Price+DataTextFormatString=N2</code> | ✅ 一致 |
| <code>ListBox[data-bound].DataTextField=text</code> | ✅ 一致 |
| <code>ListBox[data-bound].DataValueField=Code</code> | ✅ 一致 |
| <code>ListBox[naming container].(default)</code> | ✅ 一致 |
| <code>ListBox[naming container].AutoPostBack=true</code> | ✅ 一致 |
| <code>ListBox[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ListBox[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ListBox[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ListBox[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>ListBox[naming container].ID=text</code> | ✅ 一致 |
| <code>ListBox[themed].(default)</code> | ✅ 一致 |
| <code>ListBox[themed].CssClass=text</code> | ✅ 一致 |
| <code>ListBox[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>ListBox[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| ListBox [render][databind][items] first request | ✅ 一致 |
| ListBox [postdata][change][multiselect][dynamic] several selected | ✅ 一致 |
| ListBox [change][multiselect] selection changed to one | ✅ 一致 |
| ListBox [viewstate] posted again unchanged | ✅ 一致 |
| ListBox [autopostback][change] auto postback | ✅ 一致 |
| ListBox [validation] group g1, nothing chosen | ✅ 一致 |
| ListBox [validation] group g1, chosen | ✅ 一致 |
| ListBox [clientvalidation] group g3 in the browser: no request | ✅ 一致 |
| ListBox [visible][viewstate][items] hidden lists shown with the items added in code | ✅ 一致 |
| ListBox [bind] two-way binding in a template | ✅ 一致 |
| ListBox [eventvalidation] a value the list does not have posted | ✅ 一致 |
| ListBox [requestvalidation] markup posted | ✅ 一致 |

</details>
