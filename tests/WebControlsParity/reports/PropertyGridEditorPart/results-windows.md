# PropertyGridEditorPart パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 00:36
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
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
| <code>PropertyGridEditorPart.Api.Control.#ctor</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.Adapter</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.ClientID</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.Context</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.Controls</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.DataBind</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.Dispose</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.Disposed</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.Events</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.FindControl</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.Focus</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.HasControls</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.ID</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.Init</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.Load</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.OnInit</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.Page</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.Parent</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.PreRender</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.Render</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.Site</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.SkinID</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.Unload</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.ViewState</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.Control.Visible</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.ICompositeControlDesignerAccessor.RecreateChildControls</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.Panel.#ctor</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.Panel.AddAttributesToRender</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.Panel.BackImageUrl</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.Panel.CreateControlStyle</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.Panel.DefaultButton</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.Panel.Direction</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.Panel.GroupingText</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.Panel.HorizontalAlign</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.Panel.RenderBeginTag</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.Panel.RenderEndTag</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.Panel.ScrollBars</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.Panel.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.Panel.Wrap</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebControl.Width</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebParts.EditorPart.#ctor</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebParts.EditorPart.ApplyChanges</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebParts.EditorPart.Display</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebParts.EditorPart.DisplayTitle</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebParts.EditorPart.GetDesignModeState</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebParts.EditorPart.OnPreRender</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebParts.EditorPart.SetDesignModeState</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebParts.EditorPart.SyncChanges</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebParts.EditorPart.WebPartManager</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebParts.EditorPart.WebPartToEdit</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebParts.EditorPart.Zone</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebParts.Part.ChromeState</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebParts.Part.ChromeType</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebParts.Part.Controls</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebParts.Part.DataBind</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebParts.Part.Description</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebParts.Part.Title</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebParts.PropertyGridEditorPart.#ctor</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebParts.PropertyGridEditorPart.ApplyChanges</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebParts.PropertyGridEditorPart.DefaultButton</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebParts.PropertyGridEditorPart.Display</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebParts.PropertyGridEditorPart.SyncChanges</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Api.WebControls.WebParts.PropertyGridEditorPart.Title</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>PropertyGridEditorPart.(case list)</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.(default)</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.AccessKey=empty</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.AccessKey=japanese</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.AccessKey=k</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.AccessKey=markup</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.AccessKey=text</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.BackColor=#3366CC</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.BackColor=Red</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.BackColor=Transparent</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.BackImageUrl=empty</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.BackImageUrl=japanese</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.BackImageUrl=markup</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.BackImageUrl=text</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.BorderColor=Red</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.BorderColor=Transparent</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.BorderStyle=Double</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.BorderStyle=Groove</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.BorderStyle=Inset</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.BorderStyle=None</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.BorderStyle=Outset</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.BorderStyle=Solid</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.BorderWidth=120px</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.BorderWidth=2em</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.BorderWidth=50%</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ChromeState=Minimized</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ChromeState=Normal</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ChromeType=BorderOnly</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ChromeType=Default</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ChromeType=None</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ChromeType=TitleAndBorder</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ChromeType=TitleOnly</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ClientIDMode=Static</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.CssClass=empty</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.CssClass=japanese</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.CssClass=markup</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.CssClass=text</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.DefaultButton=empty</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.DefaultButton=japanese</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.DefaultButton=markup</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.DefaultButton=text</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Description=empty</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Description=japanese</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Description=markup</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Description=text</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Direction=LeftToRight</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Direction=NotSet</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Direction=RightToLeft</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.EnableTheming=false</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.EnableTheming=true</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.EnableViewState=false</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.EnableViewState=true</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Enabled=false</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Enabled=true</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Font.Bold=false</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Font.Bold=true</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Font.Italic=false</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Font.Italic=true</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Font.Name=empty</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Font.Name=japanese</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Font.Name=markup</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Font.Name=text</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Font.Overline=false</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Font.Overline=true</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Font.Size=12pt</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Font.Size=150%</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Font.Size=Large</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Font.Strikeout=false</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Font.Strikeout=true</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Font.Underline=false</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Font.Underline=true</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ForeColor=Red</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ForeColor=Transparent</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.GroupingText=empty</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.GroupingText=japanese</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.GroupingText=markup</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.GroupingText=text</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Height=120px</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Height=2em</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Height=50%</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.HorizontalAlign=Center</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.HorizontalAlign=Justify</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.HorizontalAlign=Left</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.HorizontalAlign=Right</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ID=empty</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ID=japanese</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ID=markup</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ID=text</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ScrollBars=Auto</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ScrollBars=Both</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ScrollBars=Horizontal</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ScrollBars=None</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ScrollBars=Vertical</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.SkinID=empty</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.SkinID=japanese</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.SkinID=markup</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.SkinID=text</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Style[color]=1em</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Style[color]=red</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Style[margin-left]=red</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.TabIndex=-1</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.TabIndex=0</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.TabIndex=3</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Title=empty</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Title=japanese</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Title=markup</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Title=markup+Description=text</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Title=text</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ToolTip=empty</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ToolTip=japanese</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ToolTip=markup</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ToolTip=text</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Visible=false</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Visible=true</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Width=120px</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Width=2em</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Width=50%</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Wrap=false</code> | ✅ 一致 |
| <code>PropertyGridEditorPart.Wrap=true</code> | ✅ 一致 |
| <code>PropertyGridEditorPart[naming container].(default)</code> | ✅ 一致 |
| <code>PropertyGridEditorPart[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>PropertyGridEditorPart[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>PropertyGridEditorPart[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>PropertyGridEditorPart[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>PropertyGridEditorPart[naming container].ID=text</code> | ✅ 一致 |
| <code>PropertyGridEditorPart[themed].(default)</code> | ✅ 一致 |
| <code>PropertyGridEditorPart[themed].CssClass=text</code> | ✅ 一致 |
| <code>PropertyGridEditorPart[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>PropertyGridEditorPart[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| PropertyGridEditorPart [render][databind][dynamic] first request (browse): the parts in their zones, a zone made in code | ✅ 一致 |
| PropertyGridEditorPart [viewstate] the design mode | ✅ 一致 |
| PropertyGridEditorPart [viewstate] the catalog mode: the catalog zone and its parts | ✅ 一致 |
| PropertyGridEditorPart [viewstate] a part added from the catalog (its check box posted, the zone&#39;s Add) | ✅ 一致 |
| PropertyGridEditorPart [viewstate][templates] the edit mode: the editor zone and its parts for the first part | ✅ 一致 |
| PropertyGridEditorPart [viewstate] the first part&#39;s title edited (the editor zone&#39;s OK) | ✅ 一致 |
| PropertyGridEditorPart [viewstate] the connect mode for the first part | ✅ 一致 |
| PropertyGridEditorPart [viewstate] the connections zone closed | ✅ 一致 |
| PropertyGridEditorPart [viewstate] a part minimized by its zone&#39;s verb | ✅ 一致 |
| PropertyGridEditorPart [viewstate] restored | ✅ 一致 |
| PropertyGridEditorPart [viewstate] a part of the zone made in code minimized | ✅ 一致 |
| PropertyGridEditorPart [visible][viewstate] a hidden zone shown | ✅ 一致 |

</details>
