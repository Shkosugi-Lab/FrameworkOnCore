# HtmlVideo パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 00:09
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 130 | 130 | 0 | 0 |
| ② HTML | 70 | 70 | 0 | 0 |
| ③ シナリオ | 3 | 3 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 139 件中 139 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 14 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 3 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 62 | 1 | ✅ |
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
| <code>HtmlVideo.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlVideo.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlVideo.Api.HtmlControls.HtmlContainerControl.#ctor</code> | ✅ 一致 |
| <code>HtmlVideo.Api.HtmlControls.HtmlContainerControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlVideo.Api.HtmlControls.HtmlContainerControl.InnerHtml</code> | ✅ 一致 |
| <code>HtmlVideo.Api.HtmlControls.HtmlContainerControl.InnerText</code> | ✅ 一致 |
| <code>HtmlVideo.Api.HtmlControls.HtmlContainerControl.LoadViewState</code> | ✅ 一致 |
| <code>HtmlVideo.Api.HtmlControls.HtmlContainerControl.Render</code> | ✅ 一致 |
| <code>HtmlVideo.Api.HtmlControls.HtmlContainerControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlVideo.Api.HtmlControls.HtmlContainerControl.RenderEndTag</code> | ✅ 一致 |
| <code>HtmlVideo.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlVideo.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlVideo.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlVideo.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlVideo.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlVideo.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlVideo.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlVideo.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlVideo.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlVideo.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlVideo.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlVideo.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlVideo.Api.HtmlControls.HtmlVideo.#ctor</code> | ✅ 一致 |
| <code>HtmlVideo.Api.HtmlControls.HtmlVideo.Poster</code> | ✅ 一致 |
| <code>HtmlVideo.Api.HtmlControls.HtmlVideo.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlVideo.Api.HtmlControls.HtmlVideo.Src</code> | ✅ 一致 |
| <code>HtmlVideo.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlVideo.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlVideo.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlVideo.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlVideo.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlVideo.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlVideo.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlVideo.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlVideo.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlVideo.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlVideo.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlVideo.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlVideo.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlVideo.(case list)</code> | ✅ 一致 |
| <code>HtmlVideo.(default)</code> | ✅ 一致 |
| <code>HtmlVideo.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlVideo.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlVideo.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlVideo.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlVideo.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlVideo.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlVideo.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlVideo.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlVideo.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlVideo.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlVideo.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlVideo.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlVideo.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlVideo.Disabled=false</code> | ✅ 一致 |
| <code>HtmlVideo.Disabled=true</code> | ✅ 一致 |
| <code>HtmlVideo.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlVideo.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlVideo.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlVideo.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlVideo.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlVideo.ID=empty</code> | ✅ 一致 |
| <code>HtmlVideo.ID=japanese</code> | ✅ 一致 |
| <code>HtmlVideo.ID=markup</code> | ✅ 一致 |
| <code>HtmlVideo.ID=text</code> | ✅ 一致 |
| <code>HtmlVideo.InnerHtml=empty</code> | ✅ 一致 |
| <code>HtmlVideo.InnerHtml=japanese</code> | ✅ 一致 |
| <code>HtmlVideo.InnerHtml=markup</code> | ✅ 一致 |
| <code>HtmlVideo.InnerHtml=text</code> | ✅ 一致 |
| <code>HtmlVideo.InnerText=empty</code> | ✅ 一致 |
| <code>HtmlVideo.InnerText=japanese</code> | ✅ 一致 |
| <code>HtmlVideo.InnerText=markup</code> | ✅ 一致 |
| <code>HtmlVideo.InnerText=text</code> | ✅ 一致 |
| <code>HtmlVideo.Poster=app-relative</code> | ✅ 一致 |
| <code>HtmlVideo.Poster=empty</code> | ✅ 一致 |
| <code>HtmlVideo.Poster=japanese</code> | ✅ 一致 |
| <code>HtmlVideo.Poster=markup</code> | ✅ 一致 |
| <code>HtmlVideo.Poster=text</code> | ✅ 一致 |
| <code>HtmlVideo.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlVideo.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlVideo.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlVideo.SkinID=text</code> | ✅ 一致 |
| <code>HtmlVideo.Src=absolute</code> | ✅ 一致 |
| <code>HtmlVideo.Src=absolute+Attributes[controls]=text</code> | ✅ 一致 |
| <code>HtmlVideo.Src=app-relative</code> | ✅ 一致 |
| <code>HtmlVideo.Src=app-relative+Poster=app-relative</code> | ✅ 一致 |
| <code>HtmlVideo.Src=empty</code> | ✅ 一致 |
| <code>HtmlVideo.Src=japanese</code> | ✅ 一致 |
| <code>HtmlVideo.Src=markup</code> | ✅ 一致 |
| <code>HtmlVideo.Src=text</code> | ✅ 一致 |
| <code>HtmlVideo.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlVideo.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlVideo.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlVideo.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlVideo.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlVideo.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlVideo.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlVideo.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlVideo.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlVideo.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlVideo.Visible=false</code> | ✅ 一致 |
| <code>HtmlVideo.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlVideo.Visible=true</code> | ✅ 一致 |
| <code>HtmlVideo[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlVideo[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlVideo[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlVideo[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlVideo[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlVideo[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlVideo [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlVideo [viewstate][dynamic] every input posted by a submit input | ✅ 一致 |
| HtmlVideo [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
