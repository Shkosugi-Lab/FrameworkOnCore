# DataPager パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 01:45
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 137 | 137 | 0 | 0 |
| ② HTML | 61 | 61 | 0 | 0 |
| ③ シナリオ | 8 | 8 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 144 件中 144 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 12 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 8 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 47 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 2 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 0 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 3 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| ページング | 7 | 6 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>DataPager.Api.Control.#ctor</code> | ✅ 一致 |
| <code>DataPager.Api.Control.Adapter</code> | ✅ 一致 |
| <code>DataPager.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>DataPager.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>DataPager.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>DataPager.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>DataPager.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>DataPager.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>DataPager.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>DataPager.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>DataPager.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>DataPager.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>DataPager.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>DataPager.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>DataPager.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>DataPager.Api.Control.ClientID</code> | ✅ 一致 |
| <code>DataPager.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>DataPager.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>DataPager.Api.Control.Context</code> | ✅ 一致 |
| <code>DataPager.Api.Control.Controls</code> | ✅ 一致 |
| <code>DataPager.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>DataPager.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>DataPager.Api.Control.DataBind</code> | ✅ 一致 |
| <code>DataPager.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>DataPager.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>DataPager.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>DataPager.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>DataPager.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>DataPager.Api.Control.Dispose</code> | ✅ 一致 |
| <code>DataPager.Api.Control.Disposed</code> | ✅ 一致 |
| <code>DataPager.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>DataPager.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>DataPager.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>DataPager.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>DataPager.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>DataPager.Api.Control.Events</code> | ✅ 一致 |
| <code>DataPager.Api.Control.FindControl</code> | ✅ 一致 |
| <code>DataPager.Api.Control.Focus</code> | ✅ 一致 |
| <code>DataPager.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>DataPager.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>DataPager.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>DataPager.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>DataPager.Api.Control.HasControls</code> | ✅ 一致 |
| <code>DataPager.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>DataPager.Api.Control.ID</code> | ✅ 一致 |
| <code>DataPager.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>DataPager.Api.Control.Init</code> | ✅ 一致 |
| <code>DataPager.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>DataPager.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>DataPager.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>DataPager.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>DataPager.Api.Control.Load</code> | ✅ 一致 |
| <code>DataPager.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>DataPager.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>DataPager.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>DataPager.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>DataPager.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>DataPager.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>DataPager.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>DataPager.Api.Control.OnInit</code> | ✅ 一致 |
| <code>DataPager.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>DataPager.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>DataPager.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>DataPager.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>DataPager.Api.Control.Page</code> | ✅ 一致 |
| <code>DataPager.Api.Control.Parent</code> | ✅ 一致 |
| <code>DataPager.Api.Control.PreRender</code> | ✅ 一致 |
| <code>DataPager.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>DataPager.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>DataPager.Api.Control.Render</code> | ✅ 一致 |
| <code>DataPager.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>DataPager.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>DataPager.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>DataPager.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>DataPager.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>DataPager.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>DataPager.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>DataPager.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>DataPager.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>DataPager.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>DataPager.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>DataPager.Api.Control.Site</code> | ✅ 一致 |
| <code>DataPager.Api.Control.SkinID</code> | ✅ 一致 |
| <code>DataPager.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>DataPager.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>DataPager.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>DataPager.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>DataPager.Api.Control.Unload</code> | ✅ 一致 |
| <code>DataPager.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>DataPager.Api.Control.ViewState</code> | ✅ 一致 |
| <code>DataPager.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>DataPager.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>DataPager.Api.Control.Visible</code> | ✅ 一致 |
| <code>DataPager.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>DataPager.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>DataPager.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>DataPager.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>DataPager.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>DataPager.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>DataPager.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>DataPager.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>DataPager.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>DataPager.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>DataPager.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>DataPager.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>DataPager.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.#ctor</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.AddAttributesToRender</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.Attributes</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.ConnectToEvents</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.Controls</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.CreatePagerFields</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.DataBind</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.Fields</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.FindPageableItemContainer</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.LoadControlState</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.LoadViewState</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.MaximumRows</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.OnBubbleEvent</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.OnInit</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.OnLoad</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.OnTotalRowCountAvailable</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.PageSize</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.PagedControlID</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.QueryStringField</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.RecreateChildControls</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.Render</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.RenderBeginTag</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.RenderContents</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.SaveControlState</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.SaveViewState</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.SetPageProperties</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.StartRowIndex</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.TagKey</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.TotalRowCount</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.DataPager.TrackViewState</code> | ✅ 一致 |
| <code>DataPager.Api.WebControls.ICompositeControlDesignerAccessor.RecreateChildControls</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>DataPager.(case list)</code> | ✅ 一致 |
| <code>DataPager.(default)</code> | ✅ 一致 |
| <code>DataPager.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>DataPager.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>DataPager.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>DataPager.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>DataPager.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>DataPager.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>DataPager.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>DataPager.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>DataPager.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>DataPager.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>DataPager.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>DataPager.ClientIDMode=Static</code> | ✅ 一致 |
| <code>DataPager.EnableTheming=false</code> | ✅ 一致 |
| <code>DataPager.EnableTheming=true</code> | ✅ 一致 |
| <code>DataPager.EnableViewState=false</code> | ✅ 一致 |
| <code>DataPager.EnableViewState=true</code> | ✅ 一致 |
| <code>DataPager.ID=empty</code> | ✅ 一致 |
| <code>DataPager.ID=japanese</code> | ✅ 一致 |
| <code>DataPager.ID=markup</code> | ✅ 一致 |
| <code>DataPager.ID=text</code> | ✅ 一致 |
| <code>DataPager.PageSize=-1</code> | ✅ 一致 |
| <code>DataPager.PageSize=0</code> | ✅ 一致 |
| <code>DataPager.PageSize=3</code> | ✅ 一致 |
| <code>DataPager.PageSize=3+EnableViewState=false</code> | ✅ 一致 |
| <code>DataPager.PageSize=3+Visible=false</code> | ✅ 一致 |
| <code>DataPager.PagedControlID=empty</code> | ✅ 一致 |
| <code>DataPager.PagedControlID=japanese</code> | ✅ 一致 |
| <code>DataPager.PagedControlID=list</code> | ✅ 一致 |
| <code>DataPager.PagedControlID=markup</code> | ✅ 一致 |
| <code>DataPager.PagedControlID=text</code> | ✅ 一致 |
| <code>DataPager.QueryStringField=empty</code> | ✅ 一致 |
| <code>DataPager.QueryStringField=japanese</code> | ✅ 一致 |
| <code>DataPager.QueryStringField=markup</code> | ✅ 一致 |
| <code>DataPager.QueryStringField=text</code> | ✅ 一致 |
| <code>DataPager.QueryStringField=text+PageSize=3</code> | ✅ 一致 |
| <code>DataPager.SkinID=empty</code> | ✅ 一致 |
| <code>DataPager.SkinID=japanese</code> | ✅ 一致 |
| <code>DataPager.SkinID=markup</code> | ✅ 一致 |
| <code>DataPager.SkinID=text</code> | ✅ 一致 |
| <code>DataPager.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>DataPager.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>DataPager.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>DataPager.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>DataPager.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>DataPager.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>DataPager.Visible=false</code> | ✅ 一致 |
| <code>DataPager.Visible=true</code> | ✅ 一致 |
| <code>DataPager[naming container].(default)</code> | ✅ 一致 |
| <code>DataPager[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>DataPager[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>DataPager[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>DataPager[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>DataPager[naming container].ID=text</code> | ✅ 一致 |
| <code>DataPager[next-previous].(default)</code> | ✅ 一致 |
| <code>DataPager[next-previous].PageSize=3</code> | ✅ 一致 |
| <code>DataPager[template].(default)</code> | ✅ 一致 |
| <code>DataPager[themed].(default)</code> | ✅ 一致 |
| <code>DataPager[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>DataPager[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| DataPager [render][databind][dynamic] first request | ✅ 一致 |
| DataPager [paging] a numeric pager field: page 3 | ✅ 一致 |
| DataPager [paging] a next-previous field: the first page | ✅ 一致 |
| DataPager [paging] a next-previous field: the last page | ✅ 一致 |
| DataPager [paging] the data source&#39;s list view: the last page | ✅ 一致 |
| DataPager [paging][viewstate] the previous page | ✅ 一致 |
| DataPager [dynamic][paging] a dynamic pager: page 2 | ✅ 一致 |
| DataPager [viewstate] posted: the pages kept | ✅ 一致 |

</details>
