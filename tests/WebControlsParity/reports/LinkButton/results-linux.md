# LinkButton パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 02:32
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 173 | 173 | 0 | 0 |
| ② HTML | 158 | 158 | 0 | 0 |
| ③ シナリオ | 9 | 9 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 182 件中 182 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 36 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 9 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 144 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 2 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 8 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 2 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 5 | 0 | ✅ |
| ポストバックイベント | 0 | 4 | ✅ |
| コマンドイベントと親へのバブル（CommandName） | 9 | 2 | ✅ |
| ブラウザ側のクリック処理（OnClientClick） | 9 | 1 | ✅ |
| クロスページポストバック（PostBackUrl） | 11 | 1 | ✅ |
| 既定のボタン（Panel の DefaultButton） | 0 | 1 | ✅ |
| CausesValidation / ValidationGroup | 11 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>LinkButton.Api.Control.#ctor</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.Adapter</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.ClientID</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.Context</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.Controls</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.DataBind</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.Dispose</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.Disposed</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.Events</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.FindControl</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.Focus</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.HasControls</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.ID</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.Init</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.Load</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.OnInit</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.Page</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.Parent</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.PreRender</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.Render</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.Site</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.SkinID</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.Unload</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.ViewState</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>LinkButton.Api.Control.Visible</code> | ✅ 一致 |
| <code>LinkButton.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>LinkButton.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>LinkButton.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>LinkButton.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>LinkButton.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>LinkButton.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>LinkButton.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>LinkButton.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>LinkButton.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>LinkButton.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>LinkButton.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>LinkButton.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>LinkButton.Api.IPostBackEventHandler.RaisePostBackEvent</code> | ✅ 一致 |
| <code>LinkButton.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.IButtonControl.CausesValidation</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.IButtonControl.Click</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.IButtonControl.Command</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.IButtonControl.CommandArgument</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.IButtonControl.CommandName</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.IButtonControl.PostBackUrl</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.IButtonControl.Text</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.IButtonControl.ValidationGroup</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.LinkButton.#ctor</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.LinkButton.AddAttributesToRender</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.LinkButton.AddParsedSubObject</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.LinkButton.CausesValidation</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.LinkButton.Click</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.LinkButton.Command</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.LinkButton.CommandArgument</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.LinkButton.CommandName</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.LinkButton.GetPostBackOptions</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.LinkButton.LoadViewState</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.LinkButton.OnClick</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.LinkButton.OnClientClick</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.LinkButton.OnCommand</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.LinkButton.OnPreRender</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.LinkButton.PostBackUrl</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.LinkButton.RaisePostBackEvent</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.LinkButton.RenderContents</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.LinkButton.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.LinkButton.Text</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.LinkButton.ValidationGroup</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>LinkButton.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>LinkButton.(case list)</code> | ✅ 一致 |
| <code>LinkButton.(default)</code> | ✅ 一致 |
| <code>LinkButton.AccessKey=empty</code> | ✅ 一致 |
| <code>LinkButton.AccessKey=japanese</code> | ✅ 一致 |
| <code>LinkButton.AccessKey=k</code> | ✅ 一致 |
| <code>LinkButton.AccessKey=markup</code> | ✅ 一致 |
| <code>LinkButton.AccessKey=text</code> | ✅ 一致 |
| <code>LinkButton.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>LinkButton.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>LinkButton.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>LinkButton.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>LinkButton.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>LinkButton.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>LinkButton.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>LinkButton.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>LinkButton.BackColor=#3366CC</code> | ✅ 一致 |
| <code>LinkButton.BackColor=Red</code> | ✅ 一致 |
| <code>LinkButton.BackColor=Transparent</code> | ✅ 一致 |
| <code>LinkButton.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>LinkButton.BorderColor=Red</code> | ✅ 一致 |
| <code>LinkButton.BorderColor=Transparent</code> | ✅ 一致 |
| <code>LinkButton.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>LinkButton.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>LinkButton.BorderStyle=Double</code> | ✅ 一致 |
| <code>LinkButton.BorderStyle=Groove</code> | ✅ 一致 |
| <code>LinkButton.BorderStyle=Inset</code> | ✅ 一致 |
| <code>LinkButton.BorderStyle=None</code> | ✅ 一致 |
| <code>LinkButton.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>LinkButton.BorderStyle=Outset</code> | ✅ 一致 |
| <code>LinkButton.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>LinkButton.BorderStyle=Solid</code> | ✅ 一致 |
| <code>LinkButton.BorderWidth=120px</code> | ✅ 一致 |
| <code>LinkButton.BorderWidth=2em</code> | ✅ 一致 |
| <code>LinkButton.BorderWidth=50%</code> | ✅ 一致 |
| <code>LinkButton.CausesValidation=false</code> | ✅ 一致 |
| <code>LinkButton.CausesValidation=true</code> | ✅ 一致 |
| <code>LinkButton.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>LinkButton.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>LinkButton.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>LinkButton.ClientIDMode=Static</code> | ✅ 一致 |
| <code>LinkButton.CommandArgument=empty</code> | ✅ 一致 |
| <code>LinkButton.CommandArgument=japanese</code> | ✅ 一致 |
| <code>LinkButton.CommandArgument=markup</code> | ✅ 一致 |
| <code>LinkButton.CommandArgument=text</code> | ✅ 一致 |
| <code>LinkButton.CommandName=empty</code> | ✅ 一致 |
| <code>LinkButton.CommandName=japanese</code> | ✅ 一致 |
| <code>LinkButton.CommandName=markup</code> | ✅ 一致 |
| <code>LinkButton.CommandName=text</code> | ✅ 一致 |
| <code>LinkButton.CommandName=text+CommandArgument=markup</code> | ✅ 一致 |
| <code>LinkButton.CssClass=empty</code> | ✅ 一致 |
| <code>LinkButton.CssClass=japanese</code> | ✅ 一致 |
| <code>LinkButton.CssClass=markup</code> | ✅ 一致 |
| <code>LinkButton.CssClass=text</code> | ✅ 一致 |
| <code>LinkButton.EnableTheming=false</code> | ✅ 一致 |
| <code>LinkButton.EnableTheming=true</code> | ✅ 一致 |
| <code>LinkButton.EnableViewState=false</code> | ✅ 一致 |
| <code>LinkButton.EnableViewState=true</code> | ✅ 一致 |
| <code>LinkButton.Enabled=false</code> | ✅ 一致 |
| <code>LinkButton.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>LinkButton.Enabled=true</code> | ✅ 一致 |
| <code>LinkButton.Font.Bold=false</code> | ✅ 一致 |
| <code>LinkButton.Font.Bold=true</code> | ✅ 一致 |
| <code>LinkButton.Font.Italic=false</code> | ✅ 一致 |
| <code>LinkButton.Font.Italic=true</code> | ✅ 一致 |
| <code>LinkButton.Font.Name=empty</code> | ✅ 一致 |
| <code>LinkButton.Font.Name=japanese</code> | ✅ 一致 |
| <code>LinkButton.Font.Name=markup</code> | ✅ 一致 |
| <code>LinkButton.Font.Name=text</code> | ✅ 一致 |
| <code>LinkButton.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>LinkButton.Font.Overline=false</code> | ✅ 一致 |
| <code>LinkButton.Font.Overline=true</code> | ✅ 一致 |
| <code>LinkButton.Font.Size=12pt</code> | ✅ 一致 |
| <code>LinkButton.Font.Size=150%</code> | ✅ 一致 |
| <code>LinkButton.Font.Size=Large</code> | ✅ 一致 |
| <code>LinkButton.Font.Strikeout=false</code> | ✅ 一致 |
| <code>LinkButton.Font.Strikeout=true</code> | ✅ 一致 |
| <code>LinkButton.Font.Underline=false</code> | ✅ 一致 |
| <code>LinkButton.Font.Underline=true</code> | ✅ 一致 |
| <code>LinkButton.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>LinkButton.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>LinkButton.ForeColor=Red</code> | ✅ 一致 |
| <code>LinkButton.ForeColor=Transparent</code> | ✅ 一致 |
| <code>LinkButton.Height=120px</code> | ✅ 一致 |
| <code>LinkButton.Height=2em</code> | ✅ 一致 |
| <code>LinkButton.Height=50%</code> | ✅ 一致 |
| <code>LinkButton.ID=empty</code> | ✅ 一致 |
| <code>LinkButton.ID=japanese</code> | ✅ 一致 |
| <code>LinkButton.ID=markup</code> | ✅ 一致 |
| <code>LinkButton.ID=text</code> | ✅ 一致 |
| <code>LinkButton.OnClientClick=confirm</code> | ✅ 一致 |
| <code>LinkButton.OnClientClick=confirm+CausesValidation=false</code> | ✅ 一致 |
| <code>LinkButton.OnClientClick=confirm+Enabled=false</code> | ✅ 一致 |
| <code>LinkButton.OnClientClick=confirm+ValidationGroup=text</code> | ✅ 一致 |
| <code>LinkButton.OnClientClick=empty</code> | ✅ 一致 |
| <code>LinkButton.OnClientClick=japanese</code> | ✅ 一致 |
| <code>LinkButton.OnClientClick=markup</code> | ✅ 一致 |
| <code>LinkButton.OnClientClick=text</code> | ✅ 一致 |
| <code>LinkButton.PostBackUrl=absolute</code> | ✅ 一致 |
| <code>LinkButton.PostBackUrl=app-relative</code> | ✅ 一致 |
| <code>LinkButton.PostBackUrl=app-relative+CausesValidation=false</code> | ✅ 一致 |
| <code>LinkButton.PostBackUrl=app-relative+Enabled=false</code> | ✅ 一致 |
| <code>LinkButton.PostBackUrl=app-relative+OnClientClick=confirm</code> | ✅ 一致 |
| <code>LinkButton.PostBackUrl=app-relative+ValidationGroup=text</code> | ✅ 一致 |
| <code>LinkButton.PostBackUrl=empty</code> | ✅ 一致 |
| <code>LinkButton.PostBackUrl=japanese</code> | ✅ 一致 |
| <code>LinkButton.PostBackUrl=markup</code> | ✅ 一致 |
| <code>LinkButton.PostBackUrl=text</code> | ✅ 一致 |
| <code>LinkButton.SkinID=empty</code> | ✅ 一致 |
| <code>LinkButton.SkinID=japanese</code> | ✅ 一致 |
| <code>LinkButton.SkinID=markup</code> | ✅ 一致 |
| <code>LinkButton.SkinID=text</code> | ✅ 一致 |
| <code>LinkButton.Style[color]=1em</code> | ✅ 一致 |
| <code>LinkButton.Style[color]=red</code> | ✅ 一致 |
| <code>LinkButton.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>LinkButton.Style[margin-left]=red</code> | ✅ 一致 |
| <code>LinkButton.TabIndex=-1</code> | ✅ 一致 |
| <code>LinkButton.TabIndex=0</code> | ✅ 一致 |
| <code>LinkButton.TabIndex=3</code> | ✅ 一致 |
| <code>LinkButton.Text=empty</code> | ✅ 一致 |
| <code>LinkButton.Text=japanese</code> | ✅ 一致 |
| <code>LinkButton.Text=markup</code> | ✅ 一致 |
| <code>LinkButton.Text=markup+EnableViewState=false</code> | ✅ 一致 |
| <code>LinkButton.Text=markup+ToolTip=markup</code> | ✅ 一致 |
| <code>LinkButton.Text=text</code> | ✅ 一致 |
| <code>LinkButton.Text=text+Enabled=false</code> | ✅ 一致 |
| <code>LinkButton.Text=text+Visible=false</code> | ✅ 一致 |
| <code>LinkButton.ToolTip=empty</code> | ✅ 一致 |
| <code>LinkButton.ToolTip=japanese</code> | ✅ 一致 |
| <code>LinkButton.ToolTip=markup</code> | ✅ 一致 |
| <code>LinkButton.ToolTip=text</code> | ✅ 一致 |
| <code>LinkButton.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>LinkButton.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>LinkButton.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>LinkButton.ValidationGroup=empty</code> | ✅ 一致 |
| <code>LinkButton.ValidationGroup=japanese</code> | ✅ 一致 |
| <code>LinkButton.ValidationGroup=markup</code> | ✅ 一致 |
| <code>LinkButton.ValidationGroup=text</code> | ✅ 一致 |
| <code>LinkButton.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>LinkButton.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>LinkButton.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>LinkButton.Visible=false</code> | ✅ 一致 |
| <code>LinkButton.Visible=true</code> | ✅ 一致 |
| <code>LinkButton.Width=120px</code> | ✅ 一致 |
| <code>LinkButton.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>LinkButton.Width=2em</code> | ✅ 一致 |
| <code>LinkButton.Width=50%</code> | ✅ 一致 |
| <code>LinkButton[naming container].(default)</code> | ✅ 一致 |
| <code>LinkButton[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>LinkButton[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>LinkButton[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>LinkButton[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>LinkButton[naming container].ID=text</code> | ✅ 一致 |
| <code>LinkButton[naming container].PostBackUrl=app-relative</code> | ✅ 一致 |
| <code>LinkButton[naming container].ValidationGroup=text</code> | ✅ 一致 |
| <code>LinkButton[themed].(default)</code> | ✅ 一致 |
| <code>LinkButton[themed].CssClass=text</code> | ✅ 一致 |
| <code>LinkButton[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>LinkButton[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| LinkButton [render][databind][dynamic] first request | ✅ 一致 |
| LinkButton [postbackevent][command][viewstate] a link: Click, then Command | ✅ 一致 |
| LinkButton [clientclick] OnClientClick returns false: no request | ✅ 一致 |
| LinkButton [causesvalidation][postbackevent] validating g1 (empty) | ✅ 一致 |
| LinkButton [command][databind] Command bubbled from a repeater item | ✅ 一致 |
| LinkButton [defaultbutton] Enter in a panel whose DefaultButton is the link | ✅ 一致 |
| LinkButton [dynamic][postbackevent] a dynamically created link | ✅ 一致 |
| LinkButton [visible][viewstate][postbackevent] the link shown before raises Click | ✅ 一致 |
| LinkButton [crosspage] cross-page postback (PostBackUrl) | ✅ 一致 |

</details>
