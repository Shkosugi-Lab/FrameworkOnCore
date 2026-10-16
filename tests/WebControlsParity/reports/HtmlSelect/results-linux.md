# HtmlSelect パリティ結果（Linux（Docker））

このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。

- 実行日時: 2026-10-12 02:18
- ポート: FrameworkOnCore の System.Web 10.0.29.0（Linux（Docker））
- 正解データ: 2026-10-11, .NET Framework 4.8 (System.Web 4.8.9344.0 built by: NET481REL1LAST_25H2_B), IIS 10.0.26100.1882, Windows 10.0.26200.0

## サマリ

| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |
|---|---|---|---|---|
| ① API | 166 | 166 | 0 | 0 |
| ② HTML | 117 | 117 | 0 | 0 |
| ③ シナリオ | 7 | 7 | 0 | 0 |

## カバレッジ

- ① API: .NET Framework の公開/プロテクトメンバー 175 件中 175 件を呼び出し（除外 0 件）。API の形の差: なし（ポートは同じメンバーを持つ）
- ② HTML: 設定可能なプロパティ 21 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）
- ③ シナリオ: 7 スナップショット（機構は下表）

| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |
|---|---|---|---|
| プロパティ → HTML 描画 | 91 | 1 | ✅ |
| ViewState の保存と復元 | 7 | 3 | ✅ |
| Visible=false（描画されず状態は保持） | 2 | 1 | ✅ |
| 名前付けコンテナと ClientIDMode | 6 | 0 | ✅ |
| 動的に生成したコントロールのライフサイクル | 0 | 2 | ✅ |
| データバインド（DataBind / <%# %>） | 0 | 1 | ✅ |
| Enabled=false・Disabled=true（描画とポスト値の扱い） | 5 | 0 | ✅ |
| ポストバックデータの取り込み | 0 | 1 | ✅ |
| 変更イベント（*Changed、ServerChange） | 0 | 2 | ✅ |
| リクエスト検証（ValidateRequestMode） | 3 | 0 | ✅ |
| 検証対象（サーバー側の検証） | 0 | 1 | ✅ |
| 検証対象（ブラウザ側の検証） | 0 | 1 | ✅ |
| 項目（ListItem の Selected・Enabled・Text・Value と ViewState） | 15 | 0 | ✅ |
| 複数選択 | 3 | 0 | ✅ |
| ポスト値のイベント検証（一覧にない値・無効な項目の拒否） | 0 | 1 | ✅ |
| データソース連携（DataSource・DataTextField など） | 5 | 0 | ✅ |

## 不一致の詳細

なし。

## 適用した既知の差分

なし。

## 全ケースの結果

<details><summary>① API 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlSelect.Api.Control.#ctor</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.Adapter</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.AddedControl</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.AppRelativeTemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.ApplyStyleSheetSkin</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.BeginRenderTracing</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.BindingContainer</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.BuildProfileTree</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.ChildControlsCreated</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.ClearCachedClientID</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.ClearChildControlState</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.ClearChildState</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.ClearChildViewState</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.ClearEffectiveClientIDMode</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.ClientID</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.ClientIDMode</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.ClientIDSeparator</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.Context</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.Controls</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.CreateChildControls</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.DataBind</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.DataBindChildren</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.DataBinding</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.DataItemContainer</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.DataKeysContainer</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.DesignMode</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.Dispose</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.Disposed</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.EnableTheming</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.EnableViewState</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.EndRenderTracing</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.EnsureChildControls</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.EnsureID</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.Events</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.FindControl</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.Focus</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.GetRouteUrl</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.GetUniqueIDRelativeTo</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.HasChildViewState</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.HasControls</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.HasEvents</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.ID</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.IdSeparator</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.Init</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.IsChildControlStateCleared</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.IsLiteralContent</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.IsTrackingViewState</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.IsViewStateEnabled</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.Load</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.LoadControlState</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.LoadViewState</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.LoadViewStateByID</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.MapPathSecure</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.NamingContainer</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.OnBubbleEvent</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.OnInit</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.OnLoad</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.OnPreRender</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.OnUnload</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.OpenFile</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.Page</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.Parent</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.PreRender</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.RaiseBubbleEvent</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.RemovedControl</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.Render</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.RenderChildren</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.RenderControl</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.RenderingCompatibility</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.ResolveAdapter</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.ResolveClientUrl</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.ResolveUrl</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.SaveControlState</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.SaveViewState</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.SetRenderMethodDelegate</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.SetTraceData</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.Site</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.SkinID</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.TemplateControl</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.TemplateSourceDirectory</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.TrackViewState</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.UniqueID</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.Unload</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.ValidateRequestMode</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.ViewState</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.ViewStateMode</code> | ✅ 一致 |
| <code>HtmlSelect.Api.Control.Visible</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlContainerControl.#ctor</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlContainerControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlContainerControl.InnerHtml</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlContainerControl.InnerText</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlContainerControl.LoadViewState</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlContainerControl.Render</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlContainerControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlContainerControl.RenderEndTag</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlControl.#ctor</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlControl.Attributes</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlControl.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlControl.Disabled</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlControl.GetAttribute</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlControl.Render</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlControl.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlControl.RenderBeginTag</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlControl.SetAttribute</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlControl.Style</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlControl.TagName</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlControl.ViewStateIgnoresCase</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.#ctor</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.ClearSelection</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.CreateControlCollection</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.DataMember</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.DataSource</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.DataSourceID</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.DataTextField</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.DataValueField</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.EnsureDataBound</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.GetData</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.InnerHtml</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.InnerText</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.IsBoundUsingDataSourceID</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.Items</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.LoadPostData</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.LoadViewState</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.Multiple</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.Name</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.OnDataBinding</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.OnDataPropertyChanged</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.OnDataSourceViewChanged</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.OnInit</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.OnLoad</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.OnPreRender</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.OnServerChange</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.RenderAttributes</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.RenderChildren</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.RequiresDataBinding</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.SaveViewState</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.Select</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.SelectedIndex</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.SelectedIndices</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.ServerChange</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.Size</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.TrackViewState</code> | ✅ 一致 |
| <code>HtmlSelect.Api.HtmlControls.HtmlSelect.Value</code> | ✅ 一致 |
| <code>HtmlSelect.Api.IAttributeAccessor.GetAttribute</code> | ✅ 一致 |
| <code>HtmlSelect.Api.IAttributeAccessor.SetAttribute</code> | ✅ 一致 |
| <code>HtmlSelect.Api.IControlBuilderAccessor.ControlBuilder</code> | ✅ 一致 |
| <code>HtmlSelect.Api.IControlDesignerAccessor.GetDesignModeState</code> | ✅ 一致 |
| <code>HtmlSelect.Api.IControlDesignerAccessor.SetDesignModeState</code> | ✅ 一致 |
| <code>HtmlSelect.Api.IControlDesignerAccessor.SetOwnerControl</code> | ✅ 一致 |
| <code>HtmlSelect.Api.IControlDesignerAccessor.UserData</code> | ✅ 一致 |
| <code>HtmlSelect.Api.IDataBindingsAccessor.DataBindings</code> | ✅ 一致 |
| <code>HtmlSelect.Api.IDataBindingsAccessor.HasDataBindings</code> | ✅ 一致 |
| <code>HtmlSelect.Api.IExpressionsAccessor.Expressions</code> | ✅ 一致 |
| <code>HtmlSelect.Api.IExpressionsAccessor.HasExpressions</code> | ✅ 一致 |
| <code>HtmlSelect.Api.IParserAccessor.AddParsedSubObject</code> | ✅ 一致 |
| <code>HtmlSelect.Api.IPostBackDataHandler.LoadPostData</code> | ✅ 一致 |
| <code>HtmlSelect.Api.IPostBackDataHandler.RaisePostDataChangedEvent</code> | ✅ 一致 |
| <code>HtmlSelect.Api.IUrlResolutionService.ResolveClientUrl</code> | ✅ 一致 |

</details>

<details><summary>② HTML 層</summary>

| ケース | 結果 |
|---|---|
| <code>HtmlSelect.(case list)</code> | ✅ 一致 |
| <code>HtmlSelect.(default)</code> | ✅ 一致 |
| <code>HtmlSelect.AppRelativeTemplateSourceDirectory=empty</code> | ✅ 一致 |
| <code>HtmlSelect.AppRelativeTemplateSourceDirectory=japanese</code> | ✅ 一致 |
| <code>HtmlSelect.AppRelativeTemplateSourceDirectory=markup</code> | ✅ 一致 |
| <code>HtmlSelect.AppRelativeTemplateSourceDirectory=text</code> | ✅ 一致 |
| <code>HtmlSelect.Attributes[class]=text+Style[color]=red</code> | ✅ 一致 |
| <code>HtmlSelect.Attributes[data-probe]=markup</code> | ✅ 一致 |
| <code>HtmlSelect.Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlSelect.Attributes[onfocus]=markup</code> | ✅ 一致 |
| <code>HtmlSelect.Attributes[onfocus]=text</code> | ✅ 一致 |
| <code>HtmlSelect.ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlSelect.ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlSelect.ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlSelect.ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlSelect.DataMember=empty</code> | ✅ 一致 |
| <code>HtmlSelect.DataMember=japanese</code> | ✅ 一致 |
| <code>HtmlSelect.DataMember=markup</code> | ✅ 一致 |
| <code>HtmlSelect.DataMember=text</code> | ✅ 一致 |
| <code>HtmlSelect.DataSourceID=empty</code> | ✅ 一致 |
| <code>HtmlSelect.DataSourceID=japanese</code> | ✅ 一致 |
| <code>HtmlSelect.DataSourceID=markup</code> | ✅ 一致 |
| <code>HtmlSelect.DataSourceID=text</code> | ✅ 一致 |
| <code>HtmlSelect.DataTextField=Name</code> | ✅ 一致 |
| <code>HtmlSelect.DataTextField=empty</code> | ✅ 一致 |
| <code>HtmlSelect.DataTextField=japanese</code> | ✅ 一致 |
| <code>HtmlSelect.DataTextField=markup</code> | ✅ 一致 |
| <code>HtmlSelect.DataTextField=text</code> | ✅ 一致 |
| <code>HtmlSelect.DataValueField=Code</code> | ✅ 一致 |
| <code>HtmlSelect.DataValueField=empty</code> | ✅ 一致 |
| <code>HtmlSelect.DataValueField=japanese</code> | ✅ 一致 |
| <code>HtmlSelect.DataValueField=markup</code> | ✅ 一致 |
| <code>HtmlSelect.DataValueField=text</code> | ✅ 一致 |
| <code>HtmlSelect.Disabled=false</code> | ✅ 一致 |
| <code>HtmlSelect.Disabled=true</code> | ✅ 一致 |
| <code>HtmlSelect.Disabled=true+Attributes[data-probe]=text</code> | ✅ 一致 |
| <code>HtmlSelect.EnableTheming=false</code> | ✅ 一致 |
| <code>HtmlSelect.EnableTheming=true</code> | ✅ 一致 |
| <code>HtmlSelect.EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlSelect.EnableViewState=true</code> | ✅ 一致 |
| <code>HtmlSelect.ID=empty</code> | ✅ 一致 |
| <code>HtmlSelect.ID=japanese</code> | ✅ 一致 |
| <code>HtmlSelect.ID=markup</code> | ✅ 一致 |
| <code>HtmlSelect.ID=text</code> | ✅ 一致 |
| <code>HtmlSelect.InnerHtml=empty</code> | ✅ 一致 |
| <code>HtmlSelect.InnerHtml=japanese</code> | ✅ 一致 |
| <code>HtmlSelect.InnerHtml=markup</code> | ✅ 一致 |
| <code>HtmlSelect.InnerHtml=text</code> | ✅ 一致 |
| <code>HtmlSelect.InnerText=empty</code> | ✅ 一致 |
| <code>HtmlSelect.InnerText=japanese</code> | ✅ 一致 |
| <code>HtmlSelect.InnerText=markup</code> | ✅ 一致 |
| <code>HtmlSelect.InnerText=text</code> | ✅ 一致 |
| <code>HtmlSelect.Items[1].Enabled=false</code> | ✅ 一致 |
| <code>HtmlSelect.Items[1].Enabled=true</code> | ✅ 一致 |
| <code>HtmlSelect.Items[1].Selected=false</code> | ✅ 一致 |
| <code>HtmlSelect.Items[1].Selected=true</code> | ✅ 一致 |
| <code>HtmlSelect.Items[1].Text=empty</code> | ✅ 一致 |
| <code>HtmlSelect.Items[1].Text=japanese</code> | ✅ 一致 |
| <code>HtmlSelect.Items[1].Text=markup</code> | ✅ 一致 |
| <code>HtmlSelect.Items[1].Text=markup+Items[1].Value=markup</code> | ✅ 一致 |
| <code>HtmlSelect.Items[1].Text=text</code> | ✅ 一致 |
| <code>HtmlSelect.Items[1].Value=empty</code> | ✅ 一致 |
| <code>HtmlSelect.Items[1].Value=japanese</code> | ✅ 一致 |
| <code>HtmlSelect.Items[1].Value=markup</code> | ✅ 一致 |
| <code>HtmlSelect.Items[1].Value=text</code> | ✅ 一致 |
| <code>HtmlSelect.Multiple=false</code> | ✅ 一致 |
| <code>HtmlSelect.Multiple=true</code> | ✅ 一致 |
| <code>HtmlSelect.Multiple=true+Items[0].Selected=true+Items[2].Selected=true</code> | ✅ 一致 |
| <code>HtmlSelect.Multiple=true+Size=3</code> | ✅ 一致 |
| <code>HtmlSelect.Name=empty</code> | ✅ 一致 |
| <code>HtmlSelect.Name=group</code> | ✅ 一致 |
| <code>HtmlSelect.Name=japanese</code> | ✅ 一致 |
| <code>HtmlSelect.Name=markup</code> | ✅ 一致 |
| <code>HtmlSelect.Name=text</code> | ✅ 一致 |
| <code>HtmlSelect.SelectedIndex=-1</code> | ✅ 一致 |
| <code>HtmlSelect.SelectedIndex=0</code> | ✅ 一致 |
| <code>HtmlSelect.SelectedIndex=1</code> | ✅ 一致 |
| <code>HtmlSelect.SelectedIndex=1+Disabled=true</code> | ✅ 一致 |
| <code>HtmlSelect.SelectedIndex=1+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlSelect.SelectedIndex=1+Items[1].Enabled=false</code> | ✅ 一致 |
| <code>HtmlSelect.SelectedIndex=3</code> | ✅ 一致 |
| <code>HtmlSelect.Size=-1</code> | ✅ 一致 |
| <code>HtmlSelect.Size=0</code> | ✅ 一致 |
| <code>HtmlSelect.Size=3</code> | ✅ 一致 |
| <code>HtmlSelect.SkinID=empty</code> | ✅ 一致 |
| <code>HtmlSelect.SkinID=japanese</code> | ✅ 一致 |
| <code>HtmlSelect.SkinID=markup</code> | ✅ 一致 |
| <code>HtmlSelect.SkinID=text</code> | ✅ 一致 |
| <code>HtmlSelect.Style[color]=1em</code> | ✅ 一致 |
| <code>HtmlSelect.Style[color]=red</code> | ✅ 一致 |
| <code>HtmlSelect.Style[margin-left]=1em</code> | ✅ 一致 |
| <code>HtmlSelect.Style[margin-left]=red</code> | ✅ 一致 |
| <code>HtmlSelect.ValidateRequestMode=Disabled</code> | ✅ 一致 |
| <code>HtmlSelect.ValidateRequestMode=Enabled</code> | ✅ 一致 |
| <code>HtmlSelect.ValidateRequestMode=Inherit</code> | ✅ 一致 |
| <code>HtmlSelect.Value=M</code> | ✅ 一致 |
| <code>HtmlSelect.Value=empty</code> | ✅ 一致 |
| <code>HtmlSelect.Value=japanese</code> | ✅ 一致 |
| <code>HtmlSelect.Value=markup</code> | ✅ 一致 |
| <code>HtmlSelect.Value=text</code> | ✅ 一致 |
| <code>HtmlSelect.ViewStateMode=Disabled</code> | ✅ 一致 |
| <code>HtmlSelect.ViewStateMode=Enabled</code> | ✅ 一致 |
| <code>HtmlSelect.ViewStateMode=Inherit</code> | ✅ 一致 |
| <code>HtmlSelect.Visible=false</code> | ✅ 一致 |
| <code>HtmlSelect.Visible=false+EnableViewState=false</code> | ✅ 一致 |
| <code>HtmlSelect.Visible=true</code> | ✅ 一致 |
| <code>HtmlSelect[data-bound].(default)</code> | ✅ 一致 |
| <code>HtmlSelect[data-bound].DataTextField=Name</code> | ✅ 一致 |
| <code>HtmlSelect[data-bound].DataTextField=Name+DataValueField=Code</code> | ✅ 一致 |
| <code>HtmlSelect[data-bound].DataTextField=Name+DataValueField=Code+Value=M</code> | ✅ 一致 |
| <code>HtmlSelect[data-bound].DataValueField=Code</code> | ✅ 一致 |
| <code>HtmlSelect[naming container].(default)</code> | ✅ 一致 |
| <code>HtmlSelect[naming container].ClientIDMode=AutoID</code> | ✅ 一致 |
| <code>HtmlSelect[naming container].ClientIDMode=Inherit</code> | ✅ 一致 |
| <code>HtmlSelect[naming container].ClientIDMode=Predictable</code> | ✅ 一致 |
| <code>HtmlSelect[naming container].ClientIDMode=Static</code> | ✅ 一致 |
| <code>HtmlSelect[naming container].ID=text</code> | ✅ 一致 |

</details>

<details><summary>③ シナリオ層</summary>

| スナップショット | 結果 |
|---|---|
| HtmlSelect [render][databind][dynamic] first request: plain, bound, set in code, made in code | ✅ 一致 |
| HtmlSelect [viewstate][dynamic][postdata][change] every input posted by a submit input | ✅ 一致 |
| HtmlSelect [change][viewstate] posted again unchanged: no change event | ✅ 一致 |
| HtmlSelect [visible][viewstate] hidden ones shown with what was set while hidden | ✅ 一致 |
| HtmlSelect [validation] validated on the server (g1): empty, not chosen | ✅ 一致 |
| HtmlSelect [clientvalidation] validated in the browser (g2): no request | ✅ 一致 |
| HtmlSelect [eventvalidation] a value not in the list posted: rejected | ✅ 一致 |

</details>
