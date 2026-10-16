# Panel パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 00:33
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 157 | 157 | 0 | 0 |
| ② HTML | 152 | 152 | 0 | 0 |
| ③ シナリオ | 4 | 4 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 166 件中 166 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 36 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 4 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 139 | 1 | ✅ |
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
| <code>Panel.Api.Control.#ctor</code> | ✅ 一致 |
| <code>Panel.Api.Control.Adapter</code> | ✅ 一致 |
| <code>Panel.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>Panel.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>Panel.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>Panel.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>Panel.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>Panel.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>Panel.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>Panel.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>Panel.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>Panel.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>Panel.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>Panel.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>Panel.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>Panel.Api.Control.ClientID</code> | ✅ 一致 |
| <code>Panel.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>Panel.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>Panel.Api.Control.Context</code> | ✅ 一致 |
| <code>Panel.Api.Control.Controls</code> | ✅ 一致 |
| <code>Panel.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>Panel.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>Panel.Api.Control.DataBind</code> | ✅ 一致 |
| <code>Panel.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>Panel.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>Panel.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>Panel.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>Panel.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>Panel.Api.Control.Dispose</code> | ✅ 一致 |
| <code>Panel.Api.Control.Disposed</code> | ✅ 一致 |
| <code>Panel.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>Panel.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>Panel.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>Panel.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>Panel.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>Panel.Api.Control.Events</code> | ✅ 一致 |
| <code>Panel.Api.Control.FindControl</code> | ✅ 一致 |
| <code>Panel.Api.Control.Focus</code> | ✅ 一致 |
| <code>Panel.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>Panel.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>Panel.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>Panel.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>Panel.Api.Control.HasControls</code> | ✅ 一致 |
| <code>Panel.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>Panel.Api.Control.ID</code> | ✅ 一致 |
| <code>Panel.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>Panel.Api.Control.Init</code> | ✅ 一致 |
| <code>Panel.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>Panel.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>Panel.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>Panel.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>Panel.Api.Control.Load</code> | ✅ 一致 |
| <code>Panel.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>Panel.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>Panel.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>Panel.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>Panel.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>Panel.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>Panel.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>Panel.Api.Control.OnInit</code> | ✅ 一致 |
| <code>Panel.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>Panel.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>Panel.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>Panel.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>Panel.Api.Control.Page</code> | ✅ 一致 |
| <code>Panel.Api.Control.Parent</code> | ✅ 一致 |
| <code>Panel.Api.Control.PreRender</code> | ✅ 一致 |
| <code>Panel.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>Panel.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>Panel.Api.Control.Render</code> | ✅ 一致 |
| <code>Panel.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>Panel.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>Panel.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>Panel.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>Panel.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>Panel.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>Panel.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>Panel.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>Panel.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>Panel.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>Panel.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>Panel.Api.Control.Site</code> | ✅ 一致 |
| <code>Panel.Api.Control.SkinID</code> | ✅ 一致 |
| <code>Panel.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>Panel.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>Panel.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>Panel.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>Panel.Api.Control.Unload</code> | ✅ 一致 |
| <code>Panel.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>Panel.Api.Control.ViewState</code> | ✅ 一致 |
| <code>Panel.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>Panel.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>Panel.Api.Control.Visible</code> | ✅ 一致 |
| <code>Panel.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>Panel.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>Panel.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>Panel.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>Panel.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>Panel.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>Panel.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>Panel.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>Panel.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>Panel.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>Panel.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>Panel.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>Panel.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.Panel.#ctor</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.Panel.AddAttributesToRender</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.Panel.BackImageUrl</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.Panel.CreateControlStyle</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.Panel.DefaultButton</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.Panel.Direction</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.Panel.GroupingText</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.Panel.HorizontalAlign</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.Panel.RenderBeginTag</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.Panel.RenderEndTag</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.Panel.ScrollBars</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.Panel.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.Panel.Wrap</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>Panel.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>Panel.(case list)</code> | ✅ 一致 |
| <code>Panel.(default)</code> | ✅ 一致 |
| <code>Panel.AccessKey=empty</code> | ✅ 一致 |
| <code>Panel.AccessKey=japanese</code> | ✅ 一致 |
| <code>Panel.AccessKey=markup</code> | ✅ 一致 |
| <code>Panel.AccessKey=text</code> | ✅ 一致 |
| <code>Panel.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>Panel.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>Panel.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>Panel.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>Panel.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>Panel.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>Panel.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>Panel.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>Panel.BackColor=#3366CC</code> | ✅ 一致 |
| <code>Panel.BackColor=Red</code> | ✅ 一致 |
| <code>Panel.BackColor=Transparent</code> | ✅ 一致 |
| <code>Panel.BackImageUrl=app-relative</code> | ✅ 一致 |
| <code>Panel.BackImageUrl=app-relative+Width=120px</code> | ✅ 一致 |
| <code>Panel.BackImageUrl=empty</code> | ✅ 一致 |
| <code>Panel.BackImageUrl=japanese</code> | ✅ 一致 |
| <code>Panel.BackImageUrl=markup</code> | ✅ 一致 |
| <code>Panel.BackImageUrl=text</code> | ✅ 一致 |
| <code>Panel.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>Panel.BorderColor=Red</code> | ✅ 一致 |
| <code>Panel.BorderColor=Transparent</code> | ✅ 一致 |
| <code>Panel.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>Panel.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>Panel.BorderStyle=Double</code> | ✅ 一致 |
| <code>Panel.BorderStyle=Groove</code> | ✅ 一致 |
| <code>Panel.BorderStyle=Inset</code> | ✅ 一致 |
| <code>Panel.BorderStyle=None</code> | ✅ 一致 |
| <code>Panel.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>Panel.BorderStyle=Outset</code> | ✅ 一致 |
| <code>Panel.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>Panel.BorderStyle=Solid</code> | ✅ 一致 |
| <code>Panel.BorderWidth=120px</code> | ✅ 一致 |
| <code>Panel.BorderWidth=2em</code> | ✅ 一致 |
| <code>Panel.BorderWidth=50%</code> | ✅ 一致 |
| <code>Panel.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Panel.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Panel.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Panel.ClientIDMode=Static</code> | ✅ 一致 |
| <code>Panel.CssClass=empty</code> | ✅ 一致 |
| <code>Panel.CssClass=japanese</code> | ✅ 一致 |
| <code>Panel.CssClass=markup</code> | ✅ 一致 |
| <code>Panel.CssClass=text</code> | ✅ 一致 |
| <code>Panel.DefaultButton=empty</code> | ✅ 一致 |
| <code>Panel.DefaultButton=inside</code> | ✅ 一致 |
| <code>Panel.DefaultButton=inside+Enabled=false</code> | ✅ 一致 |
| <code>Panel.DefaultButton=japanese</code> | ✅ 一致 |
| <code>Panel.DefaultButton=markup</code> | ✅ 一致 |
| <code>Panel.DefaultButton=text</code> | ✅ 一致 |
| <code>Panel.Direction=LeftToRight</code> | ✅ 一致 |
| <code>Panel.Direction=NotSet</code> | ✅ 一致 |
| <code>Panel.Direction=RightToLeft</code> | ✅ 一致 |
| <code>Panel.EnableTheming=false</code> | ✅ 一致 |
| <code>Panel.EnableTheming=true</code> | ✅ 一致 |
| <code>Panel.EnableViewState=false</code> | ✅ 一致 |
| <code>Panel.EnableViewState=true</code> | ✅ 一致 |
| <code>Panel.Enabled=false</code> | ✅ 一致 |
| <code>Panel.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>Panel.Enabled=true</code> | ✅ 一致 |
| <code>Panel.Font.Bold=false</code> | ✅ 一致 |
| <code>Panel.Font.Bold=true</code> | ✅ 一致 |
| <code>Panel.Font.Italic=false</code> | ✅ 一致 |
| <code>Panel.Font.Italic=true</code> | ✅ 一致 |
| <code>Panel.Font.Name=empty</code> | ✅ 一致 |
| <code>Panel.Font.Name=japanese</code> | ✅ 一致 |
| <code>Panel.Font.Name=markup</code> | ✅ 一致 |
| <code>Panel.Font.Name=text</code> | ✅ 一致 |
| <code>Panel.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>Panel.Font.Overline=false</code> | ✅ 一致 |
| <code>Panel.Font.Overline=true</code> | ✅ 一致 |
| <code>Panel.Font.Size=12pt</code> | ✅ 一致 |
| <code>Panel.Font.Size=150%</code> | ✅ 一致 |
| <code>Panel.Font.Size=Large</code> | ✅ 一致 |
| <code>Panel.Font.Strikeout=false</code> | ✅ 一致 |
| <code>Panel.Font.Strikeout=true</code> | ✅ 一致 |
| <code>Panel.Font.Underline=false</code> | ✅ 一致 |
| <code>Panel.Font.Underline=true</code> | ✅ 一致 |
| <code>Panel.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>Panel.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>Panel.ForeColor=Red</code> | ✅ 一致 |
| <code>Panel.ForeColor=Transparent</code> | ✅ 一致 |
| <code>Panel.GroupingText=empty</code> | ✅ 一致 |
| <code>Panel.GroupingText=japanese</code> | ✅ 一致 |
| <code>Panel.GroupingText=markup</code> | ✅ 一致 |
| <code>Panel.GroupingText=text</code> | ✅ 一致 |
| <code>Panel.GroupingText=text+CssClass=text</code> | ✅ 一致 |
| <code>Panel.GroupingText=text+Direction=RightToLeft</code> | ✅ 一致 |
| <code>Panel.GroupingText=text+EnableViewState=false</code> | ✅ 一致 |
| <code>Panel.GroupingText=text+Visible=false</code> | ✅ 一致 |
| <code>Panel.Height=120px</code> | ✅ 一致 |
| <code>Panel.Height=2em</code> | ✅ 一致 |
| <code>Panel.Height=50%</code> | ✅ 一致 |
| <code>Panel.HorizontalAlign=Center</code> | ✅ 一致 |
| <code>Panel.HorizontalAlign=Center+Wrap=false</code> | ✅ 一致 |
| <code>Panel.HorizontalAlign=Justify</code> | ✅ 一致 |
| <code>Panel.HorizontalAlign=Left</code> | ✅ 一致 |
| <code>Panel.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>Panel.HorizontalAlign=Right</code> | ✅ 一致 |
| <code>Panel.ID=empty</code> | ✅ 一致 |
| <code>Panel.ID=japanese</code> | ✅ 一致 |
| <code>Panel.ID=markup</code> | ✅ 一致 |
| <code>Panel.ID=text</code> | ✅ 一致 |
| <code>Panel.ScrollBars=Auto</code> | ✅ 一致 |
| <code>Panel.ScrollBars=Auto+Height=2em</code> | ✅ 一致 |
| <code>Panel.ScrollBars=Both</code> | ✅ 一致 |
| <code>Panel.ScrollBars=Horizontal</code> | ✅ 一致 |
| <code>Panel.ScrollBars=None</code> | ✅ 一致 |
| <code>Panel.ScrollBars=Vertical</code> | ✅ 一致 |
| <code>Panel.SkinID=empty</code> | ✅ 一致 |
| <code>Panel.SkinID=japanese</code> | ✅ 一致 |
| <code>Panel.SkinID=markup</code> | ✅ 一致 |
| <code>Panel.SkinID=text</code> | ✅ 一致 |
| <code>Panel.Style[color]=1em</code> | ✅ 一致 |
| <code>Panel.Style[color]=red</code> | ✅ 一致 |
| <code>Panel.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>Panel.Style[margin-left]=red</code> | ✅ 一致 |
| <code>Panel.TabIndex=-1</code> | ✅ 一致 |
| <code>Panel.TabIndex=0</code> | ✅ 一致 |
| <code>Panel.TabIndex=3</code> | ✅ 一致 |
| <code>Panel.ToolTip=empty</code> | ✅ 一致 |
| <code>Panel.ToolTip=japanese</code> | ✅ 一致 |
| <code>Panel.ToolTip=markup</code> | ✅ 一致 |
| <code>Panel.ToolTip=text</code> | ✅ 一致 |
| <code>Panel.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>Panel.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>Panel.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>Panel.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>Panel.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>Panel.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>Panel.Visible=false</code> | ✅ 一致 |
| <code>Panel.Visible=true</code> | ✅ 一致 |
| <code>Panel.Width=120px</code> | ✅ 一致 |
| <code>Panel.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>Panel.Width=2em</code> | ✅ 一致 |
| <code>Panel.Width=50%</code> | ✅ 一致 |
| <code>Panel.Wrap=false</code> | ✅ 一致 |
| <code>Panel.Wrap=true</code> | ✅ 一致 |
| <code>Panel[naming container].(default)</code> | ✅ 一致 |
| <code>Panel[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Panel[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Panel[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Panel[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>Panel[naming container].DefaultButton=inside</code> | ✅ 一致 |
| <code>Panel[naming container].ID=text</code> | ✅ 一致 |
| <code>Panel[themed].(default)</code> | ✅ 一致 |
| <code>Panel[themed].CssClass=text</code> | ✅ 一致 |
| <code>Panel[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>Panel[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| Panel [render][databind][dynamic] first request | ✅ 一致 |
| Panel [dynamic][viewstate] a panel&#39;s text box posts its value | ✅ 一致 |
| Panel [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| Panel [defaultbutton] Enter in a panel whose DefaultButton is a link | ✅ 一致 |

</details>
