# HtmlTrack パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 00:08
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
| <code>HtmlTrack.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlTrack.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlTrack.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlTrack.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlTrack.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlTrack.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlTrack.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlTrack.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlTrack.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlTrack.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlTrack.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlTrack.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlTrack.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlTrack.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlTrack.Api.HtmlControls.HtmlTrack.#ctor</code> | ✅ 一致 |
| <code>HtmlTrack.Api.HtmlControls.HtmlTrack.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlTrack.Api.HtmlControls.HtmlTrack.Src</code> | ✅ 一致 |
| <code>HtmlTrack.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlTrack.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlTrack.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlTrack.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlTrack.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlTrack.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlTrack.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlTrack.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlTrack.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlTrack.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlTrack.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlTrack.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlTrack.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlTrack.(case list)</code> | ✅ 一致 |
| <code>HtmlTrack.(default)</code> | ✅ 一致 |
| <code>HtmlTrack.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlTrack.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlTrack.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlTrack.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlTrack.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlTrack.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlTrack.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlTrack.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlTrack.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlTrack.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlTrack.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlTrack.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlTrack.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlTrack.Disabled=false</code> | ✅ 一致 |
| <code>HtmlTrack.Disabled=true</code> | ✅ 一致 |
| <code>HtmlTrack.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlTrack.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlTrack.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlTrack.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlTrack.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlTrack.ID=empty</code> | ✅ 一致 |
| <code>HtmlTrack.ID=japanese</code> | ✅ 一致 |
| <code>HtmlTrack.ID=markup</code> | ✅ 一致 |
| <code>HtmlTrack.ID=text</code> | ✅ 一致 |
| <code>HtmlTrack.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlTrack.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlTrack.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlTrack.SkinID=text</code> | ✅ 一致 |
| <code>HtmlTrack.Src=absolute</code> | ✅ 一致 |
| <code>HtmlTrack.Src=app-relative</code> | ✅ 一致 |
| <code>HtmlTrack.Src=app-relative+Attributes[kind]=text</code> | ✅ 一致 |
| <code>HtmlTrack.Src=empty</code> | ✅ 一致 |
| <code>HtmlTrack.Src=japanese</code> | ✅ 一致 |
| <code>HtmlTrack.Src=markup</code> | ✅ 一致 |
| <code>HtmlTrack.Src=text</code> | ✅ 一致 |
| <code>HtmlTrack.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlTrack.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlTrack.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlTrack.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlTrack.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlTrack.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlTrack.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlTrack.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlTrack.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlTrack.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlTrack.Visible=false</code> | ✅ 一致 |
| <code>HtmlTrack.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlTrack.Visible=true</code> | ✅ 一致 |
| <code>HtmlTrack[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlTrack[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlTrack[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlTrack[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlTrack[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlTrack[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlTrack [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlTrack [viewstate][dynamic] every input posted by a submit input | ✅ 一致 |
| HtmlTrack [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
