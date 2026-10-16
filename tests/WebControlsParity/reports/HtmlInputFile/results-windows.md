# HtmlInputFile パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-11 23:51
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 134 | 134 | 0 | 0 |
| ② HTML | 70 | 70 | 0 | 0 |
| ③ シナリオ | 5 | 5 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 142 件中 142 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 15 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 5 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 62 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 2 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 3 | 0 | ✅ |
| ポストバックデータの取り込み | 0 | 1 | ✅ |
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
| <code>HtmlInputFile.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.HtmlControls.HtmlInputControl.#ctor</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.HtmlControls.HtmlInputControl.Name</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.HtmlControls.HtmlInputControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.HtmlControls.HtmlInputControl.Type</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.HtmlControls.HtmlInputControl.Value</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.HtmlControls.HtmlInputFile.#ctor</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.HtmlControls.HtmlInputFile.Accept</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.HtmlControls.HtmlInputFile.LoadPostData</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.HtmlControls.HtmlInputFile.MaxLength</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.HtmlControls.HtmlInputFile.OnPreRender</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.HtmlControls.HtmlInputFile.PostedFile</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.HtmlControls.HtmlInputFile.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.HtmlControls.HtmlInputFile.Size</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.HtmlControls.HtmlInputFile.Value</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.IPostBackDataHandler.LoadPostData</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.IPostBackDataHandler.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>HtmlInputFile.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlInputFile.(case list)</code> | ✅ 一致 |
| <code>HtmlInputFile.(default)</code> | ✅ 一致 |
| <code>HtmlInputFile.Accept=empty</code> | ✅ 一致 |
| <code>HtmlInputFile.Accept=japanese</code> | ✅ 一致 |
| <code>HtmlInputFile.Accept=markup</code> | ✅ 一致 |
| <code>HtmlInputFile.Accept=text</code> | ✅ 一致 |
| <code>HtmlInputFile.Accept=text+MaxLength=3+Size=3</code> | ✅ 一致 |
| <code>HtmlInputFile.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlInputFile.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlInputFile.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlInputFile.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlInputFile.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlInputFile.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlInputFile.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlInputFile.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlInputFile.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlInputFile.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlInputFile.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputFile.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlInputFile.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlInputFile.Disabled=false</code> | ✅ 一致 |
| <code>HtmlInputFile.Disabled=true</code> | ✅ 一致 |
| <code>HtmlInputFile.Disabled=true+Accept=text</code> | ✅ 一致 |
| <code>HtmlInputFile.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlInputFile.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlInputFile.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlInputFile.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputFile.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlInputFile.ID=empty</code> | ✅ 一致 |
| <code>HtmlInputFile.ID=japanese</code> | ✅ 一致 |
| <code>HtmlInputFile.ID=markup</code> | ✅ 一致 |
| <code>HtmlInputFile.ID=text</code> | ✅ 一致 |
| <code>HtmlInputFile.MaxLength=-1</code> | ✅ 一致 |
| <code>HtmlInputFile.MaxLength=0</code> | ✅ 一致 |
| <code>HtmlInputFile.MaxLength=3</code> | ✅ 一致 |
| <code>HtmlInputFile.Name=empty</code> | ✅ 一致 |
| <code>HtmlInputFile.Name=group</code> | ✅ 一致 |
| <code>HtmlInputFile.Name=japanese</code> | ✅ 一致 |
| <code>HtmlInputFile.Name=markup</code> | ✅ 一致 |
| <code>HtmlInputFile.Name=text</code> | ✅ 一致 |
| <code>HtmlInputFile.Size=-1</code> | ✅ 一致 |
| <code>HtmlInputFile.Size=0</code> | ✅ 一致 |
| <code>HtmlInputFile.Size=3</code> | ✅ 一致 |
| <code>HtmlInputFile.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlInputFile.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlInputFile.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlInputFile.SkinID=text</code> | ✅ 一致 |
| <code>HtmlInputFile.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlInputFile.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlInputFile.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlInputFile.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlInputFile.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlInputFile.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlInputFile.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputFile.Value=empty</code> | ✅ 一致 |
| <code>HtmlInputFile.Value=japanese</code> | ✅ 一致 |
| <code>HtmlInputFile.Value=markup</code> | ✅ 一致 |
| <code>HtmlInputFile.Value=text</code> | ✅ 一致 |
| <code>HtmlInputFile.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlInputFile.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlInputFile.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputFile.Visible=false</code> | ✅ 一致 |
| <code>HtmlInputFile.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlInputFile.Visible=true</code> | ✅ 一致 |
| <code>HtmlInputFile[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlInputFile[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlInputFile[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlInputFile[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlInputFile[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlInputFile[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlInputFile [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlInputFile [viewstate][dynamic][postdata] every input posted by a submit input | ✅ 一致 |
| HtmlInputFile [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| HtmlInputFile [validation] validated on the server (g1): empty, not chosen | ✅ 一致 |
| HtmlInputFile [clientvalidation] validated in the browser (g2): no request | ✅ 一致 |

</details>
