# RequiredFieldValidator パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 03:02
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 191 | 191 | 0 | 0 |
| ② HTML | 165 | 165 | 0 | 0 |
| ③ シナリオ | 7 | 7 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 200 件中 200 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 39 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 7 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 151 | 1 | ✅ |
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
| <code>RequiredFieldValidator.Api.Control.#ctor</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.Adapter</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.ClientID</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.Context</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.Controls</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.DataBind</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.Dispose</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.Disposed</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.Events</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.FindControl</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.Focus</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.HasControls</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.ID</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.Init</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.Load</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.OnInit</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.Page</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.Parent</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.PreRender</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.Render</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.Site</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.SkinID</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.Unload</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.ViewState</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.Control.Visible</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.ITextControl.Text</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.IValidator.ErrorMessage</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.IValidator.IsValid</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.IValidator.Validate</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.#ctor</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.AddAttributesToRender</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.AssociatedControlID</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.CheckControlValidationProperty</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.ControlPropertiesValid</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.ControlToValidate</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.DetermineRenderUplevel</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.Display</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.EnableClientScript</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.Enabled</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.ErrorMessage</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.EvaluateIsValid</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.ForeColor</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.GetControlRenderID</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.GetControlValidationValue</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.GetValidationProperty</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.IsUnobtrusive</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.IsValid</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.OnInit</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.OnPreRender</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.OnUnload</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.PropertiesValid</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.RegisterValidatorCommonScript</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.RegisterValidatorDeclaration</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.Render</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.RenderUplevel</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.SetFocusOnError</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.Text</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.Validate</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.BaseValidator.ValidationGroup</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.Label.#ctor</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.Label.AddAttributesToRender</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.Label.AddParsedSubObject</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.Label.AssociatedControlID</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.Label.LoadViewState</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.Label.RenderContents</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.Label.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.Label.TagKey</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.Label.Text</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.RequiredFieldValidator.#ctor</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.RequiredFieldValidator.AddAttributesToRender</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.RequiredFieldValidator.EvaluateIsValid</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.RequiredFieldValidator.InitialValue</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>RequiredFieldValidator.(case list)</code> | ✅ 一致 |
| <code>RequiredFieldValidator.(default)</code> | ✅ 一致 |
| <code>RequiredFieldValidator.AccessKey=empty</code> | ✅ 一致 |
| <code>RequiredFieldValidator.AccessKey=japanese</code> | ✅ 一致 |
| <code>RequiredFieldValidator.AccessKey=k</code> | ✅ 一致 |
| <code>RequiredFieldValidator.AccessKey=markup</code> | ✅ 一致 |
| <code>RequiredFieldValidator.AccessKey=text</code> | ✅ 一致 |
| <code>RequiredFieldValidator.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>RequiredFieldValidator.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>RequiredFieldValidator.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>RequiredFieldValidator.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>RequiredFieldValidator.AssociatedControlID=empty</code> | ✅ 一致 |
| <code>RequiredFieldValidator.AssociatedControlID=japanese</code> | ✅ 一致 |
| <code>RequiredFieldValidator.AssociatedControlID=markup</code> | ✅ 一致 |
| <code>RequiredFieldValidator.AssociatedControlID=text</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>RequiredFieldValidator.BackColor=#3366CC</code> | ✅ 一致 |
| <code>RequiredFieldValidator.BackColor=Red</code> | ✅ 一致 |
| <code>RequiredFieldValidator.BackColor=Transparent</code> | ✅ 一致 |
| <code>RequiredFieldValidator.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>RequiredFieldValidator.BorderColor=Red</code> | ✅ 一致 |
| <code>RequiredFieldValidator.BorderColor=Transparent</code> | ✅ 一致 |
| <code>RequiredFieldValidator.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>RequiredFieldValidator.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>RequiredFieldValidator.BorderStyle=Double</code> | ✅ 一致 |
| <code>RequiredFieldValidator.BorderStyle=Groove</code> | ✅ 一致 |
| <code>RequiredFieldValidator.BorderStyle=Inset</code> | ✅ 一致 |
| <code>RequiredFieldValidator.BorderStyle=None</code> | ✅ 一致 |
| <code>RequiredFieldValidator.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>RequiredFieldValidator.BorderStyle=Outset</code> | ✅ 一致 |
| <code>RequiredFieldValidator.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>RequiredFieldValidator.BorderStyle=Solid</code> | ✅ 一致 |
| <code>RequiredFieldValidator.BorderWidth=120px</code> | ✅ 一致 |
| <code>RequiredFieldValidator.BorderWidth=2em</code> | ✅ 一致 |
| <code>RequiredFieldValidator.BorderWidth=50%</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ClientIDMode=Static</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ControlToValidate=empty</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ControlToValidate=japanese</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ControlToValidate=markup</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ControlToValidate=other</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ControlToValidate=target</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ControlToValidate=text</code> | ✅ 一致 |
| <code>RequiredFieldValidator.CssClass=empty</code> | ✅ 一致 |
| <code>RequiredFieldValidator.CssClass=japanese</code> | ✅ 一致 |
| <code>RequiredFieldValidator.CssClass=markup</code> | ✅ 一致 |
| <code>RequiredFieldValidator.CssClass=text</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Display=Dynamic</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Display=None</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Display=Static</code> | ✅ 一致 |
| <code>RequiredFieldValidator.EnableClientScript=false</code> | ✅ 一致 |
| <code>RequiredFieldValidator.EnableClientScript=false+Display=Dynamic</code> | ✅ 一致 |
| <code>RequiredFieldValidator.EnableClientScript=true</code> | ✅ 一致 |
| <code>RequiredFieldValidator.EnableTheming=false</code> | ✅ 一致 |
| <code>RequiredFieldValidator.EnableTheming=true</code> | ✅ 一致 |
| <code>RequiredFieldValidator.EnableViewState=false</code> | ✅ 一致 |
| <code>RequiredFieldValidator.EnableViewState=true</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Enabled=false</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Enabled=true</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ErrorMessage=empty</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ErrorMessage=japanese</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ErrorMessage=markup</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ErrorMessage=text</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ErrorMessage=text+EnableViewState=false</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Font.Bold=false</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Font.Bold=true</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Font.Italic=false</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Font.Italic=true</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Font.Name=empty</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Font.Name=japanese</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Font.Name=markup</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Font.Name=text</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Font.Overline=false</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Font.Overline=true</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Font.Size=12pt</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Font.Size=150%</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Font.Size=Large</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Font.Strikeout=false</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Font.Strikeout=true</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Font.Underline=false</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Font.Underline=true</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ForeColor=Red</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ForeColor=Transparent</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Height=120px</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Height=2em</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Height=50%</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ID=empty</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ID=japanese</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ID=markup</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ID=text</code> | ✅ 一致 |
| <code>RequiredFieldValidator.InitialValue=empty</code> | ✅ 一致 |
| <code>RequiredFieldValidator.InitialValue=japanese</code> | ✅ 一致 |
| <code>RequiredFieldValidator.InitialValue=markup</code> | ✅ 一致 |
| <code>RequiredFieldValidator.InitialValue=text</code> | ✅ 一致 |
| <code>RequiredFieldValidator.IsValid=false</code> | ✅ 一致 |
| <code>RequiredFieldValidator.IsValid=false+Display=Dynamic</code> | ✅ 一致 |
| <code>RequiredFieldValidator.IsValid=false+Display=None</code> | ✅ 一致 |
| <code>RequiredFieldValidator.IsValid=false+EnableClientScript=false</code> | ✅ 一致 |
| <code>RequiredFieldValidator.IsValid=false+Enabled=false</code> | ✅ 一致 |
| <code>RequiredFieldValidator.IsValid=false+ErrorMessage=markup</code> | ✅ 一致 |
| <code>RequiredFieldValidator.IsValid=false+ForeColor=#3366CC</code> | ✅ 一致 |
| <code>RequiredFieldValidator.IsValid=false+Text=markup+ErrorMessage=text</code> | ✅ 一致 |
| <code>RequiredFieldValidator.IsValid=false+Text=text</code> | ✅ 一致 |
| <code>RequiredFieldValidator.IsValid=false+Visible=false</code> | ✅ 一致 |
| <code>RequiredFieldValidator.IsValid=true</code> | ✅ 一致 |
| <code>RequiredFieldValidator.SetFocusOnError=false</code> | ✅ 一致 |
| <code>RequiredFieldValidator.SetFocusOnError=true</code> | ✅ 一致 |
| <code>RequiredFieldValidator.SetFocusOnError=true+EnableClientScript=false</code> | ✅ 一致 |
| <code>RequiredFieldValidator.SkinID=empty</code> | ✅ 一致 |
| <code>RequiredFieldValidator.SkinID=japanese</code> | ✅ 一致 |
| <code>RequiredFieldValidator.SkinID=markup</code> | ✅ 一致 |
| <code>RequiredFieldValidator.SkinID=text</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Style[color]=1em</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Style[color]=red</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Style[margin-left]=red</code> | ✅ 一致 |
| <code>RequiredFieldValidator.TabIndex=-1</code> | ✅ 一致 |
| <code>RequiredFieldValidator.TabIndex=0</code> | ✅ 一致 |
| <code>RequiredFieldValidator.TabIndex=3</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Text=empty</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Text=japanese</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Text=markup</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Text=text</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ToolTip=empty</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ToolTip=japanese</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ToolTip=markup</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ToolTip=text</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ValidationGroup=empty</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ValidationGroup=japanese</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ValidationGroup=markup</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ValidationGroup=text</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ValidationGroup=text+EnableClientScript=false</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>RequiredFieldValidator.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Visible=false</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Visible=true</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Width=120px</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Width=2em</code> | ✅ 一致 |
| <code>RequiredFieldValidator.Width=50%</code> | ✅ 一致 |
| <code>RequiredFieldValidator[naming container].(default)</code> | ✅ 一致 |
| <code>RequiredFieldValidator[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>RequiredFieldValidator[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>RequiredFieldValidator[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>RequiredFieldValidator[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>RequiredFieldValidator[naming container].ID=text</code> | ✅ 一致 |
| <code>RequiredFieldValidator[naming container].IsValid=false</code> | ✅ 一致 |
| <code>RequiredFieldValidator[naming container].ValidationGroup=text</code> | ✅ 一致 |
| <code>RequiredFieldValidator[themed].(default)</code> | ✅ 一致 |
| <code>RequiredFieldValidator[themed].CssClass=text</code> | ✅ 一致 |
| <code>RequiredFieldValidator[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>RequiredFieldValidator[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| RequiredFieldValidator [render][databind][dynamic] first request | ✅ 一致 |
| RequiredFieldValidator [servervalidate][dynamic] all empty: required ones fail, the others pass empty | ✅ 一致 |
| RequiredFieldValidator [servervalidate][dynamic] filled; the initial value still counts as empty | ✅ 一致 |
| RequiredFieldValidator [servervalidate] the initial value changed | ✅ 一致 |
| RequiredFieldValidator [clientvalidate] empty in the browser: no request | ✅ 一致 |
| RequiredFieldValidator [clientvalidate] filled in the browser: posted | ✅ 一致 |
| RequiredFieldValidator [visible][servervalidate] the hidden validator shown, then validating | ✅ 一致 |

</details>
