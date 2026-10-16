# PageCatalogPart パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 00:33
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 200 | 200 | 0 | 0 |
| ② HTML | 159 | 159 | 0 | 0 |
| ③ シナリオ | 12 | 12 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 209 件中 209 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 40 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 12 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 147 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 11 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 1 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 2 | 0 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>PageCatalogPart.Api.Control.#ctor</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.Adapter</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.ClientID</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.Context</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.Controls</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.DataBind</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.Dispose</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.Disposed</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.Events</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.FindControl</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.Focus</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.HasControls</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.ID</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.Init</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.Load</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.OnInit</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.Page</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.Parent</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.PreRender</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.Render</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.Site</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.SkinID</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.Unload</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.ViewState</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.Control.Visible</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.ICompositeControlDesignerAccessor.RecreateChildControls</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.Panel.#ctor</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.Panel.AddAttributesToRender</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.Panel.BackImageUrl</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.Panel.CreateControlStyle</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.Panel.DefaultButton</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.Panel.Direction</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.Panel.GroupingText</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.Panel.HorizontalAlign</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.Panel.RenderBeginTag</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.Panel.RenderEndTag</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.Panel.ScrollBars</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.Panel.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.Panel.Wrap</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebControl.Width</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.CatalogPart.#ctor</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.CatalogPart.DisplayTitle</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.CatalogPart.GetAvailableWebPartDescriptions</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.CatalogPart.GetDesignModeState</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.CatalogPart.GetWebPart</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.CatalogPart.OnPreRender</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.CatalogPart.SetDesignModeState</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.CatalogPart.WebPartManager</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.CatalogPart.Zone</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.PageCatalogPart.#ctor</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.PageCatalogPart.AccessKey</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.PageCatalogPart.BackColor</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.PageCatalogPart.BackImageUrl</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.PageCatalogPart.BorderColor</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.PageCatalogPart.BorderStyle</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.PageCatalogPart.BorderWidth</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.PageCatalogPart.CssClass</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.PageCatalogPart.DefaultButton</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.PageCatalogPart.Direction</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.PageCatalogPart.EnableTheming</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.PageCatalogPart.Enabled</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.PageCatalogPart.Font</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.PageCatalogPart.ForeColor</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.PageCatalogPart.GetAvailableWebPartDescriptions</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.PageCatalogPart.GetWebPart</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.PageCatalogPart.GroupingText</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.PageCatalogPart.Height</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.PageCatalogPart.HorizontalAlign</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.PageCatalogPart.ScrollBars</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.PageCatalogPart.SkinID</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.PageCatalogPart.TabIndex</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.PageCatalogPart.Title</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.PageCatalogPart.ToolTip</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.PageCatalogPart.Visible</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.PageCatalogPart.Width</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.PageCatalogPart.Wrap</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.Part.ChromeState</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.Part.ChromeType</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.Part.Controls</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.Part.DataBind</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.Part.Description</code> | ✅ 一致 |
| <code>PageCatalogPart.Api.WebControls.WebParts.Part.Title</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>PageCatalogPart.(case list)</code> | ✅ 一致 |
| <code>PageCatalogPart.(default)</code> | ✅ 一致 |
| <code>PageCatalogPart.AccessKey=empty</code> | ✅ 一致 |
| <code>PageCatalogPart.AccessKey=japanese</code> | ✅ 一致 |
| <code>PageCatalogPart.AccessKey=k</code> | ✅ 一致 |
| <code>PageCatalogPart.AccessKey=markup</code> | ✅ 一致 |
| <code>PageCatalogPart.AccessKey=text</code> | ✅ 一致 |
| <code>PageCatalogPart.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>PageCatalogPart.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>PageCatalogPart.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>PageCatalogPart.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>PageCatalogPart.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>PageCatalogPart.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>PageCatalogPart.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>PageCatalogPart.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>PageCatalogPart.BackColor=#3366CC</code> | ✅ 一致 |
| <code>PageCatalogPart.BackColor=Red</code> | ✅ 一致 |
| <code>PageCatalogPart.BackColor=Transparent</code> | ✅ 一致 |
| <code>PageCatalogPart.BackImageUrl=empty</code> | ✅ 一致 |
| <code>PageCatalogPart.BackImageUrl=japanese</code> | ✅ 一致 |
| <code>PageCatalogPart.BackImageUrl=markup</code> | ✅ 一致 |
| <code>PageCatalogPart.BackImageUrl=text</code> | ✅ 一致 |
| <code>PageCatalogPart.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>PageCatalogPart.BorderColor=Red</code> | ✅ 一致 |
| <code>PageCatalogPart.BorderColor=Transparent</code> | ✅ 一致 |
| <code>PageCatalogPart.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>PageCatalogPart.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>PageCatalogPart.BorderStyle=Double</code> | ✅ 一致 |
| <code>PageCatalogPart.BorderStyle=Groove</code> | ✅ 一致 |
| <code>PageCatalogPart.BorderStyle=Inset</code> | ✅ 一致 |
| <code>PageCatalogPart.BorderStyle=None</code> | ✅ 一致 |
| <code>PageCatalogPart.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>PageCatalogPart.BorderStyle=Outset</code> | ✅ 一致 |
| <code>PageCatalogPart.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>PageCatalogPart.BorderStyle=Solid</code> | ✅ 一致 |
| <code>PageCatalogPart.BorderWidth=120px</code> | ✅ 一致 |
| <code>PageCatalogPart.BorderWidth=2em</code> | ✅ 一致 |
| <code>PageCatalogPart.BorderWidth=50%</code> | ✅ 一致 |
| <code>PageCatalogPart.ChromeState=Minimized</code> | ✅ 一致 |
| <code>PageCatalogPart.ChromeState=Normal</code> | ✅ 一致 |
| <code>PageCatalogPart.ChromeType=BorderOnly</code> | ✅ 一致 |
| <code>PageCatalogPart.ChromeType=Default</code> | ✅ 一致 |
| <code>PageCatalogPart.ChromeType=None</code> | ✅ 一致 |
| <code>PageCatalogPart.ChromeType=TitleAndBorder</code> | ✅ 一致 |
| <code>PageCatalogPart.ChromeType=TitleOnly</code> | ✅ 一致 |
| <code>PageCatalogPart.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>PageCatalogPart.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>PageCatalogPart.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>PageCatalogPart.ClientIDMode=Static</code> | ✅ 一致 |
| <code>PageCatalogPart.CssClass=empty</code> | ✅ 一致 |
| <code>PageCatalogPart.CssClass=japanese</code> | ✅ 一致 |
| <code>PageCatalogPart.CssClass=markup</code> | ✅ 一致 |
| <code>PageCatalogPart.CssClass=text</code> | ✅ 一致 |
| <code>PageCatalogPart.DefaultButton=empty</code> | ✅ 一致 |
| <code>PageCatalogPart.DefaultButton=japanese</code> | ✅ 一致 |
| <code>PageCatalogPart.DefaultButton=markup</code> | ✅ 一致 |
| <code>PageCatalogPart.DefaultButton=text</code> | ✅ 一致 |
| <code>PageCatalogPart.Description=empty</code> | ✅ 一致 |
| <code>PageCatalogPart.Description=japanese</code> | ✅ 一致 |
| <code>PageCatalogPart.Description=markup</code> | ✅ 一致 |
| <code>PageCatalogPart.Description=text</code> | ✅ 一致 |
| <code>PageCatalogPart.Direction=LeftToRight</code> | ✅ 一致 |
| <code>PageCatalogPart.Direction=NotSet</code> | ✅ 一致 |
| <code>PageCatalogPart.Direction=RightToLeft</code> | ✅ 一致 |
| <code>PageCatalogPart.EnableTheming=false</code> | ✅ 一致 |
| <code>PageCatalogPart.EnableTheming=true</code> | ✅ 一致 |
| <code>PageCatalogPart.EnableViewState=false</code> | ✅ 一致 |
| <code>PageCatalogPart.EnableViewState=true</code> | ✅ 一致 |
| <code>PageCatalogPart.Enabled=false</code> | ✅ 一致 |
| <code>PageCatalogPart.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>PageCatalogPart.Enabled=true</code> | ✅ 一致 |
| <code>PageCatalogPart.Font.Bold=false</code> | ✅ 一致 |
| <code>PageCatalogPart.Font.Bold=true</code> | ✅ 一致 |
| <code>PageCatalogPart.Font.Italic=false</code> | ✅ 一致 |
| <code>PageCatalogPart.Font.Italic=true</code> | ✅ 一致 |
| <code>PageCatalogPart.Font.Name=empty</code> | ✅ 一致 |
| <code>PageCatalogPart.Font.Name=japanese</code> | ✅ 一致 |
| <code>PageCatalogPart.Font.Name=markup</code> | ✅ 一致 |
| <code>PageCatalogPart.Font.Name=text</code> | ✅ 一致 |
| <code>PageCatalogPart.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>PageCatalogPart.Font.Overline=false</code> | ✅ 一致 |
| <code>PageCatalogPart.Font.Overline=true</code> | ✅ 一致 |
| <code>PageCatalogPart.Font.Size=12pt</code> | ✅ 一致 |
| <code>PageCatalogPart.Font.Size=150%</code> | ✅ 一致 |
| <code>PageCatalogPart.Font.Size=Large</code> | ✅ 一致 |
| <code>PageCatalogPart.Font.Strikeout=false</code> | ✅ 一致 |
| <code>PageCatalogPart.Font.Strikeout=true</code> | ✅ 一致 |
| <code>PageCatalogPart.Font.Underline=false</code> | ✅ 一致 |
| <code>PageCatalogPart.Font.Underline=true</code> | ✅ 一致 |
| <code>PageCatalogPart.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>PageCatalogPart.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>PageCatalogPart.ForeColor=Red</code> | ✅ 一致 |
| <code>PageCatalogPart.ForeColor=Transparent</code> | ✅ 一致 |
| <code>PageCatalogPart.GroupingText=empty</code> | ✅ 一致 |
| <code>PageCatalogPart.GroupingText=japanese</code> | ✅ 一致 |
| <code>PageCatalogPart.GroupingText=markup</code> | ✅ 一致 |
| <code>PageCatalogPart.GroupingText=text</code> | ✅ 一致 |
| <code>PageCatalogPart.Height=120px</code> | ✅ 一致 |
| <code>PageCatalogPart.Height=2em</code> | ✅ 一致 |
| <code>PageCatalogPart.Height=50%</code> | ✅ 一致 |
| <code>PageCatalogPart.HorizontalAlign=Center</code> | ✅ 一致 |
| <code>PageCatalogPart.HorizontalAlign=Justify</code> | ✅ 一致 |
| <code>PageCatalogPart.HorizontalAlign=Left</code> | ✅ 一致 |
| <code>PageCatalogPart.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>PageCatalogPart.HorizontalAlign=Right</code> | ✅ 一致 |
| <code>PageCatalogPart.ID=empty</code> | ✅ 一致 |
| <code>PageCatalogPart.ID=japanese</code> | ✅ 一致 |
| <code>PageCatalogPart.ID=markup</code> | ✅ 一致 |
| <code>PageCatalogPart.ID=text</code> | ✅ 一致 |
| <code>PageCatalogPart.ScrollBars=Auto</code> | ✅ 一致 |
| <code>PageCatalogPart.ScrollBars=Both</code> | ✅ 一致 |
| <code>PageCatalogPart.ScrollBars=Horizontal</code> | ✅ 一致 |
| <code>PageCatalogPart.ScrollBars=None</code> | ✅ 一致 |
| <code>PageCatalogPart.ScrollBars=Vertical</code> | ✅ 一致 |
| <code>PageCatalogPart.SkinID=empty</code> | ✅ 一致 |
| <code>PageCatalogPart.SkinID=japanese</code> | ✅ 一致 |
| <code>PageCatalogPart.SkinID=markup</code> | ✅ 一致 |
| <code>PageCatalogPart.SkinID=text</code> | ✅ 一致 |
| <code>PageCatalogPart.Style[color]=1em</code> | ✅ 一致 |
| <code>PageCatalogPart.Style[color]=red</code> | ✅ 一致 |
| <code>PageCatalogPart.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>PageCatalogPart.Style[margin-left]=red</code> | ✅ 一致 |
| <code>PageCatalogPart.TabIndex=-1</code> | ✅ 一致 |
| <code>PageCatalogPart.TabIndex=0</code> | ✅ 一致 |
| <code>PageCatalogPart.TabIndex=3</code> | ✅ 一致 |
| <code>PageCatalogPart.Title=empty</code> | ✅ 一致 |
| <code>PageCatalogPart.Title=japanese</code> | ✅ 一致 |
| <code>PageCatalogPart.Title=markup</code> | ✅ 一致 |
| <code>PageCatalogPart.Title=markup+Description=text</code> | ✅ 一致 |
| <code>PageCatalogPart.Title=text</code> | ✅ 一致 |
| <code>PageCatalogPart.ToolTip=empty</code> | ✅ 一致 |
| <code>PageCatalogPart.ToolTip=japanese</code> | ✅ 一致 |
| <code>PageCatalogPart.ToolTip=markup</code> | ✅ 一致 |
| <code>PageCatalogPart.ToolTip=text</code> | ✅ 一致 |
| <code>PageCatalogPart.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>PageCatalogPart.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>PageCatalogPart.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>PageCatalogPart.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>PageCatalogPart.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>PageCatalogPart.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>PageCatalogPart.Visible=false</code> | ✅ 一致 |
| <code>PageCatalogPart.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>PageCatalogPart.Visible=true</code> | ✅ 一致 |
| <code>PageCatalogPart.Width=120px</code> | ✅ 一致 |
| <code>PageCatalogPart.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>PageCatalogPart.Width=2em</code> | ✅ 一致 |
| <code>PageCatalogPart.Width=50%</code> | ✅ 一致 |
| <code>PageCatalogPart.Wrap=false</code> | ✅ 一致 |
| <code>PageCatalogPart.Wrap=true</code> | ✅ 一致 |
| <code>PageCatalogPart[naming container].(default)</code> | ✅ 一致 |
| <code>PageCatalogPart[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>PageCatalogPart[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>PageCatalogPart[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>PageCatalogPart[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>PageCatalogPart[naming container].ID=text</code> | ✅ 一致 |
| <code>PageCatalogPart[themed].(default)</code> | ✅ 一致 |
| <code>PageCatalogPart[themed].CssClass=text</code> | ✅ 一致 |
| <code>PageCatalogPart[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>PageCatalogPart[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| PageCatalogPart [render][databind][dynamic] first request (browse): the parts in their zones, a zone made in code | ✅ 一致 |
| PageCatalogPart [viewstate] the design mode | ✅ 一致 |
| PageCatalogPart [viewstate][templates] the catalog mode: the catalog zone and its parts | ✅ 一致 |
| PageCatalogPart [viewstate] a part added from the catalog (its check box posted, the zone&#39;s Add) | ✅ 一致 |
| PageCatalogPart [viewstate] the edit mode: the editor zone and its parts for the first part | ✅ 一致 |
| PageCatalogPart [viewstate] the first part&#39;s title edited (the editor zone&#39;s OK) | ✅ 一致 |
| PageCatalogPart [viewstate] the connect mode for the first part | ✅ 一致 |
| PageCatalogPart [viewstate] the connections zone closed | ✅ 一致 |
| PageCatalogPart [viewstate] a part minimized by its zone&#39;s verb | ✅ 一致 |
| PageCatalogPart [viewstate] restored | ✅ 一致 |
| PageCatalogPart [viewstate] a part of the zone made in code minimized | ✅ 一致 |
| PageCatalogPart [visible][viewstate] a hidden zone shown | ✅ 一致 |

</details>
