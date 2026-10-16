# TemplatedWizardStep パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 00:58
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 128 | 128 | 0 | 0 |
| ② HTML | 54 | 54 | 0 | 0 |
| ③ シナリオ | 15 | 15 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 135 件中 135 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 12 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 15 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 43 | 1 | ✅ |
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
| <code>TemplatedWizardStep.Api.Control.#ctor</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.Adapter</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.ClientID</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.Context</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.Controls</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.DataBind</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.Dispose</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.Disposed</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.Events</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.FindControl</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.Focus</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.HasControls</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.ID</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.Init</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.Load</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.OnInit</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.Page</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.Parent</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.PreRender</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.Render</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.Site</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.SkinID</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.Unload</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.ViewState</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.Control.Visible</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.WebControls.TemplatedWizardStep.#ctor</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.WebControls.TemplatedWizardStep.ContentTemplate</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.WebControls.TemplatedWizardStep.ContentTemplateContainer</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.WebControls.TemplatedWizardStep.CustomNavigationTemplate</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.WebControls.TemplatedWizardStep.CustomNavigationTemplateContainer</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.WebControls.TemplatedWizardStep.SkinID</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.WebControls.View.#ctor</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.WebControls.View.Activate</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.WebControls.View.Deactivate</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.WebControls.View.EnableTheming</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.WebControls.View.OnActivate</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.WebControls.View.OnDeactivate</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.WebControls.View.Visible</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.WebControls.WizardStepBase.#ctor</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.WebControls.WizardStepBase.AllowReturn</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.WebControls.WizardStepBase.EnableTheming</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.WebControls.WizardStepBase.ID</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.WebControls.WizardStepBase.LoadViewState</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.WebControls.WizardStepBase.Name</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.WebControls.WizardStepBase.OnLoad</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.WebControls.WizardStepBase.RenderChildren</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.WebControls.WizardStepBase.StepType</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.WebControls.WizardStepBase.Title</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Api.WebControls.WizardStepBase.Wizard</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>TemplatedWizardStep.(case list)</code> | ✅ 一致 |
| <code>TemplatedWizardStep.(default)</code> | ✅ 一致 |
| <code>TemplatedWizardStep.AllowReturn=false</code> | ✅ 一致 |
| <code>TemplatedWizardStep.AllowReturn=true</code> | ✅ 一致 |
| <code>TemplatedWizardStep.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>TemplatedWizardStep.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>TemplatedWizardStep.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>TemplatedWizardStep.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>TemplatedWizardStep.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>TemplatedWizardStep.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>TemplatedWizardStep.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>TemplatedWizardStep.ClientIDMode=Static</code> | ✅ 一致 |
| <code>TemplatedWizardStep.EnableTheming=false</code> | ✅ 一致 |
| <code>TemplatedWizardStep.EnableTheming=true</code> | ✅ 一致 |
| <code>TemplatedWizardStep.EnableViewState=false</code> | ✅ 一致 |
| <code>TemplatedWizardStep.EnableViewState=true</code> | ✅ 一致 |
| <code>TemplatedWizardStep.ID=empty</code> | ✅ 一致 |
| <code>TemplatedWizardStep.ID=japanese</code> | ✅ 一致 |
| <code>TemplatedWizardStep.ID=markup</code> | ✅ 一致 |
| <code>TemplatedWizardStep.ID=text</code> | ✅ 一致 |
| <code>TemplatedWizardStep.SkinID=empty</code> | ✅ 一致 |
| <code>TemplatedWizardStep.SkinID=japanese</code> | ✅ 一致 |
| <code>TemplatedWizardStep.SkinID=markup</code> | ✅ 一致 |
| <code>TemplatedWizardStep.SkinID=text</code> | ✅ 一致 |
| <code>TemplatedWizardStep.StepType=Auto</code> | ✅ 一致 |
| <code>TemplatedWizardStep.StepType=Complete</code> | ✅ 一致 |
| <code>TemplatedWizardStep.StepType=Finish</code> | ✅ 一致 |
| <code>TemplatedWizardStep.StepType=Finish+Title=markup</code> | ✅ 一致 |
| <code>TemplatedWizardStep.StepType=Start</code> | ✅ 一致 |
| <code>TemplatedWizardStep.StepType=Start+AllowReturn=false</code> | ✅ 一致 |
| <code>TemplatedWizardStep.StepType=Step</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Title=empty</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Title=japanese</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Title=markup</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Title=text</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Title=text+EnableViewState=false</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Title=text+Visible=false</code> | ✅ 一致 |
| <code>TemplatedWizardStep.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>TemplatedWizardStep.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>TemplatedWizardStep.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>TemplatedWizardStep.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>TemplatedWizardStep.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>TemplatedWizardStep.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Visible=false</code> | ✅ 一致 |
| <code>TemplatedWizardStep.Visible=true</code> | ✅ 一致 |
| <code>TemplatedWizardStep[naming container].(default)</code> | ✅ 一致 |
| <code>TemplatedWizardStep[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>TemplatedWizardStep[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>TemplatedWizardStep[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>TemplatedWizardStep[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>TemplatedWizardStep[naming container].ID=text</code> | ✅ 一致 |
| <code>TemplatedWizardStep[themed].(default)</code> | ✅ 一致 |
| <code>TemplatedWizardStep[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>TemplatedWizardStep[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| TemplatedWizardStep [render][databind][dynamic][templates] first request, anonymous: the login forms, the status, the views, the wizards | ✅ 一致 |
| TemplatedWizardStep [viewstate] a login with a wrong password: its error | ✅ 一致 |
| TemplatedWizardStep [viewstate] logged in (redirected to the page): the name, the status, the role&#39;s view | ✅ 一致 |
| TemplatedWizardStep [viewstate] the password changed: the success template | ✅ 一致 |
| TemplatedWizardStep [viewstate] logged out by the status | ✅ 一致 |
| TemplatedWizardStep [viewstate] a recovery for a user who does not exist | ✅ 一致 |
| TemplatedWizardStep [viewstate] the user&#39;s question | ✅ 一致 |
| TemplatedWizardStep [viewstate] the answer right: the password reset, its mail prepared (cancelled), the success template | ✅ 一致 |
| TemplatedWizardStep [viewstate] a user created: the complete step | ✅ 一致 |
| TemplatedWizardStep [viewstate][templates] the wizard&#39;s next step (the templated one) | ✅ 一致 |
| TemplatedWizardStep [viewstate] next, then previous | ✅ 一致 |
| TemplatedWizardStep [viewstate] a step chosen in the side bar | ✅ 一致 |
| TemplatedWizardStep [viewstate] finished: the complete step, the first step&#39;s value kept | ✅ 一致 |
| TemplatedWizardStep [dynamic][viewstate] a wizard made in code: its next step | ✅ 一致 |
| TemplatedWizardStep [visible][viewstate] a hidden wizard shown | ✅ 一致 |

</details>
