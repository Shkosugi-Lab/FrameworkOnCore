# HtmlArea パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-11 23:41
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 121 | 121 | 0 | 0 |
| ② HTML | 57 | 57 | 0 | 0 |
| ③ シナリオ | 3 | 3 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 129 件中 129 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 11 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 3 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 49 | 1 | ✅ |
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
| <code>HtmlArea.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlArea.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlArea.Api.HtmlControls.HtmlArea.#ctor</code> | ✅ 一致 |
| <code>HtmlArea.Api.HtmlControls.HtmlArea.Href</code> | ✅ 一致 |
| <code>HtmlArea.Api.HtmlControls.HtmlArea.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlArea.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlArea.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlArea.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlArea.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlArea.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlArea.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlArea.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlArea.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlArea.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlArea.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlArea.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlArea.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlArea.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlArea.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlArea.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlArea.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlArea.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlArea.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlArea.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlArea.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlArea.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlArea.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlArea.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlArea.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlArea.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlArea.(case list)</code> | ✅ 一致 |
| <code>HtmlArea.(default)</code> | ✅ 一致 |
| <code>HtmlArea.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlArea.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlArea.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlArea.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlArea.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlArea.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlArea.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlArea.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlArea.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlArea.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlArea.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlArea.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlArea.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlArea.Disabled=false</code> | ✅ 一致 |
| <code>HtmlArea.Disabled=true</code> | ✅ 一致 |
| <code>HtmlArea.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlArea.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlArea.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlArea.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlArea.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlArea.Href=absolute</code> | ✅ 一致 |
| <code>HtmlArea.Href=absolute+Attributes[shape]=text</code> | ✅ 一致 |
| <code>HtmlArea.Href=app-relative</code> | ✅ 一致 |
| <code>HtmlArea.Href=app-relative+Attributes[alt]=markup</code> | ✅ 一致 |
| <code>HtmlArea.Href=empty</code> | ✅ 一致 |
| <code>HtmlArea.Href=japanese</code> | ✅ 一致 |
| <code>HtmlArea.Href=markup</code> | ✅ 一致 |
| <code>HtmlArea.Href=text</code> | ✅ 一致 |
| <code>HtmlArea.ID=empty</code> | ✅ 一致 |
| <code>HtmlArea.ID=japanese</code> | ✅ 一致 |
| <code>HtmlArea.ID=markup</code> | ✅ 一致 |
| <code>HtmlArea.ID=text</code> | ✅ 一致 |
| <code>HtmlArea.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlArea.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlArea.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlArea.SkinID=text</code> | ✅ 一致 |
| <code>HtmlArea.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlArea.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlArea.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlArea.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlArea.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlArea.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlArea.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlArea.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlArea.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlArea.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlArea.Visible=false</code> | ✅ 一致 |
| <code>HtmlArea.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlArea.Visible=true</code> | ✅ 一致 |
| <code>HtmlArea[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlArea[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlArea[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlArea[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlArea[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlArea[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlArea [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlArea [viewstate][dynamic] every input posted by a submit input | ✅ 一致 |
| HtmlArea [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
