# HtmlInputRadioButton パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 02:13
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 135 | 135 | 0 | 0 |
| ② HTML | 66 | 66 | 0 | 0 |
| ③ シナリオ | 4 | 4 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 143 件中 143 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 13 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 4 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 58 | 1 | ✅ |
| ViewState の保存と復元 | 7 | 3 | ✅ |
| Visible=false（描画されず状態は保持） | 3 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 4 | 0 | ✅ |
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
| <code>HtmlInputRadioButton.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.HtmlControls.HtmlInputControl.#ctor</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.HtmlControls.HtmlInputControl.Name</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.HtmlControls.HtmlInputControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.HtmlControls.HtmlInputControl.Type</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.HtmlControls.HtmlInputControl.Value</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.HtmlControls.HtmlInputRadioButton.#ctor</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.HtmlControls.HtmlInputRadioButton.Checked</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.HtmlControls.HtmlInputRadioButton.LoadPostData</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.HtmlControls.HtmlInputRadioButton.Name</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.HtmlControls.HtmlInputRadioButton.OnPreRender</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.HtmlControls.HtmlInputRadioButton.OnServerChange</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.HtmlControls.HtmlInputRadioButton.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.HtmlControls.HtmlInputRadioButton.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.HtmlControls.HtmlInputRadioButton.ServerChange</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.HtmlControls.HtmlInputRadioButton.Value</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.IPostBackDataHandler.LoadPostData</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.IPostBackDataHandler.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlInputRadioButton.(case list)</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.(default)</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Checked=false</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Checked=true</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Checked=true+Disabled=true</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Checked=true+Name=group</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Disabled=false</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Disabled=true</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.ID=empty</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.ID=japanese</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.ID=markup</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.ID=text</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Name=empty</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Name=group</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Name=group+Value=text</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Name=japanese</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Name=markup</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Name=text</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.SkinID=text</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Value=empty</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Value=japanese</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Value=markup</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Value=markup+Disabled=true</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Value=markup+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Value=text</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Value=text+Visible=false</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Visible=false</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputRadioButton.Visible=true</code> | ✅ 一致 |
| <code>HtmlInputRadioButton[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlInputRadioButton[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlInputRadioButton[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputRadioButton[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlInputRadioButton[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlInputRadioButton[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlInputRadioButton [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlInputRadioButton [viewstate][dynamic][postdata][change] every input posted by a submit input | ✅ 一致 |
| HtmlInputRadioButton [change][viewstate] posted again unchanged: no change event | ✅ 一致 |
| HtmlInputRadioButton [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
