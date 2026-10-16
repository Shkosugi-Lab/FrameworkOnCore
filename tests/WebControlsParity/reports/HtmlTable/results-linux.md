# HtmlTable パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 02:20
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 141 | 141 | 0 | 0 |
| ② HTML | 89 | 89 | 0 | 0 |
| ③ シナリオ | 3 | 3 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 150 件中 150 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 20 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 3 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 81 | 1 | ✅ |
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
| <code>HtmlTable.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlTable.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlContainerControl.#ctor</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlContainerControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlContainerControl.InnerHtml</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlContainerControl.InnerText</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlContainerControl.LoadViewState</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlContainerControl.Render</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlContainerControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlContainerControl.RenderEndTag</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlTable.#ctor</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlTable.Align</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlTable.BgColor</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlTable.Border</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlTable.BorderColor</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlTable.CellPadding</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlTable.CellSpacing</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlTable.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlTable.Height</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlTable.InnerHtml</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlTable.InnerText</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlTable.RenderChildren</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlTable.RenderEndTag</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlTable.Rows</code> | ✅ 一致 |
| <code>HtmlTable.Api.HtmlControls.HtmlTable.Width</code> | ✅ 一致 |
| <code>HtmlTable.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlTable.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlTable.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlTable.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlTable.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlTable.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlTable.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlTable.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlTable.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlTable.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlTable.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlTable.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlTable.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlTable.(case list)</code> | ✅ 一致 |
| <code>HtmlTable.(default)</code> | ✅ 一致 |
| <code>HtmlTable.Align=empty</code> | ✅ 一致 |
| <code>HtmlTable.Align=japanese</code> | ✅ 一致 |
| <code>HtmlTable.Align=markup</code> | ✅ 一致 |
| <code>HtmlTable.Align=text</code> | ✅ 一致 |
| <code>HtmlTable.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlTable.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlTable.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlTable.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlTable.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlTable.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlTable.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlTable.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlTable.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlTable.BgColor=empty</code> | ✅ 一致 |
| <code>HtmlTable.BgColor=japanese</code> | ✅ 一致 |
| <code>HtmlTable.BgColor=markup</code> | ✅ 一致 |
| <code>HtmlTable.BgColor=text</code> | ✅ 一致 |
| <code>HtmlTable.BgColor=text+BorderColor=text</code> | ✅ 一致 |
| <code>HtmlTable.Border=-1</code> | ✅ 一致 |
| <code>HtmlTable.Border=0</code> | ✅ 一致 |
| <code>HtmlTable.Border=3</code> | ✅ 一致 |
| <code>HtmlTable.Border=3+CellPadding=3+CellSpacing=0</code> | ✅ 一致 |
| <code>HtmlTable.BorderColor=empty</code> | ✅ 一致 |
| <code>HtmlTable.BorderColor=japanese</code> | ✅ 一致 |
| <code>HtmlTable.BorderColor=markup</code> | ✅ 一致 |
| <code>HtmlTable.BorderColor=text</code> | ✅ 一致 |
| <code>HtmlTable.CellPadding=-1</code> | ✅ 一致 |
| <code>HtmlTable.CellPadding=0</code> | ✅ 一致 |
| <code>HtmlTable.CellPadding=3</code> | ✅ 一致 |
| <code>HtmlTable.CellSpacing=-1</code> | ✅ 一致 |
| <code>HtmlTable.CellSpacing=0</code> | ✅ 一致 |
| <code>HtmlTable.CellSpacing=3</code> | ✅ 一致 |
| <code>HtmlTable.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlTable.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlTable.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlTable.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlTable.Disabled=false</code> | ✅ 一致 |
| <code>HtmlTable.Disabled=true</code> | ✅ 一致 |
| <code>HtmlTable.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlTable.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlTable.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlTable.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlTable.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlTable.Height=empty</code> | ✅ 一致 |
| <code>HtmlTable.Height=japanese</code> | ✅ 一致 |
| <code>HtmlTable.Height=markup</code> | ✅ 一致 |
| <code>HtmlTable.Height=text</code> | ✅ 一致 |
| <code>HtmlTable.ID=empty</code> | ✅ 一致 |
| <code>HtmlTable.ID=japanese</code> | ✅ 一致 |
| <code>HtmlTable.ID=markup</code> | ✅ 一致 |
| <code>HtmlTable.ID=text</code> | ✅ 一致 |
| <code>HtmlTable.InnerHtml=empty</code> | ✅ 一致 |
| <code>HtmlTable.InnerHtml=japanese</code> | ✅ 一致 |
| <code>HtmlTable.InnerHtml=markup</code> | ✅ 一致 |
| <code>HtmlTable.InnerHtml=text</code> | ✅ 一致 |
| <code>HtmlTable.InnerText=empty</code> | ✅ 一致 |
| <code>HtmlTable.InnerText=japanese</code> | ✅ 一致 |
| <code>HtmlTable.InnerText=markup</code> | ✅ 一致 |
| <code>HtmlTable.InnerText=text</code> | ✅ 一致 |
| <code>HtmlTable.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlTable.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlTable.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlTable.SkinID=text</code> | ✅ 一致 |
| <code>HtmlTable.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlTable.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlTable.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlTable.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlTable.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlTable.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlTable.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlTable.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlTable.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlTable.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlTable.Visible=false</code> | ✅ 一致 |
| <code>HtmlTable.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlTable.Visible=true</code> | ✅ 一致 |
| <code>HtmlTable.Width=empty</code> | ✅ 一致 |
| <code>HtmlTable.Width=japanese</code> | ✅ 一致 |
| <code>HtmlTable.Width=markup</code> | ✅ 一致 |
| <code>HtmlTable.Width=text</code> | ✅ 一致 |
| <code>HtmlTable.Width=text+Height=text+Align=text</code> | ✅ 一致 |
| <code>HtmlTable[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlTable[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlTable[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlTable[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlTable[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlTable[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlTable [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlTable [viewstate][dynamic] every input posted by a submit input | ✅ 一致 |
| HtmlTable [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
