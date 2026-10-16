# HtmlEmbed パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-11 23:44
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 129 | 129 | 0 | 0 |
| ② HTML | 64 | 64 | 0 | 0 |
| ③ シナリオ | 3 | 3 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 138 件中 138 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 13 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 3 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 56 | 1 | ✅ |
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
| <code>HtmlEmbed.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.HtmlControls.HtmlContainerControl.#ctor</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.HtmlControls.HtmlContainerControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.HtmlControls.HtmlContainerControl.InnerHtml</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.HtmlControls.HtmlContainerControl.InnerText</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.HtmlControls.HtmlContainerControl.LoadViewState</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.HtmlControls.HtmlContainerControl.Render</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.HtmlControls.HtmlContainerControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.HtmlControls.HtmlContainerControl.RenderEndTag</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.HtmlControls.HtmlEmbed.#ctor</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.HtmlControls.HtmlEmbed.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.HtmlControls.HtmlEmbed.Src</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlEmbed.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlEmbed.(case list)</code> | ✅ 一致 |
| <code>HtmlEmbed.(default)</code> | ✅ 一致 |
| <code>HtmlEmbed.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlEmbed.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlEmbed.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlEmbed.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlEmbed.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlEmbed.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlEmbed.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlEmbed.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlEmbed.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlEmbed.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlEmbed.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlEmbed.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlEmbed.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlEmbed.Disabled=false</code> | ✅ 一致 |
| <code>HtmlEmbed.Disabled=true</code> | ✅ 一致 |
| <code>HtmlEmbed.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlEmbed.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlEmbed.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlEmbed.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlEmbed.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlEmbed.ID=empty</code> | ✅ 一致 |
| <code>HtmlEmbed.ID=japanese</code> | ✅ 一致 |
| <code>HtmlEmbed.ID=markup</code> | ✅ 一致 |
| <code>HtmlEmbed.ID=text</code> | ✅ 一致 |
| <code>HtmlEmbed.InnerHtml=empty</code> | ✅ 一致 |
| <code>HtmlEmbed.InnerHtml=japanese</code> | ✅ 一致 |
| <code>HtmlEmbed.InnerHtml=markup</code> | ✅ 一致 |
| <code>HtmlEmbed.InnerHtml=text</code> | ✅ 一致 |
| <code>HtmlEmbed.InnerText=empty</code> | ✅ 一致 |
| <code>HtmlEmbed.InnerText=japanese</code> | ✅ 一致 |
| <code>HtmlEmbed.InnerText=markup</code> | ✅ 一致 |
| <code>HtmlEmbed.InnerText=text</code> | ✅ 一致 |
| <code>HtmlEmbed.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlEmbed.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlEmbed.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlEmbed.SkinID=text</code> | ✅ 一致 |
| <code>HtmlEmbed.Src=absolute</code> | ✅ 一致 |
| <code>HtmlEmbed.Src=app-relative</code> | ✅ 一致 |
| <code>HtmlEmbed.Src=app-relative+Attributes[type]=text</code> | ✅ 一致 |
| <code>HtmlEmbed.Src=empty</code> | ✅ 一致 |
| <code>HtmlEmbed.Src=japanese</code> | ✅ 一致 |
| <code>HtmlEmbed.Src=markup</code> | ✅ 一致 |
| <code>HtmlEmbed.Src=text</code> | ✅ 一致 |
| <code>HtmlEmbed.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlEmbed.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlEmbed.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlEmbed.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlEmbed.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlEmbed.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlEmbed.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlEmbed.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlEmbed.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlEmbed.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlEmbed.Visible=false</code> | ✅ 一致 |
| <code>HtmlEmbed.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlEmbed.Visible=true</code> | ✅ 一致 |
| <code>HtmlEmbed[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlEmbed[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlEmbed[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlEmbed[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlEmbed[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlEmbed[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlEmbed [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlEmbed [viewstate][dynamic] every input posted by a submit input | ✅ 一致 |
| HtmlEmbed [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
