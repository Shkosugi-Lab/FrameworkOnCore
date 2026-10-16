# BehaviorEditorPart パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 01:26
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
| <code>BehaviorEditorPart.Api.Control.#ctor</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.Adapter</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.ClientID</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.Context</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.Controls</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.DataBind</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.Dispose</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.Disposed</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.Events</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.FindControl</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.Focus</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.HasControls</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.ID</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.Init</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.Load</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.OnInit</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.Page</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.Parent</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.PreRender</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.Render</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.Site</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.SkinID</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.Unload</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.ViewState</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.Control.Visible</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.ICompositeControlDesignerAccessor.RecreateChildControls</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.Panel.#ctor</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.Panel.AddAttributesToRender</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.Panel.BackImageUrl</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.Panel.CreateControlStyle</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.Panel.DefaultButton</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.Panel.Direction</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.Panel.GroupingText</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.Panel.HorizontalAlign</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.Panel.RenderBeginTag</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.Panel.RenderEndTag</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.Panel.ScrollBars</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.Panel.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.Panel.Wrap</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebControl.Width</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebParts.BehaviorEditorPart.#ctor</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebParts.BehaviorEditorPart.ApplyChanges</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebParts.BehaviorEditorPart.DefaultButton</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebParts.BehaviorEditorPart.Display</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebParts.BehaviorEditorPart.SyncChanges</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebParts.BehaviorEditorPart.Title</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebParts.EditorPart.#ctor</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebParts.EditorPart.ApplyChanges</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebParts.EditorPart.Display</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebParts.EditorPart.DisplayTitle</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebParts.EditorPart.GetDesignModeState</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebParts.EditorPart.OnPreRender</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebParts.EditorPart.SetDesignModeState</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebParts.EditorPart.SyncChanges</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebParts.EditorPart.WebPartManager</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebParts.EditorPart.WebPartToEdit</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebParts.EditorPart.Zone</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebParts.Part.ChromeState</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebParts.Part.ChromeType</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebParts.Part.Controls</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebParts.Part.DataBind</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebParts.Part.Description</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Api.WebControls.WebParts.Part.Title</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>BehaviorEditorPart.(case list)</code> | ✅ 一致 |
| <code>BehaviorEditorPart.(default)</code> | ✅ 一致 |
| <code>BehaviorEditorPart.AccessKey=empty</code> | ✅ 一致 |
| <code>BehaviorEditorPart.AccessKey=japanese</code> | ✅ 一致 |
| <code>BehaviorEditorPart.AccessKey=k</code> | ✅ 一致 |
| <code>BehaviorEditorPart.AccessKey=markup</code> | ✅ 一致 |
| <code>BehaviorEditorPart.AccessKey=text</code> | ✅ 一致 |
| <code>BehaviorEditorPart.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>BehaviorEditorPart.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>BehaviorEditorPart.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>BehaviorEditorPart.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>BehaviorEditorPart.BackColor=#3366CC</code> | ✅ 一致 |
| <code>BehaviorEditorPart.BackColor=Red</code> | ✅ 一致 |
| <code>BehaviorEditorPart.BackColor=Transparent</code> | ✅ 一致 |
| <code>BehaviorEditorPart.BackImageUrl=empty</code> | ✅ 一致 |
| <code>BehaviorEditorPart.BackImageUrl=japanese</code> | ✅ 一致 |
| <code>BehaviorEditorPart.BackImageUrl=markup</code> | ✅ 一致 |
| <code>BehaviorEditorPart.BackImageUrl=text</code> | ✅ 一致 |
| <code>BehaviorEditorPart.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>BehaviorEditorPart.BorderColor=Red</code> | ✅ 一致 |
| <code>BehaviorEditorPart.BorderColor=Transparent</code> | ✅ 一致 |
| <code>BehaviorEditorPart.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>BehaviorEditorPart.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>BehaviorEditorPart.BorderStyle=Double</code> | ✅ 一致 |
| <code>BehaviorEditorPart.BorderStyle=Groove</code> | ✅ 一致 |
| <code>BehaviorEditorPart.BorderStyle=Inset</code> | ✅ 一致 |
| <code>BehaviorEditorPart.BorderStyle=None</code> | ✅ 一致 |
| <code>BehaviorEditorPart.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>BehaviorEditorPart.BorderStyle=Outset</code> | ✅ 一致 |
| <code>BehaviorEditorPart.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>BehaviorEditorPart.BorderStyle=Solid</code> | ✅ 一致 |
| <code>BehaviorEditorPart.BorderWidth=120px</code> | ✅ 一致 |
| <code>BehaviorEditorPart.BorderWidth=2em</code> | ✅ 一致 |
| <code>BehaviorEditorPart.BorderWidth=50%</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ChromeState=Minimized</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ChromeState=Normal</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ChromeType=BorderOnly</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ChromeType=Default</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ChromeType=None</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ChromeType=TitleAndBorder</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ChromeType=TitleOnly</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ClientIDMode=Static</code> | ✅ 一致 |
| <code>BehaviorEditorPart.CssClass=empty</code> | ✅ 一致 |
| <code>BehaviorEditorPart.CssClass=japanese</code> | ✅ 一致 |
| <code>BehaviorEditorPart.CssClass=markup</code> | ✅ 一致 |
| <code>BehaviorEditorPart.CssClass=text</code> | ✅ 一致 |
| <code>BehaviorEditorPart.DefaultButton=empty</code> | ✅ 一致 |
| <code>BehaviorEditorPart.DefaultButton=japanese</code> | ✅ 一致 |
| <code>BehaviorEditorPart.DefaultButton=markup</code> | ✅ 一致 |
| <code>BehaviorEditorPart.DefaultButton=text</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Description=empty</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Description=japanese</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Description=markup</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Description=text</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Direction=LeftToRight</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Direction=NotSet</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Direction=RightToLeft</code> | ✅ 一致 |
| <code>BehaviorEditorPart.EnableTheming=false</code> | ✅ 一致 |
| <code>BehaviorEditorPart.EnableTheming=true</code> | ✅ 一致 |
| <code>BehaviorEditorPart.EnableViewState=false</code> | ✅ 一致 |
| <code>BehaviorEditorPart.EnableViewState=true</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Enabled=false</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Enabled=true</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Font.Bold=false</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Font.Bold=true</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Font.Italic=false</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Font.Italic=true</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Font.Name=empty</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Font.Name=japanese</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Font.Name=markup</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Font.Name=text</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Font.Overline=false</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Font.Overline=true</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Font.Size=12pt</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Font.Size=150%</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Font.Size=Large</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Font.Strikeout=false</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Font.Strikeout=true</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Font.Underline=false</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Font.Underline=true</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ForeColor=Red</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ForeColor=Transparent</code> | ✅ 一致 |
| <code>BehaviorEditorPart.GroupingText=empty</code> | ✅ 一致 |
| <code>BehaviorEditorPart.GroupingText=japanese</code> | ✅ 一致 |
| <code>BehaviorEditorPart.GroupingText=markup</code> | ✅ 一致 |
| <code>BehaviorEditorPart.GroupingText=text</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Height=120px</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Height=2em</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Height=50%</code> | ✅ 一致 |
| <code>BehaviorEditorPart.HorizontalAlign=Center</code> | ✅ 一致 |
| <code>BehaviorEditorPart.HorizontalAlign=Justify</code> | ✅ 一致 |
| <code>BehaviorEditorPart.HorizontalAlign=Left</code> | ✅ 一致 |
| <code>BehaviorEditorPart.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>BehaviorEditorPart.HorizontalAlign=Right</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ID=empty</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ID=japanese</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ID=markup</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ID=text</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ScrollBars=Auto</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ScrollBars=Both</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ScrollBars=Horizontal</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ScrollBars=None</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ScrollBars=Vertical</code> | ✅ 一致 |
| <code>BehaviorEditorPart.SkinID=empty</code> | ✅ 一致 |
| <code>BehaviorEditorPart.SkinID=japanese</code> | ✅ 一致 |
| <code>BehaviorEditorPart.SkinID=markup</code> | ✅ 一致 |
| <code>BehaviorEditorPart.SkinID=text</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Style[color]=1em</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Style[color]=red</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Style[margin-left]=red</code> | ✅ 一致 |
| <code>BehaviorEditorPart.TabIndex=-1</code> | ✅ 一致 |
| <code>BehaviorEditorPart.TabIndex=0</code> | ✅ 一致 |
| <code>BehaviorEditorPart.TabIndex=3</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Title=empty</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Title=japanese</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Title=markup</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Title=markup+Description=text</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Title=text</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ToolTip=empty</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ToolTip=japanese</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ToolTip=markup</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ToolTip=text</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>BehaviorEditorPart.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Visible=false</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Visible=true</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Width=120px</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Width=2em</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Width=50%</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Wrap=false</code> | ✅ 一致 |
| <code>BehaviorEditorPart.Wrap=true</code> | ✅ 一致 |
| <code>BehaviorEditorPart[naming container].(default)</code> | ✅ 一致 |
| <code>BehaviorEditorPart[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>BehaviorEditorPart[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>BehaviorEditorPart[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>BehaviorEditorPart[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>BehaviorEditorPart[naming container].ID=text</code> | ✅ 一致 |
| <code>BehaviorEditorPart[themed].(default)</code> | ✅ 一致 |
| <code>BehaviorEditorPart[themed].CssClass=text</code> | ✅ 一致 |
| <code>BehaviorEditorPart[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>BehaviorEditorPart[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| BehaviorEditorPart [render][databind][dynamic] first request (browse): the parts in their zones, a zone made in code | ✅ 一致 |
| BehaviorEditorPart [viewstate] the design mode | ✅ 一致 |
| BehaviorEditorPart [viewstate] the catalog mode: the catalog zone and its parts | ✅ 一致 |
| BehaviorEditorPart [viewstate] a part added from the catalog (its check box posted, the zone&#39;s Add) | ✅ 一致 |
| BehaviorEditorPart [viewstate][templates] the edit mode: the editor zone and its parts for the first part | ✅ 一致 |
| BehaviorEditorPart [viewstate] the first part&#39;s title edited (the editor zone&#39;s OK) | ✅ 一致 |
| BehaviorEditorPart [viewstate] the connect mode for the first part | ✅ 一致 |
| BehaviorEditorPart [viewstate] the connections zone closed | ✅ 一致 |
| BehaviorEditorPart [viewstate] a part minimized by its zone&#39;s verb | ✅ 一致 |
| BehaviorEditorPart [viewstate] restored | ✅ 一致 |
| BehaviorEditorPart [viewstate] a part of the zone made in code minimized | ✅ 一致 |
| BehaviorEditorPart [visible][viewstate] a hidden zone shown | ✅ 一致 |

</details>
