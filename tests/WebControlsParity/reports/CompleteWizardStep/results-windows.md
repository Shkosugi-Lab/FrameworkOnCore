# CompleteWizardStep パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-11 23:21
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 131 | 131 | 0 | 0 |
| ② HTML | 53 | 53 | 0 | 0 |
| ③ シナリオ | 15 | 15 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 138 件中 138 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
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
| <code>CompleteWizardStep.Api.Control.#ctor</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.Adapter</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.ClientID</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.Context</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.Controls</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.DataBind</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.Dispose</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.Disposed</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.Events</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.FindControl</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.Focus</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.HasControls</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.ID</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.Init</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.Load</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.OnInit</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.Page</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.Parent</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.PreRender</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.Render</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.Site</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.SkinID</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.Unload</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.ViewState</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.Control.Visible</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.WebControls.CompleteWizardStep.#ctor</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.WebControls.CompleteWizardStep.StepType</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.WebControls.CompleteWizardStep.Title</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.WebControls.TemplatedWizardStep.#ctor</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.WebControls.TemplatedWizardStep.ContentTemplate</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.WebControls.TemplatedWizardStep.ContentTemplateContainer</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.WebControls.TemplatedWizardStep.CustomNavigationTemplate</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.WebControls.TemplatedWizardStep.CustomNavigationTemplateContainer</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.WebControls.TemplatedWizardStep.SkinID</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.WebControls.View.#ctor</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.WebControls.View.Activate</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.WebControls.View.Deactivate</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.WebControls.View.EnableTheming</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.WebControls.View.OnActivate</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.WebControls.View.OnDeactivate</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.WebControls.View.Visible</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.WebControls.WizardStepBase.#ctor</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.WebControls.WizardStepBase.AllowReturn</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.WebControls.WizardStepBase.EnableTheming</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.WebControls.WizardStepBase.ID</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.WebControls.WizardStepBase.LoadViewState</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.WebControls.WizardStepBase.Name</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.WebControls.WizardStepBase.OnLoad</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.WebControls.WizardStepBase.RenderChildren</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.WebControls.WizardStepBase.StepType</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.WebControls.WizardStepBase.Title</code> | ✅ 一致 |
| <code>CompleteWizardStep.Api.WebControls.WizardStepBase.Wizard</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>CompleteWizardStep.(case list)</code> | ✅ 一致 |
| <code>CompleteWizardStep.(default)</code> | ✅ 一致 |
| <code>CompleteWizardStep.AllowReturn=false</code> | ✅ 一致 |
| <code>CompleteWizardStep.AllowReturn=true</code> | ✅ 一致 |
| <code>CompleteWizardStep.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>CompleteWizardStep.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>CompleteWizardStep.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>CompleteWizardStep.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>CompleteWizardStep.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>CompleteWizardStep.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>CompleteWizardStep.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>CompleteWizardStep.ClientIDMode=Static</code> | ✅ 一致 |
| <code>CompleteWizardStep.EnableTheming=false</code> | ✅ 一致 |
| <code>CompleteWizardStep.EnableTheming=true</code> | ✅ 一致 |
| <code>CompleteWizardStep.EnableViewState=false</code> | ✅ 一致 |
| <code>CompleteWizardStep.EnableViewState=true</code> | ✅ 一致 |
| <code>CompleteWizardStep.ID=empty</code> | ✅ 一致 |
| <code>CompleteWizardStep.ID=japanese</code> | ✅ 一致 |
| <code>CompleteWizardStep.ID=markup</code> | ✅ 一致 |
| <code>CompleteWizardStep.ID=text</code> | ✅ 一致 |
| <code>CompleteWizardStep.SkinID=empty</code> | ✅ 一致 |
| <code>CompleteWizardStep.SkinID=japanese</code> | ✅ 一致 |
| <code>CompleteWizardStep.SkinID=markup</code> | ✅ 一致 |
| <code>CompleteWizardStep.SkinID=text</code> | ✅ 一致 |
| <code>CompleteWizardStep.StepType=Auto</code> | ✅ 一致 |
| <code>CompleteWizardStep.StepType=Complete</code> | ✅ 一致 |
| <code>CompleteWizardStep.StepType=Finish</code> | ✅ 一致 |
| <code>CompleteWizardStep.StepType=Start</code> | ✅ 一致 |
| <code>CompleteWizardStep.StepType=Step</code> | ✅ 一致 |
| <code>CompleteWizardStep.Title=empty</code> | ✅ 一致 |
| <code>CompleteWizardStep.Title=japanese</code> | ✅ 一致 |
| <code>CompleteWizardStep.Title=markup</code> | ✅ 一致 |
| <code>CompleteWizardStep.Title=markup+AllowReturn=false</code> | ✅ 一致 |
| <code>CompleteWizardStep.Title=text</code> | ✅ 一致 |
| <code>CompleteWizardStep.Title=text+EnableViewState=false</code> | ✅ 一致 |
| <code>CompleteWizardStep.Title=text+Visible=false</code> | ✅ 一致 |
| <code>CompleteWizardStep.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>CompleteWizardStep.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>CompleteWizardStep.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>CompleteWizardStep.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>CompleteWizardStep.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>CompleteWizardStep.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>CompleteWizardStep.Visible=false</code> | ✅ 一致 |
| <code>CompleteWizardStep.Visible=true</code> | ✅ 一致 |
| <code>CompleteWizardStep[naming container].(default)</code> | ✅ 一致 |
| <code>CompleteWizardStep[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>CompleteWizardStep[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>CompleteWizardStep[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>CompleteWizardStep[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>CompleteWizardStep[naming container].ID=text</code> | ✅ 一致 |
| <code>CompleteWizardStep[themed].(default)</code> | ✅ 一致 |
| <code>CompleteWizardStep[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>CompleteWizardStep[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| CompleteWizardStep [render][databind][dynamic][templates] first request, anonymous: the login forms, the status, the views, the wizards | ✅ 一致 |
| CompleteWizardStep [viewstate] a login with a wrong password: its error | ✅ 一致 |
| CompleteWizardStep [viewstate] logged in (redirected to the page): the name, the status, the role&#39;s view | ✅ 一致 |
| CompleteWizardStep [viewstate] the password changed: the success template | ✅ 一致 |
| CompleteWizardStep [viewstate] logged out by the status | ✅ 一致 |
| CompleteWizardStep [viewstate] a recovery for a user who does not exist | ✅ 一致 |
| CompleteWizardStep [viewstate] the user&#39;s question | ✅ 一致 |
| CompleteWizardStep [viewstate] the answer right: the password reset, its mail prepared (cancelled), the success template | ✅ 一致 |
| CompleteWizardStep [viewstate][templates] a user created: the complete step | ✅ 一致 |
| CompleteWizardStep [viewstate] the wizard&#39;s next step (the templated one) | ✅ 一致 |
| CompleteWizardStep [viewstate] next, then previous | ✅ 一致 |
| CompleteWizardStep [viewstate] a step chosen in the side bar | ✅ 一致 |
| CompleteWizardStep [viewstate] finished: the complete step, the first step&#39;s value kept | ✅ 一致 |
| CompleteWizardStep [dynamic][viewstate] a wizard made in code: its next step | ✅ 一致 |
| CompleteWizardStep [visible][viewstate] a hidden wizard shown | ✅ 一致 |

</details>
