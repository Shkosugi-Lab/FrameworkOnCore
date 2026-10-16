# HtmlTextArea パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 02:23
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 140 | 140 | 0 | 0 |
| ② HTML | 78 | 78 | 0 | 0 |
| ③ シナリオ | 6 | 6 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 149 件中 149 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 16 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 6 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 70 | 1 | ✅ |
| ViewState の保存と復元 | 7 | 3 | ✅ |
| Visible=false（描画されず状態は保持） | 3 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 3 | 0 | ✅ |
| ポストバックデータの取り込み | 0 | 1 | ✅ |
| 変更イベント（*Changed、ServerChange） | 0 | 2 | ✅ |
| リクエスト検証（ValidateRequestMode） | 3 | 0 | ✅ |
| 検証対象（サーバー側の検証） | 0 | 1 | ✅ |
| 検証対象（ブラウザ側の検証） | 0 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlTextArea.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlContainerControl.#ctor</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlContainerControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlContainerControl.InnerHtml</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlContainerControl.InnerText</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlContainerControl.LoadViewState</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlContainerControl.Render</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlContainerControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlContainerControl.RenderEndTag</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlTextArea.#ctor</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlTextArea.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlTextArea.Cols</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlTextArea.LoadPostData</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlTextArea.Name</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlTextArea.OnPreRender</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlTextArea.OnServerChange</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlTextArea.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlTextArea.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlTextArea.Rows</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlTextArea.ServerChange</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.HtmlControls.HtmlTextArea.Value</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.IPostBackDataHandler.LoadPostData</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.IPostBackDataHandler.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>HtmlTextArea.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlTextArea.(case list)</code> | ✅ 一致 |
| <code>HtmlTextArea.(default)</code> | ✅ 一致 |
| <code>HtmlTextArea.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlTextArea.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlTextArea.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlTextArea.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlTextArea.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlTextArea.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlTextArea.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlTextArea.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlTextArea.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlTextArea.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlTextArea.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlTextArea.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlTextArea.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlTextArea.Cols=-1</code> | ✅ 一致 |
| <code>HtmlTextArea.Cols=0</code> | ✅ 一致 |
| <code>HtmlTextArea.Cols=3</code> | ✅ 一致 |
| <code>HtmlTextArea.Disabled=false</code> | ✅ 一致 |
| <code>HtmlTextArea.Disabled=true</code> | ✅ 一致 |
| <code>HtmlTextArea.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlTextArea.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlTextArea.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlTextArea.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlTextArea.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlTextArea.ID=empty</code> | ✅ 一致 |
| <code>HtmlTextArea.ID=japanese</code> | ✅ 一致 |
| <code>HtmlTextArea.ID=markup</code> | ✅ 一致 |
| <code>HtmlTextArea.ID=text</code> | ✅ 一致 |
| <code>HtmlTextArea.InnerHtml=empty</code> | ✅ 一致 |
| <code>HtmlTextArea.InnerHtml=japanese</code> | ✅ 一致 |
| <code>HtmlTextArea.InnerHtml=markup</code> | ✅ 一致 |
| <code>HtmlTextArea.InnerHtml=text</code> | ✅ 一致 |
| <code>HtmlTextArea.InnerText=empty</code> | ✅ 一致 |
| <code>HtmlTextArea.InnerText=japanese</code> | ✅ 一致 |
| <code>HtmlTextArea.InnerText=markup</code> | ✅ 一致 |
| <code>HtmlTextArea.InnerText=text</code> | ✅ 一致 |
| <code>HtmlTextArea.Name=empty</code> | ✅ 一致 |
| <code>HtmlTextArea.Name=group</code> | ✅ 一致 |
| <code>HtmlTextArea.Name=group+Value=text</code> | ✅ 一致 |
| <code>HtmlTextArea.Name=japanese</code> | ✅ 一致 |
| <code>HtmlTextArea.Name=markup</code> | ✅ 一致 |
| <code>HtmlTextArea.Name=text</code> | ✅ 一致 |
| <code>HtmlTextArea.Rows=-1</code> | ✅ 一致 |
| <code>HtmlTextArea.Rows=0</code> | ✅ 一致 |
| <code>HtmlTextArea.Rows=3</code> | ✅ 一致 |
| <code>HtmlTextArea.Rows=3+Cols=3</code> | ✅ 一致 |
| <code>HtmlTextArea.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlTextArea.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlTextArea.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlTextArea.SkinID=text</code> | ✅ 一致 |
| <code>HtmlTextArea.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlTextArea.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlTextArea.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlTextArea.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlTextArea.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlTextArea.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlTextArea.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlTextArea.Value=empty</code> | ✅ 一致 |
| <code>HtmlTextArea.Value=japanese</code> | ✅ 一致 |
| <code>HtmlTextArea.Value=markup</code> | ✅ 一致 |
| <code>HtmlTextArea.Value=markup+Disabled=true</code> | ✅ 一致 |
| <code>HtmlTextArea.Value=markup+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlTextArea.Value=markup+Rows=3</code> | ✅ 一致 |
| <code>HtmlTextArea.Value=text</code> | ✅ 一致 |
| <code>HtmlTextArea.Value=text+Visible=false</code> | ✅ 一致 |
| <code>HtmlTextArea.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlTextArea.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlTextArea.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlTextArea.Visible=false</code> | ✅ 一致 |
| <code>HtmlTextArea.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlTextArea.Visible=true</code> | ✅ 一致 |
| <code>HtmlTextArea[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlTextArea[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlTextArea[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlTextArea[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlTextArea[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlTextArea[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlTextArea [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlTextArea [viewstate][dynamic][postdata][change] every input posted by a submit input | ✅ 一致 |
| HtmlTextArea [change][viewstate] posted again unchanged: no change event | ✅ 一致 |
| HtmlTextArea [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| HtmlTextArea [validation] validated on the server (g1): empty, not chosen | ✅ 一致 |
| HtmlTextArea [clientvalidation] validated in the browser (g2): no request | ✅ 一致 |

</details>
