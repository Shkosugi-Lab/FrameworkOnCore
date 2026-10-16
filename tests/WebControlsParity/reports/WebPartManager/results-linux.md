# WebPartManager パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-15 15:05
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 219 | 215 | 4 | 0 |
| ② HTML | 53 | 53 | 0 | 0 |
| ③ シナリオ | 12 | 12 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 229 件中 229 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 13 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 12 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 45 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 10 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 1 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| ブラウザ側の検証スクリプト（EnableClientScript） | 3 | 0 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

| 層 | ケース | 行 | 理由 |
|---|---|---|---|
| api | <code>^WebPartManager\.Api\.WebControls\.WebParts\.WebPartManager\.(Create)?AvailableTransformers$</code> | <code>AvailableTransformers = &quot;\[System\.RuntimeType System\.Web\.UI\.WebControls\.WebParts\.Row(ToField&#124;ToParameters)Transformer, System\.RuntimeType System\.Web\.UI\.WebControls\.WebParts\.Row(ToField&#124;ToParameters)Transformer\]</code> | The same two transformers (the configuration&#39;s, RowToFieldTransformer and RowToParametersTransformer), listed in a Hashtable&#39;s order: by the types&#39; hash codes, which on .NET differ from .NET Framework&#39;s (and between runs). |
| api | <code>^WebPartManager\.Api\.WebControls\.WebParts\.WebPartManager\.(Minimal&#124;Medium)PermissionSet$</code> | <code>PermissionSet = </code> | The permission sets a web part manager gives imported web parts are code access security&#39;s: .NET has none (an empty set), .NET Framework&#39;s lists its permissions. Neither restricts what the parts&#39; code does on .NET. |

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>WebPartManager.Api.Control.#ctor</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.Adapter</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.ClientID</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.Context</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.Controls</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.DataBind</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.Dispose</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.Disposed</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.Events</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.FindControl</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.Focus</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.HasControls</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.ID</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.Init</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.Load</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.OnInit</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.Page</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.Parent</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.PreRender</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.Render</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.Site</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.SkinID</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.Unload</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.ViewState</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>WebPartManager.Api.Control.Visible</code> | ✅ 一致 |
| <code>WebPartManager.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>WebPartManager.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>WebPartManager.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>WebPartManager.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>WebPartManager.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>WebPartManager.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>WebPartManager.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>WebPartManager.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>WebPartManager.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>WebPartManager.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>WebPartManager.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.IPersonalizable.IsDirty</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.IPersonalizable.Load</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.IPersonalizable.Save</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.#ctor</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.ActivateConnections</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.AddWebPart</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.AuthorizeWebPart</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.AvailableTransformers</code> | ⚠️ 既知の差分 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.BeginWebPartConnecting</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.BeginWebPartEditing</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.BrowseDisplayMode</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.CanConnectWebParts</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.CatalogDisplayMode</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.CheckRenderClientScript</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.CloseProviderWarning</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.CloseWebPart</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.ConnectDisplayMode</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.ConnectWebParts</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.Connections</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.ConnectionsActivated</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.ConnectionsActivating</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.Controls</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.CopyWebPart</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.CreateAvailableTransformers</code> | ⚠️ 既知の差分 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.CreateControlCollection</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.CreateDisplayModes</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.CreateDynamicConnectionID</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.CreateDynamicWebPartID</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.CreateErrorWebPart</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.CreatePersonalization</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.CreateWebPart</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.DeleteWarning</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.DeleteWebPart</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.DesignDisplayMode</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.DisconnectWebPart</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.DisconnectWebParts</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.DisplayMode</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.DisplayModeChanged</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.DisplayModeChanging</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.DisplayModes</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.DynamicConnections</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.EditDisplayMode</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.EnableClientScript</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.EnableTheming</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.EndWebPartConnecting</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.EndWebPartEditing</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.ExportSensitiveDataWarning</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.ExportWebPart</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.Focus</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.GetConsumerConnectionPoints</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.GetCurrentWebPartManager</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.GetDisplayTitle</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.GetExportUrl</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.GetGenericWebPart</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.GetProviderConnectionPoints</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.ImportWebPart</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.Internals</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.IsAuthorized</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.IsCustomPersonalizationStateDirty</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.LoadControlState</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.LoadCustomPersonalizationState</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.MediumPermissionSet</code> | ⚠️ 既知の差分 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.MinimalPermissionSet</code> | ⚠️ 既知の差分 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.MoveWebPart</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.OnAuthorizeWebPart</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.OnConnectionsActivated</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.OnConnectionsActivating</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.OnDisplayModeChanged</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.OnDisplayModeChanging</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.OnInit</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.OnPreRender</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.OnSelectedWebPartChanged</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.OnSelectedWebPartChanging</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.OnUnload</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.OnWebPartAdded</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.OnWebPartAdding</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.OnWebPartClosed</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.OnWebPartClosing</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.OnWebPartDeleted</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.OnWebPartDeleting</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.OnWebPartMoved</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.OnWebPartMoving</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.OnWebPartsConnected</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.OnWebPartsConnecting</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.OnWebPartsDisconnected</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.OnWebPartsDisconnecting</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.Personalization</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.RegisterClientScript</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.Render</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.SaveControlState</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.SaveCustomPersonalizationState</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.SelectedWebPart</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.SelectedWebPartChanged</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.SelectedWebPartChanging</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.SetPersonalizationDirty</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.SetSelectedWebPart</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.SkinID</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.StaticConnections</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.SupportedDisplayModes</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.TrackViewState</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.Visible</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.WebPartAdded</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.WebPartAdding</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.WebPartClosed</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.WebPartClosing</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.WebPartDeleted</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.WebPartDeleting</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.WebPartMoved</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.WebPartMoving</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.WebParts</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.WebPartsConnected</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.WebPartsConnecting</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.WebPartsDisconnected</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.WebPartsDisconnecting</code> | ✅ 一致 |
| <code>WebPartManager.Api.WebControls.WebParts.WebPartManager.Zones</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>WebPartManager.(case list)</code> | ✅ 一致 |
| <code>WebPartManager.(default)</code> | ✅ 一致 |
| <code>WebPartManager.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>WebPartManager.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>WebPartManager.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>WebPartManager.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>WebPartManager.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>WebPartManager.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>WebPartManager.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>WebPartManager.ClientIDMode=Static</code> | ✅ 一致 |
| <code>WebPartManager.CloseProviderWarning=empty</code> | ✅ 一致 |
| <code>WebPartManager.CloseProviderWarning=japanese</code> | ✅ 一致 |
| <code>WebPartManager.CloseProviderWarning=markup</code> | ✅ 一致 |
| <code>WebPartManager.CloseProviderWarning=text</code> | ✅ 一致 |
| <code>WebPartManager.DeleteWarning=empty</code> | ✅ 一致 |
| <code>WebPartManager.DeleteWarning=japanese</code> | ✅ 一致 |
| <code>WebPartManager.DeleteWarning=markup</code> | ✅ 一致 |
| <code>WebPartManager.DeleteWarning=markup+ExportSensitiveDataWarning=text</code> | ✅ 一致 |
| <code>WebPartManager.DeleteWarning=text</code> | ✅ 一致 |
| <code>WebPartManager.EnableClientScript=false</code> | ✅ 一致 |
| <code>WebPartManager.EnableClientScript=false+CloseProviderWarning=markup</code> | ✅ 一致 |
| <code>WebPartManager.EnableClientScript=true</code> | ✅ 一致 |
| <code>WebPartManager.EnableTheming=false</code> | ✅ 一致 |
| <code>WebPartManager.EnableTheming=true</code> | ✅ 一致 |
| <code>WebPartManager.EnableViewState=false</code> | ✅ 一致 |
| <code>WebPartManager.EnableViewState=true</code> | ✅ 一致 |
| <code>WebPartManager.ExportSensitiveDataWarning=empty</code> | ✅ 一致 |
| <code>WebPartManager.ExportSensitiveDataWarning=japanese</code> | ✅ 一致 |
| <code>WebPartManager.ExportSensitiveDataWarning=markup</code> | ✅ 一致 |
| <code>WebPartManager.ExportSensitiveDataWarning=text</code> | ✅ 一致 |
| <code>WebPartManager.ID=empty</code> | ✅ 一致 |
| <code>WebPartManager.ID=japanese</code> | ✅ 一致 |
| <code>WebPartManager.ID=markup</code> | ✅ 一致 |
| <code>WebPartManager.ID=text</code> | ✅ 一致 |
| <code>WebPartManager.SkinID=empty</code> | ✅ 一致 |
| <code>WebPartManager.SkinID=japanese</code> | ✅ 一致 |
| <code>WebPartManager.SkinID=markup</code> | ✅ 一致 |
| <code>WebPartManager.SkinID=text</code> | ✅ 一致 |
| <code>WebPartManager.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>WebPartManager.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>WebPartManager.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>WebPartManager.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>WebPartManager.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>WebPartManager.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>WebPartManager.Visible=false</code> | ✅ 一致 |
| <code>WebPartManager.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>WebPartManager.Visible=true</code> | ✅ 一致 |
| <code>WebPartManager[naming container].(default)</code> | ✅ 一致 |
| <code>WebPartManager[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>WebPartManager[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>WebPartManager[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>WebPartManager[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>WebPartManager[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| WebPartManager [render][databind][dynamic] first request (browse): the parts in their zones, a zone made in code | ✅ 一致 |
| WebPartManager [viewstate] the design mode | ✅ 一致 |
| WebPartManager [viewstate] the catalog mode: the catalog zone and its parts | ✅ 一致 |
| WebPartManager [viewstate] a part added from the catalog (its check box posted, the zone&#39;s Add) | ✅ 一致 |
| WebPartManager [viewstate] the edit mode: the editor zone and its parts for the first part | ✅ 一致 |
| WebPartManager [viewstate] the first part&#39;s title edited (the editor zone&#39;s OK) | ✅ 一致 |
| WebPartManager [viewstate] the connect mode for the first part | ✅ 一致 |
| WebPartManager [viewstate] the connections zone closed | ✅ 一致 |
| WebPartManager [postbackevent] a part minimized by its zone&#39;s verb | ✅ 一致 |
| WebPartManager [viewstate] restored | ✅ 一致 |
| WebPartManager [viewstate] a part of the zone made in code minimized | ✅ 一致 |
| WebPartManager [visible][viewstate] a hidden zone shown | ✅ 一致 |

</details>
