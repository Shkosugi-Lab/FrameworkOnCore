# Button パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-16 17:48
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 171 | 171 | 0 | 0 |
| ② HTML | 168 | 168 | 0 | 0 |
| ③ シナリオ | 15 | 15 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 180 件中 180 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 37 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 15 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 152 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 3 | ✅ |
| Visible=false（描画されず状態は保持） | 3 | 2 | ✅ |
| 名前付けコンテナと ClientIDMode | 9 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 5 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 1 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 2 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 5 | 1 | ✅ |
| ポストバックイベント | 0 | 5 | ✅ |
| コマンドイベントと親へのバブル（CommandName） | 9 | 2 | ✅ |
| 送信ボタン／スクリプトによるポストバック（UseSubmitBehavior） | 10 | 1 | ✅ |
| ブラウザ側のクリック処理（OnClientClick） | 10 | 2 | ✅ |
| クロスページポストバック（PostBackUrl） | 12 | 1 | ✅ |
| 既定のボタン（Panel の DefaultButton） | 0 | 1 | ✅ |
| CausesValidation / ValidationGroup | 13 | 4 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>Button.Api.Control.#ctor</code> | ✅ 一致 |
| <code>Button.Api.Control.Adapter</code> | ✅ 一致 |
| <code>Button.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>Button.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>Button.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>Button.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>Button.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>Button.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>Button.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>Button.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>Button.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>Button.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>Button.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>Button.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>Button.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>Button.Api.Control.ClientID</code> | ✅ 一致 |
| <code>Button.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>Button.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>Button.Api.Control.Context</code> | ✅ 一致 |
| <code>Button.Api.Control.Controls</code> | ✅ 一致 |
| <code>Button.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>Button.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>Button.Api.Control.DataBind</code> | ✅ 一致 |
| <code>Button.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>Button.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>Button.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>Button.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>Button.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>Button.Api.Control.Dispose</code> | ✅ 一致 |
| <code>Button.Api.Control.Disposed</code> | ✅ 一致 |
| <code>Button.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>Button.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>Button.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>Button.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>Button.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>Button.Api.Control.Events</code> | ✅ 一致 |
| <code>Button.Api.Control.FindControl</code> | ✅ 一致 |
| <code>Button.Api.Control.Focus</code> | ✅ 一致 |
| <code>Button.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>Button.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>Button.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>Button.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>Button.Api.Control.HasControls</code> | ✅ 一致 |
| <code>Button.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>Button.Api.Control.ID</code> | ✅ 一致 |
| <code>Button.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>Button.Api.Control.Init</code> | ✅ 一致 |
| <code>Button.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>Button.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>Button.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>Button.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>Button.Api.Control.Load</code> | ✅ 一致 |
| <code>Button.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>Button.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>Button.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>Button.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>Button.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>Button.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>Button.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>Button.Api.Control.OnInit</code> | ✅ 一致 |
| <code>Button.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>Button.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>Button.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>Button.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>Button.Api.Control.Page</code> | ✅ 一致 |
| <code>Button.Api.Control.Parent</code> | ✅ 一致 |
| <code>Button.Api.Control.PreRender</code> | ✅ 一致 |
| <code>Button.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>Button.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>Button.Api.Control.Render</code> | ✅ 一致 |
| <code>Button.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>Button.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>Button.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>Button.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>Button.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>Button.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>Button.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>Button.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>Button.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>Button.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>Button.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>Button.Api.Control.Site</code> | ✅ 一致 |
| <code>Button.Api.Control.SkinID</code> | ✅ 一致 |
| <code>Button.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>Button.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>Button.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>Button.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>Button.Api.Control.Unload</code> | ✅ 一致 |
| <code>Button.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>Button.Api.Control.ViewState</code> | ✅ 一致 |
| <code>Button.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>Button.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>Button.Api.Control.Visible</code> | ✅ 一致 |
| <code>Button.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>Button.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>Button.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>Button.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>Button.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>Button.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>Button.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>Button.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>Button.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>Button.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>Button.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>Button.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>Button.Api.IPostBackEventHandler.RaisePostBackEvent</code> | ✅ 一致 |
| <code>Button.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>Button.Api.WebControls.Button.#ctor</code> | ✅ 一致 |
| <code>Button.Api.WebControls.Button.AddAttributesToRender</code> | ✅ 一致 |
| <code>Button.Api.WebControls.Button.CausesValidation</code> | ✅ 一致 |
| <code>Button.Api.WebControls.Button.Click</code> | ✅ 一致 |
| <code>Button.Api.WebControls.Button.Command</code> | ✅ 一致 |
| <code>Button.Api.WebControls.Button.CommandArgument</code> | ✅ 一致 |
| <code>Button.Api.WebControls.Button.CommandName</code> | ✅ 一致 |
| <code>Button.Api.WebControls.Button.GetPostBackOptions</code> | ✅ 一致 |
| <code>Button.Api.WebControls.Button.OnClick</code> | ✅ 一致 |
| <code>Button.Api.WebControls.Button.OnClientClick</code> | ✅ 一致 |
| <code>Button.Api.WebControls.Button.OnCommand</code> | ✅ 一致 |
| <code>Button.Api.WebControls.Button.OnPreRender</code> | ✅ 一致 |
| <code>Button.Api.WebControls.Button.PostBackUrl</code> | ✅ 一致 |
| <code>Button.Api.WebControls.Button.RaisePostBackEvent</code> | ✅ 一致 |
| <code>Button.Api.WebControls.Button.RenderContents</code> | ✅ 一致 |
| <code>Button.Api.WebControls.Button.Text</code> | ✅ 一致 |
| <code>Button.Api.WebControls.Button.UseSubmitBehavior</code> | ✅ 一致 |
| <code>Button.Api.WebControls.Button.ValidationGroup</code> | ✅ 一致 |
| <code>Button.Api.WebControls.IButtonControl.CausesValidation</code> | ✅ 一致 |
| <code>Button.Api.WebControls.IButtonControl.Click</code> | ✅ 一致 |
| <code>Button.Api.WebControls.IButtonControl.Command</code> | ✅ 一致 |
| <code>Button.Api.WebControls.IButtonControl.CommandArgument</code> | ✅ 一致 |
| <code>Button.Api.WebControls.IButtonControl.CommandName</code> | ✅ 一致 |
| <code>Button.Api.WebControls.IButtonControl.PostBackUrl</code> | ✅ 一致 |
| <code>Button.Api.WebControls.IButtonControl.Text</code> | ✅ 一致 |
| <code>Button.Api.WebControls.IButtonControl.ValidationGroup</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>Button.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>Button.(case list)</code> | ✅ 一致 |
| <code>Button.(default)</code> | ✅ 一致 |
| <code>Button.AccessKey=empty</code> | ✅ 一致 |
| <code>Button.AccessKey=japanese</code> | ✅ 一致 |
| <code>Button.AccessKey=k</code> | ✅ 一致 |
| <code>Button.AccessKey=markup</code> | ✅ 一致 |
| <code>Button.AccessKey=text</code> | ✅ 一致 |
| <code>Button.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>Button.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>Button.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>Button.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>Button.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>Button.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>Button.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>Button.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>Button.BackColor=#3366CC</code> | ✅ 一致 |
| <code>Button.BackColor=Red</code> | ✅ 一致 |
| <code>Button.BackColor=Transparent</code> | ✅ 一致 |
| <code>Button.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>Button.BorderColor=Red</code> | ✅ 一致 |
| <code>Button.BorderColor=Transparent</code> | ✅ 一致 |
| <code>Button.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>Button.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>Button.BorderStyle=Double</code> | ✅ 一致 |
| <code>Button.BorderStyle=Groove</code> | ✅ 一致 |
| <code>Button.BorderStyle=Inset</code> | ✅ 一致 |
| <code>Button.BorderStyle=None</code> | ✅ 一致 |
| <code>Button.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>Button.BorderStyle=Outset</code> | ✅ 一致 |
| <code>Button.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>Button.BorderStyle=Solid</code> | ✅ 一致 |
| <code>Button.BorderWidth=120px</code> | ✅ 一致 |
| <code>Button.BorderWidth=2em</code> | ✅ 一致 |
| <code>Button.BorderWidth=50%</code> | ✅ 一致 |
| <code>Button.CausesValidation=false</code> | ✅ 一致 |
| <code>Button.CausesValidation=true</code> | ✅ 一致 |
| <code>Button.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Button.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Button.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Button.ClientIDMode=Static</code> | ✅ 一致 |
| <code>Button.CommandArgument=empty</code> | ✅ 一致 |
| <code>Button.CommandArgument=japanese</code> | ✅ 一致 |
| <code>Button.CommandArgument=markup</code> | ✅ 一致 |
| <code>Button.CommandArgument=text</code> | ✅ 一致 |
| <code>Button.CommandName=empty</code> | ✅ 一致 |
| <code>Button.CommandName=japanese</code> | ✅ 一致 |
| <code>Button.CommandName=markup</code> | ✅ 一致 |
| <code>Button.CommandName=text</code> | ✅ 一致 |
| <code>Button.CommandName=text+CommandArgument=markup</code> | ✅ 一致 |
| <code>Button.CssClass=empty</code> | ✅ 一致 |
| <code>Button.CssClass=japanese</code> | ✅ 一致 |
| <code>Button.CssClass=markup</code> | ✅ 一致 |
| <code>Button.CssClass=text</code> | ✅ 一致 |
| <code>Button.EnableTheming=false</code> | ✅ 一致 |
| <code>Button.EnableTheming=true</code> | ✅ 一致 |
| <code>Button.EnableViewState=false</code> | ✅ 一致 |
| <code>Button.EnableViewState=true</code> | ✅ 一致 |
| <code>Button.Enabled=false</code> | ✅ 一致 |
| <code>Button.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>Button.Enabled=true</code> | ✅ 一致 |
| <code>Button.Font.Bold=false</code> | ✅ 一致 |
| <code>Button.Font.Bold=true</code> | ✅ 一致 |
| <code>Button.Font.Italic=false</code> | ✅ 一致 |
| <code>Button.Font.Italic=true</code> | ✅ 一致 |
| <code>Button.Font.Name=empty</code> | ✅ 一致 |
| <code>Button.Font.Name=japanese</code> | ✅ 一致 |
| <code>Button.Font.Name=markup</code> | ✅ 一致 |
| <code>Button.Font.Name=text</code> | ✅ 一致 |
| <code>Button.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>Button.Font.Overline=false</code> | ✅ 一致 |
| <code>Button.Font.Overline=true</code> | ✅ 一致 |
| <code>Button.Font.Size=12pt</code> | ✅ 一致 |
| <code>Button.Font.Size=150%</code> | ✅ 一致 |
| <code>Button.Font.Size=Large</code> | ✅ 一致 |
| <code>Button.Font.Strikeout=false</code> | ✅ 一致 |
| <code>Button.Font.Strikeout=true</code> | ✅ 一致 |
| <code>Button.Font.Underline=false</code> | ✅ 一致 |
| <code>Button.Font.Underline=true</code> | ✅ 一致 |
| <code>Button.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>Button.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>Button.ForeColor=Red</code> | ✅ 一致 |
| <code>Button.ForeColor=Transparent</code> | ✅ 一致 |
| <code>Button.Height=120px</code> | ✅ 一致 |
| <code>Button.Height=2em</code> | ✅ 一致 |
| <code>Button.Height=50%</code> | ✅ 一致 |
| <code>Button.ID=empty</code> | ✅ 一致 |
| <code>Button.ID=japanese</code> | ✅ 一致 |
| <code>Button.ID=markup</code> | ✅ 一致 |
| <code>Button.ID=text</code> | ✅ 一致 |
| <code>Button.OnClientClick=confirm</code> | ✅ 一致 |
| <code>Button.OnClientClick=confirm+CausesValidation=false</code> | ✅ 一致 |
| <code>Button.OnClientClick=confirm+Enabled=false</code> | ✅ 一致 |
| <code>Button.OnClientClick=confirm+ValidationGroup=text</code> | ✅ 一致 |
| <code>Button.OnClientClick=empty</code> | ✅ 一致 |
| <code>Button.OnClientClick=japanese</code> | ✅ 一致 |
| <code>Button.OnClientClick=markup</code> | ✅ 一致 |
| <code>Button.OnClientClick=text</code> | ✅ 一致 |
| <code>Button.PostBackUrl=absolute</code> | ✅ 一致 |
| <code>Button.PostBackUrl=app-relative</code> | ✅ 一致 |
| <code>Button.PostBackUrl=app-relative+CausesValidation=false</code> | ✅ 一致 |
| <code>Button.PostBackUrl=app-relative+Enabled=false</code> | ✅ 一致 |
| <code>Button.PostBackUrl=app-relative+OnClientClick=confirm</code> | ✅ 一致 |
| <code>Button.PostBackUrl=app-relative+ValidationGroup=text</code> | ✅ 一致 |
| <code>Button.PostBackUrl=empty</code> | ✅ 一致 |
| <code>Button.PostBackUrl=japanese</code> | ✅ 一致 |
| <code>Button.PostBackUrl=markup</code> | ✅ 一致 |
| <code>Button.PostBackUrl=text</code> | ✅ 一致 |
| <code>Button.SkinID=empty</code> | ✅ 一致 |
| <code>Button.SkinID=japanese</code> | ✅ 一致 |
| <code>Button.SkinID=markup</code> | ✅ 一致 |
| <code>Button.SkinID=text</code> | ✅ 一致 |
| <code>Button.Style[color]=1em</code> | ✅ 一致 |
| <code>Button.Style[color]=red</code> | ✅ 一致 |
| <code>Button.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>Button.Style[margin-left]=red</code> | ✅ 一致 |
| <code>Button.TabIndex=-1</code> | ✅ 一致 |
| <code>Button.TabIndex=0</code> | ✅ 一致 |
| <code>Button.TabIndex=3</code> | ✅ 一致 |
| <code>Button.Text=empty</code> | ✅ 一致 |
| <code>Button.Text=japanese</code> | ✅ 一致 |
| <code>Button.Text=markup</code> | ✅ 一致 |
| <code>Button.Text=markup+EnableViewState=false</code> | ✅ 一致 |
| <code>Button.Text=markup+ToolTip=markup</code> | ✅ 一致 |
| <code>Button.Text=text</code> | ✅ 一致 |
| <code>Button.Text=text+Visible=false</code> | ✅ 一致 |
| <code>Button.ToolTip=empty</code> | ✅ 一致 |
| <code>Button.ToolTip=japanese</code> | ✅ 一致 |
| <code>Button.ToolTip=markup</code> | ✅ 一致 |
| <code>Button.ToolTip=text</code> | ✅ 一致 |
| <code>Button.UseSubmitBehavior=false</code> | ✅ 一致 |
| <code>Button.UseSubmitBehavior=false+CausesValidation=false</code> | ✅ 一致 |
| <code>Button.UseSubmitBehavior=false+Enabled=false</code> | ✅ 一致 |
| <code>Button.UseSubmitBehavior=false+OnClientClick=confirm</code> | ✅ 一致 |
| <code>Button.UseSubmitBehavior=false+PostBackUrl=app-relative</code> | ✅ 一致 |
| <code>Button.UseSubmitBehavior=false+Text=text</code> | ✅ 一致 |
| <code>Button.UseSubmitBehavior=false+ValidationGroup=text</code> | ✅ 一致 |
| <code>Button.UseSubmitBehavior=false+Visible=false</code> | ✅ 一致 |
| <code>Button.UseSubmitBehavior=true</code> | ✅ 一致 |
| <code>Button.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>Button.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>Button.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>Button.ValidationGroup=empty</code> | ✅ 一致 |
| <code>Button.ValidationGroup=japanese</code> | ✅ 一致 |
| <code>Button.ValidationGroup=markup</code> | ✅ 一致 |
| <code>Button.ValidationGroup=text</code> | ✅ 一致 |
| <code>Button.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>Button.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>Button.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>Button.Visible=false</code> | ✅ 一致 |
| <code>Button.Visible=true</code> | ✅ 一致 |
| <code>Button.Width=120px</code> | ✅ 一致 |
| <code>Button.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>Button.Width=2em</code> | ✅ 一致 |
| <code>Button.Width=50%</code> | ✅ 一致 |
| <code>Button[naming container].(default)</code> | ✅ 一致 |
| <code>Button[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Button[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Button[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Button[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>Button[naming container].ID=text</code> | ✅ 一致 |
| <code>Button[naming container].PostBackUrl=app-relative</code> | ✅ 一致 |
| <code>Button[naming container].UseSubmitBehavior=false</code> | ✅ 一致 |
| <code>Button[naming container].ValidationGroup=text</code> | ✅ 一致 |
| <code>Button[themed].(default)</code> | ✅ 一致 |
| <code>Button[themed].CssClass=text</code> | ✅ 一致 |
| <code>Button[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>Button[themed].SkinID=text</code> | ✅ 一致 |
| <code>Button[themed].Width=120px</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| Button [render][databind][disabled] first request | ✅ 一致 |
| Button [postbackevent][command][viewstate] submit button: Click, then Command | ✅ 一致 |
| Button [postbackevent][submitbehavior] script postback (UseSubmitBehavior=false) | ✅ 一致 |
| Button [clientclick] OnClientClick returns false: no request | ✅ 一致 |
| Button [clientclick][postbackevent] OnClientClick returns true | ✅ 一致 |
| Button [causesvalidation] group g1, empty: invalid, Click raised | ✅ 一致 |
| Button [causesvalidation] group g1, filled: valid | ✅ 一致 |
| Button [causesvalidation] CausesValidation=false: not validated | ✅ 一致 |
| Button [causesvalidation] group g2 validated in the browser: no request | ✅ 一致 |
| Button [command][databind] Command bubbled from a repeater item | ✅ 一致 |
| Button [defaultbutton] Enter in a panel with DefaultButton | ✅ 一致 |
| Button [dynamic][postbackevent] dynamically created button | ✅ 一致 |
| Button [visible][viewstate] hidden button shown, its text changed | ✅ 一致 |
| Button [visible][viewstate][postbackevent] the button shown before raises Click, its text kept | ✅ 一致 |
| Button [crosspage] cross-page postback (PostBackUrl) | ✅ 一致 |

</details>
