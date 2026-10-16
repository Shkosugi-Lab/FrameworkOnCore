# AppearanceEditorPart パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-11 23:10
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 180 | 180 | 0 | 0 |
| ② HTML | 159 | 159 | 0 | 0 |
| ③ シナリオ | 12 | 12 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 189 件中 189 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 40 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 12 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 147 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 10 | ✅ |
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
| <code>AppearanceEditorPart.Api.Control.#ctor</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.Adapter</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.ClientID</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.Context</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.Controls</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.DataBind</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.Dispose</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.Disposed</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.Events</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.FindControl</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.Focus</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.HasControls</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.ID</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.Init</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.Load</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.OnInit</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.Page</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.Parent</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.PreRender</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.Render</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.Site</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.SkinID</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.Unload</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.ViewState</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.Control.Visible</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.ICompositeControlDesignerAccessor.RecreateChildControls</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.Panel.#ctor</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.Panel.AddAttributesToRender</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.Panel.BackImageUrl</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.Panel.CreateControlStyle</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.Panel.DefaultButton</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.Panel.Direction</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.Panel.GroupingText</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.Panel.HorizontalAlign</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.Panel.RenderBeginTag</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.Panel.RenderEndTag</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.Panel.ScrollBars</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.Panel.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.Panel.Wrap</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebControl.Width</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebParts.AppearanceEditorPart.#ctor</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebParts.AppearanceEditorPart.ApplyChanges</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebParts.AppearanceEditorPart.DefaultButton</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebParts.AppearanceEditorPart.SyncChanges</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebParts.AppearanceEditorPart.Title</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebParts.EditorPart.#ctor</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebParts.EditorPart.ApplyChanges</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebParts.EditorPart.Display</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebParts.EditorPart.DisplayTitle</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebParts.EditorPart.GetDesignModeState</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebParts.EditorPart.OnPreRender</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebParts.EditorPart.SetDesignModeState</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebParts.EditorPart.SyncChanges</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebParts.EditorPart.WebPartManager</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebParts.EditorPart.WebPartToEdit</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebParts.EditorPart.Zone</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebParts.Part.ChromeState</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebParts.Part.ChromeType</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebParts.Part.Controls</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebParts.Part.DataBind</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebParts.Part.Description</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Api.WebControls.WebParts.Part.Title</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>AppearanceEditorPart.(case list)</code> | ✅ 一致 |
| <code>AppearanceEditorPart.(default)</code> | ✅ 一致 |
| <code>AppearanceEditorPart.AccessKey=empty</code> | ✅ 一致 |
| <code>AppearanceEditorPart.AccessKey=japanese</code> | ✅ 一致 |
| <code>AppearanceEditorPart.AccessKey=k</code> | ✅ 一致 |
| <code>AppearanceEditorPart.AccessKey=markup</code> | ✅ 一致 |
| <code>AppearanceEditorPart.AccessKey=text</code> | ✅ 一致 |
| <code>AppearanceEditorPart.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>AppearanceEditorPart.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>AppearanceEditorPart.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>AppearanceEditorPart.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>AppearanceEditorPart.BackColor=#3366CC</code> | ✅ 一致 |
| <code>AppearanceEditorPart.BackColor=Red</code> | ✅ 一致 |
| <code>AppearanceEditorPart.BackColor=Transparent</code> | ✅ 一致 |
| <code>AppearanceEditorPart.BackImageUrl=empty</code> | ✅ 一致 |
| <code>AppearanceEditorPart.BackImageUrl=japanese</code> | ✅ 一致 |
| <code>AppearanceEditorPart.BackImageUrl=markup</code> | ✅ 一致 |
| <code>AppearanceEditorPart.BackImageUrl=text</code> | ✅ 一致 |
| <code>AppearanceEditorPart.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>AppearanceEditorPart.BorderColor=Red</code> | ✅ 一致 |
| <code>AppearanceEditorPart.BorderColor=Transparent</code> | ✅ 一致 |
| <code>AppearanceEditorPart.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>AppearanceEditorPart.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>AppearanceEditorPart.BorderStyle=Double</code> | ✅ 一致 |
| <code>AppearanceEditorPart.BorderStyle=Groove</code> | ✅ 一致 |
| <code>AppearanceEditorPart.BorderStyle=Inset</code> | ✅ 一致 |
| <code>AppearanceEditorPart.BorderStyle=None</code> | ✅ 一致 |
| <code>AppearanceEditorPart.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>AppearanceEditorPart.BorderStyle=Outset</code> | ✅ 一致 |
| <code>AppearanceEditorPart.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>AppearanceEditorPart.BorderStyle=Solid</code> | ✅ 一致 |
| <code>AppearanceEditorPart.BorderWidth=120px</code> | ✅ 一致 |
| <code>AppearanceEditorPart.BorderWidth=2em</code> | ✅ 一致 |
| <code>AppearanceEditorPart.BorderWidth=50%</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ChromeState=Minimized</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ChromeState=Normal</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ChromeType=BorderOnly</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ChromeType=Default</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ChromeType=None</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ChromeType=TitleAndBorder</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ChromeType=TitleOnly</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ClientIDMode=Static</code> | ✅ 一致 |
| <code>AppearanceEditorPart.CssClass=empty</code> | ✅ 一致 |
| <code>AppearanceEditorPart.CssClass=japanese</code> | ✅ 一致 |
| <code>AppearanceEditorPart.CssClass=markup</code> | ✅ 一致 |
| <code>AppearanceEditorPart.CssClass=text</code> | ✅ 一致 |
| <code>AppearanceEditorPart.DefaultButton=empty</code> | ✅ 一致 |
| <code>AppearanceEditorPart.DefaultButton=japanese</code> | ✅ 一致 |
| <code>AppearanceEditorPart.DefaultButton=markup</code> | ✅ 一致 |
| <code>AppearanceEditorPart.DefaultButton=text</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Description=empty</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Description=japanese</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Description=markup</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Description=text</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Direction=LeftToRight</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Direction=NotSet</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Direction=RightToLeft</code> | ✅ 一致 |
| <code>AppearanceEditorPart.EnableTheming=false</code> | ✅ 一致 |
| <code>AppearanceEditorPart.EnableTheming=true</code> | ✅ 一致 |
| <code>AppearanceEditorPart.EnableViewState=false</code> | ✅ 一致 |
| <code>AppearanceEditorPart.EnableViewState=true</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Enabled=false</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Enabled=true</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Font.Bold=false</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Font.Bold=true</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Font.Italic=false</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Font.Italic=true</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Font.Name=empty</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Font.Name=japanese</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Font.Name=markup</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Font.Name=text</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Font.Overline=false</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Font.Overline=true</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Font.Size=12pt</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Font.Size=150%</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Font.Size=Large</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Font.Strikeout=false</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Font.Strikeout=true</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Font.Underline=false</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Font.Underline=true</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ForeColor=Red</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ForeColor=Transparent</code> | ✅ 一致 |
| <code>AppearanceEditorPart.GroupingText=empty</code> | ✅ 一致 |
| <code>AppearanceEditorPart.GroupingText=japanese</code> | ✅ 一致 |
| <code>AppearanceEditorPart.GroupingText=markup</code> | ✅ 一致 |
| <code>AppearanceEditorPart.GroupingText=text</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Height=120px</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Height=2em</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Height=50%</code> | ✅ 一致 |
| <code>AppearanceEditorPart.HorizontalAlign=Center</code> | ✅ 一致 |
| <code>AppearanceEditorPart.HorizontalAlign=Justify</code> | ✅ 一致 |
| <code>AppearanceEditorPart.HorizontalAlign=Left</code> | ✅ 一致 |
| <code>AppearanceEditorPart.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>AppearanceEditorPart.HorizontalAlign=Right</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ID=empty</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ID=japanese</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ID=markup</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ID=text</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ScrollBars=Auto</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ScrollBars=Both</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ScrollBars=Horizontal</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ScrollBars=None</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ScrollBars=Vertical</code> | ✅ 一致 |
| <code>AppearanceEditorPart.SkinID=empty</code> | ✅ 一致 |
| <code>AppearanceEditorPart.SkinID=japanese</code> | ✅ 一致 |
| <code>AppearanceEditorPart.SkinID=markup</code> | ✅ 一致 |
| <code>AppearanceEditorPart.SkinID=text</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Style[color]=1em</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Style[color]=red</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Style[margin-left]=red</code> | ✅ 一致 |
| <code>AppearanceEditorPart.TabIndex=-1</code> | ✅ 一致 |
| <code>AppearanceEditorPart.TabIndex=0</code> | ✅ 一致 |
| <code>AppearanceEditorPart.TabIndex=3</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Title=empty</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Title=japanese</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Title=markup</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Title=markup+Description=text</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Title=text</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ToolTip=empty</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ToolTip=japanese</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ToolTip=markup</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ToolTip=text</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>AppearanceEditorPart.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Visible=false</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Visible=true</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Width=120px</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Width=2em</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Width=50%</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Wrap=false</code> | ✅ 一致 |
| <code>AppearanceEditorPart.Wrap=true</code> | ✅ 一致 |
| <code>AppearanceEditorPart[naming container].(default)</code> | ✅ 一致 |
| <code>AppearanceEditorPart[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>AppearanceEditorPart[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>AppearanceEditorPart[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>AppearanceEditorPart[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>AppearanceEditorPart[naming container].ID=text</code> | ✅ 一致 |
| <code>AppearanceEditorPart[themed].(default)</code> | ✅ 一致 |
| <code>AppearanceEditorPart[themed].CssClass=text</code> | ✅ 一致 |
| <code>AppearanceEditorPart[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>AppearanceEditorPart[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| AppearanceEditorPart [render][databind][dynamic] first request (browse): the parts in their zones, a zone made in code | ✅ 一致 |
| AppearanceEditorPart [viewstate] the design mode | ✅ 一致 |
| AppearanceEditorPart [viewstate] the catalog mode: the catalog zone and its parts | ✅ 一致 |
| AppearanceEditorPart [viewstate] a part added from the catalog (its check box posted, the zone&#39;s Add) | ✅ 一致 |
| AppearanceEditorPart [viewstate][templates] the edit mode: the editor zone and its parts for the first part | ✅ 一致 |
| AppearanceEditorPart [postbackevent] the first part&#39;s title edited (the editor zone&#39;s OK) | ✅ 一致 |
| AppearanceEditorPart [viewstate] the connect mode for the first part | ✅ 一致 |
| AppearanceEditorPart [viewstate] the connections zone closed | ✅ 一致 |
| AppearanceEditorPart [viewstate] a part minimized by its zone&#39;s verb | ✅ 一致 |
| AppearanceEditorPart [viewstate] restored | ✅ 一致 |
| AppearanceEditorPart [viewstate] a part of the zone made in code minimized | ✅ 一致 |
| AppearanceEditorPart [visible][viewstate] a hidden zone shown | ✅ 一致 |

</details>
