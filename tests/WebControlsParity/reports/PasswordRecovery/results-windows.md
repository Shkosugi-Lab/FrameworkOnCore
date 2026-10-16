# PasswordRecovery パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 00:35
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 220 | 220 | 0 | 0 |
| ② HTML | 286 | 286 | 0 | 0 |
| ③ シナリオ | 15 | 15 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 229 件中 229 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 117 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 15 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 272 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 14 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 7 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 2 | 0 | ✅ |
| テンプレート | 0 | 2 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>PasswordRecovery.Api.Control.#ctor</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.Adapter</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.ClientID</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.Context</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.Controls</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.DataBind</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.Dispose</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.Disposed</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.Events</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.FindControl</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.Focus</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.HasControls</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.ID</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.Init</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.Load</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.OnInit</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.Page</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.Parent</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.PreRender</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.Render</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.Site</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.SkinID</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.Unload</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.ViewState</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.Control.Visible</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.CompositeControl.#ctor</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.CompositeControl.Controls</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.CompositeControl.DataBind</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.CompositeControl.RecreateChildControls</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.CompositeControl.Render</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.CompositeControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.ICompositeControlDesignerAccessor.RecreateChildControls</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.#ctor</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.Answer</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.AnswerLabelText</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.AnswerLookupError</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.AnswerRequiredErrorMessage</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.BorderPadding</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.CreateChildControls</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.FailureTextStyle</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.GeneralFailureText</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.HelpPageIconUrl</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.HelpPageText</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.HelpPageUrl</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.HyperLinkStyle</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.InstructionTextStyle</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.LabelStyle</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.LoadControlState</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.LoadViewState</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.MailDefinition</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.MembershipProvider</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.OnAnswerLookupError</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.OnBubbleEvent</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.OnInit</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.OnPreRender</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.OnSendMailError</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.OnSendingMail</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.OnUserLookupError</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.OnVerifyingAnswer</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.OnVerifyingUser</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.Question</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.QuestionFailureText</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.QuestionInstructionText</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.QuestionLabelText</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.QuestionTemplate</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.QuestionTemplateContainer</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.QuestionTitleText</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.Render</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.RenderOuterTable</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.SaveControlState</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.SaveViewState</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.SendMailError</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.SendingMail</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.SetDesignModeState</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.SubmitButtonCommandName</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.SubmitButtonImageUrl</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.SubmitButtonStyle</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.SubmitButtonText</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.SubmitButtonType</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.SuccessPageUrl</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.SuccessTemplate</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.SuccessTemplateContainer</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.SuccessText</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.SuccessTextStyle</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.TagKey</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.TextBoxStyle</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.TextLayout</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.TitleTextStyle</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.TrackViewState</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.UserLookupError</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.UserName</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.UserNameFailureText</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.UserNameInstructionText</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.UserNameLabelText</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.UserNameRequiredErrorMessage</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.UserNameTemplate</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.UserNameTemplateContainer</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.UserNameTitleText</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.ValidatorTextStyle</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.VerifyingAnswer</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.PasswordRecovery.VerifyingUser</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>PasswordRecovery.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>PasswordRecovery.(case list)</code> | ✅ 一致 |
| <code>PasswordRecovery.(default)</code> | ✅ 一致 |
| <code>PasswordRecovery.AccessKey=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.AccessKey=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.AccessKey=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.AccessKey=text</code> | ✅ 一致 |
| <code>PasswordRecovery.AnswerLabelText=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.AnswerLabelText=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.AnswerLabelText=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.AnswerLabelText=text</code> | ✅ 一致 |
| <code>PasswordRecovery.AnswerRequiredErrorMessage=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.AnswerRequiredErrorMessage=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.AnswerRequiredErrorMessage=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.AnswerRequiredErrorMessage=text</code> | ✅ 一致 |
| <code>PasswordRecovery.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>PasswordRecovery.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>PasswordRecovery.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>PasswordRecovery.BackColor=#3366CC</code> | ✅ 一致 |
| <code>PasswordRecovery.BackColor=Red</code> | ✅ 一致 |
| <code>PasswordRecovery.BackColor=Transparent</code> | ✅ 一致 |
| <code>PasswordRecovery.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>PasswordRecovery.BorderColor=Red</code> | ✅ 一致 |
| <code>PasswordRecovery.BorderColor=Transparent</code> | ✅ 一致 |
| <code>PasswordRecovery.BorderPadding=-1</code> | ✅ 一致 |
| <code>PasswordRecovery.BorderPadding=0</code> | ✅ 一致 |
| <code>PasswordRecovery.BorderPadding=3</code> | ✅ 一致 |
| <code>PasswordRecovery.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>PasswordRecovery.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>PasswordRecovery.BorderStyle=Double</code> | ✅ 一致 |
| <code>PasswordRecovery.BorderStyle=Groove</code> | ✅ 一致 |
| <code>PasswordRecovery.BorderStyle=Inset</code> | ✅ 一致 |
| <code>PasswordRecovery.BorderStyle=None</code> | ✅ 一致 |
| <code>PasswordRecovery.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>PasswordRecovery.BorderStyle=Outset</code> | ✅ 一致 |
| <code>PasswordRecovery.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>PasswordRecovery.BorderStyle=Solid</code> | ✅ 一致 |
| <code>PasswordRecovery.BorderWidth=120px</code> | ✅ 一致 |
| <code>PasswordRecovery.BorderWidth=2em</code> | ✅ 一致 |
| <code>PasswordRecovery.BorderWidth=50%</code> | ✅ 一致 |
| <code>PasswordRecovery.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>PasswordRecovery.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>PasswordRecovery.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>PasswordRecovery.ClientIDMode=Static</code> | ✅ 一致 |
| <code>PasswordRecovery.CssClass=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.CssClass=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.CssClass=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.CssClass=text</code> | ✅ 一致 |
| <code>PasswordRecovery.EnableTheming=false</code> | ✅ 一致 |
| <code>PasswordRecovery.EnableTheming=true</code> | ✅ 一致 |
| <code>PasswordRecovery.EnableViewState=false</code> | ✅ 一致 |
| <code>PasswordRecovery.EnableViewState=true</code> | ✅ 一致 |
| <code>PasswordRecovery.Enabled=false</code> | ✅ 一致 |
| <code>PasswordRecovery.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>PasswordRecovery.Enabled=true</code> | ✅ 一致 |
| <code>PasswordRecovery.FailureTextStyle.BackColor=Red</code> | ✅ 一致 |
| <code>PasswordRecovery.FailureTextStyle.CssClass=text</code> | ✅ 一致 |
| <code>PasswordRecovery.FailureTextStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>PasswordRecovery.FailureTextStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>PasswordRecovery.FailureTextStyle.Height=120px</code> | ✅ 一致 |
| <code>PasswordRecovery.FailureTextStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>PasswordRecovery.FailureTextStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>PasswordRecovery.FailureTextStyle.Wrap=true</code> | ✅ 一致 |
| <code>PasswordRecovery.Font.Bold=false</code> | ✅ 一致 |
| <code>PasswordRecovery.Font.Bold=true</code> | ✅ 一致 |
| <code>PasswordRecovery.Font.Italic=false</code> | ✅ 一致 |
| <code>PasswordRecovery.Font.Italic=true</code> | ✅ 一致 |
| <code>PasswordRecovery.Font.Name=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.Font.Name=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.Font.Name=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.Font.Name=text</code> | ✅ 一致 |
| <code>PasswordRecovery.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>PasswordRecovery.Font.Overline=false</code> | ✅ 一致 |
| <code>PasswordRecovery.Font.Overline=true</code> | ✅ 一致 |
| <code>PasswordRecovery.Font.Size=12pt</code> | ✅ 一致 |
| <code>PasswordRecovery.Font.Size=150%</code> | ✅ 一致 |
| <code>PasswordRecovery.Font.Size=Large</code> | ✅ 一致 |
| <code>PasswordRecovery.Font.Strikeout=false</code> | ✅ 一致 |
| <code>PasswordRecovery.Font.Strikeout=true</code> | ✅ 一致 |
| <code>PasswordRecovery.Font.Underline=false</code> | ✅ 一致 |
| <code>PasswordRecovery.Font.Underline=true</code> | ✅ 一致 |
| <code>PasswordRecovery.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>PasswordRecovery.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>PasswordRecovery.ForeColor=Red</code> | ✅ 一致 |
| <code>PasswordRecovery.ForeColor=Transparent</code> | ✅ 一致 |
| <code>PasswordRecovery.GeneralFailureText=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.GeneralFailureText=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.GeneralFailureText=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.GeneralFailureText=text</code> | ✅ 一致 |
| <code>PasswordRecovery.Height=120px</code> | ✅ 一致 |
| <code>PasswordRecovery.Height=2em</code> | ✅ 一致 |
| <code>PasswordRecovery.Height=50%</code> | ✅ 一致 |
| <code>PasswordRecovery.HelpPageIconUrl=app-relative</code> | ✅ 一致 |
| <code>PasswordRecovery.HelpPageIconUrl=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.HelpPageIconUrl=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.HelpPageIconUrl=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.HelpPageIconUrl=text</code> | ✅ 一致 |
| <code>PasswordRecovery.HelpPageText=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.HelpPageText=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.HelpPageText=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.HelpPageText=text</code> | ✅ 一致 |
| <code>PasswordRecovery.HelpPageText=text+HelpPageUrl=app-relative</code> | ✅ 一致 |
| <code>PasswordRecovery.HelpPageUrl=app-relative</code> | ✅ 一致 |
| <code>PasswordRecovery.HelpPageUrl=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.HelpPageUrl=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.HelpPageUrl=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.HelpPageUrl=text</code> | ✅ 一致 |
| <code>PasswordRecovery.HyperLinkStyle.BackColor=Red</code> | ✅ 一致 |
| <code>PasswordRecovery.HyperLinkStyle.CssClass=text</code> | ✅ 一致 |
| <code>PasswordRecovery.HyperLinkStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>PasswordRecovery.HyperLinkStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>PasswordRecovery.HyperLinkStyle.Height=120px</code> | ✅ 一致 |
| <code>PasswordRecovery.HyperLinkStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>PasswordRecovery.HyperLinkStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>PasswordRecovery.HyperLinkStyle.Wrap=true</code> | ✅ 一致 |
| <code>PasswordRecovery.ID=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.ID=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.ID=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.ID=text</code> | ✅ 一致 |
| <code>PasswordRecovery.InstructionTextStyle.BackColor=Red</code> | ✅ 一致 |
| <code>PasswordRecovery.InstructionTextStyle.CssClass=text</code> | ✅ 一致 |
| <code>PasswordRecovery.InstructionTextStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>PasswordRecovery.InstructionTextStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>PasswordRecovery.InstructionTextStyle.Height=120px</code> | ✅ 一致 |
| <code>PasswordRecovery.InstructionTextStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>PasswordRecovery.InstructionTextStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>PasswordRecovery.InstructionTextStyle.Wrap=true</code> | ✅ 一致 |
| <code>PasswordRecovery.LabelStyle.BackColor=Red</code> | ✅ 一致 |
| <code>PasswordRecovery.LabelStyle.CssClass=text</code> | ✅ 一致 |
| <code>PasswordRecovery.LabelStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>PasswordRecovery.LabelStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>PasswordRecovery.LabelStyle.Height=120px</code> | ✅ 一致 |
| <code>PasswordRecovery.LabelStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>PasswordRecovery.LabelStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>PasswordRecovery.LabelStyle.Wrap=true</code> | ✅ 一致 |
| <code>PasswordRecovery.MembershipProvider=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.MembershipProvider=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.MembershipProvider=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.MembershipProvider=text</code> | ✅ 一致 |
| <code>PasswordRecovery.QuestionFailureText=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.QuestionFailureText=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.QuestionFailureText=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.QuestionFailureText=text</code> | ✅ 一致 |
| <code>PasswordRecovery.QuestionInstructionText=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.QuestionInstructionText=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.QuestionInstructionText=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.QuestionInstructionText=text</code> | ✅ 一致 |
| <code>PasswordRecovery.QuestionLabelText=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.QuestionLabelText=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.QuestionLabelText=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.QuestionLabelText=text</code> | ✅ 一致 |
| <code>PasswordRecovery.QuestionTitleText=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.QuestionTitleText=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.QuestionTitleText=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.QuestionTitleText=text</code> | ✅ 一致 |
| <code>PasswordRecovery.RenderOuterTable=false</code> | ✅ 一致 |
| <code>PasswordRecovery.RenderOuterTable=false+UserName=text</code> | ✅ 一致 |
| <code>PasswordRecovery.RenderOuterTable=true</code> | ✅ 一致 |
| <code>PasswordRecovery.SkinID=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.SkinID=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.SkinID=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.SkinID=text</code> | ✅ 一致 |
| <code>PasswordRecovery.Style[color]=1em</code> | ✅ 一致 |
| <code>PasswordRecovery.Style[color]=red</code> | ✅ 一致 |
| <code>PasswordRecovery.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>PasswordRecovery.Style[margin-left]=red</code> | ✅ 一致 |
| <code>PasswordRecovery.SubmitButtonImageUrl=app-relative</code> | ✅ 一致 |
| <code>PasswordRecovery.SubmitButtonImageUrl=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.SubmitButtonImageUrl=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.SubmitButtonImageUrl=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.SubmitButtonImageUrl=text</code> | ✅ 一致 |
| <code>PasswordRecovery.SubmitButtonStyle.BackColor=Red</code> | ✅ 一致 |
| <code>PasswordRecovery.SubmitButtonStyle.CssClass=text</code> | ✅ 一致 |
| <code>PasswordRecovery.SubmitButtonStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>PasswordRecovery.SubmitButtonStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>PasswordRecovery.SubmitButtonStyle.Height=120px</code> | ✅ 一致 |
| <code>PasswordRecovery.SubmitButtonText=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.SubmitButtonText=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.SubmitButtonText=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.SubmitButtonText=text</code> | ✅ 一致 |
| <code>PasswordRecovery.SubmitButtonType=Button</code> | ✅ 一致 |
| <code>PasswordRecovery.SubmitButtonType=Image</code> | ✅ 一致 |
| <code>PasswordRecovery.SubmitButtonType=Image+SubmitButtonImageUrl=app-relative</code> | ✅ 一致 |
| <code>PasswordRecovery.SubmitButtonType=Link</code> | ✅ 一致 |
| <code>PasswordRecovery.SubmitButtonType=Link+SubmitButtonText=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.SuccessPageUrl=app-relative</code> | ✅ 一致 |
| <code>PasswordRecovery.SuccessPageUrl=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.SuccessPageUrl=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.SuccessPageUrl=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.SuccessPageUrl=text</code> | ✅ 一致 |
| <code>PasswordRecovery.SuccessText=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.SuccessText=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.SuccessText=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.SuccessText=text</code> | ✅ 一致 |
| <code>PasswordRecovery.SuccessTextStyle.BackColor=Red</code> | ✅ 一致 |
| <code>PasswordRecovery.SuccessTextStyle.CssClass=text</code> | ✅ 一致 |
| <code>PasswordRecovery.SuccessTextStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>PasswordRecovery.SuccessTextStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>PasswordRecovery.SuccessTextStyle.Height=120px</code> | ✅ 一致 |
| <code>PasswordRecovery.SuccessTextStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>PasswordRecovery.SuccessTextStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>PasswordRecovery.SuccessTextStyle.Wrap=true</code> | ✅ 一致 |
| <code>PasswordRecovery.TabIndex=-1</code> | ✅ 一致 |
| <code>PasswordRecovery.TabIndex=0</code> | ✅ 一致 |
| <code>PasswordRecovery.TabIndex=3</code> | ✅ 一致 |
| <code>PasswordRecovery.TextBoxStyle.BackColor=Red</code> | ✅ 一致 |
| <code>PasswordRecovery.TextBoxStyle.BackColor=Red+TitleTextStyle.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>PasswordRecovery.TextBoxStyle.CssClass=text</code> | ✅ 一致 |
| <code>PasswordRecovery.TextBoxStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>PasswordRecovery.TextBoxStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>PasswordRecovery.TextBoxStyle.Height=120px</code> | ✅ 一致 |
| <code>PasswordRecovery.TextLayout=TextOnLeft</code> | ✅ 一致 |
| <code>PasswordRecovery.TextLayout=TextOnTop</code> | ✅ 一致 |
| <code>PasswordRecovery.TitleTextStyle.BackColor=Red</code> | ✅ 一致 |
| <code>PasswordRecovery.TitleTextStyle.CssClass=text</code> | ✅ 一致 |
| <code>PasswordRecovery.TitleTextStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>PasswordRecovery.TitleTextStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>PasswordRecovery.TitleTextStyle.Height=120px</code> | ✅ 一致 |
| <code>PasswordRecovery.TitleTextStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>PasswordRecovery.TitleTextStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>PasswordRecovery.TitleTextStyle.Wrap=true</code> | ✅ 一致 |
| <code>PasswordRecovery.ToolTip=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.ToolTip=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.ToolTip=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.ToolTip=text</code> | ✅ 一致 |
| <code>PasswordRecovery.UserName=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.UserName=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.UserName=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.UserName=text</code> | ✅ 一致 |
| <code>PasswordRecovery.UserName=text+EnableViewState=false</code> | ✅ 一致 |
| <code>PasswordRecovery.UserName=text+UserNameTitleText=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.UserName=text+Visible=false</code> | ✅ 一致 |
| <code>PasswordRecovery.UserNameFailureText=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.UserNameFailureText=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.UserNameFailureText=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.UserNameFailureText=text</code> | ✅ 一致 |
| <code>PasswordRecovery.UserNameInstructionText=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.UserNameInstructionText=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.UserNameInstructionText=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.UserNameInstructionText=markup+UserNameLabelText=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.UserNameInstructionText=text</code> | ✅ 一致 |
| <code>PasswordRecovery.UserNameLabelText=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.UserNameLabelText=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.UserNameLabelText=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.UserNameLabelText=text</code> | ✅ 一致 |
| <code>PasswordRecovery.UserNameRequiredErrorMessage=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.UserNameRequiredErrorMessage=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.UserNameRequiredErrorMessage=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.UserNameRequiredErrorMessage=text</code> | ✅ 一致 |
| <code>PasswordRecovery.UserNameTitleText=empty</code> | ✅ 一致 |
| <code>PasswordRecovery.UserNameTitleText=japanese</code> | ✅ 一致 |
| <code>PasswordRecovery.UserNameTitleText=markup</code> | ✅ 一致 |
| <code>PasswordRecovery.UserNameTitleText=text</code> | ✅ 一致 |
| <code>PasswordRecovery.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>PasswordRecovery.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>PasswordRecovery.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>PasswordRecovery.ValidatorTextStyle.BackColor=Red</code> | ✅ 一致 |
| <code>PasswordRecovery.ValidatorTextStyle.CssClass=text</code> | ✅ 一致 |
| <code>PasswordRecovery.ValidatorTextStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>PasswordRecovery.ValidatorTextStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>PasswordRecovery.ValidatorTextStyle.Height=120px</code> | ✅ 一致 |
| <code>PasswordRecovery.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>PasswordRecovery.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>PasswordRecovery.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>PasswordRecovery.Visible=false</code> | ✅ 一致 |
| <code>PasswordRecovery.Visible=true</code> | ✅ 一致 |
| <code>PasswordRecovery.Width=120px</code> | ✅ 一致 |
| <code>PasswordRecovery.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>PasswordRecovery.Width=2em</code> | ✅ 一致 |
| <code>PasswordRecovery.Width=50%</code> | ✅ 一致 |
| <code>PasswordRecovery[naming container].(default)</code> | ✅ 一致 |
| <code>PasswordRecovery[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>PasswordRecovery[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>PasswordRecovery[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>PasswordRecovery[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>PasswordRecovery[naming container].ID=text</code> | ✅ 一致 |
| <code>PasswordRecovery[naming container].UserName=text</code> | ✅ 一致 |
| <code>PasswordRecovery[templates].(default)</code> | ✅ 一致 |
| <code>PasswordRecovery[themed].(default)</code> | ✅ 一致 |
| <code>PasswordRecovery[themed].CssClass=text</code> | ✅ 一致 |
| <code>PasswordRecovery[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>PasswordRecovery[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| PasswordRecovery [render][databind][dynamic][templates] first request, anonymous: the login forms, the status, the views, the wizards | ✅ 一致 |
| PasswordRecovery [viewstate] a login with a wrong password: its error | ✅ 一致 |
| PasswordRecovery [viewstate] logged in (redirected to the page): the name, the status, the role&#39;s view | ✅ 一致 |
| PasswordRecovery [viewstate] the password changed: the success template | ✅ 一致 |
| PasswordRecovery [viewstate] logged out by the status | ✅ 一致 |
| PasswordRecovery [viewstate] a recovery for a user who does not exist | ✅ 一致 |
| PasswordRecovery [viewstate] the user&#39;s question | ✅ 一致 |
| PasswordRecovery [viewstate][templates] the answer right: the password reset, its mail prepared (cancelled), the success template | ✅ 一致 |
| PasswordRecovery [viewstate] a user created: the complete step | ✅ 一致 |
| PasswordRecovery [viewstate] the wizard&#39;s next step (the templated one) | ✅ 一致 |
| PasswordRecovery [viewstate] next, then previous | ✅ 一致 |
| PasswordRecovery [viewstate] a step chosen in the side bar | ✅ 一致 |
| PasswordRecovery [viewstate] finished: the complete step, the first step&#39;s value kept | ✅ 一致 |
| PasswordRecovery [dynamic][viewstate] a wizard made in code: its next step | ✅ 一致 |
| PasswordRecovery [visible][viewstate] a hidden wizard shown | ✅ 一致 |

</details>
