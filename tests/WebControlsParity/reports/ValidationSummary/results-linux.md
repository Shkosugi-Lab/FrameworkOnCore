# ValidationSummary パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 03:22
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 159 | 159 | 0 | 0 |
| ② HTML | 149 | 149 | 0 | 0 |
| ③ シナリオ | 6 | 6 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 168 件中 168 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 37 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 6 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 135 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 0 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 8 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 2 | 0 | ✅ |
| ブラウザ側の検証スクリプト（EnableClientScript） | 5 | 2 | ✅ |
| 検証結果のまとめ（ValidationSummary） | 0 | 4 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>ValidationSummary.Api.Control.#ctor</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.Adapter</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.ClientID</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.Context</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.Controls</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.DataBind</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.Dispose</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.Disposed</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.Events</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.FindControl</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.Focus</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.HasControls</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.ID</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.Init</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.Load</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.OnInit</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.Page</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.Parent</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.PreRender</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.Render</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.Site</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.SkinID</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.Unload</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.ViewState</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>ValidationSummary.Api.Control.Visible</code> | ✅ 一致 |
| <code>ValidationSummary.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>ValidationSummary.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>ValidationSummary.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>ValidationSummary.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>ValidationSummary.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>ValidationSummary.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>ValidationSummary.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>ValidationSummary.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>ValidationSummary.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>ValidationSummary.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>ValidationSummary.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>ValidationSummary.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>ValidationSummary.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.ValidationSummary.#ctor</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.ValidationSummary.AddAttributesToRender</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.ValidationSummary.DisplayMode</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.ValidationSummary.EnableClientScript</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.ValidationSummary.ForeColor</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.ValidationSummary.HeaderText</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.ValidationSummary.OnInit</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.ValidationSummary.OnPreRender</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.ValidationSummary.Render</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.ValidationSummary.ShowMessageBox</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.ValidationSummary.ShowModelStateErrors</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.ValidationSummary.ShowSummary</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.ValidationSummary.ShowValidationErrors</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.ValidationSummary.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.ValidationSummary.ValidationGroup</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>ValidationSummary.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>ValidationSummary.(case list)</code> | ✅ 一致 |
| <code>ValidationSummary.(default)</code> | ✅ 一致 |
| <code>ValidationSummary.AccessKey=empty</code> | ✅ 一致 |
| <code>ValidationSummary.AccessKey=japanese</code> | ✅ 一致 |
| <code>ValidationSummary.AccessKey=k</code> | ✅ 一致 |
| <code>ValidationSummary.AccessKey=markup</code> | ✅ 一致 |
| <code>ValidationSummary.AccessKey=text</code> | ✅ 一致 |
| <code>ValidationSummary.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>ValidationSummary.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>ValidationSummary.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>ValidationSummary.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>ValidationSummary.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>ValidationSummary.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>ValidationSummary.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>ValidationSummary.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>ValidationSummary.BackColor=#3366CC</code> | ✅ 一致 |
| <code>ValidationSummary.BackColor=Red</code> | ✅ 一致 |
| <code>ValidationSummary.BackColor=Transparent</code> | ✅ 一致 |
| <code>ValidationSummary.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>ValidationSummary.BorderColor=Red</code> | ✅ 一致 |
| <code>ValidationSummary.BorderColor=Transparent</code> | ✅ 一致 |
| <code>ValidationSummary.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>ValidationSummary.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>ValidationSummary.BorderStyle=Double</code> | ✅ 一致 |
| <code>ValidationSummary.BorderStyle=Groove</code> | ✅ 一致 |
| <code>ValidationSummary.BorderStyle=Inset</code> | ✅ 一致 |
| <code>ValidationSummary.BorderStyle=None</code> | ✅ 一致 |
| <code>ValidationSummary.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>ValidationSummary.BorderStyle=Outset</code> | ✅ 一致 |
| <code>ValidationSummary.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>ValidationSummary.BorderStyle=Solid</code> | ✅ 一致 |
| <code>ValidationSummary.BorderWidth=120px</code> | ✅ 一致 |
| <code>ValidationSummary.BorderWidth=2em</code> | ✅ 一致 |
| <code>ValidationSummary.BorderWidth=50%</code> | ✅ 一致 |
| <code>ValidationSummary.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ValidationSummary.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ValidationSummary.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ValidationSummary.ClientIDMode=Static</code> | ✅ 一致 |
| <code>ValidationSummary.CssClass=empty</code> | ✅ 一致 |
| <code>ValidationSummary.CssClass=japanese</code> | ✅ 一致 |
| <code>ValidationSummary.CssClass=markup</code> | ✅ 一致 |
| <code>ValidationSummary.CssClass=text</code> | ✅ 一致 |
| <code>ValidationSummary.DisplayMode=BulletList</code> | ✅ 一致 |
| <code>ValidationSummary.DisplayMode=BulletList+HeaderText=text</code> | ✅ 一致 |
| <code>ValidationSummary.DisplayMode=List</code> | ✅ 一致 |
| <code>ValidationSummary.DisplayMode=List+HeaderText=markup</code> | ✅ 一致 |
| <code>ValidationSummary.DisplayMode=SingleParagraph</code> | ✅ 一致 |
| <code>ValidationSummary.DisplayMode=SingleParagraph+HeaderText=text</code> | ✅ 一致 |
| <code>ValidationSummary.EnableClientScript=false</code> | ✅ 一致 |
| <code>ValidationSummary.EnableClientScript=false+HeaderText=text</code> | ✅ 一致 |
| <code>ValidationSummary.EnableClientScript=true</code> | ✅ 一致 |
| <code>ValidationSummary.EnableTheming=false</code> | ✅ 一致 |
| <code>ValidationSummary.EnableTheming=true</code> | ✅ 一致 |
| <code>ValidationSummary.EnableViewState=false</code> | ✅ 一致 |
| <code>ValidationSummary.EnableViewState=true</code> | ✅ 一致 |
| <code>ValidationSummary.Enabled=false</code> | ✅ 一致 |
| <code>ValidationSummary.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>ValidationSummary.Enabled=true</code> | ✅ 一致 |
| <code>ValidationSummary.Font.Bold=false</code> | ✅ 一致 |
| <code>ValidationSummary.Font.Bold=true</code> | ✅ 一致 |
| <code>ValidationSummary.Font.Italic=false</code> | ✅ 一致 |
| <code>ValidationSummary.Font.Italic=true</code> | ✅ 一致 |
| <code>ValidationSummary.Font.Name=empty</code> | ✅ 一致 |
| <code>ValidationSummary.Font.Name=japanese</code> | ✅ 一致 |
| <code>ValidationSummary.Font.Name=markup</code> | ✅ 一致 |
| <code>ValidationSummary.Font.Name=text</code> | ✅ 一致 |
| <code>ValidationSummary.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>ValidationSummary.Font.Overline=false</code> | ✅ 一致 |
| <code>ValidationSummary.Font.Overline=true</code> | ✅ 一致 |
| <code>ValidationSummary.Font.Size=12pt</code> | ✅ 一致 |
| <code>ValidationSummary.Font.Size=150%</code> | ✅ 一致 |
| <code>ValidationSummary.Font.Size=Large</code> | ✅ 一致 |
| <code>ValidationSummary.Font.Strikeout=false</code> | ✅ 一致 |
| <code>ValidationSummary.Font.Strikeout=true</code> | ✅ 一致 |
| <code>ValidationSummary.Font.Underline=false</code> | ✅ 一致 |
| <code>ValidationSummary.Font.Underline=true</code> | ✅ 一致 |
| <code>ValidationSummary.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>ValidationSummary.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>ValidationSummary.ForeColor=Red</code> | ✅ 一致 |
| <code>ValidationSummary.ForeColor=Transparent</code> | ✅ 一致 |
| <code>ValidationSummary.HeaderText=empty</code> | ✅ 一致 |
| <code>ValidationSummary.HeaderText=japanese</code> | ✅ 一致 |
| <code>ValidationSummary.HeaderText=markup</code> | ✅ 一致 |
| <code>ValidationSummary.HeaderText=text</code> | ✅ 一致 |
| <code>ValidationSummary.HeaderText=text+EnableViewState=false</code> | ✅ 一致 |
| <code>ValidationSummary.HeaderText=text+Visible=false</code> | ✅ 一致 |
| <code>ValidationSummary.Height=120px</code> | ✅ 一致 |
| <code>ValidationSummary.Height=2em</code> | ✅ 一致 |
| <code>ValidationSummary.Height=50%</code> | ✅ 一致 |
| <code>ValidationSummary.ID=empty</code> | ✅ 一致 |
| <code>ValidationSummary.ID=japanese</code> | ✅ 一致 |
| <code>ValidationSummary.ID=markup</code> | ✅ 一致 |
| <code>ValidationSummary.ID=text</code> | ✅ 一致 |
| <code>ValidationSummary.ShowMessageBox=false</code> | ✅ 一致 |
| <code>ValidationSummary.ShowMessageBox=true</code> | ✅ 一致 |
| <code>ValidationSummary.ShowMessageBox=true+EnableClientScript=false</code> | ✅ 一致 |
| <code>ValidationSummary.ShowMessageBox=true+ShowSummary=false</code> | ✅ 一致 |
| <code>ValidationSummary.ShowModelStateErrors=false</code> | ✅ 一致 |
| <code>ValidationSummary.ShowModelStateErrors=true</code> | ✅ 一致 |
| <code>ValidationSummary.ShowSummary=false</code> | ✅ 一致 |
| <code>ValidationSummary.ShowSummary=true</code> | ✅ 一致 |
| <code>ValidationSummary.ShowValidationErrors=false</code> | ✅ 一致 |
| <code>ValidationSummary.ShowValidationErrors=false+HeaderText=text</code> | ✅ 一致 |
| <code>ValidationSummary.ShowValidationErrors=true</code> | ✅ 一致 |
| <code>ValidationSummary.SkinID=empty</code> | ✅ 一致 |
| <code>ValidationSummary.SkinID=japanese</code> | ✅ 一致 |
| <code>ValidationSummary.SkinID=markup</code> | ✅ 一致 |
| <code>ValidationSummary.SkinID=text</code> | ✅ 一致 |
| <code>ValidationSummary.Style[color]=1em</code> | ✅ 一致 |
| <code>ValidationSummary.Style[color]=red</code> | ✅ 一致 |
| <code>ValidationSummary.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>ValidationSummary.Style[margin-left]=red</code> | ✅ 一致 |
| <code>ValidationSummary.TabIndex=-1</code> | ✅ 一致 |
| <code>ValidationSummary.TabIndex=0</code> | ✅ 一致 |
| <code>ValidationSummary.TabIndex=3</code> | ✅ 一致 |
| <code>ValidationSummary.ToolTip=empty</code> | ✅ 一致 |
| <code>ValidationSummary.ToolTip=japanese</code> | ✅ 一致 |
| <code>ValidationSummary.ToolTip=markup</code> | ✅ 一致 |
| <code>ValidationSummary.ToolTip=text</code> | ✅ 一致 |
| <code>ValidationSummary.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>ValidationSummary.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>ValidationSummary.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>ValidationSummary.ValidationGroup=empty</code> | ✅ 一致 |
| <code>ValidationSummary.ValidationGroup=g2</code> | ✅ 一致 |
| <code>ValidationSummary.ValidationGroup=g2+HeaderText=text</code> | ✅ 一致 |
| <code>ValidationSummary.ValidationGroup=japanese</code> | ✅ 一致 |
| <code>ValidationSummary.ValidationGroup=markup</code> | ✅ 一致 |
| <code>ValidationSummary.ValidationGroup=text</code> | ✅ 一致 |
| <code>ValidationSummary.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>ValidationSummary.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>ValidationSummary.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>ValidationSummary.Visible=false</code> | ✅ 一致 |
| <code>ValidationSummary.Visible=true</code> | ✅ 一致 |
| <code>ValidationSummary.Width=120px</code> | ✅ 一致 |
| <code>ValidationSummary.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>ValidationSummary.Width=2em</code> | ✅ 一致 |
| <code>ValidationSummary.Width=50%</code> | ✅ 一致 |
| <code>ValidationSummary[naming container].(default)</code> | ✅ 一致 |
| <code>ValidationSummary[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ValidationSummary[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ValidationSummary[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ValidationSummary[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>ValidationSummary[naming container].EnableClientScript=false</code> | ✅ 一致 |
| <code>ValidationSummary[naming container].ID=text</code> | ✅ 一致 |
| <code>ValidationSummary[naming container].ValidationGroup=g2</code> | ✅ 一致 |
| <code>ValidationSummary[themed].(default)</code> | ✅ 一致 |
| <code>ValidationSummary[themed].CssClass=text</code> | ✅ 一致 |
| <code>ValidationSummary[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>ValidationSummary[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| ValidationSummary [render][databind][dynamic] first request | ✅ 一致 |
| ValidationSummary [summary][dynamic] the server&#39;s errors collected (bullets, single paragraph) | ✅ 一致 |
| ValidationSummary [summary] Page.Validate(group) in code | ✅ 一致 |
| ValidationSummary [summary][clientvalidate] the browser&#39;s errors collected (one shown only there): no request | ✅ 一致 |
| ValidationSummary [summary][clientvalidate] all filled: posted | ✅ 一致 |
| ValidationSummary [visible][servervalidate] the hidden validator shown, then validating | ✅ 一致 |

</details>
