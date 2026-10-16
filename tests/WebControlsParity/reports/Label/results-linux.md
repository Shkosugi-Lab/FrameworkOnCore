# Label パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-16 17:49
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 154 | 154 | 0 | 0 |
| ② HTML | 132 | 132 | 0 | 0 |
| ③ シナリオ | 3 | 3 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 163 件中 163 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 31 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 3 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 119 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 2 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 7 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 4 | 0 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>Label.Api.Control.#ctor</code> | ✅ 一致 |
| <code>Label.Api.Control.Adapter</code> | ✅ 一致 |
| <code>Label.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>Label.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>Label.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>Label.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>Label.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>Label.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>Label.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>Label.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>Label.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>Label.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>Label.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>Label.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>Label.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>Label.Api.Control.ClientID</code> | ✅ 一致 |
| <code>Label.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>Label.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>Label.Api.Control.Context</code> | ✅ 一致 |
| <code>Label.Api.Control.Controls</code> | ✅ 一致 |
| <code>Label.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>Label.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>Label.Api.Control.DataBind</code> | ✅ 一致 |
| <code>Label.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>Label.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>Label.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>Label.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>Label.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>Label.Api.Control.Dispose</code> | ✅ 一致 |
| <code>Label.Api.Control.Disposed</code> | ✅ 一致 |
| <code>Label.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>Label.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>Label.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>Label.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>Label.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>Label.Api.Control.Events</code> | ✅ 一致 |
| <code>Label.Api.Control.FindControl</code> | ✅ 一致 |
| <code>Label.Api.Control.Focus</code> | ✅ 一致 |
| <code>Label.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>Label.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>Label.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>Label.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>Label.Api.Control.HasControls</code> | ✅ 一致 |
| <code>Label.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>Label.Api.Control.ID</code> | ✅ 一致 |
| <code>Label.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>Label.Api.Control.Init</code> | ✅ 一致 |
| <code>Label.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>Label.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>Label.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>Label.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>Label.Api.Control.Load</code> | ✅ 一致 |
| <code>Label.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>Label.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>Label.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>Label.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>Label.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>Label.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>Label.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>Label.Api.Control.OnInit</code> | ✅ 一致 |
| <code>Label.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>Label.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>Label.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>Label.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>Label.Api.Control.Page</code> | ✅ 一致 |
| <code>Label.Api.Control.Parent</code> | ✅ 一致 |
| <code>Label.Api.Control.PreRender</code> | ✅ 一致 |
| <code>Label.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>Label.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>Label.Api.Control.Render</code> | ✅ 一致 |
| <code>Label.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>Label.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>Label.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>Label.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>Label.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>Label.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>Label.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>Label.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>Label.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>Label.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>Label.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>Label.Api.Control.Site</code> | ✅ 一致 |
| <code>Label.Api.Control.SkinID</code> | ✅ 一致 |
| <code>Label.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>Label.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>Label.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>Label.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>Label.Api.Control.Unload</code> | ✅ 一致 |
| <code>Label.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>Label.Api.Control.ViewState</code> | ✅ 一致 |
| <code>Label.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>Label.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>Label.Api.Control.Visible</code> | ✅ 一致 |
| <code>Label.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>Label.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>Label.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>Label.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>Label.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>Label.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>Label.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>Label.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>Label.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>Label.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>Label.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>Label.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>Label.Api.ITextControl.Text</code> | ✅ 一致 |
| <code>Label.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>Label.Api.WebControls.Label.#ctor</code> | ✅ 一致 |
| <code>Label.Api.WebControls.Label.AddAttributesToRender</code> | ✅ 一致 |
| <code>Label.Api.WebControls.Label.AddParsedSubObject</code> | ✅ 一致 |
| <code>Label.Api.WebControls.Label.AssociatedControlID</code> | ✅ 一致 |
| <code>Label.Api.WebControls.Label.LoadViewState</code> | ✅ 一致 |
| <code>Label.Api.WebControls.Label.RenderContents</code> | ✅ 一致 |
| <code>Label.Api.WebControls.Label.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>Label.Api.WebControls.Label.TagKey</code> | ✅ 一致 |
| <code>Label.Api.WebControls.Label.Text</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>Label.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>Label.(case list)</code> | ✅ 一致 |
| <code>Label.(default)</code> | ✅ 一致 |
| <code>Label.AccessKey=empty</code> | ✅ 一致 |
| <code>Label.AccessKey=japanese</code> | ✅ 一致 |
| <code>Label.AccessKey=k</code> | ✅ 一致 |
| <code>Label.AccessKey=markup</code> | ✅ 一致 |
| <code>Label.AccessKey=text</code> | ✅ 一致 |
| <code>Label.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>Label.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>Label.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>Label.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>Label.AssociatedControlID=empty</code> | ✅ 一致 |
| <code>Label.AssociatedControlID=japanese</code> | ✅ 一致 |
| <code>Label.AssociatedControlID=markup</code> | ✅ 一致 |
| <code>Label.AssociatedControlID=target</code> | ✅ 一致 |
| <code>Label.AssociatedControlID=target+AccessKey=k</code> | ✅ 一致 |
| <code>Label.AssociatedControlID=target+Enabled=false</code> | ✅ 一致 |
| <code>Label.AssociatedControlID=target+Text=markup</code> | ✅ 一致 |
| <code>Label.AssociatedControlID=text</code> | ✅ 一致 |
| <code>Label.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>Label.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>Label.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>Label.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>Label.BackColor=#3366CC</code> | ✅ 一致 |
| <code>Label.BackColor=Red</code> | ✅ 一致 |
| <code>Label.BackColor=Transparent</code> | ✅ 一致 |
| <code>Label.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>Label.BorderColor=Red</code> | ✅ 一致 |
| <code>Label.BorderColor=Transparent</code> | ✅ 一致 |
| <code>Label.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>Label.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>Label.BorderStyle=Double</code> | ✅ 一致 |
| <code>Label.BorderStyle=Groove</code> | ✅ 一致 |
| <code>Label.BorderStyle=Inset</code> | ✅ 一致 |
| <code>Label.BorderStyle=None</code> | ✅ 一致 |
| <code>Label.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>Label.BorderStyle=Outset</code> | ✅ 一致 |
| <code>Label.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>Label.BorderStyle=Solid</code> | ✅ 一致 |
| <code>Label.BorderWidth=120px</code> | ✅ 一致 |
| <code>Label.BorderWidth=2em</code> | ✅ 一致 |
| <code>Label.BorderWidth=50%</code> | ✅ 一致 |
| <code>Label.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Label.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Label.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Label.ClientIDMode=Static</code> | ✅ 一致 |
| <code>Label.CssClass=empty</code> | ✅ 一致 |
| <code>Label.CssClass=japanese</code> | ✅ 一致 |
| <code>Label.CssClass=markup</code> | ✅ 一致 |
| <code>Label.CssClass=text</code> | ✅ 一致 |
| <code>Label.EnableTheming=false</code> | ✅ 一致 |
| <code>Label.EnableTheming=true</code> | ✅ 一致 |
| <code>Label.EnableViewState=false</code> | ✅ 一致 |
| <code>Label.EnableViewState=true</code> | ✅ 一致 |
| <code>Label.Enabled=false</code> | ✅ 一致 |
| <code>Label.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>Label.Enabled=true</code> | ✅ 一致 |
| <code>Label.Font.Bold=false</code> | ✅ 一致 |
| <code>Label.Font.Bold=true</code> | ✅ 一致 |
| <code>Label.Font.Italic=false</code> | ✅ 一致 |
| <code>Label.Font.Italic=true</code> | ✅ 一致 |
| <code>Label.Font.Name=empty</code> | ✅ 一致 |
| <code>Label.Font.Name=japanese</code> | ✅ 一致 |
| <code>Label.Font.Name=markup</code> | ✅ 一致 |
| <code>Label.Font.Name=text</code> | ✅ 一致 |
| <code>Label.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>Label.Font.Overline=false</code> | ✅ 一致 |
| <code>Label.Font.Overline=true</code> | ✅ 一致 |
| <code>Label.Font.Size=12pt</code> | ✅ 一致 |
| <code>Label.Font.Size=150%</code> | ✅ 一致 |
| <code>Label.Font.Size=Large</code> | ✅ 一致 |
| <code>Label.Font.Strikeout=false</code> | ✅ 一致 |
| <code>Label.Font.Strikeout=true</code> | ✅ 一致 |
| <code>Label.Font.Underline=false</code> | ✅ 一致 |
| <code>Label.Font.Underline=true</code> | ✅ 一致 |
| <code>Label.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>Label.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>Label.ForeColor=Red</code> | ✅ 一致 |
| <code>Label.ForeColor=Transparent</code> | ✅ 一致 |
| <code>Label.Height=120px</code> | ✅ 一致 |
| <code>Label.Height=2em</code> | ✅ 一致 |
| <code>Label.Height=50%</code> | ✅ 一致 |
| <code>Label.ID=empty</code> | ✅ 一致 |
| <code>Label.ID=japanese</code> | ✅ 一致 |
| <code>Label.ID=markup</code> | ✅ 一致 |
| <code>Label.ID=text</code> | ✅ 一致 |
| <code>Label.SkinID=empty</code> | ✅ 一致 |
| <code>Label.SkinID=japanese</code> | ✅ 一致 |
| <code>Label.SkinID=markup</code> | ✅ 一致 |
| <code>Label.SkinID=text</code> | ✅ 一致 |
| <code>Label.Style[color]=1em</code> | ✅ 一致 |
| <code>Label.Style[color]=red</code> | ✅ 一致 |
| <code>Label.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>Label.Style[margin-left]=red</code> | ✅ 一致 |
| <code>Label.TabIndex=-1</code> | ✅ 一致 |
| <code>Label.TabIndex=0</code> | ✅ 一致 |
| <code>Label.TabIndex=3</code> | ✅ 一致 |
| <code>Label.Text=empty</code> | ✅ 一致 |
| <code>Label.Text=empty+ToolTip=text</code> | ✅ 一致 |
| <code>Label.Text=japanese</code> | ✅ 一致 |
| <code>Label.Text=markup</code> | ✅ 一致 |
| <code>Label.Text=markup+Enabled=false</code> | ✅ 一致 |
| <code>Label.Text=text</code> | ✅ 一致 |
| <code>Label.Text=text+EnableViewState=false</code> | ✅ 一致 |
| <code>Label.Text=text+Visible=false</code> | ✅ 一致 |
| <code>Label.ToolTip=empty</code> | ✅ 一致 |
| <code>Label.ToolTip=japanese</code> | ✅ 一致 |
| <code>Label.ToolTip=markup</code> | ✅ 一致 |
| <code>Label.ToolTip=text</code> | ✅ 一致 |
| <code>Label.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>Label.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>Label.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>Label.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>Label.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>Label.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>Label.Visible=false</code> | ✅ 一致 |
| <code>Label.Visible=true</code> | ✅ 一致 |
| <code>Label.Width=120px</code> | ✅ 一致 |
| <code>Label.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>Label.Width=2em</code> | ✅ 一致 |
| <code>Label.Width=50%</code> | ✅ 一致 |
| <code>Label[naming container].(default)</code> | ✅ 一致 |
| <code>Label[naming container].AssociatedControlID=target</code> | ✅ 一致 |
| <code>Label[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Label[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Label[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Label[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>Label[naming container].ID=text</code> | ✅ 一致 |
| <code>Label[themed].(default)</code> | ✅ 一致 |
| <code>Label[themed].CssClass=text</code> | ✅ 一致 |
| <code>Label[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>Label[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| Label [render][databind][dynamic] first request | ✅ 一致 |
| Label [viewstate][dynamic] posted (set in code: kept; without view state: lost) | ✅ 一致 |
| Label [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
