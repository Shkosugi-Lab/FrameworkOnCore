# MultiView パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 00:31
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 124 | 124 | 0 | 0 |
| ② HTML | 45 | 45 | 0 | 0 |
| ③ シナリオ | 14 | 14 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 131 件中 131 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 10 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 14 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 36 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 13 | ❌ 1 件 |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 7 | 0 | ✅ |
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
| <code>MultiView.Api.Control.#ctor</code> | ✅ 一致 |
| <code>MultiView.Api.Control.Adapter</code> | ✅ 一致 |
| <code>MultiView.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>MultiView.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>MultiView.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>MultiView.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>MultiView.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>MultiView.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>MultiView.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>MultiView.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>MultiView.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>MultiView.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>MultiView.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>MultiView.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>MultiView.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>MultiView.Api.Control.ClientID</code> | ✅ 一致 |
| <code>MultiView.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>MultiView.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>MultiView.Api.Control.Context</code> | ✅ 一致 |
| <code>MultiView.Api.Control.Controls</code> | ✅ 一致 |
| <code>MultiView.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>MultiView.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>MultiView.Api.Control.DataBind</code> | ✅ 一致 |
| <code>MultiView.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>MultiView.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>MultiView.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>MultiView.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>MultiView.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>MultiView.Api.Control.Dispose</code> | ✅ 一致 |
| <code>MultiView.Api.Control.Disposed</code> | ✅ 一致 |
| <code>MultiView.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>MultiView.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>MultiView.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>MultiView.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>MultiView.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>MultiView.Api.Control.Events</code> | ✅ 一致 |
| <code>MultiView.Api.Control.FindControl</code> | ✅ 一致 |
| <code>MultiView.Api.Control.Focus</code> | ✅ 一致 |
| <code>MultiView.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>MultiView.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>MultiView.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>MultiView.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>MultiView.Api.Control.HasControls</code> | ✅ 一致 |
| <code>MultiView.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>MultiView.Api.Control.ID</code> | ✅ 一致 |
| <code>MultiView.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>MultiView.Api.Control.Init</code> | ✅ 一致 |
| <code>MultiView.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>MultiView.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>MultiView.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>MultiView.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>MultiView.Api.Control.Load</code> | ✅ 一致 |
| <code>MultiView.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>MultiView.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>MultiView.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>MultiView.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>MultiView.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>MultiView.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>MultiView.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>MultiView.Api.Control.OnInit</code> | ✅ 一致 |
| <code>MultiView.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>MultiView.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>MultiView.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>MultiView.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>MultiView.Api.Control.Page</code> | ✅ 一致 |
| <code>MultiView.Api.Control.Parent</code> | ✅ 一致 |
| <code>MultiView.Api.Control.PreRender</code> | ✅ 一致 |
| <code>MultiView.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>MultiView.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>MultiView.Api.Control.Render</code> | ✅ 一致 |
| <code>MultiView.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>MultiView.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>MultiView.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>MultiView.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>MultiView.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>MultiView.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>MultiView.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>MultiView.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>MultiView.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>MultiView.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>MultiView.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>MultiView.Api.Control.Site</code> | ✅ 一致 |
| <code>MultiView.Api.Control.SkinID</code> | ✅ 一致 |
| <code>MultiView.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>MultiView.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>MultiView.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>MultiView.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>MultiView.Api.Control.Unload</code> | ✅ 一致 |
| <code>MultiView.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>MultiView.Api.Control.ViewState</code> | ✅ 一致 |
| <code>MultiView.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>MultiView.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>MultiView.Api.Control.Visible</code> | ✅ 一致 |
| <code>MultiView.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>MultiView.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>MultiView.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>MultiView.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>MultiView.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>MultiView.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>MultiView.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>MultiView.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>MultiView.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>MultiView.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>MultiView.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>MultiView.Api.WebControls.MultiView.#ctor</code> | ✅ 一致 |
| <code>MultiView.Api.WebControls.MultiView.ActiveViewChanged</code> | ✅ 一致 |
| <code>MultiView.Api.WebControls.MultiView.ActiveViewIndex</code> | ✅ 一致 |
| <code>MultiView.Api.WebControls.MultiView.AddParsedSubObject</code> | ✅ 一致 |
| <code>MultiView.Api.WebControls.MultiView.CreateControlCollection</code> | ✅ 一致 |
| <code>MultiView.Api.WebControls.MultiView.EnableTheming</code> | ✅ 一致 |
| <code>MultiView.Api.WebControls.MultiView.GetActiveView</code> | ✅ 一致 |
| <code>MultiView.Api.WebControls.MultiView.LoadControlState</code> | ✅ 一致 |
| <code>MultiView.Api.WebControls.MultiView.NextViewCommandName</code> | ✅ 一致 |
| <code>MultiView.Api.WebControls.MultiView.OnActiveViewChanged</code> | ✅ 一致 |
| <code>MultiView.Api.WebControls.MultiView.OnBubbleEvent</code> | ✅ 一致 |
| <code>MultiView.Api.WebControls.MultiView.OnInit</code> | ✅ 一致 |
| <code>MultiView.Api.WebControls.MultiView.PreviousViewCommandName</code> | ✅ 一致 |
| <code>MultiView.Api.WebControls.MultiView.RemovedControl</code> | ✅ 一致 |
| <code>MultiView.Api.WebControls.MultiView.Render</code> | ✅ 一致 |
| <code>MultiView.Api.WebControls.MultiView.SaveControlState</code> | ✅ 一致 |
| <code>MultiView.Api.WebControls.MultiView.SetActiveView</code> | ✅ 一致 |
| <code>MultiView.Api.WebControls.MultiView.SwitchViewByIDCommandName</code> | ✅ 一致 |
| <code>MultiView.Api.WebControls.MultiView.SwitchViewByIndexCommandName</code> | ✅ 一致 |
| <code>MultiView.Api.WebControls.MultiView.Views</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>MultiView.(case list)</code> | ✅ 一致 |
| <code>MultiView.(default)</code> | ✅ 一致 |
| <code>MultiView.ActiveViewIndex=-1</code> | ✅ 一致 |
| <code>MultiView.ActiveViewIndex=-1+Visible=true</code> | ✅ 一致 |
| <code>MultiView.ActiveViewIndex=0</code> | ✅ 一致 |
| <code>MultiView.ActiveViewIndex=1</code> | ✅ 一致 |
| <code>MultiView.ActiveViewIndex=1+EnableViewState=false</code> | ✅ 一致 |
| <code>MultiView.ActiveViewIndex=1+Visible=false</code> | ✅ 一致 |
| <code>MultiView.ActiveViewIndex=2</code> | ✅ 一致 |
| <code>MultiView.ActiveViewIndex=3</code> | ✅ 一致 |
| <code>MultiView.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>MultiView.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>MultiView.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>MultiView.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>MultiView.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>MultiView.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>MultiView.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>MultiView.ClientIDMode=Static</code> | ✅ 一致 |
| <code>MultiView.EnableTheming=false</code> | ✅ 一致 |
| <code>MultiView.EnableTheming=true</code> | ✅ 一致 |
| <code>MultiView.EnableViewState=false</code> | ✅ 一致 |
| <code>MultiView.EnableViewState=true</code> | ✅ 一致 |
| <code>MultiView.ID=empty</code> | ✅ 一致 |
| <code>MultiView.ID=japanese</code> | ✅ 一致 |
| <code>MultiView.ID=markup</code> | ✅ 一致 |
| <code>MultiView.ID=text</code> | ✅ 一致 |
| <code>MultiView.SkinID=empty</code> | ✅ 一致 |
| <code>MultiView.SkinID=japanese</code> | ✅ 一致 |
| <code>MultiView.SkinID=markup</code> | ✅ 一致 |
| <code>MultiView.SkinID=text</code> | ✅ 一致 |
| <code>MultiView.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>MultiView.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>MultiView.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>MultiView.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>MultiView.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>MultiView.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>MultiView.Visible=false</code> | ✅ 一致 |
| <code>MultiView.Visible=true</code> | ✅ 一致 |
| <code>MultiView[naming container].(default)</code> | ✅ 一致 |
| <code>MultiView[naming container].ActiveViewIndex=1</code> | ✅ 一致 |
| <code>MultiView[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>MultiView[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>MultiView[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>MultiView[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>MultiView[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| MultiView [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| MultiView [viewstate][dynamic] posted: files, view state, what code set (a table&#39;s rows and a literal control&#39;s text are not kept) | ✅ 一致 |
| MultiView [viewstate] the next view (NextView bubbled from a button) | ✅ 一致 |
| MultiView [viewstate] switched by ID and by index: the first view&#39;s text box kept its value | ✅ 一致 |
| MultiView [dynamic][viewstate] a dynamic multi-view&#39;s next view | ✅ 一致 |
| MultiView [viewstate] a calendar&#39;s day chosen | ✅ 一致 |
| MultiView [viewstate] a calendar&#39;s week chosen | ❌ 未実行 |
| MultiView [viewstate] a calendar&#39;s next month | ✅ 一致 |
| MultiView [viewstate] a dynamic calendar&#39;s day chosen | ✅ 一致 |
| MultiView [viewstate] an image map&#39;s hot spot clicked (PostBack) | ✅ 一致 |
| MultiView [viewstate] a dynamic image map&#39;s hot spot clicked | ✅ 一致 |
| MultiView [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| MultiView [viewstate] a file upload validated on the server (g1): no file posted | ✅ 一致 |
| MultiView [viewstate] a postback event to a table (not registered for event validation): rejected | ✅ 一致 |

</details>
