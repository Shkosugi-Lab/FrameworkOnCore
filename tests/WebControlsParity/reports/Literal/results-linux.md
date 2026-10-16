# Literal パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 02:36
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 112 | 112 | 0 | 0 |
| ② HTML | 49 | 49 | 0 | 0 |
| ③ シナリオ | 3 | 3 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 119 件中 119 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 11 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 3 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 41 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 2 | ✅ |
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
| <code>Literal.Api.Control.#ctor</code> | ✅ 一致 |
| <code>Literal.Api.Control.Adapter</code> | ✅ 一致 |
| <code>Literal.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>Literal.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>Literal.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>Literal.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>Literal.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>Literal.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>Literal.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>Literal.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>Literal.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>Literal.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>Literal.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>Literal.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>Literal.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>Literal.Api.Control.ClientID</code> | ✅ 一致 |
| <code>Literal.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>Literal.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>Literal.Api.Control.Context</code> | ✅ 一致 |
| <code>Literal.Api.Control.Controls</code> | ✅ 一致 |
| <code>Literal.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>Literal.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>Literal.Api.Control.DataBind</code> | ✅ 一致 |
| <code>Literal.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>Literal.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>Literal.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>Literal.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>Literal.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>Literal.Api.Control.Dispose</code> | ✅ 一致 |
| <code>Literal.Api.Control.Disposed</code> | ✅ 一致 |
| <code>Literal.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>Literal.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>Literal.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>Literal.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>Literal.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>Literal.Api.Control.Events</code> | ✅ 一致 |
| <code>Literal.Api.Control.FindControl</code> | ✅ 一致 |
| <code>Literal.Api.Control.Focus</code> | ✅ 一致 |
| <code>Literal.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>Literal.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>Literal.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>Literal.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>Literal.Api.Control.HasControls</code> | ✅ 一致 |
| <code>Literal.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>Literal.Api.Control.ID</code> | ✅ 一致 |
| <code>Literal.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>Literal.Api.Control.Init</code> | ✅ 一致 |
| <code>Literal.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>Literal.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>Literal.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>Literal.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>Literal.Api.Control.Load</code> | ✅ 一致 |
| <code>Literal.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>Literal.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>Literal.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>Literal.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>Literal.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>Literal.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>Literal.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>Literal.Api.Control.OnInit</code> | ✅ 一致 |
| <code>Literal.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>Literal.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>Literal.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>Literal.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>Literal.Api.Control.Page</code> | ✅ 一致 |
| <code>Literal.Api.Control.Parent</code> | ✅ 一致 |
| <code>Literal.Api.Control.PreRender</code> | ✅ 一致 |
| <code>Literal.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>Literal.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>Literal.Api.Control.Render</code> | ✅ 一致 |
| <code>Literal.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>Literal.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>Literal.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>Literal.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>Literal.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>Literal.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>Literal.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>Literal.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>Literal.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>Literal.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>Literal.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>Literal.Api.Control.Site</code> | ✅ 一致 |
| <code>Literal.Api.Control.SkinID</code> | ✅ 一致 |
| <code>Literal.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>Literal.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>Literal.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>Literal.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>Literal.Api.Control.Unload</code> | ✅ 一致 |
| <code>Literal.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>Literal.Api.Control.ViewState</code> | ✅ 一致 |
| <code>Literal.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>Literal.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>Literal.Api.Control.Visible</code> | ✅ 一致 |
| <code>Literal.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>Literal.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>Literal.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>Literal.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>Literal.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>Literal.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>Literal.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>Literal.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>Literal.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>Literal.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>Literal.Api.ITextControl.Text</code> | ✅ 一致 |
| <code>Literal.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>Literal.Api.WebControls.Literal.#ctor</code> | ✅ 一致 |
| <code>Literal.Api.WebControls.Literal.AddParsedSubObject</code> | ✅ 一致 |
| <code>Literal.Api.WebControls.Literal.CreateControlCollection</code> | ✅ 一致 |
| <code>Literal.Api.WebControls.Literal.Focus</code> | ✅ 一致 |
| <code>Literal.Api.WebControls.Literal.Mode</code> | ✅ 一致 |
| <code>Literal.Api.WebControls.Literal.Render</code> | ✅ 一致 |
| <code>Literal.Api.WebControls.Literal.Text</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>Literal.(case list)</code> | ✅ 一致 |
| <code>Literal.(default)</code> | ✅ 一致 |
| <code>Literal.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>Literal.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>Literal.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>Literal.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>Literal.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Literal.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Literal.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Literal.ClientIDMode=Static</code> | ✅ 一致 |
| <code>Literal.EnableTheming=false</code> | ✅ 一致 |
| <code>Literal.EnableTheming=true</code> | ✅ 一致 |
| <code>Literal.EnableViewState=false</code> | ✅ 一致 |
| <code>Literal.EnableViewState=true</code> | ✅ 一致 |
| <code>Literal.ID=empty</code> | ✅ 一致 |
| <code>Literal.ID=japanese</code> | ✅ 一致 |
| <code>Literal.ID=markup</code> | ✅ 一致 |
| <code>Literal.ID=text</code> | ✅ 一致 |
| <code>Literal.Mode=Encode</code> | ✅ 一致 |
| <code>Literal.Mode=Encode+Text=japanese</code> | ✅ 一致 |
| <code>Literal.Mode=Encode+Text=markup</code> | ✅ 一致 |
| <code>Literal.Mode=PassThrough</code> | ✅ 一致 |
| <code>Literal.Mode=PassThrough+Text=markup</code> | ✅ 一致 |
| <code>Literal.Mode=Transform</code> | ✅ 一致 |
| <code>Literal.Mode=Transform+Text=markup</code> | ✅ 一致 |
| <code>Literal.SkinID=empty</code> | ✅ 一致 |
| <code>Literal.SkinID=japanese</code> | ✅ 一致 |
| <code>Literal.SkinID=markup</code> | ✅ 一致 |
| <code>Literal.SkinID=text</code> | ✅ 一致 |
| <code>Literal.Text=empty</code> | ✅ 一致 |
| <code>Literal.Text=japanese</code> | ✅ 一致 |
| <code>Literal.Text=markup</code> | ✅ 一致 |
| <code>Literal.Text=text</code> | ✅ 一致 |
| <code>Literal.Text=text+EnableViewState=false</code> | ✅ 一致 |
| <code>Literal.Text=text+Visible=false</code> | ✅ 一致 |
| <code>Literal.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>Literal.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>Literal.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>Literal.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>Literal.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>Literal.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>Literal.Visible=false</code> | ✅ 一致 |
| <code>Literal.Visible=true</code> | ✅ 一致 |
| <code>Literal[naming container].(default)</code> | ✅ 一致 |
| <code>Literal[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Literal[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Literal[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Literal[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>Literal[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| Literal [render][databind][dynamic] first request | ✅ 一致 |
| Literal [viewstate][dynamic] posted (set in code: kept; without view state: lost) | ✅ 一致 |
| Literal [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
