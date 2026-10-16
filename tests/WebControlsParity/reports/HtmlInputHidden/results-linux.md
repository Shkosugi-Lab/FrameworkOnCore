# HtmlInputHidden パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 02:09
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 132 | 132 | 0 | 0 |
| ② HTML | 62 | 62 | 0 | 0 |
| ③ シナリオ | 4 | 4 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 140 件中 140 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 12 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 4 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 54 | 1 | ✅ |
| ViewState の保存と復元 | 7 | 3 | ✅ |
| Visible=false（描画されず状態は保持） | 3 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 3 | 0 | ✅ |
| ポストバックデータの取り込み | 0 | 1 | ✅ |
| 変更イベント（*Changed、ServerChange） | 0 | 2 | ✅ |
| リクエスト検証（ValidateRequestMode） | 3 | 0 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlInputHidden.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.HtmlControls.HtmlInputControl.#ctor</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.HtmlControls.HtmlInputControl.Name</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.HtmlControls.HtmlInputControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.HtmlControls.HtmlInputControl.Type</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.HtmlControls.HtmlInputControl.Value</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.HtmlControls.HtmlInputHidden.#ctor</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.HtmlControls.HtmlInputHidden.LoadPostData</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.HtmlControls.HtmlInputHidden.OnPreRender</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.HtmlControls.HtmlInputHidden.OnServerChange</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.HtmlControls.HtmlInputHidden.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.HtmlControls.HtmlInputHidden.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.HtmlControls.HtmlInputHidden.ServerChange</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.IPostBackDataHandler.LoadPostData</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.IPostBackDataHandler.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>HtmlInputHidden.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlInputHidden.(case list)</code> | ✅ 一致 |
| <code>HtmlInputHidden.(default)</code> | ✅ 一致 |
| <code>HtmlInputHidden.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlInputHidden.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlInputHidden.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlInputHidden.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlInputHidden.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlInputHidden.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlInputHidden.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlInputHidden.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlInputHidden.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlInputHidden.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlInputHidden.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputHidden.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlInputHidden.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlInputHidden.Disabled=false</code> | ✅ 一致 |
| <code>HtmlInputHidden.Disabled=true</code> | ✅ 一致 |
| <code>HtmlInputHidden.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlInputHidden.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlInputHidden.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlInputHidden.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputHidden.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlInputHidden.ID=empty</code> | ✅ 一致 |
| <code>HtmlInputHidden.ID=japanese</code> | ✅ 一致 |
| <code>HtmlInputHidden.ID=markup</code> | ✅ 一致 |
| <code>HtmlInputHidden.ID=text</code> | ✅ 一致 |
| <code>HtmlInputHidden.Name=empty</code> | ✅ 一致 |
| <code>HtmlInputHidden.Name=group</code> | ✅ 一致 |
| <code>HtmlInputHidden.Name=group+Value=text</code> | ✅ 一致 |
| <code>HtmlInputHidden.Name=japanese</code> | ✅ 一致 |
| <code>HtmlInputHidden.Name=markup</code> | ✅ 一致 |
| <code>HtmlInputHidden.Name=text</code> | ✅ 一致 |
| <code>HtmlInputHidden.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlInputHidden.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlInputHidden.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlInputHidden.SkinID=text</code> | ✅ 一致 |
| <code>HtmlInputHidden.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlInputHidden.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlInputHidden.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlInputHidden.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlInputHidden.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlInputHidden.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlInputHidden.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputHidden.Value=empty</code> | ✅ 一致 |
| <code>HtmlInputHidden.Value=japanese</code> | ✅ 一致 |
| <code>HtmlInputHidden.Value=markup</code> | ✅ 一致 |
| <code>HtmlInputHidden.Value=markup+Disabled=true</code> | ✅ 一致 |
| <code>HtmlInputHidden.Value=markup+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputHidden.Value=text</code> | ✅ 一致 |
| <code>HtmlInputHidden.Value=text+Visible=false</code> | ✅ 一致 |
| <code>HtmlInputHidden.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlInputHidden.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlInputHidden.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputHidden.Visible=false</code> | ✅ 一致 |
| <code>HtmlInputHidden.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputHidden.Visible=true</code> | ✅ 一致 |
| <code>HtmlInputHidden[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlInputHidden[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlInputHidden[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputHidden[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlInputHidden[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlInputHidden[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlInputHidden [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlInputHidden [viewstate][dynamic][postdata][change] every input posted by a submit input | ✅ 一致 |
| HtmlInputHidden [change][viewstate] posted again unchanged: no change event | ✅ 一致 |
| HtmlInputHidden [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
