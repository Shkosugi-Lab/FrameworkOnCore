# Table パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 00:51
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 161 | 161 | 0 | 0 |
| ② HTML | 253 | 253 | 0 | 0 |
| ③ シナリオ | 14 | 14 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 170 件中 170 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 36 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 14 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 138 | 1 | ✅ |
| ViewState の保存と復元 | 11 | 12 | ❌ 1 件 |
| Visible=false（描画されず状態は保持） | 3 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 3 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 3 | 0 | ✅ |
| ポストバックイベント | 0 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>Table.Api.Control.#ctor</code> | ✅ 一致 |
| <code>Table.Api.Control.Adapter</code> | ✅ 一致 |
| <code>Table.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>Table.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>Table.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>Table.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>Table.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>Table.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>Table.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>Table.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>Table.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>Table.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>Table.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>Table.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>Table.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>Table.Api.Control.ClientID</code> | ✅ 一致 |
| <code>Table.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>Table.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>Table.Api.Control.Context</code> | ✅ 一致 |
| <code>Table.Api.Control.Controls</code> | ✅ 一致 |
| <code>Table.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>Table.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>Table.Api.Control.DataBind</code> | ✅ 一致 |
| <code>Table.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>Table.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>Table.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>Table.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>Table.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>Table.Api.Control.Dispose</code> | ✅ 一致 |
| <code>Table.Api.Control.Disposed</code> | ✅ 一致 |
| <code>Table.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>Table.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>Table.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>Table.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>Table.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>Table.Api.Control.Events</code> | ✅ 一致 |
| <code>Table.Api.Control.FindControl</code> | ✅ 一致 |
| <code>Table.Api.Control.Focus</code> | ✅ 一致 |
| <code>Table.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>Table.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>Table.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>Table.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>Table.Api.Control.HasControls</code> | ✅ 一致 |
| <code>Table.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>Table.Api.Control.ID</code> | ✅ 一致 |
| <code>Table.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>Table.Api.Control.Init</code> | ✅ 一致 |
| <code>Table.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>Table.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>Table.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>Table.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>Table.Api.Control.Load</code> | ✅ 一致 |
| <code>Table.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>Table.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>Table.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>Table.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>Table.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>Table.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>Table.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>Table.Api.Control.OnInit</code> | ✅ 一致 |
| <code>Table.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>Table.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>Table.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>Table.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>Table.Api.Control.Page</code> | ✅ 一致 |
| <code>Table.Api.Control.Parent</code> | ✅ 一致 |
| <code>Table.Api.Control.PreRender</code> | ✅ 一致 |
| <code>Table.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>Table.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>Table.Api.Control.Render</code> | ✅ 一致 |
| <code>Table.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>Table.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>Table.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>Table.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>Table.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>Table.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>Table.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>Table.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>Table.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>Table.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>Table.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>Table.Api.Control.Site</code> | ✅ 一致 |
| <code>Table.Api.Control.SkinID</code> | ✅ 一致 |
| <code>Table.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>Table.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>Table.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>Table.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>Table.Api.Control.Unload</code> | ✅ 一致 |
| <code>Table.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>Table.Api.Control.ViewState</code> | ✅ 一致 |
| <code>Table.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>Table.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>Table.Api.Control.Visible</code> | ✅ 一致 |
| <code>Table.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>Table.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>Table.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>Table.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>Table.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>Table.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>Table.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>Table.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>Table.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>Table.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>Table.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>Table.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>Table.Api.IPostBackEventHandler.RaisePostBackEvent</code> | ✅ 一致 |
| <code>Table.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>Table.Api.WebControls.Table.#ctor</code> | ✅ 一致 |
| <code>Table.Api.WebControls.Table.AddAttributesToRender</code> | ✅ 一致 |
| <code>Table.Api.WebControls.Table.BackImageUrl</code> | ✅ 一致 |
| <code>Table.Api.WebControls.Table.Caption</code> | ✅ 一致 |
| <code>Table.Api.WebControls.Table.CaptionAlign</code> | ✅ 一致 |
| <code>Table.Api.WebControls.Table.CellPadding</code> | ✅ 一致 |
| <code>Table.Api.WebControls.Table.CellSpacing</code> | ✅ 一致 |
| <code>Table.Api.WebControls.Table.CreateControlCollection</code> | ✅ 一致 |
| <code>Table.Api.WebControls.Table.CreateControlStyle</code> | ✅ 一致 |
| <code>Table.Api.WebControls.Table.GridLines</code> | ✅ 一致 |
| <code>Table.Api.WebControls.Table.HorizontalAlign</code> | ✅ 一致 |
| <code>Table.Api.WebControls.Table.RaisePostBackEvent</code> | ✅ 一致 |
| <code>Table.Api.WebControls.Table.RenderBeginTag</code> | ✅ 一致 |
| <code>Table.Api.WebControls.Table.RenderContents</code> | ✅ 一致 |
| <code>Table.Api.WebControls.Table.Rows</code> | ✅ 一致 |
| <code>Table.Api.WebControls.Table.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>Table.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>Table.(case list)</code> | ✅ 一致 |
| <code>Table.(default)</code> | ✅ 一致 |
| <code>Table.AccessKey=empty</code> | ✅ 一致 |
| <code>Table.AccessKey=japanese</code> | ✅ 一致 |
| <code>Table.AccessKey=k</code> | ✅ 一致 |
| <code>Table.AccessKey=markup</code> | ✅ 一致 |
| <code>Table.AccessKey=text</code> | ✅ 一致 |
| <code>Table.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>Table.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>Table.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>Table.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>Table.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>Table.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>Table.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>Table.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>Table.BackColor=#3366CC</code> | ✅ 一致 |
| <code>Table.BackColor=Red</code> | ✅ 一致 |
| <code>Table.BackColor=Transparent</code> | ✅ 一致 |
| <code>Table.BackImageUrl=app-relative</code> | ✅ 一致 |
| <code>Table.BackImageUrl=app-relative+GridLines=Both</code> | ✅ 一致 |
| <code>Table.BackImageUrl=empty</code> | ✅ 一致 |
| <code>Table.BackImageUrl=japanese</code> | ✅ 一致 |
| <code>Table.BackImageUrl=markup</code> | ✅ 一致 |
| <code>Table.BackImageUrl=text</code> | ✅ 一致 |
| <code>Table.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>Table.BorderColor=Red</code> | ✅ 一致 |
| <code>Table.BorderColor=Transparent</code> | ✅ 一致 |
| <code>Table.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>Table.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>Table.BorderStyle=Double</code> | ✅ 一致 |
| <code>Table.BorderStyle=Groove</code> | ✅ 一致 |
| <code>Table.BorderStyle=Inset</code> | ✅ 一致 |
| <code>Table.BorderStyle=None</code> | ✅ 一致 |
| <code>Table.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>Table.BorderStyle=Outset</code> | ✅ 一致 |
| <code>Table.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>Table.BorderStyle=Solid</code> | ✅ 一致 |
| <code>Table.BorderWidth=120px</code> | ✅ 一致 |
| <code>Table.BorderWidth=2em</code> | ✅ 一致 |
| <code>Table.BorderWidth=50%</code> | ✅ 一致 |
| <code>Table.Caption=empty</code> | ✅ 一致 |
| <code>Table.Caption=japanese</code> | ✅ 一致 |
| <code>Table.Caption=markup</code> | ✅ 一致 |
| <code>Table.Caption=text</code> | ✅ 一致 |
| <code>Table.Caption=text+CaptionAlign=Bottom</code> | ✅ 一致 |
| <code>Table.CaptionAlign=Bottom</code> | ✅ 一致 |
| <code>Table.CaptionAlign=Left</code> | ✅ 一致 |
| <code>Table.CaptionAlign=NotSet</code> | ✅ 一致 |
| <code>Table.CaptionAlign=Right</code> | ✅ 一致 |
| <code>Table.CaptionAlign=Top</code> | ✅ 一致 |
| <code>Table.CellPadding=-1</code> | ✅ 一致 |
| <code>Table.CellPadding=0</code> | ✅ 一致 |
| <code>Table.CellPadding=3</code> | ✅ 一致 |
| <code>Table.CellSpacing=-1</code> | ✅ 一致 |
| <code>Table.CellSpacing=0</code> | ✅ 一致 |
| <code>Table.CellSpacing=3</code> | ✅ 一致 |
| <code>Table.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Table.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Table.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Table.ClientIDMode=Static</code> | ✅ 一致 |
| <code>Table.CssClass=empty</code> | ✅ 一致 |
| <code>Table.CssClass=japanese</code> | ✅ 一致 |
| <code>Table.CssClass=markup</code> | ✅ 一致 |
| <code>Table.CssClass=text</code> | ✅ 一致 |
| <code>Table.CssClass=text+EnableViewState=false</code> | ✅ 一致 |
| <code>Table.CssClass=text+Visible=false</code> | ✅ 一致 |
| <code>Table.EnableTheming=false</code> | ✅ 一致 |
| <code>Table.EnableTheming=true</code> | ✅ 一致 |
| <code>Table.EnableViewState=false</code> | ✅ 一致 |
| <code>Table.EnableViewState=true</code> | ✅ 一致 |
| <code>Table.Enabled=false</code> | ✅ 一致 |
| <code>Table.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>Table.Enabled=true</code> | ✅ 一致 |
| <code>Table.Font.Bold=false</code> | ✅ 一致 |
| <code>Table.Font.Bold=true</code> | ✅ 一致 |
| <code>Table.Font.Italic=false</code> | ✅ 一致 |
| <code>Table.Font.Italic=true</code> | ✅ 一致 |
| <code>Table.Font.Name=empty</code> | ✅ 一致 |
| <code>Table.Font.Name=japanese</code> | ✅ 一致 |
| <code>Table.Font.Name=markup</code> | ✅ 一致 |
| <code>Table.Font.Name=text</code> | ✅ 一致 |
| <code>Table.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>Table.Font.Overline=false</code> | ✅ 一致 |
| <code>Table.Font.Overline=true</code> | ✅ 一致 |
| <code>Table.Font.Size=12pt</code> | ✅ 一致 |
| <code>Table.Font.Size=150%</code> | ✅ 一致 |
| <code>Table.Font.Size=Large</code> | ✅ 一致 |
| <code>Table.Font.Strikeout=false</code> | ✅ 一致 |
| <code>Table.Font.Strikeout=true</code> | ✅ 一致 |
| <code>Table.Font.Underline=false</code> | ✅ 一致 |
| <code>Table.Font.Underline=true</code> | ✅ 一致 |
| <code>Table.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>Table.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>Table.ForeColor=Red</code> | ✅ 一致 |
| <code>Table.ForeColor=Transparent</code> | ✅ 一致 |
| <code>Table.GridLines=Both</code> | ✅ 一致 |
| <code>Table.GridLines=Both+CellPadding=3+CellSpacing=0</code> | ✅ 一致 |
| <code>Table.GridLines=Horizontal</code> | ✅ 一致 |
| <code>Table.GridLines=None</code> | ✅ 一致 |
| <code>Table.GridLines=Vertical</code> | ✅ 一致 |
| <code>Table.Height=120px</code> | ✅ 一致 |
| <code>Table.Height=2em</code> | ✅ 一致 |
| <code>Table.Height=50%</code> | ✅ 一致 |
| <code>Table.HorizontalAlign=Center</code> | ✅ 一致 |
| <code>Table.HorizontalAlign=Center+Width=120px</code> | ✅ 一致 |
| <code>Table.HorizontalAlign=Justify</code> | ✅ 一致 |
| <code>Table.HorizontalAlign=Left</code> | ✅ 一致 |
| <code>Table.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>Table.HorizontalAlign=Right</code> | ✅ 一致 |
| <code>Table.ID=empty</code> | ✅ 一致 |
| <code>Table.ID=japanese</code> | ✅ 一致 |
| <code>Table.ID=markup</code> | ✅ 一致 |
| <code>Table.ID=text</code> | ✅ 一致 |
| <code>Table.Rows[1].AccessKey=empty</code> | ✅ 一致 |
| <code>Table.Rows[1].AccessKey=japanese</code> | ✅ 一致 |
| <code>Table.Rows[1].AccessKey=markup</code> | ✅ 一致 |
| <code>Table.Rows[1].AccessKey=text</code> | ✅ 一致 |
| <code>Table.Rows[1].AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>Table.Rows[1].AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>Table.Rows[1].AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>Table.Rows[1].AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>Table.Rows[1].BackColor=#3366CC</code> | ✅ 一致 |
| <code>Table.Rows[1].BackColor=Red</code> | ✅ 一致 |
| <code>Table.Rows[1].BackColor=Transparent</code> | ✅ 一致 |
| <code>Table.Rows[1].BorderColor=#3366CC</code> | ✅ 一致 |
| <code>Table.Rows[1].BorderColor=Red</code> | ✅ 一致 |
| <code>Table.Rows[1].BorderColor=Transparent</code> | ✅ 一致 |
| <code>Table.Rows[1].BorderStyle=Dashed</code> | ✅ 一致 |
| <code>Table.Rows[1].BorderStyle=Dotted</code> | ✅ 一致 |
| <code>Table.Rows[1].BorderStyle=Double</code> | ✅ 一致 |
| <code>Table.Rows[1].BorderStyle=Groove</code> | ✅ 一致 |
| <code>Table.Rows[1].BorderStyle=Inset</code> | ✅ 一致 |
| <code>Table.Rows[1].BorderStyle=None</code> | ✅ 一致 |
| <code>Table.Rows[1].BorderStyle=NotSet</code> | ✅ 一致 |
| <code>Table.Rows[1].BorderStyle=Outset</code> | ✅ 一致 |
| <code>Table.Rows[1].BorderStyle=Ridge</code> | ✅ 一致 |
| <code>Table.Rows[1].BorderStyle=Solid</code> | ✅ 一致 |
| <code>Table.Rows[1].BorderWidth=120px</code> | ✅ 一致 |
| <code>Table.Rows[1].BorderWidth=2em</code> | ✅ 一致 |
| <code>Table.Rows[1].BorderWidth=50%</code> | ✅ 一致 |
| <code>Table.Rows[1].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Table.Rows[1].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Table.Rows[1].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Table.Rows[1].ClientIDMode=Static</code> | ✅ 一致 |
| <code>Table.Rows[1].CssClass=empty</code> | ✅ 一致 |
| <code>Table.Rows[1].CssClass=japanese</code> | ✅ 一致 |
| <code>Table.Rows[1].CssClass=markup</code> | ✅ 一致 |
| <code>Table.Rows[1].CssClass=text</code> | ✅ 一致 |
| <code>Table.Rows[1].EnableTheming=false</code> | ✅ 一致 |
| <code>Table.Rows[1].EnableTheming=true</code> | ✅ 一致 |
| <code>Table.Rows[1].EnableViewState=false</code> | ✅ 一致 |
| <code>Table.Rows[1].EnableViewState=true</code> | ✅ 一致 |
| <code>Table.Rows[1].Enabled=false</code> | ✅ 一致 |
| <code>Table.Rows[1].Enabled=true</code> | ✅ 一致 |
| <code>Table.Rows[1].Font.Bold=false</code> | ✅ 一致 |
| <code>Table.Rows[1].Font.Bold=true</code> | ✅ 一致 |
| <code>Table.Rows[1].Font.Italic=false</code> | ✅ 一致 |
| <code>Table.Rows[1].Font.Italic=true</code> | ✅ 一致 |
| <code>Table.Rows[1].Font.Name=empty</code> | ✅ 一致 |
| <code>Table.Rows[1].Font.Name=japanese</code> | ✅ 一致 |
| <code>Table.Rows[1].Font.Name=markup</code> | ✅ 一致 |
| <code>Table.Rows[1].Font.Name=text</code> | ✅ 一致 |
| <code>Table.Rows[1].Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>Table.Rows[1].Font.Overline=false</code> | ✅ 一致 |
| <code>Table.Rows[1].Font.Overline=true</code> | ✅ 一致 |
| <code>Table.Rows[1].Font.Size=12pt</code> | ✅ 一致 |
| <code>Table.Rows[1].Font.Size=150%</code> | ✅ 一致 |
| <code>Table.Rows[1].Font.Size=Large</code> | ✅ 一致 |
| <code>Table.Rows[1].Font.Strikeout=false</code> | ✅ 一致 |
| <code>Table.Rows[1].Font.Strikeout=true</code> | ✅ 一致 |
| <code>Table.Rows[1].Font.Underline=false</code> | ✅ 一致 |
| <code>Table.Rows[1].Font.Underline=true</code> | ✅ 一致 |
| <code>Table.Rows[1].ForeColor=#3366CC</code> | ✅ 一致 |
| <code>Table.Rows[1].ForeColor=Red</code> | ✅ 一致 |
| <code>Table.Rows[1].ForeColor=Transparent</code> | ✅ 一致 |
| <code>Table.Rows[1].Height=120px</code> | ✅ 一致 |
| <code>Table.Rows[1].Height=2em</code> | ✅ 一致 |
| <code>Table.Rows[1].Height=50%</code> | ✅ 一致 |
| <code>Table.Rows[1].HorizontalAlign=Center</code> | ✅ 一致 |
| <code>Table.Rows[1].HorizontalAlign=Justify</code> | ✅ 一致 |
| <code>Table.Rows[1].HorizontalAlign=Left</code> | ✅ 一致 |
| <code>Table.Rows[1].HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>Table.Rows[1].HorizontalAlign=Right</code> | ✅ 一致 |
| <code>Table.Rows[1].ID=empty</code> | ✅ 一致 |
| <code>Table.Rows[1].ID=japanese</code> | ✅ 一致 |
| <code>Table.Rows[1].ID=markup</code> | ✅ 一致 |
| <code>Table.Rows[1].ID=text</code> | ✅ 一致 |
| <code>Table.Rows[1].SkinID=empty</code> | ✅ 一致 |
| <code>Table.Rows[1].SkinID=japanese</code> | ✅ 一致 |
| <code>Table.Rows[1].SkinID=markup</code> | ✅ 一致 |
| <code>Table.Rows[1].SkinID=text</code> | ✅ 一致 |
| <code>Table.Rows[1].TabIndex=-1</code> | ✅ 一致 |
| <code>Table.Rows[1].TabIndex=0</code> | ✅ 一致 |
| <code>Table.Rows[1].TabIndex=3</code> | ✅ 一致 |
| <code>Table.Rows[1].TableSection=TableBody</code> | ✅ 一致 |
| <code>Table.Rows[1].TableSection=TableFooter</code> | ✅ 一致 |
| <code>Table.Rows[1].TableSection=TableHeader</code> | ✅ 一致 |
| <code>Table.Rows[1].ToolTip=empty</code> | ✅ 一致 |
| <code>Table.Rows[1].ToolTip=japanese</code> | ✅ 一致 |
| <code>Table.Rows[1].ToolTip=markup</code> | ✅ 一致 |
| <code>Table.Rows[1].ToolTip=text</code> | ✅ 一致 |
| <code>Table.Rows[1].ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>Table.Rows[1].ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>Table.Rows[1].ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>Table.Rows[1].VerticalAlign=Bottom</code> | ✅ 一致 |
| <code>Table.Rows[1].VerticalAlign=Middle</code> | ✅ 一致 |
| <code>Table.Rows[1].VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>Table.Rows[1].VerticalAlign=Top</code> | ✅ 一致 |
| <code>Table.Rows[1].ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>Table.Rows[1].ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>Table.Rows[1].ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>Table.Rows[1].Visible=false</code> | ✅ 一致 |
| <code>Table.Rows[1].Visible=true</code> | ✅ 一致 |
| <code>Table.Rows[1].Width=120px</code> | ✅ 一致 |
| <code>Table.Rows[1].Width=2em</code> | ✅ 一致 |
| <code>Table.Rows[1].Width=50%</code> | ✅ 一致 |
| <code>Table.SkinID=empty</code> | ✅ 一致 |
| <code>Table.SkinID=japanese</code> | ✅ 一致 |
| <code>Table.SkinID=markup</code> | ✅ 一致 |
| <code>Table.SkinID=text</code> | ✅ 一致 |
| <code>Table.Style[color]=1em</code> | ✅ 一致 |
| <code>Table.Style[color]=red</code> | ✅ 一致 |
| <code>Table.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>Table.Style[margin-left]=red</code> | ✅ 一致 |
| <code>Table.TabIndex=-1</code> | ✅ 一致 |
| <code>Table.TabIndex=0</code> | ✅ 一致 |
| <code>Table.TabIndex=3</code> | ✅ 一致 |
| <code>Table.ToolTip=empty</code> | ✅ 一致 |
| <code>Table.ToolTip=japanese</code> | ✅ 一致 |
| <code>Table.ToolTip=markup</code> | ✅ 一致 |
| <code>Table.ToolTip=text</code> | ✅ 一致 |
| <code>Table.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>Table.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>Table.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>Table.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>Table.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>Table.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>Table.Visible=false</code> | ✅ 一致 |
| <code>Table.Visible=true</code> | ✅ 一致 |
| <code>Table.Width=120px</code> | ✅ 一致 |
| <code>Table.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>Table.Width=2em</code> | ✅ 一致 |
| <code>Table.Width=50%</code> | ✅ 一致 |
| <code>Table[naming container].(default)</code> | ✅ 一致 |
| <code>Table[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Table[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Table[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Table[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>Table[naming container].ID=text</code> | ✅ 一致 |
| <code>Table[themed].(default)</code> | ✅ 一致 |
| <code>Table[themed].CssClass=text</code> | ✅ 一致 |
| <code>Table[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>Table[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| Table [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| Table [viewstate][dynamic] posted: files, view state, what code set (a table&#39;s rows and a literal control&#39;s text are not kept) | ✅ 一致 |
| Table [viewstate] the next view (NextView bubbled from a button) | ✅ 一致 |
| Table [viewstate] switched by ID and by index: the first view&#39;s text box kept its value | ✅ 一致 |
| Table [dynamic][viewstate] a dynamic multi-view&#39;s next view | ✅ 一致 |
| Table [viewstate] a calendar&#39;s day chosen | ✅ 一致 |
| Table [viewstate] a calendar&#39;s week chosen | ❌ 未実行 |
| Table [viewstate] a calendar&#39;s next month | ✅ 一致 |
| Table [viewstate] a dynamic calendar&#39;s day chosen | ✅ 一致 |
| Table [viewstate] an image map&#39;s hot spot clicked (PostBack) | ✅ 一致 |
| Table [viewstate] a dynamic image map&#39;s hot spot clicked | ✅ 一致 |
| Table [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| Table [viewstate] a file upload validated on the server (g1): no file posted | ✅ 一致 |
| Table [postbackevent] a postback event to a table (not registered for event validation): rejected | ✅ 一致 |

</details>
