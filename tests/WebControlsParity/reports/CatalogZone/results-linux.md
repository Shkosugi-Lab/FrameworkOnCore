# CatalogZone パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 01:31
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 242 | 242 | 0 | 0 |
| ② HTML | 238 | 238 | 0 | 0 |
| ③ シナリオ | 12 | 12 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 252 件中 252 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 121 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 12 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 226 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 10 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 1 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 2 | 0 | ✅ |
| ポストバックデータの取り込み | 0 | 1 | ✅ |
| リクエスト検証（ValidateRequestMode） | 3 | 0 | ✅ |
| ポストバックイベント | 0 | 1 | ✅ |
| テンプレート | 0 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>CatalogZone.Api.Control.#ctor</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.Adapter</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.ClientID</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.Context</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.Controls</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.DataBind</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.Dispose</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.Disposed</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.Events</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.FindControl</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.Focus</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.HasControls</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.ID</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.Init</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.Load</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.OnInit</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.Page</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.Parent</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.PreRender</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.Render</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.Site</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.SkinID</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.Unload</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.ViewState</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>CatalogZone.Api.Control.Visible</code> | ✅ 一致 |
| <code>CatalogZone.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>CatalogZone.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>CatalogZone.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>CatalogZone.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>CatalogZone.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>CatalogZone.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>CatalogZone.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>CatalogZone.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>CatalogZone.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>CatalogZone.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>CatalogZone.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>CatalogZone.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>CatalogZone.Api.IPostBackDataHandler.LoadPostData</code> | ✅ 一致 |
| <code>CatalogZone.Api.IPostBackDataHandler.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>CatalogZone.Api.IPostBackEventHandler.RaisePostBackEvent</code> | ✅ 一致 |
| <code>CatalogZone.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.CompositeControl.#ctor</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.CompositeControl.Controls</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.CompositeControl.DataBind</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.CompositeControl.RecreateChildControls</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.CompositeControl.Render</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.CompositeControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.ICompositeControlDesignerAccessor.RecreateChildControls</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebControl.Width</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZone.#ctor</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZone.CreateCatalogParts</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZone.ZoneTemplate</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.#ctor</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.AddVerb</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.CatalogPartChrome</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.CatalogParts</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.Close</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.CloseVerb</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.CreateCatalogPartChrome</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.CreateCatalogParts</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.CreateChildControls</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.EmptyZoneText</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.HeaderText</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.InstructionText</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.InvalidateCatalogParts</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.LoadControlState</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.LoadPostData</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.LoadViewState</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.OnInit</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.OnPreRender</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.PartLinkStyle</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.RaisePostBackEvent</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.Render</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.RenderBody</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.RenderCatalogPartLinks</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.RenderFooter</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.RenderVerbs</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.SaveControlState</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.SaveViewState</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.SelectTargetZoneText</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.SelectedCatalogPartID</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.SelectedPartLinkStyle</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.ShowCatalogIcons</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.CatalogZoneBase.TrackViewState</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.ToolZone.#ctor</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.ToolZone.AssociatedDisplayModes</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.ToolZone.Close</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.ToolZone.Display</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.ToolZone.EditUIStyle</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.ToolZone.HeaderCloseVerb</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.ToolZone.HeaderVerbStyle</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.ToolZone.InstructionText</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.ToolZone.InstructionTextStyle</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.ToolZone.LabelStyle</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.ToolZone.LoadViewState</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.ToolZone.OnDisplayModeChanged</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.ToolZone.OnInit</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.ToolZone.OnSelectedWebPartChanged</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.ToolZone.RaisePostBackEvent</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.ToolZone.RenderFooter</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.ToolZone.RenderHeader</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.ToolZone.RenderVerb</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.ToolZone.RenderVerbs</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.ToolZone.SaveViewState</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.ToolZone.TrackViewState</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.ToolZone.Visible</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.BackImageUrl</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.EmptyZoneText</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.EmptyZoneTextStyle</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.ErrorStyle</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.FooterStyle</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.GetEffectiveChromeType</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.HasFooter</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.HasHeader</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.HeaderStyle</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.HeaderText</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.LoadViewState</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.OnInit</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.OnPreRender</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.Padding</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.PartChromePadding</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.PartChromeStyle</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.PartChromeType</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.PartStyle</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.PartTitleStyle</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.RenderBeginTag</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.RenderBody</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.RenderClientScript</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.RenderContents</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.RenderFooter</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.RenderHeader</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.SaveViewState</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.TagKey</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.TrackViewState</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.VerbButtonType</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.VerbStyle</code> | ✅ 一致 |
| <code>CatalogZone.Api.WebControls.WebParts.WebZone.WebPartManager</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>CatalogZone.(case list)</code> | ✅ 一致 |
| <code>CatalogZone.(default)</code> | ✅ 一致 |
| <code>CatalogZone.AccessKey=empty</code> | ✅ 一致 |
| <code>CatalogZone.AccessKey=japanese</code> | ✅ 一致 |
| <code>CatalogZone.AccessKey=k</code> | ✅ 一致 |
| <code>CatalogZone.AccessKey=markup</code> | ✅ 一致 |
| <code>CatalogZone.AccessKey=text</code> | ✅ 一致 |
| <code>CatalogZone.AddVerb.Text=text+CloseVerb.Text=markup</code> | ✅ 一致 |
| <code>CatalogZone.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>CatalogZone.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>CatalogZone.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>CatalogZone.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>CatalogZone.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>CatalogZone.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>CatalogZone.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>CatalogZone.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>CatalogZone.BackColor=#3366CC</code> | ✅ 一致 |
| <code>CatalogZone.BackColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.BackColor=Transparent</code> | ✅ 一致 |
| <code>CatalogZone.BackImageUrl=empty</code> | ✅ 一致 |
| <code>CatalogZone.BackImageUrl=japanese</code> | ✅ 一致 |
| <code>CatalogZone.BackImageUrl=markup</code> | ✅ 一致 |
| <code>CatalogZone.BackImageUrl=text</code> | ✅ 一致 |
| <code>CatalogZone.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>CatalogZone.BorderColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.BorderColor=Transparent</code> | ✅ 一致 |
| <code>CatalogZone.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>CatalogZone.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>CatalogZone.BorderStyle=Double</code> | ✅ 一致 |
| <code>CatalogZone.BorderStyle=Groove</code> | ✅ 一致 |
| <code>CatalogZone.BorderStyle=Inset</code> | ✅ 一致 |
| <code>CatalogZone.BorderStyle=None</code> | ✅ 一致 |
| <code>CatalogZone.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>CatalogZone.BorderStyle=Outset</code> | ✅ 一致 |
| <code>CatalogZone.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>CatalogZone.BorderStyle=Solid</code> | ✅ 一致 |
| <code>CatalogZone.BorderWidth=120px</code> | ✅ 一致 |
| <code>CatalogZone.BorderWidth=2em</code> | ✅ 一致 |
| <code>CatalogZone.BorderWidth=50%</code> | ✅ 一致 |
| <code>CatalogZone.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>CatalogZone.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>CatalogZone.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>CatalogZone.ClientIDMode=Static</code> | ✅ 一致 |
| <code>CatalogZone.CssClass=empty</code> | ✅ 一致 |
| <code>CatalogZone.CssClass=japanese</code> | ✅ 一致 |
| <code>CatalogZone.CssClass=markup</code> | ✅ 一致 |
| <code>CatalogZone.CssClass=text</code> | ✅ 一致 |
| <code>CatalogZone.EditUIStyle.BackColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.EditUIStyle.CssClass=text</code> | ✅ 一致 |
| <code>CatalogZone.EditUIStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>CatalogZone.EditUIStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.EditUIStyle.Height=120px</code> | ✅ 一致 |
| <code>CatalogZone.EmptyZoneText=empty</code> | ✅ 一致 |
| <code>CatalogZone.EmptyZoneText=japanese</code> | ✅ 一致 |
| <code>CatalogZone.EmptyZoneText=markup</code> | ✅ 一致 |
| <code>CatalogZone.EmptyZoneText=text</code> | ✅ 一致 |
| <code>CatalogZone.EmptyZoneTextStyle.BackColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.EmptyZoneTextStyle.CssClass=text</code> | ✅ 一致 |
| <code>CatalogZone.EmptyZoneTextStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>CatalogZone.EmptyZoneTextStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.EmptyZoneTextStyle.Height=120px</code> | ✅ 一致 |
| <code>CatalogZone.EnableTheming=false</code> | ✅ 一致 |
| <code>CatalogZone.EnableTheming=true</code> | ✅ 一致 |
| <code>CatalogZone.EnableViewState=false</code> | ✅ 一致 |
| <code>CatalogZone.EnableViewState=true</code> | ✅ 一致 |
| <code>CatalogZone.Enabled=false</code> | ✅ 一致 |
| <code>CatalogZone.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>CatalogZone.Enabled=true</code> | ✅ 一致 |
| <code>CatalogZone.ErrorStyle.BackColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.ErrorStyle.CssClass=text</code> | ✅ 一致 |
| <code>CatalogZone.ErrorStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>CatalogZone.ErrorStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.ErrorStyle.Height=120px</code> | ✅ 一致 |
| <code>CatalogZone.Font.Bold=false</code> | ✅ 一致 |
| <code>CatalogZone.Font.Bold=true</code> | ✅ 一致 |
| <code>CatalogZone.Font.Italic=false</code> | ✅ 一致 |
| <code>CatalogZone.Font.Italic=true</code> | ✅ 一致 |
| <code>CatalogZone.Font.Name=empty</code> | ✅ 一致 |
| <code>CatalogZone.Font.Name=japanese</code> | ✅ 一致 |
| <code>CatalogZone.Font.Name=markup</code> | ✅ 一致 |
| <code>CatalogZone.Font.Name=text</code> | ✅ 一致 |
| <code>CatalogZone.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>CatalogZone.Font.Overline=false</code> | ✅ 一致 |
| <code>CatalogZone.Font.Overline=true</code> | ✅ 一致 |
| <code>CatalogZone.Font.Size=12pt</code> | ✅ 一致 |
| <code>CatalogZone.Font.Size=150%</code> | ✅ 一致 |
| <code>CatalogZone.Font.Size=Large</code> | ✅ 一致 |
| <code>CatalogZone.Font.Strikeout=false</code> | ✅ 一致 |
| <code>CatalogZone.Font.Strikeout=true</code> | ✅ 一致 |
| <code>CatalogZone.Font.Underline=false</code> | ✅ 一致 |
| <code>CatalogZone.Font.Underline=true</code> | ✅ 一致 |
| <code>CatalogZone.FooterStyle.BackColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.FooterStyle.CssClass=text</code> | ✅ 一致 |
| <code>CatalogZone.FooterStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>CatalogZone.FooterStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.FooterStyle.Height=120px</code> | ✅ 一致 |
| <code>CatalogZone.FooterStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>CatalogZone.FooterStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>CatalogZone.FooterStyle.Wrap=true</code> | ✅ 一致 |
| <code>CatalogZone.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>CatalogZone.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>CatalogZone.ForeColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.ForeColor=Transparent</code> | ✅ 一致 |
| <code>CatalogZone.HeaderStyle.BackColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.HeaderStyle.CssClass=text</code> | ✅ 一致 |
| <code>CatalogZone.HeaderStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>CatalogZone.HeaderStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.HeaderStyle.Height=120px</code> | ✅ 一致 |
| <code>CatalogZone.HeaderStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>CatalogZone.HeaderStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>CatalogZone.HeaderStyle.Wrap=true</code> | ✅ 一致 |
| <code>CatalogZone.HeaderText=empty</code> | ✅ 一致 |
| <code>CatalogZone.HeaderText=japanese</code> | ✅ 一致 |
| <code>CatalogZone.HeaderText=markup</code> | ✅ 一致 |
| <code>CatalogZone.HeaderText=markup+InstructionText=markup</code> | ✅ 一致 |
| <code>CatalogZone.HeaderText=text</code> | ✅ 一致 |
| <code>CatalogZone.HeaderVerbStyle.BackColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.HeaderVerbStyle.CssClass=text</code> | ✅ 一致 |
| <code>CatalogZone.HeaderVerbStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>CatalogZone.HeaderVerbStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.HeaderVerbStyle.Height=120px</code> | ✅ 一致 |
| <code>CatalogZone.Height=120px</code> | ✅ 一致 |
| <code>CatalogZone.Height=2em</code> | ✅ 一致 |
| <code>CatalogZone.Height=50%</code> | ✅ 一致 |
| <code>CatalogZone.ID=empty</code> | ✅ 一致 |
| <code>CatalogZone.ID=japanese</code> | ✅ 一致 |
| <code>CatalogZone.ID=markup</code> | ✅ 一致 |
| <code>CatalogZone.ID=text</code> | ✅ 一致 |
| <code>CatalogZone.InstructionText=empty</code> | ✅ 一致 |
| <code>CatalogZone.InstructionText=japanese</code> | ✅ 一致 |
| <code>CatalogZone.InstructionText=markup</code> | ✅ 一致 |
| <code>CatalogZone.InstructionText=text</code> | ✅ 一致 |
| <code>CatalogZone.InstructionTextStyle.BackColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.InstructionTextStyle.CssClass=text</code> | ✅ 一致 |
| <code>CatalogZone.InstructionTextStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>CatalogZone.InstructionTextStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.InstructionTextStyle.Height=120px</code> | ✅ 一致 |
| <code>CatalogZone.LabelStyle.BackColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.LabelStyle.CssClass=text</code> | ✅ 一致 |
| <code>CatalogZone.LabelStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>CatalogZone.LabelStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.LabelStyle.Height=120px</code> | ✅ 一致 |
| <code>CatalogZone.Padding=-1</code> | ✅ 一致 |
| <code>CatalogZone.Padding=0</code> | ✅ 一致 |
| <code>CatalogZone.Padding=3</code> | ✅ 一致 |
| <code>CatalogZone.PartChromePadding=120px</code> | ✅ 一致 |
| <code>CatalogZone.PartChromePadding=2em</code> | ✅ 一致 |
| <code>CatalogZone.PartChromePadding=50%</code> | ✅ 一致 |
| <code>CatalogZone.PartChromeStyle.BackColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.PartChromeStyle.CssClass=text</code> | ✅ 一致 |
| <code>CatalogZone.PartChromeStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>CatalogZone.PartChromeStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.PartChromeStyle.Height=120px</code> | ✅ 一致 |
| <code>CatalogZone.PartChromeType=BorderOnly</code> | ✅ 一致 |
| <code>CatalogZone.PartChromeType=Default</code> | ✅ 一致 |
| <code>CatalogZone.PartChromeType=None</code> | ✅ 一致 |
| <code>CatalogZone.PartChromeType=TitleAndBorder</code> | ✅ 一致 |
| <code>CatalogZone.PartChromeType=TitleOnly</code> | ✅ 一致 |
| <code>CatalogZone.PartLinkStyle.BackColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.PartLinkStyle.CssClass=text</code> | ✅ 一致 |
| <code>CatalogZone.PartLinkStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>CatalogZone.PartLinkStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.PartLinkStyle.Height=120px</code> | ✅ 一致 |
| <code>CatalogZone.PartStyle.BackColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.PartStyle.CssClass=text</code> | ✅ 一致 |
| <code>CatalogZone.PartStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>CatalogZone.PartStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.PartStyle.Height=120px</code> | ✅ 一致 |
| <code>CatalogZone.PartStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>CatalogZone.PartTitleStyle.BackColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.PartTitleStyle.CssClass=text</code> | ✅ 一致 |
| <code>CatalogZone.PartTitleStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>CatalogZone.PartTitleStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.PartTitleStyle.Height=120px</code> | ✅ 一致 |
| <code>CatalogZone.PartTitleStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>CatalogZone.PartTitleStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>CatalogZone.PartTitleStyle.Wrap=true</code> | ✅ 一致 |
| <code>CatalogZone.SelectTargetZoneText=empty</code> | ✅ 一致 |
| <code>CatalogZone.SelectTargetZoneText=japanese</code> | ✅ 一致 |
| <code>CatalogZone.SelectTargetZoneText=markup</code> | ✅ 一致 |
| <code>CatalogZone.SelectTargetZoneText=text</code> | ✅ 一致 |
| <code>CatalogZone.SelectedCatalogPartID=empty</code> | ✅ 一致 |
| <code>CatalogZone.SelectedCatalogPartID=japanese</code> | ✅ 一致 |
| <code>CatalogZone.SelectedCatalogPartID=markup</code> | ✅ 一致 |
| <code>CatalogZone.SelectedCatalogPartID=text</code> | ✅ 一致 |
| <code>CatalogZone.SelectedPartLinkStyle.BackColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.SelectedPartLinkStyle.CssClass=text</code> | ✅ 一致 |
| <code>CatalogZone.SelectedPartLinkStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>CatalogZone.SelectedPartLinkStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.SelectedPartLinkStyle.Height=120px</code> | ✅ 一致 |
| <code>CatalogZone.ShowCatalogIcons=false</code> | ✅ 一致 |
| <code>CatalogZone.ShowCatalogIcons=true</code> | ✅ 一致 |
| <code>CatalogZone.SkinID=empty</code> | ✅ 一致 |
| <code>CatalogZone.SkinID=japanese</code> | ✅ 一致 |
| <code>CatalogZone.SkinID=markup</code> | ✅ 一致 |
| <code>CatalogZone.SkinID=text</code> | ✅ 一致 |
| <code>CatalogZone.Style[color]=1em</code> | ✅ 一致 |
| <code>CatalogZone.Style[color]=red</code> | ✅ 一致 |
| <code>CatalogZone.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>CatalogZone.Style[margin-left]=red</code> | ✅ 一致 |
| <code>CatalogZone.TabIndex=-1</code> | ✅ 一致 |
| <code>CatalogZone.TabIndex=0</code> | ✅ 一致 |
| <code>CatalogZone.TabIndex=3</code> | ✅ 一致 |
| <code>CatalogZone.ToolTip=empty</code> | ✅ 一致 |
| <code>CatalogZone.ToolTip=japanese</code> | ✅ 一致 |
| <code>CatalogZone.ToolTip=markup</code> | ✅ 一致 |
| <code>CatalogZone.ToolTip=text</code> | ✅ 一致 |
| <code>CatalogZone.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>CatalogZone.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>CatalogZone.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>CatalogZone.VerbButtonType=Button</code> | ✅ 一致 |
| <code>CatalogZone.VerbButtonType=Image</code> | ✅ 一致 |
| <code>CatalogZone.VerbButtonType=Link</code> | ✅ 一致 |
| <code>CatalogZone.VerbStyle.BackColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.VerbStyle.CssClass=text</code> | ✅ 一致 |
| <code>CatalogZone.VerbStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>CatalogZone.VerbStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>CatalogZone.VerbStyle.Height=120px</code> | ✅ 一致 |
| <code>CatalogZone.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>CatalogZone.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>CatalogZone.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>CatalogZone.Visible=false</code> | ✅ 一致 |
| <code>CatalogZone.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>CatalogZone.Visible=true</code> | ✅ 一致 |
| <code>CatalogZone.Width=120px</code> | ✅ 一致 |
| <code>CatalogZone.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>CatalogZone.Width=2em</code> | ✅ 一致 |
| <code>CatalogZone.Width=50%</code> | ✅ 一致 |
| <code>CatalogZone[naming container].(default)</code> | ✅ 一致 |
| <code>CatalogZone[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>CatalogZone[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>CatalogZone[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>CatalogZone[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>CatalogZone[naming container].ID=text</code> | ✅ 一致 |
| <code>CatalogZone[themed].(default)</code> | ✅ 一致 |
| <code>CatalogZone[themed].CssClass=text</code> | ✅ 一致 |
| <code>CatalogZone[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>CatalogZone[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| CatalogZone [render][databind][dynamic] first request (browse): the parts in their zones, a zone made in code | ✅ 一致 |
| CatalogZone [viewstate] the design mode | ✅ 一致 |
| CatalogZone [viewstate][templates] the catalog mode: the catalog zone and its parts | ✅ 一致 |
| CatalogZone [postdata][postbackevent] a part added from the catalog (its check box posted, the zone&#39;s Add) | ✅ 一致 |
| CatalogZone [viewstate] the edit mode: the editor zone and its parts for the first part | ✅ 一致 |
| CatalogZone [viewstate] the first part&#39;s title edited (the editor zone&#39;s OK) | ✅ 一致 |
| CatalogZone [viewstate] the connect mode for the first part | ✅ 一致 |
| CatalogZone [viewstate] the connections zone closed | ✅ 一致 |
| CatalogZone [viewstate] a part minimized by its zone&#39;s verb | ✅ 一致 |
| CatalogZone [viewstate] restored | ✅ 一致 |
| CatalogZone [viewstate] a part of the zone made in code minimized | ✅ 一致 |
| CatalogZone [visible][viewstate] a hidden zone shown | ✅ 一致 |

</details>
