# SiteMapPath パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-16 18:11
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 182 | 182 | 0 | 0 |
| ② HTML | 172 | 172 | 0 | 0 |
| ③ シナリオ | 12 | 12 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 191 件中 191 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 56 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 12 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 157 | 1 | ✅ |
| ViewState の保存と復元 | 6 | 11 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 7 | 0 | ✅ |
| テーマとスキン（SkinID / EnableTheming） | 4 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 1 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 3 | 0 | ✅ |
| テンプレート | 0 | 1 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>SiteMapPath.Api.Control.#ctor</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.Adapter</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.ClientID</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.Context</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.Controls</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.DataBind</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.Dispose</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.Disposed</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.Events</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.FindControl</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.Focus</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.HasControls</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.ID</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.Init</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.Load</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.OnInit</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.Page</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.Parent</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.PreRender</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.Render</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.Site</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.SkinID</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.Unload</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.ViewState</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>SiteMapPath.Api.Control.Visible</code> | ✅ 一致 |
| <code>SiteMapPath.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>SiteMapPath.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>SiteMapPath.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>SiteMapPath.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>SiteMapPath.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>SiteMapPath.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>SiteMapPath.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>SiteMapPath.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>SiteMapPath.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>SiteMapPath.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>SiteMapPath.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>SiteMapPath.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>SiteMapPath.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.CompositeControl.#ctor</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.CompositeControl.Controls</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.CompositeControl.DataBind</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.CompositeControl.RecreateChildControls</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.CompositeControl.Render</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.CompositeControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.ICompositeControlDesignerAccessor.RecreateChildControls</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.#ctor</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.CreateChildControls</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.CreateControlHierarchy</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.CurrentNodeStyle</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.CurrentNodeTemplate</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.DataBind</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.InitializeItem</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.ItemCreated</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.ItemDataBound</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.LoadViewState</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.NodeStyle</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.NodeTemplate</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.OnDataBinding</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.OnItemCreated</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.OnItemDataBound</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.ParentLevelsDisplayed</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.PathDirection</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.PathSeparator</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.PathSeparatorStyle</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.PathSeparatorTemplate</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.Provider</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.Render</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.RenderContents</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.RenderCurrentNodeAsLink</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.RootNodeStyle</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.RootNodeTemplate</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.SaveViewState</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.ShowToolTips</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.SiteMapProvider</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.SkipLinkText</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.SiteMapPath.TrackViewState</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.#ctor</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.AccessKey</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.AddAttributesToRender</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.ApplyStyle</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.Attributes</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.BackColor</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.BorderColor</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.BorderStyle</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.BorderWidth</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.ControlStyle</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.ControlStyleCreated</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.CopyBaseAttributes</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.CreateControlStyle</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.CssClass</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.DisabledCssClass</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.EnableTheming</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.Enabled</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.Font</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.ForeColor</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.HasAttributes</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.Height</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.IsEnabled</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.LoadViewState</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.MergeStyle</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.Render</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.RenderBeginTag</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.RenderContents</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.RenderEndTag</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.SaveViewState</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.SkinID</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.Style</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.SupportsDisabledAttribute</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.TabIndex</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.TagKey</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.TagName</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.ToolTip</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.TrackViewState</code> | ✅ 一致 |
| <code>SiteMapPath.Api.WebControls.WebControl.Width</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>SiteMapPath.(case list)</code> | ✅ 一致 |
| <code>SiteMapPath.(default)</code> | ✅ 一致 |
| <code>SiteMapPath.AccessKey=empty</code> | ✅ 一致 |
| <code>SiteMapPath.AccessKey=japanese</code> | ✅ 一致 |
| <code>SiteMapPath.AccessKey=k</code> | ✅ 一致 |
| <code>SiteMapPath.AccessKey=markup</code> | ✅ 一致 |
| <code>SiteMapPath.AccessKey=text</code> | ✅ 一致 |
| <code>SiteMapPath.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>SiteMapPath.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>SiteMapPath.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>SiteMapPath.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>SiteMapPath.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>SiteMapPath.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>SiteMapPath.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>SiteMapPath.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>SiteMapPath.BackColor=#3366CC</code> | ✅ 一致 |
| <code>SiteMapPath.BackColor=Red</code> | ✅ 一致 |
| <code>SiteMapPath.BackColor=Transparent</code> | ✅ 一致 |
| <code>SiteMapPath.BorderColor=#3366CC</code> | ✅ 一致 |
| <code>SiteMapPath.BorderColor=Red</code> | ✅ 一致 |
| <code>SiteMapPath.BorderColor=Transparent</code> | ✅ 一致 |
| <code>SiteMapPath.BorderStyle=Dashed</code> | ✅ 一致 |
| <code>SiteMapPath.BorderStyle=Dotted</code> | ✅ 一致 |
| <code>SiteMapPath.BorderStyle=Double</code> | ✅ 一致 |
| <code>SiteMapPath.BorderStyle=Groove</code> | ✅ 一致 |
| <code>SiteMapPath.BorderStyle=Inset</code> | ✅ 一致 |
| <code>SiteMapPath.BorderStyle=None</code> | ✅ 一致 |
| <code>SiteMapPath.BorderStyle=NotSet</code> | ✅ 一致 |
| <code>SiteMapPath.BorderStyle=Outset</code> | ✅ 一致 |
| <code>SiteMapPath.BorderStyle=Ridge</code> | ✅ 一致 |
| <code>SiteMapPath.BorderStyle=Solid</code> | ✅ 一致 |
| <code>SiteMapPath.BorderWidth=120px</code> | ✅ 一致 |
| <code>SiteMapPath.BorderWidth=2em</code> | ✅ 一致 |
| <code>SiteMapPath.BorderWidth=50%</code> | ✅ 一致 |
| <code>SiteMapPath.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>SiteMapPath.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>SiteMapPath.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>SiteMapPath.ClientIDMode=Static</code> | ✅ 一致 |
| <code>SiteMapPath.CssClass=empty</code> | ✅ 一致 |
| <code>SiteMapPath.CssClass=japanese</code> | ✅ 一致 |
| <code>SiteMapPath.CssClass=markup</code> | ✅ 一致 |
| <code>SiteMapPath.CssClass=text</code> | ✅ 一致 |
| <code>SiteMapPath.CurrentNodeStyle.BackColor=Red</code> | ✅ 一致 |
| <code>SiteMapPath.CurrentNodeStyle.CssClass=text</code> | ✅ 一致 |
| <code>SiteMapPath.CurrentNodeStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>SiteMapPath.CurrentNodeStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>SiteMapPath.CurrentNodeStyle.ForeColor=Red+NodeStyle.BackColor=#3366CC</code> | ✅ 一致 |
| <code>SiteMapPath.CurrentNodeStyle.Height=120px</code> | ✅ 一致 |
| <code>SiteMapPath.EnableTheming=false</code> | ✅ 一致 |
| <code>SiteMapPath.EnableTheming=true</code> | ✅ 一致 |
| <code>SiteMapPath.EnableViewState=false</code> | ✅ 一致 |
| <code>SiteMapPath.EnableViewState=true</code> | ✅ 一致 |
| <code>SiteMapPath.Enabled=false</code> | ✅ 一致 |
| <code>SiteMapPath.Enabled=false+CssClass=text</code> | ✅ 一致 |
| <code>SiteMapPath.Enabled=false+RenderCurrentNodeAsLink=true</code> | ✅ 一致 |
| <code>SiteMapPath.Enabled=true</code> | ✅ 一致 |
| <code>SiteMapPath.Font.Bold=false</code> | ✅ 一致 |
| <code>SiteMapPath.Font.Bold=true</code> | ✅ 一致 |
| <code>SiteMapPath.Font.Italic=false</code> | ✅ 一致 |
| <code>SiteMapPath.Font.Italic=true</code> | ✅ 一致 |
| <code>SiteMapPath.Font.Name=empty</code> | ✅ 一致 |
| <code>SiteMapPath.Font.Name=japanese</code> | ✅ 一致 |
| <code>SiteMapPath.Font.Name=markup</code> | ✅ 一致 |
| <code>SiteMapPath.Font.Name=text</code> | ✅ 一致 |
| <code>SiteMapPath.Font.Names=Arial,MS-Gothic</code> | ✅ 一致 |
| <code>SiteMapPath.Font.Overline=false</code> | ✅ 一致 |
| <code>SiteMapPath.Font.Overline=true</code> | ✅ 一致 |
| <code>SiteMapPath.Font.Size=12pt</code> | ✅ 一致 |
| <code>SiteMapPath.Font.Size=150%</code> | ✅ 一致 |
| <code>SiteMapPath.Font.Size=Large</code> | ✅ 一致 |
| <code>SiteMapPath.Font.Strikeout=false</code> | ✅ 一致 |
| <code>SiteMapPath.Font.Strikeout=true</code> | ✅ 一致 |
| <code>SiteMapPath.Font.Underline=false</code> | ✅ 一致 |
| <code>SiteMapPath.Font.Underline=true</code> | ✅ 一致 |
| <code>SiteMapPath.ForeColor=#3366CC</code> | ✅ 一致 |
| <code>SiteMapPath.ForeColor=#3366CC+Style[color]=red</code> | ✅ 一致 |
| <code>SiteMapPath.ForeColor=Red</code> | ✅ 一致 |
| <code>SiteMapPath.ForeColor=Transparent</code> | ✅ 一致 |
| <code>SiteMapPath.Height=120px</code> | ✅ 一致 |
| <code>SiteMapPath.Height=2em</code> | ✅ 一致 |
| <code>SiteMapPath.Height=50%</code> | ✅ 一致 |
| <code>SiteMapPath.ID=empty</code> | ✅ 一致 |
| <code>SiteMapPath.ID=japanese</code> | ✅ 一致 |
| <code>SiteMapPath.ID=markup</code> | ✅ 一致 |
| <code>SiteMapPath.ID=text</code> | ✅ 一致 |
| <code>SiteMapPath.NodeStyle.BackColor=Red</code> | ✅ 一致 |
| <code>SiteMapPath.NodeStyle.CssClass=text</code> | ✅ 一致 |
| <code>SiteMapPath.NodeStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>SiteMapPath.NodeStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>SiteMapPath.NodeStyle.Height=120px</code> | ✅ 一致 |
| <code>SiteMapPath.ParentLevelsDisplayed=-1</code> | ✅ 一致 |
| <code>SiteMapPath.ParentLevelsDisplayed=0</code> | ✅ 一致 |
| <code>SiteMapPath.ParentLevelsDisplayed=0+PathSeparator=arrow</code> | ✅ 一致 |
| <code>SiteMapPath.ParentLevelsDisplayed=1</code> | ✅ 一致 |
| <code>SiteMapPath.ParentLevelsDisplayed=1+RenderCurrentNodeAsLink=true</code> | ✅ 一致 |
| <code>SiteMapPath.ParentLevelsDisplayed=3</code> | ✅ 一致 |
| <code>SiteMapPath.PathDirection=CurrentToRoot</code> | ✅ 一致 |
| <code>SiteMapPath.PathDirection=CurrentToRoot+PathSeparator=arrow</code> | ✅ 一致 |
| <code>SiteMapPath.PathDirection=RootToCurrent</code> | ✅ 一致 |
| <code>SiteMapPath.PathSeparator=arrow</code> | ✅ 一致 |
| <code>SiteMapPath.PathSeparator=arrow+EnableViewState=false</code> | ✅ 一致 |
| <code>SiteMapPath.PathSeparator=arrow+Visible=false</code> | ✅ 一致 |
| <code>SiteMapPath.PathSeparator=empty</code> | ✅ 一致 |
| <code>SiteMapPath.PathSeparator=japanese</code> | ✅ 一致 |
| <code>SiteMapPath.PathSeparator=markup</code> | ✅ 一致 |
| <code>SiteMapPath.PathSeparator=text</code> | ✅ 一致 |
| <code>SiteMapPath.PathSeparatorStyle.BackColor=Red</code> | ✅ 一致 |
| <code>SiteMapPath.PathSeparatorStyle.CssClass=text</code> | ✅ 一致 |
| <code>SiteMapPath.PathSeparatorStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>SiteMapPath.PathSeparatorStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>SiteMapPath.PathSeparatorStyle.Height=120px</code> | ✅ 一致 |
| <code>SiteMapPath.RenderCurrentNodeAsLink=false</code> | ✅ 一致 |
| <code>SiteMapPath.RenderCurrentNodeAsLink=true</code> | ✅ 一致 |
| <code>SiteMapPath.RenderCurrentNodeAsLink=true+ShowToolTips=false</code> | ✅ 一致 |
| <code>SiteMapPath.RootNodeStyle.BackColor=Red</code> | ✅ 一致 |
| <code>SiteMapPath.RootNodeStyle.CssClass=text</code> | ✅ 一致 |
| <code>SiteMapPath.RootNodeStyle.Font.Bold=true</code> | ✅ 一致 |
| <code>SiteMapPath.RootNodeStyle.ForeColor=Red</code> | ✅ 一致 |
| <code>SiteMapPath.RootNodeStyle.ForeColor=Red+PathSeparatorStyle.BackColor=#3366CC</code> | ✅ 一致 |
| <code>SiteMapPath.RootNodeStyle.Height=120px</code> | ✅ 一致 |
| <code>SiteMapPath.ShowToolTips=false</code> | ✅ 一致 |
| <code>SiteMapPath.ShowToolTips=true</code> | ✅ 一致 |
| <code>SiteMapPath.SiteMapProvider=default</code> | ✅ 一致 |
| <code>SiteMapPath.SiteMapProvider=empty</code> | ✅ 一致 |
| <code>SiteMapPath.SiteMapProvider=japanese</code> | ✅ 一致 |
| <code>SiteMapPath.SiteMapProvider=markup</code> | ✅ 一致 |
| <code>SiteMapPath.SiteMapProvider=text</code> | ✅ 一致 |
| <code>SiteMapPath.SkinID=empty</code> | ✅ 一致 |
| <code>SiteMapPath.SkinID=japanese</code> | ✅ 一致 |
| <code>SiteMapPath.SkinID=markup</code> | ✅ 一致 |
| <code>SiteMapPath.SkinID=text</code> | ✅ 一致 |
| <code>SiteMapPath.SkipLinkText=empty</code> | ✅ 一致 |
| <code>SiteMapPath.SkipLinkText=empty+PathSeparator=arrow</code> | ✅ 一致 |
| <code>SiteMapPath.SkipLinkText=japanese</code> | ✅ 一致 |
| <code>SiteMapPath.SkipLinkText=markup</code> | ✅ 一致 |
| <code>SiteMapPath.SkipLinkText=text</code> | ✅ 一致 |
| <code>SiteMapPath.Style[color]=1em</code> | ✅ 一致 |
| <code>SiteMapPath.Style[color]=red</code> | ✅ 一致 |
| <code>SiteMapPath.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>SiteMapPath.Style[margin-left]=red</code> | ✅ 一致 |
| <code>SiteMapPath.TabIndex=-1</code> | ✅ 一致 |
| <code>SiteMapPath.TabIndex=0</code> | ✅ 一致 |
| <code>SiteMapPath.TabIndex=3</code> | ✅ 一致 |
| <code>SiteMapPath.ToolTip=empty</code> | ✅ 一致 |
| <code>SiteMapPath.ToolTip=japanese</code> | ✅ 一致 |
| <code>SiteMapPath.ToolTip=markup</code> | ✅ 一致 |
| <code>SiteMapPath.ToolTip=text</code> | ✅ 一致 |
| <code>SiteMapPath.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>SiteMapPath.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>SiteMapPath.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>SiteMapPath.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>SiteMapPath.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>SiteMapPath.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>SiteMapPath.Visible=false</code> | ✅ 一致 |
| <code>SiteMapPath.Visible=true</code> | ✅ 一致 |
| <code>SiteMapPath.Width=120px</code> | ✅ 一致 |
| <code>SiteMapPath.Width=120px+Height=2em+BorderWidth=120px+BorderStyle=Solid+BackColor=#3366CC+ForeColor=Red+Font.Bold=true</code> | ✅ 一致 |
| <code>SiteMapPath.Width=2em</code> | ✅ 一致 |
| <code>SiteMapPath.Width=50%</code> | ✅ 一致 |
| <code>SiteMapPath[naming container].(default)</code> | ✅ 一致 |
| <code>SiteMapPath[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>SiteMapPath[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>SiteMapPath[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>SiteMapPath[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>SiteMapPath[naming container].ID=text</code> | ✅ 一致 |
| <code>SiteMapPath[naming container].RenderCurrentNodeAsLink=true</code> | ✅ 一致 |
| <code>SiteMapPath[templates].(default)</code> | ✅ 一致 |
| <code>SiteMapPath[templates].PathDirection=CurrentToRoot</code> | ✅ 一致 |
| <code>SiteMapPath[themed].(default)</code> | ✅ 一致 |
| <code>SiteMapPath[themed].CssClass=text</code> | ✅ 一致 |
| <code>SiteMapPath[themed].EnableTheming=false</code> | ✅ 一致 |
| <code>SiteMapPath[themed].SkinID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| SiteMapPath [render][databind][dynamic][templates] first request: the site map as a path, trees and menus; bound to XML; templated; made in code | ✅ 一致 |
| SiteMapPath [viewstate] a node expanded by a postback (EnableClientScript=false) | ✅ 一致 |
| SiteMapPath [viewstate] a node selected (its value a control parameter&#39;s: ControlValueProperty) | ✅ 一致 |
| SiteMapPath [viewstate] a node&#39;s check box posted | ✅ 一致 |
| SiteMapPath [viewstate] a node populated on demand by a postback | ✅ 一致 |
| SiteMapPath [viewstate] a node populated on demand by a client callback (no postback) | ✅ 一致 |
| SiteMapPath [viewstate] a menu&#39;s item clicked | ✅ 一致 |
| SiteMapPath [viewstate] a bound menu&#39;s item clicked | ✅ 一致 |
| SiteMapPath [viewstate] a templated menu&#39;s item clicked | ✅ 一致 |
| SiteMapPath [viewstate] a dynamic tree&#39;s node selected | ✅ 一致 |
| SiteMapPath [viewstate] a dynamic menu&#39;s item clicked | ✅ 一致 |
| SiteMapPath [visible][viewstate] hidden ones shown with what was added while hidden | ✅ 一致 |

</details>
