# Localize パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 00:22
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 113 | 113 | 0 | 0 |
| ② HTML | 48 | 48 | 0 | 0 |
| ③ シナリオ | 14 | 14 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 120 件中 120 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 11 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 14 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 40 | 1 | ✅ |
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
| <code>Localize.Api.Control.#ctor</code> | ✅ 一致 |
| <code>Localize.Api.Control.Adapter</code> | ✅ 一致 |
| <code>Localize.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>Localize.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>Localize.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>Localize.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>Localize.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>Localize.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>Localize.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>Localize.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>Localize.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>Localize.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>Localize.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>Localize.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>Localize.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>Localize.Api.Control.ClientID</code> | ✅ 一致 |
| <code>Localize.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>Localize.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>Localize.Api.Control.Context</code> | ✅ 一致 |
| <code>Localize.Api.Control.Controls</code> | ✅ 一致 |
| <code>Localize.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>Localize.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>Localize.Api.Control.DataBind</code> | ✅ 一致 |
| <code>Localize.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>Localize.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>Localize.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>Localize.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>Localize.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>Localize.Api.Control.Dispose</code> | ✅ 一致 |
| <code>Localize.Api.Control.Disposed</code> | ✅ 一致 |
| <code>Localize.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>Localize.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>Localize.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>Localize.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>Localize.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>Localize.Api.Control.Events</code> | ✅ 一致 |
| <code>Localize.Api.Control.FindControl</code> | ✅ 一致 |
| <code>Localize.Api.Control.Focus</code> | ✅ 一致 |
| <code>Localize.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>Localize.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>Localize.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>Localize.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>Localize.Api.Control.HasControls</code> | ✅ 一致 |
| <code>Localize.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>Localize.Api.Control.ID</code> | ✅ 一致 |
| <code>Localize.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>Localize.Api.Control.Init</code> | ✅ 一致 |
| <code>Localize.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>Localize.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>Localize.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>Localize.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>Localize.Api.Control.Load</code> | ✅ 一致 |
| <code>Localize.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>Localize.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>Localize.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>Localize.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>Localize.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>Localize.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>Localize.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>Localize.Api.Control.OnInit</code> | ✅ 一致 |
| <code>Localize.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>Localize.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>Localize.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>Localize.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>Localize.Api.Control.Page</code> | ✅ 一致 |
| <code>Localize.Api.Control.Parent</code> | ✅ 一致 |
| <code>Localize.Api.Control.PreRender</code> | ✅ 一致 |
| <code>Localize.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>Localize.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>Localize.Api.Control.Render</code> | ✅ 一致 |
| <code>Localize.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>Localize.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>Localize.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>Localize.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>Localize.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>Localize.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>Localize.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>Localize.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>Localize.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>Localize.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>Localize.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>Localize.Api.Control.Site</code> | ✅ 一致 |
| <code>Localize.Api.Control.SkinID</code> | ✅ 一致 |
| <code>Localize.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>Localize.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>Localize.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>Localize.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>Localize.Api.Control.Unload</code> | ✅ 一致 |
| <code>Localize.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>Localize.Api.Control.ViewState</code> | ✅ 一致 |
| <code>Localize.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>Localize.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>Localize.Api.Control.Visible</code> | ✅ 一致 |
| <code>Localize.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>Localize.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>Localize.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>Localize.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>Localize.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>Localize.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>Localize.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>Localize.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>Localize.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>Localize.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>Localize.Api.ITextControl.Text</code> | ✅ 一致 |
| <code>Localize.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>Localize.Api.WebControls.Literal.#ctor</code> | ✅ 一致 |
| <code>Localize.Api.WebControls.Literal.AddParsedSubObject</code> | ✅ 一致 |
| <code>Localize.Api.WebControls.Literal.CreateControlCollection</code> | ✅ 一致 |
| <code>Localize.Api.WebControls.Literal.Focus</code> | ✅ 一致 |
| <code>Localize.Api.WebControls.Literal.Mode</code> | ✅ 一致 |
| <code>Localize.Api.WebControls.Literal.Render</code> | ✅ 一致 |
| <code>Localize.Api.WebControls.Literal.Text</code> | ✅ 一致 |
| <code>Localize.Api.WebControls.Localize.#ctor</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>Localize.(case list)</code> | ✅ 一致 |
| <code>Localize.(default)</code> | ✅ 一致 |
| <code>Localize.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>Localize.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>Localize.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>Localize.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>Localize.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Localize.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Localize.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Localize.ClientIDMode=Static</code> | ✅ 一致 |
| <code>Localize.EnableTheming=false</code> | ✅ 一致 |
| <code>Localize.EnableTheming=true</code> | ✅ 一致 |
| <code>Localize.EnableViewState=false</code> | ✅ 一致 |
| <code>Localize.EnableViewState=true</code> | ✅ 一致 |
| <code>Localize.ID=empty</code> | ✅ 一致 |
| <code>Localize.ID=japanese</code> | ✅ 一致 |
| <code>Localize.ID=markup</code> | ✅ 一致 |
| <code>Localize.ID=text</code> | ✅ 一致 |
| <code>Localize.Mode=Encode</code> | ✅ 一致 |
| <code>Localize.Mode=Encode+Text=markup</code> | ✅ 一致 |
| <code>Localize.Mode=PassThrough</code> | ✅ 一致 |
| <code>Localize.Mode=PassThrough+Text=markup</code> | ✅ 一致 |
| <code>Localize.Mode=Transform</code> | ✅ 一致 |
| <code>Localize.Mode=Transform+Text=markup</code> | ✅ 一致 |
| <code>Localize.SkinID=empty</code> | ✅ 一致 |
| <code>Localize.SkinID=japanese</code> | ✅ 一致 |
| <code>Localize.SkinID=markup</code> | ✅ 一致 |
| <code>Localize.SkinID=text</code> | ✅ 一致 |
| <code>Localize.Text=empty</code> | ✅ 一致 |
| <code>Localize.Text=japanese</code> | ✅ 一致 |
| <code>Localize.Text=markup</code> | ✅ 一致 |
| <code>Localize.Text=text</code> | ✅ 一致 |
| <code>Localize.Text=text+EnableViewState=false</code> | ✅ 一致 |
| <code>Localize.Text=text+Visible=false</code> | ✅ 一致 |
| <code>Localize.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>Localize.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>Localize.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>Localize.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>Localize.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>Localize.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>Localize.Visible=false</code> | ✅ 一致 |
| <code>Localize.Visible=true</code> | ✅ 一致 |
| <code>Localize[naming container].(default)</code> | ✅ 一致 |
| <code>Localize[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Localize[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Localize[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Localize[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>Localize[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| Localize [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| Localize [viewstate][dynamic] posted: files, view state, what code set (a table&#39;s rows and a literal control&#39;s text are not kept) | ✅ 一致 |
| Localize [viewstate] the next view (NextView bubbled from a button) | ✅ 一致 |
| Localize [viewstate] switched by ID and by index: the first view&#39;s text box kept its value | ✅ 一致 |
| Localize [dynamic][viewstate] a dynamic multi-view&#39;s next view | ✅ 一致 |
| Localize [viewstate] a calendar&#39;s day chosen | ✅ 一致 |
| Localize [viewstate] a calendar&#39;s week chosen | ❌ 未実行 |
| Localize [viewstate] a calendar&#39;s next month | ✅ 一致 |
| Localize [viewstate] a dynamic calendar&#39;s day chosen | ✅ 一致 |
| Localize [viewstate] an image map&#39;s hot spot clicked (PostBack) | ✅ 一致 |
| Localize [viewstate] a dynamic image map&#39;s hot spot clicked | ✅ 一致 |
| Localize [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| Localize [viewstate] a file upload validated on the server (g1): no file posted | ✅ 一致 |
| Localize [viewstate] a postback event to a table (not registered for event validation): rejected | ✅ 一致 |

</details>
