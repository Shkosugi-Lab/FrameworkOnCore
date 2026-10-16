# Substitution パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-15 15:01
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 109 | 109 | 0 | 0 |
| ② HTML | 44 | 44 | 0 | 0 |
| ③ シナリオ | 14 | 14 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 116 件中 116 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 10 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 14 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 36 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 13 | ❌ 1 件 |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 3 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>Substitution.Api.Control.#ctor</code> | ✅ 一致 |
| <code>Substitution.Api.Control.Adapter</code> | ✅ 一致 |
| <code>Substitution.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>Substitution.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>Substitution.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>Substitution.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>Substitution.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>Substitution.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>Substitution.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>Substitution.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>Substitution.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>Substitution.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>Substitution.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>Substitution.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>Substitution.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>Substitution.Api.Control.ClientID</code> | ✅ 一致 |
| <code>Substitution.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>Substitution.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>Substitution.Api.Control.Context</code> | ✅ 一致 |
| <code>Substitution.Api.Control.Controls</code> | ✅ 一致 |
| <code>Substitution.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>Substitution.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>Substitution.Api.Control.DataBind</code> | ✅ 一致 |
| <code>Substitution.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>Substitution.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>Substitution.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>Substitution.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>Substitution.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>Substitution.Api.Control.Dispose</code> | ✅ 一致 |
| <code>Substitution.Api.Control.Disposed</code> | ✅ 一致 |
| <code>Substitution.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>Substitution.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>Substitution.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>Substitution.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>Substitution.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>Substitution.Api.Control.Events</code> | ✅ 一致 |
| <code>Substitution.Api.Control.FindControl</code> | ✅ 一致 |
| <code>Substitution.Api.Control.Focus</code> | ✅ 一致 |
| <code>Substitution.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>Substitution.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>Substitution.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>Substitution.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>Substitution.Api.Control.HasControls</code> | ✅ 一致 |
| <code>Substitution.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>Substitution.Api.Control.ID</code> | ✅ 一致 |
| <code>Substitution.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>Substitution.Api.Control.Init</code> | ✅ 一致 |
| <code>Substitution.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>Substitution.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>Substitution.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>Substitution.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>Substitution.Api.Control.Load</code> | ✅ 一致 |
| <code>Substitution.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>Substitution.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>Substitution.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>Substitution.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>Substitution.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>Substitution.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>Substitution.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>Substitution.Api.Control.OnInit</code> | ✅ 一致 |
| <code>Substitution.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>Substitution.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>Substitution.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>Substitution.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>Substitution.Api.Control.Page</code> | ✅ 一致 |
| <code>Substitution.Api.Control.Parent</code> | ✅ 一致 |
| <code>Substitution.Api.Control.PreRender</code> | ✅ 一致 |
| <code>Substitution.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>Substitution.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>Substitution.Api.Control.Render</code> | ✅ 一致 |
| <code>Substitution.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>Substitution.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>Substitution.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>Substitution.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>Substitution.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>Substitution.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>Substitution.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>Substitution.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>Substitution.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>Substitution.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>Substitution.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>Substitution.Api.Control.Site</code> | ✅ 一致 |
| <code>Substitution.Api.Control.SkinID</code> | ✅ 一致 |
| <code>Substitution.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>Substitution.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>Substitution.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>Substitution.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>Substitution.Api.Control.Unload</code> | ✅ 一致 |
| <code>Substitution.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>Substitution.Api.Control.ViewState</code> | ✅ 一致 |
| <code>Substitution.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>Substitution.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>Substitution.Api.Control.Visible</code> | ✅ 一致 |
| <code>Substitution.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>Substitution.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>Substitution.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>Substitution.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>Substitution.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>Substitution.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>Substitution.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>Substitution.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>Substitution.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>Substitution.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>Substitution.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>Substitution.Api.WebControls.Substitution.#ctor</code> | ✅ 一致 |
| <code>Substitution.Api.WebControls.Substitution.CreateControlCollection</code> | ✅ 一致 |
| <code>Substitution.Api.WebControls.Substitution.MethodName</code> | ✅ 一致 |
| <code>Substitution.Api.WebControls.Substitution.OnPreRender</code> | ✅ 一致 |
| <code>Substitution.Api.WebControls.Substitution.Render</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>Substitution.(case list)</code> | ✅ 一致 |
| <code>Substitution.(default)</code> | ✅ 一致 |
| <code>Substitution.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>Substitution.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>Substitution.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>Substitution.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>Substitution.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Substitution.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Substitution.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Substitution.ClientIDMode=Static</code> | ✅ 一致 |
| <code>Substitution.EnableTheming=false</code> | ✅ 一致 |
| <code>Substitution.EnableTheming=true</code> | ✅ 一致 |
| <code>Substitution.EnableViewState=false</code> | ✅ 一致 |
| <code>Substitution.EnableViewState=true</code> | ✅ 一致 |
| <code>Substitution.ID=empty</code> | ✅ 一致 |
| <code>Substitution.ID=japanese</code> | ✅ 一致 |
| <code>Substitution.ID=markup</code> | ✅ 一致 |
| <code>Substitution.ID=text</code> | ✅ 一致 |
| <code>Substitution.MethodName=SubstitutionText</code> | ✅ 一致 |
| <code>Substitution.MethodName=SubstitutionText+EnableViewState=false</code> | ✅ 一致 |
| <code>Substitution.MethodName=SubstitutionText+Visible=false</code> | ✅ 一致 |
| <code>Substitution.MethodName=empty</code> | ✅ 一致 |
| <code>Substitution.MethodName=japanese</code> | ✅ 一致 |
| <code>Substitution.MethodName=markup</code> | ✅ 一致 |
| <code>Substitution.MethodName=missing</code> | ✅ 一致 |
| <code>Substitution.MethodName=text</code> | ✅ 一致 |
| <code>Substitution.SkinID=empty</code> | ✅ 一致 |
| <code>Substitution.SkinID=japanese</code> | ✅ 一致 |
| <code>Substitution.SkinID=markup</code> | ✅ 一致 |
| <code>Substitution.SkinID=text</code> | ✅ 一致 |
| <code>Substitution.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>Substitution.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>Substitution.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>Substitution.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>Substitution.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>Substitution.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>Substitution.Visible=false</code> | ✅ 一致 |
| <code>Substitution.Visible=true</code> | ✅ 一致 |
| <code>Substitution[naming container].(default)</code> | ✅ 一致 |
| <code>Substitution[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Substitution[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Substitution[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Substitution[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>Substitution[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| Substitution [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| Substitution [viewstate][dynamic] posted: files, view state, what code set (a table&#39;s rows and a literal control&#39;s text are not kept) | ✅ 一致 |
| Substitution [viewstate] the next view (NextView bubbled from a button) | ✅ 一致 |
| Substitution [viewstate] switched by ID and by index: the first view&#39;s text box kept its value | ✅ 一致 |
| Substitution [dynamic][viewstate] a dynamic multi-view&#39;s next view | ✅ 一致 |
| Substitution [viewstate] a calendar&#39;s day chosen | ✅ 一致 |
| Substitution [viewstate] a calendar&#39;s week chosen | ❌ 未実行 |
| Substitution [viewstate] a calendar&#39;s next month | ✅ 一致 |
| Substitution [viewstate] a dynamic calendar&#39;s day chosen | ✅ 一致 |
| Substitution [viewstate] an image map&#39;s hot spot clicked (PostBack) | ✅ 一致 |
| Substitution [viewstate] a dynamic image map&#39;s hot spot clicked | ✅ 一致 |
| Substitution [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| Substitution [viewstate] a file upload validated on the server (g1): no file posted | ✅ 一致 |
| Substitution [viewstate] a postback event to a table (not registered for event validation): rejected | ✅ 一致 |

</details>
