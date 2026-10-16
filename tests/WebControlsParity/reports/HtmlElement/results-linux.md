# HtmlElement パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 01:59
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 129 | 129 | 0 | 0 |
| ② HTML | 63 | 63 | 0 | 0 |
| ③ シナリオ | 3 | 3 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 138 件中 138 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 13 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 3 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 55 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 2 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 2 | 0 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlElement.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlElement.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlElement.Api.HtmlControls.HtmlContainerControl.#ctor</code> | ✅ 一致 |
| <code>HtmlElement.Api.HtmlControls.HtmlContainerControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlElement.Api.HtmlControls.HtmlContainerControl.InnerHtml</code> | ✅ 一致 |
| <code>HtmlElement.Api.HtmlControls.HtmlContainerControl.InnerText</code> | ✅ 一致 |
| <code>HtmlElement.Api.HtmlControls.HtmlContainerControl.LoadViewState</code> | ✅ 一致 |
| <code>HtmlElement.Api.HtmlControls.HtmlContainerControl.Render</code> | ✅ 一致 |
| <code>HtmlElement.Api.HtmlControls.HtmlContainerControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlElement.Api.HtmlControls.HtmlContainerControl.RenderEndTag</code> | ✅ 一致 |
| <code>HtmlElement.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlElement.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlElement.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlElement.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlElement.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlElement.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlElement.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlElement.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlElement.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlElement.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlElement.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlElement.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlElement.Api.HtmlControls.HtmlElement.#ctor</code> | ✅ 一致 |
| <code>HtmlElement.Api.HtmlControls.HtmlElement.Manifest</code> | ✅ 一致 |
| <code>HtmlElement.Api.HtmlControls.HtmlElement.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlElement.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlElement.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlElement.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlElement.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlElement.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlElement.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlElement.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlElement.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlElement.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlElement.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlElement.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlElement.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlElement.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlElement.(case list)</code> | ✅ 一致 |
| <code>HtmlElement.(default)</code> | ✅ 一致 |
| <code>HtmlElement.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlElement.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlElement.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlElement.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlElement.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlElement.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlElement.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlElement.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlElement.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlElement.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlElement.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlElement.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlElement.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlElement.Disabled=false</code> | ✅ 一致 |
| <code>HtmlElement.Disabled=true</code> | ✅ 一致 |
| <code>HtmlElement.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlElement.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlElement.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlElement.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlElement.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlElement.ID=empty</code> | ✅ 一致 |
| <code>HtmlElement.ID=japanese</code> | ✅ 一致 |
| <code>HtmlElement.ID=markup</code> | ✅ 一致 |
| <code>HtmlElement.ID=text</code> | ✅ 一致 |
| <code>HtmlElement.InnerHtml=empty</code> | ✅ 一致 |
| <code>HtmlElement.InnerHtml=japanese</code> | ✅ 一致 |
| <code>HtmlElement.InnerHtml=markup</code> | ✅ 一致 |
| <code>HtmlElement.InnerHtml=text</code> | ✅ 一致 |
| <code>HtmlElement.InnerText=empty</code> | ✅ 一致 |
| <code>HtmlElement.InnerText=japanese</code> | ✅ 一致 |
| <code>HtmlElement.InnerText=markup</code> | ✅ 一致 |
| <code>HtmlElement.InnerText=text</code> | ✅ 一致 |
| <code>HtmlElement.Manifest=app-relative</code> | ✅ 一致 |
| <code>HtmlElement.Manifest=app-relative+Attributes[lang]=text</code> | ✅ 一致 |
| <code>HtmlElement.Manifest=empty</code> | ✅ 一致 |
| <code>HtmlElement.Manifest=japanese</code> | ✅ 一致 |
| <code>HtmlElement.Manifest=markup</code> | ✅ 一致 |
| <code>HtmlElement.Manifest=text</code> | ✅ 一致 |
| <code>HtmlElement.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlElement.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlElement.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlElement.SkinID=text</code> | ✅ 一致 |
| <code>HtmlElement.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlElement.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlElement.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlElement.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlElement.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlElement.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlElement.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlElement.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlElement.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlElement.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlElement.Visible=false</code> | ✅ 一致 |
| <code>HtmlElement.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlElement.Visible=true</code> | ✅ 一致 |
| <code>HtmlElement[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlElement[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlElement[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlElement[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlElement[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlElement[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlElement [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlElement [viewstate][dynamic] every input posted by a submit input | ✅ 一致 |
| HtmlElement [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
