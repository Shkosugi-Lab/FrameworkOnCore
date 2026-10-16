# Menu パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-16 18:00
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 256 | 256 | 0 | 0 |
| ② HTML | 321 | 321 | 0 | 0 |
| ③ シナリオ | 12 | 12 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 265 件中 265 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 98 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 12 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 259 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 7 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 8 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 2 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 5 | 0 | ✅ |
| ポストバックイベント | 0 | 4 | ✅ |
| テンプレート | 0 | 2 | ✅ |
| データソース連携（DataSource・DataTextField など） | 4 | 0 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>Menu.Api.Control.#ctor</code> | ✅ 一致 |
| <code>Menu.Api.Control.Adapter</code> | ✅ 一致 |
| <code>Menu.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>Menu.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>Menu.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>Menu.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>Menu.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>Menu.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>Menu.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>Menu.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>Menu.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>Menu.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>Menu.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>Menu.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>Menu.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>Menu.Api.Control.ClientID</code> | ✅ 一致 |
| <code>Menu.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>Menu.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>Menu.Api.Control.Context</code> | ✅ 一致 |
| <code>Menu.Api.Control.Controls</code> | ✅ 一致 |
| <code>Menu.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>Menu.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>Menu.Api.Control.DataBind</code> | ✅ 一致 |
| <code>Menu.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>Menu.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>Menu.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>Menu.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>Menu.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>Menu.Api.Control.Dispose</code> | ✅ 一致 |
| <code>Menu.Api.Control.Disposed</code> | ✅ 一致 |
| <code>Menu.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>Menu.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>Menu.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>Menu.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>Menu.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>Menu.Api.Control.Events</code> | ✅ 一致 |
| <code>Menu.Api.Control.FindControl</code> | ✅ 一致 |
| <code>Menu.Api.Control.Focus</code> | ✅ 一致 |
| <code>Menu.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>Menu.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>Menu.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>Menu.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>Menu.Api.Control.HasControls</code> | ✅ 一致 |
| <code>Menu.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>Menu.Api.Control.ID</code> | ✅ 一致 |
| <code>Menu.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>Menu.Api.Control.Init</code> | ✅ 一致 |
| <code>Menu.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>Menu.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>Menu.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>Menu.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>Menu.Api.Control.Load</code> | ✅ 一致 |
| <code>Menu.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>Menu.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>Menu.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>Menu.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>Menu.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>Menu.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>Menu.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>Menu.Api.Control.OnInit</code> | ✅ 一致 |
| <code>Menu.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>Menu.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>Menu.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>Menu.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>Menu.Api.Control.Page</code> | ✅ 一致 |
| <code>Menu.Api.Control.Parent</code> | ✅ 一致 |
| <code>Menu.Api.Control.PreRender</code> | ✅ 一致 |
| <code>Menu.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>Menu.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>Menu.Api.Control.Render</code> | ✅ 一致 |
| <code>Menu.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>Menu.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>Menu.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>Menu.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>Menu.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>Menu.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>Menu.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>Menu.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>Menu.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>Menu.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>Menu.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>Menu.Api.Control.Site</code> | ✅ 一致 |
| <code>Menu.Api.Control.SkinID</code> | ✅ 一致 |
| <code>Menu.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>Menu.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>Menu.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>Menu.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>Menu.Api.Control.Unload</code> | ✅ 一致 |
| <code>Menu.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>Menu.Api.Control.ViewState</code> | ✅ 一致 |
| <code>Menu.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>Menu.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>Menu.Api.Control.Visible</code> | ✅ 一致 |
| <code>Menu.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>Menu.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>Menu.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>Menu.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>Menu.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>Menu.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>Menu.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>Menu.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>Menu.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>Menu.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>Menu.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>Menu.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>Menu.Api.IPostBackEventHandler.RaisePostBackEvent</code> | ✅ 一致 |
| <code>Menu.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.BaseDataBoundControl.#ctor</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.BaseDataBoundControl.ConfirmInitState</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.BaseDataBoundControl.DataBind</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.BaseDataBoundControl.DataBound</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.BaseDataBoundControl.DataSource</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.BaseDataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.BaseDataBoundControl.EnsureDataBound</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.BaseDataBoundControl.Initialized</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.BaseDataBoundControl.IsBoundUsingDataSourceID</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.BaseDataBoundControl.IsDataBindingAutomatic</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.BaseDataBoundControl.IsUsingModelBinders</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.BaseDataBoundControl.OnDataBound</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.BaseDataBoundControl.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.BaseDataBoundControl.OnInit</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.BaseDataBoundControl.OnPagePreLoad</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.BaseDataBoundControl.OnPreRender</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.BaseDataBoundControl.PerformSelect</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.BaseDataBoundControl.RequiresDataBinding</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.BaseDataBoundControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.BaseDataBoundControl.ValidateDataSource</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.HierarchicalDataBoundControl.#ctor</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.HierarchicalDataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.HierarchicalDataBoundControl.GetData</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.HierarchicalDataBoundControl.GetDataSource</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.HierarchicalDataBoundControl.MarkAsDataBound</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.HierarchicalDataBoundControl.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.HierarchicalDataBoundControl.OnDataSourceChanged</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.HierarchicalDataBoundControl.OnLoad</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.HierarchicalDataBoundControl.OnPagePreLoad</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.HierarchicalDataBoundControl.PerformDataBinding</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.HierarchicalDataBoundControl.PerformSelect</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.HierarchicalDataBoundControl.ValidateDataSource</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.#ctor</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.AddAttributesToRender</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.Controls</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.CreateChildControls</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.DataBind</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.DataBindings</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.DisappearAfter</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.DynamicBottomSeparatorImageUrl</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.DynamicEnableDefaultPopOutImage</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.DynamicHorizontalOffset</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.DynamicHoverStyle</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.DynamicItemFormatString</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.DynamicItemTemplate</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.DynamicMenuItemStyle</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.DynamicMenuStyle</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.DynamicPopOutImageTextFormatString</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.DynamicPopOutImageUrl</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.DynamicSelectedStyle</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.DynamicTopSeparatorImageUrl</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.DynamicVerticalOffset</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.EnsureDataBound</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.FindItem</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.GetDesignModeState</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.IncludeStyleBlock</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.ItemWrap</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.Items</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.LevelMenuItemStyles</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.LevelSelectedStyles</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.LevelSubMenuStyles</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.LoadControlState</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.LoadViewState</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.MaximumDynamicDisplayLevels</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.MenuItemClick</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.MenuItemClickCommandName</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.MenuItemDataBound</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.OnBubbleEvent</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.OnDataBinding</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.OnInit</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.OnMenuItemClick</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.OnMenuItemDataBound</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.OnPreRender</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.Orientation</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.PathSeparator</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.PerformDataBinding</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.RaisePostBackEvent</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.Render</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.RenderBeginTag</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.RenderContents</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.RenderEndTag</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.RenderingMode</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.SaveControlState</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.SaveViewState</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.ScrollDownImageUrl</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.ScrollDownText</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.ScrollUpImageUrl</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.ScrollUpText</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.SelectedItem</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.SelectedValue</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.SetDesignModeState</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.SetItemDataBound</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.SetItemDataItem</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.SetItemDataPath</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.SkipLinkText</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.StaticBottomSeparatorImageUrl</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.StaticDisplayLevels</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.StaticEnableDefaultPopOutImage</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.StaticHoverStyle</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.StaticItemFormatString</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.StaticItemTemplate</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.StaticMenuItemStyle</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.StaticMenuStyle</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.StaticPopOutImageTextFormatString</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.StaticPopOutImageUrl</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.StaticSelectedStyle</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.StaticSubMenuIndent</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.StaticTopSeparatorImageUrl</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.TagKey</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.Target</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.Menu.TrackViewState</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>Menu.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>Menu.(case list)</code> | ✅ 一致 |
| <code>Menu.(default)</code> | ✅ 一致 |
| <code>Menu.AccessKey=empty</code> | ✅ 一致 |
| <code>Menu.AccessKey=japanese</code> | ✅ 一致 |
| <code>Menu.AccessKey=k</code> | ✅ 一致 |
| <code>Menu.AccessKey=markup</code> | ✅ 一致 |
| <code>Menu.AccessKey=text</code> | ✅ 一致 |
| <code>Menu.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>Menu.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>Menu.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>Menu.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>Menu.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>Menu.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>Menu.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>Menu.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>Menu.BackColor=#3366CC</code> | ✅ 一致 |
| <code>Menu.BackColor=Red</code> | ✅ 一致 |
| <code>Menu.BackColor=Transparent</code> | ✅ 一致 |
| <code>Menu.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>Menu.BorderColor=Red</code> | ✅ 一致 |
| <code>Menu.BorderColor=Transparent</code> | ✅ 一致 |
| <code>Menu.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>Menu.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>Menu.BorderStyle=Double</code> | ✅ 一致 |
| <code>Menu.BorderStyle=Groove</code> | ✅ 一致 |
| <code>Menu.BorderStyle=Inset</code> | ✅ 一致 |
| <code>Menu.BorderStyle=None</code> | ✅ 一致 |
| <code>Menu.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>Menu.BorderStyle=Outset</code> | ✅ 一致 |
| <code>Menu.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>Menu.BorderStyle=Solid</code> | ✅ 一致 |
| <code>Menu.BorderWidth=120px</code> | ✅ 一致 |
| <code>Menu.BorderWidth=2em</code> | ✅ 一致 |
| <code>Menu.BorderWidth=50%</code> | ✅ 一致 |
| <code>Menu.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Menu.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Menu.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Menu.ClientIDMode=Static</code> | ✅ 一致 |
| <code>Menu.CssClass=empty</code> | ✅ 一致 |
| <code>Menu.CssClass=japanese</code> | ✅ 一致 |
| <code>Menu.CssClass=markup</code> | ✅ 一致 |
| <code>Menu.CssClass=text</code> | ✅ 一致 |
| <code>Menu.DataSourceID=empty</code> | ✅ 一致 |
| <code>Menu.DataSourceID=japanese</code> | ✅ 一致 |
| <code>Menu.DataSourceID=markup</code> | ✅ 一致 |
| <code>Menu.DataSourceID=text</code> | ✅ 一致 |
| <code>Menu.DisappearAfter=-1</code> | ✅ 一致 |
| <code>Menu.DisappearAfter=0</code> | ✅ 一致 |
| <code>Menu.DisappearAfter=3</code> | ✅ 一致 |
| <code>Menu.DisappearAfter=3+DynamicHorizontalOffset=3</code> | ✅ 一致 |
| <code>Menu.DynamicBottomSeparatorImageUrl=empty</code> | ✅ 一致 |
| <code>Menu.DynamicBottomSeparatorImageUrl=japanese</code> | ✅ 一致 |
| <code>Menu.DynamicBottomSeparatorImageUrl=markup</code> | ✅ 一致 |
| <code>Menu.DynamicBottomSeparatorImageUrl=text</code> | ✅ 一致 |
| <code>Menu.DynamicEnableDefaultPopOutImage=false</code> | ✅ 一致 |
| <code>Menu.DynamicEnableDefaultPopOutImage=true</code> | ✅ 一致 |
| <code>Menu.DynamicHorizontalOffset=-1</code> | ✅ 一致 |
| <code>Menu.DynamicHorizontalOffset=0</code> | ✅ 一致 |
| <code>Menu.DynamicHorizontalOffset=3</code> | ✅ 一致 |
| <code>Menu.DynamicHoverStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Menu.DynamicHoverStyle.CssClass=text</code> | ✅ 一致 |
| <code>Menu.DynamicHoverStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Menu.DynamicHoverStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Menu.DynamicHoverStyle.Height=120px</code> | ✅ 一致 |
| <code>Menu.DynamicItemFormatString=empty</code> | ✅ 一致 |
| <code>Menu.DynamicItemFormatString=japanese</code> | ✅ 一致 |
| <code>Menu.DynamicItemFormatString=markup</code> | ✅ 一致 |
| <code>Menu.DynamicItemFormatString=text</code> | ✅ 一致 |
| <code>Menu.DynamicMenuItemStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Menu.DynamicMenuItemStyle.CssClass=text</code> | ✅ 一致 |
| <code>Menu.DynamicMenuItemStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Menu.DynamicMenuItemStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Menu.DynamicMenuItemStyle.Height=120px</code> | ✅ 一致 |
| <code>Menu.DynamicMenuStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Menu.DynamicMenuStyle.CssClass=text</code> | ✅ 一致 |
| <code>Menu.DynamicMenuStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Menu.DynamicMenuStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Menu.DynamicMenuStyle.Height=120px</code> | ✅ 一致 |
| <code>Menu.DynamicPopOutImageTextFormatString=empty</code> | ✅ 一致 |
| <code>Menu.DynamicPopOutImageTextFormatString=japanese</code> | ✅ 一致 |
| <code>Menu.DynamicPopOutImageTextFormatString=markup</code> | ✅ 一致 |
| <code>Menu.DynamicPopOutImageTextFormatString=text</code> | ✅ 一致 |
| <code>Menu.DynamicPopOutImageUrl=app-relative</code> | ✅ 一致 |
| <code>Menu.DynamicPopOutImageUrl=empty</code> | ✅ 一致 |
| <code>Menu.DynamicPopOutImageUrl=japanese</code> | ✅ 一致 |
| <code>Menu.DynamicPopOutImageUrl=markup</code> | ✅ 一致 |
| <code>Menu.DynamicPopOutImageUrl=text</code> | ✅ 一致 |
| <code>Menu.DynamicSelectedStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Menu.DynamicSelectedStyle.CssClass=text</code> | ✅ 一致 |
| <code>Menu.DynamicSelectedStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Menu.DynamicSelectedStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Menu.DynamicSelectedStyle.Height=120px</code> | ✅ 一致 |
| <code>Menu.DynamicTopSeparatorImageUrl=empty</code> | ✅ 一致 |
| <code>Menu.DynamicTopSeparatorImageUrl=japanese</code> | ✅ 一致 |
| <code>Menu.DynamicTopSeparatorImageUrl=markup</code> | ✅ 一致 |
| <code>Menu.DynamicTopSeparatorImageUrl=text</code> | ✅ 一致 |
| <code>Menu.DynamicVerticalOffset=-1</code> | ✅ 一致 |
| <code>Menu.DynamicVerticalOffset=0</code> | ✅ 一致 |
| <code>Menu.DynamicVerticalOffset=3</code> | ✅ 一致 |
| <code>Menu.EnableTheming=false</code> | ✅ 一致 |
| <code>Menu.EnableTheming=true</code> | ✅ 一致 |
| <code>Menu.EnableViewState=false</code> | ✅ 一致 |
| <code>Menu.EnableViewState=true</code> | ✅ 一致 |
| <code>Menu.Enabled=false</code> | ✅ 一致 |
| <code>Menu.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>Menu.Enabled=false+StaticDisplayLevels=2</code> | ✅ 一致 |
| <code>Menu.Enabled=true</code> | ✅ 一致 |
| <code>Menu.Font.Bold=false</code> | ✅ 一致 |
| <code>Menu.Font.Bold=true</code> | ✅ 一致 |
| <code>Menu.Font.Italic=false</code> | ✅ 一致 |
| <code>Menu.Font.Italic=true</code> | ✅ 一致 |
| <code>Menu.Font.Name=empty</code> | ✅ 一致 |
| <code>Menu.Font.Name=japanese</code> | ✅ 一致 |
| <code>Menu.Font.Name=markup</code> | ✅ 一致 |
| <code>Menu.Font.Name=text</code> | ✅ 一致 |
| <code>Menu.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>Menu.Font.Overline=false</code> | ✅ 一致 |
| <code>Menu.Font.Overline=true</code> | ✅ 一致 |
| <code>Menu.Font.Size=12pt</code> | ✅ 一致 |
| <code>Menu.Font.Size=150%</code> | ✅ 一致 |
| <code>Menu.Font.Size=Large</code> | ✅ 一致 |
| <code>Menu.Font.Strikeout=false</code> | ✅ 一致 |
| <code>Menu.Font.Strikeout=true</code> | ✅ 一致 |
| <code>Menu.Font.Underline=false</code> | ✅ 一致 |
| <code>Menu.Font.Underline=true</code> | ✅ 一致 |
| <code>Menu.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>Menu.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>Menu.ForeColor=Red</code> | ✅ 一致 |
| <code>Menu.ForeColor=Transparent</code> | ✅ 一致 |
| <code>Menu.Height=120px</code> | ✅ 一致 |
| <code>Menu.Height=2em</code> | ✅ 一致 |
| <code>Menu.Height=50%</code> | ✅ 一致 |
| <code>Menu.ID=empty</code> | ✅ 一致 |
| <code>Menu.ID=japanese</code> | ✅ 一致 |
| <code>Menu.ID=markup</code> | ✅ 一致 |
| <code>Menu.ID=text</code> | ✅ 一致 |
| <code>Menu.IncludeStyleBlock=false</code> | ✅ 一致 |
| <code>Menu.IncludeStyleBlock=true</code> | ✅ 一致 |
| <code>Menu.ItemWrap=false</code> | ✅ 一致 |
| <code>Menu.ItemWrap=true</code> | ✅ 一致 |
| <code>Menu.Items[1].Enabled=false</code> | ✅ 一致 |
| <code>Menu.Items[1].Enabled=false+StaticDisplayLevels=2</code> | ✅ 一致 |
| <code>Menu.Items[1].Enabled=true</code> | ✅ 一致 |
| <code>Menu.Items[1].ImageUrl=app-relative</code> | ✅ 一致 |
| <code>Menu.Items[1].ImageUrl=empty</code> | ✅ 一致 |
| <code>Menu.Items[1].ImageUrl=japanese</code> | ✅ 一致 |
| <code>Menu.Items[1].ImageUrl=markup</code> | ✅ 一致 |
| <code>Menu.Items[1].ImageUrl=text</code> | ✅ 一致 |
| <code>Menu.Items[1].NavigateUrl=app-relative</code> | ✅ 一致 |
| <code>Menu.Items[1].NavigateUrl=app-relative+Target=blank</code> | ✅ 一致 |
| <code>Menu.Items[1].NavigateUrl=empty</code> | ✅ 一致 |
| <code>Menu.Items[1].NavigateUrl=japanese</code> | ✅ 一致 |
| <code>Menu.Items[1].NavigateUrl=markup</code> | ✅ 一致 |
| <code>Menu.Items[1].NavigateUrl=text</code> | ✅ 一致 |
| <code>Menu.Items[1].PopOutImageUrl=empty</code> | ✅ 一致 |
| <code>Menu.Items[1].PopOutImageUrl=japanese</code> | ✅ 一致 |
| <code>Menu.Items[1].PopOutImageUrl=markup</code> | ✅ 一致 |
| <code>Menu.Items[1].PopOutImageUrl=text</code> | ✅ 一致 |
| <code>Menu.Items[1].Selectable=false</code> | ✅ 一致 |
| <code>Menu.Items[1].Selectable=false+Items[1].ToolTip=text</code> | ✅ 一致 |
| <code>Menu.Items[1].Selectable=true</code> | ✅ 一致 |
| <code>Menu.Items[1].Selected=false</code> | ✅ 一致 |
| <code>Menu.Items[1].Selected=true</code> | ✅ 一致 |
| <code>Menu.Items[1].Selected=true+StaticSelectedStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Menu.Items[1].SeparatorImageUrl=empty</code> | ✅ 一致 |
| <code>Menu.Items[1].SeparatorImageUrl=japanese</code> | ✅ 一致 |
| <code>Menu.Items[1].SeparatorImageUrl=markup</code> | ✅ 一致 |
| <code>Menu.Items[1].SeparatorImageUrl=text</code> | ✅ 一致 |
| <code>Menu.Items[1].Target=empty</code> | ✅ 一致 |
| <code>Menu.Items[1].Target=japanese</code> | ✅ 一致 |
| <code>Menu.Items[1].Target=markup</code> | ✅ 一致 |
| <code>Menu.Items[1].Target=text</code> | ✅ 一致 |
| <code>Menu.Items[1].Text=empty</code> | ✅ 一致 |
| <code>Menu.Items[1].Text=japanese</code> | ✅ 一致 |
| <code>Menu.Items[1].Text=markup</code> | ✅ 一致 |
| <code>Menu.Items[1].Text=text</code> | ✅ 一致 |
| <code>Menu.Items[1].ToolTip=empty</code> | ✅ 一致 |
| <code>Menu.Items[1].ToolTip=japanese</code> | ✅ 一致 |
| <code>Menu.Items[1].ToolTip=markup</code> | ✅ 一致 |
| <code>Menu.Items[1].ToolTip=text</code> | ✅ 一致 |
| <code>Menu.Items[1].Value=empty</code> | ✅ 一致 |
| <code>Menu.Items[1].Value=japanese</code> | ✅ 一致 |
| <code>Menu.Items[1].Value=markup</code> | ✅ 一致 |
| <code>Menu.Items[1].Value=text</code> | ✅ 一致 |
| <code>Menu.MaximumDynamicDisplayLevels=-1</code> | ✅ 一致 |
| <code>Menu.MaximumDynamicDisplayLevels=0</code> | ✅ 一致 |
| <code>Menu.MaximumDynamicDisplayLevels=0+StaticDisplayLevels=2</code> | ✅ 一致 |
| <code>Menu.MaximumDynamicDisplayLevels=3</code> | ✅ 一致 |
| <code>Menu.Orientation=Horizontal</code> | ✅ 一致 |
| <code>Menu.Orientation=Horizontal+StaticDisplayLevels=2</code> | ✅ 一致 |
| <code>Menu.Orientation=Vertical</code> | ✅ 一致 |
| <code>Menu.RenderingMode=Default</code> | ✅ 一致 |
| <code>Menu.RenderingMode=List</code> | ✅ 一致 |
| <code>Menu.RenderingMode=List+IncludeStyleBlock=false</code> | ✅ 一致 |
| <code>Menu.RenderingMode=Table</code> | ✅ 一致 |
| <code>Menu.RenderingMode=Table+Orientation=Horizontal</code> | ✅ 一致 |
| <code>Menu.RenderingMode=Table+StaticDisplayLevels=2</code> | ✅ 一致 |
| <code>Menu.ScrollDownImageUrl=app-relative</code> | ✅ 一致 |
| <code>Menu.ScrollDownImageUrl=empty</code> | ✅ 一致 |
| <code>Menu.ScrollDownImageUrl=japanese</code> | ✅ 一致 |
| <code>Menu.ScrollDownImageUrl=markup</code> | ✅ 一致 |
| <code>Menu.ScrollDownImageUrl=text</code> | ✅ 一致 |
| <code>Menu.ScrollDownText=empty</code> | ✅ 一致 |
| <code>Menu.ScrollDownText=japanese</code> | ✅ 一致 |
| <code>Menu.ScrollDownText=markup</code> | ✅ 一致 |
| <code>Menu.ScrollDownText=text</code> | ✅ 一致 |
| <code>Menu.ScrollUpImageUrl=empty</code> | ✅ 一致 |
| <code>Menu.ScrollUpImageUrl=japanese</code> | ✅ 一致 |
| <code>Menu.ScrollUpImageUrl=markup</code> | ✅ 一致 |
| <code>Menu.ScrollUpImageUrl=text</code> | ✅ 一致 |
| <code>Menu.ScrollUpText=empty</code> | ✅ 一致 |
| <code>Menu.ScrollUpText=japanese</code> | ✅ 一致 |
| <code>Menu.ScrollUpText=markup</code> | ✅ 一致 |
| <code>Menu.ScrollUpText=text</code> | ✅ 一致 |
| <code>Menu.SkinID=empty</code> | ✅ 一致 |
| <code>Menu.SkinID=japanese</code> | ✅ 一致 |
| <code>Menu.SkinID=markup</code> | ✅ 一致 |
| <code>Menu.SkinID=text</code> | ✅ 一致 |
| <code>Menu.SkipLinkText=empty</code> | ✅ 一致 |
| <code>Menu.SkipLinkText=empty+Orientation=Horizontal</code> | ✅ 一致 |
| <code>Menu.SkipLinkText=japanese</code> | ✅ 一致 |
| <code>Menu.SkipLinkText=markup</code> | ✅ 一致 |
| <code>Menu.SkipLinkText=text</code> | ✅ 一致 |
| <code>Menu.StaticBottomSeparatorImageUrl=empty</code> | ✅ 一致 |
| <code>Menu.StaticBottomSeparatorImageUrl=japanese</code> | ✅ 一致 |
| <code>Menu.StaticBottomSeparatorImageUrl=markup</code> | ✅ 一致 |
| <code>Menu.StaticBottomSeparatorImageUrl=text</code> | ✅ 一致 |
| <code>Menu.StaticDisplayLevels=-1</code> | ✅ 一致 |
| <code>Menu.StaticDisplayLevels=0</code> | ✅ 一致 |
| <code>Menu.StaticDisplayLevels=2</code> | ✅ 一致 |
| <code>Menu.StaticDisplayLevels=2+EnableViewState=false</code> | ✅ 一致 |
| <code>Menu.StaticDisplayLevels=2+Visible=false</code> | ✅ 一致 |
| <code>Menu.StaticDisplayLevels=3</code> | ✅ 一致 |
| <code>Menu.StaticEnableDefaultPopOutImage=false</code> | ✅ 一致 |
| <code>Menu.StaticEnableDefaultPopOutImage=false+DynamicEnableDefaultPopOutImage=false</code> | ✅ 一致 |
| <code>Menu.StaticEnableDefaultPopOutImage=true</code> | ✅ 一致 |
| <code>Menu.StaticHoverStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Menu.StaticHoverStyle.CssClass=text</code> | ✅ 一致 |
| <code>Menu.StaticHoverStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Menu.StaticHoverStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Menu.StaticHoverStyle.Height=120px</code> | ✅ 一致 |
| <code>Menu.StaticItemFormatString=empty</code> | ✅ 一致 |
| <code>Menu.StaticItemFormatString=japanese</code> | ✅ 一致 |
| <code>Menu.StaticItemFormatString=markup</code> | ✅ 一致 |
| <code>Menu.StaticItemFormatString=text</code> | ✅ 一致 |
| <code>Menu.StaticMenuItemStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Menu.StaticMenuItemStyle.CssClass=text</code> | ✅ 一致 |
| <code>Menu.StaticMenuItemStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Menu.StaticMenuItemStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Menu.StaticMenuItemStyle.ForeColor=Red+DynamicMenuItemStyle.BackColor=#3366CC</code> | ✅ 一致 |
| <code>Menu.StaticMenuItemStyle.Height=120px</code> | ✅ 一致 |
| <code>Menu.StaticMenuStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Menu.StaticMenuStyle.CssClass=text</code> | ✅ 一致 |
| <code>Menu.StaticMenuStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Menu.StaticMenuStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Menu.StaticMenuStyle.Height=120px</code> | ✅ 一致 |
| <code>Menu.StaticPopOutImageTextFormatString=empty</code> | ✅ 一致 |
| <code>Menu.StaticPopOutImageTextFormatString=japanese</code> | ✅ 一致 |
| <code>Menu.StaticPopOutImageTextFormatString=markup</code> | ✅ 一致 |
| <code>Menu.StaticPopOutImageTextFormatString=text</code> | ✅ 一致 |
| <code>Menu.StaticPopOutImageUrl=app-relative</code> | ✅ 一致 |
| <code>Menu.StaticPopOutImageUrl=empty</code> | ✅ 一致 |
| <code>Menu.StaticPopOutImageUrl=japanese</code> | ✅ 一致 |
| <code>Menu.StaticPopOutImageUrl=markup</code> | ✅ 一致 |
| <code>Menu.StaticPopOutImageUrl=text</code> | ✅ 一致 |
| <code>Menu.StaticSelectedStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Menu.StaticSelectedStyle.CssClass=text</code> | ✅ 一致 |
| <code>Menu.StaticSelectedStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Menu.StaticSelectedStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Menu.StaticSelectedStyle.Height=120px</code> | ✅ 一致 |
| <code>Menu.StaticSubMenuIndent=120px</code> | ✅ 一致 |
| <code>Menu.StaticSubMenuIndent=2em</code> | ✅ 一致 |
| <code>Menu.StaticSubMenuIndent=50%</code> | ✅ 一致 |
| <code>Menu.StaticTopSeparatorImageUrl=empty</code> | ✅ 一致 |
| <code>Menu.StaticTopSeparatorImageUrl=japanese</code> | ✅ 一致 |
| <code>Menu.StaticTopSeparatorImageUrl=markup</code> | ✅ 一致 |
| <code>Menu.StaticTopSeparatorImageUrl=text</code> | ✅ 一致 |
| <code>Menu.Style[color]=1em</code> | ✅ 一致 |
| <code>Menu.Style[color]=red</code> | ✅ 一致 |
| <code>Menu.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>Menu.Style[margin-left]=red</code> | ✅ 一致 |
| <code>Menu.TabIndex=-1</code> | ✅ 一致 |
| <code>Menu.TabIndex=0</code> | ✅ 一致 |
| <code>Menu.TabIndex=3</code> | ✅ 一致 |
| <code>Menu.Target=blank</code> | ✅ 一致 |
| <code>Menu.Target=empty</code> | ✅ 一致 |
| <code>Menu.Target=japanese</code> | ✅ 一致 |
| <code>Menu.Target=markup</code> | ✅ 一致 |
| <code>Menu.Target=text</code> | ✅ 一致 |
| <code>Menu.ToolTip=empty</code> | ✅ 一致 |
| <code>Menu.ToolTip=japanese</code> | ✅ 一致 |
| <code>Menu.ToolTip=markup</code> | ✅ 一致 |
| <code>Menu.ToolTip=text</code> | ✅ 一致 |
| <code>Menu.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>Menu.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>Menu.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>Menu.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>Menu.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>Menu.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>Menu.Visible=false</code> | ✅ 一致 |
| <code>Menu.Visible=true</code> | ✅ 一致 |
| <code>Menu.Width=120px</code> | ✅ 一致 |
| <code>Menu.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>Menu.Width=2em</code> | ✅ 一致 |
| <code>Menu.Width=50%</code> | ✅ 一致 |
| <code>Menu[data-bound].(default)</code> | ✅ 一致 |
| <code>Menu[data-bound].MaximumDynamicDisplayLevels=0</code> | ✅ 一致 |
| <code>Menu[data-bound].RenderingMode=Table</code> | ✅ 一致 |
| <code>Menu[data-bound].StaticDisplayLevels=2</code> | ✅ 一致 |
| <code>Menu[naming container].(default)</code> | ✅ 一致 |
| <code>Menu[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Menu[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Menu[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Menu[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>Menu[naming container].ID=text</code> | ✅ 一致 |
| <code>Menu[naming container].RenderingMode=Table</code> | ✅ 一致 |
| <code>Menu[naming container].StaticDisplayLevels=2</code> | ✅ 一致 |
| <code>Menu[themed].(default)</code> | ✅ 一致 |
| <code>Menu[themed].CssClass=text</code> | ✅ 一致 |
| <code>Menu[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>Menu[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| Menu [render][databind][dynamic][templates] first request: the site map as a path, trees and menus; bound to XML; templated; made in code | ✅ 一致 |
| Menu [viewstate] a node expanded by a postback (EnableClientScript=false) | ✅ 一致 |
| Menu [viewstate] a node selected (its value a control parameter&#39;s: ControlValueProperty) | ✅ 一致 |
| Menu [viewstate] a node&#39;s check box posted | ✅ 一致 |
| Menu [viewstate] a node populated on demand by a postback | ✅ 一致 |
| Menu [viewstate] a node populated on demand by a client callback (no postback) | ✅ 一致 |
| Menu [postbackevent] a menu&#39;s item clicked | ✅ 一致 |
| Menu [postbackevent][databind] a bound menu&#39;s item clicked | ✅ 一致 |
| Menu [postbackevent][templates] a templated menu&#39;s item clicked | ✅ 一致 |
| Menu [viewstate] a dynamic tree&#39;s node selected | ✅ 一致 |
| Menu [postbackevent][dynamic] a dynamic menu&#39;s item clicked | ✅ 一致 |
| Menu [visible][viewstate] hidden ones shown with what was added while hidden | ✅ 一致 |

</details>
