# DeclarativeCatalogPart パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-11 23:30
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 202 | 202 | 0 | 0 |
| ② HTML | 163 | 163 | 0 | 0 |
| ③ シナリオ | 12 | 12 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 211 件中 211 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 41 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 12 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 151 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 11 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 1 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 2 | 0 | ✅ |
| テンプレート | 0 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>DeclarativeCatalogPart.Api.Control.#ctor</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.Adapter</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.ClientID</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.Context</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.Controls</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.DataBind</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.Dispose</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.Disposed</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.Events</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.FindControl</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.Focus</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.HasControls</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.ID</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.Init</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.Load</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.OnInit</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.Page</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.Parent</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.PreRender</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.Render</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.Site</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.SkinID</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.Unload</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.ViewState</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.Control.Visible</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.ICompositeControlDesignerAccessor.RecreateChildControls</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.Panel.#ctor</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.Panel.AddAttributesToRender</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.Panel.BackImageUrl</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.Panel.CreateControlStyle</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.Panel.DefaultButton</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.Panel.Direction</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.Panel.GroupingText</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.Panel.HorizontalAlign</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.Panel.RenderBeginTag</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.Panel.RenderEndTag</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.Panel.ScrollBars</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.Panel.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.Panel.Wrap</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebControl.Width</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.CatalogPart.#ctor</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.CatalogPart.DisplayTitle</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.CatalogPart.GetAvailableWebPartDescriptions</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.CatalogPart.GetDesignModeState</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.CatalogPart.GetWebPart</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.CatalogPart.OnPreRender</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.CatalogPart.SetDesignModeState</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.CatalogPart.WebPartManager</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.CatalogPart.Zone</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.#ctor</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.AccessKey</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.BackColor</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.BackImageUrl</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.BorderColor</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.BorderStyle</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.BorderWidth</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.CssClass</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.DefaultButton</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.Direction</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.EnableTheming</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.Enabled</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.Font</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.ForeColor</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.GetAvailableWebPartDescriptions</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.GetWebPart</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.GroupingText</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.Height</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.HorizontalAlign</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.ScrollBars</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.SkinID</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.TabIndex</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.Title</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.ToolTip</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.Visible</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.WebPartsListUserControlPath</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.WebPartsTemplate</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.Width</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.DeclarativeCatalogPart.Wrap</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.Part.ChromeState</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.Part.ChromeType</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.Part.Controls</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.Part.DataBind</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.Part.Description</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Api.WebControls.WebParts.Part.Title</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>DeclarativeCatalogPart.(case list)</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.(default)</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.AccessKey=empty</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.AccessKey=japanese</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.AccessKey=k</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.AccessKey=markup</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.AccessKey=text</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.BackColor=#3366CC</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.BackColor=Red</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.BackColor=Transparent</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.BackImageUrl=empty</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.BackImageUrl=japanese</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.BackImageUrl=markup</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.BackImageUrl=text</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.BorderColor=Red</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.BorderColor=Transparent</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.BorderStyle=Double</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.BorderStyle=Groove</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.BorderStyle=Inset</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.BorderStyle=None</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.BorderStyle=Outset</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.BorderStyle=Solid</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.BorderWidth=120px</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.BorderWidth=2em</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.BorderWidth=50%</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ChromeState=Minimized</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ChromeState=Normal</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ChromeType=BorderOnly</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ChromeType=Default</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ChromeType=None</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ChromeType=TitleAndBorder</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ChromeType=TitleOnly</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ClientIDMode=Static</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.CssClass=empty</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.CssClass=japanese</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.CssClass=markup</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.CssClass=text</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.DefaultButton=empty</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.DefaultButton=japanese</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.DefaultButton=markup</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.DefaultButton=text</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Description=empty</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Description=japanese</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Description=markup</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Description=text</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Direction=LeftToRight</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Direction=NotSet</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Direction=RightToLeft</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.EnableTheming=false</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.EnableTheming=true</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.EnableViewState=false</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.EnableViewState=true</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Enabled=false</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Enabled=true</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Font.Bold=false</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Font.Bold=true</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Font.Italic=false</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Font.Italic=true</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Font.Name=empty</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Font.Name=japanese</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Font.Name=markup</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Font.Name=text</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Font.Overline=false</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Font.Overline=true</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Font.Size=12pt</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Font.Size=150%</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Font.Size=Large</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Font.Strikeout=false</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Font.Strikeout=true</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Font.Underline=false</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Font.Underline=true</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ForeColor=Red</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ForeColor=Transparent</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.GroupingText=empty</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.GroupingText=japanese</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.GroupingText=markup</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.GroupingText=text</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Height=120px</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Height=2em</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Height=50%</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.HorizontalAlign=Center</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.HorizontalAlign=Justify</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.HorizontalAlign=Left</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.HorizontalAlign=Right</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ID=empty</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ID=japanese</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ID=markup</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ID=text</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ScrollBars=Auto</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ScrollBars=Both</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ScrollBars=Horizontal</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ScrollBars=None</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ScrollBars=Vertical</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.SkinID=empty</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.SkinID=japanese</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.SkinID=markup</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.SkinID=text</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Style[color]=1em</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Style[color]=red</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Style[margin-left]=red</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.TabIndex=-1</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.TabIndex=0</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.TabIndex=3</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Title=empty</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Title=japanese</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Title=markup</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Title=markup+Description=text</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Title=text</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ToolTip=empty</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ToolTip=japanese</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ToolTip=markup</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ToolTip=text</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Visible=false</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Visible=true</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.WebPartsListUserControlPath=empty</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.WebPartsListUserControlPath=japanese</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.WebPartsListUserControlPath=markup</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.WebPartsListUserControlPath=text</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Width=120px</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Width=2em</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Width=50%</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Wrap=false</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart.Wrap=true</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart[naming container].(default)</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart[naming container].ID=text</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart[themed].(default)</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart[themed].CssClass=text</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>DeclarativeCatalogPart[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| DeclarativeCatalogPart [render][databind][dynamic] first request (browse): the parts in their zones, a zone made in code | ✅ 一致 |
| DeclarativeCatalogPart [viewstate] the design mode | ✅ 一致 |
| DeclarativeCatalogPart [viewstate][templates] the catalog mode: the catalog zone and its parts | ✅ 一致 |
| DeclarativeCatalogPart [viewstate] a part added from the catalog (its check box posted, the zone&#39;s Add) | ✅ 一致 |
| DeclarativeCatalogPart [viewstate] the edit mode: the editor zone and its parts for the first part | ✅ 一致 |
| DeclarativeCatalogPart [viewstate] the first part&#39;s title edited (the editor zone&#39;s OK) | ✅ 一致 |
| DeclarativeCatalogPart [viewstate] the connect mode for the first part | ✅ 一致 |
| DeclarativeCatalogPart [viewstate] the connections zone closed | ✅ 一致 |
| DeclarativeCatalogPart [viewstate] a part minimized by its zone&#39;s verb | ✅ 一致 |
| DeclarativeCatalogPart [viewstate] restored | ✅ 一致 |
| DeclarativeCatalogPart [viewstate] a part of the zone made in code minimized | ✅ 一致 |
| DeclarativeCatalogPart [visible][viewstate] a hidden zone shown | ✅ 一致 |

</details>
