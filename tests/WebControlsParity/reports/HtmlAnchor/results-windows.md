# HtmlAnchor パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-11 23:40
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 139 | 139 | 0 | 0 |
| ② HTML | 88 | 88 | 0 | 0 |
| ③ シナリオ | 5 | 5 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 148 件中 148 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 18 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 5 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 80 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 2 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 3 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 3 | 0 | ✅ |
| ポストバックイベント | 0 | 2 | ✅ |
| CausesValidation / ValidationGroup | 7 | 0 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlAnchor.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlAnchor.#ctor</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlAnchor.CausesValidation</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlAnchor.HRef</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlAnchor.Name</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlAnchor.OnPreRender</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlAnchor.OnServerClick</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlAnchor.RaisePostBackEvent</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlAnchor.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlAnchor.ServerClick</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlAnchor.Target</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlAnchor.Title</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlAnchor.ValidationGroup</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlContainerControl.#ctor</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlContainerControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlContainerControl.InnerHtml</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlContainerControl.InnerText</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlContainerControl.LoadViewState</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlContainerControl.Render</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlContainerControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlContainerControl.RenderEndTag</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.IPostBackEventHandler.RaisePostBackEvent</code> | ✅ 一致 |
| <code>HtmlAnchor.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlAnchor.(case list)</code> | ✅ 一致 |
| <code>HtmlAnchor.(default)</code> | ✅ 一致 |
| <code>HtmlAnchor.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlAnchor.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlAnchor.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlAnchor.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlAnchor.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlAnchor.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlAnchor.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlAnchor.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlAnchor.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlAnchor.CausesValidation=false</code> | ✅ 一致 |
| <code>HtmlAnchor.CausesValidation=false+ValidationGroup=text</code> | ✅ 一致 |
| <code>HtmlAnchor.CausesValidation=true</code> | ✅ 一致 |
| <code>HtmlAnchor.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlAnchor.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlAnchor.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlAnchor.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlAnchor.Disabled=false</code> | ✅ 一致 |
| <code>HtmlAnchor.Disabled=true</code> | ✅ 一致 |
| <code>HtmlAnchor.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlAnchor.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlAnchor.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlAnchor.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlAnchor.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlAnchor.HRef=absolute</code> | ✅ 一致 |
| <code>HtmlAnchor.HRef=absolute+Title=text</code> | ✅ 一致 |
| <code>HtmlAnchor.HRef=app-relative</code> | ✅ 一致 |
| <code>HtmlAnchor.HRef=app-relative+Disabled=true</code> | ✅ 一致 |
| <code>HtmlAnchor.HRef=app-relative+Target=blank</code> | ✅ 一致 |
| <code>HtmlAnchor.HRef=empty</code> | ✅ 一致 |
| <code>HtmlAnchor.HRef=japanese</code> | ✅ 一致 |
| <code>HtmlAnchor.HRef=markup</code> | ✅ 一致 |
| <code>HtmlAnchor.HRef=text</code> | ✅ 一致 |
| <code>HtmlAnchor.ID=empty</code> | ✅ 一致 |
| <code>HtmlAnchor.ID=japanese</code> | ✅ 一致 |
| <code>HtmlAnchor.ID=markup</code> | ✅ 一致 |
| <code>HtmlAnchor.ID=text</code> | ✅ 一致 |
| <code>HtmlAnchor.InnerHtml=empty</code> | ✅ 一致 |
| <code>HtmlAnchor.InnerHtml=japanese</code> | ✅ 一致 |
| <code>HtmlAnchor.InnerHtml=markup</code> | ✅ 一致 |
| <code>HtmlAnchor.InnerHtml=markup+HRef=app-relative</code> | ✅ 一致 |
| <code>HtmlAnchor.InnerHtml=text</code> | ✅ 一致 |
| <code>HtmlAnchor.InnerText=empty</code> | ✅ 一致 |
| <code>HtmlAnchor.InnerText=japanese</code> | ✅ 一致 |
| <code>HtmlAnchor.InnerText=markup</code> | ✅ 一致 |
| <code>HtmlAnchor.InnerText=text</code> | ✅ 一致 |
| <code>HtmlAnchor.Name=empty</code> | ✅ 一致 |
| <code>HtmlAnchor.Name=group</code> | ✅ 一致 |
| <code>HtmlAnchor.Name=japanese</code> | ✅ 一致 |
| <code>HtmlAnchor.Name=markup</code> | ✅ 一致 |
| <code>HtmlAnchor.Name=text</code> | ✅ 一致 |
| <code>HtmlAnchor.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlAnchor.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlAnchor.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlAnchor.SkinID=text</code> | ✅ 一致 |
| <code>HtmlAnchor.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlAnchor.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlAnchor.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlAnchor.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlAnchor.Target=blank</code> | ✅ 一致 |
| <code>HtmlAnchor.Target=empty</code> | ✅ 一致 |
| <code>HtmlAnchor.Target=japanese</code> | ✅ 一致 |
| <code>HtmlAnchor.Target=markup</code> | ✅ 一致 |
| <code>HtmlAnchor.Target=text</code> | ✅ 一致 |
| <code>HtmlAnchor.Title=empty</code> | ✅ 一致 |
| <code>HtmlAnchor.Title=japanese</code> | ✅ 一致 |
| <code>HtmlAnchor.Title=markup</code> | ✅ 一致 |
| <code>HtmlAnchor.Title=text</code> | ✅ 一致 |
| <code>HtmlAnchor.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlAnchor.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlAnchor.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlAnchor.ValidationGroup=empty</code> | ✅ 一致 |
| <code>HtmlAnchor.ValidationGroup=japanese</code> | ✅ 一致 |
| <code>HtmlAnchor.ValidationGroup=markup</code> | ✅ 一致 |
| <code>HtmlAnchor.ValidationGroup=text</code> | ✅ 一致 |
| <code>HtmlAnchor.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlAnchor.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlAnchor.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlAnchor.Visible=false</code> | ✅ 一致 |
| <code>HtmlAnchor.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlAnchor.Visible=true</code> | ✅ 一致 |
| <code>HtmlAnchor[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlAnchor[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlAnchor[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlAnchor[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlAnchor[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlAnchor[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlAnchor [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlAnchor [viewstate][dynamic] every input posted by a submit input | ✅ 一致 |
| HtmlAnchor [postbackevent] its server click | ✅ 一致 |
| HtmlAnchor [postbackevent][dynamic] a dynamic one&#39;s server click | ✅ 一致 |
| HtmlAnchor [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
