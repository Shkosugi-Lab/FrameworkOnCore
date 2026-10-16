# HtmlInputCheckBox パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-11 23:50
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 133 | 133 | 0 | 0 |
| ② HTML | 66 | 66 | 0 | 0 |
| ③ シナリオ | 4 | 4 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 141 件中 141 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 13 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 4 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 58 | 1 | ✅ |
| ViewState の保存と復元 | 8 | 3 | ✅ |
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
| <code>HtmlInputCheckBox.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.HtmlControls.HtmlInputCheckBox.#ctor</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.HtmlControls.HtmlInputCheckBox.Checked</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.HtmlControls.HtmlInputCheckBox.LoadPostData</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.HtmlControls.HtmlInputCheckBox.OnPreRender</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.HtmlControls.HtmlInputCheckBox.OnServerChange</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.HtmlControls.HtmlInputCheckBox.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.HtmlControls.HtmlInputCheckBox.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.HtmlControls.HtmlInputCheckBox.ServerChange</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.HtmlControls.HtmlInputControl.#ctor</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.HtmlControls.HtmlInputControl.Name</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.HtmlControls.HtmlInputControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.HtmlControls.HtmlInputControl.Type</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.HtmlControls.HtmlInputControl.Value</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.IPostBackDataHandler.LoadPostData</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.IPostBackDataHandler.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlInputCheckBox.(case list)</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.(default)</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Checked=false</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Checked=true</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Checked=true+Disabled=true</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Checked=true+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Disabled=false</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Disabled=true</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.ID=empty</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.ID=japanese</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.ID=markup</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.ID=text</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Name=empty</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Name=group</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Name=group+Value=text</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Name=japanese</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Name=markup</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Name=text</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.SkinID=text</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Value=empty</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Value=japanese</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Value=markup</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Value=markup+Disabled=true</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Value=markup+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Value=text</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Value=text+Visible=false</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Visible=false</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputCheckBox.Visible=true</code> | ✅ 一致 |
| <code>HtmlInputCheckBox[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlInputCheckBox[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlInputCheckBox[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputCheckBox[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlInputCheckBox[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlInputCheckBox[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlInputCheckBox [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlInputCheckBox [viewstate][dynamic][postdata][change] every input posted by a submit input | ✅ 一致 |
| HtmlInputCheckBox [change][viewstate] posted again unchanged: no change event | ✅ 一致 |
| HtmlInputCheckBox [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
