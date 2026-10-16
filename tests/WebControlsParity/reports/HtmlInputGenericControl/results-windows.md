# HtmlInputGenericControl パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-11 23:52
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 131 | 131 | 0 | 0 |
| ② HTML | 62 | 62 | 0 | 0 |
| ③ シナリオ | 6 | 6 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 140 件中 140 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 12 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 6 スナップショット（機構は下表）

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
| <code>HtmlInputGenericControl.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.HtmlControls.HtmlInputControl.#ctor</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.HtmlControls.HtmlInputControl.Name</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.HtmlControls.HtmlInputControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.HtmlControls.HtmlInputControl.Type</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.HtmlControls.HtmlInputControl.Value</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.HtmlControls.HtmlInputGenericControl.#ctor</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.HtmlControls.HtmlInputGenericControl.LoadPostData</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.HtmlControls.HtmlInputGenericControl.OnPreRender</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.HtmlControls.HtmlInputGenericControl.OnServerChange</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.HtmlControls.HtmlInputGenericControl.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.HtmlControls.HtmlInputGenericControl.ServerChange</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.IPostBackDataHandler.LoadPostData</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.IPostBackDataHandler.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlInputGenericControl.(case list)</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.(default)</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Disabled=false</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Disabled=true</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.ID=empty</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.ID=japanese</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.ID=markup</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.ID=text</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Name=empty</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Name=group</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Name=group+Value=text</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Name=japanese</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Name=markup</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Name=text</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.SkinID=text</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Value=empty</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Value=japanese</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Value=markup</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Value=markup+Disabled=true</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Value=markup+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Value=text</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Value=text+Visible=false</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Visible=false</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputGenericControl.Visible=true</code> | ✅ 一致 |
| <code>HtmlInputGenericControl[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlInputGenericControl[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlInputGenericControl[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputGenericControl[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlInputGenericControl[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlInputGenericControl[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlInputGenericControl [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlInputGenericControl [viewstate][dynamic][postdata][change] every input posted by a submit input | ✅ 一致 |
| HtmlInputGenericControl [change][viewstate] posted again unchanged: no change event | ✅ 一致 |
| HtmlInputGenericControl [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| HtmlInputGenericControl [validation] validated on the server (g1): empty, not chosen | ✅ 一致 |
| HtmlInputGenericControl [clientvalidation] validated in the browser (g2): no request | ✅ 一致 |

</details>
