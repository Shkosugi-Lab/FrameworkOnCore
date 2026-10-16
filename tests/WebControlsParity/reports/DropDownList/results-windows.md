# DropDownList パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-11 23:33
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 234 | 234 | 0 | 0 |
| ② HTML | 208 | 208 | 0 | 0 |
| ③ シナリオ | 11 | 11 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 243 件中 243 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 43 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 11 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 168 | 1 | ✅ |
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
| AutoPostBack（クライアントスクリプトによるポストバック） | 6 | 1 | ✅ |
| CausesValidation / ValidationGroup | 8 | 0 | ✅ |
| 検証対象（サーバー側の検証） | 0 | 2 | ✅ |
| 検証対象（ブラウザ側の検証） | 0 | 1 | ✅ |
| 双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値） | 0 | 1 | ✅ |
| 項目（ListItem の Selected・Enabled・Text・Value と ViewState） | 20 | 2 | ✅ |
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
| <code>DropDownList.Api.Control.#ctor</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.Adapter</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.ClientID</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.Context</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.Controls</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.DataBind</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.Dispose</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.Disposed</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.Events</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.FindControl</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.Focus</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.HasControls</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.ID</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.Init</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.Load</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.OnInit</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.Page</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.Parent</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.PreRender</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.Render</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.Site</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.SkinID</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.Unload</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.ViewState</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>DropDownList.Api.Control.Visible</code> | ✅ 一致 |
| <code>DropDownList.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>DropDownList.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>DropDownList.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>DropDownList.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>DropDownList.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>DropDownList.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>DropDownList.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>DropDownList.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>DropDownList.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>DropDownList.Api.IEditableTextControl.TextChanged</code> | ✅ 一致 |
| <code>DropDownList.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>DropDownList.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>DropDownList.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>DropDownList.Api.IPostBackDataHandler.LoadPostData</code> | ✅ 一致 |
| <code>DropDownList.Api.IPostBackDataHandler.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>DropDownList.Api.ITextControl.Text</code> | ✅ 一致 |
| <code>DropDownList.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.BaseDataBoundControl.#ctor</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.BaseDataBoundControl.ConfirmInitState</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.BaseDataBoundControl.DataBind</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.BaseDataBoundControl.DataBound</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.BaseDataBoundControl.DataSource</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.BaseDataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.BaseDataBoundControl.EnsureDataBound</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.BaseDataBoundControl.Initialized</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.BaseDataBoundControl.IsBoundUsingDataSourceID</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.BaseDataBoundControl.IsDataBindingAutomatic</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.BaseDataBoundControl.IsUsingModelBinders</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.BaseDataBoundControl.OnDataBound</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.BaseDataBoundControl.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.BaseDataBoundControl.OnInit</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.BaseDataBoundControl.OnPagePreLoad</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.BaseDataBoundControl.OnPreRender</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.BaseDataBoundControl.PerformSelect</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.BaseDataBoundControl.RequiresDataBinding</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.BaseDataBoundControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.BaseDataBoundControl.ValidateDataSource</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DataBoundControl.#ctor</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DataBoundControl.CallingDataMethods</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DataBoundControl.CreateDataSourceSelectArguments</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DataBoundControl.CreatingModelDataSource</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DataBoundControl.DataMember</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DataBoundControl.DataSourceObject</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DataBoundControl.GetData</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DataBoundControl.GetDataSource</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DataBoundControl.IsUsingModelBinders</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DataBoundControl.ItemType</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DataBoundControl.LoadViewState</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DataBoundControl.MarkAsDataBound</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DataBoundControl.OnCreatingModelDataSource</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DataBoundControl.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DataBoundControl.OnDataSourceViewChanged</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DataBoundControl.OnLoad</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DataBoundControl.OnPagePreLoad</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DataBoundControl.PerformDataBinding</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DataBoundControl.PerformSelect</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DataBoundControl.SaveViewState</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DataBoundControl.SelectArguments</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DataBoundControl.SelectMethod</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DataBoundControl.TrackViewState</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DataBoundControl.ValidateDataSource</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DropDownList.#ctor</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DropDownList.AddAttributesToRender</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DropDownList.BorderColor</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DropDownList.BorderStyle</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DropDownList.BorderWidth</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DropDownList.CreateControlCollection</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DropDownList.LoadPostData</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DropDownList.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DropDownList.SelectedIndex</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DropDownList.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.DropDownList.VerifyMultiSelect</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.#ctor</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.AppendDataBoundItems</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.AutoPostBack</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.CausesValidation</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.ClearSelection</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.DataTextField</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.DataTextFormatString</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.DataValueField</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.Items</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.LoadViewState</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.OnDataBinding</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.OnPreRender</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.OnSelectedIndexChanged</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.OnTextChanged</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.PerformDataBinding</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.PerformSelect</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.RenderContents</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.SaveViewState</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.SelectedIndex</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.SelectedIndexChanged</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.SelectedItem</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.SelectedValue</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.SetPostDataSelection</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.TagKey</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.Text</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.TextChanged</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.TrackViewState</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.ValidationGroup</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.ListControl.VerifyMultiSelect</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>DropDownList.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>DropDownList.(case list)</code> | ✅ 一致 |
| <code>DropDownList.(default)</code> | ✅ 一致 |
| <code>DropDownList.AccessKey=empty</code> | ✅ 一致 |
| <code>DropDownList.AccessKey=japanese</code> | ✅ 一致 |
| <code>DropDownList.AccessKey=k</code> | ✅ 一致 |
| <code>DropDownList.AccessKey=markup</code> | ✅ 一致 |
| <code>DropDownList.AccessKey=text</code> | ✅ 一致 |
| <code>DropDownList.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>DropDownList.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>DropDownList.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>DropDownList.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>DropDownList.AppendDataBoundItems=false</code> | ✅ 一致 |
| <code>DropDownList.AppendDataBoundItems=true</code> | ✅ 一致 |
| <code>DropDownList.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>DropDownList.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>DropDownList.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>DropDownList.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>DropDownList.AutoPostBack=false</code> | ✅ 一致 |
| <code>DropDownList.AutoPostBack=true</code> | ✅ 一致 |
| <code>DropDownList.AutoPostBack=true+CausesValidation=true</code> | ✅ 一致 |
| <code>DropDownList.AutoPostBack=true+CausesValidation=true+ValidationGroup=text</code> | ✅ 一致 |
| <code>DropDownList.AutoPostBack=true+Enabled=false</code> | ✅ 一致 |
| <code>DropDownList.AutoPostBack=true+Items[1].Enabled=false</code> | ✅ 一致 |
| <code>DropDownList.BackColor=#3366CC</code> | ✅ 一致 |
| <code>DropDownList.BackColor=Red</code> | ✅ 一致 |
| <code>DropDownList.BackColor=Transparent</code> | ✅ 一致 |
| <code>DropDownList.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>DropDownList.BorderColor=Red</code> | ✅ 一致 |
| <code>DropDownList.BorderColor=Transparent</code> | ✅ 一致 |
| <code>DropDownList.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>DropDownList.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>DropDownList.BorderStyle=Double</code> | ✅ 一致 |
| <code>DropDownList.BorderStyle=Groove</code> | ✅ 一致 |
| <code>DropDownList.BorderStyle=Inset</code> | ✅ 一致 |
| <code>DropDownList.BorderStyle=None</code> | ✅ 一致 |
| <code>DropDownList.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>DropDownList.BorderStyle=Outset</code> | ✅ 一致 |
| <code>DropDownList.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>DropDownList.BorderStyle=Solid</code> | ✅ 一致 |
| <code>DropDownList.BorderWidth=120px</code> | ✅ 一致 |
| <code>DropDownList.BorderWidth=2em</code> | ✅ 一致 |
| <code>DropDownList.BorderWidth=50%</code> | ✅ 一致 |
| <code>DropDownList.CausesValidation=false</code> | ✅ 一致 |
| <code>DropDownList.CausesValidation=true</code> | ✅ 一致 |
| <code>DropDownList.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>DropDownList.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>DropDownList.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>DropDownList.ClientIDMode=Static</code> | ✅ 一致 |
| <code>DropDownList.CssClass=empty</code> | ✅ 一致 |
| <code>DropDownList.CssClass=japanese</code> | ✅ 一致 |
| <code>DropDownList.CssClass=markup</code> | ✅ 一致 |
| <code>DropDownList.CssClass=text</code> | ✅ 一致 |
| <code>DropDownList.DataMember=empty</code> | ✅ 一致 |
| <code>DropDownList.DataMember=japanese</code> | ✅ 一致 |
| <code>DropDownList.DataMember=markup</code> | ✅ 一致 |
| <code>DropDownList.DataMember=text</code> | ✅ 一致 |
| <code>DropDownList.DataSourceID=empty</code> | ✅ 一致 |
| <code>DropDownList.DataSourceID=japanese</code> | ✅ 一致 |
| <code>DropDownList.DataSourceID=markup</code> | ✅ 一致 |
| <code>DropDownList.DataSourceID=text</code> | ✅ 一致 |
| <code>DropDownList.DataTextField=Name</code> | ✅ 一致 |
| <code>DropDownList.DataTextField=Price</code> | ✅ 一致 |
| <code>DropDownList.DataTextField=empty</code> | ✅ 一致 |
| <code>DropDownList.DataTextField=japanese</code> | ✅ 一致 |
| <code>DropDownList.DataTextField=markup</code> | ✅ 一致 |
| <code>DropDownList.DataTextField=text</code> | ✅ 一致 |
| <code>DropDownList.DataTextFormatString=N2</code> | ✅ 一致 |
| <code>DropDownList.DataTextFormatString=empty</code> | ✅ 一致 |
| <code>DropDownList.DataTextFormatString=japanese</code> | ✅ 一致 |
| <code>DropDownList.DataTextFormatString=markup</code> | ✅ 一致 |
| <code>DropDownList.DataTextFormatString=text</code> | ✅ 一致 |
| <code>DropDownList.DataValueField=Code</code> | ✅ 一致 |
| <code>DropDownList.DataValueField=empty</code> | ✅ 一致 |
| <code>DropDownList.DataValueField=japanese</code> | ✅ 一致 |
| <code>DropDownList.DataValueField=markup</code> | ✅ 一致 |
| <code>DropDownList.DataValueField=text</code> | ✅ 一致 |
| <code>DropDownList.EnableTheming=false</code> | ✅ 一致 |
| <code>DropDownList.EnableTheming=true</code> | ✅ 一致 |
| <code>DropDownList.EnableViewState=false</code> | ✅ 一致 |
| <code>DropDownList.EnableViewState=true</code> | ✅ 一致 |
| <code>DropDownList.Enabled=false</code> | ✅ 一致 |
| <code>DropDownList.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>DropDownList.Enabled=true</code> | ✅ 一致 |
| <code>DropDownList.Font.Bold=false</code> | ✅ 一致 |
| <code>DropDownList.Font.Bold=true</code> | ✅ 一致 |
| <code>DropDownList.Font.Italic=false</code> | ✅ 一致 |
| <code>DropDownList.Font.Italic=true</code> | ✅ 一致 |
| <code>DropDownList.Font.Name=empty</code> | ✅ 一致 |
| <code>DropDownList.Font.Name=japanese</code> | ✅ 一致 |
| <code>DropDownList.Font.Name=markup</code> | ✅ 一致 |
| <code>DropDownList.Font.Name=text</code> | ✅ 一致 |
| <code>DropDownList.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>DropDownList.Font.Overline=false</code> | ✅ 一致 |
| <code>DropDownList.Font.Overline=true</code> | ✅ 一致 |
| <code>DropDownList.Font.Size=12pt</code> | ✅ 一致 |
| <code>DropDownList.Font.Size=150%</code> | ✅ 一致 |
| <code>DropDownList.Font.Size=Large</code> | ✅ 一致 |
| <code>DropDownList.Font.Strikeout=false</code> | ✅ 一致 |
| <code>DropDownList.Font.Strikeout=true</code> | ✅ 一致 |
| <code>DropDownList.Font.Underline=false</code> | ✅ 一致 |
| <code>DropDownList.Font.Underline=true</code> | ✅ 一致 |
| <code>DropDownList.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>DropDownList.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>DropDownList.ForeColor=Red</code> | ✅ 一致 |
| <code>DropDownList.ForeColor=Transparent</code> | ✅ 一致 |
| <code>DropDownList.Height=120px</code> | ✅ 一致 |
| <code>DropDownList.Height=2em</code> | ✅ 一致 |
| <code>DropDownList.Height=50%</code> | ✅ 一致 |
| <code>DropDownList.ID=empty</code> | ✅ 一致 |
| <code>DropDownList.ID=japanese</code> | ✅ 一致 |
| <code>DropDownList.ID=markup</code> | ✅ 一致 |
| <code>DropDownList.ID=text</code> | ✅ 一致 |
| <code>DropDownList.ItemType=empty</code> | ✅ 一致 |
| <code>DropDownList.ItemType=japanese</code> | ✅ 一致 |
| <code>DropDownList.ItemType=markup</code> | ✅ 一致 |
| <code>DropDownList.ItemType=text</code> | ✅ 一致 |
| <code>DropDownList.Items[0].Selected=true+Items[2].Selected=true</code> | ✅ 一致 |
| <code>DropDownList.Items[1].Enabled=false</code> | ✅ 一致 |
| <code>DropDownList.Items[1].Enabled=true</code> | ✅ 一致 |
| <code>DropDownList.Items[1].Selected=false</code> | ✅ 一致 |
| <code>DropDownList.Items[1].Selected=true</code> | ✅ 一致 |
| <code>DropDownList.Items[1].Selected=true+Enabled=false</code> | ✅ 一致 |
| <code>DropDownList.Items[1].Text=empty</code> | ✅ 一致 |
| <code>DropDownList.Items[1].Text=empty+Items[1].Value=text</code> | ✅ 一致 |
| <code>DropDownList.Items[1].Text=japanese</code> | ✅ 一致 |
| <code>DropDownList.Items[1].Text=markup</code> | ✅ 一致 |
| <code>DropDownList.Items[1].Text=markup+Items[1].Value=markup</code> | ✅ 一致 |
| <code>DropDownList.Items[1].Text=spaces</code> | ✅ 一致 |
| <code>DropDownList.Items[1].Text=text</code> | ✅ 一致 |
| <code>DropDownList.Items[1].Value=empty</code> | ✅ 一致 |
| <code>DropDownList.Items[1].Value=japanese</code> | ✅ 一致 |
| <code>DropDownList.Items[1].Value=markup</code> | ✅ 一致 |
| <code>DropDownList.Items[1].Value=text</code> | ✅ 一致 |
| <code>DropDownList.SelectMethod=empty</code> | ✅ 一致 |
| <code>DropDownList.SelectMethod=japanese</code> | ✅ 一致 |
| <code>DropDownList.SelectMethod=markup</code> | ✅ 一致 |
| <code>DropDownList.SelectMethod=text</code> | ✅ 一致 |
| <code>DropDownList.SelectedIndex=-1</code> | ✅ 一致 |
| <code>DropDownList.SelectedIndex=0</code> | ✅ 一致 |
| <code>DropDownList.SelectedIndex=1</code> | ✅ 一致 |
| <code>DropDownList.SelectedIndex=1+EnableViewState=false</code> | ✅ 一致 |
| <code>DropDownList.SelectedIndex=1+Items[1].Enabled=false</code> | ✅ 一致 |
| <code>DropDownList.SelectedIndex=1+Items[2].Selected=true</code> | ✅ 一致 |
| <code>DropDownList.SelectedIndex=1+Visible=false</code> | ✅ 一致 |
| <code>DropDownList.SelectedIndex=3</code> | ✅ 一致 |
| <code>DropDownList.SelectedValue=B2</code> | ✅ 一致 |
| <code>DropDownList.SelectedValue=M</code> | ✅ 一致 |
| <code>DropDownList.SelectedValue=M+SelectedIndex=0</code> | ✅ 一致 |
| <code>DropDownList.SelectedValue=empty</code> | ✅ 一致 |
| <code>DropDownList.SelectedValue=japanese</code> | ✅ 一致 |
| <code>DropDownList.SelectedValue=markup</code> | ✅ 一致 |
| <code>DropDownList.SelectedValue=text</code> | ✅ 一致 |
| <code>DropDownList.SkinID=empty</code> | ✅ 一致 |
| <code>DropDownList.SkinID=japanese</code> | ✅ 一致 |
| <code>DropDownList.SkinID=markup</code> | ✅ 一致 |
| <code>DropDownList.SkinID=text</code> | ✅ 一致 |
| <code>DropDownList.Style[color]=1em</code> | ✅ 一致 |
| <code>DropDownList.Style[color]=red</code> | ✅ 一致 |
| <code>DropDownList.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>DropDownList.Style[margin-left]=red</code> | ✅ 一致 |
| <code>DropDownList.TabIndex=-1</code> | ✅ 一致 |
| <code>DropDownList.TabIndex=0</code> | ✅ 一致 |
| <code>DropDownList.TabIndex=3</code> | ✅ 一致 |
| <code>DropDownList.Text=empty</code> | ✅ 一致 |
| <code>DropDownList.Text=japanese</code> | ✅ 一致 |
| <code>DropDownList.Text=markup</code> | ✅ 一致 |
| <code>DropDownList.Text=text</code> | ✅ 一致 |
| <code>DropDownList.ToolTip=empty</code> | ✅ 一致 |
| <code>DropDownList.ToolTip=japanese</code> | ✅ 一致 |
| <code>DropDownList.ToolTip=markup</code> | ✅ 一致 |
| <code>DropDownList.ToolTip=text</code> | ✅ 一致 |
| <code>DropDownList.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>DropDownList.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>DropDownList.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>DropDownList.ValidationGroup=empty</code> | ✅ 一致 |
| <code>DropDownList.ValidationGroup=japanese</code> | ✅ 一致 |
| <code>DropDownList.ValidationGroup=markup</code> | ✅ 一致 |
| <code>DropDownList.ValidationGroup=text</code> | ✅ 一致 |
| <code>DropDownList.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>DropDownList.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>DropDownList.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>DropDownList.Visible=false</code> | ✅ 一致 |
| <code>DropDownList.Visible=true</code> | ✅ 一致 |
| <code>DropDownList.Width=120px</code> | ✅ 一致 |
| <code>DropDownList.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>DropDownList.Width=2em</code> | ✅ 一致 |
| <code>DropDownList.Width=50%</code> | ✅ 一致 |
| <code>DropDownList[data-bound].(default)</code> | ✅ 一致 |
| <code>DropDownList[data-bound].DataTextField=Name</code> | ✅ 一致 |
| <code>DropDownList[data-bound].DataTextField=Name+AppendDataBoundItems=true</code> | ✅ 一致 |
| <code>DropDownList[data-bound].DataTextField=Name+DataValueField=Code</code> | ✅ 一致 |
| <code>DropDownList[data-bound].DataTextField=Name+DataValueField=Code+SelectedIndex=1</code> | ✅ 一致 |
| <code>DropDownList[data-bound].DataTextField=Name+DataValueField=Code+SelectedValue=B2</code> | ✅ 一致 |
| <code>DropDownList[data-bound].DataTextField=Name+DataValueField=Code+SelectedValue=M</code> | ✅ 一致 |
| <code>DropDownList[data-bound].DataTextField=Price+DataTextFormatString=N2</code> | ✅ 一致 |
| <code>DropDownList[data-bound].DataTextField=text</code> | ✅ 一致 |
| <code>DropDownList[data-bound].DataValueField=Code</code> | ✅ 一致 |
| <code>DropDownList[naming container].(default)</code> | ✅ 一致 |
| <code>DropDownList[naming container].AutoPostBack=true</code> | ✅ 一致 |
| <code>DropDownList[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>DropDownList[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>DropDownList[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>DropDownList[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>DropDownList[naming container].ID=text</code> | ✅ 一致 |
| <code>DropDownList[themed].(default)</code> | ✅ 一致 |
| <code>DropDownList[themed].CssClass=text</code> | ✅ 一致 |
| <code>DropDownList[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>DropDownList[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| DropDownList [render][databind][items] first request | ✅ 一致 |
| DropDownList [postdata][change][dynamic] selections posted | ✅ 一致 |
| DropDownList [viewstate][change] posted again unchanged | ✅ 一致 |
| DropDownList [autopostback][change] auto postback | ✅ 一致 |
| DropDownList [validation] group g1, nothing chosen | ✅ 一致 |
| DropDownList [validation] group g1, chosen | ✅ 一致 |
| DropDownList [clientvalidation] group g3 in the browser: no request | ✅ 一致 |
| DropDownList [visible][viewstate][items] hidden lists shown with the items added in code | ✅ 一致 |
| DropDownList [bind] two-way binding in a template | ✅ 一致 |
| DropDownList [eventvalidation] a value the list does not have posted | ✅ 一致 |
| DropDownList [requestvalidation] markup posted | ✅ 一致 |

</details>
