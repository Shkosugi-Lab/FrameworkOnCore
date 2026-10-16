# HtmlAudio パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-11 23:41
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
| <code>HtmlAudio.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlAudio.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlAudio.Api.HtmlControls.HtmlAudio.#ctor</code> | ✅ 一致 |
| <code>HtmlAudio.Api.HtmlControls.HtmlAudio.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlAudio.Api.HtmlControls.HtmlAudio.Src</code> | ✅ 一致 |
| <code>HtmlAudio.Api.HtmlControls.HtmlContainerControl.#ctor</code> | ✅ 一致 |
| <code>HtmlAudio.Api.HtmlControls.HtmlContainerControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlAudio.Api.HtmlControls.HtmlContainerControl.InnerHtml</code> | ✅ 一致 |
| <code>HtmlAudio.Api.HtmlControls.HtmlContainerControl.InnerText</code> | ✅ 一致 |
| <code>HtmlAudio.Api.HtmlControls.HtmlContainerControl.LoadViewState</code> | ✅ 一致 |
| <code>HtmlAudio.Api.HtmlControls.HtmlContainerControl.Render</code> | ✅ 一致 |
| <code>HtmlAudio.Api.HtmlControls.HtmlContainerControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlAudio.Api.HtmlControls.HtmlContainerControl.RenderEndTag</code> | ✅ 一致 |
| <code>HtmlAudio.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlAudio.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlAudio.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlAudio.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlAudio.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlAudio.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlAudio.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlAudio.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlAudio.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlAudio.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlAudio.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlAudio.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlAudio.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlAudio.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlAudio.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlAudio.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlAudio.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlAudio.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlAudio.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlAudio.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlAudio.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlAudio.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlAudio.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlAudio.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlAudio.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlAudio.(case list)</code> | ✅ 一致 |
| <code>HtmlAudio.(default)</code> | ✅ 一致 |
| <code>HtmlAudio.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlAudio.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlAudio.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlAudio.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlAudio.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlAudio.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlAudio.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlAudio.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlAudio.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlAudio.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlAudio.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlAudio.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlAudio.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlAudio.Disabled=false</code> | ✅ 一致 |
| <code>HtmlAudio.Disabled=true</code> | ✅ 一致 |
| <code>HtmlAudio.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlAudio.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlAudio.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlAudio.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlAudio.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlAudio.ID=empty</code> | ✅ 一致 |
| <code>HtmlAudio.ID=japanese</code> | ✅ 一致 |
| <code>HtmlAudio.ID=markup</code> | ✅ 一致 |
| <code>HtmlAudio.ID=text</code> | ✅ 一致 |
| <code>HtmlAudio.InnerHtml=empty</code> | ✅ 一致 |
| <code>HtmlAudio.InnerHtml=japanese</code> | ✅ 一致 |
| <code>HtmlAudio.InnerHtml=markup</code> | ✅ 一致 |
| <code>HtmlAudio.InnerHtml=text</code> | ✅ 一致 |
| <code>HtmlAudio.InnerText=empty</code> | ✅ 一致 |
| <code>HtmlAudio.InnerText=japanese</code> | ✅ 一致 |
| <code>HtmlAudio.InnerText=markup</code> | ✅ 一致 |
| <code>HtmlAudio.InnerText=text</code> | ✅ 一致 |
| <code>HtmlAudio.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlAudio.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlAudio.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlAudio.SkinID=text</code> | ✅ 一致 |
| <code>HtmlAudio.Src=absolute</code> | ✅ 一致 |
| <code>HtmlAudio.Src=app-relative</code> | ✅ 一致 |
| <code>HtmlAudio.Src=app-relative+Attributes[controls]=text</code> | ✅ 一致 |
| <code>HtmlAudio.Src=empty</code> | ✅ 一致 |
| <code>HtmlAudio.Src=japanese</code> | ✅ 一致 |
| <code>HtmlAudio.Src=markup</code> | ✅ 一致 |
| <code>HtmlAudio.Src=text</code> | ✅ 一致 |
| <code>HtmlAudio.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlAudio.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlAudio.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlAudio.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlAudio.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlAudio.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlAudio.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlAudio.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlAudio.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlAudio.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlAudio.Visible=false</code> | ✅ 一致 |
| <code>HtmlAudio.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlAudio.Visible=true</code> | ✅ 一致 |
| <code>HtmlAudio[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlAudio[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlAudio[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlAudio[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlAudio[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlAudio[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlAudio [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlAudio [viewstate][dynamic] every input posted by a submit input | ✅ 一致 |
| HtmlAudio [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
