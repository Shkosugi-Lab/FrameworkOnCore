# LayoutEditorPart パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 02:31
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 181 | 181 | 0 | 0 |
| ② HTML | 159 | 159 | 0 | 0 |
| ③ シナリオ | 12 | 12 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 190 件中 190 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
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
| <code>LayoutEditorPart.Api.Control.#ctor</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.Adapter</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.ClientID</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.Context</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.Controls</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.DataBind</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.Dispose</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.Disposed</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.Events</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.FindControl</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.Focus</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.HasControls</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.ID</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.Init</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.Load</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.OnInit</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.Page</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.Parent</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.PreRender</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.Render</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.Site</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.SkinID</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.Unload</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.ViewState</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.Control.Visible</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.ICompositeControlDesignerAccessor.RecreateChildControls</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.Panel.#ctor</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.Panel.AddAttributesToRender</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.Panel.BackImageUrl</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.Panel.CreateControlStyle</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.Panel.DefaultButton</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.Panel.Direction</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.Panel.GroupingText</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.Panel.HorizontalAlign</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.Panel.RenderBeginTag</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.Panel.RenderEndTag</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.Panel.ScrollBars</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.Panel.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.Panel.Wrap</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebControl.Width</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebParts.EditorPart.#ctor</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebParts.EditorPart.ApplyChanges</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebParts.EditorPart.Display</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebParts.EditorPart.DisplayTitle</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebParts.EditorPart.GetDesignModeState</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebParts.EditorPart.OnPreRender</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebParts.EditorPart.SetDesignModeState</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebParts.EditorPart.SyncChanges</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebParts.EditorPart.WebPartManager</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebParts.EditorPart.WebPartToEdit</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebParts.EditorPart.Zone</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebParts.LayoutEditorPart.#ctor</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebParts.LayoutEditorPart.ApplyChanges</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebParts.LayoutEditorPart.DefaultButton</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebParts.LayoutEditorPart.Display</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebParts.LayoutEditorPart.SyncChanges</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebParts.LayoutEditorPart.Title</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebParts.Part.ChromeState</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebParts.Part.ChromeType</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebParts.Part.Controls</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebParts.Part.DataBind</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebParts.Part.Description</code> | ✅ 一致 |
| <code>LayoutEditorPart.Api.WebControls.WebParts.Part.Title</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>LayoutEditorPart.(case list)</code> | ✅ 一致 |
| <code>LayoutEditorPart.(default)</code> | ✅ 一致 |
| <code>LayoutEditorPart.AccessKey=empty</code> | ✅ 一致 |
| <code>LayoutEditorPart.AccessKey=japanese</code> | ✅ 一致 |
| <code>LayoutEditorPart.AccessKey=k</code> | ✅ 一致 |
| <code>LayoutEditorPart.AccessKey=markup</code> | ✅ 一致 |
| <code>LayoutEditorPart.AccessKey=text</code> | ✅ 一致 |
| <code>LayoutEditorPart.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>LayoutEditorPart.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>LayoutEditorPart.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>LayoutEditorPart.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>LayoutEditorPart.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>LayoutEditorPart.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>LayoutEditorPart.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>LayoutEditorPart.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>LayoutEditorPart.BackColor=#3366CC</code> | ✅ 一致 |
| <code>LayoutEditorPart.BackColor=Red</code> | ✅ 一致 |
| <code>LayoutEditorPart.BackColor=Transparent</code> | ✅ 一致 |
| <code>LayoutEditorPart.BackImageUrl=empty</code> | ✅ 一致 |
| <code>LayoutEditorPart.BackImageUrl=japanese</code> | ✅ 一致 |
| <code>LayoutEditorPart.BackImageUrl=markup</code> | ✅ 一致 |
| <code>LayoutEditorPart.BackImageUrl=text</code> | ✅ 一致 |
| <code>LayoutEditorPart.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>LayoutEditorPart.BorderColor=Red</code> | ✅ 一致 |
| <code>LayoutEditorPart.BorderColor=Transparent</code> | ✅ 一致 |
| <code>LayoutEditorPart.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>LayoutEditorPart.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>LayoutEditorPart.BorderStyle=Double</code> | ✅ 一致 |
| <code>LayoutEditorPart.BorderStyle=Groove</code> | ✅ 一致 |
| <code>LayoutEditorPart.BorderStyle=Inset</code> | ✅ 一致 |
| <code>LayoutEditorPart.BorderStyle=None</code> | ✅ 一致 |
| <code>LayoutEditorPart.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>LayoutEditorPart.BorderStyle=Outset</code> | ✅ 一致 |
| <code>LayoutEditorPart.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>LayoutEditorPart.BorderStyle=Solid</code> | ✅ 一致 |
| <code>LayoutEditorPart.BorderWidth=120px</code> | ✅ 一致 |
| <code>LayoutEditorPart.BorderWidth=2em</code> | ✅ 一致 |
| <code>LayoutEditorPart.BorderWidth=50%</code> | ✅ 一致 |
| <code>LayoutEditorPart.ChromeState=Minimized</code> | ✅ 一致 |
| <code>LayoutEditorPart.ChromeState=Normal</code> | ✅ 一致 |
| <code>LayoutEditorPart.ChromeType=BorderOnly</code> | ✅ 一致 |
| <code>LayoutEditorPart.ChromeType=Default</code> | ✅ 一致 |
| <code>LayoutEditorPart.ChromeType=None</code> | ✅ 一致 |
| <code>LayoutEditorPart.ChromeType=TitleAndBorder</code> | ✅ 一致 |
| <code>LayoutEditorPart.ChromeType=TitleOnly</code> | ✅ 一致 |
| <code>LayoutEditorPart.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>LayoutEditorPart.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>LayoutEditorPart.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>LayoutEditorPart.ClientIDMode=Static</code> | ✅ 一致 |
| <code>LayoutEditorPart.CssClass=empty</code> | ✅ 一致 |
| <code>LayoutEditorPart.CssClass=japanese</code> | ✅ 一致 |
| <code>LayoutEditorPart.CssClass=markup</code> | ✅ 一致 |
| <code>LayoutEditorPart.CssClass=text</code> | ✅ 一致 |
| <code>LayoutEditorPart.DefaultButton=empty</code> | ✅ 一致 |
| <code>LayoutEditorPart.DefaultButton=japanese</code> | ✅ 一致 |
| <code>LayoutEditorPart.DefaultButton=markup</code> | ✅ 一致 |
| <code>LayoutEditorPart.DefaultButton=text</code> | ✅ 一致 |
| <code>LayoutEditorPart.Description=empty</code> | ✅ 一致 |
| <code>LayoutEditorPart.Description=japanese</code> | ✅ 一致 |
| <code>LayoutEditorPart.Description=markup</code> | ✅ 一致 |
| <code>LayoutEditorPart.Description=text</code> | ✅ 一致 |
| <code>LayoutEditorPart.Direction=LeftToRight</code> | ✅ 一致 |
| <code>LayoutEditorPart.Direction=NotSet</code> | ✅ 一致 |
| <code>LayoutEditorPart.Direction=RightToLeft</code> | ✅ 一致 |
| <code>LayoutEditorPart.EnableTheming=false</code> | ✅ 一致 |
| <code>LayoutEditorPart.EnableTheming=true</code> | ✅ 一致 |
| <code>LayoutEditorPart.EnableViewState=false</code> | ✅ 一致 |
| <code>LayoutEditorPart.EnableViewState=true</code> | ✅ 一致 |
| <code>LayoutEditorPart.Enabled=false</code> | ✅ 一致 |
| <code>LayoutEditorPart.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>LayoutEditorPart.Enabled=true</code> | ✅ 一致 |
| <code>LayoutEditorPart.Font.Bold=false</code> | ✅ 一致 |
| <code>LayoutEditorPart.Font.Bold=true</code> | ✅ 一致 |
| <code>LayoutEditorPart.Font.Italic=false</code> | ✅ 一致 |
| <code>LayoutEditorPart.Font.Italic=true</code> | ✅ 一致 |
| <code>LayoutEditorPart.Font.Name=empty</code> | ✅ 一致 |
| <code>LayoutEditorPart.Font.Name=japanese</code> | ✅ 一致 |
| <code>LayoutEditorPart.Font.Name=markup</code> | ✅ 一致 |
| <code>LayoutEditorPart.Font.Name=text</code> | ✅ 一致 |
| <code>LayoutEditorPart.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>LayoutEditorPart.Font.Overline=false</code> | ✅ 一致 |
| <code>LayoutEditorPart.Font.Overline=true</code> | ✅ 一致 |
| <code>LayoutEditorPart.Font.Size=12pt</code> | ✅ 一致 |
| <code>LayoutEditorPart.Font.Size=150%</code> | ✅ 一致 |
| <code>LayoutEditorPart.Font.Size=Large</code> | ✅ 一致 |
| <code>LayoutEditorPart.Font.Strikeout=false</code> | ✅ 一致 |
| <code>LayoutEditorPart.Font.Strikeout=true</code> | ✅ 一致 |
| <code>LayoutEditorPart.Font.Underline=false</code> | ✅ 一致 |
| <code>LayoutEditorPart.Font.Underline=true</code> | ✅ 一致 |
| <code>LayoutEditorPart.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>LayoutEditorPart.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>LayoutEditorPart.ForeColor=Red</code> | ✅ 一致 |
| <code>LayoutEditorPart.ForeColor=Transparent</code> | ✅ 一致 |
| <code>LayoutEditorPart.GroupingText=empty</code> | ✅ 一致 |
| <code>LayoutEditorPart.GroupingText=japanese</code> | ✅ 一致 |
| <code>LayoutEditorPart.GroupingText=markup</code> | ✅ 一致 |
| <code>LayoutEditorPart.GroupingText=text</code> | ✅ 一致 |
| <code>LayoutEditorPart.Height=120px</code> | ✅ 一致 |
| <code>LayoutEditorPart.Height=2em</code> | ✅ 一致 |
| <code>LayoutEditorPart.Height=50%</code> | ✅ 一致 |
| <code>LayoutEditorPart.HorizontalAlign=Center</code> | ✅ 一致 |
| <code>LayoutEditorPart.HorizontalAlign=Justify</code> | ✅ 一致 |
| <code>LayoutEditorPart.HorizontalAlign=Left</code> | ✅ 一致 |
| <code>LayoutEditorPart.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>LayoutEditorPart.HorizontalAlign=Right</code> | ✅ 一致 |
| <code>LayoutEditorPart.ID=empty</code> | ✅ 一致 |
| <code>LayoutEditorPart.ID=japanese</code> | ✅ 一致 |
| <code>LayoutEditorPart.ID=markup</code> | ✅ 一致 |
| <code>LayoutEditorPart.ID=text</code> | ✅ 一致 |
| <code>LayoutEditorPart.ScrollBars=Auto</code> | ✅ 一致 |
| <code>LayoutEditorPart.ScrollBars=Both</code> | ✅ 一致 |
| <code>LayoutEditorPart.ScrollBars=Horizontal</code> | ✅ 一致 |
| <code>LayoutEditorPart.ScrollBars=None</code> | ✅ 一致 |
| <code>LayoutEditorPart.ScrollBars=Vertical</code> | ✅ 一致 |
| <code>LayoutEditorPart.SkinID=empty</code> | ✅ 一致 |
| <code>LayoutEditorPart.SkinID=japanese</code> | ✅ 一致 |
| <code>LayoutEditorPart.SkinID=markup</code> | ✅ 一致 |
| <code>LayoutEditorPart.SkinID=text</code> | ✅ 一致 |
| <code>LayoutEditorPart.Style[color]=1em</code> | ✅ 一致 |
| <code>LayoutEditorPart.Style[color]=red</code> | ✅ 一致 |
| <code>LayoutEditorPart.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>LayoutEditorPart.Style[margin-left]=red</code> | ✅ 一致 |
| <code>LayoutEditorPart.TabIndex=-1</code> | ✅ 一致 |
| <code>LayoutEditorPart.TabIndex=0</code> | ✅ 一致 |
| <code>LayoutEditorPart.TabIndex=3</code> | ✅ 一致 |
| <code>LayoutEditorPart.Title=empty</code> | ✅ 一致 |
| <code>LayoutEditorPart.Title=japanese</code> | ✅ 一致 |
| <code>LayoutEditorPart.Title=markup</code> | ✅ 一致 |
| <code>LayoutEditorPart.Title=markup+Description=text</code> | ✅ 一致 |
| <code>LayoutEditorPart.Title=text</code> | ✅ 一致 |
| <code>LayoutEditorPart.ToolTip=empty</code> | ✅ 一致 |
| <code>LayoutEditorPart.ToolTip=japanese</code> | ✅ 一致 |
| <code>LayoutEditorPart.ToolTip=markup</code> | ✅ 一致 |
| <code>LayoutEditorPart.ToolTip=text</code> | ✅ 一致 |
| <code>LayoutEditorPart.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>LayoutEditorPart.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>LayoutEditorPart.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>LayoutEditorPart.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>LayoutEditorPart.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>LayoutEditorPart.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>LayoutEditorPart.Visible=false</code> | ✅ 一致 |
| <code>LayoutEditorPart.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>LayoutEditorPart.Visible=true</code> | ✅ 一致 |
| <code>LayoutEditorPart.Width=120px</code> | ✅ 一致 |
| <code>LayoutEditorPart.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>LayoutEditorPart.Width=2em</code> | ✅ 一致 |
| <code>LayoutEditorPart.Width=50%</code> | ✅ 一致 |
| <code>LayoutEditorPart.Wrap=false</code> | ✅ 一致 |
| <code>LayoutEditorPart.Wrap=true</code> | ✅ 一致 |
| <code>LayoutEditorPart[naming container].(default)</code> | ✅ 一致 |
| <code>LayoutEditorPart[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>LayoutEditorPart[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>LayoutEditorPart[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>LayoutEditorPart[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>LayoutEditorPart[naming container].ID=text</code> | ✅ 一致 |
| <code>LayoutEditorPart[themed].(default)</code> | ✅ 一致 |
| <code>LayoutEditorPart[themed].CssClass=text</code> | ✅ 一致 |
| <code>LayoutEditorPart[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>LayoutEditorPart[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| LayoutEditorPart [render][databind][dynamic] first request (browse): the parts in their zones, a zone made in code | ✅ 一致 |
| LayoutEditorPart [viewstate] the design mode | ✅ 一致 |
| LayoutEditorPart [viewstate] the catalog mode: the catalog zone and its parts | ✅ 一致 |
| LayoutEditorPart [viewstate] a part added from the catalog (its check box posted, the zone&#39;s Add) | ✅ 一致 |
| LayoutEditorPart [viewstate][templates] the edit mode: the editor zone and its parts for the first part | ✅ 一致 |
| LayoutEditorPart [viewstate] the first part&#39;s title edited (the editor zone&#39;s OK) | ✅ 一致 |
| LayoutEditorPart [viewstate] the connect mode for the first part | ✅ 一致 |
| LayoutEditorPart [viewstate] the connections zone closed | ✅ 一致 |
| LayoutEditorPart [viewstate] a part minimized by its zone&#39;s verb | ✅ 一致 |
| LayoutEditorPart [viewstate] restored | ✅ 一致 |
| LayoutEditorPart [viewstate] a part of the zone made in code minimized | ✅ 一致 |
| LayoutEditorPart [visible][viewstate] a hidden zone shown | ✅ 一致 |

</details>
