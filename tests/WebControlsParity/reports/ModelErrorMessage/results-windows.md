# ModelErrorMessage パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 00:29
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 160 | 160 | 0 | 0 |
| ② HTML | 137 | 137 | 0 | 0 |
| ③ シナリオ | 15 | 15 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 169 件中 169 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 33 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 15 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 118 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 14 | ✅ |
| Visible=false（描画されず状態は保持） | 3 | 1 | ✅ |
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
| <code>ModelErrorMessage.Api.Control.#ctor</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.Adapter</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.ClientID</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.Context</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.Controls</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.DataBind</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.Dispose</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.Disposed</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.Events</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.FindControl</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.Focus</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.HasControls</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.ID</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.Init</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.Load</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.OnInit</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.Page</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.Parent</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.PreRender</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.Render</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.Site</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.SkinID</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.Unload</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.ViewState</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.Control.Visible</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.ITextControl.Text</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.Label.#ctor</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.Label.AddAttributesToRender</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.Label.AddParsedSubObject</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.Label.AssociatedControlID</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.Label.LoadViewState</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.Label.RenderContents</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.Label.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.Label.TagKey</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.Label.Text</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.ModelErrorMessage.#ctor</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.ModelErrorMessage.AssociatedControlID</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.ModelErrorMessage.ModelStateKey</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.ModelErrorMessage.OnPreRender</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.ModelErrorMessage.SetFocusOnError</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.ModelErrorMessage.Text</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>ModelErrorMessage.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>ModelErrorMessage.(case list)</code> | ✅ 一致 |
| <code>ModelErrorMessage.(default)</code> | ✅ 一致 |
| <code>ModelErrorMessage.AccessKey=empty</code> | ✅ 一致 |
| <code>ModelErrorMessage.AccessKey=japanese</code> | ✅ 一致 |
| <code>ModelErrorMessage.AccessKey=markup</code> | ✅ 一致 |
| <code>ModelErrorMessage.AccessKey=text</code> | ✅ 一致 |
| <code>ModelErrorMessage.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>ModelErrorMessage.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>ModelErrorMessage.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>ModelErrorMessage.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>ModelErrorMessage.AssociatedControlID=empty</code> | ✅ 一致 |
| <code>ModelErrorMessage.AssociatedControlID=japanese</code> | ✅ 一致 |
| <code>ModelErrorMessage.AssociatedControlID=markup</code> | ✅ 一致 |
| <code>ModelErrorMessage.AssociatedControlID=target</code> | ✅ 一致 |
| <code>ModelErrorMessage.AssociatedControlID=text</code> | ✅ 一致 |
| <code>ModelErrorMessage.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>ModelErrorMessage.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>ModelErrorMessage.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>ModelErrorMessage.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>ModelErrorMessage.BackColor=#3366CC</code> | ✅ 一致 |
| <code>ModelErrorMessage.BackColor=Red</code> | ✅ 一致 |
| <code>ModelErrorMessage.BackColor=Transparent</code> | ✅ 一致 |
| <code>ModelErrorMessage.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>ModelErrorMessage.BorderColor=Red</code> | ✅ 一致 |
| <code>ModelErrorMessage.BorderColor=Transparent</code> | ✅ 一致 |
| <code>ModelErrorMessage.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>ModelErrorMessage.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>ModelErrorMessage.BorderStyle=Double</code> | ✅ 一致 |
| <code>ModelErrorMessage.BorderStyle=Groove</code> | ✅ 一致 |
| <code>ModelErrorMessage.BorderStyle=Inset</code> | ✅ 一致 |
| <code>ModelErrorMessage.BorderStyle=None</code> | ✅ 一致 |
| <code>ModelErrorMessage.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>ModelErrorMessage.BorderStyle=Outset</code> | ✅ 一致 |
| <code>ModelErrorMessage.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>ModelErrorMessage.BorderStyle=Solid</code> | ✅ 一致 |
| <code>ModelErrorMessage.BorderWidth=120px</code> | ✅ 一致 |
| <code>ModelErrorMessage.BorderWidth=2em</code> | ✅ 一致 |
| <code>ModelErrorMessage.BorderWidth=50%</code> | ✅ 一致 |
| <code>ModelErrorMessage.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ModelErrorMessage.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ModelErrorMessage.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ModelErrorMessage.ClientIDMode=Static</code> | ✅ 一致 |
| <code>ModelErrorMessage.CssClass=empty</code> | ✅ 一致 |
| <code>ModelErrorMessage.CssClass=japanese</code> | ✅ 一致 |
| <code>ModelErrorMessage.CssClass=markup</code> | ✅ 一致 |
| <code>ModelErrorMessage.CssClass=text</code> | ✅ 一致 |
| <code>ModelErrorMessage.EnableTheming=false</code> | ✅ 一致 |
| <code>ModelErrorMessage.EnableTheming=true</code> | ✅ 一致 |
| <code>ModelErrorMessage.EnableViewState=false</code> | ✅ 一致 |
| <code>ModelErrorMessage.EnableViewState=true</code> | ✅ 一致 |
| <code>ModelErrorMessage.Enabled=false</code> | ✅ 一致 |
| <code>ModelErrorMessage.Enabled=true</code> | ✅ 一致 |
| <code>ModelErrorMessage.Font.Bold=false</code> | ✅ 一致 |
| <code>ModelErrorMessage.Font.Bold=true</code> | ✅ 一致 |
| <code>ModelErrorMessage.Font.Italic=false</code> | ✅ 一致 |
| <code>ModelErrorMessage.Font.Italic=true</code> | ✅ 一致 |
| <code>ModelErrorMessage.Font.Name=empty</code> | ✅ 一致 |
| <code>ModelErrorMessage.Font.Name=japanese</code> | ✅ 一致 |
| <code>ModelErrorMessage.Font.Name=markup</code> | ✅ 一致 |
| <code>ModelErrorMessage.Font.Name=text</code> | ✅ 一致 |
| <code>ModelErrorMessage.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>ModelErrorMessage.Font.Overline=false</code> | ✅ 一致 |
| <code>ModelErrorMessage.Font.Overline=true</code> | ✅ 一致 |
| <code>ModelErrorMessage.Font.Size=12pt</code> | ✅ 一致 |
| <code>ModelErrorMessage.Font.Size=150%</code> | ✅ 一致 |
| <code>ModelErrorMessage.Font.Size=Large</code> | ✅ 一致 |
| <code>ModelErrorMessage.Font.Strikeout=false</code> | ✅ 一致 |
| <code>ModelErrorMessage.Font.Strikeout=true</code> | ✅ 一致 |
| <code>ModelErrorMessage.Font.Underline=false</code> | ✅ 一致 |
| <code>ModelErrorMessage.Font.Underline=true</code> | ✅ 一致 |
| <code>ModelErrorMessage.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>ModelErrorMessage.ForeColor=Red</code> | ✅ 一致 |
| <code>ModelErrorMessage.ForeColor=Transparent</code> | ✅ 一致 |
| <code>ModelErrorMessage.Height=120px</code> | ✅ 一致 |
| <code>ModelErrorMessage.Height=2em</code> | ✅ 一致 |
| <code>ModelErrorMessage.Height=50%</code> | ✅ 一致 |
| <code>ModelErrorMessage.ID=empty</code> | ✅ 一致 |
| <code>ModelErrorMessage.ID=japanese</code> | ✅ 一致 |
| <code>ModelErrorMessage.ID=markup</code> | ✅ 一致 |
| <code>ModelErrorMessage.ID=text</code> | ✅ 一致 |
| <code>ModelErrorMessage.ModelStateKey=Name</code> | ✅ 一致 |
| <code>ModelErrorMessage.ModelStateKey=Name+CssClass=text</code> | ✅ 一致 |
| <code>ModelErrorMessage.ModelStateKey=empty</code> | ✅ 一致 |
| <code>ModelErrorMessage.ModelStateKey=japanese</code> | ✅ 一致 |
| <code>ModelErrorMessage.ModelStateKey=markup</code> | ✅ 一致 |
| <code>ModelErrorMessage.ModelStateKey=text</code> | ✅ 一致 |
| <code>ModelErrorMessage.SetFocusOnError=false</code> | ✅ 一致 |
| <code>ModelErrorMessage.SetFocusOnError=true</code> | ✅ 一致 |
| <code>ModelErrorMessage.SetFocusOnError=true+AssociatedControlID=target</code> | ✅ 一致 |
| <code>ModelErrorMessage.SkinID=empty</code> | ✅ 一致 |
| <code>ModelErrorMessage.SkinID=japanese</code> | ✅ 一致 |
| <code>ModelErrorMessage.SkinID=markup</code> | ✅ 一致 |
| <code>ModelErrorMessage.SkinID=text</code> | ✅ 一致 |
| <code>ModelErrorMessage.Style[color]=1em</code> | ✅ 一致 |
| <code>ModelErrorMessage.Style[color]=red</code> | ✅ 一致 |
| <code>ModelErrorMessage.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>ModelErrorMessage.Style[margin-left]=red</code> | ✅ 一致 |
| <code>ModelErrorMessage.TabIndex=-1</code> | ✅ 一致 |
| <code>ModelErrorMessage.TabIndex=0</code> | ✅ 一致 |
| <code>ModelErrorMessage.TabIndex=3</code> | ✅ 一致 |
| <code>ModelErrorMessage.Text=empty</code> | ✅ 一致 |
| <code>ModelErrorMessage.Text=japanese</code> | ✅ 一致 |
| <code>ModelErrorMessage.Text=markup</code> | ✅ 一致 |
| <code>ModelErrorMessage.Text=text</code> | ✅ 一致 |
| <code>ModelErrorMessage.ToolTip=empty</code> | ✅ 一致 |
| <code>ModelErrorMessage.ToolTip=japanese</code> | ✅ 一致 |
| <code>ModelErrorMessage.ToolTip=markup</code> | ✅ 一致 |
| <code>ModelErrorMessage.ToolTip=text</code> | ✅ 一致 |
| <code>ModelErrorMessage.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>ModelErrorMessage.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>ModelErrorMessage.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>ModelErrorMessage.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>ModelErrorMessage.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>ModelErrorMessage.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>ModelErrorMessage.Visible=false</code> | ✅ 一致 |
| <code>ModelErrorMessage.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>ModelErrorMessage.Visible=true</code> | ✅ 一致 |
| <code>ModelErrorMessage.Width=120px</code> | ✅ 一致 |
| <code>ModelErrorMessage.Width=2em</code> | ✅ 一致 |
| <code>ModelErrorMessage.Width=50%</code> | ✅ 一致 |
| <code>ModelErrorMessage[error].(default)</code> | ✅ 一致 |
| <code>ModelErrorMessage[error].Enabled=false</code> | ✅ 一致 |
| <code>ModelErrorMessage[error].ModelStateKey=Name+CssClass=text</code> | ✅ 一致 |
| <code>ModelErrorMessage[error].ModelStateKey=text</code> | ✅ 一致 |
| <code>ModelErrorMessage[error].SetFocusOnError=true+AssociatedControlID=target</code> | ✅ 一致 |
| <code>ModelErrorMessage[error].Visible=false</code> | ✅ 一致 |
| <code>ModelErrorMessage[error].Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>ModelErrorMessage[naming container].(default)</code> | ✅ 一致 |
| <code>ModelErrorMessage[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ModelErrorMessage[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ModelErrorMessage[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ModelErrorMessage[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>ModelErrorMessage[naming container].ID=text</code> | ✅ 一致 |
| <code>ModelErrorMessage[themed].(default)</code> | ✅ 一致 |
| <code>ModelErrorMessage[themed].CssClass=text</code> | ✅ 一致 |
| <code>ModelErrorMessage[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>ModelErrorMessage[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| ModelErrorMessage [render][databind][dynamic] first request: grids bound to each data source, the Access data source selected in code, ones made in code | ✅ 一致 |
| ModelErrorMessage [viewstate] an object data source&#39;s row updated | ✅ 一致 |
| ModelErrorMessage [viewstate] an object data source&#39;s row deleted | ✅ 一致 |
| ModelErrorMessage [viewstate] a SQL data source sorted | ✅ 一致 |
| ModelErrorMessage [viewstate] a SQL data source&#39;s row updated (its update command&#39;s parameters) | ✅ 一致 |
| ModelErrorMessage [viewstate] a SQL data source&#39;s row deleted | ✅ 一致 |
| ModelErrorMessage [viewstate] a SQL data source&#39;s row inserted | ✅ 一致 |
| ModelErrorMessage [viewstate] a LINQ data source paged | ✅ 一致 |
| ModelErrorMessage [viewstate] a LINQ data source sorted | ✅ 一致 |
| ModelErrorMessage [viewstate] a LINQ data source&#39;s where parameter from a text box | ✅ 一致 |
| ModelErrorMessage [viewstate] a query extender&#39;s search from a text box | ✅ 一致 |
| ModelErrorMessage [viewstate] a model error shown | ✅ 一致 |
| ModelErrorMessage [viewstate] no model error | ✅ 一致 |
| ModelErrorMessage [dynamic][viewstate] a grid sorted by a data source made in code | ✅ 一致 |
| ModelErrorMessage [visible][viewstate] a hidden grid bound to a data source shown | ✅ 一致 |

</details>
