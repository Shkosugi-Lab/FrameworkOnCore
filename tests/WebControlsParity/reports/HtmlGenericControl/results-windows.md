# HtmlGenericControl パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-11 23:46
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 128 | 128 | 0 | 0 |
| ② HTML | 66 | 66 | 0 | 0 |
| ③ シナリオ | 3 | 3 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 138 件中 138 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 13 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 3 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 58 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 2 | ✅ |
| Visible=false（描画されず状態は保持） | 3 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 3 | 0 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlGenericControl.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.HtmlControls.HtmlContainerControl.#ctor</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.HtmlControls.HtmlContainerControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.HtmlControls.HtmlContainerControl.InnerHtml</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.HtmlControls.HtmlContainerControl.InnerText</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.HtmlControls.HtmlContainerControl.LoadViewState</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.HtmlControls.HtmlContainerControl.Render</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.HtmlControls.HtmlContainerControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.HtmlControls.HtmlContainerControl.RenderEndTag</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.HtmlControls.HtmlGenericControl.#ctor</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.HtmlControls.HtmlGenericControl.TagName</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlGenericControl.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlGenericControl.(case list)</code> | ✅ 一致 |
| <code>HtmlGenericControl.(default)</code> | ✅ 一致 |
| <code>HtmlGenericControl.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlGenericControl.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlGenericControl.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlGenericControl.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlGenericControl.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlGenericControl.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlGenericControl.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlGenericControl.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlGenericControl.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlGenericControl.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlGenericControl.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlGenericControl.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlGenericControl.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlGenericControl.Disabled=false</code> | ✅ 一致 |
| <code>HtmlGenericControl.Disabled=true</code> | ✅ 一致 |
| <code>HtmlGenericControl.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlGenericControl.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlGenericControl.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlGenericControl.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlGenericControl.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlGenericControl.ID=empty</code> | ✅ 一致 |
| <code>HtmlGenericControl.ID=japanese</code> | ✅ 一致 |
| <code>HtmlGenericControl.ID=markup</code> | ✅ 一致 |
| <code>HtmlGenericControl.ID=text</code> | ✅ 一致 |
| <code>HtmlGenericControl.InnerHtml=empty</code> | ✅ 一致 |
| <code>HtmlGenericControl.InnerHtml=japanese</code> | ✅ 一致 |
| <code>HtmlGenericControl.InnerHtml=markup</code> | ✅ 一致 |
| <code>HtmlGenericControl.InnerHtml=text</code> | ✅ 一致 |
| <code>HtmlGenericControl.InnerText=empty</code> | ✅ 一致 |
| <code>HtmlGenericControl.InnerText=japanese</code> | ✅ 一致 |
| <code>HtmlGenericControl.InnerText=markup</code> | ✅ 一致 |
| <code>HtmlGenericControl.InnerText=markup+Disabled=true</code> | ✅ 一致 |
| <code>HtmlGenericControl.InnerText=text</code> | ✅ 一致 |
| <code>HtmlGenericControl.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlGenericControl.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlGenericControl.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlGenericControl.SkinID=text</code> | ✅ 一致 |
| <code>HtmlGenericControl.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlGenericControl.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlGenericControl.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlGenericControl.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlGenericControl.TagName=div</code> | ✅ 一致 |
| <code>HtmlGenericControl.TagName=div+InnerHtml=markup</code> | ✅ 一致 |
| <code>HtmlGenericControl.TagName=empty</code> | ✅ 一致 |
| <code>HtmlGenericControl.TagName=japanese</code> | ✅ 一致 |
| <code>HtmlGenericControl.TagName=markup</code> | ✅ 一致 |
| <code>HtmlGenericControl.TagName=section</code> | ✅ 一致 |
| <code>HtmlGenericControl.TagName=section+Visible=false</code> | ✅ 一致 |
| <code>HtmlGenericControl.TagName=text</code> | ✅ 一致 |
| <code>HtmlGenericControl.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlGenericControl.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlGenericControl.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlGenericControl.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlGenericControl.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlGenericControl.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlGenericControl.Visible=false</code> | ✅ 一致 |
| <code>HtmlGenericControl.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlGenericControl.Visible=true</code> | ✅ 一致 |
| <code>HtmlGenericControl[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlGenericControl[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlGenericControl[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlGenericControl[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlGenericControl[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlGenericControl[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlGenericControl [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlGenericControl [viewstate][dynamic] every input posted by a submit input | ✅ 一致 |
| HtmlGenericControl [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
