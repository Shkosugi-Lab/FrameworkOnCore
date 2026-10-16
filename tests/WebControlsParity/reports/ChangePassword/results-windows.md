# ChangePassword パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-15 13:42
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 247 | 247 | 0 | 0 |
| ② HTML | 401 | 401 | 0 | 0 |
| ③ シナリオ | 15 | 15 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 256 件中 256 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 156 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 15 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 385 | 1 | ✅ |
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
| <code>ChangePassword.Api.Control.#ctor</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.Adapter</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.ClientID</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.Context</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.Controls</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.DataBind</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.Dispose</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.Disposed</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.Events</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.FindControl</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.Focus</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.HasControls</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.ID</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.Init</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.Load</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.OnInit</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.Page</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.Parent</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.PreRender</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.Render</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.Site</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.SkinID</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.Unload</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.ViewState</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>ChangePassword.Api.Control.Visible</code> | ✅ 一致 |
| <code>ChangePassword.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>ChangePassword.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>ChangePassword.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>ChangePassword.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>ChangePassword.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>ChangePassword.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>ChangePassword.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>ChangePassword.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>ChangePassword.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>ChangePassword.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>ChangePassword.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>ChangePassword.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>ChangePassword.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.#ctor</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.BorderPadding</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.CancelButtonClick</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.CancelButtonCommandName</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.CancelButtonImageUrl</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.CancelButtonStyle</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.CancelButtonText</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.CancelButtonType</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.CancelDestinationPageUrl</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.ChangePasswordButtonCommandName</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.ChangePasswordButtonImageUrl</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.ChangePasswordButtonStyle</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.ChangePasswordButtonText</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.ChangePasswordButtonType</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.ChangePasswordError</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.ChangePasswordFailureText</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.ChangePasswordTemplate</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.ChangePasswordTemplateContainer</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.ChangePasswordTitleText</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.ChangedPassword</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.ChangingPassword</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.ConfirmNewPassword</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.ConfirmNewPasswordLabelText</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.ConfirmPasswordCompareErrorMessage</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.ConfirmPasswordRequiredErrorMessage</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.ContinueButtonClick</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.ContinueButtonCommandName</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.ContinueButtonImageUrl</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.ContinueButtonStyle</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.ContinueButtonText</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.ContinueButtonType</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.ContinueDestinationPageUrl</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.CreateChildControls</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.CreateUserIconUrl</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.CreateUserText</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.CreateUserUrl</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.CurrentPassword</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.DisplayUserName</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.EditProfileIconUrl</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.EditProfileText</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.EditProfileUrl</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.FailureTextStyle</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.HelpPageIconUrl</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.HelpPageText</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.HelpPageUrl</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.HyperLinkStyle</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.InstructionText</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.InstructionTextStyle</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.LabelStyle</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.LoadControlState</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.LoadViewState</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.MailDefinition</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.MembershipProvider</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.NewPassword</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.NewPasswordLabelText</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.NewPasswordRegularExpression</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.NewPasswordRegularExpressionErrorMessage</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.NewPasswordRequiredErrorMessage</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.OnBubbleEvent</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.OnCancelButtonClick</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.OnChangePasswordError</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.OnChangedPassword</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.OnChangingPassword</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.OnContinueButtonClick</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.OnInit</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.OnPreRender</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.OnSendMailError</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.OnSendingMail</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.PasswordHintStyle</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.PasswordHintText</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.PasswordLabelText</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.PasswordRecoveryIconUrl</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.PasswordRecoveryText</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.PasswordRecoveryUrl</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.PasswordRequiredErrorMessage</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.Render</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.RenderOuterTable</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.SaveControlState</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.SaveViewState</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.SendMailError</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.SendingMail</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.SetDesignModeState</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.SuccessPageUrl</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.SuccessTemplate</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.SuccessTemplateContainer</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.SuccessText</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.SuccessTextStyle</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.SuccessTitleText</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.TagKey</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.TextBoxStyle</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.TitleTextStyle</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.TrackViewState</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.UserName</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.UserNameLabelText</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.UserNameRequiredErrorMessage</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ChangePassword.ValidatorTextStyle</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.CompositeControl.#ctor</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.CompositeControl.Controls</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.CompositeControl.DataBind</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.CompositeControl.RecreateChildControls</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.CompositeControl.Render</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.CompositeControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.ICompositeControlDesignerAccessor.RecreateChildControls</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>ChangePassword.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>ChangePassword.(case list)</code> | ✅ 一致 |
| <code>ChangePassword.(default)</code> | ✅ 一致 |
| <code>ChangePassword.AccessKey=empty</code> | ✅ 一致 |
| <code>ChangePassword.AccessKey=japanese</code> | ✅ 一致 |
| <code>ChangePassword.AccessKey=markup</code> | ✅ 一致 |
| <code>ChangePassword.AccessKey=text</code> | ✅ 一致 |
| <code>ChangePassword.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>ChangePassword.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>ChangePassword.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>ChangePassword.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>ChangePassword.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>ChangePassword.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>ChangePassword.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>ChangePassword.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>ChangePassword.BackColor=#3366CC</code> | ✅ 一致 |
| <code>ChangePassword.BackColor=Red</code> | ✅ 一致 |
| <code>ChangePassword.BackColor=Transparent</code> | ✅ 一致 |
| <code>ChangePassword.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>ChangePassword.BorderColor=Red</code> | ✅ 一致 |
| <code>ChangePassword.BorderColor=Transparent</code> | ✅ 一致 |
| <code>ChangePassword.BorderPadding=-1</code> | ✅ 一致 |
| <code>ChangePassword.BorderPadding=0</code> | ✅ 一致 |
| <code>ChangePassword.BorderPadding=3</code> | ✅ 一致 |
| <code>ChangePassword.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>ChangePassword.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>ChangePassword.BorderStyle=Double</code> | ✅ 一致 |
| <code>ChangePassword.BorderStyle=Groove</code> | ✅ 一致 |
| <code>ChangePassword.BorderStyle=Inset</code> | ✅ 一致 |
| <code>ChangePassword.BorderStyle=None</code> | ✅ 一致 |
| <code>ChangePassword.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>ChangePassword.BorderStyle=Outset</code> | ✅ 一致 |
| <code>ChangePassword.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>ChangePassword.BorderStyle=Solid</code> | ✅ 一致 |
| <code>ChangePassword.BorderWidth=120px</code> | ✅ 一致 |
| <code>ChangePassword.BorderWidth=2em</code> | ✅ 一致 |
| <code>ChangePassword.BorderWidth=50%</code> | ✅ 一致 |
| <code>ChangePassword.CancelButtonImageUrl=app-relative</code> | ✅ 一致 |
| <code>ChangePassword.CancelButtonImageUrl=empty</code> | ✅ 一致 |
| <code>ChangePassword.CancelButtonImageUrl=japanese</code> | ✅ 一致 |
| <code>ChangePassword.CancelButtonImageUrl=markup</code> | ✅ 一致 |
| <code>ChangePassword.CancelButtonImageUrl=text</code> | ✅ 一致 |
| <code>ChangePassword.CancelButtonStyle.BackColor=Red</code> | ✅ 一致 |
| <code>ChangePassword.CancelButtonStyle.CssClass=text</code> | ✅ 一致 |
| <code>ChangePassword.CancelButtonStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>ChangePassword.CancelButtonStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>ChangePassword.CancelButtonStyle.Height=120px</code> | ✅ 一致 |
| <code>ChangePassword.CancelButtonText=empty</code> | ✅ 一致 |
| <code>ChangePassword.CancelButtonText=japanese</code> | ✅ 一致 |
| <code>ChangePassword.CancelButtonText=markup</code> | ✅ 一致 |
| <code>ChangePassword.CancelButtonText=text</code> | ✅ 一致 |
| <code>ChangePassword.CancelButtonType=Button</code> | ✅ 一致 |
| <code>ChangePassword.CancelButtonType=Image</code> | ✅ 一致 |
| <code>ChangePassword.CancelButtonType=Image+CancelButtonImageUrl=app-relative</code> | ✅ 一致 |
| <code>ChangePassword.CancelButtonType=Link</code> | ✅ 一致 |
| <code>ChangePassword.CancelDestinationPageUrl=app-relative</code> | ✅ 一致 |
| <code>ChangePassword.CancelDestinationPageUrl=empty</code> | ✅ 一致 |
| <code>ChangePassword.CancelDestinationPageUrl=japanese</code> | ✅ 一致 |
| <code>ChangePassword.CancelDestinationPageUrl=markup</code> | ✅ 一致 |
| <code>ChangePassword.CancelDestinationPageUrl=text</code> | ✅ 一致 |
| <code>ChangePassword.ChangePasswordButtonImageUrl=app-relative</code> | ✅ 一致 |
| <code>ChangePassword.ChangePasswordButtonImageUrl=empty</code> | ✅ 一致 |
| <code>ChangePassword.ChangePasswordButtonImageUrl=japanese</code> | ✅ 一致 |
| <code>ChangePassword.ChangePasswordButtonImageUrl=markup</code> | ✅ 一致 |
| <code>ChangePassword.ChangePasswordButtonImageUrl=text</code> | ✅ 一致 |
| <code>ChangePassword.ChangePasswordButtonStyle.BackColor=Red</code> | ✅ 一致 |
| <code>ChangePassword.ChangePasswordButtonStyle.CssClass=text</code> | ✅ 一致 |
| <code>ChangePassword.ChangePasswordButtonStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>ChangePassword.ChangePasswordButtonStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>ChangePassword.ChangePasswordButtonStyle.Height=120px</code> | ✅ 一致 |
| <code>ChangePassword.ChangePasswordButtonText=empty</code> | ✅ 一致 |
| <code>ChangePassword.ChangePasswordButtonText=japanese</code> | ✅ 一致 |
| <code>ChangePassword.ChangePasswordButtonText=markup</code> | ✅ 一致 |
| <code>ChangePassword.ChangePasswordButtonText=text</code> | ✅ 一致 |
| <code>ChangePassword.ChangePasswordButtonType=Button</code> | ✅ 一致 |
| <code>ChangePassword.ChangePasswordButtonType=Image</code> | ✅ 一致 |
| <code>ChangePassword.ChangePasswordButtonType=Link</code> | ✅ 一致 |
| <code>ChangePassword.ChangePasswordButtonType=Link+ChangePasswordButtonText=markup</code> | ✅ 一致 |
| <code>ChangePassword.ChangePasswordFailureText=empty</code> | ✅ 一致 |
| <code>ChangePassword.ChangePasswordFailureText=japanese</code> | ✅ 一致 |
| <code>ChangePassword.ChangePasswordFailureText=markup</code> | ✅ 一致 |
| <code>ChangePassword.ChangePasswordFailureText=text</code> | ✅ 一致 |
| <code>ChangePassword.ChangePasswordTitleText=empty</code> | ✅ 一致 |
| <code>ChangePassword.ChangePasswordTitleText=japanese</code> | ✅ 一致 |
| <code>ChangePassword.ChangePasswordTitleText=markup</code> | ✅ 一致 |
| <code>ChangePassword.ChangePasswordTitleText=markup+InstructionText=markup</code> | ✅ 一致 |
| <code>ChangePassword.ChangePasswordTitleText=text</code> | ✅ 一致 |
| <code>ChangePassword.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ChangePassword.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ChangePassword.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ChangePassword.ClientIDMode=Static</code> | ✅ 一致 |
| <code>ChangePassword.ConfirmNewPasswordLabelText=empty</code> | ✅ 一致 |
| <code>ChangePassword.ConfirmNewPasswordLabelText=japanese</code> | ✅ 一致 |
| <code>ChangePassword.ConfirmNewPasswordLabelText=markup</code> | ✅ 一致 |
| <code>ChangePassword.ConfirmNewPasswordLabelText=text</code> | ✅ 一致 |
| <code>ChangePassword.ConfirmPasswordCompareErrorMessage=empty</code> | ✅ 一致 |
| <code>ChangePassword.ConfirmPasswordCompareErrorMessage=japanese</code> | ✅ 一致 |
| <code>ChangePassword.ConfirmPasswordCompareErrorMessage=markup</code> | ✅ 一致 |
| <code>ChangePassword.ConfirmPasswordCompareErrorMessage=text</code> | ✅ 一致 |
| <code>ChangePassword.ConfirmPasswordRequiredErrorMessage=empty</code> | ✅ 一致 |
| <code>ChangePassword.ConfirmPasswordRequiredErrorMessage=japanese</code> | ✅ 一致 |
| <code>ChangePassword.ConfirmPasswordRequiredErrorMessage=markup</code> | ✅ 一致 |
| <code>ChangePassword.ConfirmPasswordRequiredErrorMessage=text</code> | ✅ 一致 |
| <code>ChangePassword.ContinueButtonImageUrl=app-relative</code> | ✅ 一致 |
| <code>ChangePassword.ContinueButtonImageUrl=empty</code> | ✅ 一致 |
| <code>ChangePassword.ContinueButtonImageUrl=japanese</code> | ✅ 一致 |
| <code>ChangePassword.ContinueButtonImageUrl=markup</code> | ✅ 一致 |
| <code>ChangePassword.ContinueButtonImageUrl=text</code> | ✅ 一致 |
| <code>ChangePassword.ContinueButtonStyle.BackColor=Red</code> | ✅ 一致 |
| <code>ChangePassword.ContinueButtonStyle.CssClass=text</code> | ✅ 一致 |
| <code>ChangePassword.ContinueButtonStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>ChangePassword.ContinueButtonStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>ChangePassword.ContinueButtonStyle.Height=120px</code> | ✅ 一致 |
| <code>ChangePassword.ContinueButtonText=empty</code> | ✅ 一致 |
| <code>ChangePassword.ContinueButtonText=japanese</code> | ✅ 一致 |
| <code>ChangePassword.ContinueButtonText=markup</code> | ✅ 一致 |
| <code>ChangePassword.ContinueButtonText=text</code> | ✅ 一致 |
| <code>ChangePassword.ContinueButtonType=Button</code> | ✅ 一致 |
| <code>ChangePassword.ContinueButtonType=Image</code> | ✅ 一致 |
| <code>ChangePassword.ContinueButtonType=Link</code> | ✅ 一致 |
| <code>ChangePassword.ContinueDestinationPageUrl=app-relative</code> | ✅ 一致 |
| <code>ChangePassword.ContinueDestinationPageUrl=empty</code> | ✅ 一致 |
| <code>ChangePassword.ContinueDestinationPageUrl=japanese</code> | ✅ 一致 |
| <code>ChangePassword.ContinueDestinationPageUrl=markup</code> | ✅ 一致 |
| <code>ChangePassword.ContinueDestinationPageUrl=text</code> | ✅ 一致 |
| <code>ChangePassword.CreateUserIconUrl=app-relative</code> | ✅ 一致 |
| <code>ChangePassword.CreateUserIconUrl=empty</code> | ✅ 一致 |
| <code>ChangePassword.CreateUserIconUrl=japanese</code> | ✅ 一致 |
| <code>ChangePassword.CreateUserIconUrl=markup</code> | ✅ 一致 |
| <code>ChangePassword.CreateUserIconUrl=text</code> | ✅ 一致 |
| <code>ChangePassword.CreateUserText=empty</code> | ✅ 一致 |
| <code>ChangePassword.CreateUserText=japanese</code> | ✅ 一致 |
| <code>ChangePassword.CreateUserText=markup</code> | ✅ 一致 |
| <code>ChangePassword.CreateUserText=text</code> | ✅ 一致 |
| <code>ChangePassword.CreateUserText=text+CreateUserUrl=app-relative</code> | ✅ 一致 |
| <code>ChangePassword.CreateUserUrl=app-relative</code> | ✅ 一致 |
| <code>ChangePassword.CreateUserUrl=empty</code> | ✅ 一致 |
| <code>ChangePassword.CreateUserUrl=japanese</code> | ✅ 一致 |
| <code>ChangePassword.CreateUserUrl=markup</code> | ✅ 一致 |
| <code>ChangePassword.CreateUserUrl=text</code> | ✅ 一致 |
| <code>ChangePassword.CssClass=empty</code> | ✅ 一致 |
| <code>ChangePassword.CssClass=japanese</code> | ✅ 一致 |
| <code>ChangePassword.CssClass=markup</code> | ✅ 一致 |
| <code>ChangePassword.CssClass=text</code> | ✅ 一致 |
| <code>ChangePassword.DisplayUserName=false</code> | ✅ 一致 |
| <code>ChangePassword.DisplayUserName=true</code> | ✅ 一致 |
| <code>ChangePassword.DisplayUserName=true+EnableViewState=false</code> | ✅ 一致 |
| <code>ChangePassword.DisplayUserName=true+UserName=text</code> | ✅ 一致 |
| <code>ChangePassword.DisplayUserName=true+Visible=false</code> | ✅ 一致 |
| <code>ChangePassword.EditProfileIconUrl=app-relative</code> | ✅ 一致 |
| <code>ChangePassword.EditProfileIconUrl=empty</code> | ✅ 一致 |
| <code>ChangePassword.EditProfileIconUrl=japanese</code> | ✅ 一致 |
| <code>ChangePassword.EditProfileIconUrl=markup</code> | ✅ 一致 |
| <code>ChangePassword.EditProfileIconUrl=text</code> | ✅ 一致 |
| <code>ChangePassword.EditProfileText=empty</code> | ✅ 一致 |
| <code>ChangePassword.EditProfileText=japanese</code> | ✅ 一致 |
| <code>ChangePassword.EditProfileText=markup</code> | ✅ 一致 |
| <code>ChangePassword.EditProfileText=text</code> | ✅ 一致 |
| <code>ChangePassword.EditProfileUrl=app-relative</code> | ✅ 一致 |
| <code>ChangePassword.EditProfileUrl=empty</code> | ✅ 一致 |
| <code>ChangePassword.EditProfileUrl=japanese</code> | ✅ 一致 |
| <code>ChangePassword.EditProfileUrl=markup</code> | ✅ 一致 |
| <code>ChangePassword.EditProfileUrl=text</code> | ✅ 一致 |
| <code>ChangePassword.EnableTheming=false</code> | ✅ 一致 |
| <code>ChangePassword.EnableTheming=true</code> | ✅ 一致 |
| <code>ChangePassword.EnableViewState=false</code> | ✅ 一致 |
| <code>ChangePassword.EnableViewState=true</code> | ✅ 一致 |
| <code>ChangePassword.Enabled=false</code> | ✅ 一致 |
| <code>ChangePassword.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>ChangePassword.Enabled=true</code> | ✅ 一致 |
| <code>ChangePassword.FailureTextStyle.BackColor=Red</code> | ✅ 一致 |
| <code>ChangePassword.FailureTextStyle.CssClass=text</code> | ✅ 一致 |
| <code>ChangePassword.FailureTextStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>ChangePassword.FailureTextStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>ChangePassword.FailureTextStyle.Height=120px</code> | ✅ 一致 |
| <code>ChangePassword.FailureTextStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>ChangePassword.FailureTextStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>ChangePassword.FailureTextStyle.Wrap=true</code> | ✅ 一致 |
| <code>ChangePassword.Font.Bold=false</code> | ✅ 一致 |
| <code>ChangePassword.Font.Bold=true</code> | ✅ 一致 |
| <code>ChangePassword.Font.Italic=false</code> | ✅ 一致 |
| <code>ChangePassword.Font.Italic=true</code> | ✅ 一致 |
| <code>ChangePassword.Font.Name=empty</code> | ✅ 一致 |
| <code>ChangePassword.Font.Name=japanese</code> | ✅ 一致 |
| <code>ChangePassword.Font.Name=markup</code> | ✅ 一致 |
| <code>ChangePassword.Font.Name=text</code> | ✅ 一致 |
| <code>ChangePassword.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>ChangePassword.Font.Overline=false</code> | ✅ 一致 |
| <code>ChangePassword.Font.Overline=true</code> | ✅ 一致 |
| <code>ChangePassword.Font.Size=12pt</code> | ✅ 一致 |
| <code>ChangePassword.Font.Size=150%</code> | ✅ 一致 |
| <code>ChangePassword.Font.Size=Large</code> | ✅ 一致 |
| <code>ChangePassword.Font.Strikeout=false</code> | ✅ 一致 |
| <code>ChangePassword.Font.Strikeout=true</code> | ✅ 一致 |
| <code>ChangePassword.Font.Underline=false</code> | ✅ 一致 |
| <code>ChangePassword.Font.Underline=true</code> | ✅ 一致 |
| <code>ChangePassword.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>ChangePassword.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>ChangePassword.ForeColor=Red</code> | ✅ 一致 |
| <code>ChangePassword.ForeColor=Transparent</code> | ✅ 一致 |
| <code>ChangePassword.Height=120px</code> | ✅ 一致 |
| <code>ChangePassword.Height=2em</code> | ✅ 一致 |
| <code>ChangePassword.Height=50%</code> | ✅ 一致 |
| <code>ChangePassword.HelpPageIconUrl=app-relative</code> | ✅ 一致 |
| <code>ChangePassword.HelpPageIconUrl=empty</code> | ✅ 一致 |
| <code>ChangePassword.HelpPageIconUrl=japanese</code> | ✅ 一致 |
| <code>ChangePassword.HelpPageIconUrl=markup</code> | ✅ 一致 |
| <code>ChangePassword.HelpPageIconUrl=text</code> | ✅ 一致 |
| <code>ChangePassword.HelpPageText=empty</code> | ✅ 一致 |
| <code>ChangePassword.HelpPageText=japanese</code> | ✅ 一致 |
| <code>ChangePassword.HelpPageText=markup</code> | ✅ 一致 |
| <code>ChangePassword.HelpPageText=text</code> | ✅ 一致 |
| <code>ChangePassword.HelpPageText=text+HelpPageUrl=app-relative</code> | ✅ 一致 |
| <code>ChangePassword.HelpPageUrl=app-relative</code> | ✅ 一致 |
| <code>ChangePassword.HelpPageUrl=empty</code> | ✅ 一致 |
| <code>ChangePassword.HelpPageUrl=japanese</code> | ✅ 一致 |
| <code>ChangePassword.HelpPageUrl=markup</code> | ✅ 一致 |
| <code>ChangePassword.HelpPageUrl=text</code> | ✅ 一致 |
| <code>ChangePassword.HyperLinkStyle.BackColor=Red</code> | ✅ 一致 |
| <code>ChangePassword.HyperLinkStyle.CssClass=text</code> | ✅ 一致 |
| <code>ChangePassword.HyperLinkStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>ChangePassword.HyperLinkStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>ChangePassword.HyperLinkStyle.Height=120px</code> | ✅ 一致 |
| <code>ChangePassword.HyperLinkStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>ChangePassword.HyperLinkStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>ChangePassword.HyperLinkStyle.Wrap=true</code> | ✅ 一致 |
| <code>ChangePassword.ID=empty</code> | ✅ 一致 |
| <code>ChangePassword.ID=japanese</code> | ✅ 一致 |
| <code>ChangePassword.ID=markup</code> | ✅ 一致 |
| <code>ChangePassword.ID=text</code> | ✅ 一致 |
| <code>ChangePassword.InstructionText=empty</code> | ✅ 一致 |
| <code>ChangePassword.InstructionText=japanese</code> | ✅ 一致 |
| <code>ChangePassword.InstructionText=markup</code> | ✅ 一致 |
| <code>ChangePassword.InstructionText=text</code> | ✅ 一致 |
| <code>ChangePassword.InstructionTextStyle.BackColor=Red</code> | ✅ 一致 |
| <code>ChangePassword.InstructionTextStyle.CssClass=text</code> | ✅ 一致 |
| <code>ChangePassword.InstructionTextStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>ChangePassword.InstructionTextStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>ChangePassword.InstructionTextStyle.Height=120px</code> | ✅ 一致 |
| <code>ChangePassword.InstructionTextStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>ChangePassword.InstructionTextStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>ChangePassword.InstructionTextStyle.Wrap=true</code> | ✅ 一致 |
| <code>ChangePassword.LabelStyle.BackColor=Red</code> | ✅ 一致 |
| <code>ChangePassword.LabelStyle.CssClass=text</code> | ✅ 一致 |
| <code>ChangePassword.LabelStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>ChangePassword.LabelStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>ChangePassword.LabelStyle.Height=120px</code> | ✅ 一致 |
| <code>ChangePassword.LabelStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>ChangePassword.LabelStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>ChangePassword.LabelStyle.Wrap=true</code> | ✅ 一致 |
| <code>ChangePassword.MembershipProvider=empty</code> | ✅ 一致 |
| <code>ChangePassword.MembershipProvider=japanese</code> | ✅ 一致 |
| <code>ChangePassword.MembershipProvider=markup</code> | ✅ 一致 |
| <code>ChangePassword.MembershipProvider=text</code> | ✅ 一致 |
| <code>ChangePassword.NewPasswordLabelText=empty</code> | ✅ 一致 |
| <code>ChangePassword.NewPasswordLabelText=japanese</code> | ✅ 一致 |
| <code>ChangePassword.NewPasswordLabelText=markup</code> | ✅ 一致 |
| <code>ChangePassword.NewPasswordLabelText=text</code> | ✅ 一致 |
| <code>ChangePassword.NewPasswordRegularExpression=empty</code> | ✅ 一致 |
| <code>ChangePassword.NewPasswordRegularExpression=japanese</code> | ✅ 一致 |
| <code>ChangePassword.NewPasswordRegularExpression=markup</code> | ✅ 一致 |
| <code>ChangePassword.NewPasswordRegularExpression=text</code> | ✅ 一致 |
| <code>ChangePassword.NewPasswordRegularExpressionErrorMessage=empty</code> | ✅ 一致 |
| <code>ChangePassword.NewPasswordRegularExpressionErrorMessage=japanese</code> | ✅ 一致 |
| <code>ChangePassword.NewPasswordRegularExpressionErrorMessage=markup</code> | ✅ 一致 |
| <code>ChangePassword.NewPasswordRegularExpressionErrorMessage=text</code> | ✅ 一致 |
| <code>ChangePassword.NewPasswordRequiredErrorMessage=empty</code> | ✅ 一致 |
| <code>ChangePassword.NewPasswordRequiredErrorMessage=japanese</code> | ✅ 一致 |
| <code>ChangePassword.NewPasswordRequiredErrorMessage=markup</code> | ✅ 一致 |
| <code>ChangePassword.NewPasswordRequiredErrorMessage=text</code> | ✅ 一致 |
| <code>ChangePassword.PasswordHintStyle.BackColor=Red</code> | ✅ 一致 |
| <code>ChangePassword.PasswordHintStyle.CssClass=text</code> | ✅ 一致 |
| <code>ChangePassword.PasswordHintStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>ChangePassword.PasswordHintStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>ChangePassword.PasswordHintStyle.Height=120px</code> | ✅ 一致 |
| <code>ChangePassword.PasswordHintStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>ChangePassword.PasswordHintStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>ChangePassword.PasswordHintStyle.Wrap=true</code> | ✅ 一致 |
| <code>ChangePassword.PasswordHintText=empty</code> | ✅ 一致 |
| <code>ChangePassword.PasswordHintText=japanese</code> | ✅ 一致 |
| <code>ChangePassword.PasswordHintText=markup</code> | ✅ 一致 |
| <code>ChangePassword.PasswordHintText=markup+NewPasswordRegularExpression=text</code> | ✅ 一致 |
| <code>ChangePassword.PasswordHintText=text</code> | ✅ 一致 |
| <code>ChangePassword.PasswordLabelText=empty</code> | ✅ 一致 |
| <code>ChangePassword.PasswordLabelText=japanese</code> | ✅ 一致 |
| <code>ChangePassword.PasswordLabelText=markup</code> | ✅ 一致 |
| <code>ChangePassword.PasswordLabelText=text</code> | ✅ 一致 |
| <code>ChangePassword.PasswordRecoveryIconUrl=app-relative</code> | ✅ 一致 |
| <code>ChangePassword.PasswordRecoveryIconUrl=empty</code> | ✅ 一致 |
| <code>ChangePassword.PasswordRecoveryIconUrl=japanese</code> | ✅ 一致 |
| <code>ChangePassword.PasswordRecoveryIconUrl=markup</code> | ✅ 一致 |
| <code>ChangePassword.PasswordRecoveryIconUrl=text</code> | ✅ 一致 |
| <code>ChangePassword.PasswordRecoveryText=empty</code> | ✅ 一致 |
| <code>ChangePassword.PasswordRecoveryText=japanese</code> | ✅ 一致 |
| <code>ChangePassword.PasswordRecoveryText=markup</code> | ✅ 一致 |
| <code>ChangePassword.PasswordRecoveryText=text</code> | ✅ 一致 |
| <code>ChangePassword.PasswordRecoveryText=text+PasswordRecoveryUrl=app-relative</code> | ✅ 一致 |
| <code>ChangePassword.PasswordRecoveryUrl=app-relative</code> | ✅ 一致 |
| <code>ChangePassword.PasswordRecoveryUrl=empty</code> | ✅ 一致 |
| <code>ChangePassword.PasswordRecoveryUrl=japanese</code> | ✅ 一致 |
| <code>ChangePassword.PasswordRecoveryUrl=markup</code> | ✅ 一致 |
| <code>ChangePassword.PasswordRecoveryUrl=text</code> | ✅ 一致 |
| <code>ChangePassword.PasswordRequiredErrorMessage=empty</code> | ✅ 一致 |
| <code>ChangePassword.PasswordRequiredErrorMessage=japanese</code> | ✅ 一致 |
| <code>ChangePassword.PasswordRequiredErrorMessage=markup</code> | ✅ 一致 |
| <code>ChangePassword.PasswordRequiredErrorMessage=text</code> | ✅ 一致 |
| <code>ChangePassword.RenderOuterTable=false</code> | ✅ 一致 |
| <code>ChangePassword.RenderOuterTable=false+DisplayUserName=true</code> | ✅ 一致 |
| <code>ChangePassword.RenderOuterTable=true</code> | ✅ 一致 |
| <code>ChangePassword.SkinID=empty</code> | ✅ 一致 |
| <code>ChangePassword.SkinID=japanese</code> | ✅ 一致 |
| <code>ChangePassword.SkinID=markup</code> | ✅ 一致 |
| <code>ChangePassword.SkinID=text</code> | ✅ 一致 |
| <code>ChangePassword.Style[color]=1em</code> | ✅ 一致 |
| <code>ChangePassword.Style[color]=red</code> | ✅ 一致 |
| <code>ChangePassword.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>ChangePassword.Style[margin-left]=red</code> | ✅ 一致 |
| <code>ChangePassword.SuccessPageUrl=app-relative</code> | ✅ 一致 |
| <code>ChangePassword.SuccessPageUrl=empty</code> | ✅ 一致 |
| <code>ChangePassword.SuccessPageUrl=japanese</code> | ✅ 一致 |
| <code>ChangePassword.SuccessPageUrl=markup</code> | ✅ 一致 |
| <code>ChangePassword.SuccessPageUrl=text</code> | ✅ 一致 |
| <code>ChangePassword.SuccessText=empty</code> | ✅ 一致 |
| <code>ChangePassword.SuccessText=japanese</code> | ✅ 一致 |
| <code>ChangePassword.SuccessText=markup</code> | ✅ 一致 |
| <code>ChangePassword.SuccessText=text</code> | ✅ 一致 |
| <code>ChangePassword.SuccessTextStyle.BackColor=Red</code> | ✅ 一致 |
| <code>ChangePassword.SuccessTextStyle.CssClass=text</code> | ✅ 一致 |
| <code>ChangePassword.SuccessTextStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>ChangePassword.SuccessTextStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>ChangePassword.SuccessTextStyle.Height=120px</code> | ✅ 一致 |
| <code>ChangePassword.SuccessTextStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>ChangePassword.SuccessTextStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>ChangePassword.SuccessTextStyle.Wrap=true</code> | ✅ 一致 |
| <code>ChangePassword.SuccessTitleText=empty</code> | ✅ 一致 |
| <code>ChangePassword.SuccessTitleText=japanese</code> | ✅ 一致 |
| <code>ChangePassword.SuccessTitleText=markup</code> | ✅ 一致 |
| <code>ChangePassword.SuccessTitleText=text</code> | ✅ 一致 |
| <code>ChangePassword.TabIndex=-1</code> | ✅ 一致 |
| <code>ChangePassword.TabIndex=0</code> | ✅ 一致 |
| <code>ChangePassword.TabIndex=3</code> | ✅ 一致 |
| <code>ChangePassword.TextBoxStyle.BackColor=Red</code> | ✅ 一致 |
| <code>ChangePassword.TextBoxStyle.BackColor=Red+TitleTextStyle.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>ChangePassword.TextBoxStyle.CssClass=text</code> | ✅ 一致 |
| <code>ChangePassword.TextBoxStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>ChangePassword.TextBoxStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>ChangePassword.TextBoxStyle.Height=120px</code> | ✅ 一致 |
| <code>ChangePassword.TitleTextStyle.BackColor=Red</code> | ✅ 一致 |
| <code>ChangePassword.TitleTextStyle.CssClass=text</code> | ✅ 一致 |
| <code>ChangePassword.TitleTextStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>ChangePassword.TitleTextStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>ChangePassword.TitleTextStyle.Height=120px</code> | ✅ 一致 |
| <code>ChangePassword.TitleTextStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>ChangePassword.TitleTextStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>ChangePassword.TitleTextStyle.Wrap=true</code> | ✅ 一致 |
| <code>ChangePassword.ToolTip=empty</code> | ✅ 一致 |
| <code>ChangePassword.ToolTip=japanese</code> | ✅ 一致 |
| <code>ChangePassword.ToolTip=markup</code> | ✅ 一致 |
| <code>ChangePassword.ToolTip=text</code> | ✅ 一致 |
| <code>ChangePassword.UserName=empty</code> | ✅ 一致 |
| <code>ChangePassword.UserName=japanese</code> | ✅ 一致 |
| <code>ChangePassword.UserName=markup</code> | ✅ 一致 |
| <code>ChangePassword.UserName=text</code> | ✅ 一致 |
| <code>ChangePassword.UserNameLabelText=empty</code> | ✅ 一致 |
| <code>ChangePassword.UserNameLabelText=japanese</code> | ✅ 一致 |
| <code>ChangePassword.UserNameLabelText=markup</code> | ✅ 一致 |
| <code>ChangePassword.UserNameLabelText=text</code> | ✅ 一致 |
| <code>ChangePassword.UserNameRequiredErrorMessage=empty</code> | ✅ 一致 |
| <code>ChangePassword.UserNameRequiredErrorMessage=japanese</code> | ✅ 一致 |
| <code>ChangePassword.UserNameRequiredErrorMessage=markup</code> | ✅ 一致 |
| <code>ChangePassword.UserNameRequiredErrorMessage=text</code> | ✅ 一致 |
| <code>ChangePassword.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>ChangePassword.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>ChangePassword.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>ChangePassword.ValidatorTextStyle.BackColor=Red</code> | ✅ 一致 |
| <code>ChangePassword.ValidatorTextStyle.CssClass=text</code> | ✅ 一致 |
| <code>ChangePassword.ValidatorTextStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>ChangePassword.ValidatorTextStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>ChangePassword.ValidatorTextStyle.Height=120px</code> | ✅ 一致 |
| <code>ChangePassword.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>ChangePassword.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>ChangePassword.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>ChangePassword.Visible=false</code> | ✅ 一致 |
| <code>ChangePassword.Visible=true</code> | ✅ 一致 |
| <code>ChangePassword.Width=120px</code> | ✅ 一致 |
| <code>ChangePassword.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>ChangePassword.Width=2em</code> | ✅ 一致 |
| <code>ChangePassword.Width=50%</code> | ✅ 一致 |
| <code>ChangePassword[logged in].(default)</code> | ✅ 一致 |
| <code>ChangePassword[logged in].DisplayUserName=true</code> | ✅ 一致 |
| <code>ChangePassword[naming container].(default)</code> | ✅ 一致 |
| <code>ChangePassword[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ChangePassword[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ChangePassword[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ChangePassword[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>ChangePassword[naming container].DisplayUserName=true</code> | ✅ 一致 |
| <code>ChangePassword[naming container].ID=text</code> | ✅ 一致 |
| <code>ChangePassword[templates].(default)</code> | ✅ 一致 |
| <code>ChangePassword[themed].(default)</code> | ✅ 一致 |
| <code>ChangePassword[themed].CssClass=text</code> | ✅ 一致 |
| <code>ChangePassword[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>ChangePassword[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| ChangePassword [render][databind][dynamic][templates] first request, anonymous: the login forms, the status, the views, the wizards | ✅ 一致 |
| ChangePassword [viewstate] a login with a wrong password: its error | ✅ 一致 |
| ChangePassword [viewstate] logged in (redirected to the page): the name, the status, the role&#39;s view | ✅ 一致 |
| ChangePassword [viewstate][templates] the password changed: the success template | ✅ 一致 |
| ChangePassword [viewstate] logged out by the status | ✅ 一致 |
| ChangePassword [viewstate] a recovery for a user who does not exist | ✅ 一致 |
| ChangePassword [viewstate] the user&#39;s question | ✅ 一致 |
| ChangePassword [viewstate] the answer right: the password reset, its mail prepared (cancelled), the success template | ✅ 一致 |
| ChangePassword [viewstate] a user created: the complete step | ✅ 一致 |
| ChangePassword [viewstate] the wizard&#39;s next step (the templated one) | ✅ 一致 |
| ChangePassword [viewstate] next, then previous | ✅ 一致 |
| ChangePassword [viewstate] a step chosen in the side bar | ✅ 一致 |
| ChangePassword [viewstate] finished: the complete step, the first step&#39;s value kept | ✅ 一致 |
| ChangePassword [dynamic][viewstate] a wizard made in code: its next step | ✅ 一致 |
| ChangePassword [visible][viewstate] a hidden wizard shown | ✅ 一致 |

</details>
