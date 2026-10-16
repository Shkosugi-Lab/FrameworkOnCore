# HtmlLink パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 02:16
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 122 | 122 | 0 | 0 |
| ② HTML | 56 | 56 | 0 | 0 |
| ③ シナリオ | 3 | 3 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 130 件中 130 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 11 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 3 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 48 | 1 | ✅ |
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
| <code>HtmlLink.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlLink.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlLink.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlLink.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlLink.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlLink.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlLink.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlLink.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlLink.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlLink.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlLink.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlLink.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlLink.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlLink.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlLink.Api.HtmlControls.HtmlLink.#ctor</code> | ✅ 一致 |
| <code>HtmlLink.Api.HtmlControls.HtmlLink.Href</code> | ✅ 一致 |
| <code>HtmlLink.Api.HtmlControls.HtmlLink.Render</code> | ✅ 一致 |
| <code>HtmlLink.Api.HtmlControls.HtmlLink.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlLink.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlLink.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlLink.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlLink.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlLink.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlLink.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlLink.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlLink.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlLink.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlLink.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlLink.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlLink.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlLink.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlLink.(case list)</code> | ✅ 一致 |
| <code>HtmlLink.(default)</code> | ✅ 一致 |
| <code>HtmlLink.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlLink.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlLink.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlLink.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlLink.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlLink.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlLink.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlLink.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlLink.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlLink.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlLink.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlLink.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlLink.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlLink.Disabled=false</code> | ✅ 一致 |
| <code>HtmlLink.Disabled=true</code> | ✅ 一致 |
| <code>HtmlLink.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlLink.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlLink.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlLink.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlLink.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlLink.Href=absolute</code> | ✅ 一致 |
| <code>HtmlLink.Href=absolute+Attributes[rel]=text</code> | ✅ 一致 |
| <code>HtmlLink.Href=app-relative</code> | ✅ 一致 |
| <code>HtmlLink.Href=empty</code> | ✅ 一致 |
| <code>HtmlLink.Href=japanese</code> | ✅ 一致 |
| <code>HtmlLink.Href=markup</code> | ✅ 一致 |
| <code>HtmlLink.Href=text</code> | ✅ 一致 |
| <code>HtmlLink.ID=empty</code> | ✅ 一致 |
| <code>HtmlLink.ID=japanese</code> | ✅ 一致 |
| <code>HtmlLink.ID=markup</code> | ✅ 一致 |
| <code>HtmlLink.ID=text</code> | ✅ 一致 |
| <code>HtmlLink.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlLink.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlLink.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlLink.SkinID=text</code> | ✅ 一致 |
| <code>HtmlLink.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlLink.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlLink.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlLink.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlLink.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlLink.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlLink.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlLink.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlLink.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlLink.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlLink.Visible=false</code> | ✅ 一致 |
| <code>HtmlLink.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlLink.Visible=true</code> | ✅ 一致 |
| <code>HtmlLink[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlLink[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlLink[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlLink[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlLink[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlLink[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlLink [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlLink [viewstate][dynamic] every input posted by a submit input | ✅ 一致 |
| HtmlLink [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
