# CreateUserWizardStep パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 01:41
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 132 | 132 | 0 | 0 |
| ② HTML | 53 | 53 | 0 | 0 |
| ③ シナリオ | 15 | 15 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 139 件中 139 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 12 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 15 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 42 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 14 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 3 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| テンプレート | 0 | 2 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>CreateUserWizardStep.Api.Control.#ctor</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.Adapter</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.ClientID</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.Context</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.Controls</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.DataBind</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.Dispose</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.Disposed</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.Events</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.FindControl</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.Focus</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.HasControls</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.ID</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.Init</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.Load</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.OnInit</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.Page</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.Parent</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.PreRender</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.Render</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.Site</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.SkinID</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.Unload</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.ViewState</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.Control.Visible</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.CreateUserWizardStep.#ctor</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.CreateUserWizardStep.AllowReturn</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.CreateUserWizardStep.StepType</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.CreateUserWizardStep.Title</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.TemplatedWizardStep.#ctor</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.TemplatedWizardStep.ContentTemplate</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.TemplatedWizardStep.ContentTemplateContainer</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.TemplatedWizardStep.CustomNavigationTemplate</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.TemplatedWizardStep.CustomNavigationTemplateContainer</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.TemplatedWizardStep.SkinID</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.View.#ctor</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.View.Activate</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.View.Deactivate</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.View.EnableTheming</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.View.OnActivate</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.View.OnDeactivate</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.View.Visible</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.WizardStepBase.#ctor</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.WizardStepBase.AllowReturn</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.WizardStepBase.EnableTheming</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.WizardStepBase.ID</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.WizardStepBase.LoadViewState</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.WizardStepBase.Name</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.WizardStepBase.OnLoad</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.WizardStepBase.RenderChildren</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.WizardStepBase.StepType</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.WizardStepBase.Title</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Api.WebControls.WizardStepBase.Wizard</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>CreateUserWizardStep.(case list)</code> | ✅ 一致 |
| <code>CreateUserWizardStep.(default)</code> | ✅ 一致 |
| <code>CreateUserWizardStep.AllowReturn=false</code> | ✅ 一致 |
| <code>CreateUserWizardStep.AllowReturn=true</code> | ✅ 一致 |
| <code>CreateUserWizardStep.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>CreateUserWizardStep.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>CreateUserWizardStep.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>CreateUserWizardStep.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>CreateUserWizardStep.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>CreateUserWizardStep.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>CreateUserWizardStep.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>CreateUserWizardStep.ClientIDMode=Static</code> | ✅ 一致 |
| <code>CreateUserWizardStep.EnableTheming=false</code> | ✅ 一致 |
| <code>CreateUserWizardStep.EnableTheming=true</code> | ✅ 一致 |
| <code>CreateUserWizardStep.EnableViewState=false</code> | ✅ 一致 |
| <code>CreateUserWizardStep.EnableViewState=true</code> | ✅ 一致 |
| <code>CreateUserWizardStep.ID=empty</code> | ✅ 一致 |
| <code>CreateUserWizardStep.ID=japanese</code> | ✅ 一致 |
| <code>CreateUserWizardStep.ID=markup</code> | ✅ 一致 |
| <code>CreateUserWizardStep.ID=text</code> | ✅ 一致 |
| <code>CreateUserWizardStep.SkinID=empty</code> | ✅ 一致 |
| <code>CreateUserWizardStep.SkinID=japanese</code> | ✅ 一致 |
| <code>CreateUserWizardStep.SkinID=markup</code> | ✅ 一致 |
| <code>CreateUserWizardStep.SkinID=text</code> | ✅ 一致 |
| <code>CreateUserWizardStep.StepType=Auto</code> | ✅ 一致 |
| <code>CreateUserWizardStep.StepType=Complete</code> | ✅ 一致 |
| <code>CreateUserWizardStep.StepType=Finish</code> | ✅ 一致 |
| <code>CreateUserWizardStep.StepType=Start</code> | ✅ 一致 |
| <code>CreateUserWizardStep.StepType=Step</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Title=empty</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Title=japanese</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Title=markup</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Title=markup+AllowReturn=false</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Title=text</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Title=text+EnableViewState=false</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Title=text+Visible=false</code> | ✅ 一致 |
| <code>CreateUserWizardStep.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>CreateUserWizardStep.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>CreateUserWizardStep.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>CreateUserWizardStep.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>CreateUserWizardStep.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>CreateUserWizardStep.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Visible=false</code> | ✅ 一致 |
| <code>CreateUserWizardStep.Visible=true</code> | ✅ 一致 |
| <code>CreateUserWizardStep[naming container].(default)</code> | ✅ 一致 |
| <code>CreateUserWizardStep[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>CreateUserWizardStep[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>CreateUserWizardStep[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>CreateUserWizardStep[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>CreateUserWizardStep[naming container].ID=text</code> | ✅ 一致 |
| <code>CreateUserWizardStep[themed].(default)</code> | ✅ 一致 |
| <code>CreateUserWizardStep[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>CreateUserWizardStep[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| CreateUserWizardStep [render][databind][dynamic][templates] first request, anonymous: the login forms, the status, the views, the wizards | ✅ 一致 |
| CreateUserWizardStep [viewstate] a login with a wrong password: its error | ✅ 一致 |
| CreateUserWizardStep [viewstate] logged in (redirected to the page): the name, the status, the role&#39;s view | ✅ 一致 |
| CreateUserWizardStep [viewstate] the password changed: the success template | ✅ 一致 |
| CreateUserWizardStep [viewstate] logged out by the status | ✅ 一致 |
| CreateUserWizardStep [viewstate] a recovery for a user who does not exist | ✅ 一致 |
| CreateUserWizardStep [viewstate] the user&#39;s question | ✅ 一致 |
| CreateUserWizardStep [viewstate] the answer right: the password reset, its mail prepared (cancelled), the success template | ✅ 一致 |
| CreateUserWizardStep [viewstate][templates] a user created: the complete step | ✅ 一致 |
| CreateUserWizardStep [viewstate] the wizard&#39;s next step (the templated one) | ✅ 一致 |
| CreateUserWizardStep [viewstate] next, then previous | ✅ 一致 |
| CreateUserWizardStep [viewstate] a step chosen in the side bar | ✅ 一致 |
| CreateUserWizardStep [viewstate] finished: the complete step, the first step&#39;s value kept | ✅ 一致 |
| CreateUserWizardStep [dynamic][viewstate] a wizard made in code: its next step | ✅ 一致 |
| CreateUserWizardStep [visible][viewstate] a hidden wizard shown | ✅ 一致 |

</details>
