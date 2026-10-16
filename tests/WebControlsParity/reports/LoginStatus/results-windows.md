# LoginStatus パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-15 13:43
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 168 | 168 | 0 | 0 |
| ② HTML | 151 | 151 | 0 | 0 |
| ③ シナリオ | 15 | 15 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 177 件中 177 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 35 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 15 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 133 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 14 | ✅ |
| Visible=false（描画されず状態は保持） | 3 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
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
| <code>LoginStatus.Api.Control.#ctor</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.Adapter</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.ClientID</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.Context</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.Controls</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.DataBind</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.Dispose</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.Disposed</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.Events</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.FindControl</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.Focus</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.HasControls</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.ID</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.Init</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.Load</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.OnInit</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.Page</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.Parent</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.PreRender</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.Render</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.Site</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.SkinID</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.Unload</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.ViewState</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>LoginStatus.Api.Control.Visible</code> | ✅ 一致 |
| <code>LoginStatus.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>LoginStatus.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>LoginStatus.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>LoginStatus.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>LoginStatus.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>LoginStatus.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>LoginStatus.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>LoginStatus.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>LoginStatus.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>LoginStatus.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>LoginStatus.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>LoginStatus.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>LoginStatus.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.CompositeControl.#ctor</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.CompositeControl.Controls</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.CompositeControl.DataBind</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.CompositeControl.RecreateChildControls</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.CompositeControl.Render</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.CompositeControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.ICompositeControlDesignerAccessor.RecreateChildControls</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.LoginStatus.#ctor</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.LoginStatus.CreateChildControls</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.LoginStatus.LoggedOut</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.LoginStatus.LoggingOut</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.LoginStatus.LoginImageUrl</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.LoginStatus.LoginText</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.LoginStatus.LogoutAction</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.LoginStatus.LogoutImageUrl</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.LoginStatus.LogoutPageUrl</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.LoginStatus.LogoutText</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.LoginStatus.OnLoggedOut</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.LoginStatus.OnLoggingOut</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.LoginStatus.OnPreRender</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.LoginStatus.Render</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.LoginStatus.RenderContents</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.LoginStatus.SetDesignModeState</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.LoginStatus.TagKey</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>LoginStatus.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>LoginStatus.(case list)</code> | ✅ 一致 |
| <code>LoginStatus.(default)</code> | ✅ 一致 |
| <code>LoginStatus.AccessKey=empty</code> | ✅ 一致 |
| <code>LoginStatus.AccessKey=japanese</code> | ✅ 一致 |
| <code>LoginStatus.AccessKey=markup</code> | ✅ 一致 |
| <code>LoginStatus.AccessKey=text</code> | ✅ 一致 |
| <code>LoginStatus.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>LoginStatus.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>LoginStatus.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>LoginStatus.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>LoginStatus.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>LoginStatus.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>LoginStatus.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>LoginStatus.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>LoginStatus.BackColor=#3366CC</code> | ✅ 一致 |
| <code>LoginStatus.BackColor=Red</code> | ✅ 一致 |
| <code>LoginStatus.BackColor=Transparent</code> | ✅ 一致 |
| <code>LoginStatus.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>LoginStatus.BorderColor=Red</code> | ✅ 一致 |
| <code>LoginStatus.BorderColor=Transparent</code> | ✅ 一致 |
| <code>LoginStatus.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>LoginStatus.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>LoginStatus.BorderStyle=Double</code> | ✅ 一致 |
| <code>LoginStatus.BorderStyle=Groove</code> | ✅ 一致 |
| <code>LoginStatus.BorderStyle=Inset</code> | ✅ 一致 |
| <code>LoginStatus.BorderStyle=None</code> | ✅ 一致 |
| <code>LoginStatus.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>LoginStatus.BorderStyle=Outset</code> | ✅ 一致 |
| <code>LoginStatus.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>LoginStatus.BorderStyle=Solid</code> | ✅ 一致 |
| <code>LoginStatus.BorderWidth=120px</code> | ✅ 一致 |
| <code>LoginStatus.BorderWidth=2em</code> | ✅ 一致 |
| <code>LoginStatus.BorderWidth=50%</code> | ✅ 一致 |
| <code>LoginStatus.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>LoginStatus.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>LoginStatus.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>LoginStatus.ClientIDMode=Static</code> | ✅ 一致 |
| <code>LoginStatus.CssClass=empty</code> | ✅ 一致 |
| <code>LoginStatus.CssClass=japanese</code> | ✅ 一致 |
| <code>LoginStatus.CssClass=markup</code> | ✅ 一致 |
| <code>LoginStatus.CssClass=text</code> | ✅ 一致 |
| <code>LoginStatus.EnableTheming=false</code> | ✅ 一致 |
| <code>LoginStatus.EnableTheming=true</code> | ✅ 一致 |
| <code>LoginStatus.EnableViewState=false</code> | ✅ 一致 |
| <code>LoginStatus.EnableViewState=true</code> | ✅ 一致 |
| <code>LoginStatus.Enabled=false</code> | ✅ 一致 |
| <code>LoginStatus.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>LoginStatus.Enabled=true</code> | ✅ 一致 |
| <code>LoginStatus.Font.Bold=false</code> | ✅ 一致 |
| <code>LoginStatus.Font.Bold=true</code> | ✅ 一致 |
| <code>LoginStatus.Font.Italic=false</code> | ✅ 一致 |
| <code>LoginStatus.Font.Italic=true</code> | ✅ 一致 |
| <code>LoginStatus.Font.Name=empty</code> | ✅ 一致 |
| <code>LoginStatus.Font.Name=japanese</code> | ✅ 一致 |
| <code>LoginStatus.Font.Name=markup</code> | ✅ 一致 |
| <code>LoginStatus.Font.Name=text</code> | ✅ 一致 |
| <code>LoginStatus.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>LoginStatus.Font.Overline=false</code> | ✅ 一致 |
| <code>LoginStatus.Font.Overline=true</code> | ✅ 一致 |
| <code>LoginStatus.Font.Size=12pt</code> | ✅ 一致 |
| <code>LoginStatus.Font.Size=150%</code> | ✅ 一致 |
| <code>LoginStatus.Font.Size=Large</code> | ✅ 一致 |
| <code>LoginStatus.Font.Strikeout=false</code> | ✅ 一致 |
| <code>LoginStatus.Font.Strikeout=true</code> | ✅ 一致 |
| <code>LoginStatus.Font.Underline=false</code> | ✅ 一致 |
| <code>LoginStatus.Font.Underline=true</code> | ✅ 一致 |
| <code>LoginStatus.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>LoginStatus.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>LoginStatus.ForeColor=Red</code> | ✅ 一致 |
| <code>LoginStatus.ForeColor=Transparent</code> | ✅ 一致 |
| <code>LoginStatus.Height=120px</code> | ✅ 一致 |
| <code>LoginStatus.Height=2em</code> | ✅ 一致 |
| <code>LoginStatus.Height=50%</code> | ✅ 一致 |
| <code>LoginStatus.ID=empty</code> | ✅ 一致 |
| <code>LoginStatus.ID=japanese</code> | ✅ 一致 |
| <code>LoginStatus.ID=markup</code> | ✅ 一致 |
| <code>LoginStatus.ID=text</code> | ✅ 一致 |
| <code>LoginStatus.LoginImageUrl=app-relative</code> | ✅ 一致 |
| <code>LoginStatus.LoginImageUrl=app-relative+LoginText=text</code> | ✅ 一致 |
| <code>LoginStatus.LoginImageUrl=empty</code> | ✅ 一致 |
| <code>LoginStatus.LoginImageUrl=japanese</code> | ✅ 一致 |
| <code>LoginStatus.LoginImageUrl=markup</code> | ✅ 一致 |
| <code>LoginStatus.LoginImageUrl=text</code> | ✅ 一致 |
| <code>LoginStatus.LoginText=empty</code> | ✅ 一致 |
| <code>LoginStatus.LoginText=japanese</code> | ✅ 一致 |
| <code>LoginStatus.LoginText=markup</code> | ✅ 一致 |
| <code>LoginStatus.LoginText=markup+LogoutText=markup</code> | ✅ 一致 |
| <code>LoginStatus.LoginText=text</code> | ✅ 一致 |
| <code>LoginStatus.LoginText=text+EnableViewState=false</code> | ✅ 一致 |
| <code>LoginStatus.LoginText=text+Visible=false</code> | ✅ 一致 |
| <code>LoginStatus.LogoutAction=Redirect</code> | ✅ 一致 |
| <code>LoginStatus.LogoutAction=Redirect+LogoutPageUrl=app-relative</code> | ✅ 一致 |
| <code>LoginStatus.LogoutAction=RedirectToLoginPage</code> | ✅ 一致 |
| <code>LoginStatus.LogoutAction=Refresh</code> | ✅ 一致 |
| <code>LoginStatus.LogoutImageUrl=app-relative</code> | ✅ 一致 |
| <code>LoginStatus.LogoutImageUrl=empty</code> | ✅ 一致 |
| <code>LoginStatus.LogoutImageUrl=japanese</code> | ✅ 一致 |
| <code>LoginStatus.LogoutImageUrl=markup</code> | ✅ 一致 |
| <code>LoginStatus.LogoutImageUrl=text</code> | ✅ 一致 |
| <code>LoginStatus.LogoutPageUrl=app-relative</code> | ✅ 一致 |
| <code>LoginStatus.LogoutPageUrl=empty</code> | ✅ 一致 |
| <code>LoginStatus.LogoutPageUrl=japanese</code> | ✅ 一致 |
| <code>LoginStatus.LogoutPageUrl=markup</code> | ✅ 一致 |
| <code>LoginStatus.LogoutPageUrl=text</code> | ✅ 一致 |
| <code>LoginStatus.LogoutText=empty</code> | ✅ 一致 |
| <code>LoginStatus.LogoutText=japanese</code> | ✅ 一致 |
| <code>LoginStatus.LogoutText=markup</code> | ✅ 一致 |
| <code>LoginStatus.LogoutText=text</code> | ✅ 一致 |
| <code>LoginStatus.SkinID=empty</code> | ✅ 一致 |
| <code>LoginStatus.SkinID=japanese</code> | ✅ 一致 |
| <code>LoginStatus.SkinID=markup</code> | ✅ 一致 |
| <code>LoginStatus.SkinID=text</code> | ✅ 一致 |
| <code>LoginStatus.Style[color]=1em</code> | ✅ 一致 |
| <code>LoginStatus.Style[color]=red</code> | ✅ 一致 |
| <code>LoginStatus.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>LoginStatus.Style[margin-left]=red</code> | ✅ 一致 |
| <code>LoginStatus.TabIndex=-1</code> | ✅ 一致 |
| <code>LoginStatus.TabIndex=0</code> | ✅ 一致 |
| <code>LoginStatus.TabIndex=3</code> | ✅ 一致 |
| <code>LoginStatus.ToolTip=empty</code> | ✅ 一致 |
| <code>LoginStatus.ToolTip=japanese</code> | ✅ 一致 |
| <code>LoginStatus.ToolTip=markup</code> | ✅ 一致 |
| <code>LoginStatus.ToolTip=text</code> | ✅ 一致 |
| <code>LoginStatus.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>LoginStatus.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>LoginStatus.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>LoginStatus.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>LoginStatus.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>LoginStatus.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>LoginStatus.Visible=false</code> | ✅ 一致 |
| <code>LoginStatus.Visible=true</code> | ✅ 一致 |
| <code>LoginStatus.Width=120px</code> | ✅ 一致 |
| <code>LoginStatus.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>LoginStatus.Width=2em</code> | ✅ 一致 |
| <code>LoginStatus.Width=50%</code> | ✅ 一致 |
| <code>LoginStatus[logged in].(default)</code> | ✅ 一致 |
| <code>LoginStatus[logged in].Enabled=false</code> | ✅ 一致 |
| <code>LoginStatus[logged in].LogoutAction=RedirectToLoginPage</code> | ✅ 一致 |
| <code>LoginStatus[logged in].LogoutImageUrl=app-relative+LogoutText=text</code> | ✅ 一致 |
| <code>LoginStatus[logged in].LogoutText=markup</code> | ✅ 一致 |
| <code>LoginStatus[logged in].LogoutText=text+Visible=false</code> | ✅ 一致 |
| <code>LoginStatus[naming container].(default)</code> | ✅ 一致 |
| <code>LoginStatus[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>LoginStatus[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>LoginStatus[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>LoginStatus[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>LoginStatus[naming container].ID=text</code> | ✅ 一致 |
| <code>LoginStatus[themed].(default)</code> | ✅ 一致 |
| <code>LoginStatus[themed].CssClass=text</code> | ✅ 一致 |
| <code>LoginStatus[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>LoginStatus[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| LoginStatus [render][databind][dynamic] first request, anonymous: the login forms, the status, the views, the wizards | ✅ 一致 |
| LoginStatus [viewstate] a login with a wrong password: its error | ✅ 一致 |
| LoginStatus [viewstate] logged in (redirected to the page): the name, the status, the role&#39;s view | ✅ 一致 |
| LoginStatus [viewstate] the password changed: the success template | ✅ 一致 |
| LoginStatus [viewstate][templates] logged out by the status | ✅ 一致 |
| LoginStatus [viewstate] a recovery for a user who does not exist | ✅ 一致 |
| LoginStatus [viewstate] the user&#39;s question | ✅ 一致 |
| LoginStatus [viewstate] the answer right: the password reset, its mail prepared (cancelled), the success template | ✅ 一致 |
| LoginStatus [viewstate] a user created: the complete step | ✅ 一致 |
| LoginStatus [viewstate] the wizard&#39;s next step (the templated one) | ✅ 一致 |
| LoginStatus [viewstate] next, then previous | ✅ 一致 |
| LoginStatus [viewstate] a step chosen in the side bar | ✅ 一致 |
| LoginStatus [viewstate] finished: the complete step, the first step&#39;s value kept | ✅ 一致 |
| LoginStatus [dynamic][viewstate] a wizard made in code: its next step | ✅ 一致 |
| LoginStatus [visible][viewstate] a hidden wizard shown | ✅ 一致 |

</details>
