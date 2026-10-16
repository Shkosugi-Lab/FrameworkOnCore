# HtmlTableRow パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 00:05
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 138 | 138 | 0 | 0 |
| ② HTML | 79 | 79 | 0 | 0 |
| ③ シナリオ | 3 | 3 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 147 件中 147 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 17 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 3 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 71 | 1 | ✅ |
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
| <code>HtmlTableRow.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlContainerControl.#ctor</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlContainerControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlContainerControl.InnerHtml</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlContainerControl.InnerText</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlContainerControl.LoadViewState</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlContainerControl.Render</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlContainerControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlContainerControl.RenderEndTag</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlTableRow.#ctor</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlTableRow.Align</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlTableRow.BgColor</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlTableRow.BorderColor</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlTableRow.Cells</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlTableRow.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlTableRow.Height</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlTableRow.InnerHtml</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlTableRow.InnerText</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlTableRow.RenderChildren</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlTableRow.RenderEndTag</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.HtmlControls.HtmlTableRow.VAlign</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlTableRow.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlTableRow.(case list)</code> | ✅ 一致 |
| <code>HtmlTableRow.(default)</code> | ✅ 一致 |
| <code>HtmlTableRow.Align=empty</code> | ✅ 一致 |
| <code>HtmlTableRow.Align=japanese</code> | ✅ 一致 |
| <code>HtmlTableRow.Align=markup</code> | ✅ 一致 |
| <code>HtmlTableRow.Align=text</code> | ✅ 一致 |
| <code>HtmlTableRow.Align=text+VAlign=text</code> | ✅ 一致 |
| <code>HtmlTableRow.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlTableRow.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlTableRow.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlTableRow.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlTableRow.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlTableRow.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlTableRow.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlTableRow.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlTableRow.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlTableRow.BgColor=empty</code> | ✅ 一致 |
| <code>HtmlTableRow.BgColor=japanese</code> | ✅ 一致 |
| <code>HtmlTableRow.BgColor=markup</code> | ✅ 一致 |
| <code>HtmlTableRow.BgColor=text</code> | ✅ 一致 |
| <code>HtmlTableRow.BgColor=text+BorderColor=text+Height=text</code> | ✅ 一致 |
| <code>HtmlTableRow.BorderColor=empty</code> | ✅ 一致 |
| <code>HtmlTableRow.BorderColor=japanese</code> | ✅ 一致 |
| <code>HtmlTableRow.BorderColor=markup</code> | ✅ 一致 |
| <code>HtmlTableRow.BorderColor=text</code> | ✅ 一致 |
| <code>HtmlTableRow.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlTableRow.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlTableRow.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlTableRow.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlTableRow.Disabled=false</code> | ✅ 一致 |
| <code>HtmlTableRow.Disabled=true</code> | ✅ 一致 |
| <code>HtmlTableRow.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlTableRow.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlTableRow.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlTableRow.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlTableRow.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlTableRow.Height=empty</code> | ✅ 一致 |
| <code>HtmlTableRow.Height=japanese</code> | ✅ 一致 |
| <code>HtmlTableRow.Height=markup</code> | ✅ 一致 |
| <code>HtmlTableRow.Height=text</code> | ✅ 一致 |
| <code>HtmlTableRow.ID=empty</code> | ✅ 一致 |
| <code>HtmlTableRow.ID=japanese</code> | ✅ 一致 |
| <code>HtmlTableRow.ID=markup</code> | ✅ 一致 |
| <code>HtmlTableRow.ID=text</code> | ✅ 一致 |
| <code>HtmlTableRow.InnerHtml=empty</code> | ✅ 一致 |
| <code>HtmlTableRow.InnerHtml=japanese</code> | ✅ 一致 |
| <code>HtmlTableRow.InnerHtml=markup</code> | ✅ 一致 |
| <code>HtmlTableRow.InnerHtml=text</code> | ✅ 一致 |
| <code>HtmlTableRow.InnerText=empty</code> | ✅ 一致 |
| <code>HtmlTableRow.InnerText=japanese</code> | ✅ 一致 |
| <code>HtmlTableRow.InnerText=markup</code> | ✅ 一致 |
| <code>HtmlTableRow.InnerText=text</code> | ✅ 一致 |
| <code>HtmlTableRow.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlTableRow.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlTableRow.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlTableRow.SkinID=text</code> | ✅ 一致 |
| <code>HtmlTableRow.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlTableRow.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlTableRow.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlTableRow.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlTableRow.VAlign=empty</code> | ✅ 一致 |
| <code>HtmlTableRow.VAlign=japanese</code> | ✅ 一致 |
| <code>HtmlTableRow.VAlign=markup</code> | ✅ 一致 |
| <code>HtmlTableRow.VAlign=text</code> | ✅ 一致 |
| <code>HtmlTableRow.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlTableRow.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlTableRow.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlTableRow.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlTableRow.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlTableRow.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlTableRow.Visible=false</code> | ✅ 一致 |
| <code>HtmlTableRow.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlTableRow.Visible=true</code> | ✅ 一致 |
| <code>HtmlTableRow[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlTableRow[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlTableRow[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlTableRow[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlTableRow[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlTableRow[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlTableRow [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlTableRow [viewstate][dynamic] every input posted by a submit input | ✅ 一致 |
| HtmlTableRow [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
