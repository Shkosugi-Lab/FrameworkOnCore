# RadioButton パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 02:57
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 172 | 172 | 0 | 0 |
| ② HTML | 154 | 154 | 0 | 0 |
| ③ シナリオ | 9 | 9 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 181 件中 181 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 36 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 9 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 139 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 2 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 9 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 4 | 0 | ✅ |
| ポストバックデータの取り込み | 0 | 1 | ✅ |
| 変更イベント（*Changed、ServerChange） | 0 | 4 | ✅ |
| リクエスト検証（ValidateRequestMode） | 3 | 1 | ✅ |
| AutoPostBack（クライアントスクリプトによるポストバック） | 6 | 2 | ✅ |
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
| <code>RadioButton.Api.Control.#ctor</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.Adapter</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.ClientID</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.Context</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.Controls</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.DataBind</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.Dispose</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.Disposed</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.Events</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.FindControl</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.Focus</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.HasControls</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.ID</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.Init</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.Load</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.OnInit</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.Page</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.Parent</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.PreRender</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.Render</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.Site</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.SkinID</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.Unload</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.ViewState</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>RadioButton.Api.Control.Visible</code> | ✅ 一致 |
| <code>RadioButton.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>RadioButton.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>RadioButton.Api.ICheckBoxControl.Checked</code> | ✅ 一致 |
| <code>RadioButton.Api.ICheckBoxControl.CheckedChanged</code> | ✅ 一致 |
| <code>RadioButton.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>RadioButton.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>RadioButton.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>RadioButton.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>RadioButton.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>RadioButton.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>RadioButton.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>RadioButton.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>RadioButton.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>RadioButton.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>RadioButton.Api.IPostBackDataHandler.LoadPostData</code> | ✅ 一致 |
| <code>RadioButton.Api.IPostBackDataHandler.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>RadioButton.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.CheckBox.#ctor</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.CheckBox.AddAttributesToRender</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.CheckBox.AutoPostBack</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.CheckBox.CausesValidation</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.CheckBox.Checked</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.CheckBox.CheckedChanged</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.CheckBox.InputAttributes</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.CheckBox.LabelAttributes</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.CheckBox.LoadPostData</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.CheckBox.LoadViewState</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.CheckBox.OnCheckedChanged</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.CheckBox.OnPreRender</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.CheckBox.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.CheckBox.Render</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.CheckBox.SaveViewState</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.CheckBox.Text</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.CheckBox.TextAlign</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.CheckBox.TrackViewState</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.CheckBox.ValidationGroup</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.RadioButton.#ctor</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.RadioButton.GroupName</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.RadioButton.LoadPostData</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.RadioButton.OnPreRender</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.RadioButton.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>RadioButton.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>RadioButton.(case list)</code> | ✅ 一致 |
| <code>RadioButton.(default)</code> | ✅ 一致 |
| <code>RadioButton.AccessKey=empty</code> | ✅ 一致 |
| <code>RadioButton.AccessKey=japanese</code> | ✅ 一致 |
| <code>RadioButton.AccessKey=k</code> | ✅ 一致 |
| <code>RadioButton.AccessKey=markup</code> | ✅ 一致 |
| <code>RadioButton.AccessKey=text</code> | ✅ 一致 |
| <code>RadioButton.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>RadioButton.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>RadioButton.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>RadioButton.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>RadioButton.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>RadioButton.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>RadioButton.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>RadioButton.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>RadioButton.AutoPostBack=false</code> | ✅ 一致 |
| <code>RadioButton.AutoPostBack=true</code> | ✅ 一致 |
| <code>RadioButton.AutoPostBack=true+CausesValidation=true</code> | ✅ 一致 |
| <code>RadioButton.AutoPostBack=true+CausesValidation=true+ValidationGroup=text</code> | ✅ 一致 |
| <code>RadioButton.AutoPostBack=true+Enabled=false</code> | ✅ 一致 |
| <code>RadioButton.BackColor=#3366CC</code> | ✅ 一致 |
| <code>RadioButton.BackColor=Red</code> | ✅ 一致 |
| <code>RadioButton.BackColor=Transparent</code> | ✅ 一致 |
| <code>RadioButton.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>RadioButton.BorderColor=Red</code> | ✅ 一致 |
| <code>RadioButton.BorderColor=Transparent</code> | ✅ 一致 |
| <code>RadioButton.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>RadioButton.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>RadioButton.BorderStyle=Double</code> | ✅ 一致 |
| <code>RadioButton.BorderStyle=Groove</code> | ✅ 一致 |
| <code>RadioButton.BorderStyle=Inset</code> | ✅ 一致 |
| <code>RadioButton.BorderStyle=None</code> | ✅ 一致 |
| <code>RadioButton.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>RadioButton.BorderStyle=Outset</code> | ✅ 一致 |
| <code>RadioButton.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>RadioButton.BorderStyle=Solid</code> | ✅ 一致 |
| <code>RadioButton.BorderWidth=120px</code> | ✅ 一致 |
| <code>RadioButton.BorderWidth=2em</code> | ✅ 一致 |
| <code>RadioButton.BorderWidth=50%</code> | ✅ 一致 |
| <code>RadioButton.CausesValidation=false</code> | ✅ 一致 |
| <code>RadioButton.CausesValidation=true</code> | ✅ 一致 |
| <code>RadioButton.Checked=false</code> | ✅ 一致 |
| <code>RadioButton.Checked=true</code> | ✅ 一致 |
| <code>RadioButton.Checked=true+EnableViewState=false</code> | ✅ 一致 |
| <code>RadioButton.Checked=true+Enabled=false</code> | ✅ 一致 |
| <code>RadioButton.Checked=true+Visible=false</code> | ✅ 一致 |
| <code>RadioButton.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>RadioButton.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>RadioButton.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>RadioButton.ClientIDMode=Static</code> | ✅ 一致 |
| <code>RadioButton.CssClass=empty</code> | ✅ 一致 |
| <code>RadioButton.CssClass=japanese</code> | ✅ 一致 |
| <code>RadioButton.CssClass=markup</code> | ✅ 一致 |
| <code>RadioButton.CssClass=text</code> | ✅ 一致 |
| <code>RadioButton.EnableTheming=false</code> | ✅ 一致 |
| <code>RadioButton.EnableTheming=true</code> | ✅ 一致 |
| <code>RadioButton.EnableViewState=false</code> | ✅ 一致 |
| <code>RadioButton.EnableViewState=true</code> | ✅ 一致 |
| <code>RadioButton.Enabled=false</code> | ✅ 一致 |
| <code>RadioButton.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>RadioButton.Enabled=true</code> | ✅ 一致 |
| <code>RadioButton.Font.Bold=false</code> | ✅ 一致 |
| <code>RadioButton.Font.Bold=true</code> | ✅ 一致 |
| <code>RadioButton.Font.Italic=false</code> | ✅ 一致 |
| <code>RadioButton.Font.Italic=true</code> | ✅ 一致 |
| <code>RadioButton.Font.Name=empty</code> | ✅ 一致 |
| <code>RadioButton.Font.Name=japanese</code> | ✅ 一致 |
| <code>RadioButton.Font.Name=markup</code> | ✅ 一致 |
| <code>RadioButton.Font.Name=text</code> | ✅ 一致 |
| <code>RadioButton.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>RadioButton.Font.Overline=false</code> | ✅ 一致 |
| <code>RadioButton.Font.Overline=true</code> | ✅ 一致 |
| <code>RadioButton.Font.Size=12pt</code> | ✅ 一致 |
| <code>RadioButton.Font.Size=150%</code> | ✅ 一致 |
| <code>RadioButton.Font.Size=Large</code> | ✅ 一致 |
| <code>RadioButton.Font.Strikeout=false</code> | ✅ 一致 |
| <code>RadioButton.Font.Strikeout=true</code> | ✅ 一致 |
| <code>RadioButton.Font.Underline=false</code> | ✅ 一致 |
| <code>RadioButton.Font.Underline=true</code> | ✅ 一致 |
| <code>RadioButton.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>RadioButton.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>RadioButton.ForeColor=Red</code> | ✅ 一致 |
| <code>RadioButton.ForeColor=Transparent</code> | ✅ 一致 |
| <code>RadioButton.GroupName=empty</code> | ✅ 一致 |
| <code>RadioButton.GroupName=japanese</code> | ✅ 一致 |
| <code>RadioButton.GroupName=markup</code> | ✅ 一致 |
| <code>RadioButton.GroupName=text</code> | ✅ 一致 |
| <code>RadioButton.GroupName=text+AutoPostBack=true</code> | ✅ 一致 |
| <code>RadioButton.GroupName=text+Checked=true</code> | ✅ 一致 |
| <code>RadioButton.Height=120px</code> | ✅ 一致 |
| <code>RadioButton.Height=2em</code> | ✅ 一致 |
| <code>RadioButton.Height=50%</code> | ✅ 一致 |
| <code>RadioButton.ID=empty</code> | ✅ 一致 |
| <code>RadioButton.ID=japanese</code> | ✅ 一致 |
| <code>RadioButton.ID=markup</code> | ✅ 一致 |
| <code>RadioButton.ID=text</code> | ✅ 一致 |
| <code>RadioButton.InputAttributes[data-probe]=text</code> | ✅ 一致 |
| <code>RadioButton.InputAttributes[data-probe]=text+Text=text</code> | ✅ 一致 |
| <code>RadioButton.LabelAttributes[data-probe]=text</code> | ✅ 一致 |
| <code>RadioButton.LabelAttributes[data-probe]=text+Text=text</code> | ✅ 一致 |
| <code>RadioButton.SkinID=empty</code> | ✅ 一致 |
| <code>RadioButton.SkinID=japanese</code> | ✅ 一致 |
| <code>RadioButton.SkinID=markup</code> | ✅ 一致 |
| <code>RadioButton.SkinID=text</code> | ✅ 一致 |
| <code>RadioButton.Style[color]=1em</code> | ✅ 一致 |
| <code>RadioButton.Style[color]=red</code> | ✅ 一致 |
| <code>RadioButton.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>RadioButton.Style[margin-left]=red</code> | ✅ 一致 |
| <code>RadioButton.TabIndex=-1</code> | ✅ 一致 |
| <code>RadioButton.TabIndex=0</code> | ✅ 一致 |
| <code>RadioButton.TabIndex=3</code> | ✅ 一致 |
| <code>RadioButton.Text=empty</code> | ✅ 一致 |
| <code>RadioButton.Text=japanese</code> | ✅ 一致 |
| <code>RadioButton.Text=markup</code> | ✅ 一致 |
| <code>RadioButton.Text=markup+Checked=true</code> | ✅ 一致 |
| <code>RadioButton.Text=text</code> | ✅ 一致 |
| <code>RadioButton.Text=text+CssClass=text</code> | ✅ 一致 |
| <code>RadioButton.Text=text+TextAlign=Left</code> | ✅ 一致 |
| <code>RadioButton.Text=text+ToolTip=text</code> | ✅ 一致 |
| <code>RadioButton.TextAlign=Left</code> | ✅ 一致 |
| <code>RadioButton.TextAlign=Right</code> | ✅ 一致 |
| <code>RadioButton.ToolTip=empty</code> | ✅ 一致 |
| <code>RadioButton.ToolTip=japanese</code> | ✅ 一致 |
| <code>RadioButton.ToolTip=markup</code> | ✅ 一致 |
| <code>RadioButton.ToolTip=text</code> | ✅ 一致 |
| <code>RadioButton.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>RadioButton.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>RadioButton.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>RadioButton.ValidationGroup=empty</code> | ✅ 一致 |
| <code>RadioButton.ValidationGroup=japanese</code> | ✅ 一致 |
| <code>RadioButton.ValidationGroup=markup</code> | ✅ 一致 |
| <code>RadioButton.ValidationGroup=text</code> | ✅ 一致 |
| <code>RadioButton.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>RadioButton.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>RadioButton.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>RadioButton.Visible=false</code> | ✅ 一致 |
| <code>RadioButton.Visible=true</code> | ✅ 一致 |
| <code>RadioButton.Width=120px</code> | ✅ 一致 |
| <code>RadioButton.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>RadioButton.Width=2em</code> | ✅ 一致 |
| <code>RadioButton.Width=50%</code> | ✅ 一致 |
| <code>RadioButton[naming container].(default)</code> | ✅ 一致 |
| <code>RadioButton[naming container].AutoPostBack=true</code> | ✅ 一致 |
| <code>RadioButton[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>RadioButton[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>RadioButton[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>RadioButton[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>RadioButton[naming container].GroupName=text</code> | ✅ 一致 |
| <code>RadioButton[naming container].ID=text</code> | ✅ 一致 |
| <code>RadioButton[naming container].Text=text</code> | ✅ 一致 |
| <code>RadioButton[themed].(default)</code> | ✅ 一致 |
| <code>RadioButton[themed].CssClass=text</code> | ✅ 一致 |
| <code>RadioButton[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>RadioButton[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| RadioButton [render][databind][dynamic] first request | ✅ 一致 |
| RadioButton [postdata][change][dynamic] radios chosen | ✅ 一致 |
| RadioButton [viewstate][change] posted again unchanged | ✅ 一致 |
| RadioButton [change] the other radio of the group chosen | ✅ 一致 |
| RadioButton [autopostback][change] auto postback | ✅ 一致 |
| RadioButton [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| RadioButton [bind] two-way binding in a template | ✅ 一致 |
| RadioButton [requestvalidation] markup posted | ✅ 一致 |
| RadioButton [autopostback][causesvalidation] auto postback validating g1 (empty) | ✅ 一致 |

</details>
