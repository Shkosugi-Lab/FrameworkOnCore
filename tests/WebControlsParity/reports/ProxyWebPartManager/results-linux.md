# ProxyWebPartManager パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 02:55
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 114 | 114 | 0 | 0 |
| ② HTML | 37 | 37 | 0 | 0 |
| ③ シナリオ | 12 | 12 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 121 件中 121 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 9 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 12 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 29 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 11 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 1 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>ProxyWebPartManager.Api.Control.#ctor</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.Adapter</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.ClientID</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.Context</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.Controls</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.DataBind</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.Dispose</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.Disposed</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.Events</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.FindControl</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.Focus</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.HasControls</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.ID</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.Init</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.Load</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.OnInit</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.Page</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.Parent</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.PreRender</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.Render</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.Site</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.SkinID</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.Unload</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.ViewState</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.Control.Visible</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.WebControls.WebParts.ProxyWebPartManager.#ctor</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.WebControls.WebParts.ProxyWebPartManager.ClientID</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.WebControls.WebParts.ProxyWebPartManager.Controls</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.WebControls.WebParts.ProxyWebPartManager.CreateControlCollection</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.WebControls.WebParts.ProxyWebPartManager.EnableTheming</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.WebControls.WebParts.ProxyWebPartManager.Focus</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.WebControls.WebParts.ProxyWebPartManager.OnInit</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.WebControls.WebParts.ProxyWebPartManager.SkinID</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.WebControls.WebParts.ProxyWebPartManager.StaticConnections</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Api.WebControls.WebParts.ProxyWebPartManager.Visible</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>ProxyWebPartManager.(case list)</code> | ✅ 一致 |
| <code>ProxyWebPartManager.(default)</code> | ✅ 一致 |
| <code>ProxyWebPartManager.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>ProxyWebPartManager.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>ProxyWebPartManager.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>ProxyWebPartManager.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>ProxyWebPartManager.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ProxyWebPartManager.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ProxyWebPartManager.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ProxyWebPartManager.ClientIDMode=Static</code> | ✅ 一致 |
| <code>ProxyWebPartManager.EnableTheming=false</code> | ✅ 一致 |
| <code>ProxyWebPartManager.EnableTheming=true</code> | ✅ 一致 |
| <code>ProxyWebPartManager.EnableViewState=false</code> | ✅ 一致 |
| <code>ProxyWebPartManager.EnableViewState=true</code> | ✅ 一致 |
| <code>ProxyWebPartManager.ID=empty</code> | ✅ 一致 |
| <code>ProxyWebPartManager.ID=japanese</code> | ✅ 一致 |
| <code>ProxyWebPartManager.ID=markup</code> | ✅ 一致 |
| <code>ProxyWebPartManager.ID=text</code> | ✅ 一致 |
| <code>ProxyWebPartManager.SkinID=empty</code> | ✅ 一致 |
| <code>ProxyWebPartManager.SkinID=japanese</code> | ✅ 一致 |
| <code>ProxyWebPartManager.SkinID=markup</code> | ✅ 一致 |
| <code>ProxyWebPartManager.SkinID=text</code> | ✅ 一致 |
| <code>ProxyWebPartManager.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>ProxyWebPartManager.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>ProxyWebPartManager.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>ProxyWebPartManager.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>ProxyWebPartManager.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>ProxyWebPartManager.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Visible=false</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>ProxyWebPartManager.Visible=true</code> | ✅ 一致 |
| <code>ProxyWebPartManager[naming container].(default)</code> | ✅ 一致 |
| <code>ProxyWebPartManager[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ProxyWebPartManager[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ProxyWebPartManager[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ProxyWebPartManager[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>ProxyWebPartManager[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| ProxyWebPartManager [render][databind][dynamic] first request (browse): the parts in their zones, a zone made in code | ✅ 一致 |
| ProxyWebPartManager [viewstate] the design mode | ✅ 一致 |
| ProxyWebPartManager [viewstate] the catalog mode: the catalog zone and its parts | ✅ 一致 |
| ProxyWebPartManager [viewstate] a part added from the catalog (its check box posted, the zone&#39;s Add) | ✅ 一致 |
| ProxyWebPartManager [viewstate] the edit mode: the editor zone and its parts for the first part | ✅ 一致 |
| ProxyWebPartManager [viewstate] the first part&#39;s title edited (the editor zone&#39;s OK) | ✅ 一致 |
| ProxyWebPartManager [viewstate] the connect mode for the first part | ✅ 一致 |
| ProxyWebPartManager [viewstate] the connections zone closed | ✅ 一致 |
| ProxyWebPartManager [viewstate] a part minimized by its zone&#39;s verb | ✅ 一致 |
| ProxyWebPartManager [viewstate] restored | ✅ 一致 |
| ProxyWebPartManager [viewstate] a part of the zone made in code minimized | ✅ 一致 |
| ProxyWebPartManager [visible][viewstate] a hidden zone shown | ✅ 一致 |

</details>
