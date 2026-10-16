# Image パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-15 13:44
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 155 | 155 | 0 | 0 |
| ② HTML | 150 | 150 | 0 | 0 |
| ③ シナリオ | 3 | 3 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 164 件中 164 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 34 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 3 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 137 | 1 | ✅ |
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
| <code>Image.Api.Control.#ctor</code> | ✅ 一致 |
| <code>Image.Api.Control.Adapter</code> | ✅ 一致 |
| <code>Image.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>Image.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>Image.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>Image.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>Image.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>Image.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>Image.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>Image.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>Image.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>Image.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>Image.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>Image.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>Image.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>Image.Api.Control.ClientID</code> | ✅ 一致 |
| <code>Image.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>Image.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>Image.Api.Control.Context</code> | ✅ 一致 |
| <code>Image.Api.Control.Controls</code> | ✅ 一致 |
| <code>Image.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>Image.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>Image.Api.Control.DataBind</code> | ✅ 一致 |
| <code>Image.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>Image.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>Image.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>Image.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>Image.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>Image.Api.Control.Dispose</code> | ✅ 一致 |
| <code>Image.Api.Control.Disposed</code> | ✅ 一致 |
| <code>Image.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>Image.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>Image.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>Image.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>Image.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>Image.Api.Control.Events</code> | ✅ 一致 |
| <code>Image.Api.Control.FindControl</code> | ✅ 一致 |
| <code>Image.Api.Control.Focus</code> | ✅ 一致 |
| <code>Image.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>Image.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>Image.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>Image.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>Image.Api.Control.HasControls</code> | ✅ 一致 |
| <code>Image.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>Image.Api.Control.ID</code> | ✅ 一致 |
| <code>Image.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>Image.Api.Control.Init</code> | ✅ 一致 |
| <code>Image.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>Image.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>Image.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>Image.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>Image.Api.Control.Load</code> | ✅ 一致 |
| <code>Image.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>Image.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>Image.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>Image.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>Image.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>Image.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>Image.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>Image.Api.Control.OnInit</code> | ✅ 一致 |
| <code>Image.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>Image.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>Image.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>Image.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>Image.Api.Control.Page</code> | ✅ 一致 |
| <code>Image.Api.Control.Parent</code> | ✅ 一致 |
| <code>Image.Api.Control.PreRender</code> | ✅ 一致 |
| <code>Image.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>Image.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>Image.Api.Control.Render</code> | ✅ 一致 |
| <code>Image.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>Image.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>Image.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>Image.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>Image.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>Image.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>Image.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>Image.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>Image.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>Image.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>Image.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>Image.Api.Control.Site</code> | ✅ 一致 |
| <code>Image.Api.Control.SkinID</code> | ✅ 一致 |
| <code>Image.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>Image.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>Image.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>Image.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>Image.Api.Control.Unload</code> | ✅ 一致 |
| <code>Image.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>Image.Api.Control.ViewState</code> | ✅ 一致 |
| <code>Image.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>Image.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>Image.Api.Control.Visible</code> | ✅ 一致 |
| <code>Image.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>Image.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>Image.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>Image.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>Image.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>Image.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>Image.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>Image.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>Image.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>Image.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>Image.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>Image.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>Image.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>Image.Api.WebControls.Image.#ctor</code> | ✅ 一致 |
| <code>Image.Api.WebControls.Image.AddAttributesToRender</code> | ✅ 一致 |
| <code>Image.Api.WebControls.Image.AlternateText</code> | ✅ 一致 |
| <code>Image.Api.WebControls.Image.DescriptionUrl</code> | ✅ 一致 |
| <code>Image.Api.WebControls.Image.Enabled</code> | ✅ 一致 |
| <code>Image.Api.WebControls.Image.Font</code> | ✅ 一致 |
| <code>Image.Api.WebControls.Image.GenerateEmptyAlternateText</code> | ✅ 一致 |
| <code>Image.Api.WebControls.Image.ImageAlign</code> | ✅ 一致 |
| <code>Image.Api.WebControls.Image.ImageUrl</code> | ✅ 一致 |
| <code>Image.Api.WebControls.Image.RenderContents</code> | ✅ 一致 |
| <code>Image.Api.WebControls.Image.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>Image.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>Image.(case list)</code> | ✅ 一致 |
| <code>Image.(default)</code> | ✅ 一致 |
| <code>Image.AccessKey=empty</code> | ✅ 一致 |
| <code>Image.AccessKey=japanese</code> | ✅ 一致 |
| <code>Image.AccessKey=k</code> | ✅ 一致 |
| <code>Image.AccessKey=markup</code> | ✅ 一致 |
| <code>Image.AccessKey=text</code> | ✅ 一致 |
| <code>Image.AlternateText=empty</code> | ✅ 一致 |
| <code>Image.AlternateText=empty+GenerateEmptyAlternateText=true</code> | ✅ 一致 |
| <code>Image.AlternateText=japanese</code> | ✅ 一致 |
| <code>Image.AlternateText=markup</code> | ✅ 一致 |
| <code>Image.AlternateText=text</code> | ✅ 一致 |
| <code>Image.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>Image.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>Image.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>Image.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>Image.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>Image.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>Image.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>Image.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>Image.BackColor=#3366CC</code> | ✅ 一致 |
| <code>Image.BackColor=Red</code> | ✅ 一致 |
| <code>Image.BackColor=Transparent</code> | ✅ 一致 |
| <code>Image.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>Image.BorderColor=Red</code> | ✅ 一致 |
| <code>Image.BorderColor=Transparent</code> | ✅ 一致 |
| <code>Image.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>Image.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>Image.BorderStyle=Double</code> | ✅ 一致 |
| <code>Image.BorderStyle=Groove</code> | ✅ 一致 |
| <code>Image.BorderStyle=Inset</code> | ✅ 一致 |
| <code>Image.BorderStyle=None</code> | ✅ 一致 |
| <code>Image.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>Image.BorderStyle=Outset</code> | ✅ 一致 |
| <code>Image.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>Image.BorderStyle=Solid</code> | ✅ 一致 |
| <code>Image.BorderWidth=120px</code> | ✅ 一致 |
| <code>Image.BorderWidth=2em</code> | ✅ 一致 |
| <code>Image.BorderWidth=50%</code> | ✅ 一致 |
| <code>Image.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Image.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Image.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Image.ClientIDMode=Static</code> | ✅ 一致 |
| <code>Image.CssClass=empty</code> | ✅ 一致 |
| <code>Image.CssClass=japanese</code> | ✅ 一致 |
| <code>Image.CssClass=markup</code> | ✅ 一致 |
| <code>Image.CssClass=text</code> | ✅ 一致 |
| <code>Image.DescriptionUrl=app-relative</code> | ✅ 一致 |
| <code>Image.DescriptionUrl=empty</code> | ✅ 一致 |
| <code>Image.DescriptionUrl=japanese</code> | ✅ 一致 |
| <code>Image.DescriptionUrl=markup</code> | ✅ 一致 |
| <code>Image.DescriptionUrl=text</code> | ✅ 一致 |
| <code>Image.EnableTheming=false</code> | ✅ 一致 |
| <code>Image.EnableTheming=true</code> | ✅ 一致 |
| <code>Image.EnableViewState=false</code> | ✅ 一致 |
| <code>Image.EnableViewState=true</code> | ✅ 一致 |
| <code>Image.Enabled=false</code> | ✅ 一致 |
| <code>Image.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>Image.Enabled=true</code> | ✅ 一致 |
| <code>Image.Font.Bold=false</code> | ✅ 一致 |
| <code>Image.Font.Bold=true</code> | ✅ 一致 |
| <code>Image.Font.Italic=false</code> | ✅ 一致 |
| <code>Image.Font.Italic=true</code> | ✅ 一致 |
| <code>Image.Font.Name=empty</code> | ✅ 一致 |
| <code>Image.Font.Name=japanese</code> | ✅ 一致 |
| <code>Image.Font.Name=markup</code> | ✅ 一致 |
| <code>Image.Font.Name=text</code> | ✅ 一致 |
| <code>Image.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>Image.Font.Overline=false</code> | ✅ 一致 |
| <code>Image.Font.Overline=true</code> | ✅ 一致 |
| <code>Image.Font.Size=12pt</code> | ✅ 一致 |
| <code>Image.Font.Size=150%</code> | ✅ 一致 |
| <code>Image.Font.Size=Large</code> | ✅ 一致 |
| <code>Image.Font.Strikeout=false</code> | ✅ 一致 |
| <code>Image.Font.Strikeout=true</code> | ✅ 一致 |
| <code>Image.Font.Underline=false</code> | ✅ 一致 |
| <code>Image.Font.Underline=true</code> | ✅ 一致 |
| <code>Image.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>Image.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>Image.ForeColor=Red</code> | ✅ 一致 |
| <code>Image.ForeColor=Transparent</code> | ✅ 一致 |
| <code>Image.GenerateEmptyAlternateText=false</code> | ✅ 一致 |
| <code>Image.GenerateEmptyAlternateText=true</code> | ✅ 一致 |
| <code>Image.Height=120px</code> | ✅ 一致 |
| <code>Image.Height=2em</code> | ✅ 一致 |
| <code>Image.Height=50%</code> | ✅ 一致 |
| <code>Image.ID=empty</code> | ✅ 一致 |
| <code>Image.ID=japanese</code> | ✅ 一致 |
| <code>Image.ID=markup</code> | ✅ 一致 |
| <code>Image.ID=text</code> | ✅ 一致 |
| <code>Image.ImageAlign=AbsBottom</code> | ✅ 一致 |
| <code>Image.ImageAlign=AbsMiddle</code> | ✅ 一致 |
| <code>Image.ImageAlign=Baseline</code> | ✅ 一致 |
| <code>Image.ImageAlign=Bottom</code> | ✅ 一致 |
| <code>Image.ImageAlign=Left</code> | ✅ 一致 |
| <code>Image.ImageAlign=Middle</code> | ✅ 一致 |
| <code>Image.ImageAlign=NotSet</code> | ✅ 一致 |
| <code>Image.ImageAlign=Right</code> | ✅ 一致 |
| <code>Image.ImageAlign=TextTop</code> | ✅ 一致 |
| <code>Image.ImageAlign=Top</code> | ✅ 一致 |
| <code>Image.ImageUrl=absolute</code> | ✅ 一致 |
| <code>Image.ImageUrl=app-relative</code> | ✅ 一致 |
| <code>Image.ImageUrl=app-relative+AlternateText=text</code> | ✅ 一致 |
| <code>Image.ImageUrl=app-relative+EnableViewState=false</code> | ✅ 一致 |
| <code>Image.ImageUrl=app-relative+Enabled=false</code> | ✅ 一致 |
| <code>Image.ImageUrl=app-relative+GenerateEmptyAlternateText=true</code> | ✅ 一致 |
| <code>Image.ImageUrl=app-relative+ImageAlign=Left</code> | ✅ 一致 |
| <code>Image.ImageUrl=app-relative+Visible=false</code> | ✅ 一致 |
| <code>Image.ImageUrl=app-relative+Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>Image.ImageUrl=empty</code> | ✅ 一致 |
| <code>Image.ImageUrl=japanese</code> | ✅ 一致 |
| <code>Image.ImageUrl=markup</code> | ✅ 一致 |
| <code>Image.ImageUrl=text</code> | ✅ 一致 |
| <code>Image.SkinID=empty</code> | ✅ 一致 |
| <code>Image.SkinID=japanese</code> | ✅ 一致 |
| <code>Image.SkinID=markup</code> | ✅ 一致 |
| <code>Image.SkinID=text</code> | ✅ 一致 |
| <code>Image.Style[color]=1em</code> | ✅ 一致 |
| <code>Image.Style[color]=red</code> | ✅ 一致 |
| <code>Image.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>Image.Style[margin-left]=red</code> | ✅ 一致 |
| <code>Image.TabIndex=-1</code> | ✅ 一致 |
| <code>Image.TabIndex=0</code> | ✅ 一致 |
| <code>Image.TabIndex=3</code> | ✅ 一致 |
| <code>Image.ToolTip=empty</code> | ✅ 一致 |
| <code>Image.ToolTip=japanese</code> | ✅ 一致 |
| <code>Image.ToolTip=markup</code> | ✅ 一致 |
| <code>Image.ToolTip=text</code> | ✅ 一致 |
| <code>Image.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>Image.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>Image.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>Image.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>Image.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>Image.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>Image.Visible=false</code> | ✅ 一致 |
| <code>Image.Visible=true</code> | ✅ 一致 |
| <code>Image.Width=120px</code> | ✅ 一致 |
| <code>Image.Width=2em</code> | ✅ 一致 |
| <code>Image.Width=50%</code> | ✅ 一致 |
| <code>Image[naming container].(default)</code> | ✅ 一致 |
| <code>Image[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Image[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Image[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Image[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>Image[naming container].ID=text</code> | ✅ 一致 |
| <code>Image[naming container].ImageUrl=app-relative</code> | ✅ 一致 |
| <code>Image[themed].(default)</code> | ✅ 一致 |
| <code>Image[themed].CssClass=text</code> | ✅ 一致 |
| <code>Image[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>Image[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| Image [render][databind][dynamic] first request | ✅ 一致 |
| Image [viewstate][dynamic] posted (set in code: kept; without view state: lost) | ✅ 一致 |
| Image [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
