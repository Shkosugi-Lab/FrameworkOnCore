# HtmlInputImage パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 02:10
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 140 | 140 | 0 | 0 |
| ② HTML | 88 | 88 | 0 | 0 |
| ③ シナリオ | 5 | 5 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 148 件中 148 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 18 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 5 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 80 | 1 | ✅ |
| ViewState の保存と復元 | 7 | 2 | ✅ |
| Visible=false（描画されず状態は保持） | 3 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 3 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 3 | 0 | ✅ |
| ポストバックデータの取り込み | 0 | 1 | ✅ |
| リクエスト検証（ValidateRequestMode） | 3 | 0 | ✅ |
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
| <code>HtmlInputImage.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlInputControl.#ctor</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlInputControl.Name</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlInputControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlInputControl.Type</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlInputControl.Value</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlInputImage.#ctor</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlInputImage.Align</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlInputImage.Alt</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlInputImage.Border</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlInputImage.CausesValidation</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlInputImage.LoadPostData</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlInputImage.OnPreRender</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlInputImage.OnServerClick</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlInputImage.RaisePostBackEvent</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlInputImage.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlInputImage.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlInputImage.ServerClick</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlInputImage.Src</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.HtmlControls.HtmlInputImage.ValidationGroup</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.IPostBackDataHandler.LoadPostData</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.IPostBackDataHandler.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.IPostBackEventHandler.RaisePostBackEvent</code> | ✅ 一致 |
| <code>HtmlInputImage.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlInputImage.(case list)</code> | ✅ 一致 |
| <code>HtmlInputImage.(default)</code> | ✅ 一致 |
| <code>HtmlInputImage.Align=empty</code> | ✅ 一致 |
| <code>HtmlInputImage.Align=japanese</code> | ✅ 一致 |
| <code>HtmlInputImage.Align=markup</code> | ✅ 一致 |
| <code>HtmlInputImage.Align=text</code> | ✅ 一致 |
| <code>HtmlInputImage.Alt=empty</code> | ✅ 一致 |
| <code>HtmlInputImage.Alt=japanese</code> | ✅ 一致 |
| <code>HtmlInputImage.Alt=markup</code> | ✅ 一致 |
| <code>HtmlInputImage.Alt=text</code> | ✅ 一致 |
| <code>HtmlInputImage.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlInputImage.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlInputImage.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlInputImage.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlInputImage.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlInputImage.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlInputImage.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlInputImage.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlInputImage.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlInputImage.Border=-1</code> | ✅ 一致 |
| <code>HtmlInputImage.Border=0</code> | ✅ 一致 |
| <code>HtmlInputImage.Border=3</code> | ✅ 一致 |
| <code>HtmlInputImage.CausesValidation=false</code> | ✅ 一致 |
| <code>HtmlInputImage.CausesValidation=false+ValidationGroup=text</code> | ✅ 一致 |
| <code>HtmlInputImage.CausesValidation=true</code> | ✅ 一致 |
| <code>HtmlInputImage.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlInputImage.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputImage.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlInputImage.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlInputImage.Disabled=false</code> | ✅ 一致 |
| <code>HtmlInputImage.Disabled=true</code> | ✅ 一致 |
| <code>HtmlInputImage.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlInputImage.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlInputImage.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlInputImage.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputImage.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlInputImage.ID=empty</code> | ✅ 一致 |
| <code>HtmlInputImage.ID=japanese</code> | ✅ 一致 |
| <code>HtmlInputImage.ID=markup</code> | ✅ 一致 |
| <code>HtmlInputImage.ID=text</code> | ✅ 一致 |
| <code>HtmlInputImage.Name=empty</code> | ✅ 一致 |
| <code>HtmlInputImage.Name=group</code> | ✅ 一致 |
| <code>HtmlInputImage.Name=group+Value=text</code> | ✅ 一致 |
| <code>HtmlInputImage.Name=japanese</code> | ✅ 一致 |
| <code>HtmlInputImage.Name=markup</code> | ✅ 一致 |
| <code>HtmlInputImage.Name=text</code> | ✅ 一致 |
| <code>HtmlInputImage.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlInputImage.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlInputImage.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlInputImage.SkinID=text</code> | ✅ 一致 |
| <code>HtmlInputImage.Src=absolute</code> | ✅ 一致 |
| <code>HtmlInputImage.Src=app-relative</code> | ✅ 一致 |
| <code>HtmlInputImage.Src=app-relative+Align=text+Border=3</code> | ✅ 一致 |
| <code>HtmlInputImage.Src=app-relative+Alt=markup</code> | ✅ 一致 |
| <code>HtmlInputImage.Src=empty</code> | ✅ 一致 |
| <code>HtmlInputImage.Src=japanese</code> | ✅ 一致 |
| <code>HtmlInputImage.Src=markup</code> | ✅ 一致 |
| <code>HtmlInputImage.Src=text</code> | ✅ 一致 |
| <code>HtmlInputImage.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlInputImage.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlInputImage.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlInputImage.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlInputImage.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlInputImage.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlInputImage.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputImage.ValidationGroup=empty</code> | ✅ 一致 |
| <code>HtmlInputImage.ValidationGroup=japanese</code> | ✅ 一致 |
| <code>HtmlInputImage.ValidationGroup=markup</code> | ✅ 一致 |
| <code>HtmlInputImage.ValidationGroup=text</code> | ✅ 一致 |
| <code>HtmlInputImage.Value=empty</code> | ✅ 一致 |
| <code>HtmlInputImage.Value=japanese</code> | ✅ 一致 |
| <code>HtmlInputImage.Value=markup</code> | ✅ 一致 |
| <code>HtmlInputImage.Value=markup+Disabled=true</code> | ✅ 一致 |
| <code>HtmlInputImage.Value=markup+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputImage.Value=text</code> | ✅ 一致 |
| <code>HtmlInputImage.Value=text+Visible=false</code> | ✅ 一致 |
| <code>HtmlInputImage.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlInputImage.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlInputImage.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputImage.Visible=false</code> | ✅ 一致 |
| <code>HtmlInputImage.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputImage.Visible=true</code> | ✅ 一致 |
| <code>HtmlInputImage[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlInputImage[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlInputImage[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputImage[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlInputImage[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlInputImage[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlInputImage [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlInputImage [viewstate][dynamic][postdata] every input posted by a submit input | ✅ 一致 |
| HtmlInputImage [postbackevent] its server click | ✅ 一致 |
| HtmlInputImage [postbackevent][dynamic] a dynamic one&#39;s server click | ✅ 一致 |
| HtmlInputImage [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
