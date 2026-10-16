# CustomValidator パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-11 23:27
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 195 | 195 | 0 | 0 |
| ② HTML | 170 | 170 | 0 | 0 |
| ③ シナリオ | 7 | 7 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 204 件中 204 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 40 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 7 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 156 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 0 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 8 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 3 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 3 | 0 | ✅ |
| サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ） | 0 | 4 | ✅ |
| ブラウザ側の検証スクリプト（EnableClientScript） | 6 | 2 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>CustomValidator.Api.Control.#ctor</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.Adapter</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.ClientID</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.Context</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.Controls</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.DataBind</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.Dispose</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.Disposed</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.Events</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.FindControl</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.Focus</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.HasControls</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.ID</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.Init</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.Load</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.OnInit</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.Page</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.Parent</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.PreRender</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.Render</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.Site</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.SkinID</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.Unload</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.ViewState</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>CustomValidator.Api.Control.Visible</code> | ✅ 一致 |
| <code>CustomValidator.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>CustomValidator.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>CustomValidator.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>CustomValidator.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>CustomValidator.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>CustomValidator.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>CustomValidator.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>CustomValidator.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>CustomValidator.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>CustomValidator.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>CustomValidator.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>CustomValidator.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>CustomValidator.Api.ITextControl.Text</code> | ✅ 一致 |
| <code>CustomValidator.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>CustomValidator.Api.IValidator.ErrorMessage</code> | ✅ 一致 |
| <code>CustomValidator.Api.IValidator.IsValid</code> | ✅ 一致 |
| <code>CustomValidator.Api.IValidator.Validate</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.#ctor</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.AddAttributesToRender</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.AssociatedControlID</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.CheckControlValidationProperty</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.ControlPropertiesValid</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.ControlToValidate</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.DetermineRenderUplevel</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.Display</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.EnableClientScript</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.Enabled</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.ErrorMessage</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.EvaluateIsValid</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.ForeColor</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.GetControlRenderID</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.GetControlValidationValue</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.GetValidationProperty</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.IsUnobtrusive</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.IsValid</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.OnInit</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.OnPreRender</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.OnUnload</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.PropertiesValid</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.RegisterValidatorCommonScript</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.RegisterValidatorDeclaration</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.Render</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.RenderUplevel</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.SetFocusOnError</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.Text</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.Validate</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.BaseValidator.ValidationGroup</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.CustomValidator.#ctor</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.CustomValidator.AddAttributesToRender</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.CustomValidator.ClientValidationFunction</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.CustomValidator.ControlPropertiesValid</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.CustomValidator.EvaluateIsValid</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.CustomValidator.OnServerValidate</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.CustomValidator.ServerValidate</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.CustomValidator.ValidateEmptyText</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.Label.#ctor</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.Label.AddAttributesToRender</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.Label.AddParsedSubObject</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.Label.AssociatedControlID</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.Label.LoadViewState</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.Label.RenderContents</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.Label.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.Label.TagKey</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.Label.Text</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>CustomValidator.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>CustomValidator.(case list)</code> | ✅ 一致 |
| <code>CustomValidator.(default)</code> | ✅ 一致 |
| <code>CustomValidator.AccessKey=empty</code> | ✅ 一致 |
| <code>CustomValidator.AccessKey=japanese</code> | ✅ 一致 |
| <code>CustomValidator.AccessKey=k</code> | ✅ 一致 |
| <code>CustomValidator.AccessKey=markup</code> | ✅ 一致 |
| <code>CustomValidator.AccessKey=text</code> | ✅ 一致 |
| <code>CustomValidator.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>CustomValidator.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>CustomValidator.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>CustomValidator.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>CustomValidator.AssociatedControlID=empty</code> | ✅ 一致 |
| <code>CustomValidator.AssociatedControlID=japanese</code> | ✅ 一致 |
| <code>CustomValidator.AssociatedControlID=markup</code> | ✅ 一致 |
| <code>CustomValidator.AssociatedControlID=text</code> | ✅ 一致 |
| <code>CustomValidator.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>CustomValidator.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>CustomValidator.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>CustomValidator.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>CustomValidator.BackColor=#3366CC</code> | ✅ 一致 |
| <code>CustomValidator.BackColor=Red</code> | ✅ 一致 |
| <code>CustomValidator.BackColor=Transparent</code> | ✅ 一致 |
| <code>CustomValidator.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>CustomValidator.BorderColor=Red</code> | ✅ 一致 |
| <code>CustomValidator.BorderColor=Transparent</code> | ✅ 一致 |
| <code>CustomValidator.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>CustomValidator.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>CustomValidator.BorderStyle=Double</code> | ✅ 一致 |
| <code>CustomValidator.BorderStyle=Groove</code> | ✅ 一致 |
| <code>CustomValidator.BorderStyle=Inset</code> | ✅ 一致 |
| <code>CustomValidator.BorderStyle=None</code> | ✅ 一致 |
| <code>CustomValidator.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>CustomValidator.BorderStyle=Outset</code> | ✅ 一致 |
| <code>CustomValidator.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>CustomValidator.BorderStyle=Solid</code> | ✅ 一致 |
| <code>CustomValidator.BorderWidth=120px</code> | ✅ 一致 |
| <code>CustomValidator.BorderWidth=2em</code> | ✅ 一致 |
| <code>CustomValidator.BorderWidth=50%</code> | ✅ 一致 |
| <code>CustomValidator.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>CustomValidator.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>CustomValidator.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>CustomValidator.ClientIDMode=Static</code> | ✅ 一致 |
| <code>CustomValidator.ClientValidationFunction=checkOk</code> | ✅ 一致 |
| <code>CustomValidator.ClientValidationFunction=checkOk+ValidateEmptyText=true</code> | ✅ 一致 |
| <code>CustomValidator.ClientValidationFunction=empty</code> | ✅ 一致 |
| <code>CustomValidator.ClientValidationFunction=japanese</code> | ✅ 一致 |
| <code>CustomValidator.ClientValidationFunction=markup</code> | ✅ 一致 |
| <code>CustomValidator.ClientValidationFunction=text</code> | ✅ 一致 |
| <code>CustomValidator.ControlToValidate=empty</code> | ✅ 一致 |
| <code>CustomValidator.ControlToValidate=empty+ClientValidationFunction=checkOk</code> | ✅ 一致 |
| <code>CustomValidator.ControlToValidate=japanese</code> | ✅ 一致 |
| <code>CustomValidator.ControlToValidate=markup</code> | ✅ 一致 |
| <code>CustomValidator.ControlToValidate=other</code> | ✅ 一致 |
| <code>CustomValidator.ControlToValidate=target</code> | ✅ 一致 |
| <code>CustomValidator.ControlToValidate=text</code> | ✅ 一致 |
| <code>CustomValidator.CssClass=empty</code> | ✅ 一致 |
| <code>CustomValidator.CssClass=japanese</code> | ✅ 一致 |
| <code>CustomValidator.CssClass=markup</code> | ✅ 一致 |
| <code>CustomValidator.CssClass=text</code> | ✅ 一致 |
| <code>CustomValidator.Display=Dynamic</code> | ✅ 一致 |
| <code>CustomValidator.Display=None</code> | ✅ 一致 |
| <code>CustomValidator.Display=Static</code> | ✅ 一致 |
| <code>CustomValidator.EnableClientScript=false</code> | ✅ 一致 |
| <code>CustomValidator.EnableClientScript=false+Display=Dynamic</code> | ✅ 一致 |
| <code>CustomValidator.EnableClientScript=true</code> | ✅ 一致 |
| <code>CustomValidator.EnableTheming=false</code> | ✅ 一致 |
| <code>CustomValidator.EnableTheming=true</code> | ✅ 一致 |
| <code>CustomValidator.EnableViewState=false</code> | ✅ 一致 |
| <code>CustomValidator.EnableViewState=true</code> | ✅ 一致 |
| <code>CustomValidator.Enabled=false</code> | ✅ 一致 |
| <code>CustomValidator.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>CustomValidator.Enabled=true</code> | ✅ 一致 |
| <code>CustomValidator.ErrorMessage=empty</code> | ✅ 一致 |
| <code>CustomValidator.ErrorMessage=japanese</code> | ✅ 一致 |
| <code>CustomValidator.ErrorMessage=markup</code> | ✅ 一致 |
| <code>CustomValidator.ErrorMessage=text</code> | ✅ 一致 |
| <code>CustomValidator.ErrorMessage=text+EnableViewState=false</code> | ✅ 一致 |
| <code>CustomValidator.Font.Bold=false</code> | ✅ 一致 |
| <code>CustomValidator.Font.Bold=true</code> | ✅ 一致 |
| <code>CustomValidator.Font.Italic=false</code> | ✅ 一致 |
| <code>CustomValidator.Font.Italic=true</code> | ✅ 一致 |
| <code>CustomValidator.Font.Name=empty</code> | ✅ 一致 |
| <code>CustomValidator.Font.Name=japanese</code> | ✅ 一致 |
| <code>CustomValidator.Font.Name=markup</code> | ✅ 一致 |
| <code>CustomValidator.Font.Name=text</code> | ✅ 一致 |
| <code>CustomValidator.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>CustomValidator.Font.Overline=false</code> | ✅ 一致 |
| <code>CustomValidator.Font.Overline=true</code> | ✅ 一致 |
| <code>CustomValidator.Font.Size=12pt</code> | ✅ 一致 |
| <code>CustomValidator.Font.Size=150%</code> | ✅ 一致 |
| <code>CustomValidator.Font.Size=Large</code> | ✅ 一致 |
| <code>CustomValidator.Font.Strikeout=false</code> | ✅ 一致 |
| <code>CustomValidator.Font.Strikeout=true</code> | ✅ 一致 |
| <code>CustomValidator.Font.Underline=false</code> | ✅ 一致 |
| <code>CustomValidator.Font.Underline=true</code> | ✅ 一致 |
| <code>CustomValidator.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>CustomValidator.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>CustomValidator.ForeColor=Red</code> | ✅ 一致 |
| <code>CustomValidator.ForeColor=Transparent</code> | ✅ 一致 |
| <code>CustomValidator.Height=120px</code> | ✅ 一致 |
| <code>CustomValidator.Height=2em</code> | ✅ 一致 |
| <code>CustomValidator.Height=50%</code> | ✅ 一致 |
| <code>CustomValidator.ID=empty</code> | ✅ 一致 |
| <code>CustomValidator.ID=japanese</code> | ✅ 一致 |
| <code>CustomValidator.ID=markup</code> | ✅ 一致 |
| <code>CustomValidator.ID=text</code> | ✅ 一致 |
| <code>CustomValidator.IsValid=false</code> | ✅ 一致 |
| <code>CustomValidator.IsValid=false+Display=Dynamic</code> | ✅ 一致 |
| <code>CustomValidator.IsValid=false+Display=None</code> | ✅ 一致 |
| <code>CustomValidator.IsValid=false+EnableClientScript=false</code> | ✅ 一致 |
| <code>CustomValidator.IsValid=false+Enabled=false</code> | ✅ 一致 |
| <code>CustomValidator.IsValid=false+ErrorMessage=markup</code> | ✅ 一致 |
| <code>CustomValidator.IsValid=false+ForeColor=#3366CC</code> | ✅ 一致 |
| <code>CustomValidator.IsValid=false+Text=markup+ErrorMessage=text</code> | ✅ 一致 |
| <code>CustomValidator.IsValid=false+Text=text</code> | ✅ 一致 |
| <code>CustomValidator.IsValid=false+Visible=false</code> | ✅ 一致 |
| <code>CustomValidator.IsValid=true</code> | ✅ 一致 |
| <code>CustomValidator.SetFocusOnError=false</code> | ✅ 一致 |
| <code>CustomValidator.SetFocusOnError=true</code> | ✅ 一致 |
| <code>CustomValidator.SetFocusOnError=true+EnableClientScript=false</code> | ✅ 一致 |
| <code>CustomValidator.SkinID=empty</code> | ✅ 一致 |
| <code>CustomValidator.SkinID=japanese</code> | ✅ 一致 |
| <code>CustomValidator.SkinID=markup</code> | ✅ 一致 |
| <code>CustomValidator.SkinID=text</code> | ✅ 一致 |
| <code>CustomValidator.Style[color]=1em</code> | ✅ 一致 |
| <code>CustomValidator.Style[color]=red</code> | ✅ 一致 |
| <code>CustomValidator.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>CustomValidator.Style[margin-left]=red</code> | ✅ 一致 |
| <code>CustomValidator.TabIndex=-1</code> | ✅ 一致 |
| <code>CustomValidator.TabIndex=0</code> | ✅ 一致 |
| <code>CustomValidator.TabIndex=3</code> | ✅ 一致 |
| <code>CustomValidator.Text=empty</code> | ✅ 一致 |
| <code>CustomValidator.Text=japanese</code> | ✅ 一致 |
| <code>CustomValidator.Text=markup</code> | ✅ 一致 |
| <code>CustomValidator.Text=text</code> | ✅ 一致 |
| <code>CustomValidator.ToolTip=empty</code> | ✅ 一致 |
| <code>CustomValidator.ToolTip=japanese</code> | ✅ 一致 |
| <code>CustomValidator.ToolTip=markup</code> | ✅ 一致 |
| <code>CustomValidator.ToolTip=text</code> | ✅ 一致 |
| <code>CustomValidator.ValidateEmptyText=false</code> | ✅ 一致 |
| <code>CustomValidator.ValidateEmptyText=true</code> | ✅ 一致 |
| <code>CustomValidator.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>CustomValidator.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>CustomValidator.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>CustomValidator.ValidationGroup=empty</code> | ✅ 一致 |
| <code>CustomValidator.ValidationGroup=japanese</code> | ✅ 一致 |
| <code>CustomValidator.ValidationGroup=markup</code> | ✅ 一致 |
| <code>CustomValidator.ValidationGroup=text</code> | ✅ 一致 |
| <code>CustomValidator.ValidationGroup=text+EnableClientScript=false</code> | ✅ 一致 |
| <code>CustomValidator.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>CustomValidator.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>CustomValidator.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>CustomValidator.Visible=false</code> | ✅ 一致 |
| <code>CustomValidator.Visible=true</code> | ✅ 一致 |
| <code>CustomValidator.Width=120px</code> | ✅ 一致 |
| <code>CustomValidator.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>CustomValidator.Width=2em</code> | ✅ 一致 |
| <code>CustomValidator.Width=50%</code> | ✅ 一致 |
| <code>CustomValidator[naming container].(default)</code> | ✅ 一致 |
| <code>CustomValidator[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>CustomValidator[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>CustomValidator[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>CustomValidator[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>CustomValidator[naming container].ID=text</code> | ✅ 一致 |
| <code>CustomValidator[naming container].IsValid=false</code> | ✅ 一致 |
| <code>CustomValidator[naming container].ValidationGroup=text</code> | ✅ 一致 |
| <code>CustomValidator[themed].(default)</code> | ✅ 一致 |
| <code>CustomValidator[themed].CssClass=text</code> | ✅ 一致 |
| <code>CustomValidator[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>CustomValidator[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| CustomValidator [render][databind][dynamic] first request | ✅ 一致 |
| CustomValidator [servervalidate] empty: ServerValidate only where ValidateEmptyText | ✅ 一致 |
| CustomValidator [servervalidate][dynamic] ServerValidate fails, a validator without a control too | ✅ 一致 |
| CustomValidator [servervalidate][dynamic] ServerValidate passes | ✅ 一致 |
| CustomValidator [clientvalidate] the client function fails: no request | ✅ 一致 |
| CustomValidator [clientvalidate] the client function passes: posted, ServerValidate too | ✅ 一致 |
| CustomValidator [visible][servervalidate] the hidden validator shown, then validating | ✅ 一致 |

</details>
