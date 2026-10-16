# HtmlInputButton パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 02:05
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 132 | 132 | 0 | 0 |
| ② HTML | 70 | 70 | 0 | 0 |
| ③ シナリオ | 5 | 5 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 141 件中 141 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
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
| ポストバックイベント | 0 | 2 | ✅ |
| CausesValidation / ValidationGroup | 8 | 0 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlInputButton.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.HtmlControls.HtmlInputButton.#ctor</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.HtmlControls.HtmlInputButton.CausesValidation</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.HtmlControls.HtmlInputButton.OnPreRender</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.HtmlControls.HtmlInputButton.OnServerClick</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.HtmlControls.HtmlInputButton.RaisePostBackEvent</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.HtmlControls.HtmlInputButton.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.HtmlControls.HtmlInputButton.ServerClick</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.HtmlControls.HtmlInputButton.ValidationGroup</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.HtmlControls.HtmlInputControl.#ctor</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.HtmlControls.HtmlInputControl.Name</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.HtmlControls.HtmlInputControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.HtmlControls.HtmlInputControl.Type</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.HtmlControls.HtmlInputControl.Value</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.IPostBackEventHandler.RaisePostBackEvent</code> | ✅ 一致 |
| <code>HtmlInputButton.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlInputButton.(case list)</code> | ✅ 一致 |
| <code>HtmlInputButton.(default)</code> | ✅ 一致 |
| <code>HtmlInputButton.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlInputButton.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlInputButton.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlInputButton.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlInputButton.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlInputButton.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlInputButton.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlInputButton.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlInputButton.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlInputButton.CausesValidation=false</code> | ✅ 一致 |
| <code>HtmlInputButton.CausesValidation=false+ValidationGroup=text</code> | ✅ 一致 |
| <code>HtmlInputButton.CausesValidation=true</code> | ✅ 一致 |
| <code>HtmlInputButton.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlInputButton.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputButton.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlInputButton.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlInputButton.Disabled=false</code> | ✅ 一致 |
| <code>HtmlInputButton.Disabled=true</code> | ✅ 一致 |
| <code>HtmlInputButton.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlInputButton.Disabled=true+CausesValidation=false</code> | ✅ 一致 |
| <code>HtmlInputButton.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlInputButton.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlInputButton.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputButton.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlInputButton.ID=empty</code> | ✅ 一致 |
| <code>HtmlInputButton.ID=japanese</code> | ✅ 一致 |
| <code>HtmlInputButton.ID=markup</code> | ✅ 一致 |
| <code>HtmlInputButton.ID=text</code> | ✅ 一致 |
| <code>HtmlInputButton.Name=empty</code> | ✅ 一致 |
| <code>HtmlInputButton.Name=group</code> | ✅ 一致 |
| <code>HtmlInputButton.Name=group+Value=text</code> | ✅ 一致 |
| <code>HtmlInputButton.Name=japanese</code> | ✅ 一致 |
| <code>HtmlInputButton.Name=markup</code> | ✅ 一致 |
| <code>HtmlInputButton.Name=text</code> | ✅ 一致 |
| <code>HtmlInputButton.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlInputButton.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlInputButton.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlInputButton.SkinID=text</code> | ✅ 一致 |
| <code>HtmlInputButton.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlInputButton.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlInputButton.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlInputButton.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlInputButton.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlInputButton.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlInputButton.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputButton.ValidationGroup=empty</code> | ✅ 一致 |
| <code>HtmlInputButton.ValidationGroup=japanese</code> | ✅ 一致 |
| <code>HtmlInputButton.ValidationGroup=markup</code> | ✅ 一致 |
| <code>HtmlInputButton.ValidationGroup=text</code> | ✅ 一致 |
| <code>HtmlInputButton.Value=empty</code> | ✅ 一致 |
| <code>HtmlInputButton.Value=japanese</code> | ✅ 一致 |
| <code>HtmlInputButton.Value=markup</code> | ✅ 一致 |
| <code>HtmlInputButton.Value=markup+Disabled=true</code> | ✅ 一致 |
| <code>HtmlInputButton.Value=markup+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputButton.Value=text</code> | ✅ 一致 |
| <code>HtmlInputButton.Value=text+Visible=false</code> | ✅ 一致 |
| <code>HtmlInputButton.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlInputButton.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlInputButton.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputButton.Visible=false</code> | ✅ 一致 |
| <code>HtmlInputButton.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputButton.Visible=true</code> | ✅ 一致 |
| <code>HtmlInputButton[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlInputButton[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlInputButton[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputButton[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlInputButton[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlInputButton[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlInputButton [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlInputButton [viewstate][dynamic] every input posted by a submit input | ✅ 一致 |
| HtmlInputButton [postbackevent] its server click | ✅ 一致 |
| HtmlInputButton [postbackevent][dynamic] a dynamic one&#39;s server click | ✅ 一致 |
| HtmlInputButton [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
