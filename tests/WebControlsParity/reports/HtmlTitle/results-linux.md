# HtmlTitle パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 02:24
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 123 | 123 | 0 | 0 |
| ② HTML | 54 | 54 | 0 | 0 |
| ③ シナリオ | 5 | 5 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 131 件中 131 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 11 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 5 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 46 | 2 | ✅ |
| ViewState の保存と復元 | 6 | 3 | ✅ |
| Visible=false（描画されず状態は保持） | 3 | 1 | ✅ |
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
| <code>HtmlTitle.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlTitle.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlTitle.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlTitle.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlTitle.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlTitle.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlTitle.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlTitle.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlTitle.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlTitle.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlTitle.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlTitle.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlTitle.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlTitle.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlTitle.Api.HtmlControls.HtmlTitle.#ctor</code> | ✅ 一致 |
| <code>HtmlTitle.Api.HtmlControls.HtmlTitle.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlTitle.Api.HtmlControls.HtmlTitle.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlTitle.Api.HtmlControls.HtmlTitle.Render</code> | ✅ 一致 |
| <code>HtmlTitle.Api.HtmlControls.HtmlTitle.Text</code> | ✅ 一致 |
| <code>HtmlTitle.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlTitle.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlTitle.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlTitle.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlTitle.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlTitle.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlTitle.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlTitle.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlTitle.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlTitle.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlTitle.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlTitle.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlTitle.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlTitle.(case list)</code> | ✅ 一致 |
| <code>HtmlTitle.(default)</code> | ✅ 一致 |
| <code>HtmlTitle.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlTitle.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlTitle.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlTitle.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlTitle.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlTitle.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlTitle.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlTitle.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlTitle.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlTitle.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlTitle.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlTitle.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlTitle.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlTitle.Disabled=false</code> | ✅ 一致 |
| <code>HtmlTitle.Disabled=true</code> | ✅ 一致 |
| <code>HtmlTitle.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlTitle.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlTitle.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlTitle.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlTitle.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlTitle.ID=empty</code> | ✅ 一致 |
| <code>HtmlTitle.ID=japanese</code> | ✅ 一致 |
| <code>HtmlTitle.ID=markup</code> | ✅ 一致 |
| <code>HtmlTitle.ID=text</code> | ✅ 一致 |
| <code>HtmlTitle.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlTitle.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlTitle.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlTitle.SkinID=text</code> | ✅ 一致 |
| <code>HtmlTitle.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlTitle.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlTitle.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlTitle.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlTitle.Text=empty</code> | ✅ 一致 |
| <code>HtmlTitle.Text=japanese</code> | ✅ 一致 |
| <code>HtmlTitle.Text=markup</code> | ✅ 一致 |
| <code>HtmlTitle.Text=markup+Visible=false</code> | ✅ 一致 |
| <code>HtmlTitle.Text=text</code> | ✅ 一致 |
| <code>HtmlTitle.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlTitle.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlTitle.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlTitle.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlTitle.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlTitle.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlTitle.Visible=false</code> | ✅ 一致 |
| <code>HtmlTitle.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlTitle.Visible=true</code> | ✅ 一致 |
| <code>HtmlTitle[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlTitle[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlTitle[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlTitle[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlTitle[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlTitle[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlTitle [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlTitle [viewstate][dynamic] every input posted by a submit input | ✅ 一致 |
| HtmlTitle [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| HtmlTitle [dynamic][render] the page&#39;s form, head and title made in code | ✅ 一致 |
| HtmlTitle [dynamic][viewstate][postdata] Enter in the form: its default button, its values and view state kept | ✅ 一致 |

</details>
