# RegularExpressionValidator パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 03:00
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 192 | 192 | 0 | 0 |
| ② HTML | 169 | 169 | 0 | 0 |
| ③ シナリオ | 8 | 8 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 201 件中 201 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 39 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 8 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 155 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 0 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 8 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 3 | 0 | ✅ |
| サーバー側の検証（Validate・IsValid・ErrorMessage・検証グループ） | 0 | 5 | ✅ |
| ブラウザ側の検証スクリプト（EnableClientScript） | 7 | 2 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>RegularExpressionValidator.Api.Control.#ctor</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.Adapter</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.ClientID</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.Context</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.Controls</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.DataBind</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.Dispose</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.Disposed</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.Events</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.FindControl</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.Focus</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.HasControls</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.ID</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.Init</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.Load</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.OnInit</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.Page</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.Parent</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.PreRender</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.Render</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.Site</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.SkinID</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.Unload</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.ViewState</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.Control.Visible</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.ITextControl.Text</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.IValidator.ErrorMessage</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.IValidator.IsValid</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.IValidator.Validate</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.#ctor</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.AddAttributesToRender</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.AssociatedControlID</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.CheckControlValidationProperty</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.ControlPropertiesValid</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.ControlToValidate</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.DetermineRenderUplevel</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.Display</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.EnableClientScript</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.Enabled</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.ErrorMessage</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.EvaluateIsValid</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.ForeColor</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.GetControlRenderID</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.GetControlValidationValue</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.GetValidationProperty</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.IsUnobtrusive</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.IsValid</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.OnInit</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.OnPreRender</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.OnUnload</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.PropertiesValid</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.RegisterValidatorCommonScript</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.RegisterValidatorDeclaration</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.Render</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.RenderUplevel</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.SetFocusOnError</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.Text</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.Validate</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.BaseValidator.ValidationGroup</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.Label.#ctor</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.Label.AddAttributesToRender</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.Label.AddParsedSubObject</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.Label.AssociatedControlID</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.Label.LoadViewState</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.Label.RenderContents</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.Label.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.Label.TagKey</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.Label.Text</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.RegularExpressionValidator.#ctor</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.RegularExpressionValidator.AddAttributesToRender</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.RegularExpressionValidator.EvaluateIsValid</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.RegularExpressionValidator.MatchTimeout</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.RegularExpressionValidator.ValidationExpression</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>RegularExpressionValidator.(case list)</code> | ✅ 一致 |
| <code>RegularExpressionValidator.(default)</code> | ✅ 一致 |
| <code>RegularExpressionValidator.AccessKey=empty</code> | ✅ 一致 |
| <code>RegularExpressionValidator.AccessKey=japanese</code> | ✅ 一致 |
| <code>RegularExpressionValidator.AccessKey=k</code> | ✅ 一致 |
| <code>RegularExpressionValidator.AccessKey=markup</code> | ✅ 一致 |
| <code>RegularExpressionValidator.AccessKey=text</code> | ✅ 一致 |
| <code>RegularExpressionValidator.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>RegularExpressionValidator.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>RegularExpressionValidator.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>RegularExpressionValidator.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>RegularExpressionValidator.AssociatedControlID=empty</code> | ✅ 一致 |
| <code>RegularExpressionValidator.AssociatedControlID=japanese</code> | ✅ 一致 |
| <code>RegularExpressionValidator.AssociatedControlID=markup</code> | ✅ 一致 |
| <code>RegularExpressionValidator.AssociatedControlID=text</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>RegularExpressionValidator.BackColor=#3366CC</code> | ✅ 一致 |
| <code>RegularExpressionValidator.BackColor=Red</code> | ✅ 一致 |
| <code>RegularExpressionValidator.BackColor=Transparent</code> | ✅ 一致 |
| <code>RegularExpressionValidator.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>RegularExpressionValidator.BorderColor=Red</code> | ✅ 一致 |
| <code>RegularExpressionValidator.BorderColor=Transparent</code> | ✅ 一致 |
| <code>RegularExpressionValidator.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>RegularExpressionValidator.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>RegularExpressionValidator.BorderStyle=Double</code> | ✅ 一致 |
| <code>RegularExpressionValidator.BorderStyle=Groove</code> | ✅ 一致 |
| <code>RegularExpressionValidator.BorderStyle=Inset</code> | ✅ 一致 |
| <code>RegularExpressionValidator.BorderStyle=None</code> | ✅ 一致 |
| <code>RegularExpressionValidator.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>RegularExpressionValidator.BorderStyle=Outset</code> | ✅ 一致 |
| <code>RegularExpressionValidator.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>RegularExpressionValidator.BorderStyle=Solid</code> | ✅ 一致 |
| <code>RegularExpressionValidator.BorderWidth=120px</code> | ✅ 一致 |
| <code>RegularExpressionValidator.BorderWidth=2em</code> | ✅ 一致 |
| <code>RegularExpressionValidator.BorderWidth=50%</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ClientIDMode=Static</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ControlToValidate=empty</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ControlToValidate=japanese</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ControlToValidate=markup</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ControlToValidate=other</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ControlToValidate=target</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ControlToValidate=text</code> | ✅ 一致 |
| <code>RegularExpressionValidator.CssClass=empty</code> | ✅ 一致 |
| <code>RegularExpressionValidator.CssClass=japanese</code> | ✅ 一致 |
| <code>RegularExpressionValidator.CssClass=markup</code> | ✅ 一致 |
| <code>RegularExpressionValidator.CssClass=text</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Display=Dynamic</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Display=None</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Display=Static</code> | ✅ 一致 |
| <code>RegularExpressionValidator.EnableClientScript=false</code> | ✅ 一致 |
| <code>RegularExpressionValidator.EnableClientScript=false+Display=Dynamic</code> | ✅ 一致 |
| <code>RegularExpressionValidator.EnableClientScript=true</code> | ✅ 一致 |
| <code>RegularExpressionValidator.EnableTheming=false</code> | ✅ 一致 |
| <code>RegularExpressionValidator.EnableTheming=true</code> | ✅ 一致 |
| <code>RegularExpressionValidator.EnableViewState=false</code> | ✅ 一致 |
| <code>RegularExpressionValidator.EnableViewState=true</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Enabled=false</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Enabled=true</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ErrorMessage=empty</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ErrorMessage=japanese</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ErrorMessage=markup</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ErrorMessage=text</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ErrorMessage=text+EnableViewState=false</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Font.Bold=false</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Font.Bold=true</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Font.Italic=false</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Font.Italic=true</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Font.Name=empty</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Font.Name=japanese</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Font.Name=markup</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Font.Name=text</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Font.Overline=false</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Font.Overline=true</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Font.Size=12pt</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Font.Size=150%</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Font.Size=Large</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Font.Strikeout=false</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Font.Strikeout=true</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Font.Underline=false</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Font.Underline=true</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ForeColor=Red</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ForeColor=Transparent</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Height=120px</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Height=2em</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Height=50%</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ID=empty</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ID=japanese</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ID=markup</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ID=text</code> | ✅ 一致 |
| <code>RegularExpressionValidator.IsValid=false</code> | ✅ 一致 |
| <code>RegularExpressionValidator.IsValid=false+Display=Dynamic</code> | ✅ 一致 |
| <code>RegularExpressionValidator.IsValid=false+Display=None</code> | ✅ 一致 |
| <code>RegularExpressionValidator.IsValid=false+EnableClientScript=false</code> | ✅ 一致 |
| <code>RegularExpressionValidator.IsValid=false+Enabled=false</code> | ✅ 一致 |
| <code>RegularExpressionValidator.IsValid=false+ErrorMessage=markup</code> | ✅ 一致 |
| <code>RegularExpressionValidator.IsValid=false+ForeColor=#3366CC</code> | ✅ 一致 |
| <code>RegularExpressionValidator.IsValid=false+Text=markup+ErrorMessage=text</code> | ✅ 一致 |
| <code>RegularExpressionValidator.IsValid=false+Text=text</code> | ✅ 一致 |
| <code>RegularExpressionValidator.IsValid=false+Visible=false</code> | ✅ 一致 |
| <code>RegularExpressionValidator.IsValid=true</code> | ✅ 一致 |
| <code>RegularExpressionValidator.SetFocusOnError=false</code> | ✅ 一致 |
| <code>RegularExpressionValidator.SetFocusOnError=true</code> | ✅ 一致 |
| <code>RegularExpressionValidator.SetFocusOnError=true+EnableClientScript=false</code> | ✅ 一致 |
| <code>RegularExpressionValidator.SkinID=empty</code> | ✅ 一致 |
| <code>RegularExpressionValidator.SkinID=japanese</code> | ✅ 一致 |
| <code>RegularExpressionValidator.SkinID=markup</code> | ✅ 一致 |
| <code>RegularExpressionValidator.SkinID=text</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Style[color]=1em</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Style[color]=red</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Style[margin-left]=red</code> | ✅ 一致 |
| <code>RegularExpressionValidator.TabIndex=-1</code> | ✅ 一致 |
| <code>RegularExpressionValidator.TabIndex=0</code> | ✅ 一致 |
| <code>RegularExpressionValidator.TabIndex=3</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Text=empty</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Text=japanese</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Text=markup</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Text=text</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ToolTip=empty</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ToolTip=japanese</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ToolTip=markup</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ToolTip=text</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ValidationExpression=digits</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ValidationExpression=digits+EnableClientScript=false</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ValidationExpression=empty</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ValidationExpression=japanese</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ValidationExpression=letters</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ValidationExpression=letters+IsValid=false</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ValidationExpression=markup</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ValidationExpression=text</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ValidationGroup=empty</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ValidationGroup=japanese</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ValidationGroup=markup</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ValidationGroup=text</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ValidationGroup=text+EnableClientScript=false</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>RegularExpressionValidator.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Visible=false</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Visible=true</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Width=120px</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Width=2em</code> | ✅ 一致 |
| <code>RegularExpressionValidator.Width=50%</code> | ✅ 一致 |
| <code>RegularExpressionValidator[naming container].(default)</code> | ✅ 一致 |
| <code>RegularExpressionValidator[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>RegularExpressionValidator[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>RegularExpressionValidator[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>RegularExpressionValidator[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>RegularExpressionValidator[naming container].ID=text</code> | ✅ 一致 |
| <code>RegularExpressionValidator[naming container].IsValid=false</code> | ✅ 一致 |
| <code>RegularExpressionValidator[naming container].ValidationGroup=text</code> | ✅ 一致 |
| <code>RegularExpressionValidator[themed].(default)</code> | ✅ 一致 |
| <code>RegularExpressionValidator[themed].CssClass=text</code> | ✅ 一致 |
| <code>RegularExpressionValidator[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>RegularExpressionValidator[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| RegularExpressionValidator [render][databind][dynamic] first request | ✅ 一致 |
| RegularExpressionValidator [servervalidate] empty: passes | ✅ 一致 |
| RegularExpressionValidator [servervalidate][dynamic] not digits | ✅ 一致 |
| RegularExpressionValidator [servervalidate] digits | ✅ 一致 |
| RegularExpressionValidator [servervalidate] Validate() of the one validator | ✅ 一致 |
| RegularExpressionValidator [clientvalidate] not digits in the browser: no request | ✅ 一致 |
| RegularExpressionValidator [clientvalidate] digits in the browser: posted | ✅ 一致 |
| RegularExpressionValidator [visible][servervalidate] the hidden validator shown, then validating | ✅ 一致 |

</details>
