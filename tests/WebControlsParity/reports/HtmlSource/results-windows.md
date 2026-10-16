# HtmlSource パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 00:03
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 121 | 121 | 0 | 0 |
| ② HTML | 56 | 56 | 0 | 0 |
| ③ シナリオ | 3 | 3 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 129 件中 129 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
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
| <code>HtmlSource.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlSource.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlSource.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlSource.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlSource.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlSource.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlSource.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlSource.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlSource.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlSource.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlSource.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlSource.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlSource.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlSource.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlSource.Api.HtmlControls.HtmlSource.#ctor</code> | ✅ 一致 |
| <code>HtmlSource.Api.HtmlControls.HtmlSource.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlSource.Api.HtmlControls.HtmlSource.Src</code> | ✅ 一致 |
| <code>HtmlSource.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlSource.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlSource.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlSource.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlSource.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlSource.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlSource.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlSource.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlSource.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlSource.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlSource.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlSource.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlSource.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlSource.(case list)</code> | ✅ 一致 |
| <code>HtmlSource.(default)</code> | ✅ 一致 |
| <code>HtmlSource.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlSource.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlSource.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlSource.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlSource.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlSource.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlSource.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlSource.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlSource.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlSource.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlSource.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlSource.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlSource.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlSource.Disabled=false</code> | ✅ 一致 |
| <code>HtmlSource.Disabled=true</code> | ✅ 一致 |
| <code>HtmlSource.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlSource.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlSource.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlSource.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlSource.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlSource.ID=empty</code> | ✅ 一致 |
| <code>HtmlSource.ID=japanese</code> | ✅ 一致 |
| <code>HtmlSource.ID=markup</code> | ✅ 一致 |
| <code>HtmlSource.ID=text</code> | ✅ 一致 |
| <code>HtmlSource.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlSource.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlSource.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlSource.SkinID=text</code> | ✅ 一致 |
| <code>HtmlSource.Src=absolute</code> | ✅ 一致 |
| <code>HtmlSource.Src=app-relative</code> | ✅ 一致 |
| <code>HtmlSource.Src=app-relative+Attributes[type]=text</code> | ✅ 一致 |
| <code>HtmlSource.Src=empty</code> | ✅ 一致 |
| <code>HtmlSource.Src=japanese</code> | ✅ 一致 |
| <code>HtmlSource.Src=markup</code> | ✅ 一致 |
| <code>HtmlSource.Src=text</code> | ✅ 一致 |
| <code>HtmlSource.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlSource.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlSource.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlSource.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlSource.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlSource.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlSource.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlSource.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlSource.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlSource.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlSource.Visible=false</code> | ✅ 一致 |
| <code>HtmlSource.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlSource.Visible=true</code> | ✅ 一致 |
| <code>HtmlSource[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlSource[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlSource[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlSource[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlSource[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlSource[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlSource [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlSource [viewstate][dynamic] every input posted by a submit input | ✅ 一致 |
| HtmlSource [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
