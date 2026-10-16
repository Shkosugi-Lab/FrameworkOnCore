# ImageMap パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-15 13:46
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 169 | 169 | 0 | 0 |
| ② HTML | 188 | 188 | 0 | 0 |
| ③ シナリオ | 14 | 14 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 178 件中 178 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 36 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 14 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 144 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 11 | ❌ 1 件 |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 7 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 4 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 3 | 0 | ✅ |
| ポストバックイベント | 0 | 2 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>ImageMap.Api.Control.#ctor</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.Adapter</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.ClientID</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.Context</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.Controls</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.DataBind</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.Dispose</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.Disposed</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.Events</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.FindControl</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.Focus</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.HasControls</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.ID</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.Init</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.Load</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.OnInit</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.Page</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.Parent</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.PreRender</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.Render</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.Site</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.SkinID</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.Unload</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.ViewState</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>ImageMap.Api.Control.Visible</code> | ✅ 一致 |
| <code>ImageMap.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>ImageMap.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>ImageMap.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>ImageMap.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>ImageMap.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>ImageMap.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>ImageMap.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>ImageMap.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>ImageMap.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>ImageMap.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>ImageMap.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>ImageMap.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>ImageMap.Api.IPostBackEventHandler.RaisePostBackEvent</code> | ✅ 一致 |
| <code>ImageMap.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.Image.#ctor</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.Image.AddAttributesToRender</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.Image.AlternateText</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.Image.DescriptionUrl</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.Image.Enabled</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.Image.Font</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.Image.GenerateEmptyAlternateText</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.Image.ImageAlign</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.Image.ImageUrl</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.Image.RenderContents</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.Image.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.ImageMap.#ctor</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.ImageMap.AddAttributesToRender</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.ImageMap.Click</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.ImageMap.Enabled</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.ImageMap.HotSpotMode</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.ImageMap.HotSpots</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.ImageMap.LoadViewState</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.ImageMap.OnClick</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.ImageMap.RaisePostBackEvent</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.ImageMap.Render</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.ImageMap.SaveViewState</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.ImageMap.Target</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.ImageMap.TrackViewState</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>ImageMap.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>ImageMap.(case list)</code> | ✅ 一致 |
| <code>ImageMap.(default)</code> | ✅ 一致 |
| <code>ImageMap.AccessKey=empty</code> | ✅ 一致 |
| <code>ImageMap.AccessKey=japanese</code> | ✅ 一致 |
| <code>ImageMap.AccessKey=k</code> | ✅ 一致 |
| <code>ImageMap.AccessKey=markup</code> | ✅ 一致 |
| <code>ImageMap.AccessKey=text</code> | ✅ 一致 |
| <code>ImageMap.AlternateText=empty</code> | ✅ 一致 |
| <code>ImageMap.AlternateText=japanese</code> | ✅ 一致 |
| <code>ImageMap.AlternateText=markup</code> | ✅ 一致 |
| <code>ImageMap.AlternateText=text</code> | ✅ 一致 |
| <code>ImageMap.AlternateText=text+EnableViewState=false</code> | ✅ 一致 |
| <code>ImageMap.AlternateText=text+Visible=false</code> | ✅ 一致 |
| <code>ImageMap.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>ImageMap.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>ImageMap.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>ImageMap.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>ImageMap.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>ImageMap.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>ImageMap.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>ImageMap.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>ImageMap.BackColor=#3366CC</code> | ✅ 一致 |
| <code>ImageMap.BackColor=Red</code> | ✅ 一致 |
| <code>ImageMap.BackColor=Transparent</code> | ✅ 一致 |
| <code>ImageMap.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>ImageMap.BorderColor=Red</code> | ✅ 一致 |
| <code>ImageMap.BorderColor=Transparent</code> | ✅ 一致 |
| <code>ImageMap.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>ImageMap.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>ImageMap.BorderStyle=Double</code> | ✅ 一致 |
| <code>ImageMap.BorderStyle=Groove</code> | ✅ 一致 |
| <code>ImageMap.BorderStyle=Inset</code> | ✅ 一致 |
| <code>ImageMap.BorderStyle=None</code> | ✅ 一致 |
| <code>ImageMap.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>ImageMap.BorderStyle=Outset</code> | ✅ 一致 |
| <code>ImageMap.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>ImageMap.BorderStyle=Solid</code> | ✅ 一致 |
| <code>ImageMap.BorderWidth=120px</code> | ✅ 一致 |
| <code>ImageMap.BorderWidth=2em</code> | ✅ 一致 |
| <code>ImageMap.BorderWidth=50%</code> | ✅ 一致 |
| <code>ImageMap.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ImageMap.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ImageMap.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ImageMap.ClientIDMode=Static</code> | ✅ 一致 |
| <code>ImageMap.CssClass=empty</code> | ✅ 一致 |
| <code>ImageMap.CssClass=japanese</code> | ✅ 一致 |
| <code>ImageMap.CssClass=markup</code> | ✅ 一致 |
| <code>ImageMap.CssClass=text</code> | ✅ 一致 |
| <code>ImageMap.DescriptionUrl=empty</code> | ✅ 一致 |
| <code>ImageMap.DescriptionUrl=japanese</code> | ✅ 一致 |
| <code>ImageMap.DescriptionUrl=markup</code> | ✅ 一致 |
| <code>ImageMap.DescriptionUrl=text</code> | ✅ 一致 |
| <code>ImageMap.EnableTheming=false</code> | ✅ 一致 |
| <code>ImageMap.EnableTheming=true</code> | ✅ 一致 |
| <code>ImageMap.EnableViewState=false</code> | ✅ 一致 |
| <code>ImageMap.EnableViewState=true</code> | ✅ 一致 |
| <code>ImageMap.Enabled=false</code> | ✅ 一致 |
| <code>ImageMap.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>ImageMap.Enabled=true</code> | ✅ 一致 |
| <code>ImageMap.Font.Bold=false</code> | ✅ 一致 |
| <code>ImageMap.Font.Bold=true</code> | ✅ 一致 |
| <code>ImageMap.Font.Italic=false</code> | ✅ 一致 |
| <code>ImageMap.Font.Italic=true</code> | ✅ 一致 |
| <code>ImageMap.Font.Name=empty</code> | ✅ 一致 |
| <code>ImageMap.Font.Name=japanese</code> | ✅ 一致 |
| <code>ImageMap.Font.Name=markup</code> | ✅ 一致 |
| <code>ImageMap.Font.Name=text</code> | ✅ 一致 |
| <code>ImageMap.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>ImageMap.Font.Overline=false</code> | ✅ 一致 |
| <code>ImageMap.Font.Overline=true</code> | ✅ 一致 |
| <code>ImageMap.Font.Size=12pt</code> | ✅ 一致 |
| <code>ImageMap.Font.Size=150%</code> | ✅ 一致 |
| <code>ImageMap.Font.Size=Large</code> | ✅ 一致 |
| <code>ImageMap.Font.Strikeout=false</code> | ✅ 一致 |
| <code>ImageMap.Font.Strikeout=true</code> | ✅ 一致 |
| <code>ImageMap.Font.Underline=false</code> | ✅ 一致 |
| <code>ImageMap.Font.Underline=true</code> | ✅ 一致 |
| <code>ImageMap.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>ImageMap.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>ImageMap.ForeColor=Red</code> | ✅ 一致 |
| <code>ImageMap.ForeColor=Transparent</code> | ✅ 一致 |
| <code>ImageMap.GenerateEmptyAlternateText=false</code> | ✅ 一致 |
| <code>ImageMap.GenerateEmptyAlternateText=true</code> | ✅ 一致 |
| <code>ImageMap.Height=120px</code> | ✅ 一致 |
| <code>ImageMap.Height=2em</code> | ✅ 一致 |
| <code>ImageMap.Height=50%</code> | ✅ 一致 |
| <code>ImageMap.HotSpotMode=Inactive</code> | ✅ 一致 |
| <code>ImageMap.HotSpotMode=Inactive+Target=blank</code> | ✅ 一致 |
| <code>ImageMap.HotSpotMode=Navigate</code> | ✅ 一致 |
| <code>ImageMap.HotSpotMode=Navigate+HotSpots[0].NavigateUrl=app-relative</code> | ✅ 一致 |
| <code>ImageMap.HotSpotMode=NotSet</code> | ✅ 一致 |
| <code>ImageMap.HotSpotMode=PostBack</code> | ✅ 一致 |
| <code>ImageMap.HotSpotMode=PostBack+Enabled=false</code> | ✅ 一致 |
| <code>ImageMap.HotSpotMode=PostBack+HotSpots[0].PostBackValue=text</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].AccessKey=empty</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].AccessKey=japanese</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].AccessKey=markup</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].AccessKey=text</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].AlternateText=empty</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].AlternateText=japanese</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].AlternateText=markup</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].AlternateText=text</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].HotSpotMode=Inactive</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].HotSpotMode=Navigate</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].HotSpotMode=NotSet</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].HotSpotMode=PostBack</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].HotSpotMode=PostBack+HotSpots[0].PostBackValue=markup</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].NavigateUrl=app-relative</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].NavigateUrl=empty</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].NavigateUrl=japanese</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].NavigateUrl=markup</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].NavigateUrl=text</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].PostBackValue=empty</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].PostBackValue=japanese</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].PostBackValue=markup</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].PostBackValue=text</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].TabIndex=-1</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].TabIndex=0</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].TabIndex=3</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].Target=blank</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].Target=blank+HotSpots[0].AccessKey=text</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].Target=empty</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].Target=japanese</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].Target=markup</code> | ✅ 一致 |
| <code>ImageMap.HotSpots[0].Target=text</code> | ✅ 一致 |
| <code>ImageMap.ID=empty</code> | ✅ 一致 |
| <code>ImageMap.ID=japanese</code> | ✅ 一致 |
| <code>ImageMap.ID=markup</code> | ✅ 一致 |
| <code>ImageMap.ID=text</code> | ✅ 一致 |
| <code>ImageMap.ImageAlign=AbsBottom</code> | ✅ 一致 |
| <code>ImageMap.ImageAlign=AbsMiddle</code> | ✅ 一致 |
| <code>ImageMap.ImageAlign=Baseline</code> | ✅ 一致 |
| <code>ImageMap.ImageAlign=Bottom</code> | ✅ 一致 |
| <code>ImageMap.ImageAlign=Left</code> | ✅ 一致 |
| <code>ImageMap.ImageAlign=Middle</code> | ✅ 一致 |
| <code>ImageMap.ImageAlign=NotSet</code> | ✅ 一致 |
| <code>ImageMap.ImageAlign=Right</code> | ✅ 一致 |
| <code>ImageMap.ImageAlign=TextTop</code> | ✅ 一致 |
| <code>ImageMap.ImageAlign=Top</code> | ✅ 一致 |
| <code>ImageMap.ImageUrl=absolute</code> | ✅ 一致 |
| <code>ImageMap.ImageUrl=app-relative</code> | ✅ 一致 |
| <code>ImageMap.ImageUrl=empty</code> | ✅ 一致 |
| <code>ImageMap.ImageUrl=japanese</code> | ✅ 一致 |
| <code>ImageMap.ImageUrl=markup</code> | ✅ 一致 |
| <code>ImageMap.ImageUrl=text</code> | ✅ 一致 |
| <code>ImageMap.SkinID=empty</code> | ✅ 一致 |
| <code>ImageMap.SkinID=japanese</code> | ✅ 一致 |
| <code>ImageMap.SkinID=markup</code> | ✅ 一致 |
| <code>ImageMap.SkinID=text</code> | ✅ 一致 |
| <code>ImageMap.Style[color]=1em</code> | ✅ 一致 |
| <code>ImageMap.Style[color]=red</code> | ✅ 一致 |
| <code>ImageMap.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>ImageMap.Style[margin-left]=red</code> | ✅ 一致 |
| <code>ImageMap.TabIndex=-1</code> | ✅ 一致 |
| <code>ImageMap.TabIndex=0</code> | ✅ 一致 |
| <code>ImageMap.TabIndex=3</code> | ✅ 一致 |
| <code>ImageMap.Target=blank</code> | ✅ 一致 |
| <code>ImageMap.Target=empty</code> | ✅ 一致 |
| <code>ImageMap.Target=japanese</code> | ✅ 一致 |
| <code>ImageMap.Target=markup</code> | ✅ 一致 |
| <code>ImageMap.Target=text</code> | ✅ 一致 |
| <code>ImageMap.ToolTip=empty</code> | ✅ 一致 |
| <code>ImageMap.ToolTip=japanese</code> | ✅ 一致 |
| <code>ImageMap.ToolTip=markup</code> | ✅ 一致 |
| <code>ImageMap.ToolTip=text</code> | ✅ 一致 |
| <code>ImageMap.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>ImageMap.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>ImageMap.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>ImageMap.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>ImageMap.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>ImageMap.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>ImageMap.Visible=false</code> | ✅ 一致 |
| <code>ImageMap.Visible=true</code> | ✅ 一致 |
| <code>ImageMap.Width=120px</code> | ✅ 一致 |
| <code>ImageMap.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>ImageMap.Width=2em</code> | ✅ 一致 |
| <code>ImageMap.Width=50%</code> | ✅ 一致 |
| <code>ImageMap[naming container].(default)</code> | ✅ 一致 |
| <code>ImageMap[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ImageMap[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ImageMap[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ImageMap[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>ImageMap[naming container].HotSpotMode=PostBack</code> | ✅ 一致 |
| <code>ImageMap[naming container].ID=text</code> | ✅ 一致 |
| <code>ImageMap[themed].(default)</code> | ✅ 一致 |
| <code>ImageMap[themed].CssClass=text</code> | ✅ 一致 |
| <code>ImageMap[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>ImageMap[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| ImageMap [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| ImageMap [viewstate][dynamic] posted: files, view state, what code set (a table&#39;s rows and a literal control&#39;s text are not kept) | ✅ 一致 |
| ImageMap [viewstate] the next view (NextView bubbled from a button) | ✅ 一致 |
| ImageMap [viewstate] switched by ID and by index: the first view&#39;s text box kept its value | ✅ 一致 |
| ImageMap [dynamic][viewstate] a dynamic multi-view&#39;s next view | ✅ 一致 |
| ImageMap [viewstate] a calendar&#39;s day chosen | ✅ 一致 |
| ImageMap [viewstate] a calendar&#39;s week chosen | ❌ 未実行 |
| ImageMap [viewstate] a calendar&#39;s next month | ✅ 一致 |
| ImageMap [viewstate] a dynamic calendar&#39;s day chosen | ✅ 一致 |
| ImageMap [postbackevent] an image map&#39;s hot spot clicked (PostBack) | ✅ 一致 |
| ImageMap [postbackevent][dynamic] a dynamic image map&#39;s hot spot clicked | ✅ 一致 |
| ImageMap [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| ImageMap [viewstate] a file upload validated on the server (g1): no file posted | ✅ 一致 |
| ImageMap [viewstate] a postback event to a table (not registered for event validation): rejected | ✅ 一致 |

</details>
