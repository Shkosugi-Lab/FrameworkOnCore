# HtmlMeta パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 00:01
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 124 | 124 | 0 | 0 |
| ② HTML | 68 | 68 | 0 | 0 |
| ③ シナリオ | 3 | 3 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 132 件中 132 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 14 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 3 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 60 | 1 | ✅ |
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
| <code>HtmlMeta.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlMeta.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlMeta.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlMeta.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlMeta.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlMeta.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlMeta.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlMeta.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlMeta.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlMeta.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlMeta.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlMeta.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlMeta.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlMeta.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlMeta.Api.HtmlControls.HtmlMeta.#ctor</code> | ✅ 一致 |
| <code>HtmlMeta.Api.HtmlControls.HtmlMeta.Content</code> | ✅ 一致 |
| <code>HtmlMeta.Api.HtmlControls.HtmlMeta.HttpEquiv</code> | ✅ 一致 |
| <code>HtmlMeta.Api.HtmlControls.HtmlMeta.Name</code> | ✅ 一致 |
| <code>HtmlMeta.Api.HtmlControls.HtmlMeta.Render</code> | ✅ 一致 |
| <code>HtmlMeta.Api.HtmlControls.HtmlMeta.Scheme</code> | ✅ 一致 |
| <code>HtmlMeta.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlMeta.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlMeta.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlMeta.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlMeta.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlMeta.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlMeta.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlMeta.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlMeta.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlMeta.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlMeta.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlMeta.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlMeta.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlMeta.(case list)</code> | ✅ 一致 |
| <code>HtmlMeta.(default)</code> | ✅ 一致 |
| <code>HtmlMeta.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlMeta.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlMeta.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlMeta.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlMeta.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlMeta.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlMeta.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlMeta.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlMeta.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlMeta.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlMeta.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlMeta.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlMeta.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlMeta.Content=empty</code> | ✅ 一致 |
| <code>HtmlMeta.Content=japanese</code> | ✅ 一致 |
| <code>HtmlMeta.Content=markup</code> | ✅ 一致 |
| <code>HtmlMeta.Content=text</code> | ✅ 一致 |
| <code>HtmlMeta.Disabled=false</code> | ✅ 一致 |
| <code>HtmlMeta.Disabled=true</code> | ✅ 一致 |
| <code>HtmlMeta.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlMeta.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlMeta.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlMeta.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlMeta.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlMeta.HttpEquiv=empty</code> | ✅ 一致 |
| <code>HtmlMeta.HttpEquiv=japanese</code> | ✅ 一致 |
| <code>HtmlMeta.HttpEquiv=markup</code> | ✅ 一致 |
| <code>HtmlMeta.HttpEquiv=text</code> | ✅ 一致 |
| <code>HtmlMeta.HttpEquiv=text+Content=markup</code> | ✅ 一致 |
| <code>HtmlMeta.ID=empty</code> | ✅ 一致 |
| <code>HtmlMeta.ID=japanese</code> | ✅ 一致 |
| <code>HtmlMeta.ID=markup</code> | ✅ 一致 |
| <code>HtmlMeta.ID=text</code> | ✅ 一致 |
| <code>HtmlMeta.Name=empty</code> | ✅ 一致 |
| <code>HtmlMeta.Name=group</code> | ✅ 一致 |
| <code>HtmlMeta.Name=japanese</code> | ✅ 一致 |
| <code>HtmlMeta.Name=markup</code> | ✅ 一致 |
| <code>HtmlMeta.Name=text</code> | ✅ 一致 |
| <code>HtmlMeta.Scheme=empty</code> | ✅ 一致 |
| <code>HtmlMeta.Scheme=japanese</code> | ✅ 一致 |
| <code>HtmlMeta.Scheme=markup</code> | ✅ 一致 |
| <code>HtmlMeta.Scheme=text</code> | ✅ 一致 |
| <code>HtmlMeta.Scheme=text+Name=group</code> | ✅ 一致 |
| <code>HtmlMeta.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlMeta.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlMeta.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlMeta.SkinID=text</code> | ✅ 一致 |
| <code>HtmlMeta.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlMeta.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlMeta.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlMeta.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlMeta.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlMeta.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlMeta.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlMeta.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlMeta.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlMeta.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlMeta.Visible=false</code> | ✅ 一致 |
| <code>HtmlMeta.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlMeta.Visible=true</code> | ✅ 一致 |
| <code>HtmlMeta[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlMeta[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlMeta[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlMeta[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlMeta[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlMeta[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlMeta [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlMeta [viewstate][dynamic] every input posted by a submit input | ✅ 一致 |
| HtmlMeta [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
