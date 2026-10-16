# HtmlTableCell パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 00:04
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 137 | 137 | 0 | 0 |
| ② HTML | 92 | 92 | 0 | 0 |
| ③ シナリオ | 3 | 3 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 147 件中 147 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 21 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 3 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 84 | 1 | ✅ |
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
| <code>HtmlTableCell.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlContainerControl.#ctor</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlContainerControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlContainerControl.InnerHtml</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlContainerControl.InnerText</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlContainerControl.LoadViewState</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlContainerControl.Render</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlContainerControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlContainerControl.RenderEndTag</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlTableCell.#ctor</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlTableCell.Align</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlTableCell.BgColor</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlTableCell.BorderColor</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlTableCell.ColSpan</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlTableCell.Height</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlTableCell.NoWrap</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlTableCell.RenderEndTag</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlTableCell.RowSpan</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlTableCell.VAlign</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.HtmlControls.HtmlTableCell.Width</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlTableCell.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlTableCell.(case list)</code> | ✅ 一致 |
| <code>HtmlTableCell.(default)</code> | ✅ 一致 |
| <code>HtmlTableCell.Align=empty</code> | ✅ 一致 |
| <code>HtmlTableCell.Align=japanese</code> | ✅ 一致 |
| <code>HtmlTableCell.Align=markup</code> | ✅ 一致 |
| <code>HtmlTableCell.Align=text</code> | ✅ 一致 |
| <code>HtmlTableCell.Align=text+VAlign=text+BgColor=text</code> | ✅ 一致 |
| <code>HtmlTableCell.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlTableCell.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlTableCell.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlTableCell.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlTableCell.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlTableCell.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlTableCell.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlTableCell.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlTableCell.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlTableCell.BgColor=empty</code> | ✅ 一致 |
| <code>HtmlTableCell.BgColor=japanese</code> | ✅ 一致 |
| <code>HtmlTableCell.BgColor=markup</code> | ✅ 一致 |
| <code>HtmlTableCell.BgColor=text</code> | ✅ 一致 |
| <code>HtmlTableCell.BorderColor=empty</code> | ✅ 一致 |
| <code>HtmlTableCell.BorderColor=japanese</code> | ✅ 一致 |
| <code>HtmlTableCell.BorderColor=markup</code> | ✅ 一致 |
| <code>HtmlTableCell.BorderColor=text</code> | ✅ 一致 |
| <code>HtmlTableCell.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlTableCell.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlTableCell.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlTableCell.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlTableCell.ColSpan=-1</code> | ✅ 一致 |
| <code>HtmlTableCell.ColSpan=0</code> | ✅ 一致 |
| <code>HtmlTableCell.ColSpan=3</code> | ✅ 一致 |
| <code>HtmlTableCell.ColSpan=3+RowSpan=3</code> | ✅ 一致 |
| <code>HtmlTableCell.Disabled=false</code> | ✅ 一致 |
| <code>HtmlTableCell.Disabled=true</code> | ✅ 一致 |
| <code>HtmlTableCell.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlTableCell.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlTableCell.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlTableCell.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlTableCell.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlTableCell.Height=empty</code> | ✅ 一致 |
| <code>HtmlTableCell.Height=japanese</code> | ✅ 一致 |
| <code>HtmlTableCell.Height=markup</code> | ✅ 一致 |
| <code>HtmlTableCell.Height=text</code> | ✅ 一致 |
| <code>HtmlTableCell.ID=empty</code> | ✅ 一致 |
| <code>HtmlTableCell.ID=japanese</code> | ✅ 一致 |
| <code>HtmlTableCell.ID=markup</code> | ✅ 一致 |
| <code>HtmlTableCell.ID=text</code> | ✅ 一致 |
| <code>HtmlTableCell.InnerHtml=empty</code> | ✅ 一致 |
| <code>HtmlTableCell.InnerHtml=japanese</code> | ✅ 一致 |
| <code>HtmlTableCell.InnerHtml=markup</code> | ✅ 一致 |
| <code>HtmlTableCell.InnerHtml=text</code> | ✅ 一致 |
| <code>HtmlTableCell.InnerText=empty</code> | ✅ 一致 |
| <code>HtmlTableCell.InnerText=japanese</code> | ✅ 一致 |
| <code>HtmlTableCell.InnerText=markup</code> | ✅ 一致 |
| <code>HtmlTableCell.InnerText=text</code> | ✅ 一致 |
| <code>HtmlTableCell.NoWrap=false</code> | ✅ 一致 |
| <code>HtmlTableCell.NoWrap=true</code> | ✅ 一致 |
| <code>HtmlTableCell.NoWrap=true+Width=text</code> | ✅ 一致 |
| <code>HtmlTableCell.RowSpan=-1</code> | ✅ 一致 |
| <code>HtmlTableCell.RowSpan=0</code> | ✅ 一致 |
| <code>HtmlTableCell.RowSpan=3</code> | ✅ 一致 |
| <code>HtmlTableCell.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlTableCell.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlTableCell.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlTableCell.SkinID=text</code> | ✅ 一致 |
| <code>HtmlTableCell.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlTableCell.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlTableCell.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlTableCell.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlTableCell.VAlign=empty</code> | ✅ 一致 |
| <code>HtmlTableCell.VAlign=japanese</code> | ✅ 一致 |
| <code>HtmlTableCell.VAlign=markup</code> | ✅ 一致 |
| <code>HtmlTableCell.VAlign=text</code> | ✅ 一致 |
| <code>HtmlTableCell.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlTableCell.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlTableCell.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlTableCell.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlTableCell.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlTableCell.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlTableCell.Visible=false</code> | ✅ 一致 |
| <code>HtmlTableCell.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlTableCell.Visible=true</code> | ✅ 一致 |
| <code>HtmlTableCell.Width=empty</code> | ✅ 一致 |
| <code>HtmlTableCell.Width=japanese</code> | ✅ 一致 |
| <code>HtmlTableCell.Width=markup</code> | ✅ 一致 |
| <code>HtmlTableCell.Width=text</code> | ✅ 一致 |
| <code>HtmlTableCell[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlTableCell[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlTableCell[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlTableCell[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlTableCell[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlTableCell[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlTableCell [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlTableCell [viewstate][dynamic] every input posted by a submit input | ✅ 一致 |
| HtmlTableCell [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
