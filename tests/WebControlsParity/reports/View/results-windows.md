# View パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 01:05
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 111 | 111 | 0 | 0 |
| ② HTML | 37 | 37 | 0 | 0 |
| ③ シナリオ | 14 | 14 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 118 件中 118 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 9 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 14 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 29 | 1 | ✅ |
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
| <code>View.Api.Control.#ctor</code> | ✅ 一致 |
| <code>View.Api.Control.Adapter</code> | ✅ 一致 |
| <code>View.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>View.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>View.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>View.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>View.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>View.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>View.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>View.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>View.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>View.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>View.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>View.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>View.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>View.Api.Control.ClientID</code> | ✅ 一致 |
| <code>View.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>View.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>View.Api.Control.Context</code> | ✅ 一致 |
| <code>View.Api.Control.Controls</code> | ✅ 一致 |
| <code>View.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>View.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>View.Api.Control.DataBind</code> | ✅ 一致 |
| <code>View.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>View.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>View.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>View.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>View.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>View.Api.Control.Dispose</code> | ✅ 一致 |
| <code>View.Api.Control.Disposed</code> | ✅ 一致 |
| <code>View.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>View.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>View.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>View.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>View.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>View.Api.Control.Events</code> | ✅ 一致 |
| <code>View.Api.Control.FindControl</code> | ✅ 一致 |
| <code>View.Api.Control.Focus</code> | ✅ 一致 |
| <code>View.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>View.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>View.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>View.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>View.Api.Control.HasControls</code> | ✅ 一致 |
| <code>View.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>View.Api.Control.ID</code> | ✅ 一致 |
| <code>View.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>View.Api.Control.Init</code> | ✅ 一致 |
| <code>View.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>View.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>View.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>View.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>View.Api.Control.Load</code> | ✅ 一致 |
| <code>View.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>View.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>View.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>View.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>View.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>View.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>View.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>View.Api.Control.OnInit</code> | ✅ 一致 |
| <code>View.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>View.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>View.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>View.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>View.Api.Control.Page</code> | ✅ 一致 |
| <code>View.Api.Control.Parent</code> | ✅ 一致 |
| <code>View.Api.Control.PreRender</code> | ✅ 一致 |
| <code>View.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>View.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>View.Api.Control.Render</code> | ✅ 一致 |
| <code>View.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>View.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>View.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>View.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>View.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>View.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>View.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>View.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>View.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>View.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>View.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>View.Api.Control.Site</code> | ✅ 一致 |
| <code>View.Api.Control.SkinID</code> | ✅ 一致 |
| <code>View.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>View.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>View.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>View.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>View.Api.Control.Unload</code> | ✅ 一致 |
| <code>View.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>View.Api.Control.ViewState</code> | ✅ 一致 |
| <code>View.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>View.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>View.Api.Control.Visible</code> | ✅ 一致 |
| <code>View.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>View.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>View.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>View.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>View.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>View.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>View.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>View.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>View.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>View.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>View.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>View.Api.WebControls.View.#ctor</code> | ✅ 一致 |
| <code>View.Api.WebControls.View.Activate</code> | ✅ 一致 |
| <code>View.Api.WebControls.View.Deactivate</code> | ✅ 一致 |
| <code>View.Api.WebControls.View.EnableTheming</code> | ✅ 一致 |
| <code>View.Api.WebControls.View.OnActivate</code> | ✅ 一致 |
| <code>View.Api.WebControls.View.OnDeactivate</code> | ✅ 一致 |
| <code>View.Api.WebControls.View.Visible</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>View.(case list)</code> | ✅ 一致 |
| <code>View.(default)</code> | ✅ 一致 |
| <code>View.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>View.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>View.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>View.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>View.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>View.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>View.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>View.ClientIDMode=Static</code> | ✅ 一致 |
| <code>View.EnableTheming=false</code> | ✅ 一致 |
| <code>View.EnableTheming=true</code> | ✅ 一致 |
| <code>View.EnableViewState=false</code> | ✅ 一致 |
| <code>View.EnableViewState=true</code> | ✅ 一致 |
| <code>View.ID=empty</code> | ✅ 一致 |
| <code>View.ID=japanese</code> | ✅ 一致 |
| <code>View.ID=markup</code> | ✅ 一致 |
| <code>View.ID=text</code> | ✅ 一致 |
| <code>View.SkinID=empty</code> | ✅ 一致 |
| <code>View.SkinID=japanese</code> | ✅ 一致 |
| <code>View.SkinID=markup</code> | ✅ 一致 |
| <code>View.SkinID=text</code> | ✅ 一致 |
| <code>View.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>View.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>View.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>View.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>View.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>View.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>View.Visible=false</code> | ✅ 一致 |
| <code>View.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>View.Visible=true</code> | ✅ 一致 |
| <code>View[naming container].(default)</code> | ✅ 一致 |
| <code>View[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>View[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>View[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>View[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>View[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| View [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| View [viewstate][dynamic] posted: files, view state, what code set (a table&#39;s rows and a literal control&#39;s text are not kept) | ✅ 一致 |
| View [viewstate] the next view (NextView bubbled from a button) | ✅ 一致 |
| View [viewstate] switched by ID and by index: the first view&#39;s text box kept its value | ✅ 一致 |
| View [dynamic][viewstate] a dynamic multi-view&#39;s next view | ✅ 一致 |
| View [viewstate] a calendar&#39;s day chosen | ✅ 一致 |
| View [viewstate] a calendar&#39;s week chosen | ❌ 未実行 |
| View [viewstate] a calendar&#39;s next month | ✅ 一致 |
| View [viewstate] a dynamic calendar&#39;s day chosen | ✅ 一致 |
| View [viewstate] an image map&#39;s hot spot clicked (PostBack) | ✅ 一致 |
| View [viewstate] a dynamic image map&#39;s hot spot clicked | ✅ 一致 |
| View [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| View [viewstate] a file upload validated on the server (g1): no file posted | ✅ 一致 |
| View [viewstate] a postback event to a table (not registered for event validation): rejected | ✅ 一致 |

</details>
