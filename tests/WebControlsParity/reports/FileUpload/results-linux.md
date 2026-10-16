# FileUpload パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-16 18:09
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 157 | 157 | 0 | 0 |
| ② HTML | 122 | 122 | 0 | 0 |
| ③ シナリオ | 14 | 14 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 166 件中 166 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 30 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 14 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 109 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 13 | ❌ 1 件 |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 7 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 3 | ✅ |
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
| <code>FileUpload.Api.Control.#ctor</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.Adapter</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.ClientID</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.Context</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.Controls</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.DataBind</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.Dispose</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.Disposed</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.Events</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.FindControl</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.Focus</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.HasControls</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.ID</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.Init</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.Load</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.OnInit</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.Page</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.Parent</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.PreRender</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.Render</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.Site</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.SkinID</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.Unload</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.ViewState</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>FileUpload.Api.Control.Visible</code> | ✅ 一致 |
| <code>FileUpload.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>FileUpload.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>FileUpload.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>FileUpload.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>FileUpload.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>FileUpload.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>FileUpload.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>FileUpload.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>FileUpload.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>FileUpload.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>FileUpload.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>FileUpload.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>FileUpload.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.FileUpload.#ctor</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.FileUpload.AddAttributesToRender</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.FileUpload.AllowMultiple</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.FileUpload.FileBytes</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.FileUpload.FileContent</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.FileUpload.FileName</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.FileUpload.HasFile</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.FileUpload.HasFiles</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.FileUpload.OnPreRender</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.FileUpload.PostedFile</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.FileUpload.PostedFiles</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.FileUpload.Render</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.FileUpload.SaveAs</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>FileUpload.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>FileUpload.(case list)</code> | ✅ 一致 |
| <code>FileUpload.(default)</code> | ✅ 一致 |
| <code>FileUpload.AccessKey=empty</code> | ✅ 一致 |
| <code>FileUpload.AccessKey=japanese</code> | ✅ 一致 |
| <code>FileUpload.AccessKey=k</code> | ✅ 一致 |
| <code>FileUpload.AccessKey=markup</code> | ✅ 一致 |
| <code>FileUpload.AccessKey=text</code> | ✅ 一致 |
| <code>FileUpload.AllowMultiple=false</code> | ✅ 一致 |
| <code>FileUpload.AllowMultiple=true</code> | ✅ 一致 |
| <code>FileUpload.AllowMultiple=true+EnableViewState=false</code> | ✅ 一致 |
| <code>FileUpload.AllowMultiple=true+Enabled=false</code> | ✅ 一致 |
| <code>FileUpload.AllowMultiple=true+ToolTip=text</code> | ✅ 一致 |
| <code>FileUpload.AllowMultiple=true+Visible=false</code> | ✅ 一致 |
| <code>FileUpload.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>FileUpload.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>FileUpload.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>FileUpload.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>FileUpload.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>FileUpload.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>FileUpload.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>FileUpload.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>FileUpload.BackColor=#3366CC</code> | ✅ 一致 |
| <code>FileUpload.BackColor=Red</code> | ✅ 一致 |
| <code>FileUpload.BackColor=Transparent</code> | ✅ 一致 |
| <code>FileUpload.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>FileUpload.BorderColor=Red</code> | ✅ 一致 |
| <code>FileUpload.BorderColor=Transparent</code> | ✅ 一致 |
| <code>FileUpload.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>FileUpload.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>FileUpload.BorderStyle=Double</code> | ✅ 一致 |
| <code>FileUpload.BorderStyle=Groove</code> | ✅ 一致 |
| <code>FileUpload.BorderStyle=Inset</code> | ✅ 一致 |
| <code>FileUpload.BorderStyle=None</code> | ✅ 一致 |
| <code>FileUpload.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>FileUpload.BorderStyle=Outset</code> | ✅ 一致 |
| <code>FileUpload.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>FileUpload.BorderStyle=Solid</code> | ✅ 一致 |
| <code>FileUpload.BorderWidth=120px</code> | ✅ 一致 |
| <code>FileUpload.BorderWidth=2em</code> | ✅ 一致 |
| <code>FileUpload.BorderWidth=50%</code> | ✅ 一致 |
| <code>FileUpload.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>FileUpload.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>FileUpload.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>FileUpload.ClientIDMode=Static</code> | ✅ 一致 |
| <code>FileUpload.CssClass=empty</code> | ✅ 一致 |
| <code>FileUpload.CssClass=japanese</code> | ✅ 一致 |
| <code>FileUpload.CssClass=markup</code> | ✅ 一致 |
| <code>FileUpload.CssClass=text</code> | ✅ 一致 |
| <code>FileUpload.EnableTheming=false</code> | ✅ 一致 |
| <code>FileUpload.EnableTheming=true</code> | ✅ 一致 |
| <code>FileUpload.EnableViewState=false</code> | ✅ 一致 |
| <code>FileUpload.EnableViewState=true</code> | ✅ 一致 |
| <code>FileUpload.Enabled=false</code> | ✅ 一致 |
| <code>FileUpload.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>FileUpload.Enabled=true</code> | ✅ 一致 |
| <code>FileUpload.Font.Bold=false</code> | ✅ 一致 |
| <code>FileUpload.Font.Bold=true</code> | ✅ 一致 |
| <code>FileUpload.Font.Italic=false</code> | ✅ 一致 |
| <code>FileUpload.Font.Italic=true</code> | ✅ 一致 |
| <code>FileUpload.Font.Name=empty</code> | ✅ 一致 |
| <code>FileUpload.Font.Name=japanese</code> | ✅ 一致 |
| <code>FileUpload.Font.Name=markup</code> | ✅ 一致 |
| <code>FileUpload.Font.Name=text</code> | ✅ 一致 |
| <code>FileUpload.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>FileUpload.Font.Overline=false</code> | ✅ 一致 |
| <code>FileUpload.Font.Overline=true</code> | ✅ 一致 |
| <code>FileUpload.Font.Size=12pt</code> | ✅ 一致 |
| <code>FileUpload.Font.Size=150%</code> | ✅ 一致 |
| <code>FileUpload.Font.Size=Large</code> | ✅ 一致 |
| <code>FileUpload.Font.Strikeout=false</code> | ✅ 一致 |
| <code>FileUpload.Font.Strikeout=true</code> | ✅ 一致 |
| <code>FileUpload.Font.Underline=false</code> | ✅ 一致 |
| <code>FileUpload.Font.Underline=true</code> | ✅ 一致 |
| <code>FileUpload.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>FileUpload.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>FileUpload.ForeColor=Red</code> | ✅ 一致 |
| <code>FileUpload.ForeColor=Transparent</code> | ✅ 一致 |
| <code>FileUpload.Height=120px</code> | ✅ 一致 |
| <code>FileUpload.Height=2em</code> | ✅ 一致 |
| <code>FileUpload.Height=50%</code> | ✅ 一致 |
| <code>FileUpload.ID=empty</code> | ✅ 一致 |
| <code>FileUpload.ID=japanese</code> | ✅ 一致 |
| <code>FileUpload.ID=markup</code> | ✅ 一致 |
| <code>FileUpload.ID=text</code> | ✅ 一致 |
| <code>FileUpload.SkinID=empty</code> | ✅ 一致 |
| <code>FileUpload.SkinID=japanese</code> | ✅ 一致 |
| <code>FileUpload.SkinID=markup</code> | ✅ 一致 |
| <code>FileUpload.SkinID=text</code> | ✅ 一致 |
| <code>FileUpload.Style[color]=1em</code> | ✅ 一致 |
| <code>FileUpload.Style[color]=red</code> | ✅ 一致 |
| <code>FileUpload.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>FileUpload.Style[margin-left]=red</code> | ✅ 一致 |
| <code>FileUpload.TabIndex=-1</code> | ✅ 一致 |
| <code>FileUpload.TabIndex=0</code> | ✅ 一致 |
| <code>FileUpload.TabIndex=3</code> | ✅ 一致 |
| <code>FileUpload.ToolTip=empty</code> | ✅ 一致 |
| <code>FileUpload.ToolTip=japanese</code> | ✅ 一致 |
| <code>FileUpload.ToolTip=markup</code> | ✅ 一致 |
| <code>FileUpload.ToolTip=text</code> | ✅ 一致 |
| <code>FileUpload.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>FileUpload.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>FileUpload.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>FileUpload.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>FileUpload.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>FileUpload.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>FileUpload.Visible=false</code> | ✅ 一致 |
| <code>FileUpload.Visible=true</code> | ✅ 一致 |
| <code>FileUpload.Width=120px</code> | ✅ 一致 |
| <code>FileUpload.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>FileUpload.Width=2em</code> | ✅ 一致 |
| <code>FileUpload.Width=50%</code> | ✅ 一致 |
| <code>FileUpload[naming container].(default)</code> | ✅ 一致 |
| <code>FileUpload[naming container].AllowMultiple=true</code> | ✅ 一致 |
| <code>FileUpload[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>FileUpload[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>FileUpload[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>FileUpload[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>FileUpload[naming container].ID=text</code> | ✅ 一致 |
| <code>FileUpload[themed].(default)</code> | ✅ 一致 |
| <code>FileUpload[themed].CssClass=text</code> | ✅ 一致 |
| <code>FileUpload[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>FileUpload[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| FileUpload [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| FileUpload [viewstate][dynamic] posted: files, view state, what code set (a table&#39;s rows and a literal control&#39;s text are not kept) | ✅ 一致 |
| FileUpload [viewstate] the next view (NextView bubbled from a button) | ✅ 一致 |
| FileUpload [viewstate] switched by ID and by index: the first view&#39;s text box kept its value | ✅ 一致 |
| FileUpload [dynamic][viewstate] a dynamic multi-view&#39;s next view | ✅ 一致 |
| FileUpload [viewstate] a calendar&#39;s day chosen | ✅ 一致 |
| FileUpload [viewstate] a calendar&#39;s week chosen | ❌ 未実行 |
| FileUpload [viewstate] a calendar&#39;s next month | ✅ 一致 |
| FileUpload [viewstate] a dynamic calendar&#39;s day chosen | ✅ 一致 |
| FileUpload [viewstate] an image map&#39;s hot spot clicked (PostBack) | ✅ 一致 |
| FileUpload [viewstate] a dynamic image map&#39;s hot spot clicked | ✅ 一致 |
| FileUpload [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| FileUpload [viewstate] a file upload validated on the server (g1): no file posted | ✅ 一致 |
| FileUpload [viewstate] a postback event to a table (not registered for event validation): rejected | ✅ 一致 |

</details>
