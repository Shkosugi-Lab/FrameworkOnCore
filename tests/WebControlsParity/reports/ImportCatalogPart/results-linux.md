# ImportCatalogPart パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 02:30
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 183 | 183 | 0 | 0 |
| ② HTML | 180 | 180 | 0 | 0 |
| ③ シナリオ | 12 | 12 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 192 件中 192 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 45 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 12 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 168 | 1 | ✅ |
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
| <code>ImportCatalogPart.Api.Control.#ctor</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.Adapter</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.ClientID</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.Context</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.Controls</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.DataBind</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.Dispose</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.Disposed</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.Events</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.FindControl</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.Focus</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.HasControls</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.ID</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.Init</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.Load</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.OnInit</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.Page</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.Parent</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.PreRender</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.Render</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.Site</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.SkinID</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.Unload</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.ViewState</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.Control.Visible</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.ICompositeControlDesignerAccessor.RecreateChildControls</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.Panel.#ctor</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.Panel.AddAttributesToRender</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.Panel.BackImageUrl</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.Panel.CreateControlStyle</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.Panel.DefaultButton</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.Panel.Direction</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.Panel.GroupingText</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.Panel.HorizontalAlign</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.Panel.RenderBeginTag</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.Panel.RenderEndTag</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.Panel.ScrollBars</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.Panel.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.Panel.Wrap</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebControl.Width</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebParts.CatalogPart.#ctor</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebParts.CatalogPart.DisplayTitle</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebParts.CatalogPart.GetAvailableWebPartDescriptions</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebParts.CatalogPart.GetDesignModeState</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebParts.CatalogPart.GetWebPart</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebParts.CatalogPart.OnPreRender</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebParts.CatalogPart.SetDesignModeState</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebParts.CatalogPart.WebPartManager</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebParts.CatalogPart.Zone</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebParts.ImportCatalogPart.#ctor</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebParts.ImportCatalogPart.BrowseHelpText</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebParts.ImportCatalogPart.DefaultButton</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebParts.ImportCatalogPart.GetAvailableWebPartDescriptions</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebParts.ImportCatalogPart.GetWebPart</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebParts.ImportCatalogPart.ImportedPartLabelText</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebParts.ImportCatalogPart.PartImportErrorLabelText</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebParts.ImportCatalogPart.Title</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebParts.ImportCatalogPart.UploadButtonText</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebParts.ImportCatalogPart.UploadHelpText</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebParts.Part.ChromeState</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebParts.Part.ChromeType</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebParts.Part.Controls</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebParts.Part.DataBind</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebParts.Part.Description</code> | ✅ 一致 |
| <code>ImportCatalogPart.Api.WebControls.WebParts.Part.Title</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>ImportCatalogPart.(case list)</code> | ✅ 一致 |
| <code>ImportCatalogPart.(default)</code> | ✅ 一致 |
| <code>ImportCatalogPart.AccessKey=empty</code> | ✅ 一致 |
| <code>ImportCatalogPart.AccessKey=japanese</code> | ✅ 一致 |
| <code>ImportCatalogPart.AccessKey=k</code> | ✅ 一致 |
| <code>ImportCatalogPart.AccessKey=markup</code> | ✅ 一致 |
| <code>ImportCatalogPart.AccessKey=text</code> | ✅ 一致 |
| <code>ImportCatalogPart.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>ImportCatalogPart.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>ImportCatalogPart.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>ImportCatalogPart.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>ImportCatalogPart.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>ImportCatalogPart.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>ImportCatalogPart.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>ImportCatalogPart.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>ImportCatalogPart.BackColor=#3366CC</code> | ✅ 一致 |
| <code>ImportCatalogPart.BackColor=Red</code> | ✅ 一致 |
| <code>ImportCatalogPart.BackColor=Transparent</code> | ✅ 一致 |
| <code>ImportCatalogPart.BackImageUrl=empty</code> | ✅ 一致 |
| <code>ImportCatalogPart.BackImageUrl=japanese</code> | ✅ 一致 |
| <code>ImportCatalogPart.BackImageUrl=markup</code> | ✅ 一致 |
| <code>ImportCatalogPart.BackImageUrl=text</code> | ✅ 一致 |
| <code>ImportCatalogPart.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>ImportCatalogPart.BorderColor=Red</code> | ✅ 一致 |
| <code>ImportCatalogPart.BorderColor=Transparent</code> | ✅ 一致 |
| <code>ImportCatalogPart.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>ImportCatalogPart.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>ImportCatalogPart.BorderStyle=Double</code> | ✅ 一致 |
| <code>ImportCatalogPart.BorderStyle=Groove</code> | ✅ 一致 |
| <code>ImportCatalogPart.BorderStyle=Inset</code> | ✅ 一致 |
| <code>ImportCatalogPart.BorderStyle=None</code> | ✅ 一致 |
| <code>ImportCatalogPart.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>ImportCatalogPart.BorderStyle=Outset</code> | ✅ 一致 |
| <code>ImportCatalogPart.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>ImportCatalogPart.BorderStyle=Solid</code> | ✅ 一致 |
| <code>ImportCatalogPart.BorderWidth=120px</code> | ✅ 一致 |
| <code>ImportCatalogPart.BorderWidth=2em</code> | ✅ 一致 |
| <code>ImportCatalogPart.BorderWidth=50%</code> | ✅ 一致 |
| <code>ImportCatalogPart.BrowseHelpText=empty</code> | ✅ 一致 |
| <code>ImportCatalogPart.BrowseHelpText=japanese</code> | ✅ 一致 |
| <code>ImportCatalogPart.BrowseHelpText=markup</code> | ✅ 一致 |
| <code>ImportCatalogPart.BrowseHelpText=markup+UploadHelpText=markup</code> | ✅ 一致 |
| <code>ImportCatalogPart.BrowseHelpText=text</code> | ✅ 一致 |
| <code>ImportCatalogPart.ChromeState=Minimized</code> | ✅ 一致 |
| <code>ImportCatalogPart.ChromeState=Normal</code> | ✅ 一致 |
| <code>ImportCatalogPart.ChromeType=BorderOnly</code> | ✅ 一致 |
| <code>ImportCatalogPart.ChromeType=Default</code> | ✅ 一致 |
| <code>ImportCatalogPart.ChromeType=None</code> | ✅ 一致 |
| <code>ImportCatalogPart.ChromeType=TitleAndBorder</code> | ✅ 一致 |
| <code>ImportCatalogPart.ChromeType=TitleOnly</code> | ✅ 一致 |
| <code>ImportCatalogPart.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ImportCatalogPart.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ImportCatalogPart.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ImportCatalogPart.ClientIDMode=Static</code> | ✅ 一致 |
| <code>ImportCatalogPart.CssClass=empty</code> | ✅ 一致 |
| <code>ImportCatalogPart.CssClass=japanese</code> | ✅ 一致 |
| <code>ImportCatalogPart.CssClass=markup</code> | ✅ 一致 |
| <code>ImportCatalogPart.CssClass=text</code> | ✅ 一致 |
| <code>ImportCatalogPart.DefaultButton=empty</code> | ✅ 一致 |
| <code>ImportCatalogPart.DefaultButton=japanese</code> | ✅ 一致 |
| <code>ImportCatalogPart.DefaultButton=markup</code> | ✅ 一致 |
| <code>ImportCatalogPart.DefaultButton=text</code> | ✅ 一致 |
| <code>ImportCatalogPart.Description=empty</code> | ✅ 一致 |
| <code>ImportCatalogPart.Description=japanese</code> | ✅ 一致 |
| <code>ImportCatalogPart.Description=markup</code> | ✅ 一致 |
| <code>ImportCatalogPart.Description=text</code> | ✅ 一致 |
| <code>ImportCatalogPart.Direction=LeftToRight</code> | ✅ 一致 |
| <code>ImportCatalogPart.Direction=NotSet</code> | ✅ 一致 |
| <code>ImportCatalogPart.Direction=RightToLeft</code> | ✅ 一致 |
| <code>ImportCatalogPart.EnableTheming=false</code> | ✅ 一致 |
| <code>ImportCatalogPart.EnableTheming=true</code> | ✅ 一致 |
| <code>ImportCatalogPart.EnableViewState=false</code> | ✅ 一致 |
| <code>ImportCatalogPart.EnableViewState=true</code> | ✅ 一致 |
| <code>ImportCatalogPart.Enabled=false</code> | ✅ 一致 |
| <code>ImportCatalogPart.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>ImportCatalogPart.Enabled=true</code> | ✅ 一致 |
| <code>ImportCatalogPart.Font.Bold=false</code> | ✅ 一致 |
| <code>ImportCatalogPart.Font.Bold=true</code> | ✅ 一致 |
| <code>ImportCatalogPart.Font.Italic=false</code> | ✅ 一致 |
| <code>ImportCatalogPart.Font.Italic=true</code> | ✅ 一致 |
| <code>ImportCatalogPart.Font.Name=empty</code> | ✅ 一致 |
| <code>ImportCatalogPart.Font.Name=japanese</code> | ✅ 一致 |
| <code>ImportCatalogPart.Font.Name=markup</code> | ✅ 一致 |
| <code>ImportCatalogPart.Font.Name=text</code> | ✅ 一致 |
| <code>ImportCatalogPart.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>ImportCatalogPart.Font.Overline=false</code> | ✅ 一致 |
| <code>ImportCatalogPart.Font.Overline=true</code> | ✅ 一致 |
| <code>ImportCatalogPart.Font.Size=12pt</code> | ✅ 一致 |
| <code>ImportCatalogPart.Font.Size=150%</code> | ✅ 一致 |
| <code>ImportCatalogPart.Font.Size=Large</code> | ✅ 一致 |
| <code>ImportCatalogPart.Font.Strikeout=false</code> | ✅ 一致 |
| <code>ImportCatalogPart.Font.Strikeout=true</code> | ✅ 一致 |
| <code>ImportCatalogPart.Font.Underline=false</code> | ✅ 一致 |
| <code>ImportCatalogPart.Font.Underline=true</code> | ✅ 一致 |
| <code>ImportCatalogPart.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>ImportCatalogPart.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>ImportCatalogPart.ForeColor=Red</code> | ✅ 一致 |
| <code>ImportCatalogPart.ForeColor=Transparent</code> | ✅ 一致 |
| <code>ImportCatalogPart.GroupingText=empty</code> | ✅ 一致 |
| <code>ImportCatalogPart.GroupingText=japanese</code> | ✅ 一致 |
| <code>ImportCatalogPart.GroupingText=markup</code> | ✅ 一致 |
| <code>ImportCatalogPart.GroupingText=text</code> | ✅ 一致 |
| <code>ImportCatalogPart.Height=120px</code> | ✅ 一致 |
| <code>ImportCatalogPart.Height=2em</code> | ✅ 一致 |
| <code>ImportCatalogPart.Height=50%</code> | ✅ 一致 |
| <code>ImportCatalogPart.HorizontalAlign=Center</code> | ✅ 一致 |
| <code>ImportCatalogPart.HorizontalAlign=Justify</code> | ✅ 一致 |
| <code>ImportCatalogPart.HorizontalAlign=Left</code> | ✅ 一致 |
| <code>ImportCatalogPart.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>ImportCatalogPart.HorizontalAlign=Right</code> | ✅ 一致 |
| <code>ImportCatalogPart.ID=empty</code> | ✅ 一致 |
| <code>ImportCatalogPart.ID=japanese</code> | ✅ 一致 |
| <code>ImportCatalogPart.ID=markup</code> | ✅ 一致 |
| <code>ImportCatalogPart.ID=text</code> | ✅ 一致 |
| <code>ImportCatalogPart.ImportedPartLabelText=empty</code> | ✅ 一致 |
| <code>ImportCatalogPart.ImportedPartLabelText=japanese</code> | ✅ 一致 |
| <code>ImportCatalogPart.ImportedPartLabelText=markup</code> | ✅ 一致 |
| <code>ImportCatalogPart.ImportedPartLabelText=text</code> | ✅ 一致 |
| <code>ImportCatalogPart.PartImportErrorLabelText=empty</code> | ✅ 一致 |
| <code>ImportCatalogPart.PartImportErrorLabelText=japanese</code> | ✅ 一致 |
| <code>ImportCatalogPart.PartImportErrorLabelText=markup</code> | ✅ 一致 |
| <code>ImportCatalogPart.PartImportErrorLabelText=text</code> | ✅ 一致 |
| <code>ImportCatalogPart.PartImportErrorLabelText=text+UploadButtonText=text</code> | ✅ 一致 |
| <code>ImportCatalogPart.ScrollBars=Auto</code> | ✅ 一致 |
| <code>ImportCatalogPart.ScrollBars=Both</code> | ✅ 一致 |
| <code>ImportCatalogPart.ScrollBars=Horizontal</code> | ✅ 一致 |
| <code>ImportCatalogPart.ScrollBars=None</code> | ✅ 一致 |
| <code>ImportCatalogPart.ScrollBars=Vertical</code> | ✅ 一致 |
| <code>ImportCatalogPart.SkinID=empty</code> | ✅ 一致 |
| <code>ImportCatalogPart.SkinID=japanese</code> | ✅ 一致 |
| <code>ImportCatalogPart.SkinID=markup</code> | ✅ 一致 |
| <code>ImportCatalogPart.SkinID=text</code> | ✅ 一致 |
| <code>ImportCatalogPart.Style[color]=1em</code> | ✅ 一致 |
| <code>ImportCatalogPart.Style[color]=red</code> | ✅ 一致 |
| <code>ImportCatalogPart.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>ImportCatalogPart.Style[margin-left]=red</code> | ✅ 一致 |
| <code>ImportCatalogPart.TabIndex=-1</code> | ✅ 一致 |
| <code>ImportCatalogPart.TabIndex=0</code> | ✅ 一致 |
| <code>ImportCatalogPart.TabIndex=3</code> | ✅ 一致 |
| <code>ImportCatalogPart.Title=empty</code> | ✅ 一致 |
| <code>ImportCatalogPart.Title=japanese</code> | ✅ 一致 |
| <code>ImportCatalogPart.Title=markup</code> | ✅ 一致 |
| <code>ImportCatalogPart.Title=text</code> | ✅ 一致 |
| <code>ImportCatalogPart.ToolTip=empty</code> | ✅ 一致 |
| <code>ImportCatalogPart.ToolTip=japanese</code> | ✅ 一致 |
| <code>ImportCatalogPart.ToolTip=markup</code> | ✅ 一致 |
| <code>ImportCatalogPart.ToolTip=text</code> | ✅ 一致 |
| <code>ImportCatalogPart.UploadButtonText=empty</code> | ✅ 一致 |
| <code>ImportCatalogPart.UploadButtonText=japanese</code> | ✅ 一致 |
| <code>ImportCatalogPart.UploadButtonText=markup</code> | ✅ 一致 |
| <code>ImportCatalogPart.UploadButtonText=text</code> | ✅ 一致 |
| <code>ImportCatalogPart.UploadHelpText=empty</code> | ✅ 一致 |
| <code>ImportCatalogPart.UploadHelpText=japanese</code> | ✅ 一致 |
| <code>ImportCatalogPart.UploadHelpText=markup</code> | ✅ 一致 |
| <code>ImportCatalogPart.UploadHelpText=text</code> | ✅ 一致 |
| <code>ImportCatalogPart.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>ImportCatalogPart.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>ImportCatalogPart.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>ImportCatalogPart.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>ImportCatalogPart.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>ImportCatalogPart.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>ImportCatalogPart.Visible=false</code> | ✅ 一致 |
| <code>ImportCatalogPart.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>ImportCatalogPart.Visible=true</code> | ✅ 一致 |
| <code>ImportCatalogPart.Width=120px</code> | ✅ 一致 |
| <code>ImportCatalogPart.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>ImportCatalogPart.Width=2em</code> | ✅ 一致 |
| <code>ImportCatalogPart.Width=50%</code> | ✅ 一致 |
| <code>ImportCatalogPart.Wrap=false</code> | ✅ 一致 |
| <code>ImportCatalogPart.Wrap=true</code> | ✅ 一致 |
| <code>ImportCatalogPart[naming container].(default)</code> | ✅ 一致 |
| <code>ImportCatalogPart[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ImportCatalogPart[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ImportCatalogPart[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ImportCatalogPart[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>ImportCatalogPart[naming container].ID=text</code> | ✅ 一致 |
| <code>ImportCatalogPart[themed].(default)</code> | ✅ 一致 |
| <code>ImportCatalogPart[themed].CssClass=text</code> | ✅ 一致 |
| <code>ImportCatalogPart[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>ImportCatalogPart[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| ImportCatalogPart [render][databind][dynamic] first request (browse): the parts in their zones, a zone made in code | ✅ 一致 |
| ImportCatalogPart [viewstate] the design mode | ✅ 一致 |
| ImportCatalogPart [viewstate][templates] the catalog mode: the catalog zone and its parts | ✅ 一致 |
| ImportCatalogPart [viewstate] a part added from the catalog (its check box posted, the zone&#39;s Add) | ✅ 一致 |
| ImportCatalogPart [viewstate] the edit mode: the editor zone and its parts for the first part | ✅ 一致 |
| ImportCatalogPart [viewstate] the first part&#39;s title edited (the editor zone&#39;s OK) | ✅ 一致 |
| ImportCatalogPart [viewstate] the connect mode for the first part | ✅ 一致 |
| ImportCatalogPart [viewstate] the connections zone closed | ✅ 一致 |
| ImportCatalogPart [viewstate] a part minimized by its zone&#39;s verb | ✅ 一致 |
| ImportCatalogPart [viewstate] restored | ✅ 一致 |
| ImportCatalogPart [viewstate] a part of the zone made in code minimized | ✅ 一致 |
| ImportCatalogPart [visible][viewstate] a hidden zone shown | ✅ 一致 |

</details>
