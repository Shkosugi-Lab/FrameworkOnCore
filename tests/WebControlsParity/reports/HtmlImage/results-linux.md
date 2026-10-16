# HtmlImage パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 02:04
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 126 | 126 | 0 | 0 |
| ② HTML | 75 | 75 | 0 | 0 |
| ③ シナリオ | 3 | 3 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 134 件中 134 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 16 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 3 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 67 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 2 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
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
| <code>HtmlImage.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlImage.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlImage.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlImage.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlImage.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlImage.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlImage.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlImage.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlImage.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlImage.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlImage.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlImage.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlImage.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlImage.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlImage.Api.HtmlControls.HtmlImage.#ctor</code> | ✅ 一致 |
| <code>HtmlImage.Api.HtmlControls.HtmlImage.Align</code> | ✅ 一致 |
| <code>HtmlImage.Api.HtmlControls.HtmlImage.Alt</code> | ✅ 一致 |
| <code>HtmlImage.Api.HtmlControls.HtmlImage.Border</code> | ✅ 一致 |
| <code>HtmlImage.Api.HtmlControls.HtmlImage.Height</code> | ✅ 一致 |
| <code>HtmlImage.Api.HtmlControls.HtmlImage.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlImage.Api.HtmlControls.HtmlImage.Src</code> | ✅ 一致 |
| <code>HtmlImage.Api.HtmlControls.HtmlImage.Width</code> | ✅ 一致 |
| <code>HtmlImage.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlImage.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlImage.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlImage.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlImage.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlImage.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlImage.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlImage.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlImage.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlImage.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlImage.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlImage.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlImage.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlImage.(case list)</code> | ✅ 一致 |
| <code>HtmlImage.(default)</code> | ✅ 一致 |
| <code>HtmlImage.Align=empty</code> | ✅ 一致 |
| <code>HtmlImage.Align=japanese</code> | ✅ 一致 |
| <code>HtmlImage.Align=markup</code> | ✅ 一致 |
| <code>HtmlImage.Align=text</code> | ✅ 一致 |
| <code>HtmlImage.Alt=empty</code> | ✅ 一致 |
| <code>HtmlImage.Alt=japanese</code> | ✅ 一致 |
| <code>HtmlImage.Alt=markup</code> | ✅ 一致 |
| <code>HtmlImage.Alt=text</code> | ✅ 一致 |
| <code>HtmlImage.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlImage.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlImage.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlImage.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlImage.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlImage.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlImage.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlImage.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlImage.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlImage.Border=-1</code> | ✅ 一致 |
| <code>HtmlImage.Border=0</code> | ✅ 一致 |
| <code>HtmlImage.Border=3</code> | ✅ 一致 |
| <code>HtmlImage.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlImage.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlImage.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlImage.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlImage.Disabled=false</code> | ✅ 一致 |
| <code>HtmlImage.Disabled=true</code> | ✅ 一致 |
| <code>HtmlImage.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlImage.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlImage.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlImage.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlImage.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlImage.Height=-1</code> | ✅ 一致 |
| <code>HtmlImage.Height=0</code> | ✅ 一致 |
| <code>HtmlImage.Height=3</code> | ✅ 一致 |
| <code>HtmlImage.ID=empty</code> | ✅ 一致 |
| <code>HtmlImage.ID=japanese</code> | ✅ 一致 |
| <code>HtmlImage.ID=markup</code> | ✅ 一致 |
| <code>HtmlImage.ID=text</code> | ✅ 一致 |
| <code>HtmlImage.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlImage.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlImage.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlImage.SkinID=text</code> | ✅ 一致 |
| <code>HtmlImage.Src=absolute</code> | ✅ 一致 |
| <code>HtmlImage.Src=absolute+Align=text</code> | ✅ 一致 |
| <code>HtmlImage.Src=app-relative</code> | ✅ 一致 |
| <code>HtmlImage.Src=app-relative+Alt=markup</code> | ✅ 一致 |
| <code>HtmlImage.Src=app-relative+Width=3+Height=3+Border=0</code> | ✅ 一致 |
| <code>HtmlImage.Src=empty</code> | ✅ 一致 |
| <code>HtmlImage.Src=japanese</code> | ✅ 一致 |
| <code>HtmlImage.Src=markup</code> | ✅ 一致 |
| <code>HtmlImage.Src=text</code> | ✅ 一致 |
| <code>HtmlImage.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlImage.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlImage.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlImage.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlImage.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlImage.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlImage.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlImage.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlImage.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlImage.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlImage.Visible=false</code> | ✅ 一致 |
| <code>HtmlImage.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlImage.Visible=true</code> | ✅ 一致 |
| <code>HtmlImage.Width=-1</code> | ✅ 一致 |
| <code>HtmlImage.Width=0</code> | ✅ 一致 |
| <code>HtmlImage.Width=3</code> | ✅ 一致 |
| <code>HtmlImage[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlImage[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlImage[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlImage[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlImage[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlImage[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlImage [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlImage [viewstate][dynamic] every input posted by a submit input | ✅ 一致 |
| HtmlImage [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |

</details>
