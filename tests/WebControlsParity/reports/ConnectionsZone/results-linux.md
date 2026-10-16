# ConnectionsZone パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 01:38
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 251 | 251 | 0 | 0 |
| ② HTML | 298 | 298 | 0 | 0 |
| ③ シナリオ | 12 | 12 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 261 件中 261 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 127 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 12 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 286 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 10 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 1 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 2 | 0 | ✅ |
| ポストバックイベント | 0 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>ConnectionsZone.Api.Control.#ctor</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.Adapter</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.ClientID</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.Context</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.Controls</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.DataBind</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.Dispose</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.Disposed</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.Events</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.FindControl</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.Focus</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.HasControls</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.ID</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.Init</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.Load</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.OnInit</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.Page</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.Parent</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.PreRender</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.Render</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.Site</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.SkinID</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.Unload</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.ViewState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.Control.Visible</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.IPostBackEventHandler.RaisePostBackEvent</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.CompositeControl.#ctor</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.CompositeControl.Controls</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.CompositeControl.DataBind</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.CompositeControl.RecreateChildControls</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.CompositeControl.Render</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.CompositeControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.ICompositeControlDesignerAccessor.RecreateChildControls</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebControl.Width</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.#ctor</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.CancelVerb</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.Close</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.CloseVerb</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.ConfigureConnectionTitle</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.ConfigureVerb</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.ConnectToConsumerInstructionText</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.ConnectToConsumerText</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.ConnectToConsumerTitle</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.ConnectToProviderInstructionText</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.ConnectToProviderText</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.ConnectToProviderTitle</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.ConnectVerb</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.ConsumersInstructionText</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.ConsumersTitle</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.CreateChildControls</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.DisconnectVerb</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.Display</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.EmptyZoneText</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.ExistingConnectionErrorMessage</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.GetFromText</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.GetText</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.HeaderText</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.InstructionText</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.InstructionTitle</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.LoadControlState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.LoadViewState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.NewConnectionErrorMessage</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.NoExistingConnectionInstructionText</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.NoExistingConnectionTitle</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.OnDisplayModeChanged</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.OnInit</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.OnSelectedWebPartChanged</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.PartChromeType</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.ProvidersInstructionText</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.ProvidersTitle</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.RaisePostBackEvent</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.Render</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.RenderBody</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.RenderVerbs</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.SaveControlState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.SaveViewState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.SendText</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.SendToText</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.TrackViewState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ConnectionsZone.WebPartToConnect</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ToolZone.#ctor</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ToolZone.AssociatedDisplayModes</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ToolZone.Close</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ToolZone.Display</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ToolZone.EditUIStyle</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ToolZone.HeaderCloseVerb</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ToolZone.HeaderVerbStyle</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ToolZone.InstructionText</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ToolZone.InstructionTextStyle</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ToolZone.LabelStyle</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ToolZone.LoadViewState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ToolZone.OnDisplayModeChanged</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ToolZone.OnInit</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ToolZone.OnSelectedWebPartChanged</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ToolZone.RaisePostBackEvent</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ToolZone.RenderFooter</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ToolZone.RenderHeader</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ToolZone.RenderVerb</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ToolZone.RenderVerbs</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ToolZone.SaveViewState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ToolZone.TrackViewState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.ToolZone.Visible</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.BackImageUrl</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.EmptyZoneText</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.EmptyZoneTextStyle</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.ErrorStyle</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.FooterStyle</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.GetEffectiveChromeType</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.HasFooter</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.HasHeader</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.HeaderStyle</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.HeaderText</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.LoadViewState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.OnInit</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.OnPreRender</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.Padding</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.PartChromePadding</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.PartChromeStyle</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.PartChromeType</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.PartStyle</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.PartTitleStyle</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.RenderBeginTag</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.RenderBody</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.RenderClientScript</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.RenderContents</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.RenderFooter</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.RenderHeader</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.SaveViewState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.TagKey</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.TrackViewState</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.VerbButtonType</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.VerbStyle</code> | ✅ 一致 |
| <code>ConnectionsZone.Api.WebControls.WebParts.WebZone.WebPartManager</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>ConnectionsZone.(case list)</code> | ✅ 一致 |
| <code>ConnectionsZone.(default)</code> | ✅ 一致 |
| <code>ConnectionsZone.AccessKey=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.AccessKey=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.AccessKey=k</code> | ✅ 一致 |
| <code>ConnectionsZone.AccessKey=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.AccessKey=text</code> | ✅ 一致 |
| <code>ConnectionsZone.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>ConnectionsZone.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>ConnectionsZone.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>ConnectionsZone.BackColor=#3366CC</code> | ✅ 一致 |
| <code>ConnectionsZone.BackColor=Red</code> | ✅ 一致 |
| <code>ConnectionsZone.BackColor=Transparent</code> | ✅ 一致 |
| <code>ConnectionsZone.BackImageUrl=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.BackImageUrl=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.BackImageUrl=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.BackImageUrl=text</code> | ✅ 一致 |
| <code>ConnectionsZone.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>ConnectionsZone.BorderColor=Red</code> | ✅ 一致 |
| <code>ConnectionsZone.BorderColor=Transparent</code> | ✅ 一致 |
| <code>ConnectionsZone.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>ConnectionsZone.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>ConnectionsZone.BorderStyle=Double</code> | ✅ 一致 |
| <code>ConnectionsZone.BorderStyle=Groove</code> | ✅ 一致 |
| <code>ConnectionsZone.BorderStyle=Inset</code> | ✅ 一致 |
| <code>ConnectionsZone.BorderStyle=None</code> | ✅ 一致 |
| <code>ConnectionsZone.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>ConnectionsZone.BorderStyle=Outset</code> | ✅ 一致 |
| <code>ConnectionsZone.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>ConnectionsZone.BorderStyle=Solid</code> | ✅ 一致 |
| <code>ConnectionsZone.BorderWidth=120px</code> | ✅ 一致 |
| <code>ConnectionsZone.BorderWidth=2em</code> | ✅ 一致 |
| <code>ConnectionsZone.BorderWidth=50%</code> | ✅ 一致 |
| <code>ConnectionsZone.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ConnectionsZone.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ConnectionsZone.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ConnectionsZone.ClientIDMode=Static</code> | ✅ 一致 |
| <code>ConnectionsZone.ConfigureConnectionTitle=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.ConfigureConnectionTitle=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.ConfigureConnectionTitle=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.ConfigureConnectionTitle=text</code> | ✅ 一致 |
| <code>ConnectionsZone.ConnectToConsumerInstructionText=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.ConnectToConsumerInstructionText=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.ConnectToConsumerInstructionText=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.ConnectToConsumerInstructionText=text</code> | ✅ 一致 |
| <code>ConnectionsZone.ConnectToConsumerText=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.ConnectToConsumerText=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.ConnectToConsumerText=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.ConnectToConsumerText=text</code> | ✅ 一致 |
| <code>ConnectionsZone.ConnectToConsumerTitle=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.ConnectToConsumerTitle=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.ConnectToConsumerTitle=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.ConnectToConsumerTitle=text</code> | ✅ 一致 |
| <code>ConnectionsZone.ConnectToProviderInstructionText=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.ConnectToProviderInstructionText=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.ConnectToProviderInstructionText=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.ConnectToProviderInstructionText=text</code> | ✅ 一致 |
| <code>ConnectionsZone.ConnectToProviderText=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.ConnectToProviderText=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.ConnectToProviderText=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.ConnectToProviderText=text</code> | ✅ 一致 |
| <code>ConnectionsZone.ConnectToProviderTitle=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.ConnectToProviderTitle=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.ConnectToProviderTitle=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.ConnectToProviderTitle=text</code> | ✅ 一致 |
| <code>ConnectionsZone.ConsumersInstructionText=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.ConsumersInstructionText=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.ConsumersInstructionText=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.ConsumersInstructionText=text</code> | ✅ 一致 |
| <code>ConnectionsZone.ConsumersTitle=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.ConsumersTitle=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.ConsumersTitle=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.ConsumersTitle=text</code> | ✅ 一致 |
| <code>ConnectionsZone.CssClass=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.CssClass=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.CssClass=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.CssClass=text</code> | ✅ 一致 |
| <code>ConnectionsZone.EditUIStyle.BackColor=Red</code> | ✅ 一致 |
| <code>ConnectionsZone.EditUIStyle.CssClass=text</code> | ✅ 一致 |
| <code>ConnectionsZone.EditUIStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>ConnectionsZone.EditUIStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>ConnectionsZone.EditUIStyle.Height=120px</code> | ✅ 一致 |
| <code>ConnectionsZone.EmptyZoneText=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.EmptyZoneText=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.EmptyZoneText=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.EmptyZoneText=text</code> | ✅ 一致 |
| <code>ConnectionsZone.EmptyZoneTextStyle.BackColor=Red</code> | ✅ 一致 |
| <code>ConnectionsZone.EmptyZoneTextStyle.CssClass=text</code> | ✅ 一致 |
| <code>ConnectionsZone.EmptyZoneTextStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>ConnectionsZone.EmptyZoneTextStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>ConnectionsZone.EmptyZoneTextStyle.Height=120px</code> | ✅ 一致 |
| <code>ConnectionsZone.EnableTheming=false</code> | ✅ 一致 |
| <code>ConnectionsZone.EnableTheming=true</code> | ✅ 一致 |
| <code>ConnectionsZone.EnableViewState=false</code> | ✅ 一致 |
| <code>ConnectionsZone.EnableViewState=true</code> | ✅ 一致 |
| <code>ConnectionsZone.Enabled=false</code> | ✅ 一致 |
| <code>ConnectionsZone.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>ConnectionsZone.Enabled=true</code> | ✅ 一致 |
| <code>ConnectionsZone.ErrorStyle.BackColor=Red</code> | ✅ 一致 |
| <code>ConnectionsZone.ErrorStyle.CssClass=text</code> | ✅ 一致 |
| <code>ConnectionsZone.ErrorStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>ConnectionsZone.ErrorStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>ConnectionsZone.ErrorStyle.Height=120px</code> | ✅ 一致 |
| <code>ConnectionsZone.ExistingConnectionErrorMessage=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.ExistingConnectionErrorMessage=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.ExistingConnectionErrorMessage=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.ExistingConnectionErrorMessage=text</code> | ✅ 一致 |
| <code>ConnectionsZone.Font.Bold=false</code> | ✅ 一致 |
| <code>ConnectionsZone.Font.Bold=true</code> | ✅ 一致 |
| <code>ConnectionsZone.Font.Italic=false</code> | ✅ 一致 |
| <code>ConnectionsZone.Font.Italic=true</code> | ✅ 一致 |
| <code>ConnectionsZone.Font.Name=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.Font.Name=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.Font.Name=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.Font.Name=text</code> | ✅ 一致 |
| <code>ConnectionsZone.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>ConnectionsZone.Font.Overline=false</code> | ✅ 一致 |
| <code>ConnectionsZone.Font.Overline=true</code> | ✅ 一致 |
| <code>ConnectionsZone.Font.Size=12pt</code> | ✅ 一致 |
| <code>ConnectionsZone.Font.Size=150%</code> | ✅ 一致 |
| <code>ConnectionsZone.Font.Size=Large</code> | ✅ 一致 |
| <code>ConnectionsZone.Font.Strikeout=false</code> | ✅ 一致 |
| <code>ConnectionsZone.Font.Strikeout=true</code> | ✅ 一致 |
| <code>ConnectionsZone.Font.Underline=false</code> | ✅ 一致 |
| <code>ConnectionsZone.Font.Underline=true</code> | ✅ 一致 |
| <code>ConnectionsZone.FooterStyle.BackColor=Red</code> | ✅ 一致 |
| <code>ConnectionsZone.FooterStyle.CssClass=text</code> | ✅ 一致 |
| <code>ConnectionsZone.FooterStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>ConnectionsZone.FooterStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>ConnectionsZone.FooterStyle.Height=120px</code> | ✅ 一致 |
| <code>ConnectionsZone.FooterStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>ConnectionsZone.FooterStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>ConnectionsZone.FooterStyle.Wrap=true</code> | ✅ 一致 |
| <code>ConnectionsZone.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>ConnectionsZone.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>ConnectionsZone.ForeColor=Red</code> | ✅ 一致 |
| <code>ConnectionsZone.ForeColor=Transparent</code> | ✅ 一致 |
| <code>ConnectionsZone.GetFromText=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.GetFromText=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.GetFromText=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.GetFromText=text</code> | ✅ 一致 |
| <code>ConnectionsZone.GetText=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.GetText=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.GetText=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.GetText=text</code> | ✅ 一致 |
| <code>ConnectionsZone.HeaderStyle.BackColor=Red</code> | ✅ 一致 |
| <code>ConnectionsZone.HeaderStyle.CssClass=text</code> | ✅ 一致 |
| <code>ConnectionsZone.HeaderStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>ConnectionsZone.HeaderStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>ConnectionsZone.HeaderStyle.Height=120px</code> | ✅ 一致 |
| <code>ConnectionsZone.HeaderStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>ConnectionsZone.HeaderStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>ConnectionsZone.HeaderStyle.Wrap=true</code> | ✅ 一致 |
| <code>ConnectionsZone.HeaderText=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.HeaderText=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.HeaderText=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.HeaderText=markup+InstructionText=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.HeaderText=text</code> | ✅ 一致 |
| <code>ConnectionsZone.HeaderVerbStyle.BackColor=Red</code> | ✅ 一致 |
| <code>ConnectionsZone.HeaderVerbStyle.CssClass=text</code> | ✅ 一致 |
| <code>ConnectionsZone.HeaderVerbStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>ConnectionsZone.HeaderVerbStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>ConnectionsZone.HeaderVerbStyle.Height=120px</code> | ✅ 一致 |
| <code>ConnectionsZone.Height=120px</code> | ✅ 一致 |
| <code>ConnectionsZone.Height=2em</code> | ✅ 一致 |
| <code>ConnectionsZone.Height=50%</code> | ✅ 一致 |
| <code>ConnectionsZone.ID=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.ID=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.ID=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.ID=text</code> | ✅ 一致 |
| <code>ConnectionsZone.InstructionText=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.InstructionText=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.InstructionText=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.InstructionText=text</code> | ✅ 一致 |
| <code>ConnectionsZone.InstructionTextStyle.BackColor=Red</code> | ✅ 一致 |
| <code>ConnectionsZone.InstructionTextStyle.CssClass=text</code> | ✅ 一致 |
| <code>ConnectionsZone.InstructionTextStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>ConnectionsZone.InstructionTextStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>ConnectionsZone.InstructionTextStyle.Height=120px</code> | ✅ 一致 |
| <code>ConnectionsZone.InstructionTitle=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.InstructionTitle=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.InstructionTitle=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.InstructionTitle=text</code> | ✅ 一致 |
| <code>ConnectionsZone.LabelStyle.BackColor=Red</code> | ✅ 一致 |
| <code>ConnectionsZone.LabelStyle.CssClass=text</code> | ✅ 一致 |
| <code>ConnectionsZone.LabelStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>ConnectionsZone.LabelStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>ConnectionsZone.LabelStyle.Height=120px</code> | ✅ 一致 |
| <code>ConnectionsZone.NewConnectionErrorMessage=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.NewConnectionErrorMessage=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.NewConnectionErrorMessage=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.NewConnectionErrorMessage=text</code> | ✅ 一致 |
| <code>ConnectionsZone.NoExistingConnectionInstructionText=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.NoExistingConnectionInstructionText=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.NoExistingConnectionInstructionText=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.NoExistingConnectionInstructionText=text</code> | ✅ 一致 |
| <code>ConnectionsZone.NoExistingConnectionTitle=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.NoExistingConnectionTitle=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.NoExistingConnectionTitle=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.NoExistingConnectionTitle=markup+NewConnectionErrorMessage=text</code> | ✅ 一致 |
| <code>ConnectionsZone.NoExistingConnectionTitle=text</code> | ✅ 一致 |
| <code>ConnectionsZone.Padding=-1</code> | ✅ 一致 |
| <code>ConnectionsZone.Padding=0</code> | ✅ 一致 |
| <code>ConnectionsZone.Padding=3</code> | ✅ 一致 |
| <code>ConnectionsZone.PartChromePadding=120px</code> | ✅ 一致 |
| <code>ConnectionsZone.PartChromePadding=2em</code> | ✅ 一致 |
| <code>ConnectionsZone.PartChromePadding=50%</code> | ✅ 一致 |
| <code>ConnectionsZone.PartChromeStyle.BackColor=Red</code> | ✅ 一致 |
| <code>ConnectionsZone.PartChromeStyle.CssClass=text</code> | ✅ 一致 |
| <code>ConnectionsZone.PartChromeStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>ConnectionsZone.PartChromeStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>ConnectionsZone.PartChromeStyle.Height=120px</code> | ✅ 一致 |
| <code>ConnectionsZone.PartChromeType=BorderOnly</code> | ✅ 一致 |
| <code>ConnectionsZone.PartChromeType=Default</code> | ✅ 一致 |
| <code>ConnectionsZone.PartChromeType=None</code> | ✅ 一致 |
| <code>ConnectionsZone.PartChromeType=TitleAndBorder</code> | ✅ 一致 |
| <code>ConnectionsZone.PartChromeType=TitleOnly</code> | ✅ 一致 |
| <code>ConnectionsZone.PartStyle.BackColor=Red</code> | ✅ 一致 |
| <code>ConnectionsZone.PartStyle.CssClass=text</code> | ✅ 一致 |
| <code>ConnectionsZone.PartStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>ConnectionsZone.PartStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>ConnectionsZone.PartStyle.Height=120px</code> | ✅ 一致 |
| <code>ConnectionsZone.PartStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>ConnectionsZone.PartTitleStyle.BackColor=Red</code> | ✅ 一致 |
| <code>ConnectionsZone.PartTitleStyle.CssClass=text</code> | ✅ 一致 |
| <code>ConnectionsZone.PartTitleStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>ConnectionsZone.PartTitleStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>ConnectionsZone.PartTitleStyle.Height=120px</code> | ✅ 一致 |
| <code>ConnectionsZone.PartTitleStyle.HorizontalAlign=NotSet</code> | ✅ 一致 |
| <code>ConnectionsZone.PartTitleStyle.VerticalAlign=NotSet</code> | ✅ 一致 |
| <code>ConnectionsZone.PartTitleStyle.Wrap=true</code> | ✅ 一致 |
| <code>ConnectionsZone.ProvidersInstructionText=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.ProvidersInstructionText=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.ProvidersInstructionText=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.ProvidersInstructionText=text</code> | ✅ 一致 |
| <code>ConnectionsZone.ProvidersTitle=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.ProvidersTitle=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.ProvidersTitle=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.ProvidersTitle=text</code> | ✅ 一致 |
| <code>ConnectionsZone.SendText=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.SendText=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.SendText=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.SendText=text</code> | ✅ 一致 |
| <code>ConnectionsZone.SendToText=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.SendToText=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.SendToText=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.SendToText=text</code> | ✅ 一致 |
| <code>ConnectionsZone.SkinID=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.SkinID=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.SkinID=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.SkinID=text</code> | ✅ 一致 |
| <code>ConnectionsZone.Style[color]=1em</code> | ✅ 一致 |
| <code>ConnectionsZone.Style[color]=red</code> | ✅ 一致 |
| <code>ConnectionsZone.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>ConnectionsZone.Style[margin-left]=red</code> | ✅ 一致 |
| <code>ConnectionsZone.TabIndex=-1</code> | ✅ 一致 |
| <code>ConnectionsZone.TabIndex=0</code> | ✅ 一致 |
| <code>ConnectionsZone.TabIndex=3</code> | ✅ 一致 |
| <code>ConnectionsZone.ToolTip=empty</code> | ✅ 一致 |
| <code>ConnectionsZone.ToolTip=japanese</code> | ✅ 一致 |
| <code>ConnectionsZone.ToolTip=markup</code> | ✅ 一致 |
| <code>ConnectionsZone.ToolTip=text</code> | ✅ 一致 |
| <code>ConnectionsZone.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>ConnectionsZone.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>ConnectionsZone.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>ConnectionsZone.VerbButtonType=Button</code> | ✅ 一致 |
| <code>ConnectionsZone.VerbButtonType=Image</code> | ✅ 一致 |
| <code>ConnectionsZone.VerbButtonType=Link</code> | ✅ 一致 |
| <code>ConnectionsZone.VerbStyle.BackColor=Red</code> | ✅ 一致 |
| <code>ConnectionsZone.VerbStyle.CssClass=text</code> | ✅ 一致 |
| <code>ConnectionsZone.VerbStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>ConnectionsZone.VerbStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>ConnectionsZone.VerbStyle.Height=120px</code> | ✅ 一致 |
| <code>ConnectionsZone.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>ConnectionsZone.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>ConnectionsZone.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>ConnectionsZone.Visible=false</code> | ✅ 一致 |
| <code>ConnectionsZone.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>ConnectionsZone.Visible=true</code> | ✅ 一致 |
| <code>ConnectionsZone.Width=120px</code> | ✅ 一致 |
| <code>ConnectionsZone.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>ConnectionsZone.Width=2em</code> | ✅ 一致 |
| <code>ConnectionsZone.Width=50%</code> | ✅ 一致 |
| <code>ConnectionsZone[naming container].(default)</code> | ✅ 一致 |
| <code>ConnectionsZone[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>ConnectionsZone[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>ConnectionsZone[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>ConnectionsZone[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>ConnectionsZone[naming container].ID=text</code> | ✅ 一致 |
| <code>ConnectionsZone[themed].(default)</code> | ✅ 一致 |
| <code>ConnectionsZone[themed].CssClass=text</code> | ✅ 一致 |
| <code>ConnectionsZone[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>ConnectionsZone[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| ConnectionsZone [render][databind][dynamic] first request (browse): the parts in their zones, a zone made in code | ✅ 一致 |
| ConnectionsZone [viewstate] the design mode | ✅ 一致 |
| ConnectionsZone [viewstate] the catalog mode: the catalog zone and its parts | ✅ 一致 |
| ConnectionsZone [viewstate] a part added from the catalog (its check box posted, the zone&#39;s Add) | ✅ 一致 |
| ConnectionsZone [viewstate] the edit mode: the editor zone and its parts for the first part | ✅ 一致 |
| ConnectionsZone [viewstate] the first part&#39;s title edited (the editor zone&#39;s OK) | ✅ 一致 |
| ConnectionsZone [viewstate] the connect mode for the first part | ✅ 一致 |
| ConnectionsZone [postbackevent] the connections zone closed | ✅ 一致 |
| ConnectionsZone [viewstate] a part minimized by its zone&#39;s verb | ✅ 一致 |
| ConnectionsZone [viewstate] restored | ✅ 一致 |
| ConnectionsZone [viewstate] a part of the zone made in code minimized | ✅ 一致 |
| ConnectionsZone [visible][viewstate] a hidden zone shown | ✅ 一致 |

</details>
