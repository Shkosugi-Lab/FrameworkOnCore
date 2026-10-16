# HiddenField パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 01:55
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 118 | 118 | 0 | 0 |
| ② HTML | 42 | 42 | 0 | 0 |
| ③ シナリオ | 6 | 6 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 125 件中 125 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 10 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 6 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 34 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 2 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| ポストバックデータの取り込み | 0 | 1 | ✅ |
| 変更イベント（*Changed、ServerChange） | 0 | 2 | ✅ |
| リクエスト検証（ValidateRequestMode） | 3 | 1 | ✅ |
| 双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値） | 0 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>HiddenField.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.Context</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.Controls</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.Events</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.Focus</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.ID</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.Init</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.Load</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.Page</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.Parent</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.Render</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.Site</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.Unload</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HiddenField.Api.Control.Visible</code> | ✅ 一致 |
| <code>HiddenField.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HiddenField.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HiddenField.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HiddenField.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HiddenField.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HiddenField.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HiddenField.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HiddenField.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HiddenField.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HiddenField.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HiddenField.Api.IPostBackDataHandler.LoadPostData</code> | ✅ 一致 |
| <code>HiddenField.Api.IPostBackDataHandler.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>HiddenField.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>HiddenField.Api.WebControls.HiddenField.#ctor</code> | ✅ 一致 |
| <code>HiddenField.Api.WebControls.HiddenField.CreateControlCollection</code> | ✅ 一致 |
| <code>HiddenField.Api.WebControls.HiddenField.EnableTheming</code> | ✅ 一致 |
| <code>HiddenField.Api.WebControls.HiddenField.Focus</code> | ✅ 一致 |
| <code>HiddenField.Api.WebControls.HiddenField.LoadPostData</code> | ✅ 一致 |
| <code>HiddenField.Api.WebControls.HiddenField.OnPreRender</code> | ✅ 一致 |
| <code>HiddenField.Api.WebControls.HiddenField.OnValueChanged</code> | ✅ 一致 |
| <code>HiddenField.Api.WebControls.HiddenField.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>HiddenField.Api.WebControls.HiddenField.Render</code> | ✅ 一致 |
| <code>HiddenField.Api.WebControls.HiddenField.SkinID</code> | ✅ 一致 |
| <code>HiddenField.Api.WebControls.HiddenField.Value</code> | ✅ 一致 |
| <code>HiddenField.Api.WebControls.HiddenField.ValueChanged</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HiddenField.(case list)</code> | ✅ 一致 |
| <code>HiddenField.(default)</code> | ✅ 一致 |
| <code>HiddenField.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HiddenField.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HiddenField.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HiddenField.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HiddenField.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HiddenField.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HiddenField.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HiddenField.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HiddenField.EnableTheming=false</code> | ✅ 一致 |
| <code>HiddenField.EnableTheming=true</code> | ✅ 一致 |
| <code>HiddenField.EnableViewState=false</code> | ✅ 一致 |
| <code>HiddenField.EnableViewState=true</code> | ✅ 一致 |
| <code>HiddenField.ID=empty</code> | ✅ 一致 |
| <code>HiddenField.ID=japanese</code> | ✅ 一致 |
| <code>HiddenField.ID=markup</code> | ✅ 一致 |
| <code>HiddenField.ID=text</code> | ✅ 一致 |
| <code>HiddenField.SkinID=empty</code> | ✅ 一致 |
| <code>HiddenField.SkinID=japanese</code> | ✅ 一致 |
| <code>HiddenField.SkinID=markup</code> | ✅ 一致 |
| <code>HiddenField.SkinID=text</code> | ✅ 一致 |
| <code>HiddenField.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HiddenField.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HiddenField.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HiddenField.Value=empty</code> | ✅ 一致 |
| <code>HiddenField.Value=japanese</code> | ✅ 一致 |
| <code>HiddenField.Value=markup</code> | ✅ 一致 |
| <code>HiddenField.Value=markup+EnableViewState=false</code> | ✅ 一致 |
| <code>HiddenField.Value=text</code> | ✅ 一致 |
| <code>HiddenField.Value=text+Visible=false</code> | ✅ 一致 |
| <code>HiddenField.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HiddenField.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HiddenField.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HiddenField.Visible=false</code> | ✅ 一致 |
| <code>HiddenField.Visible=true</code> | ✅ 一致 |
| <code>HiddenField[naming container].(default)</code> | ✅ 一致 |
| <code>HiddenField[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HiddenField[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HiddenField[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HiddenField[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HiddenField[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HiddenField [render][databind][dynamic] first request | ✅ 一致 |
| HiddenField [postdata][change][dynamic] values changed by script | ✅ 一致 |
| HiddenField [viewstate][change] posted again unchanged | ✅ 一致 |
| HiddenField [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| HiddenField [bind] two-way binding in a template | ✅ 一致 |
| HiddenField [requestvalidation] markup posted | ✅ 一致 |

</details>
