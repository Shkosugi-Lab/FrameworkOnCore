# HtmlIframe パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-11 23:47
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 129 | 129 | 0 | 0 |
| ② HTML | 65 | 65 | 0 | 0 |
| ③ シナリオ | 3 | 3 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 138 件中 138 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 13 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 3 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 57 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 2 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 3 | 0 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlIframe.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlIframe.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlIframe.Api.HtmlControls.HtmlContainerControl.#ctor</code> | ✅ 一致 |
| <code>HtmlIframe.Api.HtmlControls.HtmlContainerControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlIframe.Api.HtmlControls.HtmlContainerControl.InnerHtml</code> | ✅ 一致 |
| <code>HtmlIframe.Api.HtmlControls.HtmlContainerControl.InnerText</code> | ✅ 一致 |
| <code>HtmlIframe.Api.HtmlControls.HtmlContainerControl.LoadViewState</code> | ✅ 一致 |
| <code>HtmlIframe.Api.HtmlControls.HtmlContainerControl.Render</code> | ✅ 一致 |
| <code>HtmlIframe.Api.HtmlControls.HtmlContainerControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlIframe.Api.HtmlControls.HtmlContainerControl.RenderEndTag</code> | ✅ 一致 |
| <code>HtmlIframe.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlIframe.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlIframe.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlIframe.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlIframe.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlIframe.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlIframe.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlIframe.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlIframe.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlIframe.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlIframe.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlIframe.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlIframe.Api.HtmlControls.HtmlIframe.#ctor</code> | ✅ 一致 |
| <code>HtmlIframe.Api.HtmlControls.HtmlIframe.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlIframe.Api.HtmlControls.HtmlIframe.Src</code> | ✅ 一致 |
| <code>HtmlIframe.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlIframe.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlIframe.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlIframe.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlIframe.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlIframe.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlIframe.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlIframe.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlIframe.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlIframe.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlIframe.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlIframe.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlIframe.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlIframe.(case list)</code> | ✅ 一致 |
| <code>HtmlIframe.(default)</code> | ✅ 一致 |
| <code>HtmlIframe.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlIframe.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlIframe.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlIframe.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlIframe.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlIframe.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlIframe.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlIframe.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlIframe.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlIframe.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlIframe.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlIframe.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlIframe.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlIframe.Disabled=false</code> | ✅ 一致 |
| <code>HtmlIframe.Disabled=true</code> | ✅ 一致 |
| <code>HtmlIframe.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlIframe.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlIframe.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlIframe.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlIframe.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlIframe.ID=empty</code> | ✅ 一致 |
| <code>HtmlIframe.ID=japanese</code> | ✅ 一致 |
| <code>HtmlIframe.ID=markup</code> | ✅ 一致 |
| <code>HtmlIframe.ID=text</code> | ✅ 一致 |
| <code>HtmlIframe.InnerHtml=empty</code> | ✅ 一致 |
| <code>HtmlIframe.InnerHtml=japanese</code> | ✅ 一致 |
| <code>HtmlIframe.InnerHtml=markup</code> | ✅ 一致 |
| <code>HtmlIframe.InnerHtml=text</code> | ✅ 一致 |
| <code>HtmlIframe.InnerText=empty</code> | ✅ 一致 |
| <code>HtmlIframe.InnerText=japanese</code> | ✅ 一致 |
| <code>HtmlIframe.InnerText=markup</code> | ✅ 一致 |
| <code>HtmlIframe.InnerText=text</code> | ✅ 一致 |
| <code>HtmlIframe.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlIframe.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlIframe.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlIframe.SkinID=text</code> | ✅ 一致 |
| <code>HtmlIframe.Src=absolute</code> | ✅ 一致 |
| <code>HtmlIframe.Src=absolute+Disabled=true</code> | ✅ 一致 |
| <code>HtmlIframe.Src=app-relative</code> | ✅ 一致 |
| <code>HtmlIframe.Src=app-relative+Attributes[name]=text</code> | ✅ 一致 |
| <code>HtmlIframe.Src=empty</code> | ✅ 一致 |
| <code>HtmlIframe.Src=japanese</code> | ✅ 一致 |
| <code>HtmlIframe.Src=markup</code> | ✅ 一致 |
| <code>HtmlIframe.Src=text</code> | ✅ 一致 |
| <code>HtmlIframe.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlIframe.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlIframe.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlIframe.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlIframe.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlIframe.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlIframe.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlIframe.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlIframe.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlIframe.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlIframe.Visible=false</code> | ✅ 一致 |
| <code>HtmlIframe.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlIframe.Visible=true</code> | ✅ 一致 |
| <code>HtmlIframe[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlIframe[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlIframe[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlIframe[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlIframe[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlIframe[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlIframe [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlIframe [viewstate][dynamic] every input posted by a submit input | ✅ 一致 |
| HtmlIframe [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
