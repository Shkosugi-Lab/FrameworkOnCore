# Wizard パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-16 18:05
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 250 | 250 | 0 | 0 |
| ② HTML | 349 | 349 | 0 | 0 |
| ③ シナリオ | 15 | 15 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 259 件中 259 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 128 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 15 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 292 | 1 | ✅ |
| ViewState の保存と復元 | 11 | 14 | ✅ |
| Visible=false（描画されず状態は保持） | 3 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 7 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 3 | 0 | ✅ |
| テンプレート | 0 | 2 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>Wizard.Api.Control.#ctor</code> | ✅ 一致 |
| <code>Wizard.Api.Control.Adapter</code> | ✅ 一致 |
| <code>Wizard.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>Wizard.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>Wizard.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>Wizard.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>Wizard.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>Wizard.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>Wizard.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>Wizard.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>Wizard.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>Wizard.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>Wizard.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>Wizard.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>Wizard.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>Wizard.Api.Control.ClientID</code> | ✅ 一致 |
| <code>Wizard.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>Wizard.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>Wizard.Api.Control.Context</code> | ✅ 一致 |
| <code>Wizard.Api.Control.Controls</code> | ✅ 一致 |
| <code>Wizard.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>Wizard.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>Wizard.Api.Control.DataBind</code> | ✅ 一致 |
| <code>Wizard.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>Wizard.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>Wizard.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>Wizard.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>Wizard.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>Wizard.Api.Control.Dispose</code> | ✅ 一致 |
| <code>Wizard.Api.Control.Disposed</code> | ✅ 一致 |
| <code>Wizard.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>Wizard.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>Wizard.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>Wizard.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>Wizard.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>Wizard.Api.Control.Events</code> | ✅ 一致 |
| <code>Wizard.Api.Control.FindControl</code> | ✅ 一致 |
| <code>Wizard.Api.Control.Focus</code> | ✅ 一致 |
| <code>Wizard.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>Wizard.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>Wizard.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>Wizard.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>Wizard.Api.Control.HasControls</code> | ✅ 一致 |
| <code>Wizard.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>Wizard.Api.Control.ID</code> | ✅ 一致 |
| <code>Wizard.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>Wizard.Api.Control.Init</code> | ✅ 一致 |
| <code>Wizard.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>Wizard.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>Wizard.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>Wizard.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>Wizard.Api.Control.Load</code> | ✅ 一致 |
| <code>Wizard.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>Wizard.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>Wizard.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>Wizard.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>Wizard.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>Wizard.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>Wizard.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>Wizard.Api.Control.OnInit</code> | ✅ 一致 |
| <code>Wizard.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>Wizard.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>Wizard.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>Wizard.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>Wizard.Api.Control.Page</code> | ✅ 一致 |
| <code>Wizard.Api.Control.Parent</code> | ✅ 一致 |
| <code>Wizard.Api.Control.PreRender</code> | ✅ 一致 |
| <code>Wizard.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>Wizard.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>Wizard.Api.Control.Render</code> | ✅ 一致 |
| <code>Wizard.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>Wizard.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>Wizard.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>Wizard.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>Wizard.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>Wizard.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>Wizard.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>Wizard.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>Wizard.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>Wizard.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>Wizard.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>Wizard.Api.Control.Site</code> | ✅ 一致 |
| <code>Wizard.Api.Control.SkinID</code> | ✅ 一致 |
| <code>Wizard.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>Wizard.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>Wizard.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>Wizard.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>Wizard.Api.Control.Unload</code> | ✅ 一致 |
| <code>Wizard.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>Wizard.Api.Control.ViewState</code> | ✅ 一致 |
| <code>Wizard.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>Wizard.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>Wizard.Api.Control.Visible</code> | ✅ 一致 |
| <code>Wizard.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>Wizard.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>Wizard.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>Wizard.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>Wizard.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>Wizard.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>Wizard.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>Wizard.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>Wizard.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>Wizard.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>Wizard.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>Wizard.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>Wizard.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.CompositeControl.#ctor</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.CompositeControl.Controls</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.CompositeControl.DataBind</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.CompositeControl.RecreateChildControls</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.CompositeControl.Render</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.CompositeControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.ICompositeControlDesignerAccessor.RecreateChildControls</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.WebControl.Width</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.#ctor</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.ActiveStep</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.ActiveStepChanged</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.ActiveStepIndex</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.AllowNavigationToStep</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.CancelButtonClick</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.CancelButtonID</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.CancelButtonImageUrl</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.CancelButtonStyle</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.CancelButtonText</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.CancelButtonType</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.CancelCommandName</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.CancelDestinationPageUrl</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.CellPadding</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.CellSpacing</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.CreateChildControls</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.CreateControlCollection</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.CreateControlHierarchy</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.CreateControlStyle</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.CustomFinishButtonID</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.CustomNextButtonID</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.CustomPreviousButtonID</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.DataListID</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.DisplayCancelButton</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.DisplaySideBar</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.FinishButtonClick</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.FinishButtonID</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.FinishCompleteButtonImageUrl</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.FinishCompleteButtonStyle</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.FinishCompleteButtonText</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.FinishCompleteButtonType</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.FinishDestinationPageUrl</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.FinishNavigationTemplate</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.FinishPreviousButtonID</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.FinishPreviousButtonImageUrl</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.FinishPreviousButtonStyle</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.FinishPreviousButtonText</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.FinishPreviousButtonType</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.GetDesignModeState</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.GetHistory</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.GetStepType</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.HeaderPlaceholderId</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.HeaderStyle</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.HeaderTemplate</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.HeaderText</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.LayoutTemplate</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.LoadControlState</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.LoadViewState</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.MoveCompleteCommandName</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.MoveNextCommandName</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.MovePreviousCommandName</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.MoveTo</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.MoveToCommandName</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.NavigationButtonStyle</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.NavigationPlaceholderId</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.NavigationStyle</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.NextButtonClick</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.OnActiveStepChanged</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.OnBubbleEvent</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.OnCancelButtonClick</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.OnFinishButtonClick</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.OnInit</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.OnNextButtonClick</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.OnPreviousButtonClick</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.OnSideBarButtonClick</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.PreviousButtonClick</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.RegisterCommandEvents</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.Render</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.SaveControlState</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.SaveViewState</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.SideBarButtonClick</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.SideBarButtonID</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.SideBarButtonStyle</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.SideBarPlaceholderId</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.SideBarStyle</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.SideBarTemplate</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.SkipLinkText</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.StartNavigationTemplate</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.StartNextButtonID</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.StartNextButtonImageUrl</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.StartNextButtonStyle</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.StartNextButtonText</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.StartNextButtonType</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.StepNavigationTemplate</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.StepNextButtonID</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.StepNextButtonImageUrl</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.StepNextButtonStyle</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.StepNextButtonText</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.StepNextButtonType</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.StepPreviousButtonID</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.StepPreviousButtonImageUrl</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.StepPreviousButtonStyle</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.StepPreviousButtonText</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.StepPreviousButtonType</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.StepStyle</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.TagKey</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.TrackViewState</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.WizardStepPlaceholderId</code> | ✅ 一致 |
| <code>Wizard.Api.WebControls.Wizard.WizardSteps</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>Wizard.(case list)</code> | ✅ 一致 |
| <code>Wizard.(default)</code> | ✅ 一致 |
| <code>Wizard.AccessKey=empty</code> | ✅ 一致 |
| <code>Wizard.AccessKey=japanese</code> | ✅ 一致 |
| <code>Wizard.AccessKey=markup</code> | ✅ 一致 |
| <code>Wizard.AccessKey=text</code> | ✅ 一致 |
| <code>Wizard.ActiveStepIndex=-1</code> | ✅ 一致 |
| <code>Wizard.ActiveStepIndex=0</code> | ✅ 一致 |
| <code>Wizard.ActiveStepIndex=1</code> | ✅ 一致 |
| <code>Wizard.ActiveStepIndex=1+CellPadding=3+CellSpacing=3</code> | ✅ 一致 |
| <code>Wizard.ActiveStepIndex=1+DisplaySideBar=false</code> | ✅ 一致 |
| <code>Wizard.ActiveStepIndex=1+EnableViewState=false</code> | ✅ 一致 |
| <code>Wizard.ActiveStepIndex=1+StepNextButtonType=Image+StepNextButtonImageUrl=app-relative</code> | ✅ 一致 |
| <code>Wizard.ActiveStepIndex=1+Visible=false</code> | ✅ 一致 |
| <code>Wizard.ActiveStepIndex=2</code> | ✅ 一致 |
| <code>Wizard.ActiveStepIndex=2+FinishCompleteButtonType=Link</code> | ✅ 一致 |
| <code>Wizard.ActiveStepIndex=3</code> | ✅ 一致 |
| <code>Wizard.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>Wizard.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>Wizard.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>Wizard.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>Wizard.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>Wizard.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>Wizard.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>Wizard.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>Wizard.BackColor=#3366CC</code> | ✅ 一致 |
| <code>Wizard.BackColor=Red</code> | ✅ 一致 |
| <code>Wizard.BackColor=Transparent</code> | ✅ 一致 |
| <code>Wizard.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>Wizard.BorderColor=Red</code> | ✅ 一致 |
| <code>Wizard.BorderColor=Transparent</code> | ✅ 一致 |
| <code>Wizard.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>Wizard.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>Wizard.BorderStyle=Double</code> | ✅ 一致 |
| <code>Wizard.BorderStyle=Groove</code> | ✅ 一致 |
| <code>Wizard.BorderStyle=Inset</code> | ✅ 一致 |
| <code>Wizard.BorderStyle=None</code> | ✅ 一致 |
| <code>Wizard.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>Wizard.BorderStyle=Outset</code> | ✅ 一致 |
| <code>Wizard.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>Wizard.BorderStyle=Solid</code> | ✅ 一致 |
| <code>Wizard.BorderWidth=120px</code> | ✅ 一致 |
| <code>Wizard.BorderWidth=2em</code> | ✅ 一致 |
| <code>Wizard.BorderWidth=50%</code> | ✅ 一致 |
| <code>Wizard.CancelButtonImageUrl=app-relative</code> | ✅ 一致 |
| <code>Wizard.CancelButtonImageUrl=empty</code> | ✅ 一致 |
| <code>Wizard.CancelButtonImageUrl=japanese</code> | ✅ 一致 |
| <code>Wizard.CancelButtonImageUrl=markup</code> | ✅ 一致 |
| <code>Wizard.CancelButtonImageUrl=text</code> | ✅ 一致 |
| <code>Wizard.CancelButtonStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Wizard.CancelButtonStyle.CssClass=text</code> | ✅ 一致 |
| <code>Wizard.CancelButtonStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Wizard.CancelButtonStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Wizard.CancelButtonStyle.Height=120px</code> | ✅ 一致 |
| <code>Wizard.CancelButtonText=empty</code> | ✅ 一致 |
| <code>Wizard.CancelButtonText=japanese</code> | ✅ 一致 |
| <code>Wizard.CancelButtonText=markup</code> | ✅ 一致 |
| <code>Wizard.CancelButtonText=text</code> | ✅ 一致 |
| <code>Wizard.CancelButtonType=Button</code> | ✅ 一致 |
| <code>Wizard.CancelButtonType=Image</code> | ✅ 一致 |
| <code>Wizard.CancelButtonType=Link</code> | ✅ 一致 |
| <code>Wizard.CancelDestinationPageUrl=app-relative</code> | ✅ 一致 |
| <code>Wizard.CancelDestinationPageUrl=empty</code> | ✅ 一致 |
| <code>Wizard.CancelDestinationPageUrl=japanese</code> | ✅ 一致 |
| <code>Wizard.CancelDestinationPageUrl=markup</code> | ✅ 一致 |
| <code>Wizard.CancelDestinationPageUrl=text</code> | ✅ 一致 |
| <code>Wizard.CellPadding=-1</code> | ✅ 一致 |
| <code>Wizard.CellPadding=0</code> | ✅ 一致 |
| <code>Wizard.CellPadding=3</code> | ✅ 一致 |
| <code>Wizard.CellSpacing=-1</code> | ✅ 一致 |
| <code>Wizard.CellSpacing=0</code> | ✅ 一致 |
| <code>Wizard.CellSpacing=3</code> | ✅ 一致 |
| <code>Wizard.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Wizard.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Wizard.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Wizard.ClientIDMode=Static</code> | ✅ 一致 |
| <code>Wizard.CssClass=empty</code> | ✅ 一致 |
| <code>Wizard.CssClass=japanese</code> | ✅ 一致 |
| <code>Wizard.CssClass=markup</code> | ✅ 一致 |
| <code>Wizard.CssClass=text</code> | ✅ 一致 |
| <code>Wizard.DisplayCancelButton=false</code> | ✅ 一致 |
| <code>Wizard.DisplayCancelButton=true</code> | ✅ 一致 |
| <code>Wizard.DisplayCancelButton=true+CancelButtonText=markup</code> | ✅ 一致 |
| <code>Wizard.DisplayCancelButton=true+CancelButtonType=Image+CancelButtonImageUrl=app-relative</code> | ✅ 一致 |
| <code>Wizard.DisplaySideBar=false</code> | ✅ 一致 |
| <code>Wizard.DisplaySideBar=false+HeaderText=text</code> | ✅ 一致 |
| <code>Wizard.DisplaySideBar=true</code> | ✅ 一致 |
| <code>Wizard.EnableTheming=false</code> | ✅ 一致 |
| <code>Wizard.EnableTheming=true</code> | ✅ 一致 |
| <code>Wizard.EnableViewState=false</code> | ✅ 一致 |
| <code>Wizard.EnableViewState=true</code> | ✅ 一致 |
| <code>Wizard.Enabled=false</code> | ✅ 一致 |
| <code>Wizard.Enabled=false+ActiveStepIndex=1</code> | ✅ 一致 |
| <code>Wizard.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>Wizard.Enabled=true</code> | ✅ 一致 |
| <code>Wizard.FinishCompleteButtonImageUrl=app-relative</code> | ✅ 一致 |
| <code>Wizard.FinishCompleteButtonImageUrl=empty</code> | ✅ 一致 |
| <code>Wizard.FinishCompleteButtonImageUrl=japanese</code> | ✅ 一致 |
| <code>Wizard.FinishCompleteButtonImageUrl=markup</code> | ✅ 一致 |
| <code>Wizard.FinishCompleteButtonImageUrl=text</code> | ✅ 一致 |
| <code>Wizard.FinishCompleteButtonStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Wizard.FinishCompleteButtonStyle.CssClass=text</code> | ✅ 一致 |
| <code>Wizard.FinishCompleteButtonStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Wizard.FinishCompleteButtonStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Wizard.FinishCompleteButtonStyle.Height=120px</code> | ✅ 一致 |
| <code>Wizard.FinishCompleteButtonText=empty</code> | ✅ 一致 |
| <code>Wizard.FinishCompleteButtonText=japanese</code> | ✅ 一致 |
| <code>Wizard.FinishCompleteButtonText=markup</code> | ✅ 一致 |
| <code>Wizard.FinishCompleteButtonText=text</code> | ✅ 一致 |
| <code>Wizard.FinishCompleteButtonType=Button</code> | ✅ 一致 |
| <code>Wizard.FinishCompleteButtonType=Image</code> | ✅ 一致 |
| <code>Wizard.FinishCompleteButtonType=Link</code> | ✅ 一致 |
| <code>Wizard.FinishDestinationPageUrl=app-relative</code> | ✅ 一致 |
| <code>Wizard.FinishDestinationPageUrl=empty</code> | ✅ 一致 |
| <code>Wizard.FinishDestinationPageUrl=japanese</code> | ✅ 一致 |
| <code>Wizard.FinishDestinationPageUrl=markup</code> | ✅ 一致 |
| <code>Wizard.FinishDestinationPageUrl=text</code> | ✅ 一致 |
| <code>Wizard.FinishPreviousButtonImageUrl=app-relative</code> | ✅ 一致 |
| <code>Wizard.FinishPreviousButtonImageUrl=empty</code> | ✅ 一致 |
| <code>Wizard.FinishPreviousButtonImageUrl=japanese</code> | ✅ 一致 |
| <code>Wizard.FinishPreviousButtonImageUrl=markup</code> | ✅ 一致 |
| <code>Wizard.FinishPreviousButtonImageUrl=text</code> | ✅ 一致 |
| <code>Wizard.FinishPreviousButtonStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Wizard.FinishPreviousButtonStyle.CssClass=text</code> | ✅ 一致 |
| <code>Wizard.FinishPreviousButtonStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Wizard.FinishPreviousButtonStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Wizard.FinishPreviousButtonStyle.Height=120px</code> | ✅ 一致 |
| <code>Wizard.FinishPreviousButtonText=empty</code> | ✅ 一致 |
| <code>Wizard.FinishPreviousButtonText=japanese</code> | ✅ 一致 |
| <code>Wizard.FinishPreviousButtonText=markup</code> | ✅ 一致 |
| <code>Wizard.FinishPreviousButtonText=text</code> | ✅ 一致 |
| <code>Wizard.FinishPreviousButtonType=Button</code> | ✅ 一致 |
| <code>Wizard.FinishPreviousButtonType=Image</code> | ✅ 一致 |
| <code>Wizard.FinishPreviousButtonType=Link</code> | ✅ 一致 |
| <code>Wizard.Font.Bold=false</code> | ✅ 一致 |
| <code>Wizard.Font.Bold=true</code> | ✅ 一致 |
| <code>Wizard.Font.Italic=false</code> | ✅ 一致 |
| <code>Wizard.Font.Italic=true</code> | ✅ 一致 |
| <code>Wizard.Font.Name=empty</code> | ✅ 一致 |
| <code>Wizard.Font.Name=japanese</code> | ✅ 一致 |
| <code>Wizard.Font.Name=markup</code> | ✅ 一致 |
| <code>Wizard.Font.Name=text</code> | ✅ 一致 |
| <code>Wizard.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>Wizard.Font.Overline=false</code> | ✅ 一致 |
| <code>Wizard.Font.Overline=true</code> | ✅ 一致 |
| <code>Wizard.Font.Size=12pt</code> | ✅ 一致 |
| <code>Wizard.Font.Size=150%</code> | ✅ 一致 |
| <code>Wizard.Font.Size=Large</code> | ✅ 一致 |
| <code>Wizard.Font.Strikeout=false</code> | ✅ 一致 |
| <code>Wizard.Font.Strikeout=true</code> | ✅ 一致 |
| <code>Wizard.Font.Underline=false</code> | ✅ 一致 |
| <code>Wizard.Font.Underline=true</code> | ✅ 一致 |
| <code>Wizard.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>Wizard.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>Wizard.ForeColor=Red</code> | ✅ 一致 |
| <code>Wizard.ForeColor=Transparent</code> | ✅ 一致 |
| <code>Wizard.HeaderStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Wizard.HeaderStyle.CssClass=text</code> | ✅ 一致 |
| <code>Wizard.HeaderStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Wizard.HeaderStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Wizard.HeaderStyle.Height=120px</code> | ✅ 一致 |
| <code>Wizard.HeaderStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>Wizard.HeaderStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>Wizard.HeaderStyle.Wrap=true</code> | ✅ 一致 |
| <code>Wizard.HeaderText=empty</code> | ✅ 一致 |
| <code>Wizard.HeaderText=japanese</code> | ✅ 一致 |
| <code>Wizard.HeaderText=markup</code> | ✅ 一致 |
| <code>Wizard.HeaderText=markup+HeaderStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Wizard.HeaderText=text</code> | ✅ 一致 |
| <code>Wizard.Height=120px</code> | ✅ 一致 |
| <code>Wizard.Height=2em</code> | ✅ 一致 |
| <code>Wizard.Height=50%</code> | ✅ 一致 |
| <code>Wizard.ID=empty</code> | ✅ 一致 |
| <code>Wizard.ID=japanese</code> | ✅ 一致 |
| <code>Wizard.ID=markup</code> | ✅ 一致 |
| <code>Wizard.ID=text</code> | ✅ 一致 |
| <code>Wizard.NavigationButtonStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Wizard.NavigationButtonStyle.CssClass=text</code> | ✅ 一致 |
| <code>Wizard.NavigationButtonStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Wizard.NavigationButtonStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Wizard.NavigationButtonStyle.Height=120px</code> | ✅ 一致 |
| <code>Wizard.NavigationStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Wizard.NavigationStyle.CssClass=text</code> | ✅ 一致 |
| <code>Wizard.NavigationStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Wizard.NavigationStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Wizard.NavigationStyle.Height=120px</code> | ✅ 一致 |
| <code>Wizard.NavigationStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>Wizard.NavigationStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>Wizard.NavigationStyle.Wrap=true</code> | ✅ 一致 |
| <code>Wizard.SideBarButtonStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Wizard.SideBarButtonStyle.CssClass=text</code> | ✅ 一致 |
| <code>Wizard.SideBarButtonStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Wizard.SideBarButtonStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Wizard.SideBarButtonStyle.Height=120px</code> | ✅ 一致 |
| <code>Wizard.SideBarStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Wizard.SideBarStyle.BackColor=Red+SideBarButtonStyle.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>Wizard.SideBarStyle.CssClass=text</code> | ✅ 一致 |
| <code>Wizard.SideBarStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Wizard.SideBarStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Wizard.SideBarStyle.Height=120px</code> | ✅ 一致 |
| <code>Wizard.SideBarStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>Wizard.SideBarStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>Wizard.SideBarStyle.Wrap=true</code> | ✅ 一致 |
| <code>Wizard.SkinID=empty</code> | ✅ 一致 |
| <code>Wizard.SkinID=japanese</code> | ✅ 一致 |
| <code>Wizard.SkinID=markup</code> | ✅ 一致 |
| <code>Wizard.SkinID=text</code> | ✅ 一致 |
| <code>Wizard.SkipLinkText=empty</code> | ✅ 一致 |
| <code>Wizard.SkipLinkText=japanese</code> | ✅ 一致 |
| <code>Wizard.SkipLinkText=markup</code> | ✅ 一致 |
| <code>Wizard.SkipLinkText=text</code> | ✅ 一致 |
| <code>Wizard.StartNextButtonImageUrl=app-relative</code> | ✅ 一致 |
| <code>Wizard.StartNextButtonImageUrl=empty</code> | ✅ 一致 |
| <code>Wizard.StartNextButtonImageUrl=japanese</code> | ✅ 一致 |
| <code>Wizard.StartNextButtonImageUrl=markup</code> | ✅ 一致 |
| <code>Wizard.StartNextButtonImageUrl=text</code> | ✅ 一致 |
| <code>Wizard.StartNextButtonStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Wizard.StartNextButtonStyle.CssClass=text</code> | ✅ 一致 |
| <code>Wizard.StartNextButtonStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Wizard.StartNextButtonStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Wizard.StartNextButtonStyle.Height=120px</code> | ✅ 一致 |
| <code>Wizard.StartNextButtonText=empty</code> | ✅ 一致 |
| <code>Wizard.StartNextButtonText=japanese</code> | ✅ 一致 |
| <code>Wizard.StartNextButtonText=markup</code> | ✅ 一致 |
| <code>Wizard.StartNextButtonText=text</code> | ✅ 一致 |
| <code>Wizard.StartNextButtonType=Button</code> | ✅ 一致 |
| <code>Wizard.StartNextButtonType=Image</code> | ✅ 一致 |
| <code>Wizard.StartNextButtonType=Link</code> | ✅ 一致 |
| <code>Wizard.StartNextButtonType=Link+StartNextButtonText=markup</code> | ✅ 一致 |
| <code>Wizard.StepNextButtonImageUrl=app-relative</code> | ✅ 一致 |
| <code>Wizard.StepNextButtonImageUrl=empty</code> | ✅ 一致 |
| <code>Wizard.StepNextButtonImageUrl=japanese</code> | ✅ 一致 |
| <code>Wizard.StepNextButtonImageUrl=markup</code> | ✅ 一致 |
| <code>Wizard.StepNextButtonImageUrl=text</code> | ✅ 一致 |
| <code>Wizard.StepNextButtonStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Wizard.StepNextButtonStyle.CssClass=text</code> | ✅ 一致 |
| <code>Wizard.StepNextButtonStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Wizard.StepNextButtonStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Wizard.StepNextButtonStyle.Height=120px</code> | ✅ 一致 |
| <code>Wizard.StepNextButtonText=empty</code> | ✅ 一致 |
| <code>Wizard.StepNextButtonText=japanese</code> | ✅ 一致 |
| <code>Wizard.StepNextButtonText=markup</code> | ✅ 一致 |
| <code>Wizard.StepNextButtonText=text</code> | ✅ 一致 |
| <code>Wizard.StepNextButtonType=Button</code> | ✅ 一致 |
| <code>Wizard.StepNextButtonType=Image</code> | ✅ 一致 |
| <code>Wizard.StepNextButtonType=Link</code> | ✅ 一致 |
| <code>Wizard.StepPreviousButtonImageUrl=app-relative</code> | ✅ 一致 |
| <code>Wizard.StepPreviousButtonImageUrl=empty</code> | ✅ 一致 |
| <code>Wizard.StepPreviousButtonImageUrl=japanese</code> | ✅ 一致 |
| <code>Wizard.StepPreviousButtonImageUrl=markup</code> | ✅ 一致 |
| <code>Wizard.StepPreviousButtonImageUrl=text</code> | ✅ 一致 |
| <code>Wizard.StepPreviousButtonStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Wizard.StepPreviousButtonStyle.CssClass=text</code> | ✅ 一致 |
| <code>Wizard.StepPreviousButtonStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Wizard.StepPreviousButtonStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Wizard.StepPreviousButtonStyle.Height=120px</code> | ✅ 一致 |
| <code>Wizard.StepPreviousButtonText=empty</code> | ✅ 一致 |
| <code>Wizard.StepPreviousButtonText=japanese</code> | ✅ 一致 |
| <code>Wizard.StepPreviousButtonText=markup</code> | ✅ 一致 |
| <code>Wizard.StepPreviousButtonText=text</code> | ✅ 一致 |
| <code>Wizard.StepPreviousButtonType=Button</code> | ✅ 一致 |
| <code>Wizard.StepPreviousButtonType=Image</code> | ✅ 一致 |
| <code>Wizard.StepPreviousButtonType=Link</code> | ✅ 一致 |
| <code>Wizard.StepStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Wizard.StepStyle.CssClass=text</code> | ✅ 一致 |
| <code>Wizard.StepStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Wizard.StepStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Wizard.StepStyle.Height=120px</code> | ✅ 一致 |
| <code>Wizard.StepStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>Wizard.StepStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>Wizard.StepStyle.Wrap=true</code> | ✅ 一致 |
| <code>Wizard.Style[color]=1em</code> | ✅ 一致 |
| <code>Wizard.Style[color]=red</code> | ✅ 一致 |
| <code>Wizard.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>Wizard.Style[margin-left]=red</code> | ✅ 一致 |
| <code>Wizard.TabIndex=-1</code> | ✅ 一致 |
| <code>Wizard.TabIndex=0</code> | ✅ 一致 |
| <code>Wizard.TabIndex=3</code> | ✅ 一致 |
| <code>Wizard.ToolTip=empty</code> | ✅ 一致 |
| <code>Wizard.ToolTip=japanese</code> | ✅ 一致 |
| <code>Wizard.ToolTip=markup</code> | ✅ 一致 |
| <code>Wizard.ToolTip=text</code> | ✅ 一致 |
| <code>Wizard.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>Wizard.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>Wizard.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>Wizard.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>Wizard.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>Wizard.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>Wizard.Visible=false</code> | ✅ 一致 |
| <code>Wizard.Visible=true</code> | ✅ 一致 |
| <code>Wizard.Width=120px</code> | ✅ 一致 |
| <code>Wizard.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>Wizard.Width=2em</code> | ✅ 一致 |
| <code>Wizard.Width=50%</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].AllowReturn=false</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].AllowReturn=true</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].ClientIDMode=Static</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].EnableTheming=false</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].EnableTheming=true</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].EnableViewState=false</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].EnableViewState=true</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].ID=empty</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].ID=japanese</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].ID=markup</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].ID=text</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].SkinID=empty</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].SkinID=japanese</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].SkinID=markup</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].SkinID=text</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].StepType=Auto</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].StepType=Complete</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].StepType=Finish</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].StepType=Start</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].StepType=Step</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].Title=empty</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].Title=japanese</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].Title=markup</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].Title=text</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].Visible=false</code> | ✅ 一致 |
| <code>Wizard.WizardSteps[0].Visible=true</code> | ✅ 一致 |
| <code>Wizard[layout].(default)</code> | ✅ 一致 |
| <code>Wizard[layout].ActiveStepIndex=1</code> | ✅ 一致 |
| <code>Wizard[naming container].(default)</code> | ✅ 一致 |
| <code>Wizard[naming container].ActiveStepIndex=1</code> | ✅ 一致 |
| <code>Wizard[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Wizard[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Wizard[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Wizard[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>Wizard[naming container].ID=text</code> | ✅ 一致 |
| <code>Wizard[templates].(default)</code> | ✅ 一致 |
| <code>Wizard[templates].ActiveStepIndex=1</code> | ✅ 一致 |
| <code>Wizard[templates].HeaderText=text</code> | ✅ 一致 |
| <code>Wizard[themed].(default)</code> | ✅ 一致 |
| <code>Wizard[themed].CssClass=text</code> | ✅ 一致 |
| <code>Wizard[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>Wizard[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| Wizard [render][databind][dynamic][templates] first request, anonymous: the login forms, the status, the views, the wizards | ✅ 一致 |
| Wizard [viewstate] a login with a wrong password: its error | ✅ 一致 |
| Wizard [viewstate] logged in (redirected to the page): the name, the status, the role&#39;s view | ✅ 一致 |
| Wizard [viewstate] the password changed: the success template | ✅ 一致 |
| Wizard [viewstate] logged out by the status | ✅ 一致 |
| Wizard [viewstate] a recovery for a user who does not exist | ✅ 一致 |
| Wizard [viewstate] the user&#39;s question | ✅ 一致 |
| Wizard [viewstate] the answer right: the password reset, its mail prepared (cancelled), the success template | ✅ 一致 |
| Wizard [viewstate] a user created: the complete step | ✅ 一致 |
| Wizard [viewstate][templates] the wizard&#39;s next step (the templated one) | ✅ 一致 |
| Wizard [viewstate] next, then previous | ✅ 一致 |
| Wizard [viewstate] a step chosen in the side bar | ✅ 一致 |
| Wizard [viewstate] finished: the complete step, the first step&#39;s value kept | ✅ 一致 |
| Wizard [dynamic][viewstate] a wizard made in code: its next step | ✅ 一致 |
| Wizard [visible][viewstate] a hidden wizard shown | ✅ 一致 |

</details>
