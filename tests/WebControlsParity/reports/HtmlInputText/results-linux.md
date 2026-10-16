# HtmlInputText パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 02:16
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 135 | 135 | 0 | 0 |
| ② HTML | 70 | 70 | 0 | 0 |
| ③ シナリオ | 6 | 6 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 144 件中 144 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 14 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 6 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 62 | 1 | ✅ |
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
| <code>HtmlInputText.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlInputText.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlInputText.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlInputText.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlInputText.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlInputText.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlInputText.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlInputText.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlInputText.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputText.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlInputText.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlInputText.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlInputText.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlInputText.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlInputText.Api.HtmlControls.HtmlInputControl.#ctor</code> | ✅ 一致 |
| <code>HtmlInputText.Api.HtmlControls.HtmlInputControl.Name</code> | ✅ 一致 |
| <code>HtmlInputText.Api.HtmlControls.HtmlInputControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputText.Api.HtmlControls.HtmlInputControl.Type</code> | ✅ 一致 |
| <code>HtmlInputText.Api.HtmlControls.HtmlInputControl.Value</code> | ✅ 一致 |
| <code>HtmlInputText.Api.HtmlControls.HtmlInputText.#ctor</code> | ✅ 一致 |
| <code>HtmlInputText.Api.HtmlControls.HtmlInputText.LoadPostData</code> | ✅ 一致 |
| <code>HtmlInputText.Api.HtmlControls.HtmlInputText.MaxLength</code> | ✅ 一致 |
| <code>HtmlInputText.Api.HtmlControls.HtmlInputText.OnPreRender</code> | ✅ 一致 |
| <code>HtmlInputText.Api.HtmlControls.HtmlInputText.OnServerChange</code> | ✅ 一致 |
| <code>HtmlInputText.Api.HtmlControls.HtmlInputText.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>HtmlInputText.Api.HtmlControls.HtmlInputText.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputText.Api.HtmlControls.HtmlInputText.ServerChange</code> | ✅ 一致 |
| <code>HtmlInputText.Api.HtmlControls.HtmlInputText.Size</code> | ✅ 一致 |
| <code>HtmlInputText.Api.HtmlControls.HtmlInputText.Value</code> | ✅ 一致 |
| <code>HtmlInputText.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlInputText.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlInputText.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlInputText.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputText.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputText.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlInputText.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlInputText.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlInputText.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlInputText.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlInputText.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlInputText.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlInputText.Api.IPostBackDataHandler.LoadPostData</code> | ✅ 一致 |
| <code>HtmlInputText.Api.IPostBackDataHandler.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>HtmlInputText.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlInputText.(case list)</code> | ✅ 一致 |
| <code>HtmlInputText.(default)</code> | ✅ 一致 |
| <code>HtmlInputText.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlInputText.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlInputText.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlInputText.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlInputText.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlInputText.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlInputText.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlInputText.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlInputText.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlInputText.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlInputText.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputText.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlInputText.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlInputText.Disabled=false</code> | ✅ 一致 |
| <code>HtmlInputText.Disabled=true</code> | ✅ 一致 |
| <code>HtmlInputText.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlInputText.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlInputText.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlInputText.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputText.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlInputText.ID=empty</code> | ✅ 一致 |
| <code>HtmlInputText.ID=japanese</code> | ✅ 一致 |
| <code>HtmlInputText.ID=markup</code> | ✅ 一致 |
| <code>HtmlInputText.ID=text</code> | ✅ 一致 |
| <code>HtmlInputText.MaxLength=-1</code> | ✅ 一致 |
| <code>HtmlInputText.MaxLength=0</code> | ✅ 一致 |
| <code>HtmlInputText.MaxLength=3</code> | ✅ 一致 |
| <code>HtmlInputText.MaxLength=3+Size=3</code> | ✅ 一致 |
| <code>HtmlInputText.Name=empty</code> | ✅ 一致 |
| <code>HtmlInputText.Name=group</code> | ✅ 一致 |
| <code>HtmlInputText.Name=group+Value=text</code> | ✅ 一致 |
| <code>HtmlInputText.Name=japanese</code> | ✅ 一致 |
| <code>HtmlInputText.Name=markup</code> | ✅ 一致 |
| <code>HtmlInputText.Name=text</code> | ✅ 一致 |
| <code>HtmlInputText.Size=-1</code> | ✅ 一致 |
| <code>HtmlInputText.Size=0</code> | ✅ 一致 |
| <code>HtmlInputText.Size=3</code> | ✅ 一致 |
| <code>HtmlInputText.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlInputText.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlInputText.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlInputText.SkinID=text</code> | ✅ 一致 |
| <code>HtmlInputText.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlInputText.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlInputText.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlInputText.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlInputText.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlInputText.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlInputText.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputText.Value=empty</code> | ✅ 一致 |
| <code>HtmlInputText.Value=japanese</code> | ✅ 一致 |
| <code>HtmlInputText.Value=japanese+MaxLength=3</code> | ✅ 一致 |
| <code>HtmlInputText.Value=markup</code> | ✅ 一致 |
| <code>HtmlInputText.Value=markup+Disabled=true</code> | ✅ 一致 |
| <code>HtmlInputText.Value=markup+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputText.Value=text</code> | ✅ 一致 |
| <code>HtmlInputText.Value=text+Visible=false</code> | ✅ 一致 |
| <code>HtmlInputText.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlInputText.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlInputText.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputText.Visible=false</code> | ✅ 一致 |
| <code>HtmlInputText.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputText.Visible=true</code> | ✅ 一致 |
| <code>HtmlInputText[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlInputText[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlInputText[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputText[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlInputText[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlInputText[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlInputText [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlInputText [viewstate][dynamic][postdata][change] every input posted by a submit input | ✅ 一致 |
| HtmlInputText [change][viewstate] posted again unchanged: no change event | ✅ 一致 |
| HtmlInputText [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| HtmlInputText [validation] validated on the server (g1): empty, not chosen | ✅ 一致 |
| HtmlInputText [clientvalidation] validated in the browser (g2): no request | ✅ 一致 |

</details>
