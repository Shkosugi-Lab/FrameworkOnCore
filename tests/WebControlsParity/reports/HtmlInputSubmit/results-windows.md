# HtmlInputSubmit パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-11 23:58
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 133 | 133 | 0 | 0 |
| ② HTML | 70 | 70 | 0 | 0 |
| ③ シナリオ | 5 | 5 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 143 件中 143 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 14 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 5 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 62 | 1 | ✅ |
| ViewState の保存と復元 | 7 | 2 | ✅ |
| Visible=false（描画されず状態は保持） | 3 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 3 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 4 | 0 | ✅ |
| ポストバックイベント | 0 | 3 | ✅ |
| CausesValidation / ValidationGroup | 8 | 0 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlInputSubmit.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.HtmlControls.HtmlInputButton.#ctor</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.HtmlControls.HtmlInputButton.CausesValidation</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.HtmlControls.HtmlInputButton.OnPreRender</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.HtmlControls.HtmlInputButton.OnServerClick</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.HtmlControls.HtmlInputButton.RaisePostBackEvent</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.HtmlControls.HtmlInputButton.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.HtmlControls.HtmlInputButton.ServerClick</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.HtmlControls.HtmlInputButton.ValidationGroup</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.HtmlControls.HtmlInputControl.#ctor</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.HtmlControls.HtmlInputControl.Name</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.HtmlControls.HtmlInputControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.HtmlControls.HtmlInputControl.Type</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.HtmlControls.HtmlInputControl.Value</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.HtmlControls.HtmlInputSubmit.#ctor</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.IPostBackEventHandler.RaisePostBackEvent</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlInputSubmit.(case list)</code> | ✅ 一致 |
| <code>HtmlInputSubmit.(default)</code> | ✅ 一致 |
| <code>HtmlInputSubmit.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlInputSubmit.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlInputSubmit.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlInputSubmit.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlInputSubmit.CausesValidation=false</code> | ✅ 一致 |
| <code>HtmlInputSubmit.CausesValidation=false+ValidationGroup=text</code> | ✅ 一致 |
| <code>HtmlInputSubmit.CausesValidation=true</code> | ✅ 一致 |
| <code>HtmlInputSubmit.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlInputSubmit.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputSubmit.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlInputSubmit.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Disabled=false</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Disabled=true</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Disabled=true+CausesValidation=false</code> | ✅ 一致 |
| <code>HtmlInputSubmit.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlInputSubmit.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlInputSubmit.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputSubmit.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlInputSubmit.ID=empty</code> | ✅ 一致 |
| <code>HtmlInputSubmit.ID=japanese</code> | ✅ 一致 |
| <code>HtmlInputSubmit.ID=markup</code> | ✅ 一致 |
| <code>HtmlInputSubmit.ID=text</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Name=empty</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Name=group</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Name=group+Value=text</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Name=japanese</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Name=markup</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Name=text</code> | ✅ 一致 |
| <code>HtmlInputSubmit.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlInputSubmit.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlInputSubmit.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlInputSubmit.SkinID=text</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlInputSubmit.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlInputSubmit.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlInputSubmit.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputSubmit.ValidationGroup=empty</code> | ✅ 一致 |
| <code>HtmlInputSubmit.ValidationGroup=japanese</code> | ✅ 一致 |
| <code>HtmlInputSubmit.ValidationGroup=markup</code> | ✅ 一致 |
| <code>HtmlInputSubmit.ValidationGroup=text</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Value=empty</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Value=japanese</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Value=markup</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Value=markup+Disabled=true</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Value=markup+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Value=text</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Value=text+Visible=false</code> | ✅ 一致 |
| <code>HtmlInputSubmit.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlInputSubmit.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlInputSubmit.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Visible=false</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputSubmit.Visible=true</code> | ✅ 一致 |
| <code>HtmlInputSubmit[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlInputSubmit[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlInputSubmit[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputSubmit[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlInputSubmit[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlInputSubmit[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlInputSubmit [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlInputSubmit [viewstate][dynamic][postbackevent] every input posted by a submit input | ✅ 一致 |
| HtmlInputSubmit [postbackevent] its server click | ✅ 一致 |
| HtmlInputSubmit [postbackevent][dynamic] a dynamic one&#39;s server click | ✅ 一致 |
| HtmlInputSubmit [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
