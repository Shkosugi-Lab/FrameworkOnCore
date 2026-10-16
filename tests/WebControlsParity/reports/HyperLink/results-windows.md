# HyperLink パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-16 16:53
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 156 | 156 | 0 | 0 |
| ② HTML | 153 | 153 | 0 | 0 |
| ③ シナリオ | 4 | 4 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 165 件中 165 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 35 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 4 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 140 | 2 | ✅ |
| ViewState の保存と復元 | 6 | 2 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 7 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 3 | 0 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>HyperLink.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.Context</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.Controls</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.Events</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.Focus</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.ID</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.Init</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.Load</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.Page</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.Parent</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.Render</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.Site</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.Unload</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HyperLink.Api.Control.Visible</code> | ✅ 一致 |
| <code>HyperLink.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HyperLink.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HyperLink.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HyperLink.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HyperLink.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HyperLink.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HyperLink.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HyperLink.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HyperLink.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HyperLink.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HyperLink.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HyperLink.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HyperLink.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.HyperLink.#ctor</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.HyperLink.AddAttributesToRender</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.HyperLink.AddParsedSubObject</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.HyperLink.ImageHeight</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.HyperLink.ImageUrl</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.HyperLink.ImageWidth</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.HyperLink.LoadViewState</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.HyperLink.NavigateUrl</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.HyperLink.RenderContents</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.HyperLink.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.HyperLink.Target</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.HyperLink.Text</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>HyperLink.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HyperLink.(case list)</code> | ✅ 一致 |
| <code>HyperLink.(default)</code> | ✅ 一致 |
| <code>HyperLink.AccessKey=empty</code> | ✅ 一致 |
| <code>HyperLink.AccessKey=japanese</code> | ✅ 一致 |
| <code>HyperLink.AccessKey=k</code> | ✅ 一致 |
| <code>HyperLink.AccessKey=markup</code> | ✅ 一致 |
| <code>HyperLink.AccessKey=text</code> | ✅ 一致 |
| <code>HyperLink.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HyperLink.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HyperLink.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HyperLink.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HyperLink.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HyperLink.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HyperLink.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HyperLink.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HyperLink.BackColor=#3366CC</code> | ✅ 一致 |
| <code>HyperLink.BackColor=Red</code> | ✅ 一致 |
| <code>HyperLink.BackColor=Transparent</code> | ✅ 一致 |
| <code>HyperLink.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>HyperLink.BorderColor=Red</code> | ✅ 一致 |
| <code>HyperLink.BorderColor=Transparent</code> | ✅ 一致 |
| <code>HyperLink.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>HyperLink.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>HyperLink.BorderStyle=Double</code> | ✅ 一致 |
| <code>HyperLink.BorderStyle=Groove</code> | ✅ 一致 |
| <code>HyperLink.BorderStyle=Inset</code> | ✅ 一致 |
| <code>HyperLink.BorderStyle=None</code> | ✅ 一致 |
| <code>HyperLink.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>HyperLink.BorderStyle=Outset</code> | ✅ 一致 |
| <code>HyperLink.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>HyperLink.BorderStyle=Solid</code> | ✅ 一致 |
| <code>HyperLink.BorderWidth=120px</code> | ✅ 一致 |
| <code>HyperLink.BorderWidth=2em</code> | ✅ 一致 |
| <code>HyperLink.BorderWidth=50%</code> | ✅ 一致 |
| <code>HyperLink.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HyperLink.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HyperLink.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HyperLink.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HyperLink.CssClass=empty</code> | ✅ 一致 |
| <code>HyperLink.CssClass=japanese</code> | ✅ 一致 |
| <code>HyperLink.CssClass=markup</code> | ✅ 一致 |
| <code>HyperLink.CssClass=text</code> | ✅ 一致 |
| <code>HyperLink.EnableTheming=false</code> | ✅ 一致 |
| <code>HyperLink.EnableTheming=true</code> | ✅ 一致 |
| <code>HyperLink.EnableViewState=false</code> | ✅ 一致 |
| <code>HyperLink.EnableViewState=true</code> | ✅ 一致 |
| <code>HyperLink.Enabled=false</code> | ✅ 一致 |
| <code>HyperLink.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>HyperLink.Enabled=true</code> | ✅ 一致 |
| <code>HyperLink.Font.Bold=false</code> | ✅ 一致 |
| <code>HyperLink.Font.Bold=true</code> | ✅ 一致 |
| <code>HyperLink.Font.Italic=false</code> | ✅ 一致 |
| <code>HyperLink.Font.Italic=true</code> | ✅ 一致 |
| <code>HyperLink.Font.Name=empty</code> | ✅ 一致 |
| <code>HyperLink.Font.Name=japanese</code> | ✅ 一致 |
| <code>HyperLink.Font.Name=markup</code> | ✅ 一致 |
| <code>HyperLink.Font.Name=text</code> | ✅ 一致 |
| <code>HyperLink.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>HyperLink.Font.Overline=false</code> | ✅ 一致 |
| <code>HyperLink.Font.Overline=true</code> | ✅ 一致 |
| <code>HyperLink.Font.Size=12pt</code> | ✅ 一致 |
| <code>HyperLink.Font.Size=150%</code> | ✅ 一致 |
| <code>HyperLink.Font.Size=Large</code> | ✅ 一致 |
| <code>HyperLink.Font.Strikeout=false</code> | ✅ 一致 |
| <code>HyperLink.Font.Strikeout=true</code> | ✅ 一致 |
| <code>HyperLink.Font.Underline=false</code> | ✅ 一致 |
| <code>HyperLink.Font.Underline=true</code> | ✅ 一致 |
| <code>HyperLink.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>HyperLink.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>HyperLink.ForeColor=Red</code> | ✅ 一致 |
| <code>HyperLink.ForeColor=Transparent</code> | ✅ 一致 |
| <code>HyperLink.Height=120px</code> | ✅ 一致 |
| <code>HyperLink.Height=2em</code> | ✅ 一致 |
| <code>HyperLink.Height=50%</code> | ✅ 一致 |
| <code>HyperLink.ID=empty</code> | ✅ 一致 |
| <code>HyperLink.ID=japanese</code> | ✅ 一致 |
| <code>HyperLink.ID=markup</code> | ✅ 一致 |
| <code>HyperLink.ID=text</code> | ✅ 一致 |
| <code>HyperLink.ImageHeight=120px</code> | ✅ 一致 |
| <code>HyperLink.ImageHeight=2em</code> | ✅ 一致 |
| <code>HyperLink.ImageHeight=50%</code> | ✅ 一致 |
| <code>HyperLink.ImageUrl=absolute</code> | ✅ 一致 |
| <code>HyperLink.ImageUrl=app-relative</code> | ✅ 一致 |
| <code>HyperLink.ImageUrl=app-relative+ImageWidth=120px+ImageHeight=2em</code> | ✅ 一致 |
| <code>HyperLink.ImageUrl=app-relative+Text=text</code> | ✅ 一致 |
| <code>HyperLink.ImageUrl=app-relative+ToolTip=text</code> | ✅ 一致 |
| <code>HyperLink.ImageUrl=empty</code> | ✅ 一致 |
| <code>HyperLink.ImageUrl=japanese</code> | ✅ 一致 |
| <code>HyperLink.ImageUrl=markup</code> | ✅ 一致 |
| <code>HyperLink.ImageUrl=text</code> | ✅ 一致 |
| <code>HyperLink.ImageWidth=120px</code> | ✅ 一致 |
| <code>HyperLink.ImageWidth=2em</code> | ✅ 一致 |
| <code>HyperLink.ImageWidth=50%</code> | ✅ 一致 |
| <code>HyperLink.NavigateUrl=absolute</code> | ✅ 一致 |
| <code>HyperLink.NavigateUrl=absolute+Target=blank</code> | ✅ 一致 |
| <code>HyperLink.NavigateUrl=app-relative</code> | ✅ 一致 |
| <code>HyperLink.NavigateUrl=app-relative+Enabled=false</code> | ✅ 一致 |
| <code>HyperLink.NavigateUrl=app-relative+Text=text</code> | ✅ 一致 |
| <code>HyperLink.NavigateUrl=empty</code> | ✅ 一致 |
| <code>HyperLink.NavigateUrl=japanese</code> | ✅ 一致 |
| <code>HyperLink.NavigateUrl=markup</code> | ✅ 一致 |
| <code>HyperLink.NavigateUrl=script</code> | ✅ 一致 |
| <code>HyperLink.NavigateUrl=text</code> | ✅ 一致 |
| <code>HyperLink.SkinID=empty</code> | ✅ 一致 |
| <code>HyperLink.SkinID=japanese</code> | ✅ 一致 |
| <code>HyperLink.SkinID=markup</code> | ✅ 一致 |
| <code>HyperLink.SkinID=text</code> | ✅ 一致 |
| <code>HyperLink.Style[color]=1em</code> | ✅ 一致 |
| <code>HyperLink.Style[color]=red</code> | ✅ 一致 |
| <code>HyperLink.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HyperLink.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HyperLink.TabIndex=-1</code> | ✅ 一致 |
| <code>HyperLink.TabIndex=0</code> | ✅ 一致 |
| <code>HyperLink.TabIndex=3</code> | ✅ 一致 |
| <code>HyperLink.Target=blank</code> | ✅ 一致 |
| <code>HyperLink.Target=empty</code> | ✅ 一致 |
| <code>HyperLink.Target=japanese</code> | ✅ 一致 |
| <code>HyperLink.Target=markup</code> | ✅ 一致 |
| <code>HyperLink.Target=text</code> | ✅ 一致 |
| <code>HyperLink.Text=empty</code> | ✅ 一致 |
| <code>HyperLink.Text=japanese</code> | ✅ 一致 |
| <code>HyperLink.Text=markup</code> | ✅ 一致 |
| <code>HyperLink.Text=markup+NavigateUrl=script</code> | ✅ 一致 |
| <code>HyperLink.Text=text</code> | ✅ 一致 |
| <code>HyperLink.Text=text+EnableViewState=false</code> | ✅ 一致 |
| <code>HyperLink.Text=text+Visible=false</code> | ✅ 一致 |
| <code>HyperLink.ToolTip=empty</code> | ✅ 一致 |
| <code>HyperLink.ToolTip=japanese</code> | ✅ 一致 |
| <code>HyperLink.ToolTip=markup</code> | ✅ 一致 |
| <code>HyperLink.ToolTip=text</code> | ✅ 一致 |
| <code>HyperLink.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HyperLink.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HyperLink.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HyperLink.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HyperLink.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HyperLink.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HyperLink.Visible=false</code> | ✅ 一致 |
| <code>HyperLink.Visible=true</code> | ✅ 一致 |
| <code>HyperLink.Width=120px</code> | ✅ 一致 |
| <code>HyperLink.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>HyperLink.Width=2em</code> | ✅ 一致 |
| <code>HyperLink.Width=50%</code> | ✅ 一致 |
| <code>HyperLink[naming container].(default)</code> | ✅ 一致 |
| <code>HyperLink[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HyperLink[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HyperLink[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HyperLink[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HyperLink[naming container].ID=text</code> | ✅ 一致 |
| <code>HyperLink[naming container].NavigateUrl=app-relative</code> | ✅ 一致 |
| <code>HyperLink[themed].(default)</code> | ✅ 一致 |
| <code>HyperLink[themed].CssClass=text</code> | ✅ 一致 |
| <code>HyperLink[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>HyperLink[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HyperLink [render][databind][dynamic] first request | ✅ 一致 |
| HyperLink [viewstate][dynamic] posted (set in code: kept; without view state: lost) | ✅ 一致 |
| HyperLink [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| HyperLink [render] the link followed (a GET, no postback) | ✅ 一致 |

</details>
