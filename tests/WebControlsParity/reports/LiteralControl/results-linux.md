# LiteralControl パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 02:38
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 109 | 109 | 0 | 0 |
| ② HTML | 42 | 42 | 0 | 0 |
| ③ シナリオ | 14 | 14 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 117 件中 117 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 10 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 14 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 34 | 1 | ✅ |
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
| <code>LiteralControl.Api.Control.#ctor</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.Adapter</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.ClientID</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.Context</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.Controls</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.DataBind</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.Dispose</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.Disposed</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.Events</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.FindControl</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.Focus</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.HasControls</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.ID</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.Init</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.Load</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.OnInit</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.Page</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.Parent</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.PreRender</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.Render</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.Site</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.SkinID</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.Unload</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.ViewState</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>LiteralControl.Api.Control.Visible</code> | ✅ 一致 |
| <code>LiteralControl.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>LiteralControl.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>LiteralControl.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>LiteralControl.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>LiteralControl.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>LiteralControl.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>LiteralControl.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>LiteralControl.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>LiteralControl.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>LiteralControl.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>LiteralControl.Api.ITextControl.Text</code> | ✅ 一致 |
| <code>LiteralControl.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>LiteralControl.Api.LiteralControl.#ctor</code> | ✅ 一致 |
| <code>LiteralControl.Api.LiteralControl.CreateControlCollection</code> | ✅ 一致 |
| <code>LiteralControl.Api.LiteralControl.Render</code> | ✅ 一致 |
| <code>LiteralControl.Api.LiteralControl.Text</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>LiteralControl.(case list)</code> | ✅ 一致 |
| <code>LiteralControl.(default)</code> | ✅ 一致 |
| <code>LiteralControl.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>LiteralControl.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>LiteralControl.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>LiteralControl.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>LiteralControl.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>LiteralControl.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>LiteralControl.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>LiteralControl.ClientIDMode=Static</code> | ✅ 一致 |
| <code>LiteralControl.EnableTheming=false</code> | ✅ 一致 |
| <code>LiteralControl.EnableTheming=true</code> | ✅ 一致 |
| <code>LiteralControl.EnableViewState=false</code> | ✅ 一致 |
| <code>LiteralControl.EnableViewState=true</code> | ✅ 一致 |
| <code>LiteralControl.ID=empty</code> | ✅ 一致 |
| <code>LiteralControl.ID=japanese</code> | ✅ 一致 |
| <code>LiteralControl.ID=markup</code> | ✅ 一致 |
| <code>LiteralControl.ID=text</code> | ✅ 一致 |
| <code>LiteralControl.SkinID=empty</code> | ✅ 一致 |
| <code>LiteralControl.SkinID=japanese</code> | ✅ 一致 |
| <code>LiteralControl.SkinID=markup</code> | ✅ 一致 |
| <code>LiteralControl.SkinID=text</code> | ✅ 一致 |
| <code>LiteralControl.Text=empty</code> | ✅ 一致 |
| <code>LiteralControl.Text=japanese</code> | ✅ 一致 |
| <code>LiteralControl.Text=markup</code> | ✅ 一致 |
| <code>LiteralControl.Text=markup+EnableViewState=false</code> | ✅ 一致 |
| <code>LiteralControl.Text=text</code> | ✅ 一致 |
| <code>LiteralControl.Text=text+Visible=false</code> | ✅ 一致 |
| <code>LiteralControl.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>LiteralControl.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>LiteralControl.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>LiteralControl.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>LiteralControl.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>LiteralControl.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>LiteralControl.Visible=false</code> | ✅ 一致 |
| <code>LiteralControl.Visible=true</code> | ✅ 一致 |
| <code>LiteralControl[naming container].(default)</code> | ✅ 一致 |
| <code>LiteralControl[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>LiteralControl[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>LiteralControl[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>LiteralControl[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>LiteralControl[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| LiteralControl [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| LiteralControl [viewstate][dynamic] posted: files, view state, what code set (a table&#39;s rows and a literal control&#39;s text are not kept) | ✅ 一致 |
| LiteralControl [viewstate] the next view (NextView bubbled from a button) | ✅ 一致 |
| LiteralControl [viewstate] switched by ID and by index: the first view&#39;s text box kept its value | ✅ 一致 |
| LiteralControl [dynamic][viewstate] a dynamic multi-view&#39;s next view | ✅ 一致 |
| LiteralControl [viewstate] a calendar&#39;s day chosen | ✅ 一致 |
| LiteralControl [viewstate] a calendar&#39;s week chosen | ❌ 未実行 |
| LiteralControl [viewstate] a calendar&#39;s next month | ✅ 一致 |
| LiteralControl [viewstate] a dynamic calendar&#39;s day chosen | ✅ 一致 |
| LiteralControl [viewstate] an image map&#39;s hot spot clicked (PostBack) | ✅ 一致 |
| LiteralControl [viewstate] a dynamic image map&#39;s hot spot clicked | ✅ 一致 |
| LiteralControl [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| LiteralControl [viewstate] a file upload validated on the server (g1): no file posted | ✅ 一致 |
| LiteralControl [viewstate] a postback event to a table (not registered for event validation): rejected | ✅ 一致 |

</details>
