# TreeView パリティ結果（Windows）

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-16 16:44
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Windows）
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 252 | 252 | 0 | 0 |
| ② HTML | 294 | 294 | 0 | 0 |
| ③ シナリオ | 12 | 12 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 261 件中 261 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 79 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 12 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 229 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 6 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 8 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 3 | 0 | ✅ |
| ポストバックデータの取り込み | 0 | 1 | ✅ |
| 変更イベント（*Changed、ServerChange） | 0 | 2 | ✅ |
| リクエスト検証（ValidateRequestMode） | 3 | 0 | ✅ |
| ポストバックイベント | 0 | 4 | ✅ |
| 双方向バインド（テンプレート内の Bind）・ControlParameter（ControlValueProperty の値） | 0 | 1 | ✅ |
| ブラウザ側の検証スクリプト（EnableClientScript） | 4 | 0 | ✅ |
| データソース連携（DataSource・DataTextField など） | 5 | 0 | ✅ |
| クライアントコールバック | 0 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>TreeView.Api.Control.#ctor</code> | ✅ 一致 |
| <code>TreeView.Api.Control.Adapter</code> | ✅ 一致 |
| <code>TreeView.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>TreeView.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>TreeView.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>TreeView.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>TreeView.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>TreeView.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>TreeView.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>TreeView.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>TreeView.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>TreeView.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>TreeView.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>TreeView.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>TreeView.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>TreeView.Api.Control.ClientID</code> | ✅ 一致 |
| <code>TreeView.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>TreeView.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>TreeView.Api.Control.Context</code> | ✅ 一致 |
| <code>TreeView.Api.Control.Controls</code> | ✅ 一致 |
| <code>TreeView.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>TreeView.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>TreeView.Api.Control.DataBind</code> | ✅ 一致 |
| <code>TreeView.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>TreeView.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>TreeView.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>TreeView.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>TreeView.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>TreeView.Api.Control.Dispose</code> | ✅ 一致 |
| <code>TreeView.Api.Control.Disposed</code> | ✅ 一致 |
| <code>TreeView.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>TreeView.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>TreeView.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>TreeView.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>TreeView.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>TreeView.Api.Control.Events</code> | ✅ 一致 |
| <code>TreeView.Api.Control.FindControl</code> | ✅ 一致 |
| <code>TreeView.Api.Control.Focus</code> | ✅ 一致 |
| <code>TreeView.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>TreeView.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>TreeView.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>TreeView.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>TreeView.Api.Control.HasControls</code> | ✅ 一致 |
| <code>TreeView.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>TreeView.Api.Control.ID</code> | ✅ 一致 |
| <code>TreeView.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>TreeView.Api.Control.Init</code> | ✅ 一致 |
| <code>TreeView.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>TreeView.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>TreeView.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>TreeView.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>TreeView.Api.Control.Load</code> | ✅ 一致 |
| <code>TreeView.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>TreeView.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>TreeView.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>TreeView.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>TreeView.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>TreeView.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>TreeView.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>TreeView.Api.Control.OnInit</code> | ✅ 一致 |
| <code>TreeView.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>TreeView.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>TreeView.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>TreeView.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>TreeView.Api.Control.Page</code> | ✅ 一致 |
| <code>TreeView.Api.Control.Parent</code> | ✅ 一致 |
| <code>TreeView.Api.Control.PreRender</code> | ✅ 一致 |
| <code>TreeView.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>TreeView.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>TreeView.Api.Control.Render</code> | ✅ 一致 |
| <code>TreeView.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>TreeView.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>TreeView.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>TreeView.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>TreeView.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>TreeView.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>TreeView.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>TreeView.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>TreeView.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>TreeView.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>TreeView.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>TreeView.Api.Control.Site</code> | ✅ 一致 |
| <code>TreeView.Api.Control.SkinID</code> | ✅ 一致 |
| <code>TreeView.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>TreeView.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>TreeView.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>TreeView.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>TreeView.Api.Control.Unload</code> | ✅ 一致 |
| <code>TreeView.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>TreeView.Api.Control.ViewState</code> | ✅ 一致 |
| <code>TreeView.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>TreeView.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>TreeView.Api.Control.Visible</code> | ✅ 一致 |
| <code>TreeView.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>TreeView.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>TreeView.Api.ICallbackEventHandler.GetCallbackResult</code> | ✅ 一致 |
| <code>TreeView.Api.ICallbackEventHandler.RaiseCallbackEvent</code> | ✅ 一致 |
| <code>TreeView.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>TreeView.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>TreeView.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>TreeView.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>TreeView.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>TreeView.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>TreeView.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>TreeView.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>TreeView.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>TreeView.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>TreeView.Api.IPostBackDataHandler.LoadPostData</code> | ✅ 一致 |
| <code>TreeView.Api.IPostBackDataHandler.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>TreeView.Api.IPostBackEventHandler.RaisePostBackEvent</code> | ✅ 一致 |
| <code>TreeView.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.BaseDataBoundControl.#ctor</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.BaseDataBoundControl.ConfirmInitState</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.BaseDataBoundControl.DataBind</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.BaseDataBoundControl.DataBound</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.BaseDataBoundControl.DataSource</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.BaseDataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.BaseDataBoundControl.EnsureDataBound</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.BaseDataBoundControl.Initialized</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.BaseDataBoundControl.IsBoundUsingDataSourceID</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.BaseDataBoundControl.IsDataBindingAutomatic</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.BaseDataBoundControl.IsUsingModelBinders</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.BaseDataBoundControl.OnDataBound</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.BaseDataBoundControl.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.BaseDataBoundControl.OnInit</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.BaseDataBoundControl.OnPagePreLoad</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.BaseDataBoundControl.OnPreRender</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.BaseDataBoundControl.PerformSelect</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.BaseDataBoundControl.RequiresDataBinding</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.BaseDataBoundControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.BaseDataBoundControl.ValidateDataSource</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.HierarchicalDataBoundControl.#ctor</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.HierarchicalDataBoundControl.DataSourceID</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.HierarchicalDataBoundControl.GetData</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.HierarchicalDataBoundControl.GetDataSource</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.HierarchicalDataBoundControl.MarkAsDataBound</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.HierarchicalDataBoundControl.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.HierarchicalDataBoundControl.OnDataSourceChanged</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.HierarchicalDataBoundControl.OnLoad</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.HierarchicalDataBoundControl.OnPagePreLoad</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.HierarchicalDataBoundControl.PerformDataBinding</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.HierarchicalDataBoundControl.PerformSelect</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.HierarchicalDataBoundControl.ValidateDataSource</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.#ctor</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.AddAttributesToRender</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.AutoGenerateDataBindings</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.CheckedNodes</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.CollapseAll</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.CollapseImageToolTip</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.CollapseImageUrl</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.CreateControlCollection</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.CreateNode</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.DataBind</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.DataBindings</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.EnableClientScript</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.ExpandAll</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.ExpandDepth</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.ExpandImageToolTip</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.ExpandImageUrl</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.FindNode</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.GetCallbackResult</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.HoverNodeStyle</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.ImageSet</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.LeafNodeStyle</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.LevelStyles</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.LineImagesFolder</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.LoadPostData</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.LoadViewState</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.MaxDataBindDepth</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.NoExpandImageUrl</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.NodeIndent</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.NodeStyle</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.NodeWrap</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.Nodes</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.OnInit</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.OnPreRender</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.OnSelectedNodeChanged</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.OnTreeNodeCheckChanged</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.OnTreeNodeCollapsed</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.OnTreeNodeDataBound</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.OnTreeNodeExpanded</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.OnTreeNodePopulate</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.ParentNodeStyle</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.PathSeparator</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.PerformDataBinding</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.PopulateNodesFromClient</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.RaiseCallbackEvent</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.RaisePostBackEvent</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.RenderBeginTag</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.RenderContents</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.RenderEndTag</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.RootNodeStyle</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.SaveViewState</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.SelectedNode</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.SelectedNodeChanged</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.SelectedNodeStyle</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.SelectedValue</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.SetNodeDataBound</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.SetNodeDataItem</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.SetNodeDataPath</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.ShowCheckBoxes</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.ShowExpandCollapse</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.ShowLines</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.SkipLinkText</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.TagKey</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.Target</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.TrackViewState</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.TreeNodeCheckChanged</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.TreeNodeCollapsed</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.TreeNodeDataBound</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.TreeNodeExpanded</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.TreeNodePopulate</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.TreeView.Visible</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>TreeView.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>TreeView.(case list)</code> | ✅ 一致 |
| <code>TreeView.(default)</code> | ✅ 一致 |
| <code>TreeView.AccessKey=empty</code> | ✅ 一致 |
| <code>TreeView.AccessKey=japanese</code> | ✅ 一致 |
| <code>TreeView.AccessKey=k</code> | ✅ 一致 |
| <code>TreeView.AccessKey=markup</code> | ✅ 一致 |
| <code>TreeView.AccessKey=text</code> | ✅ 一致 |
| <code>TreeView.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>TreeView.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>TreeView.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>TreeView.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>TreeView.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>TreeView.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>TreeView.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>TreeView.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>TreeView.AutoGenerateDataBindings=false</code> | ✅ 一致 |
| <code>TreeView.AutoGenerateDataBindings=true</code> | ✅ 一致 |
| <code>TreeView.BackColor=#3366CC</code> | ✅ 一致 |
| <code>TreeView.BackColor=Red</code> | ✅ 一致 |
| <code>TreeView.BackColor=Transparent</code> | ✅ 一致 |
| <code>TreeView.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>TreeView.BorderColor=Red</code> | ✅ 一致 |
| <code>TreeView.BorderColor=Transparent</code> | ✅ 一致 |
| <code>TreeView.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>TreeView.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>TreeView.BorderStyle=Double</code> | ✅ 一致 |
| <code>TreeView.BorderStyle=Groove</code> | ✅ 一致 |
| <code>TreeView.BorderStyle=Inset</code> | ✅ 一致 |
| <code>TreeView.BorderStyle=None</code> | ✅ 一致 |
| <code>TreeView.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>TreeView.BorderStyle=Outset</code> | ✅ 一致 |
| <code>TreeView.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>TreeView.BorderStyle=Solid</code> | ✅ 一致 |
| <code>TreeView.BorderWidth=120px</code> | ✅ 一致 |
| <code>TreeView.BorderWidth=2em</code> | ✅ 一致 |
| <code>TreeView.BorderWidth=50%</code> | ✅ 一致 |
| <code>TreeView.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>TreeView.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>TreeView.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>TreeView.ClientIDMode=Static</code> | ✅ 一致 |
| <code>TreeView.CollapseImageToolTip=empty</code> | ✅ 一致 |
| <code>TreeView.CollapseImageToolTip=japanese</code> | ✅ 一致 |
| <code>TreeView.CollapseImageToolTip=markup</code> | ✅ 一致 |
| <code>TreeView.CollapseImageToolTip=text</code> | ✅ 一致 |
| <code>TreeView.CollapseImageUrl=app-relative</code> | ✅ 一致 |
| <code>TreeView.CollapseImageUrl=empty</code> | ✅ 一致 |
| <code>TreeView.CollapseImageUrl=japanese</code> | ✅ 一致 |
| <code>TreeView.CollapseImageUrl=markup</code> | ✅ 一致 |
| <code>TreeView.CollapseImageUrl=text</code> | ✅ 一致 |
| <code>TreeView.CssClass=empty</code> | ✅ 一致 |
| <code>TreeView.CssClass=japanese</code> | ✅ 一致 |
| <code>TreeView.CssClass=markup</code> | ✅ 一致 |
| <code>TreeView.CssClass=text</code> | ✅ 一致 |
| <code>TreeView.DataSourceID=empty</code> | ✅ 一致 |
| <code>TreeView.DataSourceID=japanese</code> | ✅ 一致 |
| <code>TreeView.DataSourceID=markup</code> | ✅ 一致 |
| <code>TreeView.DataSourceID=text</code> | ✅ 一致 |
| <code>TreeView.EnableClientScript=false</code> | ✅ 一致 |
| <code>TreeView.EnableClientScript=false+ExpandDepth=0</code> | ✅ 一致 |
| <code>TreeView.EnableClientScript=true</code> | ✅ 一致 |
| <code>TreeView.EnableTheming=false</code> | ✅ 一致 |
| <code>TreeView.EnableTheming=true</code> | ✅ 一致 |
| <code>TreeView.EnableViewState=false</code> | ✅ 一致 |
| <code>TreeView.EnableViewState=true</code> | ✅ 一致 |
| <code>TreeView.Enabled=false</code> | ✅ 一致 |
| <code>TreeView.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>TreeView.Enabled=false+ExpandDepth=1</code> | ✅ 一致 |
| <code>TreeView.Enabled=true</code> | ✅ 一致 |
| <code>TreeView.ExpandDepth=-1</code> | ✅ 一致 |
| <code>TreeView.ExpandDepth=0</code> | ✅ 一致 |
| <code>TreeView.ExpandDepth=1</code> | ✅ 一致 |
| <code>TreeView.ExpandDepth=1+EnableViewState=false</code> | ✅ 一致 |
| <code>TreeView.ExpandDepth=1+Visible=false</code> | ✅ 一致 |
| <code>TreeView.ExpandDepth=3</code> | ✅ 一致 |
| <code>TreeView.ExpandImageToolTip=empty</code> | ✅ 一致 |
| <code>TreeView.ExpandImageToolTip=japanese</code> | ✅ 一致 |
| <code>TreeView.ExpandImageToolTip=markup</code> | ✅ 一致 |
| <code>TreeView.ExpandImageToolTip=text</code> | ✅ 一致 |
| <code>TreeView.ExpandImageUrl=app-relative</code> | ✅ 一致 |
| <code>TreeView.ExpandImageUrl=empty</code> | ✅ 一致 |
| <code>TreeView.ExpandImageUrl=japanese</code> | ✅ 一致 |
| <code>TreeView.ExpandImageUrl=markup</code> | ✅ 一致 |
| <code>TreeView.ExpandImageUrl=text</code> | ✅ 一致 |
| <code>TreeView.Font.Bold=false</code> | ✅ 一致 |
| <code>TreeView.Font.Bold=true</code> | ✅ 一致 |
| <code>TreeView.Font.Italic=false</code> | ✅ 一致 |
| <code>TreeView.Font.Italic=true</code> | ✅ 一致 |
| <code>TreeView.Font.Name=empty</code> | ✅ 一致 |
| <code>TreeView.Font.Name=japanese</code> | ✅ 一致 |
| <code>TreeView.Font.Name=markup</code> | ✅ 一致 |
| <code>TreeView.Font.Name=text</code> | ✅ 一致 |
| <code>TreeView.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>TreeView.Font.Overline=false</code> | ✅ 一致 |
| <code>TreeView.Font.Overline=true</code> | ✅ 一致 |
| <code>TreeView.Font.Size=12pt</code> | ✅ 一致 |
| <code>TreeView.Font.Size=150%</code> | ✅ 一致 |
| <code>TreeView.Font.Size=Large</code> | ✅ 一致 |
| <code>TreeView.Font.Strikeout=false</code> | ✅ 一致 |
| <code>TreeView.Font.Strikeout=true</code> | ✅ 一致 |
| <code>TreeView.Font.Underline=false</code> | ✅ 一致 |
| <code>TreeView.Font.Underline=true</code> | ✅ 一致 |
| <code>TreeView.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>TreeView.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>TreeView.ForeColor=Red</code> | ✅ 一致 |
| <code>TreeView.ForeColor=Transparent</code> | ✅ 一致 |
| <code>TreeView.Height=120px</code> | ✅ 一致 |
| <code>TreeView.Height=2em</code> | ✅ 一致 |
| <code>TreeView.Height=50%</code> | ✅ 一致 |
| <code>TreeView.HoverNodeStyle.BackColor=Red</code> | ✅ 一致 |
| <code>TreeView.HoverNodeStyle.CssClass=text</code> | ✅ 一致 |
| <code>TreeView.HoverNodeStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>TreeView.HoverNodeStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>TreeView.HoverNodeStyle.Height=120px</code> | ✅ 一致 |
| <code>TreeView.ID=empty</code> | ✅ 一致 |
| <code>TreeView.ID=japanese</code> | ✅ 一致 |
| <code>TreeView.ID=markup</code> | ✅ 一致 |
| <code>TreeView.ID=text</code> | ✅ 一致 |
| <code>TreeView.ImageSet=Arrows</code> | ✅ 一致 |
| <code>TreeView.ImageSet=BulletedList</code> | ✅ 一致 |
| <code>TreeView.ImageSet=BulletedList2</code> | ✅ 一致 |
| <code>TreeView.ImageSet=BulletedList3</code> | ✅ 一致 |
| <code>TreeView.ImageSet=BulletedList4</code> | ✅ 一致 |
| <code>TreeView.ImageSet=Contacts</code> | ✅ 一致 |
| <code>TreeView.ImageSet=Custom</code> | ✅ 一致 |
| <code>TreeView.ImageSet=Events</code> | ✅ 一致 |
| <code>TreeView.ImageSet=Faq</code> | ✅ 一致 |
| <code>TreeView.ImageSet=Inbox</code> | ✅ 一致 |
| <code>TreeView.ImageSet=Msdn</code> | ✅ 一致 |
| <code>TreeView.ImageSet=News</code> | ✅ 一致 |
| <code>TreeView.ImageSet=Simple</code> | ✅ 一致 |
| <code>TreeView.ImageSet=Simple2</code> | ✅ 一致 |
| <code>TreeView.ImageSet=WindowsHelp</code> | ✅ 一致 |
| <code>TreeView.ImageSet=XPFileExplorer</code> | ✅ 一致 |
| <code>TreeView.ImageSet=XPFileExplorer+ShowLines=true</code> | ✅ 一致 |
| <code>TreeView.LeafNodeStyle.BackColor=Red</code> | ✅ 一致 |
| <code>TreeView.LeafNodeStyle.CssClass=text</code> | ✅ 一致 |
| <code>TreeView.LeafNodeStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>TreeView.LeafNodeStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>TreeView.LeafNodeStyle.Height=120px</code> | ✅ 一致 |
| <code>TreeView.LineImagesFolder=app-relative</code> | ✅ 一致 |
| <code>TreeView.LineImagesFolder=empty</code> | ✅ 一致 |
| <code>TreeView.LineImagesFolder=japanese</code> | ✅ 一致 |
| <code>TreeView.LineImagesFolder=markup</code> | ✅ 一致 |
| <code>TreeView.LineImagesFolder=text</code> | ✅ 一致 |
| <code>TreeView.MaxDataBindDepth=-1</code> | ✅ 一致 |
| <code>TreeView.MaxDataBindDepth=0</code> | ✅ 一致 |
| <code>TreeView.MaxDataBindDepth=3</code> | ✅ 一致 |
| <code>TreeView.NoExpandImageUrl=app-relative</code> | ✅ 一致 |
| <code>TreeView.NoExpandImageUrl=empty</code> | ✅ 一致 |
| <code>TreeView.NoExpandImageUrl=japanese</code> | ✅ 一致 |
| <code>TreeView.NoExpandImageUrl=markup</code> | ✅ 一致 |
| <code>TreeView.NoExpandImageUrl=text</code> | ✅ 一致 |
| <code>TreeView.NodeIndent=-1</code> | ✅ 一致 |
| <code>TreeView.NodeIndent=0</code> | ✅ 一致 |
| <code>TreeView.NodeIndent=3</code> | ✅ 一致 |
| <code>TreeView.NodeIndent=3+NodeWrap=true</code> | ✅ 一致 |
| <code>TreeView.NodeStyle.BackColor=Red</code> | ✅ 一致 |
| <code>TreeView.NodeStyle.CssClass=text</code> | ✅ 一致 |
| <code>TreeView.NodeStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>TreeView.NodeStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>TreeView.NodeStyle.ForeColor=Red+HoverNodeStyle.BackColor=#3366CC</code> | ✅ 一致 |
| <code>TreeView.NodeStyle.Height=120px</code> | ✅ 一致 |
| <code>TreeView.NodeWrap=false</code> | ✅ 一致 |
| <code>TreeView.NodeWrap=true</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].Checked=false</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].Checked=true</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].ImageToolTip=empty</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].ImageToolTip=japanese</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].ImageToolTip=markup</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].ImageToolTip=text</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].ImageUrl=app-relative</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].ImageUrl=empty</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].ImageUrl=japanese</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].ImageUrl=markup</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].ImageUrl=text</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].NavigateUrl=app-relative</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].NavigateUrl=app-relative+Target=blank</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].NavigateUrl=empty</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].NavigateUrl=japanese</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].NavigateUrl=markup</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].NavigateUrl=text</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].PopulateOnDemand=false</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].PopulateOnDemand=true</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].PopulateOnDemand=true+EnableClientScript=false</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].PopulateOnDemand=true+PopulateNodesFromClient=true</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].SelectAction=Expand</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].SelectAction=Expand+ExpandDepth=0</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].SelectAction=None</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].SelectAction=None+Nodes[0].ToolTip=text</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].SelectAction=Select</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].SelectAction=SelectExpand</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].Selected=false</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].Selected=true</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].Selected=true+SelectedNodeStyle.BackColor=Red</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].Target=empty</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].Target=japanese</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].Target=markup</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].Target=text</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].Text=empty</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].Text=japanese</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].Text=markup</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].Text=text</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].ToolTip=empty</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].ToolTip=japanese</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].ToolTip=markup</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].ToolTip=text</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].Value=empty</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].Value=japanese</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].Value=markup</code> | ✅ 一致 |
| <code>TreeView.Nodes[0].Value=text</code> | ✅ 一致 |
| <code>TreeView.ParentNodeStyle.BackColor=Red</code> | ✅ 一致 |
| <code>TreeView.ParentNodeStyle.CssClass=text</code> | ✅ 一致 |
| <code>TreeView.ParentNodeStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>TreeView.ParentNodeStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>TreeView.ParentNodeStyle.Height=120px</code> | ✅ 一致 |
| <code>TreeView.PopulateNodesFromClient=false</code> | ✅ 一致 |
| <code>TreeView.PopulateNodesFromClient=true</code> | ✅ 一致 |
| <code>TreeView.RootNodeStyle.BackColor=Red</code> | ✅ 一致 |
| <code>TreeView.RootNodeStyle.BackColor=Red+LeafNodeStyle.BackColor=#3366CC</code> | ✅ 一致 |
| <code>TreeView.RootNodeStyle.CssClass=text</code> | ✅ 一致 |
| <code>TreeView.RootNodeStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>TreeView.RootNodeStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>TreeView.RootNodeStyle.Height=120px</code> | ✅ 一致 |
| <code>TreeView.SelectedNodeStyle.BackColor=Red</code> | ✅ 一致 |
| <code>TreeView.SelectedNodeStyle.CssClass=text</code> | ✅ 一致 |
| <code>TreeView.SelectedNodeStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>TreeView.SelectedNodeStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>TreeView.SelectedNodeStyle.Height=120px</code> | ✅ 一致 |
| <code>TreeView.ShowCheckBoxes=All</code> | ✅ 一致 |
| <code>TreeView.ShowCheckBoxes=All+Nodes[0].Checked=true</code> | ✅ 一致 |
| <code>TreeView.ShowCheckBoxes=Leaf</code> | ✅ 一致 |
| <code>TreeView.ShowCheckBoxes=Leaf+ExpandDepth=1</code> | ✅ 一致 |
| <code>TreeView.ShowCheckBoxes=None</code> | ✅ 一致 |
| <code>TreeView.ShowCheckBoxes=Parent</code> | ✅ 一致 |
| <code>TreeView.ShowCheckBoxes=Root</code> | ✅ 一致 |
| <code>TreeView.ShowExpandCollapse=false</code> | ✅ 一致 |
| <code>TreeView.ShowExpandCollapse=false+ExpandDepth=1</code> | ✅ 一致 |
| <code>TreeView.ShowExpandCollapse=true</code> | ✅ 一致 |
| <code>TreeView.ShowLines=false</code> | ✅ 一致 |
| <code>TreeView.ShowLines=true</code> | ✅ 一致 |
| <code>TreeView.ShowLines=true+ExpandDepth=1</code> | ✅ 一致 |
| <code>TreeView.SkinID=empty</code> | ✅ 一致 |
| <code>TreeView.SkinID=japanese</code> | ✅ 一致 |
| <code>TreeView.SkinID=markup</code> | ✅ 一致 |
| <code>TreeView.SkinID=text</code> | ✅ 一致 |
| <code>TreeView.SkipLinkText=empty</code> | ✅ 一致 |
| <code>TreeView.SkipLinkText=japanese</code> | ✅ 一致 |
| <code>TreeView.SkipLinkText=markup</code> | ✅ 一致 |
| <code>TreeView.SkipLinkText=text</code> | ✅ 一致 |
| <code>TreeView.Style[color]=1em</code> | ✅ 一致 |
| <code>TreeView.Style[color]=red</code> | ✅ 一致 |
| <code>TreeView.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>TreeView.Style[margin-left]=red</code> | ✅ 一致 |
| <code>TreeView.TabIndex=-1</code> | ✅ 一致 |
| <code>TreeView.TabIndex=0</code> | ✅ 一致 |
| <code>TreeView.TabIndex=3</code> | ✅ 一致 |
| <code>TreeView.Target=blank</code> | ✅ 一致 |
| <code>TreeView.Target=empty</code> | ✅ 一致 |
| <code>TreeView.Target=japanese</code> | ✅ 一致 |
| <code>TreeView.Target=markup</code> | ✅ 一致 |
| <code>TreeView.Target=text</code> | ✅ 一致 |
| <code>TreeView.ToolTip=empty</code> | ✅ 一致 |
| <code>TreeView.ToolTip=japanese</code> | ✅ 一致 |
| <code>TreeView.ToolTip=markup</code> | ✅ 一致 |
| <code>TreeView.ToolTip=text</code> | ✅ 一致 |
| <code>TreeView.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>TreeView.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>TreeView.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>TreeView.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>TreeView.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>TreeView.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>TreeView.Visible=false</code> | ✅ 一致 |
| <code>TreeView.Visible=true</code> | ✅ 一致 |
| <code>TreeView.Width=120px</code> | ✅ 一致 |
| <code>TreeView.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>TreeView.Width=2em</code> | ✅ 一致 |
| <code>TreeView.Width=50%</code> | ✅ 一致 |
| <code>TreeView[data-bound].(default)</code> | ✅ 一致 |
| <code>TreeView[data-bound].AutoGenerateDataBindings=false</code> | ✅ 一致 |
| <code>TreeView[data-bound].ExpandDepth=0</code> | ✅ 一致 |
| <code>TreeView[data-bound].MaxDataBindDepth=0</code> | ✅ 一致 |
| <code>TreeView[data-bound].ShowCheckBoxes=All+ExpandDepth=1</code> | ✅ 一致 |
| <code>TreeView[naming container].(default)</code> | ✅ 一致 |
| <code>TreeView[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>TreeView[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>TreeView[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>TreeView[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>TreeView[naming container].ExpandDepth=1</code> | ✅ 一致 |
| <code>TreeView[naming container].ID=text</code> | ✅ 一致 |
| <code>TreeView[naming container].ShowCheckBoxes=All</code> | ✅ 一致 |
| <code>TreeView[themed].(default)</code> | ✅ 一致 |
| <code>TreeView[themed].CssClass=text</code> | ✅ 一致 |
| <code>TreeView[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>TreeView[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| TreeView [render][databind][dynamic] first request: the site map as a path, trees and menus; bound to XML; templated; made in code | ✅ 一致 |
| TreeView [postbackevent][viewstate] a node expanded by a postback (EnableClientScript=false) | ✅ 一致 |
| TreeView [postbackevent][change][bind] a node selected (its value a control parameter&#39;s: ControlValueProperty) | ✅ 一致 |
| TreeView [postdata][change] a node&#39;s check box posted | ✅ 一致 |
| TreeView [postbackevent] a node populated on demand by a postback | ✅ 一致 |
| TreeView [callback] a node populated on demand by a client callback (no postback) | ✅ 一致 |
| TreeView [viewstate] a menu&#39;s item clicked | ✅ 一致 |
| TreeView [viewstate] a bound menu&#39;s item clicked | ✅ 一致 |
| TreeView [viewstate] a templated menu&#39;s item clicked | ✅ 一致 |
| TreeView [postbackevent][dynamic] a dynamic tree&#39;s node selected | ✅ 一致 |
| TreeView [viewstate] a dynamic menu&#39;s item clicked | ✅ 一致 |
| TreeView [visible][viewstate] hidden ones shown with what was added while hidden | ✅ 一致 |

</details>
