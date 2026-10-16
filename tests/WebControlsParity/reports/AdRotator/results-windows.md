# AdRotator パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-15 13:47
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 206 | 206 | 0 | 0 |
| ② HTML | 170 | 170 | 0 | 0 |
| ③ シナリオ | 14 | 14 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 215 件中 215 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 39 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 14 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 153 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 13 | ❌ 1 件 |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 7 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 3 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 2 | 0 | ✅ |
| データソース連携（DataSource・DataTextField など） | 4 | 0 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>AdRotator.Api.Control.#ctor</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.Adapter</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.ClientID</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.Context</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.Controls</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.DataBind</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.Dispose</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.Disposed</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.Events</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.FindControl</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.Focus</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.HasControls</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.ID</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.Init</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.Load</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.OnInit</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.Page</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.Parent</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.PreRender</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.Render</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.Site</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.SkinID</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.Unload</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.ViewState</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>AdRotator.Api.Control.Visible</code> | ✅ 一致 |
| <code>AdRotator.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>AdRotator.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>AdRotator.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>AdRotator.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>AdRotator.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>AdRotator.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>AdRotator.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>AdRotator.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>AdRotator.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>AdRotator.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>AdRotator.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>AdRotator.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>AdRotator.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.AdRotator.#ctor</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.AdRotator.AdCreated</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.AdRotator.AdvertisementFile</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.AdRotator.AlternateTextField</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.AdRotator.Font</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.AdRotator.ImageUrlField</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.AdRotator.KeywordFilter</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.AdRotator.NavigateUrlField</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.AdRotator.OnAdCreated</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.AdRotator.OnInit</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.AdRotator.OnPreRender</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.AdRotator.PerformDataBinding</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.AdRotator.PerformSelect</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.AdRotator.Render</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.AdRotator.TagKey</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.AdRotator.Target</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.AdRotator.UniqueID</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.BaseDataBoundControl.#ctor</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.BaseDataBoundControl.ConfirmInitState</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.BaseDataBoundControl.DataBind</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.BaseDataBoundControl.DataBound</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.BaseDataBoundControl.DataSource</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.BaseDataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.BaseDataBoundControl.EnsureDataBound</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.BaseDataBoundControl.Initialized</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.BaseDataBoundControl.IsBoundUsingDataSourceID</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.BaseDataBoundControl.IsDataBindingAutomatic</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.BaseDataBoundControl.IsUsingModelBinders</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.BaseDataBoundControl.OnDataBound</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.BaseDataBoundControl.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.BaseDataBoundControl.OnInit</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.BaseDataBoundControl.OnPagePreLoad</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.BaseDataBoundControl.OnPreRender</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.BaseDataBoundControl.PerformSelect</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.BaseDataBoundControl.RequiresDataBinding</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.BaseDataBoundControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.BaseDataBoundControl.ValidateDataSource</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.DataBoundControl.#ctor</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.DataBoundControl.CallingDataMethods</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.DataBoundControl.CreateDataSourceSelectArguments</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.DataBoundControl.CreatingModelDataSource</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.DataBoundControl.DataMember</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.DataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.DataBoundControl.DataSourceObject</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.DataBoundControl.GetData</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.DataBoundControl.GetDataSource</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.DataBoundControl.IsUsingModelBinders</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.DataBoundControl.ItemType</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.DataBoundControl.LoadViewState</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.DataBoundControl.MarkAsDataBound</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.DataBoundControl.OnCreatingModelDataSource</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.DataBoundControl.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.DataBoundControl.OnDataSourceViewChanged</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.DataBoundControl.OnLoad</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.DataBoundControl.OnPagePreLoad</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.DataBoundControl.PerformDataBinding</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.DataBoundControl.PerformSelect</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.DataBoundControl.SaveViewState</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.DataBoundControl.SelectArguments</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.DataBoundControl.SelectMethod</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.DataBoundControl.TrackViewState</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.DataBoundControl.ValidateDataSource</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>AdRotator.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>AdRotator.(case list)</code> | ✅ 一致 |
| <code>AdRotator.(default)</code> | ✅ 一致 |
| <code>AdRotator.AccessKey=empty</code> | ✅ 一致 |
| <code>AdRotator.AccessKey=japanese</code> | ✅ 一致 |
| <code>AdRotator.AccessKey=k</code> | ✅ 一致 |
| <code>AdRotator.AccessKey=markup</code> | ✅ 一致 |
| <code>AdRotator.AccessKey=text</code> | ✅ 一致 |
| <code>AdRotator.AdvertisementFile=app-relative</code> | ✅ 一致 |
| <code>AdRotator.AdvertisementFile=empty</code> | ✅ 一致 |
| <code>AdRotator.AdvertisementFile=japanese</code> | ✅ 一致 |
| <code>AdRotator.AdvertisementFile=markup</code> | ✅ 一致 |
| <code>AdRotator.AdvertisementFile=missing</code> | ✅ 一致 |
| <code>AdRotator.AdvertisementFile=text</code> | ✅ 一致 |
| <code>AdRotator.AlternateTextField=Name</code> | ✅ 一致 |
| <code>AdRotator.AlternateTextField=empty</code> | ✅ 一致 |
| <code>AdRotator.AlternateTextField=japanese</code> | ✅ 一致 |
| <code>AdRotator.AlternateTextField=markup</code> | ✅ 一致 |
| <code>AdRotator.AlternateTextField=text</code> | ✅ 一致 |
| <code>AdRotator.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>AdRotator.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>AdRotator.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>AdRotator.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>AdRotator.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>AdRotator.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>AdRotator.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>AdRotator.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>AdRotator.BackColor=#3366CC</code> | ✅ 一致 |
| <code>AdRotator.BackColor=Red</code> | ✅ 一致 |
| <code>AdRotator.BackColor=Transparent</code> | ✅ 一致 |
| <code>AdRotator.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>AdRotator.BorderColor=Red</code> | ✅ 一致 |
| <code>AdRotator.BorderColor=Transparent</code> | ✅ 一致 |
| <code>AdRotator.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>AdRotator.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>AdRotator.BorderStyle=Double</code> | ✅ 一致 |
| <code>AdRotator.BorderStyle=Groove</code> | ✅ 一致 |
| <code>AdRotator.BorderStyle=Inset</code> | ✅ 一致 |
| <code>AdRotator.BorderStyle=None</code> | ✅ 一致 |
| <code>AdRotator.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>AdRotator.BorderStyle=Outset</code> | ✅ 一致 |
| <code>AdRotator.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>AdRotator.BorderStyle=Solid</code> | ✅ 一致 |
| <code>AdRotator.BorderWidth=120px</code> | ✅ 一致 |
| <code>AdRotator.BorderWidth=2em</code> | ✅ 一致 |
| <code>AdRotator.BorderWidth=50%</code> | ✅ 一致 |
| <code>AdRotator.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>AdRotator.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>AdRotator.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>AdRotator.ClientIDMode=Static</code> | ✅ 一致 |
| <code>AdRotator.CssClass=empty</code> | ✅ 一致 |
| <code>AdRotator.CssClass=japanese</code> | ✅ 一致 |
| <code>AdRotator.CssClass=markup</code> | ✅ 一致 |
| <code>AdRotator.CssClass=text</code> | ✅ 一致 |
| <code>AdRotator.DataMember=empty</code> | ✅ 一致 |
| <code>AdRotator.DataMember=japanese</code> | ✅ 一致 |
| <code>AdRotator.DataMember=markup</code> | ✅ 一致 |
| <code>AdRotator.DataMember=text</code> | ✅ 一致 |
| <code>AdRotator.DataSourceID=empty</code> | ✅ 一致 |
| <code>AdRotator.DataSourceID=japanese</code> | ✅ 一致 |
| <code>AdRotator.DataSourceID=markup</code> | ✅ 一致 |
| <code>AdRotator.DataSourceID=text</code> | ✅ 一致 |
| <code>AdRotator.EnableTheming=false</code> | ✅ 一致 |
| <code>AdRotator.EnableTheming=true</code> | ✅ 一致 |
| <code>AdRotator.EnableViewState=false</code> | ✅ 一致 |
| <code>AdRotator.EnableViewState=true</code> | ✅ 一致 |
| <code>AdRotator.Enabled=false</code> | ✅ 一致 |
| <code>AdRotator.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>AdRotator.Enabled=true</code> | ✅ 一致 |
| <code>AdRotator.Font.Bold=false</code> | ✅ 一致 |
| <code>AdRotator.Font.Bold=true</code> | ✅ 一致 |
| <code>AdRotator.Font.Italic=false</code> | ✅ 一致 |
| <code>AdRotator.Font.Italic=true</code> | ✅ 一致 |
| <code>AdRotator.Font.Name=empty</code> | ✅ 一致 |
| <code>AdRotator.Font.Name=japanese</code> | ✅ 一致 |
| <code>AdRotator.Font.Name=markup</code> | ✅ 一致 |
| <code>AdRotator.Font.Name=text</code> | ✅ 一致 |
| <code>AdRotator.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>AdRotator.Font.Overline=false</code> | ✅ 一致 |
| <code>AdRotator.Font.Overline=true</code> | ✅ 一致 |
| <code>AdRotator.Font.Size=12pt</code> | ✅ 一致 |
| <code>AdRotator.Font.Size=150%</code> | ✅ 一致 |
| <code>AdRotator.Font.Size=Large</code> | ✅ 一致 |
| <code>AdRotator.Font.Strikeout=false</code> | ✅ 一致 |
| <code>AdRotator.Font.Strikeout=true</code> | ✅ 一致 |
| <code>AdRotator.Font.Underline=false</code> | ✅ 一致 |
| <code>AdRotator.Font.Underline=true</code> | ✅ 一致 |
| <code>AdRotator.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>AdRotator.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>AdRotator.ForeColor=Red</code> | ✅ 一致 |
| <code>AdRotator.ForeColor=Transparent</code> | ✅ 一致 |
| <code>AdRotator.Height=120px</code> | ✅ 一致 |
| <code>AdRotator.Height=2em</code> | ✅ 一致 |
| <code>AdRotator.Height=50%</code> | ✅ 一致 |
| <code>AdRotator.ID=empty</code> | ✅ 一致 |
| <code>AdRotator.ID=japanese</code> | ✅ 一致 |
| <code>AdRotator.ID=markup</code> | ✅ 一致 |
| <code>AdRotator.ID=text</code> | ✅ 一致 |
| <code>AdRotator.ImageUrlField=Image</code> | ✅ 一致 |
| <code>AdRotator.ImageUrlField=empty</code> | ✅ 一致 |
| <code>AdRotator.ImageUrlField=japanese</code> | ✅ 一致 |
| <code>AdRotator.ImageUrlField=markup</code> | ✅ 一致 |
| <code>AdRotator.ImageUrlField=text</code> | ✅ 一致 |
| <code>AdRotator.ItemType=empty</code> | ✅ 一致 |
| <code>AdRotator.ItemType=japanese</code> | ✅ 一致 |
| <code>AdRotator.ItemType=markup</code> | ✅ 一致 |
| <code>AdRotator.ItemType=text</code> | ✅ 一致 |
| <code>AdRotator.KeywordFilter=empty</code> | ✅ 一致 |
| <code>AdRotator.KeywordFilter=japanese</code> | ✅ 一致 |
| <code>AdRotator.KeywordFilter=markup</code> | ✅ 一致 |
| <code>AdRotator.KeywordFilter=pen</code> | ✅ 一致 |
| <code>AdRotator.KeywordFilter=pen+Target=blank</code> | ✅ 一致 |
| <code>AdRotator.KeywordFilter=text</code> | ✅ 一致 |
| <code>AdRotator.NavigateUrlField=Code</code> | ✅ 一致 |
| <code>AdRotator.NavigateUrlField=empty</code> | ✅ 一致 |
| <code>AdRotator.NavigateUrlField=japanese</code> | ✅ 一致 |
| <code>AdRotator.NavigateUrlField=markup</code> | ✅ 一致 |
| <code>AdRotator.NavigateUrlField=text</code> | ✅ 一致 |
| <code>AdRotator.SelectMethod=empty</code> | ✅ 一致 |
| <code>AdRotator.SelectMethod=japanese</code> | ✅ 一致 |
| <code>AdRotator.SelectMethod=markup</code> | ✅ 一致 |
| <code>AdRotator.SelectMethod=text</code> | ✅ 一致 |
| <code>AdRotator.SkinID=empty</code> | ✅ 一致 |
| <code>AdRotator.SkinID=japanese</code> | ✅ 一致 |
| <code>AdRotator.SkinID=markup</code> | ✅ 一致 |
| <code>AdRotator.SkinID=text</code> | ✅ 一致 |
| <code>AdRotator.Style[color]=1em</code> | ✅ 一致 |
| <code>AdRotator.Style[color]=red</code> | ✅ 一致 |
| <code>AdRotator.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>AdRotator.Style[margin-left]=red</code> | ✅ 一致 |
| <code>AdRotator.TabIndex=-1</code> | ✅ 一致 |
| <code>AdRotator.TabIndex=0</code> | ✅ 一致 |
| <code>AdRotator.TabIndex=3</code> | ✅ 一致 |
| <code>AdRotator.Target=blank</code> | ✅ 一致 |
| <code>AdRotator.Target=blank+EnableViewState=false</code> | ✅ 一致 |
| <code>AdRotator.Target=blank+Visible=false</code> | ✅ 一致 |
| <code>AdRotator.Target=empty</code> | ✅ 一致 |
| <code>AdRotator.Target=japanese</code> | ✅ 一致 |
| <code>AdRotator.Target=markup</code> | ✅ 一致 |
| <code>AdRotator.Target=text</code> | ✅ 一致 |
| <code>AdRotator.ToolTip=empty</code> | ✅ 一致 |
| <code>AdRotator.ToolTip=japanese</code> | ✅ 一致 |
| <code>AdRotator.ToolTip=markup</code> | ✅ 一致 |
| <code>AdRotator.ToolTip=text</code> | ✅ 一致 |
| <code>AdRotator.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>AdRotator.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>AdRotator.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>AdRotator.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>AdRotator.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>AdRotator.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>AdRotator.Visible=false</code> | ✅ 一致 |
| <code>AdRotator.Visible=true</code> | ✅ 一致 |
| <code>AdRotator.Width=120px</code> | ✅ 一致 |
| <code>AdRotator.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>AdRotator.Width=2em</code> | ✅ 一致 |
| <code>AdRotator.Width=50%</code> | ✅ 一致 |
| <code>AdRotator[data-bound].(default)</code> | ✅ 一致 |
| <code>AdRotator[data-bound].AlternateTextField=Name+Target=blank</code> | ✅ 一致 |
| <code>AdRotator[data-bound].ImageUrlField=Image</code> | ✅ 一致 |
| <code>AdRotator[data-bound].ImageUrlField=Image+NavigateUrlField=Code+AlternateTextField=Name</code> | ✅ 一致 |
| <code>AdRotator[naming container].(default)</code> | ✅ 一致 |
| <code>AdRotator[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>AdRotator[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>AdRotator[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>AdRotator[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>AdRotator[naming container].ID=text</code> | ✅ 一致 |
| <code>AdRotator[naming container].Target=blank</code> | ✅ 一致 |
| <code>AdRotator[themed].(default)</code> | ✅ 一致 |
| <code>AdRotator[themed].CssClass=text</code> | ✅ 一致 |
| <code>AdRotator[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>AdRotator[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| AdRotator [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| AdRotator [viewstate][dynamic] posted: files, view state, what code set (a table&#39;s rows and a literal control&#39;s text are not kept) | ✅ 一致 |
| AdRotator [viewstate] the next view (NextView bubbled from a button) | ✅ 一致 |
| AdRotator [viewstate] switched by ID and by index: the first view&#39;s text box kept its value | ✅ 一致 |
| AdRotator [dynamic][viewstate] a dynamic multi-view&#39;s next view | ✅ 一致 |
| AdRotator [viewstate] a calendar&#39;s day chosen | ✅ 一致 |
| AdRotator [viewstate] a calendar&#39;s week chosen | ❌ 未実行 |
| AdRotator [viewstate] a calendar&#39;s next month | ✅ 一致 |
| AdRotator [viewstate] a dynamic calendar&#39;s day chosen | ✅ 一致 |
| AdRotator [viewstate] an image map&#39;s hot spot clicked (PostBack) | ✅ 一致 |
| AdRotator [viewstate] a dynamic image map&#39;s hot spot clicked | ✅ 一致 |
| AdRotator [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| AdRotator [viewstate] a file upload validated on the server (g1): no file posted | ✅ 一致 |
| AdRotator [viewstate] a postback event to a table (not registered for event validation): rejected | ✅ 一致 |

</details>
