# Repeater パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-16 16:41
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-10, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 150 | 150 | 0 | 0 |
| ② HTML | 58 | 58 | 0 | 0 |
| ③ シナリオ | 7 | 7 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 157 件中 157 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 13 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 7 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 45 | 1 | ✅ |
| ViewState の保存と復元 | 7 | 2 | ✅ |
| Visible=false（描画されず状態は保持） | 3 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 2 | ✅ |
| 行・項目のコマンド（RowCommand・ItemCommand へのバブル） | 0 | 3 | ✅ |
| データソースコントロールとの連携（DataSourceID） | 0 | 1 | ✅ |
| テンプレート | 0 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>Repeater.Api.Control.#ctor</code> | ✅ 一致 |
| <code>Repeater.Api.Control.Adapter</code> | ✅ 一致 |
| <code>Repeater.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>Repeater.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>Repeater.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>Repeater.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>Repeater.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>Repeater.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>Repeater.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>Repeater.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>Repeater.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>Repeater.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>Repeater.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>Repeater.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>Repeater.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>Repeater.Api.Control.ClientID</code> | ✅ 一致 |
| <code>Repeater.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>Repeater.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>Repeater.Api.Control.Context</code> | ✅ 一致 |
| <code>Repeater.Api.Control.Controls</code> | ✅ 一致 |
| <code>Repeater.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>Repeater.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>Repeater.Api.Control.DataBind</code> | ✅ 一致 |
| <code>Repeater.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>Repeater.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>Repeater.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>Repeater.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>Repeater.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>Repeater.Api.Control.Dispose</code> | ✅ 一致 |
| <code>Repeater.Api.Control.Disposed</code> | ✅ 一致 |
| <code>Repeater.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>Repeater.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>Repeater.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>Repeater.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>Repeater.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>Repeater.Api.Control.Events</code> | ✅ 一致 |
| <code>Repeater.Api.Control.FindControl</code> | ✅ 一致 |
| <code>Repeater.Api.Control.Focus</code> | ✅ 一致 |
| <code>Repeater.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>Repeater.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>Repeater.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>Repeater.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>Repeater.Api.Control.HasControls</code> | ✅ 一致 |
| <code>Repeater.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>Repeater.Api.Control.ID</code> | ✅ 一致 |
| <code>Repeater.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>Repeater.Api.Control.Init</code> | ✅ 一致 |
| <code>Repeater.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>Repeater.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>Repeater.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>Repeater.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>Repeater.Api.Control.Load</code> | ✅ 一致 |
| <code>Repeater.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>Repeater.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>Repeater.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>Repeater.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>Repeater.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>Repeater.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>Repeater.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>Repeater.Api.Control.OnInit</code> | ✅ 一致 |
| <code>Repeater.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>Repeater.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>Repeater.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>Repeater.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>Repeater.Api.Control.Page</code> | ✅ 一致 |
| <code>Repeater.Api.Control.Parent</code> | ✅ 一致 |
| <code>Repeater.Api.Control.PreRender</code> | ✅ 一致 |
| <code>Repeater.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>Repeater.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>Repeater.Api.Control.Render</code> | ✅ 一致 |
| <code>Repeater.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>Repeater.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>Repeater.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>Repeater.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>Repeater.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>Repeater.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>Repeater.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>Repeater.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>Repeater.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>Repeater.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>Repeater.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>Repeater.Api.Control.Site</code> | ✅ 一致 |
| <code>Repeater.Api.Control.SkinID</code> | ✅ 一致 |
| <code>Repeater.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>Repeater.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>Repeater.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>Repeater.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>Repeater.Api.Control.Unload</code> | ✅ 一致 |
| <code>Repeater.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>Repeater.Api.Control.ViewState</code> | ✅ 一致 |
| <code>Repeater.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>Repeater.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>Repeater.Api.Control.Visible</code> | ✅ 一致 |
| <code>Repeater.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>Repeater.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>Repeater.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>Repeater.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>Repeater.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>Repeater.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>Repeater.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>Repeater.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>Repeater.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>Repeater.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>Repeater.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.#ctor</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.AlternatingItemTemplate</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.CallingDataMethods</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.Controls</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.CreateChildControls</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.CreateControlHierarchy</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.CreateDataSourceSelectArguments</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.CreateItem</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.CreatingModelDataSource</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.DataBind</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.DataMember</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.DataSource</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.DataSourceID</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.EnableTheming</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.EnsureDataBound</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.FooterTemplate</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.GetData</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.HeaderTemplate</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.InitializeItem</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.Initialized</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.IsBoundUsingDataSourceID</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.IsDataBindingAutomatic</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.ItemCommand</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.ItemCreated</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.ItemDataBound</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.ItemTemplate</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.ItemType</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.Items</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.LoadViewState</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.OnBubbleEvent</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.OnCreatingModelDataSource</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.OnDataBinding</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.OnDataSourceViewChanged</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.OnInit</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.OnItemCommand</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.OnItemCreated</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.OnItemDataBound</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.OnLoad</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.OnPreRender</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.RequiresDataBinding</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.SaveViewState</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.SelectArguments</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.SelectMethod</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.SeparatorTemplate</code> | ✅ 一致 |
| <code>Repeater.Api.WebControls.Repeater.TrackViewState</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>Repeater.(case list)</code> | ✅ 一致 |
| <code>Repeater.(default)</code> | ✅ 一致 |
| <code>Repeater.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>Repeater.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>Repeater.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>Repeater.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>Repeater.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Repeater.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Repeater.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Repeater.ClientIDMode=Static</code> | ✅ 一致 |
| <code>Repeater.DataMember=empty</code> | ✅ 一致 |
| <code>Repeater.DataMember=japanese</code> | ✅ 一致 |
| <code>Repeater.DataMember=markup</code> | ✅ 一致 |
| <code>Repeater.DataMember=text</code> | ✅ 一致 |
| <code>Repeater.DataSourceID=empty</code> | ✅ 一致 |
| <code>Repeater.DataSourceID=japanese</code> | ✅ 一致 |
| <code>Repeater.DataSourceID=markup</code> | ✅ 一致 |
| <code>Repeater.DataSourceID=text</code> | ✅ 一致 |
| <code>Repeater.EnableTheming=false</code> | ✅ 一致 |
| <code>Repeater.EnableTheming=true</code> | ✅ 一致 |
| <code>Repeater.EnableViewState=false</code> | ✅ 一致 |
| <code>Repeater.EnableViewState=false+Visible=false</code> | ✅ 一致 |
| <code>Repeater.EnableViewState=true</code> | ✅ 一致 |
| <code>Repeater.ID=empty</code> | ✅ 一致 |
| <code>Repeater.ID=japanese</code> | ✅ 一致 |
| <code>Repeater.ID=markup</code> | ✅ 一致 |
| <code>Repeater.ID=text</code> | ✅ 一致 |
| <code>Repeater.ItemType=empty</code> | ✅ 一致 |
| <code>Repeater.ItemType=japanese</code> | ✅ 一致 |
| <code>Repeater.ItemType=markup</code> | ✅ 一致 |
| <code>Repeater.ItemType=text</code> | ✅ 一致 |
| <code>Repeater.SelectMethod=empty</code> | ✅ 一致 |
| <code>Repeater.SelectMethod=japanese</code> | ✅ 一致 |
| <code>Repeater.SelectMethod=markup</code> | ✅ 一致 |
| <code>Repeater.SelectMethod=text</code> | ✅ 一致 |
| <code>Repeater.SkinID=empty</code> | ✅ 一致 |
| <code>Repeater.SkinID=japanese</code> | ✅ 一致 |
| <code>Repeater.SkinID=markup</code> | ✅ 一致 |
| <code>Repeater.SkinID=text</code> | ✅ 一致 |
| <code>Repeater.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>Repeater.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>Repeater.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>Repeater.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>Repeater.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>Repeater.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>Repeater.Visible=false</code> | ✅ 一致 |
| <code>Repeater.Visible=true</code> | ✅ 一致 |
| <code>Repeater[empty].(default)</code> | ✅ 一致 |
| <code>Repeater[naming container].(default)</code> | ✅ 一致 |
| <code>Repeater[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>Repeater[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>Repeater[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>Repeater[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>Repeater[naming container].ID=text</code> | ✅ 一致 |
| <code>Repeater[no template].(default)</code> | ✅ 一致 |
| <code>Repeater[templates].(default)</code> | ✅ 一致 |
| <code>Repeater[templates].EnableViewState=false</code> | ✅ 一致 |
| <code>Repeater[templates].Visible=false</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| Repeater [render][databind][templates][datasourceid][dynamic] first request | ✅ 一致 |
| Repeater [viewstate][dynamic] posted: items made again from view state, the text box&#39;s value | ✅ 一致 |
| Repeater [rowcommand] a link&#39;s command bubbled to ItemCommand; rebound | ✅ 一致 |
| Repeater [rowcommand] a button&#39;s command bubbled to ItemCommand | ✅ 一致 |
| Repeater [rowcommand] a nested repeater&#39;s command | ✅ 一致 |
| Repeater [databind] rebound | ✅ 一致 |
| Repeater [visible][viewstate] the hidden repeater shown with its items | ✅ 一致 |

</details>
