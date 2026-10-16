# CompareValidator パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 01:35
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 205 | 205 | 0 | 0 |
| ② HTML | 195 | 195 | 0 | 0 |
| ③ シナリオ | 6 | 6 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 217 件中 217 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 43 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 6 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 181 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 0 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 8 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 3 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 3 | 0 | ✅ |
| サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ） | 0 | 3 | ✅ |
| ブラウザ側の検証スクリプト（EnableClientScript） | 6 | 2 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>CompareValidator.Api.Control.#ctor</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.Adapter</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.ClientID</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.Context</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.Controls</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.DataBind</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.Dispose</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.Disposed</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.Events</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.FindControl</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.Focus</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.HasControls</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.ID</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.Init</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.Load</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.OnInit</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.Page</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.Parent</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.PreRender</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.Render</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.Site</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.SkinID</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.Unload</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.ViewState</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>CompareValidator.Api.Control.Visible</code> | ✅ 一致 |
| <code>CompareValidator.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>CompareValidator.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>CompareValidator.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>CompareValidator.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>CompareValidator.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>CompareValidator.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>CompareValidator.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>CompareValidator.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>CompareValidator.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>CompareValidator.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>CompareValidator.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>CompareValidator.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>CompareValidator.Api.ITextControl.Text</code> | ✅ 一致 |
| <code>CompareValidator.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>CompareValidator.Api.IValidator.ErrorMessage</code> | ✅ 一致 |
| <code>CompareValidator.Api.IValidator.IsValid</code> | ✅ 一致 |
| <code>CompareValidator.Api.IValidator.Validate</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseCompareValidator.#ctor</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseCompareValidator.AddAttributesToRender</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseCompareValidator.CanConvert</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseCompareValidator.Compare</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseCompareValidator.Convert</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseCompareValidator.CultureInvariantValues</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseCompareValidator.CutoffYear</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseCompareValidator.DetermineRenderUplevel</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseCompareValidator.GetDateElementOrder</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseCompareValidator.GetFullYear</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseCompareValidator.Type</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.#ctor</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.AddAttributesToRender</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.AssociatedControlID</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.CheckControlValidationProperty</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.ControlPropertiesValid</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.ControlToValidate</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.DetermineRenderUplevel</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.Display</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.EnableClientScript</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.Enabled</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.ErrorMessage</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.EvaluateIsValid</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.ForeColor</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.GetControlRenderID</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.GetControlValidationValue</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.GetValidationProperty</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.IsUnobtrusive</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.IsValid</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.OnInit</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.OnPreRender</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.OnUnload</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.PropertiesValid</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.RegisterValidatorCommonScript</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.RegisterValidatorDeclaration</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.Render</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.RenderUplevel</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.SetFocusOnError</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.Text</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.Validate</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.BaseValidator.ValidationGroup</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.CompareValidator.#ctor</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.CompareValidator.AddAttributesToRender</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.CompareValidator.ControlPropertiesValid</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.CompareValidator.ControlToCompare</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.CompareValidator.EvaluateIsValid</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.CompareValidator.Operator</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.CompareValidator.ValueToCompare</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.Label.#ctor</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.Label.AddAttributesToRender</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.Label.AddParsedSubObject</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.Label.AssociatedControlID</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.Label.LoadViewState</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.Label.RenderContents</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.Label.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.Label.TagKey</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.Label.Text</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>CompareValidator.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>CompareValidator.(case list)</code> | ✅ 一致 |
| <code>CompareValidator.(default)</code> | ✅ 一致 |
| <code>CompareValidator.AccessKey=empty</code> | ✅ 一致 |
| <code>CompareValidator.AccessKey=japanese</code> | ✅ 一致 |
| <code>CompareValidator.AccessKey=k</code> | ✅ 一致 |
| <code>CompareValidator.AccessKey=markup</code> | ✅ 一致 |
| <code>CompareValidator.AccessKey=text</code> | ✅ 一致 |
| <code>CompareValidator.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>CompareValidator.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>CompareValidator.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>CompareValidator.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>CompareValidator.AssociatedControlID=empty</code> | ✅ 一致 |
| <code>CompareValidator.AssociatedControlID=japanese</code> | ✅ 一致 |
| <code>CompareValidator.AssociatedControlID=markup</code> | ✅ 一致 |
| <code>CompareValidator.AssociatedControlID=text</code> | ✅ 一致 |
| <code>CompareValidator.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>CompareValidator.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>CompareValidator.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>CompareValidator.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>CompareValidator.BackColor=#3366CC</code> | ✅ 一致 |
| <code>CompareValidator.BackColor=Red</code> | ✅ 一致 |
| <code>CompareValidator.BackColor=Transparent</code> | ✅ 一致 |
| <code>CompareValidator.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>CompareValidator.BorderColor=Red</code> | ✅ 一致 |
| <code>CompareValidator.BorderColor=Transparent</code> | ✅ 一致 |
| <code>CompareValidator.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>CompareValidator.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>CompareValidator.BorderStyle=Double</code> | ✅ 一致 |
| <code>CompareValidator.BorderStyle=Groove</code> | ✅ 一致 |
| <code>CompareValidator.BorderStyle=Inset</code> | ✅ 一致 |
| <code>CompareValidator.BorderStyle=None</code> | ✅ 一致 |
| <code>CompareValidator.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>CompareValidator.BorderStyle=Outset</code> | ✅ 一致 |
| <code>CompareValidator.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>CompareValidator.BorderStyle=Solid</code> | ✅ 一致 |
| <code>CompareValidator.BorderWidth=120px</code> | ✅ 一致 |
| <code>CompareValidator.BorderWidth=2em</code> | ✅ 一致 |
| <code>CompareValidator.BorderWidth=50%</code> | ✅ 一致 |
| <code>CompareValidator.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>CompareValidator.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>CompareValidator.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>CompareValidator.ClientIDMode=Static</code> | ✅ 一致 |
| <code>CompareValidator.ControlToCompare=empty</code> | ✅ 一致 |
| <code>CompareValidator.ControlToCompare=japanese</code> | ✅ 一致 |
| <code>CompareValidator.ControlToCompare=markup</code> | ✅ 一致 |
| <code>CompareValidator.ControlToCompare=other</code> | ✅ 一致 |
| <code>CompareValidator.ControlToCompare=other+Operator=Equal</code> | ✅ 一致 |
| <code>CompareValidator.ControlToCompare=text</code> | ✅ 一致 |
| <code>CompareValidator.ControlToValidate=empty</code> | ✅ 一致 |
| <code>CompareValidator.ControlToValidate=japanese</code> | ✅ 一致 |
| <code>CompareValidator.ControlToValidate=markup</code> | ✅ 一致 |
| <code>CompareValidator.ControlToValidate=other</code> | ✅ 一致 |
| <code>CompareValidator.ControlToValidate=target</code> | ✅ 一致 |
| <code>CompareValidator.ControlToValidate=text</code> | ✅ 一致 |
| <code>CompareValidator.CssClass=empty</code> | ✅ 一致 |
| <code>CompareValidator.CssClass=japanese</code> | ✅ 一致 |
| <code>CompareValidator.CssClass=markup</code> | ✅ 一致 |
| <code>CompareValidator.CssClass=text</code> | ✅ 一致 |
| <code>CompareValidator.CultureInvariantValues=false</code> | ✅ 一致 |
| <code>CompareValidator.CultureInvariantValues=true</code> | ✅ 一致 |
| <code>CompareValidator.Display=Dynamic</code> | ✅ 一致 |
| <code>CompareValidator.Display=None</code> | ✅ 一致 |
| <code>CompareValidator.Display=Static</code> | ✅ 一致 |
| <code>CompareValidator.EnableClientScript=false</code> | ✅ 一致 |
| <code>CompareValidator.EnableClientScript=false+Display=Dynamic</code> | ✅ 一致 |
| <code>CompareValidator.EnableClientScript=true</code> | ✅ 一致 |
| <code>CompareValidator.EnableTheming=false</code> | ✅ 一致 |
| <code>CompareValidator.EnableTheming=true</code> | ✅ 一致 |
| <code>CompareValidator.EnableViewState=false</code> | ✅ 一致 |
| <code>CompareValidator.EnableViewState=true</code> | ✅ 一致 |
| <code>CompareValidator.Enabled=false</code> | ✅ 一致 |
| <code>CompareValidator.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>CompareValidator.Enabled=true</code> | ✅ 一致 |
| <code>CompareValidator.ErrorMessage=empty</code> | ✅ 一致 |
| <code>CompareValidator.ErrorMessage=japanese</code> | ✅ 一致 |
| <code>CompareValidator.ErrorMessage=markup</code> | ✅ 一致 |
| <code>CompareValidator.ErrorMessage=text</code> | ✅ 一致 |
| <code>CompareValidator.ErrorMessage=text+EnableViewState=false</code> | ✅ 一致 |
| <code>CompareValidator.Font.Bold=false</code> | ✅ 一致 |
| <code>CompareValidator.Font.Bold=true</code> | ✅ 一致 |
| <code>CompareValidator.Font.Italic=false</code> | ✅ 一致 |
| <code>CompareValidator.Font.Italic=true</code> | ✅ 一致 |
| <code>CompareValidator.Font.Name=empty</code> | ✅ 一致 |
| <code>CompareValidator.Font.Name=japanese</code> | ✅ 一致 |
| <code>CompareValidator.Font.Name=markup</code> | ✅ 一致 |
| <code>CompareValidator.Font.Name=text</code> | ✅ 一致 |
| <code>CompareValidator.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>CompareValidator.Font.Overline=false</code> | ✅ 一致 |
| <code>CompareValidator.Font.Overline=true</code> | ✅ 一致 |
| <code>CompareValidator.Font.Size=12pt</code> | ✅ 一致 |
| <code>CompareValidator.Font.Size=150%</code> | ✅ 一致 |
| <code>CompareValidator.Font.Size=Large</code> | ✅ 一致 |
| <code>CompareValidator.Font.Strikeout=false</code> | ✅ 一致 |
| <code>CompareValidator.Font.Strikeout=true</code> | ✅ 一致 |
| <code>CompareValidator.Font.Underline=false</code> | ✅ 一致 |
| <code>CompareValidator.Font.Underline=true</code> | ✅ 一致 |
| <code>CompareValidator.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>CompareValidator.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>CompareValidator.ForeColor=Red</code> | ✅ 一致 |
| <code>CompareValidator.ForeColor=Transparent</code> | ✅ 一致 |
| <code>CompareValidator.Height=120px</code> | ✅ 一致 |
| <code>CompareValidator.Height=2em</code> | ✅ 一致 |
| <code>CompareValidator.Height=50%</code> | ✅ 一致 |
| <code>CompareValidator.ID=empty</code> | ✅ 一致 |
| <code>CompareValidator.ID=japanese</code> | ✅ 一致 |
| <code>CompareValidator.ID=markup</code> | ✅ 一致 |
| <code>CompareValidator.ID=text</code> | ✅ 一致 |
| <code>CompareValidator.IsValid=false</code> | ✅ 一致 |
| <code>CompareValidator.IsValid=false+Display=Dynamic</code> | ✅ 一致 |
| <code>CompareValidator.IsValid=false+Display=None</code> | ✅ 一致 |
| <code>CompareValidator.IsValid=false+EnableClientScript=false</code> | ✅ 一致 |
| <code>CompareValidator.IsValid=false+Enabled=false</code> | ✅ 一致 |
| <code>CompareValidator.IsValid=false+ErrorMessage=markup</code> | ✅ 一致 |
| <code>CompareValidator.IsValid=false+ForeColor=#3366CC</code> | ✅ 一致 |
| <code>CompareValidator.IsValid=false+Text=markup+ErrorMessage=text</code> | ✅ 一致 |
| <code>CompareValidator.IsValid=false+Text=text</code> | ✅ 一致 |
| <code>CompareValidator.IsValid=false+Visible=false</code> | ✅ 一致 |
| <code>CompareValidator.IsValid=true</code> | ✅ 一致 |
| <code>CompareValidator.Operator=DataTypeCheck</code> | ✅ 一致 |
| <code>CompareValidator.Operator=DataTypeCheck+Type=Integer</code> | ✅ 一致 |
| <code>CompareValidator.Operator=Equal</code> | ✅ 一致 |
| <code>CompareValidator.Operator=GreaterThan</code> | ✅ 一致 |
| <code>CompareValidator.Operator=GreaterThanEqual</code> | ✅ 一致 |
| <code>CompareValidator.Operator=LessThan</code> | ✅ 一致 |
| <code>CompareValidator.Operator=LessThanEqual</code> | ✅ 一致 |
| <code>CompareValidator.Operator=NotEqual</code> | ✅ 一致 |
| <code>CompareValidator.SetFocusOnError=false</code> | ✅ 一致 |
| <code>CompareValidator.SetFocusOnError=true</code> | ✅ 一致 |
| <code>CompareValidator.SetFocusOnError=true+EnableClientScript=false</code> | ✅ 一致 |
| <code>CompareValidator.SkinID=empty</code> | ✅ 一致 |
| <code>CompareValidator.SkinID=japanese</code> | ✅ 一致 |
| <code>CompareValidator.SkinID=markup</code> | ✅ 一致 |
| <code>CompareValidator.SkinID=text</code> | ✅ 一致 |
| <code>CompareValidator.Style[color]=1em</code> | ✅ 一致 |
| <code>CompareValidator.Style[color]=red</code> | ✅ 一致 |
| <code>CompareValidator.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>CompareValidator.Style[margin-left]=red</code> | ✅ 一致 |
| <code>CompareValidator.TabIndex=-1</code> | ✅ 一致 |
| <code>CompareValidator.TabIndex=0</code> | ✅ 一致 |
| <code>CompareValidator.TabIndex=3</code> | ✅ 一致 |
| <code>CompareValidator.Text=empty</code> | ✅ 一致 |
| <code>CompareValidator.Text=japanese</code> | ✅ 一致 |
| <code>CompareValidator.Text=markup</code> | ✅ 一致 |
| <code>CompareValidator.Text=text</code> | ✅ 一致 |
| <code>CompareValidator.ToolTip=empty</code> | ✅ 一致 |
| <code>CompareValidator.ToolTip=japanese</code> | ✅ 一致 |
| <code>CompareValidator.ToolTip=markup</code> | ✅ 一致 |
| <code>CompareValidator.ToolTip=text</code> | ✅ 一致 |
| <code>CompareValidator.Type=Currency</code> | ✅ 一致 |
| <code>CompareValidator.Type=Currency+ValueToCompare=decimal</code> | ✅ 一致 |
| <code>CompareValidator.Type=Date</code> | ✅ 一致 |
| <code>CompareValidator.Type=Date+ValueToCompare=date</code> | ✅ 一致 |
| <code>CompareValidator.Type=Double</code> | ✅ 一致 |
| <code>CompareValidator.Type=Double+ValueToCompare=decimal</code> | ✅ 一致 |
| <code>CompareValidator.Type=Double+ValueToCompare=decimal+CultureInvariantValues=true</code> | ✅ 一致 |
| <code>CompareValidator.Type=Integer</code> | ✅ 一致 |
| <code>CompareValidator.Type=Integer+ValueToCompare=text</code> | ✅ 一致 |
| <code>CompareValidator.Type=String</code> | ✅ 一致 |
| <code>CompareValidator.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>CompareValidator.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>CompareValidator.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>CompareValidator.ValidationGroup=empty</code> | ✅ 一致 |
| <code>CompareValidator.ValidationGroup=japanese</code> | ✅ 一致 |
| <code>CompareValidator.ValidationGroup=markup</code> | ✅ 一致 |
| <code>CompareValidator.ValidationGroup=text</code> | ✅ 一致 |
| <code>CompareValidator.ValidationGroup=text+EnableClientScript=false</code> | ✅ 一致 |
| <code>CompareValidator.ValueToCompare=10</code> | ✅ 一致 |
| <code>CompareValidator.ValueToCompare=3</code> | ✅ 一致 |
| <code>CompareValidator.ValueToCompare=date</code> | ✅ 一致 |
| <code>CompareValidator.ValueToCompare=decimal</code> | ✅ 一致 |
| <code>CompareValidator.ValueToCompare=empty</code> | ✅ 一致 |
| <code>CompareValidator.ValueToCompare=japanese</code> | ✅ 一致 |
| <code>CompareValidator.ValueToCompare=markup</code> | ✅ 一致 |
| <code>CompareValidator.ValueToCompare=text</code> | ✅ 一致 |
| <code>CompareValidator.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>CompareValidator.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>CompareValidator.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>CompareValidator.Visible=false</code> | ✅ 一致 |
| <code>CompareValidator.Visible=true</code> | ✅ 一致 |
| <code>CompareValidator.Width=120px</code> | ✅ 一致 |
| <code>CompareValidator.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>CompareValidator.Width=2em</code> | ✅ 一致 |
| <code>CompareValidator.Width=50%</code> | ✅ 一致 |
| <code>CompareValidator[naming container].(default)</code> | ✅ 一致 |
| <code>CompareValidator[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>CompareValidator[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>CompareValidator[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>CompareValidator[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>CompareValidator[naming container].ID=text</code> | ✅ 一致 |
| <code>CompareValidator[naming container].IsValid=false</code> | ✅ 一致 |
| <code>CompareValidator[naming container].ValidationGroup=text</code> | ✅ 一致 |
| <code>CompareValidator[themed].(default)</code> | ✅ 一致 |
| <code>CompareValidator[themed].CssClass=text</code> | ✅ 一致 |
| <code>CompareValidator[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>CompareValidator[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| CompareValidator [render][databind][dynamic] first request | ✅ 一致 |
| CompareValidator [servervalidate][dynamic] not matching, not more than 3, not a date | ✅ 一致 |
| CompareValidator [servervalidate][dynamic] matching, more than 3, a date | ✅ 一致 |
| CompareValidator [clientvalidate] not matching in the browser: no request | ✅ 一致 |
| CompareValidator [clientvalidate] matching in the browser: posted | ✅ 一致 |
| CompareValidator [visible][servervalidate] the hidden validator shown, then validating | ✅ 一致 |

</details>
