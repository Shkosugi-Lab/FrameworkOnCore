# HtmlButton パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-11 23:42
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 135 | 135 | 0 | 0 |
| ② HTML | 66 | 66 | 0 | 0 |
| ③ シナリオ | 5 | 5 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 144 件中 144 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 14 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 5 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 58 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 2 | ✅ |
| Visible=false（描画されず状態は保持） | 3 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 3 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 3 | 0 | ✅ |
| ポストバックイベント | 0 | 2 | ✅ |
| CausesValidation / ValidationGroup | 7 | 0 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlButton.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlButton.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlButton.#ctor</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlButton.CausesValidation</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlButton.OnPreRender</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlButton.OnServerClick</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlButton.RaisePostBackEvent</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlButton.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlButton.ServerClick</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlButton.ValidationGroup</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlContainerControl.#ctor</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlContainerControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlContainerControl.InnerHtml</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlContainerControl.InnerText</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlContainerControl.LoadViewState</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlContainerControl.Render</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlContainerControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlContainerControl.RenderEndTag</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlButton.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlButton.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlButton.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlButton.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlButton.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlButton.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlButton.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlButton.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlButton.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlButton.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlButton.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlButton.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlButton.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlButton.Api.IPostBackEventHandler.RaisePostBackEvent</code> | ✅ 一致 |
| <code>HtmlButton.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlButton.(case list)</code> | ✅ 一致 |
| <code>HtmlButton.(default)</code> | ✅ 一致 |
| <code>HtmlButton.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlButton.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlButton.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlButton.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlButton.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlButton.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlButton.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlButton.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlButton.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlButton.CausesValidation=false</code> | ✅ 一致 |
| <code>HtmlButton.CausesValidation=false+ValidationGroup=text</code> | ✅ 一致 |
| <code>HtmlButton.CausesValidation=true</code> | ✅ 一致 |
| <code>HtmlButton.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlButton.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlButton.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlButton.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlButton.Disabled=false</code> | ✅ 一致 |
| <code>HtmlButton.Disabled=true</code> | ✅ 一致 |
| <code>HtmlButton.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlButton.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlButton.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlButton.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlButton.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlButton.ID=empty</code> | ✅ 一致 |
| <code>HtmlButton.ID=japanese</code> | ✅ 一致 |
| <code>HtmlButton.ID=markup</code> | ✅ 一致 |
| <code>HtmlButton.ID=text</code> | ✅ 一致 |
| <code>HtmlButton.InnerHtml=empty</code> | ✅ 一致 |
| <code>HtmlButton.InnerHtml=japanese</code> | ✅ 一致 |
| <code>HtmlButton.InnerHtml=markup</code> | ✅ 一致 |
| <code>HtmlButton.InnerHtml=markup+Disabled=true</code> | ✅ 一致 |
| <code>HtmlButton.InnerHtml=text</code> | ✅ 一致 |
| <code>HtmlButton.InnerText=empty</code> | ✅ 一致 |
| <code>HtmlButton.InnerText=japanese</code> | ✅ 一致 |
| <code>HtmlButton.InnerText=markup</code> | ✅ 一致 |
| <code>HtmlButton.InnerText=markup+Visible=false</code> | ✅ 一致 |
| <code>HtmlButton.InnerText=text</code> | ✅ 一致 |
| <code>HtmlButton.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlButton.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlButton.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlButton.SkinID=text</code> | ✅ 一致 |
| <code>HtmlButton.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlButton.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlButton.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlButton.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlButton.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlButton.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlButton.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlButton.ValidationGroup=empty</code> | ✅ 一致 |
| <code>HtmlButton.ValidationGroup=japanese</code> | ✅ 一致 |
| <code>HtmlButton.ValidationGroup=markup</code> | ✅ 一致 |
| <code>HtmlButton.ValidationGroup=text</code> | ✅ 一致 |
| <code>HtmlButton.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlButton.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlButton.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlButton.Visible=false</code> | ✅ 一致 |
| <code>HtmlButton.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlButton.Visible=true</code> | ✅ 一致 |
| <code>HtmlButton[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlButton[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlButton[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlButton[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlButton[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlButton[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlButton [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlButton [viewstate][dynamic] every input posted by a submit input | ✅ 一致 |
| HtmlButton [postbackevent] its server click | ✅ 一致 |
| HtmlButton [postbackevent][dynamic] a dynamic one&#39;s server click | ✅ 一致 |
| HtmlButton [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
