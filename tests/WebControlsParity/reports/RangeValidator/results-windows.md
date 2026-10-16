# RangeValidator パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 00:42
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 204 | 204 | 0 | 0 |
| ② HTML | 189 | 189 | 0 | 0 |
| ③ シナリオ | 7 | 7 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 216 件中 216 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 42 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 7 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 175 | 1 | ✅ |
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
| <code>RangeValidator.Api.Control.#ctor</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.Adapter</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.ClientID</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.Context</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.Controls</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.DataBind</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.Dispose</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.Disposed</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.Events</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.FindControl</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.Focus</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.HasControls</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.ID</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.Init</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.Load</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.OnInit</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.Page</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.Parent</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.PreRender</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.Render</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.Site</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.SkinID</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.Unload</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.ViewState</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>RangeValidator.Api.Control.Visible</code> | ✅ 一致 |
| <code>RangeValidator.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>RangeValidator.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>RangeValidator.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>RangeValidator.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>RangeValidator.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>RangeValidator.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>RangeValidator.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>RangeValidator.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>RangeValidator.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>RangeValidator.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>RangeValidator.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>RangeValidator.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>RangeValidator.Api.ITextControl.Text</code> | ✅ 一致 |
| <code>RangeValidator.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>RangeValidator.Api.IValidator.ErrorMessage</code> | ✅ 一致 |
| <code>RangeValidator.Api.IValidator.IsValid</code> | ✅ 一致 |
| <code>RangeValidator.Api.IValidator.Validate</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseCompareValidator.#ctor</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseCompareValidator.AddAttributesToRender</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseCompareValidator.CanConvert</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseCompareValidator.Compare</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseCompareValidator.Convert</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseCompareValidator.CultureInvariantValues</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseCompareValidator.CutoffYear</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseCompareValidator.DetermineRenderUplevel</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseCompareValidator.GetDateElementOrder</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseCompareValidator.GetFullYear</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseCompareValidator.Type</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.#ctor</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.AddAttributesToRender</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.AssociatedControlID</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.CheckControlValidationProperty</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.ControlPropertiesValid</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.ControlToValidate</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.DetermineRenderUplevel</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.Display</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.EnableClientScript</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.Enabled</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.ErrorMessage</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.EvaluateIsValid</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.ForeColor</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.GetControlRenderID</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.GetControlValidationValue</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.GetValidationProperty</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.IsUnobtrusive</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.IsValid</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.OnInit</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.OnPreRender</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.OnUnload</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.PropertiesValid</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.RegisterValidatorCommonScript</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.RegisterValidatorDeclaration</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.Render</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.RenderUplevel</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.SetFocusOnError</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.Text</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.Validate</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.BaseValidator.ValidationGroup</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.Label.#ctor</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.Label.AddAttributesToRender</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.Label.AddParsedSubObject</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.Label.AssociatedControlID</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.Label.LoadViewState</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.Label.RenderContents</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.Label.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.Label.TagKey</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.Label.Text</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.RangeValidator.#ctor</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.RangeValidator.AddAttributesToRender</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.RangeValidator.ControlPropertiesValid</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.RangeValidator.EvaluateIsValid</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.RangeValidator.MaximumValue</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.RangeValidator.MinimumValue</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>RangeValidator.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>RangeValidator.(case list)</code> | ✅ 一致 |
| <code>RangeValidator.(default)</code> | ✅ 一致 |
| <code>RangeValidator.AccessKey=empty</code> | ✅ 一致 |
| <code>RangeValidator.AccessKey=japanese</code> | ✅ 一致 |
| <code>RangeValidator.AccessKey=k</code> | ✅ 一致 |
| <code>RangeValidator.AccessKey=markup</code> | ✅ 一致 |
| <code>RangeValidator.AccessKey=text</code> | ✅ 一致 |
| <code>RangeValidator.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>RangeValidator.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>RangeValidator.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>RangeValidator.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>RangeValidator.AssociatedControlID=empty</code> | ✅ 一致 |
| <code>RangeValidator.AssociatedControlID=japanese</code> | ✅ 一致 |
| <code>RangeValidator.AssociatedControlID=markup</code> | ✅ 一致 |
| <code>RangeValidator.AssociatedControlID=text</code> | ✅ 一致 |
| <code>RangeValidator.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>RangeValidator.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>RangeValidator.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>RangeValidator.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>RangeValidator.BackColor=#3366CC</code> | ✅ 一致 |
| <code>RangeValidator.BackColor=Red</code> | ✅ 一致 |
| <code>RangeValidator.BackColor=Transparent</code> | ✅ 一致 |
| <code>RangeValidator.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>RangeValidator.BorderColor=Red</code> | ✅ 一致 |
| <code>RangeValidator.BorderColor=Transparent</code> | ✅ 一致 |
| <code>RangeValidator.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>RangeValidator.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>RangeValidator.BorderStyle=Double</code> | ✅ 一致 |
| <code>RangeValidator.BorderStyle=Groove</code> | ✅ 一致 |
| <code>RangeValidator.BorderStyle=Inset</code> | ✅ 一致 |
| <code>RangeValidator.BorderStyle=None</code> | ✅ 一致 |
| <code>RangeValidator.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>RangeValidator.BorderStyle=Outset</code> | ✅ 一致 |
| <code>RangeValidator.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>RangeValidator.BorderStyle=Solid</code> | ✅ 一致 |
| <code>RangeValidator.BorderWidth=120px</code> | ✅ 一致 |
| <code>RangeValidator.BorderWidth=2em</code> | ✅ 一致 |
| <code>RangeValidator.BorderWidth=50%</code> | ✅ 一致 |
| <code>RangeValidator.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>RangeValidator.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>RangeValidator.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>RangeValidator.ClientIDMode=Static</code> | ✅ 一致 |
| <code>RangeValidator.ControlToValidate=empty</code> | ✅ 一致 |
| <code>RangeValidator.ControlToValidate=japanese</code> | ✅ 一致 |
| <code>RangeValidator.ControlToValidate=markup</code> | ✅ 一致 |
| <code>RangeValidator.ControlToValidate=other</code> | ✅ 一致 |
| <code>RangeValidator.ControlToValidate=target</code> | ✅ 一致 |
| <code>RangeValidator.ControlToValidate=text</code> | ✅ 一致 |
| <code>RangeValidator.CssClass=empty</code> | ✅ 一致 |
| <code>RangeValidator.CssClass=japanese</code> | ✅ 一致 |
| <code>RangeValidator.CssClass=markup</code> | ✅ 一致 |
| <code>RangeValidator.CssClass=text</code> | ✅ 一致 |
| <code>RangeValidator.CultureInvariantValues=false</code> | ✅ 一致 |
| <code>RangeValidator.CultureInvariantValues=true</code> | ✅ 一致 |
| <code>RangeValidator.Display=Dynamic</code> | ✅ 一致 |
| <code>RangeValidator.Display=None</code> | ✅ 一致 |
| <code>RangeValidator.Display=Static</code> | ✅ 一致 |
| <code>RangeValidator.EnableClientScript=false</code> | ✅ 一致 |
| <code>RangeValidator.EnableClientScript=false+Display=Dynamic</code> | ✅ 一致 |
| <code>RangeValidator.EnableClientScript=true</code> | ✅ 一致 |
| <code>RangeValidator.EnableTheming=false</code> | ✅ 一致 |
| <code>RangeValidator.EnableTheming=true</code> | ✅ 一致 |
| <code>RangeValidator.EnableViewState=false</code> | ✅ 一致 |
| <code>RangeValidator.EnableViewState=true</code> | ✅ 一致 |
| <code>RangeValidator.Enabled=false</code> | ✅ 一致 |
| <code>RangeValidator.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>RangeValidator.Enabled=true</code> | ✅ 一致 |
| <code>RangeValidator.ErrorMessage=empty</code> | ✅ 一致 |
| <code>RangeValidator.ErrorMessage=japanese</code> | ✅ 一致 |
| <code>RangeValidator.ErrorMessage=markup</code> | ✅ 一致 |
| <code>RangeValidator.ErrorMessage=text</code> | ✅ 一致 |
| <code>RangeValidator.ErrorMessage=text+EnableViewState=false</code> | ✅ 一致 |
| <code>RangeValidator.Font.Bold=false</code> | ✅ 一致 |
| <code>RangeValidator.Font.Bold=true</code> | ✅ 一致 |
| <code>RangeValidator.Font.Italic=false</code> | ✅ 一致 |
| <code>RangeValidator.Font.Italic=true</code> | ✅ 一致 |
| <code>RangeValidator.Font.Name=empty</code> | ✅ 一致 |
| <code>RangeValidator.Font.Name=japanese</code> | ✅ 一致 |
| <code>RangeValidator.Font.Name=markup</code> | ✅ 一致 |
| <code>RangeValidator.Font.Name=text</code> | ✅ 一致 |
| <code>RangeValidator.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>RangeValidator.Font.Overline=false</code> | ✅ 一致 |
| <code>RangeValidator.Font.Overline=true</code> | ✅ 一致 |
| <code>RangeValidator.Font.Size=12pt</code> | ✅ 一致 |
| <code>RangeValidator.Font.Size=150%</code> | ✅ 一致 |
| <code>RangeValidator.Font.Size=Large</code> | ✅ 一致 |
| <code>RangeValidator.Font.Strikeout=false</code> | ✅ 一致 |
| <code>RangeValidator.Font.Strikeout=true</code> | ✅ 一致 |
| <code>RangeValidator.Font.Underline=false</code> | ✅ 一致 |
| <code>RangeValidator.Font.Underline=true</code> | ✅ 一致 |
| <code>RangeValidator.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>RangeValidator.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>RangeValidator.ForeColor=Red</code> | ✅ 一致 |
| <code>RangeValidator.ForeColor=Transparent</code> | ✅ 一致 |
| <code>RangeValidator.Height=120px</code> | ✅ 一致 |
| <code>RangeValidator.Height=2em</code> | ✅ 一致 |
| <code>RangeValidator.Height=50%</code> | ✅ 一致 |
| <code>RangeValidator.ID=empty</code> | ✅ 一致 |
| <code>RangeValidator.ID=japanese</code> | ✅ 一致 |
| <code>RangeValidator.ID=markup</code> | ✅ 一致 |
| <code>RangeValidator.ID=text</code> | ✅ 一致 |
| <code>RangeValidator.IsValid=false</code> | ✅ 一致 |
| <code>RangeValidator.IsValid=false+Display=Dynamic</code> | ✅ 一致 |
| <code>RangeValidator.IsValid=false+Display=None</code> | ✅ 一致 |
| <code>RangeValidator.IsValid=false+EnableClientScript=false</code> | ✅ 一致 |
| <code>RangeValidator.IsValid=false+Enabled=false</code> | ✅ 一致 |
| <code>RangeValidator.IsValid=false+ErrorMessage=markup</code> | ✅ 一致 |
| <code>RangeValidator.IsValid=false+ForeColor=#3366CC</code> | ✅ 一致 |
| <code>RangeValidator.IsValid=false+Text=markup+ErrorMessage=text</code> | ✅ 一致 |
| <code>RangeValidator.IsValid=false+Text=text</code> | ✅ 一致 |
| <code>RangeValidator.IsValid=false+Visible=false</code> | ✅ 一致 |
| <code>RangeValidator.IsValid=true</code> | ✅ 一致 |
| <code>RangeValidator.MaximumValue=10</code> | ✅ 一致 |
| <code>RangeValidator.MaximumValue=3</code> | ✅ 一致 |
| <code>RangeValidator.MaximumValue=date</code> | ✅ 一致 |
| <code>RangeValidator.MaximumValue=decimal</code> | ✅ 一致 |
| <code>RangeValidator.MaximumValue=empty</code> | ✅ 一致 |
| <code>RangeValidator.MaximumValue=japanese</code> | ✅ 一致 |
| <code>RangeValidator.MaximumValue=markup</code> | ✅ 一致 |
| <code>RangeValidator.MaximumValue=text</code> | ✅ 一致 |
| <code>RangeValidator.MinimumValue=10</code> | ✅ 一致 |
| <code>RangeValidator.MinimumValue=10+MaximumValue=3</code> | ✅ 一致 |
| <code>RangeValidator.MinimumValue=3</code> | ✅ 一致 |
| <code>RangeValidator.MinimumValue=date</code> | ✅ 一致 |
| <code>RangeValidator.MinimumValue=decimal</code> | ✅ 一致 |
| <code>RangeValidator.MinimumValue=empty</code> | ✅ 一致 |
| <code>RangeValidator.MinimumValue=japanese</code> | ✅ 一致 |
| <code>RangeValidator.MinimumValue=markup</code> | ✅ 一致 |
| <code>RangeValidator.MinimumValue=text</code> | ✅ 一致 |
| <code>RangeValidator.SetFocusOnError=false</code> | ✅ 一致 |
| <code>RangeValidator.SetFocusOnError=true</code> | ✅ 一致 |
| <code>RangeValidator.SetFocusOnError=true+EnableClientScript=false</code> | ✅ 一致 |
| <code>RangeValidator.SkinID=empty</code> | ✅ 一致 |
| <code>RangeValidator.SkinID=japanese</code> | ✅ 一致 |
| <code>RangeValidator.SkinID=markup</code> | ✅ 一致 |
| <code>RangeValidator.SkinID=text</code> | ✅ 一致 |
| <code>RangeValidator.Style[color]=1em</code> | ✅ 一致 |
| <code>RangeValidator.Style[color]=red</code> | ✅ 一致 |
| <code>RangeValidator.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>RangeValidator.Style[margin-left]=red</code> | ✅ 一致 |
| <code>RangeValidator.TabIndex=-1</code> | ✅ 一致 |
| <code>RangeValidator.TabIndex=0</code> | ✅ 一致 |
| <code>RangeValidator.TabIndex=3</code> | ✅ 一致 |
| <code>RangeValidator.Text=empty</code> | ✅ 一致 |
| <code>RangeValidator.Text=japanese</code> | ✅ 一致 |
| <code>RangeValidator.Text=markup</code> | ✅ 一致 |
| <code>RangeValidator.Text=text</code> | ✅ 一致 |
| <code>RangeValidator.ToolTip=empty</code> | ✅ 一致 |
| <code>RangeValidator.ToolTip=japanese</code> | ✅ 一致 |
| <code>RangeValidator.ToolTip=markup</code> | ✅ 一致 |
| <code>RangeValidator.ToolTip=text</code> | ✅ 一致 |
| <code>RangeValidator.Type=Currency</code> | ✅ 一致 |
| <code>RangeValidator.Type=Date</code> | ✅ 一致 |
| <code>RangeValidator.Type=Date+MinimumValue=date+MaximumValue=date</code> | ✅ 一致 |
| <code>RangeValidator.Type=Double</code> | ✅ 一致 |
| <code>RangeValidator.Type=Double+MinimumValue=3+MaximumValue=decimal+CultureInvariantValues=true</code> | ✅ 一致 |
| <code>RangeValidator.Type=Integer</code> | ✅ 一致 |
| <code>RangeValidator.Type=Integer+MinimumValue=text</code> | ✅ 一致 |
| <code>RangeValidator.Type=String</code> | ✅ 一致 |
| <code>RangeValidator.Type=String+MinimumValue=text+MaximumValue=text</code> | ✅ 一致 |
| <code>RangeValidator.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>RangeValidator.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>RangeValidator.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>RangeValidator.ValidationGroup=empty</code> | ✅ 一致 |
| <code>RangeValidator.ValidationGroup=japanese</code> | ✅ 一致 |
| <code>RangeValidator.ValidationGroup=markup</code> | ✅ 一致 |
| <code>RangeValidator.ValidationGroup=text</code> | ✅ 一致 |
| <code>RangeValidator.ValidationGroup=text+EnableClientScript=false</code> | ✅ 一致 |
| <code>RangeValidator.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>RangeValidator.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>RangeValidator.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>RangeValidator.Visible=false</code> | ✅ 一致 |
| <code>RangeValidator.Visible=true</code> | ✅ 一致 |
| <code>RangeValidator.Width=120px</code> | ✅ 一致 |
| <code>RangeValidator.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>RangeValidator.Width=2em</code> | ✅ 一致 |
| <code>RangeValidator.Width=50%</code> | ✅ 一致 |
| <code>RangeValidator[naming container].(default)</code> | ✅ 一致 |
| <code>RangeValidator[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>RangeValidator[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>RangeValidator[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>RangeValidator[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>RangeValidator[naming container].ID=text</code> | ✅ 一致 |
| <code>RangeValidator[naming container].IsValid=false</code> | ✅ 一致 |
| <code>RangeValidator[naming container].ValidationGroup=text</code> | ✅ 一致 |
| <code>RangeValidator[themed].(default)</code> | ✅ 一致 |
| <code>RangeValidator[themed].CssClass=text</code> | ✅ 一致 |
| <code>RangeValidator[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>RangeValidator[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| RangeValidator [render][databind][dynamic] first request | ✅ 一致 |
| RangeValidator [servervalidate][dynamic] below the range | ✅ 一致 |
| RangeValidator [servervalidate] not an integer | ✅ 一致 |
| RangeValidator [servervalidate][dynamic] in the range | ✅ 一致 |
| RangeValidator [clientvalidate] above the range in the browser: no request | ✅ 一致 |
| RangeValidator [clientvalidate] in the range in the browser: posted | ✅ 一致 |
| RangeValidator [visible][servervalidate] the hidden validator shown, then validating | ✅ 一致 |

</details>
