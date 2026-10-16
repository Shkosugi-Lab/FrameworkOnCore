# HtmlInputReset パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-11 23:57
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 136 | 136 | 0 | 0 |
| ② HTML | 69 | 69 | 0 | 0 |
| ③ シナリオ | 4 | 4 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 146 件中 146 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 14 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 4 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 61 | 1 | ✅ |
| ViewState の保存と復元 | 7 | 2 | ✅ |
| Visible=false（描画されず状態は保持） | 3 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 3 | 0 | ✅ |
| ポストバックイベント | 0 | 1 | ✅ |
| CausesValidation / ValidationGroup | 7 | 0 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlInputReset.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlInputButton.#ctor</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlInputButton.CausesValidation</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlInputButton.OnPreRender</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlInputButton.OnServerClick</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlInputButton.RaisePostBackEvent</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlInputButton.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlInputButton.ServerClick</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlInputButton.ValidationGroup</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlInputControl.#ctor</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlInputControl.Name</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlInputControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlInputControl.Type</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlInputControl.Value</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlInputReset.#ctor</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlInputReset.CausesValidation</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlInputReset.ServerClick</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.HtmlControls.HtmlInputReset.ValidationGroup</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.IPostBackEventHandler.RaisePostBackEvent</code> | ✅ 一致 |
| <code>HtmlInputReset.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlInputReset.(case list)</code> | ✅ 一致 |
| <code>HtmlInputReset.(default)</code> | ✅ 一致 |
| <code>HtmlInputReset.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlInputReset.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlInputReset.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlInputReset.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlInputReset.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlInputReset.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlInputReset.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlInputReset.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlInputReset.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlInputReset.CausesValidation=false</code> | ✅ 一致 |
| <code>HtmlInputReset.CausesValidation=false+ValidationGroup=text</code> | ✅ 一致 |
| <code>HtmlInputReset.CausesValidation=true</code> | ✅ 一致 |
| <code>HtmlInputReset.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlInputReset.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputReset.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlInputReset.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlInputReset.Disabled=false</code> | ✅ 一致 |
| <code>HtmlInputReset.Disabled=true</code> | ✅ 一致 |
| <code>HtmlInputReset.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlInputReset.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlInputReset.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlInputReset.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputReset.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlInputReset.ID=empty</code> | ✅ 一致 |
| <code>HtmlInputReset.ID=japanese</code> | ✅ 一致 |
| <code>HtmlInputReset.ID=markup</code> | ✅ 一致 |
| <code>HtmlInputReset.ID=text</code> | ✅ 一致 |
| <code>HtmlInputReset.Name=empty</code> | ✅ 一致 |
| <code>HtmlInputReset.Name=group</code> | ✅ 一致 |
| <code>HtmlInputReset.Name=group+Value=text</code> | ✅ 一致 |
| <code>HtmlInputReset.Name=japanese</code> | ✅ 一致 |
| <code>HtmlInputReset.Name=markup</code> | ✅ 一致 |
| <code>HtmlInputReset.Name=text</code> | ✅ 一致 |
| <code>HtmlInputReset.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlInputReset.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlInputReset.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlInputReset.SkinID=text</code> | ✅ 一致 |
| <code>HtmlInputReset.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlInputReset.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlInputReset.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlInputReset.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlInputReset.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlInputReset.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlInputReset.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputReset.ValidationGroup=empty</code> | ✅ 一致 |
| <code>HtmlInputReset.ValidationGroup=japanese</code> | ✅ 一致 |
| <code>HtmlInputReset.ValidationGroup=markup</code> | ✅ 一致 |
| <code>HtmlInputReset.ValidationGroup=text</code> | ✅ 一致 |
| <code>HtmlInputReset.Value=empty</code> | ✅ 一致 |
| <code>HtmlInputReset.Value=japanese</code> | ✅ 一致 |
| <code>HtmlInputReset.Value=markup</code> | ✅ 一致 |
| <code>HtmlInputReset.Value=markup+Disabled=true</code> | ✅ 一致 |
| <code>HtmlInputReset.Value=markup+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputReset.Value=text</code> | ✅ 一致 |
| <code>HtmlInputReset.Value=text+Visible=false</code> | ✅ 一致 |
| <code>HtmlInputReset.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlInputReset.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlInputReset.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputReset.Visible=false</code> | ✅ 一致 |
| <code>HtmlInputReset.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputReset.Visible=true</code> | ✅ 一致 |
| <code>HtmlInputReset[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlInputReset[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlInputReset[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputReset[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlInputReset[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlInputReset[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlInputReset [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlInputReset [viewstate][dynamic] every input posted by a submit input | ✅ 一致 |
| HtmlInputReset [postbackevent] a reset input resets the form without a request | ✅ 一致 |
| HtmlInputReset [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
