# HtmlHead パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-11 23:47
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 133 | 133 | 0 | 0 |
| ② HTML | 77 | 77 | 0 | 0 |
| ③ シナリオ | 5 | 5 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 144 件中 144 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 16 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 5 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 69 | 2 | ✅ |
| ViewState の保存と復元 | 6 | 3 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 4 | ✅ |
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
| <code>HtmlHead.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlHead.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlHead.Api.HtmlControls.HtmlContainerControl.#ctor</code> | ✅ 一致 |
| <code>HtmlHead.Api.HtmlControls.HtmlContainerControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlHead.Api.HtmlControls.HtmlContainerControl.InnerHtml</code> | ✅ 一致 |
| <code>HtmlHead.Api.HtmlControls.HtmlContainerControl.InnerText</code> | ✅ 一致 |
| <code>HtmlHead.Api.HtmlControls.HtmlContainerControl.LoadViewState</code> | ✅ 一致 |
| <code>HtmlHead.Api.HtmlControls.HtmlContainerControl.Render</code> | ✅ 一致 |
| <code>HtmlHead.Api.HtmlControls.HtmlContainerControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlHead.Api.HtmlControls.HtmlContainerControl.RenderEndTag</code> | ✅ 一致 |
| <code>HtmlHead.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlHead.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlHead.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlHead.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlHead.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlHead.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlHead.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlHead.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlHead.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlHead.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlHead.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlHead.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlHead.Api.HtmlControls.HtmlGenericControl.#ctor</code> | ✅ 一致 |
| <code>HtmlHead.Api.HtmlControls.HtmlGenericControl.TagName</code> | ✅ 一致 |
| <code>HtmlHead.Api.HtmlControls.HtmlHead.#ctor</code> | ✅ 一致 |
| <code>HtmlHead.Api.HtmlControls.HtmlHead.Description</code> | ✅ 一致 |
| <code>HtmlHead.Api.HtmlControls.HtmlHead.Keywords</code> | ✅ 一致 |
| <code>HtmlHead.Api.HtmlControls.HtmlHead.StyleSheet</code> | ✅ 一致 |
| <code>HtmlHead.Api.HtmlControls.HtmlHead.Title</code> | ✅ 一致 |
| <code>HtmlHead.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlHead.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlHead.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlHead.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlHead.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlHead.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlHead.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlHead.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlHead.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlHead.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlHead.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlHead.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlHead.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlHead.(case list)</code> | ✅ 一致 |
| <code>HtmlHead.(default)</code> | ✅ 一致 |
| <code>HtmlHead.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlHead.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlHead.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlHead.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlHead.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlHead.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlHead.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlHead.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlHead.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlHead.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlHead.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlHead.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlHead.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlHead.Description=empty</code> | ✅ 一致 |
| <code>HtmlHead.Description=japanese</code> | ✅ 一致 |
| <code>HtmlHead.Description=markup</code> | ✅ 一致 |
| <code>HtmlHead.Description=text</code> | ✅ 一致 |
| <code>HtmlHead.Disabled=false</code> | ✅ 一致 |
| <code>HtmlHead.Disabled=true</code> | ✅ 一致 |
| <code>HtmlHead.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlHead.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlHead.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlHead.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlHead.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlHead.ID=empty</code> | ✅ 一致 |
| <code>HtmlHead.ID=japanese</code> | ✅ 一致 |
| <code>HtmlHead.ID=markup</code> | ✅ 一致 |
| <code>HtmlHead.ID=text</code> | ✅ 一致 |
| <code>HtmlHead.InnerHtml=empty</code> | ✅ 一致 |
| <code>HtmlHead.InnerHtml=japanese</code> | ✅ 一致 |
| <code>HtmlHead.InnerHtml=markup</code> | ✅ 一致 |
| <code>HtmlHead.InnerHtml=text</code> | ✅ 一致 |
| <code>HtmlHead.InnerText=empty</code> | ✅ 一致 |
| <code>HtmlHead.InnerText=japanese</code> | ✅ 一致 |
| <code>HtmlHead.InnerText=markup</code> | ✅ 一致 |
| <code>HtmlHead.InnerText=text</code> | ✅ 一致 |
| <code>HtmlHead.Keywords=empty</code> | ✅ 一致 |
| <code>HtmlHead.Keywords=japanese</code> | ✅ 一致 |
| <code>HtmlHead.Keywords=markup</code> | ✅ 一致 |
| <code>HtmlHead.Keywords=markup+Title=text</code> | ✅ 一致 |
| <code>HtmlHead.Keywords=text</code> | ✅ 一致 |
| <code>HtmlHead.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlHead.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlHead.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlHead.SkinID=text</code> | ✅ 一致 |
| <code>HtmlHead.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlHead.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlHead.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlHead.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlHead.TagName=div</code> | ✅ 一致 |
| <code>HtmlHead.TagName=empty</code> | ✅ 一致 |
| <code>HtmlHead.TagName=japanese</code> | ✅ 一致 |
| <code>HtmlHead.TagName=markup</code> | ✅ 一致 |
| <code>HtmlHead.TagName=section</code> | ✅ 一致 |
| <code>HtmlHead.TagName=text</code> | ✅ 一致 |
| <code>HtmlHead.Title=empty</code> | ✅ 一致 |
| <code>HtmlHead.Title=japanese</code> | ✅ 一致 |
| <code>HtmlHead.Title=markup</code> | ✅ 一致 |
| <code>HtmlHead.Title=markup+Description=text</code> | ✅ 一致 |
| <code>HtmlHead.Title=text</code> | ✅ 一致 |
| <code>HtmlHead.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlHead.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlHead.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlHead.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlHead.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlHead.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlHead.Visible=false</code> | ✅ 一致 |
| <code>HtmlHead.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlHead.Visible=true</code> | ✅ 一致 |
| <code>HtmlHead[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlHead[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlHead[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlHead[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlHead[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlHead[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlHead [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlHead [viewstate][dynamic] every input posted by a submit input | ✅ 一致 |
| HtmlHead [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| HtmlHead [dynamic][render] the page&#39;s form, head and title made in code | ✅ 一致 |
| HtmlHead [dynamic][viewstate][postdata] Enter in the form: its default button, its values and view state kept | ✅ 一致 |

</details>
