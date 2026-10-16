# WebPartZone パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 03:25
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 247 | 247 | 0 | 0 |
| ② HTML | 265 | 265 | 0 | 0 |
| ③ シナリオ | 12 | 12 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 256 件中 256 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 136 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 12 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 253 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 8 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 2 | 0 | ✅ |
| ポストバックイベント | 0 | 3 | ✅ |
| テンプレート | 0 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>WebPartZone.Api.Control.#ctor</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.Adapter</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.ClientID</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.Context</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.Controls</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.DataBind</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.Dispose</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.Disposed</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.Events</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.FindControl</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.Focus</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.HasControls</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.ID</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.Init</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.Load</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.OnInit</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.Page</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.Parent</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.PreRender</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.Render</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.Site</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.SkinID</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.Unload</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.ViewState</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>WebPartZone.Api.Control.Visible</code> | ✅ 一致 |
| <code>WebPartZone.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>WebPartZone.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>WebPartZone.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>WebPartZone.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>WebPartZone.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>WebPartZone.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>WebPartZone.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>WebPartZone.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>WebPartZone.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>WebPartZone.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>WebPartZone.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>WebPartZone.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>WebPartZone.Api.IPostBackEventHandler.RaisePostBackEvent</code> | ✅ 一致 |
| <code>WebPartZone.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.CompositeControl.#ctor</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.CompositeControl.Controls</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.CompositeControl.DataBind</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.CompositeControl.RecreateChildControls</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.CompositeControl.Render</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.CompositeControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.ICompositeControlDesignerAccessor.RecreateChildControls</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebControl.Width</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZone.#ctor</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZone.GetInitialWebParts</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZone.OnInit</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZone.ZoneTemplate</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.#ctor</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.AllowLayoutChange</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.BorderColor</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.BorderStyle</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.BorderWidth</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.CloseVerb</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.CloseWebPart</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.ConnectVerb</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.ConnectWebPart</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.CreateChildControls</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.CreateControlCollection</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.CreateControlStyle</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.CreateVerbs</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.CreateWebPartChrome</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.DeleteVerb</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.DeleteWebPart</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.DisplayTitle</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.DragDropEnabled</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.DragHighlightColor</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.EditVerb</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.EditWebPart</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.EmptyZoneText</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.ExportVerb</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.GetEffectiveChromeType</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.GetInitialWebParts</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.HasFooter</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.HasHeader</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.HelpVerb</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.LayoutOrientation</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.LoadViewState</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.MenuCheckImageStyle</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.MenuCheckImageUrl</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.MenuLabelHoverStyle</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.MenuLabelStyle</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.MenuLabelText</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.MenuPopupImageUrl</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.MenuPopupStyle</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.MenuVerbHoverStyle</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.MenuVerbStyle</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.MinimizeVerb</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.MinimizeWebPart</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.OnCreateVerbs</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.OnPreRender</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.RaisePostBackEvent</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.Render</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.RenderBody</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.RenderDropCue</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.RenderHeader</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.RestoreVerb</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.RestoreWebPart</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.SaveViewState</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.SelectedPartChromeStyle</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.ShowTitleIcons</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.TitleBarVerbButtonType</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.TitleBarVerbStyle</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.TrackViewState</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.VerbButtonType</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.WebPartChrome</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.WebPartVerbRenderMode</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebPartZoneBase.WebParts</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.BackImageUrl</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.EmptyZoneText</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.EmptyZoneTextStyle</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.ErrorStyle</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.FooterStyle</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.GetEffectiveChromeType</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.HasFooter</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.HasHeader</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.HeaderStyle</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.HeaderText</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.LoadViewState</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.OnInit</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.OnPreRender</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.Padding</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.PartChromePadding</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.PartChromeStyle</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.PartChromeType</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.PartStyle</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.PartTitleStyle</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.RenderBeginTag</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.RenderBody</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.RenderClientScript</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.RenderContents</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.RenderFooter</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.RenderHeader</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.SaveViewState</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.TagKey</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.TrackViewState</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.VerbButtonType</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.VerbStyle</code> | ✅ 一致 |
| <code>WebPartZone.Api.WebControls.WebParts.WebZone.WebPartManager</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>WebPartZone.(case list)</code> | ✅ 一致 |
| <code>WebPartZone.(default)</code> | ✅ 一致 |
| <code>WebPartZone.AccessKey=empty</code> | ✅ 一致 |
| <code>WebPartZone.AccessKey=japanese</code> | ✅ 一致 |
| <code>WebPartZone.AccessKey=k</code> | ✅ 一致 |
| <code>WebPartZone.AccessKey=markup</code> | ✅ 一致 |
| <code>WebPartZone.AccessKey=text</code> | ✅ 一致 |
| <code>WebPartZone.AllowLayoutChange=false</code> | ✅ 一致 |
| <code>WebPartZone.AllowLayoutChange=false+DragHighlightColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.AllowLayoutChange=true</code> | ✅ 一致 |
| <code>WebPartZone.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>WebPartZone.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>WebPartZone.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>WebPartZone.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>WebPartZone.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>WebPartZone.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>WebPartZone.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>WebPartZone.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>WebPartZone.BackColor=#3366CC</code> | ✅ 一致 |
| <code>WebPartZone.BackColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.BackColor=Transparent</code> | ✅ 一致 |
| <code>WebPartZone.BackImageUrl=empty</code> | ✅ 一致 |
| <code>WebPartZone.BackImageUrl=japanese</code> | ✅ 一致 |
| <code>WebPartZone.BackImageUrl=markup</code> | ✅ 一致 |
| <code>WebPartZone.BackImageUrl=text</code> | ✅ 一致 |
| <code>WebPartZone.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>WebPartZone.BorderColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.BorderColor=Transparent</code> | ✅ 一致 |
| <code>WebPartZone.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>WebPartZone.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>WebPartZone.BorderStyle=Double</code> | ✅ 一致 |
| <code>WebPartZone.BorderStyle=Groove</code> | ✅ 一致 |
| <code>WebPartZone.BorderStyle=Inset</code> | ✅ 一致 |
| <code>WebPartZone.BorderStyle=None</code> | ✅ 一致 |
| <code>WebPartZone.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>WebPartZone.BorderStyle=Outset</code> | ✅ 一致 |
| <code>WebPartZone.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>WebPartZone.BorderStyle=Solid</code> | ✅ 一致 |
| <code>WebPartZone.BorderWidth=120px</code> | ✅ 一致 |
| <code>WebPartZone.BorderWidth=2em</code> | ✅ 一致 |
| <code>WebPartZone.BorderWidth=50%</code> | ✅ 一致 |
| <code>WebPartZone.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>WebPartZone.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>WebPartZone.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>WebPartZone.ClientIDMode=Static</code> | ✅ 一致 |
| <code>WebPartZone.CssClass=empty</code> | ✅ 一致 |
| <code>WebPartZone.CssClass=japanese</code> | ✅ 一致 |
| <code>WebPartZone.CssClass=markup</code> | ✅ 一致 |
| <code>WebPartZone.CssClass=text</code> | ✅ 一致 |
| <code>WebPartZone.DragHighlightColor=#3366CC</code> | ✅ 一致 |
| <code>WebPartZone.DragHighlightColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.DragHighlightColor=Transparent</code> | ✅ 一致 |
| <code>WebPartZone.EmptyZoneText=empty</code> | ✅ 一致 |
| <code>WebPartZone.EmptyZoneText=japanese</code> | ✅ 一致 |
| <code>WebPartZone.EmptyZoneText=markup</code> | ✅ 一致 |
| <code>WebPartZone.EmptyZoneText=text</code> | ✅ 一致 |
| <code>WebPartZone.EmptyZoneTextStyle.BackColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.EmptyZoneTextStyle.CssClass=text</code> | ✅ 一致 |
| <code>WebPartZone.EmptyZoneTextStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>WebPartZone.EmptyZoneTextStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.EmptyZoneTextStyle.Height=120px</code> | ✅ 一致 |
| <code>WebPartZone.EnableTheming=false</code> | ✅ 一致 |
| <code>WebPartZone.EnableTheming=true</code> | ✅ 一致 |
| <code>WebPartZone.EnableViewState=false</code> | ✅ 一致 |
| <code>WebPartZone.EnableViewState=true</code> | ✅ 一致 |
| <code>WebPartZone.Enabled=false</code> | ✅ 一致 |
| <code>WebPartZone.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>WebPartZone.Enabled=true</code> | ✅ 一致 |
| <code>WebPartZone.ErrorStyle.BackColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.ErrorStyle.CssClass=text</code> | ✅ 一致 |
| <code>WebPartZone.ErrorStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>WebPartZone.ErrorStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.ErrorStyle.Height=120px</code> | ✅ 一致 |
| <code>WebPartZone.Font.Bold=false</code> | ✅ 一致 |
| <code>WebPartZone.Font.Bold=true</code> | ✅ 一致 |
| <code>WebPartZone.Font.Italic=false</code> | ✅ 一致 |
| <code>WebPartZone.Font.Italic=true</code> | ✅ 一致 |
| <code>WebPartZone.Font.Name=empty</code> | ✅ 一致 |
| <code>WebPartZone.Font.Name=japanese</code> | ✅ 一致 |
| <code>WebPartZone.Font.Name=markup</code> | ✅ 一致 |
| <code>WebPartZone.Font.Name=text</code> | ✅ 一致 |
| <code>WebPartZone.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>WebPartZone.Font.Overline=false</code> | ✅ 一致 |
| <code>WebPartZone.Font.Overline=true</code> | ✅ 一致 |
| <code>WebPartZone.Font.Size=12pt</code> | ✅ 一致 |
| <code>WebPartZone.Font.Size=150%</code> | ✅ 一致 |
| <code>WebPartZone.Font.Size=Large</code> | ✅ 一致 |
| <code>WebPartZone.Font.Strikeout=false</code> | ✅ 一致 |
| <code>WebPartZone.Font.Strikeout=true</code> | ✅ 一致 |
| <code>WebPartZone.Font.Underline=false</code> | ✅ 一致 |
| <code>WebPartZone.Font.Underline=true</code> | ✅ 一致 |
| <code>WebPartZone.FooterStyle.BackColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.FooterStyle.CssClass=text</code> | ✅ 一致 |
| <code>WebPartZone.FooterStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>WebPartZone.FooterStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.FooterStyle.Height=120px</code> | ✅ 一致 |
| <code>WebPartZone.FooterStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>WebPartZone.FooterStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>WebPartZone.FooterStyle.Wrap=true</code> | ✅ 一致 |
| <code>WebPartZone.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>WebPartZone.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>WebPartZone.ForeColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.ForeColor=Transparent</code> | ✅ 一致 |
| <code>WebPartZone.HeaderStyle.BackColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.HeaderStyle.BackColor=Red+PartStyle.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>WebPartZone.HeaderStyle.CssClass=text</code> | ✅ 一致 |
| <code>WebPartZone.HeaderStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>WebPartZone.HeaderStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.HeaderStyle.Height=120px</code> | ✅ 一致 |
| <code>WebPartZone.HeaderStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>WebPartZone.HeaderStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>WebPartZone.HeaderStyle.Wrap=true</code> | ✅ 一致 |
| <code>WebPartZone.HeaderText=empty</code> | ✅ 一致 |
| <code>WebPartZone.HeaderText=japanese</code> | ✅ 一致 |
| <code>WebPartZone.HeaderText=markup</code> | ✅ 一致 |
| <code>WebPartZone.HeaderText=markup+EmptyZoneText=markup</code> | ✅ 一致 |
| <code>WebPartZone.HeaderText=text</code> | ✅ 一致 |
| <code>WebPartZone.Height=120px</code> | ✅ 一致 |
| <code>WebPartZone.Height=2em</code> | ✅ 一致 |
| <code>WebPartZone.Height=50%</code> | ✅ 一致 |
| <code>WebPartZone.ID=empty</code> | ✅ 一致 |
| <code>WebPartZone.ID=japanese</code> | ✅ 一致 |
| <code>WebPartZone.ID=markup</code> | ✅ 一致 |
| <code>WebPartZone.ID=text</code> | ✅ 一致 |
| <code>WebPartZone.LayoutOrientation=Horizontal</code> | ✅ 一致 |
| <code>WebPartZone.LayoutOrientation=Horizontal+ShowTitleIcons=false</code> | ✅ 一致 |
| <code>WebPartZone.LayoutOrientation=Vertical</code> | ✅ 一致 |
| <code>WebPartZone.MenuCheckImageStyle.BackColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.MenuCheckImageStyle.CssClass=text</code> | ✅ 一致 |
| <code>WebPartZone.MenuCheckImageStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>WebPartZone.MenuCheckImageStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.MenuCheckImageStyle.Height=120px</code> | ✅ 一致 |
| <code>WebPartZone.MenuCheckImageUrl=empty</code> | ✅ 一致 |
| <code>WebPartZone.MenuCheckImageUrl=japanese</code> | ✅ 一致 |
| <code>WebPartZone.MenuCheckImageUrl=markup</code> | ✅ 一致 |
| <code>WebPartZone.MenuCheckImageUrl=text</code> | ✅ 一致 |
| <code>WebPartZone.MenuLabelHoverStyle.BackColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.MenuLabelHoverStyle.CssClass=text</code> | ✅ 一致 |
| <code>WebPartZone.MenuLabelHoverStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>WebPartZone.MenuLabelHoverStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.MenuLabelHoverStyle.Height=120px</code> | ✅ 一致 |
| <code>WebPartZone.MenuLabelStyle.BackColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.MenuLabelStyle.CssClass=text</code> | ✅ 一致 |
| <code>WebPartZone.MenuLabelStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>WebPartZone.MenuLabelStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.MenuLabelStyle.Height=120px</code> | ✅ 一致 |
| <code>WebPartZone.MenuLabelText=empty</code> | ✅ 一致 |
| <code>WebPartZone.MenuLabelText=japanese</code> | ✅ 一致 |
| <code>WebPartZone.MenuLabelText=markup</code> | ✅ 一致 |
| <code>WebPartZone.MenuLabelText=text</code> | ✅ 一致 |
| <code>WebPartZone.MenuPopupImageUrl=empty</code> | ✅ 一致 |
| <code>WebPartZone.MenuPopupImageUrl=japanese</code> | ✅ 一致 |
| <code>WebPartZone.MenuPopupImageUrl=markup</code> | ✅ 一致 |
| <code>WebPartZone.MenuPopupImageUrl=text</code> | ✅ 一致 |
| <code>WebPartZone.MenuPopupStyle.BackColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.MenuPopupStyle.CssClass=text</code> | ✅ 一致 |
| <code>WebPartZone.MenuPopupStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>WebPartZone.MenuPopupStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.MenuPopupStyle.Height=120px</code> | ✅ 一致 |
| <code>WebPartZone.MenuPopupStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>WebPartZone.MenuVerbHoverStyle.BackColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.MenuVerbHoverStyle.CssClass=text</code> | ✅ 一致 |
| <code>WebPartZone.MenuVerbHoverStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>WebPartZone.MenuVerbHoverStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.MenuVerbHoverStyle.Height=120px</code> | ✅ 一致 |
| <code>WebPartZone.MenuVerbStyle.BackColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.MenuVerbStyle.CssClass=text</code> | ✅ 一致 |
| <code>WebPartZone.MenuVerbStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>WebPartZone.MenuVerbStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.MenuVerbStyle.Height=120px</code> | ✅ 一致 |
| <code>WebPartZone.Padding=-1</code> | ✅ 一致 |
| <code>WebPartZone.Padding=0</code> | ✅ 一致 |
| <code>WebPartZone.Padding=3</code> | ✅ 一致 |
| <code>WebPartZone.PartChromePadding=120px</code> | ✅ 一致 |
| <code>WebPartZone.PartChromePadding=2em</code> | ✅ 一致 |
| <code>WebPartZone.PartChromePadding=50%</code> | ✅ 一致 |
| <code>WebPartZone.PartChromeStyle.BackColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.PartChromeStyle.CssClass=text</code> | ✅ 一致 |
| <code>WebPartZone.PartChromeStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>WebPartZone.PartChromeStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.PartChromeStyle.Height=120px</code> | ✅ 一致 |
| <code>WebPartZone.PartChromeType=BorderOnly</code> | ✅ 一致 |
| <code>WebPartZone.PartChromeType=BorderOnly+PartTitleStyle.BackColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.PartChromeType=Default</code> | ✅ 一致 |
| <code>WebPartZone.PartChromeType=None</code> | ✅ 一致 |
| <code>WebPartZone.PartChromeType=TitleAndBorder</code> | ✅ 一致 |
| <code>WebPartZone.PartChromeType=TitleOnly</code> | ✅ 一致 |
| <code>WebPartZone.PartStyle.BackColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.PartStyle.CssClass=text</code> | ✅ 一致 |
| <code>WebPartZone.PartStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>WebPartZone.PartStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.PartStyle.Height=120px</code> | ✅ 一致 |
| <code>WebPartZone.PartStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>WebPartZone.PartTitleStyle.BackColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.PartTitleStyle.CssClass=text</code> | ✅ 一致 |
| <code>WebPartZone.PartTitleStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>WebPartZone.PartTitleStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.PartTitleStyle.Height=120px</code> | ✅ 一致 |
| <code>WebPartZone.PartTitleStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>WebPartZone.PartTitleStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>WebPartZone.PartTitleStyle.Wrap=true</code> | ✅ 一致 |
| <code>WebPartZone.SelectedPartChromeStyle.BackColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.SelectedPartChromeStyle.CssClass=text</code> | ✅ 一致 |
| <code>WebPartZone.SelectedPartChromeStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>WebPartZone.SelectedPartChromeStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.SelectedPartChromeStyle.Height=120px</code> | ✅ 一致 |
| <code>WebPartZone.ShowTitleIcons=false</code> | ✅ 一致 |
| <code>WebPartZone.ShowTitleIcons=true</code> | ✅ 一致 |
| <code>WebPartZone.SkinID=empty</code> | ✅ 一致 |
| <code>WebPartZone.SkinID=japanese</code> | ✅ 一致 |
| <code>WebPartZone.SkinID=markup</code> | ✅ 一致 |
| <code>WebPartZone.SkinID=text</code> | ✅ 一致 |
| <code>WebPartZone.Style[color]=1em</code> | ✅ 一致 |
| <code>WebPartZone.Style[color]=red</code> | ✅ 一致 |
| <code>WebPartZone.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>WebPartZone.Style[margin-left]=red</code> | ✅ 一致 |
| <code>WebPartZone.TabIndex=-1</code> | ✅ 一致 |
| <code>WebPartZone.TabIndex=0</code> | ✅ 一致 |
| <code>WebPartZone.TabIndex=3</code> | ✅ 一致 |
| <code>WebPartZone.TitleBarVerbButtonType=Button</code> | ✅ 一致 |
| <code>WebPartZone.TitleBarVerbButtonType=Image</code> | ✅ 一致 |
| <code>WebPartZone.TitleBarVerbButtonType=Link</code> | ✅ 一致 |
| <code>WebPartZone.TitleBarVerbStyle.BackColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.TitleBarVerbStyle.CssClass=text</code> | ✅ 一致 |
| <code>WebPartZone.TitleBarVerbStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>WebPartZone.TitleBarVerbStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.TitleBarVerbStyle.Height=120px</code> | ✅ 一致 |
| <code>WebPartZone.ToolTip=empty</code> | ✅ 一致 |
| <code>WebPartZone.ToolTip=japanese</code> | ✅ 一致 |
| <code>WebPartZone.ToolTip=markup</code> | ✅ 一致 |
| <code>WebPartZone.ToolTip=text</code> | ✅ 一致 |
| <code>WebPartZone.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>WebPartZone.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>WebPartZone.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>WebPartZone.VerbButtonType=Button</code> | ✅ 一致 |
| <code>WebPartZone.VerbButtonType=Image</code> | ✅ 一致 |
| <code>WebPartZone.VerbButtonType=Link</code> | ✅ 一致 |
| <code>WebPartZone.VerbStyle.BackColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.VerbStyle.CssClass=text</code> | ✅ 一致 |
| <code>WebPartZone.VerbStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>WebPartZone.VerbStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>WebPartZone.VerbStyle.Height=120px</code> | ✅ 一致 |
| <code>WebPartZone.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>WebPartZone.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>WebPartZone.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>WebPartZone.Visible=false</code> | ✅ 一致 |
| <code>WebPartZone.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>WebPartZone.Visible=true</code> | ✅ 一致 |
| <code>WebPartZone.WebPartVerbRenderMode=Menu</code> | ✅ 一致 |
| <code>WebPartZone.WebPartVerbRenderMode=TitleBar</code> | ✅ 一致 |
| <code>WebPartZone.WebPartVerbRenderMode=TitleBar+MenuLabelText=text</code> | ✅ 一致 |
| <code>WebPartZone.Width=120px</code> | ✅ 一致 |
| <code>WebPartZone.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>WebPartZone.Width=2em</code> | ✅ 一致 |
| <code>WebPartZone.Width=50%</code> | ✅ 一致 |
| <code>WebPartZone[naming container].(default)</code> | ✅ 一致 |
| <code>WebPartZone[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>WebPartZone[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>WebPartZone[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>WebPartZone[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>WebPartZone[naming container].ID=text</code> | ✅ 一致 |
| <code>WebPartZone[themed].(default)</code> | ✅ 一致 |
| <code>WebPartZone[themed].CssClass=text</code> | ✅ 一致 |
| <code>WebPartZone[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>WebPartZone[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| WebPartZone [render][databind][dynamic][templates] first request (browse): the parts in their zones, a zone made in code | ✅ 一致 |
| WebPartZone [viewstate] the design mode | ✅ 一致 |
| WebPartZone [viewstate] the catalog mode: the catalog zone and its parts | ✅ 一致 |
| WebPartZone [viewstate] a part added from the catalog (its check box posted, the zone&#39;s Add) | ✅ 一致 |
| WebPartZone [viewstate] the edit mode: the editor zone and its parts for the first part | ✅ 一致 |
| WebPartZone [viewstate] the first part&#39;s title edited (the editor zone&#39;s OK) | ✅ 一致 |
| WebPartZone [viewstate] the connect mode for the first part | ✅ 一致 |
| WebPartZone [viewstate] the connections zone closed | ✅ 一致 |
| WebPartZone [postbackevent] a part minimized by its zone&#39;s verb | ✅ 一致 |
| WebPartZone [postbackevent] restored | ✅ 一致 |
| WebPartZone [dynamic][postbackevent] a part of the zone made in code minimized | ✅ 一致 |
| WebPartZone [visible][viewstate] a hidden zone shown | ✅ 一致 |

</details>
