# EditorZone パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 01:50
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 235 | 235 | 0 | 0 |
| ② HTML | 222 | 222 | 0 | 0 |
| ③ シナリオ | 12 | 12 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 245 件中 245 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 109 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 12 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 210 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 10 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 1 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 2 | 0 | ✅ |
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
| <code>EditorZone.Api.Control.#ctor</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.Adapter</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.ClientID</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.Context</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.Controls</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.DataBind</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.Dispose</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.Disposed</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.Events</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.FindControl</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.Focus</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.HasControls</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.ID</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.Init</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.Load</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.OnInit</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.Page</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.Parent</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.PreRender</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.Render</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.Site</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.SkinID</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.Unload</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.ViewState</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>EditorZone.Api.Control.Visible</code> | ✅ 一致 |
| <code>EditorZone.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>EditorZone.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>EditorZone.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>EditorZone.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>EditorZone.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>EditorZone.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>EditorZone.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>EditorZone.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>EditorZone.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>EditorZone.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>EditorZone.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>EditorZone.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>EditorZone.Api.IPostBackEventHandler.RaisePostBackEvent</code> | ✅ 一致 |
| <code>EditorZone.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.CompositeControl.#ctor</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.CompositeControl.Controls</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.CompositeControl.DataBind</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.CompositeControl.RecreateChildControls</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.CompositeControl.Render</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.CompositeControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.ICompositeControlDesignerAccessor.RecreateChildControls</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebControl.Width</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZone.#ctor</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZone.CreateEditorParts</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZone.ZoneTemplate</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZoneBase.#ctor</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZoneBase.ApplyVerb</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZoneBase.CancelVerb</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZoneBase.Close</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZoneBase.CreateChildControls</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZoneBase.CreateEditorPartChrome</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZoneBase.CreateEditorParts</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZoneBase.Display</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZoneBase.EditorPartChrome</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZoneBase.EditorParts</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZoneBase.EmptyZoneText</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZoneBase.ErrorText</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZoneBase.HeaderText</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZoneBase.InstructionText</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZoneBase.InvalidateEditorParts</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZoneBase.LoadViewState</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZoneBase.OKVerb</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZoneBase.OnDisplayModeChanged</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZoneBase.OnPreRender</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZoneBase.OnSelectedWebPartChanged</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZoneBase.RaisePostBackEvent</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZoneBase.Render</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZoneBase.RenderBody</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZoneBase.RenderVerbs</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZoneBase.SaveViewState</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZoneBase.TrackViewState</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.EditorZoneBase.WebPartToEdit</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.ToolZone.#ctor</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.ToolZone.AssociatedDisplayModes</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.ToolZone.Close</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.ToolZone.Display</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.ToolZone.EditUIStyle</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.ToolZone.HeaderCloseVerb</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.ToolZone.HeaderVerbStyle</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.ToolZone.InstructionText</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.ToolZone.InstructionTextStyle</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.ToolZone.LabelStyle</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.ToolZone.LoadViewState</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.ToolZone.OnDisplayModeChanged</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.ToolZone.OnInit</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.ToolZone.OnSelectedWebPartChanged</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.ToolZone.RaisePostBackEvent</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.ToolZone.RenderFooter</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.ToolZone.RenderHeader</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.ToolZone.RenderVerb</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.ToolZone.RenderVerbs</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.ToolZone.SaveViewState</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.ToolZone.TrackViewState</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.ToolZone.Visible</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.BackImageUrl</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.EmptyZoneText</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.EmptyZoneTextStyle</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.ErrorStyle</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.FooterStyle</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.GetEffectiveChromeType</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.HasFooter</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.HasHeader</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.HeaderStyle</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.HeaderText</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.LoadViewState</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.OnInit</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.OnPreRender</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.Padding</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.PartChromePadding</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.PartChromeStyle</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.PartChromeType</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.PartStyle</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.PartTitleStyle</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.RenderBeginTag</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.RenderBody</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.RenderClientScript</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.RenderContents</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.RenderFooter</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.RenderHeader</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.SaveViewState</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.TagKey</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.TrackViewState</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.VerbButtonType</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.VerbStyle</code> | ✅ 一致 |
| <code>EditorZone.Api.WebControls.WebParts.WebZone.WebPartManager</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>EditorZone.(case list)</code> | ✅ 一致 |
| <code>EditorZone.(default)</code> | ✅ 一致 |
| <code>EditorZone.AccessKey=empty</code> | ✅ 一致 |
| <code>EditorZone.AccessKey=japanese</code> | ✅ 一致 |
| <code>EditorZone.AccessKey=k</code> | ✅ 一致 |
| <code>EditorZone.AccessKey=markup</code> | ✅ 一致 |
| <code>EditorZone.AccessKey=text</code> | ✅ 一致 |
| <code>EditorZone.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>EditorZone.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>EditorZone.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>EditorZone.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>EditorZone.ApplyVerb.Text=text+OKVerb.Text=markup</code> | ✅ 一致 |
| <code>EditorZone.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>EditorZone.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>EditorZone.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>EditorZone.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>EditorZone.BackColor=#3366CC</code> | ✅ 一致 |
| <code>EditorZone.BackColor=Red</code> | ✅ 一致 |
| <code>EditorZone.BackColor=Transparent</code> | ✅ 一致 |
| <code>EditorZone.BackImageUrl=empty</code> | ✅ 一致 |
| <code>EditorZone.BackImageUrl=japanese</code> | ✅ 一致 |
| <code>EditorZone.BackImageUrl=markup</code> | ✅ 一致 |
| <code>EditorZone.BackImageUrl=text</code> | ✅ 一致 |
| <code>EditorZone.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>EditorZone.BorderColor=Red</code> | ✅ 一致 |
| <code>EditorZone.BorderColor=Transparent</code> | ✅ 一致 |
| <code>EditorZone.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>EditorZone.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>EditorZone.BorderStyle=Double</code> | ✅ 一致 |
| <code>EditorZone.BorderStyle=Groove</code> | ✅ 一致 |
| <code>EditorZone.BorderStyle=Inset</code> | ✅ 一致 |
| <code>EditorZone.BorderStyle=None</code> | ✅ 一致 |
| <code>EditorZone.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>EditorZone.BorderStyle=Outset</code> | ✅ 一致 |
| <code>EditorZone.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>EditorZone.BorderStyle=Solid</code> | ✅ 一致 |
| <code>EditorZone.BorderWidth=120px</code> | ✅ 一致 |
| <code>EditorZone.BorderWidth=2em</code> | ✅ 一致 |
| <code>EditorZone.BorderWidth=50%</code> | ✅ 一致 |
| <code>EditorZone.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>EditorZone.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>EditorZone.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>EditorZone.ClientIDMode=Static</code> | ✅ 一致 |
| <code>EditorZone.CssClass=empty</code> | ✅ 一致 |
| <code>EditorZone.CssClass=japanese</code> | ✅ 一致 |
| <code>EditorZone.CssClass=markup</code> | ✅ 一致 |
| <code>EditorZone.CssClass=text</code> | ✅ 一致 |
| <code>EditorZone.EditUIStyle.BackColor=Red</code> | ✅ 一致 |
| <code>EditorZone.EditUIStyle.CssClass=text</code> | ✅ 一致 |
| <code>EditorZone.EditUIStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>EditorZone.EditUIStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>EditorZone.EditUIStyle.Height=120px</code> | ✅ 一致 |
| <code>EditorZone.EmptyZoneText=empty</code> | ✅ 一致 |
| <code>EditorZone.EmptyZoneText=japanese</code> | ✅ 一致 |
| <code>EditorZone.EmptyZoneText=markup</code> | ✅ 一致 |
| <code>EditorZone.EmptyZoneText=text</code> | ✅ 一致 |
| <code>EditorZone.EmptyZoneTextStyle.BackColor=Red</code> | ✅ 一致 |
| <code>EditorZone.EmptyZoneTextStyle.CssClass=text</code> | ✅ 一致 |
| <code>EditorZone.EmptyZoneTextStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>EditorZone.EmptyZoneTextStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>EditorZone.EmptyZoneTextStyle.Height=120px</code> | ✅ 一致 |
| <code>EditorZone.EnableTheming=false</code> | ✅ 一致 |
| <code>EditorZone.EnableTheming=true</code> | ✅ 一致 |
| <code>EditorZone.EnableViewState=false</code> | ✅ 一致 |
| <code>EditorZone.EnableViewState=true</code> | ✅ 一致 |
| <code>EditorZone.Enabled=false</code> | ✅ 一致 |
| <code>EditorZone.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>EditorZone.Enabled=true</code> | ✅ 一致 |
| <code>EditorZone.ErrorStyle.BackColor=Red</code> | ✅ 一致 |
| <code>EditorZone.ErrorStyle.CssClass=text</code> | ✅ 一致 |
| <code>EditorZone.ErrorStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>EditorZone.ErrorStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>EditorZone.ErrorStyle.Height=120px</code> | ✅ 一致 |
| <code>EditorZone.ErrorText=empty</code> | ✅ 一致 |
| <code>EditorZone.ErrorText=japanese</code> | ✅ 一致 |
| <code>EditorZone.ErrorText=markup</code> | ✅ 一致 |
| <code>EditorZone.ErrorText=text</code> | ✅ 一致 |
| <code>EditorZone.Font.Bold=false</code> | ✅ 一致 |
| <code>EditorZone.Font.Bold=true</code> | ✅ 一致 |
| <code>EditorZone.Font.Italic=false</code> | ✅ 一致 |
| <code>EditorZone.Font.Italic=true</code> | ✅ 一致 |
| <code>EditorZone.Font.Name=empty</code> | ✅ 一致 |
| <code>EditorZone.Font.Name=japanese</code> | ✅ 一致 |
| <code>EditorZone.Font.Name=markup</code> | ✅ 一致 |
| <code>EditorZone.Font.Name=text</code> | ✅ 一致 |
| <code>EditorZone.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>EditorZone.Font.Overline=false</code> | ✅ 一致 |
| <code>EditorZone.Font.Overline=true</code> | ✅ 一致 |
| <code>EditorZone.Font.Size=12pt</code> | ✅ 一致 |
| <code>EditorZone.Font.Size=150%</code> | ✅ 一致 |
| <code>EditorZone.Font.Size=Large</code> | ✅ 一致 |
| <code>EditorZone.Font.Strikeout=false</code> | ✅ 一致 |
| <code>EditorZone.Font.Strikeout=true</code> | ✅ 一致 |
| <code>EditorZone.Font.Underline=false</code> | ✅ 一致 |
| <code>EditorZone.Font.Underline=true</code> | ✅ 一致 |
| <code>EditorZone.FooterStyle.BackColor=Red</code> | ✅ 一致 |
| <code>EditorZone.FooterStyle.CssClass=text</code> | ✅ 一致 |
| <code>EditorZone.FooterStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>EditorZone.FooterStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>EditorZone.FooterStyle.Height=120px</code> | ✅ 一致 |
| <code>EditorZone.FooterStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>EditorZone.FooterStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>EditorZone.FooterStyle.Wrap=true</code> | ✅ 一致 |
| <code>EditorZone.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>EditorZone.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>EditorZone.ForeColor=Red</code> | ✅ 一致 |
| <code>EditorZone.ForeColor=Transparent</code> | ✅ 一致 |
| <code>EditorZone.HeaderStyle.BackColor=Red</code> | ✅ 一致 |
| <code>EditorZone.HeaderStyle.CssClass=text</code> | ✅ 一致 |
| <code>EditorZone.HeaderStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>EditorZone.HeaderStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>EditorZone.HeaderStyle.Height=120px</code> | ✅ 一致 |
| <code>EditorZone.HeaderStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>EditorZone.HeaderStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>EditorZone.HeaderStyle.Wrap=true</code> | ✅ 一致 |
| <code>EditorZone.HeaderText=empty</code> | ✅ 一致 |
| <code>EditorZone.HeaderText=japanese</code> | ✅ 一致 |
| <code>EditorZone.HeaderText=markup</code> | ✅ 一致 |
| <code>EditorZone.HeaderText=markup+InstructionText=markup</code> | ✅ 一致 |
| <code>EditorZone.HeaderText=text</code> | ✅ 一致 |
| <code>EditorZone.HeaderVerbStyle.BackColor=Red</code> | ✅ 一致 |
| <code>EditorZone.HeaderVerbStyle.CssClass=text</code> | ✅ 一致 |
| <code>EditorZone.HeaderVerbStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>EditorZone.HeaderVerbStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>EditorZone.HeaderVerbStyle.Height=120px</code> | ✅ 一致 |
| <code>EditorZone.Height=120px</code> | ✅ 一致 |
| <code>EditorZone.Height=2em</code> | ✅ 一致 |
| <code>EditorZone.Height=50%</code> | ✅ 一致 |
| <code>EditorZone.ID=empty</code> | ✅ 一致 |
| <code>EditorZone.ID=japanese</code> | ✅ 一致 |
| <code>EditorZone.ID=markup</code> | ✅ 一致 |
| <code>EditorZone.ID=text</code> | ✅ 一致 |
| <code>EditorZone.InstructionText=empty</code> | ✅ 一致 |
| <code>EditorZone.InstructionText=japanese</code> | ✅ 一致 |
| <code>EditorZone.InstructionText=markup</code> | ✅ 一致 |
| <code>EditorZone.InstructionText=text</code> | ✅ 一致 |
| <code>EditorZone.InstructionTextStyle.BackColor=Red</code> | ✅ 一致 |
| <code>EditorZone.InstructionTextStyle.CssClass=text</code> | ✅ 一致 |
| <code>EditorZone.InstructionTextStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>EditorZone.InstructionTextStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>EditorZone.InstructionTextStyle.Height=120px</code> | ✅ 一致 |
| <code>EditorZone.LabelStyle.BackColor=Red</code> | ✅ 一致 |
| <code>EditorZone.LabelStyle.CssClass=text</code> | ✅ 一致 |
| <code>EditorZone.LabelStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>EditorZone.LabelStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>EditorZone.LabelStyle.Height=120px</code> | ✅ 一致 |
| <code>EditorZone.Padding=-1</code> | ✅ 一致 |
| <code>EditorZone.Padding=0</code> | ✅ 一致 |
| <code>EditorZone.Padding=3</code> | ✅ 一致 |
| <code>EditorZone.PartChromePadding=120px</code> | ✅ 一致 |
| <code>EditorZone.PartChromePadding=2em</code> | ✅ 一致 |
| <code>EditorZone.PartChromePadding=50%</code> | ✅ 一致 |
| <code>EditorZone.PartChromeStyle.BackColor=Red</code> | ✅ 一致 |
| <code>EditorZone.PartChromeStyle.CssClass=text</code> | ✅ 一致 |
| <code>EditorZone.PartChromeStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>EditorZone.PartChromeStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>EditorZone.PartChromeStyle.Height=120px</code> | ✅ 一致 |
| <code>EditorZone.PartChromeType=BorderOnly</code> | ✅ 一致 |
| <code>EditorZone.PartChromeType=Default</code> | ✅ 一致 |
| <code>EditorZone.PartChromeType=None</code> | ✅ 一致 |
| <code>EditorZone.PartChromeType=TitleAndBorder</code> | ✅ 一致 |
| <code>EditorZone.PartChromeType=TitleOnly</code> | ✅ 一致 |
| <code>EditorZone.PartStyle.BackColor=Red</code> | ✅ 一致 |
| <code>EditorZone.PartStyle.CssClass=text</code> | ✅ 一致 |
| <code>EditorZone.PartStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>EditorZone.PartStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>EditorZone.PartStyle.Height=120px</code> | ✅ 一致 |
| <code>EditorZone.PartStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>EditorZone.PartTitleStyle.BackColor=Red</code> | ✅ 一致 |
| <code>EditorZone.PartTitleStyle.CssClass=text</code> | ✅ 一致 |
| <code>EditorZone.PartTitleStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>EditorZone.PartTitleStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>EditorZone.PartTitleStyle.Height=120px</code> | ✅ 一致 |
| <code>EditorZone.PartTitleStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>EditorZone.PartTitleStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>EditorZone.PartTitleStyle.Wrap=true</code> | ✅ 一致 |
| <code>EditorZone.SkinID=empty</code> | ✅ 一致 |
| <code>EditorZone.SkinID=japanese</code> | ✅ 一致 |
| <code>EditorZone.SkinID=markup</code> | ✅ 一致 |
| <code>EditorZone.SkinID=text</code> | ✅ 一致 |
| <code>EditorZone.Style[color]=1em</code> | ✅ 一致 |
| <code>EditorZone.Style[color]=red</code> | ✅ 一致 |
| <code>EditorZone.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>EditorZone.Style[margin-left]=red</code> | ✅ 一致 |
| <code>EditorZone.TabIndex=-1</code> | ✅ 一致 |
| <code>EditorZone.TabIndex=0</code> | ✅ 一致 |
| <code>EditorZone.TabIndex=3</code> | ✅ 一致 |
| <code>EditorZone.ToolTip=empty</code> | ✅ 一致 |
| <code>EditorZone.ToolTip=japanese</code> | ✅ 一致 |
| <code>EditorZone.ToolTip=markup</code> | ✅ 一致 |
| <code>EditorZone.ToolTip=text</code> | ✅ 一致 |
| <code>EditorZone.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>EditorZone.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>EditorZone.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>EditorZone.VerbButtonType=Button</code> | ✅ 一致 |
| <code>EditorZone.VerbButtonType=Image</code> | ✅ 一致 |
| <code>EditorZone.VerbButtonType=Link</code> | ✅ 一致 |
| <code>EditorZone.VerbStyle.BackColor=Red</code> | ✅ 一致 |
| <code>EditorZone.VerbStyle.CssClass=text</code> | ✅ 一致 |
| <code>EditorZone.VerbStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>EditorZone.VerbStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>EditorZone.VerbStyle.Height=120px</code> | ✅ 一致 |
| <code>EditorZone.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>EditorZone.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>EditorZone.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>EditorZone.Visible=false</code> | ✅ 一致 |
| <code>EditorZone.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>EditorZone.Visible=true</code> | ✅ 一致 |
| <code>EditorZone.Width=120px</code> | ✅ 一致 |
| <code>EditorZone.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>EditorZone.Width=2em</code> | ✅ 一致 |
| <code>EditorZone.Width=50%</code> | ✅ 一致 |
| <code>EditorZone[naming container].(default)</code> | ✅ 一致 |
| <code>EditorZone[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>EditorZone[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>EditorZone[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>EditorZone[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>EditorZone[naming container].ID=text</code> | ✅ 一致 |
| <code>EditorZone[themed].(default)</code> | ✅ 一致 |
| <code>EditorZone[themed].CssClass=text</code> | ✅ 一致 |
| <code>EditorZone[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>EditorZone[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| EditorZone [render][databind][dynamic] first request (browse): the parts in their zones, a zone made in code | ✅ 一致 |
| EditorZone [viewstate] the design mode | ✅ 一致 |
| EditorZone [viewstate] the catalog mode: the catalog zone and its parts | ✅ 一致 |
| EditorZone [viewstate] a part added from the catalog (its check box posted, the zone&#39;s Add) | ✅ 一致 |
| EditorZone [viewstate][templates] the edit mode: the editor zone and its parts for the first part | ✅ 一致 |
| EditorZone [postbackevent] the first part&#39;s title edited (the editor zone&#39;s OK) | ✅ 一致 |
| EditorZone [viewstate] the connect mode for the first part | ✅ 一致 |
| EditorZone [viewstate] the connections zone closed | ✅ 一致 |
| EditorZone [viewstate] a part minimized by its zone&#39;s verb | ✅ 一致 |
| EditorZone [viewstate] restored | ✅ 一致 |
| EditorZone [viewstate] a part of the zone made in code minimized | ✅ 一致 |
| EditorZone [visible][viewstate] a hidden zone shown | ✅ 一致 |

</details>
