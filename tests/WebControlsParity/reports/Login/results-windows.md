# Login パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-16 16:46
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 212 | 212 | 0 | 0 |
| ② HTML | 312 | 312 | 0 | 0 |
| ③ シナリオ | 15 | 15 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 221 件中 221 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 123 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 15 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 295 | 1 | ✅ |
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
| <code>Login.Api.Control.#ctor</code> | ✅ 一致 |
| <code>Login.Api.Control.Adapter</code> | ✅ 一致 |
| <code>Login.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>Login.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>Login.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>Login.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>Login.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>Login.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>Login.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>Login.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>Login.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>Login.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>Login.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>Login.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>Login.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>Login.Api.Control.ClientID</code> | ✅ 一致 |
| <code>Login.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>Login.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>Login.Api.Control.Context</code> | ✅ 一致 |
| <code>Login.Api.Control.Controls</code> | ✅ 一致 |
| <code>Login.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>Login.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>Login.Api.Control.DataBind</code> | ✅ 一致 |
| <code>Login.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>Login.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>Login.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>Login.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>Login.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>Login.Api.Control.Dispose</code> | ✅ 一致 |
| <code>Login.Api.Control.Disposed</code> | ✅ 一致 |
| <code>Login.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>Login.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>Login.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>Login.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>Login.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>Login.Api.Control.Events</code> | ✅ 一致 |
| <code>Login.Api.Control.FindControl</code> | ✅ 一致 |
| <code>Login.Api.Control.Focus</code> | ✅ 一致 |
| <code>Login.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>Login.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>Login.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>Login.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>Login.Api.Control.HasControls</code> | ✅ 一致 |
| <code>Login.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>Login.Api.Control.ID</code> | ✅ 一致 |
| <code>Login.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>Login.Api.Control.Init</code> | ✅ 一致 |
| <code>Login.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>Login.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>Login.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>Login.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>Login.Api.Control.Load</code> | ✅ 一致 |
| <code>Login.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>Login.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>Login.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>Login.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>Login.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>Login.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>Login.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>Login.Api.Control.OnInit</code> | ✅ 一致 |
| <code>Login.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>Login.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>Login.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>Login.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>Login.Api.Control.Page</code> | ✅ 一致 |
| <code>Login.Api.Control.Parent</code> | ✅ 一致 |
| <code>Login.Api.Control.PreRender</code> | ✅ 一致 |
| <code>Login.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>Login.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>Login.Api.Control.Render</code> | ✅ 一致 |
| <code>Login.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>Login.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>Login.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>Login.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>Login.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>Login.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>Login.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>Login.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>Login.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>Login.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>Login.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>Login.Api.Control.Site</code> | ✅ 一致 |
| <code>Login.Api.Control.SkinID</code> | ✅ 一致 |
| <code>Login.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>Login.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>Login.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>Login.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>Login.Api.Control.Unload</code> | ✅ 一致 |
| <code>Login.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>Login.Api.Control.ViewState</code> | ✅ 一致 |
| <code>Login.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>Login.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>Login.Api.Control.Visible</code> | ✅ 一致 |
| <code>Login.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>Login.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>Login.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>Login.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>Login.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>Login.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>Login.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>Login.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>Login.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>Login.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>Login.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>Login.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>Login.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>Login.Api.WebControls.CompositeControl.#ctor</code> | ✅ 一致 |
| <code>Login.Api.WebControls.CompositeControl.Controls</code> | ✅ 一致 |
| <code>Login.Api.WebControls.CompositeControl.DataBind</code> | ✅ 一致 |
| <code>Login.Api.WebControls.CompositeControl.RecreateChildControls</code> | ✅ 一致 |
| <code>Login.Api.WebControls.CompositeControl.Render</code> | ✅ 一致 |
| <code>Login.Api.WebControls.CompositeControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>Login.Api.WebControls.ICompositeControlDesignerAccessor.RecreateChildControls</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.#ctor</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.Authenticate</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.BorderPadding</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.CheckBoxStyle</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.CreateChildControls</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.CreateUserIconUrl</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.CreateUserText</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.CreateUserUrl</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.DestinationPageUrl</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.DisplayRememberMe</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.FailureAction</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.FailureText</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.FailureTextStyle</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.HelpPageIconUrl</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.HelpPageText</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.HelpPageUrl</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.HyperLinkStyle</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.InstructionText</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.InstructionTextStyle</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.LabelStyle</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.LayoutTemplate</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.LoadViewState</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.LoggedIn</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.LoggingIn</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.LoginButtonCommandName</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.LoginButtonImageUrl</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.LoginButtonStyle</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.LoginButtonText</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.LoginButtonType</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.LoginError</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.MembershipProvider</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.OnAuthenticate</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.OnBubbleEvent</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.OnLoggedIn</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.OnLoggingIn</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.OnLoginError</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.OnPreRender</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.Orientation</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.Password</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.PasswordLabelText</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.PasswordRecoveryIconUrl</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.PasswordRecoveryText</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.PasswordRecoveryUrl</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.PasswordRequiredErrorMessage</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.RememberMeSet</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.RememberMeText</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.Render</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.RenderOuterTable</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.SaveViewState</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.SetDesignModeState</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.TagKey</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.TextBoxStyle</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.TextLayout</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.TitleText</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.TitleTextStyle</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.TrackViewState</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.UserName</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.UserNameLabelText</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.UserNameRequiredErrorMessage</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.ValidatorTextStyle</code> | ✅ 一致 |
| <code>Login.Api.WebControls.Login.VisibleWhenLoggedIn</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>Login.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>Login.(case list)</code> | ✅ 一致 |
| <code>Login.(default)</code> | ✅ 一致 |
| <code>Login.AccessKey=empty</code> | ✅ 一致 |
| <code>Login.AccessKey=japanese</code> | ✅ 一致 |
| <code>Login.AccessKey=markup</code> | ✅ 一致 |
| <code>Login.AccessKey=text</code> | ✅ 一致 |
| <code>Login.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>Login.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>Login.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>Login.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>Login.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>Login.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>Login.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>Login.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>Login.BackColor=#3366CC</code> | ✅ 一致 |
| <code>Login.BackColor=Red</code> | ✅ 一致 |
| <code>Login.BackColor=Transparent</code> | ✅ 一致 |
| <code>Login.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>Login.BorderColor=Red</code> | ✅ 一致 |
| <code>Login.BorderColor=Transparent</code> | ✅ 一致 |
| <code>Login.BorderPadding=-1</code> | ✅ 一致 |
| <code>Login.BorderPadding=0</code> | ✅ 一致 |
| <code>Login.BorderPadding=3</code> | ✅ 一致 |
| <code>Login.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>Login.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>Login.BorderStyle=Double</code> | ✅ 一致 |
| <code>Login.BorderStyle=Groove</code> | ✅ 一致 |
| <code>Login.BorderStyle=Inset</code> | ✅ 一致 |
| <code>Login.BorderStyle=None</code> | ✅ 一致 |
| <code>Login.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>Login.BorderStyle=Outset</code> | ✅ 一致 |
| <code>Login.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>Login.BorderStyle=Solid</code> | ✅ 一致 |
| <code>Login.BorderWidth=120px</code> | ✅ 一致 |
| <code>Login.BorderWidth=2em</code> | ✅ 一致 |
| <code>Login.BorderWidth=50%</code> | ✅ 一致 |
| <code>Login.CheckBoxStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Login.CheckBoxStyle.CssClass=text</code> | ✅ 一致 |
| <code>Login.CheckBoxStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Login.CheckBoxStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Login.CheckBoxStyle.Height=120px</code> | ✅ 一致 |
| <code>Login.CheckBoxStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>Login.CheckBoxStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>Login.CheckBoxStyle.Wrap=true</code> | ✅ 一致 |
| <code>Login.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Login.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Login.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Login.ClientIDMode=Static</code> | ✅ 一致 |
| <code>Login.CreateUserIconUrl=app-relative</code> | ✅ 一致 |
| <code>Login.CreateUserIconUrl=empty</code> | ✅ 一致 |
| <code>Login.CreateUserIconUrl=japanese</code> | ✅ 一致 |
| <code>Login.CreateUserIconUrl=markup</code> | ✅ 一致 |
| <code>Login.CreateUserIconUrl=text</code> | ✅ 一致 |
| <code>Login.CreateUserText=empty</code> | ✅ 一致 |
| <code>Login.CreateUserText=japanese</code> | ✅ 一致 |
| <code>Login.CreateUserText=markup</code> | ✅ 一致 |
| <code>Login.CreateUserText=text</code> | ✅ 一致 |
| <code>Login.CreateUserText=text+CreateUserUrl=app-relative</code> | ✅ 一致 |
| <code>Login.CreateUserUrl=app-relative</code> | ✅ 一致 |
| <code>Login.CreateUserUrl=empty</code> | ✅ 一致 |
| <code>Login.CreateUserUrl=japanese</code> | ✅ 一致 |
| <code>Login.CreateUserUrl=markup</code> | ✅ 一致 |
| <code>Login.CreateUserUrl=text</code> | ✅ 一致 |
| <code>Login.CssClass=empty</code> | ✅ 一致 |
| <code>Login.CssClass=japanese</code> | ✅ 一致 |
| <code>Login.CssClass=markup</code> | ✅ 一致 |
| <code>Login.CssClass=text</code> | ✅ 一致 |
| <code>Login.DestinationPageUrl=app-relative</code> | ✅ 一致 |
| <code>Login.DestinationPageUrl=empty</code> | ✅ 一致 |
| <code>Login.DestinationPageUrl=japanese</code> | ✅ 一致 |
| <code>Login.DestinationPageUrl=markup</code> | ✅ 一致 |
| <code>Login.DestinationPageUrl=text</code> | ✅ 一致 |
| <code>Login.DisplayRememberMe=false</code> | ✅ 一致 |
| <code>Login.DisplayRememberMe=false+RememberMeSet=true</code> | ✅ 一致 |
| <code>Login.DisplayRememberMe=true</code> | ✅ 一致 |
| <code>Login.EnableTheming=false</code> | ✅ 一致 |
| <code>Login.EnableTheming=true</code> | ✅ 一致 |
| <code>Login.EnableViewState=false</code> | ✅ 一致 |
| <code>Login.EnableViewState=true</code> | ✅ 一致 |
| <code>Login.Enabled=false</code> | ✅ 一致 |
| <code>Login.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>Login.Enabled=true</code> | ✅ 一致 |
| <code>Login.FailureAction=RedirectToLoginPage</code> | ✅ 一致 |
| <code>Login.FailureAction=Refresh</code> | ✅ 一致 |
| <code>Login.FailureText=empty</code> | ✅ 一致 |
| <code>Login.FailureText=japanese</code> | ✅ 一致 |
| <code>Login.FailureText=markup</code> | ✅ 一致 |
| <code>Login.FailureText=text</code> | ✅ 一致 |
| <code>Login.FailureTextStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Login.FailureTextStyle.CssClass=text</code> | ✅ 一致 |
| <code>Login.FailureTextStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Login.FailureTextStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Login.FailureTextStyle.Height=120px</code> | ✅ 一致 |
| <code>Login.FailureTextStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>Login.FailureTextStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>Login.FailureTextStyle.Wrap=true</code> | ✅ 一致 |
| <code>Login.Font.Bold=false</code> | ✅ 一致 |
| <code>Login.Font.Bold=true</code> | ✅ 一致 |
| <code>Login.Font.Italic=false</code> | ✅ 一致 |
| <code>Login.Font.Italic=true</code> | ✅ 一致 |
| <code>Login.Font.Name=empty</code> | ✅ 一致 |
| <code>Login.Font.Name=japanese</code> | ✅ 一致 |
| <code>Login.Font.Name=markup</code> | ✅ 一致 |
| <code>Login.Font.Name=text</code> | ✅ 一致 |
| <code>Login.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>Login.Font.Overline=false</code> | ✅ 一致 |
| <code>Login.Font.Overline=true</code> | ✅ 一致 |
| <code>Login.Font.Size=12pt</code> | ✅ 一致 |
| <code>Login.Font.Size=150%</code> | ✅ 一致 |
| <code>Login.Font.Size=Large</code> | ✅ 一致 |
| <code>Login.Font.Strikeout=false</code> | ✅ 一致 |
| <code>Login.Font.Strikeout=true</code> | ✅ 一致 |
| <code>Login.Font.Underline=false</code> | ✅ 一致 |
| <code>Login.Font.Underline=true</code> | ✅ 一致 |
| <code>Login.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>Login.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>Login.ForeColor=Red</code> | ✅ 一致 |
| <code>Login.ForeColor=Transparent</code> | ✅ 一致 |
| <code>Login.Height=120px</code> | ✅ 一致 |
| <code>Login.Height=2em</code> | ✅ 一致 |
| <code>Login.Height=50%</code> | ✅ 一致 |
| <code>Login.HelpPageIconUrl=app-relative</code> | ✅ 一致 |
| <code>Login.HelpPageIconUrl=empty</code> | ✅ 一致 |
| <code>Login.HelpPageIconUrl=japanese</code> | ✅ 一致 |
| <code>Login.HelpPageIconUrl=markup</code> | ✅ 一致 |
| <code>Login.HelpPageIconUrl=text</code> | ✅ 一致 |
| <code>Login.HelpPageText=empty</code> | ✅ 一致 |
| <code>Login.HelpPageText=japanese</code> | ✅ 一致 |
| <code>Login.HelpPageText=markup</code> | ✅ 一致 |
| <code>Login.HelpPageText=text</code> | ✅ 一致 |
| <code>Login.HelpPageText=text+HelpPageUrl=app-relative+HelpPageIconUrl=app-relative</code> | ✅ 一致 |
| <code>Login.HelpPageUrl=app-relative</code> | ✅ 一致 |
| <code>Login.HelpPageUrl=empty</code> | ✅ 一致 |
| <code>Login.HelpPageUrl=japanese</code> | ✅ 一致 |
| <code>Login.HelpPageUrl=markup</code> | ✅ 一致 |
| <code>Login.HelpPageUrl=text</code> | ✅ 一致 |
| <code>Login.HyperLinkStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Login.HyperLinkStyle.CssClass=text</code> | ✅ 一致 |
| <code>Login.HyperLinkStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Login.HyperLinkStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Login.HyperLinkStyle.Height=120px</code> | ✅ 一致 |
| <code>Login.HyperLinkStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>Login.HyperLinkStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>Login.HyperLinkStyle.Wrap=true</code> | ✅ 一致 |
| <code>Login.ID=empty</code> | ✅ 一致 |
| <code>Login.ID=japanese</code> | ✅ 一致 |
| <code>Login.ID=markup</code> | ✅ 一致 |
| <code>Login.ID=text</code> | ✅ 一致 |
| <code>Login.InstructionText=empty</code> | ✅ 一致 |
| <code>Login.InstructionText=japanese</code> | ✅ 一致 |
| <code>Login.InstructionText=markup</code> | ✅ 一致 |
| <code>Login.InstructionText=text</code> | ✅ 一致 |
| <code>Login.InstructionTextStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Login.InstructionTextStyle.CssClass=text</code> | ✅ 一致 |
| <code>Login.InstructionTextStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Login.InstructionTextStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Login.InstructionTextStyle.Height=120px</code> | ✅ 一致 |
| <code>Login.InstructionTextStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>Login.InstructionTextStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>Login.InstructionTextStyle.Wrap=true</code> | ✅ 一致 |
| <code>Login.LabelStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Login.LabelStyle.CssClass=text</code> | ✅ 一致 |
| <code>Login.LabelStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Login.LabelStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Login.LabelStyle.Height=120px</code> | ✅ 一致 |
| <code>Login.LabelStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>Login.LabelStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>Login.LabelStyle.Wrap=true</code> | ✅ 一致 |
| <code>Login.LoginButtonImageUrl=app-relative</code> | ✅ 一致 |
| <code>Login.LoginButtonImageUrl=empty</code> | ✅ 一致 |
| <code>Login.LoginButtonImageUrl=japanese</code> | ✅ 一致 |
| <code>Login.LoginButtonImageUrl=markup</code> | ✅ 一致 |
| <code>Login.LoginButtonImageUrl=text</code> | ✅ 一致 |
| <code>Login.LoginButtonStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Login.LoginButtonStyle.CssClass=text</code> | ✅ 一致 |
| <code>Login.LoginButtonStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Login.LoginButtonStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Login.LoginButtonStyle.Height=120px</code> | ✅ 一致 |
| <code>Login.LoginButtonText=empty</code> | ✅ 一致 |
| <code>Login.LoginButtonText=japanese</code> | ✅ 一致 |
| <code>Login.LoginButtonText=markup</code> | ✅ 一致 |
| <code>Login.LoginButtonText=text</code> | ✅ 一致 |
| <code>Login.LoginButtonType=Button</code> | ✅ 一致 |
| <code>Login.LoginButtonType=Image</code> | ✅ 一致 |
| <code>Login.LoginButtonType=Image+LoginButtonImageUrl=app-relative</code> | ✅ 一致 |
| <code>Login.LoginButtonType=Link</code> | ✅ 一致 |
| <code>Login.LoginButtonType=Link+LoginButtonText=markup</code> | ✅ 一致 |
| <code>Login.MembershipProvider=empty</code> | ✅ 一致 |
| <code>Login.MembershipProvider=japanese</code> | ✅ 一致 |
| <code>Login.MembershipProvider=markup</code> | ✅ 一致 |
| <code>Login.MembershipProvider=text</code> | ✅ 一致 |
| <code>Login.Orientation=Horizontal</code> | ✅ 一致 |
| <code>Login.Orientation=Horizontal+TextLayout=TextOnTop</code> | ✅ 一致 |
| <code>Login.Orientation=Vertical</code> | ✅ 一致 |
| <code>Login.PasswordLabelText=empty</code> | ✅ 一致 |
| <code>Login.PasswordLabelText=japanese</code> | ✅ 一致 |
| <code>Login.PasswordLabelText=markup</code> | ✅ 一致 |
| <code>Login.PasswordLabelText=text</code> | ✅ 一致 |
| <code>Login.PasswordRecoveryIconUrl=app-relative</code> | ✅ 一致 |
| <code>Login.PasswordRecoveryIconUrl=empty</code> | ✅ 一致 |
| <code>Login.PasswordRecoveryIconUrl=japanese</code> | ✅ 一致 |
| <code>Login.PasswordRecoveryIconUrl=markup</code> | ✅ 一致 |
| <code>Login.PasswordRecoveryIconUrl=text</code> | ✅ 一致 |
| <code>Login.PasswordRecoveryText=empty</code> | ✅ 一致 |
| <code>Login.PasswordRecoveryText=japanese</code> | ✅ 一致 |
| <code>Login.PasswordRecoveryText=markup</code> | ✅ 一致 |
| <code>Login.PasswordRecoveryText=text</code> | ✅ 一致 |
| <code>Login.PasswordRecoveryText=text+PasswordRecoveryUrl=app-relative</code> | ✅ 一致 |
| <code>Login.PasswordRecoveryUrl=app-relative</code> | ✅ 一致 |
| <code>Login.PasswordRecoveryUrl=empty</code> | ✅ 一致 |
| <code>Login.PasswordRecoveryUrl=japanese</code> | ✅ 一致 |
| <code>Login.PasswordRecoveryUrl=markup</code> | ✅ 一致 |
| <code>Login.PasswordRecoveryUrl=text</code> | ✅ 一致 |
| <code>Login.PasswordRequiredErrorMessage=empty</code> | ✅ 一致 |
| <code>Login.PasswordRequiredErrorMessage=japanese</code> | ✅ 一致 |
| <code>Login.PasswordRequiredErrorMessage=markup</code> | ✅ 一致 |
| <code>Login.PasswordRequiredErrorMessage=text</code> | ✅ 一致 |
| <code>Login.RememberMeSet=false</code> | ✅ 一致 |
| <code>Login.RememberMeSet=true</code> | ✅ 一致 |
| <code>Login.RememberMeText=empty</code> | ✅ 一致 |
| <code>Login.RememberMeText=japanese</code> | ✅ 一致 |
| <code>Login.RememberMeText=markup</code> | ✅ 一致 |
| <code>Login.RememberMeText=text</code> | ✅ 一致 |
| <code>Login.RenderOuterTable=false</code> | ✅ 一致 |
| <code>Login.RenderOuterTable=false+Orientation=Horizontal</code> | ✅ 一致 |
| <code>Login.RenderOuterTable=true</code> | ✅ 一致 |
| <code>Login.SkinID=empty</code> | ✅ 一致 |
| <code>Login.SkinID=japanese</code> | ✅ 一致 |
| <code>Login.SkinID=markup</code> | ✅ 一致 |
| <code>Login.SkinID=text</code> | ✅ 一致 |
| <code>Login.Style[color]=1em</code> | ✅ 一致 |
| <code>Login.Style[color]=red</code> | ✅ 一致 |
| <code>Login.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>Login.Style[margin-left]=red</code> | ✅ 一致 |
| <code>Login.TabIndex=-1</code> | ✅ 一致 |
| <code>Login.TabIndex=0</code> | ✅ 一致 |
| <code>Login.TabIndex=3</code> | ✅ 一致 |
| <code>Login.TextBoxStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Login.TextBoxStyle.CssClass=text</code> | ✅ 一致 |
| <code>Login.TextBoxStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Login.TextBoxStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Login.TextBoxStyle.Height=120px</code> | ✅ 一致 |
| <code>Login.TextLayout=TextOnLeft</code> | ✅ 一致 |
| <code>Login.TextLayout=TextOnTop</code> | ✅ 一致 |
| <code>Login.TitleText=empty</code> | ✅ 一致 |
| <code>Login.TitleText=japanese</code> | ✅ 一致 |
| <code>Login.TitleText=markup</code> | ✅ 一致 |
| <code>Login.TitleText=markup+InstructionText=markup</code> | ✅ 一致 |
| <code>Login.TitleText=text</code> | ✅ 一致 |
| <code>Login.TitleTextStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Login.TitleTextStyle.BackColor=Red+LoginButtonStyle.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>Login.TitleTextStyle.CssClass=text</code> | ✅ 一致 |
| <code>Login.TitleTextStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Login.TitleTextStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Login.TitleTextStyle.Height=120px</code> | ✅ 一致 |
| <code>Login.TitleTextStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>Login.TitleTextStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>Login.TitleTextStyle.Wrap=true</code> | ✅ 一致 |
| <code>Login.ToolTip=empty</code> | ✅ 一致 |
| <code>Login.ToolTip=japanese</code> | ✅ 一致 |
| <code>Login.ToolTip=markup</code> | ✅ 一致 |
| <code>Login.ToolTip=text</code> | ✅ 一致 |
| <code>Login.UserName=empty</code> | ✅ 一致 |
| <code>Login.UserName=japanese</code> | ✅ 一致 |
| <code>Login.UserName=markup</code> | ✅ 一致 |
| <code>Login.UserName=text</code> | ✅ 一致 |
| <code>Login.UserName=text+EnableViewState=false</code> | ✅ 一致 |
| <code>Login.UserName=text+UserNameRequiredErrorMessage=markup</code> | ✅ 一致 |
| <code>Login.UserName=text+Visible=false</code> | ✅ 一致 |
| <code>Login.UserNameLabelText=empty</code> | ✅ 一致 |
| <code>Login.UserNameLabelText=japanese</code> | ✅ 一致 |
| <code>Login.UserNameLabelText=markup</code> | ✅ 一致 |
| <code>Login.UserNameLabelText=markup+PasswordLabelText=markup</code> | ✅ 一致 |
| <code>Login.UserNameLabelText=text</code> | ✅ 一致 |
| <code>Login.UserNameRequiredErrorMessage=empty</code> | ✅ 一致 |
| <code>Login.UserNameRequiredErrorMessage=japanese</code> | ✅ 一致 |
| <code>Login.UserNameRequiredErrorMessage=markup</code> | ✅ 一致 |
| <code>Login.UserNameRequiredErrorMessage=text</code> | ✅ 一致 |
| <code>Login.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>Login.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>Login.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>Login.ValidatorTextStyle.BackColor=Red</code> | ✅ 一致 |
| <code>Login.ValidatorTextStyle.CssClass=text</code> | ✅ 一致 |
| <code>Login.ValidatorTextStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>Login.ValidatorTextStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>Login.ValidatorTextStyle.Height=120px</code> | ✅ 一致 |
| <code>Login.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>Login.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>Login.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>Login.Visible=false</code> | ✅ 一致 |
| <code>Login.Visible=true</code> | ✅ 一致 |
| <code>Login.VisibleWhenLoggedIn=false</code> | ✅ 一致 |
| <code>Login.VisibleWhenLoggedIn=true</code> | ✅ 一致 |
| <code>Login.Width=120px</code> | ✅ 一致 |
| <code>Login.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>Login.Width=2em</code> | ✅ 一致 |
| <code>Login.Width=50%</code> | ✅ 一致 |
| <code>Login[logged in].(default)</code> | ✅ 一致 |
| <code>Login[logged in].VisibleWhenLoggedIn=false</code> | ✅ 一致 |
| <code>Login[naming container].(default)</code> | ✅ 一致 |
| <code>Login[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Login[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Login[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Login[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>Login[naming container].ID=text</code> | ✅ 一致 |
| <code>Login[naming container].Orientation=Horizontal</code> | ✅ 一致 |
| <code>Login[templates].(default)</code> | ✅ 一致 |
| <code>Login[templates].UserName=text</code> | ✅ 一致 |
| <code>Login[themed].(default)</code> | ✅ 一致 |
| <code>Login[themed].CssClass=text</code> | ✅ 一致 |
| <code>Login[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>Login[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| Login [render][databind][dynamic][templates] first request, anonymous: the login forms, the status, the views, the wizards | ✅ 一致 |
| Login [viewstate] a login with a wrong password: its error | ✅ 一致 |
| Login [viewstate][templates] logged in (redirected to the page): the name, the status, the role&#39;s view | ✅ 一致 |
| Login [viewstate] the password changed: the success template | ✅ 一致 |
| Login [viewstate] logged out by the status | ✅ 一致 |
| Login [viewstate] a recovery for a user who does not exist | ✅ 一致 |
| Login [viewstate] the user&#39;s question | ✅ 一致 |
| Login [viewstate] the answer right: the password reset, its mail prepared (cancelled), the success template | ✅ 一致 |
| Login [viewstate] a user created: the complete step | ✅ 一致 |
| Login [viewstate] the wizard&#39;s next step (the templated one) | ✅ 一致 |
| Login [viewstate] next, then previous | ✅ 一致 |
| Login [viewstate] a step chosen in the side bar | ✅ 一致 |
| Login [viewstate] finished: the complete step, the first step&#39;s value kept | ✅ 一致 |
| Login [dynamic][viewstate] a wizard made in code: its next step | ✅ 一致 |
| Login [visible][viewstate] a hidden wizard shown | ✅ 一致 |

</details>
