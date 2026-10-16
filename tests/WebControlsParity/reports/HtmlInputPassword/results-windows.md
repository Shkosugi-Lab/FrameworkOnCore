# HtmlInputPassword パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-11 23:55
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 137 | 137 | 0 | 0 |
| ② HTML | 69 | 69 | 0 | 0 |
| ③ シナリオ | 6 | 6 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 146 件中 146 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 14 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 6 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 61 | 1 | ✅ |
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
| <code>HtmlInputPassword.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlInputControl.#ctor</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlInputControl.Name</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlInputControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlInputControl.Type</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlInputControl.Value</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlInputPassword.#ctor</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlInputPassword.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlInputText.#ctor</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlInputText.LoadPostData</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlInputText.MaxLength</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlInputText.OnPreRender</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlInputText.OnServerChange</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlInputText.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlInputText.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlInputText.ServerChange</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlInputText.Size</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.HtmlControls.HtmlInputText.Value</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.IPostBackDataHandler.LoadPostData</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.IPostBackDataHandler.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>HtmlInputPassword.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlInputPassword.(case list)</code> | ✅ 一致 |
| <code>HtmlInputPassword.(default)</code> | ✅ 一致 |
| <code>HtmlInputPassword.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlInputPassword.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlInputPassword.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlInputPassword.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlInputPassword.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlInputPassword.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlInputPassword.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlInputPassword.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlInputPassword.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlInputPassword.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlInputPassword.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputPassword.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlInputPassword.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlInputPassword.Disabled=false</code> | ✅ 一致 |
| <code>HtmlInputPassword.Disabled=true</code> | ✅ 一致 |
| <code>HtmlInputPassword.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlInputPassword.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlInputPassword.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlInputPassword.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputPassword.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlInputPassword.ID=empty</code> | ✅ 一致 |
| <code>HtmlInputPassword.ID=japanese</code> | ✅ 一致 |
| <code>HtmlInputPassword.ID=markup</code> | ✅ 一致 |
| <code>HtmlInputPassword.ID=text</code> | ✅ 一致 |
| <code>HtmlInputPassword.MaxLength=-1</code> | ✅ 一致 |
| <code>HtmlInputPassword.MaxLength=0</code> | ✅ 一致 |
| <code>HtmlInputPassword.MaxLength=3</code> | ✅ 一致 |
| <code>HtmlInputPassword.MaxLength=3+Size=3</code> | ✅ 一致 |
| <code>HtmlInputPassword.Name=empty</code> | ✅ 一致 |
| <code>HtmlInputPassword.Name=group</code> | ✅ 一致 |
| <code>HtmlInputPassword.Name=group+Value=text</code> | ✅ 一致 |
| <code>HtmlInputPassword.Name=japanese</code> | ✅ 一致 |
| <code>HtmlInputPassword.Name=markup</code> | ✅ 一致 |
| <code>HtmlInputPassword.Name=text</code> | ✅ 一致 |
| <code>HtmlInputPassword.Size=-1</code> | ✅ 一致 |
| <code>HtmlInputPassword.Size=0</code> | ✅ 一致 |
| <code>HtmlInputPassword.Size=3</code> | ✅ 一致 |
| <code>HtmlInputPassword.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlInputPassword.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlInputPassword.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlInputPassword.SkinID=text</code> | ✅ 一致 |
| <code>HtmlInputPassword.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlInputPassword.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlInputPassword.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlInputPassword.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlInputPassword.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlInputPassword.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlInputPassword.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputPassword.Value=empty</code> | ✅ 一致 |
| <code>HtmlInputPassword.Value=japanese</code> | ✅ 一致 |
| <code>HtmlInputPassword.Value=markup</code> | ✅ 一致 |
| <code>HtmlInputPassword.Value=markup+Disabled=true</code> | ✅ 一致 |
| <code>HtmlInputPassword.Value=markup+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputPassword.Value=text</code> | ✅ 一致 |
| <code>HtmlInputPassword.Value=text+Visible=false</code> | ✅ 一致 |
| <code>HtmlInputPassword.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlInputPassword.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlInputPassword.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputPassword.Visible=false</code> | ✅ 一致 |
| <code>HtmlInputPassword.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputPassword.Visible=true</code> | ✅ 一致 |
| <code>HtmlInputPassword[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlInputPassword[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlInputPassword[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputPassword[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlInputPassword[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlInputPassword[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlInputPassword [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlInputPassword [viewstate][dynamic][postdata][change] every input posted by a submit input | ✅ 一致 |
| HtmlInputPassword [change][viewstate] posted again unchanged: no change event | ✅ 一致 |
| HtmlInputPassword [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| HtmlInputPassword [validation] validated on the server (g1): empty, not chosen | ✅ 一致 |
| HtmlInputPassword [clientvalidation] validated in the browser (g2): no request | ✅ 一致 |

</details>
