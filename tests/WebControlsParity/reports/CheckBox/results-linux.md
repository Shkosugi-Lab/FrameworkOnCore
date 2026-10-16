# CheckBox パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 01:33
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 167 | 167 | 0 | 0 |
| ② HTML | 147 | 147 | 0 | 0 |
| ③ シナリオ | 9 | 9 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 176 件中 176 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 35 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 9 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 133 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 2 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 8 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 4 | 1 | ✅ |
| ポストバックデータの取り込み | 0 | 1 | ✅ |
| 変更イベント（*Changed、ServerChange） | 0 | 4 | ✅ |
| リクエスト検証（ValidateRequestMode） | 3 | 1 | ✅ |
| AutoPostBack（クライアントスクリプトによるポストバック） | 5 | 2 | ✅ |
| CausesValidation / ValidationGroup | 8 | 1 | ✅ |
| 双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値） | 0 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>CheckBox.Api.Control.#ctor</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.Adapter</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.ClientID</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.Context</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.Controls</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.DataBind</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.Dispose</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.Disposed</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.Events</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.FindControl</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.Focus</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.HasControls</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.ID</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.Init</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.Load</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.OnInit</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.Page</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.Parent</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.PreRender</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.Render</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.Site</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.SkinID</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.Unload</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.ViewState</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>CheckBox.Api.Control.Visible</code> | ✅ 一致 |
| <code>CheckBox.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>CheckBox.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>CheckBox.Api.ICheckBoxControl.Checked</code> | ✅ 一致 |
| <code>CheckBox.Api.ICheckBoxControl.CheckedChanged</code> | ✅ 一致 |
| <code>CheckBox.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>CheckBox.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>CheckBox.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>CheckBox.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>CheckBox.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>CheckBox.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>CheckBox.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>CheckBox.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>CheckBox.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>CheckBox.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>CheckBox.Api.IPostBackDataHandler.LoadPostData</code> | ✅ 一致 |
| <code>CheckBox.Api.IPostBackDataHandler.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>CheckBox.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.CheckBox.#ctor</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.CheckBox.AddAttributesToRender</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.CheckBox.AutoPostBack</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.CheckBox.CausesValidation</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.CheckBox.Checked</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.CheckBox.CheckedChanged</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.CheckBox.InputAttributes</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.CheckBox.LabelAttributes</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.CheckBox.LoadPostData</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.CheckBox.LoadViewState</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.CheckBox.OnCheckedChanged</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.CheckBox.OnPreRender</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.CheckBox.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.CheckBox.Render</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.CheckBox.SaveViewState</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.CheckBox.Text</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.CheckBox.TextAlign</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.CheckBox.TrackViewState</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.CheckBox.ValidationGroup</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>CheckBox.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>CheckBox.(case list)</code> | ✅ 一致 |
| <code>CheckBox.(default)</code> | ✅ 一致 |
| <code>CheckBox.AccessKey=empty</code> | ✅ 一致 |
| <code>CheckBox.AccessKey=japanese</code> | ✅ 一致 |
| <code>CheckBox.AccessKey=k</code> | ✅ 一致 |
| <code>CheckBox.AccessKey=markup</code> | ✅ 一致 |
| <code>CheckBox.AccessKey=text</code> | ✅ 一致 |
| <code>CheckBox.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>CheckBox.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>CheckBox.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>CheckBox.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>CheckBox.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>CheckBox.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>CheckBox.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>CheckBox.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>CheckBox.AutoPostBack=false</code> | ✅ 一致 |
| <code>CheckBox.AutoPostBack=true</code> | ✅ 一致 |
| <code>CheckBox.AutoPostBack=true+CausesValidation=true</code> | ✅ 一致 |
| <code>CheckBox.AutoPostBack=true+CausesValidation=true+ValidationGroup=text</code> | ✅ 一致 |
| <code>CheckBox.AutoPostBack=true+Enabled=false</code> | ✅ 一致 |
| <code>CheckBox.BackColor=#3366CC</code> | ✅ 一致 |
| <code>CheckBox.BackColor=Red</code> | ✅ 一致 |
| <code>CheckBox.BackColor=Transparent</code> | ✅ 一致 |
| <code>CheckBox.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>CheckBox.BorderColor=Red</code> | ✅ 一致 |
| <code>CheckBox.BorderColor=Transparent</code> | ✅ 一致 |
| <code>CheckBox.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>CheckBox.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>CheckBox.BorderStyle=Double</code> | ✅ 一致 |
| <code>CheckBox.BorderStyle=Groove</code> | ✅ 一致 |
| <code>CheckBox.BorderStyle=Inset</code> | ✅ 一致 |
| <code>CheckBox.BorderStyle=None</code> | ✅ 一致 |
| <code>CheckBox.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>CheckBox.BorderStyle=Outset</code> | ✅ 一致 |
| <code>CheckBox.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>CheckBox.BorderStyle=Solid</code> | ✅ 一致 |
| <code>CheckBox.BorderWidth=120px</code> | ✅ 一致 |
| <code>CheckBox.BorderWidth=2em</code> | ✅ 一致 |
| <code>CheckBox.BorderWidth=50%</code> | ✅ 一致 |
| <code>CheckBox.CausesValidation=false</code> | ✅ 一致 |
| <code>CheckBox.CausesValidation=true</code> | ✅ 一致 |
| <code>CheckBox.Checked=false</code> | ✅ 一致 |
| <code>CheckBox.Checked=true</code> | ✅ 一致 |
| <code>CheckBox.Checked=true+EnableViewState=false</code> | ✅ 一致 |
| <code>CheckBox.Checked=true+Enabled=false</code> | ✅ 一致 |
| <code>CheckBox.Checked=true+Visible=false</code> | ✅ 一致 |
| <code>CheckBox.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>CheckBox.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>CheckBox.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>CheckBox.ClientIDMode=Static</code> | ✅ 一致 |
| <code>CheckBox.CssClass=empty</code> | ✅ 一致 |
| <code>CheckBox.CssClass=japanese</code> | ✅ 一致 |
| <code>CheckBox.CssClass=markup</code> | ✅ 一致 |
| <code>CheckBox.CssClass=text</code> | ✅ 一致 |
| <code>CheckBox.EnableTheming=false</code> | ✅ 一致 |
| <code>CheckBox.EnableTheming=true</code> | ✅ 一致 |
| <code>CheckBox.EnableViewState=false</code> | ✅ 一致 |
| <code>CheckBox.EnableViewState=true</code> | ✅ 一致 |
| <code>CheckBox.Enabled=false</code> | ✅ 一致 |
| <code>CheckBox.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>CheckBox.Enabled=true</code> | ✅ 一致 |
| <code>CheckBox.Font.Bold=false</code> | ✅ 一致 |
| <code>CheckBox.Font.Bold=true</code> | ✅ 一致 |
| <code>CheckBox.Font.Italic=false</code> | ✅ 一致 |
| <code>CheckBox.Font.Italic=true</code> | ✅ 一致 |
| <code>CheckBox.Font.Name=empty</code> | ✅ 一致 |
| <code>CheckBox.Font.Name=japanese</code> | ✅ 一致 |
| <code>CheckBox.Font.Name=markup</code> | ✅ 一致 |
| <code>CheckBox.Font.Name=text</code> | ✅ 一致 |
| <code>CheckBox.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>CheckBox.Font.Overline=false</code> | ✅ 一致 |
| <code>CheckBox.Font.Overline=true</code> | ✅ 一致 |
| <code>CheckBox.Font.Size=12pt</code> | ✅ 一致 |
| <code>CheckBox.Font.Size=150%</code> | ✅ 一致 |
| <code>CheckBox.Font.Size=Large</code> | ✅ 一致 |
| <code>CheckBox.Font.Strikeout=false</code> | ✅ 一致 |
| <code>CheckBox.Font.Strikeout=true</code> | ✅ 一致 |
| <code>CheckBox.Font.Underline=false</code> | ✅ 一致 |
| <code>CheckBox.Font.Underline=true</code> | ✅ 一致 |
| <code>CheckBox.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>CheckBox.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>CheckBox.ForeColor=Red</code> | ✅ 一致 |
| <code>CheckBox.ForeColor=Transparent</code> | ✅ 一致 |
| <code>CheckBox.Height=120px</code> | ✅ 一致 |
| <code>CheckBox.Height=2em</code> | ✅ 一致 |
| <code>CheckBox.Height=50%</code> | ✅ 一致 |
| <code>CheckBox.ID=empty</code> | ✅ 一致 |
| <code>CheckBox.ID=japanese</code> | ✅ 一致 |
| <code>CheckBox.ID=markup</code> | ✅ 一致 |
| <code>CheckBox.ID=text</code> | ✅ 一致 |
| <code>CheckBox.InputAttributes[data-probe]=text</code> | ✅ 一致 |
| <code>CheckBox.InputAttributes[data-probe]=text+Text=text</code> | ✅ 一致 |
| <code>CheckBox.LabelAttributes[data-probe]=text</code> | ✅ 一致 |
| <code>CheckBox.LabelAttributes[data-probe]=text+Text=text</code> | ✅ 一致 |
| <code>CheckBox.SkinID=empty</code> | ✅ 一致 |
| <code>CheckBox.SkinID=japanese</code> | ✅ 一致 |
| <code>CheckBox.SkinID=markup</code> | ✅ 一致 |
| <code>CheckBox.SkinID=text</code> | ✅ 一致 |
| <code>CheckBox.Style[color]=1em</code> | ✅ 一致 |
| <code>CheckBox.Style[color]=red</code> | ✅ 一致 |
| <code>CheckBox.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>CheckBox.Style[margin-left]=red</code> | ✅ 一致 |
| <code>CheckBox.TabIndex=-1</code> | ✅ 一致 |
| <code>CheckBox.TabIndex=0</code> | ✅ 一致 |
| <code>CheckBox.TabIndex=3</code> | ✅ 一致 |
| <code>CheckBox.Text=empty</code> | ✅ 一致 |
| <code>CheckBox.Text=japanese</code> | ✅ 一致 |
| <code>CheckBox.Text=markup</code> | ✅ 一致 |
| <code>CheckBox.Text=markup+Checked=true</code> | ✅ 一致 |
| <code>CheckBox.Text=text</code> | ✅ 一致 |
| <code>CheckBox.Text=text+CssClass=text</code> | ✅ 一致 |
| <code>CheckBox.Text=text+TextAlign=Left</code> | ✅ 一致 |
| <code>CheckBox.Text=text+ToolTip=text</code> | ✅ 一致 |
| <code>CheckBox.TextAlign=Left</code> | ✅ 一致 |
| <code>CheckBox.TextAlign=Right</code> | ✅ 一致 |
| <code>CheckBox.ToolTip=empty</code> | ✅ 一致 |
| <code>CheckBox.ToolTip=japanese</code> | ✅ 一致 |
| <code>CheckBox.ToolTip=markup</code> | ✅ 一致 |
| <code>CheckBox.ToolTip=text</code> | ✅ 一致 |
| <code>CheckBox.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>CheckBox.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>CheckBox.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>CheckBox.ValidationGroup=empty</code> | ✅ 一致 |
| <code>CheckBox.ValidationGroup=japanese</code> | ✅ 一致 |
| <code>CheckBox.ValidationGroup=markup</code> | ✅ 一致 |
| <code>CheckBox.ValidationGroup=text</code> | ✅ 一致 |
| <code>CheckBox.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>CheckBox.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>CheckBox.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>CheckBox.Visible=false</code> | ✅ 一致 |
| <code>CheckBox.Visible=true</code> | ✅ 一致 |
| <code>CheckBox.Width=120px</code> | ✅ 一致 |
| <code>CheckBox.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>CheckBox.Width=2em</code> | ✅ 一致 |
| <code>CheckBox.Width=50%</code> | ✅ 一致 |
| <code>CheckBox[naming container].(default)</code> | ✅ 一致 |
| <code>CheckBox[naming container].AutoPostBack=true</code> | ✅ 一致 |
| <code>CheckBox[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>CheckBox[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>CheckBox[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>CheckBox[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>CheckBox[naming container].ID=text</code> | ✅ 一致 |
| <code>CheckBox[naming container].Text=text</code> | ✅ 一致 |
| <code>CheckBox[themed].(default)</code> | ✅ 一致 |
| <code>CheckBox[themed].CssClass=text</code> | ✅ 一致 |
| <code>CheckBox[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>CheckBox[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| CheckBox [render][databind][dynamic] first request | ✅ 一致 |
| CheckBox [postdata][change][dynamic][disabled] boxes checked; a disabled box&#39;s post | ✅ 一致 |
| CheckBox [viewstate][change] posted again unchanged | ✅ 一致 |
| CheckBox [change] a box unchecked | ✅ 一致 |
| CheckBox [autopostback][change] auto postback | ✅ 一致 |
| CheckBox [autopostback][causesvalidation] auto postback validating g1 (empty) | ✅ 一致 |
| CheckBox [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| CheckBox [bind] two-way binding in a template | ✅ 一致 |
| CheckBox [requestvalidation] markup posted | ✅ 一致 |

</details>
