# HtmlForm パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 02:01
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 144 | 144 | 0 | 0 |
| ② HTML | 92 | 92 | 0 | 0 |
| ③ シナリオ | 5 | 5 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 153 件中 153 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 20 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 5 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 84 | 2 | ✅ |
| ViewState の保存と復元 | 6 | 3 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 4 | ✅ |
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
| <code>HtmlForm.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlForm.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlContainerControl.#ctor</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlContainerControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlContainerControl.InnerHtml</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlContainerControl.InnerText</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlContainerControl.LoadViewState</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlContainerControl.Render</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlContainerControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlContainerControl.RenderEndTag</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlForm.#ctor</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlForm.Action</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlForm.ClientID</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlForm.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlForm.DefaultButton</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlForm.DefaultFocus</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlForm.Enctype</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlForm.Method</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlForm.Name</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlForm.OnInit</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlForm.OnPreRender</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlForm.Render</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlForm.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlForm.RenderChildren</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlForm.RenderControl</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlForm.SubmitDisabledControls</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlForm.Target</code> | ✅ 一致 |
| <code>HtmlForm.Api.HtmlControls.HtmlForm.UniqueID</code> | ✅ 一致 |
| <code>HtmlForm.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlForm.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlForm.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlForm.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlForm.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlForm.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlForm.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlForm.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlForm.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlForm.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlForm.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlForm.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlForm.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlForm.(case list)</code> | ✅ 一致 |
| <code>HtmlForm.(default)</code> | ✅ 一致 |
| <code>HtmlForm.Action=empty</code> | ✅ 一致 |
| <code>HtmlForm.Action=japanese</code> | ✅ 一致 |
| <code>HtmlForm.Action=markup</code> | ✅ 一致 |
| <code>HtmlForm.Action=text</code> | ✅ 一致 |
| <code>HtmlForm.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlForm.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlForm.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlForm.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlForm.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlForm.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlForm.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlForm.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlForm.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlForm.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlForm.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlForm.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlForm.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlForm.DefaultButton=empty</code> | ✅ 一致 |
| <code>HtmlForm.DefaultButton=japanese</code> | ✅ 一致 |
| <code>HtmlForm.DefaultButton=markup</code> | ✅ 一致 |
| <code>HtmlForm.DefaultButton=text</code> | ✅ 一致 |
| <code>HtmlForm.DefaultButton=text+DefaultFocus=text</code> | ✅ 一致 |
| <code>HtmlForm.DefaultFocus=empty</code> | ✅ 一致 |
| <code>HtmlForm.DefaultFocus=japanese</code> | ✅ 一致 |
| <code>HtmlForm.DefaultFocus=markup</code> | ✅ 一致 |
| <code>HtmlForm.DefaultFocus=text</code> | ✅ 一致 |
| <code>HtmlForm.Disabled=false</code> | ✅ 一致 |
| <code>HtmlForm.Disabled=true</code> | ✅ 一致 |
| <code>HtmlForm.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlForm.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlForm.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlForm.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlForm.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlForm.Enctype=empty</code> | ✅ 一致 |
| <code>HtmlForm.Enctype=japanese</code> | ✅ 一致 |
| <code>HtmlForm.Enctype=markup</code> | ✅ 一致 |
| <code>HtmlForm.Enctype=text</code> | ✅ 一致 |
| <code>HtmlForm.ID=empty</code> | ✅ 一致 |
| <code>HtmlForm.ID=japanese</code> | ✅ 一致 |
| <code>HtmlForm.ID=markup</code> | ✅ 一致 |
| <code>HtmlForm.ID=text</code> | ✅ 一致 |
| <code>HtmlForm.InnerHtml=empty</code> | ✅ 一致 |
| <code>HtmlForm.InnerHtml=japanese</code> | ✅ 一致 |
| <code>HtmlForm.InnerHtml=markup</code> | ✅ 一致 |
| <code>HtmlForm.InnerHtml=text</code> | ✅ 一致 |
| <code>HtmlForm.InnerText=empty</code> | ✅ 一致 |
| <code>HtmlForm.InnerText=japanese</code> | ✅ 一致 |
| <code>HtmlForm.InnerText=markup</code> | ✅ 一致 |
| <code>HtmlForm.InnerText=text</code> | ✅ 一致 |
| <code>HtmlForm.Method=empty</code> | ✅ 一致 |
| <code>HtmlForm.Method=japanese</code> | ✅ 一致 |
| <code>HtmlForm.Method=markup</code> | ✅ 一致 |
| <code>HtmlForm.Method=text</code> | ✅ 一致 |
| <code>HtmlForm.Method=text+Enctype=text</code> | ✅ 一致 |
| <code>HtmlForm.Name=empty</code> | ✅ 一致 |
| <code>HtmlForm.Name=group</code> | ✅ 一致 |
| <code>HtmlForm.Name=japanese</code> | ✅ 一致 |
| <code>HtmlForm.Name=markup</code> | ✅ 一致 |
| <code>HtmlForm.Name=text</code> | ✅ 一致 |
| <code>HtmlForm.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlForm.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlForm.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlForm.SkinID=text</code> | ✅ 一致 |
| <code>HtmlForm.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlForm.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlForm.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlForm.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlForm.SubmitDisabledControls=false</code> | ✅ 一致 |
| <code>HtmlForm.SubmitDisabledControls=true</code> | ✅ 一致 |
| <code>HtmlForm.SubmitDisabledControls=true+Disabled=true</code> | ✅ 一致 |
| <code>HtmlForm.Target=blank</code> | ✅ 一致 |
| <code>HtmlForm.Target=empty</code> | ✅ 一致 |
| <code>HtmlForm.Target=japanese</code> | ✅ 一致 |
| <code>HtmlForm.Target=markup</code> | ✅ 一致 |
| <code>HtmlForm.Target=text</code> | ✅ 一致 |
| <code>HtmlForm.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlForm.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlForm.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlForm.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlForm.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlForm.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlForm.Visible=false</code> | ✅ 一致 |
| <code>HtmlForm.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlForm.Visible=true</code> | ✅ 一致 |
| <code>HtmlForm[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlForm[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlForm[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlForm[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlForm[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlForm[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlForm [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlForm [viewstate][dynamic] every input posted by a submit input | ✅ 一致 |
| HtmlForm [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| HtmlForm [dynamic][render] the page&#39;s form, head and title made in code | ✅ 一致 |
| HtmlForm [dynamic][viewstate][postdata] Enter in the form: its default button, its values and view state kept | ✅ 一致 |

</details>
