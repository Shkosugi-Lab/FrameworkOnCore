# LoginView パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 00:27
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 124 | 124 | 0 | 0 |
| ② HTML | 44 | 44 | 0 | 0 |
| ③ シナリオ | 15 | 15 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 131 件中 131 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 9 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 15 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 29 | 1 | ✅ |
| ViewState の保存と復元 | 7 | 14 | ✅ |
| Visible=false（描画されず状態は保持） | 3 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 3 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| テンプレート | 0 | 3 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>LoginView.Api.Control.#ctor</code> | ✅ 一致 |
| <code>LoginView.Api.Control.Adapter</code> | ✅ 一致 |
| <code>LoginView.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>LoginView.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>LoginView.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>LoginView.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>LoginView.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>LoginView.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>LoginView.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>LoginView.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>LoginView.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>LoginView.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>LoginView.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>LoginView.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>LoginView.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>LoginView.Api.Control.ClientID</code> | ✅ 一致 |
| <code>LoginView.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>LoginView.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>LoginView.Api.Control.Context</code> | ✅ 一致 |
| <code>LoginView.Api.Control.Controls</code> | ✅ 一致 |
| <code>LoginView.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>LoginView.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>LoginView.Api.Control.DataBind</code> | ✅ 一致 |
| <code>LoginView.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>LoginView.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>LoginView.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>LoginView.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>LoginView.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>LoginView.Api.Control.Dispose</code> | ✅ 一致 |
| <code>LoginView.Api.Control.Disposed</code> | ✅ 一致 |
| <code>LoginView.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>LoginView.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>LoginView.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>LoginView.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>LoginView.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>LoginView.Api.Control.Events</code> | ✅ 一致 |
| <code>LoginView.Api.Control.FindControl</code> | ✅ 一致 |
| <code>LoginView.Api.Control.Focus</code> | ✅ 一致 |
| <code>LoginView.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>LoginView.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>LoginView.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>LoginView.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>LoginView.Api.Control.HasControls</code> | ✅ 一致 |
| <code>LoginView.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>LoginView.Api.Control.ID</code> | ✅ 一致 |
| <code>LoginView.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>LoginView.Api.Control.Init</code> | ✅ 一致 |
| <code>LoginView.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>LoginView.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>LoginView.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>LoginView.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>LoginView.Api.Control.Load</code> | ✅ 一致 |
| <code>LoginView.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>LoginView.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>LoginView.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>LoginView.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>LoginView.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>LoginView.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>LoginView.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>LoginView.Api.Control.OnInit</code> | ✅ 一致 |
| <code>LoginView.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>LoginView.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>LoginView.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>LoginView.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>LoginView.Api.Control.Page</code> | ✅ 一致 |
| <code>LoginView.Api.Control.Parent</code> | ✅ 一致 |
| <code>LoginView.Api.Control.PreRender</code> | ✅ 一致 |
| <code>LoginView.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>LoginView.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>LoginView.Api.Control.Render</code> | ✅ 一致 |
| <code>LoginView.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>LoginView.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>LoginView.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>LoginView.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>LoginView.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>LoginView.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>LoginView.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>LoginView.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>LoginView.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>LoginView.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>LoginView.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>LoginView.Api.Control.Site</code> | ✅ 一致 |
| <code>LoginView.Api.Control.SkinID</code> | ✅ 一致 |
| <code>LoginView.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>LoginView.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>LoginView.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>LoginView.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>LoginView.Api.Control.Unload</code> | ✅ 一致 |
| <code>LoginView.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>LoginView.Api.Control.ViewState</code> | ✅ 一致 |
| <code>LoginView.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>LoginView.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>LoginView.Api.Control.Visible</code> | ✅ 一致 |
| <code>LoginView.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>LoginView.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>LoginView.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>LoginView.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>LoginView.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>LoginView.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>LoginView.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>LoginView.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>LoginView.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>LoginView.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>LoginView.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>LoginView.Api.WebControls.LoginView.#ctor</code> | ✅ 一致 |
| <code>LoginView.Api.WebControls.LoginView.AnonymousTemplate</code> | ✅ 一致 |
| <code>LoginView.Api.WebControls.LoginView.Controls</code> | ✅ 一致 |
| <code>LoginView.Api.WebControls.LoginView.CreateChildControls</code> | ✅ 一致 |
| <code>LoginView.Api.WebControls.LoginView.DataBind</code> | ✅ 一致 |
| <code>LoginView.Api.WebControls.LoginView.EnableTheming</code> | ✅ 一致 |
| <code>LoginView.Api.WebControls.LoginView.Focus</code> | ✅ 一致 |
| <code>LoginView.Api.WebControls.LoginView.LoadControlState</code> | ✅ 一致 |
| <code>LoginView.Api.WebControls.LoginView.LoggedInTemplate</code> | ✅ 一致 |
| <code>LoginView.Api.WebControls.LoginView.OnInit</code> | ✅ 一致 |
| <code>LoginView.Api.WebControls.LoginView.OnPreRender</code> | ✅ 一致 |
| <code>LoginView.Api.WebControls.LoginView.OnViewChanged</code> | ✅ 一致 |
| <code>LoginView.Api.WebControls.LoginView.OnViewChanging</code> | ✅ 一致 |
| <code>LoginView.Api.WebControls.LoginView.Render</code> | ✅ 一致 |
| <code>LoginView.Api.WebControls.LoginView.RoleGroups</code> | ✅ 一致 |
| <code>LoginView.Api.WebControls.LoginView.SaveControlState</code> | ✅ 一致 |
| <code>LoginView.Api.WebControls.LoginView.SetDesignModeState</code> | ✅ 一致 |
| <code>LoginView.Api.WebControls.LoginView.SkinID</code> | ✅ 一致 |
| <code>LoginView.Api.WebControls.LoginView.ViewChanged</code> | ✅ 一致 |
| <code>LoginView.Api.WebControls.LoginView.ViewChanging</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>LoginView.(case list)</code> | ✅ 一致 |
| <code>LoginView.(default)</code> | ✅ 一致 |
| <code>LoginView.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>LoginView.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>LoginView.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>LoginView.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>LoginView.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>LoginView.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>LoginView.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>LoginView.ClientIDMode=Static</code> | ✅ 一致 |
| <code>LoginView.EnableTheming=false</code> | ✅ 一致 |
| <code>LoginView.EnableTheming=true</code> | ✅ 一致 |
| <code>LoginView.EnableViewState=false</code> | ✅ 一致 |
| <code>LoginView.EnableViewState=true</code> | ✅ 一致 |
| <code>LoginView.ID=empty</code> | ✅ 一致 |
| <code>LoginView.ID=japanese</code> | ✅ 一致 |
| <code>LoginView.ID=markup</code> | ✅ 一致 |
| <code>LoginView.ID=text</code> | ✅ 一致 |
| <code>LoginView.SkinID=empty</code> | ✅ 一致 |
| <code>LoginView.SkinID=japanese</code> | ✅ 一致 |
| <code>LoginView.SkinID=markup</code> | ✅ 一致 |
| <code>LoginView.SkinID=text</code> | ✅ 一致 |
| <code>LoginView.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>LoginView.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>LoginView.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>LoginView.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>LoginView.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>LoginView.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>LoginView.Visible=false</code> | ✅ 一致 |
| <code>LoginView.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>LoginView.Visible=true</code> | ✅ 一致 |
| <code>LoginView[logged in].(default)</code> | ✅ 一致 |
| <code>LoginView[logged in].EnableViewState=false</code> | ✅ 一致 |
| <code>LoginView[logged in].Visible=false</code> | ✅ 一致 |
| <code>LoginView[naming container].(default)</code> | ✅ 一致 |
| <code>LoginView[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>LoginView[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>LoginView[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>LoginView[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>LoginView[naming container].ID=text</code> | ✅ 一致 |
| <code>LoginView[no templates].(default)</code> | ✅ 一致 |
| <code>LoginView[themed].(default)</code> | ✅ 一致 |
| <code>LoginView[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>LoginView[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| LoginView [render][databind][dynamic][templates] first request, anonymous: the login forms, the status, the views, the wizards | ✅ 一致 |
| LoginView [viewstate] a login with a wrong password: its error | ✅ 一致 |
| LoginView [viewstate][templates] logged in (redirected to the page): the name, the status, the role&#39;s view | ✅ 一致 |
| LoginView [viewstate] the password changed: the success template | ✅ 一致 |
| LoginView [viewstate][templates] logged out by the status | ✅ 一致 |
| LoginView [viewstate] a recovery for a user who does not exist | ✅ 一致 |
| LoginView [viewstate] the user&#39;s question | ✅ 一致 |
| LoginView [viewstate] the answer right: the password reset, its mail prepared (cancelled), the success template | ✅ 一致 |
| LoginView [viewstate] a user created: the complete step | ✅ 一致 |
| LoginView [viewstate] the wizard&#39;s next step (the templated one) | ✅ 一致 |
| LoginView [viewstate] next, then previous | ✅ 一致 |
| LoginView [viewstate] a step chosen in the side bar | ✅ 一致 |
| LoginView [viewstate] finished: the complete step, the first step&#39;s value kept | ✅ 一致 |
| LoginView [dynamic][viewstate] a wizard made in code: its next step | ✅ 一致 |
| LoginView [visible][viewstate] a hidden wizard shown | ✅ 一致 |

</details>
