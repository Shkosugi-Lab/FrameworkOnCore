# LoginName パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 00:24
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 151 | 151 | 0 | 0 |
| ② HTML | 127 | 127 | 0 | 0 |
| ③ シナリオ | 15 | 15 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 160 件中 160 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 30 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 15 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 109 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 14 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
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
| <code>LoginName.Api.Control.#ctor</code> | ✅ 一致 |
| <code>LoginName.Api.Control.Adapter</code> | ✅ 一致 |
| <code>LoginName.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>LoginName.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>LoginName.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>LoginName.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>LoginName.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>LoginName.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>LoginName.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>LoginName.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>LoginName.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>LoginName.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>LoginName.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>LoginName.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>LoginName.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>LoginName.Api.Control.ClientID</code> | ✅ 一致 |
| <code>LoginName.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>LoginName.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>LoginName.Api.Control.Context</code> | ✅ 一致 |
| <code>LoginName.Api.Control.Controls</code> | ✅ 一致 |
| <code>LoginName.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>LoginName.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>LoginName.Api.Control.DataBind</code> | ✅ 一致 |
| <code>LoginName.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>LoginName.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>LoginName.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>LoginName.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>LoginName.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>LoginName.Api.Control.Dispose</code> | ✅ 一致 |
| <code>LoginName.Api.Control.Disposed</code> | ✅ 一致 |
| <code>LoginName.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>LoginName.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>LoginName.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>LoginName.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>LoginName.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>LoginName.Api.Control.Events</code> | ✅ 一致 |
| <code>LoginName.Api.Control.FindControl</code> | ✅ 一致 |
| <code>LoginName.Api.Control.Focus</code> | ✅ 一致 |
| <code>LoginName.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>LoginName.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>LoginName.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>LoginName.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>LoginName.Api.Control.HasControls</code> | ✅ 一致 |
| <code>LoginName.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>LoginName.Api.Control.ID</code> | ✅ 一致 |
| <code>LoginName.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>LoginName.Api.Control.Init</code> | ✅ 一致 |
| <code>LoginName.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>LoginName.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>LoginName.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>LoginName.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>LoginName.Api.Control.Load</code> | ✅ 一致 |
| <code>LoginName.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>LoginName.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>LoginName.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>LoginName.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>LoginName.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>LoginName.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>LoginName.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>LoginName.Api.Control.OnInit</code> | ✅ 一致 |
| <code>LoginName.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>LoginName.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>LoginName.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>LoginName.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>LoginName.Api.Control.Page</code> | ✅ 一致 |
| <code>LoginName.Api.Control.Parent</code> | ✅ 一致 |
| <code>LoginName.Api.Control.PreRender</code> | ✅ 一致 |
| <code>LoginName.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>LoginName.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>LoginName.Api.Control.Render</code> | ✅ 一致 |
| <code>LoginName.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>LoginName.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>LoginName.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>LoginName.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>LoginName.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>LoginName.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>LoginName.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>LoginName.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>LoginName.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>LoginName.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>LoginName.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>LoginName.Api.Control.Site</code> | ✅ 一致 |
| <code>LoginName.Api.Control.SkinID</code> | ✅ 一致 |
| <code>LoginName.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>LoginName.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>LoginName.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>LoginName.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>LoginName.Api.Control.Unload</code> | ✅ 一致 |
| <code>LoginName.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>LoginName.Api.Control.ViewState</code> | ✅ 一致 |
| <code>LoginName.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>LoginName.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>LoginName.Api.Control.Visible</code> | ✅ 一致 |
| <code>LoginName.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>LoginName.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>LoginName.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>LoginName.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>LoginName.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>LoginName.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>LoginName.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>LoginName.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>LoginName.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>LoginName.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>LoginName.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>LoginName.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>LoginName.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.LoginName.#ctor</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.LoginName.FormatString</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.LoginName.Render</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.LoginName.RenderBeginTag</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.LoginName.RenderContents</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.LoginName.RenderEndTag</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.LoginName.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>LoginName.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>LoginName.(case list)</code> | ✅ 一致 |
| <code>LoginName.(default)</code> | ✅ 一致 |
| <code>LoginName.AccessKey=empty</code> | ✅ 一致 |
| <code>LoginName.AccessKey=japanese</code> | ✅ 一致 |
| <code>LoginName.AccessKey=markup</code> | ✅ 一致 |
| <code>LoginName.AccessKey=text</code> | ✅ 一致 |
| <code>LoginName.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>LoginName.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>LoginName.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>LoginName.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>LoginName.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>LoginName.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>LoginName.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>LoginName.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>LoginName.BackColor=#3366CC</code> | ✅ 一致 |
| <code>LoginName.BackColor=Red</code> | ✅ 一致 |
| <code>LoginName.BackColor=Transparent</code> | ✅ 一致 |
| <code>LoginName.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>LoginName.BorderColor=Red</code> | ✅ 一致 |
| <code>LoginName.BorderColor=Transparent</code> | ✅ 一致 |
| <code>LoginName.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>LoginName.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>LoginName.BorderStyle=Double</code> | ✅ 一致 |
| <code>LoginName.BorderStyle=Groove</code> | ✅ 一致 |
| <code>LoginName.BorderStyle=Inset</code> | ✅ 一致 |
| <code>LoginName.BorderStyle=None</code> | ✅ 一致 |
| <code>LoginName.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>LoginName.BorderStyle=Outset</code> | ✅ 一致 |
| <code>LoginName.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>LoginName.BorderStyle=Solid</code> | ✅ 一致 |
| <code>LoginName.BorderWidth=120px</code> | ✅ 一致 |
| <code>LoginName.BorderWidth=2em</code> | ✅ 一致 |
| <code>LoginName.BorderWidth=50%</code> | ✅ 一致 |
| <code>LoginName.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>LoginName.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>LoginName.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>LoginName.ClientIDMode=Static</code> | ✅ 一致 |
| <code>LoginName.CssClass=empty</code> | ✅ 一致 |
| <code>LoginName.CssClass=japanese</code> | ✅ 一致 |
| <code>LoginName.CssClass=markup</code> | ✅ 一致 |
| <code>LoginName.CssClass=text</code> | ✅ 一致 |
| <code>LoginName.EnableTheming=false</code> | ✅ 一致 |
| <code>LoginName.EnableTheming=true</code> | ✅ 一致 |
| <code>LoginName.EnableViewState=false</code> | ✅ 一致 |
| <code>LoginName.EnableViewState=true</code> | ✅ 一致 |
| <code>LoginName.Enabled=false</code> | ✅ 一致 |
| <code>LoginName.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>LoginName.Enabled=true</code> | ✅ 一致 |
| <code>LoginName.Font.Bold=false</code> | ✅ 一致 |
| <code>LoginName.Font.Bold=true</code> | ✅ 一致 |
| <code>LoginName.Font.Italic=false</code> | ✅ 一致 |
| <code>LoginName.Font.Italic=true</code> | ✅ 一致 |
| <code>LoginName.Font.Name=empty</code> | ✅ 一致 |
| <code>LoginName.Font.Name=japanese</code> | ✅ 一致 |
| <code>LoginName.Font.Name=markup</code> | ✅ 一致 |
| <code>LoginName.Font.Name=text</code> | ✅ 一致 |
| <code>LoginName.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>LoginName.Font.Overline=false</code> | ✅ 一致 |
| <code>LoginName.Font.Overline=true</code> | ✅ 一致 |
| <code>LoginName.Font.Size=12pt</code> | ✅ 一致 |
| <code>LoginName.Font.Size=150%</code> | ✅ 一致 |
| <code>LoginName.Font.Size=Large</code> | ✅ 一致 |
| <code>LoginName.Font.Strikeout=false</code> | ✅ 一致 |
| <code>LoginName.Font.Strikeout=true</code> | ✅ 一致 |
| <code>LoginName.Font.Underline=false</code> | ✅ 一致 |
| <code>LoginName.Font.Underline=true</code> | ✅ 一致 |
| <code>LoginName.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>LoginName.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>LoginName.ForeColor=Red</code> | ✅ 一致 |
| <code>LoginName.ForeColor=Transparent</code> | ✅ 一致 |
| <code>LoginName.FormatString=empty</code> | ✅ 一致 |
| <code>LoginName.FormatString=invalid</code> | ✅ 一致 |
| <code>LoginName.FormatString=japanese</code> | ✅ 一致 |
| <code>LoginName.FormatString=markup</code> | ✅ 一致 |
| <code>LoginName.FormatString=text</code> | ✅ 一致 |
| <code>LoginName.FormatString=welcome</code> | ✅ 一致 |
| <code>LoginName.FormatString=welcome+Visible=false</code> | ✅ 一致 |
| <code>LoginName.Height=120px</code> | ✅ 一致 |
| <code>LoginName.Height=2em</code> | ✅ 一致 |
| <code>LoginName.Height=50%</code> | ✅ 一致 |
| <code>LoginName.ID=empty</code> | ✅ 一致 |
| <code>LoginName.ID=japanese</code> | ✅ 一致 |
| <code>LoginName.ID=markup</code> | ✅ 一致 |
| <code>LoginName.ID=text</code> | ✅ 一致 |
| <code>LoginName.SkinID=empty</code> | ✅ 一致 |
| <code>LoginName.SkinID=japanese</code> | ✅ 一致 |
| <code>LoginName.SkinID=markup</code> | ✅ 一致 |
| <code>LoginName.SkinID=text</code> | ✅ 一致 |
| <code>LoginName.Style[color]=1em</code> | ✅ 一致 |
| <code>LoginName.Style[color]=red</code> | ✅ 一致 |
| <code>LoginName.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>LoginName.Style[margin-left]=red</code> | ✅ 一致 |
| <code>LoginName.TabIndex=-1</code> | ✅ 一致 |
| <code>LoginName.TabIndex=0</code> | ✅ 一致 |
| <code>LoginName.TabIndex=3</code> | ✅ 一致 |
| <code>LoginName.ToolTip=empty</code> | ✅ 一致 |
| <code>LoginName.ToolTip=japanese</code> | ✅ 一致 |
| <code>LoginName.ToolTip=markup</code> | ✅ 一致 |
| <code>LoginName.ToolTip=text</code> | ✅ 一致 |
| <code>LoginName.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>LoginName.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>LoginName.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>LoginName.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>LoginName.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>LoginName.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>LoginName.Visible=false</code> | ✅ 一致 |
| <code>LoginName.Visible=true</code> | ✅ 一致 |
| <code>LoginName.Width=120px</code> | ✅ 一致 |
| <code>LoginName.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>LoginName.Width=2em</code> | ✅ 一致 |
| <code>LoginName.Width=50%</code> | ✅ 一致 |
| <code>LoginName[logged in].(default)</code> | ✅ 一致 |
| <code>LoginName[logged in].FormatString=invalid</code> | ✅ 一致 |
| <code>LoginName[logged in].FormatString=markup</code> | ✅ 一致 |
| <code>LoginName[logged in].FormatString=welcome</code> | ✅ 一致 |
| <code>LoginName[logged in].FormatString=welcome+CssClass=text</code> | ✅ 一致 |
| <code>LoginName[logged in].FormatString=welcome+EnableViewState=false</code> | ✅ 一致 |
| <code>LoginName[naming container].(default)</code> | ✅ 一致 |
| <code>LoginName[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>LoginName[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>LoginName[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>LoginName[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>LoginName[naming container].ID=text</code> | ✅ 一致 |
| <code>LoginName[themed].(default)</code> | ✅ 一致 |
| <code>LoginName[themed].CssClass=text</code> | ✅ 一致 |
| <code>LoginName[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>LoginName[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| LoginName [render][databind][dynamic] first request, anonymous: the login forms, the status, the views, the wizards | ✅ 一致 |
| LoginName [viewstate] a login with a wrong password: its error | ✅ 一致 |
| LoginName [viewstate] logged in (redirected to the page): the name, the status, the role&#39;s view | ✅ 一致 |
| LoginName [viewstate] the password changed: the success template | ✅ 一致 |
| LoginName [viewstate] logged out by the status | ✅ 一致 |
| LoginName [viewstate] a recovery for a user who does not exist | ✅ 一致 |
| LoginName [viewstate] the user&#39;s question | ✅ 一致 |
| LoginName [viewstate] the answer right: the password reset, its mail prepared (cancelled), the success template | ✅ 一致 |
| LoginName [viewstate] a user created: the complete step | ✅ 一致 |
| LoginName [viewstate] the wizard&#39;s next step (the templated one) | ✅ 一致 |
| LoginName [viewstate] next, then previous | ✅ 一致 |
| LoginName [viewstate] a step chosen in the side bar | ✅ 一致 |
| LoginName [viewstate] finished: the complete step, the first step&#39;s value kept | ✅ 一致 |
| LoginName [dynamic][viewstate] a wizard made in code: its next step | ✅ 一致 |
| LoginName [visible][viewstate] a hidden wizard shown | ✅ 一致 |

</details>
