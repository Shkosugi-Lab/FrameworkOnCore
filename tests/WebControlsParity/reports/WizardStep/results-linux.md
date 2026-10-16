# WizardStep パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 03:28
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 123 | 123 | 0 | 0 |
| ② HTML | 51 | 51 | 0 | 0 |
| ③ シナリオ | 15 | 15 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 130 件中 130 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 12 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 15 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 43 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 14 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>WizardStep.Api.Control.#ctor</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.Adapter</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.ClientID</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.Context</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.Controls</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.DataBind</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.Dispose</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.Disposed</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.Events</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.FindControl</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.Focus</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.HasControls</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.ID</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.Init</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.Load</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.OnInit</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.Page</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.Parent</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.PreRender</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.Render</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.Site</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.SkinID</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.Unload</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.ViewState</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>WizardStep.Api.Control.Visible</code> | ✅ 一致 |
| <code>WizardStep.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>WizardStep.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>WizardStep.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>WizardStep.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>WizardStep.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>WizardStep.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>WizardStep.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>WizardStep.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>WizardStep.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>WizardStep.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>WizardStep.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>WizardStep.Api.WebControls.View.#ctor</code> | ✅ 一致 |
| <code>WizardStep.Api.WebControls.View.Activate</code> | ✅ 一致 |
| <code>WizardStep.Api.WebControls.View.Deactivate</code> | ✅ 一致 |
| <code>WizardStep.Api.WebControls.View.EnableTheming</code> | ✅ 一致 |
| <code>WizardStep.Api.WebControls.View.OnActivate</code> | ✅ 一致 |
| <code>WizardStep.Api.WebControls.View.OnDeactivate</code> | ✅ 一致 |
| <code>WizardStep.Api.WebControls.View.Visible</code> | ✅ 一致 |
| <code>WizardStep.Api.WebControls.WizardStep.#ctor</code> | ✅ 一致 |
| <code>WizardStep.Api.WebControls.WizardStepBase.#ctor</code> | ✅ 一致 |
| <code>WizardStep.Api.WebControls.WizardStepBase.AllowReturn</code> | ✅ 一致 |
| <code>WizardStep.Api.WebControls.WizardStepBase.EnableTheming</code> | ✅ 一致 |
| <code>WizardStep.Api.WebControls.WizardStepBase.ID</code> | ✅ 一致 |
| <code>WizardStep.Api.WebControls.WizardStepBase.LoadViewState</code> | ✅ 一致 |
| <code>WizardStep.Api.WebControls.WizardStepBase.Name</code> | ✅ 一致 |
| <code>WizardStep.Api.WebControls.WizardStepBase.OnLoad</code> | ✅ 一致 |
| <code>WizardStep.Api.WebControls.WizardStepBase.RenderChildren</code> | ✅ 一致 |
| <code>WizardStep.Api.WebControls.WizardStepBase.StepType</code> | ✅ 一致 |
| <code>WizardStep.Api.WebControls.WizardStepBase.Title</code> | ✅ 一致 |
| <code>WizardStep.Api.WebControls.WizardStepBase.Wizard</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>WizardStep.(case list)</code> | ✅ 一致 |
| <code>WizardStep.(default)</code> | ✅ 一致 |
| <code>WizardStep.AllowReturn=false</code> | ✅ 一致 |
| <code>WizardStep.AllowReturn=true</code> | ✅ 一致 |
| <code>WizardStep.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>WizardStep.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>WizardStep.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>WizardStep.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>WizardStep.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>WizardStep.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>WizardStep.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>WizardStep.ClientIDMode=Static</code> | ✅ 一致 |
| <code>WizardStep.EnableTheming=false</code> | ✅ 一致 |
| <code>WizardStep.EnableTheming=true</code> | ✅ 一致 |
| <code>WizardStep.EnableViewState=false</code> | ✅ 一致 |
| <code>WizardStep.EnableViewState=true</code> | ✅ 一致 |
| <code>WizardStep.ID=empty</code> | ✅ 一致 |
| <code>WizardStep.ID=japanese</code> | ✅ 一致 |
| <code>WizardStep.ID=markup</code> | ✅ 一致 |
| <code>WizardStep.ID=text</code> | ✅ 一致 |
| <code>WizardStep.SkinID=empty</code> | ✅ 一致 |
| <code>WizardStep.SkinID=japanese</code> | ✅ 一致 |
| <code>WizardStep.SkinID=markup</code> | ✅ 一致 |
| <code>WizardStep.SkinID=text</code> | ✅ 一致 |
| <code>WizardStep.StepType=Auto</code> | ✅ 一致 |
| <code>WizardStep.StepType=Complete</code> | ✅ 一致 |
| <code>WizardStep.StepType=Finish</code> | ✅ 一致 |
| <code>WizardStep.StepType=Finish+Title=markup</code> | ✅ 一致 |
| <code>WizardStep.StepType=Start</code> | ✅ 一致 |
| <code>WizardStep.StepType=Start+AllowReturn=false</code> | ✅ 一致 |
| <code>WizardStep.StepType=Step</code> | ✅ 一致 |
| <code>WizardStep.Title=empty</code> | ✅ 一致 |
| <code>WizardStep.Title=japanese</code> | ✅ 一致 |
| <code>WizardStep.Title=markup</code> | ✅ 一致 |
| <code>WizardStep.Title=text</code> | ✅ 一致 |
| <code>WizardStep.Title=text+EnableViewState=false</code> | ✅ 一致 |
| <code>WizardStep.Title=text+Visible=false</code> | ✅ 一致 |
| <code>WizardStep.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>WizardStep.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>WizardStep.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>WizardStep.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>WizardStep.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>WizardStep.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>WizardStep.Visible=false</code> | ✅ 一致 |
| <code>WizardStep.Visible=true</code> | ✅ 一致 |
| <code>WizardStep[naming container].(default)</code> | ✅ 一致 |
| <code>WizardStep[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>WizardStep[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>WizardStep[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>WizardStep[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>WizardStep[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| WizardStep [render][databind][dynamic] first request, anonymous: the login forms, the status, the views, the wizards | ✅ 一致 |
| WizardStep [viewstate] a login with a wrong password: its error | ✅ 一致 |
| WizardStep [viewstate] logged in (redirected to the page): the name, the status, the role&#39;s view | ✅ 一致 |
| WizardStep [viewstate] the password changed: the success template | ✅ 一致 |
| WizardStep [viewstate] logged out by the status | ✅ 一致 |
| WizardStep [viewstate] a recovery for a user who does not exist | ✅ 一致 |
| WizardStep [viewstate] the user&#39;s question | ✅ 一致 |
| WizardStep [viewstate] the answer right: the password reset, its mail prepared (cancelled), the success template | ✅ 一致 |
| WizardStep [viewstate] a user created: the complete step | ✅ 一致 |
| WizardStep [viewstate][templates] the wizard&#39;s next step (the templated one) | ✅ 一致 |
| WizardStep [viewstate] next, then previous | ✅ 一致 |
| WizardStep [viewstate] a step chosen in the side bar | ✅ 一致 |
| WizardStep [viewstate] finished: the complete step, the first step&#39;s value kept | ✅ 一致 |
| WizardStep [dynamic][viewstate] a wizard made in code: its next step | ✅ 一致 |
| WizardStep [visible][viewstate] a hidden wizard shown | ✅ 一致 |

</details>
